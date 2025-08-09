namespace ITLATaskManager.Models
{
    public class ToDoTaskFactory
    {
        public static ToDoTask<string> CreateHighPriorityTask(string title, string description)
        {
            return new ToDoTask<string>
            {
                Title = title,
                Description = description,
                Status = "Pending",
                DueDate = DateTime.Now.AddDays(1),
                AdditionalData = "High Priority"
            };
        }

        public static ToDoTask<string> CreateLowPriorityTask(string title, string description)
        {
            return new ToDoTask<string>
            {
                Title = title,
                Description = description,
                Status = "Pending",
                DueDate = DateTime.Now.AddDays(7),
                AdditionalData = "Low Priority"
            };
        }
    }
}
