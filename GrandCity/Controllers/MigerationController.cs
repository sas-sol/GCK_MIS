using MeherEstateDevelopers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using Excel = Microsoft.Office.Interop.Excel;
using MeherEstateDevelopers.Models;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using MeherEstateDevelopers;
using Microsoft.AspNet.Identity;
using System.Globalization;
using MeherEstateDevelopers.Controllers;
using System.Threading;

namespace execldataimport.Controllers
{
    [LogAction]
    [Authorize]
    public class FiletransferController : Controller
    {
        public Grand_CityEntities db = new Grand_CityEntities();
        
        public void dn()
        {
            var a = db.Inventory_Stock_In.Where(x => x.GRN == "DN-I22-2").ToList();
            AccountHandlerController de = new AccountHandlerController();
            de.DNEntries(a, a.FirstOrDefault().Group_Id, a.FirstOrDefault().Created_By, a.Select(x => x.GRN).ToList());
        }
        
        public void gencan()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var date = new DateTime(2022, 8, 21);
            var voucher = db.Vouchers.Where(x => x.Userid == 20159 && x.DateTime >= date && x.PaymentType != "Cash").ToList();
            foreach (var item in voucher)
            {
                AccountHandlerController ah = new AccountHandlerController();
                var comp = ah.Company_Attr(userid);
                var credAccount = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.PDC_Payable.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
                var bank = db.Bank_Accounts.Where(x => x.Id == 10).FirstOrDefault();

                var file = db.File_Form.Where(x => x.Id == item.File_Plot_Id).FirstOrDefault();
                var res5 = db.Cancellation_Receipts.Where(x => x.File_Plot_No == item.File_Plot_Id && x.Module == Modules.FileManagement.ToString()).OrderByDescending(x => x.Id).FirstOrDefault();

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        string Sales = "";
                        string Receivable = "";
                        if (file.Type == PlotType.Commercial.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Commercial.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();

                        }
                        else if (file.Type == PlotType.Residential.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Residential.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                        }

                        var Sales_COA = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Sales, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, file.Block_Id);
                        var Plot_Head = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Receivable, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, file.Block_Id);
                        var Cancellation_COA = ah.HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId

                        decimal? Remain = res5.Plot_Total_Amount - res5.Amount;
                        decimal? Payable = res5.Amount - res5.Deduction_Amt;

