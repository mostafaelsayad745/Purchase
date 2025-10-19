# Testing Guide - Purchase Management System

## How to Run the Application

### Prerequisites
- Node.js (v18 or higher)
- .NET 9.0 SDK
- SQL Server or SQLite

### Running the Backend
```bash
cd Purchase.Server
dotnet restore
dotnet run
```
The API will be available at `https://localhost:7001`

### Running the Frontend
```bash
cd purchase.client
npm install
npm start
```
The application will be available at `https://localhost:4200`

---

## Test Scenarios

### 1. Purchase Requests Page
**Test:** Manual Purchase Request Creation

1. Navigate to "طلبات الشراء" (Purchase Requests)
2. Click "+ إنشاء طلب شراء يدوياً"
3. Select a tool request from the dropdown
4. Enter an estimated budget
5. Click "حفظ الطلب"
6. Verify success message
7. Check that the new request appears in the list

**Expected Result:** New purchase request created successfully

---

### 2. Quotations Page
**Test 2.1:** Manual Quotation Creation

1. Navigate to "عروض الأسعار" (Quotations)
2. Click "+ إضافة عرض سعر يدوياً"
3. Fill in all fields:
   - Select purchase request
   - Select supplier
   - Enter quantity offered
   - Enter unit price
   - Enter delivery time
   - Add notes (optional)
4. Click "حفظ العرض"
5. Verify success message
6. Check that the new quotation appears in the list

**Expected Result:** New quotation created successfully

**Test 2.2:** Quotation Archiving

1. Navigate to "عروض الأسعار" (Quotations)
2. Find a quotation with ranking ≤ 3
3. Click "اختيار" (Select) button
4. Enter a selection reason
5. Confirm selection
6. Scroll to "العروض المؤرشفة" section
7. Click "عرض الأرشيف" button
8. Verify other quotations are archived

**Expected Result:** Selected quotation marked as selected, other quotations archived

**Test 2.3:** Filtering

1. Navigate to "عروض الأسعار" (Quotations)
2. Test supplier name filter:
   - Type part of a supplier name
   - Verify filtered results
3. Test date filter:
   - Select a date
   - Verify only quotations from that date show
4. Test combined filters
5. Clear filters and verify all quotations show

**Expected Result:** Filters work correctly individually and in combination

---

### 3. Tool Request Page
**Test 3.1:** Create with Text Input

1. Navigate to "طلبات الأدوات" (Tool Requests)
2. Click "+ إنشاء طلب جديد"
3. In "الأداة المطلوبة" field:
   - Start typing a tool name
   - See suggestions appear
   - Select or type complete name
4. In "منطقة العمل" field:
   - Start typing work area name
   - See suggestions appear
   - Select or type complete name
5. Fill other required fields
6. Click "حفظ الطلب"

**Expected Result:** Tool request created with typed values

**Test 3.2:** Filtering

1. Navigate to "طلبات الأدوات" (Tool Requests)
2. Test status filter:
   - Select different statuses
   - Verify filtered results
3. Test tool name search:
   - Type part of a tool name
   - Verify live filtering
4. Test date filter:
   - Select a date
   - Verify only requests from that date show

**Expected Result:** All filters work correctly

**Test 3.3:** Pagination

1. Navigate to "طلبات الأدوات" (Tool Requests)
2. Verify pagination controls appear
3. Test "التالي" (Next) button
4. Test "السابق" (Previous) button
5. Change page size (10, 25, 50, 100)
6. Verify correct number of items displayed
7. Verify page counter updates correctly

**Expected Result:** Pagination works smoothly

---

### 4. Dashboard Page
**Test:** All Buttons Functional

1. Navigate to "لوحة التحكم" (Dashboard)
2. Test each workflow step button:
   - Step 1: "إنشاء طلب جديد" → Should go to create tool request
   - Step 2: "عرض الطلبات" → Should go to tool requests list
   - Step 3: "عرض الطلبات" → Should go to purchase requests
   - Step 4: "إدارة العروض" → Should go to quotations
   - Step 6: "عرض الأوامر" → Should go to purchase orders
   - Step 7: "عرض الإيصالات" → Should go to goods receipts
   - Step 8: "عرض المدفوعات" → Should go to payments

