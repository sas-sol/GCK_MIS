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
    
    public partial class Lead_Followups
    {
        public long Id { get; set; }
        public long Lead_Id { get; set; }
        public string Description { get; set; }
        public long Userid { get; set; }
        public System.DateTime Datetime { get; set; }
        public string Type { get; set; }
    }
}
