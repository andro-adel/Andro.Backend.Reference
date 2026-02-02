# 🧪 دليل اختبار Background Jobs - خطوة بخطوة

---

## 📋 الإعداد

### **1. تأكد من تشغيل المشروع:**

```powershell
# 1. Run migrations
cd src/Andro.Backend.Reference.DbMigrator
dotnet run

# 2. Run web app
cd ../Andro.Backend.Reference.Web
dotnet run
```

**انتظر حتى ترى:**
```
Now listening on: https://localhost:44385
```

---

## 🧪 اختبار 1: Background Job (LowStockAlertJob)

### **الهدف:**
اختبار أن Background Job يتم إنشاؤه تلقائياً عند نقص الكمية لأقل من 10

### **الخطوات:**

#### **Step 1: افتح Postman**

#### **Step 2: Login للحصول على Token**

```http
POST https://localhost:44385/api/account/login
Content-Type: application/json

{
  "userNameOrEmailAddress": "admin",
  "password": "1q2w3E*"
}
```

**احفظ الـ `accessToken`**

---

#### **Step 3: اختر منتج موجود**

```http
GET https://localhost:44385/api/app/product
Authorization: Bearer {accessToken}
```

**احفظ `id` لأي منتج**

---

#### **Step 4: حدّث الكمية لأقل من 10**

```http
PUT https://localhost:44385/api/app/product/{productId}
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "name": "Updated Product",
  "categoryId": "{categoryId}",
  "price": 99.99,
  "stock": 5,          ← أقل من 10
  "description": "Test low stock"
}
```

---

#### **Step 5: راقب Console Logs**

**يجب أن ترى في console:**

```log
[12:25:30 INF] 📉 Stock Changed: Updated Product - 50 → 5 (Decreased: 45)
[12:25:30 WRN] ⚠️ Low Stock Alert: Updated Product - Only 5 items left!
[12:25:30 INF] 🔔 Low stock alert job enqueued for product: Updated Product
[12:25:31 WRN] ⚠️ LOW STOCK ALERT: Product 'Updated Product' (ID: ...) - Current Stock: 5, Minimum: 10
[12:25:31 INF] 📧 Alert notification sent for low stock product: Updated Product
```

---

### **✅ ماذا حدث:**

1. ✅ **Event Handler** - استقبل `ProductStockChangedEvent`
2. ✅ **Low Stock Detection** - اكتشف أن الكمية < 10
3. ✅ **Job Enqueued** - أضاف Job للـ queue
4. ✅ **Job Executed** - Background Job Manager نفذ الـ job
5. ✅ **Alert Logged** - تم تسجيل التنبيه

---

## 🔄 اختبار 2: Background Worker (StockCheckWorker)

### **الهدف:**
اختبار أن Worker يعمل دورياً كل 5 دقائق ويفحص الكمية

### **الخطوات:**

#### **Step 1: انتظر 5 دقائق بعد تشغيل المشروع**

#### **Step 2: راقب Console Logs**

**يجب أن ترى كل 5 دقائق:**

```log
[12:30:00 INF] 🔍 Stock check worker started at 02/02/2026 12:30:00
[12:30:01 WRN] ⚠️ Found 3 products with low stock
[12:30:01 WRN] 📦 Low Stock: Product A (ID: ...) - Current: 5
[12:30:01 WRN] 📦 Low Stock: Product B (ID: ...) - Current: 8
[12:30:01 WRN] 📦 Low Stock: Product C (ID: ...) - Current: 3
[12:30:01 INF] ✅ Stock check worker completed at 02/02/2026 12:30:01
```

**أو إذا كل المنتجات كميتها كافية:**

```log
[12:30:00 INF] 🔍 Stock check worker started at 02/02/2026 12:30:00
[12:30:01 INF] ✅ All products have sufficient stock
[12:30:01 INF] ✅ Stock check worker completed at 02/02/2026 12:30:01
```

---

### **✅ ماذا حدث:**

1. ✅ **Worker Started** - Worker بدأ تلقائياً مع التطبيق
2. ✅ **Timer Triggered** - Timer نفذ الـ worker كل 5 دقائق
3. ✅ **Database Query** - فحص كل المنتجات التي stock < 10
4. ✅ **Warnings Logged** - سجل warnings للمنتجات منخفضة الكمية
5. ✅ **Repeat** - يعيد نفسه كل 5 دقائق

---

## 🎯 سيناريوهات اختبار إضافية

### **Scenario 1: زيادة الكمية (لا ينشئ Job)**

```http
PUT https://localhost:44385/api/app/product/{productId}
{
  "stock": 50  ← أكثر من 10
}
```

**النتيجة:**
```log
[12:35:00 INF] 📈 Stock Changed: Product - 5 → 50 (Increased: 45)
```
✅ **لا يوجد Background Job** - لأن الكمية كافية

