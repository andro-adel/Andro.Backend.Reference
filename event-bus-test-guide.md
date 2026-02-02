# 🧪 Event Bus Testing Guide

## 📋 كيفية اختبار Domain Events

### **ما سيحدث عند اختبار Domain Events:**

عندما تقوم بإنشاء منتج أو تغيير الكمية، سترى رسائل **Log** في الـ Console تظهر تلقائياً من الـ Event Handlers!

---

## 🔔 Test Scenarios

### **Test 1: Create Product - ProductCreatedEvent**

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Content-Type: application/json

{
  "name": "Test Event Product",
  "price": 199.99,
  "stock": 50,
  "categoryId": "{{category_id}}",
  "description": "Testing Domain Events"
}
```

**Expected Console Output:**
```
🎉 New Product Created: Test Event Product (ID: xxx-xxx-xxx) - Price: $199.99, Stock: 50
```

**Handler:** `ProductCreatedEventHandler`

---

### **Test 2: Increase Stock - ProductStockChangedEvent**

**Scenario:** استخدام Update Product لزيادة الكمية

**Request:**
```http
PUT /api/app/product/{{product_id}}
Authorization: Bearer {{access_token}}
Content-Type: application/json

{
  "name": "Test Event Product",
  "price": 199.99,
  "stock": 100,
  "categoryId": "{{category_id}}"
}
```

**Expected Console Output:**
```
📈 Stock Changed: Test Event Product - 50 → 100 (Increased: 50)
```

**Handler:** `ProductStockChangedEventHandler`

---

### **Test 3: Decrease Stock - ProductStockChangedEvent**

**Scenario:** استخدام Update Product لتقليل الكمية

**Request:**
```http
PUT /api/app/product/{{product_id}}
Authorization: Bearer {{access_token}}
Content-Type: application/json

{
  "name": "Test Event Product",
  "price": 199.99,
  "stock": 5,
  "categoryId": "{{category_id}}"
}
```

**Expected Console Output:**
```
📉 Stock Changed: Test Event Product - 100 → 5 (Decreased: 95)
⚠️ Low Stock Alert: Test Event Product - Only 5 items left!
```

**Handler:** `ProductStockChangedEventHandler` (مع Low Stock Warning!)

---

## 📊 كيف تعمل Events

### **Flow:**

```
1. User Request (Create/Update Product)
   ↓
2. ProductAppService.CreateAsync()
   ↓
3. Repository.InsertAsync(product)
   ↓
4. UnitOfWork commits transaction
   ↓
5. Event published: ProductCreatedEvent
   ↓
6. ProductCreatedEventHandler.HandleEventAsync()
   ↓
7. Log message to console: 🎉
```

---

### **Stock Change Flow:**

```
1. User Update Product (change stock)
   ↓
2. ProductAppService.UpdateAsync()
   ↓
3. product.SetStock(newStock)
   ↓
4. Product.AddLocalEvent(ProductStockChangedEvent)
   ↓
5. Repository.UpdateAsync(product)
   ↓
6. UnitOfWork commits
   ↓
7. Event published
   ↓
8. ProductStockChangedEventHandler.HandleEventAsync()
   ↓
9. Log message: 📈 or 📉
   ↓
10. If stock < 10: ⚠️ Low Stock Alert
```

---

## 🎯 Event Handlers

### **1. ProductCreatedEventHandler**

**Location:** `Application/Products/EventHandlers/ProductCreatedEventHandler.cs`

**Purpose:** Log عند إنشاء منتج جديد

**Log Format:**
```
🎉 New Product Created: {ProductName} (ID: {ProductId}) - Price: ${Price}, Stock: {Stock}
```

---

### **2. ProductStockChangedEventHandler**

**Location:** `Application/Products/EventHandlers/ProductStockChangedEventHandler.cs`

**Purpose:** 
- Log عند تغيير الكمية
- تنبيه إذا الكمية أقل من 10

**Log Format (Increase):**
```
📈 Stock Changed: {ProductName} - {OldStock} → {NewStock} (Increased: {ChangeAmount})
```

**Log Format (Decrease):**
```
📉 Stock Changed: {ProductName} - {OldStock} → {NewStock} (Decreased: {ChangeAmount})
```

**Low Stock Warning:**
```
⚠️ Low Stock Alert: {ProductName} - Only {Stock} items left!
```

---

## 💡 Extensibility

### **يمكن إضافة Handlers جديدة بسهولة:**

**مثال: إرسال Email**
```csharp
public class ProductCreatedEmailHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        // إرسال email للمسؤولين
        await _emailSender.SendAsync(...);
    }
}
```

**مثال: تحديث Statistics**
```csharp
public class ProductStatisticsHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        // تحديث الإحصائيات
        await _statisticsRepo.IncrementAsync("TotalProducts");
    }
}
```

**مثال: Cache Invalidation**
```csharp
public class ProductCacheHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        // مسح الـ cache
        await _cache.RemoveAsync("ProductList");
    }
}
```

**كل Handler مستقل تماماً! يمكن إضافة/إزالة/تعديل بدون تأثير على الباقي.**

---

## 📝 ملاحظات مهمة

### **✅ Advantages:**

1. **Loose Coupling** - الـ Handlers منفصلة تماماً
2. **Single Responsibility** - كل Handler يفعل شيء واحد
3. **Extensible** - سهل إضافة handlers جديدة
4. **Testable** - كل Handler قابل للاختبار منفصل
5. **Transaction-Safe** - Events تُنشر بعد commit

### **🔍 Where to Check:**

- **Console Output** - Terminal حيث يعمل `dotnet run`
- **Log Files** - إذا كان logging to file مفعل
- **Application Insights** - في Production

---

## 🚀 الخلاصة

**Domain Events تسمح لك:**
- ✅ فصل Business Logic
- ✅ إضافة Side Effects بسهولة
- ✅ تسجيل الأحداث المهمة
- ✅ إرسال تنبيهات
- ✅ تحديث Statistics
- ✅ Invalidate Cache
- ✅ Integration مع خدمات أخرى

**كل هذا بدون تعقيد الكود الأساسي!**

---

**جرب الـ Tests في Postman وشاهد الـ Logs! 🔔**
