//using MeherEstateDevelopers.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MeherEstateDevelopers.Models;
//using Microsoft.AspNet.Identity;
//using System.Xml.Linq;

//namespace MeherEstateDevelopers.Controllers
//{
//    [Authorize]
//    [LogAction]
//    public class AssetsController : Controller
//    {
//        // GET: Assets
//        Grand_CityEntities db = new Grand_CityEntities();

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult AddAssets(string Name, string Description, string Specs, string Type, int Company_Id)
//        {
//            ViewBag.Category = new SelectList(db.Inventory_Category.ToList(), "Id", "Name");
//            //var res = db.Sp_Add_Assets(Name, Description, Specs, Type, Company_Id);
//            return Json(true);
//        }

//        //Base View
//        public ActionResult AssetsTracker()
//        {
//            return View();
//        }

//        //Assets
//        public ActionResult AssetsList()
//        {
//            var res1 = db.Assets.Where(x => x.isDisposal == null).ToList();
//            var res = db.Company_Assets.ToList();
//            return PartialView(res1);
//        }
//        public ActionResult AddAssets()
//        {
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            ViewBag.DepreciationRules = new SelectList(db.Assets_Depreciation_Rules.ToList(), "Id", "RuleName");
//            //ViewBag.Category = new SelectList(db.Inventory_Category.ToList(), "Id", "Name");
//            ViewBag.Category = new SelectList(db.Sp_Get_CA_LeaveNodes_Fixed_Assets().ToList(), "Text_ChartCode", "Head");
//            ViewBag.Type = new SelectList(Enum.GetValues(typeof(AssetType)).Cast<AssetType>().Select(v => new SelectListItem
//            {
//                Text = v.ToString().Replace('_', ' '),
//                Value = v.ToString()
//            }).ToList());
//            return PartialView();
//        }
//        public Sp_Get_CA_Head_Parameter_Result GetHead(int Id)
//        {
//            var res = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
//            return res;
//        }
//        public JsonResult AddNewAsset(Asset ass, long trans)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res1 = db.Assets.Where(x => x.Asset_Name == ass.Asset_Name && x.Asset_Ref == ass.Asset_Ref && x.Asset_Type == ass.Asset_Type).FirstOrDefault();
//            if (res1 == null)
//            {
//                AccountHandlerController de = new AccountHandlerController();
//                var res2 = de.Asset_Account(ass.Group_Name);
//                ass.Head_Code = res2.Code.ToString();
//                ass.Userid = userid;

//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        db.Assets.Add(ass);
//                        db.SaveChanges();

//                        //var vouch = db.Sp_Add_Journal_Voucher().FirstOrDefault();
//                        //bool headcashier = false;
//                        //if (User.IsInRole("Head Cashier"))
//                        //{
//                        //    headcashier = true;
//                        //}
//                        //var res4 = de.Asset_Creation(ass.Asset_Name, ass.Asset_Ref, ass.Purchase_Date, ass.Purchase_Value, trans, ass.Group_Name, userid, vouch, headcashier);

