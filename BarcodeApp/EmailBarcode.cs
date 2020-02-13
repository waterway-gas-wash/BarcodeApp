using System;
using System.IO;
using BarcodeApp.Services;
using BarcodeApp.Models;
using log4net;
using System.Net.Mail;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace BarcodeApp
{
    public class EmailBarcode
    {

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static (string, string) BarcodeEmail(string first, string last, string email)
        {
            try
            {
                // Create new member from data being passed
                MemberRequest mr = new MemberRequest();
                mr.first = first;
                mr.last = last;
                mr.email = email;

                // Get new API token
                Token t = new Token();
                REST _r = new REST();
                Response r = new Response();

                t.grant_type = "password";
                t.username = "api@waterway.com";
                t.password = "Waterway99!";

                t.token = _r.GetToken(t);

                // Only create if this is a new email
                EmailAddress emailAddress = new EmailAddress();
                emailAddress.foundEmail = _r.CheckEmail(t, mr);

                if (emailAddress.foundEmail.Success == "false" || emailAddress.foundEmail.Id == "gkeller@waterway.com")
                {
                    // Create new Blue member
                    CreateNewMember newMember = new CreateNewMember();
                    newMember.memberCreated = _r.CreateMember(t, mr);

                    // If new member created successfully, then send email
                    if (newMember.memberCreated.Success == "true")
                    {
                        // Add note to member for tracking
                        MemberNote mn = new MemberNote();
                        mn.MemberNumber = newMember.memberCreated.Id;
                        mn.Note = "Started as one time use";
                        mn.NoteCreated = _r.CreateNote(t, mn);

                        if (mn.NoteCreated == "true")
                        {
                            // create UPC for barcode
                            //string createUPC = "0727" + newMember.memberCreated.Id + "0";

                            //LocationRequest lr = new LocationRequest();
                            //lr.UnitNumber = "62";
                            //lr.UnusedCode = _r.GetUseCode(t, lr);

                            GetSingleUseCode su = new GetSingleUseCode();
                            su.UnitNumber = "62";
                            su.DealName = "ExpressClean30000";
                            su.MemberNumber = mn.MemberNumber;
                            su.ReturnSingleUseCode = _r.GetSingleUse(t, su);

                            if (su.ReturnSingleUseCode.Success == "true")
                            {
                                string createUPC = su.ReturnSingleUseCode.Data.SingleUseCode.ToString();

                                //createUPC = CalculateChecksumDigit(createUPC);
                                createUPC = "00072795687";

                                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                                Image img = b.Encode(BarcodeLib.TYPE.UPCA, createUPC, Color.Black, Color.White, 290, 120);

                                img.Save("wwwroot/images/barcode_" + mn.MemberNumber + ".jpg");

                                var barcodeurl = "http://barcode.localhost/images/barcode_" + mn.MemberNumber + ".jpg";

                                SmtpClient smtp = new SmtpClient();
                                smtp.UseDefaultCredentials = true;
                                smtp.Port = 25;
                                smtp.Host = "msmr1.datotel.com";

                                MailAddress from = new MailAddress("customerservice@waterway.com", "Waterway Gas & Wash Company");
                                MailAddress to = new MailAddress(mr.email);
                                MailMessage mail = new MailMessage(from, to);
                                mail.IsBodyHtml = true;

                                var builder = new StringBuilder();

                                using (var reader = File.OpenText("wwwroot/template/EmailText.htm"))
                                {
                                    builder.Append(reader.ReadToEnd());
                                }

                                builder.Replace("{{barcode-image}}", barcodeurl);

                                mail.Body = builder.ToString();

                                mail.Subject = "Your Waterway Coupon Has Arrived!";

                                smtp.Send(mail);

                                return ("success", barcodeurl);
                            }
                        }
                    }
                }

                return ("failure","");

            }
            catch (Exception e)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                return ("failure","");
            }

        }

        static string CalculateChecksumDigit(string UPC)
        {
            string sTemp = UPC;
            int iSum = 0;
            int iDigit = 0;

            // Calculate the checksum digit
            for (int i = 1; i <= sTemp.Length; i++)
            {
                iDigit = Convert.ToInt32(sTemp.Substring(i - 1, 1));
                if (i % 2 == 0)
                {
                    iSum += iDigit * 1;
                }
                else
                {
                    iSum += iDigit * 3;
                }
            }

            int iCheckSum = (10 - (iSum % 10)) % 10;
            UPC = UPC + iCheckSum.ToString();

            return UPC;
        }

    }
}
