using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Utility.Email
{
    public class MailGenerator : IMailGenerator
    {
        private readonly ILogger _logger;
        private readonly MailConfiguration _appConfig;

        public MailGenerator(
            ILogger<MailGenerator> logger,
            IOptions<MailConfiguration> appConfig)
        {
            _logger = logger;
            _appConfig = appConfig.Value;
        }

        
        public void NewRegistrationViaEMail(string fullName, string verificationUrl, out string mailContent, out string mailSubject)
        {
            mailSubject = "Verify Email Address";
            mailContent = $"Hello {fullName}, \n We at Praveen Fleets are excited to welcome you and your organization and provide you with great services. Below is the verification Url please verify your email, to start using our services. \n {verificationUrl} \n Thanks Regards, \n Praveen Fleets.";
            
        }
    }

}
