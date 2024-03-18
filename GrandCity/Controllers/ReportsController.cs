using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Models;
using MeherEstateDevelopers.Filters;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using execldataimport.Controllers;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class ReportsController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        // GET: Reports
        public ActionResult RecoveryReport()
        {
            //var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Cashier")).ToList();
            var All = db.Sp_Get_Cashiers_List().ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");
            var recov = (from ReceiptTypes e in Enum.GetValues(typeof(ReceiptTypes)) select new { Name = e.ToString(), Type = "Receipts" }).ToList();
            var payme = (from Payments e in Enum.GetValues(typeof(Payments)) select new { Name = e.ToString(), Type = "Payments" }).ToList();
            recov.AddRange(payme);
            ViewBag.Type = new SelectList(recov, "Name", "Name", "Type");
            return View();
        }
        [HttpPost]
        public ActionResult SearchAccountReport(DateTime? From, DateTime? To, long?[] Users, string[] Type)
        {
            string chids = null, types = null;

            if (From == null || To == null)
            {
                From = DateTime.Now;
                To = DateTime.Now;
            }
            ViewBag.From = From;
            ViewBag.To = To;
            if (Users != null)
            {
                chids = new XElement("ChPoDd", Users.Select(x => new XElement("Details",
                                 new XAttribute("Ids", x)
                                 ))).ToString();
            }
            else
            {
                var All = db.Sp_Get_Cashiers_List().ToList();
                ViewBag.Users = new SelectList(All, "id", "Name");
                chids = new XElement("ChPoDd", All.Select(x => new XElement("Details",
                                      new XAttribute("Ids", x.Id)
                                     ))).ToString();
            }

            if (Type != null)
            {
                types = new XElement("ReceiptsType", Type.Select(x => new XElement("Receipt",
                                 new XAttribute("Type", x)
                                 ))).ToString();
            }
            else
            {
                var recov = (from ReceiptTypes e in Enum.GetValues(typeof(ReceiptTypes)) select new { Name = e.ToString() }).ToList();
                var payme = (from Payments e in Enum.GetValues(typeof(Payments)) select new { Name = e.ToString() }).ToList();
                recov.AddRange(payme);

                types = new XElement("ReceiptsType", recov.Select(x => new XElement("Receipt",
                                      new XAttribute("Type", x.Name)
                                     ))).ToString();
            }

            var res1 = db.Sp_Get_DailyRecovery_Report_Parameter(From, To, chids, types).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Block = x.Block,
                Plot_Type = x.Plot_Type,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DATETIME,
                Dealership_Name = x.Dealership_Name,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Size = x.Size,
                Project = x.Project,
                ReceiptNo = x.ReceiptNo,
                Id = x.Id,
                Type = x.Type,
                Cancel = x.Cancel,
                VoucherType = x.VoucherType,
                Description = x.Description,
                TransacitonId = x.Transaction_Id
            }).ToList();
            List<DailyCashierUser_Report> res = new List<DailyCashierUser_Report>();
            res.AddRange(res1);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Searched Accounts Reports From: " + From + "To: " + To, "Read", "Activity_Record", ActivityType.Reports.ToString(), userid);
            return PartialView(res);
        }
        public ActionResult SAMRecoveryReport()
        {
            var All = db.Sp_Get_Cashiers_List().ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult SearchSAMAccountReport(DateTime? From, DateTime? To, long?[] Users)
        {
            string chids = null;
            if (Users != null)
            {
                chids = new XElement("ChPoDd", Users.Select(x => new XElement("Details",
                                 new XAttribute("Ids", x)
                                 ))).ToString();
            }
            var res1 = db.Sp_Get_DailySAMRecovery_Report_Parameter(From, To, chids).Select(x => new DailyCashierUser_Report
            {
                Id = x.Id,
                Amount = x.Amount,
                Bank = x.Bank,
                Block = x.Block,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DateTime,
                Dealership_Name = x.Company,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.ReceiptNo,
                Type = x.Type,
                Cancel = x.Cancel
            }).ToList();
            var res2 = db.Sp_Get_DailySAMVoucher_Report_Parameter(From, To, chids).Select(x => new DailyCashierUser_Report
            {
                Id = x.Id,
                Amount = x.Amount * -1,
                Bank = x.Bank,
                Block = x.Block,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DateTime,
                Dealership_Name = x.Company,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.VoucherNo,
                Type = x.Type,
                Cancel = x.Cancel
            }).ToList();

            DailySAMCashRep Fresult = new DailySAMCashRep();
            // Cash inflow
            {
                Fresult.PlotCollection = res1.ToList();

                Fresult.Plot_Cash = Fresult.PlotCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Plot_Cheque = Fresult.PlotCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Plot_PayOrder = Fresult.PlotCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Plot_BankDraft = Fresult.PlotCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Plot_Online = Fresult.PlotCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Plot_Total = Fresult.PlotCollection.Sum(x => x.Amount);

                Fresult.Cash = res1.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Cheque = res1.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.PayOrder = res1.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.BankDraft = res1.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Online = res1.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Total = res1.Sum(x => x.Amount);
            }
            // Cash out flow
            {
                Fresult.PlotVoucher = res2.ToList();


                Fresult.V_Plot_Cash = Fresult.PlotVoucher.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.V_Plot_Cheque = Fresult.PlotVoucher.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.V_Plot_PayOrder = Fresult.PlotVoucher.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.V_Plot_BankDraft = Fresult.PlotVoucher.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.V_Plot_Online = Fresult.PlotVoucher.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.V_Plot_Total = Fresult.PlotVoucher.Sum(x => x.Amount);

                Fresult.V_Cash = res2.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.V_Cheque = res2.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.V_PayOrder = res2.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.V_BankDraft = res2.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.V_Online = res2.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.V_Total = res2.Sum(x => x.Amount);
            }
            ViewBag.Name = res1.Select(x => x.Cashier_Name).FirstOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Searched SAM Accounts Reports From: " + From + "To: " + To, "Read", "Activity_Record", ActivityType.Reports.ToString(), userid);
            return PartialView(Fresult);
        }
        public ActionResult FileStatment(long Token)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            Sp_Get_FileFormData_Result res = db.Sp_Get_FileFormData(Token).SingleOrDefault();

            if (res.Verify == true)
            {
                var userRes = db.SP_Get_FileVerifier(res.Id).SingleOrDefault();
                ViewBag.verifier = (userRes == null) ? string.Empty : userRes.Name;
            }

            if (res == null) { return Json(false); }

            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Development_Charges = res.Development_Charges.ToString(),
                Phase_Name = res.Phase,
                Plot_Size = res.Plot_Size,
                Project_Name = res.Project,
                CNIC_NICOP = res.CNIC_NICOP,
                Father_Husband = res.Father_Husband,
                Mobile_1 = res.Mobile_1,
                Name = res.Name,
                Postal_Address = res.Postal_Address,
                Balance_Amount = res.Balance_Amount,
                File_Form_Number = res.FileFormNumber,
                Plot_Prefrence = res.Plot_Prefrence,
                AuditVerified = res.Verified,
                Verify = res.Verify
            };

            var inst = db.Sp_Get_FileInstallments(res.Id).Select(x => new AdjInst
            {
                Id = x.Id,
                Amount = x.Amount,
                Due_Date = x.Due_Date,
                Installment_Name = x.Installment_Name,
                Installment_Type = x.Installment_Type,
                Status = x.Status,
                check = false
            }).ToList();
            var discount = db.Discounts.SingleOrDefault(x => x.Module_Id == res.Id && x.Module == Modules.FileManagement.ToString());
            string[] Types = { "Booking", "Installment", "Adjusted" };
            var rece = db.Sp_Get_ReceivedAmounts(Token, Modules.FileManagement.ToString()).Where(x => Types.Contains(x.Type)).ToList();
            var totalamt = Math.Round(inst.Where(x => x.Installment_Type != "2").Sum(x => x.Amount));
            var newres = new FileLedger { File = fa, Installments = inst, ReceAmts = rece, TotalAmt = totalamt, DiscountAmt = discount };

            db.Sp_Add_Activity(userid, "Viewed File Statement", "Read", "Activity_Record", ActivityType.Reports.ToString(), Token);
            return View(newres);
        }

        public ActionResult FileStatmentAcc(string Token)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.File_Form.Where(x => x.FileFormNumber == Token).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Access Ledger Report of File Id <a class='plt-data' data-id=' " + Token + "'>" + Token + "</a> ", "Read", Modules.FileManagement.ToString(), ActivityType.Details_Access.ToString(), res1.Id);
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
                CNIC_NICOP = string.Join(",", res2.Select(x => x.CNIC_NICOP)),
                Father_Husband = string.Join(",", res2.Select(x => x.Father_Husband)),
                Mobile_1 = string.Join(",", res2.Select(x => x.Mobile_1)),
                Name = string.Join(",", res2.Select(x => x.Name)),
                Postal_Address = string.Join(",", res2.Select(x => x.Postal_Address)),
                File_Form_Number = res1.FileFormNumber,
                Plot_Prefrence = res2.Select(x => x.Plot_Prefrence).FirstOrDefault(),
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
            string[] Type = { "Booking", "Installment", "Adjusted" };
            var rece = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
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
            var payment = db.Vouchers.Where(x => x.File_Plot_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).Select(x => new PlotStatment
            {
                Description = x.Description,
                Date = x.DateTime,
                Debit = x.Amount,
                Receipt_Voucher_No = x.VoucherNo,
                Credit = 0,
            }).ToList();
            var balance = db.File_Plot_Balance.Where(x => x.File_Plot_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).FirstOrDefault();
            List<PlotStatment> Rm = new List<PlotStatment>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm.AddRange(discount);
            Rm.AddRange(payment);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var res = new NewFileLedger { OwnerData = res2, FileData = fa, Report = Rm, Balance = balance };
            return View(res);
        }
        public ActionResult FileStatmentCancel(string FileNumber, long OwnerId)
        {
            var res1 = db.Sp_Get_FileAppFormData(FileNumber).SingleOrDefault();
            if (res1 == null) { return Json(false); }
            var res2 = db.Sp_Get_FileOwnerList(res1.Id).Where(x => x.Id == OwnerId && x.File_Form_Id == res1.Id).Select(x => new Sp_Get_FileLastOwner_Result
            {
                Block_Id = x.Block_Id,
                Cancel_Notice = x.Cancel_Notice,
                City = x.City,
                CNIC_NICOP = x.CNIC_NICOP,
                Customer_File_Flag = x.Customer_File_Flag,
                DateTime = x.DateTime,
                Date_Of_Birth = x.Date_Of_Birth,
                Delivery = x.Delivery,
                DiscountPercent = x.DiscountPercent,
                Email = x.Email,
                Father_Husband = x.Father_Husband,
                File_Form_Id = x.File_Form_Id,
                First_notice = x.First_notice,
                GrandTotal = x.GrandTotal,
                Id = x.Id,
                Mobile_1 = x.Mobile_1,
                Mobile_2 = x.Mobile_2,
                Name = x.Name,
                Nationality = x.Nationality,
                Nominee_Address = x.Nominee_Address,
                Nominee_CNIC_NICOP = x.Nominee_CNIC_NICOP,
                Nominee_Name = x.Nominee_Name,
                Nominee_Relation = x.Nominee_Relation,
                Occupation = x.Occupation,
                Ownership_Status = x.Ownership_Status,
                Phase_Id = x.Phase_Id,
                Phone_Office = x.Phone_Office,
                Plot_Prefrence = x.Plot_Prefrence,
                Plot_Size = x.Plot_Size,
                Postal_Address = x.Postal_Address,
                Rate = x.Rate,
                Received_Amount = x.Received_Amount,
                Residential = x.Residential,
                Residential_Address = x.Residential_Address,
                Sec_Notice = x.Sec_Notice,
                Transfer_Description = x.Transfer_Description,
                Total = x.Total
            }).ToList();
            var res4 = db.File_Plot_Balance.Where(x => x.File_Plot_Id == res1.Id).FirstOrDefault();
            decimal bal = (res4 == null) ? 0 : res4.Balance_Amount;
            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res1.Id),
                Block_Name = res1.Block_Name,
                Development_Charges = res1.Development_Charges.ToString(),
                Plot_Size = res1.Plot_Size,
                CNIC_NICOP = string.Join(",", res2.Select(x => x.CNIC_NICOP)),
                Father_Husband = string.Join(",", res2.Select(x => x.Father_Husband)),
                Mobile_1 = string.Join(",", res2.Select(x => x.Mobile_1)),
                Name = string.Join(",", res2.Select(x => x.Name)),
                Postal_Address = string.Join(",", res2.Select(x => x.Postal_Address)),
                Balance_Amount = bal,
                File_Form_Number = res1.FileFormNumber,
                Plot_Prefrence = res2.Select(x => x.Plot_Prefrence).FirstOrDefault(),
                AuditVerified = res1.Verified
            };

            var inst = db.Sp_Get_FileInstallments_Cancelled(res1.Id, OwnerId).Select(x => new PlotStatment
            {
                Debit = x.Amount,
                Credit = 0,
                Date = x.Due_Date,
                Description = x.Installment_Name,
                Type = x.Installment_Type
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
            string[] Types = { "Booking", "Installment", "Adjusted" };

            var rece = db.Sp_Get_ReceivedAmountsCancelOwner(res1.Id, Modules.FileManagement.ToString(), OwnerId).Where(x => Types.Contains(x.Type)).Select(x => new PlotStatment
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
            var payment = db.Vouchers.Where(x => x.File_Plot_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).Select(x => new PlotStatment
            {
                Description = x.Description,
                Date = x.DateTime,
                Debit = x.Amount,
                Credit = 0,
            }).ToList();
            List<PlotStatment> Rm = new List<PlotStatment>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm.AddRange(discount);
            Rm.AddRange(payment);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var balance = db.File_Plot_Balance.Where(x => x.File_Plot_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).FirstOrDefault();

            var totalamt = Math.Round(inst.Where(x => x.Type != "2").Sum(x => (double)x.Debit));
            var newres = new NewFileLedger { FileData = fa, Report = Rm, Balance = balance, OwnerData = res2 };
            return View("FileStatmentAcc", newres);
        }
        public ActionResult FileBalanceRep(long Token)
        {
            Sp_Get_FileFormData_Result res = db.Sp_Get_FileFormData(Token).SingleOrDefault();

            if (res == null) { return Json(false); }

            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Development_Charges = res.Development_Charges.ToString(),
                Phase_Name = res.Phase,
                Plot_Size = res.Plot_Size,
                Project_Name = res.Project,
                CNIC_NICOP = res.CNIC_NICOP,
                Father_Husband = res.Father_Husband,
                Mobile_1 = res.Mobile_1,
                Name = res.Name,
                Postal_Address = res.Postal_Address,
                Balance_Amount = res.Balance_Amount,
                File_Form_Number = res.FileFormNumber,
                Plot_Prefrence = res.Plot_Prefrence,
            };
            var inst = db.Sp_Get_FileInstallments(res.Id).Select(x => new ReportModel
            {
                Installments = x.Installment_Name,
                Due_Date = x.Due_Date,
                Amount = x.Amount,
                Date = x.Due_Date
            }).ToList();
            var rece = db.Sp_Get_ReceivedAmounts(res.Id, Modules.FileManagement.ToString()).Select(x => new ReportModel
            {
                Receipt = x.ReceiptNo,
                Recp_Amount = x.Amount,
                Mode = x.PaymentType,
                Chq_No = x.Ch_Pay_Draft_No,
                Date = x.DateTime,
                Bank = x.Bank,
                Recp_Date = x.DateTime
            }).ToList();

            var totalamt = Math.Round(Convert.ToDecimal(inst.Sum(x => x.Amount)));
            List<ReportModel> Rm = new List<ReportModel>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var fres = new FileBalanceLedger { File = fa, Report = Rm, TotalAmt = totalamt };

            return View(fres);
        }
        public ActionResult AllReports()
        {
            return View();
        }
        public ActionResult FileReport()
        {
            return PartialView();
        }
        public ActionResult AllFilesReports()
        {
            return View();
        }
        public ActionResult MonthlyFilesRecovery(DateTime Month)
        {
            var res = db.Sp_Get_Report_MonthRecoveryReport(Month).ToList();
            return PartialView(res);
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PlotsOutstanding()
        {
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }
        public ActionResult FilesOutstanding()
        {
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }
        public ActionResult PlotsfilesCosolidatedReports()
        {
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }

        public ActionResult FilesStatus()
        {
            var res = db.Sp_Reports_FilesStatus().ToList();
            List<string> cat = new List<string>();
            cat = res.Select(x => x.Plot_Size).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            List<Report_Series> Listrep = new List<Report_Series>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            foreach (var g in res.GroupBy(x => x.Status))
            {
                string Status = g.Key;
                Report_Series report = new Report_Series()
                {
                    name = g.Key,
                    data = new int?[cat.Count]
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == item.Plot_Size).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.No_Of_Files;
                }
                Listrep.Add(report);
            }
            Report_FilesStatus data = new Report_FilesStatus()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public JsonResult PlotsOutStandingReport(long Id)
        {
            var res1 = db.Sp_Get_Reports_PlotOutstandingByBlock(Id).ToList();
            var Total = res1.Sum(x => x.total);
            var Received = res1.Sum(x => x.Received);
            var Discount = res1.Sum(x => x.Discount);
            var Remain = res1.Where(x => x.Remaining > 0).Sum(x => x.Remaining);
            var access = res1.Where(x => x.Remaining < 0).Sum(x => x.Remaining);
            var res = new { Result = res1, TotalAmt = Total, ReceivedAmt = Received, Discount = Discount, Remaining = Remain, Access = access };
            return Json(res);
        }
        public JsonResult FilesOutStandingReport(long Id)
        {
            var res1 = db.Sp_Get_Reports_FilesOutstandingByBlock(Id).ToList();
            var Total = res1.Sum(x => x.total);
            var Received = res1.Sum(x => x.Received);
            var Discount = res1.Sum(x => x.Discount);
            var Remain = res1.Where(x => x.Remaining > 0).Sum(x => x.Remaining);
            var access = res1.Where(x => x.Remaining < 0).Sum(x => x.Remaining);
            var res = new { Result = res1, TotalAmt = Total, ReceivedAmt = Received, Discount = Discount, Remaining = Remain, Access = access };
            return Json(res);
        }
        public JsonResult FilesPlotsOutStandingReport(long Id)  // working  on this 
        {
            var res = db.Sp_Get_Reports_PlotOutstandingByBlock(Id).ToList();
            var PlotTotal = res.Sum(x => x.total);
            var PlotReceived = res.Sum(x => x.Received);
            var PlotDiscount = res.Sum(x => x.Discount);
            var PlotRemain = res.Where(x => x.Remaining > 0).Sum(x => x.Remaining);
            var Plotaccess = res.Where(x => x.Remaining < 0).Sum(x => x.Remaining);
            var res1 = new { Result = res, PlotTotalAmt = PlotTotal, PlotReceivedAmt = PlotReceived, PlotDiscount = PlotDiscount, PlotRemaining = PlotRemain, PlotAccess = Plotaccess };
            var res2 = db.Sp_Get_Reports_FilesOutstandingByBlock(Id).ToList();
            var fileTotal = res2.Sum(x => x.total);
            var fileReceived = res2.Sum(x => x.Received);
            var fileDiscount = res2.Sum(x => x.Discount);
            var fileRemain = res2.Where(x => x.Remaining > 0).Sum(x => x.Remaining);
            var fileaccess = res2.Where(x => x.Remaining < 0).Sum(x => x.Remaining);
            var res3 = new { Result = res2, fileTotalAmt = fileTotal, fileReceivedAmt = fileReceived, fileDiscount = fileDiscount, fileRemaining = fileRemain, fileAccess = fileaccess };
            var result = new { PlotResult = res1, fileResult = res3 };
            return Json(result);
            // var res1 = PlotsOutStandingReport(Id);
            // var res2 = FilesOutStandingReport(Id);
            // var result = new { PlotResult = res1, fileResult = res2 };
            //return Json(result);
        }
        public ActionResult PlotsOutStandingReport_View(string Block)
        {
            var Id = db.RealEstate_Blocks.Where(x => x.Block_Name == Block).Select(x => x.Id).FirstOrDefault();
            var res = db.Sp_Get_Reports_PlotOutstandingByBlock(Id).ToList();
            return View(res);
        }
        public ActionResult ServiceChargesBills()
        {
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }
        public ActionResult ServiceCharges(DateTime? month, DateTime? year, long?[] blockid)
        {
            List<Sp_Get_Report_ServiceCharges_Result> res3 = new List<Sp_Get_Report_ServiceCharges_Result>();
            var plotSizes = db.Plots.Select(x => new { x.Id, x.Plot_Size }).ToList();
            List<string> blocklist = new List<string>();
            if (blockid != null)
            {
                foreach (long index in blockid)
                {
                    var res13 = db.Sp_Get_Report_ServiceCharges(month, year, index).ToList<Sp_Get_Report_ServiceCharges_Result>();
                    res3.AddRange(res13);
                    var list = res13.Select(x => x.Block).FirstOrDefault();
                    blocklist.Add(list);
                }
            }
            else
            {
                var res1 = db.Sp_Get_Report_ServiceCharges(month, year, null).ToList<Sp_Get_Report_ServiceCharges_Result>();
                res1.ForEach(x => x.Plot_Size = plotSizes.Where(y => y.Id == x.Id).Select(z => z.Plot_Size).FirstOrDefault());
                res3.AddRange(res1);
            }
            // var res1 = db.Sp_Get_Report_ServiceCharges(month, year, null).ToList();
            var rec = res3.Sum(x => x.Grand_Total);
            var recd = res3.Sum(x => x.Amount_Paid);
            var Rrec = res3.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Grand_Total);
            var Rrecd = res3.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Amount_Paid);
            var Crec = res3.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Grand_Total);
            var Crecd = res3.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Amount_Paid);

            if (blockid != null)
            {
                ViewBag.Block = String.Join(",", blocklist.Cast<string>().ToArray()); ;
            }
            else
            {
                ViewBag.meherestatedevelopers = "Meher Estate Developers";
            }
            var res = new ServiceChargesDetails { Receiable = rec, Received = recd, Resi_Receiable = Rrec, Resi_Received = Rrecd, Com_Receiable = Crec, Com_Received = Crecd, ServiceChargesList = res3 };
            return PartialView(res);
        }
        public ActionResult ElectricityCharges()
        {
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }
        public ActionResult ElectricityChargesBill(DateTime? month, DateTime? year, long?[] blockid)
        {
            List<Sp_Get_Report_ElectricityCharges_Result> res3 = new List<Sp_Get_Report_ElectricityCharges_Result>();
            List<string> blocklist = new List<string>();

            if (blockid != null)
            {
                foreach (long index in blockid)
                {
                    var res13 = db.Sp_Get_Report_ElectricityCharges(month, year, index).ToList<Sp_Get_Report_ElectricityCharges_Result>();
                    res3.AddRange(res13);
                    var list = res13.Select(x => x.Block).FirstOrDefault();
                    blocklist.Add(list);
                }
            }
            else
            {
                var res1 = db.Sp_Get_Report_ElectricityCharges(month, year, null).ToList<Sp_Get_Report_ElectricityCharges_Result>();
                res3.AddRange(res1);

            }

            var rec = res3.Sum(x => x.Grand_Total);
            var recd = res3.Sum(x => x.Amount_Paid);
            var Rrec = res3.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Grand_Total);
            var Rrecd = res3.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Amount_Paid);
            var Crec = res3.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Grand_Total);
            var Crecd = res3.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Amount_Paid);

            if (blockid != null)
            {
                ViewBag.Block = String.Join(",", blocklist.Cast<string>().ToArray()); ;

            }
            else
            {
                ViewBag.meherestatedevelopers = "Meher Estate Developers";
            }
            var res = new ElectricityChargesDetails { Receiveable = rec, Received = recd, Resi_Receiable = Rrec, Resi_Received = Rrecd, Com_Receiable = Crec, Com_Received = Crecd, ElectricityChargesList = res3 };
            return PartialView(res);
        }
        public ActionResult ServiceChargesBills_Short()
        {
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return PartialView();
        }
        public ActionResult ServiceCharges_Short(DateTime? month, DateTime? year, long?[] blockid)
        {
            List<Sp_Get_Report_ServiceCharges_Result> res3 = new List<Sp_Get_Report_ServiceCharges_Result>();
            List<string> blocklist = new List<string>();
            if (blockid != null)
            {
                foreach (long index in blockid)
                {
                    var res13 = db.Sp_Get_Report_ServiceCharges(month, year, index).ToList<Sp_Get_Report_ServiceCharges_Result>();
                    res3.AddRange(res13);
                    var list = res13.Select(x => x.Block).FirstOrDefault();
                    blocklist.Add(list);
                }
            }
            else
            {
                var res1 = db.Sp_Get_Report_ServiceCharges(month, year, null).ToList<Sp_Get_Report_ServiceCharges_Result>();
                res3.AddRange(res1);
            }
            var rec = res3.Sum(x => x.Grand_Total);
            var recd = res3.Sum(x => x.Amount_Paid);
            var Rrec = res3.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Grand_Total);
            var Rrecd = res3.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Amount_Paid);
            var Crec = res3.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Grand_Total);
            var Crecd = res3.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Amount_Paid);

            List<Electricity_Report> bb1 = new List<Electricity_Report>();
            List<Electricity_Report> bb2 = new List<Electricity_Report>();

            foreach (var item in res3.GroupBy(x => x.Type))
            {
                Electricity_Report tr = new Electricity_Report();
                tr.name = item.Key;
                tr.y = item.Sum(x => x.Grand_Total);

                bb1.Add(tr);
            }
            foreach (var item in res3.GroupBy(x => x.Type))
            {
                Electricity_Report tr = new Electricity_Report();
                tr.name = item.Key;
                tr.y = item.Sum(x => x.Amount_Paid);

                bb2.Add(tr);
            }

            if (blockid != null)
            {
                ViewBag.Block = String.Join(",", blocklist.Cast<string>().ToArray()); ;
            }
            else
            {
                ViewBag.meherestatedevelopers = "Meher Estate Developers";
            }
            var res = new ServiceChargesDetails { Receiable = rec, Received = recd, Resi_Receiable = Rrec, Resi_Received = Rrecd, Com_Receiable = Crec, Com_Received = Crecd, ServiceChargesList = null, totalcounter1 = bb1, totalcounter2 = bb2 };
            return PartialView(res);
        }
        public ActionResult ElectricityChargesBill_Short()
        {
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return PartialView();
        }
        public ActionResult ElectricityCharges_Short(DateTime? month, DateTime? year, long?[] blockid)
        {
            List<Sp_Get_Report_ElectricityCharges_Result> res3 = new List<Sp_Get_Report_ElectricityCharges_Result>();
            List<string> blocklist = new List<string>();

            if (blockid != null)
            {
                foreach (long index in blockid)
                {
                    var res13 = db.Sp_Get_Report_ElectricityCharges(month, year, index).ToList();
                    res3.AddRange(res13);
                    var list = res13.Select(x => x.Block).FirstOrDefault();
                    blocklist.Add(list);
                }
            }
            else
            {
                var res1 = db.Sp_Get_Report_ElectricityCharges(month, year, null).ToList();
                res3.AddRange(res1);
            }

            var rec = res3.Sum(x => x.Grand_Total);
            var recd = res3.Sum(x => x.Amount_Paid);
            var Rrec = res3.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Grand_Total);
            var Rrecd = res3.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Amount_Paid);
            var Crec = res3.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Grand_Total);
            var Crecd = res3.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Amount_Paid);

            List<Electricity_Report> bb1 = new List<Electricity_Report>();
            List<Electricity_Report> bb2 = new List<Electricity_Report>();

            foreach (var item in res3.GroupBy(x => x.Type))
            {
                Electricity_Report tr = new Electricity_Report();
                tr.name = item.Key;
                tr.y = item.Sum(x => x.Grand_Total);

                bb1.Add(tr);
            }
            foreach (var item in res3.GroupBy(x => x.Type))
            {
                Electricity_Report tr = new Electricity_Report();
                tr.name = item.Key;
                tr.y = item.Sum(x => x.Amount_Paid);

                bb2.Add(tr);
            }


            if (blockid != null)
            {
                ViewBag.Block = String.Join(",", blocklist.Cast<string>().ToArray()); ;
            }
            else
            {
                ViewBag.meherestatedevelopers = "Meher Estate Developers";
            }

            var res = new ElectricityChargesDetails { Receiveable = rec, Received = recd, Resi_Receiable = Rrec, Resi_Received = Rrecd, Com_Receiable = Crec, Com_Received = Crecd, ElectricityChargesList = null, totalcounter1 = bb1, totalcounter2 = bb2 };
            return PartialView(res);
        }
        public ActionResult PlotBookingRep()
        {

            return View();
        }
        //................ Booking Reports
        [Authorize(Roles = "CEO,Financial Dashboard,Administrator")]
        public ActionResult PlotBooking(DateTime Startdate, DateTime? Enddate)
        {
            var res = db.SP_Reports_RegisterPlots(Startdate, Enddate).ToList();
            ViewBag.From = Startdate;
            ViewBag.To = Enddate;

            List<PaymentTypes_Report> pt = new List<PaymentTypes_Report>();

            int i = 0;

            var colors = Nomenclature.HighChartsColors().ToList();

            foreach (var item in res.GroupBy(x => x.Block))
            {

                try
                {
                    var c = item.Where(x => x != null).ToList();
                    List<string> cat = new List<string>();
                    List<decimal?> dat = new List<decimal?>();

                    foreach (var g in c.GroupBy(x => new { x.Type }))
                    {
                        cat.Add(item.Key + ": " + g.Key.Type);
                        dat.Add(g.Sum(x => x.Total_Plots_Registered));
                    }

                    PaymentTypes_Report a = new PaymentTypes_Report()
                    {
                        y = c.Sum(x => x.Total_Plots_Registered),
                        color = colors[i],
                        drilldown = new DrillDown()
                        {
                            name = item.Key,
                            categories = new List<string>(),
                            data = new List<decimal?>()
                        }
                    };
                    a.drilldown.categories = cat;
                    a.drilldown.data = dat;
                    pt.Add(a);
                    i++;
                }
                catch (Exception e)
                {

                }


            }

            ViewBag.Categories = res.GroupBy(x => x.Block).Select(x => x.Key);

            return PartialView(pt);

        }
        public JsonResult PlotBookingList(string Block, DateTime? From, DateTime? To)
        {
            var res = db.SP_Reports_RegisterPlots_List(From, To).Where(x => x.Block == Block).ToList();
            return Json(res);
        }
        //[Authorize(Roles = "CEO,Financial Dashboard")]
        //public ActionResult PlotBookingBarGraph(DateTime Startdate, DateTime? Enddate)
        //{
        //    var res = db.SP_Reports_RegisterPlots(Startdate, Enddate).ToList();
        //    List<string> cat = new List<string>();
        //    cat = res.Where(x => x.Block != null).Select(x => x.Block).Distinct().ToList();
        //    IDictionary<string, int> Blk = new Dictionary<string, int>();
        //    for (int i = 0; i < cat.Count; i++)
        //    {
        //        Blk.Add(cat[i], i);
        //    }
        //    List<Report_Series> Listrep = new List<Report_Series>();
        //    foreach (var g in res.GroupBy(x => x.Size))
        //    {
        //        Report_Series report = new Report_Series()
        //        {
        //            name = g.Key,
        //            data = new int?[cat.Count]
        //        };
        //        foreach (var item in g)
        //        {
        //            var place = Blk.Where(x => x.Key == item.Block).Select(x => x.Value).SingleOrDefault();
        //            report.data[place] = item.total_Plots_Register;
        //        }
        //        Listrep.Add(report);
        //    }
        //    Report_FilesStatus data = new Report_FilesStatus()
        //    {
        //        Categories = new List<string>(),
        //        Series = new List<Report_Series>()
        //    };
        //    data.Series.AddRange(Listrep);
        //    data.Categories.AddRange(cat);
        //    return PartialView(data);

        //}
        //.....................Transfer Reports...............................
        [Authorize(Roles = "CEO,Financial Dashboard,Administrator")]
        public ActionResult PlotTranfer(DateTime Startdate, DateTime? Enddate)
        {
            var res = db.Sp_Reports_FileTransfer(Startdate, Enddate).ToList();
            ViewBag.From = Startdate;
            ViewBag.To = Enddate;
            List<PaymentTypes_Report> pt = new List<PaymentTypes_Report>();

            int i = 0;

            var colors = Nomenclature.HighChartsColors().ToList();

            foreach (var item in res.GroupBy(x => x.Block))
            {
                try
                {
                    List<string> cat = new List<string>();
                    List<decimal?> dat = new List<decimal?>();
                    foreach (var g in item.GroupBy(x => new { x.Plot_Type }))
                    {
                        cat.Add(item.Key + ": " + g.Key.Plot_Type);
                        dat.Add(g.Sum(x => x.No_of_Files));
                    }

                    PaymentTypes_Report a = new PaymentTypes_Report()
                    {
                        y = item.Sum(x => x.No_of_Files),
                        color = colors[i],
                        drilldown = new DrillDown()
                        {
                            name = item.Key,
                            categories = new List<string>(),
                            data = new List<decimal?>()
                        }
                    };
                    a.drilldown.categories = cat;
                    a.drilldown.data = dat;
                    pt.Add(a);
                }
                catch (Exception e)
                {

                }

                i++;
            }

            ViewBag.Categories = res.GroupBy(x => x.Block).Select(x => x.Key);

            return PartialView(pt);

        }
        public JsonResult PlotTranferList(string Block, DateTime? Startdate, DateTime? Enddate)
        {
            var res = db.Sp_Reports_FileTransfer_List(Startdate, Enddate).Where(x => x.Block == Block).ToList();
            return Json(res);
        }
        //[Authorize(Roles = "CEO,Financial Dashboard")]
        //public ActionResult PlotTransferBarGraph(DateTime Startdate, DateTime? Enddate)
        //{
        //    var res = db.Sp_Reports_FileTransfer(Startdate, Enddate).ToList();

        //    List<string> cat = new List<string>();
        //    cat = res.Where(x => x.Block != null).Select(x => x.Block).Distinct().ToList();
        //    IDictionary<string, int> Blk = new Dictionary<string, int>();
        //    for (int i = 0; i < cat.Count; i++)
        //    {
        //        Blk.Add(cat[i], i);
        //    }

        //    List<Report_Series> Listrep = new List<Report_Series>();
        //    foreach (var g in res.GroupBy(x => x.Size))
        //    {
        //        Report_Series report = new Report_Series()
        //        {
        //            name = g.Key,
        //            data = new int?[cat.Count]
        //        };
        //        foreach (var item in g)
        //        {
        //            var place = Blk.Where(x => x.Key == item.Block).Select(x => x.Value).SingleOrDefault();
        //            report.data[place] = item.No_of_Files;
        //        }
        //        Listrep.Add(report);
        //    }

        //    Report_FilesStatus data = new Report_FilesStatus()
        //    {
        //        Categories = new List<string>(),
        //        Series = new List<Report_Series>()
        //    };
        //    data.Series.AddRange(Listrep);
        //    data.Categories.AddRange(cat);
        //    return PartialView(data);

        //}
        //.................... Payment Types ...............................
        [Authorize(Roles = "CEO,Financial Dashboard,Administrator")]
        public ActionResult PaymentTypes(DateTime Startdate, DateTime? Enddate)
        {
            try
            {
                var res = db.Sp_Reports_Payments(Startdate, Enddate).ToList();
                List<PaymentTypes_Report> pt = new List<PaymentTypes_Report>();
                var cash = res.SingleOrDefault(x => x.PaymentType == PaymentMethod.Cash.ToString());
                var cheq = res.SingleOrDefault(x => x.PaymentType == PaymentMethod.Cheque.ToString());

                PaymentTypes_Report a = new PaymentTypes_Report()
                {
                    y = cash.total_amount,
                    color = "",
                    drilldown = new DrillDown()
                    {
                        name = cash.PaymentType,
                        categories = new List<string>() {
                            cash.PaymentType
                        },
                        data = new List<decimal?>()
                        {
                            cash.total_amount
                        }
                    }
                };
                pt.Add(a);

                if (cheq != null)
                {
                    PaymentTypes_Report b = new PaymentTypes_Report()
                    {
                        y = cheq.total_amount,
                        color = "",
                        drilldown = new DrillDown()
                        {
                            name = cheq.PaymentType,
                            categories = new List<string>() {
                            cheq.PaymentType
                        },
                            data = new List<decimal?>()
                        {
                            cheq.total_amount
                        }
                        }
                    };
                    pt.Add(b);
                }


                string[] Type = { "Cash", "Cheque" };
                List<string> cat = new List<string>();
                List<decimal?> dat = new List<decimal?>();
                var sum = res.Where(x => !(Type.Contains(x.PaymentType))).Sum(x => x.total_amount);
                foreach (var item in res.Where(x => !(Type.Contains(x.PaymentType))))
                {
                    cat.Add(item.PaymentType);
                    dat.Add(item.total_amount);
                }
                PaymentTypes_Report c = new PaymentTypes_Report()
                {
                    y = sum,
                    color = "",
                    drilldown = new DrillDown()
                    {
                        name = "Online",
                        categories = new List<string>(),
                        data = new List<decimal?>()
                    }
                };
                c.drilldown.categories = cat;
                c.drilldown.data = dat;
                pt.Add(c);

                return PartialView(pt);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult RevenueReport(DateTime? From, DateTime? To)
        {
            try
            {
                var res = db.Sp_Reports_Revenue(From, To).ToList();
                var cashier = db.Users.Where(x => x.Roles.Any(y => y.Name == "Cashier")).Select(x => x.Id).ToList();
                List<long?> Cashiers = cashier.ConvertAll(i => (long?)i);

                var Counter = res.Where(x => Cashiers.Contains(x.Userid)).ToList();
                var Bank = res.Where(x => x.ReceiptNo.Contains("HBL") || x.ReceiptNo.Contains("DIB")).ToList();
                var Portal = res.Where(x => x.PaymentType == "Debit/Credit Card").ToList();

                List<ChartParentChildModel> Data = new List<ChartParentChildModel>();

                if (res.Any())
                {
                    ChartParentChildModel c = new ChartParentChildModel
                    {
                        id = "0",
                        name = "Total Revenue",
                        parent = "",
                        value = res.Sum(x => x.Amount)
                    };
                    Data.Add(c);

                    if (Counter.Any())
                    {
                        ChartParentChildModel c1 = new ChartParentChildModel
                        {
                            id = "1",
                            name = "Counters Collection",
                            parent = "0",
                            value = Counter.Sum(x => x.Amount)
                        };
                        Data.Add(c1);
                    }
                    if (Bank.Any())
                    {
                        ChartParentChildModel c1 = new ChartParentChildModel
                        {
                            id = "2",
                            name = "Bank",
                            parent = "0",
                            value = Bank.Sum(x => x.Amount)
                        };
                        Data.Add(c1);
                    }
                    if (Portal.Any())
                    {
                        ChartParentChildModel c1 = new ChartParentChildModel
                        {
                            id = "3",
                            name = "Customer Portal",
                            parent = "0",
                            value = Portal.Sum(x => x.Amount)
                        };
                        Data.Add(c1);
                    }


                    if (Counter.Any())
                    {
                        int count = 1;
                        foreach (var g in Counter.GroupBy(x => x.PaymentType))
                        {
                            ChartParentChildModel D = new ChartParentChildModel()
                            {
                                id = "1." + count,
                                name = g.Key,
                                parent = "1",
                                value = g.Sum(x => x.Amount)
                            };
                            Data.Add(D);
                            int count1 = 1;
                            foreach (var gg in g.GroupBy(x => x.Type))
                            {
                                ChartParentChildModel DD = new ChartParentChildModel()
                                {
                                    id = "1." + count + "." + count1,
                                    name = gg.Key,
                                    parent = "1." + count,
                                    value = gg.Sum(x => x.Amount)
                                };
                                Data.Add(DD);
                                int count2 = 1;
                                foreach (var ggg in gg.GroupBy(x => x.Block))
                                {
                                    ChartParentChildModel DDD = new ChartParentChildModel()
                                    {
                                        id = "1." + count + "." + count1 + "." + count2,
                                        name = ggg.Key,
                                        parent = "1." + count + "." + count1,
                                        value = ggg.Sum(x => x.Amount)
                                    };
                                    Data.Add(DDD);
                                    count2++;
                                }
                                count1++;

                            }
                            count++;
                        }
                    }

                    //
                    if (Bank.Any())
                    {
                        var HBL = Bank.Where(x => x.ReceiptNo.Contains("HBL"));
                        var DIB = Bank.Where(x => x.ReceiptNo.Contains("DIB"));
                        if (HBL.Any())
                        {
                            int count = 1;
                            foreach (var g in HBL.GroupBy(x => x.PaymentType))
                            {
                                ChartParentChildModel D = new ChartParentChildModel()
                                {
                                    id = "2." + count,
                                    name = "HBL",
                                    parent = "2",
                                    value = g.Sum(x => x.Amount)
                                };
                                Data.Add(D);
                                int count1 = 1;
                                foreach (var gg in g.GroupBy(x => x.Type))
                                {
                                    ChartParentChildModel DD = new ChartParentChildModel()
                                    {
                                        id = "2." + count + "." + count1,
                                        name = gg.Key,
                                        parent = "2." + count,
                                        value = gg.Sum(x => x.Amount)
                                    };
                                    Data.Add(DD);
                                    int count2 = 1;
                                    foreach (var ggg in gg.GroupBy(x => x.Block))
                                    {
                                        ChartParentChildModel DDD = new ChartParentChildModel()
                                        {
                                            id = "2." + count + "." + count1 + "." + count2,
                                            name = ggg.Key,
                                            parent = "2." + count + "." + count1,
                                            value = ggg.Sum(x => x.Amount)
                                        };
                                        Data.Add(DDD);
                                        count2++;
                                    }
                                    count1++;
                                }
                                count++;
                            }

                        }
                        if (DIB.Any())
                        {
                            int count = 1;
                            foreach (var g in DIB.GroupBy(x => x.PaymentType))
                            {
                                ChartParentChildModel D = new ChartParentChildModel()
                                {
                                    id = "3." + count,
                                    name = "DIB",
                                    parent = "2",
                                    value = g.Sum(x => x.Amount)
                                };
                                Data.Add(D);
                                int count1 = 1;
                                foreach (var gg in g.GroupBy(x => x.Type))
                                {
                                    ChartParentChildModel DD = new ChartParentChildModel()
                                    {
                                        id = "3." + count + "." + count1,
                                        name = gg.Key,
                                        parent = "3." + count,
                                        value = gg.Sum(x => x.Amount)
                                    };
                                    Data.Add(DD);
                                    int count2 = 1;
                                    foreach (var ggg in gg.GroupBy(x => x.Block))
                                    {
                                        ChartParentChildModel DDD = new ChartParentChildModel()
                                        {
                                            id = "3." + count + "." + count1 + "." + count2,
                                            name = ggg.Key,
                                            parent = "3." + count + "." + count1,
                                            value = ggg.Sum(x => x.Amount)
                                        };
                                        Data.Add(DDD);
                                        count2++;
                                    }
                                    count1++;
                                }
                                count++;
                            }

                        }
                    }

                    //
                    if (Portal.Any())
                    {
                        int count = 1;
                        foreach (var g in Portal.GroupBy(x => x.PaymentType))
                        {
                            ChartParentChildModel D = new ChartParentChildModel()
                            {
                                id = "4." + count,
                                name = g.Key,
                                parent = "3",
                                value = g.Sum(x => x.Amount)
                            };
                            Data.Add(D);
                            int count1 = 1;
                            foreach (var gg in g.GroupBy(x => x.Type))
                            {
                                ChartParentChildModel DD = new ChartParentChildModel()
                                {
                                    id = "4." + count + "." + count1,
                                    name = gg.Key,
                                    parent = "4." + count,
                                    value = gg.Sum(x => x.Amount)
                                };
                                Data.Add(DD);
                                int count2 = 1;
                                foreach (var ggg in gg.GroupBy(x => x.Block))
                                {
                                    ChartParentChildModel DDD = new ChartParentChildModel()
                                    {
                                        id = "4." + count + "." + count1 + "." + count2,
                                        name = ggg.Key,
                                        parent = "4." + count + "." + count1,
                                        value = ggg.Sum(x => x.Amount)
                                    };
                                    Data.Add(DDD);
                                    count2++;
                                }
                                count1++;

                            }
                            count++;
                        }
                    }



                }


                return PartialView(Data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult PayableReport(DateTime? From, DateTime? To)
        {
            try
            {
                var res = db.Sp_Reports_Payable(From, To).ToList();
                var Cash = res.Where(x => x.PaymentType == "Cash").ToList();
                var Bank = res.Where(x => x.PaymentType == "Cheque" || x.PaymentType == "PayOrder").ToList();

                List<ChartParentChildModel> Data = new List<ChartParentChildModel>();

                if (res.Any())
                {
                    ChartParentChildModel c = new ChartParentChildModel
                    {
                        id = "0",
                        name = "Total Paid",
                        parent = "",
                        value = res.Sum(x => x.Amount)
                    };
                    Data.Add(c);

                    if (Cash.Any())
                    {
                        ChartParentChildModel c1 = new ChartParentChildModel
                        {
                            id = "1",
                            name = "Cash",
                            parent = "0",
                            value = Cash.Sum(x => x.Amount)
                        };
                        Data.Add(c1);
                    }
                    if (Bank.Any())
                    {
                        ChartParentChildModel c1 = new ChartParentChildModel
                        {
                            id = "2",
                            name = "Bank",
                            parent = "0",
                            value = Bank.Sum(x => x.Amount)
                        };
                        Data.Add(c1);
                    }
                    if (Cash.Any())
                    {
                        int count = 1;
                        foreach (var g in Cash.GroupBy(x => x.Type))
                        {
                            ChartParentChildModel D = new ChartParentChildModel()
                            {
                                id = "1." + count,
                                name = g.Key,
                                parent = "1",
                                value = g.Sum(x => x.Amount)
                            };
                            Data.Add(D);
                            {
                                //int count1 = 1;
                                //foreach (var gg in g.GroupBy(x => x.Type))
                                //{
                                //    ChartParentChildModel DD = new ChartParentChildModel()
                                //    {
                                //        id = "1." + count + "." + count1,
                                //        name = gg.Key,
                                //        parent = "1." + count,
                                //        value = gg.Sum(x => x.Amount)
                                //    };
                                //    Data.Add(DD);
                                //    int count2 = 1;
                                //    foreach (var ggg in gg.GroupBy(x => x.Block))
                                //    {
                                //        ChartParentChildModel DDD = new ChartParentChildModel()
                                //        {
                                //            id = "1." + count + "." + count1 + "." + count2,
                                //            name = ggg.Key,
                                //            parent = "1." + count + "." + count1,
                                //            value = ggg.Sum(x => x.Amount)
                                //        };
                                //        Data.Add(DDD);
                                //        count2++;
                                //    }
                                //    count1++;
                                //}
                            }
                            count++;
                        }
                    }
                    if (Bank.Any())
                    {
                        int count = 1;
                        foreach (var g in Bank.GroupBy(x => x.Type))
                        {
                            ChartParentChildModel D = new ChartParentChildModel()
                            {
                                id = "2." + count,
                                name = "Bank",
                                parent = "2",
                                value = g.Sum(x => x.Amount)
                            };
                            Data.Add(D);
                            {
                                //int count1 = 1;
                                //foreach (var gg in g.GroupBy(x => x.Type))
                                //{
                                //    ChartParentChildModel DD = new ChartParentChildModel()
                                //    {
                                //        id = "2." + count + "." + count1,
                                //        name = gg.Key,
                                //        parent = "2." + count,
                                //        value = gg.Sum(x => x.Amount)
                                //    };
                                //    Data.Add(DD);
                                //    int count2 = 1;
                                //    foreach (var ggg in gg.GroupBy(x => x.Block))
                                //    {
                                //        ChartParentChildModel DDD = new ChartParentChildModel()
                                //        {
                                //            id = "2." + count + "." + count1 + "." + count2,
                                //            name = ggg.Key,
                                //            parent = "2." + count + "." + count1,
                                //            value = ggg.Sum(x => x.Amount)
                                //        };
                                //        Data.Add(DDD);
                                //        count2++;
                                //    }
                                //    count1++;
                                //}
                            }
                            count++;
                        }

                    }
                }
                return PartialView(Data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Authorize(Roles = "CEO,Financial Dashboard,Administrator")]
        public ActionResult PaymentBarTypes(DateTime Startdate, DateTime? Enddate)
        {
            try
            {
                var res = db.Sp_Reports_Payments(Startdate, Enddate).ToList();
                List<PaymentTypes_Report> pt = new List<PaymentTypes_Report>();
                var cash = res.SingleOrDefault(x => x.PaymentType == PaymentMethod.Cash.ToString());
                var cheq = res.SingleOrDefault(x => x.PaymentType == PaymentMethod.Cheque.ToString());

                PaymentTypes_Report a = new PaymentTypes_Report()
                {
                    y = cash.total_amount,
                    color = "",
                    drilldown = new DrillDown()
                    {
                        name = cash.PaymentType,
                        categories = new List<string>() {
                            cash.PaymentType
                        },
                        data = new List<decimal?>()
                        {
                            cash.total_amount
                        }
                    }
                };
                pt.Add(a);

                PaymentTypes_Report b = new PaymentTypes_Report()
                {
                    y = cheq.total_amount,
                    color = "",
                    drilldown = new DrillDown()
                    {
                        name = cheq.PaymentType,
                        categories = new List<string>() {
                            cheq.PaymentType
                        },
                        data = new List<decimal?>()
                        {
                            cheq.total_amount
                        }
                    }
                };
                pt.Add(b);

                string[] Type = { "Cash", "Cheque" };
                List<string> cat = new List<string>();
                List<decimal?> dat = new List<decimal?>();
                var sum = res.Where(x => !(Type.Contains(x.PaymentType))).Sum(x => x.total_amount);
                foreach (var item in res.Where(x => !(Type.Contains(x.PaymentType))))
                {
                    cat.Add(item.PaymentType);
                    dat.Add(item.total_amount);
                }
                PaymentTypes_Report c = new PaymentTypes_Report()
                {
                    y = sum,
                    color = "",
                    drilldown = new DrillDown()
                    {
                        name = "Online",
                        categories = new List<string>(),
                        data = new List<decimal?>()
                    }
                };
                c.drilldown.categories = cat;
                c.drilldown.data = dat;
                pt.Add(c);

                return PartialView(pt);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //..................... Installment report
        public ActionResult AllMonthlyInstallment()
        {
            var blocks = db.RealEstate_Blocks.Select(x => new NameValuestring { Value = x.Id.ToString(), Name = x.Block_Name }).ToList();
            var Projects = db.RealEstate_Projects.Select(x => new NameValuestring { Value = "B-" + x.Id, Name = x.Project_Name }).ToList();
            //.Where(x => x.Type == "Building")
            var all = new NameValuestring()
            {
                Name = "All",
                Value = "0"
            };
            blocks.Add(all);
            blocks.AddRange(Projects);

            ViewBag.Type = new SelectList(blocks, "Value", "Name");
            return PartialView();
        }


        [Authorize(Roles = "CEO,Financial Dashboard,Administrator")]
        public ActionResult MonthlyInstallment(DateTime? From, DateTime? To, string Type, string TypeName)
        {
            if (From == null || To == null)
            {
                From = DateTime.Now;
                To = DateTime.Now;
            }
            Report_Amounts data = new Report_Amounts()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal>()
            };
            ViewBag.Block = TypeName;
            int val = 0;
            if (Type != "0")
            {
                if (Char.IsDigit(Type, 0))
                {
                    val = Convert.ToInt32(Type);
                    Type = null;
                }
                else
                {
                    val = Convert.ToInt32(Type.Split('-')[1]);
                    Type = "B";
                }
            }
            else
            {
                Type = "All";
            }

            if (From.Value.Month == To.Value.Month && From.Value.Year == To.Value.Year)
            {
                data = SingleMonthReport(From, val, Type);
            }
            else
            {
                data = MultiMonthReport(From, To, val, Type);
            }

            return PartialView(data);
        }
        public Report_Amounts SingleMonthReport(DateTime? From, int? TypeId, string Type)
        {
            var res = db.Sp_Reports_MonthlyRecovery(From, TypeId, Type).ToList();
            List<string> cat = new List<string>();
            cat = res.OrderBy(x => x.DATE).Select(x => Convert.ToDateTime(x.DATE).Date.ToString("dd/MM/yyyy")).Distinct().ToList();
            List<Report_Series_Decimal> Listrep = new List<Report_Series_Decimal>();
            Report_Series_Decimal r1 = new Report_Series_Decimal()
            {
                name = "Amount Received",
                data = new decimal?[res.Count]
            };
            Report_Series_Decimal r2 = new Report_Series_Decimal()
            {
                name = "Expected",
                data = new decimal?[res.Count]
            };
            decimal? sum = 0;
            var newres = res.Select(x => x.AmountReceived).ToArray();
            for (int i = 0; i < newres.Length; i++)
            {
                sum = sum + newres[i];
                r1.data[i] = sum;
            }
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            r2.data = res.Select(x => x.AmountExpected).ToArray();
            //var data = res.Select(x => x.AmountExpected).ToArray();
            /*
                    decimal? sum = 0,exe_sum = 0;
            var newres = res.Select(x => x.AmountReceived).ToArray();
            for (int i = 0; i < newres.Length; i++)
            {
                sum = sum + newres[i];
                r1.data[i] = sum;
            }
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var ttl_Amt = res.Select(x => x.AmountExpected).FirstOrDefault();
            var perday = ttl_Amt / days;
            for (int i = 0; i < days; i++)
            {
                exe_sum = exe_sum + perday;
                r2.data[i] = exe_sum;
            }
             
             */


            Listrep.Add(r1);
            Listrep.Add(r2);
            Report_Amounts data = new Report_Amounts()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return data;
        }
        public Report_Amounts MultiMonthReport(DateTime? From, DateTime? To, int? TypeId, string Type)
        {
            var res1 = db.Sp_Reports_MonthlyExpectedInstallment(From, To, TypeId, Type).ToList();
            var res2 = db.Sp_Reports_MonthlyInstallmentReceived(From, To, TypeId, Type).ToList();
            List<string> cat = new List<string>();
            cat = res1.Select(x => x.MonthYear).Distinct().ToList();
            List<InstallmentReceived_Expected> res = new List<InstallmentReceived_Expected>();
            foreach (var item in res1)
            {
                InstallmentReceived_Expected sr = new InstallmentReceived_Expected()
                {
                    Expected = item.sum_amount,
                    Date = item.MonthYear,
                    Received = (res2.Any(x => x.MonthYear == item.MonthYear)) ? res2.SingleOrDefault(x => x.MonthYear == item.MonthYear).sum_amount : 0
                };
                res.Add(sr);
            }
            List<Report_Series_Decimal> Listrep = new List<Report_Series_Decimal>();

            Report_Series_Decimal r1 = new Report_Series_Decimal()
            {
                name = "Amount Received",
                data = new decimal?[res.Count]
            };
            Report_Series_Decimal r2 = new Report_Series_Decimal()
            {
                name = "Expected",
                data = new decimal?[res.Count]
            };
            var newres = res.Select(x => x.Received).ToArray();
            for (int i = 0; i < newres.Length; i++)
            {
                r1.data[i] = newres[i];
            }
            r2.data = res.Select(x => x.Expected).ToArray();

            Listrep.Add(r1);
            Listrep.Add(r2);
            Report_Amounts data = new Report_Amounts()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return data;
        }
        public ActionResult PlotsRecovery(DateTime? From, DateTime? To)
        {
            var res = db.Sp_Get_Reports_BlockOutstanding(From, To).ToList();
            List<string> cat = new List<string>();
            cat = res.Select(x => x.Block_Name).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            double j = 0;
            List<Report_Series_Decimal> Listrep = new List<Report_Series_Decimal>();
            foreach (var g in res.GroupBy(x => x.Total))
            {
                Report_Series_Decimal report = new Report_Series_Decimal()
                {
                    name = g.Key,
                    data = new decimal?[cat.Count],
                    pointPadding = 0.3 + j

                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == item.Block_Name).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Amount;
                }
                Listrep.Add(report);
                j = j + 0.1;
            }

            Report_Amounts data = new Report_Amounts()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public ActionResult FilesYearlyRecovery(long? block, string blockName, string propertytype)
        {
            var res = db.Sp_Get_Reports_FilesInstallment_Year(1, "Residential").ToList();
            ViewBag.titleName = string.Concat(blockName, " Block All Recovery");
            ViewBag.blockId = block;
            ViewBag.propertyType = propertytype;

            List<string> cat = new List<string>();
            cat = res.Select(x => x.Year.ToString()).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            List<Report_Series_Decimal_Stack> Listrep = new List<Report_Series_Decimal_Stack>();

            //   Advance
            Report_Series_Decimal_Stack report1 = new Report_Series_Decimal_Stack()
            {
                name = "Advance",
                data = new decimal?[cat.Count],
                stack = "Advance"
            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report1.data[place] = item.Advance_Amt;
            }
            Listrep.Add(report1);
            //   Paid Installments
            Report_Series_Decimal_Stack report2 = new Report_Series_Decimal_Stack()
            {
                name = "Paid - Installment",
                data = new decimal?[cat.Count],
                stack = "Installment"
            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report2.data[place] = item.PaidInst_Amt;
            }
            Listrep.Add(report2);
            // Not - Paid Installments
            Report_Series_Decimal_Stack report3 = new Report_Series_Decimal_Stack()
            {
                name = "Not Paid - Installment",
                data = new decimal?[cat.Count],
                stack = "Installment"
            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report3.data[place] = item.NotPaidInst_Amt;
            }
            Listrep.Add(report3);
            // Not - Paid Balloting
            Report_Series_Decimal_Stack report4 = new Report_Series_Decimal_Stack()
            {
                name = "Not Paid - Balloting",
                data = new decimal?[cat.Count],
                stack = "Balloting"

            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report4.data[place] = item.NotPaid_Balloting_Amt;
            }
            Listrep.Add(report4);
            // Paid Balloting
            Report_Series_Decimal_Stack report5 = new Report_Series_Decimal_Stack()
            {
                name = "Paid - Balloting",
                data = new decimal?[cat.Count],
                stack = "Balloting"

            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report5.data[place] = item.Paid_Balloting_Amt;
            }
            Listrep.Add(report5);
            // Not - Paid Development charges
            Report_Series_Decimal_Stack report6 = new Report_Series_Decimal_Stack()
            {
                name = "Not Paid - Development Charges",
                data = new decimal?[cat.Count],
                stack = "Development Charges"

            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report6.data[place] = item.NotPaid_DC_Amt;
            }
            Listrep.Add(report6);
            // Paid Balloting
            Report_Series_Decimal_Stack report7 = new Report_Series_Decimal_Stack()
            {
                name = "Paid Development Charges",
                data = new decimal?[cat.Count],
                stack = "Development Charges"
            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report7.data[place] = item.Paid_DC_Amt;
            }
            Listrep.Add(report7);
            // Not - Special Preferences
            Report_Series_Decimal_Stack report8 = new Report_Series_Decimal_Stack()
            {
                name = "Not Paid - Special Preference",
                data = new decimal?[cat.Count],
                stack = "Special_Preference"

            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report8.data[place] = item.NotPaid_SP_Amt;
            }
            Listrep.Add(report8);
            // Paid Special Preferences
            Report_Series_Decimal_Stack report9 = new Report_Series_Decimal_Stack()
            {
                name = "Paid - Special Preference",
                data = new decimal?[cat.Count],
                stack = "Special_Preference"

            };
            foreach (var item in res)
            {
                var place = Blk.Where(x => x.Key == item.Year.ToString()).Select(x => x.Value).SingleOrDefault();
                report9.data[place] = item.Paid_SP_Amt;
            }
            Listrep.Add(report9);
            Report_Amounts_Stack data = new Report_Amounts_Stack()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal_Stack>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public ActionResult TransferAllotmentLetters()
        {
            var res = db.Sp_Report_TransferAllotmentStatus().ToList();
            return View(res);
        }
        public ActionResult MapsReport()
        {

            var res1 = db.Test_Activity().ToList();
            var res = res1.Select(x => x.Description).Distinct().ToList();


            List<string> Address = new List<string>();
            foreach (var item in res)
            {
                //var seq = new char[]{ '#', ',', '&','/', '\\','@','!' };
                Regex regex = new Regex(@"[^\w\*]");
                Address.Add(regex.Replace(item, " "));
            }
            ViewBag.Res = Address;
            return View();
        }
        public ActionResult LeadsPlotSizeReports(string Status, DateTime? Startdate, DateTime? Enddate, long?[] Users, string Comp)
        {
            string users = null;
            Status = (string.IsNullOrEmpty(Status)) ? null : Status;
            if (Users != null)
            {
                users = new XElement("Users", Users.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x)
                                 ))).ToString();
            }
            else
            {
                if (Comp == "SAM")
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("sa.marketing")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
                else
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("property")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
            }
            var res = db.Sp_Reports_LeadsPlotsSize(users, Startdate, Enddate, Status).ToList();

            List<string> cat = new List<string>();
            cat = res.Select(x => x.Size).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            List<Report_Series_Stack> Listrep = new List<Report_Series_Stack>();
            string stat = "";
            IDictionary<string, string> color = new Dictionary<string, string>();
            color.Add("1 Marla", "#2f7ed8");
            color.Add("2 Marla", "#0d233a");
            color.Add("3 Marla", "#8bbc21");
            color.Add("4 Marla", "#910000");
            color.Add("5 Marla", "#492970");
            color.Add("6 Marla", "#f28f43");
            color.Add("7 Marla", "#77a1e5");
            color.Add("8 Marla", "#c42525");
            color.Add("9 Marla", "#a6c96a");
            color.Add("10 Marla", "#1aadce");

            foreach (var g in res.GroupBy(x => new { x.Name, x.Block }).OrderBy(x => x.Key.Name).ThenBy(x => x.Key.Block))
            {
                string curtstat = g.Key.Name;
                string barcolor = color.Where(x => x.Key == g.Key.Block).Select(x => x.Value).FirstOrDefault();
                string dat = (curtstat != stat) ? null : ":previous";
                Report_Series_Stack report = new Report_Series_Stack()
                {
                    name = g.Key.Name,
                    data = new int?[cat.Count],
                    stack = g.Key.Block.ToString(),
                    linkedTo = dat,
                    color = barcolor
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == item.Size).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Total;
                }
                Listrep.Add(report);
                stat = curtstat;
            }
            Report_Serie_Stack data = new Report_Serie_Stack()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Stack>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public ActionResult PropertyDealsPlotSizeReports(string Status, DateTime? Startdate, DateTime? Enddate, long?[] Users, string Comp, string Type)
        {
            string users = null;
            ViewBag.Type = Type;
            Status = (string.IsNullOrEmpty(Status)) ? null : Status;
            if (Users != null)
            {
                users = new XElement("Users", Users.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x)
                                 ))).ToString();
            }
            else
            {
                if (Comp == "SAM")
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("sa.marketing")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
                else
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("property")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
            }
            var res = db.Sp_Reports_PropertyDealPlotsSize_NewLead(users, Startdate, Enddate, Status, Type).ToList();

            List<string> cat = new List<string>();
            cat = res.Select(x => x.Size).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            List<Report_Series_Stack> Listrep = new List<Report_Series_Stack>();
            string stat = "";
            IDictionary<string, string> color = new Dictionary<string, string>();
            color.Add("1 Marla", "#2f7ed8");
            color.Add("2 Marla", "#0d233a");
            color.Add("3 Marla", "#8bbc21");
            color.Add("4 Marla", "#910000");
            color.Add("5 Marla", "#492970");
            color.Add("6 Marla", "#f28f43");
            color.Add("7 Marla", "#77a1e5");
            color.Add("8 Marla", "#c42525");
            color.Add("9 Marla", "#a6c96a");
            color.Add("10 Marla", "#1aadce");

            foreach (var g in res.GroupBy(x => new { x.Name, x.Block }).OrderBy(x => x.Key.Name).ThenBy(x => x.Key.Block))
            {
                string curtstat = g.Key.Name;
                string barcolor = color.Where(x => x.Key == g.Key.Block).Select(x => x.Value).FirstOrDefault();
                string dat = (curtstat != stat) ? null : ":previous";
                Report_Series_Stack report = new Report_Series_Stack()
                {
                    name = g.Key.Name,
                    data = new int?[cat.Count],
                    stack = g.Key.Block.ToString(),
                    linkedTo = dat,
                    color = barcolor
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == item.Size).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Total;
                }
                Listrep.Add(report);
                stat = curtstat;
            }
            Report_Serie_Stack data = new Report_Serie_Stack()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Stack>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public ActionResult LeadsReports()
        {
            return View();
        }
        public ActionResult SAMLeadsReports()
        {
            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("sa.marketing")).ToList();
            ViewBag.LeadsUser = new SelectList(All, "Id", "Name");
            return PartialView();
        }
        public ActionResult PropertyExchangeLeadsReports()
        {
            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("property")).ToList();
            ViewBag.LeadsUser = new SelectList(All, "Id", "Name");
            return PartialView();
        }
        public ActionResult LeadsStatusReport(string Status, DateTime? Startdate, DateTime? Enddate, long?[] Users, string Comp)
        {
            string users = null;
            Status = (string.IsNullOrEmpty(Status)) ? null : Status;
            if (Users != null)
            {
                users = new XElement("Users", Users.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x)
                                 ))).ToString();
            }
            else
            {
                if (Comp == "SAM")
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("sa.marketing")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
                else
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("property")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
            }
            var res = db.Sp_Get_Lead_Search_Rep(users, Startdate, Enddate, Status).ToList();
            List<string> cat = new List<string>();
            cat = res.Select(x => x.AssignedToName).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            List<Report_Series> Listrep = new List<Report_Series>();
            foreach (var g in res.GroupBy(x => x.LeadStatus))
            {
                Report_Series report = new Report_Series()
                {
                    name = g.Key,
                    data = new int?[cat.Count]
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == item.AssignedToName).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Total;
                }
                Listrep.Add(report);
            }
            Report_FilesStatus data = new Report_FilesStatus()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public ActionResult LeadsRecovery(DateTime? Startdate, DateTime? Enddate, long?[] Users, string Comp)
        {
            string users = null;
            if (Users != null)
            {
                users = new XElement("Users", Users.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x)
                                 ))).ToString();
            }
            else
            {
                if (Comp == "SAM")
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("sa.marketing")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
                else
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("property")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
            }
            var res = db.Sp_Get_Leads_Recovery_Rep(users, Startdate, Enddate).ToList();
            List<string> cat = new List<string>();
            cat = res.Select(x => string.Format("{0:dd-MMM-yyyy}", x.Date)).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            List<Report_Series_Decimal> Listrep = new List<Report_Series_Decimal>();
            foreach (var g in res.GroupBy(x => x.AssignedToName))
            {
                Report_Series_Decimal report = new Report_Series_Decimal()
                {
                    name = g.Key,
                    data = new decimal?[cat.Count]
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == string.Format("{0:dd-MMM-yyyy}", item.Date)).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Total;
                }
                Listrep.Add(report);
            }
            Report_Amounts data = new Report_Amounts()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        //public ActionResult LeadsMonthlyRevenue(DateTime From, DateTime To)
        //{
        //    Report_Amounts data = new Report_Amounts()
        //    {
        //        Categories = new List<string>(),
        //        Series = new List<Report_Series_Decimal>()
        //    };
        //    if (From.Month == To.Month && From.Year == To.Year)
        //    {
        //        data = LeadsSingleMonthReport(From);
        //    }
        //    else
        //    {
        //        data = LeadsMultiMonthReport(From, To);
        //    }

        //    return PartialView(data);
        //}
        //public Report_Amounts LeadsSingleMonthReport(DateTime From)
        //{
        //    var res = db.Sp_Reports_MonthlyRecovery(From).ToList();
        //    List<string> cat = new List<string>();
        //    cat = res.OrderBy(x => x.Date).Select(x => Convert.ToDateTime(x.Date).Date.ToString("dd/MM/yyyy")).Distinct().ToList();
        //    List<Report_Series_Decimal> Listrep = new List<Report_Series_Decimal>();
        //    Report_Series_Decimal r1 = new Report_Series_Decimal()
        //    {
        //        name = "Amount Received",
        //        data = new decimal?[res.Count]
        //    };
        //    decimal? sum = 0;
        //    var newres = res.Select(x => x.AmountReceived).ToArray();
        //    for (int i = 0; i < newres.Length; i++)
        //    {
        //        sum = sum + newres[i];
        //        r1.data[i] = sum;
        //    }
        //    Listrep.Add(r1);
        //    Report_Amounts data = new Report_Amounts()
        //    {
        //        Categories = new List<string>(),
        //        Series = new List<Report_Series_Decimal>()
        //    };
        //    data.Series.AddRange(Listrep);
        //    data.Categories.AddRange(cat);
        //    return data;
        //}
        //public Report_Amounts LeadsMultiMonthReport(DateTime From, DateTime To)
        //{
        //    var res2 = db.Sp_Reports_MonthlyInstallmentReceived(From, To).ToList();
        //    List<string> cat = new List<string>();
        //    List<InstallmentReceived_Expected> res = new List<InstallmentReceived_Expected>();

        //    List<Report_Series_Decimal> Listrep = new List<Report_Series_Decimal>();

        //    Report_Series_Decimal r1 = new Report_Series_Decimal()
        //    {
        //        name = "Amount Received",
        //        data = new decimal?[res.Count]
        //    };
        //    Report_Series_Decimal r2 = new Report_Series_Decimal()
        //    {
        //        name = "Expected",
        //        data = new decimal?[res.Count]
        //    };
        //    var newres = res.Select(x => x.Received).ToArray();
        //    for (int i = 0; i < newres.Length; i++)
        //    {
        //        r1.data[i] = newres[i];
        //    }
        //    r2.data = res.Select(x => x.Expected).ToArray();

        //    Listrep.Add(r1);
        //    Listrep.Add(r2);
        //    Report_Amounts data = new Report_Amounts()
        //    {
        //        Categories = new List<string>(),
        //        Series = new List<Report_Series_Decimal>()
        //    };
        //    data.Series.AddRange(Listrep);
        //    data.Categories.AddRange(cat);
        //    return data;
        //}

        public ActionResult DealsListReport(string Status, DateTime? Startdate, DateTime? Enddate, long?[] Users, string Comp)
        {
            string users = null;
            Status = (string.IsNullOrEmpty(Status)) ? null : Status;
            if (Users != null)
            {
                users = new XElement("Users", Users.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x)
                                 ))).ToString();
            }
            else
            {
                if (Comp == "SAM")
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("sa.marketing")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
                else
                {
                    var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("property")).ToList();
                    users = new XElement("Users", All.Select(x => new XElement("Ids",
                                 new XAttribute("Id", x.Id)
                                 ))).ToString();
                }
            }
            ViewBag.From = Startdate;
            ViewBag.To = Enddate;

            var res = db.Sp_Get_DealLedger_Parameter(users, Startdate, Enddate, Status).ToList();
            return PartialView(res);
        }



        //public ActionResult 


        /////////////// 
        /// Files
        ///////////////

        public ActionResult FilesTiles()
        {
            var res = db.Sp_Get_FilesReport_Short().ToList();
            return PartialView(res);
        }

        /////////////// 
        /// Tickets Reports
        ///////////////

        public ActionResult TicketStatus(DateTime? From, DateTime? To, int?[] Dep_Id)
        {
            string chids = null;
            if (Dep_Id != null)
            {
                chids = new XElement("Dep", Dep_Id.Select(x => new XElement("Ids",
                                 new XAttribute("Ids", x)
                                 ))).ToString();

            }
            var res = db.SP_Reports_Ticket(From, To, chids).ToList();
            List<string> cat = new List<string>();
            cat = res.Select(x => x.Department).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }

            List<Report_Series> Listrep = new List<Report_Series>();
            foreach (var g in res.GroupBy(x => x.Status))
            {
                Report_Series report = new Report_Series()
                {
                    name = g.Key,
                    data = new int?[cat.Count]
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == item.Department).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Total;
                }
                Listrep.Add(report);
            }

            Report_FilesStatus data = new Report_FilesStatus()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public ActionResult HODTicketStatus(DateTime? From, DateTime? To, long?[] UserId)
        {
            string chids = null;
            long userid = long.Parse(User.Identity.GetUserId());
            if (UserId != null)
            {
                if (UserId[0] != null)
                {

                    chids = new XElement("User", UserId.Select(x => new XElement("Ids",
                                     new XAttribute("Ids", x)
                                     ))).ToString();
                }
            }
            if (chids == null)
            {
                var empid = db.Sp_Get_Employee_UserId(userid).FirstOrDefault().Id;
                var hodusers = db.Sp_Get_ManagerStatus(empid).ToList();
                chids = new XElement("User", hodusers.Select(x => new XElement("Ids",
                                (x.Loginid == null) ? null : new XAttribute("Ids", x.Loginid)
                                ))).ToString();
            }
            var res = db.SP_Reports_HODTicket(From, To, chids).ToList();
            List<string> cat = new List<string>();
            cat = res.Select(x => x.Department).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }

            List<Report_Series> Listrep = new List<Report_Series>();
            foreach (var g in res.GroupBy(x => x.Status))
            {
                Report_Series report = new Report_Series()
                {
                    name = g.Key,
                    data = new int?[cat.Count]
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == item.Department).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Total;
                }
                Listrep.Add(report);
            }

            Report_FilesStatus data = new Report_FilesStatus()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public ActionResult MyTicketStatus(DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.SP_Reports_MyTicket(From, To, userid).ToList();
            List<string> cat = new List<string>();
            cat = res.Select(x => x.Department).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            List<Report_Series> Listrep = new List<Report_Series>();
            foreach (var g in res.GroupBy(x => x.Status))
            {
                Report_Series report = new Report_Series()
                {
                    name = g.Key,
                    data = new int?[cat.Count]
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == item.Department).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Total;
                }
                Listrep.Add(report);
            }

            Report_FilesStatus data = new Report_FilesStatus()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }

        /////////////// 
        /// Dealership Reports
        ///////////////

        public ActionResult StockIssuanceReport(long Id)
        {
            var res1 = db.Sp_Get_DealershipFilesReport(Id).ToList();
            var res = (from x in res1
                       group x by new { x.Date, x.Plot_Size, x.Status } into g
                       select new { g.Key.Date, g.Key.Plot_Size, g.Key.Status, Total = g.Count() }).ToList();

            List<string> cat = new List<string>();
            cat = res.OrderBy(x => x.Date).Select(x => string.Format("{0:dd-MMM-yyy}", x.Date)).Distinct().ToList();
            IDictionary<string, int> Blk = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Blk.Add(cat[i], i);
            }
            List<Report_Series_Stack> Listrep = new List<Report_Series_Stack>();
            string stat = "";
            IDictionary<string, string> color = new Dictionary<string, string>();
            color.Add("1 Marla", "#2f7ed8");
            color.Add("2 Marla", "#0d233a");
            color.Add("3 Marla", "#8bbc21");
            color.Add("4 Marla", "#910000");
            color.Add("5 Marla", "#492970");
            color.Add("6 Marla", "#f28f43");
            color.Add("7 Marla", "#77a1e5");
            color.Add("8 Marla", "#c42525");
            color.Add("9 Marla", "#a6c96a");
            color.Add("10 Marla", "#1aadce");

            foreach (var g in res.GroupBy(x => new { x.Status, x.Plot_Size }).OrderBy(x => x.Key.Status).ThenBy(x => x.Key.Plot_Size))
            {
                string curtstat = ((FileStatus)g.Key.Status).ToString();
                string barcolor = color.Where(x => x.Key == g.Key.Plot_Size).Select(x => x.Value).FirstOrDefault();
                string dat = (curtstat != stat) ? null : ":previous";
                Report_Series_Stack report = new Report_Series_Stack()
                {
                    name = ((FileStatus)g.Key.Status).ToString(),
                    data = new int?[cat.Count],
                    stack = g.Key.Plot_Size.ToString(),
                    linkedTo = dat,
                    color = barcolor
                };
                foreach (var item in g)
                {
                    var place = Blk.Where(x => x.Key == string.Format("{0:dd-MMM-yyy}", item.Date)).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Total;
                }
                Listrep.Add(report);
                stat = curtstat;
            }
            Report_Serie_Stack data = new Report_Serie_Stack()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Stack>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return PartialView(data);
        }
        public ActionResult Reports()
        {
            List<CanceledFileReport> fileReport = new List<CanceledFileReport>();
            var cancelledFiles = db.SP_Get_CancelledFiles().ToList();
            var cancelledFilesIds_XML_For_Installments_Procedure = new XElement("FileFormData", cancelledFiles.Select(x => new XElement("FileIDs",
                new XAttribute("FileId", x.File_Form_Id)))).ToString();
            //ViewBag.test = cancelledFilesIds_XML_For_Installments_Procedure;
            var cancelledFilesInstallments = db.SP_Get_FileInstallmentDetails(cancelledFilesIds_XML_For_Installments_Procedure).ToList();

            cancelledFiles.ForEach(x => fileReport.Add(new CanceledFileReport
            {
                CancelledOn = (DateTime)x.Canceled_On,
                FileId = (long)x.File_Form_Id,
                FileNumber = x.File_Number.ToString(),
                PlotSize = x.Plot_Size.Split(' ')[0],
                Status = x.Status,
                TotalInstallmentsNotPaidAmount = cancelledFilesInstallments.Where(z => z.Status == "NotPaid" && z.File_Id == x.File_Form_Id).Sum(z => z.Amount),
                TotalInstallmentsNotPaidCount = cancelledFilesInstallments.Where(z => z.Status == "NotPaid" && z.File_Id == x.File_Form_Id).Count(),
                TotalInstallmentsPaidAmount = cancelledFilesInstallments.Where(z => z.Status == "Paid" && z.File_Id == x.File_Form_Id).Sum(z => z.Amount),
                TotalInstallmentsPaidCount = cancelledFilesInstallments.Where(z => z.Status == "Paid" && z.File_Id == x.File_Form_Id).Count(),
                TotalInstallmentsRemainAmount = cancelledFilesInstallments.Where(z => z.Status == "Pending" && z.File_Id == x.File_Form_Id).Sum(z => z.Amount),
                TotalInstallmentsRemainCount = cancelledFilesInstallments.Where(z => z.Status == "Pending" && z.File_Id == x.File_Form_Id).Count()
            }));

            return PartialView(fileReport);
        }
        public ActionResult AttendanceReport(DateTime? start, DateTime? end, long? depart, string typ)
        {
            //for present absent and leaves
            List<Sp_Get_Attendance_Report_Graphical_Result> apr;
            try
            {
                if ((start is null) || (end is null))
                {
                    //only for today
                    DateTime dt = DateTime.Now;
                    apr = db.Sp_Get_Attendance_Report_Graphical(dt, dt, depart).ToList();
                }
                else
                {
                    apr = db.Sp_Get_Attendance_Report_Graphical(start, end, depart).ToList();
                }
                DateTime now = DateTime.Now;
                var res = db.AttendanceDatas.Where(x => x.AttendanceDate.Value.Day == now.Day && x.AttendanceDate.Value.Month == now.Month && x.AttendanceDate.Value.Year == now.Year).ToList();
                foreach (var item in res)
                {

                    var rosterRecord = db.Roster_Templates.Where(x => x.Id == item.WorkCode).FirstOrDefault();
                    if (rosterRecord != null)
                    {
                        var shiftDetails = JsonConvert.DeserializeObject<List<RosterDetails>>(rosterRecord.Shift_Details);
                        var todaysShiftDetails = shiftDetails.Where(x => x.WeekDay.ToString() == now.ToString("dddd")).FirstOrDefault();
                        int todaysCheckinHour = todaysShiftDetails.StartHour;
                        item.IsWeeklyOff = todaysShiftDetails.IsWeekend;
                        if (item.Check_In_Time == null)
                        {
                            if (item.IsWeeklyOff)
                            {
                                item.IsInShift = false;
                            }
                            else
                            {
                                if (now.Hour >= todaysCheckinHour)
                                {
                                    item.IsInShift = true;
                                }
                                else
                                {
                                    item.IsInShift = false;
                                }



                            }
                        }
                    }
                }
                apr.ForEach(x => x.AttendanceDayString = x.AttendanceDay.Value.ToShortDateString());

                var waiting = res.Where(x => x.Check_In_Time == null && x.IsInShift == false && x.IsWeeklyOff == false).Count();
                apr.ForEach(x => x.Shiftwaiting = waiting);

                var weeklyoff = res.Where(x => x.Check_In_Time == null && x.IsInShift == false && x.IsWeeklyOff == true).Count();
                apr.ForEach(x => x.Weeklyoff = weeklyoff);

                var leaves = db.Sp_Get_AcceptedLeaves_Attendance_CurDate(DateTime.Now).Count();
                apr.ForEach(x => x.Leave = leaves);

                apr.ForEach(x => x.Absents = x.Absents - waiting - weeklyoff - leaves);

                return PartialView(apr);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return PartialView(null);
            }
        }
        public ActionResult PayrollReport(DateTime? start, DateTime? end, long? depart, string typ)
        {
            try
            {
                List<Sp_Get_PayrollReport_Result> res;

                if ((start is null) || (end is null))
                {
                    DateTime dt = DateTime.Now;
                    res = db.Sp_Get_PayrollReport(dt, dt, depart).ToList();
                }
                else
                {
                    res = db.Sp_Get_PayrollReport(start, end, depart).ToList();
                }

                return PartialView(res);
            }
            catch (Exception ex)
            {
                return PartialView();
            }
        }
        public ActionResult DetailedAttendanceReport(DateTime? start, DateTime? end, string depart, string typ)
        {
            //for present absent and leaves
            List<Sp_Get_Attendance_Period_Result> apr;
            List<Sp_Get_AcceptedLeaves_Attendance_Result> leaves;
            var __RostersInfo = db.Roster_Templates.ToList();
            __RostersInfo.ForEach(x => x.ShiftMap = JsonConvert.DeserializeObject<List<RosterDetails>>(x.Shift_Details));
            if ((start is null) || (end is null))
            {
                //only for today
                DateTime dt = DateTime.Now;
                ViewBag.start = dt.ToShortDateString();
                ViewBag.end = dt.ToShortDateString();
                ViewBag.typDep = typ;
                string skls = string.Empty;
                if (!string.IsNullOrEmpty(depart) && depart != "All")
                {
                    string[] parsed = depart.Split(',');
                    skls = new XElement("Departments", parsed.Select(x => new XElement("DepartId",
                        new XAttribute("DepId", int.Parse(x))))).ToString();
                }
                apr = db.Sp_Get_Attendance_Period(dt, dt, skls).ToList();
                leaves = db.Sp_Get_AcceptedLeaves_Attendance(dt, dt, null).ToList();
                start = dt;
                end = dt;
            }
            else
            {
                ViewBag.start = start.Value.ToShortDateString();
                ViewBag.end = end.Value.ToShortDateString();
                ViewBag.typDep = typ;
                string skls = string.Empty;
                if (!string.IsNullOrEmpty(depart) && depart != "All")
                {
                    string[] parsed = depart.Split(',');
                    skls = new XElement("Departments", parsed.Select(x => new XElement("DepartId",
                        new XAttribute("DepId", int.Parse(x))))).ToString();
                    apr = db.Sp_Get_Attendance_Period(start, end, skls).ToList();
                }
                else
                {
                    apr = db.Sp_Get_Attendance_Period(start, end, null).ToList();
                }

                leaves = db.Sp_Get_AcceptedLeaves_Attendance(start, end, null).ToList();
            }
            return PartialView(new DetailedAttendanceReportView { Attendance = apr, StartDate = start, EndDate = end, AppliedLeaves = leaves, RostersTemplatesInfo = __RostersInfo });
        }
        public ActionResult DetailedPayrollReport(DateTime? start, DateTime? end, long? depart, string typ)
        {
            List<Sp_Get_Payroll_Details_Result> res;
            if ((start is null) || end is null)
            {
                DateTime dt = DateTime.Now;
                ViewBag.start = dt.ToShortDateString();
                ViewBag.end = dt.ToShortDateString();
                ViewBag.typDep = typ;
                res = db.Sp_Get_Payroll_Details(dt, dt, depart).ToList();
            }
            else
            {
                ViewBag.start = start.Value.ToShortDateString();
                ViewBag.end = end.Value.ToShortDateString();
                ViewBag.typDep = typ;
                res = db.Sp_Get_Payroll_Details(start, end, depart).ToList();
            }
            return View(res);
        }
        public ActionResult SalaryComparativeReport(DateTime? start, DateTime? end, string typ, long[] ids)
        {
            var idData = (ids is null) ? string.Empty : new XElement("CompDepDesigs", ids.Select(x => new XElement("IdsData",
                    new XAttribute("Id", x)))).ToString();
            if (start is null || end is null)
            {
                end = DateTime.Now;
                start = end.Value.AddMonths(-3);
            }
            var res = db.Sp_Get_SalaryComparativeReport(start, end, typ, (ids is null) ? null : idData).ToList();

            foreach (var v in res)
            {
                string[] prsed = v.SalaryMonth.Split('-');
                int mon = Convert.ToInt32(prsed[0]);
                int yr = Convert.ToInt32(prsed[1]);
                DateTime dt = new DateTime(yr, mon, 1);
                v.ParsedDt = dt;
            }

            return PartialView(res.OrderBy(x => x.ParsedDt).ToList());
        }
        //public JsonResult GetCompanyOwnPropsCount()
        //{
        //    var res = db.Sp_Get_CompanyProperties().ToList().Count;
        //    return Json(res);
        //}
        //public ActionResult CompanyOwned()
        //{
        //    var res = db.Sp_Get_CompanyProperties().ToList();
        //    return View(res);
        //}

        public ActionResult LateReport()
        {


            return PartialView();
        }
        public ActionResult LateReportData(DateTime From)
        {
            var res = db.Sp_Get_LateComers(From).ToList();
            return PartialView(res);
        }
        //public ActionResult FilesTabularSummary()
        //{
        //    var filesData = db.File_Form.Select(x => new Files_ABS_Summary { BallotPlotId = x.BallotedPlot_Id, FileFormNumber = (long)x.FileFormNumber, Status = ((FileStatus)x.Status).ToString(), TypeName = x.Type, StatusVal = x.Status }).ToList();
        //    return PartialView(filesData);
        //}
        public ActionResult SAPremiumSalesReport()
        {
            CommercialController cc = new CommercialController();
            cc.UpdateCommercialUnitsBalance();
            var ProjectName = "SA Premium Homes";
            var Module = "CommercialManagement";
            var res = db.Sp_Get_ComProj_SalesReport(ProjectName, Module).ToList();
            return View(res);
        }
        //public ActionResult SAFileSalesReport()
        //{
        //    var res = db.Sp_Get_FilePlot_SalesReport("File").ToList();
        //    return View(res);
        //}
        //public ActionResult SAPlotSalesReport()
        //{
        //    var res = db.Sp_Get_FilePlot_SalesReport("Plot").ToList();
        //    return View(res);
        //}
        public ActionResult MaterialReceivingReport()
        {

            return View();
        }
        public ActionResult SearchMaterialReceivingReport(DateTime? From, DateTime? To)
        {
            if (From is null || To is null)
            {
                From = DateTime.Now;
                To = DateTime.Now;

            }
            ViewBag.From = From;
            ViewBag.To = To;
            var res = db.Sp_Get_Material_Receiving_Report(From, To).ToList();
            return PartialView(res);
        }
        public ActionResult OverDueFiles()
        {
            return View();
        }
        public ActionResult RegisterOverdue(int Status)
        {
            ViewBag.Title = "Registered Files Overdue";
            var res = db.Sp_Reports_OverDue(Status).ToList();
            return PartialView(res);
        }
        public ActionResult MonthlyDue(int? month, int? year)
        {
            DateTime date = DateTime.Now;
            if (month != null || year != null)
            {
                date = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), 1);
            }
            ViewBag.Title = "Due for Month " + string.Format("{0:MMMM - yyyy}", date);
            var res = db.Sp_Reports_MonthlyOverdue(date.Month, date.Year).ToList();
            return PartialView(res);
        }
        public ActionResult PurchaseReqReport() // Report 3
        {
            return View();
        }
        public ActionResult PurchaseReqReport_Search(string PRNo, DateTime? From, DateTime? To)
        {
            long userid = User.Identity.GetUserId<long>();
            if (From == null || To == null)
            {
                From = new DateTime(DateTime.Now.Year, 1, 1);
                To = DateTime.Now;
            }
            var res = db.Sp_Get_PurchaseReq_Report(PRNo, From, To).ToList();
            return PartialView(res);
        }
        public ActionResult PurchaseReqPndReport() // Report 4 Pending requisitions
        {
            return View();
        }
        public ActionResult PurchaseReqPndReport_Search(string PRNo)
        {
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Generate Purchase Requsition Report ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            var res = db.Sp_Get_PurchaseReq_Pndg_Report(PRNo).ToList();
            return PartialView(res);
        }
        public ActionResult PurchaseReqItemReport() // report 6
        {
            return View();
        }
        public ActionResult PurchaseReqItemReport_Search(string Grn, DateTime? From, DateTime? To)
        {
            long userid = User.Identity.GetUserId<long>();
            //db.Sp_Add_Activity(userid, "Generate Purchase Requsition Report ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            if (From == null || To == null)
            {
                From = new DateTime(DateTime.Now.Year, 1, 1);
                To = DateTime.Now;
            }
            var res = db.Sp_Get_InventoryItem_PR_PO_GRN_Report(Grn, From, To).ToList();
            return PartialView(res);
        }
        public ActionResult PurchaseOrderReport() // report 9
        {
            return View();
        }
        public ActionResult PurchaseOrderReport_Search(string PO, DateTime? From, DateTime? To)
        {
            long userid = User.Identity.GetUserId<long>();
            //db.Sp_Add_Activity(userid, "Generate Purchase Requsition Report ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            if (From == null || To == null)
            {
                From = new DateTime(DateTime.Now.Year, 1, 1);
                To = DateTime.Now;
            }

            var res = db.Sp_Get_PurchaseOrder_Report(PO, From, To).ToList();
            return PartialView(res);
        }
        public ActionResult SAFileSalesReport_Search(int blockId, string Sector)
        {
            if (Sector == "")
            {
                Sector = null;
            }
            var res = db.Sp_Get_FilePlot_SalesReport(blockId, "File", Sector).ToList();
            ViewBag.Block = blockId;
            return PartialView(res);
        }

        public ActionResult SABalanceSheetReport()
        {
            return View();
        }
        public ActionResult SABalanceSheetReport_Search(string Comp, DateTime To)
        {
            var res = db.SP_GET_BalanceSheet_AllCom(To, "/" + Comp + "/");//.ToList();
            return PartialView(res);
        }
        public ActionResult IncomeStatementReport()
        {
            return View();
        }
        public ActionResult SAIncomeStatementReport_Search(string Comp, DateTime? From, DateTime? To)
        {
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Get Balance Sheet List of  " + Comp, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);

            var res = db.Sp_Get_Income_Statment(comp.Id, From, To).ToList();
            return PartialView(res);
        }
        public ActionResult SARefundReport()
        {
            return View();
        }
        public ActionResult SARefundReport_Search(string Module, DateTime? From, DateTime? To, string size, string block)
        {
            if (size == "")
            {
                size = null;
            }
            if (block == "")
            {
                block = null;
            }
            var res = db.Refund_Report(Module, From, To, size, block).ToList();
            return PartialView(res);
        }

        public ActionResult SAPlotSalesReport()
        {
            FiletransferController sc = new FiletransferController();
            sc.miger();
            var blocks = db.RealEstate_Blocks.Select(x => new NameValuestring { Value = x.Id.ToString(), Name = x.Block_Name }).ToList();
            var Projects = db.RealEstate_Projects.Where(x => x.Type == "Building").Select(x => new NameValuestring { Value = "B-" + x.Id, Name = x.Project_Name }).ToList();
            //var all = new NameValuestring()
            //{
            //    Name = "All",
            //    Value = "0"
            //};
            //blocks.Add(all);
            blocks.AddRange(Projects);

            ViewBag.Blocks = new SelectList(blocks, "Name", "Name");
            return View();
        }
        public ActionResult SAPlotSalesReport_Search(DateTime? From, DateTime? To, string Block, string Type)
        {
            if (From == null || To == null)
            {
                To = DateTime.Now;
                From = To.Value.AddMonths(-36);
            }
            if (string.IsNullOrWhiteSpace(Type))
            {
                Type = null;
            }
            if (string.IsNullOrWhiteSpace(Block))
            {
                Block = null;
            }
            var res = db.Sp_Get_SalesReport(Block, Type, From, To).ToList();
            return PartialView(res);
        }

        public ActionResult GetSaleData(JqueryDatatableParam param, DateTime? From, DateTime? To, string Block, string Type)
        {
            if (From == null || To == null)
            {
                From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                To = DateTime.Now;
            }
            var res = db.Sp_Get_SalesReport(Block, Type, From, To).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                res = res.Where(x => x.Name.ToLower().Contains(param.sSearch.ToLower())
                                              || x.Unit_No.ToLower().Contains(param.sSearch.ToLower())
                                              || x.Block.ToLower().Contains(param.sSearch.ToLower())
                                              || x.Name.ToString().Contains(param.sSearch.ToLower())
                                              || x.Total_Amt.ToString().Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"];

            //if (sortColumnIndex == )
            //{
            //    res = sortDirection == "asc" ? res.OrderBy(c => c.Age) : res.OrderByDescending(c => c.Age);
            //}
            //else if (sortColumnIndex == 4)
            //{
            //    res = sortDirection == "asc" ? res.OrderBy(c => c.StartDate) : res.OrderByDescending(c => c.StartDate);
            //}
            //else if (sortColumnIndex == 5)
            //{
            //    res = sortDirection == "asc" ? res.OrderBy(c => c.Salary) : res.OrderByDescending(c => c.Salary);
            //}
            //else
            //{
            //    //Func<Employee, string> orderingFunction = e => sortColumnIndex == 0 ? e.Name :
            //    //                                               sortColumnIndex == 1 ? e.Position :
            //    //                                               e.Location;

            //    //res = sortDirection == "asc" ? res.OrderBy(orderingFunction) : employees.OrderByDescending(orderingFunction);
            //}

            var displayResult = res.Skip(param.iDisplayStart)
                .Take(param.iDisplayLength).ToList();
            var totalRecords = res.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}