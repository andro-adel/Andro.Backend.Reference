# 🚀 Deployment Guide - دليل نشر التطبيق للـ Production

## 📋 نظرة عامة

هذا الدليل يغطي كل الخطوات اللازمة لنشر تطبيق ABP.io إلى بيئة Production، بما في ذلك Build، Configuration، Database Migration، و Deployment Options المختلفة.

---

## 🎯 Pre-Deployment Checklist

قبل النشر، تأكد من:

- ✅ **Testing** - كل الـ tests تعمل بنجاح
- ✅ **Configuration** - Production appsettings جاهزة
- ✅ **Database** - Migration scripts جاهزة
- ✅ **Security** - Secrets محفوظة بشكل آمن
- ✅ **Logging** - Logging configuration صحيحة
- ✅ **Performance** - تم اختبار الأداء
- ✅ **Backup** - خطة backup جاهزة

---

## 🏗️ Step 1: Build للـ Production

### **1.1 Clean Solution**

```powershell
# نظف كل build artifacts السابقة
dotnet clean
```

### **1.2 Restore Packages**

```powershell
# استعد كل الـ dependencies
dotnet restore
```

### **1.3 Build في Release Mode**

```powershell
# Build للـ Production
dotnet build --configuration Release
```

### **1.4 Publish Application**

```powershell
# Publish Web Application
cd src/Andro.Backend.Reference.Web
dotnet publish --configuration Release --output ../../publish/web

# Publish DbMigrator (للـ migrations في production)
cd ../Andro.Backend.Reference.DbMigrator
dotnet publish --configuration Release --output ../../publish/migrator
```

**Output:**
```
publish/
  ├── web/              ← Web Application
  └── migrator/         ← Database Migrator
```

---

## ⚙️ Step 2: Configuration للـ Production

### **2.1 إنشاء appsettings.Production.json**

```json
// src/Andro.Backend.Reference.Web/appsettings.Production.json
{
  "App": {
    "SelfUrl": "https://yourdomain.com",
    "CorsOrigins": "https://yourdomain.com,https://www.yourdomain.com",
    "RedirectAllowedUrls": "https://yourdomain.com"
  },
  "ConnectionStrings": {
    "Default": "Server=production-server;Database=AndroBackendReference;User Id=your-user;Password=your-password;TrustServerCertificate=True"
  },
  "AuthServer": {
    "Authority": "https://yourdomain.com",
    "RequireHttpsMetadata": "true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      },
      {
        "Name": "Console"
      }
    ]
  }
}
```

---

### **2.2 Environment Variables (أفضل من hardcoded passwords)**

#### **Windows (PowerShell):**
```powershell
# Set environment variables
$env:ConnectionStrings__Default="Server=...;Database=...;User Id=...;Password=..."
$env:App__SelfUrl="https://yourdomain.com"
$env:ASPNETCORE_ENVIRONMENT="Production"
```

#### **Linux:**
```bash
# Set environment variables
export ConnectionStrings__Default="Server=...;Database=...;User Id=...;Password=..."
export App__SelfUrl="https://yourdomain.com"
export ASPNETCORE_ENVIRONMENT="Production"
```

#### **Docker:**
```yaml
# docker-compose.yml
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ConnectionStrings__Default=Server=...;Database=...
  - App__SelfUrl=https://yourdomain.com
```

---

### **2.3 Secrets Management**

#### **Option 1: User Secrets (Development)**
```powershell
dotnet user-secrets set "ConnectionStrings:Default" "Server=..."
```

#### **Option 2: Azure Key Vault (Production)**
```json
// appsettings.Production.json
{
  "AzureKeyVault": {
    "VaultUri": "https://your-vault.vault.azure.net/"
  }
}
```

#### **Option 3: Environment Variables (Recommended)**
استخدم Environment Variables للـ sensitive data بدلاً من hardcoding في config files.

---

## 🗄️ Step 3: Database Migration للـ Production

### **3.1 Generate Migration Script**

```powershell
# انتقل لمشروع EntityFrameworkCore
cd src/Andro.Backend.Reference.EntityFrameworkCore

# Generate SQL script
dotnet ef migrations script --output ../../migrations/migration.sql --idempotent
```

**الفوائد:**
- ✅ **Idempotent** - يمكن تشغيله أكثر من مرة بدون مشاكل
- ✅ **Review** - يمكن مراجعة الـ SQL قبل التنفيذ
- ✅ **DBA Approval** - يمكن إرساله للـ DBA للموافقة

