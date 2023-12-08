//using MeherEstateDevelopers.Filters;
//using MeherEstateDevelopers.Models;
//using Microsoft.AspNet.Identity;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web.Mvc;
//using System.Web.Security;
//using System.Xml.Linq;
//using Newtonsoft.Json;
//using System.Threading;
//using System.Web;

//namespace MeherEstateDevelopers.Controllers
//{
//    [Authorize]
//    [LogAction]
//    public class InventoryController : Controller
//    {
//        private Grand_CityEntities db = new Grand_CityEntities();
//        // GET: Inventory
//        public ActionResult Index()
//        {
//            return View();
//        }
//        // Store Manager Dashboard
//        public ActionResult StoreManagerDashboard()
//        {
//            //List<Project_details_inventory> result = (
//            // from z in db.Projects
//            // join y in db.Prj_Costing on z.Id equals y.PrjId
//            // select new Project_details_inventory
//            // {
//            //     Id = z.Id,
//            //     Name = z.Name,
//            //     Nameofgood = y.Name,
//            //     Quantity = y.Qty,
//            //     MeasurementUnit = y.UOM,
//            // }).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Manager Dashboard Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View();
//        }
//        public ActionResult InventoryItems()
//        {
//            var res = db.Sp_Get_Inventory().ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Inventory Items Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View(res);
//        }
//        public ActionResult GRNList()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Receive Items")).ToList();
//            ViewBag.Users = new SelectList(All, "id", "Name");

//            db.Sp_Add_Activity(userid, "Accessed GRN List Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//            return View();
//        }
//        public ActionResult GRN_Search(DateTime? From, DateTime? To, long?[] Users)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            string use = "";
//            if (Users != null)
//            {
//                use = new XElement("Users", Users.Select(x => new XElement("User_Id",
//                (x == null) ? null : new XAttribute("Ids", x)
//                                 ))).ToString();
//            }

//            var res = db.Sp_Get_GRN_List(From, To, use).ToList();
//            db.Sp_Add_Activity(userid, "Accessed GRN Page from " + From + " To" + To, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView(res);
//        }
//        public ActionResult DemandOrderReq()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Demand Order  Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }
//        public ActionResult DemandOrderReqSea(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Sp_Get_Stock_Out_Userid(userid, From, To).ToList();

//            db.Sp_Add_Activity(userid, "Searched Demand Order From " + From + " To " + To, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView(res);
//        }
//        public ActionResult Get_Completed_PO(string PO)
//        {
//            long userid = User.Identity.GetUserId<long>();

//            var xml1 = new XElement("GroupId", new XElement("Id",
//                 new XAttribute("Group", PO))).ToString();
//            var res3 = db.Sp_Get_PO_Completed(null, null, xml1, false).ToList();
//            var xml2 = new XElement("GroupId", res3.Select(x => new XElement("Id",
//                 new XAttribute("Group", x.Group_Id)))).ToString();

//            var res4 = db.Sp_Get_PO_Completed_GRN(null, null, xml2).ToList();

//            var res = new PO_Completed_With_GRN { PO = res3, GRN = res4 };

//            db.Sp_Add_Activity(userid, "Accessed Completed PO Page " + PO, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView("Completed_PO", res);
//        }
//        public ActionResult Completed_PO(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id).ToList();
//            var res2 = db.Inventory_PurchaseOrder.Where(x => res1.Contains(x.Group_Id) && x.OtherCharges == null).Select(x => x.PO_Number).ToList();
//            //Requisition No.
//            var PR = db.Inventory_Purchase_Req.Where(x => res1.Contains(x.Group_Id)).Select(x => new DeptsModel { deptId = (long)x.Group_Id, deptName = x.Requisition_No }).ToList();


//            var xml1 = new XElement("GroupId", res2.Select(x => new XElement("Id",
//                 new XAttribute("Group", x)))).ToString();

//            var res3 = db.Sp_Get_PO_Completed(From, To, xml1, true).ToList();
//            var xml2 = new XElement("GroupId", res3.Select(x => new XElement("Id",
//                 new XAttribute("Group", x.Group_Id)))).ToString();

//            var res4 = db.Sp_Get_PO_Completed_GRN(From, To, xml2).ToList();

//            db.Sp_Add_Activity(userid, "Accessed Completed PO Page From " + From + " To " + To, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            var res = new PO_Completed_With_GRN { PO = res3, GRN = res4, REQ = PR };
//            return PartialView(res);
//        }
//        public ActionResult PendingApproval()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator") || User.IsInRole("All Projects"))
//            {
//                var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending_Approval.ToString()).ToList();
//                res1.ForEach(x => x.PurReq_DemReq = true);


//                var res2 = db.Inventory_Demand_Req.Where(x => x.ApprovedBy == null).ToList();
//                var res3 = res2.Select(x => new Sp_Get_Stock_In_Userid_Result
//                {
//                    Id = x.Id,
//                    Item_Name = x.Item_Name,
//                    Qty = x.Qty,
//                    Description = x.Details,
//                    ReqTo_Dep = x.ReqTo_Dep_Name,
//                    Requested_Date = x.ReqBy_Date.Value.Date,
//                    Group_Id = x.Group_Id,
//                    Dep_Name = x.Dep_Name
//                }).ToList();

//                res3.ForEach(x => x.PurReq_DemReq = false);

//                res1.AddRange(res3);
//                return PartialView(res1);
//            }
//            else
//            {
//                var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending_Approval.ToString()).ToList();
//                res1.ForEach(x => x.PurReq_DemReq = true);

//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Users = db.Sp_Get_Reporting_Employees(EmpId).Select(x => x.loginid).ToList();
//                var res2 = db.Inventory_Demand_Req.Where(x => Users.Contains(x.ReqBy) && x.ApprovedBy == null).ToList();

//                var res3 = res2.Select(x => new Sp_Get_Stock_In_Userid_Result
//                {
//                    Id = x.Id,
//                    Item_Name = x.Item_Name,
//                    Qty = x.Qty,
//                    Description = x.Details,
//                    ReqTo_Dep = x.ReqTo_Dep_Name,
//                    Requested_Date = Convert.ToDateTime(x.ReqBy_Date).Date,
//                    Group_Id = x.Group_Id,
//                    Dep_Name = x.Dep_Name
//                }).ToList();

//                res3.ForEach(x => x.PurReq_DemReq = false);

//                res1.AddRange(res3);
//                return PartialView(res1);
//            }
//        }
//        public ActionResult Pending()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator") || User.IsInRole("All Projects"))
//            {
//                var res = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Pending.ToString()).ToList();

//                db.Sp_Add_Activity(userid, "Accessed Pending PR Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//                return PartialView(res);
//            }
//            else
//            {
//                var res = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Pending.ToString()).ToList();

//                db.Sp_Add_Activity(userid, "Accessed Pending PR Page Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//                return PartialView(res);
//            }
//        }
//        public ActionResult QuotationFinalize()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Quotation Finalization Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (User.IsInRole("Administrator") || User.IsInRole("All Projects") || User.IsInRole("View All Requisitions"))
//            {
//                var res = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Quotation_Approval.ToString()).Select(x => new FinalizePurchaseQuotations
//                {
//                    Deliver_Date = x.Deliver_Date,
//                    Dep_Name = x.Dep_Name,
//                    Description = x.Description,
//                    Group_Id = x.Group_Id,
//                    Id = x.Id,
//                    Item_Id = x.Item_Id,
//                    Item_Name = x.Item_Name,
//                    Prj_Id = x.Prj_Id,
//                    Prj_Name = x.Prj_Name,
//                    Project_Department = x.Project_Department,
//                    Qty = x.Qty,
//                    Requisition_No = x.Requisition_No,
//                    PurReq_DemReq = x.PurReq_DemReq,
//                    ReqTo_Dep = x.ReqTo_Dep,
//                    RequestedBy_Name = x.RequestedBy_Name,
//                    Requested_Date = x.Requested_Date,
//                    STATUS = x.STATUS,
//                    UOM = x.UOM,
//                    SKU = x.SKU
//                }).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                List<FinalizePurchaseQuotations> lst = new List<FinalizePurchaseQuotations>();
//                var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Quotation_Approval.ToString()).Select(x => new FinalizePurchaseQuotations
//                {
//                    Deliver_Date = x.Deliver_Date,
//                    Dep_Name = x.Dep_Name,
//                    Description = x.Description,
//                    Group_Id = x.Group_Id,
//                    Id = x.Id,
//                    Item_Id = x.Item_Id,
//                    Item_Name = x.Item_Name,
//                    Prj_Id = x.Prj_Id,
//                    Prj_Name = x.Prj_Name,
//                    Project_Department = x.Project_Department,
//                    Qty = x.Qty,
//                    Requisition_No = x.Requisition_No,
//                    PurReq_DemReq = x.PurReq_DemReq,
//                    ReqTo_Dep = x.ReqTo_Dep,
//                    RequestedBy_Name = x.RequestedBy_Name,
//                    Requested_Date = x.Requested_Date,
//                    STATUS = x.STATUS,
//                    UOM = x.UOM,
//                    SKU = x.SKU
//                }).ToList();
//                lst.AddRange(res1);
//                //if (User.IsInRole("Special Purchase Requisitions"))
//                //{
//                //    var res2 = db.Sp_Get_SpecialPurchaseRequestions(userid,User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Quotation_Approval.ToString()).Select(x => new FinalizePurchaseQuotations
//                //    {
//                //        Deliver_Date = x.Deliver_Date,
//                //        Dep_Name = x.Dep_Name,
//                //        Description = x.Description,
//                //        Group_Id = x.Group_Id,
//                //        Id = x.Id,
//                //        Item_Id = x.Item_Id,
//                //        Item_Name = x.Item_Name,
//                //        Prj_Id = x.Prj_Id,
//                //        Prj_Name = x.Prj_Name,
//                //        Project_Department = x.Project_Department,
//                //        Qty = x.Qty,
//                //        Requisition_No = x.Requisition_No,
//                //        //PurReq_DemReq = x.PurReq_DemReq,
//                //        //ReqTo_Dep = x.ReqTo_Dep,
//                //        RequestedBy_Name = x.RequestedBy_Name,
//                //        Requested_Date = x.Requested_Date,
//                //        STATUS = x.STATUS,
//                //        UOM = x.UOM,
//                //        SKU = x.SKU

//                //    }).ToList();
//                //    lst.AddRange(res2);
//                //}
//                return PartialView(lst);
//            }
//        }
//        public ActionResult Demand_Finalization()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Demand Finalization Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (User.IsInRole("Administrator") || User.IsInRole("All Projects"))
//            {
//                var res = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Demand_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var res = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Demand_Approval.ToString()).ToList();
//                return PartialView(res);
//            }
//        }
//        public ActionResult PO_Generated()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Genertated PO's Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (User.IsInRole("Administrator") || User.IsInRole("All Projects"))
//            {
//                var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id);
//                var res = db.Inventory_PurchaseOrder.Where(x => res1.Contains(x.Group_Id) && x.StockEntered == false).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, User.IsInRole("Head of Department"), PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id);
//                var res = db.Inventory_PurchaseOrder.Where(x => res1.Contains(x.Group_Id) && x.StockEntered == false).ToList();
//                return PartialView(res);
//            }
//        }
//        public JsonResult GetSearchValue(string search)
//        {
//            var res = db.Inventories.Where(x => x.Complete_Name.Contains(search)).Take(40).ToList();
//            return Json(res);
//        }
//        // 





//        // Demand Order
//        public ActionResult NewPurchaseRequisition(long? Id, long? Req)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed New PR Page for " + Req, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
//            var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var res2 = db.Employee_Designations.Where(x => x.Emp_Id == res1).Select(x => (long)x.Department_Id).ToList();

//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();

//            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => res2.Contains(x.Id)), "Id", "Name", Id);


//            if (User.IsInRole("All Projects") || User.IsInRole("Administrator"))
//            {
//                ViewBag.Project = new SelectList(db.Projects.ToList(), "Id", "Name", Id);
//            }
//            else
//            {
//                var prjs = db.Prj_Attendees.Where(x => x.Userid == res1).Select(x => x.PrjId).Distinct().ToList();
//                ViewBag.Project = new SelectList(db.Projects.Where(x => prjs.Contains(x.Id)).ToList(), "Id", "Name", Id);
//            }
//            if (Req != null)
//            {
//                var res = db.Inventory_Demand_Req.Where(x => x.Group_Id == Req).ToList();
//                return View(res);
//            }
//            else
//            {
//                return View();
//            }

//        }
//        public ActionResult NewPurchaseRequisitionForOther(long? Id, long? Req)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed New PR Page for " + Req, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
//            var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var res2 = db.Employee_Designations.Where(x => x.Emp_Id == res1).Select(x => (long)x.Department_Id).ToList();
//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();


//            if (User.IsInRole("All Department") || User.IsInRole("Administrator"))
//            {
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == "Department"), "Id", "Name", Id);
//            }
//            else
//            {
//                var prjs = db.Prj_Attendees.Where(x => x.Userid == res1).Select(x => x.PrjId).Distinct().ToList();
//                ViewBag.Department = new SelectList(db.Projects.Where(x => prjs.Contains(x.Id)).ToList(), "Id", "Name", Id);

//            }

//            if (User.IsInRole("All Projects") || User.IsInRole("Administrator"))
//            {
//                ViewBag.Project = new SelectList(db.Projects.ToList(), "Id", "Name", Id);
//            }
//            else
//            {
//                var prjs = db.Prj_Attendees.Where(x => x.Userid == res1).Select(x => x.PrjId).Distinct().ToList();
//                ViewBag.Project = new SelectList(db.Projects.Where(x => prjs.Contains(x.Id)).ToList(), "Id", "Name", Id);
//            }
//            if (Req != null)
//            {
//                var res = db.Inventory_Demand_Req.Where(x => x.Group_Id == Req).ToList();
//                return View(res);
//            }
//            else
//            {
//                return View();
//            }

//        }
//        [HttpPost]
//        public JsonResult NewPurchaseRequisitionForOther(List<Inventory_Purchase_Req> demand, long? prj, long? Demand_Req, DateTime? Delivery_Date, int Department)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Created New PR with delivery date " + Delivery_Date, "Create", "Activity_Record", ActivityType.Inventory.ToString(), Demand_Req);
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var des = db.Comp_Dep_Desig.Where(x => x.Id == Department).FirstOrDefault();
//                var empDes = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();

//                List<Inventory> dem = new List<Inventory>();
//                List<Inventory_Demand_Req> invdepreq = new List<Inventory_Demand_Req>();
//                foreach (var item in demand)
//                {
//                    if (item.Item_Id == null)
//                    {
//                        Inventory inven = new Inventory()
//                        {
//                            Item_Name = item.Item_Name
//                        };
//                        item.Item_Id = this.AddNewInventoryItem_NC(inven);
//                        item.ReqStatus = true;
//                    }
//                    var inv = db.Inventories.Where(x => x.Id == item.Item_Id).FirstOrDefault();
//                    dem.Add(inv);

//                }


//                if (dem.Count > 0)
//                {
//                    var purItems = new XElement("Products", dem.Select(x => new XElement("ProductInfo",
//                                       new XAttribute("itemId", x.Id),
//                                       new XAttribute("itemName", x.Complete_Name),
//                                       (Delivery_Date == null) ? null : new XAttribute("Delivery_Date", Delivery_Date),
//                                       (x.Length == null) ? null : new XAttribute("Len", x.Length),
//                                       (x.L_UOM == null) ? null : new XAttribute("Len_UOM", x.L_UOM),
//                                       (x.Width == null) ? null : new XAttribute("Wid", x.Width),
//                                       (x.W_UOM == null) ? null : new XAttribute("Wid_UOM", x.W_UOM),
//                                       (x.Heigth == null) ? null : new XAttribute("Hei", x.Heigth),
//                                       (x.H_UOM == null) ? null : new XAttribute("Hei_UOM", x.H_UOM),
//                                       (x.Diameter == null) ? null : new XAttribute("Dia", x.Diameter),
//                                       (x.D_UOM == null) ? null : new XAttribute("Dia_UOM", x.D_UOM),
//                                       (x.Size == null) ? null : new XAttribute("Size", x.Size),
//                                       (x.Size_UOM == null) ? null : new XAttribute("S_UOM", x.Size_UOM),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Attachment_Id).FirstOrDefault() == null) ? null : new XAttribute("Attachment_Id", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Attachment_Id).FirstOrDefault()),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Qty).FirstOrDefault() == null) ? null : new XAttribute("Qty", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Qty).FirstOrDefault()),
//                                       (x.UOM == null) ? null : new XAttribute("UOM", x.UOM),
//                                       (des.Id == null) ? null : new XAttribute("Dep_Id", des.Id),
//                                       (des.Name == null) ? null : new XAttribute("Dep_Name", des.Name),
//                                       (demand.Select(y => y.Group_Id).FirstOrDefault() == null) ? null : new XAttribute("Grp", demand.Select(y => y.Group_Id).FirstOrDefault()),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Description).FirstOrDefault() == null) ? null : new XAttribute("Descr", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Description).FirstOrDefault()),
//                                       (empDes.DesignationName == null) ? null : new XAttribute("RequestedBy_Desgination", empDes.DesignationName),
//                                       (demand.Select(y => y.Demand_Req).FirstOrDefault() == null) ? null : new XAttribute("Demand_Req", demand.Select(y => y.Demand_Req).FirstOrDefault()),
//                                       new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                    if (Demand_Req != null && Demand_Req > 0)
//                    {
//                        db.Sp_Update_DemandReq_Completion(Demand_Req);
//                    }
//                    var res = db.SP_Add_PurchaseRequisition(purItems, prj, uid).ToList();
//                    if (res.Count > 0)
//                    {
//                        return Json(new Return { Status = true, Msg = "Purchase Requisition has been applied successfully and Send for approval to Manager." });
//                    }
//                    else
//                    {
//                        return Json(new Return { Status = false, Msg = "Purchase Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                    }
//                }
//                else
//                {
//                    return Json(new Return { Status = true, Msg = "Requested" });
//                }

//            }
//            catch (Exception ex)
//            {

//                db.Sp_Add_ErrorLog(ex.Message + " Inner Exception: " + ex.InnerException, "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new Return { Status = false, Msg = "Error in saving purchase requisition." });
//            }
//        }
//        public ActionResult EditPurchaseRequisition(long Group_Id)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Edit PR Page for " + Group_Id, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Group_Id);
//            var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var res2 = db.Employee_Designations.Where(x => x.Emp_Id == res1).Select(x => (long)x.Department_Id).ToList();
//            ViewBag.Group_Id = Group_Id;
//            var items_in = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group_Id).ToList();
//            var prj_id = items_in.Select(x => x.Prj_Id).FirstOrDefault();
//            if (User.IsInRole("All Projects") || User.IsInRole("Administrator"))
//            {
//                ViewBag.Project = new SelectList(db.Projects.ToList(), "Id", "Name", prj_id);
//            }
//            else
//            {
//                var prjs = db.Prj_Attendees.Where(x => x.Userid == res1).Select(x => x.PrjId).Distinct().ToList();
//                ViewBag.Project = new SelectList(db.Projects.Where(x => prjs.Contains(x.Id)).ToList(), "Id", "Name", prj_id);
//            }
//            return PartialView(items_in);

//        }
//        [HttpPost]
//        public JsonResult NewPurchaseRequisition(List<Inventory_Purchase_Req> demand, long? prj, long? Demand_Req, DateTime? Delivery_Date, int Department)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Created New PR with delivery date " + Delivery_Date, "Create", "Activity_Record", ActivityType.Inventory.ToString(), Demand_Req);
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var des = db.Sp_Get_Employee_Designation(empid).Where(x=> x.CDD_Id == Department ).FirstOrDefault();

//                List<Inventory> dem = new List<Inventory>();
//                List<Inventory_Demand_Req> invdepreq = new List<Inventory_Demand_Req>();
//                foreach (var item in demand)
//                {
//                    if (item.Item_Id == null)
//                    {
//                        Inventory inven = new Inventory()
//                        {
//                            Item_Name = item.Item_Name
//                        };
//                        item.Item_Id = this.AddNewInventoryItem_NC(inven);
//                        item.ReqStatus = true;
//                    }

