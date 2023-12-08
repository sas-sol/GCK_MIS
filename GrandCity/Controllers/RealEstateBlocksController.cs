using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using MeherEstateDevelopers.Filters;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class RealEstateBlocksController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();

        // GET: RealEstate_Blocks
        public ActionResult GetPhaseAndBlock()
        {
            var realEstate_Blocks = db.RealEstate_Blocks.Include(r => r.RealEstate_Phases);
            return View(realEstate_Blocks.ToList());
        }
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealEstate_Blocks realEstate_Blocks = db.RealEstate_Blocks.Find(id);
            if (realEstate_Blocks == null)
            {
                return HttpNotFound();
            }
            return View(realEstate_Blocks);
        }
        public ActionResult Create()
        {
            ViewBag.Phase_Id = new SelectList(db.Sp_Get_Phase(), "Id", "Phase_Name","Project_Name",1);
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "Id,Block_Name,Phase_Id")] RealEstate_Blocks realEstate_Blocks)
        {
            var res = db.Sp_Add_Block(realEstate_Blocks.Block_Name,realEstate_Blocks.Phase_Id).FirstOrDefault();
            return Json(res);
        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealEstate_Blocks realEstate_Blocks = db.RealEstate_Blocks.Find(id);
            if (realEstate_Blocks == null)
            {
                return HttpNotFound();
            }
            ViewBag.Phase_Id = new SelectList(db.RealEstate_Phases, "Id", "Phase_Name", realEstate_Blocks.Phase_Id);
            return View(realEstate_Blocks);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Blocks,Phase_Id")] RealEstate_Blocks realEstate_Blocks)
        {
            if (ModelState.IsValid)
            {
                db.Entry(realEstate_Blocks).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Phase_Id = new SelectList(db.RealEstate_Phases, "Id", "Phase_Name", realEstate_Blocks.Phase_Id);
            return View(realEstate_Blocks);
        }
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealEstate_Blocks realEstate_Blocks = db.RealEstate_Blocks.Find(id);
            if (realEstate_Blocks == null)
            {
                return HttpNotFound();
            }
            return View(realEstate_Blocks);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            RealEstate_Blocks realEstate_Blocks = db.RealEstate_Blocks.Find(id);
            db.RealEstate_Blocks.Remove(realEstate_Blocks);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult GetBlocks(long Id)
        {
            var res = new SelectList( db.Sp_Get_Project_Phase_Block_Parameter(Id).OrderBy(x=> x.Phase_Name), "Id" ,"Block_Name", "Phase_Name",1);
            return Json(res);
        }

        public JsonResult GetShops(long Id)
        {
            var res = db.Commercial_Rooms.Where(x =>x.Floor_Id == Id && x.Status == PlotsStatus.Available_For_Sale.ToString()).ToList();/*   new SelectList(db.Commercial_Rooms.Where(x => x.Plan_Id == proid && x.Floor_Id== Id),"Id", "Com_App_Shop_Number");*/
            return Json(res);
        }
        public ActionResult VerifcationRequests()
        {
            var res = db.Sp_Get_Requested_Verification_Building(1).ToList();
            return View(res);
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
