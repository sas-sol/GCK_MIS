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
    
    public partial class SP_Get_MaterialStatement_Result
    {
        public long ID { get; set; }
        public long MilestoneID { get; set; }
        public long ProjectID { get; set; }
        public string RequestedItemName { get; set; }
        public double RequestedItemQty { get; set; }
        public string RequestedItemQtyType { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<double> Length { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Height { get; set; }
        public Nullable<double> Dia { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<long> Item_Id { get; set; }
        public string MilestoneName { get; set; }
    }
}
