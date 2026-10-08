import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  standalone: false,
  selector: 'app-set-password',
  templateUrl: './set-password.component.html',
  styleUrls: ['../login/login.component.css']
})
export class SetPasswordComponent implements OnInit {
  userNameOrEmail = '';
  currentPassword = '';
  newPassword = '';
  confirmPassword = '';
  hideCurrent = true;
  hideNew = true;
  hideConfirm = true;

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
      this.userNameOrEmail = params['user'] || params['email'] || this.auth.userName() || '';
    });
  }

  formInvalid(): boolean {
    return (
      !this.userNameOrEmail.trim() ||
      !this.currentPassword ||
      !this.newPassword ||
      this.newPassword.length < 6 ||
      this.newPassword !== this.confirmPassword
    );
  }

  changePassword() {
    if (this.newPassword !== this.confirmPassword) {
      this.error = 'La nueva contraseña y su confirmación no coinciden';
      return;
    }

    if (this.newPassword.length < 6) {
      this.error = 'La nueva contraseña debe tener al menos 6 caracteres';
      return;
    }

    this.error = '';
    this.loading = true;

    const payload = {
      userName: this.userNameOrEmail.includes('@') ? undefined : this.userNameOrEmail,
      email: this.userNameOrEmail.includes('@') ? this.userNameOrEmail : undefined,
      currentPassword: this.currentPassword,
      newPassword: this.newPassword
    };

    this.auth.setPassword(payload).subscribe({
      next: () => {
        this.loading = false;
        this.success = true;
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err.error?.message || 'Contraseña actual incorrecta o error al actualizar.';
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
