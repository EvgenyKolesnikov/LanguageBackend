using Language.Database;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers;

public class UtilityController: ControllerBase
{
    public readonly MainDbContext _dbContext;

    public UtilityController(MainDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }


    
    [HttpGet("FillData")]
    public async Task<ActionResult> FillData()
    {
        
        
        
        
        return Ok();   
    }


    [HttpGet("Migrate")]
    public async Task Migrate()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        
        await _dbContext.Users.AddAsync(new User() { Id = new Guid(), Name = "Admin", Email = "test@mail.ru", Password = "1234" });
    }

}