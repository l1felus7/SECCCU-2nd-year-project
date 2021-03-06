﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SECCCU
{
    public class Report
    {
        public double GetAttendancePercentage()
        {
            List<string> list = new List<string>();
            using (var reader = new StreamReader(@"csvFiles\\report.csv"))
            {

                bool firstLine = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    list.Add(values[6]);
                }
            }
            int numberOfResults = list.Count();
            int numberOfAttendance = list.Count(str => str.Contains("Yes"));
            double percentage = numberOfAttendance / numberOfResults * 100;
            percentage = Math.Round(percentage, 2);
            return percentage;
        }

        public void SendSignInText(string phoneNumber, string name)
        {
            const string accountSid = "AC6fb9f22e315ca4639e23455b7a981c44";
            const string authToken = "268b2aceca9c6812208d9e2e45768335";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: $"Congratulations {name}, you successfully signed in at {DateTime.Now.ToShortTimeString()} on {DateTime.Now.ToShortDateString()}.",
                from: new Twilio.Types.PhoneNumber("+447480535458"),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );

            Console.WriteLine(message.Sid);
        }

        public bool SendReport(string email)
        {
                try
                { 
                    //Mail Message
                    MailMessage mM = new MailMessage();
                    //Mail Address
                    mM.From = new MailAddress("evlhax@hotmail.com");
                    //receiver email id
                    mM.To.Add($"{email}");
                    //subject of the email
                    mM.Subject = "Attendance Report";
                    //deciding for the attachment
                    mM.Attachments.Add(new Attachment(@"csvFiles\\report.csv"));
                    //add the body of the email
                    mM.Body = "Please see attached report";
                    mM.IsBodyHtml = true;
                    //SMTP client
                    SmtpClient sC = new SmtpClient("smtp.live.com");
                    //port number for Hot mail
                    sC.Port = 25;
                    //credentials to login in to hotmail account
                    sC.Credentials = new NetworkCredential("evlhax@hotmail.com","Parsewerd1!");
                    //enabled SSL
                    sC.EnableSsl = true;
                    //Send an email
                    sC.Send(mM);
                    foreach (Attachment attachment in mM.Attachments)
                    {
                        attachment.Dispose();
                    }
                    return true;
                }//end of try block
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }//end of catch

            
        }
    }
}