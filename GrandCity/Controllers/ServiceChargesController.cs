using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class ServiceChargesController : Controller
    {
        private string AccountingModuleFP = COA_Mapper_Modules.Files_Plots.ToString();
        private Grand_CityEntities db = new Grand_CityEntities();
        [NoDirectAccess] public ActionResult TypeofCharges()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddSurCharge(Service_Charges_Types sc)
        {
            if (sc.Type == "Square_Feet")
            {
                sc.Plot_Size = 0;
                sc.Commercial = "Commercial";
            }
            var res = Convert.ToBoolean(db.Sp_Add_ServiceCharges(sc.Charges_Name, sc.Rate, sc.Type, sc.Plot_Size).FirstOrDefault());
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Added New Service Charges " + sc.Charges_Name, "Create", "Activity_Record", ActivityType.Services.ToString(), userid);

            if (res)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult AllServiceCharges()
        {
            var res = db.Sp_Get_ServiceChargesTypeList().ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult ElecUnitDetails()
        {
            var res = db.Sp_Get_UnitDetails_Electricity().ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult UpdateRates(int Id)
        {
            var res = db.Sp_Get_ElectricUnitRate_Parameter(Id).SingleOrDefault();
            return PartialView(res);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult UpdateElectricRate(Electricity_Unit_Slabs eb)
        //{
        //    db.Sp_Update_ElectricUnitRate(eb.Id, eb.Start_Range, eb.End_Range, eb.Rate);
        //    return Json(true);
        //}
        [NoDirectAccess] public ActionResult Bills()
        {
            var res = db.Sp_Get_PlotsServiceChargesListRecord().ToList();
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View(res);
        }
        // commercial
        [NoDirectAccess] public ActionResult CommercialBills()
        {
            ViewBag.Project = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Commercial.ToString()).ToList(), "Id", "Project_Name");
            return View();
        }
        [NoDirectAccess] public ActionResult PlotsServiceChargesList(long Id)
        {
            var res1 = db.Sp_Get_PlotsServiceChargesList(Id).ToList();
            var rec = res1.Sum(x => x.Grand_Total);
            var recd = res1.Sum(x => x.Amount_Paid);
            var Rrec = res1.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Grand_Total);
            var Rrecd = res1.Where(x => x.Type == PlotType.Residential.ToString()).Sum(x => x.Amount_Paid);
            var Crec = res1.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Grand_Total);
            var Crecd = res1.Where(x => x.Type == PlotType.Commercial.ToString()).Sum(x => x.Amount_Paid);
            ViewBag.BlockId = Id;
            ViewBag.Block = res1.Select(x => x.Block).FirstOrDefault();
            var res = new PlotServiceChargesDetails { Receiable = rec, Received = recd, Resi_Receiable = Rrec, Resi_Received = Rrecd, Com_Receiable = Crec, Com_Received = Crecd, PlotsServiceChargesList = res1 };
            return PartialView(res);
        }
        // commercial
        //[NoDirectAccess] public ActionResult CommercialServiceChargesList(long Id)
        //{
        //    var res1 = db.Sp_Get_CommercialServiceChargesList(Id).ToList();
        //    var rec = res1.Sum(x => x.Grand_Total);
        //    var recd = res1.Sum(x => x.Amount_Paid);
        //    var Rrec = res1.Where(x => x.Type == Types.Shop.ToString()).Sum(x => x.Grand_Total);
        //    var Rrecd = res1.Where(x => x.Type == Types.Shop.ToString()).Sum(x => x.Amount_Paid);
        //    var Crec = res1.Where(x => x.Type == Types.Office.ToString()).Sum(x => x.Grand_Total);
        //    var Crecd = res1.Where(x => x.Type == Types.Office.ToString()).Sum(x => x.Amount_Paid);
        //    var Arec = res1.Where(x => x.Type == Types.Apartment.ToString()).Sum(x => x.Grand_Total);
        //    var Arecd = res1.Where(x => x.Type == Types.Apartment.ToString()).Sum(x => x.Amount_Paid);
        //    ViewBag.BlockId = Id;
        //    ViewBag.Block = res1.Select(x => x.Block).FirstOrDefault();
        //    var res = new PlotServiceChargesDetails { Receiable = rec, Received = recd, Resi_Receiable = Rrec,App_Receiable= Arec, App_Received= Arecd, Resi_Received = Rrecd, Com_Receiable = Crec, Com_Received = Crecd, ComServiceChargesList = res1 };
        //    return PartialView(res);
        //}
        // New Changed
        [NoDirectAccess] public ActionResult ElectricityBill()
        {
            //ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");

            var res1 = db.Sp_Get_PlotsElectricCharges_NewMeterReadinds().ToList();
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View(res1);
        }

        // commercial
        [NoDirectAccess] public ActionResult ElectricityBillCommercial()
        {
            ViewBag.Project = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Commercial.ToString()).ToList(), "Id", "Project_Name");
            return View();
        }
        [NoDirectAccess] public ActionResult PlotsElectricityList(long Id)
        {
            var res1 = db.Sp_Get_PlotsElectricCharges_ByBlock(Id).ToList();
            var rec = res1.Sum(x => x.Grand_Total);
            var recd = res1.Sum(x => x.Amount_Paid);
            var Rrec = res1.Where(x => x.Plot_Type == PlotType.Residential.ToString()).Sum(x => x.Grand_Total);
            var Rrecd = res1.Where(x => x.Plot_Type == PlotType.Residential.ToString()).Sum(x => x.Amount_Paid);
            var Crec = res1.Where(x => x.Plot_Type == PlotType.Commercial.ToString()).Sum(x => x.Grand_Total);
            var Crecd = res1.Where(x => x.Plot_Type == PlotType.Commercial.ToString()).Sum(x => x.Amount_Paid);
            ViewBag.BlockId = Id;
            ViewBag.Block = res1.Select(x => x.Block_Name).FirstOrDefault();
            var res = new PlotElectricDetails { Receiable = rec, Received = recd, Resi_Receiable = Rrec, Resi_Received = Rrecd, Com_Receiable = Crec, Com_Received = Crecd, PlotsElectricList = res1 };
            return PartialView(res);
        }
        //// commercial 
        //[NoDirectAccess] public ActionResult CommercialElectricityList(long Id)
        //{
        //    var res1 = db.Sp_Get_CommercialElectricCharges_ByProject(Id).ToList();
        //    var rec = res1.Sum(x => x.Grand_Total);
        //    var recd = res1.Sum(x => x.Amount_Paid);
        //    var Rrec = res1.Where(x => x.Plot_Type == Types.Shop.ToString()).Sum(x => x.Grand_Total);
        //    var Rrecd = res1.Where(x => x.Plot_Type == Types.Shop.ToString()).Sum(x => x.Amount_Paid);
        //    var Crec = res1.Where(x => x.Plot_Type == Types.Office.ToString()).Sum(x => x.Grand_Total);
        //    var Crecd = res1.Where(x => x.Plot_Type == Types.Office.ToString()).Sum(x => x.Amount_Paid);
        //    var Arec = res1.Where(x => x.Plot_Type == Types.Apartment.ToString()).Sum(x => x.Grand_Total);
        //    var Arecd = res1.Where(x => x.Plot_Type == Types.Apartment.ToString()).Sum(x => x.Amount_Paid);
        //    ViewBag.BlockId = Id;
        //    ViewBag.Block = res1.Select(x => x.Block_Name).FirstOrDefault();
        //    var res = new PlotElectricDetails { Receiable = rec, Received = recd, Resi_Receiable = Rrec, Resi_Received = Rrecd,App_Receiable=Arec,App_Received=Arecd, Com_Receiable = Crec, Com_Received = Crecd, CommercialElectricList = res1 };
        //    return PartialView(res);
        //}
        [NoDirectAccess] public ActionResult ServiceChargesBills(long Id)
        {
            var res = db.Sp_Get_PlotServiceChargeBills(Id).ToList();
            return PartialView(res);
        }
        public JsonResult ServiceChargesBillsRemarks(long Id)
        {
            var res = db.ServiceCharges_Bill.Where(x => x.Id == Id).Select(x => x.Remarks).SingleOrDefault();
            if (res != "")
            {
                return Json(res);
            }
            else
            {
                return Json(false);
            }

        }
        public JsonResult ElectricityBillsRemarks(long Id)
        {
            var res = db.Electricity_Bill.Where(x => x.Id == Id).Select(x => x.Remarks).SingleOrDefault();
            if (res != null)
            {
                return Json(res);
            }
            else
            {
                return Json(false);
            }

        }
        [NoDirectAccess] public ActionResult PlotServiceChargesList(long Id)
        {
            var res1 = db.Sp_Get_ServiceChargesTypeList().ToList();
            var res2 = db.Sp_Get_PlotServiceChargesTypeList(Id).ToList();
            ViewBag.PlotId = Id;
            var res = new Subscribed_Service_Charges { SCList = res1, Plot_SCList = res2 };
            return PartialView(res);
        }
        // commercail
        //[NoDirectAccess] public ActionResult ComServiceChargesList(long Id)
        //{
        //    var res1 = db.Sp_Get_ServiceChargesTypeList_Commercial().ToList();
        //    var res2 = db.Sp_Get_CommercialServiceChargesTypeList(Id).ToList();
        //    ViewBag.PlotId = Id;
        //    var res = new Subscribed_Service_Charges { SComList = res1, Com_SCList = res2 };
        //    return PartialView(res);
        //}
        public JsonResult Subscribe_Unsubscribe(long PlotId, int SC_Id, string Name, bool Status)
        {
            if (Status)
            {
                var res = Convert.ToBoolean(db.Sp_Add_SubscribeServiceCharges(PlotId, SC_Id, Name).FirstOrDefault());
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added New Subscribe charges ", "Create", "Activity_Record", ActivityType.Services.ToString(), PlotId);

                return Json(res);
            }
            else
            {
                var res = Convert.ToBoolean(db.Sp_Delete_UnSubscribeServiceCharges(PlotId, SC_Id).FirstOrDefault());
                return Json(res);
            }
        }
        //// commercial
        //public JsonResult Subscribe_UnsubscribeCOmmercial(long PlotId, int SC_Id, string Name, bool Status)
        //{
        //    if (Status)
        //    {
        //        var res = Convert.ToBoolean(db.Sp_Add_SubscribeServiceCharges_Commercial(PlotId, SC_Id, Name).FirstOrDefault());
        //        return Json(res);
        //    }
        //    else
        //    {
        //        var res = Convert.ToBoolean(db.Sp_Delete_UnSubscribeServiceCharges_Commercial(PlotId, SC_Id).FirstOrDefault());
        //        return Json(res);
        //    }
        //}
        [NoDirectAccess] public ActionResult CurrentMonthBill(long PlotId)
        {
            var res = db.Sp_Get_PlotServiceChargesTypeList(PlotId).ToList();
            ViewBag.Plot_Id = PlotId;
            return PartialView(res);
        }
        //// commercial
        //[NoDirectAccess] public ActionResult CurrentMonthBillCommercial(long PlotId)
        //{
        //    var res = db.Sp_Get_CommercialServiceChargesTypeList(PlotId).ToList();
        //    ViewBag.Plot_Id = PlotId;
        //    return PartialView(res);
        //}
        public JsonResult RegenrateBill(long? Plot_Id, decimal Arrears, string Remarks)
        {

            var AmountPaid = db.ServiceCharges_Bill.Where(x => x.Plot_Id == Plot_Id).OrderByDescending(x => x.Id).FirstOrDefault().Amount_Paid;
            if (AmountPaid == Convert.ToDecimal(0.00) || AmountPaid is null)
            {
                //var per = db.Fine_And_Penalties.Where(x => x.Fine_Type == FineAndPenaltiesTypes.ServiceChargesFine.ToString()).Select(x => x.Fine_Percentage).LastOrDefault();
                long userid = long.Parse(User.Identity.GetUserId());
                var nam = db.Users.Find(userid).Name;
                Remarks = Remarks + "\n" + nam + "\n";
                List<Service_Charges_Details> scdl = new List<Service_Charges_Details>();

                var res1 = db.Sp_Get_PlotDetailData(Plot_Id).SingleOrDefault();
                var res2 = db.Sp_Get_PlotServiceChargesTypeList(Plot_Id).ToList();
                var res3 = db.Sp_Get_PlotLastOwner(Plot_Id).ToList();
                decimal? Totalamt = 0;
                foreach (var item in res2)
                {
                    //if (item.Develop_Status == PlotDevelopStatus.Constructed.ToString())
                    //{
                    decimal? charges = 0;
                    if (item.Type == "Constant")
                    {
                        charges = item.Rate;
                    }
                    else if (item.Type == "Size")
                    {
                        charges = item.Rate;
                    }
                    else
                    {
                        charges = item.Rate * item.Plot_Size;
                    }
                    Service_Charges_Details scd = new Service_Charges_Details()
                    {
                        Plot_Id = Convert.ToInt64(Plot_Id),
                        Service_Charges = item.Service_Charge_Name,
                        Rate = Convert.ToDecimal(item.Rate),
                        Charges = Convert.ToDecimal(charges),
                        Type = item.Type,
                    };
                    scdl.Add(scd);
                    Totalamt += charges;
                }
                var details = new XElement("ServiceCharges", scdl.Select(x => new XElement("Details",
                   new XAttribute("Charges", x.Charges),
                   new XAttribute("Rate", x.Rate),
                   new XAttribute("Service_Charges", x.Service_Charges),
                   new XAttribute("Type", x.Type)
                   ))).ToString();
                string name = String.Join(",", res3.Select(x => x.Name));


                db.Sp_Add_Activity(userid, "Re Generated Bill ", "Create", "Activity_Record", ActivityType.Services.ToString(), Plot_Id);

                var res = db.Sp_Add_PlotServiceChargeBills_Regen(Totalamt, res1.Block_Name, name, res1.Phase_Name, Plot_Id, res1.Plot_No, res1.Type, Arrears, Remarks, 10, ((Totalamt * 10) / 100), DateTime.UtcNow.AddDays(10), (Totalamt + ((Totalamt * 10) / 100)), (Arrears + (Totalamt + ((Totalamt * 10) / 100))), details).FirstOrDefault();
                return Json(res);
            }
            else
            {
                return Json(false);
            }


        }
        //[NoDirectAccess] public ActionResult FineData()
        //{
        //    var res = db.Fine_And_Penalties.Where(x => x.Fine_Type == FineAndPenaltiesTypes.ServiceChargesFine.ToString()).ToList();
        //    return PartialView(res);
        //}
        //public JsonResult FineInsertion(int Percentage)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res = db.Sp_Add_Fine_Or_Panelties(Percentage, FineAndPenaltiesTypes.ServiceChargesFine.ToString(), userid);
        //    return Json(true);
        //}
        // Commercial
        //public JsonResult RegenrateBillCommercial(long? Plot_Id, decimal? Arrears, string Remarks)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var nam = db.Users.Find(userid).Name;
        //    Remarks = Remarks + "\n" + nam + "\n";
        //    List<Service_Charges_Details> scdl = new List<Service_Charges_Details>();

        //    var res1 = db.Sp_Get_CommercialDetailData(Plot_Id).SingleOrDefault();  /*db.Sp_Get_PlotDetailData(Plot_Id).SingleOrDefault();*/
        //    var res2 = db.Sp_Get_CommercialServiceChargesTypeList(Plot_Id).ToList();
        //    var res3 = db.Sp_Get_CommercialLastOwner(Plot_Id).SingleOrDefault();

        //    decimal? Totalamt = 0;
        //    foreach (var item in res2)
        //    {

        //        decimal? charges = 0;
        //        if (item.Type == "Square_Feet")
        //        {
        //            charges = item.Rate * item.Area;
        //        }
        //        else
        //        {
        //            charges = item.Rate;
        //        }
        //        Service_Charges_Details scd = new Service_Charges_Details()
        //        {
        //            Plot_Id = Convert.ToInt64(Plot_Id),
        //            Service_Charges = item.Service_Charge_Name,
        //            Rate = Convert.ToDecimal(item.Rate),
        //            Charges = Convert.ToDecimal(charges),
        //            Type = item.Type,
        //        };
        //        scdl.Add(scd);
        //        Totalamt += charges;

        //    }
        //    var details = new XElement("ServiceCharges", scdl.Select(x => new XElement("Details",
        //       new XAttribute("Charges", x.Charges),
        //       new XAttribute("Rate", x.Rate),
        //       new XAttribute("Service_Charges", x.Service_Charges),
        //       new XAttribute("Type", x.Type)
        //       ))).ToString();
        //    string name = (res3 == null) ? "" : res3.Name;
        //    var res = db.Sp_Add_PlotServiceChargeBills_Regen(Totalamt, res1.Module, name, res1.Project_Name, Plot_Id, Convert.ToString(res1.Plot_Id), res1.Module, Arrears, Remarks, details).FirstOrDefault();
        //    return Json(res);
        //}
        //[NoDirectAccess] public ActionResult PlotSubscribedServiceCharges(long Id)
        //{
        //    var res = db.Sp_Get_PlotServiceChargesTypeList(Id).ToList();
        //    ViewBag.PlotId = Id;
        //    return PartialView(res);
        //}
        public void PlotIdsSC() // to generate new bills
        {
            var res = db.Sp_Get_PlotIdsServiceCharges().ToList();
            foreach (var item in res)
            {
                this.GenerateServiceChargesBill(item);
            }

        }
        public JsonResult GenerateServiceChargesBill(long? Plot_Id)
        {
            List<Service_Charges_Details> scdl = new List<Service_Charges_Details>();
            DateTime dt = DateTime.Now;
            var res1 = db.Sp_Get_PlotData(Plot_Id).SingleOrDefault();
            if (res1 is null)
            {
                return Json(false);
            }
            if (res1.Block_Name == "Tahir" || res1.Block_Name == "Charagh" || res1.Block_Name == "Sher Zaman" || res1.Block_Name == "Faisal" || res1.Block_Name == "Badar")
            {
                return Json(false);
            }
            var res2 = db.Sp_Get_PlotServiceChargesTypeList(Plot_Id).ToList();
            var res3 = db.Sp_Get_PlotLastOwner(Plot_Id).ToList();
            var res4 = db.Sp_Get_PlotLastBill_SC(Plot_Id).SingleOrDefault();
            var bilno = "SC-" + db.Sp_Get_ReceiptNo("Service Charges").FirstOrDefault();
            decimal? Totalamt = 0;
            var arrearAmt = (res4 == null) ? 0 : res4.Remaining;
            var inst = db.ServiceChargesInstallments.Where(x => x.Plot_Id == res1.Id && x.ServiceType == "ServiceCharges" && x.Status == "Approved").ToList();
            if (inst != null)
            {
                foreach (var v in inst)
                {
                    var __amt = db.SvcCharges_Installments_Structure.Where(x => x.InstallmentId == v.Id && x.Installment_Month.Value.Month == dt.Month && x.Installment_Month.Value.Year == dt.Year).Select(x => new { x.Installment_Amount, x.Id }).FirstOrDefault();
                    if (__amt != null)
                    {
                        var instStatUpdtd = db.Sp_Update_SVCInstallemtStatus(__amt.Id);
                        arrearAmt += (__amt.Installment_Amount is null) ? 0 : __amt.Installment_Amount;
                    }
                    else
                    {
                        arrearAmt += 0;
                    }
                }
            }
            arrearAmt = (arrearAmt == null) ? 0 : arrearAmt;
            foreach (var item in res2)
            {

                decimal? charges = 0;
                if (item.Type == "Constant")
                {
                    charges = item.Rate;
                }
                else if (item.Type == "Size")
                {
                    charges = item.Rate;
                }
                else
                {
                    charges = item.Rate * item.Plot_Size;
                }
                Service_Charges_Details scd = new Service_Charges_Details()
                {
                    Plot_Id = Convert.ToInt64(Plot_Id),
                    Service_Charges = item.Service_Charge_Name,
                    Rate = Convert.ToDecimal(item.Rate),
                    Charges = Convert.ToDecimal(charges),
                    Type = item.Type,
                };
                scdl.Add(scd);
                Totalamt += charges;

            }
            var details = new XElement("ServiceCharges", scdl.Select(x => new XElement("Details",
               new XAttribute("Charges", x.Charges),
               new XAttribute("Rate", x.Rate),
               new XAttribute("Service_Charges", x.Service_Charges),
               new XAttribute("Type", x.Type),
               new XAttribute("billNo", bilno)
               ))).ToString();
            string name = (res3 == null) ? "" : string.Join(" , ", res3.Select(x => x.Name));
            var res = db.Sp_Add_PlotServiceChargeBills(Totalamt, res1.Block_Name, name, res1.Phase_Name, Plot_Id, res1.Plot_No, res1.Type, arrearAmt, "", 13, ((Totalamt * 13) / 100), DateTime.UtcNow.AddDays(10), (Totalamt + ((Totalamt * 13) / 100)), (arrearAmt + (Totalamt + ((Totalamt * 13) / 100))), bilno, details).FirstOrDefault();
            if (res != 0 && Totalamt != null)
            {
                System.Threading.Tasks.Task smsSender = new System.Threading.Tasks.Task(() =>
                {
                    foreach (var v in res3)
                    {
                        if (!string.IsNullOrEmpty(v.Mobile_1) && !string.IsNullOrWhiteSpace(v.Mobile_1))
                        {
                            NotifyUserAboutBillViaSMS(v.Name, v.Mobile_1, Totalamt.ToString(), DateTime.UtcNow.AddDays(10), "Service Charges", res1.Plot_No + " " + res1.Block_Name);
                        }
                        else if (!string.IsNullOrEmpty(v.Mobile_2) && !string.IsNullOrWhiteSpace(v.Mobile_2))
                        {
                            NotifyUserAboutBillViaSMS(v.Name, v.Mobile_2, Totalamt.ToString(), DateTime.UtcNow.AddDays(10), "Service Charges", res1.Plot_No + " " + res1.Block_Name);
                        }
                    }
                });
                smsSender.Start();
            }
            //long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Add_Activity(userid, "Generated Services Charges Bills ", "Create", "Activity_Record", ActivityType.Services.ToString(), Plot_Id);

            return Json(true);
        }
        //// commercial 
        //public void ComIdsSC() 
        //{
        //    var res = db.Sp_Get_CommercialIdsServiceCharges().ToList();
        //    foreach (var item in res)
        //    {
        //        this.GenerateServiceChargesBillCommercial(item);
        //    }

        //}
        // commercial
        //public JsonResult GenerateServiceChargesBillCommercial(long? Plot_Id)
        //{
        //    List<Service_Charges_Details> scdl = new List<Service_Charges_Details>();

        //    var res1 = db.Sp_Get_CommercialDetailData(Plot_Id).SingleOrDefault();  /*db.Sp_Get_PlotDetailData(Plot_Id).SingleOrDefault();*/
        //    var res2 = db.Sp_Get_CommercialServiceChargesTypeList(Plot_Id).ToList();
        //    var res3 = db.Sp_Get_CommercialLastOwner(Plot_Id).SingleOrDefault();
        //    var res4 = db.Sp_Get_PlotLastBill_SC(Plot_Id).SingleOrDefault();
        //    decimal? Totalamt = 0;
        //    var arrearAmt = (res4 == null) ? 0 : res4.Remaining;
        //    foreach (var item in res2)
        //    {

        //        decimal? charges = 0;
        //        if (item.Type == "Square_Feet")
        //        {
        //            charges = item.Rate*item.Area;
        //        }

        //        else
        //        {
        //            charges = item.Rate ;
        //        }
        //        Service_Charges_Details scd = new Service_Charges_Details()
        //        {
        //            Plot_Id = Convert.ToInt64(Plot_Id),
        //            Service_Charges = item.Service_Charge_Name,
        //            Rate = Convert.ToDecimal(item.Rate),
        //            Charges = Convert.ToDecimal(charges),
        //            Type = item.Type,
        //        };
        //        scdl.Add(scd);
        //        Totalamt += charges;

        //    }
        //    var details = new XElement("ServiceCharges", scdl.Select(x => new XElement("Details",
        //       new XAttribute("Charges", x.Charges),
        //       new XAttribute("Rate", x.Rate),
        //       new XAttribute("Service_Charges", x.Service_Charges),
        //       new XAttribute("Type", x.Type)
        //       ))).ToString();
        //    string name = (res3 == null) ? "" : res3.Name;
        //    var res = db.Sp_Add_PlotServiceChargeBills(Totalamt, res1.Module, name, res1.Project_Name, Plot_Id, Convert.ToString(res1.Plot_Id), res1.Module, arrearAmt, "", details).FirstOrDefault();
        //    return Json(true);
        //}
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        //[NoDirectAccess] public ActionResult Download(long BlockId)
        //{
        //    var res1 = db.Sp_Get_PlotsServiceCharges_ByBlock(BlockId).ToList();
        //    var res = (from x in res1
        //               group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase } into g
        //               select new ServiceChargesBill
        //               {
        //                   Name = g.Key.Name,
        //                   Address = g.Key.Plot_No + " " + g.Key.Block + " " + g.Key.Plot_Type + " " + g.Key.Phase,
        //                   BillDeta = g.Select(x => new Service_Charges_Details
        //                   {
        //                       Charges = x.Charges,
        //                       Plot_Id = x.Plot_Id,
        //                       Rate = x.Rate,
        //                       Type = x.Type,
        //                       Service_Charges = x.Service_Charges
        //                   }).ToList()
        //               }).ToList();
        //    string Htmldata = RenderPartialViewToString("ServiceChargesBillsByBlock", res);

        //    string htmlString = Htmldata;
        //    string baseUrl = "";

        //    string pdf_page_size = "A4";
        //    PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
        //        pdf_page_size, true);

        //    string pdf_orientation = "Portrait";
        //    PdfPageOrientation pdfOrientation =
        //        (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
        //        pdf_orientation, true);

        //    int webPageWidth = 700;
        //    int webPageHeight = 0;
        //    // instantiate a html to pdf converter object
        //    HtmlToPdf converter = new HtmlToPdf();
        //    // set converter options
        //    converter.Options.PdfPageSize = pageSize;
        //    converter.Options.PdfPageOrientation = pdfOrientation;
        //    converter.Options.WebPageWidth = webPageWidth;
        //    converter.Options.WebPageHeight = webPageHeight;
        //    // create a new pdf document converting an url
        //    PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
        //    // save pdf document
        //    byte[] pdf = doc.Save();
        //    // close pdf document
        //    doc.Close();
        //    // return resulted pdf document
        //    FileResult fileResult = new FileContentResult(pdf, "application/pdf");
        //    fileResult.FileDownloadName = "Document.pdf";
        //    return fileResult;
        //}
        [NoDirectAccess] public ActionResult MeterDetails(long PlotId, int SC_Source)
        {
            if (SC_Source == 1)
            {
                //var res = from x in db.Electricity_Bill 
            }
            else if (SC_Source == 2)
            {

            }
            return PartialView();
        }
        [NoDirectAccess] public ActionResult MeterReading()
        {
            ViewBag.PlotMeters = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }

        // commercial
        [NoDirectAccess] public ActionResult MeterReadingCommercial()
        {
            ViewBag.Project = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Commercial.ToString()).ToList(), "Id", "Project_Name");
            return View();
        }
        [NoDirectAccess] public ActionResult MeterReadingDetails()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult PlotsMeterList(long Id)
        {
            var res = db.Sp_Get_PlotsMeterDetails_NewMeterReadings(Id).ToList();
            return PartialView(res);
        }

        //// commercial
        //[NoDirectAccess] public ActionResult PlotsMeterListCommercial(long? Id) 
        //{
        //    var res = db.Sp_Get_CommercialMeterDetails(Id).ToList();
        //    return PartialView(res);
        //}
        [NoDirectAccess] public ActionResult AddCurrMonthReading()
        {

            return PartialView();
        }
        [NoDirectAccess] public ActionResult AddMeterNo()
        {
            return PartialView();
        }
        /// <summary>
        /// updation code
        /// </summary>
        /// 
        public void GenerateElectricBillInMeterReading()
        {
            try
            {
                var res = db.Sp_Get_PlotsMeterLastReading().ToList();

                foreach (var item in res)
                {
                    var res1 = db.Sp_Get_PlotDetailData(item.Plot_Id).SingleOrDefault();
                    var res2 = db.Sp_Get_PlotLastOwner(item.Plot_Id).ToList();
                    var lastBill = db.Sp_Get_PlotLastElectricityBills(item.Plot_Id.ToString()).ToList().Take(1).FirstOrDefault();
                    item.Block_Name = res1.Block_Name;
                    item.Name = string.Join(",", res2.Select(x => x.Name));

                    string Month = DateTime.Now.Year + "-" + DateTime.Now.Month;
                    DateTime currDate = DateTime.Now;

                    //processing installments
                    List<ServiceChargesInstallment> insts = new List<ServiceChargesInstallment>();
                    if (lastBill != null)
                    {
                        insts = db.ServiceChargesInstallments.Where(x => x.Plot_Id == item.Plot_Id && x.Status == ServiceChargesInstallmentsStatus.Approved.ToString() && x.ServiceType == ServiceType.Electricity.ToString() && x.Meter_Num == lastBill.Meter_No).OrderByDescending(x => x.Create_DateTime).ToList();
                    }
                    else
                    {
                        insts = db.ServiceChargesInstallments.Where(x => x.Plot_Id == item.Plot_Id && x.Status == ServiceChargesInstallmentsStatus.Approved.ToString() && x.ServiceType == ServiceType.Electricity.ToString()).OrderByDescending(x => x.Create_DateTime).ToList();
                    }
                    decimal instToAdd = 0;
                    foreach (var inst in insts)
                    {
                        if (!(inst is null))
                        {
                            var inst_dets = db.SvcCharges_Installments_Structure.Where(x => x.InstallmentId == inst.Id && x.Status == MonthlyInstallmentStatus.Pending.ToString()).ToList();
                            var __inst_amt = inst_dets.Where(x => x.Installment_Month.Value.Month == currDate.Month && x.Installment_Month.Value.Year == currDate.Year).Select(x => new { x.Installment_Amount, x.Id }).FirstOrDefault();
                            if (!(__inst_amt is null))
                            {
                                var instStatUpdt = db.Sp_Update_SVCInstallemtStatus(__inst_amt.Id);
                                instToAdd += (decimal)__inst_amt.Installment_Amount;
                            }
                        }
                    }
                    if (item.Month == Month)
                    {
                        continue;
                    }
                    var forRate = db.Plot_ElectricityMeters.Where(x => x.Meter_No == item.Meter_No).FirstOrDefault().Meter_Type;
                    var rate = (forRate == PlotType.Commercial.ToString()) ? 30 : 16;
                    var Pre = (item.Previous_Reading == null || item.Previous_Reading == 0) ? 0 : item.Previous_Reading;
                    var arr = (item.Arrears == null || item.Arrears == 0) ? 0 : item.Arrears;
                    var Rem = (item.Remaining == null || item.Remaining == 0) ? 0 : item.Remaining;


                    //processing Meter change
                    var metChg = db.Electricity_Meter_Reqs.Where(x => x.Plot_Id == item.Plot_Id && (x.Billed == null) && x.Approval_Status == ServiceChargesInstallmentsStatus.Approved.ToString()).FirstOrDefault();
                    if (metChg is null)
                    {
                        db.Sp_Add_MeterReadings(Pre, Rem + instToAdd, item.Plot_Id, item.Name, item.Plot_No, item.Block_Name, forRate, Pre, 0, rate, item.Meter_Id, item.Meter_No);
                    }
                    else
                    {
                        var newMet = db.Plot_ElectricityMeters.Add(new Plot_ElectricityMeters
                        {
                            Meter_No = metChg.New_Meter_No,
                            Plot_Id = item.Plot_Id
                        });
                        db.SaveChanges();
                        db.Sp_Update_MeterReq_Replaced(metChg.Id);
                        db.Sp_Add_MeterReadings(metChg.New_Mtr_Curr_Rdng, (Rem + instToAdd + metChg.Old_Meter_Amount), item.Plot_Id, item.Name, item.Plot_No, item.Block_Name, forRate, metChg.New_Mtr_Curr_Rdng, 0, rate, newMet.Id, metChg.New_Meter_No);
                    }
                }
            }
            catch (Exception ex)
            {
                EmailService e = new EmailService();
                e.SendEmail(ex.StackTrace, "taimoor@sasystems.solutions", "ServiceChargesController GenerateMeterBillInReading Exception");
            }
        }

        [NoDirectAccess] public ActionResult NewPlotsMeterList(long Id)
        {
            ViewBag.BlockId = Id;
            var res = db.Sp_Get_PlotsMeterDetails_NewMeterReadings(Id).ToList();
            var inf = db.ServiceChargesPermissions.Where(x => x.Created_Date.Value.Month == DateTime.Now.Month && x.For_ModuleId == Id).OrderByDescending(x => x.Id).FirstOrDefault();
            bool? stat = false;
            if (!(inf is null))
            {
                stat = inf.Status;
            }
            return PartialView(new MeterReadingViewDetails { ManualPermissionGranted = stat, MeterReadings = res });
        }
        public JsonResult UpdateMeterReadings(long Id, long Reading, int Units)
        {
            var res = db.Plot_Update_NewMeterReading(Id, Reading, Units);
            return Json(true);
        }

        [NoDirectAccess] public ActionResult NewMeterReading()
        {
            ViewBag.PlotMeters = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }

        public JsonResult GetUpdatedMeterReadings(long Id)
        {
            var res = db.Sp_Get_NewMeterReading(Id).ToList();
            foreach (var item in res)
            {
                var forRate = db.Plot_ElectricityMeters.Where(x => x.Meter_No == item.Meter_No).FirstOrDefault().Meter_Type;
                var slabs = db.Electricity_Unit_Slabs.Where(x => x.Plot_Type == item.Plot_Type).ToList();

                List<Electricity_Bill_Details> ebd = new List<Electricity_Bill_Details>();
                int? Rem_Units = item.Units;
                foreach (var slab in slabs)
                {
                    if (slab.End_Range == null) // this means last range
                    {
                        if (Rem_Units <= 0)
                        {
                            break;
                        }
                        Electricity_Bill_Details e = new Electricity_Bill_Details()
                        {
                            Rate = slab.Rate,
                            Units = Rem_Units,
                            Amount = slab.Rate * Rem_Units,
                            Slab_Id = slab.Id,
                            Start_Range = slab.Start_Range,
                            End_Range = slab.End_Range
                        };
                        ebd.Add(e);
                    }
                    else if (item.Units >= slab.Start_Range && item.Units <= slab.End_Range)
                    {
                        if (Rem_Units <= 0)
                        {
                            break;
                        }
                        Electricity_Bill_Details e = new Electricity_Bill_Details()
                        {
                            Rate = slab.Rate,
                            Units = Rem_Units,
                            Amount = slab.Rate * Rem_Units,
                            Slab_Id = slab.Id,
                            Start_Range = slab.Start_Range,
                            End_Range = slab.End_Range
                        };
                        ebd.Add(e);

                        var a = slab.End_Range - slab.Start_Range + 1;
                        Rem_Units = Rem_Units - a;
                    }
                    else
                    {
                        if (Rem_Units <= 0)
                        {
                            break;
                        }
                        var a = slab.End_Range - slab.Start_Range + 1;
                        Rem_Units = Rem_Units - a;
                        Electricity_Bill_Details e = new Electricity_Bill_Details()
                        {
                            Rate = slab.Rate,
                            Units = a,
                            Amount = slab.Rate * a,
                            Slab_Id = slab.Id,
                            Start_Range = slab.Start_Range,
                            End_Range = slab.End_Range
                        };
                        ebd.Add(e);
                    }
                }

                var TotalAmouont = ebd.Sum(x => x.Amount);
                var grtot = TotalAmouont + item.Arrears;

                var details = new XElement("ElectricCharges", ebd.Select(x => new XElement("Details",
                 new XAttribute("Amount", x.Amount),
                 (x.End_Range is null) ? null : new XAttribute("End_Range", x.End_Range),
                 new XAttribute("Rate", x.Rate),
                 new XAttribute("Slab_Id", x.Slab_Id),
                 new XAttribute("Start_Range", x.Start_Range),
                 new XAttribute("Units", x.Units)
                 ))).ToString();
                db.Sp_Add_ElectricityBill(item.Previous_Reading, TotalAmouont, grtot, item.Plot_Id, item.Name, item.Plot_No, item.Block, forRate, item.Current_Reading, item.Units, 0, 0, item.Meter_Id, item.Meter_No, item.Arrears, 13, ((TotalAmouont * 13) / 100), DateTime.UtcNow.AddDays(10), TotalAmouont + ((TotalAmouont * 13) / 100), item.Arrears + (TotalAmouont + ((TotalAmouont * 13) / 100)), details);
            }
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Updated Meter Reading", "Update", "Activity_Record", ActivityType.Services.ToString(), Id);

            return Json(true);
        }
        //[NoDirectAccess] public ActionResult AllBlockReadingsTypeWise(string Type)
        //{
        //    ViewBag.Type = Type;
        //    var res = db.Sp_Get_PlotsElectricCharges_NewMeterReadinds_Type_Parameter(Type).ToList();
        //    return PartialView(res);
        //}
        [NoDirectAccess] public ActionResult BillHistory(long PlotId)
        {
            var res = db.Sp_Get_PlotsElectricCharges_NewMeterReadinds_PlotId_Parameter(PlotId).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult ElectricityCharges_Short(DateTime? month, DateTime? year, long?[] blockid)
        {
            List<Sp_Get_Report_ElectricityCharges_Result> res3 = new List<Sp_Get_Report_ElectricityCharges_Result>();
            List<string> blocklist = new List<string>();

            if (blockid != null)
            {
                foreach (long index in blockid)
                {
                    var res13 = db.Sp_Get_Report_ElectricityCharges(month, year, index).ToList();
                    res3.AddRange(res13);
                    var list = res13.Select(x => x.Block).FirstOrDefault();
                    blocklist.Add(list);
                }
            }
            else
            {
                var res1 = db.Sp_Get_Report_ElectricityCharges(month, year, null).ToList();
                res3.AddRange(res1);
            }



            if (blockid != null)
            {
                ViewBag.Block = String.Join(",", blocklist.Cast<string>().ToArray()); ;
            }
            else
            {
                ViewBag.meherestatedevelopers = "Meher Estate Developers";
            }

            ViewBag.blockid = blockid;

            var res = new ElectricityChargesDetails { ElectricityChargesList = res3 };
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult ServiceCharges_Short(DateTime? month, DateTime? year, long?[] blockid)
        {
            List<Sp_Get_Report_ServiceCharges_Result> res3 = new List<Sp_Get_Report_ServiceCharges_Result>();
            List<string> blocklist = new List<string>();
            if (blockid != null)
            {
                foreach (long index in blockid)
                {
                    var res13 = db.Sp_Get_Report_ServiceCharges(month, year, index).ToList<Sp_Get_Report_ServiceCharges_Result>();
                    res3.AddRange(res13);
                    var list = res13.Select(x => x.Block).FirstOrDefault();
                    blocklist.Add(list);
                }
            }
            else
            {
                var res1 = db.Sp_Get_Report_ServiceCharges(month, year, null).ToList<Sp_Get_Report_ServiceCharges_Result>();
                res3.AddRange(res1);
            }


            if (blockid != null)
            {
                ViewBag.Block = String.Join(",", blocklist.Cast<string>().ToArray()); ;
            }
            else
            {
                ViewBag.meherestatedevelopers = "Meher Estate Developers";
            }
            var res = new ElectricityChargesDetails { ServiceChargesList = res3 };
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult ElectricityBillByTypeWise(long BlockId, string Type)
        {
            var res = db.Sp_Get_PlotsElectricCharges_NewMeterReadinds_Type_Parameter(Type, BlockId).ToList();
            List<ElectricityBillViewTypeWise> vw = new List<ElectricityBillViewTypeWise>();

            foreach (var v in res)
            {
                string metNo = v.Meter_No;
                var hist = db.Electricity_Bill.Where(x => x.Meter_No == metNo).OrderByDescending(x => x.Month).Take(6).Select(x => new BillHistoryItem { Amount = x.Net_Total.ToString(), BillMonth = x.Month, UnitsConsumed = x.Units, AmountPaid = x.Amount_Paid.ToString() }).ToList();
                var details = db.Electricity_Bill_Details.Where(x => x.Bill_Id == v.Id).ToList();

                vw.Add(new ElectricityBillViewTypeWise
                {
                    CurrBill = v,
                    PastBills = hist,
                    CurrBillDetails = details
                });
            }

            return View(vw);
        }
        // service charges type with block combine
        [NoDirectAccess] public ActionResult ServicechargesBillByTypeWise(long BlockId, string Type)
        {

            var res1 = db.Sp_Get_ServiceCharges_Type_Parameter(Type, BlockId).ToList();
            var res = (from x in res1
                       group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase, x.Arrears, x.Amount_Paid, x.Net_Total, x.Fine_Amount, x.duedate, x.Bill_No, x.Plot_Size } into g
                       select new ServiceChargesBill
                       {
                           Name = g.Key.Name,
                           Address = g.Key.Plot_No + " " + g.Key.Block + " Block " + g.Key.Plot_Type + " - " + g.Key.Plot_Size,
                           Arrears = g.Key.Arrears,
                           Amount_Paid = g.Key.Amount_Paid,
                           After_Due_Date_Amount = g.Key.Net_Total,
                           FineAmount = g.Key.Fine_Amount,
                           Due_Date = Convert.ToDateTime(g.Key.duedate),
                           Bill_No = g.Key.Bill_No,
                           Plot_No = g.Key.Plot_No,
                           Plot_Size = g.Key.Plot_Size,
                           BillDeta = g.Select(x => new Service_Charges_Details
                           {
                               Charges = x.Charges,
                               Plot_Id = x.Plot_Id,
                               Rate = x.Rate,
                               Type = x.Type,
                               Service_Charges = x.Service_Charges
                           }).ToList(),

                       }).ToList();
            var res2 = db.Sp_Get_PlotsElectricCharges_ByBlock_Type_Parameter(Type, BlockId).ToList();
            var res11 = res2.Select(x => new ServiceChargesBill
            {
                Arrears = x.Arrears,
                Block_Name = x.Block_Name,
                Current_Reading = x.Current_Reading,
                Meter_No = x.Meter_No,
                Month = x.Month,
                Name = x.Name,
                Plot_Id = x.Plot_Id,
                Plot_No = x.Plot_No,
                Plot_Type = x.Plot_Type,
                Previous_Reading = x.Previous_Reading,
                Rate = x.Rate,
                Total_Amount = x.Total_Amount,
                Units = x.Units,
                Amount_Paid = x.Amount_Paid,
                Fine_Percentage = x.Fine_Percentage,
                Fine_Amount = x.Fine_Amount,
                Due_Date = x.duedate,
                Due_Date_Amount = x.Due_Date_Amount,
                Net_Total = x.Net_Total
            }).ToList();
            var FinalResult = new ServiceChargesBill { ElectricityData = res11, ServiceChargesData = res };
            //var res = db.Sp_Get_ServiceCharges_Type_Parameter(Type, BlockId).ToList();
            //var res1= db.Sp_Get_PlotsElectricCharges_ByBlock_Type_Parameter(Type, BlockId).ToList();
            return View(FinalResult);
        }
        /// <summary>
        /// NEW Meter Reading End
        /// </summary>
        //public void GenerateElectricBill()
        //{
        //    var res = db.Sp_Get_PlotsMeterLastReading().ToList();
        //    foreach (var item in res)
        //    {
        //        string Month = DateTime.Now.Year + "-" + DateTime.Now.Month;
        //        if (item.Month == Month)
        //        {
        //            continue;
        //        }
        //        var forRate = db.Plot_ElectricityMeters.Where(x => x.Meter_No == item.Meter_No).FirstOrDefault().Meter_Type;
        //        var rate = (forRate == PlotType.Commercial.ToString()) ? 30 : 16;
        //        var Pre = (item.Previous_Reading == null || item.Previous_Reading == 0) ? 0 : item.Previous_Reading;
        //        var Rem = (item.Remaining == null || item.Remaining == 0) ? 0 : item.Remaining;
        //        //db.Sp_Add_ElectricityBill(Pre, item.Plot_Id,item.Name,item.Plot_No,item.Block_Name,forRate, 0, 0, rate, 0, item.Meter_Id,item.Meter_No, Rem);
        //    }
        //}
        //// Commercial
        //public void GenerateElectricBillCommercial()
        //{
        //    var res = db.Sp_Get_CommercialMeterLastReading().ToList();
        //    foreach (var item in res)
        //    {
        //        string Month = DateTime.Now.Year + "-" + DateTime.Now.Month;
        //        if (item.Month == Month)
        //        {
        //            continue;
        //        }
        //        var rate = (item.Type == PlotType.Commercial.ToString()) ? 30 : 16;
        //        var Pre = (item.Previous_Reading == null || item.Previous_Reading == 0) ? 0 : item.Previous_Reading;
        //        var Rem = (item.Remaining == null || item.Remaining == 0) ? 0 : item.Remaining;
        //        //db.Sp_Add_ElectricityBill(Pre, item.Plot_Id, item.Name, item.Plot_No, item.Plot_No, item.Type, 0, 0, rate, 0, item.Meter_Id, item.Meter_No, Rem);
        //    }
        //}

        [HttpPost]
        public JsonResult UpdateBillReading(long Id, long Reading)
        {
            db.Sp_Update_ElectMeterReading(Id, Reading);
            return Json(true);
        }
        [NoDirectAccess] public ActionResult ServiceChargesBillPayment()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }

        [NoDirectAccess] public ActionResult ServiceChargesDetails(string billNo)
        {
            var currBill = db.Sp_Get_ServiceCharge_Bill_ById(billNo).FirstOrDefault();
            ViewBag.Plotid = currBill.Plot_Id;
            var res = db.Sp_Get_PlotLastServiceCharges(currBill.Plot_Id).ToList();
            return PartialView(res);
        }

        [NoDirectAccess] public ActionResult ElectricityBillDetails(string MeterNo)
        {
            var currBill = db.Sp_Get_ElecBill_ById(MeterNo).FirstOrDefault();
            ViewBag.Plotid = currBill.Meter_No;
            var res = db.Sp_Get_PlotLastElectricityBills(currBill.Meter_No).ToList();
            return PartialView(res);
        }

        [NoDirectAccess] public ActionResult PayServiceChargesBill(string billNo)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            var res = db.Sp_Get_ServiceCharge_Bill_ById(billNo).FirstOrDefault();
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView(res);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult PayServiceCharges(long PlotId, decimal Amount,long TransactionId)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res1 = db.Sp_Get_PlotDetailData(PlotId).SingleOrDefault();
        //    var res2 = db.Sp_Get_PlotLastOwner(PlotId).SingleOrDefault();
        //    var res4 = db.Sp_Get_PlotLastServiceCharges(PlotId).OrderByDescending(x => x.Id).FirstOrDefault();
        //    //if(res4.Net_Total > Amount)
        //    //{
        //    //    return Json(false);
        //    //}
        //    var res3 = db.Sp_Add_Receipt(Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), null, null, null, null, res2.Mobile_1
        //           , res2.Father_Husband, PlotId, res2.Name, "Cash", 0, "Meher Estate Developers", 0, null, res1.Plot_Size, ReceiptTypes.ServiceCharges.ToString(), userid, userid, "", null, Modules.PlotManagement.ToString(), "", res1.Plot_No, res1.Block_Name, 0,TransactionId).FirstOrDefault();
        //    db.Sp_Update_PlotServiceChargesAmount(res4.Id, Amount);
        //    var res = new { Receiptid = res3.Receipt_No, Token = userid };

        //    var inst = db.ServiceChargesInstallments.Where(x => x.Plot_Id == res4.Plot_Id && x.ServiceType == ServiceType.ServiceCharges.ToString() && x.Status == ServiceChargesInstallmentsStatus.Approved.ToString()).OrderBy(x => x.Create_DateTime).FirstOrDefault();
        //    if (inst != null)
        //    {
        //        var strctr = db.SvcCharges_Installments_Structure.Where(x => (x.InstallmentId == inst.Id) && ((x.Installment_Month.Value.Month == res4.Date.Month) && (x.Installment_Month.Value.Year == res4.Date.Year)) && (x.Status == MonthlyInstallmentStatus.Pending.ToString())).FirstOrDefault();
        //        if(strctr != null)
        //        {
        //            strctr.Status = "Paid";
        //            strctr.Paid_Date = DateTime.Now;
        //            strctr.Paid_Bill_Id = res4.Id;
        //            db.SvcCharges_Installments_Structure.Attach(strctr);
        //            db.Entry(strctr).Property(x => x.Status).IsModified = true;
        //            db.Entry(strctr).Property(x => x.Paid_Date).IsModified = true;
        //            db.Entry(strctr).Property(x => x.Paid_Bill_Id).IsModified = true;
        //            db.SaveChanges();
        //        }
        //    }
        //    return Json(res);
        //}

        [NoDirectAccess] public ActionResult PayElectricityBill(string MeterNo)
        {
            var elecCurrBill = db.Electricity_Bill.Where(x => x.Bill_No == MeterNo).FirstOrDefault();
            //var res = db.Sp_Get_PlotLastElectricityBills(elecCurrBill.Plot_Id).OrderByDescending(x => x.Id).FirstOrDefault();
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            //return PartialView(res);
            return PartialView(elecCurrBill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayElectricity(long PlotId, decimal Amount, string MeterNo, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res4 = db.Sp_Get_ElecBill_ById(MeterNo).FirstOrDefault();
            var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
            var block = db.RealEstate_Blocks.Where(b => b.Id == res1.BlockIden).FirstOrDefault();
            var phase = db.RealEstate_Phases.Where(p => p.Id == block.Phase_Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).FirstOrDefault();
            //if (res4.Net_Total > Amount)
            //{
            //    return Json(false);
            //}
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
            var res3 = db.Sp_Add_Receipt(Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), null, null, null, null, res2.Mobile_1
                   , res2.Father_Husband, PlotId, res2.Name, "Cash", 0, "Grand City Kharian", 0, null, res1.Plot_Size, ReceiptTypes.Electricity_Charges.ToString(), userid, userid, "", null, Modules.PlotManagement.ToString(), "", res1.Plot_No, res1.Block_Name, res1.Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();

            // add phase in Receipt data
            var receipsdata = db.Receipts.Where(r => r.Id == res3.Receipt_Id).FirstOrDefault();
            receipsdata.Phase = phase.Phase_Name;
            db.SaveChanges();


            db.Sp_Add_Activity(userid, "Paid Electricity Bill" + Amount + " " + MeterNo, "Update", "Activity_Record", ActivityType.Services.ToString(), PlotId);

            db.Sp_Update_PlotElectricChargesAmount(res4.Id, Amount);
            var res = new { Receiptid = res3.Receipt_No, Token = userid };

            var inst = db.ServiceChargesInstallments.Where(x => x.Plot_Id == res4.Plot_Id && x.ServiceType == ServiceType.Electricity.ToString() && x.Status == ServiceChargesInstallmentsStatus.Approved.ToString()).OrderBy(x => x.Create_DateTime).FirstOrDefault();
            if (inst != null)
            {
                var strctr = db.SvcCharges_Installments_Structure.Where(x => (x.InstallmentId == inst.Id) && ((x.Installment_Month.Value.Month == res4.Month.Month) && (x.Installment_Month.Value.Year == res4.Month.Year)) && (x.Status == MonthlyInstallmentStatus.Pending.ToString())).FirstOrDefault();
                if (strctr != null)
                {
                    strctr.Status = "Paid";
                    strctr.Paid_Bill_Id = res4.Id;
                    strctr.Paid_Date = DateTime.Now;
                    db.SvcCharges_Installments_Structure.Attach(strctr);
                    db.Entry(strctr).Property(x => x.Status).IsModified = true;
                    db.Entry(strctr).Property(x => x.Paid_Date).IsModified = true;
                    db.Entry(strctr).Property(x => x.Paid_Bill_Id).IsModified = true;
                    db.SaveChanges();
                }
            }
            bool headcashier = false;
            if (User.IsInRole("Head Cashier"))
            {
                headcashier = true;
            }
            try
            {
                AccountHandlerController de = new AccountHandlerController();
                var res5 = de.Service_Charges_Electricity(Amount, res1.Plot_No.ToString(), res1.Block_Name.ToString(), "Cash", "", DateTime.Now, "", "", TransactionId, userid, res3.Receipt_No, 1, headcashier, AccountingModuleFP, res1.BlockIden);
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            }
            return Json(res);
        }
        [NoDirectAccess] public ActionResult ElectricityBillPayment()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name", "Phase_Name");
            return View();
        }
        [NoDirectAccess] public ActionResult ElectricityBillByBlock(long BlockId)
        {
            var res = db.Sp_Get_PlotsElectricCharges_ByBlock(BlockId).ToList();
            return View(res);
        }

        //// commercial
        //[NoDirectAccess] public ActionResult ElectricityBillByProjectCommercial(long ProjectId)
        //{
        //    var res = db.Sp_Get_CommercialElectricCharges_ByProject(ProjectId).ToList();
        //    return View(res);
        //}
        [NoDirectAccess] public ActionResult ServiceChargesBillsByBlock(long BlockId)
        {
            var res1 = db.Sp_Get_PlotsServiceCharges_ByBlock(BlockId).ToList();
            var res = (from x in res1
                       group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase, x.Arrears, x.Amount_Paid, x.Net_Total, x.Fine_Amount, x.duedate } into g
                       select new ServiceChargesBill
                       {
                           Name = g.Key.Name,
                           Address = g.Key.Plot_No + " " + g.Key.Block + " Block " + g.Key.Plot_Type + " " + g.Key.Phase,
                           Arrears = g.Key.Arrears,
                           Amount_Paid = g.Key.Amount_Paid,
                           After_Due_Date_Amount = g.Key.Net_Total,
                           FineAmount = g.Key.Fine_Amount,
                           Due_Date = Convert.ToDateTime(g.Key.duedate),
                           BillDeta = g.Select(x => new Service_Charges_Details
                           {
                               Charges = x.Charges,
                               Plot_Id = x.Plot_Id,
                               Rate = x.Rate,
                               Type = x.Type,
                               Service_Charges = x.Service_Charges
                           }).ToList()
                       }).ToList();
            var res2 = db.Sp_Get_PlotsElectricCharges_ByBlock(BlockId).ToList();
            var res11 = res2.Select(x => new ServiceChargesBill
            {
                Arrears = x.Arrears,
                Block_Name = x.Block_Name,
                Current_Reading = x.Current_Reading,
                Meter_No = x.Meter_No,
                Month = x.Month,
                Name = x.Name,
                Plot_Id = x.Plot_Id,
                Plot_No = x.Plot_No,
                Plot_Type = x.Plot_Type,
                Previous_Reading = x.Previous_Reading,
                Rate = x.Rate,
                Total_Amount = x.Total_Amount,
                Units = x.Units,
                Amount_Paid = x.Amount_Paid,
                Fine_Percentage = x.Fine_Percentage,
                Fine_Amount = x.Fine_Amount,
                Due_Date = x.duedate,
                Due_Date_Amount = x.Due_Date_Amount,
                Net_Total = x.Net_Total
            }).ToList();
            var FinalResult = new ServiceChargesBill { ElectricityData = res11, ServiceChargesData = res };
            return View(FinalResult);
        }
        // commercial
        //[NoDirectAccess] public ActionResult ServiceChargesBillsByProjectCommercial(long ProjectId)
        //{
        //    var res1 = db.Sp_Get_CommercialServiceCharges_ByProject(ProjectId).ToList();
        //    var res = (from x in res1
        //               group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase, x.Arrears, x.Amount_Paid , x.ShopName ,x.Project_Name} into g
        //               select new ServiceChargesBill
        //               {
        //                   Name = g.Key.Name,
        //                   Address = g.Key.ShopName + " " + g.Key.Plot_Type + " " + g.Key.Project_Name ,
        //                   Arrears = g.Key.Arrears,
        //                   Amount_Paid = g.Key.Amount_Paid,
        //                   BillDeta = g.Select(x => new Service_Charges_Details
        //                   {
        //                       Charges = x.Charges,
        //                       Plot_Id = x.Plot_Id,
        //                       Rate = x.Rate,
        //                       Type = x.Type,
        //                       Service_Charges = x.Service_Charges
        //                   }).ToList()
        //               }).ToList();
        //    return View(res);
        //}
        //// commercial
        //[NoDirectAccess] public ActionResult ServiceChargesBillsByProjectCommercial(long ProjectId)
        //{
        //    var res1 = db.Sp_Get_CommercialServiceCharges_ByProject(ProjectId).ToList();
        //    var res = (from x in res1
        //               group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase, x.Arrears, x.Amount_Paid , x.ShopName ,x.Project_Name} into g
        //               select new ServiceChargesBill
        //               {
        //                   Name = g.Key.Name,
        //                   Address = g.Key.ShopName + " " + g.Key.Plot_Type + " " + g.Key.Project_Name ,
        //                   Arrears = g.Key.Arrears,
        //                   Amount_Paid = g.Key.Amount_Paid,
        //                   BillDeta = g.Select(x => new Service_Charges_Details
        //                   {
        //                       Charges = x.Charges,
        //                       Plot_Id = x.Plot_Id,
        //                       Rate = x.Rate,
        //                       Type = x.Type,
        //                       Service_Charges = x.Service_Charges
        //                   }).ToList()
        //               }).ToList();
        //    return View(res);
        //}

        [NoDirectAccess] public ActionResult ViewPlotElectricityBill(string billNo)
        {
            var res = db.Sp_Get_ElecBill_ById(billNo).ToList();
            var thisbill = res.FirstOrDefault();
            string metNo = thisbill.Meter_No;
            var hist = db.Electricity_Bill.Where(x => x.Meter_No == metNo).OrderByDescending(x => x.Month).Take(12).Select(x => new BillHistoryItem { AmountMade = (decimal)x.Net_Total, AmountPaid_Decimal = (decimal)x.Amount_Paid, BillMonth = x.Month, UnitsConsumed = x.Units }).ToList();
            foreach (var it in hist)
            {
                it.AmountPaid = string.Format("{0:n0}", it.AmountPaid_Decimal);
                it.Amount = string.Format("{0:n0}", it.AmountMade);
            }
            var details = db.Electricity_Bill_Details.Where(x => x.Bill_Id == thisbill.Id).ToList();

            return View("ElectricityBillByBlock", new ElectricityBillView { CurrBill = res, PastBills = hist, CurrBillDetails = details });
        }
        //// commercial
        //[NoDirectAccess] public ActionResult ViewPlotElectricityBillCommercial(long Plotid)
        //{
        //    var res = db.Sp_Get_CommercialElectricCharges_ById(Plotid).Select(x => new Sp_Get_CommercialElectricCharges_ByProject_Result
        //    {
        //        Arrears = x.Arrears,
        //        Block_Name = x.Block_Name,
        //        Current_Reading = x.Current_Reading,
        //        Meter_No = x.Meter_No,
        //        Month = x.Month,
        //        Name = x.Name,
        //        Plot_Id = x.Plot_Id,
        //        ShopNo = x.ShopNo,
        //        Plot_Type = x.Plot_Type,
        //        Previous_Reading = x.Previous_Reading,
        //        Rate = x.Rate,
        //        Total_Amount = x.Total_Amount,
        //        Units = x.Units,
        //        Amount_Paid = x.Amount_Paid

        //    }).ToList();

        //    return View("ElectricityBillByProjectCommercial", res);
        //}

        [NoDirectAccess] public ActionResult ViewPlotServiceBill(long Plotid)
        {
            var res1 = db.Sp_Get_PlotsServiceCharges_ByPlot(Plotid).ToList();
            var pltId = res1.Select(x => x.Plot_Id).FirstOrDefault();
            var owns = db.Sp_Get_PlotLastOwner(pltId).ToList();
            var res = (from x in res1
                       group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase, x.Arrears } into g
                       select new ServiceChargesBill
                       {
                           Name = string.Join(" / ", owns.Select(x => x.Name)),
                           Address = g.Key.Plot_No + " " + g.Key.Block + " Block " + g.Key.Plot_Type + " " + g.Key.Phase,
                           Arrears = g.Key.Arrears,
                           BillDeta = g.Select(x => new Service_Charges_Details
                           {
                               Charges = x.Charges,
                               Plot_Id = x.Plot_Id,
                               Rate = x.Rate,
                               Type = x.Type,
                               Service_Charges = x.Service_Charges
                           }).ToList()
                       }).ToList();
            return View("ServiceChargesBillsByBlock", res);
        }
        [NoDirectAccess] public ActionResult ViewElectricityBill(long Id)
        {
            var res = db.Sp_Get_PlotsElectricCharges_ById(Id).Select(x => new Sp_Get_PlotsElectricCharges_ByBlock_Result
            {
                Arrears = x.Arrears,
                Block_Name = x.Block_Name,
                Current_Reading = x.Current_Reading,
                Meter_No = x.Meter_No,
                Month = x.Month,
                Name = x.Name,
                Plot_Id = x.Plot_Id,
                Plot_No = x.Plot_No,
                Plot_Type = x.Plot_Type,
                Previous_Reading = x.Previous_Reading,
                Rate = x.Rate,
                Total_Amount = x.Total_Amount,
                Units = x.Units,
                Amount_Paid = x.Amount_Paid
            }).ToList();

            return View("ElectricityBillByBlock", res);
        }
        [NoDirectAccess] public ActionResult ViewServiceBill(long Id)
        {
            var res1 = db.Sp_Get_PlotsServiceCharges_ById(Id).ToList();
            var pltId = res1.Select(x => x.Plot_Id).FirstOrDefault();
            var owns = db.Sp_Get_PlotLastOwner(pltId).ToList();
            var res = (from x in res1
                       group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase, x.Arrears, x.Amount_Paid, x.Net_Total, x.Fine_Amount, x.duedate, x.Bill_No, x.Amount } into g
                       select new ServiceChargesBill

                       {
                           Total_Amount = g.Key.Amount,
                           Bill_No = g.Key.Bill_No,
                           Name = string.Join(" / ", owns.Select(x => x.Name)),
                           Address = g.Key.Plot_No + " " + g.Key.Block + " Block " + g.Key.Plot_Type + " " + g.Key.Phase,
                           Arrears = g.Key.Arrears,
                           Amount_Paid = g.Key.Amount_Paid,
                           After_Due_Date_Amount = g.Key.Net_Total,
                           FineAmount = g.Key.Fine_Amount,
                           Due_Date = Convert.ToDateTime(g.Key.duedate),
                           BillDeta = g.Select(x => new Service_Charges_Details
                           {
                               Charges = x.Charges,
                               Plot_Id = x.Plot_Id,
                               Rate = x.Rate,
                               Type = x.Type,
                               Service_Charges = x.Service_Charges
                           }).ToList()
                       }).ToList();
            var pId = res1.Select(x => x.Plot_Id).FirstOrDefault();
            //var ElectricityData = db.Sp_Get_PlotsElectricCharges_ByServiceChargesPlotId(pId).ToList();
            //var res11 = ElectricityData.Select(x => new ServiceChargesBill
            //{
            //    Arrears = x.Arrears,
            //    Block_Name = x.Block,
            //    Current_Reading = x.Current_Reading,
            //    Meter_No = x.Meter_No,
            //    Month = x.Month,
            //    Name = x.Name,
            //    Plot_Id = x.Plot_Id,
            //    Plot_No = x.Plot_No,
            //    Plot_Type = x.Plot_Type,
            //    Previous_Reading = x.Previous_Reading,
            //    Rate = x.Rate,
            //    Total_Amount = x.Total_Amount,
            //    Units = x.Units,
            //    Amount_Paid = x.Amount_Paid,
            //    Fine_Percentage=x.Fine_Percentage,
            //    Fine_Amount=x.Fine_Amount,
            //    Due_Date=x.Due_Date,
            //    Due_Date_Amount=x.Due_Date_Amount,
            //    Net_Total=x.Net_Total
            //}).ToList();

            var FinalResult = new ServiceChargesBill { ElectricityData = null/*res11*/, ServiceChargesData = res };
            return View("ServiceChargesBillsByBlock", FinalResult);
        }
        //[NoDirectAccess] public ActionResult ViewServiceBillCommercial(long Id)
        //{
        //    var res1 = db.Sp_Get_CommercialServiceCharges_ById(Id).ToList();
        //    var res = (from x in res1
        //               group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase, x.Arrears, x.Amount_Paid ,x.ShopName ,x.Project_Name} into g
        //               select new ServiceChargesBill
        //               {
        //                   Name = g.Key.Name,
        //                   Address = g.Key.ShopName + " " + g.Key.Plot_Type + " " + g.Key.Project_Name,
        //                   Arrears = g.Key.Arrears,
        //                   Amount_Paid=g.Key.Amount_Paid,
        //                   BillDeta = g.Select(x => new Service_Charges_Details
        //                   {
        //                       Charges = x.Charges,
        //                       Plot_Id = x.Plot_Id,
        //                       Rate = x.Rate,
        //                       Type = x.Type,
        //                       Service_Charges = x.Service_Charges
        //                   }).ToList()
        //               }).ToList();
        //    return View("ServiceChargesBillsByProjectCommercial", res);
        //}
        //[NoDirectAccess] public ActionResult ViewServiceBillCommercial(long Id)
        //{
        //    var res1 = db.Sp_Get_CommercialServiceCharges_ById(Id).ToList();
        //    var res = (from x in res1
        //               group x by new { x.Name, x.Plot_No, x.Block, x.Plot_Type, x.Phase, x.Arrears, x.Amount_Paid ,x.ShopName ,x.Project_Name} into g
        //               select new ServiceChargesBill
        //               {
        //                   Name = g.Key.Name,
        //                   Address = g.Key.ShopName + " " + g.Key.Plot_Type + " " + g.Key.Project_Name,
        //                   Arrears = g.Key.Arrears,
        //                   Amount_Paid = g.Key.Amount_Paid,
        //                   BillDeta = g.Select(x => new Service_Charges_Details
        //                   {
        //                       Charges = x.Charges,
        //                       Plot_Id = x.Plot_Id,
        //                       Rate = x.Rate,
        //                       Type = x.Type,
        //                       Service_Charges = x.Service_Charges
        //                   }).ToList()
        //               }).ToList();
        //    return View("ServiceChargesBillsByProjectCommercial", res);
        //}
        [NoDirectAccess] public ActionResult NewConnection()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Plot Information", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);

            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }
        // Commercial
        [NoDirectAccess] public ActionResult NewConnectionCommercial()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Plot Information", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);

            ViewBag.Project = new SelectList(db.RealEstate_Projects.ToList(), "Id", "Project_Name");
            return View();
        }
        [NoDirectAccess] public ActionResult PlotsSubscribtions(long Id)
        {
            var res1 = db.Sp_Get_ServiceChargesTypeList().ToList();
            var res2 = db.Sp_Get_PlotServiceChargesTypeList(Id).ToList();
            var res3 = db.Plot_ElectricityMeters.Where(x => x.Plot_Id == Id).ToList();
            ViewBag.PlotId = Id;
            var res = new Subscribed_Charges { SCList = res1, Plot_SCList = res2, Plot_Meters = res3 };
            return PartialView(res);
        }
        // commercial
        //[NoDirectAccess] public ActionResult CommercialSubscribtions(long Id,string Type)
        //{
        //    var res1 = db.Sp_Get_ServiceChargesTypeList_Commercial().ToList();
        //    var res2 = db.Sp_Get_CommercialServiceChargesTypeList(Id).ToList();
        //    var res3 = db.Plot_ElectricityMeters.Where(x => x.Plot_Id == Id && x.Module==Type).ToList();
        //    ViewBag.ComId = Id;
        //    ViewBag.Type = Type;
        //    var res = new Subscribed_Charges { SCComList = res1, Com_SCList = res2, Plot_Meters = res3 };
        //    return PartialView(res);
        //}
        [NoDirectAccess] public ActionResult ManageServiceCharges()
        {
            List<Sp_Get_Plotlist_Block_Result> BlkList = new List<Sp_Get_Plotlist_Block_Result>();
            var Blkrest = db.RealEstate_Blocks.ToList();
            foreach (var item in Blkrest)
            {
                var re = db.Sp_Get_Plotlist_Block(item.Id).ToList();
                BlkList.AddRange(re);
            }
            var res1 = BlkList.Where(x => x.Type == PlotType.Commercial.ToString()).ToList();
            var res2 = BlkList.Where(x => x.Type == PlotType.Residential.ToString()).ToList();
            var res = new PlotsByType { Commercial = res1, Residential = res2 };
            return View(res);
        }
        // commercail
        [NoDirectAccess] public ActionResult CommercialManageServiceCharges()
        {
            var res = db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Commercial.ToString()).ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult ConnectionCharges()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult ConnectionChargesList()
        {
            var res = db.Connection_Charges.ToList();
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddConnectionCharge(Connection_Charges sc)
        {
            var res = Convert.ToBoolean(db.Sp_Add_ConnectionCharges(sc.Connection_Name, sc.Charges, sc.Type, sc.Plot_Type).FirstOrDefault());
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Added Connection Charges" + sc.Connection_Name + " " + sc.Charges, "Create", "Activity_Record", ActivityType.Services.ToString(), userid);

            if (res)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult GetCustomerData(long PlotId, string Connection)
        {
            var res1 = db.Sp_Get_PlotDetailData(PlotId).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            var res3 = db.Connection_Charges.SingleOrDefault(x => x.Connection_Name == Connection && x.Plot_Type == res1.Type);
            var res = new NewConnectionCharges { PlotData = res1, OwnerData = res2, Conch = res3 };
            return PartialView(res);
        }
        //[NoDirectAccess] public ActionResult GetCustomerDataCommercial(long PlotId, string Connection)
        //{
        //    var res1 = db.Sp_Get_CommercialDetailData(PlotId).SingleOrDefault();
        //    var res2 = db.Sp_Get_CommercialLastOwner(PlotId).SingleOrDefault();
        //    var res3 = db.Connection_Charges.SingleOrDefault(x => x.Connection_Name == Connection && x.Plot_Type == "Commercial");
        //    var res = new NewConnectionCharges { ComData = res1, OwnerDataCom = res2, Conch = res3 };
        //    return PartialView(res);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayNewConnectionCharges(long PlotId, string Connection, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res1 = db.Sp_Get_PlotData(PlotId).FirstOrDefault();
            var block = db.RealEstate_Blocks.Where(b => b.Id == res1.BlockIden).FirstOrDefault();
            var phase = db.RealEstate_Phases.Where(p => p.Id == block.Phase_Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();
            var res3 = db.Connection_Charges.SingleOrDefault(x => x.Connection_Name == Connection && x.Plot_Type == res1.Type);
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                    var res4 = db.Sp_Add_Receipt(res3.Charges, GeneralMethods.NumberToWords(Convert.ToInt32(res3.Charges)), null, null, null, null, res2.FirstOrDefault().Mobile_1
                           , string.Join(",", res2.Select(x => x.Father_Husband)), PlotId, string.Join(",", res2.Select(x => x.Name)), "Cash", 0, "Grand City Kharian", 0, null, res1.Plot_Size, ReceiptTypes.New_Connection_Charges.ToString(), userid, userid, "", null, Modules.PlotManagement.ToString(), Connection, res1.Plot_No, res1.Block_Name, res1.Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                    
                    //add phase in receipt data
                    var receiptdata = db.Receipts.Where(r => r.Id == res4.Receipt_Id).FirstOrDefault();
                    receiptdata.Phase = phase.Phase_Name;
                    db.SaveChanges();

                    bool headcashier = false;
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }
                    try
                    {
                        AccountHandlerController de = new AccountHandlerController();
                        var res5 = de.Other_Recovery(res3.Charges, res1.Plot_No.ToString(), res1.Type, res1.Block_Name.ToString(), "Cash", null, null, null, "New Connection Charges for " + res1.Plot_No + "Type: " + res1.Type + " - Block: " + res1.Block_Name, TransactionId, userid, res4.Receipt_No, 1, COA_Mapper_ModuleTypes.New_Connection_Charges.ToString(), headcashier, AccountingModuleFP);
                    }
                    catch (Exception ex)
                    {
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    }
                    Transaction.Commit();
                    var res = new { Receiptid = res4.Receipt_No, Token = userid };

                    db.Sp_Add_Activity(userid, "Payed New Connection Charges" + Connection, "Update", "Activity_Record", ActivityType.Services.ToString(), TransactionId);
                    return Json(res);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return Json(false);
                }
            }
        }
        //// commercial
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult PayNewConnectionChargesCommercial(long PlotId, string Connection)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res1 = db.Sp_Get_CommercialDetailData(PlotId).SingleOrDefault();
        //    var res2 = db.Sp_Get_CommercialLastOwner(PlotId).SingleOrDefault();
        //    var res3 = db.Connection_Charges.SingleOrDefault(x => x.Connection_Name == Connection && x.Plot_Type == "Commercial");
        //    Helpers h = new Helpers();
        //    var res4 = db.Sp_Add_Receipt(res3.Charges, h.NumberToWords(Convert.ToInt32(res3.Charges)),null, null, null, null, res2.Mobile_1
        //           , res2.Father_Husband, PlotId, res2.Name, "Cash", 0, "Meher Estate Developers", 0, null, res1.Module, ReceiptTypes.New_Connection_Charges.ToString(), userid, userid, res1.Floor,null, Modules.ShopManagement.ToString(), Connection, res1.Com_App_Shop_Number, res1.Project_Name, 0).FirstOrDefault();
        //    var res = new { Receiptid = res4.Receipt_No, Token = userid };
        //    return Json(res);
        //}
        [NoDirectAccess] public ActionResult FineAndOtherChargesCollection()
        {
            return View();
        }

        [NoDirectAccess] public ActionResult FineCollection()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Plot Information", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);

            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return PartialView();
        }
        [NoDirectAccess] public ActionResult FineResult(long PlotId)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            var res1 = db.Sp_Get_PlotDetailData(PlotId).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            var res = new NewConnectionCharges { PlotData = res1, OwnerData = res2, Conch = null };

            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult FileResult(string FileNumber)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            var Fileid = db.File_Form.Where(x => x.FileFormNumber == FileNumber).Select(x => x.Id).FirstOrDefault();
            Sp_Get_FileFormData_Result res = db.Sp_Get_FileFormData(Fileid).SingleOrDefault();
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayFineCharges(long PlotId, string Remarks, ReceiptData rd, string Module, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            if (Module == Modules.FileManagement.ToString())
            {
                // var fileNo = PlotId;
                //var Fileid = db.File_Form.Where(x => x.FileFormNumber == fileno).Select(x => x.Id).FirstOrDefault();
                var fileno = db.File_Form.Where(x => x.FileFormNumber == PlotId.ToString()).FirstOrDefault(); // Add fileformNumber
                //get phase
                var phase = db.RealEstate_Phases.Where(p => p.Id == fileno.Phase_Id).FirstOrDefault();
                List<Sp_Get_FileFormData_Result> res = db.Sp_Get_FileFormData(fileno.Id).ToList();
                if (rd.Type == ReceiptTypes.Duplicate_Customer_File.ToString())
                {
                    var a = db.Sp_Update_Duplicate_Check(res.FirstOrDefault().File_Transfer_Id, Modules.FileManagement.ToString(), rd.Type);
                }
                var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                var Narration = "Cash Amount Received against Plot no:" + res.FirstOrDefault().FileFormNumber + " Block: " + res.FirstOrDefault().Block + " Plot Type : " + res.FirstOrDefault().Plot_Type;
                var res4 = db.Sp_Add_Receipt(rd.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(rd.Amount)), null, null, null, null, string.Join(",", res.Select(x => x.Mobile_1))
                       , string.Join(",", res.Select(x => x.Father_Husband)), fileno.Id, string.Join(",", res.Select(x => x.Name)), rd.PaymentType, 0, "Grand City Kharian", 0, null, res.FirstOrDefault().Plot_Size, rd.Type, userid, userid, Remarks, null, Modules.FileManagement.ToString(), "", res.FirstOrDefault().FileFormNumber.ToString(), res.FirstOrDefault().Block, res.FirstOrDefault().Plot_Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
               
                // add phase in receipt data
                var receiptdata = db.Receipts.Where(r => r.Id == res4.Receipt_Id).FirstOrDefault();
                receiptdata.Phase = phase.Phase_Name;
                db.SaveChanges();

                if (rd.PaymentType != "Cash")
                {
                    var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                       Modules.FileManagement.ToString(), Types.Installment.ToString(), userid, rd.PayChqNo, PlotId, rd.Ch_bk_Pay_Date, rd.FilePlotNumber.ToString(), res4.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                }
                var res1 = new { Status = true, Receiptid = res4.Receipt_No, ReceiptType = rd.Type, Token = userid };
                return Json(res1);
                db.Sp_Add_Activity(userid, "Pay Fine Charges" + rd.Amount + " Remarks: " + Remarks, "Update", "Activity_Record", ActivityType.Services.ToString(), TransactionId);

            }
            else if (Module == Modules.PlotManagement.ToString())
            {
                var res1 = db.Sp_Get_PlotDetailData(PlotId).SingleOrDefault();
                //get phase
                var plotdata = db.Plots.Where(p => p.Id ==  PlotId).FirstOrDefault();
                var phase = db.RealEstate_Phases.Where(p => p.Id == plotdata.Phase_Id).FirstOrDefault();
                var res2 = db.Sp_Get_PlotLastOwner(PlotId).FirstOrDefault();
                if (rd.Type == ReceiptTypes.Duplicate_Customer_File.ToString())
                {
                    var a = db.Sp_Update_Duplicate_Check(res2.Id, Modules.PlotManagement.ToString(), rd.Type);
                }
                else if (rd.Type == ReceiptTypes.Duplicate_Allotment_Letter.ToString())
                {
                    var a = db.Sp_Update_Duplicate_Check(res2.Id, Modules.PlotManagement.ToString(), rd.Type);
                }
                //else if (Type == ReceiptTypes.Completion_Charges.ToString())
                //{

                //    var Plots = db.Plots.Where(x => x.Id == PlotId && x.Status == "Registered").FirstOrDefault();
                //    string[] parts = Plots.Plot_Size.Split(' ');
                //    string valueString = parts[0];
                //    decimal.TryParse(valueString, out decimal numericValue);
                //    decimal rate = 0;
                //    if (Plots.Type == "Residential")
                //    {
                //         rate = numericValue * 5000;
                //    }
                //    else if (Plots.Type == "Commercial")
                //    {
                //         rate = numericValue * 10000;
                //    }
                //    int rateAsInt = Convert.ToInt32(rate);
                //    //if (Amount==0) 
                //    //{
                //    //    return Json(false);
                //    //}
                //    //if (rateAsInt != Amount)
                //    //{
                //    //    return Json(false);
                //    //}

                //}
                Helpers h = new Helpers();
                var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
                var Narration = "Cash Amount Received against Plot no:" + res1.Plot_No + " Block: " + res1.Block_Name + " Plot Type : " + res1.Type;
                var res4 = db.Sp_Add_Receipt(rd.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(rd.Amount)), null, null, null, null, res2.Mobile_1
                       , res2.Father_Husband, PlotId, res2.Name, rd.PaymentType, 0, "Grand City Kharian", 0, null, res1.Plot_Size, rd.Type, userid, userid, "", null, Modules.PlotManagement.ToString(), "", res1.Plot_No, res1.Block_Name, res1.Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();

                //add phase in receipt data
                var receiptdata = db.Receipts.Where(r => r.Id == res4.Receipt_Id).FirstOrDefault();
                receiptdata.Phase = phase.Phase_Name;
                db.SaveChanges();
                if (rd.PaymentType != "Cash")
                {
                    var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                       Modules.PlotManagement.ToString(), Types.Installment.ToString(), userid, rd.PayChqNo, PlotId, rd.Ch_bk_Pay_Date, res1.Plot_No.ToString(), res4.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                }
                //bool headcashier = false;
                //if (User.IsInRole("Head Cashier"))
                //{
                //    headcashier = true;
                //}
                //try
                //{
                //    AccountHandlerController de = new AccountHandlerController();
                //    var res5 = de.Other_Recovery(Amount, res1.Plot_No.ToString(), res1.Type, res1.Block_Name.ToString(), "Cash", "", null, "", Narration + " " + Remarks, TransactionId, userid, res4.Receipt_No, 1, Type, headcashier, AccountingModuleFP);
                //}
                //catch (Exception ex)
                //{
                //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                //}
                var res = new { Status = true, Receiptid = res4.Receipt_No, ReceiptType = rd.Type, Token = userid };
                return Json(res);
            }
            else
            {
                return Json(false);
            }
        }

        //}
        //[NoDirectAccess] public ActionResult 
        public JsonResult AddMeter(string MeterNo, long PlotId)
        {
            var res = db.Plot_ElectricityMeters.SingleOrDefault(x => x.Meter_No == MeterNo);
            if (res == null)
            {
                var res1 = db.Sp_Get_PlotDetailData(PlotId).SingleOrDefault();
                Plot_ElectricityMeters PEM = new Plot_ElectricityMeters()
                {
                    Meter_No = MeterNo,
                    Plot_Id = PlotId,
                    Meter_Type = res1.Type
                };
                db.Plot_ElectricityMeters.Add(PEM);
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added New Meter " + MeterNo, "Create", "Activity_Record", ActivityType.Services.ToString(), PlotId);

                db.SaveChanges();
                return Json(true);
            }
            else
            {
                var res1 = db.Sp_Get_PlotDetailData(res.Plot_Id).SingleOrDefault();
                return Json("This Meter is already installed on " + res1.Plot_No + " of Block " + res1.Block_Name);
            }

        }
        //// commercial
        //public JsonResult AddMeterCommercial(string MeterNo, long ComId,string Type)
        //{
        //    var res = db.Plot_ElectricityMeters.SingleOrDefault(x => x.Meter_No == MeterNo && x.Module==Type && x.Plot_Id==ComId);
        //    if (res == null)
        //    {
        //        //var res1 = db.Sp_Get_PlotDetailData(PlotId).SingleOrDefault();
        //        Plot_ElectricityMeters PEM = new Plot_ElectricityMeters()
        //        {
        //            Meter_No = MeterNo,
        //            Plot_Id = ComId,
        //            Meter_Type = "Commercial",
        //            Module= Type

        //        };
        //        db.Plot_ElectricityMeters.Add(PEM);
        //        db.SaveChanges();
        //        return Json(true);
        //    }
        //    else
        //    {
        //        var res1 = db.Plot_ElectricityMeters.Where(x => x.Plot_Id == ComId && x.Module==Type).SingleOrDefault();   
        //        return Json("This Meter is already installed on " + res1.Plot_Id );
        //    }

        //}
        [NoDirectAccess] public ActionResult NewElectricityBill(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_PlotsElectricCharges_ById(Id).Select(x => new Sp_Get_PlotsElectricCharges_ByBlock_Result
            {
                Arrears = x.Arrears,
                Block_Name = x.Block_Name,
                Current_Reading = x.Current_Reading,
                Meter_No = x.Meter_No,
                Month = x.Month,
                Name = x.Name,
                Plot_Id = x.Plot_Id,
                Plot_No = x.Plot_No,
                Plot_Type = x.Plot_Type,
                Previous_Reading = x.Previous_Reading,
                Rate = x.Rate,
                Total_Amount = x.Total_Amount,
                Units = x.Units,
                Id = x.Id,
                Grand_Total = x.Grand_Total,
            }).SingleOrDefault();

            return PartialView(res);
        }
        //// commercial
        //[NoDirectAccess] public ActionResult NewElectricityBillCommercial(long Id)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res = db.Sp_Get_CommercialElectricCharges_ById(Id).Select(x => new Sp_Get_CommercialElectricCharges_ByProject_Result
        //    {
        //        Arrears = x.Arrears,
        //        Block_Name = x.Block_Name,
        //        Current_Reading = x.Current_Reading,
        //        Meter_No = x.Meter_No,
        //        Month = x.Month,
        //        Name = x.Name,
        //        Plot_Id = x.Plot_Id,
        //        ShopNo = x.ShopNo,
        //        Plot_Type = x.Plot_Type,
        //        Previous_Reading = x.Previous_Reading,
        //        Rate = x.Rate,
        //        Total_Amount = x.Total_Amount,
        //        Units = x.Units,
        //        Id = x.Id,
        //        Grand_Total = x.Grand_Total,
        //    }).SingleOrDefault();

        //    return PartialView(res);
        //}
        // for plots and commercial
        public JsonResult UpdateElecBill(long Id, string Remarks, int Current, decimal Arrears, int Previous, decimal grndtot)
        {
            var PaidAmount = db.Electricity_Bill.Where(x => x.Id == Id).Select(x => x.Amount_Paid).SingleOrDefault();
            if (PaidAmount != null || PaidAmount != Convert.ToDecimal(0.00))
            {
                if (Current < Previous)
                {
                    return Json(false);
                }
                else
                {

                    var grtot = grndtot - Arrears;
                    //var per = db.Fine_And_Penalties.Where(x => x.Fine_Type == FineAndPenaltiesTypes.ServiceChargesFine.ToString()).Select(x => x.Fine_Percentage).LastOrDefault();

                    //db.Sp_Update_ElectricityBill(Id, Remarks, Current, Arrears, grndtot, grtot);
                    var res = db.Sp_Update_ElectricityBill(Id, grtot, Arrears, Remarks, 10, ((grtot * 10) / 100), Arrears + (grtot + ((grtot * 10) / 100)), (Arrears + (grtot + ((grtot * 10) / 100))), Current, Previous, Current - Previous);
                    long userid = long.Parse(User.Identity.GetUserId());
                    db.Sp_Add_Activity(userid, "Updated Electricity Bill", "Update", "Activity_Record", ActivityType.Services.ToString(), Id);

                    return Json(true);
                }
            }
            else
            {
                return Json(false);
            }


        }

        public JsonResult RemoveMeter(long Id)
        {
            db.Sp_Delete_PlotMeter(Id);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Removed Meter", "Delete", "Activity_Record", ActivityType.Services.ToString(), Id);

            return Json(true);
        }

        public JsonResult RequestForManualBillingPermission(long block, string blockName, string rems)
        {
            try
            {
                long _grp_ = new Helpers().RandomNumber();
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var res = db.ServiceChargesPermissions.Add(new ServiceChargesPermission
                {
                    CreatedBy_Name = uname,
                    CreatedBy_UserId = uid,
                    Created_Date = DateTime.Now,
                    ModuleType = ServiceChargeRequestsType.Manual_Meter_Reading.ToString(),
                    Status = null,
                    Permission_Details = rems,
                    ForModuleType = ServiceChargeModule.Block.ToString(),
                    For_ModuleId = block,
                    For_ModuleName = blockName,
                    GroupId = _grp_,
                    ServiceType = ServiceType.Electricity.ToString()
                });
                var id = db.SaveChanges();
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Requested Fir Manuall Billing " + rems, "Delete", "Activity_Record", ActivityType.Services.ToString(), userid);

                return Json(id);
            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }

        [NoDirectAccess] public ActionResult ManualMeterReadingRequestForBlock(string block, long blockId)
        {
            ViewBag.blkName = block;
            ViewBag.blockId = blockId;
            return PartialView();
        }

        [NoDirectAccess] public ActionResult ManualMeterReadingRequests()
        {
            return View();
        }

        [NoDirectAccess] public ActionResult MeterReadingPermsTabular(int tp)
        {
            var res = db.ServiceChargesPermissions.ToList();
            switch (tp)
            {
                case 1:
                    return View(res.Where(x => x.Status == null).ToList());
                case 2:
                    return View(res.Where(x => x.Status == true).ToList());
                case 3:
                    return View(res.Where(x => x.Status == false).ToList());
                default:
                    return View(res.Where(x => x.Status == null).ToList());
            }
        }

        [NoDirectAccess] public ActionResult ManualMeterReadingDetail(long Id)
        {
            var res = db.ServiceChargesPermissions.Where(x => x.Id == Id).FirstOrDefault();
            return View(res);
        }

        public JsonResult ChangeManualReadingStatus(long Id, int stat, string rems)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                db.SP_ManualMeterReading_Request_Status(Id, (stat == 1), uid, rems);
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Changed Meter Reading " + rems, "Update", "Activity_Record", ActivityType.Services.ToString(), Id);

                return Json(new { Status = true, Msg = "Status has been updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Msg = "Unable to update the status. Contact SA Systems" });
            }
        }

        [NoDirectAccess] public ActionResult FineWaiverRequest(int tp)
        {
            ViewBag.serviceType = tp;
            ViewBag.phases = new SelectList(db.RealEstate_Phases.Select(x => new { x.Id, Name = x.Phase_Name }).ToList(), "Id", "Name");
            return View();
        }

        [NoDirectAccess] public ActionResult BillWaiverRequest(int tp)
        {
            ViewBag.serviceType = tp;
            ViewBag.phases = new SelectList(db.RealEstate_Phases.Select(x => new { x.Id, Name = x.Phase_Name }).ToList(), "Id", "Name");
            return PartialView();
        }

        [NoDirectAccess] public ActionResult ArrearsWaiverRequest(int tp)
        {
            ViewBag.serviceType = tp;
            ViewBag.phases = new SelectList(db.RealEstate_Phases.Select(x => new { x.Id, Name = x.Phase_Name }).ToList(), "Id", "Name");
            return PartialView();
        }

        public JsonResult SaveWaiverRequests(int[] phases, int[] blocks, int[] plots, int tp, string remarks, int billType, int waiveType)
        {
            long __grp__ = new Helpers().RandomNumber();
            var uid = User.Identity.GetUserId<long>();
            string serTyp = (billType == 1) ? ServiceType.Electricity.ToString() : ServiceType.ServiceCharges.ToString();
            string modTp = string.Empty;

            if (waiveType == 1)
            {
                modTp = ServiceChargeRequestsType.Fine_Waiver.ToString();
            }
            else if (waiveType == 2)
            {
                modTp = ServiceChargeRequestsType.Bill_Waiver.ToString();
            }
            else if (waiveType == 3)
            {
                modTp = ServiceChargeRequestsType.Arrears_Removal.ToString();
            }

            try
            {
                //for phase waiver
                if (tp == 1)
                {
                    //yahan phases ko waive krao
                    if (phases.Length > 0)
                    {
                        List<InstallmentStatusModel> phasesList = new List<InstallmentStatusModel>();
                        foreach (var v in phases)
                        {
                            InstallmentStatusModel ims = db.RealEstate_Phases.Where(x => x.Id == v).Select(x => new InstallmentStatusModel { Id = (int)x.Id, Status = x.Phase_Name }).FirstOrDefault();
                            phasesList.Add(ims);
                        }

                        //yahan save krao db main
                        var skls = new XElement("PermissionRequests", phasesList.Select(x => new XElement("Request",
                            new XAttribute("rems", remarks),
                            new XAttribute("ModTp", modTp),
                            new XAttribute("ModID", x.Id),
                            new XAttribute("Name", x.Status),
                            new XAttribute("ModName", ServiceChargeModule.Phase.ToString()),
                            new XAttribute("GroupId", __grp__),
                            new XAttribute("BillType", serTyp)))).ToString();
                        db.SP_Add_ServiceCharges_PermissionRequests(skls, uid);
                        long userid = long.Parse(User.Identity.GetUserId());
                        db.Sp_Add_Activity(userid, "Wavier For Phase  ", "Create", "Activity_Record", ActivityType.Services.ToString(), billType);

                    }
                }
                //for block waiver
                else if (tp == 2)
                {
                    // yahan blocks ko waive krao
                    if (blocks.Length > 0)
                    {
                        List<InstallmentStatusModel> blocksList = new List<InstallmentStatusModel>();
                        foreach (var v in blocks)
                        {
                            InstallmentStatusModel ims = db.RealEstate_Blocks.Where(x => x.Id == v).Select(x => new InstallmentStatusModel { Id = (int)x.Id, Status = x.Block_Name }).FirstOrDefault();
                            blocksList.Add(ims);
                        }
                        //yahan save krao db main
                        var skls = new XElement("PermissionRequests", blocksList.Select(x => new XElement("Request",
                            new XAttribute("rems", remarks),
                            new XAttribute("ModTp", modTp),
                            new XAttribute("ModID", x.Id),
                            new XAttribute("Name", x.Status),
                            new XAttribute("ModName", ServiceChargeModule.Block.ToString()),
                            new XAttribute("GroupId", __grp__),
                            new XAttribute("BillType", serTyp)))).ToString();
                        db.SP_Add_ServiceCharges_PermissionRequests(skls, uid);
                        long userid = long.Parse(User.Identity.GetUserId());
                        db.Sp_Add_Activity(userid, "Wavier For Block  ", "Create", "Activity_Record", ActivityType.Services.ToString(), billType);
                    }
                }
                //for plots waiver
                else if (tp == 3)
                {
                    if (plots.Length > 0)
                    {
                        //yahan plots ko waive krao
                        List<InstallmentStatusModel> plotsList = new List<InstallmentStatusModel>();
                        foreach (var v in plots)
                        {
                            InstallmentStatusModel ims = db.Plots.Where(x => x.Id == v).Select(x => new InstallmentStatusModel { Id = (int)x.Id, Status = x.Plot_Number }).FirstOrDefault();
                            plotsList.Add(ims);
                        }
                        //yahan save krao db main
                        var skls = new XElement("PermissionRequests", plotsList.Select(x => new XElement("Request",
                            new XAttribute("rems", remarks),
                            new XAttribute("ModTp", modTp),
                            new XAttribute("ModID", x.Id),
                            new XAttribute("Name", x.Status),
                            new XAttribute("ModName", ServiceChargeModule.Plot.ToString()),
                            new XAttribute("GroupId", __grp__),
                            new XAttribute("BillType", serTyp)))).ToString();
                        db.SP_Add_ServiceCharges_PermissionRequests(skls, uid);
                        long userid = long.Parse(User.Identity.GetUserId());
                        db.Sp_Add_Activity(userid, "Wavier For Plot  ", "Create", "Activity_Record", ActivityType.Services.ToString(), billType);
                    }
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public JsonResult GetBlocksByPhases(long phase)
        {
            var res = db.RealEstate_Blocks.Where(x => x.Phase_Id == phase).Select(x => new { x.Id, Name = x.Block_Name }).ToList();
            return Json(res);
        }

        public JsonResult GetPlotsbyBlocks(long block, int tp)
        {
            if (tp == 1)
            {
                var res = db.SP_Get_ElectricitySubscribed_Plots_Blockwise(block).Select(x => new { x.Id, Name = x.Plot_Number }).ToList();
                return Json(res);
            }
            else
            {
                var res = db.SP_GET_ServiceCharges_SubscribedPlots_BlockWise(block).Select(x => new { x.Id, Name = x.Plot_Number }).ToList();
                return Json(res);
            }
        }

        [NoDirectAccess] public ActionResult RequestsList(string typ, string module)
        {
            ViewBag.type = typ;
            ViewBag.mod = module;
            return View();
        }

        [NoDirectAccess] public ActionResult PermissionsView(string typ, string mod, int cat)
        {
            if (User.IsInRole("Utilities Manager") || User.IsInRole("Administrator"))
            {
                if (cat == 1)
                {
                    //pending
                    var res = db.ServiceChargesPermissions.Where(x => x.Status == null).ToList();
                    return PartialView(res);
                }

                else if (cat == 2)
                {
                    var res = db.ServiceChargesPermissions.Where(x => x.Status == true).ToList();
                    return PartialView(res);
                    //approved
                }

                else if (cat == 3)
                {
                    var res = db.ServiceChargesPermissions.Where(x => x.Status == false).ToList();
                    return PartialView(res);
                    //rejected
                }
            }
            else if (User.IsInRole("Utilities Staff"))
            {
                var uid = User.Identity.GetUserId<long>();
                if (cat == 1)
                {
                    //pending
                    var res = db.ServiceChargesPermissions.Where(x => x.Status == null && x.CreatedBy_UserId == uid).ToList();
                    return PartialView(res);
                }

                else if (cat == 2)
                {
                    var res = db.ServiceChargesPermissions.Where(x => x.Status == true && x.CreatedBy_UserId == uid).ToList();
                    return PartialView(res);
                    //approved
                }

                else if (cat == 3)
                {
                    var res = db.ServiceChargesPermissions.Where(x => x.Status == false && x.CreatedBy_UserId == uid).ToList();
                    return PartialView(res);
                    //rejected
                }
            }
            return PartialView(null);
        }

        [NoDirectAccess] public ActionResult PermissionDetails(long group)
        {
            var res = db.ServiceChargesPermissions.Where(x => x.GroupId == group).ToList();
            return PartialView(res);
        }

        public JsonResult ChangeRequestStat(long Id, int stat, string rems)
        {
            try
            {
                List<long?> res = new List<long?>();
                var uid = User.Identity.GetUserId<long>();
                var dets = db.ServiceChargesPermissions.Where(x => x.Id == Id).FirstOrDefault();
                if (!(dets is null))
                {
                    if ((dets.Created_Date.Value.Month == DateTime.Now.Month) && (dets.Created_Date.Value.Year == DateTime.Now.Year))
                    {
                        res = db.SP_Update_ServiceCharge_PermStatus(Id, (stat == 1), uid, rems).ToList();
                    }
                    else
                    {
                        res = db.SP_Update_ServiceCharge_PermStatus(Id, false, uid, rems).ToList();
                        stat = 2;
                    }
                }

                var skls = new XElement("Comments", res.Select(x => new XElement("CommentData",
                new XAttribute("Comment", "Reqest for " + dets.ModuleType + " for this plot was initiated by " + dets.CreatedBy_Name + " and was " + ((stat == 1) ? "Accepted" : "Rejected") + " by " + dets.ApprovingBody_Name + ". Amount : " + ((dets.Amount is null) ? "Full Amount Waiver" : dets.Amount.ToString()) + ". For Month : " + dets.Created_Date.Value.Month.ToString("d2") + "/" + dets.Created_Date.Value.Year.ToString("yyyy")),
                new XAttribute("ModTp", 1),
                new XAttribute("MsTp", "Text"),
                new XAttribute("Plt", x),
                new XAttribute("Uid", uid)))).ToString();
                db.Sp_Add_Plots_Comments_SvcChg(skls);
                //Notifier.Notify(uid, NotifyTo.Utili());ties_Manager, NotifierMsg.Bill_Permission_Single, new object[] { noti_tex, serTyp, scp.ForModuleType, scp.For_ModuleName }, NotifyType.ServiceCharges.ToString

                return Json(new { Status = true, Msg = "Status has been updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Msg = "Unable to update the status. Contact SA Systems" });
            }
        }

        [NoDirectAccess] public ActionResult BillInstallments(int? tp)
        {
            ViewBag.serTyp = tp;
            return View();
        }

        [NoDirectAccess] public ActionResult MeterBillDetails(string billId, int? tp)
        {
            ViewBag.serTyp = tp;

            if (tp == 1)
            {
                //electricity

                ViewBag.BillNo = billId;
                var currMonBill = db.Sp_Get_ElecBill_ById(billId).FirstOrDefault();
                if (currMonBill is null)
                {
                    return PartialView(null);
                }
                else
                {
                    ViewBag.Plotid = currMonBill.Plot_Id;
                    var res = db.Sp_Get_PlotLastElectricityBills(currMonBill.Meter_No).ToList();
                    currMonBill.PastMonthBills = res;
                    return PartialView(currMonBill);
                }
            }
            else
            {
                //service charges

                ViewBag.BillNo = billId;
                var currMonBill = db.Sp_Get_ServiceCharge_Bill_ById(billId).FirstOrDefault();
                if (currMonBill is null)
                {
                    return PartialView("ServiceBillDetails", null);
                }
                else
                {
                    ViewBag.Plotid = currMonBill.Plot_Id;
                    var res = db.Sp_Get_PlotLastServiceCharges(currMonBill.Plot_Id).ToList();
                    currMonBill.PastMonthBills = res;
                    return PartialView("ServiceBillDetails", currMonBill);
                }
            }
        }

        public JsonResult SaveInstallments(InstallmentStatusModel[] installments, int plan, string rems, Sp_Get_ElecBill_ById_Result details, int SerTyp)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).FirstOrDefault().Name;
                if (plan > 0)
                {

                    //tau aese save kro
                    ServiceChargesInstallment sci = new ServiceChargesInstallment
                    {
                        Meter_Id = details.Meter_Id,
                        Meter_Num = details.Meter_No,
                        PlanId = plan,
                        Plot_Id = details.Plot_Id,
                        Plot_Num = details.Plot_No,
                        Bill_Id = details.Id,
                        Create_DateTime = DateTime.Now,
                        OtherDetails = JsonConvert.SerializeObject(details),
                        CreatedBy_Id = uid,
                        CreatedBy_Name = uname,
                        CreatedBy_Remarks = rems,
                        Status = ServiceChargesInstallmentsStatus.Pending.ToString(),
                        ServiceType = (SerTyp == 1) ? ServiceType.Electricity.ToString() : ServiceType.ServiceCharges.ToString()
                    };

                    db.ServiceChargesInstallments.Add(sci);
                    var res = db.SaveChanges();

                    //is k baad installments details save krao
                    var dt = DateTime.Now;
                    for (int v = 0; v < plan; v++)
                    {
                        SvcCharges_Installments_Structure scis = new SvcCharges_Installments_Structure
                        {
                            InstallmentId = sci.Id,
                            Installment_Amount = installments[0].Id,
                            Installment_Month = dt.AddMonths(v),
                            Status = MonthlyInstallmentStatus.Pending.ToString()
                        };
                        db.SvcCharges_Installments_Structure.Add(scis);
                        long userid = long.Parse(User.Identity.GetUserId());
                        db.Sp_Add_Activity(userid, "Installments Changes: " + rems, "Create", "Activity_Record", ActivityType.Services.ToString(), plan);
                        db.SaveChanges();
                    }
                }
                else if (plan == 0)
                {
                    //custom plan hai
                    ServiceChargesInstallment sci = new ServiceChargesInstallment
                    {
                        Meter_Id = details.Meter_Id,
                        Meter_Num = details.Meter_No,
                        PlanId = plan,
                        Plot_Id = details.Plot_Id,
                        Plot_Num = details.Plot_No,
                        Bill_Id = details.Id,
                        Create_DateTime = DateTime.Now,
                        OtherDetails = JsonConvert.SerializeObject(details),
                        CreatedBy_Id = uid,
                        CreatedBy_Name = uname,
                        CreatedBy_Remarks = rems,
                        Status = ServiceChargesInstallmentsStatus.Pending.ToString(),
                        ServiceType = ServiceType.Electricity.ToString()
                    };

                    db.ServiceChargesInstallments.Add(sci);
                    var res = db.SaveChanges();
                    //is k baad installments details save krao
                    foreach (var v in installments)
                    {
                        string[] parsedDate = v.Status.Split('/');
                        SvcCharges_Installments_Structure scis = new SvcCharges_Installments_Structure
                        {
                            InstallmentId = sci.Id,
                            Installment_Amount = v.Id,
                            Installment_Month = new DateTime(int.Parse(parsedDate[1]), int.Parse(parsedDate[0]), 1),
                            Status = MonthlyInstallmentStatus.Pending.ToString()
                        };
                        db.SvcCharges_Installments_Structure.Add(scis);
                        long userid = long.Parse(User.Identity.GetUserId());
                        db.Sp_Add_Activity(userid, "Installments Changes: " + rems, "Create", "Activity_Record", ActivityType.Services.ToString(), plan);
                        db.SaveChanges();
                    }
                }
                else
                {
                    return Json(new { Status = false, Msg = "Invalid Installment Plan Type." });
                }

                return Json(new { Status = true, Msg = "Installments have been saved successfully. Updated bill can be printed after this request has been approved by management." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Msg = "An error occurred. Please try again." });
            }
        }

        [NoDirectAccess] public ActionResult InstallmentRequests()
        {
            return View();
        }

        [NoDirectAccess] public ActionResult InstReqView(int cat)
        {
            var uid = User.Identity.GetUserId<long>();
            if (User.IsInRole("Utilities Manager"))
            {
                if (cat == 1)
                {
                    var res = db.ServiceChargesInstallments.Where(x => x.Status == ServiceChargesInstallmentsStatus.Pending.ToString()).ToList();
                    res.ForEach(x => x.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(x.OtherDetails));
                    return PartialView(res);
                }
                else if (cat == 2)
                {
                    var res = db.ServiceChargesInstallments.Where(x => x.Status == ServiceChargesInstallmentsStatus.Approved.ToString()).ToList();
                    res.ForEach(x => x.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(x.OtherDetails));
                    return PartialView(res);
                }
                else if (cat == 3)
                {
                    var res = db.ServiceChargesInstallments.Where(x => x.Status == ServiceChargesInstallmentsStatus.Rejected.ToString()).ToList();
                    res.ForEach(x => x.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(x.OtherDetails));
                    return PartialView(res);
                }
                else
                {
                    var res = db.ServiceChargesInstallments.Where(x => x.Status == ServiceChargesInstallmentsStatus.Pending.ToString()).ToList();
                    res.ForEach(x => x.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(x.OtherDetails));
                    return PartialView(res);
                }
            }
            else if (User.IsInRole("Utilities Staff"))
            {
                if (cat == 1)
                {
                    var res = db.ServiceChargesInstallments.Where(x => x.Status == ServiceChargesInstallmentsStatus.Pending.ToString() && x.CreatedBy_Id == uid).ToList();
                    res.ForEach(x => x.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(x.OtherDetails));
                    return PartialView(res);
                }
                else if (cat == 2)
                {
                    var res = db.ServiceChargesInstallments.Where(x => x.Status == ServiceChargesInstallmentsStatus.Approved.ToString() && x.CreatedBy_Id == uid).ToList();
                    res.ForEach(x => x.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(x.OtherDetails));
                    return PartialView(res);
                }
                else if (cat == 3)
                {
                    var res = db.ServiceChargesInstallments.Where(x => x.Status == ServiceChargesInstallmentsStatus.Rejected.ToString() && x.CreatedBy_Id == uid).ToList();
                    res.ForEach(x => x.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(x.OtherDetails));
                    return PartialView(res);
                }
                else
                {
                    var res = db.ServiceChargesInstallments.Where(x => x.Status == ServiceChargesInstallmentsStatus.Pending.ToString() && x.CreatedBy_Id == uid).ToList();
                    res.ForEach(x => x.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(x.OtherDetails));
                    return PartialView(res);
                }
            }

            return PartialView(null);
        }

        [NoDirectAccess] public ActionResult InstallmentDetails(long instId)
        {
            var instOv = db.ServiceChargesInstallments.Where(x => x.Id == instId).SingleOrDefault();
            if (!(instOv is null))
            {
                instOv.BillDetails = JsonConvert.DeserializeObject<Sp_Get_ElecBill_ById_Result>(instOv.OtherDetails);
                var instDet = db.SvcCharges_Installments_Structure.Where(x => x.InstallmentId == instOv.Id).ToList();
                return PartialView(new SC_Installments_Detailed { InstallmentOverview = instOv, InstallmentStructure = instDet });
            }

            return PartialView(null);
        }

        public JsonResult UpdateInstallmentApprovalStatus(long bill, long id, string rems, int stat)
        {
            try
            {
                string status = string.Empty;
                if (stat == 1)
                {
                    status = ServiceChargesInstallmentsStatus.Approved.ToString();
                }
                else if (stat == 0)
                {
                    status = ServiceChargesInstallmentsStatus.Rejected.ToString();
                }

                var uid = User.Identity.GetUserId<long>();
                db.Sp_Update_SvChg_Installment_Status(bill, id, uid, status, rems);
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Updated Installment Approval " + rems, "Update", "Activity_Record", ActivityType.Services.ToString(), id);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [NoDirectAccess] public ActionResult ConfigurationView()
        {
            var config = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.ServiceCharges.ToString()).SingleOrDefault();
            if (config is null)
            {
                var res = db.RealEstate_Blocks.Select(x => new BlockFineConfig { Id = x.Id, BlockName = x.Block_Name, FinePercentage = 0 }).ToList();
                ViewBag.saveEssential = true;
                ServiceChargesConfig scc = new ServiceChargesConfig { BlockFines = res, DueDay_ForBill = 0, ServiceChargesBreakUp = null };
                return View(scc);
            }
            else
            {
                return View(JsonConvert.DeserializeObject<ServiceChargesConfig>(config.Config_For_Update));
            }
        }

        public JsonResult UpdateConfig(BlockFineConfig[] Config, int due, BlockFineConfig[] SvcBrk)
        {
            try
            {
                var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.ServiceCharges.ToString()).SingleOrDefault();
                List<BlockFineConfig> conf = new List<BlockFineConfig>();
                foreach (var v in Config)
                {
                    conf.Add(new BlockFineConfig { BlockName = v.BlockName, FinePercentage = v.FinePercentage });
                }
                if (res is null)
                {
                    ServiceChargesConfig scc = new ServiceChargesConfig { BlockFines = conf, DueDay_ForBill = due };
                    db.MIS_Modules_Configurations.Add(new MIS_Modules_Configurations { LastUpdated = null, Config_For_Update = JsonConvert.SerializeObject(scc), Module = MIS_Modules.ServiceCharges.ToString() });
                    db.SaveChanges();
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [NoDirectAccess] public ActionResult UpdateConfigRequest()
        {
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.ServiceCharges.ToString()).SingleOrDefault();
            return View(res);
        }

        [NoDirectAccess] public ActionResult MeterWaiver(string bill, int tp, int ser)
        {
            //long bill_Id = long.Parse(Decrypt(bill));
            ViewBag.tp = tp;
            ViewBag.serTyp = ser;
            if (ser == 1)
            {
                var currMonBill = db.Sp_Get_ElecBill_ById(bill).FirstOrDefault();
                return PartialView(currMonBill);
            }
            else
            {
                var currMonBill = db.Sp_Get_ServiceCharge_Bill_ById(bill).FirstOrDefault();
                return PartialView("ServiceWaiver", currMonBill);
            }
        }

        [NoDirectAccess] public ActionResult TrailBalance(string bill, int tp, int ser)
        {
            //long bill_Id = long.Parse(Decrypt(bill));
            ViewBag.tp = tp;
            ViewBag.serTyp = ser;
            if (ser == 1)
            {
                var currMonBill = db.Sp_Get_ElecBill_ById(bill).FirstOrDefault();
                return PartialView(currMonBill);
            }
            else
            {
                var currMonBill = db.Sp_Get_ServiceCharge_Bill_ById(bill).FirstOrDefault();
                return PartialView("ServiceTrailBalance", currMonBill);
            }
        }

        public JsonResult SaveMeterWaiverRequest(long plot, string remarks, int billType, int waiveType, float amt)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                string serTyp = (billType == 1) ? ServiceType.Electricity.ToString() : ServiceType.ServiceCharges.ToString();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var pltInfo = db.Plots.Where(x => x.Id == plot).Select(x => new { x.Id, x.Plot_Number }).FirstOrDefault();
                string modTp = string.Empty;
                string noti_tex = string.Empty;

                if (waiveType == 1)
                {
                    modTp = ServiceChargeRequestsType.Fine_Waiver.ToString();
                    noti_tex = "Fine Waiver";
                }
                else if (waiveType == 2)
                {
                    modTp = ServiceChargeRequestsType.Bill_Waiver.ToString();
                    noti_tex = "Bill Waiver";
                }
                else if (waiveType == 3)
                {
                    modTp = ServiceChargeRequestsType.Arrears_Removal.ToString();
                    noti_tex = "Arrears Waiver";
                }
                else if (waiveType == 4)
                {
                    modTp = ServiceChargeRequestsType.Trail_Balance.ToString();
                    noti_tex = "Trail Balance";
                }

                ServiceChargesPermission scp = new ServiceChargesPermission
                {
                    Amount = (decimal)amt,
                    CreatedBy_Name = uname,
                    ModuleType = modTp,
                    Permission_Details = remarks,
                    CreatedBy_UserId = uid,
                    Created_Date = DateTime.Now,
                    For_ModuleId = pltInfo.Id,
                    For_ModuleName = pltInfo.Plot_Number,
                    ForModuleType = ServiceChargeModule.Plot.ToString(),
                    GroupId = new Helpers().RandomNumber(),
                    ServiceType = serTyp,
                    Status = null
                };
                db.ServiceChargesPermissions.Add(scp);
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Meter Wavier" + remarks, "Create", "Activity_Record", ActivityType.Services.ToString(), plot);
                db.SaveChanges();

                //Sending notification here
                //Notifier.Notify(uid, NotifyTo.Utilities_Manager, NotifierMsg.Bill_Permission_Single, new object[] { scp.Id, noti_tex, serTyp, remarks, scp.ForModuleType, scp.For_ModuleName }, NotifyType.ServiceCharges.ToString());

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [NoDirectAccess] public ActionResult MeterChangeReqForm(string billNo)
        {
            //long billId = long.Parse(Decrypt(billNo));
            var bill = db.Sp_Get_ElecBill_ById(billNo).FirstOrDefault();
            return PartialView(bill);
        }

        public JsonResult SaveMeterChangeReq(Electricity_Meter_Reqs dets)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).SingleOrDefault().Name;
                dets.Requested_By_Id = uid;
                dets.Requested_By_Name = uname;
                dets.Request_Date = DateTime.Now;
                dets.Approval_Status = ServiceChargesInstallmentsStatus.Pending.ToString();
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Meter Change Request ", "Create", "Activity_Record", ActivityType.Services.ToString(), userid);
                db.Electricity_Meter_Reqs.Add(dets);
                db.SaveChanges();

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [NoDirectAccess] public ActionResult MeterChangeRequests()
        {
            return View();
        }

        [NoDirectAccess] public ActionResult MetChgReqsTabular(int tp)
        {
            string tpDec = string.Empty;
            switch (tp)
            {
                case 1:
                    tpDec = ServiceChargesInstallmentsStatus.Pending.ToString();
                    break;
                case 2:
                    tpDec = ServiceChargesInstallmentsStatus.Approved.ToString();
                    break;
                case 3:
                    tpDec = ServiceChargesInstallmentsStatus.Rejected.ToString();
                    break;
                default:
                    tpDec = ServiceChargesInstallmentsStatus.Pending.ToString();
                    break;
            }
            if (User.IsInRole("Utilities Manager"))
            {
                var res = db.Electricity_Meter_Reqs.Where(x => x.Approval_Status == tpDec).ToList();
                return PartialView(res);
            }
            else if (User.IsInRole("Utilities Staff"))
            {
                var uid = User.Identity.GetUserId<long>();
                var res = db.Electricity_Meter_Reqs.Where(x => x.Approval_Status == tpDec && x.Requested_By_Id == uid).ToList();
                return PartialView(res);
            }

            return PartialView(null);
        }

        [NoDirectAccess] public ActionResult MetChgReqDets(long id)
        {
            var res = db.Electricity_Meter_Reqs.Where(x => x.Id == id).SingleOrDefault();
            return PartialView(res);
        }

        public JsonResult UpdateMeterChangeReqStat(long id, string rems, int tp)
        {
            try
            {
                string typ = string.Empty;

                switch (tp)
                {
                    case 1:
                        typ = ServiceChargesInstallmentsStatus.Approved.ToString();
                        break;
                    case 2:
                        typ = ServiceChargesInstallmentsStatus.Rejected.ToString();
                        break;
                    default:
                        typ = ServiceChargesInstallmentsStatus.Rejected.ToString();
                        break;
                }

                var uid = User.Identity.GetUserId<long>();
                db.Sp_Update_MeterChange_Req_Stat(id, typ, uid, rems);

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public JsonResult GetReqsCountForHome()
        {
            var met_chg_reqs = db.Electricity_Meter_Reqs.Where(x => x.Approval_Status == "Pending").Count();
            var inst_reqs = db.ServiceChargesInstallments.Where(x => x.Status == "Pending").Count();
            var met_red_reqs = db.ServiceChargesPermissions.Where(x => x.ModuleType == ServiceChargeRequestsType.Manual_Meter_Reading.ToString() && x.Status == null).Count();
            var other_Reqs = db.ServiceChargesPermissions.Where(x => x.ModuleType != ServiceChargeRequestsType.Manual_Meter_Reading.ToString() && x.Status == null).Count();
            return Json(new { met_chg_reqs, inst_reqs, met_red_reqs, other_Reqs });
        }

        public void UpdateBillNo()
        {
            var res = db.Electricity_Bill.Where(x => x.Month.Year == DateTime.Now.Year && x.Month.Month == DateTime.Now.Month).Select(x => x.Id).ToList();

            foreach (var v in res)
            {
                db.TEMP_PROC_TEST(v);
            }
        }

        public void UpdateInstallmentsStatus()
        {
            var res = db.ServiceChargesInstallments.ToList();

            foreach (var v in res)
            {
                if (v.Meter_Id is null)
                {
                    //service charges
                    var insts = db.SvcCharges_Installments_Structure.Where(x => x.InstallmentId == v.Id).ToList();
                    var bills = db.ServiceCharges_Bill.Where(x => x.Plot_Id == v.Plot_Id).ToList();
                    foreach (var item in insts)
                    {
                        var foundBill = bills.Where(x => (x.Date.Month == item.Installment_Month.Value.Month) && (x.Date.Year == item.Installment_Month.Value.Year)).FirstOrDefault();
                        if (foundBill != null)
                        {
                            if (foundBill.PaidAmount_Date != null)
                            {
                                item.Paid_Bill_Id = foundBill.Id;
                                item.Paid_Date = foundBill.PaidAmount_Date;
                                item.Status = "Paid";
                                db.SvcCharges_Installments_Structure.Attach(item);
                                db.Entry(item).Property(x => x.Paid_Date).IsModified = true;
                                db.Entry(item).Property(x => x.Paid_Bill_Id).IsModified = true;
                                db.Entry(item).Property(x => x.Status).IsModified = true;
                                long userid = long.Parse(User.Identity.GetUserId());
                                db.Sp_Add_Activity(userid, "Updated Services Charges Bill ", "Update", "Activity_Record", ActivityType.Services.ToString(), foundBill.Id);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    //electricity bill
                    var insts = db.SvcCharges_Installments_Structure.Where(x => x.InstallmentId == v.Id).ToList();
                    var bills = db.Electricity_Bill.Where(x => x.Meter_Id == v.Meter_Id).ToList();
                    foreach (var item in insts)
                    {
                        var foundBill = bills.Where(x => (x.Month.Month == item.Installment_Month.Value.Month) && (x.Month.Year == item.Installment_Month.Value.Year)).FirstOrDefault();
                        if (foundBill != null)
                        {
                            if (foundBill.AmountPaid_Date != null)
                            {
                                item.Paid_Bill_Id = foundBill.Id;
                                item.Paid_Date = foundBill.AmountPaid_Date;
                                item.Status = "Paid";
                                db.SvcCharges_Installments_Structure.Attach(item);
                                db.Entry(item).Property(x => x.Paid_Date).IsModified = true;
                                db.Entry(item).Property(x => x.Paid_Bill_Id).IsModified = true;
                                db.Entry(item).Property(x => x.Status).IsModified = true;
                                long userid = long.Parse(User.Identity.GetUserId());
                                db.Sp_Add_Activity(userid, "Updated Electricity Charges Bill ", "Update", "Activity_Record", ActivityType.Services.ToString(), foundBill.Id);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayServiceCharges(long PlotId, ReceiptData rd, decimal Amount, long TransactionId, string FileImg)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
            //get phase
            var block = db.RealEstate_Blocks.Where(b => b.Id == res1.BlockIden).FirstOrDefault();
            var phase = db.RealEstate_Phases.Where(p => p.Id == block.Phase_Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).FirstOrDefault();
            var res4 = db.Sp_Get_PlotLastServiceCharges(PlotId).OrderByDescending(x => x.Id).FirstOrDefault();
            //if(res4.Net_Total > Amount)
            //{
            //    return Json(false);
            //}
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res3 = db.Sp_Add_Receipt(Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, res2.Mobile_1
                           , res2.Father_Husband, PlotId, res2.Name, rd.PaymentType, 0, "Grand City Kharian", 0, null, res1.Plot_Size, ReceiptTypes.ServiceCharges.ToString(), userid, userid, "", null, Modules.PlotManagement.ToString(), "", res1.Plot_No, res1.Block_Name, res1.Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                    // add phase in Receipt data
                    var receiptdata = db.Receipts.Where(r => r.Id == res3.Receipt_Id).FirstOrDefault();
                    receiptdata.Phase = phase.Phase_Name;
                    db.SaveChanges();

                    db.Sp_Add_Activity(userid, "Payed Services Charges Bill ", "Update", "Activity_Record", ActivityType.Services.ToString(), PlotId);
                    db.Sp_Update_PlotServiceChargesAmount(res4.Id, Amount);
                    bool headcashier = false;
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }

                    string narration = "Service Charges Against :" + res1.Plot_No.ToString() + " - " + res1.Block_Name.ToString();
                    int line = 1;
                    if (headcashier)
                    {
                        if (rd.PaymentType == "Cash" )
                        {
                            var Payment = ah.Payment_Head(rd.PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                            var Block_COA = ah.HeadFinder(AccountingModuleFP, COA_Mapper_ModuleTypes.Service_Charges_Income.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.BlockIden);//db.Sp_Get_CA_Head_MultiRef_4("Income", "Other Income", "Service Charges", Block + " Block").FirstOrDefault();

                            var Debit = db.Sp_Add_Journal_Entry(Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, narration, TransactionId, line, userid, userid, null, rd.Bank, rd.PayChqNo, Payment.PaymentStatus, rd.Ch_bk_Pay_Date, null, "", null, res3.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_Journal_Entry(0, Amount, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, narration, TransactionId, line, userid, userid, null, rd.Bank, rd.PayChqNo, Payment.PaymentStatus, rd.Ch_bk_Pay_Date, null, "", null, res3.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit

                        }
                    }
                    else
                    {
                        if (rd.PaymentType == "Cash")
                        {
                            var Payment = ah.Payment_Head(rd.PaymentType, Transaction_Type.Debit.ToString(),null, comp.Id);
                            var Block_COA = ah.HeadFinder(AccountingModuleFP, COA_Mapper_ModuleTypes.Service_Charges_Income.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.BlockIden);//db.Sp_Get_CA_Head_MultiRef_4("Income", "Other Income", "Service Charges", Block + " Block").FirstOrDefault();

                            var Debit = db.Sp_Add_General_Entry(Amount, 0, rd.Bank, rd.PayChqNo, Payment.PaymentStatus, rd.Ch_bk_Pay_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, narration, TransactionId, line, null, null, null, userid, res3.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                            var Credit = db.Sp_Add_General_Entry(0, Amount, rd.Bank, rd.PayChqNo, Payment.PaymentStatus, rd.Ch_bk_Pay_Date, Block_COA.Text_ChartCode + " - " + Block_COA.Head, Block_COA.Id, Block_COA.Text_ChartCode, Block_COA.Head, narration, TransactionId, line, null, null, null, userid, res3.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        }
                    }

                    var res = new { Receiptid = res3.Receipt_No, Token = userid };

                    var inst = db.ServiceChargesInstallments.Where(x => x.Plot_Id == res4.Plot_Id && x.ServiceType == ServiceType.ServiceCharges.ToString() && x.Status == ServiceChargesInstallmentsStatus.Approved.ToString()).OrderBy(x => x.Create_DateTime).FirstOrDefault();
                    if (inst != null)
                    {
                        var strctr = db.SvcCharges_Installments_Structure.Where(x => (x.InstallmentId == inst.Id) && ((x.Installment_Month.Value.Month == res4.Date.Month) && (x.Installment_Month.Value.Year == res4.Date.Year)) && (x.Status == MonthlyInstallmentStatus.Pending.ToString())).FirstOrDefault();
                        if (strctr != null)
                        {
                            strctr.Status = "Paid";
                            strctr.Paid_Date = DateTime.Now;
                            strctr.Paid_Bill_Id = res4.Id;
                            db.SvcCharges_Installments_Structure.Attach(strctr);
                            db.Entry(strctr).Property(x => x.Status).IsModified = true;
                            db.Entry(strctr).Property(x => x.Paid_Date).IsModified = true;
                            db.Entry(strctr).Property(x => x.Paid_Bill_Id).IsModified = true;
                            db.SaveChanges();
                        }
                    }

                    if (rd.PaymentType != "Cash")
                    {
                        var chqData = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                            Modules.PlotManagement.ToString(), ReceiptTypes.ServiceCharges.ToString(), userid, rd.PayChqNo, PlotId, rd.Ch_bk_Pay_Date, rd.FilePlotNumber, res3.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());


                        if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                        }
                        string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                        var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                        Helpers h = new Helpers();
                        var Images = h.SaveBase64Image(FileImg, pathMain, res3.ToString());
                    }
                    Transaction.Commit();
                    return Json(res);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return Json(new Return { Msg = "Cannot Process Transaction", Status = false });
                }
            }
        }
        public void NotifyUserAboutBillViaSMS(string name, string mobNo, string amount, DateTime duedate, string type, string pltNo)
        {
            var msgtext =
                   "Dear " + name + ",\n\r" +
                   ".\n\r" +
                   "Your " + type + " bill has been generated for month : " + duedate.ToString("MMM-yyyy") + " against Plot : " + pltNo + ".\n\r" +
                   "Amount : " + amount + ".\n\r" +
                   "Due Date : " + duedate.ToString("dd-MMM-yyyy") + ".\n\r" +
                   "Please pay before due date to avoid late payment surcharge.\n\r" +
                   "S A Gardens.";
            try
            {
                SmsService smsService = new SmsService();
                smsService.SendMsg(msgtext, mobNo);
                //db.sp_add_fo(item.Id, msgtext, 0, "Text");
            }
            catch (Exception ee)
            {
                EmailService e = new EmailService();
                e.SendEmail(mobNo + " & " + name + " & " + duedate.ToString("dd-MMM-yyyy"), "daniyal@sasystems.solutions", "Service Charges Msg Not Sent");
            }
        }

    }
}
