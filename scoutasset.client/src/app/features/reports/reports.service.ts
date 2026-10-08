import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Resource, Loan, Maintenance, Movement } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class ReportsService {
  constructor(private http: HttpClient) {}

  getInventory(): Observable<Resource[]> {
    return this.http.get<Resource[]>('/api/reports/inventory');
  }

  getByCategory(): Observable<any[]> {
    return this.http.get<any[]>('/api/reports/by-category');
  }

  getActiveLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>('/api/reports/active-loans');
  }

  getOverdueLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>('/api/reports/overdue-loans');
  }

  getPendingMaintenance(): Observable<Maintenance[]> {
    return this.http.get<Maintenance[]>('/api/reports/pending-maintenance');
  }

  getMovements(startDate?: string, endDate?: string): Observable<Movement[]> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    return this.http.get<Movement[]>('/api/reports/movements', { params });
  }
}
