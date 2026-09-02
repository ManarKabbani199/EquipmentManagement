using EquipmentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using EquipmentManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

//
// 1. تسجيل ApplicationDbContext
// وربطه بقاعدة بيانات SQL Server
//
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        );

    options.UseSqlServer(connectionString);
});

//
// 2. تسجيل ASP.NET Core Identity
// لإدارة المستخدمين وكلمات المرور وتسجيل الدخول والأدوار
//
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        //
        // إعدادات تسجيل الدخول
        //

        // لا نشترط تأكيد البريد حاليًا
        options.SignIn.RequireConfirmedAccount = false;

        // منع إنشاء أكثر من حساب بالبريد نفسه
        options.User.RequireUniqueEmail = true;

        //
        // إعدادات كلمة المرور
        //

        // يجب أن تحتوي كلمة المرور على رقم
        options.Password.RequireDigit = true;

        // لا نشترط وجود حرف إنجليزي صغير
        options.Password.RequireLowercase = false;

        // لا نشترط وجود حرف إنجليزي كبير
        options.Password.RequireUppercase = false;

        // لا نشترط وجود رمز خاص
        options.Password.RequireNonAlphanumeric = false;

        // الحد الأدنى لطول كلمة المرور
        options.Password.RequiredLength = 6;

        //
        // إعدادات قفل الحساب
        //

        // تفعيل القفل للحسابات الجديدة
        options.Lockout.AllowedForNewUsers = true;

        // قفل الحساب بعد خمس محاولات دخول فاشلة
        options.Lockout.MaxFailedAccessAttempts = 5;

        // مدة قفل الحساب: 15 دقيقة
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })

    // إضافة نظام الأدوار مثل Admin وEmployee
    .AddRoles<IdentityRole>()

    // تخزين بيانات Identity داخل قاعدة البيانات
    .AddEntityFrameworkStores<ApplicationDbContext>();

//
// 3. إعداد Cookie الخاص بتسجيل الدخول
//
builder.Services.ConfigureApplicationCookie(options =>
{
    // منع JavaScript من الوصول إلى Cookie
    options.Cookie.HttpOnly = true;

    // إرسال Cookie عبر HTTPS فقط
    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;

    // تقليل مخاطر هجمات CSRF
    options.Cookie.SameSite =
        SameSiteMode.Lax;

    // صفحة تسجيل الدخول
    options.LoginPath =
        "/Identity/Account/Login";

    // صفحة منع الوصول
    options.AccessDeniedPath =
        "/Identity/Account/AccessDenied";

    // انتهاء جلسة تسجيل الدخول بعد 60 دقيقة
    options.ExpireTimeSpan =
        TimeSpan.FromMinutes(60);

    // تجديد مدة الجلسة إذا استمر المستخدم بالعمل
    options.SlidingExpiration = true;
});

//
// 4. تسجيل MVC وRazor Pages
//

// Controllers وRazor Views
//
// دعم ملفات الترجمة الموجودة داخل مجلد Resources
//
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

//
// تسجيل Controllers وRazor Views
// مع دعم ترجمة النصوص ورسائل التحقق
//
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization(
        LanguageViewLocationExpanderFormat.Suffix
    )
    .AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider =
        (modelType, localizerFactory) =>
            localizerFactory.Create(
             typeof(EquipmentManagement.Web.SharedResource)
            );
});

//
// صفحات Identity مثل Login وLogout
//
builder.Services.AddRazorPages();

//
// إعداد اللغات المدعومة
//
builder.Services.Configure<RequestLocalizationOptions>(
    options =>
    {
        var supportedCultures = new[]
        {
            new CultureInfo("ar"),
            new CultureInfo("en")
        };

        // اللغة الافتراضية للمشروع
        options.DefaultRequestCulture =
            new RequestCulture("ar");

        // اللغات المدعومة لترجمة الواجهة
        options.SupportedUICultures =
            supportedCultures;

        // اللغات المدعومة للتواريخ والأرقام والعملات
        options.SupportedCultures =
            supportedCultures;
    }
);

var app = builder.Build();



