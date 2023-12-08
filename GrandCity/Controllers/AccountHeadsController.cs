//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MeherEstateDevelopers.Models;
//using Microsoft.AspNet.Identity;
//using MeherEstateDevelopers.Filters;
//using System.Xml.Linq;

//namespace MeherEstateDevelopers.Controllers
//{
//    [LogAction]
//    [Authorize]
//    public class AccountHeadsController : Controller
//    {
//        // GET: AccountHeads
//        private Grand_CityEntities db = new Grand_CityEntities();

//        private FinancialYear GetFinancialYear()
//        {
//            return new FinancialYear { Start = new DateTime(2022, 7, 1), End = new DateTime(2023, 6, 30) };
//        }
//        public ActionResult AccountHeadList()
//        {
//            return View();
//        }
//        [HttpGet]
//        public JsonResult GetHead(string q)
//        {
//            // Have to pass Chart of Account id in leave notes
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var allsearch = db.Sp_Get_CA_LeaveNodes_HeadSearch(q, comp.COA).Select(x => new { id = x.Id, text = x.Text_ChartCode + " - " + x.Head, Parent = x.parent, head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetHeadCashBanks(string q)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var allsearch = db.Sp_Get_CA_LeaveNodes_HeadSearch_Cash_Bank(q, comp.COA).Select(x => new { id = x.Id, text = x.Text_ChartCode, Parent = x.parent, head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetHeadInventoryExpenses(string q, string code)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var allsearch = db.Sp_Get_CA_LeaveNodes_HeadSearch_Parameter(q, comp.COA_Head + code, comp.COA).Select(x => new { id = x.Id, text = x.Text_ChartCode + " - " + x.Head, Parent = x.parent, head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetHeadLiabilities(string q)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var allsearch = db.Sp_Get_CA_LeaveNodes_HeadSearch_Liabilities(q, comp.COA).Select(x => new { id = x.Id, text = x.Text_ChartCode + " - " + x.Head, Parent = x.parent, head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult RFDSearch(string q)
//        {
//            var result = db.Vendors.Where(x => x.Company.Contains(q)).Select(x => new Vendor_serch { Id = x.Id, Name = x.Company, address = x.Address, Phone = x.Office_Mobile }).ToList();
//            return Json(new { items = result }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetHeadInventory(string q, string code)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var allsearch = db.Sp_Get_CA_LeaveNodes_HeadSearch_Parameter(q, comp.COA_Head + code, comp.COA).Select(x => new { id = x.Id, text = x.Text_ChartCode + " - " + x.Head, Parent = x.parent, head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetHeadWildCard(string q, string code)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var allsearch = db.Sp_Get_CA_Head_Param_Code_WildCard(q, code, comp.COA).Select(x => new { id = x.Head, text = x.Head, head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetHead_generate_bill(string q)
//        {

//            var allsearch = db.Sp_Get_CA_Head_Invtory(q).Select(x => new { id = x.Id, text = x.Text_ChartCode, Parent = x.parent, head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);

//        }
//        public ActionResult HeadLedger(DateTime? From, DateTime? To, string Code, string Head)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (From == null || To == null)
//            {
//                var fy = this.GetFinancialYear();
//                From = fy.Start;
//                To = fy.End;
//            }
//            AccountHandlerController ahc = new AccountHandlerController();
//            var comp = ahc.Company_Attr(userid);
            
//            //Code = comp.COA_Head + Code;
//            var All = db.Sp_Get_Cashiers_List().ToList();
//            string chids = new XElement("ChPoDd", All.Select(x => new XElement("Details",
//                                  new XAttribute("Ids", x.Id)
//                                 ))).ToString();
//            var res = db.Sp_Get_JournalLedger_WO_Balance(From, To, Code, chids).ToList();
//            ViewBag.LedgerNature = this.Ledger_Top(Code);
//            var res1 = db.Sp_Get_CA_Head_Param_Code(Code).FirstOrDefault();


//            ViewBag.Id = res1.Id;
//            ViewBag.From = From;
//            ViewBag.To = To;
//            ViewBag.Code = res1.HeadCode;
//            ViewBag.Head = res1.Head;
//            return PartialView(res);
//        }
//        public ActionResult AddUpdateNode(int Id)
//        {
//            var fy = GetFinancialYear();
//            ViewBag.Start = String.Format("{0:MM/dd/yyyy}", fy.Start);
//            ViewBag.End = String.Format("{0:MM/dd/yyyy}", fy.End);
//            var res = db.Sp_Get_CA_Head_Parameter(Id).SingleOrDefault();

//            long userid = long.Parse(User.Identity.GetUserId());
//            var claim = db.UserClaims.Where(x => x.UserId == userid).ToList();
//            var comp_coa = claim.Where(x => x.ClaimType == "Comp_COA").Select(x => x.ClaimValue).FirstOrDefault();

//            var res1 = db.Sp_Get_CA_Head(null, int.Parse(comp_coa)).Select(x => new { Value = x.Id, Text = x.Text_ChartCode + " - " + x.Head }).ToList();
//            //ViewBag.Type = new SelectList(res1, "Value", "Text");
//            ViewBag.Type = null;
//            return PartialView(res);
//        }
//        public JsonResult UpdateHeadName(int Id, string Head)
//        {
//            var a = db.Sp_Update_CA_Head(Id, Head);
//            return Json(true);
//        }
//        public ActionResult Create()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var claim = db.UserClaims.Where(x => x.UserId == userid).ToList();
//            var comp_coa = claim.Where(x => x.ClaimType == "Comp_COA").Select(x => x.ClaimValue).FirstOrDefault();

//            ViewBag.Type = new SelectList(db.Sp_Get_CA_Head(0, int.Parse(comp_coa)).ToList(), "Id", "Head");
//            return PartialView();
//        }
//        [HttpPost]
//        public JsonResult Create(int Id, string Head)
//        {
//            var head = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
//            var res = db.Sp_Add_CA_Head(Id, Head, null, null,head.Nature);
//            return Json(true);
//        }
//        public ActionResult TrailBalance()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
            
//            var fy = this.GetFinancialYear();
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid); 

//            var res = db.Sp_Get_Trial_Balance_New( fy.Start, fy.End,comp.Id).ToList();
//            return View(res);
//        }
//        public JsonResult GetHeadTaxAccounts(string q)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var fy = this.GetFinancialYear();
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var allsearch = db.Sp_Get_CA_LeaveNodes_HeadSearch_TaxAccounts(q,comp.Id).Select(x => new { id = x.Id, text = x.Text_ChartCode + " - " + x.Head, Parent = "", head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult GetHeadLandAccounts(string q)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var fy = this.GetFinancialYear();
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var allsearch = db.Sp_Get_CA_LeaveNodes_HeadSearch_LandAccounts(q, comp.Id).Select(x => new { id = x.Id, text = x.Text_ChartCode + " - " + x.Head, Parent = "", head = x.Head }).ToList();
//            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult BalanceSheet()
//        {
//            var fiscDate = DateTime.Now;
//            ViewBag.toDate = fiscDate.ToShortDateString();
//            var res = db.Sp_Get_Balance_Sheet_New(3, fiscDate.AddDays(1)).ToList();

//            var root = new HierarchicalNodeBalanceSheet("", "", 0, 0, 0, 0, 0);
//            foreach (var node in res)
//            {
//                var current = root;
//                string[] parts = node.ChartNodeString.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
//                for (int i = 0; i < parts.Count() - 1; i++)
//                {
//                    int parsedPart = int.Parse(parts[i]);
//                    current = current.Decendants[parsedPart - 1];
//                }
//                current.Decendants.Add(new HierarchicalNodeBalanceSheet(node.Head, node.ChartNodeString, node.Id, node.Debit ?? 0, node.Credit ?? 0, node.ChartLevel ?? 0, Convert.ToInt32(parts[0])));
//            }
//            List<HierarchicalNodeBalanceSheet> a = new List<HierarchicalNodeBalanceSheet>();
//            a.Add(root);


//            return View(a);
//        }
//        public ActionResult SearchLedger()
//        {
//            return View();
//        }
//        //public void GenerateFileLedger()
//        //{
//        //    var res = db.File_Form.Where(x => x.Status == 1).ToList();
//        //    foreach (var item in res)
//        //    {
//        //        db.Sp_Add_CA_Head(1067, item.FileFormNumber.ToString(), null, null);
//        //        //db.Sp_Add_CA_Head(1063, item.FileFormNumber.ToString(), null, null);
//        //    }
//        //}
//        public void GenerateReceivable()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.File_Form.Where(x => x.Status == 1).ToList();

//            foreach (var item in res)
//            {
//                var res1 = db.Sp_Get_CA_Head_MultiRef("Assets", item.FileFormNumber.ToString()).FirstOrDefault();
//                if (res1 == null)
//                {
//                    continue;
//                }
//                else
//                {
//                    var Total = db.File_Installments.Where(x => x.File_Id == item.Id).Sum(x => x.Amount);
//                    var Received_Amt = db.Sp_Get_ReceivedAmounts(item.Id, "FileManagement").Where(x =>
//             (x.Status == "Approved" || string.IsNullOrEmpty(x.Status) || string.IsNullOrWhiteSpace(x.Status)) && (x.PaymentType != "Cash" || string.IsNullOrEmpty(x.PaymentType) || string.IsNullOrWhiteSpace(x.PaymentType))

//                    ).Sum(x => x.Amount);

//                    var Amount = Total - Received_Amt;

//                    Helpers h = new Helpers();
//                    var TransactionId = h.RandomNumber();

//                    //var Debit = db.Sp_Add_Journal_Entry(Amount, 0, res1.Code + " - " + res1.Head, res1.Id, res1.Code, res1.Head, "Total Plot Receiveable", TransactionId, 1, userid, userid, null).FirstOrDefault();
//                    //var d = db.Sp_Add_General_Entry_Supervised(0, Amount, res5.Code + " - " + res5.Head, res5.Id, res3.Text_ChartCode, res3.Head, string.Join(" - ", res2.Select(x => x.Naration)), TransactionId, 1, null, null, null, userid, res2.Select(x => x.Supvise_by).FirstOrDefault(), res2.Select(x => x.Sup_Name).FirstOrDefault(), res2.Select(x => x.Sup_Date).FirstOrDefault(), Id).FirstOrDefault();   /// For Cash and Bank Credit
//                }
//            }
//        }
//        [Authorize(Roles = "Chart of Account Mappping, Administrator")]
//        public ActionResult COA_Mapper()
//        {
//            return View();
//        }
//        public ActionResult PettyCash_Config()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Account_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Petty_Cash.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Petty_Cash_Account.ToString()
//                        ).FirstOrDefault();

//            var res2 = db.Sp_Get_COAMapper_PettyCash(comp.Id).ToList();
//            var res3 = db.Sp_Get_COAMapper_PettyCash_Expense(comp.Id).ToList();

//            return PartialView(new PettyCashMappers { PettyCash = res1, PettyCash_Accounts = res2,PettyCash_Expense = res3 });
//        }
//        public ActionResult Vendor_Config()
//        {
//            return PartialView();
//        }
//        public JsonResult Add_PTC()
//        {
//            return Json(true);
//        }
//        public JsonResult Add_PTC_Config(long Id, int COA, string Type)
//        {
//            var res = db.Sp_Add_HeadConfiguration(COA, Id, Type);
//            return Json(true);
//        }
//        //public ActionResult GetPTC_Config()
//        //{
//        //    var res = db.Head_Configuration.Where(x => x.t == COA_Config_Types.Petty_Cash.ToString());
//        //    return PartialView(res);
//        //}
//        public JsonResult AddPrjHead(long Head_Code, long Prj_Id)
//        {
//            var a = db.Sp_Get_CA_Head_Parameter(Head_Code).FirstOrDefault();
//            var res = db.Sp_Update_PrjHead(Prj_Id, a.Text_ChartCode, "Update");
//            return Json(new Return { Status = true, Msg = "Prj Head Added" });
//        }
//        public JsonResult DeletePrjHead(long Prj_Id)
//        {
//            var res = db.Sp_Update_PrjHead(Prj_Id, null, "Delete");
//            return Json(new Return { Status = true, Msg = "Prj Head Removed" });
//        }
//        public JsonResult GetNodes_Tree(string node)
//        {
//            if (node == "#")
//            {
//                long userid = long.Parse(User.Identity.GetUserId());
//                var claim = db.UserClaims.Where(x => x.UserId == userid).ToList();
//                var comp_coa = claim.Where(x => x.ClaimType == "Comp_COA").Select(x => x.ClaimValue).FirstOrDefault();

//                var res = db.Sp_Get_CA_Head(1, int.Parse(comp_coa)).Select(x => new { id = x.Id, parent = "#", text = x.Text_ChartCode + " -  " + x.Head, children = true }).ToList();
//                return Json(res, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {
//                long id_Parsed = Convert.ToInt32(node);
//                var res = db.Sp_Get_ImmediateNodes(id_Parsed).Select(x => new { id = x.Id, text = x.Text_ChartCode + " -  " + x.Head, children = true }).ToList();
//                return Json(res, JsonRequestBehavior.AllowGet);
//            }
//        }
//        public JsonResult GetAllHeads()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());

//            var claim = db.UserClaims.Where(x => x.UserId == userid).ToList();
//            var comp_coa = claim.Where(x => x.ClaimType == "Comp_COA").Select(x => x.ClaimValue).FirstOrDefault();

//            var res = db.Sp_Get_CA_Head(null, int.Parse(comp_coa)).ToList();
//            var root = new HierarchicalNode("Chart of Accounts", "", 0);
//            foreach (var node in res)
//            {
//                var current = root;
//                string[] parts = node.Text_ChartCode.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
//                for (int i = 0; i < parts.Count() - 1; i++)
//                {
//                    int parsedPart = int.Parse(parts[i]);
//                    current = current.Decendants[parsedPart - 1];
//                }
//                current.Decendants.Add(new HierarchicalNode(node.Head, node.Text_ChartCode, node.Id));
//            }
//            List<HierarchicalNode> a = new List<HierarchicalNode>();
//            a.Add(root);
//            JsonResult resultHeads = Json(new { LinearData = res.Select(x => new { Head = x.Head, Code = x.Text_ChartCode, Id = x.Id, HasBeenFound = false }).ToList(), StructuredData = a }, JsonRequestBehavior.AllowGet);
//            resultHeads.MaxJsonLength = int.MaxValue;
//            return resultHeads;
//        }
//        public void vouchercreation()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var a = this.GetFinancialYear();
//            var All = db.Sp_Get_Cashiers_List().ToList();
//            ViewBag.Users = new SelectList(All, "id", "Name");
//            string chids = new XElement("ChPoDd", All.Select(x => new XElement("Details",
//                                  new XAttribute("Ids", x.Id)
//                                 ))).ToString();
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var Payment = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Cash_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
//            //var CashCode = db.Sp_Get_CA_Head_MultiRef("Assets", "Cash").FirstOrDefault().Code;
//            var res1 = db.Sp_Get_CashBankBook_Closing(a.Start, a.End, "Cash", Payment.Text_ChartCode, chids).Where(x => x.Voucher_Type == null).ToList();
//            var res2 = db.Sp_Get_JournalLedger(a.Start, a.End, Payment.Text_ChartCode).Where(x => x.Voucher_Type == null).ToList();

//            foreach (var item in res1)
//            {
//                if (item.Credit > 0)
//                {
//                    db.Plot_add_Plot("", item.GroupId, 0, "GE", "CPV", 0, "", "", "", "", "");
//                }
//                else if (item.Debit > 0)
//                {
//                    db.Plot_add_Plot("", item.GroupId, 0, "GE", "CRV", 0, "", "", "", "", "");
//                }
//            }

//            foreach (var item in res2)
//            {
//                if (item.Credit > 0)
//                {
//                    db.Plot_add_Plot("", item.GroupId, 0, "JE", "CPV", 0, "", "", "", "", "");
//                }
//                else if (item.Debit > 0)
//                {
//                    db.Plot_add_Plot("", item.GroupId, 0, "JE", "CRV", 0, "", "", "", "", "");
//                }
//            }
//        }
//        public void voucherBcreation()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var a = this.GetFinancialYear();
//            var All = db.Sp_Get_Cashiers_List().ToList();
//            ViewBag.Users = new SelectList(All, "id", "Name");
//            string chids = new XElement("ChPoDd", All.Select(x => new XElement("Details",
//                                  new XAttribute("Ids", x.Id)
//                                 ))).ToString();
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);
//            var Payment = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Bank_Account.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);

//            //var BankCode = db.Sp_Get_CA_Head_MultiRef("Assets", "Bank Instruments").FirstOrDefault().Code;
//            var res1 = db.Sp_Get_CashBankBook_Closing(a.Start, a.End, "Bank", Payment.Text_ChartCode, chids).Where(x => x.Voucher_Type == null).ToList();
//            var res2 = db.Sp_Get_JournalLedger(a.Start, a.End, Payment.Text_ChartCode).Where(x => x.Voucher_Type == null).ToList();

//            foreach (var item in res1)
//            {
//                if (item.Credit > 0)
//                {
//                    db.Plot_add_Plot("", item.GroupId, 0, "GE", "BPV", 0, "", "", "", "", "");
//                }
//                else if (item.Debit > 0)
//                {
//                    db.Plot_add_Plot("", item.GroupId, 0, "GE", "BRV", 0, "", "", "", "", "");
//                }
//            }

//            foreach (var item in res2)
//            {
//                if (item.Credit > 0)
//                {
//                    db.Plot_add_Plot("", item.GroupId, 0, "JE", "BPV", 0, "", "", "", "", "");
//                }
//                else if (item.Debit > 0)
//                {
//                    db.Plot_add_Plot("", item.GroupId, 0, "JE", "BRV", 0, "", "", "", "", "");
//                }
//            }


//        }

//        public string Ledger_Top(string Code)
//        {
//            string parent = "";
//            var a = Code.Split('/');
//            if (a[2] == "1")
//            {
//                parent = "Assets";
//            }
//            else if (a[2] == "2")
//            {
//                parent = "Liability";
//            }
//            else if (a[2] == "3")
//            {
//                parent = "Equity";
//            }
//            else if (a[2] == "4")
//            {
//                parent = "Income";
//            }
//            else if (a[2] == "5")
//            {
//                parent = "Expense";
//            }
//            return parent;

//        }
       
//        public ActionResult AddReceivables()
//        {
//            ViewBag.TransactionId = new Helpers().RandomNumber();
//            var code1 = "/1/1/1";
//            var code2 = "/1/1/3";
//            var code3 = "/1/1/4";
//            ViewBag.ReceivableAccounts = new SelectList(db.Sp_Get_Receivables_Accounts(code1).Select(x => new { id = x.Id, text = x.Type }), "id", "text").ToList();
//            ViewBag.debitAccounts = new SelectList(db.Sp_Get_Cash_Bank_Accounts(code2, code3).Select(x => new { id = x.Id, text = x.Type }), "id", "text").ToList();

//            return PartialView();
//        }
//        public JsonResult RecordReceivableEntries(List<Voucher_Details_General_Entries> Details, long TransactionId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            if (User.IsInRole("Head Cashier"))
//            {
//                string JournalEnt = new XElement("JournalEntries", Details.Select(x => new XElement("Entries",
//                new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
//                new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
//                new XAttribute("Head_Name", this.GetAccount(x.Account_Head_Id).Head),
//                new XAttribute("Head_Code", this.GetAccount(x.Account_Head_Id).Text_ChartCode),
//                new XAttribute("Head_Id", x.Account_Head_Id),
//                new XAttribute("Line", x.Line),
//                new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//                new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
//                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
//                new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
//                new XAttribute("Comp_Id", comp.Id),
//                new XAttribute("GroupId", TransactionId)
//                ))).ToString();
//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var res = db.Sp_Add_Journal_Entries(JournalEnt, userid).FirstOrDefault();
//                        Transaction.Commit();
//                        return Json(true);

//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        EmailService e = new EmailService();
//                        e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "General Entry Recording Error");
//                        return Json(false);
//                    }
//                }
//            }
//            else
//            {
//                string GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
//                new XAttribute("Naration", (x.Item_Name == null) ? "" : x.Item_Name),
//                new XAttribute("Head", (x.Account_Head == null) ? "" : x.Account_Head),
//                new XAttribute("Head_Name", this.GetAccount(x.Account_Head_Id).Head),
//                new XAttribute("Head_Code", this.GetAccount(x.Account_Head_Id).Text_ChartCode),
//                new XAttribute("Head_Id", x.Account_Head_Id),
//                new XAttribute("Line", x.Line),
//                new XAttribute("Qty", (x.Quantity == null) ? 0 : x.Quantity),
//                new XAttribute("UOM", (x.UOM == null) ? "" : x.UOM),
//                new XAttribute("Rate", (x.Rate == null) ? 0 : x.Rate),
//                new XAttribute("Debit", (x.Debit == null) ? 0 : x.Debit),
//                new XAttribute("Credit", (x.Credit == null) ? 0 : x.Credit),
//                new XAttribute("Comp_Id", comp.Id),
//                new XAttribute("GroupId", TransactionId)
//                ))).ToString();
//                using (var Transaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var res = db.Sp_Add_General_Entries(GeneralEnt, userid).FirstOrDefault();
//                        Transaction.Commit();
//                        return Json(true);

//                    }
//                    catch (Exception ex)
//                    {
//                        Transaction.Rollback();
//                        EmailService e = new EmailService();
//                        e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "General Entry Recording Error");
//                        return Json(false);
//                    }
//                }
//            }
//        }
//        public Sp_Get_CA_Head_Parameter_Result GetAccount(int Id)
//        {
//            var res = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
//            return res;
//        }
//        /// ///////////////////
//        public ActionResult PayableMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            string[] vs = {
//                            COA_Mapper_ModuleTypes.Vendor.ToString(),
//                            COA_Mapper_ModuleTypes.General_Trade_Payable.ToString(),
//                            COA_Mapper_ModuleTypes.Advance_Trade_payable.ToString(),
//                            COA_Mapper_ModuleTypes.Purchase_Need_Supervision.ToString()
//                           };
//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.Module == COA_Mapper_Modules.Procurement.ToString() &&
//                        vs.Contains(x.ModuleType)
//                        ).ToList();
//            return PartialView(res);
//        }
//        public JsonResult UpdatePayableMapper(int? V, int? ATP, int? GP, int? PNS)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            string[] vs = {
//                            COA_Mapper_ModuleTypes.Vendor.ToString(),
//                            COA_Mapper_ModuleTypes.General_Trade_Payable.ToString(),
//                            COA_Mapper_ModuleTypes.Advance_Trade_payable.ToString(),
//                            COA_Mapper_ModuleTypes.Purchase_Need_Supervision.ToString()
//                           };
//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.Module == COA_Mapper_Modules.Procurement.ToString() &&
//                        vs.Contains(x.ModuleType)
//                        ).ToList();
//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Procurement.ToString(),
//                                           COA_Mapper_ModuleTypes.General_Trade_Payable.ToString(),
//                                           null,
//                                           GP,
//                                           userid);
//            // for Advance payables
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Procurement.ToString(),
//                                           COA_Mapper_ModuleTypes.Advance_Trade_payable.ToString(),
//                                           null,
//                                           ATP,
//                                           userid);
//            // for Purchase Need Supervision
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Procurement.ToString(),
//                                           COA_Mapper_ModuleTypes.Purchase_Need_Supervision.ToString(),
//                                           null,
//                                           PNS,
//                                           userid);
//            // for Vendor
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Account_Head.ToString(),
//                                           COA_Mapper_Modules.Procurement.ToString(),
//                                           COA_Mapper_ModuleTypes.Vendor.ToString(),
//                                           null,
//                                           V,
//                                           userid);

//            return Json(true);
//        }
//        ////////////////////////////////
//        public ActionResult DealersMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Account_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Dealership.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Dealership.ToString()
//                        ).FirstOrDefault();

//            var res2 = db.Sp_Get_COAMapper_Dealerlist(comp.Id).ToList();
//            var res3 = db.Sp_Get_COAMapper_Dealerlist_Commission().ToList();

//            return PartialView(new DealerMappers { Dealership = res1, DealerList = res2, DealerListCom = res3 });
//        }
//        public JsonResult UpdateDealershipMapper(int? Dealer)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            string[] vs = {
//                            COA_Mapper_ModuleTypes.Dealership.ToString(),
//                           };
//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.Module == COA_Mapper_Modules.Dealership.ToString() &&
//                        vs.Contains(x.ModuleType)
//                        ).ToList();
//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Account_Head.ToString(),
//                                           COA_Mapper_Modules.Dealership.ToString(),
//                                           COA_Mapper_ModuleTypes.Dealership.ToString(),
//                                           null,
//                                           Dealer,
//                                           userid);

//            return Json(true);
//        }
//        public JsonResult UpdateDealerCommissionMapper(int? Dealer, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();

//            var res = db.COA_Mapper.Where(x =>
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Dealership.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Dealership_Commission.ToString() &&
//                        x.Module_Id == Dealer
//                        ).ToList();
//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Dealership.ToString(),
//                                           COA_Mapper_ModuleTypes.Dealership_Commission.ToString(),
//                                           Dealer,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//        public JsonResult UpdateDealerMapper(int? Dealer, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Dealership.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Dealership_List.ToString() &&
//                        x.Module_Id == Dealer
//                        ).ToList();
//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Dealership.ToString(),
//                                           COA_Mapper_ModuleTypes.Dealership_List.ToString(),
//                                           Dealer,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//        public JsonResult UpdatePettyCashAccountMapper(int? PettyCash, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Petty_Cash.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Petty_Cash_Account.ToString() &&
//                        x.Module_Id == PettyCash
//                        ).ToList();
//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Petty_Cash.ToString(),
//                                           COA_Mapper_ModuleTypes.Petty_Cash_Account.ToString(),
//                                           PettyCash,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//        public JsonResult UpdatePettyCashExpMapper(int? PettyCash, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Petty_Cash.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Petty_Cash_Expense.ToString() &&
//                        x.Module_Id == PettyCash
//                        ).ToList();
//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Petty_Cash.ToString(),
//                                           COA_Mapper_ModuleTypes.Petty_Cash_Expense.ToString(),
//                                           PettyCash,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//        public JsonResult UpdateDealerSalesMapper(int? Dealer, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Dealership.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Dealership_Commission.ToString() &&
//                        x.Module_Id == Dealer
//                        ).ToList();
//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Dealership.ToString(),
//                                           COA_Mapper_ModuleTypes.Dealership_Commission.ToString(),
//                                           Dealer,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//        /////////////////////////////
//        public ActionResult ReceivableMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.Sp_Get_COAMapper_BlocksResidential(comp.Id).ToList();
//            var res2 = db.Sp_Get_COAMapper_BlocksCommercial(comp.Id).ToList();

//            return PartialView(new ReceivableMappers { Residential = res1, Commercial = res2 });
//        }
//        public JsonResult UpdateBlockReceivableMapper(int? Block, int? Acc_Id, string Type)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Files_Plots.ToString() &&
//                        x.ModuleType == Type &&
//                        x.Module_Id == Block
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Files_Plots.ToString(),
//                                           Type,
//                                           Block,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//        /////////////////////////////
//        public ActionResult SalesMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.Sp_Get_COAMapper_Blocks_Sales_Residential(comp.Id).ToList();
//            var res2 = db.Sp_Get_COAMapper_Blocks_Sales_Commercial(comp.Id).ToList();

//            return PartialView(new SalesMappers { Residential = res1, Commercial = res2 });
//        }
//        public JsonResult UpdateBlockSalesMapper(int? Block, int? Acc_Id, string Type)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Files_Plots.ToString() &&
//                        x.ModuleType == Type &&
//                        x.Module_Id == Block
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Files_Plots.ToString(),
//                                           Type,
//                                           Block,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }

//        //////////////////////////////

//        public ActionResult OtherIncomMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            string[] vs = {
//                            COA_Mapper_ModuleTypes.Plot_Sales_Commission_Expense.ToString(),
//                             COA_Mapper_ModuleTypes.Fines_And_Penalties.ToString(),
//                             COA_Mapper_ModuleTypes.Reinstate_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Posession_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Duplicate_Allotment_Letter.ToString(),
//                             COA_Mapper_ModuleTypes.Registry_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Power_Of_Attorney.ToString(),
//                             COA_Mapper_ModuleTypes.Out_Station_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Other_Recovery.ToString(),
//                             COA_Mapper_ModuleTypes.Duplicate_Customer_File.ToString(),
//                             COA_Mapper_ModuleTypes.Other_Transfer_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Advance_Tax_236_C.ToString(),
//                             COA_Mapper_ModuleTypes.Advance_Tax_236_K.ToString(),
//                             COA_Mapper_ModuleTypes.New_Connection_Charges.ToString()
//                           };
//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.Module == COA_Mapper_Modules.Files_Plots.ToString() &&
//                        vs.Contains(x.ModuleType)
//                        ).ToList();
//            return PartialView(res);
//        }
//        public JsonResult UpdateOtherIncomMapper(int? A, int? B, int? C, int? D, int? E, int? F, int? G, int? H, int? I, int? J, int? K, int? L, int? M, int? N)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            string[] vs = {
//                             COA_Mapper_ModuleTypes.Plot_Sales_Commission_Expense.ToString(),
//                             COA_Mapper_ModuleTypes.Fines_And_Penalties.ToString(),
//                             COA_Mapper_ModuleTypes.Reinstate_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Posession_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Duplicate_Allotment_Letter.ToString(),
//                             COA_Mapper_ModuleTypes.Registry_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Power_Of_Attorney.ToString(),
//                             COA_Mapper_ModuleTypes.Out_Station_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Other_Recovery.ToString(),
//                             COA_Mapper_ModuleTypes.Duplicate_Customer_File.ToString(),
//                             COA_Mapper_ModuleTypes.Other_Transfer_Charges.ToString(),
//                             COA_Mapper_ModuleTypes.Advance_Tax_236_C.ToString(),
//                             COA_Mapper_ModuleTypes.Advance_Tax_236_K.ToString(),
//                             COA_Mapper_ModuleTypes.New_Connection_Charges.ToString()
//                        };
//            var res = db.COA_Mapper.Where(x =>
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.CompanyId == comp.Id &&
//                        x.Module == COA_Mapper_Modules.Files_Plots.ToString() &&
//                        vs.Contains(x.ModuleType)
//                        ).ToList();

//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for Plot_Sales_Commission_Expense
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Plot_Sales_Commission_Expense.ToString(), null, A, userid);
//            // for Fines_And_Penalties
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Fines_And_Penalties.ToString(), null, B, userid);
//            // for Reinstate_Charges 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Reinstate_Charges.ToString(), null, C, userid);
//            // for Posession_Charges 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Posession_Charges.ToString(), null, D, userid);
//            // for Duplicate_Allotment_Letter 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Duplicate_Allotment_Letter.ToString(), null, E, userid);
//            // for Registry_Charges 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Registry_Charges.ToString(), null, F, userid);
//            // for Power_Of_Attorney 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Power_Of_Attorney.ToString(), null, G, userid);
//            // for Out_Station_Charges 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Out_Station_Charges.ToString(), null, H, userid);
//            // for Other_Recovery 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Other_Recovery.ToString(), null, I, userid);
//            // for Duplicate_Customer_File 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Duplicate_Customer_File.ToString(), null, J, userid);
//            // for Other_Transfer_Charges 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Other_Transfer_Charges.ToString(), null, K, userid);
//            // for Advance_Tax_236_C 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Advance_Tax_236_C.ToString(), null, L, userid);
//            // for Advance_Tax_236_K 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.Advance_Tax_236_K.ToString(), null, M, userid);
//            // for New_Connection_Charges 
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(), COA_Mapper_Modules.Files_Plots.ToString(), COA_Mapper_ModuleTypes.New_Connection_Charges.ToString(), null, N, userid);

//            return Json(true);
//        }

//        ////////////////////////////

//        public ActionResult ServiceElectricMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.Sp_Get_COAMapper_Blocks_SC(comp.Id).ToList();
//            var res2 = db.Sp_Get_COAMapper_Blocks_EC(comp.Id).ToList();

//            var res3 = db.Sp_Get_COAMapper_Blocks_SC_Income(comp.Id).ToList();
//            var res4 = db.Sp_Get_COAMapper_Blocks_EC_Income(comp.Id).ToList();

//            return PartialView(new ServiceElectricMappers { ServiceCharges_Res = res1, Electric_Res = res2, Electric_Incom = res4, ServiceCharges_Incom = res3 });
//        }
//        public JsonResult UpdateSC_ResMapper(int? Block, int? Acc_Id, string Type)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Files_Plots.ToString() &&
//                        x.ModuleType == Type &&
//                        x.Module_Id == Block
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Files_Plots.ToString(),
//                                           Type,
//                                           Block,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }

//        public JsonResult UpdateEC_ResMapper(int? Block, int? Acc_Id, string Type)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Files_Plots.ToString() &&
//                        x.ModuleType == Type &&
//                        x.Module_Id == Block
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Files_Plots.ToString(),
//                                           Type,
//                                           Block,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }

//        public JsonResult UpdateSC_IncMapper(int? Block, int? Acc_Id, string Type)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Files_Plots.ToString() &&
//                        x.ModuleType == Type &&
//                        x.Module_Id == Block
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Files_Plots.ToString(),
//                                           Type,
//                                           Block,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }

//        public JsonResult UpdateEC_IncMapper(int? Block, int? Acc_Id, string Type)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Files_Plots.ToString() &&
//                        x.ModuleType == Type &&
//                        x.Module_Id == Block
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Files_Plots.ToString(),
//                                           Type,
//                                           Block,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }

//        /////////////////////////////
//        public ActionResult VendorMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.Sp_Get_COAMapper_Vendor(comp.Id).ToList();

//            return PartialView(res1);
//        }
//        public JsonResult UpdateVendorMapper(int? Vendor, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Procurement.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Vendors_List.ToString() &&
//                        x.Module_Id == Vendor
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Procurement.ToString(),
//                                           COA_Mapper_ModuleTypes.Vendors_List.ToString(),
//                                           Vendor,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }

//        /////////////////////////////
//        public ActionResult ItemMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.Sp_Get_COAMapper_ItemMapping().ToList();

//            return PartialView(res1);
//        }
//        public JsonResult UpdateItemMapper(int? Vendor, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Procurement.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Vendors_List.ToString() &&
//                        x.Module_Id == Vendor
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Procurement.ToString(),
//                                           COA_Mapper_ModuleTypes.Vendors_List.ToString(),
//                                           Vendor,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//        /// //////////////////////////////////////////////////
//        public ActionResult PrjMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.Sp_Get_COAMapper_Prj(comp.Id).ToList();

//            return PartialView(res1);
//        }
//        public JsonResult UpdatePrjMapper(int? Prj, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Projects.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Projects_List.ToString() &&
//                        x.Module_Id == Prj
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }
//            if (Acc_Id == -1)
//            {
//                return Json(true);

//            }

//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Projects.ToString(),
//                                           COA_Mapper_ModuleTypes.Projects_List.ToString(),
//                                           Prj,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//		public JsonResult UpdateBankMapper(int? BankId, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Accounting.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Bank_Account.ToString() &&
//                        x.Module_Id == BankId
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }

//            // for Bank Account Mapping
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Accounting.ToString(),
//                                           COA_Mapper_ModuleTypes.Bank_Account.ToString(),
//                                           BankId,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//		 public ActionResult HRMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            string[] vs = {
//                            COA_Mapper_ModuleTypes.Salary_AdvanceDeduction.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_Allowance.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_BasicSalary.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_Bonus.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_ExtraFuelCharges.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_LoanDeduction.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_OtherDeductions.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_Overtime.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_TaxDeduction.ToString()
//                           };
//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.Module == COA_Mapper_Modules.Human_Resource.ToString() &&
//                        vs.Contains(x.ModuleType)
//                        ).ToList();
//            return PartialView(res);
//        }
//        public JsonResult UpdateHRMapper(int? NetSalary, int? BasicSalary, int? Overtime, int? Bonus, int? Allowance, int? TaxDeduction, int? LoanDeduction, int? AdvanceDeduction, int? OtherDeduction, int? ExtraFuel)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            string[] vs = {
//                            COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_BasicSalary.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_Allowance.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_Bonus.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_Overtime.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_OtherDeductions.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_AdvanceDeduction.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_LoanDeduction.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_TaxDeduction.ToString(),
//                            COA_Mapper_ModuleTypes.Salary_ExtraFuelCharges.ToString()
//                           };
//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.Module == COA_Mapper_Modules.Human_Resource.ToString() &&
//                        vs.Contains(x.ModuleType)
//                        ).ToList();
//            if (res != null)
//            {
//                db.COA_Mapper.RemoveRange(res);
//                db.SaveChanges();
//            }

//            // for Net Salary
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(),
//                                           null,
//                                           NetSalary,
//                                           userid);
//            // for Basic Salary
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_BasicSalary.ToString(),
//                                           null,
//                                           BasicSalary,
//                                           userid);
//            // for Allowance
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_Allowance.ToString(),
//                                           null,
//                                           Allowance,
//                                           userid);
//            // for Bonus
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_Bonus.ToString(),
//                                           null,
//                                           Bonus,
//                                           userid);
//            // for Overtime
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_Overtime.ToString(),
//                                           null,
//                                           Overtime,
//                                           userid);
//            // for Advance Deduction
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_AdvanceDeduction.ToString(),
//                                           null,
//                                           AdvanceDeduction,
//                                           userid);
//            // for Loan Deduction
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_LoanDeduction.ToString(),
//                                           null,
//                                           LoanDeduction,
//                                           userid);
//            // for Tax deduction
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_TaxDeduction.ToString(),
//                                           null,
//                                           TaxDeduction,
//                                           userid);
//            // for Other Deduction
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_OtherDeductions.ToString(),
//                                           null,
//                                           OtherDeduction,
//                                           userid);
//            // for Extra Fuel
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Human_Resource.ToString(),
//                                           COA_Mapper_ModuleTypes.Salary_ExtraFuelCharges.ToString(),
//                                           null,
//                                           ExtraFuel,
//                                           userid);

//            return Json(true);
//        }

//        ////////////////////////
//        /// /////////////////////////////
//        public ActionResult OnlineBankMapper()
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res1 = db.Sp_Get_COAMapper_OnlineBank(comp.Id).ToList();

//            return PartialView(res1);
//        }
//        public JsonResult UpdateBankOnlineMapper(int? bankId, int? Acc_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            AccountHandlerController ah = new AccountHandlerController();
//            var comp = ah.Company_Attr(userid);

//            var res = db.COA_Mapper.Where(x =>
//                        x.CompanyId == comp.Id &&
//                        x.AccountType == COA_Mapper_HeadType.Transaction_Head.ToString() &&
//                        x.Module == COA_Mapper_Modules.Accounting.ToString() &&
//                        x.ModuleType == COA_Mapper_ModuleTypes.Online.ToString() &&
//                        x.Module_Id == bankId
//                        ).FirstOrDefault();
//            if (res != null)
//            {
//                db.COA_Mapper.Remove(res);
//                db.SaveChanges();
//            }
//            // for General Payable
//            ah.AddCOA_Mapper(COA_Mapper_HeadType.Transaction_Head.ToString(),
//                                           COA_Mapper_Modules.Accounting.ToString(),
//                                           COA_Mapper_ModuleTypes.Online.ToString(),
//                                           bankId,
//                                           Acc_Id,
//                                           userid);

//            return Json(true);
//        }
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}