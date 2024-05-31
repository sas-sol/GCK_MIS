using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MeherEstateDevelopers.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using MeherEstateDevelopers.Filters;
using System.Web.Http.Results;
using System.Security.Claims;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class CompanyController : Controller
    {
        // GET: Company
        Grand_CityEntities db = new Grand_CityEntities();

        /// <summary>
        /// Company Configuration for Core and Chart of Accounts.
        /// </summary>
        /// <returns></returns>

        [NoDirectAccess] public ActionResult CreateCompany()
        {
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult CreateCompany(Company comp)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    var res = db.Companies.Any(x => x.Company_Name == comp.Company_Name);
        //    if (!res)
        //    {

        //        //using (var Transaction = db.Database.BeginTransaction())
        //        //{
        //        //    try
        //        //    {
        //                db.Companies.Add(comp);
        //                db.SaveChanges();
        //                db.Sp_Add_Activity(userid, "Added New Company " + comp.Company_Name, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), null);
        //                // Add Chart of Account.

        //                 var chartid = db.Sp_Add_CA_Head(1, comp.Company_Name, null, null).FirstOrDefault();
        //                //
        //                var coa = Nomenclature.COA();
                        
        //                foreach (var item in coa)
        //                {
        //                    var allchart = db.Sp_Get_CA_Head_FullCode(null, chartid).ToList();
        //                    var newacc = allchart.FirstOrDefault().Text_ChartCode.Replace("/","") + "-" + item.Value;
        //                    var c = Array.ConvertAll(newacc.Split('-'), int.Parse);
        //                    string nc = "";
        //                    if (c[0] > 0 && c[1] == 0 && c[2] == 0 && c[3] == 0 && c[4] == 0 && c[5] == 0)
        //                    {
        //                        nc = "/" + c[0] + "/";
        //                        if (!allchart.Any(x => x.Text_ChartCode == nc))
        //                        {
        //                            var d = db.Sp_Add_CA_Head(chartid, item.Name, null, null);
        //                        }
        //                    }
        //                    else if (c[0] > 0 && c[1] > 0 && c[2] == 0 && c[3] == 0 && c[4] == 0 && c[5] == 0)
        //                    {
        //                        nc = "/" + c[0] + "/" + c[1] + "/";
        //                        if (!allchart.Any(x => x.Text_ChartCode == nc))
        //                        {
        //                            string Parentcode = "/" + c[0] + "/";
        //                            var parent = allchart.Where(x => x.Text_ChartCode == Parentcode).SingleOrDefault();
        //                            var d = db.Sp_Add_CA_Head(parent.Id, item.Name, null, null);
        //                        }
        //                    }
        //                    else if (c[0] > 0 && c[1] > 0 && c[2] > 0 && c[3] == 0 && c[4] == 0 && c[5] == 0)
        //                    {
        //                        nc = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/";
        //                        if (!allchart.Any(x => x.Text_ChartCode == nc))
        //                        {
        //                            string Parentcode = "/" + c[0] + "/" + c[1] + "/";
        //                            var parent = allchart.Where(x => x.Text_ChartCode == Parentcode).SingleOrDefault();
        //                            var d = db.Sp_Add_CA_Head(parent.Id, item.Name, null, null);
        //                        }
        //                    }
        //                    else if (c[0] > 0 && c[1] > 0 && c[2] > 0 && c[3] > 0 && c[4] == 0 && c[5] == 0)
        //                    {
        //                        nc = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/" + c[3] + "/";
        //                        if (!allchart.Any(x => x.Text_ChartCode == nc))
        //                        {
        //                            string Parentcode = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/";
        //                            var parent = allchart.Where(x => x.Text_ChartCode == Parentcode).SingleOrDefault();
        //                            var d = db.Sp_Add_CA_Head(parent.Id, item.Name, null, null);
        //                        }
        //                    }
        //                    else if (c[0] > 0 && c[1] > 0 && c[2] > 0 && c[3] > 0 && c[4] > 0 && c[5] == 0)
        //                    {
        //                        nc = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/" + c[3] + "/" + c[4] + "/";
        //                        if (!allchart.Any(x => x.Text_ChartCode == nc))
        //                        {
        //                            string Parentcode = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/" + c[3] + "/";
        //                            var parent = allchart.Where(x => x.Text_ChartCode == Parentcode).SingleOrDefault();
        //                            var d = db.Sp_Add_CA_Head(parent.Id, item.Name, null, null);
        //                        }
        //                    }
        //                    else if (c[0] > 0 && c[1] > 0 && c[2] > 0 && c[3] > 0 && c[4] > 0 && c[5] > 0)
        //                    {
        //                        nc = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/" + c[3] + "/" + c[4] + "/" + c[5] + "/";
        //                        if (!allchart.Any(x => x.Text_ChartCode == nc))
        //                        {
        //                            string Parentcode = "/" + c[0] + "/" + c[1] + "/" + c[2] + "/" + c[3] + "/" + c[4] + "/";
        //                            var parent = allchart.Where(x => x.Text_ChartCode == Parentcode).SingleOrDefault();
        //                            var d = db.Sp_Add_CA_Head(parent.Id, item.Name, null, null);
        //                        }
        //                    }

        //                }

        //                //Transaction.Commit();
        //                return Json(new Return { Status = true, Msg = "Saved" });
        //            //}
        //            //catch (Exception ex)

        //            //{
        //            //    Transaction.Rollback();
        //            //    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //            //    throw;
        //            //}
        //        //}
                
        //    }
        //    return Json(false);
        //}

      


        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>

        [NoDirectAccess] public ActionResult Index()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult AddCompany()
        {
            ViewBag.Parent_Company = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Company.ToString()).ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddCompany(string Name, string Description, int? Parent_Company)
        {
            var res = db.Comp_Dep_Desig.Any(x => x.Name == Name);
            if (!res)
            {
                db.Sp_Add_CompDepDes(Description, Name, Parent_Company, CompDepDes.Company.ToString()).FirstOrDefault();
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added New Company " + Name, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Parent_Company);
                return Json(true);
            }
            return Json(false);
        }
        [NoDirectAccess] public ActionResult UpdateCompanyDepartmentDesignation()
        {
            ViewBag.Parent_Company = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Company.ToString()).ToList(), "Id", "Name");
            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()).ToList(), "Id", "Name");
            ViewBag.Designation = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Designation.ToString()).ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public JsonResult UpdateCompanyDepartmentDesignation(int? selectedids, string company_name, string selectedoption)
        {
            if (selectedoption == "Please_select")
            {
                return Json(false);
            }
            else if (selectedoption == "Company")
            {
                if (selectedids == null || string.IsNullOrEmpty(company_name))
                {
                    return Json(false);
                }
                else if (selectedids != null && !string.IsNullOrEmpty(company_name))
                {
                    if(db.Comp_Dep_Desig.Any(x=>x.Name == company_name && x.Type == "Company"))
                    {
                        return Json(false);
                    }
                    Comp_Dep_Desig results = (from p in db.Comp_Dep_Desig
                                                    where p.Id == selectedids
                                                    select p).FirstOrDefault();
                    if (!(db.Comp_Dep_Desig.Any(x=>x.Name == company_name && x.Parent_Id == results.Parent_Id && x.Type == "Company")))
                    {
                        results.Name = company_name;
                    }
                    else
                    {
                        return Json(false);
                    }
                    db.Comp_Dep_Desig.Attach(results);
                    db.Entry(results).Property(x => x.Name).IsModified = true;
                    db.SaveChanges();
                    return Json(results);
                }
                else
                {
                    return Json(false);
                }

            }
            else if (selectedoption == "Department")
            {
                if (selectedids == null || string.IsNullOrEmpty(company_name))
                {
                    return Json(false);
                }
                else if (selectedids != null)
                {
                    Comp_Dep_Desig results = (from p in db.Comp_Dep_Desig
                                                    where p.Id == selectedids
                                                    select p).FirstOrDefault();
                    if (!(db.Comp_Dep_Desig.Any(x=>x.Name == company_name && x.Parent_Id == results.Parent_Id && x.Type == "Department")))
                    {
                        results.Name = company_name;
                    }
                    else
                    {
                        return Json(false);
                    }
                    db.Comp_Dep_Desig.Attach(results);
                    db.Entry(results).Property(x => x.Name).IsModified = true;
                    db.SaveChanges();
                    return Json(results);
                }
                else
                {
                    return Json(false);
                }

            }
            else if (selectedoption == "Designation")
            {
                if (selectedids == null || string.IsNullOrEmpty(company_name))
                {
                    return Json(false);
                }
                else if (selectedids != null)
                {
                    Comp_Dep_Desig results = (from p in db.Comp_Dep_Desig
                                                    where p.Id == selectedids
                                                    select p).FirstOrDefault();
                    if(!(db.Comp_Dep_Desig.Any(x=>x.Name == company_name && x.Parent_Id == results.Parent_Id && x.Type == "Designation")))
                    {
                        results.Name = company_name;
                    }
                    else
                    {
                        return Json(false);
                    }
                    db.SaveChanges();
                    return Json(results);
                }
                else
                {
                    return Json(false);
                }
            }
            else
            {
                return Json(false);
            }
          
        }
        [NoDirectAccess] public ActionResult UpdateParentCompany()
        {
            ViewBag.Parent_Company = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Company.ToString()).ToList(), "Id", "Name");
            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()).ToList(), "Id", "Name");
            ViewBag.Designation = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Designation.ToString()).ToList(), "Id", "Name");
            ViewBag.Allnames = new SelectList(db.Comp_Dep_Desig.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public JsonResult UpdateParentCompany(int? parent_id, int? Child_id, string selectedoption)
        {
            if (selectedoption == "Please_select")
            {
                return Json(false);
            }
            else if (selectedoption == "Company")
            {
                Comp_Dep_Desig res2 = (from p in db.Comp_Dep_Desig // ya child ka data ha
                                       where p.Id == Child_id
                                       select p).SingleOrDefault();
                Comp_Dep_Desig res = (from p in db.Comp_Dep_Desig // ya parent ka  data ha
                                      where p.Id == parent_id
                                      select p).SingleOrDefault();

                res2.Parent_Id = res.Id;
                db.SaveChanges();
            }
            else if (selectedoption == "Department")
            {
                Comp_Dep_Desig res2 = (from p in db.Comp_Dep_Desig // ya child ka data ha
                                       where p.Id == Child_id
                                       select p).SingleOrDefault();
                Comp_Dep_Desig res = (from p in db.Comp_Dep_Desig // ya parent ka  data ha
                                      where p.Id == parent_id
                                      select p).SingleOrDefault();
                res2.Parent_Id = res.Id;
                db.SaveChanges();
            }
            else if (selectedoption == "Designation")
            {
                Comp_Dep_Desig res2 = (from p in db.Comp_Dep_Desig // ya child ka data ha
                                       where p.Id == Child_id
                                       select p).SingleOrDefault();

                Comp_Dep_Desig res = (from p in db.Comp_Dep_Desig // ya parent ka  data ha
                                          where p.Id == parent_id
                                          select p).SingleOrDefault();
                res2.Parent_Id = res.Id;
                db.SaveChanges();
            }
            else
            {
                return Json(false);
            }
            return Json(true);
        }
        public JsonResult DeleteNode(int Id)
        {
            db.Sp_Delete_CompDepDes(Id);
            return Json(true);
        }
        public JsonResult RemoveAssetList(int Id)
        {
            db.Sp_Delete_Company_Assets(Id);
            return Json(true);
        }
        [NoDirectAccess] public ActionResult AssetAssinToEmployee()
        {
            ViewBag.AssetName = new SelectList(db.Company_Assets.ToList(), "Id", "Name");
            ViewBag.Employee_Name = new SelectList(db.Employees.ToList(), "Id", "Name");
            return View();
        }
        public JsonResult GetCompanyfor()
        {
            var res1 = db.Sp_Get_CompDepDes(CompDepDes.Company.ToString()).ToList();
            var res = new { compan = res1 };
            return Json(res);
        }
        public JsonResult GetCompany()
        {
            var res = db.Sp_Get_CompDepDes(CompDepDes.Company.ToString()).ToList();
            return Json(res);
        }
        [NoDirectAccess] public ActionResult CompanyList()
        {
            return PartialView();
        }
        [NoDirectAccess] public ActionResult AddDepartments()
        {
            ViewBag.Comp_Id = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Company.ToString()).ToList(), "Id", "Name");
            ViewBag.EmpsList = new SelectList(db.Employees.Where(x => x.Status == "Registered").Select(x => new { x.Id, Name = x.Name + " ( " + x.Employee_Code + " ) " }).ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddDepartments(Comp_Dep_Desig d, int Comp_Id)
        {
            if (d.Parent_Id == null)
            {
                var res = db.Sp_Add_CompDepDes(d.Description, d.Name, Comp_Id, CompDepDes.Department.ToString()).FirstOrDefault();
                return Json(res);
            }
            else
            {
                var res = db.Sp_Add_CompDepDes(d.Description, d.Name, d.Parent_Id, CompDepDes.Department.ToString()).FirstOrDefault();
                return Json(res);
            }
        }
        [HttpPost]
        public JsonResult GetCompanyDepartments(int Id)
        {
            var ee = db.Sp_Get_CompDepartment(Id).ToList();
            var res = new { Departments = ee };
            return Json(res);
        }
        [HttpPost]
        public JsonResult GetCompanyDepartmentsOnly(int Id)
        {
            var ee = (from x in db.Comp_Dep_Desig
                      join y in db.Comp_Dep_Desig on x.Id equals y.Parent_Id into xy
                      from y in xy.DefaultIfEmpty()
                      where x.Type == CompDepDes.Department.ToString() && x.Parent_Id == Id
                      select y).ToList();

            var res = new { Departments = ee };
            return Json(res);
        }
        [NoDirectAccess] public ActionResult AssignmentAsset()
        {

            return PartialView();
        }
        public JsonResult GetDepDesignation(int Id)
        {
            var res = db.Comp_Dep_Desig.Where(x=> x.Type == CompDepDes.Designation.ToString()).ToList();
            return Json(res);
        }
        [NoDirectAccess] public ActionResult AddDesignation()
        {
            ViewBag.Company_Id = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Company.ToString()).ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddDesignation(Comp_Dep_Desig d, int Company_Id)
        {
            if (d.Parent_Id == null)
            {
                var res = db.Comp_Dep_Desig.Any(x => x.Name == d.Name && x.Parent_Id == Company_Id);
                if (!res)
                {
                    var res1 = db.Sp_Add_CompDepDes(d.Description, d.Name, Company_Id, CompDepDes.Designation.ToString()).FirstOrDefault();
                    return Json(res1);
                }
                else
                {
                    return Json(false);
                }
            }
            else
            {
                var res = db.Comp_Dep_Desig.Any(x => x.Name == d.Name && x.Parent_Id == d.Parent_Id);
                if (!res)
                {
                    var res1 = db.Sp_Add_CompDepDes(d.Description, d.Name, d.Parent_Id, CompDepDes.Designation.ToString()).FirstOrDefault();
                    return Json(res1);
                }
                else
                {
                    return Json(false);
                }
            }
        }
        [NoDirectAccess] public ActionResult CompanyHierarchy()
        {
            return PartialView();
        }
        List<Orgchart> resultlist = new List<Orgchart>();
        List<Orgchart> temp = new List<Orgchart>();
        public JsonResult Read()
        {
            string[] Types = { "Company", "Department" };
            List<Orgchart> result = (
                from z in db.Comp_Dep_Desig
                join y in db.Comp_Dep_Desig on z.Id equals y.Parent_Id into zy
                from y in zy.DefaultIfEmpty()
                where Types.Contains( z.Type)
                select new Orgchart { Id = z.Id, ParentId = z.Parent_Id, Name = z.Name }
                         ).OrderBy(x => x.ParentId).Distinct().ToList();
            foreach (var a in result)
            {
                SearchParentId(a, resultlist, result);
            }

            var res = db.Comp_Dep_Desig.ToList();
            string colors = "";
            
                foreach (var a in res)
                {
                    if (a.Type == CompDepDes.Company.ToString())
                    {
                        var str = a.Id.ToString() + ": {color : \"green\" },";
                        colors += str;
                    }
                    else if (a.Type == CompDepDes.Department.ToString())
                    {
                        var str = a.Id.ToString() + ": {color : \"blue\" },";
                        colors += str;
                    }
                    else
                    {
                        var str = a.Id.ToString() + ": {color : \"pink\" },";
                        colors += str;
                    }
                }
            if (colors != "")
            {
                colors = colors.Substring(0, colors.Length - 1);
                colors = "{" + colors + "}";
                var t = JsonConvert.DeserializeObject(colors);
                var tt = JsonConvert.SerializeObject(t);
                var finalres = new { Result = resultlist, Colors = tt.ToString() };
                return Json(finalres, JsonRequestBehavior.AllowGet);
            }

            return Json(false);
        }
        public void SearchParentId(Orgchart user, List<Orgchart> userslist, List<Orgchart> actualList)
        {
            if (user.ParentId == null || userslist.Any(x => x.Id == Convert.ToInt32(user.ParentId)))
            {
                if (!(userslist.Any(x => x.Id == user.Id)))
                {
                    userslist.Add(user);
                }
            }
            else
            {
                var parentuser = actualList.Single(x => x.Id == user.ParentId);
                SearchParentId(parentuser, userslist, actualList);
                SearchParentId(user, userslist, actualList);
            }
        }
        //Asset MAanagement
        [NoDirectAccess] public ActionResult AddAsset()
        {
            ViewBag.Company_Id = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Company.ToString()).ToList(), "Id", "Name");
            ViewBag.Type = new SelectList(db.Company_Assets_Types.ToList(), "Id", "Types");
            return View();
        }
        [HttpPost]
        public JsonResult AddAsset(Company_Assets  Company_Asset, Company_Assets_Types Companyassetstype, string selectedopt)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            if (selectedopt == "Add_Assets_Types")
            {
                var res = db.Company_Assets_Types.Any(x => x.Types == Companyassetstype.Types);
                if (!res)
                {
                   

                    db.Company_Assets_Types.Add(Companyassetstype);
                    db.Sp_Assets_Logs("Add", "Type Add" ,userid, Companyassetstype.Types, "---", "---");

                    db.SaveChanges();
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            else if (selectedopt == "Add_Assets")
            {
                Helpers hel = new Helpers(Modules.AssetManagement, Types.AssetManagement);
                object[] Data = new object[2];
                if (Company_Asset.Name != null)
                {
                    Data[0] = Company_Asset.Name;
                }
                else
                {
                    Data[0] = Company_Asset.Vehicle_Company;
               
                }
                
                QRCodeReturn Qrid = hel.GenerateQRCode(Data);
                db.Sp_Add_Asset(Company_Asset.Name, Company_Asset.Type, Company_Asset.Specs, Company_Asset.Company_Id, Qrid.Id,
                                Company_Asset.Vehicle_Model,Company_Asset.Vehicle_Company,Company_Asset.Vehicle_Registration_Number,
                                Company_Asset.Vehicle_Fuel,Company_Asset.Fuel_Type_Id);
                db.Sp_Assets_Logs("Add", "Asset Add", userid, Company_Asset.Name, Company_Asset.Specs, "---");
                var returnData = new { ReturnData = ViewBag.QRCodeImage = Qrid.QR_Image };
                return Json(returnData);
               
              
            }
            else
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult AllAssetsList()
        {
            var res = db.Sp_Get_All_AssetList().ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult AllVehicleList()
        {
            var res = db.Sp_Get_All_AssetList().ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult RemaningAssets(string AssetName)
        {
            ViewBag.EmployeesNames = new SelectList(db.Sp_Get_Employee().ToList(), "Id", "Name", "Department", 1);
            var res = db.Sp_Get_Remaning_Assets_List(AssetName).ToList();
            return PartialView(res);
        }
        public JsonResult DeleteAsset(int? Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = (from z in db.Employee_Assets
                       where z.Asset_Id == Id
                       select z).SingleOrDefault();
            var assets = (from z in db.Company_Assets
                          where z.Id==res.Asset_Id
                          select z).SingleOrDefault();
            var Emp_Name = (from z in db.Employees
                            where z.Id == res.Employee_Id
                            select z).SingleOrDefault();
            db.Sp_Delete_Assign_Assets(Id);
            db.Sp_Assets_Logs("Return", "Return Asset From Employee", userid, assets.Name, assets.Specs, Emp_Name.Name);

            return Json(true);
            
        }
        //[NoDirectAccess] public ActionResult AssetDetailsList(string AssetName)
        //{
        //    var res = db.Sp_Get_Assign_List_Details(AssetName).ToList();
        //    return PartialView(res);
        //}
        [HttpPost]
        public JsonResult AssetAssinToEmployee(Employee_Assets Emp_Asset)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var assets = (from z in db.Company_Assets
                           where z.Id== Emp_Asset.Asset_Id
                           select z).SingleOrDefault();
            var Emp_Name = (from z in db.Employees
                          where z.Id == Emp_Asset.Employee_Id
                          select z).SingleOrDefault();
            if (ModelState.IsValid)
            {
                db.Employee_Assets.Add(Emp_Asset);
                db.Sp_Assets_Logs("Assign", "Assign Asset To Employee", userid, assets.Name, assets.Specs, Emp_Name.Name);
                db.SaveChanges();
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        //[NoDirectAccess] public ActionResult AssetLogsList()
        //{
        //    var res= db.Sp_Get_Assets_Logs().ToList();
        //    return View(res);
        //}
        ////////////////

        [NoDirectAccess] public ActionResult AddVehical()
        {
            ViewBag.Company_Id = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Company.ToString()).ToList(), "Id", "Name");
            ViewBag.Type = new SelectList(db.Company_Assets_Types.ToList(), "Id", "Types");
            ViewBag.FuelTpe = new SelectList(db.Fuels.ToList(), "Id", "Fuel_Type");
            return View();
        }
        



        ////////////
        ///Fuel
        ////////////
      
            
        [NoDirectAccess] public ActionResult FuelDetails()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var VehFuel = db.Company_Assets.Where(x => x.Fuel_Type_Id != null && x.Vehicle_Fuel != null).ToList();
            foreach (var item in VehFuel)
            {
                var rate = db.Fuels.Where(x => x.Id == item.Fuel_Type_Id).Select(x => x.Fuel_Rate).SingleOrDefault();
                db.Sp_Add_Fuel_Details(item.Id, item.Vehicle_Fuel, rate);
            }

            var res1 = db.Sp_Get_Fuel_Details().ToList();
            db.Sp_Assets_Logs("View Fuel Details", GeneralMethods.GetIPAddress(), userid, null, null, null);
            return View(res1);
        }
        [NoDirectAccess] public ActionResult AddFuel()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult UpdateFuelConsum(Vehicle_Fuel_Details details)
        {
            db.Sp_Update_FuelConsumption(details.Vehicle_Id, details.Vehicle_Consumption);
            var res = db.Sp_Update_Fuel_Consumption(details.Vehicle_Id, details.Vehicle_Consumption, details.Vehicle_Fuel_Allow, details.Vehicle_Fuel_Rate);
            return Json(true);
        }
        [HttpPost]
        public JsonResult AddFuelRates(Fuel fuel)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Fuel(fuel.Fuel_Type, fuel.Fuel_Rate, DateTime.UtcNow);
            db.Sp_Assets_Logs("Add Fuel Rates", GeneralMethods.GetIPAddress(), userid, null, null, null);

            return Json(true);
        }
        [NoDirectAccess] public ActionResult UpdateFuel()
        {
            var res = db.Fuels.ToList();
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult UpdateFuelRates(Fuel fuel)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_Fuel_Rate(fuel.Id, fuel.Fuel_Rate, fuel.Date);
            db.Sp_Assets_Logs("Update Fuel Rates", GeneralMethods.GetIPAddress(), userid, null, null, null);

            return Json(true);
        }

        public JsonResult SaveNewDepEmployees(int depId, long[] emp)
        {
            using(var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var res = db.Comp_Dep_Desig.Where(x => x.Id == depId).Select(x => x.Parent_Id).FirstOrDefault();
                    foreach (var v in emp)
                    {
                        Employee_Designations ed = new Employee_Designations();
                        ed.Department_Id = depId;
                        ed.Company_Id = (int)res;
                        ed.Emp_Id = v;
                        db.Employee_Designations.Add(ed);
                        db.SaveChanges();
                    }
                    trans.Commit();
                    return Json(true);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(false);
                }
            }
        }
        public ActionResult AssignCompany()
        {
            ViewBag.Company = new SelectList(db.Sp_Get_Companies(), "Id", "Company_Name");
            ViewBag.Users = new SelectList(db.Sp_Get_Users(), "Id", "Name");
            return View();

            //var wbd = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Type == "WBD").Select(x => new { x.Id, x.Title, x.Activity_Id }).Distinct().ToList();
            //ViewBag.Dependency = new SelectList(activity, "Id", "Title");
            //ViewBag.WBDDDown = new SelectList(wbd, "Id", "Title");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AssignCompany(long? Users, long? Company)
        {
            var res = Convert.ToBoolean(db.Sp_Add_AssignCompany(Company, Users).FirstOrDefault());
            if (res)
            {
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Assigned New Company to users " + Users + " " + Company, "Create", "Activity_Record", ActivityType.Roles.ToString(), Users);
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult UserCompanyManage(long Id)
        {
            var res = db.Sp_Get_UserAllCompany(Id).ToList();
            ViewBag.Username = db.Users.Where(x => x.Id == Id).Select(x => x.Email).FirstOrDefault();
            return PartialView(res);
        }
        public JsonResult unassigncompany(long Id, long uid)
        {
            db.Sp_Delete_UserCompany(Id, uid);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Unassigned Company User Role " + Id + " " + uid, "", "Activity_Record", ActivityType.Roles.ToString(), Id);
            return Json(true);
        }
    }
}