---

### **Scenario 2: نقص الكمية لكن أكثر من 10 (لا ينشئ Job)**

```http
PUT https://localhost:44385/api/app/product/{productId}
{
  "stock": 15  ← أكثر من 10
}
```

**النتيجة:**
```log
[12:36:00 INF] 📉 Stock Changed: Product - 50 → 15 (Decreased: 35)
```
✅ **لا يوجد Background Job** - لأن الكمية لا تزال > 10

---

### **Scenario 3: نقص الكمية لأقل من 10 (ينشئ Job)**

```http
PUT https://localhost:44385/api/app/product/{productId}
{
  "stock": 7  ← أقل من 10
}
```

**النتيجة:**
```log
[12:37:00 INF] 📉 Stock Changed: Product - 15 → 7 (Decreased: 8)
[12:37:00 WRN] ⚠️ Low Stock Alert: Product - Only 7 items left!
[12:37:00 INF] 🔔 Low stock alert job enqueued for product: Product
[12:37:01 WRN] ⚠️ LOW STOCK ALERT: Product 'Product' (ID: ...) - Current Stock: 7, Minimum: 10
[12:37:01 INF] 📧 Alert notification sent for low stock product: Product
```
✅ **Background Job تم إنشاؤه وتنفيذه**

---

## 📊 كيفية مراقبة Background Jobs

### **1. Console Logs (الطريقة الأسهل)**

افتح terminal حيث يعمل `dotnet run` وراقب الـ logs

---

### **2. Database (للـ Jobs المخزنة)**

```sql
-- فحص الـ Background Jobs في الـ database
SELECT * FROM AbpBackgroundJobs
ORDER BY CreationTime DESC
```

**ستجد:**
- JobName: `LowStockAlertJob`
- JobArgs: JSON بيانات الـ job
- TryCount: عدد المحاولات
- IsAbandoned: هل فشل؟
- NextTryTime: موعد المحاولة التالية

---

### **3. Log Files (إذا فعّلت File Logging)**

```
logs/
  └── app-2026-02-02.txt
```

---

## 🔧 تعديل Timer للـ Worker (للاختبار)

### **لاختبار أسرع، قلل الـ Timer:**

```csharp
// في StockCheckWorker.cs
public StockCheckWorker(...)
{
    // Run every 30 seconds (للاختبار)
    Timer.Period = 30 * 1000; // بدلاً من 5 * 60 * 1000
}
```

**أعد تشغيل المشروع:**
```powershell
dotnet run
```

**الآن Worker سيعمل كل 30 ثانية بدلاً من 5 دقائق**

---

## ✅ Checklist للتأكد من نجاح الاختبار

### **Background Job:**
- [ ] تحديث منتج لكمية < 10
- [ ] رؤية "🔔 Low stock alert job enqueued" في logs
- [ ] رؤية "⚠️ LOW STOCK ALERT" في logs
- [ ] رؤية "📧 Alert notification sent" في logs

### **Background Worker:**
- [ ] انتظار 5 دقائق (أو 30 ثانية إذا عدّلت Timer)
- [ ] رؤية "🔍 Stock check worker started" في logs
- [ ] رؤية قائمة المنتجات منخفضة الكمية
- [ ] رؤية "✅ Stock check worker completed" في logs
- [ ] يتكرر كل 5 دقائق

---

## 🐛 Troubleshooting

### **❌ Problem: لا أرى أي logs للـ Background Job**

**الحلول:**
1. تأكد أن `IsJobExecutionEnabled = true` في `ReferenceApplicationModule`
2. تأكد من تحديث الكمية لأقل من 10
3. تأكد من استخدام `PUT` وليس `POST`

---

### **❌ Problem: لا أرى logs للـ Worker**

**الحلول:**
1. تأكد من تسجيل Worker في `OnApplicationInitializationAsync`
2. انتظر 5 دقائق كاملة
3. قلل الـ Timer إلى 30 ثانية للاختبار
4. تأكد من إعادة build بعد أي تعديل

---

### **❌ Problem: Job يفشل في التنفيذ**

**الحلول:**
1. افحص الـ Exception في logs
2. تأكد من الـ dependencies في constructor
3. تأكد من `ITransientDependency` على الـ Job class

---

## 🎯 الخلاصة

### **Background Job (LowStockAlertJob):**
- ✅ **Trigger:** عند تحديث stock لأقل من 10
- ✅ **Execution:** مرة واحدة فوراً
- ✅ **Use Case:** إرسال تنبيهات فورية

### **Background Worker (StockCheckWorker):**
- ✅ **Trigger:** كل 5 دقائق تلقائياً
- ✅ **Execution:** دوري ومستمر
- ✅ **Use Case:** مراقبة دورية للمخزون

---

**تم! الآن يمكنك اختبار Background Jobs بنجاح! 🎉**