---

### **3.2 Run Migration في Production**

#### **Option 1: DbMigrator (Recommended)**

```powershell
# في production server
cd publish/migrator
dotnet Andro.Backend.Reference.DbMigrator.dll
```

**يقوم بـ:**
- ✅ Run migrations
- ✅ Seed initial data
- ✅ Handle errors gracefully

---

#### **Option 2: Manual SQL Script**

```sql
-- تشغيل الـ migration.sql يدوياً
-- في SQL Server Management Studio أو أي SQL client
```

---

#### **Option 3: EF Core CLI (Not Recommended for Production)**

```powershell
# فقط في حالات الطوارئ
dotnet ef database update
```

---

### **3.3 Verify Migration**

```sql
-- تحقق من الـ tables
SELECT * FROM INFORMATION_SCHEMA.TABLES

-- تحقق من الـ migration history
SELECT * FROM __EFMigrationsHistory
```

---

## 🚀 Step 4: Deployment Options

### **Option 1: IIS (Windows Server)**

#### **4.1 Prerequisites:**
- ✅ Windows Server
- ✅ IIS installed
- ✅ .NET Runtime installed
- ✅ SQL Server accessible

#### **4.2 Install .NET Hosting Bundle:**
```
Download from: https://dotnet.microsoft.com/download/dotnet/10.0
Install: dotnet-hosting-10.x.x-win.exe
```

#### **4.3 Create IIS Site:**

1. **Open IIS Manager**
2. **Add Website:**
   - Site name: `Andro.Backend.Reference`
   - Physical path: `C:\inetpub\wwwroot\andro-backend`
   - Binding: `https://yourdomain.com:443`
   - SSL Certificate: Import your certificate

3. **Application Pool Settings:**
   - .NET CLR Version: `No Managed Code`
   - Managed Pipeline Mode: `Integrated`
   - Identity: `ApplicationPoolIdentity` أو custom account

4. **Copy Files:**
```powershell
# Copy published files
Copy-Item -Path "publish/web/*" -Destination "C:\inetpub\wwwroot\andro-backend" -Recurse
```

5. **Set Permissions:**
```powershell
# Give IIS_IUSRS permission
icacls "C:\inetpub\wwwroot\andro-backend" /grant "IIS_IUSRS:(OI)(CI)F" /T
```

6. **Configure web.config:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet" 
                arguments=".\Andro.Backend.Reference.Web.dll" 
                stdoutLogEnabled="true" 
                stdoutLogFile=".\logs\stdout" 
                hostingModel="inprocess" />
  </system.webServer>
</configuration>
```

7. **Start Site:**
```powershell
# Start IIS site
Start-WebSite -Name "Andro.Backend.Reference"
```

---

### **Option 2: Docker**

#### **4.4 Create Dockerfile:**

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["src/Andro.Backend.Reference.Web/Andro.Backend.Reference.Web.csproj", "src/Andro.Backend.Reference.Web/"]
COPY ["src/Andro.Backend.Reference.Application/Andro.Backend.Reference.Application.csproj", "src/Andro.Backend.Reference.Application/"]
COPY ["src/Andro.Backend.Reference.Domain/Andro.Backend.Reference.Domain.csproj", "src/Andro.Backend.Reference.Domain/"]
COPY ["src/Andro.Backend.Reference.EntityFrameworkCore/Andro.Backend.Reference.EntityFrameworkCore.csproj", "src/Andro.Backend.Reference.EntityFrameworkCore/"]
COPY ["src/Andro.Backend.Reference.HttpApi/Andro.Backend.Reference.HttpApi.csproj", "src/Andro.Backend.Reference.HttpApi/"]
COPY ["src/Andro.Backend.Reference.Application.Contracts/Andro.Backend.Reference.Application.Contracts.csproj", "src/Andro.Backend.Reference.Application.Contracts/"]
COPY ["src/Andro.Backend.Reference.Domain.Shared/Andro.Backend.Reference.Domain.Shared.csproj", "src/Andro.Backend.Reference.Domain.Shared/"]

# Restore
RUN dotnet restore "src/Andro.Backend.Reference.Web/Andro.Backend.Reference.Web.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/src/Andro.Backend.Reference.Web"
RUN dotnet build "Andro.Backend.Reference.Web.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Andro.Backend.Reference.Web.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Andro.Backend.Reference.Web.dll"]
```

