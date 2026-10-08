import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Maintenance } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class MaintenanceService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Maintenance[]> {
    return this.http.get<Maintenance[]>('/api/Maintenance');
  }

  getById(id: number): Observable<Maintenance> {
    return this.http.get<Maintenance>(`/api/Maintenance/${id}`);
  }

  create(data: Partial<Maintenance>): Observable<Maintenance> {
    return this.http.post<Maintenance>('/api/Maintenance', data);
  }

  start(id: number): Observable<Maintenance> {
    return this.http.patch<Maintenance>(`/api/Maintenance/${id}/start`, {});
  }

  complete(id: number, data: { cost?: number; result?: string; nextMaintenanceDate?: string }): Observable<Maintenance> {
    return this.http.patch<Maintenance>(`/api/Maintenance/${id}/complete`, data);
  }

  autoSchedule(intervalMonths: number = 6): Observable<{ scheduledCount: number; message: string }> {
    return this.http.post<{ scheduledCount: number; message: string }>(`/api/Maintenance/auto-schedule?intervalMonths=${intervalMonths}`, {});
  }
}
