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
    
    public partial class Level
    {
        public long Id { get; set; }
        public string Level_Name { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public Nullable<int> Level_Count { get; set; }
        public string Level_Img_Name { get; set; }
    }
}
