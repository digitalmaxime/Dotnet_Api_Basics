namespace JwtAuthenticationIdentity.Options;

public class JwtOptions
{
    public required string[] ValidAudiences { get; set; }
    public required string[] ValidIssuers { get; set; }
    public required string SigningKey { get; set; }
    public required int ExpirationInMinutes { get; set; }
    
}