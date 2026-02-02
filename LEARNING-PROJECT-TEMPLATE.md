# 🎓 Learning Project Template - قالب المشاريع التعليمية

> **استخدم هذا الملف كـ Script لأي مشروع تعليمي جديد**
> 
> **فقط غير:** Framework/Language المطلوب
> 
> **الباقي:** سيتبع نفس المنهجية بالضبط

---

## 📝 Instructions للـ AI Assistant

عند استلام هذا الملف، اتبع التالي:

1. **اقرأ هذا الملف بالكامل** لفهم المنهجية
2. **استخرج** الـ Framework/Language المطلوب من الـ Configuration أدناه
3. **طبق** نفس المراحل والأسلوب على الـ Framework الجديد
4. **أنشئ** نفس هيكل التوثيق والملفات
5. **التزم** بنفس معايير الجودة

---

## ⚙️ Project Configuration

```yaml
# 🔧 Configuration - غير هذا القسم فقط
project:
  framework: "ABP.io"              # 👈 غير هنا: Laravel, NestJS, Django, Spring Boot, etc.
  language: "C#"                   # 👈 غير هنا: PHP, TypeScript, Python, Java, etc.
  type: "Backend API"              # Backend API, Full-stack, Microservices, etc.
  database: "SQL Server"           # MySQL, PostgreSQL, MongoDB, etc.
  
# ✅ الباقي - لا تغير
methodology:
  approach: "Step-by-step incremental learning"
  documentation: "Comprehensive with examples"
  testing: "Unit + Integration tests"
  quality: "Production-ready Enterprise-grade"
  language: "Arabic + English (mixed)"
```

---

## 🎯 Learning Methodology - المنهجية التعليمية

### **الأسلوب:**
- ✅ **تعليم تدريجي** - من الأساسيات للـ Advanced
- ✅ **تطبيق عملي** - كل مرحلة بها كود فعلي
- ✅ **توثيق شامل** - دليل مفصل لكل موضوع
- ✅ **اختبارات** - تأكد من الفهم والجودة
- ✅ **Production-Ready** - المشروع النهائي جاهز للاستخدام الفعلي

### **اللغة:**
- استخدم **العربية** للشرح والتوضيح
- استخدم **الإنجليزية** للمصطلحات التقنية
- اكتب الكود بـ **Best Practices** للـ Framework المحدد

---

## 📚 المراحل التعليمية - 10 Stages

### **المرحلة 1: Setup & Architecture (5%)**
**الهدف:** فهم البنية + الإعداد الأولي

**المخرجات:**
1. دليل شرح البنية (`01-architecture-explained.md`)
2. دليل الإعداد والتشغيل (`02-setup-guide.md`)
3. المشروع يعمل بنجاح
4. فهم الـ Folder Structure

**Checklist:**
- [ ] تثبيت الـ Prerequisites
- [ ] إنشاء المشروع
- [ ] تشغيل المشروع
- [ ] فهم البنية الأساسية
- [ ] توثيق كل الخطوات في `commands-log.txt`

---

### **المرحلة 2: Core Entities & Domain Layer (5%)**
**الهدف:** فهم Domain Layer وإنشاء Entity أساسي

**المخرجات:**
1. Entity رئيسي (مثل: Product, User, Post)
2. Validation Rules في Domain
3. Business Rules
4. دليل شامل (`03-domain-layer-guide.md`)

**Checklist:**
- [ ] إنشاء Entity بـ Properties مناسبة
- [ ] إضافة Validation في الـ Entity
- [ ] إضافة Business Logic Methods
- [ ] اختبار الـ Entity
- [ ] توثيق الـ Entity

**مثال:**
```
Product Entity:
- Properties: Name, Price, Stock, Description
- Validation: Name required, Price > 0, Stock >= 0
- Methods: IncreaseStock(), DecreaseStock()
```

---

### **المرحلة 3: Database & ORM (5%)**
**الهدف:** ربط الـ Database وإنشاء Relationships

**المخرجات:**
1. Database Configuration
2. ORM Setup (EF Core, Eloquent, TypeORM, etc.)
3. Migrations
4. Relationships (One-to-Many, Many-to-Many)
5. دليل شامل (`04-database-guide.md`)

