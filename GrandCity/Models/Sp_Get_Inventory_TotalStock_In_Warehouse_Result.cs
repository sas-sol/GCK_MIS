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
    
    public partial class Sp_Get_Inventory_TotalStock_In_Warehouse_Result
    {
        public long Id { get; set; }
        public string SKU { get; set; }
        public string Complete_Name { get; set; }
        public string UOM { get; set; }
        public decimal Total_In_Qty { get; set; }
        public Nullable<int> Warehouse_Id { get; set; }
        public string WarehouseName { get; set; }
    }
}
