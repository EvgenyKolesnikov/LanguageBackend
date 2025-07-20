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

}