using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsAndEnums.Enums;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.ViewModels
{
    public class TaskViewModel
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public TaskPriority Priority { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public ModelsAndEnums.Enums.TaskStatus Status { get; set; }
    }
}
