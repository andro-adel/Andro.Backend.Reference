# 🌱 Database Seed Complete Guide

## 📋 نظرة عامة

هذا الدليل يشرح كيفية إعادة بناء قاعدة البيانات من الصفر مع seed كامل للبيانات.

---

## 🎯 البيانات المتوفرة بعد الـ Seed

### 1️⃣ **Admin User** (جاهز من ABP)
```json
{
  "username": "admin",
  "email": "admin@abp.io",
  "password": "1q2w3E*"
}
```

### 2️⃣ **Categories** (5 فئات)

| Category ID | Name | Description |
|-------------|------|-------------|
| `3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f` | Electronics | Electronic devices and accessories |
| `4b182d26-7c6d-5d6f-0d2f-2d2a2d2a2d2a` | Clothing | Fashion and apparel |
| `5c293e37-8d7e-6e7a-1e3a-3e3b3e3b3e3b` | Books | Books and publications |
| `6d3a4f48-9e8f-7f8b-2f4b-4f4c4f4c4f4c` | Home & Garden | Home improvement and garden supplies |
| `7e4b5a59-0f9a-8a9c-3a5c-5a5d5a5d5a5d` | Sports | Sports equipment and fitness gear |

### 3️⃣ **Products** (15 منتج)

#### Electronics (3 منتجات):
- **Laptop Pro 15** - $1,299.99 (Stock: 50)
- **Wireless Mouse** - $29.99 (Stock: 200)
- **USB-C Hub** - $49.99 (Stock: 150)

#### Clothing (3 منتجات):
- **Cotton T-Shirt** - $19.99 (Stock: 300)
- **Denim Jeans** - $59.99 (Stock: 120)
- **Winter Jacket** - $89.99 (Stock: 80)

#### Books (3 منتجات):
- **Clean Code** - $45.99 (Stock: 100)
- **Design Patterns** - $54.99 (Stock: 75)
- **The Pragmatic Programmer** - $42.99 (Stock: 90)

#### Home & Garden (3 منتجات):
- **Garden Tool Set** - $79.99 (Stock: 60)
- **LED Desk Lamp** - $34.99 (Stock: 150)
- **Plant Pot Set** - $24.99 (Stock: 200)

#### Sports (3 منتجات):
- **Yoga Mat** - $29.99 (Stock: 180)
- **Resistance Bands Set** - $24.99 (Stock: 220)
- **Running Shoes** - $79.99 (Stock: 100)

### 4️⃣ **Roles** (جاهز من ABP)
- **admin** - Full permissions

### 5️⃣ **Permissions** (جاهز من ABP)
- Identity Management
- Tenant Management
- Settings Management
- All Application Permissions

---

## 🔄 خطوات إعادة بناء الداتابيز من الصفر

### **الخطوة 1: حذف قاعدة البيانات الحالية**

```powershell
# من PowerShell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "DROP DATABASE [Reference]"
```

**النتيجة:**
```
✅ Database dropped successfully
```

---

### **الخطوة 2: تشغيل DbMigrator**

```powershell
# الانتقال لمجلد DbMigrator
cd "d:\DevStudy\Core Platform\abp\Andro.Backend.Reference\src\Andro.Backend.Reference.DbMigrator"

# تشغيل DbMigrator
dotnet run
```

**ما يحدث:**
1. ✅ إنشاء قاعدة بيانات جديدة
2. ✅ تطبيق كل الـ Migrations
3. ✅ Seed للـ Admin User
4. ✅ Seed للـ Roles & Permissions
5. ✅ Seed للـ OpenIddict Clients
6. ✅ Seed للـ 5 Categories
7. ✅ Seed للـ 15 Products

**الناتج المتوقع:**
```
[02:54:06 INF] Started database migrations...
[02:54:06 INF] Migrating schema for host database...
[02:54:08 INF] Executing host database seed...
[02:54:10 INF] Successfully completed host database migrations.
[02:54:10 INF] Successfully completed all database migrations.
[02:54:10 INF] You can safely end this process...
```

---

### **الخطوة 3: تشغيل Web API**

```powershell
# الانتقال لمجلد Web
cd "d:\DevStudy\Core Platform\abp\Andro.Backend.Reference\src\Andro.Backend.Reference.Web"

# تشغيل API
dotnet run
```

