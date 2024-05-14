# Cookies

__Cookie__ is an Authentication scheme, like JWT. 

```csharp
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie()
```

```csharp
app.UseRouting();

app.UseAuthentication(); <---

app.UseAuthorization();

app.MapControllers();

```

## Table of Content
1) [User Claims](#user-claims)
   1) [User Claims](#user-claims)
   2) [User Claims Code Example](#user-claims-code-example)
2) [Scheme Actions](#scheme-actions)
   1) [Authenticate](#authenticate)
   2) [Challenge](#challenge)
   3) [Forbid](#forbid)
3) [Cross-Site Request Forgery](#cross-site-request-forgery)
   1) [Cross-Site Request Forgery](#cross-site-request-forgery)
   2) [Same Site Cookie Options](#same-site-cookie-options)
4) [Base Config](#base-config)
5) [External Identity Providers](#external-identity-providers)
6) [Authorize Controller](#authorize-controller)
7) [Claims Transformation](#claims-transformation)
9) [References](#references)

## User Claims

In AspnetCore, an object that represents the user is called a `ClaimsPrincipal`.

`ClaimsPrincipal`

```
ClaimsPrincipal
+-- ClaimsIdentity (for a given scheme)
|   +-- Claim
|   |   +-- Key : Value
|   |   +-- Role : Admin
|   +-- Claim
+-- ClaimsIdentity (for another given scheme)
|   +-- Claim
|   +-- Claim
```

### User Claims Code Example

```csharp
   public async Task<IActionResult> Login(LoginModel model)
    {
        var user = _userRepository.GetByUsernameAndPassword(model.Username, model.Password);

        ---> CLAIMS <---
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FavoriteColor", user.FavoriteColor)
        };

        ---> IDENTITY <---
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        ---> PRINCIPAL <---
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = model.RememberLogin });
            
        ...
```

## Scheme Actions

### Authenticate

How the claim's principal get reconstructed on each request

(Identity Cookie)

### Challenge

Determines what happens when a user tries to access a resource for which authentication is required

Cookie scheme will redirect to `/login` by default.

To use GoogleAuth, this needs to change, see [base config](#base-config)

### Forbid

Determines what happens when a user tries to access a resource without sufficient rights.

By default, the CookieScheme will redirect to `/Account/AccessDenied` by default. 

## Cross-Site Request Forgery

I'm casually browsing the web and I have a logged session in `https://globomantics.com/`.

I suddenly come across an incredible offer saying I won money in another tab.

```html
// Malicious site

<h1>You Have Won 1000000$ !!!</h1>

<form action="https://globomantics.com/approve" method="post">
    <input type="hidden" name="super-vilan" value="1234">
    <input type="submit" value="Get your free ticket!">
</form>
```

Since my browser hypothetically already has a `globomantics.com` cookie stored,

`//Malicious site` will send a request to `globomantics.com` with my cookie.

So any actions I could do on `globomantics.com` (aka my claims and identity)

will be available to `//Malicious site` since a request made from my browser to `globomantics.com` 

and will thereby use my browser's cookies. 


### Same Site Cookie Options

- strict
  - a cookie will never be sent cross-site
- Lax (default)
  - introduces 1 exception to the strict rule : 
    - only hyperlinks present in `globomantics.com` will work 
- None
  - disables same site cookie

## Base Config

```

builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(o => o.Cookie.SameSite = SameSiteMode.Strict)
    .AddGoogle(o =>
    {
        o.ClientId = "asdf";
        o.ClientSecret = "123";
    });
```

## External Identity Providers

We need to register the application (`Globomantics`) at google to get clientId, clientSecret of the app.

When a user tries to authenticate himself, `Globomantics` redirects the user to Google Identity Provider 

(if the Scheme's action was configured for it)

passing in the app's clientId clientSecret in order for Google to know which application is making a request.

Google authenticates the client using OpenIDC and redirects him to the `redirect url` passed by `Globomantics`
.

It store the user's claims in `Globomantics` and sets an identity cookie in the browser.


```
User --> Globomantics 
Globomantics --> redirects to GoogleAuth, sending (app's clientId, app's clientSecret)
Google authenticates the user
Google sends identityCookie to the user's browser
Google sends userClaims to the application
```

## Authorize Controller

```csharp
[Authorize]
```

Can be used above the Controller class or on individual controller action

If the user tries to access a resource for which he is not authorized, 

the default `cookie-scheme` will try to redirect the user to `/account/login`

If that endpoint doesn't exist, a `404` will be thrown.

```csharp
[AllowAnonymous]
```

## Claims Transformation

Using External provider identity to get claims, but customizing it more through the application's specific claims

## References

[Pluralsight's Roland Guijt's Github](https://github.com/RolandGuijt/ps-aspnetcoreauth/tree/master)
                                                      
