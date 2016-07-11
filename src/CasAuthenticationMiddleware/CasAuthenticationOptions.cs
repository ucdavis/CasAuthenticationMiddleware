using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;

namespace CasAuthenticationMiddleware
{
    public class CasAuthenticationOptions : RemoteAuthenticationOptions
    {
        public string AuthorizationEndpoint { get; set; }
    }
}
