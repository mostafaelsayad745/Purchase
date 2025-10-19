import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PurchaseOrderService, CreatePurchaseOrderDto } from '../../services/purchase-order.service';
import { QuotationService, Quotation } from '../../services/quotation.service';

@Component({
  selector: 'app-create-purchase-order',
  templateUrl: './create-purchase-order.component.html',
  styleUrls: ['./create-purchase-order.component.css']
})
export class CreatePurchaseOrderComponent implements OnInit {
  selectedQuotations: Quotation[] = [];
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  formData: CreatePurchaseOrderDto = {
    quotationId: 0,
    expectedDeliveryDate: '',
    notes: ''
  };

  constructor(
    private purchaseOrderService: PurchaseOrderService,
    private quotationService: QuotationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadSelectedQuotations();
  }

  loadSelectedQuotations(): void {
    this.loading = true;
    this.error = null;

    this.quotationService.getQuotations().subscribe({
      next: (data) => {
        this.selectedQuotations = data.filter(q => q.isSelected);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل عروض الأسعار المحددة';
        this.loading = false;
        console.error('Error loading selected quotations:', err);
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

    this.purchaseOrderService.createPurchaseOrder(this.formData).subscribe({
      next: (response) => {
        this.successMessage = 'تم إنشاء أمر الشراء بنجاح!';
        this.submitting = false;

        setTimeout(() => {
          this.router.navigate(['/purchase-orders']);
        }, 2000);
      },
      error: (err) => {
        console.error('Error creating purchase order:', err);
        this.error = err.error?.message || 'حدث خطأ في إنشاء أمر الشراء';
        this.submitting = false;
      }
    });
  }

  validateForm(): boolean {
    if (this.formData.quotationId === 0) {
      this.error = 'الرجاء اختيار عرض السعر';
      return false;
    }
    if (!this.formData.expectedDeliveryDate) {
      this.error = 'الرجاء إدخال تاريخ التسليم المتوقع';
      return false;
    }
    return true;
  }

  cancel(): void {
    this.router.navigate(['/purchase-orders']);
  }
}
