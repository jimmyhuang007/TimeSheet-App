import { Component, OnInit, Inject } from '@angular/core';
import { TimesheetsService } from '../services/timesheets.service';
import { StateService } from '../services/state.services';
import { FormControl, FormGroup } from '@angular/forms';
import { User } from '../models/User';
import { Timesheet } from '../models/Timesheet';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material';
import { MainComponent } from '../main/main.component';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.css']
})
export class TimesheetComponent implements OnInit {
  applicationForm: FormGroup;
  user: User;
  fileNameDays1 = [];
  fileNameDays2 = [];

  days = ['day1', 'day2', 'day3', 'day4', 'day5', 'day6', 'day7'];

  constructor(private _timesheets: TimesheetsService, private state: StateService, public dialog: MatDialog) { }

  get timesheet1() {
    return (<FormGroup>this.applicationForm.get('timeSheet').get('timeSheet1')).controls;
  }
  get timesheet2() {
    return (<FormGroup>this.applicationForm.get('timeSheet').get('timeSheet2')).controls;
  }

  numberOnly(event): boolean {
    const charCode = event.which;
    if ((charCode >= 48 && charCode <= 57) || charCode == 8 || charCode == 46) {
      return true
    } else {
      return false;
    }
  }

  ngOnInit() {
    this.applicationForm = this.state.formConfig;
    this.user = JSON.parse(sessionStorage.getItem('User'));
    this._timesheets.sheet1.subscribe(
      (sheet1: Timesheet) => {
        this.fileNameDays1 = [];
        this.setTimesheetValues(<FormGroup>this.applicationForm.get('timeSheet').get('timeSheet1'), sheet1);
        this.applicationForm.get('timeSheet').get('comments').setValue(sheet1.comments);
        let date = new Date(this.applicationForm.get('timeSheet').get('timeSheet1').get('startDate').value);
        for (let i = 0; i < 7; i++) {
          this.fileNameDays1.push(date.getDate())
          date.setDate(date.getDate() + 1);
        }
      });
    this._timesheets.sheet2.subscribe(
      (sheet2: Timesheet) => {
        this.fileNameDays2 = [];
        this.setTimesheetValues(<FormGroup>this.applicationForm.get('timeSheet').get('timeSheet2'), sheet2);
        let date = new Date(this.applicationForm.get('timeSheet').get('timeSheet2').get('startDate').value);
        for (let i = 0; i < 7; i++) {
          this.fileNameDays2.push(date.getDate());
          date.setDate(date.getDate() + 1);
        }
      });
  }

  setTimesheetValues(form: FormGroup, timesheet: any) {
    //how did timesheet get named?
    if (form.controls) {
      const formkey = Object.keys(form.controls);
      for (let i = 0; i < formkey.length; i++) {
        form.controls[formkey[i]].setValue(timesheet[formkey[i]]);
      }
    }
  }

  getTimesheetValues(form: FormGroup, overtime: number): object {
    const formkey = Object.keys(form.controls);
    const data = {
      TimesheetID: form.controls[formkey[2]].value,
      Day1: form.controls[formkey[3]].value,
      Day2: form.controls[formkey[4]].value,
      Day3: form.controls[formkey[5]].value,
      Day4: form.controls[formkey[6]].value,
      Day5: form.controls[formkey[7]].value,
      Day6: form.controls[formkey[8]].value,
      Day7: form.controls[formkey[9]].value,
      Overtime: overtime
    };
    return data;
  }

  over1(): number {
    const x = this.timesheet1.day1.value + this.timesheet1.day2.value +
      this.timesheet1.day3.value + this.timesheet1.day4.value +
      this.timesheet1.day5.value + this.timesheet1.day6.value + this.timesheet1.day7.value;
    if (x > 44) {
      return x - 44;
    } else {
      return 0;
    }
  }
  over2(): number {
    const x = this.timesheet2.day1.value + this.timesheet2.day2.value +
      this.timesheet2.day3.value + this.timesheet2.day4.value +
      this.timesheet2.day5.value + this.timesheet2.day6.value + this.timesheet2.day7.value;
    if (x > 44) {
      return x - 44;
    } else {
      return 0;
    }
  }

  onSubmit() {
    const sheet1 = this.getTimesheetValues(<FormGroup>this.applicationForm.get('timeSheet').get('timeSheet1'), this.over1());
    const sheet2 = this.getTimesheetValues(<FormGroup>this.applicationForm.get('timeSheet').get('timeSheet2'), this.over2());
    this._timesheets.submit(sheet1, sheet2, this.applicationForm.get('timeSheet').get('comments').value);
    this._timesheets.sucess.subscribe((sucess: boolean) => {
      if (sucess) {
        this.timesheet1.tStatus.setValue(10);
        this.timesheet2.tStatus.setValue(10);
        this._timesheets.sucess.next(false);
      }
    });
  }

  onApprove() {
    var sheet1ID = this.timesheet1["timesheetID"].value;
    var sheet2ID = this.timesheet2["timesheetID"].value;
    this._timesheets.approve(sheet1ID, sheet2ID);
    this._timesheets.sucess.subscribe((sucess: boolean) => {
      if (sucess) {
        this.timesheet1.tStatus.setValue(20);
        this.timesheet2.tStatus.setValue(20);
        this.timesheet1.dateApproved.setValue(Date());
        this._timesheets.sucess.next(false);
      }
    });
  }

  onReject() {
    var sheet1ID = this.timesheet1["timesheetID"].value;
    var sheet2ID = this.timesheet2["timesheetID"].value;
    this._timesheets.reject(sheet1ID, sheet2ID, this.applicationForm.get('timeSheet').get('comments').value);
    this._timesheets.sucess.subscribe((sucess: boolean) => {
      if (sucess) {
        this.timesheet1.tStatus.setValue(5);
        this.timesheet2.tStatus.setValue(5);
        this._timesheets.sucess.next(false);
      }
    });
  }

}
