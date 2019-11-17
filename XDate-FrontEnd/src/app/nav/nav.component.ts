import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(private auth: AuthService) { }

  ngOnInit() {
  }

  login() {
    this.auth.login(this.model).subscribe(() => {
      console.log('Logged in successfully');
    }, error => {
      console.log(error);
    });
  }

  loggedIn() {
    return  this.auth.loggedIn();
  }

  logout() {
    localStorage.removeItem('token')
    console.log('Logged out')
  }
}
