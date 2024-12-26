using CookiesWebAppMVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookiesWebAppMVC.Controllers;

[Controller]
[Route("api/[controller]")]
public class UserController: ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserController(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    // [Authorize]
    [HttpGet]
    [Route("users")]
    public async Task<ActionResult<string>> GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();

        return await Task.FromResult(System.Text.Json.JsonSerializer.Serialize(users));
    }
    
    [HttpGet]
    [Route("currentuserclaims/")]
    public async Task<ActionResult<string>> GetCurrentUserCookieClaims()
    {
        var userClaims = _httpContextAccessor.HttpContext?.User.Claims;
        var claimsDto = userClaims?.Select(x => new
        {
            x.Type,
            x.Value
        });
        
        return await Task.FromResult(System.Text.Json.JsonSerializer.Serialize(claimsDto));
    }
    
}