# 🌍 Localization Test Scenarios - Postman

## 📋 نظرة عامة

هذا الملف يحتوي على سيناريوهات اختبار شاملة للـ Localization (تعدد اللغات) في Postman.

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

## 🌐 طرق تغيير اللغة

### **الطريقة 1: Accept-Language Header** ⭐ (الأفضل)

```http
GET /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: ar-SA
```

```http
GET /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: en-US
```

### **الطريقة 2: Query String**

```http
GET /api/app/product?culture=ar-SA
Authorization: Bearer {{access_token}}
```

### **الطريقة 3: Cookie**

```http
GET /api/app/product
Authorization: Bearer {{access_token}}
Cookie: Abp.Culture=c=ar-SA|uic=ar-SA
```

---

## 🧪 Error Messages Localization Tests

### **Test 1: Category Not Found - English** 🇬🇧

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: en-US
Content-Type: application/json

{
  "name": "Test Product",
  "price": 100,
  "stock": 10,
  "categoryId": "00000000-0000-0000-0000-000000000000",
  "description": "Testing localization"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:CategoryNotFound",
    "message": "Category not found",
    "details": null,
    "data": {
      "CategoryId": "00000000-0000-0000-0000-000000000000"
    }
  }
}
```

---

### **Test 2: Category Not Found - Arabic** 🇸🇦

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: ar-SA
Content-Type: application/json

{
  "name": "Test Product",
  "price": 100,
  "stock": 10,
  "categoryId": "00000000-0000-0000-0000-000000000000",
  "description": "Testing localization"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:CategoryNotFound",
    "message": "التصنيف غير موجود",
    "details": null,
    "data": {
      "CategoryId": "00000000-0000-0000-0000-000000000000"
    }
  }
}
```

---

### **Test 3: Duplicate Product Name - English** 🇬🇧

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: en-US
Content-Type: application/json

{
  "name": "Laptop Pro 15",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:DuplicateProductName",
    "message": "A product with this name already exists",
    "data": {
      "ProductName": "Laptop Pro 15"
    }
  }
}
```

---

### **Test 4: Duplicate Product Name - Arabic** 🇸🇦

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: ar-SA
Content-Type: application/json

{
  "name": "Laptop Pro 15",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:DuplicateProductName",
    "message": "يوجد منتج بنفس الاسم بالفعل",
    "data": {
      "ProductName": "Laptop Pro 15"
    }
  }
}
```

---

### **Test 5: Invalid Product Price - English** 🇬🇧

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: en-US
Content-Type: application/json

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
    "message": "Invalid product price. Price must be between 0.01 and 1000000",
    "data": {
      "Price": 2000000,
      "MinPrice": 0.01,
      "MaxPrice": 1000000
    }
  }
}
```

---

### **Test 6: Invalid Product Price - Arabic** 🇸🇦

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: ar-SA
Content-Type: application/json

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
    "message": "سعر المنتج غير صحيح. يجب أن يكون السعر بين 0.01 و 1000000",
    "data": {
      "Price": 2000000,
      "MinPrice": 0.01,
      "MaxPrice": 1000000
    }
  }
}
```

---

### **Test 7: Invalid Stock - English** 🇬🇧

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: en-US
Content-Type: application/json

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
    "message": "Invalid stock quantity. Stock must be between 0 and 100000"
  }
}
```

---

### **Test 8: Invalid Stock - Arabic** 🇸🇦

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Accept-Language: ar-SA
Content-Type: application/json

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
    "message": "الكمية غير صحيحة. يجب أن تكون الكمية بين 0 و 100000"
  }
}
```

---

### **Test 9: Duplicate Category Name - English** 🇬🇧

**Request:**
```http
POST /api/app/category
Authorization: Bearer {{access_token}}
Accept-Language: en-US
Content-Type: application/json

{
  "name": "Electronics",
  "description": "Testing localization"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:DuplicateCategoryName",
    "message": "A category with this name already exists",
    "data": {
      "CategoryName": "Electronics"
    }
  }
}
```

---

### **Test 10: Duplicate Category Name - Arabic** 🇸🇦

**Request:**
```http
POST /api/app/category
Authorization: Bearer {{access_token}}
Accept-Language: ar-SA
Content-Type: application/json

{
  "name": "Electronics",
  "description": "Testing localization"
}
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:DuplicateCategoryName",
    "message": "يوجد تصنيف بنفس الاسم بالفعل",
    "data": {
      "CategoryName": "Electronics"
    }
  }
}
```

---

### **Test 11: Category Has Products - English** 🇬🇧

**Request:**
```http
DELETE /api/app/category/3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f
Authorization: Bearer {{access_token}}
Accept-Language: en-US
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:CategoryHasProducts",
    "message": "Cannot delete category because it has products",
    "data": {
      "CategoryName": "Electronics",
      "CategoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
    }
  }
}
```

---

### **Test 12: Category Has Products - Arabic** 🇸🇦

**Request:**
```http
DELETE /api/app/category/3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f
Authorization: Bearer {{access_token}}
Accept-Language: ar-SA
```

**Expected Response: 403 Forbidden**
```json
{
  "error": {
    "code": "Reference:CategoryHasProducts",
    "message": "لا يمكن حذف التصنيف لأنه يحتوي على منتجات",
    "data": {
      "CategoryName": "Electronics",
      "CategoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
    }
  }
}
```

