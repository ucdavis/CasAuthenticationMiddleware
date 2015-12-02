using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace CasAuthenticationMiddleware
{
    public static class CasAppBuilderExtensions
    {
        public static IApplicationBuilder UseCasAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            //TODO: allow options
            return
                app.UseMiddleware<CasAuthenticationMiddleware>(new CasAuthenticationOptions
                {
                    CallbackPath = new PathString("/Home/CasLogin"),
                    AuthenticationScheme = "Cas",
                    DisplayName = "CAS",
                    ClaimsIssuer = "Cas",
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true
                });
        }
    }
}