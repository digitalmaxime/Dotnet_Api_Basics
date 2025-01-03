using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net9RefreshTokenDemo.Api.constants;
using Net9RefreshTokenDemo.Api.Dtos;
using Net9RefreshTokenDemo.Api.Models;
using Net9RefreshTokenDemo.Api.Services;

namespace Net9RefreshTokenDemo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AuthController> _logger;
    private readonly ITokenService _tokenService; // new code
    private readonly AppDbContext _context; // new code


    public AuthController(UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager, ILogger<AuthController> logger, ITokenService tokenService, AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _tokenService = tokenService;
        _context = context;
    }

    // this method can be accessed by any one
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult Post()
    {
        return Ok();
    }

    [HttpPost("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminPost()
    {
        return Ok();
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupModel model)
    {
        try
        {
            var existingUser = await _userManager.FindByNameAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            // Create User role if it doesn't exist
            if ((await _roleManager.RoleExistsAsync(Roles.User)) == false)
            {
                var roleResult = await _roleManager
                      .CreateAsync(new IdentityRole(Roles.User));

                if (roleResult.Succeeded == false)
                {
                    var roleErros = roleResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to create user role. Errors : {string.Join(",", roleErros)}");
                    return BadRequest($"Failed to create user role. Errors : {string.Join(",", roleErros)}");
                }
            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Name = model.Name,
                EmailConfirmed = true
            };

            // Attempt to create a user
            var createUserResult = await _userManager.CreateAsync(user, model.Password);

            // Validate user creation. If user is not created, log the error and
            // return the BadRequest along with the errors
            if (createUserResult.Succeeded == false)
            {
                var errors = createUserResult.Errors.Select(e => e.Description);
                _logger.LogError(
                    $"Failed to create user. Errors: {string.Join(", ", errors)}"
                );
                return BadRequest($"Failed to create user. Errors: {string.Join(", ", errors)}");
            }

            // adding role to user
            var addUserToRoleResult = await _userManager.AddToRoleAsync(user: user, role: Roles.User);

            if (addUserToRoleResult.Succeeded == false)
            {
                var errors = addUserToRoleResult.Errors.Select(e => e.Description);
                _logger.LogError($"Failed to add role to the user. Errors : {string.Join(",", errors)}");
            }
            return CreatedAtAction(nameof(Signup), null);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginModel model)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("User with this username is not registered with us.");
            }
            bool isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (isValidPassword == false)
            {
                return Unauthorized();
            }

            // creating the necessary claims
            List<Claim> authClaims = [
                    new (ClaimTypes.Name, user.UserName),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                // unique id for token
        ];

            var userRoles = await _userManager.GetRolesAsync(user);

            // adding roles to the claims. So that we can get the user role from the token.
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // generating access token
            var token = _tokenService.GenerateAccessToken(authClaims);

            string refreshToken = _tokenService.GenerateRefreshToken();

            //save refreshToken with exp date in the database
            var tokenInfo = _context.TokenInfos.
                        FirstOrDefault(a => a.Username == user.UserName);

            // If tokenInfo is null for the user, create a new one
            if (tokenInfo == null)
            {
                var ti = new TokenInfo
                {
                    Username = user.UserName,
                    RefreshToken = refreshToken,
                    ExpiredAt = DateTime.UtcNow.AddDays(7)
                };
                _context.TokenInfos.Add(ti);
            }
            // Else, update the refresh token and expiration
            else
            {
                tokenInfo.RefreshToken = refreshToken;
                tokenInfo.ExpiredAt = DateTime.UtcNow.AddDays(7);
            }

            await _context.SaveChangesAsync();

            return Ok(new TokenModel
            {
                AccessToken = token,
                RefreshToken = refreshToken
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Unauthorized();
        }

    }

    [HttpPost("token/refresh")]
    public async Task<IActionResult> Refresh(TokenModel tokenModel)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            var username = principal.Identity.Name;

            var tokenInfo = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
            if (tokenInfo == null
                || tokenInfo.RefreshToken != tokenModel.RefreshToken
                || tokenInfo.ExpiredAt <= DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token. Please login again.");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            tokenInfo.RefreshToken = newRefreshToken; // rotating the refresh token
            await _context.SaveChangesAsync();

            return Ok(new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("token/revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke()
    {
        try
        {
            var username = User.Identity.Name; // Microsoft.AspNetCore.Mvc ClaimsPrincipal User => HttpContext?.User

            var user = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return BadRequest();
            }

            user.RefreshToken = string.Empty;
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}