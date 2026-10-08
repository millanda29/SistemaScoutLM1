import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PhysicalInventory } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class PhysicalInventoryService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<PhysicalInventory[]> {
    return this.http.get<PhysicalInventory[]>('/api/PhysicalInventory');
  }

  getById(id: number): Observable<PhysicalInventory> {
    return this.http.get<PhysicalInventory>(`/api/PhysicalInventory/${id}`);
  }

  create(data: Partial<PhysicalInventory>): Observable<PhysicalInventory> {
    return this.http.post<PhysicalInventory>('/api/PhysicalInventory', data);
  }

  start(id: number): Observable<PhysicalInventory> {
    return this.http.patch<PhysicalInventory>(`/api/PhysicalInventory/${id}/start`, {});
  }

  finish(id: number): Observable<PhysicalInventory> {
    return this.http.patch<PhysicalInventory>(`/api/PhysicalInventory/${id}/finish`, {});
  }

  reconcile(id: number): Observable<PhysicalInventory> {
    return this.http.post<PhysicalInventory>(`/api/PhysicalInventory/${id}/reconcile`, {});
  }

  registerItem(id: number, data: any): Observable<any> {
    return this.http.post<any>(`/api/PhysicalInventory/${id}/items`, data);
  }
}
