# دليل الاختبار / Testing Guide

## نظام إدارة المشتريات / Purchase Management System

---

## نظرة عامة / Overview

هذا الدليل يوضح كيفية اختبار جميع ميزات نظام إدارة المشتريات من 8 خطوات.

This guide explains how to test all features of the 8-step Purchase Management System.

---

## البيانات الاختبارية / Test Data

النظام يأتي مع بيانات اختبارية كاملة تشمل:

The system comes with complete test data including:

### مستخدمون / Users

| Username | Role | Password | Arabic Name | English Name |
|----------|------|----------|-------------|--------------|
| admin | Admin | hashed_password | أحمد محمود | Ahmed Mahmoud |
| requester1 | Requester | hashed_password | محمد علي | Mohamed Ali |
| stockmgr | StockManager | hashed_password | سارة أحمد | Sara Ahmed |
| approver1 | FirstApprover | hashed_password | خالد حسن | Khaled Hassan |
| approver2 | SecondApprover | hashed_password | فاطمة عبدالله | Fatima Abdullah |
| finance | FinancialOfficer | hashed_password | عمر إبراهيم | Omar Ibrahim |

### أدوات المخزون / Stock Items

| Tool ID | Name (AR) | Name (EN) | Current Qty | Min Threshold |
|---------|-----------|-----------|-------------|---------------|
| TOOL-001 | مفك براغي كهربائي | Electric Screwdriver | 5 | 10 |
| TOOL-002 | مثقاب كهربائي | Electric Drill | 2 | 8 |
| TOOL-003 | منشار كهربائي | Electric Saw | 5* | 5 |
| TOOL-004 | مطرقة | Hammer | 25 | 15 |
| TOOL-005 | شريط قياس | Measuring Tape | 3 | 20 |
| TOOL-006 | كماشة | Pliers | 0 | 10 |

*تم تحديثها من 0 إلى 5 بعد استكمال سير العمل النموذجي

*Updated from 0 to 5 after completing sample workflow

### موردون / Suppliers

| ID | Name (AR) | Name (EN) | Contact | Rating |
|----|-----------|-----------|---------|--------|
| 1 | شركة الأدوات المتقدمة | Advanced Tools Company | +966-12-3456789 | 4.5 |
| 2 | مؤسسة العدد الصناعية | Industrial Equipment Corporation | +966-11-7654321 | 4.2 |
| 3 | شركة الأدوات الدولية | International Tools Company | +966-13-9876543 | 4.8 |
| 4 | مركز المعدات الحديثة | Modern Equipment Center | +966-14-5432109 | 4.0 |

---

## اختبار سير العمل الكامل / Complete Workflow Testing

### سيناريو 1: سير عمل كامل (موجود بالفعل في البيانات)

الطلب: 5 مناشير كهربائية - تم إكماله بنجاح

Scenario 1: Complete Workflow (Already in data)
Request: 5 Electric Saws - Successfully Completed

#### الحالة الحالية / Current Status:
- ✅ طلب الأداة (Tool Request): OutOfStock
- ✅ طلب الشراء (Purchase Request): Approved
- ✅ عروض الأسعار (Quotations): 4 suppliers, Top 3 ranked, Best selected
- ✅ أمر الشراء (Purchase Order): PO-2025-0001, Status: Paid
- ✅ استلام البضائع (Goods Receipt): Received & Inspected, Stock Updated
- ✅ الدفع (Payment): TXN-20251001, Status: Completed

#### التحقق / Verification:

```bash
# Check Purchase Order
curl http://localhost:5274/api/purchaseorders/1

# Check Goods Receipt
curl http://localhost:5274/api/goodsreceipts/1

# Check Payment
curl http://localhost:5274/api/payments/1

# Verify Stock was updated
curl http://localhost:5274/api/stockitems | grep -A5 "Electric Saw"
```

---

### سيناريو 2: اختبار طلب جديد (خطوة بخطوة)

Scenario 2: Test New Request (Step by Step)

#### الخطوة 1: إنشاء طلب أداة جديد

Step 1: Create New Tool Request

```bash
curl -X POST http://localhost:5274/api/toolrequests \
  -H "Content-Type: application/json" \
  -d '{
    "toolId": 6,
    "quantityNeeded": 15,
    "workAreaId": 2,
    "reasonAr": "نحتاج كماشات للصيانة الدورية",
    "reasonEn": "We need pliers for routine maintenance"
  }'
```

