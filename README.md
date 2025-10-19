# نظام إدارة المشتريات - Purchase Management System

## نظرة عامة / Overview

نظام شامل لإدارة عملية الشراء من 8 خطوات، مصمم لتنظيم دورة الشراء الكاملة من الطلب الأولي حتى إتمام الدفع. النظام مبني باستخدام ASP.NET Core Web API في الخلفية و Angular في الواجهة الأمامية، مع دعم كامل للغة العربية والاتجاه من اليمين إلى اليسار (RTL).

A comprehensive 8-step Purchase Process Workflow system designed to manage the complete purchase cycle from initial request through payment completion. Built with ASP.NET Core Web API backend and Angular frontend, with full Arabic language and Right-to-Left (RTL) support.

---

## التقنيات المستخدمة / Technologies Used

### Backend (الخلفية)
- **ASP.NET Core 9.0** - Web API Framework
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server (LocalDB)** - Database engine
- **C# 12** - Programming language

### Frontend (الواجهة الأمامية)
- **Angular 17** - Frontend framework
- **TypeScript** - Programming language
- **Angular Material** - UI component library
- **RxJS** - Reactive programming
- **HTML5/CSS3** - Markup and styling with RTL support

---

## خطوات سير عمل الشراء / Purchase Workflow Steps

### الخطوة 1: الطلب الأولي / Step 1: Initial Request
**الوصف:** عملية تقديم طلب الأدوات من مناطق العمل إلى قسم المخزون

**Description:** Tool request submission process from work areas to stock department

**البيانات المطلوبة / Required Data:**
- معرف الأداة (ToolID)
- الكمية المطلوبة (QuantityNeeded)
- معرف منطقة العمل (WorkAreaID)
- تاريخ الطلب (RequestDate)
- السبب (Reason - Arabic & English)

**قواعد التحقق / Validation Rules:**
- يجب أن تكون الأداة موجودة في قاعدة البيانات
- يجب أن تكون الكمية أكبر من صفر
- يجب أن تكون منطقة العمل نشطة
- يجب تقديم سبب الطلب

**API Endpoint:**
```
POST /api/toolrequests
```

---

### الخطوة 2: التحقق من المخزون / Step 2: Stock Verification
**الوصف:** عملية فحص توفر الأداة في المخزون

**Description:** Inventory checking process

**منطق القرار / Decision Logic:**
- **إذا كان متوفراً (IN STOCK):** تلبية الطلب مباشرة وإخطار مقدم الطلب
- **إذا لم يكن متوفراً (OUT OF STOCK):** الانتقال إلى الخطوة 3

**If IN STOCK:** Fulfill directly and notify requester
**If OUT OF STOCK:** Proceed to Step 3

**معايير العتبة / Threshold Parameters:**
- الحد الأدنى للمخزون (MinimumThreshold)
- الكمية الحالية (CurrentQuantity)

**API Endpoint:**
```
POST /api/toolrequests/verify-stock
```

---

### الخطوة 3: موافقة طلب الشراء (الموافقة الأولى) / Step 3: Purchase Request Approval (First Approval)
**الوصف:** عملية إنشاء طلب شراء رسمي ومراجعة الإدارة

**Description:** Formal purchase request creation and management approval workflow

**سير عمل الموافقة / Approval Workflow:**
- الأدوار والصلاحيات (Roles & Permissions)
- المستندات المطلوبة (Required Documentation):
  - تاريخ الموافقة (Approval Date)
  - هوية الموافق (Approver Identity)
  - ملاحظات الموافقة (Approval Notes)

**معالجة الرفض / Rejection Handling:**
- سبب الرفض مطلوب
- إخطار مقدم الطلب
- إنهاء سير العمل

**API Endpoints:**
```
POST /api/purchaserequests
POST /api/purchaserequests/approve
```

---

### الخطوة 4: عرض أسعار الموردين والاختيار / Step 4: Supplier Quotation & Selection
**الوصف:** جمع عروض الأسعار من 3 موردين كحد أدنى

**Description:** Collect quotations from minimum 3 suppliers

**المتطلبات / Requirements:**
- الحد الأدنى للموردين: 3 موردين
- Minimum Suppliers: 3 suppliers

**بيانات عرض السعر / Quotation Data:**
- الكمية المعروضة (Quantity Offered)
- سعر الوحدة (Unit Price)
- السعر الإجمالي (Total Price)
- وقت التسليم (Delivery Time)
- تفاصيل المورد (Supplier Details)

**نظام الترتيب التلقائي / Automatic Ranking System:**
- ترتيب أفضل 3 عروض على أساس السعر
- Rank top 3 offers based on price
- خيار معايير مخصصة (Custom criteria option)
- درجة مخصصة (CustomScore)

