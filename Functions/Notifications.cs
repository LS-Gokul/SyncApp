using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;

namespace LSSyncApp.Functions
{
    public class Notifications
    {
        public void SendMail(string asTo, string asSubject, string asTemplate, out int aiSuccess, out string asMessage)
        {
            string lsTo;
            try
            {
                if (asTo.Length <= 0)
                {
                    aiSuccess = 0;
                    asMessage = "Email Id Not Found";
                }
                if (!asTo.Contains(";")) asTo += ";";

                MailMessage newMail = new MailMessage();
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                newMail.From = new MailAddress("noreply@leapsurge.in", "Leapsurge");
                while(asTo.Contains(";"))
                {
                    lsTo = asTo.Substring(0, asTo.IndexOf(";"));
                    asTo = asTo.Substring(asTo.IndexOf(";") + 1);
                    newMail.To.Add(lsTo);
                }
                //newMail.To.Add("gokul@leapsurge.in");
                newMail.Subject = asSubject;
                newMail.IsBodyHtml = true; newMail.Body = asTemplate;
                client.EnableSsl = true;
                client.Port = 587;
                client.Credentials = new System.Net.NetworkCredential("noreply@leapsurge.in", "LSMail@123#");
                client.Send(newMail);

                newMail.Dispose();
                client.Dispose();
                
                aiSuccess = 1;
                asMessage = "Success";
            }
            catch (Exception ex)
            {
                aiSuccess = 0;
                asMessage = ex.Message;
            }
        }

        public string SendMail(string asTo, string asSubject, string asTemplate)
        {
            string asMessage, lsList;
            int i = 0;
            try
            {
                if (asTo.Length <= 0)
                {
                    asMessage = "Email Id Not Found";
                }
                if (!asTo.Contains(";")) asTo += ";";
                lsList = asTo;
                
                while (asTo.Contains(";"))
                {
                    i++;
                    asTo = asTo.Substring(asTo.IndexOf(";") + 1);
                }
                var toRecipients = new List<EmailAddress>();
                for (int rec = 0; rec < i; rec++)
                {
                    toRecipients.Add(new EmailAddress(address: lsList.Substring(0, lsList.IndexOf(";")).ToString()));
                    lsList = lsList.Substring(lsList.IndexOf(";") + 1);
                };
                var connectionString = "";
                var emailClient = new EmailClient(connectionString);

                var sender = "donotreply@leapsurge.in";
                var recipient = asTo;
                var subject = asSubject;
                var emailContent = new EmailContent(subject)
                {
                    Html = asTemplate
                };
                var htmlContent = asTemplate;
                var emailRecipients = new EmailRecipients(toRecipients);
                
                var emailMessage = new EmailMessage(sender, emailRecipients, emailContent)
                {
                    Headers =
                    {
                        { "x-priority", "1" },
                        { "EmailTrackingHeader", "MyCustomEmailTrackingID" }
                    }
                };

                EmailSendOperation emailSendOperation = emailClient.Send(WaitUntil.Completed, emailMessage);
                string operationId = emailSendOperation.Id;
                asMessage = "Success";
            }
            catch (Exception ex)
            {
                asMessage = ex.Message;
            }
            return asMessage;
        }
    }
}