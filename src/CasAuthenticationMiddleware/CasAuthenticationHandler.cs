using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features.Authentication;

namespace CasAuthenticationMiddleware
{
    internal class CasAuthenticationHandler : RemoteAuthenticationHandler<CasAuthenticationOptions>
    {
        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            await Task.Delay(1);
            var identity = new ClaimsIdentity(Options.ClaimsIssuer);
            identity.AddClaim(new Claim("name", "postit", ClaimValueTypes.String, Options.ClaimsIssuer));
            var principal = new ClaimsPrincipal(identity);

            var authTicket = new AuthenticationTicket(principal, new AuthenticationProperties(), "CAS");

            return AuthenticateResult.Success(authTicket);
            //var query = Request.Query;
            //// get ticket & service
            //var ticket = query["ticket"];

            //if (string.IsNullOrEmpty(ticket))
            //{
            //    return AuthenticateResult.Failed("cas ticket not found");
            //}

            ////validate against CAS          
            //await Task.Delay(0); //backchannel CAS check here

            //var valid = true;

            //if (valid)
            //{
            //    var identity = new ClaimsIdentity(Options.ClaimsIssuer);
            //    identity.AddClaim(new Claim("name", "postit", ClaimValueTypes.String, Options.ClaimsIssuer));
            //    var principal = new ClaimsPrincipal(identity);

            //    var authTicket = new AuthenticationTicket(principal, new AuthenticationProperties (), "CAS");

            //    return AuthenticateResult.Success(authTicket);
            //}
            //else
            //{
            //    return AuthenticateResult.Failed("The cas ticket was missing or invalid.");
            //}

        }

        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var authorizationEndpoint = Options.AuthorizationEndpoint + "login?service=" + BuildRedirectUri(Options.CallbackPath);

            Context.Response.Redirect(authorizationEndpoint);

            //TODO: hackery
            await Task.Delay(0);
            return true;
        }
    }
}