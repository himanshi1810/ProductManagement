export interface InvoiceRequestDto {
  customerId: number;
  items: InvoiceItemDto[];
}

export interface InvoiceItemDto {
  productId: number;
  quantity: number;
}
