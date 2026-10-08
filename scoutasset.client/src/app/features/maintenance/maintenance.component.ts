import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MaintenanceService } from './maintenance.service';
import { Maintenance } from '../../core/models/models';
import { MaintenanceFormComponent } from './maintenance-form.component';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';
import { PromptDialog, PromptField } from '../../shared/prompt-dialog.component';

@Component({
  standalone: false,
  selector: 'app-maintenance',
  templateUrl: './maintenance.component.html',
})
export class MaintenanceComponent implements OnInit {
  displayedColumns = ['resourceCode', 'resourceName', 'type', 'status', 'scheduledDate', 'actions'];
  dataSource = new MatTableDataSource<Maintenance>();
  statusFilter = '';
  overdueList: Maintenance[] = [];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private service: MaintenanceService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe((res) => {
      this.dataSource.data = res || [];
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      const today = new Date();
      this.overdueList = (res || []).filter(
        (m) => m.status === 'PROGRAMADO' && new Date(m.scheduledDate) < today,
      );
      this.cdr.detectChanges();
    });
  }

  applyFilter() {
    this.dataSource.filter = this.statusFilter;
  }

  openForm() {
    this.dialog.open(MaintenanceFormComponent, { width: '600px' })
      .afterClosed().subscribe((result) => {
        if (result) this.load();
      });
  }

  autoSchedule() {
    const fields: PromptField[] = [
      {
        key: 'intervalMonths',
        label: 'Frecuencia de Auto-Programación',
        type: 'select',
        required: true,
        value: 6,
        options: [
          { value: 1, label: 'Cada 1 Mes (Mensual)' },
          { value: 2, label: 'Cada 2 Meses (Bimensual)' },
          { value: 3, label: 'Cada 3 Meses (Trimestral)' },
          { value: 6, label: 'Cada 6 Meses (Semestral)' },
          { value: 12, label: 'Cada 12 Meses (Anual)' }
        ]
      }
    ];

    this.dialog.open(PromptDialog, {
      data: {
        title: 'Auto-Programar Mantenimientos Preventivos',
        fields
      }
    }).afterClosed().subscribe((result) => {
      if (!result || !result.intervalMonths) return;
      const interval = parseInt(result.intervalMonths, 10);
      this.service.autoSchedule(interval).subscribe({
        next: (res) => {
          this.snackBar.open(res.message || `Se auto-programaron ${res.scheduledCount} mantenimientos.`, 'Cerrar', { duration: 4000 });
          this.load();
        },
        error: (err) => {
          this.snackBar.open(err.error?.message || 'Error al auto-programar mantenimientos', 'Cerrar', { duration: 3000 });
        }
      });
    });
  }

  start(m: Maintenance) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Iniciar mantenimiento', message: '¿Iniciar el mantenimiento de este bien?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.start(m.id).subscribe({
          next: () => {
            this.snackBar.open('Mantenimiento iniciado', 'Cerrar', { duration: 3000 });
            this.load();
          },
          error: (err) => {
            this.snackBar.open(err.error?.message || 'Error al iniciar mantenimiento', 'Cerrar', { duration: 3000 });
          }
        });
      });
  }

  complete(m: Maintenance) {
    const fields: PromptField[] = [
      { key: 'cost', label: 'Costo ($)', type: 'number', required: false },
      { key: 'result', label: 'Resultado del Mantenimiento', type: 'textarea', required: false },
      {
        key: 'nextInterval',
        label: 'Programar Próximo Mantenimiento En',
        type: 'select',
        required: false,
        value: 6,
        options: [
          { value: 0, label: 'No programar automático' },
          { value: 1, label: '1 Mes' },
          { value: 3, label: '3 Meses' },
          { value: 6, label: '6 Meses' },
          { value: 12, label: '12 Meses' }
        ]
      }
    ];

    this.dialog.open(PromptDialog, { data: { title: 'Completar Mantenimiento', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        const cost = result.cost ? parseFloat(result.cost) : undefined;
        let nextMaintenanceDate: string | undefined = undefined;

        if (result.nextInterval && parseInt(result.nextInterval, 10) > 0) {
          const months = parseInt(result.nextInterval, 10);
          const nextDate = new Date();
          nextDate.setMonth(nextDate.getMonth() + months);
          nextMaintenanceDate = nextDate.toISOString();
        }

        this.service.complete(m.id, { cost, result: result.result || undefined, nextMaintenanceDate }).subscribe({
          next: () => {
            this.snackBar.open('Mantenimiento completado exitosamente', 'Cerrar', { duration: 3000 });
            this.load();
          },
          error: (err) => {
            this.snackBar.open(err.error?.message || 'Error al completar mantenimiento', 'Cerrar', { duration: 3000 });
          }
        });
      });
  }
}
