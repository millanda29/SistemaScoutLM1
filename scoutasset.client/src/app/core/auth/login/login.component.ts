import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../auth.service';
import { PromptDialog, PromptField } from '../../../shared/prompt-dialog.component';

@Component({
  standalone: false,
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  email = '';
  password = '';
  hidePassword = true;
  rememberMe = false;
  loading = false;
  error = '';

  constructor(
    private auth: AuthService,
    private router: Router,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  login() {
    if (!this.email.trim() || !this.password) {
      this.error = 'Todos los campos son obligatorios';
      return;
    }

    this.error = '';
    this.loading = true;

    this.auth
      .login({ userName: this.email, password: this.password })
      .subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/dashboard']);
        },
        error: (err: any) => {
          this.loading = false;
          this.error = err.error?.message || 'Credenciales inválidas';
        },
      });
  }

  forgotPassword() {
    this.router.navigate(['/forgot-password']);
  }
}
