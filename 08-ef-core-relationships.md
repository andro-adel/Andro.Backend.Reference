# 🔗 EF Core Relationships في ABP.IO

## 📖 المفاهيم الأساسية

### ما هي الـ Relationships؟
**Relationships** هي العلاقات بين الـ Entities في قاعدة البيانات.

**مثال من الحياة الواقعية:**
- 📁 Category (فئة/تصنيف) مثل "Electronics", "Clothing"
- 📦 Product (منتج) ينتمي لـ Category واحدة
- Category واحدة تحتوي على عدة Products

---

## 🎯 أنواع Relationships

### 1. One-to-Many (واحد لمتعدد)
```
Category (1) ←→ (Many) Products

مثال:
- Electronics Category → [Laptop, Phone, TV]
- Clothing Category → [Shirt, Pants, Shoes]
```

**في الكود:**
```csharp
public class Category : AggregateRoot<Guid>
{
    public string Name { get; set; }
    
    // Navigation Property
    public ICollection<Product> Products { get; set; }
}

public class Product : AggregateRoot<Guid>
{
    public string Name { get; set; }
    
    // Foreign Key
    public Guid CategoryId { get; set; }
    
    // Navigation Property
    public Category Category { get; set; }
}
```

**في Database:**
```sql
CREATE TABLE Categories (
    Id uniqueidentifier PRIMARY KEY,
    Name nvarchar(max)
)

CREATE TABLE Products (
    Id uniqueidentifier PRIMARY KEY,
    Name nvarchar(max),
    CategoryId uniqueidentifier,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
)
```

---

### 2. One-to-One (واحد لواحد)
```
User (1) ←→ (1) UserProfile

مثال:
- كل User له UserProfile واحد فقط
- كل UserProfile تنتمي لـ User واحد فقط
```

**في الكود:**
```csharp
public class User : AggregateRoot<Guid>
{
    public string Email { get; set; }
    public UserProfile Profile { get; set; }
}

public class UserProfile : Entity<Guid>
{
    public string Bio { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
```

---

### 3. Many-to-Many (متعدد لمتعدد)
```
Student (Many) ←→ (Many) Course

مثال:
- Student واحد يسجل في عدة Courses
- Course واحد يحتوي على عدة Students
```

**في الكود (EF Core 5+):**
```csharp
public class Student : AggregateRoot<Guid>
{
    public string Name { get; set; }
    public ICollection<Course> Courses { get; set; }
}

public class Course : AggregateRoot<Guid>
{
    public string Title { get; set; }
    public ICollection<Student> Students { get; set; }
}
```

**Join Table (تلقائي):**
```sql
CREATE TABLE StudentCourse (
    StudentsId uniqueidentifier,
    CoursesId uniqueidentifier,
    PRIMARY KEY (StudentsId, CoursesId)
)
```

---

## 🛠️ Navigation Properties

### ما هي Navigation Properties؟
Properties تسمح لك بالتنقل بين الـ Entities المرتبطة بدون كتابة SQL يدوياً.

### أنواع Navigation Properties:

#### 1. Reference Navigation (Single)
```csharp
public Category Category { get; set; }  // Product → Category
```

#### 2. Collection Navigation (Multiple)
```csharp
public ICollection<Product> Products { get; set; }  // Category → Products
```

---

## 🔑 Foreign Keys

### تعريف Foreign Key بوضوح:
```csharp
public class Product : AggregateRoot<Guid>
{
    // Foreign Key Property
    public Guid CategoryId { get; set; }
    
    // Navigation Property
    public Category Category { get; set; }
}
```

### Foreign Key Conventions:
EF Core يكتشف Foreign Key تلقائياً إذا:
- اسمها `{NavigationPropertyName}Id`
- مثال: `CategoryId` لـ `Category`

---

## ⚙️ Configuring Relationships في OnModelCreating

### One-to-Many Configuration:
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.Entity<Product>(b =>
    {
        // تعريف العلاقة
        b.HasOne(p => p.Category)         // Product has one Category
         .WithMany(c => c.Products)       // Category has many Products
         .HasForeignKey(p => p.CategoryId) // Foreign Key
         .IsRequired();                   // مطلوب (NOT NULL)
    });
}
```

### Delete Behavior:
```csharp
.OnDelete(DeleteBehavior.Cascade)    // حذف Products عند حذف Category
.OnDelete(DeleteBehavior.Restrict)   // منع حذف Category إذا كان عندها Products
.OnDelete(DeleteBehavior.SetNull)    // جعل CategoryId = NULL
```

---

## 📊 Loading Related Data

### 1. Eager Loading (Include)
```csharp
// تحميل Products مع Categories في استعلام واحد
var products = await _repository
    .WithDetails(p => p.Category)  // ABP method
    .ToListAsync();

// أو EF Core مباشر
var products = await dbContext.Products
    .Include(p => p.Category)
    .ToListAsync();
```

### 2. Explicit Loading
```csharp
var product = await _repository.GetAsync(id);

