using Language.Database;
using Language.Dictionary.Responses;
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
                dict.Add(item);
                await _dbContext.BaseWords.AddAsync(item);
            }
        }

        var entity = new Model.Text()
        {
            Content = text,
            Dictionary = dict
        };
        
        var response = await _dbContext.Texts.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return new TextDto(response.Entity);
    }

    public async Task<ICollection<Text>> GetTexts()
    {
        var response = await _dbContext.Texts.ToHashSetAsync();
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
}