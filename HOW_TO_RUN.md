# 🚀 How to Run the Purchase Management System

## Quick Start Guide

### Step 1: Start the Backend Server

Open a PowerShell terminal and run:

```powershell
cd "d:\Project\purchase\New folder (2)\Purchase-master\Purchase.Server"
dotnet run
```

**Expected Output:**
- Database will be created automatically (`PurchaseWorkflow.db`)
- Mock data will be seeded
- Server will start on: `http://localhost:5151`
- You should see: "Application started. Press Ctrl+C to shut down."

### Step 2: Start the Frontend Application

Open a **NEW** PowerShell terminal and run:

```powershell
cd "d:\Project\purchase\New folder (2)\Purchase-master\purchase.client"
npm start
```

**Expected Output:**
- Angular dev server will compile
- Browser will open automatically at: `https://localhost:61710`
- You should see the Purchase Management System dashboard

## ✅ Verification

Once both servers are running:

1. **Backend Health Check:**
   - Open browser: `http://localhost:5151/api/stockitems`
   - You should see JSON data with 6 stock items

2. **Frontend Check:**
   - Open browser: `https://localhost:61710`
   - You should see the dashboard with:
     - Navigation menu on top
     - Pending requests section
     - Low stock alerts

## 🎯 Testing the Application

### Test Scenario 1: View Tool Requests
1. Click **"طلبات الأدوات"** (Tool Requests) in the menu
2. You should see 9 tool requests with different statuses
3. Try filtering by status using the dropdown

### Test Scenario 2: Create a New Tool Request
1. Click **"+ إنشاء طلب جديد"** (Create New Request)
2. Fill in the form:
   - Select a tool (e.g., "مثقاب كهربائي" - Electric Drill)
   - Enter quantity: 5
   - Select work area: "ورشة الإنتاج" (Production Workshop)
   - Enter reason in Arabic
3. Click **"حفظ الطلب"** (Save Request)
4. You should be redirected to the list with your new request

### Test Scenario 3: View Complete Workflow
1. Go to **Dashboard** (لوحة التحكم)
2. See pending tool requests
3. See low stock items
4. Navigate to:
   - **Purchase Orders** (أوامر الشراء) - See 3 orders
   - **Goods Receipts** (استلام البضائع) - See 3 receipts
   - **Payments** (المدفوعات) - See 2 payments

## 📊 Sample Data Summary

### Users Available:
- admin - System Administrator
- requester1 - Tool Requester
- stockmgr - Stock Manager
- approver1 - First Level Approver
- approver2 - Second Level Approver
- finance - Financial Officer

### Workflow Examples in Database:

**Complete Workflow (PO-2025-0001):**
- Tool Request → Purchase Request → 4 Quotations → PO Created → Goods Received → Payment Completed

**In Progress (PO-2025-0002):**
- Status: In Transit
- Expected Delivery: 3 days from now

**Quality Issues (PO-2025-0003):**
- 12 units ordered, only 10 accepted
- 2 units rejected due to defects

## 🛠️ Troubleshooting

### Problem: Backend won't start
**Solution:**
```powershell
cd "d:\Project\purchase\New folder (2)\Purchase-master\Purchase.Server"
dotnet clean
dotnet build
dotnet run
```

### Problem: Frontend shows proxy errors
**Solution:**
1. Make sure backend is running first
2. Check `proxy.conf.js` - target should be `http://localhost:5151`
3. Restart npm start

### Problem: Database errors
**Solution:**
```powershell
# Delete the database file
cd "d:\Project\purchase\New folder (2)\Purchase-master\Purchase.Server"
Remove-Item PurchaseWorkflow.db
# Restart the application
dotnet run
```

### Problem: Port already in use
**Backend (5151):**
- Edit `Purchase.Server/Properties/launchSettings.json`
- Change port number in "applicationUrl"

**Frontend (61710):**
- Edit `purchase.client/package.json`
- Update the start script

## 📱 Accessing the Application

### URLs:
- **Frontend**: https://localhost:61710
- **Backend API**: http://localhost:5151
- **API Documentation**: http://localhost:5151/scalar/v1

### Available Pages:
1. **Dashboard** (`/dashboard`)
   - Overview of pending items
   - Low stock alerts
   
2. **Tool Requests** (`/tool-requests`)
   - List of all tool requests
   - Create new request (`/tool-requests/create`)

3. **Purchase Orders** (`/purchase-orders`)
   - List of all purchase orders
   - View order details

4. **Goods Receipts** (`/goods-receipts`)
   - List of all goods receipts

5. **Payments** (`/payments`)
   - List of all payments

## 🎨 Features to Test

### ✅ Currently Working:
- [x] View all tool requests with filtering
- [x] Create new tool requests
- [x] View stock items
- [x] View work areas
- [x] View purchase orders
- [x] View goods receipts
- [x] View payments
- [x] Dashboard statistics
- [x] Bilingual UI (Arabic/English)
- [x] Status badges and colors
- [x] Form validation

### 🚧 Being Enhanced:
- [ ] Purchase request approval workflow
- [ ] Quotation comparison and selection
- [ ] Detailed PO management
- [ ] Quality inspection form
- [ ] Payment approval workflow

## 💡 Tips

1. **Navigation**: Use the top menu to navigate between sections
2. **Language**: The interface is primarily in Arabic with English labels
3. **Data**: All test data is pre-loaded automatically
4. **Reset**: To reset everything, delete `PurchaseWorkflow.db` and restart

## 📞 Need Help?

Check these files for more information:
- `APPLICATION_STATUS.md` - Current implementation status
- `README.md` - Complete documentation
- `WORKFLOW.md` - Workflow details
- `IMPLEMENTATION.md` - Technical details

---

**Enjoy testing the Purchase Management System! 🎉**
