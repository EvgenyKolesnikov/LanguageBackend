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
        _words = await _dbContext.Words.ToDictionaryAsync(i => i.WordText, i => new TranslateDto(){Translation = i.Translation, BaseWord = i.WordText, Word = i.WordText});
        
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

    public async Task<WordDto> TranslateWord(string text)
    {
        var word = await _dbContext.Words.FirstOrDefaultAsync(i => i.WordText == text);
        
        if (word?.Translation != null)
            return new WordDto(word);
        
        var translate = await _externalTranslateService.TranslateYandex(text);

        if (translate.translations.FirstOrDefault() == null)
            return new WordDto();
        
        
        if (word != null)
        {
            word.Translation = translate.translations.FirstOrDefault()?.text;
            if (word.Properties.FirstOrDefault(i => i.Translation == translate.translations.FirstOrDefault()?.text)  == null)
            {
                word.Properties.Add(new WordProperties(){Translation = word.Translation});
            }
          
            await _dbContext.SaveChangesAsync();    
            return new WordDto(word);
        }
        
        
        var translation = translate.translations.FirstOrDefault()?.text;
        if (translation != null)
        {
            var baseword = await _dictionaryService.AddWordInBaseDictionary(text, translation);
            return new WordDto(baseword);
        }
        
        return new WordDto();
    }
}