//                        //var res5 = db.Assets.Where(x => x.Asset_Name == ass.Asset_Name && x.Asset_Ref == ass.Asset_Ref && x.Group_Name == ass.Group_Name).FirstOrDefault();
//                        //List<Depreciation_Installments> depreciation_installments = new List<Depreciation_Installments>();
//                        //string installmentplan = "";
//                        //DateTime purDate = ass.Purchase_Date.Value;
//                        //int usefulLifeinYears = Convert.ToInt32(ass.Useful_Life);
//                        //var res6 = db.Assets_Depreciation_Rules.Where(x => x.Id == ass.Rule_Id).FirstOrDefault();
//                        //if (res6.DepreciationType == "Straight Line" || res6.DepreciationType == "Double Decline")
//                        //{
//                        //    var years = Convert.ToInt32(res6.DepreciationValue);
//                        //    if (years > 0)
//                        //    {
//                        //        decimal amount = (Convert.ToDecimal(ass.Purchase_Value) - Convert.ToDecimal(ass.Salvage_Value)) / years;
//                        //        for (int i = 1; i <= years; i++)
//                        //        {
//                        //            DateTime a = purDate.AddYears(i);
//                        //            DateTime dt = new DateTime(a.Year, a.Month, a.Day);
//                        //            Depreciation_Installments fi = new Depreciation_Installments()
//                        //            {
//                        //                Installment_Name = "Depreciation Installment " + i,
//                        //                Installment_Type = "1",
//                        //                Asset_Id = res5.Id,
//                        //                Status = InstallmentPaymentStatus.Pending.ToString(),
//                        //                isCancelled = false,
//                        //                Amount = amount,
//                        //                Due_Date = dt,
//                        //            };
//                        //            depreciation_installments.Add(fi);
//                        //        }
//                        //        installmentplan = new XElement("Installments", depreciation_installments.Select(x => new XElement("Installmentsdata",
//                        //                       new XAttribute("Installment_Name", x.Installment_Name),
//                        //                       new XAttribute("Installment_Type", x.Installment_Type),
//                        //                       new XAttribute("Asset_Id", x.Asset_Id),
//                        //                       new XAttribute("Status", x.Status),
//                        //                       new XAttribute("isCancelled", x.isCancelled),
//                        //                       new XAttribute("Amount", x.Amount),
//                        //                       new XAttribute("Due_Date", x.Due_Date)
//                        //                       ))).ToString();
//                        //        db.Sp_Add_Asset_Depreciation_By_Month(installmentplan, res5.Id);
//                        //    }
//                        //}
//                        ////else if (res6.DepreciationType == "Straight Line - Months")
//                        ////{
//                        ////    months = Convert.ToInt32(res6.DepreciationValue);
//                        ////    int mont = (months == 0) ? 1 : months;
//                        ////    decimal amount = (Convert.ToDecimal(ass.Purchase_Value) - Convert.ToDecimal(ass.Salvage_Value)) / mont;
//                        ////    for (int i = 1; i <= months; i++)
//                        ////    {
//                        ////        DateTime a = purDate.AddMonths(i);
//                        ////        DateTime dt = new DateTime(a.Year, a.Month, 1);
//                        ////        Depreciation_Installments fi = new Depreciation_Installments()
//                        ////        {
//                        ////            Installment_Name = "Depreciation Installment " + i,
//                        ////            Installment_Type = "1",
//                        ////            Asset_Id = res5.Id,
//                        ////            Status = InstallmentPaymentStatus.Pending.ToString(),
//                        ////            isCancelled = false,
//                        ////            Amount = amount,
//                        ////            Due_Date = dt,
//                        ////        };
//                        ////        depreciation_installments.Add(fi);
//                        ////    }
//                        ////    installmentplan = new XElement("Installments", depreciation_installments.Select(x => new XElement("Installmentsdata",
//                        ////                   new XAttribute("Installment_Name", x.Installment_Name),
//                        ////                   new XAttribute("Installment_Type", x.Installment_Type),
//                        ////                   new XAttribute("Asset_Id", x.Asset_Id),
//                        ////                   new XAttribute("Status", x.Status),
//                        ////                   new XAttribute("isCancelled", x.isCancelled),
//                        ////                   new XAttribute("Amount", x.Amount),
//                        ////                   new XAttribute("Due_Date", x.Due_Date)
//                        ////                   ))).ToString();
//                        ////    db.Sp_Add_Asset_Depreciation_By_Month(installmentplan, res5.Id);
//                        ////}
//                        //else if (res6.DepreciationType == "Reducing Balance")
//                        //{
//                        //    var depVal = Convert.ToDecimal(res6.DepreciationValue) / 100;
//                        //    decimal amount = Convert.ToDecimal(ass.Purchase_Value) * depVal;
//                        //    decimal deprec = Convert.ToDecimal(ass.Purchase_Value);
//                        //    int i = 1;
//                        //    while (amount > 1)
//                        //    {
//                        //        DateTime a = purDate.AddYears(i);
//                        //        DateTime dt = new DateTime(a.Year, a.Month, a.Day);
//                        //        Depreciation_Installments fi = new Depreciation_Installments()
//                        //        {
//                        //            Installment_Name = "Depreciation Installment " + i,
//                        //            Installment_Type = "1",
//                        //            Asset_Id = res5.Id,
//                        //            Status = InstallmentPaymentStatus.Pending.ToString(),
//                        //            isCancelled = false,
//                        //            Amount = amount,
//                        //            Due_Date = dt,
//                        //        };
//                        //        depreciation_installments.Add(fi);
//                        //        deprec = deprec - amount;
//                        //        amount = deprec * depVal;
//                        //        i++;
//                        //    }
//                        //    installmentplan = new XElement("Installments", depreciation_installments.Select(x => new XElement("Installmentsdata",
//                        //                   new XAttribute("Installment_Name", x.Installment_Name),
//                        //                   new XAttribute("Installment_Type", x.Installment_Type),
//                        //                   new XAttribute("Asset_Id", x.Asset_Id),
//                        //                   new XAttribute("Status", x.Status),
//                        //                   new XAttribute("isCancelled", x.isCancelled),
//                        //                   new XAttribute("Amount", x.Amount),
//                        //                   new XAttribute("Due_Date", x.Due_Date)
//                        //                   ))).ToString();
//                        //    db.Sp_Add_Asset_Depreciation_By_Month(installmentplan, res5.Id);
//                        //}



