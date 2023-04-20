namespace Utility.Email;

public interface IMailGenerator
{
    void NewRegistrationViaEMail(string fullName, string verificationUrl, out string mailContent,
        out string mailSubject);
}
