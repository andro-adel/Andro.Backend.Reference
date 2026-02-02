# 🧪 Validation Testing Scenarios - Postman

## 📋 نظرة عامة

هذا الملف يحتوي على سيناريوهات اختبار شاملة للـ Validation في Postman.

---

## 🔐 التحضير

### **1. Login أولاً:**
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

## 🧪 Product Validation Tests

### **Test 1: Required Field - Name Empty** ❌

**Request:**
```http
POST /api/app/product
Authorization: Bearer {{access_token}}
Content-Type: application/json

{
  "name": "",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400 Bad Request**
```json
{
  "error": {
    "code": "400",
    "message": "Your request is not valid!",
    "validationErrors": [
      {
        "message": "Product name is required",
        "members": ["name"]
      }
    ]
  }
}
```

---

### **Test 2: String Length - Too Short** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "AB",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Product name must be between 3 and 128 characters",
      "members": ["name"]
    }
  ]
}
```

---

### **Test 3: String Length - Too Long** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "A very long product name that exceeds the maximum allowed length of 128 characters and should fail validation because it is way too long for a product name AAAAAAAAAAAAAAAAAAAAAAAAA",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400**

---

### **Test 4: Price - Negative Value** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product",
  "price": -50,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Price must be between 0.01 and 1000000",
      "members": ["price"]
    }
  ]
}
```

---

### **Test 5: Price - Zero** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product",
  "price": 0,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Price must be between 0.01 and 1000000"
    }
  ]
}
```

---

### **Test 6: Price - Too High** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Expensive Product",
  "price": 2000000,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400**

---

### **Test 7: Stock - Negative** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product",
  "price": 100,
  "stock": -5,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Stock must be between 0 and 100000",
      "members": ["stock"]
    }
  ]
}
```

---

### **Test 8: Stock - Too High** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product",
  "price": 100,
  "stock": 200000,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400**

---

### **Test 9: CategoryId - Missing** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product",
  "price": 100,
  "stock": 10
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Category is required",
      "members": ["categoryId"]
    }
  ]
}
```

---

### **Test 10: Description - Too Long** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f",
  "description": "Very long description... (1001+ characters)"
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Description cannot exceed 1000 characters"
    }
  ]
}
```

---

### **Test 11: Multiple Validation Errors** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "AB",
  "price": -10,
  "stock": -5
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Product name must be between 3 and 128 characters",
      "members": ["name"]
    },
    {
      "message": "Price must be between 0.01 and 1000000",
      "members": ["price"]
    },
    {
      "message": "Stock must be between 0 and 100000",
      "members": ["stock"]
    },
    {
      "message": "Category is required",
      "members": ["categoryId"]
    }
  ]
}
```

---

### **Test 12: Valid Product - Should Succeed** ✅

**Request:**
```http
POST /api/app/product
{
  "name": "Test Validation Product",
  "price": 99.99,
  "stock": 50,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f",
  "description": "This is a valid product for testing validation"
}
```

**Expected Response: 200 OK**
```json
{
  "id": "...",
  "name": "Test Validation Product",
  "price": 99.99,
  "stock": 50,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f",
  "categoryName": "Electronics",
  "description": "This is a valid product for testing validation"
}
```

---

## 🧪 Category Validation Tests

### **Test 13: Category Name - Empty** ❌

**Request:**
```http
POST /api/app/category
{
  "name": "",
  "description": "Test category"
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Category name is required",
      "members": ["name"]
    }
  ]
}
```

---

### **Test 14: Category Name - Too Short** ❌

**Request:**
```http
POST /api/app/category
{
  "name": "AB",
  "description": "Test category"
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Category name must be between 3 and 128 characters"
    }
  ]
}
```

---

### **Test 15: Category Description - Too Long** ❌

**Request:**
```http
POST /api/app/category
{
  "name": "Test Category",
  "description": "Very long description... (513+ characters)"
}
```

**Expected Response: 400**
```json
{
  "validationErrors": [
    {
      "message": "Description cannot exceed 512 characters"
    }
  ]
}
```

---

### **Test 16: Valid Category - Should Succeed** ✅

**Request:**
```http
POST /api/app/category
{
  "name": "Test Validation Category",
  "description": "This is a valid category for testing validation"
}
```

**Expected Response: 200 OK**
```json
{
  "id": "...",
  "name": "Test Validation Category",
  "description": "This is a valid category for testing validation"
}
```

---

## 🧪 Update Validation Tests

### **Test 17: Update Product - Invalid Price** ❌

**Request:**
```http
PUT /api/app/product/{{product_id}}
{
  "name": "Updated Product",
  "price": -10,
  "stock": 20,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 400**

---

### **Test 18: Update Category - Valid** ✅

**Request:**
```http
PUT /api/app/category/{{category_id}}
{
  "name": "Updated Category Name",
  "description": "Updated description"
}
```

**Expected Response: 200 OK**

---

## 📊 Test Results Summary

| Test # | Scenario | Expected | Status |
|--------|----------|----------|--------|
| 1 | Name empty | 400 | ⬜ |
| 2 | Name too short | 400 | ⬜ |
| 3 | Name too long | 400 | ⬜ |
| 4 | Price negative | 400 | ⬜ |
| 5 | Price zero | 400 | ⬜ |
| 6 | Price too high | 400 | ⬜ |
| 7 | Stock negative | 400 | ⬜ |
| 8 | Stock too high | 400 | ⬜ |
| 9 | CategoryId missing | 400 | ⬜ |
| 10 | Description too long | 400 | ⬜ |
| 11 | Multiple errors | 400 | ⬜ |
| 12 | Valid product | 200 | ⬜ |
| 13 | Category name empty | 400 | ⬜ |
| 14 | Category name short | 400 | ⬜ |
| 15 | Category desc long | 400 | ⬜ |
| 16 | Valid category | 200 | ⬜ |
| 17 | Update invalid | 400 | ⬜ |
| 18 | Update valid | 200 | ⬜ |

**عند الانتهاء، ضع ✅ أو ❌ في عمود Status**

---

## 💡 ملاحظات مهمة

### **Validation يعمل تلقائياً:**
- ABP يقوم بالـ validation قبل Application Service
- لا حاجة لكتابة كود validation يدوي
- الرسائل واضحة ومفيدة

### **Best Practices:**
1. ✅ اختبر كل validation rule
2. ✅ اختبر multiple errors معاً
3. ✅ تأكد من رسائل الخطأ واضحة
4. ✅ اختبر الحدود (min, max)
5. ✅ اختبر valid cases أيضاً

---

## 🎯 الخلاصة

**Validation في ABP:**
- ✅ Automatic
- ✅ Declarative (Data Annotations)
- ✅ Clear error messages
- ✅ Multiple errors supported
- ✅ Consistent across all APIs

**جرب كل السيناريوهات في Postman! 🚀**
