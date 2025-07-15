import { Component, OnInit } from '@angular/core';
import { Customer } from '../../shared/models/customer.model';
import { Product } from '../../shared/models/product.model';
import { InvoiceItemDto, InvoiceRequestDto } from '../../shared/models/invoice-request.model';
import { Invoice } from '../../shared/models/invoice.model';
import { InvoiceService } from '../../Core/services/invoice.service';
import { CustomerService } from '../../Core/services/customer.service';
import { ProductService } from '../../Core/services/product.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-invoice',
  imports: [CommonModule, FormsModule],
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.css']
})
export class InvoiceComponent implements OnInit {
  productsMap: { [id: number]: string } = {};
  customers: Customer[] = [];
  products: Product[] = [];
  items: InvoiceItemDto[] = [];
  customerNames: { [invoiceId: number]: string } = {};
  selectedCustomerId: number = 0;
  selectedProductId: number = 0;
  quantity: number = 1;

  invoices: Invoice[] = [];

  constructor(
    private invoiceService: InvoiceService,
    private customerService: CustomerService,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    this.customerService.getAll().subscribe(data => this.customers = data);

    this.productService.getAll().subscribe(data => {
      this.products = data;
      this.productsMap = Object.fromEntries(
        data.map(p => [p.productId, p.name])
      );
      this.loadInvoices();
    });
  }

  getCustomerName(customerId: number): string {
    return this.customers.find(c => c.customerId === customerId)?.name || 'Unknown';
  }

  loadInvoices() {
    this.invoiceService.getAllInvoices().subscribe((data) => {
      this.invoices = data;
    });
  }

  addItem() {
    this.items.push({ productId: this.selectedProductId, quantity: this.quantity });
    this.selectedProductId = 0;
    this.quantity = 1;
  }

  getProductName(productId: number): string {
    return this.productsMap[productId] || 'Unknown';
  }


  submitInvoice() {
    const request: InvoiceRequestDto = {
      customerId: this.selectedCustomerId,
      items: this.items
    };

    this.invoiceService.createInvoice(request).subscribe(res => {
      alert('Invoice created!');
      this.items = [];
      this.loadInvoices();
    });
  }
}