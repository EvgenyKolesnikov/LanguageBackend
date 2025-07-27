using Language.Database;
using Language.Dictionary;
using Language.Dictionary.Requests;
using Language.Dictionary.Responses;
using Language.Model;
using Language.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Language.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    public readonly JwtOptions _options;
    public readonly MainDbContext _dbContext;
    public readonly DictionaryService _dictionaryService;
    public readonly TextService _textService;
    
    public AdminController(IOptions<JwtOptions> options, MainDbContext dbcontext, DictionaryService dictionaryService, TextService textService)
    {
        _options = options.Value;
        _dbContext = dbcontext;
        _dictionaryService = dictionaryService;
        _textService = textService;
    }


    /// <summary>
    /// Добавить слово в общий словарь
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Dictionary")]
    public async Task<IActionResult> AddWordInDictionary(AddWordRequest request)
    {
        try
        {
            await _dictionaryService.AddWordInBaseDictionary(request.Word, request.Translation);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        

        return Ok();
    }

    
    /// <summary>
    /// Получить все слова из словаря
    /// </summary>
    /// <returns></returns>
    [HttpGet("Dictionary")]
    public async Task<IActionResult> GetDictionary()
    {
        var response = await _dbContext.BaseWords.ToListAsync();
        return Ok(response);
    }

    
    
    
    /// <summary>
    /// Получить слова из расширенного словаря 
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetExtentedWord")]
    public async Task<IActionResult> GetExtentedWord(string baseWord)
    {
        var response = await _dbContext.ExtentedWords.Include(i => i.BaseWord)
            .Where(i => i.BaseWord.Word == baseWord.ToLower()).Select(i => new GetExtentedWords(i)).ToListAsync();
        return Ok(response);
    }
    
    /// <summary>
    /// Удалить слово из расширенного словаря
    /// </summary>
    /// <param name="wordId"></param>
    /// <returns></returns>
    [HttpDelete("ExtentedWord/{id}")]
    public async Task<IActionResult> DeleteExtentedWord(int id)
    {
        var extentedWord = await _dbContext.ExtentedWords.FirstOrDefaultAsync(i => i.Id == id);
        if (extentedWord == null)
        {
            return NotFound();
        }
        
        _dbContext.ExtentedWords.Remove(extentedWord);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Добавить слово в расширенный словарь
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("ExtentedWord")]
    public async Task<IActionResult> AddExtentedWord(AddExtentedWord request)
    {
        var extentedWord = _dbContext.ExtentedWords.FirstOrDefault(i => i.Word == request.Word.ToLower());
        if (extentedWord != null) return BadRequest("Word already exists");
        
        var entity = new ExtentedWord()
        {
            BaseWordId = request.BaseIdWord,
            Word = request.Word,
        };
        
        var addedEntity = await _dbContext.ExtentedWords.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        
        var response = new GetExtentedWords(addedEntity.Entity);
        return Ok(response);
    }

    /// <summary>
    /// Редактировать слово из расширенного словаря
    /// </summary>
    /// <param name="id"></param>
    /// <param name="extentedWord"></param>
    /// <returns></returns>
    [HttpPut("ExtentedWord/{id}")]
    public async Task<IActionResult> EditExtentedWord(int id, [FromBody] EditExtentedWordRequest extentedWord)
    {
        var wordToUpdate = await _dbContext.ExtentedWords.FindAsync(id);
        if (wordToUpdate == null) return NotFound();
        
        wordToUpdate.Word = extentedWord.Word;
        wordToUpdate.BaseWordId = extentedWord.BaseWordId;
        
        await _dbContext.SaveChangesAsync();
        
        var response = new GetExtentedWords(wordToUpdate);
        return Ok(response);
    }
    
  
    
    /// <summary>
    /// Добавить текст
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [HttpPost("Text")]
    public async Task<IActionResult> AddText(string text)
    {
        await _textService.AddText(text);
        return Ok();
    }

    /// <summary>
    /// Получить текста
    /// </summary>
    /// <returns></returns>
    [HttpGet("Text")]
    public async Task<IActionResult> GetTexts()
    {
        var response = await _textService.GetTexts();
        return Ok(response);
    }

    /// <summary>
    /// Получить слова по тексту
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Text/{id}")]
    public async Task<IActionResult> GetWordsByText(int id)
    {
        var response = await _textService.GetWordsByText(id);
        return Ok(response);
    }
}