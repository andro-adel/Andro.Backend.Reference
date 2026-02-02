# 🎯 Best Practices في ABP.io - دليل شامل

## 📋 نظرة عامة

Best Practices هي مجموعة من القواعد والأنماط المُثبتة التي تساعد في كتابة كود نظيف، قابل للصيانة، وقابل للتوسع. في هذا الدليل سنغطي SOLID Principles، Design Patterns، و Specification Pattern في سياق ABP.io.

---

## 🏛️ SOLID Principles

### **1️⃣ Single Responsibility Principle (SRP)**

**المبدأ:** كل Class يجب أن يكون له مسؤولية واحدة فقط

#### **❌ Bad Example:**

```csharp
public class ProductAppService
{
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // 1. Validation
        if (string.IsNullOrWhiteSpace(input.Name))
            throw new Exception("Name required");
        
        // 2. Business logic
        var product = new Product(...);
        
        // 3. Database
        await _repository.InsertAsync(product);
        
        // 4. Sending email
        await _emailSender.SendAsync(...);
        
        // 5. Logging
        _logger.LogInformation("Product created");
        
        // 6. Caching
        await _cache.SetAsync(...);
        
        // TOO MANY RESPONSIBILITIES! ❌
    }
}
```

#### **✅ Good Example (في ABP):**

```csharp
// 1. Validation - مسؤولية منفصلة
public class CreateProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
}

// 2. Domain Entity - Business Logic فقط
public class Product : AggregateRoot<Guid>
{
    public void DecreaseStock(int quantity)
    {
        if (Stock < quantity)
            throw new InsufficientStockException();
        
        Stock -= quantity;
        // Business rule enforced
    }
}

// 3. Application Service - Orchestration فقط
public class ProductAppService : ApplicationService
{
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        var product = new Product(...);
        await _repository.InsertAsync(product);
        
        // Event handles email, logging, caching
        await _localEventBus.PublishAsync(
            new ProductCreatedEvent(product.Id, product.Name)
        );
        
        return ObjectMapper.Map<Product, ProductDto>(product);
    }
}

// 4. Event Handler - Email فقط
public class ProductCreatedEmailHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        await _emailSender.SendAsync(...);
    }
}

// 5. Event Handler - Logging فقط
public class ProductCreatedLogHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        _logger.LogInformation("Product created: {Name}", eventData.Name);
    }
}
```

**✅ كل class له مسؤولية واحدة واضحة!**

---

### **2️⃣ Open/Closed Principle (OCP)**

**المبدأ:** Classes يجب أن تكون مفتوحة للتوسع، مغلقة للتعديل

#### **❌ Bad Example:**

```csharp
public class PriceCalculator
{
    public decimal Calculate(Product product, string customerType)
    {
        if (customerType == "Regular")
            return product.Price;
        else if (customerType == "Gold")
            return product.Price * 0.9m; // 10% discount
        else if (customerType == "Platinum")
            return product.Price * 0.8m; // 20% discount
        
        // كل مرة نضيف نوع جديد، نعدل الـ method! ❌
        return product.Price;
    }
}
```

#### **✅ Good Example:**

```csharp
// Base strategy
public interface IPricingStrategy
{
    decimal Calculate(Product product);
}

// Regular customer
public class RegularPricingStrategy : IPricingStrategy
{
    public decimal Calculate(Product product)
    {
        return product.Price;
    }
}

// Gold customer
public class GoldPricingStrategy : IPricingStrategy
{
    public decimal Calculate(Product product)
    {
        return product.Price * 0.9m; // 10% discount
    }
}

// Platinum customer
public class PlatinumPricingStrategy : IPricingStrategy
{
    public decimal Calculate(Product product)
    {
        return product.Price * 0.8m; // 20% discount
    }
}

// Calculator - no modification needed for new types
public class PriceCalculator
{
    private readonly IPricingStrategy _strategy;
    
    public PriceCalculator(IPricingStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public decimal Calculate(Product product)
    {
        return _strategy.Calculate(product);
    }
}

// يمكن إضافة أنواع جديدة بدون تعديل الـ Calculator! ✅
```

---

### **3️⃣ Liskov Substitution Principle (LSP)**

**المبدأ:** Derived classes يجب أن تكون قابلة للاستبدال بـ Base classes بدون مشاكل

#### **❌ Bad Example:**

