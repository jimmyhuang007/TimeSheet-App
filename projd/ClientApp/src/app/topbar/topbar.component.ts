import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../services/authorization.service';
import { User } from '../models/User';
import { Router } from '@angular/router';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.css']
})

export class TopbarComponent implements OnInit {
  user: User;
  constructor(private router: Router) { }

  ngOnInit() {
    this.user = JSON.parse(sessionStorage.getItem('User'));
  }

  dashboard() {
    this.router.navigate(['/main/dashboard']);
  }

  hr() {
    this.router.navigate(['/main/hr']);
  }

  payroll() {
    this.router.navigate(['/main/payroll']);
  }

  logout() {
    // remove user from local storage to log user out
    sessionStorage.removeItem('User');
    sessionStorage.removeItem('Sheets');
    sessionStorage.clear();
    this.router.navigate(['/login']);
  }
}
