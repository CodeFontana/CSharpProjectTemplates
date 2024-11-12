﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;
using WebApi.Interfaces;
using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace WebApi.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[EnableRateLimiting("fixed")]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<List<AccountModel>>>> GetAccounts()
    {
        try
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<AccountModel>>> GetAccount(string username)
    {
        try
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
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> RegisterAccount([FromBody] RegisterUserModel registerUser)
    {
        try
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
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> Login([FromBody] LoginUserModel loginUser)
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<bool>>> UpdateAccount([FromBody] AccountUpdateModel updateAccount)
    {
        try
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceResponseModel<bool>>> DeleteAccount(string username)
    {
        try
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
