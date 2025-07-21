using Language.Database;
using Language.Dictionary;
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
    
    public AdminController(IOptions<JwtOptions> options, MainDbContext dbcontext, DictionaryService dictionaryService)
    {
        _options = options.Value;
        _dbContext = dbcontext;
        _dictionaryService = dictionaryService;
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
            await _dictionaryService.AddWordInDictionary(request.Word, request.Translation);
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
        var response = await _dbContext.Dictionary.ToListAsync();
        return Ok(response);
    }
}