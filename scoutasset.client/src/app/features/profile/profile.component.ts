import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../core/auth/auth.service';

export interface UserProfileData {
  id: string;
  userName: string;
  email: string;
  roles: string[];
  ci: string;
  nombres: string;
  apellidos: string;
  telefono: string;
}

@Component({
  standalone: false,
  selector: 'app-profile',
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  profile: UserProfileData = {
    id: '',
    userName: '',
    email: '',
    roles: [],
    ci: '',
    nombres: '',
    apellidos: '',
    telefono: ''
  };

  loading = true;
  savingProfile = false;
  savingPassword = false;

  // Change Password Form
  currentPassword = '';
  newPassword = '';
  confirmPassword = '';
  hideCurrent = true;
  hideNew = true;
  hideConfirm = true;

  constructor(
    private http: HttpClient,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.loading = true;
    this.cdr.detectChanges();

    this.http.get<UserProfileData>('/api/Profile').subscribe({
      next: (data) => {
        this.profile = data || this.profile;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
        this.snackBar.open('Error al cargar la información del perfil', 'Cerrar', { duration: 3000 });
        this.cdr.detectChanges();
      }
    });

    // Safety net
    setTimeout(() => {
      if (this.loading) {
        this.loading = false;
        this.cdr.detectChanges();
      }
    }, 5000);
  }

  updateProfile() {
    if (!this.profile.nombres.trim() || !this.profile.apellidos.trim()) {
      this.snackBar.open('Nombres y apellidos son obligatorios', 'Cerrar', { duration: 3000 });
      return;
    }

    this.savingProfile = true;
    this.cdr.detectChanges();

    const body = {
      ci: this.profile.ci,
      nombres: this.profile.nombres,
      apellidos: this.profile.apellidos,
      telefono: this.profile.telefono
    };

    this.http.put('/api/Profile', body).subscribe({
      next: () => {
        this.savingProfile = false;
        this.snackBar.open('Perfil actualizado exitosamente', 'Cerrar', { duration: 3000 });
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.savingProfile = false;
        this.snackBar.open(err.error?.message || 'Error al actualizar el perfil', 'Cerrar', { duration: 3000 });
        this.cdr.detectChanges();
      }
    });
  }

  changePassword() {
    if (!this.currentPassword) {
      this.snackBar.open('Ingresa tu contraseña actual', 'Cerrar', { duration: 3000 });
      return;
    }

    if (this.newPassword.length < 6) {
      this.snackBar.open('La nueva contraseña debe tener al menos 6 caracteres', 'Cerrar', { duration: 3000 });
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.snackBar.open('La nueva contraseña y su confirmación no coinciden', 'Cerrar', { duration: 3000 });
      return;
    }

    this.savingPassword = true;
    this.cdr.detectChanges();

    const body = {
      currentPassword: this.currentPassword,
      newPassword: this.newPassword
    };

    this.http.post('/api/Profile/change-password', body).subscribe({
      next: () => {
        this.savingPassword = false;
        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';
        this.snackBar.open('¡Contraseña cambiada exitosamente!', 'Cerrar', { duration: 4000 });
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.savingPassword = false;
        this.snackBar.open(err.error?.message || 'Contraseña actual incorrecta', 'Cerrar', { duration: 4000 });
        this.cdr.detectChanges();
      }
    });
  }
}
