//using MeherEstateDevelopers.Models;
//using Microsoft.AspNet.Identity;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Xml.Linq;
//using MeherEstateDevelopers.Filters;

//namespace MeherEstateDevelopers.Controllers
//{
//    [Authorize]
//    [LogAction]
//    public class ProcurementController : Controller
//    {
//        // GET: Procurement
//        private Grand_CityEntities db = new Grand_CityEntities();
//        public ActionResult Purchases()
//        {
//            return View();
//        }
//        public ActionResult AddNewQuotation(long Req_Id, long Item_Id, string Item)
//        {
//            ViewBag.Req_Id = Req_Id;
//            ViewBag.Item_Id = Item_Id;
//            ViewBag.Item = Item;
//            return PartialView();
//        }
//        public JsonResult AddQuotation(long V, decimal Q, decimal R, string Rem, long Req_Id, long Item_Id, string C, decimal? T)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var Ven = db.Vendors.Where(x => x.Id == V).FirstOrDefault();
//            var VenExists = db.Inventory_Bidding.Where(x => x.Vendor_Id == V && x.Group_Id == Req_Id && x.Item_Id == Item_Id).Any();
//            if (VenExists)
//            {
//                var ret = new { Status = false, Msg = "Vendor is Already Added" };
//                return Json(ret);
//            }
//            var Re_Id = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Req_Id && x.Item_Id == Item_Id).ToList();

//            var itemsData = new XElement("Biddings", Re_Id.Select(x => new XElement("BidData",
//                        new XAttribute("itemId", x.Item_Id),
//                        new XAttribute("itemName", x.Item_Name),
//                        (x.Length is null) ? null : new XAttribute("Len", x.Length),
//                        (x.Len_UOM is null) ? null : new XAttribute("Len_UOM", x.Len_UOM),
//                        (x.Width is null) ? null : new XAttribute("Wid", x.Width),
//                        (x.Wid_UOM is null) ? null : new XAttribute("Wid_UOM", x.Wid_UOM),
//                        (x.Heigth is null) ? null : new XAttribute("Hei", x.Heigth),
//                        (x.Hei_UOM is null) ? null : new XAttribute("Hei_UOM", x.Hei_UOM),
//                        (x.Diameter is null) ? null : new XAttribute("Dia", x.Diameter),
//                        (x.Dia_UOM is null) ? null : new XAttribute("Dia_UOM", x.Dia_UOM),
//                        (x.Size is null) ? null : new XAttribute("Size", x.Size),
//                        (x.S_UOM is null) ? null : new XAttribute("SizeUom", x.S_UOM),
//                        (x.UOM is null) ? null : new XAttribute("Uom", x.UOM),
//                        (x.Dep_Id is null) ? null : new XAttribute("Dep_Id", x.Dep_Id),
//                        (x.Dep_Name is null) ? null : new XAttribute("Dep_Name", x.Dep_Name),
//                        new XAttribute("Qty", Q),
//                        new XAttribute("GroupId", x.Group_Id),
//                        new XAttribute("VendorId", Ven.Id),
//                        new XAttribute("VendorName", Ven.Company),
//                        (x.Description is null) ? null : new XAttribute("descr", Rem),
//                        new XAttribute("preq", x.Id),
//                        (T is null) ? null : new XAttribute("Tax", T),
//                        new XAttribute("Currency", C),
//                        new XAttribute("prate", R)))).ToString();
//            var res1 = db.Sp_Add_Bids(itemsData, userid, null).FirstOrDefault();

//            db.Sp_Add_Activity(userid, "Added New Qutation ", "Create", "Activity_Record", ActivityType.Procurement.ToString(), Item_Id);

//            var cred_term = db.Vendor_Credit_Terms.Where(x => x.Vendor_Id == V).ToList();
//            foreach (var item in cred_term)
//            {
//                var a = Convert.ToDouble(item.Days);
//                var res2 = this.SaveTerm(item.Terms, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(a).Day), Req_Id, V);
//            }

//            int index = 0;
//            foreach (string file in Request.Files)
//            {
//                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
//                index++;
//                if (hpf.ContentLength == 0)
//                    continue;
//                var imageName = Path.GetExtension(hpf.FileName);
//                if (!Directory.Exists(Server.MapPath("~/Repository/Procurement/" + Req_Id + "/" + res1.Id + "")))
//                {
//                    Directory.CreateDirectory(Server.MapPath("~/Repository/Procurement/" + Req_Id + "/" + res1.Id + ""));
//                }
//                string savedFileName = Path.Combine(Server.MapPath("~/Repository/Procurement/" + Req_Id + "/" + res1.Id + "/"), hpf.FileName);
//                hpf.SaveAs(savedFileName);
//                db.Sp_Update_QuoteImg(hpf.FileName, res1.Id);
//            }
//            var res = new { Status = true, Msg = "Successfully Saved" };
//            return Json(res);
//        }
//        public ActionResult AddAllQuotations(long Group)
//        {
//            var res = db.Sp_Get_Purchase_Req_For_Quotation(Group).ToList();
//            return PartialView(res);
//        }
//        public JsonResult AddQuotation_All(List<Inventory_Bidding> items, long Vendor_Id, long Group_Id, string Quote_Ref, string Currency, string Description, string PaymentTime)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var Ven = db.Vendors.Where(x => x.Id == Vendor_Id).FirstOrDefault();

//            List<Inventory_Bidding> ibl = new List<Inventory_Bidding>();
//            foreach (var item in items)
//            {
//                var VenExists = db.Inventory_Bidding.Where(x => x.Vendor_Id == Vendor_Id && x.Group_Id == Group_Id && x.Item_Id == item.Item_Id).Any();
//                if (VenExists)
//                {
//                    var ret = new { Status = false, Msg = "Vendor is Already Added" };
//                    return Json(ret);
//                }
//            }
//            var itemsData = new XElement("Biddings", items.Select(x => new XElement("BidData",
//                           new XAttribute("itemId", x.Item_Id),
//                           new XAttribute("PurchaseReq_Id", x.PurchaseReq_Id),
//                           (x.UOM is null) ? null : new XAttribute("Uom", x.UOM),
//                           (x.Dep_Id is null) ? null : new XAttribute("Dep_Id", x.Dep_Id),
//                           (x.Dep_Name is null) ? null : new XAttribute("Dep_Name", x.Dep_Name),
//                           new XAttribute("PaymentTime", PaymentTime),
//                           new XAttribute("Qty", x.Qty),
//                           new XAttribute("GroupId", Group_Id),
//                           new XAttribute("VendorId", Ven.Id),
//                           new XAttribute("VendorName", Ven.Company),
//                           (x.Description is null) ? null : new XAttribute("descr", Description),
//                           new XAttribute("preq", x.PurchaseReq_Id),
//                           (x.Tax is null) ? null : new XAttribute("Tax", x.Tax),
//                           new XAttribute("Currency", Currency),
//                           new XAttribute("prate", x.PurchaseRate)))).ToString();

//            var res1 = db.Sp_Add_Bids(itemsData, userid, null).FirstOrDefault();


//            db.Sp_Add_Activity(userid, "Added New Qutation " + Quote_Ref, "Create", "Activity_Record", ActivityType.Procurement.ToString(), Group_Id);



//            var res = new { Status = true, Msg = "Successfully Saved", Id = res1.Id };
//            return Json(res);
//        }
//        public JsonResult UploadQuotation(long Group_Id, long QuoteId)
//        {
//            int index = 0;
//            foreach (string file in Request.Files)
//            {
//                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
//                index++;
//                if (hpf.ContentLength == 0)
//                    continue;
//                var imageName = Path.GetExtension(hpf.FileName);
//                if (!Directory.Exists(Server.MapPath("~/Repository/Procurement/" + Group_Id + "/" + QuoteId + "")))
//                {
//                    Directory.CreateDirectory(Server.MapPath("~/Repository/Procurement/" + Group_Id + "/" + QuoteId + ""));
//                }
//                string savedFileName = Path.Combine(Server.MapPath("~/Repository/Procurement/" + Group_Id + "/" + QuoteId + "/"), hpf.FileName);
//                hpf.SaveAs(savedFileName);
//                db.Sp_Update_QuoteImg(hpf.FileName, QuoteId);
//            }
//            return Json(new { Status = true, Msg = "Successfully Saved" });
//        }
//        private string CompleteName(long? Id)
//        {
//            var res = db.Inventories.Where(z => z.Id == Id).Select(z => z.Complete_Name).FirstOrDefault();
//            return res;
//        }
//        public JsonResult GeneratePO(long Group_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var res = db.Inventory_Bidding.Where(x => x.Group_Id == Group_Id && x.IsFinalized == true && x.PO_Generated == null).ToList();
//            var req = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).FirstOrDefault();

