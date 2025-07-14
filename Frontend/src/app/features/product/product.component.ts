import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Product } from '../../shared/models/product.model';
import { Category } from '../../shared/models/category.model';
import { ProductPriceDto } from '../../shared/models/product-price.model';
import { ProductService } from '../../Core/services/product.service';
import { CategoryService } from '../../Core/services/category.service';


@Component({
  standalone: true,
  selector: 'app-product',
  imports: [CommonModule, FormsModule],
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  categories: Category[] = [];
  categoryNames: { [productId: number]: string } = {};
  productForm: { name: string; tax: number; categoryId: number } = { name: '', tax: 0, categoryId: 0 };
  isEdit = false;
  editId: number | null = null;

  priceForm: ProductPriceDto = { productId: 0, price: 0, fromDate: '', toDate: '' };

  constructor(private productService: ProductService, private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.loadProducts();
    this.categoryService.getAll().subscribe((data) => (this.categories = data));
  }

  getCategoryName(categoryId: number): string {
    return this.categories.find(c => c.categoryId === categoryId)?.name || 'Unknown';
  }

  loadProducts() {
    this.productService.getAll().subscribe((products) => {
      this.products = products;
    });
  }

  submitForm() {
    if (this.isEdit && this.editId) {
      const updatedProduct: Product = { ...this.productForm, productId: this.editId };
      this.productService.update(updatedProduct).subscribe(() => {
        this.resetForm();
        this.loadProducts();
      });
    } else {
      this.productService.add(this.productForm).subscribe(() => {
        this.resetForm();
        this.loadProducts();
      });
    }
  }

  edit(product: Product) {
    this.productForm = { name: product.name, tax: product.tax, categoryId: product.categoryId };
    this.editId = product.productId!;
    this.isEdit = true;
  }

  delete(id: number) {
    if (confirm('Are you sure?')) {
      this.productService.delete(id).subscribe(() => this.loadProducts());
    }
  }

  resetForm() {
    this.productForm = { name: '', tax: 0, categoryId: 0 };
    this.isEdit = false;
    this.editId = null;
  }

  setPrice() {
    this.productService.addPrice(this.priceForm).subscribe((msg) => {
      alert(msg);
      this.priceForm = { productId: 0, price: 0, fromDate: '', toDate: '' };
    });
  }

  getTodayPrice(id: number) {
    this.productService.getPriceForToday(id).subscribe((price) => {
      alert(`Today's price: ₹${price}`);
    });
  }
}