**Checklist:**
- [ ] تكوين الـ Database Connection
- [ ] إنشاء DbContext/Model
- [ ] إنشاء Migration
- [ ] إضافة Relationships
- [ ] إضافة Seed Data
- [ ] اختبار الـ Database

**Relationships Example:**
```
Category (1) → (Many) Products
User (1) → (Many) Orders
```

---

### **المرحلة 4: Application/Service Layer (5%)**
**الهدف:** إنشاء Business Logic Layer

**المخرجات:**
1. Service Classes
2. DTOs (Data Transfer Objects)
3. AutoMapper/Mapping
4. CRUD Operations
5. دليل شامل (`05-service-layer-guide.md`)

**Checklist:**
- [ ] إنشاء Service Interface
- [ ] إنشاء Service Implementation
- [ ] إنشاء DTOs (Create, Update, Get)
- [ ] تكوين AutoMapper
- [ ] CRUD Methods (Create, Read, Update, Delete)
- [ ] Business Validation

**مثال:**
```
ProductService:
- CreateAsync(CreateProductDto)
- UpdateAsync(id, UpdateProductDto)
- DeleteAsync(id)
- GetAsync(id)
- GetListAsync()
```

---

### **المرحلة 5: API/Controllers Layer (5%)**
**الهدف:** إنشاء RESTful API

**المخرجات:**
1. API Controllers
2. HTTP Endpoints
3. Request/Response handling
4. API Testing (Postman Collection)
5. دليل شامل (`06-api-guide.md`)

**Checklist:**
- [ ] إنشاء Controller
- [ ] إضافة Endpoints (GET, POST, PUT, DELETE)
- [ ] تكوين Routing
- [ ] إضافة Response Codes
- [ ] إنشاء Postman Collection
- [ ] اختبار كل الـ Endpoints

**API Endpoints:**
```
GET    /api/products       - List all
GET    /api/products/{id}  - Get by ID
POST   /api/products       - Create
PUT    /api/products/{id}  - Update
DELETE /api/products/{id}  - Delete
```

---

### **المرحلة 6: Authentication & Authorization (10%)**
**الهدف:** تأمين الـ API

**المخرجات:**
1. Authentication System (JWT, OAuth, etc.)
2. Authorization (Roles/Permissions)
3. Protected Endpoints
4. دليل شامل (`07-auth-guide.md`)

**Checklist:**
- [ ] تكوين Authentication
- [ ] إضافة Login/Register
- [ ] إصدار Tokens
- [ ] إضافة Authorization Attributes
- [ ] Roles/Permissions System
- [ ] اختبار الـ Authentication

**مثال:**
```
Permissions:
- Products.Create
- Products.Edit
- Products.Delete
- Products.View
```

---

### **المرحلة 7: Advanced Features (40%)**
**الهدف:** إضافة Features متقدمة

#### **7.1 Validation (8%)**
**المخرجات:**
1. Multi-layer Validation
2. Custom Validators
3. Error Messages
4. دليل + سيناريوهات (`08-validation-guide.md`, `validation-scenarios.md`)

**Checklist:**
- [ ] Data Annotations
- [ ] FluentValidation (أو ما يعادله)
- [ ] Custom Validators
- [ ] Error Handling
- [ ] اختبار 15+ سيناريو

---

#### **7.2 Exception Handling (8%)**
**المخرجات:**
1. Global Exception Handler
2. Custom Exceptions
3. Error Codes
4. دليل + سيناريوهات (`09-exception-handling-guide.md`, `exception-scenarios.md`)

**Checklist:**
- [ ] Global Exception Middleware
- [ ] Custom Exception Classes
- [ ] Error Code System
- [ ] User-friendly Messages
- [ ] اختبار 10+ سيناريو

---

#### **7.3 Localization (8%)**
**المخرجات:**
1. Multi-language Support
2. Resource Files
3. Localized Messages
4. دليل + سيناريوهات (`10-localization-guide.md`, `localization-scenarios.md`)

**Checklist:**
- [ ] تكوين Localization
- [ ] Resource Files (en.json, ar.json)
- [ ] Localized Validation Messages
- [ ] Localized Exception Messages
- [ ] اختبار بلغتين على الأقل

