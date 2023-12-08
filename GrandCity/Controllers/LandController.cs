using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MeherEstateDevelopers.Controllers
{
    public class LandController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        // GET: Land

        /// <summary>
        /// Land Seller Parties
        /// </summary>
        public ActionResult LandSellers()
        {

            var res = db.Land_Seller_Party.Where(x => x.Deleted == false || x.Deleted == null).ToList();

            return View(res);
        }
        public ActionResult UpdateSellerDetails(long? id)
        {
            var res = db.Land_Seller_Party.Where(x => x.Id == id).FirstOrDefault();
            return PartialView(res);
        }
        public JsonResult NewLandSeller(string Seller_Name, string CNIC, string Mobile_No, string PartyNo, string images)
        {
            var res = db.Sp_Add_Seller_Data(Seller_Name, CNIC, Mobile_No, PartyNo, images).ToList();
            return Json(new { Status = true, Msg = "Try Again" });
        }
        public JsonResult UpdateSeller(long? Id, string SellerName, string MobileNo, string partyNo, string CNIC)
        {
            var res = db.Sp_Update_Seller_Details(Id, SellerName, MobileNo, partyNo, CNIC).ToString();
            return Json(new { Status = true, Msg = "Try Again" });
        }
        public JsonResult DeleteSellerData(long Id)
        {
            var res = db.Sp_Delete_Seller_Data(Id).ToString();
            return Json(new { Status = true, Msg = "Try Again" });
        }

        public ActionResult AddLandSellers()
        {
            return PartialView();
        }
        public JsonResult AddLandSeller(Land_Seller_Party sell)
        {
            return Json(true);
        }

        public JsonResult UpdateLandSeller(Land_Seller_Party sell)
        {
            return Json(true);
        }
        public JsonResult GetSeller_ForSelect(string s)
        {
            var res = db.Land_Seller_Party.Where(x => x.Name.Contains(s)).Select(x => new { text = x.Name, id = x.Id }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        /////////

        public ActionResult AddLandRecord()
        {
            ViewBag.Deal_No = "L-000001";
            return View();
        }
        [HttpPost]
        public JsonResult Add_LandRecord()
        {
            return Json(true);
        }
        public JsonResult GetSellers_ForSelect(string q)
        {
            var allsearch = db.Land_Seller_Party.Where(x => x.Name.Contains(q)).Select(x => new { Id = x.Id, text = x.Name }).ToList();
            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LandRecords()
        {
            var res = db.Sp_Get_LandRecords().ToList();
            return View(res);
        }
        //Seller Data
        public ActionResult SellerData()
        {
            return PartialView();
        }
        public JsonResult SaveLandDealDetails(List<Land_Record> LandRecordData, List<Land_Payment_Schedule> LandPaymentDateAmt, string SelectSeller_Name, Nullable<long> sellerId, Nullable<decimal> PricePerAcre, string dealDesc, string DeaLNo, Nullable<decimal> dealTotAmt)
        {
            try
            {
                var LandRecord = new XElement("LandData", LandRecordData.Select(x => new XElement("LandDataInfo",
                    new XAttribute("MozaNo", x.Moza_No),
                    (x.Khasra_No == null) ? null : new XAttribute("KhasraNo", x.Khasra_No),
                    (x.Khatoni == null) ? null : new XAttribute("KhatooniNo", x.Khatoni),
                    (x.Marla == null) ? null : new XAttribute("MarlaNo", x.Marla),
                    (x.Marraba_No == null) ? null : new XAttribute("MarabbaNo", x.Marraba_No),
                    (x.Kannal == null) ? null : new XAttribute("KannalNo", x.Kannal),
                    (x.SFT == null) ? null : new XAttribute("SquareFt", x.SFT),
                    (x.Remarks == null) ? null : new XAttribute("Remark", x.Remarks),
                    (x.Khewat == null) ? null : new XAttribute("KhewatNo", x.Khewat),
                    (x.Litigation == null) ? null : new XAttribute("Litigation", x.Litigation)))).ToString();

                var LandPayment = new XElement("LandPaymentSchedule", LandPaymentDateAmt.Select(x => new XElement("LandPaymentScheduleData",
                    new XAttribute("DueDate", x.Due_Date),
                    (x.Amount == null) ? null : new XAttribute("Amount", x.Amount)))).ToString();


                var res = db.Sp_Add_LandData_Details(LandRecord, LandPayment, DeaLNo, dealDesc, dealTotAmt, PricePerAcre, SelectSeller_Name, sellerId).ToList();

                //if (res.Count == LandRecordData.Count)
                //{
                //    return Json(new { Status = true, Msg = "Deals Done Succesfully" });
                //}
                //else
                //{
                //    return Json(new { Status = false, Msg = "EEEEEERRRRRRRRROOOOORRRRRRRR" });
                //}
                return Json(new { Status = true, Msg = "Deals Done Succesfully" });
            }
            catch (Exception ex)
            {
                EmailService e = new EmailService();
                string msg = ex.Message;
                return Json(new { Status = false, Msg = "Error in Your Land Deals" });
            }

        }

        public ActionResult LandRecordDetails(long dealid)
        {
            var res1 = db.Sp_Get_LandDealsDetails(dealid).ToList();
            var res2 = db.Sp_Get_LandPaymentScheduleDetails(dealid).ToList();
            var res3 = db.Sp_Get_LandRecordDetails(dealid).ToList();
            var res = new LandRecordDetails { LandDeals = res1, LandPayments = res2, LandRecords = res3 };
            return View(res);
        }

        public JsonResult UpdateLandRecord(long? deal_id, List<Land_Record> LandRecordData, List<Land_Payment_Schedule> LandPaymentDateAmt, string DeaLNo, string dealDesc, Nullable<decimal> dealTotAmt, Nullable<decimal> PricePerAcre, string SelectSeller_Name, Nullable<long> sellerId)
        {
            try
            {
                var LandRecord = new XElement("LandData", LandRecordData.Select(x => new XElement("LandDataInfo",
                    new XAttribute("MozaNo", x.Moza_No),
                    (x.Khasra_No == null) ? null : new XAttribute("KhasraNo", x.Khasra_No),
                    (x.Khatoni == null) ? null : new XAttribute("KhatooniNo", x.Khatoni),
                    (x.Marla == null) ? null : new XAttribute("MarlaNo", x.Marla),
                    (x.Marraba_No == null) ? null : new XAttribute("MarabbaNo", x.Marraba_No),
                    (x.Kannal == null) ? null : new XAttribute("KannalNo", x.Kannal),
                    (x.SFT == null) ? null : new XAttribute("SquareFt", x.SFT),
                    (x.Remarks == null) ? null : new XAttribute("Remark", x.Remarks),
                    (x.Khewat == null) ? null : new XAttribute("KhewatNo", x.Khewat),
                    (x.Litigation == null) ? null : new XAttribute("Litigation", x.Litigation)))).ToString();

                var LandPayment = new XElement("LandPaymentSchedule", LandPaymentDateAmt.Select(x => new XElement("LandPaymentScheduleData",
                    new XAttribute("DueDate", x.Due_Date),
                    (x.Amount == null) ? null : new XAttribute("Amount", x.Amount)))).ToString();


                var res = db.Sp_Update_Land_Record(deal_id, LandRecord, LandPayment, DeaLNo, dealDesc, dealTotAmt, PricePerAcre, SelectSeller_Name, sellerId).ToList();

                //if (res.Count == LandRecordData.Count)
                //{
                //    return Json(new { Status = true, Msg = "Deals Done Succesfully" });
                //}
                //else
                //{
                //    return Json(new { Status = false, Msg = "EEEEEERRRRRRRRROOOOORRRRRRRR" });
                //}
                return Json(new { Status = true, Msg = "Updated Succesfully" });
            }
            catch (Exception ex)
            {
                EmailService e = new EmailService();
                string msg = ex.Message;
                return Json(new { Status = false, Msg = "Error in Your Land Deals" });
            }

        }
        public ActionResult PaymentDetails()
        {
            var res = db.Sp_Get_LandPaymentDetails().ToList();
            return View(res);
        }
        /////

    }
}