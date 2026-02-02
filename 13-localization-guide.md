# 🌍 Localization في ABP.io - دليل شامل

## 📋 نظرة عامة

Localization (التوطين) هو عملية جعل التطبيق يدعم لغات متعددة، بحيث يمكن للمستخدم اختيار اللغة المفضلة له وعرض كل النصوص (رسائل الأخطاء، التسميات، الأوصاف) بتلك اللغة.

---

## 🎯 أهمية Localization

### ✅ **الفوائد:**

1. **Multi-language Support** - دعم لغات متعددة (عربي، إنجليزي، إلخ)
2. **Better UX** - المستخدم يرى كل شيء بلغته
3. **Global Reach** - تطبيقك يصبح عالمي
4. **Centralized** - كل النصوص في مكان واحد
5. **Easy Maintenance** - تعديل النصوص بسهولة
6. **Professional** - تطبيق احترافي يحترم ثقافة المستخدم

### ⚠️ **بدون Localization:**

- ❌ لغة واحدة فقط (محدود)
- ❌ Hard-coded strings في الكود
- ❌ صعوبة إضافة لغة جديدة
- ❌ تجربة مستخدم سيئة للمستخدمين غير الناطقين بالإنجليزية

---

## 📚 كيف يعمل Localization في ABP

### **المكونات الأساسية:**

```
1. Localization Files (.json)
   ↓
2. IStringLocalizer / IStringLocalizerFactory
   ↓
3. Current Culture (ar-SA, en-US)
   ↓
4. Localized String
```

### **ملفات Localization:**

```
Domain.Shared/
  └── Localization/
      └── Reference/
          ├── en.json    (English)
          ├── ar.json    (Arabic)
          └── ...
```

---

## 🔧 1. بنية ملفات Localization

### **en.json (الإنجليزية):**

```json
{
  "culture": "en",
  "texts": {
    "Menu:Home": "Home",
    "Menu:Products": "Products",
    "Menu:Categories": "Categories",
    
    "Product": "Product",
    "Products": "Products",
    "ProductName": "Product Name",
    "ProductPrice": "Price",
    "ProductStock": "Stock",
    "ProductDescription": "Description",
    
    "Category": "Category",
    "Categories": "Categories",
    "CategoryName": "Category Name",
    
    "Reference:CategoryNotFound": "Category not found",
    "Reference:DuplicateCategoryName": "A category with this name already exists",
    "Reference:CategoryHasProducts": "Cannot delete category because it has products",
    "Reference:InvalidProductPrice": "Invalid product price",
    "Reference:InvalidProductStock": "Invalid stock quantity",
    "Reference:DuplicateProductName": "A product with this name already exists",
    "Reference:InsufficientStock": "Insufficient stock available",
    
    "Permission:Products": "Products Management",
    "Permission:Products.Create": "Create Products",
    "Permission:Products.Edit": "Edit Products",
    "Permission:Products.Delete": "Delete Products",
    
    "Permission:Categories": "Categories Management",
    "Permission:Categories.Create": "Create Categories",
    "Permission:Categories.Edit": "Edit Categories",
    "Permission:Categories.Delete": "Delete Categories"
  }
}
```

### **ar.json (العربية):**

```json
{
  "culture": "ar",
  "texts": {
    "Menu:Home": "الرئيسية",
    "Menu:Products": "المنتجات",
    "Menu:Categories": "التصنيفات",
    
    "Product": "منتج",
    "Products": "المنتجات",
    "ProductName": "اسم المنتج",
    "ProductPrice": "السعر",
    "ProductStock": "الكمية",
    "ProductDescription": "الوصف",
    
    "Category": "تصنيف",
    "Categories": "التصنيفات",
    "CategoryName": "اسم التصنيف",
    
    "Reference:CategoryNotFound": "التصنيف غير موجود",
    "Reference:DuplicateCategoryName": "يوجد تصنيف بنفس الاسم بالفعل",
    "Reference:CategoryHasProducts": "لا يمكن حذف التصنيف لأنه يحتوي على منتجات",
    "Reference:InvalidProductPrice": "سعر المنتج غير صحيح",
    "Reference:InvalidProductStock": "الكمية غير صحيحة",
    "Reference:DuplicateProductName": "يوجد منتج بنفس الاسم بالفعل",
    "Reference:InsufficientStock": "الكمية المتوفرة غير كافية",
    
    "Permission:Products": "إدارة المنتجات",
    "Permission:Products.Create": "إنشاء منتجات",
    "Permission:Products.Edit": "تعديل منتجات",
    "Permission:Products.Delete": "حذف منتجات",
    
    "Permission:Categories": "إدارة التصنيفات",
    "Permission:Categories.Create": "إنشاء تصنيفات",
    "Permission:Categories.Edit": "تعديل تصنيفات",
    "Permission:Categories.Delete": "حذف تصنيفات"
  }
}
```

