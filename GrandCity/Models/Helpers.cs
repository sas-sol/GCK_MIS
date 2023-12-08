using System;
using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using MeherEstateDevelopers.Models;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Net;
using System.Threading;

namespace MeherEstateDevelopers.Models
{
    public class Helpers
    {
        private readonly Grand_CityEntities db = new Grand_CityEntities();
        public Modules Module { get; set; }
        public Types Type { get; set; }
        public Helpers()
        {

        }
        public Helpers(Modules modules, Types types)
        {
            this.Module = modules;
            this.Type = types;
        }
        public long RandomNumber()
        {
            int milliseconds = 2;
            Thread.Sleep(milliseconds);
            Random random = new Random((int)DateTime.Now.Ticks);
            return random.Next();
        }
        public string stringRandomNumber()
        {
            int milliseconds = 2;
            Thread.Sleep(milliseconds);
            Random random = new Random((int)DateTime.Now.Ticks);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());

        }
        public long RandomNumber(int Seed)
        {
            Random random = new Random(Seed);
            return random.Next();
        }
        private string QRCode(string code, string Id)
        {
            string status = "error";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            var logopath = HostingEnvironment.MapPath("~/assets/static/images/logo2.png");
            var fileName = Path.GetFileName(Id);
            var galleryDirectoryPath = HostingEnvironment.MapPath("~/Repository/QR Codes/");
            var pathMain = Path.Combine(galleryDirectoryPath, Id +".png");

            if (!Directory.Exists(galleryDirectoryPath))
            {
                Directory.CreateDirectory(galleryDirectoryPath);
            }
            Bitmap b = new Bitmap(logopath);
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitMap = qrCode.GetGraphic(20, Color.Black, Color.White, b))
                {
                    bitMap.Save(pathMain, ImageFormat.Png);
                    bitMap.Save(ms, ImageFormat.Png);
                    status = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            return status;
        }
        private string QR_Code_Text_Generator(object[] Data, string uniqueNo, List<NameNumber> nameNumbers)
        {
            string FinalText = "";
            if (this.Module == Modules.Dealers)
            {
                
                FinalText = "Dealership: " + Data[0] + ".\nMembership No: " + Data[1] + ".\nOwners: " + string.Join(",",nameNumbers.Select(x=>x.Name)) +".\nCode: " + uniqueNo;
            }
            else if(this.Module == Modules.DealersForm)
            {
                FinalText = "Dealership: " + Data[0] + ".\nPlot No: " + Data[1] + ".\nPlot Size: " + Data[2] + ".\nBlock: " + Data[3] + ".\nCode: " + uniqueNo;
            }
            else if (this.Module == Modules.FileManagement && this.Type == Types.FileForm)
            {
                if (Data[4].ToString() == "4" || Data[4].ToString() == "6")
                {
                    FinalText = "File /Application Form: " + Data[0] + ".\nPhase: " + Data[1] + ".\nBlock: " + Data[2] + ".\nDealership: " + Data[3] + ".\nPlot Size: " + Data[4] + ".\nCode: " + uniqueNo;
                }
                else
                {
                    FinalText = "File /Application Form: " + Data[0] + ".\nPhase: " + Data[1] + ".\nBlock: " + Data[2] + ".\nDealership: " + Data[3] + ".\nPlot Size: " + Data[4] + ".\nCode: " + uniqueNo;
                }
            }
            else if (this.Module == Modules.PlotManagement && this.Type == Types.Booking)
            {
                FinalText = "Plot No: " + Data[0] + ".\nBlock: " + Data[1] +  ".\nType: " + Data[2] + ".\nPlot Size: " + Data[3] + ".\nCode: " + uniqueNo;
            }
            else if (this.Module == Modules.PlotManagement )
            {
                string name = Data[0].ToString();
                name = name.Replace("/", "-");
                FinalText = "Name: "+ name + ".\nPlot No: " + Data[1] + ".\nPhase: " + Data[2] + ".\nBlock: " + Data[3] + ".\nPlot Size: " + Data[4] + ".\nDimension: " + Data[5] + ".\nCode: " + uniqueNo;
            }
            else if (this.Module == Modules.ShopManagement)
            {
                FinalText = "Form Number: " + Data[0] + ".\nArea: " + Data[1] + ".\nFloor: " + Data[2] + ".\nProject: " + Data[3] + ".\nCode: " + uniqueNo;
            }
            else if (this.Module == Modules.OfficeManagement)
            {
                FinalText = "Form Number: " + Data[0] + ".\nArea: " + Data[1] + ".\nFloor: " + Data[2] + ".\nProject: " + Data[3] + ".\nCode: " + uniqueNo;
            }
            else if (this.Module == Modules.CommercialManagement)
            {
                FinalText = "Form Number: " + Data[0] + ".\nArea: " + Data[1] + ".\nFloor: " + Data[2] + ".\nProject: " + Data[3] + ".\nCode: " + uniqueNo;
            }
            return FinalText;
        }
        private long AddQRCode(string QR_text, string QRcode_img, long Unique_Number, Modules Module, Types Type)
        {
            var mod = this.Module.ToString();
            var type = this.Type.ToString();
            return Convert.ToInt32( db.Sp_Add_QRCode(QR_text, Unique_Number, mod, type).FirstOrDefault());
        }
        public QRCodeReturn GenerateQRCode(object[] Data)
        {
            var Unique_Number = this.RandomNumber();
            string QR_text = this.QR_Code_Text_Generator(Data, Unique_Number.ToString(),null);
            string QRcode_img = "";
            if (this.Module == Modules.Dealers)
            {
                QRcode_img = this.QRCode(QR_text, Data[0].ToString());
            }
            if (this.Module == Modules.DealersForm)
            {
                QRcode_img = this.QRCode(QR_text, Data[1].ToString()+"-"+Data[3]);
            }
            else if (this.Module == Modules.FileManagement)
            {
                 QRcode_img = this.QRCode(QR_text, Data[0].ToString());
            }
            else if (this.Module == Modules.PlotManagement)
            {
                string name = Data[0].ToString().Replace("/", "-");
                QRcode_img = this.QRCode(QR_text, Data[2].ToString() + "_"+ Data[1].ToString() + "_"+ name );
            }
            else if (this.Module == Modules.PlotManagement && this.Type == Types.Booking)
            {
                QRcode_img = this.QRCode(QR_text, Data[0].ToString() + "_" + Data[1].ToString());
            }
            else if (this.Module == Modules.ShopManagement)
            {
                QRcode_img = this.QRCode(QR_text, Data[3].ToString() + "_" + Data[0].ToString());
            }
            else if (this.Module == Modules.OfficeManagement)
            {
                QRcode_img = this.QRCode(QR_text, Data[3].ToString() + "_" + Data[0].ToString());
            }
            else if (this.Module == Modules.CommercialManagement)
            {
                QRcode_img = this.QRCode(QR_text, Data[3].ToString() + "_" + Data[0].ToString());
            }
            long Id = AddQRCode(QR_text, QRcode_img, Unique_Number, this.Module, this.Type);
            var QR_Data = new QRCodeReturn { Id = Id, QR_Image = QRcode_img };
            return QR_Data;
        }
        public QRCodeReturn GenerateQRCode(object[] Data,List<NameNumber> nameNumbers)
        {
            var Unique_Number = this.RandomNumber();
            string QR_text = this.QR_Code_Text_Generator(Data, Unique_Number.ToString(),nameNumbers);
            string QRcode_img = this.QRCode(QR_text, Data[0].ToString());
            long Id = AddQRCode(QR_text, QRcode_img, Unique_Number, this.Module, this.Type);
            var QR_Data = new QRCodeReturn { Id = Id, QR_Image = QRcode_img };
            return QR_Data;
        }
        public bool GetBool(int? val)
        {
            bool returnval = false;
            if(val == 1)
            {
                returnval = true;
            }
            return returnval;
        }
        public bool SaveBase64Image(string Imagestring, string Path, string Name)
        {
            Imagestring = Imagestring.Replace("data:image/png;base64,", string.Empty);
            Imagestring = Imagestring.Replace("data:image/jpeg;base64,", string.Empty);
            try
            {

                byte[] imageBytes = Convert.FromBase64String(Imagestring);

                File.WriteAllBytes(Path, imageBytes);
                return true;
            }
            catch (Exception e)
            {

                return false;
            }

        }
        public QRCodeReturn SAPremiumGenerateQRCode(object[] Data)
        {
            var Unique_Number = this.RandomNumber();
            string QR_text = this.SAPHQR_Code_Text_Generator(Data, Unique_Number.ToString(), null);
            string QRcode_img = "";
            QRcode_img = this.PremiumHomesQRCode(QR_text, Data[0].ToString());
            long Id = SaPremiumHomesAddQRCode(QR_text, QRcode_img, Unique_Number, this.Module, this.Type);
            var QR_Data = new QRCodeReturn { Id = Id, QR_Image = QRcode_img };
            return QR_Data;
        }
        private long SaPremiumHomesAddQRCode(string QR_text, string QRcode_img, long Unique_Number, Modules Module, Types Type)
        {
            var mod = this.Module.ToString();
            var type = this.Type.ToString();
            return Convert.ToInt32(db.Sp_Add_QRCode(QR_text, Unique_Number, mod, type).FirstOrDefault());
        }
        private string PremiumHomesQRCode(string code, string applicationNo)
        {
            string status = "error";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            var logopath = HostingEnvironment.MapPath("~/assets/static/images/logosaph.png");
            var fileName = Path.GetFileName(applicationNo);
            var galleryDirectoryPath = HostingEnvironment.MapPath("~/Repository/QR Codes SAPH/");
            var pathMain = Path.Combine(galleryDirectoryPath, applicationNo + ".png");

            if (!Directory.Exists(galleryDirectoryPath))
            {
                Directory.CreateDirectory(galleryDirectoryPath);
            }
            Bitmap b = new Bitmap(logopath);
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitMap = qrCode.GetGraphic(20, Color.Black, Color.White, b))
                {
                    bitMap.Save(pathMain, ImageFormat.Png);
                    bitMap.Save(ms, ImageFormat.Png);
                    status = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            return status;
        }
        private string SAPHQR_Code_Text_Generator(object[] Data, string uniqueNo, List<NameNumber> nameNumbers)
        {
            string FinalText = "";
            FinalText = "File /Application Form: " + Data[0] + ".\nProject: " + Data[1] + ".\nType: " + Data[2] + ".\nCode: " + uniqueNo;
            return FinalText;
        }

    }
    public static class GeneralMethods
    {

        public static string NumberClearence(string Number)
        {
            string F_Num = "";
            F_Num = Number.Replace(" ",String.Empty);
            F_Num = Number.Replace("-",String.Empty);
            F_Num = Number.Replace("_",String.Empty);
            string first_xter = F_Num.Substring(0, 1);
            if (first_xter == "0")
            {
                F_Num = "92" + F_Num.Remove(0, 1);
            }
            return F_Num;
        }
        public static string CharacterConversion(string stringdate)
        {
            string spearator = stringdate.Replace("Ѭ", " _ ");
            return spearator;
        }
        public static string GetIPAddress()
        {
            string HostName = Dns.GetHostName();
            string Adress = Dns.GetHostByName(HostName).AddressList[0].ToString();
            return Adress;
        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        public static decimal Percentage(decimal? val, decimal? totalval)
        {
            return (Convert.ToDecimal( val) / Convert.ToDecimal(totalval)) * 100;
        }
        public static decimal RemoveComa(string Amount)
        {
            return Convert.ToDecimal(Amount.Replace(",", ""));
        }
    }


}