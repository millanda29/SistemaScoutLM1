import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ResourcesService } from './resources.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Resource, Category, Location, Dirigente } from '../../core/models/models';

@Component({
  standalone: false,
  templateUrl: './resource-form.component.html',
})
export class ResourceFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  saving = false;

  categories: Category[] = [];
  locations: Location[] = [];
  dirigentes: Dirigente[] = [];
  acquisitionTypes = ['DONACION', 'COMPRA', 'TRANSFERENCIA', 'PROPIO'];
  physicalConditions = ['BUENO', 'REGULAR', 'DANNADO', 'EN_REPARACION'];

  constructor(
    private fb: FormBuilder,
    private service: ResourcesService,
    private dialogRef: MatDialogRef<ResourceFormComponent>,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: { resource?: Resource; categories: Category[]; locations: Location[]; dirigentes?: Dirigente[] },
  ) {
    this.categories = data.categories || [];
    this.locations = data.locations || [];
    this.dirigentes = data.dirigentes || [];
    this.isEdit = !!data.resource;
    this.form = this.fb.group({
      name: ['', Validators.required],
      categoryId: ['', Validators.required],
      locationId: ['', Validators.required],
      currentResponsibleId: [''],
      brand: [''],
      model: [''],
      serialNumber: [''],
      acquisitionType: ['DONACION'],
      acquisitionCost: [null],
      appraisedValue: [null],
      physicalCondition: ['BUENO', Validators.required],
      observations: [''],
    });
  }

  ngOnInit() {
    if (this.isEdit && this.data.resource) {
      this.form.patchValue(this.data.resource);
    }
  }

  save() {
    if (this.form.invalid) return;
    this.saving = true;
    const obs = this.isEdit
      ? this.service.update(this.data.resource!.id, this.form.value)
      : this.service.create(this.form.value);
    obs.subscribe({
      next: () => {
        this.snackBar.open(`Recurso ${this.isEdit ? 'actualizado' : 'creado'}`, 'Cerrar', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: () => { this.saving = false; },
    });
  }

  cancel() {
    this.dialogRef.close();
  }
}
