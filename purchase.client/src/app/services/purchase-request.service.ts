import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PurchaseRequest {
  id: number;
  toolRequestId: number;
  toolNameAr: string;
  quantityNeeded: number;
  requestDate: string;
  status: string;
  approvedById?: number;
  approvedByNameAr?: string;
  approvalDate?: string;
  approvalNotes?: string;
  rejectionReason?: string;
  estimatedBudget?: number;
  notes?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface CreatePurchaseRequestDto {
  toolRequestId: number;
  estimatedBudget?: number;
}

export interface ApprovePurchaseRequestDto {
  purchaseRequestId: number;
  isApproved: boolean;
  notes?: string;
  rejectionReason?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseRequestService {
  private apiUrl = `${environment.apiUrl}/purchaserequests`;

  constructor(private http: HttpClient) { }

  getPurchaseRequests(status?: string): Observable<PurchaseRequest[]> {
    const url = status ? `${this.apiUrl}?status=${status}` : this.apiUrl;
    return this.http.get<PurchaseRequest[]>(url);
  }

  getPurchaseRequest(id: number): Observable<PurchaseRequest> {
    return this.http.get<PurchaseRequest>(`${this.apiUrl}/${id}`);
  }

  createPurchaseRequest(dto: CreatePurchaseRequestDto): Observable<PurchaseRequest> {
    return this.http.post<PurchaseRequest>(this.apiUrl, dto);
  }

  approvePurchaseRequest(dto: ApprovePurchaseRequestDto): Observable<PurchaseRequest> {
    return this.http.post<PurchaseRequest>(`${this.apiUrl}/approve`, dto);
  }
}
