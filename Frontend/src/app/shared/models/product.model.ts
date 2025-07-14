import { Category } from "./category.model";
import { ProductPrice } from "./product-price.model";

export interface Product {
  productId?: number;
  name: string;
  tax: number;
  categoryId: number;
  category?: Category;
  productPrices?: ProductPrice[];
}