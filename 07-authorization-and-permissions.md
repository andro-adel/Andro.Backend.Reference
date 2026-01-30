# 🔐 Authorization & Permissions في ABP.IO

## 📖 المفاهيم الأساسية

### ما هو الـ Authorization؟
**Authorization (التفويض)** هو عملية تحديد ما إذا كان المستخدم له الحق في الوصول لموارد معينة أو تنفيذ عمليات محددة.

**مثال:**
- ✅ المدير يمكنه حذف المنتجات
- ❌ الموظف العادي لا يمكنه حذف المنتجات

---

## 🔑 نظام Permissions في ABP

### 1. مكونات نظام Permissions

#### أ) Permission Definition (تعريف الـ Permission)
```csharp
public static class ProductPermissions
{
    public const string GroupName = "ProductManagement";
    
    public const string Products = GroupName + ".Products";
    public const string Create = Products + ".Create";
    public const string Edit = Products + ".Edit";
    public const string Delete = Products + ".Delete";
}
```

**الفوائد:**
- تنظيم الـ Permissions في شكل هرمي
- سهولة المراجعة والصيانة
- Naming Convention واضح

---

#### ب) Permission Definition Provider
```csharp
public class ProductPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var productGroup = context.AddGroup(ProductPermissions.GroupName);
        
        var products = productGroup.AddPermission(
            ProductPermissions.Products, 
            L("Permission:Products")
        );
        
        products.AddChild(ProductPermissions.Create, L("Permission:Create"));
        products.AddChild(ProductPermissions.Edit, L("Permission:Edit"));
        products.AddChild(ProductPermissions.Delete, L("Permission:Delete"));
    }
}
```

**شرح:**
- `AddGroup()` - مجموعة رئيسية للـ Permissions
- `AddPermission()` - إضافة Permission أساسي
- `AddChild()` - إضافة Permission فرعي (يرث من الأب)
- `L()` - Localization للنصوص

---

#### ج) Permission Checking في Application Service
```csharp
public class ProductAppService : ApplicationService
{
    [Authorize(ProductPermissions.Products)]  // يتطلب أي Product Permission
    public async Task<PagedResultDto<ProductDto>> GetListAsync(...)
    {
        // الكود هنا
    }
    
    [Authorize(ProductPermissions.Create)]  // يتطلب Create Permission فقط
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // الكود هنا
    }
}
```

**طرق Permission Checking:**

1. **Attribute-Based:**
```csharp
[Authorize(ProductPermissions.Create)]
public async Task CreateAsync() { }
```

2. **Code-Based:**
```csharp
public async Task CreateAsync()
{
    await AuthorizationService.CheckAsync(ProductPermissions.Create);
    // أو
    if (await AuthorizationService.IsGrantedAsync(ProductPermissions.Create))
    {
        // الكود
    }
}
```

---

### 2. الهرمية في Permissions

```
ProductManagement (Group)
└── Products (Parent)
    ├── Create (Child)
    ├── Edit (Child)
    └── Delete (Child)
```

**كيف تعمل الهرمية:**
- إذا أعطيت المستخدم `Products` → سيحصل على كل الـ Children تلقائياً
- إذا أعطيت المستخدم `Create` فقط → لن يحصل على `Edit` أو `Delete`

---

### 3. ربط Permissions بالـ Roles

في ABP، الـ Permissions تُعطى للـ **Roles** وليس للمستخدمين مباشرة:

```
User (admin)
    → Role (Admin)
        → Permissions (Products.Create, Products.Edit, Products.Delete)
```

**مثال عملي:**
1. إنشاء Role اسمه "ProductManager"
2. إعطاء الـ Role صلاحيات `Products.*` (كل الصلاحيات)
3. إضافة المستخدم للـ Role
4. المستخدم يصبح عنده كل صلاحيات المنتجات

---

## 🎯 Best Practices

### 1. Permission Naming Convention
```csharp
// ❌ سيء
public const string DeleteProduct = "DP";

// ✅ جيد
public const string Delete = "ProductManagement.Products.Delete";
```

