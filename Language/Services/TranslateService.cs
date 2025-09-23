using Language.Database;
using Language.DTO;
using Language.Translate;
using Microsoft.EntityFrameworkCore;

namespace Language.Services;

public class TranslateService
{
     public readonly MainDbContext _dbContext;
     public Dictionary<string, TranslateDto?> _words = new();


     public TranslateService(MainDbContext dbcontext)
     {
         _dbContext = dbcontext;
         
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
                var response = new TranslateResponse()
                {
                    Text = value.Word,
                    Alias = value.Word == value.BaseWord ? value.Word : value.Word + $" ({value.BaseWord})",
                    Translation = value.Translation, 
                };
                return response;
            }
        }
        
        return new TranslateResponse();
    }
}