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
        
        var word1 = await _dictionaryService.AddWordInBaseDictionary("dog", "собака");
        await _dictionaryService.AddWordInBaseDictionary("dogs", "собаки", parentWordId: word1.Id);
        
        await _dictionaryService.AddWordInBaseDictionary("like", "нравится", "verb");
        await _dictionaryService.AddWordInBaseDictionary("like", "как", "preposition");
        
        
        await _dictionaryService.AddWordInBaseDictionary("table", "стол");
        await _dictionaryService.AddWordInBaseDictionary("take", "взять");
        await _dictionaryService.AddWordInBaseDictionary("take off", "снять");
        
        
        
        await _dictionaryService.AddWordToUser("dogs", userJenya);
        await _dictionaryService.AddWordToUser("table", userJenya);
        await _dictionaryService.AddWordToUser("dog", userVasya);
        

        await _textService.AddText("Dog on the table");
        await _textService.AddText("Please take off clothes and take an apple");
        await _textService.AddText("I take it. I took it");
        await _textService.AddText("I like Dog");
        await _textService.AddText("like a boss");
        
        return Ok();   
    }


    [HttpGet("Migrate")]
    public async Task MigrateData()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

}