//            var empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var emp = db.Sp_Get_Employee_Parameter(empid).FirstOrDefault();
//            var des = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    foreach (var item in res.GroupBy(x => x.Vendor_Id))
//                    {
//                        var itemsData = new XElement("Biddings", item.Select(x => new XElement("BidData",
//                           new XAttribute("itemName", this.CompleteName(x.Item_Id)),
//                           (x.Length is null) ? null : new XAttribute("Len", x.Length),
//                           (x.L_UOM is null) ? null : new XAttribute("L_UOM", x.L_UOM),
//                           (x.Width is null) ? null : new XAttribute("Wid", x.Width),
//                           (x.W_UOM is null) ? null : new XAttribute("W_UOM", x.W_UOM),
//                           (x.Heigth is null) ? null : new XAttribute("Hei", x.Heigth),
//                           (x.H_UOM is null) ? null : new XAttribute("H_UOM", x.H_UOM),
//                           (x.Diameter is null) ? null : new XAttribute("Dia", x.Diameter),
//                           (x.D_UOM is null) ? null : new XAttribute("D_UOM", x.D_UOM),
//                           (x.Size is null) ? null : new XAttribute("Size", x.Size),
//                           (x.S_UOM is null) ? null : new XAttribute("S_UOM", x.S_UOM),
//                           (x.Description is null) ? null : new XAttribute("descr", x.Description),
//                           (x.UOM is null) ? null : new XAttribute("Uom", x.UOM),
//                           (req.Dep_Id is null) ? null : new XAttribute("Dep_Id", req.Dep_Id),
//                           (req.Dep_Name is null) ? null : new XAttribute("Dep_Name", req.Dep_Name),
//                           (req.Prj_Id is null) ? null : new XAttribute("Prj_Id", req.Prj_Id),
//                           (req.Prj_Name is null) ? null : new XAttribute("Prj_Name", req.Prj_Name),
//                           (req.Prj_Offsite is null) ? null : new XAttribute("Prj_Offsite", req.Prj_Offsite),
//                           (emp.Name is null) ? null : new XAttribute("ApprovedBy_Name", emp.Name),
//                           (des.DesignationName is null) ? null : new XAttribute("ApprovedBy_Designation", des.DesignationName),
//                           new XAttribute("Qty", x.Qty),
//                           new XAttribute("GroupId", Group_Id),
//                           new XAttribute("VendorId", x.Vendor_Id),
//                           new XAttribute("VendorName", x.Vendor_Name),
//                           new XAttribute("bid", x.Id),
//                           new XAttribute("prate", x.PurchaseRate),
//                           new XAttribute("Tax", x.Tax),
//                           new XAttribute("Currency", x.Currency),
//                           (x.PaymentTime is null) ? null : new XAttribute("PaymentTime", x.PaymentTime),
//                           new XAttribute("itemId", x.Item_Id)))).ToString();
//                        var res3 = db.Sp_Add_PurchaseOrders(itemsData, userid, PurchaseRequisitionStatus.Purchase_Order_NotPrinted.ToString()).ToList();

//                        db.Sp_Add_Activity(userid, "Generated New PO " + Group_Id, "Create", "Activity_Record", ActivityType.Procurement.ToString(), Group_Id);

//                    }
//                    Transaction.Commit();
//                    var ret = new { Status = true, Msg = "Purchase Requisition is Finalized" };
//                    return Json(ret);
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    var ret = new { Status = false, Msg = "Failed to generate PO. Please try again later." };
//                    db.Sp_Add_ErrorLog(ex.Message, ex.InnerException == null ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, "Procurement", "GeneratePO");
//                    return Json(ret);
//                }
//            }
//        }
//        public JsonResult PrintPO(long Group_Id, long Vendor_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var res2 = db.Sp_Get_EmployeeFirstDesignation(res1).FirstOrDefault();

//            db.Sp_Add_Activity(userid, "Printed PO For Vendor " + Vendor_Id, "Read", "Activity_Record", ActivityType.Procurement.ToString(), Group_Id);

//            db.Sp_Update_PO_PrintBy(userid, res2.DesignationName, Group_Id, null, Vendor_Id);
//            var res = db.Inventory_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.Vendor_Id == Vendor_Id).Select(x => x.PO_Number).Distinct().ToList();
//            return Json(res);
//        }
//        //public JsonResult GenerateAllPO()
//        //{
//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//        //    var res2 = db.Sp_Get_EmployeeFirstDesignation(res1).FirstOrDefault();

//        //    var PR = db.Inventory_Purchase_Req.Where(x => x.Status == PurchaseRequisitionStatus.Approved.ToString()).Select(x=> x.Group_Id).ToList();

//        //    var res = db.Inventory_Bidding.Where(x => PR.Contains( x.Group_Id ) && x.IsFinalized == true && x.PO_Generated == null).ToList();
//        //    List<string> PoList = new List<string>();
//        //    using (var Transaction = db.Database.BeginTransaction())
//        //    {
//        //        try
//        //        {
//        //            foreach (var item in res.GroupBy(x => x.Vendor_Id))
//        //            {
//        //                var itemsData = new XElement("Biddings", item.Select(x => new XElement("BidData",
//        //                   new XAttribute("itemName", x.Item_Name),
//        //                   (x.Length is null) ? null : new XAttribute("Len", x.Length),
//        //                   (x.L_UOM is null) ? null : new XAttribute("L_UOM", x.L_UOM),
//        //                   (x.Width is null) ? null : new XAttribute("Wid", x.Width),
//        //                   (x.W_UOM is null) ? null : new XAttribute("W_UOM", x.W_UOM),
//        //                   (x.Heigth is null) ? null : new XAttribute("Hei", x.Heigth),
//        //                   (x.H_UOM is null) ? null : new XAttribute("H_UOM", x.H_UOM),
//        //                   (x.Diameter is null) ? null : new XAttribute("Dia", x.Diameter),
//        //                   (x.D_UOM is null) ? null : new XAttribute("D_UOM", x.D_UOM),
//        //                   (x.Size is null) ? null : new XAttribute("Size", x.Size),
//        //                   (x.S_UOM is null) ? null : new XAttribute("S_UOM", x.S_UOM),
//        //                   (x.Description is null) ? null : new XAttribute("descr", x.Description),
//        //                   (x.UOM is null) ? null : new XAttribute("Uom", x.UOM),
//        //                   (x.Dep_Id is null) ? null : new XAttribute("Dep_Id", x.Dep_Id),
//        //                   (x.Dep_Name is null) ? null : new XAttribute("Dep_Name", x.Dep_Name),
//        //                   (res2.DesignationName is null) ? null : new XAttribute("CreatedBy_Designation", res2.DesignationName),
//        //                   new XAttribute("Qty", x.Qty),
//        //                   new XAttribute("GroupId", x.Group_Id),
//        //                   new XAttribute("VendorId", x.Vendor_Id),
//        //                   new XAttribute("VendorName", x.Vendor_Name),
//        //                   new XAttribute("bid", x.Id),
//        //                   new XAttribute("prate", x.PurchaseRate),
//        //                   new XAttribute("itemId", x.Item_Id)))).ToString();
//        //                var res3 = db.SP_Add_PurchaseOrders(itemsData, userid, PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).ToList();

//        //                var vendor = db.Vendors.Where(x => x.Id == item.Key).FirstOrDefault();
//        //                var res4 = db.Sp_Get_CA_Head_MultiRef("Liability", "Trade Payables").FirstOrDefault();
//        //                var res44 = db.Sp_Get_CA_Head_MultiRef("Liability", vendor.Company).FirstOrDefault();
//        //                if (res44 == null)
//        //                {
//        //                    var res6 = db.Sp_Add_CA_Head(res4.Id, vendor.Company, "Vendor", vendor.Id);
//        //                }

//        //                PoList.AddRange(res3.Select(x => x.ponum));
//        //            }
//        //            Transaction.Commit();
//        //            var PosList = PoList.Select(x=> x).Distinct().ToList();
//        //            var ret = new { Status = true, Msg = "Purchase Order Generated", PoNum = PosList };
//        //            return Json(ret);
//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            EmailService e = new EmailService();
//        //            e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "General EntrSupervision  Error");
//        //            Transaction.Rollback();
//        //            var ret = new { Status = false, Msg = "Error Occured" };
//        //            return Json(ret);
//        //        }
//        //    }


