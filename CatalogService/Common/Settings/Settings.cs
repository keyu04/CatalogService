namespace CatalogService.Common.Settings;

public static class Settings
{
    public static string? JwtSecreteKey { get; set; }
    public static string? JwtIssuer { get; set; }
    public static string? JwtAudience { get; set; }
}