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
    
    public partial class Electricity_Bill_Rates
    {
        public int Id { get; set; }
        public int Start_Range { get; set; }
        public int End_Range { get; set; }
        public string Meter_Type { get; set; }
        public int Levels { get; set; }
        public Nullable<decimal> Unit_Rate { get; set; }
    }
}
