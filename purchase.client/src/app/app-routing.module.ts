import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ToolRequestListComponent } from './components/tool-request/tool-request-list.component';
import { CreateToolRequestComponent } from './components/tool-request/create-tool-request.component';
import { PurchaseRequestListComponent } from './components/purchase-request/purchase-request-list.component';
import { PurchaseRequestDetailComponent } from './components/purchase-request/purchase-request-detail.component';
import { QuotationListComponent } from './components/quotation/quotation-list.component';
import { PurchaseOrderListComponent } from './components/purchase-order/purchase-order-list.component';
import { CreatePurchaseOrderComponent } from './components/purchase-order/create-purchase-order.component';
import { PurchaseOrderDetailComponent } from './components/purchase-order/purchase-order-detail.component';
import { GoodsReceiptListComponent } from './components/goods-receipt/goods-receipt-list.component';
import { CreateGoodsReceiptComponent } from './components/goods-receipt/create-goods-receipt.component';
import { GoodsReceiptDetailComponent } from './components/goods-receipt/goods-receipt-detail.component';
import { PaymentListComponent } from './components/payment/payment-list.component';
import { CreatePaymentComponent } from './components/payment/create-payment.component';
import { PaymentDetailComponent } from './components/payment/payment-detail.component';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'tool-requests', component: ToolRequestListComponent },
  { path: 'tool-requests/create', component: CreateToolRequestComponent },
  { path: 'purchase-requests', component: PurchaseRequestListComponent },
  { path: 'purchase-requests/:id', component: PurchaseRequestDetailComponent },
  { path: 'quotations', component: QuotationListComponent },
  { path: 'purchase-orders', component: PurchaseOrderListComponent },
  { path: 'purchase-orders/create', component: CreatePurchaseOrderComponent },
  { path: 'purchase-orders/:id', component: PurchaseOrderDetailComponent },
  { path: 'goods-receipts', component: GoodsReceiptListComponent },
  { path: 'goods-receipts/create', component: CreateGoodsReceiptComponent },
  { path: 'goods-receipts/:id', component: GoodsReceiptDetailComponent },
  { path: 'payments', component: PaymentListComponent },
  { path: 'payments/create', component: CreatePaymentComponent },
  { path: 'payments/:id', component: PaymentDetailComponent },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
