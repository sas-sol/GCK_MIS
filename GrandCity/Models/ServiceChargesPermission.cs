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
    
    public partial class ServiceChargesPermission
    {
        public long Id { get; set; }
        public Nullable<long> CreatedBy_UserId { get; set; }
        public string CreatedBy_Name { get; set; }
        public Nullable<long> ApprovingBody_UserId { get; set; }
        public string ApprovingBody_Name { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Permission_Details { get; set; }
        public string ModuleType { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<long> For_ModuleId { get; set; }
        public string For_ModuleName { get; set; }
        public string ForModuleType { get; set; }
        public string ApprovingBody_Remarks { get; set; }
        public Nullable<long> GroupId { get; set; }
        public string ServiceType { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
}
