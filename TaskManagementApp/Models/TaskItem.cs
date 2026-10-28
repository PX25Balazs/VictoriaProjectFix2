using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.Models
{
    public enum TaskStatusEnum { Todo, InProgress, Done }
    public enum PriorityEnum { Low, Medium, High }

    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Todo;

        public PriorityEnum Priority { get; set; } = PriorityEnum.Medium;

        [Required(ErrorMessage = "Please specify the due date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public string? AssignedUserId { get; set; } // ID пользователя

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}