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
    
    public partial class Sp_Get_Cancellation_Plot_Result
    {
        public Nullable<long> Id { get; set; }
        public string Plot_No { get; set; }
        public string Type { get; set; }
        public string Road_Type { get; set; }
        public string STATUS { get; set; }
        public string Plot_Size { get; set; }
        public string Plot_Location { get; set; }
        public string Postal_Address { get; set; }
        public Nullable<bool> Verified { get; set; }
        public Nullable<bool> Verification_Req { get; set; }
        public string Block_Name { get; set; }
        public string Name { get; set; }
        public Nullable<long> Owner_Id { get; set; }
        public string Mobile_1 { get; set; }
        public Nullable<decimal> Balance_Amount { get; set; }
        public Nullable<int> Installments { get; set; }
        public Nullable<System.DateTime> First_Notice { get; set; }
        public Nullable<System.DateTime> Sec_Notice { get; set; }
        public Nullable<System.DateTime> Cancel_Notice { get; set; }
    }
}
