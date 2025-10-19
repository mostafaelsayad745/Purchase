import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Quotation {
  id: number;
  purchaseRequestId: number;
  supplierId: number;
  supplierNameAr: string;
  supplierNameEn: string;
  quantityOffered: number;
  unitPrice: number;
  totalPrice: number;
  deliveryTimeDays: number;
  notes?: string;
  quotationDate: string;
  ranking?: number;
  customScore?: number;
  isSelected: boolean;
  selectionReason?: string;
  rejectionReason?: string;
}

export interface CreateQuotationDto {
  purchaseRequestId: number;
  supplierId: number;
  quantityOffered: number;
  unitPrice: number;
  deliveryTimeDays: number;
  notes?: string;
}

export interface SelectQuotationDto {
  quotationId: number;
  selectionReason?: string;
}

export interface RejectQuotationDto {
  quotationId: number;
  rejectionReason: string;
}

@Injectable({
  providedIn: 'root'
})
export class QuotationService {
  private apiUrl = `${environment.apiUrl}/quotations`;

  constructor(private http: HttpClient) { }

  getQuotations(purchaseRequestId?: number): Observable<Quotation[]> {
    const url = purchaseRequestId ? `${this.apiUrl}?purchaseRequestId=${purchaseRequestId}` : this.apiUrl;
    return this.http.get<Quotation[]>(url);
  }

  getTop3Quotations(purchaseRequestId: number): Observable<Quotation[]> {
    return this.http.get<Quotation[]>(`${this.apiUrl}/purchase-request/${purchaseRequestId}/top3`);
  }

  getQuotation(id: number): Observable<Quotation> {
    return this.http.get<Quotation>(`${this.apiUrl}/${id}`);
  }

  createQuotation(dto: CreateQuotationDto): Observable<Quotation> {
    return this.http.post<Quotation>(this.apiUrl, dto);
  }

  selectQuotation(dto: SelectQuotationDto): Observable<Quotation> {
    return this.http.post<Quotation>(`${this.apiUrl}/select`, dto);
  }

  rejectQuotation(dto: RejectQuotationDto): Observable<Quotation> {
    return this.http.post<Quotation>(`${this.apiUrl}/reject`, dto);
  }
}
