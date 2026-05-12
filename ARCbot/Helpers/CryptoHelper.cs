using System.Security.Cryptography;
using System.Text;

namespace ARCbot.Helpers;

public static class CryptoHelper
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("ARCbot.SensitiveData.Protection.v1");

    public static string Protect(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var protectedBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedBytes);
    }

    public static string Unprotect(string protectedText)
    {
        if (string.IsNullOrEmpty(protectedText)) return protectedText;

        try
        {
            var protectedBytes = Convert.FromBase64String(protectedText);
            var plainBytes = ProtectedData.Unprotect(protectedBytes, Entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (Exception ex)
        {
            Logger.Warn($"CryptoHelper.Unprotect failed, returning raw value: {ex.Message}");
            return protectedText;
        }
    }

    public static bool IsEncrypted(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        if (text.StartsWith("ENC:")) return true;
        try
        {
            Convert.FromBase64String(text);
            return text.Length >= 24 && !text.Contains(' ') && !text.Contains('\n');
        }
        catch
        {
            return false;
        }
    }
}