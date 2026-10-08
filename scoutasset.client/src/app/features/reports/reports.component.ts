import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ReportsService } from './reports.service';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../core/auth/auth.service';
import { Resource, Loan, Maintenance, Movement, Category, Location } from '../../core/models/models';

@Component({
  standalone: false,
  selector: 'app-reports',
  templateUrl: './reports.component.html',
})
export class ReportsComponent implements OnInit {
  activeTabIndex = 0;

  // Filters
  selectedCategory = '';
  selectedLocation = '';
  selectedStatus = '';
  categories: Category[] = [];
  locations: Location[] = [];

  // Summary Metrics
  totalResourcesCount = 0;
  totalAcquisitionCost = 0;
  totalAppraisedValue = 0;
  totalBorrowedCount = 0;

  // Data Sources
  inventoryColumns = ['code', 'name', 'category', 'location', 'physicalCondition', 'administrativeStatus', 'acquisitionCost', 'appraisedValue'];
  inventorySource = new MatTableDataSource<any>();

  categoryColumns = ['category', 'total', 'disponibles', 'prestados', 'enMantenimiento'];
  categorySource = new MatTableDataSource<any>();

  activeLoanColumns = ['requestNumber', 'requesterName', 'reason', 'expectedReturnDate', 'resourceNames'];
  activeLoanSource = new MatTableDataSource<any>();

  overdueLoanColumns = ['requestNumber', 'requesterName', 'reason', 'expectedReturnDate', 'daysOverdue', 'resourceNames'];
  overdueLoanSource = new MatTableDataSource<any>();

  maintenanceColumns = ['resourceCode', 'resourceName', 'type', 'status', 'scheduledDate', 'description'];
  maintenanceSource = new MatTableDataSource<any>();

  movementColumns = ['resourceCode', 'resourceName', 'type', 'description', 'previousStatus', 'newStatus', 'performedAt'];
  movementSource = new MatTableDataSource<any>();

  startDate = '';
  endDate = '';
  rawInventoryList: any[] = [];

  canExport = true;

  @ViewChild('inventoryPaginator') inventoryPaginator!: MatPaginator;
  @ViewChild('inventorySort') inventorySort!: MatSort;

  constructor(
    private service: ReportsService,
    private http: HttpClient,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.canExport = this.auth.hasRole('ADMIN') || this.auth.hasRole('SUPERINTENDENTE') || this.auth.hasRole('JEFE_GRUPO');
    this.http.get<Category[]>('/api/Categories').subscribe((res) => (this.categories = res || []));
    this.http.get<Location[]>('/api/Locations').subscribe((res) => (this.locations = res || []));

    this.loadInventory();
    this.loadByCategory();
    this.loadActiveLoans();
    this.loadOverdueLoans();
    this.loadPendingMaintenance();
    this.loadMovements();
  }

  loadInventory() {
    this.service.getInventory().subscribe((res: any[]) => {
      this.rawInventoryList = res || [];
      this.calculateSummary(this.rawInventoryList);
      this.applyInventoryFilter();
    });
  }

  calculateSummary(list: any[]) {
    this.totalResourcesCount = list.length;
    this.totalAcquisitionCost = list.reduce((sum, item) => sum + (item.acquisitionCost || 0), 0);
    this.totalAppraisedValue = list.reduce((sum, item) => sum + (item.appraisedValue || 0), 0);
    this.totalBorrowedCount = list.filter((item) => item.administrativeStatus === 'PRESTADO').length;
  }

  applyInventoryFilter() {
    let filtered = [...this.rawInventoryList];

    if (this.selectedCategory) {
      filtered = filtered.filter(item => item.category === this.selectedCategory || item.categoryId === +this.selectedCategory);
    }
    if (this.selectedLocation) {
      filtered = filtered.filter(item => item.location === this.selectedLocation || item.locationId === +this.selectedLocation);
    }
    if (this.selectedStatus) {
      filtered = filtered.filter(item => item.administrativeStatus === this.selectedStatus);
    }

    this.inventorySource.data = filtered;
    this.inventorySource.paginator = this.inventoryPaginator;
    this.inventorySource.sort = this.inventorySort;
  }

