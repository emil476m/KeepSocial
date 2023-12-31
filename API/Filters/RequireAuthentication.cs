﻿using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class RequireAuthentication : ActionFilterAttribute
{
    /*
     * checks if a user is logged in
     */
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var str = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
        if (str is null) throw new AuthenticationException();
    }
}
