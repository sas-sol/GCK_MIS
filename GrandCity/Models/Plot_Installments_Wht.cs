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
    
    public partial class Plot_Installments_Wht
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public System.DateTime DueDate { get; set; }
        public long Plot_Id { get; set; }
        public string Installment_Name { get; set; }
        public Nullable<long> Owner_Id { get; set; }
        public Nullable<bool> Cancelled { get; set; }
        public string Installment_Type { get; set; }
        public string WHT_Status { get; set; }
        public string Charge { get; set; }
    }
}