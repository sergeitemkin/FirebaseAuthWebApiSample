using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace WebApi
{
    public class FirebaseTokenValidationParameters : TokenValidationParameters
    {
        // General info on Jwt validation in .net core: https://stormpath.com/blog/token-authentication-asp-net-core
        // Firebase specific setup: Verify ID tokens using a third-party Jwt library section in https://firebase.google.com/docs/auth/admin/verify-id-tokens
        //
        // Usefull place to look at the contents of your Jwt token https://jwt.io
        public FirebaseTokenValidationParameters(string projectId, IEnumerable<X509SecurityKey> issuerSigningKeys)
        {
            // Signing key must match one of the keys from the url above
            ValidateIssuerSigningKey = true;
            IssuerSigningKeys = issuerSigningKeys;
        
            // Validate the JWT Issuer (iss) claim - check https://firebase.google.com/docs/auth/admin/verify-id-tokens for more info
            ValidateIssuer = true;
            ValidIssuer = $"https://securetoken.google.com/{projectId}";
        
            // Validate the JWT Audience (aud) claim - check https://firebase.google.com/docs/auth/admin/verify-id-tokens for more info
            ValidateAudience = true;
            ValidAudience = projectId;
        
            // Validate the token expiry
            ValidateLifetime = true;
        
            // If you want to allow a certain amount of clock drift, set that here:
            ClockSkew = TimeSpan.Zero;
        }
    }
}