# 🧪 Testing في ABP.io - دليل شامل

## 📋 نظرة عامة

Testing في ABP.io سهل جدًا لأن الـ Framework يوفر كل الأدوات اللازمة للاختبار. ABP يستخدم **xUnit** كـ testing framework ويوفر base classes لتسهيل الاختبار.

---

## 🎯 أهمية Testing

### ✅ **الفوائد:**

1. **Quality Assurance** - ضمان جودة الكود
2. **Regression Prevention** - منع كسر الكود عند التعديل
3. **Documentation** - Tests توثق كيفية استخدام الكود
4. **Refactoring Confidence** - ثقة عند إعادة هيكلة الكود
5. **Early Bug Detection** - اكتشاف الأخطاء مبكرًا
6. **Design Improvement** - تحسين تصميم الكود

### ⚠️ **بدون Tests:**

- ❌ خوف من تعديل الكود
- ❌ Bugs تظهر في Production
- ❌ صعوبة Refactoring
- ❌ عدم الثقة في الكود

---

## 📚 أنواع Tests في ABP

### **1️⃣ Unit Tests**

**الهدف:** اختبار وحدة صغيرة من الكود (Method, Class) بمعزل عن الباقي

**Characteristics:**
- ✅ سريعة جدًا
- ✅ لا تحتاج Database
- ✅ تستخدم Mocking
- ✅ تختبر Logic فقط

**Examples:**
- اختبار Domain validation
- اختبار Business logic
- اختبار Event handlers

---

### **2️⃣ Integration Tests**

**الهدف:** اختبار تكامل مكونات متعددة مع بعضها

**Characteristics:**
- ⚡ أبطأ من Unit Tests
- 🗄️ تستخدم Database حقيقية (In-Memory)
- 🔗 تختبر التكامل الكامل
- 📦 تشمل Repository, Services, etc.

**Examples:**
- اختبار Application Services مع Database
- اختبار CRUD operations
- اختبار Permissions

---

## 🏗️ بنية Test Projects في ABP

عند إنشاء ABP project، تحصل على test projects جاهزة:

```
Andro.Backend.Reference.sln
├── src/
│   ├── Andro.Backend.Reference.Domain/
│   ├── Andro.Backend.Reference.Application/
│   └── ...
└── test/
    ├── Andro.Backend.Reference.Domain.Tests/        ✅ Domain Unit Tests
    ├── Andro.Backend.Reference.Application.Tests/   ✅ Application Tests
    ├── Andro.Backend.Reference.EntityFrameworkCore.Tests/ ✅ EF Core Tests
    ├── Andro.Backend.Reference.Web.Tests/           ✅ Web Tests
    └── Andro.Backend.Reference.TestBase/            ✅ Shared Test Infrastructure
```

---

## 🔧 Base Classes للـ Testing

### **1. ReferenceApplicationTestBase**

**Location:** `test/Andro.Backend.Reference.Application.Tests/ReferenceApplicationTestBase.cs`

**Usage:** Application Service tests

```csharp
public class ProductAppService_Tests : ReferenceApplicationTestBase
{
    private readonly IProductAppService _productAppService;

    public ProductAppService_Tests()
    {
        _productAppService = GetRequiredService<IProductAppService>();
    }

    [Fact]
    public async Task Should_Get_Product_List()
    {
        // Arrange, Act, Assert
    }
}
```

---

### **2. ReferenceDomainTestBase**

**Location:** `test/Andro.Backend.Reference.Domain.Tests/ReferenceDomainTestBase.cs`

**Usage:** Domain layer tests

```csharp
public class Product_Tests : ReferenceDomainTestBase
{
    [Fact]
    public void Should_Set_Price()
    {
        // Test domain logic
    }
}
```

---

### **3. ReferenceEntityFrameworkCoreTestBase**

**Location:** `test/Andro.Backend.Reference.EntityFrameworkCore.Tests/ReferenceEntityFrameworkCoreTestBase.cs`

**Usage:** Repository & Database tests

```csharp
public class ProductRepository_Tests : ReferenceEntityFrameworkCoreTestBase
{
    [Fact]
    public async Task Should_Insert_Product()
    {
        // Test repository operations
    }
}
```

