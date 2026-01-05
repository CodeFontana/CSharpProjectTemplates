using WebApi.IdentityLibrary.Entities;
using WebApi.IdentityLibrary.Identity;

namespace MinimalApi.Filters;

public sealed class UserActivityFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        object? result = await next(context);

        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return result;
        }

        string username = context.HttpContext.User.Identity?.Name ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username))
        {
            return result;
        }

        IAccountRepository repo = context.HttpContext.RequestServices.GetRequiredService<IAccountRepository>();
        AppUser? user = await repo.GetAccountAsync(username);

        if (user is null)
        {
            return result;
        }

        user.LastActive = DateTime.UtcNow;
        await repo.SaveAllAsync();
        return result;
    }
}