                        var Transac = new Helpers().RandomNumber();
                        var Sales_Debit = db.Sp_Add_Journal_Entry(res5.Plot_Total_Amount, 0, Sales_COA.Text_ChartCode + " - " + Sales_COA.Head, Sales_COA.Id, Sales_COA.Text_ChartCode, Sales_COA.Head, item.Description, Transac, 1, userid, userid, null, bank.Bank, item.Ch_Pay_Draft_No, null, item.Ch_Pay_Draft_Date, null, "", null, item.VoucherNo, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var Pay_credit = db.Sp_Add_Journal_Entry(0, Payable, credAccount.Text_ChartCode + " - " + credAccount.Head, credAccount.Id, credAccount.Text_ChartCode, credAccount.Head, item.Description, Transac, 1, userid, userid, null, bank.Bank, item.Ch_Pay_Draft_No, null, item.Ch_Pay_Draft_Date, null, "", null, item.VoucherNo, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var plot_Credit = db.Sp_Add_Journal_Entry(0, Remain, Plot_Head.Text_ChartCode + " - " + Plot_Head.Head, Plot_Head.Id, Plot_Head.Text_ChartCode, Plot_Head.Head, item.Description, Transac, 1, userid, userid, null, bank.Bank, item.Ch_Pay_Draft_No, null, item.Ch_Pay_Draft_Date, null, "", null, item.VoucherNo, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var canc_Credit = db.Sp_Add_Journal_Entry(0, res5.Deduction_Amt, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, item.Description, Transac, 1, userid, userid, null, bank.Bank, item.Ch_Pay_Draft_No, null, item.Ch_Pay_Draft_Date, null, "", null, item.VoucherNo, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();





                        var res8 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(item.Amount, bank.Bank, null, item.PaymentType, bank.Bank, bank.Account_Number, PaymentMethodStatuses.Pending.ToString(),
                                Modules.Vendor_Payment.ToString(), "Payment Voucher", userid, item.Ch_Pay_Draft_No, item.File_Plot_Id, item.Ch_Pay_Draft_Date, file.FileFormNumber.ToString(), item.Id, comp.Id, Voucher_Type.BPV.ToString()).FirstOrDefault());

                        Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    }
                }


            }

        }
        
        public void miger()
        {
            var from = new DateTime(2021, 12, 1);
            var to = DateTime.Now;
            var res = db.SP_Reports_RegisterPlots_List(from, to).ToList();
            foreach (var item in res)
            {
                var a = db.Sale_Report.Any(x => x.Module_Id == item.Id && x.Module == item.Module);
                if (a)
                {
                    continue;
                }
                string Name = "", CNIC = "";
                try
                {
                    if (item.Module == Modules.PlotManagement.ToString())
                    {
                        var data = db.Sp_Get_PlotLastOwner(item.Id).ToList().ToList();
                        Name = string.Join(",", data.Select(x => x.Name));
                        CNIC = string.Join(",", data.Select(x => x.CNIC_NICOP));

                    }
                    else if (item.Module == Modules.FileManagement.ToString())
                    {
                        var data = db.Sp_Get_FileLastOwner(item.Id).ToList().ToList();
                        Name = string.Join(",", data.Select(x => x.Name));
                        CNIC = string.Join(",", data.Select(x => x.CNIC_NICOP));
                    }
                    else if (item.Module == Modules.CommercialManagement.ToString())
                    {
                        var data = db.Sp_Get_CommercialLastOwner(item.Id).ToList().ToList();
                        Name = string.Join(",", data.Select(x => x.Name));
                        CNIC = string.Join(",", data.Select(x => x.CNIC_NICOP));

                    }

                    var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == item.Id && x.Module == item.Module).FirstOrDefault();

                    if (bal == null)
                    {
                        Sale_Report s = new Sale_Report()
                        {
                            Block = item.Block,
                            Booking_Date = item.Owner_DateTime,
                            CNIC = CNIC,
                            Discount_Amt = 0,
                            Module = item.Module,
                            Module_Id = item.Id,
                            Name = Name,
                            Received_Amt = 0,
                            Sector = item.Sector,
                            Total_Amt = 0,
                            Type = item.Type,
                            Unit_No = item.Plot_No
                        };
                        db.Sale_Report.Add(s);
                        db.SaveChanges();
                    }
                    else
                    {
                        Sale_Report s = new Sale_Report()
                        {
                            Block = item.Block,
                            Booking_Date = item.Owner_DateTime,
                            CNIC = CNIC,
                            Discount_Amt = bal.TotalDiscounts,
                            Module = item.Module,
                            Module_Id = item.Id,
                            Name = Name,
                            Received_Amt = bal.Received_Amount,
                            Sector = item.Sector,
                            Total_Amt = bal.Total_Amount,
                            Type = item.Type,
                            Unit_No = item.Plot_No
                        };
                        db.Sale_Report.Add(s);
                        db.SaveChanges();
                    }
                   
                }
                catch (Exception ex)
                {

                }
            }

        }

        public void Openingbal()
        {
            var res = db.File_Form.Where(x => x.Status != 47).ToList();
            foreach (var item in res)
            {
                try
                {
                    var lastdate = new DateTime(2022, 8, 20);
                    var res1 = db.File_Installments.Where(x => x.File_Id == item.Id && x.Installment_Type != "2").ToList();
                    var res2 = db.Sp_Get_ReceivedAmounts(item.Id, Modules.FileManagement.ToString()).Where(x => (x.Status == null || x.Status == "Approved")).ToList();
                    var res3 = db.Discounts.Where(x => x.Module_Id == item.Id && x.Module == Modules.FileManagement.ToString()).ToList();
                    var res4 = db.Vouchers.Where(x => x.File_Plot_Id == item.Id && x.Module == Modules.FileManagement.ToString() && x.Cancel == null).ToList();
                    FileStatus filestat = (FileStatus)item.Status;
                    string status = filestat.ToString();

                    decimal? Amt_20 = res2.Where(x => (x.Status == null || x.Status == "Approved") && x.DateTime <= lastdate).Sum(x => x.Amount);
                    decimal? Total_Amt = res1.Sum(x => x.Amount);
                    decimal? Rec_Amt = res2.Sum(x => x.Amount);
                    decimal? Dis_Amt = res3.Sum(x => x.Discount_Amount);
                    var a = db.Sp_Add_Balance(item.Id, item.Block, status, Modules.FileManagement.ToString(), Total_Amt, Rec_Amt, Amt_20, Dis_Amt, res4.Sum(x => x.Amount));
                }
                catch (Exception e)
                {
                    FileStatus filestat = (FileStatus)item.Status;
                    string status = filestat.ToString();
                    var a = db.Sp_Add_Balance(item.Id, item.Block, status, Modules.FileManagement.ToString(), 0, 0, 0, 0, 0);

                }
            }
            var res111 = db.Plots.ToList();
            foreach (var item in res111)
            {
                try
                {
                    var lastdate = new DateTime(2022, 8, 20);
                    var res11 = db.Plot_Installments.Where(x => x.Plot_Id == item.Id && x.Installment_Type != "2").ToList();
                    var res12 = db.Sp_Get_ReceivedAmounts(item.Id, Modules.PlotManagement.ToString()).Where(x => (x.Status == null || x.Status == "Approved")).ToList();
                    var res13 = db.Discounts.Where(x => x.Module_Id == item.Id && x.Module == Modules.PlotManagement.ToString()).ToList();
                    var res14 = db.Vouchers.Where(x => x.File_Plot_Id == item.Id && x.Module == Modules.PlotManagement.ToString() && x.Cancel == null).ToList();

                    var res15 = res12.Where(x => (x.Status == null || x.Status == "Approved") && x.DateTime <= lastdate).Sum(x => x.Amount);
                    db.Sp_Add_Balance(item.Id, item.Block_Name, item.Status, Modules.PlotManagement.ToString(), res11.Sum(x => x.Amount), res12.Sum(x => x.Amount), res15, res13.Sum(x => x.Discount_Amount), res14.Sum(x => x.Amount));


                }
                catch (Exception e)
                {
                    var a = db.Sp_Add_Balance(item.Id, item.Block_Name, item.Status, Modules.PlotManagement.ToString(), 0, 0, 0, 0, 0);

                }
            }

        }
        
        public void createled()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "G22-5753" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                de.Receive_Plot_Amount(rd.Amount, rd.File_Plot_Number, rd.Plot_Type, rd.Block, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, long.Parse(rd.Transaction_Id), (long)rd.Userid, rd.ReceiptNo, 1, false, "Files_Plots", 1);
            }
        }
        
        public void createotherrece()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "C22-19" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                de.Other_Recovery((decimal)rd.Amount, rd.File_Plot_Number, rd.Plot_Type, rd.Block, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, rd.DevelopmentCharges, long.Parse(rd.Transaction_Id), (long)rd.Userid, rd.ReceiptNo, 1, "Other_Recovery", false, "Files_Plots");
            }
        }
        
        public void createothefine()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "C22-75" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                de.Other_Recovery((decimal)rd.Amount, rd.File_Plot_Number, rd.Plot_Type, rd.Block, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, rd.DevelopmentCharges, long.Parse(rd.Transaction_Id), (long)rd.Userid, rd.ReceiptNo, 1, "Fines_And_Penalties", false, "Files_Plots");
            }
        }
        
        public void createarchi()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "C22-5029" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                de.Receipts_Architecture_Receipt((decimal)rd.Amount, rd.Name, rd.Text, rd.Contact, "Cash", null, null, null, "Received Architecture Fee", long.Parse(rd.Transaction_Id), (long)rd.Userid, rd.ReceiptNo, 1, false);
            }
        }
        
        public void createnew()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "C22-3167" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                de.Other_Recovery((decimal)rd.Amount, rd.File_Plot_Number, rd.Plot_Type, rd.Block, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, rd.DevelopmentCharges, long.Parse(rd.Transaction_Id), (long)rd.Userid, rd.ReceiptNo, 1, "New_Connection_Charges", false, "Files_Plots");
            }
        }
        
        public void createserv()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "F22-9501" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                var blk = db.RealEstate_Blocks.Where(x => x.Block_Name == rd.Block).FirstOrDefault();
                var res5 = de.Service_Charges((decimal)rd.Amount, rd.File_Plot_Number, rd.Block, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, rd.Branch_Name, "", long.Parse(rd.Transaction_Id), (long)rd.Userid, rd.ReceiptNo, 1, false, "Files_Plots", blk.Id);
            }
        }
        
        public void createelec()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "C22-4706" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                var blk = db.RealEstate_Blocks.Where(x => x.Block_Name == rd.Block).FirstOrDefault();
                var res5 = de.Service_Charges_Electricity((decimal)rd.Amount, rd.File_Plot_Number, rd.Block, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, "", long.Parse(rd.Transaction_Id), (long)rd.Userid, rd.ReceiptNo, 1, false, "Files_Plots", blk.Id);
            }
        }
        
        public void createprem()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "C22-1552" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).OrderBy(x => x.DateTime).ToList();
            foreach (var rd in res)
            {
                de.Receive_Plot_Amount(rd.Amount, rd.File_Plot_Number, rd.Plot_Type, rd.Block, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, long.Parse(rd.Transaction_Id), (long)rd.Userid, rd.ReceiptNo, 1, false, "Commercial", 3);
            }
        }

        public void createmem()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "C22-3086", "C22-3087", "C22-3088" };
            var res = db.Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                var res1 = de.Admin_Facilities_FeeReceived((decimal)rd.Amount, rd.Name, rd.Contact, "Gym", long.Parse(rd.Transaction_Id), (long)rd.Userid, 1, rd.ReceiptNo, false);
            }
        }

        public void creatsam()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "SAM-F22-8", "SAM-F22-9" };
            var res = db.SAM_Receipts.Where(x => no.Contains(x.ReceiptNo)).ToList();
            foreach (var rd in res)
            {
                de.SAM_Installment(rd.Amount, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, (long)rd.Reference, 1, rd.ReceiptNo, 1, rd.Reference.ToString(), false);
            }
        }
        
        public void creatsampay()
        {
            AccountHandlerController de = new AccountHandlerController();
            string[] no = { "SAM-V-F22-10" };
            var res = db.SAM_Voucher.Where(x => no.Contains(x.VoucherNo)).ToList();
            var trans = new Helpers().RandomNumber();
            foreach (var rd in res)
            {
                de.SAM_Vouchers(rd.Amount, rd.PaymentType, rd.Ch_Pay_Draft_No, rd.Ch_Pay_Draft_Date, rd.Bank, trans, 1, rd.VoucherNo, 1, trans.ToString(), false);
            }
        }
        
        public void Proj()
        {
            var prj = db.Projects.ToList();
            foreach (var item in prj)
            {
                if (item.Head_Code == null)
                {
                    continue;
                }

                var code = "/1" + item.Head_Code;
                var headcode = db.Sp_Get_COA_Head_Code(code).FirstOrDefault();

                AccountHandlerController ah = new AccountHandlerController();
                ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
                    COA_Mapper_Modules.Projects.ToString(), COA_Mapper_ModuleTypes.Projects_List.ToString(), item.Id, headcode.Id, 1);
            }
        }

        //public void PlotsUpdation()
        //{
        //    Excel.Application xlApp = new Excel.Application();
        //    Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"D:\Refined_COA.xlsx");
        //    Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
        //    Excel.Range xlRange = xlWorksheet.UsedRange;
        //    int rowCount = xlRange.Rows.Count;
        //    int colCount = xlRange.Columns.Count;
        //    for (int i = 2; i <= rowCount; i++)
        //    {
        //        string Code = Convert.ToString(xlRange.Cells[i, 1].Value2);
        //        string Head = Convert.ToString(xlRange.Cells[i, 2].Value2);

        //        //this.ParentAccount(Code, Head);
        //    }
        //}

        //public void InvMap()
        //{

        //    Excel.Application xlApp = new Excel.Application();
        //    Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"D:\Item.xlsx");
        //    Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
        //    Excel.Range xlRange = xlWorksheet.UsedRange;
        //    int rowCount = xlRange.Rows.Count;
        //    int colCount = xlRange.Columns.Count;
        //    for (int i = 2; i <= rowCount; i++)
        //    {
        //        string sku = Convert.ToString(xlRange.Cells[i, 1].Value2);
        //        string head = Convert.ToString(xlRange.Cells[i, 2].Value2);

        //        var item = db.Inventories.Where(x => x.SKU == sku).FirstOrDefault();
        //        var headcode = db.Sp_Get_COA_Head_Code("/1" + head + "/").FirstOrDefault();

        //        var a = db.COA_Mapper.Any(x => x.Module == COA_Mapper_Modules.Procurement.ToString() &&
        //                    x.ModuleType == COA_Mapper_ModuleTypes.Item_List.ToString() && x.Module_Id == item.Id);
        //        if (a)
        //        {
        //            continue;
        //        }

        //        AccountHandlerController ah = new AccountHandlerController();
        //        ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
        //            COA_Mapper_Modules.Procurement.ToString(), COA_Mapper_ModuleTypes.Item_List.ToString(), item.Id, headcode.Id, 1);

        //    }
        //}

        public void updaste()
        {
            var a = db.Salaries.Where(x => x.Stipend != 0).ToList();
            foreach (var item in a)
            {
                var Design = db.Sp_Get_EmployeeFirstDesignation(item.Emp_Id).ToList();
                var DR = Design.FirstOrDefault();
                var sec_desg = Design.OrderByDescending(x => x.Designation_Id).FirstOrDefault();
                db.Sp_Update_SalariesCurrentMonthDesignation(item.Emp_Id, DR.DepartmentId, DR.DepartmentName, DR.DesignationId, DR.DesignationName, DR.CompanyId, DR.CompanyName, sec_desg.DepartmentId, sec_desg.DepartmentName, sec_desg.DesignationName);
            }
        }
        
        public void extraentry()
        {
            var res = db.Test_FileBalance().ToList();
            foreach (var item in res)
            {
                db.Sp_Add_FileComments(item.Id, "Plot is Verified (Systemetically)", 20040, ActivityType.Plot_Verified.ToString());
            }
        }
        
        public void Update()
        {
            var res = db.Inventory_Stock_In.Where(x => x.GRN.Contains("DN")).ToList();

            foreach (var item in res)
            {
                Inventory_Stock_Out io = new Inventory_Stock_Out()
                {
                    Item_Id = item.Item_Id,
                    Group_Id = item.Group_Id,
                    Qty = item.Qty,
                    Prj_Id = item.Prj,
                    Prj_Name = item.Prj_Name,
                    PO_Id = item.PO_Id,
                    PO_Number = item.PO_Number,
                    IssueNote_No = item.GRN,
                    DemandOrder_Id = item.Group_Id,
                    Dep_Id = item.Dep_Id,
                    Dep_Name = item.Dep_Name,
                    IssuedBy_Date = item.DateTime,
                };
                db.Inventory_Stock_Out.Add(io);
                db.SaveChanges();
            }
        }

        public void msg()
        {
            string a = "Dear Customer,\n" +
                        "Meher Estate Developers presents you a golden investment opportunity. Now get your own 5 Marla plot in Badar D Block with immediate possession for as low as Rs. 20 lakh with No development charges and No other hidden costs.\n" +
                        "For further details call us now at 042 - 111 724 786\n" +
                        "Best Regards,\n" +
                        "Meher Estate Developers.";

            var file = db.Files_Transfer.Select(x => x.Mobile_1).ToList();
            var file_nom = db.Files_Transfer.Select(x => x.Mobile_2).ToList();
            var plot = db.Plot_Ownership.Select(x => x.Mobile_1).ToList();
            var plot_nom = db.Plot_Ownership.Select(x => x.Mobile_2).ToList();
            var com = db.Commercial_Room_Transfer.Select(x => x.Mobile_1).ToList();
            var com_nom = db.Commercial_Room_Transfer.Select(x => x.Mobile_2).ToList();

            file.AddRange(file_nom);
            file.AddRange(plot);
            file.AddRange(com);
            file.AddRange(plot_nom);
            file.AddRange(com_nom);

            file = file.Distinct().ToList();
            foreach (var dea in file)
            {
                try
                {
                    SmsService smsService = new SmsService();
                    smsService.Broadcast(a, dea);
                    //db.Sp_Add_FileComments(dea.Id, msgtext, userid, "Text");
                }
                catch (Exception ee)
                {
                }
            }
        }

        public void QR_Codes(long Id)
        {
            Helpers helpers = new Helpers(Modules.PlotManagement, Types.Plots);

            object[] data = new object[6];
            data[0] = "195";
            data[1] = "Sohail";
            data[2] = "Residential";
            data[3] = "25.00 x 45.00";
            data[4] = "5 Marla";
            var QR_Data = helpers.GenerateQRCode(data);
        }

        public void plots()
        {
            var res = db.Plots.Where(x => x.Status == "Registered" || x.Status == "Temporary_Cancelled").Select(x => x.Id).ToList();
            foreach (var item in res)
            {
                db.Sp_Update_CurrentOwner(item);
            }
        }

        public void AdjustIntallments()
        {
            FileSystemController f = new FileSystemController();
            f.UpdateFileBalances();

            PlotsController p = new PlotsController();
            p.UpdatePlotBalances();

            CommercialController c = new CommercialController();
            c.UpdateCommercialUnitsBalance();

        }
        
        public void UpdateInstallment(long FileId)
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

        public void GroupTag()
        {
            var res = db.Plot_Ownership.ToList();
            foreach (var item in res)
            {
                Helpers h = new Helpers();
                var r = db.Test_UpdateOldPlotsSher("1", h.RandomNumber().ToString(), item.Id);
            }
        }
        
        public void TransferGroupTag()
        {
            var res = db.Plots_Transfer_Request.ToList();
            foreach (var item in res)
            {
                Helpers h = new Helpers();
                var r = db.Test_UpdateOldPlotsSher("2", h.RandomNumber().ToString(), item.Id);
            }
        }
        
        public void Install()
        {
            var res = db.File_Form.Where(x => x.Status == 1 && x.Type == "Commercial" && x.Plot_Size == "2 Marla").ToList();
            foreach (var filedata in res)
            {
                InstallmentsController ic = new InstallmentsController();
                List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(filedata.Id).ToList();
                var res1 = db.Files_Transfer.Where(x => x.File_Form_Id == filedata.Id).FirstOrDefault();
                var file_Installments = ic.AddInstallmentPlan(installmentstructure, "", Convert.ToInt64(filedata.Id), Convert.ToDateTime(res1.DateTime));
            }
        }
        
        public void Updateinst()
        {
            var res = db.BallotPlots.Where(x => (x.Status == "Balloted") && (x.Sector == "EE" || x.Sector == "FF")).ToList();

            foreach (var v in res)
            {
                var file_inst = db.File_Installments.Where(x => x.File_Id == v.FileId).ToList();
                if (!file_inst.Any())
                {
                    continue;
                }
                var ttlamt = file_inst.Sum(x => x.Amount);
                if (v.PlotArea == 3)
                {

                    var baseval = 600000;
                    var remamt = baseval - ttlamt;


                }
                else if (v.PlotArea == 5)
                {
                    var baseval = 1000000;
                    var remamt = baseval - ttlamt;

                    if (remamt != 0)
                    {
                        File_Installments lastinst = new File_Installments();
                        lastinst = file_inst.Where(x => x.Installment_Type == "1").OrderByDescending(x => x.Due_Date).FirstOrDefault();
                        lastinst.Amount += remamt;
                        db.File_Installments.Attach(lastinst);
                        db.Entry(lastinst).Property(x => x.Amount).IsModified = true;
                        db.SaveChanges();
                    }
                }
                else if (v.PlotArea == 8)
                {
                    var baseval = 1600000;
                    var remamt = baseval - ttlamt;

                    if (remamt != 0)
                    {
                        File_Installments lastinst = new File_Installments();
                        lastinst = file_inst.Where(x => x.Installment_Type == "1").OrderByDescending(x => x.Due_Date).FirstOrDefault();
                        lastinst.Amount += remamt;
                        db.File_Installments.Attach(lastinst);
                        db.Entry(lastinst).Property(x => x.Amount).IsModified = true;
                        db.SaveChanges();
                    }
                }
                else if (v.PlotArea == 10)
                {
                    var baseval = 2000000;
                    var remamt = baseval - ttlamt;

                    if (remamt != 0)
                    {
                        File_Installments lastinst = new File_Installments();
                        lastinst = file_inst.Where(x => x.Installment_Type == "1").OrderByDescending(x => x.Due_Date).FirstOrDefault();
                        lastinst.Amount += remamt;
                        db.File_Installments.Attach(lastinst);
                        db.Entry(lastinst).Property(x => x.Amount).IsModified = true;
                        db.SaveChanges();
                    }
                }

                //FileSystemController fc = new FileSystemController();
                //fc.TestAdjustIntallments(long.Parse(v.BallotFile));

                //var retDat = db.Sp_Update_SpecialPrefCharge(v.Id, 0);
            }
        }
        
        public void AddPlotsToInventory()
        {
            var ballot_leftovers = db.BallotPlots.Where(x => x.Status == "Hold").ToList();
            foreach (var v in ballot_leftovers)
            {
                decimal front = 0;
                decimal back = 0;
                decimal left = 0;
                decimal right = 0;
                var blk = db.RealEstate_Blocks.Where(x => x.Block_Name == v.Block).FirstOrDefault();
                var phase = db.RealEstate_Phases.Where(x => x.Id == blk.Phase_Id).FirstOrDefault();
                if (v.PlotSize.Contains('F'))
                {
                    string[] _temp = v.PlotSize.Split('-');
                    foreach (string s in _temp)
                    {
                        if (s.Contains('F'))
                        {
                            front = Convert.ToDecimal(s.Split(' ')[0]);
                        }
                        else if (s.Contains('B'))
                        {
                            back = Convert.ToDecimal(s.Split(' ')[1]);
                        }
                        else if (s.Contains('L'))
                        {
                            left = Convert.ToDecimal(s.Split(' ')[1]);
                        }
                        else if (s.Contains('R'))
                        {
                            right = Convert.ToDecimal(s.Split(' ')[1]);
                        }
                    }
                }
                else if (v.PlotSize.Contains('x'))
                {
                    string[] _temp = v.PlotSize.Split('x');
                    front = back = Convert.ToDecimal(_temp[0].Split(' ')[0]);
                    left = right = Convert.ToDecimal(_temp[1].Split(' ')[1]);
                }
                var res = db.Plots_Category.Where(x => x.Front_Side == front && x.Back_Side == back && x.Left_Side == left && x.Right_Side == right).FirstOrDefault();
                if (res is null)
                {
                    try
                    {
                        Plots_Category pc = new Plots_Category
                        {
                            Front_Side = front,
                            Back_Side = back,
                            Left_Side = left,
                            Right_Side = right,
                            Category = v.PlotArea + " Marla",
                            Uom = "sqft"
                        };
                        db.Plots_Category.Add(pc);
                        db.SaveChanges();
                        Plot p = new Plot
                        {
                            Block_Id = blk.Id,
                            Phase_Id = phase.Id,
                            Plot_Category = pc.Id,
                            Hold = true,
                            Plot_Location = v.PlotType,
                            Plot_Number = v.PlotNo,
                            Sector = v.Sector,
                            Plot_Size = v.PlotArea + " Marla",
                            Road_Type = v.Road,
                            Block_Name = blk.Block_Name,
                            Develop_Status = "Non Constructed",
                            Verified = false,
                            Status = "Hold",
                            Type = "Residential"
                        };
                        db.Plots.Add(p);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string ssssss = ex.Message;
                    }
                }
                else
                {
                    Plot p = new Plot
                    {
                        Block_Id = blk.Id,
                        Phase_Id = phase.Id,
                        Plot_Category = res.Id,
                        Hold = true,
                        Plot_Location = v.PlotType,
                        Plot_Number = v.PlotNo,
                        Sector = v.Sector,
                        Plot_Size = v.PlotArea + " Marla",
                        Road_Type = v.Road,
                        Block_Name = blk.Block_Name,
                        Develop_Status = "Non Constructed",
                        Verified = false,
                        Status = "Hold",
                        Type = "Residential"
                    };
                    db.Plots.Add(p);
                    db.SaveChanges();
                }
                v.LetterB = true;
                v.LetterB_Date = DateTime.Now;
                db.BallotPlots.Attach(v);
                db.Entry(v).Property(x => x.LetterB).IsModified = true;
                db.Entry(v).Property(x => x.LetterB_Date).IsModified = true;
                db.SaveChanges();
            }
        }
    }
    public class Temp
    {
        public Receipt r { get; set; }
    }
}


