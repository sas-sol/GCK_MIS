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
    
    public partial class Project_Payment_Schedule
    {
        public long Id { get; set; }
        public long PrjId { get; set; }
        public string Description { get; set; }
        public double CompletionPercentage { get; set; }
        public double CompletionAmount { get; set; }
        public double WorkDonePercentage { get; set; }
        public double WorkDoneAmount { get; set; }
        public Nullable<int> SID { get; set; }
        public Nullable<long> MileId { get; set; }
    }
}
