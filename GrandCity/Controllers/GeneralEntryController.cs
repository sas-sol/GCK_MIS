//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Xml.Linq;
//using MeherEstateDevelopers.Models;
//using MeherEstateDevelopers.Filters;
//using Microsoft.AspNet.Identity;
//using Newtonsoft.Json;
//namespace MeherEstateDevelopers.Controllers
//{
//    [LogAction]
//    [Authorize]
//    public class GeneralEntryController : Controller
//    {
//        // GET: GeneralEntry
//        private Grand_CityEntities db = new Grand_CityEntities();
//        public ActionResult GeneralEntry()
//        {
//            //var res = db.Sp_Get_CA_LeaveNodes().Select(x => new { Value = x.Id, Text = x.Text_ChartCode + " - " + x.Head }).ToList();
//            //ViewBag.Type = new SelectList(res, "Value", "Text");
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Gernal Entry Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View();
//        }
//        public JsonResult RecordEntries(List<Voucher_Details_General_Entries> Details, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (User.IsInRole("Head Cashier"))
//            {
//                string JournalEnt = new XElement("JournalEntries", Details.Select(x => new XElement("Entries",
//                new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
//                new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
//                new XAttribute("Head_Name", this.GetHead(x.Account_Head_Id).Head),
//                new XAttribute("Head_Code", this.GetHead(x.Account_Head_Id).Text_ChartCode),
//                new XAttribute("Head_Id", x.Account_Head_Id),
//                new XAttribute("Line", x.Line),
//                new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//                new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
//                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
//                new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
//                new XAttribute("GroupId", TransactionId)
//                ))).ToString();

//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var res = db.Sp_Add_Journal_Entries(JournalEnt, userid).FirstOrDefault();
//                        db.Sp_Add_Activity(userid, "Added New Voucher Details in Journal Entry ", "Create", "Activity_Record", ActivityType.General_Entry.ToString(), TransactionId);
//                        Transaction.Commit();
//                        return Json(true);
//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        return Json(false);
//                    }
//                }
//            }
//            else
//            {
//                string GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
//                new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
//                new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
//                new XAttribute("Head_Name", this.GetHead(x.Account_Head_Id).Head),
//                new XAttribute("Head_Code", this.GetHead(x.Account_Head_Id).Text_ChartCode),
//                new XAttribute("Head_Id", x.Account_Head_Id),
//                new XAttribute("Line", x.Line),
//                new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//                new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
//                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
//                new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
//                new XAttribute("GroupId", TransactionId)
//                ))).ToString();

//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var res = db.Sp_Add_General_Entries(GeneralEnt, userid).FirstOrDefault();

//                        db.Sp_Add_Activity(userid, "Added New Voucher Details in Gernal Entry ", "Create", "Activity_Record", ActivityType.General_Entry.ToString(), TransactionId);
//                        Transaction.Commit();
//                        return Json(true);
//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        return Json(false);
//                    }
//                }
//            }
//        }
//        public JsonResult RecordBills(List<Bill_Details> Details, DateTime DueDate, DateTime BillDate, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            string Bill = new XElement("Bills", Details.Select(x => new XElement("Entries",
//            new XAttribute("Vendor_Id", x.VendorId),
//            new XAttribute("Terms", x.terms),
//            new XAttribute("Bill_No", x.BillNo),
//            new XAttribute("Head_Code", GetHead(x.Head_id).Text_ChartCode),
//            new XAttribute("Head_Name", GetHead(x.Head_id).Head),
//            new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//            new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//            new XAttribute("Line", x.Line),
//            new XAttribute("description", x.description),
//            new XAttribute("Head_Id", x.Head_id),
//            new XAttribute("GroupId", TransactionId),
//            //new XAttribute("PO_Group_Id", PO_group_Id),
//            new XAttribute("Comp_Id", comp.Id),
//            new XAttribute("Head", (x.Head == null) ? "" : x.Head)
//            ))).ToString();

//            try
//            {
//                var res = db.Sp_Add_Bills(Bill, userid, BillDate, DueDate).FirstOrDefault();

//                db.Sp_Add_Activity(userid, "Added New Bill For " + Details.Select(x => x.VendorId).FirstOrDefault(), "Create", "Activity_Record", ActivityType.General_Entry.ToString(), TransactionId);

//                decimal? amount = 0;
//                if (Details.Any())
//                {
//                    foreach (var item in Details)
//                    {
//                        decimal? rate = (item.Rate == null) ? 0 : item.Rate;
//                        decimal? qty = (item.Quantity == null) ? 0 : item.Quantity;
//                        amount += rate * qty;
//                    }
//                }
//                var vendor_coa = GetHead(Details.FirstOrDefault().VendorId);


//                var GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
//                new XAttribute("Naration", x.description),
//                new XAttribute("Head", (x.Head == null) ? "" : x.Head),
//                new XAttribute("Head_Name", GetHead(x.Head_id).Head),
//                new XAttribute("Head_Code", GetHead(x.Head_id).Text_ChartCode),
//                new XAttribute("Head_Id", x.Head_id),
//                new XAttribute("Line", x.Line),
//                new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//                new XAttribute("UOM", ""),
//                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                new XAttribute("Debit", ((x.Rate == null) ? 0 : x.Rate) * ((x.Quantity == null) ? 0 : x.Quantity)),
//                new XAttribute("Credit", 0),
//                new XAttribute("Comp_Id", comp.Id),
//                new XAttribute("GroupId", TransactionId)
//                )));
//                var GEapp = new XElement("GeneralEntries", new XElement("Entries",
//                new XAttribute("Naration", string.Join(" , ", Details.Select(x => x.description)) + " for Vendor " + vendor_coa.Head),
//                new XAttribute("Head", vendor_coa.Head),
//                new XAttribute("Head_Name", vendor_coa.Head),
//                new XAttribute("Head_Code", vendor_coa.Text_ChartCode),
//                new XAttribute("Head_Id", vendor_coa.Id),
//                new XAttribute("Line", Details.Count() + 1),
//                new XAttribute("Qty", 0),
//                new XAttribute("UOM", ""),
//                new XAttribute("Rate", 0),
//                new XAttribute("Debit", 0),
//                new XAttribute("Credit", amount),
//                new XAttribute("Comp_Id", comp.Id),
//                new XAttribute("GroupId", TransactionId)
//                ));
//                GeneralEnt.Add(
//                    from ge in GEapp.Elements()
//                    select ge
//                    );
//                var res1 = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
//                return Json(res);
//            }
//            catch (Exception ex)
//            {
//                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(false);
//            }



//            //using (var Transaction = db.Database.BeginTransaction())
//            //{
//            //    try
//            //    {
//            //        var res = db.Sp_Add_Bills(Bill, userid, BillDate, DueDate);
//            //        decimal? amount = 0;
//            //        if (Details.Any())
//            //        {
//            //            foreach (var item in Details)
//            //            {
//            //                decimal? rate = (item.Rate == null) ? 0 : item.Rate;
//            //                decimal? qty = (item.Quantity == null) ? 0 : item.Quantity;
//            //                amount += rate * qty;
//            //            }
//            //        }
//            //        var account = GetHead(Details.FirstOrDefault().VendorId);
//            //        AccountHandlerController de = new AccountHandlerController();
//            //        var vendor_coa = de.Vendor_Head(account.Head, account.Id);

