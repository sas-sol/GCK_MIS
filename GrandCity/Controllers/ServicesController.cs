//using MeherEstateDevelopers.Filters;
//using MeherEstateDevelopers.Models;
//using Microsoft.AspNet.Identity;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Xml.Linq;

//namespace MeherEstateDevelopers.Controllers
//{
//    [Authorize]
//    [LogAction]
//    public class ServicesController : Controller
//    {
//        // GET: Services

//        private Grand_CityEntities db = new Grand_CityEntities();
//        public ActionResult Services()
//        {
//            return View();
//        }
//         public ActionResult NewServiceRequisition(long? Id)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", Id).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            return View(res);
//        }
//        public ActionResult NewService_Requisition(long? Id)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", Id).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            return View(res);
//        }
//        public ActionResult NewService_Requisition_For_Other(long? Id , long? Req)
//        {
//            long usserid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(usserid, "Accessed New PR Page for " + Req, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
//            var res1 = db.Sp_Get_EmployeeId(usserid).FirstOrDefault();
//            var res2 = db.Employee_Designations.Where(x => x.Emp_Id == res1).Select(x => (long)x.Department_Id).ToList();

//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", Id).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");

//            if (User.IsInRole("All Department") || User.IsInRole("Administrator"))
//            {
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == "Department"), "Id", "Name", Id);
//            }
//            else
//            {
//                var prjs = db.Prj_Attendees.Where(x => x.Userid == res1).Select(x => x.PrjId).Distinct().ToList();
//                ViewBag.Department = new SelectList(db.Projects.Where(x => prjs.Contains(x.Id)).ToList(), "Id", "Name", Id);

//            }

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            return View(res);
//        }
//        [HttpPost]
//        public JsonResult NewServiceRequisitionForOther(List<Services_Purchase_Req> demand, long prj , int Department)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var depart = db.Comp_Dep_Desig.Where(x => x.Id == Department).FirstOrDefault();
//                var des = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();


//                var purItems = new XElement("Products", demand.Select(x => new XElement("ProductInfo",
//                    new XAttribute("itemName", x.Item_Name),
//                   (x.Length == null) ? null : new XAttribute("Len", x.Length),
//                   (x.Len_UOM == null) ? null : new XAttribute("Len_UOM", x.Len_UOM),
//                    (x.Width == null) ? null : new XAttribute("Wid", x.Width),
//                    (x.Wid_UOM == null) ? null : new XAttribute("Wid_UOM", x.Wid_UOM),
//                    (x.Heigth == null) ? null : new XAttribute("Hei", x.Heigth),
//                    (x.Hei_UOM == null) ? null : new XAttribute("Hei_UOM", x.Hei_UOM),
//                    (x.Diameter == null) ? null : new XAttribute("Dia", x.Diameter),
//                    (x.Dia_UOM == null) ? null : new XAttribute("Dia_UOM", x.Dia_UOM),
//                    (x.Size == null) ? null : new XAttribute("Size", x.Size),
//                    (x.S_UOM == null) ? null : new XAttribute("S_UOM", x.S_UOM),
//                    (x.Qty == null) ? null : new XAttribute("Qty", x.Qty),
//                    (x.UOM == null) ? null : new XAttribute("UOM", x.UOM),
//                    (depart.Id == null) ? null : new XAttribute("Dep_Id", des.Id),
//                    (depart.Name == null) ? null : new XAttribute("Dep_Name", depart.Name),
//                    new XAttribute("Grp", x.Group_Id),
//                    (x.Description == null) ? null : new XAttribute("Descr", x.Description),
//                    (des.DesignationName == null) ? null : new XAttribute("RequestedBy_Desgination", des.DesignationName),
//                    new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                var res = db.Sp_Add_ServiceRequisition(purItems, prj, uid).ToList();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Added New Service requisition ", "Create", "Activity_Record", ActivityType.Services.ToString(), userid);

//                if (res.Count == demand.Count)
//                {
//                    return Json(new { Status = true, Msg = "Service Requisition has been applied successfully and Send for approval." });
//                }
//                else
//                {
//                    return Json(new { Status = false, Msg = "Service Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                }
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Service Requisition Error");
//                string msg = ex.Message;
//                return Json(new { Status = false, Msg = "Error in saving Service requisition." });
//            }
//        }
//        [HttpPost]
//        public JsonResult NewServiceRequisition(List<Services_Purchase_Req> demand, long prj)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var des = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();

//                var purItems = new XElement("Products", demand.Select(x => new XElement("ProductInfo",
//                    new XAttribute("itemName", x.Item_Name),
//                   (x.Length == null) ? null : new XAttribute("Len", x.Length),
//                   (x.Len_UOM == null) ? null : new XAttribute("Len_UOM", x.Len_UOM),
//                    (x.Width == null) ? null : new XAttribute("Wid", x.Width),
//                    (x.Wid_UOM == null) ? null : new XAttribute("Wid_UOM", x.Wid_UOM),
//                    (x.Heigth == null) ? null : new XAttribute("Hei", x.Heigth),
//                    (x.Hei_UOM == null) ? null : new XAttribute("Hei_UOM", x.Hei_UOM),
//                    (x.Diameter == null) ? null : new XAttribute("Dia", x.Diameter),
//                    (x.Dia_UOM == null) ? null : new XAttribute("Dia_UOM", x.Dia_UOM),
//                    (x.Size == null) ? null : new XAttribute("Size", x.Size),
//                    (x.S_UOM == null) ? null : new XAttribute("S_UOM", x.S_UOM),
//                    (x.Qty == null) ? null : new XAttribute("Qty", x.Qty),
//                    (x.UOM == null) ? null : new XAttribute("UOM", x.UOM),
//                    (des.DepartmentId == null) ? null : new XAttribute("Dep_Id", des.DepartmentId),
//                    (des.DepartmentName == null) ? null : new XAttribute("Dep_Name", des.DepartmentName),
//                    new XAttribute("Grp", x.Group_Id),
//                    (x.Description == null) ? null : new XAttribute("Descr", x.Description),
//                    (des.DesignationName == null) ? null : new XAttribute("RequestedBy_Desgination", des.DesignationName),
//                    new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                var res = db.Sp_Add_ServiceRequisition(purItems, prj, uid).ToList();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Added New Service requisition ", "Create", "Activity_Record", ActivityType.Services.ToString(), userid);

//                if (res.Count == demand.Count)
//                {
//                    return Json(new { Status = true, Msg = "Service Requisition has been applied successfully and Send for approval." });
//                }
//                else
//                {
//                    return Json(new { Status = false, Msg = "Service Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                }
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Service Requisition Error");
//                string msg = ex.Message;
//                return Json(new { Status = false, Msg = "Error in saving Service requisition." });
//            }
//        }
//        public ActionResult NewQuotations()
//        {
//            return PartialView();
//        }
//        public ActionResult PendingApproval()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var res = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var res = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//        }
//        public ActionResult Pending_PR()
//        {
//            var res = db.Services_Purchase_Req.Where(x => x.Status == PurchaseRequisitionStatus.Pending.ToString()).ToList();
//            return PartialView(res);
//        }
//        public ActionResult QuotationFinalize()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator") || User.IsInRole("All Projects"))
//            {
//                var res = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Quotation_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var res = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Quotation_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//        }
//        public ActionResult Gen_Po()
//        {
//            var res = db.Service_PurchaseOrder.Where(x => x.Completed == false).ToList();
//            return PartialView(res);
//        }
//        public JsonResult UpdatePurchaseReq(long Group_Id, string Status)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Update_Ser_Req_Status(Group_Id, Status, userid);
      
//            db.Sp_Add_Activity(userid, "Update Purchase requisition ", "Update", "Activity_Record", ActivityType.Services.ToString(), userid);

//            return Json(true);
//        }
//        public JsonResult GetSerCount()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var res1 = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res2 = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res4 = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Quotation_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res6 = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id);
//                var res7 = db.Service_PurchaseOrder.Where(x => res6.Contains(x.Group_Id) && x.Completed == false).GroupBy(x => x.Group_Id).Count();

//                var res = new { PendingApproval = res1, Pending = res2, QuotationFinal = res4, PurchaseOrder = res7 };
//                return Json(res);
//            }
//            else
//            {
//                var res1 = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending_Approval.ToString()).GroupBy(x => x.Group_Id).Count();

//                var res2 = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending.ToString()).GroupBy(x => x.Group_Id).Count();

//                var res4 = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Quotation_Approval.ToString()).GroupBy(x => x.Group_Id).Count();

