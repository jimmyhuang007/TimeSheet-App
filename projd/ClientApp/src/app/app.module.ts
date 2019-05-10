import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { MatNativeDateModule } from '@angular/material';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ngMaterialModule } from './ngMaterial';
import { Routes, RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TopbarComponent } from './topbar/topbar.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { HttpClientModule} from '@angular/common/http';
import { LoginComponent } from './login/login.component';
import { MainComponent } from './main/main.component';
import { PayrollComponent } from './payroll/payroll.component';
import { HrComponent } from './hr/hr.component';
import { PasswordchangeComponent } from './passwordchange/passwordchange.component';
import {StatusTextPipe} from './models/StatusText';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DialogComponent } from './dialog/dialog.component';
import { TimesheetComponent } from './timesheet/timesheet.component';

@NgModule({
  declarations: [
    AppComponent,
    TimesheetComponent,
    TopbarComponent,
    SidebarComponent,
    LoginComponent,
    MainComponent,
    PayrollComponent,
    HrComponent,
    PasswordchangeComponent,
    StatusTextPipe,
    DashboardComponent,
    DialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatNativeDateModule,
    ngMaterialModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  entryComponents: [TimesheetComponent, DialogComponent],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
