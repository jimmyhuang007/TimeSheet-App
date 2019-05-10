import { Injectable, Input } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { User } from '../models/User';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { StateService } from './state.services';
import { environment } from 'src/environments/environment';
import { error } from '@angular/compiler/src/util';
import { TimesheetsService } from './timesheets.service';
import { get } from 'http';
import { Session } from 'protractor';
import { MatDialog } from '@angular/material';
import { DialogComponent } from '../dialog/dialog.component';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  // private currentUser: BehaviorSubject<User>;
  error = false;
  public currentUser: User;

  constructor(public http: HttpClient, private router: Router, private state: StateService, private time: TimesheetsService, private dialog: MatDialog) { }
  // this.currentUser = new BehaviorSubject<User>(JSON.parse(sessionStorage.getItem('User')));

  // public get getcurrentUser(): User {
  //   return this.currentUser.value;
  // }

  login(username: string, password: string) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    const logindata = JSON.stringify({ 'LoginID': username, 'PasswordID': password });
    return this.http.post(environment.loginapi, logindata, httpOptions).subscribe(
      (loguser: any) => {
        this.error = false;
        this.http.get(environment.getmaillst, httpOptions).subscribe((result: any) => sessionStorage.setItem('Maillst', JSON.stringify(result)));
        sessionStorage.setItem('User', JSON.stringify(loguser));
        this.state.user = loguser;
        // this.currentUser.next(loguser);
        if (loguser.employeeType == 1) {
          const initsheet = JSON.stringify({ 'EmployeeID': loguser.employeeID, 'ManagerID': loguser.managerID });
          this.http.post(environment.initialtimesheet, initsheet, httpOptions).subscribe(
            (allsheets: any) => {
              sessionStorage.setItem('Sheets', JSON.stringify(allsheets));
              //this.time.timesheets = allsheets;
              this.time.sidebarsheets.next(allsheets);
            });
        }
        else if (loguser.employeeType == 2) {
          const initdata = JSON.stringify({ 'EmployeeID': loguser.employeeID });
          this.http.post(environment.manager, initdata, httpOptions).subscribe(
            (allsheets: any) => {
              sessionStorage.setItem('Sheets', JSON.stringify(allsheets));
              this.time.sidebarsheets.next(allsheets);
            });
        }
        this.router.navigate(['/main/dashboard']);
        //elseif emplyeeType==9, this.router.navigate(['/main/hr'] or use authguard role based
      },
      (error) => {
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Invalid username/password' }
        });
        this.error = true;
      });
  }

  change(username: string, password: string, newpassword: string) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    const changedata = JSON.stringify({
      'LoginID': username,
      'PasswordID': password,
      'Newpassword': newpassword
    });
    this.http.put(environment.change, changedata, httpOptions).subscribe((response) => {
      const dialogRef = this.dialog.open(DialogComponent, {
        data: { action: 'Password Changed' }
      });
    })
  }
}
