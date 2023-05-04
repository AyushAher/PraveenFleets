namespace Utility.Email
{
    public class MailGenerator : IMailGenerator
    {

        private const string mailFooter = "Thanks Regards,<br/>" +
                                          "DigiCab.";

        public void NewRegistrationViaEMail(string fullName, string verificationUrl, out string mailContent,
            out string mailSubject)
        {
            mailSubject = "Verify Email Address";
            mailContent = $"Hello {fullName},<br/><br/> " +
                          "We at DigiCab are excited to welcome you and your organization and provide you with great services. Below is the verification Url please verify your email, to start using our services. <br/> " +
                          $"{verificationUrl} <br/> <br/>" + mailFooter;
        }

        public void NewOrganizationRegistration(string organizationName, out string mailContent, out string mailSubject)
        {
            mailSubject = "Verify Email Address";
            mailContent = $"Hello {organizationName},<br/><br/> " +
                          "We at DigiCab are excited to welcome you and your organization and provide you with great services.<br/> " 
                          + mailFooter;
        }
    }

}
