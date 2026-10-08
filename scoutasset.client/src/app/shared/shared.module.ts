import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ConfirmDialog } from './confirm-dialog.component';
import { PromptDialog } from './prompt-dialog.component';
import { BulkImportDialogComponent } from './bulk-import-dialog.component';

@NgModule({
  declarations: [ConfirmDialog, PromptDialog, BulkImportDialogComponent],
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  exports: [ConfirmDialog, PromptDialog, BulkImportDialogComponent, MatDialogModule],
})
export class SharedModule {}
