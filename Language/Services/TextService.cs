using Language.Database;
using Language.Dictionary.Responses;
using Language.JwtExtensions;
using Language.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Language.Services;

public class TextService
{
    public readonly MainDbContext _dbContext;
    
    
    public TextService(MainDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }
    
    
    public async Task<TextDto> AddText(string text)
    {
        var response = await ProcessNewText(text);
        return new TextDto(response);
    }

    public async Task<TextDto> ProcessText(int id)
    {
        var response = await ProcessExistText(id);
        return new TextDto(response);
    }

    public async Task<ICollection<TextDto>> GetTexts()
    {
        var response = await _dbContext.Texts.Include(i => i.Dictionary)
            .ThenInclude(i => i.Properties).Select(i => new TextDto(i)).ToHashSetAsync();
        
        // Перемешиваем тексты в случайном порядке
        var random = new Random();
        return response.OrderBy(x => random.Next()).ToList();
    }

    public async Task<GetWordsByTextResponse> GetWordsByText(int textId)
    {
        var text = await _dbContext.Texts.Include(i => i.Dictionary).FirstOrDefaultAsync(i => i.Id == textId);
        var response = new GetWordsByTextResponse(text);
        return response;
    }

    public async Task DeleteText(int id)
    {
        var text = await _dbContext.Texts.FirstOrDefaultAsync(i => i.Id == id);
        if (text == null)
        {
            throw new Exception("Объект не существует");
        }
        
        _dbContext.Texts.Remove(text);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TextDto> UnattachWordFromText(int textId, int wordId)
    {
        var text = await _dbContext.Texts.Include(i => i.Dictionary).FirstOrDefaultAsync(i => i.Id == textId);

        if (text == null)
            throw new Exception("Text not found");
        
        var word = text.Dictionary.FirstOrDefault(i => i.Id == wordId);
        if (word != null)
        {
            text.Dictionary.Remove(word);
            await _dbContext.SaveChangesAsync();
        }
        
        return new TextDto(text);
    }




    

    public async Task<ICollection<Word>> Process(string text)
    {
        var words = text.SplitToWords();
        
        var baseWords = await _dbContext.Words.ToHashSetAsync();
        var dict = new List<Word>();
        var dictToAdd = new List<Word>();
        
        foreach (var word in words)
        {
            var wordDb = baseWords.FirstOrDefault(i => i.WordText != null && i.WordText.ToLower() == word.ToLower());
            
            if (wordDb != null)
            {
                dict.Add(wordDb);
            }
            else
            {
                var item = new Model.Word()
                {
                    WordText = word.ToLower()
                };
                dictToAdd.Add(item);
                
               
            }
        }
        
        dictToAdd = dictToAdd.DistinctBy(i => i.WordText, StringComparer.OrdinalIgnoreCase).ToList();
        dict = dict.Union(dictToAdd).DistinctBy(i => i.WordText).ToList();
        
        await _dbContext.Words.AddRangeAsync(dictToAdd);
        await _dbContext.SaveChangesAsync();
        return dict;
    }
    
    
    public async Task<Text> ProcessNewText(string text)
    {
        var dictionary = await Process(text);

        var entity = new Model.Text()
        {
            Content = text,
            Dictionary = dictionary,
            WordsCount = dictionary.Count
        };

        var response = await _dbContext.Texts.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        
        return response.Entity;
    }
    public async Task<Text> ProcessExistText(int textId)
    {
        var text = await _dbContext.Texts.Include(text => text.Dictionary).FirstOrDefaultAsync(i => i.Id == textId);
        if (text != null)
        {
            var dictionary = await Process(text.Content);
            text.Dictionary = dictionary;
            await _dbContext.SaveChangesAsync();
        }
        return text;
    }
}