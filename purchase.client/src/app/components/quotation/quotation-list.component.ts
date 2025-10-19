import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { QuotationService, Quotation } from '../../services/quotation.service';
import { PurchaseRequestService, PurchaseRequest } from '../../services/purchase-request.service';

@Component({
  selector: 'app-quotation-list',
  templateUrl: './quotation-list.component.html',
  styleUrl: './quotation-list.component.css'
})
export class QuotationListComponent implements OnInit {
  quotations: Quotation[] = [];
  purchaseRequests: PurchaseRequest[] = [];
  loading = false;
  error: string | null = null;
  selectedPurchaseRequestId: number | null = null;
  groupedQuotations: Map<number, Quotation[]> = new Map();

  constructor(
    private quotationService: QuotationService,
    private purchaseRequestService: PurchaseRequestService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadApprovedPurchaseRequests();
    this.loadQuotations();
  }

  loadApprovedPurchaseRequests() {
    this.purchaseRequestService.getPurchaseRequests('Approved').subscribe({
      next: (data) => {
        this.purchaseRequests = data;
      },
      error: (err) => {
        console.error('Error loading purchase requests:', err);
      }
    });
  }

  loadQuotations() {
    this.loading = true;
    this.error = null;

    this.quotationService.getQuotations(this.selectedPurchaseRequestId || undefined).subscribe({
      next: (data) => {
        this.quotations = data;
        this.groupQuotationsByPurchaseRequest();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading quotations:', err);
        this.error = 'حدث خطأ في تحميل عروض الأسعار';
        this.loading = false;
      }
    });
  }

  groupQuotationsByPurchaseRequest() {
    this.groupedQuotations.clear();
    this.quotations.forEach(q => {
      if (!this.groupedQuotations.has(q.purchaseRequestId)) {
        this.groupedQuotations.set(q.purchaseRequestId, []);
      }
      this.groupedQuotations.get(q.purchaseRequestId)!.push(q);
    });
  }

  filterByPurchaseRequest() {
    this.loadQuotations();
  }

  getRankingBadge(ranking: number | undefined): string {
    if (!ranking) return '';
    switch (ranking) {
      case 1: return '🥇 الأول';
      case 2: return '🥈 الثاني';
      case 3: return '🥉 الثالث';
      default: return `#${ranking}`;
    }
  }

  getRankingClass(ranking: number | undefined): string {
    if (!ranking) return '';
    switch (ranking) {
      case 1: return 'rank-1';
      case 2: return 'rank-2';
      case 3: return 'rank-3';
      default: return '';
    }
  }

  selectQuotation(quotation: Quotation) {
    const reason = prompt(`سبب اختيار عرض المورد: ${quotation.supplierNameAr}`);
    if (reason) {
      this.quotationService.selectQuotation({
        quotationId: quotation.id,
        selectionReason: reason
      }).subscribe({
        next: () => {
          alert('تم اختيار العرض بنجاح');
          this.loadQuotations();
        },
        error: (err) => {
          console.error('Error selecting quotation:', err);
          alert('حدث خطأ أثناء اختيار العرض');
        }
      });
    }
  }

  rejectQuotation(quotation: Quotation) {
    const reason = prompt(`سبب رفض عرض المورد: ${quotation.supplierNameAr}`);
    if (reason) {
      this.quotationService.rejectQuotation({
        quotationId: quotation.id,
        rejectionReason: reason
      }).subscribe({
        next: () => {
          alert('تم رفض العرض بنجاح');
          this.loadQuotations();
        },
        error: (err) => {
          console.error('Error rejecting quotation:', err);
          alert('حدث خطأ أثناء رفض العرض');
        }
      });
    }
  }

  getPurchaseRequestName(purchaseRequestId: number): string {
    const pr = this.purchaseRequests.find(p => p.id === purchaseRequestId);
    return pr ? pr.toolNameAr : `طلب شراء #${purchaseRequestId}`;
  }

  getGroupKeys(): number[] {
    return Array.from(this.groupedQuotations.keys());
  }
}
