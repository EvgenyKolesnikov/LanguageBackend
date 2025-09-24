using Language.Database;
using Language.Dictionary.Responses;
using Language.DTO;
using Language.Model;
using Language.Translate;
using Microsoft.EntityFrameworkCore;

namespace Language.Services;

public class TranslateService
{
     private readonly MainDbContext _dbContext;
     private readonly ExternalTranslateService _externalTranslateService;
     private readonly DictionaryService _dictionaryService;
     public Dictionary<string, TranslateDto?> _words = new();
     
     public TranslateService(MainDbContext dbcontext, ExternalTranslateService externalTranslateService, DictionaryService dictionaryService)
     {
         _dbContext = dbcontext;
         _externalTranslateService = externalTranslateService;
         _dictionaryService = dictionaryService;
     }
     
    
    public async Task<TranslateResponse> Translate(TranslateRequest request)
    {
        _words = await _dbContext.BaseWords.ToDictionaryAsync(i => i.Word, i => new TranslateDto(){Translation = i.Translation, BaseWord = i.Word, Word = i.Word});
        var extendedWords = await _dbContext.ExtentedWords.ToDictionaryAsync(i => i.Word, i => new TranslateDto(){Translation = i.Translation, BaseWord = i.BaseWord.Word, Word = i.Word});
        
        _words = new Dictionary<string, TranslateDto?>(_words.Union(extendedWords));
        List<string> candidates = new List<string>();
        
        if (string.IsNullOrWhiteSpace(request.ClickedWord))
            throw new Exception("Word not found");
        
        
        candidates.Add($"{request.ClickedWord} {request.NextWord}");
        candidates.Add($"{request.PreviousWord} {request.ClickedWord}");
        candidates.Add($"{request.ClickedWord}");


        foreach (var candidate in candidates)
        {
            if (_words.TryGetValue(candidate.ToLower(), out var value))
            {
                if (value.Translation == null)
                    continue;
                
                var response = new TranslateResponse()
                {
                    Text = value.Word,
                    Alias = value.Word == value.BaseWord ? value.Word : value.Word + $" ({value.BaseWord})",
                    Translation = value.Translation, 
                };
                return response;
            }
        }

        var translate = await TranslateWord(request.ClickedWord);

        if (translate != null)
        {
            var response = new TranslateResponse()
            {
                Text = request.ClickedWord,
                Alias = request.ClickedWord,
                Translation = translate.Translation,
            };
            return response;
        }
        
        
        return new TranslateResponse();
    }

    public async Task<BaseWordDto> TranslateWord(string text)
    {
        var word = await _dbContext.BaseWords.FirstOrDefaultAsync(i => i.Word == text);
        
        if (word?.Translation != null)
            return new BaseWordDto(word);
        
        var translate = await _externalTranslateService.TranslateYandex(text);

        if (translate.translations.FirstOrDefault() == null)
            return new BaseWordDto();
        
        
        if (word != null)
        {
            word.Translation = translate.translations.FirstOrDefault()?.text;
            if (word.Properties.FirstOrDefault(i => i.Translation == translate.translations.FirstOrDefault()?.text)  == null)
            {
                word.Properties.Add(new WordProperties(){Translation = word.Translation});
            }
          
            await _dbContext.SaveChangesAsync();    
            return new BaseWordDto(word);
        }
        
        
        var translation = translate.translations.FirstOrDefault()?.text;
        if (translation != null)
        {
            var baseword = await _dictionaryService.AddWordInBaseDictionary(text, translation);
            return new BaseWordDto(baseword);
        }
        
        return new BaseWordDto();
    }
}