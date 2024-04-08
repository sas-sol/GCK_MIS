using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using MeherEstateDevelopers.Models;

namespace MeherEstateDevelopers.Models
{
    public class Stat
    {
        public long Status { get; set; }
        public int Total { get; set; }
    }
    public class SherAlamStats
    {
        public int Pending { get; set; }
        public int Registered { get; set; }
        public int Cancelled_Application { get; set; }
        public int Temporary_Cancelled { get; set; }
    }
    public class DealerLedger
    {
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public decimal? am { get; set; }
    }
    public class Getresult
    {
        public decimal? Amount { get; set; }
        public string PlotNo { get; set; }
        public string Block { get; set; }
        public string Sector { get; set; }
        public string PlotType { get; set; }

    }
    public partial class Sp_Get_SalesReport_Result
    {
        public string Booking_Date_String { get; set; }
    }
        public class JqueryDatatableParam
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortCol_0 { get; set; }
        public string sSortDir_0 { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
    }
    public class JournalEntry
    {
        public List<JournalEntries_Parameter> JE { get; set; }
        public List<Voucher> Previous_Vouchers { get; set; }
        public Voucher PV { get; set; }
    }
    public class GeneralEntryData
    {
        public DateTime DATE { get; set; }
        public string NARRATION { get; set; }
        public string DEBIT_ACCOUNT { get; set; }
        public string CREDIT_ACCOUNT { get; set; }
        public string INST_NO { get; set; }
        public DateTime? INST_DATE { get; set; }
        public decimal? DEBIT { get; set; }
        public decimal? CREDIT { get; set; }


    }
    public class Lines
    {
        public int payee_id { get; set; }
        public string payee_name { get; set; }
        public string narration { get; set; }
        public decimal? amount { get; set; }
    }

