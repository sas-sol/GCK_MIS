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
    
    public partial class Biding_Reserve_Plots
    {
        public long Id { get; set; }
        public long Dealer_Id { get; set; }
        public long Plot_Id { get; set; }
        public string DealerName { get; set; }
        public Nullable<decimal> PlotPrice { get; set; }
        public Nullable<long> GroupTag { get; set; }
        public Nullable<System.DateTime> DealDate { get; set; }
        public Nullable<long> AddedBy_Id { get; set; }
        public string AddedBy_Name { get; set; }
        public string PlotNum { get; set; }
        public string PlotStatus { get; set; }
        public Nullable<decimal> SpecialPrefAmount { get; set; }
        public Nullable<decimal> DCAmount { get; set; }
        public Nullable<decimal> CommisionAmount { get; set; }
        public Nullable<int> Installments_Seg { get; set; }
        public Nullable<decimal> Percentage_Adj { get; set; }
    }
}
