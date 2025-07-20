using Language.Database;
using Language.Dictionary;
using Language.Model;
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
    
    
    public AdminController(IOptions<JwtOptions> options,MainDbContext dbcontext)
    {
        _options = options.Value;
        _dbContext = dbcontext;
    }


    [HttpPost("Dictionary")]
    public async Task<IActionResult> AddWordInDictionary(AddWordRequest request)
    {
        var entity = new Model.Dictionary()
        {
            Word = request.Word.ToLower(),
            Translation = request.Translation.ToLower()
        };
        
        await _dbContext.Dictionary.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("Dictionary")]
    public async Task<IActionResult> GetDictionary()
    {
        var response = await _dbContext.Dictionary.ToListAsync();
        return Ok(response);
    }
}