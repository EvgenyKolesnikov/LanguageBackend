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


    public async Task<Word?> AddWordToUser(string word, User user)
    {
        Word? response = null;
        var wordDb = await _dbContext.Words
            .Include(i => i.Users)
            .FirstOrDefaultAsync(i => i.WordText == word.ToLower());
        
        if (wordDb != null)
        {
            wordDb.Users.Add(user);
            response = wordDb;
        }
        
        await _dbContext.SaveChangesAsync();
        return response;
    }

    public async Task<Word> AddWordInBaseDictionary(string word, string translation, string? partOfSpeech = null, int? parentWordId = null)
    {
        var entity = new Word()
        {
            WordText = word.ToLower(),
            Translation = translation,
            ParentWordId = parentWordId,
            Properties = new List<WordProperties>()
            {
              new () { Translation = translation.ToLower(), PartOfSpeech = partOfSpeech }  
            }
        };

        
        var wordDb = await _dbContext.Words.Include(baseWord => baseWord.Properties).FirstOrDefaultAsync(i => i.WordText == word.ToLower());

        if (wordDb != null)
        {
            var isExist = wordDb.Properties.FirstOrDefault(i =>
                i.Translation.ToLower() == translation.ToLower() && i.PartOfSpeech == partOfSpeech);

            if (isExist == null)
            {
                 wordDb.Properties.Add(new WordProperties()
                    { Translation = translation.ToLower(), PartOfSpeech = partOfSpeech });
                await _dbContext.SaveChangesAsync();
                return wordDb;
            }
            else
                throw new Exception("Слово уже есть в словаре");
        }
        

        var response = await _dbContext.Words.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return response.Entity;
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
        var word = await _dbContext.Words.FindAsync(wordId);
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