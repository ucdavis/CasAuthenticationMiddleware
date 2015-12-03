using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Authentication;

namespace CasAuthenticationMiddleware
{
    public class CasAuthenticationOptions : RemoteAuthenticationOptions
    {
        public string AuthorizationEndpoint { get; set; }
    }
}
