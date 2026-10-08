import { Component, Inject, Optional, ChangeDetectorRef } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

export interface BulkImportDialogData {
  title?: string;
  importUrl?: string;
  templateUrl?: string;
  templateFileName?: string;
}

export interface ImportResult {
  totalRows: number;
  successCount: number;
  errorCount: number;
  errors: string[];
  successMessages: string[];
}

@Component({
  standalone: false,
  selector: 'app-bulk-import-dialog',
  template: `
    <h2 mat-dialog-title style="display: flex; align-items: center; justify-content: space-between; font-weight: 700; color: #12053d;">
      <span><mat-icon style="vertical-align: middle; margin-right: 8px; color: #7c4dff;">table_chart</mat-icon> {{ title }}</span>
      <button mat-icon-button (click)="close()"><mat-icon>close</mat-icon></button>
    </h2>

    <mat-dialog-content>
      <div style="padding-top: 8px;">
        <!-- Step 1: Download Template -->
        <div style="background: #f4effc; border: 1px dashed #7c4dff; border-radius: 12px; padding: 16px; margin-bottom: 20px; display: flex; align-items: center; justify-content: space-between;">
          <div>
            <strong style="color: #2c1e4d; display: block; font-size: 0.95rem;">Plantilla para Hoja de Cálculo</strong>
            <span style="font-size: 0.85rem; color: #666;">Descarga el formato sugerido con las columnas requeridas para importar.</span>
          </div>
          <button mat-stroked-button color="primary" (click)="downloadTemplate()">
            <mat-icon>download</mat-icon> Descargar Plantilla
          </button>
        </div>

        <!-- Step 2: File Selector -->
        <div *ngIf="!importResult && !uploading" style="border: 2px dashed #ccc; border-radius: 12px; padding: 32px; text-align: center; background: #fafafa; transition: border-color 0.2s;" [style.border-color]="selectedFile ? '#7c4dff' : '#ccc'">
          <input type="file" #fileInput (change)="onFileSelected($event)" accept=".xlsx,.xls,.csv" style="display: none;" />
          
          <mat-icon style="font-size: 48px; width: 48px; height: 48px; color: #7c4dff; margin-bottom: 12px;">cloud_upload</mat-icon>
          
          <div *ngIf="!selectedFile">
            <p style="font-weight: 600; margin-bottom: 4px;">Selecciona tu archivo Excel o CSV</p>
            <p style="font-size: 0.85rem; color: #888; margin-bottom: 16px;">Soporta archivos .xlsx, .xls y .csv</p>
            <button mat-raised-button color="primary" (click)="fileInput.click()">
              <mat-icon>folder_open</mat-icon> Seleccionar Archivo
            </button>
          </div>

          <div *ngIf="selectedFile" style="display: flex; align-items: center; justify-content: center; gap: 12px;">
            <mat-icon style="color: #2e7d32;">insert_drive_file</mat-icon>
            <strong style="font-size: 0.95rem;">{{ selectedFile.name }}</strong>
            <span style="font-size: 0.8rem; color: #666;">({{ (selectedFile.size / 1024).toFixed(1) }} KB)</span>
            <button mat-icon-button color="warn" (click)="removeFile()"><mat-icon>delete</mat-icon></button>
          </div>
        </div>

        <!-- Importing Progress Spinner -->
        <div *ngIf="uploading" style="text-align: center; padding: 32px 0;">
          <mat-spinner diameter="40" style="margin: 0 auto 16px;"></mat-spinner>
          <p style="font-weight: 600; color: #333;">Procesando e importando hoja de cálculo...</p>
        </div>

        <!-- Step 3: Import Results Summary -->
        <div *ngIf="importResult" style="padding-top: 8px;">
          <div style="display: flex; gap: 12px; margin-bottom: 16px;">
            <div style="flex: 1; background: #e8f5e9; border: 1px solid #c8e6c9; padding: 12px; border-radius: 8px; text-align: center;">
              <span style="font-size: 1.5rem; font-weight: 800; color: #2e7d32; display: block;">{{ importResult.successCount }}</span>
              <span style="font-size: 0.85rem; color: #2e7d32; font-weight: 600;">Creados / Importados Exitosamente</span>
            </div>
            <div style="flex: 1; background: #ffebee; border: 1px solid #ffcdd2; padding: 12px; border-radius: 8px; text-align: center;">
              <span style="font-size: 1.5rem; font-weight: 800; color: #c62828; display: block;">{{ importResult.errorCount }}</span>
              <span style="font-size: 0.85rem; color: #c62828; font-weight: 600;">Errores / Omisiones</span>
            </div>
          </div>

          <div *ngIf="importResult.errors && importResult.errors.length > 0" style="max-height: 150px; overflow-y: auto; background: #fff5f5; border: 1px solid #fed7d7; padding: 12px; border-radius: 8px; margin-bottom: 12px;">
            <strong style="color: #c53030; font-size: 0.85rem; display: block; margin-bottom: 4px;">Detalles de Errores:</strong>
            <ul style="margin: 0; padding-left: 20px; font-size: 0.85rem; color: #9b2c2c;">
              <li *ngFor="let err of importResult.errors">{{ err }}</li>
            </ul>
          </div>

          <div *ngIf="importResult.successMessages && importResult.successMessages.length > 0" style="max-height: 150px; overflow-y: auto; background: #f0fff4; border: 1px solid #c6f6d5; padding: 12px; border-radius: 8px;">
            <strong style="color: #276749; font-size: 0.85rem; display: block; margin-bottom: 4px;">Registros Creados:</strong>
            <ul style="margin: 0; padding-left: 20px; font-size: 0.85rem; color: #22543d;">
              <li *ngFor="let msg of importResult.successMessages">{{ msg }}</li>
            </ul>
          </div>
        </div>
      </div>
    </mat-dialog-content>

    <mat-dialog-actions align="end" style="padding: 16px 24px;">
      <button mat-button (click)="close()">{{ importResult ? 'Cerrar' : 'Cancelar' }}</button>
      <button *ngIf="!importResult" mat-raised-button color="primary" [disabled]="!selectedFile || uploading" (click)="importFile()">
        <mat-icon>upload</mat-icon> Iniciar Importación
      </button>
    </mat-dialog-actions>
  `,
})
export class BulkImportDialogComponent {
  title = 'Carga Masiva de Datos';
  importUrl = '/api/Resources/import';
  templateUrl = '/api/Resources/template';
  templateFileName = 'Plantilla_Importacion.csv';

