import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class UsersService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<User[]> {
    return this.http.get<User[]>('/api/Users');
  }

  getById(id: string): Observable<User> {
    return this.http.get<User>(`/api/Users/${id}`);
  }

  create(data: {
    userName: string;
    email: string;
    password?: string;
    role?: string;
    ci: string;
    nombres: string;
    apellidos: string;
    telefono: string;
  }): Observable<User> {
    return this.http.post<User>('/api/Users', data);
  }

  update(
    id: string,
    data: { userName?: string; email?: string; roles?: string[] }
  ): Observable<User> {
    return this.http.put<User>(`/api/Users/${id}`, data);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`/api/Users/${id}`);
  }
}
