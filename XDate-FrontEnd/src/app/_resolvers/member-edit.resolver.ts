import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';


@Injectable()
export class MemberEditResolver implements Resolve<User> {
    constructor(private userService: UserService, private alertify: AlertifyService,
                private router: Router, private auth: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        // tslint:disable-next-line:no-string-literal
    return this.userService.getUser(this.auth.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problem retreiving your data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}