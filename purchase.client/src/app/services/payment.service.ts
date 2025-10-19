import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Payment {
  id: number;
  goodsReceiptId: number;
  transactionReference: string;
  amount: number;
  paymentDate: string;
  paymentMethod: string;
  status: string;
  documentsVerified: boolean;
  quantityVerified: boolean;
  priceVerified: boolean;
  processedById?: number;
  processedByNameAr?: string;
  processedByNameEn?: string;
  processedDate?: string;
  verificationNotes?: string;
  isArchived: boolean;
  archiveDate?: string;
  createdAt: string;
  updatedAt: string;
  goodsReceipt?: any;
}

export interface PaymentSummary {
  id: number;
  orderNumber: string;
  transactionReference: string;
  amount: number;
  paymentDate: string;
  paymentMethod: string;
  status: string;
  supplierNameAr: string;
  supplierNameEn: string;
}

export interface CreatePaymentDto {
  goodsReceiptId: number;
  amount: number;
  paymentDate: string;
  paymentMethod: string;
  processedById: number;
  verificationNotes?: string;
}

export interface VerifyPaymentDto {
  documentsVerified: boolean;
  quantityVerified: boolean;
  priceVerified: boolean;
  verificationNotes?: string;
}

export interface ProcessPaymentDto {
  transactionReference: string;
  paymentDate: string;
  paymentMethod: string;
}

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private apiUrl = `${environment.apiUrl}/payments`;

  constructor(private http: HttpClient) { }

  getPayments(): Observable<PaymentSummary[]> {
    return this.http.get<PaymentSummary[]>(this.apiUrl);
  }

  getPayment(id: number): Observable<Payment> {
    return this.http.get<Payment>(`${this.apiUrl}/${id}`);
  }

  getByStatus(status: string): Observable<PaymentSummary[]> {
    return this.http.get<PaymentSummary[]>(`${this.apiUrl}/status/${status}`);
  }

  getPendingVerification(): Observable<Payment[]> {
    return this.http.get<Payment[]>(`${this.apiUrl}/pending-verification`);
  }

  createPayment(dto: CreatePaymentDto): Observable<Payment> {
    return this.http.post<Payment>(this.apiUrl, dto);
  }

  verifyPayment(id: number, dto: VerifyPaymentDto): Observable<{ message: string; allVerified: boolean }> {
    return this.http.post<{ message: string; allVerified: boolean }>(`${this.apiUrl}/${id}/verify`, dto);
  }

  processPayment(id: number, dto: ProcessPaymentDto): Observable<{ message: string; transactionReference: string }> {
    return this.http.post<{ message: string; transactionReference: string }>(`${this.apiUrl}/${id}/process`, dto);
  }

  archivePayment(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/archive`, {});
  }
}
