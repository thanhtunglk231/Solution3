using CommonLib.Handles;
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
           

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(secretKey))
            {
                
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

              

               

                return principal;
            }
            catch (Exception ex)
            {
            
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}