import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PurchaseOrder {
  id: number;
  orderNumber: string;
  quotationId: number;
  orderDate: string;
  expectedDeliveryDate: string;
  status: string;
  totalAmount: number;
  purchaseOrderDocument?: string;
  invoiceDocument?: string;
  deliveryNoteDocument?: string;
  supplierAgreementDocument?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
  quotation?: any;
  goodsReceipt?: any;
}

export interface PurchaseOrderSummary {
  id: number;
  orderNumber: string;
  orderDate: string;
  expectedDeliveryDate: string;
  status: string;
  totalAmount: number;
  supplierNameAr: string;
  supplierNameEn: string;
  toolNameAr: string;
  toolNameEn: string;
}

export interface CreatePurchaseOrderDto {
  quotationId: number;
  expectedDeliveryDate: string;
  notes?: string;
}

export interface UpdatePurchaseOrderDto {
  purchaseOrderDocument?: string;
  invoiceDocument?: string;
  deliveryNoteDocument?: string;
  supplierAgreementDocument?: string;
  notes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderService {
  private apiUrl = '/api/purchaseorders';

  constructor(private http: HttpClient) { }

  getPurchaseOrders(): Observable<PurchaseOrderSummary[]> {
    return this.http.get<PurchaseOrderSummary[]>(this.apiUrl);
  }

  getPurchaseOrder(id: number): Observable<PurchaseOrder> {
    return this.http.get<PurchaseOrder>(`${this.apiUrl}/${id}`);
  }

  getByStatus(status: string): Observable<PurchaseOrderSummary[]> {
    return this.http.get<PurchaseOrderSummary[]>(`${this.apiUrl}/status/${status}`);
  }

  createPurchaseOrder(dto: CreatePurchaseOrderDto): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(this.apiUrl, dto);
  }

  updateDocuments(id: number, dto: UpdatePurchaseOrderDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/documents`, dto);
  }
}
