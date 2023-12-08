using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using MeherEstateDevelopers.ApiModels;
using MeherEstateDevelopers.Models;

namespace MeherEstateDevelopers.Api_Controllers
{
    public class AccountsController : ApiController
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        [HttpPost]
        public IHttpActionResult CustomerLogin(string SystemKey, string Name, string FatherHusband, string Email, string Phone, string Cnic, string Password)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        Helpers h = new Helpers();
                        ApiModels.CustomerLogin p = new ApiModels.CustomerLogin();
                        p.Add(Name, FatherHusband, Email, Phone, Cnic, Password);
                        var updateemail = db.Sp_Update_FilesTransferEmail(Email, Phone, Cnic);
                        db.API_Sp_update_PlotOwnershipEmail(Email, Cnic);
                        db.Sp_Get_User_Login_Token(Email, h.RandomNumber());
                        var info = db.Sp_Get_LoginInformation(Phone);
                        return Ok(info);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult CustomerRegisteration(string SystemKey, string Name, string FatherHusband, string Email, string Phone, string Cnic, string Password)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var res1 = db.Files_Transfer.Any(x => x.Mobile_1 == Phone && x.CNIC_NICOP == Cnic);
                        var res2 = db.Plot_Ownership.Any(x => x.Mobile_1 == Phone && x.CNIC_NICOP == Cnic);
                        if (res1 || res2)
                        {
                            ApiModels.CustomerLogin p = new ApiModels.CustomerLogin();
                            p.Register(Name, FatherHusband, Email, Phone, Cnic, Password);
                            var info = db.Sp_Get_CustomerRegisterationInformation(Phone);
                            return Ok(info);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult EmailCheckValidate(string SystemKey, string Email)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var s = db.Sp_Check_LoginEmail(Email);
                        return Ok(s);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult MobileCheckValidate(string SystemKey, string Mobile)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var s = db.Sp_Check_LoginMobile(Mobile);
                        return Ok(s);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult Customer_Login(string SystemKey, string Email, string Password)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        Helpers h = new Helpers();
                        
                        var s = db.Sp_Get_User_Login_Info(Email).ToList();
                        string pas = s.First().Password;
                        string m = s.FirstOrDefault().Email;
                        string pp = ApiModels.CustomerLogin.Dcrypt(pas);
                        if (m.Equals(Email) && pp.Equals(Password))
                        {
                            db.Sp_Get_User_Login_Token(Email, h.RandomNumber());
                            s = db.Sp_Get_User_Login_Info(Email).ToList();
                            return Ok(s);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult Customer_Login_Information(string SystemKey, string Phone)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var s = db.Sp_Get_LoginInformation(Phone).ToList();
                        return Ok(s);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult Family_Customer_Login_Information(string SystemKey, string Phone)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var s = db.Sp_Get_CustomerRegisterationInformation(Phone).ToList();
                        return Ok(s);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult Family_Information(string SystemKey, string FileFormNumber, string Cnic)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherEstateDevelopers";
                    if (key.Equals(SystemKey))
                    {
                        string Family_Password = string.Empty;
                        long fid = Convert.ToInt64(FileFormNumber);
                        var f = db.Sp_Get_FamilyInfo(fid, Cnic).ToList();
                        if (f.Count > 0)
                        {
                            string Phone = f.FirstOrDefault().Mobile_1;
                            string Cnic_Nic = f.FirstOrDefault().CNIC_NICOP;
                            string Name = f.FirstOrDefault().Name;
                            string FatherHusband = f.FirstOrDefault().Father_Husband;
                            string Email = f.FirstOrDefault().Email;
                            string Password = "MeherEstateDevelopers";
                            var s = db.Sp_Get_LoginInformation(Phone).ToList();
                            if (!s.Any())
                            {
                                ApiModels.CustomerLogin p = new ApiModels.CustomerLogin();
                                p.Register(Name, FatherHusband, Email, Phone, Cnic, Password);
                                // var info = db.Sp_Get_LoginInformation(Phone);
                                return Ok(f);
                            }
                            else
                            {
                                ApiModels.CustomerLogin c = new ApiModels.CustomerLogin();
                                var t = c.OTP_Number();
                                var uotp = db.Sp_Update_CustomerOtp(t.ToString(), Email, Phone);
                                SmsService ss = new SmsService();
                                string otp = t.ToString();
                                string message = "This is an automated SMS from website / app url Your verification pin is "+otp+".Please enter the PIN for instant verification. Thank you!";
 
                                 ss.SendMsg(message, Phone);
                                return Ok(f);
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult ForgetPassword(string SystemKey, string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var getemailcount = db.Sp_Get_ForgotPassword(email).ToList().Count;
                        if (getemailcount > 0)
                        {
                            var get = db.Sp_Get_ForgotPassword(email).ToList();
                            var mobile = get.FirstOrDefault().Phone;
                            var password = get.FirstOrDefault().Password;
                            string pass = ApiModels.CustomerLogin.Dcrypt(password);
                            SmsService sms = new SmsService();
                            sms.SendMsg("Your Forgot Password is " + pass, mobile);
                            return Ok(get);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult ChangePassword(string SystemKey, string email, string OldPassword, string NewPassword)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var getemailcount = db.Sp_Get_ForgotPassword(email).ToList().Count;
                        var gett = db.Sp_Get_ForgotPassword(email).ToList();
                        var passwd = gett.FirstOrDefault().Password;
                        string passd = ApiModels.CustomerLogin.Dcrypt(passwd);
                        if (passd.Equals(OldPassword))
                        {
                            if (getemailcount > 0)
                            {
                                var get = db.Sp_Get_ForgotPassword(email).ToList();
                                var mobile = get.FirstOrDefault().Phone;
                                var password = get.FirstOrDefault().Password;
                                string pass = ApiModels.CustomerLogin.Dcrypt(password);
                                string oldpass = ApiModels.CustomerLogin.Encrypt(OldPassword);
                                string get_old_pass = ApiModels.CustomerLogin.Dcrypt(oldpass);
                                if (get_old_pass.Equals(pass))
                                {
                                    string newencryptedpass = ApiModels.CustomerLogin.Encrypt(NewPassword);
                                    var update = db.Sp_Update_CustomerPassword(newencryptedpass, email, mobile);
                                    var getdetail = db.Sp_Get_ForgotPassword(email).ToList();
                                    return Ok(getdetail);
                                }
                                else
                                {
                                    return NotFound();
                                }
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult GetSupportComplains(string SystemKey, string Phone, string CNIC, string Token)
        {
            var token = db.CustomerLogins.Any(x => x.Token == Token && !string.IsNullOrEmpty(x.Token));
            if (!token)
            {
                return InternalServerError();
            }
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var f = db.Tickets.Where(x => x.Customer == true && x.Mobile_No == Phone).Select(x=> new { x.Id, x.Title, x.Description, x.Ticket_No, x.Status, x.Datetime}).ToList();
                        return Ok(f);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }

            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult AddSupportComplains(string SystemKey, string Name, string Phone, string Title, string Description)
        {
            try
            {
                if (!string.IsNullOrEmpty(SystemKey))
                {
                    string key = "MeherESTATE";
                    if (key.Equals(SystemKey))
                    {
                        var dep = db.Comp_Dep_Desig.Where(x => x.Type == "Department" && x.Name == "Customer Care").FirstOrDefault();
                        var res = db.Sp_Add_Ticket_Customers(Description, dep.Name, dep.Id, Title, true, Name, Phone, "").FirstOrDefault();
                        if (res.Ticket_Id > 0)
                        {
                            return Ok(res.Ticket_No);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