```csharp
public class Product
{
    public virtual void DecreaseStock(int quantity)
    {
        Stock -= quantity;
    }
}

public class DigitalProduct : Product
{
    public override void DecreaseStock(int quantity)
    {
        // Digital products have unlimited stock
        throw new NotSupportedException("Digital products don't have stock!");
        // ❌ يخالف السلوك المتوقع من الـ base class
    }
}

// استخدام
void ProcessOrder(Product product)
{
    product.DecreaseStock(1); // سيفشل مع DigitalProduct! ❌
}
```

#### **✅ Good Example:**

```csharp
public abstract class Product : AggregateRoot<Guid>
{
    public string Name { get; protected set; }
    public decimal Price { get; protected set; }
    
    public abstract bool HasInventory();
    public abstract void ProcessSale(int quantity);
}

public class PhysicalProduct : Product
{
    public int Stock { get; private set; }
    
    public override bool HasInventory() => true;
    
    public override void ProcessSale(int quantity)
    {
        if (Stock < quantity)
            throw new InsufficientStockException();
        
        Stock -= quantity;
    }
}

public class DigitalProduct : Product
{
    public override bool HasInventory() => false;
    
    public override void ProcessSale(int quantity)
    {
        // Digital products don't need stock management
        // Nothing to do - unlimited availability
    }
}

// استخدام
void ProcessOrder(Product product, int quantity)
{
    product.ProcessSale(quantity); // يعمل مع كل الأنواع! ✅
}
```

---

### **4️⃣ Interface Segregation Principle (ISP)**

**المبدأ:** لا تجبر clients على الاعتماد على interfaces لا يستخدمونها

#### **❌ Bad Example:**

```csharp
public interface IProductService
{
    Task<ProductDto> GetAsync(Guid id);
    Task<List<ProductDto>> GetListAsync();
    Task<ProductDto> CreateAsync(CreateProductDto input);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input);
    Task DeleteAsync(Guid id);
    Task ExportToExcelAsync();
    Task ImportFromExcelAsync(Stream file);
    Task SendEmailReportAsync();
    Task CalculateStatisticsAsync();
    // ❌ واجهة ضخمة - معظم المستخدمين لا يحتاجون كل الـ methods
}
```

#### **✅ Good Example:**

```csharp
// Basic CRUD
public interface IProductService
{
    Task<ProductDto> GetAsync(Guid id);
    Task<List<ProductDto>> GetListAsync();
    Task<ProductDto> CreateAsync(CreateProductDto input);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input);
    Task DeleteAsync(Guid id);
}

// Export functionality
public interface IProductExportService
{
    Task ExportToExcelAsync();
    Task ExportToPdfAsync();
}

// Import functionality
public interface IProductImportService
{
    Task ImportFromExcelAsync(Stream file);
    Task ImportFromCsvAsync(Stream file);
}

// Reporting functionality
public interface IProductReportService
{
    Task SendEmailReportAsync();
    Task CalculateStatisticsAsync();
}

// ✅ كل client يعتمد فقط على ما يحتاجه
```

---

### **5️⃣ Dependency Inversion Principle (DIP)**

**المبدأ:** High-level modules لا يجب أن تعتمد على Low-level modules. كلاهما يجب أن يعتمد على Abstractions

#### **❌ Bad Example:**

```csharp
// Low-level module
public class SqlProductRepository
{
    public async Task<Product> GetAsync(Guid id)
    {
        // SQL-specific implementation
    }
}

// High-level module depends on concrete class ❌
public class ProductAppService
{
    private readonly SqlProductRepository _repository;
    
    public ProductAppService(SqlProductRepository repository)
    {
        _repository = repository; // ❌ مرتبط بـ SQL فقط
    }
}
```

#### **✅ Good Example (ABP):**

```csharp
// Abstraction
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<Product> GetWithDetailsAsync(Guid id);
    Task<List<Product>> GetLowStockProductsAsync(int threshold);
}

// Low-level module implements abstraction
public class EfCoreProductRepository 
    : EfCoreRepository<Product, Guid>, 
      IProductRepository
{
    public async Task<Product> GetWithDetailsAsync(Guid id)
    {
        // EF Core implementation
    }
}

// High-level module depends on abstraction ✅
public class ProductAppService : ApplicationService
{
    private readonly IProductRepository _repository;
    
    public ProductAppService(IProductRepository repository)
    {
        _repository = repository; // ✅ يمكن استبداله بأي implementation
    }
}

// يمكن استبدال EF Core بـ MongoDB أو Dapper بدون تعديل AppService! ✅
```

---

## 🎨 Design Patterns في ABP

### **1. Repository Pattern**

**ABP يطبقه تلقائياً!**

