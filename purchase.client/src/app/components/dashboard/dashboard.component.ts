import { Component, OnInit } from '@angular/core';
import { ToolRequestService, ToolRequest } from '../../services/tool-request.service';
import { PurchaseRequestService, PurchaseRequest } from '../../services/purchase-request.service';
import { MasterDataService, StockItem } from '../../services/master-data.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  pendingToolRequests: ToolRequest[] = [];
  pendingPurchaseRequests: PurchaseRequest[] = [];
  lowStockItems: StockItem[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private toolRequestService: ToolRequestService,
    private purchaseRequestService: PurchaseRequestService,
    private masterDataService: MasterDataService
  ) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    this.error = null;

    // Load pending tool requests
    this.toolRequestService.getToolRequests('Pending').subscribe({
      next: (data) => this.pendingToolRequests = data,
      error: (err) => console.error('Error loading tool requests:', err)
    });

    // Load pending purchase requests
    this.purchaseRequestService.getPurchaseRequests('PendingApproval').subscribe({
      next: (data) => this.pendingPurchaseRequests = data,
      error: (err) => console.error('Error loading purchase requests:', err)
    });

    // Load low stock items
    this.masterDataService.getLowStockItems().subscribe({
      next: (data) => {
        this.lowStockItems = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading low stock items:', err);
        this.error = 'حدث خطأ في تحميل البيانات';
        this.loading = false;
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'status-pending';
      case 'Approved': return 'status-approved';
      case 'Rejected': return 'status-rejected';
      case 'InStock': return 'status-in-stock';
      case 'OutOfStock': return 'status-out-of-stock';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'قيد الانتظار';
      case 'PendingApproval': return 'في انتظار الموافقة';
      case 'Approved': return 'موافق عليه';
      case 'Rejected': return 'مرفوض';
      case 'InStock': return 'متوفر في المخزون';
      case 'OutOfStock': return 'غير متوفر';
      default: return status;
    }
  }
}
