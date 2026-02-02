# 🚨 Exception Handling في ABP.io - دليل شامل

## 📋 نظرة عامة

Exception Handling هو عملية معالجة الأخطاء والاستثناءات التي تحدث في التطبيق بطريقة احترافية، مع تقديم رسائل واضحة للمستخدم وتسجيل الأخطاء للمطورين.

---

## 🎯 أهمية Exception Handling

### ✅ **الفوائد:**

1. **User Experience** - رسائل خطأ واضحة ومفيدة للمستخدم
2. **Security** - عدم كشف تفاصيل تقنية حساسة
3. **Debugging** - سهولة تتبع الأخطاء وإصلاحها
4. **Consistency** - معالجة موحدة للأخطاء في كل التطبيق
5. **Logging** - تسجيل تلقائي للأخطاء
6. **HTTP Status Codes** - استخدام صحيح لـ HTTP status codes

### ⚠️ **بدون Exception Handling:**

- ❌ رسائل خطأ تقنية مربكة للمستخدم
- ❌ Stack traces مكشوفة (security risk)
- ❌ صعوبة في تتبع الأخطاء
- ❌ تجربة مستخدم سيئة

---

## 📚 أنواع Exceptions في ABP

### **1️⃣ Built-in ABP Exceptions**

| Exception | متى تستخدمها | HTTP Status |
|-----------|--------------|-------------|
| `AbpValidationException` | Validation errors | 400 |
| `AbpAuthorizationException` | Permission denied | 403 |
| `EntityNotFoundException` | Entity not found | 404 |
| `BusinessException` | Business rule violation | 403 |
| `UserFriendlyException` | User-friendly errors | 500 |

### **2️⃣ Custom Business Exceptions**

للقواعد الخاصة بالتطبيق (Business Rules)

---

## 🔧 1. ABP Built-in Exceptions

### **EntityNotFoundException:**

```csharp
using Volo.Abp;
using Volo.Abp.Domain.Entities;

public async Task<ProductDto> GetAsync(Guid id)
{
    var product = await _productRepository.FindAsync(id);
    
    if (product == null)
    {
        throw new EntityNotFoundException(typeof(Product), id);
    }
    
    // الكود...
}
```

**Response:**
```json
{
  "error": {
    "code": "404",
    "message": "There is no such an entity. Entity type: Product, id: xxx"
  }
}
```

---

### **AbpAuthorizationException:**

```csharp
using Volo.Abp.Authorization;

public async Task DeleteAsync(Guid id)
{
    // ABP يرمي هذا تلقائياً عند فشل [Authorize]
    await CheckPolicyAsync(ReferencePermissions.Products.Delete);
    
    // أو يدوياً:
    if (!await AuthorizationService.IsGrantedAsync(ReferencePermissions.Products.Delete))
    {
        throw new AbpAuthorizationException("Permission denied!");
    }
}
```

**Response:**
```json
{
  "error": {
    "code": "403",
    "message": "Authorization failed! Given policy has not been granted."
  }
}
```

---

### **AbpValidationException:**

```csharp
using Volo.Abp.Validation;

public async Task CreateAsync(CreateProductDto input)
{
    // ABP يرمي هذا تلقائياً للـ Data Annotations
    
    // أو يدوياً:
    if (input.Price <= 0)
    {
        throw new AbpValidationException(
            "Price must be greater than zero",
            new List<ValidationResult>
            {
                new ValidationResult(
                    "Price must be greater than zero",
                    new[] { "Price" }
                )
            }
        );
    }
}
```

---

## 🏢 2. Business Exceptions - القواعد الخاصة بالتطبيق

### **BusinessException:**

```csharp
using Volo.Abp;

public async Task DeleteCategoryAsync(Guid id)
{
    var category = await _categoryRepository.GetAsync(id);
    
    // التحقق من قاعدة عمل: لا يمكن حذف Category لها Products
    var hasProducts = await _productRepository
        .AnyAsync(p => p.CategoryId == id);
    
    if (hasProducts)
    {
        throw new BusinessException(ReferenceDomainErrorCodes.CategoryHasProducts)
            .WithData("CategoryName", category.Name)
            .WithData("CategoryId", id);
    }
    
    await _categoryRepository.DeleteAsync(category);
}
```

**Error Code Definition:**
```csharp
// ReferenceDomainErrorCodes.cs
public const string CategoryHasProducts = "Reference:CategoryHasProducts";
```

**Response:**
```json
{
  "error": {
    "code": "Reference:CategoryHasProducts",
    "message": "Cannot delete category because it has products",
    "details": null,
    "data": {
      "CategoryName": "Electronics",
      "CategoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
    }
  }
}
```

