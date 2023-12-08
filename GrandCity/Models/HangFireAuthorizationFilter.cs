using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;

namespace MeherEstateDevelopers.Models
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {

        public bool Authorize([NotNull] DashboardContext context)
        {
            // In case you need an OWIN context, use the next line, `OwinContext` class
            // is the part of the `Microsoft.Owin` package.
            return HttpContext.Current.User.IsInRole("Administrator");
            //return true;
        }
    }
}