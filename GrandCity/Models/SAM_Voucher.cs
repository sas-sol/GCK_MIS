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
    
    public partial class SAM_Voucher
    {
        public long Id { get; set; }
        public string VoucherNo { get; set; }
        public string Name { get; set; }
        public string Father_Name { get; set; }
        public string Contact { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string AmountinWords { get; set; }
        public string PaymentType { get; set; }
        public string Bank { get; set; }
        public string Ch_Pay_Draft_No { get; set; }
        public Nullable<System.DateTime> Ch_Pay_Draft_Date { get; set; }
        public string Branch_Name { get; set; }
        public string Project { get; set; }
        public Nullable<long> Lead_Id { get; set; }
        public string Size { get; set; }
        public Nullable<decimal> RatePerMarla { get; set; }
        public Nullable<decimal> Plot_Total_Amount { get; set; }
        public string Type { get; set; }
        public Nullable<long> TokenParameter { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public string Module { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Cancel { get; set; }
        public Nullable<long> Userid { get; set; }
        public string Block { get; set; }
        public string DevelopmentCharges { get; set; }
        public string File_Plot_Number { get; set; }
        public Nullable<bool> Request { get; set; }
        public Nullable<long> RequestedBy { get; set; }
        public Nullable<System.DateTime> RequestedDate { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> dayClose { get; set; }
    }
}
