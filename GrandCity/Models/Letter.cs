using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeherEstateDevelopers.Models
{
    public class Letter
    {
        public interface ILetterGenerationStrategy
        {
            string GenerateLetter(long Id , string Module);
        }
        public class AllotmentLetterStrategy : ILetterGenerationStrategy
        {
            public string GenerateLetter(long Id, string Module)
            {
                using (var db = new Grand_CityEntities())
                {
                    long userid = long.Parse(HttpContext.Current.User.Identity.GetUserId());

                    var res = db.Sp_Get_AllotmentLetter(Id).SingleOrDefault();

                    // Assuming 'Sp_Get_AllotmentLetter' returns a single object, not a collection
                    if (res != null)
                    {
                        string message = $"Access Allotment Letter of Shop/ Office/ Apartment No. {res.Plot_No} Block No: {res.Block}";
                        db.Sp_Add_Activity(userid, message, "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Id);

                        // Serialize the result to JSON
                        string resJson = JsonConvert.SerializeObject(res);

                        return resJson;
                    }
                    else
                    {
                        // Handle case when no result is returned
                        return null;
                    }
                }
            }
        }
        public class CancellationLetterstrategy : ILetterGenerationStrategy
        {
            public string GenerateLetter(long Id , string Module)
            {
                using (var db = new Grand_CityEntities())
                {
                    string resJson = "";
                    long userid = long.Parse(HttpContext.Current.User.Identity.GetUserId());
                    if(Module == "PlotManagement")
                    { 
                        db.Sp_Add_Activity(userid, "Print Cancellation Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                        db.Sp_Add_PlotComments(Id, "Print Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
                        //var res = db.Sp_Get_OverdueQualifying_Plots_Id(Id).Select(x => new OverdueQualifying_Plots
                        //{
                        //    Area = x.Area,
                        //    Block_Name = x.Block_Name,
                        //    Develop_Status = x.Develop_Status,
                        //    Dimension = x.Dimension,
                        //    Id = x.Id,
                        //    Name = x.Name,
                        //    OverDueAmount = x.Balance_Amount,
                        //    Plot_Location = x.Plot_Location,
                        //    Plot_No = x.Plot_No,
                        //    Plot_Size = x.Plot_Size,
                        //    Road_Type = x.Road_Type,
                        //    Status = x.Status,
                        //    Type = x.Type,
                        //    Verified = x.Verified,
                        //    Mobile_1 = x.Mobile_1,
                        //    Owner_Id = x.Owner_Id,
                        //    Verification_Req = x.Verification_Req,
                        //    FirstNotice = x.First_Notice,
                        //    SecNotice = x.Sec_Notice,
                        //    CancelNotice = x.Cancel_Notice,
                        //    Installments = x.Installments,
                        //    Currency_Note_No = x.Currency_Note_No
                        //}).FirstOrDefault();
                        var res = db.Plot_Ownership.FirstOrDefault(x => x.Id == Id);
                        // Serialize the result to JSON
                        resJson = JsonConvert.SerializeObject(res);
                    }
                    else if (Module == "FileManagement")
                    {
                        db.Sp_Add_Activity(userid, "Print Cancellation Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                        db.Sp_Add_FileComments(Id, "Print Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
                        Sp_Get_CurrentOwner_File_Id_Result res = db.Sp_Get_CurrentOwner_File_Id(Id).FirstOrDefault();
                        resJson = JsonConvert.SerializeObject(res);
                    }
                    return resJson;
                }
            }
        }
        public class FirstWarningLetterStrategy : ILetterGenerationStrategy  //plot
        {
            public string GenerateLetter(long Id, string Module)
            {
                using (var db = new Grand_CityEntities())
                {
                    string resJson = "";
                    long userid = long.Parse(HttpContext.Current.User.Identity.GetUserId());
                    if (Module == "PlotManagement")
                    {
                        db.Sp_Add_PlotComments(Id, "Print First Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                        db.Sp_Add_Activity(userid, "Print First Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                        //var res = db.Sp_Get_OverdueQualifying_Plots_Id(Id).Select(x => new OverdueQualifying_Plots
                        //{
                        //    Area = x.Area,
                        //    Block_Name = x.Block_Name,
                        //    Develop_Status = x.Develop_Status,
                        //    Dimension = x.Dimension,
                        //    Id = x.Id,
                        //    Name = x.Name,
                        //    OverDueAmount = x.Balance_Amount,
                        //    Plot_Location = x.Plot_Location,
                        //    Plot_No = x.Plot_No,
                        //    Plot_Size = x.Plot_Size,
                        //    Road_Type = x.Road_Type,
                        //    Status = x.Status,
                        //    Type = x.Type,
                        //    Verified = x.Verified,
                        //    Mobile_1 = x.Mobile_1,
                        //    Owner_Id = x.Owner_Id,
                        //    Verification_Req = x.Verification_Req,
                        //    FirstNotice = x.First_Notice,
                        //    SecNotice = x.Sec_Notice,
                        //    CancelNotice = x.Cancel_Notice,
                        //    PostalAddress = x.Postal_Address,
                        //    Installments = x.Installments,
                        //    Currency_Note_No = x.Currency_Note_No
                        //}).FirstOrDefault();
                        var res = db.Plot_Ownership.FirstOrDefault(x => x.Id == Id);
                        resJson = JsonConvert.SerializeObject(res);
                    }
                    else if (Module == "FileManagement")
                    {
                        db.Sp_Add_FileComments(Id, "Print First Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                        db.Sp_Add_Activity(userid, "Print First Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                        var res = db.Sp_Get_LastOwners_File_Id(Id).FirstOrDefault();
                        resJson = JsonConvert.SerializeObject(res);
                    }
                    return resJson;
                }
            }
        }
        public class SecondWarningLetterstrategy : ILetterGenerationStrategy
        {
            public string GenerateLetter(long Id, string Module)
            {
                using (var db = new Grand_CityEntities())
                {
                    string resJson = "";
                    long userid = long.Parse(HttpContext.Current.User.Identity.GetUserId());
                    if (Module == "PlotManagement")
                    {
                        db.Sp_Add_Activity(userid, "Print Second Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                        db.Sp_Add_PlotComments(Id, "Print Second Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                        //var res = db.Sp_Get_OverdueQualifying_Plots_Id(Id).Select(x => new OverdueQualifying_Plots
                        //{
                        //    Area = x.Area,
                        //    Block_Name = x.Block_Name,
                        //    Develop_Status = x.Develop_Status,
                        //    Dimension = x.Dimension,
                        //    Id = x.Id,
                        //    Name = x.Name,
                        //    OverDueAmount = x.Balance_Amount,
                        //    Plot_Location = x.Plot_Location,
                        //    Plot_No = x.Plot_No,
                        //    Plot_Size = x.Plot_Size,
                        //    Road_Type = x.Road_Type,
                        //    Status = x.Status,
                        //    Type = x.Type,
                        //    Verified = x.Verified,
                        //    Mobile_1 = x.Mobile_1,
                        //    Owner_Id = x.Owner_Id,
                        //    Verification_Req = x.Verification_Req,
                        //    FirstNotice = x.First_Notice,
                        //    SecNotice = x.Sec_Notice,
                        //    CancelNotice = x.Cancel_Notice,
                        //    Installments = x.Installments,
                        //    Currency_Note_No = x.Currency_Note_No
                        //}).FirstOrDefault();
                        var res = db.Plot_Ownership.FirstOrDefault(x => x.Id == Id);
                        resJson = JsonConvert.SerializeObject(res);
                    }
                    else if (Module == "FileManagement")
                    {
                        db.Sp_Add_Activity(userid, "Print Second Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                        db.Sp_Add_FileComments(Id, "Print Second Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                        Sp_Get_CurrentOwner_File_Id_Result res = db.Sp_Get_CurrentOwner_File_Id(Id).FirstOrDefault();
                        resJson = JsonConvert.SerializeObject(res);
                    }
                    return resJson;
                   
                }
            }
        }
        public class ThirdWarningLetterStrategy : ILetterGenerationStrategy  //plot
        {
            public string GenerateLetter(long Id, string Module)
            {
                using (var db = new Grand_CityEntities())
                {
                    string resJson = "";
                    long userid = long.Parse(HttpContext.Current.User.Identity.GetUserId());
                    if (Module == "PlotManagement")
                    {
                        db.Sp_Add_PlotComments(Id, "Print Third Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                        db.Sp_Add_Activity(userid, "Print Third Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                        //var res = db.Sp_Get_OverdueQualifying_Plots_Id(Id).Select(x => new OverdueQualifying_Plots
                        //{
                        //    Area = x.Area,
                        //    Block_Name = x.Block_Name,
                        //    Develop_Status = x.Develop_Status,
                        //    Dimension = x.Dimension,
                        //    Id = x.Id,
                        //    Name = x.Name,
                        //    OverDueAmount = x.Balance_Amount,
                        //    Plot_Location = x.Plot_Location,
                        //    Plot_No = x.Plot_No,
                        //    Plot_Size = x.Plot_Size,
                        //    Road_Type = x.Road_Type,
                        //    Status = x.Status,
                        //    Type = x.Type,
                        //    Verified = x.Verified,
                        //    Mobile_1 = x.Mobile_1,
                        //    Owner_Id = x.Owner_Id,
                        //    Verification_Req = x.Verification_Req,
                        //    FirstNotice = x.First_Notice,
                        //    SecNotice = x.Sec_Notice,
                        //    CancelNotice = x.Cancel_Notice,
                        //    PostalAddress = x.Postal_Address,
                        //    Installments = x.Installments,
                        //    Currency_Note_No = x.Currency_Note_No
                        //}).FirstOrDefault();
                        var res = db.Plot_Ownership.FirstOrDefault(x => x.Id == Id);
                        resJson = JsonConvert.SerializeObject(res);
                    }
                    else if (Module == "FileManagement")
                    {
                        db.Sp_Add_Activity(userid, "Print Third Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                        db.Sp_Add_FileComments(Id, "Print Third Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                        Sp_Get_CurrentOwner_File_Id_Result res = db.Sp_Get_CurrentOwner_File_Id(Id).FirstOrDefault();
                        resJson = JsonConvert.SerializeObject(res);
                    }
                    return resJson;
                   
                }
            }
        }
    
        public class LetterGeneratorTemplate
        {
            protected ILetterGenerationStrategy strategy;
            protected string letterType;

            public LetterGeneratorTemplate()
            {

            }
            public LetterGeneratorTemplate(ILetterGenerationStrategy strategy, string letterType)
            {
                this.strategy = strategy;
                this.letterType = letterType;
            }

            public string GenerateLetter(long id , string Module)
            {
                string jsonResult = "";
                if (letterType == "AllotmentLetter")// id=1
                {
                    var allotmentLetterStrategy = new AllotmentLetterStrategy();
                    jsonResult = allotmentLetterStrategy.GenerateLetter(id,Module);
                   
                }
                else if (letterType == "CancellationLetter")   //id= 1
                {
                   var CancellationLetterstrategy = new CancellationLetterstrategy();
                   jsonResult = CancellationLetterstrategy.GenerateLetter(id,Module);
                    
                }
                else if (letterType == "FirstWarningLetter")
                {
                   var FirstWarningLetterStrategy = new FirstWarningLetterStrategy();
                   jsonResult = FirstWarningLetterStrategy.GenerateLetter(id,Module);
                   
                }
                else if (letterType == "SecondWarningLetter")   //id= 1
                {
                    var SecondWarningLetterstrategy = new SecondWarningLetterstrategy();
                    jsonResult = SecondWarningLetterstrategy.GenerateLetter(id,Module);
                }
                else if (letterType == "ThirdWarningLetter")   //id= 1
                {
                    var ThirdWarningLetterstrategy = new ThirdWarningLetterStrategy();
                    jsonResult = ThirdWarningLetterstrategy.GenerateLetter(id,Module);
                   
                }
                else
                {
                    var results = "Invalid Letter Type";
                    return results;
                }
                return jsonResult;

                // Additional processing like merging with templates, adding images, etc.


            }
        }
    }
}