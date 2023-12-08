using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using MeherEstateDevelopers.Filters;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class GraphController : Controller
    {
        
        // GET: Graph
        private Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult DailyCollection()
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Daily Collections Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        public ActionResult DailyCollectionSearch(DateTime? From, DateTime? To, long?[] Users)
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Searched For Daily Collections From: " + From + " To: " + To, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            string chids = null;
            if (Users != null)
            {
                chids = new XElement("ChPoDd", Users.Select(x => new XElement("Details",
                                 new XAttribute("Ids", x)
                                 ))).ToString();
            }
            else
            {
                var All = db.Sp_Get_Cashiers_List().ToList();
                ViewBag.Users = new SelectList(All, "id", "Name");
                chids = new XElement("ChPoDd", All.Select(x => new XElement("Details",
                                      new XAttribute("Ids", x.Id)
                                     ))).ToString();
            }
            if (From is null)
            {
                DateTime now = DateTime.Now;
                From = Convert.ToDateTime( new DateTime(now.Year, now.Month, 1));
                To = Convert.ToDateTime(now.AddMonths(1).AddDays(-1));
            }
            var recov = (from ReceiptTypes e in Enum.GetValues(typeof(ReceiptTypes)) select new { Name = e.ToString() }).ToList();
            var payme = (from Payments e in Enum.GetValues(typeof(Payments)) select new { Name = e.ToString() }).ToList();
            recov.AddRange(payme);
            string types = new XElement("ReceiptsType", recov.Select(x => new XElement("Receipt",
                                  new XAttribute("Type", x.Name)
                                 ))).ToString();
            var res1 = db.Sp_Get_DailyRecovery_Report_Parameter(From, To, chids, types).Select(x => new DailyCashierUser_Report
            {
                Amount = x.Amount,
                Type = x.Type,
                Date_Month = string.Format("{0:dd-MMM}", x.DATETIME),
                DateTime = x.DATETIME
            }).OrderBy(x=> x.DateTime).ToList();

            var res = (from x in res1
                      group x by new { x.Type, x.Date_Month } into g
                      select new GeneralType1 { Prop1 = g.Key.Date_Month, Prop2 = g.Key.Type, Prop3 = g.Sum(x => x.Amount) }).ToList();
            return PartialView(res);
        }

        [Authorize(Roles = "CEO,Financial Dashboard")]
        public ActionResult MonthlyInflow_Outflow(DateTime From, DateTime To)
        {
            Report_Amounts data = new Report_Amounts()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal>()
            };
            if (From.Month == To.Month && From.Year == To.Year)
            {
                data = SingleMonthReport(From);
            }
            else
            {
                data = MultiMonthReport(From, To);
            }

            return PartialView(data);
        }
        public Report_Amounts SingleMonthReport(DateTime From)
        {
            var res = db.Sp_Reports_Monthly_InflowOutFlow(From).ToList();
            List<string> cat = new List<string>();
            cat = res.OrderBy(x => x.DATE).Select(x => Convert.ToDateTime(x.DATE).Date.ToString("dd/MM/yyyy")).Distinct().ToList();
            List<Report_Series_Decimal> Listrep = new List<Report_Series_Decimal>();
            Report_Series_Decimal r1 = new Report_Series_Decimal()
            {
                name = "Inflow",
                data = new decimal?[res.Count]
            };
            Report_Series_Decimal r2 = new Report_Series_Decimal()
            {
                name = "OutFlow",
                data = new decimal?[res.Count]
            };
            decimal? sum = 0;
            var newres = res.Select(x => x.Inflow).ToArray();
            for (int i = 0; i < newres.Length; i++)
            {
                sum = sum + newres[i];
                r1.data[i] = sum;
            }
            decimal? sum2 = 0;
            var newreso = res.Select(x => x.Outflow).ToArray();
            for (int i = 0; i < newreso.Length; i++)
            {
                sum2 = sum2 + newreso[i];
                r2.data[i] = sum2;
            }


            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);



            //r2.data = res.Select(x => x.Outflow).ToArray();
            //var data = res.Select(x => x.AmountExpected).ToArray();
            /*
                    decimal? sum = 0,exe_sum = 0;
            var newres = res.Select(x => x.AmountReceived).ToArray();
            for (int i = 0; i < newres.Length; i++)
            {
                sum = sum + newres[i];
                r1.data[i] = sum;
            }
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var ttl_Amt = res.Select(x => x.AmountExpected).FirstOrDefault();
            var perday = ttl_Amt / days;
            for (int i = 0; i < days; i++)
            {
                exe_sum = exe_sum + perday;
                r2.data[i] = exe_sum;
            }
             
             */


            Listrep.Add(r1);
            Listrep.Add(r2);
            Report_Amounts data = new Report_Amounts()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return data;
        }
        public Report_Amounts MultiMonthReport(DateTime From, DateTime To)
        {
            var res1 = db.Sp_Reports_Monthly_InflowOutFlow_FT(From, To).ToList();
            List<string> cat = new List<string>();
            cat = res1.Select(x => x.DATE).Distinct().ToList();
            List<InstallmentReceived_Expected> res = new List<InstallmentReceived_Expected>();
            foreach (var item in res1)
            {
                InstallmentReceived_Expected sr = new InstallmentReceived_Expected()
                {
                    Expected = (res1.Any(x => x.DATE == item.DATE)) ? res1.SingleOrDefault(x => x.DATE == item.DATE).Outflow : 0,
                    Date = item.DATE,
                    Received = (res1.Any(x => x.DATE == item.DATE)) ? res1.SingleOrDefault(x => x.DATE == item.DATE).Inflow : 0,
                };
                res.Add(sr);
            }
            List<Report_Series_Decimal> Listrep = new List<Report_Series_Decimal>();

            Report_Series_Decimal r1 = new Report_Series_Decimal()
            {
                name = "Inflow",
                data = new decimal?[res.Count]
            };
            Report_Series_Decimal r2 = new Report_Series_Decimal()
            {
                name = "Outflow",
                data = new decimal?[res.Count]
            };
            var newres = res.Select(x => x.Received).ToArray();
            for (int i = 0; i < newres.Length; i++)
            {
                r1.data[i] = newres[i];
            }
            r2.data = res.Select(x => x.Expected).ToArray();

            Listrep.Add(r1);
            Listrep.Add(r2);
            Report_Amounts data = new Report_Amounts()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_Decimal>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);
            return data;
        }

        public ActionResult PurchaseOrdersDep(DateTime From, DateTime To)
        {
            var res = db.Sp_Reports_PurchaseOrder(From , To).ToList();

            List<string> cat = new List<string>();

            cat = res.OrderBy(x => x.DATE).Select(x => Convert.ToDateTime(x.DATE).Date.ToString("dd/MM/yyyy")).Distinct().ToList();
            IDictionary<string, int> Date = new Dictionary<string, int>();
            for (int i = 0; i < cat.Count; i++)
            {
                Date.Add(cat[i], i);
            }
            List<Report_Series_D> Listrep = new List<Report_Series_D>();
            foreach (var g in res.GroupBy(x => x.Dep_Name))
            {
                Report_Series_D report = new Report_Series_D()
                {
                    name = g.Key,
                    data = new decimal?[cat.Count]
                };
                foreach (var item in g)
                {
                    var place = Date.Where(x => x.Key == Convert.ToDateTime(item.DATE).Date.ToString("dd/MM/yyyy")).Select(x => x.Value).SingleOrDefault();
                    report.data[place] = item.Value;
                }
                Listrep.Add(report);
            }
            Report_Decimal data = new Report_Decimal()
            {
                Categories = new List<string>(),
                Series = new List<Report_Series_D>()
            };
            data.Series.AddRange(Listrep);
            data.Categories.AddRange(cat);


            ///////////////////////////////

            List<Electricity_Report> bb = new List<Electricity_Report>();

            foreach (var item in res.GroupBy(x => x.Dep_Name))
            {
                Electricity_Report bk = new Electricity_Report();
                bk.name = item.Key;
                bk.y = item.Sum(x => x.Value);
                bb.Add(bk);
            }

            /////////////////////////////////

            var res1 = new PurchaseCombinedReport { BarChart = data, PieChart = bb };

            return PartialView(res1);
        }


    }
}