using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
namespace MeherEstateDevelopers.Models
{
    public abstract class postOffice
    {
        public static void dispatchNotifyMail(string usr, string url, string msg, string subject, List<string> rec)
        {
            EmailService mailer = new EmailService();
            string body = transformMailTemplate(subject, msg, url);
            //mailer.SendEmail(body, subject, rec);
        }
        public static void dispatchNotifyMail(string usr, string url, string msg, string subject, string rec)
        {
            EmailService mailer = new EmailService();
            string body = transformMailTemplate(subject, msg, url);
            mailer.SendEmail(body, subject, rec);
        }

        public static void ClientDiscussionMail(string usr, string url, string dsc, string prj, string poster, List<string> rec)
        {
            EmailService mailer = new EmailService();
            string body = "";
           
         //   mailer.SendEmail(body, "", rec);
        }

        public static void DispatchMail(string subj, string []msgs, string url, string rec)
        {
            EmailService mailer = new EmailService();
            var body = "<!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\">"+
                "<head><style>html,body {margin: 0 auto !important;padding: 0 !important;height: 100% !important;width: 100% !important;}* {-ms-text-size-adjust: 100%;}"+
        "table,td {mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;}img {-ms-interpolation-mode: bicubic;}a {text-decoration: none;}</style></head>"+
                "<body><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ed1c24\">" +
"<tbody><tr><td align=\"center\" valign=\"top\" class=\"container\" style=\"padding:50px 10px;\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">" +
"<tbody><tr><td align=\"center\"><table width=\"650\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"mobile-shell\">" +
"<tbody><tr><td class=\"td\" bgcolor=\"#ffffff\" style=\"width:650px; min-width:650px; font-size:0pt; line-height:0pt; padding:0; margin:0; font-weight:normal;\">" +
"<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\"><tbody><tr><td class=\"p30-15-0\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">" +
"<tbody><tr><td style=\"padding-top: 40px;font-size:0pt;line-height:0pt;text-align: center;\"><img src=\"http://www.meherestate.ltd/assets/img/logo2.png\" width=\"50%\"></td>" +
"</tr><tr><td class=\"separator\" style=\"padding-top: 40px; border-bottom:4px solid #000000; font-size:0pt; line-height:0pt;\">&nbsp;</td></tr>" +
"</tbody></table></td></tr></tbody></table><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\"><tbody><tr>" +
"<td class=\"p30-15\" style=\"padding: 70px 30px 70px 30px;\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr>" +
"<td class=\"h2 center pb10\" style=\"color:#000000;font-size: 32px;line-height:60px;text-align:center;padding-bottom:10px;\">"+ subj +" </td></tr>" +
          FormatMsgsView(msgs) +
"<tr><td align=\"center\"><table width=\"120\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr>" +
"<td class=\"text-button\" style=\"background: #ed1c24;color:#ffffff;font-size:14px;line-height:18px;text-align:center;padding:12px;\"><a href=\"" + url + "\" target=\"_blank\" class=\"link-white\" style=\"color:#ffffff; text-decoration:none;\"><span class=\"link-white\" style=\"color:#ffffff; text-decoration:none;\">Go To Link</span></a></td>" +
"</tr></tbody></table></td></tr></tbody></table></td></tr><tr><td class=\"fluid-img img-center pb70\" style=\"font-size:0pt; line-height:0pt; text-align:center;\"><img src=\"images/separator.jpg\" width=\"590\" height=\"1\" border=\"0\" alt=\"\"></td>"+
"</tr></tbody></table><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td class=\"footer\" style=\"padding: 0px 30px 30px 30px;\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody>"+
"<tr><td class=\"separator\" style=\"border-bottom:4px solid #000000; font-size:0pt; line-height:0pt;\"></td></tr><tr>"+
"<td class=\"text-footer\" style=\"padding-top: 30px; color:#9fadd4; font-size:12px; line-height:22px; text-align:center;\">"+
"Powered by SA Systems <br><a href=\"#\" target=\"_blank\" class=\"link4\" style=\"color:#9fadd4; text-decoration:none;\"><span class=\"link4\" style=\"color:#9fadd4; text-decoration:none;\"></span>"+
"</a></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></body></html>";
           
            Thread notiReader = new Thread(() => mailer.SendEmail(body, rec,   subj));
            notiReader.Start();
        }

