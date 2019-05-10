import { Injectable } from '@angular/core';
import {FormGroup, FormControl, FormBuilder, FormArray, Validators, Form} from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class SubmitService {

  constructor( private fb: FormBuilder) {}

  formConfig: FormGroup = new FormGroup({
    timeSheet: this.initTimesheet()
  });

  public initTimesheet() : FormGroup {
    return this.fb.group({
      employeeId: '',
      timesheetId: '',
      startDate: '',
      endDate: '',
      day1: [0, Validators.required],
      day2: [0, Validators.required],
      day3: [0, Validators.required],
      day4: [0, Validators.required],
      day5: [0, Validators.required],
      day6: [0, Validators.required],
      day7: [0, Validators.required],
    });
  }


}
