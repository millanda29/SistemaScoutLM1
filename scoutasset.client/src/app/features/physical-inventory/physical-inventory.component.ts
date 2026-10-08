import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpClient } from '@angular/common/http';
import { PhysicalInventoryService } from './physical-inventory.service';
import { PhysicalInventory, Location, Resource } from '../../core/models/models';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';
import { PromptDialog, PromptField } from '../../shared/prompt-dialog.component';

@Component({
  standalone: false,
  selector: 'app-physical-inventory',
  templateUrl: './physical-inventory.component.html',
})
export class PhysicalInventoryComponent implements OnInit {
  inventories: PhysicalInventory[] = [];
  locations: Location[] = [];
  resources: Resource[] = [];
  expandedId: number | null = null;
  processingId: number | null = null;

  constructor(
    private service: PhysicalInventoryService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.load();
    this.loadLocations();
    this.loadResources();
  }

  load() {
    this.service.getAll().subscribe((res) => {
      this.inventories = res || [];
      this.cdr.detectChanges();
    });
  }

  loadLocations() {
    this.http.get<Location[]>('/api/Locations').subscribe((res) => {
      this.locations = res || [];
      this.cdr.detectChanges();
    });
  }

  loadResources() {
    this.http.get<Resource[]>('/api/Resources').subscribe((res) => {
      this.resources = res || [];
      this.cdr.detectChanges();
    });
  }

  create() {
    const locationOptions = this.locations.map(l => ({ value: l.id, label: l.name }));

    const fields: PromptField[] = [
      { key: 'description', label: 'Descripción del inventario', type: 'text', required: true },
      {
        key: 'locationIds',
        label: 'Ubicaciones a Auditar',
        type: 'multiselect',
        required: true,
        options: locationOptions,
        value: this.locations.map(l => l.id)
      }
    ];

    this.dialog.open(PromptDialog, { data: { title: 'Nuevo Inventario Físico', fields } })
      .afterClosed().subscribe((result) => {
        if (!result || !result.locationIds || result.locationIds.length === 0) return;
        
        const payload = {
          description: result.description || '',
          startDate: new Date().toISOString(),
          locationIds: result.locationIds
        };

        this.service.create(payload).subscribe({
          next: () => {
            this.snackBar.open('Inventario físico creado exitosamente', 'Cerrar', { duration: 3000 });
            this.load();
          },
          error: (err) => {
            console.error('Error creating physical inventory:', err);
            this.snackBar.open(err.error?.message || 'Error al crear el inventario', 'Cerrar', { duration: 3000 });
          }
        });
      });
  }

  start(inv: PhysicalInventory) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Iniciar inventario', message: '¿Iniciar el proceso de conteo de inventario?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        
        this.processingId = inv.id;
        inv.status = 'EN_CURSO'; // Optimistic status update
        this.cdr.detectChanges();

