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
    
    public partial class Marketing_Receipt_UpdateRequests
    {
        public long Id { get; set; }
        public string Bank { get; set; }
        public string Ch_Pay_Draft_No { get; set; }
        public Nullable<System.DateTime> Ch_Pay_Draft_date { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<System.DateTime> Token_Exp_Date { get; set; }
        public string Description { get; set; }
        public Nullable<long> Userid { get; set; }
        public string Status { get; set; }
        public long Receipt_Id { get; set; }
        public string PaymentType { get; set; }
    }
}
