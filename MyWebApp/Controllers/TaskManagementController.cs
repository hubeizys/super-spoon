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
        public async Task<IActionResult> Index(
            string sortOrder, 
            string searchString, 
            int? projectId,
            Priority? priority,
            SpoonTaskStatus? status,
            bool? showOverdue,
            bool? showUpcoming)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPriority"] = priority;
            ViewData["CurrentStatus"] = status;
            ViewData["ShowOverdue"] = showOverdue;
            ViewData["ShowUpcoming"] = showUpcoming;

            var tasks = from t in _context.Tasks
                       select t;

            // 应用过滤条件
            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));
            }

            if (projectId.HasValue)
            {
                tasks = tasks.Where(t => t.ProjectId == projectId);
            }

            if (priority.HasValue)
            {
                tasks = tasks.Where(t => t.Priority == priority);
            }

            if (status.HasValue)
            {
                tasks = tasks.Where(t => t.Status == status);
            }

            if (showOverdue == true)
            {
                tasks = tasks.Where(t => t.DueDate < DateTime.Now);
            }

            if (showUpcoming == true)
            {
                var threeDaysFromNow = DateTime.Now.AddDays(3);
                tasks = tasks.Where(t => t.DueDate > DateTime.Now && t.DueDate < threeDaysFromNow);
            }

            // 应用排序
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

            ViewBag.Projects = await _context.Projects.ToListAsync();
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
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,Priority,ProjectId,RemindTime")] SpoonTaskModel task)
        {
            if (ModelState.IsValid)
            {
                task.Status = SpoonTaskStatus.Pending;
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,Priority,Status,ProjectId,RemindTime")] SpoonTaskModel task)
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

        // 删除任务
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 项目管理相关方法
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject([FromBody] SpoonProjectModel project)
        {
            if (ModelState.IsValid)
            {
                project.CreatedAt = DateTime.Now;
                _context.Add(project);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProject([FromBody] SpoonProjectModel project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProject = await _context.Projects.FindAsync(project.Id);
                    if (existingProject == null)
                    {
                        return NotFound();
                    }

                    existingProject.Name = project.Name;
                    existingProject.Description = project.Description;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            // 将关联的任务的ProjectId设置为null
            foreach (var task in project.Tasks)
            {
                task.ProjectId = null;
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
} 