//            //        var GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
//            //        new XAttribute("Naration", x.description),
//            //        new XAttribute("Head", (x.Head == null) ? "" : x.Head),
//            //        new XAttribute("Head_Name", GetHead(x.Head_id).Head),
//            //        new XAttribute("Head_Code", GetHead(x.Head_id).Text_ChartCode),
//            //        new XAttribute("Head_Id", x.Head_id),
//            //        new XAttribute("Line", x.Line),
//            //        new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//            //        new XAttribute("UOM", ""),
//            //        new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//            //        new XAttribute("Debit", ((x.Rate == null) ? 0 : x.Rate) * ((x.Quantity == null) ? 0 : x.Quantity)),
//            //        new XAttribute("Credit", 0),
//            //        new XAttribute("GroupId", TransactionId)
//            //        )));
//            //        var GEapp = new XElement("GeneralEntries", new XElement("Entries",
//            //        new XAttribute("Naration", "Payable Record Against Bill No. " + Details.FirstOrDefault().BillNo + " for Vendor " + vendor_coa.Head),
//            //        new XAttribute("Head", vendor_coa.Code + " - " + vendor_coa.Head),
//            //        new XAttribute("Head_Name", vendor_coa.Head),
//            //        new XAttribute("Head_Code", vendor_coa.Code),
//            //        new XAttribute("Head_Id", vendor_coa.Id),
//            //        new XAttribute("Line", Details.Count() + 1),
//            //        new XAttribute("Qty", 0),
//            //        new XAttribute("UOM", ""),
//            //        new XAttribute("Rate", 0),
//            //        new XAttribute("Debit", 0),
//            //        new XAttribute("Credit", amount),
//            //        new XAttribute("GroupId", TransactionId)
//            //        ));
//            //        GeneralEnt.Add(
//            //            from ge in GEapp.Elements()
//            //            select ge
//            //            );
//            //        var res1 = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
//            //        Transaction.Commit();
//            //        return Json(res);
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        Transaction.Rollback();
//            //        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//            //        return Json(false);
//            //    }
//            //}
//        }
//        public Sp_Get_CA_Head_Parameter_Result GetHead(int Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
//            return res;
//        }
//        /// ////////////
//        public ActionResult PendingEntries()
//        {
//            var res = db.Sp_Get_PendingPayments().ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Pending General Entries Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View(res);
//        }
//        public ActionResult EntryDetails(long Id)
//        {
//            var res2 = db.Sp_Get_GeneralEntries_Parameter(Id).ToList();
//            var res3 = db.Invoices_Files.Where(x => x.Invoice == Id).ToList();
//            var res = new EntryInvoice { General_Entries = res2, Files = res3 };
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Details of Entry ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
//            return View(res);
//        }
//        [HttpPost]
//        public JsonResult UpdateSuperVoucher(long Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var res2 = db.General_Entries.Where(x => x.GroupId == Id && x.Credit > 0).FirstOrDefault();
//                    var res1 = db.Sp_Update_Journal_Entry_Sup_VoucherNo(Id, userid).FirstOrDefault();

//                    db.Sp_Add_Activity(userid, "Supervised General Entry " + Id, "Update", "Activity_Record", ActivityType.General_Entry.ToString(), Id);


//                    db.Sp_Add_Payable(Id, res2.Head_Id, res2.Credit, null, null);
//                    Transaction.Commit();
//                    return Json(true);
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(false);
//                }
//            }
//        }
//        public JsonResult UpdateSuperVoucherReceivable(long Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var res1 = db.Sp_Update_Journal_Entry_Sup_VoucherNo(Id, userid).FirstOrDefault();
//                    db.Sp_Add_Activity(userid, "Supervised Receivable Type General Entry " + Id, "Update", "Activity_Record", ActivityType.General_Entry.ToString(), Id);
//                    var res2 = db.Sp_Get_JournalEntries_Parameter_All(Id).Where(x => x.Debit > 0).ToList().GroupBy(x => x.Head_Id);
//                    foreach (var item in res2)
//                    {
//                        var res3 = db.Sp_Add_Receivable(Id, item.Key, item.Sum(x => x.Debit));
//                    }
//                    Transaction.Commit();
//                    return Json(true);
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

//                    return Json(false);
//                }
//            }
//        }
//        [HttpPost]
//        public JsonResult SuperviseBill(long Id, long? PO_Group_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var res2 = db.General_Entries.Where(x => x.GroupId == Id && x.Credit > 0).FirstOrDefault();
//                    var res1 = db.Sp_Update_Journal_Entry_Sup_VoucherNo(Id, userid).FirstOrDefault();

//                    db.Sp_Add_Activity(userid, "Supervised Bill " + Id, "Update", "Activity_Record", ActivityType.General_Entry.ToString(), userid);


//                    db.Sp_Add_Payable(Id, res2.Head_Id, res2.Credit, PO_Group_Id, null);
//                    Transaction.Commit();
//                    return Json(true);
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

//                    return Json(false);
//                }
//            }
//        }
//        public ActionResult RecordedEntries(long? Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Sp_Get_GeneralEntry_Userid(userid).ToList();
//            return View(res);
//        }
//        public ActionResult ForUpdations()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Sp_Get_GeneralEntry_Updations(userid).ToList();
//            return View(res);
//        }
//        public ActionResult UpdateGeneralEntry(long Id)
//        {
//            var res = db.Sp_Get_GeneralEntries_Parameter(Id).ToList();
//            return View(res);
//        }
//        public JsonResult UpdateRecordedEntries(List<Voucher_Details_General_Entries> Details, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var old = db.General_Entries.Where(x => x.GroupId == TransactionId).FirstOrDefault();
//                    var res1 = db.Sp_Delete_GeneralEntry(TransactionId);

//                    string GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
//                    new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
//                    new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
//                    new XAttribute("Head_Name", this.GetHead(x.Account_Head_Id).Head),
//                    new XAttribute("Head_Code", this.GetHead(x.Account_Head_Id).Text_ChartCode),
//                    new XAttribute("Head_Id", x.Account_Head_Id),
//                    new XAttribute("Line", x.Line),
//                    new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//                    new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
//                    new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                    new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
//                    new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
//                    new XAttribute("Comp_Id", old.Comp_Id),
//                    new XAttribute("GroupId", TransactionId)
//                    ))).ToString();
//                    var res = db.Sp_Add_General_Entries(GeneralEnt, userid).FirstOrDefault();

//                    db.Sp_Add_Activity(userid, "Updated Gernal Entry ", "Update", "Activity_Record", ActivityType.Page_Access.ToString(), TransactionId);

//                    Transaction.Commit();
//                    return Json(true);
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(false);
//                }
//            }
//        }
//        public ActionResult GE_History(long GroupId)
//        {
//            var res = db.Sp_Get_JournalEntries_Parameter_All(GroupId).ToList();
//            return PartialView(res);
//        }
//        [HttpPost]
//        public JsonResult RevertEntry(long Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res1 = db.Sp_Update_GeneralEntries_Revert(Id, null);
//            return Json(true);
//        }
//        public ActionResult VoucherPayableDetails(long Id, int HeadId)
//        {
//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            decimal? PO_Val = 0;
//            var res2 = db.Sp_Get_JournalEntriesPayable_Parameter(Id, HeadId).ToList();
//            var res3 = db.Sp_Get_PayableVouchers_Head(Id, HeadId).SingleOrDefault();

//            var res4 = db.Inventory_PurchaseOrder.Where(x => x.Group_Id == res3.PO_Group_Id).ToList();
//            if (res4.Any())
//            {
//                var res5 = db.Payables.Where(x => x.PO_Group_Id == res3.PO_Group_Id).ToList();

//                var advance = res5.Where(x => x.Advance == true).ToList();
//                var Total_Adv = advance.Sum(x=> x.Amount);
//                var Total_Adv_Paid = advance.Sum(_ => _.Paid_Amount);


//                var Other = res5.Where(x => x.Advance == null).ToList();
//                var Total_oth = Other.Sum(x => x.Amount);
//                var Total_oth_Paid = Other.Sum(_ => _.Paid_Amount);



//                var Total_Paid_Amount = Total_Adv_Paid + Total_oth_Paid;
//                var Remaining = Total_oth - Total_Paid_Amount;

//                ViewBag.Paid_Amount = Total_Paid_Amount;
//                ViewBag.Remaining = Remaining;

//            }
//            else
//            {
//                ViewBag.Paid_Amount = res3.Paid_Amount;
//                ViewBag.Remaining = res3.Remaining;
//            }

//            if (res4.Any())
//            {
//                PO_Val = res4.Sum(x => x.TotalVal);
//            }

