using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class TaskManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 显示任务列表
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            var tasks = from t in _context.Tasks
                       select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    tasks = tasks.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    tasks = tasks.OrderBy(s => s.DueDate);
                    break;
                case "date_desc":
                    tasks = tasks.OrderByDescending(s => s.DueDate);
                    break;
                default:
                    tasks = tasks.OrderBy(s => s.Title);
                    break;
            }

            return View(await tasks.Include(t => t.Project).ToListAsync());
        }

        // 创建任务
        public IActionResult Create()
        {
            ViewBag.Projects = _context.Projects.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,Priority,ProjectId,RemindTime")] Models.Task task)
        {
            if (ModelState.IsValid)
            {
                task.Status = TaskStatus.Pending;
                task.CreatedAt = DateTime.Now;
                task.UpdatedAt = DateTime.Now;
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Projects = _context.Projects.ToList();
            return View(task);
        }

        // 编辑任务
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            ViewBag.Projects = _context.Projects.ToList();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,Priority,Status,ProjectId,RemindTime")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    task.UpdatedAt = DateTime.Now;
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Projects = _context.Projects.ToList();
            return View(task);
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
} 