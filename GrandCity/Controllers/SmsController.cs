using MeherEstateDevelopers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class SmsController : Controller
    {
        // GET: Sms
        Grand_CityEntities db = new Grand_CityEntities();

        //public JsonResult Fourth()
        //{
        //    var date = new DateTime(2022, 10, 29);
        //    var res = db.BallotPlots.Where(x => (x.Status == "Balloted") && x.BallotedOn == date).ToList();

        //    foreach (var v in res)
        //    {
        //        var mobileNo = db.Files_Transfer.Where(x => x.Group_Tag == v.Owner_Id).ToList();
        //        string msg = "Dear Customer,\n" +
        //                "Congratulations!\n" +
        //                "You have qualified for the SherAfghan ballot. Plot number allotted to you is " + v.PlotNo + " - " + v.Sector + " plot. " +
        //                "Kindly visit our office Kala Shah Kaku - Phase 2 and collect your ballot letter.\n" +
        //                "Best Regards,\n" +
        //                "Meher Estate Developers.\n" +
        //                "042 - 111 - 724 - 786";
        //        foreach (var item in mobileNo)
        //        {
        //            try
        //            {
        //                SmsService smsService = new SmsService();
        //                smsService.SendMsg(msg, item.Mobile_1);
        //            }
        //            catch (Exception ee)
        //            {
        //                continue;
        //            }
        //        }
        //        var res3 = db.Sp_Add_FileComments(v.FileId, msg, 0, "Text");
        //    }
        //    return Json(true);
        //}

        public ActionResult Text()
        {
            ViewBag.Block1 = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }
        public ActionResult CreateDraft(long? group_tag)
        {
             
            Helpers h = new Helpers();
            ViewBag.trans = h.RandomNumber();
            var res = db.Sms_Draft.Where(x=>x.Group_Tag == group_tag).ToList();

            return PartialView(res);
        }
        public ActionResult DisplayDraft()
        {
            var res = db.Sms_Draft.ToList();
            return PartialView(res);
        }
        public JsonResult SaveDraft(List<MsgModel> msg,long group, long? oldgroup)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            foreach (var row in msg)
            {
                if (oldgroup != null)
                {
                    db.Sp_Update_Draft(group, oldgroup, row.line, row.text, row.data, userid, DateTime.Now);
                }
                else
                {
                    db.Sp_Add_Draft(group, row.line, row.text, row.data, userid, DateTime.Now);
                } 
            }
            return Json(true);
        }
        public JsonResult TextSmsFInal(string star, string broadcast,string target , long? Block, string blkmsgType,long? installmnet,string plt_type,long? file_plot,long? dealer,long grouptag,string filenum)
        {
            var count = 0;
            long userid = long.Parse(User.Identity.GetUserId());
            if (star == "Broadcast")
            {
                if (broadcast == "Customers")
                {
                    var file = db.Files_Transfer.ToList();
                    var plot = db.Plot_Ownership.ToList();
                    var com = db.Commercial_Room_Transfer.ToList();

                    foreach (var dea in file)
                    {
                        var msgtext = CreateTextMessage(grouptag, dea.Name, null,null);
                        try
                        {
                            count++;
                            //SmsService smsService = new SmsService();
                            //smsService.SendMsg(msgtext, dea.Mobile_1);
                            //db.Sp_Add_FileComments(dea.Id, msgtext, userid, "Text");
                        }
                        catch (Exception ee)
                        {
                            
                        }
                    }
                    foreach (var dea in plot)
                    {
                        var msgtext = CreateTextMessage(grouptag, dea.Name, null, null);
                        try
                        {
                            count++;
                            //SmsService smsService = new SmsService();
                            //smsService.SendMsg(msgtext, dea.Mobile_1);
                            //db.Sp_Add_PlotComments(dea.Id, msgtext, userid, "Text");
                        }
                        catch (Exception ee)
                        {
                            
                        }
                    }
                    foreach (var dea in com)
                    {
                        var msgtext = CreateTextMessage(grouptag, dea.Name, null, null);
                        try
                        {
                            count++;
                            //SmsService smsService = new SmsService();
                            //smsService.SendMsg(msgtext, dea.Mobile_1);
                            //db.Sp_Add_CommercialComments(dea.Id, msgtext, userid, "Text");
                        }
                        catch (Exception ee)
                        {
                            
                        }
                    }

                }
                else if (broadcast == "Dealers")
                {
                    var res = db.Dealers.ToList();
                    foreach (var dea in res)
                    {
                        var msgtext = CreateTextMessage(grouptag, dea.Name, null, null);
                        try
                        {
                            count++;
                            //SmsService smsService = new SmsService();
                            //smsService.SendMsg(msgtext, dea.Mobile_1);
                            //db.Sp_Add_DealershipComments(dea.Id, msgtext, userid, "Text");
                        }
                        catch (Exception ee)
                        {
                            
                        }
                    }
                    
                }
            }
            else if (star == "Target")
            {
                if (target == "Block")
                {
                    //block 
                    if (blkmsgType == "Anouncement SMS to All Block Members")
                    {
                        //Block use Karna hai
                        var res = db.Plots.Where(x => x.Status == "Registered" && x.Block_Id == Block).Select(x => x.Id).ToList();
                        foreach (var item in res)
                        {
                            var res1 = db.Sp_Get_PlotData(item).FirstOrDefault();
                            var res2 = db.Sp_Get_PlotLastOwner(item).FirstOrDefault();
                            try
                            {
                                    var pltblk = res1.Plot_No + " - " + res1.Block_Name;
                                    var msgtext = CreateTextMessage(grouptag, res2.Name, null, pltblk);
                                    try
                                    {
                                     count++;
                                    SmsService smsService = new SmsService();
                                    smsService.Broadcast(msgtext, res2.Mobile_1);
                                }
                                    catch (Exception ee)
                                    {
                                    }
                                
                            }
                            catch (Exception eee)
                            {
                            }
                        }


                        var Fileres = db.File_Form.Where(x => x.Status == 1 && x.Block_Id == Block).ToList();
                        foreach (var item in Fileres)
                        {
                            var res1 = db.Sp_Get_FileData(item.Id).FirstOrDefault();
                            var res2 = db.Sp_Get_FileLastOwner(res1.Id).ToList();
                            foreach (var own in res2)
                            {
                                try
                                {
                                    var pltblk = res1.FileFormNumber + " - " + res1.Block;
                                    
                                    var msgtext = CreateTextMessage(grouptag, own.Name, null, pltblk);
                                    try
                                    {
                                        count++;
                                        //SmsService smsService = new SmsService();
                                        //smsService.SendMsg(msgtext,own.Mobile_1 );
                                        //db.Sp_Add_FileComments(res1.Id, msgtext, userid, "Text");
                                    }
                                    catch (Exception ee)
                                    {
                                    }

                                }
                                catch (Exception eee)
                                {
                                    
                                    
                                }
                            }       
                        }
                    }
                    else if (blkmsgType == "Overdue Amount SMS")////
                    {
                        //installments use krna hai 
                        var res = db.Sp_Get_WarningSms().Where(x=>x.BlockId == Block).ToList();
                        foreach (var item in res)
                        {
                            FileSystemController f = new FileSystemController();
                            try
                            {
                                f.TestAdjustIntallments(item.FileFormNumber);
                                Sp_Get_CurrentOwner_File_Id_Result res1 = db.Sp_Get_CurrentOwner_File_Id(item.Id).FirstOrDefault();

                                if (item.Installments == installmnet)
                                {
                                    var amt = string.Format("{0:n0}", (res1.Balance_Amount * -1));
                                    var msgtext = CreateTextMessage(grouptag, res1.Name, amt, res1.FileFormNumber.ToString());
                                    try
                                    {
                                        count++;
                                        //SmsService smsService = new SmsService();
                                        //smsService.SendMsg(msgtext, item.Mobile_1);
                                        //db.Sp_Add_FileComments(item.Id, msgtext, userid, "Text");
                                    }
                                    catch (Exception ee)
                                    {
                                        
                                        
                                    }
                                }
                            }
                            catch (Exception eee)
                            {
                                
                                
                            }
                        }
                    }
                    else if (blkmsgType == "Due Amount SMS")
                    {
                        var res = db.Plots.Where(x => x.Status == "Registered" && x.Block_Id == Block).Select(x => x.Id).ToList();
                        foreach (var item in res)
                        {
                            var res1 = db.Sp_Get_PlotData(item).FirstOrDefault();
                            var res2 = db.Sp_Get_PlotLastOwner(item).FirstOrDefault();
                            var res3 = db.Sp_Get_PlotInstallments(item).ToList();
                            var res4 = db.Sp_Get_ReceivedAmounts(item, Modules.PlotManagement.ToString()).ToList();
                            var discount = db.Discounts.Where(x => x.Module_Id == item && x.Plot_Is_Cancelled == null && x.Module == Modules.PlotManagement.ToString()).ToList();

                            PlotsController p = new PlotsController();
                            p.UpdatePlotInstallmentStatus(res3, res4, discount, item);

                            try
                            {
                                var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == item && x.Module == Modules.PlotManagement.ToString()).FirstOrDefault();
                                bal.Balance_Amount = Math.Round(bal.Balance_Amount);
                                if (bal.Balance_Amount < 0)
                                {
                                    var amt = string.Format("{0:n0}", (bal.Balance_Amount * -1));
                                    var pltblk = res1.Plot_No + " - " + res1.Block_Name;
                                    var msgtext = CreateTextMessage(grouptag, res2.Name, amt, pltblk);
                                    try
                                    {
                                        count++;
                                        //SmsService smsService = new SmsService();
                                        //smsService.SendMsg(msgtext, res2.Mobile_1);
                                        //db.Sp_Add_PlotComments(res1.Id, msgtext, userid, "Text");
                                    }
                                    catch (Exception ee)
                                    {
                                        
                                    }
                                }
                            }
                            catch (Exception eee)
                            {
                                
                            }
                        }
                        var ress = db.File_Form.Where(x => x.Status == 1 && x.Block_Id == Block).Select(x => x.Id).ToList();
                        foreach (var item in res)
                        {
                            var res1 = db.Sp_Get_FileFormData(item).FirstOrDefault();
                            var res2 = db.Sp_Get_FileLastOwner(item).FirstOrDefault();
                            var res3 = db.Sp_Get_FileInstallments(item).ToList();
                            var res4 = db.Sp_Get_ReceivedAmounts(item, Modules.FileManagement.ToString()).ToList();
                            var discount = db.Discounts.Where(x => x.Module_Id == item && x.Module == Modules.FileManagement.ToString()).ToList();

                            FileSystemController p = new FileSystemController();
                            p.TestAdjustIntallments(item);

                            try
                            {
                                var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == item && x.Module == Modules.FileManagement.ToString()).FirstOrDefault();
                                bal.Balance_Amount = Math.Round(bal.Balance_Amount);
                                if (bal.Balance_Amount < 0)
                                {
                                    var amt = string.Format("{0:n0}", (bal.Balance_Amount * -1));
                                    var pltblk = res1.FileFormNumber + " - " + res1.Block;
                                    var msgtext = CreateTextMessage(grouptag, res2.Name, amt, pltblk);
                                    try
                                    {
                                        count++;
                                        //SmsService smsService = new SmsService();
                                        //smsService.SendMsg(msgtext, res2.Mobile_1);
                                        //db.Sp_Add_FileComments(res1.Id, msgtext, userid, "Text");
                                    }
                                    catch (Exception ee)
                                    {
                                        
                                    }
                                }
                            }
                            catch (Exception eee)
                            {
                                
                            }
                        }
                    }
                }
                else if (target == "indivi")
                {
                    if (plt_type == "Residential")
                    {
                        //file_plt use krna
                        //block be 

                       
                    }
                    else if(plt_type == "Commercial")
                    {
                        //file_plt use krna
                        //block be 
                    }
                }
                else if (target == "dealer")
                {
                    //dearer use krna hai
                    var res = db.Dealers.Where(x=>x.Id == dealer).ToList();
                    foreach (var dea in res)
                    {
                        var msgtext = CreateTextMessage(grouptag, dea.Name, null, null);
                        try
                        {
                            count++;
                            //SmsService smsService = new SmsService();
                            //smsService.SendMsg(msgtext, dea.Mobile_1);
                            //db.Sp_Add_DealershipComments(dea.Id, msgtext, userid, "Text");
                        }
                        catch (Exception ee)
                        {
                            
                        }
                    }
                }
            }


            return Json(count);
        }
        //public JsonResult SaveDraft(List<MsgModel> msg,long group)
        //{
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    foreach (var row in msg)
        //    {
        //        db.Sp_Add_Draft(group, row.line, row.text, row.data, userid, DateTime.Now);
        //    }
        //    return Json(true);
        //}
        public JsonResult sendtext(string text, long group_tag)
        {
            var msgtext = CreateTextMessage(group_tag, "Muhammad Ehtisham", "12000", "1234566");
            SmsService smsService = new SmsService();
            smsService.SendMsg(msgtext, "03137317253");
            return Json(true);
        }
        public string CreateTextMessage(long group_tag, string Name, string Amount, string File_Plot_Number)
        {
            var res = db.Sms_Draft.Where(x => x.Group_Tag == group_tag).ToList();
            var msgtext = "";
            foreach (var row in res.OrderBy(x => x.Line))
            {
                if (row.Msg_Text != null)
                {
                    msgtext = msgtext + row.Msg_Text;
                }
                if (row.Msg_Data != null)
                {
                    msgtext = msgtext + " ";
                    if (row.Msg_Data == "CLIENT_NAME")
                    {
                        msgtext = msgtext + Name;
                    }
                    else if (row.Msg_Data == "DUE_AMOUNT")
                    {
                        msgtext = msgtext + Amount;
                    }
                    else if (row.Msg_Data == "FILE_PLOT_NUMBER")
                    {
                        msgtext = msgtext + File_Plot_Number;
                    }
                    msgtext = msgtext + " ";
                }
            }
            return msgtext;
        }
        //public void Eid()
        //{
        //    string a = "Dear Customer,\n" +
        //                "Meher Estate Developers proudly presents a new sector of Badar Block, Sector E. Very limited plots are available in Sector E. 5 and 10 Marla plots are available on easy installment plan, with installment starting from PKR 21,000.\n" +
        //                "For further details, please visit our Sales offices or contact us at 042-111 724 786.\n" +
        //                "Best Regards,\n" +
        //                "Meher Estate Developers";

        //    var file = db.Files_Transfer.Select(x => x.Mobile_1).ToList();
        //    var plot = db.Plot_Ownership.Select(x => x.Mobile_1).ToList();
        //    var com = db.Commercial_Room_Transfer.Select(x => x.Mobile_1).ToList();

        //    file.AddRange(plot);
        //    file.AddRange(com);
        //    file = file.Distinct().ToList();
        //    foreach (var dea in file)
        //    {
        //        try
        //        {
        //            SmsService smsService = new SmsService();
        //            smsService.Broadcast(a, dea);
        //        }
        //        catch (Exception ee)
        //        {
        //        }
        //    }
        //}

        /// <summary>
        /// ///////////////////////////////////
        /// </summary>
        /// <returns></returns>


    }
}