// Archive
//public void ImportChartAccount()
//{
//    Excel.Application xlApp = new Excel.Application();
//    Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"D:\3.xlsx");
//    Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
//    Excel.Range xlRange = xlWorksheet.UsedRange;
//    int rowCount = xlRange.Rows.Count;
//    int colCount = xlRange.Columns.Count;
//    for (int i = 3; i <= rowCount; i++)
//    {
//        try
//        {
//            string item = Convert.ToString(xlRange.Cells[i, 1].Value2);
//            string cat = Convert.ToString(xlRange.Cells[i, 2].Value2);
//            string uom = Convert.ToString(xlRange.Cells[i, 3].Value2);
//            string len = Convert.ToString(xlRange.Cells[i, 4].Value2);
//            string l_uom = Convert.ToString(xlRange.Cells[i, 5].Value2);
//            string width = Convert.ToString(xlRange.Cells[i, 6].Value2);
//            string w_uom = Convert.ToString(xlRange.Cells[i, 7].Value2);
//            string h = Convert.ToString(xlRange.Cells[i, 8].Value2);
//            string h_uom = Convert.ToString(xlRange.Cells[i, 9].Value2);
//            string dia = Convert.ToString(xlRange.Cells[i, 10].Value2);
//            string dia_uom = Convert.ToString(xlRange.Cells[i, 11].Value2);
//            string size = Convert.ToString(xlRange.Cells[i, 12].Value2);
//            string s_uom = Convert.ToString(xlRange.Cells[i, 13].Value2);
//            string packing = Convert.ToString(xlRange.Cells[i, 14].Value2);
//            string ware = Convert.ToString(xlRange.Cells[i, 15].Value2);
//            string dep = Convert.ToString(xlRange.Cells[i, 16].Value2);
//            string depname = Convert.ToString(xlRange.Cells[i, 17].Value2);
//            string remar = Convert.ToString(xlRange.Cells[i, 18].Value2);
//            string avail = Convert.ToString(xlRange.Cells[i, 19].Value2);
//            string coma = Convert.ToString(xlRange.Cells[i, 20].Value2);

