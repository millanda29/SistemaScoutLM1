import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoansService } from './loans.service';
import { HttpClient } from '@angular/common/http';
import { Resource } from '../../core/models/models';

@Component({
  standalone: false,
  templateUrl: './loan-form.component.html',
  styles: [`
    .row {
      display: flex;
      gap: 12px;
      width: 100%;
    }
    .col {
      flex: 1;
    }
    .w-full {
      width: 100%;
    }
  `]
})
export class LoanFormComponent implements OnInit {
  form: FormGroup;
  resources: Resource[] = [];
  saving = false;
  uploadingPdf = false;
  pdfPath: string | null = null;

  constructor(
    private fb: FormBuilder,
    private service: LoansService,
    private dialogRef: MatDialogRef<LoanFormComponent>,
    private snackBar: MatSnackBar,
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      resourceIds: [[], Validators.required],
      reason: ['', Validators.required],
      requesterType: ['INTERNO', Validators.required],
      loanDate: [new Date(), Validators.required],
      expectedReturnDate: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.http.get<Resource[]>('/api/Resources').subscribe((res) => {
      this.resources = (res || []).filter(r => 
        (r.administrativeStatus === 'DISPONIBLE' || r.administrativeStatus === 'ASIGNADO') &&
        r.physicalCondition !== 'DANIADO' &&
        r.physicalCondition !== 'DANNADO' &&
        r.physicalCondition !== 'DAÑADO' &&
        r.physicalCondition !== 'EN_REPARACION'
      );
      this.cdr.detectChanges();
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    if (file.type !== 'application/pdf') {
      this.snackBar.open('Solo se permiten archivos en formato PDF', 'Cerrar', { duration: 3000 });
      event.target.value = '';
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    this.uploadingPdf = true;
    this.cdr.detectChanges();

    this.http.post<{ filePath: string }>('/api/Loans/upload', formData).subscribe({
      next: (res) => {
        this.pdfPath = res.filePath || (res as any).FilePath;
        this.uploadingPdf = false;
        this.cdr.detectChanges();
        this.snackBar.open('Documento de respaldo cargado con éxito', 'Cerrar', { duration: 3000 });
      },
      error: (err) => {
        console.error(err);
        this.uploadingPdf = false;
        this.cdr.detectChanges();
        this.snackBar.open('Error al subir el documento PDF', 'Cerrar', { duration: 3000 });
        event.target.value = '';
      }
    });
  }

  save() {
    if (this.form.invalid) return;
    this.saving = true;

    const payload = {
      ...this.form.value,
      pdfDocumentPath: this.pdfPath
    };

    this.service.create(payload).subscribe({
      next: () => {
        this.snackBar.open('Préstamo registrado exitosamente', 'Cerrar', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err) => {
        console.error('Error creating loan:', err);
        this.snackBar.open(err.error?.message || 'Error al registrar el préstamo', 'Cerrar', { duration: 3000 });
        this.saving = false;
      },
    });
  }

  cancel() {
    this.dialogRef.close();
  }
}