//        public JsonResult SecondBallot()
//        {
//            var res = db.BallotPlots.Where(x => (x.Status == "Balloted") && (x.Sector == "AA" || x.Sector == "BB" || x.Sector == "CC") && (x.PlotType != "Normal")).ToList();

//            foreach (var v in res)
//            {
//                var amount = db.File_Installments.Where(x => x.File_Id == v.FileId && x.Installment_Type == "5").Select(x=> x.Amount).FirstOrDefault();
//                var mobileNo = db.Files_Transfer.Where(x => x.Group_Tag == v.Owner_Id).ToList();
//                string msg = "Dear Customer,\n" +
//                        "Congratulations!\n" +
//                        "Your file has been successfully balloted and you have been allotted a "+ v.PlotType +" plot. Preference charges levied on your plot # " + v.PlotNo + v.Sector + " would be PKR " + String.Format("{0:n0}", amount) + "/-.\n" +
//                        "You are requested to pay the due amount by 30th September 2021 otherwise you will be relocated to normal plot.\n" +
//                        "Best Regards,\n" +
//                        "Meher Estate Developers.\n" +
//                        "042 - 111 - 724 - 786";
//                foreach (var item in mobileNo)
//                {
//                    try
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(msg, item.Mobile_1);
//                    }
//                    catch (Exception ee)
//                    {
//                        continue;
//                    }
//                }
//                var res3 = db.Sp_Add_FileComments(v.FileId, msg, 0, "Text");
//            }
//            return Json(true);
//        }
//        /// <summary>
//        /// ////////////////////////////////////////////////////////////////////////////////////////
//        /// </summary>
//        /// <returns></returns>
//        public void msg()
//        {
//            string a = "Dear Customer,\n" +
//                     "Meher Estate Developers presents you a golden investment opportunity. Now get your own 5 Marla plot in Badar D Block with immediate possession for as low as Rs. 20 lakh with No development charges and No other hidden costs.\n" +
//                     "For further details call us now at 042 - 111 724 786 or email us at info@meharestate.com\n" +
//                     "Best Regards,\n" +
//                     "Meher Estate Developers.";
//            try
//            {
//                SmsService smsService = new SmsService();
//                smsService.Broadcast(a, "03158444530");
//            }
//            catch (Exception ex)
//            {
//                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//            }


