import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PaymentService, CreatePaymentDto } from '../../services/payment.service';
import { GoodsReceiptService, GoodsReceiptSummary } from '../../services/goods-receipt.service';

@Component({
  selector: 'app-create-payment',
  templateUrl: './create-payment.component.html',
  styleUrls: ['./create-payment.component.css']
})
export class CreatePaymentComponent implements OnInit {
  goodsReceipts: GoodsReceiptSummary[] = [];
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  formData: CreatePaymentDto = {
    goodsReceiptId: 0,
    amount: 0,
    paymentDate: '',
    paymentMethod: 'Bank Transfer',
    processedById: 1, // Default user
    verificationNotes: ''
  };

  constructor(
    private paymentService: PaymentService,
    private goodsReceiptService: GoodsReceiptService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadGoodsReceipts();
  }

  loadGoodsReceipts(): void {
    this.loading = true;
    this.error = null;

    this.goodsReceiptService.getGoodsReceipts().subscribe({
      next: (data) => {
        this.goodsReceipts = data.filter(gr => gr.stockUpdated);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل إيصالات الاستلام';
        this.loading = false;
        console.error('Error loading goods receipts:', err);
      }
    });
  }

  onSubmit(): void {
    if (!this.validateForm()) {
      return;
    }

    this.submitting = true;
    this.error = null;
    this.successMessage = null;

    this.paymentService.createPayment(this.formData).subscribe({
      next: (response) => {
        this.successMessage = 'تم إنشاء الدفعة بنجاح!';
        this.submitting = false;

        setTimeout(() => {
          this.router.navigate(['/payments']);
        }, 2000);
      },
      error: (err) => {
        console.error('Error creating payment:', err);
        this.error = err.error?.message || 'حدث خطأ في إنشاء الدفعة';
        this.submitting = false;
      }
    });
  }

  validateForm(): boolean {
    if (this.formData.goodsReceiptId === 0) {
      this.error = 'الرجاء اختيار إيصال الاستلام';
      return false;
    }
    if (this.formData.amount <= 0) {
      this.error = 'الرجاء إدخال مبلغ صحيح';
      return false;
    }
    if (!this.formData.paymentDate) {
      this.error = 'الرجاء إدخال تاريخ الدفع';
      return false;
    }
    return true;
  }

  cancel(): void {
    this.router.navigate(['/payments']);
  }
}
