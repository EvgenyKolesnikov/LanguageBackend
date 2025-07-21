using Language.Database;
using Language.Dictionary;
using Language.Model;
using Microsoft.EntityFrameworkCore;

namespace Language.Services;

public class DictionaryService
{
    public readonly MainDbContext _dbContext;

    public DictionaryService(MainDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }


    public async Task<Model.Dictionary?> AddWordToUser(string word, User user)
    {
        var wordDb = await _dbContext.Dictionary.Include(i => i.Users)
            .FirstOrDefaultAsync(i => i.Word == word.ToLower());

        if (wordDb != null)
        {
            wordDb.Users.Add(user);    
            await _dbContext.SaveChangesAsync();
        }
        return wordDb;
    }

    public async Task AddWordInDictionary(string word, string translation)
    {
        var entity = new Model.Dictionary()
        {
            Word = word.ToLower(),
            Translation = translation.ToLower()
        };

        if (await _dbContext.Dictionary.AnyAsync(i => i.Word == word.ToLower()))
        {
            throw new Exception("Word already exists");
        }

        await _dbContext.Dictionary.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<GetWordsByUserResponse>> GetWordsByUser(Guid userId)
    {
        var response = await _dbContext.Users.Where(i => i.Id == userId)
            .Select(i => new GetWordsByUserResponse(i.Dictionary)).ToListAsync();
        
        return response;
    }
}