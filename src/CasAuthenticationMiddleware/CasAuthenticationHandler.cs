using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features.Authentication;

namespace CasAuthenticationMiddleware
{
    public class CasAuthenticationHandler<TOptions> : RemoteAuthenticationHandler<TOptions> where TOptions : CasAuthenticationOptions
    {
        private const string StrTicket = "ticket";
        private const string StrReturnUrl = "ReturnURL";

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
            string service = Context.Request.Path.ToUriComponent() + query;

            if (string.IsNullOrWhiteSpace(ticket))
            {
                return await Task.FromResult(AuthenticateResult.Failed("No authorization ticket found"));
            }
            else
            {
                //todo: backchannel call
                var identity = new ClaimsIdentity(Options.ClaimsIssuer);
                identity.AddClaim(new Claim(ClaimTypes.Name, "postit", ClaimValueTypes.String, Options.ClaimsIssuer));
                var principal = new ClaimsPrincipal(identity);

                var authTicket = new AuthenticationTicket(principal,
                    new AuthenticationProperties {RedirectUri = returnUrl}, "CAS");

                return await Task.FromResult(AuthenticateResult.Success(authTicket));
            }
        }

        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var authorizationEndpoint = Options.AuthorizationEndpoint + "login?service=" + BuildRedirectUri(Options.CallbackPath) + "?" + StrReturnUrl + "=" + Context.Request.Path;

            Context.Response.Redirect(authorizationEndpoint);

            return await Task.FromResult(true);
        }
    }
}