//            var banks = db.Bank_Accounts.Select(x => new NameValuestring { Value = x.Id.ToString(), Name = x.Bank + " - " + x.Account_Number }).ToList();
//            var all = new NameValuestring()
//            {
//                Name = "Cash",
//                Value = "0"
//            };
//            banks.Add(all);
//            ViewBag.BanksList = new SelectList(banks.OrderBy(x => x.Value), "Value", "Name");

//            ViewBag.Head = HeadId;
//            ViewBag.HeadName = res2.FirstOrDefault().Head; //Make separate method for this if res2 stored procedure change due to style.

//            ViewBag.PO_Total_Val = PO_Val;
//            ViewBag.Amount = res3.Amount;

//            var res = db.Sp_Get_Liability_Head_Balance(HeadId).FirstOrDefault();
//            if (res == null)
//            {
//                ViewBag.TotalRemaining = 0;
//            }
//            else
//            {
//                ViewBag.TotalRemaining = res.Credit - res.Debit;
//            }
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            return PartialView(res2);
//        }
//        public ActionResult VoucherPayableDetailsByVendor(long Id, int HeadId)
//        {
//            var res1 = db.Payables.Where(x => x.Remaining > 0);

//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            var res2 = db.Sp_Get_JournalEntriesPayable_Parameter(Id, HeadId).ToList();
//            var res3 = db.Sp_Get_PayableVouchers_Head(Id, HeadId).SingleOrDefault();
//            ViewBag.Head = HeadId;
//            ViewBag.Amount = res3.Amount;
//            ViewBag.Paid_Amount = res3.Paid_Amount;
//            ViewBag.Remaining = res3.Remaining;

//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            return PartialView(res2);
//        }

//        public JsonResult AddNewPayable(List<VoucherPayableItems> vpi, int payAccId, string receivedBy, string instNo, DateTime? instDate, string branch, string payType, decimal Amount, long TransactionId, decimal? TaxAmount, int? TaxAccount, string Recorded_By, string Supervised_By)
//        {
//            AccountHandlerController ah = new AccountHandlerController();
//            long userid = long.Parse(User.Identity.GetUserId());
//            var comp = ah.Company_Attr(userid);
//            var username = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();

//            var bank = db.Bank_Accounts.Where(x => x.Id == payAccId).FirstOrDefault();
//            var headId = vpi.FirstOrDefault().HeadId;
//            var debitAcc = GetHead(headId);
//            var grpId = vpi.FirstOrDefault().GroupId;
//            var narration = "Paid for - " + vpi.FirstOrDefault().Narration;
//            string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Amount + TaxAmount));


//            if (payAccId == 0)
//            {
//                payType = "Cash";
//                PaymentTypeModel credAccount = ah.Payment_Head(payType, Transaction_Type.Credit.ToString(), null, comp.Id);



//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var r = db.Sp_Add_Voucher_Vendor("-", Amount + TaxAmount, AmountinWords, null, null, instDate, instNo, "", narration, "", null, Modules.Vendor_Payment.ToString(), receivedBy,
//                            payType, "Meher Estate Developers", "", grpId, "Payment Voucher", userid, debitAcc.Type_Id, grpId, Recorded_By, userid, DateTime.Now, Supervised_By, null, null, TransactionId).FirstOrDefault();
//                        db.Sp_Update_Payables_At_Receiving(grpId, headId, Amount + TaxAmount);

//                        try
//                        {
//                            var Debit = db.Sp_Add_Journal_Entry(Amount + TaxAmount, 0, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, narration, TransactionId, 1, userid, userid, null, null, instNo, null, instDate, null, "", null, r.Receipt_No.ToString(), credAccount.VoucherType, comp.Id).FirstOrDefault();
//                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, credAccount.PaymentData.Head, credAccount.PaymentData.Id, credAccount.PaymentData.Text_ChartCode, credAccount.PaymentData.Head, narration, TransactionId, 1, userid, userid, null, null, instNo, "", instDate, null, "", null, r.Receipt_No.ToString(), credAccount.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit


//                            if (TaxAmount > 0)
//                            {
//                                var taxAccount = GetHead(Convert.ToInt32(TaxAccount));
//                                if (taxAccount is null)
//                                {
//                                    Transaction.Rollback();
//                                    return Json(new { Status = false, Msg = "Tax Account not mapped. Please map tax account for further use." });
//                                }
//                                var taxCredit = db.Sp_Add_Journal_Entry(0, TaxAmount, taxAccount.Head, taxAccount.Id, taxAccount.Text_ChartCode, taxAccount.Head, narration, TransactionId, 1, userid, userid, null, null, instNo, "", instDate, null, "", null, r.Receipt_No.ToString(), credAccount.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit

//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Transaction.Rollback();
//                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        }
//                        Transaction.Commit();
//                        return Json(new { Status = true, Msg = "Payment Voucher Successful.", VoucherId = r.Receipt_Id, Token = grpId });
//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        return Json(new { Status = false, Msg = "Error Occured" });
//                    }
//                }



//            }
//            else
//            {
//                var credAccount = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.PDC_Payable.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);


//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var r = db.Sp_Add_Voucher_Vendor("-", Amount, AmountinWords, bank.Bank, "", instDate, instNo, "", narration, "", null, Modules.Vendor_Payment.ToString(), receivedBy,
//                            payType, "Meher Estate Developers", "", grpId, "Payment Voucher", userid, debitAcc.Type_Id, grpId, username, userid, DateTime.Now, "", null, null, TransactionId).FirstOrDefault();
//                        db.Sp_Update_Payables_At_Receiving(grpId, headId, Amount);
//                        try
//                        {
//                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, narration, TransactionId, 1, userid, userid, null, null, instNo, null, instDate, null, "", null, r.Receipt_No.ToString(), "BPV", comp.Id).FirstOrDefault();
//                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, credAccount.Head, credAccount.Id, credAccount.Text_ChartCode, credAccount.Head, narration, TransactionId, 1, userid, userid, null, null, instNo, "", instDate, null, "", null, r.Receipt_No.ToString(), "BPV", comp.Id).FirstOrDefault();   /// For Cash and Bank Credit

//                            var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(Amount, bank.Bank, null, payType, bank.Bank, bank.Account_Number, PaymentMethodStatuses.Pending.ToString(),
//                                    Modules.Vendor_Payment.ToString(), "Payment Voucher", userid, instNo, debitAcc.Type_Id, instDate, debitAcc.Head, r.Receipt_Id, comp.Id, Voucher_Type.BPV.ToString()).FirstOrDefault());

//                        }
//                        catch (Exception ex)
//                        {
//                            Transaction.Rollback();
//                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        }
//                        Transaction.Commit();
//                        return Json(new { Status = true, Msg = "Payment Voucher Successful.", VoucherId = r.Receipt_Id, Token = grpId });
//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        return Json(new { Status = false, Msg = "Error Occured" });
//                    }
//                }

//            }


//        }


