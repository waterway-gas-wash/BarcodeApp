using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using BarcodeApp.Services;
using BarcodeApp.Models;
using log4net;
using System.Net.Mail;
using System.Drawing;
using System.Net.Mime;
using System.Reflection;
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BarcodeApp
{
    public class EmailBarcode
    {

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       


        public static string BarcodeEmail(string first, string last, string email)
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

                if (emailAddress.foundEmail.Success == "false" || emailAddress.foundEmail.Id == "blauer@waterway.com")
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
                            string createUPC = "0727" + newMember.memberCreated.Id + "0";
                            createUPC = CalculateChecksumDigit(createUPC);

                            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                            Image img = b.Encode(BarcodeLib.TYPE.UPCA, createUPC, Color.Black, Color.White, 290, 120);

                            img.Save("C:/inetpub/wwwroot/barcode/wwwroot/images/barcode_" + mn.MemberNumber + ".jpg");

                            var barcodeurl = "http://localhost/images/barcode_" + mn.MemberNumber + ".jpg";

                            //LinkedResource LinkedImage = new LinkedResource("barcode.jpeg");
                            //LinkedImage.ContentId = "BC";
                            //LinkedImage.ContentType = new ContentType(MediaTypeNames.Image.Jpeg);

                            //Setup Default sender befault sending the email.
                            SmtpClient smtpClient = new SmtpClient
                            {
                                Host = "msmr1.datotel.com",
                                Port = 25,
                                UseDefaultCredentials = true
                            };
                            Email.DefaultSender = new SmtpSender(smtpClient);
                            Email.DefaultRenderer = new RazorRenderer();

                          

                            var mail = Email
                              .From("customerservice@waterway.com", "Waterway Gas & Wash Company")

                              .To(mr.email)
                              .Subject("Test")
                            
                             .UsingTemplateFromFile("C:/inetpub/wwwroot/barcode/wwwroot/email.htm", new {UPC = createUPC, FILE = barcodeurl});
                           
                            mail.Send();

                            return "success";
                        }
                    }
                }


                return "failure";
            }
            catch (Exception e)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                return "failure";
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