**مسار التدقيق / Audit Trail:**
- تخزين كامل لجميع عروض الأسعار
- Complete storage for all quotations
- تتبع التغييرات (Change tracking)

**API Endpoints:**
```
POST /api/quotations
GET /api/quotations/purchase-request/{id}/top3
```

---

### الخطوة 5: موافقة أفضل عرض (الموافقة الثانية) / Step 5: Best Offer Approval (Second Approval)
**الوصف:** مراجعة الإدارة لأفضل 3 عروض

**Description:** Management review of top 3 offers

**عملية المراجعة / Review Process:**
- مصفوفة المقارنة (Comparison Matrix)
- معايير الاختيار (Selection Criteria):
  - السعر (Price)
  - وقت التسليم (Delivery Time)
  - تقييم المورد (Supplier Rating)
  - جودة المنتج (Product Quality)

**التوثيق / Documentation:**
- تفاصيل العرض المحدد (Selected offer details)
- أسباب الرفض للعروض غير المحددة
- Rejection reasons for non-selected offers

**API Endpoints:**
```
POST /api/quotations/select
POST /api/quotations/reject
```

---

### الخطوة 6: أمر الشراء وإدارة المستندات / Step 6: Purchase Order & Document Management
**الوصف:** إنشاء أمر الشراء من عرض السعر المحدد

**Description:** PO creation from selected quotation

**متطلبات تحميل المستندات / Document Upload Requirements:**
- أمر الشراء (Purchase Order - PO)
- الفاتورة (Invoice)
- مذكرة التسليم (Delivery Note)
- اتفاقية المورد (Supplier Agreement)

**تتبع حالة أمر الشراء / PO Status Tracking:**
- **تم الإنشاء (Created):** أمر الشراء أنشئ
- **تم الاستلام (Received):** البضائع استلمت
- **تم الفحص (Inspected):** البضائع فحصت
- **تم الدفع (Paid):** المدفوعات أكملت

**تحديد تاريخ التسليم المتوقع / Expected Delivery Date:**
- تاريخ التسليم المتوقع محدد عند الإنشاء
- Expected delivery date set at creation

---

### الخطوة 7: استلام البضائع وإدارة المخزون / Step 7: Goods Reception & Stock Management
**الوصف:** عملية استلام البضائع الفعلي بواسطة قسم المخزون

**Description:** Physical goods reception process by stock department

**إجراءات التحقق / Verification Procedures:**
- **فحص الكمية (Quantity Check):**
  - تحمل ±5% (±5% tolerance)
  - إذا ضمن التحمل: مقبول
  - If within tolerance: Acceptable
  
- **فحص الجودة (Quality Check):**
  - مقبول (Accepted)
  - مرفوض (Rejected)
  - مقبول جزئياً (Partially Accepted)

- **فحص الحالة (Condition Check):**
  - توثيق أي أضرار
  - Document any damages

**متطلبات التوثيق / Documentation Requirements:**
- إثبات الاستلام (Proof of Receipt)
- صور فوتوغرافية (Photographs)
- ملاحظات الفحص (Inspection Notes)

**تحديث نظام المخزون / Stock System Update:**
- تحديث الكمية تلقائياً
- Automatically update quantity
- تسجيل تاريخ إعادة التخزين
- Log restocking date

**تتبع الحالة / Status Tracking:**
- **تم الاستلام والفحص (Received & Inspected)**

---

### الخطوة 8: معالجة الدفع / Step 8: Payment Processing
**الوصف:** عملية مراجعة القسم المالي

**Description:** Financial department review process

**قائمة التحقق / Verification Checklist:**
- ✓ اكتمال المستندات (Document Completeness)
- ✓ مطابقة الكمية (Quantity Matching)
- ✓ دقة السعر (Price Accuracy)

**سير عمل معالجة الدفع / Payment Processing Workflow:**
1. مراجعة جميع المستندات
2. التحقق من مطابقة الكمية والسعر
3. الموافقة على الدفع
4. معالجة الدفع
5. تسجيل معاملة الدفع

**طرق الدفع / Payment Methods:**
- تحويل بنكي (Bank Transfer)
- شيك (Check)
- نقدي (Cash)

**تحديثات الحالة / Status Updates:**
- حالة أمر الشراء: مدفوع (PO Status: Paid)
- إنشاء سجل الدفع (Payment record creation)
- مرجع المعاملة (Transaction reference)

**إجراءات الأرشفة / Archival Procedures:**
- أرشفة دورة الشراء الكاملة
- Archive complete purchase cycle
- الاحتفاظ بجميع المستندات
- Retain all documentation
- إمكانية الوصول للتدقيق
- Audit trail accessibility

---

## هيكل قاعدة البيانات / Database Schema

