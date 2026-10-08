import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MaintenanceService } from './maintenance.service';
import { HttpClient } from '@angular/common/http';
import { Resource } from '../../core/models/models';

@Component({
  standalone: false,
  templateUrl: './maintenance-form.component.html',
})
export class MaintenanceFormComponent implements OnInit {
  form: FormGroup;
  resources: Resource[] = [];
  types = ['PREVENTIVO', 'CORRECTIVO'];
  saving = false;

  constructor(
    private fb: FormBuilder,
    private service: MaintenanceService,
    private dialogRef: MatDialogRef<MaintenanceFormComponent>,
    private snackBar: MatSnackBar,
    private http: HttpClient,
  ) {
    this.form = this.fb.group({
      resourceId: ['', Validators.required],
      type: ['PREVENTIVO', Validators.required],
      description: ['', Validators.required],
      scheduledDate: ['', Validators.required],
      responsible: [''],
    });
  }

  ngOnInit() {
    this.http.get<Resource[]>('/api/Resources').subscribe((res) => (this.resources = res));
  }

  save() {
    if (this.form.invalid) return;
    this.saving = true;
    this.service.create(this.form.value).subscribe({
      next: () => {
        this.snackBar.open('Mantenimiento programado', 'Cerrar', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: () => { this.saving = false; },
    });
  }

  cancel() {
    this.dialogRef.close();
  }
}