//        [HttpPost]
//        public JsonResult GenerateVoucher(long Id, long TransactionId, decimal Amount, string Bank, string Branch, string Cbp_No, DateTime? Cbp_date, string Description, string PaymentType, long Head_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res1 = db.Sp_Get_PayableVouchers_Head(Id, Head_Id).SingleOrDefault();
//            var res2 = db.Sp_Get_JournalEntries_Payable_Parameter(Id, Head_Id).ToList();
//            var res3 = db.Sp_Get_CA_Head_Parameter(res2.Select(x => x.Head_Id).FirstOrDefault()).FirstOrDefault();
//            string Name = "", Company = "", Mobile = "";
//            if (res3.Type != null)
//            {
//                var res4 = db.Sp_Get_VendorDetail(res3.Type_Id).FirstOrDefault();
//                if (res4 != null)
//                {
//                    Name = res4.Name; Company = res4.Company; Mobile = res4.Office_Mobile;
//                }
//            }
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {

//                    var c = db.Sp_Update_GeneralEntries_Paid(res1.Id, Amount);
//                    var res6 = db.Sp_Get_PayableVouchers_Head(Id, Head_Id).SingleOrDefault();
//                    string des = (res6.Remaining > 0) ? "Total Paid Amount: <b>" + string.Format("{0:0,0.##}", res6.Paid_Amount) + "</b>: Remaining Amount: <b>" + string.Format("{0:0,0.##}", res6.Remaining) + "</b>" : "";

//                    //Add Payment Voucher
//                    var res30 = db.Sp_Add_Voucher_Vendor(Name, Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), Bank, Branch, Cbp_date, Cbp_No, Mobile, des,
//                     Company, res3.Id, Modules.ProjectManagement.ToString(), res3.Head, PaymentType, "Meher Estate Developers",
//                      Description, userid, Payments.Vendor_Payment.ToString(), userid, res3.Id, TransactionId, res2.Select(x => x.RecordedBy_Name).FirstOrDefault(), res2.Select(x => x.Recorded_By).FirstOrDefault(),
//                      res2.Select(x => x.RecordedBy_Date).FirstOrDefault(), res2.Select(x => x.Supviseby_Name).FirstOrDefault(), res2.Select(x => x.Supvise_by).FirstOrDefault(), res2.Select(x => x.Supviseby_Date).FirstOrDefault(), TransactionId).FirstOrDefault();

//                    // Add Voucher Details
//                    foreach (var item in res2)
//                    {
//                        var a = db.Sp_Add_VoucherDetails(item.Credit, item.Naration, item.Qty, item.Rate, item.UOM, res30.Receipt_Id).FirstOrDefault();
//                    }
//                    {
//                        //bool headcashier = false;
//                        //if (User.IsInRole("Head Cashier"))
//                        //{
//                        //    headcashier = true;
//                        //}
//                        //try
//                        //{
//                        //    AccountHandlerController de = new AccountHandlerController();
//                        //    de.VendorPayment(Amount, res3.Head, null, PaymentType, Cbp_No, Cbp_date, Bank, string.Join(":", res2.Select(x => x.Naration)) + " - " + Description, TransactionId, userid, res30.Receipt_No, 1, headcashier);
//                        //}
//                        //catch (Exception ex)
//                        //{
//                        //    Transaction.Rollback();
//                        //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        //}                        
//                    }

//                    var res = new { VoucherNo = res30.Receipt_Id, Token = userid };
//                    Transaction.Commit();
//                    return Json(res);
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

//                    return Json(false);
//                }
//            }
//        }

//        public ActionResult DirectTempVoucher()
//        {
//            var id = db.Sp_Get_CA_Head_MultiRef("Liability", "Temp").FirstOrDefault();
//            ViewBag.Bank = db.Bank_Accounts.Select(x => new { x.Bank, x.Account_Number }).ToList();

//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            if (id is null)
//            {
//                ViewBag.Head = "Transaction Cannot be recorded due to Unavailablity of Temporary account";
//                return View();
//            }
//            else
//            {
//                ViewBag.Id = id.Id;
//                ViewBag.Head = id.Code + " - " + id.Head;
//                return View();
//            }
//        }
//        [HttpPost]
//        public JsonResult RecordTempEntry(decimal Amount, string Bank, string Branch, string Inst_No, DateTime? Inst_Date, string Description, string PaymentType, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var curuse = db.Users.Find(userid);
//            var id = db.Sp_Get_CA_Head_MultiRef("Liability", "Temp").FirstOrDefault();

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var res30 = db.Sp_Add_Voucher_Vendor(id.Code, Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), null, null, null, null, null, "",
//                     id.Code + " " + id.Head, id.Id, Modules.ProjectManagement.ToString(), id.Head, "Cash", "Meher Estate Developers",
//                      "", userid, Payments.Temporary.ToString(), userid, id.Id, TransactionId, curuse.Name, userid, DateTime.Now, curuse.Name, userid, DateTime.Now, TransactionId).FirstOrDefault();
//                    var a = db.Sp_Add_VoucherDetails(Amount, Description, null, null, null, res30.Receipt_Id).FirstOrDefault();

//                    {
//                        //bool headcashier = false;
//                        //if (User.IsInRole("Head Cashier"))
//                        //{
//                        //    headcashier = true;
//                        //}
//                        //try
//                        //{
//                        //    AccountHandlerController ah = new AccountHandlerController();
//                        //    ah.TempAccountHandler(Amount, PaymentType, Inst_No, Inst_Date, Bank, TransactionId, Description, userid, res30.Receipt_No, 1, headcashier);
//                        //}
//                        //catch (Exception ex)
//                        //{
//                        //    Transaction.Rollback();
//                        //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        //}                        
//                    }

//                    var res = new { Status = true, Msg = "Payment Voucher has been Generated", VoucherNo = res30.Receipt_Id, Token = userid };
//                    Transaction.Commit();
//                    return Json(res);
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString(), "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    var res = new Return { Status = false, Msg = "Error Occured" };
//                }
//            }
//            return Json(true);
//        }
//        public ActionResult TempAccount()
//        {
//            var a = db.Sp_Get_CA_Head_MultiRef("Liability", "Temp").FirstOrDefault();
//            var res = db.Sp_Get_TempAccount(a.Code).ToList();

//            return View(res);
//        }
//        public ActionResult TempAccountJV(long Id)
//        {
//            var res = db.Sp_Get_JournalEntries_Parameter_All(Id).Where(x => x.Debit > 0).FirstOrDefault();
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            return View(res);
//        }
//        public JsonResult RecordJVEntries(List<Voucher_Details_General_Entries> Details, long TransactionId, long Ref)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            string JournalEnt = new XElement("JournalEntries", Details.Select(x => new XElement("Entries",
//            new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
//            new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
//            new XAttribute("Head_Name", this.GetHead(x.Account_Head_Id).Head),
//            new XAttribute("Head_Code", this.GetHead(x.Account_Head_Id).Text_ChartCode),
//            new XAttribute("Head_Id", x.Account_Head_Id),
//            new XAttribute("Line", x.Line),
//            new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//            new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
//            new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//            new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
//            new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
//            new XAttribute("Ref", Ref),
//            new XAttribute("Comp_Id", comp.Id),
//            new XAttribute("GroupId", TransactionId)
//            ))).ToString();
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var res1 = db.Sp_Add_Journal_Entries(JournalEnt, userid).FirstOrDefault();
//                    var res2 = db.Sp_Update_JVStatus(Ref);
//                    Transaction.Commit();
//                    return Json(true);

//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(false);
//                }
//            }

//        }

//        //public JsonResult ReverseEntry(long Id)
//        //{
//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    var res = db.Sp_Get_GeneralEntries_Parameter(Id).ToList();
//        //    using (var Transaction = db.Database.BeginTransaction())
//        //    {
//        //        try
//        //        {
//        //            foreach (var item in res.Where(x => x.Debit > 0))
//        //            {
//        //                var a = db.Sp_Update_GeneralEntry_Reversal(item.Id);
//        //                var res2 = db.Sp_Add_General_Entry(item.Credit, item.Debit, null, null, null, null, item.Head, item.Head_Id, item.Head_Code, item.Head_Name, item.Naration, item.GroupId, item.Line, item.Qty, item.UOM, item.Rate, userid, null);
//        //                var b = db.Sp_Update_GeneralEntry_Reversal(res2.FirstOrDefault());
//        //            }
//        //            foreach (var item in res.Where(x => x.Credit > 0))
//        //            {
//        //                var a = db.Sp_Update_GeneralEntry_Reversal(item.Id);
//        //                var res2 = db.Sp_Add_General_Entry(item.Credit, item.Debit, null, null, null, null, item.Head, item.Head_Id, item.Head_Code, item.Head_Name, item.Naration, item.GroupId, item.Line, item.Qty, item.UOM, item.Rate, userid, null);
//        //                var b = db.Sp_Update_GeneralEntry_Reversal(res2.FirstOrDefault());
//        //            }
//        //            Transaction.Commit();
//        //            return Json(true);
//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            Transaction.Rollback();
//        //            EmailService e = new EmailService();
//        //            e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Reverse Entry");
//        //            return Json(false);
//        //        }
//        //    }

//        //}
//        public void JournalVoucher()
//        {

//        }
//        public ActionResult PayableVouchers()
//        {
//            var res = db.Sp_Get_PayableVouchers().ToList();
//            return View(res);
//        }
//        public ActionResult CashBook()
//        {
//            var All = db.Sp_Get_Cashiers_List().ToList();
//            ViewBag.Users = new SelectList(All, "id", "Name");
//            return View();
//        }
//        [HttpPost]
//        public ActionResult SearchCashBook(DateTime? From, DateTime? To, long?[] Users)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            string chids = null;
//            if (Users != null)
//            {
//                chids = new XElement("ChPoDd", Users.Select(x => new XElement("Details",
//                                 new XAttribute("Ids", x)
//                                 ))).ToString();
//            }
//            else
//            {
//                var All = db.Sp_Get_Cashiers_List().ToList();
//                ViewBag.Users = new SelectList(All, "id", "Name");
//                chids = new XElement("ChPoDd", All.Select(x => new XElement("Details",
//                                      new XAttribute("Ids", x.Id)
//                                     ))).ToString();
//            }
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var Payment = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Cash_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").FirstOrDefault();
//            var res = db.Sp_Get_CashBankBook_Closing(From, To, "Cash", Payment.Text_ChartCode, chids).Where(x => x.Head_Code == Payment.Text_ChartCode).ToList();
//            return PartialView(res);
//        }

//        public ActionResult BankBook()
//        {
//            var All = db.Sp_Get_Cashiers_List().ToList();
//            ViewBag.Users = new SelectList(All, "id", "Name");
//            return View();
//        }
//        [HttpPost]
//        public ActionResult SearchBankBook(DateTime? From, DateTime? To, long?[] Users)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            string chids = null;
//            if (Users != null)
//            {
//                chids = new XElement("ChPoDd", Users.Select(x => new XElement("Details",
//                                 new XAttribute("Ids", x)
//                                 ))).ToString();
//            }
//            else
//            {
//                var All = db.Sp_Get_Cashiers_List().ToList();
//                ViewBag.Users = new SelectList(All, "id", "Name");
//                chids = new XElement("ChPoDd", All.Select(x => new XElement("Details",
//                                      new XAttribute("Ids", x.Id)
//                                     ))).ToString();
//            }
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var Payment = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Bank_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
//            //var BankCode = db.Sp_Get_CA_Head_MultiRef("Assets", "Bank Instruments").FirstOrDefault().Code;

//            var res = db.Sp_Get_CashBankBook_Closing(From, To, "Bank", Payment.Text_ChartCode, chids).ToList();
//            return PartialView(res);
//        }

//        public void super()
//        {
//            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Cashier")).Select(x => x.Id).ToList();
//            var res = db.Sp_Get_PendingPayments().Where(x => All.Contains((long)x.Recorded_By)).Select(x => x.GroupId).Distinct().ToList();
//            EntriesSupervision(res);
//        }

//        public JsonResult EntriesSupervision(List<long?> Grp)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    foreach (var item in Grp)
//                    {
//                        var res = db.Sp_Update_Journal_Entry_Sup(item, userid).FirstOrDefault();
//                    }
//                    Transaction.Commit();
//                    return Json(new Return { Status = true, Msg = "Supervised" });
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new Return { Status = false, Msg = "Error Occured" });

