import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PurchaseOrderService, PurchaseOrderSummary } from '../../services/purchase-order.service';

@Component({
  selector: 'app-purchase-order-list',
  templateUrl: './purchase-order-list.component.html',
  styleUrls: ['./purchase-order-list.component.css']
})
export class PurchaseOrderListComponent implements OnInit {
  purchaseOrders: PurchaseOrderSummary[] = [];
  loading = false;
  error: string | null = null;
  selectedStatus = '';

  constructor(
    private purchaseOrderService: PurchaseOrderService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadPurchaseOrders();
  }

  loadPurchaseOrders(): void {
    this.loading = true;
    this.error = null;

    this.purchaseOrderService.getPurchaseOrders().subscribe({
      next: (data) => {
        this.purchaseOrders = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل أوامر الشراء';
        this.loading = false;
        console.error('Error loading purchase orders:', err);
      }
    });
  }

  filterByStatus(): void {
    if (this.selectedStatus) {
      this.loading = true;
      this.purchaseOrderService.getByStatus(this.selectedStatus).subscribe({
        next: (data) => {
          this.purchaseOrders = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'فشل في تحميل أوامر الشراء';
          this.loading = false;
          console.error('Error loading purchase orders:', err);
        }
      });
    } else {
      this.loadPurchaseOrders();
    }
  }

  createNew(): void {
    this.router.navigate(['/purchase-orders/create']);
  }

  viewDetails(id: number): void {
    this.router.navigate(['/purchase-orders', id]);
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Created': return 'status-pending';
      case 'Received': return 'status-in-progress';
      case 'Inspected': return 'status-approved';
      case 'Paid': return 'status-fulfilled';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Created': return 'تم الإنشاء';
      case 'Received': return 'تم الاستلام';
      case 'Inspected': return 'تم الفحص';
      case 'Paid': return 'تم الدفع';
      default: return status;
    }
  }
}
