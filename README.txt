# CAS Authentication Middleware for ASPNET 5.0

## Integration
_* See the included CasAuthenticationSample project for full details._

We're going to use the Cookie Authentication Middleware to store our claim credentials so first you might need to include the `Microsoft.AspNet.Authentication.Cookies` package.

Now,in your Startup.cs file, configure the cookie auth in the `ConfigureServices()` method.

    services.AddAuthentication(sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

Next, in the `Configure()` method towards the bottom you'll need to configure the cookie authentication and then the actual CAS middleware underneith it.


    app.UseCookieAuthentication(options =>
    {
        options.AutomaticAuthenticate = true;
        options.AutomaticChallenge = true;
        options.LoginPath = new PathString("/home/caslogin");
    });


    app.UseCasAuthentication(new CasAuthenticationOptions
    {
        AuthenticationScheme = "UCDCAS",
        AuthorizationEndpoint = "https://cas.ucdavis.edu/cas/",
        CallbackPath = new PathString("/Home/caslogin"),
        DisplayName = "CAS",
        ClaimsIssuer = "Cas",
        AutomaticAuthenticate = true,
        AutomaticChallenge = true
    });

Make sure the loginPath for cookie authentication and the callback path for CAS Authentication match.

That's it, enjoy!