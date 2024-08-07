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
    
    public partial class SP_Get_PO_Full_Detail_Result
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
        public Nullable<long> Bid_Id { get; set; }
        public string PO_Number { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<decimal> Purchase_Rate { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<long> Item_Id { get; set; }
        public Nullable<bool> StockEntered { get; set; }
        public Nullable<bool> IsStoreInitiated { get; set; }
        public string L_UOM { get; set; }
        public string W_UOM { get; set; }
        public string H_UOM { get; set; }
        public string D_UOM { get; set; }
        public string S_UOM { get; set; }
        public string Size { get; set; }
        public Nullable<long> Dep_Id { get; set; }
        public string Dep_Name { get; set; }
        public string CreatedBy_Desgi { get; set; }
        public string Vendor_Comp { get; set; }
        public Nullable<System.DateTime> Summary_Date { get; set; }
        public Nullable<bool> Summary_Gen { get; set; }
        public Nullable<long> Prj_Id { get; set; }
        public string Prj_Name { get; set; }
        public Nullable<bool> Prj_Offsite { get; set; }
        public string HOD_Name { get; set; }
        public string HOD_Design { get; set; }
        public Nullable<bool> OtherCharges { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public string Currency { get; set; }
        public Nullable<int> Comp_Id { get; set; }
        public string Company_Name { get; set; }
        public Nullable<System.DateTime> Deliver_Date { get; set; }
        public Nullable<bool> Cancelled { get; set; }
        public Nullable<long> CancelledBy { get; set; }
        public string CancelReason { get; set; }
        public string PaymentTime { get; set; }
        public string Company { get; set; }
        public string SKU { get; set; }
        public string ItemDescription { get; set; }
    }
}