//        }

//        public JsonResult DealerSms()
//        {
//            string[] a = {"A S Estate Advisor ",
//"AAM Estate Advisors",
//"Ahsan State Advisor & Developer  ",
//"Aim Real Estate",
//"Aitashm Estate",
//"AJ Global Pvt Ltd ",
//"AJB Associates",
//"Ajwa Estate",
//"Al Ghafoor Gee Estate",
//"Al Hafiz Estate",
//"Al Hammad Estate",
//"Al Haram State",
//"Al Mannan Estate",
//"Al Meezan Estate Advisor",
//"Al Mustafa Estate & Developers",
//"Al Noor Estate",
//"Al Raheem Estate",
//"Al Raziq Estate",
//"Al Rehan Estate Develpers",
//"Al Sadiq Estate",
//"Al-Hareem Estate",
//"Ali Haider Estate",
//"Aswad Estate",
//"Best Buy Real Estate",
//"Bhatti Brothers Real Estate Consultants",
//"Ejaz Estate",
//"Etamad Estate & Builders",
//"Global Business Holdings",
//"Good Will Property ",
//"Green Earth Associate ",
//"Hafiz Gee Real Estate & Builders",
//"Jag Associates",
//"Khan Estate",
//"Lahore Estate",
//"Land Exchange Associates",
//"Lucky Estate",
//"M M Estate",
//"Madina Estate",
//"Malik Estate",
//"Master Mind Real Estate",
//"Mian Estate",
//"Millat Estate & Builders",
//"Mughal Estate",
//"Noorani Estate",
//"Pak Estate & Builders",
//"Plots & Plans",
//"Punjab Estate",
//"Rabbani Estate ",
//"Rana Asif Estate Advisor",
//"Rehman Estate",
//"S.A Marketing",
//"Saiban Associate",
//"Sallar Estate",
//"Seven Estate Advisors",
//"Shahen-Shah-E-Fazay Lasani",
//"Shoaib Estate & Builders",
//"Sks Interprizes Pvt. Ltd",
//"Sky Limit Marketing",
//"Wall Real Estate",
//"Waseela Developers",
//"Women Empowerment",
//"Zam Zam Estate",
//"Rai Estate & Developers",
//"Grey Stone Estate",
//"Mohsin Estate",
//"Four Brothers Real Estate",
//"Ali Real Estate",
//"ESSA Estate & Developer",
//"Al Ahad",
//"Al Hafeez",
//"Data Ali Hajveri Estate",
//"Green Land Estate",
//"Heaven Land Estate & Developers",
//"Al Wakeel Estate Advisors" };

//            string msg = "Dear Business Partners,\n" +
//                   "We are pleased to announce the launch of SA Premium Homes. The project consists of 5 Marla G+2 townhouses in our Premium Block..\n" +
//                   "All Authorized Partners are cordially invited to the launch event. Event details are mentioned below:.\n" +
//                   "Venue: Pearl Continental Lahore - Crystal Hall.\n" +
//                   "Timmings: 1PM - 4PM\n" +
//                   "Date: 4th August, 2021,\n" +
//                   "Looking forward to see you,\n" +
//                   "Regards \r\n"+
//                   "Meher Estate Developers.";
//            List<string> no = new List<string>();
//            foreach (var item in a)
//            {
//                var b = db.Dealerships.Where(x => x.Dealership_Name == item).FirstOrDefault();
//                var c = db.Dealers.Where(x => x.Dealership_Id == b.Id).ToList();

//                foreach (var g in c)
//                {
//                    try
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(msg, g.Mobile_1);
//                    }
//                    catch (Exception ee)
//                    {
//                        no.Add(item);
//                    }
//                }
//            }

//            return Json(no, JsonRequestBehavior.AllowGet);
//        }

//        public void Eid()
//        {
//            string a = "Dear Customer,\n" +
//                        "May the divine blessings of Allah bring you hope, faith and joy on Eid-ul-Adha and forever.\n" +
//                        "Our Head Office will remain operational on following next days for submission of instalments and sales i.e.\n" +
//                        "20th July - 22nd July - 23rd July.\n" +
//                        "9:00 AM - 6:00 PM\n" +
//                        "Best Regards,\n" +
//                        "Meher Estate Developers.\n" +
//                        "042 - 111 - 724 - 786";

//            var file = db.Files_Transfer.Select(x => x.Mobile_1).ToList();
//            var plot = db.Plot_Ownership.Select(x => x.Mobile_1).ToList();
//            var com = db.Commercial_Room_Transfer.Select(x => x.Mobile_1).ToList();

//            file.AddRange(plot);
//            file.AddRange(com);
//            //file = file.ForEach(x => x.Replace("-", ""));
//            file = file.Distinct().ToList();
//            foreach (var dea in file)
//            {
//                try
//                {
//                    SmsService smsService = new SmsService();
//                    smsService.Broadcast(a, dea);
//                }
//                catch (Exception ee)
//                {
//                }
//            }
//        }

//        public void SherAfghanBalloting()
//        {
//            var msg = "Dear Customer,\n" +
//                        "وعدے کی تکمیل کا سفر جاری\n\n" +
//                        "As part of our commitment to you, and with immense pride, Meher Estate Developers would like to announce the Online Balloting Event for Sher Afghan Block on 23rd August 2021 - Monday at 12:00 PM. \n\n" +
//                        "You can watch the virtual balloting take place at our official Meher Estate Developers Facebook page.\n\n" +
//                        "www.facebook.com/meherestatedevelopers\n\n" +
//                        "Best Regards,\n" +
//                        "Meher Estate Developers.\n" +
//                        "042 - 111 - 724 - 786";
//            var res = db.Sp_Get_Ballot_Qualifying_File().ToList();
//            foreach (var v in res)
//            {
//                string[] words = v.Mobile_1.Split(',');
//                foreach (var item in words)
//                {
//                    try
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(msg, item.Trim());
//                    }
//                    catch (Exception e)
//                    {
//                        continue;
//                    }
//                }
//                var res3 = db.Sp_Add_FileComments(v.Id, msg, 0, "Text");
//            }
//        }


