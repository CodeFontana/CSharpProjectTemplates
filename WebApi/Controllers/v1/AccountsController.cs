using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;
using WebApi.Services;
using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace WebApi.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[EnableRateLimiting("fixed")]
[ServiceFilter(typeof(UserActivityFilter))]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [Authorize(Policy = "Administrator")]
    [ServiceFilter(typeof(RequireNameClaimFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<List<AccountModel>>>> GetAccountsAsync()
    {
        try
        {
            string userName = HttpContext.User.Identity!.Name!;
            ServiceResponseModel<List<AccountModel>> response = await _accountService.GetAccountsAsync(userName);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception e)
        {
            return Problem(
                type: "Internal Server Error",
                title: "An error occurred while reading accounts",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{username}")]
    [Authorize(Policy = "Administrator")]
    [ServiceFilter(typeof(RequireNameClaimFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<AccountModel>>> GetAccountAsync(string username)
    {
        try
        {
            string requestor = HttpContext.User.Identity!.Name!;
            ServiceResponseModel<AccountModel> response = await _accountService.GetAccountAsync(requestor, username);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception e)
        {
            return Problem(
                type: "Internal Server Error",
                title: "An error occurred while reading an account",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> RegisterAccountAsync([FromBody] RegisterUserModel registerUser)
    {
        try
        {
            ServiceResponseModel<AuthUserModel> response = await _accountService.RegisterAsync(registerUser);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception e)
        {
            return Problem(
                type: "Internal Server Error",
                title: "An error occurred while registering an account",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> LoginAsync([FromBody] LoginUserModel loginUser)
    {
        try
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
        catch (Exception e)
        {
            return Problem(
                type: "Internal Server Error",
                title: "An error occurred during account login",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Authorize(Policy = "Administrator")]
    [ServiceFilter(typeof(RequireNameClaimFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<bool>>> UpdateAccountAsync([FromBody] AccountUpdateModel updateAccount)
    {
        try
        {
            string requestor = HttpContext.User.Identity!.Name!;
            ServiceResponseModel<bool> response = await _accountService.UpdateAccountAsync(requestor, updateAccount);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return Unauthorized(response);
            }
        }
        catch (Exception e)
        {
            return Problem(
                type: "Internal Server Error",
                title: "An error occurred while updating an account",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{username}")]
    [Authorize(Policy = "Administrator")]
    [ServiceFilter(typeof(RequireNameClaimFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<bool>>> DeleteAccountAsync(string username)
    {
        try
        {
            string requestor = HttpContext.User.Identity!.Name!;
            ServiceResponseModel<bool> response = await _accountService.DeleteAccountAsync(requestor, username);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception e)
        {
            return Problem(
                type: "Internal Server Error",
                title: "An error occurred while deleting an account",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
