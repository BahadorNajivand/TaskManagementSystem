using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public Enums.TaskStatus Status { get; set; }

        public int? AssigneeId { get; set; }

        [ForeignKey("AssigneeId")]
        public User Assignee { get; set; }
    }
}
