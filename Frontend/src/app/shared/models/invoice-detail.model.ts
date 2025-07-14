import { Product } from "./product.model";

export interface InvoiceDetail {
  invoiceDetailId: number;
  invoiceId: number;
  productId: number;
  quantity: number;
  rate: number;
  subTotal: number;
  taxAmount: number;
  totalAmount: number;
  product: Product;
}