  loadByCategory() {
    this.service.getByCategory().subscribe((res) => (this.categorySource.data = res || []));
  }

  loadActiveLoans() {
    this.service.getActiveLoans().subscribe((res) => (this.activeLoanSource.data = res || []));
  }

  loadOverdueLoans() {
    this.service.getOverdueLoans().subscribe((res) => (this.overdueLoanSource.data = res || []));
  }

  loadPendingMaintenance() {
    this.service.getPendingMaintenance().subscribe((res) => (this.maintenanceSource.data = res || []));
  }

  loadMovements() {
    this.service.getMovements(this.startDate || undefined, this.endDate || undefined).subscribe((res) => (this.movementSource.data = res || []));
  }

  exportCurrentTabToCsv() {
    let filename = 'Reporte_ScoutAsset.csv';
    let data: any[] = [];

    switch (this.activeTabIndex) {
      case 0:
        filename = 'Reporte_Inventario_General.csv';
        data = this.inventorySource.data.map(item => ({
          'Código': item.code,
          'Nombre': item.name,
          'Categoría': item.category,
          'Ubicación': item.location,
          'Condición Física': item.physicalCondition,
          'Estado Administrativo': item.administrativeStatus,
          'Costo Adquisición ($)': item.acquisitionCost || 0,
          'Valor Avalúo ($)': item.appraisedValue || 0
        }));
        break;
      case 1:
        filename = 'Reporte_Por_Categoria.csv';
        data = this.categorySource.data.map(item => ({
          'Categoría': item.category,
          'Total Bienes': item.total,
          'Disponibles': item.disponibles,
          'Prestados': item.prestados,
          'En Mantenimiento': item.enMantenimiento
        }));
        break;
      case 2:
        filename = 'Reporte_Prestamos_Activos.csv';
        data = this.activeLoanSource.data.map(item => ({
          'Solicitud #': item.requestNumber,
          'Solicitante': item.requesterName || item.requesterId,
          'Motivo': item.reason,
          'Fecha Devolución Estimada': item.expectedReturnDate,
          'Equipos Prestados': item.resourceNames
        }));
        break;
      case 3:
        filename = 'Reporte_Prestamos_Vencidos.csv';
        data = this.overdueLoanSource.data.map(item => ({
          'Solicitud #': item.requestNumber,
          'Solicitante': item.requesterName || item.requesterId,
          'Motivo': item.reason,
          'Fecha Devolución Estimada': item.expectedReturnDate,
          'Días Vencido': item.daysOverdue,
          'Equipos Prestados': item.resourceNames
        }));
        break;
      case 4:
        filename = 'Reporte_Mantenimientos_Pendientes.csv';
        data = this.maintenanceSource.data.map(item => ({
          'Código Recurso': item.resourceCode,
          'Nombre Recurso': item.resourceName,
          'Tipo Mantenimiento': item.type,
          'Estado': item.status,
          'Fecha Programada': item.scheduledDate,
          'Descripción': item.description
        }));
        break;
      case 5:
        filename = 'Reporte_Historial_Movimientos.csv';
        data = this.movementSource.data.map(item => ({
          'Código Recurso': item.resourceCode,
          'Nombre Recurso': item.resourceName,
          'Tipo Movimiento': item.type,
          'Descripción': item.description,
          'Estado Previo': item.previousStatus,
          'Nuevo Estado': item.newStatus,
          'Fecha': item.performedAt
        }));
        break;
    }

    if (data.length === 0) return;

    const headers = Object.keys(data[0]);
    const csvRows = [headers.join(',')];

    for (const row of data) {
      const values = headers.map(header => {
        const escaped = ('' + (row[header] ?? '')).replace(/"/g, '\\"');
        return `"${escaped}"`;
      });
      csvRows.push(values.join(','));
    }

    const csvString = '\uFEFF' + csvRows.join('\n'); // UTF-8 BOM
    const blob = new Blob([csvString], { type: 'text/csv;charset=utf-8;' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
  }

  printReport() {
    window.print();
  }
}
