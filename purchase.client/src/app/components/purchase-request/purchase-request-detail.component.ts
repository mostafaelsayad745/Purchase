import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PurchaseRequestService, PurchaseRequest } from '../../services/purchase-request.service';

@Component({
  selector: 'app-purchase-request-detail',
  templateUrl: './purchase-request-detail.component.html',
  styleUrl: './purchase-request-detail.component.css'
})
export class PurchaseRequestDetailComponent implements OnInit {
  purchaseRequest: PurchaseRequest | null = null;
  loading = false;
  error: string | null = null;
  requestId: number = 0;

  constructor(
    private purchaseRequestService: PurchaseRequestService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.requestId = +params['id'];
      this.loadPurchaseRequest();
    });
  }

  loadPurchaseRequest() {
    this.loading = true;
    this.error = null;

    this.purchaseRequestService.getPurchaseRequest(this.requestId).subscribe({
      next: (data) => {
        this.purchaseRequest = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading purchase request:', err);
        this.error = 'حدث خطأ في تحميل تفاصيل طلب الشراء';
        this.loading = false;
      }
    });
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
      case 'PendingApproval': return 'status-pending';
      case 'Approved': return 'status-approved';
      case 'Rejected': return 'status-rejected';
      default: return '';
    }
  }

  goBack() {
    this.router.navigate(['/purchase-requests']);
  }
}
