using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using MeherEstateDevelopers.Filters;
using System.Collections.Generic;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [LogAction]
    [Authorize]
    public class FinanceController : Controller
    {
        // GET: Finance
        Grand_CityEntities db = new Grand_CityEntities();
        [NoDirectAccess]
        public ActionResult CancelRequests()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Cancel Requests Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        [NoDirectAccess]
        public ActionResult Receipts()
        {
            var All = db.Sp_Get_Cashiers_List().ToList();
            var all = new Sp_Get_Cashiers_List_Result()
            {
                Name = "Bank",
                Id = 0
            };
            All.Add(all);
            ViewBag.Users = new SelectList(All, "id", "Name");

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Receipts Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        [NoDirectAccess]
        public ActionResult ReceiptSearchResult(DateTime? From, DateTime? To, long?[] Users)
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Searched For Recipts from: " + From + " to: " + To, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
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
            var AllType = (from ReceiptTypes e in Enum.GetValues(typeof(ReceiptTypes)) select new { Name = e.ToString() }).ToList();
            string types = new XElement("ReceiptsType", AllType.Select(x => new XElement("Receipt",
                                  new XAttribute("Type", x.Name)
                                 ))).ToString();
            var res = db.Sp_Get_DailyRecovery_Report_Parameter(From, To, chids, types).Select(x => new RecoveryReport
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Cashier_Name = x.Name,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                DateTime = x.DATETIME,
                Dealership_Name = x.Dealership_Name,
                File_Plot_No = x.File_Plot_Number,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                Size = x.Size,
                Type = x.Type,
                Block = x.Block,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Contact = x.Contact,
                ReceiptNo = x.ReceiptNo,
                Module = x.Module,
                Id = x.Id,
                Cancel = x.Cancel
            }).ToList();
            return PartialView(res);
        }

        [NoDirectAccess]
        public ActionResult PendingReceipts()
        {
            var userid = long.Parse(User.Identity.GetUserId());
            if (User.IsInRole("Administrator") || User.IsInRole("Check Others Receipts"))
            {
                var All = db.Users.ToList();
                ViewBag.Users = new SelectList(All, "id", "Name");
            }
            else
            {
                var All = db.Users.Where(x => x.Id == userid).ToList();
                ViewBag.Users = new SelectList(All, "id", "Name");
            }
            return View();
        }
        [NoDirectAccess]
        public ActionResult PendingReceiptSearchResult(DateTime? From, DateTime? To)
        {
            From = (From == null) ? new DateTime(2019, 1, 1) : From;
            To = (To == null) ? DateTime.Now : To;
            var res = db.Sp_Get_Receipt_Pending(From, To).ToList();
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult PendingReceiptWithoutFileId()
        {
            ViewBag.BankList = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return View();
        }
        [NoDirectAccess]
        public ActionResult SearchPendingReceiptWithoutFileId(DateTime From, DateTime To, string Val)
        {
            var res = db.Sp_Get_Receipts_Pending()
                       .Where(x => x.Bank == Val && x.DateTime >= From && x.DateTime <= To)
                       .ToList();
            return View(res);
        }
        public JsonResult UpdatePendingreceipt(long Id, decimal? Amount, string Amntwords, string PaymentType, string Bank, string Instno, string Type)
        {
            var res = db.Sp_Update_Pending_Receipt(Id, Amount, Amntwords, PaymentType, Bank, Instno, Type);
            return Json(true);
        }


        [NoDirectAccess]
        public ActionResult PendingReceiptsDetails(long Id)
        {
            var res = db.Receipts_Pending.Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");

            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult PendingReceiptsMultifileplot(long Id)
        {
            var res = db.Receipts_Pending.Where(x => x.Id == Id).FirstOrDefault();


            //ViewBag.Bank = new SelectList(db.Bank_Accounts.Select(x => new { id = x.Id, text = x.Bank + " - " + x.Account_Number }).ToList(), "id", "text");
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView(res);
        }

        public JsonResult UpdateFilePlotNo(long FilePlotNo, long Rcp_Id, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            if (Type == "FileManagement")
            {
                var fileplotdetails = db.File_Form.Where(x => x.Id == FilePlotNo && x.Status == 1).FirstOrDefault();
                if (fileplotdetails != null)
                {
                    var res2 = db.Sp_Get_FileLastOwner(fileplotdetails.Id).FirstOrDefault();
                    db.Sp_Update_FilePlotnmbr_Pending_Reciept(res2.Name, res2.Father_Husband, res2.Mobile_1, fileplotdetails.Block, fileplotdetails.Plot_Size, fileplotdetails.Type, Type, fileplotdetails.Id, fileplotdetails.FileFormNumber.ToString(), Rcp_Id.ToString());
                    var chids = new XElement("ChPoDd", new XElement("Details",
                              new XAttribute("Ids", Rcp_Id)
                             )).ToString();
                    var a = db.Sp_Update_Pending_To_Receipts(chids, userid);
                }
                else
                {
                    return Json(false);
                }
            }
            else if (Type == "PlotManagement")
            {
                var fileplotdetails = db.Sp_Get_PlotData(FilePlotNo).FirstOrDefault();
                if (fileplotdetails != null)
                {
                    var res2 = db.Sp_Get_PlotLastOwner(fileplotdetails.Id).FirstOrDefault();
                    db.Sp_Update_FilePlotnmbr_Pending_Reciept(res2.Name, res2.Father_Husband, res2.Mobile_1, fileplotdetails.Block_Name, fileplotdetails.Plot_Size, fileplotdetails.Type, Type, fileplotdetails.Id, fileplotdetails.Plot_No, Rcp_Id.ToString());
                    var chids = new XElement("ChPoDd", new XElement("Details",
                               new XAttribute("Ids", Rcp_Id)
                              )).ToString();
                    var a = db.Sp_Update_Pending_To_Receipts(chids, userid);
                }
                else
                {
                    return Json(false);
                }
            }
            else if (Type == "CommercialManagement")
            {
                var fileplotdetails = db.Sp_Get_CommercialData(FilePlotNo).FirstOrDefault();
                if (fileplotdetails != null)
                {
                    var res2 = db.Sp_Get_CommercialLastOwner(fileplotdetails.Id).FirstOrDefault();
                    db.Sp_Update_FilePlotnmbr_Pending_Reciept(res2.Name, res2.Father_Husband, res2.Mobile_1, fileplotdetails.Floor, fileplotdetails.Area.ToString(), fileplotdetails.Type, Type, fileplotdetails.Id, fileplotdetails.shop_no, Rcp_Id.ToString());
                    var chids = new XElement("ChPoDd", new XElement("Details",
                               new XAttribute("Ids", Rcp_Id)
                              )).ToString();
                    var a = db.Sp_Update_Pending_To_Receipts(chids, userid);
                }
                else
                {
                    return Json(false);
                }
            }

            return Json(true);
        }


        [NoDirectAccess]
        public ActionResult ReceiptsDetails(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name", res.Bank);
            ViewBag.PaymentType = new SelectList(Nomenclature.PaymentTypes(), "Value", "Name", res.PaymentType);

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Receipts Details  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ReceiptsDetail(string Bank, string Ch_Pay_Draft_No, DateTime? Ch_Pay_Draft_date, string Description, long ReceiptId, string PaymentType)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Add_Receipt_UpdateRequest(Bank, Ch_Pay_Draft_No, Ch_Pay_Draft_date, userid, Description, ReceiptId, Status.Pending.ToString(), PaymentType).FirstOrDefault();
            return Json(true);
        }
        [NoDirectAccess]
        public ActionResult UpdateRequests()
        {
            var res = db.Sp_Get_ReceiptRequests_Update().ToList();
            return View(res);
        }
        [NoDirectAccess]
        public ActionResult RequestDetails(long Id)
        {
            var res = db.Sp_Get_ReceiptRequests_Update_Details(Id).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name", res.Bank);
            ViewBag.PaymentType = new SelectList(Nomenclature.PaymentTypes(), "Value", "Name", res.PaymentType);
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateReceipts(string Bank, string Ch_Pay_Draft_No, DateTime? Ch_Pay_Draft_date, string Remarks, string Description, long ReceiptId, long RequestId, string PaymentType)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var history = Description + "\n ----- \n" + Remarks;
            db.Sp_Update_ReceiptInfo(Bank, Ch_Pay_Draft_No, Ch_Pay_Draft_date, history, ReceiptId, Status.Approved.ToString(), RequestId, PaymentType);
            //try
            //{
            //    AccountHandlerController de = new AccountHandlerController();
            //    de.Banking_Receipts_Update(ReceiptId, PaymentType, Ch_Pay_Draft_No, Ch_Pay_Draft_date, Bank, Remarks, userid);
            //}
            //catch (Exception ex)
            //{
            //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //}


            return Json(true);
        }
        [NoDirectAccess]
        public ActionResult CancelReq(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).FirstOrDefault();
            var geent = db.General_Entries.Where(x => x.Voucher_No == res.ReceiptNo).FirstOrDefault();
            if (geent == null)
            {
                ViewBag.issupervised = 0;
            }
            else
            {
                ViewBag.issupervised = 1;
            }
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name", res.Bank);
            ViewBag.PaymentType = new SelectList(Nomenclature.PaymentTypes(), "Value", "Name", res.PaymentType);
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CancelRequest(long ReceiptId, string Description)
        {
            var res = db.Sp_Update_Receipt_Req(ReceiptId, Description);
            return Json(true);
        }
        [NoDirectAccess]
        public ActionResult AllCancelRequests()
        {
            var All = db.Sp_Get_Cashiers_List().ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");
            return View();
        }
        [NoDirectAccess]
        public ActionResult AllCancelRequestSearch(DateTime? From, DateTime? To, long?[] Users)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string chids = null;
            if (Users != null)
            {
                chids = new XElement("ChPoDd", Users.Select(x => new XElement("Details",
                                 new XAttribute("Ids", x)
                                 ))).ToString();
            }
            var res = db.Sp_Get_CancelReq(From, To, chids).ToList();
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult CancelRequestDetails(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateCancelReceipts(long ReceiptId, string Description, int Cancel_Reversal, string PaymentType, string PayChqNo, string Bank, string Branch, DateTime? Ch_bk_Pay_Date)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res = db.Sp_Update_ReceiptCancelation(ReceiptId, Description, userid, Cancel_Reversal);
            var rd = db.Sp_Get_Receipt_Parameter_ById(ReceiptId).FirstOrDefault();
            if (rd.Type == ReceiptTypes.ServiceCharges.ToString())
            {
                var newDate = Convert.ToDateTime(rd.DateTime);
                var res1 = db.ServiceCharges_Bill.Where(x => x.Date.Month == newDate.Month && x.Date.Year == newDate.Year).FirstOrDefault();

                db.Sp_Update_UtilitiesReversal(res1.Id, ReceiptTypes.ServiceCharges.ToString(), rd.Amount);


            }
            else if (rd.Type == ReceiptTypes.Electricity_Charges.ToString())
            {
                var newDate = Convert.ToDateTime(rd.DateTime);
                var res1 = db.Electricity_Bill.Where(x => x.Month.Month == newDate.Month && x.Month.Year == newDate.Year).FirstOrDefault();
                db.Sp_Update_UtilitiesReversal(res1.Id, ReceiptTypes.Electricity_Charges.ToString(), rd.Amount);

            }
            if (Cancel_Reversal == 1)
            {
                var recName = db.Receipts.Where(x => x.Id == ReceiptId).Select(x => x.ReceiptNo).FirstOrDefault();
                var geent = db.General_Entries.Where(x => x.Voucher_No == recName).Select(x => x.GroupId).FirstOrDefault();
                bool headcashier = false;
                if (User.IsInRole("Head Cashier"))
                {
                    headcashier = true;
                }
                try
                {
                    AccountHandlerController de = new AccountHandlerController();
                    de.ReverseEntry(geent, userid, headcashier);
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
            }
            if (Cancel_Reversal == 3)
            {
                var r = db.Sp_Add_Voucher("-", rd.Amount, rd.AmountinWords, rd.Bank, rd.Branch_Name, rd.Ch_Pay_Draft_Date, rd.Ch_Pay_Draft_No, rd.Contact, Description,
                            rd.Father_Name, rd.File_Plot_No, rd.Module, rd.Name, rd.PaymentType, "Meher Estate Developers",
                         "", userid, "Payment Voucher", userid, null, comp.Id).FirstOrDefault();
                //bool headcashier = false;
                //if (User.IsInRole("Head Cashier"))
                //{
                //    headcashier = true;
                //}
                //try
                //{
                //    Helpers h = new Helpers();
                //    AccountHandlerController de = new AccountHandlerController();
                //    de.Service_Charges_Utilities_Reversal(rd.Amount, rd.File_Plot_Number, rd.Plot_Type, rd.Block, PaymentType, PayChqNo, Ch_bk_Pay_Date, Bank, "Return Back Amount", h.RandomNumber(), userid, r.Receipt_No, 1, headcashier);
                //}
                //catch (Exception ex)
                //{
                //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                //}

                if (PaymentType != "Cash")
                {
                    if (!Directory.Exists(Server.MapPath("~/Repository/Plot_Cancellation_Vouchers/" + "/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/Plot_Cancellation_Vouchers/"));
                    }
                    string dt = string.Format("{0:d/M/yyyy}", Ch_bk_Pay_Date);
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/Plot_Cancellation_Vouchers/" + "/"), rd.Ch_Pay_Draft_No + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                    Helpers h = new Helpers();
                    //var Images = h.SaveBase64Image(rd.FileImage, pathMain, res.ToString());
                }

                var fres = new { VoucherId = r.Receipt_Id, Token = userid };
                return Json(fres);
            }
            return Json(true);
        }

        //----------------------------------------

        [NoDirectAccess]
        public ActionResult ReceiptDetails(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).FirstOrDefault();
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return PartialView(res);
        }
        public JsonResult AdjustReq(long ReceiptId, string Reason, string PaymentType, string From, string To, long To_Id, string To_Name, string Block, string Module, string FromModule, string ToMod)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Add_ReceiptAdjust(ReceiptId, Reason, PaymentType, userid, From, To, To_Id, To_Name, Block, Module, FromModule, ToMod).FirstOrDefault();
            return Json(res);
        }
        [NoDirectAccess]
        public ActionResult FinancialAdjustList()
        {
            var res = db.Sp_Get_ReceiptAdjustReqs_Finance().ToList();
            return View(res);
        }
        [NoDirectAccess]
        public ActionResult FinanceAdjustmentDetails(long Id)
        {
            var res = db.Sp_Get_ReceiptAdjustReqs_Details(Id).FirstOrDefault();
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult ReceiptAdjustmentDetails(long Id)
        {
            var res = db.Sp_Get_ReceiptAdjustReqs_Details(Id).FirstOrDefault();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Receipts Adjustments Details Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult RecordAdjustment()
        {
            var res = db.Sp_Get_ReceiptAdjustReqs_Records().ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Record Adjustments  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(res);
        }
        public JsonResult UpdateReceiptAdjustment(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_ReceiptAdjustReqs_Details(Id).FirstOrDefault();


            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var rece = db.Receipts.Where(x => x.Id == res.Receipt_Id).SingleOrDefault();
                    if (res.FromModule == Modules.FileManagement.ToString() && res.ToModule == Modules.FileManagement.ToString())
                    {
                        var fileid1 = db.File_Form.Where(x => x.Id == rece.File_Plot_No).FirstOrDefault();
                        var fileid2 = db.File_Form.Where(x => x.Id == res.To_Plot_Id).FirstOrDefault();

                        var lastowner = db.Sp_Get_FileLastOwner(fileid2.Id).ToList();

                        var res2 = db.Sp_Add_FileComments(fileid1.Id, "Receipt: " + res.ReceiptNo + " Adjusted from " + res.From_Plot + " to " + res.To_Plot, userid, "Text");
                        var res3 = db.Sp_Add_FileComments(fileid2.Id, "Receipt: " + res.ReceiptNo + " Adjusted from " + res.From_Plot + " to " + res.To_Plot, userid, "Text");

                        var res1 = db.Sp_Update_ReceiptAdjustment(Id, res.Receipt_Id, res.To_Plot_Id, "Receipt Adjusted from " + res.From_Plot + " this to " + res.To_Plot, userid, res.To_Plot_Id, fileid2.FileFormNumber.ToString(), string.Join(",", lastowner.Select(x => x.Name)), string.Join(",", lastowner.Select(x => x.Father_Husband)), string.Join(",", lastowner.Select(x => x.Mobile_1)), fileid2.Plot_Size).FirstOrDefault();

                        //{
                        //    bool headcashier = false;
                        //    if (User.IsInRole("Head Cashier"))
                        //    {
                        //        headcashier = true;
                        //    }
                        //    try
                        //    {
                        //        Helpers H = new Helpers();
                        //        AccountHandlerController de = new AccountHandlerController();
                        //        de.Adjustment(res.Amount, fileid1.FileFormNumber.ToString(), "Residential", fileid1.Block, fileid2.FileFormNumber.ToString(), "Residential", fileid2.Block, H.RandomNumber(), userid, 1, headcashier);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Transaction.Rollback();
                        //        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        //    }                            
                        //}
                        Transaction.Commit();
                        if (res1 == true)
                        {
                            return Json(res.Receipt_Id);
                        }
                        else
                        {
                            return Json(false);
                        }
                    }
                    else if (res.FromModule == Modules.PlotManagement.ToString() && res.ToModule == Modules.PlotManagement.ToString())
                    {
                        var Plot1 = db.Sp_Get_PlotData(rece.File_Plot_No).FirstOrDefault();
                        var Plot2 = db.Sp_Get_PlotData(res.To_Plot_Id).FirstOrDefault();

                        var lastowner = db.Sp_Get_PlotLastOwner(Plot2.Id).ToList();

                        var res2 = db.Sp_Add_PlotComments(rece.File_Plot_No, "Receipt: " + res.ReceiptNo + " Adjusted from " + res.From_Plot + " to " + res.To_Plot, userid, "Text");
                        var res3 = db.Sp_Add_PlotComments(res.To_Plot_Id, "Receipt: " + res.ReceiptNo + " Adjusted from " + res.From_Plot + " to " + res.To_Plot, userid, "Text");

                        var res1 = db.Sp_Update_ReceiptAdjustment(Id, res.Receipt_Id, res.To_Plot_Id, "Receipt Adjusted from " + res.From_Plot + " this to " + res.To_Plot, userid, res.To_Plot_Id, Plot2.Plot_No.ToString(), string.Join(",", lastowner.Select(x => x.Name)), string.Join(",", lastowner.Select(x => x.Father_Husband)), string.Join(",", lastowner.Select(x => x.Mobile_1)), Plot2.Plot_Size).FirstOrDefault();

                        //{
                        //    bool headcashier = false;
                        //    if (User.IsInRole("Head Cashier"))
                        //    {
                        //        headcashier = true;
                        //    }
                        //    try
                        //    {
                        //        Helpers H = new Helpers();
                        //        AccountHandlerController de = new AccountHandlerController();
                        //        de.Adjustment(res.Amount, Plot1.Plot_No, Plot1.Type, Plot1.Block_Name, Plot2.Plot_No, Plot2.Type, Plot2.Block_Name, H.RandomNumber(), userid, 1, headcashier);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Transaction.Rollback();
                        //        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        //    }                            
                        //}
                        Transaction.Commit();
                        if (res1 == true)
                        {
                            return Json(res.Receipt_Id);
                        }
                        else
                        {
                            return Json(false);
                        }
                    }
                    else if (res.FromModule == Modules.FileManagement.ToString() && res.ToModule == Modules.PlotManagement.ToString())
                    {
                        var FromFile = db.File_Form.Where(x => x.Id == rece.File_Plot_No).FirstOrDefault();
                        var ToPlot = db.Sp_Get_PlotData(res.To_Plot_Id).FirstOrDefault();

                        var lastowner = db.Sp_Get_PlotLastOwner(ToPlot.Id).ToList();

                        var res1 = db.Sp_Update_ReceiptAdjustment(Id, res.Receipt_Id, res.To_Plot_Id, "Receipt Adjusted from " + res.From_Plot + " this to " + res.To_Plot, userid, res.To_Plot_Id, ToPlot.Plot_No.ToString(), string.Join(",", lastowner.Select(x => x.Name)), string.Join(",", lastowner.Select(x => x.Father_Husband)), string.Join(",", lastowner.Select(x => x.Mobile_1)), ToPlot.Plot_Size).FirstOrDefault();


                        var FromComment = db.Sp_Add_FileComments(FromFile.Id, "Receipt: " + res.ReceiptNo + " Adjusted from " + res.From_Plot + " to " + res.To_Plot, userid, "Text");
                        var ToComment = db.Sp_Add_PlotComments(res.To_Plot_Id, "Receipt: " + res.ReceiptNo + " Adjusted from " + res.From_Plot + " to " + res.To_Plot, userid, "Text");

                        //{
                        //    bool headcashier = false;
                        //    if (User.IsInRole("Head Cashier"))
                        //    {
                        //        headcashier = true;
                        //    }
                        //    try
                        //    {
                        //        Helpers H = new Helpers();
                        //        AccountHandlerController de = new AccountHandlerController();
                        //        de.Adjustment(res.Amount, FromFile.FileFormNumber.ToString(), FromFile.Type, FromFile.Block, ToPlot.Plot_No, ToPlot.Type, ToPlot.Block_Name, H.RandomNumber(), userid, 1, headcashier);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Transaction.Rollback();
                        //        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        //    }
                        //}
                        Transaction.Commit();
                        if (res1 == true)
                        {
                            return Json(res.Receipt_Id);
                        }
                        else
                        {
                            return Json(false);
                        }
                    }
                    else if (res.FromModule == Modules.PlotManagement.ToString() && res.ToModule == Modules.FileManagement.ToString())
                    {
                        var FromPlot = db.Sp_Get_PlotData(rece.File_Plot_No).FirstOrDefault();
                        var ToFile = db.File_Form.Where(x => x.Id == res.To_Plot_Id).FirstOrDefault();

                        var lastowner = db.Sp_Get_FileLastOwner(ToFile.Id).ToList();

                        var res1 = db.Sp_Update_ReceiptAdjustment(Id, res.Receipt_Id, res.To_Plot_Id, "Receipt Adjusted from " + res.From_Plot + " this to " + res.To_Plot, userid, res.To_Plot_Id, ToFile.FileFormNumber.ToString(), string.Join(",", lastowner.Select(x => x.Name)), string.Join(",", lastowner.Select(x => x.Father_Husband)), string.Join(",", lastowner.Select(x => x.Mobile_1)), ToFile.Plot_Size).FirstOrDefault();

                        var FromComment = db.Sp_Add_PlotComments(rece.File_Plot_No, "Receipt: " + res.ReceiptNo + " Adjusted from " + res.From_Plot + " to " + res.To_Plot, userid, "Text");
                        var ToComment = db.Sp_Add_FileComments(res.To_Plot_Id, "Receipt: " + res.ReceiptNo + " Adjusted from " + res.From_Plot + " to " + res.To_Plot, userid, "Text");

                        //{
                        //    bool headcashier = false;
                        //    if (User.IsInRole("Head Cashier"))
                        //    {
                        //        headcashier = true;
                        //    }
                        //    try
                        //    {
                        //        Helpers H = new Helpers();
                        //        AccountHandlerController de = new AccountHandlerController();
                        //        de.Adjustment(res.Amount, FromFile.FileFormNumber.ToString(), FromFile.Type, FromFile.Block, ToPlot.Plot_No, ToPlot.Type, ToPlot.Block_Name, H.RandomNumber(), userid, 1, headcashier);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Transaction.Rollback();
                        //        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        //    }
                        //}
                        Transaction.Commit();
                        if (res1 == true)
                        {
                            return Json(res.Receipt_Id);
                        }
                        else
                        {
                            return Json(false);
                        }
                    }

                    else
                    {
                        return Json(false);
                    }

                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(false);
                }
            }
        }

        public JsonResult FinanceUpdateReceiptAdjustment(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_ReceiptAdjustReqs_Details(Id).FirstOrDefault();
            //using (var Transaction = db.Database.BeginTransaction())
            //{
            //try
            //{
            var res1 = db.Sp_Update_FinanceReceiptAdjustment(Id, userid).FirstOrDefault();
            //Transaction.Commit();
            return Json(res1);
            //}
            //catch (Exception e)
            //{
            //Transaction.Rollback();
            //return Json(false);
            //}
            //}
        }

        //-----------------------------------
        [NoDirectAccess]
        public ActionResult RefundAmount(long Plot_Id, string Module)
        {
            if (Module == Modules.PlotManagement.ToString())
            {
                var res1 = db.Sp_Get_PlotData(Plot_Id).SingleOrDefault();
                var res3 = db.Sp_Get_PlotInstallments(Plot_Id).ToList();
                var res4 = db.Sp_Get_ReceivedAmounts(Plot_Id, Modules.PlotManagement.ToString()).ToList();

                var res = new PlotDetailData { PlotData = res1, PlotOwners = null, PlotInstallments = res3, PlotReceipts = res4 };
                return PartialView(res);
            }
            else
            {
                return PartialView();
            }
        }
        public JsonResult RefundRequest(decimal Refund_Amt, string Module, long Plot_Id)
        {
            var res3 = db.Sp_Get_PlotInstallments(Plot_Id).Sum(x => x.Amount);
            var res4 = db.Sp_Get_ReceivedAmounts(Plot_Id, Modules.PlotManagement.ToString()).Sum(x => x.Amount);
            Refund_Amounts r = new Refund_Amounts()
            {
                Module = Module,
                Plot_Id = Plot_Id,
                Refund_Amt = Refund_Amt,
                Received_Amt = res4,
                Total_Amt = res3
            };
            db.Refund_Amounts.Add(r);
            db.SaveChanges();
            return Json(true);
        }

        /////////////////////////////////////
        [NoDirectAccess]
        public ActionResult MarketingReceipts()
        {
            var All = db.Sp_Get_Cashiers_List().ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");
            return View();
        }
        [NoDirectAccess]
        public ActionResult MarketingReceiptSearchResult(DateTime? From, DateTime? To, long?[] Users)
        {
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
            var res = db.Sp_Get_DailySAMRecovery_Report_Parameter(From, To, chids).Select(x => new RecoveryReport
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Cashier_Name = x.Name,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                DateTime = x.DateTime,
                File_Plot_No = x.File_Plot_Number,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                Size = x.Size,
                Type = x.Type,
                Block = x.Block,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Contact = x.Contact,
                ReceiptNo = x.ReceiptNo,
                Module = x.Module,
                Id = x.Id,
                Cancel = x.Cancel,
                LeadDeal = x.LeadDeal
            }).ToList();
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult MarketingReceiptsDetails(long Id, string LeadDeal)
        {
            var res = db.Sp_Get_MarketingReceipt_Parameter_ById(Id, LeadDeal).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name", res.Bank);
            ViewBag.PaymentType = new SelectList(Nomenclature.PaymentTypes(), "Value", "Name", res.PaymentType);

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Deatil Page of Receipt " + LeadDeal, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MarketingReceiptsDetail(string Bank, string Ch_Pay_Draft_No, DateTime? Ch_Pay_Draft_date, string Description, long ReceiptId, string PaymentType)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Add_MarketingReceipt_UpdateRequest(Bank, Ch_Pay_Draft_No, Ch_Pay_Draft_date, userid, Description, ReceiptId, Status.Pending.ToString(), PaymentType).FirstOrDefault();
            return Json(true);
        }
        [NoDirectAccess]
        public ActionResult MarketingUpdateRequests()
        {
            var res = db.Sp_Get_MarketingReceiptRequests_Update().ToList();
            return View(res);
        }
        [NoDirectAccess]
        public ActionResult MarketingRequestDetails(long Id, string LeadDeal)
        {
            var res = db.Sp_Get_MarketingReceiptRequests_Update_Details(Id, LeadDeal).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name", res.Bank);
            ViewBag.LeadDeal = LeadDeal;
            ViewBag.PaymentType = new SelectList(Nomenclature.PaymentTypes(), "Value", "Name", res.PaymentType);
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MarketingUpdateReceipts(string Bank, string Ch_Pay_Draft_No, DateTime? Ch_Pay_Draft_date, string Remarks, string Description, long ReceiptId, long RequestId, string PaymentType, string LeadDeal)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var history = Description + "\n ----- \n" + Remarks;
            db.Sp_Update_MarketingReceiptInfo(Bank, Ch_Pay_Draft_No, Ch_Pay_Draft_date, history, ReceiptId, Status.Approved.ToString(), RequestId, PaymentType, LeadDeal);
            return Json(true);
        }
        [NoDirectAccess]
        public ActionResult MarketingCancelReq(long Id, string LeadDeal)
        {
            var res = db.Sp_Get_MarketingReceipt_Parameter_ById(Id, LeadDeal).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name", res.Bank);
            ViewBag.PaymentType = new SelectList(Nomenclature.PaymentTypes(), "Value", "Name", res.PaymentType);
            ViewBag.LeadDeal = LeadDeal;
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MarketingCancelRequest(long ReceiptId, string Description, string LeadDeal)
        {
            var res = db.Sp_Update_MarketingReceipt_Req(ReceiptId, Description, LeadDeal);
            return Json(true);
        }
        [NoDirectAccess]
        public ActionResult MarketingAllCancelRequests()
        {
            var All = db.Sp_Get_Cashiers_List().ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");
            return View();
        }
        [NoDirectAccess]
        public ActionResult MarketingAllCancelRequestSearch(DateTime? From, DateTime? To, long?[] Users)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string chids = null;
            if (Users != null)
            {
                chids = new XElement("ChPoDd", Users.Select(x => new XElement("Details",
                                 new XAttribute("Ids", x)
                                 ))).ToString();
            }
            var res = db.Sp_Get_MarketingCancelReq(From, To, chids).ToList();
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult MarketingCancelRequestDetails(long Id, string LeadDeal)
        {
            var res = db.Sp_Get_MarketingReceipt_Parameter_ById(Id, LeadDeal).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.LeadDeal = LeadDeal;
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MarketingUpdateCancelReceipts(long ReceiptId, string Description, int Cancel_Reversal, string PaymentType, string PayChqNo, string Bank, string Branch, DateTime? Ch_bk_Pay_Date, string LeadDeal)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Update_MarketingReceiptCancelation(ReceiptId, Description, userid, Cancel_Reversal, LeadDeal);
            return Json(true);
        }
        [NoDirectAccess]
        public ActionResult AllRequestedPayments()
        {
            var res = db.VoucherRequests.ToList();
            return View(res);
        }
        public JsonResult ReleasePayment(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var rd = db.VoucherRequests.Where(x => x.Id == Id).FirstOrDefault();
            var r = db.Sp_Add_Voucher(rd.Voucher_For, rd.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(rd.Amount)), null, null, null, null, rd.CNIC, rd.Remarks,
                            rd.Father_Name, rd.Module_Id, rd.Module, rd.Name, "Cash", "Meher Estate Developers",
                         "", userid, "Payment Voucher", userid, null, comp.Id).FirstOrDefault();
            {
                bool headcashier = false;
                if (User.IsInRole("Head Cashier"))
                {
                    headcashier = true;
                }
                try
                {
                    AccountHandlerController de = new AccountHandlerController();
                    de.Finance_Release_Voucher_Payments(rd.Amount, rd.Voucher_For, "Cash", "", null, "", "Payment released against", new Helpers().RandomNumber(), userid, r.Receipt_No, 1, headcashier);
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
            }
            var fres = new { VoucherId = r.Receipt_Id, Token = userid };
            return Json(fres);
        }
        // Patty cash Settelments
        [NoDirectAccess]
        public ActionResult PattyCashSattlment()
        {
            Helpers h = new Helpers();
            ViewBag.Transaction_Id = h.RandomNumber();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Patty Cash Sattlement Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        public JsonResult PattyCashEntry(List<PattyCash_Items> items, long Transaction_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string GeneralEnt = new XElement("PattyCash", items.Select(x => new XElement("Items",
           new XAttribute("Amount", x.Amount),
           new XAttribute("Date", (x.Date == null) ? DateTime.Now : x.Date),
           new XAttribute("Description", x.Description),
           (x.IsFood == null) ? null : new XAttribute("IsFood", x.IsFood)
           ))).ToString();

            var res = db.Sp_Add_PattyCashItems(GeneralEnt, Transaction_Id, userid).FirstOrDefault();
            if (res == true)
            {
                var ret = new { Status = true, Msg = "Submitted Successfully" };
                return Json(ret);
            }
            else
            {
                var ret = new { Status = false, Msg = "Already Submitted" };
                return Json(ret);
            }
        }
        [NoDirectAccess]
        public ActionResult AllPattyCash()
        {
            return View();
        }
        [NoDirectAccess]
        public ActionResult PattyCashStatus(string Status)
        {
            long? userid = long.Parse(User.Identity.GetUserId());
            var emp = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
            List<long> uids = new List<long>();

            var res1 = db.Sp_Get_Reporting_Employees(emp).Where(x => x.loginid != null).Select(x => x.loginid).ToList();

            var ids = new XElement("Deps", res1.Select(x => new XElement("Ids",
            new XAttribute("Id", x)
            ))).ToString();
            var res = db.Sp_Get_PattyCashItems_List(Status, ids, User.IsInRole("Administrator")).ToList();
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult MyPattyCashEntry()
        {
            return View();
        }
        [NoDirectAccess]
        public ActionResult MyPattyCashList(long Userid, string Status)
        {
            var res = db.Sp_Get_PattyCashItems_User(Userid, Status).ToList();
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult PattyCashEntryDetails(long Group_Id)
        {
            var res = db.Sp_Get_PattyCashItems(Group_Id).ToList();
            return View(res);
        }
        public JsonResult ApprovePTC(long Group_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Update_PattyCashItems_Approval(userid, Group_Id);
            var ret = new { Status = true, Msg = "Approved" };
            return Json(ret);
        }
        [NoDirectAccess]
        public ActionResult PettyCashPrint(long Group_Id)
        {
            var res = db.Sp_Get_PattyCashItems(Group_Id).ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Printed Patty Cash Page " + res.Select(x => x.Description).FirstOrDefault(), "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Group_Id);
            return View(res);
        }
        [NoDirectAccess]
        public ActionResult PettyCashAccount()
        {
            var res = db.PattyCash_Accounts.Where(x => x.Status == "Active").ToList();
            List<PettyCashAccountsDetModel> pcad = new List<PettyCashAccountsDetModel>();
            foreach (var v in res)
            {

                decimal? balance = 0;
                if (v.Account_HeadId != null)
                {
                    var a = GetFinancialYear();
                    var acc = GetHead(Convert.ToInt32(v.Account_HeadId));
                    var bal = db.Sp_Get_Head_Balance(acc.Text_ChartCode, a.Start, a.End).FirstOrDefault();
                    balance = (bal.Opening_Balance_Debit + bal.Credit) - bal.Credit;
                }

                PettyCashAccountsDetModel pca = new PettyCashAccountsDetModel();
                pca.Id = v.Id;
                pca.Employee_Id = v.Employee_Id;
                pca.Employee_Code = v.Employee_Code;
                pca.Employee_Name = v.Employee_Name;
                pca.Cash_Limit = v.Cash_Limit;
                pca.Item_Limit = v.Item_Limit;
                pca.Recorded_On = v.Recorded_On;
                pca.Balance = balance;
                pcad.Add(pca);
            }
            return PartialView(pcad);
        }
        public JsonResult DeletePTC(long Group_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Delete_PattyCash(userid, Group_Id);
            var ret = new Return { Status = true, Msg = "Deleted" };
            return Json(ret);
        }

        [NoDirectAccess]
        public ActionResult NewPattyCashAccount()
        {
            return PartialView();
        }
        public Sp_Get_CA_Head_Parameter_Result GetHead(int Id)
        {
            var res = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
            return res;
        }
        public JsonResult AddNewPattyCashAccount(string empName, long empId, decimal cashLimit, decimal itemLimit)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string[] words = empName.Split('(', ')');
            string name = "";
            string empcode = "";
            if (words.Any())
            {
                name = words[0];
                empcode = words[1];
            }
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res = db.Sp_Add_PattyCash_Account(name, empId, empcode, cashLimit, itemLimit, userid).FirstOrDefault();
                    if (res != -1)
                    {
                        AccountHandlerController de = new AccountHandlerController();
                        de.PattyCashAccount(res, name, userid);
                        Transaction.Commit();
                        return Json(new { Status = true, Msg = "Patty Cash Account Successfully Created." });
                    }
                    else
                    {
                        return Json(new { Status = false, Msg = "Patty Cash Account Already Created." });
                    }
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }

                return Json(true);
            }

        }
        public JsonResult EmployeeSearch(string s)
        {
            var result = db.Sp_Get_EmployeeList_ByParam("", s, string.Empty).Select(x => new { id = x.Id, text = x.Name + "(" + x.Employee_Code + ")" }).Distinct().ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PattyCashEmployeeSearch(string s)
        {
            var result = db.Sp_Get_PattyCash_EmployeeList("", s, string.Empty).Select(x => new { id = x.Id, text = x.Name + "(" + x.Employee_Code + ")" }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [NoDirectAccess]
        public ActionResult PattyCashSettlements()
        {
            return PartialView();
        }
        [NoDirectAccess]
        public ActionResult PattyCashSupervisionList()
        {
            long? userid = long.Parse(User.Identity.GetUserId());
            var emp = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
            List<long> uids = new List<long>();

            var res1 = db.Sp_Get_Reporting_Employees(emp).Where(x => x.loginid != null).Select(x => x.loginid).ToList();

            var ids = new XElement("Deps", res1.Select(x => new XElement("Ids",
            new XAttribute("Id", x)
            ))).ToString();
            bool bit = false;
            if (User.IsInRole("Supervise Patty Cash") || User.IsInRole("Administrator"))
            {
                bit = true;
            }
            var res = db.Sp_Get_PattyCashItems_List("Approved", ids, bit).ToList();
            return PartialView(res);
        }
        [NoDirectAccess]
        public ActionResult PattyCashSupervisionDetails(long Group_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            var res = db.Sp_Get_PattyCashItems(Group_Id).ToList();

            return View(res);
        }
        public JsonResult UpdatePattyCashAccountHead(long empId, int headid)
        {
            db.Sp_Update_Patty_Cash_Head(empId, headid);
            return Json(true);
        }
        //public JsonResult UpdatePattyCashExpense(long empid, int headid)
        //{
        //    db.Sp_Update_PattyCash_ExpenseAccount(empid, headid);
        //    return Json(true);
        //}
        [NoDirectAccess]
        public ActionResult PayPattyCash()
        {
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return View();
        }
        public JsonResult GetPattyCashBalance(int empId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.PattyCash_Accounts.Where(x => x.Employee_Id == empId).FirstOrDefault();
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);

            var res2 = db.COA_Mapper.Where(x =>
                        x.CompanyId == comp.Id &&
                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                        x.Module == COA_Mapper_Modules.Petty_Cash.ToString() &&
                        x.ModuleType == COA_Mapper_ModuleTypes.Petty_Cash_Account.ToString() &&
                        x.Module_Id == res1.Id
                        ).FirstOrDefault();

            var account = ah.GetHead(Convert.ToInt32(res2.AccountId));
            var a = GetFinancialYear();
            var res = db.Sp_Get_Head_Balance(account.Text_ChartCode, a.Start, a.End).FirstOrDefault();
            decimal? bal = (res.Opening_Balance_Credit - res.Opening_Balance_Debit) + (res.Debit == null ? 0 : res.Debit) - (res.Credit == null ? 0 : res.Credit);

            return Json(new PattyCashAmountsModel { Cashlimit = res1.Cash_Limit, Remaining = bal });
        }
        private FinancialYear GetFinancialYear()
        {
            return new FinancialYear { Start = new DateTime(2022, 7, 1), End = new DateTime(2023, 6, 30) };
        }
        public JsonResult AddNewPattyCashEntry(int payeeId, int payaccId, string desc, string receivedby, decimal amount, string paytype, string instNo, DateTime? instDate, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var amtinwords = GeneralMethods.NumberToWords(Convert.ToInt32(amount));

            AccountHandlerController de = new AccountHandlerController();
            var comp = de.Company_Attr(userid);


            var paymentaccount = de.GetHead(payaccId);
            paytype = paymentaccount.Head;

            bool headcashier = false;
            if (User.IsInRole("Head Cashier"))
            {
                headcashier = true;
            }

            var r = db.Sp_Add_Voucher("", amount, amtinwords, paymentaccount.Head, "", instDate, instNo, "", desc, "", null, Modules.PattyCash_Management.ToString(), receivedby,
                paytype, "Meher Estate Developers", desc, TransactionId, "Payment Voucher", userid, null, comp.Id).FirstOrDefault();
            try
            {
                de.PattyCashPaymentReleaseEntry(desc, receivedby, payeeId, payaccId, amount, TransactionId, r.Receipt_No, instNo, instDate, userid, headcashier);
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            }

            return Json(new { VoucherId = r.Receipt_Id, Token = TransactionId });
        }
        ////public JsonResult AddNewPattyCashEntry(int payeeId, int payaccId, string desc, string receivedby, decimal amount, string paytype, string instNo, DateTime? instDate)
        ////{

        ////    long userid = long.Parse(User.Identity.GetUserId());
        ////    var TransactionId = new Helpers().RandomNumber();
        ////    var amtinwords = GeneralMethods.NumberToWords(Convert.ToInt32(amount));

        ////    AccountHandlerController de = new AccountHandlerController();
        ////    var comp = de.Company_Attr(userid);

        ////    var paymentaccount = de.GetHead(payaccId);
        ////    if (paymentaccount.Head == "Cash")
        ////    {
        ////        paytype = paymentaccount.Head;
        ////    }
        ////    bool headcashier = false;
        ////    if (User.IsInRole("Head Cashier"))
        ////    {
        ////        headcashier = true;
        ////    }

        ////    var r = db.Sp_Add_Voucher("", amount, amtinwords, paymentaccount.Head, "", instDate, instNo, "", desc, "", null, Modules.PattyCash_Management.ToString(), receivedby,
        ////        paytype, "Meher Estate Developers", desc, TransactionId, "Payment Voucher", userid, null, t).FirstOrDefault();
        ////    try
        ////    {
        ////        de.PattyCashPaymentReleaseEntry(desc, receivedby, payeeId, payaccId, amount, TransactionId, r.Receipt_No, instNo, instDate, userid, headcashier);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        ////    }

        ////    return Json(new { VoucherId = r.Receipt_Id, Token = TransactionId });
        ////    if (User.IsInRole("Head Cashier"))
        ////    {
        ////        var JournalEnt = new XElement("JournalEntries", new XElement("Entries",
        ////            new XAttribute("Naration", desc + " - Received By: " + receivedby),
        ////            new XAttribute("Head", payeeaccount.Text_ChartCode + " - " + payeeaccount.Head),
        ////            new XAttribute("Head_Name", payeeaccount.Head),
        ////            new XAttribute("Head_Code", payeeaccount.Text_ChartCode),
        ////            new XAttribute("Head_Id", payeeaccount.Id),
        ////            new XAttribute("Line", 1),
        ////            new XAttribute("Qty", 0),
        ////            new XAttribute("UOM", ""),
        ////            new XAttribute("Rate", 0),
        ////            new XAttribute("Debit", amount),
        ////            new XAttribute("Credit", 0),
        ////            new XAttribute("GroupId", TransactionId)
        ////            ));
        ////        var GEapp = new XElement("JournalEntries", new XElement("Entries",
        ////        new XAttribute("Naration", desc + " - Received By: " + receivedby),
        ////        new XAttribute("Head", paymentaccount.Text_ChartCode + " - " + paymentaccount.Head),
        ////        new XAttribute("Head_Name", paymentaccount.Head),
        ////        new XAttribute("Head_Code", paymentaccount.Text_ChartCode),
        ////        new XAttribute("Head_Id", paymentaccount.Id),
        ////        new XAttribute("Line", 2),
        ////        new XAttribute("Qty", 0),
        ////        new XAttribute("UOM", ""),
        ////        new XAttribute("Rate", 0),
        ////        new XAttribute("Debit", 0),
        ////        new XAttribute("Credit", amount),
        ////        new XAttribute("GroupId", TransactionId)
        ////        ));
        ////        JournalEnt.Add(
        ////            from ge in GEapp.Elements()
        ////            select ge
        ////            );


        ////        var res1 = db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();
        ////        if (res1 == true)
        ////        {
        ////            return Json(new { Status = true, Msg = "Transaction Successful" });
        ////        }
        ////        else
        ////        {
        ////            return Json(new { Status = false, Msg = "Transaction Unsuccessful" });
        ////        }
        ////    }
        ////    else
        ////    {
        ////        var GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
        ////            new XAttribute("Naration", desc + " - Received By: " + receivedby),
        ////            new XAttribute("Head", payeeaccount.Text_ChartCode + " - " + payeeaccount.Head),
        ////            new XAttribute("Head_Name", payeeaccount.Head),
        ////            new XAttribute("Head_Code", payeeaccount.Text_ChartCode),
        ////            new XAttribute("Head_Id", payeeaccount.Id),
        ////            new XAttribute("Line", 1),
        ////            new XAttribute("Qty", 0),
        ////            new XAttribute("UOM", ""),
        ////            new XAttribute("Rate", 0),
        ////            new XAttribute("Debit", amount),
        ////            new XAttribute("Credit", 0),
        ////            new XAttribute("GroupId", TransactionId)
        ////            ));
        ////        var GEapp = new XElement("GeneralEntries", new XElement("Entries",
        ////        new XAttribute("Naration", desc + " - Received By: " + receivedby),
        ////        new XAttribute("Head", paymentaccount.Text_ChartCode + " - " + paymentaccount.Head),
        ////        new XAttribute("Head_Name", paymentaccount.Head),
        ////        new XAttribute("Head_Code", paymentaccount.Text_ChartCode),
        ////        new XAttribute("Head_Id", paymentaccount.Id),
        ////        new XAttribute("Line", 2),
        ////        new XAttribute("Qty", 0),
        ////        new XAttribute("UOM", ""),
        ////        new XAttribute("Rate", 0),
        ////        new XAttribute("Debit", 0),
        ////        new XAttribute("Credit", amount),
        ////        new XAttribute("GroupId", TransactionId)
        ////        ));
        ////        GeneralEnt.Add(
        ////            from ge in GEapp.Elements()
        ////            select ge
        ////            );

        ////        var res1 = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
        ////        if (res1 == true)
        ////        {
        ////            return Json(new { Status = true, Msg = "Transaction Successful" });
        ////        }
        ////        else
        ////        {
        ////            return Json(new { Status = false, Msg = "Transaction Unsuccessful" });
        ////        }
        ////    }
        ////}
        public JsonResult RecordSettlements(List<Voucher_Details_General_Entries> Details, long grpId)
        {
            var TransactionId = new Helpers().RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());

            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            if (User.IsInRole("Head Cashier"))
            {
                string JournalEnt = new XElement("JournalEntries", Details.Select(x => new XElement("Entries",
                new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
                new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
                new XAttribute("Head_Name", this.GetHead(x.Account_Head_Id).Head),
                new XAttribute("Head_Code", this.GetHead(x.Account_Head_Id).Text_ChartCode),
                new XAttribute("Head_Id", x.Account_Head_Id),
                new XAttribute("Line", x.Line),
                new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
                new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
                new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
                new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
                new XAttribute("Comp_Id", comp.Id),
                new XAttribute("GroupId", TransactionId)
                ))).ToString();

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res = db.Sp_Add_Journal_Entries(JournalEnt, userid).FirstOrDefault();
                        db.Sp_Update_PattyCashItems_Paid(userid, grpId);

                        db.Sp_Add_Activity(userid, "Added New Voucher Details in Journal Entry ", "Create", "Activity_Record", ActivityType.General_Entry.ToString(), TransactionId);
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
                string GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
                            new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
                            new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
                            new XAttribute("Head_Name", this.GetHead(x.Account_Head_Id).Head),
                            new XAttribute("Head_Code", this.GetHead(x.Account_Head_Id).Text_ChartCode),
                            new XAttribute("Head_Id", x.Account_Head_Id),
                            new XAttribute("Line", x.Line),
                            new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
                            new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
                            new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
                            new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
                            new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
                            new XAttribute("Comp_Id", comp.Id),
                            new XAttribute("GroupId", TransactionId)
                            ))).ToString();
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res = db.Sp_Add_General_Entries(GeneralEnt, userid).FirstOrDefault();
                        db.Sp_Update_PattyCashItems_Paid(userid, grpId);
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
        }

        [NoDirectAccess]
        public ActionResult TaxPayment()
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
        public JsonResult HeadBalance(int HeadId)
        {
            AccountHandlerController ah = new AccountHandlerController();
            long userid = long.Parse(User.Identity.GetUserId());

            var comp = ah.Company_Attr(userid);

            var res1 = this.GetFinancialYear();
            var res3 = ah.GetHead(HeadId);
            var res4 = db.Sp_Get_Head_OpeningClosing_Balance(res3.Text_ChartCode, "Opening", res1.Start, res1.End).FirstOrDefault();
            var res5 = db.Sp_Get_Head_Balance(res3.Text_ChartCode, res1.Start, res1.End).FirstOrDefault();
            return Json(new { Balance = (res4 == null) ? res5.Credit - res5.Debit : res4.Balance + res5.Credit - res5.Debit });
        }

        public JsonResult TaxPayments(int Head_Id, int payAccId, decimal Amount, long TransactionId, string instNo, DateTime? instDate, string branch, string payType, string Description, string Img)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var user = db.Users.Find(userid);
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var debitAcc = GetHead(Head_Id);

            if (payAccId == 0)
            {
                payType = "Cash";
                PaymentTypeModel credAccount = ah.Payment_Head(payType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var r = db.Sp_Add_Voucher_Vendor("-", Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), null, null, instDate, instNo, "", Description, "", null, Modules.Accounts_Management.ToString(), "",
                            payType, "Meher Estate Developers", "", TransactionId, "Tax Payment Voucher", userid, debitAcc.Type_Id, TransactionId, user.Name, userid, DateTime.Now, "", null, null, TransactionId).FirstOrDefault();

                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, Description, TransactionId, 1, userid, userid, null, null, instNo, null, instDate, null, "", null, r.Receipt_No.ToString(), credAccount.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, credAccount.PaymentData.Head, credAccount.PaymentData.Id, credAccount.PaymentData.Text_ChartCode, credAccount.PaymentData.Head, Description, TransactionId, 1, userid, userid, null, null, instNo, "", instDate, null, "", null, r.Receipt_No.ToString(), credAccount.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit

                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        }
                        Transaction.Commit();
                        return Json(new { Status = true, Msg = "Payment Voucher Successful.", VoucherId = r.Receipt_Id, Token = TransactionId });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(new { Status = false, Msg = "Error Occured" });
                    }
                }



            }
            else
            {
                var credAccount = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.PDC_Payable.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
                var bank = db.Bank_Accounts.Where(x => x.Id == payAccId).FirstOrDefault();

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var r = db.Sp_Add_Voucher_Vendor("-", Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), bank.Bank, "", instDate, instNo, "", Description, "", null, Modules.Accounts_Management.ToString(), "",
                            payType, "Meher Estate Developers", "", TransactionId, "Payment Voucher", userid, debitAcc.Type_Id, TransactionId, user.Name, userid, DateTime.Now, "", null, null, TransactionId).FirstOrDefault();
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, Description, TransactionId, 1, userid, userid, null, null, instNo, null, instDate, null, "", null, r.Receipt_No.ToString(), "BPV", comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, credAccount.Head, credAccount.Id, credAccount.Text_ChartCode, credAccount.Head, Description, TransactionId, 1, userid, userid, null, null, instNo, "", instDate, null, "", null, r.Receipt_No.ToString(), "BPV", comp.Id).FirstOrDefault();   /// For Cash and Bank Credit

                            var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(Amount, bank.Bank, null, payType, bank.Bank, bank.Account_Number, PaymentMethodStatuses.Pending.ToString(),
                                    Modules.Vendor_Payment.ToString(), "Payment Voucher", userid, instNo, debitAcc.Type_Id, instDate, debitAcc.Head, r.Receipt_Id, comp.Id, Voucher_Type.BPV.ToString()).FirstOrDefault());

                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        }
                        Transaction.Commit();
                        return Json(new { Status = true, Msg = "Payment Voucher Successful.", VoucherId = r.Receipt_Id, Token = TransactionId });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(new { Status = false, Msg = "Error Occured" });
                    }
                }

            }

        }


    }
}