//        //}
//        public ActionResult UpdateQuote(long Id)
//        {
//            var res = db.Inventory_Bidding.Where(x => x.Id == Id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public ActionResult POTerms(long Id)
//        {
//            var res = db.Inventory_PO_Terms.Where(x => x.Req_Id == Id).ToList();
//            return PartialView(res);
//        }
//        public JsonResult SaveTerm(string Term, DateTime? Date, long Bid_Id, long Vendor)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = Convert.ToBoolean(db.Sp_Add_POTerms(Term, Date, userid, Bid_Id, Vendor).FirstOrDefault());
//            if (res)
//            {

//                db.Sp_Add_Activity(userid, "Added New Terms " + Term, "Create", "Activity_Record", ActivityType.Procurement.ToString(), Bid_Id);

//                var ret = new { Status = true, Msg = "Successfully Added" };
//                return Json(ret);
//            }
//            else
//            {
//                var ret = new { Status = false, Msg = "Already Exists" };
//                return Json(ret);
//            }
//        }
//        public JsonResult DeleteTerm(long Term_Id)
//        {
//            db.Sp_Delete_POTerms(Term_Id);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted PO Term", "Delete", "Activity_Record", ActivityType.Procurement.ToString(), Term_Id);

//            var ret = new { Status = true, Msg = "Successfully Removed" };
//            return Json(ret);
//        }
//        public JsonResult PO_Terms(long Id, long Group_Id)
//        {
//            var res1 = db.Inventory_PO_Terms.Where(x => x.Req_Id == Group_Id).ToList();
//            var res2 = db.Inventory_Bidding.Where(x => x.Id == Id).FirstOrDefault();

//            var res = new { Terms = res1, Image = res2 };
//            return Json(res);
//        }
//        public ActionResult Invoice()
//        {
//            return View();
//        }
//        public ActionResult PO_Details(long Group_Id, string po)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Inventory_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.PO_Number == po).ToList();
//            res.ForEach(x => x.ReceivedQuantity = db.SP_Get_ReceivedQuantity_AgainstPO(x.Id).FirstOrDefault());
//            Helpers h = new Helpers();
//            ViewBag.Transaction = h.RandomNumber();
//            db.Sp_Add_Activity(userid, "Accessed PO Detail Page " + po, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Group_Id);

//            return View(res);
//        }
//        public ActionResult WO_Details(long Group_Id, string po)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Service_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.PO_Number == po).ToList();
//            Helpers h = new Helpers();
//            ViewBag.Transaction = h.RandomNumber();
//            db.Sp_Add_Activity(userid, "Accessed PO Detail Page " + po, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Group_Id);

//            return View(res);
//        }
//        public ActionResult Revise_PO(string po)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == po).ToList();
//            res.ForEach(x => x.ReceivedQuantity = db.SP_Get_ReceivedQuantity_AgainstPO(x.Id).FirstOrDefault());

//            db.Sp_Add_Activity(userid, "Accessed Revise PO Page" + po, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return PartialView(res);
//        }
//        public JsonResult UpdatePO_Qty_Rate(long id, decimal val, string Type, string PO)
//        {
//            db.Sp_Update_Qty_Rate(id, val, Type, PO + "-Revised", "");
//            var itemDat = db.Inventory_PurchaseOrder.Where(x => x.Id == id).FirstOrDefault();
//            InventoryController ic = new InventoryController();
//            ic.AdjustReceiving(itemDat.Group_Id, itemDat.Item_Id, itemDat.Vendor_Id);
//            ic.AdjustStockIn(itemDat.Group_Id, itemDat.Item_Id, itemDat.Vendor_Id);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Updated PO Quantity and rates " + PO, "Update", "Activity_Record", ActivityType.Procurement.ToString(), id);
//            return Json(true);
//        }
//        public ActionResult RecordAdv()
//        {
//            return PartialView();
//        }
//        public JsonResult RecordAdvance(string Vendor, long Vendor_Id, decimal Amount,long Transaction_Id, long PO_Group_Id, string Descriptions)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            List<Voucher_Details_General_Entries> Details = new List<Voucher_Details_General_Entries>();

//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    {

//                        bool headcashier = false;
//                        if (User.IsInRole("Head Cashier"))
//                        {
//                            headcashier = true;
//                        }
//                        try
//                        {
//                            AccountHandlerController de = new AccountHandlerController();
//                            de.Record_Vendor_Advance(Amount, Vendor, Vendor_Id, Descriptions, Transaction_Id, PO_Group_Id, userid, 1, headcashier);
//                        }
//                        catch (Exception ex)
//                        {
//                            Transaction.Rollback();
//                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        }
//                    }
//                    db.Sp_Add_Activity(userid, "REcorded Advance For Vendor " + Vendor_Id + " of Amount " + Amount, "Create", "Activity_Record", ActivityType.Procurement.ToString(), Vendor_Id);
//                    Transaction.Commit();
//                    var ret = new { Status = true, Msg = "Transaction Recorded" };
//                    return Json(ret);
//                }
//                catch (Exception e)
//                {
//                    Transaction.Rollback();
//                    var ret = new { Status = true, Msg = "Error Occured" };
//                    return Json(ret);
//                }
//            }
//        }
//        //private Voucher_Details_General_Entries GetItemInvoiceEntry(Inventory_PurchaseOrder item)
//        //{
//        //    var res1 = db.Inventories.Where(x => x.Id == item.Item_Id).FirstOrDefault();
//        //    Sp_Get_CA_Head_MultiRef_4_Result res2 = new Sp_Get_CA_Head_MultiRef_4_Result();

//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    db.Sp_Add_Activity(userid, "Accessed Detail Page of Invoice Entry of  " + item.Item_Name, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), item.Item_Id);
//        //    if (item.Asset == true)
//        //    {
//        //        res2 = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", res1.Complete_Name).FirstOrDefault();

//        //        if (res2 == null)
//        //        {
//        //            var inv = db.Inventories.Where(x => x.Id == item.Item_Id).FirstOrDefault();
//        //            var cat = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", inv.Category_Name).FirstOrDefault();
//        //            if (cat == null)
//        //            {
//        //                var head = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", "Property Plant and Equipment").FirstOrDefault();
//        //                var a = db.Sp_Add_CA_Head(head.Id, inv.Category_Name, "", inv.Category_Id);

//        //                var b = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", inv.Category_Name).FirstOrDefault();
//        //                var c = db.Sp_Add_CA_Head(b.Id, res1.Complete_Name, "", inv.Category_Id);
//        //                res2 = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", res1.Complete_Name).FirstOrDefault();


//        //            }
//        //            else
//        //            {
//        //                var c = db.Sp_Add_CA_Head(cat.Id, res1.Complete_Name, "", inv.Category_Id);
//        //                res2 = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Non Current Assets", "Fixed Assets", res1.Complete_Name).FirstOrDefault();
//        //            }
//        //        }
//        //    }
//        //    else
//        //    {
//        //        res2 = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Current Assets", "Inventory", res1.Complete_Name).FirstOrDefault();

//        //        if (res2 == null)
//        //        {
//        //            var inv = db.Inventories.Where(x => x.Id == item.Item_Id).FirstOrDefault();
//        //            var cat = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Current Assets", "Inventory", inv.Category_Name).FirstOrDefault();
//        //            if (cat == null)
//        //            {
//        //                var head = db.Sp_Get_CA_Head_MultiRef_3("Assets", "Current Assets", "Inventory").FirstOrDefault();
//        //                var a = db.Sp_Add_CA_Head(head.Id, inv.Category_Name, "", inv.Category_Id);

//        //                var b = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Current Assets", "Inventory", inv.Category_Name).FirstOrDefault();
//        //                var c = db.Sp_Add_CA_Head(b.Id, res1.Complete_Name, "", inv.Category_Id);
//        //                res2 = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Current Assets", "Inventory", res1.Complete_Name).FirstOrDefault();


//        //            }
//        //            else
//        //            {
//        //                var c = db.Sp_Add_CA_Head(cat.Id, res1.Complete_Name, "", inv.Category_Id);
//        //                res2 = db.Sp_Get_CA_Head_MultiRef_4("Assets", "Current Assets", "Inventory", res1.Complete_Name).FirstOrDefault();
//        //            }
//        //        }
//        //    }

