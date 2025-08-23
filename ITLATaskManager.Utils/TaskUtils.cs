using ITLATaskManager.DataAccess.Data;
using ITLATaskManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ITLATaskManager.Utils
{
    public static class TaskUtils
    {
        public static double CalculateCompletionPercentage(int totalTasks, int completedTasks)
        {
            if (totalTasks == 0)
                return 0;
            return (double)completedTasks / totalTasks * 100;
        }

        public static async Task<List<ToDoTask<string>>> FilterTasksByStatus(
            (ApplicationDbContext context, string status) parameters
        )
        {
            return await parameters
                .context.ToDoTasks.Where(t => t.Status == parameters.status)
                .ToListAsync();
        }
    }
}
