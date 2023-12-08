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
using Microsoft.SqlServer.Server;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class InstallmentsController : Controller
    {
        // GET: Installments
        private string AccountingModulePlots = COA_Mapper_Modules.Files_Plots.ToString();
        private string AccountingModuleCommercial = COA_Mapper_Modules.Commercial.ToString();
        private Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult ReceiveInstallments()
        {
            return View();
        }
        public ActionResult CreateInstallmentStructure()
        {
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects(), "Id", "Project_Name");
            var plots = db.Sp_Get_Plots_Size().Select(x => new { Plot = x }).ToList();
            ViewBag.Plots = new SelectList(plots.Distinct(), "Plot", "Plot");
            return View();
        }
        [HttpPost]
        public JsonResult CreateInstallmentStructure(List<Installment_Structure> I_S, string Plot, long Block, decimal Rate, decimal Total, decimal GrandTotal)
        {
            //var Installmentst = new XElement("IS", I_S.Select(x => new XElement("ISdata",
            //    new XAttribute("Install_Name", x.Installment_Name),
            //    new XAttribute("Install_Type", x.Installment_Type),
            //    new XAttribute("Rate", x.Rate),
            //    new XAttribute("Amount", x.Amount),
            //    new XAttribute("After", x.After),
            //    new XAttribute("Interval", x.Interval),
            //      new XAttribute("Module", "Plot")
            //    ))).ToString();

            var Installmentst = new XElement("IS", I_S.Select(x =>
            {
                if (x.Rate == 0)
                {
                    // If rate is zero set rate 48 by default (quick fix)
                    return new XElement("ISdata",
                        new XAttribute("Install_Name", x.Installment_Name),
                        new XAttribute("Install_Type", x.Installment_Type),
                        new XAttribute("Rate", 48), // or any specific value
                        new XAttribute("Amount", x.Amount),
                        new XAttribute("After", x.After),
                        new XAttribute("Interval", x.Interval),
                        new XAttribute("Module", "Plot")
                    );
                }
                else
                {
                    return new XElement("ISdata",
                        new XAttribute("Install_Name", x.Installment_Name),
                        new XAttribute("Install_Type", x.Installment_Type),
                        new XAttribute("Rate", x.Rate),
                        new XAttribute("Amount", x.Amount),
                        new XAttribute("After", x.After),
                        new XAttribute("Interval", x.Interval),
                        new XAttribute("Module", "Plot")
                    );
                }
            })).ToString();
            var res = db.Sp_Add_InstallmentStructure(Plot, Block, Total, Rate, GrandTotal, Types.Plots.ToString(), Installmentst).FirstOrDefault();
                return Json(res);
            
        }
        public ActionResult PlotInstallmentStructionAs()
        {
            ViewBag.Projects = new SelectList(db.Sp_Get_RealEstateProjects(), "Id", "Project_Name");
            var res = (from x in db.Installment_Plot_Category
                       select x).AsEnumerable().Select(s => new { Id = s.Id, Text = s.Plot_Size + " Marla - " + string.Format("{0:n0}", s.Grand_Total), Group = s.Plot_Size + " Marla" }).ToList();
            ViewBag.PlotInst = new SelectList(res, "Id", "Text", "Group", 1);
            return View();
        }
        public JsonResult InstallmentCategory_Details(long Id)
        {
            var res = (from x in db.Installment_Structure
                       join y in db.Installment_Plot_Category on x.Installment_Plot_Id equals y.Id
                       where x.Installment_Plot_Id == Id
                       select new Installment_Struct
                       {
                           After = x.After,
                           Amount = x.Amount,
                           Datetime = x.Datetime,
                           Id = x.Id,
                           Installment_Name = x.Installment_Name,
                           Installment_Plot_Id = x.Installment_Plot_Id,
                           Installment_Type = x.Installment_Type,
                           Module = x.Module,
                           Rate = x.Rate,
                           Plot_Size = y.Plot_Size + " Marla"
                       }).ToList();
            return Json(res);
        }
        public ActionResult PlotsAssociation()
        {
            var res = (from x in db.Installment_Plot_Category
                       select x).AsEnumerable().Select(s => new { Id = s.Id, Text = s.Plot_Size + " Marla - " + string.Format("{0:n0}", s.Grand_Total), Group = s.Plot_Size + " Marla" }).ToList();
            ViewBag.PlotInst = new SelectList(res, "Id", "Text", "Group", 1);
            return View();
        }
        public JsonResult SavePlotDeals(List<DealersPlots> PlotsList)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            foreach (var item in PlotsList)
            {
                var plots = db.Plots.Where(x => x.Id == item.Plot_Id).FirstOrDefault();

                if (plots.Status == PlotsStatus.Available_For_Sale.ToString())
                {
                    plots.Installment_Plan_Id = item.Plan_Id;

                    db.Plots.Attach(plots);
                    db.Entry(plots).Property(x => x.Installment_Plan_Id).IsModified = true;

                    db.SaveChanges();
                }
            }
            return Json(new Return { Msg = "Allocated", Status = true });
        }
        public ActionResult CreateCommercialInstallmentStructure()
        {
            ViewBag.Projects = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString()), "Id", "Project_Name");
            return View();
        }
        //[HttpPost]
        //public JsonResult CreateCommercialInstallmentStructure(List<Installment_Structure> I_S, string Plot, long Block, decimal Rate, decimal Total, decimal GrandTotal)
        //{

        //    var Installmentst = new XElement("IS", I_S.Select(x => new XElement("ISdata",
        //        new XAttribute("Install_Name", x.Installment_Name),
        //        new XAttribute("Install_Type", x.Installment_Type),
        //        new XAttribute("Rate", x.Rate),
        //        new XAttribute("Amount", x.Amount),
        //        new XAttribute("After", x.After),
        //        new XAttribute("Module", "Commercial")

        //        ))).ToString();
        //    var res = db.Sp_Add_InstallmentStructure(Plot, Block, Total, Rate, GrandTotal, ProjectCategory.Commercial.ToString(), Installmentst).FirstOrDefault();
        //    return Json(res);
        //}
        //public JsonResult CreateCommercialInstallmentStruc(List<Commercial_Installment_Structure> I_S)
        //{

        //    Helpers h = new Helpers();
        //    var ran = h.RandomNumber();

        //    foreach (var i in I_S)
        //    {
        //        db.Sp_Add_Commercial_installlments_struc(i.Plan_Name, i.Advance, i.Installments, i.Duration, i.After, ran, i.Type, i.TypeName);
        //    }
        //    return Json(true);
        //}
        public void inst()
        {
            List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure_Current(95909).ToList();
            this.AddInstallmentPlan(installmentstructure, "", Convert.ToInt64(95909), DateTime.UtcNow);

        }
        public FileInstallmentStatus AddInstallmentPlan(List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure, string DevCharStatus, long Fileid, DateTime Date)
        {
            List<File_Installments> file_Installments = new List<File_Installments>();
            decimal rate = 0, total = 0, grandtotal = 0;

            try
            {
                foreach (var item in installmentstructure.Where(x => x.Installment_Type == "3" || x.Installment_Type == "6"))
                {
                    if (item.Installment_Type == "3") // Development Charges
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        DateTime a = Date;
                        File_Installments fi = new File_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = a,
                            Amount = item.Amount,
                            File_Id = Fileid,
                            Installment_Name = item.Installment_Name,
                            Installment_Type = "3",
                        };
                        file_Installments.Add(fi);
                    }
                    else if (item.Installment_Type == "6")
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        DateTime a = Date;
                        a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                        DateTime dt = new DateTime(a.Year, a.Month, 1);
                        File_Installments fi = new File_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = dt,
                            Amount = item.Amount,
                            File_Id = Fileid,
                            Installment_Name = item.Installment_Name,
                            Installment_Type = "6",
                        };
                        file_Installments.Add(fi);
                    }
                }
                foreach (var item in installmentstructure.Where(x => x.Installment_Type == "4" || x.Installment_Type == "2" || x.Installment_Type == "0"))
                {

                    if (item.Installment_Type == "4") // for Half yearly , Quarterly
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        var eachinst = item.Amount / item.After;
                        var itemaft = item.Interval;



                        for (int i = 1; i <= item.After; i++)
                        {
                            DateTime a = Date;
                            var str = file_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                            if (str != null) // to check for confirmation
                            {
                                a = str.Due_Date;
                            }
                            a = a.AddMonths(Convert.ToInt16(itemaft));
                            DateTime dt = new DateTime(a.Year, a.Month, 1);

                            File_Installments fi = new File_Installments()
                            {
                                Status = InstallmentPaymentStatus.Pending.ToString(),
                                Due_Date = dt,
                                Amount = Convert.ToDecimal(eachinst),
                                File_Id = Fileid,
                                Installment_Name = i + " - " + item.Installment_Name,
                                Installment_Type = "1",
                            };
                            file_Installments.Add(fi);
                            itemaft += item.Interval;
                        }
                    }
                    else if (item.Installment_Type == "2") // Development Charges
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;

                        DateTime a = Date;
                        var str = file_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                        if (str != null) // to check for confirmation
                        {
                            a = str.Due_Date;
                        }

                        a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                        DateTime dt = new DateTime(a.Year, a.Month, 1);
                        var devamt = (DevCharStatus == "true") ? item.Amount : 0;
                        File_Installments fi = new File_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = dt,
                            Amount = devamt,
                            File_Id = Fileid,
                            Installment_Name = item.Installment_Name,
                            Installment_Type = "2",
                        };
                        if (DevCharStatus == "false")
                        {
                            fi.Amount = 0;
                        }
                        file_Installments.Add(fi);
                    }
                    else if (item.Installment_Type == "0")
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;

                        DateTime a = Date;
                        var str = file_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                        if (str != null) // to check for confirmation
                        {
                            a = str.Due_Date;
                        }

                        a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                        DateTime dt = new DateTime(a.Year, a.Month, 1);
                        File_Installments fi = new File_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = dt,
                            Amount = item.Amount,
                            File_Id = Fileid,
                            Installment_Name = item.Installment_Name,
                            Installment_Type = "0",
                        };
                        file_Installments.Add(fi);
                    }
                }
                foreach (var item in installmentstructure.Where(x => x.Installment_Type == "1" || x.Installment_Type == "11"))
                {
                    if (item.Installment_Type == "1") // for type installments
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        var eachinst = item.Amount / item.Rate;


                        for (int i = 1; i <= item.Rate; i++)
                        {
                            DateTime a = Date;
                            var str = file_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                            if (str != null) // to check for confirmation
                            {
                                a = str.Due_Date;
                            }
                            a = a.AddMonths(i);
                            var insRes = file_Installments.Where(x => x.Due_Date == new DateTime(a.Year, a.Month, 1)).FirstOrDefault();
                            while (insRes != null)
                            {
                                a = a.AddMonths(1);
                                insRes = file_Installments.Where(x => x.Due_Date == new DateTime(a.Year, a.Month, 1)).FirstOrDefault();
                            }
                            DateTime dt = new DateTime(a.Year, a.Month, 1);

                            File_Installments fi = new File_Installments()
                            {
                                Status = InstallmentPaymentStatus.Pending.ToString(),
                                Due_Date = dt,
                                Amount = eachinst,
                                File_Id = Fileid,
                                Installment_Name = i + " Monthly Installment",
                                Installment_Type = "1",
                            };
                            file_Installments.Add(fi);
                        }
                    }
                    if (item.Installment_Type == "11") // for type installments - fixed
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        var totalinstval = Convert.ToDecimal(item.Rate * item.After);
                        var actinstval = item.Amount;

                        for (int i = 1; i <= item.After; i++)
                        {

                            DateTime a = Date;
                            var str = file_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                            if (str != null) // to check for confirmation
                            {
                                a = str.Due_Date;
                            }
                            a = a.AddMonths(i);
                            var insRes = file_Installments.Where(x => x.Due_Date == new DateTime(a.Year, a.Month, 1)).FirstOrDefault();
                            while (insRes != null)
                            {
                                a = a.AddMonths(1);
                                insRes = file_Installments.Where(x => x.Due_Date == new DateTime(a.Year, a.Month, 1)).FirstOrDefault();
                            }

                            DateTime dt = new DateTime(a.Year, a.Month, 1);
                            File_Installments fi = new File_Installments();

                            fi.Status = InstallmentPaymentStatus.Pending.ToString();
                            fi.Due_Date = dt;
                            if (i == item.After)
                            {
                                fi.Amount = item.Rate - (totalinstval - actinstval);
                            }
                            else
                            {
                                fi.Amount = item.Rate;
                            }
                            fi.File_Id = Fileid;
                            fi.Installment_Name = i + " Monthly Installment";
                            fi.Installment_Type = "1";
                            file_Installments.Add(fi);
                        }
                    }
                }
                var installmentplan = new XElement("Installments", file_Installments.Select(x => new XElement("Installmentsdata",
                                       new XAttribute("Amount", x.Amount),
                                       new XAttribute("Due_Date", x.Due_Date),
                                       new XAttribute("Installment_Name", x.Installment_Name),
                                       new XAttribute("Installment_Type", x.Installment_Type),
                                       new XAttribute("Status", x.Status),
                                       new XAttribute("File_Id", x.File_Id)
                                       ))).ToString();

                var del = db.Sp_Delete_FileInstallment_Plan(Fileid);

                using (var Transcation = db.Database.BeginTransaction())
                {
                    try
                    {
                        var res1 = Convert.ToBoolean(db.Sp_Add_FileInstallmentsPlan(installmentplan, Fileid).FirstOrDefault());
                        FileInstallmentStatus res = new FileInstallmentStatus { Status = res1, Installments = file_Installments, Rate = rate, Total = total, Grand_Total = grandtotal };
                        Transcation.Commit();
                        return res;
                    }
                    catch (Exception e)
                    {
                        Transcation.Rollback();
                        throw e;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileInstallmentStatus AddInstallmentPlan_UnTransact(List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure, string DevCharStatus, long Fileid, DateTime Date)
        {
            List<File_Installments> file_Installments = new List<File_Installments>();
            decimal rate = 0, total = 0, grandtotal = 0;

            try
            {
                foreach (var item in installmentstructure)
                {
                    if (item.Installment_Type == "1") // for type installments
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        var eachinst = item.Amount / item.Rate;

                        for (int i = 1; i <= item.Rate; i++)
                        {
                            DateTime a = Date;
                            a = a.AddMonths(i);
                            DateTime dt = new DateTime(a.Year, a.Month, 1);

                            File_Installments fi = new File_Installments()
                            {
                                Status = InstallmentPaymentStatus.Pending.ToString(),
                                Due_Date = dt,
                                Amount = eachinst,
                                File_Id = Fileid,
                                Installment_Name = i + " Monthly Installment",
                                Installment_Type = "1",
                            };
                            file_Installments.Add(fi);
                        }
                    }
                    else if (item.Installment_Type == "4") // for type installments
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        var eachinst = item.Amount / item.After;
                        var itemaft = item.After;
                        for (int i = 1; i <= item.After; i++)
                        {
                            DateTime a = Date;
                            a = a.AddMonths(Convert.ToInt16(itemaft));
                            DateTime dt = new DateTime(a.Year, a.Month, 1);

                            File_Installments fi = new File_Installments()
                            {
                                Status = InstallmentPaymentStatus.Pending.ToString(),
                                Due_Date = dt,
                                Amount = Convert.ToDecimal(eachinst),
                                File_Id = Fileid,
                                Installment_Name = i + " - " + item.Installment_Name,
                                Installment_Type = "4",
                            };
                            file_Installments.Add(fi);
                            itemaft += item.After;
                        }

                    }
                    else if (item.Installment_Type == "2") // Development Charges
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        DateTime a = Date;
                        a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                        DateTime dt = new DateTime(a.Year, a.Month, 1);
                        var devamt = (DevCharStatus == "true") ? item.Amount : 0;
                        File_Installments fi = new File_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = dt,
                            Amount = devamt,
                            File_Id = Fileid,
                            Installment_Name = item.Installment_Name,
                            Installment_Type = "2",
                        };
                        if (DevCharStatus == "false")
                        {
                            fi.Amount = 0;
                        }
                        file_Installments.Add(fi);
                    }
                    else if (item.Installment_Type == "3") // Development Charges
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        DateTime a = Date;
                        File_Installments fi = new File_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = a,
                            Amount = item.Amount,
                            File_Id = Fileid,
                            Installment_Name = item.Installment_Name,
                            Installment_Type = "3",
                        };
                        file_Installments.Add(fi);
                    }
                    else if (item.Installment_Type == "0")
                    {
                        rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                        DateTime a = Date;
                        a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                        DateTime dt = new DateTime(a.Year, a.Month, 1);
                        File_Installments fi = new File_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = dt,
                            Amount = item.Amount,
                            File_Id = Fileid,
                            Installment_Name = item.Installment_Name,
                            Installment_Type = "0",
                        };
                        file_Installments.Add(fi);
                    }
                }
                var installmentplan = new XElement("Installments", file_Installments.Select(x => new XElement("Installmentsdata",
                                       new XAttribute("Amount", x.Amount),
                                       new XAttribute("Due_Date", x.Due_Date),
                                       new XAttribute("Installment_Name", x.Installment_Name),
                                       new XAttribute("Installment_Type", x.Installment_Type),
                                       new XAttribute("Status", x.Status),
                                       new XAttribute("File_Id", x.File_Id)
                                       ))).ToString();

                try
                {
                    var res1 = Convert.ToBoolean(db.Sp_Add_FileInstallmentsPlan(installmentplan, Fileid).FirstOrDefault());
                    FileInstallmentStatus res = new FileInstallmentStatus { Status = res1, Installments = file_Installments, Rate = rate, Total = total, Grand_Total = grandtotal };
                    return res;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileInstallmentStatus AddInstallmentPlanTest(List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure, string DevCharStatus, long Fileid, DateTime d)
        {
            List<File_Installments> file_Installments = new List<File_Installments>();
            decimal rate = 0, total = 0, grandtotal = 0;

            foreach (var item in installmentstructure)
            {
                if (item.Installment_Type == "1") // for type installments
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    var eachinst = item.Amount / item.Rate;

                    for (int i = 1; i <= item.Rate; i++)
                    {
                        DateTime a = d;
                        a = a.AddMonths(i);
                        DateTime dt = new DateTime(a.Year, a.Month, 1);

                        File_Installments fi = new File_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = dt,
                            Amount = eachinst,
                            File_Id = Fileid,
                            Installment_Name = i + " Monthly Installment",
                            Installment_Type = "1",
                        };
                        file_Installments.Add(fi);
                    }
                }
                else if (item.Installment_Type == "2") // Development Charges
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    DateTime a = d;
                    a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                    DateTime dt = new DateTime(a.Year, a.Month, 1);
                    File_Installments fi = new File_Installments()
                    {
                        Status = InstallmentPaymentStatus.Pending.ToString(),
                        Due_Date = dt,
                        Amount = item.Amount,
                        File_Id = Fileid,
                        Installment_Name = item.Installment_Name,
                        Installment_Type = "2",
                    };
                    if (DevCharStatus == "false")
                    {
                        fi.Amount = 0;
                    }
                    file_Installments.Add(fi);
                }
                else if (item.Installment_Type == "3") // Development Charges
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    DateTime a = d;
                    a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                    DateTime dt = new DateTime(a.Year, a.Month, 1);
                    File_Installments fi = new File_Installments()
                    {
                        Status = InstallmentPaymentStatus.Pending.ToString(),
                        Due_Date = dt,
                        Amount = item.Amount,
                        File_Id = Fileid,
                        Installment_Name = item.Installment_Name,
                        Installment_Type = "3",
                    };
                    file_Installments.Add(fi);
                }
                else
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    DateTime a = d;
                    a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                    DateTime dt = new DateTime(a.Year, a.Month, 1);
                    File_Installments fi = new File_Installments()
                    {
                        Status = InstallmentPaymentStatus.Pending.ToString(),
                        Due_Date = dt,
                        Amount = item.Amount,
                        File_Id = Fileid,
                        Installment_Name = item.Installment_Name,
                        Installment_Type = "0",
                    };
                    file_Installments.Add(fi);
                }
            }
            var installmentplan = new XElement("Installments", file_Installments.Select(x => new XElement("Installmentsdata",
                                   new XAttribute("Amount", x.Amount),
                                   new XAttribute("Due_Date", x.Due_Date),
                                   new XAttribute("Installment_Name", x.Installment_Name),
                                   new XAttribute("Installment_Type", x.Installment_Type),
                                   new XAttribute("Status", x.Status),
                                   new XAttribute("File_Id", x.File_Id)
                                   ))).ToString();
            var res1 = Convert.ToBoolean(db.Sp_Add_FileInstallmentsPlan(installmentplan, Fileid).FirstOrDefault());
            FileInstallmentStatus res = new FileInstallmentStatus { Status = res1, Installments = file_Installments, Rate = rate, Total = total, Grand_Total = grandtotal };
            return res;
        }
        public CommercialInstallmentsReturn AddCommercialInstallmentPlan(List<Sp_Get_CommercialInstallmentStructure_Result> installmentstructure, long? CommId)
        {
            List<Commercial_Installments> com_Installments = new List<Commercial_Installments>();
            decimal rate = 0, total = 0, grandtotal = 0;
            //var res = db.Plot_Rates.SingleOrDefault(x => x.Plot_Com_Id == CommId);
            //var Totalval = res.Final_Rate * res.Total_SqFt_Marlas;
            foreach (var item in installmentstructure)
            {

                //if (item.Installment_Type == "1") // for type installments
                //{
                //    var EacInstAmt = ((Totalval * item.) / 100) / item.Total_Inst;
                //    var time = item.After;
                //    for (int i = 1; i <= item.Total_Inst; i++)
                //    {
                //        DateTime a = DateTime.UtcNow;
                //        a = a.AddMonths(time);
                //        DateTime dt = new DateTime(a.Year, a.Month, 10);

                //        Commercial_Installments fi = new Commercial_Installments()
                //        {
                //            Status = InstallmentPaymentStatus.Pending.ToString(),
                //            Due_Date = dt,
                //            Amount = Convert.ToDecimal(EacInstAmt),
                //            Com_Id = Convert.ToInt64(CommId),
                //            Installment_Name = i + " " + item.Installment_Name,
                //            Installment_Type = 1,
                //        };
                //        com_Installments.Add(fi);
                //        time += item.After;
                //    }
                //}

                if (item.Installment_Type == "1") // for type installments
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    var eachinst = item.Amount / item.Rate;
                    for (int i = 1; i <= item.Amount; i++)
                    {
                        DateTime a = DateTime.UtcNow;
                        a = a.AddMonths(i);
                        DateTime dt = new DateTime(a.Year, a.Month, 10);

                        Commercial_Installments fi = new Commercial_Installments()
                        {
                            //Status = InstallmentPaymentStatus.Pending.ToString(),
                            //Due_Date = dt,
                            //Amount = Convert.ToDecimal(eachinst),
                            //Com_Id = Convert.ToInt64(CommId),
                            //Installment_Name = i + " " + item.Installment_Name,
                            //Installment_Type = 1,

                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            Due_Date = dt,
                            Amount = eachinst,
                            Com_Id = Convert.ToUInt32(CommId),
                            Installment_Name = i + " Monthly Installment",
                            Installment_Type = 1,

                        };
                        com_Installments.Add(fi);
                        //time += item.After_Time;
                    }
                }
                else if (item.Installment_Type == "2") // Booking Charges
                {
                    //var InstAmt = ((Totalval * item.p) / 100);
                    //DateTime a = DateTime.UtcNow;
                    //DateTime dt = new DateTime(a.Year, a.Month, 10);

                    //Commercial_Installments fi = new Commercial_Installments()
                    //{
                    //    Status = InstallmentPaymentStatus.Pending.ToString(),
                    //    Due_Date = dt,
                    //    Amount = Convert.ToDecimal(InstAmt),
                    //    Com_Id = Convert.ToInt64(CommId),
                    //    Installment_Name = item.Installment_Name,
                    //    Installment_Type = 1,
                    //};
                    //com_Installments.Add(fi);
                }
                else if (item.Installment_Type == "3") // Confirmation Charges
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    DateTime a = DateTime.UtcNow;
                    a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                    DateTime dt = new DateTime(a.Year, a.Month, 10);

                    Commercial_Installments fi = new Commercial_Installments()
                    {
                        Status = InstallmentPaymentStatus.Pending.ToString(),
                        Due_Date = dt,
                        Amount = item.Amount,
                        Com_Id = Convert.ToInt64(CommId),
                        Installment_Name = item.Installment_Name,
                        Installment_Type = 1,
                    };
                    com_Installments.Add(fi);
                }
                //else if (item.Installment_Type == "4") // Possession
                //{
                //    var InstAmt = ((Totalval * item.Percentage) / 100);
                //    DateTime a = DateTime.UtcNow;
                //    a = a.AddMonths(item.After);
                //    DateTime dt = new DateTime(a.Year, a.Month, 10);

                //    Commercial_Installments fi = new Commercial_Installments()
                //    {
                //        Status = InstallmentPaymentStatus.Pending.ToString(),
                //        Due_Date = dt,
                //        Amount = Convert.ToDecimal(InstAmt),
                //        Com_Id = Convert.ToInt64(CommId),
                //        Installment_Name = item.Installment_Name,
                //        Installment_Type = 1,
                //    };
                //    com_Installments.Add(fi);
                //}
                else
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    DateTime a = DateTime.UtcNow;
                    a = a.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                    DateTime dt = new DateTime(a.Year, a.Month, 10);
                    Commercial_Installments fi = new Commercial_Installments()
                    {
                        Status = InstallmentPaymentStatus.Pending.ToString(),
                        Due_Date = dt,
                        Amount = item.Amount,
                        Com_Id = Convert.ToInt64(CommId),
                        Installment_Name = item.Installment_Name,
                        Installment_Type = 1,
                    };
                    com_Installments.Add(fi);
                }
            }
            var installmentplan = new XElement("Installments", com_Installments.Select(x => new XElement("Installmentsdata",
                                   new XAttribute("Amount", x.Amount),
                                   new XAttribute("Due_Date", x.Due_Date),
                                   new XAttribute("Installment_Name", x.Installment_Name),
                                   new XAttribute("Installment_Type", x.Installment_Type),
                                   new XAttribute("Status", x.Status),
                                   new XAttribute("Com_Id", x.Com_Id)
                                   ))).ToString();
            var res1 = Convert.ToBoolean(db.Sp_Add_CommercialInstallmentsPlan(installmentplan, CommId, "Commercial").FirstOrDefault());
            CommercialInstallmentsReturn res = new CommercialInstallmentsReturn { Status = res1, CommercialInstallments = com_Installments, Rate = rate, Total = total, Grand_Total = grandtotal };
            //return new CommercialInstallmentsReturn { Status = res1, Module = ProjectCategory.Commercial.ToString() };
            return res;
        }

        public ActionResult AddInstallment()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }
        [HttpPost]
        public JsonResult CheckInstallment(decimal Amount, decimal Balance, long Filefromid)
        {
            List<Sp_Get_FileInstallments_Result> Allinstallments = db.Sp_Get_FileInstallments(Filefromid).ToList();
            var res = Allinstallments.Where(x => x.Status != "Paid" && x.Installment_Type == "1").ToList();
            decimal Actamt = Amount;
            if (Balance >= 0)
                Actamt = Amount + Balance;

            decimal TotalAmt = 0, AmttoPaid = 0;
            int ActualAmt = (int)Actamt;
            List<AmountToPaidInfo> latpi = new List<AmountToPaidInfo>();


            foreach (var item in res)
            {
                AmountToPaidInfo atpi = new AmountToPaidInfo();
                TotalAmt += item.Amount;
                int TtlAmt = (int)TotalAmt;
                if (TtlAmt <= ActualAmt)
                {
                    AmttoPaid += item.Amount;
                    atpi.Id = item.Id;
                    atpi.Amount = item.Amount;
                    atpi.Installment_Name = item.Installment_Name;
                    atpi.Months = string.Format("{0:MMM}", item.Due_Date);
                    latpi.Add(atpi);
                }
                else
                {
                    break;
                }
            }
            if (!latpi.Any())
            {
                if (Balance < Amount)
                {
                    AmountToPaidInfo atpi = new AmountToPaidInfo();
                    var Curentinst = res.FirstOrDefault();
                    atpi.Id = Curentinst.Id;
                    atpi.Amount = Curentinst.Amount;
                    atpi.Installment_Name = Curentinst.Installment_Name;
                    atpi.Months = string.Format("{0:MMM}", Curentinst.Due_Date);
                    latpi.Add(atpi);
                }
            }

            return Json(latpi);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayInstallment(long[] Ids, long Filefromid, ReceiptData rd, string Paymentfor, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var appdetail = db.Sp_Get_FileData(Filefromid).FirstOrDefault();
            
            var balance = db.File_Plot_Balance.Where(x => x.File_Plot_Id == appdetail.Id && x.Module == Modules.FileManagement.ToString()).FirstOrDefault();
            var rem = Math.Round( Convert.ToDouble( balance.Outstanding_Amount), 0 ) - Math.Round(Convert.ToDouble(rd.Amount), 0);
            if (rem < 0)
            {
                return Json( new { Status = false, Msg = "Amount is Greater than Outstanding Amount" });
            }

            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var re = db.Sp_Update_FileVerificationToNull(Filefromid);
            var lastowner = db.Sp_Get_FileLastOwner(appdetail.Id).ToList();
            Helpers H = new Helpers();

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {

                    var res2 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, string.Join(" , ", lastowner.Select(x => x.Mobile_1).Distinct())
                  , string.Join(" , ", lastowner.Select(x => x.Father_Husband).Distinct()), appdetail.Id, string.Join(" , ", lastowner.Select(x => x.Name).Distinct()), rd.PaymentType, rd.TotalAmount,
                  rd.Project_Name, rd.Rate, null, rd.Plot_Size, ReceiptTypes.Installment.ToString(), userid, userid, Paymentfor, null, Modules.FileManagement.ToString(), rd.DevCharges, rd.FilePlotNumber.ToString(), appdetail.Block, appdetail.Type, lastowner.Select(x => x.Group_Tag).FirstOrDefault(), TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                    if (res2.Receipt_Id > 0)
                    {
                        //bool headcashier = false;
                        //if (User.IsInRole("Head Cashier"))
                        //{
                        //    headcashier = true;
                        //}

                        string ReceivableModule = "";
                        if (appdetail.Type == PlotType.Commercial.ToString())
                        {
                            ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Commercial.ToString();
                        }
                        else
                        {
                            ReceivableModule = COA_Mapper_ModuleTypes.Plots_Receivables_Residential.ToString();
                        }

                        string Narration = "Amount Against Plot Number :" + rd.FilePlotNumber + " - " + appdetail.Type + " Block: " + appdetail.Block;
                        //int line = 1;
                        //if (headcashier)
                        //{
                        //    if (rd.PaymentType == "Cash")
                        //    {
                        //        var Payment = ah.Payment_Head(rd.PaymentType, Transaction_Type.Debit.ToString(), null, comp.Id);
                        //        var File_COA = ah.HeadFinder(AccountingModulePlots, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, appdetail.Block_Id);//this.Plot_Head(Plot_No, Type, Block);
                        //        var debit = db.Sp_Add_Journal_Entry(rd.Amount, 0, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Narration, TransactionId, line, userid, userid, null, rd.Bank, rd.PayChqNo, Payment.PaymentStatus, rd.Ch_bk_Pay_Date, null, "", null, res2.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                        //        var credit = db.Sp_Add_Journal_Entry(0, rd.Amount, File_COA.Text_ChartCode + " - " + File_COA.Head, File_COA.Id, File_COA.Text_ChartCode, File_COA.Head, Narration, TransactionId, line, userid, userid, null, rd.Bank, rd.PayChqNo, Payment.PaymentStatus, rd.Ch_bk_Pay_Date, null, "", null, res2.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                        //    }
                        //}
                        //else
                        //{
                        //    if (rd.PaymentType == "Cash")
                        //    {
                        //        var Payment = ah.Payment_Head(rd.PaymentType, Transaction_Type.Debit.ToString(), null, comp.Id);
                        //        var File_COA = ah.HeadFinder(AccountingModulePlots, ReceivableModule, COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, appdetail.Block_Id);//this.Plot_Head(Plot_No, Type, Block);
                        //        var Reciv_Debit = db.Sp_Add_General_Entry(rd.Amount, 0, rd.Bank, rd.PayChqNo, Payment.PaymentStatus, rd.Ch_bk_Pay_Date, Payment.PaymentData.Text_ChartCode + " - " + Payment.PaymentData.Head, Payment.PaymentData.Id, Payment.PaymentData.Text_ChartCode, Payment.PaymentData.Head, Narration, TransactionId, line, null, null, null, userid, res2.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();
                        //        var Receivable_Credit = db.Sp_Add_General_Entry(0, rd.Amount, rd.Bank, rd.PayChqNo, Payment.PaymentStatus, rd.Ch_bk_Pay_Date, File_COA.Text_ChartCode + " - " + File_COA.Head, File_COA.Id, File_COA.Text_ChartCode, File_COA.Head, Narration, TransactionId, line, null, null, null, userid, res2.Receipt_No, Payment.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                        //    }
                        //}
                    }
                    string text = "";
                    if (rd.PaymentType == "Cash")
                    {
                        text = "Dear " + rd.Name + ",\n\r" +
                     "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in cash for File number " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                    }
                    else
                    {
                        var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                    Modules.FileManagement.ToString(), Types.Installment.ToString(), userid, rd.PayChqNo, Filefromid, rd.Ch_bk_Pay_Date, rd.FilePlotNumber, res2.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                        if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                        }
                        string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                        var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                        var Images = H.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());

                        text = "Dear " + rd.Name + ",\n\r" +
                                                "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for File number " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

                    }
                    try
                    {
                        foreach (var v in lastowner)
                        {
                            SmsService smsService = new SmsService();
                            smsService.SendMsg(text, v.Mobile_1);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    Transaction.Commit();
                    var res = new { Status = true, Receiptid = res2.Receipt_No, Token = userid };
                    return Json(res);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    var res = new { Status = false, Msg = "Installment Cannot Be Received. Contact SA Systems" };
                    return Json(res);
                }
            }
        }
        public ActionResult InstallmentDetails()
        {
            var res = (from x in db.Installment_Structure
                       join y in db.Installment_Plot_Category on x.Installment_Plot_Id equals y.Id
                       select new Installment_Struct
                       {
                           After = x.After,
                           Amount = x.Amount,
                           Datetime = x.Datetime,
                           Id = x.Id,
                           Installment_Name = x.Installment_Name,
                           Installment_Plot_Id = x.Installment_Plot_Id,
                           Installment_Type = x.Installment_Type,
                           Module = x.Module,
                           Rate = x.Rate,
                           Plot_Size = y.Plot_Size + " Marla"
                       }).ToList();
            return PartialView(res);
        }
        public ActionResult InstallmentDetailsCommercial()
        {
            var res = db.Commercial_Installment_Structure.ToList();
            return PartialView(res);
        }
        public ActionResult OtherInstallments(string Name, decimal Amount, long Id)
        {
            ViewBag.Name = Name;
            ViewBag.Amount = Amount;
            ViewBag.Id = Id;
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }
        public ActionResult AddInstallmentTest()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayInstallmentTest(long?[] Ids, long Filefromid, ReceiptData rd, string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //var installmentids = new XElement("InstallmentIds", Ids.Select(x => new XElement("InsIds",
            //                      new XAttribute("Id", x)
            //                      ))).ToString();
            var re = db.Sp_Update_FileVerificationToNull(Filefromid);

            var appdetail = db.File_Form.Where(x => x.Id == Filefromid).FirstOrDefault();

            db.Sp_Add_FileComments(Filefromid, "Add " + rd.PaymentType + " Receipt of Amount " + rd.Amount + " with receipt no. " + rd.ReceiptNo + " date: " + string.Format("{0:dd-MMM-yyyy}", rd.Date), userid, "Text");
            // db.SP_Files_Log("Create", "Receive Installment of file " + rd.File_Plot_Number + " of Amount " + rd.Amount, "File Installments", Request.UserHostAddress, userid, Filefromid);
            if (rd.PaymentType == "Cash" || rd.PaymentType == "Online_Cash")
            {
                //var res1 = db.Sp_Update_Installments_Cash_Status(installmentids, rd.Amount).FirstOrDefault();
                if (rd.PaymentType == "Online_Cash")
                    rd.PayChqNo = rd.PayChqNo + "(Bank Acc No)";
                var res2 = db.Sp_Add_Receipt_Manual(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
                    , rd.Father_Husband, Filefromid, rd.Name, rd.PaymentType, rd.TotalAmount,
                    Modules.FileManagement.ToString(), rd.Rate, null, rd.Plot_Size, Type, userid, userid, rd.ReceiptNo, rd.Date, rd.DevCharges, rd.File_Plot_Number.ToString(), appdetail.Block, appdetail.Type).FirstOrDefault();
                var res = new { Receiptid = res2, Token = userid };
                var res3 = db.Receipts.Where(x => x.Id == res2).Select(x => x.ReceiptNo).FirstOrDefault();
                bool headcashier = false;
                if (User.IsInRole("Head Cashier"))
                {
                    headcashier = true;
                }
                try
                {
                    AccountHandlerController de = new AccountHandlerController();
                    de.Receive_Plot_Amount(rd.Amount, rd.FilePlotNumber, appdetail.Type, appdetail.Block, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, new Helpers().RandomNumber(), userid, res3, 1, headcashier, AccountingModulePlots, Convert.ToInt64(appdetail.Block_Id));
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
                return Json(res);
            }
            else
            {
                var res2 = db.Sp_Add_Receipt_Manual(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
                    , rd.Father_Husband, Filefromid, rd.Name, rd.PaymentType, rd.TotalAmount,
                    Modules.FileManagement.ToString(), rd.Rate, null, rd.Plot_Size, Type, userid, userid, rd.ReceiptNo, rd.Date, rd.DevCharges, rd.File_Plot_Number.ToString(), appdetail.Block, appdetail.Type).FirstOrDefault();

                var res = new { Receiptid = res2, Token = Filefromid };
                var res3 = db.Receipts.Where(x => x.Id == res2).Select(x => x.ReceiptNo).FirstOrDefault();
                bool headcashier = false;
                if (User.IsInRole("Head Cashier"))
                {
                    headcashier = true;
                }
                try
                {
                    AccountHandlerController de = new AccountHandlerController();
                    de.Receive_Plot_Amount(rd.Amount, rd.FilePlotNumber, appdetail.Type, appdetail.Block, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, new Helpers().RandomNumber(), userid, res3, 1, headcashier, AccountingModulePlots, Convert.ToInt64(appdetail.Block_Id));
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
                return Json(res);
            }
        }
        public ActionResult AddPlotInstallment(long Id, string Name, string Father, string Mobile)
        {
            ViewBag.Plotid = Id;
            ViewBag.Name = Name;
            ViewBag.Father = Father;
            ViewBag.Mobile = Mobile;
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView();
        }
        public ActionResult AddCommercialInstallment(long Id, string Name, string Father, string Mobile)
        {
            ViewBag.shopid = Id;
            ViewBag.Name = Name;
            ViewBag.Father = Father;
            ViewBag.Mobile = Mobile;
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayCommercialInstallment(ReceiptData rd, string Module, string CameraImg)
        {
            //Hash
            long userid = long.Parse(User.Identity.GetUserId());
            var res2 = db.Sp_Add_PlotReceipt_Manual(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
                , rd.Father_Husband, rd.File_Plot_Number.ToString(), rd.Name, rd.PaymentType, rd.TotalAmount,
                rd.Project_Name, rd.Rate, null, rd.Plot_Size, Module, userid, userid, rd.ReceiptNo, rd.Date, rd.Phase, rd.Block, null, Modules.CommercialManagement.ToString(), rd.FilePlotNumber).FirstOrDefault();

            int index = 0;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                try
                {
                    var imageName = Path.GetExtension(hpf.FileName);
                    if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                    }
                    var filename = res2 + Path.GetExtension(hpf.FileName);
                    string savedFileName = Path.Combine(Server.MapPath("~/Repository/Cheques/" + rd.File_Plot_Number + "/"), filename);
                    hpf.SaveAs(savedFileName);
                    //db.Sp_Update_PlotReceipt_Img(res2, filename);

                }
                catch (Exception)
                {
                }
            }
            if (rd.PaymentType != "Cash")
            {

                var text = "Dear " + rd.Name + ",\n\r" +
            "A Payment of Rs " + string.Format("{0:n}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for Shop number# " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

                try
                {
                    SmsService smsService = new SmsService();
                    smsService.SendMsg(text, rd.Mobile_1);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                var text = "Dear " + rd.Name + ",\n\r" +
           "A Payment of Rs " + string.Format("{0:n}", rd.Amount) + " has been received in cash for Shop number# " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";

                try
                {
                    SmsService smsService = new SmsService();
                    smsService.SendMsg(text, rd.Mobile_1);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(CameraImg))
            {
                if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                }
                var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + rd.File_Plot_Number + "/"), res2 + ".jpg");

                Helpers h = new Helpers();
                var Imgres = h.SaveBase64Image(CameraImg, pathMain, res2.ToString());
            }
            var Receivedamts = db.Sp_Get_ReceivedCommercialAmount(rd.File_Plot_Number, "Building").ToList();
            var res = new { ReturnVal = res2, ReceAmt = Receivedamts };
            return Json(res);
        }
        //public ActionResult AddCommercialInstallment(long Id, decimal Amount, long InstallmentId)
        //{
        //    ViewBag.shopid = Id;
        //    ViewBag.InsAmount = Amount;
        //    ViewBag.insid = InstallmentId;
        //    ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
        //    Helpers h = new Helpers();
        //    ViewBag.TransactionId = h.RandomNumber();
        //    return PartialView();
        //}


        //public JsonResult PayCommercialInstallment(List<ReceiptData> Receiptdata, long ShopId, long InstallmentId, long TransactionId)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var owner = db.Commercial_Room_Transfer.Where(x => x.ComRom_Id == ShopId).FirstOrDefault();
        //    var Shop = db.Sp_Get_CommercialPlotRoom(ShopId).SingleOrDefault();

        //    foreach (var rd in Receiptdata)
        //    {
        //        var res3 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, owner.Mobile_1
        //                                                                        , owner.Father_Husband, ShopId, owner.Name, rd.PaymentType, 0,
        //                                                                         rd.Project_Name, 0, null, Convert.ToString(Shop.Area), ReceiptTypes.Installment.ToString(), userid, userid, "",
        //                                                                         null, Shop.Module, null, Shop.shop_no, Shop.Floor, 0, TransactionId).FirstOrDefault();
        //        db.Sp_Update_CommercialReceipts(InstallmentId);

        //        if (rd.PaymentType != "Cash")
        //        {

        //            var reds2 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
        //                       Modules.FileManagement.ToString(), Types.Installment.ToString(), ShopId, rd.PayChqNo, null, rd.Ch_bk_Pay_Date, Shop.shop_no, res3.Receipt_Id).FirstOrDefault());
        //            var text = "Dear " + owner.Name + ",\n\r" +
        //                       "A Payment of Rs " + string.Format("{0:n}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for File number# " + Shop.shop_no + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";


        //            if (!Directory.Exists(Server.MapPath("~/Repository/Commercial/" + "/")))
        //            {
        //                Directory.CreateDirectory(Server.MapPath("~/Repository/Commercial/"));
        //            }
        //            string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
        //            var pathMain = Path.Combine(Server.MapPath("~/Repository/Commercial/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");

        //            Helpers h = new Helpers();
        //            var Imgres = h.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());


        //            try
        //            {
        //                SmsService smsService = new SmsService();
        //                smsService.SendMsg(text, owner.Mobile_1);
        //            }
        //            catch (Exception)
        //            {

        //            }

        //        }
        //        else
        //        {
        //            var text = "Dear " + owner.Name + ",\n\r" +
        //                              "A Payment of Rs " + string.Format("{0:n}", rd.Amount) + " has been received in cash for File number# " + Shop.shop_no + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";

        //            try
        //            {
        //                SmsService smsService = new SmsService();
        //                smsService.SendMsg(text, owner.Mobile_1);
        //            }
        //            catch (Exception)
        //            {
        //            }

        //        }
        //        var result = new { Receiptid = res3.Receipt_No, Token = userid };
        //        return Json(result);



        //    }
        //    return Json(false);

        //    //     var res2 = db.Sp_Add_PlotReceipt_Manual(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
        //    //     , rd.Father_Husband, rd.File_Plot_Number.ToString(), rd.Name, rd.PaymentType, rd.TotalAmount,
        //    //     rd.Project_Name, rd.Rate, null, rd.Plot_Size, Module, userid, userid, rd.ReceiptNo, rd.Date, rd.Phase, rd.Block, null, Modules.ShopManagement.ToString(), rd.FilePlotNumber).FirstOrDefault();

        //    // int index = 0;
        //    // foreach (string file in Request.Files)
        //    // {
        //    //     HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
        //    //     index++;
        //    //     if (hpf.ContentLength == 0)
        //    //         continue;
        //    //     try
        //    //     {
        //    //         var imageName = Path.GetExtension(hpf.FileName);
        //    //         if (!Directory.Exists(Server.MapPath("~/Repository/Commercial/" + "/")))
        //    //         {
        //    //             Directory.CreateDirectory(Server.MapPath("~/Repository/Commercial/"));
        //    //         }
        //    //         var filename = res2 + Path.GetExtension(hpf.FileName);
        //    //         string savedFileName = Path.Combine(Server.MapPath("~/Repository/Commercial/" + rd.File_Plot_Number + "/"), filename);
        //    //         hpf.SaveAs(savedFileName);
        //    //         db.Sp_Update_PlotReceipt_Img(res2, filename);

        //    //     }
        //    //     catch (Exception)
        //    //     {
        //    //     }
        //    // }
        //    // if (rd.PaymentType != "Cash")
        //    // {

        //    //     var text = "Dear " + rd.Name + ",\n\r" +
        //    // "A Payment of Rs " + string.Format("{0:n}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for Shop number# " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

        //    //     try
        //    //     {
        //    //         SmsService smsService = new SmsService();
        //    //         smsService.SendMsg(text, rd.Mobile_1);
        //    //     }
        //    //     catch (Exception ex)
        //    //     {
        //    //         return null;
        //    //     }
        //    // }
        //    // else
        //    // {
        //    //     var text = "Dear " + rd.Name + ",\n\r" +
        //    //"A Payment of Rs " + string.Format("{0:n}", rd.Amount) + " has been received in cash for Shop number# " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";

        //    //     try
        //    //     {
        //    //         SmsService smsService = new SmsService();
        //    //         smsService.SendMsg(text, rd.Mobile_1);
        //    //     }
        //    //     catch (Exception ex)
        //    //     {
        //    //         return null;
        //    //     }
        //    // }
        //    // if (!string.IsNullOrEmpty(CameraImg))
        //    // {
        //    //     if (!Directory.Exists(Server.MapPath("~/Repository/Commercial/" + "/")))
        //    //     {
        //    //         Directory.CreateDirectory(Server.MapPath("~/Repository/Commercial/"));
        //    //     }
        //    //     var pathMain = Path.Combine(Server.MapPath("~/Repository/Commercial/" + rd.File_Plot_Number + "/"), res2 + ".jpg");

        //    //     Helpers h = new Helpers();
        //    //     var Imgres = h.SaveBase64Image(CameraImg, pathMain, res2.ToString());
        //    // }
        //    // var Receivedamts = db.Sp_Get_ReceivedCommercialAmount(rd.File_Plot_Number, Modules.ShopManagement.ToString()).ToList();

        //}
        // Cash Counter Image/File Uploader

        [HttpPost]
        public ActionResult UploadInstrument(FormCollection fc)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    var provider = fc.ToValueProvider();
                    var dt = "";
                    //var unit_num = "";
                    if (provider.ContainsPrefix("pltid"))
                    {
                        dt = provider.GetValue("pltid").AttemptedValue;
                        //long unit_Id = Convert.ToInt64(dt);
                        //unit_num = db.Plots.Where(x => x.Id == unit_Id).FirstOrDefault().Plot_Number;
                        // using the `dt`
                    }
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        string CPDONumber = "";
                        string fileExtension = "";
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                            fileExtension = ".jpg";//Path.GetExtension(fname);
                        }
                        if (provider.ContainsPrefix("cpdnum"))
                        {
                            if (provider.GetValue("cpdnum").AttemptedValue == "")
                            {
                                return Json(new { Status = false, Msg = "Enter Instrument Number to Proceed." });
                            }
                            else {
                                CPDONumber = provider.GetValue("cpdnum").AttemptedValue + fileExtension;
                            }


                        }
                        if (!Directory.Exists(Server.MapPath("~/Repository/Instruments/Plots/" + dt + "/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Repository/Instruments/Plots/" + dt));
                        }
                        string filepath = Path.Combine(Server.MapPath("~/Repository/Instruments/Plots/" + dt + "/"), CPDONumber);
                        file.SaveAs(filepath);
                    }

                    return Json(new { Status = true, Msg = "File Uploaded Successfully." });
                    //return Json(true);
                }
                catch (Exception ex)
                {
                    return Json(new { Status = false, Msg = "Error occurred. Error details: " + ex.Message });
                    //return Json();
                }
            }
            else
            {
                return Json(new { Status = false, Msg = "No files selected." });
                //return Json("No files selected.");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayPlotInstallment(ReceiptData rd, string Module, string CameraImg)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var Plot = db.Sp_Get_PlotData(rd.File_Plot_Number).FirstOrDefault();
            var res2 = db.Sp_Add_PlotReceipt_Manual(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
                , rd.Father_Husband, rd.File_Plot_Number.ToString(), rd.Name, rd.PaymentType, rd.TotalAmount,
                rd.Project_Name, rd.Rate, null, rd.Plot_Size, Module, userid, userid, rd.ReceiptNo, rd.Date, rd.Phase, rd.Block, null, Modules.PlotManagement.ToString(), rd.FilePlotNumber).FirstOrDefault();
            db.Sp_Add_Activity(userid, "Add receipt of Plot Id  <a class='plt-data' data-id=' " + rd.File_Plot_Number + "'>" + rd.File_Plot_Number + "</a>---", "Create", Modules.PlotManagement.ToString(), ActivityType.Add_Receipt.ToString(), rd.File_Plot_Number);
            Helpers h = new Helpers();
            bool headcashier = false;
            if (User.IsInRole("Head Cashier"))
            {
                headcashier = true;
            }
            try
            {
                AccountHandlerController de = new AccountHandlerController();
                de.Receive_Plot_Amount(rd.Amount, Plot.Plot_No, Plot.Type, Plot.Block_Name, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, h.RandomNumber(), userid, rd.ReceiptNo, 1, headcashier, AccountingModulePlots, Plot.BlockIden);
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            }

            int index = 0;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                try
                {
                    var imageName = Path.GetExtension(hpf.FileName);
                    if (!Directory.Exists(Server.MapPath("~/PlotReceipts/Plots/" + rd.File_Plot_Number + "/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/PlotReceipts/Plots/" + rd.File_Plot_Number));
                    }
                    var filename = res2 + Path.GetExtension(hpf.FileName);
                    string savedFileName = Path.Combine(Server.MapPath("~/PlotReceipts/Plots/" + rd.File_Plot_Number + "/"), filename);
                    hpf.SaveAs(savedFileName);
                    //db.Sp_Update_PlotReceipt_Img(res2, filename);
                }
                catch (Exception)
                {
                }
            }
            if (!string.IsNullOrEmpty(CameraImg))
            {
                if (!Directory.Exists(Server.MapPath("~/PlotReceipts/Plots/" + rd.File_Plot_Number + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/PlotReceipts/Plots/" + rd.File_Plot_Number));
                }
                var pathMain = Path.Combine(Server.MapPath("~/PlotReceipts/Plots/" + rd.File_Plot_Number + "/"), res2 + ".png");
                var Imgres = h.SaveBase64Image(CameraImg, pathMain, res2.ToString());
            }
            var Receivedamts = db.Sp_Get_ReceivedAmounts(rd.File_Plot_Number, Modules.PlotManagement.ToString()).ToList();
            db.SP_Update_PlotVerificationToNull(rd.File_Plot_Number);
            var res = new { ReturnVal = res2, ReceAmt = Receivedamts };
            return Json(res);
        }
        public JsonResult DeleteReceipt(long Id, long Plotid)
        {
            int userid = int.Parse(User.Identity.GetUserId());
            var rec = db.Receipts.Where(x => x.Id == Id).SingleOrDefault();
            string recedetails = JsonConvert.SerializeObject(rec);

            db.Sp_Add_Activity(userid, "Delete receipt of Plot Id  <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>---" + recedetails, "Delete", Modules.PlotManagement.ToString(), ActivityType.Delete_Receipt.ToString(), Plotid);
            db.Sp_Add_PlotComments(Plotid, "Delete Receipt of No. : " + rec.ReceiptNo + " Amount: " + rec.Amount + " of " + rec.PaymentType, userid, ActivityType.Delete_Receipt.ToString());


            //try
            //{
            //    AccountHandlerController de = new AccountHandlerController();
            //    de.Banking_Receipts_Deletion(Id, userid);
            //}
            //catch (Exception ex)
            //{
            //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            //}
            var r = db.Sp_Delete_Receipt(Id).FirstOrDefault();


            var res = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).ToList();
            db.SP_Update_PlotVerificationToNull(Plotid);

            return Json(res);
        }
        public JsonResult VerifyReceipt(long Id, bool Status)
        {
            int userid = int.Parse(User.Identity.GetUserId());
            db.Sp_Update_ReceiptVerification(Id, Status);
            db.Sp_Add_Activity(userid, "verify receipt with id <a class='receip-data' data-id=' " + Id + "'>" + Id + "</a>", "Update", Modules.PlotManagement.ToString(), ActivityType.Receipt_Verified.ToString(), Id);
            return Json(true);
        }
        public JsonResult VerifyReceiptCommercial(long Id, bool Status)
        {
            int userid = int.Parse(User.Identity.GetUserId());
            db.Sp_Update_ReceiptVerification(Id, Status);
            db.Sp_Add_Activity(userid, "verify receipt with id <a class='receip-data' data-id=' " + Id + "'>" + Id + "</a>", "Update", Modules.PlotManagement.ToString(), ActivityType.Receipt_Verified.ToString(), Id);
            return Json(true);
        }
        public JsonResult VerifyFileReceipt(long Id, bool Status)
        {
            int userid = int.Parse(User.Identity.GetUserId());
            db.Sp_Update_ReceiptVerification(Id, Status);
            db.Sp_Add_Activity(userid, "verify receipt with id <a class='receip-data' data-id=' " + Id + "'>" + Id + "</a>", "Update", Modules.FileManagement.ToString(), ActivityType.Receipt_Verified.ToString(), Id);
            return Json(true);
        }
        public JsonResult UpdatePlotInstallments(List<PlotsInstallments> PI, long Plotid, decimal? Plot_Price, decimal? Discount)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var Installments = new XElement("Plots", PI.Select(x => new XElement("Installments",
               new XAttribute("Amount", x.Amount),
               new XAttribute("DueDate", x.DueDate),
               new XAttribute("Installment_Name", x.InstNo)
               ))).ToString();
            db.Sp_Update_PlotInstallments(Plotid, Plot_Price, Discount, userid, Installments);
            db.Sp_Add_Activity(userid, "Update installment Plan of Plots with id <a class='plt-data' data-id=' " + Plotid + "'>" + Plotid + "</a>", "Update", Modules.PlotManagement.ToString(), ActivityType.Plan_Updation.ToString(), Plotid);
            db.Sp_Add_PlotComments(Plotid, "Update Installment Plan with Total Price: " + Plot_Price, userid, ActivityType.Plan_Updation.ToString());
            db.SP_Update_PlotVerificationToNull(Plotid);

            return Json(true);
        }
        public void AddInstallmentPlanPlot(List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure, string DevCharStatus, long PlotId, DateTime Date)
        {
            List<Plot_Installments> plot_Installments = new List<Plot_Installments>();
            decimal rate = 0, total = 0, grandtotal = 0;
            foreach (var item in installmentstructure.Where(x => x.Installment_Type == "3" || x.Installment_Type == "6"))
            {
                if (item.Installment_Type == "3") // Development Charges
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    DateTime b = Date;
                    Plot_Installments fi = new Plot_Installments()
                    {
                        Status = InstallmentPaymentStatus.Pending.ToString(),
                        DueDate = b,
                        Amount = item.Amount,
                        Plot_Id = PlotId,
                        Installment_Name = item.Installment_Name,
                        Installment_Type = "3",
                    };
                    plot_Installments.Add(fi);
                }
                else if (item.Installment_Type == "6")
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    DateTime b = Date;
                    b = b.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                    DateTime dt = new DateTime(b.Year, b.Month, 1);
                    Plot_Installments fi = new Plot_Installments()
                    {
                        Status = InstallmentPaymentStatus.Pending.ToString(),
                        DueDate = dt,
                        Amount = item.Amount,
                        Plot_Id = PlotId,
                        Installment_Name = item.Installment_Name,
                        Installment_Type = "6",
                    };
                    plot_Installments.Add(fi);
                }
            }
            foreach (var item in installmentstructure.Where(x => x.Installment_Type == "4" || x.Installment_Type == "2" || x.Installment_Type == "0"))
            {

                if (item.Installment_Type == "4") // for Half yearly , Quarterly
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    var eachinst = item.Amount / item.After;
                    var itemaft = item.Interval;



                    for (int i = 1; i <= item.After; i++)
                    {
                        DateTime b = Date;
                        var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                        if (str != null) // to check for confirmation
                        {
                            b = str.DueDate;
                        }
                        b = b.AddMonths(Convert.ToInt16(itemaft));
                        DateTime dt = new DateTime(b.Year, b.Month, 1);

                        Plot_Installments fi = new Plot_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            DueDate = dt,
                            Amount = Convert.ToDecimal(eachinst),
                            Plot_Id = PlotId,
                            Installment_Name = i + " - " + item.Installment_Name,
                            Installment_Type = "1",
                        };
                        plot_Installments.Add(fi);
                        itemaft += item.Interval;
                    }
                }
                else if (item.Installment_Type == "0")
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;

                    DateTime b = Date;
                    var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                    if (str != null) // to check for confirmation
                    {
                        b = str.DueDate;
                    }

                    b = b.AddMonths(Convert.ToInt16((item.After == null) ? 0 : item.After));
                    DateTime dt = new DateTime(b.Year, b.Month, 1);
                    Plot_Installments fi = new Plot_Installments()
                    {
                        Status = InstallmentPaymentStatus.Pending.ToString(),
                        DueDate = dt,
                        Amount = item.Amount,
                        Plot_Id = PlotId,
                        Installment_Name = item.Installment_Name,
                        Installment_Type = "0",
                    };
                    plot_Installments.Add(fi);
                }
            }
            foreach (var item in installmentstructure.Where(x => x.Installment_Type == "1"))
            {
                if (item.Installment_Type == "1") // for type installments
                {
                    rate = item.Plot_Rate; total = item.Total; grandtotal = item.Grand_Total;
                    var eachinst = item.Amount / item.Rate;


                    for (int i = 1; i <= item.Rate; i++)
                    {
                        DateTime b = Date;
                        var str = plot_Installments.Where(x => x.Installment_Type == "6").FirstOrDefault();
                        if (str != null) // to check for confirmation
                        {
                            b = str.DueDate;
                        }
                        b = b.AddMonths(i);
                        var insRes = plot_Installments.Where(x => x.DueDate == new DateTime(b.Year, b.Month, 1)).FirstOrDefault();
                        while (insRes != null)
                        {
                            b = b.AddMonths(1);
                            insRes = plot_Installments.Where(x => x.DueDate == new DateTime(b.Year, b.Month, 1)).FirstOrDefault();
                        }
                        DateTime dt = new DateTime(b.Year, b.Month, 1);

                        Plot_Installments fi = new Plot_Installments()
                        {
                            Status = InstallmentPaymentStatus.Pending.ToString(),
                            DueDate = dt,
                            Amount = eachinst,
                            Plot_Id = PlotId,
                            Installment_Name = i + " Monthly Installment",
                            Installment_Type = "1",
                        };
                        plot_Installments.Add(fi);
                    }
                }
            }


            var installmentplan = new XElement("Plots", plot_Installments.Select(x => new XElement("Installments",
                                    new XAttribute("Amount", x.Amount),
                                    new XAttribute("DueDate", x.DueDate),
                                    new XAttribute("Installment_Name", x.Installment_Name),
                                    new XAttribute("Installment_Type", x.Installment_Type),
                                    new XAttribute("Status", x.Status)
                                    ))).ToString();

            db.Sp_Update_PlotInstallments(PlotId, 0, 0, 0, installmentplan);
        }

        public ActionResult ReceivePlotInstallments()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }
        public ActionResult PlotInstallment()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayPlotInstallmentWithReceipt(ReceiptData rd, long Plotid, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var plot = db.Sp_Get_PlotData(Plotid).FirstOrDefault();

            var balance = db.File_Plot_Balance.Where(x => x.File_Plot_Id == plot.Id && x.Module == Modules.PlotManagement.ToString()).FirstOrDefault();
            var rem = Math.Round(Convert.ToDouble(balance.Outstanding_Amount), 0) - Math.Round(Convert.ToDouble(rd.Amount), 0);
            if (rem < 0)
            {
                return Json(new { Status = false, Msg = "Amount is Greater than Outstanding Amount" });
            }

            var lastowner = db.Sp_Get_PlotLastOwner(Plotid).ToList();
            Helpers H = new Helpers();


            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {

                    var res2 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, string.Join(",", lastowner.Select(x => x.Mobile_1).Distinct())
                        , string.Join(",", lastowner.Select(x => x.Father_Husband).Distinct()), Plotid, string.Join(",", lastowner.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
                        rd.Project_Name, rd.Rate, null, rd.Plot_Size, ReceiptTypes.Installment.ToString(), userid, userid, "", null, Modules.PlotManagement.ToString(), "", plot.Plot_No, plot.Block_Name, plot.Type, lastowner.FirstOrDefault().GroupTag, TransactionId, "", receiptno, comp.Id).FirstOrDefault();

                    if (res2.Receipt_Id > 0)
                    {

                        var a = db.SP_Update_PlotVerificationToNull(Plotid);
                        string text = "";
                        {
                            bool headcashier = true;
                            //if (User.IsInRole("Head Cashier"))
                            //{
                            //    headcashier = true;
                            //}
                            try
                            {
                                AccountHandlerController de = new AccountHandlerController();
                                var df = de.Receive_Plot_Amount(rd.Amount, plot.Plot_No, plot.Type, plot.Block_Name, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res2.Receipt_No, 1, headcashier, AccountingModulePlots, plot.BlockIden);
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                var dg = db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                            }
                        }
                        if (rd.PaymentType == "Cash")
                        {
                            var res10 = db.Sp_Get_ReceivedAmounts(Plotid, Modules.PlotManagement.ToString()).Where(x => (x.Status == "Approved" || x.Status is null)).Sum(x => x.Amount);
                            var res11 = db.Sp_Get_PlotInstallments(Plotid).Sum(x => x.Amount);

                            if (res10 == res11)
                            {
                                Notifier.Notify(userid, NotifyTo.Full_Paid, NotifierMsg.Full_Paid_Plots, new object[] { plot }, NotifyType.FullPaid_Plot.ToString());
                            }
                     //       text = "Dear " + rd.Name + ",\n\r" +
                     //"A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in cash for Plot Number " + plot.Plot_No + " " + plot.Block_Name + " " + plot.Type + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";

                        }
                        else
                        {

                            var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                        Modules.PlotManagement.ToString(), Types.Installment.ToString(), userid, rd.PayChqNo, Plotid, rd.Ch_bk_Pay_Date, rd.FilePlotNumber.ToString(), res2.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());

                            if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                            }
                            string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                            var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                            //var Imges = H.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());
                            //text = "Dear " + rd.Name + ",\n\r" +
                            //                      "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for Plot Number " + plot.Plot_No + " " + plot.Block_Name + " " + plot.Type + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

                        }
                        Transaction.Commit();

                        //try
                        //{
                        //    SmsService smsService = new SmsService();
                        //    smsService.SendMsg(text, rd.Mobile_1);
                        //}
                        //catch (Exception)
                        //{
                        //}
                        var res = new { Status = true, Receiptid = res2.Receipt_No, Token = userid };
                        return Json(res);
                    }
                    else
                    {
                        var res = new { Status = false, Msg = "Transaction already done." };
                        return Json(res);
                    }

                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                    var res = new { Status = false, Msg = "Error Occured" };
                    return Json(res);
                }
            }
        }
        public ActionResult PayLeadPayment(long Id)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.TransactionId = new Helpers().RandomNumber();
            var res1 = db.Sp_Get_Lead(Id).SingleOrDefault();
            var res7 = db.SAM_Receipts.Where(x => x.Lead_Id == Id).ToList();
            var res = new LeadsData { Lead = res1 };
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult PayLeadPaymentWithReceipt(ReceiptData rd, long Id, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var name = db.Users.Find(userid);
            var l = db.Sp_Get_Lead(Id).SingleOrDefault();
            if (l.Offered_Price < rd.Amount)
            {
                var data1 = new { Status = false };
                return Json(data1);
            }
            string Comp = "SAM";

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res = db.Sp_Add_SAM_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, l.Mobile_1
                                              , l.Father_Husband, l.Id, l.Name, rd.PaymentType, 0,
                                              rd.Project_Name, 0, l.Plot_Size, ReceiptTypes.BookingToken.ToString(), TransactionId, userid, "Booking Token", null, "", "", "T-B-A", l.Block, Comp, TransactionId).FirstOrDefault();

                    bool headcashier = false;
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }
                    AccountHandlerController de = new AccountHandlerController();
                    de.SAM_Installment(rd.Amount, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res.Receipt_No, 1, l.Deal_Number, headcashier);

                    var res3 = db.Sp_Add_LeadFollowup(name.Name + " Make receipt of Amount " + string.Format("{0:n0}", rd.Amount), l.Id, userid, "Text").FirstOrDefault();
                    var text = "Dear " + l.Name + ",\n\r" +
                    "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";

                    try
                    {
                        SmsService smsService = new SmsService();
                        smsService.SendMsg(text, rd.Mobile_1);
                    }
                    catch (Exception ex)
                    {
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                    }

                    Transaction.Commit();
                    var data = new { Receiptid = res.Receipt_Id, Token = TransactionId, Status = true };
                    return Json(data);

                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    Transaction.Rollback();
                    var data = new { Msg= "Transaction cannot be process", Status = false };
                    return Json(data);

                }
            }
        }
        public ActionResult PaymentIssue(long Id)
        {
            var res = db.Sp_Get_Receipt_Parameter_ById(Id).SingleOrDefault();
            return PartialView(res);
        }
        public JsonResult PaymentIssue(long Id, long File_Plot_id, string Remarks, string Status)
        {
            //long userid = long.Parse(User.Identity.GetUserId());
            //db.Sp_Add_Activity(userid, "Update receipt of Plot Id: "+ File_Plot_id +" Receipt: "+ Id +" <a class='rece-data' data-id=' " + PO.Plot_Id + "'>" + PO.Plot_Id + "</a>, "Update", Modules.PlotManagement.ToString(), ActivityType.Record_Upatation.ToString(), File_Plot_id);
            //db.Sp_Add_PlotComments(PO.Plot_Id, "Update Name: " + res.Name + " To: " + PO.Name, userid, ActivityType.Record_Upatation.ToString()); 

            return Json(true);
        }
        public JsonResult RefundAmountPlot(long Id, Decimal Amount)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Receipts.Where(x => x.Id == Id).SingleOrDefault();
            if (res.Module == Modules.PlotManagement.ToString())
            {
                var res2 = db.Sp_Get_PlotLastOwner(res.File_Plot_No).FirstOrDefault();
                //Was passing 0 as total plot price, replaced it with plot ownership total amount
                var res4 = db.Sp_Add_Booking_Cancelation(Amount, res2.Mobile_1, res2.Father_Husband, res.File_Plot_No, res2.Name, 0, "Meher Estate Developers",
                    res.Size, Cancellations.Receipt_Refund.ToString(), userid, userid, "Refund Amount ", Modules.PlotManagement.ToString(),
                    res.File_Plot_Number, res.Block, 0, 0, "Amount Refund",res.Plot_Type).FirstOrDefault();
                RefundAmountsReq refa = new RefundAmountsReq()
                {
                    Amount = Amount,
                    Description = "Refund amount from Receipt: " + res.ReceiptNo,
                    Module = Modules.PlotManagement.ToString(),
                    Module_Id = Convert.ToInt64(res.File_Plot_No),
                    Finance_Approval = "Pending",
                    Manager_Approval = "Pending",
                    Receipt_Amount = res.Amount,
                    Receipt_Id = Id,
                    Block = res.Block,
                    Date = DateTime.UtcNow,
                    Name = res2.Name,
                    Plot_No = res.File_Plot_Number,
                    Requested_By = userid
                };
                db.RefundAmountsReqs.Add(refa);
                db.SaveChanges();
                var res5 = new { Status = "Requested", ReceiptNo = res4.Receipt_No, ReceiptId = res4.Receipt_Id, Token = userid };
                return Json(res5);
            }
            else if (res.Module == Modules.FileManagement.ToString())
            {
                var fileId = db.File_Form.Where(x => x.Id == res.File_Plot_No).Select(x => x.Id).FirstOrDefault();
                var res2 = db.Sp_Get_FileLastOwner(fileId).FirstOrDefault();
                var res4 = db.Sp_Add_Booking_Cancelation(Amount, res2.Mobile_1, res2.Father_Husband, res.File_Plot_No, res2.Name, 0, "Meher Estate Developers",
                    res.Size, Cancellations.Receipt_Refund.ToString(), userid, userid, "Refund Amount ", Modules.FileManagement.ToString(),
                    res.File_Plot_Number, res.Block, 0, 0, "Amount Refund",res.Plot_Type).FirstOrDefault();
                RefundAmountsReq refa = new RefundAmountsReq()
                {
                    Amount = Amount,
                    Description = "Refund amount from Receipt: " + res.ReceiptNo,
                    Module = Modules.FileManagement.ToString(),
                    Module_Id = Convert.ToInt64(res.File_Plot_No), //Full File Number
                    Finance_Approval = "Pending",
                    Manager_Approval = "Pending",
                    Receipt_Amount = res.Amount,
                    Receipt_Id = Id,
                    Block = res.Block,
                    Date = DateTime.UtcNow,
                    Name = res2.Name,
                    Plot_No = res.File_Plot_Number,
                    Requested_By = userid
                };
                db.RefundAmountsReqs.Add(refa);
                db.SaveChanges();
                var res5 = new { Status = "Requested", ReceiptNo = res4.Receipt_No, ReceiptId = res4.Receipt_Id, Token = userid };
                return Json(res5);
            }
            else if (res.Module == ProjectCategory.Building.ToString())
            {
                var res2 = db.Sp_Get_CommercialLastOwner(res.File_Plot_No).FirstOrDefault();
                var res1 = db.Sp_Get_CommercialData(res.File_Plot_No).FirstOrDefault();
                var res4 = db.Sp_Add_Booking_Cancelation(Amount, res2.Mobile_1, res2.Father_Husband, res.File_Plot_No, res2.Name, 0, res1.Project_Name,
         res.Size, Cancellations.Receipt_Refund.ToString(), userid, userid, "Refund Amount ", ProjectCategory.Building.ToString(),
         res.File_Plot_Number, res.Block, 0, 0, "Amount Refund",res.Plot_Type).FirstOrDefault();
                RefundAmountsReq refa = new RefundAmountsReq()
                {
                    Amount = Amount,
                    Description = "Refund amount from Receipt: " + res.ReceiptNo,
                    Module = ProjectCategory.Building.ToString(),
                    Module_Id = Convert.ToInt64(res.File_Plot_No),
                    Finance_Approval = "Pending",
                    Manager_Approval = "Pending",
                    Receipt_Amount = res.Amount,
                    Receipt_Id = Id,
                    Block = res.Block,
                    Date = DateTime.UtcNow,
                    Name = res2.Name,
                    Plot_No = res.File_Plot_Number,
                    Requested_By = userid
                };
                db.RefundAmountsReqs.Add(refa);
                db.SaveChanges();
                var res5 = new { Status = "Requested", ReceiptNo = res4.Receipt_No, ReceiptId = res4.Receipt_Id, Token = userid };
                return Json(res5);
            }
            else
            {
                var res5 = new Return { Status = false, Msg = "Please Try Later" };
                return Json(res5);
            }
        }
        public ActionResult RefundPlotsManager()
        {

            return View();
        }
        public ActionResult OtherRecovery()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }
        public ActionResult OtherRecoveryOpt(ReceiptData rd, string Module, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

            if (rd.PaymentType == "Cash")
            {

                var res2 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1, rd.Father_Husband, null, rd.Name, rd.PaymentType, rd.TotalAmount,
                    rd.Project_Name, rd.Rate, null, rd.Plot_Size, ReceiptTypes.Other_Recovery.ToString(), userid, userid, "", null, Module, rd.DevCharges, rd.FilePlotNumber, rd.Block, rd.Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                Helpers h = new Helpers();
                bool headcashier = false;
                if (User.IsInRole("Head Cashier"))
                {
                    headcashier = true;
                }
                try
                {
                    AccountHandlerController de = new AccountHandlerController();
                    de.Other_Recovery(rd.Amount, rd.FilePlotNumber, rd.Type, rd.Block_Name, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, rd.DevCharges, h.RandomNumber(), userid, res2.Receipt_No, 1, Module, headcashier, AccountingModulePlots);
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
                var res = new { Receiptid = res2.Receipt_Id, Token = userid };
                return Json(res);
            }
            else
            {
                var res1 = db.Sp_Add_Amount_Clearance(rd.Amount, "", Modules.PlotManagement.ToString(), Types.Installment.ToString()).FirstOrDefault();

                var res2 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
                    , rd.Father_Husband, 0, rd.Name, rd.PaymentType, rd.TotalAmount,
                    rd.Project_Name, rd.Rate, null, rd.Plot_Size, ReceiptTypes.Other_Recovery.ToString(), res1, userid, "", null, Module, rd.DevCharges, rd.FilePlotNumber, rd.Block, rd.Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();

                Helpers h = new Helpers();
                //bool headcashier = false;
                //if (User.IsInRole("Head Cashier"))
                //{
                //    headcashier = true;
                //}
                //try
                //{
                //    AccountHandlerController de = new AccountHandlerController();
                //    de.Other_Recovery(rd.Amount, rd.FilePlotNumber, rd.Type, rd.Block_Name, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, rd.DevCharges, h.RandomNumber(), userid, res2.Receipt_No, 1, Module, headcashier);
                //}
                //catch (Exception ex)
                //{
                //    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                //}

                var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                            Modules.PlotManagement.ToString(), Types.Installment.ToString(), res1, rd.PayChqNo, 0, rd.Ch_bk_Pay_Date, "", res2.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                }
                string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                var Imges = h.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());
                var res = new { Receiptid = res2.Receipt_Id, Token = res1 };
                return Json(res);
            }
        }
        public JsonResult UpdateReceiptNo(long Id, string Rec)
        {
            db.updreceiptno(Rec, Id);
            return Json(true);
        }

        public ActionResult InstallmentPortal()
        {
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }

        public ActionResult FetchInstallmentData(string plotno, long Block, string plttype)
        {
            if (string.IsNullOrEmpty(plotno) || string.IsNullOrWhiteSpace(plotno))
            {
                //user request was empty so we cannot let them proceed with the request
                return PartialView(null);
            }
            string[] plotnoSeg = plotno.Split(' ');
            var res = db.Sp_Get_InstallmentInfo(plotno, Block, plttype, plotnoSeg[0], (plotnoSeg.Length > 1) ? plotnoSeg[1] : string.Empty).FirstOrDefault();
            if (res is null)
            {
                //nothing found against requested plot or file so we cannot let them proceed
                return PartialView(null);
            }

            //something was found against the request so we have to send appropriate partial view

            if (res.IsPlot == true)
            {
                // this is a plot so we have to send plot partial view
                return RedirectToAction("LastestPlotDetails", "Plots", new { Plotid = res.Id });
            }
            else if (res.IsPlot == false)
            {
                //this is a file so we have to send file partial view
                if (Block == 24)
                {
                    var val =  plotnoSeg[0];
                    return RedirectToAction("FetchInstallmentData", "FileSystem", new { Filefromid = val });

                }
                else 
                {
                    long val = Convert.ToInt64(plotnoSeg[0]);
                    return RedirectToAction("FetchInstallmentData", "FileSystem", new { Filefromid = val });
                }


                //bool Result = long.TryParse(plotnoSeg[0], out long myDec);
                //if (Result == true)
                //{

                //    return RedirectToAction("FetchInstallmentData", "FileSystem", new { Filefromid = plotnoSeg[0] });
                //}
                //else
                //{
                //    return RedirectToAction("FetchInstallmentData", "FileSystem", new { Filefromid = plotnoSeg[0].ToString() });

                //}

            }
            else
            {
                //ambiguity in determining the proper type so we cannot let the request proceed
                return PartialView(null);
            }
        }

        public ActionResult CommercialInstallment(long? ShopId, long? OwnId)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.Shop = ShopId;
            ViewBag.Owner = OwnId;
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayComInstallmentWithReceipt(ReceiptData rd, long Comid, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var plot = db.Sp_Get_CommercialData(Comid).FirstOrDefault();
            var balance = db.File_Plot_Balance.Where(x => x.File_Plot_Id == plot.Id && x.Module == Modules.CommercialManagement.ToString()).FirstOrDefault();
            var rem = Math.Round(Convert.ToDouble(balance.Outstanding_Amount), 0) - Math.Round(Convert.ToDouble(rd.Amount), 0);
            if (rem < 0)
            {
                return Json(new { Status = false, Msg = "Amount is Greater than Outstanding Amount" });
            }
            var lastowner = db.Sp_Get_CommercialLastOwner(Comid).FirstOrDefault();
            Helpers H = new Helpers();
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
			bool headcashier = false;
            if (User.IsInRole("Head Cashier"))
            {
                headcashier = true;
            }
            
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var res2 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, lastowner.Mobile_1
                        , lastowner.Father_Husband, Comid, lastowner.Name, rd.PaymentType, rd.TotalAmount,
                        plot.Project_Name, rd.Rate, null,plot.Total_SqFt_Marlas.ToString() + " sq.ft", ReceiptTypes.Installment.ToString(), userid, userid, "", null, Modules.CommercialManagement.ToString()
                        , "", plot.shop_no + " " + plot.ApplicationNo + " - " + plot.Type, plot.Floor, plot.Type, lastowner.GroupTag, TransactionId, "", receiptno, comp.Id).FirstOrDefault();

                    if (res2.Receipt_No == "-1")
                    {
                        Transaction.Rollback();
                        var resdata = new { Status = false, Msg = "Error Occured" };
                        return Json(resdata);
                    }

                    db.SP_Update_VerificationToNull(Comid, ProjectCategory.Building.ToString());

                    {
                        // No plot information for commercial buildings
                        AccountHandlerController de = new AccountHandlerController();
                        de.Receive_Plot_Amount(rd.Amount, plot.shop_no, plot.Type, plot.Floor, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, TransactionId, userid, res2.Receipt_No, 1, headcashier, AccountingModuleCommercial, Convert.ToInt64(plot.ProjectId));
                    }
                    if (rd.PaymentType == "Cash")
                    {
                        var res10 = db.Sp_Get_ReceivedAmounts(Comid, plot.Module).Where(x => (x.Status == "Approved" || x.Status is null)).Sum(x => x.Amount);
                        //var res10 = db.Sp_Get_ReceivedAmounts(Comid, ProjectCategory.Building.ToString()).Where(x => (x.Status == "Approved" || x.Status is null)).Sum(x => x.Amount); //For Buildings
                        var res11 = db.Sp_Get_CommercialInstallments(Comid).Sum(x => x.Amount);

                        if (res10 == res11)
                        {
                            Notifier.Notify(userid, NotifyTo.Full_Paid, NotifierMsg.Full_Paid_Plots, new object[] { plot }, NotifyType.FullPaid_Plot.ToString());
                        }
                    }
                    else
                    {

                        var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                    plot.Module, Types.Installment.ToString(), userid, rd.PayChqNo, Comid, rd.Ch_bk_Pay_Date, plot.ApplicationNo, res2.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());

                        if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                        }
                        string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                        var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                        var Imges = H.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());

                    }
                    Transaction.Commit();
                    var res = new { Status = true, Receiptid = res2.Receipt_No, Token = userid };
                    return Json(res);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    var res = new { Status = false, Msg = "Error Occured" };
                    return Json(res);
                }
            }
        }

        public ActionResult PayReinstateFineCharges(long Req_Id)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();

            var req = db.MIS_Requests.Where(x => x.Id == Req_Id).FirstOrDefault();
            var mod = JsonConvert.DeserializeObject<FileReinstateRequest>(req.Details);

            if (mod.Module == Modules.FileManagement.ToString())
            {
                var File = db.File_Form.Where(x => x.Id == req.ModuleId).FirstOrDefault();

                ViewBag.Module = mod.Module;
                ViewBag.PlotSize = File.Plot_Size;
                ViewBag.File_Id = req.ModuleId;
                ViewBag.Req_Id = Req_Id;

            }
            else if (mod.Module == Modules.PlotManagement.ToString())
            {
                var Plot = db.Plots.Where(x => x.Id == req.ModuleId).FirstOrDefault();

                ViewBag.Module = mod.Module;
                ViewBag.PlotSize = Plot.Plot_Size;
                ViewBag.File_Id = req.ModuleId;
                ViewBag.Req_Id = Req_Id;
            }
            return PartialView();
        }
        [HttpPost]
        public JsonResult ReinstateFineCharges(long File_Id, long Req_Id, ReceiptData rd, long TransactionId, string Module)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            if (Modules.FileManagement.ToString() == Module)
            {

                var re = db.Sp_Update_FileVerificationToNull(File_Id);

                var appdetail = db.File_Form.Where(x => x.Id == File_Id).FirstOrDefault();
                var lastowner = db.Sp_Get_FileLastOwner(appdetail.Id).ToList();
                var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var uid = User.Identity.GetUserId<long>();
                        var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                        var req = db.MIS_Requests.Where(x => x.Id == Req_Id).FirstOrDefault();
                        FileReinstateRequest mdl = new FileReinstateRequest();
                        mdl = JsonConvert.DeserializeObject<FileReinstateRequest>(req.Details);
                        req.Status = "Approved";
                        mdl.StatusChangedAt = DateTime.Now;
                        mdl.User_Name = uname;
                        mdl.User_Id = uid;
                        req.Details = JsonConvert.SerializeObject(mdl);
                        db.MIS_Requests.Attach(req);
                        db.Entry(req).Property(x => x.Status).IsModified = true;
                        db.Entry(req).Property(x => x.Details).IsModified = true;
                        var retItem = db.SaveChanges();

                        var a = db.Sp_Add_Activity(req.CreatedBy_Id, "Updated Status to Reinstate Cancelled <a class='fl-data' data-id=' " + File_Id + "'>" + File_Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), File_Id);
                        var b = db.Sp_Add_FileComments(File_Id, "Reinstate the Plot with 10,000/Marla Charges. \n\r", req.CreatedBy_Id, ActivityType.Plot_Status_Updation.ToString());


                        AccountHandlerController de = new AccountHandlerController();
                        Helpers H = new Helpers();

                        var res2 = db.Sp_Add_Receipt(rd.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(rd.Amount)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, string.Join(" , ", lastowner.Select(x => x.Mobile_1))
                      , string.Join(" , ", lastowner.Select(x => x.Father_Husband)), File_Id, string.Join(" , ", lastowner.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
                        rd.Project_Name, rd.Rate, null, appdetail.Plot_Size, ReceiptTypes.Fines_And_Penalties.ToString(), userid, userid, "", null, Modules.FileManagement.ToString(), "Plot Reinstate Charges", appdetail.FileFormNumber.ToString(), appdetail.Block, appdetail.Type, lastowner.Select(x => x.Group_Tag).FirstOrDefault(), TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                        {
                            bool headcashier = false;
                            if (User.IsInRole("Head Cashier"))
                            {
                                headcashier = true;
                            }
                            de.Other_Recovery(rd.Amount, appdetail.FileFormNumber.ToString(), appdetail.Type, appdetail.Block, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, "Reinstate Fine", TransactionId, userid, res2.Receipt_No, 1, COA_Mapper_ModuleTypes.Reinstate_Charges.ToString(), headcashier, AccountingModulePlots);
                        }

                        db.Sp_Update_FileOwnerStatus(File_Id, lastowner.Select(x => x.Group_Tag).FirstOrDefault(), 1, "Owner");


                        string text = "";
                        if (rd.PaymentType == "Cash")
                        {
                            text = "Dear " + rd.Name + ",\n\r" +
                         "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in cash for File number " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                        }
                        else
                        {
                            var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                        Modules.FileManagement.ToString(), Types.Installment.ToString(), userid, rd.PayChqNo, File_Id, rd.Ch_bk_Pay_Date, rd.FilePlotNumber, res2.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                            if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                            }
                            string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                            var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                            var Images = H.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());

                            text = "Dear " + rd.Name + ",\n\r" +
                                                    "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for File number " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

                        }
                        try
                        {
                            foreach (var v in lastowner)
                            {
                                SmsService smsService = new SmsService();
                                smsService.SendMsg(text, v.Mobile_1);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        Transaction.Commit();
                        var res = new { Status = true, Receiptid = res2.Receipt_No, Token = userid };
                        return Json(res);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        var res = new { Status = false, Msg = "Installment Cannot Be Received. Contact SA Systems" };
                        return Json(res);
                    }
                }
            }
            else if (Modules.PlotManagement.ToString() == Module)
            {

                var re = db.SP_Update_PlotVerificationToNull(File_Id);

                var appdetail = db.Sp_Get_PlotData(File_Id).FirstOrDefault();
                var lastowner = db.Sp_Get_PlotLastOwner(appdetail.Id).ToList();
                var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var uid = User.Identity.GetUserId<long>();
                        var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                        var req = db.MIS_Requests.Where(x => x.Id == Req_Id).FirstOrDefault();
                        FileReinstateRequest mdl = new FileReinstateRequest();
                        mdl = JsonConvert.DeserializeObject<FileReinstateRequest>(req.Details);
                        req.Status = "Approved";
                        mdl.StatusChangedAt = DateTime.Now;
                        mdl.User_Name = uname;
                        mdl.User_Id = uid;
                        req.Details = JsonConvert.SerializeObject(mdl);
                        db.MIS_Requests.Attach(req);
                        db.Entry(req).Property(x => x.Status).IsModified = true;
                        db.Entry(req).Property(x => x.Details).IsModified = true;
                        var retItem = db.SaveChanges();

                        var a = db.Sp_Add_Activity(req.CreatedBy_Id, "Updated Status to Reinstate Cancelled <a class='fl-data' data-id=' " + File_Id + "'>" + File_Id + "</a>", "Create", Modules.FileManagement.ToString(), ActivityType.Plot_Status_Updation.ToString(), File_Id);
                        var b = db.Sp_Add_PlotComments(File_Id, "Reinstate the Plot with 10,000/Marla Charges. \n\r", req.CreatedBy_Id, ActivityType.Plot_Status_Updation.ToString());

                        AccountHandlerController de = new AccountHandlerController();
                        Helpers H = new Helpers();

                        var res2 = db.Sp_Add_Receipt(rd.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(rd.Amount)), rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, string.Join(" , ", lastowner.Select(x => x.Mobile_1))
                      , string.Join(" , ", lastowner.Select(x => x.Father_Husband)), File_Id, string.Join(" , ", lastowner.Select(x => x.Name)), rd.PaymentType, rd.TotalAmount,
                        rd.Project_Name, rd.Rate, null, appdetail.Plot_Size, ReceiptTypes.Fines_And_Penalties.ToString(), userid, userid, "", null, Modules.FileManagement.ToString(), "Plot Reinstate Charges", appdetail.Plot_No.ToString(), appdetail.Block_Name, appdetail.Type, lastowner.Select(x => x.GroupTag).FirstOrDefault(), TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                        {
                            bool headcashier = false;
                            if (User.IsInRole("Head Cashier"))
                            {
                                headcashier = true;
                            }
                            de.Other_Recovery(rd.Amount, appdetail.Plot_No.ToString(), appdetail.Type, appdetail.Block_Name, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, "Reinstate Fine", TransactionId, userid, res2.Receipt_No, 1, COA_Mapper_ModuleTypes.Reinstate_Charges.ToString(), headcashier, AccountingModulePlots);
                        }

                        db.Sp_Update_PlotOwnerStatus(File_Id, lastowner.Select(x => x.GroupTag).FirstOrDefault(), PlotsStatus.Registered.ToString(), "Owner");


                        string text = "";
                        if (rd.PaymentType == "Cash")
                        {
                            text = "Dear " + rd.Name + ",\n\r" +
                         "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received in cash for File number " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + ". Thank you for your payment.";
                        }
                        else
                        {
                            var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                                        Modules.FileManagement.ToString(), Types.Installment.ToString(), userid, rd.PayChqNo, File_Id, rd.Ch_bk_Pay_Date, rd.FilePlotNumber, res2.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                            if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                            }
                            string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                            var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                            var Images = H.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());

                            text = "Dear " + rd.Name + ",\n\r" +
                                                    "A Payment of Rs " + string.Format("{0:n0}", rd.Amount) + " has been received against " + rd.PaymentType + " No: " + rd.PayChqNo + " Bank: " + rd.Bank + " for File number " + rd.FilePlotNumber + " on " + string.Format("{0:dd MMM yyyy}", DateTime.Now) + " and has sent for clearing";

                        }
                        try
                        {
                            foreach (var v in lastowner)
                            {
                                SmsService smsService = new SmsService();
                                smsService.SendMsg(text, v.Mobile_1);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        Transaction.Commit();
                        var res = new { Status = true, Receiptid = res2.Receipt_No, Token = userid };
                        return Json(res);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        var res = new { Status = false, Msg = "Installment Cannot Be Received. Contact SA Systems" };
                        return Json(res);
                    }
                }
            }
            return Json(true);
        }
        public JsonResult ShiftRightInstallments(long Id, string Module)
        {
            var res = db.Sp_Update_ShiftInstallmentPlan(1, Id, Module);
            return Json(new Return { Msg = "Updated", Status = true });
        }
        public JsonResult ShiftLeftInstallments(long Id, string Module)
        {
            var res = db.Sp_Update_ShiftInstallmentPlan(-1, Id, Module);
            return Json(new Return { Msg = "Updated", Status = true });
        }
        public ActionResult UpdateInstallmentPlans(string Plot_Size)
        {
            var Size = Plot_Size.Split(' ')[0];
            var res = (from x in db.Installment_Structure
                       join y in db.Installment_Plot_Category on x.Installment_Plot_Id equals y.Id
                       where y.Plot_Size == Size
                       select new Installment_Struct
                       {
                           After = x.After,
                           Amount = x.Amount,
                           Datetime = x.Datetime,
                           Id = x.Id,
                           Installment_Name = x.Installment_Name,
                           Installment_Plot_Id = x.Installment_Plot_Id,
                           Installment_Type = x.Installment_Type,
                           Module = x.Module,
                           Rate = x.Rate,
                           Plot_Size = y.Plot_Size + " Marla"
                       }).ToList();
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult UpdateInstallmentPlan(int Id, long File_Id, string Module)
        {
            List<Sp_Get_InstallmentStructure_Current_Result> installmentstructure = db.Sp_Get_InstallmentStructure(Id).Select(x => new Sp_Get_InstallmentStructure_Current_Result
            {
                After = x.After,
                Installment_Type = x.Installment_Type,
                Amount = x.Amount,
                Datetime = x.Datetime,
                Grand_Total = x.Grand_Total,
                Id = x.Id,
                Installment_Name = x.Installment_Name,
                Installment_Plot_Id = x.Installment_Plot_Id,
                Module = x.Module,
                Plot_Rate = x.Plot_Rate,
                Rate = x.Rate,
                Total = x.Total,
                Interval = x.Interval

            }).ToList();
            if (Module == Modules.PlotManagement.ToString())
            {
                var inst_st_date = db.Plot_Installments.Where(x => x.Plot_Id == File_Id && x.Installment_Type == "3").FirstOrDefault().DueDate;
                 this.AddInstallmentPlanPlot(installmentstructure, "", File_Id, inst_st_date);
            }
            else if (Module == Modules.FileManagement.ToString())
            {
                var inst_st_date = db.File_Installments.Where(x => x.File_Id == File_Id && x.Installment_Type == "3").FirstOrDefault().Due_Date;
                var file_Installments = this.AddInstallmentPlan(installmentstructure, "", File_Id, inst_st_date);
            }
            return Json(true);
        }
		public ActionResult UpdateSingleInstallment(long Id, string Module)
        {
            ViewBag.Module = Module;
            if (Module == Modules.FileManagement.ToString())
            {
                var res = db.File_Installments.Where(x => x.Id == Id).Select(x=> new Installment {
                    Amount = x.Amount,
                    Due_Date = x.Due_Date,
                    File_Plot_Id = x.File_Id,
                    Id = x.Id,
                    Installment_Name = x.Installment_Name,
                    Installment_Type = x.Installment_Type,
                    Status = x.Status
                }).FirstOrDefault();
                return PartialView(res);
            }
            else if (Module == Modules.PlotManagement.ToString())
            {
                var res = db.Plot_Installments.Where(x => x.Id == Id).Select(x => new Installment
                {
                    Amount = x.Amount,
                    Due_Date = x.DueDate,
                    File_Plot_Id = x.Plot_Id,
                    Id = x.Id,
                    Installment_Name = x.Installment_Name,
                    Installment_Type = x.Installment_Type,
                    Status = x.Status
                }).FirstOrDefault();
                return PartialView(res);
            }
            else if (Module == "Building")
            {
                var res = db.Commercial_Installments.Where(x => x.Id == Id).Select(x => new Installment
                {
                    Amount = x.Amount,
                    Due_Date = x.Due_Date,
                    File_Plot_Id = x.Com_Id,
                    Id = x.Id,
                    Installment_Name = x.Installment_Name,
                    //Installment_Type = x.Installment_Type,
                    Status = x.Status
                }).FirstOrDefault();
                return PartialView(res);
            }
            else
            {
                return PartialView();
            }

        }

        public JsonResult InstallmentUpdate(InstallmentPlanUpdation plan, string Module)
        {
            long userid = long.Parse(User.Identity.GetUserId());

            ViewBag.Module = Module;
            if (Module == Modules.FileManagement.ToString())
            {
                var res = db.File_Installments.Where(x => x.Id == plan.Id).FirstOrDefault();
                db.Sp_Update_Installment_Parameter(plan.InstallmentAmountPerMonth, plan.InsatallmentStartingDate, plan.Id,Module, plan.Installment_Type);
                var Filedata = db.File_Form.Where(x => x.Id == res.File_Id).SingleOrDefault();
                db.Sp_Add_FileComments(Filedata.Id, "Installment updated from Date: " + string.Format("{0:dd-MMM-yyyy}", res.Due_Date) + " to " + string.Format("{0:dd-MMM-yyyy}", plan.InsatallmentStartingDate) + " & Amount from " + string.Format("{0:n2}", res.Amount) + " to " + string.Format("{0:n2}", plan.InstallmentAmountPerMonth) + " of " + Filedata.FileFormNumber, userid, ActivityType.Plan_Updation.ToString());
                db.Sp_Add_Activity(userid, "Updated the Installment of File Number: " + Filedata.FileFormNumber , ActivityType.Plan_Updation.ToString(),Module, ActivityType.Plan_Updation.ToString(), Filedata.Id);
                if (Filedata.Verified == true)
                {
                    db.Sp_Update_FileVerificationToNull(Filedata.Id);
                    // Pending Notification
                }
                return Json(true);
            }
            else if (Module == Modules.PlotManagement.ToString())
            {
                var res = db.Plot_Installments.Where(x => x.Id == plan.Id).FirstOrDefault();
                db.Sp_Update_Installment_Parameter(plan.InstallmentAmountPerMonth, plan.InsatallmentStartingDate, plan.Id, Module, plan.Installment_Type);
                var Plotdata = db.Sp_Get_PlotData(res.Plot_Id).SingleOrDefault();
                db.Sp_Add_PlotComments(Plotdata.Id, "Installment updated from Date: " + string.Format("{0:dd-MMM-yyyy}", res.DueDate) + " to " + string.Format("{0:dd-MMM-yyyy}", plan.InsatallmentStartingDate) + " & Amount from " + string.Format("{0:n2}", res.Amount) + " to " + string.Format("{0:n2}", plan.InstallmentAmountPerMonth) + " of " + Plotdata.Plot_No, userid, ActivityType.Plan_Updation.ToString());
                db.Sp_Add_Activity(userid, "Updated the Installment of Plot Number: " + Plotdata.Plot_No, ActivityType.Plan_Updation.ToString(), Module, ActivityType.Plan_Updation.ToString(), Plotdata.Id);
                if (Plotdata.Verified == true)
                {
                    db.SP_Update_VerificationToNull(Plotdata.Id,Module);
                    // Pending Notification
                }
                return Json(true);
            }
            else if (Module == "Building")
            {
                return Json(true);
            }
            else
            {
                return Json(true);
            }
        }
        public ActionResult SubsidiaryRecovery()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            ViewBag.Module = new SelectList(db.Sp_Get_Subsidiary_Companies().ToList(), "Head", "Head");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView();
        }
        public JsonResult SubsidiaryRecoveryReceive(ReceiptData rd, string Module, long TransactionId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);

            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

            if (rd.PaymentType == "Cash")
            {

                var res2 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1, rd.Father_Husband, null, rd.Name, rd.PaymentType, rd.TotalAmount,
                    Module, rd.Rate, null, rd.Plot_Size, ReceiptTypes.Subsidiary_Recovery.ToString(), TransactionId, userid, "", null, Module, rd.DevCharges, Module, rd.Block, rd.Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();
                Helpers h = new Helpers();
                bool headcashier = false;
                if (User.IsInRole("Head Cashier"))
                {
                    headcashier = true;
                }
                try
                {
                    AccountHandlerController de = new AccountHandlerController();
                    de.SubsidiaryPaymentReceive(rd.Amount, rd.FilePlotNumber, rd.Type, rd.Block_Name, rd.PaymentType, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Bank, rd.DevCharges, TransactionId, userid, res2.Receipt_No, 1, Module, headcashier);
                }
                catch (Exception ex)
                {
                    db.Sp_Add_ErrorLog(ex.Message + ex.InnerException.ToString() + ex.StackTrace, "", "", this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
                var res = new { Receiptid = res2.Receipt_Id, Token = TransactionId };
                return Json(res);
            }
            else
            {
                var res2 = db.Sp_Add_Receipt(rd.Amount, rd.AmountInWords, rd.Bank, rd.PayChqNo, rd.Ch_bk_Pay_Date, rd.Branch, rd.Mobile_1
                    , rd.Father_Husband, 0, rd.Name, rd.PaymentType, rd.TotalAmount,
                    rd.Project_Name, rd.Rate, null, rd.Plot_Size, ReceiptTypes.Subsidiary_Recovery.ToString(), userid, userid, "", null, Module, rd.DevCharges, rd.FilePlotNumber, rd.Block, rd.Type, 0, TransactionId, "", receiptno, comp.Id).FirstOrDefault();

                Helpers h = new Helpers();

                var res3 = Convert.ToInt64(db.Sp_Add_Cheque_BankDraft_PayOrder(rd.Amount, rd.Bank, rd.Branch, rd.PaymentType, null, null, PaymentMethodStatuses.Pending.ToString(),
                            Module, ReceiptTypes.Subsidiary_Recovery.ToString(), userid, rd.PayChqNo, 0, rd.Ch_bk_Pay_Date, "", res2.Receipt_Id, comp.Id, Voucher_Type.BRV.ToString()).FirstOrDefault());
                if (!Directory.Exists(Server.MapPath("~/Repository/Cheques/" + "/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/Cheques/"));
                }
                string dt = string.Format("{0:d/M/yyyy}", rd.Ch_bk_Pay_Date);
                var pathMain = Path.Combine(Server.MapPath("~/Repository/Cheques/" + "/"), rd.PayChqNo + "_" + rd.Bank + "_" + dt.Replace("/", "_") + ".jpg");
                var Imges = h.SaveBase64Image(rd.FileImage, pathMain, res3.ToString());
                var res = new { Receiptid = res2.Receipt_Id, Token = userid };
                return Json(res);
            }
        }

        public ActionResult AddInstallmentRow()
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView();
        }

        public JsonResult AddInstallmentPlann(Decimal Amount, DateTime Date, string Type, long id, string Name, string Module)
        {
            if (Module == Modules.PlotManagement.ToString())
            {
                Plot_Installments a = new Plot_Installments();
                a.Amount = Amount;
                a.DueDate = Date;
                a.Plot_Id = id;
                a.Installment_Name = Name;
                a.Installment_Type = Type;
                a.Status = "Pending";
                db.Plot_Installments.Add(a);
                db.SaveChanges();
            }

            else if (Module == Modules.FileManagement.ToString())
            {
                File_Installments a = new File_Installments();
                a.Amount = Amount;
                a.Due_Date = Date;
                a.File_Id = id;
                a.Installment_Name = Name;
                a.Installment_Type = Type;
                a.Status = "Pending";
                db.File_Installments.Add(a);
                db.SaveChanges();
            }

            return Json(true);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}