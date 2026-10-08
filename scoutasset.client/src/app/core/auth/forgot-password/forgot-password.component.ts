import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  standalone: false,
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['../login/login.component.css']
})
export class ForgotPasswordComponent {
  email = '';
  loading = false;
  success = false;
  error = '';

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  sendResetEmail() {
    if (!this.email.trim()) {
      this.error = 'Por favor ingresa tu correo electrónico';
      return;
    }

    this.error = '';
    this.loading = true;

    this.auth.forgotPassword(this.email.trim()).subscribe({
      next: () => {
        this.loading = false;
        this.success = true;
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err.error?.message || 'Ocurrió un error al procesar tu solicitud';
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
