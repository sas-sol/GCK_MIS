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
    
    public partial class Sp_Get_InventoryList_All_Result
    {
        public long Id { get; set; }
        public string Company { get; set; }
        public string Item_Name { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Heigth { get; set; }
        public string Diameter { get; set; }
        public string Size { get; set; }
        public string UOM { get; set; }
        public Nullable<decimal> Minimum_Qty { get; set; }
        public decimal Total_In_Qty { get; set; }
        public decimal Total_Out_Qty { get; set; }
        public string Category_Name { get; set; }
        public string NameWithProps { get; set; }
    }
}
