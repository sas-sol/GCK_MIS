using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
namespace MeherEstateDevelopers.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // TODO: Add any additional configuration code.

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
                name: "SupportTicketsAddition",
                routeTemplate: "api/Accounts/AddSupportComplains",
                defaults: new { Controller = "Accounts", Action = "AddSupportComplains", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "TicketsList",
                routeTemplate: "api/Accounts/GetSupportComplains",
                defaults: new { Controller = "Accounts", Action = "GetSupportComplains", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
              name: "Discounts",
              routeTemplate: "api/Files/GetDiscountResult",
              defaults: new { Controller = "Files", Action = "GetDiscountResult", id = RouteParameter.Optional }
          );

            config.Routes.MapHttpRoute(
            name: "PlotDiscounts",
            routeTemplate: "api/Files/GetPlotDiscountResult",
            defaults: new { Controller = "Files", Action = "GetPlotDiscountResult", id = RouteParameter.Optional }
        );

            config.Routes.MapHttpRoute(
                name: "Register",
                routeTemplate: "api/Accounts/CustomerLogin",
                defaults: new { Controller = "Accounts", Action = "CustomerLogin", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "Customer Register",
                routeTemplate: "api/Accounts/CustomerRegisteration",
                defaults: new { Controller = "Accounts", Action = "CustomerRegisteration", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "FilesList",
                routeTemplate: "api/Files/GetFiles",
                defaults: new { Controller = "Files", Action = "GetFiles", id = RouteParameter.Optional }
            );


            config.Routes.MapHttpRoute(
              name: "FamilyFilesList",
              routeTemplate: "api/Files/GetFamilyFiles",
              defaults: new { Controller = "Files", Action = "GetFamilyFiles", id = RouteParameter.Optional }
          );


            config.Routes.MapHttpRoute(
              name: "AddFamilyFiles",
              routeTemplate: "api/Files/AddFamilyFiles",
              defaults: new { Controller = "Files", Action = "AddFamilyFiles", id = RouteParameter.Optional }
          );


            config.Routes.MapHttpRoute(
              name: "FilesDetail",
              routeTemplate: "api/Files/GetFileDetails",
              defaults: new { Controller = "Files", Action = "GetFileDetails", id = RouteParameter.Optional }
          );
            config.Routes.MapHttpRoute(
              name: "FilesDetailInstallment",
              routeTemplate: "api/Files/GetFileInstallment",
              defaults: new { Controller = "Files", Action = "GetFileInstallment", id = RouteParameter.Optional }
          );

            config.Routes.MapHttpRoute(
             name: "Perosnal",
             routeTemplate: "api/Files/GetPersonalInfo",
             defaults: new { Controller = "Files", Action = "GetPersonalInfo", id = RouteParameter.Optional }
         );


            config.Routes.MapHttpRoute(
             name: "Perosnal1",
             routeTemplate: "api/Files/GetPersonalInfo1",
             defaults: new { Controller = "Files", Action = "GetPersonalInfo1", id = RouteParameter.Optional }
         );

            config.Routes.MapHttpRoute(
            name: "Nominee",
            routeTemplate: "api/Files/GetNomineeInfo",
            defaults: new { Controller = "Files", Action = "GetNomineeInfo", id = RouteParameter.Optional }
        );
            config.Routes.MapHttpRoute(
         name: "Installments",
         routeTemplate: "api/Files/GetInstallmentInfo",
         defaults: new { Controller = "Files", Action = "GetInstallmentInfo", id = RouteParameter.Optional }
     );

            config.Routes.MapHttpRoute(
         name: "FileStatmentAcc",
         routeTemplate: "api/Files/FileStatmentAcc",
         defaults: new { Controller = "Files", Action = "FileStatmentAcc", id = RouteParameter.Optional }
     );

            config.Routes.MapHttpRoute(
        name: "OtherInstallments",
        routeTemplate: "api/Files/GetOtherInstallmentInfo",
        defaults: new { Controller = "Files", Action = "GetOtherInstallmentInfo", id = RouteParameter.Optional }
    );

            config.Routes.MapHttpRoute(
 name: "Receipts",
 routeTemplate: "api/Files/GetReceiptsInfo",
 defaults: new { Controller = "Files", Action = "GetReceiptsInfo", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "TotalCheckInstallments",
routeTemplate: "api/Files/GetFilesInstallments",
defaults: new { Controller = "Files", Action = "GetFilesInstallments", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "Confirmation",
routeTemplate: "api/Files/GetConfirmationInfo",
defaults: new { Controller = "Files", Action = "GetConfirmationInfo", id = RouteParameter.Optional }
);

 

            config.Routes.MapHttpRoute(
name: "otp",
routeTemplate: "api/Files/OTP_Number",
defaults: new { Controller = "Files", Action = "OTP_Number", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "LoginInformation",
routeTemplate: "api/Accounts/Customer_Login_Information",
defaults: new { Controller = "Accounts", Action = "Customer_Login_Information", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "FamilyLoginInformation",
routeTemplate: "api/Accounts/Family_Customer_Login_Information",
defaults: new { Controller = "Accounts", Action = "Family_Customer_Login_Information", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "Family",
routeTemplate: "api/Accounts/Family_Information",
defaults: new { Controller = "Accounts", Action = "Family_Information", id = RouteParameter.Optional }
);



            config.Routes.MapHttpRoute(
name: "balance",
routeTemplate: "api/Files/GetBalanceInfo",
defaults: new { Controller = "Files", Action = "GetBalanceInfo", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "GetEmail",
routeTemplate: "api/Files/GetEmail",
defaults: new { Controller = "Files", Action = "GetEmail", id = RouteParameter.Optional }
);
            config.Routes.MapHttpRoute(
name: "UpdateEmail",
routeTemplate: "api/Files/UpdateEmail",
defaults: new { Controller = "Files", Action = "UpdateEmail", id = RouteParameter.Optional }
);







            config.Routes.MapHttpRoute(
name: "ValidationEmail",
routeTemplate: "api/Accounts/EmailCheckValidate",
defaults: new { Controller = "Accounts", Action = "EmailCheckValidate", id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
name: "ValidationMobile",
routeTemplate: "api/Accounts/MobileCheckValidate",
defaults: new { Controller = "Accounts", Action = "MobileCheckValidate", id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
name: "Login",
routeTemplate: "api/Accounts/Customer_Login",
defaults: new { Controller = "Accounts", Action = "Customer_Login", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "CustomerPaymentInfo",
routeTemplate: "api/Files/GetCustomerInfo",
defaults: new { Controller = "Files", Action = "GetCustomerInfo", id = RouteParameter.Optional }
);



            config.Routes.MapHttpRoute(
name: "AddReceipts",
routeTemplate: "api/Files/AddReceipts",
defaults: new { Controller = "Files", Action = "AddReceipts", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "ForgetPassword",
routeTemplate: "api/Accounts/ForgetPassword",
defaults: new { Controller = "Accounts", Action = "ForgetPassword", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "ChangePassword",
routeTemplate: "api/Accounts/ChangePassword",
defaults: new { Controller = "Accounts", Action = "ChangePassword", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
name: "FilesCount",
routeTemplate: "api/Files/GetFilesCount",
defaults: new { Controller = "Files", Action = "GetFilesCount", id = RouteParameter.Optional }
);





            config.Routes.MapHttpRoute(
name: "FamilyPropertyFilesList",
routeTemplate: "api/Files/GetFamilyFilesList",
defaults: new { Controller = "Files", Action = "GetFamilyFilesList", id = RouteParameter.Optional }
);
            //.....................................BANK APIS WEB API CONFIG.......................................

            config.Routes.MapHttpRoute(
name: "Qr Information",
routeTemplate: "api/Files/GetQrInformation",
defaults: new { Controller = "Files", Action = "GetQrInformation", id = RouteParameter.Optional }
);



            //.....................................BANK APIS WEB API CONFIG.......................................

            config.Routes.MapHttpRoute(
name: "File_Detail",
routeTemplate: "api/BanksApi/FileDetail",
defaults: new { Controller = "BanksApi", Action = "FileDetail", id = RouteParameter.Optional }
);



            config.Routes.MapHttpRoute(
name: "FileNumber",
routeTemplate: "api/BanksApi/PayAmount",
defaults: new { Controller = "BanksApi", Action = "PayAmount", id = RouteParameter.Optional }
);




            config.Routes.MapHttpRoute(
name: "TestFileDetail",
routeTemplate: "api/BanksApi/File_Detail",
defaults: new { Controller = "BanksApi", Action = "File_Detail", id = RouteParameter.Optional }
);
            config.Routes.MapHttpRoute(
name: "TestFileNumber",
routeTemplate: "api/BanksApi/Pay_Amount",
defaults: new { Controller = "BanksApi", Action = "Pay_Amount", id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
name: "PayTest",
routeTemplate: "api/BanksApi/PayTest",
defaults: new { Controller = "BanksApi", Action = "PayTest", id = RouteParameter.Optional }
);



            config.Routes.MapHttpRoute(
name: "FileInfoTest",
routeTemplate: "api/BanksApi/FileInfoTest",
defaults: new { Controller = "BanksApi", Action = "FileInfoTest", id = RouteParameter.Optional }
);




            config.Routes.MapHttpRoute(
name: "Get_Result",
routeTemplate: "api/BanksApi/Get_Result",
defaults: new { Controller = "BanksApi", Action = "Get_Result", id = RouteParameter.Optional }
);
            config.Routes.MapHttpRoute(
name: "Receive_Amount",
routeTemplate: "api/BanksApi/Receive_Amount",
defaults: new { Controller = "BanksApi", Action = "Receive_Amount", id = RouteParameter.Optional }
);




            //...............................Plots
            config.Routes.MapHttpRoute(
               name: "PlotsList",
               routeTemplate: "api/Files/GetPlots",
               defaults: new { Controller = "Files", Action = "GetPlots", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
            name: "PlotsDetail",
            routeTemplate: "api/Files/GetDetailofPlot",
            defaults: new { Controller = "Files", Action = "GetDetailofPlot", id = RouteParameter.Optional }
        );

            config.Routes.MapHttpRoute(
        name: "PlotsInstallmentDetail",
        routeTemplate: "api/Files/GetPlotsInstallment",
        defaults: new { Controller = "Files", Action = "GetPlotsInstallment", id = RouteParameter.Optional }
    );
        }
    }
}