### الجداول الرئيسية / Main Tables

#### WorkAreas (مناطق العمل)
- Id, NameAr, NameEn, Description, IsActive

#### Users (المستخدمون)
- Id, Username, FullNameAr, Email, Role, WorkAreaId

#### StockItems (أصناف المخزون)
- Id, ToolId, NameAr, NameEn, CurrentQuantity, MinimumThreshold

#### ToolRequests (طلبات الأدوات)
- Id, ToolId, QuantityNeeded, WorkAreaId, Status, IsInStock

#### PurchaseRequests (طلبات الشراء)
- Id, ToolRequestId, Status, ApprovedById, ApprovalDate

#### Suppliers (الموردون)
- Id, NameAr, NameEn, ContactPerson, Phone, Email, Rating

#### Quotations (عروض الأسعار)
- Id, PurchaseRequestId, SupplierId, UnitPrice, TotalPrice, Ranking

#### PurchaseOrders (أوامر الشراء)
- Id, OrderNumber, QuotationId, Status, TotalAmount

#### GoodsReceipts (إيصالات البضائع)
- Id, PurchaseOrderId, ReceivedQuantity, QualityStatus

#### Payments (المدفوعات)
- Id, GoodsReceiptId, Amount, PaymentMethod, Status

---

## التثبيت والإعداد / Installation & Setup

### المتطلبات الأساسية / Prerequisites

```bash
# .NET SDK 9.0
dotnet --version

# Node.js 20.x
node --version

# npm 10.x
npm --version
```

### خطوات التثبيت / Installation Steps

#### 1. استنساخ المستودع / Clone Repository
```bash
git clone https://github.com/mostafaelsayad745/Purchase.git
cd Purchase
```

#### 2. إعداد الخلفية / Backend Setup
```bash
cd Purchase.Server

# استعادة الحزم / Restore packages
dotnet restore

# بناء المشروع / Build project
dotnet build

# تشغيل التطبيق / Run application
dotnet run
```

**ملاحظة:** يستخدم النظام قاعدة بيانات SQLite التي سيتم إنشاؤها تلقائياً عند التشغيل الأول مع بيانات اختبارية.

**Note:** The system uses SQLite database which will be automatically created on first run with test data.

سيكون API متاحاً على / API will be available at:
- HTTPS: `https://localhost:7274`
- HTTP: `http://localhost:5274`

#### 3. إعداد الواجهة الأمامية / Frontend Setup
```bash
cd purchase.client

# تثبيت التبعيات / Install dependencies
npm install

# بناء التطبيق / Build application
npm run build

# تشغيل خادم التطوير / Run development server
npm start
```

سيكون التطبيق متاحاً على / Application will be available at:
- `https://localhost:61710`

---

## الاستخدام / Usage

### 1. إنشاء طلب أداة جديد / Create New Tool Request
```
الصفحة الرئيسية → طلبات الأدوات → إنشاء طلب جديد
Dashboard → Tool Requests → Create New Request
```

### 2. التحقق من المخزون / Verify Stock
```
طلبات الأدوات → اختيار الطلب → التحقق من المخزون
Tool Requests → Select Request → Verify Stock
```

### 3. إنشاء طلب شراء / Create Purchase Request
```
طلبات الشراء → إنشاء طلب جديد
Purchase Requests → Create New Request
```

### 4. إدارة عروض الأسعار / Manage Quotations
```
عروض الأسعار → إضافة عرض سعر جديد
Quotations → Add New Quotation
```

---

## نقاط نهاية API / API Endpoints

### Tool Requests (Step 1)
- `GET /api/toolrequests` - Get all tool requests
- `GET /api/toolrequests/{id}` - Get specific tool request
- `POST /api/toolrequests` - Create new tool request
- `POST /api/toolrequests/verify-stock` - Verify stock availability (Step 2)

### Purchase Requests (Step 3)
- `GET /api/purchaserequests` - Get all purchase requests
- `GET /api/purchaserequests/{id}` - Get specific purchase request
- `POST /api/purchaserequests` - Create new purchase request
- `POST /api/purchaserequests/approve` - Approve/reject purchase request

### Quotations (Steps 4-5)
- `GET /api/quotations` - Get all quotations
- `GET /api/quotations/{id}` - Get specific quotation
- `GET /api/quotations/purchase-request/{id}/top3` - Get top 3 quotations
- `POST /api/quotations` - Create new quotation
- `POST /api/quotations/select` - Select quotation (Step 5)
- `POST /api/quotations/reject` - Reject quotation

### Purchase Orders (Step 6)
- `GET /api/purchaseorders` - Get all purchase orders
- `GET /api/purchaseorders/{id}` - Get specific purchase order
- `GET /api/purchaseorders/status/{status}` - Get by status
- `POST /api/purchaseorders` - Create new purchase order
- `PUT /api/purchaseorders/{id}/documents` - Update documents

