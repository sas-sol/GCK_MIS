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
    
    public partial class Sp_Get_Salaries_Non_Gazetted_Result
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> Salary_Month { get; set; }
        public Nullable<decimal> Basic_salary { get; set; }
        public Nullable<decimal> Cash { get; set; }
        public Nullable<decimal> Bank { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> Emp_Id { get; set; }
        public string Employee_Name { get; set; }
        public string Employee_Designation { get; set; }
        public string Employee_code { get; set; }
        public Nullable<System.DateTime> Emp_Date_Of_Joining { get; set; }
        public string Emp_Email { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> Received_Date { get; set; }
        public Nullable<long> Userid { get; set; }
        public Nullable<decimal> Bonus { get; set; }
        public string Bonus_Description { get; set; }
        public Nullable<int> Absentee { get; set; }
        public Nullable<int> Salary_Leaves { get; set; }
        public Nullable<int> No_Of_Days { get; set; }
        public Nullable<decimal> Allownces { get; set; }
        public Nullable<int> Extra_Working_Days { get; set; }
        public Nullable<decimal> Ded_Loan { get; set; }
        public Nullable<decimal> Ded_Advance { get; set; }
        public Nullable<decimal> Ded_Tax { get; set; }
        public Nullable<decimal> Ded_Other { get; set; }
        public string CNIC { get; set; }
        public Nullable<decimal> Grand_total { get; set; }
        public Nullable<decimal> Ded_Absentee { get; set; }
        public Nullable<int> Extra_Fuel { get; set; }
        public Nullable<decimal> Extra_Fuel_Charges { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<long> Designation_Id { get; set; }
        public string Designation_name { get; set; }
        public Nullable<decimal> Previous_Basic_Salary { get; set; }
        public Nullable<int> COLA_Percentage { get; set; }
        public Nullable<decimal> COLA_Amount { get; set; }
        public string COLA_Description { get; set; }
        public Nullable<decimal> Overtime { get; set; }
        public Nullable<bool> IsHold { get; set; }
        public Nullable<decimal> OverTimeHrs { get; set; }
        public Nullable<int> BankHeadId { get; set; }
        public string InstNo { get; set; }
        public Nullable<System.DateTime> InstDate { get; set; }
        public Nullable<decimal> Stipend { get; set; }
        public Nullable<long> Stipend_Dep_Id { get; set; }
        public string Stipend_Dep_Name { get; set; }
        public string Stipend_Designation { get; set; }
        public Nullable<int> CompId { get; set; }
        public Nullable<int> Level { get; set; }
    }
}
