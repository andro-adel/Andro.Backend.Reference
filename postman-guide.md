# 📮 دليل استخدام Postman مع Andro.Backend.Reference

---

## 📥 استيراد الملفات

### 1️⃣ افتح Postman

قم بتحميل وتثبيت Postman من: https://www.postman.com/downloads/

---

### 2️⃣ استيراد Collection

1. افتح Postman
2. اضغط على **Import** في الأعلى
3. اختر ملف: `Andro.Backend.Reference.postman_collection.json`
4. اضغط **Import**

✅ سيظهر لك Collection باسم: **Andro.Backend.Reference API**

---

### 3️⃣ استيراد Environment

1. اضغط على **Import** مرة أخرى
2. اختر ملف: `Andro.Backend.Reference.postman_environment.json`
3. اضغط **Import**

✅ سيظهر لك Environment باسم: **Andro.Backend.Reference - Local**

---

### 4️⃣ تفعيل Environment

1. في الزاوية اليمنى العليا، ستجد قائمة منسدلة
2. اختر: **Andro.Backend.Reference - Local**

✅ الآن كل المتغيرات جاهزة للاستخدام

---

## 🔐 خطوات البدء

### الخطوة 1: تسجيل الدخول

1. افتح Collection: **Andro.Backend.Reference API**
2. افتح المجلد: **🔐 Authentication**
3. اختر: **Login (تسجيل الدخول)**
4. اضغط **Send**

**ماذا يحدث؟**
- ✅ سيتم تسجيل الدخول بـ `admin / 1q2w3E*`
- ✅ الـ `access_token` سيتم حفظه **تلقائياً** في Environment
- ✅ كل الطلبات التالية ستستخدم هذا الـ Token

---

### الخطوة 2: اختبار API محمي

1. افتح: **👤 User Profile** > **Get My Profile**
2. اضغط **Send**

✅ ستحصل على بياناتك الشخصية (لأن الـ Token شغال)

---

## 📂 محتويات Collection

### 🔐 Authentication
- **Login** - تسجيل الدخول (يحفظ Token تلقائياً)
- **Logout** - تسجيل الخروج
- **Register** - تسجيل مستخدم جديد

---

### 👤 User Profile
- **Get My Profile** - بياناتي الشخصية
- **Update My Profile** - تحديث البيانات
- **Change Password** - تغيير كلمة المرور

---

### ⚙️ Application Configuration
- **Get Application Configuration** - إعدادات التطبيق الكاملة
- **Get Application Localization** - الترجمات

---

### 🏥 Health Check
- **Health Status** - فحص صحة التطبيق والاتصال بقاعدة البيانات

---

### 👥 Identity Management

#### Users (إدارة المستخدمين)
- **Get Users List** - قائمة المستخدمين (مع Pagination)
- **Get User By Id** - بيانات مستخدم محدد
- **Create User** - إنشاء مستخدم جديد
- **Update User** - تحديث مستخدم
- **Delete User** - حذف مستخدم

#### Roles (إدارة الأدوار)
- **Get Roles List** - قائمة الأدوار
- **Get Role By Id** - بيانات دور محدد
- **Create Role** - إنشاء دور جديد

---

### 🏢 Tenant Management
- **Get Tenants List** - قائمة العملاء (Multi-Tenancy)
- **Get Tenant By Name** - البحث عن Tenant

---

### 📦 Products
⏳ **سيتم إضافتها في المرحلة الثانية**

عندما ننشئ Product Entity وكل الـ CRUD APIs، سنضيفها هنا تلقائياً.

---

## 🔧 المتغيرات المتاحة

في Environment ستجد المتغيرات التالية:

| المتغير | القيمة الافتراضية | الوصف |
|---------|-------------------|-------|
| `base_url` | `https://localhost:44309` | عنوان API |
| `access_token` | (تلقائي) | يتم حفظه بعد Login |
| `refresh_token` | (تلقائي) | لتجديد الـ Token |
| `user_id` | - | لحفظ ID مستخدم |
| `role_id` | - | لحفظ ID دور |
| `admin_username` | `admin` | اسم المستخدم الافتراضي |
| `admin_password` | `1q2w3E*` | كلمة المرور الافتراضية |

---

## 💡 نصائح مهمة

