using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Helpers
{
    public static class HttpRequestExtension
    {
        public static string GetHeader(this HttpRequest request, string key)
        {
            return request.Headers.FirstOrDefault(x => x.Key == key).Value.FirstOrDefault();
        }
    }

    public static class JwtSecurityTokenExtension
    {
        public static string GetTokenItem(this JwtSecurityToken securityToken, string token, string key)
        {
            token = token.Substring(7);
            JwtSecurityToken jwtToken = new JwtSecurityToken(token);
            return jwtToken.Claims.First(claim => claim.Type == key).Value;
        }

        public static int GetUserId(this JwtSecurityToken securityToken, HttpRequest request)
        {
            return int.Parse(GetTokenItem(securityToken, request.GetHeader("Authorization"), "nameid"));
        }
    }
}