//            TextInfo text = new CultureInfo("en-us", false).TextInfo;

//            Inventory a = new Inventory();
//            a.Company = coma;
//            a.Item_Name = (string.IsNullOrEmpty(item)) ? item : text.ToTitleCase(item);
//            a.Category_Id = Convert.ToInt16(cat);
//            a.UOM = uom;
//            a.Length = Convert.ToDecimal(len);
//            a.L_UOM = l_uom;
//            a.Width = Convert.ToDecimal(width);
//            a.W_UOM = w_uom;
//            a.Heigth = Convert.ToDecimal(h);
//            a.H_UOM = h_uom;
//            a.Diameter = Convert.ToDecimal(dia);
//            a.D_UOM = dia_uom;
//            a.Size = size;
//            a.Size_UOM = s_uom;
//            a.Packing_Size = Convert.ToDecimal(packing);
//            a.DateTime = DateTime.Now;
//            a.Minimum_Qty = 0;
//            a.Specifications = remar;
//            var res2 = db.Inventories.FirstOrDefault(x => x.Company == a.Company && x.Item_Name == a.Item_Name && x.Length == a.Length && x.L_UOM == a.L_UOM && x.Width == a.Width && x.W_UOM == a.W_UOM && x.Heigth == a.Heigth && x.H_UOM == a.H_UOM && x.Diameter == a.Diameter && x.D_UOM == a.D_UOM && x.Size == a.Size && x.Size_UOM == a.Size_UOM);
//            //var res2 = db.Inventories.FirstOrDefault(x => x.Company == a.Company && x.Item_Name == a.Item_Name && x.Length == a.Length && x.L_UOM == a.L_UOM && x.Width == a.Width && x.W_UOM == a.W_UOM && x.Heigth == a.Heigth && x.H_UOM == a.H_UOM /*&& x.Diameter == a.Diameter && x.D_UOM == a.D_UOM && x.Size == a.Size && x.Size_UOM == a.Size_UOM*/);
//            //var res2 = db.Inventories.FirstOrDefault(x => x.Company == a.Company &&  a.Item_Name.Contains(x.Item_Name) && x.Diameter == a.Diameter && x.D_UOM == a.D_UOM /*&& x.Length == a.Length && x.L_UOM == a.L_UOM && x.Width == a.Width && x.W_UOM == a.W_UOM && x.Heigth == a.Heigth && x.H_UOM == a.H_UOM && x.Size == a.Size && x.Size_UOM == a.Size_UOM*/);
//            if (res2 == null)
//            {
//                db.Inventories.Add(a);
//                db.SaveChanges();
//            }
//            else
//            {
//                a.Id = res2.Id;
//            }

