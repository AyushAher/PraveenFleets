using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Utility.Email
{
    public class SMTPMailService : IEMailService
    {
        private MailConfiguration _mailConfig { get; }

        private ILogger<SMTPMailService> _logger { get; }

        public SMTPMailService(ILogger<SMTPMailService> logger,
                               IOptions<MailConfiguration> appConfig)
        {
            _mailConfig = appConfig.Value;
            _logger = logger;
        }

        public bool SendEMail(EMailRequest request)
        {
            try
            {
                _logger.LogDebug("Initialing EMail '" + request.EMailStateID + "'");
                var mailMessage = new MailMessage();
                var eMailId = Guid.NewGuid();
                GenerateEMail(request, eMailId, ref mailMessage);
                var smtpClient = new SmtpClient(_mailConfig.Server, _mailConfig.Port)
                {
                    Credentials = null,
                    DeliveryFormat = SmtpDeliveryFormat.SevenBit,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = false,
                    PickupDirectoryLocation = null,
                    TargetName = null,
                    Timeout = 0,
                    UseDefaultCredentials = false
                };

                smtpClient.EnableSsl = _mailConfig.UseSsl;
                smtpClient.DeliveryFormat = SmtpDeliveryFormat.International;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                if (string.IsNullOrEmpty(_mailConfig.UserName))
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials =
                        new NetworkCredential(_mailConfig.UserName, _mailConfig.Password);
                }

                if (!string.IsNullOrEmpty(_mailConfig.TargetName))
                    smtpClient.TargetName = _mailConfig.TargetName;
                smtpClient.Timeout = 600000;
                smtpClient.Send(mailMessage);
                mailMessage?.Dispose();
                _logger.LogDebug("Sending EMail '" + request.EMailStateID + "' has been Sent!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }

            return true;
        }

        public async Task<bool> SendEMailAsync(EMailRequest request)
        {
            try
            {
                _logger.LogDebug("Initialing EMail '" + request.EMailStateID + "'");
                var mailMessage = new MailMessage();
                var eMailId = Guid.NewGuid();

                GenerateEMail(request, eMailId, ref mailMessage);
                
                var smtpClient = new SmtpClient(_mailConfig.Server, _mailConfig.Port);
                smtpClient.EnableSsl = _mailConfig.UseSsl;

                if (string.IsNullOrEmpty(_mailConfig.UserName))
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_mailConfig.UserName, _mailConfig.Password);
                }

                if (!string.IsNullOrEmpty(_mailConfig.TargetName)) smtpClient.TargetName = _mailConfig.TargetName;
                
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Timeout = 600000;
                smtpClient.SendCompleted += SendCompletedCallback;
                
                smtpClient.SendAsync(mailMessage, request.EMailStateID);
                
                _logger.LogDebug("Sending EMail '" + request.EMailStateID + "' has been attempted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }

            return true;
        }

        private void GenerateEMail(EMailRequest request, Guid eMailId, ref MailMessage mailMessage)
        {
            mailMessage.Headers.Add("Message-ID", eMailId.ToString());

            if (!string.IsNullOrEmpty(_mailConfig.ReplyToAddress))
                mailMessage.Headers.Add("Disposition-Notification-To", _mailConfig.ReplyToAddress);

            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            if (!string.IsNullOrEmpty(_mailConfig.ReplyToAddress))
                mailMessage.ReplyTo = new MailAddress(_mailConfig.ReplyToAddress);

            mailMessage.From = new MailAddress(request.FromAddress?.Address ??
                                               _mailConfig.FromAddress, request.FromAddress?.Name ??
                                                                             _mailConfig.FromName, Encoding.UTF8);

            var mailMessage1 = mailMessage;
            MailPriority mailPriority;
            switch (request.Priority)
            {
                case EMailPriority.Low:
                    mailPriority = MailPriority.Low;
                    break;
                case EMailPriority.Medium:
                    mailPriority = MailPriority.Normal;
                    break;
                case EMailPriority.High:
                    mailPriority = MailPriority.High;
                    break;
                default:
                    mailPriority = MailPriority.Normal;
                    break;
            }

            mailMessage1.Priority = mailPriority;

            if (request.ToAddresses.Count > 0)
            {
                foreach (var toAddress in request.ToAddresses)
                    mailMessage.To.Add(new MailAddress(toAddress.Address, toAddress.Name, Encoding.UTF8));
            }

            if (request.CcAddresses.Count > 0)
            {
                foreach (var ccAddress in request.CcAddresses)
                    mailMessage.CC.Add(new MailAddress(ccAddress.Address, ccAddress.Name, Encoding.UTF8));
            }

            if (request.BccAddresses.Count > 0)
            {
                foreach (var bccAddress in request.BccAddresses)
                    mailMessage.Bcc.Add(new MailAddress(bccAddress.Address, bccAddress.Name, Encoding.UTF8));
            }

            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.Subject = request.Subject;
            if (request.Attachments != null)
            {
                foreach (var attachment in request.Attachments)
                    mailMessage.Attachments.Add(new Attachment(attachment));
            }

            mailMessage.IsBodyHtml = request.IsHtml;
            mailMessage.BodyEncoding = Encoding.UTF8;

            var alternateViewFromString = AlternateView.CreateAlternateViewFromString(request.Body, Encoding.UTF8, "text/html");

            if (!string.IsNullOrEmpty(_mailConfig.Logo) &&
                request.Body.IndexOf("companyLogo.img", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                var linkedResource = new LinkedResource(_mailConfig.Logo);
                linkedResource.ContentId = "companyLogo.img";
                alternateViewFromString.LinkedResources.Add(linkedResource);
            }

            mailMessage.AlternateViews.Add(alternateViewFromString);
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.BodyEncoding = Encoding.UTF8;
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            var userState = (string)e.UserState;
            if (e.Cancelled)
                _logger.LogWarning("[{0}] EMail Send canceled.", userState);
            if (e.Error != null)
                _logger.LogError("[{0}] Send EMail Failed : {1}", userState, e.Error.ToString());
            else
                _logger.LogInformation("[{0}] Message sent.", userState);
        }
    }


    public interface IEMailService
    {
        bool SendEMail(EMailRequest request);

        Task<bool> SendEMailAsync(EMailRequest request);
    }

}