//                var res6 = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id);
//                var res7 = db.Service_PurchaseOrder.Where(x => res6.Contains(x.Group_Id) && x.Completed == false).GroupBy(x => x.Group_Id).Count();

//                var res = new { PendingApproval = res1, Pending = res2, QuotationFinal = res4, PurchaseOrder = res7 };
//                return Json(res);
//            }
//        }
//        public JsonResult DeleteServiceReq(long GroupId)
//        {
//            var res = db.Sp_Delete_ServiceReq(GroupId);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted Service Requisition ", "Delete", "Activity_Record", ActivityType.Services.ToString(), GroupId);

//            return Json(true);
//        }
//        public ActionResult BidsListing(long Group)
//        {
//            var PR = db.Services_Purchase_Req.Where(x => x.Group_Id == Group).ToList();
//            ViewBag.PurchaseReqId = PR;
//            var res = db.Sp_GET_Applied_Quotes_For_SR_Group(Group).ToList();
//            return View(new QuotationServicesViewModel { AppliedBids = res, PurchaseRequistionDetails = PR });
//        }
//        public JsonResult PO_Terms(long Id, long Group_Id)
//        {
//            var res1 = db.Inventory_PO_Terms.Where(x => x.Req_Id == Group_Id).ToList();
//            var res2 = db.Service_Bidding.Where(x => x.Id == Id).FirstOrDefault();

//            var res = new { Terms = res1, Image = res2 };
//            return Json(res);
//        }
//        public ActionResult AddNewQuotation(long Req_Id,long GroupId, string Item)
//        {
//            ViewBag.Req_Id = Req_Id;
//            ViewBag.GroupId = GroupId;
//            ViewBag.Item = Item;
//            return PartialView();
//        }
//        public JsonResult AddQuotation(long V,  decimal R,decimal? Q, string U,decimal? T, string C, string Rem, long Group_Id,long Req_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var Ven = db.Vendors.Where(x => x.Id == V).FirstOrDefault();
//            var VenExists = db.Service_Bidding.Where(x => x.Vendor_Id == V && x.ServiceReq_Id == Req_Id).Any();
//            if (VenExists)
//            {
//                var ret = new { Status = false, Msg = "Vendor is Already Added" };
//                return Json(ret);
//            }
//            var Re_Id = db.Services_Purchase_Req.Where(x => x.Id == Req_Id).ToList();

//            var itemsData = new XElement("Biddings", Re_Id.Select(x => new XElement("BidData",
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
//                        (U is null) ? null : new XAttribute("Uom", U),
//                        (T is null) ? new XAttribute("Tax", 0) : new XAttribute("Tax", T),
//                        (C is null) ? null : new XAttribute("Currency", C),
//                        (Q == null) ? null : new XAttribute("Qty", Q),
//                        (x.Dep_Id is null) ? null : new XAttribute("Dep_Id", x.Dep_Id),
//                        (x.Dep_Name is null) ? null : new XAttribute("Dep_Name", x.Dep_Name),
//                        new XAttribute("GroupId", x.Group_Id),
//                        new XAttribute("ServiceReq_Id", Req_Id),
//                        new XAttribute("VendorId", Ven.Id),
//                        new XAttribute("VendorName", Ven.Company),
//                        (x.Description is null) ? null : new XAttribute("descr", Rem),
//                        new XAttribute("preq", x.Id),
//                        new XAttribute("prate", R)))).ToString();
//            var res1 = db.Sp_Add_Ser_Bids(itemsData, userid, null).FirstOrDefault();
            
//            db.Sp_Add_Activity(userid, "Added New Qutation ", "Create", "Activity_Record", ActivityType.Services.ToString(), userid);

//            var cred_term = db.Vendor_Credit_Terms.Where(x => x.Vendor_Id == V).ToList();
//            foreach (var item in cred_term)
//            {
//                var a = Convert.ToDouble(item.Days);
//                var res2 = this.SaveTerm(item.Terms, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(a).Day), Group_Id, V);
//            }

//            int index = 0;
//            foreach (string file in Request.Files)
//            {
//                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
//                index++;
//                if (hpf.ContentLength == 0)
//                    continue;
//                var imageName = Path.GetExtension(hpf.FileName);
//                if (!Directory.Exists(Server.MapPath("~/Repository/Procurement/Services/" + Group_Id + "/" + res1.Id + "")))
//                {
//                    Directory.CreateDirectory(Server.MapPath("~/Repository/Procurement/Services/" + Group_Id + "/" + res1.Id + ""));
//                }
//                string savedFileName = Path.Combine(Server.MapPath("~/Repository/Procurement/Services/" + Group_Id + "/" + res1.Id + "/"), hpf.FileName);
//                hpf.SaveAs(savedFileName);
//                db.Sp_Update_Service_QuoteImg(hpf.FileName, res1.Id);
//            }
//            var res = new { Status = true, Msg = "Successfully Saved" };
//            return Json(res);

//        }
//        public JsonResult DeleteBid(long bid)
//        {
//            try
//            {
//                db.Sp_Delete_Bid(bid, "Services");
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Deleted Bid ", "Delete", "Activity_Record", ActivityType.Services.ToString(), bid);

//                return Json(true);
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Remove Quotation Error");
//                return Json(false);
//            }
//        }
//        public JsonResult GeneratePO(long Group_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var res = db.Service_Bidding.Where(x => x.Group_Id == Group_Id && x.IsFinalized == true && x.PO_Generated == null).ToList();
//            var req = db.Services_Purchase_Req.Where(x => x.Group_Id == Group_Id).FirstOrDefault();
//            //List<string> PoList = new List<string>();
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    foreach (var item in res.GroupBy(x => x.Vendor_Id))
//                    {
//                        var itemsData = new XElement("Biddings", item.Select(x => new XElement("BidData",
//                           new XAttribute("itemName", x.Item_Name),
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
//                           (x.Qty is null) ? null : new XAttribute("Qty", x.Qty),
//                           (x.Dep_Id is null) ? null : new XAttribute("Dep_Id", x.Dep_Id),
//                           (x.Dep_Name is null) ? null : new XAttribute("Dep_Name", x.Dep_Name),
//                           //(res2.DesignationName is null) ? null : new XAttribute("CreatedBy_Designation", res2.DesignationName),
//                           (req.Prj_Id is null) ? null : new XAttribute("Prj_Id", req.Prj_Id),
//                           (req.Prj_Name is null) ? null : new XAttribute("Prj_Name", req.Prj_Name),
//                           (req.Prj_Offsite is null) ? null : new XAttribute("Prj_Offsite", req.Prj_Offsite),
//                           (req.ApprovedBy_Name is null) ? null : new XAttribute("ApprovedBy_Name", req.ApprovedBy_Name),
//                           (req.ApprovedBy_Designation is null) ? null : new XAttribute("ApprovedBy_Designation", req.ApprovedBy_Designation),
//                           //new XAttribute("Qty", x.Qty),
//                           new XAttribute("GroupId", x.Group_Id),
//                           new XAttribute("VendorId", x.Vendor_Id),
//                           new XAttribute("VendorName", x.Vendor_Name),
//                           new XAttribute("bid", x.Id),
//                           new XAttribute("prate", x.PurchaseRate)))).ToString();
//                        var res3 = db.Sp_Add_PurchaseOrders_Ser(itemsData, userid, PurchaseRequisitionStatus.Purchase_Order_NotPrinted.ToString()).ToList();
                      
//                        db.Sp_Add_Activity(userid, "Created New PO ", "Create", "Activity_Record", ActivityType.Services.ToString(), Group_Id);

//                        //var vendor = db.Vendors.Where(x => x.Id == item.Key).FirstOrDefault();
//                        //var res4 = db.Sp_Get_CA_Head_MultiRef("Liability", "Trade Payables").FirstOrDefault();

//                        //if (!db.Sp_Get_CA_Head_MultiRef("Liability", vendor.Company).Any())
//                        //{
//                        //    var res6 = db.Sp_Add_CA_Head(res4.Id, vendor.Company, "Vendor", vendor.Id);
//                        //}

//                        //PoList.AddRange(res3.Select(x => x.ponum));
//                    }
//                    Transaction.Commit();
//                    //var ret = new { Status = true, Msg = "Purchase Order Generated", PoNum = PoList };
//                    var ret = new { Status = true, Msg = "Purchase Requisition is Finalized" };
//                    return Json(ret);
//                }
//                catch (Exception ex)
//                {
//                    EmailService e = new EmailService();
//                    e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Purchase Order Creation Error");
//                    Transaction.Rollback();
//                    var ret = new { Status = false, Msg = "Error Occured" };
//                    return Json(ret);
//                }
//            }
//        }
//        public JsonResult PrintPO(long Group_Id, string PO_Num)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var res2 = db.Sp_Get_EmployeeFirstDesignation(res1).FirstOrDefault();