```csharp
// Generic repository - ABP يوفره
IRepository<Product, Guid> _productRepository;

// Custom repository - يمكن إضافة methods خاصة
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<List<Product>> GetLowStockProductsAsync(int threshold);
    Task<Product> GetByNameAsync(string name);
}
```

**✅ Benefits:**
- فصل Data Access عن Business Logic
- سهولة Testing (Mock repositories)
- تغيير Database بدون تعديل Business Logic

---

### **2. Unit of Work Pattern**

**ABP يطبقه تلقائياً!**

```csharp
[UnitOfWork]
public async Task<ProductDto> CreateAsync(CreateProductDto input)
{
    // كل الـ operations في transaction واحدة
    var product = new Product(...);
    await _productRepository.InsertAsync(product);
    
    var category = await _categoryRepository.GetAsync(input.CategoryId);
    category.ProductCount++;
    
    // SaveChanges تلقائياً عند نهاية الـ method
    // إذا حدث exception، سيتم Rollback تلقائياً
}
```

**✅ Benefits:**
- Transaction Management تلقائي
- Consistency ضمانة
- سهولة Rollback

---

### **3. Dependency Injection Pattern**

**ABP يعتمد عليه بالكامل!**

```csharp
public class ProductAppService : ApplicationService
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly ILocalEventBus _eventBus;
    private readonly ILogger<ProductAppService> _logger;
    
    // Constructor Injection ✅
    public ProductAppService(
        IRepository<Product, Guid> productRepository,
        IRepository<Category, Guid> categoryRepository,
        ILocalEventBus eventBus,
        ILogger<ProductAppService> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _eventBus = eventBus;
        _logger = logger;
    }
}

// Registration تلقائي في ABP:
// - ApplicationService -> ITransientDependency
// - DomainService -> ITransientDependency
// - Repository -> ITransientDependency
```

---

### **4. Factory Pattern**

**مفيد لإنشاء Objects معقدة:**

```csharp
public interface IProductFactory
{
    Product CreatePhysicalProduct(string name, decimal price, int stock);
    Product CreateDigitalProduct(string name, decimal price, string downloadUrl);
}

public class ProductFactory : IProductFactory, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator;
    
    public ProductFactory(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }
    
    public Product CreatePhysicalProduct(string name, decimal price, int stock)
    {
        return new PhysicalProduct(
            _guidGenerator.Create(),
            name,
            price,
            stock
        );
    }
    
    public Product CreateDigitalProduct(string name, decimal price, string downloadUrl)
    {
        return new DigitalProduct(
            _guidGenerator.Create(),
            name,
            price,
            downloadUrl
        );
    }
}
```

---

### **5. Strategy Pattern**

**مفيد للـ Business Rules المتعددة:**

```csharp
// Discount strategy
public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal price);
    bool IsApplicable(Customer customer);
}

public class NoDiscountStrategy : IDiscountStrategy, ITransientDependency
{
    public decimal ApplyDiscount(decimal price) => price;
    public bool IsApplicable(Customer customer) => true;
}

public class SeasonalDiscountStrategy : IDiscountStrategy, ITransientDependency
{
    public decimal ApplyDiscount(decimal price) => price * 0.9m;
    public bool IsApplicable(Customer customer) => DateTime.Now.Month == 12;
}

public class LoyaltyDiscountStrategy : IDiscountStrategy, ITransientDependency
{
    public decimal ApplyDiscount(decimal price) => price * 0.85m;
    public bool IsApplicable(Customer customer) => customer.OrderCount > 10;
}

// Usage
public class OrderService : ApplicationService
{
    private readonly IEnumerable<IDiscountStrategy> _strategies;
    
    public OrderService(IEnumerable<IDiscountStrategy> strategies)
    {
        _strategies = strategies;
    }
    
    public decimal CalculateFinalPrice(decimal price, Customer customer)
    {
        var strategy = _strategies
            .FirstOrDefault(s => s.IsApplicable(customer))
            ?? _strategies.OfType<NoDiscountStrategy>().First();
        
        return strategy.ApplyDiscount(price);
    }
}
```

---

## 🔍 Specification Pattern

### **ما هو Specification Pattern؟**

نمط يستخدم لـ:
- ✅ **Encapsulate business rules** في classes منفصلة
- ✅ **Reuse queries** عبر التطبيق
- ✅ **Combine multiple conditions** بطريقة نظيفة
- ✅ **Make code readable** - self-documenting

---

### **الحل بدون Specification Pattern:**

