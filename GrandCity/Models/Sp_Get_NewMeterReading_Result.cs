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
    
    public partial class Sp_Get_NewMeterReading_Result
    {
        public long Id { get; set; }
        public Nullable<long> Plot_Id { get; set; }
        public Nullable<System.DateTime> Month { get; set; }
        public Nullable<long> Current_Reading { get; set; }
        public Nullable<long> Previous_Reading { get; set; }
        public Nullable<decimal> Arrears { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> Units { get; set; }
        public Nullable<long> Meter_Id { get; set; }
        public string Remarks { get; set; }
        public string Name { get; set; }
        public string Plot_No { get; set; }
        public string Block { get; set; }
        public string Plot_Type { get; set; }
        public string Meter_No { get; set; }
    }
}
