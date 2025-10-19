import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PurchaseRequestService, PurchaseRequest, CreatePurchaseRequestDto } from '../../services/purchase-request.service';
import { ToolRequestService, ToolRequest } from '../../services/tool-request.service';

@Component({
  selector: 'app-purchase-request-list',
  templateUrl: './purchase-request-list.component.html',
  styleUrl: './purchase-request-list.component.css'
})
export class PurchaseRequestListComponent implements OnInit {
  purchaseRequests: PurchaseRequest[] = [];
  outOfStockToolRequests: ToolRequest[] = [];
  loading = false;
  creating = false;
  error: string | null = null;
  selectedStatus = '';
  showCreateForm = false;
  
  newPurchaseRequest: CreatePurchaseRequestDto = {
    toolRequestId: 0,
    estimatedBudget: 0
  };

  constructor(
    private purchaseRequestService: PurchaseRequestService,
    private toolRequestService: ToolRequestService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadPurchaseRequests();
    this.loadOutOfStockToolRequests();
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

  loadOutOfStockToolRequests() {
    this.toolRequestService.getToolRequests('OutOfStock').subscribe({
      next: (data) => {
        this.outOfStockToolRequests = data;
      },
      error: (err) => {
        console.error('Error loading tool requests:', err);
      }
    });
  }

  createPurchaseRequest() {
    if (this.newPurchaseRequest.toolRequestId === 0) {
      alert('الرجاء اختيار طلب الأداة');
      return;
    }

    this.creating = true;
    this.purchaseRequestService.createPurchaseRequest(this.newPurchaseRequest).subscribe({
      next: () => {
        alert('تم إنشاء طلب الشراء بنجاح');
        this.showCreateForm = false;
        this.resetForm();
        this.loadPurchaseRequests();
        this.creating = false;
      },
      error: (err) => {
        console.error('Error creating purchase request:', err);
        alert('حدث خطأ في إنشاء طلب الشراء');
        this.creating = false;
      }
    });
  }

  resetForm() {
    this.newPurchaseRequest = {
      toolRequestId: 0,
      estimatedBudget: 0
    };
  }
}
