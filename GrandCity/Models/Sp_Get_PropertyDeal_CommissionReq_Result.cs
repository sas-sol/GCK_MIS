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
    
    public partial class Sp_Get_PropertyDeal_CommissionReq_Result
    {
        public long Id { get; set; }
        public string File_Plot_Number { get; set; }
        public string Block { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> Commession { get; set; }
        public string Commission_Type { get; set; }
        public Nullable<decimal> Offered_Rate { get; set; }
        public string RequestedBy { get; set; }
        public string Name { get; set; }
    }
}
