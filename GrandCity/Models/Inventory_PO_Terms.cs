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
    
    public partial class Inventory_PO_Terms
    {
        public long Id { get; set; }
        public string Terms { get; set; }
        public Nullable<long> Req_Id { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<long> Created_By { get; set; }
        public string CreatedBy_Name { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<long> Vendor_Id { get; set; }
    }
}
