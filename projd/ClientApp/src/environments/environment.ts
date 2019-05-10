// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  loginapi: 'https://localhost:44329/api/login/authenticate',
  initialtimesheet: 'https://localhost:44329/api/timesheet/initial',
  getweekly: 'https://localhost:44329/api/timesheet/weekly',
  sentmail: 'https://localhost:44329/api/email/sent',
  submit: 'https://localhost:44329/api/timesheet/submit',
  approve: 'https://localhost:44329/api/timesheet/approve',
  manager: 'https://localhost:44329/api/timesheet/manager',
  reject: 'https://localhost:44329/api/timesheet/reject',
  payroll: 'https://localhost:44329/api/admin/payroll',
  getuserlst: 'https://localhost:44329/api/admin/getuser',
  createuser: 'https://localhost:44329/api/admin/create',
  updateuser: 'https://localhost:44329/api/admin/update',
  getmaillst: 'https://localhost:44329/api/email/getemails',
  change: 'https://localhost:44329/api/login/change',
  delete: 'https://localhost:44329/api/admin/delete',
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