**النتيجة المتوقعة / Expected Result:**
- Status: "Pending"
- Tool: Pliers (TOOL-006)
- Quantity: 15

#### الخطوة 2: التحقق من المخزون

Step 2: Verify Stock

```bash
curl -X POST http://localhost:5274/api/toolrequests/verify-stock \
  -H "Content-Type: application/json" \
  -d '{
    "toolRequestId": <NEW_REQUEST_ID>,
    "isInStock": false,
    "stockVerificationNotes": "غير متوفر - يتطلب الشراء من الموردين"
  }'
```

**النتيجة المتوقعة / Expected Result:**
- Status updated to: "OutOfStock"
- IsInStock: false

#### الخطوة 3: إنشاء طلب شراء

Step 3: Create Purchase Request

```bash
curl -X POST http://localhost:5274/api/purchaserequests \
  -H "Content-Type: application/json" \
  -d '{
    "toolRequestId": <REQUEST_ID>,
    "estimatedBudget": 700.00
  }'
```

**النتيجة المتوقعة / Expected Result:**
- Status: "PendingApproval"
- EstimatedBudget: 700.00

#### الموافقة على طلب الشراء

Approve Purchase Request

```bash
curl -X POST http://localhost:5274/api/purchaserequests/approve \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseRequestId": <PR_ID>,
    "isApproved": true,
    "approvedById": 4,
    "approvalNotes": "موافق عليه - حسب الميزانية"
  }'
```

**النتيجة المتوقعة / Expected Result:**
- Status: "Approved"
- ApprovedBy: Khaled Hassan

#### الخطوة 4: إضافة عروض الأسعار (3+ موردين)

Step 4: Add Quotations (3+ suppliers)

```bash
# Supplier 1
curl -X POST http://localhost:5274/api/quotations \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseRequestId": <PR_ID>,
    "supplierId": 1,
    "quantityOffered": 15,
    "unitPrice": 48.00,
    "deliveryTimeDays": 7,
    "notes": "جودة ممتازة"
  }'

# Supplier 2
curl -X POST http://localhost:5274/api/quotations \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseRequestId": <PR_ID>,
    "supplierId": 2,
    "quantityOffered": 15,
    "unitPrice": 45.00,
    "deliveryTimeDays": 10,
    "notes": "أفضل سعر"
  }'

# Supplier 3
curl -X POST http://localhost:5274/api/quotations \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseRequestId": <PR_ID>,
    "supplierId": 3,
    "quantityOffered": 15,
    "unitPrice": 47.50,
    "deliveryTimeDays": 5,
    "notes": "توصيل سريع"
  }'
```

**التحقق من أفضل 3 عروض / Verify Top 3:**

```bash
curl http://localhost:5274/api/quotations/purchase-request/<PR_ID>/top3
```

**النتيجة المتوقعة / Expected Result:**
- Top 3 quotations ranked by price
- Ranking: 1, 2, 3

#### الخطوة 5: اختيار أفضل عرض

Step 5: Select Best Offer

```bash
curl -X POST http://localhost:5274/api/quotations/select \
  -H "Content-Type: application/json" \
  -d '{
    "quotationId": <BEST_QUOTATION_ID>,
    "selectionReason": "أفضل سعر مع جودة جيدة"
  }'
```

**النتيجة المتوقعة / Expected Result:**
- IsSelected: true
- SelectedBy: SecondApprover

#### الخطوة 6: إنشاء أمر شراء

Step 6: Create Purchase Order

```bash
curl -X POST http://localhost:5274/api/purchaseorders \
  -H "Content-Type: application/json" \
  -d '{
    "quotationId": <SELECTED_QUOTATION_ID>,
    "expectedDeliveryDate": "2025-11-01T00:00:00Z",
    "notes": "أمر شراء للكماشات"
  }'
```

**النتيجة المتوقعة / Expected Result:**
- OrderNumber: PO-2025-XXXX (auto-generated)
- Status: "Created"
- TotalAmount: calculated from quotation

#### تحميل المستندات / Upload Documents

