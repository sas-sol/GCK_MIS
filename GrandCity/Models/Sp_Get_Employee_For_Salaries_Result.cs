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
    
    public partial class Sp_Get_Employee_For_Salaries_Result
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CNIC { get; set; }
        public string Email { get; set; }
        public string Department_Designation { get; set; }
        public string Employee_Code { get; set; }
        public Nullable<System.DateTime> Date_Of_Joining { get; set; }
        public Nullable<decimal> Basic_Salary { get; set; }
        public Nullable<decimal> Stipend { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<int> BankId { get; set; }
    }
}
