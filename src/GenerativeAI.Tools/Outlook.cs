using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// Represents an email message
    /// </summary>
    public class EmailMessage
    {
        /// <summary>
        /// Sender's email
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Date when email was sent
        /// </summary>
        public string SentDate { get; set; }

        /// <summary>
        /// List of attachments in the email.
        /// </summary>
        public List<string> Attachments { get; set; } = new List<string>();
    }

    /// <summary>
    /// Utility class for Outlook specific operations.
    /// </summary>
    public static class Outlook
    {
        /// <summary>
        /// Get the outlook instance or start a new one
        /// </summary>
        /// <returns></returns>
        private static Application GetApplicationObject()
        {
            Application application = null;

            // Check whether there is an Outlook process running.
            if (Process.GetProcessesByName("OUTLOOK").Length > 0)
            {
                // If so, use the GetActiveObject method to obtain the process and cast it to an Application object.
                application = Marshal.GetActiveObject("Outlook.Application") as Application;
            }
            else
            {
                // If not, create a new instance of Outlook and sign in to the default profile.
                application = new Application();
                NameSpace nameSpace = application.GetNamespace("MAPI");
                nameSpace.Logon(Missing.Value, Missing.Value, false);
                Marshal.ReleaseComObject(nameSpace);
                nameSpace = null;
            }

            // Return the Outlook Application object.
            return application;
        }

        /// <summary>
        /// Release a pre-allocated object.
        /// </summary>
        /// <param name="outlook"></param>
        /// <param name="outlookNamespace"></param>
        /// <param name="sendAll"></param>
        private static void ReleaseApplicationObject(Application outlook, NameSpace outlookNamespace, bool sendAll = false)
        {
            if (null != outlookNamespace)
            {
                if (sendAll) outlookNamespace.SendAndReceive(false);
                Marshal.ReleaseComObject(outlookNamespace);
            }
            if (null != outlook)
            {
                Marshal.ReleaseComObject(outlook);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Get the default email address
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultEmail()
        {
            string email;

            // Get a handle on Outlook
            Application outlook = GetApplicationObject();

            // Set the correct account
            NameSpace outlookNameSpace = outlook.GetNamespace("mapi");
            if (null == outlookNameSpace.Accounts || outlookNameSpace.Accounts.Count < 1)
            {
                ReleaseApplicationObject(outlook, outlookNameSpace, false);
                outlookNameSpace = null;
                outlook = null;
                throw new System.Exception("Unable to locate a mail profile");
            }

            email = String.Copy(outlookNameSpace.Accounts[1].SmtpAddress);
            ReleaseApplicationObject(outlook, outlookNameSpace, false);
            outlookNameSpace = null;
            outlook = null;

            return email;
        }

        /// <summary>
        /// Reads email from outlook inbox based on given filters and downloads all the attachments to the filtered emails.
        /// </summary>
        /// <param name="mailboxFolder">Mailbox folder path.</param>
        /// <param name="sender">Sender whose email needs to be read. If empty, then email from all senders will be read.</param>
        /// <param name="subjectFilter">Email with specfic subject to be read. If empty, all emails will be read.</param>
        /// <param name="fromDate">A date filter for email indicating all emails from this date to be read. Date needs to be in dd/MM/yyyy format.</param>
        /// <param name="toDate">A date filter for email indicating all emails up to this date to be read. Date needs to be in dd/MM/yyyy format.</param>
        /// <param name="limit">Maximum number of emails to be returned.</param>
        /// <param name="downloadFolder">Full path of a folder where attachments to be downloaded. If it is empty
        /// <returns>Download status</returns>
        public static string DownloadEmailAttachments(string mailboxFolder, string sender, string subjectFilter, string fromDate, string toDate, int limit, string downloadFolder)
        {
            var emails = GetEmailsCore(sender, subjectFilter, mailboxFolder, fromDate, toDate, limit, downloadFolder).ToArray();
            var withattachments = emails.Where(e => e.Attachments != null && e.Attachments.Count > 0).ToArray();
            var total = withattachments.Sum(e => e.Attachments.Count);
            return $"Total emails read: {emails.Length}, Emails with attachments: {withattachments.Length}, Total attachments: {total}.";
        }

        /// <summary>
        /// Gets top emails based on the given email filters on sender, subject or timeframe.
        /// </summary>
        /// <param name="mailboxFolder">Mailbox folder path.</param>
        /// <param name="sender">Sender whose email needs to be read. If empty, then email from all senders will be read.</param>
        /// <param name="subjectFilter">Email with specfic subject to be read. If empty, all emails will be read.</param>
        /// <param name="fromDate">A date filter for email indicating all emails from this date to be read. Date needs to be in dd/MM/yyyy format.</param>
        /// <param name="toDate">A date filter for email indicating all emails up to this date to be read. Date needs to be in dd/MM/yyyy format.</param>
        /// <param name="limit">Maximum number of emails to be returned. This number can't be more than 20.</param>
        /// <returns>List of email messages</returns>
        public static IEnumerable<EmailMessage> GetTopEmails(string mailboxFolder, string sender, string subjectFilter, string fromDate, string toDate, int limit)
        {
            if(limit > 20) limit = 20;
            return GetEmailsCore(sender, subjectFilter, mailboxFolder, fromDate, toDate, limit, string.Empty);
        }

        /// <summary>
        /// Gets/Reads email from the outlook inbox
        /// </summary>
        /// <param name="sender">Sender whose email needs to be read. If empty, then email from all senders will be read.</param>
        /// <param name="subjectFilter">Email with specfic subject to be read. If empty, all emails will be read.</param>
        /// <param name="fromDate">A date filter for email indicating all emails from this date to be read. Date needs to be in dd/MM/yyyy format.</param>
        /// <param name="toDate">A date filter for email indicating all emails up to this date to be read. Date needs to be in dd/MM/yyyy format.</param>
        /// <param name="count">Maximum number of eamils to be returned.</param>
        /// <param name="downloadFolder">Full path of a folder where attachments to be downloaded. If it is empty
        /// attachments will not be downloaded.</param>
        /// <returns>List of email messages</returns>
        private static IEnumerable<EmailMessage> GetEmailsCore(string sender, string subjectFilter, string mailboxFolder, string fromDate, string toDate, int count, string downloadFolder)
        {
            List<EmailMessage> emails = new List<EmailMessage>();
            bool downloadAttachments = !string.IsNullOrWhiteSpace(downloadFolder) && Directory.Exists(downloadFolder);
            try
            {
                var outlookApp = GetApplicationObject();
                var outlookNamespace = outlookApp.GetNamespace("MAPI");

                // Get the Inbox folder
                MAPIFolder mailFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);

                if (!string.IsNullOrEmpty(mailboxFolder))
                {
                    var folder = GetEmailFolder(outlookNamespace, mailboxFolder, null);
                    if(folder != null)
                    {
                        mailFolder = folder;
                    }
                }

                sender = sender.ToLower();
                subjectFilter = subjectFilter.ToLower();

                DateTime date;
                var provider = CultureInfo.InvariantCulture;

                //Add one extra day, so that it doesn't ignore this date based on timestamp
                string filter = string.Empty;
                if (DateTime.TryParseExact(toDate, "dd/MM/yyyy", provider, DateTimeStyles.None, out date))
                {
                    toDate = date.ToShortDateString();
                }
                else
                {
                    toDate = DateTime.Now.AddDays(1).ToShortDateString();
                }

                if (DateTime.TryParseExact(fromDate, "dd/MM/yyyy", provider, DateTimeStyles.None, out date))
                {
                    fromDate = date.ToShortDateString();
                }
                else
                {
                    fromDate = DateTime.Now.AddDays(-7).ToShortDateString();
                }

                if(fromDate == toDate)
                {
                    if (DateTime.TryParseExact(fromDate, "dd/MM/yyyy", provider, DateTimeStyles.None, out date))
                    {
                        fromDate = date.AddDays(-1).ToShortDateString();
                    }
                }

                filter = $"[ReceivedTime] >= '{fromDate}' AND [ReceivedTime] <= '{toDate}'";

                // Retrieve the emails in the Inbox folder
                Items mailitems = mailFolder.Items.Restrict(filter);
                mailitems.Sort("[ReceivedTime]", true);
                int totalMails = mailitems.Count;
                int itemCount = 0;


                foreach (object item in mailitems)
                {
                    MailItem mail = item as MailItem;
                    if(mail == null) continue;

                    if (itemCount >= count) break;

                    var name = mail.SenderName.ToLower();
                    var addressEntry = mail.Sender;
                    var emailaddress = mail.SenderEmailAddress.ToLower();
                    if (addressEntry.Type == "SMTP")
                    {
                        emailaddress = addressEntry.Address;
                    }
                    else if(addressEntry.Type == "EX")
                    {
                        emailaddress = addressEntry.GetExchangeUser().PrimarySmtpAddress;
                    }
                    
                    var addressType = mail.SenderEmailType;
                    

                    if (!string.IsNullOrWhiteSpace(sender) && (!name.Contains(sender) || !emailaddress.Contains(sender))) continue;

                    var subject = mail.Subject.ToLower();
                    if (!string.IsNullOrEmpty(subjectFilter) && !subject.Contains(subjectFilter)) continue;

                    //Get a clean email message
                    var body = CleanupEmailBody(mail.Body);

                    var email = new EmailMessage() { From = emailaddress, Subject = mail.Subject, Message = body, SentDate = mail.SentOn.ToShortDateString() };
                    email.Attachments = new List<string>();
                    if (mail.Attachments.Count > 0)
                    {
                        foreach (Attachment attachment in mail.Attachments)
                        {
                            email.Attachments.Add(attachment.FileName);
                            if (downloadAttachments)
                            {
                                attachment.SaveAsFile(Path.Combine(downloadFolder, attachment.FileName));
                            }
                        }
                    }
                    emails.Add(email);
                    itemCount++;
                }

                ReleaseApplicationObject(outlookApp, outlookNamespace);
            }
            catch(System.Exception ex) 
            {
                throw new System.Exception(ex.Message, ex);
            }
            
            return emails;
        }

        /// <summary>
        /// Sends email to given recipients in toEmails with a given subject and message along with the attachments.
        /// </summary>
        /// <param name="toEmails">A comma separated list of email addresses.</param>
        /// <param name="subject">Email subject</param>
        /// <param name="message">Email message</param>
        /// <param name="attachments">A semicolon separated list of full path of the files to be sent as attachments.</param>
        /// <returns>True if successful</returns>
        public static bool SendEmail(string toEmails, string subject, string message, string attachments)
        {
            try
            {
                var outlookApp = GetApplicationObject();
                var outlookNamespace = outlookApp.GetNamespace("MAPI");

                MailItem newMail = outlookApp.CreateItem(OlItemType.olMailItem);

                // Set the To and Cc addresses
                Recipients recipients = newMail.Recipients;
                if (toEmails == null) toEmails = "";
                foreach (string s in toEmails.Split(new Char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Recipient r = recipients.Add(s.Trim());
                    r.Type = (int)OlMailRecipientType.olTo;
                    r.Resolve();
                }
                // Release this handle.
                recipients = null;

                newMail.Subject = subject;
                newMail.Body = message;

                if (!string.IsNullOrWhiteSpace(attachments))
                {
                    var files = attachments.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string file in files)
                    {
                        string trimmedFile = file.Trim();
                        if (File.Exists(trimmedFile))
                        {
                            newMail.Attachments.Add(trimmedFile, OlAttachmentType.olByValue, 0, Path.GetFileName(trimmedFile));
                        }
                    }
                }

                newMail.Send();
                ReleaseApplicationObject(outlookApp, outlookNamespace);
                return true;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message, ex);
            }
        }

        private static string CleanupEmailBody(string body)
        {
            Regex regex = new Regex(@"From:[\s\w\/\.@<>\r]+\nSent:[\s\w\,\:\r]+\nTo:[\s\w\/\.@<>\r]+");
            var match = regex.Match(body);
            var msg = regex.Split(body).First().Trim();

            //Remove http links from the message
            var links = new Regex(@"<(ftp:\/\/|www\.|https?:\/\/){1}[a-zA-Z0-9u00a1-\uffff0-]{2,}\.[a-zA-Z0-9u00a1-\uffff0-]{2,}(\S*)>");
            msg = links.Replace(msg, string.Empty);

            //Remove large words from the message, most likely some garbage
            var largeword = new Regex(@"[\w\-_\.]{100,}");
            msg = largeword.Replace(msg, string.Empty);

            return msg;
        }

        private static MAPIFolder GetEmailFolder(NameSpace outlookNameSpace, string mailboxFolder, MAPIFolder parent)
        {
            Folders folders = outlookNameSpace.Folders;
            
            if(parent != null) { folders = parent.Folders; }

            foreach (MAPIFolder folder in folders)
            {
                if (folder.FolderPath == mailboxFolder || folder.FullFolderPath == mailboxFolder || folder.FolderPath.EndsWith($@"\{mailboxFolder}")) return folder;

                var f = GetEmailFolder(outlookNameSpace, mailboxFolder, folder);
                if(f != null) return f;
            }

            return null;
        }
    }
}
