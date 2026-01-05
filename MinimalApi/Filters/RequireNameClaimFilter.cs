using WebApi.SharedLibrary.Models;

namespace MinimalApi.Filters;

public sealed class RequireNameClaimFilter : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            ServiceResponseModel<object> response = new()
            {
                Success = false,
                Message = "User is not authenticated"
            };

            IResult result = Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
            return ValueTask.FromResult<object?>(result);
        }

        string? userName = context.HttpContext.User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userName))
        {
            ServiceResponseModel<object> response = new()
            {
                Success = false,
                Message = "Authenticated user is missing the Name claim"
            };

            IResult result = Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
            return ValueTask.FromResult<object?>(result);
        }

        return next(context);
    }
}