#### **4.5 Create docker-compose.yml:**

```yaml
version: '3.8'

services:
  web:
    image: andro-backend-reference-web:latest
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "80:80"
      - "443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__Default=Server=db;Database=AndroBackendReference;User Id=sa;Password=Your_password123;TrustServerCertificate=True
      - App__SelfUrl=https://yourdomain.com
    depends_on:
      - db
    networks:
      - app-network

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_password123
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - app-network

networks:
  app-network:
    driver: bridge

volumes:
  sqldata:
```

#### **4.6 Build & Run:**

```powershell
# Build image
docker build -t andro-backend-reference-web:latest .

# Run with docker-compose
docker-compose up -d

# View logs
docker-compose logs -f web

# Stop
docker-compose down
```

---

### **Option 3: Azure App Service**

#### **4.7 Deploy to Azure:**

```powershell
# Login to Azure
az login

# Create Resource Group
az group create --name andro-backend-rg --location eastus

# Create App Service Plan
az appservice plan create --name andro-backend-plan --resource-group andro-backend-rg --sku B1 --is-linux

# Create Web App
az webapp create --resource-group andro-backend-rg --plan andro-backend-plan --name andro-backend-reference --runtime "DOTNETCORE|10.0"

# Configure Connection String
az webapp config connection-string set --resource-group andro-backend-rg --name andro-backend-reference --settings Default="Server=..." --connection-string-type SQLAzure

# Deploy
az webapp deployment source config-zip --resource-group andro-backend-rg --name andro-backend-reference --src publish.zip
```

---

### **Option 4: Linux Server (Ubuntu)**

#### **4.8 Install Prerequisites:**

```bash
# Update system
sudo apt update
sudo apt upgrade -y

# Install .NET Runtime
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y aspnetcore-runtime-10.0

# Install Nginx (Reverse Proxy)
sudo apt install -y nginx
```

#### **4.9 Copy Files:**

```bash
# Create directory
sudo mkdir -p /var/www/andro-backend

# Copy files
sudo cp -r publish/web/* /var/www/andro-backend/

# Set permissions
sudo chown -R www-data:www-data /var/www/andro-backend
```

#### **4.10 Create Systemd Service:**

```bash
# Create service file
sudo nano /etc/systemd/system/andro-backend.service
```

```ini
[Unit]
Description=Andro Backend Reference

[Service]
WorkingDirectory=/var/www/andro-backend
ExecStart=/usr/bin/dotnet /var/www/andro-backend/Andro.Backend.Reference.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=andro-backend
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Enable and start service
sudo systemctl enable andro-backend
sudo systemctl start andro-backend
sudo systemctl status andro-backend
```

#### **4.11 Configure Nginx:**

```bash
sudo nano /etc/nginx/sites-available/andro-backend
```

```nginx
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# Enable site
sudo ln -s /etc/nginx/sites-available/andro-backend /etc/nginx/sites-enabled/

# Test configuration
sudo nginx -t

# Restart Nginx
sudo systemctl restart nginx
```

#### **4.12 SSL with Let's Encrypt:**

```bash
# Install Certbot
sudo apt install -y certbot python3-certbot-nginx

# Get SSL certificate
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com

# Auto-renewal is configured automatically
```

---

## 📊 Step 5: Logging & Monitoring

### **5.1 Configure Serilog:**

```json
// appsettings.Production.json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "fileSizeLimitBytes": 10485760,
          "rollOnFileSizeLimit": true
        }
      },
      {
        "Name": "Console"
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://seq-server:5341"
        }
      }
    ]
  }
}
```

---

### **5.2 Health Checks:**

```csharp
// في Program.cs أو Startup
app.MapHealthChecks("/health");
```

**Test:**
```powershell
curl https://yourdomain.com/health
```

---

### **5.3 Application Insights (Azure):**

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-instrumentation-key"
  }
}
```

---

## 🔒 Step 6: Security Checklist

- ✅ **HTTPS Only** - Force HTTPS redirection
- ✅ **Secrets** - لا تحفظ passwords في code
- ✅ **CORS** - Configure allowed origins
- ✅ **Authentication** - Verify token validation
- ✅ **Authorization** - Check permissions
- ✅ **Rate Limiting** - Prevent abuse
- ✅ **SQL Injection** - Use parameterized queries (ABP يفعلها تلقائياً)
- ✅ **XSS** - ABP يحمي تلقائياً
- ✅ **CSRF** - ABP يحمي تلقائياً

---

## 📈 Step 7: Performance Optimization

### **7.1 Enable Response Compression:**

```csharp
// في Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