---

## 🎨 2. استخدام Localization في Application Services

### **في ProductAppService:**

```csharp
using Volo.Abp.Localization;

public class ProductAppService : ApplicationService
{
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // Check if category exists
        var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.CategoryId);
        if (!categoryExists)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.CategoryNotFound)
                .WithData("CategoryId", input.CategoryId);
        }

        // الرسالة المترجمة تأتي تلقائياً من ABP
        // ABP يبحث في ملفات Localization عن المفتاح "Reference:CategoryNotFound"
        // ويرجع النص المناسب حسب اللغة الحالية
    }
}
```

### **استخدام IStringLocalizer مباشرة:**

```csharp
using Volo.Abp.Localization;

public class ProductAppService : ApplicationService
{
    private readonly IStringLocalizer<ReferenceResource> _localizer;

    public ProductAppService(IStringLocalizer<ReferenceResource> localizer)
    {
        _localizer = localizer;
    }

    public async Task<string> GetWelcomeMessageAsync()
    {
        // استخدام المترجم مباشرة
        var message = _localizer["WelcomeMessage"];
        return message;
    }

    public async Task<string> GetFormattedMessageAsync(string userName)
    {
        // استخدام مع Parameters
        var message = _localizer["WelcomeMessage:WithName", userName];
        // في en.json: "WelcomeMessage:WithName": "Welcome, {0}!"
        // في ar.json: "WelcomeMessage:WithName": "مرحباً، {0}!"
        return message;
    }
}
```

---

## 🔗 3. Localization للـ Permissions

### **في ReferencePermissionDefinitionProvider:**

```csharp
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Andro.Backend.Reference.Permissions;

public class ReferencePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var referenceGroup = context.AddGroup(
            ReferencePermissions.GroupName,
            L("Permission:Reference"));

        var productsPermission = referenceGroup.AddPermission(
            ReferencePermissions.Products.Default,
            L("Permission:Products"));

        productsPermission.AddChild(
            ReferencePermissions.Products.Create,
            L("Permission:Products.Create"));

        productsPermission.AddChild(
            ReferencePermissions.Products.Edit,
            L("Permission:Products.Edit"));

        productsPermission.AddChild(
            ReferencePermissions.Products.Delete,
            L("Permission:Products.Delete"));

        // Categories
        var categoriesPermission = referenceGroup.AddPermission(
            ReferencePermissions.Categories.Default,
            L("Permission:Categories"));

        categoriesPermission.AddChild(
            ReferencePermissions.Categories.Create,
            L("Permission:Categories.Create"));

        categoriesPermission.AddChild(
            ReferencePermissions.Categories.Edit,
            L("Permission:Categories.Edit"));

        categoriesPermission.AddChild(
            ReferencePermissions.Categories.Delete,
            L("Permission:Categories.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ReferenceResource>(name);
    }
}
```

---

## 🧪 4. Testing Localization في Postman

### **طريقة تغيير اللغة:**

#### **الطريقة 1: عبر HTTP Header**

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

#### **الطريقة 2: عبر Query String**

```http
GET /api/app/product?culture=ar-SA
Authorization: Bearer {{access_token}}
```

#### **الطريقة 3: عبر Cookie**

```http
GET /api/app/product
Authorization: Bearer {{access_token}}
Cookie: Abp.Culture=c=ar-SA|uic=ar-SA
```

---

### **Test Scenarios:**

#### **Test 1: Error Message بالإنجليزية** 🇬🇧

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
  "categoryId": "00000000-0000-0000-0000-000000000000"
}
```

**Expected Response:**
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

#### **Test 2: Error Message بالعربية** 🇸🇦

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
  "categoryId": "00000000-0000-0000-0000-000000000000"
}
```

**Expected Response:**
```json
{
  "error": {
    "code": "Reference:CategoryNotFound",
    "message": "التصنيف غير موجود",
    "data": {
      "CategoryId": "00000000-0000-0000-0000-000000000000"
    }
  }
}
```

