namespace GitDeskImport.Services;

using Microsoft.AspNetCore.DataProtection;

public class TokenProtector
{
    private readonly IDataProtector _protector;

    public TokenProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("APITokenProtector");
    }

    public string Protect(string plain) =>
        string.IsNullOrEmpty(plain) ? string.Empty : _protector.Protect(plain);

    public string Unprotect(string encrypted)
    {
        if (string.IsNullOrEmpty(encrypted)) return string.Empty;

        try
        {
            return _protector.Unprotect(encrypted);
        }
        catch
        {
            return string.Empty;
        }
    }
}
