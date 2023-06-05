using Microsoft.EntityFrameworkCore;
using ModelsAndEnums.Models;

public class TaskService : ITaskService
{
    private readonly WebApplicationDbContext _dbContext;

    public TaskService(WebApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ModelsAndEnums.Models.Task>> GetTasks()
    {
        return await _dbContext.Tasks.ToListAsync();
    }

    public async Task<ModelsAndEnums.Models.Task> GetTaskById(int taskId)
    {
        return await _dbContext.Tasks.FindAsync(taskId);
    }

    public async Task<ModelsAndEnums.Models.Task> CreateTask(ModelsAndEnums.Models.Task task)
    {
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();

        return task;
    }

    public async Task<ModelsAndEnums.Models.Task> UpdateTask(int taskId, ModelsAndEnums.Models.Task task)
    {
        var existingTask = await _dbContext.Tasks.FindAsync(taskId);

        if (existingTask == null)
        {
            return null;
        }

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.DueDate = task.DueDate;
        existingTask.Priority = task.Priority;
        existingTask.Status = task.Status;
        existingTask.AssigneeId = task.AssigneeId;

        await _dbContext.SaveChangesAsync();

        return existingTask;
    }

    public async Task<bool> DeleteTask(int taskId)
    {
        var existingTask = await _dbContext.Tasks.FindAsync(taskId);

        if (existingTask == null)
        {
            return false;
        }

        _dbContext.Tasks.Remove(existingTask);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
