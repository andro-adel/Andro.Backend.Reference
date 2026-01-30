# ⚙️ المرحلة 1.3: تجهيز البيئة والتشغيل الأول

---

## 📋 نظرة عامة

في هذه المرحلة سنقوم بـ:
1. فهم ملفات `appsettings.json` والـ Connection String
2. تعديل Connection String (إذا لزم الأمر)
3. تشغيل **DbMigrator** لإنشاء قاعدة البيانات
4. فحص قاعدة البيانات والجداول المنشأة
5. تشغيل المشروع واختبار الـ APIs

---

## 📂 ملفات appsettings.json

في مشروعنا يوجد ملفين `appsettings.json` رئيسيين:

### 1️⃣ DbMigrator/appsettings.json

**المسار:** `src/Andro.Backend.Reference.DbMigrator/appsettings.json`

```json
{
  "ConnectionStrings": {
    "Default": "Server=(LocalDb)\\MSSQLLocalDB;Database=Reference;Trusted_Connection=True;TrustServerCertificate=true"
  },
  "OpenIddict": {
    "Applications": {
      "Reference_App": {
        "ClientId": "Reference_App"
      },
      "Reference_Swagger": {
        "ClientId": "Reference_Swagger",
        "RootUrl": "https://localhost:44309/"
      }
    }
  }
}
```

**الشرح:**
- **ConnectionStrings.Default**: اتصال قاعدة البيانات
- **OpenIddict.Applications**: تطبيقات OAuth2/OpenID Connect (للمصادقة)
  - `Reference_App`: تطبيق عادي
  - `Reference_Swagger`: تطبيق Swagger UI

---

### 2️⃣ Web/appsettings.json

**المسار:** `src/Andro.Backend.Reference.Web/appsettings.json`

```json
{
  "App": {
    "SelfUrl": "https://localhost:44309",
    "HealthCheckUrl": "/health-status"
  },
  "ConnectionStrings": {
    "Default": "Server=(LocalDb)\\MSSQLLocalDB;Database=Reference;Trusted_Connection=True;TrustServerCertificate=true"
  },
  "AuthServer": {
    "Authority": "https://localhost:44309",
    "RequireHttpsMetadata": true,
    "CertificatePassPhrase": "9bdd3596-d1df-4707-baec-1882f1d3fae2"
  },
  "StringEncryption": {
    "DefaultPassPhrase": "HQBuz03TmXIpIQMK"
  }
}
```

**الشرح:**
- **App.SelfUrl**: عنوان التطبيق
- **ConnectionStrings**: نفس الاتصال بقاعدة البيانات
- **AuthServer**: إعدادات خادم المصادقة
- **StringEncryption**: مفتاح تشفير النصوص الحساسة

---

## 🔌 فهم Connection String

**Connection String الحالي:**
```
Server=(LocalDb)\\MSSQLLocalDB;Database=Reference;Trusted_Connection=True;TrustServerCertificate=true
```

### تحليل الأجزاء:

| الجزء | الشرح | القيمة الحالية |
|-------|-------|----------------|
| **Server** | اسم السيرفر | `(LocalDb)\\MSSQLLocalDB` |
| **Database** | اسم قاعدة البيانات | `Reference` |
| **Trusted_Connection** | استخدام Windows Authentication | `True` |
| **TrustServerCertificate** | الثقة في شهادة SSL | `true` |

---

## 🎯 خيارات Connection String المختلفة

### 1️⃣ SQL Server LocalDB (الافتراضي) ✅

**مناسب للتطوير المحلي**

```json
"Default": "Server=(LocalDb)\\MSSQLLocalDB;Database=Reference;Trusted_Connection=True;TrustServerCertificate=true"
```

**المتطلبات:**
- SQL Server Express LocalDB مثبت (يأتي مع Visual Studio)

**المميزات:**
- ✅ سهل للتطوير
- ✅ لا يحتاج إعداد معقد
- ✅ قاعدة بيانات خفيفة

---

### 2️⃣ SQL Server Express/Standard (مع Windows Authentication)

```json
"Default": "Server=localhost;Database=Reference;Trusted_Connection=True;TrustServerCertificate=true"
```

أو

```json
"Default": "Server=.;Database=Reference;Trusted_Connection=True;TrustServerCertificate=true"
```

**استخدم هذا إذا:**
- عندك SQL Server Express أو Standard مثبت
- تستخدم Windows Authentication

---

### 3️⃣ SQL Server (مع SQL Authentication)

