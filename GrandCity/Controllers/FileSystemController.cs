using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using MeherEstateDevelopers.Filters;
using Microsoft.Ajax.Utilities;
using System.Threading;
using System.Web.Razor.Tokenizer;
using System.Data.Entity;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class FileSystemController : Controller
    {
        private string AccountingModule = COA_Mapper_Modules.Files_Plots.ToString();
        private Grand_CityEntities db = new Grand_CityEntities();
        // GET: FileSystem
        public ActionResult FileShortResult(string FileNumber)
        {
            var res1 = db.File_Form.Where(x => x.FileFormNumber == FileNumber).FirstOrDefault();
            var res2 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            Sp_Get_FileFormData_Result res = new Sp_Get_FileFormData_Result()
            {
                Id = res1.Id,
                FileFormNumber = res1.FileFormNumber,
                Block = res1.Block,
                Plot_Size = res1.Plot_Size,
                Name = String.Format(",", res2.Select(x => x.Name)),
                Father_Husband = String.Format(",", res2.Select(x => x.Father_Husband)),
            };
            db.Sp_Add_Activity(userid, "Accessed  File Short Details Page For  " + FileNumber, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        public ActionResult Application_Form_GCK()
        {
            return View();
        }
        public ActionResult New_Application_Form_GCK()
        {
            return View();
        }
        public ActionResult AddSecurity()
        {
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects(), "Id", "Project_Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddSecurity(Security s)
        {
            var res = Convert.ToBoolean(db.Sp_Add_SecurityPrice(s.Block, s.Phase, s.Price).FirstOrDefault());
            long userid = long.Parse(User.Identity.GetUserId());
            return Json(res);
        }
        public JsonResult GetFileSecurity(long? Phase, long? Block ,long? Sector)
        {
            var res = db.Sp_Get_File_Security(Phase, Block).SingleOrDefault();
            if (res != null)
            {
                return Json(res);
            }
            else
            {
                return Json(false);
            }
        }
        public ActionResult RegisterFile()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Dealership = new SelectList(db.Dealerships.Where(x => x.Status == "Registered").ToList(), "Id", "Dealership_Name");
            ViewBag.Plot_Prefrence = new SelectList(db.Sp_Get_Plot_Prefrences().ToList(), "Id", "Plot_Prefrence");
            Helpers h = new Helpers();
            ViewBag.grp = h.RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            var a = db.Sp_Add_Activity(userid, "Accessed File Register Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid).ToString();
            return View();
        }
        [HttpPost]
        public ActionResult UploadFiles(FormCollection fc)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    var provider = fc.ToValueProvider();
                    var dt = "";
                    if (provider.ContainsPrefix("fileid"))
                    {
                        dt = provider.GetValue("fileid").AttemptedValue;
                        // using the `dt`
                    }
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        if (!Directory.Exists(Server.MapPath("~/PlotReceipts/Files/" + dt + "/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/PlotReceipts/Files/" + dt));
                        }
                        fname = Path.Combine(Server.MapPath("~/PlotReceipts/Files/" + dt + "/"), fname);
                        file.SaveAs(fname);
                    }

                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        public JsonResult UpdateGroupIds()
        {
            var lst = db.Files_Transfer.Where(x => x.Group_Tag == null).ToList();
            foreach (var own in lst)
            {
                own.Group_Tag = new Helpers().RandomNumber();
                db.Files_Transfer.Attach(own);
                db.Entry(own).Property(x => x.Group_Tag).IsModified = true;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RegisterFile(List<Files_Transfer> filedatas, bool? Flag, string DevCharStatus, string FileFormNumber, List<ReceiptData> Receiptdata, bool? FullPaid, long? DelerId, long? recamt, string payType)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            long Payment_No = 0;
            var comp = ah.Company_Attr(userid);
            List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(filedatas.Select(x => x.File_Form_Id).FirstOrDefault()).ToList();
            string Devchar = "";
            var FileForm = db.File_Form.Where(x => x.FileFormNumber == FileFormNumber).FirstOrDefault();

            Helpers H = new Helpers();
            if (DevCharStatus == "true")
            {
                Devchar = "With Development Charges";
            }
            else if (DevCharStatus == "false")
            {
                Devchar = "Non Development Charges";
            }
            if (!installmentstructure.Any())
            {
                return Json(new Return { Status = false, Msg = "No Installment Plan Found for the Respective File" });
            }
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var filedata in filedatas)
                    {
                        filedata.Total = installmentstructure.FirstOrDefault().Total;
                        filedata.GrandTotal = installmentstructure.FirstOrDefault().Grand_Total;
                        long Owner_Id = Convert.ToInt64(db.Sp_Add_File_group(filedata.Plot_Size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
                            , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
                            filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP,
                            filedata.Plot_Prefrence, filedata.File_Form_Id, filedata.Phase_Id, filedata.Block_Id, filedata.City, "Owner", filedata.Rate, filedata.Total, filedata.GrandTotal, Flag, filedata.Group_Tag).FirstOrDefault());
                    }
                    if (DelerId > 0)
                    {
                        var a = db.File_Form.Where(x => x.FileFormNumber == FileFormNumber).FirstOrDefault();
                        var b = db.Sp_Update_FileForm_dealerId(a.Id, DelerId, "File", null);
                    }
                    var ffid = filedatas.Select(x => x.File_Form_Id).FirstOrDefault();
                    var appdetail = (from x in db.File_Form
                                     join y in db.Dealerships on x.Dealership_Id equals y.Id
                                     where x.Id == ffid
                                     select new { File = x, Dealership = y }).FirstOrDefault();
                    {
                        Dealer_Commession d = new Dealer_Commession()
                        {
                            Module = Modules.FileManagement.ToString(),
                            Com_Type = "Percentage",
                            Com_Maturity = 25,
                            Percentage = appdetail.File.Commession,
                            File_Plot_Id = appdetail.File.Id,
                            Com_Amount = (appdetail.File.Commession * installmentstructure.FirstOrDefault().Total) / 100,
                            Dealer_Id = appdetail.Dealership.Id,
                            Dealer_Name = appdetail.Dealership.Dealership_Name,
                            Plot_No = appdetail.File.FileFormNumber.ToString(),
                            Plot_Type = appdetail.File.Type,
                            Block = appdetail.File.Block,
                            Total_Price = installmentstructure.FirstOrDefault().Total
                        };
                        DealershipController dc = new DealershipController();
                        dc.AddCommession(d);
                    }
                    InstallmentsController ic = new InstallmentsController();
                    var file_Installments = ic.AddInstallmentPlan(installmentstructure, DevCharStatus, Convert.ToInt64(filedatas.Select(x => x.File_Form_Id).FirstOrDefault()), DateTime.UtcNow);
                    if (file_Installments.Status)
                    {
                        db.Sp_Update_FileRate(filedatas.Select(x => x.File_Form_Id).FirstOrDefault(), file_Installments.Rate, file_Installments.Total, file_Installments.Grand_Total);
                        List<string> ids = new List<string>();
                        SmsService smsService = new SmsService();
                        var fileno = db.File_Form.Where(x => x.FileFormNumber == FileFormNumber).FirstOrDefault();
                        if (payType == "DealerAdjustment")
                        {
                            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                            var res = db.Sp_Add_Receipt(recamt, GeneralMethods.NumberToWords((int)recamt), Receiptdata.Select(x => x.Bank).FirstOrDefault(), Receiptdata.Select(x => x.PayChqNo).FirstOrDefault(), Receiptdata.Select(x => x.Ch_bk_Pay_Date).FirstOrDefault(), Receiptdata.Select(x => x.Branch).FirstOrDefault(), string.Join(",", filedatas.Select(x => x.Mobile_1))
                                                , string.Join(",", filedatas.Select(x => x.Father_Husband)), fileno.Id, string.Join(",", filedatas.Select(x => x.Name)), Receiptdata.Select(x => x.PaymentType).FirstOrDefault(), file_Installments.Total,
                                                Receiptdata.Select(x => x.Project_Name).FirstOrDefault(), file_Installments.Rate, null, filedatas.Select(x => x.Plot_Size).FirstOrDefault(), ReceiptTypes.Booking.ToString(), filedatas.Select(x => x.File_Form_Id).FirstOrDefault(), userid, "File Booking", null, Modules.FileManagement.ToString(), Devchar, FileFormNumber, appdetail.File.Block, appdetail.File.Type, filedatas.Select(x => x.Group_Tag).FirstOrDefault(), H.RandomNumber(), appdetail.Dealership.Dealership_Name, receiptno, comp.Id).FirstOrDefault();
                            ids.Add(res.Receipt_No);

                            var dealership = db.Dealerships.Where(x => x.Id == DelerId).FirstOrDefault();
                            var dealers = db.Dealers.Where(x => x.Dealership_Id == dealership.Id).ToList();
                            string desc = "Adjustment Voucher Against the Booking of Plot " + fileno.FileFormNumber + "-" + fileno.Type + " Block No: " + fileno.Block;
                            var res5 = db.Sp_Add_Voucher(dealers.Select(x => x.Address).FirstOrDefault(), recamt, GeneralMethods.NumberToWords((int)recamt), "", "", null, "", string.Join("-", dealers.Select(x => x.Mobile_1).FirstOrDefault()), desc,
                                string.Join("-", dealers.Select(x => x.Name)), dealership.Id, Modules.Dealers.ToString(), dealership.Dealership_Name, "Cash", "",
                            "", userid, Payments.Adjustment.ToString(), userid, null, comp.Id).FirstOrDefault();
                           Payment_No = Convert.ToInt64(res5.Receipt_Id);
                            var a = db.Sp_Add_VoucherDetails(recamt, desc, null, null, null, res5.Receipt_Id).FirstOrDefault();

                            string text = "Dear " + string.Join(",", filedatas.Select(x => x.Name)) + ",\n\r" +
                         "A Payment of Rs " + string.Format("{0:n0}", recamt) + " has been received in cash for File number# " + FileFormNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";

                            try
                            {
                                //smsService.SendMsg(text, Receiptdata.Select(x => x.Mobile_1).FirstOrDefault(), Modules.FileManagement.ToString(), fileno.Id, userid);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else { 
                        foreach (var rd in Receiptdata)
                        {
                            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                            var res = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, string.Join(",", filedatas.Select(x => x.Mobile_1))
                                                , string.Join(",", filedatas.Select(x => x.Father_Husband)), fileno.Id, string.Join(",", filedatas.Select(x => x.Name)), rd.PaymentType, file_Installments.Total,
                                                FileForm.Project, file_Installments.Rate, null, filedatas.Select(x => x.Plot_Size).FirstOrDefault(), ReceiptTypes.Booking.ToString(), filedatas.Select(x => x.File_Form_Id).FirstOrDefault(), userid, "File Booking", null, Modules.FileManagement.ToString(), Devchar, FileFormNumber, appdetail.File.Block, appdetail.File.Type, filedatas.Select(x => x.Group_Tag).FirstOrDefault(), H.RandomNumber(), appdetail.Dealership.Dealership_Name, receiptno, comp.Id).FirstOrDefault();
                            ids.Add(res.Receipt_No);
                            string text = "";
                            if (rd.PaymentType == "Cash")
                            {
                                text = "Dear " + string.Join(",", filedatas.Select(x => x.Name)) + ",\n\r" +
                                 "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in cash for File number# " + rd.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                            }
                            else
                            {
                                var res2 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                    Modules.FileManagement.ToString(), Types.Booking.ToString(), 1, rd.PayChqNo, filedatas.Select(x => x.File_Form_Id).FirstOrDefault(), rd.Ch_bk_Pay_Date, FileFormNumber, res.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                                string Msg = "Dear " + rd.Name + ",\n\r" +
                                        "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for File number# " + rd.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";
                                if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                                }
                                string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                                var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                                try
                                {
                                    var Imgres = H.SaveBase64Image(rd.FileImage, pathMain, res2.ToString());
                                    // smsService.SendMsg(Msg, rd.Mobile_1);
                                }
                                catch (Exception)
                                {
                                }
                            }
                            try
                            {
                                //smsService.SendMsg(text, rd.Mobile_1);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        }
                        Transaction.Commit();
                        var data = new { Status = true, Msg = "File has been Registered", Receiptid = ids, PaymentNo = Payment_No, Token = filedatas.Select(x => x.File_Form_Id).FirstOrDefault() };
                        return Json(data);
                    }
                    else
                    {
                        Transaction.Rollback();
                        var data = new Return { Status = false, Msg = "Installment Plan Has Already Been Generated" };
                        return Json(data);
                    }
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    var data = new Return { Status = false, Msg = "Error Occured" };
                    return Json(data);
                }
            }
        }
        [HttpPost]
        public JsonResult PlotPrefrenceDetails(int Id)
        {
            var res = db.Sp_Get_Plot_Prefrences_Parameters(Id).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  Plot Prefrences Details Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return Json(res);
        }
        public DealerFileForm RegisterFileForm(DealerFileForm item)
        {
            DealerFileForm df = new DealerFileForm()
            {
                Block = item.Block,
                Block_Name = item.Block_Name,
                Dealership_Id = item.Dealership_Id,
                Dealership_Name = item.Dealership_Name,
                Dev_Charges_Id = item.Dev_Charges_Id,
                Dev_Charges_Text = item.Dev_Charges_Text,
                Phase = item.Phase,
                Type = item.Type,
                Commession = item.Commession,
                Sector = item.Sector,
                Phase_Name = item.Phase_Name,
                Plot_Size = item.Plot_Size,
                Security = item.Security,
                Sec_NoSec_Id = item.Sec_NoSec_Id,
                Sec_NoSec_Name = item.Sec_NoSec_Name,
                Group_Id = item.Group_Id,
                Installment_Plan = item.Installment_Plan,
                userid = item.userid
            };
            Helpers helpers = new Helpers(Modules.FileManagement, Types.FileForm);
            var no = helpers.stringRandomNumber();
            var FileFormNo = db.Sp_Add_FileForm(null, item.Plot_Size + " Marla", item.Dealership_Id, item.Security, item.Phase, item.Block,
                (int)FileStatus.Pending, item.Dev_Charges_Id, helpers.GetBool(item.Sec_NoSec_Id), item.Security,
                item.Group_Id, item.Installment_Plan, item.userid, item.Type, item.Commession, item.Block_Name, item.Sector, no).FirstOrDefault();
            string newFileFormno = $"{item.Block_Name}-{FileFormNo.File_Form_Id} ";
            var fileFormToUpdate = db.File_Form.FirstOrDefault(f => f.Id == FileFormNo.Id);
            if (fileFormToUpdate != null)
            {
                fileFormToUpdate.FileFormNumber = newFileFormno;
                db.SaveChanges();
            }

            object[] data = new object[6];
            data[0] = FileFormNo.File_Form_Id;
            data[1] = item.Phase_Name;
            data[2] = (string.IsNullOrEmpty(item.Block_Name)) ? "-" : item.Block_Name + " " + item.Type;
            data[3] = item.Dealership_Name;
            data[4] = item.Plot_Size + " Marla";
            data[5] = "TBA";
            var QR_Data = helpers.GenerateQRCode(data);
            var res = db.Sp_Update_FileForm_QR(QR_Data.Id, FileFormNo.Id);
            df.FileCode = FileFormNo.File_Form_Id;
            return df;
        }
        //create a methode to generate Qr code for all files
        //public ActionResult AddFileQrCode()
        //{
        //    Helpers helpers = new Helpers(Modules.FileManagement, Types.FileForm);
        //    var files = db.File_Form.ToList();
        //    foreach(var item in files)
        //    {
        //        var DealerShip = db.Dealerships.Where(d => d.Id == item.Dealership_Id).FirstOrDefault();
        //        var phase = db.RealEstate_Phases.Where(d => d.Id == item.Phase_Id).FirstOrDefault();
        //        var block = db.RealEstate_Blocks.Where(b => b.Id == item.Block_Id).FirstOrDefault();
        //        object[] data = new object[6];
        //        data[0] = item.FileFormNumber;
        //        data[1] = phase.Phase_Name;
        //        data[2] =  block.Block_Name + " " + item.Type;
        //        data[3] = DealerShip.Dealership_Name;
        //        data[4] = item.Plot_Size + " Marla";
        //        data[5] = "TBA";
        //        var QR_Data = helpers.GenerateQRCode(data);
        //        var res = db.Sp_Update_FileForm_QR(QR_Data.Id, item.Id);
        //    }
        //    return Json("ok", JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult AddFileQrCodes(long Id)
        //{
        //    Helpers helpers = new Helpers(Modules.FileManagement, Types.FileForm);
        //    for (long fileId = Id; fileId <= 8596; fileId++)
        //    {
        //        var file = db.File_Form.Where(f => f.Id == fileId).FirstOrDefault();
        //        //var files = db.File_Form.ToList();
        //        //foreach (var item in files)
        //        //{
        //        var DealerShip = db.Dealerships.Where(d => d.Id == file.Dealership_Id).FirstOrDefault();
        //        var phase = db.RealEstate_Phases.Where(d => d.Id == file.Phase_Id).FirstOrDefault();
        //        var block = db.RealEstate_Blocks.Where(b => b.Id == file.Block_Id).FirstOrDefault();
        //        object[] data = new object[6];
        //        data[0] = file.FileFormNumber;
        //        data[1] = phase.Phase_Name;
        //        data[2] = block.Block_Name + " " + file.Type;
        //        data[3] = DealerShip.Dealership_Name;
        //        data[4] = file.Plot_Size + " Marla";
        //        data[5] = "TBA";
        //        var QR_Data = helpers.GenerateQRCode(data);
        //        var res = db.Sp_Update_FileForm_QR(QR_Data.Id, file.Id);
        //        //}
        //    }
        //    return Json("ok", JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult FiesImageUploader(long ImageId, long File_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Update_PossessionLetterRequestStatus(Plot_id, 2);
            var path = "~/Repository/CustomerImages/" + File_Id;
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
                string savedFileName = Path.Combine(Server.MapPath(path + "/"), Convert.ToString(ImageId) + ".png");
                hpf.SaveAs(savedFileName);
            }
            var res = db.Sp_Update_FileImages(ImageId, File_Id).FirstOrDefault();
            return Json(true);
        }
        public ActionResult ShowFileFormList(List<DealerFileForm> dealerFileForm)
        {
            return PartialView(dealerFileForm);
        }
        public JsonResult GetAppFileFormDetails(string Filefromid)
        {
            Sp_Get_FileAppFormData_Result res = db.Sp_Get_FileAppFormData(Filefromid).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            if (res == null) { return Json(false); }
            FileStatus filestat = (FileStatus)res.Status;
            string status = filestat.ToString();
            List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(res.Id).ToList();
            decimal Advance = installmentstructure.Where(x => x.Installment_Type == "3").Select(x => x.Amount).SingleOrDefault();
            decimal Total = installmentstructure.Where(x => x.Installment_Type != "2").Sum(x => x.Amount);
            var res1 = db.Sp_Get_ReceivedAmounts(res.Id, Modules.FileManagement.ToString());
            decimal discount = 0;
            if (res.Dealership_Name == "SAG Head Office")
            {
                discount = (Total * Convert.ToDecimal(0.10)); //Employee Discount
            }
            if (!res1.Any())
            {
                FileAppFormData fa = new FileAppFormData()
                {
                    Block_Id = res.Block_Id,
                    Block_Name = res.Block_Name,
                    Type = res.Type,
                    Dealership_Name = res.Dealership_Name,
                    Development_Charges = res.Development_Charges.ToString(),
                    Development_Charges_Val = res.Development_Charges,
                    Id = res.Id,
                    Plot_Size = res.Plot_Size,
                    Status = ((FileStatus)res.Status).ToString(),
                    Advance = Advance - discount,
                    Total_Price = Total,
                    Discount = discount
                };
                return Json(fa);
            }
            else
            {
                FileAppFormData fa = new FileAppFormData()
                {
                    Block_Id = res.Block_Id,
                    Block_Name = res.Block_Name,
                    Dealership_Name = res.Dealership_Name,
                    Development_Charges = res.Development_Charges.ToString(),
                    Development_Charges_Val = res.Development_Charges,
                    Id = res.Id,
                    Plot_Size = res.Plot_Size,
                    Status = "Already Received",
                    Advance = Advance - discount,
                    Total_Price = Total,
                    Discount = discount
                };
                return Json(fa);
            }
        }
        public ActionResult GetFileDetails(string FileId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res11 = db.Sp_Get_FileAppFormData(FileId).SingleOrDefault();
            long[] size = { 1, 6 };
            if (size.Contains(res11.Status))
            {
                var res44 = TestAdjustIntallments(res11.Id);
            }
            var res1 = db.Sp_Get_FileAppFormData(FileId).SingleOrDefault();
            var res2 = db.Sp_Get_FileOwnerList(res1.Id).ToList();
            var res3 = db.Sp_Get_FileInstallments(res1.Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).ToList();
            var res5 = db.Discounts.Where(x => x.Module_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).ToList();
            var res6 = db.Vouchers.Where(x => x.File_Plot_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).ToList();
            // surcharge
            var res7= db.Plot_Installments_Surcharge.Where(x => x.Plot_Id == res1.Id && x.Modules == Modules.FileManagement.ToString()).ToList();
            var res5surcharge = db.Plot_Installments_Surcharge.Where(x => x.Plot_Id == res1.Id && x.Cancelled == null && x.Waveoff == null).OrderBy(x => x.DueDate).ToList();
            var res6surcharge = db.Sp_Get_ReceivedAmounts_Surcharge(res1.Id, Modules.FileManagement.ToString()).ToList();
            UpdatePlotInstallmentStatusSurcharge(res5surcharge, res6surcharge, res1.Id);
            var res = new FileDetailData { FileData = res1, FilesOwners = res2, FileInstallments = res3, FileReceipts = res4, Discounts = res5, PlotInstallmentsSurcharge = res7 };
            db.Sp_Add_Activity(userid, "Get full Details of File  <a class='file-data' data-id=' " + FileId + "'>" + FileId + "</a>  ", "Read", Modules.FileManagement.ToString(), ActivityType.Details_Access.ToString(), res11.Id);

            return PartialView(res);
        }
        public void UpdatePlotInstallmentStatusSurcharge(List<Plot_Installments_Surcharge> inst, List<Sp_Get_ReceivedAmounts_Surcharge_Result> Receipts, long? Plotid)
        {
            // db.Test_UpdatePendingPlotinstallmentWht(Plotid);

            decimal? TotalAmt = 0, AmttoPaid = 0, remamt = 0, TotalAmount = 0;

            string[] Type = { "SurCharge" };

            TotalAmount = Receipts.Where(x => Type.Contains(x.Type) /*&& (x.Status == null || x.Status == "Approved")*/).Sum(x => x.Amount);
            //if (Dis.Any())
            //{
            //    TotalAmount += Dis.Sum(x => x.Discount_Amount);
            //}
            var Actamt = TotalAmount;

            List<AmountToPaidInfo> latpi = new List<AmountToPaidInfo>();

            foreach (var item1 in inst)
            {
                AmountToPaidInfo atpi = new AmountToPaidInfo();
                TotalAmt += item1.SurchargeAmount;
                if (Math.Round(Convert.ToDecimal(TotalAmt)) <= Math.Round(Convert.ToDecimal(Actamt)))
                {
                    AmttoPaid += item1.SurchargeAmount;
                    atpi.Id = item1.Id;
                    latpi.Add(atpi);
                }
                else
                {
                    break;
                }
            }

            var allids = new XElement("IS", latpi.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();
            remamt = Actamt - AmttoPaid;
            db.Test_UpdatePlotinstallment_Surcharge(allids);

            var curdate = DateTime.Now;
            var res3 = db.Sp_Get_PlotInstallments_Surcharge(Plotid).ToList();
            var id = res3.Where(x => x.DueDate <= curdate && x.Status != "Paid").ToList();
            var nopaidis = new XElement("IS", id.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();

            remamt = remamt - id.Sum(x => x.Amount);

            db.Test_UpdatePlotsNotPaidinstallment_Surcharge(nopaidis);
            //  db.Test_updatebalanceWht(remamt, inst.Sum(x => x.Amount), TotalAmount, Plotid, Modules.PlotManagement.ToString(), id.Count(), 0, 0, 0, 0, 0, 0);

        }

        public ActionResult GenerateCustomerFile(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var resd = db.Sp_Update_CustomerFileRequest(Id);
            var resdd = db.Files_Transfer.Where(x => x.Id == Id).Select(x => x.File_Form_Id).SingleOrDefault();
            var FileId = db.File_Form.Where(x => x.Id == resdd).Select(x => new { x.Id, x.FileFormNumber, x.Installment_Plan_Id, x.Type }).SingleOrDefault();
            var allOwns = db.Files_Transfer.Where(x => x.File_Form_Id == resdd).Select(x => x.Group_Tag).ToList();
            if (FileId.Type == "Commercial")
            {
                return RedirectToAction("GenerateCustomerFile_Com", new { Id = Id });
            }
            var res1 = db.Sp_Get_FileAppFormData(FileId.FileFormNumber).SingleOrDefault();
            if (res1 != null && !string.IsNullOrEmpty(res1.Block_Name))
            {
                res1.Block_Name = res1.Block_Name.Trim();
            }
            long[] size = { 1, 6 };
            if (size.Contains(res1.Status))
            {
                var res44 = TestAdjustIntallments(FileId.Id);
            }
            var instStruc = db.Installment_Structure.Where(x => x.Installment_Plot_Id == FileId.Installment_Plan_Id).ToList();
            var res2 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
            if (res2.Any(x => x.IsCompanyProperty == true))
            {
                return RedirectToAction("GenerateCompanyCustomerFile", new { Id = Id });
            }
            var res3 = db.Sp_Get_FileInstallments(res1.Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(FileId.Id, Modules.FileManagement.ToString()).ToList();
            allOwns.RemoveAll(x => x == res2.Select(y => y.Group_Tag).FirstOrDefault());
            ViewBag.isTrans = allOwns.GroupBy(x => x).Count() > 0;
            var res = new CustomerFileDetailData { FileData = res1, FilesOwners = res2, FileInstallments = res3, FileReceipts = res4, InstalmentStructureDetail = instStruc };
            return View(res);
        }
        public ActionResult GenerateCompanyCustomerFile(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var resd = db.Sp_Update_CustomerFileRequest(Id);
            var resdd = db.Files_Transfer.Where(x => x.Id == Id).Select(x => x.File_Form_Id).SingleOrDefault();
            var FileId = db.File_Form.Where(x => x.Id == resdd).Select(x => new { x.Id, x.FileFormNumber, x.Installment_Plan_Id, x.Type }).SingleOrDefault();
            var res1 = db.Sp_Get_FileAppFormData(FileId.FileFormNumber).SingleOrDefault();
            long[] size = { 1, 6 };
            if (size.Contains(res1.Status))
            {
                var res44 = TestAdjustIntallments(FileId.Installment_Plan_Id);
            }
            var instStruc = db.Installment_Structure.Where(x => x.Installment_Plot_Id == FileId.Installment_Plan_Id).ToList();
            var res2 = db.Sp_Get_FileOwnerList(res1.Id).ToList();
            var res3 = db.Sp_Get_FileInstallments(res1.Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(FileId.Id, Modules.FileManagement.ToString()).ToList();
            var res = new FileDetailData { FileData = res1, FilesOwners = res2, FileInstallments = res3, FileReceipts = res4, InstalmentStructureDetail = instStruc };
            return View(res);
        }
        //[Route("FileSystem/GenerateCustomerFile_Com/{Id:long}")]
        public ActionResult GenerateCustomerFile_Com(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var resd = db.Sp_Update_CustomerFileRequest(Id);
            var resdd = db.Files_Transfer.Where(x => x.Id == Id).Select(x => x.File_Form_Id).SingleOrDefault();
            var FileId = db.File_Form.Where(x => x.Id == resdd).SingleOrDefault();
            var allOwns = db.Files_Transfer.Where(x => x.File_Form_Id == resdd).Select(x => x.Group_Tag).ToList();
            var res1 = db.Sp_Get_FileAppFormData(FileId.FileFormNumber).SingleOrDefault();
            long[] size = { 1, 6 };
            if (size.Contains(res1.Status))
            {
                var res44 = TestAdjustIntallments(FileId.Id);
            }
            var instStruc = db.Installment_Structure.Where(x => x.Installment_Plot_Id == FileId.Installment_Plan_Id).ToList();
            var res2 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
            //if (res2.Any(x => x.IsCompanyProperty == true))
            //{
            //    return RedirectToAction("GenerateCompanyCustomerFile", new { Id = Id });
            //}
            var res3 = db.Sp_Get_FileInstallments(res1.Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).ToList();
            allOwns.RemoveAll(x => x == res2.Select(y => y.Group_Tag).FirstOrDefault());
            ViewBag.isTrans = allOwns.GroupBy(x => x).Count() > 0;
            var res = new CustomerFileDetailData { FileData = res1, FilesOwners = res2, FileInstallments = res3, FileReceipts = res4, InstalmentStructureDetail = instStruc };
            return View(res);
        }
        public ActionResult AllFiles()
        {
            var res = db.Sp_Get_Files().ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed All Files  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(res);
        }
        [Authorize(Roles = "File Information,Administrator")]
        public ActionResult FileInformation()
        {
            return View();
        }
        public ActionResult FileDelivery()
        {
            return View();
        }
        public JsonResult GetFileDelivery(string Filefromid)
        {
            var res1 = db.File_Form.Where(x => x.FileFormNumber == Filefromid).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed File Delivery  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), res1.Id);
            if (res1 == null) { return Json(false); }
            List<Sp_Get_FileInstallments_Result> Allinstallments = db.Sp_Get_FileInstallments(res1.Id).ToList();
            if (Allinstallments.Any())
            {
                this.TestAdjustIntallments(res1.Id);
            }
            var res = db.Sp_Get_FileFormData(res1.Id).SingleOrDefault();
            var installments = Allinstallments.Where(x => x.Installment_Type == "1").ToList();
            var otherinstallments = Allinstallments.Where(x => x.Installment_Type != "1").ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).Where(x => x.Type == "Booking" || x.Type == "Installment" || x.Type == "Adjusted").ToList();
            FileStatus filestat = (FileStatus)res.Status;
            string status = filestat.ToString();
            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Dealership_Name = res.Dealership_Name,
                Development_Charges = res.Development_Charges.ToString(),
                Development_Charges_Val = res.Development_Charges,
                Phase_Name = res.Phase,
                Plot_Size = res.Plot_Size,
                Project_Name = res.Project,
                Status = ((FileStatus)res.Status).ToString(),
                City = res.City,
                CNIC_NICOP = res.CNIC_NICOP,
                Date_Of_Birth = res.Date_Of_Birth,
                Email = res.Email,
                Father_Husband = res.Father_Husband,
                Mobile_1 = res.Mobile_1,
                Mobile_2 = res.Mobile_2,
                Name = res.Name,
                Nationality = res.Nationality,
                Nominee_Address = res.Nominee_Address,
                Nominee_CNIC_NICOP = res.Nominee_CNIC_NICOP,
                Nominee_Name = res.Nominee_Name,
                Nominee_Relation = res.Nominee_Relation,
                Occupation = res.Occupation,
                Phone_Office = res.Phone_Office,
                Postal_Address = res.Postal_Address,
                Residential = res.Residential,
                Residential_Address = res.Residential_Address,
                Balance_Amount = res.Balance_Amount,
                Rate = res.Rate,
                TotalPlotVal = res.Total,
                File_Transfer_Id = res.File_Transfer_Id,
                Delivery = res.Delivery,
                Block_Id = res.Block_Id,
                File_Form_Number = res.FileFormNumber,
                GrandTotal = res.GrandTotal,
                Phase_Id = Convert.ToInt32(res.Phase_Id),
                QR_Id = res.FileFormNumber,
                Plot_Prefrence = res.Plot_Prefrence,
                Verification_Req = res.Verification_Req,
                AuditVerified = res.Verified
            };
            var trslist = db.Files_Transfer_Request.Where(x => x.File_Form_Id == fa.Id && x.Status == 0).ToList();
            var newres = new { File = fa, Installments = installments, OtherInstallments = otherinstallments, ReceivedAmounts = Receivedamts, TransferTo = trslist };
            return Json(newres);
        }
        public JsonResult UpdateOwnerResult(OwnerData od)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_FileOwnerList(od.FileId).ToList().Where(x => x.Id == od.Id).FirstOrDefault();
            db.Sp_Update_FileOwnerData(od.Name, od.Father_Husband, od.Postal_Address, od.Residential_Address, od.City, od.Date_Of_Birth,
                od.CNIC_NICOP, od.Nationality, od.Email, od.Phone_Office, od.Residential, od.Mobile_1, od.Mobile_2, od.Nominee_Name,
                od.Nominee_CNIC_NICOP, od.Nominee_Relation, od.Nominee_Address, od.Occupation, od.DateTime, od.FileId, od.Ownership_Status, od.Id);
            //Adding comment
            if (res.Name != od.Name && res.Name != null) { db.Sp_Add_FileComments(od.FileId, "Update Name: " + res.Name + " To: " + od.Name, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Ownership_Status != od.Ownership_Status && res.Ownership_Status != null) { db.Sp_Add_FileComments(od.FileId, "Update Ownership Staus: " + res.Ownership_Status + " To: " + od.Ownership_Status, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Father_Husband != od.Father_Husband && res.Father_Husband != null) { db.Sp_Add_FileComments(od.FileId, "Update Father Name: " + res.Father_Husband + " To: " + od.Father_Husband, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.CNIC_NICOP != od.CNIC_NICOP && res.CNIC_NICOP != null) { db.Sp_Add_FileComments(od.FileId, "Update CNIC: " + res.CNIC_NICOP + " To: " + od.CNIC_NICOP, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Mobile_1 != od.Mobile_1 && res.Mobile_1 != null) { db.Sp_Add_FileComments(od.FileId, "Update Mobile Number: " + res.Mobile_1 + " To: " + od.Mobile_1, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Postal_Address != od.Postal_Address && res.Postal_Address != null) { db.Sp_Add_FileComments(od.FileId, "Update Postal Address: " + res.Postal_Address + " To: " + od.Postal_Address, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.City != od.City && res.City != null) { db.Sp_Add_FileComments(od.FileId, "Update City: " + res.City + " To: " + od.City, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nationality != od.Nationality && res.Nationality != null) { db.Sp_Add_FileComments(od.FileId, "Update Nationality: " + res.Nationality + " To: " + od.Nationality, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Date_Of_Birth != od.Date_Of_Birth && res.Date_Of_Birth != null) { db.Sp_Add_FileComments(od.FileId, "Update Date of Birth: " + res.Date_Of_Birth + " To: " + od.Date_Of_Birth, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nominee_Name != od.Nominee_Name && res.Nominee_Name != null) { db.Sp_Add_FileComments(od.FileId, "Update Nominee Name: " + res.Nominee_Name + " To: " + od.Nominee_Name, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nominee_Relation != od.Nominee_Relation && res.Nominee_Relation != null) { db.Sp_Add_FileComments(od.FileId, "Update Nominee Relation: " + res.Nominee_Relation + " To: " + od.Nominee_Relation, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nominee_Address != od.Nominee_Address && res.Nominee_Address != null) { db.Sp_Add_FileComments(od.FileId, "Update Nominee Address: " + res.Nominee_Address + " To: " + od.Nominee_Address, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nominee_CNIC_NICOP != od.Nominee_CNIC_NICOP && res.Nominee_CNIC_NICOP != null) { db.Sp_Add_FileComments(od.FileId, "Update Nominee CNIC: " + res.Nominee_CNIC_NICOP + " To: " + od.Nominee_CNIC_NICOP, userid, ActivityType.Record_Upatation.ToString()); }
            return Json(true);
        }
        public JsonResult DeliverFile(long? id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Files_Transfer.Where(x => x.Group_Tag == id).ToList();
            db.Sp_Add_FileComments(res1.Select(x => x.File_Form_Id).FirstOrDefault(), "Customer Files of " + string.Join(",", res1.Select(x => x.Name)) + " is Delivered ", userid, ActivityType.Transfer_Request.ToString());
            var res = db.Sp_Update_FileDelivery(id);
            return Json(true);
        }
        public ActionResult FileTransfer()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            return View();
        }
        public ActionResult FileTransferTest()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            return View();
        }
        public JsonResult FileTransferr(List<Files_Transfer> filedatas, string Plot_Prefrence, ReceiptData rd, long Req_Id, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var usernam = db.Users.Find(userid).Name;
            var appdetail = db.Sp_Get_FileData(Convert.ToInt64(rd.FilePlotNumber)).FirstOrDefault();
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Sp_Add_FileTransfer(Req_Id).FirstOrDefault();
                    var res1 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, string.Join(",", filedatas.Select(x => x.Mobile_1).Distinct())
                                        , string.Join(",", filedatas.Select(x => x.Father_Husband).Distinct()), appdetail.Id, string.Join(",", filedatas.Select(x => x.Name)), "Cash", filedatas.Select(x => x.Total).FirstOrDefault(),
                                        rd.Project_Name, filedatas.Select(x => x.Rate).FirstOrDefault(), null, filedatas.Select(x => x.Plot_Size).FirstOrDefault(), ReceiptTypes.Transfer.ToString(), TransactionId, userid, "File Transfer", null, Modules.FileManagement.ToString(), rd.DevCharges, rd.File_Plot_Number.ToString(), appdetail.Block, appdetail.Type, Req_Id, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                    //{
                    //    bool headcashier = false;
                    //    if (User.IsInRole("Head Cashier"))
                    //    {
                    //        headcashier = true;
                    //    }
                    //        Helpers H = new Helpers();
                    //        AccountHandlerController de = new AccountHandlerController();
                    //        de.Transfer_Plot(rd.Amount, rd.File_Plot_Number.ToString(), appdetail.Type, appdetail.Block_Name, "Cash", rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res1.Receipt_No, 1, headcashier, AccountingModule, Convert.ToInt64(appdetail.Block_Id));
                    //}
                    db.Sp_Update_ReceiptOwner(filedatas.Select(x => x.File_Form_Id).FirstOrDefault(), Req_Id);
                    db.Sp_Update_OwnerTransferReceipt(Req_Id, res1.Receipt_Id, Modules.FileManagement.ToString());
                    db.Sp_Add_FileComments(filedatas.Select(x => x.Id).FirstOrDefault(), "File is transfered to : " + String.Join("/", filedatas.Select(x => x.Name)), userid, ActivityType.Transfer_Request.ToString());
                    Transaction.Commit();
                    var text = "Dear " + rd.Name + ",\n\r" +
                           "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in cash for Plot number# " + rd.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                    try
                    {
                        SmsService smsService = new SmsService();
                        if (!(String.IsNullOrEmpty(rd.Mobile_1)) && !(String.IsNullOrWhiteSpace(rd.Mobile_1)))
                        {
                            smsService.SendMsg(text, rd.Mobile_1);
                        }
                    }
                    catch (Exception ex)
                    {
                        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    }
                    var res = new { Status = true, Receiptid = res1.Receipt_No, Token = TransactionId };
                    return Json(res);
                }
                catch (Exception ex)
                {

                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                    var res = new { Status = false, Msg = "Error Occured" };
                    return Json(res);
                }
            }
        }
        [Authorize(Roles = "File Information,Administrator")]
        public JsonResult GetFileResult(string Filefromid)
        {
            var res1 = db.Sp_Get_FileAppFormData(Filefromid).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            if (res1 == null) { return Json(false); }
            List<Sp_Get_FileInstallments_Result> Allinstallments = db.Sp_Get_FileInstallments(res1.Id).ToList();
            if (Allinstallments.Any())
            {
                this.TestAdjustIntallments(res1.Id);
            }
            var res = db.Sp_Get_FileFormData(res1.Id).SingleOrDefault();
            var installments = Allinstallments.Where(x => x.Installment_Type == "1").ToList();
            var otherinstallments = Allinstallments.Where(x => x.Installment_Type != "1").ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(res.Id, Modules.FileManagement.ToString()).ToList();
            FileStatus filestat = (FileStatus)res.Status;
            string status = filestat.ToString();
            var Discount = db.Discounts.Where(x => x.Module_Id == res.Id).ToList();
            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Dealership_Name = res.Dealership_Name,
                Development_Charges = res.Development_Charges.ToString(),
                Development_Charges_Val = res.Development_Charges,
                Phase_Name = res.Phase,
                Plot_Size = res.Plot_Size,
                Project_Name = res.Project,
                Status = ((FileStatus)res.Status).ToString(),
                City = res.City,
                CNIC_NICOP = res.CNIC_NICOP,
                Date_Of_Birth = res.Date_Of_Birth,
                Email = res.Email,
                Father_Husband = res.Father_Husband,
                Mobile_1 = res.Mobile_1,
                Mobile_2 = res.Mobile_2,
                Name = res.Name,
                Nationality = res.Nationality,
                Nominee_Address = res.Nominee_Address,
                Nominee_CNIC_NICOP = res.Nominee_CNIC_NICOP,
                Nominee_Name = res.Nominee_Name,
                Nominee_Relation = res.Nominee_Relation,
                Occupation = res.Occupation,
                Phone_Office = res.Phone_Office,
                Postal_Address = res.Postal_Address,
                Residential = res.Residential,
                Residential_Address = res.Residential_Address,
                Balance_Amount = res.Balance_Amount,
                Rate = res.Rate,
                TotalPlotVal = res.Total,
                File_Transfer_Id = res.File_Transfer_Id,
                Delivery = res.Delivery,
                Block_Id = res.Block_Id,
                File_Form_Number = res.FileFormNumber,
                GrandTotal = res.GrandTotal,
                Phase_Id = Convert.ToInt32(res.Phase_Id),
                QR_Id = res.FileFormNumber,
                Plot_Prefrence = res.Plot_Prefrence,
                Verify = res.Verify,
                Verification_Req = res.Verification_Req,
                Date_Reg = res.DateTime,
                AuditVerified = res.Verified,
                Image1 = res.Image1,
                Image2 = res.Image2,
                Image3 = res.Image3,
                Image4 = res.Image4
            };
            db.Sp_Add_Activity(userid, "Accessed File Details  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), res1.Id);
            var newres = new { File = fa, Installments = installments, OtherInstallments = otherinstallments, ReceivedAmounts = Receivedamts, Discounts = Discount };
            return Json(newres);
        }
        public ActionResult RegisterFileTest()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Plot_Prefrence = new SelectList(db.Sp_Get_Plot_Prefrences(), "Id", "Plot_Prefrence");
            return View();
        }
        public JsonResult FileDiscount(decimal TotalAmt, decimal DiscountAmt, string Remarks, long Id, DateTime? DiscountDate)
        {
            var res = db.Sp_Add_Discount(TotalAmt, DiscountAmt, Remarks, Id, Modules.FileManagement.ToString(), DiscountDate);
            return Json(true);
        }
        [HttpPost]
        public JsonResult RegisterFileTest(Files_Transfer filedata, bool Flag, string DevCharStatus, string FileFormNumber, List<ReceiptData> Receiptdata)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            string Devchar = (DevCharStatus == "true") ? "With Development Charges" : "Non Development Charges";
            var appdetail = db.Sp_Get_FileFormData(Convert.ToInt64(FileFormNumber)).FirstOrDefault();
            List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(filedata.File_Form_Id).ToList();
            if (!installmentstructure.Any())
            {
                var data = new { Status = "-1", Data = "No Installment Plan Is Found" };
                return Json(data);
            }
            if (Flag)
            {
                long Fileid = Convert.ToInt64(db.Sp_Add_FileTest(filedata.Plot_Size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
                    , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
                    filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP,
                    filedata.Plot_Prefrence, filedata.File_Form_Id, filedata.Phase_Id, filedata.Block_Id, filedata.City, filedata.Rate, filedata.Total, filedata.GrandTotal, Flag, filedata.DateTime).FirstOrDefault());
                InstallmentsController ic = new InstallmentsController();
                var file_Installments = ic.AddInstallmentPlanTest(installmentstructure, DevCharStatus, Fileid, Convert.ToDateTime(filedata.DateTime));
                if (file_Installments.Status)
                {
                    db.Sp_Update_FileRate(Fileid, file_Installments.Rate, file_Installments.Total, file_Installments.Grand_Total);
                    List<long> ids = new List<long>();
                    foreach (var rd in Receiptdata)
                    {
                        if (rd.PaymentType == "Online")
                            rd.PayChqNo = rd.PayChqNo + "(Bank Acc No)";
                        var res = Convert.ToInt64(db.Sp_Add_Receipt_Manual(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, filedata.Mobile_1
                                            , filedata.Father_Husband, Fileid, filedata.Name, rd.PaymentType, file_Installments.Total,
                                            Modules.FileManagement.ToString(), file_Installments.Rate, null, filedata.Plot_Size, ReceiptTypes.Booking.ToString(), Fileid, userid, rd.ReceiptNo, rd.Ch_bk_Pay_Date, "", FileFormNumber, appdetail.Block, appdetail.Plot_Type).FirstOrDefault());
                        ids.Add(res);
                        //bool headcashier = false;
                        //if (User.IsInRole("Head Cashier"))
                        //{
                        //    headcashier = true;
                        //}
                        //try
                        //{
                        //    AccountHandlerController de = new AccountHandlerController();
                        //    de.Receive_Plot_Amount(rd.Amount, FileFormNumber, appdetail.Plot_Type, appdetail.Block, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, new Helpers().RandomNumber(), userid, rd.ReceiptNo, 1, headcashier);
                        //}
                        //catch (Exception ex)
                        //{
                        //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        //}
                    }
                    try
                    {
                        SmsService smsService = new SmsService();
                        smsService.SendMsg("Dear " + filedata.Name + " Your File " + FileFormNumber + " Has been live and In process for Approvals", filedata.Mobile_1);
                    }
                    catch (Exception)
                    {
                    }
                    var data = new { Status = "1", Receiptid = ids, Token = Fileid };
                    return Json(data);
                }
                else
                {
                    var data = new { Status = "0", Data = file_Installments.Installments };
                    return Json(data);
                }
            }
            else
            {
                var filedetails = new FileData
                {
                    Plot_Size = filedata.Plot_Size,
                    Name = filedata.Name,
                    Father_Husband = filedata.Father_Husband,
                    Postal_Address = filedata.Postal_Address,
                    Residential_Address = filedata.Residential_Address,
                    Phone_Office = filedata.Phone_Office,
                    Residential = filedata.Residential,
                    Mobile_1 = filedata.Mobile_1,
                    Mobile_2 = filedata.Mobile_2,
                    Email = filedata.Email,
                    Occupation = filedata.Occupation,
                    Nationality = filedata.Nationality,
                    Date_Of_Birth = filedata.Date_Of_Birth,
                    CNIC_NICOP = filedata.CNIC_NICOP,
                    Nominee_Name = filedata.Nominee_Name,
                    Nominee_Relation = filedata.Nominee_Relation,
                    Nominee_Address = filedata.Nominee_Address,
                    Nominee_CNIC_NICOP = filedata.Nominee_CNIC_NICOP,
                    Plot_Prefrence = filedata.Plot_Prefrence,
                    Id = Convert.ToInt64(filedata.File_Form_Id),
                    Phase_Id = Convert.ToInt64(filedata.Phase_Id),
                    Block_Id = filedata.Block_Id,
                    City = filedata.City,
                    Flag = Flag
                };
                string FileDetails = JsonConvert.SerializeObject(filedetails);
                decimal Totalamt = Receiptdata.Sum(x => x.Amount);
                var res1 = db.Sp_Add_Amount_Clearance(Totalamt, FileDetails, Modules.FileManagement.ToString(), Types.Booking.ToString()).FirstOrDefault();
                List<long> ids = new List<long>();
                foreach (var rd in Receiptdata)
                {
                    var res3 = Convert.ToInt64(db.Sp_Add_Receipt_Manual(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, filedata.Mobile_1
                                        , filedata.Father_Husband, filedata.File_Form_Id, filedata.Name, rd.PaymentType, null,
                                        Modules.FileManagement.ToString(), null, null, filedata.Plot_Size, ReceiptTypes.Booking.ToString(), filedata.File_Form_Id, userid, rd.ReceiptNo, DateTime.Now, Devchar, FileFormNumber, appdetail.Block, appdetail.Plot_Type).FirstOrDefault());
                    ids.Add(res3);
                    if (rd.PaymentType != "Cash")
                    {
                        var res2 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                            Modules.FileManagement.ToString(), Types.Booking.ToString(), res1, rd.PayChqNo, filedata.File_Form_Id, rd.Ch_bk_Pay_Date, FileFormNumber, res3, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                    }
                    //bool headcashier = false;
                    //if (User.IsInRole("Head Cashier"))
                    //{
                    //    headcashier = true;
                    //}
                    //try
                    //{
                    //    AccountHandlerController de = new AccountHandlerController();
                    //    de.Receive_Plot_Amount(rd.Amount, FileFormNumber, appdetail.Plot_Type, appdetail.Block, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, new Helpers().RandomNumber(), userid, rd.ReceiptNo, 1, headcashier);
                    //}
                    //catch (Exception ex)
                    //{
                    //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    //}
                }
                var data = new { Status = "2", Receiptid = ids, Token = filedata.File_Form_Id };
                return Json(data);
            }
        }
        [Authorize(Roles = "Update File Information,Administrator")]
        public ActionResult UpdateInformation()
        {
            return View();
        }

        public ActionResult UpdationForInstallments(long FileId)
        {
            ViewBag.FileID = FileId;
            return PartialView();
        }
        public ActionResult InstallmentPlanMaker(InstallmentPlanUpdation Plan)
        {
            List<File_Installments> File_inst = new List<File_Installments>();
            DateTime a = DateTime.UtcNow;
            DateTime Granddate = DateTime.UtcNow;
            if (Plan.AdvacneAmount != 0 || Plan.BallotinAmount != 0 || Plan.InstallmentAmountPerMonth != 0 || Plan.DevelopmentAmount != 0)
            {
                if (Plan.AdvacneAmount != 0)
                {
                    for (int i = 1; i <= 1; i++)
                    {
                        File_Installments fi = new File_Installments()
                        {
                            Installment_Name = "Advance",
                            Amount = Plan.AdvacneAmount,
                            Due_Date = Plan.AdvanceDate,
                            Status = "Pending",
                            Installment_Type = "3",
                        };
                        File_inst.Add(fi);
                    }
                }
                // for installment
                if (Plan.NoOfInstallments != 0)
                {
                    for (int i = 1; i <= Plan.NoOfInstallments; i++)
                    {
                        var d = DateTime.UtcNow;
                        if (i == 1)
                        {
                            d = Plan.InsatallmentStartingDate;
                        }
                        else
                        {
                            int counter = i - 1;
                            d = Plan.InsatallmentStartingDate.AddMonths(counter);
                        }
                        File_Installments fi = new File_Installments()
                        {
                            Installment_Name = i + " Monthly Installment",
                            Amount = Plan.InstallmentAmountPerMonth,
                            Due_Date = d,
                            Status = "Pending",
                            Installment_Type = "1",
                        };
                        File_inst.Add(fi);
                    }
                }
                if (Plan.BallotinAmount != 0)
                {
                    for (int i = 1; i <= 1; i++)
                    {
                        File_Installments fi = new File_Installments()
                        {
                            Installment_Name = "Balloting",
                            Amount = Plan.BallotinAmount,
                            Due_Date = Plan.BalltionDate,
                            Status = "Pending",
                            Installment_Type = "0",
                        };
                        File_inst.Add(fi);
                    }
                }
                if (Plan.DevelopmentAmount != 0)
                {
                    for (int i = 1; i <= 1; i++)
                    {
                        File_Installments fi = new File_Installments()
                        {
                            File_Id = Plan.FileId,
                            Installment_Name = "Development Charges",
                            Amount = Plan.DevelopmentAmount,
                            Due_Date = Plan.DevelopmentDate,
                            Status = "Pending",
                            Installment_Type = "2",
                        };
                        File_inst.Add(fi);
                    }
                }
                return PartialView(File_inst);
            }
            else
            {
                return PartialView(File_inst);
            }
        }
        public JsonResult FinalizeInstallments(InstallmentPlanUpdation Plan)
        {
            if (Plan.AdvacneAmount != 0 || Plan.BallotinAmount != 0 || Plan.InstallmentAmountPerMonth != 0)
            {
                var Filedata = db.File_Form.Where(x => x.Id == Plan.FileId).SingleOrDefault();
                long userid = long.Parse(User.Identity.GetUserId());
                List<File_Installments> File_inst = new List<File_Installments>();
                DateTime a = DateTime.UtcNow;
                DateTime Granddate = DateTime.UtcNow;
                if (Plan.AdvacneAmount != 0)
                {
                    for (int i = 1; i <= 1; i++)
                    {
                        File_Installments fi = new File_Installments()
                        {
                            File_Id = Plan.FileId,
                            Installment_Name = "Advance",
                            Amount = Plan.AdvacneAmount,
                            Due_Date = Plan.AdvanceDate,
                            Status = "Pending",
                            Installment_Type = "3",
                        };
                        File_inst.Add(fi);
                    }
                }
                // for installment
                if (Plan.NoOfInstallments != 0)
                {
                    for (int i = 1; i <= Plan.NoOfInstallments; i++)
                    {
                        var d = DateTime.UtcNow;
                        if (i == 1)
                        {
                            d = Plan.InsatallmentStartingDate;
                        }
                        else
                        {
                            int counter = i - 1;
                            d = Plan.InsatallmentStartingDate.AddMonths(counter);
                        }
                        File_Installments fi = new File_Installments()
                        {
                            File_Id = Plan.FileId,
                            Installment_Name = i + " Monthly Installment",
                            Amount = Plan.InstallmentAmountPerMonth,
                            Due_Date = d,
                            Status = "Pending",
                            Installment_Type = "1",
                        };
                        File_inst.Add(fi);
                    }
                }
                if (Plan.BallotinAmount != 0)
                {
                    for (int i = 1; i <= 1; i++)
                    {
                        File_Installments fi = new File_Installments()
                        {
                            File_Id = Plan.FileId,
                            Installment_Name = " Balloting ",
                            Amount = Plan.BallotinAmount,
                            Due_Date = Plan.BalltionDate,
                            Status = "Pending",
                            Installment_Type = "0",
                        };
                        File_inst.Add(fi);
                    }
                }
                if (Plan.DevelopmentAmount != 0)
                {
                    for (int i = 1; i <= 1; i++)
                    {
                        File_Installments fi = new File_Installments()
                        {
                            File_Id = Plan.FileId,
                            Installment_Name = "Development Charges",
                            Amount = Plan.DevelopmentAmount,
                            Due_Date = Plan.DevelopmentDate,
                            Status = "Pending",
                            Installment_Type = "2",
                        };
                        File_inst.Add(fi);
                    }
                }
                var installmentplan = new XElement("FileSystem", File_inst.Select(x => new XElement("Structure",
                                          new XAttribute("File_Id", x.File_Id),
                                          new XAttribute("Installment_Name", x.Installment_Name),
                                          new XAttribute("Installment_Type", x.Installment_Type),
                                          new XAttribute("Amount", x.Amount),
                                          new XAttribute("Due_Date", x.Due_Date),
                                          new XAttribute("Status", x.Status)
                                          ))).ToString();
                db.Sp_Update_FileInstallment_Plan(Plan.FileId, installmentplan);
                db.Sp_Add_FileComments(Plan.FileId, "Installment Plan updated of " + Filedata.FileFormNumber, userid, ActivityType.Plan_Updation.ToString());
                if (Filedata.Verified == true)
                {
                    db.Sp_Update_FileVerificationToNull(Filedata.Id);
                    // Pending Notification
                }
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        public JsonResult VerifyFile(long Id, bool Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_GenVerifyStatus(Id, Status, Modules.FileManagement.ToString());
            return Json(true);
        }
        public JsonResult UpdatePlotPref(long Id, string Pref)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Files_Transfer.Where(x => x.Id == Id).FirstOrDefault();
            db.Sp_Update_PlotPref(Id, Pref);
            db.Sp_Add_FileComments(Id, "Plot Prefrence of Customer : " + res.Name + " ( " + res.CNIC_NICOP + " ) has been changed from " + res.Plot_Prefrence + " To " + Pref, userid, ActivityType.Record_Upatation.ToString());
            return Json(true);
        }
        public ActionResult RegisterTokenFiles()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return View();
        }
        public JsonResult GetTokenFileResult(long Filefromid)
        {
            Sp_Get_FileFormData_Result res = db.Sp_Get_FileFormData(Filefromid).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            if (res == null) { return Json(false); }
            var Receivedamts = db.Sp_Get_ReceivedAmounts(res.Id, Modules.FileManagement.ToString()).ToList();
            FileStatus filestat = (FileStatus)res.Status;
            string status = filestat.ToString();
            List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(res.Id).ToList();
            decimal advance = installmentstructure.Where(x => x.Installment_Type == "3").Select(x => x.Amount).SingleOrDefault();
            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Dealership_Name = res.Dealership_Name,
                Development_Charges = res.Development_Charges.ToString(),
                Development_Charges_Val = res.Development_Charges,
                Phase_Name = res.Phase,
                Plot_Size = res.Plot_Size,
                Project_Name = res.Project,
                Status = ((FileStatus)res.Status).ToString(),
                City = res.City,
                CNIC_NICOP = res.CNIC_NICOP,
                Date_Of_Birth = res.Date_Of_Birth,
                Email = res.Email,
                Father_Husband = res.Father_Husband,
                Mobile_1 = res.Mobile_1,
                Mobile_2 = res.Mobile_2,
                Name = res.Name,
                Nationality = res.Nationality,
                Nominee_Address = res.Nominee_Address,
                Nominee_CNIC_NICOP = res.Nominee_CNIC_NICOP,
                Nominee_Name = res.Nominee_Name,
                Nominee_Relation = res.Nominee_Relation,
                Occupation = res.Occupation,
                Phone_Office = res.Phone_Office,
                Postal_Address = res.Postal_Address,
                Residential = res.Residential,
                Residential_Address = res.Residential_Address,
                Balance_Amount = res.Balance_Amount,
                Rate = res.Rate,
                TotalPlotVal = res.Total,
                File_Transfer_Id = res.File_Transfer_Id,
                Delivery = res.Delivery,
                Block_Id = res.Block_Id,
                File_Form_Number = res.FileFormNumber,
                GrandTotal = res.GrandTotal,
                Phase_Id = Convert.ToInt32(res.Phase_Id),
                QR_Id = res.FileFormNumber,
                Plot_Prefrence = res.Plot_Prefrence,
                Verification_Req = res.Verification_Req,
                Verify = res.Verify,
                AuditVerified = res.Verified
            };
            var newres = new { File = fa, ReceivedAmounts = Receivedamts, Advance = advance };
            db.Sp_Add_Activity(userid, "Accessed File Details  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Filefromid);
            return Json(newres);
        }
        public ActionResult FileResult(long Filefromid)
        {
            Sp_Get_FileFormData_Result res = db.Sp_Get_FileFormData(Filefromid).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            if (res == null) { return Json(false); }
            List<Sp_Get_FileInstallments_Result> Allinstallments = db.Sp_Get_FileInstallments(res.Id).ToList();
            var installments = Allinstallments.Where(x => x.Installment_Type == "1").ToList();
            var otherinstallments = Allinstallments.Where(x => x.Installment_Type != "1").ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(res.Id, Modules.FileManagement.ToString()).ToList();
            FileStatus filestat = (FileStatus)res.Status;
            string status = filestat.ToString();
            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Dealership_Name = res.Dealership_Name,
                Development_Charges = res.Development_Charges.ToString(),
                Development_Charges_Val = res.Development_Charges,
                Phase_Name = res.Phase,
                Plot_Size = res.Plot_Size,
                Project_Name = res.Project,
                Status = ((FileStatus)res.Status).ToString(),
                City = res.City,
                CNIC_NICOP = res.CNIC_NICOP,
                Date_Of_Birth = res.Date_Of_Birth,
                Email = res.Email,
                Father_Husband = res.Father_Husband,
                Mobile_1 = res.Mobile_1,
                Mobile_2 = res.Mobile_2,
                Name = res.Name,
                Nationality = res.Nationality,
                Nominee_Address = res.Nominee_Address,
                Nominee_CNIC_NICOP = res.Nominee_CNIC_NICOP,
                Nominee_Name = res.Nominee_Name,
                Nominee_Relation = res.Nominee_Relation,
                Occupation = res.Occupation,
                Phone_Office = res.Phone_Office,
                Postal_Address = res.Postal_Address,
                Residential = res.Residential,
                Residential_Address = res.Residential_Address,
                Balance_Amount = res.Balance_Amount,
                Rate = res.Rate,
                TotalPlotVal = res.Total,
                File_Transfer_Id = res.File_Transfer_Id,
                Delivery = res.Delivery,
                Block_Id = res.Block_Id,
                File_Form_Number = res.FileFormNumber,
                GrandTotal = res.GrandTotal,
                Phase_Id = Convert.ToInt32(res.Phase_Id),
                QR_Id = res.FileFormNumber,
                Plot_Prefrence = res.Plot_Prefrence,
                Verification_Req = res.Verification_Req,
                Verify = res.Verify,
                AuditVerified = res.Verified
            };
            var newres = new FileResults { File = fa, Installments = installments, OtherInstallments = otherinstallments, ReceivedAmounts = Receivedamts };
            db.Sp_Add_Activity(userid, "Accessed File Details Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Filefromid);
            return PartialView(newres);
        }
        public ActionResult TransferRequest()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.serial = h.RandomNumber();
            var res = db.Dealerships.ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed File Transfer Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(res);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult FileTransferRequest(Files_Transfer filedata, long File_Plot_Number, long TransactionId)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    if (db.BallotPlots.Any(x => x.FileId == filedata.File_Form_Id))
        //    {
        //        return Json(-1);
        //    }
        //    long id = Convert.ToInt64(db.Sp_Add_TransferRequestFile(filedata.Plot_Size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
        //            , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
        //            filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP, filedata.File_Form_Id, filedata.City, TransactionId).FirstOrDefault());
        //    return Json(TransactionId);
        //}
        public JsonResult FileTransferRequest(List<Files_Transfer> filedatas, long File_Plot_Number, long TransactionId, string plt_size, string dealingDealership)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //if (db.BallotPlots.Any(x => x.FileId == File_Plot_Number))
            //{
            //    return Json(-1);
            //}
            var grp = filedatas.Select(x => x.Group_Tag).FirstOrDefault();
            if (grp != null)
            {
                db.Sp_Add_FileComments(File_Plot_Number, "Generate Transfer Request for Owner : " + string.Join(",", filedatas.Select(x => x.Name).ToList()) + " resubmitted after editing.", userid, ActivityType.Transfer_Request.ToString());
                foreach (var filedata in filedatas)
                {
                    long id = Convert.ToInt64(db.Sp_Add_TransferRequestFile(plt_size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
                            , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
                            filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP, File_Plot_Number, filedata.City, TransactionId, "", dealingDealership, filedata.Group_Tag, filedata.IsCompanyProperty).FirstOrDefault());
                }
                return Json(TransactionId);
            }
            string rec_no = db.Sp_Get_ReceiptNo("Plot Transfer").FirstOrDefault();
            foreach (var filedata in filedatas)
            {
                long id = Convert.ToInt64(db.Sp_Add_TransferRequestFile(plt_size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
                        , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
                        filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP, File_Plot_Number, filedata.City, TransactionId, rec_no, dealingDealership, filedata.Group_Tag, filedata.IsCompanyProperty).FirstOrDefault());
            }
            db.Sp_Add_FileComments(File_Plot_Number, "Generate Transfer Request for Owner : " + string.Join(",", filedatas.Select(x => x.Name).ToList()), userid, ActivityType.Transfer_Request.ToString());
            return Json(TransactionId);
        }
        public ActionResult TransferRequestList()
        {
            var res = db.Sp_Get_TransferRequestList().ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed File Transfer REquest List  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(res);
        }
        [HttpGet]
        public ActionResult FileTransferRequestDetails(string Id)
        {
            var fileform = db.File_Form.Where( f => f.FileFormNumber == Id).FirstOrDefault();
            Sp_Get_FileFormData_Result res = db.Sp_Get_FileFormData(fileform.Id).FirstOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Details For File Transfer Request ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), fileform.Id);
            if (res == null) { return Json(false); }
            List<Sp_Get_FileInstallments_Result> Allinstallments = db.Sp_Get_FileInstallments(res.Id).ToList();
            var installments = Allinstallments.Where(x => x.Installment_Type == "1").ToList();
            var otherinstallments = Allinstallments.Where(x => x.Installment_Type != "1").ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(res.Id, Modules.FileManagement.ToString()).ToList();
            var tranres = db.Sp_Get_TransferRequestDetails_File(res.Id).ToList();
            FileStatus filestat = (FileStatus)res.Status;
            string status = filestat.ToString();
            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Dealership_Name = res.Dealership_Name,
                Development_Charges = res.Development_Charges.ToString(),
                Development_Charges_Val = res.Development_Charges,
                Phase_Name = res.Phase,
                Plot_Size = res.Plot_Size,
                Project_Name = res.Project,
                Status = ((FileStatus)res.Status).ToString(),
                City = res.City,
                CNIC_NICOP = res.CNIC_NICOP,
                Date_Of_Birth = res.Date_Of_Birth,
                Email = res.Email,
                Father_Husband = res.Father_Husband,
                Mobile_1 = res.Mobile_1,
                Mobile_2 = res.Mobile_2,
                Name = res.Name,
                Nationality = res.Nationality,
                Nominee_Address = res.Nominee_Address,
                Nominee_CNIC_NICOP = res.Nominee_CNIC_NICOP,
                Nominee_Name = res.Nominee_Name,
                Nominee_Relation = res.Nominee_Relation,
                Occupation = res.Occupation,
                Phone_Office = res.Phone_Office,
                Postal_Address = res.Postal_Address,
                Residential = res.Residential,
                Residential_Address = res.Residential_Address,
                Balance_Amount = res.Balance_Amount,
                Rate = res.Rate,
                TotalPlotVal = res.Total,
                File_Transfer_Id = res.File_Transfer_Id,
                Delivery = res.Delivery,
                Block_Id = res.Block_Id,
                File_Form_Number = res.FileFormNumber,
                GrandTotal = res.GrandTotal,
                Phase_Id = res.Phase_Id,
                QR_Id = res.FileFormNumber,
                Plot_Prefrence = res.Plot_Prefrence,
                Verification_Req = res.Verification_Req,
                Verify = res.Verify,
                AuditVerified = res.Verified
            };
            var newres = new TransferRequestDetails { File = fa, TransferTo = tranres, Installments = installments, OtherInstallments = otherinstallments, ReceivedAmounts = Receivedamts };
            return View(newres);
        }
        public JsonResult CheckList(long ReqId, int Updateprop, bool Status)
        {
            try
            {
                db.Sp_Update_TransferReqCheckList(ReqId, Status, Updateprop);
                return Json(true);
            }
            catch (Exception)
            {
                return Json(true);
            }
        }
        public JsonResult OkForTransfer(long Reqid, bool Blood_rel, bool Wave_off, bool OtherTransferCharges, decimal? Rate, string Remarks)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_TransferRequest_File(Reqid, Blood_rel, Wave_off, OtherTransferCharges, Rate, Remarks);
            return Json(true);
        }
        public ActionResult NDCForm(long SerialNum)
        {
            var res = db.Sp_Get_NDCFormDetails(SerialNum).FirstOrDefault();
            var new_owners = db.Files_Transfer_Request.Where(x => x.Group_Tag == SerialNum).ToList();
            var fileformid = new_owners.Select(y => y.File_Form_Id).FirstOrDefault();
            var filedata = db.File_Form.Where(y => y.Id == fileformid).FirstOrDefault();
            var old_owners_all = db.Sp_Get_FileLastOwner(fileformid).ToList();
            Sp_Get_NDCFormDetails_Result ress = new Sp_Get_NDCFormDetails_Result();
            ress.Id = res.Id;
            ress.FileFormNumber = res.FileFormNumber;
            ress.Block = filedata.Block;
            ress.Plot_Size = res.Plot_Size;
            ress.STATUS = res.STATUS;
            ress.TransactionId = res.TransactionId;
            ress.TransferReqNo = res.TransferReqNo;
            ress.Transfer_Success = res.Transfer_Success;
            ress.From_Name = string.Join(",", old_owners_all.Select(x => x.Name));
            ress.From_FatherName = string.Join(",", old_owners_all.Select(x => x.Father_Husband));
            ress.From_CNIC_NICOP = string.Join(",", old_owners_all.Select(x => x.CNIC_NICOP));
            ress.From_Mobile1 = string.Join(",", old_owners_all.Select(x => x.Mobile_1));
            if (new_owners.Count == 1)
            {
                ress.To_Name = new_owners.Select(x => x.Name).FirstOrDefault();
                ress.To_FatherName = new_owners.Select(x => x.Father_Husband).FirstOrDefault();
                ress.Postal_Address = new_owners.Select(x => x.Postal_Address).FirstOrDefault();
                ress.To_CNIC_NICOP = new_owners.Select(x => x.CNIC_NICOP).FirstOrDefault();
                ress.To_Mobile1 = new_owners.Select(x => x.Mobile_1).FirstOrDefault();
                ress.Nominee_Name = new_owners.Select(x => x.Nominee_Name).FirstOrDefault();
                ress.Nominee_CNIC_NICOP = new_owners.Select(x => x.Nominee_CNIC_NICOP).FirstOrDefault();
                ress.IsCompany = new_owners.Any(x => x.IsCompanyProperty == true);
            }
            else
            {
                ress.To_Name = string.Join(",", new_owners.Select(x => x.Name));
                ress.To_FatherName = string.Join(",", new_owners.Select(x => x.Father_Husband));
                ress.Postal_Address = string.Join(",", new_owners.Select(x => x.Postal_Address));
                ress.To_CNIC_NICOP = string.Join(",", new_owners.Select(x => x.CNIC_NICOP));
                ress.To_Mobile1 = string.Join(",", new_owners.Select(x => x.Mobile_1));
                ress.Nominee_Name = string.Join(",", new_owners.Select(x => x.Nominee_Name));
                ress.Nominee_CNIC_NICOP = string.Join(",", new_owners.Select(x => x.Nominee_CNIC_NICOP));
                ress.Nominee_Name = string.Join(",", new_owners.Select(x => x.Nominee_Name));
                ress.IsCompany = new_owners.Any(x => x.IsCompanyProperty == true);
            }
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed NDC " + SerialNum, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), ress.Id);
            return View(ress);
        }
        public ActionResult FileTransferRequestData(string Id)
        {
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            ViewBag.TransferRate = 0;
            long userid = long.Parse(User.Identity.GetUserId());
            var trn_res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.File_Transfer_Fee_Config.ToString()).FirstOrDefault();
            var transferData = JsonConvert.DeserializeObject<List<BlockTransferFeeModel>>(trn_res.CurrentConfig);
            var res1 = db.Sp_Get_FileAppFormData(Id).SingleOrDefault();
            var res = db.Sp_Get_FileFormData(res1.Id).SingleOrDefault();
            if (res == null) { return Json(false); }
            List<Sp_Get_FileInstallments_Result> Allinstallments = db.Sp_Get_FileInstallments(res.Id).ToList();
            var installments = Allinstallments.Where(x => x.Installment_Type == "1").ToList();
            var otherinstallments = Allinstallments.Where(x => x.Installment_Type != "1").ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(res.Id, Modules.FileManagement.ToString()).ToList();
            var tranres = db.Sp_Get_TransferRequestDetails_File(res.Id).ToList();
            FileStatus filestat = (FileStatus)res.Status;
            string status = filestat.ToString();

            var tdRate = transferData.Where(x => x.BlockName == res.Block).FirstOrDefault();
            if (tranres.Select(x => x.Dealer_Req).FirstOrDefault() == true)
            {
                //Agar Dealer Transfer hai
                if (res.Plot_Type == PlotType.Residential.ToString())
                {
                    ViewBag.TransferRate = tdRate.NC_Residential_Dealer;
                }
                else
                {
                    ViewBag.TransferRate = tdRate.NC_Commercial_Dealer;
                }
            }
            else
            {
                //Agar Dealer Transfer nahi hai
                if (res.Plot_Type == PlotType.Residential.ToString())
                {
                    ViewBag.TransferRate = tdRate.NC_Residential;
                }
                else
                {
                    ViewBag.TransferRate = tdRate.NC_Commercial;
                }
            }
            FileData fa = new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Dealership_Name = res.Dealership_Name,
                Development_Charges = res.Development_Charges.ToString(),
                Development_Charges_Val = res.Development_Charges,
                Phase_Name = "",
                Plot_Size = res.Plot_Size,
                Project_Name = "",
                Status = ((FileStatus)res.Status).ToString(),
                City = res.City,
                CNIC_NICOP = res.CNIC_NICOP,
                Date_Of_Birth = res.Date_Of_Birth,
                Email = res.Email,
                Father_Husband = res.Father_Husband,
                Mobile_1 = res.Mobile_1,
                Mobile_2 = res.Mobile_2,
                Name = res.Name,
                Nationality = res.Nationality,
                Nominee_Address = res.Nominee_Address,
                Nominee_CNIC_NICOP = res.Nominee_CNIC_NICOP,
                Nominee_Name = res.Nominee_Name,
                Nominee_Relation = res.Nominee_Relation,
                Occupation = res.Occupation,
                Phone_Office = res.Phone_Office,
                Postal_Address = res.Postal_Address,
                Residential = res.Residential,
                Residential_Address = res.Residential_Address,
                Balance_Amount = res.Balance_Amount,
                Rate = res.Rate,
                TotalPlotVal = res.Total,
                File_Transfer_Id = res.File_Transfer_Id,
                Delivery = res.Delivery,
                Block_Id = res.Block_Id,
                File_Form_Number = res.FileFormNumber,
                GrandTotal = res.GrandTotal,
                Phase_Id = res.Phase_Id,
                QR_Id = res.FileFormNumber,
                Plot_Prefrence = res.Plot_Prefrence,
                Verification_Req = res.Verification_Req,
                Verify = res.Verify,
                AuditVerified = res.Verified
            };
            var newres = new TransferRequestDetails { File = fa, TransferTo = tranres, Installments = installments, OtherInstallments = otherinstallments, ReceivedAmounts = Receivedamts };
            return PartialView(newres);
        }
        public int TestAdjustIntallments(long? Fileno)
        {
            try
            {
                var item = db.Test_FileBalance_para(Fileno).FirstOrDefault();

                //var up = db.Sp_Update_ReceivedAmount(item.Id, Modules.FileManagement.ToString(), item.TotalAmount);
                if (item == null)
                {
                    return 0;
                }
                var dis = db.Discounts.Where(x => x.Module_Id == item.Id && x.Module == Modules.FileManagement.ToString()).ToList();
                var vouch = db.Vouchers.Where(x => x.File_Plot_Id == item.Id && x.Module == Modules.FileManagement.ToString() && x.Cancel == null).ToList();
                item.TotalAmount -= vouch.Sum(x => x.Amount);
                var ReceivedAmount = item.TotalAmount;
                if (dis.Any())
                {
                    item.TotalAmount += dis.Sum(x => x.Discount_Amount);
                }
                db.Test_PendingInst(item.Id);
                var res1 = db.File_Installments.Where(x => x.File_Id == item.Id).ToList();
              /*  var inst = res1.Where(x => x.Installment_Type != "3").OrderBy(x => x.Due_Date).ToList();*/  //remove this check
                var inst = res1.Where(x => x.Installment_Type != "-1").OrderBy(x => x.Due_Date).ToList();
                var advinst = res1.Where(x => x.Installment_Type == "3").OrderBy(x => x.Due_Date).FirstOrDefault();
                List<AmountToPaidInfo> latpi = new List<AmountToPaidInfo>();
                decimal? Actamt = item.TotalAmount;
                decimal? TotalAmt = 0, AmttoPaid = 0;
                //if(advinst!= null)   //  comment this 
                //Actamt = Actamt - advinst.Amount; 
                //if (Actamt >= 0)
                //{
                //    AmountToPaidInfo atpi = new AmountToPaidInfo()
                //    {
                //        Id = advinst.Id
                //    };
                //    latpi.Add(atpi);
                //}
                //else
                //{
                //    Actamt = item.TotalAmount;
                //}
                foreach (var item1 in inst)
                {
                    AmountToPaidInfo atpi = new AmountToPaidInfo();
                    TotalAmt += item1.Amount;
                    if (Math.Round(Convert.ToDecimal(TotalAmt)) <= Math.Round(Convert.ToDecimal(Actamt)))
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
                var allids = new XElement("IS", latpi.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();
                db.Test_updateinstallment(allids);
                var curdate = DateTime.Now;
                var res3 = db.Test_FileInstallments(item.Id).ToList();
                /*   var inst1 = res3.Where(x => x.Installment_Type != "3").ToList();*/  // also
                var inst1 = res3.Where(x => x.Installment_Type != "-1").ToList();
                var id = inst1.Where(x => x.Due_Date <= curdate && x.Status != "Paid").ToList();
                var nopaidis = new XElement("IS", id.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();
                db.Test_updateNotPaidinstallment(nopaidis);
                var pendinginst = inst1.Where(x => x.Due_Date <= curdate).Sum(x => x.Amount);
                var bal = -(pendinginst - Actamt);
                db.Test_updatebalance(bal, res1.Where(x => x.Installment_Type != "10").Sum(x => x.Amount), ReceivedAmount, dis.Sum(x => x.Discount_Amount), item.Id, Modules.FileManagement.ToString(), id.Count(), advinst.Due_Date, vouch.Sum(x => x.Amount));
            }
            catch (Exception e)
            {
            }
            return 1;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchResult(string Text, int SearchOption)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Searched For " + Text + " on Files", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), SearchOption);
            if (string.IsNullOrEmpty(Text))
            {
                return Json(false);
            }
            else
            {
                var res = db.Sp_Get_File_Search(Text, SearchOption).ToList();
                return Json(res);
            }
        }
        public ActionResult FileVerification()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed File Verifications  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        public ActionResult GetFileVeriR(string FileId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res11 = db.Sp_Get_FileAppFormData(FileId).SingleOrDefault();
            if (res11.Status == 1)
            {
                this.TestAdjustIntallments(res11.Id);
            }
            var res1 = db.Sp_Get_FileAppFormData(FileId).SingleOrDefault();
            var res2 = db.Sp_Get_FileOwnerList(res1.Id).ToList();
            var res3 = db.Sp_Get_FileInstallments(res1.Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).ToList();
            db.Sp_Add_Activity(userid, "Get full Details of File  <a class='file-data' data-id=' " + FileId + "'>" + FileId + "</a>  ", "Read", Modules.FileManagement.ToString(), ActivityType.Details_Access.ToString(), res1.Id);
            var res = new FileDetailData { FileData = res1, FilesOwners = res2, FileInstallments = res3, FileReceipts = res4 };
            return PartialView(res);
        }
        public ActionResult GetFileVeriResult(string FileId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_FileAppFormData(FileId).SingleOrDefault();
            var res2 = db.Sp_Get_FileOwnerList(res1.Id).ToList();
            var res3 = db.Sp_Get_FileInstallments(res1.Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).ToList();
            var res = new FileDetailData { FileData = res1, FilesOwners = res2, FileInstallments = res3, FileReceipts = res4 };
            db.Sp_Add_Activity(userid, "Get full Details of File  <a class='file-data' data-id=' " + FileId + "'>" + FileId + "</a>  ", "Read", Modules.FileManagement.ToString(), ActivityType.Details_Access.ToString(), res1.Id);

            return View(res);
        }
        public ActionResult OpenFiles()
        {
            ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships(), "Id", "Dealership_Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return View();
        }
        //public JsonResult DealerFiles(long Id)
        //{
        //    string[] size = { "4 Marla", "6 Marla" };
        //    var res = (from x in db.File_Form
        //               join y in db.Installment_Structure on x.Installment_Plan_Id equals y.Installment_Plot_Id
        //               where size.Contains(x.Plot_Size) && y.Installment_Type == "3" && x.Status == 2 && x.Dealership_Id == Id
        //               select new BidingFiles { FormNumber = x.FileFormNumber, Plot_Size = x.Plot_Size, Amount = y.Amount }).ToList();
        //    var sum = res.Sum(x => x.Amount);
        //    var fres = new { Files = res, Sum = sum };
        //    return Json(fres);
        //}
        //[HttpPost]
        //public JsonResult BidingRegisterFiles(long Dealerid, List<ReceiptData> Receiptdata)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    string[] size = { "4 Marla", "6 Marla" };
        //    var res = (from x in db.File_Form
        //               join y in db.Installment_Structure on x.Installment_Plan_Id equals y.Installment_Plot_Id
        //               where size.Contains(x.Plot_Size) && y.Installment_Type == "3" && x.Status == 2 && x.Dealership_Id == Dealerid
        //               select new BidingFiles { Id = x.Id, FormNumber = x.FileFormNumber, Plot_Size = x.Plot_Size, Type = x.Type, Amount = y.Amount, Stat = false }).ToList();
        //    List<string> ids = new List<string>();
        //    List<ReceiptData> Holdedreceipt = new List<ReceiptData>();
        //    foreach (var rd in Receiptdata) // Receipts
        //    {
        //        decimal RemainingAmt = rd.Amount;
        //        foreach (var b in res.Where(x => x.Stat == false)) // Plots
        //        {
        //            Helpers h = new Helpers();
        //            ViewBag.TransactionId = h.RandomNumber();
        //            var advance = b.Amount;
        //            var amt = (Holdedreceipt.Any()) ? Holdedreceipt.Sum(x => x.Amount) : 0;
        //            decimal Totalamt = amt + RemainingAmt;
        //            RemainingAmt = Totalamt - Convert.ToDecimal(advance);
        //            if (RemainingAmt >= 0)
        //            {
        //                if (!Holdedreceipt.Any())
        //                {
        //                    var res3 = db.Sp_Add_Receipt(advance, GeneralMethods.NumberToWords(Convert.ToInt32(advance)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
        //                                            , rd.Father_Husband, b.FormNumber, rd.Name, rd.PaymentType, 0,
        //                                            rd.Project_Name, 0, null, b.Plot_Size, ReceiptTypes.Booking.ToString(), userid, userid, "File Booking", null, Modules.FileManagement.ToString(), Dealerid.ToString(), b.FormNumber.ToString(), rd.Block, b.Type, 0, h.RandomNumber()).FirstOrDefault();
        //                    if (rd.PaymentType != "Cash")
        //                    {
        //                        var res4 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(advance, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
        //                            Modules.FileManagement.ToString(), Types.Booking.ToString(), 1, rd.PayChqNo, b.Id, rd.Ch_bk_Pay_Date, b.FormNumber.ToString(), res3.Receipt_Id).FirstOrDefault());
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (var item in Holdedreceipt)
        //                    {
        //                        var res3 = db.Sp_Add_Receipt(item.Amount, item.AmountInWords, item.Bank, item.PayChqNo, item.Ch_bk_Pay_Date, item.Branch, rd.Mobile_1
        //                                            , rd.Father_Husband, b.FormNumber, rd.Name, item.PaymentType, 0,
        //                                                item.Project_Name, 0, null, b.Plot_Size, ReceiptTypes.Booking.ToString(), userid, userid, "File Booking", null, Modules.FileManagement.ToString(), Dealerid.ToString(), b.FormNumber.ToString(), rd.Block, b.Type, 0, h.RandomNumber()).FirstOrDefault();
        //                        if (item.PaymentType != "Cash")
        //                        {
        //                            var res4 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(item.Amount, item.Bank, item.Branch, item.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
        //                                Modules.FileManagement.ToString(), Types.Booking.ToString(), 1, item.PayChqNo, b.Id, item.Ch_bk_Pay_Date, b.FormNumber.ToString(), res3.Receipt_Id).FirstOrDefault());
        //                        }
        //                    }
        //                    Holdedreceipt.Clear();
        //                    {
        //                        var finalamt = rd.Amount - RemainingAmt;
        //                        var res3 = db.Sp_Add_Receipt(finalamt, GeneralMethods.NumberToWords(Convert.ToInt32(finalamt)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
        //                                                , rd.Father_Husband, b.FormNumber, rd.Name, rd.PaymentType, 0,
        //                                                rd.Project_Name, 0, null, b.Plot_Size, ReceiptTypes.Booking.ToString(), userid, userid, "File Booking", null, Modules.FileManagement.ToString(), Dealerid.ToString(), b.FormNumber.ToString(), rd.Block, b.Type, 0, h.RandomNumber()).FirstOrDefault();
        //                        if (rd.PaymentType != "Cash")
        //                        {
        //                            var res4 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(finalamt, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
        //                                Modules.FileManagement.ToString(), Types.Booking.ToString(), 1, rd.PayChqNo, b.Id, rd.Ch_bk_Pay_Date, b.FormNumber.ToString(), res3.Receipt_Id).FirstOrDefault());
        //                        }
        //                    }
        //                }
        //                b.Stat = true;
        //            }
        //            else
        //            {
        //                ReceiptData hold = new ReceiptData()
        //                {
        //                    Amount = Totalamt,
        //                    AmountInWords = GeneralMethods.NumberToWords(Convert.ToInt32(Totalamt)),
        //                    Bank = rd.Bank,
        //                    Block_Name = rd.Block_Name,
        //                    Branch = rd.Branch,
        //                    Block = rd.Block,
        //                    Category = rd.Category,
        //                    TotalAmount = rd.TotalAmount,
        //                    Ch_bk_Pay_Date = rd.Ch_bk_Pay_Date,
        //                    Date = rd.Date,
        //                    Father_Husband = rd.Father_Husband,
        //                    Mobile_1 = rd.Mobile_1,
        //                    PayChqNo = rd.PayChqNo,
        //                    PaymentType = rd.PaymentType,
        //                    Name = rd.Name
        //                };
        //                Holdedreceipt.Add(hold);
        //                break;
        //            }
        //        }
        //    }
        //    var data = new { Status = "2", Receiptid = Dealerid, Token = userid };
        //    return Json(data);
        //}
        public ActionResult BiddingFileRegister()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Plot_Prefrence = new SelectList(db.Sp_Get_Plot_Prefrences(), "Id", "Plot_Prefrence");
            return View();
        }
        //public JsonResult GetAppFileFormDetails_Bid(long Filefromid)
        //{
        //    Sp_Get_FileAppFormData_Result res = db.Sp_Get_FileAppFormData(Filefromid).SingleOrDefault();
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    if (res == null) { return Json(false); }
        //    FileStatus filestat = (FileStatus)res.Status;
        //    string status = filestat.ToString();
        //    FileAppFormData fa = new FileAppFormData()
        //    {
        //        Block_Id = res.Block_Id,
        //        Block_Name = res.Block_Name,
        //        Dealership_Name = res.Dealership_Name,
        //        Id = res.Id,
        //        Plot_Size = res.Plot_Size,
        //        Status = ((FileStatus)res.Status).ToString(),
        //    };
        //    var Receivedamts = db.Sp_Get_ReceivedAmounts(Filefromid, Modules.FileManagement.ToString()).ToList();
        //    var newres = new { File = fa, ReceivedAmounts = Receivedamts };
        //    return Json(newres);
        //}
        //[HttpPost]
        //public JsonResult RegisterBidingFile(Files_Transfer filedata, string FileFormNumber)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var asdf = Convert.ToInt64(FileFormNumber);
        //    var res = db.File_Form.Where(x => x.FileFormNumber == asdf).FirstOrDefault();
        //    if (res.Status != 5)
        //    {
        //        var data = new { Status = "0", Data = "Suspened" };
        //        return Json(data);
        //    }
        //    List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(filedata.File_Form_Id).ToList();
        //    if (!installmentstructure.Any())
        //    {
        //        var data = new { Status = "-1", Data = "No Installment Plan Is Found" };
        //        return Json(data);
        //    }
        //    long Fileid = Convert.ToInt64(db.Sp_Add_File(filedata.Plot_Size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
        //        , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
        //        filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP,
        //        filedata.Plot_Prefrence, filedata.File_Form_Id, filedata.Phase_Id, filedata.Block_Id, filedata.City, "Owner", filedata.Rate, filedata.Total, filedata.GrandTotal, true).FirstOrDefault());
        //    InstallmentsController ic = new InstallmentsController();
        //    var file_Installments = ic.AddInstallmentPlan(installmentstructure, "false", Convert.ToInt64(filedata.File_Form_Id), DateTime.UtcNow);
        //    if (file_Installments.Status)
        //    {
        //        db.Sp_Update_FileRate(Fileid, file_Installments.Rate, file_Installments.Total, file_Installments.Grand_Total);
        //        List<string> ids = new List<string>();
        //        SmsService smsService = new SmsService();
        //        try
        //        {
        //            smsService.SendMsg("Dear " + filedata.Name + " Your File " + FileFormNumber + " Has been live and In process for Approvals", filedata.Mobile_1);
        //        }
        //        catch (Exception)
        //        {
        //        }
        //        var data = new { Status = "1" };
        //        return Json(data);
        //    }
        //    else
        //    {
        //        var data = new { Status = "0", Data = file_Installments.Installments };
        //        return Json(data);
        //    }
        //}
        ////////////////////
        //   OVerDue Files
        ////////////////////
        public ActionResult OverDueFile()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  Overdue Files Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            var blocks = db.RealEstate_Blocks.Select(x => new NameValuestring { Value = x.Id.ToString(), Name = x.Block_Name }).ToList();
            ViewBag.Blocks = new SelectList(blocks, "Name", "Name");
            return View();
        }
        public ActionResult OverDueFiles(string Block)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Blocks = Block;
            db.Sp_Add_Activity(userid, "Accessed  Overdue Files Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        public ActionResult OverDueFilesReport(string Block)
        {
            var res1 = db.Sp_Get_Report_OverDue(Block).ToList();
            var res2 = db.Sp_Get_CancelFilesReport(Block).ToList();
            foreach (var item in res2)
            {
                item.Deduction = (item.Total * 10) / 100;
                item.Payable = (item.Received_Amount - item.Deduction);
            }
            var res = new Overdue_CancelReport { CancelFiles = res2, OverdueFiles = res1 };
            return PartialView(res);
        }
        //public ActionResult testingrep()
        //{
        //    var res2 = db.Sp_Get_CancelFilesReport().ToList();
        //    foreach (var item in res2)
        //    {
        //        item.Deduction = (item.Total * 10) / 100;
        //        item.Payable = (item.Received_Amount - item.Deduction);
        //    }
        //    return View(res2);
        //}
        public ActionResult QualifyingFiles(Search_OverDue s, string Block)
        {
            var res = db.Sp_Get_OverDueAmount_Search(s.Installments, s.S_Inst_Range, s.E_Inst_Range, s.Plot_Size, s.Dealer_Id, s.S_Range, s.E_Range, s.G_Amt, s.L_Amt, Block).ToList();
            int count = res.Count;

            // Pass the count to the ViewBag
            ViewBag.QualifyingFilesCount = count;
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Qualifying Files Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        public ActionResult FirstWarning(Search_OverDue s, string Block)
        {
            var res = db.Sp_Get_FirstWarning_File(s.Installments, s.S_Inst_Range, s.E_Inst_Range, s.Plot_Size, s.Dealer_Id, s.S_Range, s.E_Range, s.G_Amt, s.L_Amt, Block).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  First Warning Files Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        public ActionResult FirstWarning(Search_OverDue s)
        {
            var res = db.Sp_Get_FirstWarning_File(s.Installments, s.S_Inst_Range, s.E_Inst_Range, s.Plot_Size, s.Dealer_Id, s.S_Range, s.E_Range, s.G_Amt, s.L_Amt).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  Second Warning Files Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        public ActionResult SecWarning(Search_OverDue s)
        {
            var res = db.Sp_Get_SecWarning_File(s.Installments, s.S_Inst_Range, s.E_Inst_Range, s.Plot_Size, s.Dealer_Id, s.S_Range, s.E_Range, s.G_Amt, s.L_Amt).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  Second Warning Files Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        public ActionResult CancelledFiles(Search_OverDue s)
        {
            var res = db.Sp_Get_TempCancel_File(s.Installments, s.S_Inst_Range, s.E_Inst_Range, s.Plot_Size, s.Dealer_Id, s.S_Range, s.E_Range, s.G_Amt, s.L_Amt, Block).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  Canclled Files Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        public JsonResult WarningIssuesFileMove(long Id, long OwnerId, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Update_WarningLetterStatus_FileMove(Id, OwnerId, Type);
            db.Sp_Add_FileComments(Id, "Update the Letter Status to " + Type, userid, ActivityType.Plot_Status_Updation.ToString());

            return Json(true);
        }
        //public void FirstsaySecond()
        //{
        //    Search_OverDue s = new Search_OverDue();
        //    var res = db.Sp_Get_FirstWarning_File(s.Installments, s.S_Inst_Range, s.E_Inst_Range, s.Plot_Size, s.Dealer_Id, s.S_Range, s.E_Range, s.G_Amt, s.L_Amt).ToList();
        //    foreach (var item in res)
        //    {
        //        this.WarningIssues(item.Id, item.Owner_Id, "Second");
        //    }
        //}
        public JsonResult WarningIssues(long? Id, long? OwnerId, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.File_Form.Where(x => x.Id == Id).FirstOrDefault();
            this.TestAdjustIntallments(res1.Id);
            Sp_Get_CurrentOwner_File_Id_Result res = db.Sp_Get_CurrentOwner_File_Id(Id).FirstOrDefault();
            if (res.Installments > -3)
            {
                return Json(false);
            }
            db.Sp_Update_WarningLetterStatus_File(Id, OwnerId, Type);
            if (Type == "First")
            {
                db.Sp_Add_Activity(userid, "Issued First Warning Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_FileComments(Id, "Issued First Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                var msgtext =
                "Respected Customer,\n\r" +
                "Kindly note last date for the submission of your plot no:" + res1.FileFormNumber + "-" + res1.Type + " - " + res1.Block + " instalment has passed.You are requested to make payment. Failing to do so will be resulting in qualification for cancellation.\n\r" +
                "Therefore; Submit your dues timely to ensure the safety of your plot.\n\r" +
                "Best Regards,\n\r" +
                "Grand City.\n\r" +
                "042 – 111 724 786\n\r";
                try
                {
                    SmsService smsService = new SmsService();
                    smsService.SendMsg(msgtext, res.Mobile_1);
                }
                catch (Exception ee)
                {
                    EmailService e = new EmailService();
                    e.SendEmail(msgtext + " --- " + res.FileFormNumber.ToString(), "taimoor@sasystems.solutions", "Msg Not Sent");
                }
            }
            else if (Type == "Second")
            {
                db.Sp_Add_Activity(userid, "Issued Second Warning Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_FileComments(Id, "Issued Second Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                var msgtext =
               "Respected Customer,\n\r" +
               "This is to inform you that your installment is still pending against your plot no:" + res1.FileFormNumber + "-" + res1.Type + " - " + res1.Block + ". A reminder message was also sent to you on (" + string.Format("{0:dd-MMM-yyyy}", res.First_Notice) + ") along with a letter. You are requested to submit due instalments, otherwise your plot will be cancelled.\n\r" +
               "Best Regards,\n\r" +
               "Grand City.\n\r" +
               "042 – 111 724 786\n\r";
                try
                {
                    SmsService smsService = new SmsService();
                    smsService.SendMsg(msgtext, res.Mobile_1);
                }
                catch (Exception ee)
                {
                    EmailService e = new EmailService();
                    e.SendEmail(msgtext + " --- " + res.FileFormNumber.ToString(), "taimoor@sasystems.solutions", "Msg Not Sent");
                }
            }
            else if (Type == "Third")
            {
                db.Sp_Add_Activity(userid, "Issued Third Warning Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_FileComments(Id, "Issued Third Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                var msgtext =
               "Respected Customer,\n\r" +
               "This is to inform you that your installment is still pending against your plot no:" + res1.FileFormNumber + "-" + res1.Type + " - " + res1.Block + ". A reminder message was also sent to you on (" + string.Format("{0:dd-MMM-yyyy}", res.Sec_Notice) + ") along with a letter. You are requested to submit due instalments, otherwise your plot will be cancelled.\n\r" +
               "Best Regards,\n\r" +
               "Grand City.\n\r" +
               "042 – 111 724 786\n\r";
                try
                {
                    SmsService smsService = new SmsService();
                    smsService.SendMsg(msgtext, res.Mobile_1);
                }
                catch (Exception ee)
                {
                    EmailService e = new EmailService();
                    e.SendEmail(msgtext + " --- " + res.FileFormNumber.ToString(), "taimoor@sasystems.solutions", "Msg Not Sent");
                }
            }
            else if (Type == "Last")
            {
                db.Sp_Add_Activity(userid, "Issued Cancellation Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_FileComments(Id, "Issued Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
                var msgtext =
          "Respected Customer,\n\r" +
          "This is to remind you that your plot no:" + res1.FileFormNumber + "-" + res1.Type + " - " + res1.Block + " has been cancelled, because of ample amount due on your side you haven’t paid your remaining amount.\n\r" +
          "You are kindly requested to visit our Head Office and submit your complete documents for the processing of your refund.\n\r" +
          "Best Regards,\n\r" +
          "Grand City.\n\r" +
          "042 – 111 724 786\n\r";
                try
                {
                    SmsService smsService = new SmsService();
                    smsService.SendMsg(msgtext, res.Mobile_1);
                }
                catch (Exception ee)
                {
                    EmailService e = new EmailService();
                    e.SendEmail(msgtext + " --- " + res.FileFormNumber.ToString(), "taimoor@sasystems.solutions", "Msg Not Sent");
                }
            }
            return Json(true);
        }
        public ActionResult FirstWarningLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print First Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            var res = db.Sp_Get_LastOwners_File_Id(Id).ToList(); //db.Sp_Get_CurrentOwner_File_Id(Id).FirstOrDefault();
            return View(res);
        }
        public ActionResult SecondWarningLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print Second Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            db.Sp_Add_FileComments(Id, "Print Second Warning Letter", userid, ActivityType.Warning_Letter.ToString());
            Sp_Get_CurrentOwner_File_Id_Result res = db.Sp_Get_CurrentOwner_File_Id(Id).FirstOrDefault();
            return View(res);
        }
        public ActionResult CancellationLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print Cancellation Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            db.Sp_Add_FileComments(Id, "Print Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
            Sp_Get_CurrentOwner_File_Id_Result res = db.Sp_Get_CurrentOwner_File_Id(Id).FirstOrDefault();
            return View(res);
        }
        //////////////////////
        // Files Cancelation
        ////////////////////
        public ActionResult UpdateFileStatus(long FileId, string Status)
        {
            ViewBag.FileId = FileId;
            ViewBag.C_Status = Status;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult FileStatusUpdate(string Remarks, long FileId, string Status)
        {
            var userid = long.Parse(User.Identity.GetUserId());
            var res = db.File_Form.Where(x => x.Id== FileId).FirstOrDefault();
            var res1 = db.Sp_Get_FileAppFormData(res.FileFormNumber).SingleOrDefault();
            if (Status == FileStatus.Temporary_Cancelled.ToString())
            {
                db.Sp_Update_FileStatus(res1.Id, "6");
                db.Sp_Add_Activity(userid, "Updated Status to Temporary Cancelled <a class='fl-data' data-id=' " + res1.Id + "'>" + res1.Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), res1.Id);
                db.Sp_Add_FileComments(res1.Id, "Updated to Temporary Cancelled. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            }
            else if (Status == FileStatus.Cancelled.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Cancel Request <a class='plt-data' data-id=' " + res1.Id + "'>" + res1.Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), res1.Id);
                db.Sp_Add_FileComments(res1.Id, "Requested to Cancel. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                db.Sp_Update_FileStatus(res1.Id, "8");
                var res2 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
                long? OwnerId = res2.Select(x => x.Group_Tag).FirstOrDefault();
                db.Sp_Add_FileCancelation_Req(res1.Id, OwnerId, userid, Remarks, res1.FileFormNumber.ToString(), res1.Block_Name, FileStatus.Cancelled.ToString());
                var res5 = new { Status = "Requested" };
                return Json(res5);
            }
            else if (Status == FileStatus.Hold.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Hold <a class='plt-data' data-id=' " + res1.Id + "'>" + res1.Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), res1.Id);
                db.Sp_Update_FileStatus(res1.Id, "8");
                db.Sp_Add_FileComments(res1.Id, "Updated to Hold. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                db.Sp_Update_FileOwnershipStatus(res1.Id, Ownership_Status.Cancelled.ToString());
            }
            else if (Status == FileStatus.Registered.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Registered <a class='plt-data' data-id=' " + res1.Id + "'>" + res1.Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), res1.Id);
                db.Sp_Update_FileStatus(res1.Id, "1");
                db.Sp_Add_FileComments(res1.Id, "Updated to Registered. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            }
            else if (Status == FileStatus.Repurchased.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Repurchased <a class='plt-data' data-id=' " + res1.Id + "'>" + res1.Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), res1.Id);
                var res2 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
                db.Sp_Add_FileCancelation_Req(res1.Id, res2.Select(x => x.Group_Tag).FirstOrDefault(), userid, Remarks, res1.FileFormNumber.ToString(), res1.Block_Name, FileStatus.Repurchased.ToString());
                db.Sp_Add_FileComments(res1.Id, "Requested to Repurchased. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                var res5 = new { Status = "Repurchased" };
                return Json(res5);
            }
            return Json(true);
        }
        public JsonResult FileReinstate(long Id, string Remarks)
        {
            var res2 = db.Sp_Get_FileLastOwner(Id).ToList();
            var userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_FileOwnerStatus(Id, res2.Select(x => x.Group_Tag).FirstOrDefault(), 1, "Owner");
            db.Sp_Add_Activity(userid, "Updated Status to Reinstate Cancelled <a class='fl-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Id);
            db.Sp_Add_FileComments(Id, Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            db.Sp_Add_FileComments(Id, "Reinstate the Plot. \n\r", userid, ActivityType.Plot_Status_Updation.ToString());
            return Json(true);
        }
        public ActionResult CancellationReqs()
        {
            if (User.IsInRole("Files Manager"))
            {
                var res = db.Sp_Get_FilesCancelation_Req("Files Manager").ToList();
                return View(res);
            }
            else if (User.IsInRole("Finance Manager"))
            {
                var res = db.Sp_Get_FilesCancelation_Req("Finance Manager").ToList();
                return View(res);
            }
            else
            {
                var res = db.Sp_Get_FilesCancelation_Req("Files Manager").ToList();
               
                return View(res);
            }
        }
        public ActionResult FileCancellationDetails(string FileId, long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.CanId = Id;
            var res1 = db.Sp_Get_FileAppFormData(FileId).SingleOrDefault();
            var res2 = db.Sp_Get_FileOwnerList(res1.Id).ToList();
            var res3 = db.Sp_Get_FileInstallments(res1.Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).ToList();
            var res5 = db.Cancellation_Receipts.Where(x => x.File_Plot_No == res1.Id && x.Module == Modules.FileManagement.ToString()).FirstOrDefault();
            var res6 = db.File_Cancelation_Req.SingleOrDefault(x => x.Id == Id);
            var res = new FileCancelDetailData { FileData = res1, FilesOwners = res2, FileInstallments = res3, FileReceipts = res4, File_Cancel = res5, File_Req = res6 };
            db.Sp_Add_Activity(userid, "Get full Details of File  <a class='file-data' data-id=' " + FileId + "'>" + FileId + "</a>  ", "Read", Modules.FileManagement.ToString(), ActivityType.Details_Access.ToString(), res1.Id);
            return View(res);
        }
        public JsonResult VerifyReq(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var a = db.Sp_Add_Activity(userid, "Request for Verification of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Verified.ToString(), Id);
            var b = db.Sp_Add_FileComments(Id, "Request for Verification", userid, ActivityType.Plot_Verified.ToString());
            var c = db.Sp_Update_RequestForVerify_File(Id).ToString();
            var file = db.File_Form.Where(x => x.Id == Id).FirstOrDefault();
            Notifier.Notify(userid, NotifyTo.Audit, NotifierMsg.Request_For_Verification_File, new object[] { file }, NotifyType.Files.ToString());
            return Json(true);
        }
        public ActionResult VerificationRequested()
        {
            var res = db.Sp_Get_Files_Veri_Parameter("VerifyRequest").ToList();
            return View(res);
        }
        public JsonResult AuditVerifyFile(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_FileComments(Id, "Plot Verified", userid, ActivityType.Plot_Verified.ToString());
            db.SP_Update_VerifyStatus(Id, Modules.FileManagement.ToString());
            return Json(true);
        }
        public JsonResult UpdateCancelStat(long Id, long FileId, string FileNumber, string Remarks, bool? Status, decimal? Deduction, decimal? Repurchase)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_FileAppFormData(FileNumber).SingleOrDefault();
            var res2 = db.Sp_Get_FileLastOwner(FileId).ToList();
            var res3 = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).Where(x => x.Type == "Installment" || x.Type == "Booking" || x.Type == "Adjusted").ToList();
            var res7 = db.Sp_Get_FileInstallments(FileId).Where(x => x.Installment_Type != "2").Sum(x => x.Amount);
            var res6 = db.File_Cancelation_Req.SingleOrDefault(x => x.Id == Id).Type;
            var receivedamt = res3.Where(x => x.Status is null || x.Status == "Approved").Sum(x => x.Amount);
            string desc = "";
            foreach (var item in res3)
            {
                desc = desc + item.ReceiptNo + ", ";
            }
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Sp_Update_FileCancelation_Req(Id, FileId, FileNumber, Remarks, Status, "Files Manager", userid);
                    if (res6 == "Cancelled")
                    {
                        db.Sp_Add_FileComments(FileId, "Plot Cancellation is Approved With " + Deduction + "% Deduction", userid, ActivityType.Plot_Status_Updation.ToString());
                    }
                    else
                    {
                        db.Sp_Add_FileComments(FileId, "Plot Repurchased is Approved With " + Repurchase, userid, ActivityType.Plot_Status_Updation.ToString());
                    }
                    if (res1.Status == 5) // Open
                    {
                        var res4 = db.Sp_Add_Booking_Cancelation(receivedamt, res3.FirstOrDefault().Contact, res3.FirstOrDefault().Father_Name, res1.Id, res3.FirstOrDefault().Name, res7, "Meher Estate Developers",
                                        res1.Plot_Size, Cancellations.File_Cancellation.ToString(), userid, userid, desc, Modules.FileManagement.ToString(),
                                        res1.FileFormNumber.ToString(), res1.Block_Name, Deduction, Repurchase, res6, res1.Type).FirstOrDefault();
                        var res5 = new { Status = "Requested", ReceiptNo = res4.Receipt_No, ReceiptId = res4.Receipt_Id, Token = userid };
                        Transaction.Commit();
                        return Json(res5);
                    }
                    else
                    {
                        var res4 = db.Sp_Add_Booking_Cancelation(receivedamt, string.Join(" , ", res2.Select(x => x.Mobile_1)), string.Join(" , ", res2.Select(x => x.Father_Husband)), res1.Id, string.Join(" , ", res2.Select(x => x.Name)), res7, "Meher Estate Developers",
                        res1.Plot_Size, Cancellations.File_Cancellation.ToString(), userid, userid, desc, Modules.FileManagement.ToString(),
                        res1.FileFormNumber.ToString(), res1.Block_Name, Deduction, Repurchase, res6, res1.Type).FirstOrDefault();
                        var res5 = new { Status = "Requested", ReceiptNo = res4.Receipt_No, ReceiptId = res4.Receipt_Id, Token = userid };
                        Transaction.Commit();
                        return Json(res5);
                    }
                }
                catch (Exception ex)
                {
                    Error_Log r = new Error_Log()
                    {
                        InnerException = ex.InnerException.ToString(),
                        Msg = ex.Message.ToString(),
                        StackTrace = ex.StackTrace.ToString()
                    };
                    db.Error_Log.Add(r);
                    db.SaveChanges();
                    Transaction.Rollback();
                    return Json(false);
                }
            }
        }
        public ActionResult FileCancelation()
        {
            var banks = db.Bank_Accounts.Select(x => new NameValuestring { Value = x.Id.ToString(), Name = x.Bank + " - " + x.Account_Number }).ToList();
            var all = new NameValuestring()
            {
                Name = "Cash",
                Value = "0"
            };
            banks.Add(all);
            ViewBag.payAccId = new SelectList(banks.OrderBy(x => x.Value), "Value", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }
        public JsonResult FinalPlotCancelation(decimal Amount, int payAccId, long TransactionId, string instNo, DateTime? instDate, string branch, string payType, bool? Status, string Remarks, string Description, long Id, long FileId, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res1 = db.Sp_Get_FileData(FileId).SingleOrDefault();
            var res2 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
            var res5 = db.Cancellation_Receipts.Where(x => x.File_Plot_No == res1.Id && x.Module == Modules.FileManagement.ToString()).OrderByDescending(x => x.Id).FirstOrDefault();

            string Narration = "Against Cancellation of Plot no: " + res1.FileFormNumber + " Block No: " + res1.Block + "`" + Description;
            Sp_Add_Voucher_Result res = new Sp_Add_Voucher_Result();

            if (payAccId == 0)
            {
                payType = "Cash";
                PaymentTypeModel credAccount = ah.Payment_Head(payType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Sp_Update_FileCancelation_Req(Id, res1.Id, res1.FileFormNumber, Remarks, Status, "Finance Manager", userid);
                        if (Type == "Cancelled")
                        {
                            db.Sp_Add_FileComments(FileId, "Plot Cancellation is Approved", userid, ActivityType.Plot_Status_Updation.ToString());
                        }
                        else
                        {
                            db.Sp_Add_FileComments(FileId, "Plot Repurchased is Approved ", userid, ActivityType.Plot_Status_Updation.ToString());
                        }

                        res = db.Sp_Add_Voucher("-", Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), null, null, null, null, String.Join(",", res2.Select(x => x.Mobile_1)), Narration,
                                          String.Join(",", res2.Select(x => x.Father_Husband)), res1.Id, Modules.FileManagement.ToString(), String.Join(",", res2.Select(x => x.Name)), payType, "Meher Estate Developers",
                                          "", TransactionId, "Sale return Voucher", userid, null, comp.Id).FirstOrDefault();

                        string Sales = "";
                        string Receivable = "";
                        if (res1.Type == PlotType.Commercial.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Commercial.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();

                        }
                        else if (res1.Type == PlotType.Residential.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Residential.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                        }

                        var Sales_COA = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Sales, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.Block_Id);
                        var Plot_Head = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Receivable, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.Block_Id);
                        var Cancellation_COA = ah.HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId

                        decimal? Remain = res5.Plot_Total_Amount - res5.Amount;
                        decimal? Payable = res5.Amount - res5.Deduction_Amt;

                        var Sales_Debit = db.Sp_Add_Journal_Entry(res5.Plot_Total_Amount, 0, Sales_COA.Text_ChartCode + " - " + Sales_COA.Head, Sales_COA.Id, Sales_COA.Text_ChartCode, Sales_COA.Head, Narration, TransactionId, 1, userid, userid, null, null, instNo, null, instDate, null, "", null, res.Receipt_No, credAccount.VoucherType, comp.Id).FirstOrDefault();
                        var Pay_credit = db.Sp_Add_Journal_Entry(0, Payable, credAccount.PaymentData.Text_ChartCode + " - " + credAccount.PaymentData.Head, credAccount.PaymentData.Id, credAccount.PaymentData.Text_ChartCode, credAccount.PaymentData.Head, Narration, TransactionId, 1, userid, userid, null, null, instNo, credAccount.PaymentStatus, instDate, null, "", null, res.Receipt_No, credAccount.VoucherType, comp.Id).FirstOrDefault();
                        var plot_Credit = db.Sp_Add_Journal_Entry(0, Remain, Plot_Head.Text_ChartCode + " - " + Plot_Head.Head, Plot_Head.Id, Plot_Head.Text_ChartCode, Plot_Head.Head, Narration, TransactionId, 1, userid, userid, null, null, instNo, credAccount.PaymentStatus, instDate, null, "", null, res.Receipt_No, credAccount.VoucherType, comp.Id).FirstOrDefault();
                        var canc_Credit = db.Sp_Add_Journal_Entry(0, res5.Deduction_Amt, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, Narration, TransactionId, 1, userid, userid, null, null, instNo, credAccount.PaymentStatus, instDate, null, "", null, res.Receipt_No, credAccount.VoucherType, comp.Id).FirstOrDefault();



                        if (Type == "Repurchased")
                        {
                            db.Sp_Update_FileStatus(res1.Id, "5");
                            //db.Sp_Add_FileTransfer("", "Meher Estate Developers", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", res1.Id, 0, res1.Block_Id, "", res2.Rate, res2.Total, res2.GrandTotal, 0).FirstOrDefault();
                            db.Sp_Add_FileTransfer(0).FirstOrDefault();
                        }

                        Transaction.Commit();
                        return Json(new { Status = true, Msg = "Payment Voucher Successful.", VoucherId = res.Receipt_Id, Token = TransactionId });
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
                        db.Sp_Update_FileCancelation_Req(Id, res1.Id, res1.FileFormNumber, Remarks, Status, "Finance Manager", userid);
                        if (Type == "Cancelled")
                        {
                            db.Sp_Add_FileComments(FileId, "Plot Cancellation is Approved", userid, ActivityType.Plot_Status_Updation.ToString());
                        }
                        else
                        {
                            db.Sp_Add_FileComments(FileId, "Plot Repurchased is Approved ", userid, ActivityType.Plot_Status_Updation.ToString());
                        }

                        res = db.Sp_Add_Voucher("-", Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), bank.Bank, branch, instDate, instNo, String.Join(",", res2.Select(x => x.Mobile_1)), Narration,
                                          String.Join(",", res2.Select(x => x.Father_Husband)), res1.Id, Modules.FileManagement.ToString(), String.Join(",", res2.Select(x => x.Name)), payType, "Meher Estate Developers",
                                          "", TransactionId, "Sale return Voucher", userid, null, comp.Id).FirstOrDefault();



                        string Sales = "";
                        string Receivable = "";
                        if (res1.Type == PlotType.Commercial.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Commercial.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();

                        }
                        else if (res1.Type == PlotType.Residential.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Residential.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                        }

                        var Sales_COA = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Sales, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.Block_Id);
                        var Plot_Head = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Receivable, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.Block_Id);
                        var Cancellation_COA = ah.HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId

                        decimal? Remain = res5.Plot_Total_Amount - res5.Amount;
                        decimal? Payable = res5.Amount - res5.Deduction_Amt;

                        var Sales_Debit = db.Sp_Add_Journal_Entry(res5.Plot_Total_Amount, 0, Sales_COA.Text_ChartCode + " - " + Sales_COA.Head, Sales_COA.Id, Sales_COA.Text_ChartCode, Sales_COA.Head, Narration, TransactionId, 1, userid, userid, null, bank.Bank, instNo, null, instDate, null, "", null, res.Receipt_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var Pay_credit = db.Sp_Add_Journal_Entry(0, Payable, credAccount.Text_ChartCode + " - " + credAccount.Head, credAccount.Id, credAccount.Text_ChartCode, credAccount.Head, Narration, TransactionId, 1, userid, userid, null, bank.Bank, instNo, null, instDate, null, "", null, res.Receipt_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var plot_Credit = db.Sp_Add_Journal_Entry(0, Remain, Plot_Head.Text_ChartCode + " - " + Plot_Head.Head, Plot_Head.Id, Plot_Head.Text_ChartCode, Plot_Head.Head, Narration, TransactionId, 1, userid, userid, null, bank.Bank, instNo, null, instDate, null, "", null, res.Receipt_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var canc_Credit = db.Sp_Add_Journal_Entry(0, res5.Deduction_Amt, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, Narration, TransactionId, 1, userid, userid, null, bank.Bank, instNo, null, instDate, null, "", null, res.Receipt_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();




                        if (Type == "Repurchased")
                        {
                            db.Sp_Update_FileStatus(res1.Id, "5");
                            //db.Sp_Add_FileTransfer("", "Meher Estate Developers", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", res1.Id, 0, res1.Block_Id, "", res2.Rate, res2.Total, res2.GrandTotal, 0).FirstOrDefault();
                            db.Sp_Add_FileTransfer(0).FirstOrDefault();
                        }

                        var res8 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(Amount, bank.Bank, null, payType, bank.Bank, bank.Account_Number, PaymentMethodStatuses.Pending.ToString(),
                                Modules.Vendor_Payment.ToString(), "Payment Voucher", userid, instNo, FileId, instDate, res1.FileFormNumber.ToString(), res.Receipt_Id, comp.Id, Voucher_Type.BPV.ToString()).FirstOrDefault());

                        Transaction.Commit();
                        return Json(new { Status = true, Msg = "Payment Voucher Successful.", VoucherId = res.Receipt_Id, Token = TransactionId });
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
        /////////////
        // Special form Registeration
        public ActionResult SpecialRegisteration()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name", "Lahore");
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return View();
        }
        public ActionResult FileStatusView(int status)
        {
            FileStatus filestat = (FileStatus)status;
            ViewBag.Title = filestat.ToString();

            if (User.IsInRole("Sher Alam View"))
            {
                var res = (from x in db.File_Form
                           join y in db.Dealerships on x.Dealership_Id equals y.Id
                           where x.Status == status
                           select new FileData
                           {
                               Id = x.Id,
                               File_Plot_Number = x.FileFormNumber.ToString() + " - " + x.BallotedPlot_Number,
                               Dealership_Name = y.Dealership_Name,
                               Date_Reg = x.DateTime,
                               Plot_Size = x.Plot_Size,
                               Type = x.Type,
                               Block_Name = x.Block,
                               Sector = x.Sector,
                               Phase_Name = x.Phase,
                               Plot_Id = x.BallotedPlot_Id
                           }).ToList();
                ViewBag.FileStatus = status;
                ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships().Where(x => x.Status == "Registered").Select(x => new { x.Id, x.Dealership_Name }).Distinct(), "Id", "Dealership_Name");
                long userid = long.Parse(User.Identity.GetUserId());
                //db.Sp_Add_Activity(userid, "Accessed File Status Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Convert.ToInt64(status));
                return View(res);
            }
            else
            {
                var res = (from x in db.File_Form
                           join y in db.Dealerships on x.Dealership_Id equals y.Id
                           where x.Status == status && x.Block != "Sher Alam"
                           select new FileData
                           {
                               Id = x.Id,
                               File_Plot_Number = x.FileFormNumber.ToString() + " - " + x.BallotedPlot_Number,
                               Dealership_Name = y.Dealership_Name,
                               Date_Reg = x.DateTime,
                               Plot_Size = x.Plot_Size,
                               Type = x.Type,
                               Block_Name = x.Block,
                               Sector = x.Sector,
                               Phase_Name = x.Phase,
                               Plot_Id = x.BallotedPlot_Id
                           }).ToList();
                ViewBag.FileStatus = status;
                ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships().Where(x => x.Status == "Registered").Select(x => new { x.Id, x.Dealership_Name }).Distinct(), "Id", "Dealership_Name");
                long userid = long.Parse(User.Identity.GetUserId());
                //db.Sp_Add_Activity(userid, "Accessed File Status Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Convert.ToInt64(status));
                return View(res);
            }

        }

        public ActionResult FileStatusViewSA(int status)
        {
            FileStatus filestat = (FileStatus)status;
            ViewBag.Title = filestat.ToString();

            var res = (from x in db.File_Form
                       join y in db.Dealerships on x.Dealership_Id equals y.Id
                       where x.Status == status && x.Block == "Sher Alam"
                       select new FileData
                       {
                           Id = x.Id,
                           File_Plot_Number = x.FileFormNumber.ToString() + " - " + x.BallotedPlot_Number,
                           Dealership_Name = y.Dealership_Name,
                           Date_Reg = x.DateTime,
                           Plot_Size = x.Plot_Size,
                           Type = x.Type,
                           Block_Name = x.Block,
                           Sector = x.Sector,
                           Phase_Name = x.Phase,
                           Plot_Id = x.BallotedPlot_Id
                       }).ToList();
            ViewBag.FileStatus = status;
            ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships().Where(x => x.Status == "Registered").Select(x => new { x.Id, x.Dealership_Name }).Distinct(), "Id", "Dealership_Name");
            long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Add_Activity(userid, "Accessed File Status Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Convert.ToInt64(status));
            return View("FileStatusView", res);

        }
        //public JsonResult GroupFiles(long Id)
        //{
        //    var res = (from x in db.File_Form
        //               join y in db.Installment_Structure on x.Installment_Plan_Id equals y.Installment_Plot_Id
        //               where y.Installment_Type == "3" && x.Status == 2 && x.Group_Id == Id
        //               select new BidingFiles { FormNumber = x.FileFormNumber, Plot_Size = x.Plot_Size, Amount = y.Amount }).ToList();
        //    var sum = res.Sum(x => x.Amount);
        //    var fres = new { Files = res, Sum = sum };
        //    return Json(fres);
        //}

        public void UpdateInstallment(long FileId)
        {
            try
            {



                var res = db.Sp_Get_FileInstallments(FileId).Where(x => x.Installment_Type == "1").ToList();
                var last = res.OrderByDescending(x => x.Id).FirstOrDefault();
                decimal points = 0;
                decimal amount = Convert.ToDecimal(last.Amount.ToString().Split('.')[0]);
                foreach (var item in res)
                {
                    var a = item.Amount.ToString().Split('.');
                    File_Installments f = new File_Installments()
                    {
                        Amount = Convert.ToDecimal(a[0]),
                        Cancelled = item.Cancelled,
                        File_Id = item.File_Id,
                        Due_Date = item.Due_Date,
                        Id = item.Id,
                        Installment_Name = item.Installment_Name,
                        Installment_Type = item.Installment_Type,
                        Owner_Id = item.Owner_Id,
                        Paid_Date = item.Paid_Date,
                        Status = item.Status
                    };
                    using (Grand_CityEntities edb = new Grand_CityEntities())
                    {
                        edb.File_Installments.Attach(f);
                        var entry = edb.Entry(f);
                        entry.Property(e => e.Amount).IsModified = true;
                        edb.SaveChanges();
                    };
                    points += Convert.ToDecimal(a[1]) / 100;
                }
                File_Installments ff = new File_Installments()
                {
                    Amount = amount + points,
                    Cancelled = last.Cancelled,
                    File_Id = last.File_Id,
                    Due_Date = last.Due_Date,
                    Id = last.Id,
                    Installment_Name = last.Installment_Name,
                    Installment_Type = last.Installment_Type,
                    Owner_Id = last.Owner_Id,
                    Paid_Date = last.Paid_Date,
                    Status = last.Status
                };
                using (Grand_CityEntities edb = new Grand_CityEntities())
                {
                    edb.File_Installments.Attach(ff);
                    var entry = edb.Entry(ff);
                    entry.Property(e => e.Amount).IsModified = true;
                    edb.SaveChanges();
                };

            }
            catch (Exception ex)
            {

            }
        }
        public void UpdateOwners(long FileId)
        {
            var res = db.Sp_Update_CurrentOwner_File(FileId);
        }
        public JsonResult SearchFileForEmp(string Text, int SearchOption)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            if (string.IsNullOrEmpty(Text))
            {
                return Json(false);
            }
            else
            {
                var res = db.Sp_Get_File_Search(Text, SearchOption).ToList();
                return Json(res);
            }
        }
        public ActionResult FileOwnersData(long fileId)
        {
            var owners = db.Files_Transfer.Where(x => x.File_Form_Id == fileId).ToList();
            return PartialView(owners);
        }
        public ActionResult NewFileOwner(long fileId)
        {
            ViewBag.fileId = fileId;
            return View();
        }
        public JsonResult SaveNewFileOwner(OwnerData od)
        {
            try
            {
                if (od != null)
                {
                    var uid = User.Identity.GetUserId<long>();
                    var res = db.File_Form.Where(x => x.Id == od.FileId).Select(x => new { x.Phase_Id, x.Block_Id }).FirstOrDefault();
                    db.Files_Transfer.Add(new Files_Transfer
                    {
                        Ownership_Status = od.Ownership_Status,
                        City = od.City,
                        CNIC_NICOP = od.CNIC_NICOP,
                        DateTime = od.DateTime,
                        Date_Of_Birth = od.Date_Of_Birth,
                        Email = od.Email,
                        Father_Husband = od.Father_Husband,
                        File_Form_Id = od.FileId,
                        Mobile_1 = od.Mobile_1,
                        Mobile_2 = od.Mobile_2,
                        Name = od.Name,
                        Phone_Office = od.Phone_Office,
                        Postal_Address = od.Postal_Address,
                        Residential = od.Residential,
                        Residential_Address = od.Residential_Address,
                        Occupation = od.Occupation,
                        Nationality = od.Nationality,
                        Block_Id = res.Block_Id,
                        Phase_Id = res.Phase_Id
                    });
                    db.SaveChanges();
                    db.Sp_Add_FileComments(od.FileId, "New plot owner added with status : " + od.Ownership_Status + ". Owner Name : " + od.Name + " , CNIC : " + od.CNIC_NICOP, uid, ActivityType.Add_Plot_Owner.ToString());
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public JsonResult ReinstateWithTBA(long Id, string Remarks, int ReinstateType)
        {
            var res1 = db.File_Form.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Sp_Get_FileLastOwner(Id).ToList();
            var userid = long.Parse(User.Identity.GetUserId());
            if (ReinstateType == 2)
            {
                var a = db.Sp_Update_DevelopmentCharges(Id, null, Modules.FileManagement.ToString());
                var b = db.Sp_Update_FileOwnerStatus(Id, res2.Select(x => x.Group_Tag).FirstOrDefault(), 1, "Owner");
                db.Sp_Add_Activity(userid, "Updated Status to Reinstate Cancelled <a class='fl-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Id);
                db.Sp_Add_FileComments(Id, "Reinstate the Plot with Development Charges To-Be-Announced. \n\r", userid, ActivityType.Plot_Status_Updation.ToString());
                db.Sp_Add_FileComments(Id, Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            }
            else
            {
                var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
                try
                {
                    FileReinstateRequest pd = new FileReinstateRequest
                    {
                        DescriptionText = "Request for Reinstate of Plot Number : " + res1.FileFormNumber,
                        New_Status = FileStatus.Registered.ToString(),
                        Old_Status = ((FileStatus)res1.Status).ToString(),
                        Urgency = UrgencyStatus.Urgent,
                        Module = Modules.FileManagement.ToString()
                    };
                    var packed = JsonConvert.SerializeObject(pd);
                    db.MIS_Requests.Add(new MIS_Requests
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy_Id = userid,
                        CreatedBy_Name = uname,
                        Details = packed,
                        ModuleId = res1.Id,
                        ModuleType = RequestType.Reinstate_With_Fine.ToString(),
                        Status = RequestStatus.Pending.ToString(),
                        Type = RequestType.Reinstate_With_Fine.ToString()
                    });
                    db.SaveChanges();
                    return Json(new Return { Status = true, Msg = "Requested at Cash counter" });
                }
                catch (Exception ex)
                {
                    return Json(false);
                }
            }
            return Json(true);
        }
        public JsonResult RemoveDiscount(long Id, long Plot_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Discounts.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Sp_Delete_Discount(Id);
            db.Sp_Add_FileComments(Plot_Id, "Discount of Amount " + res1.Discount_Amount + " (Discount Remarks: " + res1.Remarks + ") is removed", userid, ActivityType.Delete_Receipt.ToString());
            return Json(new Return { Status = true, Msg = "Discount is Removed" });
        }
        public ActionResult FetchInstallmentData(string Filefromid)
        {
            var res1 = db.Sp_Get_FileAppFormData(Filefromid).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            if (res1 == null) { return Json(false); }
            List<Sp_Get_FileInstallments_Result> Allinstallments = db.Sp_Get_FileInstallments(res1.Id).ToList();
            if (Allinstallments.Any())
            {
                this.TestAdjustIntallments(res1.Id);
            }
            var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == res1.Id && x.Module == Modules.FileManagement.ToString()).FirstOrDefault();
            var res = db.Sp_Get_FileData(res1.Id).SingleOrDefault();
            var ownsData = db.Sp_Get_FileLastOwner(res.Id).ToList();
            var installments = Allinstallments.Where(x => x.Installment_Type == "1").ToList();
            var otherinstallments = Allinstallments.Where(x => x.Installment_Type != "1").ToList();
            var Receivedamts = db.Sp_Get_ReceivedAmounts(res.Id, Modules.FileManagement.ToString()).ToList();
            FileStatus filestat = (FileStatus)res.Status;
            string status = filestat.ToString();
            var Discount = db.Discounts.Where(x => x.Module_Id == res.Id).ToList();
            List<FileData> fa = ownsData.Select(x => new FileData()
            {
                Id = Convert.ToInt64(res.Id),
                Block_Name = res.Block,
                Dealership_Name = res.Dealership_Name,
                Development_Charges = res.Development_Charges.ToString(),
                Development_Charges_Val = res.Development_Charges,
                Phase_Name = res.Phase,
                Plot_Size = res.Plot_Size,
                Project_Name = res.Project,
                Status = ((FileStatus)res.Status).ToString(),
                City = x.City,
                CNIC_NICOP = x.CNIC_NICOP,
                Date_Of_Birth = x.Date_Of_Birth,
                Email = x.Email,
                Father_Husband = x.Father_Husband,
                Mobile_1 = x.Mobile_1,
                Mobile_2 = x.Mobile_2,
                Name = x.Name,
                Nationality = x.Nationality,
                Nominee_Address = x.Nominee_Address,
                Nominee_CNIC_NICOP = x.Nominee_CNIC_NICOP,
                Nominee_Name = x.Nominee_Name,
                Nominee_Relation = x.Nominee_Relation,
                Occupation = x.Occupation,
                Phone_Office = x.Phone_Office,
                Postal_Address = x.Postal_Address,
                Residential = x.Residential,
                Residential_Address = x.Residential_Address,
                Balance_Amount = bal.Balance_Amount,
                Rate = x.Rate,
                TotalPlotVal = x.Total,
                File_Transfer_Id = x.Id,
                Delivery = x.Delivery,
                Block_Id = x.Block_Id,
                File_Form_Number = res.FileFormNumber,
                GrandTotal = x.GrandTotal,
                Phase_Id = x.Phase_Id,
                QR_Id = res.FileFormNumber,
                Plot_Prefrence = x.Plot_Prefrence,
                Verify = res.Verify,
                Verification_Req = res.Verification_Req,
                Date_Reg = x.DateTime,
                AuditVerified = res.Verified,
                Image1 = x.Image1,
            }).ToList();
            var newres = new FileDetailsModel { File = fa, Installments = installments, OtherInstallments = otherinstallments, ReceivedAmounts = Receivedamts, Discounts = Discount };
            return PartialView(newres);
        }
        public ActionResult ReinstateFile()
        {
            return PartialView();
        }
        public ActionResult AddNewOwner(long Grp_Id, long FileId)
        {
            ViewBag.grp = Grp_Id;
            ViewBag.filid = FileId;
            return PartialView();
        }
        public JsonResult AddOwnergrp(OwnerDataGroup od)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_FileOwnerList(od.FileId).ToList().Where(x => x.Id == od.Id).FirstOrDefault();
            db.Sp_Add_FileOwnerData_grp(od.Name, od.Father_Husband, od.Postal_Address, od.Residential_Address, od.City, od.Date_Of_Birth,
                od.CNIC_NICOP, od.Nationality, od.Email, od.Phone_Office, od.Residential, od.Mobile_1, od.Mobile_2, od.Nominee_Name,
                od.Nominee_CNIC_NICOP, od.Nominee_Relation, od.Nominee_Address, od.Occupation, od.DateTime, od.FileId, "Owner", od.Group_Tag);
            return Json(true);
        }
        public JsonResult UpdateLetterStep(long Id, int Let)
        {
            var a = db.Sp_Update_WarningLetter_Steps(Id, Modules.FileManagement.ToString(), Let);
            return Json(new Return { Msg = "Letter is steped back", Status = true });
        }
        public ActionResult Noti(long? id, NotifierMsg? tp, long? noti)
        {
            Thread notiReader = new Thread(() => Notifier.ReadNotification((long)noti));
            notiReader.Start();

            if (tp == NotifierMsg.Request_For_Verification_File)
            {
                return RedirectToAction("GetFileVeriResult", "FileSystem", new { FileId = id });
            }
            else //if (tp == NotifierMsg.Item_Issued)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult File_TransferFeesConfiguration()
        {
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.File_Transfer_Fee_Config.ToString()).FirstOrDefault();
            if (res is null)
            {
                //means no configurations exists yet
                //so we're gonna create a new config
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var dummyData = db.RealEstate_Blocks.Select(x => new BlockTransferFeeModel
                {
                    BlockId = x.Id,
                    BlockName = x.Block_Name,
                    NC_Residential = 0,
                    NC_Commercial = 0,
                    NC_Residential_Dealer = 0,
                    NC_Commercial_Dealer = 0
                }).ToList();
                MIS_Modules_Configurations mmc = new MIS_Modules_Configurations
                {
                    CurrentConfig = JsonConvert.SerializeObject(dummyData),
                    Config_For_Update = null,
                    LastUpdated = DateTime.Now,
                    Module = MISModuleType.File_Transfer_Fee_Config.ToString(),
                    UpdatedBy_Id = uid,
                    UpdatedBy_Name = uname
                };
                db.MIS_Modules_Configurations.Add(mmc);
                db.SaveChanges();
                return PartialView(dummyData);
            }
            else
            {
                //config exists so throw this config to the view directly
                var parsed = JsonConvert.DeserializeObject<List<BlockTransferFeeModel>>(res.CurrentConfig);
                return PartialView(parsed);
            }
        }
        public JsonResult File_SaveBlockTransferFeeConfig(List<BlockTransferFeeModel> config)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.File_Transfer_Fee_Config.ToString()).FirstOrDefault();
                res.CurrentConfig = JsonConvert.SerializeObject(config);
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
        public ActionResult ExtraAmountRefundFiles()
        {
            if (User.IsInRole("Plots Manager"))
            {
                var res = db.Sp_Get_PlotReceiptRefund_Req("Plots Manager", Modules.FileManagement.ToString()).ToList();
                return View(res);
            }
            else if (User.IsInRole("Finance Manager"))
            {
                var res = db.Sp_Get_PlotReceiptRefund_Req("Finance Manager", Modules.FileManagement.ToString()).ToList();
                return View(res);
            }
            else
            {
                var res = db.Sp_Get_PlotReceiptRefund_Req("Plots Manager", Modules.FileManagement.ToString()).ToList();
                return View(res);
            }
        }
        public void UpdateFileBalances()
        {
            var res = db.File_Form.Where(x => x.Status == 1 || x.Status == 6).ToList();
            foreach (var item in res)
            {
                this.UpdateOwners(item.Id);
                this.TestAdjustIntallments(item.Id);
            }
        }
        public ActionResult FileReceiptRefundDetails(long FileNo, long ReqId)
        {
            var res1 = db.Sp_Get_FileFormData(FileNo).FirstOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get full Details of Plot Id  <a class='plt-data' data-id=' " + res1.Id + "'>" + res1.Id + "</a>  ", "Read", Modules.FileManagement.ToString(), ActivityType.Details_Access.ToString(), res1.Id);
            var res6 = db.RefundAmountsReqs.Where(x => x.Id == ReqId).FirstOrDefault();
            var res2 = db.Sp_Get_FileOwnerList(res1.Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(res1.Id, Modules.FileManagement.ToString()).ToList();
            var discount = db.Discounts.Where(x => x.Module_Id == res1.Id && x.Module == "FileManagement").ToList();
            TestAdjustIntallments(res1.Id);
            var res5 = db.Sp_Get_FileInstallments(res1.Id).ToList();
            var res = new FileRefundDetailData { FileData = res1, FileOwners = res2, FileInstallments = res5, FileReceipts = res4, File_Req = res6 };
            return View(res);
        }
        public ActionResult RefundAmount(long Id)
        {
            var res = db.RefundAmountsReqs.Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.RefundAmount = res.Amount;
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView();
        }
        [ValidateAntiForgeryToken]
        public JsonResult FinalRefundAmount(ReceiptData rd, string Status, string Remarks, string Description, long Id, long FileId, string Type, long ReceiptId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res1 = db.Sp_Get_FileData(FileId).FirstOrDefault();
            var res2 = db.Sp_Get_FileLastOwner(res1.Id).SingleOrDefault();
            var res3 = db.Sp_Get_Receipt_Parameter_ById(ReceiptId).FirstOrDefault();
            db.Sp_Update_PlotRefund_Req(Id, Status, "Finance Manager");
            var res = db.Sp_Add_Voucher(res2.Postal_Address, rd.Amount, rd.AmountInWords, rd.Bank, rd.Branch, rd.Ch_bk_Pay_Date, rd.PayChqNo, res2.Mobile_1, "Refund Amount against the " + res3.PaymentType + " Receipt No " + res3.ReceiptNo + "  of File No: " + res1.FileFormNumber + " Block No: " + res1.Block + "`" + Description,
                res2.Father_Husband, res1.Id, Modules.FileManagement.ToString(), res2.Name, rd.PaymentType, "Meher Estate Developers",
                 "", userid, Payments.Receipt_Refund.ToString(), userid, null, comp.Id).FirstOrDefault();
            try
            {
                AccountHandlerController de = new AccountHandlerController();
                de.Refund_Plot_Amount(rd.Amount, res1.FileFormNumber.ToString(), res1.Type, res1.Block, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, new Helpers().RandomNumber(), userid, res.Receipt_No, 1, true, AccountingModule, res1.Block_Id);
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            }
            if (rd.PaymentType != "Cash")
            {
                if (!Directory.Exists(Server.MapPath("~/Repository/Receipt_Refund_Voucher/" + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/Receipt_Refund_Voucher/"));
                }
                string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                var pathMain = Path.Combine(Server.MapPath("~/Repository/Receipt_Refund_Voucher/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                Helpers h = new Helpers();
                var Images = h.SaveBase64Image(rd.FileImage, pathMain, res.ToString());
            }
            db.Sp_Update_ReceiptCancel_Parameter(ReceiptId, Description);
            var fres = new { VoucherId = res.Receipt_Id, Token = userid };
            return Json(fres);
        }
        public ActionResult UpdateInstallmentInfo(long id)
        {
            ViewBag.id = id;
            var res = db.File_Installments.Where(x => x.File_Id == id).ToList();
            return PartialView(res);
        }
        //public JsonResult UpdateInstallmentInfoFile(long id, List<File_Installments> installmentData)
        //{
        //    var InstallmentStructureData = new XElement("InstallmentData", installmentData.Select(x => new XElement("InstallmentDataInfo",
        //        new XAttribute("InstallmentType", x.Installment_Type),
        //        new XAttribute("InstallmentName", x.Installment_Name),
        //        new XAttribute("Amount", x.Amount),
        //        new XAttribute("DueDate", x.Due_Date)))).ToString();

        //    var res = db.Sp_Update_Installment_File_Plot_Comm(id, "FileManagement", InstallmentStructureData);
        //    return Json(new { Status = true, Msg = "Updated Succesfully" });
        //}
        public JsonResult UpdateInstallmentInfoFile(long fileid, List<File_Installments> installmentData)
        {
            foreach (var item in installmentData)
            {
                if(item.Id==0)
                { 
                    var InstallmentStructureData = new XElement("InstallmentData", new XElement("InstallmentDataInfo",
                        new XAttribute("Id", item.Id),
                        new XAttribute("InstallmentType", item.Installment_Type),
                        new XAttribute("InstallmentName", item.Installment_Name),
                        new XAttribute("Amount", item.Amount),
                        new XAttribute("DueDate", item.Due_Date))).ToString();

                    // Call the stored procedure for each installment
                    var res = db.Sp_InsertUpdate_Installment_File_Plot_Comm(fileid, "FileManagement", InstallmentStructureData);
                }
                else
                {
                    var InstallmentStructureData = new XElement("InstallmentData", new XElement("InstallmentDataInfo",
                       new XAttribute("Id", item.Id),
                       new XAttribute("InstallmentType", item.Installment_Type),
                       new XAttribute("InstallmentName", item.Installment_Name),
                       new XAttribute("Amount", item.Amount),
                       new XAttribute("DueDate", item.Due_Date))).ToString();

                    // Call the stored procedure for each installment
                    var res = db.Sp_InsertUpdate_Installment_File_Plot_Comm(item.Id, "FileManagement", InstallmentStructureData);
                }
            }

            return Json(new { Status = true, Msg = "Updated Successfully" });
        }


        public ActionResult FilesPlotsReport()
        {
            ViewBag.Block = new SelectList(db.RealEstate_Blocks.Where(x => x.Block_Name != null), "Id", "Block_Name").ToList();
            return View();
        }
        public ActionResult FilesPlotsReportSearchBlock(string block)
        {
            var Total = db.Sp_Get_FilesPlots_Report_blockwise(block, FilePlotsReportType.Total.ToString()).Count();
            var Cancelled = db.Sp_Get_FilesPlots_Report_blockwise(block, FilePlotsReportType.Cancelled.ToString()).Count();
            var Registered = db.Sp_Get_FilesPlots_Report_blockwise(block, FilePlotsReportType.Registered.ToString()).Count();
            var RegisteredActive = db.Sp_Get_FilesPlots_Report_blockwise(block, FilePlotsReportType.RegisteredActive.ToString()).Count();
            var RegisteredInActive = db.Sp_Get_FilesPlots_Report_blockwise(block, FilePlotsReportType.RegisteredInActive.ToString()).Count();
            ViewBag.Total = Total;
            ViewBag.Cancelled = Cancelled;
            ViewBag.Registered = Registered;
            ViewBag.RegisteredActive = RegisteredActive;
            ViewBag.RegisteredInActive = RegisteredInActive;

            var all = db.Sp_Get_FilePlot_NDC_WDC_TBA_Report(block).ToList();
            var NDC = all.Where(x => x.Development_Charges == false).ToList();
            var WDC = all.Where(x => x.Development_Charges == true).ToList();
            var TBA = all.Where(x => x.Development_Charges == null).ToList();
            var Ballotted = db.Sp_Get_FilePlot_BallotedNotBalloted_Reprot(block, FilePlotsReportType.Ballotted.ToString()).Count();
            var NotBallotted = db.Sp_Get_FilePlot_BallotedNotBalloted_Reprot(block, FilePlotsReportType.NotBallotted.ToString()).Count();
            ViewBag.Ballotted = Ballotted;
            ViewBag.NotBallotted = NotBallotted;
            ViewBag.block = block;

            var res = new FilePlotReport { NDC = NDC, WDC = WDC, TBA = TBA };
            return PartialView(res);
        }

        public ActionResult FilesPlotsReportDetails(string id, string block)
        {
            ViewBag.id = id;
            var res = db.Sp_Get_FilesPlots_Details_Report(block, id).ToList();
            return PartialView(res);
        }
    
        public void UpdateFile_Balances()
        {
            var res = db.File_Form.Where(x => x.Status == 1).ToList();
            foreach (var item in res)
            {
                this.TestAdjustIntallments(item.Id);
            }
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
//public ActionResult ExpoRegisterFile()
//{
//    ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//    ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
//    ViewBag.Plot_Prefrence = new SelectList(db.Sp_Get_Plot_Prefrences(), "Id", "Plot_Prefrence");
//    return View();
//}
// [HttpPost]
//public JsonResult ExpoRegisterFile(Files_Transfer filedata, bool Flag, string DevCharStatus, string FileFormNumber, List<ReceiptData> Receiptdata, bool Token, DateTime? TokenExp)
//{
//    long userid = long.Parse(User.Identity.GetUserId());
//    List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(filedata.File_Form_Id).ToList();
//    string Devchar = (DevCharStatus == "true") ? "With Development Charges" : "Non Development Charges";
//    if (!installmentstructure.Any())
//    {
//        var data = new { Status = "-1", Data = "No Installment Plan Is Found" };
//        return Json(data);
//    }
//    if (Token)
//    {
//        long tokenid = Convert.ToInt64(db.Sp_Add_ExpoFile(filedata.Plot_Size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
//            , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
//            filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP,
//            filedata.Plot_Prefrence, filedata.File_Form_Id, filedata.Phase_Id, filedata.Block_Id,  filedata.City).FirstOrDefault());
//        var filedetails = new FileData
//        {
//            Plot_Size = filedata.Plot_Size,
//            Name = filedata.Name,
//            Father_Husband = filedata.Father_Husband,
//            Postal_Address = filedata.Postal_Address,
//            Residential_Address = filedata.Residential_Address,
//            Phone_Office = filedata.Phone_Office,
//            Residential = filedata.Residential,
//            Mobile_1 = filedata.Mobile_1,
//            Mobile_2 = filedata.Mobile_2,
//            Email = filedata.Email,
//            Occupation = filedata.Occupation,
//            Nationality = filedata.Nationality,
//            Date_Of_Birth = filedata.Date_Of_Birth,
//            CNIC_NICOP = filedata.CNIC_NICOP,
//            Nominee_Name = filedata.Nominee_Name,
//            Nominee_Relation = filedata.Nominee_Relation,
//            Nominee_Address = filedata.Nominee_Address,
//            Nominee_CNIC_NICOP = filedata.Nominee_CNIC_NICOP,
//            Plot_Prefrence = filedata.Plot_Prefrence,
//            Id = Convert.ToInt64(filedata.File_Form_Id),
//            Phase_Id = Convert.ToInt64(filedata.Phase_Id),
//            Block_Id = filedata.Block_Id,
//            City = filedata.City,
//            Flag = Flag
//        };
//        string FileDetails = JsonConvert.SerializeObject(filedetails);
//        decimal Totalamt = Receiptdata.Sum(x => x.Amount);
//        var res1 = db.Sp_Add_Amount_Clearance(Totalamt, FileDetails, Modules.FileManagement.ToString(), Types.Booking.ToString()).FirstOrDefault();
//        foreach (var rd in Receiptdata)
//        {
//            if (rd.PaymentType == "Online")
//                rd.PayChqNo = rd.PayChqNo + "(Bank Acc No)";
//            var res = Convert.ToInt64(db.Sp_Add_Receipt_Manual(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, filedata.Mobile_1
//                                , filedata.Father_Husband, FileFormNumber, filedata.Name, rd.PaymentType, null,
//                                Modules.FileManagement.ToString(), null, null, filedata.Plot_Size, ReceiptTypes.Booking.ToString(), filedata.File_Form_Id, userid, rd.ReceiptNo, rd.Ch_bk_Pay_Date, "", FileFormNumber).FirstOrDefault());
//        }
//        var data = new { Status = "2", Token = filedata.File_Form_Id };
//        return Json(data);
//    }
//    if (Flag)
//    {
//        long Fileid = Convert.ToInt64(db.Sp_Add_File(filedata.Plot_Size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
//            , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
//            filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP,
//            filedata.Plot_Prefrence, filedata.File_Form_Id, filedata.Phase_Id, filedata.Block_Id,  filedata.City, filedata.Rate, filedata.Total, filedata.GrandTotal, Flag).FirstOrDefault());
//        InstallmentsController ic = new InstallmentsController();
//        var file_Installments = ic.AddInstallmentPlan(installmentstructure, DevCharStatus, Fileid);
//        if (file_Installments.Status)
//        {
//            db.Sp_Update_AdvancePayment(Fileid);
//            db.Sp_Update_FileRate(Fileid, file_Installments.Rate, file_Installments.Total, file_Installments.Grand_Total);
//            List<string> ids = new List<string>();
//            foreach (var rd in Receiptdata)
//            {
//                if (rd.PaymentType == "Online")
//                    rd.PaymentType = rd.PaymentType + "(Bank Acc No)";
//                var res = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, filedata.Mobile_1
//                                    , filedata.Father_Husband, long.Parse(FileFormNumber), filedata.Name, rd.PaymentType, file_Installments.Total,
//                                    rd.Project_Name, file_Installments.Rate, null, filedata.Plot_Size, ReceiptTypes.Booking.ToString(), Fileid, userid, "File Booking", null, Modules.FileManagement.ToString(), Devchar, FileFormNumber, "Blocks").FirstOrDefault();
//                ids.Add(res.Receipt_No);
//            }
//            try
//            {
//                SmsService smsService = new SmsService();
//                smsService.SendMsg("Dear " + filedata.Name + " Your File " + FileFormNumber + " Has been live and In process for Approvals", filedata.Mobile_1);
//            }
//            catch (Exception)
//            {
//            }
//            var data = new { Status = "1", Receiptid = ids, Token = Fileid };
//            return Json(data);
//        }
//        else
//        {
//            var data = new { Status = "0", Data = file_Installments.Installments };
//            return Json(data);
//        }
//    }
//    else
//    {
//        long tokenid = Convert.ToInt64(db.Sp_Add_ExpoFile(filedata.Plot_Size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
//           , filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
//           filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP,
//           filedata.Plot_Prefrence, filedata.File_Form_Id, filedata.Phase_Id, filedata.Block_Id, filedata.QR_Code, filedata.City).FirstOrDefault());
//        var filedetails = new FileData
//        {
//            Plot_Size = filedata.Plot_Size,
//            Name = filedata.Name,
//            Father_Husband = filedata.Father_Husband,
//            Postal_Address = filedata.Postal_Address,
//            Residential_Address = filedata.Residential_Address,
//            Phone_Office = filedata.Phone_Office,
//            Residential = filedata.Residential,
//            Mobile_1 = filedata.Mobile_1,
//            Mobile_2 = filedata.Mobile_2,
//            Email = filedata.Email,
//            Occupation = filedata.Occupation,
//            Nationality = filedata.Nationality,
//            Date_Of_Birth = filedata.Date_Of_Birth,
//            CNIC_NICOP = filedata.CNIC_NICOP,
//            Nominee_Name = filedata.Nominee_Name,
//            Nominee_Relation = filedata.Nominee_Relation,
//            Nominee_Address = filedata.Nominee_Address,
//            Nominee_CNIC_NICOP = filedata.Nominee_CNIC_NICOP,
//            Plot_Prefrence = filedata.Plot_Prefrence,
//            Id = Convert.ToInt64(filedata.File_Form_Id),
//            Phase_Id = Convert.ToInt64(filedata.Phase_Id),
//            Block_Id = filedata.Block_Id,
//            QR_Id = filedata.QR_Code,
//            City = filedata.City,
//            Flag = Flag
//        };
//        string FileDetails = JsonConvert.SerializeObject(filedetails);
//        decimal Totalamt = Receiptdata.Sum(x => x.Amount);
//        var res1 = db.Sp_Add_Amount_Clearance(Totalamt, FileDetails, Modules.FileManagement.ToString(), Types.Booking.ToString()).FirstOrDefault();
//        List<string> ids = new List<string>();
//        foreach (var rd in Receiptdata)
//        {
//            var res3 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, filedata.Mobile_1
//                                , filedata.Father_Husband, long.Parse(FileFormNumber), filedata.Name, rd.PaymentType, null,
//                                rd.Project_Name, null, null, filedata.Plot_Size, ReceiptTypes.Booking.ToString(), filedata.File_Form_Id, userid, "File Booking", null, Modules.FileManagement.ToString(), Devchar, FileFormNumber, "Blocks").FirstOrDefault();
//            if (rd.PaymentType != "Cash")
//            {
//                var res2 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
//                    Modules.FileManagement.ToString(), Types.Booking.ToString(), res1, rd.PayChqNo, filedata.File_Form_Id, rd.Ch_bk_Pay_Date, FileFormNumber, res3.Receipt_Id).FirstOrDefault());
//            }
//            ids.Add(res3.Receipt_No);
//        }
//        var data = new { Status = "2", Receiptid = ids, Token = filedata.File_Form_Id };
//        return Json(data);
//    }
//}

//public JsonResult RegisterSpecialForm(long GroupId, List<ReceiptData> Receiptdata, Files_Transfer filedata)
//{
//    long userid = long.Parse(User.Identity.GetUserId());
//    var allfiles = (from x in db.File_Form
//                    join y in db.Installment_Structure on x.Installment_Plan_Id equals y.Installment_Plot_Id
//                    where y.Installment_Type == "3" && x.Status == 2 && x.Group_Id == GroupId
//                    select new { Id = x.Id, FormNumber = x.FileFormNumber, Plot_Size = x.Plot_Size, Amount = y.Amount, Type = x.Type, Block = x.Block }).OrderBy(x => x.FormNumber).ToList();
//    foreach (var item in allfiles)
//    {
//        List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(item.Id).ToList();
//        long Owner_Id = Convert.ToInt64(db.Sp_Add_File(filedata.Plot_Size, filedata.Name, filedata.Father_Husband, filedata.Postal_Address, filedata.Residential_Address
//, filedata.Phone_Office, filedata.Residential, filedata.Mobile_1, filedata.Mobile_2, filedata.Email, filedata.Occupation, filedata.Nationality,
//filedata.Date_Of_Birth, filedata.CNIC_NICOP, filedata.Nominee_Name, filedata.Nominee_Relation, filedata.Nominee_Address, filedata.Nominee_CNIC_NICOP,
//filedata.Plot_Prefrence, item.Id, filedata.Phase_Id, filedata.Block_Id, filedata.City, "Owner", filedata.Rate, filedata.Total, filedata.GrandTotal, true).FirstOrDefault());
//        InstallmentsController ic = new InstallmentsController();
//        DateTime a = DateTime.UtcNow;
//        a = a.AddMonths(3);
//        var file_Installments = ic.AddInstallmentPlan(installmentstructure, false.ToString(), Convert.ToInt64(item.Id), a);
//        db.Sp_Update_FileRate(item.Id, file_Installments.Rate, file_Installments.Total, file_Installments.Grand_Total);
//        List<string> ids = new List<string>();
//        SmsService smsService = new SmsService();
//        foreach (var rd in Receiptdata)
//        {
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            if (rd.PaymentType == "Online")
//                rd.PaymentType = rd.PaymentType + "(Bank Acc No)";
//            var res = db.Sp_Add_Receipt(item.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(item.Amount)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, filedata.Mobile_1
//                                , filedata.Father_Husband, item.FormNumber, filedata.Name, rd.PaymentType, file_Installments.Total,
//                                rd.Project_Name, file_Installments.Rate, null, item.Plot_Size, ReceiptTypes.Booking.ToString(), item.Id, 24, "File Booking", null, Modules.FileManagement.ToString(), GroupId.ToString(), item.FormNumber.ToString(), item.Block, item.Type, Owner_Id, h.RandomNumber(),"", receiptno).FirstOrDefault();
//            ids.Add(res.Receipt_No);
//        }
//    }
//    var data = new { Status = "5", Receiptid = GroupId };
//    return Json(data);
//}