```bash
curl -X PUT http://localhost:5274/api/purchaseorders/<PO_ID>/documents \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseOrderDocument": "documents/po/PO-2025-XXXX.pdf",
    "invoiceDocument": "documents/invoices/INV-2025-XXXX.pdf",
    "deliveryNoteDocument": "documents/delivery/DN-2025-XXXX.pdf",
    "supplierAgreementDocument": "documents/agreements/AGR-2025-XXXX.pdf"
  }'
```

#### الخطوة 7: إنشاء إيصال استلام البضائع

Step 7: Create Goods Receipt

```bash
curl -X POST http://localhost:5274/api/goodsreceipts \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseOrderId": <PO_ID>,
    "receivedQuantity": 15,
    "qualityStatus": "Accepted",
    "qualityNotes": "جودة ممتازة - جميع الوحدات فحصت بنجاح",
    "conditionNotes": "لا توجد أضرار",
    "receivedById": 3
  }'
```

**النتيجة المتوقعة / Expected Result:**
- ExpectedQuantity: 15
- ReceivedQuantity: 15
- QuantityVariancePercentage: 0%
- IsQuantityAcceptable: true
- StockUpdated: true (automatic if quality = Accepted)
- PO Status updated to: "Inspected"

**التحقق من تحديث المخزون / Verify Stock Update:**

```bash
curl http://localhost:5274/api/stockitems | grep -A5 "Pliers"
# Should show CurrentQuantity: 15 (was 0 before)
```

#### الخطوة 8: إنشاء دفعة

Step 8: Create Payment

```bash
curl -X POST http://localhost:5274/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "goodsReceiptId": <GR_ID>,
    "amount": 675.00,
    "paymentDate": "2025-10-20T00:00:00Z",
    "paymentMethod": "Bank Transfer",
    "processedById": 6,
    "verificationNotes": "جميع المستندات صحيحة"
  }'
```

**النتيجة المتوقعة / Expected Result:**
- Status: "Pending"
- TransactionReference: TXN-YYYYMMXXXX (auto-generated)
- All verification flags: false

#### التحقق من الدفع / Verify Payment

```bash
curl -X POST http://localhost:5274/api/payments/<PAYMENT_ID>/verify \
  -H "Content-Type: application/json" \
  -d '{
    "documentsVerified": true,
    "quantityVerified": true,
    "priceVerified": true,
    "verificationNotes": "تم التحقق من جميع المستندات والكميات والأسعار"
  }'
```

#### معالجة الدفع / Process Payment

```bash
curl -X POST http://localhost:5274/api/payments/<PAYMENT_ID>/process \
  -H "Content-Type: application/json" \
  -d '{
    "transactionReference": "TXN-20251020001",
    "paymentDate": "2025-10-20T00:00:00Z",
    "paymentMethod": "Bank Transfer"
  }'
```

**النتيجة المتوقعة / Expected Result:**
- Status: "Completed"
- ProcessedDate: current date
- PO Status updated to: "Paid"

---

## اختبار حالات الحدود / Edge Cases Testing

### 1. اختبار تحمل الكمية (±5%)

Test Quantity Tolerance (±5%)

#### سيناريو أ: ضمن التحمل

Scenario A: Within Tolerance

```bash
# Expected: 100, Received: 103 (3% variance - Acceptable)
curl -X POST http://localhost:5274/api/goodsreceipts \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseOrderId": <PO_ID>,
    "receivedQuantity": 103,
    "qualityStatus": "Accepted",
    "receivedById": 3
  }'
```

**النتيجة / Result:** IsQuantityAcceptable = true

#### سيناريو ب: خارج التحمل

Scenario B: Outside Tolerance

```bash
# Expected: 100, Received: 107 (7% variance - Not Acceptable)
curl -X POST http://localhost:5274/api/goodsreceipts \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseOrderId": <PO_ID>,
    "receivedQuantity": 107,
    "qualityStatus": "Accepted",
    "receivedById": 3
  }'
```

**النتيجة / Result:** IsQuantityAcceptable = false, StockUpdated = false

### 2. اختبار رفض الجودة

Test Quality Rejection

```bash
curl -X POST http://localhost:5274/api/goodsreceipts \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseOrderId": <PO_ID>,
    "receivedQuantity": 15,
    "qualityStatus": "Rejected",
    "qualityNotes": "جودة غير مقبولة - تالف",
    "receivedById": 3
  }'
```

