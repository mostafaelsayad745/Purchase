# Quick Start Guide
# دليل البدء السريع

## Get Started in 5 Minutes! / ابدأ في 5 دقائق!

### Prerequisites / المتطلبات

Make sure you have these installed:
تأكد من تثبيت التالي:

- ✅ .NET SDK 9.0 or later
- ✅ Node.js 20.x or later
- ✅ npm 10.x or later

Check versions / تحقق من الإصدارات:
```bash
dotnet --version
node --version
npm --version
```

---

## Step 1: Clone Repository / استنساخ المستودع

```bash
git clone https://github.com/mostafaelsayad745/Purchase.git
cd Purchase
```

---

## Step 2: Setup Backend / إعداد الخلفية

```bash
# Navigate to backend
cd Purchase.Server

# Restore packages
dotnet restore

# Run the application
dotnet run
```

**✅ Backend is ready!** API running at `https://localhost:7274`

**الخلفية جاهزة!** واجهة برمجة التطبيقات تعمل على `https://localhost:7274`

---

## Step 3: Setup Frontend / إعداد الواجهة الأمامية

Open a **NEW terminal window** / افتح نافذة طرفية **جديدة**

```bash
# Navigate to frontend
cd purchase.client

# Install dependencies (first time only)
npm install

# Start development server
npm start
```

**✅ Frontend is ready!** Application running at `https://localhost:61710`

**الواجهة الأمامية جاهزة!** التطبيق يعمل على `https://localhost:61710`

---

## Step 4: Access the Application / الوصول إلى التطبيق

Open your browser and go to:
افتح المتصفح واذهب إلى:

```
https://localhost:61710
```

You should see the **Dashboard** with:
يجب أن ترى **لوحة التحكم** مع:

- 📊 Statistics cards / بطاقات الإحصائيات
- 🔄 8-step workflow overview / نظرة عامة على سير العمل من 8 خطوات
- ⚠️ Low stock alerts / تنبيهات المخزون المنخفض

---

## Step 5: Try Creating a Tool Request / جرّب إنشاء طلب أداة

1. Click **"إنشاء طلب جديد"** (Create New Request)
2. Fill in the form:
   - اختر الأداة (Select tool)
   - أدخل الكمية (Enter quantity)
   - اختر منطقة العمل (Select work area)
   - أدخل السبب (Enter reason)
3. Click **"حفظ الطلب"** (Save Request)

**🎉 Congratulations!** You've created your first tool request!

**مبروك!** لقد أنشأت طلب الأداة الأول الخاص بك!

---

## What's Available / ما هو متاح

### Current Features / الميزات الحالية

✅ **Dashboard (لوحة التحكم)**
   - View statistics
   - See pending requests
   - Check low stock items

✅ **Tool Requests (طلبات الأدوات)**
   - Create new requests
   - View all requests
   - Filter by status

✅ **Stock Management (إدارة المخزون)**
   - View stock items
   - See current quantities
   - Low stock alerts

---

## Sample Data / البيانات النموذجية

The system comes with pre-loaded data:
يأتي النظام ببيانات محملة مسبقاً:

### Stock Items / أصناف المخزون
- مفك براغي كهربائي (Electric Screwdriver)
- مثقاب كهربائي (Electric Drill)
- منشار كهربائي (Electric Saw)
- مطرقة (Hammer)
- شريط قياس (Measuring Tape)
- كماشة (Pliers)

### Work Areas / مناطق العمل
- ورشة الإنتاج (Production Workshop)
- قسم الصيانة (Maintenance Department)
- المخازن (Warehouse)
- قسم الجودة (Quality Department)

### Suppliers / الموردون
- شركة الأدوات المتقدمة
- مؤسسة العدد الصناعية
- شركة الأدوات الدولية
- مركز المعدات الحديثة

---

## Troubleshooting / حل المشاكل

### Backend not starting? / الخلفية لا تبدأ؟

Check if port 7274 is available:
تحقق من توفر المنفذ 7274:
```bash
netstat -ano | findstr :7274
```

### Frontend not starting? / الواجهة الأمامية لا تبدأ؟

Try clearing npm cache:
جرّب مسح ذاكرة التخزين المؤقت لـ npm:
```bash
npm cache clean --force
npm install
```

### Database errors? / أخطاء قاعدة البيانات؟

Delete the database and restart:
احذف قاعدة البيانات وأعد التشغيل:
```bash
# The database will be recreated automatically
# سيتم إعادة إنشاء قاعدة البيانات تلقائياً
```

---

## Next Steps / الخطوات التالية

### Learn More / تعلم المزيد
- 📖 Read the full [README.md](README.md)
- 🔄 Check the [WORKFLOW.md](WORKFLOW.md) for workflow details
- 💻 Review [IMPLEMENTATION.md](IMPLEMENTATION.md) for technical details

### Try These Features / جرّب هذه الميزات
1. Create multiple tool requests
2. Filter requests by status
3. Check the low stock alerts
4. Navigate between different pages

### Explore the Code / استكشف الكود
- Backend Controllers: `Purchase.Server/Controllers/`
- Frontend Components: `purchase.client/src/app/components/`
- Database Models: `Purchase.Server/Models/`
- API Services: `purchase.client/src/app/services/`

---

## Need Help? / تحتاج مساعدة؟

- 📧 Create an issue on GitHub
- 💬 Check existing documentation
- 🔍 Review the code comments

---

## Quick Commands Reference / مرجع الأوامر السريعة

### Backend / الخلفية
```bash
dotnet restore          # Restore packages
dotnet build           # Build project
dotnet run             # Run application
dotnet watch run       # Run with hot reload
```

### Frontend / الواجهة الأمامية
```bash
npm install            # Install dependencies
npm start              # Start dev server
npm run build          # Build for production
npm test               # Run tests
```

---

## Architecture Overview / نظرة عامة على البنية

```
┌─────────────────────────────────────────────────┐
│           Browser / المتصفح                      │
│         https://localhost:61710                 │
└─────────────────────────────────────────────────┘
                      ↕
┌─────────────────────────────────────────────────┐
│        Angular 17 Frontend                      │
│     (RTL Arabic Interface)                      │
│     - Components                                │
│     - Services                                  │
│     - Routing                                   │
└─────────────────────────────────────────────────┘
                      ↕
             HTTPS REST API
                      ↕
┌─────────────────────────────────────────────────┐
│      ASP.NET Core 9.0 Backend                   │
│         https://localhost:7274                  │
│     - Controllers                               │
│     - DTOs                                      │
│     - Business Logic                            │
└─────────────────────────────────────────────────┘
                      ↕
┌─────────────────────────────────────────────────┐
│    Entity Framework Core 9.0                    │
│         (ORM Layer)                             │
└─────────────────────────────────────────────────┘
                      ↕
┌─────────────────────────────────────────────────┐
│       SQL Server LocalDB                        │
│      (PurchaseWorkflowDb)                       │
│     - 10 Tables                                 │
│     - Relationships                             │
│     - Seed Data                                 │
└─────────────────────────────────────────────────┘
```

---

## Success! / نجح!

If you can see the dashboard with Arabic text and RTL layout, you're all set! 🎉

إذا كان بإمكانك رؤية لوحة التحكم بنص عربي وتخطيط RTL، فأنت جاهز! 🎉

**Welcome to the Purchase Management System!**

**مرحباً بك في نظام إدارة المشتريات!**

---

**Last Updated:** 2025-10-19
**Version:** 1.0.0
