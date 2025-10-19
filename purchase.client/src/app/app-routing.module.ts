import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ToolRequestListComponent } from './components/tool-request/tool-request-list.component';
import { CreateToolRequestComponent } from './components/tool-request/create-tool-request.component';
import { PurchaseOrderListComponent } from './components/purchase-order/purchase-order-list.component';
import { GoodsReceiptListComponent } from './components/goods-receipt/goods-receipt-list.component';
import { PaymentListComponent } from './components/payment/payment-list.component';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'tool-requests', component: ToolRequestListComponent },
  { path: 'tool-requests/create', component: CreateToolRequestComponent },
  { path: 'purchase-orders', component: PurchaseOrderListComponent },
  { path: 'goods-receipts', component: GoodsReceiptListComponent },
  { path: 'payments', component: PaymentListComponent },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