---

## 🧪 كتابة Unit Tests

### **Example 1: Testing Domain Validation**

```csharp
// test/Andro.Backend.Reference.Domain.Tests/Products/Product_Tests.cs
using System;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Andro.Backend.Reference.Products;

public class Product_Tests : ReferenceDomainTestBase
{
    [Fact]
    public void Should_Set_Valid_Price()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act
        product.SetPrice(199.99m);

        // Assert
        product.Price.ShouldBe(199.99m);
    }

    [Fact]
    public void Should_Not_Allow_Invalid_Price()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<BusinessException>(() => 
        {
            product.SetPrice(2000000m); // Price > MaxPrice
        });
    }

    [Fact]
    public void Should_Decrease_Stock()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            100,
            Guid.NewGuid()
        );

        // Act
        product.DecreaseStock(30);

        // Assert
        product.Stock.ShouldBe(70);
    }

    [Fact]
    public void Should_Throw_When_Insufficient_Stock()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<InsufficientStockException>(() => 
        {
            product.DecreaseStock(20); // More than available
        });
    }
}
```

---

### **Example 2: Testing Application Service**

```csharp
// test/Andro.Backend.Reference.Application.Tests/Products/ProductAppService_Tests.cs
using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace Andro.Backend.Reference.Products;

public class ProductAppService_Tests : ReferenceApplicationTestBase
{
    private readonly IProductAppService _productAppService;

    public ProductAppService_Tests()
    {
        _productAppService = GetRequiredService<IProductAppService>();
    }

    [Fact]
    public async Task Should_Get_Product_List()
    {
        // Act
        var result = await _productAppService.GetListAsync(
            new PagedAndSortedResultRequestDto()
        );

        // Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldNotBeNull();
        result.Items.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Create_Valid_Product()
    {
        // Arrange
        var input = new CreateProductDto
        {
            Name = "Test Product",
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.Parse("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"), // من Seed
            Description = "Test description"
        };

        // Act
        var result = await _productAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(input.Name);
        result.Price.ShouldBe(input.Price);
        result.Stock.ShouldBe(input.Stock);
    }

    [Fact]
    public async Task Should_Not_Create_Product_With_Invalid_Category()
    {
        // Arrange
        var input = new CreateProductDto
        {
            Name = "Test Product",
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.NewGuid(), // Non-existent category
            Description = "Test"
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.CreateAsync(input);
        });

        exception.Code.ShouldBe(ReferenceDomainErrorCodes.CategoryNotFound);
    }

    [Fact]
    public async Task Should_Not_Create_Duplicate_Product()
    {
        // Arrange - Create first product
        var input1 = new CreateProductDto
        {
            Name = "Unique Product Name",
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.Parse("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f")
        };
        await _productAppService.CreateAsync(input1);

        // Act & Assert - Try to create duplicate
        var input2 = new CreateProductDto
        {
            Name = "Unique Product Name", // Same name
            Price = 199.99m,
            Stock = 20,
            CategoryId = Guid.Parse("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f")
        };

        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.CreateAsync(input2);
        });

        exception.Code.ShouldBe(ReferenceDomainErrorCodes.DuplicateProductName);
    }

    [Fact]
    public async Task Should_Update_Product()
    {
        // Arrange - Create product first
        var createInput = new CreateProductDto
        {
            Name = "Original Name",
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.Parse("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f")
        };
        var created = await _productAppService.CreateAsync(createInput);

        // Act - Update
        var updateInput = new UpdateProductDto
        {
            Name = "Updated Name",
            Price = 199.99m,
            Stock = 20,
            CategoryId = Guid.Parse("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f")
        };
        var updated = await _productAppService.UpdateAsync(created.Id, updateInput);

        // Assert
        updated.Name.ShouldBe("Updated Name");
        updated.Price.ShouldBe(199.99m);
        updated.Stock.ShouldBe(20);
    }

    [Fact]
    public async Task Should_Delete_Product()
    {
        // Arrange - Create product
        var createInput = new CreateProductDto
        {
            Name = "To Be Deleted",
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.Parse("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f")
        };
        var created = await _productAppService.CreateAsync(createInput);

        // Act
        await _productAppService.DeleteAsync(created.Id);

        // Assert - Should not find deleted product
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _productAppService.GetAsync(created.Id);
        });
    }
}
```

