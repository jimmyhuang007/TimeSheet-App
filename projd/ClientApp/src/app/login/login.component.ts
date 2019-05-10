import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthorizationService } from '../services/authorization.service';
import {Router} from '@angular/router';
import {User} from '../models/User';
import {StateService} from '../services/state.services';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  error = false;

  constructor(private formBuilder: FormBuilder,
              private author: AuthorizationService,
              private router: Router,
              private state: StateService) {}

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      LoginID: ['', Validators.required],
      PasswordID: ['', Validators.required]
    });
  }

  onlogin() { 
    this.author.login(this.loginForm.controls.LoginID.value, this.loginForm.controls.PasswordID.value);
    this.error = this.author.error;
  }
}
