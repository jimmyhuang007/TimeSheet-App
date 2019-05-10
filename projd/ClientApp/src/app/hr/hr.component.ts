import { Component, OnInit, Inject } from '@angular/core';
import { User } from '../models/User';
import { FormGroup } from '@angular/forms';
import { AdminService } from '../services/admin.service';
import { MatDialog } from '@angular/material';
import { DialogComponent } from '../dialog/dialog.component';

@Component({
  selector: 'app-hr',
  templateUrl: './hr.component.html',
  styleUrls: ['./hr.component.css']
})

export class HrComponent implements OnInit {
  form1: FormGroup;
  form2: FormGroup;
  lstuser: User[];
  managerlst: User[];

  roles: role[] = [
    { value: 1, viewValue: 'Employee' },
    { value: 2, viewValue: 'Manager' }
  ];

  numberOnly(event): boolean {
    const charCode = event.which;
    if ((charCode >= 48 && charCode <= 57) || charCode == 8) {
      return true
    } else {
      return false;
    }
  }

  constructor(private admins: AdminService, private dialog: MatDialog) { }

  ngOnInit() {
    this.form1 = this.admins.form1;
    this.form2 = this.admins.form2;
    this.admins.getlstUser().subscribe(
      (userlst: any) => {
        this.lstuser = userlst;
        this.managerlst = userlst.filter(obj => {
          return obj.employeeType === 2;
        });
      });
  }

  select(val1 : any) {
    var result = this.lstuser.find(obj => obj.employeeID === val1);
    console.log(result);
    this.setformValues(this.form1, result);
  }

  setformValues(form: FormGroup, user: any) {
    if (form.controls) {
      const formkey = Object.keys(form.controls);
      for (let i = 0; i < formkey.length; i++) {
        form.controls[formkey[i]].setValue(user[formkey[i]]);
      }
    }
  }

  getUserValues(form: FormGroup): object {
    const formkey = Object.keys(form.controls);
    const data = {
      EmployeeID: form.controls[formkey[0]].value,
      LoginID: form.controls[formkey[1]].value,
      PasswordID: form.controls[formkey[2]].value,
      EmployeeType: form.controls[formkey[3]].value,
      LastName: form.controls[formkey[5]].value,
      FirstName: form.controls[formkey[6]].value,
      Email: form.controls[formkey[7]].value,
      DepartmentName: form.controls[formkey[8]].value,
      PositionTitle: form.controls[formkey[9]].value,
      ManagerID: form.controls[formkey[4]].value,
    };
    return data;
  }

  update() {
    var updateuser = this.getUserValues(this.form1);
    this.admins.update(updateuser).subscribe(
      (userlst: any) => {
        this.lstuser = userlst;
        this.managerlst = userlst.filter(obj => {
          return obj.employeeType === 2;
        });
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Update Success' }
        })
      },
      (error) => {
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Update Failed' }
        });
      });
  }

  create() {
    var createuser = this.getUserValues(this.form2);
    this.admins.create(createuser).subscribe(
      (userlst: any) => {
        this.lstuser = userlst;
        this.managerlst = userlst.filter(obj => {
          return obj.employeeType === 2;
        });
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Create Success' }
        })
      },
      (error) => {
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Create Failed' }
        });
      });
  }

  delete() {
    var deleteuser = this.getUserValues(this.form1);
    this.admins.delete(deleteuser).subscribe(
      (userlst: any) => {
        this.lstuser = userlst;
        this.managerlst = userlst.filter(obj => {
          return obj.employeeType === 2;
        });
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Delete Success' }
        })
      },
      (error) => {
        const dialogRef = this.dialog.open(DialogComponent, {
          data: { action: 'Delete Failed' }
        });
      });
  }

}

export interface role {
  value: number,
  viewValue: string,
}

