import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../../_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;

  user: User;

  @HostListener('window:beforeunload', ['$event'] )
  onloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
    private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
      this.route.data.subscribe(data => {
        this.user = data['user'];
    });
  }

  updateUser() {
    // get decoded token id
    // returnining an observable, so we need to subscribe to it
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertify.success('Profile updated successfully!');
      // if this is blank it will not show ANY user data on the page
      // this.editForm.reset();
      // this should reset the form values to what their saved values
      this.editForm.reset(this.user);
    }, error => {
      this.alertify.error(error);
    });
  }
}
