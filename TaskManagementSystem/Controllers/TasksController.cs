using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelsAndEnums.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Task = ModelsAndEnums.Models.Task;
using TaskManagementSystem.ViewModels;

namespace TaskManagementSystem.Controllers
{
    public class TasksController : Controller
    {
        private readonly WebApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public int? _userId;

        public TasksController(WebApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
        }

        // GET: Tasks
        public async Task<IActionResult> Index(int? page, string sortOrder, string searchString)
        {
            // Retrieve the UserId from the session

            if (_userId ==  null) {
                return RedirectToAction("Index", "Home");
            }

            // Get tasks related to the user (Assignee)
            var tasksQuery = _context.Tasks.Include(t => t.Assignee).Where(t => t.AssigneeId == _userId);

            // Filtering
            if (!string.IsNullOrEmpty(searchString))
            {
                tasksQuery = tasksQuery.Where(t => t.Title.Contains(searchString) || t.Description.Contains(searchString));
            }

            // Sorting
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DueDateSortParam"] = sortOrder == "DueDate" ? "dueDate_desc" : "DueDate";
            ViewData["PrioritySortParam"] = sortOrder == "Priority" ? "priority_desc" : "Priority";
            ViewData["StatusSortParam"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["CurrentFilter"] = searchString;

            switch (sortOrder)
            {
                case "title_desc":
                    tasksQuery = tasksQuery.OrderByDescending(t => t.Title);
                    break;
                case "DueDate":
                    tasksQuery = tasksQuery.OrderBy(t => t.DueDate);
                    break;
                case "dueDate_desc":
                    tasksQuery = tasksQuery.OrderByDescending(t => t.DueDate);
                    break;
                case "Priority":
                    tasksQuery = tasksQuery.OrderBy(t => t.Priority);
                    break;
                case "priority_desc":
                    tasksQuery = tasksQuery.OrderByDescending(t => t.Priority);
                    break;
                case "Status":
                    tasksQuery = tasksQuery.OrderBy(t => t.Status);
                    break;
                case "status_desc":
                    tasksQuery = tasksQuery.OrderByDescending(t => t.Status);
                    break;
                default:
                    tasksQuery = tasksQuery.OrderBy(t => t.Title);
                    break;
            }

            // Pagination setup
            const int pageSize = 10; // Number of tasks per page
            var pageNumber = page ?? 1; // Current page number, defaults to 1

            // Retrieve the tasks for the current page
            var tasksPage = await tasksQuery.Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();

            // Count the total number of tasks
            var totalTasksCount = await tasksQuery.CountAsync();

            // Create a PaginatedList with the tasks for the current page
            var tasksPaginatedList = new PaginatedList<Task>(tasksPage, totalTasksCount, pageNumber, pageSize);

            return View(tasksPaginatedList);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["AssigneeId"] = _userId;
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,Title,Description,DueDate,Priority,Status,AssigneeId")] TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new Task {
                    Title = task.Title,
                    Description = task.Description,
                    AssigneeId = _userId,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    Status = task.Status
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(new TaskViewModel { 
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                TaskId = task.TaskId,
                DueDate = task.DueDate,
                Priority = task.Priority,
            });
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,Title,Description,DueDate,Priority,Status,AssigneeId")] TaskViewModel task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTask = _context.Tasks.Where(t => t.TaskId == task.TaskId).FirstOrDefault();

                    existingTask.Title = task.Title;
                    existingTask.Description = task.Description;
                    existingTask.AssigneeId = _userId;
                    existingTask.DueDate = task.DueDate;
                    existingTask.Priority = task.Priority;
                    existingTask.Status = task.Status;

                    _context.Update(existingTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.TaskId))
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
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'TaskManagementSystemContext.Task'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
          return (_context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}

public class PaginatedList<T>
{
    public List<T> Items { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        Items = items;
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
