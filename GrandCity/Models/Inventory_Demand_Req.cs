//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeherEstateDevelopers.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Inventory_Demand_Req
    {
        public long Id { get; set; }
        public string Item_Name { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public string Details { get; set; }
        public Nullable<long> ReqTo_Dep { get; set; }
        public Nullable<long> ReqBy { get; set; }
        public Nullable<System.DateTime> ReqBy_Date { get; set; }
        public string ReqBy_Name { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedBy_Date { get; set; }
        public string ApprovedBy_Name { get; set; }
        public Nullable<long> Prj_Id { get; set; }
        public string Prj_Name { get; set; }
        public Nullable<long> Group_Id { get; set; }
        public string ReqTo_Dep_Name { get; set; }
        public Nullable<int> Dep_Id { get; set; }
        public string Dep_Name { get; set; }
        public string Status { get; set; }
        public Nullable<long> Attachment_Id { get; set; }
    }
}
