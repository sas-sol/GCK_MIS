using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using MeherEstateDevelopers.Filters;
using Newtonsoft.Json;
using System.IO;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class PropertyDealController : Controller
    {
        // GET: PropertyDeal
        private Grand_CityEntities db = new Grand_CityEntities();
        [NoDirectAccess] public ActionResult AllDeales()
        {
            var res = db.Sp_Get_PropertyDeals().ToList().OrderByDescending(x => x.Datetime);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed All Deals  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(res);
        }
        [NoDirectAccess] public ActionResult CreateDeal()
        {
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == "General" || x.Type == "Building").ToList(), "Project_Name", "Project_Name");
            ViewBag.Block = new SelectList(db.Sp_Get_Block().ToList(), "Id", "Block_Name");
            ViewBag.NewLeadPlots = new SelectList(db.Plots.Where(x => x.Status == PlotsStatus.Available_For_Sale.ToString()).Select(x => new { Id = x.Id, Name = x.Plot_Number + " " + x.Sector + " - " + x.Type + " - " + x.Block_Name }).ToList(), "Id", "Name");

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  Create Deal  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateDeal(Property_Deal p, string Buyer_Seller, string File_Plot, string Name, string Mobile, string Address, decimal? Offered_Price, decimal? Demand, decimal? RatePerMarla, string Source, long? PlotId, string Projects, long? UnitNo)
        {
            var userid = long.Parse(User.Identity.GetUserId());

            var u = db.Users.Find(userid);
            string Company = u.Email.Split('@')[1];
            string Comp = "SAM";
            if (Company == "sa.marketing") { Comp = "SAM"; }
            else if (Company == "sagpropertyexchange.com") { Comp = "SPE"; }
            var dealNumber = db.Sp_Add_SAM_Deal_Number(Comp + "-").FirstOrDefault();
            if (Projects == "SA Premium Homes")
            {
                var res = db.Sp_Get_CommercialData(UnitNo).FirstOrDefault();
                if (p.Type == "New Lead")
                {
                    var a = db.Property_Deal.Any(x => x.File_Plot_Number == res.ApplicationNo && x.Plot_Type == res.Type && x.Status == LeadsStatus.Close.ToString());
                    if (a)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var a = db.Property_Deal.Any(x => x.File_Plot_Number == res.ApplicationNo && x.Plot_Type == res.Type && x.Status != LeadsStatus.Close.ToString());
                    if (a)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }

                Property_Deal deal = new Property_Deal()
                {
                    Block = " ",
                    Datetime = DateTime.Now,
                    File_Plot_Number = res.ApplicationNo,
                    Plot_Type = res.Type,
                    Size = res.Area.ToString(),
                    Status = LeadsStatus.Initial_Discussion.ToString(),
                    Type = p.Type,
                    Company = Comp,
                    Userid = userid,
                    DealNumber = dealNumber.ToString(),
                    Project = Projects
                };
                db.Property_Deal.Add(deal);
                db.SaveChanges();

                Property_Deal_Parties pd = new Property_Deal_Parties()
                {
                    Address = Address,
                    Commession = 0,
                    Datetime = DateTime.Now,
                    Deal_Id = deal.Id,
                    Mobile = Mobile,
                    Name = Name,
                    Offered_Rate = Offered_Price,
                    Rate_Per_Marla = RatePerMarla,
                    Type = Buyer_Seller,
                    Status = LeadsStatus.Initial_Discussion.ToString(),
                    Userid = userid,
                    Demand = Demand,
                    Source = Source
                };
                db.Property_Deal_Parties.Add(pd);
                db.SaveChanges();
            }
            else
            {
                if (File_Plot == "File")
                {
                    if (p.Type == "New Lead")
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == p.File_Plot_Number && x.Block == p.Block && x.Plot_Type == p.Plot_Type && x.Status == LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false);
                        }
                    }
                    else
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == p.File_Plot_Number && x.Block == p.Block && x.Plot_Type == p.Plot_Type && x.Status != LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false);
                        }
                    }
                    Property_Deal deal = new Property_Deal()
                    {
                        Block = " ",
                        Datetime = DateTime.Now,
                        File_Plot_Number = p.File_Plot_Number,
                        Plot_Type = "Residential",
                        Size = p.Size,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Type = p.Type,
                        Company = Comp,
                        Userid = userid,
                        DealNumber = dealNumber.ToString(),
                        Project = Projects
                    };
                    db.Property_Deal.Add(deal);
                    db.SaveChanges();

                    Property_Deal_Parties pd = new Property_Deal_Parties()
                    {
                        Address = Address,
                        Commession = 0,
                        Datetime = DateTime.Now,
                        Deal_Id = deal.Id,
                        Mobile = Mobile,
                        Name = Name,
                        Offered_Rate = Offered_Price,
                        Rate_Per_Marla = RatePerMarla,
                        Type = Buyer_Seller,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Userid = userid,
                        Demand = Demand,
                        Source = Source
                    };
                    db.Property_Deal_Parties.Add(pd);
                    db.SaveChanges();
                }
                else if (File_Plot == "Plot")
                {
                    var res = db.Sp_Get_PlotData(PlotId).FirstOrDefault();
                    if (res.Status != "Registered" && p.Type == "Resale")
                    {
                        return Json(false);
                    }
                    if (p.Type == "New Lead")
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == p.File_Plot_Number && x.Block == p.Block && x.Plot_Type == p.Plot_Type && x.Status == LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false);
                        }
                    }
                    else
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == p.File_Plot_Number && x.Block == p.Block && x.Plot_Type == p.Plot_Type && x.Status != LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false);
                        }
                    }
                    Property_Deal deal = new Property_Deal()
                    {
                        Block = res.Block_Name,
                        Datetime = DateTime.Now,
                        File_Plot_Number = res.Plot_No,
                        Plot_Type = res.Type,
                        Size = res.Plot_Size,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Type = p.Type,
                        Company = Comp,
                        Userid = userid,
                        DealNumber = dealNumber.ToString(),
                        Project = Projects
                    };
                    db.Property_Deal.Add(deal);
                    db.SaveChanges();

                    Property_Deal_Parties pd = new Property_Deal_Parties()
                    {
                        Address = Address,
                        Commession = 0,
                        Datetime = DateTime.Now,
                        Deal_Id = deal.Id,
                        Mobile = Mobile,
                        Name = Name,
                        Offered_Rate = Offered_Price,
                        Rate_Per_Marla = RatePerMarla,
                        Type = Buyer_Seller,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Userid = userid,
                        Demand = Demand,
                        Source = Source

                    };
                    db.Property_Deal_Parties.Add(pd);
                    db.SaveChanges();
                }
            }

            return Json(true);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess] public ActionResult CreateLeadToDeal(Property_Deal p, string Buyer_Seller, string File_Plot, string Name, string Mobile, string Address, decimal? Offered_Price, decimal? Demand, decimal? RatePerMarla, string Source, long? PlotId, long LeadId, string Project, long? UnitNo)
        {
            var userid = long.Parse(User.Identity.GetUserId());
            var u = db.Users.Find(userid);
            var deal_number = db.Leads.Where(x => x.Id == LeadId).Select(x => x.Deal_Number).FirstOrDefault();
            string Company = u.Email.Split('@')[1];
            string Comp = "SAM";
            if (Company == "sa.marketing") { Comp = "SAM"; }
            else if (Company == "sagpropertyexchange.com") { Comp = "SPE"; }
            long Deal_Id = 0, Party_Id = 0;
            if (Project == "SA Premium Homes")
            {
                var res = db.Sp_Get_CommercialData(UnitNo).FirstOrDefault();
                if (p.Type == "New Lead")
                {
                    var a = db.Property_Deal.Any(x => x.File_Plot_Number == res.ApplicationNo && x.Plot_Type == res.Type && x.Status == LeadsStatus.Close.ToString());
                    if (a)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var a = db.Property_Deal.Any(x => x.File_Plot_Number == res.ApplicationNo && x.Plot_Type == res.Type && x.Status != LeadsStatus.Close.ToString());
                    if (a)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }

                Property_Deal deal = new Property_Deal()
                {
                    Block = " ",
                    Datetime = DateTime.Now,
                    File_Plot_Number = res.ApplicationNo,
                    Plot_Type = res.Type,
                    Size = res.Area.ToString(),
                    Status = LeadsStatus.Initial_Discussion.ToString(),
                    Type = p.Type,
                    Company = Comp,
                    Userid = userid,
                    DealNumber = deal_number,
                    Project = Project
                };
                db.Property_Deal.Add(deal);
                db.SaveChanges();

                Property_Deal_Parties pd = new Property_Deal_Parties()
                {
                    Address = Address,
                    Commession = 0,
                    Datetime = DateTime.Now,
                    Deal_Id = deal.Id,
                    Mobile = Mobile,
                    Name = Name,
                    Offered_Rate = Offered_Price,
                    Rate_Per_Marla = RatePerMarla,
                    Type = Buyer_Seller,
                    Status = LeadsStatus.Initial_Discussion.ToString(),
                    Userid = userid,
                    Demand = Demand,
                    Source = Source
                };
                db.Property_Deal_Parties.Add(pd);
                db.SaveChanges();
                Deal_Id = deal.Id;
                Party_Id = pd.Id;
            }
            else
            {
                if (File_Plot == "File")
                {
                    if (p.Type == "New Lead")
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == p.File_Plot_Number && x.Block == p.Block && x.Plot_Type == p.Plot_Type && x.Status == LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == p.File_Plot_Number && x.Block == p.Block && x.Plot_Type == p.Plot_Type && x.Status != LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }

                    Property_Deal deal = new Property_Deal()
                    {
                        Block = " ",
                        Datetime = DateTime.Now,
                        File_Plot_Number = p.File_Plot_Number,
                        Plot_Type = "Residential",
                        Size = p.Size,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Type = p.Type,
                        Company = Comp,
                        Userid = userid,
                        DealNumber = deal_number,
                        Project = Project
                    };
                    db.Property_Deal.Add(deal);
                    db.SaveChanges();

                    Property_Deal_Parties pd = new Property_Deal_Parties()
                    {
                        Address = Address,
                        Commession = 0,
                        Datetime = DateTime.Now,
                        Deal_Id = deal.Id,
                        Mobile = Mobile,
                        Name = Name,
                        Offered_Rate = Offered_Price,
                        Rate_Per_Marla = RatePerMarla,
                        Type = Buyer_Seller,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Userid = userid,
                        Demand = Demand,
                        Source = Source
                    };
                    db.Property_Deal_Parties.Add(pd);
                    db.SaveChanges();
                    Deal_Id = deal.Id;
                    Party_Id = pd.Id;
                }
                else if (File_Plot == "Plot")
                {
                    var res = db.Sp_Get_PlotData(PlotId).FirstOrDefault();
                    if (res.Status != "Registered" && p.Type == "Resale")
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    if (p.Type == "New Lead")
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == p.File_Plot_Number && x.Block == p.Block && x.Plot_Type == p.Plot_Type && x.Status == LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == p.File_Plot_Number && x.Block == p.Block && x.Plot_Type == p.Plot_Type && x.Status != LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    Property_Deal deal = new Property_Deal()
                    {
                        Block = res.Block_Name,
                        Datetime = DateTime.Now,
                        File_Plot_Number = res.Plot_No,
                        Plot_Type = res.Type,
                        Size = res.Plot_Size,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Type = p.Type,
                        Company = Comp,
                        Userid = userid,
                        DealNumber = deal_number,
                        Project = Project
                    };
                    db.Property_Deal.Add(deal);
                    db.SaveChanges();

                    Property_Deal_Parties pd = new Property_Deal_Parties()
                    {
                        Address = Address,
                        Commession = 0,
                        Datetime = DateTime.Now,
                        Deal_Id = deal.Id,
                        Mobile = Mobile,
                        Name = Name,
                        Offered_Rate = Offered_Price,
                        Rate_Per_Marla = RatePerMarla,
                        Type = Buyer_Seller,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Userid = userid,
                        Demand = Demand,
                        Source = Source
                    };
                    db.Property_Deal_Parties.Add(pd);
                    db.SaveChanges();
                    Deal_Id = deal.Id;
                    Party_Id = pd.Id;
                }
                else
                {
                    var res = db.Sp_Get_CommercialData(UnitNo).FirstOrDefault();
                    if (p.Type == "New Lead")
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == res.ApplicationNo && x.Plot_Type == res.Type && x.Status == LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var a = db.Property_Deal.Any(x => x.File_Plot_Number == res.ApplicationNo && x.Plot_Type == res.Type && x.Status != LeadsStatus.Close.ToString());
                        if (a)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }

                    Property_Deal deal = new Property_Deal()
                    {
                        Block = " ",
                        Datetime = DateTime.Now,
                        File_Plot_Number = res.ApplicationNo,
                        Plot_Type = res.Type,
                        Size = res.Area.ToString(),
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Type = p.Type,
                        Company = Comp,
                        Userid = userid,
                        DealNumber = deal_number,
                        Project = Project
                    };
                    db.Property_Deal.Add(deal);
                    db.SaveChanges();

                    Property_Deal_Parties pd = new Property_Deal_Parties()
                    {
                        Address = Address,
                        Commession = 0,
                        Datetime = DateTime.Now,
                        Deal_Id = deal.Id,
                        Mobile = Mobile,
                        Name = Name,
                        Offered_Rate = Offered_Price,
                        Rate_Per_Marla = RatePerMarla,
                        Type = Buyer_Seller,
                        Status = LeadsStatus.Initial_Discussion.ToString(),
                        Userid = userid,
                        Demand = Demand,
                        Source = Source
                    };
                    db.Property_Deal_Parties.Add(pd);
                    db.SaveChanges();
                    Deal_Id = deal.Id;
                    Party_Id = pd.Id;
                }
            }

            var res7 = db.SAM_Receipts.Where(x => x.Lead_Id == LeadId).ToList();
            var res8 = db.SAM_Voucher.Where(x => x.Lead_Id == LeadId && x.VoucherNo != null).ToList();
            var res9 = db.Lead_Followups.Where(x => x.Lead_Id == LeadId).ToList();

            foreach (var item in res7)
            {
                var res1 = db.Sp_Add_PropertyDealReceipt_Move(item.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(item.Amount)), item.Bank, "", item.Ch_Pay_Draft_No, item.Ch_Pay_Draft_Date, item.Branch_Name, item.Contact
                                            , Deal_Id, item.Name, item.PaymentType, item.Plot_Total_Amount, item.RatePerMarla, item.Size, "Token", item.Id, item.Userid, item.File_Plot_Number, item.Block, item.Company, item.ReceiptNo, item.DateTime, item.Status, item.DayClose).FirstOrDefault();
            }
            foreach (var item in res8)
            {
                var res1 = db.Sp_Add_PropertyVoucherMove("", item.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(item.Amount)), item.Bank, item.Block, item.Branch_Name, item.Ch_Pay_Draft_Date, item.Ch_Pay_Draft_No, "",
                    item.Company, item.Contact, Deal_Id, Party_Id, item.Description, item.File_Plot_Number, item.Name, item.PaymentType, item.Plot_Total_Amount, item.RatePerMarla, item.Size, item.Id, item.Userid, item.Type, item.Userid, item.VoucherNo, item.DateTime, item.dayClose).FirstOrDefault();
            }
            foreach (var item in res9)
            {
                var res3 = db.Sp_Add_LeadFollowup(item.Description, item.Lead_Id, userid, "Text").FirstOrDefault();
            }
            db.Sp_Delete_LeadData(LeadId);
            return RedirectToAction("DealDetails", "PropertyDeal", new { Id = Deal_Id });
        }
        [NoDirectAccess] public ActionResult DealDetails(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.UserId = userid;
            var res1 = db.Property_Deal.Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.Name = db.Users.Find(res1.Userid).Name;
            var res2 = db.Sp_Get_PropertyDeals_Parties(Id).ToList();
            var res3 = db.PropertyDeal_Receipts.Where(x => x.Lead_Id == Id && x.Cancel == null).ToList();
            var res4 = db.PropertyDeal_Voucher.Where(x => x.Deal_Id == Id && x.Cancel == null).ToList();

            var res5 = db.Sp_Get_PropertyDeal_Request_Deal(Id).ToList();
            var res7 = res3.Where(x => x.Type != "Direct Payment" && x.Cancel == null).Select(x => new DealLedger { Amount = x.Amount, Name = x.Name, Receipt = "Receipt", ReceiptNo = x.ReceiptNo, Type = x.Type, Date = x.DateTime, Status = x.Status, PaymentType = x.PaymentType }).ToList();
            var res8 = db.PropertyDeal_Voucher.Where(x => x.Deal_Id == Id && x.Cancel == null).Select(x => new DealLedger { Amount = x.Amount * -1, Name = x.Name, Receipt = "Voucher", ReceiptNo = x.VoucherNo, Type = x.Type, Date = x.DateTime, Status = "", PaymentType = x.PaymentType }).ToList();

            List<DealLedger> Ledgers = new List<DealLedger>();
            Ledgers.AddRange(res7);
            Ledgers.AddRange(res8);

            var res9 = Enum.GetValues(typeof(LeadsStatus)).Cast<LeadsStatus>().Select(e => new { Value = e, Text = e.ToString().Replace("_", " ") }).ToList();
            ViewBag.Status = new SelectList(res9, "Value", "Text", res1.Status);
            var res = new DealModel { Deal = res1, Parties = res2, Request = res5, Ledger = Ledgers, Receipts = res3, Voucher = res4 };
            return View(res);
        }
        [NoDirectAccess] public ActionResult AddBuyer()
        {
            return PartialView();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult AddBuyer(Property_Deal_Parties p)
        {
            var userid = long.Parse(User.Identity.GetUserId());
            Property_Deal_Parties pd = new Property_Deal_Parties()
            {
                Address = p.Address,
                Commission_Type = p.Commission_Type,
                Datetime = DateTime.Now,
                Deal_Id = p.Deal_Id,
                Mobile = p.Mobile,
                Name = p.Name,
                Offered_Rate = p.Offered_Rate,
                Rate_Per_Marla = p.Rate_Per_Marla,
                Type = "Buyer",
                Status = LeadsStatus.Initial_Discussion.ToString(),
                Userid = userid,
                Demand = p.Demand
            };
            db.Property_Deal_Parties.Add(pd);
            db.SaveChanges();
            return Json(true);
        }
        [NoDirectAccess] public ActionResult UpdatePartyInfo(long Id)
        {
            var res = db.Property_Deal_Parties.Where(x => x.Id == Id).FirstOrDefault();
            var res1 = db.Sp_Get_PropertyDeals_Parties(res.Deal_Id).Select(x => new { x.Status, x.Type }).ToList();
            var res2 = db.Property_Deal.Where(x => x.Id == res.Deal_Id).SingleOrDefault();
            ViewBag.DealClose = res1.Any(x => x.Status == LeadsStatus.Close.ToString() && x.Type == "Buyer");
            ViewBag.Type = res2.Type;
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateParty_Info(Property_Deal_Parties p)
        {
            var res1 = db.Property_Deal_Parties.Where(x => x.Id == p.Id).SingleOrDefault();
            var res2 = db.PropertyDeal_Receipts.Any(x => x.Lead_Id == p.Deal_Id && x.Cancel == null);
            var res3 = db.PropertyDeal_Voucher.Any(x => x.Deal_Id == p.Deal_Id && x.Cancel == null);
            if (res1.Offered_Rate != p.Offered_Rate)
            {
                if (res2 || res3)
                {
                    var ret1 = new { Status = false, Msg = "The offered price cannot be updated as there are existing receipts/vouchers" };
                    return Json(ret1);
                }
            }
            var res = db.Sp_Update_DealParty_Information(p.Id, p.Name, p.Mobile, p.Address, p.Offered_Rate, p.Rate_Per_Marla, p.Commession, p.Status, p.Commission_Type);
            var ret = new { Status = true, Msg = "Information updated successfully" };
            return Json(ret);
        }
        public JsonResult PaymentRequest(string Type, decimal Amount, string Description, long Deal_Id, long Party_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
            var res1 = db.Sp_Get_PropertyDeals_Parties(Deal_Id).Select(x => new { x.Status, x.Id, x.Type }).ToList();
            var res2 = db.Property_Deal_Parties.Where(x => x.Id == Party_Id).FirstOrDefault();
            var res3 = db.Property_Deal.Where(x => x.Id == Deal_Id).FirstOrDefault();
            if (res3.Type == "Investment")
            {
                if (res1.Any(x => x.Status == LeadsStatus.Close.ToString() && x.Type == "Buyer"))
                {
                    if (!res1.Any(x => x.Id == Party_Id && x.Status == LeadsStatus.Close.ToString()))
                        return Json(false);
                }
            }
            //else if (res3.Type == "Resale")
            //{
            //    if (res1.Any(x => x.Status == LeadsStatus.Close.ToString() && x.Type == "Buyer"))
            //    {
            //        if (!res1.Any(x => x.Id == Party_Id && res2.Status == LeadsStatus.Close.ToString()))
            //            return Json(false);
            //    }
            //}
            else
            {
                if (res1.Any(x => x.Status == LeadsStatus.Close.ToString() && res2.Type == "Buyer"))
                {
                    if (!res1.Any(x => x.Id == Party_Id && res2.Status == LeadsStatus.Close.ToString()))
                        return Json(false);
                }
            }


            //if (res3.Type == "Investment" && Type != "Commession")
            //{
            //    if (res2.Type == "Buyer" && res2.Status == LeadsStatus.Close.ToString())
            //    {
            //        return Json(false);
            //    }
            //}
            //else
            //{

            //}

            //if (res3.Type == "Investment" && Type == "Voucher" )
            //{
            //    Deal_Request pd = new Deal_Request
            //    {
            //        Amount = Amount,
            //        Deal_Id = Deal_Id,
            //        Description = Description,
            //        Party_Id = Party_Id,
            //        Status = "Pending",
            //        Type = Type,
            //        Userid = userid
            //    };
            //    var packed = JsonConvert.SerializeObject(pd);
            //    db.MIS_Requests.Add(new MIS_Requests
            //    {
            //        CreatedAt = DateTime.Now,
            //        CreatedBy_Id = userid,
            //        CreatedBy_Name = uname,
            //        Details = packed,
            //        ModuleId = Deal_Id,
            //        ModuleType = "Investment Payment Out",
            //        Status = RequestStatus.Pending.ToString(),
            //        Type = RequestType.Investment_Payment_Out.ToString()
            //    });
            //    db.SaveChanges();
            //    return Json(new Return { Status = false, Msg = "Your Payment has been requested for Approval to Director of Sales" });
            //}
            //else
            //{
            var res = db.Sp_Add_PropertyDeal_Request(Type, Amount, Description, Deal_Id, Party_Id, userid);
            return Json(new Return { Status = true, Msg = "Payment has been requested successfully" });
            //}
        }
        [NoDirectAccess] public ActionResult PaymentsRequest()
        {
            var res = db.Sp_Get_PropertyDeal_Request("Pending").ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult CashierDealDetails(long Id)
        {
            var res1 = db.Property_Deal.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Sp_Get_PropertyDeals_Parties(Id).ToList();
            var res3 = db.PropertyDeal_Receipts.Where(x => x.Lead_Id == Id && x.Cancel == null).ToList();
            var res4 = db.PropertyDeal_Voucher.Where(x => x.Deal_Id == Id && x.Cancel == null).ToList();
            var res5 = db.Sp_Get_PropertyDeal_Request_Deal(Id).ToList();
            var res7 = db.PropertyDeal_Receipts.Where(x => x.Lead_Id == Id && x.Cancel == null && x.Type != "Direct Payment").Select(x => new DealLedger { Amount = x.Amount, Name = x.Name, Receipt = "Receipt", ReceiptNo = x.ReceiptNo, Type = x.Type, Date = x.DateTime }).ToList();
            var res8 = db.PropertyDeal_Voucher.Where(x => x.Deal_Id == Id && x.Cancel == null).Select(x => new DealLedger { Amount = x.Amount * -1, Name = x.Name, Receipt = "Voucher", ReceiptNo = x.VoucherNo, Type = x.Type, Date = x.DateTime }).ToList();

            List<DealLedger> Ledgers = new List<DealLedger>();
            Ledgers.AddRange(res7);
            Ledgers.AddRange(res8);
            var res6 = Enum.GetValues(typeof(LeadsStatus)).Cast<LeadsStatus>().Select(e => new { Value = e, Text = e.ToString().Replace("_", " ") }).ToList();
            ViewBag.Status = new SelectList(res6, "Value", "Text", res1.Status);
            var res = new DealModel { Deal = res1, Parties = res2, Request = res5, Ledger = Ledgers, Receipts = res3, Voucher = res4 };

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed  Cashier Deals Details  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }
        [NoDirectAccess] public ActionResult PaymentRequestDetails(long Id)
        {
            var res = db.Sp_Get_PropertyDeal_Request_Id(Id).FirstOrDefault();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");

            ViewBag.TransactionId = new Helpers().RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Payment Request Details  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        public JsonResult PaymentGenerate(long? Id, string Bank, string PayChqNo, DateTime? Ch_bk_Pay_Date, string Branch, string PaymentType, string ch_Pay_Draft_To, string FileImg, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_PropertyDeal_Request_Id(Id).FirstOrDefault();
            var re = db.Property_Deal.Where(x => x.Id == res.Deal_Id).FirstOrDefault();
            if (res.Type == "Voucher")
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res1 = db.Sp_Add_PropertyVoucher("", res.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(res.Amount)), Bank, res.Block, Branch, Ch_bk_Pay_Date, PayChqNo, ch_Pay_Draft_To,
                        res.Company, res.Mobile, res.Deal_Id, res.Party_Id, res.File_Plot_Number + " " + res.Plot_Type + " " + res.Block + " " + res.Description, res.File_Plot_Number, res.Name, PaymentType, res.Offered_Rate, res.Rate_Per_Marla, res.Size, res.Id, userid, re.Company, res.Userid, TransactionId).FirstOrDefault();
                        bool headcashier = false;
                        if (User.IsInRole("Head Cashier"))
                        {
                            headcashier = true;
                        }
                        var recNo = db.PropertyDeal_Voucher.Where(x => x.Deal_Id == res.Deal_Id && x.DealParty_Id == res.Party_Id && x.Amount == res.Amount).OrderByDescending(y => y.DateTime).Select(x => x.VoucherNo).FirstOrDefault();
                        AccountHandlerController de = new AccountHandlerController();
                        de.SAM_Vouchers(res.Amount, PaymentType, PayChqNo, Ch_bk_Pay_Date, Bank, new Helpers().RandomNumber(), userid, recNo, 1, re.DealNumber, headcashier);
                        db.Sp_Update_PropertyDeal_RequestStatus_Id(res.Id, "Processed");
                        var res2 = db.Sp_Update_PropertyDeals_PartyStatus(res.Party_Id, LeadsStatus.Close.ToString());

                        Transaction.Commit();
                        var Data = new { Status = true, VoucherId = res1, Token = res.Id, Type = res.Type };
                        return Json(Data);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        var Data = new { Status = false };
                        return Json(Data);
                    }
                }


            }
            else if (res.Type == "Receipt")
            {
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res1 = db.Sp_Add_PropertyDealReceipt(res.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(res.Amount)), Bank, ch_Pay_Draft_To, PayChqNo, Ch_bk_Pay_Date, Branch, res.Mobile
                                                                       , res.Deal_Id, res.Name, PaymentType, res.Offered_Rate, res.Rate_Per_Marla, res.Size, "Token", res.Id, userid, res.File_Plot_Number, res.Block, re.Company, TransactionId).FirstOrDefault();
                        bool headcashier = false;
                        if (User.IsInRole("Head Cashier"))
                        {
                            headcashier = true;
                        }
                        try
                        {
                            AccountHandlerController de = new AccountHandlerController();
                            de.SAM_Installment(res.Amount, PaymentType, PayChqNo, Ch_bk_Pay_Date, Bank, TransactionId, userid, res1.Receipt_No, 1, re.DealNumber, headcashier);
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            var a = new { Status = false };
                            return Json(a);
                        }

                        if (PaymentType != "Cash")
                        {
                            if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                            }
                            string dt = string.Format("{0:d/M/yyyy}", Ch_bk_Pay_Date);
                            var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), PayChqNo + "_" + Bank + "_" + dt.Replace("/", "_") + ".jpg");
                            Helpers h = new Helpers();
                            var Images = h.SaveBase64Image(FileImg, pathMain, res1.Receipt_No.ToString());
                        }

                        db.Sp_Update_PropertyDeal_RequestStatus_Id(res.Id, "Processed");
                        var res2 = db.Sp_Update_PropertyDeals_PartyStatus(res.Party_Id, LeadsStatus.Close.ToString());
                        var Data = new { Status = true, ReceiptId = res1.Receipt_Id, Token = res.Id, Type = res.Type };






                        Transaction.Commit();
                        return Json(Data);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        var Data = new { Status = false };
                        return Json(Data);
                    }
                }


            }
            else if (res.Type == "Commession")
            {
                var res1 = db.Sp_Add_PropertyDealReceipt(res.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(res.Amount)), Bank, ch_Pay_Draft_To, PayChqNo, Ch_bk_Pay_Date, Branch, res.Mobile
                                           , res.Deal_Id, res.Name, PaymentType, res.Offered_Rate, res.Rate_Per_Marla, res.Size, "Commession", res.Id, userid, res.File_Plot_Number, res.Block, res.Company, TransactionId).FirstOrDefault();
                bool headcashier = false;
                if (User.IsInRole("Head Cashier"))
                {
                    headcashier = true;
                }
                try
                {
                    AccountHandlerController de = new AccountHandlerController();
                    de.SAM_Installment(res.Amount, PaymentType, PayChqNo, Ch_bk_Pay_Date, Bank, new Helpers().RandomNumber(), userid, res1.Receipt_No, 1, re.DealNumber, headcashier);
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    var a = new { Status = false };
                    return Json(a);
                }

                db.Sp_Update_PropertyDeal_RequestStatus_Id(res.Id, "Processed");
                var Data = new { Status = true, ReceiptId = res1.Receipt_Id, Token = res.Id, Type = res.Type };
                return Json(Data);
            }
            else
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult CurrentBalance()
        {
            var res1 = db.PropertyDeal_Receipts.Where(x => x.Type != "Direct Payment").Select(x => new DealLedger { Block = x.Block, Plot_number = x.File_Plot_Number, Amount = x.Amount, Name = x.Name, Receipt = "Receipt", ReceiptNo = x.ReceiptNo, Type = x.Type, Date = x.DateTime }).ToList();
            var res2 = db.PropertyDeal_Voucher.Select(x => new DealLedger { Block = x.Block, Plot_number = x.File_Plot_Number, Amount = x.Amount * -1, Name = x.Name, Receipt = "Voucher", ReceiptNo = x.VoucherNo, Type = x.Type, Date = x.DateTime }).ToList();
            List<DealLedger> Ledgers = new List<DealLedger>();
            Ledgers.AddRange(res1);
            Ledgers.AddRange(res2);
            return View(Ledgers);
        }
        [NoDirectAccess] public ActionResult DirectDeal(long Id)
        {
            var res2 = db.Property_Deal_Parties.Where(x => x.Deal_Id == Id).ToList();
            var res3 = db.PropertyDeal_Receipts.Where(x => x.Lead_Id == Id && x.Cancel == null && x.Type == "Commession").ToList();
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            var res = new PropertyDeal { Parties = res2, Receipts = res3 };

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Direct Deal Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return PartialView(res);
        }
        public JsonResult DirectPayment(long? Deal_Id, decimal Amount, string Bank, string PayChqNo, DateTime? Ch_bk_Pay_Date, string Branch, string PaymentType, string ch_Pay_Draft_To, long SellerId, long BuyerId)
        {
            var TransactionId = new Helpers().RandomNumber();
            long userid = long.Parse(User.Identity.GetUserId());
            var buyer = db.Property_Deal_Parties.Where(x => x.Id == BuyerId).SingleOrDefault();
            var seller = db.Property_Deal_Parties.Where(x => x.Id == SellerId).SingleOrDefault();
            var res1 = db.Sp_Add_PropertyDealReceipt(Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), Bank, ch_Pay_Draft_To, PayChqNo, Ch_bk_Pay_Date, Branch, buyer.Mobile
                                       , Deal_Id, buyer.Name, PaymentType, buyer.Offered_Rate, buyer.Rate_Per_Marla, seller.Name, "Direct Payment", seller.Id, userid, BuyerId.ToString(), "", "", TransactionId).FirstOrDefault();



            return Json(true);
        }
        public JsonResult UpdateDealReceipt(long Id, string Description)
        {
            db.Sp_Update_PropertyDealCheque(Id, Description);
            return Json(true);
        }
        public JsonResult RemoveRequest(long Id)
        {
            var res = db.Sp_Delete_PropertyDeal_Req(Id);
            return Json(true);
        }
        [NoDirectAccess] public ActionResult CommissionApproval()
        {
            var res = db.Sp_Get_PropertyDeal_CommissionReq().ToList();
            return View(res);
        }
        public JsonResult UpdateApproval(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Update_PropertyDeal_Com_Appr(Id, "", userid);
            return Json(true);
        }
        public JsonResult ReturnAmount(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.PropertyDeal_Receipts.Where(x => x.Id == Id).SingleOrDefault();
            var res1 = db.Sp_Add_PropertyVoucher("", res.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(res.Amount)), res.Bank, res.Block, res.Branch_Name, res.Ch_Pay_Draft_Date, res.Ch_Pay_Draft_No, "",
                    res.Company, res.Contact, res.Lead_Id, 0, "Amount Refund Against Cancelation of receipt no & Booking of Plot no " + res.ReceiptNo + "   " + res.File_Plot_Number + " " + res + " " + res.Block + " respectively.", res.File_Plot_Number, res.Name, "Cash", 0, 0, res.Size, res.Id, userid, res.Type, res.Userid, res.TransactionId).FirstOrDefault();
            var res2 = db.Sp_Update_ProptyDealReceipt_Cancel(Id, "Amount Refund Against Cancelation of Booking of Plot " + res.File_Plot_Number + " " + res + " " + res.Block + " ");
            var Data = new { VoucherId = res1, Token = res.Id, Type = res.Type };
            return Json(Data);
        }
        public void UpdatePropertyDeal()
        {
            var res = db.Property_Deal.ToList();
            foreach (var item in res)
            {
                var Buyer = db.Property_Deal_Parties.Where(x => x.Deal_Id == item.Id && x.Type == "Buyer").ToList();
                var Seler = db.Property_Deal_Parties.Where(x => x.Deal_Id == item.Id && x.Type == "Seller").ToList();

                if (item.Type == "New_Lead")
                {
                    if (Buyer.Select(x => x.Status).FirstOrDefault() == LeadsStatus.Close.ToString())
                    {
                        item.Status = LeadsStatus.Close.ToString();
                        using (Grand_CityEntities edb = new Grand_CityEntities())
                        {
                            edb.Property_Deal.Attach(item);
                            var entry = edb.Entry(item);
                            entry.Property(e => e.Status).IsModified = true;
                            edb.SaveChanges();
                        };

                    }
                }
                else if (item.Type == "Resale" || item.Type == "Investment")
                {
                    if (Buyer.Any() && Seler.Any())
                    {
                        if (Buyer.Select(x => x.Status).FirstOrDefault() == LeadsStatus.Close.ToString() && Seler.Select(x => x.Status).FirstOrDefault() == LeadsStatus.Close.ToString())
                        {
                            item.Status = LeadsStatus.Close.ToString();
                            using (Grand_CityEntities edb = new Grand_CityEntities())
                            {
                                edb.Property_Deal.Attach(item);
                                var entry = edb.Entry(item);
                                entry.Property(e => e.Status).IsModified = true;
                                edb.SaveChanges();
                            };
                        }
                    }
                }
            }
        }
        [NoDirectAccess] public ActionResult PremiumHomes()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult PremiumHomesInventory()
        {
            var project = "SA Premium Homes";
            var res = db.Sp_Get_Commercial_Project_Inventory(project).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult PremiumHomesLeads(DateTime? From, DateTime? To, long? salesperson)
        {
            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive" || y.Name == "Sales Manager")).ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");
            var project = "SA Premium Homes";
            if (From == null || To == null)
            {
                var fy = GetFinancialYear();
                From = fy.Start;
                To = fy.End;
            }
            var res = db.Sp_Get_Commercial_Project_Leads(project, From, To, salesperson).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult PremiumUnassignedLeads(DateTime? From, DateTime? To)
        {
            var project = "SA Premium Homes";
            ViewBag.Project = project;
            if (From == null || To == null)
            {
                var fy = GetFinancialYear();
                From = fy.Start;
                To = fy.End;
            }
            var res = db.Sp_Get_Project_Unassigned_Leads(project, From, To).ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult AssignNewLeads()
        {
            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Active == 1).ToList();
            var project = "SA Premium Homes";
            ViewBag.Project = project;
            var fy = GetFinancialYear();
            ViewBag.TotalLeads = db.Leads.Where(x => x.Project == project && x.AssignedTo == 0).Count();
            return PartialView(All);
        }
        private FinancialYear GetFinancialYear()
        {
            return new FinancialYear { Start = new DateTime(2022, 7, 1), End = new DateTime(2023, 6, 30) };
        }
        [NoDirectAccess] public ActionResult PremiumHomesRoom(long Id)
        {
            var res1 = db.Sp_Get_CommercialData(Id).FirstOrDefault();
            var res2 = db.Sp_Get_Receipts_Param(res1.Project_Name, ReceiptTypes.BookingToken.ToString(), res1.shop_no).ToList();
            var res3 = db.Sp_Get_PropertyDeals_Param(res1.shop_no).ToList();
            var res = new PremiumHomeRoomModel { RoomData = res1, Receipts = res2, Leads = res3 };
            return View(res);
        }
        [NoDirectAccess] public ActionResult PremiumHomeLead()
        {
            return PartialView();
        }
        [NoDirectAccess] public ActionResult PremiumLeadInfo(long Id)
        {
            var res = db.Property_Deal_Parties.Where(x => x.Id == Id).FirstOrDefault();
            var res1 = db.Sp_Get_PropertyDeals_Parties(res.Deal_Id).Select(x => new { x.Status, x.Type }).ToList();
            var res2 = db.Property_Deal.Where(x => x.Id == res.Deal_Id).SingleOrDefault();
            ViewBag.DealClose = res1.Any(x => x.Status == LeadsStatus.Close.ToString() && x.Type == "Buyer");
            ViewBag.Type = res2.Type;
            return PartialView(res);
        }
        public JsonResult PremiumLeadInfoUpdate(Property_Deal_Parties p)
        {
            var res = db.Sp_Update_PremiumLead_Information(p.Id, p.Name, p.Mobile, p.Father_Name, p.Address);
            var ret = new { Status = true, Msg = "Information Updated" };
            return Json(ret);
        }
        public JsonResult PremiumLeadPaymentRequest(string Type, decimal Amount, string Description, long Deal_Id, long Party_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();

            var res1 = db.Sp_Get_PropertyDeals_Parties(Deal_Id).Select(x => new { x.Status, x.Id, x.Type }).ToList();
            var res2 = db.Property_Deal_Parties.Where(x => x.Id == Party_Id).FirstOrDefault();
            //var res3 = db.Property_Deal.Where(x => x.Id == Deal_Id).FirstOrDefault();
            if (res1.Any(x => x.Status == LeadsStatus.Close.ToString() && res2.Type == "Buyer"))
            {
                if (!res1.Any(x => x.Id == Party_Id && res2.Status == LeadsStatus.Close.ToString()))
                    return Json(false);
            }
            var res = db.Sp_Add_PropertyDeal_Request(Type, Amount, Description, Deal_Id, Party_Id, userid);
            return Json(new Return { Status = true, Msg = "Your Payment has been requested" });
            //}
        }
        public JsonResult AssignLeadsToSales(List<NewLeadModel> SalesLeads)
        {
            if (SalesLeads.Any())
            {
                var project = "SA Premium Homes";
                foreach (var v in SalesLeads)
                {
                    db.Sp_Update_Leads_Assign(project, v.SalesPersonId, v.LeadsCount);
                }
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult PremiumLeadDetails(long Id)
        {
            var project = "SA Premium Homes";
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_Lead(Id).SingleOrDefault();
            if (res1 == null)
            {
                return RedirectToAction("PremiumHomesLeads");
            }
            var projDet = db.RealEstate_Projects.Where(x => x.Project_Name == project).FirstOrDefault();

            ViewBag.Floor = new SelectList(db.Sp_Get_Project_CommercialFloor_Parameter(projDet.Id).Select(x => new { Id = x.Id, Name = x.Floor }), "Id", "Name");
            ViewBag.Units = new SelectList(db.Commercial_Rooms.Where(x => x.Project_Name == project && x.Status == PlotsStatus.Available_For_Sale.ToString()).Select(x => new { Id = x.Id, Name = x.ApplicationNo }), "Id", "Name");
            var res4 = Enum.GetValues(typeof(LeadsStatus)).Cast<LeadsStatus>().Select(e => new { Value = e, Text = e.ToString().Replace("_", " ") }).ToList();
            ViewBag.LeadStatus = new SelectList(res4, "Value", "Text", res1.LeadStatus);
            var res7 = db.Receipts.Where(x => x.File_Plot_Number == res1.Deal_Number && x.Project == project && x.Type == ReceiptTypes.BookingToken.ToString() && x.Cancel == null).ToList();
            var res8 = db.Vouchers.Where(x => x.Vendor_Id == Id && x.VoucherNo != null && x.Cancel == null).ToList();
            var res = new PremiumHomesLeadsData { Lead = res1, Receipts = res7, Vouchers = res8 };
            return View(res);
        }
        public JsonResult GetUnits(long Id)
        {
            var res = db.Commercial_Rooms.Where(x => x.Floor_Id == Id && x.Status == PlotsStatus.Available_For_Sale.ToString()).ToList();/*   new SelectList(db.Commercial_Rooms.Where(x => x.Plan_Id == proid && x.Floor_Id== Id),"Id", "Com_App_Shop_Number");*/
            return Json(res);
        }
        public JsonResult UpdateLeads(string PreLeadStatus, Lead L)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_Lead(L.Id, L.Name, L.Father_Husband, L.Mobile_1, L.Address, null, L.LeadStatus, null, null);
            string Desc = "";
            if (PreLeadStatus != L.LeadStatus)
            {
                Desc = Desc + "Lead Status " + PreLeadStatus + " Change To : " + L.LeadStatus + " -- ";
            }
            if (!(string.IsNullOrEmpty(Desc) || string.IsNullOrWhiteSpace(Desc)))
            {
                var res4 = db.Sp_Add_LeadFollowup(Desc, L.Id, userid, "Status");
            }
            return Json(true);
        }
        //public JsonResult RequestReceiptPayment(long Id, decimal Amount, long unitId)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res1 = db.Sp_Get_Lead(Id).SingleOrDefault();
        //    var res2 = db.Commercial_Rooms.Where(x => x.Id == unitId).FirstOrDefault();
        //    var floor = db.Commercial_FloorsPlan.Where(x => x.Id == res2.Floor_Id).FirstOrDefault();
        //    var res = db.Sp_Add_Receipt_Request(res1.Name, res1.Father_Husband, res1.Mobile_1, Modules.CommercialManagement.ToString(), unitId, "", "", Amount, ReceiptTypes.BookingToken.ToString(), userid, res2.ApplicationNo + " - " + floor.Floor, "Pending", res1.Id, res2.Project_Name).FirstOrDefault();
        //    //var res = db.Sp_Add_SAM_Voucher_Req(Amount, GeneralMethods.NumberToWords(Convert.ToInt32(Amount)), userid, res1.Name, res1.Father_Husband, res1.Block, "", Id, 0, "", "", res1.Offered_Price, res1.Mobile_1, res1.Address, Description, "", Comp);
        //    return Json(true);
        //}

        [NoDirectAccess] public ActionResult CommercialProjectReceiptRequest()
        {
            //var project = "SA Premium Homes";
            // Change  
            var res = db.ReceiptRequests.Where(x => x.Status == "Pending").ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult CommercialProjectReceipt(long Id)
        {
            ViewBag.TransactionId = new Helpers().RandomNumber();
            var res = db.Sp_Get_Commercial_Receipt_Request(Id).FirstOrDefault();
            ViewBag.Status = db.Commercial_Rooms.Where(x => x.Id == res.Module_Id).Select(x => x.Status).FirstOrDefault();
            return PartialView(res);
        }
        public JsonResult CommecialProjectReceiptAdd(long Id, string PaymentType, string InstNo, DateTime? InstDate, string Bank, string Branch, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var recData = db.Sp_Get_Commercial_Receipt_Request(Id).FirstOrDefault();
            var roomData = db.Sp_Get_Commercial_Unit(recData.Module_Id).FirstOrDefault();

            var exp = DateTime.Now;
            if (recData.Amount < 50000)
            {
                exp = exp.AddDays(3);
            }
            else if (recData.Amount < 100000)
            {
                exp = exp.AddDays(7);
            }
            else
            {
                exp = exp.AddDays(10);
            }
            SmsService smsService = new SmsService();

            db.Sp_Add_CommercialComments(Convert.ToInt32(recData.Module_Id), roomData.ApplicationNo + "Token", userid, ActivityType.Record_Upatation.ToString());

            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
            var res = db.Sp_Add_Receipt("", recData.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(recData.Amount)), Bank, InstNo, InstDate, Branch, recData.Contact
                                                          , recData.Father_Name, recData.Module_Id, recData.Name, PaymentType, 0,
                                                          recData.Project, 0, null, "", ReceiptTypes.BookingToken.ToString(), TransactionId, userid, "Commercial Token", exp
                                                          , recData.Module, null, roomData.ApplicationNo, roomData.FloorName, null, recData.Lead_Id, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
            //Double Entry
            db.Sp_Update_Status_Commercial_Unit(recData.Id, MonthlyInstallmentStatus.Paid.ToString(), recData.Module_Id, PlotsStatus.Token.ToString(), recData.Lead_Id, NewLeadsStatus.Token.ToString());
            if (PaymentType != "Cash")
            {
                var res2 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(recData.Amount, Bank, Branch, PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                    Modules.CommercialManagement.ToString(), Types.Reciept.ToString(), userid, InstNo, recData.Module_Id, InstDate, roomData.ApplicationNo, res.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                var text = "Dear " + recData.Name + ",\n\r" +
                          "A Payment of Rs " + string.Format("{0:n}", recData.Amount) + " has been received against " + PaymentType + " No: " + InstNo + " Bank: " + Bank + " for " + roomData.ApplicationNo + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";
                try
                {
                    smsService.SendMsg(text, recData.Contact);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                var text = "Dear " + recData.Name + ",\n\r" +
                           "A Payment of Rs " + string.Format("{0:n}", recData.Amount) + " has been received in cash for  " + roomData.ApplicationNo + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                try
                {
                    smsService.SendMsg(text, recData.Contact);
                }
                catch (Exception)
                {
                }
            }
            var data1 = new { Status = "1", Receiptid = res.Receipt_No, Token = TransactionId };
            return Json(data1);
        }
        [NoDirectAccess] public ActionResult AddNewInventory()
        {
            return PartialView();
        }
        public JsonResult UploadInventory(List<PremiumHomesData> AllData, string PrjName)
        {
            //var data = new XElement("Units", AllData.Select(x => new XElement("UnitItem",
            //        new XAttribute("Type", x.Type),
            //        new XAttribute("Area", x.Area),
            //        new XAttribute("Location", "-"),
            //        new XAttribute("Floor_Id", x.Floor),
            //        new XAttribute("Status", "Available_For_Sale"),
            //        new XAttribute(",miu", PrjName),
            //        new XAttribute("ApplicationNo", x.ApplicationNo),
            //        new XAttribute("Rate", x.Rate)
            //        ))).ToString();

            foreach (var v in AllData)
            {
                db.Sp_Add_Commercial_Inventory(v.ApplicationNo, v.Type, v.Area, v.Location, v.Floor, "Available_For_Sale", PrjName, v.Rate);
            }
            return Json(true);
        }
        public JsonResult GetUnitData(long Id)
        {
            var res = db.Sp_Get_CommercialData(Id).FirstOrDefault();
            return Json(res);
        }
        [NoDirectAccess] public ActionResult PrintPremiumHomesForms()
        {
            var project = "SA Premium Homes";
            var res = db.Sp_Get_Commercial_Project_Inventory(project).ToList();
            Helpers h = new Helpers(Modules.CommercialManagement, Types.Apartment);
            foreach (var v in res)
            {
                object[] data = new object[3];
                data[0] = v.ApplicationNo;
                data[1] = project;
                var QR_Data = h.SAPremiumGenerateQRCode(data);
            }
            return View(res);
        }
        //
        [NoDirectAccess] public ActionResult SAGardenUnassignedLeads(DateTime? From, DateTime? To)
        {
            var project = "Grand City";
            ViewBag.Project = project;
            if (From == null || To == null)
            {
                var fy = GetFinancialYear();
                From = fy.Start;
                To = fy.End;
            }
            var res = db.Sp_Get_Project_Unassigned_Leads(project, From, To).ToList();
            return View(res);
        }
        [NoDirectAccess] public ActionResult SAGardenUploadLeads(string Project, string status)
        {
            ViewBag.Status = status;
            ViewBag.Project = Project;
            return PartialView();
        }
        [NoDirectAccess] public ActionResult SAGardenAssignNewLeads()
        {
            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Active == 1).ToList();
            var project = "Grand City";
            ViewBag.Project = project;
            var fy = GetFinancialYear();
            ViewBag.TotalLeads = db.Leads.Where(x => x.Project == project && x.AssignedTo == 0).Count();
            return PartialView(All);
        }
        public JsonResult SAGardenAssignLeadsToSales(List<NewLeadModel> SalesLeads)
        {
            if (SalesLeads.Any())
            {
                var project = "Grand City";
                foreach (var v in SalesLeads)
                {
                    db.Sp_Update_Leads_Assign(project, v.SalesPersonId, v.LeadsCount);
                }
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult LeadsFollowup()
        {
            long userid = User.Identity.GetUserId<long>();
            var All = db.Users.Where(x => x.Name != null).Select(x => new { x.Id, x.Name }).ToList();
            ViewBag.Users = new SelectList(All, "id", "Name");

            db.Sp_Add_Activity(userid, "Accessed Follow up List Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        //[NoDirectAccess] public ActionResult LeadsFollowup_Search(DateTime? From, DateTime? To, long? User)
        //{
        //    if (From == null || To == null)
        //    {
        //        From = DateTime.Now;
        //        To = DateTime.Now;
        //    }
        //    var res1 = db.Sp_Get_Lead_Followup_Search(From, To, User).ToList();
        //    return PartialView(res1);
        //}
    }
}