```csharp
// في ProductAppService
public async Task<List<ProductDto>> GetLowStockProductsAsync()
{
    var products = await _repository
        .Where(p => p.Stock < 10 && p.IsActive)
        .ToListAsync();
    
    return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
}

public async Task<List<ProductDto>> GetExpensiveProductsAsync()
{
    var products = await _repository
        .Where(p => p.Price > 1000 && p.IsActive)
        .ToListAsync();
    
    return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
}

// ❌ Queries متكررة
// ❌ Business rules مكررة (IsActive)
// ❌ صعب إعادة الاستخدام
```

---

### **الحل مع Specification Pattern:**

#### **Step 1: إنشاء Specification Base Class**

```csharp
// Domain/Products/Specifications/ProductSpecification.cs
using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;

namespace Andro.Backend.Reference.Products.Specifications;

public abstract class ProductSpecification : Specification<Product>
{
    public override abstract Expression<Func<Product, bool>> ToExpression();
}
```

---

#### **Step 2: إنشاء Concrete Specifications**

```csharp
// Low Stock Specification
public class LowStockProductSpecification : ProductSpecification
{
    private readonly int _threshold;
    
    public LowStockProductSpecification(int threshold = 10)
    {
        _threshold = threshold;
    }
    
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.Stock < _threshold;
    }
}

// Active Product Specification
public class ActiveProductSpecification : ProductSpecification
{
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.IsActive;
    }
}

// Expensive Product Specification
public class ExpensiveProductSpecification : ProductSpecification
{
    private readonly decimal _minPrice;
    
    public ExpensiveProductSpecification(decimal minPrice = 1000)
    {
        _minPrice = minPrice;
    }
    
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.Price >= _minPrice;
    }
}

// In Price Range Specification
public class ProductInPriceRangeSpecification : ProductSpecification
{
    private readonly decimal _minPrice;
    private readonly decimal _maxPrice;
    
    public ProductInPriceRangeSpecification(decimal minPrice, decimal maxPrice)
    {
        _minPrice = minPrice;
        _maxPrice = maxPrice;
    }
    
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.Price >= _minPrice && p.Price <= _maxPrice;
    }
}

// By Category Specification
public class ProductByCategorySpecification : ProductSpecification
{
    private readonly Guid _categoryId;
    
    public ProductByCategorySpecification(Guid categoryId)
    {
        _categoryId = categoryId;
    }
    
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.CategoryId == _categoryId;
    }
}
```

---

#### **Step 3: استخدام Specifications**

```csharp
public class ProductAppService : ApplicationService
{
    private readonly IRepository<Product, Guid> _repository;
    
    // Get low stock active products
    public async Task<List<ProductDto>> GetLowStockProductsAsync()
    {
        var spec = new LowStockProductSpecification()
            .And(new ActiveProductSpecification());
        
        var products = await _repository
            .Where(spec.ToExpression())
            .ToListAsync();
        
        return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
    }
    
    // Get expensive active products in category
    public async Task<List<ProductDto>> GetExpensiveProductsInCategoryAsync(Guid categoryId)
    {
        var spec = new ExpensiveProductSpecification(minPrice: 500)
            .And(new ActiveProductSpecification())
            .And(new ProductByCategorySpecification(categoryId));
        
        var products = await _repository
            .Where(spec.ToExpression())
            .ToListAsync();
        
        return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
    }
    
    // Get products in price range
    public async Task<List<ProductDto>> GetProductsInRangeAsync(
        decimal minPrice, 
        decimal maxPrice)
    {
        var spec = new ProductInPriceRangeSpecification(minPrice, maxPrice)
            .And(new ActiveProductSpecification());
        
        var products = await _repository
            .Where(spec.ToExpression())
            .ToListAsync();
        
        return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
    }
}
```

---

### **✅ Benefits of Specification Pattern:**

1. **Reusability** - استخدم نفس الـ specification في أماكن متعددة
2. **Composability** - ادمج specifications بـ `And`, `Or`, `Not`
3. **Testability** - اختبر كل specification منفصلة
4. **Readability** - self-documenting code
5. **Maintainability** - Business rules في مكان واحد

---

## 📝 Coding Best Practices

### **1. Use Meaningful Names**

```csharp
// ❌ Bad
var p = await _repo.GetAsync(id);
var l = p.Where(x => x.S < 10).ToList();

// ✅ Good
var product = await _productRepository.GetAsync(productId);
var lowStockProducts = products.Where(p => p.Stock < 10).ToList();
```

---

### **2. Keep Methods Small**

