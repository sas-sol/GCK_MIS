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
    
    public partial class Voucher
    {
        public long Id { get; set; }
        public string VoucherNo { get; set; }
        public string Name { get; set; }
        public string Father_Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Voucher_Amount { get; set; }
        public string AmountinWords { get; set; }
        public string PaymentType { get; set; }
        public string Bank { get; set; }
        public string Ch_Pay_Draft_No { get; set; }
        public Nullable<System.DateTime> Ch_Pay_Draft_Date { get; set; }
        public string Branch_Name { get; set; }
        public string Project { get; set; }
        public Nullable<long> File_Plot_Id { get; set; }
        public string Module { get; set; }
        public string Type { get; set; }
        public Nullable<long> TokenParameter { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Cancel { get; set; }
        public Nullable<long> Vendor_Id { get; set; }
        public string Gen_Name { get; set; }
        public Nullable<long> Userid { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public string Pre_Name { get; set; }
        public Nullable<long> Prepared_by { get; set; }
        public Nullable<System.DateTime> Pre_Datetime { get; set; }
        public string Sup_Name { get; set; }
        public Nullable<long> Supervised_By { get; set; }
        public Nullable<System.DateTime> Sup_Datetime { get; set; }
        public string Status { get; set; }
        public Nullable<long> Group_Id { get; set; }
        public Nullable<long> Transaction_Id { get; set; }
        public Nullable<bool> Checked { get; set; }
        public Nullable<System.DateTime> dayClose { get; set; }
        public Nullable<int> Comp_Id { get; set; }
        public Nullable<long> Sale_Id { get; set; }
        public Nullable<bool> Plot_Is_Cancelled { get; set; }
    }
}
