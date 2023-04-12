namespace Shared.Configuration;

/// <summary>
/// POCO Class for Email Settings in appSettings.json
/// </summary>
public class MailConfiguration
{
    public static string SectionLabel { get; set; } = "MailConfiguration";

    public string Server { get; set; } = string.Empty;

    public int Port { get; set; } = 25;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool UseSsl { get; set; } = true;

    public string TargetName { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;

    public string FromAddress { get; set; } = string.Empty;

    public string ReplyToAddress { get; set; } = string.Empty;

    public string Logo { get; set; } = string.Empty;
}

