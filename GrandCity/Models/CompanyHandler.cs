//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading;
//using System.Web;

//namespace MeherEstateDevelopers.Models
//{
//    public static class CompanyHandler
//    {
//        public static string Company
//        {
//            get
//            {
//                var Identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
//                var company = Identity.Claims.Where(x => x.Type == "Company").Select(x => x.Value).FirstOrDefault();
//                return company;
//            }
//            set { }
//        }
//        public static string Comp_Id
//        {
//            get
//            {
//                var Identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
//                var Comp_Id = Identity.Claims.Where(x => x.Type == "Comp_Id").Select(x => x.Value).FirstOrDefault();
//                return Comp_Id;
//            }
//            set { }
//        }
//        public static int Comp_COA
//        {
//            get
//            {
//                var Identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
//                var Comp_COA = Identity.Claims.Where(x => x.Type == "Comp_COA").OrderByDescending(x => x.Value).Select(x => x.Value).FirstOrDefault();
//                return int.Parse(Comp_COA);
//            }
//            set { }
//        }
       
//    }
//}