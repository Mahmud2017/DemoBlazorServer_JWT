﻿using DemoBlazorServerWithJWTAuth.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DemoBlazorServerWithJWTAuth.States
{
    public static class DecryptJwtService
    {
        public static CustomUserClaims DecryptToken(string jwtToken)
        {
            try
            {
                if (jwtToken == null)
                    return new CustomUserClaims();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                var name = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                var email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var role = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                return new CustomUserClaims(name!.Value, email!.Value, role!.Value);
            }
            catch 
            {
                return null!;
            }
        }
    }
}
