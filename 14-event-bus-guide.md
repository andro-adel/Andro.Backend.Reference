# 🔔 Event Bus & Domain Events في ABP.io - دليل شامل

## 📋 نظرة عامة

Event Bus هو نظام لإرسال واستقبال الأحداث (Events) بين مكونات التطبيق المختلفة. Domain Events هي أحداث تحدث في الـ Domain Layer وتعبر عن شيء مهم حدث في Business Logic.

---

## 🎯 أهمية Domain Events

### ✅ **الفوائد:**

1. **Loose Coupling** - فصل المكونات عن بعضها
2. **Single Responsibility** - كل Handler يفعل شيء واحد
3. **Scalability** - سهل توسيع الوظائف
4. **Testability** - سهل اختبار كل جزء منفصل
5. **Clear Business Logic** - يعبر عن Business Events بوضوح
6. **Async Processing** - معالجة غير متزامنة ممكنة

### ⚠️ **بدون Domain Events:**

- ❌ كود متشابك ومعقد
- ❌ صعوبة إضافة وظائف جديدة
- ❌ Multiple responsibilities في نفس المكان
- ❌ صعوبة الاختبار

---

## 📚 أنواع Events في ABP

### **1️⃣ Local Events (In-Process)**

**الاستخدام:** داخل نفس التطبيق  
**الأداء:** سريع جداً  
**الموثوقية:** Transaction-safe

```csharp
// نشر Event محلي
await _localEventBus.PublishAsync(new ProductCreatedEvent(product));

// معالج محلي
public class ProductCreatedEventHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        // معالجة الحدث
    }
}
```

---

### **2️⃣ Distributed Events (Cross-Process)**

**الاستخدام:** بين تطبيقات مختلفة  
**الأداء:** أبطأ (Network)  
**الموثوقية:** قد تحتاج Message Queue (RabbitMQ, Kafka)

```csharp
// نشر Event موزع
await _distributedEventBus.PublishAsync(new ProductCreatedEto(product));

// معالج موزع
public class ProductCreatedDistributedHandler 
    : IDistributedEventHandler<ProductCreatedEto>
{
    public async Task HandleEventAsync(ProductCreatedEto eventData)
    {
        // معالجة موزعة
    }
}
```

---

## 🏗️ بنية Domain Events

### **المكونات الأساسية:**

```
1. Event Class (الحدث)
   ↓
2. Publish Event (نشر الحدث)
   ↓
3. Event Handler (معالج الحدث)
   ↓
4. Business Logic (المعالجة)
```

---

## 🔧 1. إنشاء Domain Event

### **مثال: ProductCreatedEvent**

```csharp
// Domain/Products/ProductCreatedEvent.cs
using Volo.Abp.Domain.Entities.Events;

namespace Andro.Backend.Reference.Products;

public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ProductCreatedEvent(
        Guid productId,
        string productName,
        decimal price,
        int stock,
        Guid categoryId)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
    }
}
```

---

### **مثال: ProductStockChangedEvent**

```csharp
// Domain/Products/ProductStockChangedEvent.cs
namespace Andro.Backend.Reference.Products;

public class ProductStockChangedEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int OldStock { get; set; }
    public int NewStock { get; set; }
    public int ChangeAmount { get; set; }
    public StockChangeType ChangeType { get; set; }
    public DateTime ChangedAt { get; set; }

    public ProductStockChangedEvent(
        Guid productId,
        string productName,
        int oldStock,
        int newStock,
        StockChangeType changeType)
    {
        ProductId = productId;
        ProductName = productName;
        OldStock = oldStock;
        NewStock = newStock;
        ChangeAmount = Math.Abs(newStock - oldStock);
        ChangeType = changeType;
        ChangedAt = DateTime.UtcNow;
    }
}

public enum StockChangeType
{
    Increased,
    Decreased
}
```

---

## 🎨 2. نشر Events في Domain Layer

### **في Product Entity:**

```csharp
// Domain/Products/Product.cs
using Volo.Abp.Domain.Entities;

public class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    
    // إضافة Local Events
    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessException("Invalid quantity");

        var oldStock = Stock;
        Stock += quantity;

        // نشر Domain Event
        AddLocalEvent(new ProductStockChangedEvent(
            Id,
            Name,
            oldStock,
            Stock,
            StockChangeType.Increased
        ));
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessException("Invalid quantity");

        if (Stock < quantity)
            throw new InsufficientStockException(Name, quantity, Stock);

        var oldStock = Stock;
        Stock -= quantity;

        // نشر Domain Event
        AddLocalEvent(new ProductStockChangedEvent(
            Id,
            Name,
            oldStock,
            Stock,
            StockChangeType.Decreased
        ));
    }
}
```

