import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToolRequestService, ToolRequest } from '../../services/tool-request.service';
import { PurchaseRequestService } from '../../services/purchase-request.service';

@Component({
  selector: 'app-tool-request-list',
  templateUrl: './tool-request-list.component.html',
  styleUrl: './tool-request-list.component.css'
})
export class ToolRequestListComponent implements OnInit {
  toolRequests: ToolRequest[] = [];
  filteredRequests: ToolRequest[] = [];
  paginatedRequests: ToolRequest[] = [];
  loading = false;
  error: string | null = null;
  selectedStatus = '';
  filterToolName = '';
  filterDate = '';
  
  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  startIndex = 0;
  endIndex = 0;

  constructor(
    private toolRequestService: ToolRequestService,
    private purchaseRequestService: PurchaseRequestService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadToolRequests();
  }

  loadToolRequests() {
    this.loading = true;
    this.error = null;

    this.toolRequestService.getToolRequests().subscribe({
      next: (data) => {
        this.toolRequests = data;
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading tool requests:', err);
        this.error = 'حدث خطأ في تحميل طلبات الأدوات';
        this.loading = false;
      }
    });
  }

  applyFilters() {
    this.filteredRequests = this.toolRequests.filter(request => {
      const matchesStatus = !this.selectedStatus || request.status === this.selectedStatus;
      const matchesToolName = !this.filterToolName || 
        request.toolNameAr.includes(this.filterToolName) ||
        request.toolNameEn.toLowerCase().includes(this.filterToolName.toLowerCase());
      const matchesDate = !this.filterDate || 
        request.requestDate.startsWith(this.filterDate);
      
      return matchesStatus && matchesToolName && matchesDate;
    });
    
    this.currentPage = 1;
    this.updatePagination();
  }

  updatePagination() {
    this.totalPages = Math.ceil(this.filteredRequests.length / this.pageSize);
    this.startIndex = (this.currentPage - 1) * this.pageSize;
    this.endIndex = Math.min(this.startIndex + this.pageSize, this.filteredRequests.length);
    this.paginatedRequests = this.filteredRequests.slice(this.startIndex, this.endIndex);
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePagination();
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePagination();
    }
  }

  onPageSizeChange() {
    this.currentPage = 1;
    this.updatePagination();
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'قيد الانتظار';
      case 'InStock': return 'متوفر في المخزون';
      case 'OutOfStock': return 'غير متوفر';
      case 'Rejected': return 'مرفوض';
      default: return status;
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'status-pending';
      case 'InStock': return 'status-in-stock';
      case 'OutOfStock': return 'status-out-of-stock';
      case 'Rejected': return 'status-rejected';
      default: return '';
    }
  }

  createNew() {
    this.router.navigate(['/tool-requests/create']);
  }

  viewDetails(id: number) {
    // For now, just show an alert with details. In production, navigate to a detail page
    const request = this.toolRequests.find(r => r.id === id);
    if (request) {
      alert(`
تفاصيل الطلب:
رقم الطلب: ${request.id}
الأداة: ${request.toolNameAr}
الكمية: ${request.quantityNeeded}
منطقة العمل: ${request.workAreaNameAr}
مقدم الطلب: ${request.requesterNameAr}
التاريخ: ${new Date(request.requestDate).toLocaleString('ar-EG')}
الحالة: ${this.getStatusText(request.status)}
السبب: ${request.reasonAr}
      `);
    }
  }

  sendToPurchaseRequest(request: ToolRequest) {
    if (confirm(`هل تريد إرسال طلب الأداة "${request.toolNameAr}" إلى طلبات الشراء؟`)) {
      this.purchaseRequestService.createPurchaseRequest({
        toolRequestId: request.id,
        estimatedBudget: 0
      }).subscribe({
        next: (purchaseRequest) => {
          alert(`تم إنشاء طلب الشراء بنجاح (رقم ${purchaseRequest.id})`);
          this.loadToolRequests();
        },
        error: (err) => {
          console.error('Error creating purchase request:', err);
          alert('حدث خطأ في إنشاء طلب الشراء');
        }
      });
    }
  }
}
