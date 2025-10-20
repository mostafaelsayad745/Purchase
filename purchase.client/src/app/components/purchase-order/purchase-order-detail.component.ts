import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PurchaseOrderService, PurchaseOrder } from '../../services/purchase-order.service';

@Component({
  selector: 'app-purchase-order-detail',
  templateUrl: './purchase-order-detail.component.html',
  styleUrls: ['./purchase-order-detail.component.css']
})
export class PurchaseOrderDetailComponent implements OnInit {
  purchaseOrder: PurchaseOrder | null = null;
  loading = false;
  error: string | null = null;
  editMode = false;
  saving = false;

  // Edit form data
  editData = {
    notes: '',
    purchaseOrderDocument: '',
    invoiceDocument: '',
    deliveryNoteDocument: '',
    supplierAgreementDocument: ''
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private purchaseOrderService: PurchaseOrderService
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadPurchaseOrder(id);
    }
  }

  loadPurchaseOrder(id: number): void {
    this.loading = true;
    this.error = null;

    this.purchaseOrderService.getPurchaseOrder(id).subscribe({
      next: (data) => {
        this.purchaseOrder = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل تفاصيل أمر الشراء';
        this.loading = false;
        console.error('Error loading purchase order:', err);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/purchase-orders']);
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Created': return 'تم الإنشاء';
      case 'Received': return 'تم الاستلام';
      case 'Inspected': return 'تم الفحص';
      case 'Paid': return 'تم الدفع';
      default: return status;
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Created': return 'status-pending';
      case 'Received': return 'status-in-progress';
      case 'Inspected': return 'status-approved';
      case 'Paid': return 'status-fulfilled';
      default: return '';
    }
  }

  enableEditMode(): void {
    this.editMode = true;
    if (this.purchaseOrder) {
      this.editData = {
        notes: this.purchaseOrder.notes || '',
        purchaseOrderDocument: this.purchaseOrder.purchaseOrderDocument || '',
        invoiceDocument: this.purchaseOrder.invoiceDocument || '',
        deliveryNoteDocument: this.purchaseOrder.deliveryNoteDocument || '',
        supplierAgreementDocument: this.purchaseOrder.supplierAgreementDocument || ''
      };
    }
  }

  cancelEdit(): void {
    this.editMode = false;
  }

  saveChanges(): void {
    if (!this.purchaseOrder) return;

    this.saving = true;
    this.purchaseOrderService.updateDocuments(this.purchaseOrder.id, this.editData).subscribe({
      next: () => {
        alert('تم حفظ التغييرات بنجاح');
        this.editMode = false;
        this.saving = false;
        this.loadPurchaseOrder(this.purchaseOrder!.id);
      },
      error: (err) => {
        console.error('Error saving changes:', err);
        alert('حدث خطأ في حفظ التغييرات');
        this.saving = false;
      }
    });
  }
}
