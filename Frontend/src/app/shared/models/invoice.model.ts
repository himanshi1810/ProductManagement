import { Customer } from "./customer.model";
import { InvoiceDetail } from "./invoice-detail.model";

export interface Invoice {
  invoiceId: number;
  invoiceDate: string;
  customerId: number;
  total: number;
  customer: Customer;
  invoiceDetails: InvoiceDetail[];
}
