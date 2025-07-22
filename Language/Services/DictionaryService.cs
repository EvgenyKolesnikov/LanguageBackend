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


    public async Task<BaseWord?> AddWordToUser(string word, User user)
    {
        BaseWord? response = null;
        var extentedWord = await _dbContext.ExtentedWords
            .Include(i => i.BaseWord)
            .ThenInclude(i => i.Users)
            .FirstOrDefaultAsync(i => i.Word == word.ToLower());
        if (extentedWord != null)
        {
            extentedWord.BaseWord.Users.Add(user);
            response = extentedWord.BaseWord;
        }
        else
        {
            var wordDb = await _dbContext.BaseWords.Include(i => i.Users)
                .FirstOrDefaultAsync(i => i.Word == word.ToLower());

            if (wordDb != null)
            {
                wordDb.Users.Add(user);    
                response = wordDb;
                
            }
        }
        
        await _dbContext.SaveChangesAsync();
        return response;
    }

    public async Task AddWordInBaseDictionary(string word, string translation)
    {
        var entity = new BaseWord()
        {
            Word = word.ToLower(),
            Translation = translation.ToLower()
        };

        if (await _dbContext.BaseWords.AnyAsync(i => i.Word == word.ToLower()))
        {
            throw new Exception("Word already exists");
        }

        await _dbContext.BaseWords.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddWordInExtentedDictionary(string word, string baseWord)
    {
        var baseWordDb = await _dbContext.BaseWords.FirstOrDefaultAsync(i => i.Word == baseWord.ToLower());
        if (baseWordDb == null) return;
        
        var entity = new ExtentedWord()
        {
            Word = word.ToLower(),
            BaseWord = baseWordDb
        };

        if (await _dbContext.ExtentedWords.AnyAsync(i => i.Word == word.ToLower()))
        {
            throw new Exception("Word already exists");
        }

        await _dbContext.ExtentedWords.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<GetWordsByUserResponse>> GetWordsByUser(Guid userId)
    {
        var response = await _dbContext.Users.Where(i => i.Id == userId)
            .Select(i => new GetWordsByUserResponse(i.Dictionary)).ToListAsync();
        
        return response;
    }
}