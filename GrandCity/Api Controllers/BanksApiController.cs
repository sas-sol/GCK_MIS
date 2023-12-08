using MeherEstateDevelopers.Models;
using System;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using MeherEstateDevelopers.Controllers;
using System.Collections.Generic;

namespace MeherEstateDevelopers.Api_Controllers
{
    public class BanksApiController : BaseApiController
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        [HttpPost]
        public IHttpActionResult FileDetail([FromUri] string FileFormNumber)
        {
            try
            {
                CustomerFileInfoShort fc = new CustomerFileInfoShort();
                string FileFormNo = ApiModels.CustomerLogin.Dcrypt(FileFormNumber);
                var filejson = JsonConvert.DeserializeObject<CustomerFileInfoShort>(FileFormNo);
                if (!string.IsNullOrEmpty(FileFormNo.ToString()))
                {
                    var detailCount = db.Sp_Get_Bank_FileList(filejson.FileFormNumber).Count();
                    if (detailCount > 0)
                    {
                        var detail = db.Sp_Get_Bank_FileList(filejson.FileFormNumber).FirstOrDefault();
                        fc.FileFormNumber = detail.FileFormNumber.ToString();
                        fc.Name = detail.Name;
                        fc.Mobile = detail.Mobile_1;
                        fc.CNIC_NICOP = detail.CNIC_NICOP;
                        fc.Plot_Size = detail.Plot_Size;
                        fc.Phase = detail.Phase_Name;
                        fc.Block = detail.Block_Name;
                        fc.InstallmentAmount = Math.Round(detail.InstallmentAmount);
                        string json = JsonConvert.SerializeObject(fc);
                        string encrypteddata = ApiModels.CustomerLogin.Encrypt(json);
                        return Ok(encrypteddata);
                    }
                    else
                    {
                        return BadRequest("No Data Found");
                    }
                }
                else
                {
                    return BadRequest("File Number Required");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError("Technical Error!");
            }
        }
        //[HttpPost]
        //public IHttpActionResult PayAmount([FromUri]string data)
        //{
        //    try
        //    {
        //        CustomerReceipt cr = new CustomerReceipt();
        //        string result = ApiModels.CustomerLogin.Dcrypt(data);
        //        var Rdata = JsonConvert.DeserializeObject<CustomerReceipt>(result);
        //        if (Rdata.FileFormNumber > 0)
        //        {
        //            var filecustomerdetail = db.Sp_Get_Bank_FileList(Rdata.FileFormNumber).FirstOrDefault();
        //            if (filecustomerdetail != null)
        //            {
        //                if (Rdata.Amount >= filecustomerdetail.InstallmentAmount)
        //                {
        //                    string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Rdata.Amount));
        //                    string dev = "";
        //                    if (filecustomerdetail.Development_Charges == false)
        //                    {
        //                        dev = "Non Developemetn Charges";
        //                    }
        //                    else if (filecustomerdetail.Development_Charges == true)
        //                    {
        //                        dev = "With Developemetn Charges";
        //                    }
        //                    else
        //                    {
        //                        dev = "To Be Announce";
        //                    }
        //                    Helpers h = new Helpers();
        //                    long Tran = h.RandomNumber();
        //                    var res = db.Sp_Add_BankOnlineReceipt(Rdata.Amount, AmountinWords, Rdata.Bank, Rdata.Ch_Pay_Draft_No, Rdata.Ch_Pay_Draft_Date, Rdata.Branch_Name, filecustomerdetail.Mobile_1
        //                                , filecustomerdetail.Father_Husband, filecustomerdetail.FileFormNumber, filecustomerdetail.Name, "Online_" + Rdata.Payment_Type, 0,
        //                                "Meher Estate Developers", 0, null, filecustomerdetail.Plot_Size, ReceiptTypes.Installment.ToString(), 0, 0, "", null, Modules.FileManagement.ToString(), dev, filecustomerdetail.FileFormNumber.ToString(), filecustomerdetail.Block_Name, filecustomerdetail.Plot_Type, Rdata.ReqBank,Tran).FirstOrDefault();
        //                    var Newres = new { ReceiptNo = res };
        //                    var text = "Dear " + filecustomerdetail.Name + ",\n\r" +
        //                                "A Payment of Rs " + string.Format("{0:n0}", Rdata.Amount) + " has been received in cash for File number# " + filecustomerdetail.FileFormNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
        //                    try
        //                    {
        //                        SmsService smsService = new SmsService();
        //                        smsService.SendMsg(text, filecustomerdetail.Mobile_1);
        //                    }
        //                    catch (Exception)
        //                    {
        //                    }
        //                    string receiptjson = JsonConvert.SerializeObject(Newres);
        //                    string encryptedreceipts = ApiModels.CustomerLogin.Encrypt(receiptjson);
        //                    return Ok(encryptedreceipts);
        //                }
        //                else
        //                {
        //                    return BadRequest("Amount is Less than to Installment Amount");
        //                }
        //            }
        //            else
        //            {
        //                return BadRequest("Invalid File Form Number");
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest("File Number Required");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError("Technical Error! Please Contact the SA SYSTEM Team");
        //    }
        //}
        //...........................................These are the two apis of bank without encryption
        [HttpPost]
        public IHttpActionResult File_Detail(string FileFormNumber)
        {

            var detail = db.Sp_Get_Bank_FileList(FileFormNumber).FirstOrDefault();
            if (detail != null)
            {
                string Name = "", Mobile = "", CNIC = "";
                if (detail.Module == "FileManagement")
                {
                    var plot = db.Sp_Get_FileLastOwner(detail.File_Id).ToList();
                    Name = string.Join(" , ", plot.Select(x => x.Name));
                    Mobile = string.Join(" , ", plot.Select(x => x.Name));
                    CNIC = string.Join(" , ", plot.Select(x => x.Name));
                }
                else if (detail.Module == "PlotManagement")
                {
                    var plot = db.Sp_Get_PlotLastOwner(detail.File_Id).ToList();
                    Name = string.Join(" , ", plot.Select(x => x.Name));
                    Mobile = string.Join(" , ", plot.Select(x => x.Name));
                    CNIC = string.Join(" , ", plot.Select(x => x.Name));
                }
                else if (detail.Module == "PlotManagement")
                {
                    var plot = db.Sp_Get_CommercialLastOwner(detail.File_Id).ToList();
                    Name = string.Join(" , ", plot.Select(x => x.Name));
                    Mobile = string.Join(" , ", plot.Select(x => x.Name));
                    CNIC = string.Join(" , ", plot.Select(x => x.Name));
                }
                CustomerFileInfoShort fc = new CustomerFileInfoShort();
                fc.FileFormNumber = detail.FileFormNumber.ToString();
                fc.Name = Name;
                fc.Mobile = Mobile;
                fc.CNIC_NICOP = CNIC;
                fc.Plot_Size = detail.Plot_Size;
                fc.Phase = detail.Phase_Name;
                fc.Block = detail.Block_Name;
                fc.InstallmentAmount = Math.Round(detail.InstallmentAmount);
                return Ok(fc);
            }
            else
            {
                return BadRequest("No Data Found");
            }

        }
        [HttpPost]
        public IHttpActionResult Pay_Amount(string FileFormNumber, decimal Amount, string Bank, string Branch_Name, string Ch_Pay_Draft_No, DateTime? Ch_Pay_Draft_Date, string Type, string Payment_Type, string ReceiptNo, string ReqBank)
        {
            try
            {
                var filecustomerdetail = db.Sp_Get_Bank_FileList(FileFormNumber).FirstOrDefault();
                if (filecustomerdetail != null)
                {
                    string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Amount));
                    string dev = "";
                    if (filecustomerdetail.Development_Charges == false)
                    {
                        dev = "Non Developemetn Charges";
                    }
                    else if (filecustomerdetail.Development_Charges == true)
                    {
                        dev = "With Developemetn Charges";
                    }
                    else
                    {
                        dev = "To Be Announce";
                    }
                    Helpers h = new Helpers();
                    long Tran = h.RandomNumber();
                    var recepno = db.Sp_Get_ReceiptNo(ReqBank).FirstOrDefault();
                    var res = db.Sp_Add_BankOnlineReceipt(Amount, AmountinWords, Bank, Ch_Pay_Draft_No, Ch_Pay_Draft_Date, Branch_Name, filecustomerdetail.Mobile_1
                                , filecustomerdetail.Father_Husband, filecustomerdetail.File_Id, filecustomerdetail.Name, "Online_" + Payment_Type, 0,
                                "Meher Estate Developers", 0, null, filecustomerdetail.Plot_Size, ReceiptTypes.Installment.ToString(), 0, 0, "", null, Modules.FileManagement.ToString(), dev, filecustomerdetail.FileFormNumber.ToString(), filecustomerdetail.Block_Name, filecustomerdetail.Plot_Type, ReqBank, ReqBank + "-" + recepno, Tran).FirstOrDefault();
                    if (res == "Exists")
                    {
                        return BadRequest("Invalid File Form Number");
                    }
                    var filedata = db.File_Form.Where(x => x.FileFormNumber == filecustomerdetail.FileFormNumber).FirstOrDefault();
                    if (filedata.BallotedPlot_Id == null)
                    {
                        var re = db.Sp_Update_FileVerificationToNull(filecustomerdetail.File_Id);
                    }
                    else
                    {
                        var re = db.SP_Update_PlotVerificationToNull(filedata.BallotedPlot_Id);
                    }
                    var Newres = new { ReceiptNo = res };
                    var text = "Dear " + filecustomerdetail.Name + ",\n\r" +
                                "A Payment of Rs " + string.Format("{0:n0}", Amount) + " has been received in cash for File number# " + filecustomerdetail.FileFormNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                    try
                    {
                        SmsService smsService = new SmsService();
                        smsService.SendMsg(text, filecustomerdetail.Mobile_1);
                    }
                    catch (Exception)
                    {
                    }
                    return Ok(Newres);
                }
                else
                {
                    return BadRequest("Invalid File Form Number");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError("Technical Error! Please Contact the SA SYSTEM Team");
            }
        }
        //............These are the two apis of bank without encryption
        //[HttpPost]
        //public IHttpActionResult FileInfoTest(long FileFormNumber)
        //{
        //    var detail = db.Sp_Get_Bank_FileList_Test(FileFormNumber).FirstOrDefault();
        //    if (detail != null)
        //    {
        //        CustomerFileInfoShort fc = new CustomerFileInfoShort();
        //        fc.FileFormNumber = detail.FileFormNumber.ToString();
        //        fc.Name = detail.Name;
        //        fc.Mobile = detail.Mobile_1;
        //        fc.CNIC_NICOP = detail.CNIC_NICOP;
        //        fc.Plot_Size = detail.Plot_Size;
        //        fc.Phase = detail.Phase_Name;
        //        fc.Block = detail.Block_Name;
        //        fc.InstallmentAmount = Math.Round(detail.InstallmentAmount);
        //        var data = new { Status = 1, Data = fc };
        //        return Ok(data);
        //    }
        //    else
        //    {
        //        var data = new { Status = 2, Data = "No Data Found" };
        //        return Ok(data);
        //    }
        //}
        //[HttpPost]
        //public IHttpActionResult PayTest(long FileFormNumber, decimal Amount, string Bank, string Branch_Name, string Ch_Pay_Draft_No, DateTime? Ch_Pay_Draft_Date, string Type, string Payment_Type, string ReceiptNo, string ReqBank, string Transaction_Id)
        //{
        //    try
        //    {
        //        var filecustomerdetail = db.Sp_Get_Bank_FileList_Test(FileFormNumber).FirstOrDefault();
        //        if (filecustomerdetail != null)
        //        {
        //            string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Amount));
        //            string Devch = (filecustomerdetail.Development_Charges) ? "With Development Charges" : "Non Development Charges";
        //            var res = db.Sp_Add_BankOnlineReceipt_Test(Amount, AmountinWords, Bank, Ch_Pay_Draft_No, Ch_Pay_Draft_Date, Branch_Name, filecustomerdetail.Mobile_1
        //                        , filecustomerdetail.Father_Husband, filecustomerdetail.FileFormNumber, filecustomerdetail.Name, "Online_" + Payment_Type, 0,
        //                        "Meher Estate Developers", 0, null, filecustomerdetail.Plot_Size, ReceiptTypes.Installment.ToString(), 0, 0, "", null, Modules.FileManagement.ToString(), Devch, filecustomerdetail.FileFormNumber.ToString(), "Sher Afghan", ReqBank, Transaction_Id).FirstOrDefault();
        //            if (res == "Exists")
        //            {
        //                var data1 = new { Status = 4, Data = "Transaction ID Already Exists" };
        //                return Ok(data1);
        //            }
        //            var Newres = new { ReceiptNo = res };
        //            var text = "Dear " + filecustomerdetail.Name + ",\n\r" +
        //                        "A Payment of Rs " + string.Format("{0:n0}", Amount) + " has been received in cash for File number" + filecustomerdetail.FileFormNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
        //            try
        //            {
        //                SmsService smsService = new SmsService();
        //                smsService.SendMsg(text, filecustomerdetail.Mobile_1);
        //            }
        //            catch (Exception)
        //            {
        //            }
        //            var data = new { Status = 5, Data = res };
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            var data = new { Status = 2, Data = "No Data Found" };
        //            return Ok(data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var data = new { Status = 0, Data = "Technical Error! Please Contact the SA SYSTEM Team" };
        //        return Ok(data);
        //    }
        //}
        // For HBL
        [HttpPost]
        public IHttpActionResult Get_Result(string FileFormNumber)
        {
            var detail = db.Sp_Get_Bank_FileList(FileFormNumber).FirstOrDefault();
            if (detail != null)
            {

                CustomerFileInfoShort fc = new CustomerFileInfoShort();
                fc.FileFormNumber = detail.FileFormNumber.ToString();
                fc.Name = detail.Name;
                fc.Mobile = detail.Mobile_1;
                fc.CNIC_NICOP = detail.CNIC_NICOP;
                fc.Plot_Size = detail.Plot_Size;
                fc.Phase = detail.Phase_Name;
                fc.Block = detail.Block_Name;
                fc.InstallmentAmount = Math.Round(detail.InstallmentAmount);
                var data = new { Status = 1, Data = fc };
                return Ok(data);
            }
            else
            {
                var data = new { Status = 2, Data = "No Data Found" };
                return Ok(data);
            }
        }
        [HttpPost]
        public IHttpActionResult Receive_Amount(string FileFormNumber, decimal Amount, string Bank, string Branch_Name, string Ch_Pay_Draft_No, DateTime? Ch_Pay_Draft_Date, string Type, string Payment_Type, string ReceiptNo, string ReqBank, string Transaction_Id)
        {
            try
            {
                var filecustomerdetail = db.Sp_Get_Bank_FileList(FileFormNumber).FirstOrDefault();
                if (filecustomerdetail != null)
                {
                    string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Amount));
                    var recepno = db.Sp_Get_ReceiptNo(ReqBank).FirstOrDefault();
                    var res = db.Sp_Add_BankOnlineReceipt(Amount, AmountinWords, Bank, Ch_Pay_Draft_No, Ch_Pay_Draft_Date, Branch_Name, filecustomerdetail.Mobile_1
                                , filecustomerdetail.Father_Husband, filecustomerdetail.File_Id  /* File Id */ , filecustomerdetail.Name, "Online_" + Payment_Type, 0,
                                "Meher Estate Developers", 0, null, filecustomerdetail.Plot_Size,
                                ReceiptTypes.Installment.ToString(), 0, 0, "", null,
                                filecustomerdetail.Module, "", filecustomerdetail.FileFormNumber.ToString(),
                                 filecustomerdetail.Block_Name, filecustomerdetail.Plot_Type, ReqBank, ReqBank + "-" + recepno, Convert.ToInt64(Transaction_Id)).FirstOrDefault();
                    if (res == "Exists")
                    {
                        var data1 = new { Status = 4, Data = "Transaction ID Already Exists" };
                        return Ok(data1);
                    }
                    var filedata = db.File_Form.Where(x => x.FileFormNumber == filecustomerdetail.FileFormNumber).FirstOrDefault();
                    if (filedata.BallotedPlot_Id == null)
                    {
                        var re = db.Sp_Update_FileVerificationToNull(filecustomerdetail.File_Id);
                    }
                    else
                    {
                        var re = db.SP_Update_PlotVerificationToNull(filedata.BallotedPlot_Id);
                    }
                    var Newres = new { ReceiptNo = res };
                    var text = "Dear " + filecustomerdetail.Name + ",\n\r" +
                                "A Payment of Rs " + string.Format("{0:n0}", Amount) + " has been received in cash for File number" + filecustomerdetail.FileFormNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                    try
                    {
                        SmsService smsService = new SmsService();
                        smsService.SendMsg(text, filecustomerdetail.Mobile_1);
                    }
                    catch (Exception)
                    {
                    }
                    var data = new { Status = 5, Data = res };
                    return Ok(data);
                }
                else
                {
                    var data = new { Status = 2, Data = "No Data Found" };
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                var data = new { Status = 0, Data = "Technical Error! Please Contact the SA SYSTEM Team" };
                return Ok(data);
            }
        }
    }
}
