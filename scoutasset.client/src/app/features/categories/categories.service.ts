import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class CategoriesService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>('/api/Categories');
  }

  create(data: Partial<Category>): Observable<Category> {
    return this.http.post<Category>('/api/Categories', data);
  }

  update(id: number, data: Partial<Category>): Observable<Category> {
    return this.http.put<Category>(`/api/Categories/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`/api/Categories/${id}`);
  }
}