### 1️⃣ التفويض (Authorization)

Collection معدة لاستخدام **Bearer Token** تلقائياً من المتغير `{{access_token}}`

**لا تحتاج أن تضيف Token يدوياً!** ✅

---

### 2️⃣ استخدام المتغيرات

في أي مكان في Postman، استخدم:
```
{{base_url}}
{{access_token}}
{{user_id}}
```

**مثال:**
```
{{base_url}}/api/identity/users/{{user_id}}
```

---

### 3️⃣ الـ Login Script

في طلب Login، يوجد **Test Script** يحفظ Token تلقائياً:

```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    pm.environment.set("access_token", jsonData.access_token);
    pm.environment.set("refresh_token", jsonData.refresh_token);
    console.log("✅ تم تسجيل الدخول بنجاح");
}
```

---

### 4️⃣ Pagination

في APIs القوائم، استخدم:
- `MaxResultCount`: عدد النتائج (افتراضي: 10)
- `SkipCount`: تخطي النتائج (للصفحات)

**مثال للصفحة الثانية:**
```
/api/identity/users?MaxResultCount=10&SkipCount=10
```

---

### 5️⃣ البحث والفلترة

معظم APIs تدعم `Filter` للبحث:
```
/api/identity/users?Filter=admin&MaxResultCount=10
```

---

## 🎯 أمثلة عملية

### مثال 1: إنشاء مستخدم جديد

1. سجل دخول أولاً (Login)
2. افتح: **Identity Management** > **Users** > **Create User**
3. عدل البيانات في Body:
```json
{
  "userName": "ahmed",
  "name": "أحمد",
  "surname": "محمد",
  "email": "ahmed@example.com",
  "phoneNumber": "+201234567890",
  "isActive": true,
  "lockoutEnabled": false,
  "roleNames": [],
  "password": "Ahmed123!"
}
```
4. اضغط **Send**

✅ سيتم إنشاء المستخدم

---

### مثال 2: البحث عن مستخدمين

1. افتح: **Get Users List**
2. أضف Query Parameter:
   - Key: `Filter`
   - Value: `ahmed`
3. اضغط **Send**

✅ سيظهر فقط المستخدمين الذين أسماؤهم تحتوي على "ahmed"

---

### مثال 3: إنشاء دور (Role) جديد

1. افتح: **Identity Management** > **Roles** > **Create Role**
2. Body:
```json
{
  "name": "Manager",
  "isDefault": false,
  "isPublic": true
}
```
3. اضغط **Send**

✅ سيتم إنشاء الدور

---

## 🔄 تحديث Collection

عندما نضيف APIs جديدة (مثل Products في المرحلة 2)، سيتم:
1. تحديث ملف `Andro.Backend.Reference.postman_collection.json`
2. إعادة استيراده في Postman
3. (أو استخدام Sync إذا كنت تستخدم Postman Cloud)

---

## 🐛 حل المشاكل

### مشكلة: 401 Unauthorized

**السبب:** الـ Token منتهي أو غير صحيح

**الحل:**
1. سجل دخول مرة أخرى (Login)
2. تأكد أن Environment مفعل

---

### مشكلة: Could not get response

**السبب:** المشروع غير شغال

**الحل:**
```powershell
cd "src/Andro.Backend.Reference.Web"
dotnet run
```

تأكد أن المشروع شغال على `https://localhost:44309`

---

### مشكلة: SSL Certificate Error

**الحل في Postman:**
1. Settings (⚙️)
2. عطل: **SSL certificate verification**

---

## 📚 الخطوة التالية

في **المرحلة 2** من خطة التعلم سنقوم بـ:
1. إنشاء **Product Entity**
2. إنشاء **CRUD APIs للـ Products**
3. **تحديث Postman Collection** بالـ APIs الجديدة:
   - Get Products List
   - Get Product By Id
   - Create Product
   - Update Product
   - Delete Product

---

## ✅ ملخص سريع

```
1. استورد Collection + Environment
2. فعل Environment
3. سجل دخول (Login) → Token يحفظ تلقائياً
4. جرب أي API تاني
5. استمتع! 🚀
```

---

**📌 ملاحظة:** هذا الملف سيتم تحديثه مع كل مرحلة جديدة في خطة التعلم.
