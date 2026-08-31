using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using EquipmentManagement.Web;


namespace EquipmentManagement.Web.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class EquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IStringLocalizer<SharedResource> _localizer;

        public EquipmentController(
    ApplicationDbContext context,
    IWebHostEnvironment webHostEnvironment,
    IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
        }

        // GET: Equipment
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Equipment.Include(e => e.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Equipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // GET: Equipment/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Equipment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
     [Bind(
        "Name,SerialNumber,Description,PurchaseDate," +
        "PurchasePrice,IsAvailable,CategoryId"
    )]
    Equipment equipment,
     IFormFile? imageFile)
        {
            const long maximumImageSize = 5 * 1024 * 1024;

            var allowedExtensions = new[]
            {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

            var allowedContentTypes = new[]
            {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

            // تنظيف الرقم التسلسلي
            equipment.SerialNumber =
                (equipment.SerialNumber ?? string.Empty).Trim();

            // منع تكرار الرقم التسلسلي
            if (!string.IsNullOrWhiteSpace(equipment.SerialNumber))
            {
                var serialNumberExists =
                    await _context.Equipment.AnyAsync(
                        e => e.SerialNumber == equipment.SerialNumber
                    );

                if (serialNumberExists)
                {
                    ModelState.AddModelError(
                        nameof(equipment.SerialNumber),
                        _localizer["DuplicateSerialNumber"]
                    );
                }
            }

            // التحقق من الصورة
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(
                    imageFile.FileName
                ).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "imageFile",
                        _localizer["AllowedImageTypes"]
                    );
                }

                var contentType =
                    imageFile.ContentType.ToLowerInvariant();

                if (!allowedContentTypes.Contains(contentType))
                {
                    ModelState.AddModelError(
                        "imageFile",
                        _localizer["InvalidImageType"]
                    );
                }

                if (imageFile.Length > maximumImageSize)
                {
                    ModelState.AddModelError(
                        "imageFile",
                        _localizer["ImageSizeExceeded"]
                    );
                }
            }

            if (equipment.PurchaseDate == default)
            {
                ModelState.Remove(
                    nameof(equipment.PurchaseDate)
                );

                ModelState.AddModelError(
                    nameof(equipment.PurchaseDate),
                    _localizer["PurchaseDateRequired"]
                );
            }


            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(
                    _context.Categories,
                    "Id",
                    "Name",
                    equipment.CategoryId
                );

                return View(equipment);
            }

            // حفظ المعدة أولًا للحصول على Id
            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();

            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(
                    imageFile.FileName
                ).ToLowerInvariant();

                var fileName = $"{equipment.Id}{extension}";

                var uploadsDirectory = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "uploads",
                    "equipment"
                );

                Directory.CreateDirectory(uploadsDirectory);

                var physicalFilePath = Path.Combine(
                    uploadsDirectory,
                    fileName
                );

                try
                {
                    await using var fileStream = new FileStream(
                        physicalFilePath,
                        FileMode.Create
                    );

                    await imageFile.CopyToAsync(fileStream);

                    equipment.ImagePath =
                        $"/uploads/equipment/{fileName}";

                    await _context.SaveChangesAsync();
                }
                catch
                {
                    // التراجع عن إضافة المعدة إذا فشل حفظ الصورة
                    _context.Equipment.Remove(equipment);
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError(
                        "imageFile",
                        _localizer["ImageSaveFailed"]
                    );

                    ViewData["CategoryId"] = new SelectList(
                        _context.Categories,
                        "Id",
                        "Name",
                        equipment.CategoryId
                    );

                    return View(equipment);
                }
            }

            TempData["SuccessMessage"] = _localizer[
            "EquipmentCreatedSuccessfully"].Value;

            return RedirectToAction(nameof(Index));
        }
        // GET: Equipment/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", equipment.CategoryId);
            return View(equipment);
        }

        // POST: Equipment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind(
        "Id,Name,SerialNumber,Description,PurchaseDate," +
        "PurchasePrice,IsAvailable,CategoryId"
    )]
    Equipment equipment,
    IFormFile? imageFile)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            var existingEquipment =
                await _context.Equipment.FindAsync(id);

            if (existingEquipment == null)
            {
                return NotFound();
            }

            const long maximumImageSize = 5 * 1024 * 1024;

            var allowedExtensions = new[]
            {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

            var allowedContentTypes = new[]
            {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

            // تنظيف الرقم التسلسلي
            equipment.SerialNumber =
                (equipment.SerialNumber ?? string.Empty).Trim();

            // منع استخدام رقم تسلسلي لمعدة أخرى
            if (!string.IsNullOrWhiteSpace(equipment.SerialNumber))
            {
                var serialNumberExists =
                    await _context.Equipment.AnyAsync(
                        e => e.SerialNumber == equipment.SerialNumber &&
                             e.Id != id
                    );

                if (serialNumberExists)
                {
                    ModelState.AddModelError(
                        nameof(equipment.SerialNumber),
                        "يوجد جهاز آخر يستخدم هذا الرقم التسلسلي."
                    );
                }
            }

            // التحقق من الصورة الجديدة
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(
                    imageFile.FileName
                ).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "يُسمح فقط بصور JPG وPNG وWEBP."
                    );
                }

                var contentType =
                    imageFile.ContentType.ToLowerInvariant();

                if (!allowedContentTypes.Contains(contentType))
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "نوع ملف الصورة غير صالح."
                    );
                }

                if (imageFile.Length > maximumImageSize)
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "يجب ألا يزيد حجم الصورة عن 5 MB."
                    );
                }
            }

            if (equipment.PurchaseDate == default)
            {
                ModelState.Remove(
                    nameof(equipment.PurchaseDate)
                );

                ModelState.AddModelError(
                    nameof(equipment.PurchaseDate),
                    _localizer["PurchaseDateRequired"]
                );
            }

            if (!ModelState.IsValid)
            {
                // إعادة عرض الصورة الحالية
                equipment.ImagePath =
                    existingEquipment.ImagePath;

                ViewData["CategoryId"] = new SelectList(
                    _context.Categories,
                    "Id",
                    "Name",
                    equipment.CategoryId
                );

                return View(equipment);
            }

            // تعديل الحقول المطلوبة فقط
            existingEquipment.Name = equipment.Name;
            existingEquipment.SerialNumber =
                equipment.SerialNumber;
            existingEquipment.Description =
                equipment.Description;
            existingEquipment.PurchaseDate =
                equipment.PurchaseDate;
            existingEquipment.PurchasePrice =
                equipment.PurchasePrice;
            existingEquipment.IsAvailable =
                equipment.IsAvailable;
            existingEquipment.CategoryId =
                equipment.CategoryId;

            // تغيير الصورة فقط عند اختيار صورة جديدة
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(
                    imageFile.FileName
                ).ToLowerInvariant();

                var fileName =
                    $"{existingEquipment.Id}{extension}";

                var uploadsDirectory = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "uploads",
                    "equipment"
                );

                Directory.CreateDirectory(uploadsDirectory);

                var newPhysicalPath = Path.Combine(
                    uploadsDirectory,
                    fileName
                );

                var oldImagePath =
                    existingEquipment.ImagePath;

                try
                {
                    await using var fileStream = new FileStream(
                        newPhysicalPath,
                        FileMode.Create
                    );

                    await imageFile.CopyToAsync(fileStream);

                    existingEquipment.ImagePath =
                        $"/uploads/equipment/{fileName}";

                    // حذف الصورة القديمة إذا تغير الامتداد
                    if (!string.IsNullOrWhiteSpace(oldImagePath) &&
                        oldImagePath != existingEquipment.ImagePath)
                    {
                        var oldFileName =
                            Path.GetFileName(oldImagePath);

                        var oldPhysicalPath = Path.Combine(
                            uploadsDirectory,
                            oldFileName
                        );

                        if (System.IO.File.Exists(oldPhysicalPath))
                        {
                            System.IO.File.Delete(oldPhysicalPath);
                        }
                    }
                }
                catch
                {
                    equipment.ImagePath =
                        oldImagePath;

                    ModelState.AddModelError(
                        "imageFile",
                        _localizer["NewImageSaveFailed"]
                    );

                    ViewData["CategoryId"] = new SelectList(
                        _context.Categories,
                        "Id",
                        "Name",
                        equipment.CategoryId
                    );

                    return View(equipment);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["SuccessMessage"] = _localizer["EquipmentUpdatedSuccessfully"].Value;

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        // GET: Equipment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            // التحقق من وجود سجلات استعارة مرتبطة بالمعدة
            var hasBorrowingRecords =
                await _context.BorrowingRecords
                    .AnyAsync(b => b.EquipmentId == id);

            if (hasBorrowingRecords)
            {
                TempData["ErrorMessage"] = _localizer[
                "EquipmentHasBorrowingRecords"].Value;

                return RedirectToAction(nameof(Index));
            }

            // الاحتفاظ بمسار الصورة قبل حذف المعدة
            var imagePath = equipment.ImagePath;

            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();

            // حذف ملف الصورة بعد نجاح حذف المعدة
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                var fileName = Path.GetFileName(imagePath);

                var physicalImagePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "uploads",
                    "equipment",
                    fileName
                );

                if (System.IO.File.Exists(physicalImagePath))
                {
                    System.IO.File.Delete(physicalImagePath);
                }
            }

            TempData["SuccessMessage"] =
           _localizer["EquipmentDeletedSuccessfully"].Value;

            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }
    }
}