//        //    Voucher_Details_General_Entries entry = new Voucher_Details_General_Entries()
//        //    {
//        //        Account_Head = res2.Head,
//        //        Account_Head_Code = res2.Code,
//        //        Account_Head_Id = res2.Id,
//        //        Amount = item.TotalVal,
//        //        Credit = 0,
//        //        Debit = item.TotalVal,
//        //        Item_Name = "Invoice Against " + item.PO_Number,
//        //        Line = 1,
//        //        Quantity = item.Qty,
//        //        Rate = item.Purchase_Rate,
//        //        UOM = item.UOM
//        //    };
//        //    return entry;
//        //}
//        //private Voucher_Details_General_Entries OtherPropotionate(decimal? Val, string Nar, long? Item_Id, int line)
//        //{
//        //    var res1 = db.Inventories.Where(x => x.Id == Item_Id).FirstOrDefault();
//        //    Sp_Get_CA_Head_MultiRef_Result res2 = new Sp_Get_CA_Head_MultiRef_Result();
//        //    res2 = db.Sp_Get_CA_Head_MultiRef("Assets", res1.Complete_Name).FirstOrDefault();
//        //    if (res2 == null)
//        //    {
//        //        var res3 = db.Sp_Get_CA_Head_MultiRef("Assets", "Inventory").FirstOrDefault();
//        //        var res4 = db.Sp_Add_CA_Head(res3.Id, res1.Complete_Name, "Inventory", res1.Id);
//        //        res2 = db.Sp_Get_CA_Head_MultiRef("Assets", res1.Complete_Name).FirstOrDefault();
//        //    }
//        //    Voucher_Details_General_Entries entry = new Voucher_Details_General_Entries()
//        //    {
//        //        Account_Head = res2.Head,
//        //        Account_Head_Code = res2.Code,
//        //        Account_Head_Id = res2.Id,
//        //        Amount = Val,
//        //        Credit = 0,
//        //        Debit = Val,
//        //        Item_Name = Nar,
//        //        Line = line,
//        //        Quantity = null,
//        //        Rate = null,
//        //        UOM = null
//        //    };
//        //    return entry;
//        //}
//        public JsonResult GenerateInvoice(long? Vend, long? GroupId, long? Transaction_Id, long userid, List<GRNItems> Items)
//        {
//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.Group_Id == GroupId && x.Vendor_Id == Vend && x.OtherCharges != true).ToList();
//            res1.ForEach(x => x.Asset = Items.Where(y => y.Item_Id == x.Item_Id).Select(z => z.IsAsset).FirstOrDefault());

//            try
//            {
//                foreach (var item in Items)
//                {
//                    var d = db.Sp_Update_Inventory_Supervision(item.Id, item.IsAsset, item.Expirey);
//                }

//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "General EntrSupervision  Error");
//                var ret2 = new { Status = false, Msg = "Error Occured" };
//                return Json(ret2);
//            }
//            var ret3 = new { Status = true, Msg = "Invoice has been submitted" };
//            return Json(ret3);
//        }
//        public ActionResult PendingApprovalPurchaseRequistion()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Approval Pending PR's Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (User.IsInRole("Administrator") || User.IsInRole("All Projects"))
//            {
//                var res = db.Inventory_Purchase_Req.Where(x =>
//            (x.Status == PurchaseRequisitionStatus.Pending_Approval.ToString()
//            || x.Status == PurchaseRequisitionStatus.Quotation_Approval.ToString())).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Users = db.Sp_Get_Reporting_Employees(EmpId).Select(x => x.loginid).ToList();
//                var res = db.Inventory_Purchase_Req.Where(x =>
//                (x.Status == PurchaseRequisitionStatus.Pending_Approval.ToString()
//                || x.Status == PurchaseRequisitionStatus.Quotation_Approval.ToString()) && Users.Contains(x.Requested_By)).ToList();
//                return PartialView(res);
//            }
//        }
//        public JsonResult UpdatePurchaseReq(long Group_Id, string Status)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Update_Purchase_Req_Status(Group_Id, Status, userid);
//            this.UpdateStat(Group_Id);
//            db.Sp_Add_Activity(userid, "Updated Status of PR " + Status, "Update", "Activity_Record", ActivityType.Procurement.ToString(), Group_Id);
//            return Json(true);
//        }
//        public JsonResult UpdatePurchaseReqBack(long Group_Id, string Status, string Reason)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Update_Purchase_Req_Status_rzn(Group_Id, Status, Reason, userid);
//            this.UpdateStat(Group_Id);
//            db.Sp_Add_Activity(userid, "Updated Status of PR " + Status + " With Reason " + Reason, "Update", "Activity_Record", ActivityType.Procurement.ToString(), Group_Id);

//            {
//                // Notification Section idhar aye ga

//            }
//            return Json(true);
//        }
//        public JsonResult UpdateStat(long Group_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            var res2 = db.Inventory_Demand_Req.Where(x => x.Group_Id == Group_Id).FirstOrDefault();
//            if (res2 == null)
//            {
//                return Json(true);
//            }
//            var res1 = db.Sp_Update_DemandReq_Approval(userid, Group_Id);

//            // Send notification to Department

//            var res3 = new XElement("Deps", new XElement("Ids", new XAttribute("Id", res2.ReqTo_Dep))).ToString();
//            var res4 = db.Sp_Get_Employees_Dep(res3).Select(x => x.Id).ToList();
//            var res5 = db.Sp_Get_HeadOfDepartment(res2.ReqTo_Dep).ToList();
//            foreach (var item in res5)
//            {
//                Notifier.Notify(userid, Convert.ToInt64(item.Id), NotifierMsg.Demand_Approved, new object[] { res2 }, NotifierMsg.Demand_Approved.ToString());
//            }
//            return Json(true);
//        }
//        public ActionResult Pending_Approval_Purchase_Req()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Approval Pending PR Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }
//        public ActionResult QuotationFinalization(long Group_Id)
//        {
//            var PR = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            PR.ForEach(x => x.SKU = this.ItemCode(x.Item_Id));
//            ViewBag.PurchaseReqId = PR;
//            var res = db.SP_GET_Applied_Quotes_For_PR_Group(Group_Id).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Qutation Finalization Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Group_Id);
//            return View(new QuotationsViewModel { AppliedBids = res, PurchaseRequistionDetails = PR });
//        }
//        public ActionResult DemandFinalization(long Group_Id)
//        {
//            var PR = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            ViewBag.PurchaseReqId = PR;
//            var res = db.SP_GET_Applied_Quotes_For_PR_Group(Group_Id).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Demand Finalization Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Group_Id);
//            return View(new QuotationsViewModel { AppliedBids = res, PurchaseRequistionDetails = PR });
//        }
//        public JsonResult SendBackToProc(long GroupId, string Type, string Status, string Reason)
//        {
//            var res = db.Sp_Update_PurchaseReq_Status(GroupId, Status, Type, Reason);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "PR Sent Back To Procurement with Reason " + Reason + " Status: " + Status, "Update", "Activity_Record", ActivityType.Procurement.ToString(), GroupId);
//            return Json(true);
//        }
//        public JsonResult FinalizeQuotations(long Group_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            //var itemsData = new XElement("Biddings", Id.Select(x => new XElement("BidData",
//            //   new XAttribute("Id", x)))).ToString();
//            //var res = db.Sp_Update_QuotationFinalize(itemsData);
//            //db.Sp_Update_Purchase_Req_Status(Group_Id, PurchaseRequisitionStatus.Approved.ToString(), userid);
//            var ret = new { Status = true, Msg = "Quotations are finalized" };
//            return Json(ret);

//        }
//        public JsonResult DeletePurchaseReq(long GroupId)
//        {
//            var res = db.Sp_Delete_PurchaseReq(GroupId);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted PR", "Delete", "Activity_Record", ActivityType.Procurement.ToString(), GroupId);
//            return Json(true);
//        }
//        public JsonResult UpdateCompPurchaseCompRep(List<string> Group)
//        {
//            var str = new XElement("Group", Group.Select(x => new XElement("Id",
//                        new XAttribute("GroupId", x)))).ToString();
//            var res1 = db.Sp_Update_POSummary(str);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Updated PO Summary " + str, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return Json(string.Format("{0:MM/dd/yyyy}", DateTime.Now));
//        }
//        public ActionResult CompPurchaseCompRep(DateTime date)
//        {
//            var res = db.Sp_Get_PurchaseOrder_Date(date).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed purchase order  Page for date: " + date, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View(res);
//        }
//        public ActionResult PurchaseCompritiveReport(long Group_Id)
//        {
//            var PR = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            ViewBag.PurchaseReqId = PR;
//            var res = db.SP_GET_Applied_Quotes_For_PR_Group(Group_Id).ToList();
//            return View(new QuotationsViewModel { AppliedBids = res, PurchaseRequistionDetails = PR });
//        }
//        public ActionResult PurchaseRequisitionPrint(long Group_Id)
//        {
//            var res = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            res.ForEach(x => x.SKU = db.Inventories.Where(y => y.Id == x.Item_Id).Select(z => z.SKU).FirstOrDefault());
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Printed PR", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Group_Id);
//            return View(res);
//        }
//        public ActionResult PO_Invoice()
//        {
//            var res2 = db.Inventory_PurchaseOrder.ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed INventory Purchase Orders Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View(res2);
//        }
//        public JsonResult MarkQuotation(int Id, bool Status, string Remarks)
//        {
//            var uid = User.Identity.GetUserId<long>();
//            var EmpId = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//            var Depid = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => x.Department_Id).FirstOrDefault();
//            //if ()  // Ledgendary Chaipy
//            //{
//            //var res1 = db.Projects.Where(x => x.CreatedBy_Dep == "Development").Select(x => x.Id).ToList();
//            //var res2 = db.Inventory_Bidding.Where(x => x.Id == Id).Select(x => x.Group_Id).FirstOrDefault();
//            //var res3 = db.Inventory_Purchase_Req.Any(x => x.Group_Id == res2 && res1.Contains((long)x.Prj_Id));
//            if (User.IsInRole("Purchase Comparative Approval"))
//            {
//                var res = db.Sp_Update_MarkQuotation(Id, Status, Remarks);
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Updated Qutation " + Remarks, "Read", "Activity_Record", ActivityType.Procurement.ToString(), Id);
//                return Json(new Return { Status = true, Msg = "" });
//            }
//            else
//            {
//                return Json(new Return { Status = false, Msg = "You cannot Approve this Requistion" });