//                }
//            }
//        }

//        public JsonResult GetPayableBalance(int? HeadId)
//        {
//            var res = db.Sp_Get_Liability_Head_Balance(HeadId).FirstOrDefault();
//            var balance = res.Credit - res.Debit;
//            return Json(balance);
//        }
//        public ActionResult PayableEntrySupervision(long Id)
//        {
//            var res2 = db.Sp_Get_GeneralEntries_Parameter(Id).ToList();
//            var res3 = db.Bills.Where(x => x.GroupId == Id).ToList();
//            var res4 = GetHead(Convert.ToInt32(res3.FirstOrDefault().Vendor_Id));
//            var res = new PayableVouchers { General_Entries = res2, BillItems = res3, vendor = res4 };
//            return View(res);
//        }
//        public ActionResult BillsNeededUpdation()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Sp_Get_GeneralEntry_Bills_Updation(userid).ToList();
//            return View(res);
//        }
//        public ActionResult BillUpdation(long Id)
//        {
//            ViewBag.TransactionId = new Helpers().RandomNumber();
//            var res2 = db.Sp_Get_GeneralEntries_Parameter(Id).ToList();
//            var res3 = db.Bills.Where(x => x.GroupId == Id).ToList();
//            var res4 = GetHead(Convert.ToInt32(res3.FirstOrDefault().Vendor_Id));
//            var res = new PayableVouchers { General_Entries = res2, BillItems = res3, vendor = res4 };
//            return View(res);
//        }
//        public JsonResult UpdateBillEntries(List<Bill_Details> Details, DateTime DueDate, DateTime BillDate, long TransactionId, long GroupId)
//        {


//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            string Bill = new XElement("Bills", Details.Select(x => new XElement("Entries",
//            new XAttribute("Vendor_Id", x.VendorId),
//            new XAttribute("Terms", x.terms),
//            new XAttribute("Bill_No", x.BillNo),
//            new XAttribute("Head_Code", GetHead(x.Head_id).Text_ChartCode),
//            new XAttribute("Head_Name", GetHead(x.Head_id).Head),
//            new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//            new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//            new XAttribute("Line", x.Line),
//            new XAttribute("description", x.description),
//            new XAttribute("Head_Id", x.Head_id),
//            new XAttribute("GroupId", TransactionId),
//                //new XAttribute("PO_Group_Id", PO_group_Id),
//                new XAttribute("Comp_Id", comp.Id),
//            new XAttribute("Head", (x.Head == null) ? "" : x.Head)
//            ))).ToString();

//            try
//            {
//                db.Sp_Delete_Reverted_Bills(GroupId);
//                var res = db.Sp_Add_Bills(Bill, userid, BillDate, DueDate);
//                decimal? amount = 0;
//                if (Details.Any())
//                {
//                    foreach (var item in Details)
//                    {
//                        decimal? rate = (item.Rate == null) ? 0 : item.Rate;
//                        decimal? qty = (item.Quantity == null) ? 0 : item.Quantity;
//                        amount += rate * qty;
//                    }
//                }
//                var vendor_coa = GetHead(Details.FirstOrDefault().VendorId);


//                var GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
//                new XAttribute("Naration", x.description),
//                new XAttribute("Head", (x.Head == null) ? "" : x.Head),
//                new XAttribute("Head_Name", GetHead(x.Head_id).Head),
//                new XAttribute("Head_Code", GetHead(x.Head_id).Text_ChartCode),
//                new XAttribute("Head_Id", x.Head_id),
//                new XAttribute("Line", x.Line),
//                new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//                new XAttribute("UOM", ""),
//                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                new XAttribute("Debit", ((x.Rate == null) ? 0 : x.Rate) * ((x.Quantity == null) ? 0 : x.Quantity)),
//                new XAttribute("Credit", 0),
//                new XAttribute("Comp_Id", comp.Id),
//                new XAttribute("GroupId", TransactionId)
//                )));
//                var GEapp = new XElement("GeneralEntries", new XElement("Entries",
//                new XAttribute("Naration", "Payable Record Against Bill No. " + Details.FirstOrDefault().BillNo + " for Vendor " + vendor_coa.Head),
//                new XAttribute("Head", vendor_coa.Head),
//                new XAttribute("Head_Name", vendor_coa.Head),
//                new XAttribute("Head_Code", vendor_coa.Text_ChartCode),
//                new XAttribute("Head_Id", vendor_coa.Id),
//                new XAttribute("Line", Details.Count() + 1),
//                new XAttribute("Qty", 0),
//                new XAttribute("UOM", ""),
//                new XAttribute("Rate", 0),
//                new XAttribute("Debit", 0),
//                new XAttribute("Credit", amount),
//                new XAttribute("Comp_Id", comp.Id),
//                new XAttribute("GroupId", TransactionId)
//                ));
//                GeneralEnt.Add(
//                    from ge in GEapp.Elements()
//                    select ge
//                    );
//                var res1 = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
//                return Json(res);
//            }
//            catch (Exception ex)
//            {
//                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(false);
//            }
//        }

