public interface ITaskService
{
    Task<IEnumerable<ModelsAndEnums.Models.Task>> GetTasks();
    Task<ModelsAndEnums.Models.Task> GetTaskById(int taskId);
    Task<ModelsAndEnums.Models.Task> CreateTask(ModelsAndEnums.Models.Task task);
    Task<ModelsAndEnums.Models.Task> UpdateTask(int taskId, ModelsAndEnums.Models.Task task);
    Task<bool> DeleteTask(int taskId);
}