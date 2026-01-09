namespace MyApp.Services;

/// <summary>
/// Service interface for Windows Certificate Enrollment using CertEnroll COM API.
/// This demonstrates usage of the Interop.CERTENROLLLib package.
/// </summary>
public interface ICertEnrollService
{
    /// <summary>
    /// Creates a PKCS#10 certificate signing request using CertEnroll API.
    /// </summary>
    string CreateCertificateRequest(CertEnrollRequestOptions options);

    /// <summary>
    /// Installs a certificate response from a CA.
    /// </summary>
    bool InstallCertificate(string certificateResponse, string password);

    /// <summary>
    /// Gets available certificate templates from the enrollment policy.
    /// </summary>
    IEnumerable<string> GetAvailableTemplates();

    /// <summary>
    /// Creates a self-signed certificate using CertEnroll API.
    /// </summary>
    string CreateSelfSignedCertificate(string subjectName, int validityDays);
}

/// <summary>
/// Options for creating a certificate enrollment request.
/// </summary>
public class CertEnrollRequestOptions
{
    /// <summary>
    /// The X.500 distinguished name for the certificate subject (e.g., "CN=example.com").
    /// </summary>
    public string SubjectName { get; set; } = string.Empty;

    /// <summary>
    /// The certificate template name to use (requires AD CS infrastructure).
    /// </summary>
    public string? TemplateName { get; set; }

    /// <summary>
    /// The RSA key length in bits. Default is 2048.
    /// </summary>
    public int KeyLength { get; set; } = 2048;

    /// <summary>
    /// The key usage extension value. Default is "DigitalSignature".
    /// </summary>
    public string KeyUsage { get; set; } = "DigitalSignature";

    /// <summary>
    /// Whether the private key should be exportable. Default is false.
    /// </summary>
    public bool Exportable { get; set; }

    /// <summary>
    /// A friendly name for the certificate in the certificate store.
    /// </summary>
    public string? FriendlyName { get; set; }

    /// <summary>
    /// Subject Alternative Names (SANs) to include in the certificate.
    /// </summary>
    public string[]? SubjectAlternativeNames { get; set; }
}
