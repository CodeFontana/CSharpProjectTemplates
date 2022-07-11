using DataLibrary.Identity.Models;
using DataLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Interfaces;

namespace WebApi.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ServiceFilter(typeof(UserActivity))]
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> RegisterAsync(RegisterUserModel registerUser)
    {
        ServiceResponseModel<AuthUserModel> response = await _accountService.RegisterAsync(HttpContext.User.Identity.Name, registerUser);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ServiceFilter(typeof(UserActivity))]
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> LoginAsync(LoginUserModel loginUser)
    {
        ServiceResponseModel<AuthUserModel> response = await _accountService.LoginAsync(loginUser);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return Unauthorized(response);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    [ServiceFilter(typeof(UserActivity))]
    public async Task<ActionResult<ServiceResponseModel<AccountModel>>> GetAccountAsync(string username)
    {
        ServiceResponseModel<AccountModel> response = await _accountService.GetAccount(HttpContext.User.Identity.Name, username);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ServiceResponseModel<bool>>> DeleteAccountAsync(string username)
    {
        ServiceResponseModel<bool> response = await _accountService.DeleteAccount(HttpContext.User.Identity.Name, username);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }
}
