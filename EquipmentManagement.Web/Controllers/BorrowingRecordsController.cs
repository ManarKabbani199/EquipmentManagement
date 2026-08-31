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
using System.Security.Claims;
using Microsoft.Extensions.Localization;


namespace EquipmentManagement.Web.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class BorrowingRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public BorrowingRecordsController(
            ApplicationDbContext context,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        private async Task<Employee?> GetCurrentEmployeeAsync()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(identityUserId))
            {
                return null;
            }

            return await _context.Employees
                .FirstOrDefaultAsync(e => e.IdentityUserId == identityUserId);
        }

        // GET: BorrowingRecords
        public async Task<IActionResult> Index(
    string? search,
    string? status,
    int? employeeId,
      int pageNumber = 1)
        {
            var query = _context.BorrowingRecords
                .Include(b => b.Employee)
                .Include(b => b.Equipment)
                .AsQueryable();

            if (User.IsInRole("Admin"))
            {
                ViewData["EmployeeId"] = new SelectList(
                    await _context.Employees
                        .OrderBy(e => e.FullName)
                        .ToListAsync(),
                    "Id",
                    "FullName",
                    employeeId
                );

                if (employeeId.HasValue)
                {
                    query = query.Where(
                        b => b.EmployeeId == employeeId.Value
                    );
                }
            }
            else
            {
                var currentEmployee = await GetCurrentEmployeeAsync();

                if (currentEmployee == null)
                {
                    return Forbid();
                }

                query = query.Where(
                    b => b.EmployeeId == currentEmployee.Id
                );
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b =>
                    b.Equipment.Name.Contains(search) ||
                    b.Employee.FullName.Contains(search)
                );
            }

            switch (status)
            {
                case "active":
                    query = query.Where(b =>
                        !b.IsReturned &&
                        b.ExpectedReturnDate >= DateTime.Now
                    );
                    break;

                case "returned":
                    query = query.Where(b => b.IsReturned);
                    break;

                case "overdue":
                    query = query.Where(b =>
                        !b.IsReturned &&
                        b.ExpectedReturnDate < DateTime.Now
                    );
                    break;
            }

            ViewData["CurrentSearch"] = search;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentEmployeeId"] = employeeId;

            const int pageSize = 10;

            // حساب العدد الإجمالي للسجلات بعد البحث والتصفية
            var totalRecords = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                totalRecords / (double)pageSize
            );

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (totalPages > 0 && pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            // جلب سجلات الصفحة الحالية فقط
            var borrowingRecords = await query
                .OrderByDescending(b => b.BorrowDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["TotalRecords"] = totalRecords;

            return View(borrowingRecords);
        }
        // GET: BorrowingRecords/Details/5// GET: BorrowingRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = _context.BorrowingRecords
                .Include(b => b.Employee)
                .Include(b => b.Equipment)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var currentEmployee =
                    await GetCurrentEmployeeAsync();

                if (currentEmployee == null)
                {
                    return Forbid();
                }

                // منع الموظف من مشاهدة استعارات موظف آخر
                query = query.Where(
                    b => b.EmployeeId == currentEmployee.Id
                );
            }

            var borrowingRecord =
                await query.FirstOrDefaultAsync(
                    b => b.Id == id.Value
                );

            if (borrowingRecord == null)
            {
                return NotFound();
            }

            return View(borrowingRecord);
        }

        // GET: BorrowingRecords/Create
        public async Task<IActionResult> Create()
        {
            var availableEquipment =
                await _context.Equipment
                    .AsNoTracking()
                    .Where(e => e.IsAvailable)
                    .OrderBy(e => e.Name)
                    .ToListAsync();

            ViewData["EquipmentId"] = new SelectList(
                availableEquipment,
                "Id",
                "Name"
            );

            // إرسال رقم كل معدة ومسار صورتها إلى الصفحة
            ViewData["EquipmentImages"] =
                availableEquipment.ToDictionary(
                    e => e.Id,
                    e => e.ImagePath ?? string.Empty
                );

            if (User.IsInRole("Admin"))
            {
                ViewData["EmployeeId"] = new SelectList(
                    await _context.Employees
                        .AsNoTracking()
                        .OrderBy(e => e.FullName)
                        .ToListAsync(),
                    "Id",
                    "FullName"
                );
            }
            else
            {
                var currentEmployee =
                    await GetCurrentEmployeeAsync();

                if (currentEmployee == null)
                {
                    return Forbid();
                }
            }

            return View(new BorrowingRecord
            {
                BorrowDate = DateTime.Now,
                ExpectedReturnDate =
                    DateTime.Now.AddDays(7)
            });
        }
        // POST: BorrowingRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
   [Bind("EquipmentId,EmployeeId,ExpectedReturnDate")]
    BorrowingRecord borrowingRecord)
        {
            // منع التحقق من خصائص العلاقات؛ لأنها لا تأتي من النموذج
            ModelState.Remove(nameof(BorrowingRecord.Employee));
            ModelState.Remove(nameof(BorrowingRecord.Equipment));

            if (User.IsInRole("Admin"))
            {
                var employeeExists = await _context.Employees
                    .AnyAsync(e => e.Id == borrowingRecord.EmployeeId);

                if (!employeeExists)
                {
                    ModelState.AddModelError(
                        "EmployeeId",
                       _localizer["InvalidEmployeeSelection"]
                    );
                }
            }
            else
            {
                var currentEmployee = await GetCurrentEmployeeAsync();

                if (currentEmployee == null)
                {
                    return Forbid();
                }

                // الموظف لا يستطيع اختيار موظف آخر
                borrowingRecord.EmployeeId = currentEmployee.Id;
                ModelState.Remove(  nameof(BorrowingRecord.EmployeeId)  );
            }

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e =>
                    e.Id == borrowingRecord.EquipmentId &&
                    e.IsAvailable
                );

            if (equipment == null)
            {
                ModelState.AddModelError(
                    "EquipmentId",
                    _localizer["EquipmentUnavailable"]
                );
            }

            if (borrowingRecord.ExpectedReturnDate <= DateTime.Now)
            {
                ModelState.AddModelError(
                    "ExpectedReturnDate",
                    _localizer["ExpectedReturnDateFuture"]
                );
            }

            if (ModelState.IsValid && equipment != null)
            {
                borrowingRecord.BorrowDate = DateTime.Now;
                borrowingRecord.ActualReturnDate = null;
                borrowingRecord.IsReturned = false;

                equipment.IsAvailable = false;

                _context.BorrowingRecords.Add(borrowingRecord);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] =
               _localizer["BorrowingCreatedSuccessfully"].Value;


                return RedirectToAction(nameof(Index));
            }

            var availableEquipment =
    await _context.Equipment
        .AsNoTracking()
        .Where(e => e.IsAvailable)
        .OrderBy(e => e.Name)
        .ToListAsync();

            ViewData["EquipmentId"] = new SelectList(
                availableEquipment,
                "Id",
                "Name",
                borrowingRecord.EquipmentId
            );

            ViewData["EquipmentImages"] =
                availableEquipment.ToDictionary(
                    e => e.Id,
                    e => e.ImagePath ?? string.Empty
                );
            if (User.IsInRole("Admin"))
            {
                ViewData["EmployeeId"] = new SelectList(
                    await _context.Employees.ToListAsync(),
                    "Id",
                    "FullName",
                    borrowingRecord.EmployeeId
                );
            }

            return View(borrowingRecord);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnEquipment(int id)
        {
            var borrowingRecord = await _context.BorrowingRecords
                .Include(b => b.Equipment)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowingRecord == null)
            {
                return NotFound();
            }

            if (borrowingRecord.IsReturned)
            {
                TempData["ErrorMessage"] =
                _localizer["EquipmentAlreadyReturned"].Value;
                return RedirectToAction(nameof(Index));
            }

            borrowingRecord.IsReturned = true;
            borrowingRecord.ActualReturnDate = DateTime.Now;

            borrowingRecord.Equipment.IsAvailable = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["EquipmentReturnedSuccessfully"].Value;

            return RedirectToAction(nameof(Index));
        }

        private bool BorrowingRecordExists(int id)
        {
            return _context.BorrowingRecords.Any(e => e.Id == id);
        }
    }
}
