import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UsersService } from './users.service';
import { User } from '../../core/models/models';
import { UserFormComponent } from './user-form.component';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';
import { BulkImportDialogComponent } from '../../shared/bulk-import-dialog.component';

@Component({
  standalone: false,
  selector: 'app-users',
  templateUrl: './users.component.html',
})
export class UsersComponent implements OnInit {
  displayedColumns = ['ci', 'fullName', 'userName', 'email', 'roles', 'actions'];
  dataSource = new MatTableDataSource<User>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private service: UsersService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe((res) => {
      this.dataSource.data = res;
      this.dataSource.sort = this.sort;
      this.dataSource.paginator = this.paginator;
    });
  }

  openForm(user?: User) {
    this.dialog.open(UserFormComponent, {
      width: '480px',
      data: { user },
    }).afterClosed().subscribe((result) => {
      if (result) this.load();
    });
  }

  openImportDialog() {
    this.dialog.open(BulkImportDialogComponent, {
      width: '550px',
      data: {
        title: 'Carga Masiva de Dirigentes / Usuarios',
        importUrl: '/api/Users/import',
        templateUrl: '/api/Users/template',
        templateFileName: 'Plantilla_Importacion_Dirigentes.csv'
      }
    }).afterClosed().subscribe((result) => {
      if (result) this.load();
    });
  }

  delete(user: User) {
    if (user.userName === 'admin') {
      this.snackBar.open('No se puede eliminar el administrador', 'Cerrar', {
        duration: 3000,
      });
      return;
    }
    this.dialog.open(ConfirmDialog, {
      data: { title: 'Eliminar usuario', message: `¿Eliminar al usuario "${user.userName}"?` },
    }).afterClosed().subscribe((result) => {
      if (!result) return;
      this.service.delete(user.id).subscribe({
        next: () => {
          this.snackBar.open('Usuario eliminado', 'Cerrar', { duration: 3000 });
          this.load();
        },
        error: () =>
          this.snackBar.open('Error al eliminar usuario', 'Cerrar', {
            duration: 3000,
          }),
      });
    });
  }
}
