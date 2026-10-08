import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Location } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class LocationsService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Location[]> {
    return this.http.get<Location[]>('/api/Locations');
  }

  create(data: Partial<Location>): Observable<Location> {
    return this.http.post<Location>('/api/Locations', data);
  }

  update(id: number, data: Partial<Location>): Observable<Location> {
    return this.http.put<Location>(`/api/Locations/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`/api/Locations/${id}`);
  }
}
