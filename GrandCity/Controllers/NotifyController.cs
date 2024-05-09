using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    public class NotifyController : Controller
    {
        // GET: Notify
        private readonly Grand_CityEntities db = new Grand_CityEntities();
        [NoDirectAccess] public ActionResult Index()
        {
            return View();
        }
        public JsonResult TotalNotifications()
        {
            int count = 0;
            long UserId = long.Parse(User.Identity.GetUserId());
            count = (from a in db.Notifications
                     where (a.Destination == UserId && a.IsRead == false)
                     select a).Count();
            //int msgs = db.ChatMessages.Where(x => x.ToUser == UserId && x.IsRead == false).Count(x => x.IsRead == false);
            var data = new
            {
                nots = count,
                //msg = msgs
            };
            return Json(data);
        }
        [NoDirectAccess] public ActionResult AllNotifications()
        {
            long UserId = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Notifications_User(UserId).ToList();
            return View(res);
        }

        public JsonResult SendToDepartmentHOD(long? Dep, long From, object[] data, NotifierMsg msg)
        {
            var res3 = new XElement("Deps", new XElement("Ids", new XAttribute("Id", Dep))).ToString();
            var res4 = db.Sp_Get_Employees_Dep(res3).Select(x => x.Id).ToList();
            var res5 = db.SP_Get_HeadOfDepartment(Dep).ToList();
            foreach (var item in res5)
            {
                Notifier.Notify(From, Convert.ToInt64(item.Id), msg,  data, msg.ToString());
            }
            return Json(true);
        }

        public JsonResult Pending_PurchaseOrders()
        {
            var res = db.Inventory_PurchaseOrder.Where(x => x.StockEntered == false).Count();
            Notifier.Notify(0, NotifyTo.Purchase_Heads, NotifierMsg.Pending_Purchase_Orders, new object[] { res }, NotifyType.Procurment.ToString());
            return Json(true);
        }
    }
}