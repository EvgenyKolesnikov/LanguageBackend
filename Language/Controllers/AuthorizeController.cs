using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Language.Database;
using Language.Model;
using Language.Requests;
using Language.Responses;
using Language.Services;
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
    public readonly AuthService _authService;

    public AuthorizeController(IOptions<JwtOptions> options,MainDbContext dbcontext, AuthService authService)
    {
        _options = options.Value;
        _dbContext = dbcontext;
        _authService = authService;
    }


    /// <summary>
    /// Создание нового пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUser request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(i => i.Email == request.Email);
        if (user != null)
            return Conflict("Пользователь c таким Email уже существует");
       
        await _authService.RegisterUser(request);
    
        return Ok();
    }
    
    /// <summary>
    /// Аутенфикация нового пользователя
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Проверка валидности токена
    /// </summary>
    /// <returns></returns>
    [HttpGet("CheckToken")]
    [CustomAuthorize]
    public IActionResult CheckToken()
    {
        return Ok();
    }
    
}