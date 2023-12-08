using System.Web.Mvc;

public class LicenseAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationContext filterContext)
    {
        if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        {
            // Call the license validation API
            bool isLicenseValid = CallLicenseValidationApi();

            // If the license is not valid, deny access
            if (!isLicenseValid)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        else
        {
            // If the user is not authenticated, deny access
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }

    private bool CallLicenseValidationApi()
    {
        //Pass Project Unique Id from here (string)
        //This will return API result later
        return (false);
    }
}
