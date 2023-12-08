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
    public class RealEstatePhasesController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();

        // GET: RealEstate_Phases
        public JsonResult GetPhasesAndBlock(long id)
        {
            var realEstate_Phases = db.Sp_Get_Project_Phase_Block_Parameter(id);
            return Json(realEstate_Phases);
        }
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealEstate_Phases realEstate_Phases = db.RealEstate_Phases.Find(id);
            if (realEstate_Phases == null)
            {
                return HttpNotFound();
            }
            return View(realEstate_Phases);
        }
        public ActionResult Create()
        {
            ViewBag.Project_Id = new SelectList(db.RealEstate_Projects, "Id", "Project_Name");
            return PartialView();
        } 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "Id,Phase_Name,Description,Project_Id")] RealEstate_Phases realEstate_Phases)
        {
            var res = db.Sp_Add_Phase(realEstate_Phases.Phase_Name, realEstate_Phases.Description, realEstate_Phases.Project_Id).FirstOrDefault();
            return Json(res);
        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealEstate_Phases realEstate_Phases = db.RealEstate_Phases.Find(id);
            if (realEstate_Phases == null)
            {
                return HttpNotFound();
            }
            ViewBag.Project_Id = new SelectList(db.RealEstate_Projects, "Id", "Project_Name", realEstate_Phases.Project_Id);
            return View(realEstate_Phases);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Phase_Name,Description,Project_Id")] RealEstate_Phases realEstate_Phases)
        {
            if (ModelState.IsValid)
            {
                db.Entry(realEstate_Phases).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Project_Id = new SelectList(db.RealEstate_Projects, "Id", "Project_Name", realEstate_Phases.Project_Id);
            return View(realEstate_Phases);
        }


        /// <summary>
        /// ///////////////////////////////////////////////
        /// </summary>
        /// <param name="disposing">Commercial Floor Creatation</param>
        /// 

        public ActionResult CreateCommercialFloor()
        {
            ViewBag.Project_Id = new SelectList(db.RealEstate_Projects, "Id", "Project_Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateCommercialFloor(Commercial_FloorsPlan cf)
        {
            var res = db.Sp_Add_CommercialFloor(cf.Description, cf.Floor, cf.Project_Id).FirstOrDefault();
            return Json(res);
        }
        public JsonResult GetCommercialFloor(long Id)
        {
            var res = db.Sp_Get_Project_CommercialFloor_Parameter(Id);
            return Json(res);
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
