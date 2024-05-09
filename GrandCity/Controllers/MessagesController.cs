using MeherEstateDevelopers.Filters;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using System.IO;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class MessagesController : Controller
    {
        // GET: Messages
        private Grand_CityEntities db = new Grand_CityEntities();
        [NoDirectAccess] public ActionResult ShortMessages()
        {
            int userid = int.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Messages_Short(userid).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult PlotsComments(long Plotid)
        {
            ViewBag.PlotId = Plotid;
            var res = db.Sp_Get_PlotComments(Plotid).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult CommercialComments(long Commercial_id)
        {
            ViewBag.PlotId = Commercial_id;
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_CommercialComments(Commercial_id).ToList();
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult PostPlotComment(long Plot_id, string Comment)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            int index = 0;
            List<Plots_Comments> fd = new List<Plots_Comments>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/PlotsData/" + Plot_id + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/PlotsData/" + Plot_id + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/PlotsData/" + Plot_id + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                db.Sp_Add_PlotComments(Plot_id, hpf.FileName, userid, "file");
                Plots_Comments f = new Plots_Comments()
                {
                    Comment = Comment,
                    Plot_Id = Plot_id,
                    Msg_Type = "file"
                };
                fd.Add(f);
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                db.Sp_Add_PlotComments(Plot_id, Comment, userid, "Text");
                Plots_Comments f = new Plots_Comments()
                {
                    Comment = Comment,
                    Plot_Id = Plot_id,
                    Msg_Type = "Text"
                };
                fd.Add(f);
            }
            return Json(fd);
        }
        [HttpPost]
        public JsonResult PostCommercialComment(long Com_id, string Comment)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            int index = 0;
            List<Plots_Comments> fd = new List<Plots_Comments>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/BuildingData/" + Com_id + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/BuildingData/" + Com_id + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/BuildingData/" + Com_id + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                db.Sp_Add_CommercialComments(Com_id, hpf.FileName, userid, "file");
                Plots_Comments f = new Plots_Comments()
                {
                    Comment = Comment,
                    Plot_Id = Com_id,
                    Msg_Type = "file"
                };
                fd.Add(f);
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                db.Sp_Add_CommercialComments(Com_id, Comment, userid, "Text");
                Plots_Comments f = new Plots_Comments()
                {
                    Comment = Comment,
                    Plot_Id = Com_id,
                    Msg_Type = "Text"
                };
                fd.Add(f);
            }
            return Json(fd);
        }
        [NoDirectAccess] public ActionResult FilesComments(long Fileid)
        {
            ViewBag.Fileid = Fileid;
            var res = db.Sp_Get_FileComments(Fileid).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult LeadComments(long LeadId)
        {
            ViewBag.LeadId = LeadId;
            var res = db.Sp_Get_LeadFollowups(LeadId).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult DealComments(long DealId)
        {
            ViewBag.DealId = DealId;
            var res = db.Sp_Get_PropertyDealFollowups(DealId).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult TicketComments(long TicketId)
        {
            ViewBag.TicketId = TicketId;
            var res = db.Sp_Get_Ticket_Comments(TicketId).ToList();
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult PostFileComment(long Plot_id, string Comment)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            int index = 0;
            List<Files_Comments> fd = new List<Files_Comments>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/FilesData/" + Plot_id + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/FilesData/" + Plot_id + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/FilesData/" + Plot_id + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                db.Sp_Add_FileComments(Plot_id, hpf.FileName, userid, "file");
                Files_Comments f = new Files_Comments()
                {
                    Comment = Comment,
                    File_Id = Plot_id,
                    Msg_Type = "file"
                };
                fd.Add(f);
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                db.Sp_Add_FileComments(Plot_id, Comment, userid, "Text");
                Files_Comments f = new Files_Comments()
                {
                    Comment = Comment,
                    File_Id = Plot_id,
                    Msg_Type = "Text"
                };
                fd.Add(f);
            }
            return Json(fd);
        }
        [HttpPost]
        public JsonResult LeadComment(long Lead_id, string Comment)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            int index = 0;
            List<Lead_Followups> fd = new List<Lead_Followups>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/LeadData/" + Lead_id + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/LeadData/" + Lead_id + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/LeadData/" + Lead_id + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                var res3 = db.Sp_Add_LeadFollowup(Comment, Lead_id, userid, "file");
                Lead_Followups f = new Lead_Followups()
                {
                    Description = Comment,
                    Lead_Id = Lead_id,
                    Type = "Img"
                };
                fd.Add(f);
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                var res3 = db.Sp_Add_LeadFollowup(Comment, Lead_id, userid, "Text");
                Lead_Followups f = new Lead_Followups()
                {
                    Description = Comment,
                    Lead_Id = Lead_id,
                    Type = "Text"
                };
                fd.Add(f);
            }
            return Json(fd);
        }
        [HttpPost]
        public JsonResult DealComment(long Deal_id, string Comment)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            int index = 0;
            List<PropertyDeal_Followups> fd = new List<PropertyDeal_Followups>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/DealData/" + Deal_id + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/DealData/" + Deal_id + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/DealData/" + Deal_id + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                var res3 = db.Sp_Add_DealFollowup(Comment, Deal_id, userid, "file");
                PropertyDeal_Followups f = new PropertyDeal_Followups()
                {
                    Description = Comment,
                    Deal_Id = Deal_id,
                    Type = "Img"
                };
                fd.Add(f);
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                var res3 = db.Sp_Add_DealFollowup(Comment, Deal_id, userid, "Text");
                PropertyDeal_Followups f = new PropertyDeal_Followups()
                {
                    Description = Comment,
                    Deal_Id = Deal_id,
                    Type = "Text"
                };
                fd.Add(f);
            }
            return Json(fd);
        }
        [HttpPost]
        public JsonResult TicketComment(long Ticket_Id, string Comment)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_Ticket_Parameter(Ticket_Id).SingleOrDefault();
            int index = 0;
            List<Ticket_Comments> fd = new List<Ticket_Comments>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/Tickets/" + Ticket_Id + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/Tickets/" + Ticket_Id + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/Tickets/" + Ticket_Id + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                var res3 = db.Sp_Add_Ticket_Comments(hpf.FileName, Ticket_Id, userid, "file");
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                var res3 = db.Sp_Add_Ticket_Comments(Comment, Ticket_Id, userid, "Text");
                Ticket_Comments f = new Ticket_Comments()
                {
                    Comment = Comment,
                    Ticket_Id = Ticket_Id,
                    Type = "Text"
                };
                fd.Add(f);
            }
            var res = db.Sp_Get_TicketAttendee(Ticket_Id).ToList();
            foreach (var item in res)
            {
                Notifier.Notify(userid, item.UserId, NotifierMsg.TicketComment, new object[] { res1 }, NotifierMsg.Ticket.ToString());
            }
            return Json(fd);
        }

        //public void Plots()
        //{
        //    List<long> plt = new List<long>();
        //    string[] arslplt_res = { "3", "22", "23", "24", "34", "59", "62", "64", "70", "81", "91", "94", "96", "135", "146", "159", "162", "163", "168", "170", "178", "213", "222", "236", "260", "303", "309", "310", "312", "315", "318", "319", "331", "334", "343", "352", "367", "391", "398", "412", "414", "417", "422", "447", "453", "460", "461", "466" };
        //    var res1 = db.Plots.Where(x => arslplt_res.Contains(x.Plot_Number) && x.Block_Id == 2 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res1);
        //    string[] arslplt_com = { "10", "31", "32", "33", "38", "51", "55" };
        //    var res2 = db.Plots.Where(x => arslplt_com.Contains(x.Plot_Number) && x.Block_Id == 2 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    plt.AddRange(res2);

        //    string[] char_res = { "82", "86", "91", "92", "99", "101", "102", "104", "106", "109", "110", "114", "128", "131", "133", "136", "143", "164", "169", "184", "185", "191", "192", "193", "194", "256", "259", "263", "268", "269", "11", "16", "17", "20", "34", "35", "42", "44", "47", "57", "71", "72", "79", "124", "127", "150", "246", "247", "54", "55" };
        //    var res3 = db.Plots.Where(x => char_res.Contains(x.Plot_Number) && x.Block_Id == 12 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res3);
        //    string[] char_com = { "1", "2", "19", "33", "34", "35", "39", "43", "48", "49", "56", "57", "59", "67", "68", "73", "75", "76", "79", "80", "46", "20", "61" };
        //    var res4 = db.Plots.Where(x => char_com.Contains(x.Plot_Number) && x.Block_Id == 12 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    plt.AddRange(res4);

        //    string[] fai_res = { "45", "49", "60", "74", "85", "113", "116", "120", "125", "137", "143", "173", "198", "203", "205", "207", "212", "223", "225", "226", "230", "253", "282", "287", "2", "5", "7", "13", "17", "25", "59", "71", "97", "10", "209", "276", "278" };
        //    var res5 = db.Plots.Where(x => fai_res.Contains(x.Plot_Number) && x.Block_Id == 15 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res5);

        //    string[] faris_res = { "10", "33", "35", "45", "59", "84", "104", "116", "118", "136", "146", "153", "166", "177", "192", "202", "226", "249", "3", "12", "16", "96" };
        //    var res6 = db.Plots.Where(x => faris_res.Contains(x.Plot_Number) && x.Block_Id == 5 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res6);

        //    string[] fur_res = { "5", "38", "40", "65", "69", "88", "94", "95", "110", "113", "114", "137", "159", "170", "172", "177", "209" };
        //    var res7 = db.Plots.Where(x => fur_res.Contains(x.Plot_Number) && x.Block_Id == 6 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res7);
        //    string[] fur_com = { "33", "37", "44" };
        //    var res8 = db.Plots.Where(x => fur_com.Contains(x.Plot_Number) && x.Block_Id == 6 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    plt.AddRange(res8);

        //    string[] kam_res = { "3", "7", "41", "53", "73", "76", "86", "102", "121", "158", "181", "189", "198", "211", "220", "221", "224", "228", "229", "233", "259", "263", "300", "309", "313", "323", "326", "350", "352", "354", "357" };
        //    var res9 = db.Plots.Where(x => kam_res.Contains(x.Plot_Number) && x.Block_Id == 3 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res9);
        //    string[] kam_com = { "39", "42", "62", "63", "64", "67", "68", "70", "71", "73", "75", "76", "78", "90", "93", "94", "102", "107", "112", "113" };
        //    var res10 = db.Plots.Where(x => kam_com.Contains(x.Plot_Number) && x.Block_Id == 3 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    plt.AddRange(res10);

        //    string[] mgt_com = { "16" };
        //    var res11 = db.Plots.Where(x => mgt_com.Contains(x.Plot_Number) && x.Block_Id == 14 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    plt.AddRange(res11);

        //    string[] sherafz_res = { "6", "25", "43", "50", "62", "120", "192", "217", "226", "230" };
        //    var res12 = db.Plots.Where(x => sherafz_res.Contains(x.Plot_Number) && x.Block_Id == 7 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res12);
        //    string[] sherafz_com = { "25" };
        //    var res13 = db.Plots.Where(x => sherafz_com.Contains(x.Plot_Number) && x.Block_Id == 7 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    plt.AddRange(res13);

        //    string[] sheraz_res = { "52", "62", "70", "86", "87", "91", "101", "122", "130", "131", "167", "168", "169", "175", "177", "178", "188", "208", "216", "258", "264", "274", "276", "277" };
        //    var res14 = db.Plots.Where(x => sheraz_res.Contains(x.Plot_Number) && x.Block_Id == 8 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res14);

        //    string[] sherzamn_res = { "4", "5", "8", "9", "10", "14", "15", "17", "28", "30", "31", "33", "34", "35", "37", "42", "44", "50", "55", "60", "62", "68", "69", "75", "85", "108", "110", "114", "126", "127", "128", "129", "130", "147", "149", "157", "160", "167", "168", "178", "181", "184", "187", "191", "195", "198", "199", "201", "203", "222", "235", "239", "243", "246", "253", "255", "256", "257", "261", "263", "267", "268", "281", "283", "284", "287", "291", "300", "305", "321", "322", "323", "330", "335", "336", "338", "345", "346", "349", "354", "355", "356", "360", "367", "368", "373", "378", "379", "382", "391", "414", "415" };
        //    var res15 = db.Plots.Where(x => sherzamn_res.Contains(x.Plot_Number) && x.Block_Id == 11 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res15);

        //    string[] shoib_res = { "47", "56", "84", "94", "117", "120", "126", "132", "136", "140", "143", "152", "170", "174", "185", "194", "195", "197", "209", "226", "243", "246", "269", "271", "272", "273", "275", "286", "287", "290", "297", "306", "307", "315", "317", "346", "351", "354", "389", "390", "394", "400", "401", "403", "413", "414", "5", "6", "10", "19", "24", "25", "33", "32", "123", "234", "237", "310", "325", "332", "335", "338", "350", "386", "107", "109", "160", "220", "222", "270", "292", "35", "37", "372", "373", "392", "406", "46" };
        //    var res16 = db.Plots.Where(x => shoib_res.Contains(x.Plot_Number) && x.Block_Id == 4 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res16);
        //    string[] shoib_com = { "6", "9", "20", "21", "28", "36", "42" };
        //    var res17 = db.Plots.Where(x => shoib_com.Contains(x.Plot_Number) && x.Block_Id == 4 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    plt.AddRange(res17);

        //    string[] soh_res = { "31", "55", "56", "75", "76", "78", "87", "101", "114", "118", "120", "128", "161", "162", "169", "170", "172", "174" };
        //    var res18 = db.Plots.Where(x => soh_res.Contains(x.Plot_Number) && x.Block_Id == 9 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res18);

        //    string[] badr_secA = {"9","11","13","17","22","39","45","53","66","72","73","79","82","83","84","87","90","92","99","100","109","112","114","116","117","137","149","152","182","188","193","202","256","267","286","288","319","324","335","340","346","350"};
        //    var res19 = db.Plots.Where(x => badr_secA.Contains(x.Plot_Number) && x.Block_Id == 10 && x.Sector == "A" && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res19);

        //    string[] badr_secC = { "1", "2", "3", "4", "35", "52", "70", "87", "105", "135", "137", "169", "171", "178", "185", "196", "197", "199", "201", "204", "215", "216", "217", "218", "219", "265", "277", "278", "279", "284", "364", "377", "412", "415", "418", "435", "450", "452", "455", "458", "483", "487", "491", "496", "502", "503", "504", "505", "506", "511", "517", "519", "555", "582", "593", "594", "596", "597", "602", "612", "616" };
        //    var res20 = db.Plots.Where(x => badr_secC.Contains(x.Plot_Number) && x.Block_Id == 10 && x.Sector == "C" && x.Type == "Residential").Select(x => x.Id).ToList();
        //    plt.AddRange(res20);

        //    foreach (var item in plt)
        //    {
        //        PlotComment(item);
        //    }

        //}

        //public void PlotComment(long Plot_id)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());

        //    string[] files = { "11.jpg", "12.jpg", "13.jpg", "14.jpg", "15.jpg", "16.jpg", "17.jpg" };

        //    foreach (var item in files)
        //    {
        //        string fileName = item;
        //        string sourcePath = Server.MapPath("~/images/");
        //        if (!Directory.Exists(Server.MapPath("~/PlotsData/" + Plot_id + "")))
        //        {
        //            Directory.CreateDirectory(Server.MapPath("~/PlotsData/" + Plot_id + ""));
        //        }
        //        string targetPath = Server.MapPath("~/PlotsData/" + Plot_id + "");
        //        string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
        //        string destFile = System.IO.Path.Combine(targetPath, fileName);

        //        System.IO.File.Copy(sourceFile, destFile, true);

        //        var com = db.Sp_Add_PlotComments(Plot_id, fileName, 20073, "file");
        //        var up = db.Sp_Update_VerifyStatus(Plot_id, Modules.PlotManagement.ToString());
        //        var comt = db.Sp_Add_PlotComments(Plot_id, "Plot is Verified by Ammad Bin Shabbir (Systematically). As per the attachment", 20073, "Text");

        //    }
        //}

        [NoDirectAccess] public ActionResult DealershipComments(long DealershipId)
        {
            ViewBag.DealershipId = DealershipId;
            var res = db.Sp_Get_DealershipComments(DealershipId).ToList();
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult PostDealershipComment(long DealershipId, string Comment)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            int index = 0;
            List<Dealership_Comments> fd = new List<Dealership_Comments>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/DealershipData/" + DealershipId + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/DealershipData/" + DealershipId + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/DealershipData/" + DealershipId + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                db.Sp_Add_DealershipComments(DealershipId, hpf.FileName, userid, "file");
                Dealership_Comments f = new Dealership_Comments()
                {
                    Comment = Comment,
                    Dealership_Id = DealershipId,
                    Msg_Type = "file"
                };
                fd.Add(f);
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                db.Sp_Add_DealershipComments(DealershipId, Comment, userid, "Text");
                Dealership_Comments f = new Dealership_Comments()
                {
                    Comment = Comment,
                    Dealership_Id = DealershipId,
                    Msg_Type = "Text"
                };
                fd.Add(f);
            }
            return Json(fd);
        }
    }
}