# Equipment Management System | نظام إدارة المعدات

[العربية](#العربية) | [English](#english)

> A bilingual equipment management and borrowing system built with ASP.NET Core MVC, Clean Architecture, SQL Server, and ASP.NET Core Identity.
>
> نظام ثنائي اللغة لإدارة المعدات وعمليات الاستعارة، مبني باستخدام ASP.NET Core MVC وClean Architecture وSQL Server وASP.NET Core Identity.

---

## العربية

### نبذة عن المشروع

نظام ويب متكامل لإدارة المعدات والتصنيفات والموظفين وعمليات الاستعارة والإرجاع داخل المؤسسات. يوفر النظام صلاحيات منفصلة للمدير والموظف، ويدعم واجهتين عربية وإنجليزية مع اتجاهي RTL وLTR.

تم تطوير المشروع بوصفه تطبيقًا عمليًا على بناء أنظمة ASP.NET Core قابلة للصيانة والتوسع، مع فصل المسؤوليات وفق مبادئ Clean Architecture.

### المميزات الرئيسية

- تسجيل الدخول وإدارة الحسابات باستخدام ASP.NET Core Identity.
- نظام صلاحيات يعتمد على دوري `Admin` و`Employee`.
- إدارة التصنيفات والمعدات والموظفين.
- إضافة صورة لكل معدة وعرض معلوماتها.
- عرض المعدات المتاحة فقط عند إنشاء عملية استعارة.
- ربط الاستعارة تلقائيًا بالموظف المسجل دخوله.
- تسجيل تاريخ الاستعارة وتاريخ الإرجاع المتوقع.
- تحديث حالة المعدة تلقائيًا عند الاستعارة والإرجاع.
- تمكين المدير من متابعة عمليات الاستعارة والبحث والتصفية.
- منع الموظف العادي من تعديل أو حذف عمليات الاستعارة.
- دعم اللغتين العربية والإنجليزية واتجاهي RTL وLTR.
- فهارس في قاعدة البيانات لتحسين أداء البحث والاستعلامات.
- تصميم متجاوب باستخدام Bootstrap.

### الصلاحيات

| الوظيفة | المدير | الموظف |
|---|:---:|:---:|
| إدارة التصنيفات | ✅ | ❌ |
| إضافة وتعديل وحذف المعدات | ✅ | ❌ |
| إدارة الموظفين والحسابات | ✅ | ❌ |
| مشاهدة جميع الاستعارات | ✅ | ❌ |
| البحث وتصفية الاستعارات | ✅ | محدود بسجلاته |
| استعارة المعدات المتاحة | ✅ | ✅ |
| مشاهدة استعاراته | ✅ | ✅ |

### التقنيات المستخدمة

- C# و.NET 10
- ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- ASP.NET Core Identity وRoles
- SQL Server
- Bootstrap
- HTML وCSS وJavaScript
- Localization وResource Files
- Git وGitHub
- Azure App Service وAzure SQL Database — مرحلة النشر

### هيكل المشروع

```text
EquipmentManagement/
├── EquipmentManagement.Domain
│   └── Entities and core business models
├── EquipmentManagement.Application
│   └── Application rules and use cases
├── EquipmentManagement.Infrastructure
│   └── Database, Entity Framework Core and Identity
└── EquipmentManagement.Web
    └── MVC controllers, Razor views and static files
```

### التشغيل محليًا

#### المتطلبات

- .NET 10 SDK
- SQL Server
- Visual Studio أو Visual Studio Code
- Git

#### خطوات التشغيل

1. استنساخ المستودع:

```bash
git clone YOUR_REPOSITORY_URL
cd EquipmentManagement
```

2. ضبط `DefaultConnection` باستخدام User Secrets أو إعدادات التطوير المحلية. لا تضف بيانات الاتصال السرية إلى GitHub.

3. تطبيق Migrations:

```bash
dotnet ef database update --project EquipmentManagement.Infrastructure --startup-project EquipmentManagement.Web
```

4. تشغيل المشروع:

```bash
dotnet run --project EquipmentManagement.Web
```

### النشر

سيتم نشر التطبيق باستخدام Azure App Service وربطه بقاعدة Azure SQL Database، مع إعداد CI/CD من GitHub إلى Azure.

**رابط النسخة المباشرة:** قريبًا

### الأمان

- لا يحتوي المستودع على كلمات مرور أو بيانات اتصال حقيقية.
- تُخزن كلمات المرور بواسطة ASP.NET Core Identity كقيم Hash آمنة.
- تعتمد حماية الصفحات والعمليات على Authentication وRole-based Authorization.
- يجب حفظ إعدادات الإنتاج السرية داخل إعدادات Azure أو خدمة أسرار مناسبة.

---

## English

### About the project

Equipment Management System is a bilingual web application for managing equipment, categories, employees, borrowing, and return operations within an organization. It provides separate Admin and Employee permissions and supports Arabic and English interfaces with RTL and LTR layouts.

The project demonstrates maintainable ASP.NET Core development, separation of concerns, secure identity management, and a Clean Architecture solution structure.

### Key features

- Authentication and account management with ASP.NET Core Identity.
- Role-based authorization using `Admin` and `Employee` roles.
- Category, equipment, and employee management.
- Equipment image upload and display.
- Only available equipment can be selected for borrowing.
- Borrowing records are linked automatically to the signed-in employee.
- Automatic borrowing date and expected return date tracking.
- Automatic equipment availability updates after borrowing and return.
- Admin monitoring, search, and filtering of borrowing records.
- Employees cannot edit or delete borrowing records.
- Arabic and English localization with RTL and LTR support.
- Database indexes for faster searches and common queries.
- Responsive interface built with Bootstrap.

### Roles and permissions

| Feature | Admin | Employee |
|---|:---:|:---:|
| Manage categories | ✅ | ❌ |
| Create, edit, and delete equipment | ✅ | ❌ |
| Manage employees and accounts | ✅ | ❌ |
| View all borrowing records | ✅ | ❌ |
| Search and filter borrowing records | ✅ | Own records only |
| Borrow available equipment | ✅ | ✅ |
| View personal borrowing records | ✅ | ✅ |

### Technology stack

- C# and .NET 10
- ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- ASP.NET Core Identity and Roles
- SQL Server
- Bootstrap
- HTML, CSS, and JavaScript
- Localization and resource files
- Git and GitHub
- Azure App Service and Azure SQL Database — deployment stage

### Architecture

```text
EquipmentManagement/
├── EquipmentManagement.Domain
│   └── Entities and core business models
├── EquipmentManagement.Application
│   └── Application rules and use cases
├── EquipmentManagement.Infrastructure
│   └── Database, Entity Framework Core and Identity
└── EquipmentManagement.Web
    └── MVC controllers, Razor views and static files
```

### Run locally

#### Requirements

- .NET 10 SDK
- SQL Server
- Visual Studio or Visual Studio Code
- Git

#### Setup

1. Clone the repository:

```bash
git clone YOUR_REPOSITORY_URL
cd EquipmentManagement
```

2. Configure `DefaultConnection` through User Secrets or local development settings. Never commit real credentials to GitHub.

3. Apply the database migrations:

```bash
dotnet ef database update --project EquipmentManagement.Infrastructure --startup-project EquipmentManagement.Web
```

4. Run the application:

```bash
dotnet run --project EquipmentManagement.Web
```

### Screenshots

> Dashboard, equipment, employee, borrowing, Arabic, and English interface screenshots will be added before the final release.

### Deployment

The application will be deployed to Azure App Service with Azure SQL Database and a GitHub-to-Azure CI/CD workflow.

**Live demo:** Coming soon

### Security

- No real passwords or production connection strings are stored in this repository.
- Passwords are securely hashed and managed by ASP.NET Core Identity.
- Pages and operations are protected through authentication and role-based authorization.
- Production secrets must be stored in Azure configuration or an appropriate secret-management service.

---

## Project status | حالة المشروع

Active development — preparing the GitHub release and Azure deployment.

قيد التطوير — جارٍ تجهيز نسخة GitHub والنشر على Azure.

## Author | المطوّر

Developed as a professional portfolio project demonstrating ASP.NET Core MVC, Clean Architecture, Identity, SQL Server, localization, GitHub, and Azure deployment skills.

تم تطويره كمشروع احترافي لعرض مهارات ASP.NET Core MVC وClean Architecture وIdentity وSQL Server والتعريب وGitHub وAzure.

## License | الترخيص

This project is provided for educational and portfolio purposes.

هذا المشروع مقدم لأغراض تعليمية وللعرض ضمن ملف الأعمال.
