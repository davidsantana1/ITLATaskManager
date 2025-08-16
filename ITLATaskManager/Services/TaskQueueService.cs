using System.Reactive.Linq;
using ITLATaskManager.DataAccess.Data;
using ITLATaskManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ITLATaskManagerAPI.Services
{
    public interface ITaskQueueService
    {
        void StartProcessing();
        void StopProcessing();
        int QueueLength { get; }
        bool IsProcessing { get; }
    }

    public class TaskQueueService : ITaskQueueService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private bool _isProcessing;
        private readonly object _lockObject = new object();
        private IDisposable _queueProcessor;

        public TaskQueueService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _isProcessing = false;
        }

        public int QueueLength
        {
            get
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                return context.ToDoTasks.Count(t => t.Status == "Pending");
            }
        }

        public bool IsProcessing => _isProcessing;

        public void StartProcessing()
        {
            if (_queueProcessor == null)
            {
                _queueProcessor = Observable
                    .Interval(TimeSpan.FromMilliseconds(1000))
                    .Subscribe(_ => ProcessNextTask());
            }
        }

        public void StopProcessing()
        {
            _queueProcessor?.Dispose();
            _queueProcessor = null;
        }

        private async void ProcessNextTask()
        {
            if (_isProcessing)
                return;

            lock (_lockObject)
            {
                if (_isProcessing)
                    return;
                _isProcessing = true;
            }

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var pendingTask = await context
                    .ToDoTasks.Where(t => t.Status == "Pending")
                    .OrderBy(t => t.Id)
                    .FirstOrDefaultAsync();

                if (pendingTask != null)
                {
                    await ProcessTaskAsync(pendingTask, context);
                }
            }
            finally
            {
                lock (_lockObject)
                {
                    _isProcessing = false;
                }
            }
        }

        private async Task ProcessTaskAsync(ToDoTask<string> task, ApplicationDbContext context)
        {
            task.Status = "Processing";
            await context.SaveChangesAsync();

            var time = task.AdditionalData == "High Priority" ? 2000 : 5000;
            await Task.Delay(time);

            task.Status = "Completed";
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _queueProcessor?.Dispose();
        }
    }
}
