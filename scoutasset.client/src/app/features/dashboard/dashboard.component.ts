import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DashboardData, CategoryStat, RecentMovement } from '../../core/models/models';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  standalone: false,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  data: DashboardData | null = null;
  error = false;
  loaded = false;
  currentDate = new Date();
  
  // KPI Metrics (Primary Cards)
  primaryMetrics: { key: string; icon: string; label: string; color: string; colorLight: string; value: number }[] = [];
  
  // Financial Valuation Metrics
  totalAcquisitionCost = 0;
  totalAppraisedValue = 0;

  // Secondary Alerts / Status
  alerts: { key: string; icon: string; label: string; color: string; colorLight: string; value: number }[] = [];

  // Category & Recent Activity Lists
  categoryStats: CategoryStat[] = [];
  recentMovements: RecentMovement[] = [];

  // Availability percentage
  availabilityPercent = 0;
  strokeDashArray = '0, 100';

  constructor(
    private http: HttpClient, 
    private cdr: ChangeDetectorRef,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.fetchData();
  }

  fetchData() {
    const empty: DashboardData = {
      total: 0, 
      disponibles: 0, 
      prestados: 0, 
      enMantenimiento: 0,
      noLocalizados: 0, 
      perdidos: 0, 
      dadosDeBaja: 0, 
      daniados: 0,
      prestamosVencidos: 0,
      totalAcquisitionCost: 0,
      totalAppraisedValue: 0,
      categoryStats: [],
      recentMovements: []
    };

    this.error = false;
    this.loaded = false;
    this.cdr.detectChanges();

    this.http.get<DashboardData>('/api/Dashboard').subscribe({
      next: (res) => {
        try {
          this.data = res;
          this.processData(res || empty);
          this.loaded = true;
          this.cdr.detectChanges();
        } catch (e) {
          console.error(e);
          this.error = true;
          this.data = empty;
          this.processData(empty);
          this.loaded = true;
          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error(err);
        this.error = true;
        this.data = empty;
        this.processData(empty);
        this.loaded = true;
        this.cdr.detectChanges();
      },
    });

    setTimeout(() => {
      if (!this.loaded) {
        this.loaded = true;
        this.data = this.data || empty;
        this.processData(this.data);
        this.cdr.detectChanges();
      }
    }, 8000);
  }

  private processData(res: DashboardData) {
    if (!res) return;

    const total = typeof res.total === 'number' ? res.total : 0;
    const disponibles = typeof res.disponibles === 'number' ? res.disponibles : 0;
    const prestados = typeof res.prestados === 'number' ? res.prestados : 0;
    const enMantenimiento = typeof res.enMantenimiento === 'number' ? res.enMantenimiento : 0;
    const noLocalizados = typeof res.noLocalizados === 'number' ? res.noLocalizados : 0;
    const perdidos = typeof res.perdidos === 'number' ? res.perdidos : 0;
    const dadosDeBaja = typeof res.dadosDeBaja === 'number' ? res.dadosDeBaja : 0;
    const daniados = typeof res.daniados === 'number' ? res.daniados : 0;
    const prestamosVencidos = typeof res.prestamosVencidos === 'number' ? res.prestamosVencidos : 0;

    this.totalAcquisitionCost = res.totalAcquisitionCost || 0;
    this.totalAppraisedValue = res.totalAppraisedValue || 0;
    this.categoryStats = res.categoryStats || [];
    this.recentMovements = res.recentMovements || [];

    // Primary metrics
    this.primaryMetrics = [
      { key: 'total', icon: 'inventory_2', label: 'Total Recursos', color: '#7c4dff', colorLight: 'rgba(124, 77, 255, 0.1)', value: total },
      { key: 'disponibles', icon: 'check_circle', label: 'Disponibles', color: '#2e7d32', colorLight: 'rgba(46, 125, 50, 0.1)', value: disponibles },
      { key: 'prestados', icon: 'handshake', label: 'Prestados', color: '#1565c0', colorLight: 'rgba(21, 101, 192, 0.1)', value: prestados },
      { key: 'mantenimiento', icon: 'build', label: 'En Soporte', color: '#e65100', colorLight: 'rgba(230, 81, 0, 0.1)', value: enMantenimiento },
    ];

    // Secondary alerts status
    this.alerts = [
      { key: 'vencidos', icon: 'schedule', label: 'Préstamos Vencidos', color: '#b71c1c', colorLight: 'rgba(183, 28, 28, 0.08)', value: prestamosVencidos },
      { key: 'daniados', icon: 'warning', label: 'Dañados', color: '#d84315', colorLight: 'rgba(216, 67, 21, 0.08)', value: daniados },
      { key: 'noLocalizados', icon: 'search_off', label: 'No Localizados', color: '#f9a825', colorLight: 'rgba(249, 168, 37, 0.08)', value: noLocalizados },
      { key: 'perdidos', icon: 'cancel', label: 'Perdidos', color: '#c62828', colorLight: 'rgba(198, 40, 40, 0.08)', value: perdidos },
      { key: 'dadosDeBaja', icon: 'delete_forever', label: 'Bajas', color: '#4e342e', colorLight: 'rgba(78, 52, 46, 0.08)', value: dadosDeBaja },
    ];

    // Calculate availability percentage safely
    if (total > 0) {
      this.availabilityPercent = Math.round((disponibles / total) * 100);
    } else {
      this.availabilityPercent = 0;
    }
    
    this.strokeDashArray = `${this.availabilityPercent}, 100`;
  }
}
