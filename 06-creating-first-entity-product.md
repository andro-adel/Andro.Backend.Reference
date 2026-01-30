# 📦 المرحلة 2: إنشاء أول Entity - Product

---

## 📚 الشرح النظري

### ما الذي سنفعله؟

سننشئ **Product Entity** كاملة من الصفر، وهذا يشمل:

1. **Domain Layer**: تعريف الـ Entity
2. **EntityFrameworkCore Layer**: إضافة DbSet و Migration
3. **Application.Contracts Layer**: تعريف DTOs و Interface
4. **Application Layer**: تطبيق الـ Service
5. **Testing**: اختبار APIs عبر Postman

---

## 🎯 ما هي الـ Entity؟

**Entity** هي كائن له هوية فريدة (ID) وتُخزن في قاعدة البيانات.

### مثال: Product Entity

```csharp
public class Product
{
    public Guid Id { get; set; }           // المُعرف الفريد
    public string Name { get; set; }       // اسم المنتج
    public decimal Price { get; set; }     // السعر
    public int Stock { get; set; }         // الكمية المتوفرة
}
```

---

## 🏗️ Base Classes في ABP

ABP يوفر **Base Classes** جاهزة بميزات قوية:

### 1️⃣ `Entity<TKey>`
أبسط Base Class - فقط `Id`

```csharp
public class Product : Entity<Guid>
{
    // Id موجود تلقائياً من Entity<Guid>
    public string Name { get; set; }
}
```

---

### 2️⃣ `AggregateRoot<TKey>`
يضيف **Domain Events** (أحداث النطاق)

```csharp
public class Product : AggregateRoot<Guid>
{
    // Id + إمكانية إطلاق أحداث (Events)
    public string Name { get; set; }
}
```

**متى تستخدمه؟**
- عندما تكون الـ Entity هي الجذر الرئيسي لـ Aggregate
- عندما تحتاج لإطلاق Domain Events

---

### 3️⃣ `FullAuditedAggregateRoot<TKey>`
يضيف **معلومات التدقيق الكاملة** (Auditing)

```csharp
public class Product : FullAuditedAggregateRoot<Guid>
{
    // Id + Events + Auditing
    public string Name { get; set; }
}
```

**معلومات التدقيق المضافة تلقائياً:**
- `CreationTime`: تاريخ الإنشاء
- `CreatorId`: من أنشأه
- `LastModificationTime`: تاريخ آخر تعديل
- `LastModifierId`: من عدله آخر مرة
- `IsDeleted`: هل محذوف؟ (Soft Delete)
- `DeletionTime`: تاريخ الحذف
- `DeleterId`: من حذفه

✅ **هذا هو الأفضل في معظم الحالات!**

---

## 🎨 CRUD Operations

### ما هي CRUD؟

- **C**reate: إنشاء منتج جديد
- **R**ead: قراءة/عرض المنتجات
- **U**pdate: تحديث منتج
- **D**elete: حذف منتج

---

## 📋 DTOs (Data Transfer Objects)

**DTO** هو كائن لنقل البيانات بين الطبقات.

### لماذا نحتاج DTOs؟

❌ **خطأ:** إرسال Entity مباشرة للـ API

```csharp
[HttpGet]
public Product GetProduct() 
{
    return product; // ❌ يكشف كل الـ Properties حتى الحساسة!
}
```

✅ **صح:** استخدام DTO

```csharp
[HttpGet]
public ProductDto GetProduct() 
{
    return productDto; // ✅ فقط البيانات المطلوبة
}
```

---

### أنواع DTOs للـ CRUD:

#### 1️⃣ `CreateProductDto`
البيانات المطلوبة **لإنشاء** منتج جديد

```csharp
public class CreateProductDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
}
```

---

#### 2️⃣ `UpdateProductDto`
البيانات المطلوبة **لتحديث** منتج

```csharp
public class UpdateProductDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
}
```

---

#### 3️⃣ `ProductDto`
البيانات المُرجعة **عند القراءة**

```csharp
public class ProductDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreationTime { get; set; }
}
```

---

## 🔧 Application Service

