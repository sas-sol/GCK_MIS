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
    
    public partial class PlotToMove
    {
        public long Id { get; set; }
        public string Plot_No { get; set; }
        public string Dimension { get; set; }
        public string Plot_Size { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Road { get; set; }
        public Nullable<long> Block { get; set; }
        public string Sector { get; set; }
    }
}