---

### **في Application Service:**

```csharp
// Application/Products/ProductAppService.cs
public class ProductAppService : ApplicationService
{
    private readonly IRepository<Product, Guid> _repository;
    private readonly ILocalEventBus _localEventBus;

    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // Create product
        var product = new Product(
            GuidGenerator.Create(),
            input.Name,
            input.Price,
            input.Stock,
            input.CategoryId
        );

        await _repository.InsertAsync(product);

        // نشر Event بعد الحفظ
        await _localEventBus.PublishAsync(
            new ProductCreatedEvent(
                product.Id,
                product.Name,
                product.Price,
                product.Stock,
                product.CategoryId
            )
        );

        return MapToDto(product);
    }
}
```

---

## 🎧 3. إنشاء Event Handlers

### **مثال: ProductCreatedEventHandler**

```csharp
// Application/Products/EventHandlers/ProductCreatedEventHandler.cs
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Andro.Backend.Reference.Products.EventHandlers;

public class ProductCreatedEventHandler 
    : ILocalEventHandler<ProductCreatedEvent>,
      ITransientDependency
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        _logger.LogInformation(
            "🎉 New Product Created: {ProductName} (ID: {ProductId}) - Price: {Price}, Stock: {Stock}",
            eventData.ProductName,
            eventData.ProductId,
            eventData.Price,
            eventData.Stock
        );

        // يمكن إضافة logic إضافية هنا:
        // - إرسال email للمسؤولين
        // - تحديث cache
        // - إرسال notification
        // - تسجيل في Audit Log
        // - إلخ...

        await Task.CompletedTask;
    }
}
```

---

### **مثال: ProductStockChangedEventHandler**

```csharp
// Application/Products/EventHandlers/ProductStockChangedEventHandler.cs
public class ProductStockChangedEventHandler 
    : ILocalEventHandler<ProductStockChangedEvent>,
      ITransientDependency
{
    private readonly ILogger<ProductStockChangedEventHandler> _logger;

    public ProductStockChangedEventHandler(
        ILogger<ProductStockChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(ProductStockChangedEvent eventData)
    {
        var changeIcon = eventData.ChangeType == StockChangeType.Increased 
            ? "📈" : "📉";

        _logger.LogInformation(
            "{Icon} Stock Changed: {ProductName} - {OldStock} → {NewStock} ({ChangeType}: {ChangeAmount})",
            changeIcon,
            eventData.ProductName,
            eventData.OldStock,
            eventData.NewStock,
            eventData.ChangeType,
            eventData.ChangeAmount
        );

        // معالجة إضافية:
        if (eventData.ChangeType == StockChangeType.Decreased 
            && eventData.NewStock < 10)
        {
            _logger.LogWarning(
                "⚠️ Low Stock Alert: {ProductName} - Only {Stock} items left!",
                eventData.ProductName,
                eventData.NewStock
            );

            // يمكن إرسال تنبيه للمسؤولين
        }

        await Task.CompletedTask;
    }
}
```

---

## 🔄 Event Flow

```
User Action (Create Product)
  ↓
Application Service
  ↓
Domain Entity (Product)
  ↓
AddLocalEvent(ProductCreatedEvent)
  ↓
Repository.InsertAsync()
  ↓
UnitOfWork commits transaction
  ↓
Events are published
  ↓
ProductCreatedEventHandler.HandleEventAsync()
  ↓
Business Logic (Log, Email, etc.)
```

---

## 🎯 Use Cases

### **1. Audit Logging**

```csharp
public class ProductAuditEventHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        // تسجيل في Audit Log
        await _auditLogRepository.InsertAsync(new AuditLog
        {
            Action = "ProductCreated",
            EntityId = eventData.ProductId,
            Details = $"Product '{eventData.ProductName}' created"
        });
    }
}
```

---

### **2. Email Notifications**

```csharp
public class ProductCreatedEmailHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    private readonly IEmailSender _emailSender;

    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        // إرسال email للمسؤولين
        await _emailSender.SendAsync(
            to: "admin@company.com",
            subject: "New Product Created",
            body: $"Product '{eventData.ProductName}' has been created"
        );
    }
}
```

---

### **3. Cache Invalidation**

