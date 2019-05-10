import { Injectable } from '@angular/core';
import {FormGroup, FormControl, FormBuilder, FormArray, Validators, Form} from '@angular/forms';
import {User} from '../models/User';

@Injectable({
  providedIn: 'root'
})

export class StateService {
  user: User;
  constructor( private fb: FormBuilder) {}

  formConfig: FormGroup = new FormGroup({
    timeSheet: this.initTimesheet()
  });

  public initUser() {
    this.user = JSON.parse(sessionStorage.getItem('User'));
  }


  public initTimesheet(): FormGroup {
    return this.fb.group({
      timeSheet1: this.fb.group({
        startDate: '',
        endDate: '',
        timesheetID: 0,
        day1: [0, [Validators.min(0), Validators.max(24)]],
        day2: [0, [Validators.min(0), Validators.max(24)]],
        day3: [0, [Validators.min(0), Validators.max(24)]],
        day4: [0, [Validators.min(0), Validators.max(24)]],
        day5: [0, [Validators.min(0), Validators.max(24)]],
        day6: [0, [Validators.min(0), Validators.max(24)]],
        day7: [0, [Validators.min(0), Validators.max(24)]],
        overtime: [0],
        tStatus: [0],
        dateApproved: '',
      }),
      timeSheet2: this.fb.group({
        startDate: '',
        endDate: '',
        timesheetID: 0,
        day1: [0, [Validators.min(0), Validators.max(24)]],
        day2: [0, [Validators.min(0), Validators.max(24)]],
        day3: [0, [Validators.min(0), Validators.max(24)]],
        day4: [0, [Validators.min(0), Validators.max(24)]],
        day5: [0, [Validators.min(0), Validators.max(24)]],
        day6: [0, [Validators.min(0), Validators.max(24)]],
        day7: [0, [Validators.min(0), Validators.max(24)]],
        overtime: [0],
        tStatus: [0],
        dateApproved: '',
      }),
      comments: '',
    });
  }


}