//            db.Sp_Update_Ser_PO_PrintBy(userid, res2.DesignationName, Group_Id, null, PO_Num);
//            var res = db.Service_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.PO_Number == PO_Num).Select(x => x.PO_Number).Distinct().ToList();
//            return Json(res);
//        }
//        public JsonResult SaveTerm(string Term, DateTime? Date, long Bid_Id, long Vendor)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = Convert.ToBoolean(db.Sp_Add_POTerms(Term, Date, userid, Bid_Id, Vendor).FirstOrDefault());
          
//            db.Sp_Add_Activity(userid, "Updated Term for Vendor ", "Update", "Activity_Record", ActivityType.Services.ToString(), Vendor);

//            if (res)
//            {
//                var ret = new { Status = true, Msg = "Successfully Added" };
//                return Json(ret);
//            }
//            else
//            {
//                var ret = new { Status = false, Msg = "Already Exists" };
//                return Json(ret);
//            }
//        }
//        public ActionResult QuotationFinalization(long Group_Id)
//        {
//            var PR = db.Services_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            ViewBag.SerReqId = PR;
//            var res = db.Sp_GET_Applied_Quotes_For_SR_Group(Group_Id).ToList();
//            return View(new QuotationServicesViewModel { AppliedBids = res, PurchaseRequistionDetails = PR });
//        }
//        public JsonResult MarkQuotation(int Id, bool Status, string Remarks)
//        {
//            var res = db.Sp_Update_MarkQuotation_Ser(Id, Status,Remarks);
//            return Json(true);
//        }
//        public ActionResult PrintPurchaseOrder(string poNum)
//        {
//            try
//            {
//                var poData = db.Service_PurchaseOrder.Where(x=> x.PO_Number == poNum).ToList();
//                var venId = poData.Select(y => new { y.Vendor_Id, y.Group_Id }).FirstOrDefault();
//                var res = db.Vendors.Where(x => x.Id == venId.Vendor_Id).FirstOrDefault();
//                var terms = db.Inventory_PO_Terms.Where(x => x.Req_Id == venId.Group_Id).ToList();
//                var uid = User.Identity.GetUserId<long>();
//                ViewBag.unm = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
//                return View(new PurchaseOrderPrintViewSer { PO_Data = poData, Vendor_Data = res, PO_Terms = terms });

//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "SaveIssueRequest Demand Order Error");
//                return View();
//            }
//            //var res = db.Sp_Get_PurchaseOrder(Token, Id).ToList();
//            //return View(res);
//        }
//        public JsonResult UpdateCompPurchaseCompRep(List<string> Group)
//        {
//            var str = new XElement("Group", Group.Select(x => new XElement("Id",
//                        new XAttribute("GroupId", x)))).ToString();
//            var res1 = db.Sp_Update_POSummary_Ser(str);
//            return Json(string.Format("{0:MM/dd/yyyy}", DateTime.Now));
//        }
//        public ActionResult CompPurchaseCompRep(DateTime date)
//        {
//            var res = db.Sp_Get_PurchaseOrder_Ser_Date(date).ToList();
//            return View(res);
//        }
//        public JsonResult SendBackToDepartment(long Group_Id, string PoNumber)
//        {
//            var res = db.Sp_Update_ReverseWO(Group_Id, PoNumber);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Purchase Order Sent Back", "Update", "Activity_Record", ActivityType.PO_Send_Back.ToString(), userid);
//            return Json(true);
//        }
//        public ActionResult WorkCompletion(long? Group_Id, string poNum)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res1 = db.Service_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.PO_Number == poNum).ToList();
//            //res.ForEach(x => x.ReceivedQuantity = db.SP_Get_ReceivedQuantity_AgainstPO(x.Id).FirstOrDefault());
//            var ids = new XElement("Deps", res1.Select(x => new XElement("Ids",
//             new XAttribute("Id", x.Dep_Id)
//             ))).ToString();
//            var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).ToList();
//            ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);

//            var res2 = db.Service_Completion.Where(x => x.Group_Id == Group_Id && x.PO_Number == poNum).ToList();

//            var res = new Serv_Comp { PO = res1, SC = res2 };

//            return View(res);
//        }
//        public JsonResult ServiceCompleteNo(Service_Completion[] items)
//        {
//            try
//            {
//                var group = items.Select(x => x.Group_Id).FirstOrDefault();
//                var po = items.Select(x => x.PO_Number).FirstOrDefault();

//                var uid = long.Parse(User.Identity.GetUserId());
//                var itemsData = new XElement("InventoryProducts", items.Select(x => new XElement("InventoryItem",
//                    new XAttribute("depId", x.Dep_Id),
//                    new XAttribute("depName", x.Dep_Name),
//                    (x.QualityCheck_By is null) ? null : new XAttribute("QualityCheck_By", x.QualityCheck_By),
//                    (x.QualityCheck_By_Name is null) ? null : new XAttribute("QualityCheck_By_Name", x.QualityCheck_By_Name),
//                    new XAttribute("groupId", x.Group_Id),
//                    (x.PO_Id is null) ? null : new XAttribute("poId", x.PO_Id),
//                    (x.PO_Number is null) ? null : new XAttribute("poNum", x.PO_Number),
//                    new XAttribute("qty", x.Qty),
//                    (x.Rate is null) ? null : new XAttribute("rate", x.Rate),
//                    (x.Remarks is null) ? null : new XAttribute("rema", x.Remarks),
//                    (x.Vendor_Id is null) ? null : new XAttribute("vendorId", x.Vendor_Id),
//                    (x.Vendor_Name is null) ? null : new XAttribute("vendorName", x.Vendor_Name)))).ToString();
//                var res = db.Sp_Add_Service_Complet(itemsData, uid).ToList();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Added Services items ", "Delete", "Activity_Record", ActivityType.Services.ToString(), userid);

//                //foreach (var g in items)
//                //{
//                //    g.Id = (long)res.Where(x => x.id == g.Id && x.vendor == g.Vendor_Id).Select(x => x.id).FirstOrDefault();
//                //}
              
//                if (res.Count() == items.Length)
//                {
//                    var mod = res.GroupBy(x => x.vendor).ToList();
//                    List<string> Grns = new List<string>();
//                    foreach (var vend in mod)
//                    {
//                        string gnrNo = "SC-" + db.Sp_Get_ReceiptNo("Service Complete").FirstOrDefault();
//                        foreach (var vendor in vend)
//                        {
//                            db.Sp_Update_Service_No(vendor.id, gnrNo);
//                        }
//                        Grns.Add(gnrNo);
//                    }
//                    var res1 = db.Service_PurchaseOrder.Where(x => x.Group_Id == group && x.PO_Number == po).FirstOrDefault();
//                    var prjheadcode = db.Projects.Where(x => x.Id == res1.Prj_Id).Select(x => x.Head_Code).FirstOrDefault();
//                    var prjhead = db.Sp_Get_CA_Head_Param_Code(prjheadcode).FirstOrDefault();
//                    //AccountHandlerController de = new AccountHandlerController();
//                    //var venhead = de.Vendor_Head(res1.Vendor_Name, res1.Vendor_Id);
//                    //decimal? balance = 0;
//                    //foreach (var v in items)
//                    //{
//                    //    balance += (v.Qty * v.Rate);
//                    //}
//                    //var TransactionId = new Helpers().RandomNumber();
//                    //int i = 0;
//                    ////Items Debit Entry
//                    //var GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
//                    //new XAttribute("Naration", "Payable Record Against Work Order No. " + po + " of Vendor " + items.FirstOrDefault().Vendor_Name + "for Project " + res1.Prj_Name),
//                    //new XAttribute("Head", prjhead.HeadCode + " - " + prjhead.Head),
//                    //new XAttribute("Head_Name", prjhead.Head),
//                    //new XAttribute("Head_Code", prjhead.HeadCode),
//                    //new XAttribute("Head_Id", prjhead.Id),
//                    //new XAttribute("Line", ++i),
//                    //new XAttribute("Qty", 0),
//                    //new XAttribute("UOM", ""),
//                    //new XAttribute("Rate", 0),
//                    //new XAttribute("Debit", balance),
//                    //new XAttribute("Credit", 0),
//                    //new XAttribute("GroupId", TransactionId)
//                    //));
//                    ////Vendor Credit Entry
//                    //var GEapp = new XElement("GeneralEntries", new XElement("Entries",
//                    //new XAttribute("Naration", "Payable Record Against Work Order No. " + po + " of Vendor " + items.FirstOrDefault().Vendor_Name + "for Project " + res1.Prj_Name),
//                    //new XAttribute("Head", venhead.Code + " - " + venhead.Head),
//                    //new XAttribute("Head_Name", venhead.Head),
//                    //new XAttribute("Head_Code", venhead.Code),
//                    //new XAttribute("Head_Id", venhead.Id),
//                    //new XAttribute("Line", ++i),
//                    //new XAttribute("Qty", 0),
//                    //new XAttribute("UOM", ""),
//                    //new XAttribute("Rate", 0),
//                    //new XAttribute("Debit", 0),
//                    //new XAttribute("Credit", balance),
//                    //new XAttribute("GroupId", TransactionId)
//                    //));
//                    //GeneralEnt.Add(
//                    //    from ge in GEapp.Elements()
//                    //    select ge
//                    //    );

