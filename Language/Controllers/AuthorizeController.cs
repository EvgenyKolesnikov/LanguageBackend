using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Language.Database;
using Language.Model;
using Language.Requests;
using Language.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Language.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorizeController : ControllerBase
{
    public readonly JwtOptions _options;
    public readonly MainDbContext _dbContext;

    public AuthorizeController(IOptions<JwtOptions> options,MainDbContext dbcontext)
    {
        _options = options.Value;
        _dbContext = dbcontext;
    }



    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUser request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(i => i.Email == request.Email);
        if (user != null)
            return Conflict("Пользователь c таким Email уже существует");
        
        var newUser = new User()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };
        
        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpGet("Login")]
    public async Task<ActionResult<LoginResponse>> GetToken(string email, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(i => i.Email == email && i.Password == password);
        if (user == null)
            return Unauthorized();
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));;
      

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddYears(10),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        var response = new LoginResponse(new JwtSecurityTokenHandler().WriteToken(jwt));
        return new ObjectResult(response);
    }
    
    [HttpGet("CheckToken")]
    [CustomAuthorize]
    public IActionResult CheckToken()
    {
        return Ok();
    }
    
}