```csharp
// ❌ Bad - method طويل جداً
public async Task<ProductDto> CreateAsync(CreateProductDto input)
{
    // 50+ lines of code
    // validation
    // business logic
    // database
    // events
    // logging
    // etc...
}

// ✅ Good - methods صغيرة ومركزة
public async Task<ProductDto> CreateAsync(CreateProductDto input)
{
    await ValidateCategoryAsync(input.CategoryId);
    
    var product = await CreateProductEntityAsync(input);
    
    await _repository.InsertAsync(product);
    
    await PublishProductCreatedEventAsync(product);
    
    return MapToDto(product);
}

private async Task ValidateCategoryAsync(Guid categoryId) { }
private async Task<Product> CreateProductEntityAsync(CreateProductDto input) { }
private async Task PublishProductCreatedEventAsync(Product product) { }
private ProductDto MapToDto(Product product) { }
```

---

### **3. Avoid Magic Numbers**

```csharp
// ❌ Bad
if (product.Stock < 10)
{
    // ما معنى 10؟
}

// ✅ Good
private const int LowStockThreshold = 10;

if (product.Stock < LowStockThreshold)
{
    // واضح!
}
```

---

### **4. Use Guard Clauses**

```csharp
// ❌ Bad - nested if
public void ProcessOrder(Order order)
{
    if (order != null)
    {
        if (order.Items.Count > 0)
        {
            if (order.Customer != null)
            {
                // actual logic
            }
        }
    }
}

// ✅ Good - early return
public void ProcessOrder(Order order)
{
    if (order == null) return;
    if (order.Items.Count == 0) return;
    if (order.Customer == null) return;
    
    // actual logic - flat structure
}
```

---

### **5. Use var Appropriately**

```csharp
// ✅ Good - type واضح
var product = new Product(...);
var products = await _repository.GetListAsync();

// ❌ Bad - type غير واضح
var result = Calculate();
var data = Process();

// ✅ Good - explicit type
decimal totalPrice = Calculate();
List<ProductDto> processedData = Process();
```

---

## 🧪 Testing Best Practices

### **1. Follow AAA Pattern**

```csharp
[Fact]
public async Task Should_Create_Product_Successfully()
{
    // Arrange - الإعداد
    var input = new CreateProductDto
    {
        Name = "Test Product",
        Price = 99.99m
    };
    
    // Act - التنفيذ
    var result = await _productAppService.CreateAsync(input);
    
    // Assert - التحقق
    result.ShouldNotBeNull();
    result.Name.ShouldBe("Test Product");
}
```

---

### **2. One Assert Per Test (Usually)**

```csharp
// ❌ Bad - too many asserts
[Fact]
public async Task Test_Everything()
{
    var product = await CreateProduct();
    product.Name.ShouldBe("Test");
    product.Price.ShouldBe(99);
    product.Stock.ShouldBe(10);
    
    await UpdateProduct(product.Id);
    // more asserts...
    
    await DeleteProduct(product.Id);
    // more asserts...
}

// ✅ Good - focused tests
[Fact]
public async Task Should_Create_Product_With_Correct_Name() { }

[Fact]
public async Task Should_Create_Product_With_Correct_Price() { }

[Fact]
public async Task Should_Update_Product_Successfully() { }

[Fact]
public async Task Should_Delete_Product_Successfully() { }
```

---

## 🎯 الخلاصة

### **SOLID Principles:**
- ✅ **SRP** - مسؤولية واحدة لكل class
- ✅ **OCP** - مفتوح للتوسع، مغلق للتعديل
- ✅ **LSP** - قابل للاستبدال
- ✅ **ISP** - واجهات صغيرة ومركزة
- ✅ **DIP** - اعتمد على abstractions

### **Design Patterns:**
- ✅ **Repository** - فصل Data Access
- ✅ **Unit of Work** - Transaction Management
- ✅ **Dependency Injection** - Loose Coupling
- ✅ **Factory** - إنشاء Objects معقدة
- ✅ **Strategy** - Business Rules متعددة

### **Specification Pattern:**
- ✅ **Encapsulate** - Business rules في classes
- ✅ **Reuse** - نفس الـ queries
- ✅ **Combine** - And, Or, Not
- ✅ **Readable** - self-documenting

### **Coding Best Practices:**
- ✅ Meaningful names
- ✅ Small methods
- ✅ No magic numbers
- ✅ Guard clauses
- ✅ Appropriate var usage

---

**Best Practices = Clean Code + Maintainable + Scalable! 🎯**
