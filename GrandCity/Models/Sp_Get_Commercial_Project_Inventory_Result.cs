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
    
    public partial class Sp_Get_Commercial_Project_Inventory_Result
    {
        public long Id { get; set; }
        public string Com_App_Shop_Number { get; set; }
        public string Type { get; set; }
        public decimal Area { get; set; }
        public string Location { get; set; }
        public long Floor_Id { get; set; }
        public Nullable<long> Plan_Id { get; set; }
        public string Status { get; set; }
        public Nullable<long> Installment_Plan_Id { get; set; }
        public Nullable<bool> Verify { get; set; }
        public Nullable<System.DateTime> Verification_Req_Datetime { get; set; }
        public string Map_Cord { get; set; }
        public Nullable<bool> Verification_Req { get; set; }
        public Nullable<System.DateTime> Verification_Date { get; set; }
        public Nullable<int> Shop_Dimension { get; set; }
        public Nullable<decimal> North { get; set; }
        public Nullable<decimal> South { get; set; }
        public Nullable<decimal> East { get; set; }
        public Nullable<decimal> West { get; set; }
        public Nullable<decimal> North_East { get; set; }
        public Nullable<decimal> North_West { get; set; }
        public Nullable<decimal> South_East { get; set; }
        public Nullable<decimal> South_West { get; set; }
        public Nullable<decimal> Front_Side { get; set; }
        public Nullable<decimal> Back_Side { get; set; }
        public Nullable<decimal> Right_Side { get; set; }
        public Nullable<decimal> Left_Side { get; set; }
        public string Front { get; set; }
        public Nullable<long> Project_Id { get; set; }
        public string Project_Name { get; set; }
        public string ApplicationNo { get; set; }
        public string FloorName { get; set; }
    }
}
