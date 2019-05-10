import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from '../models/User';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { AdminService } from '../services/admin.service';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-payroll',
  templateUrl: './payroll.component.html',
  styleUrls: ['./payroll.component.css']
})
export class PayrollComponent implements OnInit {
  Paydata: Payrow[];
  displayedColumns: string[] = ['Employee ID', 'Last Name', 'First Name', 'Manager', 'Regular Hours', 'Overtime', 'Total Hours'];

  constructor(private adminservice: AdminService) { }

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  /*applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }*/
  ngOnInit() {
  }

  onSearch(val1: Date, val2: Date) {
    if (val1 < val2) {
      this.adminservice.submit(val1, val2).subscribe(
        (paydata: Payrow[]) => {
          this.Paydata = paydata;
        }
      );
    } else {
      this.adminservice.submit(val2, val1).subscribe(
        (paydata: Payrow[]) => {
          this.Paydata = paydata;
        }
      );
    }
  }

  export() {
    /* make the worksheet */
    var ws = XLSX.utils.json_to_sheet(this.Paydata);

    /* add to workbook */
    var wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Data");

    /* write workbook */
    XLSX.writeFile(wb, 'Timesheet.xlsx');
  }
}

export interface Payrow {
  employeeID: number;
  studentF: string;
  studentL: string;
  manager: string;
  regular: number;
  overtime: number
  total: number;
}

