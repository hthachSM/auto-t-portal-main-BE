using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace auto_t_portal_main_BE.Helpers;

public class VnPayHelper(IConfiguration config)
{
    private readonly SortedDictionary<string, string> _data = new();

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            _data[key] = value;
    }

    public string BuildPaymentUrl()
    {
        var baseUrl = config["VnPay:BaseUrl"]!;
        var hashSecret = config["VnPay:HashSecret"]!;

        var query = new StringBuilder();
        foreach (var (key, value) in _data)
        {
            query.Append(WebUtility.UrlEncode(key));
            query.Append('=');
            query.Append(WebUtility.UrlEncode(value));
            query.Append('&');
        }

        var rawData = query.ToString().TrimEnd('&');
        var secureHash = HmacSha512(hashSecret, rawData);

        return $"{baseUrl}?{rawData}&vnp_SecureHash={secureHash}";
    }

    public bool ValidateSignature(IQueryCollection query)
    {
        var hashSecret = config["VnPay:HashSecret"]!;
        var vnpSecureHash = query["vnp_SecureHash"].ToString();

        var data = new SortedDictionary<string, string>();
        foreach (var (key, value) in query)
        {
            if (key.StartsWith("vnp_") && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                data[key] = value.ToString();
        }

        var rawData = string.Join("&", data.Select(kv =>
            $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

        var expectedHash = HmacSha512(hashSecret, rawData);
        return string.Equals(expectedHash, vnpSecureHash, StringComparison.OrdinalIgnoreCase);
    }

    private static string HmacSha512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