//                    //var rere = db.Sp_Add_General_Entries(GeneralEnt.ToString(), uid).FirstOrDefault();
                    
//                    //string Bill = new XElement("Bills", new XElement("Entries",
//                    //new XAttribute("Vendor_Id", venhead.Id),
//                    //new XAttribute("Terms", "30 Days"),
//                    //new XAttribute("Bill_No", items.FirstOrDefault().PO_Number),
//                    //new XAttribute("Head_Code", prjhead.HeadCode),
//                    //new XAttribute("Head_Name", prjhead.Head),
//                    //new XAttribute("Qty", 1),
//                    //new XAttribute("Rate", balance),
//                    //new XAttribute("Line", 1),
//                    //new XAttribute("description", "Services Against Work Order No. " + items.FirstOrDefault().PO_Number),
//                    //new XAttribute("Head_Id", prjhead.Id),
//                    //new XAttribute("GroupId", TransactionId),
//                    //new XAttribute("Head", prjhead.HeadCode + " - " + prjhead.Head)
//                    //)).ToString();

//                    //db.Sp_Add_Bills(Bill, uid, DateTime.Now, DateTime.Now.AddDays(30));

//                    return Json(new { Status = true, Msg = "Added to inventory successfully", PO = po });
//                }
//                else
//                {
//                    var mod = res.GroupBy(x => x.vendor).ToList();
//                    List<string> Grns = new List<string>();
//                    foreach (var vend in mod)
//                    {
//                        string gnrNo = db.Sp_Get_ReceiptNo("Service Complete").FirstOrDefault();
//                        foreach (var vendor in vend)
//                        {
//                            db.Sp_Update_Service_No(vendor.id, gnrNo);
//                        }
//                        Grns.Add(gnrNo);
//                    }
//                    return Json(new { Status = false, Msg = "Failed to add to inventory", PO = po });
//                }
//            }
//            catch (Exception ex)
//            {
//                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new { Status = false, Msg = "Failed to add to inventory" });
//            }
//        }
//        public ActionResult CompletionCertification(long GroupId)
//        {
//            return View();
//        }
//        public ActionResult Revise_PO(string po)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Service_PurchaseOrder.Where(x => x.PO_Number == po).ToList();
//            return PartialView(res);
//        }
//        public JsonResult UpdatePO_Qty_Rate(long id, decimal val, string Type, string PO)
//        {
//            db.Sp_Update_Qty_Rate(id, val, Type, PO,"Services");
//            return Json(true);
//        }
//        public ActionResult OtherExpense(long? Id)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", Id).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            return View(res);
//        }
//        [HttpPost]
//        public JsonResult OtherExpenseSub(List<Other_Expense> demand, long prj)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var des = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();

//                var purItems = new XElement("Products", demand.Select(x => new XElement("ProductInfo",
//                    new XAttribute("itemName", x.Item_Name),
//                    (des.DepartmentId == null) ? null : new XAttribute("Dep_Id", des.DepartmentId),
//                    (des.DepartmentName == null) ? null : new XAttribute("Dep_Name", des.DepartmentName),
//                    new XAttribute("Grp", x.Group_Id),
//                    new XAttribute("Amount", x.Amount),
//                    new XAttribute("Currency", x.Currency),
//                    (x.Description == null) ? null : new XAttribute("Descr", x.Description),
//                    (des.DesignationName == null) ? null : new XAttribute("RequestedBy_Desgination", des.DesignationName),
//                    new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                var res = db.Sp_Add_OtherExpense(purItems, prj, uid).ToList();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Added Requisition And Sent For Approval ", "Create", "Activity_Record", ActivityType.Services.ToString(), prj);

//                if (res.Count == demand.Count)
//                {
//                    return Json(new { Status = true, Msg = "Requisition has been applied successfully and Send for approval." });
//                }
//                else
//                {
//                    return Json(new { Status = false, Msg = "Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                }
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Other Expense Requisition Error");
//                string msg = ex.Message;
//                return Json(new { Status = false, Msg = "Error in saving requisition." });
//            }
//        }

//        public JsonResult Update_Exp_Status(long Group_Id, string Status)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Update_OtherExp_Status(Group_Id, Status, userid);
           
//            db.Sp_Add_Activity(userid, "Updated EXP. Status ", "Update", "Activity_Record", ActivityType.Services.ToString(), Group_Id);

//            return Json(true);
//        }
//        public JsonResult DeleteOE(long GroupId)
//        {
//            var res = db.Sp_Delete_OtherExp(GroupId);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted EXP. Status ", "Delete", "Activity_Record", ActivityType.Services.ToString(), GroupId);

//            return Json(true);
//        }
//        public ActionResult ExpenseDetails(long GroupId)
//        {
//            var res = db.Other_Expense.Where(x => x.Group_Id == GroupId).ToList();
//            return View(res);
//        }


//        public ActionResult User_Service_Req()
//        {
//            return PartialView();
//        }

//        public ActionResult Dep_ServiceReq()
//        {
//            return PartialView();
//        }
//        public ActionResult Dep_ServiceReq_Search(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var res2 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//                string Ids = new XElement("Dep", res2.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();

//                var res = db.Sp_Get_Service_Req_List(From, To, Ids).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res2 = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long)x.Department_Id).ToList();
//                var ids = new XElement("Dep", res2.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();

//                var res = db.Sp_Get_Service_Req_List(From, To, ids).ToList();
//                return PartialView(res);
//            }
//        }

//        public ActionResult Dep_ServiceOrder()
//        {
//            return PartialView();
//        }
//        public ActionResult Dep_ServiceOrder_Search(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var res1 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//                string Ids = new XElement("Dep", res1.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();
//                var res2 = db.Sp_Get_Service_Order_List(From, To, Ids).ToList();

//                //var xml1 = new XElement("GroupId", res2.Select(x => new XElement("Id",
//                //            new XAttribute("Group", x.PO_Number)))).ToString();

//                //var res3 = db.Sp_Get_PO_Completed_GRN(From, To, xml1).ToList();

//                //var res = new PO_Completed_With_GRN { PO_list = res2, GRN = res3 };
//                return PartialView(res2);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res1 = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long)x.Department_Id).ToList();
//                var ids = new XElement("Dep", res1.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();
//                var res2 = db.Sp_Get_Service_Order_List(From, To, ids).ToList();

//                //var xml1 = new XElement("GroupId", res2.Select(x => new XElement("Id",
//                //     new XAttribute("Group", x.PO_Number)))).ToString();

//                //var res3 = db.Sp_Get_PO_Completed_GRN(From, To, xml1).ToList();
//                //var res = new PO_Completed_With_GRN { PO_list = res2, GRN = res3 };
//                return PartialView(res2);
//            }

//        }

//        //--------------------------------------------------------------- Service Contracts ------------------------------------------------------------------------------------------


//        public ActionResult ServicesContract()
//        {
//            return View();
//        }
//        public ActionResult NewServiceRequisition_SC(long? Id)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res1 = db.Projects.Where(x => x.Id == Id).ToList();
//            var res2 = db.Services_Purchase_Req.Where(x => x.Id == Id).ToList();

