import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TimesheetComponent } from './timesheet/timesheet.component';
import { MainComponent } from './main/main.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './services/authguard.service';
import { PasswordchangeComponent } from './passwordchange/passwordchange.component';
import { HrComponent } from './hr/hr.component';
import { PayrollComponent } from './payroll/payroll.component';
import { DashboardComponent } from './dashboard/dashboard.component';

const routes: Routes = [
  {
    path: 'main', component: MainComponent, /*canActivate: [AuthGuard],*/ children: [
    { path: 'dashboard', component: DashboardComponent },
    { path: 'timesheet', component: TimesheetComponent },
    { path: 'change', component: PasswordchangeComponent },
    { path: 'hr', component: HrComponent },
    { path: 'payroll', component: PayrollComponent },
    ]
  },
  { path: 'login', component: LoginComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
