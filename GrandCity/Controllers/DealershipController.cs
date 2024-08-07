using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]

    public class DealershipController : Controller
    {
        // GET: Dealership
        private readonly Grand_CityEntities db = new Grand_CityEntities();
        [Authorize(Roles = "Dealership Information,Administrator")]
        [NoDirectAccess] public ActionResult AllDealerships()
        {
            long userid = long.Parse(User.Identity.GetUserId());

            var res = db.Sp_Get_Dealerships().ToList(); 
            db.Sp_Add_Activity(userid, "Accessed All Dealersip Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return View(res);
        }
        [NoDirectAccess] public ActionResult DealershipData(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Id = Id;
            var res1 = db.Dealerships.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Dealers.Where(x => x.Dealership_Id == Id).ToList();
            var res = new DealersData { Dealers = res2, Dealership = res1.Dealership_Name, RegisterationDate = res1.Registeration_Date };
   
            db.Sp_Add_Activity(userid, "Accessed Dealership Details  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }
        [NoDirectAccess] public ActionResult SAMarketingData(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Id = Id;
            var res1 = db.Dealerships.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Dealers.Where(x => x.Dealership_Id == Id).ToList();
            var res = new DealersData { Dealers = res2, Dealership = res1.Dealership_Name, RegisterationDate = res1.Registeration_Date };
       
            db.Sp_Add_Activity(userid, "Accessed  SA Marketing Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }
        [NoDirectAccess] public ActionResult DealerStockSummary(long Id)
        {
            var res = db.Sp_Get_DealershipFilesReport(Id).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult DealerPlotSummary(long Id)
        {
            var res = db.Sp_Get_Dealership_Plots_Report(Id).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult AddDealer()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddDealer(string Dealership, string COA_CommisionCodes, string COA_Codes, List<Dealer> Dealers)
        {

            long userid = long.Parse(User.Identity.GetUserId());

            Helpers hel = new Helpers(Modules.Dealers, Types.DealerRegister);
            object[] Data = new object[2];
            Data[0] = Dealership;
            Data[1] = Dealership;
            List<NameNumber> nameNumbers = Dealers.Select(x => new NameNumber { Name = x.Name, Number = x.Mobile_1 }).ToList();
            QRCodeReturn Qrid = hel.GenerateQRCode(Data, nameNumbers);
            var DealerXML = new XElement("Dealers", Dealers.Select(x => new XElement("Dealersdata",
               new XAttribute("Name", x.Name),
               new XAttribute("Father_Name", x.Father_Name),
               new XAttribute("CNIC_NICOP", x.CNIC_NICOP),
               new XAttribute("Mobile_1", x.Mobile_1),
               (string.IsNullOrEmpty(x.Mobile_2)) ? null : new XAttribute("Mobile_2", x.Mobile_2),
               new XAttribute("Image", x.Image)
               ))).ToString();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res = db.Sp_Add_Dealership_Dealers(Dealership, Qrid.Id, DealerXML, COA_CommisionCodes,COA_Codes).FirstOrDefault();

                    //Adding Income Account if not Present

                    AccountHandlerController ah = new AccountHandlerController();
                    ah.RegisterDealership(res, Dealership, userid);

                    var a = db.Sp_Add_Activity(userid, "Added New Dealership " + Dealership, "Create", "Activity_Record", ActivityType.Dealership.ToString(), userid);
                    transaction.Commit();

                    var returnData = new { DealerData = res, ReturnData = ViewBag.QRCodeImage = Qrid.QR_Image };
                    return Json(returnData);
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return Json(false);
                }
            }
        }
        [Authorize(Roles = "Generate File,Administrator")]
        [NoDirectAccess] public ActionResult DealerFormAssociation()
        {
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects().ToList(), "Id", "Project_Name");
            var plots = db.Sp_Get_Plots_Size().Select(x => new { Plot = x }).ToList();
            ViewBag.Plots = new SelectList(plots, "Plot", "Plot");
            ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships().Where(x => x.Status == "Registered").Select(x => new { x.Id, x.Dealership_Name }).Distinct().ToList(), "Id", "Dealership_Name");

            var res = (from x in db.Installment_Plot_Category
                       select x).AsEnumerable().Select(s => new { Id = s.Id, Text = s.Plot_Size + " Marla - " + string.Format("{0:n0}", s.Grand_Total), Group = s.Plot_Size + " Marla" }).ToList();
            ViewBag.PlotInst = new SelectList(res, "Id", "Text", "Group", 1);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Dealer Assosiaction Form Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        [HttpPost]
        public JsonResult DealerFormAssociation(List<DealerFileForm> dealerFileForms)
        {
            List<DealerFileForm> Dff = new List<DealerFileForm>();
            Helpers h = new Helpers();
            long Groupid = h.RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            var did = dealerFileForms.Select(x => x.Dealership_Id).FirstOrDefault();
            db.Sp_Add_DealershipComments(did, "Generated File Form", userid, "Dealer_Form_Association");
            //db.SP_Files_Log("Create", "Generate File Form of Group : " + Groupid, "File Form", Request.UserHostAddress, userid, null);
            foreach (var item in dealerFileForms)
            {
                decimal Security_Value = item.Security / item.Filecount;
                for (int i = 1; i <= item.Filecount; i++)
                {
                    item.Security = Security_Value;
                    item.Group_Id = Groupid;
                    item.userid = userid;
                    FileSystemController fs = new FileSystemController();
                    var res = fs.RegisterFileForm(item);
                    Dff.Add(res);
                }
            }
            return Json(Dff);
        }
        [HttpPost]
        public JsonResult DealsFormAssociation(List<DealerFileForm> dealerFileForms)
        {
            List<DealerFileForm> Dff = new List<DealerFileForm>();
            Helpers h = new Helpers();
            long Groupid = h.RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            foreach (var item2 in dealerFileForms)
            {
                if (item2.Id != 0)
                {
                    db.Sp_Update_FileGroup(item2.Id, Groupid);
                }

            }
            var did = dealerFileForms.Select(x => x.Dealership_Id).FirstOrDefault();
            db.Sp_Add_DealershipComments(did, "Generated File Form", userid, "Deals_Form_Association");
            db.SP_Files_Log("Create", "Generate File Form of Group : " + Groupid, "File Form", Request.UserHostAddress, userid, null);
            foreach (var item in dealerFileForms)
            {
                decimal Security_Value = item.Security / item.Filecount;
                for (int i = 1; i <= item.Filecount; i++)
                {
                    item.Security = Security_Value;
                    item.Group_Id = Groupid;
                    item.userid = userid;
                    FileSystemController fs = new FileSystemController();
                    var res = fs.RegisterFileForm(item);
                    Dff.Add(res);
                }
            }
            return Json(Dff);
        }
        [NoDirectAccess] public ActionResult GenerateDealerForms(long Group_Id)
        {
            var res1 = db.Sp_Get_FileAppForms_GParameter(Group_Id).ToList();
            var res = res1.Select(x => new DealerFileForm
            {
                Dealership_Name = x.Dealership_Name,
                Dealership_Id = x.Dealership_Id,
                Plot_Size = x.Plot_Size,
                FileCode = x.FileFormNumber.ToString(),
                Dev_Charges_Text = x.Development_Charges.ToString(),
                Type = x.Type,
                Block_Name = x.Block,
                Installment_Plan = Convert.ToInt16(x.Installment_Plan_Id)
            });
            return View(res);
        }
        [NoDirectAccess] public ActionResult GenerateDealForms(long Group_Id)
        {
            var res1 = db.Sp_Get_FileAppForms_GParameter(Group_Id).ToList();
            db.Sp_Update_Print_Flag(Group_Id);
            var res = res1.Select(x => new DealerFileForm
            {
                Dealership_Name = x.Dealership_Name,
                Dealership_Id = x.Dealership_Id,
                Plot_Size = x.Plot_Size,
                FileCode = x.FileFormNumber.ToString(),
                Dev_Charges_Text = x.Development_Charges.ToString(),
                Installment_Plan = Convert.ToInt16(x.Installment_Plan_Id)
            });


            return View(res);
        }
        [NoDirectAccess] public ActionResult AllGeneratedFiles()
        {
            var res = db.Sp_Get_FileAppForms().ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult UpdateInfo(long Id)
        {
            var res = db.Dealerships.SingleOrDefault(x => x.Id == Id);
            return PartialView(res);
        }
        public JsonResult UpdateDealership(long Id, string Dealername, string Status, DateTime Date)
        {
            var res = db.Sp_Update_Dealership(Id, Dealername, Date, Status,"","");
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_DealershipComments(Id, "Updated Details " + string.Join(" , ",Dealername,Status,Date), userid, "Update_Dealership");
            return Json(true);
        }
        [NoDirectAccess] public ActionResult DealersData(long Id)
        {
            var res = db.Dealers.Where(x => x.Dealership_Id == Id).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult AddDealers(long Id)
        {
            ViewBag.Dealer = db.Dealerships.SingleOrDefault(x => x.Id == Id).Registeration_Date;
            ViewBag.Id = Id;
            var res = db.Dealers.Where(x => x.Dealership_Id == Id).ToList();
            return PartialView(res);
        }
        public JsonResult AddDealerInfo(Dealer Dealers)
        {
            var res = db.Sp_Add_Dealers(Dealers.Name, Dealers.Father_Name, Dealers.CNIC_NICOP, Dealers.Mobile_1, Dealers.Mobile_2, Dealers.Address, Dealers.Dealership_Id, Dealers.City, Dealers.Date_Birth).FirstOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_DealershipComments(Dealers.Dealership_Id, "Added Dealer Details: " + string.Join(" , ", Dealers.Name, Dealers.Father_Name), userid, "Update_Dealer");
            return Json(res);
        }
        public JsonResult UpdateDealerInfo(Dealer Dealers, string img)
        {
            db.Sp_Update_Dealers(Dealers.Name, Dealers.Father_Name, Dealers.CNIC_NICOP, Dealers.Mobile_1, Dealers.Mobile_2, Dealers.Address, Dealers.Id, Dealers.Image, Dealers.City, Dealers.Date_Birth);
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_DealershipComments(Dealers.Dealership_Id, "Updated Dealer Details: " + string.Join(" , ", Dealers.Name, Dealers.Father_Name), userid, "Update_Dealer");
            if (!Directory.Exists(Server.MapPath("~/Repository/Dealers/" + "/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Repository/Dealers/"));
            }

            var pathMain = Path.Combine(Server.MapPath("~/Repository/Dealers/" + "/"), Dealers.Id + ".jpg");
            Helpers h = new Helpers();
            var Images = h.SaveBase64Image(img, pathMain, Dealers.Id.ToString());
            return Json(true);
        }
        public JsonResult GetDealerNames(long Id)
        {
            var res = db.Dealers.Where(x => x.Dealership_Id == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(res);
        }
        public JsonResult GetDealerDetails(long Id)
        {
            var res = db.Dealers.Where(x => x.Id == Id).Select(x => new
            {
                x.Name,
                x.Father_Name,
                x.Address,
                x.Date_Birth,
                x.Mobile_1,
                x.Mobile_2,
                x.City,
                x.CNIC_NICOP
            }).SingleOrDefault();
            return Json(res);
        }
        [NoDirectAccess] public ActionResult NewFileDesign(long Group_Id)
        {
            var res1 = db.Sp_Get_FileAppForms_GParameter(Group_Id).ToList();
            var res = res1.Select(x => new DealerFileForm
            {
                Dealership_Name = x.Dealership_Name,
                Dealership_Id = x.Dealership_Id,
                Plot_Size = x.Plot_Size,
                FileCode = x.FileFormNumber.ToString(),
                Type = x.Type,
                Block_Name = x.Block,
                Sector = x.Sector,
                Dev_Charges_Text = x.Development_Charges.ToString(),
                Installment_Plan = Convert.ToInt16(x.Installment_Plan_Id)
            });
            return View(res);
        }
        [NoDirectAccess] public ActionResult DealershipRegisteration()
        {
            ViewBag.Dealership = new SelectList(db.Dealerships.Select(x => new { x.Id, x.Dealership_Name }), "Id", "Dealership_Name");
            ViewBag.Bank = new SelectList(Nomenclature.Banks().ToList(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return View();
        }
        public JsonResult DealerRegister(long Id, decimal? Amount, long TransactionId, string payType, string chqNo, string bankName, string bankBranch, DateTime? payDate)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res2 = db.Dealerships.Where(x => x.Id == Id).SingleOrDefault();
            var res1 = db.Dealers.Where(x => x.Dealership_Id == Id).ToList();
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res4 = db.Sp_Add_Receipt("", Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), bankName, chqNo, null, bankBranch, string.Join(",", res1.Select(x=> x.Mobile_1))
                   , string.Join(",", res1.Select(x => x.Father_Name)), Id, string.Join(",", res1.Select(x => x.Name)), payType, 0, "Grand City Kharian", 0, null, null, ReceiptTypes.DealershipRegisteration.ToString(), userid, userid, ""
                   , null, Modules.PlotManagement.ToString(), null, res2.Dealership_Name, null, null, 0, TransactionId, res2.Dealership_Name, receiptno, comp.Id).FirstOrDefault();
                    var desc = "Dealership Registration Fee by " + res2.Dealership_Name;
               
                    db.Sp_Add_Activity(userid, "Registered New Dealer in " + res2.Dealership_Name, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
                    if (payType != "Cash")
                    {
                        var res22 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(Amount, bankName, bankBranch, payType, null, null, PaymentMethodStatuses.Pending.ToString(),
                            Modules.Dealers.ToString(), Types.Booking.ToString(), userid, chqNo, null, payDate, "", res4.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                    }
                    bool headcashier = false;
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }
                    try
                    {
                        AccountHandlerController de = new AccountHandlerController();
                        var res3 = de.DealerRegisterationFee(Amount, res2.Dealership_Name, payType, chqNo, payDate, bankName, desc, TransactionId, userid, res4.Receipt_No, 1, headcashier);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    }
                    var res = new { Status = true, Receiptid = res4.Receipt_No, Token = userid };
                    Transaction.Commit();
                    return Json(res);
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                        db.Sp_Add_ErrorLog(e.Message + e.InnerException.ToString() + e.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return Json(new Return { Status = false, Msg = "Error Occured" });

                }
            }
        }
        [NoDirectAccess] public ActionResult DealershipSecurity()
        {
            ViewBag.Dealership = new SelectList(db.Dealerships.Select(x => new { x.Id, x.Dealership_Name }), "Id", "Dealership_Name");
            ViewBag.Bank = new SelectList(Nomenclature.Banks().ToList(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Dealership Security Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        [HttpPost]
        public JsonResult DealerSecurity(long Id, decimal? Amount, long TransactionId, string PaymentType, string Bank, string Inst_No, DateTime? Inst_Date, string Branch, string Description, string Img, long? Deal)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res2 = db.Dealerships.Where(x => x.Id == Id).SingleOrDefault();
            var res1 = db.Dealers.Where(x => x.Dealership_Id == Id).ToList();
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res4 = db.Sp_Add_Receipt("", Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), Bank, Inst_No, Inst_Date, Branch, string.Join(",", res1.Select(x => x.Mobile_1))
                      , string.Join(",", res1.Select(x => x.Name)), Id, res2.Dealership_Name, PaymentType, 0, "Grand City Kharian", 0, null, null, ReceiptTypes.DealerAdvance.ToString(), userid, userid, Description, null, Modules.PlotManagement.ToString(), null, res2.Dealership_Name, null, null, 0, TransactionId,res2.Dealership_Name, receiptno,comp.Id).FirstOrDefault();

                    if (PaymentType != "Cash")
                    {
                        var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(Amount, Bank, Branch, PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                Modules.Dealers.ToString(), Types.DealerAdvance.ToString(), userid, Inst_No, Id, Inst_Date, "", res4.Receipt_Id,comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                        if (string.IsNullOrEmpty(Img))
                        {
                            int index = 0;
                            foreach (string file in Request.Files)
                            {
                                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                                index++;
                                if (hpf.ContentLength == 0)
                                    continue;
                                var imageName = Path.GetExtension(hpf.FileName);
                                if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                                }
                                string dt = string.Format("{0:d/M/yyyy}", Inst_Date);
                                string savedFileName = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), Inst_No + "_" + Bank + "_" + dt.Replace("/", "_") + ".jpg");
                                hpf.SaveAs(savedFileName);
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                            }
                            string dt = string.Format("{0:d/M/yyyy}", Inst_Date);
                            var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), Inst_No + "_" + Bank + "_" + dt.Replace("/", "_") + ".jpg");

                            Helpers H = new Helpers();

                            var Images = H.SaveBase64Image(Img, pathMain, res3.ToString());
                        }
                    }

                    var res = new { Receiptid = res4.Receipt_No, Token = userid };

                    //{
                    //    bool headcashier = false;
                    //    if (User.IsInRole("Head Cashier"))
                    //    {
                    //        headcashier = true;
                    //    }
                    //    try
                    //    {
                    //        AccountHandlerController de = new AccountHandlerController();
                    //        de.DealerAdvance(Amount, res2.Id, PaymentType, Inst_No, Inst_Date, Bank, Description, TransactionId, userid, res4.Receipt_No, 1, headcashier);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Transaction.Rollback();
                    //        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    //    }
                    //}

                    if (Deal != null && PaymentType == "Cash")
                    {
                        var dealData = db.DealerDeals.Where(x => x.Id == Deal).FirstOrDefault();
                        dealData.Received += Amount;
                        db.DealerDeals.Attach(dealData);
                        db.Entry(dealData).Property(x => x.Received).IsModified = true;
                        db.SaveChanges();
                    }

                    Transaction.Commit();
                    return Json(new { Status = true, Msg= "Receipt generated successfully", Receiptid = res4.Receipt_No, Token = userid });
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(new Return { Status = false, Msg = "Error Occured" });
                }
            }
        }

        [NoDirectAccess] public ActionResult DealershipPayment()
        {
            ViewBag.Dealership = new SelectList(db.Dealerships.Select(x => new { x.Id, x.Dealership_Name }), "Id", "Dealership_Name");
            ViewBag.Bank = new SelectList(Nomenclature.Banks().ToList(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return View();
        }
        public JsonResult DealerPayment(long Id, decimal? Amount, long TransactionId, string PaymentType, string Bank, string Inst_No, DateTime? Inst_Date, string Branch, string Description, string Img, decimal? TaxAmount, int? TaxAccount, string Led_Nat)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var user = db.Users.Find(userid);
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res2 = db.Dealerships.Where(x => x.Id == Id).SingleOrDefault();
            var res1 = db.Dealers.Where(x => x.Dealership_Id == Id).ToList();
            var Dealer_COA = new Sp_Get_COA_Head_Code_Result();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {

                    var voch = db.Sp_Add_Voucher_Vendor("-", Amount , GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), null, null, Inst_Date, Inst_No, "", Description, "", null, Modules.Dealers.ToString(), string.Join(",", res1.Select(x => x.Name)),
                        PaymentType, "Grand City", "", TransactionId, "Dealer Payment", userid, res2.Id, TransactionId, user.Name, userid, DateTime.Now, user.Name, null, null, TransactionId).FirstOrDefault();


                    var a = db.Sp_Add_VoucherDetails(Amount, Description, null, null, null, voch.Receipt_Id).FirstOrDefault();
                    {
                        try
                        {
                            var Payment = ah.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                            if (Led_Nat == "Advance")
                            {
                                Dealer_COA = ah.Dealer_Head(Id, comp.Id);

                            }
                            else
                            {
                                Dealer_COA = ah.Dealer_Commission_Head(Id, comp.Id);
                            }

                            var Receivable_Credit = db.Sp_Add_Journal_Entry(Amount + TaxAmount, 0, Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, 1, userid, userid, null, Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, voch.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            var Reciv_Debit = db.Sp_Add_Journal_Entry(0, Amount , Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, 1, userid, userid, null, Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, voch.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();

                            if (TaxAmount > 0)
                            {
                                var taxAccount = ah.GetHead(Convert.ToInt32(TaxAccount));
                                if (taxAccount is null)
                                {
                                    Transaction.Rollback();
                                    return Json(new { Status = false, Msg = "Tax Account not mapped. Please map tax account for further use." });
                                }
                                var taxCredit = db.Sp_Add_Journal_Entry(0, TaxAmount, taxAccount.Head, taxAccount.Id, taxAccount.Text_ChartCode, taxAccount.Head, Description, TransactionId, 1, userid, userid, null, null, Inst_No, "", Inst_Date, null, "", null, voch.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit

                            }


                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        }
                    }
                    Transaction.Commit();
                    return Json(new { Status = true, VoucherNo = voch.Receipt_Id, Token = TransactionId });
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(new Return { Status = false, Msg = "Something went wrong" });
                }
            }
        }

        [NoDirectAccess] public ActionResult SpecialForm(long Group_Id)
        {
            var res1 = db.Sp_Get_FileAppForms_GParameter(Group_Id).ToList();
            List<FileData> re = new List<FileData>();

            foreach (var item in res1)
            {

                FileData f = new FileData
                {
                    Dealership_Name = item.Dealership_Name,
                    Plot_Size = item.Plot_Size,
                    File_Form_Number = item.FileFormNumber,
                    Name = "Abdul Ghaffar",
                    Father_Husband = "Muhammad Ismail",
                    Postal_Address = "House # 1 Street 1 Muhallah Islam Nagar Shahdra Lahore",
                    Residential_Address = "House # 1 Street 1 Muhallah Islam Nagar Shahdra Lahore",
                    Mobile_1 = "03004003740",
                    Phone_Office = "",
                    Email = "",
                    Mobile_2 = "",
                    Residential = "",
                    Occupation = "Businessman",
                    City = "Lahore",
                    Development_Charges = "T-B-A",
                    Block_Name = "",
                    Nationality = "Pakistani",
                    Date_Of_Birth = "30-April-1973",
                    CNIC_NICOP = "35202-5726239-5",
                    Nominee_Name = "Abdul Jabbar",
                    Nominee_CNIC_NICOP = "35202-5058139-3",
                    Nominee_Relation = "Brother",
                    Nominee_Address = "House # 1 Street 1 Muhallah Islam Nagar Shahdra Lahore"
                };
                re.Add(f);
            }
            return View(re);
        }
        //////////////
        /// Dealership Deal Module
        /////////////

        // Add deal
        [NoDirectAccess] public ActionResult Deal()
        {
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects(), "Id", "Project_Name");
            ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships(), "Id", "Dealership_Name");
            return View();
        }
        // Dealers Deals
        //public JsonResult AddDeal(Dealers_Deals dealers_deals, List<Final_Deal_Plans> deal)
        //{
        //    var res1 = db.Dealers_Deals.Any(x => x.Deal_Total_Amount == dealers_deals.Deal_Total_Amount && x.Dealer_Id == dealers_deals.Dealer_Id && x.Dealer_Name == dealers_deals.Dealer_Name && x.Advance == dealers_deals.Advance);
        //    if (res1)
        //    {
        //        return Json(false);
        //    }
        //    else
        //    {
        //        var Id = db.Sp_Add_Deals(dealers_deals.Dealer_Name, dealers_deals.Dealer_Id, dealers_deals.Deal_Total_Amount, dealers_deals.Advance, dealers_deals.Rebate, dealers_deals.Project_Id, dealers_deals.Phase_Id, dealers_deals.Block_Id, dealers_deals.Project_Name, dealers_deals.Phase_Name, dealers_deals.Block_Name, dealers_deals.Deposit).SingleOrDefault();
        //        Helpers h = new Helpers();
        //        long Groupid = h.RandomNumber();

        //        foreach (var item in deal)
        //        {
        //            db.Sp_Add_Deal_Plan(item.Rate_Per_Marla, item.Plot_Size, item.Num_Of_Files, item.Num_Of_Files, item.Marlas, item.Total_Files, item.Total_Marlas, item.Total_Price, item.Deposit, item.Advance, item.Rebate, Groupid, Id);
        //        }


        //        return Json(true);
        //    }
        //    // Plan Generation

        //}
        // Deal List
        //[NoDirectAccess] public ActionResult DealList()
        //{
        //    var res = db.Sp_Get_Deals().ToList();
        //    return View(res);
        //}
        //// Update Deal View
        //[NoDirectAccess] public ActionResult DealUpdation(long Id)
        //{
        //    var res = db.Sp_Get_Deal_Parameter(Id).SingleOrDefault();
        //    return PartialView(res);
        //}
        // Update dealers dael
        //public JsonResult UpdateDeal(Dealers_Deals dealers_Deals)
        //{
        //    db.Sp_Update_Dealers_Deal(dealers_Deals.Id, dealers_Deals.Deal_Total_Amount, dealers_Deals.Advance, dealers_Deals.Rebate);
        //    return Json(true);
        //}
        // deal basic details
        //[NoDirectAccess] public ActionResult Detail(long details)
        //{
        //    var res = db.Sp_Get_Deal_Parameter(details).SingleOrDefault();
        //    return View(res);
        //}
        // Deal With Assignment
        //[NoDirectAccess] public ActionResult FullDeal(long Id)
        //{

        //    var Plan = db.Final_Deal_Plans.Where(x => x.Deal_Id == Id).ToList();
        //    var DealerDetails = db.Sp_Get_Deal_Parameter(Id).SingleOrDefault();
        //    ViewBag.VersionAmt = db.Deal_Versions.Where(x => x.Deal_Id == Id).Select(x => x.Deposit).Distinct().Sum();
        //    var res = new DealershipDeals { PlanDetails = Plan, Dealer = DealerDetails };
        //    return PartialView(res);
        //}
        // Version 
        [NoDirectAccess] public ActionResult Version()
        {
            ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships(), "Id", "Dealership_Name");
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects(), "Id", "Project_Name");
            return View();
        }
        // Version List
        //[NoDirectAccess] public ActionResult VersionList(long Deal_Id)
        //{
        //    ViewBag.Advancevalue = db.Dealers_Deals.Where(x => x.Id == Deal_Id).Select(x => x.Advance).SingleOrDefault();
        //    var res = db.Sp_Get_Version(Deal_Id).ToList();
        //    return PartialView(res);
        //}
        //// Version Creation
        //[HttpPost]
        //public JsonResult CreateVersion(long Id, List<Final_Deal_Plans> deal, List<Deal_Versions> version)
        //{
        //    List<Deal_Versions> List = new List<Deal_Versions>();
        //    if (deal != null || version != null)
        //    {
        //        if (deal.Count() == version.Count())
        //        {
        //            var deatails = db.Dealers_Deals.Where(x => x.Id == Id).SingleOrDefault();
        //            var dealer = db.Dealerships.Where(x => x.Id == deatails.Dealer_Id).SingleOrDefault();
        //            Helpers h = new Helpers();
        //            long Groupid = h.RandomNumber();
        //            foreach (var item3 in version)
        //            {
        //                var IdPlot = db.Installment_Plot_Category.Where(x => x.Plot_Size == item3.Plot_Size.ToString() && x.Rate == item3.Rare_Per_marla).Select(x => x.Id).FirstOrDefault();
        //                if (IdPlot != 0)
        //                {
        //                    item3.Installment_Plan = IdPlot;
        //                }
        //                else
        //                {
        //                    return Json(false);
        //                }
        //            }
        //            foreach (var item2 in version)
        //            {

        //                var VerId = db.Sp_Add_Version(item2.Plot_Size, item2.Filecount, item2.Dealership_Id, deatails.Dealer_Name, item2.Project_Id, item2.Phase,
        //                    item2.Block, item2.Rare_Per_marla, item2.Amount, item2.Deposit, Groupid, item2.Deal_Id, deatails.Project_Name, deatails.Phase_Name, deatails.Block_Name, item2.Installment_Plan).SingleOrDefault();

        //                var VerList = db.Deal_Versions.Where(x => x.Id == VerId).SingleOrDefault();
        //                List.Add(VerList);
        //            }
        //            foreach (var item in deal)
        //            {
        //                db.Sp_Update_Deal_File(item.Id, item.Remaning_Files);
        //            }
        //            db.Sp_Add_Ledger(DealerShipDeals.Dealer_Advance.ToString(), version.Select(x => x.Deposit).FirstOrDefault(), DateTime.UtcNow, null, 0, version.Select(x => x.Deal_Id).FirstOrDefault(), false, dealer.Id, dealer.Dealership_Name, 0, 0);
        //            return Json(List);
        //        }
        //        return Json(false);
        //    }
        //    else
        //    {
        //        return Json(false);
        //    }
        //}
        // Deals Files List
        [NoDirectAccess] public ActionResult DealFiles()
        {
            var res = db.Sp_Get_DealFiles().ToList();
            return View(res);
        }
        // Voucher
        //[NoDirectAccess] public ActionResult DealVoucher(int PlanId, long FileFormId)
        //{
        //    Helpers h = new Helpers();
        //    long Groupid = h.RandomNumber();
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    AccountHandlerController ah = new AccountHandlerController();
        //    var comp = ah.Company_Attr(userid);
        //    var res = db.Installment_Structure.Where(x => x.Installment_Type == "3" && x.Installment_Plot_Id == PlanId).FirstOrDefault();
        //    var res2 = db.File_Form.Where(x => x.Id == FileFormId).SingleOrDefault();
        //    var res3 = db.Dealerships.Where(x => x.Id == res2.Dealership_Id).SingleOrDefault();
        //    var res5 = db.Deal_Versions.Where(x => x.Form_Group_Id == res2.Group_Id).FirstOrDefault();
        //    var Id = db.Sp_Add_Voucher(null, res.Amount, null, null, null, null, null, null
        //        , "Advance against " + res2.FileFormNumber.ToString() + " file number", null, res.Installment_Plot_Id,
        //        Modules.Dealers.ToString(), res3.Dealership_Name, "Advance", null, null, Groupid, "Dealer Advance", userid, null,comp.Id).SingleOrDefault();
        //    //bool headcashier = false;
        //    //if (User.IsInRole("Head Cashier"))
        //    //{
        //    //    headcashier = true;
        //    //}
        //    //try
        //    //{
        //    //    AccountHandlerController de = new AccountHandlerController();
        //    //    de.DealerAdvanceRet(res.Amount, res3.Dealership_Name, "Cash", null, null, null, "Advance against " + res2.FileFormNumber.ToString() + " file number", Groupid, userid, Id.Receipt_No, 1, headcashier);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //    //}            
        //    db.Sp_Update_File_Print_Flag(FileFormId);
        //    db.Sp_Add_Ledger(DealerShipDeals.File_Advance.ToString(), ((-1) * res.Amount), DateTime.Now, DateTime.Now, res2.FileFormNumber, res5.Deal_Id, true, res3.Id, res3.Dealership_Name, ((res.Amount / 100) * 50), 0);
        //    var res4 = db.Vouchers.Where(x => x.Id == Id.Receipt_Id).SingleOrDefault();
           

        //    db.Sp_Add_DealershipComments(res2.Dealership_Id, "Generated Deal Voucher of " +  res.Amount, userid, "Dealer_Voucher");
        //    return View(res4);
        //}
        // Ledger Report
        //[NoDirectAccess] public ActionResult Ledger(long Deal_Id)
        //{
        //    var res1 = db.Sp_Get_Ledger(Deal_Id).ToList();
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    db.Sp_Add_Activity(userid, "Accessed Dealership Ledger Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Deal_Id);
        //    return PartialView(res1);
        //}
        //[NoDirectAccess] public ActionResult Receipt()
        //{
        //    var res = db.Sp_Get_Deal_Receipt().ToList();
        //    return View(res);
        //}
        //

        // Generate Recepit
        //public JsonResult ReceiptGenerate(long Id, List<ReceiptData> Receiptdata)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    db.Sp_Update_Deal_Receipt(Id, DateTime.Now);
        //    var res1 = db.Deal_Ledger.Where(x => x.Id == Id).SingleOrDefault();
        //    var res2 = db.Dealers_Deals.Where(x => x.Id == res1.Deal_Id).FirstOrDefault();
        //    var dealer = db.Dealers.Where(x => x.Dealership_Id == res2.Dealer_Id).FirstOrDefault();
        //    List<string> ids = new List<string>();
        //    foreach (var rd in Receiptdata)
        //    {

        //        var res3 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, dealer.Mobile_1,
        //                                      dealer.Name, null, res2.Dealer_Name, rd.PaymentType, null,
        //                                      res2.Project_Name, null, null, null, "Dealer Advance", userid, userid, "Dealership Deal", DateTime.UtcNow, null,
        //                                      null, null, res2.Block_Name, 0).FirstOrDefault();

        //        if (rd.PaymentType != "Cash")
        //        {
        //            var res4 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
        //                "Dealership Deals", "Dealer Advance", res1.Id, rd.PayChqNo, 1, rd.Ch_bk_Pay_Date, "", res3.Receipt_Id).FirstOrDefault());
        //            var text = "Dear " + rd.Name + ",\n\r" +
        //        "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

        //            if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
        //            {
        //                Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
        //            }
        //            string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
        //            var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
        //            Helpers h = new Helpers();
        //            var Imgres = h.SaveBase64Image(rd.FileImage, pathMain, res4.ToString());
        //            try
        //            {
        //                SmsService smsService = new SmsService();
        //                smsService.SendMsg(text, dealer.Mobile_1);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        //        else
        //        {
        //            var text = "Dear " + rd.Name + ",\n\r" +
        //          "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";

        //            try
        //            {
        //                SmsService smsService = new SmsService();
        //                //smsService.SendMsg(text, dealer.Mobile_1);
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }

        //        ids.Add(res3.Receipt_No);
        //    }

        //    var data = new { Receiptid = ids, Token = userid };
        //    return Json(data);

        //}
        // Printed FilesFormNumber List show
        [NoDirectAccess] public ActionResult FilesRetrievel(long Groupid)
        {
            var res = db.Sp_Get_Printed_FilesFormsNumber(Groupid).ToList();
            return PartialView(res);
        }

     
        // Rebate Release
        //public JsonResult RebateRelease(RebateModel rebate)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    if (rebate.Method == "Adjust_in_adv")
        //    {
        //        var res = db.Deal_Ledger.Where(x => x.Deal_Id == rebate.Deal_Id).ToList();
        //        var res2 = db.Dealers_Deals.Where(x => x.Id == rebate.Deal_Id).SingleOrDefault();
        //        var SumRebate = res.Select(x => x.Rebate).Sum();
        //        db.Sp_Add_Ledger(DealerShipDeals.Dealer_Commission_Adjust.ToString(), (rebate.Amount), DateTime.Now, DateTime.Now, 0, rebate.Deal_Id, true, res.Select(x => x.Dealer_Id).FirstOrDefault(), res.Select(x => x.Dealership_Name).FirstOrDefault(), ((-1) * rebate.Amount), (res.Where(x => x.Type == "File_Advance" || x.Type == "Dealer_Advance").Select(x => x.Amount).Sum() + rebate.Amount));

        //        var Id = db.Sp_Add_Voucher(null, (-1) * rebate.Amount, null, null, null, null, null,
        //                                   null, "Rebate Payment Adjusted In Advance", null, null,
        //                                    Modules.Dealers.ToString(), res2.Dealer_Name, "Adjust In Advance", null, res2.Project_Name,
        //                                    userid, "Dealer Rebate", userid, null).SingleOrDefault();
        //        return Json(Id);
        //    }
        //    else
        //    {
        //        var res = db.Deal_Ledger.Where(x => x.Deal_Id == rebate.Deal_Id).ToList();
        //        var res2 = db.Dealers_Deals.Where(x => x.Id == rebate.Deal_Id).SingleOrDefault();
        //        //var SumRebate = res.Select(x => x.Rebate).Sum();

        //        db.Sp_Add_Ledger(DealerShipDeals.Dealer_Commission.ToString(), 0, DateTime.Now, DateTime.Now, 0, rebate.Deal_Id, true,
        //            res.Select(x => x.Dealer_Id).FirstOrDefault(), res.Select(x => x.Dealership_Name).FirstOrDefault(), ((-1) * rebate.Amount), res.Where(x => x.Type == "File_Advance" || x.Type == "Dealer_Advance").Select(x => x.Amount).Sum());

        //        var VouId = db.Sp_Add_Voucher(null, (-1) * rebate.Amount, null, null, null, null, null,
        //                                  null, "Rebate Payment " + rebate.Amount, null, null,
        //                                   Modules.Dealers.ToString(), res2.Dealer_Name, rebate.paymentType, res2.Project_Name,
        //                                   "", userid, "Dealer Rebate", userid, null).SingleOrDefault();
        //        return Json(VouId);
        //    }

        //}
        // rebate voucher 
        [NoDirectAccess] public ActionResult RebateVoucherRelease(long VoucherId)
        {
            var res4 = db.Vouchers.Where(x => x.Id == VoucherId).SingleOrDefault();
            return View(res4);

        }
        public JsonResult AddCommession(Dealer_Commession Com)
        {

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (Com != null)
                    {
                        string text = "";
                        if (Com.Com_Type == "Fixed")
                        {
                            text = "Plot No " + Com.Plot_No + " Block : " + Com.Block + " with  " + Com.Com_Type + " commission value " + string.Format("{0:n0}", Com.Com_Amount) + " on Booking value " + string.Format("{0:n0}", Com.Total_Price);
                        }
                        else if (Com.Com_Type == "Percentage")
                        {
                            text = "Plot No " + Com.Plot_No + " Block : " + Com.Block + " with  " + Com.Percentage + "% commission value " + string.Format("{0:n0}", Com.Com_Amount) + " on Booking value " + string.Format("{0:n0}", Com.Total_Price);
                        }

                        Com.Datetime = DateTime.Now;
                        Com.Text = text;


                        db.Dealer_Commession.Add(Com);
                        db.SaveChanges();
                    }
                    Transaction.Commit();
                    return Json(new Return { Status = true });

                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    throw e;
                }
            }


        }

        public JsonResult AddCommession_UnTransact(Dealer_Commession Com)
        {

            try
            {
                if (Com != null)
                {
                    string text = "";
                    if (Com.Com_Type == "Fixed")
                    {
                        text = "Plot No " + Com.Plot_No + " Block : " + Com.Block + " with  " + Com.Com_Type + " commission value " + string.Format("{0:n0}", Com.Com_Amount) + " on Booking value " + string.Format("{0:n0}", Com.Total_Price);
                    }
                    else if (Com.Com_Type == "Percentage")
                    {
                        text = "Plot No " + Com.Plot_No + " Block : " + Com.Block + " with  " + Com.Percentage + "% commission value " + string.Format("{0:n0}", Com.Com_Amount) + " on Booking value " + string.Format("{0:n0}", Com.Total_Price);
                    }

                    Com.Datetime = DateTime.Now;
                    Com.Text = text;


                    db.Dealer_Commession.Add(Com);
                    db.SaveChanges();
                }
                
                return Json(new Return { Status = true });

            }
            catch (Exception e)
            {

                throw e;
            }


        }

        [NoDirectAccess] public ActionResult Dealer_Commessions(long Id)
        {
            var res = db.Dealer_Commession.Where(x => x.Dealer_Id == Id).ToList();
            return View(res);
        }
        public JsonResult Process_Commession(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Dealer_Commession.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Dealerships.Where(x => x.Id == res1.Dealer_Id).FirstOrDefault();

            Return check = (Return)(this.CheckMaturity(res1.File_Plot_Id, res1.Module, res1.Com_Maturity).Data);
            if (check.Status)
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Helpers h = new Helpers();
                        var TransactionId = h.RandomNumber();
                        var res3 = db.Sp_Update_DealerCommession(Id);
                        {
                            bool headcashier = false;
                            if (User.IsInRole("Head Cashier"))
                            {
                                headcashier = true;
                            }
                            try
                            {
                                var jv = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                                AccountHandlerController de = new AccountHandlerController();
                                de.DealerComeRec(res1.Com_Amount, res2.Dealership_Name, res1.Text, TransactionId, userid, 1, jv.ToString(), headcashier, res1.Dealer_Id);
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            }

                        }
                        db.Sp_Add_DealershipComments(Id, "Commission Processed of RS " + res1.Com_Amount, userid, "Dealer_Commission");
                        Transaction.Commit();
                        return Json(new Return { Msg = "Commession is Processed", Status = true });
                    }
                    catch (Exception e)
                    {
                        Transaction.Rollback();
                        return Json(new Return { Msg = "Error Occured", Status = false });
                    }
                }
            }
            else
            {
                return Json(new Return { Msg = "Plot is not Matured right now", Status = false });
            }
        }
        public JsonResult CheckMaturity(long? Id, string Module, decimal? Maturity)
        {
            if (Module == Modules.PlotManagement.ToString())
            {
                var res1 = db.Sp_Get_PlotInstallments(Id).Sum(x => x.Amount);
                var res2 = db.Sp_Get_ReceivedAmounts(Id, Modules.PlotManagement.ToString()).Where(x => (x.Status is null || x.Status == "Approved")).Sum(x => x.Amount);

                var res3 = (res2 / res1) * 100;
                if (res3 >= Maturity)
                {
                    return Json(new Return { Status = true });
                }
                else
                {
                    return Json(new Return { Status = false });
                }

            }
            else if (Module == Modules.FileManagement.ToString())
            {
                var file = db.File_Form.Where(x => x.Id == Id).FirstOrDefault();
                var res1 = db.Sp_Get_FileInstallments(Id).Sum(x => x.Amount);
                var res2 = db.Sp_Get_ReceivedAmounts(file.Id, Modules.FileManagement.ToString()).Where(x => (x.Status is null || x.Status == "Approved")).Sum(x => x.Amount);

                var res3 = (res2 / res1) * 100;
                if (res3 >= Maturity)
                {
                    return Json(new Return { Status = true });
                }
                else
                {
                    return Json(new Return { Status = false });
                }
            }
            return Json(new Return { Status = false });
            //else if (Module == Modules.ShopManagement.ToString())
            //{

            //}
        }
        [NoDirectAccess] public ActionResult DealershipLedger(long Id)
        {
            var dealer = db.Dealerships.Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.Id = Id;
            ViewBag.Name = dealer.Dealership_Name;
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Dealership Ledger  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return View();
        }
        [HttpPost]
        //[NoDirectAccess] public ActionResult DealerLedgerSearch(DateTime? From, DateTime? To, long Id)
        //{
        //    var dealer = db.Dealerships.Where(x => x.Id == Id).FirstOrDefault();
        //    var fiscal = this.GetFinancialYear();
        //    var CashCode = db.Sp_Get_CA_Head_MultiRef_3("Liability", "Dealers Payable", dealer.Dealership_Name).FirstOrDefault().Code;
        //    var res = db.Sp_Get_JournalLedger(fiscal.Start, fiscal.End, CashCode).ToList();
        //    AccountHeadsController ah = new AccountHeadsController();
        //    ViewBag.LedgerNature = ah.Ledger_Top(CashCode);
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    db.Sp_Add_Activity(userid, "Accessed Dealership Ledger from " + From +" to: " + To, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
        //    return PartialView(res);
        //}
        public async Task<JsonResult> DealerBalance(long DealerId,string Led_Nature)
        {
            AccountHandlerController ah = new AccountHandlerController();
            long userid = long.Parse(User.Identity.GetUserId());

            var comp = ah.Company_Attr(userid);

            if (Led_Nature == "Advance")
            {
                decimal? Bal = 0;
                var res2 = db.Dealerships.Where(x => x.Id == DealerId).FirstOrDefault();
                string Baseurl = "http://116.58.24.62:82/";
                using (var client = new HttpClient())
                {
                    //Passing service base url
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    //Define request data format
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                    HttpResponseMessage Res = await client.GetAsync("grand_city1.1/accounts/ledger_method_report_balance2/" + res2.COA_Code);
                    //Checking the response is successful or not which is sent using HttpClient
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api
                        var Data = Res.Content.ReadAsStringAsync().Result;
                        //Deserializing the response recieved from web api and storing into the Employee list
                        var res = JsonConvert.DeserializeObject<List<DealerLedger>>(Data);
                        if (res.Any())
                        {
                            Bal = res.FirstOrDefault().am;
                        }
                    }
                    //returning the employee list to view
                }
                return Json(new { Balance = Bal });

            }
            else
            {
                var res1 = this.GetFinancialYear();
                var res2 = db.Dealerships.Where(x => x.Id == DealerId).FirstOrDefault();
                var res3 = ah.Dealer_Commission_Head(DealerId, comp.Id);
                var res4 = db.Sp_Get_Head_OpeningClosing_Balance(res3.Text_ChartCode, "Opening", res1.Start, res1.End).FirstOrDefault();
                var res5 = db.Sp_Get_Head_Balance(res3.Text_ChartCode, res1.Start, res1.End).FirstOrDefault();
                return Json(new { Balance = (res4 == null) ? res5.Credit - res5.Debit : (res4.Credit_Balance - res4.Debit_Balance) + res5.Credit - res5.Debit });
            }
            
        }
        private FinancialYear GetFinancialYear()
        {
            return new FinancialYear { Start = new DateTime(2022, 7, 1), End = new DateTime(2023, 6, 30) };
        }
        public JsonResult DealerPlot_Details(long Id)
        {
            var res1 = db.Sp_Get_PlotData(Id).FirstOrDefault();
            var price = db.Installment_Structure.Where(x => x.Installment_Plot_Id == res1.Installment_Plan_Id && x.Installment_Type == "3").FirstOrDefault();
            var Totalprice = db.Installment_Structure.Where(x => x.Installment_Plot_Id == res1.Installment_Plan_Id).ToList().Sum(x => x.Amount);

            var res2 = db.Biding_Reserve_Plots.Where(x => x.Plot_Id == Id && x.PlotStatus == "Available").FirstOrDefault();
            if (res2 is null)
            {
                var res = new { PlotData = res1, Dealer = true, GrandTotal = price.Amount, TotalAmount = Totalprice };
                return Json(res);
            }
            else
            {
                if (Totalprice > 0)
                {
                    var res3 = db.Dealerships.Where(x => x.Id == res2.Dealer_Id).FirstOrDefault();
                    var res = new { PlotData = res1, Dealer = false, Dealership = res3, GrandTotal = price.Amount, TotalAmount = Totalprice };
                    return Json(res);
                }
                else
                {
                    var res3 = db.Dealerships.Where(x => x.Id == res2.Dealer_Id).FirstOrDefault();
                    var res = new { PlotData = res1, Dealer = false, Dealership = res3, GrandTotal = res2.GrandTotal, TotalAmount = res2.GrandTotal };
                    return Json(res);

                }
              
            }
        }
        // Add Deal Allotment Plan
        //[NoDirectAccess] public ActionResult AllotmentPlan(long Id)
        //{
        //    ViewBag.DealerAdvance = db.Dealers_Deals.Where(x => x.Id == Id).Select(x => x.Advance).SingleOrDefault();
        //    ViewBag.DealId = Id;
        //    return PartialView();
        //}
        // Generate Plan Of Full Deal
        //public JsonResult GeneratePlan(long Id,List<Final_Deal_Plans> deal)
        //{
        //    Helpers h = new Helpers();
        //    long Groupid = h.RandomNumber();
        //    var res= db.Sp_Get_Deal_Parameter(Id).SingleOrDefault();
        //    foreach (var item in deal)
        //    {
        //       //db.Sp_Add_Deal_Plan(item.Rate_Per_Marla, item.Plot_Size, item.Num_Of_Files,item.Num_Of_Files, item.Marlas, item.Total_Files, item.Total_Marlas, res.Deal_Total_Amount, item.Deposit, res.Advance, res.Rebate, Groupid);
        //    } 

        //    return Json(true);
        //}

        ///////////////////
        /// Deal Version 
        //////////////////



        //[NoDirectAccess] public ActionResult GeneratedFiles(long GroupId)
        //{
        //    var res = db.Final_Deal_Plans.Where(x => x.Group_Id == GroupId).ToList();
        //    return PartialView(res);
        //}
        [NoDirectAccess] public ActionResult CommessionList()
        {
            var res = db.Dealer_Commession.ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Dealership Commition Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(res);
        }
        
        [NoDirectAccess] public ActionResult UpdateDealerId(long Id)
        {
            var res = db.Dealer_Commession.Where(x=>x.Id == Id).FirstOrDefault();
            //if (res.Module == "FileManagement")
            //{
            //    var file = db.File_Form.Where(x => x.Id == res.File_Plot_Id).FirstOrDefault();
                
            //}
            //else if (res.Module == "PlotManagement")
            //{
            //}
            ViewBag.Dealership = new SelectList(db.Dealerships.Where(x => x.Status == "Registered").ToList(), "Id", "Dealership_Name");
            return PartialView(res);
        }

        
        public JsonResult UpdateDealercommesionId(long CommId, long DelerId,long FileplotId, string Module)
        {

            var Dealername = db.Dealerships.Where(x => x.Id == DelerId).Select(x => x.Dealership_Name).FirstOrDefault();
            var Commeession = db.Sp_Update_FileForm_dealerId(CommId, DelerId, "Comm", Dealername);
          
            if (Module == "FileManagement")
            {
                var b = db.Sp_Update_FileForm_dealerId(FileplotId, DelerId,"File",null);
            }
            else if (Module == "PlotManagement")
            {
                var b = db.Sp_Update_FileForm_dealerId(FileplotId, DelerId, "Plot", Dealername);
            }
            return Json(true);
        }


        [NoDirectAccess] public ActionResult PlotsAssignment()
        {
            var res = (from x in db.Installment_Plot_Category
                       select x).AsEnumerable().Select(s => new { Id = s.Id, Text = s.Plot_Size + " Marla - " + string.Format("{0:n0}", s.Grand_Total), Group = s.Plot_Size + " Marla" }).ToList();
            ViewBag.PlotInst = new SelectList(res, "Id", "Text", "Group", 1);
            //var plots = db.Sp_Get_Plots_Size().Select(x => new { Plot = x }).ToList();
            //ViewBag.Plots = new SelectList(plots, "Plot", "Plot");
            //var res = (from x in db.Installment_Plot_Category
            //           select x).AsEnumerable().Select(s => new { Id = s.Id, Text = s.Plot_Size + " Marla - " + string.Format("{0:n0}", s.Grand_Total), Group = s.Plot_Size + " Marla" }).ToList();
            //ViewBag.PlotInst = new SelectList(res, "Id", "Text", "Group", 1);
            return View();
        }

        //Brought from Iqbal gardens

        [NoDirectAccess] public ActionResult DealerPlotsAssociation()
        {
            ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships().Where(x => x.Status == "Registered").Select(x => new { x.Id, x.Dealership_Name }).Distinct().ToList(), "Id", "Dealership_Name");
            var res = (from x in db.Installment_Plot_Category
                       select x).AsEnumerable().Select(s => new { Id = s.Id, Text = s.Plot_Size + " Marla - " + string.Format("{0:n0}", s.Grand_Total), Group = s.Plot_Size + " Marla" }).ToList();
            ViewBag.PlotInst = new SelectList(res, "Id", "Text", "Group", 1);
            return View();
        }

        public JsonResult SavePlotDeals(List<DealersPlots> PlotsList, long Dealer_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var dealer = db.Dealerships.Where(x => x.Id == Dealer_Id).FirstOrDefault();

            foreach (var item in PlotsList)
            {
                var plots = db.Plots.Where(x => x.Id == item.Plot_Id).FirstOrDefault();

                if (plots.Status == PlotsStatus.Available_For_Sale.ToString())
                {
                    plots.Installment_Plan_Id = item.Plan_Id;
                    plots.Dealership_Id = dealer.Id;
                    plots.Dealership_Name = dealer.Dealership_Name;

                    db.Plots.Attach(plots);
                    db.Entry(plots).Property(x => x.Installment_Plan_Id).IsModified = true;
                    db.Entry(plots).Property(x => x.Dealership_Id).IsModified = true;
                    db.Entry(plots).Property(x => x.Dealership_Name).IsModified = true;

                    db.SaveChanges();
                }
            }
            return Json(new Return { Msg = "Allocated", Status = true });
        }
        [NoDirectAccess] public ActionResult PlotsAssignmentEdit(long GroupTag)
        {
            var res = db.Biding_Reserve_Plots.Where(x => x.GroupTag == GroupTag).ToList();
            var res2 = db.DealerDeals.Where(x => x.GroupTag == GroupTag).FirstOrDefault();
            var deal = new DealsModelEdit();
            deal.DealsList = res;
            deal.Deal = res2;

            return View(deal);
        }
        public JsonResult GetDealers_ForSelect(string s)
        {
            var res = db.Dealerships.Where(x => x.Dealership_Name.Contains(s)).Select(x => new { text = x.Dealership_Name, id = x.Id }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPlots_ForSelect(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return Json(new { text = "Select Plot", id = 0 }, JsonRequestBehavior.AllowGet);
            }
            string[] parsed = s.Split(' ');
            var exstngPlts = db.Biding_Reserve_Plots.Where(x => x.GroupTag != null).Select(x => x.Plot_Id).ToList();
            if (parsed.Length > 1)
            {
                string gplt = parsed[0];
                string blk = parsed[1];
                var res = db.Plots.Where(x => x.Plot_Number.Contains(gplt) && x.Block_Name.Contains(blk) && x.Status == "Available_For_Sale" && !exstngPlts.Contains(x.Id)).Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + "-" + x.Block_Name + " ( " + x.Plot_Location + " )" + " ( " + x.Plot_Size + " )", id = x.Id, size=x.Plot_Size }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = db.Plots.Where(x => x.Plot_Number.Contains(s) && x.Status == "Available_For_Sale" && !exstngPlts.Contains(x.Id)).Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + " - " + x.Block_Name + " ( " + x.Plot_Location + " )" + " ( " + x.Plot_Size + " )", id = x.Id, size= x.Plot_Size }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetPlots_Search(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return Json(new { text = "Select Plot", id = 0 }, JsonRequestBehavior.AllowGet);
            }
            string[] parsed = s.Split(' ');
            if (parsed.Length > 1)
            {
                string gplt = parsed[0];
                string blk = parsed[1];
                var res = db.Plots.Where(x => x.Plot_Number.Contains(gplt) && x.Block_Name.Contains(blk) && x.Status == "Available_For_Sale").Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + "-" + x.Block_Name + " ( " + x.Plot_Location + " )", id = x.Id }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = db.Plots.Where(x => x.Plot_Number.Contains(s) && x.Status == "Available_For_Sale").Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + " - " + x.Block_Name + " ( " + x.Plot_Location + " )", id = x.Id }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveDealerDeal(List<Biding_Reserve_Plots> DealData, string title)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                long grp = new Helpers().RandomNumber();

                DealData.ForEach(x => x.DealDate = DateTime.Now);
                DealData.ForEach(x => x.AddedBy_Id = uid);
                DealData.ForEach(x => x.AddedBy_Name = uname);
                DealData.ForEach(x => x.GroupTag = grp);
                DealData.ForEach(x => x.PlotStatus = "Available");

                db.Biding_Reserve_Plots.AddRange(DealData);
                db.SaveChanges();
                // Add this code for update plot dealership  
                var plotIdsToUpdate = DealData.Select(x => x.Plot_Id).ToList();

                var plotsToUpdate = db.Plots.Where(x => plotIdsToUpdate.Contains(x.Id)).ToList();

                foreach (var plot in plotsToUpdate)
                {
                    var dealDataForPlot = DealData.FirstOrDefault(x => x.Plot_Id == plot.Id);
                    if (dealDataForPlot != null)
                    {
                        plot.Dealership_Id = dealDataForPlot.Dealer_Id;
                        plot.Dealership_Name = dealDataForPlot.DealerName;
                    }
                }

                db.SaveChanges();

                DealerDeal dd = new DealerDeal
                {
                    // Removed SPecial and Development charges from plot deals
                    //DealData.Sum(x => x.SpecialPrefAmount) + DealData.Sum(x => x.DCAmount) +
                    Amount = DealData.Sum(x => x.PlotPrice) +  DealData.Sum(x => x.CommisionAmount),
                    Received = 0,
                    DateOfEntry = DateTime.Now,
                    DealerId = DealData.Select(x => x.Dealer_Id).FirstOrDefault(),
                    DealerName = DealData.Select(x => x.DealerName).FirstOrDefault(),
                    Description = string.Join(" , ", DealData.Select(x => x.PlotNum)),
                    GroupTag = grp,
                    DealTitle = title,
                };
                db.DealerDeals.Add(dd);
                db.SaveChanges();
                var dnam = DealData.Select(x => x.DealerName).FirstOrDefault();
                var did = db.Dealerships.Where(x => x.Dealership_Name == dnam).Select(x => x.Id).FirstOrDefault();
                db.Sp_Add_DealershipComments(did, title + " Deal Processed" , uid, "Dealer_Deal");

                return Json(new Return { Msg = "Deal has been saved successfully.", Status = true });
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.ToString(), "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return Json(new Return { Msg = "Failed to save the deal data. Please try again.", Status = false });
            }
        } 
        public JsonResult SaveDealerDealEdit(List<Biding_Reserve_Plots> DealData, string title)
        {
            try
            {
                db.Sp_Delete_Deal(DealData.Select(x => x.GroupTag).FirstOrDefault());
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                long grp = new Helpers().RandomNumber();

                DealData.ForEach(x => x.DealDate = DateTime.Now);
                DealData.ForEach(x => x.AddedBy_Id = uid);
                DealData.ForEach(x => x.AddedBy_Name = uname);
                DealData.ForEach(x => x.GroupTag = grp);
                DealData.ForEach(x => x.PlotStatus = "Available");

                db.Biding_Reserve_Plots.AddRange(DealData);
                db.SaveChanges();

                DealerDeal dd = new DealerDeal
                {
                    Amount = DealData.Sum(x => x.PlotPrice) + DealData.Sum(x => x.SpecialPrefAmount) + DealData.Sum(x => x.DCAmount) + DealData.Sum(x => x.CommisionAmount),
                    Received = 0,
                    DateOfEntry = DateTime.Now,
                    DealerId = DealData.Select(x => x.Dealer_Id).FirstOrDefault(),
                    DealerName = DealData.Select(x => x.DealerName).FirstOrDefault(),
                    Description = string.Join(" , ", DealData.Select(x => x.PlotNum)),
                    GroupTag = grp,
                    DealTitle = title
                };
                db.DealerDeals.Add(dd);
                db.SaveChanges();
                var dnam = DealData.Select(x => x.DealerName).FirstOrDefault();
                var did = db.Dealerships.Where(x => x.Dealership_Name == dnam).Select(x => x.Id).FirstOrDefault();
                db.Sp_Add_DealershipComments(did, title + " Deal Edited" , uid, "Dealer_Deal");

                return Json(new Return { Msg = "Deal has been edited successfully.", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Return { Msg = "Failed to save the deal data. Please try again.", Status = false });
            }
        }
        [NoDirectAccess] public ActionResult DealershipDeals()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  Dealership Deals Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        [NoDirectAccess] public ActionResult DealershipDealsDetails(long DealerId)
        {
            var deals = db.DealerDeals.Where(x => x.DealerId == DealerId).ToList();
            long?[] grps = deals.Where(x => x.GroupTag != null).Select(x => x.GroupTag).Distinct().ToArray();
            var res = db.Biding_Reserve_Plots.Where(x => grps.Contains(x.GroupTag)).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Deatils of Dealership Deals Page for " + string.Join(",", deals.Select(x=>x.DealerName)), "Read", "Activity_Record", ActivityType.Details_Access.ToString(), DealerId);
            return PartialView(new DealershipDealsDetailsModel { DealPlots = res, Deals = deals });
        }
        [NoDirectAccess] public ActionResult DealershipDealSecurity(long Id)
        {
            var res = db.DealerDeals.Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.Amount = res.BalanceAmount;
            ViewBag.Bank = new SelectList(Nomenclature.Banks().ToList(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Dealership deal Security Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }
        [NoDirectAccess] public ActionResult DealerDealForm(long Id,long? Dealer)
        {
            var res = db.Biding_Reserve_Plots.Where(x => x.GroupTag == Id).ToList();
            Helpers h = new Helpers(Modules.DealersForm, Types.Booking);
            List<Sp_Get_PlotData_Result> pltDetails = new List<Sp_Get_PlotData_Result>();
            ViewBag.DealerName = res.Select(x => x.DealerName).FirstOrDefault();
            foreach (var item in res)
            {
                var pltDets = db.Sp_Get_PlotData(item.Plot_Id).FirstOrDefault();
                pltDetails.Add(pltDets);
                object[] data = new object[4];
                data[0] = res.Select(x => x.DealerName).FirstOrDefault();
                data[1] = pltDets.Plot_No;
                data[2] = pltDets.Plot_Size;
                data[3] = pltDets.Block_Name;
                var QR_Data = h.GenerateQRCode(data);
            }
            if (Dealer == 0)
            {
                DealerFormPrint dfp = new DealerFormPrint();
                dfp.dealer = null;
                dfp.plt = pltDetails;
                return View(dfp);
            }
            else
            {
                var dealer = db.Dealers.Where(x => x.Id == Dealer).FirstOrDefault();

                DealerFormPrint dfp = new DealerFormPrint();
                dfp.dealer = dealer;
                dfp.plt = pltDetails;
                return View(dfp);
            }
           
        }
    
        //public JsonResult DeleteDealerDeal(long grouptag)
        //{
        //    db.Sp_Delete_DealerDeal(grouptag);
        //    var uid = User.Identity.GetUserId<long>();

        //    var dealdet = db.DealerDeals.Where(x => x.GroupTag == grouptag).Select(x => x.DealerName).FirstOrDefault();
        //    var dealnam = db.DealerDeals.Where(x => x.GroupTag == grouptag).Select(x => x.DealTitle).FirstOrDefault();
        //    var did = db.Dealerships.Where(x => x.Dealership_Name == dealdet).Select(x => x.Id).FirstOrDefault();
        //    db.Sp_Add_DealershipComments(did, dealnam + " Deal Deleted", uid, "Dealer_Deal");
        //    return Json(true);
        //}

        [NoDirectAccess] public ActionResult DealerListForPrint(long id,long grp)
        {
            var res = db.Dealers.Where(x => x.Dealership_Id == id).ToList();
            ViewBag.grp = grp;
            return PartialView(res);
        }
		public JsonResult DealerListForSelect(long id)
        {
            var res = db.Dealers.Where(x => x.Dealership_Id == id).ToList();
            
            return Json(res);
        }
        public JsonResult GetDealerResult(long id)
        {
            var res = db.Dealers.Where(x => x.Id == id).FirstOrDefault();
            return Json(res);
        }
        public JsonResult UpdateDealeridofFiles(long[] Ids, long Dealer_Id, decimal Commission)
        {
            var DealerXML = new XElement("Dealers", Ids.Select(x => new XElement("Dealersdata",
            new XAttribute("Id", x)
            ))).ToString(); db.Sp_Update_FilesDealers(DealerXML, Dealer_Id, Commission);
            return Json(true);
        }
        public JsonResult test(long[] Ids, long Dealer_Id, decimal Commission)
        {
            var DealerXML = new XElement("Dealers", Ids.Select(x => new XElement("Dealersdata",
            new XAttribute("Id", x)
            ))).ToString(); db.Sp_Update_FilesDealers(DealerXML, Dealer_Id, Commission);
            return Json(true);
        }
    }
}