//            }
//            //}
//            //else
//            //{
//            //    var res = db.Sp_Update_MarkQuotation(Id, Status, Remarks);
//            //    return Json(new Return { Status = true, Msg = "" });
//            //}
//        }
//        public JsonResult GetReqcount()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Detail Page of PO's", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (User.IsInRole("Administrator"))
//            {
//                var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res2 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res4 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Quotation_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res5 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Demand_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res6 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id);
//                var res7 = db.Inventory_PurchaseOrder.Where(x => res6.Contains(x.Group_Id) && x.StockEntered == false).GroupBy(x => x.Group_Id).Count();
//                var res = new { PendingApproval = res1, Pending = res2, QuotationFinal = res4, Demand_Approval = res5, PurchaseOrder = res7 };
//                return Json(res);
//            }
//            else
//            {
//                var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res2 = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res4 = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Quotation_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res5 = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Demand_Approval.ToString()).GroupBy(x => x.Group_Id).Count();

//                var res6 = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id);
//                var res7 = db.Inventory_PurchaseOrder.Where(x => res6.Contains(x.Group_Id) && x.StockEntered == false).GroupBy(x => x.Group_Id).Count();

//                var res = new { PendingApproval = res1, Pending = res2, QuotationFinal = res4, Demand_Approval = res5, PurchaseOrder = res7 };
//                return Json(res);
//            }
//        }
//        private string GetProjectDepartment(long? Id)
//        {
//            var dep = db.Projects.Where(x => x.Id == Id).Select(x => x.CreatedBy_Dep).FirstOrDefault();
//            return dep;
//        }
//        public ActionResult Pending_PR()
//        {
//            var res = db.Inventory_Purchase_Req.Where(x => x.Status == PurchaseRequisitionStatus.Pending.ToString() && x.Cancel == null).ToList();
//            res.ForEach(x => x.Project_Department = this.GetProjectDepartment(x.Prj_Id));
//            res.ForEach(x => x.SKU = this.ItemCode(x.Item_Id));
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Pending PR Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView(res);
//        }
//        public ActionResult Sent_to_Dep()
//        {
//            var res = db.Inventory_Purchase_Req.Where(x => x.Status == PurchaseRequisitionStatus.Quotation_Approval.ToString()).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Gen_Po()
//        {
//            //var res = db.Inventory_PurchaseOrder.Where(x => (x.StockEntered == false || x.StockEntered == null) && (x.OtherCharges == null)).ToList();
//            //res.ForEach(x => x.Project_Department = this.GetProjectDepartment(x.Prj_Id));
//            //return PartialView(res);
//            return PartialView();
//        }
//        public ActionResult PurchaseOrderDetails()
//        {
//            return View();
//        }
//        [HttpGet]
//        public JsonResult GetPOGRN(string q)
//        {
//            var allsearch = db.Inventory_Stock_In.Where(x => x.PO_Number.Contains(q) || x.GRN.Contains(q)).Select(x => new { id = x.PO_Number, text = x.PO_Number + " - " + x.GRN }).Distinct().ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }

//        public JsonResult GetIssueNote(string q)
//        {
//            var allsearch = db.Inventory_Stock_Out.Where(x => x.IssueNote_No.Contains(q)).Select(x => new { id = x.IssueNote_No, text = x.IssueNote_No }).Distinct().ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult DeliverInAutoView(string po)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == po).ToList();
//            res.ForEach(x => x.ReceivedQuantity = db.SP_Get_ReceivedQuantity_AgainstPO(x.Id).FirstOrDefault());
//            var ids = new XElement("Deps", res.Select(x => new XElement("Ids",
//             new XAttribute("Id", x.Dep_Id)
//             ))).ToString();
//            var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).ToList();
//            ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);

//            Helpers h = new Helpers();
//            ViewBag.Transaction = h.RandomNumber();
//            return View(res);
//        }
//        public JsonResult SendBackToDepartment(long Group_Id, string PoNumber)
//        {
//            var res = db.Sp_Update_ReversePO(Group_Id, PoNumber);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Purchase Order Sent Back", "Update", "Activity_Record", ActivityType.PO_Send_Back.ToString(), userid);
//            return Json(true);
//        }
//        public ActionResult PurchaseDetails(string po)
//        {
//            var res1 = db.Sp_Get_Inventory_PurchaseOrder_list(po).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Purchase Details Page of " + po, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            //Changed to check HASH
//            if (res1.Count <= 0)
//            {
//                po = string.Concat(po, "-Revised");
//                res1 = db.Sp_Get_Inventory_PurchaseOrder_list(po).ToList();
//            }
//            res1.ForEach(x => x.SKU = this.ItemCode(x.Item_Id));

//            var G = res1.FirstOrDefault().Group_Id;
//            var res2 = db.Inventory_Purchase_Req.Where(x => x.Group_Id == G).ToList();
//            var res = new PO_Req { Req = res2, PO = res1 };
//            return View(res);
//        }
//        public ActionResult AddOtherCharge(long Group_Id, long Vend_Id, string Vend_Name, string Vend_Comp, string PO)
//        {
//            ViewBag.Group_Id = Group_Id;
//            ViewBag.Vend_Id = Vend_Id;
//            ViewBag.PO_Number = PO;
//            ViewBag.Vend_Name = Vend_Name;
//            ViewBag.Vend_Comp = Vend_Comp;
//            return PartialView();
//        }
//        [HttpPost]
//        public JsonResult AddOtherCharges(long Group, string Charges, string Amount, long Vend_Id, string Vend_Name, string Vend_Comp, string PO_Num, string C)
//        {
//            var res = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == PO_Num).FirstOrDefault();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Added Other Charges of Amount RS " + Amount + " For Vendor " + Vend_Name, "Read", "Activity_Record", ActivityType.Procurement.ToString(), Group);
//            if (res.Created_By == null)
//            {
//                Inventory_PurchaseOrder p = new Inventory_PurchaseOrder()
//                {
//                    Group_Id = Group,
//                    Item_Name = Charges,
//                    Qty = 1,
//                    Purchase_Rate = GeneralMethods.RemoveComa(Amount),
//                    OtherCharges = true,
//                    Vendor_Id = Vend_Id,
//                    Vendor_Name = Vend_Name,
//                    Vendor_Comp = Vend_Comp,
//                    PO_Number = (PO_Num.Contains("-Revised")) ? PO_Num : PO_Num + "-Revised",
//                    Prj_Id = res.Prj_Id,
//                    Prj_Name = res.Prj_Name,
//                    Dep_Id = res.Dep_Id,
//                    Dep_Name = res.Dep_Name,
//                    DateTime = res.DateTime,
//                    Currency = C,
//                    Tax = 0
//                };
//                db.Inventory_PurchaseOrder.Add(p);
//                var res1 = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == PO_Num).ToList();
//                foreach (var a in res1)
//                {
//                    a.PO_Number = a.PO_Number.Replace("-Revised", "") + "-Revised";
//                    db.Inventory_PurchaseOrder.Attach(a);
//                    db.Entry(a).Property(x => x.PO_Number).IsModified = true;
//                }
//                db.SaveChanges();
//                return Json(true);
//            }
//            else
//            {
//                Inventory_PurchaseOrder p = new Inventory_PurchaseOrder()
//                {
//                    Group_Id = Group,
//                    Item_Name = Charges,
//                    Qty = 1,
//                    Purchase_Rate = GeneralMethods.RemoveComa(Amount),
//                    OtherCharges = true,
//                    Vendor_Id = Vend_Id,
//                    Vendor_Name = Vend_Name,
//                    Vendor_Comp = Vend_Comp,
//                    PO_Number = (PO_Num.Contains("-Revised")) ? PO_Num : PO_Num + "-Revised",
//                    Prj_Id = res.Prj_Id,
//                    Prj_Name = res.Prj_Name,
//                    Dep_Id = res.Dep_Id,
//                    Dep_Name = res.Dep_Name,
//                    DateTime = res.DateTime,
//                    Currency = C,
//                    Tax = 0
//                };
//                db.Inventory_PurchaseOrder.Add(p);

