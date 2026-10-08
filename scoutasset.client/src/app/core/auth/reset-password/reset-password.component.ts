import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  standalone: false,
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['../login/login.component.css'] // Re-use the login layout CSS styles!
})
export class ResetPasswordComponent implements OnInit {
  token = '';
  email = '';
  newPassword = '';
  confirmPassword = '';
  hidePassword = true;
  hideConfirmPassword = true;
  loading = false;
  success = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private auth: AuthService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'] || '';
      this.email = params['email'] || '';

      if (!this.token || !this.email) {
        this.error = 'Enlace de restablecimiento inválido o expirado. Por favor, solicita uno nuevo.';
      }
    });
  }

  formInvalid(): boolean {
    return (
      !this.newPassword ||
      this.newPassword.length < 6 ||
      this.newPassword !== this.confirmPassword
    );
  }

  resetPassword() {
    if (this.formInvalid()) {
      this.error = 'Las contraseñas deben coincidir y tener al menos 6 caracteres';
      return;
    }

    this.error = '';
    this.loading = true;

    const payload = {
      email: this.email,
      token: this.token,
      newPassword: this.newPassword
    };

    this.auth.resetPassword(payload).subscribe({
      next: () => {
        this.loading = false;
        this.success = true;
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 5000); // Redirect to login after 5 seconds
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err.error?.message || 'Error al restablecer la contraseña. Solicita un nuevo enlace.';
      }
    });
  }
}