//        public JsonResult SendCommercialSMS()
//        {
//            var res1 = db.Sp_Get_RegisterFileDueAmount().Select(x=> new { x.Mobile_1 }).Distinct().ToList();


//            foreach (var item in res1)
//            {
//                try
//                {
//                    var msgtext = "Respected Customer ,\n\r" +
//                    "It is with great pleasure to inform you that Sher Afghan block commercial plots are now available on installments for pre-booking. \n\r" +
//                    "\n\r" +
//                    "For more details, kindly visit our offices  or contact our helpline. \n\r" +
//                    "\n\r" +
//                    "Best Regards,\n\r" +
//                    "Meher Estate Developers.\n\r" +
//                    "042-111 724 786";

//                    try
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(msgtext, item.Mobile_1);

//                        //if (!string.IsNullOrEmpty(item.Mobile_2) || !string.IsNullOrWhiteSpace(item.Mobile_2))
//                        //{
//                        //    smsService.SendMsg(msgtext, item.Mobile_2);
//                        //}
//                    }
//                    catch (Exception e)
//                    {
//                        continue;
//                    }
//                }
//                catch (Exception ee)
//                {
//                    continue;
//                }

//            }
//            return Json(true);
//        }

//        public JsonResult DaniyalNoTest()
//        {
//            //var res1 = db.Sp_Get_RegisterFileDueAmount().ToList();
//            var res1 = new List<Sp_Get_RegisterFileDueAmount_Result>();
//            res1.Add(new Sp_Get_RegisterFileDueAmount_Result
//            {
//                Name = "Daniyal Humayun",
//                Mobile_1 = "03231611324",
//                Mobile_2 = "03231611324"
//            });

//            foreach (var item in res1)
//            {
//                try
//                {
//                    //FileSystemController f = new FileSystemController();
//                    //f.TestAdjustIntallments(item.FileFormNumber);
//                    //Sp_Get_CurrentOwner_File_Id_Result res2 = db.Sp_Get_CurrentOwner_File_Id(item.Id).FirstOrDefault();

//                    //var msgtext = "Dear " + res2.Name + ", \n\r" +
//                    // "Congratulations. Your Plot No. " + res2.FileFormNumber + " has successfully qualified for the first round of balloting for Sher Afghan block. You are cordially invited to our Live Ballot event scheduled to be held on 15th November, 2020 at Phase 2, Meher Estate Developers.\n\r" +
//                    // "For further information call at our help line.\n\r" +
//                    // "Meher Estate Developers\n\r" +
//                    // "042 - 111 724 786";
//                    var msgtext = "Dear " + item.Name + ",\n\r" +
//                    "Please don't forget to bring your original CNIC with you as entry to the event tomorrow is restricted to it.  Below is the schedule of the event: \n\r" +
//                    "\n\r" +
//                    "4:00 PM  -  Balloting Process \n\r" +
//                    "6:00 PM  -  Musical Performance - Asrar Shah\n\r" +
//                    "7:00 PM  -  Grand Musical Performance - Abida Parveen\n\r" +
//                    "8:00 PM. -  Grand Dinner\n\r" +
//                    "\n\r" +
//                    "Enjoy a special video from our side to you and your family,\n\r" +
//                    "Link to the video https://youtu.be/W18PCnhYAa8 \n\r" +
//                    "Best Regards,\n\r" +
//                    "Meher Estate Developers.";

//                    try
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(msgtext, item.Mobile_1);
//                        //var res3 = db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");

//                        if (!string.IsNullOrEmpty(item.Mobile_2) || !string.IsNullOrWhiteSpace(item.Mobile_2))
//                        {
//                            smsService.SendMsg(msgtext, item.Mobile_2);
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        continue;
//                    }
//                }
//                catch (Exception ee)
//                {
//                    continue;
//                }

//            }
//            return Json(true);
//        }
//        public JsonResult TextSms(string Option1, string Target, long Block, string Type, long Plot_Id, string Text)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (Option1 == "Invitation")
//            {
//                var res1 = (from element in db.Files_Transfer
//                            group element by element.File_Form_Id
//                            into groups
//                            select groups.OrderByDescending(p => p.Id).FirstOrDefault()).ToList();

//                var res = res1.Select(x => x.Mobile_1).Distinct().ToList();
//                foreach (var item in res)
//                {
//                    SmsService smsService = new SmsService();
//                    smsService.SendMsg(Text, "03158444530");
//                }
//            }
//            else
//            {
//                if (Target == "indivi")
//                {
//                    var plot_details = db.Sp_Get_PlotData(Plot_Id).FirstOrDefault();
//                    var res2 = db.Sp_Add_FileComments(Plot_Id, Text, userid, "Text");
//                    SmsService smsService = new SmsService();
//                    smsService.SendMsg(Text, "03158444530");
//                }
//                else
//                {
//                    var res1 = (from x in db.Plots
//                                join element in db.Plot_Ownership on x.Id equals element.Plot_Id
//                                where x.Block_Id == Block
//                                group element by new { element.Plot_Id, x.Block_Id }
//                                into groups
//                                select groups.OrderByDescending(p => p.Id).FirstOrDefault()).ToList();

//                    var res = res1.Select(x => x.Mobile_1).Distinct().ToList();
//                    foreach (var item in res)
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(Text, "03158444530");
//                    }
//                }
//            }



//            return Json(true);
//        }

//        //public void SecondWarningMsg()
//        //{
//        //    var res = db.Sp_Get_SecWarning_File(null, null, null, null, null, null, null, null, null).ToList();

//        //    foreach (var item in res)
//        //    {
//        //        var msgtext = "Dear " + item.Name + "\n\r," +
//        //               "We shall be soon announcing balloting of Sher Afgan Block. Only those clients may qualify who are current with their installments. As your outstanding is Rs ( " + string.Format("{0:n}", Math.Round(Convert.ToDouble(item.Balance_Amount))) + "  ), and you have already been issued 2nd reminder letter, kindly clear your dues at your earliest to avail this opportunity.\n\r" +
//        //               "Meher Estate Developers\n\r" +
//        //               "042 - 111 724 786";
//        //        try
//        //        {
//        //            SmsService smsService = new SmsService();
//        //            smsService.PremiumSub(msgtext, item.Mobile_1);
//        //            db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");
//        //        }
//        //        catch (Exception ee)
//        //        {
//        //            
//        //            e.SendEmail(item.Name + " " + item.FileFormNumber, "taimoor@sasystems.solutions", "Msg Not Sent");
//        //        }
//        //    }
//        //}
//        //public void FirstWarningMsg()
//        //{
//        //    var res = db.Sp_Get_FirstWarning_File(null, null, null, null, null, null, null, null, null).ToList();

//        //    foreach (var item in res)
//        //    {
//        //        var msgtext = "Dear " + item.Name + "\n\r," +
//        //               "We shall be soon announcing balloting of Sher Afgan Block. Only those clients may qualify who are current with their installments. As your outstanding is Rs ( " + string.Format("{0:n}", Math.Round(Convert.ToDouble(item.Balance_Amount))) + "  ), and you have already been issued 1st reminder letter, kindly clear your dues at your earliest to avail this opportunity.\n\r" +
//        //               "Meher Estate Developers\n\r" +
//        //               "042 - 111 724 786";
//        //        try
//        //        {
//        //            SmsService smsService = new SmsService();
//        //            smsService.PremiumSub(msgtext, item.Mobile_1);
//        //            db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");
//        //        }
//        //        catch (Exception ee)
//        //        {
//        //            
//        //            e.SendEmail(item.Name + " " + item.FileFormNumber, "taimoor@sasystems.solutions", "Msg Not Sent");
//        //        }
//        //    }
//        //}