**Application Service** هو الطبقة التي تحتوي على **Business Logic** وتربط بين:
- الـ **Domain** (Entities & Repositories)
- الـ **DTOs** (لنقل البيانات)
- الـ **HTTP API** (Controllers)

### مثال:

```csharp
public class ProductAppService : ApplicationService, IProductAppService
{
    private readonly IRepository<Product, Guid> _productRepository;

    public ProductAppService(IRepository<Product, Guid> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> GetAsync(Guid id)
    {
        var product = await _productRepository.GetAsync(id);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        var product = new Product
        {
            Name = input.Name,
            Price = input.Price,
            Stock = input.Stock
        };

        await _productRepository.InsertAsync(product);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }
}
```

---

## 🚀 ABP CrudAppService

ABP يوفر **Base Class** جاهز لـ CRUD بدون كتابة كود كثير!

```csharp
public class ProductAppService : 
    CrudAppService<
        Product,           // Entity
        ProductDto,        // DTO للقراءة
        Guid,              // Primary Key Type
        PagedAndSortedResultRequestDto,  // للـ Pagination
        CreateProductDto,  // DTO للإنشاء
        UpdateProductDto   // DTO للتحديث
    >, 
    IProductAppService
{
    public ProductAppService(IRepository<Product, Guid> repository) 
        : base(repository)
    {
    }
}
```

**هذا يعطيك تلقائياً:**
- ✅ `GetAsync(id)` - الحصول على منتج
- ✅ `GetListAsync()` - قائمة المنتجات (مع Pagination و Sorting)
- ✅ `CreateAsync(input)` - إنشاء منتج
- ✅ `UpdateAsync(id, input)` - تحديث منتج
- ✅ `DeleteAsync(id)` - حذف منتج

---

## 🗺️ Object Mapping (AutoMapper)

ABP يستخدم **AutoMapper** لتحويل Entities إلى DTOs تلقائياً.

### التعريف:

في `ReferenceApplicationAutoMapperProfile.cs`:

```csharp
public class ReferenceApplicationAutoMapperProfile : Profile
{
    public ReferenceApplicationAutoMapperProfile()
    {
        // Entity -> DTO
        CreateMap<Product, ProductDto>();
        
        // CreateDto -> Entity
        CreateMap<CreateProductDto, Product>();
        
        // UpdateDto -> Entity
        CreateMap<UpdateProductDto, Product>();
    }
}
```

---

## 🔄 Database Migration

بعد إنشاء Entity، نحتاج لتحديث قاعدة البيانات.

### الخطوات:

#### 1️⃣ إضافة DbSet في DbContext

```csharp
public DbSet<Product> Products { get; set; }
```

#### 2️⃣ إنشاء Migration

```powershell
cd "src/Andro.Backend.Reference.EntityFrameworkCore"
dotnet ef migrations add "Added_Product_Entity"
```

#### 3️⃣ تطبيق Migration على قاعدة البيانات

```powershell
cd "../Andro.Backend.Reference.DbMigrator"
dotnet run
```

---

## 📊 الناتج النهائي - APIs

بعد إتمام كل الخطوات، سيكون لدينا:

### 1️⃣ Get Products List
```http
GET /api/app/product
```

### 2️⃣ Get Product By Id
```http
GET /api/app/product/{id}
```

### 3️⃣ Create Product
```http
POST /api/app/product
Body: CreateProductDto
```

### 4️⃣ Update Product
```http
PUT /api/app/product/{id}
Body: UpdateProductDto
```

### 5️⃣ Delete Product
```http
DELETE /api/app/product/{id}
```

---

## 📝 ملخص الخطوات العملية

```
1. إنشاء Product.cs في Domain/Products
2. إضافة DbSet<Product> في DbContext
3. Configuration للـ Entity (EF Core)
4. إنشاء Migration وتطبيقها
5. إنشاء DTOs في Application.Contracts/Products
6. إنشاء IProductAppService Interface
7. إنشاء ProductAppService في Application/Products
8. إضافة Mapping في AutoMapperProfile
9. تشغيل المشروع
10. اختبار APIs في Postman
```

---

## 🎯 الآن نبدأ التطبيق العملي!

في الخطوات التالية، سننفذ كل هذا بالتفصيل خطوة بخطوة. 🚀
