import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  decodedToken: any;
  constructor(private http: HttpClient) { }


  register(user: any) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  login(user: any) {
    return this.http.post(this.baseUrl + 'login', user).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          this.decodedToken = user.token;
        }
      })
    )
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }
}
