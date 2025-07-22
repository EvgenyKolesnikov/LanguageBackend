using Language.Database;

namespace Language.Services;

public class TextService
{
    public readonly MainDbContext _dbContext;
    
    
    public TextService(MainDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }
    
    
    public async Task AddText(string text)
    {
        var words = text.Split(" ");
        
        var dictionary = _dbContext.BaseWords.ToHashSet();
        var dict = new List<Model.BaseWord>();
        foreach (var word in words)
        {
            var wordDb = dictionary.FirstOrDefault(i => i.Word == word.ToLower());
            if (wordDb != null)
            {
                dict.Add(wordDb);
            }
            else
            {
                var item = new Model.BaseWord()
                {
                    Word = word.ToLower()
                };
                
                await _dbContext.BaseWords.AddAsync(item);
            }
        }

        var entity = new Model.Text()
        {
            Content = text,
            Dictionary = dict
        };
        
        await _dbContext.Texts.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }
}