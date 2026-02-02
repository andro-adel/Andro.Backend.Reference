# 🧪 Exception Handling Test Scenarios - Postman

## 📋 نظرة عامة

هذا الملف يحتوي على سيناريوهات اختبار شاملة للـ Exception Handling في Postman.

---

## 🔐 التحضير

### **Login أولاً:**
```http
POST https://localhost:44309/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
username=admin
password=1q2w3E*
client_id=Reference_App
scope=offline_access Reference
```

---

## 🧪 Product Exception Tests

### **Test 1: Entity Not Found (404)** ❌

**Request:**
```http
GET /api/app/product/00000000-0000-0000-0000-000000000000
Authorization: Bearer {{access_token}}
```

**Expected Response: 404 Not Found**
```json
{
  "error": {
    "code": null,
    "message": "There is no such an entity. Entity type: Product, id: 00000000-0000-0000-0000-000000000000",
    "details": null
  }
}
```

---

### **Test 2: Category Not Found (403)** ❌

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Content-Type: application/json

{
  "name": "Test Product",
  "price": 99.99,
  "stock": 10,
  "categoryId": "00000000-0000-0000-0000-000000000000",
  "description": "Testing category not found"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:CategoryNotFound",
    "message": "Business exception occurred",
    "details": null,
    "data": {
      "CategoryId": "00000000-0000-0000-0000-000000000000"
    }
  }
}
```

---

### **Test 3: Duplicate Product Name (403)** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Laptop Pro 15",
  "price": 99.99,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:DuplicateProductName",
    "message": "Business exception occurred",
    "data": {
      "ProductName": "Laptop Pro 15"
    }
  }
}
```

---

### **Test 4: Invalid Price - Domain Validation (403)** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product Invalid Price",
  "price": 2000000,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:InvalidProductPrice",
    "message": "Business exception occurred",
    "data": {
      "Price": 2000000,
      "MinPrice": 0.01,
      "MaxPrice": 1000000
    }
  }
}
```

---

### **Test 5: Invalid Stock - Domain Validation (403)** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product Invalid Stock",
  "price": 99.99,
  "stock": 200000,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:InvalidProductStock",
    "data": {
      "Stock": 200000,
      "MinStock": 0,
      "MaxStock": 100000
    }
  }
}
```

---

### **Test 6: Update with Invalid Category (403)** ❌

**Request:**
```http
PUT /api/app/product/{{product_id}}
{
  "name": "Updated Product",
  "price": 99.99,
  "stock": 10,
  "categoryId": "00000000-0000-0000-0000-000000000000"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:CategoryNotFound",
    "data": {
      "CategoryId": "00000000-0000-0000-0000-000000000000"
    }
  }
}
```

---

### **Test 7: Update with Duplicate Name (403)** ❌

**Scenario:** محاولة تعديل منتج ليصبح له نفس اسم منتج آخر موجود

**Request:**
```http
PUT /api/app/product/{{product_id}}
{
  "name": "Wireless Mouse",
  "price": 99.99,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:DuplicateProductName",
    "data": {
      "ProductName": "Wireless Mouse"
    }
  }
}
```

---

### **Test 8: Valid Product Creation (Success)** ✅

**Request:**
```http
POST /api/app/product
{
  "name": "Test Exception Handling Product",
  "price": 199.99,
  "stock": 25,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f",
  "description": "Testing exception handling"
}
```

**Expected Response: 200 OK**
```json
{
  "id": "...",
  "name": "Test Exception Handling Product",
  "price": 199.99,
  "stock": 25,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f",
  "categoryName": "Electronics",
  "description": "Testing exception handling"
}
```

---

## 🧪 Category Exception Tests

### **Test 9: Category Not Found (404)** ❌

**Request:**
```http
GET /api/app/category/00000000-0000-0000-0000-000000000000
```

**Expected Response: 404 Not Found**
```json
{
  "error": {
    "message": "There is no such an entity. Entity type: Category, id: 00000000-0000-0000-0000-000000000000"
  }
}
```

---

### **Test 10: Duplicate Category Name (403)** ❌

**Request:**
```http
POST /api/app/category
{
  "name": "Electronics",
  "description": "Testing duplicate"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:DuplicateCategoryName",
    "data": {
      "CategoryName": "Electronics"
    }
  }
}
```

---

### **Test 11: Cannot Delete Category with Products (403)** ❌

