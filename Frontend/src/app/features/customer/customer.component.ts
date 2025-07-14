import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Customer } from '../../shared/models/customer.model';
import { CustomerService } from '../../Core/services/customer.service';

@Component({
  standalone: true,
  selector: 'app-customer',
  imports: [CommonModule, FormsModule],
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css'],
})
export class CustomerComponent implements OnInit {
  customers: Customer[] = [];
  customerForm: Customer = { name: '', email: '' };
  isEdit = false;

  constructor(private customerService: CustomerService) {}

  ngOnInit(): void {
    this.fetchCustomers();
  }

  fetchCustomers() {
    this.customerService.getAll().subscribe((data) => (this.customers = data));
  }

  submitForm() {
    if (this.isEdit && this.customerForm.customerId) {
      this.customerService.update(this.customerForm).subscribe(() => {
        this.resetForm();
        this.fetchCustomers();
      });
    } else {
      this.customerService.add(this.customerForm).subscribe(() => {
        this.resetForm();
        this.fetchCustomers();
      });
    }
  }

  edit(customer: Customer) {
    this.customerForm = { ...customer };
    this.isEdit = true;
  }

  delete(id: number) {
    if (confirm('Are you sure you want to delete this customer?')) {
      this.customerService.delete(id).subscribe(() => this.fetchCustomers());
    }
  }

  resetForm() {
    this.customerForm = { name: '', email: '' };
    this.isEdit = false;
  }
}
