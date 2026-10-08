import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ResourcesService } from './resources.service';
import { Resource, Category, Location, Dirigente } from '../../core/models/models';
import { HttpClient } from '@angular/common/http';
import { ResourceFormComponent } from './resource-form.component';
import { BulkImportDialogComponent } from '../../shared/bulk-import-dialog.component';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';

@Component({
  standalone: false,
  selector: 'app-resources',
  templateUrl: './resources.component.html',
})
export class ResourcesComponent implements OnInit {
  displayedColumns = ['code', 'name', 'category', 'location', 'responsible', 'physicalCondition', 'administrativeStatus', 'actions'];
  dataSource = new MatTableDataSource<any>();
  categories: Category[] = [];
  locations: Location[] = [];
  dirigentes: Dirigente[] = [];
  dirigenteMap = new Map<string, string>();
  search = '';
  selectedCategory = '';
  selectedStatus = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private service: ResourcesService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private http: HttpClient,
  ) {}

  ngOnInit() {
    this.http.get<Category[]>('/api/Categories').subscribe((r) => (this.categories = r || []));
    this.http.get<Location[]>('/api/Locations').subscribe((r) => (this.locations = r || []));
    this.http.get<Dirigente[]>('/api/Dirigentes').subscribe((r) => {
      this.dirigentes = r || [];
      this.dirigenteMap.clear();
      for (const d of this.dirigentes) {
        this.dirigenteMap.set(d.id.toString(), `${d.nombres} ${d.apellidos}`);
      }
      this.load();
    });
  }

  load() {
    this.service.getAll().subscribe((res: any[]) => {
      const mapped = (res || []).map((r) => ({
        ...r,
        responsibleName: r.currentResponsibleId && this.dirigenteMap.has(r.currentResponsibleId)
          ? this.dirigenteMap.get(r.currentResponsibleId)
          : (r.currentResponsibleId ? `ID: ${r.currentResponsibleId}` : 'Sin Custodio')
      }));
      this.dataSource.data = mapped;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  applyFilter() {
    this.dataSource.filterPredicate = (data: any, filter: string) => {
      const matchSearch = !this.search || data.name.toLowerCase().includes(this.search.toLowerCase()) || data.code.toLowerCase().includes(this.search.toLowerCase());
      const matchCat = !this.selectedCategory || data.categoryId === +this.selectedCategory;
      const matchStatus = !this.selectedStatus || data.administrativeStatus === this.selectedStatus;
      return matchSearch && matchCat && matchStatus;
    };
    this.dataSource.filter = Math.random().toString();
  }

  openForm(resource?: Resource) {
    this.dialog.open(ResourceFormComponent, {
      width: '640px',
      data: { resource, categories: this.categories, locations: this.locations, dirigentes: this.dirigentes },
    }).afterClosed().subscribe((result) => {
      if (result) this.load();
    });
  }

  openImportDialog() {
    this.dialog.open(BulkImportDialogComponent, {
      width: '550px',
      data: {
        title: 'Carga Masiva de Recursos',
        importUrl: '/api/Resources/import',
        templateUrl: '/api/Resources/template',
        templateFileName: 'Plantilla_Importacion_Recursos.csv'
      }
    }).afterClosed().subscribe((result) => {
      if (result) this.load();
    });
  }

  delete(id: number) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Eliminar recurso', message: '¿Eliminar este recurso?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.delete(id).subscribe(() => {
          this.snackBar.open('Recurso eliminado', 'Cerrar', { duration: 3000 });
          this.load();
        });
      });
  }
}
