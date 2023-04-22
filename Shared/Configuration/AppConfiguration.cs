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

    public string JwtSecurityKey { get; set; }

    public string JwtIssuer { get; set; } 

    public string JwtAudience { get; set; }

    public int JwtExpiryInHours { get; set; } = 6;

}

