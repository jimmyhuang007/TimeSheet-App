import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import {Router} from '@angular/router';
import {AuthorizationService} from './authorization.service';
import {StateService} from './state.services';

@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate {
  constructor(private auth: AuthorizationService,
              private myRoute: Router,
              private state: StateService) {}

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this.state.user) {
      return true;
    } else {
      return false;
    }
  }
}
