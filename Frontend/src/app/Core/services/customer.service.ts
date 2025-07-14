import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Customer } from '../../shared/models/customer.model';
import { Observable } from 'rxjs';
import { Environment } from '../../environment'; 
@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  private apiUrl = `${Environment.apiUrl}/Customer`;

  constructor(private http : HttpClient) { }

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.apiUrl);
  }

  getById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.apiUrl}/${id}`);
  }

  add(customer: Customer): Observable<void> {
    return this.http.post<void>(this.apiUrl, { ...customer, Invoices: null });
  }

  update(customer: Customer): Observable<void> {
    return this.http.put<void>(this.apiUrl, { ...customer, Invoices: null });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
