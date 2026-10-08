import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UsersService } from './users.service';
import { User } from '../../core/models/models';

@Component({
  standalone: false,
  templateUrl: './user-form.component.html',
})
export class UserFormComponent {
  isEdit: boolean;
  userName = '';
  email = '';
  ci = '';
  nombres = '';
  apellidos = '';
  telefono = '';
  selectedRoles: string[] = [];
  saving = false;

  roles = ['ADMIN', 'SUPERINTENDENTE', 'AUXILIAR', 'JEFE_GRUPO'];

  constructor(
    private service: UsersService,
    private dialogRef: MatDialogRef<UserFormComponent>,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: { user?: User },
  ) {
    this.isEdit = !!data.user;
    if (data.user) {
      this.userName = data.user.userName;
      this.email = data.user.email;
      this.selectedRoles = [...data.user.roles];
    }
  }

  get isAdmin() {
    return this.data.user?.userName === 'admin';
  }

  save() {
    if (!this.userName.trim()) return;
    if (!this.isEdit && (!this.ci.trim() || !this.nombres.trim() || !this.apellidos.trim())) return;

    this.saving = true;

    if (this.isEdit) {
      this.service
        .update(this.data.user!.id, {
          userName: this.userName,
          email: this.email,
          roles: this.selectedRoles,
        })
        .subscribe({
          next: () => {
            this.snackBar.open('Usuario actualizado', 'Cerrar', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (err) => {
            console.error('Error updating user:', err);
            this.snackBar.open('Error al actualizar usuario', 'Cerrar', { duration: 3000 });
            this.saving = false;
          },
        });
    } else {
      this.service
        .create({
          userName: this.userName,
          email: this.email,
          role: this.selectedRoles[0],
          ci: this.ci,
          nombres: this.nombres,
          apellidos: this.apellidos,
          telefono: this.telefono,
        })
        .subscribe({
          next: () => {
            this.snackBar.open('Usuario creado. Las credenciales se enviarán por SMTP.', 'Cerrar', { duration: 5000 });
            this.dialogRef.close(true);
          },
          error: (err) => {
            console.error('Error creating user:', err);
            this.snackBar.open('Error al crear usuario', 'Cerrar', { duration: 3000 });
            this.saving = false;
          },
        });
    }
  }

  cancel() {
    this.dialogRef.close();
  }
}
