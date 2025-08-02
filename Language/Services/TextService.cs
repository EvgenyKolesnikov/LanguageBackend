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
        var response = await _dbContext.Texts.Include(i => i.Dictionary).Select(i => new TextDto(i)).ToHashSetAsync();
        return response;
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




    

    public async Task<ICollection<BaseWord>> Process(string text)
    {
        var words = text.SplitToWords();
        
        var baseWords = await _dbContext.BaseWords.Include(i => i.ExtentedWords).ToHashSetAsync();
        var dict = new List<Model.BaseWord>();
        
        foreach (var word in words)
        {
            var wordDb = baseWords.FirstOrDefault(
                i => i.ExtentedWords != null && (i.Word.ToLower() == word.ToLower() ||
                i.ExtentedWords.Any(e => e.Word.ToLower() == word.ToLower())));
            
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
                dict.Add(item);
                
                await _dbContext.BaseWords.AddAsync(item);
            }
        }
        
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