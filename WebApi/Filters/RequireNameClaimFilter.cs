using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.SharedLibrary.Models;

namespace WebApi.Filters;

public sealed class RequireNameClaimFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedObjectResult(new ServiceResponseModel<object>
            {
                Success = false,
                Message = "User is not authenticated"
            });

            return Task.CompletedTask;
        }

        string? userName = context.HttpContext.User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userName))
        {
            context.Result = new UnauthorizedObjectResult(new ServiceResponseModel<object>
            {
                Success = false,
                Message = "Authenticated user is missing the Name claim"
            });

            return Task.CompletedTask;
        }

        return next();
    }
}
