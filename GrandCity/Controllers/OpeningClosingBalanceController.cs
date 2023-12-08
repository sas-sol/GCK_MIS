using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    public class OpeningClosingBalanceController : Controller
    {
        // GET: OpeningClosingBalance
        private MED_MISEntities db = new MED_MISEntities();

        public ActionResult Balances()
        {

            long userid = long.Parse(User.Identity.GetUserId());
            
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            
            var fy = this.GetFinancialYear();
            ViewBag.From = fy.Start;
            ViewBag.To = fy.End;
            var res = db.Sp_Get_CA_OpeningClosing_Balance(fy.Start, fy.End, COA_OpeningClosingBalance_Type.Opening.ToString(), 6, comp.Id ).ToList();
            foreach (var item in res)
            {
                var a = item.Text_ChartCode.Split('/');
                var b = db.Sp_Get_CA_Head_Param_Code("/"+a[1]+"/" + a[2] + "/").FirstOrDefault();
                item.MainAccount = b.Head;
            }
            return View(res);
        }
        public JsonResult EditOpeningBal(int Id, decimal? D_bal, decimal? C_Bal)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = this.GetFinancialYear();
            var res2 = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
            var res3 = db.Opening_Closing_Balance.Where(x => x.Balance_Type == COA_OpeningClosingBalance_Type.Opening.ToString() && x.Head_Id == Id && x.Head_Code == res2.Text_ChartCode && x.Fiscal_Start == res1.Start && x.Fiscal_End == res1.End).FirstOrDefault();
            if (res3 == null)
            {
                var a = db.Sp_Add_Head_Opening_Closing_Bal(D_bal, C_Bal, COA_OpeningClosingBalance_Type.Opening.ToString(), res1.End, res1.Start, DateTime.Now, res2.Text_ChartCode, Id, res2.Head, userid);
                return Json(new { Status = true, Msg = "Updated successfully." });
            }
            else
            {
                var a = db.Sp_Update_Opening_Balance(res1.Start, res1.End, D_bal, C_Bal, Id);
                return Json(new { Status = true, Msg = "Updated successfully." });
            }
        }

        private FinancialYear GetFinancialYear()
        {
            return new FinancialYear { Start = new DateTime(2022, 7, 1), End = new DateTime(2023, 6, 30) };
        }
    }
}