---

#### **7.4 Background Jobs (8%)**
**المخرجات:**
1. Background Job System
2. Periodic Workers
3. Job Processing
4. دليل + اختبار (`11-background-jobs-guide.md`, `jobs-testing.md`)

**Checklist:**
- [ ] تكوين Background Jobs
- [ ] إنشاء Job Class
- [ ] إنشاء Worker Class
- [ ] Queue Management
- [ ] اختبار Jobs

**مثال:**
```
Jobs:
- SendEmailJob
- GenerateReportJob
- CleanupJob (Worker - every hour)
```

---

#### **7.5 Events (8%)**
**المخرجات:**
1. Event System
2. Event Handlers
3. Event-driven Architecture
4. دليل + اختبار (`12-events-guide.md`, `events-testing.md`)

**Checklist:**
- [ ] إنشاء Events
- [ ] إنشاء Event Handlers
- [ ] Event Publishing
- [ ] Event Subscription
- [ ] اختبار Events

**مثال:**
```
Events:
- ProductCreatedEvent → Log, SendNotification
- OrderPlacedEvent → UpdateInventory, SendEmail
```

---

### **المرحلة 8: Testing (9%)**
**الهدف:** اختبارات شاملة

**المخرجات:**
1. Unit Tests
2. Integration Tests
3. Test Coverage
4. دليل شامل (`13-testing-guide.md`)

**Checklist:**
- [ ] Unit Tests للـ Domain Layer (10+ tests)
- [ ] Unit Tests للـ Application Layer (15+ tests)
- [ ] Integration Tests للـ API (10+ tests)
- [ ] Event/Job Tests (5+ tests)
- [ ] **الهدف:** 35+ test على الأقل

**Testing Frameworks:**
- C#: xUnit, NUnit
- PHP: PHPUnit
- TypeScript: Jest
- Python: pytest
- Java: JUnit

---

### **المرحلة 9: Best Practices (8%)**
**الهدف:** تطبيق أفضل الممارسات

**المخرجات:**
1. SOLID Principles
2. Design Patterns
3. Specification/Query Pattern
4. Clean Code
5. دليل شامل (`14-best-practices-guide.md`)

**Checklist:**
- [ ] فهم وتطبيق SOLID Principles (5 principles)
- [ ] استخدام Design Patterns (Repository, Factory, Strategy, etc.)
- [ ] إنشاء Specification Pattern (5+ specifications)
- [ ] Clean Code Practices
- [ ] Code Review

**SOLID:**
- **S**ingle Responsibility
- **O**pen/Closed
- **L**iskov Substitution
- **I**nterface Segregation
- **D**ependency Inversion

**Design Patterns:**
- Repository Pattern
- Unit of Work Pattern
- Factory Pattern
- Strategy Pattern
- Specification Pattern

---

### **المرحلة 10: Deployment (8%)**
**الهدف:** نشر للـ Production

**المخرجات:**
1. Production Configuration
2. Deployment Options
3. CI/CD Pipeline
4. Monitoring & Logging
5. دليل شامل (`15-deployment-guide.md`)

**Checklist:**
- [ ] Production Configuration
- [ ] Build & Publish
- [ ] Database Migration Strategy
- [ ] 4 Deployment Options:
  - [ ] Cloud (AWS, Azure, GCP)
  - [ ] Docker
  - [ ] Traditional Server
  - [ ] Platform-specific (Heroku, Vercel, etc.)
- [ ] Logging & Monitoring
- [ ] Backup Strategy
- [ ] CI/CD Pipeline (GitHub Actions)

---

## 📝 Documentation Standards - معايير التوثيق

### **لكل مرحلة أنشئ:**

#### **1. Comprehensive Guide (دليل شامل):**
```markdown
Structure:
1. نظرة عامة (Overview)
2. المفاهيم الأساسية (Concepts)
3. التطبيق العملي (Implementation)
4. أمثلة كاملة (Examples)
5. Best Practices
6. Common Issues & Solutions
7. الخلاصة (Summary)
```

