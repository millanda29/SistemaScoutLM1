import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

export interface PromptField {
  key: string;
  label: string;
  type: 'text' | 'textarea' | 'select' | 'number' | 'multiselect';
  required: boolean;
  options?: { value: any; label: string }[];
  value?: any;
}

@Component({
  standalone: false,
  template: `
    <h2 mat-dialog-title>{{ data.title }}</h2>
    <mat-dialog-content>
      <div class="dialog-form" style="padding-top: 8px;">
        <mat-form-field *ngFor="let field of data.fields" appearance="outline" class="full-width" [ngSwitch]="field.type">
          <mat-label>{{ field.label }}</mat-label>
          <input *ngSwitchCase="'text'" matInput [(ngModel)]="field.value" [name]="field.key" [required]="field.required" />
          <input *ngSwitchCase="'number'" matInput type="number" [(ngModel)]="field.value" [name]="field.key" [required]="field.required" />
          <textarea *ngSwitchCase="'textarea'" matInput [(ngModel)]="field.value" [name]="field.key" [required]="field.required" rows="3"></textarea>
          
          <mat-select *ngSwitchCase="'select'" [(ngModel)]="field.value" [name]="field.key" [required]="field.required">
            <mat-option *ngFor="let opt of field.options" [value]="opt.value">{{ opt.label }}</mat-option>
          </mat-select>

          <mat-select *ngSwitchCase="'multiselect'" [(ngModel)]="field.value" [name]="field.key" [required]="field.required" multiple>
            <mat-option *ngFor="let opt of field.options" [value]="opt.value">{{ opt.label }}</mat-option>
          </mat-select>
        </mat-form-field>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="cancel()">Cancelar</button>
      <button mat-raised-button color="primary" [disabled]="!isValid()" (click)="confirm()">Aceptar</button>
    </mat-dialog-actions>
  `,
})
export class PromptDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { title: string; fields: PromptField[] },
    private dialogRef: MatDialogRef<PromptDialog>,
  ) {}

  isValid(): boolean {
    return this.data.fields.every((f) => {
      if (!f.required) return true;
      if (f.value === null || f.value === undefined || f.value === '') return false;
      if (Array.isArray(f.value) && f.value.length === 0) return false;
      return true;
    });
  }

  confirm() {
    if (!this.isValid()) return;
    const result: any = {};
    for (const f of this.data.fields) result[f.key] = f.value;
    this.dialogRef.close(result);
  }

  cancel() {
    this.dialogRef.close(null);
  }
}
