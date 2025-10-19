import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PaymentService, PaymentSummary } from '../../services/payment.service';

@Component({
  selector: 'app-payment-list',
  templateUrl: './payment-list.component.html',
  styleUrls: ['./payment-list.component.css']
})
export class PaymentListComponent implements OnInit {
  payments: PaymentSummary[] = [];
  loading = false;
  error: string | null = null;
  selectedStatus = '';

  constructor(
    private paymentService: PaymentService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadPayments();
  }

  loadPayments(): void {
    this.loading = true;
    this.error = null;

    this.paymentService.getPayments().subscribe({
      next: (data) => {
        this.payments = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل المدفوعات';
        this.loading = false;
        console.error('Error loading payments:', err);
      }
    });
  }

  filterByStatus(): void {
    if (this.selectedStatus) {
      this.loading = true;
      this.paymentService.getByStatus(this.selectedStatus).subscribe({
        next: (data) => {
          this.payments = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'فشل في تحميل المدفوعات';
          this.loading = false;
          console.error('Error loading payments:', err);
        }
      });
    } else {
      this.loadPayments();
    }
  }

  createNew(): void {
    this.router.navigate(['/payments/create']);
  }

  viewDetails(id: number): void {
    this.router.navigate(['/payments', id]);
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'status-pending';
      case 'Completed': return 'status-fulfilled';
      case 'Failed': return 'status-rejected';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'قيد الانتظار';
      case 'Completed': return 'مكتمل';
      case 'Failed': return 'فشل';
      default: return status;
    }
  }

  getPaymentMethodText(method: string): string {
    switch (method) {
      case 'Bank Transfer': return 'تحويل بنكي';
      case 'Check': return 'شيك';
      case 'Cash': return 'نقدي';
      default: return method;
    }
  }
}
