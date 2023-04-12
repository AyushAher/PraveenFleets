namespace Utility.Email;

public interface IMailGenerator
{
    public void DefaultEmailTemplate(
        List<AttributeValue> attributeValue,
        string subjectHeader,
        out string mailContent);
}
