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
    
    public partial class SP_Get_IssueNote_Items_Result
    {
        public long Id { get; set; }
        public Nullable<long> Item_Id { get; set; }
        public Nullable<long> Group_Id { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<long> Prj_Id { get; set; }
        public string Prj_Name { get; set; }
        public string Status { get; set; }
        public Nullable<long> Issue_To { get; set; }
        public string IssueTo_Name { get; set; }
        public Nullable<System.DateTime> IssuedTo_Date { get; set; }
        public Nullable<long> Requested_By { get; set; }
        public string RequestedBy_Name { get; set; }
        public Nullable<System.DateTime> Requested_Date { get; set; }
        public Nullable<long> Issue_By { get; set; }
        public string IssueBy_Name { get; set; }
        public Nullable<System.DateTime> IssuedBy_Date { get; set; }
        public Nullable<long> DemandOrder_Id { get; set; }
        public string IssueNote_No { get; set; }
        public Nullable<long> Dep_Id { get; set; }
        public string Dep_Name { get; set; }
        public Nullable<long> Warehouse_Id { get; set; }
        public string Warehouse_Name { get; set; }
        public string PO_Number { get; set; }
        public Nullable<long> PO_Id { get; set; }
        public Nullable<bool> Supervise { get; set; }
        public Nullable<long> SuperviseBy_Id { get; set; }
        public Nullable<long> SuperviseBy_Name { get; set; }
        public Nullable<System.DateTime> SuperviseBy_Date { get; set; }
        public Nullable<int> Comp_Id { get; set; }
        public string Company_Name { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> Cancelled { get; set; }
        public string Issuer_Name { get; set; }
        public string Received_By { get; set; }
        public string Complete_Name { get; set; }
        public string UOM { get; set; }
        public string SKU { get; set; }
    }
}
