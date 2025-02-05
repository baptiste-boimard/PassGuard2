using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PassGuard.Api.Repositories;
using PassGuard.Shared.Models;
using static PassGuard.Api.Service.JwtService;


namespace PassGuard.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthRepository _authRepository;

    public AuthController(AuthRepository authRepository)
    {
        _authRepository = authRepository;
    }
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterAccountForm registerAccountForm)
    {
        if (registerAccountForm.Username == null || registerAccountForm.Password == null)
        {
            return BadRequest(new { message = "Le nom d'utilisateur et/ou le mot de passe sont incorrects\"" });
        }
        
        var existingAccount = await _authRepository.VerifyExistingAccount(registerAccountForm);
        
        if (existingAccount != null)
        {
            return Conflict(new { message = "Cet utilisateur existe déjà" });
        }

        var newAccountDTO = await _authRepository.SaveNewAccount(registerAccountForm);
        
        return Ok(newAccountDTO);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginAccountForm loginAccountForm)
    {
        var exixtingAccountDTO = await _authRepository.Login(loginAccountForm);

        if (exixtingAccountDTO == null)
        {
            return Unauthorized(new { message = "Le nom d'utilisateur et/ou le mot de passe sont incorrects"});
        }

        JwtToken tokenString = JwtCreateToken(exixtingAccountDTO);
        
        return Ok(tokenString);
    }
}