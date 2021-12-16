using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Candidatura.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RERAuthorizeAttribute : AuthorizeAttribute
    {
        protected virtual RERPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as RERPrincipal; }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return ((CurrentUser != null && !CurrentUser.IsInRole(Roles)) || CurrentUser == null) ? false : true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RedirectToRouteResult routeData = null;

            routeData = new RedirectToRouteResult
            (new System.Web.Routing.RouteValueDictionary
             (new
             {
                 controller = "NonAutorizzato",
                 action = "Index"
             }
             ));

            filterContext.Result = routeData;
        }
    }
}