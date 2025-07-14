export interface ProductPriceDto {
  productId: number;
  price: number;
  fromDate: string; 
  toDate: string;
}

export interface ProductPrice {
  id: number;
  productId: number;
  price: number;
  fromDate: string;
  toDate: string;
}
