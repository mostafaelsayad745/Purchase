import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ToolRequestListComponent } from './components/tool-request/tool-request-list.component';
import { CreateToolRequestComponent } from './components/tool-request/create-tool-request.component';
import { PurchaseOrderListComponent } from './components/purchase-order/purchase-order-list.component';
import { GoodsReceiptListComponent } from './components/goods-receipt/goods-receipt-list.component';
import { PaymentListComponent } from './components/payment/payment-list.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    ToolRequestListComponent,
    CreateToolRequestComponent,
    PurchaseOrderListComponent,
    GoodsReceiptListComponent,
    PaymentListComponent
  ],
  imports: [
    BrowserModule, 
    HttpClientModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
