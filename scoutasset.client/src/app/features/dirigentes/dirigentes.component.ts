import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DirigentesService, Dirigente } from './dirigentes.service';
import { DirigenteFormComponent } from './dirigente-form.component';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';
import { BulkImportDialogComponent } from '../../shared/bulk-import-dialog.component';

@Component({
  standalone: false,
  selector: 'app-dirigentes',
  templateUrl: './dirigentes.component.html',
})
export class DirigentesComponent implements OnInit {
  displayedColumns = ['ci', 'fullName', 'correo', 'telefono', 'habilitadoParaCustodio', 'actions'];
  dataSource = new MatTableDataSource<Dirigente>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private service: DirigentesService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
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

  openForm(dirigente?: Dirigente) {
    this.dialog.open(DirigenteFormComponent, {
      width: '500px',
      data: { dirigente }
    }).afterClosed().subscribe((result) => {
      if (!result) return;

      if (dirigente) {
        this.service.update(dirigente.id, result).subscribe(() => {
          this.snackBar.open('Dirigente actualizado exitosamente', 'Cerrar', { duration: 3000 });
          this.load();
        });
      } else {
        this.service.create(result).subscribe(() => {
          this.snackBar.open('Dirigente creado exitosamente', 'Cerrar', { duration: 3000 });
          this.load();
        });
      }
    });
  }

  openImportDialog() {
    this.dialog.open(BulkImportDialogComponent, {
      width: '550px',
      data: {
        title: 'Carga Masiva de Dirigentes',
        importUrl: '/api/Dirigentes/import',
        templateUrl: '/api/Dirigentes/template',
        templateFileName: 'Plantilla_Importacion_Dirigentes.csv'
      }
    }).afterClosed().subscribe((result) => {
      if (result) this.load();
    });
  }

  delete(id: number) {
    this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Eliminar Dirigente',
        message: '¿Está seguro de que desea eliminar este dirigente? Esta acción no se puede deshacer.'
      }
    }).afterClosed().subscribe((result) => {
      if (!result) return;

      this.service.delete(id).subscribe(() => {
        this.snackBar.open('Dirigente eliminado', 'Cerrar', { duration: 3000 });
        this.load();
      });
    });
  }
}
