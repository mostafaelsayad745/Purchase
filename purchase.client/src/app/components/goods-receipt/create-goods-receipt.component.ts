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
  selectedFile: File | null = null;
  uploadedFiles: string[] = [];

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

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      // Simulate file upload - in a real app, you would upload to a server
      const fileName = `receipt_${Date.now()}_${file.name}`;
      this.uploadedFiles.push(fileName);
      this.formData.proofOfReceiptDocument = this.uploadedFiles.join(', ');
      alert(`تم تحميل الملف: ${file.name}`);
    }
  }

  removeFile(fileName: string): void {
    this.uploadedFiles = this.uploadedFiles.filter(f => f !== fileName);
    this.formData.proofOfReceiptDocument = this.uploadedFiles.join(', ');
  }

  printReceipt(): void {
    if (!this.formData.purchaseOrderId) {
      alert('الرجاء إنشاء الإيصال أولاً');
      return;
    }
    
    const printWindow = window.open('', '_blank');
    if (printWindow) {
      const selectedOrder = this.purchaseOrders.find(o => o.id === this.formData.purchaseOrderId);
      
      printWindow.document.write(`
        <!DOCTYPE html>
        <html dir="rtl" lang="ar">
        <head>
          <meta charset="UTF-8">
          <title>إيصال استلام البضائع</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 20px; direction: rtl; }
            h1 { text-align: center; color: #2c3e50; }
            .section { margin: 20px 0; padding: 15px; border: 1px solid #ddd; border-radius: 5px; }
            .section h2 { color: #3498db; margin-top: 0; }
            .field { margin: 10px 0; }
            .field strong { display: inline-block; width: 200px; }
            .files-list { list-style: none; padding: 0; }
            .files-list li { padding: 5px 0; border-bottom: 1px solid #eee; }
            @media print {
              button { display: none; }
            }
          </style>
        </head>
        <body>
          <h1>📦 إيصال استلام البضائع</h1>
          
          <div class="section">
            <h2>معلومات الطلب</h2>
            <div class="field"><strong>رقم أمر الشراء:</strong> ${selectedOrder?.orderNumber || '-'}</div>
            <div class="field"><strong>اسم الأداة:</strong> ${selectedOrder?.toolNameAr || '-'}</div>
            <div class="field"><strong>تاريخ الاستلام:</strong> ${new Date().toLocaleDateString('ar-SA')}</div>
          </div>
          
          <div class="section">
            <h2>تفاصيل الاستلام</h2>
            <div class="field"><strong>الكمية المستلمة:</strong> ${this.formData.receivedQuantity}</div>
            <div class="field"><strong>حالة الجودة:</strong> ${this.getQualityStatusText(this.formData.qualityStatus)}</div>
            <div class="field"><strong>ملاحظات الجودة:</strong> ${this.formData.qualityNotes || '-'}</div>
            <div class="field"><strong>ملاحظات الحالة:</strong> ${this.formData.conditionNotes || '-'}</div>
          </div>
          
          <div class="section">
            <h2>المستندات المرفقة</h2>
            <ul class="files-list">
              ${this.uploadedFiles.length > 0 ? 
                this.uploadedFiles.map(f => `<li>📎 ${f}</li>`).join('') : 
                '<li>لا توجد ملفات مرفقة</li>'
              }
            </ul>
          </div>
          
          <div class="section">
            <div class="field"><strong>التوقيع:</strong> _________________</div>
            <div class="field"><strong>التاريخ:</strong> _________________</div>
          </div>
          
          <button onclick="window.print()" style="padding: 10px 20px; background: #3498db; color: white; border: none; border-radius: 5px; cursor: pointer; margin-top: 20px;">
            طباعة
          </button>
        </body>
        </html>
      `);
      printWindow.document.close();
    }
  }

  getQualityStatusText(status: string): string {
    switch (status) {
      case 'Accepted': return 'مقبول';
      case 'Rejected': return 'مرفوض';
      case 'PartiallyAccepted': return 'مقبول جزئياً';
      default: return status;
    }
  }
}
