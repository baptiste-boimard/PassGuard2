using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PassGuard.Api.Repositories;
using PassGuard.Shared.Models;

namespace PassGuard.Api.Controllers;

[Route("api/password")]
[ApiController]
public class DataController : ControllerBase
{
    private readonly DataRepository _dataRepository;

    public DataController(DataRepository dataRepository)
    {
        _dataRepository = dataRepository;
    }

    [HttpPost]
    [Route("postpassword")]
    public async Task<IActionResult> PostPassword([FromBody] ObjectPasswordForm objectPasswordForm)
    {
        var existingOne = await _dataRepository.VerifyExistingOne(objectPasswordForm);

        if (existingOne != null)
        {
            return Conflict(new { message = "Cet utilisateur pour ce site existe déjà !" });
        }

        var newObjectPasswordDTO = await _dataRepository.SaveNewObjectPassword(objectPasswordForm);

        return Ok(newObjectPasswordDTO);
    }

    [HttpGet]
    [Route("getpassword")]
    public async Task<IActionResult> GetPassword()
    {
        var passwordArray = await _dataRepository.GetPasswords();

        if (passwordArray == null)
        {
            return NoContent();
        }
        
        return Ok(passwordArray);
    }
}