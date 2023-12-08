using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using MeherEstateDevelopers.Filters;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize(Roles = "Administrator")]
    [LogAction]
    public class RolesAndResponsibilitiesController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();

        // GET: RolesAndResponsibilities
        public ActionResult CreateRoles()
        {
            var Group = Enum.GetValues(typeof(RoleGroup)).Cast<RoleGroup>().Select(e => new { Value = e, Text = e.ToString().Replace("_", " ") }).ToList();
            ViewBag.Group = new SelectList(Group, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateRoles(Role role)
        {
            var res = Convert.ToBoolean(db.Sp_Add_Role(role.Name, role.Group).FirstOrDefault());

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Created New Role " + role.Name, "Create", "Activity_Record", ActivityType.Roles.ToString(), userid);
            if (res)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public ActionResult CreateResponsibilities()
        {
            return View();
        }

        public ActionResult AssignRoles()
        {
            ViewBag.Users = new SelectList(db.Sp_Get_Users(), "Id", "Name");
            ViewBag.Roles = new SelectList(db.Sp_Get_Roles().OrderBy(x => x.Id), "Id", "Name", "Group", 1);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AssignRoles(long? Users, long? Roles)
        {
            var res = Convert.ToBoolean(db.Sp_Add_AssignRole(Roles, Users).FirstOrDefault());
            if (res)
            {
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Assigned New Role to users " + Users + " " + Roles, "Create", "Activity_Record", ActivityType.Roles.ToString(), Users);
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public ActionResult RolesResponsibilitiesCreation()
        {
            ViewBag.Roles = new SelectList(db.Sp_Get_Roles(), "Id", "Name");
            return View();
        }

        public ActionResult MISAccountToEmp()
        {
            ViewBag.Users = new SelectList(db.Sp_Get_Users(), "Id", "Name");
            ViewBag.Employee = new SelectList(db.Sp_Get_Employee().Where(x => x.Status == "Registered").Select(x => new { x.Id, Name = x.Name + " (" + x.Employee_Code + " )", x.Department }).ToList(), "Id", "Name", "Department");
            return View();
        }

        public JsonResult AssignMISAccount(long? Users, long? Employee)
        {
            db.Sp_Update_EmployeeUser(Users, Employee);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Created MIS Account for Employee " + Employee, "Create", "Activity_Record", ActivityType.Roles.ToString(), Users);
            return Json(true);
        }

        public JsonResult ResetEmployeePassword(long? Id)
        {
            var res = db.Sp_Update_ResetPassword_Employee(Id);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Reset Employee Password " + Id, "Create", "Activity_Record", ActivityType.Roles.ToString(), null);
            return Json(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult UserRolesManage(long Id)
        {
            var res = db.Sp_Get_UserAllRoles(Id).ToList();
            ViewBag.Username = db.Users.Where(x => x.Id == Id).Select(x => x.Email).FirstOrDefault();
            ViewBag.Id = Id;

            return PartialView(res);
        }

        public JsonResult UnassignRole(long Id, long uid)
        {
            db.Sp_Delete_UserRole(Id, uid);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Unassigned Role " + Id + " " + uid, "", "Activity_Record", ActivityType.Roles.ToString(), Id);
            return Json(true);
        }
    }
}