---

## 🎨 3. Custom Business Exception Classes

### **إنشاء Exception مخصص:**

```csharp
using Volo.Abp;

namespace Andro.Backend.Reference.Products
{
    public class InsufficientStockException : BusinessException
    {
        public InsufficientStockException(
            string productName, 
            int requestedQuantity, 
            int availableStock)
            : base(ReferenceDomainErrorCodes.InsufficientStock)
        {
            WithData("ProductName", productName);
            WithData("RequestedQuantity", requestedQuantity);
            WithData("AvailableStock", availableStock);
        }
    }
}
```

### **الاستخدام:**

```csharp
public async Task<OrderDto> CreateOrderAsync(CreateOrderDto input)
{
    var product = await _productRepository.GetAsync(input.ProductId);
    
    if (product.Stock < input.Quantity)
    {
        throw new InsufficientStockException(
            product.Name,
            input.Quantity,
            product.Stock
        );
    }
    
    // إنشاء الطلب...
}
```

---

## 🔗 4. Domain Layer Validation مع Exceptions

### **في الـ Entity نفسه:**

```csharp
namespace Andro.Backend.Reference.Products
{
    public class Product : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }

        protected Product()
        {
            Name = string.Empty;
        }

        public Product(
            Guid id,
            string name,
            decimal price,
            int stock,
            Guid categoryId,
            string? description = null) : base(id)
        {
            SetName(name);
            SetPrice(price);
            SetStock(stock);
            CategoryId = categoryId;
            Description = description;
        }

        public void SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(
                name,
                nameof(name),
                ProductConsts.MaxNameLength
            );
        }

        public void SetPrice(decimal price)
        {
            if (price < ProductConsts.MinPrice || price > ProductConsts.MaxPrice)
            {
                throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductPrice)
                    .WithData("Price", price)
                    .WithData("MinPrice", ProductConsts.MinPrice)
                    .WithData("MaxPrice", ProductConsts.MaxPrice);
            }
            
            Price = price;
        }

        public void SetStock(int stock)
        {
            if (stock < ProductConsts.MinStock || stock > ProductConsts.MaxStock)
            {
                throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductStock)
                    .WithData("Stock", stock)
                    .WithData("MinStock", ProductConsts.MinStock)
                    .WithData("MaxStock", ProductConsts.MaxStock);
            }
            
            Stock = stock;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductStock)
                    .WithData("Quantity", quantity);
            }

            if (Stock < quantity)
            {
                throw new InsufficientStockException(Name, quantity, Stock);
            }

            Stock -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductStock)
                    .WithData("Quantity", quantity);
            }

            Stock += quantity;
        }
    }
}
```

---

## 🎯 5. Application Service مع Exception Handling

### **مثال كامل:**

```csharp
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Andro.Backend.Reference.Products
{
    public class ProductAppService : ApplicationService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Category, Guid> _categoryRepository;

        public ProductAppService(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto input)
        {
            // 1. التحقق من وجود Category
            var categoryExists = await _categoryRepository
                .AnyAsync(c => c.Id == input.CategoryId);
            
            if (!categoryExists)
            {
                throw new BusinessException(ReferenceDomainErrorCodes.CategoryNotFound)
                    .WithData("CategoryId", input.CategoryId);
            }

            // 2. التحقق من عدم تكرار الاسم
            var existingProduct = await _productRepository
                .FirstOrDefaultAsync(p => p.Name == input.Name);
            
            if (existingProduct != null)
            {
                throw new BusinessException(ReferenceDomainErrorCodes.DuplicateProductName)
                    .WithData("ProductName", input.Name);
            }

            // 3. إنشاء Product
            // Domain validation يحدث هنا في Constructor
            var product = new Product(
                GuidGenerator.Create(),
                input.Name,
                input.Price,
                input.Stock,
                input.CategoryId,
                input.Description
            );

            await _productRepository.InsertAsync(product);

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public async Task<ProductDto> GetAsync(Guid id)
        {
            var product = await _productRepository.FindAsync(id);

            if (product == null)
            {
                throw new EntityNotFoundException(typeof(Product), id);
            }

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetAsync(id);

            // يمكن إضافة Business Rules هنا
            // مثلاً: لا يمكن حذف Product في طلبات نشطة

            await _productRepository.DeleteAsync(product);
        }
    }
}
```

---

## 🧪 6. Testing Exceptions في Postman

