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
    
    public partial class Journal_Entries
    {
        public long Id { get; set; }
        public string Naration { get; set; }
        public string Head { get; set; }
        public string Head_Code { get; set; }
        public string Head_Name { get; set; }
        public Nullable<int> Head_Id { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<long> Line { get; set; }
        public Nullable<long> GroupId { get; set; }
        public Nullable<long> Recorded_By { get; set; }
        public System.DateTime RecordedBy_Date { get; set; }
        public string RecordedBy_Name { get; set; }
        public Nullable<long> Supvise_by { get; set; }
        public string Supviseby_Name { get; set; }
        public Nullable<System.DateTime> Supviseby_Date { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Nullable<long> Ref_Id { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string UOM { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> Conversion_Rate { get; set; }
        public string Inst_No { get; set; }
        public Nullable<System.DateTime> Inst_Date { get; set; }
        public string Inst_Status { get; set; }
        public string Inst_Bank { get; set; }
        public string Block { get; set; }
        public string Voucher_No { get; set; }
        public string Voucher_Type { get; set; }
        public Nullable<int> Comp_Id { get; set; }
        public string Paid_By { get; set; }
    }
}