//                    if (item.ReqStatus)
//                    {
//                        var inv = db.Inventories.Where(x => x.Id == item.Item_Id).FirstOrDefault();
//                        dem.Add(inv);
//                    }
//                    else
//                    {
//                        var depitem = db.Inventory_Department.Where(x => x.Item_Id == item.Item_Id).FirstOrDefault();
//                        var inv = db.Inventories.Where(x => x.Id == item.Item_Id).FirstOrDefault();
//                        Inventory_Demand_Req idr = new Inventory_Demand_Req()
//                        {
//                            Item_Name = inv.Complete_Name,
//                            ReqTo_Dep = depitem.Dep_Id,
//                            ReqTo_Dep_Name = depitem.Dep_Name,
//                            Details = item.Description,
//                            Qty = item.Qty,
//                            Dep_Id = des.CDD_Id,
//                            Dep_Name = des.Name
//                        };
//                        invdepreq.Add(idr);
//                    }
//                }
//                if (invdepreq.Any())
//                {
//                    this.AddDemandReq(invdepreq, demand.Select(y => y.Group_Id).FirstOrDefault());
//                }

//                if (dem.Count > 0)
//                {
//                    var purItems = new XElement("Products", dem.Select(x => new XElement("ProductInfo",
//                                       new XAttribute("itemId", x.Id),
//                                       new XAttribute("itemName", x.Complete_Name),
//                                       (Delivery_Date == null) ? null : new XAttribute("Delivery_Date", Delivery_Date),
//                                       (x.Length == null) ? null : new XAttribute("Len", x.Length),
//                                       (x.L_UOM == null) ? null : new XAttribute("Len_UOM", x.L_UOM),
//                                       (x.Width == null) ? null : new XAttribute("Wid", x.Width),
//                                       (x.W_UOM == null) ? null : new XAttribute("Wid_UOM", x.W_UOM),
//                                       (x.Heigth == null) ? null : new XAttribute("Hei", x.Heigth),
//                                       (x.H_UOM == null) ? null : new XAttribute("Hei_UOM", x.H_UOM),
//                                       (x.Diameter == null) ? null : new XAttribute("Dia", x.Diameter),
//                                       (x.D_UOM == null) ? null : new XAttribute("Dia_UOM", x.D_UOM),
//                                       (x.Size == null) ? null : new XAttribute("Size", x.Size),
//                                       (x.Size_UOM == null) ? null : new XAttribute("S_UOM", x.Size_UOM),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Attachment_Id).FirstOrDefault() == null) ? null : new XAttribute("Attachment_Id", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Attachment_Id).FirstOrDefault()),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Qty).FirstOrDefault() == null) ? null : new XAttribute("Qty", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Qty).FirstOrDefault()),
//                                       (x.UOM == null) ? null : new XAttribute("UOM", x.UOM),
//                                       (des.CDD_Id == null) ? null : new XAttribute("Dep_Id", des.CDD_Id),
//                                       (des.Name== null) ? null : new XAttribute("Dep_Name", des.Name),
//                                       (demand.Select(y => y.Group_Id).FirstOrDefault() == null) ? null : new XAttribute("Grp", demand.Select(y => y.Group_Id).FirstOrDefault()),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Description).FirstOrDefault() == null) ? null : new XAttribute("Descr", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Description).FirstOrDefault()),
//                                       (demand.Select(y => y.Demand_Req).FirstOrDefault() == null) ? null : new XAttribute("Demand_Req", demand.Select(y => y.Demand_Req).FirstOrDefault()),
//                                       new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                    if (Demand_Req != null && Demand_Req > 0)
//                    {
//                        db.Sp_Update_DemandReq_Completion(Demand_Req);
//                    }
//                    var res = db.SP_Add_PurchaseRequisition(purItems, prj, uid).ToList();
//                    if (res.Count > 0)
//                    {
//                        return Json(new Return { Status = true, Msg = "Purchase Requisition has been applied successfully and Send for approval to Manager." });
//                    }
//                    else
//                    {
//                        return Json(new Return { Status = false, Msg = "Purchase Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                    }
//                }
//                else
//                {
//                    return Json(new Return { Status = true, Msg = "Requested" });
//                }

//            }
//            catch (Exception ex)
//            {

//                db.Sp_Add_ErrorLog(ex.Message + " Inner Exception: " + ex.InnerException, "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new Return { Status = false, Msg = "Error in saving purchase requisition." });
//            }
//        }
//        [HttpPost]
//        public JsonResult savePurchaseRequisition(List<Inventory_Purchase_Req> demand, long? prj, long? Demand_Req, DateTime? Delivery_Date)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Created New PR with delivery date " + Delivery_Date, "Create", "Activity_Record", ActivityType.Inventory.ToString(), Demand_Req);
//                var empid = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var des = db.Sp_Get_EmployeeFirstDesignation(empid).FirstOrDefault();
//                var grp_id = demand.Select(x => x.Group_Id).FirstOrDefault();
//                db.Sp_Delete_PurchaseReq(grp_id);
//                List<Inventory> dem = new List<Inventory>();
//                List<Inventory_Demand_Req> invdepreq = new List<Inventory_Demand_Req>();
//                foreach (var item in demand)
//                {
//                    if (item.Item_Id == null)
//                    {
//                        Inventory inven = new Inventory()
//                        {
//                            Item_Name = item.Item_Name
//                        };
//                        item.Item_Id = this.AddNewInventoryItem_NC(inven);
//                        item.ReqStatus = true;
//                    }

//                    var inv = db.Inventories.Where(x => x.Id == item.Item_Id).FirstOrDefault();
//                    dem.Add(inv);
//                }

//                if (dem.Count > 0)
//                {
//                    var purItems = new XElement("Products", dem.Select(x => new XElement("ProductInfo",
//                                       new XAttribute("itemId", x.Id),
//                                       new XAttribute("itemName", x.Complete_Name),
//                                       (Delivery_Date == null) ? null : new XAttribute("Delivery_Date", Delivery_Date),
//                                       (x.Length == null) ? null : new XAttribute("Len", x.Length),
//                                       (x.L_UOM == null) ? null : new XAttribute("Len_UOM", x.L_UOM),
//                                       (x.Width == null) ? null : new XAttribute("Wid", x.Width),
//                                       (x.W_UOM == null) ? null : new XAttribute("Wid_UOM", x.W_UOM),
//                                       (x.Heigth == null) ? null : new XAttribute("Hei", x.Heigth),
//                                       (x.H_UOM == null) ? null : new XAttribute("Hei_UOM", x.H_UOM),
//                                       (x.Diameter == null) ? null : new XAttribute("Dia", x.Diameter),
//                                       (x.D_UOM == null) ? null : new XAttribute("Dia_UOM", x.D_UOM),
//                                       (x.Size == null) ? null : new XAttribute("Size", x.Size),
//                                       (x.Size_UOM == null) ? null : new XAttribute("S_UOM", x.Size_UOM),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Attachment_Id).FirstOrDefault() == null) ? null : new XAttribute("Attachment_Id", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Attachment_Id).FirstOrDefault()),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Qty).FirstOrDefault() == null) ? null : new XAttribute("Qty", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Qty).FirstOrDefault()),
//                                       (x.UOM == null) ? null : new XAttribute("UOM", x.UOM),
//                                       (des.DepartmentId == null) ? null : new XAttribute("Dep_Id", des.DepartmentId),
//                                       (des.DepartmentName == null) ? null : new XAttribute("Dep_Name", des.DepartmentName),
//                                       (demand.Select(y => y.Group_Id).FirstOrDefault() == null) ? null : new XAttribute("Grp", demand.Select(y => y.Group_Id).FirstOrDefault()),
//                                       (demand.Where(y => y.Item_Id == x.Id).Select(y => y.Description).FirstOrDefault() == null) ? null : new XAttribute("Descr", demand.Where(y => y.Item_Id == x.Id).Select(y => y.Description).FirstOrDefault()),
//                                       (des.DesignationName == null) ? null : new XAttribute("RequestedBy_Desgination", des.DesignationName),
//                                       (demand.Select(y => y.Demand_Req).FirstOrDefault() == null) ? null : new XAttribute("Demand_Req", demand.Select(y => y.Demand_Req).FirstOrDefault()),
//                                       new XAttribute("Stat", PurchaseRequisitionStatus.Pending_Approval)))).ToString();
//                    if (Demand_Req != null && Demand_Req > 0)
//                    {
//                        db.Sp_Update_DemandReq_Completion(Demand_Req);
//                    }
//                    var res = db.SP_Add_PurchaseRequisition(purItems, prj, uid).ToList();
//                    if (res.Count > 0)
//                    {
//                        return Json(new Return { Status = true, Msg = "Purchase Requisition has been applied successfully and Send for approval to Manager." });
//                    }
//                    else
//                    {
//                        return Json(new Return { Status = false, Msg = "Purchase Requisition forwarded with some errors. Please check the record from your requisitions tab." });
//                    }
//                }
//                else
//                {
//                    return Json(new Return { Status = true, Msg = "Requested" });
//                }

//            }
//            catch (Exception ex)
//            {

//                db.Sp_Add_ErrorLog(ex.Message + " Inner Exception: " + ex.InnerException, "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new Return { Status = false, Msg = "Error in saving purchase requisition." });
//            }
//        }
//        private long? AddNewInventoryIte(Inventory Inv)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Added New Inventory Item " + Inv.Item_Name, "Create", "Activity_Record", ActivityType.Inventory.ToString(), Inv.Category_Id);
//            try
//            {
//                var res2 = db.Inventories.Any(x => x.Item_Name == Inv.Item_Name && x.Length == Inv.Length && x.Width == Inv.Width && x.Heigth == Inv.Heigth && x.Diameter == Inv.Diameter);
//                if (res2)
//                {
//                    var myRes = db.Inventories.Where(x => x.Item_Name == Inv.Item_Name && x.Length == Inv.Length && x.Width == Inv.Width && x.Heigth == Inv.Heigth && x.Diameter == Inv.Diameter).FirstOrDefault();
//                    return myRes.Id;
//                }
//                db.Inventories.Add(new Inventory
//                {
//                    Description = Inv.Description,
//                    Diameter = Inv.Diameter,
//                    Heigth = Inv.Heigth,
//                    Is_Perishable = Inv.Is_Perishable,
//                    Item_Name = Inv.Item_Name,
//                    Length = Inv.Length,
//                    Minimum_Qty = Inv.Minimum_Qty,
//                    UOM = Inv.UOM,
//                    Userid = userid,
//                    Width = Inv.Width,
//                    L_UOM = Inv.L_UOM,
//                    W_UOM = Inv.W_UOM,
//                    H_UOM = Inv.H_UOM,
//                    D_UOM = Inv.D_UOM,
//                    Size = Inv.Size,
//                    Size_UOM = Inv.Size_UOM
//                });
//                var id = db.SaveChanges();
//                return id;
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Add Inventory Item Error");
//                return 0;
//            }
//        }
//        // Vendor
//        /// <summary>
//        /// ///////////////////////// Payment Module
//        /// </summary>
//        /// <returns></returns>
//        /// <summary>///////

//        [HttpPost]
//        public JsonResult UpdateGeneralEntry(long Id, long Head_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.Sp_Get_GeneralEntry_Parameter(Id).SingleOrDefault();
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var a = db.Sp_Update_GeneralEntry_Reversal(Id).ToString();
//                    var res2 = db.Sp_Add_General_Entry(res1.Credit, res1.Debit, null, null, null, null, res1.Head, res1.Head_Id, res1.Head_Code, res1.Head_Name, res1.Naration, res1.GroupId, res1.Line, res1.Qty, res1.UOM, res1.Rate, userid, null, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
//                    var b = db.Sp_Update_GeneralEntry_Reversal(res2).ToString();
//                    var res3 = db.Sp_Get_CA_Head_Parameter(Head_Id).SingleOrDefault();
//                    var res4 = db.Sp_Add_General_Entry(res1.Debit, res1.Credit, null, null, null, null, res3.Text_ChartCode + " - " + res3.Head, res3.Id, res3.Text_ChartCode, res3.Head, res1.Naration, res1.GroupId, res1.Line, res1.Qty, res1.UOM, res1.Rate, userid, null, Voucher_Type.JV.ToString(), comp.Id).FirstOrDefault();
//                    Transaction.Commit();
//                    return Json(true);
//                }
//                catch (Exception ex)
//                {
//                    EmailService e = new EmailService();
//                    e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Update Journal Entry Error");
//                    Transaction.Rollback();
//                    return Json(false);
//                }
//            }
//        }
//        /// <summary>///////

//        /// <summary>
//        /// //////////////////////////////
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult AddInventory()
//        {
//            ViewBag.Category = new SelectList(db.Inventory_Category.ToList(), "Id", "Name");
//            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == "Department").ToList(), "Id", "Name");
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Added Inventory  Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }
//        [HttpPost]
//        public JsonResult AddInventory(Inventory Inv, List<DeptsModel> depts)
//        {
//            var res = AddNewInventoryItem(Inv, depts);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Added New Inventory " + Inv.Item_Name, "Create", "Activity_Record", ActivityType.Inventory.ToString(), Inv.Category_Id);
//            if (res is null)
//            {
//                var res1 = new { Status = false, Msg = "Item Already Exist." };
//                return Json(res1);
//            }
//            else if (res == 0)
//            {
//                var res1 = new { Status = false, Msg = "Item Already Exist." };
//                return Json(res1);
//            }
//            else
//            {
//                var res1 = new { Status = true, Msg = "Item Added Successfully", Id = res };
//                return Json(res1);
//            }
//        }
//        private long? AddNewInventoryItem_NC(Inventory Inv) // without code
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            try
//            {
//                var res2 = db.Inventories.FirstOrDefault(x => x.Company == Inv.Company && x.Item_Name == Inv.Item_Name && x.Length == Inv.Length && x.L_UOM == Inv.L_UOM && x.Width == Inv.Width && x.W_UOM == Inv.W_UOM && x.Heigth == Inv.Heigth && x.H_UOM == Inv.H_UOM && x.Diameter == Inv.Diameter && x.D_UOM == Inv.D_UOM && x.Size == Inv.Size && x.Size_UOM == Inv.Size_UOM);
//                if (res2 != null)
//                {
//                    return res2.Id;
//                }
//                db.Inventories.Add(Inv);
//                db.SaveChanges();
//                return Inv.Id;
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Add Inventory Item Error");
//                return 0;
//            }
//        }
//        private long? AddNewInventoryItem(Inventory Inv, List<DeptsModel> depts)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            try
//            {
//                var res2 = db.Inventories.FirstOrDefault(x => x.Company == Inv.Company && x.Item_Name == Inv.Item_Name && x.Length == Inv.Length && x.L_UOM == Inv.L_UOM && x.Width == Inv.Width && x.W_UOM == Inv.W_UOM && x.Heigth == Inv.Heigth && x.H_UOM == Inv.H_UOM && x.Diameter == Inv.Diameter && x.D_UOM == Inv.D_UOM && x.Size == Inv.Size && x.Size_UOM == Inv.Size_UOM);
//                if (res2 != null)
//                { 
//                    return res2.Id;
//                }
//                db.Inventories.Add(Inv);
//                db.SaveChanges();
//                var inv = false;

//                int? headId = Convert.ToInt32(Inv.Head_Code);
//                AccountHandlerController ah = new AccountHandlerController();
//                if (headId != null)
//                {
//                    var comp = ah.Company_Attr(userid);
//                    var headdel = db.COA_Mapper.Where(x =>
//                            x.CompanyId == comp.Id &&
//                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
//                            x.ModuleType == COA_Mapper_ModuleTypes.Item_List.ToString() &&
//                            x.Module_Id == Inv.Id
//                            ).FirstOrDefault();


//                    if (headdel != null)
//                    {
//                        db.COA_Mapper.Remove(headdel);
//                        db.SaveChanges();
//                    }
//                    ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Procurement.ToString(), COA_Mapper_ModuleTypes.Item_List.ToString(), Inv.Id, headId, userid);

//                    var del = db.COA_Mapper.Where(x =>
//                            x.CompanyId == comp.Id &&
//                            x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                            x.Module == COA_Mapper_Modules.Procurement.ToString() &&
//                            x.ModuleType == COA_Mapper_ModuleTypes.Item_List.ToString() &&
//                            x.Module_Id == Inv.Id
//                            ).FirstOrDefault();

//                    var cod = "/1/1/2/1/";
//                    if (del.AccountCode.Contains(cod))
//                    {
//                        inv = true;
//                    }
//                }


//                var cat = db.Inventory_Category.Where(x => x.Id == Inv.Category_Id).FirstOrDefault();
//                if (cat == null)
//                {
//                    return Inv.Id;
//                }
//                var num = db.Sp_Get_ReceiptNo("Item Code").FirstOrDefault();
//                var a = num.Split('-')[1];
//                string code = "SAG-" + cat.Code + "-" + a;
                
//                var c = db.Plot_Add_Receipt(code, null, null, null, null, null, null, null, null, Inv.Id, null, null, inv);

//                //Setting Up Departments
//                foreach (var ind in depts)
//                {
//                    db.Sp_Add_Inventory_Dep(Inv.Id, ind.deptId, ind.deptName);
//                }
//                return Inv.Id;
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Add Inventory Item Error");
//                return 0;
//            }
//        }
//        public ActionResult GoodReceiveNotePrint(string Id)
//        {
//            var uid = User.Identity.GetUserId<long>();
//            var res = db.Inventory_Stock_In.Where(x => x.GRN == Id).ToList();
//            ViewBag.grn = Id;
//            ViewBag.username = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
//            ViewBag.Inv = db.Inventory_Stock_In.Where(x => x.GRN == Id).Select(x => x.InvoiceNo).FirstOrDefault();
//            foreach (var v in res)
//            {
//                var dat = db.Inventories.Where(x => x.Id == v.Item_Id).Select(x => new { x.Complete_Name, x.UOM, x.SKU }).FirstOrDefault();
//                v.Item_name = dat.Complete_Name;
//                v.UOM = dat.UOM;
//                v.SKU = dat.SKU;
//            }
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Printed GRN " + Id, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View(res);
//        }
//        public ActionResult ManualStockInPrint(string Id)
//        {
//            var uid = User.Identity.GetUserId<long>();
//            var res = db.Inventory_Stock_In.Where(x => x.GRN == Id).ToList();
//            ViewBag.grn = Id;
//            ViewBag.username = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
//            ViewBag.Inv = db.Inventory_Stock_In.Where(x => x.GRN == Id).Select(x => x.InvoiceNo).FirstOrDefault();
//            foreach (var v in res)
//            {
//                var dat = db.Inventories.Where(x => x.Id == v.Item_Id).Select(x => new { x.Complete_Name, x.UOM, x.SKU }).FirstOrDefault();
//                v.Item_name = dat.Complete_Name;
//                v.UOM = dat.UOM;
//                v.SKU = dat.SKU;
//            }
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Printed GRN " + Id, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View(res);
//        }
//        public ActionResult ReturnNotePrint(string Id)
//        {
//            var uid = User.Identity.GetUserId<long>();
//            var res = db.Inventory_Stock_In.Where(x => x.GRN == Id).ToList();
//            ViewBag.grn = Id;
//            ViewBag.username = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
//            ViewBag.Inv = db.Inventory_Stock_In.Where(x => x.GRN == Id).Select(x => x.InvoiceNo).FirstOrDefault();
//            foreach (var v in res)
//            {
//                var dat = db.Inventories.Where(x => x.Id == v.Item_Id).Select(x => new { x.Complete_Name, x.UOM, x.SKU }).FirstOrDefault();
//                v.Item_name = dat.Complete_Name;
//                v.UOM = dat.UOM;
//                v.SKU = dat.SKU;
//            }
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Printed Return Note " + Id, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View(res);
//        }
//        public ActionResult DeliveryNotePrint(string Id)
//        {
//            var uid = User.Identity.GetUserId<long>();
//            var res = db.Inventory_Stock_In.Where(x => x.GRN == Id).ToList();
//            var po = res.Select(y => y.PO_Number).FirstOrDefault();
//            var prj = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == po).FirstOrDefault();
//            ViewBag.PrjName = prj.Prj_Name;
//            ViewBag.Prjoffsite = prj.Prj_Offsite;
//            ViewBag.grn = Id;
//            ViewBag.username = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
//            foreach (var v in res)
//            {
//                var dat = db.Inventories.Where(x => x.Id == v.Item_Id).Select(x => new { x.Complete_Name, x.UOM }).FirstOrDefault();
//                v.Item_name = dat.Complete_Name;
//                v.UOM = dat.UOM;
//                v.SKU = ItemCode(v.Item_Id);
//            }
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Printed Delivery Note " + Id, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View(res);
//        }
//        public ActionResult InventoryList()
//        {
//            return View();
//        }
//        public ActionResult Department_Wise()
//        {
//            var Dep = db.Inventory_Stock_In.Select(x => new LeavesTypes { Value = x.Dep_Id, Name = x.Dep_Name }).Distinct().ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Inventory List Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView(Dep);
//        }
//        public ActionResult Dep_InventoryList(long Id)
//        {
//            string Ids = new XElement("Dep", new XElement("DepId", new XAttribute("Id", Id))).ToString();
//            var res = db.Sp_Get_InventoryList_Dep(Ids).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Departmental Inventory Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
//            return PartialView(res);
//        }
//        public ActionResult OnePagerInventory()
//        {
//            var dep = db.Comp_Dep_Desig.Where(x => x.Type == "Department").ToList();
//            string Ids = new XElement("Dep", dep.Select(x=>  new XElement("DepId", new XAttribute("Id", x.Id)))).ToString();
//            var res = db.Sp_Get_InventoryList_Dep(Ids).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Departmental Inventory Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), null);
//            return PartialView(res);
//        }
//        public ActionResult InventoryByIdSearch(long Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res1 = db.Inventories.Where(x => x.Id == Id).FirstOrDefault();
//            var invdep = db.Inventory_Department.Where(x => x.Item_Id == Id).ToList();

