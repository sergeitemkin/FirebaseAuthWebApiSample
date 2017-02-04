# Sample Angular 2 SPA using Firebase Authentication with a .Net Core WebApi
There are two projects rolled into the solution here. One is a SPA client that calls an api endpoint, and the other is a WebApi that serves data via the aforementioned endpoint.

##Client (Angular 2 SPA)
Mostly generated via the angular-cli. The actual firebase authentication sample is in src/app/app.component.ts. The sample Api call would normally happen via an Angular Service, but I was lazy so it's kinda bundled into the component. This part is pretty straight forward:

Check out [Firebase docs](https://firebase.google.com/docs/auth/web/password-auth) for various ways to authenticate.

Things to look out for:
- You'll want to use lock down your Api key to only be used by your web app at: https://console.developers.google.com/apis/credentials?project={YOUR_FIREBASE_PROJECTID}. Set up your key to use Http referrers, and enter your apps url.
- You'll want to store the bearer token you get from firebase.auth() in session storage or a javascript cookie (depending on how secure you wanna be). Else the user will be required to log in everytime they refresh the page. You do get some form of single sign-on when you use 3rd party authentication providers like Google, but if you're using email/password, there's no server component to store the cookie to remember who is logged in.
- You'll want to write an interceptor for the Http service to require login if 401s are returned
- You'll want to write something that'll append the Authentication header to all your api requests

##WebApi (.Net Core)

Things to look out for:
- Make sure the middleware is added to the pipeline in the right order. Authentication middleware has to go first or the rest of the middleware won't be able to make use of it and Authorize attributes on routes will fail.
- Restrict CORS policy to at least the origin of your web app
- Sometimes it's useful to check out the contents of your Jwt token at http://jwt.io