//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", Id).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            var res = new Project_And_Service_Purchase_Req { Project = res1, Service_PR = res2 };
//            return View(res);
//        }
//        public ActionResult NewService_Requisition_SC(long? Id)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", Id).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            return View(res);
//        }
//        public ActionResult NewService_Requisition_For_Other_SC(long? Id, long? Req)
//        {
//            long usserid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(usserid, "Accessed New PR Page for " + Req, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
//            var res1 = db.Sp_Get_EmployeeId(usserid).FirstOrDefault();
//            var res2 = db.Employee_Designations.Where(x => x.Emp_Id == res1).Select(x => (long)x.Department_Id).ToList();

//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", Id).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");

//            if (User.IsInRole("All Department") || User.IsInRole("Administrator"))
//            {
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == "Department"), "Id", "Name", Id);
//            }
//            else
//            {
//                var prjs = db.Prj_Attendees.Where(x => x.Userid == res1).Select(x => x.PrjId).Distinct().ToList();
//                ViewBag.Department = new SelectList(db.Projects.Where(x => prjs.Contains(x.Id)).ToList(), "Id", "Name", Id);

//            }

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            return View(res);
//        }
//        [HttpPost]
//        public JsonResult NewServiceRequisitionForOther_SC(List<Services_Purchase_Req> demand, long prj, int Department)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var des = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();

//                var purItems = new XElement("Products", demand.Select(x => new XElement("ProductInfo",
//                    new XAttribute("itemName", x.Item_Name),
//                   (x.Length == null) ? null : new XAttribute("Len", x.Length),
//                   (x.Len_UOM == null) ? null : new XAttribute("Len_UOM", x.Len_UOM),
//                    (x.Width == null) ? null : new XAttribute("Wid", x.Width),
//                    (x.Wid_UOM == null) ? null : new XAttribute("Wid_UOM", x.Wid_UOM),
//                    (x.Heigth == null) ? null : new XAttribute("Hei", x.Heigth),
//                    (x.Hei_UOM == null) ? null : new XAttribute("Hei_UOM", x.Hei_UOM),
//                    (x.Diameter == null) ? null : new XAttribute("Dia", x.Diameter),
//                    (x.Dia_UOM == null) ? null : new XAttribute("Dia_UOM", x.Dia_UOM),
//                    (x.Size == null) ? null : new XAttribute("Size", x.Size),
//                    (x.S_UOM == null) ? null : new XAttribute("S_UOM", x.S_UOM),
//                    (x.Qty == null) ? null : new XAttribute("Qty", x.Qty),
//                    (x.UOM == null) ? null : new XAttribute("UOM", x.UOM),
//                    (des.DepartmentId == null) ? null : new XAttribute("Dep_Id", des.DepartmentId),
//                    (des.DepartmentName == null) ? null : new XAttribute("Dep_Name", des.DepartmentName),
//                    new XAttribute("Grp", x.Group_Id),
//                    (x.Description == null) ? null : new XAttribute("Descr", x.Description),
//                    (des.DesignationName == null) ? null : new XAttribute("RequestedBy_Desgination", des.DesignationName),
//                    new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                var res = db.Sp_Add_ServiceRequisition(purItems, prj, uid).ToList();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Added New Service requisition ", "Create", "Activity_Record", ActivityType.Services.ToString(), userid);

//                if (res.Count == demand.Count)
//                {
//                    return Json(new { Status = true, Msg = "Service Requisition has been applied successfully and Send for approval." });
//                }
//                else
//                {
//                    return Json(new { Status = false, Msg = "Service Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                }
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Service Requisition Error");
//                string msg = ex.Message;
//                return Json(new { Status = false, Msg = "Error in saving Service requisition." });
//            }
//        }
//        [HttpPost]
//        public JsonResult NewServiceRequisition_SC(List<Services_Purchase_Req> demand, long prj)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var des = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();

//                var purItems = new XElement("Products", demand.Select(x => new XElement("ProductInfo",
//                    new XAttribute("itemName", x.Item_Name),
//                   (x.Length == null) ? null : new XAttribute("Len", x.Length),
//                   (x.Len_UOM == null) ? null : new XAttribute("Len_UOM", x.Len_UOM),
//                    (x.Width == null) ? null : new XAttribute("Wid", x.Width),
//                    (x.Wid_UOM == null) ? null : new XAttribute("Wid_UOM", x.Wid_UOM),
//                    (x.Heigth == null) ? null : new XAttribute("Hei", x.Heigth),
//                    (x.Hei_UOM == null) ? null : new XAttribute("Hei_UOM", x.Hei_UOM),
//                    (x.Breadth == null) ? null : new XAttribute("Breadth", x.Breadth),
//                    (x.B_UOM == null) ? null : new XAttribute("B_UOM", x.B_UOM),
//                    (x.Diameter == null) ? null : new XAttribute("Dia", x.Diameter),
//                    (x.Dia_UOM == null) ? null : new XAttribute("Dia_UOM", x.Dia_UOM),
//                    (x.Size == null) ? null : new XAttribute("Size", x.Size),
//                    (x.S_UOM == null) ? null : new XAttribute("S_UOM", x.S_UOM),
//                    (x.Qty == null) ? null : new XAttribute("Qty", x.Qty),
//                    (x.NO == null) ? null : new XAttribute("NO", x.NO),
//                    (x.UOM == null) ? null : new XAttribute("UOM", x.UOM),
//                    (x.Milestone_Id == null) ? null : new XAttribute("Milestone_Id", x.Milestone_Id),
//                    (x.Milestone_Name == null) ? null : new XAttribute("Milestone_Name", x.Milestone_Name),
//                    (des.DepartmentId == null) ? null : new XAttribute("Dep_Id", des.DepartmentId),
//                    (des.DepartmentName == null) ? null : new XAttribute("Dep_Name", des.DepartmentName),
//                    new XAttribute("Grp", x.Group_Id),
//                    (x.Description == null) ? null : new XAttribute("Descr", x.Description),
//                    (des.DesignationName == null) ? null : new XAttribute("RequestedBy_Desgination", des.DesignationName),
//                    new XAttribute("Develop", true),
//                    new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                var res = db.Sp_Add_ServiceRequisition(purItems, prj, uid).ToList();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Added New Service requisition ", "Create", "Activity_Record", ActivityType.Services.ToString(), userid);

//                if (res.Count == demand.Count)
//                {
//                    return Json(new { Status = true, Msg = "Service Requisition has been applied successfully and Send for approval." });
//                }
//                else
//                {
//                    return Json(new { Status = false, Msg = "Service Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                }
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Service Requisition Error");
//                string msg = ex.Message;
//                return Json(new { Status = false, Msg = "Error in saving Service requisition." });
//            }
//        }
//        public ActionResult NewQuotations_SC()
//        {
//            return PartialView();
//        }
//        public ActionResult PendingApproval_SC()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var res = db.Sp_Get_Service_In_Userid_SC(userid, null, null, null, PurchaseRequisitionStatus.Pending_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var res = db.Sp_Get_Service_In_Userid_SC(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//        }
//        public ActionResult Pending_PR_SC()
//        {
//            var res = db.Services_Purchase_Req.Where(x => x.Status == PurchaseRequisitionStatus.Pending.ToString() && x.Development == true ).ToList();
//            return PartialView(res);
//        }
//        public ActionResult QuotationFinalize_SC()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator") || User.IsInRole("All Projects"))
//            {
//                var res = db.Sp_Get_Service_In_Userid_SC(userid, null, null, null, PurchaseRequisitionStatus.Quotation_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var res = db.Sp_Get_Service_In_Userid_SC(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Quotation_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//        }
//        public ActionResult Gen_Po_SC()
//        {
//            var res = db.Service_PurchaseOrder.Where(x => x.Completed == false ).ToList();
//            return PartialView(res);
//        }
//        public JsonResult UpdatePurchaseReq_SC(long Group_Id, string Status)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Update_Ser_Req_Status(Group_Id, Status, userid);

//            db.Sp_Add_Activity(userid, "Update Purchase requisition ", "Update", "Activity_Record", ActivityType.Services.ToString(), userid);

//            return Json(true);
//        }
//        public JsonResult GetSerCount_SC()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var res1 = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res2 = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res4 = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Quotation_Approval.ToString()).GroupBy(x => x.Group_Id).Count();
//                var res6 = db.Sp_Get_Service_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id);
//                var res7 = db.Service_PurchaseOrder.Where(x => res6.Contains(x.Group_Id) && x.Completed == false).GroupBy(x => x.Group_Id).Count();

//                var res = new { PendingApproval = res1, Pending = res2, QuotationFinal = res4, PurchaseOrder = res7 };
//                return Json(res);
//            }
//            else
//            {
//                var res1 = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending_Approval.ToString()).GroupBy(x => x.Group_Id).Count();

