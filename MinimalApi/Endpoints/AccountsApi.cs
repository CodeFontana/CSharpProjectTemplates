using Microsoft.AspNetCore.Mvc;
using MinimalApi.Filters;
using MinimalApi.Services;
using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace MinimalApi.Endpoints;

public static class AccountsApi
{
    public static void AddAccountsApiEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/Accounts", GetAccountsAsync)
            .RequireAuthorization("Administrator")
            .AddEndpointFilter<RequireNameClaimFilter>()
            .AddEndpointFilter<UserActivityFilter>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireRateLimiting("fixed");

        app.MapGet("/api/v1/Accounts/{username:required}", GetAccountAsync)
            .RequireAuthorization("Administrator")
            .AddEndpointFilter<RequireNameClaimFilter>()
            .AddEndpointFilter<UserActivityFilter>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireRateLimiting("fixed");

        app.MapPost("/api/v1/Accounts", RegisterAccountAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireRateLimiting("fixed");

        app.MapPost("/api/v1/Accounts/login", LoginAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireRateLimiting("fixed");

        app.MapPut("/api/v1/Accounts", UpdateAccountAsync)
            .RequireAuthorization("Administrator")
            .AddEndpointFilter<RequireNameClaimFilter>()
            .AddEndpointFilter<UserActivityFilter>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireRateLimiting("fixed");

        app.MapDelete("/api/v1/Accounts/{username:required}", DeleteAccountAsync)
            .RequireAuthorization("Administrator")
            .AddEndpointFilter<RequireNameClaimFilter>()
            .AddEndpointFilter<UserActivityFilter>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireRateLimiting("fixed");
    }

    private static async Task<IResult> GetAccountsAsync(HttpContext httpContext,
                                                        [FromServices] IAccountService accountService)
    {
        try
        {
            string userName = httpContext.User.Identity!.Name!;
            ServiceResponseModel<List<AccountModel>> response = await accountService.GetAccountsAsync(userName);

            if (response.Success)
            {
                return Results.Ok(response);
            }
            else
            {
                return Results.BadRequest(response);
            }
        }
        catch (Exception e)
        {
            return Results.Problem(
                type: "Internal Server Error",
                title: "An error occurred while reading accounts",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> GetAccountAsync(HttpContext httpContext,
                                                       [FromServices] IAccountService accountService,
                                                       [FromRoute] string username)
    {
        try
        {
            string requestor = httpContext.User.Identity!.Name!;
            ServiceResponseModel<AccountModel> response = await accountService.GetAccountAsync(requestor, username);

            if (response.Success)
            {
                return Results.Ok(response);
            }
            else
            {
                return Results.BadRequest(response);
            }
        }
        catch (Exception e)
        {
            return Results.Problem(
                type: "Internal Server Error",
                title: "An error occurred while reading an account",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> RegisterAccountAsync([FromServices] IAccountService accountService,
                                                            [FromBody] RegisterUserModel registerUser)
    {
        try
        {
            ServiceResponseModel<AuthUserModel> response = await accountService.RegisterAsync(registerUser);

            if (response.Success)
            {
                return Results.Ok(response);
            }
            else
            {
                return Results.BadRequest(response);
            }
        }
        catch (Exception e)
        {
            return Results.Problem(
                type: "Internal Server Error",
                title: "An error occurred while registering an account",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> LoginAsync([FromServices] IAccountService accountService,
                                                  [FromBody] LoginUserModel loginUser)
    {
        try
        {
            ServiceResponseModel<AuthUserModel> response = await accountService.LoginAsync(loginUser);

            if (response.Success)
            {
                return Results.Ok(response);
            }
            else
            {
                return Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
            }
        }
        catch (Exception e)
        {
            return Results.Problem(
                type: "Internal Server Error",
                title: "An error occurred during account login",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> UpdateAccountAsync(HttpContext httpContext,
                                                         [FromServices] IAccountService accountService,
                                                         [FromBody] AccountUpdateModel updateAccount)
    {
        try
        {
            string requestor = httpContext.User.Identity!.Name!;
            ServiceResponseModel<bool> response = await accountService.UpdateAccountAsync(requestor, updateAccount);

            if (response.Success)
            {
                return Results.Ok(response);
            }
            else
            {
                return Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
            }
        }
        catch (Exception e)
        {
            return Results.Problem(
                type: "Internal Server Error",
                title: "An error occurred while updating an account",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> DeleteAccountAsync(HttpContext httpContext,
                                                         [FromServices] IAccountService accountService,
                                                         [FromRoute] string username)
    {
        try
        {
            string requestor = httpContext.User.Identity!.Name!;
            ServiceResponseModel<bool> response = await accountService.DeleteAccountAsync(requestor, username);

            if (response.Success)
            {
                return Results.Ok(response);
            }
            else
            {
                return Results.BadRequest(response);
            }
        }
        catch (Exception e)
        {
            return Results.Problem(
                type: "Internal Server Error",
                title: "An error occurred while deleting an account",
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
