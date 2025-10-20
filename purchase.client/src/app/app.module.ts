import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
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

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    ToolRequestListComponent,
    CreateToolRequestComponent,
    PurchaseRequestListComponent,
    PurchaseRequestDetailComponent,
    QuotationListComponent,
    PurchaseOrderListComponent,
    CreatePurchaseOrderComponent,
    PurchaseOrderDetailComponent,
    GoodsReceiptListComponent,
    CreateGoodsReceiptComponent,
    GoodsReceiptDetailComponent,
    PaymentListComponent,
    CreatePaymentComponent,
    PaymentDetailComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