//        public void WarningMsg()
//        {
//            var res = db.Sp_Get_WarningSms().ToList();
//            foreach (var item in res)
//            {
//                FileSystemController f = new FileSystemController();
//                try
//                {
//                    f.TestAdjustIntallments(item.FileFormNumber);
//                    Sp_Get_CurrentOwner_File_Id_Result res1 = db.Sp_Get_CurrentOwner_File_Id(item.Id).FirstOrDefault();

//                    if (res1.Balance_Amount < 0)
//                    {
//                        var msgtext = "Dear " + res1.Name + ",\n\r" +
//                        "Your Over due amount is PKR " + string.Format("{0:n0}", (res1.Balance_Amount * -1)) + "- against File Number " + res1.FileFormNumber + " . You are requested to clear pending dues to avoid any further inconvenience.\n\r" +
//                        "If you have Already Cleared your dues then ignore this message or Contact our Help Desk Dial UAN 111 724 786";
//                        try
//                        {
//                            SmsService smsService = new SmsService();
//                            smsService.SendMsg(msgtext, item.Mobile_1);
//                            db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");
//                        }
//                        catch (Exception ee)
//                        {
//                            
//                            
//                        }
//                    }
//                }
//                catch (Exception eee)
//                {
//                    
//                    
//                }
//            }
//        }

//        public void WarningPlotMsgs()
//        {
//            var res = db.Plots.Where(x=> x.Status == "Registered").Select(x=> x.Id ).ToList();
//            foreach (var item in res)
//            {
//                var res1 = db.Sp_Get_PlotData(item).FirstOrDefault();
//                var res2 = db.Sp_Get_PlotLastOwner(item).FirstOrDefault();
//                var res3 = db.Sp_Get_PlotInstallments(item).ToList();
//                var res4 = db.Sp_Get_ReceivedAmounts(item, Modules.PlotManagement.ToString()).ToList();
//                var discount = db.Discounts.Where(x => x.Module_Id == item && x.Module == Modules.PlotManagement.ToString()).ToList();

//                PlotsController p = new PlotsController();
//                p.UpdatePlotInstallmentStatus(res3, res4, discount, item);

//                try
//                {
//                    var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == item && x.Module == Modules.PlotManagement.ToString()).FirstOrDefault();
//                    bal.Balance_Amount = Math.Round(bal.Balance_Amount);
//                    if (bal.Balance_Amount < 0)
//                    {
//                        var msgtext = "Dear " + res2.Name + "\n\r," +
//                        "Your Over due amount is PKR " + string.Format("{0:n0}", (bal.Balance_Amount * -1)) + "- against Plot Number " + res1.Plot_No + " - " + res1.Block_Name + " . You are requested to clear pending dues to avoid any further inconvenience.\n\r" +
//                        "If you have Already Cleared your dues then ignore this message or Contact our Help Desk Dial UAN 111 724 786";
//                        try
//                        {
//                            SmsService smsService = new SmsService();
//                            smsService.SendMsg(msgtext, res2.Mobile_1);
//                            db.Sp_Add_PlotComments(res1.Id, msgtext, 0, "Text");
//                        }
//                        catch (Exception ee)
//                        {
//                            
//                            e.SendEmail(res1.Plot_No, "taimoor@sasystems.solutions", "Msg Not Sent");
//                        }
//                    }
//                }
//                catch (Exception eee)
//                {
//                    
//                    e.SendEmail(res1.Plot_No, "taimoor@sasystems.solutions", "Msg Not Sent");
//                }
//            }
//        }


//        //public ActionResult Index()
//        //{
//        //    return View();
//        //}
//        public void TEstSendMsg()
//        {
//            var res1 = (from element in db.Files_Transfer
//                        group element by element.File_Form_Id
//                      into groups
//                        select groups.OrderByDescending(p => p.Id).FirstOrDefault()).ToList();

//            var res = res1.Select(x => x.Mobile_1).Distinct().ToList();
//            foreach (var item in res)
//            {
//                var text = "Dear Customer,\r\n" +
//                       "Eid Mubarak to you and your Family. May Allah (SWT) enriches our lives with happiness, Peace and prosperity.\r\n" +
//                       "Regards,\r\n" +
//                       "S A Gardens.\r\n\r\n";
//                SmsService smsService = new SmsService();
//                smsService.SendMsg(text, item);
//            }

//            var res2 = (from element in db.Plot_Ownership
//                        group element by element.Plot_Id
//                      into groups
//                        select groups.OrderByDescending(p => p.Id).FirstOrDefault()).ToList();

//            var res3 = res2.Select(x => x.Mobile_1).Distinct().ToList();
//            foreach (var item in res3)
//            {
//                var text = "Dear Customer,\r\n" +
//                       "Eid Mubarak to you and your Family. May Allah (SWT) enriches our lives with happiness, Peace & prosperity.\r\n" +
//                       "Regards,\r\n" +
//                       "S A Gardens.\r\n\r\n";
//                SmsService smsService = new SmsService();
//                smsService.SendMsg(text, item);
//            }


//        }
//        //public void TEsts()
//        //{

//        //        var text = "Dear Colleagues,\r\n" +
//        //               "Kindly Share the Following information TODAY with HR Department.\r\n" +
//        //               "1. Full Name.\r\n" +
//        //               "2. Department.\r\n" +
//        //               "We require these details for everone who wants to participate in the SAG Table Tennis Tournament.\r\n" +
//        //               "Table Tennis Tournament will be Held on 4th Feburary 2020.\r\n" +
//        //               "Timing: 4:00 PM .\r\n" +
//        //               "LFO 3rd Floor.\r\n" +
//        //               "Regards,\r\n" +
//        //               "S A Gardens.\r\n\r\n";

//        //    var res = db.Employees.Where(x => x.Status == "Registered" && (x.Mobile_1 != null || x.Mobile_1 != "")).ToList();
//        //    foreach (var item in res)
//        //    {
//        //        SmsService smsService = new SmsService();
//        //        smsService.PremiumSub(text, item.Mobile_1);
//        //    }

//        //}
//        //public void TEstSendMsg1()
//        //{
//        //    var res1 = (from element in db.Files_Transfer
//        //                group element by element.File_Form_Id
//        //              into groups
//        //                select groups.OrderByDescending(p => p.Id).FirstOrDefault()).ToList();
//        //    var res = res1.Select(x => x.Mobile_1).Distinct().ToList();

//        //    foreach (var item in res)
//        //    {
//        //        var text = "Be our guest at the 10th Anniversary celebration of Meher Estate Developers. Do not miss Pakistan's most culturally diversified festival, SAG Fest 2019. Join us on 22nd, 23rd and 24th November 2019 at Meher Estate Developers Phase II, Kala Shah Kaku Lahore."+
//        //            "Activities like Tent Pegging, Snooker Championship, Basketball, Kabaddi, MMA Fight, PMX Race, Jumping Competition ,Marathon Race , Food Stall and Concert are waiting for you.";
//        //        SmsService smsService = new SmsService();
//        //        smsService.PromotionalSub(text, item);
//        //    }
//        //}
//        //public void TEstSendMsg1()
//        //{
//        //    var res1 = (from element in db.Files_Transfer
//        //                group element by element.File_Form_Id
//        //              into groups
//        //                select groups.OrderByDescending(p => p.Id).FirstOrDefault()).ToList();
//        //    var res = res1.Select(x => x.Mobile_1).Distinct().ToList();