//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var itemhead = ah.HeadFinder(COA_Mapper_Modules.Procurement.ToString(), COA_Mapper_ModuleTypes.Item_List.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, Id);
//            ViewBag.Category = new SelectList(db.Inventory_Category.ToList(), "Id", "Name", res1.Category_Id);
//            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == "Department").ToList(), "Id", "Name");
//            var res = new Inv_Dep { Inv = res1, InvDep = invdep, InvHead = itemhead };
//            db.Sp_Add_Activity(userid, "Searched Inventory for " + res1.Item_Name, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
//            return PartialView(res);
//        }
//        [HttpPost]
//        public JsonResult InventoryUpdate(Inventory inv, List<DeptsModel> depts)
//        {
//            var pre = db.Inventories.Where(x => x.Id == inv.Id).FirstOrDefault();
//            if (pre.Category_Id != inv.Category_Id)
//            {
//                var cat = db.Inventory_Category.Where(x => x.Id == inv.Category_Id).FirstOrDefault();
//                var num = db.Sp_Get_ReceiptNo("Item Code").FirstOrDefault();
//                var a = num.Split('-')[1];
//                string code = "SAG-" + cat.Code + "-" + a;
//                var c = db.Plot_Add_Receipt(code, null, null, null, null, null, null, null, null, inv.Id, null, null, null);
//            }
//            db.Sp_Update_Inventory(inv.Id, inv.Category_Id, inv.Company, inv.Item_Name, inv.UOM, inv.Minimum_Qty, inv.Length, inv.L_UOM, inv.Width, inv.W_UOM, inv.Heigth, inv.H_UOM, inv.Diameter, inv.D_UOM, inv.Size, inv.Size_UOM, inv.Description, inv.Is_Perishable, inv.Packing_Size, inv.Hide, inv.Head_Code);
//            long userid = long.Parse(User.Identity.GetUserId());
//            int? headId = Convert.ToInt32(inv.Head_Code);
//            AccountHandlerController ah = new AccountHandlerController();
//            if (headId != null)
//            {
//                var comp = ah.Company_Attr(userid);
//                var headdel = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Procurement.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Item_List.ToString() &&
//                        x.Module_Id == inv.Id
//                        ).FirstOrDefault();
//                if (headdel != null)
//                {
//                    db.COA_Mapper.Remove(headdel);
//                    db.SaveChanges();
//                }
//                ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Procurement.ToString(), COA_Mapper_ModuleTypes.Item_List.ToString(), inv.Id, headId, userid);
//            }

//            db.Sp_Add_Activity(userid, "Updated Inventory Item " + inv.Item_Name, "Update", "Activity_Record", ActivityType.Inventory.ToString(), inv.Category_Id);
//            //update departments
//            if (depts != null)
//            {
//                foreach (var ind in depts)
//                {
//                    db.Sp_Add_Inventory_Dep(inv.Id, ind.deptId, ind.deptName);
//                }
//            }
//            var res = new { Status = true, Msg = "Item Updated Successfully" };
//            return Json(res);
//        }
//        [HttpPost]
//        public JsonResult InventoryDelete(Inventory inv)
//        {
//            db.Sp_Delete_Inventory(inv.Id);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted Inventory " + inv.Item_Name, "Delete", "Activity_Record", ActivityType.Inventory.ToString(), inv.Category_Id);
//            return Json(true);
//        }
//        //Puchase order & bidding & Good Receving
//        public ActionResult NewQuotations()
//        {
//            return PartialView();
//        }
//        public ActionResult PrintPurchaseOrder(string poNum)
//        {
//            try
//            {
//                var poData = db.Sp_Get_PO_Full_Detail(poNum).ToList();
//                var venId = poData.Select(y => new { y.Vendor_Id, y.Group_Id }).FirstOrDefault();
//                var res = db.Vendors.Where(x => x.Id == venId.Vendor_Id).FirstOrDefault();
//                var terms = db.Inventory_PO_Terms.Where(x => x.Req_Id == venId.Group_Id).ToList();
//                var reqdet = db.Inventory_Purchase_Req.Where(x => x.Group_Id == venId.Group_Id).FirstOrDefault();
//                ViewBag.Reqno = reqdet.Requisition_No;
//                ViewBag.ApprovedBy = reqdet.ApprovedBy_Name;
//                var uid = User.Identity.GetUserId<long>();
//                ViewBag.unm = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Printed PO " + poNum, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//                var rep = db.Vendor_Representative.Where(x => x.Vendor_Id == res.Id).ToList();
//                ViewBag.rep = string.Join(",", rep.Select(x => x.Name));
//                ViewBag.phone = string.Join(",", rep.Select(x => x.Mobile_Number ?? x.Office_Mobile));
//                return View(new PurchaseOrderPrintView { PO_Data = poData, Vendor_Data = res, PO_Terms = terms });

//            }
//            catch (Exception ex)
//            {
//                return View();
//            }
//        }
//        public ActionResult PurcahseOrderList()
//        {
//            var res = db.Inventory_PurchaseOrder.ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed PO's List ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View(res);
//        }
//        public ActionResult MultiItemIssueRequest(long Prj_Id)
//        {
//            long userid = User.Identity.GetUserId<long>();

//            if (User.IsInRole("Administrator"))
//            {
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x=> x.Type == "Department").ToList(), "Id", "Name", 1);

//                ViewBag.Employees = new SelectList(db.Sp_Get_Employee().Where(x => x.Status == "Registered").OrderBy(x => x.Department).ToList(), "Id", "Name", "Department", 1);
//                Helpers h = new Helpers();
//                ViewBag.GroupId = h.RandomNumber();
//            }
//            else
//            {
//                var res = db.Prj_Attendees.Where(x => x.PrjId == Prj_Id).Select(x => x.Dep_Id).ToList();
//                var ids = new XElement("Deps", res.Select(x => new XElement("Ids",
//                 new XAttribute("Id", x)
//                 ))).ToString();
//                var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).OrderBy(x => x.Department).ToList();
//                ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);

//                var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res2 = db.Employee_Designations.Where(x => x.Emp_Id == res1).Select(x => (long)x.Department_Id).ToList();
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => res2.Contains(x.Id)).ToList(), "Id", "Name", 1);


//                Helpers h = new Helpers();
//                ViewBag.GroupId = h.RandomNumber();
//            }

//            return PartialView();
//        }
//        public ActionResult MultiItemIssueDemRequest(long Prj_Id, long Req)
//        {
//            long userid = User.Identity.GetUserId<long>();

//            if (User.IsInRole("Administrator"))
//            {
//                var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res2 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//                ViewBag.Employees = new SelectList(db.Sp_Get_Employee().Where(x => x.Status == "Registered").OrderBy(x => x.Department).ToList(), "Id", "Name", "Department", 1);
//                Helpers h = new Helpers();
//                ViewBag.GroupId = h.RandomNumber();

//                var res = db.Inventory_Demand_Req.Where(x => x.Group_Id == Req).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var res = db.Prj_Attendees.Where(x => x.PrjId == Prj_Id).Select(x => x.Dep_Id).ToList();

//                var ids = new XElement("Deps", res.Select(x => new XElement("Ids",
//                 new XAttribute("Id", x)
//                 ))).ToString();
//                var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).OrderBy(x => x.Department).ToList();

//                ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);
//                Helpers h = new Helpers();
//                ViewBag.GroupId = h.RandomNumber();
//                var res4 = db.Inventory_Demand_Req.Where(x => x.Group_Id == Req).ToList();
//                return PartialView(res4);
//            }
//        }
//        public ActionResult ItemIssueDemRequest(long Req)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();

//            if (User.IsInRole("All Projects") || User.IsInRole("Administrator"))
//            {
//                var res2 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//                ViewBag.Employees = new SelectList(db.Sp_Get_Employee().Where(x => x.Status == "Registered").OrderBy(x => x.Department).ToList(), "Id", "Name", "Department", 1);
//                Helpers h = new Helpers();
//                ViewBag.GroupId = h.RandomNumber();
//                ViewBag.Project = new SelectList(db.Projects.ToList(), "Id", "Name");

//                var res = db.Inventory_Demand_Req.Where(x => x.Group_Id == Req).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                var prjs = db.Prj_Attendees.Where(x => x.Userid == res1).Select(x => x.PrjId).Distinct().ToList();
//                ViewBag.Project = new SelectList(db.Projects.Where(x => prjs.Contains(x.Id)).ToList(), "Id", "Name");

//                var res2 = db.Employee_Designations.Where(x => x.Emp_Id == res1).Select(x => (long)x.Department_Id).ToList();

//                var ids = new XElement("Deps", res2.Select(x => new XElement("Ids",
//                 new XAttribute("Id", x)
//                 ))).ToString();
//                var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).OrderBy(x => x.Department).ToList();

//                ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);
//                Helpers h = new Helpers();
//                ViewBag.GroupId = h.RandomNumber();
//                var res4 = db.Inventory_Demand_Req.Where(x => x.Group_Id == Req).ToList();
//                return PartialView(res4);
//            }
//        }
//        public ActionResult NewDemandOrder()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            Helpers h = new Helpers();
//            ViewBag.GroupId = h.RandomNumber();
//            db.Sp_Add_Activity(userid, "Accessed Mew Demand order Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }
//        public JsonResult SaveIssueRequest(long? item, long proj, string prj_Name, long? emp, string emp_Name, decimal qty, string rem, long Group_Id)
//        {
//            var uid = User.Identity.GetUserId<long>();
//            try
//            {
//                var EmpId = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var Depid = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => x.Department_Id).FirstOrDefault();
//                var res = db.Sp_Add_Stockout_Req("Pending", Group_Id, emp, emp_Name, item, proj, prj_Name, qty, uid, Depid).FirstOrDefault();
//                if (res == "0")
//                {
//                    var res2 = new { Status = false, Msg = "Demand Order Already Generated" };
//                    return Json(res2);
//                }
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Generated Demand Order  " + res, "Create", "Activity_Record", ActivityType.Inventory.ToString(), proj);
//                var res1 = new { Status = true, Msg = "Demand Order Generated. Demand Order Number is : " + res };
//                return Json(res1);
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "SaveIssueRequest Demand Order Error");
//                var res1 = new { Status = false, Msg = "Error Occured" };
//                return Json(res1);
//            }
//        }
//        public JsonResult SaveMultiIssueRequest(MultiDemandOrderViewData[] items, long Emp_Id, string Emp_Name, long Group_Id, long proj, string prj_name, long? Demand_Req, int? To_Comp_Id,int Department)
//        {
//            var uid = User.Identity.GetUserId<long>();
//            try
//            {
//                var itemsXML = new XElement("Demands", items.Select(x => new XElement("DemandItem",
//                      new XAttribute("itemId", x.item),
//                      new XAttribute("emp", Emp_Id),
//                      new XAttribute("empName", Emp_Name),
//                      new XAttribute("qty", x.qty),
//                      (Demand_Req is null) ? null : new XAttribute("Demand_Req", Demand_Req),
//                      (To_Comp_Id is null) ? null : new XAttribute("Comp_Id", To_Comp_Id),
//                      (x.rem is null) ? null : new XAttribute("rems", x.rem))
//                      )).ToString();
//                var EmpId = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var res = db.Sp_Add_Stockout_Multiple_Req(itemsXML, uid, Group_Id, proj, prj_name, Department).FirstOrDefault();

//                // Notifications
//                var users = db.Users.Where(x => x.Roles.Any(y => y.Name == "Demand Orders")).ToList();

//                NameValuestring n = new NameValuestring()
//                {
//                    Name = res,
//                    Value = Group_Id.ToString()
//                };


//                if (res == "0")
//                {
//                    var res2 = new { Status = false, Msg = "Demand Order Already Generated" };
//                    return Json(res2);
//                }
//                else
//                {
//                    foreach (var u in users)
//                    {
//                        Notifier.Notify(uid, Convert.ToInt64(u.Id), NotifierMsg.Issuance_Request, new object[] { n }, NotifierMsg.Issuance_Request.ToString());
//                    }
//                }
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Generated Demand Order  " + res, "Create", "Activity_Record", ActivityType.Inventory.ToString(), proj);

//                var res1 = new { Status = true, Msg = "Demand Order Generated. Demand Order No : " + res };
//                return Json(res1);
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "SaveMultiissueitem Demand Order Error");
//                var res1 = new { Status = false, Msg = "Error Occured" };
//                return Json(res1);
//            }
//        }
//        public ActionResult DemandOrders()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Demand Order Page", "Read", "Activity_Record", ActivityType.Inventory.ToString(), userid);

//            return View();
//        }
//        public ActionResult DemandOrdersList(DateTime? From, DateTime? To)
//        {
//            ViewBag.From = From;
//            ViewBag.To = To;
//            ViewBag.Type = "Pending";
//            var res = db.Sp_Get_StockoutList(From, To, null, null).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Pending Demand Order List From " + From + " To " + To, "Read", "Activity_Record", ActivityType.Inventory.ToString(), userid);
//            return PartialView(res);
//        }
//        public ActionResult ArchiveList(DateTime? From, DateTime? To)
//        {
//            ViewBag.From = From;
//            ViewBag.To = To;
//            ViewBag.Type = "Delivered";
//            var res = db.Sp_Get_StockoutList(From, To, "Delivered", null).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Delivered Demand Order List From " + From + " To " + To, "Read", "Activity_Record", ActivityType.Inventory.ToString(), userid);
//            return PartialView(res);
//        }
//        public ActionResult DemandOrder_Archive(DateTime? From, DateTime? To)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            ViewBag.From = From;
//            ViewBag.To = To;
//            ViewBag.Type = "Delivered";
//            var users = new XElement("Users", new XElement("UserId", new XAttribute("Id", userid))).ToString();
//            var res = db.Sp_Get_StockoutList(From, To, "Delivered", users).ToList();
//            return PartialView("DemandOrdersList", res);
//        }
//        public ActionResult IssueNote(long id)
//        {
//            var res = db.Sp_Get_Stockout_Details(id).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Issue Note ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), id);
//            return View(res);
//        }
//        public ActionResult DemandOrder_Details(long Group_Id)
//        {
//            var dep_id = db.Comp_Dep_Desig.Where(x => x.Name == "Store / Warehouse").Select(x => x.Id).FirstOrDefault();
//            var ids = new XElement("Deps", new XElement("Ids",
//                         new XAttribute("Id", dep_id)
//                         )).ToString();
//            ViewBag.issuer = new SelectList(db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name }).ToList(), "Id", "Name");

//            var res = db.Sp_Get_DemandOrder_Details(Group_Id).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Demand Order Details of  " + string.Join(",", res.Select(x => x.Complete_Name)), "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Group_Id);
//            foreach (var item in res)
//            {
//                var dep = new XElement("Deps", new XElement("Id", new XAttribute("Dep", item.Dep_Id))).ToString();

//                var st_in = db.Sp_Get_Inventory_TotalStock_In_Warehouse(item.Item_Id, dep, false).ToList();
//                var st_out = db.Sp_Get_Inventory_TotalStock_Out_Warehouse(item.Item_Id, dep, false).ToList();

//                //var st_in = (from x in db.Inventory_Stock_In
//                //             where x.Item_Id == item.Item_Id && x.Dep_Id == item.Dep_Id 
//                //             group x by new { x.Warehouse_Id, x.WarehouseName } into g
//                //             select new StockIn_Sum { Warehouse_Id = g.Key.Warehouse_Id, Warehouse_Name = g.Key.WarehouseName, Qty = g.Sum(x => x.Qty) }).ToList();



//                //var st_out = (from x in db.Inventory_Stock_Out
//                //              where x.Item_Id == item.Item_Id && x.Dep_Id == item.Dep_Id
//                //              group x by new { x.Warehouse_Id, x.Warehouse_Name } into g
//                //              select new StockIn_Sum { Warehouse_Id = g.Key.Warehouse_Id, Warehouse_Name = g.Key.Warehouse_Name, Qty = g.Sum(x => x.Qty) }).ToList();

//                var warehouse = db.Inventory_Warehouse.ToList();
//                foreach (var ware in warehouse)
//                {
//                    var stin = st_in.Where(x => x.Warehouse_Id == ware.Id).FirstOrDefault();
//                    var stou = st_out.Where(x => x.Warehouse_Id == ware.Id).FirstOrDefault();
//                    if (stin == null && stou == null)
//                    {
//                        continue;
//                    }

