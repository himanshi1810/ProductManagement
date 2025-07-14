import { Injectable } from '@angular/core';
import { Environment } from '../../environment';
import { HttpClient } from '@angular/common/http';
import { Invoice } from '../../shared/models/invoice.model';
import { Observable } from 'rxjs';
import { InvoiceRequestDto } from '../../shared/models/invoice-request.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  private apiUrl = `${Environment.apiUrl}/Invoice`;
  constructor(private http : HttpClient) { }

  getAllInvoices(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.apiUrl);
  }

  getInvoiceById(id: number): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/${id}`);
  }

  createInvoice(request: InvoiceRequestDto): Observable<Invoice> {
    return this.http.post<Invoice>(this.apiUrl, request);
  }
}
