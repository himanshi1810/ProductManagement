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
import { InvoiceDetail } from '../../shared/models/invoice-detail.model';

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
  invoiceDeytails: InvoiceDetail[] = [];
  invoices: Invoice[] = [];
  selectedInvoiceDetails: InvoiceDetail[] = [];
  showModal: boolean = false;

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
    this.loadInvoiceDetails();
  }

  getCustomerName(customerId: number): string {
    return this.customers.find(c => c.customerId === customerId)?.name || 'Unknown';
  }

  loadInvoices() {
    this.invoiceService.getAllInvoices().subscribe((data) => {
      console.log(data);
      this.invoices = data;
    });
  }

  loadInvoiceDetails() {
    this.invoiceService.getInvoiceDetails().subscribe((data) => {
      console.log(data);
      this.invoiceDeytails = data;
    });
  }

  addItem() {
    if (!this.isFormValid()) {
      alert("Please select all fields properly before adding.");
      return;
    }
    this.items.push({ productId: this.selectedProductId, quantity: this.quantity });
    this.selectedProductId = 0;
    this.quantity = 1;
  }

  getProductName(productId: number): string {
    return this.productsMap[productId] || 'Unknown';
  }

  openInvoiceDetails(invoiceId: number) {
    this.selectedInvoiceDetails = this.invoiceDeytails.filter(i => i.invoiceId === invoiceId);
    this.showModal = true;
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
      window.location.reload();
    });
  }

  isFormValid(): boolean {
    return (
      this.selectedCustomerId > 0 &&
      this.selectedProductId > 0 &&
      this.quantity > 0
    );
  }

  closeModal() {
    this.showModal = false;
  }
}