//                var res2 = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending.ToString()).GroupBy(x => x.Group_Id).Count();

//                var res4 = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Quotation_Approval.ToString()).GroupBy(x => x.Group_Id).Count();

//                var res6 = db.Sp_Get_Service_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id);
//                var res7 = db.Service_PurchaseOrder.Where(x => res6.Contains(x.Group_Id) && x.Completed == false).GroupBy(x => x.Group_Id).Count();

//                var res = new { PendingApproval = res1, Pending = res2, QuotationFinal = res4, PurchaseOrder = res7 };
//                return Json(res);
//            }
//        }
//        public JsonResult DeleteServiceReq_SC(long GroupId)
//        {
//            var res = db.Sp_Delete_ServiceReq(GroupId);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted Service Requisition ", "Delete", "Activity_Record", ActivityType.Services.ToString(), GroupId);

//            return Json(true);
//        }
//        public ActionResult BidsListing_SC(long Group)
//        {
//            var PR = db.Services_Purchase_Req.Where(x => x.Group_Id == Group).ToList();
//            ViewBag.PurchaseReqId = PR;
//            var res = db.Sp_GET_Applied_Quotes_For_SR_Group(Group).ToList();
//            return View(new QuotationServicesViewModel { AppliedBids = res, PurchaseRequistionDetails = PR });
//        }
//        public JsonResult PO_Terms_SC(long Id, long Group_Id)
//        {
//            var res1 = db.Inventory_PO_Terms.Where(x => x.Req_Id == Group_Id).ToList();
//            var res2 = db.Service_Bidding.Where(x => x.Id == Id).FirstOrDefault();

//            var res = new { Terms = res1, Image = res2 };
//            return Json(res);
//        }
//        public ActionResult AddNewQuotation_SC(long? GroupId, long? Req_Id, string Item)
//        {
//            var res = db.Services_Purchase_Req.Where(x => x.Group_Id == GroupId).ToList();
//            ViewBag.Req_Id = Req_Id;
//            ViewBag.GroupId = GroupId;
//            ViewBag.Item = Item;
//            return PartialView(res);
//        }
//        public JsonResult AddQuotation_SC(List<Service_Bidding> items, long Vendor_Id, long Group_Id, string Quote_Ref, string Currency, string Description, DateTime? DeliveryDate)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var Ven = db.Vendors.Where(x => x.Id == Vendor_Id).FirstOrDefault();

//            List<Service_Bidding> sbl = new List<Service_Bidding>();
//            foreach (var item in items)
//            {
//                var VenExists = db.Service_Bidding.Where(x => x.Vendor_Id == Vendor_Id && x.Group_Id == Group_Id && x.Item_Name == item.Item_Name).Any();
//                if (VenExists)
//                {
//                    var ret = new { Status = false, Msg = "Vendor is Already Added" };
//                    return Json(ret);
//                }
//            }

//            var itemsData = new XElement("Biddings", items.Select(x => new XElement("BidData",
//                        new XAttribute("itemName", x.Item_Name),
//                        new XAttribute("Len", (x.Length == null) ? 0 : x.Length),
//                        new XAttribute("Wid", (x.Width == null) ? 0 : x.Width),
//                        new XAttribute("Hei", (x.Heigth == null) ? 0 : x.Heigth),
//                        new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
//                        (x.UOM == null) ? null : new XAttribute("Uom", x.UOM),
//                        new XAttribute("GroupId", Group_Id),
//                        new XAttribute("VendorId", Ven.Id),
//                        new XAttribute("VendorName", Ven.Company),
//                        (x.Dep_Id == null) ? null : new XAttribute("Dep_Id", x.Dep_Id),
//                        (x.Dep_Name == null) ? null : new XAttribute("Dep_Name", x.Dep_Name),
//                        (x.Description == null) ? null : new XAttribute("descr", Description),
//                        new XAttribute("ServiceReq_Id", x.ServiceReq_Id),
//                        new XAttribute("prate", x.PurchaseRate),
//                        (x.Tax == null) ? null : new XAttribute("Tax", x.Tax),
//                        (x.Currency == null) ? null : new XAttribute("Currency", x.Currency),
//                        new XAttribute("Breadth", (x.Breadth is null) ? 0 : x.Breadth),
//                        (x.Milestone_Id == null) ? null : new XAttribute("Milestone_Id", x.Milestone_Id),
//                        (x.Milestone_Name == null) ? null : new XAttribute("Milestone_Name", x.Milestone_Name)))).ToString();

//            var res1 = db.Sp_Add_Ser_Bids(itemsData, userid, null).FirstOrDefault();
//            db.Sp_Add_Activity(userid, "Added New Qutation " + Quote_Ref, "Create", "Activity_Record", ActivityType.Services.ToString(), Group_Id);
//            var res = new { Status = true, Msg = "Successfully Saved", Id = res1.Id };
//            return Json(res);
//        }
//        public ActionResult UpdateQuote_SC(long Id)
//        {
//            var res = db.Service_Bidding.Where(x => x.Id == Id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public JsonResult UpdateQuote1_SC(long bid, decimal quantity, decimal prate)
//        {
//            var res1 = db.Sp_Update_Ser_Bids(quantity, prate, bid);
//            return Json(res1);
//        }

//        public JsonResult DeleteBid_SC(long bid)
//        {
//            try
//            {
//                db.Sp_Delete_Bid(bid, "Services");
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Deleted Bid ", "Delete", "Activity_Record", ActivityType.Services.ToString(), bid);

//                return Json(true);
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Remove Quotation Error");
//                return Json(false);
//            }
//        }
//        public JsonResult GeneratePO_SC(long Group_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var res = db.Service_Bidding.Where(x => x.Group_Id == Group_Id && x.IsFinalized == true && x.PO_Generated == null).ToList();
//            var req = db.Services_Purchase_Req.Where(x => x.Group_Id == Group_Id).FirstOrDefault();
//            //List<string> PoList = new List<string>();
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    foreach (var item in res.GroupBy(x => x.Vendor_Id))
//                    {
//                        var itemsData = new XElement("Biddings", item.Select(x => new XElement("BidData",
//                           new XAttribute("itemName", x.Item_Name),
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
//                           (x.Qty is null) ? null : new XAttribute("Qty", x.Qty),
//                           (x.Dep_Id is null) ? null : new XAttribute("Dep_Id", x.Dep_Id),
//                           (x.Dep_Name is null) ? null : new XAttribute("Dep_Name", x.Dep_Name),
//                           //(res2.DesignationName is null) ? null : new XAttribute("CreatedBy_Designation", res2.DesignationName),
//                           (req.Prj_Id is null) ? null : new XAttribute("Prj_Id", req.Prj_Id),
//                           (req.Prj_Name is null) ? null : new XAttribute("Prj_Name", req.Prj_Name),
//                           (req.Prj_Offsite is null) ? null : new XAttribute("Prj_Offsite", req.Prj_Offsite),
//                           (req.ApprovedBy_Name is null) ? null : new XAttribute("ApprovedBy_Name", req.ApprovedBy_Name),
//                           (req.ApprovedBy_Designation is null) ? null : new XAttribute("ApprovedBy_Designation", req.ApprovedBy_Designation),
//                           //new XAttribute("Qty", x.Qty),
//                           new XAttribute("GroupId", x.Group_Id),
//                           new XAttribute("VendorId", x.Vendor_Id),
//                           new XAttribute("VendorName", x.Vendor_Name),
//                           new XAttribute("bid", x.Id),
//                           new XAttribute("prate", x.PurchaseRate)))).ToString();
//                        var res3 = db.Sp_Add_PurchaseOrders_Ser(itemsData, userid, PurchaseRequisitionStatus.Purchase_Order_NotPrinted.ToString()).ToList();

//                        db.Sp_Add_Activity(userid, "Created New PO ", "Create", "Activity_Record", ActivityType.Services.ToString(), Group_Id);

//                        var vendor = db.Vendors.Where(x => x.Id == item.Key).FirstOrDefault();
//                        var res4 = db.Sp_Get_CA_Head_MultiRef("Liability", "Trade Payables").FirstOrDefault();

//                        if (!db.Sp_Get_CA_Head_MultiRef("Liability", vendor.Company).Any())
//                        {
//                            var res6 = db.Sp_Add_CA_Head(res4.Id, vendor.Company, "Vendor", vendor.Id,COA_Nature.Liability.ToString());
//                        }

