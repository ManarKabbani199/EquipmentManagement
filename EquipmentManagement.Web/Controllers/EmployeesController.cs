using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Infrastructure.Data;
using EquipmentManagement.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
namespace EquipmentManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class EmployeesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    private readonly IStringLocalizer<SharedResource>
    _localizer;

    public EmployeesController(
    ApplicationDbContext context,
    UserManager<IdentityUser> userManager,
    IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _userManager = userManager;
        _localizer = localizer;
    }

    // GET: Employees
    public async Task<IActionResult> Index()
    {
        return View(
            await _context.Employees.ToListAsync());
    }

    // GET: Employees/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(employee => employee.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // GET: Employees/Create
    public IActionResult Create()
    {
        return View();
    }

    private string LocalizeIdentityError(
    IdentityError error)
    {
        return error.Code switch
        {
            "DuplicateEmail" =>
                _localizer["EmailAlreadyUsed"],

            "DuplicateUserName" =>
                _localizer["EmailAlreadyUsed"],

            "InvalidEmail" =>
                _localizer["InvalidEmail"],

            "PasswordTooShort" =>
                _localizer["PasswordMinimumLength"],

            "PasswordRequiresDigit" =>
                _localizer["PasswordRequiresDigit"],

            "PasswordRequiresLower" =>
                _localizer["PasswordRequiresLowercase"],

            "PasswordRequiresUpper" =>
                _localizer["PasswordRequiresUppercase"],

            "PasswordRequiresNonAlphanumeric" =>
                _localizer["PasswordRequiresSymbol"],

            _ =>
                _localizer["IdentityOperationFailed"]
        };
    }

    // POST: Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateEmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // التأكد من عدم استخدام البريد سابقًا
        var existingIdentityUser =
            await _userManager.FindByEmailAsync(model.Email);

        var existingEmployee =
            await _context.Employees.AnyAsync(
                employee => employee.Email == model.Email);

        if (existingIdentityUser != null || existingEmployee)
        {
            ModelState.AddModelError(
            nameof(model.Email),
           _localizer["EmailAlreadyUsed"]
           );

            return View(model);
        }

        // إنشاء حساب تسجيل الدخول
        var identityUser = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(
            identityUser,
            model.Password);

        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(
                 string.Empty,
                LocalizeIdentityError(error)
                );
            }

            return View(model);
        }

        // منح المستخدم دور Employee
        var roleResult = await _userManager.AddToRoleAsync(
            identityUser,
            "Employee");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(identityUser);

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(
                string.Empty,
                LocalizeIdentityError(error));
            }

            return View(model);
        }

        // إنشاء سجل الموظف وربطه بحساب Identity
        var employee = new Employee
        {
            FullName = model.FullName,
            Email = model.Email,
            Department = model.Department,
            IdentityUserId = identityUser.Id
        };

        try
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
        catch
        {
            // التراجع عن إنشاء الحساب إذا فشل حفظ الموظف
            await _userManager.DeleteAsync(identityUser);

            ModelState.AddModelError(
                string.Empty,
                _localizer["EmployeeSaveFailed"]);

            return View(model);
        }

        TempData["SuccessMessage"] =
        _localizer[
        "EmployeeUpdatedSuccessfully"
         ].Value;

        return RedirectToAction(nameof(Index));
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee =
            await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,FullName,Email,Department")] Employee employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        var existingEmployee =
            await _context.Employees.FindAsync(id);

        if (existingEmployee == null)
        {
            return NotFound();
        }

        // تعديل البيانات مع عدم فقدان IdentityUserId
        existingEmployee.FullName = employee.FullName;
        existingEmployee.Department = employee.Department;

        // سنعالج تغيير البريد في حساب Identity لاحقًا
        // لذلك نحافظ حاليًا على البريد المرتبط بالحساب.
        employee.Email = existingEmployee.Email;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmployeeExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Employees/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(employee => employee.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        var hasBorrowingRecords =
            await _context.BorrowingRecords
                .AnyAsync(b => b.EmployeeId == id);

        if (hasBorrowingRecords)
        {
            TempData["ErrorMessage"] = _localizer["EmployeeHasBorrowingRecords"].Value;

            return RedirectToAction(nameof(Index));
        }

        IdentityUser? identityUser = null;

        if (!string.IsNullOrWhiteSpace(employee.IdentityUserId))
        {
            identityUser = await _userManager.FindByIdAsync(
                employee.IdentityUserId
            );
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            if (identityUser != null)
            {
                var deleteResult =
                    await _userManager.DeleteAsync(identityUser);

                if (!deleteResult.Succeeded)
                {
                    throw new InvalidOperationException(
                   _localizer[
                   "IdentityAccountDeleteFailed"
                   ]);
                }
            }

            await transaction.CommitAsync();

            TempData["SuccessMessage"] = _localizer["EmployeeDeletedSuccessfully"].Value;
        }
        catch
        {
            await transaction.RollbackAsync();

            TempData["ErrorMessage"] =
            _localizer[
            "EmployeeDeleteFailed"
            ].Value;
        }

        return RedirectToAction(nameof(Index));
    }

    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(
            employee => employee.Id == id);
    }
}