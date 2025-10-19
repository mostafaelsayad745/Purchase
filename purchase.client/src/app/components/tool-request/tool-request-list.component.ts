import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToolRequestService, ToolRequest } from '../../services/tool-request.service';

@Component({
  selector: 'app-tool-request-list',
  templateUrl: './tool-request-list.component.html',
  styleUrl: './tool-request-list.component.css'
})
export class ToolRequestListComponent implements OnInit {
  toolRequests: ToolRequest[] = [];
  loading = false;
  error: string | null = null;
  selectedStatus = '';

  constructor(
    private toolRequestService: ToolRequestService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadToolRequests();
  }

  loadToolRequests() {
    this.loading = true;
    this.error = null;

    const status = this.selectedStatus || undefined;
    this.toolRequestService.getToolRequests(status).subscribe({
      next: (data) => {
        this.toolRequests = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading tool requests:', err);
        this.error = 'حدث خطأ في تحميل طلبات الأدوات';
        this.loading = false;
      }
    });
  }

  filterByStatus() {
    this.loadToolRequests();
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
}
