using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAssitantAPI.Authentication
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock) { }


        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string apiValue;

            if (Request.Headers.Count <= 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("No header information!"));
            }
            else
            {
                apiValue = Request.Headers.FirstOrDefault(x => x.Key == "ApiKey").Value.ToString();

                if (apiValue == Environment.GetEnvironmentVariable("API_KEY"))
                {

                    var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "travelassistantApi") };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
                else {
                    return Task.FromResult(AuthenticateResult.Fail("api key not foun to compare!"));
                }

            }            

        }
       
    }

}
