import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentService, Payment } from '../../services/payment.service';

@Component({
  selector: 'app-payment-detail',
  templateUrl: './payment-detail.component.html',
  styleUrls: ['./payment-detail.component.css']
})
export class PaymentDetailComponent implements OnInit {
  payment: Payment | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private paymentService: PaymentService
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadPayment(id);
    }
  }

  loadPayment(id: number): void {
    this.loading = true;
    this.error = null;

    this.paymentService.getPayment(id).subscribe({
      next: (data) => {
        this.payment = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل تفاصيل الدفعة';
        this.loading = false;
        console.error('Error loading payment:', err);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/payments']);
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'قيد الانتظار';
      case 'Completed': return 'مكتمل';
      case 'Failed': return 'فشل';
      default: return status;
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'status-pending';
      case 'Completed': return 'status-fulfilled';
      case 'Failed': return 'status-rejected';
      default: return '';
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
