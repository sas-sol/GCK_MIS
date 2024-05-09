using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class VendorController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        // GET: Vendor
        [NoDirectAccess] public ActionResult Index()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult VendorList()
        {
            var res = db.Sp_Get_Vendor().ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult ActiveVendors()
        {
            var res = db.Sp_Get_Vendor_Status(1).ToList();
            return PartialView(res);
        } 
        [NoDirectAccess] public ActionResult ArchiveVendors()
        {
            var res = db.Sp_Get_Vendor_Status(0).ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult RegisterVendor()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RegisterVendor(Vendor vendor)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res = db.Sp_Add_Vendor(vendor.Company, vendor.Company, vendor.Address, vendor.Email, vendor.Office_Mobile, vendor.Fax, vendor.City, userid, vendor.NTN, vendor.STRN).FirstOrDefault();

                    AccountHandlerController ah = new AccountHandlerController();
                    ah.RegisterVendor(res.VendorId, vendor.Company ,userid);

                    db.Sp_Add_Activity(userid, "Registered New Vendor " + vendor.Name, "Create", "Activity_Record", ActivityType.Vendor_Register.ToString(), res.VendorId);
                    Transaction.Commit();
                    return Json(true);
                }

                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(false);
                }
            }
        }
        [NoDirectAccess] public ActionResult TemporaryRegisterVendor()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult TempRegisterVendor(Vendor vendor)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Add_Vendor(vendor.Company, vendor.Company, vendor.Address, vendor.Email, vendor.Office_Mobile, vendor.Fax, vendor.City, userid, vendor.NTN, vendor.STRN).FirstOrDefault();
            db.Sp_Add_Activity(userid, "Registered New Vendor " + vendor.Name, "Create", "Activity_Record", ActivityType.Vendor_Register.ToString(), res.VendorId);

            return Json(true);
        }
        [NoDirectAccess] public ActionResult VendorDetails(long Id)
        {
            var res1 = db.Vendors.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Vendor_Representative.Where(x => x.Vendor_Id == Id).ToList();
            var res3 = db.Vendor_Credit_Terms.Where(x => x.Vendor_Id == Id).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Vendor Details of " + res1.Name, "Read", "Activity_Record", ActivityType.Vendor.ToString(), res1.Id);
            var res = new VendorFullDetail { Representative = res2, Terms = res3, Vendor = res1 };
            var ress1 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id).ToList();
            var ress2 = db.Inventory_PurchaseOrder.Where(x => ress1.Contains(x.Group_Id) && x.Vendor_Id == Id).Select(x => x.PO_Number).ToList();
            //Requisition No.
            var PR = db.Inventory_Purchase_Req.Where(x => ress1.Contains(x.Group_Id)).Select(x => new DeptsModel { deptId = (long)x.Group_Id, deptName = x.Requisition_No }).ToList();


            var xml1 = new XElement("GroupId", ress2.Select(x => new XElement("Id",
                 new XAttribute("Group", x)))).ToString();

            var ress3 = db.Sp_Get_PO_Completed(res1.Register_Date.Value.Date, DateTime.Now.Date, xml1, false).ToList();

            ViewBag.TotalPO = ress3.Sum(x => x.Total);
            var ress4 = db.Vouchers.Where(x => x.Vendor_Id == Id).ToList();
            
            var resss3 = db.Sp_Get_PO_Completed(null, null, xml1, false).ToList();
            var xml2 = new XElement("GroupId", resss3.Select(x => new XElement("Id",
                 new XAttribute("Group", x.PO_Number)))).ToString();

            var res4 = db.Sp_Get_PO_Completed_GRN(null, null, xml2).ToList();
            decimal? Receivedval = 0;
            //x => x.Group_Id && x.PO_Number, x.PO_Date, x.Vendor_Name
            foreach (var po in ress3.GroupBy(x=>x.Group_Id  ))
            {
                foreach (var a in po)
                {
                    var ppo = po.Select(x => x.PO_Number).FirstOrDefault();
                    var it = res4.Where(x => x.Item_Id == a.Item_Id && x.PO_Number ==ppo ).FirstOrDefault();
                    if (it != null)
                    {
                        Receivedval += it.R_Qty * a.Purchase_Rate;
                    }
                }
                
            }
            ViewBag.TotalPayment = Receivedval;

            return View(res);
        }
        [NoDirectAccess] public ActionResult VendorTerms(long Id)
        {
            var res = db.Vendor_Credit_Terms.Where(x => x.Vendor_Id == Id).ToList();
            return PartialView(res);
        }
        public JsonResult SaveTerm(string Term, int? Date, long Ven_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            Vendor_Credit_Terms t = new Vendor_Credit_Terms()
            {
                Vendor_Id = Ven_Id,
                Days = Date,
                Terms = Term
            };
            db.Vendor_Credit_Terms.Add(t);
            
            db.Sp_Add_Activity(userid, "Added New Terms For Vendor " + Term, "Create", "Activity_Record", ActivityType.Vendor_Terms.ToString(), Ven_Id);
            db.SaveChanges();
                var ret = new { Status = true, Msg = "Successfully Added" };
                return Json(ret);
        }
        [NoDirectAccess] public ActionResult AddVendorRepresentative()
        {

            return PartialView();
        }
        public JsonResult AddVendorRepresentat(Vendor_Representative v)
        {
            db.Vendor_Representative.Add(v);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Added New Vendor Representative " + v.Name, "Create", "Activity_Record", ActivityType.Vendor.ToString(), userid);
            db.SaveChanges();
            return Json(true);
        }
        [NoDirectAccess] public ActionResult VendorDetail(long? VendorBidId, long? VendorId)
        {
            var res = db.Sp_Get_VendorDetail(VendorId).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult VendorDetailById(long? VendorId)
        {
            var res = db.Vendors.Where(x => x.Id == VendorId).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Vendor Details of " + res.Select(x=>x.Name).FirstOrDefault(), "Read", "Activity_Record", ActivityType.Vendor.ToString(), VendorId);
            return PartialView(res);
        }
        public JsonResult VendorDetails_Short(long? Id)
        {
            var res = db.Vendors.Where(x => x.Id == Id).Select(x => new { x.Name, x.Company, x.Address }).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Vendor Details of " + res.Name, "Read", "Activity_Record", ActivityType.Vendor.ToString(), Id);
            return Json(res);
        }
        [HttpPost]
        public JsonResult DeleteVendor(long Id)
        {
            db.Sp_Delete_Vendor(Id);
            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Deleted Vendor "+ Id , "Delete", "Activity_Record", ActivityType.Vendor.ToString(), Id);
            return Json(true);
        }
        [NoDirectAccess] public ActionResult VendorUpdate(long Id)
        {
            var res = db.Vendors.Where(x => x.Id == Id).FirstOrDefault();
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult UpdateVendorResult(Vendor v)
        {
            db.Sp_Update_Vendor(v.Id, v.Company, v.Company, v.Address, v.Email, v.Office_Mobile, v.Fax, v.City,v.NTN,v.STRN);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Updated Vendor Details of " + v.Name, "Update", "Activity_Record", ActivityType.Vendor.ToString(), v.Id);
            return Json(true);
        }
        [HttpGet]
        public JsonResult GetVendorsForAutoComplete(string q)
        {
            var allsearch = db.Vendors.Where(x => x.Name.Contains(q) || x.Company.Contains(q)).Select(x => new { id = x.Id, text = x.Company }).ToList();
            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
        }

        [NoDirectAccess] public ActionResult AddTermsVendor()
        {


            return PartialView();
        }
        [NoDirectAccess] public ActionResult PurchaseOrdersList(long Id)
        {
            ViewBag.Id = Id;

            return PartialView();
        }


        [NoDirectAccess] public ActionResult PurchaseOrders(long Id,DateTime? From,DateTime? To)
        {
            long userid = User.Identity.GetUserId<long>();
            var res1 = db.Sp_Get_Stock_In_Userid(userid, null, null, null, PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).Select(x => x.Group_Id).ToList();
            var res2 = db.Inventory_PurchaseOrder.Where(x => res1.Contains(x.Group_Id) && x.Vendor_Id == Id).Select(x => x.PO_Number).ToList();
            //Requisition No.
            var PR = db.Inventory_Purchase_Req.Where(x => res1.Contains(x.Group_Id)).Select(x => new DeptsModel { deptId = (long)x.Group_Id, deptName = x.Requisition_No }).ToList();

            var xml1 = new XElement("GroupId", res2.Select(x => new XElement("Id",
                 new XAttribute("Group", x)))).ToString();

            var res3 = db.Sp_Get_PO_Completed(From, To, xml1, true).ToList();
            var xml2 = new XElement("GroupId", res3.Select(x => new XElement("Id",
                 new XAttribute("Group", x.PO_Number)))).ToString();

            var res4 = db.Sp_Get_PO_Completed_GRN(From, To, xml2).ToList();
            var res = new PO_Completed_With_GRN { PO = res3, GRN = res4, REQ = PR };
            
            return PartialView(res);
        } 
        [NoDirectAccess] public ActionResult VendorPayments(long Id)
        {

            var res = db.Vouchers.Where(x => x.Vendor_Id == Id).ToList();
            return PartialView(res);
        } 
        [NoDirectAccess] public ActionResult Ledger(long Id)
        {


            return PartialView();
        }
      
        public JsonResult DeleteRepresentative(long id)
        {
            db.Sp_Delete_Vendor_Rep(id);
            return Json(true);
        }

    }
}