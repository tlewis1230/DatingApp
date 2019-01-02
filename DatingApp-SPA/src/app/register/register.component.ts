import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 // @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();

  // model: any = {};
  user: User;
  registerForm: FormGroup;
  // make ALL properties in type optional
  bsConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService, private router:Router,
    private alertify: AlertifyService, private fb: FormBuilder) { }

  ngOnInit() {
    // Form Group contains form controls
    // this.registerForm = new FormGroup({
    //  username: new FormControl('', Validators.required),
    //  password: new FormControl('',
    //  [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //  confirmPassword: new FormControl('', Validators.required)
    //  }, this.passwordMatchValidator);
    this.bsConfig = {
      containerClass: 'theme-red'
    },
    this.createRegisterForm();
    }

createRegisterForm() {
// equivalent to above this.registerForm = new FormGroup
  this.registerForm = this.fb.group ({
    // formstate, validators
    gender: ['male'],
    username: ['', Validators.required],
    knownAs: ['', Validators.required],
    dateOfBirth: [null, Validators.required],
    city: ['', Validators.required],
    country: ['', Validators.required],
    password: ['',[Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
    confirmPassword: ['', Validators.required]
  },
  {validator: this.passwordMatchValidator});
}


    passwordMatchValidator(g: FormGroup) {
      return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};

    }

  register() {
    if (this.registerForm.valid) {
      // take values from register form and pass them to our user object
      // clones values from this.registerForm.value into empty object {}
      // 3rd param: also want to log user in after registration
      this.user = Object.assign ({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Registration successful');
        }, error => {
           this.alertify.error(error);
        }, () => {
        this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
        });
      });
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
    // console.log('cancelled');
  }
}
