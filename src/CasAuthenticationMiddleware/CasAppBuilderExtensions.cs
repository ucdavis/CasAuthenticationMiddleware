using System;
using Microsoft.AspNetCore.Builder;

namespace CasAuthenticationMiddleware
{
    public static class CasAppBuilderExtensions
    {
        public static IApplicationBuilder UseCasAuthentication(this IApplicationBuilder app, CasAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CasAuthenticationMiddleware<CasAuthenticationOptions>>(options);
        }
    }
}