//                    WarehouseModel w = new WarehouseModel()
//                    {
//                        Total_Stock_In = (stin == null) ? 0 : stin.Total_In_Qty,
//                        Total_Stock_Out = (stou == null) ? 0 : stou.Total_Out_Qty,
//                        Warehouse_Id = (stin == null) ? null : stin.Warehouse_Id,
//                        Warehouse_Name = (stin == null) ? null : stin.WarehouseName
//                    };
//                    item.Warehouse_Rep.Add(w);
//                }
//            }
//            //foreach (var item in res)
//            //{
//            //    var res1 = db.Sp_Get_Inventory_Availability_Parameter(item.Item_Id).FirstOrDefault();
//            //    item.Total_Stock_In = res1.Total_In_Qty;
//            //    item.Total_Stock_Out = res1.Total_Out_Qty;
//            //}
//            Helpers H = new Helpers();
//            ViewBag.Group_Id = H.RandomNumber();
//            return PartialView(res);
//        }
//        public ActionResult DemandOrderDetails(string dem)
//        {
//            var res = db.SP_Get_DemandOrderItemDetails(dem).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Demand Order Details " + dem, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView(res);
//        }
//        public JsonResult CancelDemandOrder(long? Group_Id)
//        {
//            var res1 = db.Sp_Update_DemandOrder(Group_Id).ToString();
//            return Json(res1);
//        }
//        public JsonResult UpdateStockout(long Group_Id, long DemandOrder_Id, List<IssueReq> Items)
//        {
//            long Userid = User.Identity.GetUserId<long>();
//            var res1 = db.Inventory_Demand_Order.Where(x => x.Group_Id == DemandOrder_Id).FirstOrDefault();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Updated Stock Out for Demand order  " + DemandOrder_Id, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Group_Id);
//            Items.RemoveAll(x => x.Qty <= 0);
//            var lastdate = new DateTime(2021, 7, 1);
//            if (res1.To_Comp_Id != null)
//            {
//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var data = new XElement("Issue", Items.Select(x => new XElement("IssueItem",
//                        new XAttribute("Item_Id", x.Item_Id),
//                        new XAttribute("Qty", x.Qty),
//                        new XAttribute("Prj_Id", res1.Prj_Id),
//                        new XAttribute("Prj_Name", res1.Prj_Name),
//                        new XAttribute("Issue_To", res1.Issue_To),
//                        new XAttribute("IssueTo_Name", res1.IssueTo_Name),
//                        new XAttribute("Requested_By", res1.Requested_By),
//                        new XAttribute("RequestedBy_Name", res1.RequestedBy_Name),
//                        new XAttribute("Requested_Date", res1.RequestedBy_Date),
//                        new XAttribute("DemandOrder_Id", DemandOrder_Id),
//                        new XAttribute("Dep_Id", res1.Dep_Id),
//                        new XAttribute("Dep_Name", res1.Dep_Name),
//                        new XAttribute("Warehouse_Id", x.Warehouse_Id),
//                        new XAttribute("Warehouse_Name", x.Warehouse_Name)
//                        //new XAttribute("Issuer_Name", x.Issuer_Name)
//                        ))).ToString();
//                        var res = db.Sp_Update_Stockout_Handover(Group_Id, Userid, data).FirstOrDefault();

//                        if (res == "0")
//                        {
//                            var ret = new { Status = false, Msg = "Cannot Handover Stock Multiple time" };
//                            return Json(ret);
//                        }
//                        else
//                        {
//                            try
//                            {

//                                AccountHandlerController ah = new AccountHandlerController();
//                                var comp = ah.Company_Attr(userid);
//                                List<Sp_Get_COA_Head_Code_Result> itemHeads = new List<Sp_Get_COA_Head_Code_Result>();
//                                List<StockOutItemDetailsModel> itemDetails = new List<StockOutItemDetailsModel>();
//                                decimal? totalamt = 0;
//                                // Get Heads
//                                var prjhead = ah.Project_Head(res1.Prj_Id, comp.Id);
//                                //var to_comphead = ;
//                                //var to_Comphead


//                                foreach (var v in Items)
//                                {
//                                    var itemName = db.Inventories.Where(x => x.Id == v.Item_Id).Select(x => new { x.Complete_Name, x.Is_Asset }).FirstOrDefault();
//                                    if (itemName.Is_Asset == true)
//                                    {
//                                        continue;
//                                    }
//                                    StockOutItemDetailsModel det = new StockOutItemDetailsModel();
//                                    det.qty = v.Qty;
//                                    det.rate = db.Inventory_Stock_In.Where(x => x.Item_Id == v.Item_Id && x.DateTime.Value.Date >= lastdate.Date).Average(x => x.Rate);
//                                    totalamt += det.qty* det.rate ;
//                                    itemHeads.Add(ah.Inventory_Head(v.Item_Id, itemName.Complete_Name, comp.Id));
//                                    itemDetails.Add(det);
//                                }
//                                int i = 0;
//                                int j = 0;

//                                if (itemHeads.Any())
//                                {
//                                    var Journal = new XElement("JournalEntries", new XElement("Entries",
//                                new XAttribute("Naration", "Items Issued to " + res1.IssueTo_Name + " against Project: " + res1.Prj_Name + " Issue Note No. " + res),
//                                new XAttribute("Head", prjhead.Head),
//                                new XAttribute("Head_Name", prjhead.Head),
//                                new XAttribute("Head_Code", prjhead.Text_ChartCode),
//                                new XAttribute("Head_Id", prjhead.Id),
//                                new XAttribute("Line", ++j),
//                                new XAttribute("Qty", 0),
//                                new XAttribute("UOM", 0),
//                                new XAttribute("Rate", 0),
//                                new XAttribute("Debit", totalamt),
//                                new XAttribute("Credit", 0),
//                                new XAttribute("GroupId", Group_Id)
//                                ));
//                                    var VendorEnt = new XElement("JournalEntries", Items.Select(x => new XElement("Entries",
//                                    new XAttribute("Naration", itemHeads[i].Head + " Issued to " + res1.IssueTo_Name + " of Qty: " + itemDetails[i].qty + " against Project: " + res1.Prj_Name + ". Issue Note No. " + res),
//                                    new XAttribute("Head", itemHeads[i].Head),
//                                    new XAttribute("Head_Name", itemHeads[i].Head),
//                                    new XAttribute("Head_Code", itemHeads[i].Text_ChartCode),
//                                    new XAttribute("Head_Id", itemHeads[i].Id),
//                                    new XAttribute("Line", ++j),
//                                    new XAttribute("Qty", itemDetails[i].qty),
//                                    new XAttribute("UOM", ""),
//                                    new XAttribute("Rate", itemDetails[i].rate),
//                                    new XAttribute("Debit", 0),
//                                    new XAttribute("Credit", itemDetails[i].qty * itemDetails[i++].rate),
//                                    new XAttribute("GroupId", Group_Id)
//                                    )));

//                                    Journal.Add(
//                                                from ge in VendorEnt.Elements()
//                                                select ge
//                                                );
//                                    try
//                                    {
//                                        var res4 = db.Sp_Add_Journal_Entries(Journal.ToString(), userid).FirstOrDefault();
//                                        Transaction.Commit();
//                                        return Json(new Return { Status = true, Msg = "Saved" });
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                                        Transaction.Rollback();
//                                        throw;
//                                    }
//                                }
//                                else
//                                {
//                                    Transaction.Commit();
//                                    return Json(new Return { Status = true, Msg = "Saved" });
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                Transaction.Rollback();
//                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                            }

//                            Notifier.Notify(Userid, Convert.ToInt64(res1.Requested_By), NotifierMsg.Item_Issued, new object[] { res }, NotifierMsg.Item_Issued.ToString());
//                            var ret1 = new { Status = true, Msg = "Issue Note Generate : " + res };
//                            var itCurrDt = DateTime.Now;

//                            foreach (var it_ite in Items)
//                            {
//                                var rt = db.Sp_Get_InventoryItemLastRates(it_ite.Item_Id).Select(x => x.AVCO).FirstOrDefault() ?? 0;
//                                AddLedger(new GRNItems
//                                {
//                                    Expirey = null,
//                                    IsAsset = null,
//                                    Item_Id = it_ite.Item_Id,
//                                    Qty = it_ite.Qty,
//                                    Rate = rt
//                                }, Group_Id, res, itCurrDt);
//                            }
//                            return Json(ret1);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        throw;
//                    }
//                }
//            }
//            else
//            {
//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var data = new XElement("Issue", Items.Select(x => new XElement("IssueItem",
//                        new XAttribute("Item_Id", x.Item_Id),
//                        new XAttribute("Qty", x.Qty),
//                        new XAttribute("Prj_Id", res1.Prj_Id),
//                        new XAttribute("Prj_Name", res1.Prj_Name),
//                        new XAttribute("Issue_To", res1.Issue_To),
//                        new XAttribute("IssueTo_Name", res1.IssueTo_Name),
//                        new XAttribute("Requested_By", res1.Requested_By),
//                        new XAttribute("RequestedBy_Name", res1.RequestedBy_Name),
//                        new XAttribute("Requested_Date", res1.RequestedBy_Date),
//                        new XAttribute("DemandOrder_Id", DemandOrder_Id),
//                        new XAttribute("Dep_Id", res1.Dep_Id),
//                        new XAttribute("Dep_Name", res1.Dep_Name),
//                        new XAttribute("Warehouse_Id", x.Warehouse_Id),
//                        new XAttribute("Warehouse_Name", x.Warehouse_Name)
//                        //new XAttribute("Issuer_Name", x.Issuer_Name)
//                        ))).ToString();
//                        var res = db.Sp_Update_Stockout_Handover(Group_Id, Userid, data).FirstOrDefault();

//                        if (res == "0")
//                        {
//                            var ret = new { Status = false, Msg = "Cannot Handover Stock Multiple time" };
//                            return Json(ret);
//                        }
//                        else
//                        {
//                            try
//                            {

//                                AccountHandlerController ah = new AccountHandlerController();
//                                var comp = ah.Company_Attr(userid);
//                                List<Sp_Get_COA_Head_Code_Result> itemHeads = new List<Sp_Get_COA_Head_Code_Result>();
//                                List<StockOutItemDetailsModel> itemDetails = new List<StockOutItemDetailsModel>();
//                                decimal? totalamt = 0;
//                                var prjhead = ah.Project_Head(res1.Prj_Id, comp.Id);
//                                foreach (var v in Items.ToList())
//                                {
//                                    var itemName = db.Inventories.Where(x => x.Id == v.Item_Id).Select(x => new { x.Complete_Name, x.Is_Asset }).FirstOrDefault();
//                                    if (itemName.Is_Asset == true)
//                                    {
//                                        Items.Remove(v);
//                                        continue;
//                                    }
//                                    StockOutItemDetailsModel det = new StockOutItemDetailsModel();
//                                    det.qty = v.Qty;
//                                    det.rate = db.Inventory_Stock_In.Where(x => x.Item_Id == v.Item_Id && x.DateTime >= lastdate).Average(x => x.Rate);
//                                    if (det.rate == null)
//                                    {
//                                        det.rate = db.Inventory_Stock_In.Where(x => x.Item_Id == v.Item_Id).Average(x => x.Rate);
//                                    }
//                                    totalamt += det.qty*det.rate;
//                                    itemHeads.Add(ah.Inventory_Head(v.Item_Id, itemName.Complete_Name, comp.Id));
//                                    itemDetails.Add(det);
//                                }
//                                if (itemHeads.Any())
//                                {

//                                    int i = 0;
//                                    int j = 0;

//                                    var Journal = new XElement("JournalEntries", new XElement("Entries",
//                                    new XAttribute("Naration", "Items Issued to " + res1.IssueTo_Name + " against Project: " + res1.Prj_Name + " Issue Note No. " + res),
//                                    new XAttribute("Head", prjhead.Head),
//                                    new XAttribute("Head_Name", prjhead.Head),
//                                    new XAttribute("Head_Code", prjhead.Text_ChartCode),
//                                    new XAttribute("Head_Id", prjhead.Id),
//                                    new XAttribute("Line", ++j),
//                                    new XAttribute("Qty", 0),
//                                    new XAttribute("UOM", 0),
//                                    new XAttribute("Rate", 0),
//                                    new XAttribute("Debit", totalamt),
//                                    new XAttribute("Credit", 0),
//                                    new XAttribute("GroupId", Group_Id)
//                                    ));
//                                    var VendorEnt = new XElement("JournalEntries", Items.Select(x => new XElement("Entries",
//                                    new XAttribute("Naration", itemHeads[i].Head + " Issued to " + res1.IssueTo_Name + " of Qty: " + itemDetails[i].qty + " against Project: " + res1.Prj_Name + ". Issue Note No. " + res),
//                                    new XAttribute("Head", itemHeads[i].Head),
//                                    new XAttribute("Head_Name", itemHeads[i].Head),
//                                    new XAttribute("Head_Code", itemHeads[i].Text_ChartCode),
//                                    new XAttribute("Head_Id", itemHeads[i].Id),
//                                    new XAttribute("Line", ++j),
//                                    new XAttribute("Qty", itemDetails[i].qty),
//                                    new XAttribute("UOM", ""),
//                                    new XAttribute("Rate", itemDetails[i].rate),
//                                    new XAttribute("Debit", 0),
//                                    new XAttribute("Credit", itemDetails[i].qty * itemDetails[i++].rate),
//                                    new XAttribute("GroupId", Group_Id)
//                                    )));

//                                    Journal.Add(
//                                                from ge in VendorEnt.Elements()
//                                                select ge
//                                                );
//                                    try
//                                    {
//                                        var res4 = db.Sp_Add_Journal_Entries(Journal.ToString(), userid).FirstOrDefault();
//                                        Transaction.Commit();
//                                        return Json(new Return { Status = true, Msg = "Saved" });
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                                        Transaction.Rollback();
//                                        throw;
//                                    }
//                                }
//                                else
//                                {
//                                    Transaction.Commit();
//                                    return Json(new Return { Status = true, Msg = "Saved" });
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                Transaction.Rollback();
//                                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                            }

//                            Notifier.Notify(Userid, Convert.ToInt64(res1.Requested_By), NotifierMsg.Item_Issued, new object[] { res }, NotifierMsg.Item_Issued.ToString());
//                            var ret1 = new { Status = true, Msg = "Issue Note Generate : " + res };
//                            var itCurrDt = DateTime.Now;

//                            foreach (var it_ite in Items)
//                            {
//                                var rt = db.Sp_Get_InventoryItemLastRates(it_ite.Item_Id).Select(x => x.AVCO).FirstOrDefault() ?? 0;
//                                AddLedger(new GRNItems
//                                {
//                                    Expirey = null,
//                                    IsAsset = null,
//                                    Item_Id = it_ite.Item_Id,
//                                    Qty = it_ite.Qty,
//                                    Rate = rt
//                                }, Group_Id, res, itCurrDt);
//                            }
//                            return Json(ret1);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        throw;
//                    }
//                }
//            }
//        }
//        public ActionResult User_Demand_Orders()
//        {
//            return PartialView();
//        }
//        public ActionResult ManualStockIn()
//        {
//            ViewBag.group = new Helpers().RandomNumber();
//            ViewBag.vendors = new SelectList(db.Sp_Get_Vendor().ToList(), "Id", "Name");
//            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()).ToList(), "Id", "Name");
//            ViewBag.inv = new SelectList(db.SP_Get_Inventory_List_With_Properties().Select(x => new { x.Id, Name = x.NameWithProps }).ToList(), "Id", "Name");
//            ViewBag.Employees = new SelectList(db.Sp_Get_Employee().Where(x => x.Status == "Registered").ToList(), "Id", "Name", "Department", 1);
//            ViewBag.shelf = db.Warehouse_Shelf.ToList();
//            ViewBag.warehouse = db.Inventory_Warehouse.ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Manual Stock In Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View();
//        }
//        public JsonResult InventoryStockIn(List<Inventory_Stock_In> items, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var uid = long.Parse(User.Identity.GetUserId());
//                    items.RemoveAll(x => x.Qty <= 0);
//                    var itemsData = new XElement("InventoryProducts", items.Select(x => new XElement("InventoryItem",
//                        new XAttribute("itemId", x.Item_Id),
//                        new XAttribute("depId", x.Dep_Id),
//                        new XAttribute("depName", x.Dep_Name),
//                        (x.Prj is null) ? null : new XAttribute("PrjId", x.Prj),
//                        (x.Prj_Name is null) ? null : new XAttribute("PrjName", x.Prj_Name),
//                        (x.QualityCheck_By is null) ? null : new XAttribute("QualityCheck_By", x.QualityCheck_By),
//                        (x.QualityCheck_By_Name is null) ? null : new XAttribute("QualityCheck_By_Name", x.QualityCheck_By_Name),
//                        (x.Expire_Date is null) ? null : new XAttribute("expiry", x.Expire_Date),
//                        new XAttribute("groupId", x.Group_Id),
//                        (x.PO_Id is null) ? null : new XAttribute("poId", x.PO_Id),
//                        (x.PO_Number is null) ? null : new XAttribute("poNum", x.PO_Number),
//                        new XAttribute("qty", x.Qty),
//                        (x.Rate is null) ? null : new XAttribute("rate", x.Rate),
//                        (x.Remarks is null) ? null : new XAttribute("rema", x.Remarks),
//                        (x.GRN is null) ? null : new XAttribute("GRN", x.GRN),
//                        (x.Vendor_Id is null) ? null : new XAttribute("vendorId", x.Vendor_Id),
//                        (x.Vendor_Name is null) ? null : new XAttribute("vendorName", x.Vendor_Name),
//                        (x.IssueNoteNumber is null) ? null : new XAttribute("issNum", x.IssueNoteNumber),
//                        (x.Warehouse_Id is null) ? null : new XAttribute("Warehouse_Id", x.Warehouse_Id),
//                        (x.WarehouseName is null) ? null : new XAttribute("WarehouseName", x.WarehouseName),
//                        (x.InvoiceNo is null) ? null : new XAttribute("InvoiceNo", x.InvoiceNo),
//                         new XAttribute("Comp_Id", x.Comp_Id)
//                        ))).ToString();
//                    var res = db.SP_Add_Inventory_StockIn(itemsData, uid, TransactionId).ToList();
//                    int index = 0;
//                    foreach (string file in Request.Files)
//                    {
//                        HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
//                        index++;
//                        if (hpf.ContentLength == 0)
//                            continue;
//                        var imageName = Path.GetExtension(hpf.FileName);
//                        if (!Directory.Exists(Server.MapPath("~/Repository/Invoice/" + TransactionId)))
//                        {
//                            Directory.CreateDirectory(Server.MapPath("~/Repository/Invoice/" + TransactionId));
//                        }
//                        string savedFileName = Path.Combine(Server.MapPath("~/Repository/Invoice/" + TransactionId), hpf.FileName);
//                        hpf.SaveAs(savedFileName);
//                        Invoices_Files ii = new Invoices_Files()
//                        {
//                            File_Name = hpf.FileName,
//                            Invoice = Convert.ToInt64(TransactionId)
//                        };
//                        db.Invoices_Files.Add(ii);
//                        db.SaveChanges();
//                    }

//                    foreach (var g in items)
//                    {
//                        g.Id = (long)res.Where(x => x.item == g.Item_Id && x.vendor == g.Vendor_Id).Select(x => x.id).FirstOrDefault();
//                    }
//                    foreach (var invItem in items)
//                    {
//                        this.AdjustReceiving(invItem.Group_Id, invItem.Item_Id, invItem.Vendor_Id);
//                    }
//                    if (res.Count() == items.Count())
//                    {
//                        var mod = res.GroupBy(x => x.vendor).ToList();
//                        List<string> Grns = new List<string>();
//                        var myCurrDt = DateTime.Now;
//                        foreach (var vend in mod)
//                        {
//                            string gnrNo = "GRN-" + db.Sp_Get_ReceiptNo("Goods Receive Note").FirstOrDefault();
//                            foreach (var vendor in vend)
//                            {
//                                var venRes = db.SP_Add_Inventory_StockIn_GRN(vendor.id, gnrNo);
//                            }
//                            Grns.Add(gnrNo);
//                            if (vend.Key != null)
//                            {
//                                var itm_lst = items.Where(z => z.Vendor_Id == vend.Key.Value).ToList();
//                                foreach (var it_item in itm_lst)
//                                {
//                                    AddLedger(new GRNItems
//                                    {
//                                        Expirey = it_item.Expire_Date,
//                                        IsAsset = it_item.IsAsset,
//                                        Item_Id = it_item.Item_Id,
//                                        Qty = it_item.Qty,
//                                        Rate = it_item.Rate
//                                    }, it_item.Group_Id, gnrNo, myCurrDt);
//                                }
//                            }
//                        }
//                        try
//                        {
//                            AccountHandlerController ah = new AccountHandlerController();
//                            List<Sp_Get_COA_Head_Code_Result> itemHeads = new List<Sp_Get_COA_Head_Code_Result>();

//                            var comp = ah.Company_Attr(userid);

//                            var po_num = items.Select(x => x.PO_Number).FirstOrDefault();
//                            var PO = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == po_num ).ToList();
//                            var POVal = PO.Where(x => x.PO_Number == po_num && x.OtherCharges == null ).Sum(x => x.Purchase_Rate * x.Qty);
//                            var OtherCharges = PO.Where(x => x.PO_Number == po_num && x.OtherCharges == true).Sum(x => x.Purchase_Rate);

