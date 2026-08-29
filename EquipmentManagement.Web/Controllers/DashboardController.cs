using EquipmentManagement.Infrastructure.Data;
using EquipmentManagement.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var viewModel = new DashboardViewModel
        {
            TotalEquipment =
                await _context.Equipment.CountAsync(),

            AvailableEquipment =
                await _context.Equipment
                    .CountAsync(e => e.IsAvailable),

            BorrowedEquipment =
                await _context.Equipment
                    .CountAsync(e => !e.IsAvailable),

            TotalEmployees =
                await _context.Employees.CountAsync(),

            ActiveBorrowings =
                await _context.BorrowingRecords
                    .CountAsync(b => !b.IsReturned),

            OverdueBorrowings =
                await _context.BorrowingRecords
                    .CountAsync(b =>
                        !b.IsReturned &&
                        b.ExpectedReturnDate < today),

            RecentBorrowings =
                await _context.BorrowingRecords
                    .AsNoTracking()
                    .Include(b => b.Equipment)
                    .Include(b => b.Employee)
                    .Where(b => !b.IsReturned)
                    .OrderByDescending(b => b.BorrowDate)
                    .Take(5)
                    .ToListAsync()
        };

        return View(viewModel);
    }


}