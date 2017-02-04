# Sample Angular 2 SPA using Firebase Authentication with a .Net Core WebApi
There are two projects rolled into the solution here. One is a SPA client that calls an api endpoint, and the other is a WebApi that serves data via the aforementioned endpoint.

##Client (Angular 2 SPA)
Mostly generated via the angular-cli. The actual firebase authentication sample is in `src/app/app.component.ts`. The sample Api call would normally happen via an Angular Service, but I was lazy so it's kinda bundled into the component. This part is pretty straight forward:

1) Create your Firebase config (you really only need the `authDomain` setting and your api key for this)

    var config = {
      apiKey: "AIzaSyD1JWKae5SRLMbVwWdRz2YXu6z-jMzWNJU",
      authDomain: "testing-6bf89.firebaseapp.com"
    };
    
2) If you're allowing 3rd party authentication providers, call `firebase.auth().getRedirectResult()` first to check if this is a returning redirect response from said providers. If there's still no user, then you can redirect to the provider, or display an authentication form, etc.

3) Once you get a user, use the `Dd` property on the object to get the Id Token. This is a token containing various claims for the user, including the user's Firebase uid. Pop the Id Token into http://jwt.io to see the claims if you're curious.
 
Check out [Firebase docs](https://firebase.google.com/docs/auth/web/password-auth) and related links under Authentication for various ways to authenticate.

Things to look out for:
- You'll want to use lock down your Api key to only be used by your web app at: https://console.developers.google.com/apis/credentials?project={YOUR_FIREBASE_PROJECTID}. Set up your key to use Http referrers, and enter your apps url.
- You'll want to store the bearer token you get from `firebase.auth()` in session storage or a javascript cookie (depending on how secure you wanna be). Else the user will be required to log in everytime they refresh the page. You do get some form of single sign-on when you use 3rd party authentication providers like Google, but if you're using email/password, there's no server component to store the cookie to remember who is logged in.
- You'll want to write an interceptor (I think the angular 2 equivalent is to extend Http service) to display the login form if 401s are returned
- You'll want to write something that'll append the Authentication header to all your api requests

##WebApi (.Net Core)
Simple, nearly blank, .Net Core app with one controller to return the user's claims.

1) Setup CORS policy (else Authorize attribute will throw errors)

2) Setup Authenticaton Middleware (else Authorize attribute will throw errors). This middleware will read the Jwt token and stuff the claims into the User claims principal.

3) Add the `[Authorize]` attribute to your controllers or use a filter

Things to look out for:
- Make sure the middleware is added to the pipeline in the right order. Authentication middleware has to go first or the rest of the middleware won't be able to make use of it and `Authorize` attributes on routes will fail.
- Restrict CORS policy to at least the origin of your web app
- Some of the claim names in the Api User object won't match the claim names in the original Jwt. This is because of the silly claim map in `JwtSecurityTokenHandler`. In .Net not core you could clear the claims via `JwtSecurityTokenHandler.InboundClaimTypeMap.Clear();` call in `Startup.cs`, however, I didn't have time to figure out how to do that in Core.
