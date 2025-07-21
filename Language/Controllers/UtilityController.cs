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

    public UtilityController(MainDbContext dbcontext, DictionaryService dictionaryService, AuthService authService)
    {
        _dbContext = dbcontext;
        _dictionaryService = dictionaryService;
        _authService = authService;
    }


    
    [HttpGet("FillData")]
    public async Task<ActionResult> FillData()
    {
        var userJenya = await _authService.RegisterUser(new RegisterUser()
            { Email = "Jenya@mail.ru", Name = "Jenya", Password = "1234" });
        var userVasya = await _authService.RegisterUser(new RegisterUser()
            { Email = "Vasya@mail.ru", Name = "Vasya", Password = "1111" });
        
        
        
        await _dictionaryService.AddWordInDictionary("dog", "собака");
        await _dictionaryService.AddWordInDictionary("table", "стол");
        
        await _dictionaryService.AddWordToUser("dog", userJenya);
        await _dictionaryService.AddWordToUser("table", userJenya);
        await _dictionaryService.AddWordToUser("dog", userVasya);
        
        return Ok();   
    }


    [HttpGet("Migrate")]
    public async Task MigrateData()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

}