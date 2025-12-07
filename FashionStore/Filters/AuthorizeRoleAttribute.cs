using System;
using System.Web;
using System.Web.Mvc;

namespace FashionStore.Filters
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string[] _allowedRoles;

        public AuthorizeRoleAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var session = httpContext.Session;
            if (session["UserId"] == null)
                return false;

            var userRole = session["Role"]?.ToString();
            if (string.IsNullOrEmpty(userRole))
                return false;

            foreach (var role in _allowedRoles)
            {
                if (userRole.Equals(role, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
            else
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "Unauthorized"
                };
            }
        }
    }
}

