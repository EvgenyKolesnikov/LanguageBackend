using Language.Database;
using Language.Translate;

namespace Language.Services;

public class TranslateService
{
    
     public readonly MainDbContext _dbContext;
     public Dictionary<string,string> _words = new();


     public TranslateService(MainDbContext dbcontext)
     {
         _dbContext = dbcontext;
         
     }
     
    
    public async Task<string> Translate(TranslateRequest request)
    {
        _words = _dbContext.BaseWords.ToDictionary(i => i.Word, i => i.Translation);
        
        
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
                return value;
            }
        }
        
        return "";
    }
}