### Goods Receipts (Step 7)
- `GET /api/goodsreceipts` - Get all goods receipts
- `GET /api/goodsreceipts/{id}` - Get specific goods receipt
- `POST /api/goodsreceipts` - Create new goods receipt
- `PUT /api/goodsreceipts/{id}` - Update goods receipt
- `POST /api/goodsreceipts/{id}/update-stock` - Manually update stock

### Payments (Step 8)
- `GET /api/payments` - Get all payments
- `GET /api/payments/{id}` - Get specific payment
- `GET /api/payments/status/{status}` - Get by status
- `GET /api/payments/pending-verification` - Get pending verification
- `POST /api/payments` - Create new payment
- `POST /api/payments/{id}/verify` - Verify payment checklist
- `POST /api/payments/{id}/process` - Process payment
- `POST /api/payments/{id}/archive` - Archive payment

### Master Data
- `GET /api/stockitems` - Get all stock items
- `GET /api/stockitems/low-stock` - Get low stock items
- `GET /api/workareas` - Get all work areas
- `GET /api/suppliers` - Get all suppliers

---

## الميزات / Features

✅ **دعم كامل للغة العربية** / Full Arabic Language Support
✅ **واجهة RTL (من اليمين إلى اليسار)** / RTL Interface
✅ **8 خطوات سير عمل شراء شاملة** / 8-Step Comprehensive Purchase Workflow
✅ **إدارة المخزون التلقائية** / Automatic Stock Management
✅ **نظام موافقة متعدد المستويات** / Multi-Level Approval System
✅ **إدارة عروض الأسعار** / Quotation Management
✅ **ترتيب تلقائي لأفضل العروض** / Automatic Best Offer Ranking
✅ **إدارة المستندات** / Document Management
✅ **مسار تدقيق كامل** / Complete Audit Trail
✅ **تتبع الحالة** / Status Tracking
✅ **لوحة معلومات تفاعلية** / Interactive Dashboard

---

## البيانات الابتدائية / Seed Data

يتم تحميل النظام ببيانات ابتدائية تشمل:

The system comes pre-loaded with seed data including:

- 4 مناطق عمل / 4 Work Areas
- 6 مستخدمين بأدوار مختلفة / 6 Users with different roles
- 6 أصناف مخزون / 6 Stock Items
- 4 موردين / 4 Suppliers

**أدوار المستخدمين / User Roles:**
- Admin (المدير)
- Requester (مقدم الطلب)
- StockManager (مدير المخزون)
- FirstApprover (الموافق الأول)
- SecondApprover (الموافق الثاني)
- FinancialOfficer (الموظف المالي)

---

## المساهمة / Contributing

نرحب بالمساهمات! يرجى اتباع الخطوات التالية:

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## الترخيص / License

هذا المشروع مرخص بموجب رخصة MIT

This project is licensed under the MIT License

---

## الاتصال / Contact

**مصطفى السيد** / Mostafa Elsayad
- GitHub: [@mostafaelsayad745](https://github.com/mostafaelsayad745)

---

## الحالة / Status

🚀 **النظام جاهز للاستخدام - كامل** / System Ready for Use - Complete

- ✅ Backend API Complete (8 Steps)
- ✅ Frontend Dashboard Complete
- ✅ Tool Request Management Complete (Step 1)
- ✅ Stock Verification Complete (Step 2)
- ✅ Purchase Request Management Complete (Step 3)
- ✅ Quotation Management Complete (Step 4-5)
- ✅ Purchase Order Management Complete (Step 6)
- ✅ Goods Receipt Management Complete (Step 7)
- ✅ Payment Processing Complete (Step 8)
- ✅ Database Seeding with Complete Workflow
- ✅ Arabic RTL Support
- ✅ SQLite Database (Cross-platform)

---

## الإصدار / Version

**v1.0.0** - Complete 8-Step Purchase Workflow

### What's Included in v1.0.0:
- ✅ Full 8-step purchase workflow implementation
- ✅ Arabic RTL support across all components
- ✅ Complete backend API with all controllers
- ✅ Complete frontend with Angular components
- ✅ SQLite database with automatic seeding
- ✅ Sample workflow data for testing
- ✅ Document management support
- ✅ Automatic stock updates with ±5% tolerance
- ✅ Multi-level approval system
- ✅ Payment verification checklist
- ✅ Complete audit trail
- ✅ Status tracking across all steps

---

## شكر خاص / Acknowledgments

- ASP.NET Core Team
- Angular Team
- Entity Framework Core Team
- جميع المساهمين في المشروع / All project contributors