//                var res1 = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == PO_Num).ToList();
//                foreach (var a in res1)
//                {
//                    a.PO_Number = a.PO_Number.Replace("-Revised", "") + "-Revised";
//                    db.Inventory_PurchaseOrder.Attach(a);
//                    db.Entry(a).Property(x => x.PO_Number).IsModified = true;
//                }

//                db.SaveChanges();

//                return Json(true);
//            }


//        }
//        public ActionResult User_Purchase_Req()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed PR Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }

//        public ActionResult Dep_PurchaseReq()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Departmental PR Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }
//        public ActionResult Dep_PurchaseReq_Search(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Details of Departmental PR Page from " + From + " to date " + To, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (User.IsInRole("Administrator"))
//            {
//                var res2 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//                string Ids = new XElement("Dep", res2.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();

//                var res = db.Sp_Get_Purchase_Req_List(From, To, Ids).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res2 = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long)x.Department_Id).ToList();

//                var ids = new XElement("Dep", res2.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();

//                var res = db.Sp_Get_Purchase_Req_List(From, To, ids).ToList();
//                return PartialView(res);
//            }
//        }

//        public ActionResult Dep_PurchaseOrder()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Departmental PO Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }
//        public ActionResult Dep_PurchaseOrder_Search(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Departmental PO Page from " + From + " To:" + To, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (User.IsInRole("Administrator") || User.IsInRole("Audit"))
//            {
//                var res1 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//                string Ids = new XElement("Dep", res1.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();
//                var res2 = db.Sp_Get_Purchase_Order_List(From, To, Ids).ToList();

//                var xml1 = new XElement("GroupId", res2.Select(x => new XElement("Id",
//                            new XAttribute("Group", x.PO_Number)))).ToString();

//                var res3 = db.Sp_Get_PO_Completed_GRN(From, To, xml1).ToList();

//                var res = new PO_Completed_With_GRN { PO_list = res2, GRN = res3 };
//                return PartialView(res);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res1 = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long)x.Department_Id).ToList();
//                var ids = new XElement("Dep", res1.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();
//                var res2 = db.Sp_Get_Purchase_Order_List(From, To, ids).ToList();

//                var xml1 = new XElement("GroupId", res2.Select(x => new XElement("Id",
//                     new XAttribute("Group", x.PO_Number)))).ToString();

//                var res3 = db.Sp_Get_PO_Completed_GRN(From, To, xml1).ToList();
//                var res = new PO_Completed_With_GRN { PO_list = res2, GRN = res3 };
//                return PartialView(res);
//            }

//        }
//        //////////////////////////////
//        public string ItemCode(long? Id)
//        {
//            var res = db.Inventories.Where(x => x.Id == Id).FirstOrDefault();

//            return (res == null) ? string.Empty : (res.SKU == null) ? string.Empty : res.SKU;
//        }
//        public string RequisitionNO(long? Group_Id)
//        {
//            var res = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).FirstOrDefault();
//            if (res == null)
//            {
//                return "";
//            }
//            return res.Requisition_No;
//        }
//        public ActionResult Pending_PO()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Pending Po Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.StockEntered == false).ToList();

//            //res1.ForEach(x => x.SKU = this.ItemCode(x.Item_Id));
//            res1.ForEach(x => x.Requistion_No = this.RequisitionNO(x.Group_Id));

//            var xml1 = new XElement("GroupId", res1.Select(x => new XElement("Id",
//                   new XAttribute("Group", x.Group_Id)))).ToString();
//            var res2 = db.Sp_Get_PO_Completed_GRN(null, null, xml1).ToList();

//            var res = new POStatuses { PO = res1, GRN = res2 };
//            return PartialView(res);
//        }
//        public ActionResult Partial_PO()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Partial PO Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.StockEntered == false).ToList();
//            res1.ForEach(x => x.SKU = this.ItemCode(x.Item_Id));
//            res1.ForEach(x => x.Requistion_No = this.RequisitionNO(x.Group_Id));

//            var xml1 = new XElement("GroupId", res1.Select(x => new XElement("Id",
//                   new XAttribute("Group", x.Group_Id)))).ToString();
//            var res2 = db.Sp_Get_PO_Completed_GRN(null, null, xml1).ToList();
//            var res = new POStatuses { PO = res1, GRN = res2 };
//            return PartialView(res);
//        }

//        //public JsonResult CancelPO(long grp, string reason)
//        //{
//        //    var res = db.Inventory_PurchaseOrder.Where(x => x.Group_Id == grp).ToList();
//        //    if(res.Any(x=>x.Cancelled != null))
//        //    {
//        //        foreach(var v in res)
//        //        {
//        //            v.Cancelled = true;
//        //            v.CancelledBy = User.Identity.GetUserId<long>();
//        //            v.CancelReason = reason;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        return Json(false);
//        //    }
//        //}

//        public JsonResult DeletePR(long? grp_Id, string reason)
//        {
//            var res = db.Inventory_PurchaseOrder.Where(x => x.Group_Id == grp_Id).Count();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted PR with reason " + reason, "Delete", "Activity_Record", ActivityType.Procurement.ToString(), grp_Id);
//            if (res > 0)
//            {
//                return Json(false);
//            }
//            else
//            {
//                db.Sp_Update_PR_Over_Deletion(grp_Id, reason);
//                return Json(true);
//            }
//        }
//        public ActionResult GenerateBill()
//        {
//            List<Vendor> vendors = db.Vendors.ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Generate Bill  Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            //var list = db.Sp_Get_CA_Head_Search().Select(x => new { id = x.Id, text = x.Text_ChartCode, Parent = x.parent, head = x.Head }).ToList();
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            return View(vendors);
//        }
//        public JsonResult GetVendors_Select(string s)
//        {
//            var result = db.Vendors.Where(x => x.Name.Contains(s)).Select(x => new Vendor_serch { Id = x.Id, Name = x.Name, address = x.Address }).ToList();
//            var dealers = db.Dealerships.Where(x => x.Dealership_Name.Contains(s)).Select(x => new Vendor_serch { Id = x.Id, Name = x.Dealership_Name, address = x.Address }).ToList();
//            foreach (var item in dealers)
//            {
//                result.Add(new Vendor_serch { Id = item.Id, Name = item.Name, address = item.address });
//            }
//            result.Add(new Vendor_serch { Id = -1, Name = "Add new Vendor" });

//            return Json(new { items = result }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetVendors_Select_RFQ(string s)
//        {

//            var result = db.Vendors.Where(x => x.Company.Contains(s)).Select(x => new Vendor_serch { Id = x.Id, Name = x.Company, address = x.Address, Phone = x.Office_Mobile }).ToList();

//            result.Add(new Vendor_serch { Id = -1, Name = "Add new Vendor" });

//            return Json(new { items = result }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult AllBills()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.Sp_Get_Bill_Pending_Entries(comp.Id).ToList();
//            db.Sp_Add_Activity(userid, "Accessed All Bills Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View(res);
//        }
//        public ActionResult AllBillsVendor()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var vouch = db.Sp_Get_Pending_Payables(comp.Id).ToList();
//            var payables = (from x in db.Bills
//                            join y in db.Payables on x.GroupId equals y.GroupId
//                            where y.Remaining > 0 && x.Comp_Id == comp.Id
//                            select y).Distinct().ToList();

