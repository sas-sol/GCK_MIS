using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using System.Xml.Linq;
using System.Xml;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class AccountHandlerController : Controller
    {
        // GET: AccountHandler
        private Grand_CityEntities db = new Grand_CityEntities();

        public Sp_Get_COA_Head_Code_Result HeadFinder(string Module, string ModuleType, string AccountType, int CompanyId, long? ModuleId)
        {
            var res = db.COA_Mapper.Where(x =>
                           x.CompanyId == CompanyId &&
                           x.Module == Module &&
                           x.ModuleType == ModuleType &&
                           x.AccountType == AccountType &&
                           x.Module_Id == ModuleId
                           ).FirstOrDefault();

            if (res == null)
            {
                return null;
            }

            var Item_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Item_COA;
        }

        public PaymentTypeModel Payment_Head(string PaymentType, string Debit_Credit, int? bankId, int CompanyId)
        {
            if (PaymentType == "Cash")
            {
                var Payment = HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Cash_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), CompanyId, null);//db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").FirstOrDefault();
                var vt = (Debit_Credit == "Debit") ? Voucher_Type.CRV.ToString() : Voucher_Type.CPV.ToString();
                var res = new PaymentTypeModel { PaymentStatus = null, PaymentData = Payment, VoucherType = vt };
                return res;
            }
            else
            {
                var Payment = HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Bank_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), CompanyId, Convert.ToInt64(bankId));//db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").FirstOrDefault();
                var vt = (Debit_Credit == "Debit") ? Voucher_Type.BRV.ToString() : Voucher_Type.BPV.ToString();
                var res = new PaymentTypeModel { PaymentStatus = null, PaymentData = Payment, VoucherType = vt };
                return res;
            }
        }
        public Sp_Get_COA_Head_Code_Result Banks_Head(string BankAccount, int CompanyId)
        {
            var bankdet = db.Bank_Accounts.Where(x => x.Account_Number == BankAccount).FirstOrDefault();
            var account = HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Bank_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), CompanyId, bankdet.Id);
            return account;
        }
        public Sp_Get_COA_Head_Code_Result Sub_Banks_Head(string BankAccount)
        {
            var bankdet = db.Bank_Accounts.Where(x => x.Account_Number == BankAccount).FirstOrDefault();
            var res = db.COA_Mapper.Where(x =>
                           x.Module == COA_Mapper_Modules.Accounting.ToString() &&
                           x.ModuleType == COA_Mapper_ModuleTypes.Bank_Account.ToString() &&
                           x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                           x.Module_Id == bankdet.Id
                           ).FirstOrDefault();

            if (res == null)
            {
                return null;
            }
            var Item_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            Item_COA.Comp_Id = res.CompanyId;
            return Item_COA;
        }
        public Sp_Get_COA_Head_Code_Result Banks_Online_Head(string BankAccount, int CompanyId)
        {
            var bankdet = db.Bank_Online_Accounts.Where(x => x.Account_No == BankAccount).FirstOrDefault();
            var account = HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Online.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), CompanyId, bankdet.Id);
            return account;
        }
        //File Plot Management

        public JsonResult Register_Plot(decimal? Amount, string Plot_No, string Type, string Block, long TransactionId, long userid, int line, string jv, string Module, long BlockId)
        {
            // Idhar sare Plot Registeration ki Double Entries ain g i ..
            var comp = Company_Attr(userid);
            string IncomeModule = "";
            string ReceivableModule = "";
            if (Module == COA_Mapper_Modules.Commercial.ToString())
            {
                IncomeModule = COA_Mapper_ModuleTypes.Commercial_Project_Sales.ToString();
                ReceivableModule = COA_Mapper_ModuleTypes.Commercial_Project_Receivables.ToString();
                var File_COA = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId); //BlockId her will be ProjectId of CommercialProject
                var File_Sales = HeadFinder(Module, IncomeModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, File_COA.Text_ChartCode + " - " + File_COA.Head, File_COA.Id, File_COA.Text_ChartCode, File_COA.Head, "Booking of Unit number: " + Plot_No, TransactionId, line, userid, null, null, "", "", "", null, null, null, null, jv.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, File_Sales.Text_ChartCode + " - " + File_Sales.Head, File_Sales.Id, File_Sales.Text_ChartCode, File_Sales.Head, "Booking of Unit number: " + Plot_No, TransactionId, line, userid, null, null, "", "", "", null, null, null, null, jv.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                if (Type == PlotType.Commercial.ToString())
                {
                    IncomeModule = COA_Mapper_ModuleTypes.Plots_Sales_Commercial.ToString();
                    ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();
                }
                else
                {
                    IncomeModule = COA_Mapper_ModuleTypes.Plots_Sales_Residential.ToString();
                    ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                }
                var File_COA = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);
                var File_Sales = HeadFinder(Module, IncomeModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, File_COA.Text_ChartCode + " - " + File_COA.Head, File_COA.Id, File_COA.Text_ChartCode, File_COA.Head, "Booking of Plot number: " + Plot_No, TransactionId, line, userid, null, null, "", "", "", null, null, null, null, jv.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, File_Sales.Text_ChartCode + " - " + File_Sales.Head, File_Sales.Id, File_Sales.Text_ChartCode, File_Sales.Head, "Booking of Plot number: " + Plot_No, TransactionId, line, userid, null, null, "", "", "", null, null, null, null, jv.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }
        public JsonResult Receive_Plot_Amount(decimal? Amount, string Plot_No, string Type, string Block, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string VoucherNo, int line, bool headcashier, string Module, long BlockId)
        {
            var comp = Company_Attr(userid);
            string ReceivableModule = "";
            if (Module == COA_Mapper_Modules.Commercial.ToString())
            {
                ReceivableModule = COA_Mapper_ModuleTypes.Commercial_Project_Receivables.ToString();
                if (headcashier)
                {
                    if (PaymentType == "Cash")
                    {
                        var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                        var File_COA = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);//BlockId here is ModuleId of COAMapper which is projectId from Commercial Floors
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Amount Against Unit Number :" + Plot_No + " Floor: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var credit = db.Sp_Add_Journal_Entry(0, Amount, File_COA.Text_ChartCode + " - " + File_COA.Head, File_COA.Id, File_COA.Text_ChartCode, File_COA.Head, "Amount Against Unit Number :" + Plot_No + " Floor: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        var File_COA = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);//BlockId here is ModuleId of COAMapper which is projectId from Commercial Floors
                        try
                        {
                            var a = File_COA.Text_ChartCode;
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }

                    }
                }
                else
                {
                    if (PaymentType == "Cash")
                    {
                        var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                        var File_COA = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);//BlockId here is ModuleId of COAMapper which is projectId from Commercial Floors
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var Reciv_Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Amount Against Unit Number :" + Plot_No + " Floor: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Payment.VoucherType, 9).FirstOrDefault();
                                var Receivable_Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, File_COA.Text_ChartCode + " - " + File_COA.Head, File_COA.Id, File_COA.Text_ChartCode, File_COA.Head, "Amount Against Unit Number :" + Plot_No + " Floor: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Payment.VoucherType, 9).FirstOrDefault();   /// For Cash and Bank Credit
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        var File_COA = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);//BlockId here is ModuleId of COAMapper which is projectId from Commercial Floors
                        try
                        {
                            var a = File_COA.Text_ChartCode;
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
            }
            else
            {
                if (Type == PlotType.Commercial.ToString())
                {
                    ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();
                }
                else
                {
                    ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                }
                if (headcashier)
                {
                    if (PaymentType == "Cash")
                    {
                        var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                        var File_COA = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);//this.Plot_Head(Plot_No, Type, Block);
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                //var debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();
                                //var credit = db.Sp_Add_Journal_Entry(0, Amount, File_COA.Text_ChartCode + " - " + File_COA.Head, File_COA.Id, File_COA.Text_ChartCode, File_COA.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                }
                else
                {
                    if (PaymentType == "Cash")
                    {
                        var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                        var File_COA = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);//this.Plot_Head(Plot_No, Type, Block);
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var Reciv_Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var Receivable_Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, File_COA.Text_ChartCode + " - " + File_COA.Head, File_COA.Id, File_COA.Text_ChartCode, File_COA.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                }
            }
        }
        public JsonResult Transfer_Plot(decimal Amount, string Plot_No, string Type, string Block, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string Voucher_No, int line, bool headcashier, string Module, long ModuleId)
        {
            var comp = Company_Attr(userid);
            string moduleType = "";
            if (Module == COA_Mapper_Modules.Commercial.ToString())
            {
                moduleType = COA_Mapper_ModuleTypes.Commercial_Project_Transfer_Charges.ToString();
                if (headcashier)
                {
                    if (PaymentType == "Cash")
                    {
                        var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                        var Block_COA = HeadFinder(Module, moduleType, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//ModuleId will be ProjectId
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Transfer Fee Against :" + Plot_No + " - Floor: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var Credit = db.Sp_Add_Journal_Entry(0, Amount, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, "Transfer Fee Against :" + Plot_No + " - Floor: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                }
                else
                {
                    if (PaymentType == "Cash")
                    {
                        var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                        var Block_COA = HeadFinder(Module, moduleType, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//ModuleId will be ProjectId
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Transfer Fee Against :" + Plot_No + " - Floor: " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, "Transfer Fee Against :" + Plot_No + " - Floor: " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                }
            }
            else
            {
                if (Type == "Residential")
                {
                    moduleType = COA_Mapper_ModuleTypes.Plots_Transfer_Charges_Residential.ToString();
                }
                else
                {
                    moduleType = COA_Mapper_ModuleTypes.Plots_Transfer_Charges_Commercial.ToString();
                }
                if (headcashier)
                {
                    if (PaymentType == "Cash")
                    {
                        var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                        var Block_COA = HeadFinder(Module, moduleType, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//Transfer_Head(Plot_No, Type, Block);
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Transfer Fee Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var Credit = db.Sp_Add_Journal_Entry(0, Amount, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, "Transfer Fee Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                }
                else
                {
                    if (PaymentType == "Cash")
                    {
                        var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                        var Block_COA = HeadFinder(Module, moduleType, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//Transfer_Head(Plot_No, Type, Block);
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Transfer Fee Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, "Transfer Fee Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                }
            }
        }
        public JsonResult Other_Recovery(decimal Amount, string Plot_No, string Type, string Block, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, string recoveryModule, bool headcashier, string Module)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Mis_Income_COA = HeadFinder(Module, recoveryModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//OtherRecoveryCharges(recoveryModule);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Remarks + " :" + Plot_No, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, Mis_Income_COA.Text_ChartCode + " - " + Mis_Income_COA.Head, Mis_Income_COA.Id, Mis_Income_COA.Text_ChartCode, Mis_Income_COA.Head, Remarks + " :" + Plot_No, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Mis_Income_COA = HeadFinder(Module, recoveryModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//OtherRecoveryCharges(recoveryModule);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Remarks + " :" + Plot_No, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Mis_Income_COA.Text_ChartCode + " - " + Mis_Income_COA.Head, Mis_Income_COA.Id, Mis_Income_COA.Text_ChartCode, Mis_Income_COA.Head, Remarks + " :" + Plot_No, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
        }
        public JsonResult Service_Charges_Electricity(decimal Amount, string Plot_No, string Block, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, bool headcashier, string Module, long ModuleId)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Block_COA = HeadFinder(Module, COA_Mapper_ModuleTypes.Electricity_Charges_Income.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//db.Sp_Get_CA_Head_MultiRef_4("Income", "Other Income", "Electricity Charges", Block + " Block").FirstOrDefault();
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Electricity Service Charges Against :" + Plot_No, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, "Electricity Service Charges Against :" + Plot_No, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Block_COA = HeadFinder(Module, COA_Mapper_ModuleTypes.Electricity_Charges_Income.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//db.Sp_Get_CA_Head_MultiRef_4("Income", "Other Income", "Electricity Charges", Block + " Block").FirstOrDefault();
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Electricity Service Charges Against :" + Plot_No, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, "Electricity Service Charges Against :" + Plot_No, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
        }
        public JsonResult Service_Charges(decimal Amount, string Plot_No, string Block, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Inst_Branch, string Remarks, long TransactionId, long userid, string Voucher_No, int line, bool headcashier, string Module, long ModuleId)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Block_COA = HeadFinder(Module, COA_Mapper_ModuleTypes.Service_Charges_Income.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//db.Sp_Get_CA_Head_MultiRef_4("Income", "Other Income", "Service Charges", Block + " Block").FirstOrDefault();
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Service Charges Against :" + Plot_No + " - " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, "Service Charges Against :" + Plot_No + " - " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Block_COA = HeadFinder(Module, COA_Mapper_ModuleTypes.Service_Charges_Income.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//db.Sp_Get_CA_Head_MultiRef_4("Income", "Other Income", "Service Charges", Block + " Block").FirstOrDefault();
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Service Charges Against :" + Plot_No + " - " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, "Service Charges Against :" + Plot_No + " - " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
        }
        public JsonResult DiscountPosting(decimal Amount, string Plot_No, string Block, string Type, long TransactionId, long userid, string VoucherNo, int line, bool headcashier, string Module, long ModuleId)
        {
            var comp = Company_Attr(userid);
            string ReceivableModule = "";
            string ExpenseModule = "";
            if (Module == COA_Mapper_Modules.Commercial.ToString())
            {
                ReceivableModule = COA_Mapper_ModuleTypes.Commercial_Project_Receivables.ToString();
                ExpenseModule = COA_Mapper_ModuleTypes.Commercial_Project_Discount.ToString();
                if (headcashier)
                {
                    var debitAccount = HeadFinder(Module, ExpenseModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
                    var creditaccount = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//BlockId here is ModuleId of COAMapper which is projectId from Commercial Floors
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Discount Against Unit Number :" + Plot_No + " Floor: " + Block, TransactionId, line, userid, userid, null, null, null, null, null, null, "", null, VoucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var credit = db.Sp_Add_Journal_Entry(0, Amount, creditaccount.Text_ChartCode + " - " + creditaccount.Head, creditaccount.Id, creditaccount.Text_ChartCode, creditaccount.Head, "Discount Against Unit Number :" + Plot_No + " Floor: " + Block, TransactionId, line, userid, userid, null, null, null, null, null, null, "", null, VoucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    var debitAccount = HeadFinder(Module, ExpenseModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
                    var creditAccount = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//BlockId here is ModuleId of COAMapper which is projectId from Commercial Floors
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Reciv_Debit = db.Sp_Add_General_Entry(Amount, 0, null, null, "Pending", null, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Discount Against Unit Number :" + Plot_No + " Floor: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Receivable_Credit = db.Sp_Add_General_Entry(0, Amount, null, null, "Pending", null, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Discount Against Unit Number :" + Plot_No + " Floor: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
            }
            else
            {
                if (Type == PlotType.Commercial.ToString())
                {
                    ExpenseModule = COA_Mapper_ModuleTypes.Plots_Discount_Commercial.ToString();
                    ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();
                }
                else
                {
                    ExpenseModule = COA_Mapper_ModuleTypes.Plots_Discount_Residential.ToString();
                    ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                }
                if (headcashier)
                {
                    var debitAccount = HeadFinder(Module, ExpenseModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
                    var creditAccount = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Discount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, null, null, "Pending", null, null, "", null, VoucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var credit = db.Sp_Add_Journal_Entry(0, Amount, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, null, null, "Pending", null, null, "", null, VoucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    var debitAccount = HeadFinder(Module, ExpenseModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
                    var creditAccount = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Reciv_Debit = db.Sp_Add_General_Entry(Amount, 0, null, null, "Pending", null, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Discount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Receivable_Credit = db.Sp_Add_General_Entry(0, Amount, null, null, "Pending", null, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
            }
        }
        public JsonResult Dishonor_Cheq_Recovery(decimal Amount, string Plot_No, string Type, string Block, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string VoucherNo, int line, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Cancellation_COA = HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Receivable_Credit = db.Sp_Add_Journal_Entry(0, Amount, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Cancellation_COA = HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Receivable_Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, "Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, VoucherNo, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }

        }

        public JsonResult Cancellation(decimal? T_Amount, decimal? Rec_Amount, decimal? Deduction, string Plot_No, string Type, string Block, long? BlockId, string Module, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        {
            var comp = Company_Attr(userid);
            string Sales = "";
            string Receivable = "";
            if (Module == COA_Mapper_Modules.Commercial.ToString())
            {
                Sales = COA_Mapper_ModuleTypes.Commercial_Project_Sales.ToString();
                Receivable = COA_Mapper_ModuleTypes.Commercial_Project_Receivables.ToString();
            }
            else
            {
                if (Type == PlotType.Commercial.ToString())
                {
                    Sales = COA_Mapper_ModuleTypes.Plots_Sales_Commercial.ToString();
                    Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();

                }
                else if (Type == PlotType.Residential.ToString())
                {
                    Sales = COA_Mapper_ModuleTypes.Plots_Sales_Residential.ToString();
                    Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                }
            }
            var Sales_COA = HeadFinder(Module, Sales, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);
            var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
            var Plot_Head = HeadFinder(Module, Receivable, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);
            var Cancellation_COA = HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId

            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    try
                    {

                        decimal? Remain = T_Amount - Rec_Amount;
                        decimal? Payable = Rec_Amount - Deduction;
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var Sales_Debit = db.Sp_Add_Journal_Entry(T_Amount, 0, Sales_COA.Text_ChartCode + " - " + Sales_COA.Head, Sales_COA.Id, Sales_COA.Text_ChartCode, Sales_COA.Head, "Cancellation Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var Pay_credit = db.Sp_Add_Journal_Entry(0, Payable, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Cancellation Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var plot_Credit = db.Sp_Add_Journal_Entry(0, Remain, Plot_Head.Text_ChartCode + " - " + Plot_Head.Head, Plot_Head.Id, Plot_Head.Text_ChartCode, Plot_Head.Head, "Cancellation Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var canc_Credit = db.Sp_Add_Journal_Entry(0, Deduction, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, "Cancellation Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        db.Sp_Add_ErrorLog(ex1.Message, (ex1.InnerException is null) ? string.Empty : ex1.InnerException.ToString(), ex1.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(new Return { Status = false, Msg = "Transaction couldn't proceed. Contact SA Systems" });
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    try
                    {

                        decimal? Remain = T_Amount - Rec_Amount;
                        decimal? Payable = Rec_Amount - Deduction;
                        using (var Transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var Sales_Debit = db.Sp_Add_General_Entry(T_Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Sales_COA.Text_ChartCode + " - " + Sales_COA.Head, Sales_COA.Id, Sales_COA.Text_ChartCode, Sales_COA.Head, "Cancellation Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var Pay_credit = db.Sp_Add_General_Entry(0, Payable, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, "Cancellation Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                                var plot_Credit = db.Sp_Add_General_Entry(0, Remain, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Plot_Head.Text_ChartCode + " - " + Plot_Head.Head, Plot_Head.Id, Plot_Head.Text_ChartCode, Plot_Head.Head, "Cancellation Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                                var canc_Credit = db.Sp_Add_General_Entry(0, Deduction, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, "Cancellation Against :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                                Transaction.Commit();
                                return Json(new Return { Status = true, Msg = "Saved" });
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                                throw;
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        db.Sp_Add_ErrorLog(ex1.Message, (ex1.InnerException is null) ? string.Empty : ex1.InnerException.ToString(), ex1.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(new Return { Status = false, Msg = "Transaction couldn't proceed. Contact SA Systems" });
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
        }



        //Dealership Management

        public JsonResult DealerAdvanceRet(decimal? Amount, string Dealership, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Description, long TransactionId, long userid, string Voucher_No, int line, bool headcashier, string Module, long? ModuleId)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var Payment = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                var Dealer_COA = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_List.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//this.Dealer_Head(Dealership);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Receivable_Credit = db.Sp_Add_Journal_Entry(Amount, 0, Dealer_COA.Text_ChartCode + " - " + Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        var Reciv_Debit = db.Sp_Add_Journal_Entry(0, Amount, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var Payment = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                var Dealer_COA = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_List.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//this.Dealer_Head(Dealership);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Receivable_Credit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Dealer_COA.Text_ChartCode + " - " + Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        var Reciv_Debit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }
        public JsonResult DealerComeRec(decimal? Amount, string Dealership, string Description, long TransactionId, long userid, int line, string vouch, bool headcashier, long? ModuleId)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var Dealer_COA = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_Commission.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//db.Sp_Get_CA_Head_MultiRef("Expenses", "Plots - Sales Commission").FirstOrDefault();
                var ComExp = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Plot_Sales_Commission_Expense.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//this.Dealer_Head(Dealership);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res7 = db.Sp_Add_Journal_Entry(Amount, 0, ComExp.Text_ChartCode + " - " + ComExp.Head, ComExp.Id, ComExp.Text_ChartCode, ComExp.Head, Description, TransactionId, line, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var res6 = db.Sp_Add_Journal_Entry(0, Amount, Dealer_COA.Text_ChartCode + " - " + Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, line, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var Dealer_COA = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_Commission.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);//db.Sp_Get_CA_Head_MultiRef("Expenses", "Plots - Sales Commission").FirstOrDefault();
                var ComExp = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Plot_Sales_Commission_Expense.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//this.Dealer_Head(Dealership);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res7 = db.Sp_Add_General_Entry(Amount, 0, null, null, null, null, ComExp.Text_ChartCode, ComExp.Id, ComExp.Text_ChartCode, ComExp.Head, Description, TransactionId, line, null, null, null, userid, vouch.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var res6 = db.Sp_Add_General_Entry(0, Amount, null, null, null, null, ComExp.Text_ChartCode, ComExp.Id, ComExp.Text_ChartCode, ComExp.Head, Description, TransactionId, line, null, null, null, userid, vouch.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }

        //Human Resource
        public JsonResult Salary_Posting(decimal? BasicSalary, decimal? Allowance, decimal? Bonus, decimal? Overtime, decimal? TaxDeduction, decimal? LoanDeduction, decimal? AdvanceDeduction, decimal? OtherDeduction, decimal? ExtraFuel, decimal? NetSalary, long TransactionId, long userid, string vouch, bool headcashier, string Module, int ModuleId, DateTime? SalaryDate)
        {
            var BSAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_BasicSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            var ALAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_Allowance.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            var BOAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_Bonus.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            var OTAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_Overtime.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            var TDAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_TaxDeduction.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            var LDAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_LoanDeduction.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            var ADAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_AdvanceDeduction.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            var ODAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_OtherDeductions.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            //var EFAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_ExtraFuelCharges.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
            var NSAccount = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (BasicSalary > 0) { var res1 = db.Sp_Add_Journal_Entry(BasicSalary, 0, BSAccount.Text_ChartCode, BSAccount.Id, BSAccount.Text_ChartCode, BSAccount.Head, "Basic Salary for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 1, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    if (Allowance > 0) { var res2 = db.Sp_Add_Journal_Entry(Allowance, 0, ALAccount.Text_ChartCode, ALAccount.Id, ALAccount.Text_ChartCode, ALAccount.Head, "Allowance for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 2, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    if (Bonus > 0) { var res3 = db.Sp_Add_Journal_Entry(Bonus, 0, BOAccount.Text_ChartCode, BOAccount.Id, BOAccount.Text_ChartCode, BOAccount.Head, "Bonus for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 3, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    if (Overtime > 0) { var res4 = db.Sp_Add_Journal_Entry(Overtime, 0, OTAccount.Text_ChartCode, OTAccount.Id, OTAccount.Text_ChartCode, OTAccount.Head, "Overtime for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 4, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    if (TaxDeduction > 0) { var res5 = db.Sp_Add_Journal_Entry(0, TaxDeduction, TDAccount.Text_ChartCode, TDAccount.Id, TDAccount.Text_ChartCode, TDAccount.Head, "Tax Deduction for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 5, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    if (LoanDeduction > 0) { var res6 = db.Sp_Add_Journal_Entry(0, LoanDeduction, LDAccount.Text_ChartCode, LDAccount.Id, LDAccount.Text_ChartCode, LDAccount.Head, "Loan Deduction for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 6, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    if (AdvanceDeduction > 0) { var res7 = db.Sp_Add_Journal_Entry(0, AdvanceDeduction, ADAccount.Text_ChartCode, ADAccount.Id, ADAccount.Text_ChartCode, ADAccount.Head, "Advance Deduction for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 7, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    if (OtherDeduction > 0) { var res8 = db.Sp_Add_Journal_Entry(0, OtherDeduction, ODAccount.Text_ChartCode, ODAccount.Id, ODAccount.Text_ChartCode, ODAccount.Head, "Other Deduction for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 8, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    if (NetSalary > 0) { var res10 = db.Sp_Add_Journal_Entry(0, NetSalary, NSAccount.Text_ChartCode, NSAccount.Id, NSAccount.Text_ChartCode, NSAccount.Head, "Net Salary for the Month of " + String.Format("{0:MMM dd, yyyy}", SalaryDate), TransactionId, 10, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), ModuleId).FirstOrDefault(); }
                    Transaction.Commit();
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    throw;
                }
            }
        }
        public JsonResult DisburseSalary(decimal? Amount, string name, string EmpCode, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string Voucher_No, int line, bool headcashier, string Module, int ModuleId)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var debitAcc = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
                var creditAcc = Payment_Head_Salary(PaymentType, Transaction_Type.Credit.ToString(), Inst_Bank);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Salary Received by "+ name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType).FirstOrDefault();
                        //var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType).FirstOrDefault();   /// For Cash and Bank Credit
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
                //var creditAcc = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString());
                var creditAcc = Payment_Head_Salary(PaymentType, Transaction_Type.Credit.ToString(), Inst_Bank);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }

        public JsonResult DisburseSalaryArrear(decimal? Amount, string name, string EmpCode, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string Voucher_No, int line, bool headcashier, string Module, int ModuleId)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var debitAcc = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
                var creditAcc = Payment_Head_Salary(PaymentType, Transaction_Type.Credit.ToString(), Inst_Bank);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Salary Received by "+ name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType).FirstOrDefault();
                        //var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType).FirstOrDefault();   /// For Cash and Bank Credit
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), ModuleId, null);
                //var creditAcc = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString());
                var creditAcc = Payment_Head_Salary(PaymentType, Transaction_Type.Credit.ToString(), Inst_Bank);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }



        /// <summary>
        /// Dealerships
        /// <summary>

        public JsonResult DealerAdvance(decimal? Amount, long Dealership_Id, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Description, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        {
            var comp = this.Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Dealer_COA = this.Dealer_Head(Dealership_Id, comp.Id);

                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Reciv_Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Receivable_Credit = db.Sp_Add_Journal_Entry(0, Amount, Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Dealer_COA = this.Dealer_Head(Dealership_Id, comp.Id);

                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Reciv_Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Receivable_Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
        }
        public JsonResult DealerComeRec(decimal? Amount, long Dealership_Id, string Description, long TransactionId, long userid, int line, string vouch, bool headcashier)
        {
            var comp = this.Company_Attr(userid);
            if (headcashier)
            {
                var res5 = db.Sp_Get_CA_Head_MultiRef("Expenses", "Plots - Sales Commission").FirstOrDefault();
                var Dealer_COA = this.Dealer_Head(Dealership_Id, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res7 = db.Sp_Add_Journal_Entry(Amount, 0, res5.Head + " - " + res5.Code, res5.Id, res5.Code, res5.Head, Description, TransactionId, line, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var res6 = db.Sp_Add_Journal_Entry(0, Amount, Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, line, userid, userid, null, null, null, null, null, null, "", null, vouch.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var res5 = db.Sp_Get_CA_Head_MultiRef("Expenses", "Plots - Sales Commission").FirstOrDefault();
                var Dealer_COA = this.Dealer_Head(Dealership_Id, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res7 = db.Sp_Add_General_Entry(Amount, 0, null, null, null, null, res5.Head + " - " + res5.Code, res5.Id, res5.Code, res5.Head, Description, TransactionId, line, null, null, null, userid, vouch.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var res6 = db.Sp_Add_General_Entry(0, Amount, null, null, null, null, Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Text_ChartCode, Dealer_COA.Head, Description, TransactionId, line, null, null, null, userid, vouch.ToString(), Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }
        public JsonResult DealerRegisterationFee(decimal? Amount, string Dealership, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Description, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    //Check this line
                    var Dealer_COA = db.Sp_Get_CA_Head_MultiRef_3("Income", "Other Income", "Other Misc Income").FirstOrDefault();
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, Dealer_COA.Code + " - " + Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Code, Dealer_COA.Head, Description, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    //Check this line
                    var Dealer_COA = db.Sp_Get_CA_Head_MultiRef_3("Income", "Dealerships", Dealership).FirstOrDefault();
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Dealer_COA.Code + " - " + Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Code, Dealer_COA.Head, Description, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }

        }

        // Procurement  

        // Vendor
        public JsonResult Record_Vendor_Advance(decimal? Amount, string Vendor, long? Vendor_Id, string Description, long TransactionId,long PO_group_Id, long userid, int line, bool headcashier)
        {
            var comp = this.Company_Attr(userid);
            //if (headcashier)
            //{
            var Vendor_COA = this.Vendor_Head(Vendor, Vendor_Id, comp.Id);
            var Advance_TradePayable = this.Advance_TradePayable(comp.Id);

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Advance Payable Recording
                    string Bill1 = new XElement("Bills", new XElement("Entries",
                       new XAttribute("Vendor_Id", Advance_TradePayable.Id),
                       new XAttribute("Terms", "30 Days"),
                       new XAttribute("Bill_No", "Advance"),
                       new XAttribute("Head_Code", Vendor_COA.Text_ChartCode),
                       new XAttribute("Head_Name", Vendor_COA.Head),
                       new XAttribute("Qty", 1),
                       new XAttribute("Rate", Amount),
                       new XAttribute("Line", 1),
                       new XAttribute("description", Description),
                       new XAttribute("Head_Id", Vendor_COA.Id),
                       new XAttribute("GroupId", TransactionId),
                       new XAttribute("PO_Group_Id", PO_group_Id),
                       new XAttribute("Head", Vendor_COA.Head),
                       new XAttribute("Comp_Id", comp.Id)
                       )).ToString();

                    var res6 = db.Sp_Add_Bills(Bill1, userid, DateTime.Now, DateTime.Now.AddDays(30)).FirstOrDefault();
                    var res7 = db.Sp_Add_Payable(TransactionId, Advance_TradePayable.Id, Amount, PO_group_Id, true);

                    var res4 = db.Sp_Add_Journal_Entry(Amount, 0, Vendor_COA.Head, Vendor_COA.Id, Vendor_COA.Text_ChartCode, Vendor_COA.Head, Description, TransactionId, line, userid, userid, null, null, null, null, null, null, "", null, null, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                    var res3 = db.Sp_Add_Journal_Entry(0, Amount, Advance_TradePayable.Head, Advance_TradePayable.Id, Advance_TradePayable.Text_ChartCode, Advance_TradePayable.Head, Description, TransactionId, line, userid, userid, null, null, null, null, null, null, "", null, null, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();




                    //int i = 0;
                    //string Bill2 = new XElement("Bills", items.Select(x => new XElement("Entries",
                    //new XAttribute("Vendor_Id", venHead.Id),
                    //new XAttribute("Terms", "30 Days"),
                    //new XAttribute("Bill_No", x.PO_Number + "-" + Grns.FirstOrDefault()),
                    //new XAttribute("Head_Code", itemHeads[j].Text_ChartCode),
                    //new XAttribute("Head_Name", itemHeads[j].Head),
                    //new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
                    //new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
                    //new XAttribute("Line", ++i),
                    //new XAttribute("description", narration),
                    //new XAttribute("Head_Id", itemHeads[j].Id),
                    //new XAttribute("GroupId", tranNew),
                    //new XAttribute("Head", itemHeads[j++].Head),
                    //new XAttribute("Comp_Id", comp.Id)
                    //))).ToString();


                    //var res8 = db.Sp_Add_Bills(Bill2, userid, DateTime.Now, DateTime.Now.AddDays(30)).FirstOrDefault();




                    Transaction.Commit();
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    throw;
                }
            }

        }
        public JsonResult VendorPayment(decimal? Amount, string Vendor, long? Vendor_Id, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Description, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        {
            var comp = this.Company_Attr(userid);

            if (headcashier)
            {
                var Payment = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                var Vendor_COA = this.Vendor_Head(Vendor, Vendor_Id, comp.Id);


                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var debit = db.Sp_Add_Journal_Entry(Amount, 0, Vendor_COA.Head, Vendor_COA.Id, Vendor_COA.Text_ChartCode, Vendor_COA.Head, Description, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        var credit = db.Sp_Add_Journal_Entry(0, Amount, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var Payment = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                var Vendor_COA = this.Vendor_Head(Vendor, Vendor_Id, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Vendor_COA.Head, Vendor_COA.Id, Vendor_COA.Text_ChartCode, Vendor_COA.Head, Description, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        var credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Description, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }

        }

        // Banking Instruments
        public JsonResult BankGLEntry(long? ReceiptId, string BankAccount, long userid)
        {
            var comp = this.Company_Attr(userid);
            //var res = db.Receipts.Where(x => x.Id == ReceiptId).FirstOrDefault();
            //if (res.Module == Modules.PlotManagement.ToString() || res.Module == Modules.FileManagement.ToString())
            //{
            //    if (res.Type == ReceiptTypes.Dealership_Security.ToString() || res.Type == ReceiptTypes.DealerAdvance.ToString() || res.Type == ReceiptTypes.DealershipRegisteration.ToString())
            //    {
            //        var debitAccount = Banks_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_List.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res.File_Plot_No);//this.Dealer_Head(res.File_Plot_No, comp.Id);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received By :" + res.File_Plot_Number + " - " + res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received By :" + res.File_Plot_Number + " - " + res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.Booking.ToString() || res.Type == ReceiptTypes.Installment.ToString() || res.Type == ReceiptTypes.Advance.ToString() || res.Type == ReceiptTypes.Confirmation.ToString())
            //    {
            //        long blockId;
            //        if (res.Module == Modules.PlotManagement.ToString())
            //        {
            //            blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
            //        }
            //        else if (res.Module == Modules.FileManagement.ToString())
            //        {
            //            blockId = Convert.ToInt64(db.Sp_Get_FileAppFormData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
            //        }
            //        else
            //        {
            //            blockId = 0;
            //        }
            //        string receivableModule = "";
            //        if (res.Plot_Type == PlotType.Commercial.ToString())
            //        {
            //            receivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();
            //        }
            //        else
            //        {
            //            receivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
            //        }
            //        var debitAccount = Banks_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), receivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.ServiceCharges.ToString())
            //    {
            //        long blockId;
            //        if (res.Module == Modules.PlotManagement.ToString())
            //        {
            //            blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
            //        }
            //        else if (res.Module == Modules.FileManagement.ToString())
            //        {
            //            blockId = Convert.ToInt64(db.Sp_Get_FileData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
            //        }
            //        else
            //        {
            //            blockId = 0;
            //        }
            //        var debitAccount = Banks_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Electricity_Charges_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);//Plot_Head(res.File_Plot_Number, res.Plot_Type, res.Block);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Service Charges of :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Service Charges of :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.Electricity_Charges.ToString())
            //    {
            //        long blockId;
            //        if (res.Module == Modules.PlotManagement.ToString())
            //        {
            //            blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
            //        }
            //        else if (res.Module == Modules.FileManagement.ToString())
            //        {
            //            blockId = Convert.ToInt64(db.Sp_Get_FileData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
            //        }
            //        else
            //        {
            //            blockId = 0;
            //        }
            //        var debitAccount = Banks_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Electricity_Charges_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);//Plot_Head(res.File_Plot_Number, res.Plot_Type, res.Block);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Service Charges of :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Service Charges of :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.Cancellation.ToString())
            //    {
            //        var debitAccount = Banks_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Cancellation.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Cancellation Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Cancellation Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.Transfer.ToString())
            //    {
            //        long blockId;
            //        if (res.Module == Modules.PlotManagement.ToString())
            //        {
            //            blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
            //        }
            //        else if (res.Module == Modules.FileManagement.ToString())
            //        {
            //            blockId = Convert.ToInt64(db.Sp_Get_FileData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
            //        }
            //        else
            //        {
            //            blockId = 0;
            //        }
            //        string moduleType = "";
            //        if (res.Plot_Type == PlotType.Residential.ToString())
            //        {
            //            moduleType = COA_Mapper_ModuleTypes.Plots_Transfer_Charges_Residential.ToString();
            //        }
            //        else
            //        {
            //            moduleType = COA_Mapper_ModuleTypes.Plots_Transfer_Charges_Commercial.ToString();
            //        }
            //        var debitAccount = Banks_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), moduleType, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        var debitAccount = Banks_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), res.Type, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//Plot_Head(res.File_Plot_Number, res.Plot_Type, res.Block);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against " + res.Type.Replace('_', ' '), Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against " + res.Type.Replace('_', ' '), Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //}
            //else if (res.Module == Modules.CommercialManagement.ToString())
            //{
            //    if (res.Type == ReceiptTypes.Booking.ToString() || res.Type == ReceiptTypes.Confirmation.ToString() || res.Type == ReceiptTypes.Installment.ToString() || res.Type == ReceiptTypes.Advance.ToString())
            //    {
            //        var projectid = db.Sp_Get_CommercialData(res.File_Plot_No).Select(x => x.ProjectId).FirstOrDefault();

            //        /* Bank account*/
            //        //var sub_debitAccount = Sub_Banks_Head(BankAccount);
            //        //if (comp.Id != sub_debitAccount.Comp_Id)
            //        //{
            //        //    /* Liability account*/
            //        //    var credac = this.Subsidary_Account(comp.Id, (int)sub_debitAccount.Comp_Id);

            //        //    var debitac = this.SAGPayable_Head(comp.Id);
            //        //    var creditAccount = HeadFinder(COA_Mapper_Modules.Commercial.ToString(), COA_Mapper_ModuleTypes.Commercial_Project_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, projectid);
            //        //    using (var Transaction = db.Database.BeginTransaction())
            //        //    {
            //        //        try
            //        //        {
            //        //            var Debit_comp1 = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, sub_debitAccount.Text_ChartCode + " - " + sub_debitAccount.Head, sub_debitAccount.Id, sub_debitAccount.Text_ChartCode, sub_debitAccount.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", (int)sub_debitAccount.Comp_Id).FirstOrDefault();
            //        //            var Credit_comp1 = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, credac.Text_ChartCode + " - " + credac.Head, credac.Id, credac.Text_ChartCode, credac.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", (int)sub_debitAccount.Comp_Id).FirstOrDefault();

            //        //            var Debit_comp2 = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, sub_debitAccount.Text_ChartCode + " - " + debitac.Head, debitac.Id, debitac.Text_ChartCode, debitac.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //        //            var Credit_comp2 = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();

            //        //            Transaction.Commit();
            //        //            return Json(new Return { Status = true, Msg = "Saved" });
            //        //        }
            //        //        catch (Exception ex)
            //        //        {
            //        //            Transaction.Rollback();
            //        //            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //        //            throw;
            //        //        }
            //        //    }
            //        //}
            //        //else
            //        //{

            //            var debitAccount = Banks_Head(BankAccount, comp.Id);
            //            var creditAccount = HeadFinder(COA_Mapper_Modules.Commercial.ToString(), COA_Mapper_ModuleTypes.Commercial_Project_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, projectid);
            //            using (var Transaction = db.Database.BeginTransaction())
            //            {
            //                try
            //                {
            //                    var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                    var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                    Transaction.Commit();
            //                    return Json(new Return { Status = true, Msg = "Saved" });
            //                }
            //                catch (Exception ex)
            //                {
            //                    Transaction.Rollback();
            //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                    throw;
            //                }
            //            }
            //        //}

            //    }
            //}
            //else if (res.Module == Modules.Dealers.ToString())
            //{
            //    var debitAccount = Banks_Head(BankAccount, comp.Id);
            //    var creditAccount = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_Registration.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
            //    using (var Transaction = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Dealership Registration of " + res.File_Plot_Number, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Dealership Registration of " + res.File_Plot_Number, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            Transaction.Commit();
            //            return Json(new Return { Status = true, Msg = "Saved" });
            //        }
            //        catch (Exception ex)
            //        {
            //            Transaction.Rollback();
            //            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //            throw;
            //        }
            //    }
            //}
            //else if (res.Module == Modules.Accounts_Management.ToString())
            //{
            //    //File Plot Number in this section is Head Id of a Receivable Account
            //    var debitAccount = Banks_Head(BankAccount, comp.Id);
            //    var creditAccount = GetHead(Convert.ToInt32(res.File_Plot_Number));
            //    using (var Transaction = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            Transaction.Commit();
            //            return Json(new Return { Status = true, Msg = "Saved" });
            //        }
            //        catch (Exception ex)
            //        {
            //            Transaction.Rollback();
            //            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //            throw;
            //        }
            //    }
            //}
            //else if (res.Module == "Other_Recovery")
            //{
            //    //File Plot Number in this section is Head Id of a Receivable Account
            //    var debitAccount = Banks_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId
            //    using (var Transaction = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            Transaction.Commit();
            //            return Json(new Return { Status = true, Msg = "Saved" });
            //        }
            //        catch (Exception ex)
            //        {
            //            Transaction.Rollback();
            //            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //            throw;
            //        }
            //    }
            //}

            return Json(true);
        }
        //public JsonResult BankJLEntry(long? ReceiptId, string BankAccount, long userid, DateTime date)
        //{
        //    var comp = this.Company_Attr(userid);
        //    var res = db.Receipts.Where(x => x.Id == ReceiptId).FirstOrDefault();
        //    if (res.Module == Modules.PlotManagement.ToString() || res.Module == Modules.FileManagement.ToString())
        //    {
        //        if (res.Type == ReceiptTypes.Dealership_Security.ToString() || res.Type == ReceiptTypes.DealerAdvance.ToString() || res.Type == ReceiptTypes.DealershipRegisteration.ToString())
        //        {
        //            var debitAccount = Banks_Head(BankAccount, comp.Id);
        //            var creditAccount = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_List.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res.File_Plot_No);//this.Dealer_Head(res.File_Plot_No, comp.Id);
        //            using (var Transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var Debit = new Journal_Entries()
        //                    {
        //                        Debit = res.Amount,
        //                        Credit = 0,
        //                        Inst_Bank = res.Bank,
        //                        Inst_No = res.Ch_Pay_Draft_No,
        //                        Status = "Approved",
        //                        Inst_Date = res.Ch_Pay_Draft_Date,
        //                        Head = debitAccount.Head,
        //                        Head_Id = debitAccount.Id,
        //                        Head_Code = debitAccount.Text_ChartCode,
        //                        Head_Name = debitAccount.Head,
        //                        Naration = "Amount Received By :" + res.File_Plot_Number + " - " + res.Text,
        //                        GroupId = Convert.ToInt64(res.Transaction_Id),
        //                        Line = 1,
        //                        Qty = null,
        //                        UOM = null,
        //                        Rate = null,
        //                        Recorded_By = userid,
        //                        RecordedBy_Date = date,
        //                        Supvise_by = userid,
        //                        Supviseby_Date = date,
        //                        Voucher_No = res.ReceiptNo,
        //                        Voucher_Type = "BRV",
        //                        Comp_Id = comp.Id
        //                    };
        //                    var Credit = new Journal_Entries()
        //                    {
        //                        Debit = 0,
        //                        Credit = res.Amount,
        //                        Inst_Bank = res.Bank,
        //                        Inst_No = res.Ch_Pay_Draft_No,
        //                        Status = "Approved",
        //                        Inst_Date = res.Ch_Pay_Draft_Date,
        //                        Head = creditAccount.Head,
        //                        Head_Id = creditAccount.Id,
        //                        Head_Code = creditAccount.Text_ChartCode,
        //                        Head_Name = creditAccount.Head,
        //                        Naration = "Amount Received By :" + res.File_Plot_Number + " - " + res.Text,
        //                        GroupId = Convert.ToInt64(res.Transaction_Id),
        //                        Line = 1,
        //                        Qty = null,
        //                        UOM = null,
        //                        Rate = null,
        //                        Recorded_By = userid,
        //                        RecordedBy_Date = date,
        //                        Supvise_by = userid,
        //                        Supviseby_Date = date,
        //                        Voucher_No = res.ReceiptNo,
        //                        Voucher_Type = "BRV",
        //                        Comp_Id = comp.Id
        //                    };
        //                    db.Journal_Entries.Add(Debit);
        //                    db.Journal_Entries.Add(Credit);
        //                    db.SaveChanges();

        //                    Transaction.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //        else if (res.Type == ReceiptTypes.Booking.ToString() || res.Type == ReceiptTypes.Installment.ToString() || res.Type == ReceiptTypes.Advance.ToString() || res.Type == ReceiptTypes.Confirmation.ToString())
        //        {
        //            long blockId;
        //            if (res.Module == Modules.PlotManagement.ToString())
        //            {
        //                blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
        //            }
        //            else if (res.Module == Modules.FileManagement.ToString())
        //            {
        //                blockId = Convert.ToInt64(db.Sp_Get_FileAppFormData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
        //            }
        //            else
        //            {
        //                blockId = 0;
        //            }
        //            string receivableModule = "";
        //            if (res.Plot_Type == PlotType.Commercial.ToString())
        //            {
        //                receivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();
        //            }
        //            else
        //            {
        //                receivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
        //            }
        //            var debitAccount = Banks_Head(BankAccount, comp.Id);
        //            var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), receivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);
        //            using (var Transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var Debit = new Journal_Entries()
        //                    {
        //                        Debit = res.Amount,
        //                        Credit = 0,
        //                        Inst_Bank = res.Bank,
        //                        Inst_No = res.Ch_Pay_Draft_No,
        //                        Status = "Approved",
        //                        Inst_Date = res.Ch_Pay_Draft_Date,
        //                        Head = debitAccount.Head,
        //                        Head_Id = debitAccount.Id,
        //                        Head_Code = debitAccount.Text_ChartCode,
        //                        Head_Name = debitAccount.Head,
        //                        Naration = "Amount Received By :" + res.File_Plot_Number + " - " + res.Text,
        //                        GroupId = Convert.ToInt64(res.Transaction_Id),
        //                        Line = 1,
        //                        Qty = null,
        //                        UOM = null,
        //                        Rate = null,
        //                        Recorded_By = userid,
        //                        RecordedBy_Date = date,
        //                        Supvise_by = userid,
        //                        Supviseby_Date = date,
        //                        Voucher_No = res.ReceiptNo,
        //                        Voucher_Type = "BRV",
        //                        Comp_Id = comp.Id
        //                    };
        //                    var Credit = new Journal_Entries()
        //                    {
        //                        Debit = 0,
        //                        Credit = res.Amount,
        //                        Inst_Bank = res.Bank,
        //                        Inst_No = res.Ch_Pay_Draft_No,
        //                        Status = "Approved",
        //                        Inst_Date = res.Ch_Pay_Draft_Date,
        //                        Head = creditAccount.Head,
        //                        Head_Id = creditAccount.Id,
        //                        Head_Code = creditAccount.Text_ChartCode,
        //                        Head_Name = creditAccount.Head,
        //                        Naration = "Amount Received By :" + res.File_Plot_Number + " - " + res.Text,
        //                        GroupId = Convert.ToInt64(res.Transaction_Id),
        //                        Line = 1,
        //                        Qty = null,
        //                        UOM = null,
        //                        Rate = null,
        //                        Recorded_By = userid,
        //                        RecordedBy_Date = date,
        //                        Supvise_by = userid,
        //                        Supviseby_Date = date,
        //                        Voucher_No = res.ReceiptNo,
        //                        Voucher_Type = "BRV",
        //                        Comp_Id = comp.Id
        //                    };
        //                    db.Journal_Entries.Add(Debit);
        //                    db.Journal_Entries.Add(Credit);
        //                    db.SaveChanges();

        //                    Transaction.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //        else if (res.Type == ReceiptTypes.ServiceCharges.ToString())
        //        {
        //            long blockId;
        //            if (res.Module == Modules.PlotManagement.ToString())
        //            {
        //                blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
        //            }
        //            else if (res.Module == Modules.FileManagement.ToString())
        //            {
        //                blockId = Convert.ToInt64(db.Sp_Get_FileData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
        //            }
        //            else
        //            {
        //                blockId = 0;
        //            }
        //            var debitAccount = Banks_Head(BankAccount, comp.Id);
        //            var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Electricity_Charges_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);//Plot_Head(res.File_Plot_Number, res.Plot_Type, res.Block);
        //            using (var Transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var Debit = new Journal_Entries()
        //                    {
        //                        Debit = res.Amount,
        //                        Credit = 0,
        //                        Inst_Bank = res.Bank,
        //                        Inst_No = res.Ch_Pay_Draft_No,
        //                        Status = "Approved",
        //                        Inst_Date = res.Ch_Pay_Draft_Date,
        //                        Head = debitAccount.Head,
        //                        Head_Id = debitAccount.Id,
        //                        Head_Code = debitAccount.Text_ChartCode,
        //                        Head_Name = debitAccount.Head,
        //                        Naration = "Amount Received By :" + res.File_Plot_Number + " - " + res.Text,
        //                        GroupId = Convert.ToInt64(res.Transaction_Id),
        //                        Line = 1,
        //                        Qty = null,
        //                        UOM = null,
        //                        Rate = null,
        //                        Recorded_By = userid,
        //                        RecordedBy_Date = date,
        //                        Supvise_by = userid,
        //                        Supviseby_Date = date,
        //                        Voucher_No = res.ReceiptNo,
        //                        Voucher_Type = "BRV",
        //                        Comp_Id = comp.Id
        //                    };
        //                    var Credit = new Journal_Entries()
        //                    {
        //                        Debit = 0,
        //                        Credit = res.Amount,
        //                        Inst_Bank = res.Bank,
        //                        Inst_No = res.Ch_Pay_Draft_No,
        //                        Status = "Approved",
        //                        Inst_Date = res.Ch_Pay_Draft_Date,
        //                        Head = creditAccount.Head,
        //                        Head_Id = creditAccount.Id,
        //                        Head_Code = creditAccount.Text_ChartCode,
        //                        Head_Name = creditAccount.Head,
        //                        Naration = "Amount Received By :" + res.File_Plot_Number + " - " + res.Text,
        //                        GroupId = Convert.ToInt64(res.Transaction_Id),
        //                        Line = 1,
        //                        Qty = null,
        //                        UOM = null,
        //                        Rate = null,
        //                        Recorded_By = userid,
        //                        RecordedBy_Date = date,
        //                        Supvise_by = userid,
        //                        Supviseby_Date = date,
        //                        Voucher_No = res.ReceiptNo,
        //                        Voucher_Type = "BRV",
        //                        Comp_Id = comp.Id
        //                    };
        //                    db.Journal_Entries.Add(Debit);
        //                    db.Journal_Entries.Add(Credit);
        //                    db.SaveChanges();

        //                    Transaction.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //    }
        //    else if (res.Module == Modules.CommercialManagement.ToString())
        //    {
        //        if (res.Type == ReceiptTypes.Booking.ToString() || res.Type == ReceiptTypes.Confirmation.ToString() || res.Type == ReceiptTypes.Installment.ToString() || res.Type == ReceiptTypes.Advance.ToString())
        //        {
        //            var projectid = db.Sp_Get_CommercialData(res.File_Plot_No).Select(x => x.ProjectId).FirstOrDefault();
        //            var debitAccount = Banks_Head(BankAccount, 9);
        //            var creditAccount = HeadFinder(COA_Mapper_Modules.Commercial.ToString(), COA_Mapper_ModuleTypes.Commercial_Project_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, projectid);
        //            using (var Transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
        //                    var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
        //                    Transaction.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //    }
        //    return Json(true);
        //}

        public JsonResult ApprovedInstrument(long TransactionId, string Status, long userid, long? RefId)
        {
            var comp = Company_Attr(userid);
            var res = db.Sp_Get_GeneralEntries_Parameter(TransactionId).ToList();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var v in res)
                    {
                        db.Sp_Add_Journal_Entry(v.Debit, v.Credit, v.Head, v.Head_Id, v.Head_Code, v.Head_Name, v.Naration, v.GroupId, v.Line, v.Recorded_By, userid, RefId, v.Inst_Bank, v.Inst_No, Status, v.Inst_Date, null, "", null, v.Voucher_No, v.Voucher_Type, comp.Id).FirstOrDefault();
                    }
                    db.Sp_Delete_GeneralEntry(TransactionId);
                    Transaction.Commit();
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    throw;
                }
            }
        }

        public JsonResult ApprovedInstrument_PDC(long? TransactionId, string Status, long userid, long? RefId,string BankAccount, string Voucher_No)
        {
            var comp = Company_Attr(userid);
            var res = db.Sp_Get_JournalEntries_Parameter_All(TransactionId).Where(x=> x.Credit > 0).FirstOrDefault();
            var debitAccount = Banks_Head(BankAccount, comp.Id);

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {

                    var res4 = db.Sp_Add_Journal_Entry(0, res.Credit, debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, res.Naration, TransactionId, 1, userid, userid, null, null, null, null, null, null, "", null, Voucher_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                    var res3 = db.Sp_Add_Journal_Entry(res.Credit, 0, res.Head, res.Head_Id, res.Head_Code, res.Head, res.Naration, TransactionId, 1, userid, userid, null, null, null, null, null, null, "", null, Voucher_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();


                    Transaction.Commit();
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    throw;
                }
            }
        }
        public JsonResult DishonerInstrument(long TransactionId, string Status, long userid, long? RefId)
        {
            var comp = Company_Attr(userid);
            var res = db.Sp_Get_GeneralEntries_Parameter(TransactionId).ToList();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var v in res)
                    {
                        db.Sp_Add_Journal_Entry(v.Debit, v.Credit, v.Head, v.Head_Id, v.Head_Code, v.Head_Name, v.Naration, v.GroupId, v.Line, v.Recorded_By, userid, RefId, v.Inst_Bank, v.Inst_No, Status, v.Inst_Date, null, "", null, v.Voucher_No, v.Voucher_Type, comp.Id).FirstOrDefault();
                    }
                    foreach (var v in res)
                    {
                        db.Sp_Add_Journal_Entry(v.Credit, v.Debit, v.Head, v.Head_Id, v.Head_Code, v.Head_Name, "Reversal" + " - " + v.Naration, v.GroupId, v.Line, v.Recorded_By, userid, RefId, v.Inst_Bank, v.Inst_No, Status, v.Inst_Date, null, "", null, v.Voucher_No, v.Voucher_Type, comp.Id).FirstOrDefault();
                    }
                    Transaction.Commit();
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    throw;
                }
            }
        }
        public JsonResult DishonerInstrument_PDC(long? TransactionId, string Status, long userid, long? RefId)
        {
            var comp = Company_Attr(userid);
            var res = db.Sp_Get_GeneralEntries_Parameter(TransactionId).ToList();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var v in res)
                    {
                        db.Sp_Add_Journal_Entry(v.Credit, v.Debit, v.Head, v.Head_Id, v.Head_Code, v.Head_Name, v.Naration, v.GroupId, v.Line, v.Recorded_By, userid, RefId, v.Inst_Bank, v.Inst_No, Status, v.Inst_Date, null, "", null, v.Voucher_No, v.Voucher_Type, comp.Id).FirstOrDefault();
                    }
                    db.Sp_Delete_GeneralEntry(TransactionId);
                    Transaction.Commit();
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    throw;
                }
            }
        }
        //public JsonResult DishonoredInstrument(decimal? Amount, string BankAccount, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Inst_Status, long TransactionId, long userid, int line, long? RefId)
        //{
        //    var Bank_Head = this.Banks_Head(BankAccount);
        //    var Other_Head = db.Sp_Get_JournalEntries_Parameter_All(RefId).Where(x => x.Credit > 0).FirstOrDefault();
        //    using (var Transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Other_Head.Head, Convert.ToInt32(Other_Head.Id), Other_Head.Head_Code, Other_Head.Head, "", TransactionId, line, userid, userid, RefId, Inst_Bank, Inst_No, Inst_Status, Inst_Date, null, null, null, null /* Voucher No */, Voucher_Type.JV.ToString()).FirstOrDefault();   /// For Cash and Bank Credit
        //            var Credit = db.Sp_Add_Journal_Entry(0, Amount, Bank_Head.Code + " - " + Bank_Head.Head, Bank_Head.Id, Bank_Head.Code, Bank_Head.Head, "", TransactionId, line, userid, userid, RefId, Inst_Bank, Inst_No, Inst_Status, Inst_Date, null, null, null, null /* Voucher No */, Voucher_Type.JV.ToString()).FirstOrDefault();
        //            var res1 = db.Sp_Update_InstrumentsApproval(RefId, Inst_Status);
        //            Transaction.Commit();
        //            return Json(new Return { Status = true, Msg = "Saved" });
        //        }
        //        catch (Exception ex)
        //        {
        //            Transaction.Rollback();
        //            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //            throw;
        //        }

        //    }


        //    return Json(true);
        //}

        //public JsonResult InventoryIssuance(long? Inv_Id, string Complete_Name, long? Prj_Id, string Prj_Name, string Dep_Name, DateTime? Inst_Date, string Inst_Bank, string Description, long TransactionId, long userid)
        //{
        //    var Inv = this.Inventory_Head(Inv_Id, Complete_Name);
        //    var Dealer_COA = this.Project_Head(Prj_Id, Prj_Name, Dep_Name);

        //    using (var Transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var Reciv_Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, "Pending", Inst_Date, Payment.Code + " - " + Payment.Head, Payment.Id, Payment.Code, Payment.Head, Description, TransactionId, line, null, null, null, userid).FirstOrDefault();
        //            var Receivable_Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, "Pending", Inst_Date, Dealer_COA.Code + " - " + Dealer_COA.Head, Dealer_COA.Id, Dealer_COA.Code, Dealer_COA.Head, Description, TransactionId, line, null, null, null, userid).FirstOrDefault();   /// For Cash and Bank Credit
        //            Transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            EmailService e = new EmailService();
        //            e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Installment Error");
        //            var res = new { Status = false, Msg = "Installment Cannot Be Received. Contact SA Systems" };
        //        }
        //    }
        //    return Json(true);
        //}
        public JsonResult Refund_Plot_Amount(decimal Amount, string Plot_No, string Type, string Block, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string VoucherNo, int line, bool headcashier, string Module, long? BlockId)
        {
            var comp = Company_Attr(userid);
            string ReceivableModule = "";
            if (Type == PlotType.Commercial.ToString())
            {
                ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();
            }
            else
            {
                ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
            }
                var debitAcc = HeadFinder(Module, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, BlockId);//this.Plot_Head(Plot_No, Type, Block);
                var creditAcc = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Refund Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, VoucherNo, creditAcc.VoucherType,comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Refund Amount Against Plot Number :" + Plot_No + " - " + Type + " Block: " + Block, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, VoucherNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }

        }


        public Sp_Get_COA_Head_Code_Result Inventory_Head(long? Id, string Complete_Name, int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                           x.CompanyId == Comp_Id &&
                           x.Module == COA_Mapper_Modules.Procurement.ToString() &&
                           x.ModuleType == COA_Mapper_ModuleTypes.Item_List.ToString() &&
                           x.Module_Id == Id
                           ).FirstOrDefault();

            var Item_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Item_COA;
        }

        public PaymentTypeModel Payment_Head_Salary(string PaymentType, string Debit_Credit, string Headname)
        {
            var Payment = HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Cash_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), 1, null);//db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").FirstOrDefault();
            var vt = (Debit_Credit == "Debit") ? Voucher_Type.CRV.ToString() : Voucher_Type.CPV.ToString();
            var res = new PaymentTypeModel { PaymentStatus = null, PaymentData = Payment, VoucherType = vt };
            return res;
        }


        //public Sp_Get_CA_Head_MultiRef_3_Result BlockHeadIncome(string Block)
        //{
        //    var a = db.Sp_Get_CA_Head_MultiRef_3("Income", "Plots  Sales", Block + " Block Sales").FirstOrDefault();
        //    if (a == null)
        //    {
        //        var b = db.Sp_Get_CA_Head_MultiRef("Income", "Plots  Sales").FirstOrDefault();
        //        db.Sp_Add_CA_Head(b.Id, Block + " Block Sales", "", null);
        //        var c = db.Sp_Get_CA_Head_MultiRef_3("Income", "Plots  Sales", Block + " Block Sales").FirstOrDefault();
        //        return c;
        //    }
        //    return a;
        //}
        //public Sp_Get_CA_Head_MultiRef_3_Result SuspenceAccountHead()
        //{
        //    var acc = db.Sp_Get_CA_Head_MultiRef_3("Equity", "Shareholder Funds", "Suspense Account").FirstOrDefault();
        //    if (acc == null)
        //    {
        //        var a = db.Sp_Get_CA_Head_MultiRef("Equity", "Shareholder Funds").FirstOrDefault();
        //        db.Sp_Add_CA_Head(a.Id, "Suspense Account", "", null);
        //        var b = db.Sp_Get_CA_Head_MultiRef_3("Equity", "Shareholder Funds", "Suspense Account").FirstOrDefault();
        //        return b;
        //    }
        //    return acc;
        //}
        // Get Vendor
        public Sp_Get_COA_Head_Code_Result Vendor_Head(string Vendor, long? Id, int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Vendors_List.ToString() &&
                            x.Module_Id == Id
                            ).FirstOrDefault();
            if (res == null)
            {
                // General Suppliers ka head lay ao.
                var Vendor_COA = this.GeneralSuppliers_Head(Comp_Id);
                return Vendor_COA;
            }
            else
            {
                var Vendor_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
                return Vendor_COA;
            }
        }
        public Sp_Get_COA_Head_Code_Result VendorAccount_Head(long userid)
        {
            var comp = this.Company_Attr(userid);
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == comp.Id &&
                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Vendor.ToString()
                            ).FirstOrDefault();
            var Vendor_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Vendor_COA;
        }
        public void RegisterVendor(long? Vendorid, string VendorName, long userid)
        {
            var comp = this.Company_Attr(userid);
            var res1 = db.COA_Mapper.Where(x =>
                            x.CompanyId == comp.Id &&
                            x.AccountType == COA_Mapper_HeadType.Account_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Vendor.ToString()
                            ).FirstOrDefault();

            var res2 = db.Sp_Add_CA_Head((int)res1.AccountId, VendorName, "Vendor", null, COA_Nature.Liability.ToString()).FirstOrDefault();
            var res3 = db.Sp_Get_CA_Head_Parameter(res2).FirstOrDefault();
            this.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
                                           COA_Mapper_Modules.Procurement.ToString(),
                                           COA_Mapper_ModuleTypes.Vendors_List.ToString(),
                                           Vendorid,
                                           res3.Id,
                                           userid
                                           );
        }

        public void RegisterDealership(long? Dealerid, string DealerName, long userid)
        {
            var comp = this.Company_Attr(userid);
            var res1 = db.COA_Mapper.Where(x =>
                            x.CompanyId == comp.Id &&
                            x.AccountType == COA_Mapper_HeadType.Account_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Dealership.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Dealership.ToString()
                            ).FirstOrDefault();
            var res2 = db.Sp_Add_CA_Head((int)res1.AccountId, DealerName, "Dealer", null, COA_Nature.Liability.ToString()).FirstOrDefault();
            this.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
                                           COA_Mapper_Modules.Dealership.ToString(),
                                           COA_Mapper_ModuleTypes.Dealership_List.ToString(),
                                           Dealerid,
                                           res2,
                                           userid);
        }
        public void AddCOA_Mapper(string HeadType, string Modules, string ModuleTypes, long? ModuleId, int? COA_Id, long userid)
        {
            var comp = this.Company_Attr(userid);
            var res3 = db.Sp_Get_CA_Head_Parameter(COA_Id).FirstOrDefault();
            var res4 = db.Sp_Add_COAMapper(HeadType, Modules, ModuleTypes, ModuleId, res3.Id.ToString(), res3.Text_ChartCode, res3.Head, comp.Id, comp.Company_Name);
        }


        public Sp_Get_COA_Head_Code_Result Advance_TradePayable(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Advance_Trade_payable.ToString()
                            ).FirstOrDefault();

            var advance_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return advance_COA;
        }
        // Get Bank

        public Sp_Get_CA_Head_Parameter_Result GetHead(int Id)
        {
            var res = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
            return res;
        }


        //public Sp_Get_CA_Head_MultiRef_4_Result SubsidiaryAccount(string module)
        //{
        //    var a = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Long Term Investment", "Subsidiary", "Recovery - " + module).FirstOrDefault();
        //    if (a == null)
        //    {
        //        var b = db.Sp_Get_CA_Head_MultiRef_Fixed_Param_4("Assets", "Long Term Investment", "Subsidiary", module).FirstOrDefault();
        //        db.Sp_Add_CA_Head(b.Id, "Recovery - " + module, "", null);
        //        var c = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Long Term Investment", "Subsidiary", "Recovery - " + module).FirstOrDefault();
        //        return c;
        //    }
        //    return a;
        //}
        //Get Salaries Accounts


        //Salary Disbursement

        //public JsonResult DisburseSalary(decimal? Amount, string name, string EmpCode, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string Voucher_No, int line, bool headcashier, string Module, int? ModuleId)
        //{
        //    var comp = Company_Attr(userid);
        //    if (headcashier)
        //    {
        //        var debitAcc = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
        //        var creditAcc = Payment_Head_Salary(PaymentType, Transaction_Type.Credit.ToString(),  comp.Id, Inst_Bank);
        //        using (var Transaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                //var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Salary Received by "+ name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType).FirstOrDefault();
        //                //var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType).FirstOrDefault();   /// For Cash and Bank Credit
        //                var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
        //                var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
        //                Transaction.Commit();
        //                return Json(new Return { Status = true, Msg = "Saved" });
        //            }
        //            catch (Exception ex)
        //            {
        //                Transaction.Rollback();
        //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                throw;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var debitAcc = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
        //        var creditAcc = Payment_Head_Salary(PaymentType, Transaction_Type.Credit.ToString(),  comp.Id, Inst_Bank);
        //        using (var Transaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
        //                var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
        //                Transaction.Commit();
        //                return Json(new Return { Status = true, Msg = "Saved" });
        //            }
        //            catch (Exception ex)
        //            {
        //                Transaction.Rollback();
        //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                throw;
        //            }
        //        }
        //    }
        //}
        public JsonResult DisburseArrearSalary(decimal? Amount, string name, string EmpCode, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string Voucher_No, int line, bool headcashier, string Module, int? ModuleId)
        {
            var comp = Company_Attr(userid);
            var debitAcc = HeadFinder(Module, COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, ModuleId);
            var creditAcc = new PaymentTypeModel();
            if (PaymentType == "Cash")
            {
                creditAcc = Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
            }
            else
            {
                var bankHead = GetHead(Convert.ToInt32(Inst_Bank));
                var bankmod = db.COA_Mapper.Where(x => x.AccountId == bankHead.Id).FirstOrDefault();
                creditAcc = Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), Convert.ToInt32(bankmod.Module_Id), comp.Id);
            }
            if (headcashier)
            {
               

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Salary Received by " + name + " - " + EmpCode + ".", TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }


        // Asset Transactions

        public Sp_Get_CA_Head_MultiRef_4_Result Asset_Account(string accountname)
        {
            var FAAccount = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", accountname).FirstOrDefault();
            if (FAAccount == null)
            {
                var a = db.Sp_Get_CA_Head_MultiRef_3("Assets", "Non Current Assets", "Fixed Assets").FirstOrDefault();
                if (a == null)
                {
                    var b = db.Sp_Get_CA_Head_MultiRef("Assets", "Non Current Assets").FirstOrDefault();
                    db.Sp_Add_CA_Head(b.Id, "Fixed Assets", "", null, COA_Nature.Assets.ToString());
                    var c = db.Sp_Get_CA_Head_MultiRef_3("Assets", "Non Current Assets", "Fixed Assets").FirstOrDefault();
                    db.Sp_Add_CA_Head(c.Id, accountname, "", null, COA_Nature.Assets.ToString());
                    var d = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", accountname).FirstOrDefault();
                    return d;
                }
                else
                {
                    db.Sp_Add_CA_Head(a.Id, accountname, "", null, COA_Nature.Assets.ToString());
                    var b = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", accountname).FirstOrDefault();
                    return b;
                }
            }
            return FAAccount;
        }
        public Sp_Get_CA_Head_MultiRef_4_Result Asset_Depreciation_Account(string accountname)
        {
            var depAccount = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", "Accumulated Depreciation-" + accountname).FirstOrDefault();
            if (depAccount == null)
            {
                var a = db.Sp_Get_CA_Head_MultiRef_3("Assets", "Non Current Assets", "Fixed Assets").FirstOrDefault();
                if (a == null)
                {
                    var b = db.Sp_Get_CA_Head_MultiRef("Assets", "Non Current Assets").FirstOrDefault();
                    db.Sp_Add_CA_Head(b.Id, "Fixed Assets", "", null, COA_Nature.Assets.ToString());
                    var c = db.Sp_Get_CA_Head_MultiRef_3("Assets", "Non Current Assets", "Fixed Assets").FirstOrDefault();
                    db.Sp_Add_CA_Head(c.Id, "Accumulated Depreciation-" + accountname, "", null, COA_Nature.Assets.ToString());
                    var d = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", "Accumulated Depreciation-" + accountname).FirstOrDefault();
                    return d;
                }
                else
                {
                    db.Sp_Add_CA_Head(a.Id, "Accumulated Depreciation-" + accountname, "", null, COA_Nature.Assets.ToString());
                    var b = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", "Accumulated Depreciation-" + accountname).FirstOrDefault();
                    return b;
                }
            }
            return depAccount;
        }
        public Sp_Get_CA_Head_MultiRef_Result DepreciationExpenseAccount()
        {
            var account = db.Sp_Get_CA_Head_MultiRef("Expenses", "Depriciation").FirstOrDefault();
            return account;
        }
        public Sp_Get_CA_Head_MultiRef_3_Result GainLossAccount()
        {
            var gain = db.Sp_Get_CA_Head_MultiRef_3("Income", "Other Income", "Gain or Loss on Assets").FirstOrDefault();
            if (gain == null)
            {
                var a = db.Sp_Get_CA_Head_MultiRef("Income", "Other Income").FirstOrDefault();
                db.Sp_Add_CA_Head(a.Id, "Gain or Loss on Assets", "", null, COA_Nature.Income.ToString());
                var b = db.Sp_Get_CA_Head_MultiRef_3("Income", "Other Income", "Gain or Loss on Assets").FirstOrDefault();
                return b;
            }
            return gain;
        }
        public JsonResult Asset_Creation(string assetName, string assetRef, DateTime? purchaseDate, decimal? purchaseVal, long transactionId, string groupName, long userId, string voucherNo, bool headcashier)
        {
            var comp = Company_Attr(userId);
            if (headcashier)
            {
                var debitAcc = Asset_Account(groupName);
                var creditAcc = db.Sp_Get_CA_Head_MultiRef("Liability", "Other General Payables").FirstOrDefault();
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(purchaseVal, 0, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Purchased Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, "", "", "Pending", null, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, purchaseVal, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, "Purchased Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, "", "", "Pending", null, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = Asset_Account(groupName);
                var creditAcc = db.Sp_Get_CA_Head_MultiRef("Liability", "Other General Payables").FirstOrDefault();
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(purchaseVal, 0, "", "", "Pending", purchaseDate, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Purchased Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, purchaseVal, "", "", "Pending", purchaseDate, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, "Purchased Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }


        }
        public JsonResult Asset_Disposal_Scrapped(string assetName, string assetRef, DateTime? dateval, decimal? amount, long transactionId, string groupName, long userId, string voucherNo, bool headcashier)
        {
            var comp = Company_Attr(userId);
            if (headcashier)
            {
                var debitAcc = Asset_Depreciation_Account(groupName);
                var creditAcc = Asset_Account(groupName);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(amount, 0, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Scrapped Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, "", "", "Pending", null, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, amount, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, "Scrapped Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, "", "", "Pending", null, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = Asset_Depreciation_Account(groupName);
                var creditAcc = Asset_Account(groupName);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(amount, 0, "", "", "Pending", dateval, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Scrapped Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, amount, "", "", "Pending", dateval, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, "Scrapped Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }

        }
        public JsonResult Asset_Disposal_Sell_Off(string assetName, string assetRef, DateTime? dateval, decimal? amount, long transactionId, string groupName, long userId, string voucherNo, bool headcashier)
        {
            var comp = Company_Attr(userId);
            if (headcashier)
            {
                var debitAcc = Asset_Depreciation_Account(groupName);
                var creditAcc = Asset_Account(groupName);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(amount, 0, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, "", "", "Pending", null, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, amount, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, "", "", "Pending", null, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = Asset_Depreciation_Account(groupName);
                var creditAcc = Asset_Account(groupName);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(amount, 0, "", "", "Pending", dateval, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, amount, "", "", "Pending", dateval, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }

        }
        public JsonResult AssetSellOffProfit(string assetName, string assetRef, decimal? amount, decimal? profit, string paytype, string instNo, DateTime? instDate, string bank, long transactionId, string groupName, long userId, string voucherNo, bool headcashier)
        {
            var comp = Company_Attr(userId);
            if (headcashier)
            {
                if (paytype == "Cash")
                {
                    var debitAcc = Payment_Head(paytype, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var creditAccPr = GainLossAccount();
                    var creditAccFa = Asset_Account(groupName);

                    var fixasset = amount - profit;

                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(amount, 0, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, "Sell Off Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, bank, instNo, debitAcc.PaymentStatus, instDate, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, profit, creditAccPr.Code + " - " + creditAccPr.Head, creditAccPr.Id, creditAccPr.Code, creditAccPr.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, bank, instNo, debitAcc.PaymentStatus, instDate, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Credit1 = db.Sp_Add_Journal_Entry(0, fixasset, creditAccFa.Code + " - " + creditAccFa.Head, creditAccFa.Id, creditAccFa.Code, creditAccFa.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 2, userId, userId, null, bank, instNo, debitAcc.PaymentStatus, instDate, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (paytype == "Cash")
                {
                    var debitAcc = Payment_Head(paytype, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var creditAccPr = GainLossAccount();
                    var creditAccFa = Asset_Account(groupName);

                    var fixasset = amount - profit;

                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(amount, 0, bank, instNo, "Pending", instDate, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, "Sell Off Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, profit, bank, instNo, "Pending", instDate, creditAccPr.Code + " - " + creditAccPr.Head, creditAccPr.Id, creditAccPr.Code, creditAccPr.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 2, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Credit1 = db.Sp_Add_General_Entry(0, fixasset, bank, instNo, "Pending", instDate, creditAccFa.Code + " - " + creditAccFa.Head, creditAccFa.Id, creditAccFa.Code, creditAccFa.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 3, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }

        }
        public JsonResult AssetSellOffLoss(string assetName, string assetRef, decimal? amount, decimal? loss, string paytype, string instNo, DateTime? instDate, string bank, long transactionId, string groupName, long userId, string voucherNo, bool headcashier)
        {
            var comp = Company_Attr(userId);
            if (headcashier)
            {
                if (paytype == "Cash")
                {
                    var debitAcc = Payment_Head(paytype, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var debitAccPr = GainLossAccount();
                    var creditAccFa = Asset_Account(groupName);

                    var fixasset = amount + loss;

                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(amount, 0, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, "Sell Off Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, bank, instNo, debitAcc.PaymentStatus, instDate, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Debit1 = db.Sp_Add_Journal_Entry(loss, 0, debitAccPr.Code + " - " + debitAccPr.Head, debitAccPr.Id, debitAccPr.Code, debitAccPr.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 2, userId, userId, null, bank, instNo, debitAcc.PaymentStatus, instDate, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, fixasset, creditAccFa.Code + " - " + creditAccFa.Head, creditAccFa.Id, creditAccFa.Code, creditAccFa.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, userId, userId, null, bank, instNo, debitAcc.PaymentStatus, instDate, null, "", null, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (paytype == "Cash")
                {
                    var debitAcc = Payment_Head(paytype, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var debitAccPr = GainLossAccount();
                    var creditAccFa = Asset_Account(groupName);

                    var fixasset = amount + loss;

                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(amount, 0, bank, instNo, debitAcc.PaymentStatus, instDate, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, "Sell Off Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 1, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Debit1 = db.Sp_Add_General_Entry(loss, 0, bank, instNo, debitAcc.PaymentStatus, instDate, debitAccPr.Code + " - " + debitAccPr.Head, debitAccPr.Id, debitAccPr.Code, debitAccPr.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 2, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, fixasset, bank, instNo, debitAcc.PaymentStatus, instDate, creditAccFa.Code + " - " + creditAccFa.Head, creditAccFa.Id, creditAccFa.Code, creditAccFa.Head, "Sell Off Depreciation Against Asset " + assetName + ". Reference No. " + assetRef, transactionId, 3, null, null, null, userId, voucherNo, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }

        }

        // Admin Facilities

        public JsonResult Admin_Facilities_FeeReceived(decimal? Amount, string name, string contact, string project, long? TransactionId, long userid, int line, string voucher_No, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var debitAcc = Payment_Head("Cash", Transaction_Type.Debit.ToString(),null, comp.Id);
                var creditAcc = this.Gym_Head(comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, "Fee Received against " + project + " By: " + name + " - " + contact, TransactionId, line, userid, userid, null, "", "", "Pending", null, null, "", null, voucher_No, debitAcc.VoucherType.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.Head, creditAcc.Id, creditAcc.Text_ChartCode, creditAcc.Head, "Fee Received against " + project + " By: " + name + " - " + contact, TransactionId, line, userid, userid, null, "", "", "Pending", null, null, "", null, voucher_No, debitAcc.VoucherType.ToString(), comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Fee Received of Amount: " + Amount });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = Payment_Head("Cash", Transaction_Type.Debit.ToString(),null, comp.Id);
                var creditAcc = this.Gym_Head(comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, "", "", "Pending", null, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, "Fee Received against " + project + " By: " + name + " - " + contact, TransactionId, line, null, null, null, userid, voucher_No, debitAcc.VoucherType.ToString(), comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, "", "", "Pending", null, creditAcc.Head, creditAcc.Id, creditAcc.Text_ChartCode, creditAcc.Head, "Fee Received against " + project + " By: " + name + " - " + contact, TransactionId, line, null, null, null, userid, voucher_No, debitAcc.VoucherType.ToString(), comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Fee Received of Amount: " + Amount });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }

        // Receipt

        public JsonResult Receipts_Architecture_Receipt(decimal? Amount, string name, string cnic, string contact, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Mis_Income_COA = this.ArchiHead(comp.Id);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Remarks + " from " + name + " - " + cnic + " - " + contact, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, Mis_Income_COA.Head, Mis_Income_COA.Id, Mis_Income_COA.Text_ChartCode, Mis_Income_COA.Head, Remarks + " from " + name + " - " + cnic + " - " + contact, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Mis_Income_COA = this.ArchiHead(comp.Id);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Remarks + " from " + name + " - " + cnic + " - " + contact, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Mis_Income_COA.Head, Mis_Income_COA.Id, Mis_Income_COA.Text_ChartCode, Mis_Income_COA.Head, Remarks + " from " + name + " - " + cnic + " - " + contact, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }

        }
        public JsonResult Receipts_Architecture_Voucher(decimal? Amount, string name, string cnic, string contact, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var debitAcc = this.ArchiHead(comp.Id);
                var creditAcc = Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, Remarks + " by " + name + " - " + cnic + " - " + contact, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, Remarks + " by " + name + " - " + cnic + " - " + contact, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = this.ArchiHead(comp.Id);

                var creditAcc = Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, Remarks + " by " + name + " - " + cnic + " - " + contact, TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, Remarks + " by " + name + " - " + cnic + " - " + contact, TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }

        }

        // Loans

        public JsonResult Loan_Generate_Loan(decimal? Amount, string Loan_type, string name, string cnic, string contact, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var debitAcc = this.LoanAdvanceHead(comp.Id, Loan_type);
                var creditAcc = Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, Remarks + " by " + name + " - " + cnic + " - " + contact, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, Remarks + " by " + name + " - " + cnic + " - " + contact, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = this.LoanAdvanceHead(comp.Id, Loan_type);
                var creditAcc = Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, Remarks + " by " + name + " - " + cnic + " - " + contact, TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, Remarks + " by " + name + " - " + cnic + " - " + contact, TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }

        }
        //public JsonResult Loan_Received_Installment(decimal? Amount, long empId, string name, string cnic, string contact, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        //{
        //    var comp = Company_Attr(userid);
        //    if (headcashier)
        //    {
        //        if (PaymentType == "Cash")
        //        {
        //            var debitAcc = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
        //            var creditAcc = EmployeeLoanHead(empId.ToString());
        //            using (var Transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, Remarks + " from " + name + " - " + cnic + " - " + contact, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
        //                    var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, Remarks + " from " + name + " - " + cnic + " - " + contact, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
        //                    Transaction.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return Json(new Return { Status = true, Msg = "Saved" });
        //        }
        //    }
        //    else
        //    {
        //        if (PaymentType == "Cash")
        //        {
        //            var debitAcc = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
        //            var creditAcc = EmployeeLoanHead(empId.ToString());
        //            using (var Transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, Remarks + " from " + name + " - " + cnic + " - " + contact, TransactionId, line, null, null, null, userid, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
        //                    var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, Remarks + " from " + name + " - " + cnic + " - " + contact, TransactionId, line, null, null, null, userid, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
        //                    Transaction.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return Json(new Return { Status = true, Msg = "Saved" });
        //        }
        //    }

        //}

        // Receipts Updation / Deletion

        //public JsonResult Banking_Receipts_Deletion(long receipt_Id, long userid)
        //{
        //    //Reevaluate

        //    var transId = new Helpers().RandomNumber();
        //    var vouch = db.Sp_Add_Journal_Voucher().FirstOrDefault();
        //    var res1 = db.Receipts.Where(x => x.Id == receipt_Id).FirstOrDefault();
        //    var res2 = db.Journal_Entries.Where(x => x.Voucher_No == res1.ReceiptNo).ToList();
        //    if (res2 == null)
        //    {
        //        var res3 = db.General_Entries.Where(x => x.Voucher_No == res1.ReceiptNo).ToList();
        //        if (res3 == null)
        //        {
        //            var debitAcc = Plot_Head(res1.File_Plot_Number, res1.Plot_Type, res1.Block);
        //            var creditAcc = SuspenceAccountHead();
        //            var tran = new Helpers().RandomNumber();
        //            var jv = db.Sp_Add_Journal_Voucher().FirstOrDefault();
        //            using (var Transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var Debit = db.Sp_Add_General_Entry(res1.Amount, 0, res1.Bank, res1.Ch_Pay_Draft_No, "Pending", res1.Ch_Pay_Draft_Date, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, "Reversal - " + res1.ReceiptNo + " - " + res1.Text + " of " + res1.Name + " - " + res1.Father_Name + " - " + res1.Contact, tran, 1, null, null, null, userid, jv, Voucher_Type.JV.ToString()).FirstOrDefault();
        //                    var Credit = db.Sp_Add_General_Entry(0, res1.Amount, res1.Bank, res1.Ch_Pay_Draft_No, "Pending", res1.Ch_Pay_Draft_Date, creditAcc.Code + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Code, creditAcc.Head, "Reversal - " + res1.ReceiptNo + " - " + res1.Text + " of " + res1.Name + " - " + res1.Father_Name + " - " + res1.Contact, tran, 1, null, null, null, userid, jv, Voucher_Type.JV.ToString()).FirstOrDefault();   /// For Cash and Bank Credit
        //                    Transaction.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            using (var Transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    foreach (var a in res3)
        //                    {
        //                        var trans = db.Sp_Add_General_Entry(a.Credit, a.Debit, a.Inst_Bank, a.Inst_No, a.Inst_Status, a.Inst_Date, a.Head, a.Head_Id, a.Head_Code, a.Head_Name, "Reversal - " + a.Naration, transId, 1, null, null, null, userid, vouch.ToString(), Voucher_Type.JV.ToString()).FirstOrDefault();
        //                    }
        //                    Transaction.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Transaction.Rollback();
        //                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        using (var Transaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                foreach (var a in res2)
        //                {
        //                    var trans = db.Sp_Add_General_Entry(a.Credit, a.Debit, a.Inst_Bank, a.Inst_No, a.Inst_Status, a.Inst_Date, a.Head, a.Head_Id, a.Head_Code, a.Head_Name, "Reversal - " + a.Naration, transId, 1, null, null, null, userid, vouch.ToString(), Voucher_Type.JV.ToString()).FirstOrDefault();
        //                }
        //                Transaction.Commit();
        //                return Json(new Return { Status = true, Msg = "Saved" });
        //            }
        //            catch (Exception ex)
        //            {
        //                Transaction.Rollback();
        //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                throw;
        //            }
        //        }
        //    }
        //}
        public JsonResult Banking_Receipts_Updation_Wiith_Deletion(long receipt_Id, decimal? Amount, string name, string contact, string payType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string filePlotno, string fptype, string voucher, string Block)
        {
            int userid = int.Parse(User.Identity.GetUserId());
            var comp = Company_Attr(userid);
            var trans = new Helpers().RandomNumber();
            var res1 = db.Receipts.Where(x => x.Id == receipt_Id).FirstOrDefault();
            var res2 = db.General_Entries.Where(x => x.Voucher_No == res1.ReceiptNo).ToList();
            string payStatus = "", recType = "";
            if (payType == "Cash")
            {
                payStatus = "";
                recType = Voucher_Type.CRV.ToString();
            }
            else
            {
                payStatus = "Pending";
                recType = Voucher_Type.BRV.ToString();
            }
            if (res2 is null)
            {
                var res3 = db.Journal_Entries.Where(x => x.Voucher_No == res1.ReceiptNo).ToList();

                if (res3 is null)
                {
                    return Json(new { Status = false, Msg = "Receipt Reversal Cannot Be Made Right Now. Contact SA Systems" });
                }
                else
                {
                    //Banking_Receipts_Deletion(receipt_Id, userid);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, payStatus, Inst_Date, res3[0].Head, res3[0].Head_Id, res3[0].Head_Code, res3[0].Head_Name, "Receipt Update - Amount Against Plot Number :" + filePlotno + " - " + fptype + " Block: " + Block, trans, 1, null, null, null, userid, voucher, recType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, payStatus, Inst_Date, res3[1].Head, res3[1].Head_Id, res3[1].Head_Code, res3[1].Head_Name, "Receipt Update - Amount Against Plot Number :" + filePlotno + " - " + fptype + " Block: " + Block, trans, 1, null, null, null, userid, voucher, recType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
            }
            else
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Banking_Receipts_Deletion(receipt_Id, userid);
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, payStatus, Inst_Date, res2[0].Head, res2[0].Head_Id, res2[0].Head_Code, res2[0].Head_Name, "Receipt Update - Amount Against Plot Number :" + filePlotno + " - " + fptype + " Block: " + Block, trans, 1, null, null, null, userid, voucher, recType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, payStatus, Inst_Date, res2[1].Head, res2[1].Head_Id, res2[1].Head_Code, res2[1].Head_Name, "Receipt Update - Amount Against Plot Number :" + filePlotno + " - " + fptype + " Block: " + Block, trans, 1, null, null, null, userid, voucher, recType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }
        public JsonResult Banking_Receipts_Update(long receipt_Id, string payType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long userid)
        {
            var comp = Company_Attr(userid);
            var trans = new Helpers().RandomNumber();
            var res1 = db.Receipts.Where(x => x.Id == receipt_Id).FirstOrDefault();
            var res2 = db.General_Entries.Where(x => x.Voucher_No == res1.ReceiptNo).ToList();
            string payStatus = "", recType = "";
            if (payType == "Cash")
            {
                payStatus = "";
                recType = Voucher_Type.CRV.ToString();
            }
            else
            {
                payStatus = "Pending";
                recType = Voucher_Type.BRV.ToString();
            }
            if (res2 is null)
            {
                var res3 = db.Journal_Entries.Where(x => x.Voucher_No == res1.ReceiptNo).ToList();

                if (res3 is null)
                {
                    return Json(new { Status = false, Msg = "Receipt Reversal Cannot Be Made Right Now. Contact SA Systems" });
                }
                else
                {
                    //Banking_Receipts_Deletion(receipt_Id, userid);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(res3[0].Debit, res3[0].Credit, Inst_Bank, Inst_No, payStatus, Inst_Date, res3[0].Head, res3[0].Head_Id, res3[0].Head_Code, res3[0].Head_Name, "Receipt Update - " + res3[0].Naration, trans, 1, null, null, null, userid, res3[0].Voucher_No, recType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(res3[1].Debit, res3[1].Credit, Inst_Bank, Inst_No, payStatus, Inst_Date, res3[1].Head, res3[1].Head_Id, res3[1].Head_Code, res3[1].Head_Name, "Receipt Update - " + res3[1].Naration, trans, 1, null, null, null, userid, res3[1].Voucher_No, recType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
            }
            else
            {
                //Banking_Receipts_Deletion(receipt_Id, userid);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(res2[0].Debit, res2[0].Credit, Inst_Bank, Inst_No, payStatus, Inst_Date, res2[0].Head, res2[0].Head_Id, res2[0].Head_Code, res2[0].Head_Name, "Receipt Update - " + res2[0].Naration, trans, 1, null, null, null, userid, res2[0].Voucher_No, recType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(res2[1].Debit, res2[1].Credit, Inst_Bank, Inst_No, payStatus, Inst_Date, res2[1].Head, res2[1].Head_Id, res2[1].Head_Code, res2[1].Head_Name, "Receipt Update - " + res2[1].Naration, trans, 1, null, null, null, userid, res2[1].Voucher_No, recType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }

        // Finance

        public JsonResult Finance_Release_Voucher_Payments(decimal? Amount, string BlkTypPlot, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var debitAcc = db.Sp_Get_CA_Head_MultiRef_4("Liability", "Current Liability", "Other Payables", "Other General Payables").FirstOrDefault();
                var creditAcc = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, Remarks + " :" + BlkTypPlot, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, Remarks + " :" + BlkTypPlot, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = db.Sp_Get_CA_Head_MultiRef_4("Liability", "Current Liability", "Other Payables", "Other General Payables").FirstOrDefault();
                var creditAcc = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, Remarks + " :" + words[2] + " - " + words[1] + " - " + words[0], TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType).FirstOrDefault();
                        //var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, Remarks + " :" + words[2] + " - " + words[1] + " - " + words[0], TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType).FirstOrDefault();   /// For Cash and Bank Credit
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Code + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Code, debitAcc.Head, Remarks + " :" + BlkTypPlot, TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, Remarks + " :" + BlkTypPlot, TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }


        }

        // General Entry Controller
        public JsonResult GE_Release_Payable_Payments(decimal? Amount, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, int debitAccId, bool headcashier)
        {
            var comp = Company_Attr(userid);
            var debitAcc = GetHead(debitAccId);
            var creditAcc = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, Remarks, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                    var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, Remarks, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                    Transaction.Commit();
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    throw;
                }
            }
        }

        //SAM Accounts

        public JsonResult SAM_Installment(decimal? Amount, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string Voucher_No, int line, string dealNumber, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var debitAcc = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var creditAcc = SAMHead(comp.Id);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, "Amount Received against Deal Number: " + dealNumber, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.Head, creditAcc.Id, creditAcc.Text_ChartCode, creditAcc.Head, "Amount Received against Deal Number: " + dealNumber, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var debitAcc = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var creditAcc = SAMHead(comp.Id);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, "Amount Received against Deal Number: " + dealNumber, TransactionId, line, null, null, null, userid, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, creditAcc.Head, creditAcc.Id, creditAcc.Text_ChartCode, creditAcc.Head, "Amount Received against Deal Number: " + dealNumber, TransactionId, line, null, null, null, userid, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }

        }

        public JsonResult SAM_Vouchers(decimal? Amount, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, long TransactionId, long userid, string Voucher_No, int line, string dealNumber, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                var debitAcc = SAMHead(comp.Id);
                var creditAcc = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Amount Paid against Deal Number: " + dealNumber, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Amount Paid against Deal Number: " + dealNumber, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                var debitAcc = SAMHead(comp.Id);
                var creditAcc = this.Payment_Head(PaymentType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, "Amount Paid against Deal Number: " + dealNumber, TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, creditAcc.PaymentStatus, Inst_Date, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, "Amount Paid against Deal Number: " + dealNumber, TransactionId, line, null, null, null, userid, Voucher_No, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }

        }
        public JsonResult SAM_Receipts_Cancel(long ReceiptId, string leaddeal)
        {
            try
            {
                var res = db.Sp_Get_MarketingReceipt_Parameter_ById(ReceiptId, leaddeal).FirstOrDefault();
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                throw;
            }
            return Json(true);
        }

        //Patty Cash


        public void PattyCashAccount(long? PettAcc_Id, string Emp_Name, long userid)
        {
            var comp = this.Company_Attr(userid);
            var res1 = db.COA_Mapper.Where(x =>
                            x.CompanyId == comp.Id &&
                            x.AccountType == COA_Mapper_HeadType.Account_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Petty_Cash.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Petty_Cash_Account.ToString()
                            ).FirstOrDefault();
            var res2 = db.Sp_Add_CA_Head((int)res1.AccountId, Emp_Name, "Petty Cast", null, COA_Nature.Assets.ToString()).FirstOrDefault();
            this.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
                                           COA_Mapper_Modules.Petty_Cash.ToString(),
                                           COA_Mapper_ModuleTypes.Petty_Cash_Account.ToString(),
                                           PettAcc_Id,
                                           res2,
                                           userid);
        }

        public JsonResult PattyCashPaymentReleaseEntry(string desc, string receivedby, int payeeId, int payaccId, decimal Amount, long TransId, string voucherNo, string instNo, DateTime? instDate, long userid, bool headCashier)
        {
            var comp = Company_Attr(userid);
            var payee = db.PattyCash_Accounts.Where(x => x.Employee_Id == payeeId).FirstOrDefault();
            var res2 = db.COA_Mapper.Where(x =>
                       x.CompanyId == comp.Id &&
                       x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                       x.Module == COA_Mapper_Modules.Petty_Cash.ToString() &&
                       x.ModuleType == COA_Mapper_ModuleTypes.Petty_Cash_Account.ToString() &&
                       x.Module_Id == payee.Id
                       ).FirstOrDefault();
            var debitAcc = GetHead(Convert.ToInt32(res2.AccountId));
            var creditAcc = GetHead(payaccId);
            string bank = "";
            string vouchType = "CPV";
            
            if (headCashier)
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, desc + ". Received By: " + receivedby, TransId, 1, userid, userid, null, bank, instNo, "Approved", instDate, null, "", null, voucherNo, vouchType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.Text_ChartCode + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Text_ChartCode, creditAcc.Head, desc + ". Received By: " + receivedby, TransId, 1, userid, userid, null, bank, instNo, "Approved", instDate, null, "", null, voucherNo, vouchType, comp.Id).FirstOrDefault();
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Debit = db.Sp_Add_General_Entry(Amount, 0, bank, instNo, "Pending", instDate, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, desc + ". Received By: " + receivedby, TransId, 1, null, null, null, userid, voucherNo, vouchType, comp.Id).FirstOrDefault();
                        var Credit = db.Sp_Add_General_Entry(0, Amount, bank, instNo, "Pending", instDate, creditAcc.Text_ChartCode + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Text_ChartCode, creditAcc.Head, desc + ". Received By: " + receivedby, TransId, 1, null, null, null, userid, voucherNo, vouchType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        Transaction.Commit();
                        return Json(new Return { Status = true, Msg = "Saved" });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
        }

        //Receivables
        public JsonResult PR_Receive_Receivable_Amount(decimal? Amount, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, int creditAccId, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var debitAcc = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var creditAcc = GetHead(creditAccId);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, Remarks, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, creditAcc.Text_ChartCode + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Text_ChartCode, creditAcc.Head, Remarks, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, null, "", null, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var debitAcc = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var creditAcc = GetHead(creditAccId);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, debitAcc.PaymentData.Text_ChartCode + " - " + debitAcc.PaymentData.Head, debitAcc.PaymentData.Id, debitAcc.PaymentData.Text_ChartCode, debitAcc.PaymentData.Head, Remarks, TransactionId, line, null, null, null, userid, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, debitAcc.PaymentStatus, Inst_Date, creditAcc.Text_ChartCode + " - " + creditAcc.Head, creditAcc.Id, creditAcc.Text_ChartCode, creditAcc.Head, Remarks, TransactionId, line, null, null, null, userid, Voucher_No, debitAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
        }
        //Reversal
        public JsonResult ReverseEntry(long? groupid, long userid, bool headcashier)
        {
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var TransactionId = new Helpers().RandomNumber();
            var res = db.Sp_Get_GeneralEntries_Parameter(groupid).ToList();
            if (res.Any())
            {
                var JournalEnt = new XElement("JournalEntries", res.Select(x => new XElement("Entries",
                    new XAttribute("Naration", "Reversal - " + x.Voucher_No + " - " + x.Naration),
                    new XAttribute("Head", (x.Head == null) ? "" : x.Head),
                    new XAttribute("Head_Name", x.Head_Name),
                    new XAttribute("Head_Code", x.Head_Code),
                    new XAttribute("Head_Id", x.Head_Id),
                    new XAttribute("Line", x.Line),
                    new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
                    new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
                    new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
                    new XAttribute("Debit", (x.Credit == null) ? 0 : x.Credit),
                    new XAttribute("Credit", (x.Debit == null) ? 0 : x.Debit),
                    new XAttribute("GroupId", TransactionId),
                    new XAttribute("Comp_Id", comp.Id),
                    new XAttribute("Ref", groupid)
                    ))).ToString();

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res4 = db.Sp_Add_Journal_Entries(JournalEnt, userid).FirstOrDefault();
                        var res1 = db.Sp_Update_Journal_Entry_Sup(groupid, userid).FirstOrDefault();
                        Transaction.Commit();
                        return Json(true);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            else
            {
                return Json(new Return { Status = false, Msg = "Saved" });
            }
        }

        //Inventory - Procurement
        //public JsonResult GRNEntries(List<Inventory_Stock_In> items, long? TransactionId, long userid, List<string> grn)
        //{
        //    try
        //    {
        //        List<Sp_Get_CA_Head_MultiRef_Result> itemHeads = new List<Sp_Get_CA_Head_MultiRef_Result>();
        //        decimal? amount = 0;
        //        if (items.Any())
        //        {
        //            foreach (var item in items)
        //            {
        //                itemHeads.Add(Inventory_Head(item.Item_Id, item.Item_name));
        //                decimal? rate = (item.Rate == null) ? 0 : item.Rate;
        //                decimal? qty = (item.Qty == null) ? 0 : item.Qty;
        //                amount += rate * qty;
        //            }
        //        }
        //        int i = 0;
        //        var pnsAccount = PayableNeedsSupervisionHead("Purchase Need Supervision");
        //        var venname = items.Select(x => x.Vendor_Name).FirstOrDefault();
        //        var venid = items.Select(x => x.Vendor_Id).FirstOrDefault();
        //        var venHead = Vendor_Head(venname, venid);
        //        //Journal Entry Debit
        //        var JournalEnt = new XElement("JournalEntries", items.Select(x => new XElement("Entries",
        //        new XAttribute("Naration", x.SKU + " - " + x.Item_name + " - " + x.PO_Number + " - " + grn.FirstOrDefault()),
        //        new XAttribute("Head", itemHeads[i].Code + " - " + itemHeads[i].Head),
        //        new XAttribute("Head_Name", itemHeads[i].Head),
        //        new XAttribute("Head_Code", itemHeads[i].Code),
        //        new XAttribute("Head_Id", itemHeads[i].Id),
        //        new XAttribute("Line", ++i),
        //        new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
        //        new XAttribute("UOM", ""),
        //        new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
        //        new XAttribute("Debit", ((x.Rate == null) ? 0 : x.Rate) * ((x.Qty == null) ? 0 : x.Qty)),
        //        new XAttribute("Credit", 0),
        //        new XAttribute("GroupId", TransactionId)
        //        )));
        //        // Journal Entry Credit
        //        var JEapp = new XElement("JournalEntries", new XElement("Entries",
        //        new XAttribute("Naration", "Payable Record Against PO No. " + items.FirstOrDefault().PO_Number + " - " + grn.FirstOrDefault() + " for Vendor " + items.FirstOrDefault().Vendor_Name),
        //        new XAttribute("Head", pnsAccount.Code + " - " + pnsAccount.Head),
        //        new XAttribute("Head_Name", pnsAccount.Head),
        //        new XAttribute("Head_Code", pnsAccount.Code),
        //        new XAttribute("Head_Id", pnsAccount.Id),
        //        new XAttribute("Line", items.Count() + 1),
        //        new XAttribute("Qty", 0),
        //        new XAttribute("UOM", ""),
        //        new XAttribute("Rate", 0),
        //        new XAttribute("Debit", 0),
        //        new XAttribute("Credit", amount),
        //        new XAttribute("GroupId", TransactionId)
        //        ));
        //        JournalEnt.Add(
        //            from ge in JEapp.Elements()
        //            select ge
        //            );
        //        long tranNew = new Helpers().RandomNumber();
        //        // General Entry Debit
        //        var GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
        //        new XAttribute("Naration", "Payable Record Against PO No. " + items.Select(x => x.PO_Number).FirstOrDefault() + " - " + grn.FirstOrDefault() + " for Vendor " + items.FirstOrDefault().Vendor_Name),
        //        new XAttribute("Head", pnsAccount.Code + " - " + pnsAccount.Head),
        //        new XAttribute("Head_Name", pnsAccount.Head),
        //        new XAttribute("Head_Code", pnsAccount.Code),
        //        new XAttribute("Head_Id", pnsAccount.Id),
        //        new XAttribute("Line", 1),
        //        new XAttribute("Qty", 0),
        //        new XAttribute("UOM", ""),
        //        new XAttribute("Rate", 0),
        //        new XAttribute("Debit", amount),
        //        new XAttribute("Credit", 0),
        //        new XAttribute("GroupId", tranNew),
        //        new XAttribute("Ref", TransactionId)
        //        ));
        //        // General Entry Credit
        //        var GEapp = new XElement("GeneralEntries", new XElement("Entries",
        //        new XAttribute("Naration", "Payable Record Against PO No. " + items.Select(x => x.PO_Number).FirstOrDefault() + " - " + grn.FirstOrDefault() + " for Vendor " + items.FirstOrDefault().Vendor_Name),
        //        new XAttribute("Head", venHead.Code + " - " + venHead.Head),
        //        new XAttribute("Head_Name", venHead.Head),
        //        new XAttribute("Head_Code", venHead.Code),
        //        new XAttribute("Head_Id", venHead.Id),
        //        new XAttribute("Line", 1),
        //        new XAttribute("Qty", 0),
        //        new XAttribute("UOM", ""),
        //        new XAttribute("Rate", 0),
        //        new XAttribute("Debit", 0),
        //        new XAttribute("Credit", amount),
        //        new XAttribute("GroupId", tranNew),
        //        new XAttribute("Ref", TransactionId)
        //        ));
        //        GeneralEnt.Add(
        //            from ge in GEapp.Elements()
        //            select ge
        //            );


        //        int j = 0;
        //        i = 0;
        //        string Bill = new XElement("Bills", items.Select(x => new XElement("Entries",
        //        new XAttribute("Vendor_Id", venHead.Id),
        //        new XAttribute("Terms", "30 Days"),
        //        new XAttribute("Bill_No", x.PO_Number + "-" + grn.FirstOrDefault()),
        //        new XAttribute("Head_Code", itemHeads[j].Code),
        //        new XAttribute("Head_Name", itemHeads[j].Head),
        //        new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
        //        new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
        //        new XAttribute("Line", ++i),
        //        new XAttribute("description", itemHeads[j].Head),
        //        new XAttribute("Head_Id", itemHeads[j].Id),
        //        new XAttribute("GroupId", tranNew),
        //        new XAttribute("Head", itemHeads[j].Code + " - " + itemHeads[j++].Head)
        //        ))).ToString();

        //        using (var TRN = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var res6 = db.Sp_Add_Bills(Bill, userid, DateTime.Now, DateTime.Now.AddDays(30));
        //                var res4 = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
        //                var res3 = db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();


        //                if (res3 == true && res4 == true)
        //                {
        //                    TRN.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                else
        //                {
        //                    var ret1 = new { Status = false, Msg = "Invoice Already Exist" };
        //                    TRN.Rollback();
        //                    return Json(ret1);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                TRN.Rollback();
        //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                throw;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //        throw;
        //    }
        //}
        public JsonResult DNEntries(List<Inventory_Stock_In> items, long? TransactionId, long? userid, List<string> issuenotenum)
        {
            try
            {
                var comp = this.Company_Attr(Convert.ToInt32( userid));
                List<Sp_Get_COA_Head_Code_Result> itemHeads = new List<Sp_Get_COA_Head_Code_Result>();
                List<StockOutItemDetailsModel> itemDetails = new List<StockOutItemDetailsModel>();
                decimal? amount = 0;
                var venHead = Vendor_Head(items.FirstOrDefault().Vendor_Name, items.FirstOrDefault().Vendor_Id, comp.Id);
                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        itemHeads.Add(Inventory_Head(item.Item_Id, item.Item_name, comp.Id));
                        decimal? rate = (item.Rate == null) ? 0 : item.Rate;
                        decimal? qty = (item.Qty == null) ? 0 : item.Qty;
                        amount += rate * qty;
                    }
                }
                var prjId = items.FirstOrDefault().Prj;

                var projectHead = db.COA_Mapper.Where(x =>
                          x.CompanyId == comp.Id &&
                          x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                          x.Module == COA_Mapper_Modules.Projects.ToString() &&
                          x.ModuleType == COA_Mapper_ModuleTypes.Projects_List.ToString() &&
                          x.Module_Id == prjId
                          ).FirstOrDefault();


                int i = 0;
                //Items Debit Entry

                var JournalEnt = new XElement("JournalEntries", items.Select(x => new XElement("Entries",
                new XAttribute("Naration", x.SKU + " - " + x.Item_name + " - " + x.PO_Number + " - " + issuenotenum.FirstOrDefault()),
                new XAttribute("Head", itemHeads[i].Text_ChartCode + " - " + itemHeads[i].Head),
                new XAttribute("Head_Name", itemHeads[i].Head),
                new XAttribute("Head_Code", itemHeads[i].Text_ChartCode),
                new XAttribute("Head_Id", itemHeads[i].Id),
                new XAttribute("Line", ++i),
                new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
                new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
                new XAttribute("Debit", ((x.Rate == null) ? 0 : x.Rate) * ((x.Qty == null) ? 0 : x.Qty)),
                new XAttribute("Credit", 0),
                new XAttribute("Comp_Id", comp.Id),
                new XAttribute("GroupId", TransactionId)
                )));
                // //Vendor Credit Entry
                var JEapp = new XElement("JournalEntries", new XElement("Entries",
                new XAttribute("Naration", "Payable Record Against PO No. " + items.FirstOrDefault().PO_Number + " - " + issuenotenum.FirstOrDefault() + " for Vendor " + items.FirstOrDefault().Vendor_Name),
                new XAttribute("Head", venHead.Head),
                new XAttribute("Head_Name", venHead.Head),
                new XAttribute("Head_Code", venHead.Text_ChartCode),
                new XAttribute("Head_Id", venHead.Id),
                new XAttribute("Line", items.Count() + 1),
                new XAttribute("Qty", 0),
                new XAttribute("UOM", ""),
                new XAttribute("Rate", 0),
                new XAttribute("Debit", 0),
                new XAttribute("Credit", amount),
                new XAttribute("Comp_Id", comp.Id),
                new XAttribute("GroupId", TransactionId)
                ));
                JournalEnt.Add(
                    from ge in JEapp.Elements()
                    select ge
                    );
                long tranNew = new Helpers().RandomNumber();
                //Project Debit Entry
                i = 0;
                var GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
                    new XAttribute("Naration", "Items Issued against Project: " + items.FirstOrDefault().Prj_Name + " Issue Note No. " + issuenotenum.FirstOrDefault()),
                    new XAttribute("Head", projectHead.AccountCode + " - " + projectHead.Head_Name),
                    new XAttribute("Head_Name", projectHead.Head_Name),
                    new XAttribute("Head_Code", projectHead.AccountCode),
                    new XAttribute("Head_Id", projectHead.Id),
                    new XAttribute("Line", ++i),
                    new XAttribute("Qty", 0),
                    new XAttribute("UOM", ""),
                    new XAttribute("Rate", 0),
                    new XAttribute("Debit", amount),
                    new XAttribute("Credit", 0),
                    new XAttribute("GroupId", tranNew),
                new XAttribute("Comp_Id", comp.Id),
                    new XAttribute("Ref", TransactionId)
                    ));
                //Items Credit Entry
                int j = 0;
                var itemCred = new XElement("GeneralEntries", items.Select(x => new XElement("Entries",
                new XAttribute("Naration", itemHeads[j].Head + " Issued of Qty: " + x.Qty + " against Project: " + x.Prj_Name + " Issue Note No. " + issuenotenum.FirstOrDefault()),
                new XAttribute("Head", itemHeads[j].Head),
                new XAttribute("Head_Name", itemHeads[j].Head),
                new XAttribute("Head_Code", itemHeads[j].Text_ChartCode),
                new XAttribute("Head_Id", itemHeads[j++].Id),
                new XAttribute("Line", ++i),
                new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
                new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
                new XAttribute("Debit", 0),
                new XAttribute("Credit", ((x.Rate == null) ? 0 : x.Rate) * ((x.Qty == null) ? 0 : x.Qty)),
                new XAttribute("GroupId", tranNew),
                new XAttribute("Comp_Id", comp.Id),
                new XAttribute("Ref", TransactionId)
                )));
                GeneralEnt.Add(
                    from ge in itemCred.Elements()
                    select ge
                    );
                j = 0;
                i = 0;
                string Bill = new XElement("Bills", items.Select(x => new XElement("Entries",
                new XAttribute("Vendor_Id", venHead.Id),
                new XAttribute("Terms", "30 Days"),
                new XAttribute("Bill_No", x.PO_Number + "-" + issuenotenum.FirstOrDefault()),
                new XAttribute("Head_Code", itemHeads[j].Text_ChartCode),
                new XAttribute("Head_Name", itemHeads[j].Head),
                new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
                new XAttribute("Line", ++i),
                new XAttribute("description", itemHeads[j].Head),
                new XAttribute("Head_Id", itemHeads[j].Id),
                new XAttribute("GroupId", tranNew),
                new XAttribute("PO_Group_Id", tranNew),
                new XAttribute("Comp_Id", comp.Id),
                new XAttribute("Head", itemHeads[j++].Head)
                ))).ToString();

                using (var TRN = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res3 = db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();
                        var res4 = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
                        var res6 = db.Sp_Add_Bills(Bill, userid, DateTime.Now, DateTime.Now.AddDays(30)).FirstOrDefault();
                        if (res3 == true && res4 == true)
                        {
                            TRN.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        else
                        {
                            var ret1 = new { Status = false, Msg = "Invoice Already Exist" };
                            TRN.Rollback();
                            return Json(ret1);
                        }
                    }
                    catch (Exception ex)
                    {
                        TRN.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                throw;
            }
        }
        //public JsonResult INEntries(List<IssueReq> Items, long TransactionId, long userid, Inventory_Demand_Order Ido, string issuenote)
        //{


        //}
        public JsonResult DINEntries(List<IssueReq> Items, long userId, long Transaction_Id, List<string> issuenote)
        {
            try
            {
                int k = 0;
                var comp = this.Company_Attr(userId);
                foreach (var prj in Items.GroupBy(x => x.Prj_Id))
                {
                    List<Sp_Get_COA_Head_Code_Result> itemHeads = new List<Sp_Get_COA_Head_Code_Result>();
                    List<StockOutItemDetailsModel> itemDetails = new List<StockOutItemDetailsModel>();
                    decimal? totalamt = 0;
                    var prjId = prj.Select(y => y.Prj_Id).FirstOrDefault();
                    var head = db.Projects.Where(x => x.Id == prjId).Select(x => x.Head_Code).FirstOrDefault();
                    var projectHead = db.Sp_Get_CA_Head_Param_Code(head).FirstOrDefault();
                    foreach (var v in prj)
                    {
                        StockOutItemDetailsModel det = new StockOutItemDetailsModel();
                        det.qty = v.Qty;
                        det.rate = db.Inventory_Stock_In.Where(x => x.Item_Id == v.Item_Id).Average(x => x.Rate);
                        totalamt += det.qty * det.rate;
                        var itemName = db.Inventories.Where(x => x.Id == v.Item_Id).Select(x => x.Complete_Name).FirstOrDefault();
                        itemHeads.Add(Inventory_Head(v.Item_Id, itemName, comp.Id));
                        itemDetails.Add(det);
                    }
                    int i = 0;
                    int j = 0;

                    var Journal = new XElement("JournalEntries", new XElement("Entries",
                    new XAttribute("Naration", "Items Issued to " + prj.FirstOrDefault().IssueTo_Name + " against Project: " + prj.FirstOrDefault().Prj_Name + " Issue Note No. " + issuenote[k]),
                    new XAttribute("Head", projectHead.HeadCode + " - " + projectHead.Head),
                    new XAttribute("Head_Name", projectHead.Head),
                    new XAttribute("Head_Code", projectHead.HeadCode),
                    new XAttribute("Head_Id", projectHead.Id),
                    new XAttribute("Line", ++j),
                    new XAttribute("Qty", 0),
                    new XAttribute("UOM", 0),
                    new XAttribute("Rate", 0),
                    new XAttribute("Debit", totalamt),
                    new XAttribute("Credit", 0),
                new XAttribute("Comp_Id", comp.Id),
                    new XAttribute("GroupId", Transaction_Id)
                    ));
                    var VendorEnt = new XElement("JournalEntries", prj.Select(x => new XElement("Entries",
                    new XAttribute("Naration", itemHeads[i].Head + " Issued to " + x.IssueTo_Name + " of Qty: " + x.Qty + " against Project: " + x.Prj_Name + " Issue Note No. " + issuenote[k]),
                    new XAttribute("Head", itemHeads[i].Head),
                    new XAttribute("Head_Name", itemHeads[i].Head),
                    new XAttribute("Head_Code", itemHeads[i].Text_ChartCode),
                    new XAttribute("Head_Id", itemHeads[i].Id),
                    new XAttribute("Line", ++j),
                    new XAttribute("Qty", itemDetails[i].qty),
                    new XAttribute("UOM", ""),
                    new XAttribute("Rate", itemDetails[i].rate),
                    new XAttribute("Debit", 0),
                    new XAttribute("Credit", itemDetails[i].qty * itemDetails[i++].rate),
                new XAttribute("Comp_Id", comp.Id),
                    new XAttribute("GroupId", Transaction_Id)
                    )));

                    Journal.Add(
                                from ge in VendorEnt.Elements()
                                select ge
                                );
                    try
                    {
                        var res4 = db.Sp_Add_Journal_Entries(Journal.ToString(), userId).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        throw;
                    }
                    k++;
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                throw;
            }

        }
        //public JsonResult GRNReversal(long TransactionId, long userid)
        //{
        //    var comp = Company_Attr(userid);
        //    try
        //    {
        //        var tran = new Helpers().RandomNumber();
        //        var res = db.Sp_Get_JournalEntries_Parameter_All(TransactionId).ToList();
        //        var JournalEnt = new XElement("JournalEntries", res.Select(x => new XElement("Entries",
        //            new XAttribute("Naration", "Reversal - " + x.Naration),
        //            new XAttribute("Head", x.Head),
        //            new XAttribute("Head_Name", x.Head_Name),
        //            new XAttribute("Head_Code", x.Head_Code),
        //            new XAttribute("Head_Id", x.Head_Id),
        //            new XAttribute("Line", 1),
        //            new XAttribute("Qty", 0),
        //            new XAttribute("UOM", ""),
        //            new XAttribute("Rate", 0),
        //            new XAttribute("Debit", x.Credit),
        //            new XAttribute("Credit", x.Debit),
        //            new XAttribute("GroupId", tran),
        //    new XAttribute("Comp_Id", comp.Id),
        //            new XAttribute("Ref", TransactionId)
        //            ))).ToString();


        //        using (var TRN = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var res3 = db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();
        //                db.Sp_Delete_GRNReversal_Entries(TransactionId);
        //                if (res3 == true)
        //                {
        //                    TRN.Commit();
        //                    return Json(new Return { Status = true, Msg = "Saved" });
        //                }
        //                else
        //                {
        //                    var ret1 = new { Status = false, Msg = "Invoice Already Exist" };
        //                    TRN.Rollback();
        //                    return Json(ret1);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                TRN.Rollback();
        //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //                throw;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //        throw;
        //    }
        //}
        public JsonResult SubsidiaryPaymentReceive(decimal Amount, string Plot_No, string Type, string Block, string PaymentType, string Inst_No, DateTime? Inst_Date, string Inst_Bank, string Remarks, long TransactionId, long userid, string Voucher_No, int line, string module, bool headcashier)
        {
            var comp = Company_Attr(userid);
            if (headcashier)
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Mis_Income_COA = this.Subsidary_Account(comp.Id, comp.Id);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Remarks + " :" + Plot_No, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, Mis_Income_COA.Text_ChartCode + " - " + Mis_Income_COA.Head, Mis_Income_COA.Id, Mis_Income_COA.Text_ChartCode, Mis_Income_COA.Head, Remarks + " :" + Plot_No, TransactionId, line, userid, userid, null, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, null, "", null, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
            else
            {
                if (PaymentType == "Cash")
                {
                    var Payment = this.Payment_Head(PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                    var Mis_Income_COA = this.Subsidary_Account(comp.Id, comp.Id);
                    using (var Transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Debit = db.Sp_Add_General_Entry(Amount, 0, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Remarks + " :" + Plot_No, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, Inst_Bank, Inst_No, Payment.PaymentStatus, Inst_Date, Mis_Income_COA.Text_ChartCode + " - " + Mis_Income_COA.Head, Mis_Income_COA.Id, Mis_Income_COA.Text_ChartCode, Mis_Income_COA.Head, Remarks + " :" + Plot_No, TransactionId, line, null, null, null, userid, Voucher_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                            Transaction.Commit();
                            return Json(new Return { Status = true, Msg = "Saved" });
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    return Json(new Return { Status = true, Msg = "Saved" });
                }
            }
        }
        public Company Company_Attr(long userid)
        {
            var res = db.UserClaims.Where(x => x.UserId == userid).ToList();

            Company c = new Company()
            {
                Company_Name = res.Where(x => x.ClaimType == "Company").FirstOrDefault().ClaimValue,
                Id = int.Parse(res.Where(x => x.ClaimType == "Comp_Id").FirstOrDefault().ClaimValue),
                COA = int.Parse(res.Where(x => x.ClaimType == "Comp_COA").FirstOrDefault().ClaimValue),
                COA_Head = res.Where(x => x.ClaimType == "Comp_COAHead").FirstOrDefault().ClaimValue
            };
            return c;
        }

        public Sp_Get_COA_Head_Code_Result PurchaseNeedSupervision_Head(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.CompanyId == Comp_Id &&
                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Purchase_Need_Supervision.ToString()
                            ).FirstOrDefault();

            var PNS_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return PNS_COA;
        }
        public Sp_Get_COA_Head_Code_Result GeneralSuppliers_Head(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.General_Trade_Payable.ToString()
                            ).FirstOrDefault();
            var PNS_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return PNS_COA;
        }
        public Sp_Get_COA_Head_Code_Result Income_Head(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Purchase_Need_Supervision.ToString()
                            ).FirstOrDefault();

            var PNS_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return PNS_COA;
        }
        public Sp_Get_COA_Head_Code_Result Dealer_Head(long? Id, int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Dealership.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Dealership_List.ToString() &&
                            x.Module_Id == Id
                            ).FirstOrDefault();
            var Vendor_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Vendor_COA;
        }
        public Sp_Get_COA_Head_Code_Result Dealer_Commission_Head(long? Id, int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Dealership.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Dealership_Commission.ToString() &&
                            x.Module_Id == Id
                            ).FirstOrDefault();
            var Vendor_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Vendor_COA;
        }


        public Sp_Get_COA_Head_Code_Result Project_Head(long? Id, int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Projects.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Projects_List.ToString() &&
                            x.Module_Id == Id
                            ).FirstOrDefault();
            var Vendor_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Vendor_COA;
        }
        public Sp_Get_COA_Head_Code_Result Gym_Head(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Amenities.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Gym.ToString()
                            ).FirstOrDefault();
            var Gym_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Gym_COA;
        }
        public Sp_Get_COA_Head_Code_Result Riding_Head(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Amenities.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Riding_Club.ToString()
                            ).FirstOrDefault();
            var RC_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return RC_COA;
        }
        public Sp_Get_COA_Head_Code_Result SAMHead(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Sales.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Token.ToString()
                            ).FirstOrDefault();
            var SAM_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return SAM_COA;
        }
        public Sp_Get_COA_Head_Code_Result ArchiHead(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.ODD.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Architecture_Fee.ToString()
                            ).FirstOrDefault();
            var SAM_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return SAM_COA;
        }
        public Sp_Get_COA_Head_Code_Result LoanAdvanceHead(int Comp_Id, string Loan_Type)
        {
            if (Loan_Type == "Loan")
            {
                var res = db.COA_Mapper.Where(x =>
                                           x.CompanyId == Comp_Id &&
                                           x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                                           x.Module == COA_Mapper_Modules.Human_Resource.ToString() &&
                                           x.ModuleType == COA_Mapper_ModuleTypes.Loan.ToString()
                                           ).FirstOrDefault();
                var SAM_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
                return SAM_COA;
            }
            else
            {
                var res = db.COA_Mapper.Where(x =>
                           x.CompanyId == Comp_Id &&
                           x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                           x.Module == COA_Mapper_Modules.Human_Resource.ToString() &&
                           x.ModuleType == COA_Mapper_ModuleTypes.Advance.ToString()
                           ).FirstOrDefault();
                var SAM_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
                return SAM_COA;
            }


        }


        public Sp_Get_COA_Head_Code_Result SAGPayable_Head(int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.SA_Gardens_Payable.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Liability.ToString()
                            ).FirstOrDefault();
            var Vendor_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Vendor_COA;
        }
        public Sp_Get_COA_Head_Code_Result Subsidary_Account(long? Id, int Comp_Id)
        {
            var res = db.COA_Mapper.Where(x =>
                            x.CompanyId == Comp_Id &&
                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
                            x.Module == COA_Mapper_Modules.Subsidary.ToString() &&
                            x.ModuleType == COA_Mapper_ModuleTypes.Liability.ToString() &&
                            x.Module_Id == Id
                            ).FirstOrDefault();
            var Vendor_COA = db.Sp_Get_COA_Head_Code(res.AccountCode).FirstOrDefault();
            return Vendor_COA;
        }



        public JsonResult UnderSuper(long? ReceiptId, string BankAccount, long userid)
        {
            //var comp = this.Company_Attr(userid);
            //var res = db.Receipts.Where(x => x.Id == ReceiptId).FirstOrDefault();
            //if (res.Module == Modules.PlotManagement.ToString() || res.Module == Modules.FileManagement.ToString())
            //{
            //    if (res.Type == ReceiptTypes.Dealership_Security.ToString() || res.Type == ReceiptTypes.DealerAdvance.ToString() || res.Type == ReceiptTypes.DealershipRegisteration.ToString())
            //    {
            //        var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_List.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res.File_Plot_No);//this.Dealer_Head(res.File_Plot_No, comp.Id);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received By :" + res.File_Plot_Number + " - " + res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received By :" + res.File_Plot_Number + " - " + res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.Booking.ToString() || res.Type == ReceiptTypes.Installment.ToString() || res.Type == ReceiptTypes.Advance.ToString() || res.Type == ReceiptTypes.Confirmation.ToString())
            //    {
            //        long blockId;
            //        if (res.Module == Modules.PlotManagement.ToString())
            //        {
            //            blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
            //        }
            //        else if (res.Module == Modules.FileManagement.ToString())
            //        {
            //            blockId = Convert.ToInt64(db.Sp_Get_FileAppFormData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
            //        }
            //        else
            //        {
            //            blockId = 0;
            //        }
            //        string receivableModule = "";
            //        if (res.Plot_Type == PlotType.Commercial.ToString())
            //        {
            //            receivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();
            //        }
            //        else
            //        {
            //            receivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
            //        }
            //        var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), receivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.ServiceCharges.ToString())
            //    {
            //        long blockId;
            //        if (res.Module == Modules.PlotManagement.ToString())
            //        {
            //            blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
            //        }
            //        else if (res.Module == Modules.FileManagement.ToString())
            //        {
            //            blockId = Convert.ToInt64(db.Sp_Get_FileData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
            //        }
            //        else
            //        {
            //            blockId = 0;
            //        }
            //        var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Electricity_Charges_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);//Plot_Head(res.File_Plot_Number, res.Plot_Type, res.Block);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Service Charges of :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Service Charges of :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.Electricity_Charges.ToString())
            //    {
            //        long blockId;
            //        if (res.Module == Modules.PlotManagement.ToString())
            //        {
            //            blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
            //        }
            //        else if (res.Module == Modules.FileManagement.ToString())
            //        {
            //            blockId = Convert.ToInt64(db.Sp_Get_FileData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
            //        }
            //        else
            //        {
            //            blockId = 0;
            //        }
            //        var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Electricity_Charges_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);//Plot_Head(res.File_Plot_Number, res.Plot_Type, res.Block);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Service Charges of :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Service Charges of :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.Cancellation.ToString())
            //    {
            //        var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Cancellation.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Cancellation Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Cancellation Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else if (res.Type == ReceiptTypes.Transfer.ToString())
            //    {
            //        long blockId;
            //        if (res.Module == Modules.PlotManagement.ToString())
            //        {
            //            blockId = db.Sp_Get_PlotData(res.File_Plot_No).Select(x => x.BlockIden).FirstOrDefault();
            //        }
            //        else if (res.Module == Modules.FileManagement.ToString())
            //        {
            //            blockId = Convert.ToInt64(db.Sp_Get_FileData(res.File_Plot_No).Select(x => x.Block_Id).FirstOrDefault());
            //        }
            //        else
            //        {
            //            blockId = 0;
            //        }
            //        string moduleType = "";
            //        if (res.Plot_Type == PlotType.Residential.ToString())
            //        {
            //            moduleType = COA_Mapper_ModuleTypes.Plots_Transfer_Charges_Residential.ToString();
            //        }
            //        else
            //        {
            //            moduleType = COA_Mapper_ModuleTypes.Plots_Transfer_Charges_Commercial.ToString();
            //        }
            //        var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), moduleType, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, blockId);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against :" + res.File_Plot_Number + " - " + res.Plot_Type + " Block: " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), res.Type, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//Plot_Head(res.File_Plot_Number, res.Plot_Type, res.Block);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against " + res.Type.Replace('_', ' '), Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against " + res.Type.Replace('_', ' '), Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //}
            //else if (res.Module == Modules.CommercialManagement.ToString())
            //{
            //    if (res.Type == ReceiptTypes.Booking.ToString() || res.Type == ReceiptTypes.Confirmation.ToString() || res.Type == ReceiptTypes.Installment.ToString() || res.Type == ReceiptTypes.Advance.ToString())
            //    {
            //        var projectid = db.Sp_Get_CommercialData(res.File_Plot_No).Select(x => x.ProjectId).FirstOrDefault();
            //        var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //        var creditAccount = HeadFinder(COA_Mapper_Modules.Commercial.ToString(), COA_Mapper_ModuleTypes.Commercial_Project_Receivables.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, projectid);
            //        using (var Transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Unit " + res.File_Plot_Number + " - " + res.Plot_Type + " - Floor " + res.Block, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //                Transaction.Commit();
            //                return Json(new Return { Status = true, Msg = "Saved" });
            //            }
            //            catch (Exception ex)
            //            {
            //                Transaction.Rollback();
            //                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //                throw;
            //            }
            //        }
            //    }
            //}
            //else if (res.Module == Modules.Dealers.ToString())
            //{
            //    var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //    var creditAccount = HeadFinder(COA_Mapper_Modules.Dealership.ToString(), COA_Mapper_ModuleTypes.Dealership_Registration.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
            //    using (var Transaction = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, "Amount Received Against Dealership Registration of " + res.File_Plot_Number, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, "Amount Received Against Dealership Registration of " + res.File_Plot_Number, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            Transaction.Commit();
            //            return Json(new Return { Status = true, Msg = "Saved" });
            //        }
            //        catch (Exception ex)
            //        {
            //            Transaction.Rollback();
            //            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //            throw;
            //        }
            //    }
            //}
            //else if (res.Module == Modules.Accounts_Management.ToString())
            //{
            //    //File Plot Number in this section is Head Id of a Receivable Account
            //    var debitAccount = Banks_Online_Head(BankAccount, comp.Id);
            //    var creditAccount = GetHead(Convert.ToInt32(res.File_Plot_Number));
            //    using (var Transaction = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var Debit = db.Sp_Add_General_Entry(res.Amount, 0, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, debitAccount.Text_ChartCode + " - " + debitAccount.Head, debitAccount.Id, debitAccount.Text_ChartCode, debitAccount.Head, res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            var Credit = db.Sp_Add_General_Entry(0, res.Amount, res.Bank, res.Ch_Pay_Draft_No, "Pending", res.Ch_Pay_Draft_Date, creditAccount.Text_ChartCode + " - " + creditAccount.Head, creditAccount.Id, creditAccount.Text_ChartCode, creditAccount.Head, res.Text, Convert.ToInt64(res.Transaction_Id), 1, null, null, null, userid, res.ReceiptNo, "BRV", comp.Id).FirstOrDefault();
            //            Transaction.Commit();
            //            return Json(new Return { Status = true, Msg = "Saved" });
            //        }
            //        catch (Exception ex)
            //        {
            //            Transaction.Rollback();
            //            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //            throw;
            //        }
            //    }
            //}
            return Json(true);
        }
        //public PaymentTypeModel Payment_Head_Salary(string PaymentType, string Debit_Credit, int CompId, string Headname)
        //{
        //    var Payment = HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Cash_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), CompId, null);//db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").FirstOrDefault();
        //    var vt = (Debit_Credit == "Debit") ? Voucher_Type.CRV.ToString() : Voucher_Type.CPV.ToString();
        //    var res = new PaymentTypeModel { PaymentStatus = null, PaymentData = Payment, VoucherType = vt };
        //    return res;
        //}

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
