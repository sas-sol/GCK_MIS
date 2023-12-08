using MeherEstateDevelopers.Filters;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class SideWidgetsController : Controller
    {
        // GET: SideWidgets
        private Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult LeftSideWidget()
        {
            return PartialView();
        }
        public ActionResult StaffLeftSideWidget()
        {
            var userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_UserGroup(userid).ToList();
            return PartialView(res);
        }
        public ActionResult LeftSideCEOWidget()
        {
            return PartialView();
        }
        public ActionResult Header()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_UserDetails_Short(userid).SingleOrDefault();
            var res2 = db.Sp_Get_Notifications_User_Short(userid).ToList();
            var res3 = db.Sp_Get_UserCompany(userid).ToList();

            var claim = db.UserClaims.Where(x => x.UserId == userid).ToList();
            ViewBag.Company = claim.Where(x => x.ClaimType == "Company").Select(x => x.ClaimValue).FirstOrDefault();

            var res = new Header { Details = res1, Noties = res2, Companylist = res3  };
            return PartialView(res);
        }
        public ActionResult StaffHeader()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_UserDetails_Short(userid).SingleOrDefault();
            var res2 = db.Sp_Get_Notifications_User_Short(userid).ToList();
            var res3 = db.Sp_Get_UserCompany(userid).ToList();

            var claim = db.UserClaims.Where(x => x.UserId == userid).ToList();
            ViewBag.Company = claim.Where(x => x.ClaimType == "Company").Select(x => x.ClaimValue).FirstOrDefault();

            var res = new Header { Details = res1, Noties = res2, Companylist = res3 };
            return PartialView(res);
        }
        public ActionResult GuestHeader()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_UserDetails_Short(userid).SingleOrDefault();
            return PartialView(res);
        }
        public ActionResult Footer()
        {
            return PartialView();
        }

    }
}