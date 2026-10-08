import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LocationsService } from './locations.service';
import { Location } from '../../core/models/models';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';
import { PromptDialog, PromptField } from '../../shared/prompt-dialog.component';
import { BulkImportDialogComponent } from '../../shared/bulk-import-dialog.component';

@Component({
  standalone: false,
  selector: 'app-locations',
  templateUrl: './locations.component.html',
})
export class LocationsComponent implements OnInit {
  displayedColumns = ['name', 'description', 'actions'];
  dataSource = new MatTableDataSource<Location>();
  editingId: number | null = null;
  editName = '';
  editDescription = '';

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private service: LocationsService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
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

  startEdit(loc: Location) {
    this.editingId = loc.id;
    this.editName = loc.name;
    this.editDescription = loc.description || '';
  }

  saveEdit(loc: Location) {
    if (!this.editName.trim()) return;
    this.service.update(loc.id, { name: this.editName, description: this.editDescription }).subscribe(() => {
      this.snackBar.open('Ubicación actualizada', 'Cerrar', { duration: 3000 });
      this.editingId = null;
      this.load();
    });
  }

  cancelEdit() {
    this.editingId = null;
  }

  create() {
    const fields: PromptField[] = [
      { key: 'name', label: 'Nombre de la ubicación', type: 'text', required: true },
      { key: 'description', label: 'Descripción', type: 'text', required: false },
    ];
    this.dialog.open(PromptDialog, { data: { title: 'Nueva Ubicación', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.create({ name: result.name, description: result.description || '' }).subscribe(() => {
          this.snackBar.open('Ubicación creada', 'Cerrar', { duration: 3000 });
          this.load();
        });
      });
  }

  openImportDialog() {
    this.dialog.open(BulkImportDialogComponent, {
      width: '550px',
      data: {
        title: 'Carga Masiva de Ubicaciones',
        importUrl: '/api/Locations/import',
        templateUrl: '/api/Locations/template',
        templateFileName: 'Plantilla_Importacion_Ubicaciones.csv'
      }
    }).afterClosed().subscribe((result) => {
      if (result) this.load();
    });
  }

  delete(id: number) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Eliminar ubicación', message: '¿Eliminar esta ubicación?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.delete(id).subscribe(() => {
          this.snackBar.open('Ubicación eliminada', 'Cerrar', { duration: 3000 });
          this.load();
        });
      });
  }
}
