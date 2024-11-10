using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Interfaces;
using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace WebApi.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ServiceFilter(typeof(UserActivity))]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [Authorize(Policy = "Administrator")]
    public async Task<ActionResult<ServiceResponseModel<List<AccountModel>>>> GetAccounts()
    {
        ServiceResponseModel<List<AccountModel>> response = await _accountService.GetAccountsAsync(HttpContext.User.Identity.Name);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpGet("{username}")]
    [Authorize(Policy = "Administrator")]
    public async Task<ActionResult<ServiceResponseModel<AccountModel>>> GetAccount(string username)
    {
        ServiceResponseModel<AccountModel> response = await _accountService.GetAccountAsync(HttpContext.User.Identity.Name, username);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> RegisterAccount([FromBody] RegisterUserModel registerUser)
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
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> Login([FromBody] LoginUserModel loginUser)
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

    [HttpPut]
    [Authorize(Policy = "Administrator")]
    public async Task<ActionResult<ServiceResponseModel<bool>>> UpdateAccount([FromBody] AccountUpdateModel updateAccount)
    {
        ServiceResponseModel<bool> response = await _accountService.UpdateAccountAsync(HttpContext.User.Identity.Name, updateAccount);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return Unauthorized(response);
        }
    }

    [HttpDelete("{username}")]
    [Authorize(Policy = "Administrator")]
    public async Task<ActionResult<ServiceResponseModel<bool>>> DeleteAccount(string username)
    {
        ServiceResponseModel<bool> response = await _accountService.DeleteAccountAsync(HttpContext.User.Identity.Name, username);

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
