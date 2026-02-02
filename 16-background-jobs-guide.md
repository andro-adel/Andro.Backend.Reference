# ⚙️ Background Jobs في ABP.io - دليل شامل

## 📋 نظرة عامة

Background Jobs هي مهام تعمل في الخلفية بشكل غير متزامن (Asynchronous) بعيداً عن الـ HTTP request الأساسي. تُستخدم لتنفيذ عمليات طويلة أو متكررة بدون إبطاء استجابة المستخدم.

---

## 🎯 أهمية Background Jobs

### ✅ **الفوائد:**

1. **Better User Experience** - لا انتظار للعمليات الطويلة
2. **Scalability** - توزيع الحمل على الخوادم
3. **Reliability** - إعادة المحاولة عند الفشل
4. **Scheduling** - جدولة مهام دورية
5. **Async Processing** - معالجة غير متزامنة
6. **Resource Management** - استخدام أفضل للموارد

### ⚠️ **بدون Background Jobs:**

- ❌ Timeout للـ requests الطويلة
- ❌ تجربة مستخدم سيئة
- ❌ استهلاك موارد HTTP threads
- ❌ صعوبة جدولة المهام الدورية

---

## 📚 أنواع Background Processing في ABP

### **1️⃣ Background Jobs**

**الاستخدام:** مهام تُنفذ مرة واحدة في المستقبل

**Characteristics:**
- ✅ One-time execution
- ✅ Queued for later execution
- ✅ Retry on failure
- ✅ Persistent (stored in DB)

**Examples:**
- إرسال email
- معالجة ملف كبير
- إنشاء تقرير
- Sync with external system

---

### **2️⃣ Background Workers**

**الاستخدام:** مهام دورية تعمل باستمرار

**Characteristics:**
- ✅ Periodic execution
- ✅ Run in background continuously
- ✅ Timer-based or event-based
- ✅ No user interaction

**Examples:**
- تنظيف البيانات القديمة
- فحص الكمية المنخفضة
- Sync data periodically
- Health checks

---

## 🏗️ Background Job Architecture

```
User Action
  ↓
Enqueue Background Job
  ↓
Job stored in Database
  ↓
Return immediately to user
  ↓
Background Job Manager
  ↓
Execute job async
  ↓
Retry if failed
```

---

## 🔧 1. إنشاء Background Job

### **Step 1: Create Job Arguments Class**

```csharp
// Application/Products/Jobs/LowStockAlertJobArgs.cs
public class LowStockAlertJobArgs
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }

    public LowStockAlertJobArgs()
    {
    }

    public LowStockAlertJobArgs(
        Guid productId,
        string productName,
        int currentStock,
        int minimumStock)
    {
        ProductId = productId;
        ProductName = productName;
        CurrentStock = currentStock;
        MinimumStock = minimumStock;
    }
}
```

---

### **Step 2: Create Job Class**

```csharp
// Application/Products/Jobs/LowStockAlertJob.cs
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Andro.Backend.Reference.Products.Jobs;

public class LowStockAlertJob 
    : AsyncBackgroundJob<LowStockAlertJobArgs>, 
      ITransientDependency
{
    private readonly ILogger<LowStockAlertJob> _logger;

    public LowStockAlertJob(ILogger<LowStockAlertJob> logger)
    {
        _logger = logger;
    }

    public override async Task ExecuteAsync(LowStockAlertJobArgs args)
    {
        _logger.LogWarning(
            "⚠️ LOW STOCK ALERT: Product '{ProductName}' (ID: {ProductId}) - Current Stock: {CurrentStock}, Minimum: {MinimumStock}",
            args.ProductName,
            args.ProductId,
            args.CurrentStock,
            args.MinimumStock
        );

        // يمكن إضافة logic إضافية:
        // - إرسال email للمسؤولين
        // - إرسال SMS
        // - إنشاء notification في النظام
        // - تحديث dashboard
        // - إلخ...

        await Task.CompletedTask;
    }
}
```

---

### **Step 3: Enqueue the Job**

```csharp
// في Event Handler أو Service
using Volo.Abp.BackgroundJobs;

public class ProductStockChangedEventHandler 
    : ILocalEventHandler<ProductStockChangedEvent>
{
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly ILogger<ProductStockChangedEventHandler> _logger;

    public async Task HandleEventAsync(ProductStockChangedEvent eventData)
    {
        // ... existing logging ...

        // إذا الكمية أقل من 10، أنشئ Background Job
        if (eventData.ChangeType == StockChangeType.Decreased 
            && eventData.NewStock < 10)
        {
            // Enqueue background job
            await _backgroundJobManager.EnqueueAsync(
                new LowStockAlertJobArgs(
                    eventData.ProductId,
                    eventData.ProductName,
                    eventData.NewStock,
                    10 // minimum stock
                )
            );

            _logger.LogInformation(
                "🔔 Low stock alert job enqueued for product: {ProductName}",
                eventData.ProductName
            );
        }
    }
}
```

---