//            Helpers help = new Helpers();
//            Inventory_Stock_In isi = new Inventory_Stock_In();
//            isi.Created_By = 1;
//            isi.DateTime = DateTime.Now;
//            isi.Dep_Id = Convert.ToInt32(dep);
//            isi.Dep_Name = depname;
//            isi.Group_Id = help.RandomNumber();
//            isi.Item_Id = a.Id;
//            isi.Item_name = a.Item_Name;
//            isi.Qty = Convert.ToDecimal(avail);
//            isi.Rate = null;

//            db.Inventory_Stock_In.Add(isi);
//            db.SaveChanges();

//            Inventory_Location il = new Inventory_Location();
//            il.Item_Id = a.Id;
//            il.Item_Name = a.Item_Name;
//            il.WarehouseId = Convert.ToInt32(ware);
//            il.Qty = isi.Qty;
//            il.Stock_In_Id = isi.Id;

//            db.Inventory_Location.Add(il);
//            db.SaveChanges();
//        }
//        catch (Exception e)
//        {
//            continue;

//        }


//    }
//}
//public void ParentAccount(string Code, string Head)
//{
//    var allchart = db.Sp_Get_CA_Head().ToList();
//    var c = Array.ConvertAll(Code.Split('-'), int.Parse);
//    string nc = "";
//    if (c[0] > 0 && c[1] == 0 && c[2] == 0 && c[3] == 0)
//    {
//        nc = "/" + c[0] + "/";
//        if (!allchart.Any(x => x.Text_ChartCode == nc))
//        {
//            db.Sp_Add_CA_Head(1, Head, null, null);
//        }
//    }
//    else if (c[0] > 0 && c[1] > 0 && c[2] == 0 && c[3] == 0)
//    {
//        nc = "/" + c[0] + "/" + c[1] + "/";
//        if (!allchart.Any(x => x.Text_ChartCode == nc))
//        {
//            string Parentcode = "/" + c[0] + "/";
//            var parent = allchart.Where(x => x.Text_ChartCode == Parentcode).SingleOrDefault();
//            db.Sp_Add_CA_Head(parent.Id, Head, null, null);
//        }
//    }
//    else if (c[0] > 0 && c[1] > 0 && c[2] > 0 && c[3] == 0)
//    {
//        nc = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/";
//        if (!allchart.Any(x => x.Text_ChartCode == nc))
//        {
//            string Parentcode = "/" + c[0] + "/" + c[1] + "/";
//            var parent = allchart.Where(x => x.Text_ChartCode == Parentcode).SingleOrDefault();
//            db.Sp_Add_CA_Head(parent.Id, Head, null, null);
//        }
//    }
//    else if (c[0] > 0 && c[1] > 0 && c[2] > 0 && c[3] > 0)
//    {
//        nc = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/" + c[3] + "/";
//        if (!allchart.Any(x => x.Text_ChartCode == nc))
//        {
//            string Parentcode = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/";
//            var parent = allchart.Where(x => x.Text_ChartCode == Parentcode).SingleOrDefault();
//            db.Sp_Add_CA_Head(parent.Id, Head, null, null);
//        }
//    }