app.UseResponseCompression();
```

### **7.2 Enable Response Caching:**

```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

### **7.3 Database Indexes:**

```csharp
// في DbContext
modelBuilder.Entity<Product>()
    .HasIndex(p => p.Name);

modelBuilder.Entity<Product>()
    .HasIndex(p => p.CategoryId);
```

### **7.4 Enable Output Caching (ASP.NET Core 10):**

```csharp
builder.Services.AddOutputCache();
app.UseOutputCache();

// في Controller
[OutputCache(Duration = 60)]
public async Task<ProductDto> GetAsync(Guid id)
{
    // ...
}
```

---

## 🔄 Step 8: CI/CD Pipeline

### **8.1 GitHub Actions Example:**

```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish src/Andro.Backend.Reference.Web/Andro.Backend.Reference.Web.csproj --configuration Release --output ./publish
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'andro-backend-reference'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

---

## 📦 Step 9: Backup Strategy

### **9.1 Database Backup:**

```sql
-- Full Backup
BACKUP DATABASE [AndroBackendReference]
TO DISK = 'C:\Backups\AndroBackendReference_Full.bak'
WITH FORMAT, INIT, NAME = 'Full Backup';

-- Differential Backup
BACKUP DATABASE [AndroBackendReference]
TO DISK = 'C:\Backups\AndroBackendReference_Diff.bak'
WITH DIFFERENTIAL, INIT;
```

### **9.2 Automated Backup:**

```powershell
# PowerShell script for automated backup
$date = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFile = "C:\Backups\AndroBackendReference_$date.bak"

Invoke-Sqlcmd -Query "BACKUP DATABASE [AndroBackendReference] TO DISK = '$backupFile'"
```

---

## 🔍 Step 10: Post-Deployment Verification

### **10.1 Verify Application:**

```powershell
# Check if app is running
curl https://yourdomain.com/health

# Check API
curl https://yourdomain.com/api/app/product
```

### **10.2 Check Logs:**

```powershell
# IIS
Get-Content "C:\inetpub\wwwroot\andro-backend\Logs\log-*.txt" -Tail 50

# Linux
sudo journalctl -u andro-backend -n 50 -f

# Docker
docker-compose logs -f web
```

### **10.3 Monitor Performance:**

- ✅ Response times
- ✅ Memory usage
- ✅ CPU usage
- ✅ Database connections
- ✅ Error rates

---

## 🆘 Troubleshooting

### **Issue 1: Application won't start**

```powershell
# Check logs
# Check environment variables
# Verify connection string
# Check .NET Runtime version
```

### **Issue 2: Database connection fails**

```powershell
# Test connection string
# Check firewall rules
# Verify SQL Server is running
# Check credentials
```

### **Issue 3: 502 Bad Gateway (Nginx)**

```bash
# Check if app is running
sudo systemctl status andro-backend

# Check Nginx logs
sudo tail -f /var/log/nginx/error.log
```

---

## 📝 Deployment Checklist Summary

**Pre-Deployment:**
- [ ] All tests passing
- [ ] Configuration reviewed
- [ ] Secrets secured
- [ ] Database backup taken
- [ ] Deployment plan reviewed

**During Deployment:**
- [ ] Build & Publish successful
- [ ] Database migrated
- [ ] Application deployed
- [ ] Configuration applied
- [ ] Services started

**Post-Deployment:**
- [ ] Health check passing
- [ ] API endpoints working
- [ ] Logs reviewed
- [ ] Performance monitored
- [ ] Team notified

---

## 🎯 الخلاصة

**Deployment Options:**
- ✅ **IIS** - Windows Server
- ✅ **Docker** - Containerized
- ✅ **Azure** - Cloud Platform
- ✅ **Linux** - Ubuntu/Nginx

**Key Points:**
- ✅ Use **Environment Variables** for secrets
- ✅ Run **DbMigrator** for database updates
- ✅ Configure **Logging** properly
- ✅ Enable **HTTPS**
- ✅ Monitor **Performance**
- ✅ Have **Backup** strategy
- ✅ Implement **CI/CD**

---

**مشروع ABP.io جاهز للنشر في Production! 🚀**
