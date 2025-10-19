using Microsoft.EntityFrameworkCore;
using Purchase.Server.Models;

namespace Purchase.Server.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(PurchaseDbContext context)
    {
        // Check if data already exists
        if (context.WorkAreas.Any())
        {
            return; // Database has been seeded
        }

        // Seed Work Areas
        var workAreas = new List<WorkArea>
        {
            new WorkArea { NameAr = "ورشة الإنتاج", NameEn = "Production Workshop", Description = "Main production area", IsActive = true },
            new WorkArea { NameAr = "قسم الصيانة", NameEn = "Maintenance Department", Description = "Equipment maintenance", IsActive = true },
            new WorkArea { NameAr = "المخازن", NameEn = "Warehouse", Description = "Storage and inventory", IsActive = true },
            new WorkArea { NameAr = "قسم الجودة", NameEn = "Quality Department", Description = "Quality control", IsActive = true }
        };
        context.WorkAreas.AddRange(workAreas);
        await context.SaveChangesAsync();

        // Seed Users
        var users = new List<User>
        {
            new User 
            { 
                Username = "admin", 
                FullNameAr = "أحمد محمود", 
                FullNameEn = "Ahmed Mahmoud", 
                Email = "admin@purchase.com",
                PasswordHash = "hashed_password", // In real app, use proper password hashing
                Role = "Admin",
                WorkAreaId = 1,
                IsActive = true
            },
            new User 
            { 
                Username = "requester1", 
                FullNameAr = "محمد علي", 
                FullNameEn = "Mohamed Ali", 
                Email = "requester@purchase.com",
                PasswordHash = "hashed_password",
                Role = "Requester",
                WorkAreaId = 1,
                IsActive = true
            },
            new User 
            { 
                Username = "stockmgr", 
                FullNameAr = "سارة أحمد", 
                FullNameEn = "Sara Ahmed", 
                Email = "stock@purchase.com",
                PasswordHash = "hashed_password",
                Role = "StockManager",
                WorkAreaId = 3,
                IsActive = true
            },
            new User 
            { 
                Username = "approver1", 
                FullNameAr = "خالد حسن", 
                FullNameEn = "Khaled Hassan", 
                Email = "approver1@purchase.com",
                PasswordHash = "hashed_password",
                Role = "FirstApprover",
                IsActive = true
            },
            new User 
            { 
                Username = "approver2", 
                FullNameAr = "فاطمة عبدالله", 
                FullNameEn = "Fatima Abdullah", 
                Email = "approver2@purchase.com",
                PasswordHash = "hashed_password",
                Role = "SecondApprover",
                IsActive = true
            },
            new User 
            { 
                Username = "finance", 
                FullNameAr = "عمر إبراهيم", 
                FullNameEn = "Omar Ibrahim", 
                Email = "finance@purchase.com",
                PasswordHash = "hashed_password",
                Role = "FinancialOfficer",
                IsActive = true
            }
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // Seed Stock Items
        var stockItems = new List<StockItem>
        {
            new StockItem 
            { 
                ToolId = "TOOL-001", 
                NameAr = "مفك براغي كهربائي", 
                NameEn = "Electric Screwdriver",
                DescriptionAr = "مفك براغي كهربائي قوة 500 واط",
                DescriptionEn = "Electric screwdriver 500W",
                CurrentQuantity = 5,
                MinimumThreshold = 10,
                Unit = "قطعة",
                LastPurchasePrice = 150.00m
            },
            new StockItem 
            { 
                ToolId = "TOOL-002", 
                NameAr = "مثقاب كهربائي", 
                NameEn = "Electric Drill",
                DescriptionAr = "مثقاب كهربائي 750 واط",
                DescriptionEn = "Electric drill 750W",
                CurrentQuantity = 2,
                MinimumThreshold = 8,
                Unit = "قطعة",
                LastPurchasePrice = 250.00m
            },
            new StockItem 
            { 
                ToolId = "TOOL-003", 
                NameAr = "منشار كهربائي", 
                NameEn = "Electric Saw",
                DescriptionAr = "منشار كهربائي دائري",
                DescriptionEn = "Circular electric saw",
                CurrentQuantity = 0,
                MinimumThreshold = 5,
                Unit = "قطعة",
                LastPurchasePrice = 400.00m
            },
            new StockItem 
            { 
                ToolId = "TOOL-004", 
                NameAr = "مطرقة", 
                NameEn = "Hammer",
                DescriptionAr = "مطرقة حديدية 1 كجم",
                DescriptionEn = "Iron hammer 1kg",
                CurrentQuantity = 25,
                MinimumThreshold = 15,
                Unit = "قطعة",
                LastPurchasePrice = 30.00m
            },
            new StockItem 
            { 
                ToolId = "TOOL-005", 
                NameAr = "شريط قياس", 
                NameEn = "Measuring Tape",
                DescriptionAr = "شريط قياس 5 متر",
                DescriptionEn = "Measuring tape 5m",
                CurrentQuantity = 3,
                MinimumThreshold = 20,
                Unit = "قطعة",
                LastPurchasePrice = 15.00m
            },
            new StockItem 
            { 
                ToolId = "TOOL-006", 
                NameAr = "كماشة", 
                NameEn = "Pliers",
                DescriptionAr = "كماشة متعددة الاستخدام",
                DescriptionEn = "Multi-purpose pliers",
                CurrentQuantity = 0,
                MinimumThreshold = 10,
                Unit = "قطعة",
                LastPurchasePrice = 45.00m
            }
        };
        context.StockItems.AddRange(stockItems);
        await context.SaveChangesAsync();

        // Seed Suppliers
        var suppliers = new List<Supplier>
        {
            new Supplier
            {
                NameAr = "شركة الأدوات المتقدمة",
                NameEn = "Advanced Tools Company",
                ContactPerson = "أحمد السيد",
                Phone = "+966-12-3456789",
                Email = "info@advancedtools.com",
                Address = "الرياض، المملكة العربية السعودية",
                TaxNumber = "300123456700003",
                Rating = 4.5m,
                IsActive = true
            },
            new Supplier
            {
                NameAr = "مؤسسة العدد الصناعية",
                NameEn = "Industrial Equipment Corporation",
                ContactPerson = "محمد خالد",
                Phone = "+966-11-7654321",
                Email = "sales@industrial-equip.com",
                Address = "جدة، المملكة العربية السعودية",
                TaxNumber = "300234567800003",
                Rating = 4.2m,
                IsActive = true
            },
            new Supplier
            {
                NameAr = "شركة الأدوات الدولية",
                NameEn = "International Tools Company",
                ContactPerson = "سعيد عبدالله",
                Phone = "+966-13-9876543",
                Email = "contact@intl-tools.com",
                Address = "الدمام، المملكة العربية السعودية",
                TaxNumber = "300345678900003",
                Rating = 4.8m,
                IsActive = true
            },
            new Supplier
            {
                NameAr = "مركز المعدات الحديثة",
                NameEn = "Modern Equipment Center",
                ContactPerson = "علي حسن",
                Phone = "+966-14-5432109",
                Email = "info@modern-equip.com",
                Address = "مكة المكرمة، المملكة العربية السعودية",
                TaxNumber = "300456789000003",
                Rating = 4.0m,
                IsActive = true
            }
        };
        context.Suppliers.AddRange(suppliers);
        await context.SaveChangesAsync();

        // Seed sample workflow data for testing
        await SeedSampleWorkflowAsync(context);
        
        // Seed additional test scenarios
        await SeedAdditionalTestScenariosAsync(context);
    }

    private static async Task SeedSampleWorkflowAsync(PurchaseDbContext context)
    {
        // Step 1: Create Tool Requests
        var toolRequests = new List<ToolRequest>
        {
            // Request 1: Out of stock - Complete workflow
            new ToolRequest
            {
                ToolId = 3, // Electric Saw (out of stock)
                QuantityNeeded = 5,
                WorkAreaId = 1,
                RequesterId = 2,
                RequestDate = DateTime.UtcNow.AddDays(-15),
                ReasonAr = "نحتاج إلى مناشير كهربائية لمشروع جديد",
                ReasonEn = "We need electric saws for a new project",
                Status = "OutOfStock",
                IsInStock = false,
                StockVerificationNotes = "المخزون غير كافٍ - يتطلب الشراء",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-14)
            },
            // Request 2: In stock - Fulfilled directly
            new ToolRequest
            {
                ToolId = 4, // Hammer (in stock)
                QuantityNeeded = 10,
                WorkAreaId = 2,
                RequesterId = 2,
                RequestDate = DateTime.UtcNow.AddDays(-10),
                ReasonAr = "استبدال المطارق القديمة",
                ReasonEn = "Replace old hammers",
                Status = "InStock",
                IsInStock = true,
                QuantityFulfilled = 10,
                FulfilledDate = DateTime.UtcNow.AddDays(-10),
                StockVerificationNotes = "متوفر في المخزون - تم التوريد",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            // Request 3: Out of stock - In progress
            new ToolRequest
            {
                ToolId = 6, // Pliers (out of stock)
                QuantityNeeded = 15,
                WorkAreaId = 2,
                RequesterId = 2,
                RequestDate = DateTime.UtcNow.AddDays(-5),
                ReasonAr = "لأعمال الصيانة الدورية",
                ReasonEn = "For routine maintenance work",
                Status = "OutOfStock",
                IsInStock = false,
                StockVerificationNotes = "غير متوفر - يتطلب الشراء",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };
        context.ToolRequests.AddRange(toolRequests);
        await context.SaveChangesAsync();

        // Step 3: Create Purchase Requests
        var purchaseRequests = new List<PurchaseRequest>
        {
            // PR for first tool request - Approved
            new PurchaseRequest
            {
                ToolRequestId = 1,
                RequestDate = DateTime.UtcNow.AddDays(-14),
                Status = "Approved",
                ApprovedById = 4,
                ApprovalDate = DateTime.UtcNow.AddDays(-13),
                ApprovalNotes = "موافق عليه - أولوية عالية",
                EstimatedBudget = 2000.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-14),
                UpdatedAt = DateTime.UtcNow.AddDays(-13)
            },
            // PR for third tool request - Pending
            new PurchaseRequest
            {
                ToolRequestId = 3,
                RequestDate = DateTime.UtcNow.AddDays(-5),
                Status = "PendingApproval",
                EstimatedBudget = 700.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };
        context.PurchaseRequests.AddRange(purchaseRequests);
        await context.SaveChangesAsync();

        // Step 4: Create Quotations for first PR
        var quotations = new List<Quotation>
        {
            // Supplier 1 - Rank 2
            new Quotation
            {
                PurchaseRequestId = 1,
                SupplierId = 1,
                QuantityOffered = 5,
                UnitPrice = 380.00m,
                TotalPrice = 1900.00m,
                DeliveryTimeDays = 7,
                Notes = "منتج عالي الجودة مع ضمان سنتين",
                QuotationDate = DateTime.UtcNow.AddDays(-12),
                Ranking = 2,
                CustomScore = 8.5m,
                IsSelected = false,
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            // Supplier 2 - Rank 3
            new Quotation
            {
                PurchaseRequestId = 1,
                SupplierId = 2,
                QuantityOffered = 5,
                UnitPrice = 420.00m,
                TotalPrice = 2100.00m,
                DeliveryTimeDays = 5,
                Notes = "توصيل سريع",
                QuotationDate = DateTime.UtcNow.AddDays(-12),
                Ranking = 3,
                CustomScore = 7.8m,
                IsSelected = false,
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            // Supplier 3 - Rank 1 (Selected)
            new Quotation
            {
                PurchaseRequestId = 1,
                SupplierId = 3,
                QuantityOffered = 5,
                UnitPrice = 360.00m,
                TotalPrice = 1800.00m,
                DeliveryTimeDays = 10,
                Notes = "أفضل سعر مع جودة ممتازة",
                QuotationDate = DateTime.UtcNow.AddDays(-12),
                Ranking = 1,
                CustomScore = 9.2m,
                IsSelected = true,
                SelectedById = 5,
                SelectionDate = DateTime.UtcNow.AddDays(-10),
                SelectionReason = "أفضل عرض من حيث السعر والجودة",
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            // Supplier 4 - Not in top 3
            new Quotation
            {
                PurchaseRequestId = 1,
                SupplierId = 4,
                QuantityOffered = 5,
                UnitPrice = 450.00m,
                TotalPrice = 2250.00m,
                DeliveryTimeDays = 14,
                Notes = "عرض قياسي",
                QuotationDate = DateTime.UtcNow.AddDays(-12),
                Ranking = null,
                IsSelected = false,
                RejectionReason = "السعر مرتفع جداً",
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };
        context.Quotations.AddRange(quotations);
        await context.SaveChangesAsync();

        // Step 6: Create Purchase Order
        var purchaseOrder = new PurchaseOrder
        {
            OrderNumber = "PO-2025-0001",
            QuotationId = 3, // Selected quotation
            OrderDate = DateTime.UtcNow.AddDays(-9),
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(1),
            Status = "Inspected",
            TotalAmount = 1800.00m,
            PurchaseOrderDocument = "documents/po/PO-2025-0001.pdf",
            InvoiceDocument = "documents/invoices/INV-2025-0001.pdf",
            DeliveryNoteDocument = "documents/delivery/DN-2025-0001.pdf",
            SupplierAgreementDocument = "documents/agreements/AGR-2025-0001.pdf",
            Notes = "أمر شراء لمناشير كهربائية",
            CreatedAt = DateTime.UtcNow.AddDays(-9),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        };
        context.PurchaseOrders.Add(purchaseOrder);
        await context.SaveChangesAsync();

        // Step 7: Create Goods Receipt
        var goodsReceipt = new GoodsReceipt
        {
            PurchaseOrderId = 1,
            ReceivedDate = DateTime.UtcNow.AddDays(-2),
            ReceivedQuantity = 5,
            ExpectedQuantity = 5,
            QuantityVariancePercentage = 0.0m,
            IsQuantityAcceptable = true,
            QualityStatus = "Accepted",
            QualityNotes = "جودة ممتازة - جميع الوحدات تم فحصها بنجاح",
            ConditionNotes = "لا توجد أضرار - التغليف سليم",
            ProofOfReceiptDocument = "documents/receipts/GR-2025-0001.pdf",
            ReceivedById = 3,
            StockUpdated = true,
            StockUpdateDate = DateTime.UtcNow.AddDays(-2),
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        };
        context.GoodsReceipts.Add(goodsReceipt);

        // Update stock quantity (already done by goods receipt, but we ensure it's correct)
        var electricSaw = await context.StockItems.FirstOrDefaultAsync(s => s.Id == 3);
        if (electricSaw != null)
        {
            electricSaw.CurrentQuantity = 5;
            electricSaw.UpdatedAt = DateTime.UtcNow.AddDays(-2);
        }

        await context.SaveChangesAsync();

        // Step 8: Create Payment
        var payment = new Payment
        {
            GoodsReceiptId = 1,
            TransactionReference = "TXN-20251001",
            Amount = 1800.00m,
            PaymentDate = DateTime.UtcNow.AddDays(-1),
            PaymentMethod = "Bank Transfer",
            Status = "Completed",
            DocumentsVerified = true,
            QuantityVerified = true,
            PriceVerified = true,
            ProcessedById = 6,
            ProcessedDate = DateTime.UtcNow.AddDays(-1),
            VerificationNotes = "جميع المستندات صحيحة والكمية والسعر متطابقان",
            IsArchived = false,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        context.Payments.Add(payment);

        // Update PO status to Paid
        purchaseOrder.Status = "Paid";
        purchaseOrder.UpdatedAt = DateTime.UtcNow.AddDays(-1);

        await context.SaveChangesAsync();
    }

    private static async Task SeedAdditionalTestScenariosAsync(PurchaseDbContext context)
    {
        // Scenario 2: Tool Request with multiple quotations pending selection
        var toolRequest2 = new ToolRequest
        {
            ToolId = 2, // Electric Drill (low stock)
            QuantityNeeded = 10,
            WorkAreaId = 1,
            RequesterId = 2,
            RequestDate = DateTime.UtcNow.AddDays(-8),
            ReasonAr = "توسيع المخزون للإنتاج المتزايد",
            ReasonEn = "Expand inventory for increased production",
            Status = "OutOfStock",
            IsInStock = false,
            StockVerificationNotes = "المخزون الحالي أقل من الحد الأدنى",
            CreatedAt = DateTime.UtcNow.AddDays(-8),
            UpdatedAt = DateTime.UtcNow.AddDays(-8)
        };
        context.ToolRequests.Add(toolRequest2);
        await context.SaveChangesAsync();

        var purchaseRequest2 = new PurchaseRequest
        {
            ToolRequestId = 4,
            RequestDate = DateTime.UtcNow.AddDays(-7),
            Status = "Approved",
            ApprovedById = 5,
            ApprovalDate = DateTime.UtcNow.AddDays(-6),
            ApprovalNotes = "موافق عليه - مطلوب بشكل عاجل",
            EstimatedBudget = 2600.00m,
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            UpdatedAt = DateTime.UtcNow.AddDays(-6)
        };
        context.PurchaseRequests.Add(purchaseRequest2);
        await context.SaveChangesAsync();

        // Multiple quotations for this PR
        var quotations2 = new List<Quotation>
        {
            new Quotation
            {
                PurchaseRequestId = 3,
                SupplierId = 1,
                QuantityOffered = 10,
                UnitPrice = 245.00m,
                TotalPrice = 2450.00m,
                DeliveryTimeDays = 7,
                Notes = "منتج أصلي مع ضمان",
                QuotationDate = DateTime.UtcNow.AddDays(-5),
                Ranking = 1,
                CustomScore = 9.0m,
                IsSelected = false,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new Quotation
            {
                PurchaseRequestId = 3,
                SupplierId = 3,
                QuantityOffered = 10,
                UnitPrice = 240.00m,
                TotalPrice = 2400.00m,
                DeliveryTimeDays = 12,
                Notes = "أفضل سعر مع توصيل متأخر",
                QuotationDate = DateTime.UtcNow.AddDays(-5),
                Ranking = 2,
                CustomScore = 8.7m,
                IsSelected = false,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new Quotation
            {
                PurchaseRequestId = 3,
                SupplierId = 2,
                QuantityOffered = 10,
                UnitPrice = 260.00m,
                TotalPrice = 2600.00m,
                DeliveryTimeDays = 5,
                Notes = "توصيل سريع",
                QuotationDate = DateTime.UtcNow.AddDays(-5),
                Ranking = 3,
                CustomScore = 8.3m,
                IsSelected = false,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            }
        };
        context.Quotations.AddRange(quotations2);
        await context.SaveChangesAsync();

        // Scenario 3: New pending tool requests
        var newToolRequests = new List<ToolRequest>
        {
            new ToolRequest
            {
                ToolId = 1, // Electric Screwdriver
                QuantityNeeded = 8,
                WorkAreaId = 4,
                RequesterId = 2,
                RequestDate = DateTime.UtcNow.AddDays(-3),
                ReasonAr = "لقسم الجودة للفحص الدقيق",
                ReasonEn = "For quality department precision inspection",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new ToolRequest
            {
                ToolId = 5, // Measuring Tape (very low stock)
                QuantityNeeded = 20,
                WorkAreaId = 1,
                RequesterId = 2,
                RequestDate = DateTime.UtcNow.AddDays(-2),
                ReasonAr = "استبدال الشرائط القديمة والتالفة",
                ReasonEn = "Replace old and damaged tapes",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new ToolRequest
            {
                ToolId = 4, // Hammer (in stock)
                QuantityNeeded = 5,
                WorkAreaId = 2,
                RequesterId = 2,
                RequestDate = DateTime.UtcNow.AddDays(-1),
                ReasonAr = "احتياطي إضافي",
                ReasonEn = "Additional reserve",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };
        context.ToolRequests.AddRange(newToolRequests);
        await context.SaveChangesAsync();

        // Scenario 4: Purchase Order in transit
        var toolRequest3 = new ToolRequest
        {
            ToolId = 5, // Measuring Tape
            QuantityNeeded = 25,
            WorkAreaId = 3,
            RequesterId = 3,
            RequestDate = DateTime.UtcNow.AddDays(-20),
            ReasonAr = "تجديد المخزون",
            ReasonEn = "Stock renewal",
            Status = "OutOfStock",
            IsInStock = false,
            StockVerificationNotes = "المخزون أقل بكثير من الحد الأدنى",
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            UpdatedAt = DateTime.UtcNow.AddDays(-19)
        };
        context.ToolRequests.Add(toolRequest3);
        await context.SaveChangesAsync();

        var purchaseRequest3 = new PurchaseRequest
        {
            ToolRequestId = 7,
            RequestDate = DateTime.UtcNow.AddDays(-19),
            Status = "Approved",
            ApprovedById = 4,
            ApprovalDate = DateTime.UtcNow.AddDays(-18),
            ApprovalNotes = "موافق - أولوية عالية",
            EstimatedBudget = 400.00m,
            CreatedAt = DateTime.UtcNow.AddDays(-19),
            UpdatedAt = DateTime.UtcNow.AddDays(-18)
        };
        context.PurchaseRequests.Add(purchaseRequest3);
        await context.SaveChangesAsync();

        var quotation3 = new Quotation
        {
            PurchaseRequestId = 4,
            SupplierId = 2,
            QuantityOffered = 25,
            UnitPrice = 14.00m,
            TotalPrice = 350.00m,
            DeliveryTimeDays = 7,
            Notes = "كمية كبيرة - خصم 10%",
            QuotationDate = DateTime.UtcNow.AddDays(-17),
            Ranking = 1,
            CustomScore = 9.5m,
            IsSelected = true,
            SelectedById = 5,
            SelectionDate = DateTime.UtcNow.AddDays(-16),
            SelectionReason = "أفضل سعر وتوصيل سريع",
            CreatedAt = DateTime.UtcNow.AddDays(-17),
            UpdatedAt = DateTime.UtcNow.AddDays(-16)
        };
        context.Quotations.Add(quotation3);
        await context.SaveChangesAsync();

        var purchaseOrder2 = new PurchaseOrder
        {
            OrderNumber = "PO-2025-0002",
            QuotationId = 7,
            OrderDate = DateTime.UtcNow.AddDays(-15),
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(3),
            Status = "InTransit",
            TotalAmount = 350.00m,
            PurchaseOrderDocument = "documents/po/PO-2025-0002.pdf",
            Notes = "أمر شراء لشرائط القياس",
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            UpdatedAt = DateTime.UtcNow.AddDays(-15)
        };
        context.PurchaseOrders.Add(purchaseOrder2);
        await context.SaveChangesAsync();

        // Scenario 5: Rejected purchase request
        var toolRequest4 = new ToolRequest
        {
            ToolId = 3,
            QuantityNeeded = 20,
            WorkAreaId = 1,
            RequesterId = 2,
            RequestDate = DateTime.UtcNow.AddDays(-12),
            ReasonAr = "لمشروع مستقبلي",
            ReasonEn = "For future project",
            Status = "OutOfStock",
            IsInStock = false,
            StockVerificationNotes = "غير متوفر",
            CreatedAt = DateTime.UtcNow.AddDays(-12),
            UpdatedAt = DateTime.UtcNow.AddDays(-11)
        };
        context.ToolRequests.Add(toolRequest4);
        await context.SaveChangesAsync();

        var purchaseRequest4 = new PurchaseRequest
        {
            ToolRequestId = 8,
            RequestDate = DateTime.UtcNow.AddDays(-11),
            Status = "Rejected",
            ApprovedById = 5,
            ApprovalDate = DateTime.UtcNow.AddDays(-10),
            ApprovalNotes = "مرفوض - الميزانية غير كافية والكمية كبيرة جداً",
            RejectionReason = "تجاوز الميزانية المعتمدة",
            EstimatedBudget = 8000.00m,
            CreatedAt = DateTime.UtcNow.AddDays(-11),
            UpdatedAt = DateTime.UtcNow.AddDays(-10)
        };
        context.PurchaseRequests.Add(purchaseRequest4);
        await context.SaveChangesAsync();

        // Scenario 6: Goods Receipt with quality issues
        var toolRequest5 = new ToolRequest
        {
            ToolId = 6, // Pliers
            QuantityNeeded = 12,
            WorkAreaId = 2,
            RequesterId = 2,
            RequestDate = DateTime.UtcNow.AddDays(-25),
            ReasonAr = "للصيانة الروتينية",
            ReasonEn = "For routine maintenance",
            Status = "OutOfStock",
            IsInStock = false,
            StockVerificationNotes = "المخزون فارغ",
            CreatedAt = DateTime.UtcNow.AddDays(-25),
            UpdatedAt = DateTime.UtcNow.AddDays(-24)
        };
        context.ToolRequests.Add(toolRequest5);
        await context.SaveChangesAsync();

        var purchaseRequest5 = new PurchaseRequest
        {
            ToolRequestId = 9,
            RequestDate = DateTime.UtcNow.AddDays(-24),
            Status = "Approved",
            ApprovedById = 4,
            ApprovalDate = DateTime.UtcNow.AddDays(-23),
            ApprovalNotes = "موافق عليه",
            EstimatedBudget = 550.00m,
            CreatedAt = DateTime.UtcNow.AddDays(-24),
            UpdatedAt = DateTime.UtcNow.AddDays(-23)
        };
        context.PurchaseRequests.Add(purchaseRequest5);
        await context.SaveChangesAsync();

        var quotation4 = new Quotation
        {
            PurchaseRequestId = 6,
            SupplierId = 4,
            QuantityOffered = 12,
            UnitPrice = 42.00m,
            TotalPrice = 504.00m,
            DeliveryTimeDays = 5,
            Notes = "توصيل سريع",
            QuotationDate = DateTime.UtcNow.AddDays(-22),
            Ranking = 1,
            CustomScore = 8.5m,
            IsSelected = true,
            SelectedById = 5,
            SelectionDate = DateTime.UtcNow.AddDays(-21),
            SelectionReason = "توصيل سريع وسعر معقول",
            CreatedAt = DateTime.UtcNow.AddDays(-22),
            UpdatedAt = DateTime.UtcNow.AddDays(-21)
        };
        context.Quotations.Add(quotation4);
        await context.SaveChangesAsync();

        var purchaseOrder3 = new PurchaseOrder
        {
            OrderNumber = "PO-2025-0003",
            QuotationId = 8,
            OrderDate = DateTime.UtcNow.AddDays(-20),
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(-15),
            Status = "PartiallyReceived",
            TotalAmount = 504.00m,
            PurchaseOrderDocument = "documents/po/PO-2025-0003.pdf",
            InvoiceDocument = "documents/invoices/INV-2025-0003.pdf",
            DeliveryNoteDocument = "documents/delivery/DN-2025-0003.pdf",
            Notes = "بعض الوحدات بها مشاكل جودة",
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            UpdatedAt = DateTime.UtcNow.AddDays(-4)
        };
        context.PurchaseOrders.Add(purchaseOrder3);
        await context.SaveChangesAsync();

        var goodsReceipt2 = new GoodsReceipt
        {
            PurchaseOrderId = 3,
            ReceivedDate = DateTime.UtcNow.AddDays(-4),
            ReceivedQuantity = 10,
            ExpectedQuantity = 12,
            QuantityVariancePercentage = -16.67m,
            IsQuantityAcceptable = false,
            QualityStatus = "PartiallyAccepted",
            QualityNotes = "تم قبول 10 وحدات فقط - 2 وحدة بها عيوب تصنيع",
            ConditionNotes = "بعض الوحدات لا تفتح وتغلق بشكل صحيح",
            ProofOfReceiptDocument = "documents/receipts/GR-2025-0003.pdf",
            ReceivedById = 3,
            StockUpdated = true,
            StockUpdateDate = DateTime.UtcNow.AddDays(-4),
            CreatedAt = DateTime.UtcNow.AddDays(-4),
            UpdatedAt = DateTime.UtcNow.AddDays(-4)
        };
        context.GoodsReceipts.Add(goodsReceipt2);

        // Update pliers stock
        var pliers = await context.StockItems.FirstOrDefaultAsync(s => s.Id == 6);
        if (pliers != null)
        {
            pliers.CurrentQuantity = 10;
            pliers.UpdatedAt = DateTime.UtcNow.AddDays(-4);
        }

        await context.SaveChangesAsync();

        var payment2 = new Payment
        {
            GoodsReceiptId = 2,
            TransactionReference = "TXN-20251010",
            Amount = 420.00m, // Adjusted for 10 units instead of 12
            PaymentDate = DateTime.UtcNow.AddDays(-3),
            PaymentMethod = "Bank Transfer",
            Status = "Completed",
            DocumentsVerified = true,
            QuantityVerified = false,
            PriceVerified = true,
            ProcessedById = 6,
            ProcessedDate = DateTime.UtcNow.AddDays(-3),
            VerificationNotes = "تم تعديل المبلغ بناءً على الكمية المستلمة الفعلية (10 وحدات بدلاً من 12)",
            IsArchived = false,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            UpdatedAt = DateTime.UtcNow.AddDays(-3)
        };
        context.Payments.Add(payment2);
        await context.SaveChangesAsync();

        // Scenario 7: Payment pending for goods received
        var goodsReceipt3 = new GoodsReceipt
        {
            PurchaseOrderId = 1, // Already has one payment, adding second scenario
            ReceivedDate = DateTime.UtcNow.AddDays(-2),
            ReceivedQuantity = 5,
            ExpectedQuantity = 5,
            QuantityVariancePercentage = 0.0m,
            IsQuantityAcceptable = true,
            QualityStatus = "Accepted",
            QualityNotes = "جودة ممتازة",
            ConditionNotes = "لا توجد مشاكل",
            ProofOfReceiptDocument = "documents/receipts/GR-2025-0004.pdf",
            ReceivedById = 3,
            StockUpdated = false,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        };
        context.GoodsReceipts.Add(goodsReceipt3);
        await context.SaveChangesAsync();
    }
}
