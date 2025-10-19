import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GoodsReceiptService, GoodsReceipt } from '../../services/goods-receipt.service';

@Component({
  selector: 'app-goods-receipt-detail',
  templateUrl: './goods-receipt-detail.component.html',
  styleUrls: ['./goods-receipt-detail.component.css']
})
export class GoodsReceiptDetailComponent implements OnInit {
  goodsReceipt: GoodsReceipt | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private goodsReceiptService: GoodsReceiptService
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadGoodsReceipt(id);
    }
  }

  loadGoodsReceipt(id: number): void {
    this.loading = true;
    this.error = null;

    this.goodsReceiptService.getGoodsReceipt(id).subscribe({
      next: (data) => {
        this.goodsReceipt = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل تفاصيل إيصال الاستلام';
        this.loading = false;
        console.error('Error loading goods receipt:', err);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/goods-receipts']);
  }

  getQualityText(status: string): string {
    switch (status) {
      case 'Accepted': return 'مقبول';
      case 'Rejected': return 'مرفوض';
      case 'PartiallyAccepted': return 'مقبول جزئياً';
      default: return status;
    }
  }

  getQualityColor(status: string): string {
    switch (status) {
      case 'Accepted': return 'status-fulfilled';
      case 'Rejected': return 'status-rejected';
      case 'PartiallyAccepted': return 'status-pending';
      default: return '';
    }
  }
}
