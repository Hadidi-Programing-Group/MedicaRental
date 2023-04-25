using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

//public record LoginStatusWithTokenDto(string StatusMessage, HttpStatusCode StatusCode, bool isAuthenticated, string? Token, DateTime? Expiry) : StatusDto (StatusMessage, StatusCode);



public class LoginStatusWithTokenDto
{
    public string? StatusMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public string? UserRole { get; set; }  

    [JsonIgnore]
    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiration { get; set; }

    public LoginStatusWithTokenDto(string statusMessage, HttpStatusCode statusCode, bool isAuthenticated, string? token, DateTime? expiry, string? refreshToken, DateTime refreshTokenExpiration, string? userRole)
    {
        StatusMessage = statusMessage;
        StatusCode = statusCode;
        IsAuthenticated = isAuthenticated;
        Token = token;
        Expiry = expiry;
        RefreshToken = refreshToken;
        RefreshTokenExpiration = refreshTokenExpiration;
        UserRole = userRole;
    }
}
