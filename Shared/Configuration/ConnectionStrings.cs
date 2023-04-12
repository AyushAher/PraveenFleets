namespace Shared.Configuration;

/// <summary>
/// POCO Class for ConnectionStrings Section in appSettings.json
/// </summary>
public class ConnectionStrings
{
    public static string SectionLabel { get; } = "ConnectionStrings";
    public string RedisCachingConnection { get; set; }

    public ConnectionStrings()
    {
        RedisCachingConnection = string.Empty;
    }

}