## 🔄 2. إنشاء Background Worker

### **Example: Periodic Stock Check Worker**

```csharp
// Application/Products/Workers/StockCheckWorker.cs
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using Volo.Abp.Domain.Repositories;

namespace Andro.Backend.Reference.Products.Workers;

public class StockCheckWorker : AsyncPeriodicBackgroundWorkerBase
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly ILogger<StockCheckWorker> _logger;

    public StockCheckWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IRepository<Product, Guid> productRepository,
        ILogger<StockCheckWorker> logger) 
        : base(timer, serviceScopeFactory)
    {
        _productRepository = productRepository;
        _logger = logger;
        
        // Run every 5 minutes
        Timer.Period = 5 * 60 * 1000; // milliseconds
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        _logger.LogInformation("🔍 Stock check worker started at {Time}", DateTime.Now);

        // Get all products with low stock
        var lowStockProducts = await _productRepository
            .GetListAsync(p => p.Stock < 10);

        if (lowStockProducts.Count > 0)
        {
            _logger.LogWarning(
                "⚠️ Found {Count} products with low stock",
                lowStockProducts.Count
            );

            foreach (var product in lowStockProducts)
            {
                _logger.LogWarning(
                    "📦 Low Stock: {ProductName} - Current: {Stock}",
                    product.Name,
                    product.Stock
                );

                // يمكن إنشاء notifications أو alerts
            }
        }
        else
        {
            _logger.LogInformation("✅ All products have sufficient stock");
        }
    }
}
```

---

### **Register Worker in Module**

```csharp
// Application/ReferenceApplicationModule.cs
using Andro.Backend.Reference.Products.Workers;

[DependsOn(typeof(AbpBackgroundWorkersModule))]
public class ReferenceApplicationModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(
        ApplicationInitializationContext context)
    {
        // Add background worker
        await context.AddBackgroundWorkerAsync<StockCheckWorker>();
    }
}
```

---

## 📊 Job Execution Flow

### **Background Job:**
```
1. User Action (e.g., Update Stock)
   ↓
2. Event Handler detects low stock
   ↓
3. Enqueue LowStockAlertJob
   ↓
4. Job saved to database
   ↓
5. Return to user immediately
   ↓
6. Background Job Manager picks up job
   ↓
7. Execute job (send alerts)
   ↓
8. Mark job as completed
```

### **Background Worker:**
```
1. Application starts
   ↓
2. StockCheckWorker registered
   ↓
3. Worker starts with timer
   ↓
4. Every 5 minutes:
   - Check all products
   - Find low stock items
   - Log warnings
   - Create alerts
   ↓
5. Repeat continuously
```

---

## 🎯 Use Cases

### **1. Email Sending**

```csharp
public class SendEmailJobArgs
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}

public class SendEmailJob : AsyncBackgroundJob<SendEmailJobArgs>
{
    private readonly IEmailSender _emailSender;

    public override async Task ExecuteAsync(SendEmailJobArgs args)
    {
        await _emailSender.SendAsync(
            args.To,
            args.Subject,
            args.Body
        );
    }
}
```

---

### **2. Report Generation**

```csharp
public class GenerateReportJob : AsyncBackgroundJob<GenerateReportJobArgs>
{
    public override async Task ExecuteAsync(GenerateReportJobArgs args)
    {
        // 1. Fetch data
        var data = await _repository.GetListAsync();

        // 2. Generate report (PDF, Excel, etc.)
        var report = await _reportGenerator.GenerateAsync(data);

        // 3. Save to file system or cloud
        await _fileStorage.SaveAsync(report);

        // 4. Send notification to user
        await _notificationService.NotifyAsync(args.UserId, "Report ready!");
    }
}
```

---

### **3. Data Cleanup Worker**

```csharp
public class DataCleanupWorker : AsyncPeriodicBackgroundWorkerBase
{
    public DataCleanupWorker(AbpAsyncTimer timer, ...) : base(timer, ...)
    {
        Timer.Period = 24 * 60 * 60 * 1000; // Run daily
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        // Delete old logs older than 30 days
        var cutoffDate = DateTime.Now.AddDays(-30);
        await _logRepository.DeleteAsync(l => l.CreatedTime < cutoffDate);

        _logger.LogInformation("🗑️ Old logs cleaned up");
    }
}
```

---

### **4. External API Sync**

```csharp
public class SyncProductsWorker : AsyncPeriodicBackgroundWorkerBase
{
    public SyncProductsWorker(AbpAsyncTimer timer, ...) : base(timer, ...)
    {
        Timer.Period = 60 * 60 * 1000; // Run every hour
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        // Fetch from external API
        var externalProducts = await _externalApi.GetProductsAsync();

        // Sync with local database
        foreach (var externalProduct in externalProducts)
        {
            var localProduct = await _productRepository
                .FirstOrDefaultAsync(p => p.ExternalId == externalProduct.Id);

            if (localProduct == null)
            {
                // Create new
                await _productRepository.InsertAsync(...);
            }
            else
            {
                // Update existing
                localProduct.Price = externalProduct.Price;
                await _productRepository.UpdateAsync(localProduct);
            }
        }

        _logger.LogInformation("🔄 Products synced with external system");
    }
}
```

