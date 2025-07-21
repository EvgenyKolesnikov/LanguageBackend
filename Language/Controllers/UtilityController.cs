using Language.Database;
using Language.Model;
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
        await _dbContext.Users.AddAsync(new User() { Id = new Guid(), Name = "Admin", Email = "test@mail.ru", Password = "1234" });
        await _dbContext.SaveChangesAsync();
        
        
        
        return Ok();   
    }


    [HttpGet("Migrate")]
    public async Task MigrateData()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

}