#### **2. Test Scenarios (للـ Advanced Features):**
```markdown
Structure:
1. قائمة بـ Scenarios (10-20 scenario)
2. خطوات الاختبار لكل scenario
3. Expected Results
4. Actual Results
5. Pass/Fail Status
```

#### **3. Commands Log:**
- سجل **كل** أمر يتم تنفيذه
- مع التاريخ والوقت
- مع الناتج (Success/Error)

#### **4. Learning Roadmap:**
- تتبع Progress لكل مرحلة
- Checklist لكل feature
- نسبة الإنجاز
- قائمة الملفات التوثيقية

---

## 🎯 Quality Standards - معايير الجودة

### **Code Quality:**
- ✅ **Clean Code** - meaningful names, small functions
- ✅ **SOLID Principles** - applied throughout
- ✅ **Design Patterns** - where appropriate
- ✅ **Comments** - only where necessary (code should be self-documenting)
- ✅ **Formatting** - consistent style

### **Testing:**
- ✅ **Minimum 35 tests** - Unit + Integration
- ✅ **Test Coverage** - aim for 70%+ coverage
- ✅ **AAA Pattern** - Arrange, Act, Assert
- ✅ **Meaningful test names** - should describe what's being tested

### **Documentation:**
- ✅ **15+ comprehensive guides**
- ✅ **5+ test scenario files**
- ✅ **Complete commands log**
- ✅ **Postman Collection** - for API testing
- ✅ **README** - clear setup instructions

### **Build:**
- ✅ **0 Errors**
- ✅ **0 Warnings**
- ✅ **All tests passing**
- ✅ **Production-ready**

---

## 🏗️ Project Structure - الهيكل المتوقع

```
project-root/
├── src/                          # Source code
│   ├── Domain/                   # Domain Layer
│   ├── Application/              # Service Layer
│   ├── Infrastructure/           # Data Access
│   └── API/                      # Controllers/Endpoints
│
├── test/                         # Tests
│   ├── Domain.Tests/
│   ├── Application.Tests/
│   └── Integration.Tests/
│
├── docs/                         # Documentation
│   ├── 01-architecture-explained.md
│   ├── 02-setup-guide.md
│   ├── 03-domain-layer-guide.md
│   ├── ...
│   ├── 15-deployment-guide.md
│   ├── validation-scenarios.md
│   ├── exception-scenarios.md
│   └── ...
│
├── commands-log.txt              # All commands executed
├── learning-roadmap.md           # Progress tracking
├── PROJECT-SUMMARY.md            # Final summary
└── README.md                     # Quick start guide
```

---

## 📊 Success Metrics - مؤشرات النجاح

### **بنهاية المشروع يجب تحقيق:**

| Metric | Target | Status |
|--------|--------|--------|
| **Stages Completed** | 10/10 | ✅ |
| **Documentation Files** | 25+ files | ✅ |
| **Tests Written** | 35+ tests | ✅ |
| **Build Status** | 0 errors, 0 warnings | ✅ |
| **Features** | All 10 stages | ✅ |
| **Deployment Options** | 4 options | ✅ |
| **Code Quality** | SOLID + Patterns | ✅ |
| **Production Ready** | Yes | ✅ |

---

## 🎓 Example Use Cases - أمثلة للاستخدام

### **مثال 1: Laravel Project**
```yaml
project:
  framework: "Laravel"
  language: "PHP"
  type: "Backend API"
  database: "MySQL"
```

**سيتم إنشاء:**
- Product Model with Eloquent
- ProductController with Resource Routes
- ProductService for Business Logic
- Validation with FormRequest
- Authorization with Gates/Policies
- Background Jobs with Queue
- Events & Listeners
- PHPUnit Tests
- Deployment to Laravel Forge/AWS

---

### **مثال 2: NestJS Project**
```yaml
project:
  framework: "NestJS"
  language: "TypeScript"
  type: "Backend API"
  database: "PostgreSQL"
```

**سيتم إنشاء:**
- Product Entity with TypeORM
- ProductController with Decorators
- ProductService with Dependency Injection
- Validation with class-validator
- Guards for Authorization
- Bull Queue for Background Jobs
- Event Emitter
- Jest Tests
- Deployment to AWS/Heroku

