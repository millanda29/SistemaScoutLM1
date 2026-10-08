import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Resource } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class ResourcesService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Resource[]> {
    return this.http.get<Resource[]>('/api/Resources');
  }

  getById(id: number): Observable<Resource> {
    return this.http.get<Resource>(`/api/Resources/${id}`);
  }

  create(data: Partial<Resource>): Observable<Resource> {
    return this.http.post<Resource>('/api/Resources', data);
  }

  update(id: number, data: Partial<Resource>): Observable<Resource> {
    return this.http.put<Resource>(`/api/Resources/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`/api/Resources/${id}`);
  }
}
