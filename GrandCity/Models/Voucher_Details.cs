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
    
    public partial class Voucher_Details
    {
        public long Id { get; set; }
        public string Item_Name { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public string UOM { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<long> Voucher_Id { get; set; }
    }
}
