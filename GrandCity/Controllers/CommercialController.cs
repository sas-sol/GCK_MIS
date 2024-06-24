using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using MeherEstateDevelopers.Filters;
using Microsoft.Ajax.Utilities;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class CommercialController : Controller
    {
        public string AccountingModule = COA_Mapper_Modules.Commercial.ToString();
        // GET: Commercial
        Grand_CityEntities db = new Grand_CityEntities();

        /// <summary>
        /// Map Ending
        /// </summary>
        /// <returns></returns>
        /// 

        [NoDirectAccess] public ActionResult ComShortResult(long ComId)
        {
            var res1 = db.Sp_Get_CommercialData(ComId).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialLastOwner(ComId).ToList();
            var res = new CommercialDetailData { commercialData = res1, shopOwnersforallt = res2 };
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Plot Details  ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), ComId);
            return PartialView(res);
        }

        [NoDirectAccess] public ActionResult AddShopOrApartment()
        {
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");
            ViewBag.Floor = new SelectList(db.Commercial_FloorsPlan.ToList(), "Id", "Floor");
            return View();
        }
        public JsonResult AddShopCommercial(List<Commercial_Roomss> cr)
        {
            List<string> Names = new List<string>();
            foreach (var i in cr)
            {
                var res = db.Sp_Add_CommercialShops(i.Com_App_Shop_Number, i.Type, i.Area, i.Location, i.Floor_Id, i.Plan_Id, "Available_For_Sale", i.rate_sq_ft * i.Area, i.ExtraAmount, i.Area, i.Project_Name).SingleOrDefault();

                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added New Shop " + i.Com_App_Shop_Number, "Create", "Activity_Record", ActivityType.Commercial.ToString(), userid);
                if (res != 1)
                {
                    Names.Add(i.Com_App_Shop_Number);
                }
            }
            if (Names.Any())
            {
                return Json(Names);
            }
            return Json(true);
        }
        [NoDirectAccess] public ActionResult AllProjects()
        {
            var res = db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult Building(long Id)
        {
            ViewBag.ProjectId = Id;
            return View();
        }
        //[NoDirectAccess] public ActionResult BuildingDetails(long ProjectId)
        //{
        //    var res = db.Sp_Get_Project_Detials(ProjectId).ToList();
        //    return PartialView(res);
        //}
        [NoDirectAccess] public ActionResult BuildingRegistration()
        {
            ViewBag.ProjectId = new SelectList(db.RealEstate_Projects.ToList(), "Id", "Project_Name");
            return View();
        }
        //public JsonResult BuildingFloorRegistration(Level L, List<Commercial_Levels_Shop> CLS)
        //{

        //    Helpers h = new Helpers();
        //    var PicNumber = h.RandomNumber();
        //    if (!string.IsNullOrEmpty(L.Level_Img_Name))
        //    {
        //        if (!Directory.Exists(Server.MapPath("~/Repository/DWGImages/" + PicNumber + "/")))
        //        {
        //            Directory.CreateDirectory(Server.MapPath("~/Repository/DWGImages/" + PicNumber));
        //        }
        //        var pathMain = Path.Combine(Server.MapPath("~/Repository/DWGImages/" + PicNumber + "/"), PicNumber + ".dwg");

        //        var Imgres = h.SaveBase64Image(L.Level_Img_Name, pathMain, PicNumber.ToString());
        //    }
        //    else
        //    {
        //        int index = 0;
        //        foreach (string file in Request.Files)
        //        {
        //            HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
        //            index++;
        //            if (hpf.ContentLength == 0)
        //                continue;
        //            string imageName = Path.GetExtension(hpf.FileName);
        //            if (!Directory.Exists(Server.MapPath("~/Repository/DWGImages/")))
        //            {
        //                Directory.CreateDirectory(Server.MapPath("~/Repository/DWGImages/"));
        //            }
        //            string savedFileName = Path.Combine(Server.MapPath("~/Repository/DWGImages/"), imageName);
        //            hpf.SaveAs(savedFileName);
        //        }
        //    }
        //    string MyDir = "E:\\Management Information System\\Management Information System\\Content\\Building\\img\\";
        //    using (Image image = Image.Load("E:\\Management Information System\\Management Information System\\Repository\\DWGImages\\" + PicNumber + "\\" + PicNumber + ".dwg"))
        //    {
        //        var options = new SvgOptions();
        //        options.ColorType = Aspose.CAD.ImageOptions.SvgOptionsParameters.SvgColorMode.Ycck;
        //        options.TextAsShapes = true;
        //        options.ResolutionSettings.HorizontalResolution = (double)50;
        //        options.ResolutionSettings.VerticalResolution = (double)50;
        //        image.Save(MyDir + PicNumber + ".svg");
        //    }

        //    return Json(true);
        //}
        /// <summary>
        /// Map Ending
        /// </summary>
        /// <returns></returns>
        [NoDirectAccess] public ActionResult CommercialRegisteration()
        {
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.Cities = new SelectList(Nomenclature.Cities(), "Name", "Name");
            var res = (from x in db.Commercial_Installment_Structure
                       select x).AsEnumerable().Select(s => new { Id = s.Plan_Group, Text = s.Plan_Name + " Plan - " + s.TypeName + " - " + s.Advance + " - " + s.Installments, Group = s.Plan_Name }).ToList();
            ViewBag.InstallmentStruct = new SelectList(res, "Id", "Text", "Group", 1);
            return View();
        }
        [NoDirectAccess] public ActionResult GetStructure()
        {
            var res = (from x in db.Commercial_Installment_Structure
                       select x).AsEnumerable().Select(s => new { Id = s.Plan_Group, Text = s.Plan_Name + " Plan - " + s.TypeName + " - " + s.Advance + " - " + s.Installments, Group = s.Plan_Name }).ToList();
            ViewBag.InstallmentStruct = new SelectList(res, "Id", "Text", "Group", 1);
            return PartialView();
        }
        public JsonResult SearchComFile(long ProjectId, string FormId)
        {
            var res = db.Sp_Get_CommercialAppFormDetails(ProjectId, FormId).SingleOrDefault();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Commercial Form Page " + FormId, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), ProjectId);
            if (res != null)
            {
                return Json(res);
            }
            else
            {
                return Json(false);
            }

        }
        public JsonResult SearchComReceipts(string ProjName, string Unit)
        {
            var re = db.Sp_Get_Receipts_Param(ProjName, "BookingToken", Unit).ToList();
            if (re != null)
            {
                return Json(re);
            }
            else
            {
                return Json(false);
            }

        }
        [NoDirectAccess] public ActionResult InstallmetsCategoryCommercial(long Id)
        {
            var res = db.Commercial_Installment_Structure.Where(x => x.Plan_Group == Id).ToList();
            return PartialView(res);
        }
        public JsonResult GetRates(long Id)
        {
            //var comRom = db.Sp_Get_CommercialData(Id).SingleOrDefault();
            var res = db.Plot_Rates.Where(x => x.Plot_Com_Id == Id && x.Module == Modules.CommercialManagement.ToString()).SingleOrDefault();
            return Json(res.Final_Rate);
        }
        [HttpPost]
        public JsonResult CommercialRegisteration(Commercial_Room_Transfer comdata, bool Flag, string ComRom_Num, bool Token, List<ReceiptData> Receiptdata, long PlanId, bool FullPaid, string Image, decimal? DisAmt, decimal? TotalAmnt)
        {
            Helpers h = new Helpers();
            long TransactionId = h.RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            Sp_Add_Receipt_Result res = new Sp_Add_Receipt_Result();
            SmsService smsService = new SmsService();

            List<Commercial_Installments> com_Installments = new List<Commercial_Installments>();
            DateTime a = DateTime.UtcNow;
            // installments createion
            var structure = db.Commercial_Installment_Structure.Where(x => x.Plan_Group == PlanId).ToList();
            var AdvanceContains = structure.Where(x => x.Type == 1).Count();

            // Get the floor name
            var CommercialRoomDetails = db.Sp_Get_CommercialData(comdata.ComRom_Id).SingleOrDefault();

            if (comdata.Owner_Image1 == null)
            {
                comdata.Owner_Image1 = Image;
            }

            //  1 for advance 2 form installments 3 for grand installment 4 for other
            DateTime Granddate = DateTime.UtcNow;
            DateTime PossDate = DateTime.UtcNow;
            DateTime InstDate = DateTime.UtcNow;
            decimal AdAmt = 0, PerMonthCal = 0, Grand = 0, cal = 0, poss = 0, ballpay = 0, confirm = 0;
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
             
                    db.Sp_Add_CommercialComments(Convert.ToInt32(comdata.ComRom_Id), ComRom_Num + " Registered", userid, ActivityType.Record_Upatation.ToString());
                    db.Sp_Add_Activity(userid, Convert.ToInt32(comdata.ComRom_Id) + ComRom_Num + "Register", "Create", "Activity_Record", ActivityType.Commercial.ToString(), userid);
                    if (!FullPaid)
                    {
                        if (AdvanceContains == 1)
                        {
                            var inscounter = structure.Where(x => x.Type == 1).Select(x => x.Installments).SingleOrDefault();
                            if (inscounter != 1)
                            {
                                return Json(false);
                            }
                            //
                            foreach (var str in structure)
                            {  // Advance 
                                if (str.Type == 1)
                                {
                                    AdAmt = Math.Truncate(Convert.ToDecimal(((TotalAmnt / 100) * str.Advance)));
                                    Commercial_Installments fi = new Commercial_Installments()
                                    {
                                        Status = InstallmentPaymentStatus.Paid.ToString(),
                                        Due_Date = a,
                                        Amount = Math.Truncate(AdAmt),
                                        Com_Id = Convert.ToUInt32(comdata.ComRom_Id),
                                        Installment_Name = "Advance",
                                        Installment_Type = 1,
                                    };
                                    com_Installments.Add(fi);
                                    cal += AdAmt - Math.Truncate(AdAmt);
                                }
                                //Confirmation
                                if (str.Type == 7)
                                {
                                    confirm = Math.Truncate(Convert.ToDecimal(((TotalAmnt / 100) * str.Advance)));
                                    a = a.AddDays(45); // For SA Premium Homes
                                    PossDate = a;
                                    InstDate = a;
                                    Commercial_Installments fi = new Commercial_Installments()
                                    {
                                        Status = InstallmentPaymentStatus.Pending.ToString(),
                                        Due_Date = a,
                                        Amount = Math.Truncate(confirm),
                                        Com_Id = Convert.ToUInt32(comdata.ComRom_Id),
                                        Installment_Name = "Confirmation",
                                        Installment_Type = 7,
                                    };
                                    com_Installments.Add(fi);
                                    cal += confirm - Math.Truncate(confirm);
                                }
                                // for installment
                                if (str.Type == 2)
                                {
                                    PerMonthCal = Convert.ToDecimal(((((TotalAmnt) / 100) * str.Advance)));
                                    PerMonthCal = Convert.ToDecimal(PerMonthCal / str.Installments);

                                    for (int i = 1; i <= str.Installments; i++)
                                    {
                                        if (str.After == 1)
                                        {
                                            str.After = 1;
                                        }
                                        int m = Convert.ToInt16((str.After));
                                        a = a.AddMonths(m);
                                        DateTime dt = new DateTime(a.Year, a.Month, 10);
                                        if (i == str.Installments)
                                        {
                                            Granddate = dt;
                                        }
                                        Commercial_Installments fi = new Commercial_Installments()
                                        {
                                            Status = InstallmentPaymentStatus.Pending.ToString(),
                                            Due_Date = dt,
                                            Amount = Math.Truncate(PerMonthCal),
                                            Com_Id = Convert.ToUInt32(comdata.ComRom_Id),
                                            Installment_Name = i + " Monthly Installment",
                                            Installment_Type = 2,
                                        };
                                        com_Installments.Add(fi);
                                        cal += PerMonthCal - Math.Truncate(PerMonthCal);
                                    }
                                }
                                // For grand installments
                                if (str.Type == 3)
                                {
                                    Grand = Convert.ToDecimal(((((TotalAmnt) / 100) * str.Advance)));
                                    Grand = Convert.ToDecimal(Grand / str.Installments);
                                    for (int i = 1; i <= str.Installments; i++)
                                    {

                                        if (str.After == 1)
                                        {
                                            str.After = i;
                                        }

                                        int m = Convert.ToInt16((str.After));
                                        Granddate = a;
                                        a = Granddate.AddMonths(m);
                                        DateTime dt = new DateTime(a.Year, a.Month, 10);

                                        Commercial_Installments fi = new Commercial_Installments()
                                        {
                                            Status = InstallmentPaymentStatus.Pending.ToString(),
                                            Due_Date = dt,
                                            Amount = Math.Truncate(Grand),
                                            Com_Id = Convert.ToUInt32(comdata.ComRom_Id),
                                            Installment_Name = i + " Grand Installment",
                                            Installment_Type = 3,
                                        };
                                        com_Installments.Add(fi);
                                        cal += Grand - Math.Truncate(Grand);
                                    }
                                }
                                //Possession
                                if (str.Type == 4)
                                {
                                    poss = Math.Truncate(Convert.ToDecimal(((TotalAmnt / 100) * str.Advance)));
                                    int m = Convert.ToInt16((str.After));
                                    PossDate = PossDate.AddMonths(m);
                                    DateTime dt = new DateTime(PossDate.Year, PossDate.Month, 10);
                                    Commercial_Installments fi = new Commercial_Installments()
                                    {
                                        Status = InstallmentPaymentStatus.Pending.ToString(),
                                        Due_Date = dt,
                                        Amount = Math.Truncate(poss),
                                        Com_Id = Convert.ToUInt32(comdata.ComRom_Id),
                                        Installment_Name = "Possession",
                                        Installment_Type = 4,
                                    };
                                    com_Installments.Add(fi);
                                    cal += poss - Math.Truncate(poss);
                                }
                                //Balloon Payments
                                if (str.Type == 5)
                                {
                                    ballpay = Convert.ToDecimal(((((TotalAmnt) / 100) * str.Advance)));
                                    ballpay = Convert.ToDecimal(ballpay / str.Installments);
                                    for (int i = 1; i <= str.Installments; i++)
                                    {
                                        if (str.After == 1)
                                        {
                                            str.After = i;
                                        }

                                        int m = Convert.ToInt16((str.After));
                                        Granddate = a;
                                        a = Granddate.AddMonths(m);
                                        var insRes = com_Installments.Where(x => x.Due_Date == new DateTime(a.Year, a.Month, 10)).FirstOrDefault();
                                        while (insRes != null)
                                        {
                                            a = a.AddMonths(1);
                                            insRes = com_Installments.Where(x => x.Due_Date == new DateTime(a.Year, a.Month, 10)).FirstOrDefault();
                                        }
                                        DateTime dt = new DateTime(a.Year, a.Month, 10);

                                        Commercial_Installments fi = new Commercial_Installments()
                                        {
                                            Status = InstallmentPaymentStatus.Pending.ToString(),
                                            Due_Date = dt,
                                            Amount = Math.Truncate(ballpay),
                                            Com_Id = Convert.ToUInt32(comdata.ComRom_Id),
                                            Installment_Name = i + " Balloon Payment",
                                            Installment_Type = 5,
                                        };
                                        com_Installments.Add(fi);
                                        cal += ballpay - Math.Truncate(ballpay);
                                    }
                                }
                                //Skipping Installments
                                if (str.Type == 6)
                                {
                                    PerMonthCal = Convert.ToDecimal(((((TotalAmnt) / 100) * str.Advance)));
                                    PerMonthCal = Convert.ToDecimal(PerMonthCal / str.Installments);
                                    a = InstDate;
                                    for (int i = 1; i <= str.Installments; i++)
                                    {
                                        if (str.After == 1)
                                        {
                                            str.After = 1;
                                        }
                                        int m = Convert.ToInt16((str.After));
                                        a = a.AddMonths(m);
                                        var insRes = com_Installments.Where(x => x.Due_Date == new DateTime(a.Year, a.Month, 10)).FirstOrDefault();
                                        while (insRes != null)
                                        {
                                            a = a.AddMonths(1);
                                            insRes = com_Installments.Where(x => x.Due_Date == new DateTime(a.Year, a.Month, 10)).FirstOrDefault();
                                        }
                                        DateTime dt = new DateTime(a.Year, a.Month, 10);
                                        if (i == str.Installments)
                                        {
                                            Granddate = dt;
                                        }
                                        Commercial_Installments fi = new Commercial_Installments()
                                        {
                                            Status = InstallmentPaymentStatus.Pending.ToString(),
                                            Due_Date = dt,
                                            Amount = Math.Truncate(PerMonthCal),
                                            Com_Id = Convert.ToUInt32(comdata.ComRom_Id),
                                            Installment_Name = i + " Monthly Installment",
                                            Installment_Type = 6,
                                        };
                                        com_Installments.Add(fi);
                                        cal += PerMonthCal - Math.Truncate(PerMonthCal);
                                    }
                                }
                            }
                            var found = com_Installments.LastOrDefault();
                            found.Amount = Math.Round(found.Amount + cal);
                            //
                            //foreach (var r in Receiptdata)
                            //{

                            //}

                            var installmentplan = new XElement("Installments", com_Installments.Select(x => new XElement("Installmentsdata",
                                                     new XAttribute("Amount", x.Amount),
                                                     new XAttribute("Due_Date", x.Due_Date),
                                                     new XAttribute("Installment_Name", x.Installment_Name),
                                                     new XAttribute("Installment_Type", x.Installment_Type),
                                                     new XAttribute("Status", x.Status),
                                                     new XAttribute("Com_Id", x.Com_Id)
                                                     ))).ToString();
                            var insss = Convert.ToBoolean(db.Sp_Add_CommercialInstallmentsPlan(installmentplan, comdata.ComRom_Id, CommercialRoomDetails.Module).FirstOrDefault());
                            //updation of Installmnet plan id
                            db.Sp_Update_CommercialInstallmentPlan(comdata.ComRom_Id, PlanId);
                            db.Sp_Add_CommercialComments(Convert.ToInt32(comdata.ComRom_Id), "Installment Structure Added of" + ComRom_Num, userid, ActivityType.Record_Upatation.ToString());
                        }
                    }

                    var OwnerId = db.Sp_Add_CommercialUserTransfer(comdata.Name, comdata.Father_Husband, comdata.Postal_Address, comdata.Residential_Address, comdata.Phone_Office,
                                                    comdata.Residential, comdata.Mobile_1, comdata.Mobile_2, comdata.Email, comdata.Occupation, comdata.Nationality,
                                                    comdata.Date_Of_Birth, comdata.CNIC_NICOP, comdata.Nominee_Name, comdata.Nominee_Relation, comdata.Nominee_Address,
                                                    comdata.Nominee_CNIC_NICOP, comdata.ComRom_Id, comdata.City, false, Flag, "Owner", comdata.Owner_Image1, comdata.Owner_Image2
                                                    , comdata.Owner_Image3, comdata.Owner_Image4, TransactionId).FirstOrDefault();

                    // Full paid , Advance ,booking, receipt addition
                    List<string> ids = new List<string>();
                    decimal DiscountAmount = 0;
                    foreach (var rd in Receiptdata)
                    {

                        comdata.Father_Husband = GeneralMethods.CharacterConversion(comdata.Father_Husband);
                        comdata.Name = GeneralMethods.CharacterConversion(comdata.Name);
                        comdata.Mobile_1 = GeneralMethods.CharacterConversion(comdata.Mobile_1);
                        string[] s = comdata.Mobile_1.Split(" _ ".ToCharArray());
                        rd.Mobile_1 = s[0];
                        if (FullPaid)
                        {
                            if (DisAmt != null)
                            {
                                db.Sp_Add_Discount(rd.Amount, (((rd.Amount / 100) * DisAmt)), "On Full Payment Discount", comdata.ComRom_Id, Modules.CommercialManagement.ToString(), DateTime.UtcNow);
                                DiscountAmount = Convert.ToDecimal((rd.Amount / 100) * DisAmt);
                                rd.Amount = Convert.ToDecimal((rd.Amount) - ((rd.Amount / 100) * DisAmt));
                            }
                            else
                            {
                                DisAmt = 0;
                                DiscountAmount = 0;
                            }
                            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                            res = db.Sp_Add_Receipt("",rd.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(rd.Amount)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, comdata.Mobile_1
                                                                                         , comdata.Father_Husband, comdata.ComRom_Id, comdata.Name, rd.PaymentType, 0,
                                                                                         CommercialRoomDetails.Project_Name, 0, null, "", ReceiptTypes.Booking.ToString(), comdata.ComRom_Id, userid, ComRom_Num, null, Modules.CommercialManagement.ToString(), null, ComRom_Num, CommercialRoomDetails.Floor, null, OwnerId, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                            {
                                bool headcashier = false;
                                if (User.IsInRole("Head Cashier"))
                                {
                                    headcashier = true;
                                }
                                //Accounting Entries
                                var jvNo = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                                //Sale Registration
                                ah.Register_Plot(TotalAmnt, CommercialRoomDetails.shop_no, CommercialRoomDetails.Type, CommercialRoomDetails.Floor, TransactionId, userid, 1, jvNo, AccountingModule, Convert.ToInt64(CommercialRoomDetails.ProjectId));

                                //Discount Entry
                                if (DiscountAmount > 0)
                                {
                                    var jvNoDis = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                                    ah.DiscountPosting(DiscountAmount, CommercialRoomDetails.shop_no, CommercialRoomDetails.Floor, CommercialRoomDetails.Type, TransactionId, userid, jvNoDis, 1, headcashier, AccountingModule, Convert.ToInt64(CommercialRoomDetails.ProjectId));
                                }
                                //Receivable Entry
                                ah.Receive_Plot_Amount(rd.Amount, CommercialRoomDetails.shop_no, CommercialRoomDetails.Type, CommercialRoomDetails.Floor, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res.Receipt_No, 1, headcashier, AccountingModule, Convert.ToInt64(CommercialRoomDetails.ProjectId));
                            }
                            db.Sp_Add_CommercialComments(comdata.ComRom_Id, "Full paid amount: " + rd.Amount + "" + " by" + comdata.Name.Replace("Ѭ", "-") + " With Discount :" + DisAmt + "%", userid, "Full_Paid_Commercial");
                            if (rd.PaymentType != "Cash")
                            {
                                //  amount clearance wnter 
                                string FileDetails = JsonConvert.SerializeObject(comdata);
                                decimal Totalamt = Receiptdata.Sum(x => x.Amount);
                                var res1 = db.Sp_Add_Amount_Clearance(Totalamt, FileDetails, Modules.CommercialManagement.ToString(), Types.Booking.ToString()).FirstOrDefault();
                                // bank addition
                                var res2 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                    Modules.CommercialManagement.ToString(), Types.Booking.ToString(), res1, rd.PayChqNo, comdata.ComRom_Id, rd.Ch_bk_Pay_Date, ComRom_Num, res.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                                var text = "Dear " + comdata.Name + ",\n\r" +
                                           "A Payment of Rs " + string.Format("{0:n}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for " + ComRom_Num + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing" + " With Discount " +
                                           "With Discount of" + DisAmt + "%";
                                if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                                }
                                string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                                var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                                var Imgres = h.SaveBase64Image(rd.FileImage, pathMain, res2.ToString());
                                try
                                {
                                    smsService.SendMsg(text, rd.Mobile_1);
                                }
                                catch (Exception)
                                {
                                }
                                Transaction.Commit();
                                var data = new { Status = "2", Receiptid = res.Receipt_No, Token = comdata.ComRom_Id };
                                return Json(data);
                            }
                            else
                            {
                                var text = "Dear " + comdata.Name + ",\n\r" +
                                           "A Payment of Rs " + string.Format("{0:n}", rd.Amount) + " has been received in cash for  " + ComRom_Num + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment." +
                                           "With Discount of" + DisAmt + "%";
                                try
                                {
                                    smsService.SendMsg(text, rd.Mobile_1);
                                }
                                catch (Exception)
                                {
                                }
                                Transaction.Commit();
                                var data = new { Status = "1", Receiptid = res.Receipt_No, Token = comdata.ComRom_Id };
                                return Json(data);
                            }
                        }
                        else
                        {
                            if (Token) // Advance 
                            {
                                var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                                res = db.Sp_Add_Receipt(rd.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(rd.Amount)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, comdata.Mobile_1
                                                                          , comdata.Father_Husband, comdata.ComRom_Id, comdata.Name, rd.PaymentType, 0,
                                                                          CommercialRoomDetails.Project_Name, 0, null, CommercialRoomDetails.Area.ToString(), ReceiptTypes.Booking.ToString(), comdata.ComRom_Id, userid, "Commercial Advance", null, Modules.CommercialManagement.ToString(), null, ComRom_Num, CommercialRoomDetails.Floor, null, OwnerId, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                                ids.Add(res.Receipt_No);
                                {
                                    bool headcashier = false;
                                    if (User.IsInRole("Head Cashier"))
                                    {
                                        headcashier = true;
                                    }
                                    //Accounting Entries
                                    var jvNo = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                                    //Sale Registration
                                    ah.Register_Plot(TotalAmnt, CommercialRoomDetails.shop_no, CommercialRoomDetails.Type, CommercialRoomDetails.Floor, TransactionId, userid, 1, jvNo, AccountingModule, Convert.ToInt64(CommercialRoomDetails.ProjectId));
                                    //Receivable Entry
                                    ah.Receive_Plot_Amount(rd.Amount, CommercialRoomDetails.shop_no, CommercialRoomDetails.Type, CommercialRoomDetails.Floor, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res.Receipt_No, 1, headcashier, AccountingModule, Convert.ToInt64(CommercialRoomDetails.ProjectId));
                                }
                            }
                            else // booking
                            {
                                var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                                res = db.Sp_Add_Receipt(rd.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(rd.Amount)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, comdata.Mobile_1
                                                                           , comdata.Father_Husband, comdata.ComRom_Id, comdata.Name, rd.PaymentType, 0,
                                                                           CommercialRoomDetails.Project_Name, 0, null, CommercialRoomDetails.Area.ToString(), ReceiptTypes.Booking.ToString(), comdata.ComRom_Id, userid, "Commercial Booking", null, Modules.CommercialManagement.ToString(), null, ComRom_Num, CommercialRoomDetails.Floor, null, OwnerId, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                                ids.Add(res.Receipt_No);
                                {
                                    bool headcashier = false;
                                    if (User.IsInRole("Head Cashier"))
                                    {
                                        headcashier = true;
                                    }
                                   
                                    var jvNo = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                                    //Sale Registration
                                    ah.Register_Plot(TotalAmnt, CommercialRoomDetails.shop_no, CommercialRoomDetails.Type, CommercialRoomDetails.Floor, TransactionId, userid, 1, jvNo, AccountingModule, Convert.ToInt64(CommercialRoomDetails.ProjectId));
                                    //Receivable Entry
                                    ah.Receive_Plot_Amount(rd.Amount, CommercialRoomDetails.shop_no, CommercialRoomDetails.Type, CommercialRoomDetails.Floor, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res.Receipt_No, 1, headcashier, AccountingModule, Convert.ToInt64(CommercialRoomDetails.ProjectId));
                                }
                            }
                            if (rd.PaymentType != "Cash")
                            {
                                //  amount clearance wnter 
                                //string FileDetails = JsonConvert.SerializeObject(comdata);
                                //decimal Totalamt = Receiptdata.Sum(x => x.Amount);
                                //var res1 = db.Sp_Add_Amount_Clearance(AdAmt, FileDetails, CommercialRoomDetails.Module, Types.Booking.ToString()).FirstOrDefault();
                                // banking addition
                                var res2 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                    Modules.CommercialManagement.ToString(), Types.Booking.ToString(), userid, rd.PayChqNo, comdata.ComRom_Id, rd.Ch_bk_Pay_Date, ComRom_Num, res.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                                var text = "Dear " + comdata.Name + ",\n\r" +
                                          "A Payment of Rs " + string.Format("{0:n}", AdAmt) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for " + ComRom_Num + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";
                                if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                                }
                                string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                                var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                                var Imgres = h.SaveBase64Image(rd.FileImage, pathMain, res2.ToString());
                                try
                                {
                                    smsService.SendMsg(text, rd.Mobile_1);
                                }
                                catch (Exception)
                                {
                                }
                            }
                            else
                            {
                                var text = "Dear " + comdata.Name + ",\n\r" +
                                           "A Payment of Rs " + string.Format("{0:n}", AdAmt) + " has been received in cash for  " + ComRom_Num + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                                try
                                {
                                    smsService.SendMsg(text, rd.Mobile_1);
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        }
                    }
                    Transaction.Commit();
                    var data1 = new { Status = "1", Receiptid = ids, Token = comdata.ComRom_Id };
                    return Json(data1);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return Json(false);
                }
            }
        }
        [NoDirectAccess] public ActionResult CommercialMannualRegisteration()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects().Where(x => x.Type == ProjectCategory.Building.ToString()), "Id", "Project_Name");
            return View();
        }
        public JsonResult GetCommercialFloors(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Floor of the Project Id  <a class='blk-data' data-id=' " + Id + "'>" + Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            var res = db.Commercial_FloorsPlan.Where(x => x.Project_Id == Id).ToList();
            return Json(res);
        }
        public JsonResult GetUnits(long Id, string Type, long Floor)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Units List of Project Id  <a class='blk-data' data-id=' " + Id + "'>" + Id + "</a> of Type " + Type, "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            var res = db.Sp_Get_Commercial_Units_By_Param(Id, Floor, Type).ToList();
            return Json(res);
        }
        [NoDirectAccess] public ActionResult PreviousOwners(long? Shopid)
        {
            var res = db.Commercial_Room_Transfer.Where(x => x.ComRom_Id == Shopid && x.Ownership_Status == "Transfer").ToList();
            return PartialView(res);
        }
        public JsonResult GetCommercial_Rooms(long Id)
        {
            var res = db.Sp_Get_Commercialrooms(Id).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Commercial Rooms of Phase Id  <a class='blk-data' data-id=' " + Id + "'>" + Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            return Json(res);
        }
        public JsonResult GetCommercial_RoomsDetail(long Commercial_Id)
        {
            var res = db.Sp_Get_CommercialData(Commercial_Id).FirstOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail  <a class='blk-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(Commercial_Id));
            return Json(res);
        }
        [HttpPost]
        public JsonResult CommercialMannualRegisteration(Commercial_Room_Transfer comdata, List<PlotsInstallments> PI)
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Added Shop Owner Data of Shop No. <a class='plt-data' data-id=' " + comdata.ComRom_Id + "'>" + comdata.ComRom_Id + "</a> To Name : " + comdata.Name, "Create", Modules.CommercialManagement.ToString(), ActivityType.Add_Plot_Owner.ToString(), comdata.ComRom_Id);
            var Installments = new XElement("Commercial", PI.Select(x => new XElement("Installments",
             new XAttribute("Amount", x.Amount),
             new XAttribute("Due_Date", x.DueDate.ToShortDateString()),
             new XAttribute("Installment_Name", x.InstNo),
             new XAttribute("Installment_Type", ""),
             new XAttribute("Status", "Pending"),
             new XAttribute("Com_Id", comdata.ComRom_Id)
             ))).ToString();
            db.Sp_Add_CommercialRoomTransfer(comdata.Name, comdata.Father_Husband, comdata.Postal_Address, comdata.Residential_Address, comdata.Phone_Office,
                                            comdata.Residential, comdata.Mobile_1, comdata.Mobile_2, comdata.Email, comdata.Occupation, comdata.Nationality,
                                            comdata.Date_Of_Birth, comdata.CNIC_NICOP, comdata.Nominee_Name, comdata.Nominee_Relation, comdata.Nominee_Address,
                                            comdata.Nominee_CNIC_NICOP, comdata.ComRom_Id, comdata.City, false, comdata.DateTime, comdata.Total_Price, comdata.Discount, null, userid, true, null, Installments);
            return Json(true);
        }
        public JsonResult CommercialTransferData(Commercial_Room_Transfer comdata)
        {
            Helpers h = new Helpers();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Transfer Shop with Id <a class='plt-data' data-id=' " + comdata.ComRom_Id + "'>" + comdata.ComRom_Id + "</a> To Name " + comdata.Name, "Update", Modules.CommercialManagement.ToString(), ActivityType.Record_Upatation.ToString(), comdata.ComRom_Id);
            db.Sp_Add_CommercialComments(comdata.ComRom_Id, "New Owner Added Name:" + comdata.Name, userid, ActivityType.Record_Upatation.ToString());
            var res1 = db.Sp_Add_CommercialShopTransfer(comdata.Name, comdata.Father_Husband, comdata.Postal_Address, comdata.Residential_Address, comdata.Phone_Office,
                                           comdata.Residential, comdata.Mobile_1, comdata.Mobile_2, comdata.Email, comdata.Occupation, comdata.Nationality,
                                           comdata.Date_Of_Birth, comdata.CNIC_NICOP, comdata.Nominee_Name, comdata.Nominee_Relation, comdata.Nominee_Address,
                                           comdata.Nominee_CNIC_NICOP, comdata.ComRom_Id, comdata.City, false, comdata.DateTime, h.RandomNumber(), comdata.Owner_Image1, comdata.Owner_Image2
                                           , comdata.Owner_Image3, comdata.Owner_Image4, null);

            return Json(true);
        }
        [NoDirectAccess] public ActionResult UpdateCommercialInformation()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");
            return View();
        }
        [NoDirectAccess] public ActionResult AllotmentInformation()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");
            return View();
        }
        [NoDirectAccess] public ActionResult CommercialOwners(long? Commercial_Id)
        {
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).ToList();
            return PartialView(res2);
        }
        [NoDirectAccess] public ActionResult CommercialUpStatus(long Comid, string Status)
        {
            ViewBag.ComId = Comid;
            ViewBag.Status = Status;
            var res = Enum.GetValues(typeof(PlotsStatus)).Cast<PlotsStatus>().Select(e => new { Value = e, Text = e.ToString().Replace("_", " ") }).ToList();
            ViewBag.ComStatus = new SelectList(res, "Value", "Text");
            return PartialView();
        }
        [NoDirectAccess] public ActionResult CommercialInformation(long Commercial_Id)
        {
            ViewBag.Cities = new SelectList(Nomenclature.Cities(), "Name", "Name");
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail for Update  <a class='blk-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(Commercial_Id));
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).ToList();
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();
            var r = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, Receivedamts, r, Commercial_Id);


            var res5 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();

            var res4 = new CommercialDetailData { Discounts = r, commercialData = res, shopOwnersMultiple = res2, CommercialInstallments = res5, CommercialReceipts = Receivedamts };
            return PartialView(res4);
        }
        [NoDirectAccess] public ActionResult AllotmentCommercialInformation(long? Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail for Update  <a class='blk-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(Commercial_Id));
            var res = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialLastOwner(Commercial_Id).ToList();
            var res5 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).ToList();  /*db.Commercial_Room_Transfer.Where(x => x.ComRom_Id == Commercial_Id).ToList();*/
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();
            var res4 = new CommercialDetailData { commercialData = res, shopOwnersMultiple = res5, shopOwnersforallt = res2, CommercialInstallments = res3, CommercialReceipts = Receivedamts };
            return PartialView(res4);
        }
        [NoDirectAccess] public ActionResult AllotmentCommercialInformationAppr(long? Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail for Update  <a class='blk-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(Commercial_Id));
            var res = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).ToList();/*  Sp_Get_CommercialOwnerDetail(Commercial_Id).ToList();*/
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();
            var res4 = new CommercialDetailData { commercialData = res, shopOwnersMultiple = res2, CommercialInstallments = res3, CommercialReceipts = Receivedamts };
            return View(res4);
        }
        [NoDirectAccess] public ActionResult CommercialInformationMapView(long? Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail for Update  <a class='blk-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(Commercial_Id));
            var res = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).ToList();
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();
            var r = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
            var res4 = new CommercialDetailData { Discounts = r, commercialData = res, shopOwnersMultiple = res2, CommercialInstallments = res3, CommercialReceipts = Receivedamts };
            return PartialView(res4);
        }
        [NoDirectAccess] public ActionResult CommercialInfo()
        {
            return PartialView();
        }
        public JsonResult UpdateCommercialOwnerResult(Commercial_Room_Transfer com)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Update Data of Shop Id <a class='plt-data' data-id=' " + com.ComRom_Id + "'>" + com.ComRom_Id + "</a> ", "Update", Modules.CommercialManagement.ToString(), ActivityType.Record_Upatation.ToString(), com.ComRom_Id);
            db.Sp_Add_CommercialComments(com.ComRom_Id, "Update Commercial Record of '" + com.Name.Replace("Ѭ", "-") + "'", userid, ActivityType.Record_Upatation.ToString());
            db.Sp_Update_CommercialOwnerData(com.Name, com.Father_Husband, com.Postal_Address, com.Residential_Address, com.Phone_Office, com.Residential
                , com.Mobile_1, com.Mobile_2, com.Email, com.Occupation, com.Nationality, com.Date_Of_Birth, com.CNIC_NICOP, com.Nominee_Name, com.Nominee_Relation,
                com.Nominee_Address, com.Nominee_CNIC_NICOP, com.Id, com.City, false, com.DateTime, userid, com.ComRom_Id,com.Ownership_Status);
            return Json(true);
        }
        public JsonResult UpdateCommercialInstallments(List<PlotsInstallments> comIns, long shopid, decimal? shop_Price, decimal? Discount)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var Installments = new XElement("Commercial", comIns.Select(x => new XElement("Installments",
              new XAttribute("Amount", x.Amount),
              new XAttribute("Due_Date", x.DueDate.ToShortDateString()),
              new XAttribute("Installment_Name", x.InstNo),
              new XAttribute("Installment_Type", ""),
              new XAttribute("Status", "Pending"),
              new XAttribute("Com_Id", shopid)
              ))).ToString();

            db.Sp_Update_CommercialInstallments(shopid, shop_Price, Discount, userid, Installments);
            db.Sp_Add_Activity(userid, "Update installment Plan of Shop with id <a class='plt-data' data-id=' " + shopid + "'>" + shopid + "</a>", "Update", Modules.CommercialManagement.ToString(), ActivityType.Plan_Updation.ToString(), shopid);
            // db.SP_Update_PlotVerificationToNull(shopid);

            return Json(true);
        }
        //public JsonResult UpdateCommercialInstallments(long? Plan_Id, long? ShopId, decimal? DisAmt)
        //{

        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res = db.Sp_Get_CommercialData(ShopId).SingleOrDefault();
        //    var structure = db.Commercial_Installment_Structure.Where(x => x.Plan_Group == Plan_Id).ToList();
        //    var ShopRate = db.Plot_Rates.Where(x => x.Plot_Com_Id == ShopId && x.Module == res.Module).SingleOrDefault();
        //    var Gi = structure.Where(x => x.Type == 3).Count();
        //    var Ins = structure.Where(x => x.Type == 2).Count();
        //    var AdvanceContains = structure.Where(x => x.Type == 1).Count();
        //    if (AdvanceContains == 0)
        //    {

        //        decimal PerMonthCal = 0, Grand = 0, DecimalCalculation = 0, HoldVariable = 0, HoldVariable1 = 0,cal = 0;
        //        var GetOverAllCalculation = db.Commercial_Installments.Where(x => x.Com_Id == ShopId).ToList();
        //        decimal GrandInstallment = GetOverAllCalculation.Where(x => x.Installment_Type == 3).Select(x => x.Amount).Sum();
        //        decimal Installmentsum = GetOverAllCalculation.Where(x => x.Installment_Type == 2).Select(x => x.Amount).Sum();
        //        int? TotalNumberOfInstallment = structure.Select(x => x.Installments).Sum();
        //        Convert.ToDecimal((DecimalCalculation * TotalNumberOfInstallment));
        //        List<Commercial_Installments> com_Installments = new List<Commercial_Installments>();
        //        DateTime a = DateTime.UtcNow;
        //        DateTime Granddate = DateTime.UtcNow;
        //        if ( DisAmt == null)
        //        {

        //        }
        //        else
        //        {
        //            HoldVariable = Convert.ToDecimal((((GrandInstallment + Installmentsum) / 100) * DisAmt));
        //            db.Sp_Add_Discount((GrandInstallment + Installmentsum), ((GrandInstallment + Installmentsum) - HoldVariable), "On Installment Adjustment", ShopId, res.Module);
        //        }
        //        foreach (var str in structure)
        //        {
        //            // for installment
        //            if (str.Type == 2)
        //            {
        //                if (HoldVariable != 0)
        //                {
        //                    PerMonthCal = Convert.ToDecimal(((((HoldVariable) / 100) * str.Advance)));
        //                    PerMonthCal = Convert.ToDecimal(PerMonthCal / str.Installments);
        //                }
        //                else
        //                {
        //                    PerMonthCal = Convert.ToDecimal(((((GrandInstallment + Installmentsum) / 100) * str.Advance)));
        //                    PerMonthCal = Convert.ToDecimal(PerMonthCal / str.Installments);
        //                }
        //                for (int i = 1; i <= str.Installments; i++)
        //                {
        //                    if (str.After == 1)
        //                    {
        //                        str.After = 1;
        //                    }
        //                    int m = Convert.ToInt16((str.After));
        //                    a = a.AddMonths(m);
        //                    DateTime dt = new DateTime(a.Year, a.Month, 10);
        //                    if (i == str.Installments)
        //                    {
        //                        Granddate = dt;
        //                    }
        //                    Commercial_Installments fi = new Commercial_Installments()
        //                    {

        //                        Status = InstallmentPaymentStatus.Pending.ToString(),
        //                        Due_Date = dt,
        //                        Amount = Math.Truncate(PerMonthCal),
        //                        Com_Id = Convert.ToUInt32(ShopId),
        //                        Installment_Name = i + " Monthly Installment",
        //                        Installment_Type = 2,
        //                    };
        //                    com_Installments.Add(fi);
        //                    cal += PerMonthCal - Math.Truncate(PerMonthCal);

        //                }
        //            }
        //            // For grand installments
        //            if (str.Type == 3)
        //            {

        //                if (HoldVariable1 != 0)
        //                {
        //                    Grand = Convert.ToDecimal(((((HoldVariable1 / 100) * str.Advance))));
        //                    Grand = Convert.ToDecimal(Grand / str.Installments);
        //                }
        //                else
        //                {
        //                    Grand = Convert.ToDecimal(((((GrandInstallment + Installmentsum) / 100) * str.Advance)));
        //                    Grand = Convert.ToDecimal(Grand / str.Installments);
        //                }

        //                for (int i = 1; i <= str.Installments; i++)
        //                {
        //                    //DateTime a = DateTime.UtcNow;
        //                    if (str.After == 1)
        //                    {
        //                        str.After = i;
        //                    }

        //                    int m = Convert.ToInt16((str.After));
        //                    Granddate = a;
        //                    a = Granddate.AddMonths(m);
        //                    DateTime dt = new DateTime(a.Year, a.Month, 10);
        //                    Commercial_Installments fi = new Commercial_Installments()
        //                    {
        //                        Status = InstallmentPaymentStatus.Pending.ToString(),
        //                        Due_Date = dt,
        //                        Amount = Math.Truncate(Grand),
        //                        Com_Id = Convert.ToUInt32(ShopId),
        //                        Installment_Name = i + " Grand Installment",
        //                        Installment_Type = 3,

        //                    };
        //                    com_Installments.Add(fi);
        //                    cal += Grand - Math.Truncate(Grand);

        //                }
        //            }
        //        }
        //        //var found = com_Installments.Where(x=>x.Amount);
        //        //found.Amount = Math.Round(found.Amount + cal);
        //        var installmentplan = new XElement("Installments", com_Installments.Select(x => new XElement("Installmentsdata",
        //                              new XAttribute("Amount", x.Amount),
        //                              new XAttribute("Due_Date", x.Due_Date),
        //                              new XAttribute("Installment_Name", x.Installment_Name),
        //                              new XAttribute("Installment_Type", x.Installment_Type),
        //                              new XAttribute("Status", x.Status),
        //                              new XAttribute("Com_Id", x.Com_Id)
        //                              ))).ToString();
        //        //var insss = Convert.ToBoolean(db.Sp_Add_CommercialInstallmentsPlan(installmentplan, ShopId).FirstOrDefault());
        //        db.Sp_Update_CommercialInstallmentPlan(ShopId, Plan_Id);
        //        db.Sp_Update_CommercialInstallments(ShopId, userid, DisAmt, userid,installmentplan);
        //        //db.Sp_Add_Activity(userid, "Update installment Plan of Shop with id <a class='plt-data' data-id=' " + ShopId + "'>" + ShopId + "</a>", "Update", ProjectCategory.Commercial.ToString(), ActivityType.Plan_Updation.ToString(), ShopId);
        //        //db.Sp_Add_CommercialComments(ShopId, "Update and apply new installment plan "+ "And plan name is"+ structure.Select(x=>x.Plan_Group).FirstOrDefault(), userid, ActivityType.Record_Upatation.ToString());

        //        return Json(true);
        //    }
        //    else
        //    {
        //        return Json(false);
        //    }
        //}

        [NoDirectAccess] public ActionResult ComUpdateReceipt(long Id)
        {
            return PartialView();
        }
        [NoDirectAccess] public ActionResult ShopUpdateStatus(long ShopId, string Status)
        {
            ViewBag.ShopId = ShopId;
            ViewBag.Status = Status;
            var res = Enum.GetValues(typeof(PlotsStatus)).Cast<PlotsStatus>().Select(e => new { Value = e, Text = e.ToString().Replace("_", " ") }).ToList();
            ViewBag.ShopStatus = new SelectList(res, "Value", "Text");
            return PartialView();
        }
        public JsonResult ShopUpdateStatus1(string Remarks, long Commercial_Id, string ShopStatus)
        {
            var userid = long.Parse(User.Identity.GetUserId());
            if (ShopStatus == PlotsStatus.Available_For_Sale.ToString())
            {
                var res = db.Sp_Get_CommercialData(Commercial_Id).FirstOrDefault();
                if (res.Status == PlotsStatus.Available_For_Sale.ToString())
                {
                    var res5 = new { Status = "You can not Change Shop status from Registered to Available For Sale" };
                    return Json(res5);
                }
                db.Sp_Update_CommercialStatus(PlotsStatus.Available_For_Sale.ToString(), Commercial_Id);
                db.Sp_Add_CommercialComments(Commercial_Id, "Updated to Avaialable for Sale. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            }
            else if (ShopStatus == PlotsStatus.Temporary_Cancelled.ToString())
            {
                db.Sp_Update_CommercialStatus(PlotsStatus.Temporary_Cancelled.ToString(), Commercial_Id);
                db.Sp_Add_Activity(userid, "Updated Status to Temporary Cancelled <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Create", Modules.CommercialManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Commercial_Id);
                db.Sp_Add_CommercialComments(Commercial_Id, "Updated to Temporary Cancelled. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            }
            else if (ShopStatus == PlotsStatus.Cancelled.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Cancel Request <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Create", Modules.CommercialManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Commercial_Id);
                db.Sp_Add_CommercialComments(Commercial_Id, "Requested to Cancel. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                var res1 = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
                var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).FirstOrDefault();
                db.Sp_Update_CommercialStatus(PlotsStatus.Hold.ToString(), Commercial_Id);

                db.Sp_Add_CommercialCancelation_Req(Commercial_Id, res2.Id, userid, Remarks, res1.shop_no + " - " + res1.ApplicationNo , res1.Floor, PlotsStatus.Cancelled.ToString());
                var res5 = new { Status = "Requested" };
                return Json(res5);

            }
            else if (ShopStatus == PlotsStatus.Disputed.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Disputed <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Create", Modules.CommercialManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Commercial_Id);
                db.Sp_Update_CommercialStatus(PlotsStatus.Disputed.ToString(), Commercial_Id);
                db.Sp_Add_CommercialComments(Commercial_Id, "Updated to Disputed. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            }
            else if (ShopStatus == PlotsStatus.Hold.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Hold <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Create", Modules.CommercialManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Commercial_Id);
                db.Sp_Update_CommercialStatus(PlotsStatus.Hold.ToString(), Commercial_Id);
                db.Sp_Add_CommercialComments(Commercial_Id, "Updated to Hold. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                //db.Sp_Update_CommercialOwnershipStatus(Commercial_Id, Ownership_Status.Cancelled.ToString());
            }
            else if (ShopStatus == PlotsStatus.Registered.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Registered <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Create", Modules.CommercialManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Commercial_Id);
                db.Sp_Update_CommercialStatus(PlotsStatus.Registered.ToString(), Commercial_Id);
                db.Sp_Add_CommercialComments(Commercial_Id, "Updated to Registered. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            }
            else if (ShopStatus == PlotsStatus.Repurchased.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Repurchased <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Commercial_Id);
                var res1 = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
                var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).FirstOrDefault();
                db.Sp_Add_CommercialCancelation_Req(Commercial_Id, res2.Id, userid, Remarks, res1.shop_no + " - " + res1.ApplicationNo + " " + res1.Floor, res1.Project_Name, PlotsStatus.Repurchased.ToString());
                db.Sp_Add_CommercialComments(Commercial_Id, "Requested to Repurchased. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                var res5 = new { Status = "Repurchased" };
                return Json(res5);
            }
            return Json(true);
        }
        [NoDirectAccess] public ActionResult CancellationReqs()
        {
            if (User.IsInRole("Commercial Manager"))
            {
                var res = db.Sp_Get_Commercial_Cancelation_Req("Commercial Manager").ToList();
                return View(res);
            }
            else if (User.IsInRole("Finance Manager"))
            {
                var res = db.Sp_Get_Commercial_Cancelation_Req("Finance Manager").ToList();
                return View(res);
            }
            else
            {
                var res = db.Sp_Get_Commercial_Cancelation_Req("Commercial Manager").ToList();
                return View(res);
            }
        }
        [NoDirectAccess] public ActionResult CommercialCancellationDetails(long Commercial_Id, long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get full Details of Plot Id  <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>  ", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Commercial_Id);

            ViewBag.CanId = Id;
            var res7 = db.Commercial_Cancelation_Req.SingleOrDefault(x => x.Id == Id);
            var res1 = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).ToList();
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();
            var r = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
            if (res1.Status == "Registered" || res1.Status == "Temporary_Cancelled")
            {
                UpdatePlotInstallmentStatus(res3, res4, r, Commercial_Id);
            }

            var res5 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var res6 = db.Cancellation_Receipts.Where(x => x.File_Plot_No == Commercial_Id).FirstOrDefault();
            var res = new CommercialDetailData { commercialData = res1, shopOwnersMultiple = res2, CommercialInstallments = res5, CommercialReceipts = res4, Plot_Cancel = res6, Plot_Req = res7 };
            return View(res);
        }
        public JsonResult UpdateCancelStat(long Id, long ComId, string Remarks, bool? Status, decimal? Deduction, decimal? Repurchase)
        {
            string[] ReceiptTypes = { "Booking", "Installment" };
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_CommercialData(ComId).SingleOrDefault();
            if (res1.Status != PlotsStatus.Registered.ToString() && res1.Status != PlotsStatus.Temporary_Cancelled.ToString() && res1.Status != PlotsStatus.Hold.ToString())
            {
                return Json(false);
            }
            var res2 = db.Sp_Get_CommercialLastOwner(ComId).SingleOrDefault();
            var res3 = db.Sp_Get_ReceivedAmounts(ComId, res1.Module).Where(x => ReceiptTypes.Contains(x.Type)).ToList();
            var res7 = db.Sp_Get_CommercialInstallments(ComId).Sum(x => x.Amount);
            var res6 = db.Commercial_Cancelation_Req.SingleOrDefault(x => x.Id == Id).Type;
            var receivedamt = res3.Where(x => (x.Status == "Approved" || x.Status is null)).Sum(x => x.Amount);
            string desc = "";
            desc = string.Join(",", res3.Select(x => x.ReceiptNo));

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res4 = db.Sp_Add_Booking_Cancelation(receivedamt, res2.Mobile_1, res2.Father_Husband, res1.Id, res2.Name, res7, res1.Project_Name,
                                res1.Area.ToString(), res1.Type, userid, userid, desc, res1.Module,
                                res1.shop_no +" - " + res1.ApplicationNo + " - " + res1.Type, res1.Floor, Deduction, Repurchase, res6, res1.Type).FirstOrDefault();
                    db.Sp_Update_CommercialCancelation_Req(Id, ComId, Remarks, Status, "Commercial Manager", userid);

                    Transaction.Commit();
                    var res5 = new { Status = "Requested", ReceiptNo = res4.Receipt_No, ReceiptId = res4.Receipt_Id, Token = userid };
                    return Json(res5);
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(false);
                }
            }
        }
        [NoDirectAccess] public ActionResult ComCancelation()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView();
        }
        public JsonResult FinalComCancelation(ReceiptData rd, bool? Status, string Remarks, string Description, long Id, long ComId, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            long TranId = new Helpers().RandomNumber();
            var res1 = db.Sp_Get_CommercialData(ComId).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialLastOwner(ComId).SingleOrDefault();
            var res5 = db.Cancellation_Receipts.Where(x => x.File_Plot_No == res1.Id && x.Module == Modules.CommercialManagement.ToString()).OrderByDescending(x => x.Id).FirstOrDefault();

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Sp_Update_CommercialCancelation_Req(Id, ComId, Remarks, Status, "Finance Manager", userid);
                    var res = db.Sp_Add_Voucher(res2.Postal_Address, rd.Amount, rd.AmountInWords, rd.Bank, rd.Branch, rd.Ch_bk_Pay_Date, rd.PayChqNo, res2.Mobile_1, "Against Cancellation of " + res1.Type + " no: " + res1.shop_no + " Floor: " + res1.Floor + "`" + Description,
                        res2.Father_Husband, res1.Id, Modules.CommercialManagement.ToString(), res2.Name, rd.PaymentType, res1.Project_Name,
                         "", userid, Payments.Cancellation.ToString(), userid, null, comp.Id).FirstOrDefault();
                    if (rd.PaymentType != "Cash")
                    {
                        if (!Directory.Exists(Server.MapPath("~/Repository/Com_Cancellation_Vouchers/" + "/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Repository/Com_Cancellation_Vouchers/"));
                        }
                        string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                        var pathMain = Path.Combine(Server.MapPath("~/Repository/Com_Cancellation_Vouchers/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                        Helpers h = new Helpers();
                        var Images = h.SaveBase64Image(rd.FileImage, pathMain, res.ToString());
                    }

                    if (Type == "Repurchased")
                    {
                        db.Sp_Update_PlotStatus(ComId, PlotsStatus.Repurchased.ToString());
                        db.Sp_Add_PlotTransfer("", "Meher Estate Developers", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ComId, "", null, "Owner", "", DateTime.Now, TranId).FirstOrDefault();
                    }
                    Transaction.Commit();
                    var fres = new { VoucherId = res.Receipt_Id, Token = userid };
                    return Json(fres);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    EmailService e = new EmailService();
                    e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace + " - " + JsonConvert.SerializeObject(res1) + " - " + JsonConvert.SerializeObject(res2), "taimoor@sasystems.solutions", "Plot Cancellation");
                    return Json(false);
                }
            }
        }
        [NoDirectAccess] public ActionResult SearchResult(string Text, int SearchOption)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return PartialView();
            }
            else
            {
                var res = db.Sp_Get_Commercial_Search(SearchOption, Text).ToList();
                return PartialView(res);
            }
        }
        [NoDirectAccess] public ActionResult CommercialInformationSearch(long Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_CommercialData(Commercial_Id).FirstOrDefault();
            var res2 = db.Sp_Get_CommercialOwners(Commercial_Id).OrderByDescending(x => x.DateTime).ToList();
            var res5 = db.Sp_Get_CommercialLastOwner(Commercial_Id).FirstOrDefault();/*  Sp_Get_CommercialOwnerDetail(Commercial_Id).ToList();*/
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();
            var r = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, Receivedamts, r, Commercial_Id);
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail for Update  <a class='blk-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(Commercial_Id));
            var res7 = db.File_Plot_Balance.Where(x => x.Module == Modules.CommercialManagement.ToString() && x.File_Plot_Id == Commercial_Id).FirstOrDefault();
            var res4 = new CommercialDetailDataForInfo { Discounts = r, commercialData = res, shopOwnersMultiple = res2, shopOwnersforallt = res5, CommercialInstallments = res3, CommercialReceipts = Receivedamts, Balance = res7 };
            return PartialView(res4);
        }
        [NoDirectAccess] public ActionResult DetailInformation(long Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail for Update  <a class='blk-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(Commercial_Id));
            var res = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).ToList();

            var res5 = db.Sp_Get_CommercialLastOwner(Commercial_Id).ToList();/*  Sp_Get_CommercialOwnerDetail(Commercial_Id).ToList();*/
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();

            var Receivedamts = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();
            var r = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, Receivedamts, r, Commercial_Id);
            var res4 = new CommercialDetailData { Discounts = r, commercialData = res, shopOwnersMultiple = res2, shopOwnersforallt = res5, CommercialInstallments = res3, CommercialReceipts = Receivedamts };
            return View(res4);
        }
        public void UpdatePlotInstallmentStatus(List<Sp_Get_CommercialInstallments_Result> inst, List<Sp_Get_ReceivedAmounts_Result> Receipts, List<Discount> Dis, long Com_Id)
        {
            decimal? TotalAmt = 0, AmttoPaid = 0, remamt = 0, TotalAmount = 0;
            string[] Installments = { "Booking", "Installment" };
            TotalAmount = Receipts.Where(x => Installments.Contains(x.Type)  && x.Status == null || x.Status == "Approved").Sum(x => x.Amount);
            var ReceivedAmount = TotalAmount;
            if (Dis.Any())
            {
                TotalAmount += Dis.Sum(x => x.Discount_Amount);
            }
            var voucher = db.Vouchers.Where(x => x.File_Plot_Id == Com_Id && x.Module == Modules.PlotManagement.ToString() && x.Plot_Is_Cancelled == null).ToList();
            if (voucher.Any())
            {
                TotalAmount -= voucher.Sum(x => x.Amount);
            }

            var Actamt = TotalAmount;
            List<AmountToPaidInfo> latpi = new List<AmountToPaidInfo>();

            var c = db.Sp_Update_Commercial_Installments_Status_Pndg(Com_Id);

            foreach (var item1 in inst)
            {
                try
                {
                    AmountToPaidInfo atpi = new AmountToPaidInfo();
                    TotalAmt += item1.Amount;

                    if (Convert.ToInt32(TotalAmt) <= Actamt)
                    {
                        AmttoPaid += item1.Amount;
                        atpi.Id = item1.Id;
                        latpi.Add(atpi);
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                }
            }

            var allids = new XElement("IS", latpi.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();
            remamt = Actamt - AmttoPaid;
            db.Test_UpdateBuilding_installment(allids);

            var curdate = DateTime.Now;
            var res3 = db.Sp_Get_CommercialInstallments(Com_Id).ToList();
            var id = res3.Where(x => x.Due_Date <= curdate && x.Status != "Paid").ToList();
            var nopaidis = new XElement("IS", id.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();

            remamt = remamt - id.Sum(x => x.Amount);

            db.Test_UpdateBuilding_NotPaidinstallment(nopaidis);
            db.Test_updatebalance(remamt, inst.Sum(x => x.Amount), ReceivedAmount, Dis.Sum(x => x.Discount_Amount), Com_Id, Modules.CommercialManagement.ToString(), id.Count(),inst.OrderBy(x=>x.Due_Date).FirstOrDefault().Due_Date, voucher.Sum(x => x.Amount));
        }
        public JsonResult AllotmentLetterStatus(long Id, int Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Request for Verification for Unit Id <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.CommercialManagement.ToString(), ActivityType.Plot_Verified.ToString(), Id);
            db.Sp_Add_CommercialComments(Id, "Request for Verification", userid, ActivityType.Allotment_Letter_Requested.ToString());
            db.Sp_Update_RequestForVerify_Building(Id);
            return Json(true);
        }
        [NoDirectAccess] public ActionResult VerifcationRequests()
        {
            var res = db.Sp_Get_Requested_Verification_Building(1).ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult AllotmentRequest()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Allotment Letter Request", "Read", Modules.CommercialManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            var res = db.Sp_Get_CommercialAllotmentLetterRequests().ToList();
            return View(res);
        }
        // Later use of Possession
        public JsonResult VerificationforAllotment(long Id, long PlotOwnId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Verify Shop Id  <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>  for Allotment Letter", "Update", Modules.CommercialManagement.ToString(), ActivityType.Shop_Transfered.ToString(), Id);
            db.Sp_Add_CommercialComments(Id, "Verify Shop", userid, ActivityType.Shop_Transfered.ToString());
            db.Sp_Update_CommercialVerifiedforAllotment(Id, PlotOwnId);
            return Json(true);
        }
        // Later use of Possession
        public JsonResult Verification(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Verify Shop Id  <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>  for Allotment Letter", "Update", Modules.CommercialManagement.ToString(), ActivityType.Shop_Transfered.ToString(), Id);
            db.Sp_Add_CommercialComments(Id, "Verify Shop", userid, ActivityType.Plot_Verified.ToString());
            db.SP_Update_VerifyStatus(Id, Modules.CommercialManagement.ToString());
            return Json(true);
        }
        [NoDirectAccess] public ActionResult Ledger(long? Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).Where(x => x.Ownership_Status != "Cancelled").OrderBy(x => x.DateTime).Select(x => new { x.Total_Price, x.Discount }).FirstOrDefault();
            var res3 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).Where(x => x.Ownership_Status != "Cancelled").OrderByDescending(x => x.DateTime).Select(x => new { x.Total_Price, x.Discount }).FirstOrDefault();
            try
            {
                var inst = db.Sp_Get_CommercialInstallments(Commercial_Id).Select(x => new PlotStatment
                {
                    Description = x.Installment_Name,
                    Date = x.Due_Date,
                    Debit = x.Amount,
                    Credit = 0,
                }).ToList();
                string[] Type = { "Booking", "Installment" };
                var rece = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
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
                var discount = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).FirstOrDefault();
                var payments = db.Vouchers.Where(x => x.File_Plot_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString() && x.Type == "Receipt_Refund").Select(x => new PlotStatment
                {
                    Receipt_Voucher_No = x.VoucherNo,
                    Debit = x.Amount,
                    Mode = x.PaymentType,
                    Chq_No = x.Ch_Pay_Draft_No,
                    Date = x.DateTime,
                    Bank = x.Bank,
                    RcvdDate = x.DateTime,
                    Type = x.Type,
                    Inst_Status = x.Status
                }).ToList();

                ViewBag.TotalPrice = (res2 == null) ? 0 : res2.Total_Price;
                ViewBag.Discount = (discount == null) ? 0 : discount.Discount_Amount;
                ViewBag.EstMrtVal = (res3 == null) ? 0 : res3.Total_Price;

                List<PlotStatment> Rm = new List<PlotStatment>();
                Rm.AddRange(inst);
                Rm.AddRange(rece);
                Rm.AddRange(payments);
                Rm = Rm.OrderBy(x => x.Date).ToList();
                var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).FirstOrDefault();
                var res = new CommercialLedger { Data = res1, Report = Rm, Discount = discount, Balance = bal };
                return PartialView(res);
            }
            catch (Exception ex)
            {
                EmailService e = new EmailService();
                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace + " - " + JsonConvert.SerializeObject(res1) + " - " + JsonConvert.SerializeObject(res2), "taimoor@sasystems.solutions", "Plot Cancellation");
                return PartialView(null);
            }
        }
        [NoDirectAccess] public ActionResult LedgerdetailReport(long? Commercial_Id)
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Ledger Report of Plot Id <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a> ", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Commercial_Id);

            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).FirstOrDefault();

            var inst = db.Sp_Get_CommercialInstallments(Commercial_Id).Select(x => new PlotStatment
            {
                Description = x.Installment_Name,
                Date = x.Due_Date,
                Debit = x.Amount,
                Credit = 0,
            }).ToList();
            string[] Type = { "Booking", "Installment" };
            var rece = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
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
            var discount = db.Discounts.SingleOrDefault(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString());
            var payments = db.Vouchers.Where(x => x.File_Plot_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString() && x.Type == "Receipt_Refund").Select(x => new PlotStatment
            {
                Receipt_Voucher_No = x.VoucherNo,
                Debit = x.Amount,
                Mode = x.PaymentType,
                Chq_No = x.Ch_Pay_Draft_No,
                Date = x.DateTime,
                Bank = x.Bank,
                RcvdDate = x.DateTime,
                Type = x.Type,
                Inst_Status = x.Status
            }).ToList();

            List<PlotStatment> Rm = new List<PlotStatment>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).FirstOrDefault();
            var res = new CommercialLedger { OwnerData = res2, Data = res1, Report = Rm, Discount = discount, Balance = bal };
            return View(res);
        }
        [NoDirectAccess] public ActionResult PreviousDetailReport(long Commercial_Id, long OwnerId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Ledger Report of Plot Id <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a> ", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Commercial_Id);

            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).Where(x => x.Id == OwnerId).Select(x => new Sp_Get_PlotLastOwner_Result
            {
                Name = x.Name,
                CNIC_NICOP = x.CNIC_NICOP,
                Mobile_1 = x.Mobile_1,
                Postal_Address = x.Postal_Address,
                Total_Price = x.Total_Price,
                Discount = x.Discount
            }).ToList();

            var inst = db.Commercial_Installments.Where(x => x.Com_Id == Commercial_Id && x.Owner_Id == OwnerId && x.Cancelled == true).Select(x => new PlotStatment
            {
                Description = x.Installment_Name,
                Date = x.Due_Date,
                Debit = x.Amount,
                Credit = 0,
            }).ToList();
            string[] Type = { "Booking", "Installment" };
            var rece = db.Sp_Get_ReceivedAmountsCancelOwner(Commercial_Id, Modules.CommercialManagement.ToString(), OwnerId).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
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
            var discount = db.Discounts.SingleOrDefault(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString());

            List<PlotStatment> Rm = new List<PlotStatment>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var res = new CommercialLastOwnerLedger { OwnerData = res2, Data = res1, Report = Rm, Discount = discount };
            return View(res);
        }
        //public JsonResult CommerAllotmentLetter(long Id, string Witness1, string Witness2, string Name, string Designation)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res1 = db.Sp_Get_CommercialData(Id).SingleOrDefault();
        //    var res2 = db.Sp_Get_CommercialLastOwner(Id).SingleOrDefault();
        //    db.Sp_Add_Activity(userid, "Generated a New Allotment Letter of Shop No." + res1.shop_no + " For :" + res2.Name, "Create", ProjectCategory.Commercial.ToString(), ActivityType.Allotment_Letter_Generate.ToString(), Id);
        //    db.Sp_Add_CommercialComments(Id, "Generated a New Allotment Letter of " + res2.Name.Replace("Ѭ", "-"), userid, ActivityType.Allotment_Letter_Generate.ToString());
        //    db.Sp_Update_CommercialAllotmentLetter(res2.Id);

        //    var res = db.Sp_Add_AllotmentLetter(res2.Id.ToString(), res2.Name, res2.Father_Husband, res2.CNIC_NICOP, res2.Postal_Address, res1.Location, null,
        //        res1.Project_Name, res1.shop_no, Convert.ToString(res1.Area), ProjectCategory.Commercial.ToString(), res1.Area.ToString(), Convert.ToString(res1.Area), Witness1, Witness2, userid, res2.Id, res2.Id, res2.Owner_Image1, res2.Owner_Image2, res2.Owner_Image3, res2.Owner_Image4, Name, Designation).FirstOrDefault();
        //    Helpers helpers = new Helpers(Modules.ShopManagement, Types.Plots);

        //    object[] data = new object[6];
        //    data[0] = res2.Name;
        //    data[1] = res1.shop_no;
        //    data[2] = res1.Floor;
        //    data[3] = res1.Area;
        //    data[4] = res1.Area;
        //    data[4] = res1.Project_Name;
        //    var QR_Data = helpers.GenerateQRCode(data);
        //    return Json(res);
        //}
        [NoDirectAccess] public ActionResult AllotmentLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            var res = db.Sp_Get_AllotmentLetter(Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Access Allotment Letter of Shop/ Office/ Apartment No. " + res.Plot_No + " Block No: " + res.Block, "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }
        [NoDirectAccess] public ActionResult ProjectShops(long? Project_Id)
        {
            var res = db.Sp_Get_CommercialProjectShops(Project_Id).ToList();
            return PartialView(res);
        }
        // commercail
        [NoDirectAccess] public ActionResult CommercialUpdation()
        {
            var res = db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList();
            return View(res);
        }
        public JsonResult DeleteReceipt(long Id, long Com_id)
        {
            long userid = int.Parse(User.Identity.GetUserId());
            var rec = db.Receipts.Where(x => x.Id == Id).SingleOrDefault();
            string recedetails = JsonConvert.SerializeObject(rec);

            db.Sp_Add_Activity(userid, "Delete receipt of Plot Id  <a class='plt-data' data-id=' " + Com_id + "'>" + Com_id + "</a>---" + recedetails, "Delete", Modules.CommercialManagement.ToString(), ActivityType.Delete_Receipt.ToString(), Com_id);
            db.Sp_Add_CommercialComments(Com_id, "Delete Receipt of No. : " + rec.ReceiptNo + " Amount: " + rec.Amount + " of " + rec.PaymentType, userid, ActivityType.Delete_Receipt.ToString());
            var r = db.Sp_Delete_Receipt(Id).FirstOrDefault();

            //try
            //{
            //    AccountHandlerController de = new AccountHandlerController();
            //    de.Banking_Receipts_Deletion(Id, userid);
            //}
            //catch (Exception ex)
            //{
            //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //}


            var res = db.Sp_Get_ReceivedAmounts(Com_id, Modules.CommercialManagement.ToString()).ToList();
            db.SP_Update_VerificationToNull(Com_id, Modules.CommercialManagement.ToString());

            return Json(res);
        }
        //
        [NoDirectAccess] public ActionResult CommercialManagementDetails(long? Id)
        {
            var res = db.Sp_Get_CommercialManagementDetail(Id).ToList();
            return PartialView(res);
        }
        public JsonResult CommercialManagementDelete(long? Id)
        {
            var res = db.Sp_Delete_CommercialManagement_Parameter(Id).SingleOrDefault();
            if (res == 1)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }
        public JsonResult DeleteInstallmentStructure(long? PlanId)
        {
            var res = db.Sp_Delete_CommercialInstallment_Parameter(PlanId).SingleOrDefault();
            if (res == 1)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }
        [NoDirectAccess] public ActionResult TranferRequest()
        {
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");
            return View();

        }
        [NoDirectAccess] public ActionResult GetTranferData(long? COmId)
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail for Tranfer <a class='blk-data' data-id=' " + COmId + "'>" + COmId + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(COmId));
            var res = db.Sp_Get_CommercialData(COmId).SingleOrDefault();
            var res2 = db.Sp_Get_TranferRequests(COmId).ToList();
            var res3 = db.Sp_Get_CommercialInstallments(COmId).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(res.Id, Modules.CommercialManagement.ToString()).ToList();
            var res4 = new CommercialDetailData { commercialData = res, TranferRequtedData = res2, CommercialInstallments = res3, CommercialReceipts = Receivedamts };
            return PartialView(res4);

        }
        public JsonResult TranferRecored(long? Ownerid, long? TransferId, long? ComId)
        {
            db.Sp_Update_TrasnferRequest(Ownerid, TransferId, ComId);
            return Json(true);

        }
        [NoDirectAccess] public ActionResult CustomerFile(long Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();

            if (res1.Project_Name != "SA Premium Homes")
            {
                return RedirectToAction("CustomerFileAyanCenter", new { Commercial_Id });
            }
            var r = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).ToList();
            var res2 = db.Sp_Get_CommercialLastOwner(Commercial_Id).ToList();/*  Sp_Get_CommercialOwnerDetail(Commercial_Id).ToList();*/
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();

            var discount = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == res1.Module).ToList();
            UpdatePlotInstallmentStatus(res3, res4, discount, Commercial_Id);

            db.Sp_Add_Activity(userid, "Generate Customer File for Commercial Id  <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>  ", "Create", Modules.CommercialManagement.ToString(), ActivityType.Customer_File.ToString(), Commercial_Id);
            db.Sp_Add_CommercialComments(Commercial_Id, "Generate Customer File for : " + res2.Select(x => x.Name), userid, ActivityType.Customer_File.ToString());
            var res = new CommercialDetailData { Discounts = discount, commercialData = res1, shopOwnersforallt = res2, CommercialInstallments = res3, CommercialReceipts = res4 };
            return View(res);
        }

        [NoDirectAccess] public ActionResult CustomerFileAyanCenter(long Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_CommercialData(Commercial_Id).SingleOrDefault();
            var r = db.Sp_Get_CommercialOwnerDetail(Commercial_Id).OrderByDescending(x => x.DateTime).ToList();
            var res2 = db.Sp_Get_CommercialLastOwner(Commercial_Id).ToList();/*  Sp_Get_CommercialOwnerDetail(Commercial_Id).ToList();*/
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();
            var discount = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, res4, discount, Commercial_Id);
            db.Sp_Add_Activity(userid, "Generate Customer File for Commercial Id  <a class='plt-data' data-id=' " + Commercial_Id + "'>" + Commercial_Id + "</a>  ", "Create", Modules.CommercialManagement.ToString(), ActivityType.Customer_File.ToString(), Commercial_Id);
            db.Sp_Add_CommercialComments(Commercial_Id, "Generate Customer File for : " + res2.Select(x => x.Name).FirstOrDefault(), userid, ActivityType.Customer_File.ToString());
            var res = new CommercialDetailData { Discounts = discount, commercialData = res1, shopOwnersforallt = res2, CommercialInstallments = res3, CommercialReceipts = res4 };
            return View(res);
        }
        public JsonResult UpdateCustomerImg(long id, string img, int cnt)
        {
            try
            {
                var res = db.Commercial_Room_Transfer.Where(x => x.Id == id).FirstOrDefault();

                if (cnt == 1)
                {
                    res.Owner_Image1 = img;
                    db.Commercial_Room_Transfer.Attach(res);
                    db.Entry(res).Property(x => x.Owner_Image1).IsModified = true;
                    db.SaveChanges();
                }
                else if (cnt == 2)
                {
                    res.Owner_Image2 = img;
                    db.Commercial_Room_Transfer.Attach(res);
                    db.Entry(res).Property(x => x.Owner_Image2).IsModified = true;
                    db.SaveChanges();
                }
                else if (cnt == 3)
                {
                    res.Owner_Image3 = img;
                    db.Commercial_Room_Transfer.Attach(res);
                    db.Entry(res).Property(x => x.Owner_Image3).IsModified = true;
                    db.SaveChanges();
                }
                else if (cnt == 4)
                {
                    res.Owner_Image4 = img;
                    db.Commercial_Room_Transfer.Attach(res);
                    db.Entry(res).Property(x => x.Owner_Image4).IsModified = true;
                    db.SaveChanges();
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult PossessionRequests()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult PossessionList(int tp)
        {
            var res = db.Sp_Get_CommercialPossessionsList(tp).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult PossessionWorking(long id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_CommercialData(id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialLastOwner(id).ToList();

            //Helpers helpers = new Helpers(Modules.PlotManagement, Types.DealerRegister);

            //object[] data = new object[6];
            //data[0] = res2.OrderByDescending(x => x.DateTime).Select(x => x.Name).FirstOrDefault();
            //data[1] = res1.shop_no.ToString();
            //data[2] = res1.Floor;
            //data[3] = res1.Project_Name;
            //data[4] = res1.Area;
            //data[5] = res1.Dimension;
            //var QR_Data = helpers.GenerateQRCode(data);

            var res = new CommercialPossessionDataModel { CommercialDetails = res1, CommercialOwnersDetails = res2 };
            return View(res);
        }
        [NoDirectAccess] public ActionResult CommercialBounding(long id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_CommercialData(id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialLastOwner(id).ToList();
            var room = db.Commercial_Rooms.Where(x => x.Id == id).SingleOrDefault();
            var boundings = db.CommercialShopBounds.Where(x => x.RoomId == id).ToList();
            List<Commercial_Rooms> plist = new List<Commercial_Rooms>();

            if (room.Front != null)
            {
                Commercial_Rooms p = new Commercial_Rooms
                {
                    Id = res1.Id,
                    North = room.North,
                    South = room.South,
                    East = room.East,
                    West = room.West,
                    North_East = room.North_East,
                    North_West = room.North_West,
                    South_East = room.South_East,
                    South_West = room.South_West,
                    Front = room.Front,
                };
                plist.Add(p);
            }
            else
            {
                Commercial_Rooms p = new Commercial_Rooms
                {
                    Id = res1.Id,
                    North = room.Front_Side,
                    South = room.Back_Side,
                    East = room.Right_Side,
                    West = room.Left_Side,
                    North_East = room.North_East,
                    North_West = room.North_West,
                    South_East = room.South_East,
                    South_West = room.South_West,
                    Front = "North",
                };
                plist.Add(p);
            }


            //Helpers helpers = new Helpers(Modules.PlotManagement, Types.DealerRegister);

            //object[] data = new object[6];
            //data[0] = res2.OrderByDescending(x => x.DateTime).Select(x => x.Name).FirstOrDefault();
            //data[1] = res1.shop_no.ToString();
            //data[2] = res1.Floor;
            //data[3] = res1.Project_Name;
            //data[4] = res1.Area;
            //data[5] = res1.Dimension;
            //var QR_Data = helpers.GenerateQRCode(data);

            var res = new CommercialPossessionDataModel { CommercialDetails = res1, CommercialOwnersDetails = res2, Positions = plist, ShopBoundings = boundings };
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult UpdateCommercialBounding(long id)
        {
            var res1 = db.Sp_Get_CommercialData(id).SingleOrDefault();
            var room = db.Commercial_Rooms.Where(x => x.Id == id).SingleOrDefault();
            var boundings = db.CommercialShopBounds.Where(x => x.RoomId == id).ToList();
            List<Commercial_Rooms> plist = new List<Commercial_Rooms>();

            if (room.Front != null)
            {
                Commercial_Rooms p = new Commercial_Rooms
                {
                    Id = res1.Id,
                    North = room.North,
                    South = room.South,
                    East = room.East,
                    West = room.West,
                    North_East = room.North_East,
                    North_West = room.North_West,
                    South_East = room.South_East,
                    South_West = room.South_West,
                    Front = room.Front,
                };
                plist.Add(p);
            }
            else
            {
                Commercial_Rooms p = new Commercial_Rooms
                {
                    Id = res1.Id,
                    North = room.Front_Side,
                    South = room.Back_Side,
                    East = room.Right_Side,
                    West = room.Left_Side,
                    North_East = room.North_East,
                    North_West = room.North_West,
                    South_East = room.South_East,
                    South_West = room.South_West,
                    Front = "North",
                };
                plist.Add(p);
            }

            var res = new CommercialPossessionDataModel { CommercialDetails = res1, Positions = plist, ShopBoundings = boundings };
            return PartialView(res);
        }
        public JsonResult DeleteBounding(long id)
        {
            try
            {
                db.Sp_Add_CommercialShopBound(id, null, string.Empty, null, string.Empty, string.Empty, string.Empty, string.Empty);
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult CommercialPositions(long id)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            var res33 = db.Commercial_Rooms.Where(x => x.Id == id).FirstOrDefault();
            var ammendedBlocks = db.RealEstate_Projects.ToList();
            ammendedBlocks.Add(new RealEstate_Projects { Project_Name = "Public Building", Id = -997 });
            ammendedBlocks.Add(new RealEstate_Projects { Project_Name = "Park", Id = -996 });
            ammendedBlocks.Add(new RealEstate_Projects { Project_Name = "Road", Id = -998 });
            ammendedBlocks.Add(new RealEstate_Projects { Project_Name = "Boundary Wall", Id = -999 });
            ViewBag.Block = new SelectList(ammendedBlocks, "Id", "Project_Name");

            if (res33.Front == null)
            {
                Plots_Category res1 = (from pe in db.Commercial_Rooms
                                       join y in db.Plots_Category on pe.Shop_Dimension equals y.Id
                                       where pe.Id == id
                                       select y).SingleOrDefault();
                if (res1 != null)
                {
                    var a = db.Sp_Update_CommerRoomPositions(id, res1.Front_Side, res1.Back_Side, res1.Right_Side, res1.Left_Side, res1.North_East, res1.North_West, res1.South_West, res1.South_East, "North");
                }

                //Plot res2 = (from pe in db.Plots
                //                       where pe.Id == Plotid
                //                       select pe).FirstOrDefault();
                var res2 = db.Commercial_Rooms.Where(x => x.Id == id).SingleOrDefault();

                return PartialView(res2);
            }
            else
            {
                var res2 = db.Commercial_Rooms.Where(x => x.Id == id).SingleOrDefault();

                return PartialView(res2);
            }
        }
        public JsonResult CommercialPositionUpdation(CommercialShopBound p)
        {
            var res = db.Sp_Add_CommercialShopBound(p.RoomId, p.BoundedLocationId, p.BoundedLocationName, p.Project_Id, p.Project_Name, p.Type, p.Shop_No, p.Position).ToString();
            var ow = db.Sp_Get_CommercialLastOwner(p.RoomId).ToList();
            db.Sp_Update_CommercialPossessLtrRqstStatus(ow.FirstOrDefault().GroupTag, 1, string.Empty);
            return Json(true);
        }
        [HttpPost]
        public JsonResult CommercialPossessionGenerate(long roomId, string Check, string mat_altd_plt)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var path = "";
            var ow = db.Sp_Get_CommercialLastOwner(roomId).ToList();
            var plotdata = db.Sp_Get_CommercialData(roomId).FirstOrDefault();
            if (Check != null)
            {
                db.Sp_Update_CommercialPossessLtrRqstStatus(ow.FirstOrDefault().GroupTag, 3, mat_altd_plt);
                path = "~/Repository/PlotsData/Commercials/" + roomId + "/DesignHouse/";
            }
            else
            {
                db.Sp_Update_PossessionLetterRequestStatus(ow.FirstOrDefault().GroupTag, 2, mat_altd_plt);

                path = "~/Repository/PlotsData/Commercials/" + roomId + "/SitePlan/";
            }

            //if (plotdata.Type == "Residential")
            //{
            //    var res1 = Convert.ToBoolean(db.Sp_Add_SubscribeServiceCharges(Plot_id, 26, "Service Charges").FirstOrDefault());

            //}
            //else
            //{
            //    var res1 = Convert.ToBoolean(db.Sp_Add_SubscribeServiceCharges(Plot_id, 27, "Service Charges").FirstOrDefault());

            //}
            int index = 0;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath(path + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath(path + "/"));
                }
                string savedFileName = Path.Combine(Server.MapPath(path + "/"), Convert.ToString(roomId) + ".png");
                hpf.SaveAs(savedFileName);
            }

            db.Sp_Update_CommercialPossessLtrRqstStatus(ow.FirstOrDefault().GroupTag, 2, string.Empty);

            if (Check != null)
            {
                return Json(roomId);

            }
            else
            {
                return Json(true);
            }
        }
        [NoDirectAccess] public ActionResult PossessionLetter(long room)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Commercial_Rooms.Where(x => x.Id == room).FirstOrDefault();
            var res2 = db.CommercialShopBounds.Where(x => x.RoomId == room).ToList();
            var res3 = db.Sp_Get_CommercialLastOwner(room).ToList();
            var res5 = db.Sp_Get_CommercialData(room).SingleOrDefault();
            var res6 = db.Sp_Update_CommercialPossessionDate(res3.FirstOrDefault().GroupTag);
            db.Sp_Update_CommercialPossessLtrRqstStatus(res3.FirstOrDefault().GroupTag, 4, string.Empty);
            //if(res3.Allotted_Mat_Plot_Name is null)
            //{
            //    db.SP_Update_Plot_AlottedForMaterial_ForPosession(altdPlt, res3.Id);
            //    res3.Allotted_Mat_Plot_Name = altdPlt;
            //}
            db.Sp_Add_CommercialComments(room, "Generated a New Possession Letter of " + string.Join(",", res3.Select(x => x.Name)), userid, ActivityType.Possession_Letter_Generated.ToString());
            var res = new CommercialPossessionDataModel { CommercialDetails = res5, CommercialRoomData = res1, ShopBoundings = res2, CommercialOwnersDetails = res3, };

            return View(res);
        }
        public JsonResult RequestForPossession(long room)
        {
            try
            {
                var res = db.Sp_Update_CommercialPossessionRequest(room);
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult CommercialNOC(long Commercial_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_CommercialData(Commercial_Id).FirstOrDefault();
            if (res1.Status != "Registered")
            {
                CommercialNOC Overall = new CommercialNOC();
                ViewBag.OverdueReason = "Shop Status is " + res1.Status;
                return View(Overall);
            }
            var res2 = db.Sp_Get_CommercialLastOwner(Commercial_Id).ToList();
            var res3 = db.Sp_Get_CommercialInstallments(Commercial_Id).ToList();

            var res4 = db.Sp_Get_ReceivedAmounts(Commercial_Id, Modules.CommercialManagement.ToString()).ToList();

            var res5 = db.Discounts.Where(x => x.Module_Id == Commercial_Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, res4, res5, Commercial_Id);

            var TotalInstAmt = res3.Sum(x => x.Amount);
            var ReceAmt = res4.Where(x => x.Status == null || x.Status == "Approved").Sum(x => x.Amount);
            if (res5.Any())
            {
                ReceAmt += res5.Sum(x => x.Discount_Amount);
            }

            if (Math.Round(TotalInstAmt) > Math.Round(Convert.ToDecimal(ReceAmt)))
            {
                CommercialNOC Overall = new CommercialNOC();
                ViewBag.OverdueReason = "Overdue amount of Installments";
                return View(Overall);
            }

            var dis = ((res5.Any()) ? res5.Sum(x => x.Discount_Amount) : 0);
            var DueAmt = TotalInstAmt - ReceAmt - dis;
            DueAmt = (DueAmt < 0) ? 0 : Math.Round(Convert.ToDecimal(DueAmt), 0);

            Helpers h = new Helpers();
            var num = h.RandomNumber().ToString();

            db.Sp_Add_CommercialComments(Commercial_Id, "Generated NOC.", userid, ActivityType.Plot_NOC.ToString());
            CommercialNOC res = new CommercialNOC()
            {
                DateTime = DateTime.UtcNow,
                DueAmt = Convert.ToDecimal(DueAmt),
                Name = string.Join(",", res2.Select(x => x.Name)),
                Floor = res1.Floor,
                Com_Id = res1.Id,
                Com_No = res1.shop_no,
                Serial_No = num,
                Size = res1.Area + " Sq Ft",
                TotalAmt = TotalInstAmt,
                Type = res1.Type,
                Userid = userid,
            };
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            return View(res);
        }
        [NoDirectAccess] public ActionResult CommercialReceivableReport()
        {
            var res = db.Sp_Report_CommercialReceivable(3).ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult CommercialToken()
        {
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.Cities = new SelectList(Nomenclature.Cities(), "Name", "Name");
            return View();
        }
        [HttpPost]
        public JsonResult CommercialTokenRegisteration(decimal? amount, Commercial_Room_Transfer comdata, string ComRom_Num, ReceiptData Receiptdata, bool Token)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);

            Sp_Add_Receipt_Result res = new Sp_Add_Receipt_Result();
            SmsService smsService = new SmsService();
            Helpers h = new Helpers();
            List<Commercial_Installments> com_Installments = new List<Commercial_Installments>();
            DateTime a = DateTime.UtcNow;

            var exp = DateTime.Now;
            if (amount < 50000)
            {
                exp = exp.AddDays(3);
            }
            else if (amount < 100000)
            {
                exp = exp.AddDays(7);
            }
            else
            {
                exp = exp.AddDays(10);
            }

            // Get the floor name
            var CommercialRoomDetails = db.Sp_Get_CommercialData(comdata.ComRom_Id).SingleOrDefault();

            db.Sp_Add_CommercialComments(Convert.ToInt32(comdata.ComRom_Id), CommercialRoomDetails.shop_no + " " + CommercialRoomDetails.ApplicationNo + " - " + CommercialRoomDetails.Type + "Token", userid, ActivityType.Record_Upatation.ToString());

            // Advance ,booking, receipt addition
            comdata.Father_Husband = GeneralMethods.CharacterConversion(comdata.Father_Husband);
            comdata.Name = GeneralMethods.CharacterConversion(comdata.Name);
            comdata.Mobile_1 = GeneralMethods.CharacterConversion(comdata.Mobile_1);
            string[] s = comdata.Mobile_1.Split(" _ ".ToCharArray());
            Receiptdata.Mobile_1 = s[0];
            var leadId = db.Sp_Add_Lead("", userid, CommercialRoomDetails.Floor, comdata.Mobile_1, comdata.Name, comdata.Father_Husband, amount, "Walkin", "", NewLeadsStatus.Token.ToString(), Receiptdata.Project_Name, DateTime.Now).FirstOrDefault();
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
            if (Token) // Booking 
            {
                res = db.Sp_Add_Receipt(amount, GeneralMethods.NumberToWords(Convert.ToInt32(amount)), Receiptdata.Bank, Receiptdata.PayChqNo, Receiptdata.Ch_bk_Pay_Date, Receiptdata.Branch, comdata.Mobile_1
                                                          , comdata.Father_Husband, comdata.ComRom_Id, comdata.Name, Receiptdata.PaymentType, 0,
                                                          Receiptdata.Project_Name, 0, null, "", ReceiptTypes.Booking.ToString(), comdata.ComRom_Id, userid, "Commercial Booking", null
                                                          , Modules.CommercialManagement.ToString(), null, CommercialRoomDetails.shop_no + " " + CommercialRoomDetails.ApplicationNo + " - " + CommercialRoomDetails.Type, CommercialRoomDetails.Floor, null, leadId, h.RandomNumber(), "", receiptno, comp.Id).FirstOrDefault();
                // Double Entry Yok
            }
            else // Token
            {
                res = db.Sp_Add_Receipt(amount, GeneralMethods.NumberToWords(Convert.ToInt32(amount)), Receiptdata.Bank, Receiptdata.PayChqNo, Receiptdata.Ch_bk_Pay_Date, Receiptdata.Branch, comdata.Mobile_1
                                                           , comdata.Father_Husband, comdata.ComRom_Id, comdata.Name, Receiptdata.PaymentType, 0,
                                                           Receiptdata.Project_Name, 0, null, "", ReceiptTypes.BookingToken.ToString(), comdata.ComRom_Id, userid, "Commercial Token", exp
                                                           , Modules.CommercialManagement.ToString(), null, CommercialRoomDetails.shop_no + " " + CommercialRoomDetails.ApplicationNo + " - " + CommercialRoomDetails.Type, CommercialRoomDetails.Floor, null, leadId, h.RandomNumber(), "", receiptno, comp.Id).FirstOrDefault();
                db.Sp_Update_CommercialStatus(PlotsStatus.Token.ToString(), comdata.ComRom_Id);
                // Double Entry
            }
            if (Receiptdata.PaymentType != "Cash")
            {
                //  amount clearance wnter 
                string FileDetails = JsonConvert.SerializeObject(comdata);
                var res1 = db.Sp_Add_Amount_Clearance(amount, FileDetails, Modules.CommercialManagement.ToString(), Types.Booking.ToString()).FirstOrDefault();
                // banking addition
                var res2 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(Receiptdata.Amount, Receiptdata.Bank, Receiptdata.Branch, Receiptdata.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                    Modules.CommercialManagement.ToString(), Types.Booking.ToString(), res1, Receiptdata.PayChqNo, comdata.ComRom_Id, Receiptdata.Ch_bk_Pay_Date, ComRom_Num, res.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                var text = "Dear " + comdata.Name + ",\n\r" +
                          "A Payment of Rs " + string.Format("{0:n}", amount) + " has been received against " + Receiptdata.PaymentType + " No: " + Receiptdata.PayChqNo + " Bank: " + Receiptdata.Bank + " for " + ComRom_Num + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";
                if (!Directory.Exists(Server.MapPath("~/Repositery/Commercial/" + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repositery/Commercial/"));
                }
                string dt = string.Format("{0:d/M/yyyy}", Receiptdata.Ch_bk_Pay_Date);
                var pathMain = Path.Combine(Server.MapPath("~/Repositery/Commercial/" + "/"), Receiptdata.PayChqNo + "_" + Receiptdata.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                var Imgres = h.SaveBase64Image(Receiptdata.FileImage, pathMain, res2.ToString());
                try
                {
                    smsService.SendMsg(text, Receiptdata.Mobile_1);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                var text = "Dear " + comdata.Name + ",\n\r" +
                           "A Payment of Rs " + string.Format("{0:n}", amount) + " has been received in cash for  " + ComRom_Num + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                try
                {
                    smsService.SendMsg(text, Receiptdata.Mobile_1);
                }
                catch (Exception)
                {
                }
            }
            var data1 = new { Status = "1", Receiptid = res.Receipt_No, Token = comdata.ComRom_Id };
            return Json(data1);
        }
        [NoDirectAccess] public ActionResult CommercialFileRegistration()
        {
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.Cities = new SelectList(Nomenclature.Cities(), "Name", "Name");
            var res = (from x in db.Commercial_Installment_Structure
                       select x).AsEnumerable().Select(s => new { Id = s.Plan_Group, Text = s.Plan_Name + " Plan - " + s.TypeName + " - " + s.Advance + " - " + s.Installments, Group = s.Plan_Name }).ToList();
            ViewBag.InstallmentStruct = new SelectList(res, "Id", "Text", "Group", 1);
            return View();
        }
        public JsonResult CommercialUnitDiscount(decimal TotalAmt, decimal DiscountAmt, string Remarks, long Id, DateTime? DiscountDate)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            Helpers h = new Helpers();
            var TransactionId = h.RandomNumber();
            bool headcashier = false;
            if (User.IsInRole("Head Cashier"))
            {
                headcashier = true;
            }
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res = db.Sp_Add_Discount(TotalAmt, DiscountAmt, Remarks, Id, Modules.CommercialManagement.ToString(), DiscountDate);
                    {
                        var comDetails = db.Sp_Get_CommercialData(Id).FirstOrDefault();
                        var jvNo = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                        AccountHandlerController ah = new AccountHandlerController();
                        ah.DiscountPosting(DiscountAmt, comDetails.shop_no, comDetails.Floor, comDetails.Type, TransactionId, userid, jvNo, 1, headcashier, AccountingModule, Convert.ToInt64(comDetails.ProjectId));
                    }
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
        public JsonResult MarkCommercialFileDelivered(long? OwnerId)
        {
            if (OwnerId != null || OwnerId != 0)
            {
                db.Sp_Update_Mark_CommercialFile_Delivered(OwnerId);
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        public void UpdateCommercialUnitsBalance()
        {
            var comUnits = db.Commercial_Rooms.Where(x => x.Status == PlotsStatus.Registered.ToString() || x.Status == PlotsStatus.Temporary_Cancelled.ToString()).ToList();
            foreach (var v in comUnits)
            {
                var res = db.Sp_Get_CommercialData(v.Id).SingleOrDefault();
                var res2 = db.Sp_Get_CommercialOwnerDetail(v.Id).OrderByDescending(x => x.DateTime).ToList();
                var res3 = db.Sp_Get_CommercialInstallments(v.Id).ToList();
                var Receivedamts = db.Sp_Get_ReceivedAmounts(v.Id, Modules.CommercialManagement.ToString()).ToList();
                var r = db.Discounts.Where(x => x.Module_Id == v.Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
                UpdatePlotInstallmentStatus(res3, Receivedamts, r, v.Id);
            }
        }
        [NoDirectAccess] public ActionResult AyanCenterApplicationFormPrint()
        {
            var project = "Ayan Center";
            var res = db.Sp_Get_Commercial_Project_Inventory(project).ToList();
            Helpers h = new Helpers(Modules.ApartmentManagement, Types.Apartment);
            foreach (var v in res)
            {
                object[] data = new object[3];
                data[0] = v.Com_App_Shop_Number;
                data[1] = project;
                data[2] = v.Type;
                var QR_Data = h.SAPremiumGenerateQRCode(data);
            }
            return View(res);
        }
        public JsonResult GetCommercialUnitDetails(long ComId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_CommercialData(ComId).FirstOrDefault();
            var res2 = db.Sp_Get_CommercialLastOwner(ComId).ToList();
            var res3 = db.Sp_Get_CommercialInstallments(ComId).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(ComId, Modules.CommercialManagement.ToString()).ToList();
            var r = db.Discounts.Where(x => x.Module_Id == ComId && x.Module == Modules.CommercialManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, Receivedamts, r, ComId);
            var TransfFee = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Commercial_Transfer_Fee_Config.ToString()).FirstOrDefault();
            var parsedInfo = JsonConvert.DeserializeObject<List<CommercialTransferFeeModel>>(TransfFee.CurrentConfig);
            var blockData = parsedInfo.Where(x => x.ProjectName == res1.Project_Name && x.ComType == res1.Type).FirstOrDefault();
            var bal = db.File_Plot_Balance.Where(x => x.Module == Modules.CommercialManagement.ToString() && x.File_Plot_Id == ComId).FirstOrDefault();
            var alreadyTransList = db.Commercial_Transfer_Request.Where(x => x.Status == 0 && x.Com_Id == res1.Id).ToList();
            db.Sp_Add_Activity(userid, "Get Commercial Rooms Detail for Transfer Request  <a class='blk-data' data-id=' " + ComId + "'>" + ComId + "</a>", "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), Convert.ToInt64(ComId));
            var res = new CommercialTransferRequestdata { Balance = bal, UnitData = res1, UnitOwners = res2, UnitInstalments = res3, UnitReceipts = Receivedamts, UnitDiscount = r, UnitTransferFee = blockData, UnitTransferRequest = alreadyTransList };
            return Json(res);
        }
        [NoDirectAccess] public ActionResult CommercialTransferFeeConfiguration()
        {
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == "Commercial_Transfer_Fee_Config").FirstOrDefault();
            if (res is null)
            {
                //means no configurations exists yet
                //so we're gonna create a new config
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var prjData = db.Sp_Get_Projects_with_Types().Select(x => new CommercialTransferFeeModel
                {
                    ProjectId = x.Id,
                    ProjectName = x.Project_Name,
                    ComType = x.Type,
                    TransferAmountType = "",
                    Dealer = 0,
                    Non_Dealer = 0
                }).ToList();
                MIS_Modules_Configurations mmc = new MIS_Modules_Configurations
                {
                    CurrentConfig = JsonConvert.SerializeObject(prjData),
                    Config_For_Update = null,
                    LastUpdated = DateTime.Now,
                    Module = "Commercial_Transfer_Fee_Config",
                    UpdatedBy_Id = uid,
                    UpdatedBy_Name = uname
                };
                db.MIS_Modules_Configurations.Add(mmc);
                db.SaveChanges();
                return PartialView(prjData);
            }
            else
            {
                //config exists so throw this config to the view directly
                var parsed = JsonConvert.DeserializeObject<List<CommercialTransferFeeModel>>(res.CurrentConfig);
                return PartialView(parsed);
            }
        }
        public JsonResult SaveCommercialTransferFeeConfig(List<CommercialTransferFeeModel> config)
        {
            try
            {
                List<CommercialTransferFeeModel> chkMod = new List<CommercialTransferFeeModel>();
                foreach (var v in config)
                {
                    if (v.ProjectId != 0 && v.ProjectName != null && v.TransferAmountType != "" && v.ComType != "")
                    {
                        chkMod.Add(v);
                    }
                }
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var res = db.MIS_Modules_Configurations.Where(x => x.Module == "Commercial_Transfer_Fee_Config").FirstOrDefault();
                res.CurrentConfig = JsonConvert.SerializeObject(chkMod);
                res.LastUpdated = DateTime.Now;
                res.UpdatedBy_Id = uid;
                res.UpdatedBy_Name = uname;
                db.MIS_Modules_Configurations.Attach(res);
                db.Entry(res).Property(x => x.LastUpdated).IsModified = true;
                db.Entry(res).Property(x => x.CurrentConfig).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Id).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Name).IsModified = true;
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult CommercialMultiOwners(long Commercial_Id)
        {
            var res = db.Sp_Get_CommercialLastOwner(Commercial_Id).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult UpdateInstallmentInfo(long id)
        {
            ViewBag.id = id;
            var res = db.Commercial_Installments.Where(x => x.Com_Id == id).ToList();
            return PartialView(res);
        }
        public JsonResult UpdateInstallmentInfoCommercial(long id, List<Commercial_Installments> installmentData)
        {
            var InstallmentStructureData = new XElement("InstallmentData", installmentData.Select(x => new XElement("InstallmentDataInfo",
                new XAttribute("InstallmentType", x.Installment_Type),
                new XAttribute("InstallmentName", x.Installment_Name),
                new XAttribute("Amount", x.Amount),
                new XAttribute("DueDate", x.Due_Date)))).ToString();


            //var InstallmentStructureData = new XElement("InstallmentData", installmentData.Select(x => new XElement("InstallmentDataInfo",
            //        (x.Installment_Type == 0) ? 0 : new XAttribute("InstallmentType", x.Installment_Type),
            //        (x.Installment_Name == null) ? null : new XAttribute("InstallmentName", x.Installment_Name),
            //        (x.Amount == 0) ? 0 : new XAttribute("Amount", x.Amount),
            //        (x.Due_Date == null) ? null : new XAttribute("DueDate", x.Due_Date)))).ToString();

            var res = db.Sp_Update_Installment_File_Plot_Comm(id, "CommercialManagement", InstallmentStructureData);


            return Json(new { Status = true, Msg = "Updated Succesfully" });

            //foreach(installmentComPlotFileData data in installmentData)
            //{

            //    long inst_Id = data.inst_Id;
            //    string name = data.name;
            //    decimal amount = data.amount;
            //    DateTime dateTime = data.datetime;
            //    db.Sp_Update_Installment_File_Plot_Comm(inst_Id, id, name, amount, dateTime, "CommercialManagement");
            //}
        }

        // Cancellation

        [NoDirectAccess] public ActionResult OverDueShop()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult QualifyingShops()
        {
            var res = db.Sp_Get_OverdueQualifying_Shops().Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Installments = x.Installments,
                Shop_Location = x.Location,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req
            }).ToList();
           
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult FirstWarning(Search_OverDue s)
        {
            var res = db.Sp_Get_FirstWarning_Shop().Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Installments = x.Installments,
                Shop_Location = x.Location,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                FirstNotice = x.First_Notice,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req
            }).ToList();

          
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult SecWarning(Search_OverDue s)
        {
            var res = db.Sp_Get_SecWarning_Shop().Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Installments = x.Installments,
                Shop_Location = x.Location,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                Verification_Req = x.Verification_Req
            }).ToList();

            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult CancelledShopsLetter(Search_OverDue s)
        {
            var res = db.Sp_Get_Cancellation_Shop().Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Installments = x.Installments,
                Shop_Location = x.Location,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice,
                Verification_Req = x.Verification_Req
            }).ToList();
          
            return PartialView(res);
        }
        public JsonResult WarningIssues(long Id, long OwnerId, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res3 = db.Sp_Get_CommercialInstallments(Id).ToList();
            var r = db.Discounts.Where(x => x.Module_Id == Id && x.Module == Modules.CommercialManagement.ToString()).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(Id, Modules.CommercialManagement.ToString()).ToList();

            UpdatePlotInstallmentStatus(res3, Receivedamts, r, Id);

           


            db.Sp_Update_WarningLetterStatus_Shop(Id, OwnerId, Type);
            var res = db.Sp_Get_OverdueQualifying_Shop_Id(Id).Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Shop_Location = x.Location,
                Installments = x.Installments,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice
            }).FirstOrDefault();
            if (Type == "First")
            {
                db.Sp_Add_Activity(userid, "Issued First Warning Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_CommercialComments(Id, "Issued First Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                var text =
                "Respected Customer,\n\r" +
                "Kindly note last date for the submission of your instalment has passed.You are requested to make payment within next 7 days’ time.Failing to do so will be resulting in qualification for cancellation.\n\r" +
                "Therefore; Submit your dues timely to ensure the safety of your property.\n\r" +
                "Best Regards,\n\r" +
                "Meher Estate Developers.\n\r" +
                "042 – 111 724 786\n\r";
                SmsService s = new SmsService();
                s.SendMsg(text, res.Mobile_1);
            }
            else if (Type == "Second")
            {
                db.Sp_Add_Activity(userid, "Issued Second Warning Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_CommercialComments(Id, "Issued Second Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                var text =
              "Respected Customer,\n\r" +
              "This is to inform you that your installment is still pending against your property. A reminder message was also sent to you on (" + string.Format("{0:dd-MMM-yyyy}", res.FirstNotice) + ") along with a letter. You are requested to submit due instalments within next 7 days, otherwise your plot will be cancelled.\n\r" +
              "Best Regards,\n\r" +
              "Meher Estate Developers.\n\r" +
              "042 – 111 724 786\n\r";
                SmsService s = new SmsService();
                s.SendMsg(text, res.Mobile_1);
            }
            else if (Type == "Last")
            {
                db.Sp_Add_Activity(userid, "Issued Cancellation Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_CommercialComments(Id, "Issued Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
                var text =
           "Respected Customer,\n\r" +
           "This is to remind you that your property has been cancelled, because of ample amount due on your side. It has been reminded two times before but despite these reminders, you haven’t paid your remaining amount.\n\r" +
           "You are kindly requested to visit our Head Office and submit your complete documents for the processing of your refund.\n\r" +
           "Best Regards,\n\r" +
           "Meher Estate Developers.\n\r" +
           "042 – 111 724 786\n\r";
                SmsService s = new SmsService();
                s.SendMsg(text, res.Mobile_1);
            }
            else if (Type == "ConfirmLast")
            {
                var res4 = db.Sp_Get_CommercialInstallments(Id).Where(x => x.Installment_Type == 7).FirstOrDefault();
                if (res4.Status == "Paid")
                {
                    return Json(false);
                }


                db.Sp_Add_Activity(userid, "Issued Cancellation Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_CommercialComments(Id, "Issued Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
                var text =
           "Respected Customer,\n\r" +
           "This is to remind you that your property has been cancelled, on account of not paying the confirmation amount which was due after 45 days of booking.\n\r" +
           "You are kindly requested to visit our Head Office and submit your complete documents for the processing of your refund.\n\r" +
           "Best Regards,\n\r" +
           "Meher Estate Developers.\n\r" +
           "042 – 111 724 786\n\r";
                SmsService s = new SmsService();
                s.SendMsg(text, res.Mobile_1);
            }
            return Json(true);
        }
        [NoDirectAccess] public ActionResult FirstWarningLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print First Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", ProjectCategory.Building.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            db.Sp_Add_CommercialComments(Id, "Print First Warning Letter", userid, ActivityType.Warning_Letter.ToString());
            var res = db.Sp_Get_OverdueQualifying_Shop_Id(Id).Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Shop_Location = x.Location,
                Installments = x.Installments,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice,
            }).FirstOrDefault();

            return View(res);
        }
        [NoDirectAccess] public ActionResult SecondWarningLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print Second Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", ProjectCategory.Building.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            db.Sp_Add_CommercialComments(Id, "Print Second Warning Letter", userid, ActivityType.Warning_Letter.ToString());
            var res = db.Sp_Get_OverdueQualifying_Shop_Id(Id).Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Shop_Location = x.Location,
                Installments = x.Installments,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice
            }).FirstOrDefault();
            return View(res);
        }
        [NoDirectAccess] public ActionResult CancellationLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print Cancellation Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", ProjectCategory.Building.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            db.Sp_Add_CommercialComments(Id, "Print Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
            var res = db.Sp_Get_OverdueQualifying_Shop_Id(Id).Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Installments = x.Installments,
                Shop_Location = x.Location,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice
            }).FirstOrDefault();

            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult CancellationLetterConfirm(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print Cancellation Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", ProjectCategory.Building.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            db.Sp_Add_CommercialComments(Id, "Print Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
            var res = db.Sp_Get_OverdueQualifying_Shop_Id(Id).Select(x => new OverdueQualifying_Shops
            {
                Area = x.Area,
                Project = x.Project_Name,
                Floor = x.Floor,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.OverDueAmount,
                Installments = x.Installments,
                Shop_Location = x.Location,
                Shop_No = x.Shop_no,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verify,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice
            }).FirstOrDefault();

            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult ReinstateUnit()
        {
            return PartialView();
        }
        public JsonResult UnitReinstate(long Id, string Remarks)
        {
            var userid = long.Parse(User.Identity.GetUserId());
            var res2 = db.Sp_Get_CommercialLastOwner(Id).ToList();
            db.Sp_Update_CommercialStatus("Registered",Id);
            db.Sp_Add_Activity(userid, "Updated Status to Reinstate Cancelled <a class='fl-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Id);
            db.Sp_Add_CommercialComments(Id, Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            db.Sp_Add_CommercialComments(Id, "Reinstate the unit. \n\r", userid, ActivityType.Plot_Status_Updation.ToString());
            return Json(true);
        }
        public JsonResult WarningIssuesShopsMove(long Id, long OwnerId, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Update_WarningLetterStatus_ShopMove(Id, OwnerId, Type);
            db.Sp_Add_CommercialComments(Id, "Update the Letter Status to " + Type, userid, ActivityType.Plot_Status_Updation.ToString());

            return Json(true);
        }

        public void comup(long ComId)
        {
            var res3 = db.Sp_Get_CommercialInstallments(ComId).ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(ComId, Modules.CommercialManagement.ToString()).ToList();
            var r = db.Discounts.Where(x => x.Module_Id == ComId && x.Module == Modules.CommercialManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, Receivedamts, r, ComId);
        }
    }
}