**النتيجة / Result:** StockUpdated = false (even if quantity is acceptable)

### 3. اختبار القبول الجزئي

Test Partial Acceptance

```bash
curl -X POST http://localhost:5274/api/goodsreceipts \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseOrderId": <PO_ID>,
    "receivedQuantity": 12,
    "qualityStatus": "PartiallyAccepted",
    "qualityNotes": "3 وحدات تالفة من أصل 15",
    "receivedById": 3
  }'
```

**النتيجة / Result:** StockUpdated = false (manual approval required)

---

## اختبار واجهة المستخدم / UI Testing

### 1. اختبار دعم اللغة العربية و RTL

Test Arabic Language & RTL Support

1. افتح المتصفح على: https://localhost:61710
   Open browser at: https://localhost:61710

2. تحقق من:
   Verify:
   - ✓ الاتجاه من اليمين إلى اليسار / Direction is Right-to-Left
   - ✓ النصوص بالعربية / Text is in Arabic
   - ✓ القوائم محاذاة على اليمين / Menus aligned to right
   - ✓ الأزرار في الأماكن الصحيحة / Buttons in correct positions

### 2. اختبار التنقل

Test Navigation

القائمة الرئيسية يجب أن تحتوي على:

Main menu should contain:

- لوحة التحكم / Dashboard
- طلبات الأدوات / Tool Requests
- طلبات الشراء / Purchase Requests
- عروض الأسعار / Quotations
- أوامر الشراء / Purchase Orders
- استلام البضائع / Goods Receipts
- المدفوعات / Payments

### 3. اختبار القوائم

Test Lists

لكل قائمة، تحقق من:

For each list, verify:

- ✓ عرض البيانات بشكل صحيح / Data displays correctly
- ✓ الفلترة حسب الحالة تعمل / Status filtering works
- ✓ الترقيم الصفحات (إن وجد) / Pagination (if implemented)
- ✓ الألوان الصحيحة للحالات / Correct status colors

---

## اختبار الأداء / Performance Testing

### 1. اختبار الحمل

Load Testing

```bash
# Test multiple requests
for i in {1..100}; do
  curl -s http://localhost:5274/api/purchaseorders > /dev/null &
done
wait
echo "Load test completed"
```

### 2. اختبار الاستجابة

Response Time Testing

```bash
# Measure API response time
time curl http://localhost:5274/api/purchaseorders/1
```

**النتيجة المتوقعة / Expected Result:** < 500ms

---

## المشاكل الشائعة وحلولها / Common Issues & Solutions

### 1. قاعدة البيانات لم يتم إنشاؤها

Database not created

**الحل / Solution:**
```bash
cd Purchase.Server
rm -f PurchaseWorkflow.db*
dotnet run
```

### 2. Angular لا يعمل

Angular not working

**الحل / Solution:**
```bash
cd purchase.client
rm -rf node_modules package-lock.json
npm install
npm start
```

### 3. خطأ CORS

CORS Error

**الحل / Solution:**
تأكد من أن Angular يعمل على المنفذ الصحيح (61710)

Ensure Angular is running on correct port (61710)

---

## ملخص قائمة الاختبار / Testing Checklist

### Backend API
- [ ] All endpoints respond correctly
- [ ] Data validation works
- [ ] Status transitions work
- [ ] Stock updates work
- [ ] Database seeding works

### Frontend
- [ ] All pages load correctly
- [ ] Arabic RTL displays correctly
- [ ] Navigation works
- [ ] Lists display data
- [ ] Filtering works

### Workflow
- [ ] Complete workflow from Step 1 to Step 8
- [ ] Stock verification logic
- [ ] Approval workflow
- [ ] Quotation ranking
- [ ] Goods receipt with tolerance
- [ ] Payment verification

### Edge Cases
- [ ] Quantity variance outside ±5%
- [ ] Quality rejection
- [ ] Partial acceptance
- [ ] Multiple quotations
- [ ] Duplicate requests

---

## الدعم / Support

للحصول على الدعم أو الإبلاغ عن مشاكل:

For support or to report issues:

- GitHub Issues: https://github.com/mostafaelsayad745/Purchase/issues
- Email: mostafaelsayad745@users.noreply.github.com

---

**نهاية دليل الاختبار / End of Testing Guide**