//    //var b = Code.Substring(0, Code.IndexOf('-'));
//    //var c = Code.Substring(Code.IndexOf('-') + 1);

//    //if ( c[1] > 0 && c[2] > 0 && c[3] > 0)
//    //{

//    //}



//}
//public void UpdatePlot()
//{
//    var res = db.PlotToMoves.ToList();
//    foreach (var item in res)
//    {
//        var plot = db.Plots.Where(x => x.Plot_Number == item.Plot_No && x.Sector == item.Sector && x.Block_Id == item.Block && x.Type == item.Type).SingleOrDefault();
//        if (plot == null)
//        {
//            var str = item.Dimension.Split(new char[] { 'x', 'X' });
//            var multisi = str[0].Split(':');
//            if (multisi.Count() == 1)
//            {
//                decimal front = decimal.Parse(str[0]);
//                decimal side = decimal.Parse(str[1]);
//                var res1 = db.Plots_Category.Where(x => x.Front_Side == front && x.Left_Side == side).FirstOrDefault();
//                if (res1 == null)
//                {
//                    Plots_Category p = new Plots_Category()
//                    {
//                        Back_Side = front,
//                        Category = "marla",
//                        Front_Side = front,
//                        Left_Side = side,
//                        Right_Side = side,
//                        Uom = "Sq Ft"
//                    };
//                    db.Plots_Category.Add(p);
//                    db.SaveChanges();
//                    Plot pl = new Plot()
//                    {
//                        Plot_Number = item.Plot_No,
//                        Sector = item.Sector,
//                        Block_Id = Convert.ToInt64(item.Block),
//                        Type = item.Type,
//                        Phase_Id = 2,
//                        Plot_Category = p.Id,
//                        Plot_Size = item.Plot_Size + " Marla",
//                        Develop_Status = PlotDevelopStatus.Non_Constructed.ToString(),
//                        Plot_Location = item.Location,
//                        Road_Type = item.Road
//                    };
//                    db.Plots.Add(pl);
//                    db.SaveChanges();
//                }
//                else
//                {
//                    Plot pl = new Plot()
//                    {
//                        Plot_Number = item.Plot_No,
//                        Block_Id = Convert.ToInt64(item.Block),
//                        Type = item.Type,
//                        Sector = item.Sector,
//                        Phase_Id = 2,
//                        Plot_Category = res1.Id,
//                        Plot_Size = item.Plot_Size + " Marla",
//                        Develop_Status = PlotDevelopStatus.Non_Constructed.ToString(),
//                        Plot_Location = item.Location,
//                        Road_Type = item.Road
//                    };
//                    db.Plots.Add(pl);
//                    db.SaveChanges();
//                }
//            }
//            else
//            {

