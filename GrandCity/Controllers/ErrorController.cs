using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNet.Identity;



namespace MeherEstateDevelopers.Controllers
{
    public class ErrorController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Error404()
        {
            // Return the Error404 view
            return View();
        }
        public ActionResult Error400()
        {
            // Return the Error404 view
            return View();
        }
        public ActionResult Error500()
        {
            // Return the Error404 view
            return View();
        }
       
        [HttpPost]
        public ActionResult ReportError(ErrorLog errorData)
        {
            try
            {
                int userid = int.Parse(User.Identity.GetUserId());
                errorData.UserId = userid;

                using (SHA256 sha256Hash = SHA256.Create())
                {
                    string dataToHash = (errorData.ActionName ?? "") + (errorData.ControllerName ?? "") + (errorData.Exception ?? "") + (errorData.UserId ?? 0);

                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        Console.WriteLine($"Byte {i}: {bytes[i]}");
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    errorData.ErrorDataHash = builder.ToString();
                }

                // Check if the same hash exists in the database
                var existingErrorLog = db.ErrorLogs.FirstOrDefault(e => e.ErrorDataHash == errorData.ErrorDataHash);
                if (existingErrorLog != null)
                {
                    return Json(new { success = true, message = "The error has been reported. Your tracking Id is " + existingErrorLog.Id });
                }
                else
                {
                    errorData.DateTime = DateTime.UtcNow;

                    db.ErrorLogs.Add(errorData);
                    db.SaveChanges();
                    int insertedId = errorData.Id;

                    return Json(new { success = true, message = "The error has been reported. Your tracking Id is " + insertedId });

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "There seems to be a Problem", error = ex.Message });
            }
        }
    }
}