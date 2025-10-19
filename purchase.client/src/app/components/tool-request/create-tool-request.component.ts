import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToolRequestService, CreateToolRequestDto } from '../../services/tool-request.service';
import { MasterDataService, StockItem, WorkArea } from '../../services/master-data.service';

@Component({
  selector: 'app-create-tool-request',
  templateUrl: './create-tool-request.component.html',
  styleUrl: './create-tool-request.component.css'
})
export class CreateToolRequestComponent implements OnInit {
  stockItems: StockItem[] = [];
  workAreas: WorkArea[] = [];
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  formData: any = {
    toolName: '',
    workAreaName: '',
    quantityNeeded: 1,
    reasonAr: '',
    reasonEn: ''
  };

  constructor(
    private toolRequestService: ToolRequestService,
    private masterDataService: MasterDataService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadMasterData();
  }

  loadMasterData() {
    this.loading = true;
    // Load data for reference but don't require selection
    this.masterDataService.getStockItems().subscribe({
      next: (data) => {
        this.stockItems = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading stock items:', err);
        this.loading = false;
      }
    });
  }

  onSubmit() {
    if (!this.validateForm()) {
      return;
    }

    this.submitting = true;
    this.error = null;
    this.successMessage = null;

    this.toolRequestService.createToolRequest(this.formData).subscribe({
      next: (response) => {
        this.successMessage = 'تم إنشاء الطلب بنجاح!';
        this.submitting = false;
        
        // Redirect to list after 2 seconds
        setTimeout(() => {
          this.router.navigate(['/tool-requests']);
        }, 2000);
      },
      error: (err) => {
        console.error('Error creating tool request:', err);
        this.error = err.error?.message || 'حدث خطأ في إنشاء الطلب';
        this.submitting = false;
      }
    });
  }

  validateForm(): boolean {
    if (!this.formData.toolName || !this.formData.toolName.trim()) {
      this.error = 'الرجاء إدخال اسم الأداة';
      return false;
    }
    if (this.formData.quantityNeeded < 1) {
      this.error = 'الرجاء إدخال كمية صحيحة';
      return false;
    }
    if (!this.formData.workAreaName || !this.formData.workAreaName.trim()) {
      this.error = 'الرجاء إدخال اسم منطقة العمل';
      return false;
    }
    if (!this.formData.reasonAr.trim()) {
      this.error = 'الرجاء إدخال سبب الطلب';
      return false;
    }
    return true;
  }

  cancel() {
    this.router.navigate(['/tool-requests']);
  }
}
