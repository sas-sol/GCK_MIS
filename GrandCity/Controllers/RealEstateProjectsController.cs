using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class RealEstateProjectsController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();

        [NoDirectAccess] public ActionResult ProjectConfiguration()
        {
            return View();
        }
        
        [NoDirectAccess] public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealEstate_Projects realEstate_Projects = db.RealEstate_Projects.Find(id);
            if (realEstate_Projects == null)
            {
                return HttpNotFound();
            }
            return View(realEstate_Projects);
        }

        [NoDirectAccess] public ActionResult Create()
        {
            return PartialView();
        }
        
        [NoDirectAccess] public ActionResult ProjectList()
        {
            var __rpl = db.RealEstate_Projects.Where(x=>x.Type == "General").ToList();
            var __phases = db.RealEstate_Phases.Select(x => new { x.Id, x.Phase_Name, x.Project_Id, MType = 2 }).ToList();
            var __projects = db.RealEstate_Projects.Select(x => new { x.Id, x.Project_Name, MType = 1 }).ToList();
            var __blocks = db.RealEstate_Blocks.Select(x => new { x.Id, x.Block_Name, x.Phase_Id, MType = 3 }).ToList();


            List<RealEstateHierarichalViewModel> __frmtd = new List<RealEstateHierarichalViewModel>();
            List<RealEstateHierarichalViewModel> __phases__frmtd = new List<RealEstateHierarichalViewModel>();
            List<RealEstateHierarichalViewModel> __blks__frmtd = new List<RealEstateHierarichalViewModel>();

            __blks__frmtd.AddRange(
                __blocks.Select(x => new RealEstateHierarichalViewModel
                {
                    Children = null,
                    Id = x.Id,
                    Name = x.Block_Name,
                    Type = x.MType,
                    Parent = x.Phase_Id
                })
                );

            __phases__frmtd.AddRange(
                __phases.Select(x => new RealEstateHierarichalViewModel
                {
                    Children = null,
                    Id = x.Id,
                    Name = x.Phase_Name,
                    Type = x.MType,
                    Parent = x.Project_Id
                })
                );
            __phases__frmtd.ForEach(x => x.Children = __blks__frmtd.Where(y => y.Parent == x.Id).ToList());
            __frmtd.AddRange(
                __projects.Select(x => new RealEstateHierarichalViewModel
                {
                    Children = __phases__frmtd.Where(y => y.Parent == x.Id).ToList(),
                    Id = x.Id,
                    Name = x.Project_Name,
                    Type = x.MType
                })
                );
            return PartialView(__frmtd);
        }
        
        [NoDirectAccess] public ActionResult ProjectsDetail()
        {
            return PartialView();
        }
        
        [NoDirectAccess] public ActionResult NewProject()
        {
            return PartialView();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess] public ActionResult Create([Bind(Include = "Id,Project_Name,Description,Type")] RealEstate_Projects realEstate_Projects)
        {
            var res = db.Sp_Add_RealEstateProject(realEstate_Projects.Project_Name, realEstate_Projects.Description, realEstate_Projects.Type).FirstOrDefault();
            //if (res == true)
            //{
                TempData["SuccessMessage"] = "Project added successfully.";
                //return Json(res);
                return RedirectToAction("ProjectsDetail", "RealEstateProjects");
            //}
            //else
            //{
            //    TempData["SuccessMessage"] = "Error Occur, Project not added.";
            //    //return Json(res);
            //    return RedirectToAction("ProjectsDetail", "RealEstateProjects");
            //}
        }
        
        [NoDirectAccess] public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealEstate_Projects realEstate_Projects = db.RealEstate_Projects.Find(id);
            if (realEstate_Projects == null)
            {
                return HttpNotFound();
            }
            return View(realEstate_Projects);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess] public ActionResult Edit([Bind(Include = "Id,Project_Name,Description")] RealEstate_Projects realEstate_Projects)
        {
            if (ModelState.IsValid)
            {
                db.Entry(realEstate_Projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(realEstate_Projects);
        }
        
        [NoDirectAccess] public ActionResult RealestatePrjDetail(long Id)
        {
            return View();
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
