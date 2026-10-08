import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpClient } from '@angular/common/http';
import { RetirementsService } from './retirements.service';
import { Retirement, Resource } from '../../core/models/models';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';
import { PromptDialog, PromptField } from '../../shared/prompt-dialog.component';

@Component({
  standalone: false,
  selector: 'app-retirements',
  templateUrl: './retirements.component.html',
})
export class RetirementsComponent implements OnInit {
  displayedColumns = ['retirementNumber', 'resourceCode', 'resourceName', 'reason', 'status', 'actions'];
  retirements: Retirement[] = [];
  resources: Resource[] = [];

  constructor(
    private service: RetirementsService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.load();
    this.loadResources();
  }

  load() {
    this.service.getAll().subscribe((res) => (this.retirements = res));
  }

  loadResources() {
    this.http.get<Resource[]>('/api/Resources').subscribe((res) => {
      this.resources = res || [];
    });
  }

  solicitar() {
    if (!this.resources || this.resources.length === 0) {
      this.snackBar.open('Cargando lista de recursos...', 'Cerrar', { duration: 2000 });
      return;
    }

    const resourceOptions = this.resources.map((r) => ({
      value: r.id,
      label: `${r.code} - ${r.name}`,
    }));

    const fields: PromptField[] = [
      {
        key: 'resourceId',
        label: 'Seleccionar Recurso',
        type: 'select',
        required: true,
        options: resourceOptions,
      },
      { key: 'reason', label: 'Motivo de baja', type: 'textarea', required: true },
    ];

    this.dialog.open(PromptDialog, { data: { title: 'Solicitar Baja de Recurso', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.create({ resourceId: +result.resourceId, reason: result.reason }).subscribe({
          next: () => {
            this.snackBar.open('Solicitud de baja registrada exitosamente', 'Cerrar', { duration: 3000 });
            this.load();
          },
          error: (err) => {
            console.error('Error al solicitar la baja:', err);
            this.snackBar.open(err.error?.message || 'Error al solicitar la baja', 'Cerrar', { duration: 3000 });
          }
        });
      });
  }

  review(r: Retirement) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Revisar solicitud', message: '¿Revisar y aprobar esta solicitud?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.review(r.id, {}).subscribe({
          next: () => {
            this.snackBar.open('Solicitud revisada', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }

  authorize(r: Retirement) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Autorizar baja', message: '¿Autorizar baja?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.authorize(r.id).subscribe({
          next: () => {
            this.snackBar.open('Baja autorizada', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }

  execute(r: Retirement) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Ejecutar baja', message: '¿Ejecutar baja?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.execute(r.id).subscribe({
          next: () => {
            this.snackBar.open('Baja ejecutada', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }

  reject(r: Retirement) {
    const fields: PromptField[] = [
      { key: 'rejectionReason', label: 'Motivo de rechazo', type: 'textarea', required: true },
    ];
    this.dialog.open(PromptDialog, { data: { title: 'Rechazar solicitud', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.reject(r.id, { rejectionReason: result.rejectionReason }).subscribe({
          next: () => {
            this.snackBar.open('Solicitud rechazada', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }
}
