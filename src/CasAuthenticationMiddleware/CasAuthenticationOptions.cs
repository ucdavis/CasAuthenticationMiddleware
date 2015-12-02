using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Authentication;

namespace CasAuthenticationMiddleware
{
    public class CasAuthenticationOptions : RemoteAuthenticationOptions
    {
        public CasAuthenticationOptions()
        {
            AuthenticationScheme = "UCDCAS";
            DisplayName = AuthenticationScheme;
            AuthorizationEndpoint = "https://cas.ucdavis.edu/cas/";
        }

        public string AuthorizationEndpoint { get; set; }
    }
}
