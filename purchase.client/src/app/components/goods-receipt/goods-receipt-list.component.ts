import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GoodsReceiptService, GoodsReceiptSummary } from '../../services/goods-receipt.service';

@Component({
  selector: 'app-goods-receipt-list',
  templateUrl: './goods-receipt-list.component.html',
  styleUrls: ['./goods-receipt-list.component.css']
})
export class GoodsReceiptListComponent implements OnInit {
  goodsReceipts: GoodsReceiptSummary[] = [];
  loading = false;
  error: string | null = null;

  constructor(
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
        this.goodsReceipts = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل في تحميل إيصالات البضائع';
        this.loading = false;
        console.error('Error loading goods receipts:', err);
      }
    });
  }

  createNew(): void {
    this.router.navigate(['/goods-receipts/create']);
  }

  viewDetails(id: number): void {
    this.router.navigate(['/goods-receipts', id]);
  }

  getQualityColor(status: string): string {
    switch (status) {
      case 'Accepted': return 'status-fulfilled';
      case 'Rejected': return 'status-rejected';
      case 'PartiallyAccepted': return 'status-pending';
      default: return '';
    }
  }

  getQualityText(status: string): string {
    switch (status) {
      case 'Accepted': return 'مقبول';
      case 'Rejected': return 'مرفوض';
      case 'PartiallyAccepted': return 'مقبول جزئياً';
      default: return status;
    }
  }
}
