# 🏛️ المرحلة 1.2: فهم Domain Driven Design (DDD)

---

## ما هو Domain Driven Design؟

**DDD** هو نهج لتصميم البرمجيات يركز على:
- **الـ Domain** (مجال العمل) هو محور التطبيق
- تنظيم الكود حسب المنطق التجاري (Business Logic)
- عزل Business Logic عن التفاصيل التقنية (قاعدة البيانات، UI، إلخ)

**مثال بسيط:**
لو بنعمل تطبيق متجر إلكتروني:
- الـ Domain = المنتجات، الطلبات، العملاء، السلة
- ليس الـ Domain = كيف نحفظ في SQL، كيف نعرض الـ UI

---

## 🧱 المكونات الأساسية للـ DDD

### 1️⃣ Entity (الكيان)

**التعريف:**
كائن له **هوية فريدة** (Unique Identity) تميزه عن باقي الكائنات، حتى لو كانت جميع خصائصه متطابقة.

**مثال من الحياة:**
- **الشخص** = Entity
  - لو عندك شخصين اسمهم "أحمد محمد" ونفس العمر ونفس المدينة
  - لكن كل واحد له **رقم قومي مختلف** → هويتين مختلفتين

**مثال في التطبيق:**

```csharp
public class Product : Entity<Guid>
{
    public string Name { get; set; }           // "iPhone 15"
    public decimal Price { get; set; }         // 1000
    public int StockQuantity { get; set; }     // 50
    
    // الـ Id (من Entity<Guid>) هو الهوية الفريدة
}
```

**الفرق بين Entity و Object عادي:**

```csharp
// Entity
var product1 = new Product { Id = Guid.Parse("123..."), Name = "iPhone" };
var product2 = new Product { Id = Guid.Parse("456..."), Name = "iPhone" };
// product1 != product2 لأن الـ Id مختلف

// Object عادي (مثل DTO)
var dto1 = new ProductDto { Name = "iPhone", Price = 1000 };
var dto2 = new ProductDto { Name = "iPhone", Price = 1000 };
// dto1 == dto2 لأن كل الخصائص متطابقة
```

**خصائص الـ Entity:**
- ✅ له **Id** فريد
- ✅ له **دورة حياة** (يُنشأ، يُعدل، يُحذف)
- ✅ **يُحفظ في قاعدة البيانات**
- ✅ يمكن **تتبعه** (Tracking)

---

### 2️⃣ Aggregate & Aggregate Root

**المشكلة:**
لو عندك كيانات مرتبطة ببعض، مين المسؤول عن الحفاظ على **التناسق** (Consistency) بينهم؟

**الحل:** Aggregate

**Aggregate** = مجموعة من Entities و Value Objects مرتبطة ببعض، ليها **Aggregate Root** واحد.

**Aggregate Root** = الـ Entity الرئيسي اللي بنتعامل معاه من الخارج، وهو المسؤول عن باقي الكيانات.

---

**مثال عملي: طلب شراء (Order)**

```
Order (Aggregate Root)
├── Id
├── OrderDate
├── TotalPrice
├── CustomerId
└── OrderItems (مجموعة)
    ├── OrderItem 1
    │   ├── ProductId
    │   ├── Quantity
    │   └── Price
    ├── OrderItem 2
    └── OrderItem 3
```

**القواعد:**

1️⃣ **الوصول من الخارج فقط عبر الـ Root**
```csharp
// ✅ صح
var order = await orderRepository.GetAsync(orderId);
order.AddItem(productId, quantity, price);
await orderRepository.UpdateAsync(order);

// ❌ خطأ - لا تتعامل مع OrderItem مباشرة
var orderItem = new OrderItem(...);
await orderItemRepository.InsertAsync(orderItem); // خطأ!
```

2️⃣ **الـ Root مسؤول عن الـ Validation والـ Business Rules**
```csharp
public class Order : AggregateRoot<Guid>
{
    public List<OrderItem> Items { get; private set; }
    
    public void AddItem(Guid productId, int quantity, decimal price)
    {
        // Business Rule 1: التحقق من الكمية
        if (quantity <= 0)
            throw new BusinessException("الكمية يجب أن تكون أكبر من صفر");
        
        // Business Rule 2: منع التكرار
        var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            Items.Add(new OrderItem(productId, quantity, price));
        }
        
        // تحديث الإجمالي تلقائياً
        RecalculateTotalPrice();
    }
    
    private void RecalculateTotalPrice()
    {
        TotalPrice = Items.Sum(x => x.Quantity * x.Price);
    }
}
```

3️⃣ **الـ Repository يكون فقط للـ Aggregate Root**
```csharp
// ✅ يوجد
IRepository<Order, Guid> orderRepository

// ❌ لا يوجد
// IRepository<OrderItem, Guid> orderItemRepository
```

**الفائدة:**
- ✅ **التناسق مضمون** - كل التعديلات تمر عبر الـ Root
- ✅ **Business Rules محمية** - ما حدش يقدر يكسر القواعد
- ✅ **الكود منظم** - واضح مين المسؤول عن إيه

---

### 3️⃣ Value Object (كائن القيمة)