//                            {
//                                if (items.Any())
//                                {
//                                    foreach (var item in items)
//                                    {
//                                        itemHeads.Add(ah.Inventory_Head(item.Item_Id, item.Item_name, comp.Id));
//                                        item.Rate = item.Rate +  this.PropotionateRate(OtherCharges, item.Rate,POVal);
//                                        decimal? qty = (item.Qty == null) ? 0 : item.Qty;
//                                        item.PropotionateVal = item.Rate * qty;
//                                    }
//                                }
                                
//                                int i = 0;
//                                var pnsAccount = ah.PurchaseNeedSupervision_Head(comp.Id);

//                                var venname = items.Select(x => x.Vendor_Name).FirstOrDefault();
//                                var venid = items.Select(x => x.Vendor_Id).FirstOrDefault();
//                                var venHead = ah.Vendor_Head(venname, venid, comp.Id);

//                                //Journal Entry Debit
//                                string allitem = string.Join(",", items.Select(x => x.Item_name));
//                                var narration = "Payable Record Against PO No. " + po_num + " against purchase of  " + allitem + " for Vendor " + items.FirstOrDefault().Vendor_Name + " on " + Grns.FirstOrDefault();


//                                var JournalEnt = new XElement("JournalEntries", items.Select(x => new XElement("Entries",
//                                new XAttribute("Naration", narration),
//                                new XAttribute("Head", itemHeads[i].Head),
//                                new XAttribute("Head_Name", itemHeads[i].Head),
//                                new XAttribute("Head_Code", itemHeads[i].Text_ChartCode),
//                                new XAttribute("Head_Id", itemHeads[i].Id),
//                                new XAttribute("Line", ++i),
//                                new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
//                                new XAttribute("UOM", ""),
//                                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                                new XAttribute("Debit", x.PropotionateVal),
//                                new XAttribute("Credit", 0),
//                                new XAttribute("Comp_Id", comp.Id),
//                                new XAttribute("GroupId", TransactionId)
//                                )));
//                                // Journal Entry Credit
//                                var JEapp = new XElement("JournalEntries", new XElement("Entries",
//                                new XAttribute("Naration", narration),
//                                new XAttribute("Head", pnsAccount.Head),
//                                new XAttribute("Head_Name", pnsAccount.Head),
//                                new XAttribute("Head_Code", pnsAccount.Text_ChartCode),
//                                new XAttribute("Head_Id", pnsAccount.Id),
//                                new XAttribute("Line", items.Count() + 1),
//                                new XAttribute("Qty", 0),
//                                new XAttribute("UOM", ""),
//                                new XAttribute("Rate", 0),
//                                new XAttribute("Debit", 0),
//                                new XAttribute("Credit", items.Sum(x=> x.PropotionateVal)),
//                                new XAttribute("Comp_Id", comp.Id),
//                                new XAttribute("GroupId", TransactionId)
//                                ));
//                                JournalEnt.Add(from ge in JEapp.Elements() select ge);
//                                long tranNew = new Helpers().RandomNumber();
//                                // General Entry Debit
//                                var GeneralEnt = new XElement("GeneralEntries", new XElement("Entries",
//                                new XAttribute("Naration", narration),
//                                new XAttribute("Head", pnsAccount.Head),
//                                new XAttribute("Head_Name", pnsAccount.Head),
//                                new XAttribute("Head_Code", pnsAccount.Text_ChartCode),
//                                new XAttribute("Head_Id", pnsAccount.Id),
//                                new XAttribute("Line", 1),
//                                new XAttribute("Qty", 0),
//                                new XAttribute("UOM", ""),
//                                new XAttribute("Rate", 0),
//                                new XAttribute("Debit", items.Sum(x => x.PropotionateVal)),
//                                new XAttribute("Credit", 0),
//                                new XAttribute("GroupId", tranNew),
//                                new XAttribute("Comp_Id", comp.Id),
//                                new XAttribute("Ref", TransactionId)
//                                ));
//                                // General Entry Credit
//                                var GEapp = new XElement("GeneralEntries", new XElement("Entries",
//                                new XAttribute("Naration", narration),
//                                new XAttribute("Head", venHead.Head),
//                                new XAttribute("Head_Name", venHead.Head),
//                                new XAttribute("Head_Code", venHead.Text_ChartCode),
//                                new XAttribute("Head_Id", venHead.Id),
//                                new XAttribute("Line", 1),
//                                new XAttribute("Qty", 0),
//                                new XAttribute("UOM", ""),
//                                new XAttribute("Rate", 0),
//                                new XAttribute("Debit", 0),
//                                new XAttribute("Credit", items.Sum(x => x.PropotionateVal)),
//                                new XAttribute("GroupId", tranNew),
//                                new XAttribute("Comp_Id", comp.Id),
//                                new XAttribute("Ref", TransactionId)

//                                ));
//                                GeneralEnt.Add(from ge in GEapp.Elements() select ge);

//                                int j = 0;
//                                i = 0;
//                                string Bill = new XElement("Bills", items.Select(x => new XElement("Entries",
//                                new XAttribute("Vendor_Id", venHead.Id),
//                                new XAttribute("Terms", "30 Days"),
//                                new XAttribute("Bill_No", x.PO_Number + "-" + Grns.FirstOrDefault()),
//                                new XAttribute("Head_Code", itemHeads[j].Text_ChartCode),
//                                new XAttribute("Head_Name", itemHeads[j].Head),
//                                new XAttribute("Qty", (x.Qty == null) ? 0 : x.Qty),
//                                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                                new XAttribute("Line", ++i),
//                                new XAttribute("description", narration),
//                                new XAttribute("Head_Id", itemHeads[j].Id),
//                                new XAttribute("GroupId", tranNew),
//                                new XAttribute("PO_Group_Id", PO.FirstOrDefault().Group_Id),
//                                new XAttribute("Head", itemHeads[j++].Head),
//                                new XAttribute("Comp_Id", comp.Id)
//                                ))).ToString();


//                                var res6 = db.Sp_Add_Bills(Bill, userid, DateTime.Now, DateTime.Now.AddDays(30)).FirstOrDefault();
//                                var res4 = db.Sp_Add_General_Entries(GeneralEnt.ToString(), userid).FirstOrDefault();
//                                var res3 = db.Sp_Add_Journal_Entries(JournalEnt.ToString(), userid).FirstOrDefault();

//                                Transaction.Commit();
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Transaction.Rollback();
//                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                        }

//                        var actRes = db.Sp_Add_Activity(userid, "Added Items to Inventory ", "Create", "Activity_Record", ActivityType.Inventory.ToString(), TransactionId);
//                        return Json(new { Status = true, Msg = "Added to inventory successfully", GRNS = Grns });
//                    }
//                    else
//                    {
//                        List<string> Grns = new List<string>();
//                        Transaction.Rollback();
//                        return Json(new { Status = false, Msg = "Failed to add to the inventory.", GRNS = Grns });
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    var ra = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Failed to add to inventory" });
//                }
//            }
//        }
//        public decimal? PropotionateRate( decimal? OtherCharges, decimal? Rate, decimal? TotalPOVal)
//        {
//            var per = GeneralMethods.Percentage(Rate, TotalPOVal);
//            var pro = (OtherCharges * per) / 100;
//            return pro;
//        }
//        public JsonResult Manual_InventoryStockIn(List<Inventory_Stock_In> items, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            using (var Transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var uid = long.Parse(User.Identity.GetUserId());
//                    items.RemoveAll(x => x.Qty <= 0);
//                    var itemsData = new XElement("InventoryProducts", items.Select(x => new XElement("InventoryItem",
//                        new XAttribute("itemId", x.Item_Id),
//                        new XAttribute("depId", x.Dep_Id),
//                        new XAttribute("depName", x.Dep_Name),
//                        (x.Prj is null) ? null : new XAttribute("PrjId", x.Prj),
//                        (x.Prj_Name is null) ? null : new XAttribute("PrjName", x.Prj_Name),
//                        (x.QualityCheck_By is null) ? null : new XAttribute("QualityCheck_By", x.QualityCheck_By),
//                        (x.QualityCheck_By_Name is null) ? null : new XAttribute("QualityCheck_By_Name", x.QualityCheck_By_Name),
//                        (x.Expire_Date is null) ? null : new XAttribute("expiry", x.Expire_Date),
//                        new XAttribute("groupId", x.Group_Id),
//                        (x.PO_Id is null) ? null : new XAttribute("poId", x.PO_Id),
//                        (x.PO_Number is null) ? null : new XAttribute("poNum", x.PO_Number),
//                        new XAttribute("qty", x.Qty),
//                        (x.GRN is null) ? null : new XAttribute("GRN", x.GRN),
//                        (x.Rate is null) ? null : new XAttribute("rate", x.Rate),
//                        (x.Remarks is null) ? null : new XAttribute("rema", x.Remarks),
//                        (x.Vendor_Id is null) ? null : new XAttribute("vendorId", x.Vendor_Id),
//                        (x.Vendor_Name is null) ? null : new XAttribute("vendorName", x.Vendor_Name),
//                        (x.IssueNoteNumber is null) ? null : new XAttribute("issNum", x.IssueNoteNumber),
//                        (x.Warehouse_Id is null) ? null : new XAttribute("Warehouse_Id", x.Warehouse_Id),
//                        (x.WarehouseName is null) ? null : new XAttribute("WarehouseName", x.WarehouseName),
//                        (x.InvoiceNo is null) ? null : new XAttribute("InvoiceNo", x.InvoiceNo)
//                        ))).ToString();
//                    var res = db.SP_Add_Inventory_StockIn(itemsData, uid, TransactionId).ToList();
//                    //int index = 0;
//                    //foreach (string file in Request.Files)
//                    //{
//                    //    HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
//                    //    index++;
//                    //    if (hpf.ContentLength == 0)
//                    //        continue;
//                    //    var imageName = Path.GetExtension(hpf.FileName);
//                    //    if (!Directory.Exists(Server.MapPath("~/Repository/Invoice/" + TransactionId)))
//                    //    {
//                    //        Directory.CreateDirectory(Server.MapPath("~/Repository/Invoice/" + TransactionId));
//                    //    }
//                    //    string savedFileName = Path.Combine(Server.MapPath("~/Repository/Invoice/" + TransactionId), hpf.FileName);
//                    //    hpf.SaveAs(savedFileName);
//                    //    Invoices_Files ii = new Invoices_Files()
//                    //    {
//                    //        File_Name = hpf.FileName,
//                    //        Invoice = Convert.ToInt64(TransactionId)
//                    //    };
//                    //    db.Invoices_Files.Add(ii);
//                    //    db.SaveChanges();
//                    //}
//                    foreach (var g in items)
//                    {
//                        g.Id = (long)res.Where(x => x.item == g.Item_Id && x.vendor == g.Vendor_Id).Select(x => x.id).FirstOrDefault();
//                    }
//                    foreach (var invItem in items)
//                    {
//                        this.AdjustReceiving(invItem.Group_Id, invItem.Item_Id, invItem.Vendor_Id);
//                    }
//                    if (res.Count() == items.Count())
//                    {
//                        var mod = res.GroupBy(x => x.vendor).ToList();
//                        List<string> Grns = new List<string>();
//                        var myCurrDt = DateTime.Now;
//                        foreach (var vend in mod)
//                        {
//                            string gnrNo = "MI-" + db.Sp_Get_ReceiptNo("Manual Stock In").FirstOrDefault();
//                            foreach (var vendor in vend)
//                            {
//                                db.SP_Add_Inventory_StockIn_GRN(vendor.id, gnrNo);
//                            }
//                            Grns.Add(gnrNo);
//                            if (vend.Key != null)
//                            {
//                                var itm_lst = items.Where(z => z.Vendor_Id == vend.Key.Value).ToList();
//                                foreach (var it_item in itm_lst)
//                                {
//                                    AddLedger(new GRNItems
//                                    {
//                                        Expirey = it_item.Expire_Date,
//                                        IsAsset = it_item.IsAsset,
//                                        Item_Id = it_item.Item_Id,
//                                        Qty = it_item.Qty,
//                                        Rate = it_item.Rate
//                                    }, it_item.Group_Id, gnrNo, myCurrDt);
//                                }
//                            }
//                        }
//                        db.Sp_Add_Activity(userid, "Added Items to Inventory ", "Craete", "Activity_Record", ActivityType.Inventory.ToString(), TransactionId);
//                        Transaction.Commit();
//                        return Json(new { Status = true, Msg = "Added to inventory successfully", GRNS = Grns });
//                    }
//                    else
//                    {
//                        //var mod = res.GroupBy(x => x.vendor).ToList();
//                        List<string> Grns = new List<string>();
//                        //foreach (var vend in mod)
//                        //{
//                        //    string gnrNo = db.Sp_Get_ReceiptNo("Added to inventory successfully").FirstOrDefault();
//                        //    foreach (var vendor in vend)
//                        //    {
//                        //        db.SP_Add_Inventory_StockIn_GRN(vendor.item, gnrNo);
//                        //    }
//                        //    Grns.Add(gnrNo);
//                        //}
//                        Transaction.Rollback();
//                        return Json(new { Status = false, Msg = "Failed to add to the inventory.", GRNS = Grns });
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Transaction.Rollback();
//                    var ra = db.Sp_Add_ErrorLog(ex.Message + ex.InnerException, "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    return Json(new { Status = false, Msg = "Failed to add to inventory" });
//                }
//            }
//        }
//        public JsonResult InventoryDelivery(List<Inventory_Stock_In> items, long TransactionId)
//        {
//            try
//            {
//                var uid = long.Parse(User.Identity.GetUserId());

//                string dn = "DN-" + db.Sp_Get_ReceiptNo("Delivery Note").FirstOrDefault();
//                var itemsData = new XElement("InventoryProducts", items.Select(x => new XElement("InventoryItem",
//                    new XAttribute("itemId", x.Item_Id),
//                    new XAttribute("depId", x.Dep_Id),
//                    new XAttribute("depName", x.Dep_Name),
//                    new XAttribute("PrjId", x.Prj),
//                    new XAttribute("PrjName", x.Prj_Name),
//                    new XAttribute("GRN", dn),
//                    (x.QualityCheck_By is null) ? null : new XAttribute("QualityCheck_By", x.QualityCheck_By),
//                    (x.QualityCheck_By_Name is null) ? null : new XAttribute("QualityCheck_By_Name", x.QualityCheck_By_Name),
//                    (x.Expire_Date is null) ? null : new XAttribute("expiry", x.Expire_Date),
//                    new XAttribute("groupId", x.Group_Id),
//                    (x.PO_Id is null) ? null : new XAttribute("poId", x.PO_Id),
//                    (x.PO_Number is null) ? null : new XAttribute("poNum", x.PO_Number),
//                    new XAttribute("qty", x.Qty),
//                    (x.Rate is null) ? null : new XAttribute("rate", x.Rate),
//                    (x.Remarks is null) ? null : new XAttribute("rema", x.Remarks),
//                    (x.Vendor_Id is null) ? null : new XAttribute("vendorId", x.Vendor_Id),
//                    (x.Vendor_Name is null) ? null : new XAttribute("vendorName", x.Vendor_Name)))).ToString();
//                var res = db.SP_Add_Inventory_StockIn(itemsData, uid, TransactionId).ToList();

//                var data = new XElement("Issue", items.Select(x => new XElement("IssueItem",
//                       new XAttribute("Item_Id", x.Item_Id),
//                       new XAttribute("Qty", x.Qty),
//                       new XAttribute("Prj_Id", x.Prj),
//                       new XAttribute("Prj_Name", x.Prj_Name),
//                       new XAttribute("PO_Id", x.PO_Id),
//                       new XAttribute("PO_Number", x.PO_Number),
//                       new XAttribute("IssueNo", dn),
//                       new XAttribute("DemandOrder_Id", TransactionId),
//                       new XAttribute("Group_Id", x.Group_Id),
//                       new XAttribute("Dep_Id", x.Dep_Id),
//                       new XAttribute("Dep_Name", x.Dep_Name)
//                       ))).ToString();
//                var res1 = db.Sp_Update_Stockout_DirectIssuance(TransactionId, uid, data).FirstOrDefault();
//                foreach (var g in items)
//                {
//                    g.Id = (long)res.Where(x => x.item == g.Item_Id && x.vendor == g.Vendor_Id).Select(x => x.id).FirstOrDefault();
//                }

//                if (res.Count() == items.Count())
//                {
//                    var mod = res.GroupBy(x => x.vendor).ToList();
//                    List<string> Dns = new List<string>();
//                    foreach (var vend in mod)
//                    {
//                        foreach (var vendor in vend)
//                        {
//                            db.SP_Add_Inventory_StockIn_GRN(vendor.id, dn);
//                        }
//                        Dns.Add(dn);
//                    }
//                    try
//                    {
//                        AccountHandlerController de = new AccountHandlerController();
//                        de.DNEntries(items, TransactionId, uid, Dns);
//                    }
//                    catch (Exception ex)
//                    {
//                        var ra = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    }
//                    return Json(new { Status = true, Msg = "Added to inventory successfully", GRNS = Dns });
//                }
//                else
//                {
//                    var mod = res.GroupBy(x => x.vendor).ToList();
//                    List<string> dns = new List<string>();
//                    foreach (var vend in mod)
//                    {
//                        foreach (var vendor in vend)
//                        {
//                            db.SP_Add_Inventory_StockIn_GRN(vendor.item, dn);
//                        }
//                        dns.Add(dn);
//                    }
//                    return Json(new { Status = false, Msg = "Added to inventory successfully", GRNS = dns });
//                }
//            }
//            catch (Exception ex)
//            {
//                //EmailService e = new EmailService();
//                //e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Inventory Error");
//                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
//                return Json(new { Status = false, Msg = "Failed to add to inventory" });
//            }
//        }
//        public ActionResult StockInAutoView(string po)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            ViewBag.shelf = db.Warehouse_Shelf.ToList();
//            ViewBag.warehouse = db.Inventory_Warehouse.ToList();

//            var res = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == po && x.OtherCharges == null).ToList();
//            res.ForEach(x => x.SKU = this.ItemCode(x.Item_Id));
//            res.ForEach(x => x.ReceivedQuantity = db.SP_Get_ReceivedQuantity_AgainstPO(x.Id).FirstOrDefault());
//            var ids = new XElement("Deps", res.Select(x => new XElement("Ids",
//             new XAttribute("Id", x.Dep_Id)
//             ))).ToString();
//            var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).ToList();

//            Helpers H = new Helpers();
//            ViewBag.TransactionId = H.RandomNumber();

