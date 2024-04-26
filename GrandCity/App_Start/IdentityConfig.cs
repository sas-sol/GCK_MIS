using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MeherEstateDevelopers.Models;
using System.Net;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Net.Mail;

namespace MeherEstateDevelopers
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.

            return Task.FromResult(0);
        }
        public Task SendEmail(string Body, string To, string Subject)
        {
            //var client = new SmtpClient("smtp.office365.com")
            //{
            //    Port = 587,
            //    UseDefaultCredentials = false,
            //    EnableSsl = true,
            //    Credentials = new NetworkCredential("mis@meherestate.ltd", "i!VEQREqOPF@")
            //};

            //var mailMessage = new MailMessage { From = new MailAddress("mis@meherestate.ltd") };
            //mailMessage.To.Add(To);
            //mailMessage.Body = Body;
            //mailMessage.Subject = Subject;
            //mailMessage.IsBodyHtml = true;
            //try
            //{
            //    client.Send(mailMessage);
            //}
            //catch (Exception e)
            //{

            //};
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.Destination) && !string.IsNullOrEmpty(msg.Body))
            {
                //String result = "";
                //String message = HttpUtility.UrlEncode(msg.Body);
                //string From = "SA Systems";
                //String strPost = "id=rchsagarden&pass=garden44&msg=" + msg.Body + "&to=" + msg.Destination + "&mask=" + From + "&type=xml&lang=English";
                //StreamWriter myWriter = null;
                //HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("http://www.outreach.pk/api/sendsms.php/sendsms/url");

                //objRequest.Method = "POST";
                //objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
                //objRequest.ContentType = "application/x-www-form-urlencoded";
                //try
                //{
                //    myWriter = new StreamWriter(objRequest.GetRequestStream());
                //    myWriter.Write(strPost);
                //}
                //catch (Exception e)
                //{
                //    //return e.Message; 
                //}
                //finally
                //{
                //    myWriter.Close();
                //}

                //HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                //using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                //{
                //    result = sr.ReadToEnd();   // Close and clean up the StreamReader   
                //    sr.Close();
                //}
            }
            return Task.FromResult(0);
        }
        public Task SendMsg(string Body, string Destination)
        {
            Destination = Destination.Replace("-", String.Empty);
            Destination = Destination.Replace(" ", String.Empty);
            string firstch = Destination.Substring(0, 1);
            if (firstch == "0")
            {
                Destination = "92" + Destination.Remove(0, 1);
            }
            //var res2 = PremiumSub(Body, Destination);

            return Task.FromResult(0);
        }
        private string PremiumSub(string Body, string Destination)
        {
            string download = "no";
            //try
            //{
            //    string apipart1 = "http://smsctp3.eocean.us:24555/api?action=sendmessage&username=sa_gardens_api&password=pak!stan123!4";
            //    string apipart2 = "&recipient=" + Destination;
            //    string apipart3 = "&originator=99090";
            //    string apipart4 = "&messagedata=" + Body;
            //    System.Net.WebClient myWebClient;
            //    myWebClient = new System.Net.WebClient();
            //    download = myWebClient.DownloadString(apipart1 + apipart2 + apipart3 + apipart4);
            //}
            //catch (Exception ex)
            //{
            //    download = ex.Message;
            //}
            return download;
        }
        public Task Broadcast(string Body, string Destination)
        {
            Destination = Destination.Replace("-", String.Empty);
            Destination = Destination.Replace(" ", String.Empty);
            string firstch = Destination.Substring(0, 1);
            if (firstch == "0")
            {
                Destination = "92" + Destination.Remove(0, 1);
            }
            string tele = Destination.Substring(0, 5);
            var res2 = Promotional(Body, Destination);

            return Task.FromResult(0);
        }
        private string Promotional(string Body, string Destination)
        {
            string download = "no";
            try
            {
                string apipart1 = "https://pk.eocean.us/APIManagement/API/RequestAPI?user=sa_gardens&pwd=AIZQPnnNvMA0%2fX0qvZ5u%2fCEZ7LBGTHI%2bpL%2bWWLViYAKDs0WtDhs%2frdhld7HclY7CAA%3d%3d";
                string apipart2 = "&sender=Meher Estate Developers";
                string apipart3 = "&reciever=" + Destination;
                string apipart4 = "&msg-data=" + Body;
                string apipart5 = "&response=string";
                System.Net.WebClient myWebClient;
                myWebClient = new System.Net.WebClient();
                download = myWebClient.DownloadString(apipart1 + apipart2 + apipart3 + apipart4 + apipart5);
            }
            catch (Exception ex)
            {
                using (Grand_CityEntities db = new Grand_CityEntities())
                {
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, null, ex.Message);
                }
                download = ex.Message;
            }
            return download;
        }
    }
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<MyUser, long>
    {
        public ApplicationUserManager(IUserStore<MyUser, long> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<MyUser, MyRole, long, MyLogin, MyUserRole, MyClaim>(new ApplicationDbContext()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<MyUser, long>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<MyUser, long>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<MyUser, long>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<MyUser, long>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<MyUser, long>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        // public override Task<ClaimsIdentity> CreateUserIdentityAsync(MyUser user)
        //  {
        //return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        //  }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