---

### **Example 3: Testing Event Handlers**

```csharp
// test/Andro.Backend.Reference.Application.Tests/Products/ProductCreatedEventHandler_Tests.cs
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Andro.Backend.Reference.Products.EventHandlers;

public class ProductCreatedEventHandler_Tests : ReferenceApplicationTestBase
{
    private readonly ProductCreatedEventHandler _handler;
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler_Tests()
    {
        _logger = GetRequiredService<ILogger<ProductCreatedEventHandler>>();
        _handler = new ProductCreatedEventHandler(_logger);
    }

    [Fact]
    public async Task Should_Handle_ProductCreated_Event()
    {
        // Arrange
        var eventData = new ProductCreatedEvent(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act - Should not throw
        await _handler.HandleEventAsync(eventData);

        // Assert - Event handled successfully
        // (في الواقع، هنا يمكن التحقق من الـ logging إذا كنت تستخدم mock logger)
        eventData.ShouldNotBeNull();
    }
}
```

---

## 🎯 AAA Pattern في Testing

**AAA = Arrange, Act, Assert**

```csharp
[Fact]
public async Task Should_Do_Something()
{
    // 1. Arrange - إعداد البيانات
    var input = new CreateProductDto { ... };

    // 2. Act - تنفيذ الكود
    var result = await _productAppService.CreateAsync(input);

    // 3. Assert - التحقق من النتيجة
    result.ShouldNotBeNull();
    result.Name.ShouldBe(input.Name);
}
```

---

## 📝 Shouldly Assertions

ABP يستخدم **Shouldly** library للـ assertions (أفضل من Assert):

```csharp
// ✅ Shouldly - readable & clear
result.ShouldNotBeNull();
result.Name.ShouldBe("Expected Name");
result.Price.ShouldBeGreaterThan(0);
result.Items.Count.ShouldBe(5);

// ❌ Traditional Assert - less readable
Assert.NotNull(result);
Assert.Equal("Expected Name", result.Name);
Assert.True(result.Price > 0);
Assert.Equal(5, result.Items.Count);
```

**Common Shouldly Methods:**
```csharp
value.ShouldBe(expected)
value.ShouldNotBe(unexpected)
value.ShouldBeNull()
value.ShouldNotBeNull()
value.ShouldBeGreaterThan(5)
value.ShouldBeLessThan(10)
value.ShouldBeInRange(1, 10)
collection.ShouldContain(item)
collection.ShouldNotContain(item)
collection.ShouldBeEmpty()
collection.ShouldNotBeEmpty()
Should.Throw<Exception>(() => { ... })
await Should.ThrowAsync<Exception>(async () => { ... })
```

---

## 🚀 تشغيل Tests

### **من Command Line:**

```powershell
# Run all tests
dotnet test

# Run specific test project
dotnet test test/Andro.Backend.Reference.Application.Tests

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~ProductAppService_Tests.Should_Create_Valid_Product"
```

---

### **من Visual Studio:**

1. **Test Explorer** - View → Test Explorer
2. **Run All** - تشغيل كل Tests
3. **Run Selected** - تشغيل tests محددة
4. **Debug** - تشغيل مع Debugging

---

### **من Rider:**

1. **Unit Tests Window** - View → Tool Windows → Unit Tests
2. **Run** - تشغيل tests
3. **Debug** - تشغيل مع debugging
4. **Coverage** - تشغيل مع code coverage

---

## 🗄️ Testing مع Database

ABP يستخدم **In-Memory Database** للـ tests تلقائياً:

```csharp
// لا تحتاج إعداد Database!
// ABP يوفر In-Memory DB automatically

public class ProductAppService_Tests : ReferenceApplicationTestBase
{
    // يمكنك استخدام Repository مباشرة
    private readonly IRepository<Product, Guid> _productRepository;

    [Fact]
    public async Task Can_Insert_Product()
    {
        var product = new Product(...);
        await _productRepository.InsertAsync(product);
        
        // تم الحفظ في In-Memory DB
        var saved = await _productRepository.GetAsync(product.Id);
        saved.ShouldNotBeNull();
    }
}
```

