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
    
    public partial class LeaveRequisition
    {
        public long Id { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<System.DateTimeOffset> CreatedAt { get; set; }
        public string Reason { get; set; }
        public Nullable<long> EmpId { get; set; }
        public Nullable<int> NoOfDays { get; set; }
        public Nullable<int> OfficalLeave { get; set; }
        public string HrApproval { get; set; }
        public string ManagerApproval { get; set; }
        public string HrRemarks { get; set; }
        public string ManagerRemarks { get; set; }
        public Nullable<bool> Cancel { get; set; }
        public string LeaveType { get; set; }
        public Nullable<long> AppliedBy { get; set; }
        public Nullable<int> Status { get; set; }
        public string AvailLeave { get; set; }
    }
}
