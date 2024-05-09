using MeherEstateDevelopers.App_Start;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MeherEstateDevelopers
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            //public override void OnActionExecuting(ActionExecutingContext filterContext)
            //{
            //    if (filterContext.HttpContext.Request.UrlReferrer == null ||
            //                filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            //    {
            //        //filterContext.Result = new RedirectToRouteResult(new
            //        //               RouteValueDictionary(new { controller = "Account", action = "DirectUrlHit", area = "" }));
            //        string hittingUrl = filterContext.HttpContext.Request.Url.ToString();
            //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            //        {
            //            controller = "Account",
            //            action = "DirectUrlHit",
            //            area = "",
            //            hittingUrl = hittingUrl // Add the hittingUrl parameter
            //        }));
            //    }
            //}
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (!filterContext.IsChildAction)
                {
                    if (filterContext.HttpContext.Request.UrlReferrer == null ||
                        filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                    {
                        string hittingUrl = filterContext.HttpContext.Request.Url.ToString();
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Account",
                            action = "DirectUrlHit",
                            area = "",
                            hittingUrl = hittingUrl
                        }));
                    }
                }
            }

        }
    }
}