        public static void DispatchSAGMail(string subj, string[] msgs, string rec)
        {
            EmailService mailer = new EmailService();
            var body =
                "<!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><style>html, body {margin: 0 auto !important;padding: 0 !important;height: 100% !important;width: 100% !important;}* {-ms-text-size-adjust: 100%;}table, td {mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;}img {-ms-interpolation-mode: bicubic;}a {text-decoration: none;}</style></head>" +
"<body><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ed1c24\"><tbody>" +
"<tr><td align=\"center\" valign=\"top\" class=\"container\" style=\"padding:50px 10px;\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr>" +
"<td align=\"center\"><table width=\"650\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"mobile-shell\"><tbody><tr><td class=\"td\" bgcolor=\"#ffffff\" style=\"width:650px; min-width:650px; font-size:0pt; line-height:0pt; padding:0; margin:0; font-weight:normal;\">" +
"<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\"><tbody><tr><td class=\"p30-15-0\"><table width=\"100%\" style=\"background-color: #f2e36f\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr>" +
"<td style=\"padding-top: 40px;font-size:0pt;line-height:0pt;text-align: center;\"><img src=\"https://www.meharestate.com/assets/img/footerlogo.png\" width=\"50%\"></td></tr><tr><td class=\"separator\" style=\"padding-top: 40px; border-bottom:4px solid #000000; font-size:0pt; line-height:0pt;\">&nbsp;</td></tr></tbody></table>" +
"</td></tr></tbody></table><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\"><tbody><tr><td class=\"p30-15\" style=\"padding: 70px 30px 70px 30px;\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td class=\"h2 center pb10\" style=\"color:#000000;line-height:30px;text-align:center;padding-bottom:10px;\"><span style=\"font-size:24px\">Plot Alert: </span>&nbsp;<span style=\"font-size:22px\">"+ subj +"</td></tr>" +
"<tr><td class=\"h5 center blue pb30\" style=\"font-size:20px;line-height:26px;text-align:left;padding-bottom:30px;\">" + FormatMsgsView(msgs)  +"</td></tr></tbody></table></td></tr><tr><td class=\"fluid-img img-center pb70\" style=\"font-size:0pt; line-height:0pt; text-align:center;\"><img src=\"images/separator.jpg\" width=\"590\" height=\"1\" border=\"0\" alt=\"\"></td>" +
"</tr></tbody></table><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td class=\"footer\" style=\"padding: 0px 30px 30px 30px;\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td class=\"separator\" style=\"border-bottom:4px solid #000000; font-size:0pt; line-height:0pt;\"></td></tr><tr><td class=\"text-footer\" style=\"padding-top: 30px; color:#9fadd4; font-size:12px; line-height:22px; text-align:center;\">Powered by SA Systems <br><a href=\"#\" target=\"_blank\" class=\"link4\" style=\"color:#9fadd4; text-decoration:none;\"><span class=\"link4\" style=\"color:#9fadd4; text-decoration:none;\"></span>" +
"</a></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></body></html>";

            Thread notiReader = new Thread(() => mailer.SendEmail(body, rec, subj));
            notiReader.Start();
        }


        private static string transformMailTemplate(string subj,string msg, string url)
        {
            return "";
        }

        private static string FormatMsgsView(string [] msgs)
        {
            string test =  FormatMsgsView(msgs, 0);
            return test;
        }

        private static string FormatMsgsView(string[] msgs, int ind)
        {
            string res = string.Empty;
            if (ind < (msgs.Length - 1))
            {
                res = FormatMsgsView(msgs, ind + 1);
            }
            return "<tr><td class=\"h5 center blue pb30\" style=\"font-size:20px;line-height:26px;text-align:center;padding-bottom:30px;\">"+msgs[ind] +"</td></tr>" + res;
        }
    }
}