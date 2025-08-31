using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ITLATaskManagerAPI.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var isAdmin = user.IsInRole("Admin");
            var isUser = user.IsInRole("User");

            if (!isAdmin && !isUser)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!isAdmin && context.HttpContext.Request.Method != "GET")
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
