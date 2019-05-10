import { Component, OnInit } from '@angular/core';
import { User } from '../models/User';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  user: User;
  time: number;
  i: number = Math.floor((Math.random() * 9));
  viewlst: String[] = ['Un-complicate', 'Win as a Team', 'Know your Business', 'Execute with Excellence', 'Amaze Them Every Time', 'Act with Urgency', 'Say Yes First', 'Be Proud of our Home', 'Work hard, Play hard']

  constructor() { }

  ngOnInit() {
    this.user = JSON.parse(sessionStorage.getItem('User'));
    //this.user = { "employeeID": 200957, "loginID": "yseo", "employeeType": 1, "lastName": "Seo", "firstName": "YoungEun", "email": "Youngeun.Seo@hometrust.ca", "departmentName": "Data Management and Analytics", "positionTitle": "Co-op Student, Data Governance", "managerID": 94247, "jwt": "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjIwMDk1NyIsInR5cGUiOiIxIn0.O9s3u9rIWDlUU5uht2NsRrupmyHbE4mae2U4DwoqohY" };
    this.time = Date.now();
  }

  after(val: number) {
    const len = this.viewlst.length;
    var x = this.i;
    if ((x += val) < 0) {
      return this.i = len - 1;
    } else if ((x += val) > len ) {
      return this.i = 0;
    } else {
      this.i += val;
    }
  }
}
