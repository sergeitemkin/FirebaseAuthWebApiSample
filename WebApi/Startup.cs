using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace WebApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddLogging();

            services.AddCors(options => {
                options.AddPolicy("AllowEverythingLol", 
                    builder => builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin() // You'll want to restrict the policy to only your allowed origins (i.e. the address of the site hitting the API)
                        .AllowCredentials()
                    );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            // Cors policy middleware needs to be toward the top also because the request needs to be allowed
            // via the policy before it will be "routed".
            app.UseCors("AllowEverythingLol");

            var issuerKeyProvider = new FirebaseIssuerKeyProvider();
            var signingKeys = issuerKeyProvider.GetSigningKeys().Result;

            // The projectId can be found in Firebase project settings
            var firebaseProjectId = "testing-6bf89";

            // Setup middleware to read the Jwt token from the header, validate it using the params above
            // and set the User principal object with the claims from the Jwt token.
            //
            // This middleware must be added prior to any other middleware that relies on authentication
            // such as the below MVC routing middleware that uses the Authorize attribute.
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {                
                AutomaticAuthenticate = true, 
                AutomaticChallenge = true,
                TokenValidationParameters = new FirebaseTokenValidationParameters(firebaseProjectId, signingKeys)
            });

            // Routing and whatnot
            app.UseMvcWithDefaultRoute();
        }
    }
}