//        //    foreach (var item in res)
//        //    {
//        //        var text = "Be our guest at the 10th Anniversary celebration of Meher Estate Developers. Do not miss Pakistan's most culturally diversified festival, SAG Fest 2019. Join us on 22nd, 23rd and 24th November 2019 at Meher Estate Developers Phase II, Kala Shah Kaku Lahore."+
//        //            "Activities like Tent Pegging, Snooker Championship, Basketball, Kabaddi, MMA Fight, PMX Race, Jumping Competition ,Marathon Race , Food Stall and Concert are waiting for you.";
//        //        SmsService smsService = new SmsService();
//        //        smsService.PromotionalSub(text, item);
//        //    }
//        //}
//        //public ActionResult PlotNumberChange()
//        //{
//        //    string[] sectorA = { "335", "352", "339", "205", "32","310","4", "272", "159", "158", "141", "258", "271", "77", "301" };
//        //    string[] sectorC = {
//        //        "148","202","201","37","369","469", "413", "429", "415", "131", "629","384","204","434","27", "32", "255", "206","624","398","259","300","301", "252","251","118","305","602","22","464","612","24","221","558","86","369","247" };

//        //    var res1 = (from x in db.Plots
//        //                where sectorA.Contains(x.Plot_Number) && x.Sector == "A"
//        //                select new PlotSigned { Name = db.Plot_Ownership.Where(a => a.Plot_Id == x.Id).OrderByDescending(a => a.Id).FirstOrDefault().Name, PlotNumber = x.Plot_Number + " A", Old_PlotNumber = x.Old_PlotNumber, Plot_Size = x.Plot_Size }).OrderBy(x => x.PlotNumber).ToList();
//        //    var res2 = (from x in db.Plots
//        //                join y in db.Plot_Ownership on x.Id equals y.Plot_Id
//        //                where sectorC.Contains(x.Plot_Number) && x.Sector == "C"
//        //                select new PlotSigned { Name = db.Plot_Ownership.Where(a => a.Plot_Id == x.Id).OrderByDescending(a => a.Id).FirstOrDefault().Name, PlotNumber = x.Plot_Number + " C", Old_PlotNumber = x.Old_PlotNumber, Plot_Size = x.Plot_Size }).OrderBy(x => x.PlotNumber).ToList();
//        //    List<PlotSigned> plots = new List<PlotSigned>();
//        //    plots.AddRange(res1);
//        //    plots.AddRange(res2);
//        //    return View(plots);
//        //}
//        //public ActionResult SherzamLetter()
//        //{
//        //    string[] plot = {"406","390","339","102","77","74","72","70","420","412","384","334","171","342","45","276","347","392","277","309","407","183","182","81" };
//        //    var res1 = (from x in db.Plots
//        //                where plot.Contains(x.Plot_Number) && x.Block_Id == 11
//        //                select new PlotSigned { Name = db.Plot_Ownership.Where(a => a.Plot_Id == x.Id).OrderByDescending(a => a.Id).FirstOrDefault().Name, PlotNumber = x.Plot_Number, Old_PlotNumber = x.Old_PlotNumber, Plot_Size = x.Plot_Size }).OrderBy(x => x.PlotNumber).ToList();
//        //    return View(res1);
//        //}

//        //public ActionResult PaymentModeSMS()
//        //{
//        //    PaymentModeInformation();
//        //    return Json(true,JsonRequestBehavior.AllowGet);
//        //}


//        public void DealersSMS()
//        {
//            var res = (from x in db.Dealerships
//                       where x.Status == "Registered"
//                       select x).ToList();

//            var html = "Respected Business Partners\n\r" +
//                "Kindly note last date for the booking of Sher Afghan Commercial is 31st March, 2021.\n\r" +
//                "After the mentioned date no forms of Sher Afghan Commercial will be entertained.\n\r" +
//                "Regards,\n\r" +
//                "S A Gardens.";
//            foreach (var b in res)
//            {
//                var re1 = db.Dealers.Where(x => x.Dealership_Id == b.Id).ToList();
//                foreach (var c in re1)
//                {
//                    try
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(html, c.Mobile_1);
//                    }
//                    catch (Exception ex)
//                    {
//                        //db.Sp_Add_ErrorLog(ex.Message +  ex.InnerException.ToString(),null, ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
//                    }
//                }

//            }

//        }
//        public void FinalCancelSMS()
//        {
//            var res = db.Sp_Get_TempCancel_File(null, null, null, null, null, null, null, null, null).ToList();

//            foreach (var item in res)
//            {
//                var msgtext = "Dear " + item.Name + "\n\r," +
//                       "7 days are left till the extended date. i.e. 31st August 2020 for the complete submission of your dues against your cancelled plot.\n\r" +
//                       "Kindly visit our head office to reinstate your plot and for its payment.\n\r" +
//                       "Meher Estate Developers\n\r" +
//                       "042 - 111 724 786";
//                try
//                {
//                    SmsService smsService = new SmsService();
//                    smsService.SendMsg(msgtext, item.Mobile_1);
//                    db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");
//                }
//                catch (Exception ee)
//                {
//                    
//                    e.SendEmail(item.Name + " " + item.FileFormNumber, "taimoor@sasystems.solutions", "Msg Not Sent");
//                }
//            }
//        }

//        //public void FridayOpenSMS()
//        //{
//        //    var res = db.Sp_Get_TempCancel_File(null, null, null, null, null, null, null, null, null).ToList();
//        //    string[] others = new string[] { "4910505", "4910506", "4900632", "4900634", "4912101", "110074406" };

//        //    DateTime deadlineDate = new DateTime(2020, 08, 31);
//        //    DateTime currDate = DateTime.Now;

//        //    int ttlDaysLeft = (int)Math.Ceiling((deadlineDate - DateTime.Now).TotalDays);

//        //    if (ttlDaysLeft < 0)
//        //    {
//        //        return;
//        //    }

//        //    foreach (var item in res)
//        //    {
//        //        var msgtext = "Respected Customer,\n\r" +
//        //            "This is to inform you that tomorrow Friday, 28th August,2020, Meher Estate Developers Head Office will remain open. (9:00AM - 6:00PM)\n\r" +
//        //            "Kindly visit our head office for any querries.\n\r" +
//        //            "Best Regards.\n\r" +
//        //            "S A Gardens.";
//        //        try
//        //        {
//        //            SmsService smsService = new SmsService();
//        //            smsService.SendMsg(msgtext, item.Mobile_1);
//        //            //db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");
//        //        }
//        //        catch (Exception ee)
//        //        {
//        //            
//        //            e.SendEmail(item.Name + " " + item.FileFormNumber, "daniyal@sasystems.solutions", "Msg Not Sent");
//        //        }
//        //    }

//        //    foreach (var othFile in others)
//        //    {
//        //        var cust = db.Sp_Get_File_Search(othFile, 1).FirstOrDefault();
//        //        try
//        //        {
//        //            long parsedFileNo = Convert.ToInt32(othFile);
//        //            var fileInf = db.File_Form.Where(x => x.FileFormNumber == parsedFileNo).Select(x => x.Id).FirstOrDefault();
//        //            var msgtext = "Respected Customer,\n\r" +
//        //            "This is to inform you that tomorrow Friday, 28th August,2020, Meher Estate Developers Head Office will remain open. (9:00AM - 6:00PM)\n\r" +
//        //            "Kindly visit our head office for any querries.\n\r" +
//        //            "Best Regards.\n\r" +
//        //            "S A Gardens."; ;

//        //            SmsService smsService = new SmsService();
//        //            smsService.SendMsg(msgtext, cust.Mobile_1);
//        //            //db.Sp_Add_FileComments(fileInf, msgtext, 0, "Text");
//        //        }
//        //        catch (Exception ee)
//        //        {
//        //            
//        //            e.SendEmail(cust.Name + " " + cust.FileFormNumber, "daniyal@sasystems.solutions", "Msg Not Sent");
//        //        }
//        //    }
//        //}
//        public void PossessionSMS()
//        {
//            var res = db.temp_sms().ToList();

//            var res2 = res.Select(x => x.Mobile_1).Distinct().ToList();