//        public ActionResult ContraEntry()
//        {
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            return View();
//        }
//        public JsonResult RecordContraEntries(List<Voucher_Details_General_Entries> Details, long TransactionId, string paytype, string instnumber, string branch, DateTime? instDate, string drAcc, string crAcc)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController de = new AccountHandlerController();
//            var comp = de.Company_Attr(userid);

//            string GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
//            new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
//            new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
//            new XAttribute("Head_Name", this.GetHead(x.Account_Head_Id).Head),
//            new XAttribute("Head_Code", this.GetHead(x.Account_Head_Id).Text_ChartCode),
//            new XAttribute("Head_Id", x.Account_Head_Id),
//            new XAttribute("Line", x.Line),
//            new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//            new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
//            new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//            new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
//            new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
//            new XAttribute("Comp_Id", comp.Id),
//            new XAttribute("GroupId", TransactionId)

//            ))).ToString();
//            var vouchtype = "";
//            string[] drwords = drAcc.Split(' ');
//            string[] crwords = crAcc.Split(' ');
//            var draccount = string.Join(" ", drwords, 2, drwords.Count() - 2);
//            var craccount = string.Join(" ", crwords, 2, crwords.Count() - 2);

//            if (draccount == "Cash")
//            {
//                vouchtype = "CPV";
//            }
//            else if (craccount == "Cash")
//            {
//                vouchtype = "CRV";
//            }
//            else
//            {
//                vouchtype = "BRV";
//            }

//            //if (Details.Any())
//            //{
//            //    foreach (var v in Details)
//            //    {
//            //        if (v.Account_Head == "/1/1/3/ - Cash" && v.Debit > 0)
//            //        {
//            //            vouchtype = "CRV";
//            //            amt = Convert.ToDecimal(v.Debit);
//            //            break;
//            //        }
//            //        else if (v.Account_Head == "/1/1/3/ - Cash" && v.Credit > 0)
//            //        {
//            //            vouchtype = "CPV";
//            //            amt = Convert.ToDecimal(v.Credit);
//            //            break;
//            //        }
//            //        else
//            //        {
//            //            amt = Convert.ToDecimal(v.Credit);
//            //        }
//            //    }
//            //}


//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    if (vouchtype == "CRV")
//                    {
//                        var receipt = db.Sp_Add_Receipt_Contra(Details.FirstOrDefault().Debit, GeneralMethods.NumberToWords(Convert.ToInt32(Details.FirstOrDefault().Debit)), draccount, instnumber, instDate, branch, "", "", null, "", paytype, null,
//                                                "Meher Estate Developers", 0, null, "", ReceiptTypes.Contra.ToString(), userid, userid, "", null, Modules.Accounts_Management.ToString(), "",
//                                                "", "", "", 0, TransactionId).FirstOrDefault();
//                        var res = db.Sp_Add_General_Entries_Param(GeneralEnt, userid, receipt.Receipt_No, vouchtype).FirstOrDefault();
//                    }
//                    else if (vouchtype == "CPV")
//                    {
//                        var username = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
//                        var receipt = db.Sp_Add_Voucher_Vendor_Contra("-", Details.FirstOrDefault().Debit, GeneralMethods.NumberToWords(Convert.ToInt32(Details.FirstOrDefault().Debit)), "", "", null, "", "", "", "", null, Modules.Accounts_Management.ToString(), "",
//                        PaymentMethod.Cash.ToString(), "Meher Estate Developers", "", userid, "Contra Voucher", userid, null, TransactionId, username, userid, DateTime.Now, "", null, null).FirstOrDefault();
//                        var res = db.Sp_Add_General_Entries_Param(GeneralEnt, userid, receipt.Receipt_No, vouchtype).FirstOrDefault();
//                    }
//                    else
//                    {
//                        var receipt = db.Sp_Add_Receipt_Contra(Details.FirstOrDefault().Debit, GeneralMethods.NumberToWords(Convert.ToInt32(Details.FirstOrDefault().Debit)), draccount, instnumber, instDate, branch, "", "", null, "", paytype, null,
//                                                "Meher Estate Developers", 0, null, "", ReceiptTypes.Contra.ToString(), userid, userid, "", null, Modules.Accounts_Management.ToString(), "",
//                                                "", "", "", 0, TransactionId).FirstOrDefault();
//                        var res = db.Sp_Add_General_Entries_Param(GeneralEnt, userid, receipt.Receipt_No, vouchtype).FirstOrDefault();
//                    }
//                    Transaction.Commit();
//                    return Json(true);
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(false);
//                }
//            }

//        }
//        public JsonResult GetReceivableBalance(int? HeadId)
//        {
//            var headBal = db.Sp_Get_CA_Head_Receivable_Balance(HeadId).FirstOrDefault();
//            return Json(headBal);
//        }

//        public ActionResult ExpensePayable()
//        {
//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            return PartialView();
//        }
//        public ActionResult PayOpenPaymentCash()
//        {
//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            ViewBag.Amount = 0.00;
//            ViewBag.Paid_Amount = 0.00;
//            ViewBag.Remaining = 0.00;
//            ViewBag.TotalRemaining = 1000.00;

//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();

//            return PartialView();
//        }
//        public ActionResult PayOpenPaymentBank()
//        {
//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            ViewBag.BanksList = new SelectList(db.Bank_Accounts.Select(x => new { id = x.Id, text = x.Bank + " - " + x.Account_Number }).ToList(), "id", "text");
//            ViewBag.Amount = 0.00;
//            ViewBag.Paid_Amount = 0.00;
//            ViewBag.Remaining = 0.00;
//            ViewBag.TotalRemaining = 1000.00;
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            return PartialView();
//        }
//        public ActionResult PaymentCash()
//        {
//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            ViewBag.Amount = 0.00;
//            ViewBag.Paid_Amount = 0.00;
//            ViewBag.Remaining = 0.00;
//            ViewBag.TotalRemaining = 1000.00;

//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();