---

## 🧹 Data Isolation في Tests

كل test يعمل في **UnitOfWork** منفصل:

```csharp
[Fact]
public async Task Test1()
{
    // هذا الـ test له UnitOfWork خاص
    await _repository.InsertAsync(product1);
    // البيانات لن تؤثر على Test2
}

[Fact]
public async Task Test2()
{
    // هذا الـ test له UnitOfWork خاص أيضاً
    await _repository.InsertAsync(product2);
    // البيانات مستقلة تماماً
}
```

---

## 🎨 Testing Best Practices

### ✅ **Do:**

1. ✅ **Test One Thing**
   - كل test يختبر شيء واحد فقط

2. ✅ **Clear Names**
   - أسماء واضحة توضح ماذا يختبر
   - `Should_Create_Product_When_Valid_Input`

3. ✅ **AAA Pattern**
   - Arrange, Act, Assert

4. ✅ **Independent Tests**
   - كل test مستقل ولا يعتمد على الآخر

5. ✅ **Fast Tests**
   - Tests يجب أن تكون سريعة

6. ✅ **Meaningful Assertions**
   - تحقق من الأشياء المهمة فقط

---

### ❌ **Don't:**

1. ❌ **Multiple Assertions للأشياء غير المرتبطة**
   - لا تختبر أشياء كثيرة في test واحد

2. ❌ **Tests تعتمد على ترتيب التنفيذ**
   - كل test يجب أن يعمل بمفرده

3. ❌ **Hard-coded Values**
   - استخدم constants أو test data builders

4. ❌ **Testing Implementation Details**
   - اختبر السلوك وليس التفاصيل الداخلية

5. ❌ **Ignoring Failed Tests**
   - أصلح الـ tests المكسورة فوراً

---

## 📊 Test Coverage

**الهدف:** 80%+ code coverage

```powershell
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# With ReportGenerator (install first)
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage
```

---

## 🔍 Testing Checklist

### **Domain Layer:**
- [ ] Entity constructors
- [ ] Domain validation methods
- [ ] Business rule enforcement
- [ ] Domain events

### **Application Layer:**
- [ ] CRUD operations
- [ ] Business exceptions
- [ ] Permission checks
- [ ] Input validation

### **Event Handlers:**
- [ ] Event handling logic
- [ ] Side effects

---

## 💡 مثال كامل - Test Suite

```csharp
public class ProductAppService_Complete_Tests : ReferenceApplicationTestBase
{
    private readonly IProductAppService _service;
    private readonly Guid _testCategoryId = Guid.Parse("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f");

    public ProductAppService_Complete_Tests()
    {
        _service = GetRequiredService<IProductAppService>();
    }

    [Fact] public async Task Should_Get_List() { ... }
    [Fact] public async Task Should_Get_By_Id() { ... }
    [Fact] public async Task Should_Create() { ... }
    [Fact] public async Task Should_Update() { ... }
    [Fact] public async Task Should_Delete() { ... }
    [Fact] public async Task Should_Validate_Input() { ... }
    [Fact] public async Task Should_Check_Permissions() { ... }
    [Fact] public async Task Should_Throw_On_Duplicate() { ... }
    [Fact] public async Task Should_Throw_On_Invalid_Category() { ... }
}
```

---

## 🚀 الخلاصة

**Testing في ABP:**
- ✅ **Built-in Support** - كل شيء جاهز
- ✅ **xUnit** - Modern testing framework
- ✅ **Shouldly** - Readable assertions
- ✅ **In-Memory DB** - سريع وسهل
- ✅ **Base Classes** - تسهل الاختبار
- ✅ **Data Isolation** - كل test مستقل
- ✅ **UnitOfWork** - Transaction management

**Structure:**
```
Test Project
  ├── Domain Tests (Unit)
  ├── Application Tests (Integration)
  ├── Repository Tests (Integration)
  └── Event Handler Tests (Unit)
```

**Benefits:**
- 🎯 Quality assurance
- 🔒 Regression prevention
- 📚 Living documentation
- 💪 Refactoring confidence
- 🐛 Early bug detection

---

**Tests = Safety Net + Documentation + Quality! 🧪**
