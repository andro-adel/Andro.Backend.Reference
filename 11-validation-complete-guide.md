# 🛡️ Validation في ABP.io - دليل شامل

## 📋 نظرة عامة

الـ Validation هو عملية التحقق من صحة البيانات قبل معالجتها. في ABP.io، يتم الـ validation تلقائياً على مستوى Application Service.

---

## 🎯 أهمية الـ Validation

### ✅ **الفوائد:**

1. **Data Integrity** - ضمان بيانات صحيحة في قاعدة البيانات
2. **Security** - منع SQL Injection وهجمات أخرى
3. **User Experience** - رسائل خطأ واضحة ومفيدة
4. **Business Rules** - تطبيق قواعد العمل
5. **Automatic** - ABP يقوم بالـ validation تلقائياً
6. **Consistency** - نفس قواعد الـ validation في كل المشروع

### ⚠️ **بدون Validation:**

- ❌ بيانات فاسدة في DB
- ❌ Exceptions غير متوقعة
- ❌ Security vulnerabilities
- ❌ تجربة مستخدم سيئة

---

## 📚 أنواع الـ Validation في ABP

### **1️⃣ Data Annotations Validation**
استخدام Attributes من `System.ComponentModel.DataAnnotations`

### **2️⃣ IValidatableObject**
Validation معقد داخل الـ DTO نفسه

### **3️⃣ FluentValidation**
Library خارجي للـ validation المتقدم

### **4️⃣ Domain Validation**
Business rules في الـ Domain Layer

---

## 🔧 1. Data Annotations Validation

### **Built-in Validators:**

```csharp
using System.ComponentModel.DataAnnotations;

public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(128, MinimumLength = 3, 
        ErrorMessage = "Name must be between 3 and 128 characters")]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0.01, 1000000, 
        ErrorMessage = "Price must be between 0.01 and 1,000,000")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue, 
        ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    [StringLength(1000, 
        ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? ContactEmail { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? Website { get; set; }
}
```

### **Common Validators:**

| Attribute | الوصف | مثال |
|-----------|--------|------|
| `[Required]` | الحقل مطلوب | `[Required]` |
| `[StringLength]` | طول النص | `[StringLength(100, MinimumLength = 3)]` |
| `[Range]` | نطاق الأرقام | `[Range(1, 100)]` |
| `[EmailAddress]` | بريد إلكتروني | `[EmailAddress]` |
| `[Phone]` | رقم هاتف | `[Phone]` |
| `[Url]` | رابط URL | `[Url]` |
| `[RegularExpression]` | Pattern معين | `[RegularExpression(@"^\d{3}-\d{3}-\d{4}$")]` |
| `[Compare]` | مقارنة حقلين | `[Compare("Password")]` |
| `[CreditCard]` | بطاقة ائتمان | `[CreditCard]` |

---

## 🎨 2. Custom Validation Attributes

### **إنشاء Custom Validator:**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Andro.Backend.Reference.Validation
{
    /// <summary>
    /// Validates that a decimal value is positive (greater than zero)
    /// </summary>
    public class PositiveDecimalAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value, 
            ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is decimal decimalValue)
            {
                if (decimalValue <= 0)
                {
                    return new ValidationResult(
                        ErrorMessage ?? "Value must be greater than zero");
                }
            }

            return ValidationResult.Success;
        }
    }
}
```

### **الاستخدام:**

```csharp
public class CreateProductDto
{
    [Required]
    [PositiveDecimal(ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }
}
```

---

## 🔗 3. IValidatableObject - Complex Validation

### **للـ Validation المعقد:**

```csharp
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class CreateProductDto : IValidatableObject
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; }

    public decimal? DiscountPrice { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        // Validate: DiscountPrice must be less than Price
        if (DiscountPrice.HasValue && DiscountPrice >= Price)
        {
            yield return new ValidationResult(
                "Discount price must be less than regular price",
                new[] { nameof(DiscountPrice) }
            );
        }

        // Validate: Stock should be reasonable
        if (Stock > 10000)
        {
            yield return new ValidationResult(
                "Stock quantity seems unusually high. Please verify.",
                new[] { nameof(Stock) }
            );
        }
    }
}
```

---

## 🏢 4. Domain Validation - Business Rules

### **في الـ Domain Layer:**

```csharp
namespace Andro.Backend.Reference.Categories
{
    public class Category : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; }

        protected Category()
        {
            Products = new List<Product>();
        }

        public Category(Guid id, string name, string? description = null)
            : base(id)
        {
            SetName(name);
            Description = description;
            Products = new List<Product>();
        }

        public void SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(
                name, 
                nameof(name), 
                CategoryConsts.MaxNameLength
            );

            // Business Rule: Name must not contain special characters
            if (!IsValidCategoryName(name))
            {
                throw new BusinessException(ReferenceDomainErrorCodes.InvalidCategoryName)
                    .WithData("Name", name);
            }
        }

        private bool IsValidCategoryName(string name)
        {
            // Only letters, numbers, spaces, and hyphens
            return System.Text.RegularExpressions.Regex.IsMatch(
                name, 
                @"^[a-zA-Z0-9\s\-]+$"
            );
        }
    }
}
```

### **Error Codes:**

```csharp
// في ReferenceConsts.cs أو ملف منفصل
namespace Andro.Backend.Reference
{
    public static class ReferenceDomainErrorCodes
    {
        public const string InvalidCategoryName = "Reference:InvalidCategoryName";
        public const string DuplicateCategoryName = "Reference:DuplicateCategoryName";
        public const string ProductPriceTooLow = "Reference:ProductPriceTooLow";
        public const string InsufficientStock = "Reference:InsufficientStock";
    }
}
```

---

## ⚡ 5. ABP Automatic Validation

### **كيف يعمل:**

ABP يقوم بـ validation تلقائياً قبل تنفيذ Application Service method:

```csharp
public class ProductAppService : ApplicationService
{
    // ABP automatically validates CreateProductDto
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // إذا فشل الـ validation، ABP يرمي AbpValidationException
        // ولن يصل الكود إلى هنا
        
