﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Language.JwtExtensions;
using Language.Database;
using Language.Dictionary;
using Language.Model;
using Language.Profile;
using Language.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Language.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    public readonly MainDbContext _dbContext;
    public readonly DictionaryService _dictionaryService;
    
    public ProfileController(MainDbContext dbcontext, DictionaryService dictionaryService)
    {
        _dbContext = dbcontext;
        _dictionaryService = dictionaryService;
    }


    /// <summary>
    /// Получить информацию о пользователе
    /// </summary>
    /// <returns></returns>
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

    
    /// <summary>
    /// Добавить слово в словарь пользователя
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    [CustomAuthorize]
    [HttpPost("UserDictionary")]
    public async Task<IActionResult> AddWordToUser(string word)
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var userIdClaim = authHeader.GetClaimFromJwtToken(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null)
            return BadRequest("UserId claim not found in token");

        var user = await _dbContext.Users.FindAsync(new Guid (userIdClaim));
        if (user == null) return BadRequest("User Not Found");
        
        var wordDb = await _dictionaryService.AddWordToUser(word, user);

        if (wordDb != null)
        {
            return Ok();
        }
        
        return BadRequest("Word not found");
    }

    /// <summary>
    /// Получить словарь пользователя
    /// </summary>
    /// <returns></returns>
    [CustomAuthorize]
    [HttpGet("UserDictionary")]
    public async Task<IActionResult> GetWordsByUser()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var userIdClaim = authHeader.GetClaimFromJwtToken(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null)
            return BadRequest("UserId claim not found in token");
        
        var response = await _dictionaryService.GetWordsByUser(new Guid(userIdClaim));
        
        return Ok(response);
    }
}