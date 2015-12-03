using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features.Authentication;

namespace CasAuthenticationMiddleware
{
    public class CasAuthenticationHandler<TOptions> : RemoteAuthenticationHandler<TOptions> where TOptions : CasAuthenticationOptions
    {
        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            var query = Context.Request.Query;
            var identity = new ClaimsIdentity(Options.ClaimsIssuer);
            identity.AddClaim(new Claim("name", "postit", ClaimValueTypes.String, Options.ClaimsIssuer));
            var principal = new ClaimsPrincipal(identity);

            var authTicket = new AuthenticationTicket(principal, new AuthenticationProperties(), "CAS");

            var uri = CurrentUri;
            return await Task.FromResult(AuthenticateResult.Success(authTicket));
        }

        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var authorizationEndpoint = Options.AuthorizationEndpoint + "login?service=" + BuildRedirectUri(Options.CallbackPath);

            Context.Response.Redirect(authorizationEndpoint);

            return await Task.FromResult(true);
        }
    }
}