**التعريف:**
كائن **ليس له هوية فريدة**، المهم فيه هو **القيم** اللي فيه. لو قيمتين متطابقة = نفس الشيء.

**الفرق عن Entity:**

```csharp
// Entity - الهوية مهمة
Person person1 = new Person { Id = 1, Name = "أحمد" };
Person person2 = new Person { Id = 2, Name = "أحمد" };
// person1 != person2 (هويات مختلفة)

// Value Object - القيم مهمة
Address address1 = new Address { City = "القاهرة", Street = "شارع 1" };
Address address2 = new Address { City = "القاهرة", Street = "شارع 1" };
// address1 == address2 (نفس القيم = نفس الشيء)
```

**أمثلة على Value Objects:**

1️⃣ **العنوان (Address)**
```csharp
public class Address : ValueObject
{
    public string City { get; private set; }
    public string Street { get; private set; }
    public string ZipCode { get; private set; }
    
    public Address(string city, string street, string zipCode)
    {
        City = city;
        Street = street;
        ZipCode = zipCode;
    }
    
    // ABP يوفر GetAtomicValues للمقارنة
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return City;
        yield return Street;
        yield return ZipCode;
    }
}
```

2️⃣ **النطاق السعري (PriceRange)**
```csharp
public class PriceRange : ValueObject
{
    public decimal MinPrice { get; private set; }
    public decimal MaxPrice { get; private set; }
    
    public PriceRange(decimal minPrice, decimal maxPrice)
    {
        if (minPrice > maxPrice)
            throw new ArgumentException("الحد الأدنى أكبر من الحد الأقصى!");
            
        MinPrice = minPrice;
        MaxPrice = maxPrice;
    }
    
    public bool IsInRange(decimal price)
    {
        return price >= MinPrice && price <= MaxPrice;
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return MinPrice;
        yield return MaxPrice;
    }
}
```

3️⃣ **المال (Money)**
```csharp
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("لا يمكن جمع عملات مختلفة");
            
        return new Money(Amount + other.Amount, Currency);
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

**خصائص Value Object:**
- ✅ **Immutable** (غير قابل للتعديل) - لو عايز تغير قيمة، اعمل كائن جديد
- ✅ **المقارنة بالقيمة** وليس بالهوية
- ✅ **ليس له Id**
- ✅ **يحتوي على Business Logic** خاص بيه

**متى نستخدم Value Object؟**
- العنوان، المال، النطاق الزمني، الإحداثيات، اللون، إلخ
- أي شيء معرف **بقيمته** وليس بهويته

---

### 4️⃣ Domain Service (خدمة المجال)

**متى نحتاج Domain Service؟**

عندما يكون عندك **Business Logic لا ينتمي لـ Entity واحد محدد**.

**أمثلة:**

❌ **لا نحتاج Domain Service:**
```csharp
// العملية خاصة بـ Product فقط → تكون في Product Entity
public class Product : Entity<Guid>
{
    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new BusinessException("السعر لا يمكن أن يكون سالباً");
            
        Price = newPrice;
    }
}
```

✅ **نحتاج Domain Service:**
```csharp
// العملية تحتاج Product + Inventory + Pricing Rules → Domain Service
public class ProductManager : DomainService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Inventory> _inventoryRepository;
    
    public ProductManager(
        IRepository<Product> productRepository,
        IRepository<Inventory> inventoryRepository)
    {
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
    }
    
    public async Task<Product> CreateProductAsync(
        string name, 
        decimal price, 
        int initialStock)
    {
        // Business Rule 1: التحقق من عدم التكرار
        var existingProduct = await _productRepository
            .FirstOrDefaultAsync(x => x.Name == name);
            
        if (existingProduct != null)
            throw new BusinessException("المنتج موجود بالفعل");
        
        // Business Rule 2: التحقق من السعر حسب الفئة
        if (price < 10)
            throw new BusinessException("السعر الأدنى هو 10");
        
        // إنشاء المنتج
        var product = new Product(GuidGenerator.Create(), name, price);
        
        // إنشاء المخزون المرتبط
        var inventory = new Inventory(product.Id, initialStock);
        
        await _productRepository.InsertAsync(product);
        await _inventoryRepository.InsertAsync(inventory);
        
        return product;
    }
}
```

**الفرق بين Domain Service و Application Service:**

| Domain Service | Application Service |
|---------------|---------------------|
| في الـ **Domain Layer** | في الـ **Application Layer** |
| **Business Logic** نقي | **Use Cases** وتنسيق |
| يتعامل مع **Entities** | يتعامل مع **DTOs** |
| لا يعتمد على قاعدة بيانات محددة | يستخدم Repositories و Mapping |

**مثال للتوضيح:**

```csharp
// Domain Service - Business Logic
public class OrderManager : DomainService
{
    public void PlaceOrder(Order order, Customer customer)
    {
        // Business Rule: التحقق من رصيد العميل
        if (customer.Balance < order.TotalPrice)
            throw new InsufficientBalanceException();
            
        // Business Rule: خصم من الرصيد
        customer.DeductBalance(order.TotalPrice);
        
        // Business Rule: تأكيد الطلب
        order.Confirm();
    }
}

