import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { RolesModelComponent } from '../roles-model/roles-model.component';

@Component({
  selector: 'app-user-managment',
  templateUrl: './user-managment.component.html',
  styleUrls: ['./user-managment.component.css']
})
export class UserManagmentComponent implements OnInit {
  users: User[];
  bsModalRef: BsModalRef;

  constructor(private admin: AdminService,
              private modalService: BsModalService, 
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.admin.getUserRoles().subscribe((users: User[]) => {
      this.users=users;
    }, error => {
      this.alertify.error(error);
    })
  }

  editRolesModal(user: User) {
    const initialState = {
      user,
      roles: this.getRolesArray(user)
    };
    this.bsModalRef = this.modalService.show(RolesModelComponent, {initialState});
    this.bsModalRef.content.updateSelectedRoles.subscribe((values)=> {
      const rolesToUpdate={
        roleNames: [...values.filter(el=>el.checked===true).map(el=>el.name)]
      };

      if(rolesToUpdate) {
        this.admin.updateUserRoles(user, rolesToUpdate).subscribe(()=> {
          user.roles=[...rolesToUpdate.roleNames];
        }, error => {
          this.alertify.error(error);
        });
      }
    })
  }

  private getRolesArray(user) {
    const roles=[];
    const userRoles=user.roles;
    const availableRoles: any[]=[
      {name: 'Admin', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
      {name: 'Member', value: 'Member'},
      {name: 'VIP', value: 'VIP'}
    ];

    for(let i=0;i<availableRoles.length;i++){
      let isMatch=false;
      for(let j=0;j<userRoles.length;j++) {
        if(availableRoles[i].name===userRoles[j]) {
          isMatch=true;
          availableRoles[i].checked=true;
          roles.push(availableRoles[i]);
          break;
        }
      }

      if(!isMatch) {
        availableRoles[i].checked=false;
        roles.push(availableRoles[i]);
      }
    }
      return roles;
    
  }
}
