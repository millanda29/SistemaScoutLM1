import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LossesService } from './losses.service';
import { Loss } from '../../core/models/models';
import { ConfirmDialog } from '../../shared/confirm-dialog.component';

@Component({
  standalone: false,
  selector: 'app-losses',
  templateUrl: './losses.component.html',
})
export class LossesComponent implements OnInit {
  displayedColumns = ['lossNumber', 'resourceCode', 'resourceName', 'status', 'circumstances', 'actions'];
  losses: Loss[] = [];

  constructor(
    private service: LossesService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe((res) => (this.losses = res));
  }

  confirm(loss: Loss) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Confirmar pérdida', message: '¿Confirmar pérdida?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.confirm(loss.id).subscribe({
          next: () => {
            this.snackBar.open('Pérdida confirmada', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }

  recover(loss: Loss) {
    this.dialog.open(ConfirmDialog, { data: { title: 'Recuperar recurso', message: '¿Recuperar recurso?' } })
      .afterClosed().subscribe((result) => {
        if (!result) return;
        this.service.recover(loss.id).subscribe({
          next: () => {
            this.snackBar.open('Recurso recuperado', 'Cerrar', { duration: 3000 });
            this.load();
          },
        });
      });
  }
}