**Expected Result:** All buttons navigate to correct pages

---

### 5. Purchase Orders Page
**Test 5.1:** Create Purchase Order

1. Navigate to "أوامر الشراء" (Purchase Orders)
2. Click "+ إنشاء أمر شراء جديد"
3. Select a quotation
4. Select expected delivery date
5. Add notes (optional)
6. Click "حفظ الأمر"
7. Verify success message
8. Verify redirect to list

**Expected Result:** New purchase order created

**Test 5.2:** View Details

1. Navigate to "أوامر الشراء" (Purchase Orders)
2. Click "عرض التفاصيل" on any order
3. Verify all order information displayed:
   - Order number
   - Status
   - Dates
   - Amount
   - Quotation details
   - Documents
4. Click "العودة للقائمة"

**Expected Result:** Complete order details displayed

---

### 6. Goods Receipts Page
**Test 6.1:** Create Goods Receipt

1. Navigate to "إيصالات استلام البضائع" (Goods Receipts)
2. Click "+ إنشاء إيصال استلام جديد"
3. Select purchase order
4. Enter received quantity
5. Select quality status
6. Add notes (optional)
7. Click "حفظ الإيصال"
8. Verify success message

**Expected Result:** New goods receipt created

**Test 6.2:** View Details

1. Navigate to "إيصالات استلام البضائع" (Goods Receipts)
2. Click "عرض التفاصيل" on any receipt
3. Verify all information displayed:
   - Receipt details
   - Quality information
   - Stock update status
   - Documents
4. Click "العودة للقائمة"

**Expected Result:** Complete receipt details displayed

---

### 7. Payments Page
**Test 7.1:** Create Payment

1. Navigate to "المدفوعات" (Payments)
2. Click "+ إنشاء دفعة جديدة"
3. Select goods receipt
4. Enter amount
5. Select payment date
6. Select payment method
7. Add verification notes (optional)
8. Click "حفظ الدفعة"
9. Verify success message

**Expected Result:** New payment created

**Test 7.2:** View Details

1. Navigate to "المدفوعات" (Payments)
2. Click "عرض التفاصيل" on any payment
3. Verify all information displayed:
   - Payment details
   - Verification status
   - Goods receipt info
   - Archive status
4. Click "العودة للقائمة"

**Expected Result:** Complete payment details displayed

---

## Integration Testing

### Complete Workflow Test
1. Create tool request (Step 1)
2. Verify stock (Step 2) - system should mark as out of stock
3. Create purchase request manually (Step 3)
4. Approve purchase request
5. Add multiple quotations manually (Step 4)
6. Select best quotation (Step 5)
7. Verify other quotations archived
8. Create purchase order (Step 6)
9. Create goods receipt (Step 7)
10. Create payment (Step 8)
11. Verify complete workflow

**Expected Result:** Full 8-step workflow completes successfully

---

## Visual Verification Checklist

- [ ] All Arabic text displayed correctly (RTL)
- [ ] Colors and styling consistent
- [ ] Buttons have hover effects
- [ ] Forms have proper validation
- [ ] Error messages appear in red
- [ ] Success messages appear in green
- [ ] Status badges color-coded correctly
- [ ] Loading spinners appear during data fetch
- [ ] Pagination controls styled properly
- [ ] Filter inputs clearly labeled
- [ ] Archived section visually distinct

---

## Browser Compatibility
Test in:
- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Edge (latest)

---

## Performance Checks
- [ ] Pages load within 2 seconds
- [ ] Filtering responds instantly
- [ ] No console errors
- [ ] No memory leaks
- [ ] Smooth pagination transitions

---

## Accessibility
- [ ] All forms keyboard-navigable
- [ ] Tab order logical
- [ ] Focus indicators visible
- [ ] Labels associated with inputs
- [ ] ARIA attributes where needed
