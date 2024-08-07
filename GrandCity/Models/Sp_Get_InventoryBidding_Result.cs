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
    
    public partial class Sp_Get_InventoryBidding_Result
    {
        public long Id { get; set; }
        public string Item_Name { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Heigth { get; set; }
        public Nullable<decimal> Diameter { get; set; }
        public string UOM { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<long> Group_Id { get; set; }
        public Nullable<long> Vendor_Id { get; set; }
        public string Vendor_Name { get; set; }
        public Nullable<long> Created_By { get; set; }
        public string CreatedBy_Name { get; set; }
        public string Description { get; set; }
        public Nullable<long> PurchaseReq_Id { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<decimal> PurchaseRate { get; set; }
        public Nullable<bool> IsFinalized { get; set; }
        public Nullable<long> Item_Id { get; set; }
        public string L_UOM { get; set; }
        public string W_UOM { get; set; }
        public string H_UOM { get; set; }
        public string D_UOM { get; set; }
        public string S_UOM { get; set; }
        public string Size { get; set; }
        public string Quote_Img { get; set; }
        public Nullable<long> Dep_Id { get; set; }
        public string Dep_Name { get; set; }
        public string CreatedBy_Designation { get; set; }
        public Nullable<bool> PO_Generated { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public string Currency { get; set; }
        public Nullable<int> Comp_Id { get; set; }
        public string Company_Name { get; set; }
        public Nullable<System.DateTime> Deliver_Date { get; set; }
        public Nullable<bool> OtherCharges { get; set; }
        public string PaymentTime { get; set; }
    }
}