### **Test 1: Category Not Found** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Test Product",
  "price": 100,
  "stock": 10,
  "categoryId": "00000000-0000-0000-0000-000000000000"
}
```

**Expected Response: 403**
```json
{
  "error": {
    "code": "Reference:CategoryNotFound",
    "message": "Category not found",
    "data": {
      "CategoryId": "00000000-0000-0000-0000-000000000000"
    }
  }
}
```

---

### **Test 2: Duplicate Product Name** ❌

**Request:**
```http
POST /api/app/product
{
  "name": "Laptop Pro 15",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response: 403**
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

### **Test 3: Entity Not Found** ❌

**Request:**
```http
GET /api/app/product/00000000-0000-0000-0000-000000000000
```

**Expected Response: 404**
```json
{
  "error": {
    "code": "404",
    "message": "There is no such an entity. Entity type: Product, id: 00000000-0000-0000-0000-000000000000"
  }
}
```

---

### **Test 4: Cannot Delete Category with Products** ❌

**Request:**
```http
DELETE /api/app/category/3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f
```

**Expected Response: 403**
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

## 📝 Best Practices

### ✅ **Do:**

1. ✅ استخدم `BusinessException` للـ business rules
2. ✅ استخدم `EntityNotFoundException` للـ entities المفقودة
3. ✅ استخدم Error Codes (`ReferenceDomainErrorCodes`)
4. ✅ أضف `WithData()` لمعلومات إضافية
5. ✅ ضع Domain Validation في Entities
6. ✅ ضع Business Validation في Application Services
7. ✅ استخدم رسائل خطأ واضحة ومفيدة
8. ✅ لا تكشف تفاصيل تقنية حساسة

### ❌ **Don't:**

1. ❌ لا ترمي `Exception` عادي - استخدم ABP exceptions
2. ❌ لا تكشف Stack Traces للمستخدم
3. ❌ لا تضع Business Logic في catch blocks
4. ❌ لا تتجاهل Exceptions بدون معالجة
5. ❌ لا تستخدم Exceptions للـ control flow
6. ❌ لا تكرر نفس رسائل الخطأ - استخدم constants

---

## 🔄 Exception Handling Workflow

```
Request → ABP Middleware
          ↓
      Validation (Data Annotations)
          ↓
      Application Service
          ↓
      Business Validation
          ↓ (Exception thrown?)
         Yes → BusinessException
          ↓
      Domain Layer
          ↓ (Domain rule violated?)
         Yes → BusinessException
          ↓
      Repository
          ↓ (Entity not found?)
         Yes → EntityNotFoundException
          ↓
      Success → Return 200
      
Exception → ABP Exception Filter
          ↓
      Log Exception
          ↓
      Format Error Response
          ↓
      Return appropriate HTTP Status
```

---

## 📊 HTTP Status Codes المستخدمة

| Status | متى يستخدم | ABP Exception |
|--------|-----------|---------------|
| 400 | Validation errors | `AbpValidationException` |
| 401 | Not authenticated | `AbpAuthorizationException` |
| 403 | No permission / Business rule | `BusinessException` |
| 404 | Entity not found | `EntityNotFoundException` |
| 500 | Server error | `Exception` |

---

## 💡 مثال: Localization للـ Exception Messages

### **في ملف Localization:**

```json
{
  "Reference:CategoryNotFound": "التصنيف غير موجود",
  "Reference:DuplicateProductName": "يوجد منتج بنفس الاسم بالفعل",
  "Reference:CategoryHasProducts": "لا يمكن حذف التصنيف لأنه يحتوي على منتجات",
  "Reference:InsufficientStock": "الكمية المطلوبة غير متوفرة في المخزون"
}
```

### **الاستخدام:**

```csharp
throw new BusinessException(ReferenceDomainErrorCodes.CategoryNotFound)
    .WithData("CategoryId", id);

// ABP سيستخدم Localization تلقائياً
```

---

## 🚀 الخلاصة

**Exception Handling في ABP:**
- ✅ **Built-in exceptions** - جاهزة للاستخدام
- ✅ **Business exceptions** - للقواعد الخاصة
- ✅ **Automatic formatting** - JSON response موحد
- ✅ **Logging** - تسجيل تلقائي
- ✅ **Localization** - دعم تعدد اللغات
- ✅ **HTTP Status Codes** - استخدام صحيح

**Layers:**
1. **DTO Validation** → `AbpValidationException` (400)
2. **Application Service** → `BusinessException` (403)
3. **Domain Layer** → `BusinessException` (403)
4. **Repository** → `EntityNotFoundException` (404)

**Next Steps:**
1. إضافة Business Exceptions
2. تطبيق Domain Validation
3. Testing في Postman
4. Localization (optional)

---

**Exception Handling الصحيح = User Experience ممتاز! 🚨**
