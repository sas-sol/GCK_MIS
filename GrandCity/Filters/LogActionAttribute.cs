using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;

namespace MeherEstateDevelopers.Filters
{
    public class LogActionAttribute : ActionFilterAttribute
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.RequestContext.RouteData.Values["Controller"];
            var action = filterContext.RequestContext.RouteData.Values["Action"];
            var user = filterContext.RequestContext.HttpContext.User.Identity.GetUserId();
            var parameters = JsonConvert.SerializeObject (filterContext.ActionParameters);
            //
            // Perform logging here
            try
            {
                //var a = db.SP_System_Log(controller + " - " + action + " - " + parameters, GetIp(), long.Parse(user));
            }
            catch (Exception ex)
            {
                //EmailService e = new EmailService();
                //e.SendEmail(ex.Message + " " + ex.InnerException, "taimoor@sasystems.solutions", "Error In Loging");
            }
            //



            base.OnActionExecuting(filterContext);
        }
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }
    }
}