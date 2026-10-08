import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoriesService } from './categories.service';
import { Category } from '../../core/models/models';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';
import { PromptDialog, PromptField } from '../../shared/prompt-dialog.component';
import { BulkImportDialogComponent } from '../../shared/bulk-import-dialog.component';

@Component({
  standalone: false,
  selector: 'app-categories',
  templateUrl: './categories.component.html',
})
export class CategoriesComponent implements OnInit {
  displayedColumns = ['name', 'prefix', 'actions'];
  dataSource = new MatTableDataSource<Category>();
  editingId: number | null = null;
  editName = '';
  editPrefix = '';

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private service: CategoriesService,
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

  startEdit(cat: Category) {
    this.editingId = cat.id;
    this.editName = cat.name;
    this.editPrefix = cat.prefix;
  }

  saveEdit(cat: Category) {
    if (!this.editName.trim() || !this.editPrefix.trim()) return;
    this.service.update(cat.id, { name: this.editName, prefix: this.editPrefix }).subscribe(() => {
      this.snackBar.open('Categoría actualizada', 'Cerrar', { duration: 3000 });
      this.editingId = null;
      this.load();
    });
  }

  cancelEdit() {
    this.editingId = null;
  }

  create() {
    const fields: PromptField[] = [
      { key: 'name', label: 'Nombre de la categoría', type: 'text', required: true },
      { key: 'prefix', label: 'Prefijo', type: 'text', required: true },
    ];
    this.dialog.open(PromptDialog, { data: { title: 'Nueva Categoría', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.create({ name: result.name, prefix: result.prefix }).subscribe(() => {
          this.snackBar.open('Categoría creada', 'Cerrar', { duration: 3000 });
          this.load();
        });
      });
  }

  openImportDialog() {
    this.dialog.open(BulkImportDialogComponent, {
      width: '550px',
      data: {
        title: 'Carga Masiva de Categorías',
        importUrl: '/api/Categories/import',
        templateUrl: '/api/Categories/template',
        templateFileName: 'Plantilla_Importacion_Categorias.csv'
      }
    }).afterClosed().subscribe((result) => {
      if (result) this.load();
    });
  }

  delete(id: number) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Eliminar categoría', message: '¿Eliminar esta categoría?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.delete(id).subscribe(() => {
          this.snackBar.open('Categoría eliminada', 'Cerrar', { duration: 3000 });
          this.load();
        });
      });
  }
}
