namespace OptionsPatternComplexeClass.Options;

public class HttpClientSecurityOption
{
    public const string Name = "HttpClientSecurity";
    public ClientSecurity? Client1 { get; set; }
    public ClientSecurity? Client2 { get; set; }
    public ClientSecurity? Client3 { get; set; }

    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;

    public string Uri { get; set; } = null!;
    public string AppRefCode { get; set; } = null!;
    public string GrantType { get; set; } = null!;

    public class ClientSecurity
    {
        public string? UserName { get; set; } = null!;

        public string Scope { get; set; } = null!;

        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}