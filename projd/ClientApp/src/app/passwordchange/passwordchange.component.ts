import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../services/authorization.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-passwordchange',
  templateUrl: './passwordchange.component.html',
  styleUrls: ['./passwordchange.component.css']
})
export class PasswordchangeComponent implements OnInit {
  changeform: FormGroup;
  constructor(private author: AuthorizationService, private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.changeform = this.formBuilder.group({
      LoginID: ['', Validators.required],
      PasswordID: ['', Validators.required],
      NewPassword: ['', Validators.required],
    });
  }

  change() {
    this.author.change(this.changeform.controls.LoginID.value, this.changeform.controls.PasswordID.value, this.changeform.controls.NewPassword.value);
  }

}
