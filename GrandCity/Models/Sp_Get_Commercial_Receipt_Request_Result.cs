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
    
    public partial class Sp_Get_Commercial_Receipt_Request_Result
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Father_Name { get; set; }
        public string Contact { get; set; }
        public string Module { get; set; }
        public Nullable<long> Module_Id { get; set; }
        public string Remarks { get; set; }
        public string Narration { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Type { get; set; }
        public Nullable<long> Requested_By { get; set; }
        public string RequestedBy_Name { get; set; }
        public Nullable<System.DateTime> RequestedBy_Date { get; set; }
        public Nullable<long> Approved_By { get; set; }
        public string ApprovedBy_Name { get; set; }
        public Nullable<System.DateTime> ApprovedBy_Date { get; set; }
        public string Voucher_For { get; set; }
        public string Status { get; set; }
        public Nullable<long> Lead_Id { get; set; }
        public string Project { get; set; }
    }
}