---

### **Test 13: Entity Not Found - English** 🇬🇧

**Request:**
```http
GET /api/app/product/00000000-0000-0000-0000-000000000000
Authorization: Bearer {{access_token}}
Accept-Language: en-US
```

**Expected Response: 404 Not Found**
```json
{
  "error": {
    "message": "There is no such an entity. Entity type: Product, id: 00000000-0000-0000-0000-000000000000"
  }
}
```

---

### **Test 14: Entity Not Found - Arabic** 🇸🇦

**Request:**
```http
GET /api/app/product/00000000-0000-0000-0000-000000000000
Authorization: Bearer {{access_token}}
Accept-Language: ar-SA
```

**Expected Response: 404 Not Found**
```json
{
  "error": {
    "message": "There is no such an entity. Entity type: Product, id: 00000000-0000-0000-0000-000000000000"
  }
}
```

---

## 📊 Test Results Summary

| Test # | Scenario | Language | Expected | Status |
|--------|----------|----------|----------|--------|
| 1 | Category not found | EN 🇬🇧 | 403 | ⬜ |
| 2 | Category not found | AR 🇸🇦 | 403 | ⬜ |
| 3 | Duplicate product | EN 🇬🇧 | 403 | ⬜ |
| 4 | Duplicate product | AR 🇸🇦 | 403 | ⬜ |
| 5 | Invalid price | EN 🇬🇧 | 403 | ⬜ |
| 6 | Invalid price | AR 🇸🇦 | 403 | ⬜ |
| 7 | Invalid stock | EN 🇬🇧 | 403 | ⬜ |
| 8 | Invalid stock | AR 🇸🇦 | 403 | ⬜ |
| 9 | Duplicate category | EN 🇬🇧 | 403 | ⬜ |
| 10 | Duplicate category | AR 🇸🇦 | 403 | ⬜ |
| 11 | Category has products | EN 🇬🇧 | 403 | ⬜ |
| 12 | Category has products | AR 🇸🇦 | 403 | ⬜ |
| 13 | Entity not found | EN 🇬🇧 | 404 | ⬜ |
| 14 | Entity not found | AR 🇸🇦 | 404 | ⬜ |

**عند الانتهاء، ضع ✅ أو ❌ في عمود Status**

---

## 💡 ملاحظات مهمة

### **Supported Languages:**

| Code | Language | Flag |
|------|----------|------|
| en-US | English (US) | 🇺🇸 |
| en-GB | English (UK) | 🇬🇧 |
| ar-SA | Arabic (Saudi) | 🇸🇦 |
| ar | Arabic | 🇸🇦 |
| de-DE | German | 🇩🇪 |
| fr | French | 🇫🇷 |
| es | Spanish | 🇪🇸 |

**ملحوظة:** العربية والإنجليزية مطبقة كاملاً. اللغات الأخرى تحتاج ترجمة.

---

### **ABP Localization Flow:**

```
Request with Accept-Language header
  ↓
ABP reads header (ar-SA / en-US)
  ↓
Sets CurrentCulture
  ↓
BusinessException thrown
  ↓
ABP looks up error code in localization files
  ↓
Returns message in requested language
  ↓
Response with localized error message
```

---

### **Best Practices:**

1. ✅ **Always use Accept-Language header** في Postman
2. ✅ **Test both languages** لكل error scenario
3. ✅ **Verify message content** ليس فقط status code
4. ✅ **Check parameters** في رسائل الخطأ
5. ✅ **Test all error codes** المعرفة في ReferenceDomainErrorCodes

---

## 🔄 Adding a New Language

### **خطوات إضافة لغة جديدة:**

**1. إنشاء ملف JSON جديد:**
```
Domain.Shared/Localization/Reference/fr.json
```

**2. نسخ من en.json وترجمة:**
```json
{
  "culture": "fr",
  "texts": {
    "Reference:CategoryNotFound": "Catégorie introuvable",
    "Reference:DuplicateProductName": "Un produit avec ce nom existe déjà",
    ...
  }
}
```

**3. Build & Test:**
```powershell
dotnet build
dotnet run
```

**4. Test في Postman:**
```http
Accept-Language: fr
```

---

## 🚀 الخلاصة

**Localization في ABP:**
- ✅ **Automatic** - ABP يترجم تلقائياً
- ✅ **Multi-language** - دعم لغات متعددة
- ✅ **Centralized** - كل النصوص في JSON files
- ✅ **Easy to maintain** - سهل التعديل والإضافة
- ✅ **Professional UX** - المستخدم يرى كل شيء بلغته

**Error Messages المترجمة:**
- ✅ CategoryNotFound
- ✅ DuplicateCategoryName
- ✅ CategoryHasProducts
- ✅ InvalidProductPrice
- ✅ InvalidProductStock
- ✅ DuplicateProductName
- ✅ InsufficientStock
- ✅ InvalidCategoryName

**Permissions المترجمة:**
- ✅ Product Management
- ✅ Category Management
- ✅ Create/Edit/Delete permissions

---

**جرب كل السيناريوهات في Postman! 🌍**