//                        Transaction.Commit();
//                        return Json(new { Status = true, Msg = "Asset added successfully." });

//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        return Json(new { Status = false, Msg = "Asset couldn't add. Please try later." });
//                    }
//                }
//            }
//            else
//            {
//                return Json(new { Status = false, Msg = "Asset already exist." });
//            }
//        }
//        public ActionResult ViewAssetDetails(long Id)
//        {
//            var res = db.Assets.Where(x => x.Id == Id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public ActionResult UpdateAsset(long id)
//        {
//            var res = db.Assets.Where(x => x.Id == id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public JsonResult UpdateAssetDetails(Asset ass)
//        {
//            try
//            {
//                db.Sp_Update_Asset_Details(ass.Id, ass.Asset_Name, ass.Vendor_Name, ass.Asset_Life, ass.Length, ass.L_UOM, ass.Width, ass.W_UOM, ass.Height, ass.H_UOM, ass.Diameter, ass.D_UOM, ass.Size, ass.Size_UOM, ass.Asset_Description);
//                return Json(new { Status = true, Msg = "Asset updated successfully." });
//            }
//            catch (Exception ex)
//            {
//                var ra = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new { Status = false, Msg = "Failed to update Asset." });
//            }
//        }
//        public ActionResult AssetSellOut(long id)
//        {
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            var res = db.Assets.Where(x => x.Id == id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public JsonResult AssetSell(long id, long trans, decimal amount, string payType, string chqno, string bank, string branch, DateTime? paydate)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res1 = db.Assets.Where(x => x.Id == id).FirstOrDefault();
//            var depreciatedValue = db.Depreciation_Installments.Where(x => x.Asset_Id == id).Sum(y => y.Amount);
//            decimal disposalValue = 0; //Book Value
//            disposalValue = Convert.ToDecimal(res1.Purchase_Value) - Convert.ToDecimal(depreciatedValue);



//            //var res3 = db.Sp_Get_Head_Id_COA("Accumulated Depreciation-" + res1.Group_Name).FirstOrDefault(); ;
//            //var res4 = db.Sp_Get_Head_Id_COA(res1.Group_Name).FirstOrDefault();

//            //string GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
//            //new XAttribute("Naration", "Amount against sell-off disposal of " + res1.Asset_Name + "Reference No. " + res1.Asset_Ref),
//            //new XAttribute("Head", (res3.Head == null) ? "" : res3.Head),
//            //new XAttribute("Head_Name", this.GetHead(res3.Id).Head),
//            //new XAttribute("Head_Code", this.GetHead(res3.Id).Text_ChartCode),
//            //new XAttribute("Head_Id", res3.Id),
//            //new XAttribute("Line", 1),
//            //new XAttribute("Qty", 0),
//            //new XAttribute("UOM", ""),
//            //new XAttribute("Rate", 0),
//            //new XAttribute("Debit", (disposalValue == 0) ? 0 : disposalValue),
//            //new XAttribute("Credit", 0),
//            //new XAttribute("GroupId", trans)
//            //),
//            //new XElement("Entries",
//            //new XAttribute("Naration", "Amount against sell-off disposal of " + res1.Asset_Name + "Reference No. " + res1.Asset_Ref),
//            //new XAttribute("Head", (res4.Head == null) ? "" : res4.Head),
//            //new XAttribute("Head_Name", this.GetHead(res4.Id).Head),
//            //new XAttribute("Head_Code", this.GetHead(res4.Id).Text_ChartCode),
//            //new XAttribute("Head_Id", res4.Id),
//            //new XAttribute("Line", 1),
//            //new XAttribute("Qty", 0),
//            //new XAttribute("UOM", ""),
//            //new XAttribute("Rate", 0),
//            //new XAttribute("Debit", 0),
//            //new XAttribute("Credit", (disposalValue == 0) ? 0 : disposalValue),
//            //new XAttribute("GroupId", trans)
//            //)).ToString();

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    db.Sp_Update_Asset_For_Disposal("Sell Off", disposalValue, res1.Id);
//                    //var vouch = db.Sp_Add_Journal_Voucher().FirstOrDefault();
//                    //bool headcashier = false;
//                    //if (User.IsInRole("Head Cashier"))
//                    //{
//                    //    headcashier = true;
//                    //}
//                    //try
//                    //{
//                    //    AccountHandlerController de = new AccountHandlerController();
//                    //    de.Asset_Disposal_Sell_Off(res1.Asset_Name, res1.Asset_Ref, DateTime.Now, depreciatedValue, trans, res1.Group_Name, userid, vouch, headcashier);
//                    //    if (amount < disposalValue)
//                    //    {
//                    //        //Loss
//                    //        de.AssetSellOffLoss(res1.Asset_Name, res1.Asset_Ref, amount, disposalValue - amount, payType, chqno, paydate,bank, trans, res1.Group_Name, userid, vouch, headcashier);
//                    //    }
//                    //    else
//                    //    {
//                    //        //Gain
//                    //        de.AssetSellOffProfit(res1.Asset_Name, res1.Asset_Ref, amount, amount - disposalValue, payType, chqno, paydate, bank, trans, res1.Group_Name, userid, vouch, headcashier);
//                    //    }
//                    //}
//                    //catch (Exception ex)
//                    //{
//                    //    Transaction.Rollback();
//                    //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    //}

//                    //Debit Entry in Cash or Receivable
//                    //Profit and Loss Entry

//                    Transaction.Commit();
//                    return Json(new { Status = true, Msg = "Asset sell off successfully" });

//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Asset couldn't sell off. Please try later." });
//                }
//            }
//        }
//        public ActionResult AssetScrap(long id)
//        {
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            var res = db.Assets.Where(x => x.Id == id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public JsonResult ScrappedAsset(long id, long trans)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res1 = db.Assets.Where(x => x.Id == id).FirstOrDefault();
//            var res2 = db.Depreciation_Installments.Where(x => x.Asset_Id == id && x.isCancelled == false && x.Status == InstallmentPaymentStatus.Paid.ToString()).ToList();
//            decimal depreciatedValue = 0;
//            decimal disposalValue = 0;
//            if (res2 != null)
//            {
//                foreach (var a in res2)
//                {
//                    depreciatedValue = depreciatedValue + Convert.ToDecimal(a.Amount);
//                }
//                disposalValue = Convert.ToDecimal(res1.Purchase_Value) - depreciatedValue;
//            }

//            //var res3 = db.Sp_Get_Head_Id_COA("Accumulated Depreciation-"+res1.Group_Name).FirstOrDefault(); ;
//            //var res4 = db.Sp_Get_Head_Id_COA(res1.Group_Name).FirstOrDefault();

//            //string GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
//            //new XAttribute("Naration", "Amount against scrapped disposal of " + res1.Asset_Name + "Reference No. " + res1.Asset_Ref),
//            //new XAttribute("Head", (res3.Head == null) ? "" : res3.Head),
//            //new XAttribute("Head_Name", this.GetHead(res3.Id).Head),
//            //new XAttribute("Head_Code", this.GetHead(res3.Id).Text_ChartCode),
//            //new XAttribute("Head_Id", res3.Id),
//            //new XAttribute("Line", 1),
//            //new XAttribute("Qty", 0),
//            //new XAttribute("UOM", ""),
//            //new XAttribute("Rate", 0),
//            //new XAttribute("Debit", (depreciatedValue == 0) ? 0 : depreciatedValue),
//            //new XAttribute("Credit", 0),
//            //new XAttribute("GroupId", trans)
//            //),
//            //new XElement("Entries",
//            //new XAttribute("Naration", "Amount against scrapped disposal of " + res1.Asset_Name + "Reference No. " + res1.Asset_Ref),
//            //new XAttribute("Head", (res4.Head == null) ? "" : res4.Head),
//            //new XAttribute("Head_Name", this.GetHead(res4.Id).Head),
//            //new XAttribute("Head_Code", this.GetHead(res4.Id).Text_ChartCode),
//            //new XAttribute("Head_Id", res4.Id),
//            //new XAttribute("Line", 1),
//            //new XAttribute("Qty", 0),
//            //new XAttribute("UOM", ""),
//            //new XAttribute("Rate", 0),
//            //new XAttribute("Debit", 0),
//            //new XAttribute("Credit", (depreciatedValue == 0) ? 0 : depreciatedValue),
//            //new XAttribute("GroupId", trans)
//            //)).ToString();

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    db.Sp_Update_Asset_For_Disposal("Scrapped", disposalValue, res1.Id);
//                    db.Sp_Update_Depreciation_Installments_At_Disposal(res1.Id);
//                    //var vouch = db.Sp_Add_Journal_Voucher().FirstOrDefault();
//                    //bool headcashier = false;
//                    //if (User.IsInRole("Head Cashier"))
//                    //{
//                    //    headcashier = true;
//                    //}
//                    //try
//                    //{
//                    //    AccountHandlerController de = new AccountHandlerController();
//                    //    de.Asset_Disposal_Scrapped(res1.Asset_Name, res1.Asset_Ref, DateTime.Now, depreciatedValue, trans, res1.Group_Name, userid, vouch, headcashier);
//                    //}
//                    //catch (Exception ex)
//                    //{
//                    //    Transaction.Rollback();
//                    //    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    //}

//                    Transaction.Commit();
//                    return Json(new { Status = true, Msg = "Asset scrapped successfully" });

//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Asset couldn't scrapped. Please try later." });
//                }
//            }
//        }

//        //Depreciation Rules
//        public ActionResult DepreciationRules()
//        {
//            var res = db.Assets_Depreciation_Rules.ToList();
//            return PartialView(res);
//        }
//        public ActionResult AddDepreciationRules()
//        {
//            ViewBag.depreciationMethods = new SelectList(Nomenclature.DepreciationMethods(), "Value", "Name");
//            return PartialView();
//        }
//        public ActionResult UpdateDepreciationRule(long id)
//        {
//            ViewBag.depreciationMethods = new SelectList(Nomenclature.DepreciationMethods(), "Value", "Name");
//            var res = db.Assets_Depreciation_Rules.Where(x => x.Id == id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public JsonResult AddRule(Assets_Depreciation_Rules adr)
//        {
//            var res1 = db.Assets_Depreciation_Rules.Where(x => x.RuleName == adr.RuleName && x.DepreciationType == adr.DepreciationType && x.DepreciationValue == adr.DepreciationValue).Count();
//            if (res1 > 0)
//            {
//                return Json(new { Status = false, Msg = "Rule Already Exist" });
//            }
//            else
//            {
//                try
//                {
//                    db.Assets_Depreciation_Rules.Add(adr);
//                    db.SaveChanges();
//                    return Json(new { Status = true, Msg = "Rule Added Successfully" });
//                }
//                catch (Exception ex)
//                {
//                    var ra = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Failed to add rule." });
//                }
//            }
//        }
//        public JsonResult UpdateRule(Assets_Depreciation_Rules adr)
//        {
//            try
//            {
//                db.Sp_Update_Assets_Depreciation_Rule(adr.Id, adr.RuleName, adr.DepreciationType, adr.DepreciationValue);
//                return Json(new { Status = true, Msg = "Rule updated successfully." });
//            }
//            catch (Exception ex)
//            {
//                var ra = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new { Status = false, Msg = "Failed to update rule." });
//            }
//        }

//        //Asset Group
//        public ActionResult AssetGroup()
//        {
//            //var res = db.Inventory_Category.ToList();
//            var res = db.Sp_Get_CA_LeaveNodes_Fixed_Assets().ToList();
//            return PartialView(res);
//        }
//        public ActionResult AddAssetGroup()
//        {
//            ViewBag.DepreciationRules = new SelectList(db.Assets_Depreciation_Rules.ToList(), "id", "ruleName");
//            return PartialView();
//        }
//        public JsonResult AddNewAssetGroup(Inventory_Category ic)
//        {
//            var res = db.Inventory_Category.Where(x => x.Name == ic.Name).FirstOrDefault();
//            if (res == null)
//            {
//                using (var transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        db.Inventory_Category.Add(ic);
//                        db.SaveChanges();
//                        var res1 = db.Sp_Get_CA_Head_MultiRef_3("Assets", "Non Current Assets", "Fixed Assets").FirstOrDefault();
//                        var res2 = db.Sp_Get_CA_Head_MultiRef("Expenses", "Depriciation").FirstOrDefault();
//                        db.Sp_Add_CA_Head(res1.Id, ic.Name, "Asset", null,COA_Nature.Assets.ToString());
//                        db.Sp_Add_CA_Head(res1.Id, "Accumulated Depreciation-" + ic.Name, "Asset - Depreciation", null, COA_Nature.Assets.ToString());
//                        db.Sp_Add_CA_Head(res2.Id, "Depreciation-" + ic.Name, "Asset - Depreciation Expense", null, COA_Nature.Expense.ToString());
//                        transaction.Commit();
//                        return Json(new { Status = true, Msg = "Asset group added successfully." });
//                    }
//                    catch
//                    {
//                        transaction.Rollback();
//                        return Json(new { Status = false, Msg = "Asset group couldn't add. Please try later." });
//                    }
//                }
//            }
//            else
//            {
//                return Json(new { Status = false, Msg = "Asset group already present." });
//            }
//        }
//        public ActionResult UpdateAssetGroup(int id)
//        {
//            ViewBag.DepreciationRules = new SelectList(db.Assets_Depreciation_Rules.ToList(), "id", "ruleName");
//            var res = db.Inventory_Category.Where(x => x.Id == id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public JsonResult UpdateAssetGroupAG(Inventory_Category ic, int itemid)
//        {
//            db.Sp_Update_Inventory_Category(ic.Name, ic.Default_Rule_Id, ic.Default_Rule_Name, ic.Description, itemid);
//            return Json(new { Status = true, Msg = "Asset group updated successfully." });
//        }

//        public JsonResult ApplyDepreciation()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Assets.Where(x => x.isDisposal == null && x.Rule_Name != "No Depreciation").ToList();
//            bool allOk = true;
//            foreach (var v in res)
//            {
//                var Transact = new Helpers().RandomNumber();
//                decimal depval = 0;
//                decimal? valtoDep = v.Purchase_Value;
//                decimal totalDepVal = 0;

//                //Checking Dates - From Where To Start Depreciating
//                var startDate = new DateTime(v.Commencement_Date.Value.Year, v.Commencement_Date.Value.Month, 25);
//                var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
//                var ldo = db.Sp_Get_Last_Paid_DepInstallment(v.Id).FirstOrDefault();
//                if (ldo != null)
//                {
//                    startDate = ldo.Due_Date.Value.AddMonths(1);
//                    valtoDep = ldo.Amount;
//                }

//                //Journal Entry Head
//                AccountHandlerController de = new AccountHandlerController();
//                var accDepAccount = de.Asset_Depreciation_Account(v.Group_Name);
//                var expDepAcc = de.DepreciationExpenseAccount();
//                //Finding Depreciation Value
//                var depreRule = db.Assets_Depreciation_Rules.Where(x => x.Id == v.Rule_Id).FirstOrDefault();
//                if (depreRule.DepreciationType == "Straight Line")
//                {
//                    List<DepreciationInstallmentsModel> dimList = new List<DepreciationInstallmentsModel>();
//                    if (depreRule.DepreciationValue > 0)
//                    {
//                        int months = Convert.ToInt32(depreRule.DepreciationValue) * 12;
//                        var maxDate = v.Commencement_Date.Value.AddMonths(months);
//                        var peryear = Convert.ToDecimal(v.Purchase_Value) / Convert.ToDecimal(depreRule.DepreciationValue);
//                        var permonth = Math.Round(peryear / 12);
//                        int j = 1;
//                        for (var i = startDate.Date; i < dt.Date && i < maxDate.Date; i = i.AddMonths(1))
//                        {
//                            totalDepVal += Math.Round(permonth);
//                            DepreciationInstallmentsModel dim1 = new DepreciationInstallmentsModel();
//                            dim1.InstallmentName = "Installment for month: " + i.ToString("dd/MM/yyyy");
//                            dim1.InstallmentType = "Depreciation Installments";
//                            dim1.AssetId = v.Id;
//                            dim1.Status = "Paid";
//                            dim1.IsCancelled = false;
//                            dim1.Amount = permonth;
//                            dim1.DueDate = i;
//                            dim1.PaidDate = DateTime.Now;
//                            dimList.Add(dim1);
//                        }
//                        var DepreciationEnt = new XElement("DepreciationEntries", dimList.Select(x => new XElement("Entries",
//                        new XAttribute("Installment_Name", x.InstallmentName),
//                        new XAttribute("Installment_Type", x.InstallmentType),
//                        new XAttribute("Asset_Id", x.AssetId),
//                        new XAttribute("Status", x.Status),
//                        new XAttribute("isCancelled", x.IsCancelled),
//                        new XAttribute("Amount", x.Amount),
//                        new XAttribute("Due_Date", x.DueDate),
//                        new XAttribute("Paid_Date", x.PaidDate)
//                        ))).ToString();
//                        //    var JournalEnt = new XElement("JournalEntries", new XElement("Entries",
//                        //    new XAttribute("Naration", "Depreciation Recorded Against " + v.Asset_Ref + " - " + v.Asset_Name),
//                        //    new XAttribute("Head", expDepAcc.Code + " - " + expDepAcc.Head),
//                        //    new XAttribute("Head_Name", expDepAcc.Head),
//                        //    new XAttribute("Head_Code", expDepAcc.Code),
//                        //    new XAttribute("Head_Id", expDepAcc.Id),
//                        //    new XAttribute("Line", 1),
//                        //    new XAttribute("Qty", 0),
//                        //    new XAttribute("UOM", ""),
//                        //    new XAttribute("Rate", 0),
//                        //    new XAttribute("Debit", totalDepVal),
//                        //    new XAttribute("Credit", 0),
//                        //    new XAttribute("GroupId", Transact)
//                        //));
//                        //    var JEapp = new XElement("JournalEntries", new XElement("Entries",
//                        //        new XAttribute("Naration", "Depreciation Recorded Against " + v.Asset_Ref + " - " + v.Asset_Name),
//                        //        new XAttribute("Head", accDepAccount.Code + " - " + accDepAccount.Head),
//                        //        new XAttribute("Head_Name", accDepAccount.Head),
//                        //        new XAttribute("Head_Code", accDepAccount.Code),
//                        //        new XAttribute("Head_Id", accDepAccount.Id),
//                        //        new XAttribute("Line", 1),
//                        //        new XAttribute("Qty", 0),
//                        //        new XAttribute("UOM", ""),
//                        //        new XAttribute("Rate", 0),
//                        //        new XAttribute("Debit", 0),
//                        //        new XAttribute("Credit", totalDepVal),
//                        //        new XAttribute("GroupId", Transact)
//                        //    ));
//                        //    JournalEnt.Add(
//                        //        from ge in JEapp.Elements()
//                        //        select ge
//                        //    );

//                        using (var Transaction = db.Database.BeginTransaction())
//                        {
//                            try
//                            {
//                                db.Sp_Add_Depreciation_Installments(DepreciationEnt, userid);
//                                //db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();
//                                Transaction.Commit();
//                            }
//                            catch (Exception ex)
//                            {
//                                Transaction.Rollback();
//                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                                allOk = false;
//                            }
//                        }
//                    }
//                }
//                else if (depreRule.DepreciationType == "Reducing Balance")
//                {
//                    List<DepreciationInstallmentsModel> dimList = new List<DepreciationInstallmentsModel>();
//                    var peryear = Convert.ToDecimal(valtoDep) * (Convert.ToDecimal(depreRule.DepreciationValue) / 100);
//                    depval = Math.Round(peryear / 12);
//                    int j = 1;
//                    for (var i = startDate.Date; i < dt.Date; i = i.AddMonths(1))
//                    {
//                        if (depval > 0)
//                        {
//                            totalDepVal += Math.Round(depval);
//                            DepreciationInstallmentsModel dim1 = new DepreciationInstallmentsModel();
//                            dim1.InstallmentName = "Installment for month: " + i.ToString("dd/MM/yyyy");
//                            dim1.InstallmentType = "Depreciation Installments";
//                            dim1.AssetId = v.Id;
//                            dim1.Status = "Paid";
//                            dim1.IsCancelled = false;
//                            dim1.Amount = depval;
//                            dim1.DueDate = i;
//                            dim1.PaidDate = DateTime.Now;
//                            dimList.Add(dim1);
//                            valtoDep = valtoDep - depval;
//                            peryear = Math.Round(Convert.ToDecimal(valtoDep) * (Convert.ToDecimal(depreRule.DepreciationValue) / 100));
//                            depval = Math.Round(peryear / 12);
//                        }
//                        //else if (depval > 0)
//                        //{
//                        //    depval = 1;
//                        //    totalDepVal += Math.Round(depval);
//                        //    DepreciationInstallmentsModel dim1 = new DepreciationInstallmentsModel();
//                        //    dim1.InstallmentName = "Installment for month: " + i.ToString("dd/MM/yyyy");
//                        //    dim1.InstallmentType = "Depreciation Installments";
//                        //    dim1.AssetId = v.Id;
//                        //    dim1.Status = "Paid";
//                        //    dim1.IsCancelled = false;
//                        //    dim1.Amount = depval;
//                        //    dim1.DueDate = i;
//                        //    dim1.PaidDate = DateTime.Now;
//                        //    dimList.Add(dim1);
//                        //    valtoDep = valtoDep - depval;
//                        //    peryear = Convert.ToDecimal(valtoDep) * (Convert.ToDecimal(depreRule.DepreciationValue) / 100);
//                        //    depval = peryear / 12;
//                        //}
//                        else
//                        {
//                            break;
//                        }

//                    }
//                    var DepreciationEnt = new XElement("DepreciationEntries", dimList.Select(x => new XElement("Entries",
//                        new XAttribute("Installment_Name", x.InstallmentName),
//                        new XAttribute("Installment_Type", x.InstallmentType),
//                        new XAttribute("Asset_Id", x.AssetId),
//                        new XAttribute("Status", x.Status),
//                        new XAttribute("isCancelled", x.IsCancelled),
//                        new XAttribute("Amount", x.Amount),
//                        new XAttribute("Due_Date", x.DueDate),
//                        new XAttribute("Paid_Date", x.PaidDate)
//                        ))).ToString();
//                    var JournalEnt = new XElement("JournalEntries", new XElement("Entries",
//                        new XAttribute("Naration", "Depreciation Recorded Against " + v.Asset_Ref + " - " + v.Asset_Name),
//                        new XAttribute("Head", expDepAcc.Code + " - " + expDepAcc.Head),
//                        new XAttribute("Head_Name", expDepAcc.Head),
//                        new XAttribute("Head_Code", expDepAcc.Code),
//                        new XAttribute("Head_Id", expDepAcc.Id),
//                        new XAttribute("Line", 1),
//                        new XAttribute("Qty", 0),
//                        new XAttribute("UOM", ""),
//                        new XAttribute("Rate", 0),
//                        new XAttribute("Debit", totalDepVal),
//                        new XAttribute("Credit", 0),
//                        new XAttribute("GroupId", Transact)
//                    ));
//                    var JEapp = new XElement("JournalEntries", new XElement("Entries",
//                        new XAttribute("Naration", "Depreciation Recorded Against " + v.Asset_Ref + " - " + v.Asset_Name),
//                        new XAttribute("Head", accDepAccount.Code + " - " + accDepAccount.Head),
//                        new XAttribute("Head_Name", accDepAccount.Head),
//                        new XAttribute("Head_Code", accDepAccount.Code),
//                        new XAttribute("Head_Id", accDepAccount.Id),
//                        new XAttribute("Line", 1),
//                        new XAttribute("Qty", 0),
//                        new XAttribute("UOM", ""),
//                        new XAttribute("Rate", 0),
//                        new XAttribute("Debit", 0),
//                        new XAttribute("Credit", totalDepVal),
//                        new XAttribute("GroupId", Transact)
//                    ));
//                    JournalEnt.Add(
//                        from ge in JEapp.Elements()
//                        select ge
//                    );
//                    using (var Transaction = db.Database.BeginTransaction())
//                    {
//                        try
//                        {
//                            db.Sp_Add_Depreciation_Installments(DepreciationEnt, userid);
//                            db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();
//                            Transaction.Commit();
//                        }
//                        catch (Exception ex)
//                        {
//                            Transaction.Rollback();
//                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                            allOk = false;
//                        }
//                    }
//                }
//            }
//            if (allOk)
//            {
//                return Json(new Return { Status = true, Msg = "Depreciation Applied to All Assets" });
//            }
//            else
//            {
//                return Json(new Return { Status = true, Msg = "Error Occured While Depreciating Assets" });
//            }

//        }

//        public void As()
//        {
//            var res = (from x in db.Inventories
//                       join y in db.Inventory_Stock_In on x.Id equals y.Item_Id
//                       where x.Is_Asset == true && y.Cancelled == null
//                       select new
//                       {
//                           x.Description,
//                           x.Complete_Name,
//                           y.Group_Id,
//                           y.Vendor_Id,
//                           y.Vendor_Name,
//                           y.CreatedBy_Name,
//                           y.Created_By,
//                           y.DateTime,
//                           y.Qty,
//                           y.Rate
//                       }).ToList();

//            foreach (var a in res)
//            {
//                for (int i = 0; i < a.Qty; i++)
//                {
//                    Asset ass = new Asset()
//                    {
//                        Asset_Description = a.Description
//    ,
//                        Asset_Name = a.Complete_Name
//    ,
//                        Group_Id = Convert.ToInt32(a.Group_Id)
//    ,
//                        Vendor_Id = a.Vendor_Id
//    ,
//                        Vendor_Name = a.Vendor_Name
//    ,
//                        User_Name = a.CreatedBy_Name
//    ,
//                        Userid = a.Created_By
//    ,
//                        Purchase_Date = a.DateTime
//    ,
//                        Purchase_Value = a.Rate
//                    };
//                    db.Assets.Add(ass);
//                    db.SaveChanges();
//                }
                
//            }
//        }

//    }
//}