﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        var token = JwtCreateToken(exixtingAccountDTO);

        return Ok(token);
    }

    [HttpPost]
    [Route("verifypass")]
    public async Task<IActionResult> VerifyPassword([FromBody] VerifyPassword payload)
    {
        var DecryptedPassword = await _authRepository.VerifyPassword(payload);

        if (DecryptedPassword != null)
        {
            return Ok(DecryptedPassword);
        }

        return Unauthorized(new { message = "Vous n'êtes pas un utilisateur reconnu" });
    }
    
    [HttpPost]
    [Route("getemail")]
    public async Task<IActionResult> GetEmail([FromBody] string token)
    {
        var email = await _authRepository.GetEmail(token);

        if (email != null)
        {
            return Ok(email);
        }

        return Unauthorized(new { message = "Vous n'êtes pas un utilisateur reconnu" });
    }
}