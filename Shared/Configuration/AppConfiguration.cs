namespace Shared.Configuration;

/// <summary>
/// POCO Class for AppConfiguration Section in appSettings.json
/// </summary>
public class AppConfiguration
{
    public static string SectionLabel { get; } = "AppConfiguration";

    public string SiteUrl { get; set; } = string.Empty;
    public string AccConfirmPath { get; set; } = string.Empty;

    public string SupportMailAddress { get; set; } = string.Empty;

    public string JwtSecurityKey { get; set; } = string.Empty;

    public string JwtIssuer { get; set; } = string.Empty;

    public string JwtAudience { get; set; } = string.Empty;

    public int JwtExpiryInHours { get; set; } = 6;

}

