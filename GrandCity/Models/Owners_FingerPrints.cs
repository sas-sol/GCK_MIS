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
    
    public partial class Owners_FingerPrints
    {
        public long Id { get; set; }
        public Nullable<long> OwnerId { get; set; }
        public string Fp_PrintCode { get; set; }
        public string Fp_Image { get; set; }
        public Nullable<int> Finger_Num { get; set; }
        public Nullable<System.DateTime> AddedOn_Date { get; set; }
        public string Added_ByName { get; set; }
        public Nullable<long> AddedBy_Id { get; set; }
    }
}