// Application Service - Use Case
public class OrderAppService : ApplicationService
{
    private readonly OrderManager _orderManager;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Customer> _customerRepository;
    
    public async Task<OrderDto> PlaceOrderAsync(PlaceOrderInput input)
    {
        // 1. جلب البيانات
        var customer = await _customerRepository.GetAsync(input.CustomerId);
        var order = await _orderRepository.GetAsync(input.OrderId);
        
        // 2. تنفيذ Business Logic (عبر Domain Service)
        _orderManager.PlaceOrder(order, customer);
        
        // 3. حفظ التغييرات
        await _customerRepository.UpdateAsync(customer);
        await _orderRepository.UpdateAsync(order);
        
        // 4. تحويل لـ DTO وإرجاع
        return ObjectMapper.Map<Order, OrderDto>(order);
    }
}
```

---

### 5️⃣ Repository (المستودع)

**التعريف:**
واجهة (Interface) للوصول إلى البيانات **بدون معرفة تفاصيل قاعدة البيانات**.

**الفكرة:**
بدل ما تكتب SQL أو LINQ في كل مكان، تستخدم Repository اللي بيوفر methods جاهزة.

**في ABP:**

```csharp
// ABP توفر IRepository<TEntity, TKey> جاهز
public class ProductAppService : ApplicationService
{
    private readonly IRepository<Product, Guid> _productRepository;
    
    public ProductAppService(IRepository<Product, Guid> productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<ProductDto> GetAsync(Guid id)
    {
        // لا نكتب SQL - نستخدم Repository
        var product = await _productRepository.GetAsync(id);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }
    
    public async Task<List<ProductDto>> GetListAsync()
    {
        var products = await _productRepository.GetListAsync();
        return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
    }
}
```

**Methods الجاهزة في IRepository:**

```csharp
// Create
await repository.InsertAsync(entity);

// Read
var entity = await repository.GetAsync(id);
var list = await repository.GetListAsync();
var entity = await repository.FirstOrDefaultAsync(x => x.Name == "Test");

// Update
await repository.UpdateAsync(entity);

// Delete
await repository.DeleteAsync(id);
await repository.DeleteAsync(entity);

// Query
var query = await repository.GetQueryableAsync();
var filtered = query.Where(x => x.Price > 100).ToList();

// Count
var count = await repository.CountAsync();
var count = await repository.CountAsync(x => x.IsActive);
```

**Custom Repository:**

لو محتاج methods معقدة:

```csharp
// 1. تعريف Interface في Domain
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<List<Product>> GetExpensiveProductsAsync(decimal minPrice);
    Task<Product> GetMostPopularProductAsync();
}

// 2. التنفيذ في EntityFrameworkCore
public class ProductRepository : EfCoreRepository<MyDbContext, Product, Guid>, IProductRepository
{
    public ProductRepository(IDbContextProvider<MyDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
    
    public async Task<List<Product>> GetExpensiveProductsAsync(decimal minPrice)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Price >= minPrice)
            .OrderByDescending(x => x.Price)
            .ToListAsync();
    }
    
    public async Task<Product> GetMostPopularProductAsync()
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .OrderByDescending(x => x.SalesCount)
            .FirstOrDefaultAsync();
    }
}
```

**الفوائد:**
- ✅ **فصل** Business Logic عن Database Logic
- ✅ **سهولة الاختبار** - ممكن تعمل Mock للـ Repository
- ✅ **إعادة الاستخدام** - نفس الـ Methods في أكثر من مكان
- ✅ **تغيير قاعدة البيانات سهل** - غير التنفيذ فقط

---

## 📊 ملخص المفاهيم

| المفهوم | التعريف | مثال | مكانه |
|---------|---------|------|-------|
| **Entity** | كائن له هوية فريدة | Product, Order, Customer | Domain |
| **Aggregate Root** | Entity رئيسي يدير كيانات أخرى | Order (يدير OrderItems) | Domain |
| **Value Object** | كائن معرف بقيمته | Address, Money, DateRange | Domain |
| **Domain Service** | Business Logic لا ينتمي لـ Entity واحد | OrderManager, ProductManager | Domain |
| **Repository** | واجهة الوصول للبيانات | IRepository<Product> | Domain (Interface) / Infrastructure (Implementation) |

---

## 🎯 الخلاصة

**DDD يساعدك في:**
1. تنظيم الكود حول **Business Logic**
2. حماية **Business Rules** من الانتهاك
3. **عزل** التفاصيل التقنية عن المنطق التجاري
4. جعل الكود **قابل للصيانة والتوسع**

---

## ✅ المرحلة 1.2 مكتملة

دلوقتي فهمنا:
- ✅ Entity و هويته الفريدة
- ✅ Aggregate & Aggregate Root ودورهم في الحفاظ على التناسق
- ✅ Value Object والفرق بينه وبين Entity
- ✅ Domain Service ومتى نستخدمه
- ✅ Repository وكيف نتعامل مع البيانات

**الخطوة التالية:** المرحلة 1.3 - تجهيز البيئة وتشغيل المشروع! 🚀
