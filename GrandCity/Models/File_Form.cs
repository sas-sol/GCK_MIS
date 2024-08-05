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
    
    public partial class File_Form
    {
        public long Id { get; set; }
        public string FileFormNumber { get; set; }
        public Nullable<long> QRCode { get; set; }
        public string Plot_Size { get; set; }
        public long Dealership_Id { get; set; }
        public decimal Security { get; set; }
        public Nullable<long> Phase_Id { get; set; }
        public Nullable<long> Block_Id { get; set; }
        public long Status { get; set; }
        public Nullable<bool> Development_Charges { get; set; }
        public bool Security_NoSecurity { get; set; }
        public System.DateTime DateTime { get; set; }
        public Nullable<decimal> Security_Amount { get; set; }
        public Nullable<long> Group_Id { get; set; }
        public Nullable<long> Installment_Plan_Id { get; set; }
        public Nullable<long> Generated_By { get; set; }
        public Nullable<bool> Verify { get; set; }
        public Nullable<bool> Verified { get; set; }
        public Nullable<bool> Verification_Req { get; set; }
        public string Phase { get; set; }
        public string Block { get; set; }
        public Nullable<System.DateTime> Verifi_Req_Date { get; set; }
        public Nullable<bool> Deal_Voucher_Flag { get; set; }
        public Nullable<System.DateTime> VerifiedDate { get; set; }
        public Nullable<long> BallotedPlot_Id { get; set; }
        public string BallotedPlot_Number { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> Commession { get; set; }
        public string Project { get; set; }
        public string Sector { get; set; }
        public Nullable<decimal> SecCommission { get; set; }
        public Nullable<decimal> DealerShipCommisionAmt { get; set; }
    }
}
