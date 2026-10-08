import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Retirement } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class RetirementsService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Retirement[]> {
    return this.http.get<Retirement[]>('/api/Retirements');
  }

  create(data: Partial<Retirement>): Observable<Retirement> {
    return this.http.post<Retirement>('/api/Retirements', data);
  }

  review(id: number, data: { rejectionReason?: string }): Observable<Retirement> {
    return this.http.put<Retirement>(`/api/Retirements/${id}/review`, data);
  }

  authorize(id: number): Observable<Retirement> {
    return this.http.put<Retirement>(`/api/Retirements/${id}/authorize`, {});
  }

  execute(id: number): Observable<Retirement> {
    return this.http.put<Retirement>(`/api/Retirements/${id}/execute`, {});
  }

  reject(id: number, data: { rejectionReason: string }): Observable<Retirement> {
    return this.http.put<Retirement>(`/api/Retirements/${id}/reject`, data);
  }
}