---

#### **Test 3: Duplicate Name Error بالإنجليزية**

**Request:**
```http
POST /api/app/product
Accept-Language: en-US

{
  "name": "Laptop Pro 15",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response:**
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

#### **Test 4: Duplicate Name Error بالعربية**

**Request:**
```http
POST /api/app/product
Accept-Language: ar-SA

{
  "name": "Laptop Pro 15",
  "price": 100,
  "stock": 10,
  "categoryId": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

**Expected Response:**
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

## 📝 5. Best Practices

### ✅ **Do:**

1. ✅ استخدم مفاتيح واضحة ومعبرة
2. ✅ نظم المفاتيح بشكل هرمي (Menu:Products, Permission:Products.Create)
3. ✅ ضع كل Error Codes في ملفات Localization
4. ✅ استخدم Parameters للنصوص الديناميكية
5. ✅ اختبر كل اللغات
6. ✅ استخدم RTL للعربية في الـ UI

### ❌ **Don't:**

1. ❌ لا تكتب النصوص hard-coded في الكود
2. ❌ لا تنسى ترجمة Error messages
3. ❌ لا تستخدم Google Translate للترجمة الاحترافية
4. ❌ لا تنسى Permissions localization
5. ❌ لا تخلط بين اللغات في نفس الملف

---

## 🔄 Localization Workflow

```
User Request
  ↓
Accept-Language Header (ar-SA / en-US)
  ↓
ABP sets CurrentCulture
  ↓
Application Service throws BusinessException
  ↓
ABP Exception Filter
  ↓
Look up error code in Localization files
  ↓
Return localized message
  ↓
Response with translated error
```

---

## 🌐 Supported Cultures

### **في ABP Module:**

```csharp
// ReferenceDomainSharedModule.cs
public override void ConfigureServices(ServiceConfigurationContext context)
{
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
        options.FileSets.AddEmbedded<ReferenceDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
        options.Resources
            .Add<ReferenceResource>("en")
            .AddVirtualJson("/Localization/Reference");

        options.DefaultResourceType = typeof(ReferenceResource);
    });
}
```

---

## 💡 مثال كامل: Exception مترجمة

### **الكود:**

```csharp
public async Task DeleteAsync(Guid id)
{
    var category = await _repository.GetAsync(id);

    var hasProducts = await _productRepository.AnyAsync(p => p.CategoryId == id);
    
    if (hasProducts)
    {
        throw new BusinessException(ReferenceDomainErrorCodes.CategoryHasProducts)
            .WithData("CategoryName", category.Name)
            .WithData("CategoryId", id);
    }

    await _repository.DeleteAsync(category);
}
```

### **en.json:**
```json
{
  "Reference:CategoryHasProducts": "Cannot delete category '{CategoryName}' because it has products"
}
```

### **ar.json:**
```json
{
  "Reference:CategoryHasProducts": "لا يمكن حذف التصنيف '{CategoryName}' لأنه يحتوي على منتجات"
}
```

### **Response بالإنجليزية:**
```json
{
  "error": {
    "code": "Reference:CategoryHasProducts",
    "message": "Cannot delete category 'Electronics' because it has products"
  }
}
```

### **Response بالعربية:**
```json
{
  "error": {
    "code": "Reference:CategoryHasProducts",
    "message": "لا يمكن حذف التصنيف 'الإلكترونيات' لأنه يحتوي على منتجات"
  }
}
```

---

## 🚀 الخلاصة

**Localization في ABP:**
- ✅ **Multi-language** - دعم لغات متعددة
- ✅ **Centralized** - كل النصوص في مكان واحد
- ✅ **Automatic** - ABP يترجم تلقائياً
- ✅ **Flexible** - سهل إضافة لغات جديدة
- ✅ **Professional** - تجربة مستخدم ممتازة

**Structure:**
```
Domain.Shared/
  └── Localization/
      └── Reference/
          ├── en.json (English)
          ├── ar.json (Arabic)
          └── ReferenceResource.cs
```

**Usage:**
1. **Automatic** - Exception messages
2. **Manual** - `IStringLocalizer`
3. **Permissions** - `L("Permission:Name")`

**Testing:**
- Use `Accept-Language` header
- Test all error scenarios
- Verify translations

---

**Localization = Global Application! 🌍**
