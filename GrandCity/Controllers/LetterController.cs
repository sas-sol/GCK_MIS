using MeherEstateDevelopers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MeherEstateDevelopers.Models.Letter;

namespace MeherEstateDevelopers.Controllers
{
    public class LetterController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        private readonly ILetterGenerationStrategy letterStrategy;
        private readonly string letterType;
        private readonly long Id;
        public LetterController(ILetterGenerationStrategy letterStrategy, string letterType, long Id)
        {
            this.letterStrategy = letterStrategy;
            this.letterType = letterType;
            this.Id = Id;
        }
        public LetterController()
        {

        }
        public ActionResult GenerateLetter(string letterType, long Id, string Module)
        {
            // Use the injected letterStrategy instead of instantiating LetterGeneratorTemplate directly
            var generator = new LetterGeneratorTemplate(letterStrategy, letterType);
            string jsonResult = generator.GenerateLetter(Id,Module);
            if (letterType == "AllotmentLetter")  //id= 1
            {
                Sp_Get_AllotmentLetter_Result model = JsonConvert.DeserializeObject<Sp_Get_AllotmentLetter_Result>(jsonResult);
                // Pass the model to the view
                return View("~/Views/commercial/AllotmentLetter.cshtml", model);
            }
            else if (letterType == "CancellationLetter")   //id= 1
            {
                if(Module== "PlotManagement")
                { 
                    OverdueQualifying_Plots model = JsonConvert.DeserializeObject<OverdueQualifying_Plots>(jsonResult);
                    ViewBag.letterType = "CancellationLetter";
                    return View("~/Views/Plots/FirstWarningLetter.cshtml", model);
                }
                else if (Module == "FileManagement")
                {
                    Sp_Get_CurrentOwner_File_Id_Result model = JsonConvert.DeserializeObject<Sp_Get_CurrentOwner_File_Id_Result>(jsonResult);
                    ViewBag.letterType = "CancellationLetter";
                    return View("~/Views/FileSystem/CancellationLetter.cshtml", model);
                }
           
            }
            else if (letterType == "FirstWarningLetter")   //id= 1  //plots
            {
                if (Module == "PlotManagement")
                {
                    OverdueQualifying_Plots model = JsonConvert.DeserializeObject<OverdueQualifying_Plots>(jsonResult);
                    // Pass the model to the view
                    ViewBag.subject = "1st Reminder Notice on Overdue Installments";
                    ViewBag.msg = "1st REMINDER NOTICE";
                    return View("~/Views/Plots/FirstWarningLetter.cshtml", model);
                }
                else if (Module == "FileManagement")
                {
                    Sp_Get_LastOwners_File_Id_Result model = JsonConvert.DeserializeObject<Sp_Get_LastOwners_File_Id_Result>(jsonResult);
                    ViewBag.subject = "1st Reminder Notice on Overdue Installments";
                    ViewBag.msg = "1st REMINDER NOTICE";
                    return View("~/Views/FileSystem/FirstWarningLetter.cshtml", model);
                }
              
            }
            else if (letterType == "SecondWarningLetter")   //id= 1
            {
                if (Module == "PlotManagement")
                {
                    OverdueQualifying_Plots model = JsonConvert.DeserializeObject<OverdueQualifying_Plots>(jsonResult);
                    ViewBag.subject = "2nd Reminder Notice on Overdue Installments";
                    ViewBag.msg = "2nd REMINDER NOTICE";
                    return View("~/Views/Plots/FirstWarningLetter.cshtml", model);
                }
                else if (Module == "FileManagement")
                {
                    Sp_Get_CurrentOwner_File_Id_Result model = JsonConvert.DeserializeObject<Sp_Get_CurrentOwner_File_Id_Result>(jsonResult);
                    ViewBag.subject = "2nd Reminder Notice on Overdue Installments";
                    ViewBag.msg = "2nd REMINDER NOTICE";
                    return View("~/Views/FileSystem/SecondWarningLetter.cshtml", model);
                }
               
            }
            else if (letterType == "ThirdWarningLetter")   //id= 1
            {
                if (Module == "PlotManagement")
                {
                    OverdueQualifying_Plots model = JsonConvert.DeserializeObject<OverdueQualifying_Plots>(jsonResult);
                    ViewBag.subject = "3rd Reminder Notice on Overdue Installments";
                    ViewBag.msg = "3rd REMINDER NOTICE";
                    return View("~/Views/Plots/FirstWarningLetter.cshtml", model);
                }
                else if (Module == "FileManagement")
                {
                    Sp_Get_CurrentOwner_File_Id_Result model = JsonConvert.DeserializeObject<Sp_Get_CurrentOwner_File_Id_Result>(jsonResult);
                    ViewBag.subject = "3rd Reminder Notice on Overdue Installments";
                    ViewBag.msg = "3rd REMINDER NOTICE";
                    return View("~/Views/FileSystem/SecondWarningLetter.cshtml", model);
                }
              
            }


            return View();

        }
    }
}