//
// 5. إنشاء الأدوار وحساب المدير عند تشغيل المشروع
//
using (var scope = app.Services.CreateScope())
{
    // خدمة إدارة الأدوار
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<
                RoleManager<IdentityRole>
            >();

    // خدمة إدارة المستخدمين
    var userManager =
        scope.ServiceProvider
            .GetRequiredService<
                UserManager<IdentityUser>
            >();

    // الأدوار المطلوبة داخل النظام
    string[] roles =
    {
        "Admin",
        "Employee"
    };

    //
    // إنشاء الأدوار إذا لم تكن موجودة
    //
    foreach (var role in roles)
    {
        var roleExists =
            await roleManager.RoleExistsAsync(role);

        if (!roleExists)
        {
            var roleResult =
                await roleManager.CreateAsync(
                    new IdentityRole(role)
                );

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        error => error.Description
                    )
                );

                throw new Exception(
                    $"فشل إنشاء الدور {role}: {errors}"
                );
            }
        }
    }

    //
    // قراءة بيانات المدير
    //
    // في جهاز التطوير تُقرأ من User Secrets.
    // عند النشر على Azure ستُضاف في App Settings.
    //
    var adminEmail =
        builder.Configuration["AdminUser:Email"];

    var adminPassword =
        builder.Configuration["AdminUser:Password"];

    // إيقاف التشغيل إذا لم تُضبط بيانات المدير
    if (string.IsNullOrWhiteSpace(adminEmail) ||
        string.IsNullOrWhiteSpace(adminPassword))
    {
        throw new Exception(
            "بيانات حساب المدير غير موجودة في إعدادات التطبيق."
        );
    }

    //
    // البحث عن حساب المدير بواسطة البريد
    //
    var adminUser =
        await userManager.FindByEmailAsync(
            adminEmail
        );

    //
    // إنشاء حساب المدير إذا لم يكن موجودًا
    //
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult =
            await userManager.CreateAsync(
                adminUser,
                adminPassword
            );

        if (!createResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                createResult.Errors.Select(
                    error => error.Description
                )
            );

            throw new Exception(
                $"فشل إنشاء حساب المدير: {errors}"
            );
        }
    }

    //
    // منح الحساب دور Admin إذا لم يكن يمتلكه
    //
    var isAdmin =
        await userManager.IsInRoleAsync(
            adminUser,
            "Admin"
        );

    if (!isAdmin)
    {
        var addRoleResult =
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin"
            );

        if (!addRoleResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                addRoleResult.Errors.Select(
                    error => error.Description
                )
            );

            throw new Exception(
                $"فشل منح المدير الصلاحية: {errors}"
            );
        }
    }
}

//
// 6. إعداد مسار معالجة طلبات HTTP
//

// في بيئة الإنتاج لا نعرض تفاصيل الأخطاء التقنية
if (!app.Environment.IsDevelopment())
{
    // توجيه الأخطاء إلى صفحة عامة
    app.UseExceptionHandler("/Home/Error");

    // إجبار المتصفح على استخدام HTTPS مستقبلًا
    app.UseHsts();
}

// تحويل HTTP إلى HTTPS
app.UseHttpsRedirection();
//
// تفعيل اللغة المختارة داخل كل طلب
//
var localizationOptions =
    app.Services
        .GetRequiredService<
            Microsoft.Extensions.Options
                .IOptions<RequestLocalizationOptions>
        >()
        .Value;

app.UseRequestLocalization(localizationOptions);

//
// السماح بعرض الملفات الموجودة داخل wwwroot
// ويشمل صور المعدات داخل uploads/equipment
//
app.UseStaticFiles();

// تفعيل نظام التوجيه
app.UseRouting();

//
// يجب أن يأتي Authentication قبل Authorization
//

// معرفة هوية المستخدم المسجل
app.UseAuthentication();

// التحقق من صلاحيات المستخدم وأدواره
app.UseAuthorization();

//
// 7. ربط الملفات الثابتة المحسّنة
//
app.MapStaticAssets();

//
// 8. تعريف مسار Controllers الافتراضي
//
app.MapControllerRoute(
        name: "default",
        pattern:
            "{controller=Home}/{action=Index}/{id?}"
    )
    .WithStaticAssets();

//
// 9. ربط صفحات Identity
// مثل Login وLogout وAccessDenied
//
app.MapRazorPages();

//
// 10. تشغيل التطبيق
//
app.Run();