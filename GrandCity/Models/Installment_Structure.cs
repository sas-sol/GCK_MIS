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
    
    public partial class Installment_Structure
    {
        public int Id { get; set; }
        public string Installment_Name { get; set; }
        public string Installment_Type { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime Datetime { get; set; }
        public int Installment_Plot_Id { get; set; }
        public Nullable<int> After { get; set; }
        public string Module { get; set; }
        public Nullable<int> Interval { get; set; }
    
        public virtual Installment_Plot_Category Installment_Plot_Category { get; set; }
    }
}
