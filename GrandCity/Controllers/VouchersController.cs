using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using MeherEstateDevelopers.Filters;
using System.Xml.Linq;
using System.IO;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class VouchersController : Controller
    {
        // GET: Vouchers
        private Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult Voucher(long? GroupId)
        {
            var res = db.Sp_Get_JournalEntries_Parameter_All(GroupId).Select(x => new JournalEntries_Parameter
            {
                Credit = x.Credit,
                Debit = x.Debit,
                GroupId = GroupId,
                Head_Code = x.Head_Code,
                Head_Id = x.Head_Id,
                Head_Name = x.Head_Name,
                Id = x.Id,
                Inst_Bank = x.Inst_Bank,
                Inst_Date = x.Inst_Date,
                Inst_No = x.Inst_No,
                Naration = x.Naration,
                Qty = x.Qty,
                Rate = x.Rate,
                RecordedBy_Date = x.RecordedBy_Date,
                RecordedBy_Name = x.RecordedBy_Name,
                Supviseby_Date = x.Supviseby_Date,
                Supviseby_Name = x.Supviseby_Name,
                UOM = x.UOM,
                Voucher_No = x.Voucher_No,
                Voucher_Type = x.Voucher_Type,
            }).ToList();
            var res2 = db.Vouchers.Where(x => x.Transaction_Id == GroupId).FirstOrDefault();
            var res4 = db.Vouchers.Where(x => x.Group_Id == res2.Group_Id).ToList();
            if (!res.Any())
            {
                var res1 = db.Sp_Get_GeneralEntries_Parameter(GroupId).Select(x => new JournalEntries_Parameter
                {
                    Credit = x.Credit,
                    Debit = x.Debit,
                    GroupId = GroupId,
                    Head_Code = x.Head_Code,
                    Head_Id = x.Head_Id,
                    Head_Name = x.Head_Name,
                    Id = x.Id,
                    Inst_Bank = x.Inst_Bank,
                    Inst_Date = x.Inst_Date,
                    Inst_No = x.Inst_No,
                    Naration = x.Naration,
                    Qty = x.Qty,
                    Rate = x.Rate,
                    RecordedBy_Date = x.Date,
                    RecordedBy_Name = x.Rec_Name,
                    Supviseby_Date = x.Sup_Date,
                    Supviseby_Name = x.Sup_Name,
                    UOM = x.UOM,
                    Voucher_No = x.Voucher_No,
                    Voucher_Type = x.Voucher_Type,

                }).ToList();
                var res5 = new JournalEntry { JE = res, PV = res2 };
                return View(res5);
            }
            var res3 = new JournalEntry { JE = res, PV = res2, Previous_Vouchers = res4 };
            return View(res3);
        }
        public ActionResult SAM_Voucher(long Id, long Token)
        {
            var res = db.Sp_Get_Voucher(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult SPE_Voucher(long Id, long Token)
        {
            var res = db.Sp_Get_Voucher(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult PropertyDeal_Voucher(long Id, long Token)
        {
            var res = db.Sp_Get_PropertyDeal_Voucher(Id, Token).SingleOrDefault();
            return View(res);
        }
        public ActionResult SAGVouchers_Vendor(long Id, long Token)
        {
            var res1 = db.Vouchers.SingleOrDefault(x => x.Id == Id && x.TokenParameter == Token);
            var res2 = db.Voucher_Details.Where(x => x.Voucher_Id == Id).ToList();
            var username = db.Users.Where(x => x.Id == res1.Userid).Select(x => x.Name).FirstOrDefault();
            res1.Pre_Name = username;
            var res = new VendorVoucher { Voucher = res1, Details = res2 };
            return View(res);
        }
        public ActionResult SAGVouchers_Payable(long Id, long Token)
        {
            var res1 = db.Vouchers.SingleOrDefault(x => x.Id == Id && x.TokenParameter == Token);
            var res2 = db.Bills.Where(x => x.GroupId == Token).ToList();
            var res3 = db.Payables.Where(x => x.GroupId == Token).FirstOrDefault();
            var res4 = db.Vouchers.Where(x => x.TokenParameter == Token && x.Id != Id).ToList();
            var res = new VendorPayable { Voucher = res1, Details = res2, paydet = res3, VoucherList = res4 };
            return View(res);
        }
        public ActionResult SAGVouchers(long Id, long Token)
        {
            var res = db.Vouchers.SingleOrDefault(x => x.Id == Id && x.TokenParameter == Token);
            return View(res);
        }
        public ActionResult SAGVoucher(long Id)
        {
            var res1 = db.Vouchers.SingleOrDefault(x => x.Id == Id);
            var res2 = db.Voucher_Details.Where(x => x.Voucher_Id == Id).ToList();
            var res = new VendorVoucher { Voucher = res1, Details = res2 };
            return View("SAGVouchers_Vendor", res);
        }
        public ActionResult SAGLoanAdvanceVouchers(long Id, long Token)
        {
            var res = db.Vouchers.SingleOrDefault(x => x.Id == Id && x.TokenParameter == Token);
            return View(res);
        }
        public ActionResult Create_Voucher(long Id)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            var res1 = db.Sp_Get_Lead(Id).SingleOrDefault();
            var res7 = db.SAM_Receipts.Where(x => x.Lead_Id == Id).ToList();
            var res = new LeadsData { Lead = res1 };
            return PartialView(res);
        }
        public JsonResult SAM_ProcessVoucher(ReceiptData rd, long Id)
        {
            return Json(true);
        }
        public JsonResult RequestPaymentVoucher(long Id, decimal Amount, string Description)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Requested Payment Voucher of Amount " + Amount + " For " + Description, "Request", "Activity_Record", ActivityType.Voucher_Request.ToString(), Id);

            var res1 = db.Sp_Get_Lead(Id).SingleOrDefault();
            var u1 = db.Users.Find(res1.AssignedTo);
            string Company = u1.Email.Split('@')[1];
            string Comp = "";
            if (Company == "sagpropertyexchange.com") { Comp = "SPE"; } else { Comp = "SAM"; }
            var res = db.Sp_Add_SAM_Voucher_Req(Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), userid, res1.Name, res1.Father_Husband, res1.Block, "", Id, 0, "", "", res1.Offered_Price, res1.Mobile_1, res1.Address, Description, "", Comp);
            return Json(true);
        }
        public ActionResult VoucherRequest()
        {
            var res = db.Sp_Get_SAM_Voucher_Req().ToList();
            return View(res);
        }
        public ActionResult VoucherRequestDetails(long Id)
        {
            var res = db.Sp_Get_SAM_Voucher_Req_Id(Id).SingleOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Detail Page of " + res.Description + " Voucher", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);

            return PartialView(res);
        }
        public JsonResult GenerateVoucher(long Id, string Bank, string Branch, string Cbp_No, DateTime? Cbp_date, string PaymentType, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            bool headcashier = false;
            if (User.IsInRole("Head Cashier"))
            {
                headcashier = true;
            }
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res1 = db.Sp_Update_Voucher(Id, Bank, Branch, Cbp_No, Cbp_date, PaymentType, userid, Type).FirstOrDefault();
                    var res2 = db.Sp_Get_SAM_Voucher_Req_Id(Id).SingleOrDefault();
                    var dealnumber = db.Leads.Where(x => x.Id == res2.Lead_Id).Select(x => x.Deal_Number).FirstOrDefault();

                    AccountHandlerController de = new AccountHandlerController();
                    de.SAM_Vouchers(res2.Amount, PaymentType, Cbp_No, Cbp_date, Bank, new Helpers().RandomNumber(), userid, res1.ToString(), 1, dealnumber, headcashier);
                    var a = db.Sp_Add_Activity(userid, "Updated Voucher Payment Details to " + PaymentType, "Update", "Activity_Record", ActivityType.Voucher_Update.ToString(), Id);

                    Transaction.Commit();
                    var res = new { VoucherId = Id, Token = userid, Status = true };
                    return Json(res);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    var res = new Return { Status = false, Msg = "Payment Voucher cannot be generated" };
                    return Json(res);
                }
            }
        }
        public ActionResult Payment_Voucher_Req(long Id)
        {
            var res = db.Prj_Voucher_Req.Where(x => x.Group_Id == Id).ToList();
            return View(res);
        }
        public ActionResult PettyCashVoucher(long Id, long Token)
        {
            var res = db.Vouchers.SingleOrDefault(x => x.Id == Id && x.TokenParameter == Token);
            return View(res);
        }


        ////////////////////////////
        ///

        public ActionResult PlotReg()
        {
            return View();
        }
        public void RegPlot(List<Getresult> AllData)
        {
            foreach (var item in AllData)
            {
                var plot = db.Plots.Where(x => x.Plot_Number == item.PlotNo &&
                    x.Type == item.PlotType &&
                    x.Sector == item.Sector &&
                    x.Block_Name == item.Block
                ).FirstOrDefault();

                var res3 = db.Sp_Get_PlotData(plot.Id).FirstOrDefault();
                var res2 = db.Sp_Get_PlotLastOwner(plot.Id).ToList();
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Sp_Add_Activity(20038, "Generated a New Allotment Letter of Plot No." + plot.Plot_Number + " Block:" + plot.Block_Name + " For :" + string.Join(",", res2.Select(x => x.Name)), "Create", Modules.PlotManagement.ToString(), ActivityType.Allotment_Letter_Generate.ToString(), plot.Id);
                        db.Sp_Add_PlotComments(plot.Id, "Generated a New Allotment Letter of " + string.Join(",", res2.Select(x => x.Name)), 20038, ActivityType.Allotment_Letter_Generate.ToString());
                        string[] img = new string[6];
                        for (int i = 0; i < res2.Count(); i++)
                        {
                            img[i] = res2[i].Owner_Img;
                        }
                        var res = db.Sp_Add_AllotmentLetter(string.Join(",", res2.Select(x => x.Id)), string.Join(",", res2.Select(x => x.Name)), string.Join(",", res2.Select(x => x.Father_Husband)), string.Join(",", res2.Select(x => x.CNIC_NICOP)), string.Join(",", res2.Select(x => x.Postal_Address)), "Meher Estate Developers", plot.Phase_Name,
                            plot.Block_Name, plot.Plot_Number + " " + plot.Sector, plot.Plot_Size, plot.Type, plot.Area.ToString(), res3.Dimension, "Zubair Hashmi", "Faheem Yousif", 20038, plot.Id, Convert.ToInt64(res2.Select(x => x.GroupTag).FirstOrDefault()), "1.jpg", img[1], img[2], img[3], img[4], img[5], "Shoaib Afzal Malik", "Cheif Executive Officer").FirstOrDefault();
                        if (res == 0)
                        {
                            Transaction.Rollback();
                            var ret2 = new { Status = false, Msg = "Already Exists" };
                        }
                        Helpers helpers = new Helpers(Modules.PlotManagement, Types.Plots);
                        object[] data = new object[6];
                        data[0] = string.Join(",", res2.Select(x => x.Name));
                        data[1] = plot.Plot_Number + " " + plot.Sector;
                        data[2] = plot.Phase_Name;
                        data[3] = plot.Block_Name;
                        data[4] = plot.Plot_Size;
                        data[5] = res3.Dimension;
                        var QR_Data = helpers.GenerateQRCode(data);
                        Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        var ret2 = new { Status = false, Msg = "Error Occured" };
                    }
                }

            }
        }


        //public void RegPlot(List<Getresult> AllData)
        //{
        //    foreach (var a in AllData)
        //    {
        //        var plot = db.Plots.Where(x => x.Plot_Number == a.PlotNo &&
        //            x.Type == a.PlotType &&
        //            x.Sector == a.Sector &&
        //            x.Block_Name == a.Block
        //        ).FirstOrDefault();

        //        var owner = db.Sp_Get_PlotLastOwner(plot.Id).FirstOrDefault();

        //        string[] files = { "1.jpg" };

        //        foreach (var item in files)
        //        {
        //            string fileName = item;
        //            string sourcePath = Server.MapPath("~/images/");
        //            if (!Directory.Exists(Server.MapPath("~/Repository/CustomerImagesPlots/" + plot.Id + "/" + owner.Id+"")))
        //            {
        //                Directory.CreateDirectory(Server.MapPath("~/Repository/CustomerImagesPlots/" + plot.Id + "/" + owner.Id + ""));
        //            }
        //            string targetPath = Server.MapPath("~/Repository/CustomerImagesPlots/" + plot.Id + "/" + owner.Id + "");
        //            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
        //            string destFile = System.IO.Path.Combine(targetPath, fileName);
        //            System.IO.File.Copy(sourceFile, destFile, true);
        //        }
        //    }
        //}










    }
}