//                decimal front = decimal.Parse(str[0].Split(':')[1]);
//                decimal b = decimal.Parse(str[1].Split(':')[1]);
//                decimal l = decimal.Parse(str[2].Split(':')[1]);
//                decimal r = decimal.Parse(str[3].Split(':')[1]);
//                var res1 = db.Plots_Category.Where(x => x.Front_Side == front && x.Back_Side == b && x.Left_Side == l && x.Right_Side == r).FirstOrDefault();
//                if (res1 == null)
//                {
//                    Plots_Category p = new Plots_Category()
//                    {
//                        Back_Side = b,
//                        Category = "marla",
//                        Front_Side = front,
//                        Left_Side = l,
//                        Right_Side = r,
//                        Uom = "Sq Ft"
//                    };
//                    db.Plots_Category.Add(p);
//                    db.SaveChanges();
//                    Plot pl = new Plot()
//                    {
//                        Plot_Number = item.Plot_No,
//                        Block_Id = Convert.ToInt64(item.Block),
//                        Type = item.Type,
//                        Sector = item.Sector,
//                        Phase_Id = 2,
//                        Plot_Category = p.Id,
//                        Plot_Size = item.Plot_Size + " Marla",
//                        Develop_Status = PlotDevelopStatus.Non_Constructed.ToString(),
//                        Plot_Location = item.Location,
//                        Road_Type = item.Road,
//                        Status = PlotsStatus.Available_For_Sale.ToString()
//                    };
//                    db.Plots.Add(pl);
//                    db.SaveChanges();
//                }
//                else
//                {
//                    Plot pl = new Plot()
//                    {
//                        Plot_Number = item.Plot_No,
//                        Block_Id = Convert.ToInt64(item.Block),
//                        Type = item.Type,
//                        Phase_Id = 2,
//                        Sector = item.Sector,
//                        Plot_Category = res1.Id,
//                        Plot_Size = item.Plot_Size + " Marla",
//                        Develop_Status = PlotDevelopStatus.Non_Constructed.ToString(),
//                        Plot_Location = item.Location,
//                        Road_Type = item.Road,
//                        Status = PlotsStatus.Available_For_Sale.ToString()
//                    };
//                    db.Plots.Add(pl);
//                    db.SaveChanges();
//                }
//            }

//        }
//        else
//        {
//            var str = item.Dimension.Split(new char[] { 'x', 'X' });
//            var multisi = str[0].Split(':');
//            if (multisi.Count() == 1)
//            {
//                decimal front = decimal.Parse(str[0]);
//                decimal side = decimal.Parse(str[1]);
//                var res1 = db.Plots_Category.Where(x => x.Front_Side == front && x.Left_Side == side).FirstOrDefault();
//                if (res1 == null)
//                {
//                    Plots_Category p = new Plots_Category()
//                    {
//                        Back_Side = front,
//                        Category = "marla",
//                        Front_Side = front,
//                        Left_Side = side,
//                        Right_Side = side,
//                        Uom = "Sq Ft"
//                    };
//                    db.Plots_Category.Add(p);
//                    db.SaveChanges();
//                    db.Plot_update_Plotdata(plot.Id, item.Location, p.Id, item.Road, item.Plot_Size + " Marla");


//                }
//                else
//                {
//                    db.Plot_update_Plotdata(plot.Id, item.Location, res1.Id, item.Road, item.Plot_Size + " Marla");
//                }
//            }
//            else
//            {
//                decimal front = decimal.Parse(str[0].Split(':')[1]);
//                decimal b = decimal.Parse(str[1].Split(':')[1]);
//                decimal l = decimal.Parse(str[2].Split(':')[1]);
//                decimal r = decimal.Parse(str[3].Split(':')[1]);
//                var res1 = db.Plots_Category.Where(x => x.Front_Side == front && x.Back_Side == b && x.Left_Side == l && x.Right_Side == r).FirstOrDefault();
//                if (res1 == null)
//                {
//                    Plots_Category p = new Plots_Category()
//                    {
//                        Back_Side = b,
//                        Category = "marla",
//                        Front_Side = front,
//                        Left_Side = l,
//                        Right_Side = r,
//                        Uom = "Sq Ft"
//                    };
//                    db.Plots_Category.Add(p);
//                    db.SaveChanges();
//                    db.Plot_update_Plotdata(plot.Id, item.Location, p.Id, item.Road, item.Plot_Size + " Marla");


//                }
//                else
//                {
//                    db.Plot_update_Plotdata(plot.Id, item.Location, res1.Id, item.Road, item.Plot_Size + " Marla");
//                }
//            }
//        }
//    }
//}
//public void Receipts()
//{
//    var res = db.PlotToMoves.ToList();
//    List<Receipt> rr = new List<Receipt>();
//    int count = 4686;
//    foreach (var item in res)
//    {
//        Receipt r = new Receipt()
//        {
//            ReceiptNo = "DIB-A20-" + count ,
//            Amount = Convert.ToDecimal(item.Location) ,
//            Name = item.Dimension ,
//            File_Plot_No = Convert.ToInt64(item.Plot_No) ,
//            File_Plot_Number = item.Plot_No,
//            Module = Modules.FileManagement.ToString(),
//            Block = "Blocks",
//            Size = item.Plot_Size,
//            PaymentType = "Online_Cash",
//            DateTime = Convert.ToDateTime(item.Road)
//        };
//        rr.Add(r);
//        count++;
//    }
//    db.Receipts.AddRange(rr);
//    db.SaveChanges();
//}

