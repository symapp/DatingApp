using System.Diagnostics;
using API.Data;
using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (resultContext.HttpContext.User.Identity is { IsAuthenticated: false }) return;

        var userId = resultContext.HttpContext.User.GetUserId();
        var uow = resultContext.HttpContext.RequestServices.GetService<UnitOfWork>();

        if (uow != null)
        {
            var user = await uow.UserRepository.GetUserByIdAsync(userId); 
            user.LastActive = DateTime.Now;
            await uow.Complete();
        }
    }
}