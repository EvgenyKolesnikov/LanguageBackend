using Language.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Language;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomAuthorizeAttribute: Attribute,IAsyncAuthorizationFilter
{
    private MainDbContext _dbcontext;
    

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<MainDbContext>();
        
        var userId = context.HttpContext.User.GetUserId();
        
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userExists = await dbContext.Users.AnyAsync(u => u.Id == id);
        if (!userExists)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}