//public void test()
//{
//    var text = "Dear MUHAMMAD OMAR NAEEM ,\n\r" +
//                "A Payment of Rs 34,000 has been received against Cheque No: 30071582 Bank: Bank Alfalah for File number 4908918  on 29-Feb-2020 and has sent for clearing";

//        SmsService smsService = new SmsService();
//        smsService.SendMsg(text, "03214332803");
//}
//public void Plotaddition()
//{
//    Excel.Application xlApp = new Excel.Application();
//    Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"D:\Documents\DIB3.xlsx");
//    Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
//    Excel.Range xlRange = xlWorksheet.UsedRange;
//    int rowCount = xlRange.Rows.Count;
//    int colCount = xlRange.Columns.Count;
//    List<PlotToMove> pp = new List<PlotToMove>();
//    for (int i = 1; i <= rowCount; i++)
//    {
//        string file = Convert.ToString(xlRange.Cells[i, 1].Value2);
//        string name = Convert.ToString(xlRange.Cells[i, 2].Value2);
//        string size = Convert.ToString(xlRange.Cells[i, 3].Value2);
//        string amount = Convert.ToString(xlRange.Cells[i, 4].Value2);

//        PlotToMove p = new PlotToMove() {
//            Plot_No = file,
//            Dimension = name,
//            Plot_Size = size,
//            Location = amount,
//            Road = "2020-1-30"
//        };
//        pp.Add(p);
//    }
//    db.PlotToMoves.AddRange(pp);
//    db.SaveChanges();
//    xlApp.Workbooks.Close();
//}
//public void Desgination()
//{
//    Excel.Application xlApp = new Excel.Application();
//    Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"D:\Documents\Emp.xlsx");
//    Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
//    Excel.Range xlRange = xlWorksheet.UsedRange;
//    int rowCount = xlRange.Rows.Count;
//    int colCount = xlRange.Columns.Count;

//    for (int i = 1; i <= rowCount; i++)
//    {
//        string name = Convert.ToString(xlRange.Cells[i, 1].Value2);
//        string level = Convert.ToString(xlRange.Cells[i, 2].Value2);

//        var n = name.Split(',');
//        foreach (var item in n)
//        {
//            Comp_Dep_Desig nw = new Comp_Dep_Desig()
//            {
//                Name = item,
//                Description = " ",
//                Parent_Id = null,
//                Type = "Designation",
//                Level = Convert.ToInt16(level)
//            };
//            db.Comp_Dep_Desig.Add(nw);
//            db.SaveChanges();
//        }
//    }
//}
//public JsonResult Mig()
//{
//    var res = db.getreceipt().ToList();
//        List<string> data = new List<string>();
//    foreach (var item in res)
//    {
//        var info = item.Info.Replace("Banking - UpdateReceipt - ", "");
//        Temp a = JsonConvert.DeserializeObject<Temp>(info);
//        var rece = db.Sp_Get_Receipt_Parameter_ById(a.r.Id).FirstOrDefault();

//        if (rece is null)
//        {
//            data.Add(info);
//            continue;
//        }


//        if (rece.Module == Modules.FileManagement.ToString())
//        {
//            db.Sp_Update_Receipt(a.r.Id, a.r.Amount, a.r.Name, a.r.Father_Name, a.r.Contact, a.r.AmountinWords, a.r.PaymentType, a.r.Bank, a.r.Ch_Pay_Draft_No, "SA Systems", rece.File_Plot_No, a.r.Size,
//                    a.r.Type, a.r.TokenParameter, a.r.Userid, a.r.DateTime, rece.Module, a.r.ReceiptNo, a.r.Ch_Pay_Draft_Date);
//        }
//        else if (rece.Module == Modules.PlotManagement.ToString())
//        {
//            db.Sp_Update_Receipt(a.r.Id, a.r.Amount, a.r.Name, a.r.Father_Name, a.r.Contact, a.r.AmountinWords, a.r.PaymentType, a.r.Bank, a.r.Ch_Pay_Draft_No, "SA Systems", rece.File_Plot_No, a.r.Size,
//            a.r.Type, a.r.TokenParameter, a.r.Userid, a.r.DateTime, rece.Module, a.r.ReceiptNo, a.r.Ch_Pay_Draft_Date);
//        }




//        //var lastowner = db.Sp_Get_FileLastOwner(a.Filefromid).FirstOrDefault();
//        //var installmentids = new XElement("InstallmentIds", a.Ids.Select(x => new XElement("InsIds",
//        //                 new XAttribute("Id", x)
//        //                 ))).ToString();
//        //var res1 = db.Sp_Update_Installments_Cash_Status(installmentids, a.rd.Amount).FirstOrDefault();

//        //var res2 = db.Sp_Add_Receipt_Manual(a.rd.Amount, a.rd.AmountInWords, a.rd.Bank, a.rd.PayChqNo, a.rd.Ch_bk_Pay_Date, a.rd.Branch, a.rd.Mobile_1
//        //    , a.rd.Father_Husband, a.rd.FilePlotNumber.ToString(), a.rd.Name, a.rd.PaymentType, a.rd.TotalAmount,
//        //    Modules.FileManagement.ToString(), a.rd.Rate, null, a.rd.Plot_Size, ReceiptTypes.Installment.ToString(), item.Userid, item.Userid, a.rd.ReceiptNo, item.Datetime, a.rd.DevCharges, a.rd.FilePlotNumber.ToString()).FirstOrDefault();

//    }
//    return Json(data, JsonRequestBehavior.AllowGet);
//}
//public void rehmam()
//{
//    var res = db.PlotToMoves.ToList().Distinct();
//    DateTime dat = new DateTime(2020, 1, 30);
//    foreach (var item in res)
//    {
//        var res2 = db.Sp_Add_Receipt_Manual(Convert.ToDecimal( item.Dimension), GeneralMethods.NumberToWords(Convert.ToInt32(item.Dimension)) , "", "",null, "", "03004003740"
//          , "Muhammad Ismail", item.Plot_No.ToString(), "Abdul Ghaffar", "Cash", 0,
//          Modules.FileManagement.ToString(), 0, null, "", ReceiptTypes.Booking.ToString(), 13, 13, item.Plot_Size, dat, "", item.Plot_No.ToString()).FirstOrDefault();

//    }
//}

