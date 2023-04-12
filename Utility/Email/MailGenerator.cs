using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Utility.Email
{
    // TODO: Add email content
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

        public void DefaultEmailTemplate(
            List<AttributeValue> attributeValue,
            string subjectHeader,
            out string mailContent)
        {
            mailContent = string.Empty;
            try
            {
                _logger.LogInformation("EMail content generation for 'Default Template' successful");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "EMail content generation for 'Default Template' failed");
            }
        }

    }

}
