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
    
    public partial class FuelSheet
    {
        public long Id { get; set; }
        public Nullable<decimal> FilledLitre { get; set; }
        public Nullable<decimal> AmountMade { get; set; }
        public Nullable<System.DateTime> FillingDate { get; set; }
        public string FuelType { get; set; }
        public Nullable<decimal> FuelRate { get; set; }
        public Nullable<long> EmpId { get; set; }
        public string EmpName { get; set; }
        public Nullable<long> CreatedBy_Id { get; set; }
        public string CreatedBy_Name { get; set; }
        public Nullable<bool> IsDiscount { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<long> VehId { get; set; }
        public string VehName { get; set; }
    }
}
