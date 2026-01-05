using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.IdentityLibrary.Entities;
using WebApi.IdentityLibrary.Identity;

namespace WebApi.Filters;

public class UserActivityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ActionExecutedContext resultContext = await next();

        if (resultContext == null || resultContext.HttpContext.User.Identity?.IsAuthenticated == false)
        {
            return;
        }

        IAccountRepository repo = resultContext.HttpContext.RequestServices.GetRequiredService<IAccountRepository>();
        string username = resultContext.HttpContext.User.Identity?.Name ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(username))
        {
            AppUser? user = await repo.GetAccountAsync(username);

            if (user is not null)
            {
                user.LastActive = DateTime.UtcNow;
                await repo.SaveAllAsync();
            }
        }
    }
}
