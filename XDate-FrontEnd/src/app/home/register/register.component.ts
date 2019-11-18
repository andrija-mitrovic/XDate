import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  user: any = {};
  constructor(private auth: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  register() {
    this.auth.register(this.user).subscribe(() => {
      this.alertify.success('Registration successful')
    }, error => {
      this.alertify.error(error);
    });
  }
}
