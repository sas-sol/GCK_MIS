using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class TransferController : Controller
    {
        // GET: Transfer
        string AccountingModuleFP = COA_Mapper_Modules.Files_Plots.ToString();
        string AccountingModuleCom = COA_Mapper_Modules.Commercial.ToString();

        Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult PlotTransferRequest()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Dealership = new SelectList(db.Dealerships.Where(x => x.Status == "Registered").ToList(), "Id", "Dealership_Name");

            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsed = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(res.CurrentConfig);

            ViewBag.Buyer_filerRate = (double)(parsed.Buyer_FilerPercent / 100);
            ViewBag.Buyer_nonFilerRate = (double)(parsed.Buyer_NonFilerPercent / 100);

            ViewBag.Seller_filerRate = (double)(parsed.Seller_FilerPercent / 100);
            ViewBag.Seller_nonFilerRate = (double)(parsed.Seller_NonFilerPercent / 100);

            ViewBag.ExpDays = parsed.Tax_Exemption_Days;
            ViewBag.isExp = parsed.Tax_Exemption_Applicable;

            Helpers h = new Helpers();
            ViewBag.serial = h.RandomNumber();
            return View();
        }

        public JsonResult SavePlotTransferRequest(List<Plots_Transfer_Request> Plotdata, long TransactionId, long Plot_Id, string Plot_Size, bool? Dealership, decimal Plot_Price, bool? Employee_Req, bool buyExe, string buyExeRzn, bool sellExe, string sellExeRzn, string dealingDealerName)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            var res1 = db.Sp_Get_PlotInstallments(Plot_Id).Sum(x => x.Amount);
            string[] type = { "Booking", "Installment" };
            var res2 = db.Sp_Get_ReceivedAmounts(Plot_Id, Modules.PlotManagement.ToString()).Where(x => type.Contains(x.Type) && (x.Status == "Approved" || x.Status == null)).Sum(x => x.Amount);
            var disc = db.Discounts.Where(x => x.Module_Id == Plot_Id && x.Module == Modules.PlotManagement.ToString()).ToList();

            decimal res3 = 0;

            if (disc.Any())
            {
                res3 = disc.Sum(x => x.Discount_Amount);
            }
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //if (Math.Round( res1) != ( Math.Round(Convert.ToDecimal( res2)) + Math.Round(Convert.ToDecimal(res3))))
                    //{
                    //    Transaction.Rollback();
                    //    return Json(new Return { Status = false, Msg = "Cannot Transfer due to Overdue Amount"  });
                    //}
                    db.Sp_Add_PlotComments(Plot_Id, "Generate Transfer Request for Owner : " + string.Join(",", Plotdata.Select(x => x.Name).ToList()), userid, ActivityType.Transfer_Request.ToString());
                    var recNo = db.Sp_Get_ReceiptNo("Plot Transfer").FirstOrDefault();
                    List<long> resIds = new List<long>();
                    foreach (var v in Plotdata)
                    {
                        long id = Convert.ToInt64(db.Sp_Add_TransferRequestPlot(Plot_Size, v.Name, v.Father_Husband, v.Postal_Address,
                        v.Residential_Address, v.Phone_Office, v.Residential, v.Mobile_1, v.Mobile_2, v.Email, v.Occupation,
                        v.Nationality, v.Date_Of_Birth, v.CNIC_NICOP, v.Nominee_Name, v.Nominee_Relation, v.Nominee_Address,
                        v.Nominee_CNIC_NICOP, Plot_Id, v.City, Plot_Price, Dealership, Employee_Req, TransactionId, v.IsFiler, buyExe, buyExeRzn, sellExe, dealingDealerName, sellExeRzn, recNo, v.GroupTag, v.IsCompanyProperty).FirstOrDefault());

                        resIds.Add(id);
                    }
                    db.Sp_Add_Activity(userid, "Generate Transfer Request for Owner : " + string.Join(",", Plotdata.Select(x => x.Name).ToList()), "Create", Modules.PlotManagement.ToString(), ActivityType.Transfer_Request.ToString(), userid);
                    Transaction.Commit();
                    return Json(new { Status = true, Msg = "Transfer Request Generated", Transcation = TransactionId });

                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(new Return { Status = false, Msg = "Error Occured" });
                }
            }

            //db.SP_Files_Log("Add", "Request Transfer Plot " + Plotdata.Plot_Id + "", "Plot Transfer", Request.UserHostAddress, userid, Plotdata.Plot_Id);

        }
        public ActionResult PlotNDCForm(long SerialNum)
        {
            var ptr = db.Plots_Transfer_Request.Where(x => x.GroupTag == SerialNum).ToList();
            var res = db.Sp_Get_NDCFormDetails_Plot(ptr.Select(x => x.Plot_Id).FirstOrDefault()).ToList();
            //ViewBag.SerialNum = SerialNum;
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Accessecd NDC Form " + SerialNum + " At " + DateTime.Now.ToString(), "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return View(new NDC_View { CurrentTransferRequest = ptr, PreviousOwners = res });
        }
        public ActionResult PlotTransferRequestList()
        {
            var res = db.Sp_Get_TransferRequestList_Plot().ToList();
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Accessecd Plot Transfer Requests " + " At " + DateTime.Now.ToString(), "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return View(res);
        }
        public ActionResult PlotTransferDetails(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Add_Activity(userid, "", "Create", Modules.PlotManagement.ToString());

            var conf_res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsed = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(conf_res.CurrentConfig);

            ViewBag.Buyer_filerRate = (double)(parsed.Buyer_FilerPercent / 100);
            ViewBag.Buyer_nonFilerRate = (double)(parsed.Buyer_NonFilerPercent / 100);

            ViewBag.Seller_filerRate = (double)(parsed.Seller_FilerPercent / 100);
            ViewBag.Seller_nonFilerRate = (double)(parsed.Seller_NonFilerPercent / 100);


            ViewBag.ExpDays = parsed.Tax_Exemption_Days;
            ViewBag.isExp = parsed.Tax_Exemption_Applicable;


            var res5 = db.Sp_Get_TransferRequestDetails_Plot(Id).ToList();
            var Plot_Id = res5.Select(x => x.Plot_Id).FirstOrDefault();
            var res1 = db.Sp_Get_PlotDetailData(Plot_Id).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plot_Id).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plot_Id).ToList();
            string[] types = { "Booking", "Installment" };
            var res4 = db.Sp_Get_ReceivedAmounts(Plot_Id, Modules.PlotManagement.ToString()).Where(x => types.Contains(x.Type) && (x.Status == "Approved" || x.Status == null)).ToList();
            var lastInstDate = db.Plot_Installments.Where(x => x.Plot_Id == Plot_Id).OrderByDescending(x => x.DueDate).Select(x => x.DueDate).FirstOrDefault();
            res2.ForEach(x => x.Ownership_DateTime = ((x.Ownership_DateTime.Value.Date < lastInstDate.Date) ? lastInstDate : x.Ownership_DateTime));
            var res = new PlotTransferDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4, TransferTo = res5 };

            db.Sp_Add_Activity(userid, "Accessecd Plot Transfer Details of " + string.Join(res1.Plot_No, res1.Block_Name) + " " + " At " + DateTime.Now.ToString(), "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return View(res);
        }
        public JsonResult PlotCheckList(long ReqId, int Updateprop, bool Status)
        {
            try
            {
                db.Sp_Update_TransferReqCheckList_Plot(ReqId, Status, Updateprop);
                return Json(true);
            }
            catch (Exception)
            {
                return Json(true);
            }
        }
        public JsonResult PlotOkForTransfer(long Plot_Id, long Reqid, bool Blood_rel, bool Wave_off, bool OtherTransferCharges, decimal? Rate, string Remarks)
        {
            db.Sp_Update_TransferRequest_Plot(Reqid, Blood_rel, Wave_off, OtherTransferCharges, Rate, Remarks);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_PlotComments(Plot_Id, "Transfer Finalized \n\r ", userid, ActivityType.Plot_Verified.ToString());
            db.Sp_Add_Activity(userid, "Transfered Plot with Remarks " + Remarks + " " + " At " + DateTime.Now.ToString(), "Update", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return Json(true);
        }
        public ActionResult PlotTransfer()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            return View();
        }
        public ActionResult PlotTransferData(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Add_Activity(userid, "", "Create", Modules.PlotManagement.ToString());

            var conf_res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsed = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(conf_res.CurrentConfig);

            var trn_res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Transfer_Fee_Config.ToString()).FirstOrDefault();
            var transferData = JsonConvert.DeserializeObject<List<BlockTransferFeeModel>>(trn_res.CurrentConfig);

            ViewBag.Buyer_filerRate = (double)(parsed.Buyer_FilerPercent / 100);
            ViewBag.Buyer_nonFilerRate = (double)(parsed.Buyer_NonFilerPercent / 100);

            ViewBag.Seller_filerRate = (double)(parsed.Seller_FilerPercent / 100);
            ViewBag.Seller_nonFilerRate = (double)(parsed.Seller_NonFilerPercent / 100);


            ViewBag.ExpDays = parsed.Tax_Exemption_Days;
            ViewBag.isExp = parsed.Tax_Exemption_Applicable;


            ViewBag.TransferRate = 0;

            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            var res1 = db.Sp_Get_PlotDetailData(Id).SingleOrDefault();

            var res2 = db.Sp_Get_PlotLastOwner(Id).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Id, Modules.PlotManagement.ToString()).ToList();
            var res5 = db.Sp_Get_TransferRequestDetails_PlotId(Id).Select(x => new Sp_Get_TransferRequestDetails_Plot_Result
            {
                Id = x.Id,
                AllDocuments = x.AllDocuments,
                Plot_Id = x.Plot_Id,
                Amount = x.Amount,
                Blood_Realation = x.Blood_Realation,
                CashPayments = x.CashPayments,
                Cheques_dd_po = x.Cheques_dd_po,
                City = x.City,
                CNIC_NICOP = x.CNIC_NICOP,
                ComputerRecord = x.ComputerRecord,
                DateTime = x.DateTime,
                Date_Of_Birth = x.Date_Of_Birth,
                Dealer_Req = x.Dealer_Req,
                Email = x.Email,
                Employee_Req = x.Employee_Req,
                Father_Husband = x.Father_Husband,
                Mobile_1 = x.Mobile_1,
                Mobile_2 = x.Mobile_2,
                Name = x.Name,
                Nationality = x.Nationality,
                TransferRequestNo = x.TransferRequestNo,
                Nominee_Address = x.Nominee_Address,
                Nominee_CNIC_NICOP = x.Nominee_CNIC_NICOP,
                Nominee_Name = x.Nominee_Name,
                Nominee_Relation = x.Nominee_Relation,
                Occupation = x.Occupation,
                Other_Transfer_Charges = x.Other_Transfer_Charges,
                Phone_Office = x.Phone_Office,
                Plot_Price = x.Plot_Price,
                Plot_Size = x.Plot_Size,
                Postal_Address = x.Postal_Address,
                Remarks = x.Remarks,
                Residential = x.Residential,
                Residential_Address = x.Residential_Address,
                Status = x.Status,
                Wave_Off = x.Wave_Off,
                Transfer_Success = x.Transfer_Success,
                IsFiler = x.IsFiler,
                GroupTag = x.GroupTag,
                TransactionId = x.TransactionId,
                NDC_SerialNo = x.NDC_SerialNo,
                SellerExe = !string.IsNullOrEmpty(x.Filer_NonFiler),
                TaxExe = x.TaxExempted == true,
                TaxExeReason = x.TaxExemptReason,
                IsCompanyProperty = x.IsCompanyProperty
            }).ToList();
            var tdRate = transferData.Where(x => x.BlockName == res1.Block_Name).FirstOrDefault();
            if (res5.Select(x => x.Dealer_Req).FirstOrDefault() == true)
            {
                if (res1.Develop_Status == "Non Constructed")
                {
                    if (res1.Type == PlotType.Residential.ToString())
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
                    if (res1.Type == PlotType.Residential.ToString())
                    {
                        ViewBag.TransferRate = tdRate.C_Residential_Dealer;
                    }
                    else
                    {
                        ViewBag.TransferRate = tdRate.C_Commercial_Dealer;
                    }
                }
                //Agar Dealer Hai
            }
            else
            {
                //Agar Dealer Transfer nahi hai
                if (res1.Develop_Status == "Non Constructed")
                {
                    if (res1.Type == PlotType.Residential.ToString())
                    {
                        ViewBag.TransferRate = tdRate.NC_Residential;
                    }
                    else
                    {
                        ViewBag.TransferRate = tdRate.NC_Commercial;
                    }
                }
                else
                {
                    if (res1.Type == PlotType.Residential.ToString())
                    {
                        ViewBag.TransferRate = tdRate.C_Residential;
                    }
                    else
                    {
                        ViewBag.TransferRate = tdRate.C_Commercial;
                    }
                }
            }
            var res = new PlotTransferDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4, TransferTo = res5 };
            db.Sp_Add_Activity(userid, "Accessecd Plot Transfer Details of " + string.Join(res1.Plot_No, res1.Block_Name) + " " + " At " + DateTime.Now.ToString(), "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return PartialView(res);
        }
        public JsonResult SavePlotTransfer(List<Plot_Ownership> PO, ReceiptData rd, long TransferRequestId, long TransactionId, long Plot_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            List<string> Mobs = PO.Select(x => x.Mobile_1).ToList();
            long? grpTag = PO.Select(x => x.GroupTag).FirstOrDefault();
            long blk = db.Plots.Where(x => x.Id == Plot_Id).Select(x => x.Block_Id).FirstOrDefault();
            var lastOwners = db.Sp_Get_PlotLastOwner(Plot_Id).ToList();
            string[] types = { "Booking", "Installment", "Advance" };
            var lastInstDate = db.Sp_Get_ReceivedAmounts(Plot_Id, Modules.PlotManagement.ToString()).Where(x => types.Contains(x.Type)).OrderByDescending(x => x.DateTime).Select(x => x.DateTime).FirstOrDefault();
            //yean wala kaam below
            var config = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault().CurrentConfig;
            var plotTransfFee = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsedInfo = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(plotTransfFee.CurrentConfig);
            float Buyer_rateFiler = 0, Buyer_rateNonFiler = 0, Seller_rateFiler = 0, Seller_rateNonFiler = 0;
            int ExpDay = 0; bool isExp = false;
            var taxExeInfo = db.Plots_Transfer_Request.Where(x => x.Id == TransferRequestId).FirstOrDefault();
            var blockData = parsedInfo.FeeInfo.Where(x => x.BlockId == blk).FirstOrDefault();
            if (blockData != null && (blockData.IsApplicable))
            {
                Buyer_rateFiler = parsedInfo.Buyer_FilerPercent / (float)100.0;
                Buyer_rateNonFiler = parsedInfo.Buyer_NonFilerPercent / (float)100.0;

                Seller_rateFiler = parsedInfo.Seller_FilerPercent / (float)100.0;
                Seller_rateNonFiler = parsedInfo.Seller_NonFilerPercent / (float)100.0;

                ExpDay = parsedInfo.Tax_Exemption_Days;
                isExp = parsedInfo.Tax_Exemption_Applicable;
            }

            //db.SP_Files_Log("Update", "Transfered Plot " + rd.File_Plot_Number + "", "Plot Transfer", Request.UserHostAddress, userid, PO.Plot_Id);
            db.Sp_Add_PlotComments(Plot_Id, "Plot Transfered to : " + string.Join(",", PO.Select(x => x.Name)), userid, ActivityType.Transfer_Request.ToString());
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var plot = db.Sp_Get_PlotData(Plot_Id).FirstOrDefault();
                    var Owner_Id = db.Sp_Add_Transfer_Plot(grpTag).FirstOrDefault();

                    //if (plot.Verified == true)
                    //{
                    //    db.SP_Update_PlotVerificationToNull(Plot_Id);
                    //    Notifier.Notify(userid, NotifyTo.Audit, NotifierMsg.Unverified, new object[] { plot }, NotifyType.Plots.ToString());
                    //}

                    var Group_Id = PO.Select(x => x.GroupTag).FirstOrDefault();
                    var reg = PO.Select(x => x.GroupTag).FirstOrDefault().ToString() + Plot_Id.ToString();
                    reg = reg.PadLeft(6, '0');
                    db.Sp_Update_PlotRegNo(Group_Id, reg);
                    var receiptno1 = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                    var res1 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, string.Join(",", PO.Select(x => x.Mobile_1).FirstOrDefault())
                                        , string.Join("/", PO.Select(x => x.Father_Husband)), rd.File_Plot_Number, string.Join("/", PO.Select(x => x.Name)), PaymentMethod.Cash.ToString(), 0,
                                        "Meher Estate Developers", 0, null, PO.Select(x => x.Plot_Size).FirstOrDefault(), ReceiptTypes.Transfer.ToString(), userid, userid, "Plot Transfer", null, Modules.PlotManagement.ToString(), Group_Id.ToString(), rd.FilePlotNumber.ToString(), plot.Block_Name, plot.Type, Group_Id, TransactionId, "", receiptno1, comp.Id).FirstOrDefault();

                    {
                        bool headcashier = false;
                        if (User.IsInRole("Head Cashier"))
                        {
                            headcashier = true;
                        }
                        try
                        {
                            AccountHandlerController de = new AccountHandlerController();
                            de.Transfer_Plot(rd.Amount, plot.Plot_No, plot.Type, plot.Block_Name, PaymentMethod.Cash.ToString(), rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res1.Receipt_No, 1, headcashier, AccountingModuleFP, plot.BlockIden);
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        }
                    }

                    res1.IsTax = false;
                    List<Sp_Add_Receipt_Result> taxReceipts = new List<Sp_Add_Receipt_Result>();
                    Helpers h = new Helpers();
                    decimal plt_prc = 0;
                    if (blockData.IsApplicable)
                    {
                        //buyers k tax
                        if (!taxExeInfo.TaxExempted == true)
                        {
                            foreach (var v in PO)
                            {
                                int amt = 0;
                                string amtInWords = string.Empty;
                                plt_prc = (v.Total_Price is null) ? 0 : (decimal)v.Total_Price;
                                if (v.IsFiler == true)
                                {
                                    v.Total_Price = (v.Total_Price is null) ? 0 : v.Total_Price;
                                    amt = (int)Math.Ceiling(Buyer_rateFiler * ((float)v.Total_Price / PO.Count()));
                                    amtInWords = GeneralMethods.NumberToWords(amt);
                                }
                                else
                                {
                                    v.Total_Price = (v.Total_Price is null) ? 0 : v.Total_Price;
                                    amt = (int)Math.Ceiling(Buyer_rateNonFiler * ((float)v.Total_Price / PO.Count()));
                                    amtInWords = GeneralMethods.NumberToWords(amt);
                                }
                                long transId = h.RandomNumber();
                                var receiptno2 = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                                var receipt = db.Sp_Add_Receipt(amt, amtInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, v.Mobile_1
                                                , v.Father_Husband, rd.File_Plot_Number, v.Name, PaymentMethod.Cash.ToString(), plt_prc,
                                                rd.Project_Name, 0, null, v.Plot_Size, ReceiptTypes.Advance_Tax_236_K.ToString(), userid, userid, v.CNIC_NICOP,
                                                null, Modules.PlotManagement.ToString(), Group_Id.ToString(),
                                                rd.FilePlotNumber.ToString(), plot.Block_Name, plot.Type, Group_Id, transId, "", receiptno2, comp.Id).FirstOrDefault();

                                {
                                    bool headcashier = false;
                                    if (User.IsInRole("Head Cashier"))
                                    {
                                        headcashier = true;
                                    }
                                    try
                                    {
                                        Helpers H = new Helpers();
                                        AccountHandlerController de = new AccountHandlerController();
                                        de.Other_Recovery(amt, plot.Plot_No, plot.Type, plot.Block_Name, PaymentMethod.Cash.ToString(), "", null, "", "Buyer Tax of " + plot.Plot_No + " - " + plot.Type + " - " + plot.Block_Name, TransactionId, userid, receipt.Receipt_No, 1, COA_Mapper_ModuleTypes.Advance_Tax_236_K.ToString(), headcashier, AccountingModuleFP);
                                        //de.Tax(amt, plot.Plot_No, plot.Type, plot.Block_Name, PaymentMethod.Cash.ToString(), rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, H.RandomNumber(), userid, receipt.Receipt_No, 1, headcashier);
                                    }
                                    catch (Exception ex)
                                    {
                                        Transaction.Rollback();
                                        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                    }
                                }



                                receipt.IsTax = true;
                                taxReceipts.Add(receipt);
                            }
                        }

                        //yahan sellers k tax

                        if (string.IsNullOrEmpty(taxExeInfo.Filer_NonFiler))
                        {
                            plt_prc = (PO.Select(x => x.Total_Price).FirstOrDefault() is null) ? 0 : (decimal)PO.Select(x => x.Total_Price).FirstOrDefault();
                            foreach (var v in lastOwners)
                            {
                                int amt = 0;
                                string amtInWords = string.Empty;
                                v.Ownership_DateTime = (v.Ownership_DateTime.Value.Date < lastInstDate.Value.Date) ? lastInstDate : v.Ownership_DateTime;
                                TimeSpan t = (DateTime.Now - v.Ownership_DateTime.Value);
                                if (v.IsFiler == true)//3653 = 4 years
                                {
                                    amt = (int)Math.Ceiling(Seller_rateFiler * ((float)plt_prc / lastOwners.Count()));
                                    amtInWords = GeneralMethods.NumberToWords(amt);
                                }
                                else
                                {
                                    amt = (int)Math.Ceiling(Seller_rateNonFiler * ((float)plt_prc / lastOwners.Count()));
                                    amtInWords = GeneralMethods.NumberToWords(amt);
                                }
                                if (isExp)
                                {
                                    if ((t.TotalDays <= ExpDay))
                                    {
                                        long transId = h.RandomNumber();
                                        var receiptno3 = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                                        var receipt = db.Sp_Add_Receipt(amt, amtInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, v.Mobile_1
                                                        , v.Father_Husband, rd.File_Plot_Number, v.Name, PaymentMethod.Cash.ToString(), plt_prc,
                                                        rd.Project_Name, 0, null, v.Plot_Size, ReceiptTypes.Advance_Tax_236_C.ToString(), userid, userid, v.CNIC_NICOP,
                                                        null, Modules.PlotManagement.ToString(), Group_Id.ToString(),
                                                        rd.FilePlotNumber.ToString(), plot.Block_Name, plot.Type, Group_Id, transId, "", receiptno3, comp.Id).FirstOrDefault();
                                        receipt.IsTax = true;
                                        taxReceipts.Add(receipt);
                                        {
                                            bool headcashier = false;
                                            if (User.IsInRole("Head Cashier"))
                                            {
                                                headcashier = true;
                                            }
                                            try
                                            {
                                                Helpers H = new Helpers();
                                                AccountHandlerController de = new AccountHandlerController();
                                                //de.Tax(amt, rd.FilePlotNumber, plot.Type, plot.Block_Name, PaymentMethod.Cash.ToString(), rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, transId, userid, receipt.Receipt_No, 1, headcashier);
                                                de.Other_Recovery(amt, plot.Plot_No, plot.Type, plot.Block_Name, PaymentMethod.Cash.ToString(), "", null, "", "Seller Tax of " + plot.Plot_No + " - " + plot.Type + " - " + plot.Block_Name, transId, userid, receipt.Receipt_No, 1, COA_Mapper_ModuleTypes.Advance_Tax_236_C.ToString(), headcashier, AccountingModuleFP);
                                            }
                                            catch (Exception ex)
                                            {
                                                Transaction.Rollback();
                                                db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        db.Sp_Add_PlotComments(rd.File_Plot_Number, "Seller is tax exempted since he is the owner of the property for " + ExpDay + " Days.", 0, ActivityType.Add_Receipt.ToString());
                                    }
                                }
                                else
                                {
                                    long transId = h.RandomNumber();
                                    var receiptno3 = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                                    var receipt = db.Sp_Add_Receipt(amt, amtInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, v.Mobile_1
                                                    , v.Father_Husband, rd.File_Plot_Number, v.Name, PaymentMethod.Cash.ToString(), plt_prc,
                                                    rd.Project_Name, 0, null, v.Plot_Size, ReceiptTypes.Advance_Tax_236_C.ToString(), userid, userid, v.CNIC_NICOP,
                                                    null, Modules.PlotManagement.ToString(), Group_Id.ToString(),
                                                    rd.FilePlotNumber.ToString(), plot.Block_Name, plot.Type, Group_Id, transId, "", receiptno3, comp.Id).FirstOrDefault();
                                    receipt.IsTax = true;
                                    taxReceipts.Add(receipt);
                                    {
                                        bool headcashier = false;
                                        if (User.IsInRole("Head Cashier"))
                                        {
                                            headcashier = true;
                                        }
                                        try
                                        {
                                            Helpers H = new Helpers();
                                            AccountHandlerController de = new AccountHandlerController();
                                            //de.Tax(amt, rd.FilePlotNumber, plot.Type, plot.Block_Name, PaymentMethod.Cash.ToString(), rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, transId, userid, receipt.Receipt_No, 1, headcashier);
                                            de.Other_Recovery(amt, plot.Plot_No, plot.Type, plot.Block_Name, PaymentMethod.Cash.ToString(), "", null, "", "Seller Tax of " + plot.Plot_No + " - " + plot.Type + " - " + plot.Block_Name, transId, userid, receipt.Receipt_No, 1, COA_Mapper_ModuleTypes.Advance_Tax_236_C.ToString(), headcashier, AccountingModuleFP);
                                        }
                                        catch (Exception ex)
                                        {
                                            Transaction.Rollback();
                                            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    taxReceipts.Add(res1);

                    db.Sp_Add_PlotComments(Plot_Id, "Plot is Transfered to " + string.Join(",", PO.Select(x => x.Name)), userid, ActivityType.Plot_Transfered.ToString());
                    db.Sp_Add_Activity(userid, "Plot is Transfered to " + string.Join(",", PO.Select(x => x.Name)) + " " + " At " + DateTime.Now.ToString(), "Update", Modules.PlotManagement.ToString(), ActivityType.Plot_Transfered.ToString(), Plot_Id);

                    db.Sp_Update_ReceiptOwner(Plot_Id, Group_Id);
                    db.Sp_Update_OwnerTransferReceipt(Group_Id, res1.Receipt_Id, Modules.PlotManagement.ToString());
                    Transaction.Commit();
                    var text = "Dear " + string.Join("-", PO.Select(x => x.Name)) + ",\n\r" +
                           "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in cash for Plot number " + plot.Plot_No + " - " + plot.Sector + " Block: " + plot.Block_Name + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                    try
                    {

                        SmsService smsService = new SmsService();
                        foreach (var v in Mobs)
                        {
                            smsService.SendMsg(text, v);
                        }
                    }
                    catch (Exception ex)
                    {
                        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                    }
                    var res = new { ReceiptId = grpTag, Token = userid };
                    return Json(res);
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                    Transaction.Rollback();
                    return Json(false);
                }
            }
        }
        public ActionResult RepurchaseTransfer(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_PlotData(Id).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Id).FirstOrDefault();
            db.Sp_Add_PlotComments(Id, "Generated Plot Transfer Document. \n\r", userid, ActivityType.Plot_Transfered.ToString());
            db.Sp_Add_Activity(userid, "Generated Plot Transfer Document. \n\r" + " " + " At " + DateTime.Now.ToString(), "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Transfered.ToString(), Id);

            Sp_Get_NDCFormDetails_Plot_Result S = new Sp_Get_NDCFormDetails_Plot_Result()
            {
                Block_Name = res1.Block_Name,
                Dimension = res1.Dimension,
                From_CNIC_NICOP = res2.CNIC_NICOP,
                From_FatherName = res2.Father_Husband,
                From_Mobile1 = res2.Mobile_1,
                From_Name = res2.Name,
                Plot_Size = res1.Plot_Size,
                Plot_Location = res1.Plot_Location,
                Plot_Number = res1.Plot_No,
                Postal_Address = res2.Postal_Address,
                To_CNIC_NICOP = "-",
                To_FatherName = "-",
                To_Mobile1 = "-",
                To_Name = "Meher Estate Developers",
                Type = res1.Type
            };
            return View(S);
        }
        public ActionResult FilesTransfersList()
        {
            var res = db.Sp_Get_FilesTransferList().ToList();
            return View(res);
        }
        public ActionResult FileTransferLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res3 = db.Files_Transfer.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.File_Form.Where(x => x.Id == res3.File_Form_Id).FirstOrDefault();
            var disc = db.Discounts.Where(x => x.Module == Modules.FileManagement.ToString() && x.Module_Id == res3.File_Form_Id).ToList();
            string[] res6 = { "Booking", "Installment" };
            var lastOwns = db.Sp_Get_FileLastOwner(res3.File_Form_Id).ToList();
            var seclastowns = db.Sp_Get_FileSecondLastOwner(res3.File_Form_Id).ToList();
            var transdate = lastOwns.Select(x => x.DateTime).FirstOrDefault();
            var res4 = db.Sp_Get_ReceivedAmounts(res3.File_Form_Id, Modules.FileManagement.ToString()).Where(x => x.DateTime <= transdate && (x.Status == null || x.Status == "Approved") && (res6.Contains(x.Type))).ToList().Sum(x => x.Amount);
            var totalPlotValue = db.File_Installments.Where(x => x.File_Id == res3.File_Form_Id).Sum(x => x.Amount);
            var res5 = db.Sp_Update_Received_AmountTransfer(Id, res4, Modules.FileManagement.ToString());
            var updtStat = db.Sp_Update_TransferPaperStatus(lastOwns.Select(x => x.Group_Tag).FirstOrDefault());
            var res1 = new TransferLetter
            {
                //Id = x.Id,
                Owner_Id = lastOwns.Select(x => x.Group_Tag).FirstOrDefault(),
                Block = res2.Block,
                CNIC_NICOP = string.Join(" , ", lastOwns.Select(x => x.CNIC_NICOP)),
                Plot_No = res2.FileFormNumber.ToString(),
                Mobile_1 = string.Join(" , ", lastOwns.Select(x => x.Mobile_1)),
                Name = string.Join(" , ", lastOwns.Select(x => x.Name)),
                Total = totalPlotValue,
                Transfer_From = seclastowns.Select(x => x.Name).FirstOrDefault(),
                T_CNIC = seclastowns.Select(x => x.CNIC_NICOP).FirstOrDefault(),
                T_Mobile1 = seclastowns.Select(x => x.Mobile_1).FirstOrDefault(),
                Size = res2.Plot_Size,
                Transfer_Date = transdate,
                Received = res4,
                IsCompanyProperty = lastOwns.Any(x => x.IsCompanyProperty == true),
                Discount = disc.Sum(x => x.Discount_Amount)
            };
            db.Sp_Add_Activity(userid, "Generate File Transfer Letter of this : " + Id, "Create", Modules.FileManagement.ToString(), "File transfer Letter", userid);

            return View(res1);
        }
        public ActionResult PlotsTransfersList()
        {
            var res = db.Sp_Get_PlotsTransferList().ToList();
            return View(res);
        }
        public ActionResult PlotsTransferLetter(long Id)
        {
            var res3 = db.Plot_Ownership.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotData(res3.Plot_Id).FirstOrDefault();
            string[] res6 = { "Booking", "Advance", "Installment" };
            var res4 = db.Sp_Get_ReceivedAmounts(res3.Plot_Id, Modules.PlotManagement.ToString()).Where(x => x.DateTime <= res3.Ownership_DateTime && (x.Status == null || x.Status == "Approved") && (res6.Contains(x.Type))).ToList().Sum(x => x.Amount);
            var disc = db.Discounts.Where(x => x.Module == Modules.PlotManagement.ToString() && x.Module_Id == res3.Plot_Id).ToList();

            var totalPlotValue = db.Plot_Installments.Where(x => x.Plot_Id == res3.Plot_Id).Sum(x => x.Amount);
            var lastOwns = db.Sp_Get_PlotLastOwner(res3.Plot_Id).ToList();
            var transdate = lastOwns.Select(x => x.Ownership_DateTime).FirstOrDefault();
            var seclastowns = db.Sp_Get_PlotSecondLastOwner(res3.Plot_Id).ToList();

            var res1 = new TransferLetter
            {
                //Id = x.Id,
                Owner_Id = lastOwns.Select(x => x.GroupTag).FirstOrDefault(),
                Block = res2.Block_Name,
                CNIC_NICOP = string.Join(" , ", lastOwns.Select(x => x.CNIC_NICOP)),
                Plot_No = res2.Plot_No,
                Mobile_1 = string.Join(" , ", lastOwns.Select(x => x.Mobile_1)),
                Name = string.Join(" , ", lastOwns.Select(x => x.Name)),
                Total = totalPlotValue,
                Transfer_From = seclastowns.Select(x => x.Name).FirstOrDefault(),
                T_CNIC = seclastowns.Select(x => x.CNIC_NICOP).FirstOrDefault(),
                T_Mobile1 = seclastowns.Select(x => x.Mobile_1).FirstOrDefault(),
                Size = res2.Plot_Size,
                Transfer_Date = transdate,
                Received = res4,
                IsCompanyProperty = lastOwns.Any(x => x.IsCompanyProperty == true),
                Discount = disc.Sum(x => x.Discount_Amount)
            };


            return View(res1);
        }
        // Send Back to Manager 
        public JsonResult SendBackToManager(long Id, string Module)
        {
            var a = db.Sp_Update_TransferReq_Back(Id, Module);
            var res = new { Status = true, Msg = "Sent Back to Manager" };
            return Json(res);
        }
        public JsonResult SendBackToCounter(long Id, string Module)
        {
            var a = db.Sp_Update_TransferReq_Back_Counter(Id, Module);
            var res = new { Status = true, Msg = "Sent Back to Counter" };
            return Json(res);
        }
        public JsonResult UpdatePlotTransferFilerStatus(long owner, bool filer)
        {
            var res = db.Plots_Transfer_Request.Where(x => x.Id == owner).FirstOrDefault();

            if (res is null)
            {
                return Json(false);
            }

            res.IsFiler = filer;
            db.Plots_Transfer_Request.Attach(res);
            db.Entry(res).Property(x => x.IsFiler).IsModified = true;
            db.SaveChanges();
            return Json(true);
        }
        public ActionResult TaxCalculator()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }

        public JsonResult UpdateTaxExemptionStatus(long grp, bool tp, bool enable, string rzn)
        {
            try
            {
                var trans = db.Plots_Transfer_Request.Where(x => x.GroupTag == grp).ToList();
                if (tp == true)
                {
                    //buyer
                    foreach (var v in trans)
                    {
                        v.TaxExempted = (enable == true) ? true : false;
                        v.TaxExemptReason = rzn;
                        db.SaveChanges();
                    }
                }
                else
                {
                    //seller
                    foreach (var v in trans)
                    {
                        v.Filer_NonFiler = (enable == true) ? "Exempted" : string.Empty;
                        v.SellExemptReason = rzn;
                        db.SaveChanges();
                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        //
        //
        //Commercial Transfer Module
        //
        //

        public ActionResult CommercialTransferRequest()
        {

            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == "Building").ToList(), "Id", "Project_Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Dealership = new SelectList(db.Dealerships.Where(x => x.Status == "Registered").ToList(), "Id", "Dealership_Name");

            Helpers h = new Helpers();
            ViewBag.serial = h.RandomNumber();
            return View();
        }

        public JsonResult SaveCommercialTransferRequest(List<Plots_Transfer_Request> Plotdata, long TransactionId, long Plot_Id, string Plot_Size, bool? Dealership, decimal? Plot_Price, bool? Employee_Req, bool buyExe, string buyExeRzn, bool sellExe, string sellExeRzn, string dealingDealerName)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            var res1 = db.Sp_Get_CommercialInstallments(Plot_Id).Sum(x => x.Amount);
            string[] type = { "Advance", "Installment" };
            var res2 = db.Sp_Get_ReceivedAmounts(Plot_Id, Modules.CommercialManagement.ToString()).Where(x => type.Contains(x.Type) && (x.Status == "Approved" || x.Status == null)).Sum(x => x.Amount);
            var disc = db.Discounts.Where(x => x.Module_Id == Plot_Id && x.Module == Modules.CommercialManagement.ToString()).ToList();

            decimal res3 = 0;

            if (disc.Any())
            {
                res3 = disc.Sum(x => x.Discount_Amount);
            }
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //if (Math.Round( res1) != ( Math.Round(Convert.ToDecimal( res2)) + Math.Round(Convert.ToDecimal(res3))))
                    //{
                    //    Transaction.Rollback();
                    //    return Json(new Return { Status = false, Msg = "Cannot Transfer due to Overdue Amount"  });
                    //}
                    db.Sp_Add_PlotComments(Plot_Id, "Generate Transfer Request for Owner : " + string.Join(",", Plotdata.Select(x => x.Name).ToList()), userid, ActivityType.Transfer_Request.ToString());
                    var recNo = db.Sp_Get_ReceiptNo("Commercial Transfer").FirstOrDefault();
                    List<long> resIds = new List<long>();
                    foreach (var v in Plotdata)
                    {
                        long id = Convert.ToInt64(db.Sp_Add_TransferRequestCommercial(Plot_Size, v.Name, v.Father_Husband, v.Postal_Address,
                        v.Residential_Address, v.Phone_Office, v.Residential, v.Mobile_1, v.Mobile_2, v.Email, v.Occupation,
                        v.Nationality, v.Date_Of_Birth, v.CNIC_NICOP, v.Nominee_Name, v.Nominee_Relation, v.Nominee_Address,
                        v.Nominee_CNIC_NICOP, Plot_Id, v.City, Plot_Price, Dealership, Employee_Req, TransactionId, v.IsFiler, buyExe, buyExeRzn, sellExe, dealingDealerName, sellExeRzn, recNo, v.GroupTag, v.IsCompanyProperty).FirstOrDefault());

                        resIds.Add(id);
                    }
                    db.Sp_Add_Activity(userid, "Generate Transfer Request for Owner : " + string.Join(",", Plotdata.Select(x => x.Name).ToList()), "Create", Modules.CommercialManagement.ToString(), ActivityType.Transfer_Request.ToString(), userid);
                    Transaction.Commit();
                    return Json(new { Status = true, Msg = "Transfer Request Generated", Transcation = TransactionId });

                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(new Return { Status = false, Msg = "Error Occured" });
                }
            }

            //db.SP_Files_Log("Add", "Request Transfer Plot " + Plotdata.Plot_Id + "", "Plot Transfer", Request.UserHostAddress, userid, Plotdata.Plot_Id);

        }
        public ActionResult CommercialNDCForm(long SerialNum)
        {
            var ptr = db.Commercial_Transfer_Request.Where(x => x.GroupTag == SerialNum).ToList();
            var res = db.Sp_Get_NDCFormDetails_Commercial(ptr.Select(x => x.Com_Id).FirstOrDefault()).ToList();
            //ViewBag.SerialNum = SerialNum;
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Accessecd NDC Form " + SerialNum + " At " + DateTime.Now.ToString(), "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return View(new NDC_View_Commercial { CurrentTransferRequest = ptr, PreviousOwners = res });
        }
        public ActionResult CommercialTransferRequestList()
        {
            var res = db.Sp_Get_TransferRequestList_Commercial().ToList();
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Accessecd Commercial Transfer Requests " + " At " + DateTime.Now.ToString(), "Read", Modules.CommercialManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return View(res);
        }
        public ActionResult CommercialTransferDetails(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Add_Activity(userid, "", "Create", Modules.PlotManagement.ToString());

            var conf_res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsed = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(conf_res.CurrentConfig);

            ViewBag.Buyer_filerRate = (double)(parsed.Buyer_FilerPercent / 100);
            ViewBag.Buyer_nonFilerRate = (double)(parsed.Buyer_NonFilerPercent / 100);

            ViewBag.Seller_filerRate = (double)(parsed.Seller_FilerPercent / 100);
            ViewBag.Seller_nonFilerRate = (double)(parsed.Seller_NonFilerPercent / 100);



            var res5 = db.Sp_Get_TransferRequestDetails_ComId(Id).ToList();
            var Com_Id = res5.Select(x => x.Com_Id).FirstOrDefault();
            var res1 = db.Sp_Get_CommercialData(Com_Id).SingleOrDefault();
            var res2 = db.Sp_Get_CommercialLastOwner(Com_Id).ToList();
            var res3 = db.Sp_Get_CommercialInstallments(Com_Id).ToList();
            string[] types = { "Booking", "Installment" };
            var res4 = db.Sp_Get_ReceivedAmounts(Com_Id, Modules.CommercialManagement.ToString()).Where(x => types.Contains(x.Type) && (x.Status == "Approved" || x.Status == null)).ToList();
            var lastInstDate = db.Commercial_Installments.Where(x => x.Com_Id == Com_Id).OrderByDescending(x => x.Due_Date).Select(x => x.Due_Date).FirstOrDefault();
            res2.ForEach(x => x.DateTime = ((x.DateTime.Value.Date < lastInstDate.Date) ? lastInstDate : x.DateTime));
            var res6 = db.Discounts.Where(x => x.Module == Modules.CommercialManagement.ToString() && x.Module_Id == Com_Id).ToList();
            var res7 = db.File_Plot_Balance.Where(x => x.Module == Modules.CommercialManagement.ToString() && x.File_Plot_Id == Com_Id).FirstOrDefault();
            var res = new CommercialTransferDetailData { Data = res1, Owners = res2, Installments = res3, Receipts = res4, TransferTo = res5, Balance = res7, Discounts = res6 };

            db.Sp_Add_Activity(userid, "Accessecd Plot Transfer Details of " + string.Join(res1.shop_no, res1.Floor) + " " + " At " + DateTime.Now.ToString(), "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return View(res);
        }
        public JsonResult CommercialCheckList(long ReqId, int Updateprop, bool Status)
        {
            try
            {
                db.Sp_Update_TransferReqCheckList_Commercial(ReqId, Status, Updateprop);
                return Json(true);
            }
            catch (Exception)
            {
                return Json(true);
            }
        }
        public JsonResult CommercialOkForTransfer(long Reqid, bool Blood_rel, bool Wave_off, bool OtherTransferCharges, decimal? Rate, string Remarks)
        {
            db.Sp_Update_TransferRequest_Commercial(Reqid, Blood_rel, Wave_off, OtherTransferCharges, Rate, Remarks);
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Transfered Plot with Remarks " + Remarks + " " + " At " + DateTime.Now.ToString(), "Update", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return Json(true);
        }
        public ActionResult CommercialTransfer()
        {
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()).ToList(), "Id", "Project_Name");
            return View();
        }
        public ActionResult CommercialTransferData(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Add_Activity(userid, "", "Create", Modules.PlotManagement.ToString());

            var conf_res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsed = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(conf_res.CurrentConfig);

            var trn_res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Commercial_Transfer_Fee_Config.ToString()).FirstOrDefault();
            var transferData = JsonConvert.DeserializeObject<List<CommercialTransferFeeModel>>(trn_res.CurrentConfig);

            ViewBag.Buyer_filerRate = (double)(parsed.Buyer_FilerPercent / 100);
            ViewBag.Buyer_nonFilerRate = (double)(parsed.Buyer_NonFilerPercent / 100);

            ViewBag.Seller_filerRate = (double)(parsed.Seller_FilerPercent / 100);
            ViewBag.Seller_nonFilerRate = (double)(parsed.Seller_NonFilerPercent / 100);


            ViewBag.TransferRate = 0;

            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            var res1 = db.Sp_Get_CommercialData(Id).SingleOrDefault();

            var res2 = db.Sp_Get_CommercialLastOwner(Id).ToList();
            var res3 = db.Sp_Get_CommercialInstallments(Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Id, Modules.CommercialManagement.ToString()).ToList();
            var res5 = db.Sp_Get_TransferRequestDetails_ComId(Id).ToList();
            var res6 = db.Discounts.Where(x => x.Module == Modules.CommercialManagement.ToString() && x.Module_Id == Id).ToList();
            var res7 = db.File_Plot_Balance.Where(x => x.Module == Modules.CommercialManagement.ToString() && x.File_Plot_Id == Id).FirstOrDefault();



            var tdRate = transferData.Where(x => x.ProjectName == res1.Project_Name && x.ComType == res1.Type).FirstOrDefault();
            if (res5.Select(x => x.Dealer_Req).FirstOrDefault() == true)
            {
                //Agar Dealer Hai
                if (tdRate.TransferAmountType == "Fixed")
                {
                    ViewBag.TransferRate = tdRate.Dealer;
                }
                else
                {
                    ViewBag.TransferRate = res3.Sum(x => x.Amount) * (tdRate.Dealer * Convert.ToDecimal(0.01));
                }
            }
            else
            {
                //Agar Dealer Transfer nahi hai
                if (tdRate.TransferAmountType == "Fixed")
                {
                    ViewBag.TransferRate = tdRate.Non_Dealer;
                }
                else
                {
                    ViewBag.TransferRate = res3.Sum(x => x.Amount) * (tdRate.Non_Dealer * Convert.ToDecimal(0.01));
                }
            }
            var res = new CommercialTransferDetailData { Data = res1, Owners = res2, Installments = res3, Receipts = res4, TransferTo = res5, Balance = res7, Discounts = res6 };
            db.Sp_Add_Activity(userid, "Accessecd Plot Transfer Details of " + string.Join(res1.shop_no, res1.Floor) + " " + " At " + DateTime.Now.ToString(), "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), userid);

            return PartialView(res);
        }
        public JsonResult SaveCommercialTransfer(ReceiptData rd, long TransferRequestId, long TransactionId, long Com_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res1 = db.Sp_Get_CommercialData(Com_Id).SingleOrDefault();
            var lastOwners = db.Sp_Get_CommercialLastOwner(Com_Id).ToList();
            var res5 = db.Sp_Get_TransferRequestDetails_ComId(Com_Id).ToList();

            List<string> Mobs = res5.Select(x => x.Mobile_1).ToList();
            long? grpTag = res5.Select(x => x.GroupTag).FirstOrDefault();
            string[] types = { "Booking", "Installment", "Advance" };
            var lastInstDate = db.Sp_Get_ReceivedAmounts(Com_Id, Modules.CommercialManagement.ToString()).Where(x => types.Contains(x.Type)).OrderByDescending(x => x.DateTime).Select(x => x.DateTime).FirstOrDefault();
            //yean wala kaam below
            var config = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault().CurrentConfig;
            var plotTransfFee = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsedInfo = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(plotTransfFee.CurrentConfig);
            db.Sp_Add_CommercialComments(Com_Id, "Transfered to : " + string.Join(",", res5.Select(x => x.Name)), userid, ActivityType.Transfer_Request.ToString());
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var Owner_Id = db.Sp_Add_Transfer_Commercial(grpTag).FirstOrDefault();

                    //if (plot.Verified == true)
                    //{
                    //    db.SP_Update_PlotVerificationToNull(Plot_Id);
                    //    Notifier.Notify(userid, NotifyTo.Audit, NotifierMsg.Unverified, new object[] { plot }, NotifyType.Plots.ToString());
                    //}

                    var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                    var res10 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, string.Join(",", res5.Select(x => x.Mobile_1).FirstOrDefault())
                                        , string.Join("/", res5.Select(x => x.Father_Husband)), res1.Id, string.Join("/", res5.Select(x => x.Name)), PaymentMethod.Cash.ToString(), 0,
                                        res1.Project_Name, 0, null, res1.Area.ToString(), ReceiptTypes.Transfer.ToString(), userid, userid, res1.Type + " Transfer", null, Modules.CommercialManagement.ToString(), "", res1.shop_no + " " + res1.ApplicationNo, res1.Floor, res1.Area.ToString(), grpTag, TransactionId, "", receiptno, comp.Id).FirstOrDefault();

                    {
                        Helpers H = new Helpers();
                        bool headcashier = false;
                        if (User.IsInRole("Head Cashier"))
                        {
                            headcashier = true;
                        }

                        try
                        {
                            AccountHandlerController de = new AccountHandlerController();
                            //de.Transfer_Plot(rd.Amount, plot.Plot_No, plot.Type, plot.Block_Name, PaymentMethod.Cash.ToString(), rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, H.RandomNumber(), userid, res1.Receipt_No, 1, headcashier);
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        }
                    }


                    db.Sp_Add_CommercialComments(Com_Id, "Transfered to " + string.Join(",", res5.Select(x => x.Name)), userid, ActivityType.Plot_Transfered.ToString());
                    db.Sp_Add_Activity(userid, "Transfered to " + string.Join(",", res5.Select(x => x.Name)) + " " + " At " + DateTime.Now.ToString(), "Update", Modules.CommercialManagement.ToString(), ActivityType.Plot_Transfered.ToString(), Com_Id);
                    Transaction.Commit();
                    var text = "Dear " + rd.Name + ",\n\r" +
                           "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in cash for " + res1.shop_no + " number " + rd.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                    try
                    {

                        SmsService smsService = new SmsService();
                        foreach (var v in Mobs)
                        {
                            smsService.SendMsg(text, v);
                        }
                    }
                    catch (Exception ex)
                    {
                        EmailService e = new EmailService();
                        e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Plot Transfer Error");

                    }
                    var res = new { ReceiptId = res10.Receipt_No, Token = userid };
                    return Json(res);
                }
                catch (Exception ex)
                {
                    EmailService e = new EmailService();
                    e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Purchase Requisition Error");

                    Transaction.Rollback();
                    return Json(false);
                }
            }
        }
        public ActionResult CommercialTransfersList()
        {
            var res = db.Sp_Get_PlotsTransferList().ToList();
            return View(res);
        }
        public ActionResult CommercialTransferLetter(long Id)
        {
            var res3 = db.Commercial_Room_Transfer.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Commercial_Rooms.Where(x => x.Id == res3.ComRom_Id).FirstOrDefault();
            string[] res6 = { "Booking", "Advance", "Installment" };
            var res4 = db.Sp_Get_ReceivedAmounts(res3.ComRom_Id, Modules.CommercialManagement.ToString()).Where(x => x.DateTime <= res3.DateTime && (x.Status == null || x.Status == "Approved") && (res6.Contains(x.Type))).ToList().Sum(x => x.Amount);
            var disc = db.Discounts.Where(x => x.Module == Modules.CommercialManagement.ToString() && x.Module_Id == res3.ComRom_Id).ToList();

            var totalPlotValue = db.Commercial_Installments.Where(x => x.Com_Id == res3.ComRom_Id).Sum(x => x.Amount);
            var lastOwns = db.Sp_Get_CommercialLastOwner(res3.ComRom_Id).ToList();
            var transdate = lastOwns.Select(x => x.DateTime).FirstOrDefault();
            var seclastowns = db.Sp_Get_CommercialSecondLastOwner(res3.ComRom_Id).ToList();

            var res1 = new TransferLetter
            {
                //Id = x.Id,
                Owner_Id = lastOwns.Select(x => x.GroupTag).FirstOrDefault(),
                Block = res2.Floor,
                CNIC_NICOP = string.Join(" , ", lastOwns.Select(x => x.CNIC_NICOP)),
                Plot_No = res2.ApplicationNo.ToString(),
                Mobile_1 = string.Join(" , ", lastOwns.Select(x => x.Mobile_1)),
                Name = string.Join(" , ", lastOwns.Select(x => x.Name)),
                Total = totalPlotValue,
                Transfer_From = seclastowns.Select(x => x.Name).FirstOrDefault(),
                T_CNIC = seclastowns.Select(x => x.CNIC_NICOP).FirstOrDefault(),
                T_Mobile1 = seclastowns.Select(x => x.Mobile_1).FirstOrDefault(),
                Size = res2.Area.ToString(),
                Transfer_Date = transdate,
                Received = res4,
                IsCompanyProperty = lastOwns.Any(x => x.IsCompanyProperty == true),
                Discount = disc.Sum(x => x.Discount_Amount)
            };
            return View(res1);
        }
        public JsonResult UpdateCommercialTransferFilerStatus(long owner, bool filer)
        {
            var res = db.Plots_Transfer_Request.Where(x => x.Id == owner).FirstOrDefault();

            if (res is null)
            {
                return Json(false);
            }

            res.IsFiler = filer;
            db.Plots_Transfer_Request.Attach(res);
            db.Entry(res).Property(x => x.IsFiler).IsModified = true;
            db.SaveChanges();
            return Json(true);
        }
        //public ActionResult TaxCalculator()
        //{
        //    ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
        //    return View();
        //}

        public JsonResult UpdateCommercialTaxExemptionStatus(long grp, bool tp, bool enable, string rzn)
        {
            try
            {
                var trans = db.Plots_Transfer_Request.Where(x => x.GroupTag == grp).ToList();
                if (tp == true)
                {
                    //buyer
                    foreach (var v in trans)
                    {
                        v.TaxExempted = (enable == true) ? true : false;
                        v.TaxExemptReason = rzn;
                        db.SaveChanges();
                    }
                }
                else
                {
                    //seller
                    foreach (var v in trans)
                    {
                        v.Filer_NonFiler = (enable == true) ? "Exempted" : string.Empty;
                        v.SellExemptReason = rzn;
                        db.SaveChanges();
                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
    }
}