using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Language.Database;
using Language.Profile;
using Microsoft.AspNetCore.Mvc;

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
}