using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data;
using TaskManagementApp.Models;

[Authorize]
public class TasksController : Controller
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString, PriorityEnum? priorityFilter)
    {
        var tasksQuery = _context.Tasks.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            tasksQuery = tasksQuery.Where(t => t.Title.Contains(searchString) || t.Description.Contains(searchString));
        }

        if (priorityFilter.HasValue)
        {
            tasksQuery = tasksQuery.Where(t => t.Priority == priorityFilter.Value);
        }

        var tasks = await tasksQuery.ToListAsync();

        ViewBag.TotalTasks = tasks.Count;
        ViewBag.TodoCount = tasks.Count(t => t.Status == TaskStatusEnum.Todo);
        ViewBag.InProgressCount = tasks.Count(t => t.Status == TaskStatusEnum.InProgress);
        ViewBag.DoneCount = tasks.Count(t => t.Status == TaskStatusEnum.Done);
        ViewBag.CurrentSearch = searchString;
        ViewBag.CurrentPriority = priorityFilter;

        return View(tasks);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var taskItem = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id);
        if (taskItem == null) return NotFound();
        return View(taskItem);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,Status,Priority,DueDate,AssignedUserId")] TaskItem taskItem)
    {
        if (ModelState.IsValid)
        {
            taskItem.CreatedAt = DateTime.UtcNow;
            taskItem.UpdatedAt = DateTime.UtcNow;
            _context.Add(taskItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(taskItem);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var taskItem = await _context.Tasks.FindAsync(id);
        if (taskItem == null) return NotFound();
        return View(taskItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Priority,DueDate,AssignedUserId,CreatedAt")] TaskItem taskItem)
    {
        if (id != taskItem.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                taskItem.UpdatedAt = DateTime.UtcNow;
                _context.Update(taskItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tasks.Any(e => e.Id == taskItem.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(taskItem);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var taskItem = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id);
        if (taskItem == null) return NotFound();
        return View(taskItem);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var taskItem = await _context.Tasks.FindAsync(id);
        if (taskItem != null)
        {
            _context.Tasks.Remove(taskItem);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}