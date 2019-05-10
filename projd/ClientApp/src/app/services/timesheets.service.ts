import { Injectable } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, FormArray, Validators, Form } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { StateService } from './state.services';
import { Quicksheet } from '../models/Quicksheet';
import { Router } from '@angular/router';
import { Timesheet } from '../models/Timesheet';
import { environment } from 'src/environments/environment';
import { User } from '../models/User';
import { MatDialog } from '@angular/material';
import { error } from '@angular/compiler/src/util';
import { DialogComponent } from '../dialog/dialog.component';

@Injectable({
  providedIn: 'root'
})
export class TimesheetsService {
  public clicked = new Subject<any>();
  public sheet1 = new Subject<Timesheet>();
  public sheet2 = new Subject<Timesheet>();
  public sidebarsheets = new Subject<Quicksheet[]>();
  public sucess = new Subject<boolean>();

  constructor(private fb: FormBuilder,
    private http: HttpClient,
    private state: StateService,
    private router: Router,
    private dialog: MatDialog) { }

  getTimesheet(timesheetId1: number, timesheetId2: number) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    var _user = JSON.parse(sessionStorage.getItem('User'));

    const week1ID = JSON.stringify({
      "TimesheetID": timesheetId1,
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "jwt": _user.jwt
    });
    this.http.post(environment.getweekly, week1ID, httpOptions).subscribe(
      (week1: any) => this.sheet1.next(week1));

    const week2ID = JSON.stringify({
      'TimesheetID': timesheetId2,
      'EmployeeID': _user.employeeID,
      "EmployeeType": _user.employeeType,
      'jwt': _user.jwt
    });
    this.http.post(environment.getweekly, week2ID, httpOptions).subscribe(
      (week2: any) => this.sheet2.next(week2));

    this.router.navigate(['/main/timesheet']);
  }

  onTimesheetClick() {
    //const timesheets = this.getTimesheet("", "");
    //this.clicked.next(timesheets);//sheet1 then sheet2
  }

  submit(sheet1: object, sheet2: object, comments: string) {
    var _user = JSON.parse(sessionStorage.getItem('User'));
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    const data = JSON.stringify({
      "timesheet1": sheet1,
      "timesheet2": sheet2,
      "Comments": comments,
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "ManagerID" : _user.managerID,
      "jwt": _user.jwt
    });
    this.http.post(environment.submit, data, httpOptions).subscribe(
      (biweekly: any) => {
        sessionStorage.setItem('Sheets', JSON.stringify(biweekly));
        this.sidebarsheets.next(biweekly);
        this.sucess.next(true);

        var maillst = JSON.parse(sessionStorage.getItem('Maillst'));
        var result = maillst.find(obj => obj.employeeID === _user.managerID);
        const datamail = JSON.stringify({
          "EmailAddress": result.emailAddress,
          "MailSubject": "Timesheet Application: Submit",
          "MailBody": _user.firstName + " has just submitted a timesheet for you to review. Please login at https://localhost:44390/login"
        });
        this.http.put(environment.sentmail, datamail, httpOptions).subscribe(response => { });

        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Submit Success' }
        });
      },
      (error) => {
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Submit Failed' }
        });
      });
  }

  approve(sheet1ID: number, sheet2ID: number) {
    var _user = JSON.parse(sessionStorage.getItem('User'));
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    const data = JSON.stringify({
      "Sheet1ID": sheet1ID,
      "Sheet2ID": sheet2ID,
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "jwt": _user.jwt
    });
    this.http.post(environment.approve, data, httpOptions).subscribe(
      (biweekly: any) => {
        sessionStorage.setItem('Sheets', JSON.stringify(biweekly));
        this.sidebarsheets.next(biweekly);
        this.sucess.next(true);

        var mail = JSON.parse(sessionStorage.getItem('Maillst')).find(obj => obj.employeeID === _user.employeeID);
        const datamail = JSON.stringify({
          "EmailAddress": mail.employeeMail,
          "MailSubject": "Timesheet Application: Approved",
          "MailBody": _user.firstName + " has just approved your timesheet."
        });
        this.http.put(environment.sentmail, datamail, httpOptions).subscribe(response => { });

        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Approve Success' }
        });
      },
      (error) => {
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Approve Failed' }
        });
      });
  }

  reject(sheet1ID: number, sheet2ID: number, comments: string) {
    var _user = JSON.parse(sessionStorage.getItem('User'));
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    const data = JSON.stringify({
      "Sheet1ID": sheet1ID,
      "Sheet2ID": sheet2ID,
      "Comments": comments,
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "jwt": _user.jwt
    });
    this.http.post(environment.reject, data, httpOptions).subscribe(
      (biweekly: any) => {
        sessionStorage.setItem('Sheets', JSON.stringify(biweekly));
        this.sidebarsheets.next(biweekly);
        this.sucess.next(true);

        var mail = JSON.parse(sessionStorage.getItem('Maillst')).find(obj => obj.employeeID === _user.employeeID);
        const datamail = JSON.stringify({
          "EmailAddress": mail.employeeMail,
          "MailSubject": "Timesheet Application: Reject",
          "MailBody": _user.firstName + " has just rejected your timesheet with reason: " + comments +". Please edit at https://localhost:44390/login"
        });
        this.http.put(environment.sentmail, datamail, httpOptions).subscribe(response => { });

        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Reject Success' }
        });
      },
      (error) => {
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Reject Failed' }
        });
      });
  }

  tochange() {
    this.router.navigate(['/main/change']);
  }

}