---

## ⚙️ Configuration

### **Enable Background Jobs in Module**

```csharp
// ReferenceApplicationModule.cs
[DependsOn(
    typeof(AbpBackgroundJobsModule),
    typeof(AbpBackgroundWorkersModule)
)]
public class ReferenceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Configure background jobs
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = true; // Enable job execution
        });

        Configure<AbpBackgroundJobWorkerOptions>(options =>
        {
            options.DefaultTimeout = 60000; // 60 seconds
        });
    }
}
```

---

### **Disable in Development (Optional)**

```csharp
// appsettings.Development.json
{
  "BackgroundJobs": {
    "IsJobExecutionEnabled": false
  }
}
```

---

## 🔁 Retry Mechanism

Background Jobs تدعم إعادة المحاولة تلقائياً:

```csharp
public class MyJob : AsyncBackgroundJob<MyJobArgs>
{
    public override async Task ExecuteAsync(MyJobArgs args)
    {
        try
        {
            // Execute job
            await DoWorkAsync();
        }
        catch (Exception ex)
        {
            // Job will be retried automatically
            // ABP handles retry logic
            throw;
        }
    }
}
```

**Default Retry:**
- يُعاد المحاولة عدة مرات
- مع delay متزايد بين المحاولات
- إذا فشلت كل المحاولات، يُسجل الخطأ

---

## 📝 Best Practices

### ✅ **Do:**

1. ✅ **Keep Jobs Small & Focused**
   - كل job يفعل شيء واحد

2. ✅ **Use Arguments Class**
   - سهل تمرير البيانات

3. ✅ **Log Everything**
   - سجل البداية والنهاية والأخطاء

4. ✅ **Handle Exceptions**
   - استخدم try-catch

5. ✅ **Make Jobs Idempotent**
   - تنفيذها مرتين لا يسبب مشاكل

6. ✅ **Use Appropriate Timer**
   - اختر period مناسب للـ worker

---

### ❌ **Don't:**

1. ❌ **Long Running Jobs**
   - قسم المهام الطويلة لأجزاء صغيرة

2. ❌ **Blocking Operations**
   - استخدم async/await

3. ❌ **Too Frequent Workers**
   - لا تجعل timer قصير جداً

4. ❌ **Store Large Data in Args**
   - فقط IDs والبيانات الضرورية

5. ❌ **Forget Error Handling**
   - دائماً handle exceptions

---

## 🧪 Testing Background Jobs

### **Unit Test:**

```csharp
public class LowStockAlertJob_Tests
{
    [Fact]
    public async Task Should_Execute_Job()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LowStockAlertJob>>();
        var job = new LowStockAlertJob(logger);
        
        var args = new LowStockAlertJobArgs(
            Guid.NewGuid(),
            "Test Product",
            5,
            10
        );

        // Act
        await job.ExecuteAsync(args);

        // Assert
        // Verify logging happened
        logger.Received().LogWarning(Arg.Any<string>(), Arg.Any<object[]>());
    }
}
```

---

## 🔍 Monitoring Jobs

### **Check Job Status:**

```csharp
// في Controller أو Service
public class JobMonitoringService
{
    private readonly IBackgroundJobStore _jobStore;

    public async Task<List<BackgroundJobInfo>> GetPendingJobsAsync()
    {
        // Get all pending jobs
        return await _jobStore.GetWaitingJobsAsync(maxResultCount: 100);
    }
}
```

---

## 📊 Comparison: Jobs vs Workers

| Feature | Background Job | Background Worker |
|---------|----------------|-------------------|
| **Execution** | One-time | Periodic/Continuous |
| **Triggering** | Enqueued manually | Timer-based |
| **Persistence** | Stored in DB | In-memory |
| **Retry** | Yes | Manual |
| **Use Case** | Send email, Generate report | Data cleanup, Monitoring |

---

## 🚀 الخلاصة

**Background Jobs في ABP:**
- ✅ **Async Processing** - لا انتظار
- ✅ **Reliability** - retry mechanism
- ✅ **Scalability** - توزيع الحمل
- ✅ **Scheduling** - مهام دورية
- ✅ **Easy to Use** - API بسيط
- ✅ **Well Integrated** - مع ABP framework

**Structure:**
```
Application/
  └── Products/
      ├── Jobs/
      │   ├── LowStockAlertJob.cs
      │   └── LowStockAlertJobArgs.cs
      └── Workers/
          └── StockCheckWorker.cs
```

**Usage:**
1. **Create Job** - Define args & job class
2. **Enqueue Job** - `_backgroundJobManager.EnqueueAsync()`
3. **Execute** - ABP handles execution
4. **Monitor** - Check logs & status

---

**Background Jobs = Better Performance + Better UX! ⚙️**
