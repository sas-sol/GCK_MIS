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
    
    public partial class Opening_Closing_Balance
    {
        public long Id { get; set; }
        public string Head { get; set; }
        public string Head_Code { get; set; }
        public string Head_Name { get; set; }
        public Nullable<int> Head_Id { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string Balance_Type { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<long> Userid { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Fiscal_Start { get; set; }
        public Nullable<System.DateTime> Fiscal_End { get; set; }
        public Nullable<decimal> Debit_Balance { get; set; }
        public Nullable<decimal> Credit_Balance { get; set; }
    }
}
