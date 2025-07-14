import { Injectable } from '@angular/core';
import { Environment } from '../../environment';
import { HttpClient } from '@angular/common/http';
import { Category } from '../../shared/models/category.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private apiUrl = `${Environment.apiUrl}/Category`;
  constructor(private http : HttpClient) { }

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }
  getById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }
  add(category: Category): Observable<void> {
    return this.http.post<void>(this.apiUrl, { ...category, products: null });
  }
  update(category: Category): Observable<void> {
    return this.http.put<void>(this.apiUrl, { ...category, products: null });
  }
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
