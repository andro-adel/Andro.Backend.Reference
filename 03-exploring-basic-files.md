# 🔍 استكشاف الملفات الأساسية في المشروع

---

## 📂 Domain.Shared Layer - الطبقة المشتركة

### 1️⃣ MultiTenancyConsts.cs

**المسار:** `Domain.Shared/MultiTenancy/MultiTenancyConsts.cs`

```csharp
public static class MultiTenancyConsts
{
    public const bool IsEnabled = true;
}
```

**الشرح:**
- **Multi-Tenancy** = نظام يسمح بوجود عملاء متعددين (Tenants) في نفس التطبيق
- مثال: لو عاملين نظام محاسبة SaaS، كل شركة هتبقى Tenant منفصل
- كل Tenant ليه بياناته الخاصة المعزولة
- `IsEnabled = true` يعني الخاصية دي مفعلة
- **لو مش محتاجها:** غير القيمة لـ `false`

---

### 2️⃣ ReferenceDomainErrorCodes.cs

**المسار:** `Domain.Shared/ReferenceDomainErrorCodes.cs`

```csharp
public static class ReferenceDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */
}
```

**الشرح:**
- هنا بنحط **أكواد الأخطاء** الخاصة بالـ Business Logic
- مثال عملي:

```csharp
public static class ReferenceDomainErrorCodes
{
    public const string ProductNotFound = "Reference:00001";
    public const string ProductOutOfStock = "Reference:00002";
    public const string InsufficientBalance = "Reference:00003";
}
```

**ليه نستخدمها؟**
- بدل ما نرجع رسالة مباشرة، نرجع كود
- الكود يتترجم حسب اللغة (Localization)
- سهولة في تتبع الأخطاء

---

## 🔐 Permissions - نظام الصلاحيات

### 3️⃣ ReferencePermissions.cs

**المسار:** `Application.Contracts/Permissions/ReferencePermissions.cs`

```csharp
public static class ReferencePermissions
{
    public const string GroupName = "Reference";
    
    // مثال:
    // public const string MyPermission1 = GroupName + ".MyPermission1";
}
```

**الشرح:**
- هنا بنعرف **أسماء الصلاحيات** (Permissions)
- الصلاحيات بتتحكم في مين يقدر يعمل إيه

**مثال عملي:**

```csharp
public static class ReferencePermissions
{
    public const string GroupName = "Reference";

    public static class Products
    {
        public const string Default = GroupName + ".Products";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Orders
    {
        public const string Default = GroupName + ".Orders";
        public const string Create = Default + ".Create";
        public const string Approve = Default + ".Approve";
        public const string Cancel = Default + ".Cancel";
    }
}
```

**النتيجة:**
- `Reference.Products.Create`
- `Reference.Products.Edit`
- `Reference.Orders.Approve`

---

### 4️⃣ ReferencePermissionDefinitionProvider.cs

**المسار:** `Application.Contracts/Permissions/ReferencePermissionDefinitionProvider.cs`

**الدور:**
- بيسجل الصلاحيات في نظام ABP
- بيحدد الـ Localized Names للصلاحيات

**مثال عملي:**

```csharp
public class ReferencePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ReferencePermissions.GroupName);

        // تعريف صلاحيات المنتجات
        var productsPermission = myGroup.AddPermission(
            ReferencePermissions.Products.Default, 
            L("Permission:Products")
        );
        
        productsPermission.AddChild(
            ReferencePermissions.Products.Create, 
            L("Permission:Products.Create")
        );
        
        productsPermission.AddChild(
            ReferencePermissions.Products.Edit, 
            L("Permission:Products.Edit")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ReferenceResource>(name);
    }
}
```

**الشرح:**
- `AddGroup` = إنشاء مجموعة صلاحيات
- `AddPermission` = إضافة صلاحية رئيسية
- `AddChild` = إضافة صلاحية فرعية
- `L("...")` = الاسم المترجم للصلاحية

---

## 📊 كيف يعمل نظام الصلاحيات؟

### الخطوات:

```
1. تعريف الصلاحية في ReferencePermissions.cs
   ↓
2. تسجيلها في ReferencePermissionDefinitionProvider.cs
   ↓
3. ربطها بـ Role (Admin, User, etc.)
   ↓
4. استخدامها في Application Service
```

### مثال استخدام في Application Service:

```csharp
public class ProductAppService : ApplicationService
{
    [Authorize(ReferencePermissions.Products.Create)]
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // هنا الكود لإنشاء منتج
        // الـ Method دي مش هتشتغل إلا لو المستخدم عنده صلاحية Create
    }
    
    [Authorize(ReferencePermissions.Products.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        // هنا كود حذف المنتج
        // فقط من له صلاحية Delete
    }
}
```

---

## 🌍 Localization - الترجمة

### المسار: `Domain.Shared/Localization/Reference/`

ABP بيدعم الترجمة لأكتر من لغة. الملفات الموجودة:
- `en.json` - الإنجليزية
- `ar.json` - العربية (ممكن نضيفها)
- `tr.json` - التركية
- وهكذا...

**مثال ملف en.json:**

```json
{
  "Culture": "en",
  "Texts": {
    "Menu:Home": "Home",
    "Welcome": "Welcome to Reference Application",
    "Permission:Products": "Products Management",
    "Permission:Products.Create": "Create Product",
    "Permission:Products.Edit": "Edit Product",
    "Permission:Products.Delete": "Delete Product"
  }
}
```

---

## ✅ الملخص

| الملف | الدور | متى نستخدمه |
|-------|-------|-------------|
| **MultiTenancyConsts** | تفعيل/تعطيل Multi-Tenancy | لو محتاجين عملاء متعددين |
| **ErrorCodes** | أكواد الأخطاء | عند رفع Business Exceptions |
| **Permissions** | أسماء الصلاحيات | تعريف صلاحيات جديدة |
| **PermissionDefinitionProvider** | تسجيل الصلاحيات | تفعيل الصلاحيات في النظام |
| **Localization** | الترجمة | دعم لغات متعددة |

---

## 🎯 الخطوة التالية

الآن فهمنا البنية النظرية والملفات الأساسية. 

**جاهزين للخطوة العملية الأولى:**
- تجهيز قاعدة البيانات
- تشغيل المشروع لأول مرة
- اختبار الـ APIs الجاهزة

هل نبدأ؟ 🚀