    public class JournalEntries_Parameter
    {
        public long Id { get; set; }
        public string Naration { get; set; }
        public string Head_Code { get; set; }
        public string Head_Name { get; set; }
        public Nullable<int> Head_Id { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<long> GroupId { get; set; }
        public System.DateTime? RecordedBy_Date { get; set; }
        public string RecordedBy_Name { get; set; }
        public Nullable<System.DateTime> Supviseby_Date { get; set; }
        public string Supviseby_Name { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string UOM { get; set; }
        public string Inst_No { get; set; }
        public Nullable<System.DateTime> Inst_Date { get; set; }
        public string Inst_Bank { get; set; }
        public string Voucher_No { get; set; }
        public string Voucher_Type { get; set; }
        public string MainAccount { get; set; }
        public string Paid_By { get; set; }

    }
    public partial class Sp_Get_CA_OpeningClosing_Balance_Result
    {
        public string MainAccount { get; set; }
    }
    public partial class Sp_Get_CA_Head_Result
    {
        public string MainAccount { get; set; }
    }
    public class DealersPlots
    {
        public int Plan_Id { get; set; }
        public long Plot_Id { get; set; }
    }
    public class FilePlotReport
    {
        public List<Sp_Get_FilePlot_NDC_WDC_TBA_Report_Result> all { get; set; }
        public List<Sp_Get_FilePlot_NDC_WDC_TBA_Report_Result> NDC { get; set; }
        public List<Sp_Get_FilePlot_NDC_WDC_TBA_Report_Result> WDC { get; set; }
        public List<Sp_Get_FilePlot_NDC_WDC_TBA_Report_Result> TBA { get; set; }

    }
    public class PaymentTypeModel
    {
        public string PaymentStatus { get; set; }
        public string VoucherType { get; set; }
        public Sp_Get_COA_Head_Code_Result PaymentData { get; set; }
    }
    public partial class Sp_Get_CancelFilesReport_Result
    {
        public decimal? Deduction { get; set; }
        public decimal? Payable { get; set; }
    }
    public class Overdue_CancelReport
    {
        public List<Sp_Get_CancelFilesReport_Result> CancelFiles { get; set; }
        public List<Sp_Get_Report_OverDue_Result> OverdueFiles { get; set; }
    }
    public partial class Sp_Get_CancelPlotsReport_Result
    {
        public decimal? Deduction { get; set; }
        public decimal? Payable { get; set; }
    }
    public class Overdue_CancelReport_Plots
    {
        public List<Sp_Get_CancelPlotsReport_Result> CancelPlots { get; set; }
        public List<Sp_Get_Report_OverDue_Plots_Result> OverduePlots { get; set; }
    }
    public partial class Inventory_Demand_Req
    {
        public string Type { get; set; }
    }
    public partial class Sp_Get_Report_Balloting_Est_Result
    {
        public DateTime? Date { get { return new DateTime(Convert.ToInt16(Year), Convert.ToInt16(Month), 1); } }
    }
    public partial class PO_Completed_With_GRN
    {
        public List<Sp_Get_PO_Completed_Result> PO { get; set; }
        public List<Sp_Get_PO_Completed_GRN_Result> GRN { get; set; }
        public List<DeptsModel> REQ { get; set; }
    }
    public partial class PO_Completed_With_GRN
    {
        public List<Sp_Get_Purchase_Order_List_Result> PO_list { get; set; }
    }
    public partial class Sp_Get_PO_Completed_Result
    {
        public decimal? Total { get { return Purchase_Rate * T_Qty; } }
    }
    public partial class Sp_Get_PurchaseOrder_Date_Result
    {
        public decimal? TotalVal { get { return Purchase_Rate * Qty; } }
    }
    public partial class Sp_Get_Reports_PlotOutstandingByBlock_Result
    {
        public decimal? Remaining { get { return total - Received - Discount; } }
    }
    public partial class Sp_Get_Reports_FilesOutstandingByBlock_Result
    {
        public decimal? Remaining { get { return total - Received - Discount; } }
    }
    public partial class Sp_Get_InventoryList_Dep_Result
    {
        public decimal? Remaining { get { return Total_In_Qty - Total_Out_Qty; } }
    }
    public partial class Sp_Get_Inventory_TotalStock_In_Result
    {
        public decimal? Total_Out_Qty { get; set; }
        public decimal? Remaining { get { return Total_In_Qty - Total_Out_Qty; } }
    }
    public partial class SP_GET_Applied_Quotes_For_PR_Group_Result
    {
        public decimal? TotalVal { get { return BidQty * Bid_Pur_Rate + Tax; } }
    }
    public partial class SP_GET_Applied_Quotes_For_PR_List_Result
    {
        public decimal? TotalVal { get { return BidQty * Bid_Pur_Rate + Tax; } }
    }
    public partial class Sp_GET_Applied_Quotes_For_SR_Group_Result
    {
        public decimal? TotalVal { get { return Bid_Pur_Rate + Tax; } }
    }
    //public partial class SP_GET_Applied_Quotes_For_SR_List_Result
    //{
    //    public decimal? TotalVal { get { return BidQty * Bid_Pur_Rate + Tax; } }
    //}
    public partial class Sp_Get_Inventory_PurchaseOrder_list_Result
    {
        public decimal? TotalVal { get { return (Purchase_Rate * Qty) + Tax; } }
        public string SKU { get; set; }
        public string Requistion_No { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public bool? Asset { get; set; }
        public string Project_Department { get; set; }
    }
    public partial class Inventory_PurchaseOrder
    {
        public decimal? TotalVal { get { return (Purchase_Rate * Qty) + Tax; } }
        public string SKU { get; set; }
        public string Requistion_No { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public bool? Asset { get; set; }
        public string Project_Department { get; set; }
    }
    public class EntryInvoice
    {
        public List<Sp_Get_GeneralEntries_Parameter_Result> General_Entries { get; set; }
        public List<Invoices_Files> Files { get; set; }
    }
    public class PayableVouchers
    {
        public List<Sp_Get_GeneralEntries_Parameter_Result> General_Entries { get; set; }
        public List<Bill> BillItems { get; set; }
        public Sp_Get_CA_Head_Parameter_Result vendor { get; set; }
    }
    public class ReceivableVouchers
    {
        public List<Sp_Get_GeneralEntries_Parameter_Result> General_Entries { get; set; }
        public List<Invoice> InvoiceItems { get; set; }
        public Sp_Get_CA_Head_Parameter_Result vendor { get; set; }
    }
    public class DealsModelEdit
    {
        public List<Biding_Reserve_Plots> DealsList { get; set; }
        public DealerDeal Deal { get; set; }
    }
    public class InventoryTemplate
    {
        public long Id { get; set; }
        public string Item_Code { get; set; }
        public string Item_Name { get; set; }
        public string Dep { get; set; }
        public long? Dep_Id { get; set; }
        public long Stock_Id { get; set; }
        public decimal? Qty { get; set; }
        public long? Ware_Id { get; set; }
        public string Warehouse_Name { get; set; }

    }
    public partial class Inventory_Stock_In
    {
        public decimal? PropotionateVal { get; set; }
        public string Item_name { get; set; }
        public string UOM { get; set; }
        public long ShelfId { get; set; }
        public string ShelfName { get; set; }
        public string SKU { get; set; }
    }
    public class LeadsFile
    {
        public int Sr { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Mobile { get; set; }
        public string Source { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
    public class UsersDep
    {
        public long Userid { get; set; }
        public string Name { get; set; }
        public long Dep_Id { get; set; }
        public string Dep_Name { get; set; }
    }
    public partial class Sp_Get_Inventory_Availability_Parameter_Result
    {
        public decimal Remaining { get { return Total_In_Qty - Total_Out_Qty; } }
    }
    public partial class Sp_Get_Inventory_Availability_Dep_Parameter_Result
    {
        public decimal Remaining { get { return Total_In_Qty - Total_Out_Qty; } }
    }


    //public partial class Sp_Get_DemandOrder_Details_Result
    //{
    //    public long Warehouse_Id { get; set; }
    //    public string Warehouse_Name { get; set; }
    //    public decimal Total_Stock_In { get; set; }
    //    public decimal Total_Stock_Out { get; set; }
    //    public decimal Remaining { get { return Total_Stock_In - Total_Stock_Out; } }
    //}
    public partial class Sp_Get_DemandOrder_Details_Result
    {
        public List<WarehouseModel> Warehouse_Rep { get; } = new List<WarehouseModel>();
    }
    public class WarehouseModel
    {
        public long? Warehouse_Id { get; set; }
        public string Warehouse_Name { get; set; }
        public decimal? Total_Stock_In { get; set; }
        public decimal? Total_Stock_Out { get; set; }
        public decimal? Remaining { get { return Total_Stock_In - Total_Stock_Out; } }
    }


    public class SalaryModelListing
    {
        public List<Sp_Get_Last_Month_Salary_Report_Result> PreviusNotGenerated { get; set; }
        public List<BasicSalaryComparison> PreviousBasicSalaryvariance { get; set; }
    }
    public class Prj_Financial_Summary
    {
        public Nullable<decimal> Prj_Budget { get; set; }
        public Nullable<decimal> Actual_Budget { get; set; }
        public Nullable<decimal> Budget_Diff { get; set; }
        public Nullable<decimal> Payment_Requests { get; set; }

    }
    public class MTS_MTSV
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public Nullable<long> Dep_Id { get; set; }

        public string Title_R { get; set; }
        public string Description_R { get; set; }
        public string Type_R { get; set; }
        public string Unit_R { get; set; }
        public Nullable<decimal> Qty_R { get; set; }
        public Nullable<decimal> Rate_R { get; set; }
        public Nullable<decimal> Amount_R { get; set; }
        public Nullable<System.DateTime> Start_Date_R { get; set; }
        public Nullable<System.DateTime> Deadline_R { get; set; }
        public Nullable<long> Dep_Id_R { get; set; }


        public Nullable<long> Task_CreatedBy { get; set; }
        public string Task_CreatedBy_Name { get; set; }
        public string Current_Status { get; set; }
    }
    public class InstallmentPlanUpdation
    {
        public DateTime AdvanceDate { get; set; }
        public DateTime InsatallmentStartingDate { get; set; }
        public DateTime BalltionDate { get; set; }
        public DateTime DevelopmentDate { get; set; }
        public decimal DevelopmentAmount { get; set; }
        public decimal AdvacneAmount { get; set; }
        public decimal InstallmentAmountPerMonth { get; set; }
        public decimal BallotinAmount { get; set; }
        public int NoOfInstallments { get; set; }
        public long FileId { get; set; }
        public long Id { get; set; }
        public string Installment_Type { get; set; }
    }
    public class ServiceChargesBill
    {
        public string Bill_No { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal? Arrears { get; set; }
        public decimal? Amount_Paid { get; set; }
        public decimal? After_Due_Date_Amount { get; set; }
        public decimal? FineAmount { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Total_Amount { get; set; }
        public decimal? Net_Total { get; set; }
        public decimal? Fine_Amount { get; set; }
        public decimal? Due_Date_Amount { get; set; }
        public DateTime Month { get; set; }
        public DateTime? Due_Date { get; set; }
        public string Block_Name { get; set; }
        public string Plot_No { get; set; }
        public string Plot_Type { get; set; }
        public long Current_Reading { get; set; }
        public string Meter_No { get; set; }
        public string Plot_Size { get; set; }
        public long Previous_Reading { get; set; }
        public int Units { get; set; }
        public int? Fine_Percentage { get; set; }
        public long Plot_Id { get; set; }
        public List<Service_Charges_Details> BillDeta { get; set; }
        public List<ServiceChargesBill> ElectricityData { get; set; }
        public List<ServiceChargesBill> ServiceChargesData { get; set; }
    }


    public class BasicSalaryComparison
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> Salary_Month { get; set; }
        public Nullable<decimal> Basic_salary { get; set; }
        public Nullable<int> COLA_Percentage { get; set; }
        public Nullable<decimal> COLA_Amount { get; set; }
        public string COLA_Description { get; set; }
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
        public string Bonus_Description { get; set; }
        public Nullable<int> Absentee { get; set; }
        public Nullable<int> Salary_Leaves { get; set; }
        public Nullable<int> No_Of_Days { get; set; }
        public Nullable<decimal> Allownces { get; set; }
        public Nullable<decimal> Bonus { get; set; }
        public Nullable<decimal> Extra_Fuel_Charges { get; set; }
        public Nullable<int> Extra_Working_Days { get; set; }
        public Nullable<decimal> Overtime { get; set; }
        public Nullable<decimal> Ded_Loan { get; set; }
        public Nullable<decimal> Ded_Advance { get; set; }
        public Nullable<decimal> Ded_Tax { get; set; }
        public Nullable<decimal> Ded_Other { get; set; }
        public Nullable<decimal> Ded_Absentee { get; set; }
        public string CNIC { get; set; }
        public Nullable<decimal> Grand_total { get; set; }
        public Nullable<int> Extra_Fuel { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<long> Designation_Id { get; set; }
        public string Designation_name { get; set; }
        public Nullable<decimal> Previous_Basic_Salary { get; set; }

    }
    public class Payment_Voucher_Req
    {
        public long Prj_Id { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public decimal? Total { get; set; }
        public string Description { get; set; }
        public string Milestone { get; set; }
        public long Mile_Id { get; set; }
    }

    public class PlotsByType
    {
        public List<Sp_Get_Plotlist_Block_Result> Residential { set; get; }
        public List<Sp_Get_Plotlist_Block_Result> Commercial { set; get; }
    }
    public class TransferLetter
    {
        public long? Id { get; set; }
        public long? Owner_Id { get; set; }
        public string Plot_No { get; set; }
        public string Block { get; set; }
        public string Size { get; set; }
        public string Transfer_From { get; set; }
        public string T_CNIC { get; set; }
        public string T_Mobile1 { get; set; }
        public string Name { get; set; }
        public string CNIC_NICOP { get; set; }
        public string Mobile_1 { get; set; }
        public decimal? Total { get; set; }
        public decimal? Received { get; set; }
        public DateTime? Transfer_Date { get; set; }
        public bool IsCompanyProperty { get; set; }
        public decimal? Discount { get; set; }
        public string T_Address { get; set; }
        public string Address { get; set; }
        public string T_City { get; set; }
        public string City { get; set; }
        public string Phase { get; set; }
       
    }

    public class PropertyDeal
    {
        public List<Property_Deal_Parties> Parties { get; set; }
        public List<PropertyDeal_Receipts> Receipts { get; set; }
    }
    public class HierarchicalNode
    {
        public List<HierarchicalNode> Decendants { get; } = new List<HierarchicalNode>();
        public string Head { get; }
        public string Code { get; }
        public int Id { get; }

        public HierarchicalNode(string Head, string Code, int id)
        {
            this.Head = Head;
            this.Code = Code;
            this.Id = id;
        }
    }

    public class VendorVoucher
    {
        public Voucher Voucher { get; set; }
        public List<Voucher_Details> Details { get; set; }
    }
    public class VendorPayable
    {
        public Voucher Voucher { get; set; }
        public List<Bill> Details { get; set; }
        public Payable paydet { get; set; }
        public List<Voucher> VoucherList { get; set; }
    }
    public class Voucher_Details_General_Entries
    {
        public string Item_Name { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public string UOM { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Account_Head { get; set; }
        public string Account_Head_Code { get; set; }
        public int Account_Head_Id { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<int> Line { get; set; }
    }
    public class PlotNOC
    {
        public long Plot_Id { get; set; }
        public string Name { get; set; }
        public string Plot_No { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Phase { get; set; }
        public string Serial_No { get; set; }
        public System.DateTime DateTime { get; set; }
        public long Userid { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal DueAmt { get; set; }
        public string Block { get; set; }
        public bool? Registery { get; set; }
        public bool? Mortgage { get; set; }
        public int? Dup_Allotment_Letter { get; set; }
    }
    public class CommercialNOC
    {
        public long Com_Id { get; set; }
        public string Name { get; set; }
        public string Com_No { get; set; }
        public string Floor { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Serial_No { get; set; }
        public System.DateTime DateTime { get; set; }
        public long Userid { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal DueAmt { get; set; }
    }
    public class OverdueQualifying_Plots
    {
        public long? Id { get; set; }
        public string Plot_No { get; set; }
        public string Type { get; set; }
        public string Road_Type { get; set; }
        public string Status { get; set; }
        public string Plot_Size { get; set; }
        public string Plot_Location { get; set; }
        public Nullable<bool> Verified { get; set; }
        public string Dimension { get; set; }
        public Nullable<decimal> Area { get; set; }
        public string Develop_Status { get; set; }
        public string Block_Name { get; set; }
        public string Name { get; set; }
        public string Mobile_1 { get; set; }
        public bool? Verification_Req { get; set; }
        public long? Owner_Id { get; set; }
        public decimal? OverDueAmount { get; set; }
        public int? Installments { get; set; }
        public DateTime? FirstNotice { get; set; }
        public DateTime? SecNotice { get; set; }
        public DateTime? Third_Notice { get; set; }
        public DateTime? CancelNotice { get; set; }
        public string Postal_Address { get; set; }
        public string Currency_Note_No { get; set; }
    }


    public class OverdueQualifying_Shops
    {
        public long Id { get; set; }
        public string Shop_No { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Shop_Location { get; set; }
        public Nullable<bool> Verified { get; set; }
        public string Dimension { get; set; }
        public Nullable<decimal> Area { get; set; }
        public string Floor { get; set; }
        public string Project { get; set; }
        public string Name { get; set; }
        public string Mobile_1 { get; set; }
        public bool? Verification_Req { get; set; }
        public long? Owner_Id { get; set; }
        public int? Installments { get; set; }
        public decimal? OverDueAmount { get; set; }
        public DateTime? FirstNotice { get; set; }
        public DateTime? SecNotice { get; set; }
        public DateTime? CancelNotice { get; set; }
    }
    public class ArrearSalaryDetail
    {
        public Sp_Get_Arrears_Parameter_Result Details { get; set; }
        public List<Sp_Get_Arrear_Salary_Details_Result> ArrearDetail { get; set; }
    }
    public class DateSearch
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public class FileDataForReport
    {
        public long? FileFormNumber { get; set; }
        public string Plot_Size { get; set; }
        public decimal? PaidAountTillDate { get; set; }
        public decimal? TotalPlotAmount { get; set; }
    }
    public class BidingFiles
    {
        public long? FormNumber { get; set; }
        public string Plot_Size { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public bool Stat { get; set; }
        public long Id { get; set; }
    }
    public class DealModel
    {
        public Property_Deal Deal { get; set; }
        public List<Sp_Get_PropertyDeals_Parties_Result> Parties { get; set; }
        public List<PropertyDeal_Receipts> Receipts { get; set; }
        public List<PropertyDeal_Voucher> Voucher { get; set; }
        public List<DealLedger> Ledger { get; set; }
        public List<Sp_Get_PropertyDeal_Request_Deal_Result> Request { get; set; }
    }
    public class DealLedger
    {
        public string Plot_number { get; set; }
        public string Block { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Buyer -- Seller
        public string Receipt { get; set; }
        public string Voucher { get; set; }
        public decimal? Amount { get; set; }
        public string ReceiptNo { get; set; }
        public string Status { get; set; }
        public string PaymentType { get; set; }
        public DateTime? Date { get; set; }
    }

    public class PlotSigned
    {
        public string PlotNumber { get; set; }
        public string Old_PlotNumber { get; set; }
        public string Plot_Size { get; set; }
        public string Name { get; set; }
    }
    public class InstallmentReceived_Expected
    {
        public decimal? Expected { get; set; }
        public decimal? Received { get; set; }
        public string Date { get; set; }
    }
    public class InstallmentData
    {
        public string FileNumber { get; set; }
        public long File_Id { get; set; }
        public List<long> Ids { get; set; }
    }
    public class ActivitiesList
    {
        public long Id { get; set; }
        public long Userid { get; set; }
        public string Description { get; set; }
        public string Activity { get; set; }
        public string Module { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public string Activity_Type { get; set; }
        public string Name { get; set; }
    }
    public class DailyCashRep
    {
        public List<DailyCashierUser_Report> CancelledReceipts { get; set; }
        public List<DailyCashierUser_Report> FileCollection { get; set; }
        public decimal? File_Cash { get; set; }
        public decimal? File_Cheque { get; set; }
        public decimal? File_PayOrder { get; set; }
        public decimal? File_BankDraft { get; set; }
        public decimal? File_Online { get; set; }
        public decimal? File_Total { get; set; }
        public List<DailyCashierUser_Report> PlotCollection { get; set; }
        public decimal? Plot_Cash { get; set; }
        public decimal? Plot_Cheque { get; set; }
        public decimal? Plot_PayOrder { get; set; }
        public decimal? Plot_BankDraft { get; set; }
        public decimal? Plot_Online { get; set; }
        public decimal? Plot_Total { get; set; }

        //////////////
        public List<DailyCashierUser_Report> CommercialCollection { get; set; }
        public decimal? Com_Cash { get; set; }
        public decimal? Com_Cheque { get; set; }
        public decimal? Com_PayOrder { get; set; }
        public decimal? Com_BankDraft { get; set; }
        public decimal? Com_Online { get; set; }
        public decimal? Com_Total { get; set; }

        /// <summary>
        /// //////////////////
        /// </summary>
        public List<DailyCashierUser_Report> ServiceChargesCollection { get; set; }
        public decimal? Ser_Cash { get; set; }
        public decimal? Ser_Cheque { get; set; }
        public decimal? Ser_PayOrder { get; set; }
        public decimal? Ser_BankDraft { get; set; }
        public decimal? Ser_Online { get; set; }
        public decimal? Ser_Total { get; set; }
        /// <summary>
        /// //////////////////////
        /// </summary>
        public List<DailyCashierUser_Report> OtherCollection { get; set; }
        public decimal? Other_Cash { get; set; }
        public decimal? Other_Cheque { get; set; }
        public decimal? Other_PayOrder { get; set; }
        public decimal? Other_BankDraft { get; set; }
        public decimal? Other_Online { get; set; }
        public decimal? Other_Total { get; set; }
        /// <summary>
        /// ////////////
        /// </summary>
        public List<DailyCashierUser_Report> Advance_236_C_Collection { get; set; }
        public decimal? Ad_C_Cash { get; set; }
        public decimal? Ad_C_Cheque { get; set; }
        public decimal? Ad_C_PayOrder { get; set; }
        public decimal? Ad_C_BankDraft { get; set; }
        public decimal? Ad_C_Online { get; set; }
        public decimal? Ad_C_Total { get; set; }
        /// <summary>
        /// ///////////////////////////
        /// </summary>
        public List<DailyCashierUser_Report> Advance_236_K_Collection { get; set; }
        public decimal? Ad_K_Cash { get; set; }
        public decimal? Ad_K_Cheque { get; set; }
        public decimal? Ad_K_PayOrder { get; set; }
        public decimal? Ad_K_BankDraft { get; set; }
        public decimal? Ad_K_Online { get; set; }
        public decimal? Ad_K_Total { get; set; }
        /// //////////////
        public List<DailyCashierUser_Report> FileTransferCollection { get; set; }
        public decimal? Ft_Cash { get; set; }
        public decimal? Ft_Cheque { get; set; }
        public decimal? Ft_PayOrder { get; set; }
        public decimal? Ft_BankDraft { get; set; }
        public decimal? Ft_Online { get; set; }
        public decimal? Ft_Total { get; set; }

        ////////////
        public List<DailyCashierUser_Report> PlotsTransferCollection { get; set; }
        public decimal? Pt_Cash { get; set; }
        public decimal? Pt_Cheque { get; set; }
        public decimal? Pt_PayOrder { get; set; }
        public decimal? Pt_BankDraft { get; set; }
        public decimal? Pt_Online { get; set; }
        public decimal? Pt_Total { get; set; }

        ////////////
        public List<DailyCashierUser_Report> CommercialTransferCollection { get; set; }
        public decimal? Ct_Cash { get; set; }
        public decimal? Ct_Cheque { get; set; }
        public decimal? Ct_PayOrder { get; set; }
        public decimal? Ct_BankDraft { get; set; }
        public decimal? Ct_Online { get; set; }
        public decimal? Ct_Total { get; set; }

        ////////////
        public List<DailyCashierUser_Report> Payments { get; set; }
        public List<DailyCashierUser_Report> ArchiFees { get; set; }
        public decimal? Pay_Cash { get; set; }
        public decimal? Pay_Cheque { get; set; }
        public decimal? Pay_PayOrder { get; set; }
        public decimal? Pay_BankDraft { get; set; }
        public decimal? Pay_Online { get; set; }
        public decimal? Pay_Total { get; set; }

        ////////////
        public decimal? Cash { get; set; }
        public decimal? Cheque { get; set; }
        public decimal? PayOrder { get; set; }
        public decimal? BankDraft { get; set; }
        public decimal? Online { get; set; }
        public decimal? Total { get; set; }


        ////////////
        public decimal? Grand_Cash { get; set; }
        public decimal? Grand_Cheque { get; set; }
        public decimal? Grand_PayOrder { get; set; }
        public decimal? Grand_BankDraft { get; set; }
        public decimal? Grand_Online { get; set; }
        public decimal? Grand_Total { get; set; }


    }
    public class DailySAMCashRep
    {
        public List<DailyCashierUser_Report> FileCollection { get; set; }
        public decimal? File_Cash { get; set; }
        public decimal? File_Cheque { get; set; }
        public decimal? File_PayOrder { get; set; }
        public decimal? File_BankDraft { get; set; }
        public decimal? File_Online { get; set; }
        public decimal? File_Total { get; set; }
        public List<DailyCashierUser_Report> PlotCollection { get; set; }
        public decimal? Plot_Cash { get; set; }
        public decimal? Plot_Cheque { get; set; }
        public decimal? Plot_PayOrder { get; set; }
        public decimal? Plot_BankDraft { get; set; }
        public decimal? Plot_Online { get; set; }
        public decimal? Plot_Total { get; set; }

        ////////////
        public decimal? Cash { get; set; }
        public decimal? Cheque { get; set; }
        public decimal? PayOrder { get; set; }
        public decimal? BankDraft { get; set; }
        public decimal? Online { get; set; }
        public decimal? Total { get; set; }

        public List<DailyCashierUser_Report> FileVoucher { get; set; }
        public decimal? V_File_Cash { get; set; }
        public decimal? V_File_Cheque { get; set; }
        public decimal? V_File_PayOrder { get; set; }
        public decimal? V_File_BankDraft { get; set; }
        public decimal? V_File_Online { get; set; }
        public decimal? V_File_Total { get; set; }
        public List<DailyCashierUser_Report> PlotVoucher { get; set; }
        public decimal? V_Plot_Cash { get; set; }
        public decimal? V_Plot_Cheque { get; set; }
        public decimal? V_Plot_PayOrder { get; set; }
        public decimal? V_Plot_BankDraft { get; set; }
        public decimal? V_Plot_Online { get; set; }
        public decimal? V_Plot_Total { get; set; }
        ////////////
        public decimal? V_Cash { get; set; }
        public decimal? V_Cheque { get; set; }
        public decimal? V_PayOrder { get; set; }
        public decimal? V_BankDraft { get; set; }
        public decimal? V_Online { get; set; }
        public decimal? V_Total { get; set; }

    }

    public class SmsModel
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }
    public class ReceiptModel
    {
        public string ReceiptNo { get; set; }
        public long? ReceiptId { get; set; }
        public string Type { get; set; }
        public string Module { get; set; }
    }
    public class TransferRequestDetails
    {
        public FileData File { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> ReceivedAmounts { get; set; }
        public List<Sp_Get_FileInstallments_Result> Installments { get; set; }
        public List<Sp_Get_FileInstallments_Result> OtherInstallments { get; set; }
        public List<Sp_Get_TransferRequestDetails_File_Result> TransferTo { get; set; }
    }
    public class FollowupData
    {
        public long File_plot_number { get; set; }
        public long Userid { get; set; }
        public string Name { get; set; }
        public string Msg { get; set; }
        public string Type { get; set; }
    }
    public class DealersData
    {
        public string Dealership { get; set; }
        public DateTime? RegisterationDate { get; set; }
        public string QR_Code { get; set; }
        public List<Dealer> Dealers { get; set; }
    }
    public class DealersFilesData
    {
        public string PlotSize { get; set; }
        public bool? DevelopmentCharges { get; set; }
        public bool Security { get; set; }
        public string Phase { get; set; }
        public string Block { get; set; }
        public int? TotalFile { get; set; }
        public int? Cancelled { get; set; }
        public int? Register { get; set; }
        public int? Pending { get; set; }
    }
    public class FileInstallmentStatus
    {
        public bool Status { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public decimal Grand_Total { get; set; }
        public List<File_Installments> Installments { get; set; }
    }
    public partial class FilePlotArray
    {
        public string Bank { get; set; }
        public string From_Bank { get; set; }
        public string AmountInWords { get; set; }
        public decimal Amount { get; set; }
    }
    public class ReceiptData
    {
        public string ReceiptNumber { get; set; }
        public DateTime? Ch_bk_Pay_Date { get; set; }
        public string Plot_Size { get; set; }
        public string Block_Name { get; set; }
        public string Type { get; set; }
        public string Project_Name { get; set; }
        public string Name { get; set; }
        public string Father_Husband { get; set; }
        public string Mobile_1 { get; set; }
        public string AmountInWords { get; set; }
        public string PaymentType { get; set; }
        public string Bank { get; set; }
        public string PayChqNo { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public long File_Plot_Number { get; set; }
        public DateTime Date { get; set; }
        public string Branch { get; set; }
        public string ReceiptNo { get; set; }
        public string Phase { get; set; }
        public string Block { get; set; }
        public string DevCharges { get; set; }
        public string FilePlotNumber { get; set; }
        public string FileImage { get; set; }
        public string ModuleType { get; set; }
    }

    public class BookingReceiptData
    {
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string PayChqNo { get; set; }
        public DateTime? Ch_bk_Pay_Date { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
    }
    public class RecoveryReport
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public string Name { get; set; }
        public string File_Plot_No { get; set; }
        public decimal? Amount { get; set; }
        public string Bank { get; set; }
        public string Ch_Pay_Draft_No { get; set; }
        public DateTime? Ch_Pay_Draft_Date { get; set; }
        public string PaymentType { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Dealership_Name { get; set; }
        public string Project { get; set; }
        public string ReceiptNo { get; set; }
        public string Cashier_Name { get; set; }
        public string Contact { get; set; }
        public string Block { get; set; }
        public string Module { get; set; }
        public bool? Cancel { get; set; }
        public string LeadDeal { get; set; }
    }
    public class FileResults
    {
        public FileData File { get; set; }
        public List<Sp_Get_FileInstallments_Result> Installments { get; set; }
        public List<Sp_Get_FileInstallments_Result> OtherInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> ReceivedAmounts { get; set; }
    }
    public class Subscribed_Service_Charges
    {
        public List<Sp_Get_ServiceChargesTypeList_Result> SCList { get; set; }
        public List<Sp_Get_PlotServiceChargesTypeList_Result> Plot_SCList { get; set; }
    }
    public class Subscribed_Charges
    {
        public List<Sp_Get_ServiceChargesTypeList_Result> SCList { get; set; }
        public List<Sp_Get_PlotServiceChargesTypeList_Result> Plot_SCList { get; set; }
        public List<Plot_ElectricityMeters> Plot_Meters { get; set; }
    }
    public class DealerFileForm
    {
        public long Id { get; set; }
        public long Phase { get; set; }
        public string Phase_Name { get; set; }
        public long Block { get; set; }
        public string Block_Name { get; set; }
        public long Dealership_Id { get; set; }
        public string Dealership_Name { get; set; }
        public string Sector { get; set; }
        public string Plot_Size { get; set; }
        public string Type { get; set; }
        public int Filecount { get; set; }
        public decimal? Commession { get; set; }
        public bool? Dev_Charges_Id { get; set; }
        public string Dev_Charges_Text { get; set; }
        public int Sec_NoSec_Id { get; set; }
        public string Sec_NoSec_Name { get; set; }
        public decimal Security { get; set; }
        public string FileCode { get; set; }
        public string QR_Code { get; set; }
        public long Group_Id { get; set; }
        public int Installment_Plan { get; set; }
        public long? userid { get; set; }

    }
    //public class DealershipDeals
    //{
    //    public List<Final_Deal_Plans> PlanDetails { get; set; }
    //    public List<Sp_Get_Ledger_Result> LedgerDetail { get; set; }
    //    public Sp_Get_Deal_Parameter_Result Dealer { get; set; }
    //}

    public class FileAppFormData
    {
        public long Id { get; set; }
        public string Plot_Size { get; set; }
        public string Development_Charges { get; set; }
        public bool? Development_Charges_Val { get; set; }
        public string Status { get; set; }
        public string Dealership_Name { get; set; }
        public long? Block_Id { get; set; }
        public string Block_Name { get; set; }
        public string Type { get; set; }
        public decimal Advance { get; set; }
        public decimal Total_Price { get; set; }
        public decimal? Discount { get; set; }
    }
    public class OwnerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Father_Husband { get; set; }
        public string Postal_Address { get; set; }
        public string Residential_Address { get; set; }
        public string Phone_Office { get; set; }
        public string Residential { get; set; }
        public string Mobile_1 { get; set; }
        public string Mobile_2 { get; set; }
        public string Email { get; set; }
        public string Occupation { get; set; }
        public string Nationality { get; set; }
        public string Date_Of_Birth { get; set; }
        public string CNIC_NICOP { get; set; }
        public string Nominee_Name { get; set; }
        public string Nominee_Relation { get; set; }
        public string Nominee_Address { get; set; }
        public string Nominee_CNIC_NICOP { get; set; }
        public string City { get; set; }
        public DateTime? DateTime { get; set; }
        public string Ownership_Status { get; set; }
        public long? FileId { get; set; }

    }
    public class AmountToPaidInfo
    {
        public long Id { get; set; }
        public string Installment_Name { get; set; }
        public decimal Amount { get; set; }
        public string Months { get; set; }
    }
    public class BanksModel
    {
        public string Name { get; set; }
    }
    public class NameValuestring
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
    public class InstallmentStatusModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
    public class QRCodeReturn
    {
        public long Id { get; set; }
        public string QR_Image { get; set; }
    }
    public class NameNumber
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }
    public class FileData
    {
        public long Id { get; set; }
        public long? File_Transfer_Id { get; set; }
        public string File_Form_Number { get; set; }
        public string File_Plot_Number { get; set; }
        public string Sector { get; set; }
        public long? Plot_Id { get; set; }
        public string Plot_Size { get; set; }
        public string Type { get; set; }
        public string Development_Charges { get; set; }
        public bool? Development_Charges_Val { get; set; }
        public string Status { get; set; }
        public string Dealership_Name { get; set; }
        public string Block_Name { get; set; }
        public long? Block_Id { get; set; }
        public string Phase_Name { get; set; }
        public long? Phase_Id { get; set; }
        public string Project_Name { get; set; }
        public string  QR_Id { get; set; }
        public string Name { get; set; }
        public string Father_Husband { get; set; }
        public string Postal_Address { get; set; }
        public string Residential_Address { get; set; }
        public string Phone_Office { get; set; }
        public string Residential { get; set; }
        public string Mobile_1 { get; set; }
        public string Mobile_2 { get; set; }
        public string Email { get; set; }
        public string Occupation { get; set; }
        public string Nationality { get; set; }
        public string Date_Of_Birth { get; set; }
        public string CNIC_NICOP { get; set; }
        public string Nominee_Name { get; set; }
        public string Nominee_Relation { get; set; }
        public string Nominee_Address { get; set; }
        public string Nominee_CNIC_NICOP { get; set; }
        public string City { get; set; }
        public decimal? Balance_Amount { get; set; }
        public decimal? Rate { get; set; }
        public decimal? TotalPlotVal { get; set; }
        public decimal? GrandTotal { get; set; }
        public bool? Delivery { get; set; }
        public string Plot_Prefrence { get; set; }
        public bool? Flag { get; set; }
        public bool? Verify { get; set; }
        public DateTime? Date_Reg { get; set; }
        public bool Full_Paid { get; set; }
        public bool? AuditVerified { get; set; }
        public bool? Verification_Req { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }

    }
    public class PlotsInstallments
    {
        public string InstNo { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
    public class InstallmentsReport
    {
        public List<Sp_Get_PlotInstallments_Result> Installments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> Receipts { get; set; }
    }
    public class ReportModel
    {
        public string Installments { get; set; }
        public DateTime? Due_Date { get; set; }
        public decimal? Amount { get; set; }
        public string Receipt { get; set; }
        public decimal? Recp_Amount { get; set; }
        public string Mode { get; set; }
        public string Chq_No { get; set; }
        public string Bank { get; set; }
        public DateTime? Recp_Date { get; set; }
        public DateTime? Date { get; set; }
        public decimal Bal { get; set; }
        public string Type { get; set; }
        public string Inst_Status { get; set; }
        public bool? Cancel { get; set; }
    }
    public class PlotStatment
    {
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? RcvdDate { get; set; }
        public string Receipt_Voucher_No { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public string Mode { get; set; }
        public string Chq_No { get; set; }
        public string Bank { get; set; }
        public string Type { get; set; }
        public string Inst_Status { get; set; }
        public bool? Cancel { get; set; }
    }

    public class EmpModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Father_Name { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> Date_Of_Birth { get; set; }
        public string Blood_Group { get; set; }
        public string Emergency_Contact { get; set; }
        public string Relationship { get; set; }
        public string Mobile_1 { get; set; }
        public string Mobile_2 { get; set; }
        public string Email { get; set; }
        public string Company_Email { get; set; }
        public string Marital_Status { get; set; }
        public string Image { get; set; }
        public string City { get; set; }
        public Nullable<long> loginId { get; set; }
        public string Department_Designation { get; set; }
        public string JobDescription { get; set; }
        public string Employee_Code { get; set; }
        public Nullable<System.DateTime> Date_Of_Joining { get; set; }
        public Nullable<long> Reporting_To { get; set; }
        public Nullable<long> Reporting_To_2 { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> Basic_Salary { get; set; }
        public Nullable<int> Active { get; set; }
        public int Installment_Values { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
    }
    public class PlotDetailData
    {
        public Sp_Get_PlotData_Result PlotData { get; set; }
        public List<Sp_Get_PlotOwnerList_Result> PlotOwners { get; set; }
        public List<Sp_Get_PlotInstallments_Result> PlotInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> PlotReceipts { get; set; }
        public List<Discount> Discounts { get; set; }
        public File_Plot_Balance PlotBalDets { get; set; }
        public Cancellation_Receipts Refunded_Repurchased { get; set; }
        public List<Plot_Installments_Surcharge> PlotInstallmentsSurcharge { get; set; }
        public List<Sp_Get_PlotInstallments_Wht_Result> PlotInstallmentsWHT { get; set; }
    }
    public class PlotCancelDetailData
    {
        public Sp_Get_PlotData_Result PlotData { get; set; }
        public List<Sp_Get_PlotOwnerList_Result> PlotOwners { get; set; }
        public List<Sp_Get_PlotInstallments_Result> PlotInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> PlotReceipts { get; set; }
        public Cancellation_Receipts Plot_Cancel { get; set; }
        public Plot_Cancelation_Req Plot_Req { get; set; }

    }
    public class PlotRefundDetailData
    {
        public Sp_Get_PlotData_Result PlotData { get; set; }
        public List<Sp_Get_PlotOwnerList_Result> PlotOwners { get; set; }
        public List<Sp_Get_PlotInstallments_Result> PlotInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> PlotReceipts { get; set; }
        public RefundAmountsReq Plot_Req { get; set; }

    }
    public class FileRefundDetailData
    {
        public Sp_Get_FileFormData_Result FileData { get; set; }
        public List<Sp_Get_FileOwnerList_Result> FileOwners { get; set; }
        public List<Sp_Get_FileInstallments_Result> FileInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> FileReceipts { get; set; }
        public RefundAmountsReq File_Req { get; set; }

    }
    public class FileCancelDetailData
    {
        public Sp_Get_FileAppFormData_Result FileData { get; set; }
        public List<Sp_Get_FileOwnerList_Result> FilesOwners { get; set; }
        public List<Sp_Get_FileInstallments_Result> FileInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> FileReceipts { get; set; }
        public Cancellation_Receipts File_Cancel { get; set; }
        public File_Cancelation_Req File_Req { get; set; }

    }
    public class FileDetailData
    {
        public Sp_Get_FileAppFormData_Result FileData { get; set; }
        public List<Sp_Get_FileOwnerList_Result> FilesOwners { get; set; }
        public List<Sp_Get_FileInstallments_Result> FileInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> FileReceipts { get; set; }
        public List<Installment_Structure> InstalmentStructureDetail { get; set; }
        public List<Plot_Installments_Surcharge> PlotInstallmentsSurcharge { get; set; }
        public List<Discount> Discounts { get; set; }
        public List<Voucher> PaymentVoucher { get; set; }
    }

    public class CustomerFileDetailData
    {
        public Sp_Get_FileAppFormData_Result FileData { get; set; }
        public List<Sp_Get_FileLastOwner_Result> FilesOwners { get; set; }
        public List<Sp_Get_FileInstallments_Result> FileInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> FileReceipts { get; set; }
        public List<Installment_Structure> InstalmentStructureDetail { get; set; }
        public List<Discount> Discounts { get; set; }
    }
    public class CommercialDetailData
    {
        public Sp_Get_CommercialData_Result commercialData { get; set; }
        public List<Sp_Get_CommercialOwnerDetail_Result> shopOwners { get; set; }
        public List<Sp_Get_CommercialLastOwner_Result> shopOwnersforallt { get; set; }
        public Commercial_Room_Transfer ShopOwner { get; set; }
        public List<Sp_Get_CommercialOwnerDetail_Result> shopOwnersMultiple { get; set; }
        public List<Sp_Get_CommercialInstallments_Result> CommercialInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> CommercialReceipts { get; set; }
        public List<Sp_Get_TranferRequests_Result> TranferRequtedData { get; set; }
        public List<Discount> Discounts { get; set; }
        public Cancellation_Receipts Plot_Cancel { get; set; }
        public Commercial_Cancelation_Req Plot_Req { get; set; }
    }

    public class CommercialDetailDataForInfo
    {
        public Sp_Get_CommercialData_Result commercialData { get; set; }
        public List<Sp_Get_CommercialOwnerDetail_Result> shopOwners { get; set; }
        public Sp_Get_CommercialLastOwner_Result shopOwnersforallt { get; set; }
        public Commercial_Room_Transfer ShopOwner { get; set; }
        public List<Sp_Get_CommercialOwners_Result> shopOwnersMultiple { get; set; }
        public List<Sp_Get_CommercialInstallments_Result> CommercialInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> CommercialReceipts { get; set; }
        public List<Sp_Get_TranferRequests_Result> TranferRequtedData { get; set; }
        public List<Discount> Discounts { get; set; }
        public Cancellation_Receipts Plot_Cancel { get; set; }
        public Commercial_Cancelation_Req Plot_Req { get; set; }
        public File_Plot_Balance Balance { get; set; }
    }
    public class LatestPlotDetailData
    {
        public Sp_Get_PlotData_Result PlotData { get; set; }
        public Sp_Get_PlotRecord_Parameter_Result PlotsData { get; set; }
        public List<Sp_Get_PlotLastOwner_Result> PlotOwners { get; set; }
        public List<Sp_Get_PlotInstallments_Result> PlotInstallments { get; set; }
        public List<Plot> PlotPosition { get; set; }
        public Plot Plots { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> PlotReceipts { get; set; }
        public List<Discount> Discounts { get; set; }
        public List<Plot_BondedBy> PlotBounding { get; set; }
        public List<Installment_Structure> InstalmentStructureDetail { get; set; }
        public decimal Plot_Price_DC_Rate { get; set; }
        public List<Plots_Transfer_Request> TransferPending { get; set; }
    }
    public class CommercialTransferRequestdata
    {
        public Sp_Get_CommercialData_Result UnitData { get; set; }
        public List<Sp_Get_CommercialLastOwner_Result> UnitOwners { get; set; }
        public List<Sp_Get_CommercialInstallments_Result> UnitInstalments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> UnitReceipts { get; set; }
        public List<Discount> UnitDiscount { get; set; }
        public CommercialTransferFeeModel UnitTransferFee { get; set; }
        public List<Commercial_Transfer_Request> UnitTransferRequest { get; set; }
        public File_Plot_Balance Balance { get; set; }
    }

    public class PlotTransferDetailData
    {
        public Sp_Get_PlotDetailData_Result PlotData { get; set; }
        public List<Sp_Get_PlotLastOwner_Result> PlotOwners { get; set; }
        public List<Sp_Get_PlotInstallments_Result> PlotInstallments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> PlotReceipts { get; set; }
        public List<Sp_Get_TransferRequestDetails_Plot_Result> TransferTo { get; set; }
    }
    public class PlotLedger
    {
        public Sp_Get_PlotData_Result PlotData { get; set; }
        public Sp_Get_PlotLastOwner_Result OwnerData { get; set; }
        public List<ReportModel> Report { get; set; }
        public Discount Discount { get; set; }
    }

    public class CommercialLedger
    {
        public Sp_Get_CommercialData_Result Data { get; set; }
        public Sp_Get_CommercialOwnerDetail_Result OwnerData { get; set; }
        public List<PlotStatment> Report { get; set; }
        public Discount Discount { get; set; }
        public File_Plot_Balance Balance { get; set; }
    }

    public class NewPlotLedger
    {
        public Sp_Get_PlotData_Result PlotData { get; set; }
        public List<Sp_Get_PlotOwnerList_Result> OwnerData { get; set; }
        public List<PlotStatment> Report { get; set; }
        public List<Discount> Discount { get; set; }
        public File_Plot_Balance Balance { get; set; }
        public Cancellation_Receipts Refunded_Repurchased { get; set; }
    }
    public class PlotLastOwnerLedger
    {
        public Sp_Get_PlotData_Result PlotData { get; set; }
        public List<Sp_Get_PlotLastOwner_Result> OwnerData { get; set; }
        public List<PlotStatment> Report { get; set; }
        public Discount Discount { get; set; }
    }

    public class CommercialLastOwnerLedger
    {
        public Sp_Get_CommercialData_Result Data { get; set; }
        public List<Sp_Get_PlotLastOwner_Result> OwnerData { get; set; }
        public List<PlotStatment> Report { get; set; }
        public Discount Discount { get; set; }
    }

    public class NewFileLedger
    {
        public FileData FileData { get; set; }
        public List<Sp_Get_FileLastOwner_Result> OwnerData { get; set; }
        public List<PlotStatment> Report { get; set; }
        public List<Discount> Discount { get; set; }
        public File_Plot_Balance Balance { get; set; }
    }
    public class FileLedger
    {
        public FileData File { get; set; }
        public List<AdjInst> Installments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> ReceAmts { get; set; }
        public decimal TotalAmt { get; set; }
        public Discount DiscountAmt { get; set; }

    }
    public class FileBalanceLedger
    {
        public FileData File { get; set; }
        public List<ReportModel> Report { get; set; }
        public decimal TotalAmt { get; set; }

    }
    public class AdjInst
    {
        public long Id { get; set; }
        public string Installment_Name { get; set; }
        public string Installment_Type { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime Due_Date { get; set; }
        public string Status { get; set; }
        public bool check { get; set; }

    }
    [XmlRoot("corpsms")]
    public class corpsms
    {
        [XmlElement("code")]
        public string code { get; set; }
        [XmlElement("type")]
        public string type { get; set; }
        [XmlElement("response")]
        public string response { get; set; }
        [XmlElement("transactionID")]
        public string transactionID { get; set; }
    }
    public class BidPlotinfo
    {
        public long PlotId { get; set; }
        public string PlotNumber { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public decimal? Calc_Size { get; set; }
        public decimal? Rate { get; set; }
        public bool Stat { get; set; }
    }
    //public class ServiceChargesBill
    //{
    //    public string Name { get; set; }
    //    public string Address { get; set; }
    //    public decimal? Arrears { get; set; }
    //    public decimal? Amount_Paid { get; set; }
    //    public List<Service_Charges_Details> BillDeta { get; set; }
    //}
    public class CommercialInstallmentsReturn
    {
        public string Module { get; set; }
        public bool Status { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public decimal Grand_Total { get; set; }

        public List<Commercial_Installments> CommercialInstallments { get; set; }

    }
    public class PremiumAllotmentLet
    {
        public string Size { get; set; }
        public long Owner_Id { get; set; }
        public string Name { get; set; }
        public string Father_Husband { get; set; }
        public string CNIC_NICOP { get; set; }
        public string Postal_Address { get; set; }
        public string City { get; set; }
        public string Owner_Img { get; set; }
        public string Owner_Img2 { get; set; }
        public string Owner_Img3 { get; set; }
        public string Owner_Img4 { get; set; }
    }
    public class DealershipDetails
    {
        public Dealership Dealership { get; set; }
        public List<Dealer> Dealer { get; set; }
    }
    public class CustomerReceipt
    {
        public long FileFormNumber { get; set; }
        public decimal Amount { get; set; }
        public string Bank { get; set; }
        public string Branch_Name { get; set; }
        public string Ch_Pay_Draft_No { get; set; }
        public DateTime? Ch_Pay_Draft_Date { get; set; }
        public string Type { get; set; }
        public string Payment_Type { get; set; }
        public string ReceiptNo { get; set; }
        public string ReqBank { get; set; }
        public string Transaction_Id { get; set; }
    }
    public class CustomerFileInfoShort
    {
        public string FileFormNumber { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string CNIC_NICOP { get; set; }
        public string Plot_Size { get; set; }
        public string Phase { get; set; }
        public string Block { get; set; }
        public decimal InstallmentAmount { get; set; }
    }
    public class PlotServiceChargesDetails
    {
        public decimal? Receiable { get; set; }
        public decimal? Received { get; set; }
        public decimal? Resi_Receiable { get; set; }
        public decimal? Resi_Received { get; set; }
        public decimal? Com_Receiable { get; set; }
        public decimal? Com_Received { get; set; }
        public List<Sp_Get_PlotsServiceChargesList_Result> PlotsServiceChargesList { get; set; }
    }
    public class ServiceChargesDetails
    {
        public decimal? Receiable { get; set; }
        public decimal? Received { get; set; }
        public decimal? Resi_Receiable { get; set; }
        public decimal? Resi_Received { get; set; }
        public decimal? Com_Receiable { get; set; }
        public decimal? Com_Received { get; set; }
        public List<Sp_Get_Report_ServiceCharges_Result> ServiceChargesList { get; set; }
        public List<Electricity_Report> totalcounter1 { get; set; }
        public List<Electricity_Report> totalcounter2 { get; set; }
    }

    public class Electricity_Report
    {
        public string name { get; set; }
        public decimal? y { get; set; }
        public decimal? z { get; set; }
    }

    public class ElectricityChargesDetails
    {

        public decimal? Receiveable { get; set; }
        public decimal? Received { get; set; }
        public decimal? Resi_Receiable { get; set; }
        public decimal? Resi_Received { get; set; }
        public decimal? Com_Receiable { get; set; }
        public decimal? Com_Received { get; set; }
        public List<Sp_Get_Report_ElectricityCharges_Result> ElectricityChargesList { get; set; }
        public List<Sp_Get_Report_ServiceCharges_Result> ServiceChargesList { get; set; }
        public List<Electricity_Report> totalcounter1 { get; set; }
        public List<Electricity_Report> totalcounter2 { get; set; }
    }
    public class PlotElectricDetails
    {
        public decimal? Receiable { get; set; }
        public decimal? Received { get; set; }
        public decimal? Resi_Receiable { get; set; }
        public decimal? Resi_Received { get; set; }
        public decimal? Com_Receiable { get; set; }
        public decimal? Com_Received { get; set; }
        public List<Sp_Get_PlotsElectricCharges_ByBlock_Result> PlotsElectricList { get; set; }
    }
    public class NewConnectionCharges
    {
        public Sp_Get_PlotDetailData_Result PlotData { get; set; }
        public List<Sp_Get_PlotLastOwner_Result> OwnerData { get; set; }
        public Connection_Charges Conch { get; set; }
    }
    public class LeadsData
    {
        public Sp_Get_Lead_Result Lead { get; set; }
        public List<SAM_Receipts> Receipts { get; set; }
        public List<SAM_Voucher> Vouchers { get; set; }
    }
    public class Orgchart
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string image { get; set; }
    }
    public class AssetListing
    {
        public Sp_Get_All_AssetList_Result ListRes { get; set; }
    }

    public class SalariesDetails
    {

        public Sp_Get_Salary_Result salary { get; set; }
        public List<Sp_Get_Salary_Details_Result> details { get; set; }

    }
    //public class AsstDetails
    //{
    //    public Sp_Get_Asset_Parameter_Result Assetinfo { get; set; }

    //}
    public class OrgColor
    {
        public string color { get; set; }
    }
    public class DailyCashierUser_Report
    {
        public Nullable<decimal> Amount { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Bank { get; set; }
        public string Ch_Pay_Draft_No { get; set; }
        public Nullable<System.DateTime> Ch_Pay_Draft_Date { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public string File_Plot_Number { get; set; }
        public string Block { get; set; }
        public string Plot_Type { get; set; }
        public string ReceiptNo { get; set; }
        public string Dealership_Name { get; set; }
        public string Module { get; set; }
        public string PaymentType { get; set; }
        public string Project { get; set; }
        public string Type { get; set; }
        public string Cashier_Name { get; set; }
        public string Size { get; set; }
        public bool? Cancel { get; set; }
        public string ReportType { get; set; }
        public string Description { get; set; }
        public string Date_Month { get; set; }
        public DateTime? ReversalDate { get; set; }
        public string VoucherType { get; set; }
        public long? TransacitonId { get; set; }
    }
    public class Assets
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public long Type { get; set; }
        public string Description { get; set; }
        public string Specs { get; set; }
        public int Company_Id { get; set; }
        public string Types { get; set; }
        public string Vehicle_Model { get; set; }
        public string Vehicle_Company { get; set; }
        public string Vehicle_Registration_Number { get; set; }
        public Nullable<decimal> Vehicle_Fuel { get; set; }


    }
    public class AssetsTypes
    {
        public long Id { get; set; }
        public string Types { get; set; }

    }
    public class Company_Assets_model
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Type { get; set; }
        public string Description { get; set; }
        public string Specs { get; set; }
        public int Company_Id { get; set; }
        public string Types { get; set; }


    }
    public class Activitiess
    {
        public long Id { get; set; }
        public string Activity_Type { get; set; }
        public string Activity { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> datesele { get; set; }
        public Nullable<System.TimeSpan> timeselec { get; set; }
        public string Name { get; set; }


    }
    public class Report_FilesStatus
    {
        public List<string> Categories { get; set; }
        public List<Report_Series> Series { get; set; }

    }

    public class Report_Series
    {
        public string name { get; set; }
        public int?[] data { get; set; }
    }
    public class Report_Series_Stack
    {
        public string name { get; set; }
        public int?[] data { get; set; }
        public string stack { get; set; }
        public string linkedTo { get; set; }
        public string color { get; set; }
    }
    public class Report_Amounts
    {
        public List<string> Categories { get; set; }
        public List<Report_Series_Decimal> Series { get; set; }
    }
    public class Report_Decimal
    {
        public List<string> Categories { get; set; }
        public List<Report_Series_D> Series { get; set; }
    }
    public class Report_Amounts_Stack
    {
        public List<string> Categories { get; set; }
        public List<Report_Series_Decimal_Stack> Series { get; set; }
    }
    public class Report_Serie_Stack
    {
        public List<string> Categories { get; set; }
        public List<Report_Series_Stack> Series { get; set; }
    }
    public class Report_Hire_Fire
    {
        public string Name { get; set; }
        public long count { get; set; }

    }
    public class Employee_HR_Report
    {
        public List<Report_Hire_Fire> Graphical { get; set; }
        public List<Employee> Tabular { get; set; }
        public List<Sp_Get_Employee_Result> HiredEmployees { get; set; }
        public List<Sp_Get_Employee_Result> SuspendedEmployees { get; set; }
        public List<Sp_Get_Employee_Result> TerminatedEmployees { get; set; }
        public List<Sp_Get_Employee_Result> ResignedEmployees { get; set; }
    }
    public partial class Sp_Get_Employee_Result
    {
        public EmployeeOtherDetails ParsedOtherDetails { get; set; }
        public string ExpectedProbationEnd { get; set; }
        public bool PastProbation { get; set; }
        public DateTime? StatusChangeDate { get; set; }
    }
    public class Report_Series_D
    {
        public string name { get; set; }
        public decimal?[] data { get; set; }
    }
    public class Report_Series_Decimal
    {
        public string name { get; set; }
        public decimal?[] data { get; set; }
        public double? pointPadding { get; set; }
    }
    public class Report_Series_Decimal_Stack
    {
        public string name { get; set; }
        public decimal?[] data { get; set; }
        public string stack { get; set; }
    }
    public class PurchaseCombinedReport
    {
        public Report_Decimal BarChart { get; set; }
        public List<Electricity_Report> PieChart { get; set; }
    }

    public class Booking_Report
    {
        public string name { get; set; }
        public int? y { get; set; }
    }
    public class Transfer_Report
    {
        public string name { get; set; }
        public int? y { get; set; }
    }


    public class PaymentTypes_Report
    {
        public decimal? y { get; set; }
        public string color { get; set; }
        public DrillDown drilldown { get; set; }
    }
    public class DrillDown
    {
        public string name { get; set; }
        public List<string> categories { get; set; }
        public List<decimal?> data { get; set; }
    }

    public class Salaries_Allownce_model
    {
        public long Id { get; set; }
        public string AllownceName { get; set; }

        public decimal allownce_amout { get; set; }
    }

    public class LeaveRequisitionModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<System.DateTimeOffset> CreatedAt { get; set; }
        public string Reason { get; set; }
        public long UserID { get; set; }
        public Nullable<long> NoOfDays { get; set; }
        public Nullable<int> OfficalLeave { get; set; }
        public string HrApproval { get; set; }
        public string ManagerApproval { get; set; }
        public string HrRemarks { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string AppliedbyUserName { get; set; }
        public string ManagerRemarks { get; set; }
        public bool Cancel { get; set; }
        public Nullable<int> LeaveType { get; set; }


    }
    public class PurchaseOrder_Model
    {
        public long Id { get; set; }
        public string Requisition_Product_Name { get; set; }
        public Nullable<long> Requisition_Product_Id { get; set; }
        public string Vendor_Name { get; set; }
        public Nullable<long> Vendor_Id { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<System.DateTime> Date { get; set; }

        public long ProId { get; set; }
        public long BidId { get; set; }
        public long VenId { get; set; }
        public long GroupId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Job_Title { get; set; }
        public string Office_Mobile { get; set; }
        public string Mobile_Number { get; set; }
        public string Fax { get; set; }
        public string City { get; set; }
        public string State { get; set; }

    }

    public class cddc
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> Parent_Id { get; set; }
        public string Type { get; set; }
    }
    public class ChartParentChildModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string parent { get; set; }
        public decimal? value { get; set; }
    }
    public class LeavesTypes
    {
        public int? Value { get; set; }
        public string Name { get; set; }
    }
    public class LeaveStatusModal
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }
    public class UserLeaveSub
    {
        public int? Granted { get; set; }
        public int? Spent { get; set; }
        public string LeaveType { get; set; }
    }
    public class AssetModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string speci { get; set; }
        public long assetid { get; set; }



    }
    public class LeavesModel
    {
        public int Causual { get; set; }
        public int Sick { get; set; }
        public int Annual { get; set; }
        public int Ctype { get; set; }
        public int Stype { get; set; }
        public int Atype { get; set; }

    }
    public class LoanRequisitionModel
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string Reason { get; set; }
        public string EmpName { get; set; }
        public Nullable<long> UserID { get; set; }
        public string HrApproval { get; set; }
        public string ManagerApproval { get; set; }
        public string HrRemarks { get; set; }
        public string ManagerRemarks { get; set; }
        public Nullable<decimal> LoanAmtPerMonth { get; set; }
        public Nullable<int> Installments { get; set; }
        public long AppId { get; set; }
        public Nullable<decimal> Loan_Amt { get; set; }
        public Nullable<long> Emp_id { get; set; }
        public string Hr_Status { get; set; }
        public string Finance_Receiving { get; set; }
    }
    public class TextAmt
    {
        public string Text { get; set; }
        public decimal Amt { get; set; }


    }
    public class AdvanceOrLoanVoucher
    {
        public long id { get; set; }
        public string name { get; set; }
        public decimal ln_amt { get; set; }
        public int insta { get; set; }
        public string remarks { get; set; }
    }

    public class PostModel
    {
        public long Prj_Id { get; set; }
        public long?[] Id { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string[] URLs { get; set; }
    }
    public class Search_OverDue
    {
        public int? Installments { get; set; }
        public int? S_Inst_Range { get; set; }
        public int? E_Inst_Range { get; set; }
        public string Plot_Size { get; set; }
        public long? Dealer_Id { get; set; }
        public decimal? Range { get; set; }
        public decimal? S_Range { get; set; }
        public decimal? E_Range { get; set; }
        public decimal? G_Amt { get; set; }
        public decimal? L_Amt { get; set; }
    }

    public class Project_details_inventory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public string Nameofgood { get; set; }
        public string MeasurementUnit { get; set; }
        public Nullable<long> ProjectId { get; set; }
    }
    //public class InvenDetails
    //{
    //    public List<Sp_Get_Project_Details_Result> ProInfo { get; set; }
    //    public List<Inventory_Details> Inven { get; set; }

    //}
    public class DemandOrderModel
    {
        public long Id { get; set; }
        public string Product { get; set; }
        public string Measurement { get; set; }
        public Nullable<System.DateTime> Required_Date { get; set; }
        public Nullable<System.DateTime> Apply_Date { get; set; }
        public string Status { get; set; }
        public Nullable<long> Quantity { get; set; }
        public string Project_Name { get; set; }
        public Nullable<long> Project_Id { get; set; }
        public long Quantity_Assigning { get; set; }
        public long Inventory_id { get; set; }
    }
    public class InventoryBiddingModel
    {
        public long Id { get; set; }
        public long Demad_Order_id { get; set; }
        public long Quantity { get; set; }

        public string Requisition_Product_Name { get; set; }
        public Nullable<long> Requisition_Product_Id { get; set; }
        public string Vendor_Name { get; set; }
        public Nullable<long> Vendor_Id { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Image { get; set; }
    }
    public class InventoryModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Nullable<long> Quantity { get; set; }
        public string Measurement { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Purchase_Rate { get; set; }
        public Nullable<decimal> Minimum_Quantity { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Supplier_Name { get; set; }
        public string Image { get; set; }
        public Nullable<long> Supplier_Id { get; set; }
        public Nullable<long> GroupId { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> Invoice_Date { get; set; }
        public Nullable<long> Invoice_Number { get; set; }
        public Nullable<System.DateTime> PO_Date { get; set; }
        public Nullable<long> PO_Number { get; set; }
        public class IssueRequestData
        {
            public long Id { get; set; }
            public long Item_Id { get; set; }
            public string Item_Name { get; set; }
            public float Item_Qty { get; set; }
            public string Item_UOM { get; set; }
            public string RequestedBy { get; set; }
            public string IssueTo { get; set; }
            public string ForProject { get; set; }
            public int? Status { get; set; }
            public long TotalAvailQty { get; set; }
            public string MgrRemks { get; set; }
            public string IssuerRemks { get; set; }
        }

        public class ViewIssueRequestData
        {
            public long? Id { get; set; }
            public long? Item_Id { get; set; }
            public string Item_Name { get; set; }
            public float? Item_Qty { get; set; }
            public string Item_UOM { get; set; }
            public string RequestedBy { get; set; }
            public string IssueTo { get; set; }
            public string ForProject { get; set; }
            public int? Status { get; set; }
            public long? TotalAvailQty { get; set; }
            public string MgrRemks { get; set; }
            public string IssuerRemks { get; set; }
            public float? len { get; set; }
            public float? wid { get; set; }
            public float? hei { get; set; }
            public float? dia { get; set; }
        }
    }

    public class LoansDetails
    {
        public Sp_Get_LoanDetails_Result Loan { get; set; }
        public List<Sp_Get_LoanInstallments_Result> Installments { get; set; }
    }
    public class UserNotifierData
    {
        public long notID { get; set; }
        public DateTime createdAt { get; set; }
        public long fromUserId { get; set; }
        public string hitUrl { get; set; }
        public string notificationMsg { get; set; }
        public string refreshUrl { get; set; }
        public string refreshWidget { get; set; }
        public long toUserId { get; set; }
    }
    public class Header
    {
        public Sp_Get_UserDetails_Short_Result Details { get; set; }
        public List<Sp_Get_Notifications_User_Short_Result> Noties { get; set; }
        public List<Sp_Get_UserCompany_Result> Companylist { get; set; }
    }
    public class NotifyPerson
    {
        public long UserID { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserProfileImage { get; set; }
    }
    public class MyTickets
    {
        public List<Sp_Get_Tickets_Assigned_Result> AssignedTickets { get; set; }
        public List<Sp_Get_Tickets_CreatedMe_Result> CreateTickets { get; set; }

    }
    public class EmpDetails
    {
        public Sp_Get_Employee_Parameter_Result PersonalInfo { get; set; }
        public List<Sp_Get_Employee_Designation_Result> EmpDesigation { get; set; }


    }
    public class EmpWorkExperience
    {
        public List<Education> Edu { get; set; }
        public List<WorkExperience> WorkExp { get; set; }
    }
    public class EmploymentDetails
    {
        public Sp_Get_Employee_Parameter_Result PersonalInfo { get; set; }
        public List<Sp_Get_Employee_Designation_Result> EmpDesigation { get; set; }
        public List<Sp_Get_UserAllRoles_Result> AllRoles { get; set; }
    }
    public class RebateModel
    {
        public long Deal_Id { get; set; }
        public decimal Amount { get; set; }
        public string paymentType { get; set; }
        public string Method { get; set; }


    }
    public class AdminFacilitiesMembership
    {

        public RegisterMember MemberDetails { get; set; }
        public List<Members_Subscription> Subscription { get; set; }
        public List<MembersFee> FeeStructure { get; set; }


    }
    public class AdminProjects
    {
        public long Id { get; set; }
        public Nullable<long> Project_Id { get; set; }
        public string project_Name { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Emp_Fee { get; set; }
        public Nullable<decimal> Emp_Mem_Fee { get; set; }
        public Nullable<decimal> Res_Fee { get; set; }
        public Nullable<decimal> Res_Mem_Fee { get; set; }
        public Nullable<decimal> Out_Fee { get; set; }
        public Nullable<decimal> Out_Mem_Fee { get; set; }
        public Nullable<System.DateTime> Entry_Date { get; set; }
    }
    public class MeasurementType
    {
        public int ID { get; set; }
        public string Unit { get; set; }
    }

    public class MaterialStatementData
    {
        public string item { get; set; }
        public float qty { get; set; }
        public string qtytp { get; set; }
        public float? len { get; set; }
        public float? wid { get; set; }
        public float? hei { get; set; }
        public float? dia { get; set; }
        public string remarks { get; set; }
        public string description { get; set; }
        public decimal? Rate { get; set; }
    }

    public class MaterialStatementViewData
    {
        public long Id { get; set; }
        public string item { get; set; }
        public float? qty { get; set; }
        public string qtytp { get; set; }
        public float? len { get; set; }
        public float? wid { get; set; }
        public float? hei { get; set; }
        public float? dia { get; set; }
        public string remarks { get; set; }
        public string description { get; set; }
        public string MilestoneName { get; set; }
    }

    public class NewReportModel
    {
        public int RegisteredFiles { get; set; }
        public int CancelledFiles { get; set; }
        public int RefundedFiles { get; set; }
        public int TempCancelledFiles { get; set; }
        public List<DealersFileReport> DealershipPerformanceInfo { get; set; }
    }

    public class DealersFileReport
    {
        public long DealerId { get; set; }
        public string DealerName { get; set; }
        public int Marla3FilesReg { get; set; }
        public int Marla5FilesReg { get; set; }
        public int Marla10FilesReg { get; set; }
        public int Marla8FilesReg { get; set; }
        public int Marla3FilesPen { get; set; }
        public int Marla5FilesPen { get; set; }
        public int Marla10FilesPen { get; set; }
        public int Marla8FilesPen { get; set; }
        public int Marla3FilesCan { get; set; }
        public int Marla5FilesCan { get; set; }
        public int Marla10FilesCan { get; set; }
        public int Marla8FilesCan { get; set; }
    }

    public class InventoryItem
    {
        public string name { get; set; }
        public float qty { get; set; }
        public string uom { get; set; }
        public long? vendor { get; set; }
        public float? rate { get; set; }
        public DateTime expiry { get; set; }
        public float? minqty { get; set; }
        public long brand { get; set; }
        public string sku { get; set; }
        public float? len { get; set; }
        public float? wid { get; set; }
        public float? hei { get; set; }
        public float? dia { get; set; }
        public string grn { get; set; }
        public string bid { get; set; }
        public long location { get; set; }
        public string holdpurpose { get; set; }
        public long? holdfor { get; set; }
        public float? holdqty { get; set; }
    }

    public class PaymentScheduleData
    {
        public string desc { get; set; }
        public long descid { get; set; }
        public float c_per { get; set; }
        public float c_amt { get; set; }
        public float w_per { get; set; }
        public float w_amt { get; set; }
        public string rem { get; set; }
    }

    public class MultiDemandOrderViewData
    {
        public long? item { get; set; }
        public long? emp { get; set; }
        public string emp_Name { get; set; }
        public decimal qty { get; set; }
        public string rem { get; set; }
    }

    public partial class Sp_Get_Inventory_Result
    {
        public string DepartmentName { get; set; }
    }

    public partial class Inventory_Bidding
    {
        public int itemCustomId { get; set; }
    }

    //public class PurchaseOrderPrintView
    //{
    //    public List<Sp_Get_PO_Full_Detail_Result> PO_Data { get; set; }
    //    public Vendor Vendor_Data { get; set; }
    //    public List<Inventory_PO_Terms> PO_Terms { get; set; }
    //}
    public class PurchaseOrderPrintViewSer
    {
        public List<Service_PurchaseOrder> PO_Data { get; set; }
        public Vendor Vendor_Data { get; set; }
        public List<Inventory_PO_Terms> PO_Terms { get; set; }
    }

    public class PaymentVoucherReqModel
    {
        public List<Prj_Voucher_Req> VoucherReqs { get; set; }
        public List<SP_Get_PaymentSchedule_ByMile_Result> PaymentScheduleDetailsData { get; set; }
    }

    public class QuotationsViewModel
    {
        public List<SP_GET_Applied_Quotes_For_PR_Group_Result> AppliedBids { get; set; }
        public List<Inventory_Purchase_Req> PurchaseRequistionDetails { get; set; }
    }
    public class QuotationServicesViewModel
    {
        public List<Sp_GET_Applied_Quotes_For_SR_Group_Result> AppliedBids { get; set; }
        public List<Services_Purchase_Req> PurchaseRequistionDetails { get; set; }
    }
    public class QuotationsListViewModel
    {
        public List<SP_GET_Applied_Quotes_For_PR_List_Result> AppliedBids { get; set; }
        public List<Inventory_Purchase_Req> PurchaseRequistionDetails { get; set; }
    }

    public class CanceledFileReport
    {
        public long FileId { get; set; }
        public long Status { get; set; }
        public string FileNumber { get; set; }
        public string PlotSize { get; set; }
        public DateTime CancelledOn { get; set; }
        public decimal? TotalInstallmentsPaidAmount { get; set; }
        public int? TotalInstallmentsPaidCount { get; set; }
        public decimal? TotalInstallmentsRemainAmount { get; set; }
        public int? TotalInstallmentsRemainCount { get; set; }
        public decimal? TotalInstallmentsNotPaidAmount { get; set; }
        public int? TotalInstallmentsNotPaidCount { get; set; }
    }
    public class AuditorHome_Report
    {
        public Sp_Get_PlotsShortSummary_Result Summary { get; set; }
        public Sp_Get_FilesShortSummary_Result FilesSummary { get; set; }
        public Sp_Get_OverDueFilesReport_Result OverDueFiles { get; set; }
        public SherAlamStats SherAlam { get; set; }
    }
    public class FileActions
    {
        public string deleteType { get; set; }
        public string name { get; set; }
        public long size { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteUrl { get; set; }
        public string downUrl { get; set; }
        public string path { get; set; }
    }

    public partial class Inventory_Stock_Out
    {
        public string ItemName { get; set; }
        public string Uom { get; set; }
        public decimal? Len { get; set; }
        public decimal? Wid { get; set; }
        public decimal? Hei { get; set; }
        public decimal? Dia { get; set; }
        public string Size { get; set; }
        public string L_uom { get; set; }
        public string W_uom { get; set; }
        public string H_uom { get; set; }
        public string D_uom { get; set; }
        public string Size_uom { get; set; }
    }

    public class ReturnItemsModel
    {
        public long item { get; set; }
        public string itemname { get; set; }
        public long issueId { get; set; }
        public long shelfId { get; set; }
        public long Warehouse_Id { get; set; }
        public long Prj_Id { get; set; }
        public string Prj_Name { get; set; }
        public long Dep_Id { get; set; }
        public string Dep_Name { get; set; }
        public string Warehouse_Name { get; set; }
        public string shelfName { get; set; }
        public string issueNum { get; set; }
        public decimal qtyRet { get; set; }
        public decimal rqstdQty { get; set; }
    }
    public class MeterReadingViewDetails
    {
        public List<Sp_Get_PlotsMeterDetails_NewMeterReadings_Result> MeterReadings { get; set; }
        public bool? ManualPermissionGranted { get; set; }
    }

    public class ManualMeterReadingPermParams
    {
        public long Block { get; set; }
        public string Reason { get; set; }
    }

    public partial class Sp_Get_ElecBill_ById_Result
    {
        public Sp_Get_ElecBill_ById_Result CurrMonthBillInfo { get; set; }
        public List<Sp_Get_PlotLastElectricityBills_Result> PastMonthBills { get; set; }
    }

    public partial class Sp_Get_ServiceCharge_Bill_ById_Result
    {
        public Sp_Get_ServiceCharge_Bill_ById_Result CurrMonthBillInfo { get; set; }
        public List<Sp_Get_PlotLastServiceCharges_Result> PastMonthBills { get; set; }
    }

    public partial class ServiceChargesInstallment
    {
        public Sp_Get_ElecBill_ById_Result BillDetails { get; set; }
    }

    public class SC_Installments_Detailed
    {
        public ServiceChargesInstallment InstallmentOverview { get; set; }
        public List<SvcCharges_Installments_Structure> InstallmentStructure { get; set; }
    }

    public class BlockFineConfig
    {
        public long Id { get; set; }
        public int FinePercentage { get; set; }
        public string BlockName { get; set; }
    }

    public class ServiceChargesConfig
    {
        public List<BlockFineConfig> BlockFines { get; set; }
        public List<BlockFineConfig> ServiceChargesBreakUp { get; set; }
        public int DueDay_ForBill { get; set; }
    }

    public class BillHistoryItem
    {
        public DateTime BillMonth { get; set; }
        public string Amount { get; set; }
        public string AmountPaid { get; set; }
        public int UnitsConsumed { get; set; }
        public decimal AmountMade { get; set; }
        public decimal AmountPaid_Decimal { get; set; }
    }
    public class InvoiceDetails
    {
        public string r { get; set; }
        public decimal q { get; set; }
        public decimal t { get; set; }
        public long v { get; set; }
    }
    public class VendorFullDetail
    {
        public Vendor Vendor { get; set; }
        public List<Vendor_Representative> Representative { get; set; }
        public List<Vendor_Credit_Terms> Terms { get; set; }
    }

    public class ElectricityBillView
    {
        public List<Sp_Get_ElecBill_ById_Result> CurrBill { get; set; }
        public List<Electricity_Bill_Details> CurrBillDetails { get; set; }
        public List<BillHistoryItem> PastBills { get; set; }
    }

    public class ElectricityBillViewTypeWise
    {
        public Sp_Get_PlotsElectricCharges_NewMeterReadinds_Type_Parameter_Result CurrBill { get; set; }
        public List<Electricity_Bill_Details> CurrBillDetails { get; set; }
        public List<BillHistoryItem> PastBills { get; set; }
    }

    public class AttendanceView
    {
        public List<AttendanceData> CurrAttendance { get; set; }
        public Sp_Get_Employee_Parameter_Result EmployeeDetails { get; set; }
        public List<Sp_GetEmpCompleteDesigs_Result> EmployeeDepsInfo { get; set; }
        public List<Attendance_Holidays> Holidays { get; set; }
        public List<Roster_Templates> Roster_Details { get; set; }
        public List<Attendance_TimeAdjustments> AttTimeAdjustments { get; set; }
        public DateTime FiscMonthStart { get; set; }
        public DateTime FiscMonthEnd { get; set; }
    }

    public partial class AttendanceData
    {
        //public DateTime AttendanceDate { get; set; }
        public long? DepId { get; set; }
        public string DepName { get; set; }
        public long? CompId { get; set; }
        public string CompName { get; set; }
        public long? DesigId { get; set; }
        public string DesigName { get; set; }
        public bool IsInShift { get; set; }
        public bool IsWeeklyOff { get; set; }
        public bool IsLeave { get; set; }
    }

    public class TimeAdjustmentView
    {
        public AttendanceData CheckIn_Info { get; set; }
        public Employee EmployeeInfo { get; set; }
    }

    public class AttendanceHomeWidget
    {
        public Employee EmpData { get; set; }
        public List<AttendanceData> AttendanceRecord { get; set; }
        public List<Roster_Templates> RosterInfo { get; set; }
    }
    public class MultiRosterAssignmentModel
    {
        public List<Sp_Get_DepartmentEmployees_Result> EmpsData { get; set; }
        public Dictionary<long, string> RostersData { get; set; }
    }
    public partial class Attendance_Holidays
    {
        public bool IsLeave { get; set; }
        public long? ForEmployee { get; set; }
        public long Id { get; set; }
        public string HolidayTitle { get; set; }
        public Nullable<System.DateTime> HolidayDate { get; set; }
        public Nullable<long> CreatedBy_Id { get; set; }
        public string CreatedBy_Name { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }

    }

    public class AttendanceReportView
    {
        public List<AttendanceData> Attendance { get; set; }
        public List<Attendance_Holidays> Holidays { get; set; }


    }

    public class ManualAttendanceForm
    {
        public string EmployeeName { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime ManualAttendanceDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public long? AttId { get; set; }
    }

    public class RosterDetails
    {
        public int StartHour { get; set; }
        public int StartMin { get; set; }
        public int EndHour { get; set; }
        public int EndMin { get; set; }
        public bool IsWeekend { get; set; }
        public string WeekDay { get; set; }
        public int WeekDayCode { get; set; }
    }

    public partial class Roster_Templates
    {
        public List<RosterDetails> ShiftMap { get; set; }
    }

    public class AttendanceExemption
    {
        public AttendanceExemptionStatus Status { get; set; }
        public DateTime? StatusChangeDate { get; set; }
        public DateTime StartingDate { get; set; }
        public long AddedBy_Id { get; set; }
        public string AddedBy_Name { get; set; }
    }

    public class EmployeeOtherDetails
    {
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? Resig_Termin_Date { get; set; }
        public DateTime? Contract_Validity { get; set; }
        public AttendanceExemption Atten_Exempt { get; set; }
    }

    public partial class Employee
    {
        public EmployeeOtherDetails ParsedOtherDetails { get; set; }
        public string ReportingToName { get; set; }
        public string ExpectedProbationEnd { get; set; }
        public bool PastProbation { get; set; }
        public DateTime? StatusChangeDate { get; set; }
        public DateTime? Contract_Validity { get; set; }

    }

    public partial class Sp_Get_Employee_Parameter_Result
    {
        public EmployeeOtherDetails ParsedOtherDetails { get; set; }
    }

    public class HRConfiguration
    {
        public int Gratuity_Starts_After_Days { get; set; }
        public int GratuityInterval_InMonths { get; set; }
        public int Anual_Leaves_Quota { get; set; }
        public int Sick_Leaves_Quota { get; set; }
        public int Casual_Leaves_Quota { get; set; }
        public int Late_Coming_Deduction_Perc { get; set; }
        public int Late_Coming_Grace_Mins { get; set; }
        public int Probation_Days { get; set; }
        public int Absent_Fine_Percentage { get; set; }
        public int Max_Installments { get; set; }
        public int Max_Loan_Allowed { get; set; }
        public int Max_Annual_Leaves_Accumulation { get; set; }
        public DateTime FiscalYearStart { get; set; }
        public DateTime FiscalYearEnd { get; set; }
        public DateTime PayCycleStart { get; set; }
        public DateTime PayCycleEnd { get; set; }
        public List<FixedAllowanceConfigModel> FixedAllowances { get; set; }
        public decimal OverTimeRate { get; set; }
    }

    public class FixedAllowanceConfigModel
    {
        public long DepId { get; set; }
        public string DepName { get; set; }
        public decimal MinSalaryAmt { get; set; }
        public decimal PerMonthAmount { get; set; }
    }

    public class LoanApprovalView
    {
        public Sp_Get_LoanDetails_Result LoanoverView { get; set; }
        public List<Loan_Installments> InstallmentsStructure { get; set; }
    }

    public class EmployeeProfileLoanView
    {
        public List<LoanRequisition> Requisitions { get; set; }
        public List<Loan_Installments> Installments { get; set; }
    }

    public class OrganoNode
    {
        public long id { get; set; }
        public string collapsed { get; set; }
        public string className { get; set; }
        public string nodeTitlePro { get; set; }
        public string nodeContentPro { get; set; }
        public string relationship { get; set; }
        public List<OrganoNode> children { get; set; }
    }

    public class MultipleChoiceQuestion
    {
        public string Question { get; set; }
        public int QNo { get; set; }
        public List<string> Choices { get; set; }
        public List<int> SelectedOptions { get; set; }
        public int MinSelectionCount { get; set; }
        public bool Required { get; set; }
    }

    public class ShortAnswerQuestion
    {
        public string Question { get; set; }
        public int QNo { get; set; }
        public string Answer { get; set; }
        public bool Required { get; set; }
    }

    public class QuestionInfo
    {
        public int Qno { get; set; }
        public string QData { get; set; }
        public QuestionType Type { get; set; }
    }

    public class RoadMapModel
    {
        public DateTime AssignedDate { get; set; }
        public string date { get; set; }
        public string content { get; set; }
        public long Id { get; set; }
    }

    public class SMARTGoalDeepDetail
    {
        public SmartGoal Goal { get; set; }
        public List<QuestionnaireFeedback> SubmittedReport { get; set; }
    }

    public class QuestionnaireView
    {
        public QuestionnaireForm QuestDetails { get; set; }
        public List<MultipleChoiceQuestion> Mcqs { get; set; }
        public List<ShortAnswerQuestion> Sqs { get; set; }
    }

    public class QuestionnaireFillView
    {
        public QuestionnaireFeedback QuestDetails { get; set; }
        public List<MultipleChoiceQuestion> Mcqs { get; set; }
        public List<ShortAnswerQuestion> Sqs { get; set; }
    }

    public class McqAns
    {
        public int QuestNo { get; set; }
        public List<int> SlctdAns { get; set; }
    }

    public class ParaAns
    {
        public int QuestNo { get; set; }
        public string Ans { get; set; }
    }
    public class Commercial_Roomss
    {
        public long Id { get; set; }
        public string Com_App_Shop_Number { get; set; }
        public string Type { get; set; }
        public decimal Area { get; set; }
        public decimal rate_sq_ft { get; set; }
        public decimal ExtraAmount { get; set; }
        public string Location { get; set; }
        public long Floor_Id { get; set; }
        public Nullable<long> Plan_Id { get; set; }
        public string Status { get; set; }
        public Nullable<long> Installment_Plan_Id { get; set; }
        public string Project_Name { get; set; }
    }

    public class PurchaseRequisitons
    {
        public List<Inventory_Purchase_Req> Pending { get; set; }
        public List<Inventory_Purchase_Req> In_Quotation_Process { get; set; }
        public List<Inventory_Purchase_Req> Quotation_Approval { get; set; }
        public List<Inventory_Purchase_Req> Approved { get; set; }
        public List<Inventory_PurchaseOrder> Purchase_Order_Generated { get; set; }
        public List<Inventory_Purchase_Req> Good_Received { get; set; }
    }

    public class LoanView
    {
        public LoanRequisition LoanInfo { get; set; }
        public List<Loan_Installments> Installments { get; set; }
    }

    public class LoanInstallmentModel
    {
        public long Id { get; set; }
        public int Amount { get; set; }
        public string InstallmentMonth { get; set; }
        public DateTime? InstMonth { get; set; }
        public bool IsPaid { get; set; }
    }

    public class SMARTDescription
    {
        public bool S_Part { get; set; }
        public bool M_Part { get; set; }
        public bool A_Part { get; set; }
        public bool R_Part { get; set; }
        public bool T_Part { get; set; }
    }

    public partial class Sp_Get_Attendance_Report_Graphical_Result
    {
        public string AttendanceDayString { get; set; }
        public Nullable<int> Shiftwaiting { get; set; }
        public Nullable<int> Weeklyoff { get; set; }
        public Nullable<int> Leave { get; set; }
    }
    public class PlotDiscountView
    {
        public List<Sp_Get_PlotInstallments_Result> InstallmentsInfo { get; set; }
        public Sp_Get_PlotData_Result PlotDetails { get; set; }
        public Sp_Get_PlotOwnerList_Result OwnerInfo { get; set; }
        public long PlotId { get; set; }
    }
    public class Attendance_Exemption_List : AttendanceExemption
    {
        public long empId { get; set; }
        public string empName { get; set; }
        public string empCode { get; set; }
    }
    //public class BallotReqtModel
    //{
    //    public string block { get; set; }
    //    public string sector { get; set; }
    //    public long start { get; set; }
    //    public long end { get; set; }
    //}
    public class LoanReportView
    {
        public List<LoanRequisition> LoansData { get; set; }
        public List<Loan_Installments> InstallmentsData { get; set; }
    }
    //public class BallotResultModel
    //{
    //    public string Preference { get; set; }
    //    public string Name { get; set; }
    //    public string CNIC_NICOP { get; set; }
    //    public string PlotNo { get; set; }
    //    public long File_Id { get; set; }
    //    public DateTime Due_Date { get; set; }
    //    public string FileNo { get; set; }
    //    public string BookingDate { get; set; }
    //}
    //public class PreferenceFilteredPlots
    //{
    //    public string Prefrence { get; set; }
    //    public List<BallotPlot> Plots { get; set; }
    //    public int Priority { get; set; }
    //}

    //public class PreferenceFilteredFiles
    //{
    //    public string Prefrence { get; set; }
    //    public List<Sp_Get_Ballot_Qualifying_File_Result> Files { get; set; }
    //    public int Priority { get; set; }
    //}

    public class BlocksTransferFee
    {
        public long BlockId { get; set; }
        public string BlockName { get; set; }
        public decimal DC_Rate_Per_Marla_Residential { get; set; }
        public decimal DC_Rate_Per_Marla_Commercial { get; set; }
        public bool IsApplicable { get; set; }
    }

    public class Plot_Transfer_Configuration
    {
        public List<BlocksTransferFee> FeeInfo { get; set; }
        public float Buyer_FilerPercent { get; set; }
        public float Buyer_NonFilerPercent { get; set; }
        public float Seller_FilerPercent { get; set; }
        public float Seller_NonFilerPercent { get; set; }
        public int Tax_Exemption_Days { get; set; }
        public bool Tax_Exemption_Applicable { get; set; }
    }

    public class PlotTransfer_ConfigView
    {
        public Plot_Transfer_Configuration ConfigInfo { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public partial class Sp_Add_Receipt_Result
    {
        public bool IsTax { get; set; }
    }

    public class NDC_View
    {
        public List<Sp_Get_NDCFormDetails_Plot_Result> PreviousOwners { get; set; }
        public List<Plots_Transfer_Request> CurrentTransferRequest { get; set; }
    }
    public class NDC_View_Commercial
    {
        public List<Sp_Get_NDCFormDetails_Commercial_Result> PreviousOwners { get; set; }
        public List<Commercial_Transfer_Request> CurrentTransferRequest { get; set; }
    }

    public partial class Sp_Get_NDCFormDetails_Plot_Result
    {
        public string Postal_Address { get; set; }
        public string To_CNIC_NICOP { get; set; }
        public string To_FatherName { get; set; }
        public string To_Mobile1 { get; set; }
        public string To_Name { get; set; }
    }
    public partial class Sp_Get_NDCFormDetails_Commercial_Result
    {
        public string Postal_Address { get; set; }
        public string To_CNIC_NICOP { get; set; }
        public string To_FatherName { get; set; }
        public string To_Mobile1 { get; set; }
        public string To_Name { get; set; }
    }
    public class ReceiptMessageTemplate
    {
        public string MobileNumber { get; set; }
        public string Message { get; set; }
    }

    public partial class SmartGoal
    {
        public SMARTDescription ParsedDesc { get; set; }
    }

    public class DetailedAttendanceReportView
    {
        public List<Sp_Get_Attendance_Period_Result> Attendance { get; set; }
        public List<Sp_Get_AcceptedLeaves_Attendance_Result> AppliedLeaves { get; set; }
        public List<Roster_Templates> RostersTemplatesInfo { get; set; }


        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class PO_Req
    {
        public List<Sp_Get_Inventory_PurchaseOrder_list_Result> PO { get; set; }
        public List<Inventory_Purchase_Req> Req { get; set; }
    }
    public class PlotOwnInfoTax
    {
        public string Name { get; set; }
        public bool IsFiler { get; set; }
    }

    public class PlotTaxDetails
    {
        public List<PlotOwnInfoTax> CurrentOwners { get; set; }
        public decimal DC_Rate { get; set; }
        public decimal Min_Sell_Price { get; set; }
        public string PlotSize { get; set; }
        public bool TaxApplicable { get; set; }
        public double Buyer_FilerPerc { get; set; }
        public double Buyer_NonFilerPerc { get; set; }
        public double Seller_FilerPerc { get; set; }
        public double Seller_NonFilerPerc { get; set; }
        public decimal TransferFee { get; set; }
    }

    public partial class Sp_Get_SalaryComparativeReport_Result
    {
        public DateTime? ParsedDt { get; set; }
    }

    public class RentalInfoView
    {
        public Plot_Rental RentalDetails { get; set; }
        public List<Sp_Get_PlotLastOwner_Result> PlotOwners { get; set; }
    }
    public class RentalInfoViewEdit
    {
        public Plot_Rental RentalDetails { get; set; }
        public List<Sp_Get_PlotLastOwner_Result> PlotOwners { get; set; }
        public Sp_Get_PlotData_Result PlotData { get; set; }
    }

    public class SalaryTaxModel
    {
        public decimal StartRange { get; set; }
        public decimal EndRange { get; set; }
        public float Percent { get; set; }
        public decimal StaticTaxAmt { get; set; }
        public decimal ExceedAmount { get; set; }
    }
    public class Employee_serch
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Basic_Salary { get; set; }
        public string EmpCode { get; set; }

    }
    public class SalaryTaxEmployee
    {
        public string EmployeeName { get; set; }
        public long EmployeeId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal TaxAmount { get; set; }
        public string EmpCode { get; set; }

    }
    public class CalculatedSalaryTaxModel
    {
        public decimal TaxAmount { get; set; }
        public long EmpId { get; set; }
        public long SalaryId { get; set; }
    }
    public class FuelSheetView
    {
        public List<Sp_Get_FuelSheet_Result> FuelReceipts { get; set; }
        public List<EmployeeFuel> Vehicles { get; set; }
    }

    public class GeneralType1
    {
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
        public decimal? Prop3 { get; set; }
    }

    public class GratuityView
    {
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public Employee EmployeeData { get; set; }
        public bool isProcessed { get; set; }
        public DateTime RequestedOn { get; set; }
        public string Status { get; set; }
    }
    public class Project_Services
    {
        public List<Services_Purchase_Req> Req { get; set; }
        public List<Service_PurchaseOrder> PO { get; set; }
    }
    public class Serv_Comp
    {
        public List<Service_PurchaseOrder> PO { get; set; }
        public List<Service_Completion> SC { get; set; }
    }
    public partial class Sp_Get_EmployeesAll_WithCDD_Result
    {
        public string ParsedDOB { get; set; }
        public string ParsedDOJ { get; set; }
        public int Age { get; set; }
        public int TotalPeriod { get; set; }
    }
    public class ExcelMilestoneData
    {
        public string Sr { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal? No { get; set; }
        public decimal? L { get; set; }
        public decimal? H { get; set; }
        public decimal? B { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public decimal Amount { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Deadline { get; set; }
    }
    public class Return
    {
        public long? Id { get; set; }
        public bool Status { get; set; }
        public string Msg { get; set; }
    }
    public partial class Dealer_Commession
    {
        public decimal? Percentage { get; set; }
        public decimal? Total_Price { get; set; }
        public string Plot_No { get; set; }
        public string Plot_Type { get; set; }
        public string Block { get; set; }
    }
    public class BallotReqtModel
    {
        public string block { get; set; }
        public string sector { get; set; }
        public long start { get; set; }
        public long end { get; set; }
    }
    public class BallotResultModel
    {
        public string Preference { get; set; }
        public string Name { get; set; }
        public string CNIC_NICOP { get; set; }
        public string PlotNo { get; set; }
        public long File_Id { get; set; }
        public DateTime Due_Date { get; set; }
        //public BallotPlot PlotInfo { get; set; }
        //public Sp_Get_Ballot_Qualifying_File_Result FileInfo { get; set; }
        public string FileNo { get; set; }
        public string BookingDate { get; set; }
        public string FirstInstallment { get; set; }
        public string PlotArea { get; set; }
        public string PlotDimensions { get; set; }
        public string FileSize { get; set; }
        public bool? DevelopmentCharges { get; set; }
        public decimal? BalanceAmount { get; set; }
    }
    public class PreferenceFilteredPlots
    {
        public string Prefrence { get; set; }
        public List<BallotPlot> Plots { get; set; }
        public int Priority { get; set; }
        public int PlotSize { get; set; }
    }
    public class BallotSummary
    {
        public string Ident { get; set; }
        public int Count { get; set; }
    }
    public class PreferenceFilteredFiles
    {
        public string Prefrence { get; set; }
        public List<Sp_Get_Ballot_Qualifying_File_Result> Files { get; set; }
        public int Priority { get; set; }
        public int PlotSize { get; set; }
    }
    public class BallotPlotFileInventory
    {
        public int SR { get; set; }
        public string BLOCK { get; set; }
        public string PLOT { get; set; }
        public string SECTOR { get; set; }
        public string LOCATION { get; set; }
        public string SIZE { get; set; }
        public decimal AREA { get; set; }
        public string DIMENSION { get; set; }
        public string ROAD { get; set; }
        public string STATUS { get; set; }
        public string TYPE { get; set; }
        public string REF { get; set; }
    }
    public class BallotOutputModel
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
        public List<BallotResultModel> ballotFiles { get; set; }
        public List<PreferenceFilteredPlots> unballotedPlots { get; set; }
        public List<Sp_Get_Ballot_Qualifying_File_Result> unballotedFiles { get; set; }
        public List<BallotSummary> filesSummary { get; set; }
        public List<BallotSummary> plotSummary { get; set; }
        public List<BallotSummary> HoldPlotSummary { get; set; }
        public List<BallotComparativeReport> ComparativeReport { get; set; }

    }
    public class JsTreeModel
    {
        public long id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public bool children { get; set; } // if node has sub-nodes set true or not set false
    }
    public class Marketing_Receipt
    {
        public string ReceiptNo { get; set; }
        public string Name { get; set; }
        public string Father_Name { get; set; }
        public string Contact { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string AmountinWords { get; set; }
        public string PaymentType { get; set; }
        public string Bank { get; set; }
        public string Ch_Pay_Draft_No { get; set; }
        public string Project { get; set; }
        public string Size { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public string Text { get; set; }
        public string Block { get; set; }
        public string File_Plot_Number { get; set; }
        public string Company { get; set; }
    }
    public class BallotComparativeReport
    {
        public string Ident { get; set; }
        public int Total { get; set; }
        public int Hold { get; set; }
        public int Available { get; set; }
        public int Requests { get; set; }
        public int Balloted { get; set; }
    }
    public class EmployeeInfoUpdationRequest : GenericMISRequest
    {
        public long ManagerApproval { get; set; }
        public long Manager_Id { get; set; }
        public string ManagerName { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public Employee OldEmployeeDetails { get; set; }
        public Employee NewEmployeeDetails { get; set; }
    }
    public class FileReinstateRequest : GenericMISRequest
    {
        public long User_Approval { get; set; }
        public long User_Id { get; set; }
        public string User_Name { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public string Old_Status { get; set; }
        public string New_Status { get; set; }
        public string Module { get; set; }
    }
    public class EmployeeUpdationReviewModel
    {
        public EmployeeInfoUpdationRequest UpdationDetails { get; set; }
        public string Old_FirstRep { get; set; }
        public string Old_SecRep { get; set; }
        public string New_FirstRep { get; set; }
        public string New_SecRep { get; set; }
        public string OldJobStatus { get; set; }
        public string NewJobStatus { get; set; }
    }
    public class POStatuses
    {
        public List<Inventory_PurchaseOrder> PO { get; set; }
        public List<Sp_Get_PO_Completed_GRN_Result> GRN { get; set; }
    }
    public class AttendeeMonitorModel
    {
        public List<Ballot_Event_Entry> GuestsList { get; set; }
        public List<Sp_Get_BallotedPlotsList_Result> ServedFiles { get; set; }
    }

    public class Tempopp
    {
        public string OwnerName { get; set; }
        public string FileNum { get; set; }
        public int? Male { get; set; }
        public int? FeMale { get; set; }
        public int? Childern { get; set; }
        public DateTime? EntryTime { get; set; }
    }
    public class FileDetailsModel
    {
        public List<FileData> File { get; set; }
        public List<Sp_Get_FileInstallments_Result> Installments { get; set; }
        public List<Sp_Get_FileInstallments_Result> OtherInstallments { get; set; }
        public List<Discount> Discounts { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> ReceivedAmounts { get; set; }
    }
    public class StockIn_Sum
    {
        public long? Warehouse_Id { get; set; }
        public string Warehouse_Name { get; set; }
        public decimal? Qty { get; set; }
    }
    public class IssueReq
    {
        public long? Item_Id { get; set; }
        public long? Warehouse_Id { get; set; }
        public string Warehouse_Name { get; set; }
        public decimal? Qty { get; set; }
        public long? Prj_Id { get; set; }
        public string Prj_Name { get; set; }
        public long? Issue_To { get; set; }
        public string IssueTo_Name { get; set; }
        public string Issuer_Name { get; set; }
    }
    public class Stock_In_Details
    {
        public Inventory_Stock_In Inv { get; set; }
        public string Item_Name { get; set; }
        public string UOM { get; set; }
        public string Warehouse { get; set; }
    }
    public class Stock_Out_Details
    {
        public Inventory_Stock_Out Inv { get; set; }
        public string DemandOrder_No { get; set; }
        public string Item_Name { get; set; }
        public string UOM { get; set; }
        public string Warehouse { get; set; }
        public decimal? IssuanceRate { get; set; }
    }
    public class ArchiServiceDetails
    {
        public string ServiceName { get; set; }
        public decimal ServiceAmount { get; set; }
        public string Description { get; set; }
    }

    public class ArchiReceiptModel
    {
        public List<ArchiServiceDetails> archiServices { get; set; }
        public Sp_Get_Receipt_Parameter_ById_Result ReceiptinfoData { get; set; }
    }
    public class GRNItems
    {
        public long InsertionType { get; set; }
        public long Id { get; set; }
        public long? Item_Id { get; set; }
        public bool? IsAsset { get; set; }
        public DateTime? Expirey { get; set; }
        public DateTime? ActionDate { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public string Nums { get; set; }
        public long? GroupTag { get; set; }
    }
    public class DirectStock
    {
        public long? Item_Id { get; set; }
        public string DemandOrder_No { get; set; }
        public string Item_Name { get; set; }
        public string UOM { get; set; }
        public decimal? Qty { get; set; }
        public string Prj_Name { get; set; }
        public string IssueNote_No { get; set; }
        public DateTime? IssuedTo_Date { get; set; }
        public string IssueBy_Name { get; set; }
        public string IssueTo_Name { get; set; }
    }
    public class SpecialPref
    {
        public string Plot_Size { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
    }
    public class CommercialPossessionDataModel
    {
        public Sp_Get_CommercialData_Result CommercialDetails { get; set; }
        public Commercial_Rooms CommercialRoomData { get; set; }
        public List<Sp_Get_CommercialLastOwner_Result> CommercialOwnersDetails { get; set; }
        public List<CommercialShopBound> ShopBoundings { get; set; }
        public List<Commercial_Rooms> Positions { get; set; }
    }
    public class Inv_Dep
    {
        public Inventory Inv { get; set; }
        public List<Inventory_Department> InvDep { get; set; }
        public Sp_Get_COA_Head_Code_Result InvHead { get; set; }
    }
    public partial class Inventory_Purchase_Req
    {
        public bool ReqStatus { get; set; }
        public string SKU { get; set; }
        public string Project_Department { get; set; }
    }
    public partial class Sp_Get_Stock_In_Userid_Result
    {
        public bool PurReq_DemReq { get; set; }
        public string ReqTo_Dep { get; set; }
    }
    public class FinancialYear
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

    }
    public class Current_Head_Balance
    {
        public decimal? Opening { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }
    }
    public partial class Biding_Reserve_Plots
    {
        public decimal? GrandTotal { get { return PlotPrice +  CommisionAmount; } }
        //SpecialPrefAmount + DCAmount +
    }
    public class DealershipDealsDetailsModel
    {
        public List<DealerDeal> Deals { get; set; }
        public List<Biding_Reserve_Plots> DealPlots { get; set; }
    }
    public class Installment_Struct
    {
        public int Id { get; set; }
        public string Installment_Name { get; set; }
        public string Installment_Type { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime Datetime { get; set; }
        public int Installment_Plot_Id { get; set; }
        public Nullable<int> After { get; set; }
        public string Module { get; set; }
        public string Plot_Size { get; set; }
    }
    public class FinalizePurchaseQuotations
    {
        public long Id { get; set; }
        public string Item_Name { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public Nullable<long> Item_Id { get; set; }
        public Nullable<long> Group_Id { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<long> Prj_Id { get; set; }
        public string Prj_Name { get; set; }
        public string Project_Department { get; set; }
        public string STATUS { get; set; }
        public string RequestedBy_Name { get; set; }
        public Nullable<System.DateTime> Requested_Date { get; set; }
        public string Requisition_No { get; set; }
        public string Dep_Name { get; set; }
        public Nullable<System.DateTime> Deliver_Date { get; set; }
        public bool PurReq_DemReq { get; set; }
        public string ReqTo_Dep { get; set; }
        public string SKU { get; set; }
    }
    public class MsgModel
    {
        public long line { get; set; }
        public string text { get; set; }
        public string data { get; set; }
    }
    public class ArchiCustomerLedgerModel
    {
        public Architecture_Customers CustomerDetails { get; set; }
        public List<ArchiDepsNWithdrawals> LedgerDetails { get; set; }
    }

    public class ArchiDepsNWithdrawals
    {
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public DateTime ActionDate { get; set; }
    }

    public class AttendanceWrapperRequestModel
    {
        public string EnrollNumber { get; set; }
        public int IsInValid { get; set; }
        public int AttState { get; set; }
        public int VerifyMethod { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int WorkCode { get; set; }
        public long Device { get; set; }
    }
    public partial class Sp_Get_MIS_Requests_Result
    {
        public PlotDiscountRequest RequestDetails { get; set; }
        //public ManualAssignmentRequest ManualPlotAssignment_RequestDetails { get; set; }
        public EmployeeInfoUpdationRequest EmpDataUpdate_RequestDetails { get; set; }
        public LoanWaiverRequest WaiverDetails { get; set; }
        public FileReinstateRequest FileReinstate { get; set; }
        public DepartmentalTimeAdjustmentRequestModel TimeAdjustmentDetail { get; set; }
        public SwapPlotRequest PlotSwapsRequests { get; set; }
        public AdjPlotRequest PlotAdjRequests { get; set; }
    }
    public class LoanWaiverRequest : GenericMISRequest
    {
        public bool? FinanceManagerApproval { get; set; }
        public long FinanceManager_Id { get; set; }
        public string FinanceManagerName { get; set; }
        public bool? HrMgrApproval { get; set; }
        public long HRMgr_Id { get; set; }
        public long Inst_Id { get; set; }
        public string HrMgrName { get; set; }
        public DateTime HrStatusChangedAt { get; set; }
        public DateTime MgrStatusChangedAt { get; set; }
        public decimal WaiverAmount { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public decimal LoanTotalAmount { get; set; }
        public bool voucher_Print { get; set; }
        public long voucher_PrintedBy_Id { get; set; }
        public string voucher_PrintedBy_Name { get; set; }
        public DateTime voucher_PrintedDate { get; set; }

    }
    public class SimpleReturnMessage
    {
        public string Msg { get; set; }
        public bool Status { get; set; }
        public string _Log { get; set; }
    }
    public class DealerFormPrint
    {
        public List<Sp_Get_PlotData_Result> plt { get; set; }
        public Dealer dealer { get; set; }

    }
    public class PlotDiscountRequest : GenericMISRequest
    {
        public long ManagerApproval { get; set; }
        public long Manager_Id { get; set; }
        public string ManagerName { get; set; }
        public string ManagerRemarks { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public decimal DiscountAmount { get; set; }
        public string PlotNo { get; set; }
    }
    public class GenericMISRequest
    {
        public string DescriptionText { get; set; }
        public UrgencyStatus Urgency { get; set; }
    }
    public class DepartmentalTimeAdjustmentRequisition
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Reason { get; set; }
        public long EmployeeId { get; set; }
        public List<TimeAdjustmentFilteredList> AdjustmentsList { get; set; }
        public bool? Status { get; set; }
    }
    public class MarkedMultipleAttendanceModel
    {
        public long AttendanceId { get; set; }
        public int AdjustedFor { get; set; }
        public int? InHrs { get; set; }
        public int? InMins { get; set; }
        public int? OutHrs { get; set; }
        public int? OutMins { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public DateTime AttendanceDate { get; set; }
    }
    public class EmployeeMarkedMultipleAttendancePacket
    {
        public List<MarkedMultipleAttendanceModel> MarkedAttendance { get; set; }
        public long EmpId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
    }
    public class DepartmentalTimeAdjustmentRequestModel
    {
        public List<EmployeeMarkedMultipleAttendancePacket> DepartmentAttendanceData { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long DepId { get; set; }
        public string DepName { get; set; }
        public string Status { get; set; }
        public string AppliedBy { get; set; }
        public string AppliedAt { get; set; }
        public long ReqId { get; set; }
        public string HrStatus { get; set; }
        public string HrStatusMarkedBy_Name { get; set; }
        public long HrStatusMarkedBy_Id { get; set; }
        public DateTime HrStatusUpdationDate { get; set; }
        public string DepMgrStatus { get; set; }
        public string DepMgrStatusMarkedBy_Name { get; set; }
        public long DepMgrStatusMarkedBy_Id { get; set; }
        public DateTime DepMgrStatusUpdationDate { get; set; }
    }
    public class MultipleTimeAdjustmentModel
    {
        public List<MarkedMultipleAttendanceModel> MarkedAttendance { get; set; }
        public List<Sp_Get_AttendanceReport_Result> AttendanceRecord { get; set; }
        //abi roster template b ana hai
    }
    public class AdjustmentDelRequest
    {
        public long EmpId { get; set; }
        public long ReqId { get; set; }
        public List<long> AttendanceIds { get; set; }
    }
    public class TimeAdjustmentFilteredList
    {
        public DateTime AttendanceDate { get; set; }
        public DateTime CheckInLog { get; set; }
        public DateTime CheckOutLog { get; set; }
        public int AdjustType { get; set; }
        public long AttId { get; set; }
        public bool? Status { get; set; }
    }
    public class LoanCompleteDetails
    {
        public Loan_Installments InstallmentsInfo { get; set; }
        public LoanRequisition LoanData { get; set; }
        public List<Sp_GetEmpCompleteDesigs_Result> EmployeeDetails { get; set; }
    }
    public class RealEstateHierarichalViewModel
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public int Type { get; set; }
        public long? Parent { get; set; }
        public List<RealEstateHierarichalViewModel> Children { get; set; }
    }
    public class PlotStatusReport
    {
        public string Block { get; set; }
        public string Plot_Type { get; set; }
        public string Status { get; set; }
        public string PhaseName { get; set; }
        public int Total { get; set; }
    }
    public class PhaseReport
    {
        public string Phase { get; set; }
        public string Type { get; set; }
        public int Total { get; set; }
    }
    public class VerifiReport
    {
        public string Block { get; set; }
        public string Type { get; set; }
        public bool? Verified { get; set; }
        public int Total { get; set; }
    }
    public class ConstructionReport
    {
        public string Block { get; set; }
        public string Type { get; set; }
        public string DevelopStatus { get; set; }
        public int Total { get; set; }
    }
    public class CompiledReport
    {
        public List<PhaseReport> Phases { get; set; }
        public List<PlotStatusReport> PlotStatuses { get; set; }
        public List<VerifiReport> PlotVerifications { get; set; }
        public List<ConstructionReport> PlotConstructions { get; set; }
    }
    public class BiometricOwnerDetails
    {
        public long Id { get; set; }
        public long CNIC { get; set; }
        public string CNIC_Uparsed { get; set; }
        public string Name { get; set; }
    }
    public class BiometricVerification : BiometricOwnerDetails
    {
        public bool isMatched { get; set; }
        public string SavedTemplate { get; set; }
    }

    public class LoanAdvancePhysicalDocModel
    {
        public Employee EmployeeDetails { get; set; }
        public LoanRequisition LoanData { get; set; }
        public List<Sp_GetEmpCompleteDesigs_Result> Designations { get; set; }
        public List<LoanRequisition> PreviousLoanRequests { get; set; }
        public List<Loan_Installments> PreviousLoanInstallments { get; set; }
    }

    public class BlockTransferFeeModel
    {
        public string BlockName { get; set; }
        public long BlockId { get; set; }
        public decimal NC_Residential { get; set; }
        public decimal NC_Commercial { get; set; }
        public decimal NC_Residential_Dealer { get; set; }
        public decimal NC_Commercial_Dealer { get; set; }
        public decimal C_Residential { get; set; }
        public decimal C_Commercial { get; set; }
        public decimal C_Residential_Dealer { get; set; }
        public decimal C_Commercial_Dealer { get; set; }
    }
    public class CommercialTransferFeeModel
    {
        public string ProjectName { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ComType { get; set; }
        public string TransferAmountType { get; set; }
        public decimal Non_Dealer { get; set; }
        public decimal Dealer { get; set; }
    }

    public class PlotTransferFeeConfig
    {
        public List<BlocksTransferFee> TransferFees { get; set; }
    }

    public class DiscountConfigModel
    {
        public int PaidFileAmount { get; set; }
        public float DiscountPercentage { get; set; }
        public string Description { get; set; }
    }

    public class DiscountConfig
    {
        public List<DiscountConfigModel> DiscountConfigDetails { get; set; }
    }

    public class OverDuePolicyConfig
    {
        public int First_Warn_AfterInstallments_Pending { get; set; }
        public int Sec_Warn_AfterInstallments_Pending { get; set; }
        public int Sec_Warn_After_FirstWarn_Days { get; set; }
        public int Third_Warn_After_SecWarn_Days { get; set; }
        public int Third_Warn_After_Installments_Pending { get; set; }
    }

    public partial class Sp_Get_TransferRequestDetails_Plot_Result
    {
        public bool TaxExe { get; set; }
        public string TaxExeReason { get; set; }
        public bool SellerExe { get; set; }
    }

    public class OwnerDataGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Father_Husband { get; set; }
        public string Postal_Address { get; set; }
        public string Residential_Address { get; set; }
        public string Phone_Office { get; set; }
        public string Residential { get; set; }
        public string Mobile_1 { get; set; }
        public string Mobile_2 { get; set; }
        public string Email { get; set; }
        public string Occupation { get; set; }
        public string Nationality { get; set; }
        public string Date_Of_Birth { get; set; }
        public string CNIC_NICOP { get; set; }
        public string Nominee_Name { get; set; }
        public string Nominee_Relation { get; set; }
        public string Nominee_Address { get; set; }
        public string Nominee_CNIC_NICOP { get; set; }
        public string City { get; set; }
        public DateTime? DateTime { get; set; }
        public string Ownership_Status { get; set; }
        public long? FileId { get; set; }
        public long Group_Tag { get; set; }
    }

    public class SwapPlotRequestModel
    {
        public long FromPlot { get; set; }
        public string FromPlotNum { get; set; }
        public long ToPlot { get; set; }
        public string ToPlotNum { get; set; }
    }

    public class SwapPlotRequest : GenericMISRequest
    {
        public string ManagerApproval { get; set; }
        public long Manager_Id { get; set; }
        public string ManagerName { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public List<SwapPlotRequestModel> Swaps { get; set; }
    }

    public class ConfModel
    {
        public bool isAllowed { get; set; }
        public DateTime AllowedTill { get; set; }
        public DateTime LastHit { get; set; }
        public string PingLink { get; set; }
        public bool Status { get; set; }
    }

    public class Files_ABS_Summary
    {
        public string TypeName { get; set; }
        public long FileFormNumber { get; set; }
        public long? BallotPlotId { get; set; }
        public string Status { get; set; }
        public long StatusVal { get; set; }
    }
    public class DeptsModel
    {
        public long deptId { get; set; }
        public string deptName { get; set; }
    }

    public class Employee_leaves
    {
        public List<Sp_Get_ALLLeaveRequisition_Result> Applied { get; set; }
        public List<Sp_Get_EmployeeLeaves_Result> total { get; set; }
    }

    public partial class MIS_Reminders
    {
        [AllowHtml]
        public string UnParsedDesc { get; set; }
    }

    public class InventoryLedgerInserterModel
    {
        public long InsertionType { get; set; }
        public long? Item_Id { get; set; }
        public bool? IsAsset { get; set; }
        public DateTime? ActionDate { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public string Nums { get; set; }
        public long? GroupTag { get; set; }
    }

    public partial class Sp_Get_NDCFormDetails_Result
    {
        public bool IsCompany { get; set; }
        public string Block { get; set; }
    }

    public class VoucherPayableItems
    {
        public int HeadId { get; set; }
        public long GroupId { get; set; }
        public string Narration { get; set; }
    }
    public class Bill_Details
    {
        public int VendorId { get; set; }
        public string terms { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime DueDate { get; set; }
        public string BillNo { get; set; }
        public string Head_Code { get; set; }
        public string Head_Name { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public int userid { get; set; }
        public int grpid { get; set; }
        public int Head_id { get; set; }
        public string Head { get; set; }
        public string description { get; set; }
        public Nullable<int> Line { get; set; }
    }
    public class Vendor_serch
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string address { get; set; }
        public string Phone { get; set; }
    }

    public class HierarchicalNodeBalanceSheet
    {
        public List<HierarchicalNodeBalanceSheet> Decendants { get; } = new List<HierarchicalNodeBalanceSheet>();
        public string Head { get; }
        public string Code { get; }
        public int? Id { get; }
        public decimal Debit { get; }
        public decimal Credit { get; }
        public int Level { get; }
        public int AccType { get; }
        public HierarchicalNodeBalanceSheet(string Head, string Code, int? id, decimal debit, decimal credit, int level, int accType)
        {
            this.Head = Head;
            this.Code = Code;
            this.Id = id;
            this.Debit = debit;
            this.Credit = credit;
            this.Level = level;
            this.AccType = accType;
        }
    }
    public class VenBillDet
    {
        public decimal BillPaid { get; set; }
        public decimal BillRemaining { get; set; }
        public decimal BillTotal { get; set; }
        public string BillName { get; set; }
        public long BillId { get; set; }
    }
    public class PayableBillDetail
    {
        public List<VenBillDet> bills { get; set; }
        public string Name { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }
        public decimal Total { get; set; }
        public long Id { get; set; }
        public long HeadId { get; set; }
    }
    public partial class Sp_Get_Salaries_Gazetted_Result
    {
        public decimal? Gross_Salary { get { return Basic_salary + Allownces + Overtime + Bonus; } }
        public decimal? Stipend_Gross_Salary { get { return Stipend + Allownces + Bonus; } }
        public decimal? Stipend_Grand_Total { get { return Stipend - Ded_Loan - Ded_Advance + Allownces + Bonus; } }
    }
    public partial class Sp_Get_Salaries_Non_Gazetted_Result
    {
        public decimal? Gross_Salary { get { return Basic_salary + Allownces + Overtime + Bonus; } }
        public decimal? Stipend_Gross_Salary { get { return Stipend + Allownces + Bonus; } }
        public decimal? Stipend_Grand_Total { get { return Stipend - Ded_Loan - Ded_Advance + Allownces + Bonus; } }
    }
    public partial class Sp_Get_All_Employess_Salaries_Result
    {
        public decimal? Gross_Salary { get { return Basic_salary + Allownces + Overtime + Bonus; } }
        public decimal? Act_Salary { get { return ((Basic_salary/30)*(No_Of_Days-Absentee)) + ((Basic_salary / 30) * (Extra_Working_Days)); } }
        public decimal? Stipend_Gross_Salary { get { return Stipend + Allownces + Bonus; } }
        public decimal? Stipend_Grand_Total { get { return Stipend - Ded_Loan - Ded_Advance + Allownces + Bonus; } }
    }
    public class Installment
    {
        public long Id { get; set; }
        public long File_Plot_Id { get; set; }
        public string Installment_Name { get; set; }
        public string Installment_Type { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime Due_Date { get; set; }
        public string Status { get; set; }
    }
    public class PattyCashAmountsModel
    {
        public Nullable<decimal> Cashlimit { get; set; }
        public Nullable<decimal> Remaining { get; set; }
    }
    public class StockOutItemDetailsModel
    {
        public decimal? qty { get; set; }
        public decimal? rate { get; set; }
    }

    public class GeneralEntryDetailsModel
    {
        public string Narration { get; set; }
        public string HeadCode { get; set; }
        public string HeadName { get; set; }
        public int HeadId { get; set; }
        public long TransactionId { get; set; }
        public int Line { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
    }

    public class bankSalaryModel
    {
        public long SalaryId { get; set; }
        public Nullable<int> Bankhead { get; set; }
        public string InstrumentNo { get; set; }
        public Nullable<DateTime> InstrumentDate { get; set; }
    }
    public class PendingInvoicesModel
    {
        public string RecHead { get; set; }
        public int RecHeadId { get; set; }
        public string InvoiceNumber { get; set; }
        public long GroupId { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }
        public string Details { get; set; }
    }
    public class DepreciationInstallmentsModel
    {
        public string InstallmentName { get; set; }
        public string InstallmentType { get; set; }
        public long AssetId { get; set; }
        public string Status { get; set; }
        public bool IsCancelled { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaidDate { get; set; }
    }
    public class PettyCashAccountsDetModel
    {
        public long Id { get; set; }
        public Nullable<long> Employee_Id { get; set; }
        public string Employee_Name { get; set; }
        public string Employee_Code { get; set; }
        public Nullable<decimal> Cash_Limit { get; set; }
        public Nullable<decimal> Item_Limit { get; set; }
        public Nullable<long> Recorded_By { get; set; }
        public string Recorded_By_Name { get; set; }
        public Nullable<System.DateTime> Recorded_On { get; set; }
        public string Status { get; set; }
        public Nullable<int> Account_HeadId { get; set; }
        public Nullable<decimal> Balance { get; set; }
    }
    public class PremiumHomeRoomModel
    {
        public Sp_Get_CommercialData_Result RoomData { get; set; }
        public List<Sp_Get_Receipts_Param_Result> Receipts { get; set; }
        public List<Sp_Get_PropertyDeals_Param_Result> Leads { get; set; }
    }
    public class NewLeadModel
    {
        public long SalesPersonId { get; set; }
        public int LeadsCount { get; set; }
    }
    public class PremiumHomesLeadsData
    {
        public Sp_Get_Lead_Result Lead { get; set; }
        public List<Receipt> Receipts { get; set; }
        public List<Voucher> Vouchers { get; set; }
    }
    public class PremiumHomesData
    {
        public string Sr { get; set; }
        public string ApplicationNo { get; set; }
        public long Floor { get; set; }
        public string Type { get; set; }
        public decimal Area { get; set; }
        public string Location { get; set; }
        public decimal Rate { get; set; }

    }
    public class AdjPlotRequestModel
    {
        public long FromPlot { get; set; }
        public string FromPlotNum { get; set; }
        public long ToPlot { get; set; }
        public string ToPlotNum { get; set; }
    }
    public class AdjPlotRequest : GenericMISRequest
    {
        public string ManagerApproval { get; set; }
        public long Manager_Id { get; set; }
        public string ManagerName { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public List<AdjPlotRequestModel> Adjusts { get; set; }
    }
    public class ExcelMaterialStatementData
    {
        public string Sr { get; set; }
        public string Title { get; set; }
        public string Material { get; set; }
        public string Unit { get; set; }
        public double? Qty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
    }
    public class SalaryBankEmployee
    {
        public string EmployeeName { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string COA_HeadName { get; set; }
        public int COA_HeadId { get; set; }
    }
    public class Employee_Search_Bank
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DeptDesig { get; set; }
        public string EmpCode { get; set; }
    }
    public class SalaryStep3Model
    {
        public List<Sp_Get_All_Employess_Salaries_Result> Emp_Salaries { get; set; }
        public List<Employee> EmpBankList { get; set; }
    }
    public class PendingPurchaseorder
    {
        public List<Sp_Get_Pending_purchaseOrder_Result> PO { get; set; }
        public List<Sp_Get_PO_Completed_GRN_Result> GRN { get; set; }
    }
    public partial class Sp_Get_Pending_purchaseOrder_Result
    {
        public decimal? TotalVal { get { return (Purchase_Rate * Qty) + Tax; } }
        public string SKU { get; set; }
        public string Requistion_No { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public bool? Asset { get; set; }
        public string Project_Department { get; set; }
    }
    public class AttendanceMarkFromAway
    {
        public Employee EmployeeData { get; set; }
        public AttendanceData EmpAttData { get; set; }

    }
    public class CommercialLastOwnerDiscount
    {
        public Sp_Get_CommercialData_Result Data { get; set; }
        public List<Sp_Get_CommercialLastOwner_Result> OwnerData { get; set; }
        public List<PlotStatment> Report { get; set; }
        public Discount Discount { get; set; }
    }


    public class JournalEntriesPayable
    {
        public List<Sp_Get_JournalEntriesPayable_Parameter_Result> PayableData { get; set; }
        public List<Bill> itemDetails { get; set; }
    }
    public class JournalEntriesRecievable
    {
        public List<Sp_Get_JournalEntriesPayable_Parameter_Result> RecievebleData { get; set; }
        public List<Invoice> itemDetails { get; set; }
    }

    public class StandingOrderDetails
    {
        public int? HeadId { get; set; }
        public string InstNo { get; set; }
        public DateTime? InstDate { get; set; }
        public decimal? TotalAmnt { get; set; }
    }
    public class Project_And_Service_Purchase_Req
    {
        public List<Project> Project { get; set; }
        public List<Services_Purchase_Req> Service_PR { get; set; }
    }
    public class LandRecordDetails
    {
        public List<Sp_Get_LandDealsDetails_Result> LandDeals { get; set; }
        public List<Sp_Get_LandRecordDetails_Result> LandRecords { get; set; }
        public List<Sp_Get_LandPaymentScheduleDetails_Result> LandPayments { get; set; }
    }
    public partial class Sp_Reports_OverDue_Result
    {
        public decimal? Net_Receivable { get { return Total_Amount - TotalDiscounts; } }
        public decimal? Out_Standing { get { return Net_Receivable - Received_Amount; } }
    }
    public class MaterialShiftItems
    {
        public long item_Id { get; set; }
        public decimal Qty { get; set; }
        public long PItemId { get; set; }

    }
    public class CommercialTransferDetailData
    {
        public Sp_Get_CommercialData_Result Data { get; set; }
        public List<Sp_Get_CommercialLastOwner_Result> Owners { get; set; }
        public List<Sp_Get_CommercialInstallments_Result> Installments { get; set; }
        public List<Sp_Get_ReceivedAmounts_Result> Receipts { get; set; }
        public List<Sp_Get_TransferRequestDetails_ComId_Result> TransferTo { get; set; }
        public List<Discount> Discounts { get; set; }
        public File_Plot_Balance Balance { get; set; }
    }
    public partial class Sp_Get_TeamAttendanceToday_Result
    {
        public bool IsInShift { get; set; }
        public bool IsWeeklyOff { get; set; }
        public bool IsLeave { get; set; }
    }
    public class DealerMappers
    {
        public COA_Mapper Dealership { get; set; }
        public List<Sp_Get_COAMapper_Dealerlist_Result> DealerList { get; set; }
        public List<Sp_Get_COAMapper_Dealerlist_Commission_Result> DealerListCom { get; set; }
    }
    public class PettyCashMappers
    {
        public COA_Mapper PettyCash { get; set; }
        public List<Sp_Get_COAMapper_PettyCash_Result> PettyCash_Accounts { get; set; }
        public List<Sp_Get_COAMapper_PettyCash_Expense_Result> PettyCash_Expense{ get; set; }
        //public List<Sp_Get_COAMapper_Dealerlist_Commission_Result> DealerListCom { get; set; }
    }
    public class ReceivableMappers
    {
        public List<Sp_Get_COAMapper_BlocksResidential_Result> Residential { get; set; }
        public List<Sp_Get_COAMapper_BlocksCommercial_Result> Commercial { get; set; }
    }
    public class SalesMappers
    {
        public List<Sp_Get_COAMapper_Blocks_Sales_Residential_Result> Residential { get; set; }
        public List<Sp_Get_COAMapper_Blocks_Sales_Commercial_Result> Commercial { get; set; }
    }
    public class ServiceElectricMappers
    {
        public List<Sp_Get_COAMapper_Blocks_SC_Result> ServiceCharges_Res { get; set; }
        public List<Sp_Get_COAMapper_Blocks_EC_Result> Electric_Res { get; set; }
        public List<Sp_Get_COAMapper_Blocks_SC_Income_Result> ServiceCharges_Incom { get; set; }
        public List<Sp_Get_COAMapper_Blocks_EC_Income_Result> Electric_Incom { get; set; }
    }
    public class BankAccountsMapper
    {
        public List<Bank_Accounts> BankAccounts { get; set; }
        public List<Sp_Get_COAMapper_BankAccounts_Result> BankAccountsCOA { get; set; }
    }
    public class OnlineBankAccountsMapper
    {
        public List<Bank_Online_Accounts> OnlineBankAccounts { get; set; }
        public List<Sp_Get_COAMapper_OnlineBank_Result> OnlineBankAccountsCOA { get; set; }
    }
    public enum WarningLetterType
    {
        First_Warning,
        Second_Warning,
        Suspension,
        Termination,
    }
    public class IssueLetterData
    {
        public long? EmployeeId { get; set; }
        public string LetterType { get; set; }
        public string Reason { get; set; }
        public string Text { get; set; }
    }
    public partial class Sp_Get_COA_Head_Code_Result
    {
        public long? Comp_Id { get; set; }
    }
    public partial class Receipt
    {
        public string Company { get; set; }
    }
    //public partial class Sp_Get_Inventory_TotalStock_In_HOH_Result
    //{
    //    public decimal? Total_Out_Qty { get; set; }
    //    public decimal? Remaining { get { return Total_In_Qty - Total_Out_Qty; } }
    //}
    public partial class HOH_Inventory
    {

        public decimal? AvgRate { get; set; }
        public decimal? Total_In { get; set; }
        public decimal? Total_Out { get; set; }
        public decimal? Balance { get { return Total_In - Total_Out; } }
        public decimal? StockValue { get { return AvgRate * Balance; } }
    }
    public class TicketDetails
    {
        public List<Repository_Files> Files { get; set; }
        public Sp_Get_Ticket_Parameter_Result Ticket { get; set; }
    }
    public class EmployeePrintAttributes
    {

        public Sp_Get_EmpDetails_Print_Result EmpDetailsPrint { get; set; }
        public List<Sp_Get_Employee_Achievement_print_Result> Achievement { get; set; }
        public List<Sp_Get_Employee_Training_print_Result> Training { get; set; }
        public List<Sp_Get_Employee_Reward_print_Result> Reward { get; set; }
        public List<Sp_Get_Employee_Award_print_Result> Award { get; set; }
        public List<Sp_Get_Employee_Warning_Letter_Result> WarningLetter { get; set; }



    }
}
