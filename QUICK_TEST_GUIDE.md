# Quick Test Guide - Purchase Management System

## 🚀 How to Start the Application

### 1. Start Backend Server
```bash
cd Purchase.Server
dotnet run
```
**Expected**: Server running on http://localhost:5151

### 2. Start Frontend (In a new terminal)
```bash
cd purchase.client
npm start
```
**Expected**: Angular dev server running on https://127.0.0.1:61710

---

## ✅ Quick Navigation Test (2 Minutes)

### Test All Menu Items (Click each one):

1. **لوحة التحكم** (Dashboard)
   - ✅ Should show pending requests and low stock items
   - URL: `/dashboard`

2. **طلبات الأدوات** (Tool Requests)
   - ✅ Should list 9 tool requests
   - ✅ Try clicking "إنشاء طلب جديد" (Create New)
   - URL: `/tool-requests`

3. **طلبات الشراء** (Purchase Requests)
   - ✅ Should list 2 purchase requests
   - ✅ One approved, one pending
   - URL: `/purchase-requests`

4. **عروض الأسعار** (Quotations)
   - ✅ Should show 4 quotations grouped
   - ✅ Ranking badges visible (🥇🥈🥉)
   - URL: `/quotations`

5. **أوامر الشراء** (Purchase Orders)
   - ✅ Should list 1 purchase order
   - ✅ Status: Paid
   - URL: `/purchase-orders`

6. **استلام البضائع** (Goods Receipts)
   - ✅ Should list 1 goods receipt
   - ✅ Quality status: Accepted
   - URL: `/goods-receipts`

7. **المدفوعات** (Payments)
   - ✅ Should list 1 payment
   - ✅ Status: Completed
   - URL: `/payments`

---

## 🧪 API Quick Test

```bash
# Test all endpoints at once
curl -s http://localhost:5151/api/toolrequests | jq 'length'        # Expect: 9
curl -s http://localhost:5151/api/purchaserequests | jq 'length'   # Expect: 2
curl -s http://localhost:5151/api/quotations | jq 'length'         # Expect: 4
curl -s http://localhost:5151/api/purchaseorders | jq 'length'     # Expect: 1
curl -s http://localhost:5151/api/goodsreceipts | jq 'length'      # Expect: 1
curl -s http://localhost:5151/api/payments | jq 'length'           # Expect: 1
```

---

## 📊 Expected Data Counts

| Feature | Count | Status |
|---------|-------|--------|
| Tool Requests | 9 | ✅ |
| Purchase Requests | 2 | ✅ |
| Quotations | 4 | ✅ |
| Purchase Orders | 1 | ✅ |
| Goods Receipts | 1 | ✅ |
| Payments | 1 | ✅ |
| Stock Items | 6 | ✅ |
| Suppliers | 4 | ✅ |
| Work Areas | 4 | ✅ |

---

## 🎯 Interactive Features to Test

### 1. Create New Tool Request
1. Go to "طلبات الأدوات" (Tool Requests)
2. Click "إنشاء طلب جديد" (Create New)
3. Fill the form:
   - Select tool: "مثقاب كهربائي"
   - Quantity: 3
   - Work area: "ورشة الإنتاج"
   - Reason: "اختبار النظام"
4. Click "حفظ الطلب" (Save)
5. ✅ Should redirect back to list with new request

### 2. Approve Purchase Request
1. Go to "طلبات الشراء" (Purchase Requests)
2. Find request with status "في انتظار الموافقة"
3. Click "✓ موافقة" (Approve)
4. Confirm the dialog
5. ✅ Status should change to "موافق عليه"

### 3. Select Best Quotation
1. Go to "عروض الأسعار" (Quotations)
2. Find a quotation with ranking 1-3
3. Click "✓ اختيار" (Select)
4. Enter selection reason
5. ✅ Quotation should show "تم اختيار هذا العرض"

### 4. Filter by Status
Try filtering on these pages:
- ✅ Tool Requests (by status dropdown)
- ✅ Purchase Requests (by status dropdown)
- ✅ Quotations (by purchase request dropdown)

---

## 🔍 Visual Indicators to Verify

### Status Badges
- 🟡 Yellow/Warning: Pending, PendingApproval
- 🟢 Green/Success: Approved, InStock, Accepted, Completed
- 🔴 Red/Danger: Rejected, OutOfStock, Failed

### Ranking Badges
- 🥇 Gold: Rank 1 (best offer)
- 🥈 Silver: Rank 2
- 🥉 Bronze: Rank 3

### Card Borders
- Gold border: Rank 1 quotation
- Silver border: Rank 2 quotation
- Bronze border: Rank 3 quotation
- Green border: Selected quotation

---

## 🐛 Common Issues & Solutions

### Issue 1: Cannot connect to backend
**Solution**: Make sure backend is running on port 5151
```bash
curl http://localhost:5151/api/stockitems
```

### Issue 2: Cannot load frontend
**Solution**: Make sure Angular dev server is running
```bash
cd purchase.client
npm start
```

### Issue 3: SSL certificate error
**Solution**: Use `curl -k` or accept the self-signed certificate in browser

### Issue 4: Database not seeded
**Solution**: Delete database and restart
```bash
cd Purchase.Server
rm PurchaseWorkflow.db
dotnet run
```

---

## 📱 Mobile Testing

The application is responsive. Test on:
- Desktop (1920×1080): ✅ Full layout
- Tablet (768×1024): ✅ Adapted layout
- Mobile (375×667): ✅ Mobile menu

---

## ⚡ Performance Expectations

- Page load: < 2 seconds
- API calls: < 100ms
- Navigation: < 100ms
- Form submission: < 200ms

---

## 📝 Test Checklist

Copy this checklist for manual testing:

```
[ ] Backend started successfully
[ ] Frontend started successfully
[ ] Dashboard loads with data
[ ] Tool Requests page loads
[ ] Can create new tool request
[ ] Purchase Requests page loads
[ ] Can approve purchase request
[ ] Quotations page loads
[ ] Can select quotation
[ ] Purchase Orders page loads
[ ] Goods Receipts page loads
[ ] Payments page loads
[ ] All filters work
[ ] All status badges visible
[ ] Ranking badges visible (🥇🥈🥉)
[ ] Arabic text displays correctly
[ ] RTL layout works
[ ] All API endpoints respond
[ ] Navigation menu works
[ ] Mobile responsive
```

---

## 🎉 Success Criteria

✅ All 7 navigation items clickable  
✅ All pages load with data  
✅ Forms submit successfully  
✅ Filters work correctly  
✅ Status badges display properly  
✅ Arabic/English bilingual support  
✅ API responses in < 100ms  
✅ No console errors  

**If all checks pass: ✅ SYSTEM READY**

---

*Last Updated: October 19, 2025*
