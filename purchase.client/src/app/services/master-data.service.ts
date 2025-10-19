import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface StockItem {
  id: number;
  toolId: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  currentQuantity: number;
  minimumThreshold: number;
  unit: string;
  lastPurchasePrice?: number;
  lastRestockDate?: string;
}

export interface WorkArea {
  id: number;
  nameAr: string;
  nameEn: string;
  description?: string;
  isActive: boolean;
}

export interface Supplier {
  id: number;
  nameAr: string;
  nameEn: string;
  contactPerson: string;
  phone: string;
  email: string;
  address?: string;
  taxNumber?: string;
  rating?: number;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class MasterDataService {
  constructor(private http: HttpClient) { }

  getStockItems(): Observable<StockItem[]> {
    return this.http.get<StockItem[]>('/api/stockitems');
  }

  getStockItem(id: number): Observable<StockItem> {
    return this.http.get<StockItem>(`/api/stockitems/${id}`);
  }

  getLowStockItems(): Observable<StockItem[]> {
    return this.http.get<StockItem[]>('/api/stockitems/low-stock');
  }

  getWorkAreas(): Observable<WorkArea[]> {
    return this.http.get<WorkArea[]>('/api/workareas');
  }

  getWorkArea(id: number): Observable<WorkArea> {
    return this.http.get<WorkArea>(`/api/workareas/${id}`);
  }

  getSuppliers(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>('/api/suppliers');
  }

  getSupplier(id: number): Observable<Supplier> {
    return this.http.get<Supplier>(`/api/suppliers/${id}`);
  }
}
