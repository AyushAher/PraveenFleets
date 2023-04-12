namespace Utility.Email
{

    public class AttributeValue
    {
        public string FieldName { get; set; }

        public string FieldValue { get; set; }
    }

    public class EMailRequest
    {
        public EMailAddress? FromAddress { get; set; }

        public List<EMailAddress> ToAddresses { get; set; }

        public List<EMailAddress> CcAddresses { get; set; }

        public List<EMailAddress> BccAddresses { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public bool IsHtml { get; set; } = true;
        public EMailPriority Priority { get; set; } = EMailPriority.Medium;

        public string EMailStateID { get; set; } = string.Empty;

        public List<string> Attachments { get; set; }

        public EMailRequest()
        {
            ToAddresses = new List<EMailAddress>();
            CcAddresses = new List<EMailAddress>();
            BccAddresses = new List<EMailAddress>();
        }
    }

    public class EMailAddress
    {
        private string _name;

        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? Address : _name;
            set => _name = value;
        }

        public string Address { get; set; }

        public EMailAddress(string name, string address)
        {
            _name = name;
            Address = address;
        }
    }

    public enum EMailPriority
    {
        Low,
        Medium,
        High,
    }
}
