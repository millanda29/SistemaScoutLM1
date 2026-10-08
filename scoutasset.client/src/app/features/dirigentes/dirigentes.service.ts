import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Dirigente {
  id: number;
  ci: string;
  nombres: string;
  apellidos: string;
  correo: string;
  telefono: string;
  habilitadoParaCustodio: boolean;
  createdAt: string;
  updatedAt?: string;
}

@Injectable({ providedIn: 'root' })
export class DirigentesService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Dirigente[]> {
    return this.http.get<Dirigente[]>('/api/Dirigentes');
  }

  getById(id: number): Observable<Dirigente> {
    return this.http.get<Dirigente>(`/api/Dirigentes/${id}`);
  }

  create(data: Partial<Dirigente>): Observable<Dirigente> {
    return this.http.post<Dirigente>('/api/Dirigentes', data);
  }

  update(id: number, data: Partial<Dirigente>): Observable<void> {
    return this.http.put<void>(`/api/Dirigentes/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`/api/Dirigentes/${id}`);
  }
}
