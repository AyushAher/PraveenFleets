namespace Shared.Configuration;

public class AWSS3Config
{
    public static string Label = "AWSS3Settings";

    public string PublicBucket { get; set; } = string.Empty;

    public string PrivateBucket { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string AWSAccessKey { get; set; } = string.Empty;

    public string AWSSecretAccessKey { get; set; } = string.Empty;

    public double AWSSessionTimeOut { get; set; } = 2;

}