**API متاح على:**
```
https://localhost:44309
```

---

## 🔐 Postman - الاستخدام الفوري

### **1. Login (Get Access Token)**

```http
POST https://localhost:44309/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
&username=admin
&password=1q2w3E*
&client_id=Reference_App
&scope=offline_access Reference
```

**Response:**
```json
{
  "access_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "..."
}
```

---

### **2. Get All Categories**

```http
GET https://localhost:44309/api/app/category
Authorization: Bearer {access_token}
```

**Expected Response:** 5 categories

---

### **3. Get All Products**

```http
GET https://localhost:44309/api/app/product
Authorization: Bearer {access_token}
```

**Expected Response:** 15 products

---

### **4. Get Category by ID**

```http
GET https://localhost:44309/api/app/category/3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f
Authorization: Bearer {access_token}
```

**Expected Response:**
```json
{
  "id": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f",
  "name": "Electronics",
  "description": "Electronic devices and accessories"
}
```

---

## 📝 الملفات المهمة

### **ReferenceDataSeedContributor.cs**
المسار:
```
src/Andro.Backend.Reference.Domain/Data/ReferenceDataSeedContributor.cs
```

**الوظيفة:**
- Seed للـ Categories (5 فئات)
- Seed للـ Products (15 منتج)
- يعمل تلقائياً عند تشغيل DbMigrator
- يتحقق من عدم وجود بيانات قبل الـ Insert (idempotent)

---

## ✅ التحقق من نجاح الـ Seed

### **1. عدد الـ Categories**
```http
GET https://localhost:44309/api/app/category
```
**Expected:** `totalCount: 5`

### **2. عدد الـ Products**
```http
GET https://localhost:44309/api/app/product
```
**Expected:** `totalCount: 15`

### **3. Electronics Products**
```http
GET https://localhost:44309/api/app/product?CategoryId=3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f
```
**Expected:** 3 products (Laptop, Mouse, Hub)

---

## 🎯 Postman Environment Variables

قم بتحديث الـ Environment Variables في Postman:

```json
{
  "base_url": "https://localhost:44309",
  "admin_username": "admin",
  "admin_password": "1q2w3E*",
  "category_id": "3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f"
}
```

---

## 🔄 متى تحتاج لإعادة Seed؟

### **الحالات:**
1. ✅ تغيير في الـ Migration
2. ✅ تلف البيانات
3. ✅ إضافة seed data جديد
4. ✅ Testing من الصفر
5. ✅ Reset للبيئة

### **الخطوات:**
```powershell
# 1. حذف DB
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "DROP DATABASE [Reference]"

# 2. Re-seed
cd src\Andro.Backend.Reference.DbMigrator
dotnet run

# 3. Start API
cd ..\Andro.Backend.Reference.Web
dotnet run
```

---

## 💡 نصائح

### ✅ **Best Practices:**
1. احتفظ بنسخة من الـ seed data
2. استخدم GUIDs ثابتة للبيانات المهمة (Categories)
3. اجعل الـ DataSeedContributor idempotent
4. وثق كل الـ IDs المستخدمة
5. استخدم Environment Variables في Postman

### ⚠️ **تحذيرات:**
1. `DROP DATABASE` يحذف كل البيانات نهائياً
2. DbMigrator يعمل seed مرة واحدة فقط (unless forced)
3. تأكد من تشغيل API قبل استخدام Postman

---

## 📊 Database Schema Overview

### **Tables Created:**
- `AppCategories` - 5 rows
- `AppProducts` - 15 rows
- `AbpUsers` - 1 row (admin)
- `AbpRoles` - 1 row (admin)
- `AbpPermissionGrants` - Multiple rows
- `OpenIddictApplications` - 2 rows (Reference_App, Swagger)

---

## 🎉 الخلاصة

بعد إتمام الخطوات:
- ✅ Database جديدة من الصفر
- ✅ 5 Categories جاهزة
- ✅ 15 Products جاهزة
- ✅ Admin user جاهز
- ✅ Roles & Permissions جاهزة
- ✅ OpenIddict configured
- ✅ Postman جاهز للاستخدام الفوري!

**كل شيء جاهز للتجربة! 🚀**