---

### **مثال 3: Django REST Framework**
```yaml
project:
  framework: "Django REST Framework"
  language: "Python"
  type: "Backend API"
  database: "PostgreSQL"
```

**سيتم إنشاء:**
- Product Model with Django ORM
- ProductViewSet with DRF
- ProductSerializer
- Validation with Serializers
- Permissions Classes
- Celery for Background Tasks
- Django Signals
- pytest Tests
- Deployment to AWS/DigitalOcean

---

## 🔄 Workflow - سير العمل

### **لكل مرحلة:**

```
1. 📖 قراءة المتطلبات
   ↓
2. 📝 إنشاء الدليل التوثيقي
   ↓
3. 💻 تطبيق الكود العملي
   ↓
4. ✅ كتابة الاختبارات
   ↓
5. 🔨 Build & Test
   ↓
6. 📋 تحديث Learning Roadmap
   ↓
7. 📝 تحديث Commands Log
   ↓
8. ✅ Mark as Complete
```

---

## 🎯 Final Deliverables - المخرجات النهائية

### **عند انتهاء المشروع:**

1. ✅ **Working Application** - تطبيق كامل يعمل
2. ✅ **25+ Documentation Files** - توثيق شامل
3. ✅ **35+ Tests** - اختبارات شاملة
4. ✅ **Postman Collection** - اختبار API
5. ✅ **Commands Log** - سجل كامل
6. ✅ **Learning Roadmap** - تتبع الإنجاز
7. ✅ **PROJECT-SUMMARY.md** - ملخص نهائي
8. ✅ **Production Config** - جاهز للنشر
9. ✅ **4 Deployment Options** - طرق نشر متعددة
10. ✅ **CI/CD Pipeline** - أتمتة النشر

---

## 📞 Communication Style - أسلوب التواصل

### **عند تطبيق هذا Template:**

1. **استخدم العربية** للشرح والتوضيح
2. **استخدم الإنجليزية** للمصطلحات التقنية وأسماء Classes/Methods
3. **كن مفصلاً** في الشرح
4. **أضف أمثلة عملية** لكل مفهوم
5. **اشرح "لماذا"** وليس "كيف" فقط
6. **أضف Best Practices** في كل مرحلة
7. **تأكد من Build Success** بعد كل مرحلة
8. **وثق كل شيء** - لا تترك شيء بدون توثيق

---

## ⚡ Key Success Factors - عوامل النجاح

### **لضمان نجاح المشروع:**

1. ✅ **التدرج** - من البسيط للمعقد
2. ✅ **التطبيق العملي** - كود فعلي وليس نظري فقط
3. ✅ **التوثيق الشامل** - اكتب كل شيء
4. ✅ **الاختبارات** - test everything
5. ✅ **الجودة** - production-ready code
6. ✅ **Best Practices** - SOLID + Patterns
7. ✅ **التنوع** - 4 deployment options
8. ✅ **الكمال** - 0 warnings, 0 errors

---

## 🎓 Learning Outcomes - المخرجات التعليمية

### **بنهاية المشروع، يجب أن يكون لديك:**

#### **Technical Skills:**
- ✅ إتقان الـ Framework المختار
- ✅ فهم عميق للـ Architecture Patterns
- ✅ معرفة بـ SOLID Principles
- ✅ خبرة في Design Patterns
- ✅ مهارات Testing
- ✅ خبرة Deployment

#### **Soft Skills:**
- ✅ Problem Solving
- ✅ Documentation Writing
- ✅ Code Organization
- ✅ Best Practices Application

#### **Deliverables:**
- ✅ مشروع كامل Production-Ready
- ✅ Portfolio piece
- ✅ Reference للمشاريع المستقبلية
- ✅ 25+ Documentation files
- ✅ 35+ Tests

---

## 🚀 How to Use This Template - كيفية الاستخدام

### **للـ User (أنت):**

1. **انسخ** هذا الملف لمجلد المشروع الجديد
2. **عدل** الـ Configuration section:
   - `framework`: ضع الـ Framework الجديد
   - `language`: ضع اللغة الجديدة
   - `database`: ضع قاعدة البيانات
