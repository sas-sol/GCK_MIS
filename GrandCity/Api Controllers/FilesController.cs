using MeherEstateDevelopers.Controllers;
using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace MeherEstateDevelopers.Api_Controllers
{
    public class FilesController : ApiController
    {
        private Grand_CityEntities db = new Grand_CityEntities();


        [HttpPost]
        public IHttpActionResult GetFiles(string SystemKey, string Phone, string CNIC, string Token)
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
                        var f = db.Sp_Get_Files_Lists(Phone, CNIC);
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
        public IHttpActionResult AddFamilyFiles(string SystemKey, string Phone, string FileFormNumber, string Token, string Cust_ID = "", string Cust_Phone = "")
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
                        var f = db.Sp_Get_FamilyFiles_List(Phone, FileFormNumber).ToList();
                        var name = f.FirstOrDefault().Name;
                        var phone = f.FirstOrDefault().Mobile_1;
                        var father_husband = f.FirstOrDefault().Father_Husband;
                        var cnic = f.FirstOrDefault().CNIC_NICOP;
                        var custid = Convert.ToInt32(Cust_ID);
                        var fileid = f.FirstOrDefault().File_Form_Id;
                        var address = f.FirstOrDefault().Postal_Address;
                        var familylistcount = db.Sp_Get_FamilyProperties(custid, Convert.ToInt64(FileFormNumber)).Count();
                        if (familylistcount > 0)
                        {
                            var deletedfamily = db.Sp_Delete_FamilyProperty(custid, Convert.ToInt64(FileFormNumber));
                        }
                        var fm = db.Sp_Add_FamilyProperties(name, father_husband, phone, cnic, FileFormNumber, fileid.ToString(), custid, Cust_Phone, address);

                        // var familylist = db.Sp_Get_FamilyProperties(custid);

                        return Ok("Successfully Saved");
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
        /// <summary>
        /// This is use to Get the Family Property Files if the OTP Verifies it
        /// </summary>
        /// <param name="SystemKey"></param>
        /// <param name="Cust_ID"></param>
        /// <param name="FamilyOtp"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetFamilyFiles(string SystemKey, string Cust_ID, string FamilyOtp, string mobile, string FileFormNumber, string CNIC, string Token)
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
                        //var s = db.Sp_Get_LoginInformation(mobile).ToList();Sp_Get_CustomerLoginInformation
                        var ss = db.Sp_Get_RegisterCustomerInformation(mobile, CNIC).ToList();
                        var fotp = ss.FirstOrDefault().OTP;
                        var otp = Convert.ToInt32(FamilyOtp);
                        if (otp.Equals(fotp))
                        {
                            var custid = Convert.ToInt32(Cust_ID);
                            var familylist = db.Sp_Get_FamilyProperties(custid, Convert.ToInt64(FileFormNumber)).ToList();
                            //var familylist = db.Sp_Get_CustomerFamilyProperties(custid);

                            return Ok(familylist);
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
        public IHttpActionResult GetFamilyFilesList(string SystemKey, string Cust_ID, string Token)
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

                        var custid = Convert.ToInt32(Cust_ID);
                        var familylist = db.Sp_Get_CustomerFamilyProperties(custid);
                        return Ok(familylist);
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
        public IHttpActionResult GetFileDetails(string SystemKey, string FileFormNo, string Token)
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
                        var f = db.Sp_Get_FileDetail(FileFormNo);
                        FileSystemController a = new FileSystemController();
                        a.TestAdjustIntallments(Convert.ToInt32(FileFormNo));
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
        public IHttpActionResult GetFileInstallment(string SystemKey, string FileId, string Token)
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
                        var f = db.Sp_Get_FileInstallment(FileId);
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
        public IHttpActionResult GetPersonalInfo(string SystemKey, string FileId, string cnic, string Token)
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
                        var f = db.Sp_Get_PersonalInfo(FileId, cnic);
                        FileSystemController a = new FileSystemController();
                        a.TestAdjustIntallments(Convert.ToInt64(FileId));

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
        public IHttpActionResult GetPersonalInfo1(string SystemKey, string FileId, string Token)
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
                        var f = db.Sp_Get_PersonalInfo1(FileId);
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
        //[HttpGet]
        //public IHttpActionResult GetNomineeInfo(string SystemKey, string FileId)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(SystemKey))
        //        {
        //            string key = "MeherESTATE";
        //            if (key.Equals(SystemKey))
        //            {
        //                var f = db.Sp_Get_NomineeInfo(FileId);
        //                return Ok(f);
        //            }
        //            else
        //            {
        //                return NotFound();
        //            }
        //        }
        //        else
        //        {
        //            return InternalServerError();
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        return InternalServerError(ex);
        //    }
        //}
        [HttpPost]
        public IHttpActionResult GetInstallmentInfo(string SystemKey, string FileId, string cnic, string Token)
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
                        var f = db.Sp_Get_Installments(FileId, cnic);
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

        public IHttpActionResult FileStatmentAcc(string SystemKey, string FileId, string cnic, string Token)
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

                        var res1 = db.Sp_Get_FileFormData(Convert.ToInt64(FileId)).SingleOrDefault();
                        var res2 = db.Sp_Get_FileLastOwner(res1.Id).ToList();

                        if (res1 == null) { return Json(false); }

                        FileData fa = new FileData()
                        {
                            Id = Convert.ToInt64(res1.Id),
                            Block_Name = res1.Block,
                            Development_Charges = res1.Development_Charges.ToString(),
                            Phase_Name = res1.Phase,
                            Plot_Size = res1.Plot_Size,
                            Project_Name = res1.Project,
                            CNIC_NICOP = res1.CNIC_NICOP,
                            Father_Husband = res1.Father_Husband,
                            Mobile_1 = res1.Mobile_1,
                            Name = res1.Name,
                            Postal_Address = res1.Postal_Address,
                            Balance_Amount = res1.Balance_Amount,
                            File_Form_Number = res1.FileFormNumber,
                            Plot_Prefrence = res1.Plot_Prefrence,
                            AuditVerified = res1.Verified
                        };

                        var inst = db.Sp_Get_FileInstallments(res1.Id).Select(x => new PlotStatment
                        {
                            Description = x.Installment_Name,
                            Date = x.Due_Date,
                            Debit = x.Amount,
                            Credit = 0,
                            Type = x.Installment_Type
                        }).ToList();
                        string[] Type = { "Booking", "Installment" };
                        var rece = db.Sp_Get_ReceivedAmounts(Convert.ToInt64(FileId), Modules.FileManagement.ToString()).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
                        {
                            Receipt_Voucher_No = x.ReceiptNo,
                            Credit = x.Amount,
                            Mode = x.PaymentType,
                            Chq_No = x.Ch_Pay_Draft_No,
                            Date = x.DateTime,
                            Bank = x.Bank,
                            RcvdDate = x.DateTime,
                            Type = x.Type,
                            Inst_Status = x.Status
                        }).ToList();
                        var discount = db.Discounts.Where(x => x.Module_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).Select(x => new PlotStatment
                        {
                            Receipt_Voucher_No = "Discount",
                            Credit = x.Discount_Amount,
                            Mode = "",
                            Chq_No = "",
                            Date = x.DateTime,
                            Bank = "",
                            RcvdDate = x.DateTime,
                            Type = "",
                            Inst_Status = null
                        }).ToList();
                        var balance = db.File_Plot_Balance.Where(x => x.File_Plot_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).FirstOrDefault();
                        List<PlotStatment> Rm = new List<PlotStatment>();
                        Rm.AddRange(inst);
                        Rm.AddRange(rece);
                        Rm.AddRange(discount);
                        Rm = Rm.OrderBy(x => x.Date).ToList();
                        var res = new { OwnerData = res2, FileData = fa, Report = Rm, TotalAmt = balance.Total_Amount };

                        return Ok(res);
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
        public IHttpActionResult GetOtherInstallmentInfo(string SystemKey, string FileId, string cnic, string Token)
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
                        var f = db.Sp_Get_OtherInstallments(FileId, cnic);
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
        public IHttpActionResult GetReceiptsInfo(string SystemKey, string FileId, string cnic, string Token)
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
                        var f = db.Sp_Get_Receipts(FileId, cnic);
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
        public IHttpActionResult GetFilesInstallments(string SystemKey, decimal Amount, decimal Balance, long FileId, string Token)
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
                        List<Sp_Get_FileInstallments_Result> Allinstallments = db.Sp_Get_FileInstallments(FileId).ToList();
                        var res = Allinstallments.Where(x => x.Status != "Paid" && x.Installment_Type == "1").ToList();
                        decimal Actamt = Amount;
                        if (Balance >= 0)
                            Actamt = Amount + Balance;
                        decimal TotalAmt = 0, AmttoPaid = 0;
                        int ActualAmt = (int)Actamt;
                        List<AmountToPaidInfo> latpi = new List<AmountToPaidInfo>();

                        foreach (var item in res)
                        {
                            AmountToPaidInfo atpi = new AmountToPaidInfo();
                            TotalAmt += item.Amount;
                            int TtlAmt = (int)TotalAmt;
                            if (TtlAmt <= ActualAmt)
                            {
                                AmttoPaid += item.Amount;
                                atpi.Id = item.Id;
                                atpi.Amount = item.Amount;
                                atpi.Installment_Name = item.Installment_Name;
                                atpi.Months = string.Format("{0:MMM}", item.Due_Date);
                                latpi.Add(atpi);
                            }
                            else
                            {
                                break;
                            }
                        }
                        return Ok(latpi);
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
        public IHttpActionResult GetBalanceInfo(string SystemKey, long FileFormId, int installmenttype, string Token)
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
                        //var f = db.Sp_Get_Balance(FileId);
                        //var f = db.Sp_Get_FileDevelopment_Balance(FileFormId);
                        var f = db.Sp_Get_FileInstallmentAmount(FileFormId, installmenttype);
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
        public IHttpActionResult GetConfirmationInfo(string SystemKey, long FileId, string Token)
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
                        var f = db.Sp_Get_Confirmation_Info(FileId);
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
        public IHttpActionResult AddReceipts(string SystemKey, decimal amount, string bank, string paychk, Nullable<DateTime> paydate, string branch, string mobile1, string Father_Husband, string Name, string PaymentType, Nullable<decimal> totalamount, string projectname, Nullable<decimal> rate, string userid, string receiptno, Nullable<DateTime> date, string charges, string Token)
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
                        string blockname = string.Empty;
                        string getplot = string.Empty;
                        string email = string.Empty;
                        string installment_name = string.Empty;
                        string cons_file = userid.ToString();
                        string module = string.Empty;
                        string FilePlotNumber = string.Empty;
                        string PlotType = string.Empty;
                        string smstext = string.Empty;

                        string[] filenumber = Regex.Split(cons_file, "_");
                        long id = Convert.ToInt64(filenumber[0]);
                        string file_plot_number = filenumber[1];
                        string installment_type = filenumber[2];
                        var getdb = db.Sp_Get_FileAppFormData(file_plot_number).FirstOrDefault();
                        if (getdb != null)
                        {
                            var cust = db.Sp_Get_FileLastOwner(getdb.Id).FirstOrDefault();
                            blockname = getdb.Block_Name;
                            getplot = getdb.Plot_Size;
                            email = cust.Email;
                            PlotType = getdb.Type;
                            module = Modules.FileManagement.ToString();
                            FilePlotNumber = file_plot_number.ToString();
                            smstext = "Successfully Paid your Installment of file# " + FilePlotNumber;
                        }
                        else
                        {
                            var plot = db.Sp_Get_PlotData(Convert.ToInt64(file_plot_number)).FirstOrDefault();
                            var cust = db.Sp_Get_PlotLastOwner(plot.Id).FirstOrDefault();
                            blockname = plot.Block_Name;
                            getplot = plot.Plot_Size;
                            email = cust.Email;
                            module = Modules.PlotManagement.ToString();
                            PlotType = plot.Type;
                            if (!string.IsNullOrEmpty(plot.Application_FileNo))
                            {
                                FilePlotNumber = plot.Plot_No + " " + plot.Sector + "( " + plot.Application_FileNo + " )";
                            }
                            else
                            {
                                FilePlotNumber = plot.Plot_No + " " + plot.Sector;
                            }
                            smstext = "Successfully Paid your Installment of Plot# " + FilePlotNumber;
                        }
                        installment_name = "Installment";

                        string amountinwords = GeneralMethods.NumberToWords(Convert.ToInt32(amount));
                        //var res1 = db.Sp_Update_Installments_Cash_Status(installmentids, rd.Amount).FirstOrDefault(); ReceiptTypes.Installment.ToString()
                        var res = db.Sp_Add_Receipt_Manual(amount, amountinwords, bank, paychk, paydate, branch, mobile1, Father_Husband, getdb.Id, Name, PaymentType, totalamount, module, rate, null, getplot, ReceiptTypes.Installment.ToString(), Convert.ToInt64(id), Convert.ToInt64(id), receiptno, date, charges, FilePlotNumber, blockname, PlotType).FirstOrDefault();
                        //var res2 = db.Sp_Add_Receipt_Manual(amount,amountinwords,bank,paychk,paydate,branch,mobile1,Father_Husband,file_plot_number,Name,PaymentType, totalamount,projectname,rate, registerationno, plotsize,PaymentType,tokennumber,userid,receiptno,date,charges,file_plot_number.ToString()).FirstOrDefault();
                        SmsService sms = new SmsService();
                        sms.SendMsg(smstext, mobile1);
                        EmailService es = new EmailService();

                        string customerreceiptdesign = "<html><head><style type='text/css'>body, div, table, thead, tbody, tfoot, tr, th, td, p {font-family: 'Calibri';font-size: 19px;}table td {height: 28px;}body {-webkit-print-color-adjust: exact;margin: 0;}</style></head><body> <table cellspacing='0' border='0' style='width:1000px'><tr><td width='800px' align='right' valign=bottom style='padding-right: 95px;'><img src='http://202.141.230.181/assets/static/images/InstallmentReceipts/customer.png' width=450 height=125></td><td width='200px' align='right' valign=bottom><img src='http://202.141.230.181/assets/static/images/customer-copy.png' width=125 height=125></td></tr></table><table cellspacing='0' border='0' style='width: 1000px'><tr><td width = '250px'></td><td width = '90px' valign = bottom style = 'font-weight:bold'>Receipt No.</td ><td width = '125px' align = 'center' valign = bottom style = 'border-bottom:1px solid'>'" + receiptno + "'</td><td width = '246x' valign = bottom ></td></tr></table>   <table cellspacing='0' border='0' style='width: 1000px'><tr><td width = '340px' align = 'left' valign = bottom style = 'font-weight:bold'> Received with Thanks from Mr./ Mrs./ Miss:</td><td width = '660px' align = 'center' valign = bottom style = 'border-bottom:1px solid'>'" + Name + "'</td></tr></table><table cellspacing='0' border='0' style='width:1000px'><tr><td width='105px' align='left' valign=bottom style='font-weight:bold'>S/O,D/O,W/O</td><td width='535px' align='center' valign=bottom style='border-bottom:1px solid'>'" + Father_Husband + "'</td><td width='95px' align='left' valign=bottom style='font-weight:bold'>Contact No.</td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + mobile1 + "'</td></tr></table><table cellspacing='0' border='0' style='width:1000px'><tr><td width='140px' align='left' valign=bottom style='font-weight:bold'>A Sum of Rupees</td><td width='530px' align='center' valign=bottom style='border-bottom:1px solid'>'" + amountinwords + "'</td><td width='10px'></td><td width='220px' align='center' valign=bottom style='border-bottom:1px solid;font-weight:bold'></td></tr></table><table cellspacing='0' border='0' style='width:1000px'><tr><td width='340px' align='left' valign=bottom style='font-weight:bold'>In Cash / Cheque / Pay Order / Bank Draft</td><td width='360px' align='center' valign=bottom style='border-bottom:1px solid'>'" + PaymentType + "'</td><td width='35px' align='left' valign=bottom style='font-weight:bold'>Date </td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + DateTime.Now + "'</td></tr></table><table cellspacing='0' border='0' style='width:1000px'><tr><td width='60px' align='left' valign=bottom style='font-weight:bold'>File No </td><td width='300px' align='center' valign=bottom style='border-bottom:1px solid'>'" + FilePlotNumber + "'</td><td width='45px' align='left' valign=bottom style='font-weight:bold'>Block </td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + blockname + "'</td><td width='35px' align='left' valign=bottom style='font-weight:bold'>Size </td><td width='235px' align='center' valign=bottom style='border-bottom:1px solid'>'" + getplot + "'</td></tr></table><table cellspacing='0' border='0' style='width:1000px'><tr><td align='right' valign=bottom><font size='4'>A Project of</font></td></tr></table><table cellspacing='0' border='0' style='width:1000px'><tr><td width='35px' align='left' valign=bottom style='font-weight:bold'><font size='6'>Rs.</font></td><td width='170px' align='center' valign=bottom style='border-bottom:1px solid'><font size='6'>'" + amount + "'</font></td><td width='60px' align='left' valign=bottom><font size='4'></font></td><td width='220px' align='left' valign=bottom style='font-weight:bold'><font size='6'>For Meher Estate Developers</font></td><td width='180px' align='center' valign=bottom style='border-bottom:1px solid'><font size='6'></font></td><td width='180px' align='left' valign=bottom><font size='4'></font></td><td width='130px' align='left' style='vertical-align:top'><img src='http://202.141.230.181/assets/static/images/meherestate-simple.png' width=130 height=29></td></tr></body></html>";
                        string accountsreceiptdesign = "<html><head><style type='text/css'>body, div, table, thead, tbody, tfoot, tr, th, td, p {font-family: 'Calibri';font-size: 19px;}table td {height: 28px;}body {-webkit-print-color-adjust: exact;margin: 0;}</style></head><body> <table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width:1000px'><tr><td width='800px' align='right' valign=bottom style='padding-right: 95px;'><img src='http://202.141.230.181/assets/static/images/InstallmentReceipts/accounts.png' width=450 height=125></td><td width='200px' align='right' valign=bottom><img src='http://202.141.230.181/assets/static/images/accounts-copy.png' width=125 height=125></td></tr></table><table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width: 1000px'><tr><td width = '250px'></td><td width = '90px' valign = bottom style = 'font-weight:bold'>Receipt No.</td ><td width = '125px' align = 'center' valign = bottom style = 'border-bottom:1px solid'>'" + receiptno + "'</td><td width = '246x' valign = bottom ></td></tr></table>   <table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width: 1000px'><tr><td width = '340px' align = 'left' valign = bottom style = 'font-weight:bold'> Received with Thanks from Mr./ Mrs./ Miss:</td><td width = '660px' align = 'center' valign = bottom style = 'border-bottom:1px solid'>'" + Name + "'</td></tr></table><table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width:1000px'><tr><td width='105px' align='left' valign=bottom style='font-weight:bold'>S/O,D/O,W/O</td><td width='535px' align='center' valign=bottom style='border-bottom:1px solid'>'" + Father_Husband + "'</td><td width='95px' align='left' valign=bottom style='font-weight:bold'>Contact No.</td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + mobile1 + "'</td></tr></table><table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width:1000px'><tr><td width='140px' align='left' valign=bottom style='font-weight:bold'>A Sum of Rupees</td><td width='530px' align='center' valign=bottom style='border-bottom:1px solid'>'" + amountinwords + "'</td><td width='10px'></td><td width='220px' align='center' valign=bottom style='border-bottom:1px solid;font-weight:bold'></td></tr></table><table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width:1000px'><tr><td width='340px' align='left' valign=bottom style='font-weight:bold'>In Cash / Cheque / Pay Order / Bank Draft</td><td width='360px' align='center' valign=bottom style='border-bottom:1px solid'>'" + PaymentType + "'</td><td width='35px' align='left' valign=bottom style='font-weight:bold'>Date </td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + DateTime.Now + "'</td></tr></table><table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width:1000px'><tr><td width='60px' align='left' valign=bottom style='font-weight:bold'>File No </td><td width='300px' align='center' valign=bottom style='border-bottom:1px solid'>'" + FilePlotNumber + "'</td><td width='45px' align='left' valign=bottom style='font-weight:bold'>Block </td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + blockname + "'</td><td width='35px' align='left' valign=bottom style='font-weight:bold'>Size </td><td width='235px' align='center' valign=bottom style='border-bottom:1px solid'>'" + getplot + "'</td></tr></table><table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width:1000px'><tr><td align='right' valign=bottom><font size='4'>A Project of</font></td></tr></table><table bgcolor='#F7DCF7' cellspacing='0' border='0' style='width:1000px'><tr><td width='35px' align='left' valign=bottom style='font-weight:bold'><font size='6'>Rs.</font></td><td width='170px' align='center' valign=bottom style='border-bottom:1px solid'><font size='6'>'" + amount + "'</font></td><td width='60px' align='left' valign=bottom><font size='4'></font></td><td width='220px' align='left' valign=bottom style='font-weight:bold'><font size='6'>For Meher Estate Developers</font></td><td width='180px' align='center' valign=bottom style='border-bottom:1px solid'><font size='6'></font></td><td width='180px' align='left' valign=bottom><font size='4'></font></td><td width='130px' align='left' style='vertical-align:top'><img src='http://202.141.230.181/assets/static/images/meherestate-simple.png' width=130 height=29></td></tr></body></html>";
                        string filereceiptdesign = "<html><head><style type='text/css'>body, div, table, thead, tbody, tfoot, tr, th, td, p {font-family: 'Calibri';font-size: 19px;}table td {height: 28px;}body {-webkit-print-color-adjust: exact;margin: 0;}</style></head><body> <table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width:1000px'><tr><td width='800px' align='right' valign=bottom style='padding-right: 95px;'><img src='http://202.141.230.181/assets/static/images/InstallmentReceipts/file.png' width=450 height=125></td><td width='200px' align='right' valign=bottom><img src='http://202.141.230.181/assets/static/images/files-copy.png' width=125 height=125></td></tr></table> <table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width: 1000px'><tr><td width = '250px'></td><td width = '90px' valign = bottom style = 'font-weight:bold'>Receipt No.</td ><td width = '125px' align = 'center' valign = bottom style = 'border-bottom:1px solid'>'" + receiptno + "'</td><td width = '246x' valign = bottom ></td></tr></table>   <table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width: 1000px'><tr><td width = '340px' align = 'left' valign = bottom style = 'font-weight:bold'> Received with Thanks from Mr./ Mrs./ Miss:</td><td width = '660px' align = 'center' valign = bottom style = 'border-bottom:1px solid'>'" + Name + "'</td></tr></table><table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width:1000px'><tr><td width='105px' align='left' valign=bottom style='font-weight:bold'>S/O,D/O,W/O</td><td width='535px' align='center' valign=bottom style='border-bottom:1px solid'>'" + Father_Husband + "'</td><td width='95px' align='left' valign=bottom style='font-weight:bold'>Contact No.</td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + mobile1 + "'</td></tr></table><table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width:1000px'><tr><td width='140px' align='left' valign=bottom style='font-weight:bold'>A Sum of Rupees</td><td width='530px' align='center' valign=bottom style='border-bottom:1px solid'>'" + amountinwords + "'</td><td width='10px'></td><td width='220px' align='center' valign=bottom style='border-bottom:1px solid;font-weight:bold'></td></tr></table><table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width:1000px'><tr><td width='340px' align='left' valign=bottom style='font-weight:bold'>In Cash / Cheque / Pay Order / Bank Draft</td><td width='360px' align='center' valign=bottom style='border-bottom:1px solid'>'" + PaymentType + "'</td><td width='35px' align='left' valign=bottom style='font-weight:bold'>Date </td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + DateTime.Now + "'</td></tr></table><table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width:1000px'><tr><td width='60px' align='left' valign=bottom style='font-weight:bold'>File No </td><td width='300px' align='center' valign=bottom style='border-bottom:1px solid'>'" + FilePlotNumber + "'</td><td width='45px' align='left' valign=bottom style='font-weight:bold'>Block </td><td width='260px' align='center' valign=bottom style='border-bottom:1px solid'>'" + blockname + "'</td><td width='35px' align='left' valign=bottom style='font-weight:bold'>Size </td><td width='235px' align='center' valign=bottom style='border-bottom:1px solid'>'" + getplot + "'</td></tr></table><table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width:1000px'><tr><td align='right' valign=bottom><font size='4'>A Project of</font></td></tr></table><table bgcolor='#FAFAC6' cellspacing='0' border='0' style='width:1000px'><tr><td width='35px' align='left' valign=bottom style='font-weight:bold'><font size='6'>Rs.</font></td><td width='170px' align='center' valign=bottom style='border-bottom:1px solid'><font size='6'>'" + amount + "'</font></td><td width='60px' align='left' valign=bottom><font size='4'></font></td><td width='220px' align='left' valign=bottom style='font-weight:bold'><font size='6'>For Meher Estate Developers</font></td><td width='180px' align='center' valign=bottom style='border-bottom:1px solid'><font size='6'></font></td><td width='180px' align='left' valign=bottom><font size='4'></font></td><td width='130px' align='left' style='vertical-align:top'><img src='http://202.141.230.181/assets/static/images/meherestate-simple.png' width=130 height=29></td></tr></body></html>";
                        string Subject = "Receipt# " + receiptno + " of File Number: " + FilePlotNumber;
                        if (!string.IsNullOrEmpty(email))
                        {
                            es.SendEmail(customerreceiptdesign, email, Subject);
                        }
                        es.SendEmail(accountsreceiptdesign, "test@meharestate.com", Subject);
                        es.SendEmail(filereceiptdesign, "test@meharestate.com", Subject);
                        return Ok(res);
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
        public IHttpActionResult GetCustomerInfo(string SystemKey, string FileId, string Token)
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
                        var f = db.Sp_Get_CustomerInfo(FileId);
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
        public IHttpActionResult GetFilesCount(string SystemKey, string CNIC, string Token)
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
                        var f = db.Sp_Get_CountFiles(CNIC);
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
        /// <summary>
        /// This is use to get the Email from Files Transfer
        /// </summary>
        /// <param name="SystemKey"></param>
        /// <param name="CNIC"></param>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetEmail(string SystemKey, string CNIC, string Mobile, string Token)
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
                        var f = db.Sp_Get_Email(Mobile, CNIC);
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
        /// <summary>
        /// This is used to Update the Email to Files Transfer if the Email is Null
        /// </summary>
        /// <param name="SystemKey"></param>
        /// <param name="CNIC"></param>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateEmail(string SystemKey, string Email, string CNIC, string Mobile, string Token)
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
                        var f = db.Sp_Update_FilesTransferEmail(Email, Mobile, CNIC);
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
        //[HttpPost]
        //public IHttpActionResult AddReceipts(string SystemKey, decimal amount, string bank, string paychk, Nullable<DateTime> paydate, string branch, string mobile1, string Father_Husband, string Name, string PaymentType, Nullable<decimal> totalamount, string projectname, Nullable<decimal> rate, string userid, string receiptno, Nullable<DateTime> date, string charges)
        //{
        //    try
        //    {
        //        return Ok(SystemKey+ amount);

        //    }
        //    catch (Exception ex)
        //    {

        //        return InternalServerError(ex);
        //    }
        //}
        [HttpPost]
        public IHttpActionResult GetQrInformation(string SystemKey, string qrvalue, string qrfileformvalue, string Token)
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
                        string oldcode = qrvalue.Substring(0, 5);

                        if (oldcode == "SAFV0")
                        {

                            var qrcodevalue1 = db.Sp_Get_QrCodeValue(Convert.ToInt64(qrfileformvalue)).FirstOrDefault();
                            if (qrcodevalue1.QR_Code_Text.Equals(qrvalue))
                            {
                                var oldcodeinfo = db.Sp_Get_QrCodeInformation(Convert.ToInt64(qrfileformvalue)).FirstOrDefault();
                                var registerationinfo = db.Sp_Get_CustomerRegisterationInformation(oldcodeinfo.Mobile_1).FirstOrDefault();
                                if (oldcodeinfo.CNIC_NICOP.Equals(registerationinfo.CNIC))
                                {
                                    return Ok(registerationinfo);
                                }
                                else
                                {
                                    return BadRequest("NotMatched");
                                }
                            }
                            else
                            {
                                return BadRequest("Not Found");
                            }


                        }
                        else
                        {
                            var qrcodevalue = db.Sp_Get_QrCodeValue(Convert.ToInt64(qrvalue)).FirstOrDefault();
                            long getuniquevalue = qrcodevalue.QR_Code_Uniqe_No;
                            if (qrvalue.Equals(getuniquevalue))
                            {
                                var info = db.Sp_Get_QrCodeInformation(Convert.ToInt64(qrfileformvalue)).FirstOrDefault();

                                var getregisterationinfo = db.Sp_Get_CustomerRegisterationInformation(info.Mobile_1).FirstOrDefault();
                                if (info.CNIC_NICOP.Equals(getregisterationinfo.CNIC))
                                {
                                    return Ok(getregisterationinfo);
                                }
                                else
                                {
                                    return BadRequest("NotMatched");
                                }

                            }
                            else
                            {
                                return BadRequest("NotFound");
                            }
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
                return BadRequest("Please Contact to Meher Estate Developers Team.");
                //return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult GetPlots(string SystemKey, string Phone, string CNIC, string Token)
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
                        var f = db.API_Sp_Get_Plots(Phone, CNIC);
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
        public IHttpActionResult GetDetailofPlot(string SystemKey, long Plotid, string Token)
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

                        var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
                        var res2 = db.Sp_Get_PlotLastOwner(Plotid).FirstOrDefault();
                        var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
                        var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
                        var res5 = db.File_Plot_Balance.Where(x => x.Module == Modules.PlotManagement.ToString() && x.File_Plot_Id == Plotid).FirstOrDefault().Balance_Amount;
                        var res = new { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4, Balance = res5 };
                        return Ok(res);
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
        public IHttpActionResult GetPlotsInstallment(string SystemKey, long Plotid, string Token)
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
                        var plotinstallment = db.API_Sp_Get_PlotsInstallmentInfo(Plotid).FirstOrDefault();
                        return Ok(plotinstallment);
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
        public IHttpActionResult GetDiscountResult(string SystemKey, long moduleid, string Token)
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
                        var dis = db.Discounts.SingleOrDefault(x => x.Module_Id == moduleid && x.Module == Modules.FileManagement.ToString());

                        return Ok(dis);
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
        public IHttpActionResult GetPlotDiscountResult(string SystemKey, long moduleid, string Token)
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
                        var dis = db.Discounts.SingleOrDefault(x => x.Module_Id == moduleid && x.Module == Modules.PlotManagement.ToString());

                        return Ok(dis);
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
