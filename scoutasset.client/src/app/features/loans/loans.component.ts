import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoansService } from './loans.service';
import { Loan } from '../../core/models/models';
import { LoanFormComponent } from './loan-form.component';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';
import { PromptDialog, PromptField } from '../../shared/prompt-dialog.component';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  standalone: false,
  selector: 'app-loans',
  templateUrl: './loans.component.html',
})
export class LoansComponent implements OnInit {
  displayedColumns = ['requestNumber', 'requesterType', 'reason', 'loanDate', 'expectedReturnDate', 'status', 'pdfDocumentPath', 'actions'];
  dataSource = new MatTableDataSource<Loan>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private service: LoansService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe((res) => {
      this.dataSource.data = res;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  openForm() {
    this.dialog.open(LoanFormComponent, { width: '600px' })
      .afterClosed().subscribe((result) => {
        if (result) this.load();
      });
  }

  approve(loan: Loan) {
    const fields: PromptField[] = [
      {
        key: 'approvalType', label: 'Tipo de aprobación', type: 'select', required: true,
        options: [
          { value: 'DOCUMENTADA', label: 'Documentada' },
          { value: 'VERBAL', label: 'Verbal' },
        ],
      },
      { key: 'observations', label: 'Observaciones', type: 'textarea', required: false },
    ];
    this.dialog.open(PromptDialog, { data: { title: 'Aprobar Préstamo', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.approve(loan.id, { approvalType: result.approvalType, observations: result.observations || undefined }).subscribe({
          next: () => {
            this.snackBar.open('Préstamo aprobado', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }

  reject(loan: Loan) {
    const fields: PromptField[] = [
      { key: 'rejectionReason', label: 'Motivo de rechazo', type: 'textarea', required: true },
    ];
    this.dialog.open(PromptDialog, { data: { title: 'Rechazar Préstamo', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.reject(loan.id, { rejectionReason: result.rejectionReason }).subscribe({
          next: () => {
            this.snackBar.open('Préstamo rechazado', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }

  deliver(loan: Loan) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Entregar préstamo', message: '¿Entregar este préstamo?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.deliver(loan.id).subscribe({
          next: () => {
            this.snackBar.open('Préstamo entregado', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }

  returnLoan(loan: Loan) {
    const fields: PromptField[] = [
      {
        key: 'conditionAtReturn', label: 'Condición al devolver', type: 'select', required: true,
        options: [
          { value: 'BUENO', label: 'Bueno' },
          { value: 'REGULAR', label: 'Regular' },
          { value: 'DANIADO', label: 'Dañado' },
        ],
      },
      { key: 'damagesDetected', label: 'Daños detectados', type: 'text', required: false },
    ];
    this.dialog.open(PromptDialog, { data: { title: 'Devolver Préstamo', fields } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.returnLoan(loan.id, {
          conditionAtReturn: result.conditionAtReturn,
          damagesDetected: result.damagesDetected || undefined,
        }).subscribe({
          next: () => {
            this.snackBar.open('Préstamo devuelto', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }
}