3. **أرسل** الملف كاملاً للـ AI في محادثة جديدة
4. **قل:** "اتبع هذا Template لمشروع [Framework Name]"

### **للـ AI Assistant (أنا):**

1. **اقرأ** الملف كاملاً
2. **استخرج** الـ Configuration
3. **طبق** نفس المراحل العشرة
4. **اتبع** نفس معايير التوثيق
5. **التزم** بنفس معايير الجودة
6. **أنشئ** نفس Structure
7. **احرص** على 0 warnings, 0 errors
8. **تأكد** من Production-ready

---

## 📋 Checklist Template - قائمة التحقق

### **نسخ واستخدم هذا لكل مشروع:**

```markdown
## Project Progress

- [ ] Stage 1: Setup & Architecture (5%)
  - [ ] Guide written
  - [ ] Project running
  - [ ] Commands logged

- [ ] Stage 2: Domain Layer (5%)
  - [ ] Entity created
  - [ ] Validation added
  - [ ] Guide written

- [ ] Stage 3: Database (5%)
  - [ ] DB configured
  - [ ] Migrations created
  - [ ] Relationships added
  - [ ] Guide written

- [ ] Stage 4: Service Layer (5%)
  - [ ] Services created
  - [ ] DTOs created
  - [ ] CRUD implemented
  - [ ] Guide written

- [ ] Stage 5: API Layer (5%)
  - [ ] Controllers created
  - [ ] Endpoints tested
  - [ ] Postman collection
  - [ ] Guide written

- [ ] Stage 6: Auth (10%)
  - [ ] Authentication working
  - [ ] Authorization working
  - [ ] Permissions system
  - [ ] Guide written

- [ ] Stage 7: Advanced Features (40%)
  - [ ] 7.1: Validation (8%)
  - [ ] 7.2: Exceptions (8%)
  - [ ] 7.3: Localization (8%)
  - [ ] 7.4: Background Jobs (8%)
  - [ ] 7.5: Events (8%)

- [ ] Stage 8: Testing (9%)
  - [ ] 10+ Domain tests
  - [ ] 15+ Application tests
  - [ ] 10+ Integration tests
  - [ ] Guide written

- [ ] Stage 9: Best Practices (8%)
  - [ ] SOLID applied
  - [ ] Patterns implemented
  - [ ] Specifications created
  - [ ] Guide written

- [ ] Stage 10: Deployment (8%)
  - [ ] Production config
  - [ ] 4 deployment options
  - [ ] CI/CD pipeline
  - [ ] Guide written

## Quality Metrics
- [ ] 0 Build errors
- [ ] 0 Build warnings
- [ ] 35+ Tests written
- [ ] All tests passing
- [ ] 25+ Documentation files
- [ ] Commands log complete
- [ ] PROJECT-SUMMARY.md written
```

---

## 🎉 Final Notes - ملاحظات نهائية

### **تذكر:**

1. ✅ **الجودة > السرعة** - خذ وقتك
2. ✅ **التوثيق مهم** - وثق كل شيء
3. ✅ **الاختبارات ضرورية** - اختبر كل شيء
4. ✅ **Best Practices** - طبقها دائماً
5. ✅ **Production-Ready** - هذا هو الهدف
6. ✅ **0 Warnings** - نظف الكود
7. ✅ **اتبع المراحل** - لا تتخطى مرحلة
8. ✅ **استمتع** - التعلم رحلة ممتعة!

---

## 📖 Example Message to AI

```markdown
مرحباً! 👋

أريد إنشاء مشروع تعليمي باستخدام هذا Template:

[الصق محتوى هذا الملف هنا]

Configuration المطلوب:
- Framework: NestJS
- Language: TypeScript
- Database: PostgreSQL

من فضلك اتبع نفس المنهجية بالضبط كما في المشروع السابق (ABP.io).

ابدأ من المرحلة 1: Setup & Architecture
```

---

**🎓 Template Version:** 1.0  
**📅 Created:** Based on Andro.Backend.Reference project  
**✅ Status:** Ready to use  
**🎯 Purpose:** Standardized learning approach for any framework  

---

**🚀 Happy Learning! 🚀**