```json
"Default": "Server=localhost;Database=Reference;User Id=sa;Password=YourPassword123;TrustServerCertificate=true"
```

**استخدم هذا إذا:**
- تستخدم SQL Authentication
- عندك username و password للـ SQL Server

⚠️ **تحذير:** لا تحط الـ Password في الكود! استخدم User Secrets أو Environment Variables في Production.

---

### 4️⃣ SQL Server على Azure

```json
"Default": "Server=tcp:yourserver.database.windows.net,1433;Database=Reference;User Id=yourusername;Password=yourpassword;Encrypt=true;"
```

---

### 5️⃣ PostgreSQL

```json
"Default": "Host=localhost;Database=Reference;Username=postgres;Password=yourpassword"
```

**ملاحظة:** محتاج تغيير Database Provider في الكود من SQL Server لـ PostgreSQL.

---

### 6️⃣ MySQL

```json
"Default": "Server=localhost;Database=Reference;Uid=root;Pwd=yourpassword;"
```

**ملاحظة:** محتاج تغيير Database Provider في الكود من SQL Server لـ MySQL.

---

## 🔧 كيف تعدل Connection String؟

### الخطوات:

1️⃣ **حدد نوع قاعدة البيانات اللي عندك**
   - LocalDB؟ (الافتراضي)
   - SQL Server Express؟
   - SQL Server كامل؟
   - غير ذلك؟

2️⃣ **اختر الـ Connection String المناسب** من الأمثلة أعلاه

3️⃣ **عدل في ملفين:**
   - `src/Andro.Backend.Reference.DbMigrator/appsettings.json`
   - `src/Andro.Backend.Reference.Web/appsettings.json`

4️⃣ **تأكد إن اسم قاعدة البيانات مناسب**
   - القيمة الافتراضية: `Reference`
   - ممكن تغيرها لـ: `Andro.Backend.Reference` أو أي اسم تانى

---

## 🏃‍♂️ تشغيل DbMigrator

**DbMigrator** هو Console Application بيعمل:
1. إنشاء قاعدة البيانات (إذا لم تكن موجودة)
2. تطبيق كل الـ **Migrations** (التحديثات على بنية قاعدة البيانات)
3. إدخال **Initial Data** (بيانات أولية مثل Admin User)

### طريقتان للتشغيل:

#### الطريقة 1️⃣: من Visual Studio (الأسهل)

1. اضغط Right Click على مشروع `Andro.Backend.Reference.DbMigrator`
2. اختر **Set as Startup Project**
3. اضغط `F5` أو `Ctrl+F5`

---

#### الطريقة 2️⃣: من PowerShell (الموصى بها)

**الأمر:**
```powershell
cd "src/Andro.Backend.Reference.DbMigrator"
dotnet run
```

**ما يحدث:**
```
[12:00:00 INF] Started database migrations...
[12:00:01 INF] Migrating database schema...
[12:00:02 INF] Executing DbMigrator...
[12:00:03 INF] Seeding initial data...
[12:00:04 INF] Successfully completed database migrations.
```

**علامات النجاح:**
- ✅ `Successfully completed database migrations`
- ✅ لا توجد أخطاء حمراء
- ✅ البرنامج ينتهي تلقائياً

---

## 🗄️ التحقق من قاعدة البيانات

بعد تشغيل DbMigrator، افتح SQL Server Management Studio (SSMS) أو أي أداة:

### الجداول الرئيسية المتوقعة:

#### 1️⃣ **Identity & User Management**
- `AbpUsers` - المستخدمين
- `AbpRoles` - الأدوار (Admin, User, etc.)
- `AbpUserRoles` - ربط المستخدمين بالأدوار
- `AbpUserClaims` - معلومات إضافية عن المستخدمين
- `AbpRoleClaims` - صلاحيات الأدوار

#### 2️⃣ **Authentication (OpenIddict)**
- `OpenIddictApplications` - التطبيقات المسموح لها بالوصول
- `OpenIddictAuthorizations` - التراخيص
- `OpenIddictTokens` - التوكنات (Access Tokens, Refresh Tokens)
- `OpenIddictScopes` - الأذونات (Scopes)

#### 3️⃣ **Permissions**
- `AbpPermissionGrants` - الصلاحيات الممنوحة للمستخدمين/الأدوار

#### 4️⃣ **Audit Logging**
- `AbpAuditLogs` - سجل كل العمليات
- `AbpAuditLogActions` - تفاصيل الإجراءات

#### 5️⃣ **Settings**
- `AbpSettings` - الإعدادات

