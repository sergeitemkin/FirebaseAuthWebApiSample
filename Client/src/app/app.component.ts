import { Component } from '@angular/core';
import * as firebase from 'firebase';
import { Http, Response, RequestOptionsArgs, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'check out that console window!';

  constructor (private http: Http) {}

  ngOnInit()
  {
    // This is straight from the Firebase snippet. Note that you'll want
    // to restrict your Api key when you go live at:
    // https://console.developers.google.com/apis/credentials?project={YOUR_FIREBASE_PROJECTID}
    //
    // Once you get there, edit your Api key, and set it to have HTTP referrers. Then 
    // whitelist your web app there.
    var config = {
      apiKey: '',
      authDomain: ''
    };
    if (!config.apiKey || !config.authDomain)
    {
      console.error('Need to add your Api key and authDomain (from Firebase project settings)');
      return;
    }

    firebase.initializeApp(config);

      // if user is going through a 3rd party login, like Google
    firebase.auth().getRedirectResult().then((result) => {
      if (!result.user)
      {
        // No user, so redirect to provider for authentication
        firebase.auth().signInWithRedirect(new firebase.auth.GoogleAuthProvider());
      }
      else {
        // This is a redirect from the provider, yay!

        console.log('doing the thing via Google auth')
        // No idea why Dd is the IdToken ok, but it is.
        this.doTheThing(result.user.Dd);
      }
    });

    firebase.auth()
      // if user is sining in with email/password
      .signInWithEmailAndPassword('testemail@stuff.com', 'lololol')
      .then((result) => {
        console.log('doing the thing via email/password')
        // No idea why Dd is the IdToken ok, but it is.
        this.doTheThing(result.Dd);
    });
  }

  private doTheThing(idToken)
  {
    var headers = new Headers();
    headers.append('Authorization', 'Bearer ' + idToken);

    var observable = this.http
      .get('http://localhost:5000/api/claims', { headers: headers })
      .map(this.extractData)
      .catch(this.handleError);

    console.log('going to http://localhost:5000/api/claims with this token:', idToken);
    observable.forEach((value) => {
      console.log('got you these claims:', value);
    });
  }

  private extractData(res: Response) {
    let claims = res.json();
    return claims || { };
  }

  private handleError (error: Response | any) {
    // In a real world app, we might use a remote logging infrastructure
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || '';
      const err = body.error || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.error(errMsg);
    return Observable.throw(errMsg);
  }
}