        this.service.start(inv.id).subscribe({
          next: () => {
            this.processingId = null;
            this.snackBar.open('Inventario iniciado. Ya puedes registrar los hallazgos.', 'Cerrar', { duration: 3000 });
            this.load();
          },
          error: (err) => {
            this.processingId = null;
            inv.status = 'PLANIFICADA';
            this.cdr.detectChanges();
            this.snackBar.open(err.error?.message || 'Error al iniciar inventario', 'Cerrar', { duration: 3000 });
          }
        });
      });
  }

  registerItem(inv: PhysicalInventory) {
    const resourceOptions = this.resources.map(r => ({ value: r.id, label: `${r.code} - ${r.name}` }));
    
    // Assigned locations for this inventory
    const assignedLocIds = inv.locations ? inv.locations.map(l => l.locationId) : [];
    const assignedLocs = this.locations.filter(l => assignedLocIds.includes(l.id));
    const locationOptions = (assignedLocs.length > 0 ? assignedLocs : this.locations).map(l => ({ value: l.id, label: l.name }));

    const fields: PromptField[] = [
      { key: 'resourceId', label: 'Recurso Encontrado', type: 'select', required: true, options: resourceOptions },
      { key: 'locationId', label: 'Ubicación del Conteo', type: 'select', required: true, options: locationOptions },
      {
        key: 'result', label: 'Resultado', type: 'select', required: true,
        options: [
          { value: 'COINCIDIENTE', label: 'Coincidente (Existe)' },
          { value: 'SOBRANTE', label: 'Sobrante (No Estaba Registrado)' },
          { value: 'FALTANTE', label: 'Faltante (No Encontrado)' }
        ],
        value: 'COINCIDIENTE'
      },
      {
        key: 'physicalCondition', label: 'Condición Física', type: 'select', required: true,
        options: [
          { value: 'BUENO', label: 'Bueno' },
          { value: 'REGULAR', label: 'Regular' },
          { value: 'DANIADO', label: 'Dañado' }
        ],
        value: 'BUENO'
      },
      { key: 'observations', label: 'Observaciones', type: 'textarea', required: false }
    ];

    this.dialog.open(PromptDialog, { data: { title: 'Registrar Ítem en Inventario', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        
        this.processingId = inv.id;
        this.cdr.detectChanges();

        const payload = {
          locationId: +result.locationId,
          resourceId: +result.resourceId,
          result: result.result,
          physicalCondition: result.physicalCondition,
          observations: result.observations || ''
        };

        this.service.registerItem(inv.id, payload).subscribe({
          next: () => {
            this.processingId = null;
            this.snackBar.open('Ítem registrado en el inventario', 'Cerrar', { duration: 3000 });
            this.load();
          },
          error: (err) => {
            console.error('Error registering item:', err);
            this.processingId = null;
            this.cdr.detectChanges();
            this.snackBar.open(err.error?.message || 'Error al registrar ítem', 'Cerrar', { duration: 3000 });
          }
        });
      });
  }

  finish(inv: PhysicalInventory) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Finalizar conteo', message: '¿Finalizar el conteo de ítems para este inventario?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;

        this.processingId = inv.id;
        inv.status = 'FINALIZADA'; // Optimistic status update
        this.cdr.detectChanges();

        this.service.finish(inv.id).subscribe({
          next: () => {
            this.processingId = null;
            this.snackBar.open('Conteo finalizado. Ya puedes ejecutar la conciliación.', 'Cerrar', { duration: 3000 });
            this.load();
          },
          error: (err) => {
            this.processingId = null;
            inv.status = 'EN_CURSO';
            this.cdr.detectChanges();
            this.snackBar.open(err.error?.message || 'Error al finalizar inventario', 'Cerrar', { duration: 3000 });
          }
        });
      });
  }

  reconcile(inv: PhysicalInventory) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Ejecutar Conciliación', message: '¿Generar conciliación y actualizar estados de bienes?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;

        this.processingId = inv.id;
        inv.status = 'CONCILIADA'; // Optimistic status update
        this.cdr.detectChanges();

        this.service.reconcile(inv.id).subscribe({
          next: () => {
            this.processingId = null;
            this.snackBar.open('Inventario conciliado exitosamente', 'Cerrar', { duration: 3000 });
            this.load();
          },
          error: (err) => {
            this.processingId = null;
            inv.status = 'FINALIZADA';
            this.cdr.detectChanges();
            this.snackBar.open(err.error?.message || 'Error al conciliar inventario', 'Cerrar', { duration: 3000 });
          }
        });
      });
  }

  toggleDetails(id: number) {
    this.expandedId = this.expandedId === id ? null : id;
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'PLANIFICADA': return 'Planificada';
      case 'EN_CURSO': return 'En Curso (Conteo)';
      case 'FINALIZADA': return 'Finalizada';
      case 'CONCILIADA': return 'Conciliada (Completado)';
      default: return status;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'PLANIFICADA': return 'stat-mantenimiento';
      case 'EN_CURSO': return 'stat-prestado';
      case 'FINALIZADA': return 'role-superintendente';
      case 'CONCILIADA': return 'stat-disponible';
      default: return '';
    }
  }
}
