import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ToolRequest {
  id: number;
  toolId: number;
  toolNameAr: string;
  toolNameEn: string;
  quantityNeeded: number;
  workAreaId: number;
  workAreaNameAr: string;
  workAreaNameEn: string;
  requesterId: number;
  requesterNameAr: string;
  requestDate: string;
  reasonAr: string;
  reasonEn: string;
  status: string;
  isInStock: boolean;
  quantityFulfilled?: number;
  fulfilledDate?: string;
  stockVerificationNotes?: string;
}

export interface CreateToolRequestDto {
  toolId: number;
  quantityNeeded: number;
  workAreaId: number;
  reasonAr: string;
  reasonEn: string;
}

export interface StockVerificationDto {
  toolRequestId: number;
  isInStock: boolean;
  quantityFulfilled?: number;
  stockVerificationNotes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ToolRequestService {
  private apiUrl = '/api/toolrequests';

  constructor(private http: HttpClient) { }

  getToolRequests(status?: string): Observable<ToolRequest[]> {
    const url = status ? `${this.apiUrl}?status=${status}` : this.apiUrl;
    return this.http.get<ToolRequest[]>(url);
  }

  getToolRequest(id: number): Observable<ToolRequest> {
    return this.http.get<ToolRequest>(`${this.apiUrl}/${id}`);
  }

  createToolRequest(dto: CreateToolRequestDto): Observable<ToolRequest> {
    return this.http.post<ToolRequest>(this.apiUrl, dto);
  }

  verifyStock(dto: StockVerificationDto): Observable<ToolRequest> {
    return this.http.post<ToolRequest>(`${this.apiUrl}/verify-stock`, dto);
  }
}
