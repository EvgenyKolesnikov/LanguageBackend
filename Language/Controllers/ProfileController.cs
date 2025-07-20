using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Language.Database;
using Language.Dictionary;
using Language.Model;
using Language.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Language.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    public readonly MainDbContext _dbContext;
    
    public ProfileController(MainDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }


    [HttpGet("User")]
    [CustomAuthorize]
    public async Task<ActionResult> GetUser()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader.Substring("Bearer ".Length).Trim();
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null)
            return BadRequest("UserId claim not found in token");
        
        var response = await _dbContext.Users.FindAsync(new Guid (userIdClaim.Value));
        var userResponse = new GetUserResponse(response);
        
        return Ok(userResponse);
    }

    
    [CustomAuthorize]
    [HttpPost("UserDictionary")]
    public async Task<IActionResult> AddWordToUser(string word)
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader.Substring("Bearer ".Length).Trim();
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null)
            return BadRequest("UserId claim not found in token");

        var user = await _dbContext.Users.FindAsync(new Guid (userIdClaim.Value));
        if (user == null) return BadRequest("User Not Found");
        
        var wordDb = await _dbContext.Dictionary.Include(i => i.Users)
            .FirstOrDefaultAsync(i => i.Word == word.ToLower());

        if (wordDb != null)
        {
            wordDb.Users.Add(user);    
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
        return BadRequest("Word not found");
    }

    [CustomAuthorize]
    [HttpGet("UserDictionary")]
    public async Task<IActionResult> GetWordsByUser()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader.Substring("Bearer ".Length).Trim();
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null)
            return BadRequest("UserId claim not found in token");

        var listDictionary = await _dbContext.Users.Where(i => i.Id == new Guid(userIdClaim.Value))
            .Select(i => new GetWordsByUserResponse(i.Dictionary)).ToListAsync();
        
        return Ok(listDictionary);
    }
}