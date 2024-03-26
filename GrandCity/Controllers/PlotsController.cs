using MeherEstateDevelopers.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Threading;
using UnityEngine.SocialPlatforms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class PlotsController : Controller
    {
        string AccountingModuleFP = COA_Mapper_Modules.Files_Plots.ToString();
        // GET: Plots
        private Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult CreatePlotInventory()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Create Plots Invntory Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects(), "Id", "Project_Name");

            //var re = db.Receipts.Where(x => x.PaymentType == "Debit/Credit Card" && x.Module == "PlotManagement" && x.File_Plot_Number == null).ToList();
            //foreach (var item in re)
            //{
            //    var p = db.Plots.Where(x => x.Id == item.File_Plot_No).FirstOrDefault();
            //    if (p != null) 
            //    {
            //        if (p.Application_FileNo == null)
            //        {
            //             var a = p.Plot_Number + p.Sector ;
            //            db.Sp_Update_receiptsfileno(item.Id, a, "1", null, null, null);
            //        }
            //        else 
            //        {
            //            var b =  p.Plot_Number + " " + p.Sector + " " + " ( " + p.Application_FileNo + " ) ";
            //            db.Sp_Update_receiptsfileno(item.Id, b, "1", null, null, null);
            //        }
            //    }

            //}


            //var re1 = db.Receipts.Where(x => x.PaymentType == "Debit/Credit Card" && x.Module == "fileManagement" && x.File_Plot_Number == null).ToList();
            //foreach (var item in re1)
            //{
            //    var p = db.File_Form.Where(x => x.Id == item.File_Plot_No).FirstOrDefault();
            //    if (p != null)
            //    {
            //        db.Sp_Update_receiptsfileno(item.Id, p.FileFormNumber, "1", null, null, null);

            //    }   
            //}

            //var re2 = db.Receipts.Where(x => x.PaymentType == "Debit/Credit Card" && x.Module == "fileManagement" && x.Name == null).ToList();
            //foreach (var item in re2)
            //{
            //    var file = db.Sp_Get_FileLastOwner(item.File_Plot_No).FirstOrDefault();
            //    if (file != null)
            //    {
            //        db.Sp_Update_receiptsfileno(item.Id, null, "2", file.Name, file.Father_Husband, file.Mobile_1);
            //    }
            //}

            //var re3 = db.Receipts.Where(x => x.PaymentType == "Debit/Credit Card" && x.Module == "plotManagement" && x.Name == null).ToList();
            //foreach (var item in re3)
            //{
            //    var plot = db.Sp_Get_PlotLastOwner(item.File_Plot_No).SingleOrDefault();
            //    if (plot != null)
            //    {
            //        var a=db.Sp_Update_receiptsfileno(item.Id, null, "2", plot.Name, plot.Father_Husband, plot.Mobile_1);
            //    }
            //}


            return PartialView();
        }
        public JsonResult CreatePlotsInventory(List<BallotPlotFileInventory> plotsData, long Block)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Created Plots Inventory", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Block);
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                foreach (var v in plotsData)
                {
                    decimal front = 0;
                    decimal back = 0;
                    decimal left = 0;
                    decimal right = 0;
                    decimal north_east = 0;
                    decimal north_west = 0;
                    decimal south_east = 0;
                    decimal south_west = 0;
                    var blk = db.RealEstate_Blocks.Where(x => x.Id == Block).FirstOrDefault();
                    var phase = db.RealEstate_Phases.Where(x => x.Id == blk.Phase_Id).FirstOrDefault();
                    if (v.DIMENSION.Contains('F'))
                    {
                        string[] _temp = v.DIMENSION.Split('-');
                        foreach (string s in _temp)
                        {
                            if (s.Contains('F'))
                            {
                                front = Convert.ToDecimal(s.Split(' ')[0]);
                            }
                            else if (s.Contains('B'))
                            {
                                back = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                            else if (s.Contains('L'))
                            {
                                left = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                            else if (s.Contains('R'))
                            {
                                right = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                            else if (s.Contains("R1"))
                            {
                                left = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                            else if (s.Contains("R2"))
                            {
                                right = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                            else if (s.Contains("R3"))
                            {
                                north_east = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                            else if (s.Contains("L1"))
                            {
                                north_west = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                            else if (s.Contains("L2"))
                            {
                                south_east = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                            else if (s.Contains("L3"))
                            {
                                south_west = Convert.ToDecimal(s.Split(' ')[1]);
                            }
                        }
                    }
                    else if (v.DIMENSION.Contains('x'))
                    {
                        string[] _temp = v.DIMENSION.Split('x');
                        front = back = Convert.ToDecimal(_temp[0].Split(' ')[0]);
                        left = right = Convert.ToDecimal(_temp[1].Split(' ')[1]);
                    }
                    var res = db.Plots_Category.FirstOrDefault(x => x.Front_Side == front &&
                    x.Back_Side == back &&
                    x.Left_Side == left &&
                    x.Right_Side == right &&
                    x.North_East == north_east &&
                    x.North_West == north_west &&
                    x.South_East == south_east &&
                    x.South_West == south_west); //.FirstOrDefault();

                    if (res is null)
                    {
                        try
                        {
                            Plots_Category pc = new Plots_Category
                            {
                                Front_Side = front,
                                Back_Side = back,
                                Left_Side = left,
                                Right_Side = right,
                                North_East = north_east,
                                North_West = north_west,
                                South_East = south_east,
                                South_West = south_west,
                                Category = v.SIZE + " Marla",
                                Uom = "sqft"
                            };

                            db.Plots_Category.Add(pc);
                            db.SaveChanges();
                            //bool isDuplicate = db.Plots_Category.Any(x => x.Equals(pc));

                            //if (!isDuplicate)
                            //{
                            //    db.Plots_Category.Add(pc);
                            //    db.SaveChanges();
                            //}
                            //db.Plots_Category.Add(pc);
                            //db.SaveChanges();
                            if (v.TYPE == "C" || v.TYPE == "c")
                            {
                                v.TYPE = PlotType.Commercial.ToString();
                            }
                            else if (v.TYPE == "R" || v.TYPE == "r")
                            {
                                v.TYPE = PlotType.Residential.ToString();
                            }
                            else
                            {
                                return Json(false);
                            }
                            Plot p = new Plot
                            {
                                Block_Id = blk.Id,
                                Phase_Id = phase.Id,
                                Plot_Category = 193,
                                Hold = true,
                                Plot_Location = v.LOCATION,
                                Plot_Number = v.PLOT,
                                Sector = v.SECTOR,
                                Plot_Size = v.SIZE + " Marla",
                                Area = v.AREA,
                                Road_Type = v.ROAD,
                                Block_Name = blk.Block_Name,
                                Develop_Status = "Non Constructed",
                                Verified = false,
                                Status = v.STATUS,
                                Type = v.TYPE,
                            };
                            db.Plots.Add(p);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            string ssssss = ex.Message;
                        }
                    }
                    else
                    {
                        if (v.TYPE == "C" || v.TYPE == "c")
                        {
                            v.TYPE = PlotType.Commercial.ToString();
                        }
                        else if (v.TYPE == "R" || v.TYPE == "r")
                        {
                            v.TYPE = PlotType.Residential.ToString();
                        }
                        else
                        {
                            return Json(false);
                        }
                        Plot p = new Plot
                        {
                            Block_Id = blk.Id,
                            Phase_Id = phase.Id,
                            Plot_Category = res.Id,
                            Hold = true,
                            Plot_Location = v.LOCATION,
                            Plot_Number = v.PLOT,
                            Sector = v.SECTOR,
                            Plot_Size = v.SIZE + " Marla",
                            Area = v.AREA,
                            Road_Type = v.ROAD,
                            Block_Name = blk.Block_Name,
                            Develop_Status = "Non Constructed",
                            Verified = false,
                            Status = v.STATUS,
                            Type = v.TYPE
                        };
                        db.Plots.Add(p);
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                var a = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, "Plots", "SavePlotOwnerData");
                return Json(false);
            }
        }

        //public JsonResult CreatePlotsInventory(List<BallotPlotFileInventory> plotsData, long Block)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    db.Sp_Add_Activity(userid, "Created Plots Inventory", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Block);
        //    try
        //    {
        //        var uid = User.Identity.GetUserId<long>();
        //        var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
        //        foreach (var v in plotsData)
        //        {
        //            decimal front = 0;
        //            decimal back = 0;
        //            decimal left = 0;
        //            decimal right = 0;
        //            var blk = db.RealEstate_Blocks.Where(x => x.Id == Block).FirstOrDefault();
        //            var phase = db.RealEstate_Phases.Where(x => x.Id == blk.Phase_Id).FirstOrDefault();
        //            if (v.DIMENSION.Contains('F'))
        //            {
        //                string[] _temp = v.DIMENSION.Split('-');
        //                foreach (string s in _temp)
        //                {
        //                    if (s.Contains('F'))
        //                    {
        //                        front = Convert.ToDecimal(s.Split(' ')[0]);
        //                    }
        //                    else if (s.Contains('B'))
        //                    {
        //                        back = Convert.ToDecimal(s.Split(' ')[1]);
        //                    }
        //                    else if (s.Contains('L'))
        //                    {
        //                        left = Convert.ToDecimal(s.Split(' ')[1]);
        //                    }
        //                    else if (s.Contains('R'))
        //                    {
        //                        right = Convert.ToDecimal(s.Split(' ')[1]);
        //                    }
        //                }
        //            }
        //            else if (v.DIMENSION.Contains('x'))
        //            {
        //                string[] _temp = v.DIMENSION.Split('x');
        //                front = back = Convert.ToDecimal(_temp[0].Split(' ')[0]);
        //                left = right = Convert.ToDecimal(_temp[1].Split(' ')[1]);
        //            }
        //            var res = db.Plots_Category.Where(x => x.Front_Side == front && x.Back_Side == back && x.Left_Side == left && x.Right_Side == right).FirstOrDefault();
        //            if (res is null)
        //            {
        //                try
        //                {
        //                    //Plots_Category pc = new Plots_Category
        //                    //{
        //                    //    Front_Side = front,
        //                    //    Back_Side = back,
        //                    //    Left_Side = left,
        //                    //    Right_Side = right,
        //                    //    Category = v.SIZE + " Marla",
        //                    //    Uom = "sqft"
        //                    //};
        //                    //db.Plots_Category.Add(pc);
        //                    //db.SaveChanges();
        //                    if (v.TYPE == "C" || v.TYPE == "c")
        //                    {
        //                        v.TYPE = PlotType.Commercial.ToString();
        //                    }
        //                    else if (v.TYPE == "R" || v.TYPE == "r")
        //                    {
        //                        v.TYPE = PlotType.Residential.ToString();
        //                    }
        //                    else
        //                    {
        //                        return Json(false);
        //                    }
        //                    Plot p = new Plot
        //                    {
        //                        Block_Id = blk.Id,
        //                        Phase_Id = phase.Id,
        //                        Plot_Category = 193,
        //                        Hold = true,
        //                        Plot_Location = v.LOCATION,
        //                        Plot_Number = v.PLOT,
        //                        Sector = v.SECTOR,
        //                        Plot_Size = v.SIZE + " Marla",
        //                        Area = v.AREA,
        //                        Road_Type = v.ROAD,
        //                        Block_Name = blk.Block_Name,
        //                        Develop_Status = "Non Constructed",
        //                        Verified = false,
        //                        Status = v.STATUS,
        //                        Type = v.TYPE,
        //                    };
        //                    db.Plots.Add(p);
        //                    db.SaveChanges();
        //                }
        //                catch (Exception ex)
        //                {
        //                    string ssssss = ex.Message;
        //                }
        //            }
        //            else
        //            {
        //                if (v.TYPE == "C" || v.TYPE == "c")
        //                {
        //                    v.TYPE = PlotType.Commercial.ToString();
        //                }
        //                else if (v.TYPE == "R" || v.TYPE == "r")
        //                {
        //                    v.TYPE = PlotType.Residential.ToString();
        //                }
        //                else
        //                {
        //                    return Json(false);
        //                }
        //                Plot p = new Plot
        //                {
        //                    Block_Id = blk.Id,
        //                    Phase_Id = phase.Id,
        //                    Plot_Category = 193,
        //                    Hold = true,
        //                    Plot_Location = v.LOCATION,
        //                    Plot_Number = v.PLOT,
        //                    Sector = v.SECTOR,
        //                    Plot_Size = v.SIZE + " Marla",
        //                    Area = v.AREA,
        //                    Road_Type = v.ROAD,
        //                    Block_Name = blk.Block_Name,
        //                    Develop_Status = "Non Constructed",
        //                    Verified = false,
        //                    Status = v.STATUS,
        //                    Type = v.TYPE
        //                };
        //                db.Plots.Add(p);
        //                db.SaveChanges();
        //            }
        //            db.SaveChanges();
        //        }
        //        return Json(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        var a = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, "Plots", "SavePlotOwnerData");
        //        return Json(false);
        //    }
        //}
        public JsonResult GetPlots_ForSelect(string s, string Plot_Size)
        {
            if (string.IsNullOrEmpty(s))
            {
                return Json(new { text = "Select Plot", id = 0 }, JsonRequestBehavior.AllowGet);
            }
            string[] parsed = s.Split(' ');
            var exstngPlts = db.Biding_Reserve_Plots.Where(x => x.GroupTag != null).Select(x => x.Plot_Id).ToList();
            if (parsed.Length > 1)
            {
                string gplt = parsed[0];
                string blk = parsed[1];
                var res = db.Plots.Where(x => x.Plot_Size == Plot_Size && x.Plot_Number.Contains(gplt) && x.Block_Name.Contains(blk) && x.Status == "Available_For_Sale" && !exstngPlts.Contains(x.Id)).Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + "-" + x.Block_Name + " ( " + x.Plot_Location + " )", id = x.Id }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = db.Plots.Where(x => x.Plot_Size == Plot_Size && x.Plot_Number.Contains(s) && x.Status == "Available_For_Sale" && !exstngPlts.Contains(x.Id)).Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + " - " + x.Block_Name + " ( " + x.Plot_Location + " )", id = x.Id }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RegisterPlot()
        {
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Block = new SelectList(db.Sp_Get_Block().ToList(), "Id", "Block_Name");
            ViewBag.Dealers = new SelectList(db.Dealerships.ToList(), "Id", "Dealership_Name");
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Register Plot Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }

        //[HttpPost]
        //public ActionResult UploadFiles(FormCollection fc)
        //{
        //    // Checking no of files injected in Request object  
        //    if (Request.Files.Count > 0)
        //    {
        //        try
        //        {
        //            var provider = fc.ToValueProvider();
        //            var dt = "";
        //            if (provider.ContainsPrefix("pltid"))
        //            {
        //                dt = provider.GetValue("pltid").AttemptedValue;
        //                // using the `dt`
        //            }
        //            //  Get all files from Request object  
        //            HttpFileCollectionBase files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                HttpPostedFileBase file = files[i];
        //                string fname;
        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = file.FileName;
        //                }
        //                if (!Directory.Exists(Server.MapPath("~/PlotReceipts/Plots/" + dt + "/")))
        //                {
        //                    Directory.CreateDirectory(Server.MapPath("~/PlotReceipts/Plots/" + dt));
        //                }
        //                fname = Path.Combine(Server.MapPath("~/PlotReceipts/Plots/" + dt + "/"), fname);
        //                file.SaveAs(fname);
        //            }

        //            return Json("File Uploaded Successfully!");
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json("Error occurred. Error details: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return Json("No files selected.");
        //    }
        //}
        public ActionResult PlotsShortResult(long PlotId)
        {
            var res1 = db.Sp_Get_PlotDetailData(PlotId).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();
            var res = new NewConnectionCharges { PlotData = res1, OwnerData = res2, Conch = null };
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Plot Details  ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), PlotId);
            return PartialView(res);
        }
        public ActionResult RegistryView()
        {
            var res = db.Sp_Get_PlotsShortSummary().SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Plots Registery Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        public ActionResult CreatePlotsNomenclature()
        {
            return View();
        }

        public ActionResult PlotsInventory()
        {
            var res = db.Plots.Where(x => x.Status == "Available_For_Sale").ToList();
            return View(res);
        }

        [HttpPost]
        public ActionResult DeletePlotInventory(int id)
        {
            var PlotDeleted = db.Plots.Where(x => x.Id == id).FirstOrDefault();
            if(PlotDeleted != null)
            {
                Plots_Deleted pd = new Plots_Deleted
                {
                    Id = PlotDeleted.Id,
                    Plot_Number = PlotDeleted.Plot_Number,
                    Block_Id = PlotDeleted.Block_Id,
                    Phase_Id = PlotDeleted.Phase_Id,
                    Sector = PlotDeleted.Sector,
                    Plot_Size = PlotDeleted.Plot_Size,
                    Plot_Category = PlotDeleted.Plot_Category,
                    Develop_Status = PlotDeleted.Develop_Status,
                    Plot_Location = PlotDeleted.Plot_Location,
                    Type = PlotDeleted.Type,
                    Road_Type = PlotDeleted.Road_Type,
                    Status = PlotDeleted.Status,
                    Verified = PlotDeleted.Verified,
                    AllotmentReq = PlotDeleted.AllotmentReq,
                    AllotmentReq_Date = PlotDeleted.AllotmentReq_Date,
                    Old_PlotNumber = PlotDeleted.Old_PlotNumber,
                    reg = PlotDeleted.reg,
                    IsForApproval = PlotDeleted.IsForApproval,
                    Verification_Req = PlotDeleted.Verification_Req,
                    Remarks = PlotDeleted.Remarks,
                    Verification_Date = PlotDeleted.Verification_Date,
                    Registry = PlotDeleted.Registry,
                    Block_Name = PlotDeleted.Block_Name,
                    Mortgage = PlotDeleted.Mortgage,
                    Mortgage_by = PlotDeleted.Mortgage_by,
                    VerificationReqDate = PlotDeleted.VerificationReqDate,
                    VerifiedDate = PlotDeleted.VerifiedDate,
                    Hold = PlotDeleted.Hold,
                    PossessionReq = PlotDeleted.PossessionReq,
                    PossessionReq_Date = PlotDeleted.PossessionReq_Date,
                    North = PlotDeleted.North,
                    South = PlotDeleted.South,
                    East = PlotDeleted.East,
                    West = PlotDeleted.West,
                    North_East = PlotDeleted.North_East,
                    North_West = PlotDeleted.North_West,
                    South_East = PlotDeleted.South_East,
                    South_West = PlotDeleted.South_West,
                    Front = PlotDeleted.Front,
                    Phase_Name = PlotDeleted.Phase_Name,
                    Rental = PlotDeleted.Rental,
                    Application_FileNo = PlotDeleted.Application_FileNo,
                    Application_FileId = PlotDeleted.Application_FileId,
                    Development_Charges = PlotDeleted.Development_Charges,
                    Dealership_Id = PlotDeleted.Dealership_Id,
                    Dealership_Name = PlotDeleted.Dealership_Name,
                    Installment_Plan_Id = PlotDeleted.Installment_Plan_Id,
                    Area = PlotDeleted.Area,
                    Dealership_Submission_Id = PlotDeleted.Dealership_Submission_Id,
                    Dealership_Submission_Name = PlotDeleted.Dealership_Submission_Name

                };
                db.Plots_Deleted.Add(pd);
                db.SaveChanges();
            }
            
            try
            {
                var record = db.Plots.Find(id);
                if (record != null)
                {
                    db.Plots.Remove(record);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Plot deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Plot not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the Plot." + ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreatePlotsNomenclature(Plots_Category Pn)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Created Plot Nomenclature of Plot Size " + Pn.Plot_Size, "Create", Modules.PlotManagement.ToString(), ActivityType.Dimension_Create.ToString(), null);
            var res = db.Sp_Add_Plots_Size(Pn.Uom, Pn.Front_Side, Pn.Back_Side, Pn.Left_Side, Pn.Right_Side, Pn.Category, Pn.North_East, Pn.North_West, Pn.South_West, Pn.South_East).FirstOrDefault();
            return Json(res);
        }
        public JsonResult PlotListByBlock(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Plot List of Block With Id <a class='blk-data' data-id=' " + Id + "'>" + Id + "</a> ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            var re = db.Sp_Get_BlockPlotlist_Short(Id).ToList();
            var res1 = re.Where(x => x.Type == PlotType.Commercial.ToString()).ToList();
            var res2 = re.Where(x => x.Type == PlotType.Residential.ToString()).ToList();
            var res = new { Commercial = res1, Residential = res2 };
            return Json(res);
        }
        public ActionResult PlotOwnership()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships().Where(x => x.Status == "Registered").Select(x => new { x.Id, x.Dealership_Name }).Distinct(), "Id", "Dealership_Name");
            return View();
        }
        public ActionResult PlotBooking()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.Dealership = new SelectList(db.Sp_Get_Dealerships().Where(x => x.Status == "Registered").Select(x => new { x.Id, x.Dealership_Name }).Distinct(), "Id", "Dealership_Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return View();
        }
        public JsonResult RegisterDealerPlot(long? Plot_Id, List<Plot_Ownership> Owners, long TransactionId, bool? isPayment, int? DealersId, BookingReceiptData brdd)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();

            var comp = ah.Company_Attr(userid);
            Helpers h = new Helpers();
            var DealerName = db.Dealerships.Where(x => x.Id == DealersId).Select(x => x.Dealership_Name).FirstOrDefault();
            if (DealerName == null)
            {
                return Json(new { Status = false, Msg = "This Plot is not Assigned to Dealership!" });
            }
            else if(DealerName != null)
            {
                var AddDealership = db.Sp_Update_Plot_Reg_DealershipName(Plot_Id, DealersId, DealerName);

            }
            var plot = db.Sp_Get_PlotData(Plot_Id).FirstOrDefault();
            var res2 = db.Biding_Reserve_Plots.Where(x => x.Plot_Id == Plot_Id && x.PlotStatus == "Available").FirstOrDefault();
            var dealership = db.Dealerships.Where(x => x.Id == res2.Dealer_Id).FirstOrDefault();
            var dealers = db.Dealers.Where(x => x.Dealership_Id == res2.Dealer_Id).ToList();
            var installment_plan_Id = db.Plots.Where(x => x.Id == Plot_Id).FirstOrDefault().Installment_Plan_Id;
            decimal Plot_Total_Price = 0;
            decimal Rate_Per_Marla = 0;
            var Receipt_No = "";
            if(installment_plan_Id != null)
            {
                var P_T_P = db.Installment_Plot_Category.Where(x => x.Id == installment_plan_Id).FirstOrDefault();
                Plot_Total_Price = P_T_P.Grand_Total;
                Rate_Per_Marla = P_T_P.Rate;
            }

            Helpers H = new Helpers();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    long GroupTag = h.RandomNumber();
                    Owners.ForEach(x => x.GroupTag = GroupTag);
                    //Owners.ForEach(x => x.Ownership_DateTime = DateTime.Now);
                    Owners.ForEach(x => x.Ownership_Status = "Owner");
                    foreach (var PO in Owners)
                    {
                        db.Plot_Ownership.Add(PO);
                        db.SaveChanges();
                    }



                    if (plot.Installment_Plan_Id == null)
                    {
                        //List<PlotsInstallments> PI = new List<PlotsInstallments>()
                        //{
                        //    new PlotsInstallments()
                        //    {
                        //        Amount = (decimal)res2.GrandTotal,
                        //        DueDate = DateTime.Now,
                        //        InstNo = "Advance"
                        //    }
                        //};
                        //var Installments = new XElement("Plots", PI.Select(x => new XElement("Installments",
                        //  new XAttribute("Amount", x.Amount),
                        //  new XAttribute("DueDate", x.DueDate),
                        //  new XAttribute("Installment_Name", x.InstNo)
                        //  ))).ToString();
                        //db.Sp_Update_PlotInstallments(Plot_Id, res2.GrandTotal, 0, userid, Installments);
                        return Json(new { Status = false, Msg = "Attach Payment Plan to Proceed!" });
                    }
                    //0 other charges
                    //1 Installments
                    //2 Development Charges
                    //3 Advance
                    //4 Possession
                    //5 After Time (Half Yearly, Annual Quarterly etc)
                    //6 Confirmation
                    else
                    {
                        var installmentstructure = db.Sp_Get_InstallmentStructure((int)plot.Installment_Plan_Id).ToList();


                        List<Plot_Installments> plot_Installments = new List<Plot_Installments>();
                        decimal rate = 0, total = 0, grandtotal = 0;
                        var Date = DateTime.UtcNow;
                        var PlotId = plot.Id;
                        //Advance or one time Payments
                        foreach (var item in installmentstructure.Where(x => x.Installment_Type == "3" || x.Installment_Type == "6" || x.Installment_Type == "4" || x.Installment_Type == "2"))
                        {
                            if (item.Installment_Type == "3") // Advance, Booking
                               
                            {
                                rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                                DateTime b = Date;
                                Plot_Installments fi = new Plot_Installments()
                                {
                                    Owner_Id = GroupTag,
                                    Status = InstallmentPaymentStatus.Pending.ToString(),
                                    DueDate = b,
                                    Amount = item.Amount,
                                    Plot_Id = PlotId,
                                    Installment_Name = item.Installment_Name,
                                    Installment_Type = "3"
                                };
                                plot_Installments.Add(fi);
                            }
                            else if (item.Installment_Type == "6") // Confirmation
                            {
                                rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                                DateTime b = Date;
                                b = b.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                                DateTime dt = new DateTime(b.Year, b.Month, 1);
                                Plot_Installments fi = new Plot_Installments()
                                {
                                    Owner_Id = GroupTag,
                                    Status = InstallmentPaymentStatus.Pending.ToString(),
                                    DueDate = dt,
                                    Amount = item.Amount,
                                    Plot_Id = PlotId,
                                    Installment_Name = item.Installment_Name,
                                    Installment_Type = "6"
                                };
                                plot_Installments.Add(fi);
                            }

                            else if (item.Installment_Type == "4") // Possession
                            {
                                rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                                DateTime b = Date;
                                b = b.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                                DateTime dt = new DateTime(b.Year, b.Month, 1);
                                Plot_Installments fi = new Plot_Installments()
                                {
                                    Owner_Id = GroupTag,
                                    Status = InstallmentPaymentStatus.Pending.ToString(),
                                    DueDate = dt,
                                    Amount = item.Amount,
                                    Plot_Id = PlotId,
                                    Installment_Name = item.Installment_Name,
                                    Installment_Type = "4"
                                };
                                plot_Installments.Add(fi);
                            }

                            else if (item.Installment_Type == "2") // Development Charges
                            {
                                rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                                DateTime b = Date;
                                b = b.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                                DateTime dt = new DateTime(b.Year, b.Month, 1);
                                Plot_Installments fi = new Plot_Installments()
                                {
                                    Owner_Id = GroupTag,
                                    Status = InstallmentPaymentStatus.Pending.ToString(),
                                    DueDate = dt,
                                    Amount = item.Amount,
                                    Plot_Id = PlotId,
                                    Installment_Name = item.Installment_Name,
                                    Installment_Type = "2"
                                };
                                plot_Installments.Add(fi);
                            }
                        }
                        foreach (var item in installmentstructure.Where(x => x.Installment_Type == "5" ||  x.Installment_Type == "0"))
                        {

                            if (item.Installment_Type == "5") // for Half yearly , Quarterly
                            {
                                rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                                var eachinst = item.Amount / item.After;
                                var itemaft = item.Interval;



                                for (int i = 1; i <= item.After; i++)
                                {
                                    DateTime b = Date;
                                    //var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                                    //if (str != null) // to check for confirmation
                                    //{
                                    //    b = str.DueDate;
                                    //}
                                    b = b.AddMonths(Convert.ToInt16(itemaft));
                                    DateTime dt = new DateTime(b.Year, b.Month, 1);

                                    Plot_Installments fi = new Plot_Installments()
                                    {
                                        Owner_Id = GroupTag,
                                        Status = InstallmentPaymentStatus.Pending.ToString(),
                                        DueDate = dt,
                                        Amount = Convert.ToDecimal(eachinst),
                                        Plot_Id = PlotId,
                                        Installment_Name = i + " - " + item.Installment_Name,
                                        //Installment type will be one as 1 represents multiple installment types
                                        Installment_Type = "1"
                                    };
                                    plot_Installments.Add(fi);
                                    itemaft += item.Interval;
                                }
                            }
                            else if (item.Installment_Type == "0")
                            {
                                rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;

                                DateTime b = Date;
                                //var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                                //if (str != null) // to check for confirmation
                                //{
                                //    b = str.DueDate;
                                //}

                                b = b.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                                DateTime dt = new DateTime(b.Year, b.Month, 1);
                                Plot_Installments fi = new Plot_Installments()
                                {
                                    Owner_Id = GroupTag,
                                    Status = InstallmentPaymentStatus.Pending.ToString(),
                                    DueDate = dt,
                                    Amount = item.Amount,
                                    Plot_Id = PlotId,
                                    Installment_Name = item.Installment_Name,
                                    Installment_Type = "0"
                                };
                                plot_Installments.Add(fi);
                            }
                        }
                        foreach (var item in installmentstructure.Where(x => x.Installment_Type == "1")) //Installments
                        {
                            if (item.Installment_Type == "1") // for type installments
                            {
                                rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                                var eachinst = item.Amount / item.Rate;


                                for (int i = 1; i <= item.Rate; i++)
                                {
                                    DateTime b = Date;
                                    //var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                                    //if (str != null) // to check for confirmation
                                    //{
                                    //    b = str.DueDate;
                                    //}
                                    b = b.AddMonths(i);
                                    //This Piece of Code is for skipping monthly insatllment when half yearly is due.
                                    //var insRes = plot_Installments.Where(x => x.DueDate == new DateTime(b.Year, b.Month, 1)).FirstOrDefault();
                                    //while (insRes != null)
                                    //{
                                    //    b = b.AddMonths(1);
                                    //    insRes = plot_Installments.Where(x => x.DueDate == new DateTime(b.Year, b.Month, 1)).FirstOrDefault();
                                    //}
                                    DateTime dt = new DateTime(b.Year, b.Month, 1);

                                    Plot_Installments fi = new Plot_Installments()
                                    {
                                        Owner_Id = GroupTag,
                                        Status = InstallmentPaymentStatus.Pending.ToString(),
                                        DueDate = dt,
                                        Amount = eachinst,
                                        Plot_Id = PlotId,
                                        Installment_Name = i + " Monthly Installment",
                                        Installment_Type = "1"
                                    };
                                    plot_Installments.Add(fi);
                                }
                            }
                        }


                        var installmentplan = new XElement("Plots", plot_Installments.Select(x => new XElement("Installments",
                                                new XAttribute("Owner_Id", x.Owner_Id),
                                                new XAttribute("Amount", x.Amount),
                                                new XAttribute("DueDate", x.DueDate),
                                                new XAttribute("Installment_Name", x.Installment_Name),
                                                new XAttribute("Installment_Type", x.Installment_Type),
                                                new XAttribute("Status", x.Status)
                                                ))).ToString();

                        db.Sp_Update_PlotInstallments(plot.Id, 0, 0, 0, installmentplan);
                    }

                    db.Sp_Update_PlotStatus(Plot_Id, PlotsStatus.Registered.ToString());
                    db.Sp_Add_Activity(userid, "Register Plot Data of Plot Id. <a class='plt-data' data-id=' " + Plot_Id + "'>" + Plot_Id + "</a> To Name : " + string.Join("-", Owners.Select(x => x.Name)), "Create", Modules.PlotManagement.ToString(), ActivityType.Add_Plot_Owner.ToString(), Plot_Id);
                    db.Sp_Add_PlotComments(Plot_Id, "Register Plot to Owners : " + string.Join("-", Owners.Select(x => x.Name)), userid, ActivityType.Add_Plot_Owner.ToString());

                    // Receipt Add 

                    var voucher_amt = Math.Round(Convert.ToDecimal((res2.GrandTotal * res2.Percentage_Adj) / 100), 2);

                    //string desc = "Adjustment Voucher Against the Booking of Plot " + plot.Plot_No + "-" + plot.Type + " Block No: " + plot.Block_Name;
                    //var res5 = db.Sp_Add_Voucher(dealers.Select(x => x.Address).FirstOrDefault(), voucher_amt, GeneralMethods.NumberToWords((int)voucher_amt), "", "", null, "", string.Join("-", dealers.Select(x => x.Mobile_1)), desc,
                    //    string.Join("-", dealers.Select(x => x.Name)), dealership.Id, Modules.Dealers.ToString(), dealership.Dealership_Name, "Cash", "SA Gardens",
                    //"", userid, Payments.Adjustment.ToString(), userid, null, comp.Id).FirstOrDefault();
                    //var a = db.Sp_Add_VoucherDetails(voucher_amt, desc, null, null, null, res5.Receipt_Id).FirstOrDefault();

                    var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

                    if (brdd.PaymentType == "Cash")
                    {
                        if (brdd.Amount > 0)
                        {
                            var res3 = db.Sp_Add_Receipt(brdd.Amount, GeneralMethods.NumberToWords((int)brdd.Amount), "", "", null, "", string.Join("-", Owners.Select(x => x.Mobile_1))
                        , string.Join("-", Owners.Select(x => x.Father_Husband)), Plot_Id, string.Join("-", Owners.Select(x => x.Name)), "Cash", Plot_Total_Price,
                        "MED", Rate_Per_Marla, null, plot.Plot_Size, ReceiptTypes.Booking.ToString(), userid, userid, "", null, Modules.PlotManagement.ToString(), "", plot.Plot_No, plot.Block_Name, plot.Type, GroupTag, TransactionId, res2.DealerName, receiptno, comp.Id).FirstOrDefault();

                            Receipt_No = res3.Receipt_No;
                            if (res3.Receipt_Id == -1)
                            {
                                Transaction.Rollback();
                                return Json(new { Status = false, Msg = "Cannot Reregister the Plot again." });
                            }
                        }
                        else
                        {
                            return Json(new { Status = false, Msg = "Amount is Invalid!" });
                        }
                    }
                    else
                    {
                        if (brdd.Ch_bk_Pay_Date !=null && brdd.Amount <= 0)
                        {
                            var res3 = db.Sp_Add_Receipt(brdd.Amount, GeneralMethods.NumberToWords((int)brdd.Amount), brdd.Bank, brdd.PayChqNo, brdd.Ch_bk_Pay_Date, brdd.Branch, string.Join("-", Owners.Select(x => x.Mobile_1))
                            , string.Join("-", Owners.Select(x => x.Father_Husband)), Plot_Id, string.Join("-", Owners.Select(x => x.Name)), brdd.PaymentType, Plot_Total_Price,
                            "MED", Rate_Per_Marla, null, plot.Plot_Size, ReceiptTypes.Booking.ToString(), userid, userid, "", null, Modules.PlotManagement.ToString(), "", plot.Plot_No, plot.Block_Name, plot.Type, GroupTag, TransactionId, res2.DealerName, receiptno, comp.Id).FirstOrDefault();

                            var res4 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(brdd.Amount, brdd.Bank, brdd.Branch, brdd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                            Modules.PlotManagement.ToString(), Types.Booking.ToString(), userid, brdd.PayChqNo, Plot_Id, brdd.Ch_bk_Pay_Date, plot.Plot_No.ToString(), res3.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                            Receipt_No = res3.Receipt_No;
                            if (res3.Receipt_Id == -1)
                            {
                                Transaction.Rollback();
                                return Json(new { Status = false, Msg = "Cannot Reregister the Plot again." });
                            }
                        }
                        else
                        {
                            return Json(new { Status = false, Msg = "Either Amount is Invalid or Instrument Date is not Provided!" });
                        }
                        

                    }
                    //if (res3.Receipt_Id == -1)
                    //{
                    //    Transaction.Rollback();
                    //    return Json(new { Status = false, Msg = "Cannot Reregister the Plot again." });
                    //}
                    db.SP_Update_PlotVerificationToNull(Plot_Id);
                    bool headcashier = false;
                    {
                        Dealer_Commession d = new Dealer_Commession()
                        {
                            Module = Modules.FileManagement.ToString(),
                            Com_Type = "Fixed",
                            Com_Maturity = 25,
                            Percentage = 0,
                            File_Plot_Id = plot.Id,
                            Com_Amount = res2.CommisionAmount,
                            Dealer_Id = dealership.Id,
                            Dealer_Name = dealership.Dealership_Name,
                            Plot_No = plot.Plot_No,
                            Plot_Type = plot.Type,
                            Block = plot.Block_Name,
                            Total_Price = res2.GrandTotal
                        };
                        DealershipController dc = new DealershipController();
                        dc.AddCommession(d);
                    }
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }
                    if (isPayment == true)
                    {
                        try
                        {
                            //AccountHandlerController de = new AccountHandlerController();
                            //var grpTrans = h.RandomNumber();
                            //var jv = db.Sp_Add_Journal_Voucher().FirstOrDefault();

                            //de.Register_Plot(voucher_amt, plot.Plot_No, plot.Type, plot.Block_Name, grpTrans, userid, 2, jv.ToString(), AccountingModuleFP, plot.BlockIden);
                            //de.Receive_Plot_Amount((decimal)voucher_amt, plot.Plot_No, plot.Type, plot.Block_Name, "Cash", "", null, "", grpTrans, userid, res3.Receipt_No, 1, headcashier, AccountingModuleFP, plot.BlockIden);

                            //var jv1 = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                            //de.DealerComeRec(res2.CommisionAmount, dealership.Dealership_Name, "Dealer Commession on Booking of Plot " + plot.Plot_No + "-" + plot.Type + " Block No: " + plot.Block_Name, grpTrans, userid, 3, jv1.ToString(), headcashier, dealership.Id);
                            // Dealership Commession

                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        }
                    }
                    else
                    {
                        try
                        {
                            //AccountHandlerController de = new AccountHandlerController();
                            //var grpTrans = h.RandomNumber();
                            //var jv = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                            ////de.Register_Plot(voucher_amt, plot.Plot_No, plot.Type, plot.Block_Name, grpTrans, userid, 3, jv.ToString(), AccountingModuleFP, plot.BlockIden);
                            ////de.Receive_Plot_Amount((decimal)voucher_amt, plot.Plot_No, plot.Type, plot.Block_Name, "Cash", "", null, "", grpTrans, userid, res3.Receipt_No, 2, headcashier, AccountingModuleFP, plot.BlockIden);
                            ////de.DealerAdvanceRet(voucher_amt, dealership.Dealership_Name, "Cash", "", null, "", desc, grpTrans, userid, res5.Receipt_No, 1, headcashier, AccountingModuleFP, dealership.Id);
                            //var jv1 = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                            //de.DealerComeRec(res2.CommisionAmount, dealership.Dealership_Name, "Dealer Commession on Booking of Plot " + plot.Plot_No + "-" + plot.Type + " Block No: " + plot.Block_Name, grpTrans, userid, 4, jv1.ToString(), headcashier, dealership.Id);
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        }
                    }
                    Transaction.Commit();
                    this.updatebalance(plot.Id);
                    return Json(new { Status = true, ReceiptId = Receipt_No, Token = userid });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString(), "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return Json(new { Status = false, Msg = "Error Occured" });
                }
            }
        }
        public JsonResult GetPlots(long Id, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Plot List of Block Id  <a class='blk-data' data-id=' " + Id + "'>" + Id + "</a> of Type " + Type, "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            var res = db.Sp_Get_BlockPlotNoList(Id, Type).ToList();
            return Json(res);
        }
        public JsonResult GetPlotData(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get Plot Short Data Info of Id <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            var res = db.Sp_Get_PlotData(Id).FirstOrDefault();
            return Json(res);
        }
        public JsonResult SavePlotOwnerData(Plot_Ownership PO, List<PlotsInstallments> PI, Dealer_Commession Com)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            Helpers h = new Helpers();
            var Installments = new XElement("Plots", PI.Select(x => new XElement("Installments",
               new XAttribute("Amount", x.Amount),
               new XAttribute("DueDate", x.DueDate),
               new XAttribute("Installment_Name", x.InstNo)
               ))).ToString();
            var plot = db.Sp_Get_PlotData(PO.Plot_Id).FirstOrDefault();
            Helpers H = new Helpers();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res1 = db.Sp_Add_PlotOwnerShip(PO.Plot_Size, PO.Name, PO.Father_Husband, PO.CNIC_NICOP, PO.Mobile_1,
                                                        PO.Mobile_2, PO.Phone_Office, PO.Residential, PO.Postal_Address, PO.Residential_Address,
                                                        PO.Email, PO.Occupation, PO.Nationality, PO.Date_Of_Birth, PO.Nominee_Name,
                                                        PO.Nominee_Relation, PO.Nominee_Address, PO.Nominee_CNIC_NICOP, PO.Plot_Id, PO.City, PO.Total_Price,
                                                        PO.Owner_Img, PO.Discount, userid, DateTime.Now, "Owner", h.RandomNumber(), Installments).FirstOrDefault();
                    db.Sp_Add_Activity(userid, "Added Plot Owner Data of Plot Id. <a class='plt-data' data-id=' " + PO.Plot_Id + "'>" + PO.Plot_Id + "</a> To Name : " + PO.Name, "Create", Modules.PlotManagement.ToString(), ActivityType.Add_Plot_Owner.ToString(), PO.Plot_Id);
                    db.Sp_Add_PlotComments(PO.Plot_Id, "Add new Owner Name: " + PO.Name, userid, ActivityType.Add_Plot_Owner.ToString());
                    //Commenting the below code due to error in saving
                    {
                        //var jv = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                        //try
                        //{
                        //    AccountHandlerController de = new AccountHandlerController();
                        //    de.Register_Plot(PO.Total_Price, plot.Plot_No, plot.Type, plot.Block_Name, H.RandomNumber(), userid, 1, jv.ToString());
                        //}
                        //catch (Exception ex)
                        //{
                        //    Transaction.Rollback();
                        //    var a = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, "Plots", "SavePlotOwnerData");
                        //}
                    }
                    var reg = res1.ToString() + PO.Plot_Id.ToString();
                    reg = reg.PadLeft(6, '0');
                    db.Sp_Update_PlotRegNo(res1, reg);
                    if (Com != null)
                    {
                        string text = "";
                        if (Com.Com_Type == "Fixed")
                        {
                            text = "Plot No " + plot.Plot_No + " Block : " + plot.Block_Name + " with  " + Com.Com_Type + " Commession value " + string.Format("{0:n0}", Com.Com_Amount) + " on Booking value " + string.Format("{0:n0}", PO.Total_Price);
                        }
                        else if (Com.Com_Type == "Percentage")
                        {
                            text = "Plot No " + plot.Plot_No + " Block : " + plot.Block_Name + " with  " + Com.Percentage + "% Commession value " + string.Format("{0:n0}", Com.Com_Amount) + " on Booking value " + string.Format("{0:n0}", PO.Total_Price);
                        }
                        Dealer_Commession d = new Dealer_Commession()
                        {
                            Com_Amount = Com.Com_Amount,
                            Com_Type = Com.Com_Type,
                            Dealer_Id = Com.Dealer_Id,
                            Dealer_Name = Com.Dealer_Name,
                            File_Plot_Id = PO.Plot_Id,
                            Module = Modules.PlotManagement.ToString(),
                            Text = text,
                            Datetime = DateTime.Now,
                            Com_Maturity = Com.Com_Maturity
                        };
                        db.Dealer_Commession.Add(d);
                        db.SaveChanges();
                    }
                    Transaction.Commit();
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    var a = db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, "Plots", "SavePlotOwnerData");
                    return Json(new Return { Status = false, Msg = "Cannot Register this Plot Right Now." });
                }
            }
            var res = db.Sp_Get_PlotInstallments(PO.Plot_Id).ToList();
            return Json(res);
        }
        public JsonResult PlotTransferData(Plot_Ownership PO)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            long TransactionId = new Helpers().RandomNumber();
            db.Sp_Add_Activity(userid, "Transfer Plot with Id <a class='plt-data' data-id=' " + PO.Plot_Id + "'>" + PO.Plot_Id + "</a> To Name " + PO.Name, "Update", Modules.PlotManagement.ToString(), ActivityType.Record_Upatation.ToString(), PO.Plot_Id);
            db.Sp_Add_PlotComments(PO.Plot_Id, "Transfer Plot to Owner : " + PO.Name, userid, ActivityType.Record_Upatation.ToString());
            var res1 = db.Sp_Add_PlotTransfer(PO.Plot_Size, PO.Name, PO.Father_Husband, PO.CNIC_NICOP, PO.Mobile_1,
                PO.Mobile_2, PO.Phone_Office, PO.Residential, PO.Postal_Address, PO.Residential_Address,
                PO.Email, PO.Occupation, PO.Nationality, PO.Date_Of_Birth, PO.Nominee_Name,
                PO.Nominee_Relation, PO.Nominee_Address, PO.Nominee_CNIC_NICOP, PO.Plot_Id, PO.City, PO.Total_Price, PO.Ownership_Status,
                PO.Owner_Img, PO.Ownership_DateTime, TransactionId).FirstOrDefault();
            var reg = res1.ToString() + PO.Plot_Id.ToString();
            reg = reg.PadLeft(6, '0');
            db.Sp_Update_PlotRegNo(res1, reg);
            var plot = db.Sp_Get_PlotData(PO.Plot_Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(PO.Plot_Id).OrderByDescending(x => x.Ownership_DateTime).ToList();
            var CurrentOwner = res2.FirstOrDefault();
            if (PO.Ownership_DateTime == CurrentOwner.Ownership_DateTime && PO.Name == CurrentOwner.Name && PO.CNIC_NICOP == CurrentOwner.CNIC_NICOP)
            {
                if (plot.Verified == true)
                {
                    db.SP_Update_PlotVerificationToNull(PO.Plot_Id);
                    Notifier.Notify(userid, NotifyTo.Audit, NotifierMsg.Unverified, new object[] { plot }, NotifyType.Plots.ToString());
                }
            }
            db.Sp_Update_CurrentOwner(PO.Plot_Id);
            return Json(true);
        }
        public ActionResult PlotVerification(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access full Data of Plot Id <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>  for Verification ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var res = new PlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4 };
            return PartialView(res);
        }
        public ActionResult PlotDetails(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get full Details of Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>  ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            db.Sp_Update_CurrentOwner(Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            ViewBag.CNo = db.Plot_Ownership.Where(x => x.Plot_Id == Plotid).FirstOrDefault()?.Currency_Note_No ?? "Non Registered";
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var discount = db.Discounts.Where(x => x.Module_Id == Plotid && x.Module == Modules.PlotManagement.ToString() && x.Plot_Is_Cancelled == null).ToList();
            UpdatePlotInstallmentStatus(res3, res4, discount, Plotid);
            var fpb = db.File_Plot_Balance.Where(x => x.File_Plot_Id == Plotid && x.Module == "PlotManagement").FirstOrDefault();
            var res5 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res6 = db.Discounts.Where(x => x.Module_Id == Plotid && x.Module == Modules.PlotManagement.ToString() && x.Plot_Is_Cancelled == null).ToList();
            //surcharge 
            var res7 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var res8 = db.Sp_Get_PlotInstallments_Wht(Plotid).ToList();
            var res10 = db.Plot_Installments_Surcharge.Where(x => x.Plot_Id == Plotid && x.Modules == "PlotManagement").ToList();
            var res5surcharge = db.Plot_Installments_Surcharge.Where(x => x.Plot_Id == Plotid && x.Cancelled == null && x.Waveoff == null).OrderBy(x => x.DueDate).ToList();
            var res6surcharge = db.Sp_Get_ReceivedAmounts_Surcharge(Plotid, Modules.PlotManagement.ToString()).ToList();
            UpdatePlotInstallmentStatusSurcharge(res5surcharge, res6surcharge, Plotid);
            Cancellation_Receipts cr = null;
            if (res1.Status == PlotsStatus.Repurchased.ToString() || (res1.Status == PlotsStatus.Hold.ToString() && res2.Select(x => x.Ownership_Status).FirstOrDefault() == Ownership_Status.Refunded.ToString()))
            {
                cr = db.Cancellation_Receipts.Where(x => x.File_Plot_No == Plotid && x.Module == Modules.PlotManagement.ToString()).OrderByDescending(x => x.Id).FirstOrDefault();
            }
            var res = new PlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res5, PlotReceipts = res4, Discounts = res6, PlotBalDets = fpb, Refunded_Repurchased = cr , PlotInstallmentsSurcharge = res10, PlotInstallmentsWHT = res8};
            return PartialView(res);
        }

        public ActionResult UpdateWaveOffStatus(int id)
        {
            var plotInstallment = db.Plot_Installments_Surcharge.FirstOrDefault(p => p.Id == id);

            if (plotInstallment != null)
            {
                // Update the 'Waveoff' property
                plotInstallment.Waveoff = 1;

                // Save the changes to the database
                db.SaveChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public void UpdatePlotInstallmentStatusSurcharge(List<Plot_Installments_Surcharge> inst, List<Sp_Get_ReceivedAmounts_Surcharge_Result> Receipts, long? Plotid)
        {
            // db.Test_UpdatePendingPlotinstallmentWht(Plotid);

            decimal? TotalAmt = 0, AmttoPaid = 0, remamt = 0, TotalAmount = 0;

            string[] Type = { "SurCharge" };

            TotalAmount = Receipts.Where(x => Type.Contains(x.Type) /*&& (x.Status == null || x.Status == "Approved")*/).Sum(x => x.Amount);
            //if (Dis.Any())
            //{
            //    TotalAmount += Dis.Sum(x => x.Discount_Amount);
            //}
            var Actamt = TotalAmount;

            List<AmountToPaidInfo> latpi = new List<AmountToPaidInfo>();

            foreach (var item1 in inst)
            {
                AmountToPaidInfo atpi = new AmountToPaidInfo();
                TotalAmt += item1.SurchargeAmount;
                if (Math.Round(Convert.ToDecimal(TotalAmt)) <= Math.Round(Convert.ToDecimal(Actamt)))
                {
                    AmttoPaid += item1.SurchargeAmount;
                    atpi.Id = item1.Id;
                    latpi.Add(atpi);
                }
                else
                {
                    break;
                }
            }

            var allids = new XElement("IS", latpi.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();
            remamt = Actamt - AmttoPaid;
            db.Test_UpdatePlotinstallment_Surcharge(allids);

            var curdate = DateTime.Now;
            var res3 = db.Sp_Get_PlotInstallments_Surcharge(Plotid).ToList();
            var id = res3.Where(x => x.DueDate <= curdate && x.Status != "Paid").ToList();
            var nopaidis = new XElement("IS", id.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();

            remamt = remamt - id.Sum(x => x.Amount);

            db.Test_UpdatePlotsNotPaidinstallment_Surcharge(nopaidis);
            //  db.Test_updatebalanceWht(remamt, inst.Sum(x => x.Amount), TotalAmount, Plotid, Modules.PlotManagement.ToString(), id.Count(), 0, 0, 0, 0, 0, 0);

        }

        public ActionResult PlotReceipts(long PlotId)
        {
            var res1 = db.Sp_Get_PlotInstallments(PlotId).ToList();
            var res2 = db.Sp_Get_ReceivedAmounts(PlotId, Modules.PlotManagement.ToString()).ToList();
            var res3 = db.Discounts.Where(x => x.Module_Id == PlotId && x.Module == Modules.PlotManagement.ToString()).ToList();
            var res = new PlotDetailData { PlotInstallments = res1, PlotReceipts = res2, Discounts = res3 };
            return PartialView();
        }
        public ActionResult PlotInstallmentsReports(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).Where(x => x.Ownership_Status != "Cancelled").OrderBy(x => x.Ownership_DateTime).Select(x => new { x.Total_Price, x.Discount }).FirstOrDefault();
            var res3 = db.Sp_Get_PlotOwnerList(Plotid).Where(x => x.Ownership_Status != "Cancelled").OrderByDescending(x => x.Ownership_DateTime).Select(x => new { x.Total_Price, x.Discount }).FirstOrDefault();
            try
            {
                var inst = db.Sp_Get_PlotInstallments(Plotid).Select(x => new PlotStatment
                {
                    Description = x.Installment_Name,
                    Date = x.DueDate,
                    Debit = x.Amount,
                    Credit = 0,
                }).ToList();
                string[] Type = { "Booking", "Installment" };
                var rece = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
                {
                    Receipt_Voucher_No = x.ReceiptNo,
                    Credit = x.Amount,
                    Mode = x.PaymentType,
                    Chq_No = x.Ch_Pay_Draft_No,
                    Date = x.DateTime,
                    Bank = x.Bank,
                    RcvdDate = x.DateTime,
                    Type = x.Type,
                    Inst_Status = x.Status
                }).ToList();
                var discount = db.Discounts.Where(x => x.Module_Id == Plotid && x.Module == Modules.PlotManagement.ToString() && x.Plot_Is_Cancelled == null).ToList();
                ViewBag.TotalPrice = (res2 == null) ? 0 : res2.Total_Price;
                ViewBag.Discount = (res2 == null) ? 0 : res2.Discount ?? 0;
                var configmis = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
                Plot_Transfer_Configuration config = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(configmis.CurrentConfig);
                ViewBag.EstMrtVal = (res3 == null) ? 0 : -1;
                foreach (var blk in config.FeeInfo)
                {
                    if (blk.BlockName == res1.Block_Name && blk.IsApplicable == true)
                    {
                        ViewBag.EstMrtVal = (res3 == null) ? 0 : res3.Total_Price;
                    }
                }
                var payment = db.Vouchers.Where(x => x.File_Plot_Id == Plotid && x.Module == Modules.PlotManagement.ToString() && x.Plot_Is_Cancelled == null).Select(x => new PlotStatment
                {
                    Description = x.Description,
                    Date = x.DateTime,
                    Debit = x.Amount,
                    Credit = 0,
                }).ToList();
                List<PlotStatment> Rm = new List<PlotStatment>();
                Rm.AddRange(inst);
                Rm.AddRange(rece);
                Rm.AddRange(payment);
                Rm = Rm.OrderBy(x => x.Date).ToList();
                var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == Plotid && x.Module == Modules.PlotManagement.ToString()).FirstOrDefault();
                var res = new NewPlotLedger { PlotData = res1, Report = Rm, Discount = discount, Balance = bal };
                //var TotalPric = db.Plot_Installments.Where(x => x.Installment_Type != "10").Sum(x => x.Amount);
                //ViewBag.TotalPric = TotalPric;
                return PartialView(res);
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return PartialView(null);
            }
        }
        public ActionResult CancelledOwnerLedger(long Plotid, long OwnerId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Add_Activity(userid, "", "Create", Modules.PlotManagement.ToString());
            var inst = db.Sp_Get_PlotCancelOwnerInstallments(Plotid, OwnerId).Select(x => new ReportModel
            {
                Installments = x.Installment_Name,
                Due_Date = x.DueDate,
                Amount = x.Amount,
                Date = x.DueDate
            }).ToList();
            string[] Type = { "Booking", "Installment" };
            var rece = db.Sp_Get_ReceivedAmtCancelledOwner(Plotid, Modules.PlotManagement.ToString(), OwnerId).Where(x => Type.Contains(x.Type)).Select(x => new ReportModel
            {
                Receipt = x.ReceiptNo,
                Recp_Amount = x.Amount,
                Mode = x.PaymentType,
                Chq_No = x.Ch_Pay_Draft_No,
                Date = x.DateTime,
                Bank = x.Bank,
                Recp_Date = x.DateTime,
                Type = x.Type,
                Inst_Status = x.Status
            }).ToList();
            List<ReportModel> Rm = new List<ReportModel>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var res = new PlotLedger { Report = Rm, Discount = null };
            return PartialView("PlotInstallmentsReports", res);
        }

        [Authorize(Roles = "Plots Information,Administrator")]
        //[LicenseAuthorize]
        public ActionResult PlotInformation(long? bltId, string plotno)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Plot Information", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            if (bltId != null)
            {
                ViewBag.bltplt = bltId;
                ViewBag.bltpltno = plotno;
            }
            return View();
        }
        [Authorize(Roles = "Plots Information Update,Administrator")]
        public ActionResult UpdatePlotInformation()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Plot Updation", "Update", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }
        [Authorize(Roles = "Plots Information Update,Administrator")]
        public ActionResult UpdateInformation(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get full Details of Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>  for Updatations ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            var CurrencyNoteNumber = db.Plot_Ownership.Where(x => x.Plot_Id == Plotid).FirstOrDefault()?.Currency_Note_No;
            if (CurrencyNoteNumber != null) {
                ViewBag.currencyNo = CurrencyNoteNumber;
            }
            else
            {
                ViewBag.currencyNo = "N/A";
            }
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var res5 = db.Plot_Installments_Surcharge.Where(x => x.Plot_Id == Plotid && x.Modules == "PlotManagement").ToList();
            var res = new PlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4, PlotInstallmentsSurcharge = res5 };
            return PartialView(res);
        }
        public JsonResult UpdateConstructionStatus(long Id, string DevelopStatus)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_PlotComments(Id, "Update Construction Status to :" + DevelopStatus, userid, ActivityType.Plot_NOC.ToString());
            var res1 = db.Sp_Get_PlotData(Id).SingleOrDefault();
            var a = db.Sp_Update_PlotConstructionStatus(Id, DevelopStatus);
            return Json(new Return { Msg = "Updated", Status = true });
        }
        public JsonResult UpdateOwnerResult(Plot_Ownership PO)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var plot = db.Sp_Get_PlotData(PO.Plot_Id).FirstOrDefault();
            var res = db.Sp_Get_PlotOwnerId(PO.Id).SingleOrDefault();
            db.Sp_Update_PlotOwnerData(PO.Name, PO.Father_Husband, PO.Postal_Address, PO.Residential_Address,
               PO.City, PO.Date_Of_Birth, PO.CNIC_NICOP, PO.Nationality, PO.Email, PO.Phone_Office
               , PO.Residential, PO.Mobile_1, PO.Mobile_2, PO.Nominee_Name, PO.Nominee_CNIC_NICOP,
               PO.Nominee_Relation, PO.Nominee_Address, PO.Occupation, PO.Id, userid, PO.Owner_Img, PO.Owner_Img2, PO.Owner_Img3, PO.Owner_Img4, PO.Ownership_DateTime, PO.Ownership_Status, PO.Currency_Note_No);
            var data = JsonConvert.SerializeObject(res);
            db.Sp_Add_Activity(userid, "Update Data of Plot Id <a class='plt-data' data-id=' " + PO.Plot_Id + "'>" + PO.Plot_Id + "</a> Data: " + data, "Update", Modules.PlotManagement.ToString(), ActivityType.Record_Upatation.ToString(), PO.Plot_Id);
            if (res.Name != PO.Name && res.Name != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Name: " + res.Name + " To: " + PO.Name, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Ownership_Status != PO.Ownership_Status && res.Ownership_Status != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Ownership Staus: " + res.Ownership_Status + " To: " + PO.Ownership_Status, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Father_Husband != PO.Father_Husband && res.Father_Husband != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Father Name: " + res.Father_Husband + " To: " + PO.Father_Husband, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.CNIC_NICOP != PO.CNIC_NICOP && res.CNIC_NICOP != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update CNIC: " + res.CNIC_NICOP + " To: " + PO.CNIC_NICOP, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Mobile_1 != PO.Mobile_1 && res.Mobile_1 != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Mobile Number: " + res.Mobile_1 + " To: " + PO.Mobile_1, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Postal_Address != PO.Postal_Address && res.Postal_Address != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Postal Address: " + res.Postal_Address + " To: " + PO.Postal_Address, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.City != PO.City && res.City != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update City: " + res.City + " To: " + PO.City, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nationality != PO.Nationality && res.Nationality != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Nationality: " + res.Nationality + " To: " + PO.Nationality, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Date_Of_Birth != PO.Date_Of_Birth && res.Date_Of_Birth != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Date of Birth: " + res.Date_Of_Birth + " To: " + PO.Date_Of_Birth, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nominee_Name != PO.Nominee_Name && res.Nominee_Name != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Nominee Name: " + res.Nominee_Name + " To: " + PO.Nominee_Name, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nominee_Relation != PO.Nominee_Relation && res.Nominee_Relation != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Nominee Relation: " + res.Nominee_Relation + " To: " + PO.Nominee_Relation, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nominee_Address != PO.Nominee_Address && res.Nominee_Address != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Nominee Address: " + res.Nominee_Address + " To: " + PO.Nominee_Address, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Nominee_CNIC_NICOP != PO.Nominee_CNIC_NICOP && res.Nominee_CNIC_NICOP != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Nominee CNIC: " + res.Nominee_CNIC_NICOP + " To: " + PO.Nominee_CNIC_NICOP, userid, ActivityType.Record_Upatation.ToString()); }
            if (res.Owner_Img != PO.Owner_Img && res.Owner_Img != null) { db.Sp_Add_PlotComments(PO.Plot_Id, "Update Owner Image", userid, ActivityType.Record_Upatation.ToString()); }
            var res2 = db.Sp_Get_PlotOwnerList(PO.Plot_Id).OrderByDescending(x => x.Ownership_DateTime).ToList();
            var CurrentOwner = res2.FirstOrDefault();
            if (PO.Ownership_DateTime == CurrentOwner.Ownership_DateTime && PO.Name == CurrentOwner.Name && PO.CNIC_NICOP == CurrentOwner.CNIC_NICOP)
            {
                if (res.CNIC_NICOP != PO.CNIC_NICOP || res.Name != PO.Name)
                {
                    if (plot.Verified == true)
                    {
                        db.SP_Update_PlotVerificationToNull(PO.Plot_Id);
                        Notifier.Notify(userid, NotifyTo.Audit, NotifierMsg.Unverified, new object[] { plot }, NotifyType.Plots.ToString());
                    }
                }
            }
            db.Sp_Update_CurrentOwner(PO.Plot_Id);
            return Json(true);
        }
        public JsonResult AddNewPlotOwner(Plot_Ownership PO)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var plot = db.Sp_Get_PlotData(PO.Plot_Id).FirstOrDefault();

            // check if Plot_Ownership exists
            var plotOwnershipExists = db.Plot_Ownership.Any(p => p.CNIC_NICOP == PO.CNIC_NICOP && p.Plot_Id == PO.Plot_Id);

            if (plotOwnershipExists)
            {
                return Json(new { success = false, message = "Ownership Already Exists." });
            }
            else
            {
                db.Plot_Ownership.Add(PO);
                db.SaveChanges();
                var data = JsonConvert.SerializeObject(PO);
                db.Sp_Add_Activity(userid, "Updated Data of Owner trail of Plot Id <a class='plt-data' data-id=' " + PO.Plot_Id + "'>" + PO.Plot_Id + "</a> Data: " + data, "Update", Modules.PlotManagement.ToString(), ActivityType.Record_Upatation.ToString(), PO.Plot_Id);
                //var res2 = db.Sp_Get_PlotOwnerList(PO.Plot_Id).OrderByDescending(x => x.Ownership_DateTime).ToList();
                //var CurrentOwner = res2.FirstOrDefault();
                db.SP_Update_PlotVerificationToNull(PO.Plot_Id);
                //return Json(true);
                return Json(new { success = true, message = "Ownership Added Successfuly." });
            }
            
        }
        public JsonResult DeleteOwner(long OwnerId, long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            try
            {
                db.Sp_Add_Activity(userid, "Delete owner of Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>  for Updatations ", "Read", Modules.PlotManagement.ToString(), ActivityType.Delete_OwnerShip.ToString(), Plotid);
                var res = db.Sp_Get_PlotOwnerList(Plotid).Where(x => x.Id == OwnerId).FirstOrDefault();
                var plot = db.Sp_Get_PlotData(Plotid).FirstOrDefault();
                db.Sp_Add_PlotComments(Plotid, "Owner " + res.Name + " CNIC is" + res.CNIC_NICOP + " is removed by", userid, ActivityType.Delete_OwnerShip.ToString());
                db.Sp_Delete_PlotsOwnerShips(OwnerId);
                db.SP_Update_PlotVerificationToNull(Plotid);
                Notifier.Notify(userid, NotifyTo.Audit, NotifierMsg.Unverified, new object[] { plot }, NotifyType.Plots.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(true);
        }
        public ActionResult PlotDetailReport(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Ledger Report of Plot Id <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a> ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).FirstOrDefault();
            if (res1.Verified == true)
            {
                var userRes = db.SP_Get_PlotsVerifier(res1.Id).SingleOrDefault();
                ViewBag.verifier = (userRes == null) ? "Audit Department" : userRes.Name;
            }
            var inst = db.Sp_Get_PlotInstallments(Plotid).Select(x => new ReportModel
            {
                Installments = x.Installment_Name,
                Due_Date = x.DueDate,
                Amount = x.Amount,
                Date = x.DueDate
            }).ToList();
            string[] Type = { "Booking", "Installment" };
            var rece = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).Where(x => Type.Contains(x.Type)).Select(x => new ReportModel
            {
                Receipt = x.ReceiptNo,
                Recp_Amount = x.Amount,
                Mode = x.PaymentType,
                Chq_No = x.Ch_Pay_Draft_No,
                Date = x.DateTime,
                Bank = x.Bank,
                Recp_Date = x.DateTime,
                Type = x.Type,
                Inst_Status = x.Status
            }).ToList();
            var discount = db.Discounts.SingleOrDefault(x => x.Module_Id == Plotid && x.Module == Modules.PlotManagement.ToString());
            List<ReportModel> Rm = new List<ReportModel>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var res = new PlotLedger { OwnerData = res2, PlotData = res1, Report = Rm, Discount = discount };
            return View(res);
        }
        public ActionResult PlotStatment(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Ledger Report of Plot Id <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a> ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            //get company name
            var claim = db.UserClaims.Where(x => x.UserId == userid).ToList();
            ViewBag.Company = claim.Where(x => x.ClaimType == "Company").Select(x => x.ClaimValue).FirstOrDefault();

            var inst = db.Sp_Get_PlotInstallments(Plotid).Select(x => new PlotStatment
            {
                Description = x.Installment_Name,
                Date = x.DueDate,
                Debit = x.Amount,
                Credit = 0,
            }).ToList();
            string[] Type = { "Advance", "Booking", "Installment" };
            var rece = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
            {
                Receipt_Voucher_No = x.ReceiptNo,
                Credit = x.Amount,
                Mode = x.PaymentType,
                Chq_No = x.Ch_Pay_Draft_No,
                Date = x.DateTime,
                Bank = x.Bank,
                RcvdDate = x.DateTime,
                Type = x.Type,
                Inst_Status = x.Status
            }).ToList();
            var discount = db.Discounts.Where(x => x.Module_Id == Plotid && x.Module == Modules.PlotManagement.ToString()).ToList();
            var payment = db.Vouchers.Where(x => x.File_Plot_Id == Plotid && x.Module == Modules.PlotManagement.ToString()).Select(x => new PlotStatment
            {
                Description = x.Description,
                Date = x.DateTime,
                Debit = x.Amount,
                Credit = 0,
            }).ToList();
            List<PlotStatment> Rm = new List<PlotStatment>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            //Rm.AddRange(payment);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            Cancellation_Receipts cr = null;
            if (res1.Status == PlotsStatus.Repurchased.ToString() || (res1.Status == PlotsStatus.Hold.ToString() && res2.Select(x => x.Ownership_Status).FirstOrDefault() == Ownership_Status.Refunded.ToString()))
            {
                cr = db.Cancellation_Receipts.Where(x => x.File_Plot_No == Plotid && x.Module == Modules.PlotManagement.ToString()).OrderByDescending(x => x.Id).FirstOrDefault();
            }
            var balance = db.File_Plot_Balance.Where(x => x.File_Plot_Id == Plotid).FirstOrDefault();
            var res = new NewPlotLedger { OwnerData = res2, PlotData = res1, Report = Rm, Discount = discount, Balance = balance, Refunded_Repurchased = cr };
            return View(res);
        }
        public ActionResult PreviousPlotDetailReport(long Plotid, long OwnerId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Ledger Report of Plot Id <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a> ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).Where(x => x.Id == OwnerId).Select(x => new Sp_Get_PlotLastOwner_Result
            {
                Reg_No = x.Reg_No,
                Name = x.Name,
                CNIC_NICOP = x.CNIC_NICOP,
                Mobile_1 = x.Mobile_1,
                Postal_Address = x.Postal_Address,
                Total_Price = x.Total_Price,
                Discount = x.Discount
            }).ToList();
            var inst = db.Sp_Get_PlotCancelOwnerInstallments(Plotid, OwnerId).Select(x => new PlotStatment
            {
                Description = x.Installment_Name,
                Date = x.DueDate,
                Debit = x.Amount,
                Credit = 0,
            }).ToList();
            string[] Type = { "Booking", "Installment" };
            var rece = db.Sp_Get_ReceivedAmountsCancelOwner(Plotid, Modules.PlotManagement.ToString(), OwnerId).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
            {
                Receipt_Voucher_No = x.ReceiptNo,
                Credit = x.Amount,
                Mode = x.PaymentType,
                Chq_No = x.Ch_Pay_Draft_No,
                Date = x.DateTime,
                Bank = x.Bank,
                RcvdDate = x.DateTime,
                Type = x.Type,
                Inst_Status = x.Status
            }).ToList();
            var discount = db.Discounts.SingleOrDefault(x => x.Module_Id == Plotid && x.Module == Modules.PlotManagement.ToString());
            List<PlotStatment> Rm = new List<PlotStatment>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var res = new PlotLastOwnerLedger { OwnerData = res2, PlotData = res1, Report = Rm, Discount = discount };
            return View(res);
        }
        public JsonResult UpdatePlotDisputedStatus(long Id, bool Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string text = "";
            if (Status == false)
            {
                text = "Reinstate";
            }
            else
            {
                text = "Disputed";
            }
            db.Sp_Add_Activity(userid, "Updated Plot Status to " + text + " of Plot id <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a> ", "Create", Modules.PlotManagement.ToString(), ActivityType.Record_Upatation.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Update Plot Status to " + text, userid, ActivityType.Record_Upatation.ToString());
            db.Sp_Update_PlotDisputeStatus(Id, Status);
            return Json(true);
        }
        public ActionResult ReinstatePlot()
        {
            return PartialView();
        }
        public JsonResult PlotReinstate(long Id, string Remarks)
        {
            var res2 = db.Sp_Get_PlotLastOwner(Id).ToList();
            var userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_PlotOwnerStatus(Id, res2.Select(x => x.GroupTag).FirstOrDefault(), PlotsStatus.Registered.ToString(), "Owner");
            db.Sp_Add_Activity(userid, "Updated Status to Reinstate Cancelled <a class='fl-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Id);
            db.Sp_Add_PlotComments(Id, Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            db.Sp_Add_PlotComments(Id, "Reinstate the Plot. \n\r", userid, ActivityType.Plot_Status_Updation.ToString());
            return Json(true);
        }
        public JsonResult ReinstateWithTBA(long Id, string Remarks, int ReinstateType)
        {
            var res1 = db.Plots.Where(x => x.Id == Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Id).ToList();
            var userid = long.Parse(User.Identity.GetUserId());
            if (ReinstateType == 2)
            {
                var a = db.Sp_Update_DevelopmentCharges(Id, null, Modules.PlotManagement.ToString());
                var b = db.Sp_Update_PlotOwnerStatus(Id, res2.Select(x => x.GroupTag).FirstOrDefault(), PlotsStatus.Registered.ToString(), "Owner");
                db.Sp_Add_Activity(userid, "Updated Status to Reinstate Cancelled <a class='fl-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), Id);
                db.Sp_Add_PlotComments(Id, "Reinstate the Plot with Development Charges To-Be-Announced. \n\r", userid, ActivityType.Plot_Status_Updation.ToString());
                db.Sp_Add_PlotComments(Id, Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
            }
            else
            {
                var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
                try
                {
                    FileReinstateRequest pd = new FileReinstateRequest
                    {
                        DescriptionText = "Request for Reinstate of Plot Number : " + res1.Plot_Number + " - " + res1.Block_Name,
                        New_Status = PlotsStatus.Registered.ToString(),
                        Old_Status = res1.Status,
                        Urgency = UrgencyStatus.Urgent,
                        Module = Modules.PlotManagement.ToString()
                    };
                    var packed = JsonConvert.SerializeObject(pd);
                    db.MIS_Requests.Add(new MIS_Requests
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy_Id = userid,
                        CreatedBy_Name = uname,
                        Details = packed,
                        ModuleId = res1.Id,
                        ModuleType = RequestType.Reinstate_With_Fine.ToString(),
                        Status = RequestStatus.Pending.ToString(),
                        Type = RequestType.Reinstate_With_Fine.ToString()
                    });
                    db.SaveChanges();
                    return Json(new Return { Status = true, Msg = "Requested at Cash counter" });
                }
                catch (Exception ex)
                {
                    return Json(false);
                }
            }
            return Json(true);
        }
        public JsonResult UpdatePlotRegistryStatus(long Id, bool Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string text = "";
            if (Status == false)
            {
                text = "Remove Registry Status of Plot";
            }
            else
            {
                text = "Mark Plot as Registry Plot";
            }
            db.Sp_Add_Activity(userid, text, "Update", Modules.PlotManagement.ToString(), ActivityType.Record_Upatation.ToString(), Id);
            db.Sp_Add_PlotComments(Id, text, userid, ActivityType.Record_Upatation.ToString());
            db.Sp_Update_PlotRegistryStatus(Id, Status);
            return Json(true);
        }
        public ActionResult LastestPlotDetails(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var discount = db.Discounts.Where(x => x.Module_Id == Plotid && x.Plot_Is_Cancelled == null && x.Module == Modules.PlotManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, res4, discount, Plotid);
            var res5 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res = new LatestPlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res5, PlotReceipts = res4 };
            return PartialView(res);
        }
        public JsonResult LastestPlotDetailsData(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            string[] types = { "Booking", "Installment" };
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).Where(x => types.Contains(x.Type) && (x.Status == "Approved" || x.Status == null)).ToList();
            var res5 = db.Discounts.Where(x => x.Module == Modules.PlotManagement.ToString() && x.Plot_Is_Cancelled == null && x.Module_Id == Plotid).ToList();
            UpdatePlotInstallmentStatus(res3, res4, res5, Plotid);
            var plotTransfFee = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsedInfo = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(plotTransfFee.CurrentConfig);
            decimal rate = 0;
            var blockData = parsedInfo.FeeInfo.Where(x => x.BlockId == res1.BlockIden).FirstOrDefault();
            if (blockData != null && (blockData.IsApplicable))
            {
                if (res1.Type == "Residential")
                {
                    rate = blockData.DC_Rate_Per_Marla_Residential;
                }
                else
                {
                    rate = parsedInfo.FeeInfo.Where(x => x.BlockId == res1.BlockIden).FirstOrDefault().DC_Rate_Per_Marla_Commercial;
                }
            }
            decimal totalMarlas = Convert.ToDecimal(res1.Plot_Size.Split(' ')[0]);
            decimal DcRate = Math.Ceiling(rate * totalMarlas);
            var res6 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var alreadyTransList = db.Plots_Transfer_Request.Where(x => x.Status == 0 && x.Plot_Id == res1.Id).ToList();
            var res = new LatestPlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res6, PlotReceipts = res4, Discounts = res5, Plot_Price_DC_Rate = DcRate, TransferPending = alreadyTransList };
            return Json(res);
        }
        public ActionResult PlotsOwnersListUpadate(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Plot_Ownership.Where(x => x.Plot_Id == Id).ToList();
            return PartialView(res);
        }
        public ActionResult PlotRegisteration()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Plot Registeration", "Create", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }
        public ActionResult BidingRegisterPlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            return View();
        }
        public JsonResult DealerPlots(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = (from x in db.Biding_Reserve_Plots
                       join a in db.Plots on x.Plot_Id equals a.Id
                       join b in db.Plot_Rates on a.Id equals b.Plot_Com_Id
                       where x.Dealer_Id == Id
                       select new { PlotId = a.Id, Block = "Badar Block", PlotNumber = a.Plot_Number, Sector = a.Sector, Size = a.Plot_Size, Location = a.Plot_Location, Rate = b.RatePerMarla_SqFt }).ToList();
            return Json(res);
        }
        public ActionResult PlotsVerification(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get full Details of Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a> for verification ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var fpb = db.File_Plot_Balance.Where(x => x.File_Plot_Id == Plotid && x.Module == "PlotManagement").FirstOrDefault();
            var res = new PlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4, PlotBalDets = fpb, };
            return View(res);
        }
        public JsonResult VerifingPlot(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Update Plot id <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>  To Verify ", "Update", Modules.PlotManagement.ToString(), ActivityType.Plot_Verified.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Plot Verified", userid, ActivityType.Plot_Verified.ToString());
            db.SP_Update_VerifyStatus(Id, Modules.PlotManagement.ToString());
            return Json(true);
        }
        public ActionResult PostPlotVerification()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }
        public ActionResult PostPlotDetails(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get full Details of Plot Id <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a> ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var res = new PlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4 };
            return PartialView(res);
        }
        public JsonResult PostVerify(long Id, bool Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Update Plot id " + Id + " To Pre Verification : " + Status, "Update", Modules.PlotManagement.ToString(), ActivityType.Pre_Verification.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Plot Pre-Verified", userid, ActivityType.Plot_Verified.ToString());
            db.Sp_Update_PostVerificationPlot(Id, Status);
            return Json(true);
        }

        public ActionResult AddPlotCategory()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddPlotCategory1(decimal front, decimal back, decimal left, decimal right, decimal? north_east, decimal? north_west, decimal? south_east, decimal? south_west)
        {
                Plots_Category pc = db.Plots_Category.FirstOrDefault(p =>
                p.Front_Side == front &&
                p.Back_Side == back &&
                p.Left_Side == left &&
                p.Right_Side == right &&
                p.North_East == north_east &&
                p.North_West == north_west &&
                p.South_East == south_east &&
                p.South_West == south_west
                );

                if (pc == null)
                {
                    pc = new Plots_Category
                    {
                        Uom = "sqft",
                        Front_Side = front,
                        Back_Side = back,
                        Left_Side = left,
                        Right_Side = right,
                        Category = "marla",
                        North_East = north_east,
                        North_West = north_west,
                        South_East = south_east,
                        South_West = south_west
                    };

                    db.Plots_Category.Add(pc);
                    db.SaveChanges();
                
                return Json("Plot Category Addded!");
            }
            else {
                return Json("Plot Category Already Exists!");
            }
        }
        public ActionResult UpdatePlotDimension(string Road, string Plotsize, string Dimension, int Plot_Cat, decimal Area, string Location)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access Update Plot Dimensions", "Create", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Road = Road;
            ViewBag.Plotsize = Plotsize;
            ViewBag.LastDimension = Dimension;
            ViewBag.Area = Area;
            ViewBag.PlotCat = Plot_Cat;
            ViewBag.Location = Location;
            ViewBag.Dimension = new SelectList(db.Sp_Get_PlotsCategory(), "Id", "Dimension", Plot_Cat); //Updated this Procedure to add all dimensions
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdatePlotDimension1(string Road, int Dimension, long Id, string Plot_Size, decimal Area, string Location)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_PlotData(Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Updated the Plot Dimensions  <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Update", Modules.PlotManagement.ToString(), ActivityType.Dimension_Updation.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Updated the Plot Dimensions from: " + res.Plot_Size, userid, ActivityType.Dimension_Updation.ToString());
            db.SP_Update_PlotDimension(Road, Area, Dimension, Plot_Size + " Marla", Location, Id);
            if ((res.Verified == true) && (res.Plot_Category != Dimension))
            {
                db.SP_Update_PlotVerificationToNull(Id);
                // Notify
                Notifier.Notify(userid, NotifyTo.Audit, NotifierMsg.Unverified, new object[] { res }, NotifyType.Plots.ToString());
            }
            return Json(true);
        }
        public void UpdatePlotInstallmentStatus(List<Sp_Get_PlotInstallments_Result> inst, List<Sp_Get_ReceivedAmounts_Result> Receipts, List<Discount> Dis, long Plotid)
        {
            db.Test_UpdatePendingPlotinstallment(Plotid);
            decimal? TotalAmt = 0, AmttoPaid = 0, remamt = 0, TotalAmount = 0;
            string[] Type = { "Advance", "Booking", "Installment" };
            TotalAmount = Receipts.Where(x => Type.Contains(x.Type) && (x.Status == null || x.Status == "Approved")).Sum(x => x.Amount);
            var ReceivedAmount = TotalAmount;
            if (Dis.Any())
            {
                TotalAmount += Dis.Sum(x => x.Discount_Amount);
            }
            var voucher = db.Vouchers.Where(x => x.File_Plot_Id == Plotid && x.Module == Modules.PlotManagement.ToString() && x.Plot_Is_Cancelled == null).ToList();
            if (voucher.Any())
            {
                TotalAmount -= voucher.Sum(x => x.Amount);
            }
            //Total Paid Amount
            var Actamt = TotalAmount;

            List<AmountToPaidInfo> latpi = new List<AmountToPaidInfo>();
            foreach (var item1 in inst)
            {
                AmountToPaidInfo atpi = new AmountToPaidInfo();
                TotalAmt += item1.Amount;
                if (Math.Round(Convert.ToDecimal(TotalAmt)) <= Math.Round(Convert.ToDecimal(Actamt)))
                {
                    AmttoPaid += item1.Amount;
                    atpi.Id = item1.Id;
                    latpi.Add(atpi);
                }
                else
                {
                    break;
                }
            }
            var allids = new XElement("IS", latpi.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();
            remamt = Actamt - AmttoPaid;
            db.Test_UpdatePlotinstallment(allids);
            var curdate = DateTime.Now;
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var id = res3.Where(x => x.DueDate <= curdate && x.Status != "Paid").ToList();
            var nopaidis = new XElement("IS", id.Select(x => new XElement("ISS", new XAttribute("Id", x.Id)))).ToString();
            remamt = remamt - id.Sum(x => x.Amount);
            db.Test_UpdatePlotsNotPaidinstallment(nopaidis);
            var duedate = inst.OrderBy(x => x.DueDate).FirstOrDefault();
            var res1 = db.Plot_Installments.Where(x => x.Plot_Id == Plotid).ToList();
            if (duedate != null)
            {
                //db.Test_updatebalance(remamt, inst.Sum(x => x.Amount), ReceivedAmount, Dis.Sum(x => x.Discount_Amount), Plotid, Modules.PlotManagement.ToString(), id.Count(), duedate.DueDate, voucher.Sum(x => x.Amount));
                db.Test_updatebalance(remamt, res1.Where(x => x.Installment_Type != "10").Sum(x => x.Amount), ReceivedAmount, Dis.Sum(x => x.Discount_Amount), Plotid, Modules.PlotManagement.ToString(), id.Count(), duedate.DueDate, voucher.Sum(x => x.Amount));

            }
            else
            {
                //.Where(x => x.Installment_Type != "2")
                //db.Test_updatebalance(remamt, inst.Sum(x => x.Amount), ReceivedAmount, Dis.Sum(x => x.Discount_Amount), Plotid, Modules.PlotManagement.ToString(), id.Count(), null, voucher.Sum(x => x.Amount));
                db.Test_updatebalance(remamt, res1.Where(x => x.Installment_Type != "10").Sum(x => x.Amount), ReceivedAmount, Dis.Sum(x => x.Discount_Amount), Plotid, Modules.PlotManagement.ToString(), id.Count(), duedate.DueDate, voucher.Sum(x => x.Amount));

            }
        }
        public JsonResult VerifyReq(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Request for Verification of Plot <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Verified.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Request for Verification", userid, ActivityType.Plot_Verified.ToString());
            db.Sp_Update_RequestForVerify_Plot(Id);
            return Json(true);
        }
        public ActionResult ReqVerificationList()
        {
            var res = db.Sp_Get_PlotVerifReq().ToList();
            return View(res);
        }
        public ActionResult PlotNOC(long Id, string CustomeType)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_PlotData(Id).SingleOrDefault();
            ViewBag.Purpose = CustomeType;
            if (res1.Status != "Registered")
            {
                PlotNOC Overall = new PlotNOC();
                ViewBag.OverdueReason = "Plot Status is " + res1.Status;
                return View(Overall);
            }
            var res2 = db.Sp_Get_PlotLastOwner(Id).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Id, Modules.PlotManagement.ToString()).ToList();
            var res5 = db.Discounts.Where(x => x.Module_Id == Id && x.Module == Modules.PlotManagement.ToString());
            var TotalInstAmt = res3.Sum(x => x.Amount);
            var ReceAmt = res4.Where(x => x.Status == null || x.Status == "Approved").Sum(x => x.Amount);
            if (res5.Any())
            {
                ReceAmt += res5.Sum(x => x.Discount_Amount);
            }
            if (Math.Round(TotalInstAmt) > Math.Round(Convert.ToDecimal(ReceAmt)))
            {
                PlotNOC Overall = new PlotNOC();
                ViewBag.OverdueReason = "Overdue amount of Installments";
                return View(Overall);
            }
            var Serv = db.ServiceCharges_Bill.Where(x => x.Plot_Id == Id).OrderByDescending(x => x.Id).FirstOrDefault();
            var Elec = db.Electricity_Bill.Where(x => x.Plot_Id == Id).OrderByDescending(x => x.Id).FirstOrDefault();
            if (Serv != null)
            {
                if (Math.Round(Convert.ToDecimal(Serv.Remaining)) > 0)
                {
                    PlotNOC Overall = new PlotNOC();
                    ViewBag.OverdueReason = "Overdue Amount of Service Charges";
                    return View(Overall);
                }
            }
            if (Elec != null)
            {
                if (Math.Round(Convert.ToDecimal(Elec.Remaining)) > 0)
                {
                    PlotNOC Overall = new PlotNOC();
                    ViewBag.OverdueReason = "Overdue amount of Electricity Charges";
                    return View(Overall);
                }
            }
            var dis = ((res5.Any()) ? res5.Sum(x => x.Discount_Amount) : 0);
            var DueAmt = TotalInstAmt - ReceAmt - dis;
            DueAmt = (DueAmt < 0) ? 0 : Math.Round(Convert.ToDecimal(DueAmt), 0);
            Helpers h = new Helpers();
            var num = h.RandomNumber().ToString();
            db.Sp_Add_PlotNOC(string.Join(",", res2.Select(x => x.Name)), res1.Phase_Name, res1.Id, res1.Plot_No, TotalInstAmt, DueAmt, num, res1.Plot_Size, res1.Type, userid, res1.Block_Name);
            db.Sp_Add_PlotComments(Id, "Generated Customer  " + CustomeType + " NOC. \n\r", userid, ActivityType.Plot_NOC.ToString());
            PlotNOC res = new PlotNOC()
            {
                DateTime = DateTime.UtcNow,
                DueAmt = Convert.ToDecimal(DueAmt),
                Name = string.Join(",", res2.Select(x => x.Name)),
                Phase = res1.Phase_Name,
                Plot_Id = res1.Id,
                Plot_No = res1.Plot_No,
                Serial_No = num,
                Size = res1.Plot_Size,
                TotalAmt = TotalInstAmt,
                Type = res1.Type,
                Userid = userid,
                Block = res1.Block_Name,
                Registery = res1.Registry,
                Mortgage = res1.Mortgage,
                Dup_Allotment_Letter = res2.Select(x => x.Dup_Allotment_Letter).FirstOrDefault()
            };
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            return View(res);
        }
        public ActionResult DealerNOC(long Id, string DealerType, long Dealer_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Dealership = db.Dealerships.Where(x => x.Id == Dealer_Id).Select(x => x.Dealership_Name).FirstOrDefault();
            var res1 = db.Sp_Get_PlotData(Id).SingleOrDefault();
            if (res1.Status != "Registered")
            {
                PlotNOC Overall = new PlotNOC();
                ViewBag.OverdueReason = "Plot Status is " + res1.Status;
                return View(Overall);
            }
            var res2 = db.Sp_Get_PlotLastOwner(Id).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Id).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Id, Modules.PlotManagement.ToString()).ToList();
            var res5 = db.Discounts.Where(x => x.Module_Id == Id && x.Module == Modules.PlotManagement.ToString());
            ViewBag.Purpose = DealerType;
            var TotalInstAmt = res3.Sum(x => x.Amount);
            var ReceAmt = res4.Where(x => x.Status == null || x.Status == "Approved").Sum(x => x.Amount);
            if (res5.Any())
            {
                ReceAmt += res5.Sum(x => x.Discount_Amount);
            }
            if (Math.Round(TotalInstAmt) > Math.Round(Convert.ToDecimal(ReceAmt)))
            {
                PlotNOC Overall = new PlotNOC();
                ViewBag.OverdueReason = "Overdue amount of Installments";
                return View(Overall);
            }
            var Serv = db.ServiceCharges_Bill.Where(x => x.Plot_Id == Id).OrderByDescending(x => x.Id).FirstOrDefault();
            var Elec = db.Electricity_Bill.Where(x => x.Plot_Id == Id).OrderByDescending(x => x.Id).FirstOrDefault();
            if (Serv != null)
            {
                if (Math.Round(Convert.ToDecimal(Serv.Remaining)) > 0)
                {
                    PlotNOC Overall = new PlotNOC();
                    ViewBag.OverdueReason = "Overdue Amount of Service Charges";
                    return View(Overall);
                }
            }
            if (Elec != null)
            {
                if (Math.Round(Convert.ToDecimal(Elec.Remaining)) > 0)
                {
                    PlotNOC Overall = new PlotNOC();
                    ViewBag.OverdueReason = "Overdue amount of Electricity Charges";
                    return View(Overall);
                }
            }
            var dis = ((res5.Any()) ? res5.Sum(x => x.Discount_Amount) : 0);
            var DueAmt = TotalInstAmt - ReceAmt - dis;
            DueAmt = (DueAmt < 0) ? 0 : Math.Round(Convert.ToDecimal(DueAmt), 0);
            Helpers h = new Helpers();
            var num = h.RandomNumber().ToString();
            db.Sp_Add_PlotNOC(string.Join(",", res2.Select(x => x.Name)), res1.Phase_Name, res1.Id, res1.Plot_No, TotalInstAmt, DueAmt, num, res1.Plot_Size, res1.Type, userid, res1.Block_Name);
            db.Sp_Add_PlotComments(Id, "Generated Dealer  " + DealerType + " NOC for " + ViewBag.Dealership + ". \n\r ", userid, ActivityType.Plot_NOC.ToString());
            PlotNOC res = new PlotNOC()
            {
                DateTime = DateTime.UtcNow,
                DueAmt = Convert.ToDecimal(DueAmt),
                Name = string.Join(",", res2.Select(x => x.Name)),
                Phase = res1.Phase_Name,
                Plot_Id = res1.Id,
                Plot_No = res1.Plot_No,
                Serial_No = num,
                Size = res1.Plot_Size,
                TotalAmt = TotalInstAmt,
                Type = res1.Type,
                Userid = userid,
                Block = res1.Block_Name,
                Registery = res1.Registry,
                Mortgage = res1.Mortgage,
                Dup_Allotment_Letter = res2.Select(x => x.Dup_Allotment_Letter).FirstOrDefault()
            };
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            return View(res);
        }
        public JsonResult CheckPosessionNOC(long PlotId)
        {
            var res = db.Receipts.Where(x => x.File_Plot_No == PlotId && x.Type == "Posession_Charges").FirstOrDefault();
            if (res == null)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }
        public ActionResult PlotCustomerFile(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var discount = db.Discounts.Where(x => x.Module_Id == Plotid && x.Plot_Is_Cancelled == null && x.Module == Modules.PlotManagement.ToString()).ToList();
            UpdatePlotInstallmentStatus(res3, res4, discount, Plotid);
            var res5 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            db.Sp_Add_Activity(userid, "Generate Customer File for Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>  ", "Create", Modules.PlotManagement.ToString(), ActivityType.Customer_File.ToString(), Plotid);
            db.Sp_Add_PlotComments(Plotid, "Generate Customer File for : " + string.Join(",", res2.Select(x => x.Name)), userid, ActivityType.Customer_File.ToString());
            var res = new LatestPlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4 };
            return View(res);
        }
        public ActionResult PlotCustomer_App(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string name = db.Users.SingleOrDefault(x => x.Id == userid).Name;
            ViewBag.Name = name;
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            var transCheck = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            ViewBag.isTrans = transCheck.Any(x => x.Ownership_Status == "Transfered");
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var res5 = db.Installment_Structure.Where(x => x.Installment_Plot_Id == res1.Installment_Plan_Id).ToList();
            if (!System.IO.File.Exists(Server.MapPath("~/QR Codes/" + res1.Plot_No + "-" + res1.Type + "-" + res1.Block_Name + ".png")))
            {
                Helpers h = new Helpers(Modules.PlotManagement, Types.Booking);
                {
                    object[] data = new object[4];
                    data[0] = res1.Plot_No;
                    data[1] = res1.Block_Name;
                    data[2] = res1.Type;
                    data[3] = res1.Plot_Size;
                    var QR_Data = h.GenerateQRCode(data);
                }
            }
            db.Sp_Add_Activity(userid, "Generate Customer File for Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>  ", "Create", Modules.PlotManagement.ToString(), ActivityType.Customer_File.ToString(), Plotid);
            db.Sp_Add_PlotComments(Plotid, "Generate Customer File for : " + string.Join(",", res2.Select(x => x.Name)), userid, ActivityType.Customer_File.ToString());
            var res = new LatestPlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4, InstalmentStructureDetail = res5 };
            return View(res);
        }
        public void CancelPlots()
        {
            var plots = db.Plots.Where(x => x.Status == "Registered").Select(x => x.Id).ToList();
            foreach (var Plotid in plots)
            {
                try
                {
                    var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
                    var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
                    var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
                    var res5 = db.Sp_Get_PlotInstallments(Plotid).ToList();
                    var res6 = db.Discounts.Where(x => x.Module_Id == Plotid && x.Plot_Is_Cancelled == null && x.Module == "PlotManagement").ToList();
                    UpdatePlotInstallmentStatus(res3, res4, res6, Plotid);
                    var date = res5.Where(x => x.Status == "NotPaid").OrderByDescending(x => x.Id).FirstOrDefault();
                    if (date != null)
                    {
                        var now = DateTime.Now;
                        var startOfMonth = new DateTime(now.Year, now.Month, 1);
                        var lastdate = startOfMonth.AddMonths(0);
                        var instdate = new DateTime(date.DueDate.Year, date.DueDate.Month, 1);
                        if (instdate <= lastdate)
                        {
                            db.Sp_Update_PlotStatus(Plotid, PlotsStatus.Temporary_Cancelled.ToString());
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        public ActionResult VerifiedPlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Verified Plots", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Verified Plots";
            var res = db.Sp_Get_PlotList_Parameter("Verified").ToList();
            return View(res);
        }
        public ActionResult DisputedPlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Disputed Plots", "Create", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Disputed Plots";
            var res = db.Sp_Get_PlotList_Parameter("Disputed").ToList();
            return View("VerifiedPlots", res);
        }
        public ActionResult PendingApproval()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Pending For Approval", "Create", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Pending For Approval";
            var res = db.Sp_Get_PlotList_Parameter("PendingApproval").ToList();
            return View("VerifiedPlots", res);
        }
        public ActionResult NotVerified()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Not Verified Plots", "Create", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Not Verified Plots";
            var res = db.Sp_Get_PlotList_Parameter("Pending").ToList();
            return View("VerifiedPlots", res);
        }
        public ActionResult CancelledPlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Cancelled Plots", "read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Cancelled Plots";
            var res = db.Sp_Get_PlotList_Parameter("Cancelled").ToList();
            return PartialView("VerifiedPlots", res);
        }
        public ActionResult HoldedPlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Holded Plots", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Holded Plots";
            var res = db.Sp_Get_PlotList_Parameter("Hold").ToList();
            return View("VerifiedPlots", res);
        }
        public ActionResult Repurchased()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Repurchased Plots", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Repurchased Plots";
            var res = db.Sp_Get_PlotList_Parameter("Repurchased").ToList();
            return View("VerifiedPlots", res);
        }
        public ActionResult AvaiableForSalePlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Avaiable for Sales Plots", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Available for Sale Plots";
            ViewBag.Parameter = "Available_For_Sale";
            var res = db.Sp_Get_PlotList_Parameter("Available_For_Sale").ToList();
            return View("VerifiedPlots", res);
        }
        public ActionResult ContructedPlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Constructed Plots", "Create", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Constructed Plots";
            var res = db.Sp_Get_PlotList_Parameter("Constructed").ToList();
            return View("VerifiedPlots", res);
        }
        public ActionResult NonDevelopedPlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Page access of Non Developed Plots", "Create", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Title = "Non Developed Plots";
            var res = db.Sp_Get_PlotList_Parameter("Boundry").ToList();
            return View("VerifiedPlots", res);
        }
        public ActionResult PlotOthersOwnership()
        {
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            ViewBag.Premium_Id = new SelectList(db.PremiumPlots.ToList(), "Id", "Plot_Number");
            return View();
        }
        public JsonResult PlotOthersData(Premium_Ownership po)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            po.RecordAddedby = userid;
            db.Premium_Ownership.Add(po);
            db.SaveChanges();
            return Json(true);
        }
        public JsonResult SearchResult(string Text, int SearchOption)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Searched For " + Text + " In Plot Information", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), SearchOption);
            if (string.IsNullOrEmpty(Text))
            {
                return Json(false);
            }
            else
            {
                if (SearchOption == 1)
                {
                    var Texts = Text.Split(' ');
                    if (Texts.Count() == 1)
                    {
                        var res = db.Sp_Get_Plot_Search(SearchOption, Texts[0], null, null).ToList();
                        return Json(res);
                    }
                    else if (Texts.Count() == 2)
                    {
                        var Token = Texts[1].ToCharArray();
                        if (Token.Count() <= 4)
                        {
                            var res = db.Sp_Get_Plot_Search(SearchOption, Texts[0], Texts[1], null).ToList();
                            return Json(res);
                        }
                        else
                        {
                            var val = Texts[1].ToString().Split('-');
                            if (val.Count() == 1)
                            {
                                var res = db.Sp_Get_Plot_Search(SearchOption, Texts[0], null, Texts[1]).ToList();
                                return Json(res);
                            }
                            else
                            {
                                var res = db.Sp_Get_Plot_Search(SearchOption, Texts[0], Texts[1], null).ToList();
                                return Json(res);
                            }
                        }
                    }
                    else
                    {
                        var val = Texts[1].ToString().Split('-');
                        if (val.Count() == 1)
                        {
                            var Token = Texts[1].ToCharArray();
                            if (Token.Count() == 1)
                            {
                                var res = db.Sp_Get_Plot_Search(SearchOption, Texts[0], Texts[1], Texts[2]).ToList();
                                return Json(res);
                            }
                            else
                            {
                                var res = db.Sp_Get_Plot_Search(SearchOption, Texts[0], null, Texts[1] + " " + Texts[2]).ToList();
                                return Json(res);
                            }
                        }
                        else
                        {
                            var res = db.Sp_Get_Plot_Search(SearchOption, Texts[0], Texts[1], Texts[2]).ToList();
                            return Json(res);
                        }
                    }
                }
                else if (SearchOption == 8)
                {
                    var res = db.Sp_Get_Plot_Search(SearchOption, Text, null, null).ToList();
                    return Json(res);
                }
                else
                {
                    var res = db.Sp_Get_Plot_Search(SearchOption, Text, null, null).ToList();
                    return Json(res);
                }
            }
        }
        public ActionResult PlotUpStatus(long PlotId, string Status)
        {
            ViewBag.PlotId = PlotId;
            ViewBag.Status = Status;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PlotUpStatus1(string Remarks, long PlotId, string PlotStatus)
        {
            var userid = long.Parse(User.Identity.GetUserId());
            if (PlotStatus == PlotsStatus.Available_For_Sale.ToString())
            {
                var res = db.Sp_Get_PlotData(PlotId).FirstOrDefault();
                if (res.Status == PlotsStatus.Available_For_Sale.ToString())
                {
                    var res5 = new Return { Status = false, Msg = "Plot status cannot be changed from 'Registered' to 'Available for Sale'" };
                    return Json(res5);
                }
                db.Sp_Update_PlotStatus(PlotId, PlotsStatus.Available_For_Sale.ToString());
                db.Sp_Add_PlotComments(PlotId, "Updated to Avaialable for Sale. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                return Json(new Return { Status = true, Msg = "Plot status updated to 'Available for Sale' successfully" });
            }
            else if (PlotStatus == PlotsStatus.Temporary_Cancelled.ToString())
            {
                db.Sp_Update_PlotStatus(PlotId, PlotsStatus.Temporary_Cancelled.ToString());
                db.Sp_Add_Activity(userid, "Updated Status to Temporary Cancelled <a class='plt-data' data-id=' " + PlotId + "'>" + PlotId + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), PlotId);
                db.Sp_Add_PlotComments(PlotId, "Updated to Temporary Cancelled. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                return Json(new Return { Status = true, Msg = "Plot status updated to 'Temporary Cancelled' successfully" });
            }
            else if (PlotStatus == PlotsStatus.Cancelled.ToString())
            {
                var res = db.Plot_Cancelation_Req.Any(x => x.Plot_Id == PlotId && x.PlotsMang_Approval != true && x.FinancMang_Approval != true && x.Type == "Cancelled");
                if (res)
                {
                    return Json(new Return { Status = false, Msg = "Plot cancellation request already requested" });
                }
                var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
                var res2 = db.Sp_Get_PlotLastOwner(PlotId).SingleOrDefault();
                db.Sp_Update_PlotStatus(PlotId, PlotsStatus.Hold.ToString());
                db.Sp_Add_PlotCancelation_Req(PlotId, res2.Id, userid, Remarks, res1.Plot_No, res1.Block_Name, PlotsStatus.Cancelled.ToString());
                db.Sp_Add_Activity(userid, "Updated Status to Cancel Request <a class='plt-data' data-id=' " + PlotId + "'>" + PlotId + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), PlotId);
                db.Sp_Add_PlotComments(PlotId, "Requested to Cancel. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                return Json(new Return { Status = true, Msg = "Cancellation request for the plot has been submitted successfully" });
            }
            else if (PlotStatus == PlotsStatus.Disputed.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Disputed <a class='plt-data' data-id=' " + PlotId + "'>" + PlotId + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), PlotId);
                db.Sp_Update_PlotStatus(PlotId, PlotsStatus.Disputed.ToString());
                db.Sp_Add_PlotComments(PlotId, "Updated to Disputed. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                return Json(new Return { Status = true, Msg = "Plot status updated to 'Disputed' successfully" });
            }
            else if (PlotStatus == PlotsStatus.Hold.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Hold <a class='plt-data' data-id=' " + PlotId + "'>" + PlotId + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), PlotId);
                db.Sp_Update_PlotStatus(PlotId, PlotsStatus.Hold.ToString());
                db.Sp_Add_PlotComments(PlotId, "Updated to Hold. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                db.Sp_Update_PlotOwnershipStatus(PlotId, Ownership_Status.Cancelled.ToString());
                return Json(new Return { Status = true, Msg = "Plot status updated to 'Hold' successfully" });
            }
            else if (PlotStatus == PlotsStatus.Registered.ToString())
            {
                db.Sp_Add_Activity(userid, "Updated Status to Registered <a class='plt-data' data-id=' " + PlotId + "'>" + PlotId + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), PlotId);
                db.Sp_Update_PlotStatus(PlotId, PlotsStatus.Registered.ToString());
                db.Sp_Add_PlotComments(PlotId, "Updated to Registered. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                return Json(new Return { Status = true, Msg = "Plot status updated to 'Registered' successfully" });
            }
            else if (PlotStatus == PlotsStatus.Repurchased.ToString())
            {
                var res = db.Plot_Cancelation_Req.Any(x => x.Plot_Id == PlotId && x.PlotsMang_Approval != true && x.FinancMang_Approval != true && x.Type == "Repurchased");
                if (res)
                {
                    return Json(new Return { Status = false, Msg = "Plot repurchase request already requested" });
                }
                var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
                var res2 = db.Sp_Get_PlotLastOwner(PlotId).SingleOrDefault();
                db.Sp_Add_PlotCancelation_Req(PlotId, res2.Id, userid, Remarks, res1.Plot_No, res1.Block_Name, PlotsStatus.Repurchased.ToString());
                db.Sp_Add_PlotComments(PlotId, "Requested to Repurchased. \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                db.Sp_Add_Activity(userid, "Updated Status to Repurchased <a class='plt-data' data-id=' " + PlotId + "'>" + PlotId + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), PlotId);
                return Json(new Return { Status = true, Msg = "Repurchase request for the plot has been submitted successfully" });
            }
            else if (PlotStatus == "Repurchase")
            {
                db.Sp_Add_Activity(userid, "Updated Status to Repurchased <a class='plt-data' data-id=' " + PlotId + "'>" + PlotId + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), PlotId);
                var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
                db.Sp_Update_PlotStatus(PlotId, "Repurchase");
                db.Sp_Add_PlotComments(PlotId, "Updated Status to Repurchased . \n\r" + Remarks, userid, ActivityType.Plot_Status_Updation.ToString());
                return Json(true);
            }
            return Json(true);
        }
        public ActionResult TransferList()
        {
            var res = db.Sp_Get_TransferList_Plot().ToList();
            return View(res);
        }
        public ActionResult CancellationReqs()
        {
            if (User.IsInRole("Plots Manager"))
            {
                var res = db.Sp_Get_PlotCancelation_Req("Plots Manager").ToList();
                return View(res);
            }
            else if (User.IsInRole("Finance Manager"))
            {
                var res = db.Sp_Get_PlotCancelation_Req("Finance Manager").ToList();
                return View(res);
            }
            else
            {
                var res = db.Sp_Get_PlotCancelation_Req("Plots Manager").ToList();
                return View(res);
            }
        }
        public ActionResult PlotCancellationDetails(long Plotid, long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get full Details of Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>  ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            ViewBag.CanId = Id;
            var res7 = db.Plot_Cancelation_Req.SingleOrDefault(x => x.Id == Id);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var discounts = db.Discounts.Where(x => x.Module_Id == Plotid && x.Plot_Is_Cancelled == null && x.Module == "PlotManagement").ToList();
            UpdatePlotInstallmentStatus(res3, res4, discounts, Plotid);
            var res5 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res6 = db.Cancellation_Receipts.Where(x => x.File_Plot_No == Plotid).FirstOrDefault();
            var res = new PlotCancelDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res5, PlotReceipts = res4, Plot_Cancel = res6, Plot_Req = res7 };
            return View(res);
        }
        public JsonResult UpdateCancelStat(long Id, long PlotId, string Remarks, bool? Status, decimal? Deduction, decimal? Repurchase)
        {
            string[] ReceiptTypes = { "Booking", "Installment" };
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
            if (res1.Status != PlotsStatus.Registered.ToString() && res1.Status != PlotsStatus.Temporary_Cancelled.ToString() && res1.Status != PlotsStatus.Hold.ToString())
            {
                return Json(false);
            }
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).SingleOrDefault();
            var res3 = db.Sp_Get_ReceivedAmounts(PlotId, Modules.PlotManagement.ToString()).Where(x => ReceiptTypes.Contains(x.Type)).ToList();
            var res7 = db.Sp_Get_PlotInstallments(PlotId).Sum(x => x.Amount);
            var res6 = db.Plot_Cancelation_Req.SingleOrDefault(x => x.Id == Id).Type;
            var receivedamt = res3.Where(x => (x.Status == "Approved" || x.Status is null)).Sum(x => x.Amount);
            string desc = "";
            foreach (var item in res3)
            {
                desc = desc + item.ReceiptNo + ", ";
            }
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res4 = db.Sp_Add_Booking_Cancelation(receivedamt, res2.Mobile_1, res2.Father_Husband, res1.Id, res2.Name, res7, "Meher Estate Developers",
                                res1.Plot_Size, Cancellations.Plot_Cancellation.ToString(), userid, userid, desc, Modules.PlotManagement.ToString(),
                                res1.Plot_No + " - " + res1.Type, res1.Block_Name, Deduction, Repurchase, res6, res1.Type).FirstOrDefault();
                    db.Sp_Update_PlotCancelation_Req(Id, PlotId, Remarks, Status, "Plots Manager", userid);
                    Transaction.Commit();
                    var res5 = new { Status = "Requested", ReceiptNo = res4.Receipt_No, ReceiptId = res4.Receipt_Id, Token = userid };
                    return Json(res5);
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(false);
                }
            }
        }
        public ActionResult PlotCancelation()
        {
            var banks = db.Bank_Accounts.Select(x => new NameValuestring { Value = x.Id.ToString(), Name = x.Bank + " - " + x.Account_Number }).ToList();
            var all = new NameValuestring()
            {
                Name = "Cash",
                Value = "0"
            };
            banks.Add(all);
            ViewBag.payAccId = new SelectList(banks.OrderBy(x => x.Value), "Value", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }
        public JsonResult FinalPlotCancelation(bool? Status, int payAccId, long TransactionId, string instNo, DateTime? instDate, string branch, string payType, string Remarks, string Description, long Id, long PlotId, string Type, ReceiptData rd)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            long TransId = new Helpers().RandomNumber();
            var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();
            var res5 = db.Cancellation_Receipts.Where(x => x.File_Plot_No == res1.Id && x.Module == Modules.PlotManagement.ToString()).OrderByDescending(x => x.Id).FirstOrDefault();

            string Narration = "Against Cancellation of Plot no: " + res1.Plot_No + " Block No: " + res1.Block_Name + " Type :  " + res1.Type + " - " + Description;

            if (payAccId == 0)
            {
                payType = "Cash";
                PaymentTypeModel credAccount = ah.Payment_Head(payType, Transaction_Type.Credit.ToString(), null, comp.Id);
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var upd = db.Sp_Update_PlotCancelation_Req(Id, PlotId, Remarks, Status, "Finance Manager", userid);
                        //db.Sp_Update_FileCancelation_Req(Id, res1.Id, FileId, Remarks, Status, "Finance Manager", userid);
                        if (Type == "Cancelled")
                        {
                            db.Sp_Add_PlotComments(PlotId, "Plot Cancellation is Approved", userid, ActivityType.Plot_Status_Updation.ToString());
                        }
                        else
                        {
                            db.Sp_Add_PlotComments(PlotId, "Plot Repurchased is Approved ", userid, ActivityType.Plot_Status_Updation.ToString());
                        }

                        var res = db.Sp_Add_Voucher(string.Join(",", res2.Select(x => x.Postal_Address)), rd.Amount, rd.AmountInWords, rd.Bank, rd.Branch, rd.Ch_bk_Pay_Date, rd.PayChqNo, string.Join(",", res2.Select(x => x.Mobile_1)), "Against Cancellation of Plot no: " + res1.Plot_No + " Block No: " + res1.Block_Name + "`" + Description,
                        string.Join(",", res2.Select(x => x.Father_Husband)), res1.Id, Modules.PlotManagement.ToString(), string.Join(",", res2.Select(x => x.Name)), rd.PaymentType, "Meher Estate Developers",
                         "", userid, Payments.Cancellation.ToString(), userid, null, comp.Id).FirstOrDefault();


                        string Sales = "";
                        string Receivable = "";
                        if (res1.Type == PlotType.Commercial.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Commercial.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();

                        }
                        else if (res1.Type == PlotType.Residential.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Residential.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                        }

                        var Sales_COA = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Sales, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.BlockIden);
                        var Plot_Head = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Receivable, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.BlockIden);
                        var Cancellation_COA = ah.HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId

                        decimal? Remain = res5.Plot_Total_Amount - res5.Amount;
                        decimal? Payable = res5.Amount - res5.Deduction_Amt;

                        var Sales_Debit = db.Sp_Add_Journal_Entry(res5.Plot_Total_Amount, 0, Sales_COA.Text_ChartCode + " - " + Sales_COA.Head, Sales_COA.Id, Sales_COA.Text_ChartCode, Sales_COA.Head, Narration, TransactionId, 1, userid, userid, null, null, instNo, null, instDate, null, "", null, res.Receipt_No, credAccount.VoucherType, comp.Id).FirstOrDefault();
                        var Pay_credit = db.Sp_Add_Journal_Entry(0, Payable, credAccount.PaymentData.Text_ChartCode + " - " + credAccount.PaymentData.Head, credAccount.PaymentData.Id, credAccount.PaymentData.Text_ChartCode, credAccount.PaymentData.Head, Narration, TransactionId, 1, userid, userid, null, null, instNo, credAccount.PaymentStatus, instDate, null, "", null, res.Receipt_No, credAccount.VoucherType, comp.Id).FirstOrDefault();
                        var plot_Credit = db.Sp_Add_Journal_Entry(0, Remain, Plot_Head.Text_ChartCode + " - " + Plot_Head.Head, Plot_Head.Id, Plot_Head.Text_ChartCode, Plot_Head.Head, Narration, TransactionId, 1, userid, userid, null, null, instNo, credAccount.PaymentStatus, instDate, null, "", null, res.Receipt_No, credAccount.VoucherType, comp.Id).FirstOrDefault();
                        var canc_Credit = db.Sp_Add_Journal_Entry(0, res5.Deduction_Amt, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, Narration, TransactionId, 1, userid, userid, null, null, instNo, credAccount.PaymentStatus, instDate, null, "", null, res.Receipt_No, credAccount.VoucherType, comp.Id).FirstOrDefault();



                        if (Type == "Repurchased")
                        {
                            db.Sp_Update_PlotStatus(PlotId, PlotsStatus.Repurchased.ToString());
                            db.Sp_Add_PlotTransfer("", "Grand City", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", PlotId, "", null, "Owner", "", DateTime.Now, TransId).FirstOrDefault();
                        }

                        Transaction.Commit();
                        return Json(new { Status = true, Msg = "Payment Voucher Successful.", VoucherId = res.Receipt_Id, Token = TransactionId });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(new { Status = false, Msg = "Something went wrong" });
                    }
                }
            }
            else
            {
                var credAccount = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.PDC_Payable.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
                var bank = db.Bank_Accounts.Where(x => x.Id == payAccId).FirstOrDefault();

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var upd = db.Sp_Update_PlotCancelation_Req(Id, PlotId, Remarks, Status, "Finance Manager", userid);
                        //db.Sp_Update_FileCancelation_Req(Id, res1.Id, FileId, Remarks, Status, "Finance Manager", userid);
                        if (Type == "Cancelled")
                        {
                            db.Sp_Add_PlotComments(PlotId, "Plot Cancellation is Approved", userid, ActivityType.Plot_Status_Updation.ToString());
                        }
                        else
                        {
                            db.Sp_Add_PlotComments(PlotId, "Plot Repurchased is Approved ", userid, ActivityType.Plot_Status_Updation.ToString());
                        }

                        var res = db.Sp_Add_Voucher(string.Join(",", res2.Select(x => x.Postal_Address)), rd.Amount, rd.AmountInWords, bank.Bank, branch, instDate, instNo, string.Join(",", res2.Select(x => x.Mobile_1)), "Against Cancellation of Plot no: " + res1.Plot_No + " Block No: " + res1.Block_Name + "`" + Description,
                       string.Join(",", res2.Select(x => x.Father_Husband)), res1.Id, Modules.PlotManagement.ToString(), string.Join(",", res2.Select(x => x.Name)), rd.PaymentType, "Meher Estate Developers",
                        "", userid, Payments.Cancellation.ToString(), userid, null, comp.Id).FirstOrDefault();


                        string Sales = "";
                        string Receivable = "";
                        if (res1.Type == PlotType.Commercial.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Commercial.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();

                        }
                        else if (res1.Type == PlotType.Residential.ToString())
                        {
                            Sales = COA_Mapper_ModuleTypes.Plots_Sales_Residential.ToString();
                            Receivable = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                        }

                        var Sales_COA = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Sales, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.BlockIden);
                        var Plot_Head = ah.HeadFinder(COA_Mapper_Modules.Files_Plots.ToString(), Receivable, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, res1.BlockIden);
                        var Cancellation_COA = ah.HeadFinder("Files_Plots", "Other_Recovery", COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);//ModuleId will be ProjectId

                        decimal? Remain = res5.Plot_Total_Amount - res5.Amount;
                        decimal? Payable = res5.Amount - res5.Deduction_Amt;

                        var Sales_Debit = db.Sp_Add_Journal_Entry(res5.Plot_Total_Amount, 0, Sales_COA.Text_ChartCode + " - " + Sales_COA.Head, Sales_COA.Id, Sales_COA.Text_ChartCode, Sales_COA.Head, Narration, TransactionId, 1, userid, userid, null, bank.Bank, instNo, null, instDate, null, "", null, res.Receipt_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var Pay_credit = db.Sp_Add_Journal_Entry(0, Payable, credAccount.Text_ChartCode + " - " + credAccount.Head, credAccount.Id, credAccount.Text_ChartCode, credAccount.Head, Narration, TransactionId, 1, userid, userid, null, bank.Bank, instNo, null, instDate, null, "", null, res.Receipt_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var plot_Credit = db.Sp_Add_Journal_Entry(0, Remain, Plot_Head.Text_ChartCode + " - " + Plot_Head.Head, Plot_Head.Id, Plot_Head.Text_ChartCode, Plot_Head.Head, Narration, TransactionId, 1, userid, userid, null, bank.Bank, instNo, null, instDate, null, "", null, res.Receipt_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();
                        var canc_Credit = db.Sp_Add_Journal_Entry(0, res5.Deduction_Amt, Cancellation_COA.Text_ChartCode + " - " + Cancellation_COA.Head, Cancellation_COA.Id, Cancellation_COA.Text_ChartCode, Cancellation_COA.Head, Narration, TransactionId, 1, userid, userid, null, bank.Bank, instNo, null, instDate, null, "", null, res.Receipt_No, Voucher_Type.BPV.ToString(), comp.Id).FirstOrDefault();




                        if (Type == "Repurchased")
                        {
                            db.Sp_Update_FileStatus(res1.Id, "5");
                            //db.Sp_Add_FileTransfer("", "Grand City", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", res1.Id, 0, res1.Block_Id, "", res2.Rate, res2.Total, res2.GrandTotal, 0).FirstOrDefault();
                            db.Sp_Add_FileTransfer(0).FirstOrDefault();
                        }

                        var res8 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, bank.Bank, null, payType, bank.Bank, bank.Account_Number, PaymentMethodStatuses.Pending.ToString(),
                                Modules.Vendor_Payment.ToString(), "Payment Voucher", userid, instNo, PlotId, instDate, res1.Plot_No, res.Receipt_Id, comp.Id, Voucher_Type.BPV.ToString()).FirstOrDefault());

                        Transaction.Commit();
                        return Json(new { Status = true, Msg = "Payment Voucher Successful.", VoucherId = res.Receipt_Id, Token = TransactionId });
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(new { Status = false, Msg = "Error Occured" });
                    }
                }

            }
        }
        public ActionResult PlotsFile(long Id)
        {
            var res = db.Sp_Get_PlotData(Id).ToList();
            Helpers h = new Helpers(Modules.PlotManagement, Types.Booking);
            foreach (var item in res)
            {
                object[] data = new object[4];
                data[0] = item.Plot_No;
                data[1] = item.Block_Name;
                data[2] = item.Type;
                data[3] = item.Plot_Size;
                var QR_Data = h.GenerateQRCode(data);
            }
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Plot File Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }

        //create a function for generate Qr code of plots
        public ActionResult PlotsQrCode()
        {
            var plots = db.Plots.ToList();
            Helpers h = new Helpers(Modules.PlotManagement, Types.Booking);
            foreach (var item in plots)
            {
                object[] data = new object[4];
                data[0] = item.Plot_Number;
                data[1] = item.Block_Name;
                data[2] = item.Type;
                data[3] = item.Plot_Size;
                var QR_Data = h.GenerateQRCode(data);
            }
               return Json("ok", JsonRequestBehavior.AllowGet);
        }
        public ActionResult BlockFile(string Id)
        {
            var res = db.Sp_Get_PlotList_Block_Parameter(Id).Select(x => new Sp_Get_PlotData_Result
            {
                Area = x.Area,
                Block_Name = x.Block_Name,
                Id = x.Id,
                Develop_Status = x.Develop_Status,
                Dimension = x.Dimension,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                RatePerMarla_SqFt = x.RatePerMarla_SqFt,
                Road_Type = x.Road_Type,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verified
            }).ToList();
            Helpers h = new Helpers(Modules.PlotManagement, Types.Booking);
            foreach (var item in res)
            {
                object[] data = new object[4];
                data[0] = item.Plot_No;
                data[1] = item.Block_Name;
                data[2] = item.Type;
                data[3] = item.Plot_Size;
                var QR_Data = h.GenerateQRCode(data);
            }
            return View("PlotsFile", res);
        }
        /// Allotment Letters
        public ActionResult AllotmentTrackingViews()
        {
            var res = db.Sp_Get_AllotmentLetterTracking().ToList();
            return PartialView(res);
        }
        public JsonResult SignAllotmentLetter(long Id, long PlotId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetter(Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Mark Signed Allotment Letter with id: " + Id, "Update", Modules.PlotManagement.ToString(), ActivityType.Signed.ToString(), PlotId);
            db.Sp_Add_PlotComments(PlotId, "Mark Allotment Letter Signed of " + res.Name, userid, ActivityType.Signed.ToString());
            var res1 = db.Sp_Get_PlotDetailData(PlotId).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();
            var text = "Dear " + string.Join(",", res2.Select(x => x.Name)) + ",\n\r" +
                "Your Allotment Letter for Plot No: " + res1.Plot_No + " Block: " + res1.Block_Name + " is ready. Please collect your Allotment Letter. \n\r " +
                "Thank you for choosing Grand City. \n\r " +
                "For more information Dial: 111 724 786.";
            try
            {
                foreach (var item in res2)
                {
                    SmsService smsService = new SmsService();
                    smsService.SendMsg(text, item.Mobile_1);
                }
            }
            catch (Exception)
            {
            }
            db.Sp_Update_ALSign(Id, true);
            return Json(true);
        }
        // Un singed
        public JsonResult UnsingedAllotmentLetter(long Id, long PlotId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetter(Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Mark Un singed Allotment Letter with id: " + Id, "Update", Modules.PlotManagement.ToString(), ActivityType.UnSigned.ToString(), PlotId);
            db.Sp_Add_PlotComments(PlotId, "Mark Allotment Letter Unsigned of " + res.Name, userid, ActivityType.UnSigned.ToString());
            db.Sp_Update_ALSign(Id, false);
            return Json(true);
        }
        public ActionResult LetterInfomation(long PlotId, long Id)
        {
            ViewBag.PlotId = PlotId;
            ViewBag.Id = Id;
            return PartialView();
        }
        public JsonResult UpdateAllotmentLetterInfo(long Id, long PlotId, string Witness1, string Witness2)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).ToList();
            db.Sp_Add_Activity(userid, "Update Allotment Letter Information with id: " + Id, "Update", Modules.PlotManagement.ToString(), ActivityType.Record_Upatation.ToString(), PlotId);
            db.Sp_Add_PlotComments(PlotId, "Update the Allotment Letter Information", userid, ActivityType.Record_Upatation.ToString());
            string[] img = new string[6];
            for (int i = 0; i < res2.Count(); i++)
            {
                img[i] = res2[i].Owner_Img;
            }
            db.Sp_Update_AllotmentLetterInfo(Id, string.Join(",", res2.Select(x => x.Name)), string.Join(",", res2.Select(x => x.Father_Husband)), string.Join(",", res2.Select(x => x.Postal_Address)), res1.Plot_Size, res1.Dimension, res1.Area.ToString(), img[0], img[1], img[2], img[3], img[4], img[5], Witness1, Witness2);
            Helpers helpers = new Helpers(Modules.PlotManagement, Types.Plots);
            object[] data = new object[6];
            data[0] = string.Join(",", res2.Select(x => x.Name));
            data[1] = res1.Plot_No;
            data[2] = res1.Phase_Name;
            data[3] = res1.Block_Name;
            data[4] = res1.Plot_Size;
            data[5] = res1.Dimension;
            var QR_Data = helpers.GenerateQRCode(data);
            return Json(true);
        }
        public ActionResult AllotmentLetters()
        {
            var res = db.AllotmentLetters.OrderByDescending(x => x.Datetime).ToList();
            return View(res);
        }
        public ActionResult AllotmentLetterDelivery()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Visited Page of Allotment Letters List", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            return View();
        }
        public ActionResult PreAllotmentLetter()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = (from x in db.PremiumPlots
                        join y in db.Premium_Ownership on x.Id equals y.Premium_Id
                        select new PremiumAllotmentLet
                        {
                            Size = x.Plot_Size,
                            City = y.City,
                            CNIC_NICOP = y.CNIC_NICOP,
                            Father_Husband = y.Father_Husband,
                            Name = y.Name,
                            Owner_Id = y.Id,
                            Owner_Img = y.Owner_Img,
                            Owner_Img2 = y.Owner_Img2,
                            Owner_Img3 = y.Owner_Img3,
                            Owner_Img4 = y.Owner_Img4,
                            Postal_Address = y.Postal_Address
                        }).ToList();
            return View(res1);
        }
        public JsonResult AllotmentLetterStatus(long Id, int Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_PlotLastOwner(Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Request for Allotment Letter for Plot Id <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Allotment_Letter_Requested.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Request for Allotment Letter for : " + res.Name, userid, ActivityType.Allotment_Letter_Requested.ToString());
            db.Sp_Update_AllotmentLetterRequestStatus(res.Id, Status);
            return Json(true);
        }
        // For Possession Letter
        public JsonResult PossessionLetterStatus(long Id, int Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_PlotLastOwner(Id).ToList();
            db.Sp_Add_Activity(userid, "Request for Possession Letter for Plot Id <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Allotment_Letter_Requested.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Request for Possession Letter for : " + string.Join(",", res.Select(x => x.Name)), userid, ActivityType.Possession_Letter.ToString());
            db.Sp_Update_PossessionLetterRequestStatus(res.Select(x => x.GroupTag).FirstOrDefault(), Status, null);
            return Json(true);
        }
        public ActionResult PlotAllotmentReqAppr(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Approval of Allotment Letter for Plot Id <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var res6 = db.Discounts.Where(x => x.Module_Id == Plotid && x.Module == "PlotManagement").ToList();
            UpdatePlotInstallmentStatus(res3, res4, res6, Plotid);
            var res5 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res = new LatestPlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res5, PlotReceipts = res4 };
            return View(res);
        }
        public JsonResult UpdateDeliveryofAllotment(long Id, DateTime Delivery_Date)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.AllotmentLetters.SingleOrDefault(x => x.Id == Id);
            db.Sp_Add_Activity(userid, "Delivered Allotment Letter of owner", "Create", Modules.PlotManagement.ToString(), ActivityType.Allotment_Letter_Delivered.ToString(), Id);
            db.Sp_Add_PlotComments(res.Plot_Id, "Mark Allotment Letter Delivered of : " + res.Name, userid, ActivityType.Allotment_Letter_Delivered.ToString());
            int index = 0;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                string imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/DeliveredAllotmentLetters/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/DeliveredAllotmentLetters/"));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/DeliveredAllotmentLetters/"), res.PlotOwner_Id + imageName);
                hpf.SaveAs(savedFileName);
            }
            db.Sp_Update_AllotmentLetterDelivery(Id, Delivery_Date);
            return Json(true);
        }
        public JsonResult UpdateDeliveryofAllotmentPrevious(long Id, DateTime Delivered_Date)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Plot_Ownership.SingleOrDefault(x => x.Id == Id);
            db.Sp_Add_Activity(userid, "Delivered Allotment Letter of owner", "Create", Modules.PlotManagement.ToString(), ActivityType.Allotment_Letter_Delivered.ToString(), res.Plot_Id);
            db.Sp_Add_PlotComments(res.Plot_Id, "Mark Allotment Letter Delivered of : " + res.Name, userid, ActivityType.Allotment_Letter_Delivered.ToString());
            int index = 0;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                string imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/DeliveredAllotmentLetters/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/DeliveredAllotmentLetters/"));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/DeliveredAllotmentLetters/"), Id + imageName);
                hpf.SaveAs(savedFileName);
            }
            db.Sp_Update_AllotmentLetterDeliveryPre(Id, Delivered_Date);
            return Json(true);
        }
        public ActionResult AllotmentDuplicateLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var allot = db.AllotmentLetters.Where(x => x.PlotOwner_Id == Id).FirstOrDefault();
            var res = db.Sp_Get_AllotmentLetter(allot.Id).FirstOrDefault();
            db.Sp_Update_DuplicateAllotmentLetterCheck(res.PlotOwner_Id);
            ViewBag.Owners = db.Sp_Get_PlotLastOwner(res.Plot_Id).Select(x => x.Id).ToList();

            db.Sp_Add_Activity(userid, "Access Duplicate Allotment Letter of Plot No. " + res.Plot_No + " Block No: " + res.Block, "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            db.Sp_Add_PlotComments(allot.Plot_Id, "Generated Duplicate Allotment Letter for : " + res.Name, userid, ActivityType.Allotment_Letter_Generate.ToString());
            return View(res);
        }
        public ActionResult AllotmentLettersList()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Visited Page of Allotment Letters List", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            return View();
        }
        public ActionResult VerifiedAllotmentLetters()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetters("Verified").ToList();
            db.Sp_Add_Activity(userid, "Visited Page of Verified Allotment Letters List", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Page = "Verified";
            return PartialView(res);
        }
        public ActionResult RequestedAllotmentLetters()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetters("Requested").ToList();
            db.Sp_Add_Activity(userid, "Visited Page of Requested Allotment Letters List", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Page = "Requested";
            return PartialView("VerifiedAllotmentLetters", res);
        }
        public ActionResult SignedAllotmentLetters()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetters("Signed").ToList();
            db.Sp_Add_Activity(userid, "Visited Page of Signed Allotment Letters List", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Page = "Signed";
            return PartialView("VerifiedAllotmentLetters", res);
        }
        public ActionResult DeliveredAllotmentLetters()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetters("Delivered").ToList();
            db.Sp_Add_Activity(userid, "Visited Page of Delivered Allotment Letters List", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Page = "Delivered";
            return PartialView("VerifiedAllotmentLetters", res);
        }
        public JsonResult PlotAllotmentLetter(long Id, string Witness1, string Witness2, string Name, string Designation)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_PlotData(Id).SingleOrDefault();
            var plot = db.Plots.Where(x => x.Id == res1.Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Id).ToList();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Sp_Add_Activity(userid, "Generated a New Allotment Letter of Plot No." + res1.Plot_No + " Block:" + res1.Block_Name + " For :" + string.Join(",", res2.Select(x => x.Name)), "Create", Modules.PlotManagement.ToString(), ActivityType.Allotment_Letter_Generate.ToString(), Id);
                    db.Sp_Add_PlotComments(Id, "Generated a New Allotment Letter of " + string.Join(",", res2.Select(x => x.Name)), userid, ActivityType.Allotment_Letter_Generate.ToString());
                    string[] img = new string[6];
                    for (int i = 0; i < res2.Count(); i++)
                    {
                        img[i] = res2[i].Owner_Img;
                    }
                    var res = db.Sp_Add_AllotmentLetter(string.Join(",", res2.Select(x => x.Id)), string.Join(",", res2.Select(x => x.Name)), string.Join(",", res2.Select(x => x.Father_Husband)), string.Join(",", res2.Select(x => x.CNIC_NICOP)), string.Join(",", res2.Select(x => x.Postal_Address)), "Meher Estate Developers", res1.Phase_Name,
                        res1.Block_Name, plot.Plot_Number + " " + plot.Sector, res1.Plot_Size, res1.Type, res1.Area.ToString(), res1.Dimension, Witness1, Witness2, userid, Id, Convert.ToInt64(res2.Select(x => x.GroupTag).FirstOrDefault()), "1.jpg", img[1], img[2], img[3], img[4], img[5], Name, Designation).FirstOrDefault();
                    if (res == 0)
                    {
                        Transaction.Rollback();
                        var ret2 = new { Status = false, Msg = "Already Exists" };
                        return Json(ret2);
                    }
                    Helpers helpers = new Helpers(Modules.PlotManagement, Types.Plots);
                    object[] data = new object[6];
                    data[0] = string.Join(",", res2.Select(x => x.Name));
                    data[1] = plot.Plot_Number + " " + plot.Sector;
                    data[2] = res1.Block_Name;
                    data[3] = res1.Block_Name;
                    data[4] = res1.Plot_Size;
                    data[5] = res1.Dimension;
                    var QR_Data = helpers.GenerateQRCode(data);
                    Transaction.Commit();
                    return Json(new { Status = true, Id = res });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    var ret2 = new { Status = false, Msg = "Error Occured" };
                    return Json(ret2);
                }
            }
        }
        public JsonResult SpecialAllotmentLetter(long Id, string Witness1, string Witness2, string Name, string Designation)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_PlotDetailData(Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Generated a New Allotment Letter of Plot No." + res1.Plot_No + " Block:" + res1.Block_Name + " For : Meher Estate Developers", "Create", Modules.PlotManagement.ToString(), ActivityType.Allotment_Letter_Generate.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Generated a New Allotment Letter of Grand City", userid, ActivityType.Allotment_Letter_Generate.ToString());
            //db.Sp_Update_AllotmentLetter(res2.Id);
            var res = db.Sp_Add_AllotmentLetter(null, "SA Gardnes", "CEO", "-", "Grand City", res1.Project_Name, res1.Phase_Name,
                res1.Block_Name, res1.Plot_No, res1.Plot_Size, res1.Type, res1.Area.ToString(), res1.Dimension, Witness1, Witness2, userid, res1.Id, null, null, null, null, null, null, null, Name, Designation).FirstOrDefault();
            Helpers helpers = new Helpers(Modules.PlotManagement, Types.Plots);
            object[] data = new object[6];
            data[0] = "SA Gardnes";
            data[1] = res1.Plot_No;
            data[2] = res1.Phase_Name;
            data[3] = res1.Block_Name;
            data[4] = res1.Plot_Size;
            data[5] = res1.Dimension;
            var QR_Data = helpers.GenerateQRCode(data);
            return Json(res);
        }
        public ActionResult AllotmentLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetter(Id).SingleOrDefault();
            ViewBag.Owners = db.Sp_Get_PlotLastOwner(res.Plot_Id).Select(x => x.Id).ToList();
            var a = db.Sp_Add_Activity(userid, "Access Allotment Letter of Plot No. " + res.Plot_No + " Block No: " + res.Block, "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }
        public ActionResult Special_AllotmentLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetter(Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Access Allotment Letter of Plot No. " + res.Plot_No + " Block No: " + res.Block, "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }
        public ActionResult AllotmentLetterView(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_AllotmentLetter(Id).SingleOrDefault();
            db.Sp_Add_Activity(userid, "Access Allotment Letter of Plot No. " + res.Plot_No + " Block No: " + res.Block, "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Id);
            return View(res);
        }
        public ActionResult AllotmentRequest()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Allotment Letter Request", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            var res = db.Sp_Get_AllotmentLetterRequests().ToList();
            return View(res);
        }
        // Possession Request
        public ActionResult PossessionRequests()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Possession Letter Request", "Read", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            return View();
        }
        public ActionResult PossessionList(int type)
        {
            var res = db.Sp_Get_PossessionLetterRequests(type).ToList();
            return PartialView(res);
        }
        // For Possession
        public ActionResult GetPlotPossessionRequest(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Approval of possession Letter for Plot Id <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            var res3 = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            Helpers helpers = new Helpers(Modules.PlotManagement, Types.DealerRegister);
            object[] data = new object[6];
            data[0] = res3.OrderBy(x => x.Ownership_DateTime).Select(x => x.Name).FirstOrDefault();
            data[1] = res1.Plot_No.ToString();
            data[2] = res1.Phase_Name;
            data[3] = res1.Block_Name;
            data[4] = res1.Plot_Size;
            data[5] = res1.Dimension;
            var QR_Data = helpers.GenerateQRCode(data);
            var res = new PlotDetailData { PlotData = res1, PlotOwners = res2 };
            return View(res);
        }
        public ActionResult PlotBoundTabular(long Plot_Id)
        {
            List<Plot> plist = new List<Plot>();
            var res1 = db.Sp_Get_PlotData(Plot_Id).SingleOrDefault();
            var res3 = db.Plots.Where(x => x.Id == Plot_Id).FirstOrDefault();
            if (res3.Front != null)
            {
                Plot p = new Plot
                {
                    Id = res3.Id,
                    North = res3.North,
                    South = res3.South,
                    East = res3.East,
                    West = res3.West,
                    North_East = res3.North_East,
                    North_West = res3.North_West,
                    South_East = res3.South_East,
                    South_West = res3.South_West,
                    Front = res3.Front,
                };
                plist.Add(p);
            }
            else
            {
                Plot p = new Plot
                {
                    Id = res1.Id,
                    North = res1.Front_Side,
                    South = res1.Back_Side,
                    East = res1.Right_Side,
                    West = res1.Left_Side,
                    North_East = res1.North_East,
                    North_West = res1.North_West,
                    South_East = res1.South_East,
                    South_West = res1.South_West,
                    Front = "North",
                };
                plist.Add(p);
            }
            var res5 = db.Plot_BondedBy.Where(x => x.Plot_Id == Plot_Id).ToList();
            var res2 = db.Sp_Get_PlotLastOwner(Plot_Id).ToList();
            var res = new LatestPlotDetailData { PlotData = res1, PlotOwners = res2, PlotPosition = plist, PlotBounding = res5 };
            return PartialView(res);
        }
        public JsonResult DeleteBounding(long Id)
        {
            db.Sp_delete_PlotBounding(Id);
            return Json(true);
        }
        //public JsonResult PlotBounding(Plot_BondedBy B)
        //{
        //    var plot = db.Plots.Where(x=>x.Id==B.Plot_Id).SingleOrDefault();
        //    if (plot.Front == B.Position)
        //    {
        //        var res1 = new { Status = false, Msg = "Plot Front Cannot be Bound" };
        //        return Json(res1);
        //    }
        //    if (B.Plot_Id == B.BoundedPlotId)
        //    {
        //        var res1 = new { Status = false, Msg = "Same Plot Cannot be Bound" };
        //        return Json(res1);
        //    }
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var OfficialPlot = db.Sp_Get_PlotData(B.Plot_Id).SingleOrDefault();
        //    var BoundedPlot = db.Sp_Get_PlotData(B.BoundedPlotId).SingleOrDefault();
        //    var res= db.Sp_Update_PlotBounding(B.Plot_Id, B.BoundedPlotId, B.Block, OfficialPlot.Plot_No, BoundedPlot.Plot_No, B.Position,B.Block_Name,B.Plot_Type).FirstOrDefault();
        //    if (res != 0)
        //    {
        //        var res1 = new { Status = true, Msg = "Plot Bounded Successfully" };
        //        return Json(res1);
        //    }
        //    else
        //    {
        //        var res1 = new { Status = false, Msg = "Plot cannot be bounded more than once" };
        //        return Json(res1);
        //    }
        //}
        public JsonResult PlotBounding(Plot_BondedBy B, bool? isRoad)
        {
            var plot = db.Plots.Where(x => x.Id == B.Plot_Id).SingleOrDefault();
            if ((isRoad == false || isRoad is null))
            {
                if (plot.Front == B.Position)
                {
                    var res1 = new { Status = false, Msg = "The front of the plot cannot be bounded" };
                    return Json(res1);
                }
            }
            if (B.Plot_Id == B.BoundedPlotId)
            {
                var res1 = new { Status = false, Msg = "The same plot cannot be bound" };
                return Json(res1);
            }
            long userid = long.Parse(User.Identity.GetUserId());
            var OfficialPlot = db.Sp_Get_PlotData(B.Plot_Id).SingleOrDefault();
            string boundedPlt = string.Empty;
            if (isRoad is null || isRoad is false)
            {
                var BoundedPlot = db.Sp_Get_PlotData(B.BoundedPlotId).SingleOrDefault();
                boundedPlt = BoundedPlot.Plot_No;
            }
            else
            {
                if (B.BoundedPlotId.ToString().Contains("-999"))
                {
                    boundedPlt = "Boundary Wall";
                }
                else if (B.BoundedPlotId.ToString().Contains("-997"))
                {
                    boundedPlt = "Public Building";
                }
                else if (B.BoundedPlotId.ToString().Contains("-996"))
                {
                    boundedPlt = "Park";
                }
                else if (B.BoundedPlotId.ToString().Contains("-1000"))
                {
                    boundedPlt = "Degh";
                }
                else
                {
                    boundedPlt = "Road " + B.BoundedPlotId + "'";
                }
            }
            var res = db.Sp_Update_PlotBounding(B.Plot_Id, B.BoundedPlotId, B.Block, ((OfficialPlot == null) ? "" : OfficialPlot.Plot_No), boundedPlt, B.Position, B.Block_Name, (B.Plot_Type == "0") ? string.Empty : B.Plot_Type).FirstOrDefault();
            if (res != 0)
            {
                var res1 = new { Status = true, Msg = "Plot Bounded Successfully" };
                return Json(res1);
            }
            else
            {
                var res1 = new { Status = false, Msg = "Plot cannot be bounded more than once" };
                return Json(res1);
            }
        }
        public ActionResult PlotPosition(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res33 = db.Plots.Where(x => x.Id == Plotid).FirstOrDefault();
            var ammendedBlocks = db.Sp_Get_Block().ToList();
            ammendedBlocks.Add(new Sp_Get_Block_Result { Block_Name = "Public Building", Id = -997, Phase_Id = 0, Phase_Name = "", Project_Id = 0, Project_Name = "" });
            ammendedBlocks.Add(new Sp_Get_Block_Result { Block_Name = "Park", Id = -996, Phase_Id = 0, Phase_Name = "", Project_Id = 0, Project_Name = "" });
            ammendedBlocks.Add(new Sp_Get_Block_Result { Block_Name = "Road", Id = -998, Phase_Id = 0, Phase_Name = "", Project_Id = 0, Project_Name = "" });
            ammendedBlocks.Add(new Sp_Get_Block_Result { Block_Name = "Boundary Wall", Id = -999, Phase_Id = 0, Phase_Name = "", Project_Id = 0, Project_Name = "" });
            ammendedBlocks.Add(new Sp_Get_Block_Result { Block_Name = "Degh", Id = -1000, Phase_Id = 0, Phase_Name = "", Project_Id = 0, Project_Name = "" });
            ViewBag.Block = new SelectList(ammendedBlocks, "Id", "Block_Name");
            if (res33.Front == null)
            {
                Plots_Category res1 = (from pe in db.Plots
                                       join y in db.Plots_Category on pe.Plot_Category equals y.Id
                                       where pe.Id == Plotid
                                       select y).SingleOrDefault();
                var a = db.Sp_Update_PlotsPositions(Plotid, res1.Front_Side, res1.Back_Side, res1.Right_Side, res1.Left_Side, res1.North_East, res1.North_West, res1.South_West, res1.South_East, "North");
                db.SaveChanges();
                //Plot res2 = (from pe in db.Plots
                //                       where pe.Id == Plotid
                //                       select pe).FirstOrDefault();
                var res2 = db.Sp_Get_PlotRecord_Parameter(Plotid).SingleOrDefault();
                var res = new LatestPlotDetailData { PlotsData = res2 };
                return PartialView(res);
            }
            else
            {
                var res2 = db.Sp_Get_PlotRecord_Parameter(Plotid).SingleOrDefault();
                var res = new LatestPlotDetailData { PlotsData = res2 };
                return PartialView(res);
            }
        }
        //public ActionResult PlotPosition(long Plotid)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res33 = db.Plots.Where(x => x.Id == Plotid).FirstOrDefault();
        //    ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
        //    if(res33.Front ==null)
        //    {
        //        Plots_Category res1 = (from pe in db.Plots
        //                               join y in db.Plots_Category on pe.Plot_Category equals y.Id
        //                               where pe.Id == Plotid
        //                               select y).SingleOrDefault();
        //        var a= db.Sp_Update_PlotsPositions(Plotid, res1.Front_Side, res1.Back_Side, res1.Right_Side, res1.Left_Side, res1.North_East, res1.North_West, res1.South_West, res1.South_East, "North");
        //        db.SaveChanges();
        //        //Plot res2 = (from pe in db.Plots
        //        //                       where pe.Id == Plotid
        //        //                       select pe).FirstOrDefault();
        //        var res2 = db.Sp_Get_PlotRecord_Parameter(Plotid).SingleOrDefault();
        //        var res = new LatestPlotDetailData {  PlotsData = res2 };
        //        return PartialView(res);
        //    }
        //    else
        //    {
        //        var res2 = db.Sp_Get_PlotRecord_Parameter(Plotid).SingleOrDefault();
        //        var res = new LatestPlotDetailData {  PlotsData = res2 };
        //        return PartialView(res);
        //    }
        //}
        public JsonResult PlotPositionUpdation(Plot p)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            if (p.Front == "" || p.Front == null)
            {
                p.Front = "North";
            }
            db.Sp_Update_PlotsPositions(p.Id, p.North, p.South, p.East, p.West, p.North_East, p.North_West, p.South_West, p.South_East, p.Front);
            return Json(true);
        }
        public ActionResult PlotBounded(long Plot_Id)
        {
            List<Plot> plist = new List<Plot>();
            var res1 = db.Sp_Get_PlotData(Plot_Id).SingleOrDefault();
            var res3 = db.Plots.Where(x => x.Id == Plot_Id).FirstOrDefault();
            if (res3.Front != null)
            {
                Plot p = new Plot
                {
                    Id = res3.Id,
                    North = res3.North,
                    South = res3.South,
                    East = res3.East,
                    West = res3.West,
                    North_East = res3.North_East,
                    North_West = res3.North_West,
                    South_East = res3.South_East,
                    South_West = res3.South_West,
                    Front = res3.Front,
                };
                plist.Add(p);
            }
            else
            {
                Plot p = new Plot
                {
                    Id = res1.Id,
                    North = res1.Front_Side,
                    South = res1.Back_Side,
                    East = res1.Right_Side,
                    West = res1.Left_Side,
                    North_East = res1.North_East,
                    North_West = res1.North_West,
                    South_East = res1.South_East,
                    South_West = res1.South_West,
                    Front = "North",
                };
                plist.Add(p);
            }
            var res5 = db.Plot_BondedBy.Where(x => x.Plot_Id == Plot_Id).ToList();
            var res = new LatestPlotDetailData { PlotPosition = plist, PlotBounding = res5 };
            return PartialView(res);
        }
        // possession status
        // 1 for request
        // 2 for in process and image upload 
        // 3 for generate letter and finalize this and possession request in plotes will be 1
        [HttpPost]
        public JsonResult PossessionGenerate(long Plot_id, string Check, string mat_altd_plt)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var path = "";
            var ow = db.Sp_Get_PlotLastOwner(Plot_id).ToList();
            var plotdata = db.Sp_Get_PlotData(Plot_id).FirstOrDefault();
            if (Check != null)
            {
                db.Sp_Update_PossessionLetterRequestStatus(ow.FirstOrDefault().GroupTag, 3, mat_altd_plt);
                path = "~/Repository/PlotsData/" + Plot_id + "/DesignHouse/";
            }
            else
            {
                db.Sp_Update_PossessionLetterRequestStatus(ow.FirstOrDefault().GroupTag, 2, mat_altd_plt);
                path = "~/Repository/PlotsData/" + Plot_id + "/SitePlan/";
            }
            if (plotdata.Type == "Residential")
            {
                var res1 = Convert.ToBoolean(db.Sp_Add_SubscribeServiceCharges(Plot_id, 30, "Service Charges").FirstOrDefault());
            }
            else
            {
                var res1 = Convert.ToBoolean(db.Sp_Add_SubscribeServiceCharges(Plot_id, 31, "Service Charges").FirstOrDefault());
            }
            var a = db.Sp_Update_PlotConstructionStatus(Plot_id, "Constructed");
            int index = 0;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath(path + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath(path + "/"));
                }
                string savedFileName = Path.Combine(Server.MapPath(path + "/"), Convert.ToString(Plot_id) + ".png");
                hpf.SaveAs(savedFileName);
            }
            if (Check != null)
            {
                return Json(Plot_id);
            }
            else
            {
                return Json(true);
            }
        }
        public ActionResult PossessionPrint(long PlotId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Plots.Where(x => x.Id == PlotId).FirstOrDefault();
            var res2 = db.Plot_BondedBy.Where(x => x.Plot_Id == PlotId).ToList();
            var res3 = db.Sp_Get_PlotLastOwner(PlotId).ToList();
            var res5 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
            var res6 = db.Sp_Update_PossessionDate(res3.FirstOrDefault().GroupTag);
            //if(res3.Allotted_Mat_Plot_Name is null)
            //{
            //    db.SP_Update_Plot_AlottedForMaterial_ForPosession(altdPlt, res3.Id);
            //    res3.Allotted_Mat_Plot_Name = altdPlt;
            //}
            db.Sp_Add_PlotComments(PlotId, "Generated a New Possession Letter of " + string.Join(",", res3.Select(x => x.Name)), userid, ActivityType.Possession_Letter_Generated.ToString());
            var res = new LatestPlotDetailData { PlotData = res5, Plots = res1, PlotBounding = res2, PlotOwners = res3, };
            return View(res);
        }
        public ActionResult NewAllotmentLetters()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }
        public ActionResult NewOwnerAllotmentLetter(long Plotid)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page for Requesting New Allotment Letter", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var res5 = db.Discounts.Where(x => x.Module_Id == Plotid && x.Module == Modules.PlotManagement.ToString()).ToList();
            var res = new LatestPlotDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res3, PlotReceipts = res4, Discounts = res5 };
            return PartialView(res);
        }
        public JsonResult VerificationforAllotment(long Id, long PlotOwnId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Verify Plot Id  <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>  for Allotment Letter", "Update", Modules.PlotManagement.ToString(), ActivityType.Plot_Verified.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Verify Plot", userid, ActivityType.Plot_Verified.ToString());
            db.Sp_Update_VerifiedforAllotment(Id, PlotOwnId);
            return Json(true);
        }
        public ActionResult GenerateAllotLetterList()
        {
            var res = db.Sp_Get_VerifiedforAllotmentLetters().ToList();
            return View(res);
        }
        public ActionResult AddPlots()
        {
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects(), "Id", "Project_Name");
            var plots = db.Sp_Get_Plots_Size().Select(x => new { Plot = x }).ToList();
            ViewBag.Plots = new SelectList(plots.Distinct(), "Plot", "Plot");
            ViewBag.Dimension = new SelectList(db.Sp_Get_PlotsCategory(), "Id", "Dimension");
            return View();
        }
        public JsonResult Addplot(Plot plot, int DimensionId)
        {
            var size = db.Plots_Category.Where(x => x.Id == DimensionId).Select(x => x.Plot_Size).SingleOrDefault();
            var phase = db.RealEstate_Blocks.Where(x => x.Id == plot.Block_Id).Select(x => x.Phase_Id).SingleOrDefault();
            var res1 = db.Plots.Any(x => x.Block_Id == plot.Block_Id && x.Plot_Number == plot.Plot_Number && x.Sector == plot.Sector && x.Plot_Location == plot.Plot_Location && x.Road_Type == plot.Road_Type && x.Type == plot.Type);
            if (res1)
            {
                return Json(false);
            }
            else
            {
                plot.Plot_Size = string.Format("{0:n0}", size) + " Marla";
                plot.Plot_Category = DimensionId;
                plot.Phase_Id = phase;
                db.Plots.Add(plot);
                db.SaveChanges();
                return Json(true);
            }
        }
        /// Plots Surplus Amount
        /// 
        public ActionResult ExtraAmountRefund()
        {
            if (User.IsInRole("Plots Manager"))
            {
                var res = db.Sp_Get_PlotReceiptRefund_Req("Plots Manager", Modules.PlotManagement.ToString()).ToList();
                return View(res);
            }
            else if (User.IsInRole("Finance Manager"))
            {
                var res = db.Sp_Get_PlotReceiptRefund_Req("Finance Manager", Modules.PlotManagement.ToString()).ToList();
                return View(res);
            }
            else
            {
                var res = db.Sp_Get_PlotReceiptRefund_Req("Plots Manager", Modules.PlotManagement.ToString()).ToList();
                return View(res);
            }
        }
        public ActionResult PlotReceiptRefundDetails(long Plotid, long ReqId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Get full Details of Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>  ", "Read", Modules.PlotManagement.ToString(), ActivityType.Details_Access.ToString(), Plotid);
            var res6 = db.RefundAmountsReqs.Where(x => x.Id == ReqId).FirstOrDefault();
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotOwnerList(Plotid).ToList();
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var discount = db.Discounts.Where(x => x.Module_Id == Plotid && x.Module == "PlotManagement").ToList();
            UpdatePlotInstallmentStatus(res3, res4, discount, Plotid);
            var res5 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res = new PlotRefundDetailData { PlotData = res1, PlotOwners = res2, PlotInstallments = res5, PlotReceipts = res4, Plot_Req = res6 };
            return View(res);
        }
        public JsonResult ApproveRefund(long Id)
        {
            db.Sp_Update_PlotRefund_Req(Id, "Approve", "Plots Manager");
            return Json(true);
        }
        public ActionResult RefundAmount(long Id)
        {
            var res = db.RefundAmountsReqs.Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.RefundAmount = res.Amount;
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView();
        }
        [ValidateAntiForgeryToken]
        public JsonResult FinalRefundAmount(ReceiptData rd, string Status, string Remarks, string Description, long Id, long PlotId, string Type, long ReceiptId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var res1 = db.Sp_Get_PlotData(PlotId).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(PlotId).SingleOrDefault();
            var res3 = db.Sp_Get_Receipt_Parameter_ById(ReceiptId).FirstOrDefault();
            db.Sp_Update_PlotRefund_Req(Id, Status, "Finance Manager");
            var TransactionId = new Helpers().RandomNumber();
            var res = db.Sp_Add_Voucher(res2.Postal_Address, rd.Amount, rd.AmountInWords, rd.Bank, rd.Branch, rd.Ch_bk_Pay_Date, rd.PayChqNo, res2.Mobile_1, "Refund Amount against the " + res3.PaymentType + " Receipt No " + res3.ReceiptNo + "  of Plot no: " + res1.Plot_No + " Block No: " + res1.Block_Name + "`" + Description,
                res2.Father_Husband, res1.Id, Modules.PlotManagement.ToString(), res2.Name, rd.PaymentType, "Meher Estate Developers",
                 "", TransactionId, Payments.Receipt_Refund.ToString(), userid, null, comp.Id).FirstOrDefault();
            bool headcashier = false;
            if (User.IsInRole("Head Cashier"))
            {
                headcashier = true;
            }
            try
            {
                AccountHandlerController de = new AccountHandlerController();
                de.Refund_Plot_Amount(rd.Amount, res1.Plot_No, res1.Type, res1.Block_Name, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res.Receipt_No, 1, headcashier, COA_Mapper_Modules.Files_Plots.ToString(), res1.BlockIden);
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            }
            if (rd.PaymentType != "Cash")
            {
                if (!Directory.Exists(Server.MapPath("~/Repository/Receipt_Refund_Voucher/" + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/Receipt_Refund_Voucher/"));
                }
                string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                var pathMain = Path.Combine(Server.MapPath("~/Repository/Receipt_Refund_Voucher/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                Helpers h = new Helpers();
                var Images = h.SaveBase64Image(rd.FileImage, pathMain, res.ToString());
            }
            db.Sp_Update_ReceiptCancel_Parameter(ReceiptId, Description);
            var fres = new { VoucherId = res.Receipt_Id, Token = userid };
            return Json(fres);
        }
        ////////////////////
        //   OVerDue Files
        ////////////////////
        public ActionResult OverDuePlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Overdue Plots List Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        public ActionResult OverDuePlotsReport()
        {
            var res1 = db.Sp_Get_Report_OverDue_Plots().ToList();
            var res2 = db.Sp_Get_CancelPlotsReport().ToList();
            foreach (var item in res2)
            {
                item.Deduction = (item.Total * 10) / 100;
                item.Payable = (item.Received_Amount - item.Deduction);
            }
            var res = new Overdue_CancelReport_Plots { CancelPlots = res2, OverduePlots = res1 };
            return PartialView(res);
        }
        public ActionResult QualifyingPlots()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Qualifying plots List Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            var res = db.Sp_Get_OverdueQualifying_Plots().Select(x => new OverdueQualifying_Plots
            {
                Block_Name = x.Block_Name,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.Balance_Amount,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                Road_Type = x.Road_Type,
                Status = x.STATUS,
                Postal_Address = x.Postal_Address,
                Type = x.Type,
                Verified = x.Verified,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                Installments = x.Installments
            }).ToList();
            return PartialView(res);
        }
        public ActionResult FirstWarning(Search_OverDue s)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed First Warning Plots List Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            var res = db.Sp_Get_FirstWarning_Plot().Select(x => new OverdueQualifying_Plots
            {
                Block_Name = x.Block_Name,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.Balance_Amount,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                Road_Type = x.Road_Type,
                Status = x.STATUS,
                Type = x.Type,
                Verified = x.Verified,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                Installments = x.Installments,
                FirstNotice = x.First_Notice,
                Postal_Address = x.Postal_Address
            }).ToList();
            return PartialView(res);
        }
        public ActionResult SecWarning(Search_OverDue s)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Second Warning Plots List Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            var res = db.Sp_Get_SecWarning_Plot().Select(x => new OverdueQualifying_Plots
            {
                Block_Name = x.Block_Name,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.Balance_Amount,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                Road_Type = x.Road_Type,
                Status = x.STATUS,
                Type = x.Type,
                Verified = x.Verified,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                Installments = x.Installments,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                Postal_Address = x.Postal_Address
            }).ToList();
            return PartialView(res);
        }
        public ActionResult CancelledPlotsLetter(Search_OverDue s)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Cancelled Plots List Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            var res = db.Sp_Get_Cancellation_Plot().Select(x => new OverdueQualifying_Plots
            {
                Block_Name = x.Block_Name,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.Balance_Amount,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                Road_Type = x.Road_Type,
                Status = x.STATUS,
                Type = x.Type,
                Verified = x.Verified,
                Mobile_1 = x.Mobile_1,
                Postal_Address = x.Postal_Address,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                Installments = x.Installments,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice
            }).ToList();
            return PartialView(res);
        }
        public JsonResult WarningIssuesPlotsMove(long Id, long OwnerId, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Update_WarningLetterStatus_PlotMove(Id, OwnerId, Type);
            db.Sp_Add_PlotComments(Id, "Update the Letter Status to " + Type, userid, ActivityType.Plot_Status_Updation.ToString());

            return Json(true);
        }
        public JsonResult WarningIssues(long Id, long OwnerId, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_WarningLetterStatus_Plot(Id, OwnerId, Type);
            var res = db.Sp_Get_OverdueQualifying_Plots_Id(Id).Select(x => new OverdueQualifying_Plots
            {
                Area = x.Area,
                Block_Name = x.Block_Name,
                Develop_Status = x.Develop_Status,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.Balance_Amount,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                Road_Type = x.Road_Type,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verified,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice
            }).FirstOrDefault();
            if (Type == "First")
            {
                db.Sp_Add_Activity(userid, "Issued First Warning Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_PlotComments(Id, "Issued First Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                var text =
                "Respected Customer,\n\r" +
                "Kindly note last date for the submission of your Plot no:" + res.Plot_No + "-" + res.Type + " - " + res.Block_Name + " instalment has passed.You are requested to make payment within next 7 days’ time.Failing to do so will be resulting in qualification for cancellation.\n\r" +
                "Therefore; Submit your dues timely to ensure the safety of your plot.\n\r" +
                "Best Regards,\n\r" +
                "Grand City.\n\r" +
                "042 – 111 724 786\n\r";
                SmsService s = new SmsService();
                s.SendMsg(text, res.Mobile_1);
            }
            else if (Type == "Second")
            {
                db.Sp_Add_Activity(userid, "Issued Second Warning Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_PlotComments(Id, "Issued Second Warning Letter", userid, ActivityType.Warning_Letter.ToString());
                var text =
              "Respected Customer,\n\r" +
              "This is to inform you that your installment is still pending against your plot no:" + res.Plot_No + "-" + res.Type + " - " + res.Block_Name + ". A reminder message was also sent to you on (" + string.Format("{0:dd-MMM-yyyy}", res.FirstNotice) + ") along with a letter. You are requested to submit due instalments within next 7 days, otherwise your plot will be cancelled.\n\r" +
              "Best Regards,\n\r" +
              "Grand City.\n\r" +
              "042 – 111 724 786\n\r";
                SmsService s = new SmsService();
                s.SendMsg(text, res.Mobile_1);
            }
            else if (Type == "Last")
            {
                db.Sp_Add_Activity(userid, "Issued Cancellation Letter <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
                db.Sp_Add_PlotComments(Id, "Issued Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
                var text =
           "Respected Customer,\n\r" +
           "This is to remind you that your plot no:" + res.Plot_No + "-" + res.Type + " - " + res.Block_Name + " has been cancelled, because of ample amount due on your side. It has been reminded two times before but despite these reminders, you haven't paid your remaining amount.\n\r" +
           "You are kindly requested to visit our Head Office and submit your complete documents for the processing of your refund.\n\r" +
           "Best Regards,\n\r" +
           "Grand City.\n\r" +
           "042 – 111 724 786\n\r";
                SmsService s = new SmsService();
                s.SendMsg(text, res.Mobile_1);
            }
            return Json(true);
        }
        public ActionResult FirstWarningLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print First Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            var res = db.Sp_Get_OverdueQualifying_Plots_Id(Id).Select(x => new OverdueQualifying_Plots
            {
                Area = x.Area,
                Block_Name = x.Block_Name,
                Develop_Status = x.Develop_Status,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.Balance_Amount,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                Road_Type = x.Road_Type,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verified,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice,
                Postal_Address = x.Postal_Address,
                Installments = x.Installments
            }).FirstOrDefault();
            return View(res);
        }
        public ActionResult SecondWarningLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print Second Warning Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Print Second Warning Letter", userid, ActivityType.Warning_Letter.ToString());
            var res = db.Sp_Get_OverdueQualifying_Plots_Id(Id).Select(x => new OverdueQualifying_Plots
            {
                Area = x.Area,
                Block_Name = x.Block_Name,
                Develop_Status = x.Develop_Status,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.Balance_Amount,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                Road_Type = x.Road_Type,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verified,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice,
                Installments = x.Installments
            }).FirstOrDefault();
            return View(res);
        }
        public ActionResult CancellationLetter(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Print Cancellation Letter of File <a class='plt-data' data-id=' " + Id + "'>" + Id + "</a>", "Create", Modules.PlotManagement.ToString(), ActivityType.Warning_Letter.ToString(), Id);
            db.Sp_Add_PlotComments(Id, "Print Cancellation Letter", userid, ActivityType.Warning_Letter.ToString());
            var res = db.Sp_Get_OverdueQualifying_Plots_Id(Id).Select(x => new OverdueQualifying_Plots
            {
                Area = x.Area,
                Block_Name = x.Block_Name,
                Develop_Status = x.Develop_Status,
                Dimension = x.Dimension,
                Id = x.Id,
                Name = x.Name,
                OverDueAmount = x.Balance_Amount,
                Plot_Location = x.Plot_Location,
                Plot_No = x.Plot_No,
                Plot_Size = x.Plot_Size,
                Road_Type = x.Road_Type,
                Status = x.Status,
                Type = x.Type,
                Verified = x.Verified,
                Mobile_1 = x.Mobile_1,
                Owner_Id = x.Owner_Id,
                Verification_Req = x.Verification_Req,
                FirstNotice = x.First_Notice,
                SecNotice = x.Sec_Notice,
                CancelNotice = x.Cancel_Notice,
                Installments = x.Installments
            }).FirstOrDefault();
            return PartialView(res);
        }
        public void UpdatePlotBalances()
        {
            var res = db.Plots.Where(x => x.Status == PlotsStatus.Registered.ToString() || x.Status == PlotsStatus.Temporary_Cancelled.ToString()).ToList();
            foreach (var item in res)
            {
                this.updatebalance(item.Id);
            }
        }
        public void updatebalance(long Plotid)
        {
            var res3 = db.Sp_Get_PlotInstallments(Plotid).ToList();
            var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            var discount = db.Discounts.Where(x => x.Module_Id == Plotid && x.Plot_Is_Cancelled == null && x.Module == Modules.PlotManagement.ToString()).ToList();

            UpdatePlotInstallmentStatus(res3, res4, discount, Plotid);
        }
        public ActionResult DiscountRequest(long Plotid)
        {
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            var inst = db.Sp_Get_PlotInstallments(Plotid).Select(x => new PlotStatment
            {
                Description = x.Installment_Name,
                Date = x.DueDate,
                Debit = x.Amount,
                Credit = 0,
            }).ToList();
            string[] Type = { "Booking", "Installment" };
            var rece = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).Where(x => Type.Contains(x.Type)).Select(x => new PlotStatment
            {
                Receipt_Voucher_No = x.ReceiptNo,
                Credit = x.Amount,
                Mode = x.PaymentType,
                Chq_No = x.Ch_Pay_Draft_No,
                Date = x.DateTime,
                Bank = x.Bank,
                RcvdDate = x.DateTime,
                Type = x.Type,
                Inst_Status = x.Status
            }).ToList();
            var discount = db.Discounts.SingleOrDefault(x => x.Module_Id == Plotid && x.Module == Modules.PlotManagement.ToString());
            List<PlotStatment> Rm = new List<PlotStatment>();
            Rm.AddRange(inst);
            Rm.AddRange(rece);
            Rm = Rm.OrderBy(x => x.Date).ToList();
            var res = new PlotLastOwnerLedger { OwnerData = res2, PlotData = res1, Report = Rm, Discount = discount };
            return PartialView(res);
        }
        public JsonResult SaveDiscountRequest(long plot, decimal amt, string rems, int urgency)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                PlotDiscountRequest pd = new PlotDiscountRequest
                {
                    DescriptionText = rems,
                    DiscountAmount = amt,
                    Urgency = (UrgencyStatus)urgency
                };
                var packed = JsonConvert.SerializeObject(pd);
                db.MIS_Requests.Add(new MIS_Requests
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy_Id = uid,
                    CreatedBy_Name = uname,
                    Details = packed,
                    ModuleId = plot,
                    ModuleType = "Plot_Discount",
                    Status = RequestStatus.Pending.ToString(),
                    Type = RequestType.Plot_Discount_Requests.ToString()
                });
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public JsonResult SettleExistingMultiOwners()
        {
            //var owners = db.Plot_Ownership.ToList();
            Helpers h = new Helpers();
            //foreach (var v in owners)
            //{
            //    long grp = h.RandomNumber();
            //    v.GroupTag = grp;
            //    db.Plot_Ownership.Attach(v);
            //    db.Entry(v).Property(x => x.GroupTag).IsModified = true;
            //    db.SaveChanges();
            //}
            var plotTrans = db.Plots_Transfer_Request.ToList();
            foreach (var e in plotTrans)
            {
                long grp = h.RandomNumber();
                e.GroupTag = grp;
                db.Plots_Transfer_Request.Attach(e);
                db.Entry(e).Property(x => x.GroupTag).IsModified = true;
                db.SaveChanges();
            }
            return Json(true);
        }
        public ActionResult TransferFeeConfig()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Transfer Fee Cobnfiguration Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            if (res is null)
            {
                //this means that there is no existing configuration of this module
                //When there is no initial configuration present, we need to add a defualt configuration
                //To add a defualt configuration, we need to add all blocks by default
                var blks = db.RealEstate_Blocks.ToList();
                Plot_Transfer_Configuration config = new Plot_Transfer_Configuration();
                config.FeeInfo = new List<BlocksTransferFee>();
                foreach (var block in blks)
                {
                    config.FeeInfo.Add(new BlocksTransferFee
                    {
                        BlockId = block.Id,
                        BlockName = block.Block_Name,
                        IsApplicable = false,
                        DC_Rate_Per_Marla_Commercial = 0,
                        DC_Rate_Per_Marla_Residential = 0
                    });
                }
                //Now that all rates have been saved, throw this data to the front end for further manipulation
                PlotTransfer_ConfigView ptcv = new PlotTransfer_ConfigView
                {
                    ConfigInfo = config,
                    LastUpdateDate = null,
                    LastUpdatedBy = string.Empty
                };
                return PartialView(ptcv);
            }
            else
            {
                // this means that there is already a configuration present
                Plot_Transfer_Configuration config = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(res.CurrentConfig);
                PlotTransfer_ConfigView ptcv = new PlotTransfer_ConfigView
                {
                    ConfigInfo = config,
                    LastUpdateDate = res.LastUpdated,
                    LastUpdatedBy = res.UpdatedBy_Name
                };
                return PartialView(ptcv);
            }
        }
        public JsonResult SaveTransferFeeConfig(List<BlocksTransferFee> ptc, float B_filer, float B_nonFiler, float S_filer, float S_nonFiler, int Exemp_Day, bool Tax_Emp)
        {
            Plot_Transfer_Configuration dat = new Plot_Transfer_Configuration();
            dat.FeeInfo = ptc;

            dat.Buyer_FilerPercent = B_filer;
            dat.Buyer_NonFilerPercent = B_nonFiler;
            dat.Seller_FilerPercent = S_filer;
            dat.Seller_NonFilerPercent = S_nonFiler;

            dat.Tax_Exemption_Days = Exemp_Day;
            dat.Tax_Exemption_Applicable = Tax_Emp;


            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            if (res is null)
            {
                //new record
                db.MIS_Modules_Configurations.Add(new MIS_Modules_Configurations
                {
                    CurrentConfig = JsonConvert.SerializeObject(dat),
                    LastUpdated = DateTime.Now,
                    Module = MISModuleType.Plot_Transfer.ToString(),
                    UpdatedBy_Id = uid,
                    UpdatedBy_Name = uname
                });
                db.SaveChanges();
            }
            else
            {
                //old record
                res.CurrentConfig = JsonConvert.SerializeObject(dat);
                res.LastUpdated = DateTime.Now;
                res.UpdatedBy_Id = uid;
                res.UpdatedBy_Name = uname;
                db.MIS_Modules_Configurations.Attach(res);
                db.Entry(res).Property(x => x.UpdatedBy_Name).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Id).IsModified = true;
                db.Entry(res).Property(x => x.LastUpdated).IsModified = true;
                db.Entry(res).Property(x => x.CurrentConfig).IsModified = true;
                db.SaveChanges();
            }
            return Json(true);
        }
        public ActionResult PlotOwners(long plotId, bool? current)
        {
            if (current == true)
            {
                var res = db.Sp_Get_PlotLastOwner(plotId).ToList();
                var lastInstDate = db.Plot_Installments.Where(x => x.Plot_Id == plotId).OrderByDescending(x => x.DueDate).Select(x => x.DueDate).FirstOrDefault();
                res.ForEach(x => x.Ownership_DateTime = (x.Ownership_DateTime.Value.Date < lastInstDate) ? lastInstDate : x.Ownership_DateTime);
                return PartialView(res);
            }
            else
            {
                var res = db.Sp_Get_PlotOwnerList(plotId).ToList();
                return PartialView(res);
            }
        }
        public JsonResult UpdateFilerStatus(long owner, bool filer)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Updated Plot Owner Filer Status to: " + filer, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), owner);
            var res = db.Plot_Ownership.Where(x => x.Id == owner).FirstOrDefault();
            if (res is null)
            {
                return Json(false);
            }
            res.IsFiler = filer;
            db.Plot_Ownership.Attach(res);
            db.Entry(res).Property(x => x.IsFiler).IsModified = true;
            db.SaveChanges();
            return Json(true);
        }
        public JsonResult PlotTaxData(long Plotid)
        {
            decimal rate = 0;
            var res1 = db.Sp_Get_PlotData(Plotid).SingleOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plotid).Select(x => new PlotOwnInfoTax { Name = x.Name, IsFiler = (x.IsFiler == true) }).ToList();
            var plotTransfFee = db.MIS_Modules_Configurations.Where(x => x.Module == MISModuleType.Plot_Transfer.ToString()).FirstOrDefault();
            var parsedInfo = JsonConvert.DeserializeObject<Plot_Transfer_Configuration>(plotTransfFee.CurrentConfig);
            var blockData = parsedInfo.FeeInfo.Where(x => x.BlockId == res1.BlockIden).FirstOrDefault();
            if (blockData != null && (blockData.IsApplicable))
            {
                if (res1.Type == "Residential")
                {
                    rate = blockData.DC_Rate_Per_Marla_Residential;
                }
                else
                {
                    rate = parsedInfo.FeeInfo.Where(x => x.BlockId == res1.BlockIden).FirstOrDefault().DC_Rate_Per_Marla_Commercial;
                }
            }
            decimal totalMarlas = Convert.ToDecimal(res1.Plot_Size.Split(' ')[0]);
            decimal DcRate = Math.Ceiling(rate * totalMarlas);
            var res = new PlotTaxDetails { CurrentOwners = res2, DC_Rate = rate, Min_Sell_Price = DcRate, PlotSize = res1.Plot_Size, TaxApplicable = (blockData.IsApplicable), Buyer_FilerPerc = parsedInfo.Buyer_FilerPercent, Buyer_NonFilerPerc = parsedInfo.Buyer_NonFilerPercent, Seller_FilerPerc = parsedInfo.Seller_FilerPercent, Seller_NonFilerPerc = parsedInfo.Seller_NonFilerPercent, TransferFee = (6000 * totalMarlas) };
            return Json(res);
        }
        public ActionResult FingerPrint(long Id)
        {
            var res = db.Plot_Ownership.Where(x => x.Id == Id).FirstOrDefault();
            return View(res);
        }
        public void SpecialPrefPaymentSMS()
        {
            using (MeherEstateDevelopers.Models.Grand_CityEntities db = new Grand_CityEntities())
            {
                var SpecialPlots = db.Plot_Installments.Where(x => x.Installment_Type == "5").ToList();
                int beforeHand = 0;
                int afterHand = 0;
                foreach (var v in SpecialPlots)
                {
                    var res2 = db.Sp_Get_PlotLastOwner(v.Plot_Id).FirstOrDefault();
                    var res3 = db.Sp_Get_PlotInstallments(v.Plot_Id).ToList();
                    var res4 = db.Sp_Get_ReceivedAmounts(v.Plot_Id, Modules.PlotManagement.ToString()).ToList();
                    var discount = db.Discounts.Where(x => x.Module_Id == v.Plot_Id && x.Plot_Is_Cancelled == null && x.Module == Modules.PlotManagement.ToString()).ToList();
                    if (v.Status != "Paid")
                    {
                        beforeHand++;
                    }
                    UpdatePlotInstallmentStatus(res3, res4, discount, v.Plot_Id);
                    var afterUpdation = db.Sp_Get_PlotInstallments(v.Plot_Id).ToList();
                    string checkPaid = afterUpdation.Where(x => x.Installment_Type == "5").FirstOrDefault().Status;
                    if (checkPaid == "Paid")
                    {
                        afterHand++;
                    }
                    if (checkPaid != "Paid")
                    {
                        //{
                        //    var msgtext = "Dear " + res2.Name + "\n\r," +
                        //    "Today is last date of depositing Plot Prefernce Charges.\n\r" +
                        //    "Deposit the amount to retain your plot preference  otherwise it will be converted to normal without any prior notice.\n\r" +
                        //    "\n\r" +
                        //    "For more information contact our helpline UAN 111 724 786.\n\r" +
                        //    "Best Regards, \n\r" +
                        //    "Grand City.\n\r";
                        //    try
                        //    {
                        //        SmsService smsService = new SmsService();
                        //        smsService.PremiumSub(msgtext, res2.Mobile_1);
                        //        db.Sp_Add_PlotComments(res2.Plot_Id, msgtext, 0, "text");
                        //        //db.Sp_Add_Followup(item.FileFormNumber, msgtext, item.Name, 0, "Text");
                        //    }
                        //    catch (Exception ee)
                        //    {
                        //        EmailService e = new EmailService();
                        //        e.SendEmail(res2.Plot_Id.ToString(), "daniyal@sasystems.solutions", "Special Pref Msg Not Sent");
                        //    }
                        //}
                    }
                }
                //SmsService smsService2 = new SmsService();
                //smsService2.PremiumSub("Before Hand : " + beforeHand + " ----- After Hand : " + afterHand, "03231611324");
            }
        }
        public ActionResult SpecialPrefrencesCharges()
        {
            var res = (from x in db.Plots
                       join y in db.Plot_Installments on x.Id equals y.Plot_Id
                       where x.Status == "Registered" && y.Installment_Type == "5"
                       select new SpecialPref { Plot_Size = x.Plot_Size, DueDate = y.DueDate, Amount = y.Amount, Status = y.Status }).ToList();
            return View(res);
        }
        public ActionResult Dealershipname(long Id)
        {
            ViewBag.Dealership = new SelectList(db.Dealerships.Where(x => x.Status == "Registered"), "Id", "Dealership_Name");
            ViewBag.Plot_Id = Id;
            return PartialView();
        }
        //public JsonResult FingerPrintUpdater(string img, string patt, int ftype, long ownId)
        //{
        //    try
        //    {
        //        var ownData = db.Plot_Ownership.Where(x => x.Id == ownId).FirstOrDefault();
        //        if(ownData != null)
        //        {
        //            var plotNo = db.Plots.Where(x => x.Id == ownData.Plot_Id).Select(x => x.Plot_Number).FirstOrDefault();
        //            var uid = User.Identity.GetUserId<long>();
        //            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
        //            Owners_FingerPrints fp = new Owners_FingerPrints
        //            {
        //                Added_ByName = uname,
        //                AddedOn_Date = DateTime.Now,
        //                AddedBy_Id = uid,
        //                FilePlotId = ownData.Plot_Id,
        //                FilePlotNo = plotNo,
        //                OwnerId = ownData.Id,
        //                OwnerName = ownData.Name
        //            };
        //            if (ftype == 9)
        //            {
        //                //right small finger
        //                fp.Right_LittleFinger_Image = img;
        //                fp.Right_LittleFinger_PrintCode = patt;
        //            }
        //            else if (ftype == 8)
        //            {
        //                //right ring finger
        //                fp.Right_RingFinger_Image = img;
        //                fp.Right_RingFinger_PrintCode = patt;
        //            }
        //            else if (ftype == 7)
        //            {
        //                //right middle finger
        //                fp.Right_MiddleFinger_Image = img;
        //                fp.Right_MiddleFinger_PrintCode = patt;
        //            }
        //            else if (ftype == 6)
        //            {
        //                //right index finger
        //                fp.Right_IndexFinger_Image = img;
        //                fp.Right_IndexFinger_PrintCode = patt;
        //            }
        //            else if (ftype == 10)
        //            {
        //                //right thumb
        //                fp.Right_Thumb_Image = img;
        //                fp.Right_Thumb_PrintCode = patt;
        //            }
        //            else if (ftype == 5)
        //            {
        //                //left thumb
        //                fp.Left_Thumb_Image = img;
        //                fp.Left_Thumb_PrintCode = patt;
        //            }
        //            else if (ftype == 4)
        //            {
        //                //left index finger
        //                fp.Left_IndexFinger_Image = img;
        //                fp.Left_IndexFinger_PrintCode = patt;
        //            }
        //            else if (ftype == 3)
        //            {
        //                //left middle finger
        //                fp.Left_MiddleFinger_Image = img;
        //                fp.Left_MiddleFinger_PrintCode = patt;
        //            }
        //            else if (ftype == 2)
        //            {
        //                //left ring finger
        //                fp.Left_RingFinger_Image = img;
        //                fp.Left_RingFinger_PrintCode = patt;
        //            }
        //            else if (ftype == 1)
        //            {
        //                //left small finger
        //                fp.Left_LittleFinger_Image = img;
        //                fp.Left_LittleFinger_PrintCode = patt;
        //            }
        //            db.Owners_FingerPrints.Add(fp);
        //            db.SaveChanges();
        //            string pth = Server.MapPath("/Repository/Plot Owners Finger Prints/");
        //            if(!Directory.Exists(pth))
        //            {
        //                Directory.CreateDirectory(pth);
        //            }
        //            new Helpers().SaveBase64Image(img, pth + "/" + ownId.ToString() + "_" + ftype.ToString() + ".png", string.Empty);
        //            return Json(true);
        //        }
        //        return Json(false);
        //    }
        //    catch(Exception ex)
        //    {
        //        return Json(false);
        //    }
        //}
        //public JsonResult MatchFingers(string img, long ownId, int ftype)
        //{
        //    //initializing matching dlls
        //    SGFingerPrintManager m_FPM = new SGFingerPrintManager();
        //    //get existing data of owner
        //    var res = db.Owners_FingerPrints.Where(x => x.OwnerId == ownId).FirstOrDefault();
        //    byte[] currImg = Convert.FromBase64String(img);
        //    byte[] savedImg = new byte[1];
        //    if (ftype == 9)
        //    {
        //        //right small finger
        //        savedImg = Convert.FromBase64String(res.Right_LittleFinger_Image);
        //    }
        //    else if (ftype == 8)
        //    {
        //        //right ring finger
        //        savedImg = Convert.FromBase64String(res.Right_RingFinger_Image);
        //    }
        //    else if (ftype == 7)
        //    {
        //        //right middle finger
        //        savedImg = Convert.FromBase64String(res.Right_MiddleFinger_Image);
        //    }
        //    else if (ftype == 6)
        //    {
        //        //right index finger
        //        savedImg = Convert.FromBase64String(res.Right_IndexFinger_Image);
        //    }
        //    else if (ftype == 10)
        //    {
        //        //right thumb
        //        savedImg = Convert.FromBase64String(res.Right_Thumb_Image);
        //    }
        //    else if (ftype == 5)
        //    {
        //        //left thumb
        //        savedImg = Convert.FromBase64String(res.Left_Thumb_Image);
        //    }
        //    else if (ftype == 4)
        //    {
        //        //left index finger
        //        savedImg = Convert.FromBase64String(res.Left_IndexFinger_Image);
        //    }
        //    else if (ftype == 3)
        //    {
        //        //left middle finger
        //        savedImg = Convert.FromBase64String(res.Left_MiddleFinger_Image);
        //    }
        //    else if (ftype == 2)
        //    {
        //        //left ring finger
        //        savedImg = Convert.FromBase64String(res.Left_RingFinger_Image);
        //    }
        //    else if (ftype == 1)
        //    {
        //        //left small finger
        //        savedImg = Convert.FromBase64String(res.Left_LittleFinger_Image);
        //    }
        //    //Matching
        //    Int32 iError;
        //    bool matched = false;
        //    SGFPMSecurityLevel securityLevel = SGFPMSecurityLevel.NONE;
        //    iError = m_FPM.MatchTemplate(currImg, savedImg, securityLevel, ref matched);
        //    return Json(new { Code = iError, Status = matched });
        //}

        public ActionResult NewPlotOwner(long grp)
        {
            ViewBag.grpTag = grp;
            return PartialView();
        }
        public JsonResult UpdatePlotOwnerImage(string img, long ownId)
        {
            try
            {
                var res = db.Plot_Ownership.Where(x => x.Id == ownId).FirstOrDefault();
                res.Owner_Img = img;
                db.Plot_Ownership.Attach(res);
                db.Entry(res).Property(x => x.Owner_Img).IsModified = true;
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public JsonResult PlotsByBlockId_Select(string s, long block)
        {
            var res = db.Plots.Where(x => x.Plot_Number.Contains(s) && x.Block_Id == block).Select(x => new { id = x.Id, text = x.Plot_Number + " " + x.Sector + " - " + x.Plot_Location + " - " + x.Type }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPlotLastOwnerData(long pltId)
        {
            var res = db.Sp_Get_PlotLastOwner(pltId).ToList();
            var pltSize = db.Plots.Where(x => x.Id == pltId).Select(x => x.Plot_Size).FirstOrDefault();
            return Json(new { Name = string.Join(" , ", res.Select(x => x.Name)), FatherName = string.Join(" , ", res.Select(x => x.Father_Husband)), Address = string.Join(" , ", res.Select(x => x.Residential_Address)), CNIC = string.Join(" , ", res.Select(x => x.CNIC_NICOP)), Contact = string.Join(" , ", res.Select(x => x.Mobile_1)), Size = pltSize });
        }
        public ActionResult SpecialPrefList(DateTime? Due_Date, string Status)
        {
            var res = db.Sp_Get_SpecialPrefPlot_list(Due_Date, Status).ToList();
            return PartialView(res);
        }
        public ActionResult PlotsDataDetailView()
        {
            var res1 = db.Plots.GroupBy(x => new { Type = x.Type, Phase_name = x.Phase_Name }).
               Select(x => new PhaseReport { Phase = x.Key.Phase_name, Type = x.Key.Type, Total = x.Count() }).ToList();
            var res2 = db.Plots.GroupBy(x => new { Block = x.Block_Name, Phase_Name = x.Phase_Name, Type = x.Type, Status = x.Status }).
                Select(x => new PlotStatusReport { Block = x.Key.Block, Plot_Type = x.Key.Type, Status = x.Key.Status, PhaseName = x.Key.Phase_Name, Total = x.Count() }).ToList();
            var res3 = db.Plots.GroupBy(x => new { Type = x.Type, Block = x.Block_Name, Verified = x.Verified }).
                 Select(x => new VerifiReport { Block = x.Key.Block, Type = x.Key.Type, Verified = x.Key.Verified, Total = x.Count() }).ToList();
            var res4 = db.Plots.GroupBy(x => new { Type = x.Type, Block = x.Block_Name, DevelopStatus = x.Develop_Status }).
              Select(x => new ConstructionReport { Block = x.Key.Block, Type = x.Key.Type, DevelopStatus = x.Key.DevelopStatus, Total = x.Count() }).ToList();
            CompiledReport result = new CompiledReport();
            result.Phases = res1;
            result.PlotStatuses = res2;
            result.PlotVerifications = res3;
            result.PlotConstructions = res4;
            return View(result);
        }
        public ActionResult PlotsModuleConfiguration()
        {
            return PartialView();
        }
        public ActionResult DiscountConfiguration()
        {
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == "Discount Configuration").FirstOrDefault();
            if (res is null)
            {
                //means no current discount policy exists
                return PartialView();
            }
            else
            {
                //throw this discount policy to the front end
                var parsed = JsonConvert.DeserializeObject<List<DiscountConfigModel>>(res.CurrentConfig);
                return PartialView(parsed);
            }
        }
        public JsonResult SaveDiscountConfig(List<DiscountConfigModel> discounts)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var res = db.MIS_Modules_Configurations.Where(x => x.Module == "Discount Configuration").FirstOrDefault();
                if (res is null)
                {
                    res = new MIS_Modules_Configurations
                    {
                        CurrentConfig = JsonConvert.SerializeObject(discounts),
                        LastUpdated = DateTime.Now,
                        Module = "Discount Configuration",
                        UpdatedBy_Id = uid,
                        UpdatedBy_Name = uname
                    };
                    db.MIS_Modules_Configurations.Add(res);
                    db.SaveChanges();
                }
                else
                {
                    res.CurrentConfig = JsonConvert.SerializeObject(discounts);
                    res.LastUpdated = DateTime.Now;
                    res.UpdatedBy_Id = uid;
                    res.UpdatedBy_Name = uname;
                    db.MIS_Modules_Configurations.Attach(res);
                    db.Entry(res).Property(x => x.CurrentConfig).IsModified = true;
                    db.Entry(res).Property(x => x.LastUpdated).IsModified = true;
                    db.Entry(res).Property(x => x.UpdatedBy_Id).IsModified = true;
                    db.Entry(res).Property(x => x.UpdatedBy_Name).IsModified = true;
                    db.SaveChanges();
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public JsonResult MarkAllotmentLtrUnDelivered(long id, string reason)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var res = db.AllotmentLetters.Where(x => x.Id == id).FirstOrDefault();
                res.Delivered = null;
                res.Delivery_Date = null;
                db.AllotmentLetters.Attach(res);
                db.Entry(res).Property(x => x.Delivery_Date).IsModified = true;
                db.Entry(res).Property(x => x.Delivered).IsModified = true;
                db.SaveChanges();
                db.Sp_Add_PlotComments(res.Plot_Id, "Allotment letter has been marked as Un delivered. Reason : " + reason, uid, "text");
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        //public void Plotsverify()
        //{
        //    List<long> pltis = new List<long>();
        //    //    string[] c_ars = { "13", "25", "72" };
        //    //    var plot1 = db.Plots.Where(x => c_ars.Contains(x.Plot_Number) && x.Block_Id == 2 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    //    pltis.AddRange(plot1);
        //    string[] r_ars = { "169","214","220","386","40","407","445","86","272","340","18","345","263","279" };
        //    var plot2 = db.Plots.Where(x => r_ars.Contains(x.Plot_Number) && x.Block_Id == 2 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot2);
        //    string[] r_cha = { "157","171","24","255","95" };
        //    var plot3 = db.Plots.Where(x => r_cha.Contains(x.Plot_Number) && x.Block_Id == 12 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot3);
        //    string[] c_cha = { "41" };
        //    var plot4 = db.Plots.Where(x => c_cha.Contains(x.Plot_Number) && x.Block_Id == 12 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot4);
        //    string[] r_fai = { "119" };
        //    var plot5 = db.Plots.Where(x => r_fai.Contains(x.Plot_Number) && x.Block_Id == 15 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot5);
        //    string[] r_fari = { "154","159","167","175","181","182","183","187","188","90","94-B","199","195", };
        //    var plot6 = db.Plots.Where(x => r_fari.Contains(x.Plot_Number) && x.Block_Id == 5 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot6);
        //    string[] c_fari = { "27" };
        //    var plot7 = db.Plots.Where(x => c_fari.Contains(x.Plot_Number) && x.Block_Id == 5 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot7);
        //    string[] r_fur = { "145","147","156","158","162","164","165","166","178","179","181","32","56","67","74","75","76","82","92","70","193","173" };
        //    var plot8 = db.Plots.Where(x => r_fur.Contains(x.Plot_Number) && x.Block_Id == 6 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot8);
        //    string[] r_kam = { "11","134","135","142","147","178","192","204","21","253","265","307","312","315","57","70","85","87","88","89","97","40","184","186" };
        //    var plot9 = db.Plots.Where(x => r_kam.Contains(x.Plot_Number) && x.Block_Id == 3 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot9);
        //    string[] c_kam = { "121", "122", "14", "79", "80", "83" };
        //    var plot10 = db.Plots.Where(x => c_kam.Contains(x.Plot_Number) && x.Block_Id == 3 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot10);
        //    string[] r_sheafz = { "112","132","171","173","189","199","228","244","26","27","40","59","84", };
        //    var plot11 = db.Plots.Where(x => r_sheafz.Contains(x.Plot_Number) && x.Block_Id == 7 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot11);
        //    string[] c_sheafz = { "1","11","14","2","20","22","26","3","8", };
        //    var plot12 = db.Plots.Where(x => c_sheafz.Contains(x.Plot_Number) && x.Block_Id == 7 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot12);
        //    //    string[] r_sheaz = { "102","112","118","142","32","46","94" };
        //    //    var plot13 = db.Plots.Where(x => r_sheaz.Contains(x.Plot_Number) && x.Block_Id == 8 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    //    pltis.AddRange(plot13);
        //    string[] c_sheaz = { "7","9" };
        //    var plot14 = db.Plots.Where(x => c_sheaz.Contains(x.Plot_Number) && x.Block_Id == 8 && x.Type == "Commercial").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot14);
        //    string[] r_sherzamn = { "215","251","292","297","413","252" };
        //    var plot15 = db.Plots.Where(x => r_sherzamn.Contains(x.Plot_Number) && x.Block_Id == 11 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot15);
        //    string[] r_shoai = { "320","324","53", };
        //    var plot16 = db.Plots.Where(x => r_shoai.Contains(x.Plot_Number) && x.Block_Id == 4 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot16);
        //    string[] r_sohai = { "119","25","34","99","99","102" };
        //    var plot17 = db.Plots.Where(x => r_sohai.Contains(x.Plot_Number) && x.Block_Id == 9 && x.Type == "Residential").Select(x => x.Id).ToList();
        //    pltis.AddRange(plot17);
        //string[] BadarA = { "206", "207", "241", "94" };
        //var plot18 = db.Plots.Where(x => BadarA.Contains(x.Plot_Number) && x.Block_Id == 10 && x.Sector == "A" && x.Type == "Residential").Select(x => x.Id).ToList();
        //pltis.AddRange(plot18);
        //string[] BadarC = { "556" };
        //var plot19 = db.Plots.Where(x => BadarC.Contains(x.Plot_Number) && x.Block_Id == 10 && x.Sector == "C" && x.Type == "Residential").Select(x => x.Id).ToList();
        //pltis.AddRange(plot19);
        //    foreach (var item in pltis)
        //    {
        //        var up = db.Sp_Update_VerifyStatus(item, Modules.PlotManagement.ToString());
        //        var comt = db.Sp_Add_PlotComments(item, "Plot is Verified by Ammad Bin Shabbir (Systematically). As per the attachment", 20073, "Text");
        //        string fileName = "capture.png";
        //        string sourcePath = Server.MapPath("~/images/");
        //        if (!Directory.Exists(Server.MapPath("~/PlotsData/" + item + "")))
        //        {
        //            Directory.CreateDirectory(Server.MapPath("~/PlotsData/" + item + ""));
        //        }
        //        string targetPath = Server.MapPath("~/PlotsData/" + item + "");
        //        string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
        //        string destFile = System.IO.Path.Combine(targetPath, fileName);
        //        System.IO.File.Copy(sourceFile, destFile, true);
        //        var com = db.Sp_Add_PlotComments(item, fileName, 20073, "file");
        //    }
        //}
        public ActionResult SwapPlot()
        {
            return View();
        }
        public JsonResult GetPlots_ForSelect_PlotSwap(string s, bool typ)
        {
            if (string.IsNullOrEmpty(s))
            {
                return Json(new { text = "Select Plot", id = 0 }, JsonRequestBehavior.AllowGet);
            }
            string[] parsed = s.Split(' ');
            if (parsed.Length > 1)
            {
                string gplt = parsed[0];
                string blk = parsed[1];
                var res = db.Plots.Where(x => x.Plot_Number.Contains(gplt) && x.Block_Name.Contains(blk) && x.Status == ((typ) ? "Registered" : "Hold")).Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + "-" + x.Block_Name + " ( " + x.Plot_Location + " )", id = x.Id }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = db.Plots.Where(x => x.Plot_Number.Contains(s) && x.Status == ((typ) ? "Registered" : "Hold")).Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + " - " + x.Block_Name + " ( " + x.Plot_Location + " )", id = x.Id }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveSwapRequest(List<SwapPlotRequestModel> swaps)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                MIS_Requests misr = new MIS_Requests
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy_Id = uid,
                    CreatedBy_Name = uname,
                    Details = JsonConvert.SerializeObject(new SwapPlotRequest
                    {
                        DescriptionText = "Request for plot swapping.",
                        Urgency = UrgencyStatus.Urgent,
                        Swaps = swaps
                    }),
                    ModuleId = 0,
                    ModuleType = "PlotSwap",
                    Type = "PlotSwap",
                    Status = "Pending"
                };
                db.MIS_Requests.Add(misr);
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public JsonResult ApproveSwapRequest(long req)
        {
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res = db.MIS_Requests.Where(x => x.Id == req).FirstOrDefault();
                    if (res.Status != "Pending")
                    {
                        return Json(false);
                    }
                    var uid = User.Identity.GetUserId<long>();
                    var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                    res.Status = "Approved";
                    var prsed = JsonConvert.DeserializeObject<SwapPlotRequest>(res.Details);
                    prsed.StatusChangedAt = DateTime.Now;
                    prsed.ManagerApproval = "Approved";
                    prsed.ManagerName = uname;
                    prsed.Manager_Id = uid;
                    res.Details = JsonConvert.SerializeObject(prsed);
                    db.MIS_Requests.Attach(res);
                    db.Entry(res).Property(x => x.Details).IsModified = true;
                    db.Entry(res).Property(x => x.Status).IsModified = true;
                    db.SaveChanges();
                    // is k baad ab sab plots ko swap kro
                    foreach (var v in prsed.Swaps)
                    {
                        var fromPlt = db.Plots.Where(x => x.Id == v.FromPlot).FirstOrDefault();
                        var toPlt = db.Plots.Where(x => x.Id == v.ToPlot).FirstOrDefault();
                        toPlt.Verified = null;
                        toPlt.VerifiedDate = null;
                        db.Sp_Swap_Plot(fromPlt.Id, toPlt.Plot_Number, toPlt.Block_Id, toPlt.Phase_Id, toPlt.Sector, toPlt.Plot_Size, toPlt.Plot_Category, toPlt.Develop_Status, toPlt.Plot_Location, toPlt.Type,
                            toPlt.Road_Type, fromPlt.Status, toPlt.Verified, toPlt.AllotmentReq, toPlt.AllotmentReq_Date, toPlt.Old_PlotNumber, toPlt.reg, toPlt.IsForApproval, toPlt.Verification_Req, toPlt.Verification_Date, toPlt.Remarks,
                            toPlt.Registry, toPlt.Block_Name, toPlt.Mortgage, toPlt.Mortgage_by, toPlt.VerificationReqDate, toPlt.VerifiedDate, toPlt.Hold, toPlt.PossessionReq, toPlt.PossessionReq_Date, toPlt.North, toPlt.South, toPlt.East,
                            toPlt.West, toPlt.North_East, toPlt.North_West, toPlt.South_West, toPlt.Front, toPlt.Phase_Name, toPlt.South_East, toPlt.Rental, toPlt.Application_FileNo, toPlt.Application_FileId, toPlt.Development_Charges,
                            toPlt.Dealership_Id, toPlt.Dealership_Name, toPlt.Installment_Plan_Id, toPlt.Area);
                        db.Sp_Swap_Plot(toPlt.Id, fromPlt.Plot_Number, fromPlt.Block_Id, fromPlt.Phase_Id, fromPlt.Sector, fromPlt.Plot_Size, fromPlt.Plot_Category, fromPlt.Develop_Status, fromPlt.Plot_Location, fromPlt.Type,
                           fromPlt.Road_Type, toPlt.Status, fromPlt.Verified, fromPlt.AllotmentReq, fromPlt.AllotmentReq_Date, fromPlt.Old_PlotNumber, fromPlt.reg, fromPlt.IsForApproval, fromPlt.Verification_Req, fromPlt.Verification_Date, fromPlt.Remarks,
                           fromPlt.Registry, fromPlt.Block_Name, fromPlt.Mortgage, fromPlt.Mortgage_by, fromPlt.VerificationReqDate, fromPlt.VerifiedDate, fromPlt.Hold, fromPlt.PossessionReq, fromPlt.PossessionReq_Date, fromPlt.North, fromPlt.South, fromPlt.East,
                           fromPlt.West, fromPlt.North_East, fromPlt.North_West, fromPlt.South_West, fromPlt.Front, fromPlt.Phase_Name, fromPlt.South_East, fromPlt.Rental, fromPlt.Application_FileNo, fromPlt.Application_FileId, fromPlt.Development_Charges,
                           fromPlt.Dealership_Id, fromPlt.Dealership_Name, fromPlt.Installment_Plan_Id, fromPlt.Area);
                        var bltfrom = db.File_Form.Where(x => x.BallotedPlot_Id == fromPlt.Id).FirstOrDefault();
                        var bltto = db.File_Form.Where(x => x.BallotedPlot_Id == toPlt.Id).FirstOrDefault();
                        if (bltfrom != null)
                        {
                            bltfrom.BallotedPlot_Number = toPlt.Plot_Number + " " + toPlt.Sector;
                            db.File_Form.Attach(bltfrom);
                            db.Entry(bltfrom).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        if (bltto != null)
                        {
                            bltto.BallotedPlot_Number = fromPlt.Plot_Number + " " + fromPlt.Sector;
                            db.File_Form.Attach(bltto);
                            db.Entry(bltto).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        db.Sp_Add_PlotComments(toPlt.Id, "Customer has been assigned plot # " + fromPlt.Plot_Number + " " + fromPlt.Sector + " " + fromPlt.Block_Name + " From " + toPlt.Plot_Number + " " + toPlt.Sector + " " + toPlt.Block_Name, res.CreatedBy_Id, "text");
                        db.Sp_Add_PlotComments(fromPlt.Id, "Customer has been assigned plot # " + toPlt.Plot_Number + " " + toPlt.Sector + " " + toPlt.Block_Name + " From " + fromPlt.Plot_Number + " " + fromPlt.Sector + " " + fromPlt.Block_Name, res.CreatedBy_Id, "text");
                    }
                    Transaction.Commit();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult RejectSwapRequest(long req)
        {
            try
            {
                var res = db.MIS_Requests.Where(x => x.Id == req).FirstOrDefault();
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                res.Status = "Rejected";
                var prsed = JsonConvert.DeserializeObject<SwapPlotRequest>(res.Details);
                prsed.StatusChangedAt = DateTime.Now;
                prsed.ManagerApproval = "Rejected";
                prsed.ManagerName = uname;
                prsed.Manager_Id = uid;
                res.Details = JsonConvert.SerializeObject(prsed);
                db.MIS_Requests.Attach(res);
                db.Entry(res).Property(x => x.Details).IsModified = true;
                db.Entry(res).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public ActionResult PlotSwapPartial(long id)
        {
            var res = db.MIS_Requests.Where(x => x.Id == id).FirstOrDefault();
            SwapPlotRequest spr = JsonConvert.DeserializeObject<SwapPlotRequest>(res.Details);
            return PartialView(spr);
        }
        public JsonResult DeletePlotInstallment(long insId, string comment)
        {
            long? userid = long.Parse(User.Identity.GetUserId());
            var res = db.Plot_Installments.Where(x => x.Id == insId).FirstOrDefault();
            var comm = res.Installment_Name + " deleted of amount " + string.Format("{0:0,0.##}", res.Amount) + ". Due date was " + string.Format("{0:dd-MMM-yyyy}", res.DueDate) + ". Reaon: " + comment;
            db.Sp_Delete_Plot_Installment(insId);
            db.Sp_Add_PlotComments(res.Plot_Id, comm, userid, "Installment Structure Updation");
            return Json(true);
        }
        [HttpPost]
        public JsonResult FiesImageUploader(long OwnerId, long Plot_Id, long imgID)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            var path = "~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId;


            int index = 0;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath(path + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath(path + "/"));
                }
                string savedFileName = Path.Combine(Server.MapPath(path + "/"), Convert.ToString(imgID) + ".jpg");
                hpf.SaveAs(savedFileName);
            }
            var res = db.Sp_Update_PlotImages(imgID, Plot_Id, OwnerId).FirstOrDefault();
            db.Sp_Add_PlotComments(Plot_Id, "Update Owner Image", userid, ActivityType.Record_Upatation.ToString());
            return Json(true);

        }
        public JsonResult ConverterPlotsImages()
        {
            long userid = long.Parse(User.Identity.GetUserId());

            var res_list = db.Sp_Get_Plot_Image().ToList();
            long OwnerId;
            long Plot_Id;
            long imgID;
            string img;
            Helpers H = new Helpers();
            foreach (var own in res_list)
            {

                if (own.Owner_Img != null)
                {
                    OwnerId = own.Id;
                    Plot_Id = own.Plot_Id ?? 0;
                    imgID = 1;
                    img = own.Owner_Img;

                    if (!Directory.Exists(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId));
                    }
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId + "/"), "1.jpg");
                    H.SaveBase64Image(img, pathMain, "1.jpg");
                    var res = db.Sp_Update_PlotImages(imgID, Plot_Id, OwnerId).FirstOrDefault();
                }
                if (own.Owner_Img2 != null)
                {
                    OwnerId = own.Id;
                    Plot_Id = own.Plot_Id ?? 0;
                    imgID = 2;
                    img = own.Owner_Img;
                    if (!Directory.Exists(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId));
                    }
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId + "/"), "2.jpg");
                    H.SaveBase64Image(img, pathMain, "2.jpg");
                    var res = db.Sp_Update_PlotImages(imgID, Plot_Id, OwnerId).FirstOrDefault();
                }
                if (own.Owner_Img3 != null)
                {
                    OwnerId = own.Id;
                    Plot_Id = own.Plot_Id ?? 0;
                    imgID = 3;
                    img = own.Owner_Img;
                    if (!Directory.Exists(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId));
                    }
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId + "/"), "3.jpg");
                    H.SaveBase64Image(img, pathMain, "3.jpg");
                    var res = db.Sp_Update_PlotImages(imgID, Plot_Id, OwnerId).FirstOrDefault();
                }
                if (own.Owner_Img4 != null)
                {
                    OwnerId = own.Id;
                    Plot_Id = own.Plot_Id ?? 0;
                    imgID = 4;
                    img = own.Owner_Img;
                    if (!Directory.Exists(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId));
                    }
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/CustomerImagesPlots/" + Plot_Id + "/" + OwnerId + "/"), "4.jpg");
                    H.SaveBase64Image(img, pathMain, "4.jpg");
                    var res = db.Sp_Update_PlotImages(imgID, Plot_Id, OwnerId).FirstOrDefault();
                }
            }


            return Json(true);

        }
        public JsonResult UpdateLetterStep(long Id, int Let)
        {
            var a = db.Sp_Update_WarningLetter_Steps(Id, Modules.PlotManagement.ToString(), Let);
            return Json(new Return { Msg = "Letter is steped back", Status = true });
        }
        public JsonResult ConverterAllotmentImages()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res_list = db.Sp_Get_AllotmentImage().ToList();
            long OwnerId;

            long imgID;
            string img;
            Helpers H = new Helpers();
            foreach (var own in res_list)
            {

                if (own.Own_Img != null)
                {
                    OwnerId = own.Id;
                    imgID = 1;
                    img = own.Own_Img;
                    if (!Directory.Exists(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId));
                    }
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId + "/"), "1.jpg");
                    H.SaveBase64Image(img, pathMain, "1.jpg");

                    var res = db.Sp_Update_AllotmentImages(imgID, OwnerId).FirstOrDefault();
                }
                if (own.Owner_Img2 != null)
                {
                    OwnerId = own.Id;
                    imgID = 2;
                    img = own.Owner_Img2;
                    if (!Directory.Exists(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId));
                    }
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId + "/"), "2.jpg");
                    H.SaveBase64Image(img, pathMain, "2.jpg");
                    var res = db.Sp_Update_AllotmentImages(imgID, OwnerId).FirstOrDefault();

                }
                if (own.Owner_Img3 != null)
                {
                    OwnerId = own.Id;
                    imgID = 3;
                    img = own.Owner_Img3;
                    if (!Directory.Exists(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId));
                    }
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId + "/"), "3.jpg");
                    H.SaveBase64Image(img, pathMain, "3.jpg");
                    var res = db.Sp_Update_AllotmentImages(imgID, OwnerId).FirstOrDefault();

                }
                if (own.Owner_Img4 != null)
                {
                    OwnerId = own.Id;
                    imgID = 4;
                    img = own.Owner_Img4;
                    if (!Directory.Exists(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId)))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId));
                    }
                    var pathMain = Path.Combine(Server.MapPath("~/Repository/AllotmentLetterCustomers/" + OwnerId + "/"), "4.jpg");
                    H.SaveBase64Image(img, pathMain, "4.jpg");

                    var res = db.Sp_Update_AllotmentImages(imgID, OwnerId).FirstOrDefault();

                }
            }


            return Json(true);

        }
        public JsonResult UpdatePlotMortgagedStatus(long Id, bool Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            string text = "";
            if (Status == false)
            {
                text = "Remove Mortgaged Status of Plot";
            }
            else
            {
                text = "Mark Plot as Mortgaged";
            }
            db.Sp_Add_Activity(userid, text, "Update", Modules.PlotManagement.ToString(), ActivityType.Record_Upatation.ToString(), Id);
            db.Sp_Add_PlotComments(Id, text, userid, ActivityType.Record_Upatation.ToString());
            db.Sp_Update_PlotMorgagedStatus(Id, Status);
            return Json(true);
        }
        public ActionResult AdjustPlotToOtherPlot()
        {
            return View();
        }
        public JsonResult GetPlots_ForSelect_Plot_Adj(string s, bool typ)
        {
            if (string.IsNullOrEmpty(s))
            {
                return Json(new { text = "Select Plot", id = 0 }, JsonRequestBehavior.AllowGet);
            }
            string[] parsed = s.Split(' ');
            if (parsed.Length > 1)
            {
                string gplt = parsed[0];
                string blk = parsed[1];
                var res = db.Plots.Where(x => x.Plot_Number.Contains(gplt) && x.Block_Name.Contains(blk)).Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + "-" + x.Block_Name + " ( " + x.Plot_Location + " )", id = x.Id }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = db.Plots.Where(x => x.Plot_Number.Contains(s)).Select(x => new { text = x.Plot_Number + "-" + x.Sector + "-" + x.Type + " - " + x.Block_Name + " ( " + x.Plot_Location + " )", id = x.Id }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveAdjRequest(List<AdjPlotRequestModel> swaps)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                MIS_Requests misr = new MIS_Requests
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy_Id = uid,
                    CreatedBy_Name = uname,
                    Details = JsonConvert.SerializeObject(new AdjPlotRequest
                    {
                        DescriptionText = "Request for plot Adjustment.",
                        Urgency = UrgencyStatus.Urgent,
                        Adjusts = swaps
                    }),
                    ModuleId = 0,
                    ModuleType = "PlotAdjustment",
                    Type = "PlotAdjustment",
                    Status = "Pending"
                };
                db.MIS_Requests.Add(misr);
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public JsonResult ApproveAdjRequest(long req)
        {
            string temp = "";
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res = db.MIS_Requests.Where(x => x.Id == req).FirstOrDefault();
                    if (res.Status != "Pending")
                    {
                        return Json(false);
                    }
                    var uid = User.Identity.GetUserId<long>();
                    var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                    res.Status = "Approved";
                    var prsed = JsonConvert.DeserializeObject<AdjPlotRequest>(res.Details);
                    temp = res.Details;
                    prsed.StatusChangedAt = DateTime.Now;
                    prsed.ManagerApproval = "Approved";
                    prsed.ManagerName = uname;
                    prsed.Manager_Id = uid;
                    res.Details = JsonConvert.SerializeObject(prsed);
                    db.MIS_Requests.Attach(res);
                    db.Entry(res).Property(x => x.Details).IsModified = true;
                    db.Entry(res).Property(x => x.Status).IsModified = true;
                    db.SaveChanges();
                    // is k baad ab sab plots ko swap kro
                    foreach (var v in prsed.Adjusts)
                    {
                        var fromPlt = db.Sp_Get_PlotData(v.FromPlot).FirstOrDefault();
                        var toPlt = db.Sp_Get_PlotData(v.ToPlot).FirstOrDefault();
                        var lastowner = db.Sp_Get_PlotLastOwner(v.FromPlot).Select(x => x.GroupTag).FirstOrDefault();
                        db.Sp_Update_PlotAdjustment(fromPlt.Id, fromPlt.Plot_No + " - " + fromPlt.Block_Name, toPlt.Id, toPlt.Plot_No + " - " + toPlt.Block_Name, Modules.PlotManagement.ToString(), uid, lastowner);
                    }
                    Transaction.Commit();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    var a = db.Sp_Add_ErrorLog(ex.Message + temp, (ex.InnerException == null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, "Plots", "SavePlotOwnerData");
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult RejectAdjustRequest(long req)
        {
            try
            {
                var res = db.MIS_Requests.Where(x => x.Id == req).FirstOrDefault();
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                res.Status = "Rejected";
                var prsed = JsonConvert.DeserializeObject<AdjPlotRequest>(res.Details);
                prsed.StatusChangedAt = DateTime.Now;
                prsed.ManagerApproval = "Rejected";
                prsed.ManagerName = uname;
                prsed.Manager_Id = uid;
                res.Details = JsonConvert.SerializeObject(prsed);
                db.MIS_Requests.Attach(res);
                db.Entry(res).Property(x => x.Details).IsModified = true;
                db.Entry(res).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public ActionResult PlotAdjustPartial(long id)
        {
            var res = db.MIS_Requests.Where(x => x.Id == id).FirstOrDefault();
            AdjPlotRequest spr = JsonConvert.DeserializeObject<AdjPlotRequest>(res.Details);
            return PartialView(spr);
        }

        public ActionResult Noti(long? id, NotifierMsg? tp, long? noti)
        {
            Thread notiReader = new Thread(() => Notifier.ReadNotification((long)noti));
            notiReader.Start();

            if (tp == NotifierMsg.Request_For_Verification_Plot)
            {
                return RedirectToAction("DemandOrders", "Inventory");
            }
            else //if (tp == NotifierMsg.Item_Issued)
            {
                return RedirectToAction("ProjectConfiguration", "ConstructProjectManagement", new { Id = id });
            }
        }
        public ActionResult TransferFeesConfiguration()
        {
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == "Transfer_Fee_Config").FirstOrDefault();
            if (res is null)
            {
                //means no configurations exists yet
                //so we're gonna create a new config
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var dummyData = db.RealEstate_Blocks.Select(x => new BlockTransferFeeModel
                {
                    BlockId = x.Id,
                    BlockName = x.Block_Name,
                    NC_Residential = 0,
                    NC_Commercial = 0,
                    NC_Residential_Dealer = 0,
                    NC_Commercial_Dealer = 0
                }).ToList();
                MIS_Modules_Configurations mmc = new MIS_Modules_Configurations
                {
                    CurrentConfig = JsonConvert.SerializeObject(dummyData),
                    Config_For_Update = null,
                    LastUpdated = DateTime.Now,
                    Module = "Transfer_Fee_Config",
                    UpdatedBy_Id = uid,
                    UpdatedBy_Name = uname
                };
                db.MIS_Modules_Configurations.Add(mmc);
                db.SaveChanges();
                return PartialView(dummyData);
            }
            else
            {
                //config exists so throw this config to the view directly
                var parsed = JsonConvert.DeserializeObject<List<BlockTransferFeeModel>>(res.CurrentConfig);
                return PartialView(parsed);
            }
        }
        public JsonResult SaveBlockTransferFeeConfig(List<BlockTransferFeeModel> config)
        {
            try
            {
                List<BlockTransferFeeModel> chkMod = new List<BlockTransferFeeModel>();
                foreach (var v in config)
                {
                    if (v.BlockId != 0 && v.BlockName != null)
                    {
                        chkMod.Add(v);
                    }
                }
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var res = db.MIS_Modules_Configurations.Where(x => x.Module == "Transfer_Fee_Config").FirstOrDefault();
                res.CurrentConfig = JsonConvert.SerializeObject(chkMod);
                res.LastUpdated = DateTime.Now;
                res.UpdatedBy_Id = uid;
                res.UpdatedBy_Name = uname;
                db.MIS_Modules_Configurations.Attach(res);
                db.Entry(res).Property(x => x.LastUpdated).IsModified = true;
                db.Entry(res).Property(x => x.CurrentConfig).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Id).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Name).IsModified = true;
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(false);
            }
        }
        public ActionResult UpdateInstallmentInfo(long id)
        {
            ViewBag.id = id;
            var res = db.Plot_Installments.Where(x => x.Plot_Id == id).ToList();
            return PartialView(res);
        }
        public JsonResult UpdateInstallmentInfoPlot(long id, List<Plot_Installments> installmentData)
        {
            //var InstallmentStructureData = new XElement("InstallmentData", installmentData.Select(x => new XElement("InstallmentDataInfo",
            //    new XAttribute("InstallmentType", x.Installment_Type ?? "1"),
            //    //new XAttribute("InstallmentType", x.Installment_Type),
            //    new XAttribute("InstallmentName", x.Installment_Name),
            //    new XAttribute("Amount", x.Amount),
            //    new XAttribute("DueDate", x.DueDate)))).ToString();
            var InstallmentStructureData = new XElement("InstallmentData", installmentData.Select(x => new XElement("InstallmentDataInfo",
             new XAttribute("InstallmentType", x.Installment_Type),
             new XAttribute("InstallmentName", x.Installment_Name),
             new XAttribute("Amount", x.Amount),
             new XAttribute("DueDate", x.DueDate)))).ToString();

            var res = db.Sp_Update_Installment_File_Plot_Comm(id, "PlotManagement", InstallmentStructureData);
            return Json(new { Status = true, Msg = "Updated Succesfully" });
        }
        public ActionResult EmployeeFile()
        {
            var res = db.Sp_Get_EmployeeFilePlotInfo().ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Employee Plots  ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(res);
        }
        public void UpdatePlot_Balances()
        {
            var res = db.Plots.Where(x => x.Status == "Registered").ToList();
            foreach (var item in res)
            {
                var res3 = db.Sp_Get_PlotInstallments(item.Id).ToList();
                var res4 = db.Sp_Get_ReceivedAmounts(item.Id, Modules.PlotManagement.ToString()).ToList();
                var discount = db.Discounts.Where(x => x.Module_Id == item.Id && x.Plot_Is_Cancelled == null && x.Module == Modules.PlotManagement.ToString()).ToList();
                UpdatePlotInstallmentStatus(res3, res4, discount, item.Id);
            }

        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //public ActionResult WHTPlotInstallmentAndReceiptsPartial(long Plotid)
        //{
        //    var uid = User.Identity.GetUserId<long>();
        //    var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
        //    ViewBag.Username = uname;
        //    var res4 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();

        //    var res5 = db.Sp_Get_PlotInstallments_Wht(Plotid).ToList();
        //    var res6 = db.Sp_Get_PlotInstallments(Plotid).ToList();
        //    var res7 = db.Plot_Installments_Surcharge.Where(x => x.Plot_Id == Plotid).ToList();
        //    var res = new PlotDetailData { PlotInstallmentsSurcharge = res7, PlotInstallments = res6, PlotInstallmentsWHT = res5, PlotReceipts = res4 };
        //    return PartialView(res);
        //}
        //public JsonResult WHTChargerPlotInstallment(long Plotid, String Status)
        //{
        //    double Prcntg = 0;
        //    double Charge = 0;
        //    DateTime july2021 = new DateTime(2021, 07, 1);
        //    DateTime june2022 = new DateTime(2022, 06, 30);
        //    DateTime july2022 = new DateTime(2022, 07, 1);
        //    DateTime june2023 = new DateTime(2023, 06, 30);

        //    if (Status == "Filer")
        //    {
        //        var installments = db.Plot_Installments.Where(x => x.Plot_Id == Plotid).ToList();
        //        var inst_Wht = db.Plot_Installments_Wht.Where(x => x.Plot_Id == Plotid).FirstOrDefault();
        //        foreach (var item in installments)
        //        {
        //            if ((item.DueDate > july2021) && (june2022 > item.DueDate))
        //            {
        //                Charge = 0.01;
        //                Prcntg = Convert.ToDouble(item.Amount) * (Charge);
        //            }
        //            else if (item.DueDate > july2022)
        //            {
        //                Charge = 0.02;
        //                Prcntg = Convert.ToDouble(item.Amount) * (Charge);
        //            }
        //            if (inst_Wht == null)
        //            {
        //                db.Sp_Add_plotInstallmet_WHT(Plotid, Convert.ToDecimal(Prcntg), item.Installment_Name, item.DueDate, item.Installment_Type, "Pending", Status, Charge.ToString());

        //            }
        //            else
        //            {
        //                db.Sp_Update_plotInstallmet_WHT(Plotid, Convert.ToDecimal(Prcntg), item.Installment_Name, Status, item.Status, Charge.ToString());
        //            }
        //        }
        //    }
        //    else if (Status == "Non-Filer")
        //    {
        //        var installments = db.Plot_Installments.Where(x => x.Plot_Id == Plotid).ToList();
        //        var inst_Wht = db.Plot_Installments_Wht.Where(x => x.Plot_Id == Plotid).FirstOrDefault();
        //        foreach (var item in installments)
        //        {
        //            if ((item.DueDate > july2021) && (june2022 > item.DueDate))
        //            {
        //                Charge = 0.02;
        //                Prcntg = Convert.ToDouble(item.Amount) * (Charge);
        //            }
        //            else if (item.DueDate > july2022)
        //            {
        //                Charge = 0.07;
        //                Prcntg = Convert.ToDouble(item.Amount) * (Charge);
        //            }
        //            if (inst_Wht == null)
        //            {
        //                db.Sp_Add_plotInstallmet_WHT(Plotid, Convert.ToDecimal(Prcntg), item.Installment_Name, item.DueDate, item.Installment_Type, "Pending", Status, Charge.ToString());

        //            }
        //            else
        //            {
        //                db.Sp_Update_plotInstallmet_WHT(Plotid, Convert.ToDecimal(Prcntg), item.Installment_Name, Status, item.Status, Charge.ToString());
        //            }
        //        }
        //    }
        //    return Json(true);
        //}
        //Meher Only
        //public JsonResult DataDumpDealershipMeherOnly()
        //{
        //    string plotNo = "";
        //    string sector = "";
        //    decimal Com_Amount = 0;
        //    decimal Sales_Amount = 0;
        //    Excel.Application xlApp = new Excel.Application();
        //    Excel.Workbook xlWorkbook = xlApp.Workbooks.Open("C:\\p21\\DataDump2.xlsx");
        //    Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
        //    Excel.Range xlRange = xlWorksheet.UsedRange;
        //    for (int i = 2; i <= xlRange.Rows.Count; i++)
        //    {
        //        plotNo = Convert.ToString(xlRange.Cells[i, 1].Value2);
        //        sector = Convert.ToString(xlRange.Cells[i, 2].Value2);
        //        Com_Amount = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 11].Value2));
        //        Sales_Amount = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 9].Value2));
        //        var plot = db.Plots.Where(a => a.Plot_Number == plotNo && a.Sector == sector).FirstOrDefault();
        //        if (plot.Status == "Registered")
        //        {
        //            var dealership = db.Dealerships.Where(x => x.Id == plot.Dealership_Submission_Id).FirstOrDefault();
        //            var dealers = db.Dealers.Where(x => x.Dealership_Id == plot.Dealership_Submission_Id).FirstOrDefault();
        //            var Grand_Total = db.Installment_Plot_Category.Where(a => a.Id == plot.Installment_Plan_Id).FirstOrDefault().Grand_Total;


        //            Dealer_Commession d = new Dealer_Commession()
        //            {
        //                Dealer_Id = dealers.Id,
        //                Dealer_Name = dealers.Name,
        //                File_Plot_Id = plot.Id,
        //                Com_Amount = Com_Amount,
        //                Com_Type = "Fixed",
        //                Module = Modules.PlotManagement.ToString(),
        //                Datetime = DateTime.Now,
        //                Com_Maturity = 100,
        //                Process = true,
        //                Plot_No = plot.Plot_Number,
        //                Plot_Type = plot.Type,
        //                Block = plot.Sector,
        //                Total_Price = Grand_Total
        //            };
        //            DealershipController dc = new DealershipController();
        //            dc.AddCommession(d);
        //        }
        //    }

        //    xlWorkbook.Close(false, Type.Missing, Type.Missing);
        //    Marshal.ReleaseComObject(xlWorkbook);
        //    Marshal.ReleaseComObject(xlApp);

        //    return null;
        //}
        //public JsonResult DataDumpMeherOnly()  
        //{
        //    double plotSize = 0;
        //    string plotNo = "";
        //    string sector = "";
        //    decimal rate = 0;
        //    decimal total = 0;
        //    decimal grandTotal = 0;
        //    DateTime dateTime = DateTime.Now;
        //    decimal DownPayment = 0;
        //    decimal Installments = 0;
        //    decimal ISRate = 0;
        //    decimal ISAmount = 0;
        //    decimal HalfYearly = 0;
        //    decimal Possession = 0;
        //    Plot_Ownership ownership = new Plot_Ownership();

        //    Excel.Application xlApp = new Excel.Application();
        //    Excel.Workbook xlWorkbook = xlApp.Workbooks.Open("C:\\p21\\DataDump.xlsx");
        //    Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
        //    Excel.Range xlRange = xlWorksheet.UsedRange;
        //    for (int i = 2; i <= xlRange.Rows.Count; i++)
        //    {
        //        plotNo = Convert.ToString(xlRange.Cells[i, 3].Value2);
        //        sector = Convert.ToString(xlRange.Cells[i, 4].Value2);
        //        var plot = db.Plots.Where(a => a.Plot_Number == plotNo && a.Sector == sector).FirstOrDefault();
        //        //if (plot != null)
        //        //{
        //        //    var _val = plot.Plot_Size.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //        //    plotSize = Convert.ToDouble(_val[0]);
        //        //}

        //        //rate = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 8].Value2));
        //        //total = rate * Convert.ToDecimal(plotSize);
        //        //grandTotal = total;
        //        //var dddd = Convert.ToDateTime(xlRange.Cells[i, 1].Value2);
        //        //dateTime = Convert.ToDateTime(Convert.ToString(xlRange.Cells[i, 1].Value2));

        //        //var Installment_Plot_Id = AddInstallmentCategoryMeherOnly(Convert.ToString(plotSize), rate, total, grandTotal, dateTime, plot.Block_Id);
        //        //if (plot != null)
        //        //{
        //        //    plot.Installment_Plan_Id = Installment_Plot_Id;
        //        //    db.SaveChanges();
        //        //}

        //        //DownPayment = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 14].Value2));
        //        //AddInstallmentStructureMeherOnly("Advance / Down Payment", "3", DownPayment, DownPayment, dateTime, Installment_Plot_Id, 0, null);

        //        //Installments = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 15].Value2));
        //        //AddInstallmentStructureMeherOnly("48 Installments", "1", 48, Installments * 48, dateTime, Installment_Plot_Id, 0, null);

        //        //HalfYearly = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 16].Value2));
        //        //AddInstallmentStructureMeherOnly("Half Yearly", "4", HalfYearly, HalfYearly * 8, dateTime, Installment_Plot_Id, 8, 6);

        //        //Possession = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 17].Value2));
        //        //AddInstallmentStructureMeherOnly("Possession", "0", Possession, Possession, dateTime, Installment_Plot_Id, 48, null);

        //        //ownership.Name = Convert.ToString(xlRange.Cells[i, 18].Value2);
        //        //ownership.Father_Husband = Convert.ToString(xlRange.Cells[i, 19].Value2);
        //        //ownership.CNIC_NICOP = Convert.ToString(xlRange.Cells[i, 20].Value2);
        //        //ownership.Date_Of_Birth = Convert.ToString(xlRange.Cells[i, 21].Value2);
        //        //ownership.Nationality = Convert.ToString(xlRange.Cells[i, 22].Value2);
        //        //ownership.Occupation = Convert.ToString(xlRange.Cells[i, 23].Value2);
        //        //ownership.Postal_Address = Convert.ToString(xlRange.Cells[i, 24].Value2);
        //        //ownership.Residential_Address = Convert.ToString(xlRange.Cells[i, 24].Value2);
        //        //ownership.Mobile_1 = Convert.ToString(xlRange.Cells[i, 26].Value2);
        //        //ownership.Mobile_2 = Convert.ToString(xlRange.Cells[i, 27].Value2);

        //        //ownership.Nominee_Name = Convert.ToString(xlRange.Cells[i, 28].Value2);
        //        //ownership.Nominee_CNIC_NICOP = Convert.ToString(xlRange.Cells[i, 30].Value2);
        //        //ownership.Nominee_Relation = Convert.ToString(xlRange.Cells[i, 31].Value2);
        //        //ownership.Nominee_Address = "Phone Number: " + Convert.ToString(xlRange.Cells[i, 32].Value2);

        //        var voucher_amt = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 11].Value2));

        //        RegisterDealerPlotMeherOnlyReceipts(plot.Id, plot.Installment_Plan_Id, voucher_amt);
        //    }

        //    xlWorkbook.Close(false, Type.Missing, Type.Missing);
        //    Marshal.ReleaseComObject(xlWorkbook);
        //    Marshal.ReleaseComObject(xlApp);

        //    return null;
        //}
        //public int AddInstallmentCategoryMeherOnly(string plotSize, decimal rate, decimal total, decimal grandTotal, DateTime dateTime, long blockId)
        //{
        //    Installment_Plot_Category IPC = new Installment_Plot_Category
        //    {
        //        Plot_Size = plotSize,
        //        Rate = rate,
        //        Total = total,
        //        Grand_Total = grandTotal,
        //        DateTime = dateTime,
        //        Block_Id = blockId,
        //        Module = "Plots"
        //    };

        //    db.Installment_Plot_Category.Add(IPC);
        //    db.SaveChanges();
        //    return IPC.Id;
        //}
        //public void AddInstallmentStructureMeherOnly(string installmentName, string installmentType, decimal rate, decimal amount, DateTime dateTime, int installmentPlotId, int? after, int? interval)
        //{
        //    Installment_Structure IS = new Installment_Structure
        //    {
        //        Installment_Name = installmentName,
        //        Installment_Type = installmentType,
        //        Rate = rate,
        //        Amount = amount,
        //        Datetime = dateTime,
        //        Installment_Plot_Id = installmentPlotId,
        //        After = after,
        //        Module = "Plots",
        //        Interval = interval,
        //    };
        //    db.Installment_Structure.Add(IS);
        //    db.SaveChanges();
        //}
        //public JsonResult RegisterDealerPlotMeherOnly(long? Plot_Id, long? InstallmentPlotId, Plot_Ownership Owners)
        //{
        //    Helpers h = new Helpers();
        //    AccountHandlerController ah = new AccountHandlerController();

        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var comp = ah.Company_Attr(userid);
        //    var plot = db.Sp_Get_PlotData(Plot_Id).FirstOrDefault();
        //    var res2 = db.Plots.Where(x => x.Id == Plot_Id).FirstOrDefault();
        //    var dealership = db.Dealerships.Where(x => x.Id == res2.Dealership_Id).FirstOrDefault();
        //    var dealers = db.Dealers.Where(x => x.Dealership_Id == res2.Dealership_Id).ToList();
        //    var Grand_Total = db.Installment_Plot_Category.Where(a => a.Id == InstallmentPlotId).FirstOrDefault().Grand_Total;

        //    Helpers H = new Helpers();
        //    using (var Transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            long GroupTag = h.RandomNumber();
        //            Owners.GroupTag = GroupTag;
        //            Owners.Ownership_DateTime = DateTime.Now;
        //            Owners.Ownership_Status = "Owner";
        //            db.Plot_Ownership.Add(Owners);
        //            db.SaveChanges();

        //            if (plot.Installment_Plan_Id == null)
        //            {
        //                //List<PlotsInstallments> PI = new List<PlotsInstallments>()
        //                //{
        //                //    new PlotsInstallments()
        //                //    {
        //                //        Amount = (decimal)res2.GrandTotal,
        //                //        DueDate = DateTime.Now,
        //                //        InstNo = "Advance"
        //                //    }
        //                //};
        //                //var Installments = new XElement("Plots", PI.Select(x => new XElement("Installments",
        //                //  new XAttribute("Amount", x.Amount),
        //                //  new XAttribute("DueDate", x.DueDate),
        //                //  new XAttribute("Installment_Name", x.InstNo)
        //                //  ))).ToString();
        //                //db.Sp_Update_PlotInstallments(Plot_Id, res2.GrandTotal, 0, userid, Installments);
        //            }
        //            else
        //            {
        //                var installmentstructure = db.Sp_Get_InstallmentStructure((int)plot.Installment_Plan_Id).ToList();
        //                List<Plot_Installments> plot_Installments = new List<Plot_Installments>();
        //                decimal rate = 0, total = 0, grandtotal = 0;
        //                var Date = DateTime.UtcNow;
        //                var PlotId = plot.Id;
        //                foreach (var item in installmentstructure.Where(x => x.Installment_Type == "3" || x.Installment_Type == "6"))
        //                {
        //                    if (item.Installment_Type == "3") // Development Charges
        //                    {
        //                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
        //                        DateTime b = Date;
        //                        Plot_Installments fi = new Plot_Installments()
        //                        {
        //                            Status = InstallmentPaymentStatus.Pending.ToString(),
        //                            DueDate = b,
        //                            Amount = item.Amount,
        //                            Plot_Id = PlotId,
        //                            Installment_Name = item.Installment_Name,
        //                            Installment_Type = "3",
        //                        };
        //                        plot_Installments.Add(fi);
        //                    }
        //                    else if (item.Installment_Type == "6")
        //                    {
        //                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
        //                        DateTime b = Date;
        //                        b = b.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
        //                        DateTime dt = new DateTime(b.Year, b.Month, 1);
        //                        Plot_Installments fi = new Plot_Installments()
        //                        {
        //                            Status = InstallmentPaymentStatus.Pending.ToString(),
        //                            DueDate = dt,
        //                            Amount = item.Amount,
        //                            Plot_Id = PlotId,
        //                            Installment_Name = item.Installment_Name,
        //                            Installment_Type = "6",
        //                        };
        //                        plot_Installments.Add(fi);
        //                    }
        //                }
        //                foreach (var item in installmentstructure.Where(x => x.Installment_Type == "4" || x.Installment_Type == "0"))
        //                {
        //                    if (item.Installment_Type == "4") // for Half yearly , Quarterly
        //                    {
        //                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
        //                        var eachinst = item.Amount / item.After;
        //                        var itemaft = item.Interval;
        //                        for (int i = 1; i <= item.After; i++)
        //                        {
        //                            DateTime b = Date;
        //                            var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
        //                            if (str != null) // to check for confirmation
        //                            {
        //                                b = str.DueDate;
        //                            }
        //                            b = b.AddMonths(Convert.ToInt16(itemaft));
        //                            DateTime dt = new DateTime(b.Year, b.Month, 1);

        //                            Plot_Installments fi = new Plot_Installments()
        //                            {
        //                                Status = InstallmentPaymentStatus.Pending.ToString(),
        //                                DueDate = dt,
        //                                Amount = Convert.ToDecimal(eachinst),
        //                                Plot_Id = PlotId,
        //                                Installment_Name = i + " - " + item.Installment_Name,
        //                                Installment_Type = "1",
        //                            };
        //                            plot_Installments.Add(fi);
        //                            itemaft += item.Interval;
        //                        }
        //                    }
        //                    else if (item.Installment_Type == "0")
        //                    {
        //                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;

        //                        DateTime b = Date;
        //                        var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
        //                        if (str != null) // to check for confirmation
        //                        {
        //                            b = str.DueDate;
        //                        }

        //                        b = b.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
        //                        DateTime dt = new DateTime(b.Year, b.Month, 1);
        //                        Plot_Installments fi = new Plot_Installments()
        //                        {
        //                            Status = InstallmentPaymentStatus.Pending.ToString(),
        //                            DueDate = dt,
        //                            Amount = item.Amount,
        //                            Plot_Id = PlotId,
        //                            Installment_Name = item.Installment_Name,
        //                            Installment_Type = "0",
        //                        };
        //                        plot_Installments.Add(fi);
        //                    }
        //                }
        //                foreach (var item in installmentstructure.Where(x => x.Installment_Type == "1"))
        //                {
        //                    if (item.Installment_Type == "1") // for type installments
        //                    {
        //                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
        //                        var eachinst = item.Amount / item.Rate;

        //                        for (int i = 1; i <= item.Rate; i++)
        //                        {
        //                            DateTime b = Date;
        //                            var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
        //                            if (str != null) // to check for confirmation
        //                            {
        //                                b = str.DueDate;
        //                            }
        //                            b = b.AddMonths(i);
        //                            var insRes = plot_Installments.Where(x => x.DueDate == new DateTime(b.Year, b.Month, 1)).FirstOrDefault();
        //                            while (insRes != null)
        //                            {
        //                                b = b.AddMonths(1);
        //                                insRes = plot_Installments.Where(x => x.DueDate == new DateTime(b.Year, b.Month, 1)).FirstOrDefault();
        //                            }
        //                            DateTime dt = new DateTime(b.Year, b.Month, 1);

        //                            Plot_Installments fi = new Plot_Installments()
        //                            {
        //                                Status = InstallmentPaymentStatus.Pending.ToString(),
        //                                DueDate = dt,
        //                                Amount = eachinst,
        //                                Plot_Id = PlotId,
        //                                Installment_Name = i + " Monthly Installment",
        //                                Installment_Type = "1",
        //                            };
        //                            plot_Installments.Add(fi);
        //                        }
        //                    }
        //                }

        //                var installmentplan = new XElement("Plots", plot_Installments.Select(x => new XElement("Installments",
        //                                        new XAttribute("Amount", x.Amount),
        //                                        new XAttribute("DueDate", x.DueDate),
        //                                        new XAttribute("Installment_Name", x.Installment_Name),
        //                                        new XAttribute("Installment_Type", x.Installment_Type),
        //                                        new XAttribute("Status", x.Status)
        //                                        ))).ToString();

        //                db.Sp_Update_PlotInstallments(plot.Id, 0, 0, 0, installmentplan);
        //            }

        //            db.Sp_Add_Activity(userid, "Register Plot Data of Plot Id. <a class='plt-data' data-id=' " + Plot_Id + "'>" + Plot_Id + "</a> To Name : " + Owners.Name, "Create", Modules.PlotManagement.ToString(), ActivityType.Add_Plot_Owner.ToString(), Plot_Id);
        //            db.Sp_Add_PlotComments(Plot_Id, "Register Plot to Owners : " + Owners.Name, userid, ActivityType.Add_Plot_Owner.ToString());

        //            var voucher_amt = Math.Round(Grand_Total, 2);

        //            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

        //            var res3 = db.Sp_Add_Receipt(voucher_amt, GeneralMethods.NumberToWords((int)voucher_amt), "", "", null, "", Owners.Mobile_1
        //                , Owners.Father_Husband, Plot_Id, Owners.Name, "Cash", Grand_Total,
        //                "SA Gardens", 0, null, plot.Plot_Size, ReceiptTypes.Booking.ToString(), userid, userid, "", null, Modules.PlotManagement.ToString(), "", plot.Plot_No, plot.Block_Name, plot.Type, GroupTag, h.RandomNumber(), res2.Dealership_Name, receiptno, comp.Id).FirstOrDefault();
        //            if (res3.Receipt_Id == -1)
        //            {
        //                Transaction.Rollback();
        //                return Json(new { Status = false, Msg = "Cannot Reregister the Plot again." });
        //            }
        //            db.SP_Update_PlotVerificationToNull(Plot_Id);
        //            //    Dealer_Commession d = new Dealer_Commession()
        //            //    {
        //            //        Module = Modules.FileManagement.ToString(),
        //            //        Com_Type = "Fixed",
        //            //        Com_Maturity = 25,
        //            //        Percentage = 0,
        //            //        File_Plot_Id = plot.Id,
        //            //        Com_Amount = 0,
        //            //        Dealer_Id = dealership.Id,
        //            //        Dealer_Name = dealership.Dealership_Name,
        //            //        Plot_No = plot.Plot_No,
        //            //        Plot_Type = plot.Type,
        //            //        Block = plot.Block_Name,
        //            //        Total_Price = Grand_Total
        //            //    };
        //            //    DealershipController dc = new DealershipController();
        //            //    dc.AddCommession(d);

        //            Transaction.Commit();
        //            this.updatebalance(plot.Id);
        //            return Json(new { Status = true, ReceiptId = res3.Receipt_No, Token = userid });
        //        }
        //        catch (Exception ex)
        //        {
        //            Transaction.Rollback();
        //            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString(), "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //            return Json(new { Status = false, Msg = "Error Occured" });
        //        }
        //    }
        //}
        //public JsonResult RegisterOwnerMeherOnly(long? Plot_Id, long? InstallmentPlotId, Plot_Ownership Owners)
        //{
        //    Helpers h = new Helpers();
        //    using (var Transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var plot = db.Sp_Get_PlotData(Plot_Id).FirstOrDefault();
        //            var Grand_Total = db.Installment_Plot_Category.Where(a => a.Id == InstallmentPlotId).FirstOrDefault().Grand_Total;

        //            long GroupTag = h.RandomNumber();
        //            Owners.GroupTag = GroupTag;
        //            Owners.Ownership_DateTime = DateTime.Now;
        //            Owners.Ownership_Status = "Owner";
        //            Owners.Plot_Id = Plot_Id;
        //            Owners.Plot_Size = plot.Plot_Size;
        //            Owners.Total_Price = Grand_Total;
        //            db.Plot_Ownership.Add(Owners);
        //            db.SaveChanges();

        //            Transaction.Commit();
        //            return Json(new { Status = true });
        //        }
        //        catch (Exception ex)
        //        {
        //            Transaction.Rollback();
        //            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString(), "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //            return Json(new { Status = false, Msg = "Error Occured" });
        //        }
        //    }
        //}
        //public JsonResult RegisterDealerPlotMeherOnlyReceipts(long Plot_Id, long? InstallmentPlotId, decimal voucher_amt)
        //{
        //    AccountHandlerController ah = new AccountHandlerController();
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var comp = ah.Company_Attr(userid);
        //    var plot = db.Sp_Get_PlotData(Plot_Id).FirstOrDefault();
        //    var res2 = db.Plots.Where(x => x.Id == Plot_Id).FirstOrDefault();
        //    var pl = db.Plot_Ownership.Where(a => a.Plot_Id == Plot_Id).FirstOrDefault();
        //    var Grand_Total = db.Installment_Plot_Category.Where(a => a.Id == InstallmentPlotId).FirstOrDefault().Grand_Total;

        //    Helpers h = new Helpers();
        //    using (var Transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

        //            var res3 = db.Sp_Add_Receipt(voucher_amt, GeneralMethods.NumberToWords((int)voucher_amt), "", "", null, "", pl.Mobile_1,
        //                pl.Father_Husband, Plot_Id, pl.Name, "Cash", Grand_Total, "MED", 0, null, plot.Plot_Size, ReceiptTypes.Booking.ToString(),
        //                userid, userid, "", null, Modules.PlotManagement.ToString(), "", plot.Plot_No, plot.Block_Name, plot.Type, pl.GroupTag, h.RandomNumber(),
        //                res2.Dealership_Name, receiptno, comp.Id).FirstOrDefault();


        //            if (res3.Receipt_Id == -1)
        //            {
        //                Transaction.Rollback();
        //                return Json(new { Status = false, Msg = "Cannot Reregister the Plot again." });
        //            }

        //            Transaction.Commit();
        //            this.updatebalance(Plot_Id);
        //            return Json(new { Status = true, ReceiptId = res3.Receipt_No });
        //        }
        //        catch (Exception ex)
        //        {
        //            Transaction.Rollback();
        //            db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString(), "", ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //            return Json(new { Status = false, Msg = "Error Occured" });
        //        }
        //    }
        //}

        public JsonResult DataDumpDealershipMeherOnly()   //comment for Microsoft.Office.Interop.Excel missing
        {
            string plotNo = "";
            string sector = "";
            decimal Com_Amount = 0;
            decimal Sales_Amount = 0;
            decimal Base_Amount = 0;
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open("C:\\p21\\DataDump2.xlsx");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            for (int i = 2; i <= xlRange.Rows.Count; i++)
            {
                plotNo = Convert.ToString(xlRange.Cells[i, 1].Value2);
                sector = Convert.ToString(xlRange.Cells[i, 2].Value2);
                Sales_Amount = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 9].Value2));
                Com_Amount = Convert.ToDecimal(Convert.ToString(xlRange.Cells[i, 11].Value2));
                Base_Amount = Sales_Amount - Com_Amount;


                var plot = db.Plots.Where(a => a.Plot_Number == plotNo && a.Sector == sector).FirstOrDefault();
                if (plot != null && plot.Status == "Registered")
                {
                    var plotOwnership = db.Plot_Ownership.Where(x => x.Plot_Id == plot.Id).FirstOrDefault();
                    Biding_Reserve_Plots biding_Reserve_Plots = new Biding_Reserve_Plots
                    {
                        Dealer_Id = (long)plot.Dealership_Submission_Id,
                        Plot_Id = plot.Id,
                        DealerName = plot.Dealership_Submission_Name,
                        PlotPrice = Base_Amount,
                        GroupTag = plotOwnership.GroupTag,
                        DealDate = plotOwnership.Ownership_DateTime,
                        AddedBy_Id = 1,
                        AddedBy_Name = "Testing User",
                        PlotNum = plot.Plot_Number + "-" + plot.Sector + "-" + plot.Type + " - " + plot.Sector + " ( " + plot.Plot_Location + " ) ",
                        PlotStatus = "Registered",
                        SpecialPrefAmount = 0,
                        DCAmount = 0,
                        CommisionAmount = Com_Amount,
                        Installments_Seg = null,
                        Percentage_Adj = 0
                    };

                    db.Biding_Reserve_Plots.Add(biding_Reserve_Plots);
                    db.SaveChanges();
                }
            }

            xlWorkbook.Close(false, Type.Missing, Type.Missing);
            Marshal.ReleaseComObject(xlWorkbook);
            Marshal.ReleaseComObject(xlApp);

            return null;
        }

        public JsonResult DD()
        {
            foreach (var item in db.Dealerships.ToList())
            {
                var b = db.Biding_Reserve_Plots.Where(a => a.Dealer_Id == item.Id).ToList();
                var c = b.Select(a => a.PlotNum).ToList();
                var d = b.Sum(a => a.PlotPrice);
                var e = b.Sum(a => a.CommisionAmount);
                var t = d + e;

                DealerDeal dealerDeal = new DealerDeal
                {
                    DealerId = item.Id,
                    DealerName = item.Dealership_Name,
                    GroupTag = b.Select(a => a.GroupTag).FirstOrDefault(),
                    Description = string.Join<string>(", ", c),
                    Amount = t,
                    Received = 0,
                    BalanceAmount = 0,
                    DateOfEntry = DateTime.Now,
                    DealTitle = item.Dealership_Name + " Deals"
                };
                db.DealerDeals.Add(dealerDeal);
                db.SaveChanges();
            }



            return null;
        }


    }
}
//public JsonResult BidingRegisterPlots(Plot_Ownership PO, bool Flag, long Dealerid, long[] PlotId, List<ReceiptData> Receiptdata, long TransactionId)
//{
//    long userid = long.Parse(User.Identity.GetUserId());
//    db.Sp_Add_Activity(userid, "Register Bidding Plot of Dealer " + Dealerid, "Create", Modules.PlotManagement.ToString(), ActivityType.Plot_Registeration.ToString(), Dealerid);
//    var PlotIds = PlotId.Select(x => Convert.ToInt64(x)).ToList();
//    var dealership = db.Dealerships.Where(x => x.Id == Dealerid).Select(x=> x.Dealership_Name).FirstOrDefault();
//    if (Flag)
//    {
//        ReceiptData rd = Receiptdata.FirstOrDefault();
//        decimal RemainingAmt = Receiptdata.FirstOrDefault().Amount;
//        List<string> ids = new List<string>();
//        var res1 = (from x in db.Plots
//                    join a in db.Plots_Category on x.Plot_Category equals a.Id
//                    join b in db.Plot_Rates on x.Id equals b.Plot_Com_Id
//                    where PlotIds.Contains(x.Id)
//                    select new { PlotId = x.Id, PlotNumber = x.Plot_Number, x.Sector, Size = x.Plot_Size, x.Type, x.Block_Name, Calc_Size = a.Plot_Size, Rate = b.RatePerMarla_SqFt }).ToList();
//        foreach (var item in res1)
//        {
//            var size = Convert.ToDecimal(item.Size.Split(' ')[0]);
//            var Totalval = size * item.Rate;
//            var advance = (Totalval * 25) / 100;
//            var RemainAmt = Totalval - advance;
//            PlotsInstallments pi1 = new PlotsInstallments()
//            {
//                Amount = Convert.ToDecimal(advance),
//                DueDate = DateTime.Now,
//                InstNo = "25% Advance"
//            };
//            PlotsInstallments pi2 = new PlotsInstallments()
//            {
//                Amount = Convert.ToDecimal(RemainAmt),
//                DueDate = DateTime.Now.AddMonths(2),
//                InstNo = "75% Installment"
//            };
//            List<PlotsInstallments> PIs = new List<PlotsInstallments>();
//            PIs.Add(pi1);
//            PIs.Add(pi2);
//            var Installments = new XElement("Plots", PIs.Select(x => new XElement("Installments",
//                                 new XAttribute("Amount", x.Amount),
//                                 new XAttribute("DueDate", x.DueDate),
//                                 new XAttribute("Installment_Name", x.InstNo)
//                                 ))).ToString();
//            Helpers h = new Helpers();
//            var Owner_Id = db.Sp_Add_PlotOwnerShip(PO.Plot_Size, PO.Name, PO.Father_Husband, PO.CNIC_NICOP, PO.Mobile_1,
//                           PO.Mobile_2, PO.Phone_Office, PO.Residential, PO.Postal_Address, PO.Residential_Address,
//                           PO.Email, PO.Occupation, PO.Nationality, PO.Date_Of_Birth, PO.Nominee_Name,
//                           PO.Nominee_Relation, PO.Nominee_Address, PO.Nominee_CNIC_NICOP, item.PlotId, PO.City, Totalval,
//                           PO.Owner_Img, PO.Discount, userid, DateTime.Now, "Owner", h.RandomNumber(), Installments).FirstOrDefault();
//            SmsService smsService = new SmsService();
//            if (RemainAmt >= advance)
//            {
//                if (rd.PaymentType == "Online")
//                    rd.PaymentType = rd.PaymentType + "(Bank Acc No)";
//                var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
//                var res = db.Sp_Add_Receipt(advance, GeneralMethods.NumberToWords(Convert.ToInt32(advance)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, PO.Mobile_1
//                                    , PO.Father_Husband, item.PlotId, PO.Name, rd.PaymentType, 0,
//                                    rd.Project_Name, 0, null, item.Size, ReceiptTypes.Booking.ToString(), userid, userid, "Plot Booking", null, Modules.PlotManagement.ToString(), Dealerid.ToString(), item.PlotNumber + " " + item.Sector, item.Block_Name, item.Type, Owner_Id, TransactionId, dealership, receiptno).FirstOrDefault();
//                ids.Add(res.Receipt_No);
//                var text = "Dear " + PO.Name + ",\n\r" +
//                "A Payment of Rs " + string.Format("{0:n0}", advance) + " has been received in cash for Plot number# " + rd.File_Plot_Number + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
//                try
//                {
//                    smsService.SendMsg(text, rd.Mobile_1);
//                }
//                catch (Exception)
//                {
//                }
//            }
//        }
//        var data = new { Status = "1", Receiptid = Dealerid, Token = userid };
//        return Json(data);
//    }
//    else
//    {
//        var res1 = (from x in db.Plots
//                    join a in db.Plots_Category on x.Plot_Category equals a.Id
//                    join b in db.Plot_Rates on x.Id equals b.Plot_Com_Id
//                    where PlotIds.Contains(x.Id)
//                    select new BidPlotinfo { PlotId = x.Id, PlotNumber = x.Plot_Number, Sector = x.Sector, Type = x.Type, Block = x.Block_Name, Size = x.Plot_Size, Calc_Size = a.Plot_Size, Rate = b.RatePerMarla_SqFt, Stat = false }).ToList();
//        List<string> ids = new List<string>();
//        List<ReceiptData> Holdedreceipt = new List<ReceiptData>();
//        foreach (var rd in Receiptdata) // Receipts
//        {
//            decimal RemainingAmt = rd.Amount;
//            string PlotDetails = "";
//            foreach (var b in res1.Where(x => x.Stat == false)) // Plots
//            {
//                var size = Convert.ToDecimal(b.Size.Split(' ')[0]);
//                var PlotTotalval = size * b.Rate;
//                var advance = (PlotTotalval * 25) / 100;
//                var amt = (Holdedreceipt.Any()) ? Holdedreceipt.Sum(x => x.Amount) : 0;
//                decimal Totalamt = amt + RemainingAmt;
//                RemainingAmt = Totalamt - Convert.ToDecimal(advance);
//                if (RemainingAmt >= 0)
//                {
//                    var plotdetails = new Plot_Ownership
//                    {
//                        Plot_Size = PO.Plot_Size,
//                        Name = PO.Name,
//                        Father_Husband = PO.Father_Husband,
//                        Postal_Address = PO.Postal_Address,
//                        Residential_Address = PO.Residential_Address,
//                        Phone_Office = PO.Phone_Office,
//                        Residential = PO.Residential,
//                        Mobile_1 = PO.Mobile_1,
//                        Mobile_2 = PO.Mobile_2,
//                        Email = PO.Email,
//                        Occupation = PO.Occupation,
//                        Nationality = PO.Nationality,
//                        Date_Of_Birth = PO.Date_Of_Birth,
//                        CNIC_NICOP = PO.CNIC_NICOP,
//                        Nominee_Name = PO.Nominee_Name,
//                        Nominee_Relation = PO.Nominee_Relation,
//                        Nominee_Address = PO.Nominee_Address,
//                        Nominee_CNIC_NICOP = PO.Nominee_CNIC_NICOP,
//                        Id = b.PlotId,
//                        City = PO.City,
//                        Total_Price = PlotTotalval
//                    };
//                    PlotDetails = PlotDetails + JsonConvert.SerializeObject(plotdetails);
//                    var res2 = db.Sp_Add_Amount_Clearance(Totalamt, PlotDetails, Modules.PlotManagement.ToString(), Types.Booking.ToString()).FirstOrDefault();
//                    if (!Holdedreceipt.Any())
//                    {
//                        var res3 = db.Sp_Add_Receipt(advance, GeneralMethods.NumberToWords(Convert.ToInt32(advance)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, PO.Mobile_1
//                                                , PO.Father_Husband, b.PlotId, PO.Name, rd.PaymentType, 0,
//                                                rd.Project_Name, 0, null, b.Size, ReceiptTypes.Booking.ToString(), userid, userid, "Plot Booking", null, Modules.PlotManagement.ToString(), Dealerid.ToString(), b.PlotNumber + " " + b.Sector, b.Block, b.Type, 0, TransactionId, dealership, receiptno).FirstOrDefault();
//                        if (rd.PaymentType != "Cash")
//                        {
//                            var res4 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
//                                Modules.PlotManagement.ToString(), Types.Booking.ToString(), res2, rd.PayChqNo, res2, rd.Ch_bk_Pay_Date, b.PlotNumber + " " + b.Sector, res3.Receipt_Id).FirstOrDefault());
//                        }
//                    }
//                    else
//                    {
//                        foreach (var item in Holdedreceipt)
//                        {
//                            var res3 = db.Sp_Add_Receipt(item.Amount, item.AmountInWords, item.Bank, item.PayChqNo, item.Ch_bk_Pay_Date, item.Branch, PO.Mobile_1
//                                                    , PO.Father_Husband, b.PlotId, PO.Name, item.PaymentType, 0,
//                                                    item.Project_Name, 0, null, PO.Plot_Size, ReceiptTypes.Booking.ToString(), userid, userid, "Plot Booking", null, Modules.PlotManagement.ToString(), Dealerid.ToString(), b.PlotNumber + " " + b.Sector, b.Block, b.Type, 0, TransactionId, dealership, receiptno).FirstOrDefault();
//                            if (item.PaymentType != "Cash")
//                            {
//                                var res4 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(item.Amount, item.Bank, item.Branch, item.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
//                                    Modules.PlotManagement.ToString(), Types.Booking.ToString(), res2, item.PayChqNo, res2, item.Ch_bk_Pay_Date, b.PlotNumber + " " + b.Sector, res3.Receipt_Id).FirstOrDefault());
//                            }
//                        }
//                        Holdedreceipt.Clear();
//                        {
//                            var finalamt = rd.Amount - RemainingAmt;
//                            var res3 = db.Sp_Add_Receipt(finalamt, GeneralMethods.NumberToWords(Convert.ToInt32(finalamt)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, PO.Mobile_1
//                                                    , PO.Father_Husband, b.PlotId, PO.Name, rd.PaymentType, 0,
//                                                    rd.Project_Name, 0, null, b.Size, ReceiptTypes.Booking.ToString(), userid, userid, "Plot Booking", null, Modules.PlotManagement.ToString(), Dealerid.ToString(), b.PlotNumber + " " + b.Sector, b.Block, b.Type, 0, TransactionId, dealership, receiptno).FirstOrDefault();
//                            if (rd.PaymentType != "Cash")
//                            {
//                                var res4 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(finalamt, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
//                                    Modules.PlotManagement.ToString(), Types.Booking.ToString(), res2, rd.PayChqNo, res2, rd.Ch_bk_Pay_Date, b.PlotNumber + " " + b.Sector, res3.Receipt_Id).FirstOrDefault());
//                            }
//                        }
//                    }
//                    b.Stat = true;
//                }
//                else
//                {
//                    ReceiptData hold = new ReceiptData()
//                    {
//                        Amount = Totalamt,
//                        AmountInWords = GeneralMethods.NumberToWords(Convert.ToInt32(Totalamt)),
//                        Bank = rd.Bank,
//                        Block_Name = rd.Block_Name,
//                        Branch = rd.Branch,
//                        Block = rd.Block,
//                        Category = rd.Category,
//                        TotalAmount = rd.TotalAmount,
//                        Ch_bk_Pay_Date = rd.Ch_bk_Pay_Date,
//                        Date = rd.Date,
//                        Father_Husband = rd.Father_Husband,
//                        Mobile_1 = rd.Mobile_1,
//                        PayChqNo = rd.PayChqNo,
//                        PaymentType = rd.PaymentType,
//                        Name = rd.Name
//                    };
//                    Holdedreceipt.Add(hold);
//                    break;
//                }
//            }
//        }
//        var data = new { Status = "2", Receiptid = Dealerid, Token = userid };
//        return Json(data);
//    }
//}