```csharp
public class ProductCacheInvalidationHandler 
    : ILocalEventHandler<ProductCreatedEvent>,
      ILocalEventHandler<ProductStockChangedEvent>
{
    private readonly IDistributedCache _cache;

    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        // مسح الـ cache
        await _cache.RemoveAsync("ProductList");
    }

    public async Task HandleEventAsync(ProductStockChangedEvent eventData)
    {
        await _cache.RemoveAsync($"Product:{eventData.ProductId}");
    }
}
```

---

### **4. Statistics & Analytics**

```csharp
public class ProductStatisticsHandler 
    : ILocalEventHandler<ProductCreatedEvent>
{
    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        // تحديث الإحصائيات
        await _statisticsRepository.IncrementAsync("TotalProducts");
        await _statisticsRepository.IncrementAsync(
            $"ProductsInCategory:{eventData.CategoryId}"
        );
    }
}
```

---

## 📝 Best Practices

### ✅ **Do:**

1. ✅ **استخدم Events للـ Side Effects**
   - Logging
   - Notifications
   - Cache management
   - Statistics

2. ✅ **Keep Events Simple**
   - فقط البيانات المهمة
   - Immutable objects

3. ✅ **Single Responsibility**
   - كل Handler يفعل شيء واحد

4. ✅ **Error Handling**
   - لا تجعل الـ Handler يفشل المعاملة الأصلية

5. ✅ **Use Async**
   - استخدم async/await

6. ✅ **Logging**
   - سجل كل event مهم

---

### ❌ **Don't:**

1. ❌ **لا تضع Business Logic الأساسية في Handlers**
   - Domain Logic يبقى في Domain Layer

2. ❌ **لا تعتمد على ترتيب التنفيذ**
   - Handlers قد تعمل بأي ترتيب

3. ❌ **لا تجعل Handlers تفشل Transaction**
   - استخدم try-catch

4. ❌ **لا تنشر Events كثيرة جداً**
   - فقط الأحداث المهمة

5. ❌ **لا تضع data كثيرة في Event**
   - فقط IDs والبيانات الضرورية

---

## 🧪 Testing Events

### **Unit Test للـ Event Handler:**

```csharp
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

        // Act
        await _handler.HandleEventAsync(eventData);

        // Assert
        // تحقق من أن الـ logging تم
        // تحقق من أن Side effects حدثت
    }
}
```

---

## 🔀 Local vs Distributed Events

### **متى تستخدم Local Events:**

- ✅ داخل نفس التطبيق
- ✅ Transaction-safe مطلوب
- ✅ Performance مهم
- ✅ Side effects بسيطة

**Examples:**
- Logging
- Cache invalidation
- Statistics update
- Audit logs

---

### **متى تستخدم Distributed Events:**

- ✅ بين تطبيقات مختلفة (Microservices)
- ✅ Async processing مطلوب
- ✅ Eventual consistency مقبول
- ✅ Message queue متوفر

**Examples:**
- Email notifications (خارجي)
- SMS service (خارجي)
- Integration مع نظام آخر
- Background jobs طويلة

---

## 💡 مثال عملي كامل

### **Scenario: Product Created**

**1. User creates product**  
**2. Event published**  
**3. Multiple handlers react:**

```
ProductCreatedEvent
  ├─ ProductCreatedEventHandler
  │   └─ Log creation
  │
  ├─ ProductAuditEventHandler
  │   └─ Create audit log
  │
  ├─ ProductCacheHandler
  │   └─ Invalidate cache
  │
  ├─ ProductStatisticsHandler
  │   └─ Update statistics
  │
  └─ ProductEmailHandler
      └─ Send notification email
```

**كل Handler مستقل وقابل للاختبار منفصل!**

---

## 🚀 الخلاصة

**Domain Events في ABP:**
- ✅ **Loose Coupling** - فصل المكونات
- ✅ **Extensible** - سهل إضافة handlers جديدة
- ✅ **Testable** - كل handler منفصل
- ✅ **Clear Business Logic** - يعبر عن الأحداث المهمة
- ✅ **Transaction-Safe** - مع Local Events
- ✅ **Scalable** - مع Distributed Events

**Structure:**
```
Domain/
  └── Products/
      ├── Product.cs (Entity with events)
      ├── ProductCreatedEvent.cs
      └── ProductStockChangedEvent.cs

Application/
  └── Products/
      └── EventHandlers/
          ├── ProductCreatedEventHandler.cs
          └── ProductStockChangedEventHandler.cs
```

**Usage:**
1. **Define Event** - في Domain Layer
2. **Publish Event** - من Entity أو Service
3. **Create Handler** - في Application Layer
4. **React** - معالجة الحدث

---

**Events = Clean Architecture + Extensibility! 🔔**
