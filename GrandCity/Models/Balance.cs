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
    
    public partial class Balance
    {
        public long Id { get; set; }
        public Nullable<long> File_Plot_Id { get; set; }
        public string Module { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
        public Nullable<decimal> Received_Amount { get; set; }
        public Nullable<decimal> Received_Till_20 { get; set; }
        public string Status { get; set; }
        public string Block { get; set; }
        public string Refund_Amt { get; set; }
        public string Discount_Amt { get; set; }
    }
}
