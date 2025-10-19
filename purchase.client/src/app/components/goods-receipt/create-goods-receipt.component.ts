import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GoodsReceiptService, CreateGoodsReceiptDto } from '../../services/goods-receipt.service';
import { PurchaseOrderService, PurchaseOrderSummary } from '../../services/purchase-order.service';

@Component({
  selector: 'app-create-goods-receipt',
  templateUrl: './create-goods-receipt.component.html',
  styleUrls: ['./create-goods-receipt.component.css']
})
export class CreateGoodsReceiptComponent implements OnInit {
  purchaseOrders: PurchaseOrderSummary[] = [];
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  formData: CreateGoodsReceiptDto = {
    purchaseOrderId: 0,
    receivedQuantity: 0,
    qualityStatus: 'Accepted',
    qualityNotes: '',
    conditionNotes: '',
    proofOfReceiptDocument: '',
    receivedById: 1 // Default user
  };

  constructor(
    private goodsReceiptService: GoodsReceiptService,
    private purchaseOrderService: PurchaseOrderService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadPurchaseOrders();
  }

  loadPurchaseOrders(): void {
    this.loading = true;
    this.error = null;

    this.purchaseOrderService.getByStatus('Created').subscribe({
      next: (data) => {
        this.purchaseOrders = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل أوامر الشراء';
        this.loading = false;
        console.error('Error loading purchase orders:', err);
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

    this.goodsReceiptService.createGoodsReceipt(this.formData).subscribe({
      next: (response) => {
        this.successMessage = 'تم إنشاء إيصال الاستلام بنجاح!';
        this.submitting = false;

        setTimeout(() => {
          this.router.navigate(['/goods-receipts']);
        }, 2000);
      },
      error: (err) => {
        console.error('Error creating goods receipt:', err);
        this.error = err.error?.message || 'حدث خطأ في إنشاء إيصال الاستلام';
        this.submitting = false;
      }
    });
  }

  validateForm(): boolean {
    if (this.formData.purchaseOrderId === 0) {
      this.error = 'الرجاء اختيار أمر الشراء';
      return false;
    }
    if (this.formData.receivedQuantity <= 0) {
      this.error = 'الرجاء إدخال كمية صحيحة';
      return false;
    }
    return true;
  }

  cancel(): void {
    this.router.navigate(['/goods-receipts']);
  }
}
