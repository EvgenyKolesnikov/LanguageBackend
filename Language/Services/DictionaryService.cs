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

    public async Task<BaseWord> AddWordInBaseDictionary(string word, string translation, string? partOfSpeech = null)
    {
        var entity = new BaseWord()
        {
            Word = word.ToLower(),
            Translation = translation,
            Properties = new List<WordProperties>()
            {
              new () {Translation = translation.ToLower(), PartOfSpeech = partOfSpeech}  
            }
        };

        
        var wordDb = await _dbContext.BaseWords.FirstOrDefaultAsync(i => i.Word == word.ToLower());
        {
            if (wordDb != null && 
                wordDb.Properties.FirstOrDefault(i => i.Translation.ToLower() == translation.ToLower() && i.PartOfSpeech == partOfSpeech) == null)
            {
                throw new Exception("Слово уже есть в словаре");
            }
        }
       

        var response = await _dbContext.BaseWords.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return response.Entity;
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

    public async Task DeleteWordByUser(Guid userId, int wordId)
    {
        // Используем FindAsync для поиска по первичному ключу (более эффективно)
        var word = await _dbContext.BaseWords.FindAsync(wordId);
        if (word == null)
        {
            throw new KeyNotFoundException($"Word with id {wordId} not found");
        }

        // Загружаем пользователя с его словарем, но только нужные данные
        var user = await _dbContext.Users
            .Include(u => u.Dictionary)
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                User = u,
                Dictionary = u.Dictionary.Where(d => d.Id == wordId).FirstOrDefault()
            })
            .FirstOrDefaultAsync();
        if (user == null)
            throw new KeyNotFoundException($"User with id {userId} not found");

        if (user.Dictionary == null)
            throw new InvalidOperationException($"Word with id {wordId} not found in user's dictionary");
        
        
        user.User.Dictionary.Remove(word);
        await _dbContext.SaveChangesAsync();
    }
}