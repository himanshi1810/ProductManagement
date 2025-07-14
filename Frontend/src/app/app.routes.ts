import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CategoryComponent } from './features/category/category.component';
import { CustomerComponent } from './features/customer/customer.component';
import { ProductComponent } from './features/product/product.component';
import { InvoiceComponent } from './features/invoice/invoice.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    component: DashboardComponent
  },
  {
    path: 'category',
    component:CategoryComponent
  },
  {
    path: 'customer',
    component: CustomerComponent
  },
  {
    path: 'product',
    component:ProductComponent
  },
  {
    path: 'invoice',
    component:InvoiceComponent
  },
];
