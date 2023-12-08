using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Filters;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class JournalEntryController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        // GET: JournalEntry
        //public JsonResult AddJournalEntry(decimal Amount, string Code,   )
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    Helpers h = new Helpers();
        //    var TransactionId = h.RandomNumber();
        //    var Debit = db.Sp_Add_Journal_Entry(Amount, 0, res1.Code + " - " + res1.Head, res1.Id, res1.Code, res1.Head, "Total Plot Receiveable", TransactionId, 1, userid, userid, null).FirstOrDefault();
        //    return Json(true);
        //}
        private FinancialYear GetFinancialYear()
        {
            return new FinancialYear { Start = new DateTime(2022, 7, 1), End = new DateTime(2023, 6, 30) };
        }
        public JsonResult InstallmentJournalEntry()
        {
            return Json(true);
        }
        public ActionResult VouchersReports(string r)
        {
            var res = db.Journal_Entries.Select(x => new { Id = x.Recorded_By, Name = x.RecordedBy_Name }).Distinct().ToList();
            ViewBag.Users = new SelectList(res, "Id", "Name");
            ViewBag.VT = r;
            return View();
        }
        public ActionResult SearchVouchers(DateTime? From, DateTime? To,  long?[] Users, string VT)
        {
            ViewBag.From = From;
            ViewBag.To = To;
            ViewBag.VT = VT;
            string use = "";
            if (Users != null)
            {
                use = new XElement("Users", Users.Select(x => new XElement("U_Id",
                                       (x == null) ? null : new XAttribute("Ids", x)
                                ))).ToString();
            }
            var Cash = db.Sp_Get_JournalEntries_All(From, To, Voucher_Type.CPV.ToString(), use).ToList();
            var Bank = db.Sp_Get_JournalEntries_All(From, To, Voucher_Type.BPV.ToString() , use).ToList();

            ViewBag.Cash_P = db.Sp_Get_JournalEntries_All(From, To, Voucher_Type.CPV.ToString(), use).Where(x=> x.Credit >= 0).Sum(x=> x.Credit);
            ViewBag.Bank_P = db.Sp_Get_JournalEntries_All(From, To, Voucher_Type.BPV.ToString(), use).Where(x => x.Credit >= 0).Sum(x => x.Credit);
            ViewBag.Cash_R = db.Sp_Get_JournalEntries_All(From, To, Voucher_Type.CRV.ToString(), use).Where(x => x.Debit >= 0).Sum(x => x.Debit);
            ViewBag.Bank_R = db.Sp_Get_JournalEntries_All(From, To, Voucher_Type.BRV.ToString(), use).Where(x => x.Debit >= 0).Sum(x => x.Debit);
            ViewBag.Jv = db.Sp_Get_JournalEntries_All(From, To, Voucher_Type.JV.ToString(), use).Sum(x => x.Debit);

            return PartialView();
        }
        public ActionResult CashReceiptVouchers(DateTime? From, DateTime? To, long?[] Users, string Type)
        {
            string use = "";
            ViewBag.Type = Type;
            if (Users != null)
            {
                use = new XElement("Users", Users.Select(x => new XElement("U_Id",
                                       (x == null) ? null : new XAttribute("Ids", x)
                                ))).ToString();
            }
            var res = db.Sp_Get_JournalEntries_All(From, To, Type, use).ToList();
            ViewBag.heads = db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").Select(x=> x.Code).ToList();
            return PartialView(res);
        }
        public ActionResult BankReceiptVouchers(DateTime? From, DateTime? To, long?[] Users, string Type)
        {
            string use = "";
            ViewBag.Type = Type;
            if (Users != null)
            {
                use = new XElement("Users", Users.Select(x => new XElement("U_Id",
                                       (x == null) ? null : new XAttribute("Ids", x)
                                ))).ToString();
            }
            var res = db.Sp_Get_JournalEntries_All(From, To, Type, use).ToList();
            ViewBag.heads = db.Sp_Get_CA_Head_MultiRef("Assets", "Bank").Select(x => x.Code).ToList();
            return PartialView( res);
        }
        public ActionResult CashPaymentVouchers(DateTime? From, DateTime? To, long?[] Users, string Type)
        {
            string use = "";
            ViewBag.Type = Type;
            if (Users != null)
            {
                use = new XElement("Users", Users.Select(x => new XElement("U_Id",
                                      (x == null) ? null : new XAttribute("Ids", x)
                               ))).ToString();
            }
            var res = db.Sp_Get_JournalEntries_All(From, To, Type, use).ToList();
            ViewBag.heads = db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").Select(x => x.Code).ToList();
            return PartialView( res);
        }
        public ActionResult BankPaymentVouchers(DateTime? From, DateTime? To, long?[] Users, string Type)
        {
            string use = "";
            ViewBag.Type = Type;
            if (Users != null)
            {
                use = new XElement("Users", Users.Select(x => new XElement("U_Id",
                                     (x == null) ? null : new XAttribute("Ids", x)
                              ))).ToString();
            }
            var res = db.Sp_Get_JournalEntries_All(From, To, Type, use).ToList();
            ViewBag.heads = db.Sp_Get_CA_Head_MultiRef("Assets", "Bank").Select(x => x.Code).ToList();
            return PartialView( res);
        }
        public ActionResult JournalVouchers(DateTime? From, DateTime? To, long?[] Users, string Type)
        {
            string use = "";
            ViewBag.Type = Type;
            if (Users != null)
            {
                use = new XElement("Users", Users.Select(x => new XElement("U_Id",
                                      (x == null) ? null : new XAttribute("Ids", x)
                               ))).ToString();
            }

            var res = db.Sp_Get_JournalEntries_All(From, To, Type, use).ToList();
            return PartialView( res);
        }
        public ActionResult CashBook()
        {
            var All = db.Sp_Get_Cashiers_List().ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");
            var fy = GetFinancialYear();
            ViewBag.FiscalStart = fy.Start;
            ViewBag.FiscalEnd = fy.End;
            return View();
        }
        [HttpPost]
        public ActionResult SearchCashBook(DateTime? From, DateTime? To, long?[] Users)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string chids = null;
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
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var cashAccount = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Cash_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").FirstOrDefault().Code;
            var res = db.Sp_Get_JournalLedger_New(From, To, cashAccount.Text_ChartCode, chids).ToList();
            return PartialView(res);
        }


        public ActionResult BankBook()
        {
            var All = db.Users.ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");
            var banks = db.Bank_Accounts.Select(x => new NameValuestring { Value = x.Id.ToString(), Name = x.Bank + " - " + x.Account_Number }).ToList();

            ViewBag.Bank = new SelectList(banks.OrderBy(x => x.Value), "Value", "Name", 1);
         
            return View();
        }
        [HttpPost]
        public ActionResult SearchBankBook(DateTime? From, DateTime? To, long?[] Users,int Bank_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            if (From == null || To == null)
            {
                var fy = GetFinancialYear();
                From = fy.Start;
                To = fy.End;
            }
            string chids = null;
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
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var Payment = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Bank_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, Bank_Id);

            var res = db.Sp_Get_JournalLedger_New(From, To, Payment.Text_ChartCode, chids).ToList();
            return PartialView(res);
        }


        public ActionResult LandPayments()
        {
            var banks = db.Bank_Accounts.Select(x => new NameValuestring { Value = x.Id.ToString(), Name = x.Bank + " - " + x.Account_Number }).ToList();
            var all = new NameValuestring()
            {
                Name = "Cash",
                Value = "0"
            };
            banks.Add(all);
            ViewBag.BanksList = new SelectList(banks.OrderBy(x => x.Value), "Value", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return View();
        }

        //public JsonResult LandPayment(int Head_Id, int payAccId, decimal? Amount, long TransactionId, string PaymentType, string Bank, string Inst_No, DateTime? Inst_Date, string Branch, string Description, string Img, decimal? TaxAmount, int? TaxAccount)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    AccountHandlerController ah = new AccountHandlerController();
        //    var comp = ah.Company_Attr(userid);
        //    var Dealer_COA = new Sp_Get_COA_Head_Code_Result();
        //    using (var Transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var voch = db.Sp_Add_Voucher(null, Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), Bank, Branch, Inst_Date, Inst_No, string.Join(",", res1.Select(x => x.Mobile_1)),
        //                "", string.Join(",", res1.Select(x => x.Name)), Id, Modules.Dealers.ToString(), res2.Dealership_Name, PaymentType, null, null, TransactionId, "Dealer Advance", userid, null, comp.Id).SingleOrDefault();

        //            var a = db.Sp_Add_VoucherDetails(Amount, Description, null, null, null, voch.Receipt_Id).FirstOrDefault();
        //            {
        //                try
        //                {
        //                    var Payment = ah.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
        //                    {
        //                        Dealer_COA = ah.Dealer_Commission_Head(Id, comp.Id);
        //                    }

        //                    var Receivable_Credit = db.Sp_Add_Journal_Entry(Amount + TaxAmount, 0, Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, 1, userid, userid, null, Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, voch.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
        //                    var Reciv_Debit = db.Sp_Add_Journal_Entry(0, Amount, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, 1, userid, userid, null, Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, voch.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();

        //                    if (TaxAmount > 0)
        //                    {
        //                        var taxAccount = ah.GetHead(Convert.ToInt32(TaxAccount));
        //                        if (taxAccount is null)
        //                        {
        //                            Transaction.Rollback();
        //                            return Json(new { Status = false, Msg = "Tax Account not mapped. Please map tax account for further use." });
        //                        }
        //                        var taxCredit = db.Sp_Add_Journal_Entry(0, TaxAmount, taxAccount.Head, taxAccount.Id, taxAccount.Text_ChartCode, taxAccount.Head, Description, TransactionId, 1, userid, userid, null, null, Inst_No, "", Inst_Date, null, "", null, voch.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit

        //                    }


        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                }
        //            }
        //            Transaction.Commit();
        //            return Json(new { Status = true, VoucherNo = voch.Receipt_Id, Token = TransactionId });
        //        }
        //        catch (Exception e)
        //        {
        //            Transaction.Rollback();
        //            return Json(new Return { Status = false, Msg = "Error Occured" });
        //        }
        //    }
        //}
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