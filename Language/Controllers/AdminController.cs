using Language.Database;
using Language.Dictionary;
using Language.Dictionary.Requests;
using Language.Dictionary.Responses;
using Language.Dictionary.Responses.Translate;
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
    public readonly ExternalTranslateService ExternalTranslateService;
    
    public AdminController(IOptions<JwtOptions> options, MainDbContext dbcontext, DictionaryService dictionaryService, TextService textService, ExternalTranslateService externalTranslateService)
    {
        _options = options.Value;
        _dbContext = dbcontext;
        _dictionaryService = dictionaryService;
        _textService = textService;
        ExternalTranslateService = externalTranslateService;
    }


    /// <summary>
    /// Добавить слово в общий словарь
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Dictionary")]
    public async Task<IActionResult> AddWordInDictionary(AddWordProperty request)
    {
        try
        {
            var response = await _dictionaryService.AddWordInBaseDictionary(request.WordText, request.Translation);
            return Ok(new WordDto(response));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Редактировать слово словаря
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("Dictionary/{id}")]
    public async Task<IActionResult> EditDictionary(int id, EditWordRequest request)
    {
        var wordToUpdate = await _dbContext.Words.Include(i => i.ChildrenWords).FirstOrDefaultAsync(i => i.Id == id);
        if (wordToUpdate == null) return NotFound();
        
        wordToUpdate.WordText = request.WordText;
        wordToUpdate.Translation = request.Translation;

        if (wordToUpdate.Id != wordToUpdate.ParentWordId)
        {
            if (wordToUpdate!.ChildrenWords.Any())
                return BadRequest("ParentWord не может содержать другой ParentWord");
            wordToUpdate.ParentWordId  = request.ParentWordId == 0 ? null : request.ParentWordId;
        }
        
        
        await _dbContext.SaveChangesAsync();
        return Ok(wordToUpdate);
    }
    
    /// <summary>
    /// Получить все слова из словаря
    /// </summary>
    /// <returns></returns>
    [HttpGet("Dictionary")]
    public async Task<IActionResult> GetDictionary(bool includeChildren = false)
    {
        List<Word> baseWords = new List<Word>();
        if (includeChildren)
        { 
            baseWords = await _dbContext.Words.Include(i => i.Properties).ToListAsync();
        }
        else
        { 
            baseWords = await _dbContext.Words.Include(i => i.Properties)
                .Include(i => i.ChildrenWords)
                .Where(i => i.ParentWordId == null).ToListAsync();
        }
        

        var response = baseWords.Select(i => new WordDto(i));
        return Ok(response);
    }

    
    /// <summary>
    /// Получить слово
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Dictionary/{id}")]
    public async Task<IActionResult> GetWord(int id)
    {
        var word = await _dbContext.Words
            .Include(i => i.ChildrenWords)
            .Include(i => i.Properties).FirstOrDefaultAsync(i => i.Id == id);
        var response = new WordDto(word);
        return Ok(response);
    }

    /// <summary>
    /// Удалить слово из словаря
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("Dictionary/{id}")]
    public async Task<IActionResult> DeleteBaseWord(int id)
    {
        var baseWord = await _dbContext.Words.Include(i => i.ChildrenWords).FirstOrDefaultAsync(i => i.Id == id);
        if (baseWord == null)
        {
            return NotFound();
        }
        
        _dbContext.Words.Remove(baseWord);
        _dbContext.Words.RemoveRange(baseWord.ChildrenWords);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    
    
  
    
    /// <summary>
    /// Добавить текст
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [HttpPost("Text")]
    public async Task<IActionResult> AddText([FromBody] string text)
    {
        var response = await _textService.AddText(text);
        return Ok(response);
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

    /// <summary>
    /// Изменить текст
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPut("Text/{id}")]
    public async Task<IActionResult> EditText(int id, EditTextRequest request)
    {
        var textToUpdate = await _dbContext.Texts.FindAsync(id);
        if (textToUpdate == null) return NotFound();
        
        textToUpdate.Content = request.Content;
        
        await _dbContext.SaveChangesAsync();
        
        return Ok(textToUpdate);
    }

    /// <summary>
    /// Удалить текст
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("Text/{id}")]
    public async Task<IActionResult> DeleteText(int id)
    {
        try
        {
            await _textService.DeleteText(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    /// <summary>
    /// Отвязать слово от текста
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("Text/{textId}/Word/{wordId}")]
    public async Task<IActionResult> DeleteBaseWordFromText(int textId, int wordId)
    {
        try
        {
            await _textService.UnattachWordFromText(textId, wordId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("Text/Process/{id}")]
    public async Task<IActionResult> ProcessText(int id)
    {
        var response = await _textService.ProcessText(id);
        return Ok(response);
    }


    /// <summary>
    /// Translate
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("Word/Translate/{id}")]
    public async Task<IActionResult> TranslateWord(int id)
    {
        var word = await _dbContext.Words.FirstOrDefaultAsync(i => i.Id == id);
        
        var translate = await ExternalTranslateService.TranslateYandex(word.WordText);
        
        if (word.Properties.FirstOrDefault(i => i.Translation == translate.translations.FirstOrDefault()?.text) != null)
            return Ok(new WordDto(word));
        
        
        word.Translation = translate.translations.FirstOrDefault()?.text;
        word.Properties.Add(new WordProperties(){Translation = word.Translation});
        await _dbContext.SaveChangesAsync();
        var response = new WordDto(word);
        return Ok(response);
    }
    
    /// <summary>
    /// Add WordProperty
    /// </summary>
    /// <returns></returns>
    [HttpPost("WordProperty")]
    public async Task<IActionResult> AddWordProperty(AddWordProperty request)
    {
        var word = await _dbContext.Words.FirstOrDefaultAsync(i => i.Id == request.WordId);
        
        if (word == null) return NotFound("Word not found");
        
        var entity = new WordProperties()
        {
            Translation = request.Translation,
            PartOfSpeech = request.PartOfSpeech,
            Word = word
        };
        
        await _dbContext.WordProperties.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        
        var response = new WordPropertiesDto(entity);
        return Ok(response) ;
    }
    
    /// <summary>
    /// Add WordProperty
    /// </summary>
    /// <returns></returns>
    [HttpPut("WordProperty")]
    public async Task<IActionResult> EditWordProperty(EditPropertyWordRequest request)
    {
        var word = await _dbContext.Words.Include(i => i.Properties).FirstOrDefaultAsync(i => i.Id == request.BaseWordId);
        
        if (word == null) return NotFound("Word not found");
        
        var property = word.Properties.FirstOrDefault(i => i.Id == request.PropertyWordId);
        
        if (property == null) return NotFound("Property not found");
        
        property.Translation = request.Translation;
        await _dbContext.SaveChangesAsync();
        
        var response = new WordPropertiesDto(property);
        return Ok(response) ;
    }

    /// <summary>
    /// Delete Word Property
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("DeleteWordProperty/{id}")]
    public async Task<IActionResult> DeleteWordProperty(int id)
    {
        var proprety = await _dbContext.WordProperties.FirstOrDefaultAsync(i => i.Id == id);
        if (proprety == null) return NotFound();
        
        
        _dbContext.WordProperties.Remove(proprety);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }


    [HttpGet("BookNames")]
    public async Task<IActionResult> GetBookNames()
    {
        var response = await _textService.GetBookNamesAsync();
        
        return Ok(response);
    }
    
    [HttpGet("Books")]
    public async Task<IActionResult> GetAllBooks()
    {
        var response = await _textService.GetBooksAsync();
        return Ok(response);
    }

    [HttpGet("Book/{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var response = await _textService.GetBookAsync(id);
        return Ok(response);
    }
}