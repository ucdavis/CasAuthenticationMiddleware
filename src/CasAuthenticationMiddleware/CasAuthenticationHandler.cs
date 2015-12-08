using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features.Authentication;

namespace CasAuthenticationMiddleware
{
    public class CasAuthenticationHandler<TOptions> : RemoteAuthenticationHandler<TOptions> where TOptions : CasAuthenticationOptions
    {
        private const string StrTicket = "ticket";
        private const string StrReturnUrl = "ReturnURL";

        public CasAuthenticationHandler(HttpClient backchannel)
        {
            Backchannel = backchannel;
        }

        protected HttpClient Backchannel { get; private set; }

        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            // build query string but strip out ticket if it is defined
            var query = Context.Request.Query.Keys.Where(
                key => string.Compare(key, StrTicket, StringComparison.OrdinalIgnoreCase) != 0)
                .Aggregate("", (current, key) => current + ("&" + key + "=" + Context.Request.Query[key]));

            // replace 1st character with ? if query is not empty
            if (!string.IsNullOrEmpty(query))
            {
                query = "?" + query.Substring(1);
            }

            // get ticket & service
            string ticket = Context.Request.Query[StrTicket];
            var returnUrl = Context.Request.Query[StrReturnUrl];
            
            string service = BuildRedirectUri(Context.Request.Path) + query;

            if (string.IsNullOrWhiteSpace(ticket))
            {
                return AuthenticateResult.Failed("No authorization ticket found");
            }

            var responseStream =
                await
                    Backchannel.GetStreamAsync(Options.AuthorizationEndpoint + "validate?ticket=" + ticket +
                                               "&service=" + service);

            using (var sr = new StreamReader(responseStream))
            {
                // parse text file
                if (sr.ReadLine() == "yes")
                {
                    // get kerberos id
                    string kerberos = sr.ReadLine();

                    var identity = new ClaimsIdentity(Options.ClaimsIssuer);
                    identity.AddClaim(new Claim(ClaimTypes.Name, kerberos, ClaimValueTypes.String,
                        Options.ClaimsIssuer));
                    var principal = new ClaimsPrincipal(identity);

                    var authTicket = new AuthenticationTicket(principal,
                        new AuthenticationProperties {RedirectUri = returnUrl}, Options.AuthenticationScheme);

                    return AuthenticateResult.Success(authTicket);
                }
                else
                {
                    return AuthenticateResult.Failed("Invalid ticket");
                }
            }
        }

        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var authorizationEndpoint = Options.AuthorizationEndpoint + "login?service=" + BuildRedirectUri(Options.CallbackPath) + "?" + StrReturnUrl + "=" + new PathString(Context.Request.Path + Context.Request.QueryString);

            Context.Response.Redirect(authorizationEndpoint);

            return await Task.FromResult(true);
        }
    }
}