**Request:**
```http
DELETE /api/app/category/3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:CategoryHasProducts",
    "message": "Business exception occurred",
    "data": {
      "CategoryName": "Electronics",
      "CategoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
    }
  }
}
```

---

### **Test 12: Update Category with Duplicate Name (403)** ❌

**Request:**
```http
PUT /api/app/category/{{category_id}}
{
  "name": "Electronics",
  "description": "Trying to use existing name"
}
```

**Expected Response: 403 Forbidden**

---

### **Test 13: Valid Category Creation (Success)** ✅

**Request:**
```http
POST /api/app/category
{
  "name": "Test Exception Handling Category",
  "description": "Testing exception handling"
}
```

**Expected Response: 200 OK**

---

### **Test 14: Delete Empty Category (Success)** ✅

**Scenario:** إنشاء category جديدة ثم حذفها (بدون products)

**Step 1: Create Category**
```http
POST /api/app/category
{
  "name": "Temporary Category",
  "description": "Will be deleted"
}
```

**Step 2: Delete Category**
```http
DELETE /api/app/category/{{category_id}}
```

**Expected Response: 204 No Content**

---

## 📊 Test Results Summary

| Test # | Scenario | Expected | Status |
|--------|----------|----------|--------|
| 1 | Product not found | 404 | ⬜ |
| 2 | Category not found | 403 | ⬜ |
| 3 | Duplicate product name | 403 | ⬜ |
| 4 | Invalid price (domain) | 403 | ⬜ |
| 5 | Invalid stock (domain) | 403 | ⬜ |
| 6 | Update invalid category | 403 | ⬜ |
| 7 | Update duplicate name | 403 | ⬜ |
| 8 | Valid product | 200 | ⬜ |
| 9 | Category not found | 404 | ⬜ |
| 10 | Duplicate category | 403 | ⬜ |
| 11 | Delete category with products | 403 | ⬜ |
| 12 | Update duplicate category | 403 | ⬜ |
| 13 | Valid category | 200 | ⬜ |
| 14 | Delete empty category | 204 | ⬜ |

**عند الانتهاء، ضع ✅ أو ❌ في عمود Status**

---

## 💡 ملاحظات مهمة

### **HTTP Status Codes:**

| Code | معناها | متى تظهر |
|------|--------|----------|
| 200 | Success | عملية ناجحة |
| 204 | No Content | حذف ناجح |
| 400 | Bad Request | Validation error |
| 403 | Forbidden | Business exception |
| 404 | Not Found | Entity not found |
| 500 | Server Error | خطأ غير متوقع |

### **Exception Types:**

1. **EntityNotFoundException (404)**
   - Entity غير موجود
   - `FindAsync` returns null

2. **BusinessException (403)**
   - Business rule violation
   - Custom error codes
   - `WithData()` للمعلومات الإضافية

3. **AbpValidationException (400)**
   - Data annotations validation
   - تلقائي من ABP

---

## 🎯 Domain vs Application Validation

### **Domain Validation:**
```csharp
// في Product.cs
public void SetPrice(decimal price)
{
    if (price < ProductConsts.MinPrice || price > ProductConsts.MaxPrice)
    {
        throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductPrice)
            .WithData("Price", price);
    }
    Price = price;
}
```

### **Application Validation:**
```csharp
// في ProductAppService.cs
var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.CategoryId);
if (!categoryExists)
{
    throw new BusinessException(ReferenceDomainErrorCodes.CategoryNotFound)
        .WithData("CategoryId", input.CategoryId);
}
```

---

## 🔄 Exception Flow

```
Request
  ↓
DTO Validation (400 - AbpValidationException)
  ↓
Application Service
  ↓
Business Rules Check (403 - BusinessException)
  ↓
Domain Layer
  ↓
Domain Validation (403 - BusinessException)
  ↓
Repository
  ↓
Entity Not Found? (404 - EntityNotFoundException)
  ↓
Success (200/204)
```

---

## ✅ الخلاصة

**Exception Handling في ABP:**
- ✅ **Multi-layered** - عدة مستويات من الـ validation
- ✅ **Clear errors** - رسائل واضحة مع data
- ✅ **Consistent** - نفس الطريقة في كل المشروع
- ✅ **Secure** - لا تكشف تفاصيل تقنية حساسة
- ✅ **Automatic** - ABP يعالج كل شيء تلقائياً

**جرب كل السيناريوهات في Postman! 🚀**