//                        //PoList.AddRange(res3.Select(x => x.ponum));
//                    }
//                    Transaction.Commit();
//                    //var ret = new { Status = true, Msg = "Purchase Order Generated", PoNum = PoList };
//                    var ret = new { Status = true, Msg = "Purchase Requisition is Finalized" };
//                    return Json(ret);
//                }
//                catch (Exception ex)
//                {
//                    EmailService e = new EmailService();
//                    e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Purchase Order Creation Error");
//                    Transaction.Rollback();
//                    var ret = new { Status = false, Msg = "Error Occured" };
//                    return Json(ret);
//                }
//            }
//        }
//        public JsonResult PrintPO_SC(long Group_Id, string PO_Num)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var res2 = db.Sp_Get_EmployeeFirstDesignation(res1).FirstOrDefault();

//            db.Sp_Update_Ser_PO_PrintBy(userid, res2.DesignationName, Group_Id, null, PO_Num);
//            var res = db.Service_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.PO_Number == PO_Num).Select(x => x.PO_Number).Distinct().ToList();
//            return Json(res);
//        }
//        public JsonResult SaveTerm_SC(string Term, DateTime? Date, long Bid_Id, long Vendor)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = Convert.ToBoolean(db.Sp_Add_POTerms(Term, Date, userid, Bid_Id, Vendor).FirstOrDefault());

//            db.Sp_Add_Activity(userid, "Updated Term for Vendor ", "Update", "Activity_Record", ActivityType.Services.ToString(), Vendor);

//            if (res)
//            {
//                var ret = new { Status = true, Msg = "Successfully Added" };
//                return Json(ret);
//            }
//            else
//            {
//                var ret = new { Status = false, Msg = "Already Exists" };
//                return Json(ret);
//            }
//        }
//        public ActionResult QuotationFinalization_SC(long Group_Id)
//        {
//            var PR = db.Services_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            ViewBag.SerReqId = PR;
//            var res = db.Sp_GET_Applied_Quotes_For_SR_Group(Group_Id).ToList();
//            return View(new QuotationServicesViewModel { AppliedBids = res, PurchaseRequistionDetails = PR });
//        }
//        public JsonResult MarkQuotation_SC(int Id, bool Status, string Remarks)
//        {
//            var res = db.Sp_Update_MarkQuotation_Ser(Id, Status, Remarks);
//            return Json(true);
//        }
//        public ActionResult PrintPurchaseOrder_SC(string poNum)
//        {
//            try
//            {
//                var poData = db.Service_PurchaseOrder.Where(x => x.PO_Number == poNum).ToList();
//                var venId = poData.Select(y => new { y.Vendor_Id, y.Group_Id }).FirstOrDefault();
//                var res = db.Vendors.Where(x => x.Id == venId.Vendor_Id).FirstOrDefault();
//                var terms = db.Inventory_PO_Terms.Where(x => x.Req_Id == venId.Group_Id).ToList();
//                var uid = User.Identity.GetUserId<long>();
//                ViewBag.unm = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
//                return View(new PurchaseOrderPrintViewSer { PO_Data = poData, Vendor_Data = res, PO_Terms = terms });

//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "SaveIssueRequest Demand Order Error");
//                return View();
//            }
//            //var res = db.Sp_Get_PurchaseOrder(Token, Id).ToList();
//            //return View(res);
//        }
//        public JsonResult UpdateCompPurchaseCompRep_SC(List<string> Group)
//        {
//            var str = new XElement("Group", Group.Select(x => new XElement("Id",
//                        new XAttribute("GroupId", x)))).ToString();
//            var res1 = db.Sp_Update_POSummary_Ser(str);
//            return Json(string.Format("{0:MM/dd/yyyy}", DateTime.Now));
//        }
//        public ActionResult CompPurchaseCompRep_SC(DateTime date)
//        {
//            var res = db.Sp_Get_PurchaseOrder_Ser_Date(date).ToList();
//            return View(res);
//        }

//        public ActionResult WorkCompletion_SC(long? Group_Id, string poNum)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res1 = db.Service_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.PO_Number == poNum).ToList();
//            //res.ForEach(x => x.ReceivedQuantity = db.SP_Get_ReceivedQuantity_AgainstPO(x.Id).FirstOrDefault());
//            var ids = new XElement("Deps", res1.Select(x => new XElement("Ids",
//             new XAttribute("Id", x.Dep_Id)
//             ))).ToString();
//            var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).ToList();
//            ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);

//            var res2 = db.Service_Completion.Where(x => x.Group_Id == Group_Id && x.PO_Number == poNum).ToList();

//            var res = new Serv_Comp { PO = res1, SC = res2 };

//            return View(res);
//        }
//        public JsonResult ServiceCompleteNo_SC(Service_Completion[] items,decimal Amount, string Remarks)
//        {
//            try
//            {
//                var group = items.Select(x => x.Group_Id).FirstOrDefault();
//                var po = items.Select(x => x.PO_Number).FirstOrDefault();

//                var uid = long.Parse(User.Identity.GetUserId());
//                AccountHandlerController ac = new AccountHandlerController();
//                var comp = ac.Company_Attr(uid);

//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var itemsData = new XElement("InventoryProducts", items.Select(x => new XElement("InventoryItem",
//                        new XAttribute("depId", x.Dep_Id),
//                        new XAttribute("depName", x.Dep_Name),
//                        new XAttribute("groupId", x.Group_Id),
//                        (x.QualityCheck_By is null) ? null : new XAttribute("QualityCheck_By", x.QualityCheck_By),
//                        (x.QualityCheck_By_Name is null) ? null : new XAttribute("QualityCheck_By_Name", x.QualityCheck_By_Name),
//                        (x.PO_Id is null) ? null : new XAttribute("poId", x.PO_Id),
//                        (x.PO_Number is null) ? null : new XAttribute("poNum", x.PO_Number),
//                        (x.Qty is null) ? null : new XAttribute("qty", x.Qty),
//                        (x.Length is null) ? null : new XAttribute("Length", x.Length),
//                        (x.Width is null) ? null : new XAttribute("Width", x.Width),
//                        (x.Heigth is null) ? null : new XAttribute("Heigth", x.Heigth),
//                        (x.Breadth is null) ? null : new XAttribute("Breadth", x.Breadth),
//                        (x.Rate is null) ? null : new XAttribute("rate", x.Rate),
//                        (x.Remarks is null) ? null : new XAttribute("rema", x.Remarks),
//                        (x.Vendor_Id is null) ? null : new XAttribute("vendorId", x.Vendor_Id),
//                        (x.Vendor_Name is null) ? null : new XAttribute("vendorName", x.Vendor_Name)))).ToString();
//                        var res = db.Sp_Add_Service_Complet(itemsData, uid).ToList();
//                        long userid = long.Parse(User.Identity.GetUserId());
//                        var temp2 = db.Sp_Add_Activity(userid, "Added Services items ", "Delete", "Activity_Record", ActivityType.Services.ToString(), userid);

//                        //foreach (var g in items)
//                        //{
//                        //    g.Id = (long)res.Where(x => x.id == g.Id && x.vendor == g.Vendor_Id).Select(x => x.id).FirstOrDefault();
//                        //}

//                        if (res.Count() == items.Length)
//                        {
//                            var mod = res.GroupBy(x => x.vendor).ToList();
//                            List<string> Grns = new List<string>();
//                            foreach (var vend in mod)
//                            {
//                                string gnrNo = "SC-" + db.Sp_Get_ReceiptNo("Service Complete").FirstOrDefault();
//                                foreach (var vendor in vend)
//                                {
//                                    var temp1 = db.Sp_Update_Service_No(vendor.id, gnrNo);
//                                }
//                                Grns.Add(gnrNo);
//                            }
//                            var res1 = db.Service_PurchaseOrder.Where(x => x.Group_Id == group && x.PO_Number == po).FirstOrDefault();
//                            AccountHandlerController de = new AccountHandlerController();

//                            var prjhead = de.HeadFinder(COA_Mapper_Modules.Projects.ToString(), COA_Mapper_ModuleTypes.Projects_List.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.Prj_Id);
//                            var venhead = de.Vendor_Head("", res1.Vendor_Id, comp.Id);
//                            decimal? balance = 0;
//                            foreach (var v in items)
//                            {
//                                balance += (v.Qty * v.Rate);
//                            }
//                            var TransactionId = new Helpers().RandomNumber();
//                            int i = 0;
//                            //Items Debit Entry

//                            string narration = "Payable record against services of " + Remarks + " against Workorder " + res1.PO_Number + " for project " + res1.Prj_Name; 