        var product = new Product(/* ... */);
        // ...
    }
}
```

### **الـ Response عند فشل Validation:**

```json
{
  "error": {
    "code": "400",
    "message": "Your request is not valid!",
    "validationErrors": [
      {
        "message": "Product name is required",
        "members": ["name"]
      },
      {
        "message": "Price must be greater than zero",
        "members": ["price"]
      }
    ]
  }
}
```

---

## 🧪 6. Testing Validation

### **في Postman:**

#### **Test 1: Required Field**
```http
POST /api/app/product
{
  "name": "",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected:** ❌ 400 Bad Request
```json
{
  "error": {
    "validationErrors": [
      {
        "message": "Product name is required",
        "members": ["name"]
      }
    ]
  }
}
```

#### **Test 2: String Length**
```http
POST /api/app/product
{
  "name": "AB",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected:** ❌ 400 Bad Request
```json
{
  "validationErrors": [
    {
      "message": "Name must be between 3 and 128 characters"
    }
  ]
}
```

#### **Test 3: Range Validation**
```http
POST /api/app/product
{
  "name": "Test Product",
  "price": -50,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected:** ❌ 400 Bad Request
```json
{
  "validationErrors": [
    {
      "message": "Price must be greater than zero"
    }
  ]
}
```

---

## 📝 Best Practices

### ✅ **Do:**

1. ✅ استخدم Data Annotations للـ simple validation
2. ✅ استخدم IValidatableObject للـ cross-property validation
3. ✅ ضع Business Rules في Domain Layer
4. ✅ استخدم رسائل خطأ واضحة ومفيدة
5. ✅ استخدم Error Codes للـ Business Exceptions
6. ✅ اختبر كل validation scenarios
7. ✅ استخدم Constants للـ max lengths

### ❌ **Don't:**

1. ❌ لا تضع Business Logic في DTOs
2. ❌ لا تكرر validation في أماكن متعددة
3. ❌ لا تستخدم magic numbers
4. ❌ لا تترك رسائل خطأ عامة
5. ❌ لا تنسى validation في Update DTOs

---

## 🎯 مثال كامل: ProductDto مع Validation محسن

### **CreateProductDto:**

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace Andro.Backend.Reference.Products
{
    public class CreateProductDto : IValidatableObject
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(
            ProductConsts.MaxNameLength, 
            MinimumLength = ProductConsts.MinNameLength,
            ErrorMessage = "Product name must be between {2} and {1} characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, 
            ErrorMessage = "Price must be between 0.01 and 1,000,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, 100000, 
            ErrorMessage = "Stock must be between 0 and 100,000")]
        public int Stock { get; set; }

        [StringLength(
            ProductConsts.MaxDescriptionLength,
            ErrorMessage = "Description cannot exceed {1} characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            // Additional complex validation if needed
            if (Price < 1 && Stock > 1000)
            {
                yield return new ValidationResult(
                    "Products with price less than $1 cannot have stock greater than 1000",
                    new[] { nameof(Price), nameof(Stock) }
                );
            }
        }
    }
}
```

### **ProductConsts:**

```csharp
namespace Andro.Backend.Reference.Products
{
    public static class ProductConsts
    {
        public const int MaxNameLength = 128;
        public const int MinNameLength = 3;
        public const int MaxDescriptionLength = 1000;
    }
}
```

---

## 🔄 Validation Workflow

```
1. Client sends request
   ↓
2. ABP intercepts before reaching Application Service
   ↓
3. ABP validates DTO using Data Annotations
   ↓
4. If validation fails → return 400 with error details
   ↓
5. If validation passes → execute Application Service
   ↓
6. Application Service may have additional business validation
   ↓
7. Domain Layer has business rules validation
   ↓
8. Return success or business exception
```

---

## 📊 Validation Levels

| Level | Type | Example | When to use |
|-------|------|---------|-------------|
| **DTO** | Data Annotations | `[Required]`, `[Range]` | Simple field validation |
| **DTO** | IValidatableObject | Cross-property checks | Complex DTO validation |
| **Application** | Manual checks | Duplicate check | Application-level rules |
| **Domain** | Business rules | Price rules | Core business logic |

---

## 🚀 الخلاصة

**Validation في ABP:**
- ✅ **Automatic** - يعمل تلقائياً
- ✅ **Multi-layered** - عدة مستويات
- ✅ **Consistent** - نفس الطريقة في كل المشروع
- ✅ **Clear errors** - رسائل خطأ واضحة
- ✅ **Security** - حماية ضد البيانات الخاطئة

**Next Steps:**
1. تحسين DTOs الحالية
2. إضافة Custom Validators
3. إضافة Business Validation
4. Testing في Postman
5. Documentation

---

**الـ Validation هو خط الدفاع الأول ضد البيانات الفاسدة! 🛡️**
