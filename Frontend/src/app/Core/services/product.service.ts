import { Injectable } from '@angular/core';
import { Environment } from '../../environment';
import { HttpClient } from '@angular/common/http';
import { ProductCreateDto } from '../../shared/models/dtos/product-create.dto';
import { Product } from '../../shared/models/product.model';
import { Observable } from 'rxjs';
import { ProductPriceDto } from '../../shared/models/product-price.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private apiUrl = `${Environment.apiUrl}/Product`;
  constructor(private http : HttpClient) { }

    getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  add(product: ProductCreateDto): Observable<void> {
    return this.http.post<void>(this.apiUrl, product);
  }

  update(product: Product): Observable<void> {
    return this.http.put<void>(this.apiUrl, product);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getPriceForToday(productId: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/${productId}/price-today`);
  }

  addPrice(dto: ProductPriceDto): Observable<string> {
    return this.http.post(`${this.apiUrl}/add-price`, dto, { responseType: 'text' });
  }
}