//                            var GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
//                            new XAttribute("Naration", narration),
//                            new XAttribute("Head", prjhead.Text_ChartCode + " - " + prjhead.Head),
//                            new XAttribute("Head_Name", prjhead.Head),
//                            new XAttribute("Head_Code", prjhead.Text_ChartCode),
//                            new XAttribute("Head_Id", prjhead.Id),
//                            new XAttribute("Line", ++i),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", Amount),
//                            new XAttribute("Credit", 0),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("GroupId", TransactionId)
//                            ));
//                            //Vendor Credit Entry
//                            var GEapp = new XElement("GeneralEntries", new XElement("Entries",
//                            new XAttribute("Naration", narration),
//                            new XAttribute("Head", venhead.Text_ChartCode + " - " + venhead.Head),
//                            new XAttribute("Head_Name", venhead.Head),
//                            new XAttribute("Head_Code", venhead.Text_ChartCode),
//                            new XAttribute("Head_Id", venhead.Id),
//                            new XAttribute("Line", ++i),
//                            new XAttribute("Qty", 0),
//                            new XAttribute("UOM", ""),
//                            new XAttribute("Rate", 0),
//                            new XAttribute("Debit", 0),
//                            new XAttribute("Credit", Amount),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("GroupId", TransactionId)
//                            ));
//                            GeneralEnt.Add(
//                                from ge in GEapp.Elements()
//                                select ge
//                                );

//                            var rere = db.Sp_Add_General_Entries(GeneralEnt.ToString(), uid).FirstOrDefault();

//                            string Bill = new XElement("Bills", new XElement("Entries",
//                            new XAttribute("Vendor_Id", venhead.Id),
//                            new XAttribute("Terms", "30 Days"),
//                            new XAttribute("Bill_No", items.FirstOrDefault().PO_Number),
//                            new XAttribute("Head_Code", prjhead.Text_ChartCode),
//                            new XAttribute("Head_Name", prjhead.Head),
//                            new XAttribute("Qty", 1),
//                            new XAttribute("Rate", Amount),
//                            new XAttribute("Line", 1),
//                            new XAttribute("description", narration),
//                            new XAttribute("Head_Id", prjhead.Id),
//                            new XAttribute("GroupId", TransactionId),
//                       new XAttribute("PO_Group_Id", items.FirstOrDefault().Group_Id),
//                            new XAttribute("Comp_Id", comp.Id),
//                            new XAttribute("Head", prjhead.Text_ChartCode + " - " + prjhead.Head)
//                            )).ToString();

//                            var temp  =db.Sp_Add_Bills(Bill, uid, DateTime.Now, DateTime.Now.AddDays(30)).FirstOrDefault();
//                            Transaction.Commit();
//                            return Json(new { Status = true, Msg = "Added to inventory successfully", PO = po });
//                        }
//                        else
//                        {
//                            var mod = res.GroupBy(x => x.vendor).ToList();
//                            List<string> Grns = new List<string>();
//                            foreach (var vend in mod)
//                            {
//                                string gnrNo = db.Sp_Get_ReceiptNo("Service Complete").FirstOrDefault();
//                                foreach (var vendor in vend)
//                                {
//                                    db.Sp_Update_Service_No(vendor.id, gnrNo);
//                                }
//                                Grns.Add(gnrNo);
//                            }
//                            Transaction.Rollback();
//                            return Json(new { Status = false, Msg = "Failed to add to inventory", PO = po });
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        return Json(new { Status = false, Msg = "Failed to add to inventory" });
//                    }
//                }  
//            }
//            catch (Exception ex)
//            {
//                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new { Status = false, Msg = "Failed to add to inventory" });
//            }
//        }
//        public ActionResult CompletionCertification_SC(long GroupId)
//        {
//            return View();
//        }
//        public ActionResult Revise_PO_SC(string po)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Service_PurchaseOrder.Where(x => x.PO_Number == po).ToList();
//            return PartialView(res);
//        }
//        public JsonResult UpdatePO_Qty_Rate_SC(long id, decimal val, string Type, string PO)
//        {
//            db.Sp_Update_Qty_Rate(id, val, Type, PO, "Services");
//            return Json(true);
//        }
//        public ActionResult OtherExpense_SC(long? Id)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", Id).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            return View(res);
//        }
//        [HttpPost]
//        public JsonResult OtherExpenseSub_SC(List<Other_Expense> demand, long prj)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var des = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();

//                var purItems = new XElement("Products", demand.Select(x => new XElement("ProductInfo",
//                    new XAttribute("itemName", x.Item_Name),
//                    (des.DepartmentId == null) ? null : new XAttribute("Dep_Id", des.DepartmentId),
//                    (des.DepartmentName == null) ? null : new XAttribute("Dep_Name", des.DepartmentName),
//                    new XAttribute("Grp", x.Group_Id),
//                    new XAttribute("Amount", x.Amount),
//                    new XAttribute("Currency", x.Currency),
//                    (x.Description == null) ? null : new XAttribute("Descr", x.Description),
//                    (des.DesignationName == null) ? null : new XAttribute("RequestedBy_Desgination", des.DesignationName),
//                    new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                var res = db.Sp_Add_OtherExpense(purItems, prj, uid).ToList();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Added Requisition And Sent For Approval ", "Create", "Activity_Record", ActivityType.Services.ToString(), prj);

//                if (res.Count == demand.Count)
//                {
//                    return Json(new { Status = true, Msg = "Requisition has been applied successfully and Send for approval." });
//                }
//                else
//                {
//                    return Json(new { Status = false, Msg = "Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                }
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Other Expense Requisition Error");
//                string msg = ex.Message;
//                return Json(new { Status = false, Msg = "Error in saving requisition." });
//            }
//        }

//        public JsonResult Update_Exp_Status_SC(long Group_Id, string Status)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Update_OtherExp_Status(Group_Id, Status, userid);

//            db.Sp_Add_Activity(userid, "Updated EXP. Status ", "Update", "Activity_Record", ActivityType.Services.ToString(), Group_Id);

//            return Json(true);
//        }
//        public JsonResult DeleteOE_SC(long GroupId)
//        {
//            var res = db.Sp_Delete_OtherExp(GroupId);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted EXP. Status ", "Delete", "Activity_Record", ActivityType.Services.ToString(), GroupId);

//            return Json(true);
//        }
//        public ActionResult ExpenseDetails_SC(long GroupId)
//        {
//            var res = db.Other_Expense.Where(x => x.Group_Id == GroupId).ToList();
//            return View(res);
//        }


//        public ActionResult User_Service_Req_SC()
//        {
//            return PartialView();
//        }

//        public ActionResult Dep_ServiceReq_SC()
//        {
//            return PartialView();
//        }
//        public ActionResult Dep_ServiceReq_Search_SC(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var res2 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//                string Ids = new XElement("Dep", res2.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();

//                var res = db.Sp_Get_Service_Req_List(From, To, Ids).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res2 = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long)x.Department_Id).ToList();
//                var ids = new XElement("Dep", res2.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();

//                var res = db.Sp_Get_Service_Req_List(From, To, ids).ToList();
//                return PartialView(res);
//            }
//        }

//        public ActionResult Dep_ServiceOrder_SC()
//        {
//            return PartialView();
//        }
//        public ActionResult Dep_ServiceOrder_Search_SC(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var res1 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//                string Ids = new XElement("Dep", res1.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();
//                var res2 = db.Sp_Get_Service_Order_List(From, To, Ids).ToList();

//                //var xml1 = new XElement("GroupId", res2.Select(x => new XElement("Id",
//                //            new XAttribute("Group", x.PO_Number)))).ToString();

//                //var res3 = db.Sp_Get_PO_Completed_GRN(From, To, xml1).ToList();

//                //var res = new PO_Completed_With_GRN { PO_list = res2, GRN = res3 };
//                return PartialView(res2);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res1 = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long)x.Department_Id).ToList();
//                var ids = new XElement("Dep", res1.Select(x => new XElement("DepId", new XAttribute("Id", x)))).ToString();
//                var res2 = db.Sp_Get_Service_Order_List(From, To, ids).ToList();

//                //var xml1 = new XElement("GroupId", res2.Select(x => new XElement("Id",
//                //     new XAttribute("Group", x.PO_Number)))).ToString();

//                //var res3 = db.Sp_Get_PO_Completed_GRN(From, To, xml1).ToList();
//                //var res = new PO_Completed_With_GRN { PO_list = res2, GRN = res3 };
//                return PartialView(res2);
//            }

//        }


//    }
//}