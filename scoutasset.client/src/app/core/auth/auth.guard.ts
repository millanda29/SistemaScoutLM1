import { Injectable } from '@angular/core';
import {
  CanActivate,
  Router,
  ActivatedRouteSnapshot,
} from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    if (!this.auth.isAuthenticated()) {
      this.router.navigate(['/login']);
      return false;
    }

    const roles = route.data['roles'] as string[];
    if (roles && roles.length > 0) {
      const hasRole = roles.some((r) => this.auth.hasRole(r));
      if (!hasRole) {
        this.router.navigate(['/dashboard']);
        return false;
      }
    }

    return true;
  }
}