### 2. Granular Permissions
```csharp
// ❌ صلاحية واحدة عامة
public const string Products = "Products";

// ✅ صلاحيات محددة لكل عملية
public const string Create = "Products.Create";
public const string Edit = "Products.Edit";
public const string Delete = "Products.Delete";
public const string ViewPrice = "Products.ViewPrice";
```

### 3. Permission Groups
```csharp
// تنظيم الـ Permissions في Groups منطقية
- ProductManagement
  - Products
  - Categories
  - Orders
- UserManagement
  - Users
  - Roles
```

---

## 🔍 Permission Checking Flow

### عند استدعاء API:

```
1. المستخدم يرسل Request مع JWT Token
   ↓
2. ABP يتحقق من الـ Token ويستخرج User ID
   ↓
3. يتحقق من الـ [Authorize] Attribute على الـ Method
   ↓
4. يجلب Roles الخاصة بالمستخدم
   ↓
5. يجلب Permissions الخاصة بكل Role
   ↓
6. يتحقق إذا كان عند المستخدم الـ Permission المطلوب
   ↓
7. ✅ نجح → تنفيذ الـ Method
   ❌ فشل → إرجاع 403 Forbidden
```

---

## 🛠️ التطبيق العملي

### مثال: Product Permissions

#### 1. تعريف الـ Permissions
```csharp
// في Application.Contracts
public static class ProductPermissions
{
    public const string GroupName = "ProductManagement";
    
    public static class Products
    {
        public const string Default = GroupName + ".Products";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
```

#### 2. Permission Definition Provider
```csharp
public class ReferencePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var productGroup = context.AddGroup(
            ProductPermissions.GroupName,
            L("Permission:ProductManagement")
        );
        
        var products = productGroup.AddPermission(
            ProductPermissions.Products.Default,
            L("Permission:Products")
        );
        
        products.AddChild(
            ProductPermissions.Products.Create,
            L("Permission:Products.Create")
        );
        
        products.AddChild(
            ProductPermissions.Products.Edit,
            L("Permission:Products.Edit")
        );
        
        products.AddChild(
            ProductPermissions.Products.Delete,
            L("Permission:Products.Delete")
        );
    }
}
```

#### 3. تطبيق Authorization
```csharp
public class ProductAppService : ApplicationService, IProductAppService
{
    [Authorize(ProductPermissions.Products.Default)]
    public async Task<PagedResultDto<ProductDto>> GetListAsync(...)
    {
        // قراءة المنتجات
    }
    
    [Authorize(ProductPermissions.Products.Create)]
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // إنشاء منتج
    }
    
    [Authorize(ProductPermissions.Products.Edit)]
    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input)
    {
        // تحديث منتج
    }
    
    [Authorize(ProductPermissions.Products.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        // حذف منتج
    }
}
```

---

## 📱 اختبار Permissions في Postman

### الحالة 1: بدون Permissions ❌
```
Request: GET /api/app/product
Token: صالح لكن بدون Product permissions

Response: 403 Forbidden
{
  "error": {
    "code": "Volo.Authorization:010001",
    "message": "Authorization failed! Given policy has not been granted."
  }
}
```

### الحالة 2: مع Permissions ✅
```
Request: GET /api/app/product
Token: صالح مع Product.Default permission

Response: 200 OK
{
  "totalCount": 5,
  "items": [...]
}
```

---

## 🎓 ملخص

### الفوائد:
✅ **أمان محكم** - التحكم الدقيق في الصلاحيات
✅ **مرونة** - سهولة إضافة/تعديل Permissions
✅ **قابلية الصيانة** - كود منظم وواضح
✅ **Multi-Tenancy Ready** - يعمل مع نظام الـ Tenants

### الخطوات:
1. تعريف Permission Constants
2. إنشاء Permission Definition Provider
3. تطبيق `[Authorize]` على Application Services
4. إعطاء Permissions للـ Roles في الـ UI
5. اختبار الـ APIs

---

## 📚 المصادر

- [ABP Authorization Documentation](https://docs.abp.io/en/abp/latest/Authorization)
- [Permission Management](https://docs.abp.io/en/abp/latest/Modules/Permission-Management)
- [Identity Module](https://docs.abp.io/en/abp/latest/Modules/Identity)