//            ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);
//            return View(res);
//        }
//        public ActionResult InventoryReport()
//        {
//            ViewBag.Prj_Id = new SelectList(db.Projects.ToList(), "Id", "Name");
//            ViewBag.Vendor = new SelectList(db.Vendors.ToList(), "Id", "Name");
//            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Demand Orders")).ToList();
//            ViewBag.Users = new SelectList(All, "id", "Name");
//            ViewBag.Inv = new SelectList(db.SP_Get_Inventory_List_With_Properties().Select(x => new { x.Id, Name = x.NameWithProps }).ToList(), "Id", "Name");
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Inventory Report ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return View();
//        }
//        public ActionResult SearchInventoryReport(DateTime? From, DateTime? To, long?[] Prj_Id, long?[] Users, long?[] Vendor, long?[] Inv, int In_Out)
//        {
//            string prj = "", inv = "", vend = "", use = "";
//            if (Prj_Id != null)
//            {
//                prj = new XElement("Prjs", Prj_Id.Select(x => new XElement("Prj_Id",
//                                       (x == null) ? null : new XAttribute("Ids", x)
//                                ))).ToString();
//            }
//            if (Vendor != null)
//            {
//                vend = new XElement("Vendor", Vendor.Select(x => new XElement("Ven_Id",
//                (x == null) ? null : new XAttribute("Ids", x)
//                                 ))).ToString();
//            }
//            if (Inv != null)
//            {
//                inv = new XElement("Inventory", Inv.Select(x => new XElement("Inv_Id",
//                (x == null) ? null : new XAttribute("Ids", x)
//                                 ))).ToString();
//            }
//            if (Users != null)
//            {
//                use = new XElement("Users", Users.Select(x => new XElement("User_Id",
//                (x == null) ? null : new XAttribute("Ids", x)
//                                 ))).ToString();
//            }
//            var res = db.Sp_Get_Inventory_Report(From, To, prj, vend, inv, use, In_Out).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Item_In_Out_Details(DateTime? From, DateTime? To, long?[] Prj_Id, long?[] Users, long?[] Vendor, long?[] Inv, int In_Out)
//        {
//            string prj = "", inv = "", vend = "", use = "";
//            if (Prj_Id != null)
//            {
//                prj = new XElement("Prjs", Prj_Id.Select(x => new XElement("Prj_Id",
//                                       (x == null) ? null : new XAttribute("Ids", x)
//                                ))).ToString();
//            }
//            if (Vendor != null)
//            {
//                vend = new XElement("Vendor", Vendor.Select(x => new XElement("Ven_Id",
//                (x == null) ? null : new XAttribute("Ids", x)
//                                 ))).ToString();
//            }
//            if (Inv != null)
//            {
//                inv = new XElement("Inventory", Inv.Select(x => new XElement("Inv_Id",
//                (x == null) ? null : new XAttribute("Ids", x)
//                                 ))).ToString();
//            }
//            if (Users != null)
//            {
//                use = new XElement("Users", Users.Select(x => new XElement("User_Id",
//                (x == null) ? null : new XAttribute("Ids", x)
//                                 ))).ToString();
//            }
//            var res = db.Sp_Get_Inventory_Report(From, To, prj, vend, inv, use, In_Out).ToList();
//            return PartialView(res);
//        }

//        [HttpGet]
//        public JsonResult GetInventoryItem(string q)
//        {
//            var res = db.Inventories.Where(x => x.Complete_Name.Contains(q) || x.SKU.Contains(q)).Select(x => new { id = x.Id, text = x.Complete_Name }).Take(20).ToList();
//            return Json(new { items = res }, JsonRequestBehavior.AllowGet);
//        }
//        [HttpGet]
//        public JsonResult GetInventoryItem_WC(string q) // Get Items with Code
//        {
//            var res = db.Inventories.Where(x => (x.Complete_Name.Contains(q) || x.SKU.Contains(q)) && x.SKU != null).Select(x => new { id = x.Id, text = x.Complete_Name }).Take(20).ToList();
//            return Json(new { items = res }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult UpdateQualityStatus(long? po)
//        {
//            if (!(po is null))
//            {
//                db.SP_Update_Quality_Check(po);
//                return Json(true);
//            }
//            return Json(false);
//        }
//        public ActionResult BidsListing(long Group)
//        {
//            var PR = db.Inventory_Purchase_Req.Where(x => x.Group_Id == Group).ToList();
//            PR.ForEach(x => x.SKU = this.ItemCode(x.Item_Id));
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Bid Listings", "Read", "Activity_Record", ActivityType.Inventory.ToString(), Group);
//            ViewBag.PurchaseReqId = PR;
//            var res = db.SP_GET_Applied_Quotes_For_PR_Group(Group).ToList();
//            return View(new QuotationsViewModel { AppliedBids = res, PurchaseRequistionDetails = PR });
//        }
//        public JsonResult DeleteBid(long bid)
//        {
//            try
//            {
//                db.Sp_Delete_Bid(bid, "");
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Deleted Bid ", "Delete", "Activity_Record", ActivityType.Inventory.ToString(), bid);
//                return Json(true);
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Remove Quotation Error");
//                return Json(false);
//            }
//        }
//        public ActionResult InventoryReturn()
//        {
//            ViewBag.Project = new SelectList(db.Projects.ToList(), "Id", "Name");
//            return View();
//        }
//        public ActionResult InventoryReturnDetail(string issueNum)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Inventory Return Page " + issueNum, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            var res2 = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => (long)x.Id).ToList();
//            string Ids = new XElement("Dep", res2.Select(x => new XElement("DepId",
//                                 new XAttribute("Id", x)
//                                 ))).ToString();

//            ViewBag.Employees = new SelectList(db.Sp_Get_Employee().Where(x => x.Status == "Registered").ToList(), "Id", "Name", "Department", 1);
//            var res = db.SP_Get_IssueNote_Items(issueNum).ToList();
//            Helpers H = new Helpers();
//            ViewBag.TransactionId = H.RandomNumber();

//            return PartialView(res);
//        }
//        public ActionResult GeneratedPurchaseOrders()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Generated PO's ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }
//        public JsonResult ReturnItems(ReturnItemsModel[] item, long TransactionId)
//        {

//            var items = item.Where(x => x.qtyRet > 0).ToList();
//            foreach (var v in items)
//            {
//                if (db.Inventory_Stock_In.Any(x => x.IssueNoteNumber == v.issueNum && x.Item_Id == v.item))
//                {
//                    var sum = db.Inventory_Stock_In.Where(x => x.IssueNoteNumber == v.issueNum && x.Item_Id == v.item).ToList();
//                    if (sum.Sum(x => x.Qty) + v.qtyRet > v.rqstdQty)
//                    {
//                        return Json(new
//                        {
//                            Status = false,
//                            Msg = "Failed to return these items. Items are already received in GRN : " + sum.Select(x => x.GRN).FirstOrDefault() + " on Date : " + string.Format("{0:dd-MMM-yyyy}", sum.Select(x => x.DateTime).FirstOrDefault()),
//                            GRN = ""
//                        });
//                    }
//                }
//            }
//            try
//            {
//                var uid = long.Parse(User.Identity.GetUserId());
//                var itemsData = new XElement("InventoryProducts", items.Where(x => x.qtyRet > 0).Select(x => new XElement("InventoryItem",
//                     new XAttribute("itemId", x.item),
//                     new XAttribute("depId", x.Dep_Id),
//                     new XAttribute("depName", x.Dep_Name),
//                     new XAttribute("PrjId", x.Prj_Id),
//                     new XAttribute("PrjName", x.Prj_Name),
//                     null,
//                     null,
//                     new XAttribute("issId", x.issueId),
//                     new XAttribute("issNum", x.issueNum),
//                     new XAttribute("qty", x.qtyRet),
//                     new XAttribute("rate", 0),
//                     null,
//                     null,
//                     null,
//                     new XAttribute("GRN", ""),
//                     (x.Warehouse_Id == null) ? null : new XAttribute("Warehouse_Id", x.Warehouse_Id),
//                     (x.Warehouse_Name == null) ? null : new XAttribute("WarehouseName", x.Warehouse_Name)))).ToString();
//                var res = db.SP_Add_Inventory_StockIn(itemsData, uid, TransactionId).ToList();
//                if (res.Count() == items.Count())
//                {
//                    var mod = res.GroupBy(x => x.vendor).ToList();
//                    List<string> Grns = new List<string>();
//                    foreach (var vend in mod)
//                    {
//                        string gnrNo = "RN-" + db.Sp_Get_ReceiptNo("Return Note").FirstOrDefault();
//                        foreach (var vendor in vend)
//                        {
//                            db.SP_Add_Inventory_StockIn_GRN(vendor.id, gnrNo);
//                        }
//                        Grns.Add(gnrNo);
//                    }
//                    long userid = long.Parse(User.Identity.GetUserId());
//                    db.Sp_Add_Activity(userid, "Returned Items to inventory", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), TransactionId);
//                    return Json(new { Status = true, Msg = "Added to inventory successfully", GRNS = Grns });
//                }
//                else
//                {
//                    var mod = res.GroupBy(x => x.vendor).ToList();
//                    List<string> Grns = new List<string>();
//                    foreach (var vend in mod)
//                    {
//                        string gnrNo = "RN-" + db.Sp_Get_ReceiptNo("Return Note").FirstOrDefault();
//                        foreach (var vendor in vend)
//                        {
//                            db.SP_Add_Inventory_StockIn_GRN(vendor.item, gnrNo);
//                        }
//                        Grns.Add(gnrNo);
//                    }
//                    return Json(new { Status = false, Msg = "Failed to add to inventory", GRNS = Grns });
//                }
//            }
//            catch (Exception ex)
//            {
//                EmailService e = new EmailService();
//                e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Inventory Error");
//                return Json(new { Status = false, Msg = "Failed to add to inventory" });
//            }
//        }
//        public JsonResult GetInventory_DepWise_AutoComp(string search)
//        {
//            var userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Dep = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).ToList();
//                string Ids = new XElement("Dep", Dep.Select(x => new XElement("DepId",
//                                     new XAttribute("Id", x.Department_Id)
//                                     ))).ToString();
//                var res = db.Sp_Get_InventoryList_Dep(Ids).Where(x => x.Complete_Name.ToLowerInvariant().Contains(search)).ToList();
//                return Json(res, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Dep = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).ToList();
//                string Ids = new XElement("Dep", Dep.Select(x => new XElement("DepId",
//                                     new XAttribute("Id", x.Department_Id)
//                                     ))).ToString();
//                var res = db.Sp_Get_InventoryList_Dep(Ids).Where(x => x.Complete_Name.ToLowerInvariant().Contains(search)).ToList();
//                return Json(res, JsonRequestBehavior.AllowGet);
//            }

//        }
//        public ActionResult StockTaking()
//        {
//            var Dep = db.Inventory_Stock_In.Select(x => new LeavesTypes { Value = x.Dep_Id, Name = x.Dep_Name }).Distinct().ToList();
//            return View(Dep);
//        }
//        public ActionResult List(long?[] Dep)
//        {
//            //var Dep = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => x.Id).ToList();
//            string Ids = new XElement("Dep", Dep.Select(x => new XElement("DepId",
//                                    new XAttribute("Id", x)
//                                    ))).ToString();
//            var res = db.Sp_Get_InventoryList_Dep(Ids).
//                      Select(x => new InventoryTemplate
//                      {
//                          Id = x.Id,
//                          Dep = x.Dep_Name,
//                          Item_Code = x.SKU,
//                          Dep_Id = x.Dep_Id,
//                          Item_Name = x.Complete_Name,
//                          Qty = x.Remaining,
//                          Warehouse_Name = x.WarehouseName,
//                          Ware_Id = x.Warehouse_Id

//                      }).ToList();
//            return PartialView(res);
//        }
//        public JsonResult UpdateInv(long Id, decimal? Qty, decimal? Pre_Qty, long DepId, string Dep_Nam, long Ware_Id, string Ware_Name)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (Pre_Qty != Qty)
//            {
//                Helpers h = new Helpers();
//                var res = db.Sp_Update_StockTaking(Id, Qty, Pre_Qty, userid, h.RandomNumber(), DepId, Dep_Nam, Ware_Id, Ware_Name);
//                return Json(true);
//            }
//            else
//            {
//                return Json(false);
//            }
//        }
//        public JsonResult GetInventoryItemById(long Id, int? Dep_Id)
//        {
//            if (User.IsInRole("Administrator"))
//            {
//                var userid = User.Identity.GetUserId<long>();
//                var invDep = db.Inventory_Department.Where(x => x.Item_Id == Id).ToList();
//                //var res1 = db.Sp_Get_Inventory_Availability_Parameter(Id).FirstOrDefault();
//                var r1 = db.Sp_Get_Inventory_TotalStock_In(Id, null, true).FirstOrDefault();
//                var r2 = db.Sp_Get_Inventory_TotalStock_Out(Id, null, true).FirstOrDefault();
//                r1.Total_Out_Qty = r2.Total_Out_Qty;

//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Dep = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).ToList();
//                var fildeps = invDep.Select(x => x.Dep_Id);

//                var res = new { InvDeps = invDep, Inventory = r1, Status = Dep.Any(x => fildeps.Contains((int)x.Department_Id)) };
//                return Json(res);
//            }
//            else
//            {
//                var userid = User.Identity.GetUserId<long>();
//                var invDep = db.Inventory_Department.Where(x => x.Item_Id == Id).ToList();

//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Dep = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).ToList();
//                var fildeps1 = invDep.Select(x => x.Dep_Id).ToList();

//                var dep = new XElement("Deps",  new XElement("Id",
//                new XAttribute("Dep", Dep_Id))).ToString();

//                //var res = db.Sp_Get_Inventory_Availability_Dep_Parameter(Id, dep).ToList();
//                var r1 = db.Sp_Get_Inventory_TotalStock_In(Id, dep, false).FirstOrDefault();
//                var r2 = db.Sp_Get_Inventory_TotalStock_Out(Id, dep, false).FirstOrDefault();

//                r1.Total_Out_Qty = r2.Total_Out_Qty;

//                var res2 = new
//                {
//                    InvDeps = invDep,
//                    Inventory = r1,
//                    Status = Dep.Any(x => fildeps1.Contains((long)x.Department_Id))
//                };
//                return Json(res2);
//            }
//        }
//        public ActionResult ReceiveItems()
//        {
//            return View();
//        }
//        public ActionResult PendingPO()
//        {

//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Pending PO ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            //var res1 = db.Inventory_PurchaseOrder.Where(x => x.StockEntered == false && (x.Prj_Offsite == null || x.Prj_Offsite == false)).ToList();

//            var res3 = db.Sp_Get_Pending_purchaseOrder().ToList();

//            var xml1 = new XElement("GroupId", res3.Select(x => new XElement("Id",
//                   new XAttribute("Group", x.Group_Id)))).ToString();

//            var res2 = db.Sp_Get_PO_Completed_GRN(null, null, xml1).ToList();

//            var res = new PendingPurchaseorder { PO = res3, GRN = res2 };
//            return PartialView(res);
//        }
//        public ActionResult PartialPO()
//        {

//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Partial PO's ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.StockEntered == false && (x.Prj_Offsite == null || x.Prj_Offsite == false)).ToList();

//            var xml1 = new XElement("GroupId", res1.Select(x => new XElement("Id",
//                   new XAttribute("Group", x.Group_Id)))).ToString();

//            var res2 = db.Sp_Get_PO_Completed_GRN(null, null, xml1).ToList();

//            var res = new POStatuses { PO = res1, GRN = res2 };
//            return PartialView(res);
//        }

//        //// Demand Requisitions

//        public ActionResult DemandRequisition()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var Deps = db.Comp_Dep_Desig.Where(x => x.Type == "Department").Select(x => new { Id = x.Id, Name = x.Name }).Distinct().ToList();
//            ViewBag.Department = new SelectList(Deps, "Id", "Name");
//            Helpers h = new Helpers();
//            ViewBag.Group_Id = h.RandomNumber();
//            db.Sp_Add_Activity(userid, "Accessed Demand Requisition page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            return PartialView();
//        }
//        public JsonResult AddDemandReq(List<Inventory_Demand_Req> items, long? Group_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            var itemsData = new XElement("InventoryProducts", items.Select(x => new XElement("InventoryItem",
//                new XAttribute("Item_Name", x.Item_Name),
//                new XAttribute("ReqTo_Dep", x.ReqTo_Dep),
//                new XAttribute("Details", x.Details),
//                new XAttribute("Qty", x.Qty),
//                new XAttribute("Dep_Id", x.Dep_Id),
//                new XAttribute("Dep_Name", x.Dep_Name),
//                new XAttribute("ReqTo_Dep_Name", x.ReqTo_Dep_Name)
//                ))).ToString();
//            var res1 = db.Sp_Add_DemandReq(itemsData, userid, Group_Id);

//            // Notify
//            var Rep_Emp = db.Sp_Get_Employee_UserId(userid).FirstOrDefault().Reporting_To;
//            var Rep_Emp_Det = db.Sp_Get_Employee_Parameter(Rep_Emp).FirstOrDefault();

//            if (Rep_Emp_Det.loginId != null)
//            {
//                Notifier.Notify(userid, Convert.ToInt64(Rep_Emp_Det.loginId), NotifierMsg.Demand_Requisitions, new object[] { items.FirstOrDefault() }, NotifierMsg.Demand_Requisitions.ToString());
//            }
//            db.Sp_Add_Activity(userid, "Added Demand requisition", "Create", "Activity_Record", ActivityType.Inventory.ToString(), Group_Id);
//            var res = new { Msg = "Requested", Status = true };
//            return Json(res);
//        }
//        public ActionResult DemandReqList(long? Prj_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Demand Order List ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Prj_Id);
//            if (User.IsInRole("Administrator"))
//            {
//                var res = db.Inventory_Demand_Req.Where(x => x.Prj_Id == Prj_Id).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                List<Inventory_Demand_Req> res = new List<Inventory_Demand_Req>();
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Users = db.Sp_Get_Reporting_Employees(EmpId).Select(x => x.loginid).ToList();
//                var res1 = db.Inventory_Demand_Req.Where(x => Users.Contains(x.ReqBy) && x.Prj_Id == Prj_Id).ToList();
//                res1.ForEach(x => x.Type = "Requisition");
//                res.AddRange(res1);

//                var res2 = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long?)x.Department_Id).ToList();
//                var res3 = db.Inventory_Demand_Req.Where(x => x.ApprovedBy != null && res2.Contains(x.ReqTo_Dep) && x.Prj_Id == Prj_Id).ToList();
//                res3.ForEach(x => x.Type = "Approved");
//                res.AddRange(res3);
//                return PartialView(res);
//            }
//        }
//        public JsonResult DeleteDemandReq(long Group_Id)
//        {
//            var res = db.Sp_Delete_DemandReq(Group_Id);
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted Demand Req.", "Delete", "Activity_Record", ActivityType.Inventory.ToString(), Group_Id);
//            return Json(true);
//        }
//        public ActionResult DemandReq()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Demand Req. Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
//            if (User.IsInRole("Administrator"))
//            {
//                var res = db.Inventory_Demand_Req.Where(x => x.ReqBy == userid).ToList();
//                return PartialView(res);
//            }
//            else
//            {
//                List<Inventory_Demand_Req> res = new List<Inventory_Demand_Req>();
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Users = db.Sp_Get_Reporting_Employees(EmpId).Select(x => x.loginid).ToList();
//                var res1 = db.Inventory_Demand_Req.Where(x => Users.Contains(x.ReqBy)).ToList();
//                res1.ForEach(x => x.Type = "Requisition");
//                res.AddRange(res1);

//                var res2 = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long?)x.Department_Id).ToList();
//                var res3 = db.Inventory_Demand_Req.Where(x => x.ApprovedBy != null && res2.Contains(x.ReqTo_Dep)).ToList();
//                res3.ForEach(x => x.Type = "Approved");
//                res.AddRange(res3);
//                return PartialView(res);
//            }
//        }
//        public void adjuststock()
//        {
//            var res2 = db.Inventory_PurchaseOrder.Where(x => x.StockEntered == false).ToList();
//            foreach (var invItem in res2)
//            {
//                this.AdjustReceiving(invItem.Group_Id, invItem.Item_Id, invItem.Vendor_Id);
//            }
//        }
//        public void AdjustReceiving(long? Group_Id, long? Item_Id, long? Vendor_Id)
//        {
//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.Item_Id == Item_Id && x.Vendor_Id == Vendor_Id).ToList();
//            var res2 = db.Inventory_Stock_In.Where(x => x.Group_Id == Group_Id && x.Item_Id == Item_Id && x.Vendor_Id == Vendor_Id).Sum(x => x.Qty);
//            if (res1.Sum(x => x.Qty) == res2)
//            {
//                var re = db.Sp_Update_Inventory_PO_Delivery(Vendor_Id, Group_Id, Item_Id);
//            }
//            if (res1.Any(x => x.OtherCharges == true))
//            {
//                foreach (var item in res1)
//                {
//                    var re = db.Sp_Update_Inventory_PO_Delivery(Vendor_Id, Group_Id, item.Item_Id);
//                }
//            }
//        }
//        public ActionResult Prj_IssuedInventory(long? Prj_Id)
//        {
//            var res = db.Sp_Get_Stockout_Prj(Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Received_Items()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Recived Items  Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return PartialView();
//        }
//        public ActionResult CompletedPo()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Completed PO Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return PartialView();
//        }
//        public JsonResult DeleteItem(long Id, string Module)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Deleted Item " + Module, "Delete", "Activity_Record", ActivityType.Inventory.ToString(), Id);
//            var res = db.Sp_Delete_RequestedItem(Id, Module);
//            return Json(true);
//        }
//        public ActionResult DirectItems()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Direct Items Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View();
//        }
//        public ActionResult Pending_DirectItems()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Pending Direct Items Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.Dep_Id == 16 && (x.Prj_Offsite == null || x.Prj_Offsite == false)).ToList();

