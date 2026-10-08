import { Component, Input } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  standalone: false,
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  @Input() sidenav!: MatSidenav;

  constructor(public auth: AuthService) {}

  logout() {
    this.auth.logout();
  }
}
