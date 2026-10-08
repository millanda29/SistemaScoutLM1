import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Loan } from '../../core/models/models';

@Injectable({ providedIn: 'root' })
export class LoansService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Loan[]> {
    return this.http.get<Loan[]>('/api/Loans');
  }

  getById(id: number): Observable<Loan> {
    return this.http.get<Loan>(`/api/Loans/${id}`);
  }

  create(data: Partial<Loan>): Observable<Loan> {
    return this.http.post<Loan>('/api/Loans', data);
  }

  approve(id: number, data: { approvalType: string; observations?: string }): Observable<Loan> {
    return this.http.patch<Loan>(`/api/Loans/${id}/approve`, data);
  }

  reject(id: number, data: { rejectionReason: string }): Observable<Loan> {
    return this.http.patch<Loan>(`/api/Loans/${id}/reject`, data);
  }

  deliver(id: number): Observable<Loan> {
    return this.http.patch<Loan>(`/api/Loans/${id}/deliver`, {});
  }

  returnLoan(id: number, data: { conditionAtReturn: string; damagesDetected?: string }): Observable<Loan> {
    return this.http.patch<Loan>(`/api/Loans/${id}/return`, data);
  }
}