//            return PartialView();
//        }
//        public JsonResult OpenPayableEntry(int payAccId, string paidBy, string instNo, string InstBank, DateTime? instDate, string branch, string payType, decimal? Amount, string Description, DateTime? EntryDate, int? BankId, decimal? TaxAmount, int? TaxAccount, long? TransactionId, string Department)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var username = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
//            AccountHandlerController de = new AccountHandlerController();
//            var comp = de.Company_Attr(userid);
//            var debAccount = GetHead(payAccId);
//            string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Amount));
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var r = db.Sp_Add_Voucher_Vendor("-", Amount, AmountinWords, InstBank, branch, instDate, instNo, "", Description, "", payAccId, Modules.Accounts_Management.ToString(), paidBy,
//                       payType, "Iqbal Gardens", "", TransactionId, "Payment Voucher", userid, payAccId, TransactionId, username, userid, EntryDate, "", null, null, TransactionId).FirstOrDefault();

//                    bool headcashier = false;
//                    if (User.IsInRole("Head Cashier"))
//                    {
//                        headcashier = true;
//                    }

//                    //de.PR_Receive_Receivable_Amount(Amount, payType, instNo, instDate, InstBank, Description + " - Paid By: " + paidBy, trans, userid, r.Receipt_No.ToString(), 1, payAccId, headcashier);
//                    PaymentTypeModel credAccount = new PaymentTypeModel();
//                    if (payType == "Cash")
//                    {
//                        credAccount = de.Payment_Head(payType, Transaction_Type.Credit.ToString(), null, comp.Id);
//                    }
//                    else
//                    {
//                        credAccount = de.Payment_Head(payType, Transaction_Type.Credit.ToString(), BankId, comp.Id);
//                    }
//                    var vouno = credAccount.VoucherType + "-" + db.Sp_Get_ReceiptNo(credAccount.VoucherType).FirstOrDefault();

//                    if (headcashier)
//                    {
//                        var JournalEnt = new XElement("JournalEntries", new XElement("Entries",
//                            new XAttribute("Naration", (Description == null) ? "" : Description),
//                            new XAttribute("Head", (debAccount.Head == null) ? "" : debAccount.Text_ChartCode + " - " + debAccount.Head),
//                            new XAttribute("Head_Name", (debAccount.Head == null) ? "" : debAccount.Head),
//                            new XAttribute("Head_Code", (debAccount.Text_ChartCode == null) ? "" : debAccount.Text_ChartCode),
//                            new XAttribute("Head_Id", (debAccount.Id == null) ? 0 : debAccount.Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", (Amount + TaxAmount)),
//                            new XAttribute("Credit", 0),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstNo", (instNo == null) ? "" : instNo),
//                            (instDate == null) ? null : new XAttribute("InstDate", instDate),
//                            new XAttribute("InstBank", (payType == "Cash") ? "" : InstBank),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("Department", Department),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                        ));
//                        // Journal Entry Credit
//                        var JEapp = new XElement("JournalEntries", new XElement("Entries",
//                            new XAttribute("Naration", (Description == null) ? "" : Description),
//                            new XAttribute("Head", (credAccount.PaymentData.Head == null) ? "" : credAccount.PaymentData.Text_ChartCode + " - " + credAccount.PaymentData.Head),
//                            new XAttribute("Head_Name", (credAccount.PaymentData.Head == null) ? "" : credAccount.PaymentData.Head),
//                            new XAttribute("Head_Code", (credAccount.PaymentData.Text_ChartCode == null) ? "" : credAccount.PaymentData.Text_ChartCode),
//                            new XAttribute("Head_Id", (credAccount.PaymentData.Id == null) ? 0 : credAccount.PaymentData.Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", 0),
//                            new XAttribute("Credit", Amount),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstNo", (instNo == null) ? "" : instNo),
//                            (instDate == null) ? null : new XAttribute("InstDate", instDate),
//                            new XAttribute("InstBank", (payType == "Cash") ? "" : InstBank),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("Department", Department),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                        ));
//                        JournalEnt.Add(
//                            from ge in JEapp.Elements()
//                            select ge
//                            );
//                        if (TaxAmount > 0)
//                        {
//                            var taxAccount = GetHead(Convert.ToInt32(TaxAccount));
//                            if (taxAccount is null)
//                            {
//                                Transaction.Rollback();
//                                return Json(new { Status = false, Msg = "Tax Account not mapped. Please map tax account for further use." });
//                            }
//                            var Taxapp = new XElement("JournalEntries", new XElement("Entries",
//                            new XAttribute("Naration", (Description == null) ? "" : Description),
//                            new XAttribute("Head", (taxAccount.Head == null) ? "" : taxAccount.Text_ChartCode + " - " + taxAccount.Head),
//                            new XAttribute("Head_Name", (taxAccount.Head == null) ? "" : taxAccount.Head),
//                            new XAttribute("Head_Code", (taxAccount.Text_ChartCode == null) ? "" : taxAccount.Text_ChartCode),
//                            new XAttribute("Head_Id", (taxAccount.Id == null) ? 0 : taxAccount.Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", 0),
//                            new XAttribute("Credit", TaxAmount),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstNo", (instNo == null) ? "" : instNo),
//                            (instDate == null) ? null : new XAttribute("InstDate", instDate),
//                            new XAttribute("InstBank", (payType == "Cash") ? "" : InstBank),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                        ));
//                            JournalEnt.Add(
//                                from ge in Taxapp.Elements()
//                                select ge
//                                );
//                        }
//                        var res = db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();
//                    }
//                    else
//                    {
//                        var GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
//                            new XAttribute("Naration", (Description == null) ? "" : Description),
//                            new XAttribute("Head", (debAccount.Head == null) ? "" : debAccount.Text_ChartCode + " - " + debAccount.Head),
//                            new XAttribute("Head_Name", (debAccount.Head == null) ? "" : debAccount.Head),
//                            new XAttribute("Head_Code", (debAccount.Text_ChartCode == null) ? "" : debAccount.Text_ChartCode),
//                            new XAttribute("Head_Id", (debAccount.Id == null) ? 0 : debAccount.Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", (Amount + TaxAmount)),
//                            new XAttribute("Credit", 0),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstNo", (instNo == null) ? "" : instNo),
//                            (instDate == null) ? null : new XAttribute("InstDate", instDate),
//                            new XAttribute("InstBank", (payType == "Cash") ? "" : InstBank),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                        ));
//                        var GEapp = new XElement("GeneralEntries", new XElement("Entries",
//                            new XAttribute("Naration", (Description == null) ? "" : Description),
//                            new XAttribute("Head", (credAccount.PaymentData.Head == null) ? "" : credAccount.PaymentData.Text_ChartCode + " - " + credAccount.PaymentData.Head),
//                            new XAttribute("Head_Name", (credAccount.PaymentData.Head == null) ? "" : credAccount.PaymentData.Head),
//                            new XAttribute("Head_Code", (credAccount.PaymentData.Text_ChartCode == null) ? "" : credAccount.PaymentData.Text_ChartCode),
//                            new XAttribute("Head_Id", (credAccount.PaymentData.Id == null) ? 0 : credAccount.PaymentData.Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", 0),
//                            new XAttribute("Credit", Amount),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstNo", (instNo == null) ? "" : instNo),
//                            (instDate == null) ? null : new XAttribute("InstDate", instDate),
//                            new XAttribute("InstBank", (payType == "Cash") ? "" : InstBank),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                            ));
//                        GeneralEnt.Add(
//                            from ge in GEapp.Elements()
//                            select ge
//                            );
//                        if (TaxAmount > 0)
//                        {
//                            var taxAccount = GetHead(Convert.ToInt32(TaxAccount));
//                            if (taxAccount is null)
//                            {
//                                return Json(new { Status = false, Msg = "Tax Account not mapped. Please map tax account for further use." });
//                            }
//                            var Taxapp = new XElement("GeneralEntries", new XElement("Entries",
//                            new XAttribute("Naration", (Description == null) ? "" : Description),
//                            new XAttribute("Head", (taxAccount.Head == null) ? "" : taxAccount.Text_ChartCode + " - " + taxAccount.Head),
//                            new XAttribute("Head_Name", (taxAccount.Head == null) ? "" : taxAccount.Head),
//                            new XAttribute("Head_Code", (taxAccount.Text_ChartCode == null) ? "" : taxAccount.Text_ChartCode),
//                            new XAttribute("Head_Id", (taxAccount.Id == null) ? 0 : taxAccount.Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", 0),
//                            new XAttribute("Credit", TaxAmount),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstNo", (instNo == null) ? "" : instNo),
//                            (instDate == null) ? null : new XAttribute("InstDate", instDate),
//                            new XAttribute("InstBank", (payType == "Cash") ? "" : InstBank),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("Department", Department),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                            ));
//                            GeneralEnt.Add(
//                                from ge in Taxapp.Elements()
//                                select ge
//                                );
//                        }
//                        var res = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
//                    }
//                    Transaction.Commit();
//                    return Json(new { Status = true, Msg = "Payable Entry Successful.", VoucherId = TransactionId, Token = TransactionId });
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Error Occured" });
//                }
//            }
//        }
//        public JsonResult OpenCashPayableEntry(List<Lines> Lines, string paidBy, string payType, DateTime? EntryDate, long? TransactionId, string Department)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var username = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
//            AccountHandlerController de = new AccountHandlerController();
//            var comp = de.Company_Attr(userid);
//            string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Lines.Sum(x => x.amount)));
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var r = db.Sp_Add_Voucher_Vendor("-", Lines.Sum(x => x.amount), AmountinWords, "", "", null, null, "", string.Join(", ", Lines.Select(x => x.narration).Distinct()), "", TransactionId, Modules.Accounts_Management.ToString(), paidBy,
//                       payType, "Meher Estate Developers", "", TransactionId, "Payment Voucher", userid, TransactionId, TransactionId, username, userid, EntryDate, "", null, null, TransactionId).FirstOrDefault();

//                    bool headcashier = false;
//                    if (User.IsInRole("Head Cashier"))
//                    {
//                        headcashier = true;
//                    }

//                    //de.PR_Receive_Receivable_Amount(Amount, payType, instNo, instDate, InstBank, Description + " - Paid By: " + paidBy, trans, userid, r.Receipt_No.ToString(), 1, payAccId, headcashier);
//                    PaymentTypeModel credAccount = new PaymentTypeModel();
//                    if (payType == "Cash")
//                    {
//                        credAccount = de.Payment_Head(payType, Transaction_Type.Credit.ToString(), null, comp.Id);
//                    }
//                    var vouno = credAccount.VoucherType + "-" + db.Sp_Get_ReceiptNo(credAccount.VoucherType).FirstOrDefault();

//                    if (headcashier)
//                    {
//                        var JournalEnt = new XElement("JournalEntries", Lines.Select(x => new XElement("Entries",
//                           new XAttribute("Naration", (x.narration == null) ? "" : x.narration),
//                           new XAttribute("Head", GetHead(x.payee_id).Head),
//                           new XAttribute("Head_Name", GetHead(x.payee_id).Head),
//                           new XAttribute("Head_Code", GetHead(x.payee_id).Text_ChartCode),
//                           new XAttribute("Head_Id", GetHead(x.payee_id).Id),
//                           new XAttribute("Line", 1),
//                           new XAttribute("Qty", 0),
//                           new XAttribute("UOM", ""),
//                           new XAttribute("Rate", 0),
//                           new XAttribute("Debit", x.amount),
//                           new XAttribute("Credit", 0),
//                           new XAttribute("Description", paidBy),
//                           new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                           new XAttribute("GroupId", TransactionId),
//                           new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                           new XAttribute("VoucherNo", vouno),
//                           new XAttribute("Comp_Id", comp.Id),
//                           new XAttribute("VoucherType", credAccount.VoucherType)
//                       )));
//                        // Journal Entry Credit
//                        var JEapp = new XElement("JournalEntries", Lines.Select(x => new XElement("Entries",
//                            new XAttribute("Naration", x.narration),
//                            new XAttribute("Head", (credAccount.PaymentData.Head == null) ? "" : credAccount.PaymentData.Text_ChartCode + " - " + credAccount.PaymentData.Head),
//                            new XAttribute("Head_Name", (credAccount.PaymentData.Head == null) ? "" : credAccount.PaymentData.Head),
//                            new XAttribute("Head_Code", (credAccount.PaymentData.Text_ChartCode == null) ? "" : credAccount.PaymentData.Text_ChartCode),
//                            new XAttribute("Head_Id", (credAccount.PaymentData.Id == null) ? 0 : credAccount.PaymentData.Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", 0),
//                            new XAttribute("Credit", x.amount),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                        )));
//                        JournalEnt.Add(
//                            from ge in JEapp.Elements()
//                            select ge
//                            );
//                        var res = db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();
//                    }
//                    else
//                    {
//                        var GeneralEnt = new XElement("GeneralEntries", Lines.Select(x => new XElement("Entries",
//                            new XAttribute("Naration", (x.narration == null) ? "" : x.narration),
//                            new XAttribute("Head", GetHead(x.payee_id).Head),
//                            new XAttribute("Head_Name", GetHead(x.payee_id).Head),
//                            new XAttribute("Head_Code", GetHead(x.payee_id).Text_ChartCode),
//                            new XAttribute("Head_Id", GetHead(x.payee_id).Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", x.amount),
//                            new XAttribute("Credit", 0),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                        )));
//                        var GEapp = new XElement("GeneralEntries", Lines.Select(x => new XElement("Entries",
//                            new XAttribute("Naration", x.narration),
//                            new XAttribute("Head", (credAccount.PaymentData.Head == null) ? "" : credAccount.PaymentData.Text_ChartCode + " - " + credAccount.PaymentData.Head),
//                            new XAttribute("Head_Name", (credAccount.PaymentData.Head == null) ? "" : credAccount.PaymentData.Head),
//                            new XAttribute("Head_Code", (credAccount.PaymentData.Text_ChartCode == null) ? "" : credAccount.PaymentData.Text_ChartCode),
//                            new XAttribute("Head_Id", (credAccount.PaymentData.Id == null) ? 0 : credAccount.PaymentData.Id),
//                            new XAttribute("Line", 1),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", 0),
//                            new XAttribute("Credit", x.amount),
//                            new XAttribute("Description", paidBy),
//                            new XAttribute("Recorded_Date", (EntryDate == null) ? DateTime.Now : EntryDate),
//                            new XAttribute("GroupId", TransactionId),
//                            new XAttribute("InstStatus", (payType == "Cash") ? "" : "Pending"),
//                            new XAttribute("VoucherNo", vouno),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("VoucherType", credAccount.VoucherType)
//                            )));
//                        GeneralEnt.Add(
//                            from ge in GEapp.Elements()
//                            select ge
//                            );
//                        var res = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
//                    }
//                    Transaction.Commit();
//                    return Json(new { Status = true, Msg = "Payable Entry Successful.", VoucherId = TransactionId, Token = TransactionId });
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Error Occured" });
//                }
//            }
//        }

//        public ActionResult ImportJournal()
//        {
//            return View();
//        }
//        //public JsonResult ImportJournalEntry(List<GeneralEntryData> AllData)
//        //{
//        //    long user = long.Parse(User.Identity.GetUserId());

//        //    var Cashcode = "/1/1/1/1/1/";
//        //    var bankcodes = db.Sp_Get_CA_LeaveNodes_HeadSearch_CodeLevel("", "/1/1/2/1/").Select(x => x.Text_ChartCode).ToList();

//        //    foreach (var item in AllData)
//        //    {
//        //        var grp = new Helpers().RandomNumber();
//        //        var deb_code = db.Sp_Get_CA_Head_Param_Code(item.DEBIT_ACCOUNT).FirstOrDefault();
//        //        var Cre_code = db.Sp_Get_CA_Head_Param_Code(item.CREDIT_ACCOUNT).FirstOrDefault();


//        //        string vouno = "", vouchtype = "";
//        //        if (deb_code.HeadCode == Cashcode)
//        //        {
//        //            vouno = "CRV-" + db.Sp_Get_ReceiptNo("Receipt Voucher").FirstOrDefault();
//        //            vouchtype = "CRV";
//        //        }
//        //        else if (Cre_code.HeadCode == Cashcode)
//        //        {
//        //            vouno = "CPV-" + db.Sp_Get_ReceiptNo("Payment Voucher").FirstOrDefault();
//        //            vouchtype = "CPV";
//        //        }
//        //        else if (bankcodes.Contains(deb_code.HeadCode))
//        //        {
//        //            vouno = "BRV-" + db.Sp_Get_ReceiptNo("Receipt Voucher").FirstOrDefault();
//        //            vouchtype = "BRV";
//        //        }
//        //        else if (bankcodes.Contains(Cre_code.HeadCode))
//        //        {
//        //            vouno = "BPV-" + db.Sp_Get_ReceiptNo("Payment Voucher").FirstOrDefault();
//        //            vouchtype = "BPV";
//        //        }
//        //        else
//        //        {
//        //            vouno = "JV-" + db.Sp_Get_ReceiptNo("Journal Voucher").FirstOrDefault();
//        //            vouchtype = "JV";
//        //        }
//        //        var res = db.Sp_Add_Journal_Entry(item.DEBIT, 0, deb_code.Head, deb_code.Id, deb_code.HeadCode, deb_code.Head, item.NARRATION, grp, 1, user, user, null, null, item.INST_NO, "Approved", item.INST_DATE, null, null, null, vouno, vouchtype, comp_I);
//        //        var res1 = db.Sp_Add_Journal_Entry(0, item.CREDIT, Cre_code.Head, Cre_code.Id, Cre_code.HeadCode, Cre_code.Head, item.NARRATION, grp, 1, user, user, null, null, item.INST_NO, "Approved", item.INST_DATE, null, null, null, vouno, vouchtype, item.DATE);
//        //    }
//        //    return Json(new Return { Msg = "Uploaded", Status = true });
//        //}
//    }
//}