#### 6️⃣ **Tenants (Multi-Tenancy)**
- `AbpTenants` - العملاء (إذا كان Multi-Tenancy مفعل)

#### 7️⃣ **Features**
- `AbpFeatures` - الخصائص المفعلة

#### 8️⃣ **Background Jobs**
- `AbpBackgroundJobs` - المهام الخلفية

---

## 👤 البيانات الأولية (Initial Data)

DbMigrator بيضيف مستخدم Admin افتراضي:

**البيانات:**
```
Username: admin
Email: admin@abp.io
Password: 1q2w3E*
```

⚠️ **مهم جداً:** غير الـ Password ده في Production!

---

## 🚀 تشغيل المشروع (Web Application)

بعد ما قاعدة البيانات جاهزة، نشغل المشروع:

### من PowerShell:

```powershell
cd "src/Andro.Backend.Reference.Web"
dotnet run
```

أو من Visual Studio:
- Set `Andro.Backend.Reference.Web` as Startup Project
- اضغط `F5`

---

## 📡 الوصول للتطبيق

بعد التشغيل، افتح المتصفح على:

### 1️⃣ الصفحة الرئيسية
```
https://localhost:44309
```

### 2️⃣ Swagger UI (API Documentation)
```
https://localhost:44309/swagger
```

**Swagger UI** بيعرض كل الـ APIs الجاهزة ويسمح لك باختبارها مباشرة!

---

## 🧪 اختبار APIs عبر Swagger

### الخطوات:

1️⃣ **افتح Swagger UI:**
   - `https://localhost:44309/swagger`

2️⃣ **ستجد APIs جاهزة:**
   - **Account** - تسجيل دخول، تسجيل
   - **Profile** - بروفايل المستخدم
   - **AbpApplicationConfiguration** - إعدادات التطبيق
   - **AbpTenant** - Tenants (إذا مفعل)

3️⃣ **تسجيل الدخول:**
   - ابحث عن `/api/account/login`
   - اضغط **Try it out**
   - أدخل:
     ```json
     {
       "userNameOrEmailAddress": "admin",
       "password": "1q2w3E*"
     }
     ```
   - اضغط **Execute**

4️⃣ **ستحصل على Access Token**
   ```json
   {
     "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Ij...",
     "token_type": "Bearer",
     "expires_in": 3600
   }
   ```

5️⃣ **استخدم الـ Token:**
   - اضغط على زر **Authorize** في أعلى Swagger
   - أدخل: `Bearer YOUR_TOKEN_HERE`
   - الآن تقدر تستدعي APIs المحمية

---

## ✅ علامات النجاح

تأكد من:
- ✅ قاعدة البيانات اتنشأت وفيها جداول
- ✅ DbMigrator اشتغل بدون أخطاء
- ✅ المشروع اشتغل على `https://localhost:44309`
- ✅ Swagger UI بيفتح ويعرض APIs
- ✅ تقدر تسجل دخول بـ `admin / 1q2w3E*`

---

## 🐛 حل المشاكل الشائعة

### مشكلة 1: "Cannot connect to database"

**الحل:**
- تأكد إن SQL Server شغال
- تأكد من الـ Connection String
- جرب تفتح SQL Server Management Studio وتتصل بنفس الـ Server

---

### مشكلة 2: "LocalDB not found"

**الحل:**
- ثبت SQL Server Express LocalDB
- أو غير الـ Connection String لـ SQL Server عادي

---

### مشكلة 3: "Login failed for user"

**الحل:**
- لو بتستخدم SQL Authentication، تأكد من الـ username/password صح
- لو بتستخدم Windows Authentication، تأكد إن `Trusted_Connection=True`

---

### مشكلة 4: "Port 44309 already in use"

**الحل:**
- غير الـ Port في `appsettings.json` و `launchSettings.json`

---

## 🎯 الخطوة التالية

الآن المشروع شغال! 🎉

**في المرحلة التالية سنتعلم:**
- إنشاء أول Entity (Product)
- إضافة Migration
- إنشاء Application Service
- إنشاء DTOs
- اختبار الـ CRUD APIs

---

## 📝 ملخص الأوامر المستخدمة

سيتم تسجيل كل الأوامر في `commands-log.txt` تلقائياً.

**الأوامر الرئيسية:**
```powershell
# 1. تشغيل DbMigrator
cd "src/Andro.Backend.Reference.DbMigrator"
dotnet run

# 2. تشغيل المشروع
cd "src/Andro.Backend.Reference.Web"
dotnet run

# 3. فتح Swagger
# افتح المتصفح: https://localhost:44309/swagger
```
