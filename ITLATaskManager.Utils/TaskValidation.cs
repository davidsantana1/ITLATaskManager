using ITLATaskManager.Models;

namespace ITLATaskManager.Utils
{
    public class TaskValidation
    {
        public delegate bool ValidateTask(ToDoTask<string> task);

        public static ValidateTask DefaultValidator = task => !string.IsNullOrWhiteSpace(task.Description) && task.DueDate > DateTime.Now;
    }
}
