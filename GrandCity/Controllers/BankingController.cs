using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class BankingController : Controller
    {
        // GET: Banking
        private Grand_CityEntities db = new Grand_CityEntities();

        // Cash counter recovery report

        [NoDirectAccess] public ActionResult DailyCashReport()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult FullDailyCashReport()
        {
            var All = db.Sp_Get_Cashiers_List().ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Daily Cash Report Page", "Read", "Activity_Record", ActivityType.Banking.ToString(), userid);
            return PartialView();
        }
        [NoDirectAccess] public ActionResult SearchDailyCashReport(DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Searched Daily Cash Report from " + From + " To: " + To, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            if (From == null || To == null)
            {
                From = DateTime.Now;
                To = DateTime.Now;
            }
            var chids = new XElement("ChPoDd", new XElement("Details", new XAttribute("Ids", userid))).ToString();
            var res = db.Sp_Get_DailyCashierUser_Report(userid, From, To).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DateTime,
                Dealership_Name = x.Dealership_Name,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.ReceiptNo,
                Type = x.Type,
                File_Plot_Number = x.File_Plot_Number,
                Block = x.Block,
                Plot_Type = x.Plot_Type,
                Cancel = x.Cancel,
                Description = x.Description,
                VoucherType = x.VoucherType
            }).ToList();

            DailyCashRep Fresult = new DailyCashRep();
            string[] Types = { ReceiptTypes.Booking.ToString(), ReceiptTypes.Installment.ToString(), ReceiptTypes.BookingToken.ToString(), ReceiptTypes.Cancellation.ToString(), ReceiptTypes.DealershipRegisteration.ToString(), ReceiptTypes.Confirmation.ToString() };
            string[] Type2 = { ReceiptTypes.ServiceCharges.ToString(), ReceiptTypes.Electricity_Charges.ToString(), ReceiptTypes.New_Connection_Charges.ToString() };
            string[] Type3 = { ReceiptTypes.Transfer.ToString() };
            string[] Type4 = { ReceiptTypes.Membership_Monthly_Fee.ToString(), ReceiptTypes.Membership_Fee.ToString(), ReceiptTypes.Fines_And_Penalties.ToString(), ReceiptTypes.Duplicate_Allotment_Letter.ToString(), ReceiptTypes.Duplicate_Customer_File.ToString(), ReceiptTypes.Power_Of_Attorney.ToString(), ReceiptTypes.Other_Recovery.ToString(), ReceiptTypes.Out_Station_Charges.ToString(), ReceiptTypes.Other_Transfer_Charges.ToString(), ReceiptTypes.Dealership_Security.ToString(), ReceiptTypes.DealerAdvance.ToString(), ReceiptTypes.Subsidiary_Recovery.ToString(), ReceiptTypes.Posession_Charges.ToString(), ReceiptTypes.Receivable_Receipt.ToString(), ReceiptTypes.LoanSettlement.ToString() };
            string[] Type5 = { ReceiptTypes.Advance_Tax_236_C.ToString() };
            string[] Type6 = { ReceiptTypes.Advance_Tax_236_K.ToString() };
            string[] Type7 = { ReceiptTypes.Architecture_Fees.ToString() };

            Fresult.FileCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block == "Sher Afghan" && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.PlotCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && x.Module != Modules.CommercialManagement.ToString() && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.CommercialCollection = res.Where(x => x.VoucherType == "Receipts" && x.Module == Modules.CommercialManagement.ToString() && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.ServiceChargesCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type2.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.OtherCollection = res.Where(x => x.VoucherType == "Receipts" && Type4.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.FileTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block == "Sher Afghan" && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.PlotsTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && x.Module != Modules.CommercialManagement.ToString() && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.CommercialTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Module == Modules.CommercialManagement.ToString() && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.Advance_236_C_Collection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type5.Contains(x.Type) && (x.Cancel == null)).ToList();
            Fresult.Advance_236_K_Collection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type6.Contains(x.Type) && (x.Cancel == null)).ToList();
            Fresult.ArchiFees = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type7.Contains(x.Type) && (x.Cancel == null)).ToList();

            Fresult.Payments = res.Where(x => x.VoucherType == "Payments" && (x.Cancel == null)).ToList();

            {
                Fresult.File_Cash = Fresult.FileCollection.Where(x => x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Cheque = Fresult.FileCollection.Where(x => x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_PayOrder = Fresult.FileCollection.Where(x => x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_BankDraft = Fresult.FileCollection.Where(x => x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Online = Fresult.FileCollection.Where(x => x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Total = Fresult.FileCollection.Where(x => x.Cancel == null).Sum(x => x.Amount);
            }
            {
                Fresult.Plot_Cash = Fresult.PlotCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Plot_Cheque = Fresult.PlotCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Plot_PayOrder = Fresult.PlotCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Plot_BankDraft = Fresult.PlotCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Plot_Online = Fresult.PlotCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Plot_Total = Fresult.PlotCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Com_Cash = Fresult.CommercialCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Com_Cheque = Fresult.CommercialCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Com_PayOrder = Fresult.CommercialCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Com_BankDraft = Fresult.CommercialCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Com_Online = Fresult.CommercialCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Com_Total = Fresult.CommercialCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ser_Cash = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ser_Cheque = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ser_PayOrder = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ser_BankDraft = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ser_Online = Fresult.ServiceChargesCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ser_Total = Fresult.ServiceChargesCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ft_Cash = Fresult.FileTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ft_Cheque = Fresult.FileTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ft_PayOrder = Fresult.FileTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ft_BankDraft = Fresult.FileTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ft_Online = Fresult.FileTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ft_Total = Fresult.FileTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Other_Cash = Fresult.OtherCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Other_Cheque = Fresult.OtherCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Other_PayOrder = Fresult.OtherCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Other_BankDraft = Fresult.OtherCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Other_Online = Fresult.OtherCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Other_Total = Fresult.OtherCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ad_C_Cash = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ad_C_Cheque = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ad_C_PayOrder = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ad_C_BankDraft = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ad_C_Online = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ad_C_Total = Fresult.Advance_236_C_Collection.Sum(x => x.Amount);
            }
            {
                Fresult.Ad_K_Cash = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ad_K_Cheque = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ad_K_PayOrder = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ad_K_BankDraft = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ad_K_Online = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ad_K_Total = Fresult.Advance_236_K_Collection.Sum(x => x.Amount);
            }
            {
                Fresult.Pt_Cash = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Pt_Cheque = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Pt_PayOrder = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Pt_BankDraft = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Pt_Online = Fresult.PlotsTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Pt_Total = Fresult.PlotsTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ct_Cash = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ct_Cheque = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ct_PayOrder = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ct_BankDraft = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ct_Online = Fresult.CommercialTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ct_Total = Fresult.CommercialTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Pay_Cash = Fresult.Payments.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Pay_Cheque = Fresult.Payments.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Pay_PayOrder = Fresult.Payments.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Pay_BankDraft = Fresult.Payments.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Pay_Online = Fresult.Payments.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Pay_Total = Fresult.Payments.Sum(x => x.Amount);
            }

            Fresult.Cash = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Cheque = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.PayOrder = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.BankDraft = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Online = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Total = res.Where(x => x.VoucherType == "Receipts").Sum(x => x.Amount);

            Fresult.Grand_Cash = res.Where(x => x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Cheque = res.Where(x => x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_PayOrder = res.Where(x => x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_BankDraft = res.Where(x => x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Online = res.Where(x => x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Total = res.Where(x => x.Cancel == null).Sum(x => x.Amount);

            Fresult.CancelledReceipts = res.Where(x => x.Cancel == true && x.ReportType is null).ToList();



            ViewBag.Name = res.Select(x => x.Cashier_Name).FirstOrDefault();
            ViewBag.From = From;
            ViewBag.To = To;

            return PartialView(Fresult);
        }
        [NoDirectAccess] public ActionResult PendingDayCloseCashReport()
        {
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Searched Pending Day Close Cash Report", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            var From = DateTime.Now;
            var To = DateTime.Now;
            var chids = new XElement("ChPoDd", new XElement("Details", new XAttribute("Ids", userid))).ToString();
            var res = db.Sp_Get_DayCloseCashierUser_Report(userid, From, To, "Pending").Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DATETIME,
                Dealership_Name = x.Dealership_Name,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.ReceiptNo,
                Type = x.Type,
                File_Plot_Number = x.File_Plot_Number,
                Block = x.Block,
                Plot_Type = x.Plot_Type,
                Cancel = x.Cancel,
                Description = x.Description,
                VoucherType = x.VoucherType
            }).ToList();

            DailyCashRep Fresult = new DailyCashRep();
            string[] Types = { ReceiptTypes.Booking.ToString(), ReceiptTypes.Installment.ToString(), ReceiptTypes.BookingToken.ToString(), ReceiptTypes.Cancellation.ToString(), ReceiptTypes.DealershipRegisteration.ToString(), ReceiptTypes.Confirmation.ToString() };
            string[] Type2 = { ReceiptTypes.ServiceCharges.ToString(), ReceiptTypes.Electricity_Charges.ToString(), ReceiptTypes.New_Connection_Charges.ToString() };
            string[] Type3 = { ReceiptTypes.Transfer.ToString() };
            string[] Type4 = { ReceiptTypes.Membership_Monthly_Fee.ToString(), ReceiptTypes.Membership_Fee.ToString(), ReceiptTypes.Fines_And_Penalties.ToString(), ReceiptTypes.Duplicate_Allotment_Letter.ToString(), ReceiptTypes.Duplicate_Customer_File.ToString(), ReceiptTypes.Power_Of_Attorney.ToString(), ReceiptTypes.Other_Recovery.ToString(), ReceiptTypes.Out_Station_Charges.ToString(), ReceiptTypes.Other_Transfer_Charges.ToString(), ReceiptTypes.Dealership_Security.ToString(), ReceiptTypes.DealerAdvance.ToString(), ReceiptTypes.Subsidiary_Recovery.ToString(), ReceiptTypes.Posession_Charges.ToString(), ReceiptTypes.Receivable_Receipt.ToString(), ReceiptTypes.LoanSettlement.ToString() };
            string[] Type5 = { ReceiptTypes.Advance_Tax_236_C.ToString() };
            string[] Type6 = { ReceiptTypes.Advance_Tax_236_K.ToString() };
            string[] Type7 = { ReceiptTypes.Architecture_Fees.ToString() };

            Fresult.FileCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block == "Sher Afghan" && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.PlotCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && x.Module != Modules.CommercialManagement.ToString() && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.CommercialCollection = res.Where(x => x.VoucherType == "Receipts" && x.Module == Modules.CommercialManagement.ToString() && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.ServiceChargesCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type2.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.OtherCollection = res.Where(x => x.VoucherType == "Receipts" && Type4.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.FileTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block == "Sher Afghan" && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.PlotsTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && x.Module != Modules.CommercialManagement.ToString() && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.CommercialTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Module == Modules.CommercialManagement.ToString() && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.Advance_236_C_Collection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type5.Contains(x.Type) && (x.Cancel == null)).ToList();
            Fresult.Advance_236_K_Collection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type6.Contains(x.Type) && (x.Cancel == null)).ToList();
            Fresult.ArchiFees = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type7.Contains(x.Type) && (x.Cancel == null)).ToList();

            Fresult.Payments = res.Where(x => x.VoucherType == "Payments" && (x.Cancel == null)).ToList();

            {
                Fresult.File_Cash = Fresult.FileCollection.Where(x => x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Cheque = Fresult.FileCollection.Where(x => x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_PayOrder = Fresult.FileCollection.Where(x => x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_BankDraft = Fresult.FileCollection.Where(x => x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Online = Fresult.FileCollection.Where(x => x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Total = Fresult.FileCollection.Where(x => x.Cancel == null).Sum(x => x.Amount);
            }
            {
                Fresult.Plot_Cash = Fresult.PlotCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Plot_Cheque = Fresult.PlotCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Plot_PayOrder = Fresult.PlotCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Plot_BankDraft = Fresult.PlotCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Plot_Online = Fresult.PlotCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Plot_Total = Fresult.PlotCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Com_Cash = Fresult.CommercialCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Com_Cheque = Fresult.CommercialCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Com_PayOrder = Fresult.CommercialCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Com_BankDraft = Fresult.CommercialCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Com_Online = Fresult.CommercialCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Com_Total = Fresult.CommercialCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ser_Cash = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ser_Cheque = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ser_PayOrder = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ser_BankDraft = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ser_Online = Fresult.ServiceChargesCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ser_Total = Fresult.ServiceChargesCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ft_Cash = Fresult.FileTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ft_Cheque = Fresult.FileTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ft_PayOrder = Fresult.FileTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ft_BankDraft = Fresult.FileTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ft_Online = Fresult.FileTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ft_Total = Fresult.FileTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Other_Cash = Fresult.OtherCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Other_Cheque = Fresult.OtherCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Other_PayOrder = Fresult.OtherCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Other_BankDraft = Fresult.OtherCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Other_Online = Fresult.OtherCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Other_Total = Fresult.OtherCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ad_C_Cash = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ad_C_Cheque = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ad_C_PayOrder = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ad_C_BankDraft = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ad_C_Online = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ad_C_Total = Fresult.Advance_236_C_Collection.Sum(x => x.Amount);
            }
            {
                Fresult.Ad_K_Cash = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ad_K_Cheque = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ad_K_PayOrder = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ad_K_BankDraft = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ad_K_Online = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ad_K_Total = Fresult.Advance_236_K_Collection.Sum(x => x.Amount);
            }
            {
                Fresult.Pt_Cash = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Pt_Cheque = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Pt_PayOrder = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Pt_BankDraft = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Pt_Online = Fresult.PlotsTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Pt_Total = Fresult.PlotsTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ct_Cash = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ct_Cheque = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ct_PayOrder = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ct_BankDraft = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ct_Online = Fresult.CommercialTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ct_Total = Fresult.CommercialTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Pay_Cash = Fresult.Payments.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Pay_Cheque = Fresult.Payments.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Pay_PayOrder = Fresult.Payments.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Pay_BankDraft = Fresult.Payments.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Pay_Online = Fresult.Payments.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Pay_Total = Fresult.Payments.Sum(x => x.Amount);
            }

            Fresult.Cash = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Cheque = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.PayOrder = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.BankDraft = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Online = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Total = res.Where(x => x.VoucherType == "Receipts").Sum(x => x.Amount);

            Fresult.Grand_Cash = res.Where(x => x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Cheque = res.Where(x => x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_PayOrder = res.Where(x => x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_BankDraft = res.Where(x => x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Online = res.Where(x => x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Total = res.Where(x => x.Cancel == null).Sum(x => x.Amount);

            Fresult.CancelledReceipts = res.Where(x => x.Cancel == true && x.ReportType is null).ToList();

            ViewBag.Name = res.Select(x => x.Cashier_Name).FirstOrDefault();

            return PartialView(Fresult);
        }
        [NoDirectAccess] public ActionResult DayClosedCashReport()
        {
            var All = db.Sp_Get_Cashiers_List().ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Day Closed Cash Report Page", "Read", "Activity_Record", ActivityType.Banking.ToString(), userid);
            return PartialView();
        }
        [NoDirectAccess] public ActionResult SearchDayClosedCashReport(DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Searched Daily Cash Report from " + From + " To: " + To, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            if (From == null || To == null)
            {
                From = DateTime.Now;
                To = DateTime.Now;
            }
            var chids = new XElement("ChPoDd", new XElement("Details", new XAttribute("Ids", userid))).ToString();
            var res = db.Sp_Get_DayCloseCashierUser_Report(userid, From, To, "Closed").Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DATETIME,
                Dealership_Name = x.Dealership_Name,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.ReceiptNo,
                Type = x.Type,
                File_Plot_Number = x.File_Plot_Number,
                Block = x.Block,
                Plot_Type = x.Plot_Type,
                Cancel = x.Cancel,
                Description = x.Description,
                VoucherType = x.VoucherType
            }).ToList();

            DailyCashRep Fresult = new DailyCashRep();
            string[] Types = { ReceiptTypes.Booking.ToString(), ReceiptTypes.Installment.ToString(), ReceiptTypes.BookingToken.ToString(), ReceiptTypes.Cancellation.ToString(), ReceiptTypes.DealershipRegisteration.ToString(), ReceiptTypes.Confirmation.ToString() };
            string[] Type2 = { ReceiptTypes.ServiceCharges.ToString(), ReceiptTypes.Electricity_Charges.ToString(), ReceiptTypes.New_Connection_Charges.ToString() };
            string[] Type3 = { ReceiptTypes.Transfer.ToString() };
            string[] Type4 = { ReceiptTypes.Membership_Monthly_Fee.ToString(), ReceiptTypes.Membership_Fee.ToString(), ReceiptTypes.Fines_And_Penalties.ToString(), ReceiptTypes.Duplicate_Allotment_Letter.ToString(), ReceiptTypes.Duplicate_Customer_File.ToString(), ReceiptTypes.Power_Of_Attorney.ToString(), ReceiptTypes.Other_Recovery.ToString(), ReceiptTypes.Out_Station_Charges.ToString(), ReceiptTypes.Other_Transfer_Charges.ToString(), ReceiptTypes.Dealership_Security.ToString(), ReceiptTypes.DealerAdvance.ToString(), ReceiptTypes.Subsidiary_Recovery.ToString(), ReceiptTypes.Posession_Charges.ToString(), ReceiptTypes.Receivable_Receipt.ToString(), ReceiptTypes.LoanSettlement.ToString() };
            string[] Type5 = { ReceiptTypes.Advance_Tax_236_C.ToString() };
            string[] Type6 = { ReceiptTypes.Advance_Tax_236_K.ToString() };
            string[] Type7 = { ReceiptTypes.Architecture_Fees.ToString() };

            Fresult.FileCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block == "Sher Afghan" && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.PlotCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && x.Module != Modules.CommercialManagement.ToString() && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.CommercialCollection = res.Where(x => x.VoucherType == "Receipts" && x.Module == Modules.CommercialManagement.ToString() && Types.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.ServiceChargesCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type2.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.OtherCollection = res.Where(x => x.VoucherType == "Receipts" && Type4.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.FileTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block == "Sher Afghan" && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.PlotsTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && x.Module != Modules.CommercialManagement.ToString() && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.CommercialTransferCollection = res.Where(x => x.VoucherType == "Receipts" && x.Module == Modules.CommercialManagement.ToString() && Type3.Contains(x.Type) && (x.Cancel == null || x.ReportType == "Reversal")).ToList();
            Fresult.Advance_236_C_Collection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type5.Contains(x.Type) && (x.Cancel == null)).ToList();
            Fresult.Advance_236_K_Collection = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type6.Contains(x.Type) && (x.Cancel == null)).ToList();
            Fresult.ArchiFees = res.Where(x => x.VoucherType == "Receipts" && x.Block != "Sher Afghan" && Type7.Contains(x.Type) && (x.Cancel == null)).ToList();

            Fresult.Payments = res.Where(x => x.VoucherType == "Payments" && (x.Cancel == null)).ToList();

            {
                Fresult.File_Cash = Fresult.FileCollection.Where(x => x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Cheque = Fresult.FileCollection.Where(x => x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_PayOrder = Fresult.FileCollection.Where(x => x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_BankDraft = Fresult.FileCollection.Where(x => x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Online = Fresult.FileCollection.Where(x => x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
                Fresult.File_Total = Fresult.FileCollection.Where(x => x.Cancel == null).Sum(x => x.Amount);
            }
            {
                Fresult.Plot_Cash = Fresult.PlotCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Plot_Cheque = Fresult.PlotCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Plot_PayOrder = Fresult.PlotCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Plot_BankDraft = Fresult.PlotCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Plot_Online = Fresult.PlotCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Plot_Total = Fresult.PlotCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Com_Cash = Fresult.CommercialCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Com_Cheque = Fresult.CommercialCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Com_PayOrder = Fresult.CommercialCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Com_BankDraft = Fresult.CommercialCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Com_Online = Fresult.CommercialCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Com_Total = Fresult.CommercialCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ser_Cash = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ser_Cheque = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ser_PayOrder = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ser_BankDraft = Fresult.ServiceChargesCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ser_Online = Fresult.ServiceChargesCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ser_Total = Fresult.ServiceChargesCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ft_Cash = Fresult.FileTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ft_Cheque = Fresult.FileTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ft_PayOrder = Fresult.FileTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ft_BankDraft = Fresult.FileTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ft_Online = Fresult.FileTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ft_Total = Fresult.FileTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Other_Cash = Fresult.OtherCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Other_Cheque = Fresult.OtherCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Other_PayOrder = Fresult.OtherCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Other_BankDraft = Fresult.OtherCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Other_Online = Fresult.OtherCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Other_Total = Fresult.OtherCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ad_C_Cash = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ad_C_Cheque = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ad_C_PayOrder = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ad_C_BankDraft = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ad_C_Online = Fresult.Advance_236_C_Collection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ad_C_Total = Fresult.Advance_236_C_Collection.Sum(x => x.Amount);
            }
            {
                Fresult.Ad_K_Cash = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ad_K_Cheque = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ad_K_PayOrder = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ad_K_BankDraft = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ad_K_Online = Fresult.Advance_236_K_Collection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ad_K_Total = Fresult.Advance_236_K_Collection.Sum(x => x.Amount);
            }
            {
                Fresult.Pt_Cash = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Pt_Cheque = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Pt_PayOrder = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Pt_BankDraft = Fresult.PlotsTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Pt_Online = Fresult.PlotsTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Pt_Total = Fresult.PlotsTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Ct_Cash = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Ct_Cheque = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Ct_PayOrder = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Ct_BankDraft = Fresult.CommercialTransferCollection.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Ct_Online = Fresult.CommercialTransferCollection.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Ct_Total = Fresult.CommercialTransferCollection.Sum(x => x.Amount);
            }
            {
                Fresult.Pay_Cash = Fresult.Payments.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount);
                Fresult.Pay_Cheque = Fresult.Payments.Where(x => x.PaymentType == "Cheque").Sum(x => x.Amount);
                Fresult.Pay_PayOrder = Fresult.Payments.Where(x => x.PaymentType == "PayOrder").Sum(x => x.Amount);
                Fresult.Pay_BankDraft = Fresult.Payments.Where(x => x.PaymentType == "BankDraft").Sum(x => x.Amount);
                Fresult.Pay_Online = Fresult.Payments.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.Pay_Total = Fresult.Payments.Sum(x => x.Amount);
            }

            Fresult.Cash = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Cheque = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.PayOrder = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.BankDraft = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Online = res.Where(x => x.VoucherType == "Receipts" && x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Total = res.Where(x => x.VoucherType == "Receipts").Sum(x => x.Amount);

            Fresult.Grand_Cash = res.Where(x => x.PaymentType == "Cash" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Cheque = res.Where(x => x.PaymentType == "Cheque" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_PayOrder = res.Where(x => x.PaymentType == "PayOrder" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_BankDraft = res.Where(x => x.PaymentType == "BankDraft" && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Online = res.Where(x => x.PaymentType.Contains("Online") && x.Cancel == null).Sum(x => x.Amount);
            Fresult.Grand_Total = res.Where(x => x.Cancel == null).Sum(x => x.Amount);

            Fresult.CancelledReceipts = res.Where(x => x.Cancel == true && x.ReportType is null).ToList();


            ViewBag.Name = res.Select(x => x.Cashier_Name).FirstOrDefault();
            ViewBag.From = From;
            ViewBag.To = To;

            return PartialView(Fresult);
        }

        [NoDirectAccess] public ActionResult BankingInstrument()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Banks Payments Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return View();
        }

        [NoDirectAccess] public ActionResult AllBankPayments()
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Banks Payments Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView();
        }
        [NoDirectAccess] public ActionResult Cheques(string Status, DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            ViewBag.Status = Status;
            if (From == null || To == null)
            {
                var fy = this.GetFinancialYear();
                From = fy.Start;
                To = fy.End;

                ViewBag.From = From;
                ViewBag.To = To;
            }
            else
            {
                ViewBag.From = From;
                ViewBag.To = To;
            }
            var res = db.Sp_Get_ChDDPO("Cheque", Status, From, To, Voucher_Type.BRV.ToString(), comp.Id).ToList();

            db.Sp_Add_Activity(userid, "Accessed Cheques Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult BankDrafts(string Status, DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            ViewBag.Status = Status;
            if (From == null || To == null)
            {
                var fy = this.GetFinancialYear();
                From = fy.Start;
                To = fy.End;

                ViewBag.From = From;
                ViewBag.To = To;
            }
            else
            {
                ViewBag.From = From;
                ViewBag.To = To;
            }
            var res = db.Sp_Get_ChDDPO("BankDraft", Status, From, To, Voucher_Type.BRV.ToString(), comp.Id).ToList();


            db.Sp_Add_Activity(userid, "Accessed Bank Drafts Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult Payorder(string Status, DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);

            ViewBag.Status = Status;
            if (From == null || To == null)
            {
                var fy = this.GetFinancialYear();
                From = fy.Start;
                To = fy.End;

                ViewBag.From = From;
                ViewBag.To = To;
            }
            else
            {
                ViewBag.From = From;
                ViewBag.To = To;
            }
            var res = db.Sp_Get_ChDDPO("PayOrder", Status, From, To, Voucher_Type.BRV.ToString(), comp.Id).ToList();

            db.Sp_Add_Activity(userid, "Accessed Payorders Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult Online(string Status, DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            ViewBag.Status = Status;
            if (From == null || To == null)
            {
                var fy = this.GetFinancialYear();
                From = fy.Start;
                To = fy.End;

                ViewBag.From = From;
                ViewBag.To = To;
            }
            else
            {
                ViewBag.From = From;
                ViewBag.To = To;
            }
            var res = db.Sp_Get_Online(Status, From, To, comp.Id).ToList();

            db.Sp_Add_Activity(userid, "Accessed Oline Payments Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult DebitCredit(string Status, DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            ViewBag.Status = Status;
            if (From == null || To == null)
            {
                var fy = this.GetFinancialYear();
                From = fy.Start;
                To = fy.End;

                ViewBag.From = From;
                ViewBag.To = To;
            }
            else
            {
                ViewBag.From = From;
                ViewBag.To = To;
            }
            var res = db.Sp_Get_ChDDPO("Debit / Credit Card", Status, From, To, Voucher_Type.BRV.ToString(), comp.Id).ToList();

            db.Sp_Add_Activity(userid, "Accessed Debit / Credit Card Payments Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult SearchInstruments(string Type, string Status, DateTime? From, DateTime? To)
        {
            if (Type == "Cheque")
            {
                return RedirectToAction("Cheques", new { Status, From, To });
            }
            else if (Type == "Payorder")
            {
                return RedirectToAction("Payorder", new { Status, From, To });
            }
            else if (Type == "BankDraft")
            {
                return RedirectToAction("BankDrafts", new { Status, From, To });
            }
            else if (Type == "Online")
            {
                return RedirectToAction("Online", new { Status, From, To });
            }
            else if (Type == "DebitCredit")
            {
                return RedirectToAction("DebitCredit", new { Status, From, To });
            }
            return PartialView();

        }
        [NoDirectAccess] public ActionResult Cash()
        {
            var res = db.Sp_Get_DailyCash_Report().ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Daily Cash Report Page", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(res);
        }
        [NoDirectAccess] public ActionResult CBPDetails(string No, DateTime Date, string Bank, string Status)
        {
            var res = db.Sp_Get_Cheque_BankDraft_PayOrder_MultiParamter(No, Status, Bank, Date, Voucher_Type.BRV.ToString()).ToList();
            ViewBag.Bank = db.Bank_Accounts.Select(x => new { x.Bank, x.Account_Number }).ToList();
            ViewBag.Reason = new SelectList(Nomenclature.DishoneredReason(), "Value", "Name");
            var paymentstatus = from PaymentMethodStatuses e in Enum.GetValues(typeof(PaymentMethodStatuses)) select new { Name = e.ToString() };
            ViewBag.Status = new SelectList(paymentstatus, "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed CBP Detail Page", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateCbp(DateTime? Posted_Date, string Status, string Bank, string Deposit_Bank_Acc_Number, DateTime? Clearance_Date, string Reason, long[] Id, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var chids = new XElement("ChPoDd", Id.Select(x => new XElement("Details",
                                   new XAttribute("Ids", x)
                                  ))).ToString();
            var Returnids = db.Sp_Get_ChDDPODetails_Multi(chids).ToList();
            string name = "", mobile = "", type = "", number = "", fileno = "";
            foreach (var cbp in Returnids)
            {
                { // For Accounts Module 
                    AccountHandlerController ah = new AccountHandlerController();
                    var receipt = db.Receipts.Where(x => x.Id == cbp.ReceiptId).FirstOrDefault(); //db.Sp_Get_Receipt_Parameter_ById(cbp.ReceiptId).FirstOrDefault();
                    if (receipt.Transaction_Id == null)
                    {
                        bool? flag = false;
                        long tran = 0;
                        while (flag == false)
                        {
                            tran = new Helpers().RandomNumber();
                            flag = db.Sp_Update_Receipt_Transaction_Id(receipt.Id, tran).FirstOrDefault();
                        }
                        receipt.Transaction_Id = tran.ToString();
                    }
                    if (Status == PaymentMethodStatuses.Deposited.ToString())
                    {
                        ah.BankGLEntry(cbp.ReceiptId, Deposit_Bank_Acc_Number, userid);
                    }
                    if (Status == PaymentMethodStatuses.Approved.ToString())
                    {
                        ah.ApprovedInstrument(long.Parse(receipt.Transaction_Id), Status, userid, receipt.TokenParameter);
                    }
                    if (Status == PaymentMethodStatuses.Dishonored.ToString())
                    {
                        ah.DishonerInstrument(long.Parse(receipt.Transaction_Id), Status, userid, receipt.TokenParameter);
                    }
                }

                fileno = fileno + cbp.File_No + " ";
                var res = db.Sp_Update_Cheque_BankDraft_PayOrder(cbp.Id, Status, Bank, Deposit_Bank_Acc_Number, cbp.Module, cbp.Type, cbp.Reference_Id, Clearance_Date, cbp.Module_Id, Posted_Date, Reason).FirstOrDefault();
                name = res.Name; mobile = res.Mobile_1; type = cbp.Check_DemandDraft_PayOrder; number = cbp.Cheque_BankDraft_Payorder_Number;
                if (cbp.Module == Modules.FileManagement.ToString() && cbp.Type == Types.Booking.ToString())
                {
                    var rec = (from x in db.Receipts
                               join z in db.File_Form on x.File_Plot_No equals z.Id
                               where x.Id == cbp.ReceiptId
                               select new { z.Id }).FirstOrDefault();
                    db.Sp_Update_FileStatus(rec.Id, FileStatus.Registered.ToString());
                }
                else if (cbp.Module == Modules.PlotManagement.ToString() && cbp.Type == Types.Booking.ToString())
                {
                    var rec = (from x in db.Receipts
                               join z in db.Plots on x.File_Plot_No equals z.Id
                               where x.Id == cbp.ReceiptId
                               select new { z.Id }).FirstOrDefault();
                    db.Sp_Update_PlotStatus(rec.Id, PlotsStatus.Registered.ToString());
                }
            }

            if (Status == PaymentMethodStatuses.Approved.ToString())
            {
                {
                    // Full Paid Check and Notification
                }
                SmsService sms = new SmsService();
                string msg = "Dear " + name + "\n\r" +
                    "Your " + type + " No. " + number + " has been Cleared against File Number: " + fileno + ". Thank you for your payment.";
                try
                {
                    sms.SendMsg(msg, mobile);
                }
                catch (Exception)
                {
                }

            }
            else if (Status == PaymentMethodStatuses.Dishonored.ToString())
            {
                {

                }
                SmsService sms = new SmsService();
                string msg = "Dear " + name + "\n\r" +
                    "Your " + type + " No. " + number + " has been Bounced Due to " + Reason + ". A penalty of Rs 1,000 charged to your File number#" + fileno +
                    ". Please collect your " + type + " from Accounts Department and clear your dues with in 3 Days.";
                try
                {
                    sms.SendMsg(msg, mobile);
                }
                catch (Exception)
                {
                }
            }
            return Json(new Return { Status = true, Msg = "Updated" });
        }
        [NoDirectAccess] public ActionResult Receipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        [NoDirectAccess] public ActionResult InstallmentReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        [NoDirectAccess] public ActionResult Dup_Receipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        [NoDirectAccess] public ActionResult Dup_InstallmentReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        [NoDirectAccess] public ActionResult FileBiddingReceipt(string Id)
        {
            var res = (from x in db.Receipts
                       join y in db.File_Form on x.File_Plot_No equals y.Id
                       where x.Module == Modules.FileManagement.ToString() && x.DevelopmentCharges == Id
                       select x).ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult TransferReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }

        [NoDirectAccess] public ActionResult DailySAMCashReport()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult DailySAMCashReportView()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var chids = new XElement("ChPoDd", new XElement("Details", new XAttribute("Ids", userid))).ToString();

            db.Sp_Add_Activity(userid, "Accessed Daily SAM Cash Report Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            var res1 = db.Sp_Get_DailySAMCashierUser_Report(userid).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Block = x.Block,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DateTime,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.ReceiptNo,
                Dealership_Name = x.Company,
                Type = x.Type,
                Cancel = x.Cancel
            }).ToList();
            var res2 = db.Sp_Get_DailySAMCashierVoucherUser_Report(userid).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount * -1,
                Bank = x.Bank,
                Block = x.Block,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DateTime,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.VoucherNo,
                Dealership_Name = x.Company,
                Type = x.Type,
                Cancel = x.Cancel
            }).ToList();

            DailySAMCashRep Fresult = new DailySAMCashRep();
            string[] Types = { "Booking", "Installment", "BookingToken", "Transfer", "Cancellation" };
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
                Fresult.V_File_Online = res2.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.V_Total = res2.Sum(x => x.Amount);
            }
            ViewBag.Name = res1.Select(x => x.Cashier_Name).FirstOrDefault();
            return PartialView(Fresult);
        }
        [NoDirectAccess] public ActionResult PendingDailySamCashReport()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var chids = new XElement("ChPoDd", new XElement("Details", new XAttribute("Ids", userid))).ToString();

            db.Sp_Add_Activity(userid, "Accessed Daily SAM Cash Report Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            var res1 = db.Sp_Get_Pending_SAMCashier_Report(userid).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Block = x.Block,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DATETIME,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.ReceiptNo,
                Dealership_Name = x.Company,
                Type = x.Type,
                Cancel = x.Cancel
            }).ToList();
            var res2 = db.Sp_Get_PendingSAMCashierVoucherUser_Report(userid).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount * -1,
                Bank = x.Bank,
                Block = x.Block,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DATETIME,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.VoucherNo,
                Dealership_Name = x.Company,
                Type = x.Type,
                Cancel = x.Cancel
            }).ToList();

            DailySAMCashRep Fresult = new DailySAMCashRep();
            string[] Types = { "Booking", "Installment", "BookingToken", "Transfer", "Cancellation" };
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
                Fresult.V_File_Online = res2.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.V_Total = res2.Sum(x => x.Amount);
            }
            ViewBag.Name = res1.Select(x => x.Cashier_Name).FirstOrDefault();
            return PartialView(Fresult);
        }
        [NoDirectAccess] public ActionResult DayClosedSAMCashReceipt()
        {
            return PartialView();
        }
        [NoDirectAccess] public ActionResult DayClosedSamCashReceiptView(DateTime? From, DateTime? To)
        {
            if (From is null || To is null)
            {
                From = DateTime.Now;
                To = DateTime.Now;
            }
            ViewBag.From = From;
            ViewBag.To = To;
            long userid = long.Parse(User.Identity.GetUserId());
            var chids = new XElement("ChPoDd", new XElement("Details", new XAttribute("Ids", userid))).ToString();

            db.Sp_Add_Activity(userid, "Accessed Daily SAM Cash Report Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            var res1 = db.Sp_Get_DayClosed_SAMCashier_Report(userid, From, To).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount,
                Bank = x.Bank,
                Block = x.Block,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DATETIME,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.ReceiptNo,
                Dealership_Name = x.Company,
                Type = x.Type,
                Cancel = x.Cancel
            }).ToList();
            var res2 = db.Sp_Get_ClosedSAMCashierVoucherUser_Report(userid, From, To).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount * -1,
                Bank = x.Bank,
                Block = x.Block,
                Cashier_Name = x.Cashier_Name,
                Ch_Pay_Draft_Date = x.Ch_Pay_Draft_Date,
                Ch_Pay_Draft_No = x.Ch_Pay_Draft_No,
                Contact = x.Contact,
                DateTime = x.DATETIME,
                File_Plot_Number = x.File_Plot_Number,
                Module = x.Module,
                Name = x.Name,
                PaymentType = x.PaymentType,
                Project = x.Project,
                ReceiptNo = x.VoucherNo,
                Dealership_Name = x.Company,
                Type = x.Type,
                Cancel = x.Cancel
            }).ToList();

            DailySAMCashRep Fresult = new DailySAMCashRep();
            string[] Types = { "Booking", "Installment", "BookingToken", "Transfer", "Cancellation" };
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
                Fresult.V_File_Online = res2.Where(x => x.PaymentType.Contains("Online")).Sum(x => x.Amount);
                Fresult.V_Total = res2.Sum(x => x.Amount);
            }
            ViewBag.Name = res1.Select(x => x.Cashier_Name).FirstOrDefault();
            return PartialView(Fresult);
        }
        [NoDirectAccess] public ActionResult ReceiptData(long Id)
        {
            var res = db.Sp_Get_Receipt_Result(Id).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name", res.Bank);
            var rectyp = from ReceiptTypes e in Enum.GetValues(typeof(ReceiptTypes)) select new { Name = e.ToString() };
            ViewBag.Type = new SelectList(rectyp, "Name", "Name", res.Type);

            var paymeth = (from PaymentMethod e in Enum.GetValues(typeof(PaymentMethod)) select new { Name = e.ToString() }).ToList();
            paymeth.Add(new { Name = "Debit/Credit Card" });

            ViewBag.PaymentType = new SelectList(paymeth, "Name", "Name", res.PaymentType);

            return PartialView(res);
        }
        public JsonResult ReceiptDelete(long ReceiptId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Receipt_Result(ReceiptId).FirstOrDefault();
            if (res != null)
            {
                var res2 = db.File_Form.Where(x => x.Id == res.File_Plot_No).SingleOrDefault();
                try
                {
                    db.Sp_Add_FileComments(res2.Id, "Receipt no " + res.ReceiptNo + " of Amount " + res.Amount + " Dated: " + string.Format("{0:dd-MMM-yyyy}", res.DateTime) + " is deleted", userid, ActivityType.Delete_Receipt.ToString());
                    //try
                    //{
                    //    AccountHandlerController de = new AccountHandlerController();
                    //    var result = de.Banking_Receipts_Deletion(ReceiptId, userid);
                    //}
                    //catch (Exception ex)
                    //{
                    //    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    //}
                    db.Sp_Delete_Receipt(ReceiptId);

                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
                //using (var transaction = db.Database.BeginTransaction())
                //{
                //    try
                //    {
                //        db.Sp_Add_FileComments(res2.Id, "Receipt no " + res.ReceiptNo + " of Amount " + res.Amount + " Dated: " + string.Format("{0:dd-MMM-yyyy}", res.DateTime) + " is deleted", userid, ActivityType.Delete_Receipt.ToString());
                //        db.Sp_Delete_Receipt(ReceiptId);
                //        //try
                //        //{
                //        //    AccountHandlerController de = new AccountHandlerController();
                //        //    var result = de.Banking_Receipts_Deletion(ReceiptId, userid);
                //        //}
                //        //catch (Exception ex)
                //        //{
                //        //    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                //        //    Transaction.Rollback();
                //        //}
                //        transaction.Commit();
                //    }
                //    catch (Exception ex)

                //    {
                //        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                //        transaction.Rollback();
                //    }
                //}

                if (res2.Verified == true)
                {
                    db.Sp_Update_FileVerificationToNull(res.Id);
                }

                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateReceipt(Receipt r)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var rece = db.Sp_Get_Receipt_Parameter_ById(r.Id).FirstOrDefault();

            if (rece.Module == Modules.FileManagement.ToString())
            {
                var fileinfo = db.Sp_Get_FileData(rece.File_Plot_No).FirstOrDefault();

                string recedetails = JsonConvert.SerializeObject(rece);
                db.Sp_Add_Activity(userid, "Update receipt of File Id <a class='plt-data' data-id=' " + rece.File_Plot_No + "'>" + rece.File_Plot_No + "</a>---" + recedetails, "Update", Modules.FileManagement.ToString(), ActivityType.Update_Receipt.ToString(), rece.File_Plot_No);
                db.Sp_Add_FileComments(fileinfo.Id, "Updated Receipt : " + rece.ReceiptNo + " Amount: " + string.Format("{0:n0}", rece.Amount) + " of " + rece.PaymentType, userid, ActivityType.Update_Receipt.ToString());
                var previous = db.Receipts.Where(x => x.Id == r.Id).FirstOrDefault();
                var res = db.Sp_Update_Receipt(r.Id, r.Amount, r.Name, r.Father_Name, r.Contact, r.AmountinWords, r.PaymentType, r.Bank, r.Ch_Pay_Draft_No, "SA Systems"
                    , rece.File_Plot_No, r.Size, r.Type, r.TokenParameter, r.Userid, r.DateTime, rece.Module, r.ReceiptNo, r.Ch_Pay_Draft_Date);
                var newr = db.Receipts.Where(x => x.Id == r.Id).FirstOrDefault();
                if (previous.Amount != newr.Amount || previous.PaymentType != newr.PaymentType)
                {
                    //try
                    //{
                    //    AccountHandlerController de = new AccountHandlerController();
                    //    de.Banking_Receipts_Updation_Wiith_Deletion(r.Id, r.Amount, r.Name, r.Contact, r.PaymentType, r.Ch_Pay_Draft_No, r.Ch_Pay_Draft_Date, r.Bank, rece.File_Plot_Number, r.Type, r.ReceiptNo, r.Block);
                    //}
                    //catch (Exception ex)
                    //{
                    //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    //}
                }

                var re = db.Sp_Update_FileVerificationToNull(fileinfo.Id);
                var data1 = new { Status = true, Msg = "Receipt updated successfully" };
                return Json(data1);
            }
            else if (rece.Module == Modules.PlotManagement.ToString())
            {
                string recedetails = JsonConvert.SerializeObject(rece);
                db.Sp_Add_Activity(userid, "Update receipt of Plot Id  <a class='plt-data' data-id=' " + rece.File_Plot_No + "'>" + rece.File_Plot_No + "</a>---" + recedetails, "Update", Modules.PlotManagement.ToString(), ActivityType.Delete_Receipt.ToString(), rece.File_Plot_No);
                db.Sp_Add_PlotComments(rece.File_Plot_No, "Updated Receipt : " + rece.ReceiptNo + " Amount: " + string.Format("{0:n0}", rece.Amount) + " of " + rece.PaymentType, userid, ActivityType.Update_Receipt.ToString());
                var previous = db.Receipts.Where(x => x.Id == r.Id).FirstOrDefault();
                var res = db.Sp_Update_Receipt(r.Id, r.Amount, r.Name, r.Father_Name, r.Contact, r.AmountinWords, r.PaymentType, r.Bank, r.Ch_Pay_Draft_No, "SA Systems", rece.File_Plot_No, r.Size,
                r.Type, r.TokenParameter, r.Userid, r.DateTime, rece.Module, r.ReceiptNo, r.Ch_Pay_Draft_Date);
                var newr = db.Receipts.Where(x => x.Id == r.Id).FirstOrDefault();
                if (previous.Amount != newr.Amount || previous.PaymentType != newr.PaymentType)
                {
                    //try
                    //{
                    //    AccountHandlerController de = new AccountHandlerController();
                    //    de.Banking_Receipts_Updation_Wiith_Deletion(r.Id, r.Amount, r.Name, r.Contact, r.PaymentType, r.Ch_Pay_Draft_No, r.Ch_Pay_Draft_Date, r.Bank, rece.File_Plot_Number, r.Type, r.ReceiptNo, r.Block);
                    //}
                    //catch (Exception ex)
                    //{
                    //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    //}
                }

                if (!(r.Type == ReceiptTypes.Registry_Charges.ToString()))
                {
                    db.SP_Update_PlotVerificationToNull(rece.File_Plot_No);
                }
                var data1 = new { Status = true, Msg = "Receipt updated successfully" };
                return Json(data1);
            }
            else if (rece.Module == Modules.CommercialManagement.ToString())
            {
                string recedetails = JsonConvert.SerializeObject(rece);
                db.Sp_Add_Activity(userid, "Update receipt of Plot Id  <a class='plt-data' data-id=' " + rece.File_Plot_No + "'>" + rece.File_Plot_No + "</a>---" + recedetails, "Update", Modules.PlotManagement.ToString(), ActivityType.Delete_Receipt.ToString(), rece.File_Plot_No);
                db.Sp_Add_CommercialComments(rece.File_Plot_No, "Updated Receipt : " + rece.ReceiptNo + " Amount: " + string.Format("{0:n0}", rece.Amount) + " of " + rece.PaymentType, userid, ActivityType.Update_Receipt.ToString());
                var previous = db.Receipts.Where(x => x.Id == r.Id).FirstOrDefault();
                var res = db.Sp_Update_Receipt(r.Id, r.Amount, r.Name, r.Father_Name, r.Contact, r.AmountinWords, r.PaymentType, r.Bank, r.Ch_Pay_Draft_No, "SA Systems", rece.File_Plot_No, r.Size,
                r.Type, r.TokenParameter, r.Userid, r.DateTime, rece.Module, r.ReceiptNo, r.Ch_Pay_Draft_Date);
                var newr = db.Receipts.Where(x => x.Id == r.Id).FirstOrDefault();
                if (previous.Amount != newr.Amount || previous.PaymentType != newr.PaymentType)
                {
                    //try
                    //{
                    //    AccountHandlerController de = new AccountHandlerController();
                    //    de.Banking_Receipts_Updation_Wiith_Deletion(r.Id, r.Amount, r.Name, r.Contact, r.PaymentType, r.Ch_Pay_Draft_No, r.Ch_Pay_Draft_Date, r.Bank, rece.File_Plot_Number, r.Type, r.ReceiptNo, r.Block);
                    //}
                    //catch (Exception ex)
                    //{
                    //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    //}
                }

                if (!(r.Type == ReceiptTypes.Registry_Charges.ToString()))
                {
                    db.SP_Update_VerificationToNull(rece.File_Plot_No, ProjectCategory.Building.ToString());
                }
                var data1 = new { Status = true, Msg = "Receipt updated successfully" };
                return Json(data1);
            }
            //else if (rece.Module == Modules.OfficeManagement.ToString())
            //{
            //    return Json(true);
            //}
            var data = new { Status = false, Msg = "Invalid data" };
            return Json(data);
        }
        public JsonResult SupervisePndReceipts(List<long> Id)
        {
            if (Id is null)
            {
                return Json(new Return { Status = false, Msg = "Error" });
            }
            else
            {
                long userid = long.Parse(User.Identity.GetUserId());
                var chids = new XElement("ChPoDd", Id.Select(x => new XElement("Details",
                                      new XAttribute("Ids", x)
                                     ))).ToString();
                var a = db.Sp_Update_Pending_To_Receipts(chids, userid);
                return Json(new Return { Status = true, Msg = "Supervised" });
            }

        }
        [NoDirectAccess] public ActionResult AddOnlineAmount()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }

        [NoDirectAccess] public ActionResult AddOnlineAmountUnSupervised()
        {
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            ViewBag.Bank = new SelectList(db.Bank_Accounts.Select(x => new { id = x.Id, text = x.Bank + " - " + x.Account_Number }).ToList(), "id", "text");
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddOnlineAmountUnSupervised(ReceiptData rd, long? Filefromid, long? PlotId, string From_Bank, string Module, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var receiptno = db.Sp_Get_ReceiptNo(From_Bank).FirstOrDefault() + "-" + From_Bank;
            var bank = int.Parse(rd.Bank);
            var banks = db.Bank_Accounts.Where(x => x.Id == bank).FirstOrDefault();
            var res = db.Sp_Add_UnadjustableAmounts_Pending(rd.Amount, rd.AmountInWords, banks.Bank, rd.PayChqNo, null
                         , null, null, null, rd.PaymentType, rd.TotalAmount,
                         rd.Project_Name, rd.Rate, null, null, "Installment", userid, userid, rd.Date, null /*Filefromid*/, From_Bank, null, "", null /*res1.Plot_No.ToString()*/, null, TransactionId, receiptno, null).FirstOrDefault();
            if (res == "0")
            {
                return Json(new Return { Status = false, Msg = "Transaction cannot be duplicate" });
            }
            else
            {
                return Json(new Return { Status = false, Msg = "Receipt No. is:" + res });
            }
        }
        public JsonResult AddOnlineAmountMultiunitsUnSupervised(long Rcp_Id, List<FilePlotArray> FilePlotArray)
        {
            var pndreceipt = db.Receipts_Pending.Where(x => x.Id == Rcp_Id).FirstOrDefault();

            var listsum = FilePlotArray.Select(x => x.Amount).Sum();
            if (listsum == pndreceipt.Amount)
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        long userid = long.Parse(User.Identity.GetUserId());
                        foreach (var item in FilePlotArray)
                        {
                            Helpers h = new Helpers();
                            var TransactionId = h.RandomNumber();

                            var receiptno = db.Sp_Get_ReceiptNo(item.From_Bank).FirstOrDefault() + "-" + item.From_Bank;
                            var bank = int.Parse(item.Bank);
                            var banks = db.Bank_Accounts.Where(x => x.Id == bank).FirstOrDefault();
                            var res = db.Sp_Add_UnadjustableAmounts_Pending(item.Amount, item.AmountInWords, banks.Bank, pndreceipt.Ch_Pay_Draft_No, null
                                         , null, pndreceipt.Id, null, pndreceipt.PaymentType, pndreceipt.Plot_Total_Amount,
                                         pndreceipt.Project, pndreceipt.RatePerMarla, null, null, "Installment", userid, userid, pndreceipt.DateTime, null /*Filefromid enter effected receipt id in saleid coulmn*/, item.From_Bank, null, "", null /*res1.Plot_No.ToString()*/, null, TransactionId, receiptno, null).FirstOrDefault();
                            if (res == "0")
                            {
                                return Json(new Return { Status = false, Msg = "Transaction cannot be duplicate" });
                            }
                        }
                        (from x in db.Receipts_Pending where x.Id == pndreceipt.Id select x).ToList().ForEach(xx => xx.Sale_Id = 0);
                        db.SaveChanges();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Transaction Added Sucessfully" });
                    }
                    catch (Exception e)
                    {
                        Transaction.Rollback();
                        return Json(false);
                    }
                }
            }
            else
            {
                return Json(new Return { Status = false, Msg = "Sum of all enter amounts should be equal to Receipt Amount." });
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddOnlineAmountPending(ReceiptData rd, long? Filefromid, long? PlotId, string From_Bank, string Module, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var receiptno = db.Sp_Get_ReceiptNo(From_Bank).FirstOrDefault() + "-" + From_Bank;
            if (Module == Modules.PlotManagement.ToString())
            {
                var res1 = db.Sp_Get_PlotData(PlotId).FirstOrDefault();
                var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();

                var res = db.Sp_Add_UnadjustableAmounts_Pending(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, string.Join(",", res2.Select(x => x.Mobile_1))
                             , string.Join(",", res2.Select(x => x.Father_Husband)), PlotId, string.Join(",", res2.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
                             rd.Project_Name, rd.Rate, null, res1.Plot_Size, rd.Type, userid, userid, rd.Date, Filefromid, From_Bank, Modules.PlotManagement.ToString(), "", res1.Plot_No.ToString(), res1.Block_Name, TransactionId, receiptno, null).FirstOrDefault();
                db.SP_Update_VerificationToNull(res1.Id, Modules.PlotManagement.ToString());
                if (res == "0")
                {
                    return Json(new Return { Status = false, Msg = "Duplicate transactions are not allowed" });
                }
                else
                {
                    return Json(new Return { Status = false, Msg = "Receipt No. is:" + res });
                }

            }
            else if (Module == Modules.FileManagement.ToString())
            {
                var res1 = db.File_Form.Where(x => x.Id == PlotId).FirstOrDefault();
                var res5 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
                var res = db.Sp_Add_UnadjustableAmounts_Pending(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, string.Join(",", res5.Select(x => x.Mobile_1))
             , string.Join(",", res5.Select(x => x.Father_Husband)), rd.File_Plot_Number, string.Join(",", res5.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
             rd.Project_Name, rd.Rate, null, res1.Plot_Size, rd.Type, userid, userid, rd.Date, Filefromid, From_Bank, Modules.FileManagement.ToString(), "", res1.FileFormNumber.ToString(), res1.Block, TransactionId, receiptno, null).FirstOrDefault();
                db.SP_Update_VerificationToNull(res1.Id, Modules.FileManagement.ToString());

                if (res == "0")
                {
                    return Json(new Return { Status = false, Msg = "Duplicate transactions are not allowed" });
                }
                else
                {
                    return Json(new Return { Status = false, Msg = "Receipt No. is:" + res });
                }
            }
            else if (Module == Modules.CommercialManagement.ToString())
            {
                var res1 = db.Sp_Get_CommercialData(PlotId).FirstOrDefault();
                var res5 = db.Sp_Get_CommercialLastOwner(PlotId).ToList();

                var res = db.Sp_Add_UnadjustableAmounts_Pending(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, string.Join(",", res5.Select(x => x.Mobile_1))
             , string.Join(",", res5.Select(x => x.Father_Husband)), PlotId, string.Join(",", res5.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
                 res1.Project_Name, rd.Rate, null, res1.Area.ToString() + " sq-ft", rd.Type, userid, userid, rd.Date, Filefromid, From_Bank, Modules.CommercialManagement.ToString(), "", res1.ApplicationNo + " - " + res1.shop_no, res1.Floor, TransactionId, receiptno, null).FirstOrDefault();
                db.SP_Update_VerificationToNull(res1.Id, Modules.CommercialManagement.ToString());

                if (res == "0")
                {
                    return Json(new Return { Status = false, Msg = "Duplicate transactions are not allowed" });
                }
                else
                {
                    return Json(new Return { Status = false, Msg = "Receipt No. is:" + res });
                }
            }
            else
            {
                return Json(new Return { Status = false, Msg = "Duplicate transactions are not allowed" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddOnlineAmount(ReceiptData rd, long? Filefromid, long? PlotId, string From_Bank, string Module, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var receiptno = db.Sp_Get_ReceiptNo(From_Bank).FirstOrDefault() + "-" + From_Bank;
            if (Module == Modules.PlotManagement.ToString())
            {
                var res1 = db.Sp_Get_PlotData(PlotId).FirstOrDefault();
                var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();

                var res = db.Sp_Add_UnadjustableAmounts(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, string.Join(",", res2.Select(x => x.Mobile_1))
                             , string.Join(",", res2.Select(x => x.Father_Husband)), PlotId, string.Join(",", res2.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
                             rd.Project_Name, rd.Rate, null, res1.Plot_Size, ReceiptTypes.Installment.ToString(), userid, userid, rd.Date, Filefromid, From_Bank, Modules.PlotManagement.ToString(), "", res1.Plot_No.ToString(), res1.Block_Name, TransactionId, receiptno, res1.Type).FirstOrDefault();
                db.SP_Update_VerificationToNull(res1.Id, Modules.PlotManagement.ToString());
                if (res == "0")
                {
                    return Json(new Return { Status = false, Msg = "Duplicate transactions are not allowed" });
                }
                else
                {
                    return Json(new Return { Status = false, Msg = "Receipt No. is:" + res });
                }

            }
            else if (Module == Modules.FileManagement.ToString())
            {
                var res1 = db.File_Form.Where(x => x.Id == PlotId).FirstOrDefault();
                var res5 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
                var res = db.Sp_Add_UnadjustableAmounts(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, string.Join(",", res5.Select(x => x.Mobile_1))
             , string.Join(",", res5.Select(x => x.Father_Husband)), PlotId, string.Join(",", res5.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
             rd.Project_Name, rd.Rate, null, res1.Plot_Size, ReceiptTypes.Installment.ToString(), userid, userid, rd.Date, Filefromid, From_Bank, Modules.FileManagement.ToString(), "", res1.FileFormNumber.ToString(), res1.Block, TransactionId, receiptno, res1.Type).FirstOrDefault();
                db.SP_Update_VerificationToNull(res1.Id, Modules.FileManagement.ToString());

                if (res == "0")
                {
                    return Json(new Return { Status = false, Msg = "Duplicate transactions are not allowed" });
                }
                else
                {
                    return Json(new Return { Status = false, Msg = "Receipt No. is:" + res });
                }
            }
            else if (Module == Modules.CommercialManagement.ToString())
            {
                var res1 = db.Sp_Get_CommercialData(PlotId).FirstOrDefault();
                var res5 = db.Sp_Get_CommercialLastOwner(PlotId).ToList();

                var res = db.Sp_Add_UnadjustableAmounts(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, string.Join(",", res5.Select(x => x.Mobile_1))
             , string.Join(",", res5.Select(x => x.Father_Husband)), PlotId, string.Join(",", res5.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
                 res1.Project_Name, rd.Rate, null, res1.Area.ToString() + " sq-ft", ReceiptTypes.Installment.ToString(), userid, userid, rd.Date, Filefromid, From_Bank, Modules.CommercialManagement.ToString(), "", res1.ApplicationNo + " - " + res1.shop_no, res1.Floor, TransactionId, receiptno, res1.Type).FirstOrDefault();
                db.SP_Update_VerificationToNull(res1.Id, Modules.FileManagement.ToString());

                if (res == "0")
                {
                    return Json(new Return { Status = false, Msg = "Duplicate transactions are not allowed" });
                }
                else
                {
                    return Json(new Return { Status = false, Msg = "Receipt No. is:" + res });
                }
            }
            else
            {
                return Json(new Return { Status = false, Msg = "Duplicate transactions are not allowed" });
            }
        }
        [NoDirectAccess] public ActionResult PlotReceipt(string Id, long Token)
        {
            var res = db.Sp_Get_Receipt_Parameter(Id, Token).SingleOrDefault();
            return View(res);
        }
        [NoDirectAccess] public ActionResult BidingPlotReceipt(string Id)
        {
            var res = (from x in db.Receipts where x.DevelopmentCharges == Id select x).ToList();
            return View(res);
        }
   
        [NoDirectAccess] public ActionResult DishonredCheqList()
        {
            var res = db.Sp_Get_DishonredCheques().ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Dishounered Cheques Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return View(res);
        }
        [NoDirectAccess] public ActionResult ReturnedCheques()
        {
            var res = db.Cheque_DemandDraft_PayOrder.Where(x => x.Status == PaymentMethodStatuses.Dishonored.ToString() && x.Delivered_Back == true).ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Returnes Cheques Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult DisHoncheqesDetails(string No, DateTime Date, string Bank, string Status)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            var res = db.Sp_Get_Cheque_BankDraft_PayOrder_MultiParamter(No, Status, Bank, Date, Voucher_Type.BRV.ToString()).ToList();
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Details of Dishounered Cheques No. " + No, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }

        //bool isFine,
        [HttpPost]
        public JsonResult GenerateCancelation(long[] Id, List<ReceiptData> Receiptdata, List<ReceiptData> FineData, long[] RecpIds, bool CancelFile, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            List<ReceiptModel> ids = new List<ReceiptModel>();

            try
            {
                var cheq = db.Cheque_DemandDraft_PayOrder.Where(x => Id.Contains(x.Id)).ToList();
                var receiptids = db.Receipts.Where(x => RecpIds.Contains(x.Id)).ToList();


                string receipt = "", date = "";
                //////// Cancel the receitps
                foreach (var id in receiptids)
                {
                    db.Sp_Update_ReceiptCancel(id.Id, "Recovery Against dishornered Cheque");
                    receipt = receipt + ",";
                    date = string.Format("{0:dd-MM-yyyy}", id.DateTime);
                }
                ///// Process the cheques
                foreach (var id in Id)
                {
                    db.Sp_Update_ChequePrcess(id);
                }
                ////// 
                string file = "", bank = "", Cheq_No = "", Fileno = "", type = ""; DateTime? Date = null; long? Owner_Id = 0;
                ///// Release the information of Cheques
                foreach (var item in cheq)
                {
                    file = file + item.File_No + ", ";
                    bank = item.Bank;
                    Cheq_No = item.Cheque_BankDraft_Payorder_Number;
                    Date = item.Cheque_DemandDraft_PayOrder_Date;
                    Fileno = item.File_No;
                    type = item.Type;
                    //if (!CancelFile)
                    //{
                    //    Owner_Id = this.Backdoor(item.Reference_Id, item.Module, item.Type);
                    //}
                }
                // Cancelation Receipt
                // Recovery receipts
                int Onetime = 1;

                if (CancelFile)
                {
                    using (var Transcation = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in RecpIds)
                            {

                                var rcid = (from x in db.Receipts
                                            join y in db.Cheque_DemandDraft_PayOrder on x.Id equals y.ReceiptId
                                            where x.Id == item
                                            select new { Receipt = x, Instrument = y }).SingleOrDefault();

                                if (Onetime == 1)
                                {
                                    foreach (var v in FineData)
                                    {
                                        var receiptno1 = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                                        var res1 = db.Sp_Add_Receipt(v.Amount, v.AmountInWords, v.Bank, v.PayChqNo, v.Ch_bk_Pay_Date, v.Branch, rcid.Receipt.Contact
                                                                    , rcid.Receipt.Father_Name, rcid.Receipt.File_Plot_No, rcid.Receipt.Name, v.PaymentType, null,
                                                                    "Grand City", null, null, rcid.Receipt.Size, ReceiptTypes.Cancellation.ToString(), userid, userid, "Cancelled against Dishonored Cheque " + bank + "  " + Cheq_No + "  " + receipt + " Date:" + date, null, rcid.Receipt.Module, "", file, rcid.Receipt.Block, rcid.Receipt.Plot_Type, Owner_Id, TransactionId, rcid.Receipt.Dealership, receiptno1, comp.Id).FirstOrDefault();
                                        {
                                            bool headcashier = false;
                                            if (User.IsInRole("Head Cashier"))
                                            {
                                                headcashier = true;
                                            }
                                            try
                                            {
                                                AccountHandlerController de = new AccountHandlerController();
                                                de.Dishonor_Cheq_Recovery(v.Amount, file, rcid.Receipt.Plot_Type, rcid.Receipt.Block, v.PaymentType, v.PayChqNo, v.Ch_bk_Pay_Date, v.Bank, TransactionId, userid, res1.Receipt_No, 1, headcashier);
                                            }
                                            catch (Exception ex)
                                            {
                                                Transcation.Rollback();
                                                db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                            }

                                        }
                                        if (v.PaymentType != "Cash")
                                        {
                                            var res33 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(v.Amount, v.Bank, v.Branch, v.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                                rcid.Receipt.Module, rcid.Receipt.Type, rcid.Instrument.Reference_Id, v.PayChqNo, rcid.Instrument.Module_Id, v.Ch_bk_Pay_Date, rcid.Receipt.File_Plot_Number, res1.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                                            var text = "Dear " + rcid.Receipt.Name + ",\n\r" +
                                        "A Payment of Rs " + string.Format("{0:n0}", rcid.Receipt.Amount) + " has been received against " + v.PaymentType + " No: " + v.PayChqNo + " Bank: " + v.Bank + " for File number# " + rcid.Receipt.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

                                            if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                                            {
                                                Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                                            }
                                            string dt = string.Format("{0:d/M/yyyy}", rcid.Instrument.Cheque_DemandDraft_PayOrder_Date);
                                            var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rcid.Instrument.Cheque_BankDraft_Payorder_Number + "_" + rcid.Instrument.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                                            try
                                            {
                                                Helpers H = new Helpers();
                                                var Imgres = H.SaveBase64Image(v.FileImage, pathMain, res33.ToString());
                                                SmsService smsService = new SmsService();
                                                smsService.SendMsg(text, rcid.Receipt.Contact);
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                        ReceiptModel r1 = new ReceiptModel()
                                        {
                                            ReceiptNo = res1.Receipt_No,
                                            Type = ReceiptTypes.Cancellation.ToString(),
                                            Module = rcid.Receipt.Module
                                        };

                                        ids.Add(r1);
                                    }
                                    Onetime++;
                                }
                            }

                            Transcation.Commit();
                        }
                        catch (Exception ex)
                        {
                            Transcation.Rollback();
                            var ra = db.Sp_Add_ErrorLog(ex.Message + ex.InnerException, "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                        }
                    }

                }
                else
                {
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var rd in Receiptdata)
                            {
                                foreach (var item in RecpIds)
                                {
                                    var rcid = (from x in db.Receipts
                                                join y in db.Cheque_DemandDraft_PayOrder on x.Id equals y.ReceiptId
                                                where x.Id == item
                                                select new { Receipt = x, Instrument = y }).SingleOrDefault();
                                    if (Onetime == 1)
                                    {
                                        foreach (var v in FineData)
                                        {
                                            var receiptno2 = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                                            var res1 = db.Sp_Add_Receipt(v.Amount, v.AmountInWords, v.Bank, v.PayChqNo, v.Ch_bk_Pay_Date, v.Branch, rcid.Receipt.Contact
                                                                        , rcid.Receipt.Father_Name, rcid.Receipt.File_Plot_No, rcid.Receipt.Name, v.PaymentType, null,
                                                                        "Grand City Kharian", null, null, rcid.Receipt.Size, ReceiptTypes.Cancellation.ToString(), userid, userid, "Cancelled against Dishonored Cheque " + bank + "  " + Cheq_No + "  " + receipt + " Date:" + date, null, rcid.Receipt.Module, "", file, rcid.Receipt.Block, rcid.Receipt.Plot_Type, 0, TransactionId, rcid.Receipt.Size, receiptno2, comp.Id).FirstOrDefault();

                                            {
                                                bool headcashier = false;
                                                if (User.IsInRole("Head Cashier"))
                                                {
                                                    headcashier = true;
                                                }
                                                try
                                                {
                                                    AccountHandlerController de = new AccountHandlerController();
                                                    de.Dishonor_Cheq_Recovery(v.Amount, file, rcid.Receipt.Plot_Type, rcid.Receipt.Block, v.PaymentType, v.PayChqNo, v.Ch_bk_Pay_Date, v.Bank, TransactionId, userid, res1.Receipt_No, 1, headcashier);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Transaction.Rollback();
                                                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                                }
                                            }
                                            if (v.PaymentType != "Cash")
                                            {
                                                var res33 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(v.Amount, v.Bank, v.Branch, v.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                                    rcid.Receipt.Module, rcid.Receipt.Type, rcid.Instrument.Reference_Id, v.PayChqNo, rcid.Instrument.Module_Id, v.Ch_bk_Pay_Date, rcid.Receipt.File_Plot_Number, res1.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                                                var text = "Dear " + rcid.Receipt.Name + ",\n\r" +
                                            "A Payment of Rs " + string.Format("{0:n0}", rcid.Receipt.Amount) + " has been received against " + v.PaymentType + " No: " + v.PayChqNo + " Bank: " + v.Bank + " for File number# " + rcid.Receipt.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

                                                try
                                                {
                                                    SmsService smsService = new SmsService();
                                                    smsService.SendMsg(text, rcid.Receipt.Contact);
                                                }
                                                catch (Exception)
                                                {
                                                }
                                            }
                                            ReceiptModel r1 = new ReceiptModel()
                                            {
                                                ReceiptNo = res1.Receipt_No,
                                                Type = ReceiptTypes.Cancellation.ToString(),
                                                Module = rcid.Receipt.Module
                                            };
                                            ids.Add(r1);
                                        }
                                        Onetime++;
                                    }
                                    Helpers h = new Helpers();
                                    string Block = "", Type = "", Plot_No = ""; long? Block_Id = 0;
                                    if (rcid.Receipt.Module == Modules.FileManagement.ToString())
                                    {
                                        var filedata = db.File_Form.Where(x => x.Id == rcid.Receipt.File_Plot_No).FirstOrDefault();
                                        Block = filedata.Block;
                                        Block_Id = filedata.Block_Id;
                                        Type = filedata.Type;
                                        Plot_No = filedata.FileFormNumber.ToString();
                                    }
                                    else if (rcid.Receipt.Module == Modules.PlotManagement.ToString())
                                    {
                                        var Plotdata = db.Plots.Where(x => x.Id == rcid.Receipt.File_Plot_No).FirstOrDefault();
                                        Block_Id = Plotdata.Block_Id;
                                        Block = Plotdata.Block_Name;
                                        Type = Plotdata.Type;
                                        Plot_No = Plotdata.Plot_Number.ToString() + " " + Plotdata.Plot_Number.ToString();
                                    }
                                    else if (rcid.Receipt.Module == Modules.CommercialManagement.ToString())
                                    {
                                        var comdata = db.Sp_Get_CommercialData(rcid.Receipt.File_Plot_No).FirstOrDefault();
                                        Block_Id = comdata.Floor_Id;
                                        Block = comdata.Floor;
                                        Type = comdata.Type;
                                        Plot_No = comdata.ApplicationNo;
                                    }

                                    var Transaction_Id = h.RandomNumber();
                                    var receiptno3 = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

                                    var res2 = db.Sp_Add_Receipt(rcid.Receipt.Amount, rcid.Receipt.AmountinWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rcid.Receipt.Contact
                                                   , rcid.Receipt.Father_Name, rcid.Receipt.File_Plot_No, rcid.Receipt.Name, rd.PaymentType, null,
                                                   rcid.Receipt.Project, null, null, rcid.Receipt.Size, rcid.Receipt.Type, userid, userid, "Against Dishonored Cheque -" + bank + " - " + Cheq_No + " - " + "Amount: " + string.Format("{0:n0}", rcid.Receipt.Amount) + " Receipt No: " + rcid.Receipt.ReceiptNo, null, rcid.Receipt.Module, rcid.Receipt.DevelopmentCharges, Plot_No, Block, Type, Owner_Id, Transaction_Id, rcid.Receipt.Size, receiptno3, comp.Id).FirstOrDefault();

                                    //{
                                    //    bool headcashier = false;
                                    //    if (User.IsInRole("Head Cashier"))
                                    //    {
                                    //        headcashier = true;
                                    //    }
                                    //    try
                                    //    {
                                    //        AccountHandlerController de = new AccountHandlerController();
                                    //        de.Receive_Plot_Amount(rcid.Receipt.Amount, Plot_No, Type, Block, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, new Helpers().RandomNumber(), userid, res2.Receipt_No, 1, headcashier, COA_Mapper_Modules.Files_Plots.ToString(), Convert.ToInt64(Block_Id));
                                    //    }
                                    //    catch (Exception ex)
                                    //    {
                                    //        Transaction.Rollback();
                                    //        db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                    //    }

                                    //}

                                    if (rd.PaymentType != "Cash")
                                    {
                                        var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rcid.Receipt.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                            rcid.Receipt.Module, rcid.Receipt.Type, rcid.Instrument.Reference_Id, rd.PayChqNo, rcid.Instrument.Module_Id, rd.Ch_bk_Pay_Date, rcid.Receipt.File_Plot_Number, res2.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                                        var text = "Dear " + rd.Name + ",\n\r" +
                                    "A Payment of Rs " + string.Format("{0:n0}", rcid.Receipt.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for File number# " + rcid.Receipt.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

                                        try
                                        {
                                            SmsService smsService = new SmsService();
                                            smsService.SendMsg(text, rd.Mobile_1);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    else
                                    {
                                        var text = "Dear " + rd.Name + ",\n\r" +
                                        "A Payment of Rs " + string.Format("{0:n0}", rcid.Receipt.Amount) + " has been received in cash for File number# " + rcid.Receipt.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                                        //var res1 = db.Sp_Add_Receipt(1000, "One Thousand", bank, Cheq_No, Date, null, rcid.Receipt.Contact
                                        //                            , rcid.Receipt.Father_Name, rcid.Receipt.File_Plot_No, rcid.Receipt.Name, "Cash", null,
                                        //                            "SA Systems", null, null, rcid.Receipt.Size, ReceiptTypes.Cancellation.ToString(), userid, userid, "Cancelled against Dishonored Cheque " + bank + "  " + Cheq_No + "  " + receipt + " Date:" + date, null, rcid.Receipt.Module, "", file, rcid.Receipt.Block, rcid.Receipt.Plot_Type, 0, h.RandomNumber()).FirstOrDefault();

                                        //{
                                        //    de.Dishonor_Cheq_Recovery(1000, file, rcid.Receipt.Plot_Type, rcid.Receipt.Block, "Cash", "", null, "", TransactionId, userid, res1.Receipt_No, 1);
                                        //}

                                        try
                                        {
                                            SmsService smsService = new SmsService();
                                            smsService.SendMsg(text, rd.Mobile_1);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    ReceiptModel r2 = new ReceiptModel()
                                    {
                                        ReceiptNo = res2.Receipt_No,
                                        Type = type,
                                        Module = rcid.Receipt.Module
                                    };
                                    ids.Add(r2);

                                }
                            }
                            Transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            EmailService e = new EmailService();
                            e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "SaveIssueRequest Demand Order Error");

                            Transaction.Rollback();
                            var data1 = new Return { Status = false, Msg = "Error Occured" };
                            return Json(data1);
                        }
                    }
                }

                var data = new { Receiptid = ids, Token = userid };
                return Json(data);
            }
            catch (Exception ex)
            {
                EmailService e = new EmailService();
                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "SaveIssueRequest Demand Order Error");

                var data = new Return { Status = false, Msg = "Error Occured" };
                return Json(data);
            }

        }
        [NoDirectAccess] public ActionResult DishonoredCheqs()
        {
            var res = db.Sp_Get_DishonredCheques().ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult BanksMIS()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult Search_BanksMIS(DateTime From, DateTime To, string Val)
        {
            var res = db.SP_Reports_BANK_MIS(From, To, Val).ToList();
            ViewBag.Bank = db.Bank_Online_Accounts.ToList();
            return PartialView(res);
        }
        // SA Marketing  -- SAS Property Exchange
        [NoDirectAccess] public ActionResult LeadAllBankPayments(string Company)
        {
            ViewBag.Comp = Company;
            return View();
        }
        [NoDirectAccess] public ActionResult Leads_Cheques(string Status, string Company)
        {
            var res = db.Sp_Get_ChDDPO_Leads("Cheque", Status, Company).ToList();
            ViewBag.Status = Status;
            ViewBag.Comp = Company;
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult Leads_BankDrafts(string Status, string Company)
        {
            var res = db.Sp_Get_ChDDPO_Leads("BankDraft", Status, Company).ToList();
            ViewBag.Comp = Company;
            ViewBag.Status = Status;
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult Leads_Payorder(string Status, string Company)
        {
            var res = db.Sp_Get_ChDDPO_Leads("PayOrder", Status, Company).ToList();
            ViewBag.Comp = Company;
            ViewBag.Status = Status;
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult Leads_Online(string Status, string Company)
        {
            var res = db.Sp_Get_Online_Leads(Status, Company).ToList();
            ViewBag.Comp = Company;
            ViewBag.Status = Status;
            return PartialView(res);
        }

        [NoDirectAccess] public ActionResult LeadsCBPDetails(string No, DateTime Date, string Bank, string Status)
        {
            var res = db.Sp_Get_LeadInstrument_MultiParamter(No, Status, Bank, Date).ToList();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.Reason = new SelectList(Nomenclature.DishoneredReason(), "Value", "Name");
            var paymentstatus = from PaymentMethodStatuses e in Enum.GetValues(typeof(PaymentMethodStatuses)) select new { Name = e.ToString() };
            ViewBag.Status = new SelectList(paymentstatus, "Name", "Name");
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LeadUpdateCbp(DateTime? Posted_Date, string Status, string Bank, string Deposit_Bank_Acc_Number, DateTime? Clearance_Date, string Reason, long[] Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var chids = new XElement("ChPoDd", Id.Select(x => new XElement("Details",
                                   new XAttribute("Ids", x)
                                  ))).ToString();
            var Returnids = db.Sp_Get_Leads_ChDDPODetails_Multi(chids).ToList();
            string name = "", mobile = "", type = "", number = "", fileno = "";
            foreach (var cbp in Returnids)
            {
                fileno = fileno + cbp.File_Plot_Number + " ";
                var res = db.Sp_Update_LeadsInstrument(cbp.Id, Status, Bank, Deposit_Bank_Acc_Number, cbp.Module, cbp.Type, null, Clearance_Date, null, Posted_Date, Reason);
                name = cbp.Name; mobile = cbp.Contact; type = cbp.PaymentType; number = cbp.Ch_Pay_Draft_No;
            }
            if (Status == PaymentMethodStatuses.Approved.ToString())
            {
                SmsService sms = new SmsService();

                string msg = "Dear " + name + "\n\r" +
                    "Your " + type + " No. " + number + " has been Cleared against File Number: " + fileno + ". Thank you for your payment.";
                try
                {
                    sms.SendMsg(msg, mobile);
                }
                catch (Exception)
                {
                }
            }
            else if (Status == PaymentMethodStatuses.Dishonored.ToString())
            {
                SmsService sms = new SmsService();

                string msg = "Dear " + name + "\n\r" +
                    "Your " + type + " No. " + number + " has been Bounced Due to " + Reason + ". A penalty of Rs 1,000 charged to your File number#" + fileno +
                    ". Please collect your " + type + " from Accounts Department and clear your dues with in 3 Days.";
                try
                {
                    sms.SendMsg(msg, mobile);
                }
                catch (Exception)
                {
                }
            }
            return Json(true);
        }
        [NoDirectAccess] public ActionResult LeadsDishonoredCheqs()
        {
            var res = db.Sp_Get_LeadsDishonredCheques().ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult LeadsDisHoncheqesDetails(string No, DateTime Date, string Bank, string Status)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            var res = db.Sp_Get_LeadInstrument_MultiParamter(No, Status, Bank, Date).ToList();
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult LeadsGenerateCancelation(long[] Id, List<ReceiptData> Receiptdata, bool CancelFile)
        {
            var TransactionId = new Helpers().RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            List<ReceiptModel> ids = new List<ReceiptModel>();

            var chids = new XElement("ChPoDd", Id.Select(x => new XElement("Details",
                                new XAttribute("Ids", x)
                               ))).ToString();
            var Rece_cheq = db.Sp_Get_Leads_ChDDPODetails_Multi(chids).ToList();

            string receipt = "", date = "";
            //////// Cancel the receitps
            foreach (var id in Rece_cheq)
            {
                db.Sp_Update_LeadReceiptCancel(id.Id, "Recovery Against dishornered Cheque");
                receipt = receipt + ",";
                date = string.Format("{0:dd-MM-yyyy}", id.DateTime);
            }

            ////// 
            string file = "", bank = "", Cheq_No = "", Fileno = "", type = "", Company = ""; DateTime? Date = null;
            ///// Release the information of Cheques
            foreach (var item in Rece_cheq)
            {
                file = file + item.File_Plot_Number + ", ";
                bank = item.Bank;
                Cheq_No = item.Ch_Pay_Draft_No;
                Date = item.Ch_Pay_Draft_Date;
                Fileno = item.File_Plot_Number;
                type = item.Type;
                Company = item.Company;
            }
            // Recovery receipts
            int Onetime = 1;
            foreach (var rd in Receiptdata)
            {
                foreach (var item in Rece_cheq)
                {
                    if (Onetime == 1)
                    {
                        var res1 = db.Sp_Add_SAM_Receipt(1000, "One Thousand", rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, item.Contact
                                           , item.Father_Name, item.Lead_Id, item.Name, rd.PaymentType, item.Plot_Total_Amount,
                                           rd.Project_Name, item.RatePerMarla, item.Size, ReceiptTypes.Cancellation.ToString(), userid, userid, "Cancelled against Dishonored Cheque " + bank + "  " + Cheq_No + "  " + receipt + " Date:" + date, null, item.Module, item.DevelopmentCharges, item.File_Plot_Number, item.Block, item.Company, TransactionId).FirstOrDefault();

                        ReceiptModel r1 = new ReceiptModel()
                        {
                            ReceiptNo = res1.Receipt_No,
                            Type = ReceiptTypes.Cancellation.ToString(),
                            Module = item.Module,
                            ReceiptId = res1.Receipt_Id
                        };
                        ids.Add(r1);
                        Onetime++;
                    }
                    if (rd.Amount <= 0)
                    {
                        continue;
                    }
                    var res2 = db.Sp_Add_SAM_Receipt(item.Amount, item.AmountinWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, item.Contact
                                           , item.Father_Name, item.Lead_Id, item.Name, rd.PaymentType, item.Plot_Total_Amount,
                                           rd.Project_Name, item.RatePerMarla, item.Size, item.Type, userid, userid, "Against Dischounred Cheque -" + bank + " - " + Cheq_No + " - " + "Amount: " + string.Format("{0:n0}", item.Amount) + " Receipt No: " + item.ReceiptNo, null, item.Module, item.DevelopmentCharges, item.File_Plot_Number, item.Block, item.Company, TransactionId).FirstOrDefault();


                    if (rd.PaymentType != "Cash")
                    {
                        var text = "Dear " + rd.Name + ",\n\r" +
                        "A Payment of Rs " + string.Format("{0:n0}", item.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for File number# " + item.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";
                        if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                        }
                        string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                        var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                        try
                        {
                            Helpers H = new Helpers();
                            var Imgres = H.SaveBase64Image(rd.FileImage, pathMain, res2.ToString());
                            SmsService smsService = new SmsService();
                            smsService.SendMsg(text, rd.Mobile_1);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        var text = "Dear " + rd.Name + ",\n\r" +
                        "A Payment of Rs " + string.Format("{0:n0}", item.Amount) + " has been received in cash for File number# " + item.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";

                        try
                        {
                            SmsService smsService = new SmsService();
                            smsService.SendMsg(text, rd.Mobile_1);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    ReceiptModel r2 = new ReceiptModel()
                    {
                        ReceiptNo = res2.Receipt_No,
                        Type = type,
                        Module = item.Module,
                        ReceiptId = res2.Receipt_Id

                    };
                    ids.Add(r2);

                }
            }
            var data = new { Receiptid = ids, Token = userid, Company = Company };
            return Json(data);
        }
        // Banks & Accounts
        [NoDirectAccess] public ActionResult BankAndAccount()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res = db.Bank_Accounts.Where(x => x.Comp_Id == comp.Id).ToList();
            var mapperAccounts = db.Sp_Get_COAMapper_BankAccounts(comp.Id).ToList();
            return PartialView(new BankAccountsMapper { BankAccounts = res, BankAccountsCOA = mapperAccounts });
        }
        public JsonResult AddBankAccount(Bank_Accounts bankaccouunt)
        {
            var res = db.Sp_Add_BankAccount(bankaccouunt.Bank, bankaccouunt.Account_Number).FirstOrDefault();
            {
                // chart of Account Addition

            }

            return Json(res);
        }
        [NoDirectAccess] public ActionResult GetBankAccount()
        {
            var res = db.Bank_Accounts.ToList();
            return PartialView(res);

        }
        public JsonResult RemoveBankAccount(long Id)
        {
            db.Sp_Delete_BankAccount(Id);
            return Json(true);
        }
        public void GenerateCheques()
        {
            var res = db.temp_Sp1().ToList();
            List<Cheque_DemandDraft_PayOrder> asd = new List<Cheque_DemandDraft_PayOrder>();
            foreach (var x in res)
            {
                Cheque_DemandDraft_PayOrder c = new Cheque_DemandDraft_PayOrder()
                {
                    Amount = x.Amount,
                    Bank = x.Bank,
                    Branch_Name = x.Branch_Name,
                    Check_DemandDraft_PayOrder = x.PaymentType,
                    Cheque_BankDraft_Payorder_Number = x.Ch_Pay_Draft_No,
                    Cheque_DemandDraft_PayOrder_Date = x.Ch_Pay_Draft_Date,
                    Status = "Approved",
                    Module = x.Module,
                    Type = x.Type,
                    Reference_Id = 1,
                    Module_Id = x.File_Plot_No,
                    File_No = x.File_Plot_Number + " - " + x.Block,
                    ReceiptId = x.Id
                };
                asd.Add(c);
            }
            db.Cheque_DemandDraft_PayOrder.AddRange(asd);
            db.SaveChanges();


        }
        public JsonResult UpdateChargeBack(long Id)
        {
            var res = db.Sp_Update_Instrument_Status(Id, "Dishonored");
            return Json(true);
        }
        private FinancialYear GetFinancialYear()
        {
            return new FinancialYear { Start = new DateTime(2022, 7, 1), End = new DateTime(2023, 6, 30) };
        }
        public JsonResult CashierDayClose()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var dt = DateTime.Now;
            db.Sp_Update_Receipts_CashierDayClose(userid, dt);
            return Json(true);
        }
        public JsonResult CashierSAMDayClose()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var dt = DateTime.Now;
            db.Sp_Update_SAM_CashierDayClose(userid, dt);
            return Json(true);
        }
        [NoDirectAccess] public ActionResult OnlineBankAccount()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            ViewBag.Bank = db.Bank_Accounts.Select(x => new { x.Bank, x.Account_Number }).ToList();
            var res = db.Bank_Online_Accounts.ToList();
            var mapperAccounts = db.Sp_Get_COAMapper_OnlineBank(comp.Id).ToList();
            return PartialView(new OnlineBankAccountsMapper { OnlineBankAccounts = res, OnlineBankAccountsCOA = mapperAccounts });
        }
        public JsonResult AddBankOnlineAccount(Bank_Accounts bankaccouunt)
        {
            var Bank = db.Bank_Accounts.Where(x => x.Account_Number == bankaccouunt.Account_Number && x.Bank == bankaccouunt.Bank).FirstOrDefault();
            Bank_Online_Accounts a = new Bank_Online_Accounts()
            {
                Bank = Bank.Bank,
                Account_No = Bank.Account_Number,
                Bank_Id = Bank.Id,
                Code = ""
            };
            db.Bank_Online_Accounts.Add(a);
            db.SaveChanges();
            return Json(true);
        }
        [NoDirectAccess] public ActionResult GetBankOnlineAccount()
        {
            var res = db.Bank_Online_Accounts.ToList();
            return PartialView(res);
        }
        public JsonResult RemoveBankOnlineAccount(long Id)
        {
            var a = db.Bank_Online_Accounts.Where(x => x.Id == Id).ToList();
            db.Bank_Online_Accounts.RemoveRange(a);
            db.SaveChanges();
            return Json(true);
        }
        public JsonResult ReadySuperviseOnline(List<long?> Id, int BankId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var bank = db.Bank_Online_Accounts.Where(x => x.Id == BankId).FirstOrDefault();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    AccountHandlerController ah = new AccountHandlerController();
                    foreach (var item in Id)
                    {
                        var res = ah.UnderSuper(item, bank.Account_No, userid);
                    }
                    Transaction.Commit();
                    return Json(new Return { Status = true, Msg = "Supervised" });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    EmailService e = new EmailService();
                    e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Supervision Entries");
                    return Json(new Return { Status = false, Msg = "Error Occured" });

                }
            }
        }



        [NoDirectAccess] public ActionResult InstrumentClearing()
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Banks Payments Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView();
        }
        [NoDirectAccess] public ActionResult PDCCheques(string Status, DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            ViewBag.Status = Status;
            if (From == null || To == null)
            {
                var fy = this.GetFinancialYear();
                From = fy.Start;
                To = fy.End;

                ViewBag.From = From;
                ViewBag.To = To;
            }
            else
            {
                ViewBag.From = From;
                ViewBag.To = To;
            }
            var res = db.Sp_Get_ChDDPO("Cheque", Status, From, To, Voucher_Type.BPV.ToString(), comp.Id).ToList();

            db.Sp_Add_Activity(userid, "Accessed Cheques Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult PDCBankDrafts(string Status, DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            ViewBag.Status = Status;
            if (From == null || To == null)
            {
                var fy = this.GetFinancialYear();
                From = fy.Start;
                To = fy.End;

                ViewBag.From = From;
                ViewBag.To = To;
            }
            else
            {
                ViewBag.From = From;
                ViewBag.To = To;
            }
            var res = db.Sp_Get_ChDDPO("BankDraft", Status, From, To, Voucher_Type.BPV.ToString(), comp.Id).ToList();


            db.Sp_Add_Activity(userid, "Accessed Bank Drafts Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult PDCPayorder(string Status, DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);

            ViewBag.Status = Status;
            if (From == null || To == null)
            {
                var fy = this.GetFinancialYear();
                From = fy.Start;
                To = fy.End;

                ViewBag.From = From;
                ViewBag.To = To;
            }
            else
            {
                ViewBag.From = From;
                ViewBag.To = To;
            }
            var res = db.Sp_Get_ChDDPO("PayOrder", Status, From, To, Voucher_Type.BPV.ToString(), comp.Id).ToList();

            db.Sp_Add_Activity(userid, "Accessed Payorders Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult PDCCBPDetails(string No, DateTime Date, string Bank, string Status)
        {
            var res = db.Sp_Get_Cheque_BankDraft_PayOrder_MultiParamter(No, Status, Bank, Date, Voucher_Type.BPV.ToString()).ToList();
            ViewBag.Bank = db.Bank_Accounts.Select(x => new { x.Bank, x.Account_Number }).ToList();
            ViewBag.Reason = new SelectList(Nomenclature.DishoneredReason(), "Value", "Name");
            var paymentstatus = from PaymentMethodStatuses e in Enum.GetValues(typeof(PaymentMethodStatuses)) select new { Name = e.ToString() };
            ViewBag.Status = new SelectList(paymentstatus, "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed CBP Detail Page", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateInstPDC(string Status, DateTime? Clearance_Date, long Inst_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var chids = new XElement("ChPoDd", new XElement("Details",
                                   new XAttribute("Ids", Inst_Id)
                                  )).ToString();
            var Returnids = db.Sp_Get_ChDDPODetails_Multi(chids).ToList();
            foreach (var cbp in Returnids)
            {
                { // For Accounts Module 
                    AccountHandlerController ah = new AccountHandlerController();
                    var receipt = db.Vouchers.Where(x => x.Id == cbp.ReceiptId).FirstOrDefault(); //db.Sp_Get_Receipt_Parameter_ById(cbp.ReceiptId).FirstOrDefault();

                    if (Status == PaymentMethodStatuses.Approved.ToString())
                    {
                        var vouchno = "V-SAG-" + db.Sp_Get_ReceiptNo("SAG Voucher").FirstOrDefault();
                        ah.ApprovedInstrument_PDC(receipt.Transaction_Id, Status, userid, receipt.TokenParameter, cbp.Deposit_Bank_Acc_Number, vouchno);
                    }
                    if (Status == PaymentMethodStatuses.Dishonored.ToString())
                    {
                        ah.DishonerInstrument_PDC(receipt.Transaction_Id, Status, userid, receipt.TokenParameter);
                    }
                }

                var res = db.Sp_Update_Cheque_BankDraft_PayOrder(cbp.Id, Status, cbp.Bank, cbp.Deposit_Bank_Acc_Number, cbp.Module, cbp.Type, cbp.Reference_Id, Clearance_Date, cbp.Module_Id, cbp.Posted_Date, null).FirstOrDefault();
            }

            return Json(new Return { Status = true, Msg = "Updated" });
        }

        [NoDirectAccess] public ActionResult PDCSearchInstruments(string Type, string Status, DateTime? From, DateTime? To)
        {
            if (Type == "Cheque")
            {
                return RedirectToAction("PDCCheques", new { Status, From, To });
            }
            else if (Type == "Payorder")
            {
                return RedirectToAction("PDCPayorder", new { Status, From, To });
            }
            else if (Type == "BankDraft")
            {
                return RedirectToAction("PDCBankDrafts", new { Status, From, To });
            }
            else if (Type == "Online")
            {
                return RedirectToAction("PDCOnline", new { Status, From, To });
            }
            else if (Type == "DebitCredit")
            {
                return RedirectToAction("PDCDebitCredit", new { Status, From, To });
            }
            return PartialView();

        }
        //[NoDirectAccess] public ActionResult PDCOnline(string Status, DateTime? From, DateTime? To)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    AccountHandlerController ah = new AccountHandlerController();
        //    var comp = ah.Company_Attr(userid);
        //    ViewBag.Status = Status;
        //    if (From == null || To == null)
        //    {
        //        var fy = this.GetFinancialYear();
        //        From = fy.Start;
        //        To = fy.End;

        //        ViewBag.From = From;
        //        ViewBag.To = To;
        //    }
        //    else
        //    {
        //        ViewBag.From = From;
        //        ViewBag.To = To;
        //    }
        //    var res = db.Sp_Get_Online(Status, From, To, comp.Id).ToList();

        //    db.Sp_Add_Activity(userid, "Accessed Oline Payments Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
        //    return PartialView(res);
        //}
        //[NoDirectAccess] public ActionResult PDCDebitCredit(string Status, DateTime? From, DateTime? To)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    AccountHandlerController ah = new AccountHandlerController();
        //    var comp = ah.Company_Attr(userid);
        //    ViewBag.Status = Status;
        //    if (From == null || To == null)
        //    {
        //        var fy = this.GetFinancialYear();
        //        From = fy.Start;
        //        To = fy.End;

        //        ViewBag.From = From;
        //        ViewBag.To = To;
        //    }
        //    else
        //    {
        //        ViewBag.From = From;
        //        ViewBag.To = To;
        //    }
        //    var res = db.Sp_Get_ChDDPO("Debit / Credit Card", Status, From, To, comp.Id).ToList();

        //    db.Sp_Add_Activity(userid, "Accessed Debit / Credit Card Payments Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
        //    return PartialView(res);
        //}

    }
}