//            foreach (var item in res2)
//            {
//                var msgtext = "Respected Clients,\n\r" +
//                    "Congratulations!!It gives us immense pleasure to offer the POSSESSION of Badar (Sector A) block. Visit our head office at your earliest and take another step towards constructing your dream house. We appreciate you for reposing you’re trust and confidence in Meher Estate Developers.\n\r" +
//                    "Best wishes and regards.\n\r" +
//                    "S A Gardens.";
//                try
//                {
//                    SmsService smsService = new SmsService();
//                    smsService.SendMsg(msgtext, item);
//                }
//                catch (Exception ee)
//                {
//                    //
//                    //e.SendEmail(item.Name + " " + item.Plot_Id, "ans.malik@meharestate.com", "Msg Not Sent");
//                }
//            }
//        }
//        public JsonResult CancelledSms()
//        {
//            long?[] tempar = {4900587,
//4911795,
//5330087785,
//950087387,
//530087786,
//4905515,
//4905514,
//4909623,
//4909624,
//4916027,
//4905603,
//850073582,
//4904700,
//4902130,
//4912010,
//4910110,
//4903260,
//170062980,
//4905599,
//4909384,
//4907422,
//4913125,
//4905599,
//4913087,
//740063021,
//4905393,
//4905760,
//4912047,
//4905548,
//4907320,
//4910747,
//850087011,
//4911002,
//4905811,
//4901464,
//4913289,
//4904128,
//4907477,
//4908346,
//850074076,
//4913288,
//4904768,
//4910754,
//4910755,
//4910017,
//4911030,
//4907792,
//4903745,
//4912461,
//4903313,
//4911541,
//4906885,
//820085036,
//4910194,
//4908014,
//4904009,
//4906385,
//4905415,
//4913270,
//4910536,
//820085011,
//820085012,
//4910448,
//4903813,
//4903814,
//4903815,
//4903816,
//4903817,
//4903818,
//4903819,
//4903820,
//4910707,
//4910708,
//4910709,
//4908340,
//4916837,
//740063048,
//4903620,
//4908064,
//4901698,
//4909220,
//140074277,
//4906824,
//4906825,
//4906826,
//4907107,
//4904557,
//4907883,
//4907884,
//4905492,
//4907321,
//4903063,
//4908044,
//4912455,
//4906812,
//4909402,
//800074594,
//4902223,
//4905353,
//4905354,
//4902160,
//4903844,
//4905166,
//4907266,
//4905340,
//4906423,
//4916357,
//740063040,
//4910453,
//4911681,
//4908318,
//4908422,
//4905545,
//4907540,
//4900878,
//4907220,
//4913576,
//4913577,
//4903347,
//4900440,
//4905071,
//4902958,
//4907592,
//4913168,
//4903856,
//4910885,
//4902262,
//4902264,
//4903414,
//4914658};
//            var res = db.Sp_Get_TempCancel_File(null, null, null, null, null, null, null, null, null).Where(x => tempar.Contains(x.FileFormNumber)).ToList();


//            foreach (var item in res)
//            {
//                var msgtext =
//                    "Dear Customer,\n\r" +
//                    "With reference to the application you had submitted. Company’s CEO has granted final grace period till 16th of July,2020 to submit your complete overdue amount.\n\r" +
//                    "After the mentioned date your file will be permanently blocked without any prior notice leading to refund process only, On which 10% cancelation charges of total plot price will be deducted from your deposited amount.\n\r" +
//                    "Best wishes and regards.\n\r" +
//                    "S A Gardens.";
//                try
//                {
//                    SmsService smsService = new SmsService();
//                    smsService.SendMsg(msgtext, item.Mobile_1);
//                    //db.sp_add_fo(item.Id, msgtext, 0, "Text");
//                }
//                catch (Exception ee)
//                {
//                    
//                    e.SendEmail(item.Name + " " + item.FileFormNumber, "info@meharestate.com", "Msg Not Sent");
//                }
//            }

//            return Json(res.Select(x => new { File = x.FileFormNumber, name = x.Name, Phone = x.Mobile_1 }), JsonRequestBehavior.AllowGet);
//        }
//        public void text1()
//        {
//            long?[] tempar = {4900587,
//4911795,
//5330087785,
//950087387,
//530087786,
//4914658};
//            var msgtext =
//                   "Dear Customer,\n\r" +
//                   "With reference to the application you had submitted. Company’s CEO has granted final grace period till 16th of July,2020 to submit your complete overdue amount.\n\r" +
//                   "After the mentioned date your file will be permanently blocked without any prior notice leading to refund process only, On which 10% cancelation charges of total plot price will be deducted from your deposited amount.\n\r" +
//                   "Best wishes and regards.\n\r" +
//                   "S A Gardens.";

//            var res = db.File_Form.Where(x => tempar.Contains(x.FileFormNumber)).ToList();
//            foreach (var item in res)
//            {
//                Files_Comments f = new Files_Comments()
//                {
//                    Comment = msgtext,
//                    Date = new DateTime(2020, 7, 16),
//                    File_Id = item.Id,
//                    Msg_Type = "Text",
//                    Userid = 0
//                };
//                db.Files_Comments.Add(f);
//                db.SaveChanges();
//            }
//        }


//        public void sherafghansms()
//        {
//            List<DealerSms> d = new List<DealerSms>();
//            d.Add(new DealerSms { Id = 81, Dealershipname = "Rehman Estate", Marla2 = 50, Marla4 = 25 });
//            d.Add(new DealerSms { Id = 12, Dealershipname = "A R Estate", Marla2 = 10, Marla4 = 5 });
//            d.Add(new DealerSms { Id = 34, Dealershipname = "Al Raheem Estate", Marla2 = 10, Marla4 = 5 });
//            d.Add(new DealerSms { Id = 91, Dealershipname = "Sky Limit Real Estate", Marla2 = 100, Marla4 = 50 });
//            d.Add(new DealerSms { Id = 29, Dealershipname = "Al Mannan Estate", Marla2 = 40, Marla4 = 20 });
//            d.Add(new DealerSms { Id = 77, Dealershipname = "Punjab Estate", Marla2 = 20, Marla4 = 10 });
//            d.Add(new DealerSms { Id = 28, Dealershipname = "Al Haram State", Marla2 = 4, Marla4 = 2 });
//            d.Add(new DealerSms { Id = 93, Dealershipname = "Wall Real Estate", Marla2 = 30, Marla4 = 15 });
//            d.Add(new DealerSms { Id = 57, Dealershipname = "Jag Associates", Marla2 = 60, Marla4 = 30 });

//            foreach (var item in d)
//            {
//                var deal = db.Dealers.Where(x => x.Dealership_Id == item.Id).ToList();

//                foreach (var i in deal)
//                {
//                    var msgtext = "Dear " + item.Dealershipname + ",\n\r" +
//                    "Congratulations! We are pleased to inform you that you have been allotted ‘"+ item.Marla4 +"’ 4 Marla and ‘"+ item.Marla2+"’ 2 Marla Sher Afghan Commercial Files. Please deposit your security against allotted Files till 20th November 2020. . \n\r" +
//                    "\n\r" +
//                    "Best Regards,\n\r" +
//                    "Meher Estate Developers.\n\r" +
//                    "042-111 724 786";

//                    try
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(msgtext, i.Mobile_1);
//                    }
//                    catch (Exception e)
//                    {
//                        continue;
//                    }
//                }

//            }



//        }


