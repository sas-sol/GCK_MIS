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
    
    public partial class Sp_Get_Report_ElectricityCharges_Result
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Develop_Status { get; set; }
        public long Id { get; set; }
        public long Plot_Id { get; set; }
        public string Plot_No { get; set; }
        public string Block { get; set; }
        public System.DateTime Month { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
        public Nullable<decimal> Arrears { get; set; }
        public Nullable<decimal> Grand_Total { get; set; }
        public Nullable<decimal> Amount_Paid { get; set; }
        public Nullable<decimal> Remaining { get; set; }
        public Nullable<System.DateTime> AmountPaid_Date { get; set; }
        public string Meter_No { get; set; }
        public long Previous_Reading { get; set; }
        public long Current_Reading { get; set; }
        public int Units { get; set; }
        public long Block_Id { get; set; }
        public string Bill_No { get; set; }
    }
}
