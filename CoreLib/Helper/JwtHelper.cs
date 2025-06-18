using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoreLib.Helper
{
    public static class JwtHelper
    {
        public static ClaimsPrincipal? GetPrincipalFromToken(string token, string? secretKey)
        {
            Console.WriteLine("🔐 [JwtHelper] Đang giải mã token...");
            Console.WriteLine($"🔑 Token: {token}");
            Console.WriteLine($"🔑 SecretKey: {secretKey}");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(secretKey))
            {
                Console.WriteLine("❌ Token hoặc SecretKey bị null hoặc rỗng.");
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                Console.WriteLine("✅ Token giải mã thành công.");

                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"➡ Claim: {claim.Type} = {claim.Value}");
                }

                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi giải mã token:");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
