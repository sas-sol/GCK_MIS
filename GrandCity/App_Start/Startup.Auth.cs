using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using MeherEstateDevelopers.Models;
using System.Web;
using System.Web.Security;
using System.Configuration;

namespace MeherEstateDevelopers
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                //provider = new cookieauthenticationprovider
                //{
                //    // enables the application to validate the security stamp when the user logs in.
                //    // this is a security feature which is used when you change a password or add an external login to your account.  
                //    onvalidateidentity = securitystampvalidator.onvalidateidentity<applicationusermanager, myuser>(
                //        validateinterval: timespan.fromminutes(30),
                //        regenerateidentity: (manager, user) => user.generateuseridentityasync(manager))
                //}
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }

    //public class LogoutModule : IHttpModule
    //{
    //    #region IHttpModule Members
    //    void IHttpModule.Dispose() { }
    //    void IHttpModule.Init(HttpApplication context)
    //    {
    //        context.AuthenticateRequest += new EventHandler(context_AuthenticateRequest);
    //    }
    //    #endregion


    //    /// <summary>
    //    /// Handle the authentication request and force logouts according to web.config
    //    /// </summary>
    //    /// <remarks>See "How To Implement IPrincipal" in MSDN</remarks>
    //    private void context_AuthenticateRequest(object sender, EventArgs e)
    //    {
    //        HttpApplication a = (HttpApplication)sender;
    //        HttpContext context = a.Context;

    //        // Extract the forms authentication cookie
    //        string cookieName = FormsAuthentication.FormsCookieName;
    //        HttpCookie authCookie = context.Request.Cookies[cookieName];
    //        if (authCookie != null )
    //        {
    //            // Delete the auth cookie and let them start over.
    //            authCookie.Expires = DateTime.Now.AddDays(-1);
    //            context.Response.Cookies.Add(authCookie);
    //            context.Response.Redirect(FormsAuthentication.LoginUrl);
    //            context.Response.End();
    //        }
    //    }
    //}
}