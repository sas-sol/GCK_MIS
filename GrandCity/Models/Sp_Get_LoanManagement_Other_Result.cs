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
    
    public partial class Sp_Get_LoanManagement_Other_Result
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string Reason { get; set; }
        public Nullable<long> Emp_Id { get; set; }
        public string HrApproval { get; set; }
        public string ManagerApproval { get; set; }
        public string HrRemarks { get; set; }
        public string ManagerRemarks { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> Installments { get; set; }
        public Nullable<long> User_Id { get; set; }
        public string Employee_Name { get; set; }
        public Nullable<long> HR_Id { get; set; }
        public string HR_Name { get; set; }
        public Nullable<long> Manager_Id { get; set; }
        public string Manager_Name { get; set; }
        public string Type { get; set; }
        public Nullable<bool> Paid_Status { get; set; }
        public Nullable<int> PhysicalDoc { get; set; }
        public string PhysicalDocNum { get; set; }
    }
}
