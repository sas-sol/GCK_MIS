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
    
    public partial class Sp_Get_CA_OpeningClosing_Balance_Result
    {
        public string Text_ChartCode { get; set; }
        public int Id { get; set; }
        public string Head { get; set; }
        public Nullable<decimal> Debit_Balance { get; set; }
        public Nullable<decimal> Credit_Balance { get; set; }
        public string Balance_Type { get; set; }
        public Nullable<System.DateTime> Fiscal_Start { get; set; }
        public Nullable<System.DateTime> Fiscal_End { get; set; }
    }
}
