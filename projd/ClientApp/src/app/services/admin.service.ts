import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';


@Injectable({
  providedIn: 'root'
})
export class AdminService {
  form1: FormGroup = this.initForm();
  form2: FormGroup = this.initForm();

  private initForm(): FormGroup {
    return this.fb.group({
      employeeID: 0,
      loginID: '',
      passwordID: '',
      employeeType: 0,
      managerID: 0,
      lastName: '',
      firstName: '',
      email: '',
      departmentName: '',
      positionTitle: '',
    })
  }

  constructor(private http: HttpClient, private fb: FormBuilder) { }

  submit(val1: Date, val2: Date) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    var _user = JSON.parse(sessionStorage.getItem('User'));

    const data = JSON.stringify({
      "StartDate": val1,
      "EndDate": val2,
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "jwt": _user.jwt
    });
    return this.http.post(environment.payroll, data, httpOptions);
  }

  getlstUser() {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    var _user = JSON.parse(sessionStorage.getItem('User'));
    const data = JSON.stringify({
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "jwt": _user.jwt
    });
    return this.http.post(environment.getuserlst, data, httpOptions);
  }

  update(val1: object) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    var _user = JSON.parse(sessionStorage.getItem('User'));
    const data = JSON.stringify({
      "AdminUse": val1,
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "jwt": _user.jwt
    });
    return this.http.post(environment.updateuser, data, httpOptions);
  }

  create(val1: object) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    var _user = JSON.parse(sessionStorage.getItem('User'));
    const data = JSON.stringify({
      "AdminUse": val1,
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "jwt": _user.jwt
    });
    return this.http.post(environment.createuser, data, httpOptions);
  }

  delete(val1: object) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
    };
    var _user = JSON.parse(sessionStorage.getItem('User'));
    const data = JSON.stringify({
      "AdminUse": val1,
      "EmployeeID": _user.employeeID,
      "EmployeeType": _user.employeeType,
      "jwt": _user.jwt
    });
    return this.http.post(environment.delete, data, httpOptions);
  }

}
