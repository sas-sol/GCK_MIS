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
    
    public partial class Sp_Get_PlotsServiceCharges_ByBlock_Result
    {
        public string Name { get; set; }
        public long Plot_Id { get; set; }
        public string Plot_No { get; set; }
        public string Block { get; set; }
        public string Phase { get; set; }
        public System.DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Plot_Type { get; set; }
        public decimal Charges { get; set; }
        public decimal Rate { get; set; }
        public string Service_Charges { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> Arrears { get; set; }
        public Nullable<decimal> Amount_Paid { get; set; }
        public Nullable<int> Fine_Percentage { get; set; }
        public Nullable<decimal> Fine_Amount { get; set; }
        public Nullable<System.DateTime> duedate { get; set; }
        public Nullable<decimal> Due_Date_Amount { get; set; }
        public Nullable<decimal> Net_Total { get; set; }
    }
}