//            var xml1 = new XElement("GroupId", res1.Select(x => new XElement("Id",
//                   new XAttribute("Group", x.Group_Id)))).ToString();

//            var res2 = db.Sp_Get_PO_Completed_GRN(null, null, xml1).ToList();

//            var res = new POStatuses { PO = res1, GRN = res2 };
//            return PartialView(res);
//        }
//        public ActionResult Partial_DirectItems()
//        {
//            long userid = User.Identity.GetUserId<long>();

//            db.Sp_Add_Activity(userid, "Accessed Partial Direct Items Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.Dep_Id == 16 && (x.Prj_Offsite == null || x.Prj_Offsite == false)).ToList();

//            var xml1 = new XElement("GroupId", res1.Select(x => new XElement("Id",
//                   new XAttribute("Group", x.Group_Id)))).ToString();

//            var res2 = db.Sp_Get_PO_Completed_GRN(null, null, xml1).ToList();

//            var res = new POStatuses { PO = res1, GRN = res2 };
//            return PartialView(res);
//        }
//        public ActionResult Completed_DirectItems()
//        {
//            long userid = User.Identity.GetUserId<long>();

//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.Dep_Id == 16 && (x.Prj_Offsite == null || x.Prj_Offsite == false)).ToList();

//            var xml1 = new XElement("GroupId", res1.Select(x => new XElement("Id",
//                   new XAttribute("Group", x.Group_Id)))).ToString();

//            var res2 = db.Sp_Get_PO_Completed_GRN(null, null, xml1).ToList();

//            var res = new POStatuses { PO = res1, GRN = res2 };
//            return PartialView(res);
//        }
//        public ActionResult DirectIssuanceDetails(string PO, long GroupId)
//        {
//            ViewBag.shelf = db.Warehouse_Shelf.ToList();
//            ViewBag.warehouse = db.Inventory_Warehouse.ToList();
//            long userid = long.Parse(User.Identity.GetUserId());

//            db.Sp_Add_Activity(userid, "Accessed Direct Issuance Details of PO " + PO, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), GroupId);
//            var res = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == PO && x.Group_Id == GroupId).ToList();

//            res.ForEach(x => x.ReceivedQuantity = db.SP_Get_ReceivedQuantity_AgainstPO(x.Id).FirstOrDefault());
//            res.ForEach(x => x.IssuedQuantity = db.Sp_Get_IssuedQuantity_AgainstPO(x.PO_Number, x.Item_Id).FirstOrDefault());


//            var ids = new XElement("Deps", res.Select(x => new XElement("Ids",
//             new XAttribute("Id", x.Dep_Id)
//             ))).ToString();
//            var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).ToList();

//            Helpers H = new Helpers();
//            ViewBag.TransactionId = H.RandomNumber();

//            ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);
//            return View(res);
//        }
//        public ActionResult Prj_Inv_Issuance(long GroupId, string PO)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == PO && x.Group_Id == GroupId).ToList();
//            db.Sp_Add_Activity(userid, "Accessed Details of PO " + PO, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), GroupId);

//            res.ForEach(x => x.ReceivedQuantity = db.SP_Get_ReceivedQuantity_AgainstPO(x.Id).FirstOrDefault());
//            return PartialView(res);
//        }
//        public JsonResult DirectIssue(long Group_Id, long Transaction_Id, string PO_Number, List<IssueReq> Items)
//        {
//            long Userid = User.Identity.GetUserId<long>();
//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.PO_Number == PO_Number).ToList();
//            var res2 = (from x in db.Inventory_Stock_In
//                        where x.PO_Number == PO_Number
//                        select new { x.Warehouse_Id, x.WarehouseName }).FirstOrDefault();
//            Items.RemoveAll(x => x.Qty <= 0);

//            //using (var Transaction = db.Database.BeginTransaction())
//            //{
//            //bool flag = false;

//            List<string> dinums = new List<string>();
//            foreach (var prj in Items.GroupBy(x => x.Prj_Id))
//            {
//                string issueNotNum = "DI-" + db.Sp_Get_ReceiptNo("Direct Issuance").FirstOrDefault();
//                var data = new XElement("Issue", prj.Select(x => new XElement("IssueItem",
//                   new XAttribute("Item_Id", x.Item_Id),
//                   new XAttribute("Qty", x.Qty),
//                   new XAttribute("Prj_Id", x.Prj_Id),
//                   new XAttribute("Prj_Name", x.Prj_Name),
//                   new XAttribute("Issue_To", x.Issue_To),
//                   new XAttribute("IssueTo_Name", x.IssueTo_Name),
//                   //new XAttribute("Requested_By", res1.Requested_By),
//                   //new XAttribute("RequestedBy_Name", res1.RequestedBy_Name),
//                   new XAttribute("PO_Number", res1.FirstOrDefault().PO_Number),
//                   new XAttribute("IssueNo", issueNotNum),
//                   new XAttribute("DemandOrder_Id", Group_Id),
//                   new XAttribute("Dep_Id", res1.FirstOrDefault().Dep_Id),
//                   new XAttribute("Dep_Name", res1.FirstOrDefault().Dep_Name),
//                   new XAttribute("Warehouse_Id", res2.Warehouse_Id),
//                   new XAttribute("Warehouse_Name", res2.WarehouseName)
//                   ))).ToString();
//                var res = db.Sp_Update_Stockout_DirectIssuance(Transaction_Id, Userid, data).FirstOrDefault();
//                dinums.Add(issueNotNum);
//                db.Sp_Add_Activity(Userid, "Direct Issued " + PO_Number, "Create", "Activity_Record", ActivityType.Inventory.ToString(), Group_Id);


//            }

//            //if (flag)
//            //{
//            //    Transaction.Rollback();
//            //    var ret = new { Status = false, Msg = "Cannot Handover Stock Multiple time" };
//            //    return Json(ret);
//            //}
//            //{
//            //    AccountHandlerController de = new AccountHandlerController();
//            //    de.DINEntries(Items, Userid, Transaction_Id, dinums);
//            //}
//            //Transaction.Commit();
//            //}

//            //Notifier.Notify(Userid, Convert.ToInt64(res1.Requested_By), NotifierMsg.Item_Issued, new object[] { res }, NotifierMsg.Item_Issued.ToString());
//            var ret1 = new { Status = true, Msg = "Direct Issue Note Generated", DI = dinums };
//            return Json(ret1);
//        }
//        public ActionResult DirectIssueNote(string Id, long TransactionId)
//        {
//            var res = (from x in db.Inventory_Stock_Out
//                       join y in db.Inventories on x.Item_Id equals y.Id
//                       where x.Group_Id == TransactionId && x.IssueNote_No == Id
//                       select new DirectStock
//                       {
//                           Item_Id = x.Item_Id,
//                           DemandOrder_No = x.PO_Number,
//                           Item_Name = y.Complete_Name,
//                           UOM = y.UOM,
//                           Qty = x.Qty,
//                           Prj_Name = x.Prj_Name,
//                           IssueNote_No = x.IssueNote_No,
//                           IssuedTo_Date = x.IssuedTo_Date,
//                           IssueBy_Name = x.IssueBy_Name,
//                           IssueTo_Name = x.IssueTo_Name,
//                       }).ToList();
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Direct Issue Note", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), TransactionId);

//            return View(res);
//        }
//        public ActionResult Categories()
//        {
//            return View();
//        }
//        public ActionResult CategoryList()
//        {
//            var res = db.Inventory_Category.ToList();
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Category List", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return PartialView(res);
//        }
//        public JsonResult AddCategory(string Name, string Code)
//        {
//            var res = db.Inventory_Category.Any(x => x.Name == Name);
//            if (res)
//            {
//                return Json(new Return { Status = false, Msg = "" });
//            }
//            Inventory_Category c = new Inventory_Category()
//            {
//                Name = Name,
//                Code = Code
//            };
//            db.Inventory_Category.Add(c);
//            db.SaveChanges();
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Added New Category " + Name + " Code: " + Code, "Create", "Activity_Record", ActivityType.Inventory.ToString(), userid);

//            return Json(new Return { Status = true, Msg = "Category Added" });
//        }
//        public JsonResult DeleteCategory(long Id)
//        {
//            var res = db.Inventory_Category.Where(x => x.Id == Id).FirstOrDefault();
//            db.Inventory_Category.Remove(res);
//            db.SaveChanges();
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Deleted Category  " + res.Name, "Delete", "Activity_Record", ActivityType.Inventory.ToString(), Id);

//            return Json(new Return { Status = true, Msg = "Category Removed" });
//        }
//        public ActionResult UpdateCategory(long Id)
//        {
//            var res = db.Inventory_Category.Where(x => x.Id == Id).FirstOrDefault();
//            return PartialView(res);
//        }
//        [HttpPost]
//        public JsonResult UpdateCate(int Id, string Name, string Code)
//        {
//            Inventory_Category cat = new Inventory_Category
//            {
//                Id = Id,
//                Name = Name,
//                Code = Code
//            };
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Updated Category " + Name + " Code" + Code, "Update", "Activity_Record", ActivityType.Inventory.ToString(), Id);

//            db.Inventory_Category.Attach(cat);
//            db.Entry(cat).Property(x => x.Name).IsModified = true;
//            db.Entry(cat).Property(x => x.Code).IsModified = true;
//            db.SaveChanges();
//            return Json(new Return { Status = true, Msg = "Updated" });
//        }
//        public ActionResult GRNNeedSupervision()
//        {
//            var res = (from x in db.Inventory_Stock_In
//                       join y in db.Inventories on x.Item_Id equals y.Id
//                       where x.Supervise == false
//                       select new Stock_In_Details { Item_Name = y.Complete_Name, UOM = y.UOM, Inv = x }).ToList();
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed GRN Supervision List", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View(res);
//        }
//        public ActionResult GRNDetails(string GRN)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = (from x in db.Inventory_Stock_In
//                       join y in db.Inventories on x.Item_Id equals y.Id
//                       where x.GRN == GRN
//                       select new Stock_In_Details { Item_Name = y.Complete_Name, UOM = y.UOM, Inv = x, Warehouse = x.WarehouseName }).ToList();

//            Helpers H = new Helpers();
//            ViewBag.TransactionId = H.RandomNumber();

//            db.Sp_Add_Activity(userid, "Accessed GRN Details", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View(res);
//        }
//        public JsonResult SuperviseGRN(List<GRNItems> items, long Vendor_Id, long Group_Id, long StockIn_Trans, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var stockin = db.Inventory_Stock_In.Where(x => x.TransactionId == StockIn_Trans).ToList();

//            db.Sp_Add_Activity(userid, "Supervised GRN for Vendor " + Vendor_Id, "Update", "Activity_Record", ActivityType.Inventory.ToString(), Group_Id);

//            if (stockin.Any(x => x.Supervise == true))
//            {
//                return Json(new Return { Status = false, Msg = "GRN already Supervised" });
//            }

//            var itemids = items.Select(z => z.Item_Id).ToList();
//            var checkcat = db.Inventories.Where(x => itemids.Contains(x.Id) && x.Category_Id == null).Select(x => x.Complete_Name).ToList();

//            if (checkcat.Any())
//            {
//                return Json(new Return { Status = false, Msg = string.Join(" ,", checkcat) + " are not Categorized" });
//            }

//            ProcurementController p = new ProcurementController();
//            p.GenerateInvoice(Vendor_Id, Group_Id, TransactionId, userid, items);
//            return Json(new Return { Status = true, Msg = "Supervised Sucessfully" });
//        }
//        public JsonResult SuperviseDN(List<GRNItems> items, long Vendor_Id, long Group_Id, long StockIn_Trans, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var stockin = db.Inventory_Stock_In.Where(x => x.TransactionId == StockIn_Trans).ToList();

//            if (stockin.Any(x => x.Supervise == true))
//            {
//                return Json(new Return { Status = false, Msg = "DN already Supervised" });
//            }

//            var itemids = items.Select(z => z.Item_Id).ToList();
//            var checkcat = db.Inventories.Where(x => itemids.Contains(x.Id) && x.Category_Id == null).Select(x => x.Complete_Name).ToList();

//            if (checkcat.Any())
//            {
//                return Json(new Return { Status = false, Msg = string.Join(" ,", checkcat) + " are not Categorized" });
//            }

//            ProcurementController p = new ProcurementController();
//            p.GenerateInvoice(Vendor_Id, Group_Id, TransactionId, userid, items);
//            return Json(new Return { Status = true, Msg = "Supervised Sucessfully" });
//        }
//        public ActionResult IssuanceNeedSupervision()
//        {
//            var res = (from x in db.Inventory_Stock_Out
//                       join y in db.Inventories on x.Item_Id equals y.Id
//                       join z in db.Inventory_Demand_Order on x.DemandOrder_Id equals z.Group_Id
//                       where x.Supervise == false
//                       select new Stock_Out_Details { Item_Name = y.Complete_Name, UOM = y.UOM, Inv = x, DemandOrder_No = z.DemandOrder_No }).ToList();
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Issuance Supervision Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View(res);
//        }
//        public ActionResult DeliveryNoteNeedSupervision()
//        {
//            var res = (from x in db.Inventory_Stock_In
//                       join y in db.Inventories on x.Item_Id equals y.Id
//                       where x.Supervise == false && x.GRN.Contains("DN")
//                       select new Stock_In_Details { Item_Name = y.Complete_Name, UOM = y.UOM, Inv = x }).ToList();

//            return View(res);
//        }
//        public ActionResult DNDetails(string DN)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = (from x in db.Inventory_Stock_In
//                       join y in db.Inventories on x.Item_Id equals y.Id
//                       where x.GRN == DN
//                       select new Stock_In_Details { Item_Name = y.Complete_Name, UOM = y.UOM, Inv = x, Warehouse = x.WarehouseName }).ToList();

//            Helpers H = new Helpers();
//            ViewBag.TransactionId = H.RandomNumber();
//            return View(res);
//        }
//        public ActionResult IssuanceDetails(string isn)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = (from x in db.Inventory_Stock_Out
//                       join y in db.Inventories on x.Item_Id equals y.Id
//                       where x.IssueNote_No == isn
//                       select new Stock_Out_Details { Item_Name = y.Complete_Name, UOM = y.UOM, Inv = x, Warehouse = x.Warehouse_Name }).ToList();

//            db.Sp_Add_Activity(userid, "Accessed Details of Issuance " + isn, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            var demnot = db.Inventory_Demand_Order.Where(x => x.Group_Id == res.FirstOrDefault().Inv.Group_Id).FirstOrDefault();

//            res.ForEach(x => x.DemandOrder_No = demnot.DemandOrder_No);

//            Helpers H = new Helpers();
//            ViewBag.TransactionId = H.RandomNumber();
//            return View(res);
//        }
//        public void GenerateItemBalance(long? Item_Id)
//        {
//            List<GRNItems> grns = new List<GRNItems>();
//            var res = db.Inventory_Stock_In.Where(x => x.Item_Id == Item_Id && x.Cancelled != true).Select(x => new GRNItems
//            {
//                ActionDate = x.DateTime,
//                IsAsset = x.IsAsset,
//                Item_Id = x.Item_Id,
//                Qty = x.Qty,
//                Rate = x.Rate,
//                InsertionType = 1,//stock In
//                Nums = x.GRN,
//                GroupTag = x.Group_Id
//            }).ToList();
//            var res_stockOut = db.Inventory_Stock_Out.Where(x => x.Item_Id == Item_Id && x.Cancelled != true).Select(x => new GRNItems
//            {
//                InsertionType = 0,
//                Item_Id = x.Item_Id,
//                Qty = x.Qty,
//                ActionDate = x.IssuedTo_Date,
//                Nums = x.IssueNote_No,
//                GroupTag = x.Group_Id
//            }).ToList();
//            grns.AddRange(res);
//            grns.AddRange(res_stockOut);

//            grns.OrderBy(x => x.ActionDate);
//            grns.Where(x => x.InsertionType == 0).ToList().ForEach(x => x.Rate = db.Inventory_Ledger.OrderByDescending(z => z.Id).Select(y => y.AVCO).FirstOrDefault());
//            grns.RemoveAll(x => x.Qty == null || x.Qty == 0);

//            foreach (var t in grns)
//            {
//                AddLedger(t, t.GroupTag, t.Nums, t.ActionDate);
//            }

//            //foreach (var i in res)
//            //{
//            //    GRNItems g = new GRNItems
//            //    {
//            //        Expirey = i.Expire_Date,
//            //        IsAsset = i.IsAsset,
//            //        Item_Id = i.Item_Id,
//            //        Qty = i.Qty,
//            //        Rate = i.Rate
//            //    };
//            //    this.AddLedger(g, i.Group_Id, i.GRN, i.DateTime);
//            //}
//            //return;
//        }
//        public void Generateitemrate()
//        {
//            var res = db.Inventory_Stock_In.Where(x => x.Rate == null).ToList();
//            foreach (var g in res.GroupBy(x => x.Item_Id))
//            {
//                var firstrate = db.Inventory_Stock_In.Where(x => x.Item_Id == g.Key && x.Rate != null).FirstOrDefault();

//                if (firstrate == null)
//                {
//                    continue;
//                }
//                foreach (var item in g)
//                {
//                    item.Rate = firstrate.Rate;
//                    db.Inventory_Stock_In.Attach(item);
//                    db.Entry(item).Property(x => x.Rate).IsModified = true;
//                    db.SaveChanges();
//                }
//            }
//        }
//        public void AddLedger_Beta(InventoryLedgerInserterModel i, long? Grp, string refno, DateTime? date)
//        {
//            decimal? bq = 0, bp = 0;
//            var latest = db.Sp_Get_InventoryItemLastRates(i.Item_Id).FirstOrDefault();
//            if (latest != null)
//            {
//                bq = latest.Balance_Qty;
//                bp = latest.Balance_Price;
//            }

//            if (i.InsertionType == 1)
//            {
//                Inventory_Ledger l = new Inventory_Ledger()
//                {
//                    Qty = Convert.ToDecimal(i.Qty),
//                    Rate = Convert.ToDecimal(i.Rate),
//                    Balance_Qty = i.Qty + bq,
//                    Balance_Price = (i.Qty * i.Rate) + bp,
//                    Item_Id = Convert.ToInt64(i.Item_Id),
//                    In_Out = true,
//                    Ref_Group_Id = Grp,
//                    Ref_No = refno,
//                    Ref_Type = "GRN",
//                    DateTime = date
//                };
//                db.Inventory_Ledger.Add(l);
//                db.SaveChanges();
//            }
//            else
//            {
//                Inventory_Ledger l = new Inventory_Ledger()
//                {
//                    Qty = Convert.ToDecimal(i.Qty),
//                    Rate = Convert.ToDecimal(i.Rate),
//                    Balance_Qty = i.Qty + bq,
//                    Balance_Price = (i.Qty * i.Rate) + bp,
//                    Item_Id = Convert.ToInt64(i.Item_Id),
//                    In_Out = false,
//                    Ref_Group_Id = Grp,
//                    Ref_No = refno,
//                    Ref_Type = "Issue",
//                    DateTime = date
//                };
//                db.Inventory_Ledger.Add(l);
//                db.SaveChanges();
//            }

//        }
//        public void AddLedger(GRNItems i, long? Grp, string refno, DateTime? date)
//        {
//            decimal? bq = 0, bp = 0;
//            var latest = db.Sp_Get_InventoryItemLastRates(i.Item_Id).FirstOrDefault();
//            if (latest != null)
//            {
//                bq = latest.Balance_Qty;
//                bp = latest.Balance_Price;
//            }