// تحميل Category بعدين
await dbContext.Entry(product)
    .Reference(p => p.Category)
    .LoadAsync();
```

### 3. Lazy Loading (غير مفضل في ABP)
```csharp
// يحتاج proxies - لا يُنصح به في ABP
public virtual Category Category { get; set; }
```

---

## 🎯 Best Practices في ABP

### 1. استخدم AggregateRoot للـ Parent
```csharp
public class Category : FullAuditedAggregateRoot<Guid>  // ✅
{
    public ICollection<Product> Products { get; set; }
}
```

### 2. Foreign Keys دائماً required إلا لو optional
```csharp
public Guid CategoryId { get; set; }  // Required
public Guid? CategoryId { get; set; } // Optional (nullable)
```

### 3. استخدم WithDetails في Repository
```csharp
// ✅ ABP Way
var products = await _productRepository
    .WithDetails(p => p.Category)
    .ToListAsync();

// ❌ لا تستخدم Include مباشرة على IRepository
```

### 4. Navigation Properties في DTOs
```csharp
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    // Include related data
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }  // من Category.Name
}
```

---

## 💡 مثال عملي كامل: Product & Category

### 1. Define Entities

#### Category Entity:
```csharp
public class Category : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    // Navigation Property
    public ICollection<Product> Products { get; set; }
    
    protected Category() { }
    
    public Category(Guid id, string name, string description = null)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), 128);
        Description = description;
        Products = new List<Product>();
    }
}
```

#### Product Entity (Updated):
```csharp
public class Product : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; }
    
    // Foreign Key
    public Guid CategoryId { get; set; }
    
    // Navigation Property
    public Category Category { get; set; }
    
    protected Product() { }
    
    public Product(
        Guid id, 
        string name, 
        decimal price, 
        int stock,
        Guid categoryId,
        string description = null)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), 128);
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        Description = description;
    }
}
```

---

### 2. Configure in DbContext

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    builder.Entity<Category>(b =>
    {
        b.ToTable("Categories");
        
        b.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);
            
        b.Property(x => x.Description)
            .HasMaxLength(512);
            
        b.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    });
    
    builder.Entity<Product>(b =>
    {
        // ... existing Product config
        
        b.HasIndex(p => p.CategoryId);  // Index على Foreign Key
    });
}
```

---

### 3. Create Migration

```powershell
cd src/Andro.Backend.Reference.EntityFrameworkCore

# إضافة Migration
dotnet ef migrations add "Added_Category_Entity"

# تطبيق Migration
dotnet ef database update
```

---

### 4. DTOs with Relationships

```csharp
// CategoryDto.cs
public class CategoryDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProductsCount { get; set; }  // عدد المنتجات
}

// ProductDto.cs (Updated)
public class ProductDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; }
    
    // Category Info
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
}
```

---

### 5. Application Service with Include

```csharp
public class ProductAppService : ApplicationService
{
    private readonly IRepository<Product, Guid> _productRepository;
    
    public async Task<ProductDto> GetAsync(Guid id)
    {
        // Include Category
        var product = await _productRepository
            .WithDetails(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
            
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name  // من العلاقة
        };
    }
}
```

---

## ⚠️ Common Pitfalls

### 1. Circular Reference في JSON
```csharp
// ❌ مشكلة
public class Category
{
    public ICollection<Product> Products { get; set; }
}

public class Product
{
    public Category Category { get; set; }
}

// Category → Products → Category → Products → ...
```

**الحل:**
- لا ترجع Navigation Properties في DTOs
- استخدم `[JsonIgnore]` أو اجعل DTOs بسيطة

### 2. N+1 Query Problem
```csharp
// ❌ سيء - N+1 queries
var products = await _repository.GetListAsync();
foreach (var product in products)
{
    // هنا query لكل product!
    var category = product.Category.Name;
}

// ✅ جيد - 1 query
var products = await _repository
    .WithDetails(p => p.Category)
    .ToListAsync();
```

### 3. Missing Foreign Key Index
```csharp
// ✅ دائماً اعمل Index على Foreign Keys
b.HasIndex(p => p.CategoryId);
```

---

## 📚 ملخص

### One-to-Many Relationship:
1. ✅ Parent Entity → `ICollection<Child>`
2. ✅ Child Entity → `ParentId` + `Parent`
3. ✅ Configure في `OnModelCreating`
4. ✅ استخدم `WithDetails()` للـ Include
5. ✅ DTOs بدون Navigation Properties

### الفوائد:
- 🎯 Data Integrity (Foreign Key Constraints)
- 🚀 Query Optimization (Indexes)
- 💪 Type Safety
- 🔄 Automatic Tracking

---

## 📖 المصادر

- [EF Core Relationships](https://docs.microsoft.com/ef/core/modeling/relationships)
- [ABP Repository](https://docs.abp.io/en/abp/latest/Repositories)
- [Entity Framework Core Best Practices](https://docs.microsoft.com/ef/core/performance/)
