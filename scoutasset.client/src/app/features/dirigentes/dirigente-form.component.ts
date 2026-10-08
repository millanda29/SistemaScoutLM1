import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Dirigente } from './dirigentes.service';

@Component({
  standalone: false,
  selector: 'app-dirigente-form',
  template: `
    <h2 mat-dialog-title>{{ data.dirigente ? 'Editar Dirigente' : 'Nuevo Dirigente' }}</h2>
    <form [formGroup]="form" (ngSubmit)="submit()">
      <mat-dialog-content>
        <div class="form-grid" style="padding-top: 8px;">
          <mat-form-field appearance="outline">
            <mat-label>Cédula de Identidad (CI)</mat-label>
            <input matInput formControlName="ci" placeholder="Ej. 1726543210" required />
            <mat-error *ngIf="form.get('ci')?.hasError('required')">CI es requerido</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Teléfono / Celular</mat-label>
            <input matInput formControlName="telefono" required />
            <mat-error *ngIf="form.get('telefono')?.hasError('required')">Teléfono es requerido</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Nombres</mat-label>
            <input matInput formControlName="nombres" required />
            <mat-error *ngIf="form.get('nombres')?.hasError('required')">Nombres son requeridos</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Apellidos</mat-label>
            <input matInput formControlName="apellidos" required />
            <mat-error *ngIf="form.get('apellidos')?.hasError('required')">Apellidos son requeridos</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="grid-span-2">
            <mat-label>Correo Electrónico</mat-label>
            <input matInput type="email" formControlName="correo" required />
            <mat-error *ngIf="form.get('correo')?.hasError('required')">Correo es requerido</mat-error>
            <mat-error *ngIf="form.get('correo')?.hasError('email')">Correo no es válido</mat-error>
          </mat-form-field>

          <div class="grid-span-2" style="margin: 10px 0;">
            <mat-checkbox formControlName="habilitadoParaCustodio" color="primary">
              Habilitado como Custodio
            </mat-checkbox>
          </div>
        </div>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" mat-dialog-close>Cancelar</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">Guardar</button>
      </mat-dialog-actions>
    </form>
  `
})
export class DirigenteFormComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<DirigenteFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { dirigente?: Dirigente }
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      ci: [this.data.dirigente?.ci || '', Validators.required],
      nombres: [this.data.dirigente?.nombres || '', Validators.required],
      apellidos: [this.data.dirigente?.apellidos || '', Validators.required],
      correo: [this.data.dirigente?.correo || '', [Validators.required, Validators.email]],
      telefono: [this.data.dirigente?.telefono || '', Validators.required],
      habilitadoParaCustodio: [this.data.dirigente?.habilitadoParaCustodio !== false]
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.dialogRef.close(this.form.value);
  }
}
