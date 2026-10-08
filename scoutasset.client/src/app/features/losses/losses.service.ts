import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Loss } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class LossesService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Loss[]> {
    return this.http.get<Loss[]>('/api/Losses');
  }

  confirm(id: number): Observable<Loss> {
    return this.http.put<Loss>(`/api/Losses/${id}/confirm`, {});
  }

  recover(id: number): Observable<Loss> {
    return this.http.put<Loss>(`/api/Losses/${id}/recover`, {});
  }
}
