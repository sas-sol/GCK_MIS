using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.IO;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class ReceiptsController : Controller
    {
        // GET: Receipts
        Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult Receipt(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).FirstOrDefault();
            return View(res);
        }
        public ActionResult DupReceipt(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).FirstOrDefault();
            return View(res);
        }
        public ActionResult AdjustmentReceipt(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).FirstOrDefault();
            return View(res);
        }
        public ActionResult PlotTransferReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult ServiceChargesReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult ElectricChargesReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }

        public ActionResult FaisalHeight(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult CommercialReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult PlotPaymentReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult CancellationReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult canc()
        {
            var res = db.Cancellation_Receipts.Where(x => x.TokenParameter == 1).ToList();
            return View(res);
        }
        public ActionResult RefundReceipt(long Id, long Token)
        {
            var res = db.Cancellation_Receipts.Where(x => x.Id == Id && x.TokenParameter == Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult PlotCanReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult NewConnctionCharges(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult FineCharges(string Id,string ReceiptType, long Token)
        {
            ViewBag.type = ReceiptType;
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult DealerAdvance(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult SAM_Receipts(long Id, long Token)
        {
            var res = db.SAM_Receipts.Where(x => x.Id == Id && x.TokenParameter == Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult SAM_Receipt(long Id)
        {
            var res1 = db.SAM_Receipts.Where(x => x.Id == Id).Select(x => new Marketing_Receipt
            {
                Company = x.Company,
                ReceiptNo = x.ReceiptNo,
                Text = x.Text,
                Name = x.Name,
                Father_Name = x.Father_Name,
                Contact = x.Contact,
                AmountinWords = x.AmountinWords,
                PaymentType = x.PaymentType,
                DateTime = x.DateTime,
                Bank = x.Bank,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Project = x.Project,
                File_Plot_Number = x.File_Plot_Number,
                Block = x.Block,
                Size = x.Size,
                Amount = x.Amount
            }).FirstOrDefault();
            if (res1 == null)
            {
                var res2 = db.PropertyDeal_Receipts.Where(x => x.Id == Id).Select(x => new Marketing_Receipt
                {
                    Company = x.Company,
                    ReceiptNo = x.ReceiptNo,
                    Text = x.Text,
                    Name = x.Name,
                    Father_Name = x.Father_Name,
                    Contact = x.Contact,
                    AmountinWords = x.AmountinWords,
                    PaymentType = x.PaymentType,
                    DateTime = x.DateTime,
                    Bank = x.Bank,
                    Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                    Project = x.Project,
                    File_Plot_Number = x.File_Plot_Number,
                    Block = x.Block,
                    Size = x.Size,
                    Amount = x.Amount
                }).FirstOrDefault();
                return View(res2);
            }
            return View(res1);
        }
        public ActionResult Dup_SAM_Receipts(long Id)
        {
            var res1 = db.SAM_Receipts.Where(x => x.Id == Id).Select(x => new Marketing_Receipt
            {
                Company = x.Company,
                ReceiptNo = x.ReceiptNo,
                Text = x.Text,
                Name = x.Name,
                Father_Name = x.Father_Name,
                Contact = x.Contact,
                AmountinWords = x.AmountinWords,
                PaymentType = x.PaymentType,
                DateTime = x.DateTime,
                Bank = x.Bank,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Project = x.Project,
                File_Plot_Number = x.File_Plot_Number,
                Block = x.Block,
                Size = x.Size,
                Amount = x.Amount
            }).FirstOrDefault();
            if (res1 == null)
            {
                var res2 = db.PropertyDeal_Receipts.Where(x => x.Id == Id).Select(x => new Marketing_Receipt
                {
                    Company = x.Company,
                    ReceiptNo = x.ReceiptNo,
                    Text = x.Text,
                    Name = x.Name,
                    Father_Name = x.Father_Name,
                    Contact = x.Contact,
                    AmountinWords = x.AmountinWords,
                    PaymentType = x.PaymentType,
                    DateTime = x.DateTime,
                    Bank = x.Bank,
                    Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                    Project = x.Project,
                    File_Plot_Number = x.File_Plot_Number,
                    Block = x.Block,
                    Size = x.Size,
                    Amount = x.Amount
                }).FirstOrDefault();
                return View(res2);
            }
            return View(res1);
        }
        public ActionResult SPE_Receipts(string Id, long Token)
        {
            var res = db.SAM_Receipts.Where(x => x.ReceiptNo == Id && x.TokenParameter == Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult PropertyDeal_Receipts(long Id, long Token)
        {
            var res = db.PropertyDeal_Receipts.Where(x => x.Id == Id && x.TokenParameter == Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult Cancellation_Receipts(long Id, long Token)
        {
            var res = db.Cancellation_Receipts.Where(x => x.Id == Id && x.TokenParameter == Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult Cancellation_Receipts_Commercial(long Id, long Token)
        {
            var res = db.Cancellation_Receipts.Where(x => x.Id == Id && x.TokenParameter == Token).SingleOrDefault();
            return View(res);
        }

        public ActionResult OtherRecovery(long Id, long Token)
        {
            var res = db.Receipts.Where(x => x.Id == Id && x.TokenParameter == Token).SingleOrDefault();
            return View(res);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public ActionResult Dealership_Registeration(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult MembershipReceipts(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult SportssMonthlyFee(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult TaxReceipt(string Token)
        {
            if (string.IsNullOrEmpty(Token) || string.IsNullOrWhiteSpace(Token))
            {
                return View();
            }
            else
            {
                var res = db.Receipts.Where(x => x.DevelopmentCharges == Token).ToList();
                return View(res);
            }
        }
        public ActionResult TaxCollection()
        {
            ViewBag.Cashiers = new SelectList(db.Users.Where(x => x.Roles.Any(z => z.Name == "Cashier")).Select(x => new { x.Id, x.Name }).ToList(), "Id", "Name");
            return View();
        }
        public ActionResult TaxCollectionDetails(DateTime? rangeStart, DateTime? rangeEnd, long? uid)
        {
            var res = db.Sp_Get_TaxReport(rangeStart, rangeEnd, uid).ToList();
            return PartialView(res);
        }
        //public JsonResult UpdateWHTDetails(List<long> recs, DateTime? dt)
        //{
        //    try
        //    {
        //        long uid = User.Identity.GetUserId<long>();
        //        if (dt is null)
        //        {
        //            dt = DateTime.Now;
        //        }
        //        var recIds = new XElement("WHTReceipts", recs.Select(x => new XElement("RecIds",
        //            new XAttribute("ReceiptId", x)))).ToString();
        //        db.Sp_Update_WHTReceipts(recIds, dt, uid);
        //        return Json(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(false);
        //    }
        //}

        public ActionResult PunchCPR(long recId)
        {
            var res = db.Receipts.Where(x => x.Id == recId).FirstOrDefault();
            return PartialView(res);
        }

        [HttpPost]
        //public JsonResult SaveCPR(long recId, DateTime pdDt)
        //{
        //    try
        //    {
        //        if (Request.Files.Count <= 0)
        //        {
        //            return Json("No CPR File was attached. Please attach a CPR file before saving.");
        //        }

        //        var uid = User.Identity.GetUserId<long>();
        //        var recIds = new XElement("WHTReceipts", new XElement("RecIds",
        //           new XAttribute("ReceiptId", recId))).ToString();
        //        db.Sp_Update_WHTReceipts(recIds, pdDt, uid);

        //        string pth = Server.MapPath("/Repository/WHT Submissions Files/");

        //        if (!Directory.Exists(pth))
        //        {
        //            Directory.CreateDirectory(pth);
        //        }

        //        HttpPostedFileBase file = Request.Files[0];
        //        int fileSize = file.ContentLength;
        //        string fileName = file.FileName;
        //        string mimeType = file.ContentType;
        //        Stream fileContent = file.InputStream;

        //        string[] prsedFileName = fileName.Split('.');
        //        string extFound = prsedFileName[prsedFileName.Length - 1];
        //        string newFileName = recId.ToString() + "." + extFound;
        //        file.SaveAs(pth + newFileName);
        //        return Json(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json("Failed to save the CPR due to internal server error.");
        //    }
        //}

        public ActionResult ArchiCustomers()
        {
            var custs = db.Architecture_Customers.ToList();
            return View(custs);
        }
        public ActionResult ArchitectureReceipt()
        {
            ViewBag.trans = new Helpers().RandomNumber();
            return PartialView();
        }

        public ActionResult ArchitectureVoucher()
        {
            ViewBag.trans = new Helpers().RandomNumber();
            return PartialView();
        }

        public ActionResult AddCustomer()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.Plots = new SelectList(db.Plots.ToList().Select(x => new { Id = x.Id, Name = x.Plot_Number + " " + x.Sector + " - " + x.Type + " - " + x.Block_Name }), "Id", "Name");
            return PartialView();
        }

        public JsonResult SaveNewCustomer(Architecture_Customers ac)
        {
            try
            {
                if (db.Architecture_Customers.Any(x => x.CustomerCNIC == ac.CustomerCNIC && x.PlotNo == ac.PlotNo && x.Block == ac.Block))
                {
                    return Json(false);
                }
                db.Architecture_Customers.Add(ac);
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public JsonResult SearchArchiCustomers_Select(string s)
        {
            var res = db.Architecture_Customers.Where(x => x.CustomerName.Contains(s) || x.CustomerFatherName.Contains(s) || x.CustomerCNIC.Contains(s)).Select(x => new { id = x.Id, text = x.CustomerName + " | " + x.CustomerCNIC }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveArchiFee(Receipt r, List<ArchiServiceDetails> servs, long cust)
        {
            if (r is null)
            {
                return Json(new { Status = false, Msg = "", _Log = "" });
            }
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var custData = db.Architecture_Customers.Where(x => x.Id == cust).FirstOrDefault();

                Architecture_Requests ac = new Architecture_Requests
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy_Id = uid,
                    CreatedBy_Name = uname,
                    CustomerAddress = r.File_Plot_Number,
                    CustomerCNIC = custData.CustomerCNIC,
                    CustomerContact = custData.CustomerContact,
                    CustomerFatherName = custData.CustomerFatherName,
                    CustomerName = custData.CustomerName,
                    PlotSize = r.Size,
                    Status = "Requested",
                    TotalAmount = r.Amount,
                    ServicesDetails = JsonConvert.SerializeObject(servs),
                    CustomerId = cust,
                    TransactionType = "Receipt"
                };

                db.Architecture_Requests.Add(ac);
                db.SaveChanges();

                return Json(new { Status = true, Msg = "Request has been submitted successfully.", _Log = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Msg = "", _Log = "Inner : " + ex.InnerException + " ---- Msg : " + ex.Message + "  ------ Stack Trace : " + ex.StackTrace });
            }
        }

        public JsonResult SaveArchiVoucher(long cust, decimal amount, string desc)
        {
            var income = db.Receipts.Where(x => x.File_Plot_No == cust && x.Type == "Architecture_Fees").ToList().Sum(x => x.Amount);
            var outgo = db.Vouchers.Where(x => x.File_Plot_Id == cust && x.Module == "Architecture_Submission").ToList().Sum(x => x.Amount);

            if (amount > (income - outgo))
            {
                return Json(new Return { Msg = "Cannot request a voucher with amount greater than customer's remaining balance.", Status = false });
            }

            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var custData = db.Architecture_Customers.Where(x => x.Id == cust).FirstOrDefault();

                Architecture_Requests ac = new Architecture_Requests
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy_Id = uid,
                    CreatedBy_Name = uname,
                    CustomerAddress = custData.CustomerAddress,
                    CustomerCNIC = custData.CustomerCNIC,
                    CustomerContact = custData.CustomerContact,
                    CustomerFatherName = custData.CustomerFatherName,
                    CustomerName = custData.CustomerName,
                    Status = "Requested",
                    TotalAmount = amount,
                    ServicesDetails = desc,
                    CustomerId = cust,
                    TransactionType = "Voucher"
                };

                db.Architecture_Requests.Add(ac);
                db.SaveChanges();

                return Json(new { Status = true, Msg = "Request has been submitted successfully.", _Log = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Msg = "", _Log = "Inner : " + ex.InnerException + " ---- Msg : " + ex.Message + "  ------ Stack Trace : " + ex.StackTrace });
            }
        }

        public ActionResult ArchitectureFeeRequests()
        {
            var reqDets = db.Architecture_Requests.ToList();
            return View(reqDets);
        }

        public ActionResult ArchiCustomer(long id)
        {
            var custData = db.Architecture_Customers.Where(x => x.Id == id).FirstOrDefault();
            var rcpts = db.Receipts.Where(x => x.File_Plot_No == id && x.Type == "Architecture_Fees").ToList();
            var vchrs = db.Vouchers.Where(x => x.File_Plot_Id == id && x.Module == "Architecture_Submission").ToList();
            ViewBag.trans = new Helpers().RandomNumber();
            ViewBag.cust = id;
            List<ArchiDepsNWithdrawals> ledger = new List<ArchiDepsNWithdrawals>();

            ledger.AddRange(rcpts.Select(x => new ArchiDepsNWithdrawals { ActionDate = (DateTime)x.DateTime, Credit = (decimal)x.Amount, Debit = 0 }));
            ledger.AddRange(vchrs.Select(x => new ArchiDepsNWithdrawals { ActionDate = (DateTime)x.DateTime, Debit = (decimal)x.Amount, Credit = 0 }));

            return View(new ArchiCustomerLedgerModel { CustomerDetails = custData, LedgerDetails = ledger });
        }

        public JsonResult ArchiReceiptReq(long Id, long token)
        {
            var uid = User.Identity.GetUserId<long>();
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(uid);
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            var reqDets = db.Architecture_Requests.Where(x => x.Id == Id).FirstOrDefault();
            List<ArchiServiceDetails> servs = new List<ArchiServiceDetails>();
            servs = JsonConvert.DeserializeObject<List<ArchiServiceDetails>>(reqDets.ServicesDetails);
            string amtInWords = GeneralMethods.NumberToWords(Convert.ToInt32(reqDets.TotalAmount));

            if (!db.Receipts.Any(x => x.Transaction_Id == token.ToString()))
            {
                var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res = db.Sp_Add_Receipt(reqDets.TotalAmount, amtInWords, "", string.Join(" , ", servs.Select(x => x.Description)), null, "", reqDets.CustomerContact,
                                                    reqDets.CustomerFatherName, reqDets.CustomerId, reqDets.CustomerName, "Cash", null, "", null, "", reqDets.PlotSize, "Architecture_Fees", null, uid, reqDets.CustomerCNIC,
                                                    DateTime.Now, "", "", reqDets.CustomerAddress, "", null, null, token, "", receiptno, comp.Id).FirstOrDefault();
                        bool headcashier = false;
                        if (User.IsInRole("Head Cashier"))
                        {
                            headcashier = true;
                        }
                        AccountHandlerController de = new AccountHandlerController();
                        de.Receipts_Architecture_Receipt(reqDets.TotalAmount, reqDets.CustomerName, reqDets.CustomerCNIC, reqDets.CustomerContact, "Cash", null, null, null, "Received Architecture Fee", new Helpers().RandomNumber(), uid, res.Receipt_No, 1, headcashier);


                        reqDets.ReceiptGeneratedBy_Id = uid;
                        reqDets.ReceiptGeneratedBy_Name = uname;
                        reqDets.ReceiptGenerated_At = DateTime.Now;
                        reqDets.Status = "Receipt_Generated";

                        db.Architecture_Requests.Attach(reqDets);
                        db.Entry(reqDets).Property(x => x.Status).IsModified = true;
                        db.Entry(reqDets).Property(x => x.ReceiptGeneratedBy_Id).IsModified = true;
                        db.Entry(reqDets).Property(x => x.ReceiptGeneratedBy_Name).IsModified = true;
                        db.Entry(reqDets).Property(x => x.ReceiptGenerated_At).IsModified = true;
                        db.SaveChanges();
                        Transaction.Commit();

                        return Json(true);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(false);
                    }
                }
            }
            else
            {
                return Json(false);
            }
        }

        public JsonResult ArchiVoucherReq(long Id, long token)
        {
            var uid = User.Identity.GetUserId<long>();
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(uid);
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            var reqDets = db.Architecture_Requests.Where(x => x.Id == Id).FirstOrDefault();

            string amtInWords = GeneralMethods.NumberToWords(Convert.ToInt32(reqDets.TotalAmount));

            if (!db.Vouchers.Any(x => x.Transaction_Id == token))
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var voch = db.Sp_Add_Voucher(null, reqDets.TotalAmount, GeneralMethods.NumberToWords(Convert.ToInt32(reqDets.TotalAmount)), "", "", null, "", reqDets.CustomerContact,
                       reqDets.ServicesDetails, reqDets.CustomerName, reqDets.CustomerId, "Architecture", reqDets.CustomerName, "Cash", null, null, token, "Architecture_Submission", uid, null, comp.Id).SingleOrDefault();
                        bool headcashier = false;
                        if (User.IsInRole("Head Cashier"))
                        {
                            headcashier = true;
                        }

                        AccountHandlerController de = new AccountHandlerController();
                        de.Receipts_Architecture_Voucher(reqDets.TotalAmount, reqDets.CustomerName, reqDets.CustomerCNIC, reqDets.CustomerContact, "Cash", null, null, null, "Received Architecture Fee", new Helpers().RandomNumber(), uid, voch.Receipt_No, 1, headcashier);


                        reqDets.ReceiptGeneratedBy_Id = uid;
                        reqDets.ReceiptGeneratedBy_Name = uname;
                        reqDets.ReceiptGenerated_At = DateTime.Now;
                        reqDets.Status = "Receipt_Generated";

                        db.Architecture_Requests.Attach(reqDets);
                        db.Entry(reqDets).Property(x => x.Status).IsModified = true;
                        db.Entry(reqDets).Property(x => x.ReceiptGeneratedBy_Id).IsModified = true;
                        db.Entry(reqDets).Property(x => x.ReceiptGeneratedBy_Name).IsModified = true;
                        db.Entry(reqDets).Property(x => x.ReceiptGenerated_At).IsModified = true;
                        db.SaveChanges();
                        Transaction.Commit();
                        return Json(true);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(true);
                    }
                }
            }
            else
            {
                        return Json(false);
            }
        }

        public ActionResult ArchiReceipt(long Id, long token)
        {
            var uid = User.Identity.GetUserId<long>();
            var reqDets = db.Architecture_Requests.Where(x => x.Id == Id).FirstOrDefault();
            List<ArchiServiceDetails> servs = new List<ArchiServiceDetails>();
            servs = JsonConvert.DeserializeObject<List<ArchiServiceDetails>>(reqDets.ServicesDetails);
            var res = db.Receipts.Where(x => x.Transaction_Id == token.ToString()).Select(x => x.Id).FirstOrDefault();
            var recpt = db.Sp_Get_Receipt_Parameter_ById(res).FirstOrDefault();
            return View(new ArchiReceiptModel { archiServices = servs, ReceiptinfoData = recpt });
        }

        public ActionResult ArchiVoucher(long Id, long token)
        {
            var recpt = db.Vouchers.Where(x => x.TokenParameter == token).FirstOrDefault();
            return View(recpt);
        }

        public ActionResult LoanSettlementReceipt(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).FirstOrDefault();
            return View(res);
        }
        public ActionResult InvoiceReceipt(long Id, long Token)
        {
            var res = db.Receipts.Where(x => x.Id == Id && x.TokenParameter == Token).SingleOrDefault();
            res.Company = db.Companies.Where(x => x.Id == res.Comp_Id).FirstOrDefault().Company_Name;
            return View(res);
        }
        public ActionResult SubsidiaryRecovery(long Id, long Token)
        {
            var res = db.Receipts.Where(x => x.Id == Id && x.TokenParameter == Token).SingleOrDefault();
            res.Company = db.Companies.Where(x => x.Id == res.Comp_Id).FirstOrDefault().Company_Name;
            return View(res);
        }

    }
}