  selectedFile: File | null = null;
  uploading = false;
  importResult: ImportResult | null = null;

  constructor(
    @Optional() @Inject(MAT_DIALOG_DATA) public data: BulkImportDialogData,
    private dialogRef: MatDialogRef<BulkImportDialogComponent>,
    private http: HttpClient,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {
    if (data) {
      if (data.title) this.title = data.title;
      if (data.importUrl) this.importUrl = data.importUrl;
      if (data.templateUrl) this.templateUrl = data.templateUrl;
      if (data.templateFileName) this.templateFileName = data.templateFileName;
    }
  }

  downloadTemplate() {
    this.http.get(this.templateUrl, { responseType: 'blob' }).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = this.templateFileName;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.cdr.detectChanges();
    }
  }

  removeFile() {
    this.selectedFile = null;
    this.importResult = null;
    this.cdr.detectChanges();
  }

  importFile() {
    if (!this.selectedFile) return;

    this.uploading = true;
    this.cdr.detectChanges();

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.http.post<ImportResult>(this.importUrl, formData).subscribe({
      next: (res) => {
        this.uploading = false;
        this.importResult = res;
        this.cdr.detectChanges();
        this.snackBar.open(`Importación completada: ${res.successCount} registros creados.`, 'Cerrar', { duration: 4000 });
      },
      error: (err) => {
        this.uploading = false;
        this.cdr.detectChanges();
        console.error('Import error:', err);
        this.snackBar.open(err.error?.message || 'Error al importar el archivo', 'Cerrar', { duration: 4000 });
      }
    });
  }

  close() {
    this.dialogRef.close(this.importResult ? true : false);
  }
}
