import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { QuotationService, Quotation, CreateQuotationDto } from '../../services/quotation.service';
import { PurchaseRequestService, PurchaseRequest } from '../../services/purchase-request.service';
import { MasterDataService, Supplier } from '../../services/master-data.service';

@Component({
  selector: 'app-quotation-list',
  templateUrl: './quotation-list.component.html',
  styleUrl: './quotation-list.component.css'
})
export class QuotationListComponent implements OnInit {
  quotations: Quotation[] = [];
  filteredQuotations: Quotation[] = [];
  purchaseRequests: PurchaseRequest[] = [];
  suppliers: Supplier[] = [];
  loading = false;
  creating = false;
  error: string | null = null;
  selectedPurchaseRequestId: number | null = null;
  groupedQuotations: Map<number, Quotation[]> = new Map();
  showCreateForm = false;
  filterSupplierName = '';
  filterDate = '';
  
  newQuotation: CreateQuotationDto = {
    purchaseRequestId: 0,
    supplierId: 0,
    quantityOffered: 0,
    unitPrice: 0,
    deliveryTimeDays: 0,
    notes: ''
  };

  constructor(
    private quotationService: QuotationService,
    private purchaseRequestService: PurchaseRequestService,
    private masterDataService: MasterDataService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadApprovedPurchaseRequests();
    this.loadQuotations();
    this.loadSuppliers();
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
        this.filteredQuotations = data;
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
    this.filteredQuotations.forEach(q => {
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

  loadSuppliers() {
    this.masterDataService.getSuppliers().subscribe({
      next: (data) => {
        this.suppliers = data;
      },
      error: (err) => {
        console.error('Error loading suppliers:', err);
      }
    });
  }

  createQuotation() {
    if (this.newQuotation.purchaseRequestId === 0 || this.newQuotation.supplierId === 0) {
      alert('الرجاء ملء جميع الحقول المطلوبة');
      return;
    }

    this.creating = true;
    this.quotationService.createQuotation(this.newQuotation).subscribe({
      next: () => {
        alert('تم إضافة عرض السعر بنجاح');
        this.showCreateForm = false;
        this.resetForm();
        this.loadQuotations();
        this.creating = false;
      },
      error: (err) => {
        console.error('Error creating quotation:', err);
        alert('حدث خطأ في إضافة عرض السعر');
        this.creating = false;
      }
    });
  }

  resetForm() {
    this.newQuotation = {
      purchaseRequestId: 0,
      supplierId: 0,
      quantityOffered: 0,
      unitPrice: 0,
      deliveryTimeDays: 0,
      notes: ''
    };
  }

  applyFilters() {
    this.filteredQuotations = this.quotations.filter(q => {
      const matchesSupplier = !this.filterSupplierName || 
        q.supplierNameAr.includes(this.filterSupplierName) || 
        q.supplierNameEn.toLowerCase().includes(this.filterSupplierName.toLowerCase());
      
      const matchesDate = !this.filterDate || 
        q.quotationDate.startsWith(this.filterDate);
      
      return matchesSupplier && matchesDate;
    });
    this.groupQuotationsByPurchaseRequest();
  }
}
