import { Component, OnInit } from '@angular/core';
import {TimesheetsService} from '../services/timesheets.service';
import {StateService} from '../services/state.services';
import {User} from '../models/User';
import { Quicksheet } from '../models/Quicksheet';
import { Router, Route } from '@angular/router';
import {MatDialog} from '@angular/material';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  user: User;
  timesheets: Quicksheet[];

  constructor(private sheetservice: TimesheetsService, private state: StateService, private dialog: MatDialog) { }

  ngOnInit() {
    this.user = JSON.parse(sessionStorage.getItem('User'));
    this.timesheets = JSON.parse(sessionStorage.getItem('Sheets'));
    this.sheetservice.sidebarsheets.subscribe(
      (biweekly) => {
        this.timesheets = biweekly;
      });
  }

  onTimesheetSelect(date1: number, date2: number,) {
    this.sheetservice.getTimesheet(date1, date2);
  }

  tosetting() {
    this.sheetservice.tochange();
  }
}
