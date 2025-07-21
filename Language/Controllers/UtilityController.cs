using Language.Database;
using Language.Model;
using Language.Requests;
using Language.Services;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers;

public class UtilityController: ControllerBase
{
    public readonly MainDbContext _dbContext;
    public readonly DictionaryService _dictionaryService;
    public readonly AuthService _authService;
    public readonly TextService _textService;

    public UtilityController(MainDbContext dbcontext, DictionaryService dictionaryService, AuthService authService, TextService textService)
    {
        _dbContext = dbcontext;
        _dictionaryService = dictionaryService;
        _authService = authService;
        _textService = textService;
    }


    
    [HttpGet("FillData")]
    public async Task<ActionResult> FillData()
    {
        _dbContext.ClearAll();
        var userJenya = await _authService.RegisterUser(new RegisterUser()
            { Email = "Jenya@mail.ru", Name = "Jenya", Password = "1234" });
        var userVasya = await _authService.RegisterUser(new RegisterUser()
            { Email = "Vasya@mail.ru", Name = "Vasya", Password = "1111" });
        
        
        
        await _dictionaryService.AddWordInDictionary("dog", "собака");
        await _dictionaryService.AddWordInDictionary("table", "стол");
        
        await _dictionaryService.AddWordToUser("dog", userJenya);
        await _dictionaryService.AddWordToUser("table", userJenya);
        await _dictionaryService.AddWordToUser("dog", userVasya);

        await _textService.AddText("Dog on the table");
        
        return Ok();   
    }


    [HttpGet("Migrate")]
    public async Task MigrateData()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

}