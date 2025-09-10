using Language.Database;
using Language.Services;
using Language.Translate;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers;


[Route("api/[controller]")]
[ApiController]
public class TranslateController : ControllerBase
{
    public readonly TranslateService _translateService;
    
    
    public TranslateController(TranslateService translateService)
    {
        _translateService = translateService;
    }
    
    /// <summary>
    /// Перевести слово/фразу
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Translate")]
    public async Task<IActionResult> Translate([FromBody] TranslateRequest request)
    {
        var response = await _translateService.Translate(request);
        return Ok(response);
    }
}