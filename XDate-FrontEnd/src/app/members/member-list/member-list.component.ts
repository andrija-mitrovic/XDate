import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  user = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'male', dispaly: 'Males'}, {value: 'female', dispaly: 'Female'}];
  userParams: any = {};  
  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
   this.route.data.subscribe(data => {
     this.users = data['users'].result;
     this.pagination=data['users'].pagination;
   });

   this.userParams.gender = this.user.gender === 'male' ? 'female' : 'male';
   this.userParams.minAge = 18;
   this.userParams.maxAge = 99;
   this.userParams.orderBy = 'lastActive';
  }

  resetUsers() {
    this.userParams.gender = this.user.gender === 'male' ? 'female' : 'male';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUsers();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      .subscribe((res: PaginatedResult<User[]>) => {
        this.users=res.result;
        this.pagination=res.pagination;
      }, error => {
        this.alertify.error(error);
      });
  }
}
