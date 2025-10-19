import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PurchaseRequestService, PurchaseRequest } from '../../services/purchase-request.service';

@Component({
  selector: 'app-purchase-request-list',
  templateUrl: './purchase-request-list.component.html',
  styleUrl: './purchase-request-list.component.css'
})
export class PurchaseRequestListComponent implements OnInit {
  purchaseRequests: PurchaseRequest[] = [];
  loading = false;
  error: string | null = null;
  selectedStatus = '';

  constructor(
    private purchaseRequestService: PurchaseRequestService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadPurchaseRequests();
  }

  loadPurchaseRequests() {
    this.loading = true;
    this.error = null;

    const status = this.selectedStatus || undefined;
    this.purchaseRequestService.getPurchaseRequests(status).subscribe({
      next: (data) => {
        this.purchaseRequests = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading purchase requests:', err);
        this.error = 'حدث خطأ في تحميل طلبات الشراء';
        this.loading = false;
      }
    });
  }

  filterByStatus() {
    this.loadPurchaseRequests();
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'PendingApproval': return 'في انتظار الموافقة';
      case 'Approved': return 'موافق عليه';
      case 'Rejected': return 'مرفوض';
      default: return status;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'PendingApproval': return 'badge-warning';
      case 'Approved': return 'badge-success';
      case 'Rejected': return 'badge-danger';
      default: return 'badge-secondary';
    }
  }

  viewDetails(id: number) {
    this.router.navigate(['/purchase-requests', id]);
  }

  approveRequest(request: PurchaseRequest) {
    if (confirm(`هل تريد الموافقة على طلب الشراء للأداة: ${request.toolNameAr}؟`)) {
      this.purchaseRequestService.approvePurchaseRequest({
        purchaseRequestId: request.id,
        isApproved: true,
        notes: 'تمت الموافقة'
      }).subscribe({
        next: () => {
          alert('تمت الموافقة على الطلب بنجاح');
          this.loadPurchaseRequests();
        },
        error: (err) => {
          console.error('Error approving request:', err);
          alert('حدث خطأ أثناء الموافقة على الطلب');
        }
      });
    }
  }

  rejectRequest(request: PurchaseRequest) {
    const reason = prompt(`سبب رفض طلب الشراء للأداة: ${request.toolNameAr}`);
    if (reason) {
      this.purchaseRequestService.approvePurchaseRequest({
        purchaseRequestId: request.id,
        isApproved: false,
        rejectionReason: reason
      }).subscribe({
        next: () => {
          alert('تم رفض الطلب بنجاح');
          this.loadPurchaseRequests();
        },
        error: (err) => {
          console.error('Error rejecting request:', err);
          alert('حدث خطأ أثناء رفض الطلب');
        }
      });
    }
  }
}
