export interface ProductPriceDto {
  productId: number;
  price: number;
  fromDate?: string;
  toDate?: string;
  isDefault: boolean;
}

export interface ProductPrice {
  id: number;
  productId: number;
  price: number;
  fromDate?: string;
  toDate?: string;
  isDefault: boolean;
}
