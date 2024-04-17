using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using MeherEstateDevelopers.Filters;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class BallotingController : Controller
    {
        // GET: Balloting
        //public static List<KioskServer> kioskServers = new List<KioskServer>();

        private Grand_CityEntities db = new Grand_CityEntities();

        public ActionResult BallotingReport()
        {
            var res = db.Sp_Get_Report_Balloting_Est().ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Balloting Report Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return View(res);
        }

        public ActionResult Ballot()
        {
            var blks = db.BallotPlots.Select(x => x.Block).Distinct().ToList();
            var sects = db.BallotPlots.Select(x => x.Sector).Distinct().OrderBy(x => x).ToList();
            ViewBag.blocks = blks;
            ViewBag.sectors = sects;
            return View();
        }

        public JsonResult PerformBallot(BallotReqtModel[] reqPlots, int? pdInst, DateTime? bookStart, DateTime? bookEnd, int? threshold, bool? allAvailable)
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Performed Balloting ", "Create", "Activity_Record", ActivityType.Balloting.ToString(), userid);
            List<BallotPlot> plotsInHand = new List<BallotPlot>();
            List<BallotResultModel> ballotResult = new List<BallotResultModel>();
            List<PreferenceFilteredPlots> UnballottedPlots = new List<PreferenceFilteredPlots>();
            List<File_Form> tempFileCollection = db.File_Form.ToList();
            List<BallotSummary> plotsSummaryData = new List<BallotSummary>();
            List<BallotSummary> filesSummaryData = new List<BallotSummary>();

            //var uid = User.Identity.GetUserId<long>();
            //var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();

            try
            {
                if (allAvailable == true)
                {
                    var foundPlots = db.BallotPlots.Where(x => (x.Status == "Available")).ToList();
                    plotsInHand.AddRange(foundPlots);
                }
                else
                {
                    foreach (var v in reqPlots)
                    {
                        var foundPlots = db.BallotPlots.Where(x => (x.Block == v.block) && (v.sector == x.Sector) && (x.Status == "Available") && ((x.PlotSerial >= v.start) && (x.PlotSerial <= v.end))).ToList();
                        plotsInHand.AddRange(foundPlots);
                    }
                }

                var qlfying = db.Sp_Get_Ballot_Qualifying_File().ToList();

                //var skls = new XElement("Files", qlfying.GroupBy(x => x.File_Id).Select(x => new XElement("Ids",
                //      new XAttribute("FileId", x.Key)))).ToString();

                //var firstInsts = db.Sp_Get_1stMonth_Installments_Dates(skls).ToList();

                //--------------- Balloting process
                //--------------- Filter on prefrence
                //--------------- Filter on booking date

                //Filtering out Plots First
                List<PreferenceFilteredPlots> filteredPlots = new List<PreferenceFilteredPlots>();
                foreach (var item in plotsInHand.GroupBy(x => x.PlotArea))
                {
                    int pltsz_temp = Convert.ToInt32(item.Key.Value);
                    foreach (var v in item.GroupBy(x => x.PlotType))
                    {
                        int prty = 0;
                        if (v.Key.Contains("Normal"))
                        {
                            prty = 5;
                        }
                        else if (v.Key.Contains("Boulevard"))
                        {
                            prty = 3;
                        }
                        else if (v.Key.Contains("Corner"))
                        {
                            prty = 2;
                        }
                        else if (v.Key.Contains("Boulevard Corner"))
                        {
                            prty = 1;
                        }
                        else if (v.Key.Contains("Park Facing"))
                        {
                            prty = 4;
                        }
                        filteredPlots.Add(new PreferenceFilteredPlots
                        {
                            Prefrence = v.Key,
                            Plots = v.OrderBy(x => x.PlotSerial).ToList(),
                            Priority = prty,
                            PlotSize = pltsz_temp
                        });
                    }
                }

                List<PreferenceFilteredFiles> filteredFiles = new List<PreferenceFilteredFiles>();
                foreach (var item in qlfying.GroupBy(x => x.Plot_Size))
                {
                    int pltsz_temp = int.Parse(item.Key.Split(' ')[0]);
                    foreach (var v in item.GroupBy(x => x.Plot_Prefrence))
                    {
                        int prty = 0;
                        if (v.Key == "Normal")
                        {
                            prty = 5;
                        }
                        else if (v.Key == "Boulevard")
                        {
                            prty = 3;
                        }
                        else if (v.Key == "Corner")
                        {
                            prty = 2;
                        }
                        else if (v.Key == "Boulevard Corner")
                        {
                            prty = 1;
                        }
                        else if (v.Key == "Park Facing")
                        {
                            prty = 4;
                        }
                        filteredFiles.Add(new PreferenceFilteredFiles
                        {
                            Prefrence = v.Key,
                            Files = v.OrderBy(x => x.Due_Date).ToList(),
                            Priority = prty,
                            PlotSize = pltsz_temp
                        });
                    }
                }

                foreach (var __check in filteredPlots)
                {
                    plotsSummaryData.Add(new BallotSummary
                    {
                        Count = __check.Plots.Count(),
                        Ident = __check.PlotSize + " Marla ( " + __check.Prefrence + " )"
                    });
                }

                foreach (var __check in filteredFiles)
                {
                    filesSummaryData.Add(new BallotSummary
                    {
                        Count = __check.Files.Count(),
                        Ident = __check.PlotSize + " Marla ( " + __check.Prefrence + " )"
                    });
                }
                Random _randomizer = new Random();
                filteredFiles = filteredFiles.OrderBy(x => x.Priority).ToList();
                foreach (var prefType in filteredFiles)
                {
                    //Balloting Boulevard Corner Files First (Random)
                    //prefType.Files = GroupCnicsTogether(prefType.Files);
                    int ttlRun = prefType.Files.Count;
                    if (prefType.Prefrence == "Boulevard Corner")
                    {
                        var __temp_bc = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_bc is null) ? null : __temp_bc.Plots;
                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                        }
                        else
                        {
                            for (int i = 0; i < ttlRun && (selectedPlots != null); i++)
                            {
                                int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0)
                                {
                                    selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    var fileData = prefType.Files.ElementAt(0);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo,
                                        Due_Date = (DateTime)fileData.BookingDate,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory

                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);


                                    //if((i==(ttlRun-1)) && selectedPlots.Count >0)
                                    //{

                                    //}
                                    prefType.Files.RemoveAt(selectedFile);
                                    selectedPlots.RemoveAt(0);

                                    if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0
                                        });
                                    }
                                }
                                else
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Corner (Random)
                    if (prefType.Prefrence == "Corner")
                    {
                        var __temp_c = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_c is null) ? null : __temp_c.Plots;
                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                        }
                        else
                        {
                            for (int i = 0; i < ttlRun && selectedPlots != null; i++)
                            {
                                int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0)
                                {
                                    selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    var fileData = prefType.Files.ElementAt(selectedFile);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector + " ( " + plotsData.Block + " )",
                                        Due_Date = (DateTime)fileData.BookingDate,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);

                                    prefType.Files.RemoveAt(selectedFile);
                                    selectedPlots.RemoveAt(0);
                                    fileData = null;
                                    plotsData = null;
                                    if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0
                                        });
                                    }
                                }
                                else
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Boulevard (Random)
                    if (prefType.Prefrence == "Boulevard")
                    {
                        var __temp_b = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_b is null) ? null : __temp_b.Plots;

                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                        }
                        else
                        {
                            for (int i = 0; i < ttlRun && (selectedPlots != null); i++)
                            {
                                int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0)
                                {
                                    selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    var fileData = prefType.Files.ElementAt(selectedFile);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector + " ( " + plotsData.Block + " )",
                                        Due_Date = (DateTime)fileData.BookingDate,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = fileData.BookingDate.Value.ToShortDateString(),
                                        FirstInstallment = fileData.First_Installment.Value.ToShortDateString()
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);

                                    prefType.Files.RemoveAt(selectedFile);
                                    selectedPlots.RemoveAt(0);
                                    if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0
                                        });
                                    }
                                }
                                else
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Park Facing (Random)
                    if (prefType.Prefrence == "Park Facing")
                    {
                        var __temp_pf = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_pf is null) ? null : __temp_pf.Plots;
                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                        }
                        else
                        {
                            for (int i = 0; i < ttlRun && (selectedPlots != null); i++)
                            {
                                int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0 && prefType.Files.Count() > 0)
                                {
                                    selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    var fileData = prefType.Files.ElementAt(selectedFile);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector + " ( " + plotsData.Block + " )",
                                        Due_Date = (DateTime)fileData.Due_Date,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = fileData.BookingDate.Value.ToShortDateString(),
                                        FirstInstallment = fileData.First_Installment.Value.ToShortDateString()
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);

                                    prefType.Files.RemoveAt(selectedFile);
                                    selectedPlots.RemoveAt(0);
                                    if ((prefType.Files.Count() <= 0) && (selectedPlots.Count > 0))
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0
                                        });
                                    }
                                }
                                else if ((prefType.Files.Count() > 0) && (selectedPlots.Count <= 0))
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Normal Files (Not Random)
                    if (prefType.Prefrence == "Normal")
                    {
                        var __selection_of_plots__ = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__selection_of_plots__ is null) ? null : __selection_of_plots__.Plots;
                        if (selectedPlots != null)
                        {
                            prefType.Files = prefType.Files.OrderBy(x => x.Due_Date).ToList();
                            for (int i = 0; i < ttlRun && (selectedPlots.Count > 0); i++)
                            {
                                if ((selectedPlots.Count > 0) && (prefType.Files.Count() > 0))
                                {
                                    var fileData = prefType.Files.ElementAt(0);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __tempbr = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector + " ( " + plotsData.Block + " )",
                                        Due_Date = (DateTime)fileData.Due_Date,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = fileData.BookingDate.Value.ToShortDateString(),
                                        FirstInstallment = fileData.First_Installment.Value.ToShortDateString()
                                    };
                                    ballotResult.Add(__tempbr);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __tempbr.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __tempbr.File_Id);

                                    prefType.Files.RemoveAt(0);
                                    selectedPlots.RemoveAt(0);
                                    if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

             

                ballotResult.ForEach(x => x.BookingDate = x.Due_Date.ToShortDateString());
                //foreach(var v in ballotResult)
                //{
                //    v.FileNo = db.File_Form.Where(x => x.Id == v.File_Id).Select(x => x.FileFormNumber).FirstOrDefault().ToString();
                //}

                List<Sp_Get_Ballot_Qualifying_File_Result> remainderFiles = new List<Sp_Get_Ballot_Qualifying_File_Result>();

                foreach (var remFile in filteredFiles)
                {
                    remainderFiles.AddRange(remFile.Files);
                }

                remainderFiles.Where(x => x.BookingDate != null).ToList().ForEach(x => x.Mobile_2 = x.BookingDate.Value.ToShortDateString());

                var ret_res = Json(new BallotOutputModel { Status = true, Msg = "Ballot performed successfully.", ballotFiles = ballotResult, unballotedPlots = UnballottedPlots, unballotedFiles = remainderFiles, filesSummary = filesSummaryData, plotSummary = plotsSummaryData });
                ret_res.MaxJsonLength = int.MaxValue;
                return ret_res;
            }
            catch (Exception ex)
            {
                return Json(new BallotOutputModel { Status = false, Msg = ex.Message, ballotFiles = new List<BallotResultModel>(), unballotedPlots = new List<PreferenceFilteredPlots>(), unballotedFiles = new List<Sp_Get_Ballot_Qualifying_File_Result>(), filesSummary = new List<BallotSummary>(), plotSummary = new List<BallotSummary>() });
            }
        }

        public ActionResult BallotResult(BallotReqtModel[] reqPlots, int? pdInst, DateTime? bookStart, DateTime? bookEnd, int? threshold, bool? allAvailable)
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed BAlloting Results  Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            List<BallotPlot> plotsInHand = new List<BallotPlot>();
            List<BallotResultModel> ballotResult = new List<BallotResultModel>();
            List<PreferenceFilteredPlots> UnballottedPlots = new List<PreferenceFilteredPlots>();
            List<File_Form> tempFileCollection = db.File_Form.ToList();
            List<BallotSummary> plotsSummaryData = new List<BallotSummary>();
            List<BallotSummary> heldPlotsSummaryData = new List<BallotSummary>();
            List<BallotSummary> filesSummaryData = new List<BallotSummary>();
            List<BallotComparativeReport> BallotComparator = new List<BallotComparativeReport>();
            //var uid = User.Identity.GetUserId<long>();
            //var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();

            var backupAvlbl = db.BallotPlots.Where(x => (x.Status == "Available")).ToList();
            var backuphold = db.BallotPlots.Where(x => (x.Status == "Hold")).ToList();

            try
            {
                if (allAvailable == true)
                {
                    var foundPlots = db.BallotPlots.Where(x => (x.Status == "Available")).ToList();
                    plotsInHand.AddRange(foundPlots);
                }
                else
                {
                    foreach (var v in reqPlots)
                    {
                        var foundPlots = db.BallotPlots.Where(x => (x.Block == v.block) && (v.sector == x.Sector) && (x.Status == "Available") && ((x.PlotSerial >= v.start) && (x.PlotSerial <= v.end))).ToList();
                        plotsInHand.AddRange(foundPlots);
                    }
                }


                var qlfying = db.Sp_Get_Ballot_Qualifying_File().ToList();

                var backupQlfying = db.Sp_Get_Ballot_Qualifying_File().ToList();
                var qlfying_temp_size_range = qlfying.Select(x => x.Plot_Size).Distinct().ToList();
                List<double?> sizes_converted = new List<double?>();
                for (int i = 0; i < qlfying_temp_size_range.Count(); i++)
                {
                    string[] _temp_fl_sz = qlfying_temp_size_range[i].Split(' ');
                    if (_temp_fl_sz.Length > 1)
                    {
                        sizes_converted.Add(double.Parse(_temp_fl_sz[0]));
                    }
                }
                var heldPlots = db.BallotPlots.Where(x => (x.Status == "Hold") && sizes_converted.Contains(x.PlotArea)).ToList();
                //var skls = new XElement("Files", qlfying.GroupBy(x => x.File_Id).Select(x => new XElement("Ids",
                //      new XAttribute("FileId", x.Key)))).ToString();

                //var firstInsts = db.Sp_Get_1stMonth_Installments_Dates(skls).ToList();

                //--------------- Balloting process
                //--------------- Filter on prefrence
                //--------------- Filter on booking date

                //Filtering out Plots First
                List<PreferenceFilteredPlots> filteredPlots = new List<PreferenceFilteredPlots>();
                foreach (var item in plotsInHand.GroupBy(x => x.PlotArea))
                {
                    int pltsz_temp = Convert.ToInt32(item.Key.Value);
                    foreach (var v in item.GroupBy(x => x.PlotType))
                    {
                        int prty = 0;
                        if (v.Key.Contains("Boulevard Corner"))
                        {
                            prty = 1;
                        }
                        else if (v.Key.Contains("Corner"))
                        {
                            prty = 2;
                        }
                        else if (v.Key.Contains("Boulevard"))
                        {
                            prty = 3;
                        }
                        else if (v.Key.Contains("Park Facing"))
                        {
                            prty = 4;
                        }
                        else if (v.Key.Contains("Corner & Park Facing"))
                        {
                            prty = 5;
                        }
                        else if (v.Key.Contains("Normal"))
                        {
                            prty = 6;
                        }
                        
                        filteredPlots.Add(new PreferenceFilteredPlots
                        {
                            Prefrence = v.Key,
                            Plots = v.OrderBy(x => x.PlotSerial).ToList(),
                            Priority = prty,
                            PlotSize = pltsz_temp
                        });
                    }
                }

                //Filtering out Qualifying Files
                List<PreferenceFilteredFiles> filteredFiles = new List<PreferenceFilteredFiles>();
                foreach (var item in qlfying.GroupBy(x => x.Plot_Size))
                {
                    int pltsz_temp = int.Parse(item.Key.Split(' ')[0]);
                    filteredFiles.Add(new PreferenceFilteredFiles
                    {
                        Prefrence = "Boulevard Corner",
                        Files = item.Where(x => x.Plot_Prefrence == "Boulevard Corner").OrderBy(x => x.Due_Date).ToList(),
                        Priority = 1,
                        PlotSize = pltsz_temp
                    });
                    filteredFiles.Add(new PreferenceFilteredFiles
                    {
                        Prefrence = "Corner",
                        Files = item.Where(x => x.Plot_Prefrence == "Corner").OrderBy(x => x.Due_Date).ToList(),
                        Priority = 2,
                        PlotSize = pltsz_temp
                    });
                    filteredFiles.Add(new PreferenceFilteredFiles
                    {
                        Prefrence = "Boulevard",
                        Files = item.Where(x => x.Plot_Prefrence == "Boulevard").OrderBy(x => x.Due_Date).ToList(),
                        Priority = 3,
                        PlotSize = pltsz_temp
                    });
                    filteredFiles.Add(new PreferenceFilteredFiles
                    {
                        Prefrence = "Park Facing",
                        Files = item.Where(x => x.Plot_Prefrence == "Park Facing").OrderBy(x => x.Due_Date).ToList(),
                        Priority = 4,
                        PlotSize = pltsz_temp
                    });
                    filteredFiles.Add(new PreferenceFilteredFiles
                    {
                        Prefrence = "Corner & Park Facing",
                        Files = item.Where(x => x.Plot_Prefrence == "Corner & Park Facing").OrderBy(x => x.Due_Date).ToList(),
                        Priority = 5,
                        PlotSize = pltsz_temp
                    });
                    filteredFiles.Add(new PreferenceFilteredFiles
                    {
                        Prefrence = "Normal",
                        Files = item.Where(x => x.Plot_Prefrence == "Normal").OrderBy(x => x.Due_Date).ToList(),
                        Priority = 6,
                        PlotSize = pltsz_temp
                    });
                    
                    
                }

                //Filtering out Hold Plots
                List<PreferenceFilteredPlots> filteredHeldPlots = new List<PreferenceFilteredPlots>();
                foreach (var item in heldPlots.GroupBy(x => x.PlotArea))
                {
                    int pltsz_temp = Convert.ToInt32(item.Key.Value);
                    foreach (var v in item.GroupBy(x => x.PlotType))
                    {
                        int prty = 0;
                        if (v.Key.Contains("Boulevard Corner"))
                        {
                            prty = 1;
                        }
                        else if (v.Key.Contains("Corner"))
                        {
                            prty = 2;
                        }
                        else if (v.Key.Contains("Boulevard"))
                        {
                            prty = 3;
                        }
                        else if (v.Key.Contains("Park Facing"))
                        {
                            prty = 4;
                        }
                        else if (v.Key.Contains("Corner & Park Facing"))
                        {
                            prty = 5;
                        }
                        if (v.Key.Contains("Normal"))
                        {
                            prty = 6;
                        }
                        filteredHeldPlots.Add(new PreferenceFilteredPlots
                        {
                            Prefrence = v.Key,
                            Plots = v.OrderBy(x => x.PlotSerial).ToList(),
                            Priority = prty,
                            PlotSize = pltsz_temp
                        });
                    }
                }

                foreach (var __check in filteredPlots)
                {
                    plotsSummaryData.Add(new BallotSummary
                    {
                        Count = __check.Plots.Count(),
                        Ident = __check.PlotSize + " Marla ( " + __check.Prefrence + " )"
                    });
                }

                foreach (var __check in filteredFiles)
                {
                    filesSummaryData.Add(new BallotSummary
                    {
                        Count = __check.Files.Count(),
                        Ident = __check.PlotSize + " Marla ( " + __check.Prefrence + " )"
                    });
                }

                foreach (var __check in filteredHeldPlots)
                {
                    heldPlotsSummaryData.Add(new BallotSummary
                    {
                        Count = __check.Plots.Count(),
                        Ident = __check.PlotSize + " Marla ( " + __check.Prefrence + " )"
                    });
                }

                Random _randomizer = new Random();
                filteredFiles = filteredFiles.OrderBy(x => x.Priority).ToList();
                foreach (var prefType in filteredFiles)
                {
                    //Balloting Boulevard Corner Files First (Random)
                    int ttlRun = prefType.Files.Count;
                    if (prefType.Prefrence == "Boulevard Corner")
                    {
                        var __temp_bc = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_bc is null) ? null : __temp_bc.Plots;
                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            if (filteredFiles.Where(x => x.Prefrence == "Corner" && x.PlotSize == prefType.PlotSize).FirstOrDefault() == null)
                            {
                                prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                            }
                            else
                            {
                                prefType.Files.ForEach(x => x.Plot_Prefrence = "Corner");
                                filteredFiles.Where(x => x.Prefrence == "Corner" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                            }
                        }
                        else
                        {
                            prefType.Files = prefType.Files.OrderBy(x => x.BookingDate).ToList();
                            selectedPlots = selectedPlots.OrderBy(x => x.PlotSerial).ToList();
                            for (int i = 0; i < ttlRun && (selectedPlots != null); i++)
                            {
                                //int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0)
                                {
                                    //selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    var fileData = prefType.Files.ElementAt(0);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector,
                                        Due_Date = DateTime.Now,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = "",
                                        FirstInstallment = (fileData.First_Installment == null) ? "02/12/2018" : fileData.First_Installment.Value.ToShortDateString(),
                                        BalanceAmount = fileData.Balance_Amount
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory

                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);


                                    //if((i==(ttlRun-1)) && selectedPlots.Count >0)
                                    //{

                                    //}
                                    prefType.Files.RemoveAt(0);
                                    selectedPlots.RemoveAt(0);

                                    if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0,
                                            PlotSize = prefType.PlotSize
                                        });
                                    }
                                }
                                else
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Corner (Random)
                    if (prefType.Prefrence == "Corner")
                    {
                        var __temp_c = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_c is null) ? null : __temp_c.Plots;
                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                        }
                        else
                        {
                            prefType.Files = prefType.Files.OrderBy(x => x.BookingDate).ToList();
                            selectedPlots = selectedPlots.OrderBy(x => x.PlotSerial).ToList();
                            selectedPlots = GroupSerialPlotsTogether(selectedPlots);
                            if (prefType.Files.Count() > selectedPlots.Count())
                            {
                                var extra = prefType.Files.GetRange(selectedPlots.Count(), prefType.Files.Count() - selectedPlots.Count);
                                extra.ForEach(x => x.Plot_Prefrence = "Normal");
                                filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(extra);
                            }
                            //prefType.Files = GroupCnicsTogether_FirstPos(prefType.Files.GetRange(0, selectedPlots.Count()));
                            for (int i = 0; i < ttlRun && selectedPlots != null; i++)
                            {
                                //int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0)
                                {
                                    //selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    //var fileData = prefType.Files.ElementAt(selectedFile);
                                    var fileData = prefType.Files.ElementAt(0);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector,
                                        Due_Date = DateTime.Now,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = "",
                                        FirstInstallment = (fileData.First_Installment is null) ? "02/12/2018" : fileData.First_Installment.Value.ToShortDateString(),
                                        BalanceAmount = fileData.Balance_Amount
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);

                                    //prefType.Files.RemoveAt(selectedFile);
                                    prefType.Files.RemoveAt(0);
                                    selectedPlots.RemoveAt(0);
                                    fileData = null;
                                    plotsData = null;
                                    if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0,
                                            PlotSize = prefType.PlotSize
                                        });
                                    }
                                }
                                else
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Boulevard (Random)
                    if (prefType.Prefrence == "Boulevard")
                    {
                        var __temp_b = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_b is null) ? null : __temp_b.Plots;

                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                        }
                        else
                        {
                            prefType.Files = prefType.Files.OrderBy(x => x.BookingDate).ToList();
                            selectedPlots = selectedPlots.OrderBy(x => x.PlotSerial).ToList();
                            for (int i = 0; i < ttlRun && (selectedPlots != null); i++)
                            {
                                //int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0)
                                {
                                    //selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    //var fileData = prefType.Files.ElementAt(selectedFile);
                                    var fileData = prefType.Files.ElementAt(0);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector,
                                        Due_Date = DateTime.Now,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = "",
                                        FirstInstallment = (fileData.First_Installment == null) ? "02/12/2018" : fileData.First_Installment.Value.ToShortDateString(),
                                        BalanceAmount = fileData.Balance_Amount
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);

                                    //prefType.Files.RemoveAt(selectedFile);
                                    prefType.Files.RemoveAt(0);
                                    selectedPlots.RemoveAt(0);
                                    if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0,
                                            PlotSize = prefType.PlotSize
                                        });
                                    }
                                }
                                else
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Park Facing (Random)
                    if (prefType.Prefrence == "Park Facing")
                    {
                        var __temp_pf = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_pf is null) ? null : __temp_pf.Plots;
                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                        }
                        else
                        {
                            prefType.Files = prefType.Files.OrderBy(x => x.BookingDate).ToList();
                            selectedPlots = selectedPlots.OrderBy(x => x.PlotSerial).ToList();
                            for (int i = 0; i < ttlRun && (selectedPlots != null); i++)
                            {
                                //int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0 && prefType.Files.Count() > 0)
                                {
                                    //selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    //var fileData = prefType.Files.ElementAt(selectedFile);
                                    var fileData = prefType.Files.ElementAt(0);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector,
                                        Due_Date = DateTime.Now,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = "",
                                        FirstInstallment = (fileData.First_Installment == null) ? "02/12/2018" : fileData.First_Installment.Value.ToShortDateString(),
                                        BalanceAmount = fileData.Balance_Amount
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);

                                    //prefType.Files.RemoveAt(selectedFile);
                                    prefType.Files.RemoveAt(0);
                                    selectedPlots.RemoveAt(0);
                                    if ((prefType.Files.Count() <= 0) && (selectedPlots.Count > 0))
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0,
                                            PlotSize = prefType.PlotSize
                                        });
                                    }
                                }
                                else if ((prefType.Files.Count() > 0) && (selectedPlots.Count <= 0))
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Corner & Park Facing (Random)
                    if (prefType.Prefrence == "Corner & Park Facing")
                    {
                        var __temp_pf = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__temp_pf is null) ? null : __temp_pf.Plots;
                        if (selectedPlots == null || selectedPlots.Count() <= 0)
                        {
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                        }
                        else
                        {
                            prefType.Files = prefType.Files.OrderBy(x => x.BookingDate).ToList();
                            selectedPlots = selectedPlots.OrderBy(x => x.PlotSerial).ToList();
                            for (int i = 0; i < ttlRun && (selectedPlots != null); i++)
                            {
                                //int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                                if (selectedPlots.Count > 0 && prefType.Files.Count() > 0)
                                {
                                    //selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                                    //var fileData = prefType.Files.ElementAt(selectedFile);
                                    var fileData = prefType.Files.ElementAt(0);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __temp_br = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector,
                                        Due_Date = DateTime.Now,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = "",
                                        FirstInstallment = (fileData.First_Installment == null) ? "02/12/2018" : fileData.First_Installment.Value.ToShortDateString(),
                                        BalanceAmount = fileData.Balance_Amount
                                    };
                                    ballotResult.Add(__temp_br);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);

                                    //prefType.Files.RemoveAt(selectedFile);
                                    prefType.Files.RemoveAt(0);
                                    selectedPlots.RemoveAt(0);
                                    if ((prefType.Files.Count() <= 0) && (selectedPlots.Count > 0))
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0,
                                            PlotSize = prefType.PlotSize
                                        });
                                    }
                                }
                                else if ((prefType.Files.Count() > 0) && (selectedPlots.Count <= 0))
                                {
                                    //breaking the code to save the time
                                    //and adding the remaining files to normal plots prefrence type
                                    prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                                    filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                                    break;
                                }
                            }
                        }
                    }
                    //Balloting Normal Files (Not Random)
                    if (prefType.Prefrence == "Normal")
                    {
                        var __selection_of_plots__ = filteredPlots.Where(x => x.Prefrence == prefType.Prefrence && x.PlotSize == prefType.PlotSize).FirstOrDefault();
                        var selectedPlots = (__selection_of_plots__ is null) ? null : __selection_of_plots__.Plots;
                        if (selectedPlots != null)
                        {
                            prefType.Files = prefType.Files.Distinct().ToList();
                            prefType.Files = prefType.Files.OrderBy(x => x.BookingDate).Distinct().ToList();
                            //prefType.Files = GroupCnicsTogether(prefType.Files);
                            selectedPlots = selectedPlots.OrderBy(x => x.PlotSerial).ToList();
                            for (int i = 0; i < ttlRun && (selectedPlots.Count > 0); i++)
                            {
                                if ((selectedPlots.Count > 0) && (prefType.Files.Count() > 0))
                                {
                                    var fileData = prefType.Files.ElementAt(0);
                                    var plotsData = selectedPlots.ElementAt(0);
                                    string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                                    var __tempbr = new BallotResultModel
                                    {
                                        Preference = prefType.Prefrence,
                                        File_Id = (long)fileData.Id,
                                        PlotNo = plotsData.PlotNo + " " + plotsData.Sector,
                                        Due_Date = DateTime.Now,
                                        CNIC_NICOP = fileData.CNIC_NICOP,
                                        Name = fileData.Name,
                                        FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                        FileSize = fileData.Plot_Size,
                                        PlotArea = plotsData.PlotArea.ToString(),
                                        PlotDimensions = plotsData.PlotSize,
                                        DevelopmentCharges = fileData.Development_Charges,
                                        BookingDate = "",
                                        FirstInstallment = (fileData.First_Installment == null) ? "02/12/2018" : fileData.First_Installment.Value.ToShortDateString(),
                                        BalanceAmount = fileData.Balance_Amount
                                    };
                                    ballotResult.Add(__tempbr);

                                    //updating the plots inventory
                                    db.Sp_Update_AssignBallotPlot(plotsData.Id, __tempbr.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __tempbr.File_Id);

                                    prefType.Files.RemoveAt(0);
                                    selectedPlots.RemoveAt(0);
                                    if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                                    {
                                        //it means some extra plots have been left unballoted in this category
                                        UnballottedPlots.Add(new PreferenceFilteredPlots
                                        {
                                            Plots = selectedPlots,
                                            Prefrence = prefType.Prefrence,
                                            Priority = 0,
                                            PlotSize = prefType.PlotSize
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                ///////////////////////////////////////////////////////////

                var nextbal = db.Sp_Get_Ballot_Qualifying_File().ToList();
                List<BallotPlot> rem_plotsInHand = new List<BallotPlot>();
                rem_plotsInHand = db.BallotPlots.Where(x => (x.Status == "Available")).ToList();
                List<PreferenceFilteredPlots> RemfilteredPlots = new List<PreferenceFilteredPlots>();


                foreach (var v in rem_plotsInHand.GroupBy(x => x.PlotArea))
                {
                    int pltsz_temp = Convert.ToInt32(v.Key.Value);
                    int prty = 0;
                    RemfilteredPlots.Add(new PreferenceFilteredPlots
                    {
                        Prefrence = "",
                        Plots = v.OrderBy(x => x.PlotSerial).ToList(),
                        Priority = prty,
                        PlotSize = pltsz_temp
                    });
                }

                List<PreferenceFilteredFiles> rem_filteredFiles = new List<PreferenceFilteredFiles>();
                foreach (var v in nextbal.GroupBy(x => x.Plot_Size))
                {
                    int pltsz_temp = int.Parse(v.Key.Split(' ')[0]);
                    int prty = 0;
                    
                    rem_filteredFiles.Add(new PreferenceFilteredFiles
                    {
                        Prefrence = nextbal.FirstOrDefault().Plot_Prefrence,
                        Files = v.OrderBy(x => x.Due_Date).ToList(),
                        Priority = prty,
                        PlotSize = pltsz_temp
                    });
                }


                foreach (var prefType in rem_filteredFiles)
                {
                    var __temp_b = RemfilteredPlots.Where(x => x.PlotSize == prefType.PlotSize).FirstOrDefault();
                    var selectedPlots = (__temp_b is null) ? null : __temp_b.Plots;
                    int ttlRun = prefType.Files.Count;
                    for (int i = 0; i < ttlRun && (selectedPlots != null); i++)
                    {
                        int selectedFile = _randomizer.Next(0, prefType.Files.Count) - 1;
                        if (selectedPlots.Count > 0)
                        {
                            selectedFile = (selectedFile < 0) ? 0 : selectedFile;
                            var fileData = prefType.Files.ElementAt(selectedFile);
                            var plotsData = selectedPlots.ElementAt(0);
                            string tempFileNo = tempFileCollection.Where(x => x.Id == fileData.Id).Select(x => x.FileFormNumber).FirstOrDefault();
                            var __temp_br = new BallotResultModel
                            {
                                Preference = prefType.Prefrence,
                                File_Id = (long)fileData.Id,
                                PlotNo = plotsData.PlotNo + " " + plotsData.Sector + " ( " + plotsData.Block + " )",
                                Due_Date = DateTime.Now,
                                CNIC_NICOP = fileData.CNIC_NICOP,
                                Name = fileData.Name,
                                FileNo = (tempFileNo is null) ? "-" : tempFileNo.ToString(),
                                FileSize = fileData.Plot_Size,
                                PlotArea = plotsData.PlotArea.ToString(),
                                PlotDimensions = plotsData.PlotSize,
                                DevelopmentCharges = fileData.Development_Charges,
                                BookingDate = DateTime.Now.ToString(),
                                FirstInstallment = fileData.First_Installment.Value.ToShortDateString()
                            };
                            ballotResult.Add(__temp_br);

                            //updating the plots inventory
                            db.Sp_Update_AssignBallotPlot(plotsData.Id, __temp_br.FileNo, DateTime.Now, fileData.OwnerId, fileData.Name, __temp_br.File_Id);

                            prefType.Files.RemoveAt(selectedFile);
                            selectedPlots.RemoveAt(0);
                            if ((i == (ttlRun - 1)) && selectedPlots.Count > 0)
                            {
                                //it means some extra plots have been left unballoted in this category
                                UnballottedPlots.Add(new PreferenceFilteredPlots
                                {
                                    Plots = selectedPlots,
                                    Prefrence = prefType.Prefrence,
                                    Priority = 0
                                });
                            }
                        }
                        else
                        {
                            //breaking the code to save the time
                            //and adding the remaining files to normal plots prefrence type
                            prefType.Files.ForEach(x => x.Plot_Prefrence = "Normal");
                            rem_filteredFiles.Where(x => x.Prefrence == "Normal" && x.PlotSize == prefType.PlotSize).FirstOrDefault().Files.AddRange(prefType.Files);
                            break;
                        }
                    }
                }


                ballotResult.ForEach(x => x.BookingDate = x.Due_Date.ToShortDateString());
                //foreach(var v in ballotResult)
                //{
                //    v.FileNo = db.File_Form.Where(x => x.Id == v.File_Id).Select(x => x.FileFormNumber).FirstOrDefault().ToString();
                //}

                List<Sp_Get_Ballot_Qualifying_File_Result> remainderFiles = new List<Sp_Get_Ballot_Qualifying_File_Result>();

                foreach (var remFile in filteredFiles.Where(x => x.Prefrence == "Normal"))
                {
                    remainderFiles.AddRange(remFile.Files);
                }

                remainderFiles.Where(x => x.BookingDate != null).ToList().ForEach(x => x.Mobile_2 = x.BookingDate.Value.ToShortDateString());

                backupQlfying.AddRange(nextbal);

                foreach (var pp in backupQlfying.GroupBy(x => x.Plot_Prefrence))
                {
                    foreach (var ps in pp.GroupBy(x => x.Plot_Size))
                    {
                        string[] _s_temp = ps.Key.Split(' ');
                        int size_ident = 0;
                        size_ident = int.Parse(_s_temp[0]);
                        BallotComparativeReport bcp = new BallotComparativeReport();
                        bcp.Available = backupAvlbl.Where(x => x.PlotType == pp.Key && x.PlotArea == size_ident).Count();
                        bcp.Hold = backuphold.Where(x => x.PlotType == pp.Key && x.PlotArea == size_ident).Count();
                        bcp.Ident = _s_temp[0] + " Marla ( " + pp.Key + " )";
                        bcp.Requests = ps.Count();
                        bcp.Total = bcp.Available + bcp.Hold;
                        bcp.Balloted = ballotResult.Where(x => x.PlotArea == _s_temp[0] && x.Preference == pp.Key).Count();
                        BallotComparator.Add(bcp);
                    }
                }


                var _output = new BallotOutputModel
                {
                    Status = true,
                    Msg = "Ballot performed successfully.",
                    ballotFiles = ballotResult,
                    unballotedPlots = UnballottedPlots,
                    unballotedFiles = remainderFiles,
                    filesSummary = filesSummaryData,
                    plotSummary = plotsSummaryData,
                    HoldPlotSummary = heldPlotsSummaryData,
                    ComparativeReport = BallotComparator
                };




                var ret_res = PartialView(_output);
                //ret_res.MaxJsonLength = int.MaxValue;
                return ret_res;
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                return PartialView(new BallotOutputModel { Status = false, Msg = ex.Message + " --- INNER Message ---" + ex.InnerException.Message, ballotFiles = new List<BallotResultModel>(), unballotedPlots = new List<PreferenceFilteredPlots>(), unballotedFiles = new List<Sp_Get_Ballot_Qualifying_File_Result>(), filesSummary = new List<BallotSummary>(), plotSummary = new List<BallotSummary>(), HoldPlotSummary = new List<BallotSummary>() });
            }
        }

        public JsonResult Balloting(int bType, long plotStart, long plotEnd, long fileStart, long fileEnd)
        {
            return Json(null);
        }

        public void AddPlots(int ttl, int size, string sect, string blk, int startFrm)
        {
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).FirstOrDefault().Name;
            DateTime dt = DateTime.Now;
            List<BallotPlot> bp = new List<BallotPlot>();
            for (int i = 0; i < ttl; i++)
            {
                bp.Add(new BallotPlot
                {
                    Block = blk,
                    PlotArea = size,
                    Sector = sect,
                    UploadedBy_Id = uid,
                    UploadedBy_Name = uname,
                    PlotSerial = startFrm + i,
                    UploadedAt = dt,
                    Status = "Available"
                });
            }
            db.BallotPlots.AddRange(bp);

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Added plots For Balloting", "Create", "Activity_Record", ActivityType.Balloting.ToString(), userid);
            db.SaveChanges();
        }

        public void SetPlotsType(int ttlPlts, string type)
        {
            var totalPlots = db.BallotPlots.Where(x => string.IsNullOrEmpty(x.PlotType)).Count();
            for (int i = 1; i <= ttlPlts; i++)
            {
                Random rnd = new Random();
                int val = rnd.Next(1, ttlPlts);
                var plt = db.BallotPlots.Where(x => x.PlotSerial == val).FirstOrDefault();
                while (!(string.IsNullOrEmpty(plt.PlotType)))
                {
                    val = rnd.Next(1, totalPlots);
                    plt = db.BallotPlots.Where(x => x.PlotSerial == val).FirstOrDefault();
                }
                plt.PlotType = type;
                db.BallotPlots.Attach(plt);
                db.Entry(plt).Property(x => x.PlotType).IsModified = true;

                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added Plots Types For Balloting", "Create", "Activity_Record", ActivityType.Balloting.ToString(), ttlPlts);
                db.SaveChanges();
            }
        }

        public void SetPlotsTypeRandom(int ttlPlts, string type)
        {
            var totalPlots = db.BallotPlots.Count();
            for (int i = 1; i <= ttlPlts; i++)
            {
                Random rnd = new Random();
                int val = rnd.Next(1, ttlPlts);
                var plt = db.BallotPlots.Where(x => x.PlotSerial == val).FirstOrDefault();
                //while (!(string.IsNullOrEmpty(plt.PlotType)))
                //{
                //    val = rnd.Next(1, totalPlots);
                //    plt = db.BallotPlots.Where(x => x.PlotSerial == val).FirstOrDefault();
                //}
                plt.PlotType = type;
                db.BallotPlots.Attach(plt);
                db.Entry(plt).Property(x => x.PlotType).IsModified = true;

                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added Random Plots Types For Balloting", "Create", "Activity_Record", ActivityType.Page_Access.ToString(), ttlPlts);
                db.SaveChanges();
            }
        }

        public void AssignPlotNumbers()
        {
            var res = db.BallotPlots.GroupBy(x => new { x.Block, x.Sector }).ToList();
            int counter = 1;
            foreach (var v in res)
            {
                counter = 1;
                foreach (var item in v)
                {
                    item.PlotNo = counter.ToString() + " " + item.Sector;
                    db.BallotPlots.Attach(item);
                    db.Entry(item).Property(x => x.PlotNo).IsModified = true;
                    db.SaveChanges();

                    long userid = long.Parse(User.Identity.GetUserId());
                    db.Sp_Add_Activity(userid, "Assigned Plot Number "+ item.PlotNo + item.Block + item.Sector + " to " + item.FileId, "Create", "Activity_Record", ActivityType.Balloting.ToString(), userid);
                    counter++;
                }
            }
        }

        public JsonResult GetPlotsRange(string s, long? start, string blk, string sec)
        {
            if (start is null)
            {
                var result = db.BallotPlots.Where(x => (x.Block == blk) && (x.Sector == sec) && (x.PlotNo.Contains(s)) && (x.Status == "Available")).Select(x => new { id = x.PlotSerial, text = x.PlotNo }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = db.BallotPlots.Where(x => (x.Block == blk) && (x.Sector == sec) && (x.PlotSerial > start) && (x.PlotNo.Contains(s)) && (x.Status == "Available")).Select(x => new { id = x.PlotSerial, text = x.PlotNo }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UploadPlotsInventory()
        {
            return PartialView();
        }

        public JsonResult SavePlotsInventory(List<BallotPlotFileInventory> plotsData)
        {
            List<BallotPlot> __processed_bp__ = new List<BallotPlot>();
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                foreach (var v in plotsData)
                {
                    File_Form File = new File_Form();
                    List<Sp_Get_FileLastOwner_Result> ownerlist = new List<Sp_Get_FileLastOwner_Result>();
                    if (v.REF != null)
                    {
                        File = db.File_Form.Where(x => x.FileFormNumber.ToString() == v.REF).FirstOrDefault();
                        ownerlist = db.Sp_Get_FileLastOwner(File.Id).ToList();

                    }
                    var check = db.BallotPlots.Any(x => x.Block == v.BLOCK && x.PlotNo == v.PLOT && x.PlotType == v.TYPE);
                    if (check)
                    {
                        continue;
                    }
                    if (File.FileFormNumber != null)
                    {
                        BallotPlot __temp__bp__ = new BallotPlot
                        {
                            PlotArea = Convert.ToDouble(v.SIZE),
                            Block = v.BLOCK,
                            PlotNo = v.PLOT,
                            PlotSerial = v.SR,
                            PlotSize = v.DIMENSION,
                            Road = v.ROAD,
                            Sector = v.SECTOR,
                            PlotType = v.LOCATION,
                            UploadedAt = DateTime.Now,
                            UploadedBy_Id = uid,
                            UploadedBy_Name = uname,
                            Status = "Balloted",
                            Plot_Type = v.TYPE,
                            BallotFile = File.FileFormNumber.ToString(),
                            FileId = File.Id,
                            BallotedOn = DateTime.Now,
                            Owner_Id = ownerlist.FirstOrDefault().Group_Tag,
                            Owner_Name = String.Join(",", ownerlist.Select(x=> x.Name)),
                            Actual_Size = v.SIZE
                        };
                        __processed_bp__.Add(__temp__bp__);
                    }
                    else
                    {
                        BallotPlot __temp__bp__ = new BallotPlot
                        {
                            PlotArea = Convert.ToDouble(v.SIZE),
                            Block = v.BLOCK,
                            PlotNo = v.PLOT,
                            PlotSerial = v.SR,
                            PlotSize = v.DIMENSION,
                            Road = v.ROAD,
                            Sector = v.SECTOR,
                            PlotType = v.LOCATION,
                            UploadedAt = DateTime.Now,
                            UploadedBy_Id = uid,
                            Plot_Type = v.TYPE,
                            UploadedBy_Name = uname,
                            Status = v.STATUS,
                            Actual_Size = v.SIZE
                        };
                        __processed_bp__.Add(__temp__bp__);
                    }
                }

                db.BallotPlots.AddRange(__processed_bp__);
                db.SaveChanges();

                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added Plots Inventory For Sher Afghan ", "Create", "Activity_Record", ActivityType.Balloting.ToString(), userid);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public ActionResult BallotedPlots()
        {
            //var res = db.BallotPlots.Where(x => x.Status == "Balloted").ToList();
            var res = db.Sp_Get_BallotedPlotsList().ToList();
            return PartialView(res);
        }
        public JsonResult BallotedPlotsData(string query)
        {
            var res = db.Sp_Get_BallotedPlotsList_Param(query).ToList();
            return Json(res);
        }

        public ActionResult BallotInventory()
        {
            var res = db.BallotPlots.ToList();
            return View(res);
        }

        //private List<Sp_Get_Ballot_Qualifying_File_Result> GroupCnicsTogether(List<Sp_Get_Ballot_Qualifying_File_Result> data)
        //{
        //    int __temp_len_flag__ = data.Count();
        //    for (int i = 0; i < __temp_len_flag__; i++)
        //    {
        //        for (int j = i + 1; j < __temp_len_flag__; j++)
        //        {
        //            if (data[i].CNIC_NICOP == data[j].CNIC_NICOP)
        //            {
        //                var ___temp__file__holder___ = data.ElementAt(j);
        //                data.RemoveAt(j);
        //                data.Insert(i + 1, ___temp__file__holder___);
        //            }
        //        }
        //    }
        //    return data;
        //}

        //private List<Sp_Get_Ballot_Qualifying_File_Result> GroupCnicsTogether_FirstPos(List<Sp_Get_Ballot_Qualifying_File_Result> data)
        //{
        //    var res = data.GroupBy(x => x.CNIC_NICOP);
        //    res = res.OrderBy(x => x.Count());
        //    List<Sp_Get_Ballot_Qualifying_File_Result> finalised = new List<Sp_Get_Ballot_Qualifying_File_Result>();
        //    foreach (var v in res)
        //    {
        //        finalised.AddRange(v.ToList());
        //    }
        //    return finalised;
        //}

        public JsonResult GroupCnicsTogether_FirstPos()
        {
            List<Sp_Get_Ballot_Qualifying_File_Result> data = db.Sp_Get_Ballot_Qualifying_File().Where(x => x.Plot_Prefrence == "Corner" && x.Plot_Size.Contains("5")).ToList();
            var res = data.GroupBy(x => x.CNIC_NICOP);
            res = res.OrderByDescending(x => x.Count());
            List<Sp_Get_Ballot_Qualifying_File_Result> finalised = new List<Sp_Get_Ballot_Qualifying_File_Result>();
            foreach (var v in res)
            {
                finalised.AddRange(v.ToList());
            }
            return Json(finalised, JsonRequestBehavior.AllowGet);
        }

        private List<BallotPlot> GroupSerialPlotsTogether(List<BallotPlot> plots)
        {
            plots = plots.OrderBy(x => x.PlotSerial).ToList();
            int ttl_Length = plots.Count();
            List<BallotPlot> finalisedPlots = new List<BallotPlot>();

            for (int i = 0; i < (ttl_Length - 1); i++)
            {
                if (plots[i].PlotSerial == plots[i + 1].PlotSerial)
                {
                    while ((i < (ttl_Length - 1)) && plots[i].PlotSerial == plots[i + 1].PlotSerial)
                    {
                        var __temp = plots[i];
                        plots.RemoveAt(i);
                        finalisedPlots.Add(__temp);
                        i++;
                        ttl_Length = plots.Count();
                    }
                }
            }
            plots.OrderBy(x => x.PlotSerial);
            finalisedPlots.AddRange(plots);
            return finalisedPlots;
        }

        public JsonResult GroupSerialPlotsTogether()
        {
            List<BallotPlot> plots = db.BallotPlots.Where(x => x.PlotArea == 5 && x.PlotType == "Corner").ToList();
            plots = plots.OrderBy(x => x.PlotSerial).ToList();
            int ttl_Length = plots.Count();
            List<BallotPlot> finalisedPlots = new List<BallotPlot>();

            for (int i = 0; i < (ttl_Length - 1); i++)
            {
                if (plots[i].PlotSerial == plots[i + 1].PlotSerial)
                {
                    while ((i < (ttl_Length - 1)) && plots[i].PlotSerial == plots[i + 1].PlotSerial)
                    {
                        var __temp = plots[i];
                        plots.RemoveAt(i);
                        finalisedPlots.Add(__temp);
                        i++;
                        ttl_Length = plots.Count();
                    }
                }
            }
            plots.OrderBy(x => x.PlotSerial);
            finalisedPlots.AddRange(plots);
            return Json(finalisedPlots, JsonRequestBehavior.AllowGet);
        }

        //Ballot Event Related Functionalities are as below

        public ActionResult EntryPoint()
        {
            //Getting data and throwing it to front end to avoid minimal client server interaction;
            var res = db.Sp_Get_BallotEventFiles_ForEntry().ToList();

            return View(res);
        }

        public JsonResult NewEntry(List<Ballot_Event_Entry> bee)
        {
            try
            {
                var dt = DateTime.Now;
                bee.ForEach(x => x.EntranceTime = dt);
                db.Ballot_Event_Entry.AddRange(bee);
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult AppNewEntry(long File_Id, string File_Number, int Male, int Female, DateTime EntranceTime, int child, string cnic)
        {
            try
            {
                Ballot_Event_Entry bee = new Ballot_Event_Entry
                {
                    Adults = Male,
                    Children = child,
                    EntranceTime = EntranceTime,
                    Entrance_Gate = "1",
                    Entry_CNIC = cnic,
                    Female = Female,
                    File_Id = File_Id,
                    File_Number = File_Number
                };
                db.Ballot_Event_Entry.Add(bee);
                db.SaveChanges();
                return Json(new { Status = true, Message = "Data has been saved" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Failed to save data" }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult KioskScreen()
        //{
        //    long uid = User.Identity.GetUserId<long>();
        //    if(!kioskServers.Any(x=>x.UserId == uid))
        //    {
        //        kioskServers.Add(new KioskServer { UserId = uid, CurrentlyServing = 0 });
        //    }
        //    return View();
        //}

        //public ActionResult KioskAllottedFiles()
        //{
        //    try
        //    {
        //        var uid = User.Identity.GetUserId<long>();
        //        var kioskSession = kioskServers.Where(x => x.UserId == uid).FirstOrDefault();
        //        var files = db.Sp_Get_BallotEventFiles_ForEntry().OrderBy(x => x.Id).ToList();
        //        var kiosks = db.Sp_Get_UsersByRole("KioskScreen").ToList();
        //        List<long> kiosks_users = kiosks.Select(x => x.UserId).ToList();
        //        var connectedUsers = NotificationHub.ConnectedUsers.Where(x => kiosks_users.Contains(x.UserId)).Select(x=>x.UserId).Distinct().ToList();

        //        var nChunk = files.Count() / connectedUsers.Count();

        //        //remove the current serving file before formatting the list
        //        if (kioskSession != null)
        //        {
        //            var currServ = files.Where(x => x.FileFormNumber == (long)kioskSession.CurrentlyServing).FirstOrDefault();
        //            files.Remove(currServ);
        //        }

        //        List<Sp_Get_BallotEventFiles_ForEntry_Result> filtered = new List<Sp_Get_BallotEventFiles_ForEntry_Result>();
        //        int nPos = connectedUsers.IndexOf(uid);

        //        int st = nPos * nChunk;
        //        int end = (st + nChunk);

        //        if (st >= 0 && end <= (files.Count()))
        //        {
        //            filtered = files.GetRange(st, nChunk);
        //        }


        //        return PartialView(filtered);
        //    }
        //    catch(Exception ex)
        //    {
        //        return PartialView(null);
        //    }
        //}

        public ActionResult AtendeeMonitoring()
        {
            var res = db.Ballot_Event_Entry.ToList();
            var res2 = db.Sp_Get_BallotedPlotsList().ToList();
            return View(new AttendeeMonitorModel { GuestsList = res, ServedFiles = res2 });
        }

        public ActionResult OldBallot()
        {
            return View();
        }

        public ActionResult BallotResultOld(long? id)
        {
            if (id is null)
            {
                id = db.SpeedFests.Max(x => x.Id);
            }
            var res = db.SpeedFests.OrderByDescending(x => x.Id).Take(1).FirstOrDefault();
            var output = JsonConvert.DeserializeObject<BallotOutputModel>(res.Address);
            return PartialView(output);
        }

        public ActionResult IntimationLetter(long id)
        {
            long uid = User.Identity.GetUserId<long>();
            var res = db.Sp_Get_BallotIntimationLetter(id, uid).FirstOrDefault();
            var res1 = db.Sp_Get_FileAppFormData(res.BallotFile).SingleOrDefault();

            // Now you can use the fileFormIds to filter Files_Transfer
            var filedata = db.Files_Transfer.SingleOrDefault(x => x.File_Form_Id == res1.Id);
            ViewBag.Mobile = filedata.Mobile_1;
            res.Owner_Name = res.Name;
            return View(res);
        }

        public void MoveRecordToPlots()
        {
            //Moving Installments Structure
            //Assigning Plots Id
            //Moving Records of File Owners Table To Plot Owners Table
        }

        public void AddPlotsToInventory()
        {
            var ballot_leftovers = db.BallotPlots.Where(x => x.Status == "Hold").ToList();

            foreach (var v in ballot_leftovers)
            {
                decimal front = 0;
                decimal back = 0;
                decimal left = 0;
                decimal right = 0;
                var blk = db.RealEstate_Blocks.Where(x => x.Block_Name == v.Block).FirstOrDefault();
                var phase = db.RealEstate_Phases.Where(x => x.Id == blk.Phase_Id).FirstOrDefault();
                if (v.PlotSize.Contains('F'))
                {
                    string[] _temp = v.PlotSize.Split('-');
                    foreach (string s in _temp)
                    {
                        if (s.Contains('F'))
                        {
                            front = Convert.ToDecimal(s.Split(' ')[0]);
                        }
                        else if (s.Contains('B'))
                        {
                            back = Convert.ToDecimal(s.Split(' ')[0]);
                        }
                        else if (s.Contains('L'))
                        {
                            left = Convert.ToDecimal(s.Split(' ')[0]);
                        }
                        else if (s.Contains('R'))
                        {
                            right = Convert.ToDecimal(s.Split(' ')[0]);
                        }
                    }
                }
                else if (v.PlotSize.Contains('x'))
                {
                    v.PlotSize = v.PlotSize.Remove(' ');
                    string[] _temp = v.PlotSize.Split('x');
                    front = back = Convert.ToDecimal(_temp[0]);
                    left = right = Convert.ToDecimal(_temp[1]);
                }
                var res = db.Plots_Category.Where(x => x.Front_Side == front && x.Back_Side == back && x.Left_Side == left && x.Right_Side == right).FirstOrDefault();

                if (res is null)
                {
                    Plots_Category pc = new Plots_Category
                    {
                        Front_Side = front,
                        Back_Side = back,
                        Left_Side = left,
                        Right_Side = right,
                        Category = v.PlotArea + " Marla",
                    };
                    db.Plots_Category.Add(pc);
                    db.SaveChanges();

                    Plot p = new Plot
                    {
                        Block_Id = blk.Id,
                        Phase_Id = phase.Id,
                        Plot_Category = pc.Id,
                        Hold = true,
                        Plot_Location = v.PlotType,
                        Plot_Number = v.PlotNo,
                        Sector = v.Sector,
                        Plot_Size = v.PlotSize,
                        Road_Type = v.Road,
                        Block_Name = blk.Block_Name,
                        Develop_Status = "Non Constructed",
                        Verified = false,
                        Status = "Hold",
                        Type = "Residential"
                    };
                    db.Plots.Add(p);
                    db.SaveChanges();
                }
                else
                {
                    Plot p = new Plot
                    {
                        Block_Id = blk.Id,
                        Phase_Id = phase.Id,
                        Plot_Category = res.Id,
                        Hold = true,
                        Plot_Location = v.PlotType,
                        Plot_Number = v.PlotNo,
                        Sector = v.Sector,
                        Plot_Size = v.PlotSize,
                        Road_Type = v.Road,
                        Block_Name = blk.Block_Name,
                        Develop_Status = "Non Constructed",
                        Verified = false,
                        Status = "Hold",
                        Type = "Residential"
                    };
                    db.Plots.Add(p);
                    db.SaveChanges();
                }
            }
        }

        //public JsonResult ServeNextFile(long file)
        //{
        //    long uid = User.Identity.GetUserId<long>();
        //    kioskServers.Where(x => x.UserId == uid).FirstOrDefault().CurrentlyServing = file;
        //    return Json(true);
        //}

        public ActionResult ManualPlotAssignment()
        {
            return View();
        }

        public JsonResult AddPlotRequest(long pltId, string fileNo, long fileId)
        {
            try
            {
                var res = db.BallotPlots.Where(x => x.Id == pltId).FirstOrDefault();
                return Json(new { status = true, plotInfo = res, FileNum = fileNo, FileId = fileId });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, plotInfo = "null", FileNum = fileNo, FileId = fileId });
            }
        }

        //public JsonResult SearchFile(string s)
        //{
        //    var result = db.Sp_Get_File_Mnl_Blt(s).Select(x => new { id = x.Id, text = x.FileFormNumber }).ToList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult SearchPlot(string s)
        //{
        //    var result = db.BallotPlots.Where(x => (x.PlotNo.Contains(s)) && (x.Status != "Balloted")).Select(x => new { id = x.PlotSerial, text = x.PlotNo + " - " + x.Sector }).ToList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult SaveManualPlotAssign(List<FilePlotManualAsgnmntModel> reqs, string rems)
        //{
        //    try
        //    {
        //        var uid = User.Identity.GetUserId<long>();
        //        var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();

        //        ManualAssignmentRequest pd = new ManualAssignmentRequest
        //        {
        //            DescriptionText = rems,
        //            Urgency = (UrgencyStatus.Very_Urgent),
        //            Files_N_Plots = reqs
        //        };
        //        var packed = JsonConvert.SerializeObject(pd);
        //        db.MIS_Requests.Add(new MIS_Requests
        //        {
        //            CreatedAt = DateTime.Now,
        //            CreatedBy_Id = uid,
        //            CreatedBy_Name = uname,
        //            Details = packed,
        //            //ModuleId = plot,
        //            ModuleType = "Plot_Assignment",
        //            Status = RequestStatus.Pending.ToString(),
        //            Type = "Plot_Assignment"
        //        });
        //        db.SaveChanges();
        //        return Json(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(false);
        //    }
        //}

        public JsonResult AcceptManualBallot()
        {
            try
            {
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(true);
            }
        }

        public ActionResult MapViewSherAfghan()
        {
            return View();
        }

        public JsonResult MigrateFileRecordToPlotRecordAll()
        {
            //var ballotedPlot = db.BallotPlots.Where(x => x.Id == bp).FirstOrDefault();

            //if (ballotedPlot is null /*|| ballotedPlot.LetterA == null*/)
            //{
            //    return;
            //}

            var plts = db.BallotPlots.Where(x => x.Status == "Balloted" && (x.Sector == "AA" || x.Sector == "BB" || x.Sector == "CC")).ToList();

            List<BallotPlot> residue = new List<BallotPlot>();

            foreach (var ballotedPlot in plts)
            {
                if(db.Plots.Any(x=>x.Application_FileNo == ballotedPlot.BallotFile))
                {
                    continue;
                }
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //determining category
                        decimal front = 0;
                        decimal back = 0;
                        decimal left = 0;
                        decimal right = 0;
                        long _temp_pc = 0;
                        var blk = db.RealEstate_Blocks.Where(x => x.Block_Name == ballotedPlot.Block).FirstOrDefault();
                        var phase = db.RealEstate_Phases.Where(x => x.Id == blk.Phase_Id).FirstOrDefault();
                        if (ballotedPlot.PlotSize.Contains('F'))
                        {
                            string[] _temp = ballotedPlot.PlotSize.Split('-');
                            foreach (string s in _temp)
                            {
                                if (s.Contains('F'))
                                {
                                    front = Convert.ToDecimal(s.Split(' ')[0]);
                                }
                                else if (s.Contains('B'))
                                {
                                    back = Convert.ToDecimal(s.Split(' ')[0]);
                                }
                                else if (s.Contains('L'))
                                {
                                    left = Convert.ToDecimal(s.Split(' ')[0]);
                                }
                                else if (s.Contains('R'))
                                {
                                    right = Convert.ToDecimal(s.Split(' ')[0]);
                                }
                            }
                        }
                        else if (ballotedPlot.PlotSize.Contains('x'))
                        {
                            string[] _temp = ballotedPlot.PlotSize.Split('x');
                            front = back = Convert.ToDecimal(_temp[0]);
                            left = right = Convert.ToDecimal(_temp[1]);
                        }
                        var res = db.Plots_Category.Where(x => x.Front_Side == front && x.Back_Side == back && x.Left_Side == left && x.Right_Side == right).FirstOrDefault();

                        if (res is null)
                        {
                            Plots_Category pc = new Plots_Category
                            {
                                Front_Side = front,
                                Back_Side = back,
                                Left_Side = left,
                                Right_Side = right,
                                Category = ballotedPlot.PlotArea + " Marla",
                            };
                            db.Plots_Category.Add(pc);
                            db.SaveChanges();
                            _temp_pc = res.Id;
                        }
                        //after category is finalized

                        var owne = db.Sp_Get_FileOwnerList(ballotedPlot.FileId).ToList();
                        long? result = 0;
                        if (res is null)
                        {
                            result = db.Sp_Migrate_File_To_Plot(ballotedPlot.Id, _temp_pc).FirstOrDefault();
                        }
                        else
                        {
                            result = db.Sp_Migrate_File_To_Plot(ballotedPlot.Id, res.Id).FirstOrDefault();
                        }


                        foreach (var v in owne)
                        {
                            var grpId = new Helpers().RandomNumber();
                            var newOwnId = db.Sp_Migrate_FileOwners(ballotedPlot.FileId, ballotedPlot.BallotFile, v.Id, grpId, result, ballotedPlot.PlotNo + " - " + ballotedPlot.Sector + " ( " + ballotedPlot.Block + " )").FirstOrDefault();

                            if (!string.IsNullOrEmpty(v.Image1))
                            {
                                if (Directory.Exists(Server.MapPath("~/Repository/CustomerImages/" + v.Id)))
                                {
                                    if (!Directory.Exists(Server.MapPath("~/Repository/PlotsCustomers/" + newOwnId)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath("~/Repository/PlotsCustomers/" + newOwnId));
                                    }
                                    string _src = Server.MapPath("~/Repository/CustomerImages/");
                                    _src += Path.Combine(v.Id.ToString(), v.Image1);

                                    string _dest = Server.MapPath("~/Repository/PlotsCustomers/");
                                    _dest += Path.Combine(newOwnId.ToString(), v.Image1);

                                    System.IO.File.Copy(_src, _dest);
                                    db.Sp_Update_MigratedOwnerImages(newOwnId, "/Repository/PlotsCustomers/" + newOwnId.ToString() + "/" + v.Image1, 1);
                                }
                            }
                            else if (!string.IsNullOrEmpty(v.Image2))
                            {
                                if (Directory.Exists(Server.MapPath("~/Repository/CustomerImages/" + v.Id)))
                                {
                                    if (!Directory.Exists(Server.MapPath("~/Repository/PlotsCustomers/" + newOwnId)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath("~/Repository/PlotsCustomers/" + newOwnId));
                                    }
                                    string _src = Server.MapPath("~/Repository/CustomerImages/");
                                    _src += Path.Combine(v.Id.ToString(), v.Image2);

                                    string _dest = Server.MapPath("~/Repository/PlotsCustomers/");
                                    _dest += Path.Combine(newOwnId.ToString(), v.Image2);

                                    System.IO.File.Copy(_src, _dest);
                                    db.Sp_Update_MigratedOwnerImages(newOwnId, "/Repository/PlotsCustomers/" + newOwnId.ToString() + "/" + v.Image1, 2);
                                }
                            }
                            if (!string.IsNullOrEmpty(v.Image3))
                            {
                                if (Directory.Exists(Server.MapPath("~/Repository/CustomerImages/" + v.Id)))
                                {
                                    if (!Directory.Exists(Server.MapPath("~/Repository/PlotsCustomers/" + newOwnId)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath("~/Repository/PlotsCustomers/" + newOwnId));
                                    }
                                    string _src = Server.MapPath("~/Repository/CustomerImages/");
                                    _src += Path.Combine(v.Id.ToString(), v.Image3);

                                    string _dest = Server.MapPath("~/Repository/PlotsCustomers/");
                                    _dest += Path.Combine(newOwnId.ToString(), v.Image3);

                                    System.IO.File.Copy(_src, _dest);
                                    db.Sp_Update_MigratedOwnerImages(newOwnId, "/Repository/PlotsCustomers/" + newOwnId.ToString() + "/" + v.Image1, 3);
                                }
                            }
                            if (!string.IsNullOrEmpty(v.Image4))
                            {
                                if (Directory.Exists(Server.MapPath("~/Repository/CustomerImages/" + v.Id)))
                                {
                                    if (!Directory.Exists(Server.MapPath("~/Repository/PlotsCustomers/" + newOwnId)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath("~/Repository/PlotsCustomers/" + newOwnId));
                                    }
                                    string _src = Server.MapPath("~/Repository/CustomerImages/");
                                    _src += Path.Combine(v.Id.ToString(), v.Image4);

                                    string _dest = Server.MapPath("~/Repository/PlotsCustomers/");
                                    _dest = Path.Combine(newOwnId.ToString(), v.Image4);

                                    System.IO.File.Copy(_src, _dest);
                                    db.Sp_Update_MigratedOwnerImages(newOwnId, "/Repository/PlotsCustomers/" + newOwnId.ToString() + "/" + v.Image1, 4);
                                }
                            }
                        }

                        var cmtsIO = db.Sp_Migrate_FileComments_To_PlotComments(ballotedPlot.FileId, result).ToList();

                        //Migrating Comments Files ----- Disc I/O Operation (Time Cost)
                        if (cmtsIO.Any())
                        {
                            if (!Directory.Exists(Server.MapPath("~/PlotsData/" + result + "")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/PlotsData/" + result + ""));
                            }

                            foreach (var item in cmtsIO)
                            {
                                string src = Server.MapPath("~/FilesData/" + result + "");
                                src = Path.Combine(src, item.Comment);
                                string dest = Server.MapPath("~/PlotsData/" + result + "");
                                dest = Path.Combine(dest, item.Comment);
                                System.IO.File.Copy(src, dest);
                                System.IO.File.Delete(src);
                            }
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        residue.Add(ballotedPlot);
                        trans.Rollback();
                    }
                }
            }

            return (residue.Count() <= 0) ? Json("All Migrated", JsonRequestBehavior.AllowGet) : Json(residue, JsonRequestBehavior.AllowGet);
        }
        public void MigrateAllRecord()
        {
            var date = new DateTime(2022, 10, 29);
            //var res = db.BallotPlots.Where(x => (x.Status == "Balloted") && x.BallotedOn == date).ToList();
            var res = (from x in db.BallotPlots
                       join y in db.File_Form on x.FileId equals y.Id
                       where (x.Status == "Balloted") && x.BallotedOn == date && y.Status == 1
                       select x).ToList();

            foreach (var v in res)
            {
                MigrateFileRecordToPlotRecord(v.Id);
            }
        }
        public JsonResult MigrateFileRecordToPlotRecord(long? bp )
        {
            var ballotedPlot = db.BallotPlots.Where(x => x.Id == bp).FirstOrDefault();
            if (ballotedPlot is null /*|| ballotedPlot.LetterA == null*/)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            //var plts = db.BallotPlots.Where(x => x.Status == "Balloted").ToList();
            List<BallotPlot> residue = new List<BallotPlot>();
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    //determining category
                    decimal front = 0;
                    decimal back = 0;
                    decimal left = 0;
                    decimal right = 0;
                    long _temp_pc = 0;
                    var blk = db.RealEstate_Blocks.Where(x => x.Block_Name == ballotedPlot.Block).FirstOrDefault();
                    var phase = db.RealEstate_Phases.Where(x => x.Id == blk.Phase_Id).FirstOrDefault();
                    if (ballotedPlot.PlotSize.Contains('F'))
                    {
                        string[] _temp = ballotedPlot.PlotSize.Split('-');
                        foreach (string s in _temp)
                        {
                            if (s.Contains('F'))
                            {
                                var t = s.Trim();
                                front = Convert.ToDecimal(t.Split(' ')[0]);
                            }
                            else if (s.Contains('B'))
                            {
                                var t = s.Trim();
                                back = Convert.ToDecimal(t.Split(' ')[0]);
                            }
                            else if (s.Contains('L'))
                            {
                                var t = s.Trim();
                                left = Convert.ToDecimal(t.Split(' ')[0]);
                            }
                            else if (s.Contains('R'))
                            {
                                var t = s.Trim();
                                right = Convert.ToDecimal(t.Split(' ')[0]);
                            }
                        }
                    }
                    else if (ballotedPlot.PlotSize.Contains('x'))
                    {
                        string[] _temp = ballotedPlot.PlotSize.Split('x');
                        front = back = Convert.ToDecimal(_temp[0]);
                        left = right = Convert.ToDecimal(_temp[1]);
                    }
                    var res = db.Plots_Category.Where(x => x.Front_Side == front && x.Back_Side == back && x.Left_Side == left && x.Right_Side == right).FirstOrDefault();
                    if (res is null)
                    {
                        Plots_Category pc = new Plots_Category
                        {
                            Front_Side = front,
                            Back_Side = back,
                            Left_Side = left,
                            Right_Side = right,
                            Category = ballotedPlot.PlotArea + " Marla",
                            Uom = "sqft"
                        };
                        db.Plots_Category.Add(pc);
                        db.SaveChanges();
                        _temp_pc = pc.Id;
                    }
                    //after category is finalized
                    var owne = db.Sp_Get_FileOwnerList(ballotedPlot.FileId).ToList();
                    long? result = 0;
                    if (res is null)
                    {
                        result = db.Sp_Migrate_File_To_Plot(ballotedPlot.Id, _temp_pc).FirstOrDefault();
                    }
                    else
                    {
                        result = db.Sp_Migrate_File_To_Plot(ballotedPlot.Id, res.Id).FirstOrDefault();
                    }
                    foreach (var v in owne)
                    {
                        var grpId = new Helpers().RandomNumber();
                        var newOwnId = db.Sp_Migrate_FileOwners(ballotedPlot.FileId, ballotedPlot.BallotFile, v.Id, grpId, result, ballotedPlot.PlotNo + " - " + ballotedPlot.Sector + " ( " + ballotedPlot.Block + " )").FirstOrDefault();
                        if (!string.IsNullOrEmpty(v.Image1))
                        {
                            if (Directory.Exists(Server.MapPath("~/Repository/CustomerImages/" + v.Id)))
                            {
                                if (!Directory.Exists(Server.MapPath("~/Repository/CustomerImagesPlots/" + result + "/" + newOwnId)))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Repository/CustomerImagesPlots/" + result + "/" + newOwnId));
                                }
                                string _src = Server.MapPath("~/Repository/CustomerImages/");
                                _src += Path.Combine(v.Id.ToString(), v.Image1);
                                string _dest = Server.MapPath("~/Repository/CustomerImagesPlots/" + result + "/" + newOwnId + "/");
                                _dest += Path.Combine("", "1.jpg");
                                System.IO.File.Copy(_src, _dest, true);
                                db.Sp_Update_MigratedOwnerImages(newOwnId, "1.jpg", 1);
                            }
                        }
                    }
                    var cmtsIO = db.Sp_Migrate_FileComments_To_PlotComments(ballotedPlot.FileId, result).ToList();
                    //Migrating Comments Files ----- Disc I/O Operation (Time Cost)
                    if (cmtsIO.Any())
                    {
                        if (!Directory.Exists(Server.MapPath("~/Repository/PlotsData/" + result + "")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Repository/PlotsData/" + result + ""));
                        }
                        foreach (var item in cmtsIO)
                        {
                            string src = Server.MapPath("~/Repository/FilesData/" + ballotedPlot.FileId + "");
                            src = Path.Combine(src, item.Comment);
                            string dest = Server.MapPath("~/Repository/PlotsData/" + result + "");
                            dest = Path.Combine(dest, item.Comment);
                            try
                            {
                                System.IO.File.Copy(src, dest, true);
                                System.IO.File.Delete(src);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    residue.Add(ballotedPlot);
                    trans.Rollback();
                }
            }
            return (residue.Count() <= 0) ? Json("All Migrated", JsonRequestBehavior.AllowGet) : Json(residue, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReMigrateFileRecordToPlotRecord(long? Id)
        {
            List<BallotPlot> residue = new List<BallotPlot>();
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    //Un assigned the ballot plot
                    var a = db.Plots.Where(x => x.Id == Id).FirstOrDefault();
                    var res = db.Sp_Update_UnBallotPlot(Id, a.Application_FileId);
                    var plotowners = db.Sp_Get_PlotOwnerList(Id).ToList();
                    foreach (var v in plotowners)
                    {
                        var newOwnId = db.Sp_Update_UnBallotPlot_MoveOwners(v.Id, a.Application_FileId).FirstOrDefault();
                        if (!string.IsNullOrEmpty(v.Owner_Img))
                        {
                            if (Directory.Exists(Server.MapPath("~/Repository/CustomerImagesPlots/" + Id + "/" + v.Id)))
                            {
                                if (!Directory.Exists(Server.MapPath("~/Repository/CustomerImages/" + newOwnId)))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Repository/CustomerImages/" + newOwnId));
                                }
                                string _src = Server.MapPath("~/Repository/CustomerImagesPlots/" + Id + "/" + v.Id + "/");
                                _src += Path.Combine("", "1.jpg");
                                string _dest = Server.MapPath("~/Repository/CustomerImages/");
                                _dest += Path.Combine(newOwnId.ToString(), "1.jpg");
                                System.IO.File.Copy(_src, _dest, true);
                            }
                        }
                    }
                    var cmtsIO = db.Sp_Migrate_PlotComments_To_FileComments(a.Id, a.Application_FileId).ToList();
                    //Migrating Comments Files ----- Disc I/O Operation (Time Cost)
                    if (cmtsIO.Any())
                    {
                        if (!Directory.Exists(Server.MapPath("~/Repository/FilesData/" + a.Application_FileId + "")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Repository/FilesData/" + a.Application_FileId + ""));
                        }
                        foreach (var item in cmtsIO)
                        {
                            string src = Server.MapPath("~/Repository/PlotsData/" + Id + "");
                            src = Path.Combine(src, item.Comment);
                            string dest = Server.MapPath("~/Repository/FilesData/" + a.Application_FileId + "");
                            dest = Path.Combine(dest, item.Comment);
                            System.IO.File.Copy(src, dest, true);
                            System.IO.File.Delete(src);
                        }
                    }
                    db.Sp_Add_FileComments(a.Application_FileId, "Plot is unballoted from " + a.Plot_Number + "-" + a.Sector + " " + a.Block_Name, 0, ActivityType.Plan_Updation.ToString());
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    trans.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
            }
            return (residue.Count() <= 0) ? Json("All Migrated", JsonRequestBehavior.AllowGet) : Json(residue, JsonRequestBehavior.AllowGet);
        }



        public ActionResult BallotLettersPrintAll(string ident, string sect)
        {
            var res = db.BallotPlots.Where(x => x.Status == "Balloted" && x.PlotType == ident && x.Sector == sect).ToList();
            return View(res);
        }

        public JsonResult FilesTest(long? fileno)
        {
            try
            {
                var res = db.BallotPlots.Where(x => x.LetterA_IssuedBy_Id != null).ToList();
                if (fileno != null)
                {
                    res = res.Where(x => x.FileId == fileno).ToList();
                }
                var uid = User.Identity.GetUserId<long>();
                foreach (var v in res)
                {
                    db.Sp_Get_BallotIntimationLetter(v.Id, uid);
                }
                return Json("All Done");
            }
            catch (Exception ex)
            {
                return Json("Job failed");
            }
        }

        public void UpdateInstStrcSpecialPref()
        {
            var date = new DateTime(2022, 10, 29);
            var res = db.BallotPlots.Where(x => (x.Status == "Balloted") && x.BallotedOn == date).ToList();


            foreach (var v in res)
            {
                var retDat = db.Sp_Update_SpecialPrefCharge(v.Id, 0);
            }
        }
        //public ActionResult BallotSearch(long? Id)
        //{
        //    var res = db.BallotPlots.Where(x => x.Id == Id).FirstOrDefault();
        //    return PartialView(res);
        //}
        public ActionResult BallotSearch(string plotno)
        {
            var res = db.BallotPlots.Where(x => x.PlotNo == plotno).FirstOrDefault();
            return PartialView(res);
        }
        public ActionResult NewPlot_Ballot()
        {
            ViewBag.Blocks = new SelectList(db.Sp_Get_Block(), "Block_Name", "Block_Name","Project_Name");
            return PartialView();
        }
        public JsonResult BlockPlots(string Block)
        {
            var res = db.BallotPlots.Where(x => x.Block == Block && x.Status == "Available").ToList();
            return Json(res);
        }
        public JsonResult UpdateInvStatus(long Id, string Status)
        {
            var a = db.BallotPlots.Where(x => x.Id == Id).FirstOrDefault();
            if (a.FileId != null)
            {
                var b = db.Plots.Where(x => x.Application_FileId == a.FileId).FirstOrDefault();
                if (b != null)
                {
                    return Json(new Return { Msg = "Cannot " + Status + " this Unit Because this is already moved to PLOTS.", Status = false });
                }
                else
                {
                    return Json(new Return { Msg = "Deallocate Unit first.", Status = false });
                }
            }

            else
            {
                a.Status = Status;

                db.BallotPlots.Attach(a);
                db.Entry(a).Property(x => x.Status).IsModified = true;
                db.SaveChanges();

                return Json(new Return { Msg = "Unit is " + Status, Status = true });
            }
        }
        public JsonResult NewPlotAllocation(long? File_Id, long? New_Id)
        {
            var file = db.File_Form.Where(x => x.Id == File_Id).FirstOrDefault();
            var owner = db.Sp_Get_FileLastOwner(file.Id).ToList();
            db.Sp_Update_AssignBallotPlot(New_Id, file.FileFormNumber.ToString(), DateTime.Now, owner.Select(x => x.Group_Tag).FirstOrDefault(), string.Join(",", owner.Select(x => x.Name)), file.Id);
            return Json(true);
        }


        public ActionResult WelcomeBallotScreen()
        {
            return View();
        }
        public ActionResult BallotScreen()
        {
            var res = db.BallotPlots.OrderByDescending(x=> x.BallotedOn).Take(500).ToList();
            return PartialView(res);
        }

        public void ExtraArea()
        {
            var date = new DateTime(2018, 10, 29);
            var res = db.BallotPlots.Where(x => (x.Status == "Balloted") && x.BallotedOn == date &&  x.Actual_Size != x.PlotArea.ToString()).ToList();

            foreach (var item in res)
            {
                var baserate = 200000;
                var actsize = double.Parse(item.Actual_Size);
                var rem = actsize - Convert.ToDouble( item.PlotArea);
                var extracharges = Math.Round( rem * baserate, 0);

                File_Installments fi = new File_Installments()
                {
                    Amount = Convert.ToDecimal(extracharges),
                    Due_Date = new DateTime(2022, 12, 1),
                    File_Id = Convert.ToInt64(item.FileId),
                    Installment_Name = "Extra Size",
                    Installment_Type = "0",
                    Status = "Pending"
                };
                db.File_Installments.Add(fi);
                db.SaveChanges();

            }

        }
    }
}