//            if (refno.Contains("GRN"))
//            {
//                Inventory_Ledger l = new Inventory_Ledger()
//                {
//                    Qty = Convert.ToDecimal(i.Qty),
//                    Rate = Convert.ToDecimal(i.Rate),
//                    Balance_Qty = i.Qty + bq,
//                    Balance_Price = (i.Qty * i.Rate) + bp,
//                    Item_Id = Convert.ToInt64(i.Item_Id),
//                    In_Out = true,
//                    Ref_Group_Id = Grp,
//                    Ref_No = refno,
//                    Ref_Type = "GRN",
//                    DateTime = date
//                };
//                db.Inventory_Ledger.Add(l);
//                db.SaveChanges();
//            }
//            else
//            {
//                Inventory_Ledger l = new Inventory_Ledger()
//                {
//                    Qty = Convert.ToDecimal(i.Qty),
//                    Rate = Convert.ToDecimal(i.Rate),
//                    Balance_Qty = i.Qty + bq,
//                    Balance_Price = (i.Qty * i.Rate) + bp,
//                    Item_Id = Convert.ToInt64(i.Item_Id),
//                    In_Out = false,
//                    Ref_Group_Id = Grp,
//                    Ref_No = refno,
//                    Ref_Type = "Issue",
//                    DateTime = date
//                };
//                db.Inventory_Ledger.Add(l);
//                db.SaveChanges();
//            }
//        }
//        public JsonResult SuperviseIssuance(List<GRNItems> items, long Vendor_Id, long Group_Id, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            ProcurementController p = new ProcurementController();
//            p.GenerateInvoice(Vendor_Id, Group_Id, TransactionId, userid, items);

//            db.Sp_Add_Activity(userid, "Supervised Issuance " + string.Join(",", items.Select(x => x.Item_Id)), "Read", "Activity_Record", ActivityType.Inventory.ToString(), Group_Id);

//            return Json(new Return { Status = true, Msg = "Supervised Sucessfully" });
//        }
//        public void ItemCodeShode()
//        {
//            var items = (from x in db.Inventories
//                         where x.SKU == null
//                         select x).ToList();

//            foreach (var item in items)
//            {
//                var cat = db.Inventory_Category.Where(x => x.Id == item.Category_Id).FirstOrDefault();
//                if (cat == null)
//                {
//                    continue;
//                }
//                var num = db.Sp_Get_ReceiptNo("Item Code").FirstOrDefault();
//                var a = num.Split('-')[1];
//                string code = "SAG-" + cat.Code + "-" + a;

//                var c = db.Plot_Add_Receipt(code, null, null, null, null, null, null, null, null, item.Id, null, null, null);

//            }
//        }
//        public JsonResult SaveItemDepartment(long Id, long Dep_Id, string Dep_Name)
//        {
//            var res = db.Sp_Add_Inventory_Dep(Id, Dep_Id, Dep_Name);
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Added Departmental Item --> " + Dep_Name, "Create", "Activity_Record", ActivityType.Inventory.ToString(), Id);

//            return Json(new Return { Status = true, Msg = "Added Successfully" });
//        }
//        public void InveDep()
//        {
//            var deps = db.Comp_Dep_Desig.Where(x => x.Type == "Department").ToList();
//            string Ids = new XElement("Dep", deps.Select(x => new XElement("DepId", new XAttribute("Id", x.Id)
//                                               ))).ToString();
//            var res = db.Sp_Get_InventoryList_Dep(Ids).ToList();

//            foreach (var a in res)
//            {
//                Inventory_Department i = new Inventory_Department()
//                {
//                    Dep_Id = (long)a.Dep_Id,
//                    Dep_Name = a.Dep_Name,
//                    Item_Id = a.Id
//                };
//                db.Inventory_Department.Add(i);
//                db.SaveChanges();
//            }
//        }
//        public JsonResult DeleteItemDepartment(long Id)
//        {
//            var res = db.Sp_Delete_Inventory_Dep(Id);
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Deleted Departmental Item ", "Delete", "Activity_Record", ActivityType.Inventory.ToString(), Id);

//            return Json(new Return { Status = true, Msg = "Deleted Successfully" });
//        }
//        public JsonResult UploadRef(long Group_ref)
//        {
//            int index = 0; long Id = 0;
//            foreach (string file in Request.Files)
//            {
//                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
//                index++;
//                if (hpf.ContentLength == 0)
//                    continue;
//                var ext = Path.GetExtension(hpf.FileName);
//                if (!Directory.Exists(Server.MapPath("~/Repository/PurchaseRef/" + Group_ref)))
//                {
//                    Directory.CreateDirectory(Server.MapPath("~/Repository/PurchaseRef/" + Group_ref));
//                }
//                string savedFileName = Path.Combine(Server.MapPath("~/Repository/PurchaseRef/" + Group_ref), hpf.FileName);
//                hpf.SaveAs(savedFileName);
//                Repository_Files ii = new Repository_Files()
//                {
//                    Filename = hpf.FileName,
//                    DateTime = DateTime.Now,
//                    Ext = ext,
//                    Module = "Purchase_Requisition",
//                    Module_Id = Group_ref
//                };
//                db.Repository_Files.Add(ii);
//                db.SaveChanges();
//                Id = ii.Id;
//            }
//            return Json(new { Status = true, Id = Id });
//        }

//        public JsonResult InvoiceRefFile(long Group_ref)
//        {
//            int index = 0; long Id = 0;
//            foreach (string file in Request.Files)
//            {
//                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
//                index++;
//                if (hpf.ContentLength == 0)
//                    continue;
//                var ext = Path.GetExtension(hpf.FileName);
//                var fileName = Group_ref.ToString() + ext;
//                if (!Directory.Exists(Server.MapPath("~/Repository/InvoiceRef/" + Group_ref)))
//                {
//                    Directory.CreateDirectory(Server.MapPath("~/Repository/InvoiceRef/" + Group_ref));
//                }
//                string savedFileName = Path.Combine(Server.MapPath("~/Repository/InvoiceRef/" + Group_ref), fileName);
//                hpf.SaveAs(savedFileName);
//            }
//            return Json(new { Status = true, Id = Id });
//        }
//        public ActionResult UNCode_Inventory()
//        {
//            var res = db.Sp_Get_Stock_SKU_Null().ToList();
//            return View(res);
//        }
//        public ActionResult ChangeCodeOfItem(long? Id)
//        {
//            var res = db.Inventories.Where(x => x.Id == Id).FirstOrDefault();
//            return PartialView(res);
//        }

//        public JsonResult ChangeItemCode(long PreItem, long Item)
//        {
//            if (PreItem == Item)
//            {
//                return Json(new Return { Msg = "Same item cannot be replaced", Status = false });
//            }
//            else
//            {
//                var res = db.Sp_Update_ItemCodes(PreItem, Item);
//                long userid = User.Identity.GetUserId<long>();
//                db.Sp_Add_Activity(userid, "Updated Item Code " + PreItem, "Update", "Activity_Record", ActivityType.Inventory.ToString(), Item);
//                return Json(new Return { Msg = "Item has Replaced", Status = true });
//            }
//        }
//        public string ItemCode(long? Id)
//        {
//            var res = db.Inventories.Where(x => x.Id == Id).FirstOrDefault();
//            return (res == null) ? string.Empty : (res.SKU == null) ? string.Empty : res.SKU;
//        }

//        public ActionResult FullTagger()
//        {
//            var res = db.Sp_Get_Inventory_List_With_Departments().ToList();
//            //var res = db.Inventories.ToList();
//            return PartialView(res);
//        }

//        public ActionResult DemandOrderPrint(long Group_Id)
//        {
//            var res = db.Sp_Get_DemandOrder_Details(Group_Id).ToList();
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Demand Order For Print", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Group_Id);

//            var uid = User.Identity.GetUserId<long>();
//            ViewBag.printee = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
//            foreach (var item in res)
//            {
//                var st_in = (from x in db.Inventory_Stock_In
//                             where x.Item_Id == item.Item_Id
//                             group x by new { x.Warehouse_Id, x.WarehouseName } into g
//                             select new StockIn_Sum { Warehouse_Id = g.Key.Warehouse_Id, Warehouse_Name = g.Key.WarehouseName, Qty = g.Sum(x => x.Qty) }).ToList();



//                var st_out = (from x in db.Inventory_Stock_Out
//                              where x.Item_Id == item.Item_Id
//                              group x by new { x.Warehouse_Id, x.Warehouse_Name } into g
//                              select new StockIn_Sum { Warehouse_Id = g.Key.Warehouse_Id, Warehouse_Name = g.Key.Warehouse_Name, Qty = g.Sum(x => x.Qty) }).ToList();

//                var warehouse = db.Inventory_Warehouse.ToList();
//                foreach (var ware in warehouse)
//                {
//                    var stin = st_in.Where(x => x.Warehouse_Id == ware.Id).FirstOrDefault();
//                    var stou = st_out.Where(x => x.Warehouse_Id == ware.Id).FirstOrDefault();
//                    if (stin == null && stou == null)
//                    {
//                        continue;
//                    }

//                    WarehouseModel w = new WarehouseModel()
//                    {
//                        Total_Stock_In = (stin == null) ? 0 : stin.Qty,
//                        Total_Stock_Out = (stou == null) ? 0 : stou.Qty,
//                        Warehouse_Id = (stin == null) ? null : stin.Warehouse_Id,
//                        Warehouse_Name = (stin == null) ? null : stin.Warehouse_Name
//                    };
//                    item.Warehouse_Rep.Add(w);
//                }
//            }


//            //foreach (var item in res)
//            //{
//            //    var res1 = db.Sp_Get_Inventory_Availability_Parameter(item.Item_Id).FirstOrDefault();
//            //    item.Total_Stock_In = res1.Total_In_Qty;
//            //    item.Total_Stock_Out = res1.Total_Out_Qty;
//            //}
//            Helpers H = new Helpers();
//            ViewBag.Group_Id = H.RandomNumber();
//            return View(res);
//        }
//        public void AdjustStockIn(long? Group_Id, long? Item_Id, long? Vendor_Id)
//        {
//            var res1 = db.Inventory_PurchaseOrder.Where(x => x.Group_Id == Group_Id && x.Item_Id == Item_Id && x.Vendor_Id == Vendor_Id).ToList();
//            foreach (var a in res1)
//            {
//                db.Sp_Update_Stock_In(a.PO_Number, Group_Id, Vendor_Id, Item_Id);
//            }
//        }
//        public JsonResult ReverseGRN(string grn)
//        {
//            try
//            {
//                long userid = User.Identity.GetUserId<long>();
//                var a = db.Sp_Update_GRNReversal(grn);
//                db.Sp_Add_Activity(userid, "Reversed GRN " + grn, "Update", "Activity_Record", ActivityType.Inventory.ToString(), userid);
//                return Json(true);
//            }
//            catch (Exception e)
//            {
//                return Json(false);
//            }
//        }
//        public ActionResult InventoryTagging()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            db.Sp_Add_Activity(userid, "Accessed Inventory Tagging Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);


//            return View();
//        }


//        // For Only Use in Demand order

//        [HttpGet]
//        public JsonResult SearchInventory(string d)
//        {
//            var userid = User.Identity.GetUserId<long>();
//            var res = db.Inventories.Where(x =>  (x.Complete_Name.Contains(d) || x.SKU.Contains(d)) && (x.SKU != null)).Select(x => new { id = x.Id, text = x.Complete_Name }).Take(20).ToList();
//            return Json(new { items = res }, JsonRequestBehavior.AllowGet);
//        }


//        //[HttpGet]
//        //public JsonResult GetInventory_DepWise(string d)
//        //{
//        //    var userid = User.Identity.GetUserId<long>();
//        //    if (User.IsInRole("Administrator"))
//        //    {
//        //        var Dep = db.Comp_Dep_Desig.Where(x => x.Type == "Department").ToList();
//        //        string Ids = new XElement("Dep", Dep.Select(x => new XElement("DepId",
//        //                             new XAttribute("Id", x.Id)
//        //                             ))).ToString();
//        //        var res = db.Sp_Get_InventoryList_Dep(Ids).Where(x => x.Complete_Name.ToLowerInvariant().Contains(d)).Select(x => new { id = x.Id, text = x.Complete_Name }).ToList();

//        //        return Json(new { items = res }, JsonRequestBehavior.AllowGet);
//        //    }
//        //    else
//        //    {
//        //        var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//        //        var Dep = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).ToList();
//        //        string Ids = new XElement("Dep", Dep.Select(x => new XElement("DepId",
//        //                             new XAttribute("Id", x.Department_Id)
//        //                             ))).ToString();
//        //        var res = db.Sp_Get_InventoryList_Dep(Ids).Where(x => x.Complete_Name.ToLowerInvariant().Contains(d)).Select(x => new { id = x.Id, text = x.Complete_Name }).ToList();

//        //        return Json(new { items = res }, JsonRequestBehavior.AllowGet);
//        //    }

//        //}

//        public ActionResult Noti(long? id, NotifierMsg? tp, long? noti)
//        {
//            Thread notiReader = new Thread(() => Notifier.ReadNotification((long)noti));
//            notiReader.Start();

//            if (tp == NotifierMsg.Issuance_Request)
//            {
//                return RedirectToAction("DemandOrders", "Inventory");
//            }
//            else //if (tp == NotifierMsg.Item_Issued)
//            {
//                return RedirectToAction("ProjectConfiguration", "ConstructProjectManagement", new { Id = id });
//            }
//        }

//        public ActionResult ItemsInventory()
//        {
//            var res = db.Sp_Get_Inventory_List_With_Departments().ToList();
//            return View(res);
//        }
//        //public Sp_Get_CA_Head_MultiRef_Result Inventory_Head(long? Id, string Complete_Name)
//        //{
//        //    Sp_Get_CA_Head_MultiRef_Result res2 = new Sp_Get_CA_Head_MultiRef_Result();
//        //    res2 = db.Sp_Get_CA_Head_MultiRef("Assets", Complete_Name).FirstOrDefault();
//        //    if (res2 == null)
//        //    {
//        //        var res3 = db.Sp_Get_CA_Head_MultiRef("Assets", "Inventory").FirstOrDefault();
//        //        var res4 = db.Sp_Add_CA_Head(res3.Id, Complete_Name, "Inventory", Id);
//        //        res2 = db.Sp_Get_CA_Head_MultiRef("Assets", Complete_Name).FirstOrDefault();
//        //    }
//        //    return res2;
//        //}

//        // Material Shifting
//        public ActionResult MaterialShifting()
//        {
//            Helpers h = new Helpers();
//            var Group_Id = h.RandomNumber();
//            ViewBag.Group_Id = Group_Id;
//            return View();
//        }
//        public ActionResult GetIssueNoteDetails(string IssueNote, long? id)
//        {
//            var res2 = db.Sp_Get_Inventory_Details(id, IssueNote, "", "IssueNote").ToList();
//            return PartialView(res2);
//        }
//        public ActionResult GetProjectDetails(string projName, long? id)
//        {
//            var res2 = db.Sp_Get_Inventory_Details(id, projName, "", "IssueNote").ToList();
//            return PartialView(res2);
//        }
//        public JsonResult GetIssueNote(string q)
//        {
//            var allsearch = db.Inventory_Stock_Out.Where(x => x.IssueNote_No.Contains(q)).Select(x => new { id = x.Item_Id, text = x.IssueNote_No }).Distinct().ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetProject(string q)
//        {
//            var allsearch = db.Projects.Where(x => x.Name.Contains(q)).Select(x => new { id = x.Id, text = x.Name }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult IssueToName(string q)
//        {
//            var allsearch = db.Employees.Where(x => x.Name.Contains(q)).Select(x => new { id = x.Id, text = x.Name }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult ReceivedBy(string q)
//        {
//            var allsearch = db.Employees.Where(x => x.Name.Contains(q)).Select(x => new { id = x.Id, text = x.Name }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult AddMaterialShiftEntries(List<MaterialShiftItems> InventoryStockOut, long? Group_Id, long? PrjId, string PrjName, long? IssueToId, string IssueToName, long? RequestedById, string RequestedByName, DateTime? RequestedDate, long? IssueById, string IssueByName, long? DemandOrderId, string IssueNoteNo, long? DepId, string DepName, long? WId, string WName, long? PoId, string IssueNoteNoPrj, long? PItemId, long? ProjId, string ProjName, string ReceivedBy)
//        {
//            InventoryStockOut.RemoveAll(x => x.Qty <= 0);
//            long userid = long.Parse(User.Identity.GetUserId());
//            string userName = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();

//            var data = new XElement("Inventory", InventoryStockOut.Select(x => new XElement("Entries",
//                     new XAttribute("itemId", x.item_Id),
//                     new XAttribute("qty", x.Qty)))).ToString();
//            db.SP_Add_Inventory_Stockout_Material_Shifting(data, Group_Id, PrjId, PrjName, null, IssueToId, IssueToName, null, RequestedById, RequestedByName, RequestedDate, userid, userName, null, DemandOrderId, "", DepId, DepName, WId, WName, IssueNoteNoPrj, ProjId, ProjName, ReceivedBy);
//            db.Sp_Add_Activity(userid, "Accessed Issue Note ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Group_Id);
//            return Json(true);
//        }

//        public ActionResult MaterialShiftIssueNote(long id, string ProjName)
//        {
//            var projName = db.Inventory_Stock_Out.Where(x => x.Group_Id == id && x.Qty > 0).Select(x => x.Prj_Name).FirstOrDefault();
//            ViewBag.PrjName = projName;
//            var res = db.Sp_Get_Stockout_Details(id).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Issue Note ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), id);
//            return View(res);
//        }
//        public ActionResult MaterialShift()
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Receive Items")).ToList();
//            ViewBag.Users = new SelectList(All, "id", "Name");

//            db.Sp_Add_Activity(userid, "Accessed GRN List Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//            return View();
//        }
//        public ActionResult MaterialShift_Search(DateTime? From, DateTime? To)
//        {
//            long userid = User.Identity.GetUserId<long>();
//            var res = db.Sp_Get_MaterialShift_List(From, To).ToList();
//            db.Sp_Add_Activity(userid, "Search Material Shift Notes from " + From + " To" + To, "Read", "Activity_Record", ActivityType.Material_Shift.ToString(), userid);
//            return PartialView(res);
//        }

//        // Company Multi issuance


//        public ActionResult MultiItemIssueRequest_Comp(long Prj_Id)
//        {
//            long userid = User.Identity.GetUserId<long>();

//            ViewBag.Company = new SelectList(db.Companies.ToList(), "Id", "Company_Name", 1);
//            if (User.IsInRole("Administrator"))
//            {

//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.ToList(), "Id", "Name", 1);

//                ViewBag.Employees = new SelectList(db.Sp_Get_Employee().Where(x => x.Status == "Registered").OrderBy(x => x.Department).ToList(), "Id", "Name", "Department", 1);
//                Helpers h = new Helpers();
//                ViewBag.GroupId = h.RandomNumber();
//            }
//            else
//            {
//                var res = db.Prj_Attendees.Where(x => x.PrjId == Prj_Id).Select(x => x.Dep_Id).ToList();
//                var ids = new XElement("Deps", res.Select(x => new XElement("Ids",
//                 new XAttribute("Id", x)
//                 ))).ToString();
//                var res3 = db.Sp_Get_Employees_Dep(ids).Select(x => new { x.Id, x.Name, x.Department }).OrderBy(x => x.Department).ToList();
//                ViewBag.Employees = new SelectList(res3, "Id", "Name", "Department", 1);

//                var res1 = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var res2 = db.Employee_Designations.Where(x => x.Emp_Id == res1).Select(x => (long)x.Id).ToList();
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => res2.Contains(x.Id)).ToList(), "Id", "Name", 1);


//                Helpers h = new Helpers();
//                ViewBag.GroupId = h.RandomNumber();
//            }
//            return PartialView();
//        }
//    }
//}