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
    
    public partial class CommercialShopBound
    {
        public long Id { get; set; }
        public Nullable<long> RoomId { get; set; }
        public Nullable<long> BoundedLocationId { get; set; }
        public Nullable<long> Project_Id { get; set; }
        public string Project_Name { get; set; }
        public string Type { get; set; }
        public string Shop_No { get; set; }
        public string BoundedLocationName { get; set; }
        public string Position { get; set; }
    }
}
