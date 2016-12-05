# CAS Authentication Middleware for ASPNET 5.0

## Integration
_* See the included CasAuthenticationSample project for full details._

We're going to use the Cookie Authentication Middleware to store our claim credentials so first you might need to include the `Microsoft.AspNetCore.Authentication.Cookies` package.

Now,in your Startup.cs file, configure the cookie auth in the `ConfigureServices()` method.

    services.AddAuthentication(sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

Next, in the `Configure()` method towards the bottom you'll need to configure the cookie authentication and then the actual CAS middleware underneath it.


    app.UseCookieAuthentication(new CookieAuthenticationOptions
    {
        AutomaticAuthenticate = true,
        AutomaticChallenge = true
    });


    app.UseCasAuthentication(new CasAuthenticationOptions
    {
        AuthenticationScheme = "ucdcas",
        AuthorizationEndpoint = "https://cas.ucdavis.edu/cas/",
        CallbackPath = new PathString("/home/caslogin"),
        DisplayName = "UCD CAS"
    });

Finally, you can protect your resources in several different ways using the standard .NET Security methods.

For ASP.NET MVC, you'll probably just want to use `[Authorize(ActiveAuthenticationSchemes = "ucdcas")]`.

If you'd rather perform the authentication challenge in code, you can always do `await context.Authentication.ChallengeAsync("UCDCAS");`.  If you have multiple authentication systems this is a great way to segment user authentication in different parts of your app, or just to let your users decide how they want to log in.

That's it, enjoy!