//            List<PendingInvoicesModel> voucherlist = new List<PendingInvoicesModel>();
//            foreach (var v in payables)
//            {
//                PendingInvoicesModel data = new PendingInvoicesModel();
//                data.RecHead = GetHead(Convert.ToInt32(vouch.Where(x => x.GroupId == v.GroupId).Select(x => x.Vendor_Id).FirstOrDefault())).Head.ToString();
//                data.RecHeadId = Convert.ToInt32(vouch.Where(x => x.GroupId == v.GroupId).Select(x => x.Vendor_Id).FirstOrDefault());
//                data.InvoiceNumber = vouch.Where(x => x.GroupId == v.GroupId).Select(x => x.Bill_No).FirstOrDefault() + " - " + vouch.Where(x => x.GroupId == v.GroupId).Select(x => x.description).FirstOrDefault() + " - " + vouch.Where(x => x.GroupId == v.GroupId).Select(x => x.Bill_Date).FirstOrDefault().ToLongDateString();
//                data.GroupId = v.GroupId;
//                data.BillDate = Convert.ToDateTime(vouch.Where(x => x.GroupId == v.GroupId).Select(x => x.Bill_Date).FirstOrDefault());
//                data.DueDate = Convert.ToDateTime(vouch.Where(x => x.GroupId == v.GroupId).Select(x => x.Due_Date).FirstOrDefault());
//                data.TotalAmount = v.Amount;
//                data.Paid = v.Paid_Amount;
//                data.Remaining = Convert.ToDecimal(v.Remaining);
//                voucherlist.Add(data);
//            }
//            return PartialView(voucherlist);
//        }
//        public ActionResult PendingBills(DateTime? From, DateTime? To)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Pending Bills Page from " + From + " To " + To, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (From == null || To == null)
//            {
//                var bills = db.Sp_Get_Bills(From, To).ToList();
//                List<Sp_Get_Bills_Result> flt_bills = new List<Sp_Get_Bills_Result>();
//                foreach (var bill in bills)
//                {
//                    if (db.Payables.Where(x => x.GroupId == bill.GroupId && x.Remaining > 0).FirstOrDefault() != null)
//                    {
//                        flt_bills.Add(bill);
//                    }
//                }
//                return PartialView(flt_bills);
//            }
//            else
//            {
//                var bills = db.Sp_Get_Bills(From, To).ToList();
//                List<Sp_Get_Bills_Result> flt_bills = new List<Sp_Get_Bills_Result>();
//                foreach (var bill in bills)
//                {
//                    if (db.Payables.Where(x => x.GroupId == bill.GroupId && x.Remaining > 0).FirstOrDefault() != null)
//                    {
//                        flt_bills.Add(bill);
//                    }
//                }
//                return PartialView(flt_bills);
//            }
//        }
//        public ActionResult UnsupervisedInvoices()
//        {
//            var res = db.Sp_Get_Pending_Invoice_Entries().ToList();
//            return View(res);
//        }
//        public ActionResult ReceivableNeedsSupervision(long Id)
//        {
//            var res2 = db.Sp_Get_GeneralEntries_Parameter(Id).ToList();
//            var res3 = db.Invoices.Where(x => x.PO_Group_Id == Id).ToList();
//            var res4 = GetHead(Convert.ToInt32(res3.FirstOrDefault().Vendor_Id));
//            var res = new ReceivableVouchers { General_Entries = res2, InvoiceItems = res3, vendor = res4 };
//            return View(res);
//        }
//        public ActionResult PrintRFQ(long Group_Id, long Id)
//        {
//            var req = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            var result = db.Vendors.Where(x => x.Id == Id).Select(x => new Vendor_serch { Id = x.Id, Name = x.Name, address = x.Address, Phone = x.Office_Mobile }).FirstOrDefault();

//            ViewBag.Name = result.Name;
//            ViewBag.address = result.address;
//            ViewBag.Phone = result.Phone;
//            long userid = long.Parse(User.Identity.GetUserId());
//            var emp = db.Employees.Where(x => x.loginId == userid).FirstOrDefault();
//            ViewBag.User = emp.Name;
//            ViewBag.Phonenum = emp.Mobile_1;
//            return PartialView(req);
//        }
//        public ActionResult PrReport()
//        {
//            var res = db.Sp_Get_PR_report().ToList();
//            return PartialView(res);
//        }
//        public ActionResult VendorReport()
//        {
//            var res = db.Sp_Get_Vendor_report().ToList();
//            return PartialView(res);
//        }
//        public ActionResult RecordInvoice()
//        {
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            return View();
//        }
//        public JsonResult GenerateNewInvoice(List<Bill_Details> Details, DateTime DueDate, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            string invoice = new XElement("Invoices", Details.Select(x => new XElement("Entries",
//            new XAttribute("Invoice_Amount", ((x.Quantity == null) ? 0 : x.Quantity) * ((x.Rate == null) ? 0 : x.Rate)),
//            new XAttribute("PO_Amount", ((x.Quantity == null) ? 0 : x.Quantity) * ((x.Rate == null) ? 0 : x.Rate)),
//            new XAttribute("Advance_Paid", 0),
//            new XAttribute("Paid", ((x.Quantity == null) ? 0 : x.Quantity) * ((x.Rate == null) ? 0 : x.Rate)),
//            new XAttribute("Vendor", GetHead(x.VendorId).Head),
//            new XAttribute("Vendor_Id", x.VendorId),
//            new XAttribute("PO_Number", x.BillNo),
//            new XAttribute("PO_Group_Id", TransactionId),
//            new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//            new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//            new XAttribute("Description", x.description),
//            new XAttribute("ItemHead", GetHead(x.Head_id).Head),
//                new XAttribute("Comp_Id", comp.Id),
//            new XAttribute("ItemHeadId", x.Head_id)
//            ))).ToString();

//            try
//            {
//                var res = db.Sp_Add_Invoice(invoice, userid, DueDate);
//                decimal? amount = 0;
//                if (Details.Any())
//                {
//                    foreach (var item in Details)
//                    {
//                        decimal? rate = (item.Rate == null) ? 0 : item.Rate;
//                        decimal? qty = (item.Quantity == null) ? 0 : item.Quantity;
//                        amount += rate * qty;
//                    }
//                }

//                var vendor_coa = GetHead(Details.FirstOrDefault().VendorId);

//                var GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
//                new XAttribute("Naration", x.description),
//                new XAttribute("Head", (x.Head == null) ? "" : x.Head),
//                new XAttribute("Head_Name", GetHead(x.Head_id).Head),
//                new XAttribute("Head_Code", GetHead(x.Head_id).Text_ChartCode),
//                new XAttribute("Head_Id", x.Head_id),
//                new XAttribute("Line", x.Line),
//                new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//                new XAttribute("UOM", ""),
//                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                new XAttribute("Debit", 0),
//                new XAttribute("Credit", ((x.Rate == null) ? 0 : x.Rate) * ((x.Quantity == null) ? 0 : x.Quantity)),
//                new XAttribute("Comp_Id", comp.Id),
//                new XAttribute("GroupId", TransactionId)
//                )));
//                var GEapp = new XElement("GeneralEntries", new XElement("Entries",
//                new XAttribute("Naration", "Receivable Record Against Invoice No. " + Details.FirstOrDefault().BillNo + " for " + vendor_coa.Head),
//                new XAttribute("Head", vendor_coa.Text_ChartCode + " - " + vendor_coa.Head),
//                new XAttribute("Head_Name", vendor_coa.Head),
//                new XAttribute("Head_Code", vendor_coa.Text_ChartCode),
//                new XAttribute("Head_Id", vendor_coa.Id),
//                new XAttribute("Line", Details.Count() + 1),
//                new XAttribute("Qty", 0),
//                new XAttribute("UOM", ""),
//                new XAttribute("Rate", 0),
//                new XAttribute("Debit", amount),
//                new XAttribute("Credit", 0),
//                new XAttribute("Comp_Id", comp.Id),
//                new XAttribute("GroupId", TransactionId)
//                ));
//                GeneralEnt.Add(
//                    from ge in GEapp.Elements()
//                    select ge
//                    );
//                var res1 = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
//                return Json(new { Status = true, Msg = "Receivable Entry Recorded." });
//            }
//            catch (Exception ex)
//            {
//                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new { Status = false, Msg = "Receivable entry couldn't recorded. Please try later" });
//            }
//        }
//        public ActionResult InvoiceList()
//        {
//            var invoices = db.Sp_Get_Pending_Invoices().ToList();
//            var receivables = db.Receivables.Where(x => x.Remaining > 0).ToList();


//            List<PendingInvoicesModel> invoicelist = new List<PendingInvoicesModel>();
//            foreach (var v in receivables)
//            {
//                PendingInvoicesModel pim = new PendingInvoicesModel();
//                pim.RecHead = invoices.Where(x => x.PO_Group_Id == v.GroupId).Select(x => x.Vendor).FirstOrDefault();
//                pim.RecHeadId = Convert.ToInt32(invoices.Where(x => x.PO_Group_Id == v.GroupId).Select(x => x.Vendor_Id).FirstOrDefault());
//                pim.InvoiceNumber = invoices.Where(x => x.PO_Group_Id == v.GroupId).Select(x => x.PO_Number).FirstOrDefault();
//                pim.GroupId = v.GroupId;
//                pim.BillDate = Convert.ToDateTime(invoices.Where(x => x.PO_Group_Id == v.GroupId).Select(x => x.Recorded_On).FirstOrDefault());
//                pim.DueDate = Convert.ToDateTime(invoices.Where(x => x.PO_Group_Id == v.GroupId).Select(x => x.DueDate).FirstOrDefault());
//                pim.TotalAmount = v.Amount;
//                pim.Paid = v.Paid_Amount;
//                pim.Remaining = Convert.ToDecimal(v.Remaining);