//        public void DifferPlotSizeSms()
//        {
//            //var res = db.Sp_Get_DifferSize_Plots().FirstOrDefault();
//            //var pltId = db.Plots.Where(x => x.Plot_Number == res.PlotNo && x.Sector == res.Sector && x.Block_Name == res.Block).Select(x => x.Id).FirstOrDefault();
//            //var res2 = db.Sp_Get_PlotLastOwner(pltId).ToList();
//            //var text = "Dear Customer,\n" +
//            //    "With due respect your plot size after balloting has been increased from " + res.PlotArea + " Marla to " + res.Actual_Size + " Marla and amount of the increased marla is adjusted in your remaining installments with effect from September, 2021.\n" +
//            //    "For any further information contact our helpline or visit our head office.\n" +
//            //    "Best Regards,\n" +
//            //    "Meher Estate Developers.\n" +
//            //    "042 - 111 724 786";
//            //foreach (var item in res2)
//            //{
//            //    try
//            //    {
//            //        SmsService smsService = new SmsService();
//            //        smsService.SendMsg(text, "03038526637");
//            //    }
//            //    catch (Exception ee)
//            //    {
//            //        //
//            //        //e.SendEmail(item.Name + " " + item.Plot_Id, "info@meharestate.com", "Msg Not Sent");
//            //    }
//            //}
//            var res = db.Sp_Get_DifferSize_Plots().ToList();
//            foreach (var v in res)
//            {
//                var pltId = db.Plots.Where(x => x.Plot_Number == v.PlotNo && x.Sector == v.Sector && x.Block_Name == v.Block).Select(x => x.Id).FirstOrDefault();
//                var res2 = db.Sp_Get_PlotLastOwner(pltId).ToList();
//                var text = "Dear Customer,\n" +
//                    "With due respect your plot size after balloting has been increased from " + v.PlotArea + " Marla to " + v.Actual_Size + " Marla and amount of the increased marla is adjusted in your remaining installments with effect from September, 2021.\n" +
//                    "For any further information contact our helpline or visit our head office.\n" +
//                    "Best Regards,\n" +
//                    "Meher Estate Developers.\n" +
//                    "042 - 111 724 786";
//                foreach (var item in res2)
//                {
//                    try
//                    {
//                        SmsService smsService = new SmsService();
//                        smsService.SendMsg(text, item.Mobile_1);
//                    }
//                    catch (Exception ee)
//                    {
//                        //
//                        //e.SendEmail(item.Name + " " + item.Plot_Id, "info@meharestate.com", "Msg Not Sent");
//                    }
//                }
//            }

//        }

//        //public void PaymentModeInformation()
//        //{

//        //    var res = db.Files_Transfer.Select(x => new { FileNo = x.File_Form_Id, MobNum = x.Mobile_1 }).ToList();

//        //    foreach(var item in res)
//        //    {
//        //        var msgtext = "Respected Customers,\n\r" +
//        //                            "Kindly note the only acceptable mode of payments for instalment are as follows:\n\r" +
//        //                            "1. SA Garden Offices\n\r" +
//        //                            "2. Challan form at HBL\n\r" +
//        //                            "3. Challan form at Dubai Islamic Bank\n\r" +
//        //                            "4. Meher Estate Developers Customer Portal.\n\r" +
//        //                            "Payments can also be deposited at our office in Lahore at Center Point Plaza - Kalma Chowk.\n\r" +
//        //                            "Anonymous online payment method will not be entertained.\n\r" +
//        //                            "Best Regards,\n\r" +
//        //                            "S A Gardens.";
//        //        try
//        //        {
//        //            SmsService smsService = new SmsService();
//        //            smsService.PremiumSub(msgtext, item.MobNum);
//        //            //db.sp_add_fo(item.Id, msgtext, 0, "Text");
//        //        }
//        //        catch (Exception ee)
//        //        {
//        //            
//        //            e.SendEmail(item.FileNo + " " + item.MobNum, "daniyal@sasystems.solutions", "Msg Not Sent");
//        //        }
//        //    }


//        //    //foreach (var item in res)
//        //    //{
//        //    //    var msgtext = "Respected Customers,\n\r" +
//        //    //        "Kindly note the only acceptable mode of payments for instalment are as follows:\n\r" +
//        //    //        "1. SA Garden Offices\n\r" +
//        //    //        "2. Challan form at HBL\n\r" +
//        //    //        "3. Challan form at Dubai Islamic Bank\n\r" +
//        //    //        "4. Meher Estate Developers Customer Portal.\n\r" +
//        //    //        "Payments can also be deposited at our office in Lahore at Center Point Plaza - Kalma Chowk.\n\r" +
//        //    //        "Anonymous online payment method will not be entertained.\n\r" +
//        //    //        "Best Regards,\n\r" +
//        //    //        "S A Gardens.";
//        //    //    try
//        //    //    {
//        //    //        SmsService smsService = new SmsService();
//        //    //        smsService.PremiumSub(msgtext, item.Mobile_1);
//        //    //        //db.sp_add_fo(item.Id, msgtext, 0, "Text");
//        //    //    }
//        //    //    catch (Exception ee)
//        //    //    {
//        //    //        
//        //    //        e.SendEmail(item.Name + " " + item.FileFormNumber, "info@meharestate.com", "Msg Not Sent");
//        //    //    }
//        //    //}
//        //}
//public JsonResult Transfer()
//{
//    var res1 = db.Sp_Get_RegisterFileDueAmount().ToList();

//    foreach (var item in res1)
//    {
//        try
//        {
//            FileSystemController f = new FileSystemController();
//            f.TestAdjustIntallments(Convert.ToInt64(item.FileFormNumber));
//            Sp_Get_CurrentOwner_File_Id_Result res2 = db.Sp_Get_CurrentOwner_File_Id(item.Id).FirstOrDefault();

//            if (res2.Balance_Amount < 0)
//            {
//                var msgtext = "Dear " + res2.Name + ", \n\r" +
//             "Congratulations! We are proud to announce that your Plot Number. " + res2.FileFormNumber + " is eligible for the ballot of Sher Afghan Block. To Qualify, Please pay your Ballot Charges and your Balance Amount of Rs. " + (res2.Balance_Amount) * -1 + " at your earliest.\n\r" +
//             "For further information call at our help line.\n\r" +
//             "Best Regards,\n\r" +
//             "Meher Estate Developers.\n\r" +
//             "042 - 111 724 786";
//                try
//                {
//                    SmsService smsService = new SmsService();
//                    smsService.SendMsg(msgtext, item.Mobile_1);
//                    var res3 = db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");

//                    if (!string.IsNullOrEmpty(item.Mobile_2) || !string.IsNullOrWhiteSpace(item.Mobile_2))
//                    {
//                        smsService.SendMsg(msgtext, item.Mobile_2);
//                    }
//                }
//                catch (Exception e)
//                {
//                    continue;
//                }
//            }
//            else
//            {
//                var msgtext = "Dear " + res2.Name + ", \n\r" +

//             "Congratulation! We are proud to announce that your plot. " + res2.FileFormNumber + " has qualified in the 3rd ballot for Sher Afghan Block. The plot number you have been allocated is "+ item.PlotNo  +". Kindly visit our office at Meher Estate Developers, Phase 2, Kala Shah Kaku and collect your ballot letter.\n\r" +
//             "For further information call at our help line.\n\r" +
//             "Best Regards,\n\r" +
//             "Meher Estate Developers.\n\r" +
//             "042 - 111 724 786";
//                try
//                {
//                    SmsService smsService = new SmsService();
//                    smsService.SendMsg(msgtext, item.Mobile_1);
//                    var res3 = db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");

//                    if (!string.IsNullOrEmpty(item.Mobile_2) || !string.IsNullOrWhiteSpace(item.Mobile_2))
//                    {
//                        smsService.SendMsg(msgtext, item.Mobile_2);
//                    }
//                }
//                catch (Exception e)
//                {
//                    continue;
//                }
//            }

//        }
//        catch (Exception ee)
//        {
//            continue;
//        }

//    }
//    return Json(true);
//}


//public JsonResult testmethod()
//{
//    var res1 = db.Sp_Get_RegisterFileDueAmount().ToList();
//    return Json(res1.Count(),JsonRequestBehavior.AllowGet);
//}