import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface GoodsReceipt {
  id: number;
  purchaseOrderId: number;
  receivedDate: string;
  receivedQuantity: number;
  expectedQuantity: number;
  quantityVariancePercentage: number;
  isQuantityAcceptable: boolean;
  qualityStatus: string;
  qualityNotes?: string;
  conditionNotes?: string;
  proofOfReceiptDocument?: string;
  receivedById?: number;
  receivedByNameAr?: string;
  receivedByNameEn?: string;
  stockUpdated: boolean;
  stockUpdateDate?: string;
  createdAt: string;
  updatedAt: string;
  purchaseOrder?: any;
  payment?: any;
}

export interface GoodsReceiptSummary {
  id: number;
  orderNumber: string;
  receivedDate: string;
  receivedQuantity: number;
  expectedQuantity: number;
  qualityStatus: string;
  stockUpdated: boolean;
  toolNameAr: string;
  toolNameEn: string;
}

export interface CreateGoodsReceiptDto {
  purchaseOrderId: number;
  receivedQuantity: number;
  qualityStatus: string;
  qualityNotes?: string;
  conditionNotes?: string;
  proofOfReceiptDocument?: string;
  receivedById: number;
}

export interface UpdateGoodsReceiptDto {
  qualityStatus: string;
  qualityNotes?: string;
  conditionNotes?: string;
  proofOfReceiptDocument?: string;
}

@Injectable({
  providedIn: 'root'
})
export class GoodsReceiptService {
  private apiUrl = '/api/goodsreceipts';

  constructor(private http: HttpClient) { }

  getGoodsReceipts(): Observable<GoodsReceiptSummary[]> {
    return this.http.get<GoodsReceiptSummary[]>(this.apiUrl);
  }

  getGoodsReceipt(id: number): Observable<GoodsReceipt> {
    return this.http.get<GoodsReceipt>(`${this.apiUrl}/${id}`);
  }

  createGoodsReceipt(dto: CreateGoodsReceiptDto): Observable<GoodsReceipt> {
    return this.http.post<GoodsReceipt>(this.apiUrl, dto);
  }

  updateGoodsReceipt(id: number, dto: UpdateGoodsReceiptDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  updateStock(id: number): Observable<{ message: string; newQuantity: number }> {
    return this.http.post<{ message: string; newQuantity: number }>(`${this.apiUrl}/${id}/update-stock`, {});
  }
}
