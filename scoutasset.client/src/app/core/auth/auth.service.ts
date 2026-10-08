import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, tap, throwError } from 'rxjs';
import { LoginRequest, LoginResponse } from '../models/models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'auth_token';
  private readonly userKey = 'auth_user';

  isAuthenticated = signal(false);
  userRoles = signal<string[]>([]);
  userName = signal<string>('');

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadSession();
  }

  login(request: LoginRequest) {
    return this.http
      .post<LoginResponse>('/api/auth/login', request)
      .pipe(
        tap((res) => {
          localStorage.setItem(this.tokenKey, res.token);
          localStorage.setItem(
            this.userKey,
            JSON.stringify({ userName: res.userName, roles: res.roles })
          );
          this.isAuthenticated.set(true);
          this.userRoles.set(res.roles);
          this.userName.set(res.userName);
        })
      );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticated.set(false);
    this.userRoles.set([]);
    this.userName.set('');
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  hasRole(role: string): boolean {
    return this.userRoles().includes(role);
  }

  forgotPassword(email: string) {
    return this.http.post('/api/auth/forgot-password', { email });
  }

  resetPassword(data: { email: string; token: string; newPassword: string }) {
    return this.http.post('/api/auth/reset-password', data);
  }

  setPassword(data: { email?: string; userName?: string; currentPassword: string; newPassword: string }) {
    return this.http.post('/api/auth/set-password', data);
  }

  private loadSession() {
    const token = localStorage.getItem(this.tokenKey);
    const userData = localStorage.getItem(this.userKey);
    if (token && userData) {
      const user = JSON.parse(userData);
      this.isAuthenticated.set(true);
      this.userRoles.set(user.roles);
      this.userName.set(user.userName);
    }
  }
}