//                invoicelist.Add(pim);
//            }
//            return View(invoicelist);
//        }
//        public ActionResult ReceivePayment(long Id, int HeadId)
//        {
//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            var res2 = db.Sp_Get_JournalEntriesPayable_Parameter(Id, HeadId).ToList();
//            var res3 = db.Sp_Get_Receivables_Head(Id, HeadId).SingleOrDefault();
//            ViewBag.Head = HeadId;
//            ViewBag.HeadName = res2.FirstOrDefault().Head; //Make separate method for this if res2 stored procedure change due to style.
//            ViewBag.Amount = res3.Amount;
//            ViewBag.Paid_Amount = res3.Paid_Amount;
//            ViewBag.Remaining = res3.Remaining;
//            var res = db.Sp_Get_Liability_Head_Balance(HeadId).FirstOrDefault();
//            if (res == null)
//            {
//                ViewBag.TotalRemaining = 0;
//            }
//            else
//            {
//                ViewBag.TotalRemaining = res.Credit - res.Debit;
//            }
//            Helpers h = new Helpers();
//            ViewBag.TransactionId = h.RandomNumber();
//            return PartialView(res2);
//        }
//        public JsonResult AddNewReceivable(List<VoucherPayableItems> vpi, int payAccId, string receivedBy, string instNo, DateTime? instDate, string branch, string payType, decimal Amount)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var username = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
//            var res1 = GetHead(payAccId);
//            var paytype = "Cash";
//            var bank = "";
//            var bankBranch = "";
//            if (res1.Head != "Cash")
//            {
//                bank = res1.Head;
//                paytype = payType;
//                bankBranch = branch;
//            }
//            var headId = vpi.FirstOrDefault().HeadId;
//            var grpId = vpi.FirstOrDefault().GroupId;
//            var narration = "Received for - " + vpi.FirstOrDefault().Narration;
//            string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Amount));
//            Helpers H = new Helpers();
//            var trans = H.RandomNumber();
//            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var r = db.Sp_Add_Receipt(Amount, AmountinWords, bank, instNo, instDate, branch, "", "", null, receivedBy, paytype, null, comp.Company_Name, null, "", "",
//                        ReceiptTypes.Receivable_Receipt.ToString(), grpId, userid, narration, null, Modules.Accounts_Management.ToString(), "", headId.ToString(), "", "", null, trans, "", receiptno, comp.Id).FirstOrDefault();

//                    //var r = db.Sp_Add_Voucher_Vendor("-", Amount, AmountinWords, bank, bankBranch, instDate, instNo, "", narration, "", null, Modules.Vendor_Payment.ToString(), receivedBy,
//                    //    paytype, "Meher Estate Developers", "", grpId, "Payment Voucher", userid, null, grpId, username, userid, DateTime.Now, "", null, null).FirstOrDefault();

//                    if (payType != "Cash")
//                    {
//                        var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(Amount, bank, branch, payType, null, null, PaymentMethodStatuses.Pending.ToString(),
//                                    Modules.Accounts_Management.ToString(), Types.PaymentReceivable.ToString(), userid, instNo, null, instDate, "", r.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());

//                        //if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
//                        //{
//                        //    Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
//                        //}
//                        //string dt = string.Format("{0:d/M/yyyy}", instDate);
//                        //var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), instNo + "_" + bank + "_" + dt.Replace("/", "_") + ".jpg");
//                        //var Imges = H.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());
//                    }

//                    bool headcashier = false;
//                    if (User.IsInRole("Head Cashier"))
//                    {
//                        headcashier = true;
//                    }
//                    db.Sp_Update_Receivable_At_Receiving(grpId, headId, Amount);
//                    //AccountHandlerController de = new AccountHandlerController();
//                    //de.PR_Receive_Receivable_Amount(Amount, paytype, instNo, instDate, bank, narration, trans, userid, r.Receipt_No.ToString(), 1, headId, headcashier);

//                    Transaction.Commit();
//                    return Json(new { Status = true, Msg = "Receivable Entry Successful.", VoucherId = r.Receipt_Id, Token = grpId });
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Error Occured" });
//                }
//            }
//        }
//        public Sp_Get_CA_Head_Parameter_Result GetHead(int Id)
//        {
//            var res = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
//            return res;
//        }
//        public ActionResult GenerateRFQ(long Group_Id)
//        {
//            var req = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Generated RQF For PR " + req.Select(x => x.Requisition_No), "Create", "Activity_Record", ActivityType.Inventory.ToString(), userid);

//            var emp = db.Employees.Where(x => x.loginId == userid).FirstOrDefault();
//            ViewBag.User = emp.Name;
//            ViewBag.Phonenum = emp.Mobile_1;
//            return PartialView(req);
//        }
//        public ActionResult ReceiveOpenPayment()
//        {
//            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
//            ViewBag.Amount = 0.00;
//            ViewBag.Paid_Amount = 0.00;
//            ViewBag.Remaining = 0.00;
//            ViewBag.TotalRemaining = 1000.00;
//            return View();
//        }
//        public JsonResult OpenReceivableEntry(int payAccId, string paidBy, string instNo, string InstBank, DateTime? instDate, string branch, string payType, decimal? Amount, string Description)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var username = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
//            var res1 = GetHead(payAccId);
//            var grpId = new Helpers().RandomNumber();
//            string AmountinWords = GeneralMethods.NumberToWords(Convert.ToInt32(Amount));
//            Helpers H = new Helpers();
//            var trans = H.RandomNumber();
//            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var r = db.Sp_Add_Receipt(Amount, AmountinWords, InstBank, instNo, instDate, branch, "", "", null, paidBy, payType, null, "Meher Estate Developers", null, "", "",
//                        ReceiptTypes.Receivable_Receipt.ToString(), grpId, userid, Description, null, Modules.Accounts_Management.ToString(), "", payAccId.ToString(), "", "", null, trans, "", receiptno, comp.Id).FirstOrDefault();

//                    if (payType != "Cash")
//                    {
//                        var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(Amount, InstBank, branch, payType, null, null, PaymentMethodStatuses.Pending.ToString(),
//                                    Modules.Accounts_Management.ToString(), Types.PaymentReceivable.ToString(), userid, instNo, null, instDate, "", r.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
//                    }
//                    bool headcashier = false;
//                    if (User.IsInRole("Head Cashier"))
//                    {
//                        headcashier = true;
//                    }
//                    AccountHandlerController de = new AccountHandlerController();
//                    de.PR_Receive_Receivable_Amount(Amount, payType, instNo, instDate, InstBank, Description + " - Paid By: " + paidBy, trans, userid, r.Receipt_No.ToString(), 1, payAccId, headcashier);

//                    Transaction.Commit();
//                    return Json(new { Status = true, Msg = "Receivable Entry Successful.", VoucherId = r.Receipt_Id, Token = grpId });
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Error Occured" });
//                }
//            }
//        }
//        public ActionResult PurchaseOrders()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Pending Po Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            var res1 = db.Inventory_PurchaseOrder.Where(x => (x.StockEntered == false || x.StockEntered == null) && x.Created_By != null).ToList();

//            res1.ForEach(x => x.Requistion_No = this.RequisitionNO(x.Group_Id));
//            var res = new POStatuses { PO = res1};
//            return View(res);
//        }
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//        //public ActionResult InventoryReport()
//        //{
//        //    return View();
//        //}
//        //public ActionResult InventoryReport_Search(string itemCode)
//        //{
//        //    long userid = User.Identity.GetUserId<long>();
//        //    //db.Sp_Add_Activity(userid, "Generate Purchase Requsition Report ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//        //    var res = db.Sp_Get_Inventory_prcrmt_Report(itemCode).ToList();
//        //    return PartialView(res);
//        //}
//        //public ActionResult InventorQuantitativeDataReport()
//        //{
//        //    return View();
//        //}
//        //public ActionResult InventorQuantitativeDataReport_Search(string Itemcode)
//        //{
//        //    long userid = User.Identity.GetUserId<long>();
//        //    //db.Sp_Add_Activity(userid, "Generate Purchase Requsition Report ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//        //    var res = db.Sp_Get_Inventory_QnttvData_Report(Itemcode).ToList();
//        //    return PartialView(res);
//        //}


//    }
//}