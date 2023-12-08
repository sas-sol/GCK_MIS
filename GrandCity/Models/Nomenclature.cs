using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MeherEstateDevelopers.Models
{

    public static class Nomenclature
    {

        public static List<BanksModel> Banks()
        {
            return new List<BanksModel>{
                new BanksModel {Name="Al Baraka Bank" },
                new BanksModel {Name="Allied Bank" },
                new BanksModel {Name="Apna Microfinance Bank" },
                new BanksModel {Name="Askari Bank" },
                new BanksModel {Name="Bank Al Habib" },
                new BanksModel {Name="Bank Alfalah" },
                new BanksModel {Name="Bank Islami" },
                new BanksModel {Name="Bank of Punjab" },
                new BanksModel {Name="Barclays" },
                new BanksModel {Name="Burj Bank" },
                new BanksModel {Name="Citibank N.A." },
                new BanksModel {Name="Dubai Islamic Bank" },
                new BanksModel {Name="FINCA Microfinance Bank" },
                new BanksModel {Name="Faysal Bank" },
                new BanksModel {Name="First Women Bank Limited" },
                new BanksModel {Name="First Microfinance Bank" },
                new BanksModel {Name="Habib Bank" },
                new BanksModel {Name="Habib Metropolitan Bank" },
                new BanksModel {Name="ICBC" },
                new BanksModel {Name="JS Bank" },
                new BanksModel {Name="KASB Bank Limited" },
                new BanksModel {Name="MCB" },
                new BanksModel {Name="MIB" },
                new BanksModel {Name="Meezan Bank Limited" },
                new BanksModel {Name="Mobilink Microfinance Bank LTD" },
                new BanksModel {Name="NIB" },
                new BanksModel {Name="National Bank of Pakistan" },
                new BanksModel {Name="Standard Chartered" },
                new BanksModel {Name="Samba Bank" },
                new BanksModel {Name="Silk Bank" },
                new BanksModel {Name="Sindh Bank" },
                new BanksModel {Name="Soneri Bank" },
                new BanksModel {Name="Summit Bank" },
                new BanksModel {Name="Tameer Bank" },
                new BanksModel {Name="U Microfinance Bank" },
                new BanksModel {Name="United Bank" },
                new BanksModel {Name="UnCategorized" }
            };
        }
        public static List<BanksModel> Cities()
        {
            return new List<BanksModel>{
                new BanksModel {Name="Abbottabad"},
                new BanksModel {Name="Adezai"},
                new BanksModel {Name="Ahmadpur East"},
                new BanksModel {Name="Ahmed Nager Chatha"},
                new BanksModel {Name="Alipur"},
                new BanksModel {Name="Alpuri"},
                new BanksModel {Name="Arifwala"},
                new BanksModel {Name="Attock"},
                new BanksModel {Name="Ayubia"},
                new BanksModel {Name="Badin"},
                new BanksModel {Name="Bahawalnagar"},
                new BanksModel {Name="Bahawalpur"},
                new BanksModel {Name="Banda Daud Shah"},
                new BanksModel {Name="Bannu"},
                new BanksModel {Name="Batkhela"},
                new BanksModel {Name="Battagram"},
                new BanksModel {Name="Bhakkar"},
                new BanksModel {Name="Bhalwal"},
                new BanksModel {Name="Birote"},
                new BanksModel {Name="Burewala"},
                new BanksModel {Name="Chakdara"},
                new BanksModel {Name="Chakwal"},
                new BanksModel {Name="Chaman"},
                new BanksModel {Name="Charsadda"},
                new BanksModel {Name="Chichawatni"},
                new BanksModel {Name="Chillianwala"},
                new BanksModel {Name="Chiniot"},
                new BanksModel {Name="Chishtian"},
                new BanksModel {Name="Chitral"},
                new BanksModel {Name="Dadu"},
                new BanksModel {Name="Daggar"},
                new BanksModel {Name="Dargai"},
                new BanksModel {Name="Darya Khan"},
                new BanksModel {Name="Daska"},
                new BanksModel {Name="Dera Allah Yar"},
                new BanksModel {Name="Dera Ghazi Khan"},
                new BanksModel {Name="Dera Ismail Khan"},
                new BanksModel {Name="Dera Murad Jamali"},
                new BanksModel {Name="Dhaular"},
                new BanksModel {Name="Digri"},
                new BanksModel {Name="Dina"},
                new BanksModel {Name="Dinga"},
                new BanksModel {Name="Dipalpur"},
                new BanksModel {Name="Diplo"},
                new BanksModel {Name="Dir"},
                new BanksModel {Name="Dokri"},
                new BanksModel {Name="Drosh"},
                new BanksModel {Name="Faisalabad"},
                new BanksModel {Name="Fateh Jang"},
                new BanksModel {Name="Ghakhar Mandi"},
                new BanksModel {Name="Ghotki"},
                new BanksModel {Name="Gilgit"},
                new BanksModel {Name="Gojra"},
                new BanksModel {Name="Gujar Khan"},
                new BanksModel {Name="Gujranwala"},
                new BanksModel {Name="Gujrat"},
                new BanksModel {Name="Gwadar"},
                new BanksModel {Name="Haala"},
                new BanksModel {Name="Hafizabad"},
                new BanksModel {Name="Hangu"},
                new BanksModel {Name="Haripur"},
                new BanksModel {Name="Haroonabad"},
                new BanksModel {Name="Hasilpur"},
                new BanksModel {Name="Haveli Lakha"},
                new BanksModel {Name="Hub"},
                new BanksModel {Name="Hyderabad"},
                new BanksModel {Name="Islamabad"},
                new BanksModel {Name="Islamkot"},
                new BanksModel {Name="Jacobabad"},
                new BanksModel {Name="Jalalpur Jattan"},
                new BanksModel {Name="Jampur"},
                new BanksModel {Name="Jamshoro"},
                new BanksModel {Name="Jaranwala"},
                new BanksModel {Name="Jauharabad"},
                new BanksModel {Name="Jhang"},
                new BanksModel {Name="Jhelum"},
                new BanksModel {Name="Jungshahi"},
                new BanksModel {Name="Kalabagh"},
                new BanksModel {Name="Kalat"},
                new BanksModel {Name="Kamalia"},
                new BanksModel {Name="Kamoke"},
                new BanksModel {Name="Kandiaro"},
                new BanksModel {Name="Karachi"},
                new BanksModel {Name="Karak"},
                new BanksModel {Name="Karor Lal Esan"},
                new BanksModel {Name="Kashmore"},
                new BanksModel {Name="Kasur"},
                new BanksModel {Name="Keti Bandar"},
                new BanksModel {Name="Khairpur"},
                new BanksModel {Name="Khanewal"},
                new BanksModel {Name="Khanpur"},
                new BanksModel {Name="Kharan"},
                new BanksModel {Name="Kharian"},
                new BanksModel {Name="Khushab"},
                new BanksModel {Name="Khuzdar"},
                new BanksModel {Name="Kohat"},
                new BanksModel {Name="Kot Adu"},
                new BanksModel {Name="Kotri"},
                new BanksModel {Name="Lahore"},
                new BanksModel {Name="Lakki Marwat"},
                new BanksModel {Name="Lalamusa"},
                new BanksModel {Name="Larkana"},
                new BanksModel {Name="Latamber"},
                new BanksModel {Name="Layyah"},
                new BanksModel {Name="Liaquat Pur"},
                new BanksModel {Name="Lodhran"},
                new BanksModel {Name="Loralai"},
                new BanksModel {Name="Madyan"},
                new BanksModel {Name="Mailsi"},
                new BanksModel {Name="Mamoori"},
                new BanksModel {Name="Mandi Bahauddin"},
                new BanksModel {Name="Mansehra"},
                new BanksModel {Name="Mardan"},
                new BanksModel {Name="Mastuj"},
                new BanksModel {Name="Mastung"},
                new BanksModel {Name="Matiari"},
                new BanksModel {Name="Mehar"},
                new BanksModel {Name="Mehrabpur"},
                new BanksModel {Name="Mian Channu"},
                new BanksModel {Name="Mianwali"},
                new BanksModel {Name="Mingora"},
                new BanksModel {Name="Mirpur Khas"},
                new BanksModel {Name="Mithani"},
                new BanksModel {Name="Mithi"},
                new BanksModel {Name="Moro"},
                new BanksModel {Name="Multan"},
                new BanksModel {Name="Muridke"},
                new BanksModel {Name="Murree"},
                new BanksModel {Name="Muzaffarabad"},
                new BanksModel {Name="Muzaffargarh"},
                new BanksModel {Name="Nagarparkar"},
                new BanksModel {Name="Narowal"},
                new BanksModel {Name="Naudero"},
                new BanksModel {Name="Naushahro Feroze"},
                new BanksModel {Name="Naushara"},
                new BanksModel {Name="Nawabshah"},
                new BanksModel {Name="Nazimabad"},
                new BanksModel {Name="Nowshera"},
                new BanksModel {Name="Nushki"},
                new BanksModel {Name="Okara"},
                new BanksModel {Name="Paharpur"},
                new BanksModel {Name="Pakpattan"},
                new BanksModel {Name="Pasni"},
                new BanksModel {Name="Pattoki"},
                new BanksModel {Name="Peshawar"},
                new BanksModel {Name="Pir Mahal"},
                new BanksModel {Name="Qila Didar Singh"},
                new BanksModel {Name="Quetta"},
                new BanksModel {Name="Rabwah"},
                new BanksModel {Name="Rahim Yar Khan"},
                new BanksModel {Name="Raiwind"},
                new BanksModel {Name="Rajanpur"},
                new BanksModel {Name="Rajo Khanani"},
                new BanksModel {Name="Ranipur"},
                new BanksModel {Name="Ratodero"},
                new BanksModel {Name="Rawalpindi"},
                new BanksModel {Name="Renala Khurd"},
                new BanksModel {Name="Rohri"},
                new BanksModel {Name="Sadiqabad"},
                new BanksModel {Name="Safdarabad"},
                new BanksModel {Name="Sahiwal"},
                new BanksModel {Name="Saidu Sharif"},
                new BanksModel {Name="Sakrand"},
                new BanksModel {Name="Sanghar"},
                new BanksModel {Name="Sangla Hill"},
                new BanksModel {Name="Sarai Alamgir"},
                new BanksModel {Name="Sargodha"},
                new BanksModel {Name="Shahbandar"},
                new BanksModel {Name="Shahdadkot"},
                new BanksModel {Name="Shahdadpur"},
                new BanksModel {Name="Shahpur Chakar"},
                new BanksModel {Name="Shakargarh"},
                new BanksModel {Name="Sheikhupura"},
                new BanksModel {Name="Shikarpaur"},
                new BanksModel {Name="Sialkot"},
                new BanksModel {Name="Sibi"},
                new BanksModel {Name="Sohawa"},
                new BanksModel {Name="Soianwala"},
                new BanksModel {Name="Sukkur"},
                new BanksModel {Name="Swabi"},
                new BanksModel {Name="Swat"},
                new BanksModel {Name="Talagang"},
                new BanksModel {Name="Tando Adam Khan"},
                new BanksModel {Name="Tando Allahyar"},
                new BanksModel {Name="Tangi"},
                new BanksModel {Name="Tank"},
                new BanksModel {Name="Taxila"},
                new BanksModel {Name="Thall"},
                new BanksModel {Name="Thatta"},
                new BanksModel {Name="Timergara"},
                new BanksModel {Name="Toba Tek Singh"},
                new BanksModel {Name="Tordher"},
                new BanksModel {Name="Turbat"},
                new BanksModel {Name="Umerkot"},
                new BanksModel {Name="Usta Mohammad"},
                new BanksModel {Name="Vehari"},
                new BanksModel {Name="Wah Cantonment"},
                new BanksModel {Name="Warah"},
                new BanksModel {Name="Wazirabad"},
                new BanksModel {Name="Zhob"},
                new BanksModel {Name="Mirpur-AJK"},
                new BanksModel {Name="Kotli-AJK"},
                new BanksModel {Name="Bhimber-AJK"},
                new BanksModel {Name="Muzaffarabad-AJK"},
                new BanksModel {Name="Hattian-AJK"},
                new BanksModel {Name="Neelam Valley-AJK"},
                new BanksModel {Name="Poonch-AJK"},
                new BanksModel {Name="Haveli-AJK"},
                new BanksModel {Name="Bagh-AJK"},
                new BanksModel {Name="Sudhanoti-AJK"},
                new BanksModel {Name="Athmuqam-AJK"},
                new BanksModel {Name="Rawalakot-AJK"},
                new BanksModel {Name="Hattian Bala-AJK"},
                new BanksModel {Name="Palandri-AJK"},
                new BanksModel {Name="Other"},
            };
        }
        public static List<NameValuestring> PaymentTypes()
        {
            return new List<NameValuestring>{
                new NameValuestring {Value = "Cash", Name="Cash"},
                new NameValuestring {Value = "Cheque", Name="Cheque"},
                new NameValuestring {Value = "BankDraft", Name="Bank Draft"},
                new NameValuestring {Value = "PayOrder", Name="Pay Order"},
                new NameValuestring {Value = "Online_Cash", Name="Online - Cash"},
                new NameValuestring {Value = "Online_Cheque", Name="Online - Cheque"},
                new NameValuestring {Value = "Online_Payorder   ", Name="Online - PayOrder"},
                new NameValuestring {Value = "Online_Bankdraft", Name="Online - BankDraft"},
                new NameValuestring {Value = "Debit/Credit", Name="Debit/Credit"}
            };
        }
        public static List<NameValuestring> DishoneredReason()
        {
            return new List<NameValuestring>{
                new NameValuestring {Value = "Funds insufficient", Name="Funds insufficient"},
                new NameValuestring {Value = "Payment stopped by drawer", Name="Payment stopped by drawer"},
                new NameValuestring {Value = "Cheque is postdated/undated/stale with incorrect date", Name="Cheque is postdated/undated/stale with incorrect date"},
                new NameValuestring {Value = "Amount in words and figures differs", Name="Amount in words and figures differs"},
                new NameValuestring {Value = "Drawer's signature required/incomplete/differs/Forged/Missing/Anuthorized", Name="Drawer's signature required/incomplete/differs/Forged/Missing/Anuthorized"},
                new NameValuestring {Value = "Intercity/Same day/All previous clearing stamps require cancellation", Name="Intercity/Same day/All previous clearing stamps require cancellation"},
                new NameValuestring {Value = "Alteration in date/amount in figures/amount in words/payee name requires drawer's fill signature", Name="Alteration in date/amount in figures/amount in words/payee name requires drawer's fill signature"},
                new NameValuestring {Value = "Company's rubber stamp required", Name="Company's rubber stamp required"},
                new NameValuestring {Value = "Not drawn on us", Name="Not drawn on us"},
                new NameValuestring {Value = "Collecting Bank's endorsement/discharge unsigned/irregular/required/illegible", Name="Collecting Bank's endorsement/discharge unsigned/irregular/required/illegible"},
                new NameValuestring {Value = "Clearing stamp required/irregular", Name="Clearing stamp required/irregular"},
                new NameValuestring {Value = "Payee's discharge on revenue stamps required", Name="Payee's discharge on revenue stamps required"},
                new NameValuestring {Value = "Unclaim Deposit", Name="Unclaim Deposit"},
                new NameValuestring {Value = "Date is Missing", Name="Date is Missing"},
                new NameValuestring {Value = "Fake Instrument", Name="Fake Instrument"},
                new NameValuestring {Value = "Tempered Instrument", Name="Tempered Instrument"},
                new NameValuestring {Value = "Incomplete Instrument", Name="Incomplete Instrument"},
                new NameValuestring {Value = "Payment Instrument Contains extraneous Matter/conditional statments", Name="Payment Instrument Contains extraneous Matter/conditional statments"},
                new NameValuestring {Value = "Suspious", Name="Suspious"},
                new NameValuestring {Value = "Nonresident Account. Form A-7 required", Name="Nonresident Account. Form A-7 required"},
                new NameValuestring {Value = "Bank special Crossing required", Name="Bank special Crossing required"},
                new NameValuestring {Value = "Stamp Date is invalid", Name="Stamp Date is invalid"},
                new NameValuestring {Value = "Payment Stopped on order of Legal/Court or any Law enforcement agency.", Name="Payment Stopped on order of Legal/Court or any Law enforcement agency."},
                new NameValuestring {Value = "Payment cannot be processed due to force majeure event", Name="Payment cannot be processed due to force majeure event"},
                new NameValuestring {Value = "Blocked/Frozen", Name="Blocked/Frozen"},
                new NameValuestring {Value = "Dormant Account", Name="Dormant Account"},
                new NameValuestring {Value = "Photo Account", Name="Photo Account"},
                new NameValuestring {Value = "System Error", Name="System Error"},
                new NameValuestring {Value = "Closed/Inactive", Name="Closed/Inactive"},
                new NameValuestring {Value = "Endorsement Incomplete/Forged", Name="Endorsement Incomplete/Forged"},
                new NameValuestring {Value = "Collecting Banks endorsements/Discharged/Unsigned/Irregullar/Illegal", Name="Collecting Banks endorsements/Discharged/Unsigned/Irregullar/Illegal"},
                new NameValuestring {Value = "Duplicate Instrument/Instrument lodge again in clearing", Name="Duplicate Instrument/Instrument lodge again in clearing"},
                new NameValuestring {Value = "Account Limit Exceeded", Name="Account Limit Exceeded"}
            };
        }
        public static List<NameValuestring> DepreciationMethods()
        {
            return new List<NameValuestring>{
                new NameValuestring {Value = "Straight Line", Name="Straight Line"},
                new NameValuestring {Value = "Reducing Balance", Name="Reducing Balance"},
                //new NameValuestring {Value = "Double Decline", Name="Double Decline"},
                //new NameValuestring {Value = "Straight Line - Months", Name="Straight Line - Months"}
            };
        }
        public static List<MeasurementType> MeasurementUnits()
        {
            return new List<MeasurementType>
            {
                new MeasurementType{ ID = 1, Unit = "KG" },
                new MeasurementType { ID = 2, Unit = "Litre" }
            };
        }

        public static List<string> HighChartsColors()
        {
            return new List<string>
            {
                "#2f7ed8",
            "#0d233a",
             "#8bbc21",
             "#910000",
            "#492970",
            "#f28f43",
             "#77a1e5",
             "#c42525",
             "#a6c96a",
            "#1aadce"
        };
        }
        public static List<NameValuestring> COA()
        {
            return new List<NameValuestring>{
               new NameValuestring {Value = "1-00-000-0000-00000", Name="Assets"},
                new NameValuestring {Value = "1-01-000-0000-00000", Name="Current Assets"},
                new NameValuestring {Value = "1-01-001-0000-00000", Name="Cash"},
                new NameValuestring {Value = "1-01-001-0001-00000", Name="Cash In hand"},
                new NameValuestring {Value = "1-01-001-0001-00001", Name="Cash In hand"},
                new NameValuestring {Value = "1-01-001-0002-00000", Name="Petty Cash"},
                new NameValuestring {Value = "1-01-002-0000-00000", Name="Cash at Bank"},
                new NameValuestring {Value = "1-01-002-0001-00000", Name="Bank"},
                new NameValuestring {Value = "1-01-002-0001-00001", Name="Account 1"},
                new NameValuestring {Value = "1-01-003-0000-00000", Name="Pre-payments"},
                new NameValuestring {Value = "1-01-003-0001-00000", Name="Pre-Paid Rent"},
                new NameValuestring {Value = "1-01-003-0001-00001", Name="House rent"},
                new NameValuestring {Value = "1-01-004-0000-00000", Name="Receivables"},
                new NameValuestring {Value = "1-01-004-0001-00000", Name="Employee Receivables"},
                new NameValuestring {Value = "1-01-004-0001-00001", Name="Loan Receivable"},
                new NameValuestring {Value = "1-01-004-0001-00002", Name="Advance Receivable"},
                new NameValuestring {Value = "1-01-005-0000-00000", Name="Inventory"},
                new NameValuestring {Value = "1-01-005-0001-00000", Name="Building Material"},
                new NameValuestring {Value = "1-01-005-0001-00001", Name="Cement"},
                new NameValuestring {Value = "1-01-005-0002-00000", Name="Electric Material"},
                new NameValuestring {Value = "1-01-005-0002-00001", Name="Wire"},
                new NameValuestring {Value = "1-01-005-0003-00000", Name="Plumbing Material"},
                new NameValuestring {Value = "1-01-005-0003-00001", Name="PVC Pipes"},
                new NameValuestring {Value = "1-02-000-0000-00000", Name="Non Current Assets"},
                new NameValuestring {Value = "1-02-001-0000-00000", Name="PPE"},
                new NameValuestring {Value = "1-02-001-0001-00000", Name="Buildings"},
                new NameValuestring {Value = "1-02-001-0001-00001", Name="Head office Building"},
                new NameValuestring {Value = "1-02-001-0002-00000", Name="Plants and Equipment"},
                new NameValuestring {Value = "1-02-001-0002-00001", Name="Gensets"},
                new NameValuestring {Value = "1-02-001-0003-00000", Name="Furniture"},
                new NameValuestring {Value = "1-02-001-0003-00001", Name="Office Furniture"},
                new NameValuestring {Value = "1-02-001-0004-00000", Name="Machinery"},
                new NameValuestring {Value = "1-02-001-0004-00001", Name="Excevators"},
                new NameValuestring {Value = "1-02-001-0004-00002", Name="Tractors"},
                new NameValuestring {Value = "1-02-001-0005-00000", Name="Vehicles"},
                new NameValuestring {Value = "1-02-001-0005-00001", Name="Cars"},
                new NameValuestring {Value = "1-02-001-0006-00000", Name="Computer Systems"},
                new NameValuestring {Value = "1-02-001-0006-00001", Name="Laptops"},
                new NameValuestring {Value = "1-02-002-0000-00000", Name="Intengible Assets"},
                new NameValuestring {Value = "1-02-002-0001-00000", Name="Licences"},
                new NameValuestring {Value = "1-02-002-0001-00001", Name="Licences"},
                new NameValuestring {Value = "1-02-002-0002-00000", Name="Trade Mark"},
                new NameValuestring {Value = "1-02-002-0002-00001", Name="Trade Mark"},
                new NameValuestring {Value = "1-02-002-0003-00000", Name="Patents"},
                new NameValuestring {Value = "1-02-002-0003-00001", Name="Patents"},
                new NameValuestring {Value = "1-02-002-0004-00000", Name="Other Intangible Assets"},
                new NameValuestring {Value = "1-02-002-0004-00001", Name="Other Intangible Assets"},
                new NameValuestring {Value = "1-02-003-0000-00000", Name="Long Term Investment"},
                new NameValuestring {Value = "1-02-004-0000-00000", Name="Long Term Loan"},
                new NameValuestring {Value = "1-02-004-0001-00000", Name="Long Term Loan"},
                new NameValuestring {Value = "1-02-004-0001-00001", Name="Long Term Loan"},
                new NameValuestring {Value = "2-00-000-0000-00000", Name="Liabilites"},
                new NameValuestring {Value = "2-01-000-0000-00000", Name="Current Liabilities"},
                new NameValuestring {Value = "2-01-001-0000-00000", Name="Trade Payables"},
                new NameValuestring {Value = "2-01-001-0001-00000", Name="Vendors"},
                new NameValuestring {Value = "2-01-002-0000-00000", Name="Salary Payables"},
                new NameValuestring {Value = "2-01-002-0001-00000", Name="Salary Payables"},
                new NameValuestring {Value = "2-01-002-0001-00001", Name="Salary Payables"},
                new NameValuestring {Value = "2-02-000-0000-00000", Name="Non Current Current Liabilities"},
                new NameValuestring {Value = "2-02-001-0000-00000", Name="Long term loan"},
                new NameValuestring {Value = "2-02-001-0001-00000", Name="Long term loan"},
                new NameValuestring {Value = "2-02-001-0001-00001", Name="Long term loan"},
                new NameValuestring {Value = "3-00-000-0000-00000", Name="Equity"},
                new NameValuestring {Value = "3-01-000-0000-00000", Name="Equity Capital"},
                new NameValuestring {Value = "3-01-001-0000-00000", Name="Equity Capital"},
                new NameValuestring {Value = "3-01-001-0001-00000", Name="Equity Capital"},
                new NameValuestring {Value = "3-01-001-0001-00001", Name="Equity Capital"},
                new NameValuestring {Value = "3-02-000-0000-00000", Name="Retained Earning"},
                new NameValuestring {Value = "3-01-002-0000-00000", Name="Retained Earning"},
                new NameValuestring {Value = "3-01-002-0001-00000", Name="Retained Earning"},
                new NameValuestring {Value = "3-01-002-0001-00001", Name="Retained Earning"},
                new NameValuestring {Value = "4-00-000-0000-00000", Name="Income"},
                new NameValuestring {Value = "4-01-000-0000-00000", Name="Sales"},
                new NameValuestring {Value = "4-01-001-0000-00000", Name="Sales"},
                new NameValuestring {Value = "4-01-001-0001-00000", Name="Sales"},
                new NameValuestring {Value = "4-01-001-0001-00001", Name="Sales"},
                new NameValuestring {Value = "4-02-000-0000-00000", Name="Other Income"},
                new NameValuestring {Value = "4-02-001-0000-00000", Name="Other Income"},
                new NameValuestring {Value = "4-02-001-0001-00000", Name="Other Income"},
                new NameValuestring {Value = "4-02-001-0001-00001", Name="Other Income"},
                new NameValuestring {Value = "5-00-000-0000-00000", Name="Expenses"},
                new NameValuestring {Value = "5-01-000-0000-00000", Name="Administration and Distribution Expenses"},
                new NameValuestring {Value = "5-01-001-0000-00000", Name="Payrol"},
                new NameValuestring {Value = "5-01-001-0001-00000", Name="Payrol"},
                new NameValuestring {Value = "5-01-001-0001-00001", Name="Payrol"},
                new NameValuestring {Value = "5-01-002-0000-00000", Name="Utilities"},
                new NameValuestring {Value = "5-01-002-0001-00000", Name="Utilities"},
                new NameValuestring {Value = "5-01-002-0001-00001", Name="Utilities"},
                new NameValuestring {Value = "5-01-003-0000-00000", Name="Fuel Expenses"},
                new NameValuestring {Value = "5-01-003-0001-00000", Name="Fuel Expenses"},
                new NameValuestring {Value = "5-01-003-0001-00001", Name="Fuel Expenses"},
                new NameValuestring {Value = "5-01-04-0000-00000", Name="Misc Admin expenses"},
                new NameValuestring {Value = "5-01-04-0001-00000", Name="Misc Admin expenses"},
                new NameValuestring {Value = "5-01-04-0001-00001", Name="Misc Admin expenses"},
                new NameValuestring {Value = "5-02-000-0000-00000", Name="Repair and Maintenance"},
                new NameValuestring {Value = "5-02-001-0000-00000", Name="Vehicle Maintenance"},
                new NameValuestring {Value = "5-02-001-0001-00000", Name="Vehicle Maintenance"},
                new NameValuestring {Value = "5-02-001-0001-00001", Name="Vehicle Maintenance"}

            };
        }
    }
    public enum Ownership_Status
    {
        Transfered,
        Cancelled,
        Owner,
        Pending,
        Refunded
    }
    public enum Cancellations
    {
        Plot_Cancellation,
        File_Cancellation,
        Shop,
        Apartment,
        Office,
        Receipt_Refund
    }
    public enum Types
    {
        Cheque,
        BankDraft,
        Payorder,
        Reciept,
        Bounceslip,
        Transfer,
        DealerRegister,
        FileForm,
        Plots,
        FileRegisteration,
        Installment,
        Booking,
        Office,
        Apartment,
        Shop,
        Registery,
        AssetManagement,
        DealerAdvance,
        Sports,
        PaymentReceivable
    }
    public enum RoleGroup
    {
        Roles_And_Responsibilites,
        Dealership_Management,
        Cash_Counter,
        Files_Management,
        Plots_Management,
        File_Delivery,
        Transfer_Requests,
        Audit,
        Reports,
        Tickets
    }
    public enum Modules
    {
        PlotManagement,
        Dealers,
        FileManagement,
        CommercialManagement,
        ApartmentManagement,
        OfficeManagement,
        ShopManagement,
        LeadManagement,
        AssetManagement,
        LoanManagement,
        AdvanceSalaryManagement,
        ProjectManagement,
        DealersForm,
        Sports,
        Project_Discussion,
        Vendor_Payment,
        Accounts_Management,
        PattyCash_Management
    }
    public enum LeaveTypes
    {
        Annual,
        Sick,
        Casual,
        CPL
    }
    public enum FilePlotsReportType
    {
        Total,
        Registered,
        Cancelled,
        RegisteredActive,
        RegisteredInActive,
        Ballotted,
        NotBallotted
    }
    public enum LeaveStatus
    {
        Approved,
        Rejected,
        Pending
    }
    public enum DealerShipDeals
    {
        File_Advance,
        Dealer_Advance,
        Dealer_Commission,
        Dealer_Commission_Adjust
    }
    public enum InstallmentPaymentStatus
    {
        Paid = 1,
        NotPaid = 0,
        Pending = 2
    }
    public enum PaymentMethod
    {
        Cash,
        Cheque,
        BankDraft,
        PayOrder,
        Online_Cash,
        Online_Cheque,
        Online_Payorder,
        Online_Bankdraft,
    }
    public enum PaymentMethodStatuses
    {
        Approved = 1,
        Pending = 2,
        Dishonored = 3,
        Deposited = 4
    }
    public enum Status
    {
        Approved = 1,
        Pending = 2,
        Reject = 0,
        Wrong_Entry = 3
    }
    public enum ReceiptTypes
    {
        Advance,
        Installment,
        Booking,
        Confirmation,
        BookingToken,
        ServiceCharges,
        Electricity_Charges,
        Transfer,
        New_Connection_Charges,
        Fines_And_Penalties,
        Power_Of_Attorney,
        Duplicate_Allotment_Letter,
        Duplicate_Customer_File,
        Cancellation,
        DealershipRegisteration,
        Dealership_Security,
        Registry_Charges,
        Other_Recovery,
        Membership_Fee,
        Membership_Monthly_Fee,
        Out_Station_Charges,
        Other_Transfer_Charges,
        Advance_Tax_236_C,
        Advance_Tax_236_K,
        Architecture_Fees,
        Contra,
        Subsidiary_Recovery,
        Posession_Charges,
        DealerAdvance,
        Receivable_Receipt,
        LoanSettlement
    }
    public enum Payments
    {
        Receipt_Refund,
        Advance_Salary,
        Loan,
        Vendor_Payment,
        Cancellation,
        Temporary,
        Adjustment,
        Payment_Voucher
    }
    public enum ProjectCategory
    {
        General,
        Commercial,
        Building
    }
    public enum PlotType
    {
        Commercial,
        Residential
    }
    public enum PlotDevelopStatus
    {
        Constructed,
        Non_Constructed
    }
    public enum PlotsStatus
    {
        Registered,
        Available_For_Sale,
        Hold,
        Cancelled,
        Disputed,
        Repurchased,
        Temporary_Cancelled,
        Cancelled_Not_Refunded,
        Repurchased_Not_Refunded,
        Token
    }
    public enum FileStatus
    {
        Rejected = 0,
        Registered = 1,
        Pending = 2,
        Open = 5,
        Cancelled = 4,
        Temporary_Cancelled = 6,
        Hold = 8,
        Repurchased = 9,
        Cancelled_Due_Amount = 10,
        Disputed = 11,
        Refund = 13,
        Suspend = 14,
        Repurchased_Not_Refunded = 17,
        Cancelled_Not_Refunded = 15
    }

    public enum MeterSourceTypes
    {
        Electric = 1,
        Water = 2
    }
    public enum CallCenterFeedback
    {
        Receipts_Issue = 1,
        Unanswered = 2,
        Contact_Number_Invalid = 3,
        Customer_Data_Invalid = 4
    }
    public enum AssetType
    {
        Car,
        Mobile,
        Cellular_Sim,
        Motorcycle,
        Laptop_Computer,
        Desktop_Computer
    }
    public enum SalaryStatus
    {
        Finance_Approved,
        HR_Approved,
        Inprocess,
        Received
    }
    public enum AllotmentLetterStatus
    {
        NotGenerated = 0,
        Generated = 1,
        OkForGeneration = 2,
        Requested = 3
    }
    public enum ActivityType
    {
        Add_Receipt,
        Details_Access,
        Allotment_Letter_Delivered,
        Signed,
        Plot_Verified,
        Allotment_Letter_Generate,
        Allotment_Letter_Requested,
        Dimension_Create,
        Dimension_Updation,
        Receipt_Verified,
        Delete_Receipt,
        Plan_Updation,
        Add_Plot_Owner,
        Record_Upatation,
        Page_Access,
        Plot_Registeration,
        Pre_Verification,
        Customer_File,
        Plot_Status_Updation,
        Plot_NOC,
        Transfer_Request,
        Plot_Transfered,
        Warning_Letter,
        Update_Receipt,
        Delete_OwnerShip,
        UnSigned,
        Possession_Letter,
        Possession_Letter_Generated,
        Shop_Transfered,
        Rental,
        HR_Added_Employee,
        HR_Comp_Dept_Desig_Update,
        HR_Employee_Info_Updation,
        HR_EducationInfo_Added,
        HR_EducationInfo_Removed,
        HR_LoanApplied,
        HR_LoanStatusChanged,
        HR_ManualComment,
        HR_Gratuity_Request,
        HR_Arrears,
        Voucher_Request,
        Voucher_Update,
        Vendor_Register,
        Vendor,
        Vendor_Terms,
        Services,
        Salaries,
        Roles,
        Reports,
        Arreares_Salaries,
        Admin_Facilities,
        BioMetric,
        Call_Center,
        Attandance,
        Balloting,
        Commercial,
        Banking,
        Dealership,
        General_Entry,
        Procurement,
        Inventory,
        Lead,
        PR_Send_Back,
        PO_Send_Back,
        Material_Shift
    }
    public enum LeadsStatus
    {
        Token,
        Mature,
        Close,
        Loose,
        Initial_Discussion
    }
    public enum NewLeadsStatus
    {
        Token,
        Loose,
        Mature,
        Initial_Discussion,
        Close
    }
    public enum CompDepDes
    {
        Company,
        Department,
        Designation
    }
    public enum LeaveStatuses
    {
        Rejected,
        Approved,
        Pending
    }
    public enum EmployeeStatus
    {
        Suspended,
        Terminate,
        Resigned
    }
    public enum LoanStatus
    {
        Rejected,
        Approved,
        Pending,
        InProcess,
        Paid,
        Advance_Salary,
        Loan
    }
    public enum AssetsStatus
    {
        Return
    }
    public enum ProjectStatus
    {
        Pending,
        Live,
        Closed
    }
    public enum SalaryAdditions
    {
        AllowncesAndBonus,
        DeductionAndTaxes,
        OtherDeduction,
        Others
    }
    public enum DemandOrderStatus
    {
        Deliver,
        NonDeliver,
    }
    public enum PurchaseRequisitionStatus
    {
        Pending_Approval,
        Pending,
        Demand_Approval,
        Quotation_Approval,
        Purchase_Order_Generated,
        Purchase_Order_NotPrinted,
    }

    public enum ProjectType
    {
        Recurring,
        NonRecurring
    }

    public enum ServiceChargeRequestsType
    {
        Manual_Meter_Reading,
        Arrears_Removal,
        Bill_Waiver,
        Fine_Waiver,
        Trail_Balance
    }

    public enum ServiceType
    {
        Electricity,
        ServiceCharges
    }

    public enum ServiceChargeModule
    {
        Block,
        Plot,
        Phase
    }

    public enum ServiceChargesInstallmentsStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum MonthlyInstallmentStatus
    {
        Paid,
        Pending,
    }

    public enum MIS_Modules
    {
        HR,
        ServiceCharges,
        Salary_Tax,
        Salary_Bank
    }

    public enum Attendance_Checkout_Method
    {
        Machine_Checkout,
        MIS_Checkout,
        Not_CheckedOutYet,
        Auto_CheckOut,
        Manual_Attendance
    }

    public enum TimeAdjustmentType
    {
        CheckOut,
        CheckIn
    }

    public enum AttendanceExemptionStatus
    {
        Pending,
        Approved,
        Rejected,
        Removed,
    }

    public enum QuestionType
    {
        Multiple_Choice_Question,
        Single_Choice_Question,
        Short_Para_Question
    }

    public enum QuestionnaireType
    {
        Interview_Questionnaire,
        SMART_Goal_Questionnaire,
        Probation_Appraisal_Questionnaire
    }
    public enum SMARTGoalStatus
    {
        Goal_Achieved,
        In_Progress,
        Given_Up
    }
    public enum QuestionnaireFeedbackStatus
    {
        Filled,
        Pending
    }
    public enum COA_Config_Types
    {
        Petty_Cash
    }
    public enum UrgencyStatus
    {
        Very_Urgent = 3,
        Urgent = 2,
        Normal = 1
    }
    public enum MISModuleType
    {
        Plot_Management,
        File_Management,
        Plot_Transfer,
        Transfer_Fee_Config,
        File_Transfer_Fee_Config,
        Commercial_Transfer_Fee_Config
    }
    public enum RequestType
    {
        Plot_Discount_Requests,
        Investment_Payment_Out,
        Reinstate_With_Fine
    }
    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected,
        InProcess,
    }
    public enum Transaction_Type
    {
        Debit,
        Credit
    }
    public enum Voucher_Type
    {
        CRV,
        BRV,
        CPV,
        BPV,
        JV
    }
    public enum COA_Nature
    {
        Assets,
        Liability,
        Equity,
        Income,
        Expense
    }


    public enum COA_OpeningClosingBalance_Type
    {
        Opening,
        Closing
    }
    public enum Asset_Type
    {
        Owned,
        Leased
    }
    public enum ReminderStatus
    {
        Open,
        Closed,
        Delete
    }
    public enum NotifierMsg
    {
        //Tickets Notifier Types
        Ticket,
        TicketComment,
        TicketStatusUpdate,
        TicketForward,
        // Plot Notification Type

        Verified,
        Unverified,
        Full_Paid_Plots,
        Request_For_Verification_File,
        Request_For_Verification_Plot,

        // Demand Requisitions
        Demand_Requisitions,
        Demand_Approved,

        // Issuance
        Issuance_Request,
        Item_Issued,

        // Procurement
        Pending_Purchase_Orders,

        // Human Resource

        Probation_Finish
    }

    public enum NotifyTo
    {
        Registry,
        Files,
        Audit,
        Plots,
        Full_Paid,
        Directors,
        Purchase_Heads,
        Probation
    }
    public enum NotifyType
    {
        Ticket,
        Plots,
        FullPaid_Plot,
        Files,
        Procurment,
        Probation_Finish
    }
    public enum COA_Mapper_Modules
    {
        Human_Resource,
        Files_Plots,
        Dealership,
        Commercial,
        Procurement,
        Accounting,
        Projects,
        Petty_Cash,
        Amenities,
        Sales,
        ODD,
        Subsidary,
        SA_Gardens_Payable
    }
    public enum COA_Mapper_ModuleTypes
    {
        Plots_Receivables_Commercial,
        Plots_Receivables_Residential,
        Plots_Sales_Commercial,
        Plots_Sales_Residential,
        Commercial_Project_Receivables,
        Commercial_Project_Sales,
        Plot_Sales_Commission_Expense,
        Plot_Sales_Commission_Payable,
        Service_Charges_Receivables,
        Service_Charges_Income,
        Electricity_Charges_Receivables,
        Electricity_Charges_Income,
        Fines_And_Penalties,
        Reinstate_Charges,
        Posession_Charges,
        Duplicate_Allotment_Letter,
        Registry_Charges,
        Power_Of_Attorney,
        Out_Station_Charges,
        Other_Recovery,
        Other_Transfer_Charges,
        Advance_Tax_236_C,
        Advance_Tax_236_K,
        New_Connection_Charges,
        Duplicate_Customer_File,
        Plots_Transfer_Charges_Commercial,
        Plots_Transfer_Charges_Residential,
        Commercial_Project_Transfer_Charges,
        Plots_Discount_Receivable,
        Plots_Discount_Expense,
        Plots_Discount_Commercial,
        Plots_Discount_Residential,
        //
        Vendor,
        Inventory,
        Advance_Trade_payable,
        General_Trade_Payable,
        Vendors_List,
        Purchase_Need_Supervision,
        Item_List,
        //
        Dealership,
        Cash_Account,
        Bank_Account,
        Receivables_Accounts,
        Payables_Accounts,
        Dealership_List,
        Dealership_Commission,
        //
        Salary_Overtime,
        Salary_Bonus,
        Salary_Allowance,
        Salary_BasicSalary,
        Salary_TaxDeduction,
        Salary_LoanDeduction,
        Salary_AdvanceDeduction,
        Salary_OtherDeductions,
        Salary_ExtraFuelCharges,
        Salary_NetSalary,
        Loan,
        Advance,
        //
        Projects_List,
        // Amenities
        Gym,
        Riding_Club,
        // Commercial
        Commercial_Project_Discount,
        // SA Marketing
        Token,
        // ODD
        Architecture_Fee,
        //
        Cancellation,
        //
        Dealership_Registration,
        //
        Online,
        //
        Liability,
        Petty_Cash_Account,
        Petty_Cash_Expense,
        //
        PDC_Payable,
        PDC_Receivable

    }
    public enum COA_Mapper_HeadType
    {
        Transaction_Head,
        Account_Head
    }
    public enum WbdResourceType
    {
        Equipment,
        Labour,
        Material
    }
    public enum InstallmentTypes
    {
        Installments = 1,
        Development_Charges = 2,
        Advance = 3,
        Confirmation = 6,
        After_Time = 4,
        Special_Preference = 5,
        Other_Charges = 0,
    }
}