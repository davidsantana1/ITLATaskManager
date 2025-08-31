using System.Collections.Concurrent;
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
        void EnqueueTask(ToDoTask<string> task);
        int QueueLength { get; }
        bool IsProcessing { get; }
    }

    public class TaskQueueService : ITaskQueueService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ConcurrentQueue<ToDoTask<string>> _taskQueue;
        private bool _isProcessing;
        private readonly object _lockObject = new object();
        private IDisposable _queueProcessor;

        public TaskQueueService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _taskQueue = new ConcurrentQueue<ToDoTask<string>>();
            _isProcessing = false;
        }

        public int QueueLength => _taskQueue.Count;

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
                if (_taskQueue.TryDequeue(out var task))
                {
                    await ProcessTaskAsync(task);
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

        private async Task ProcessTaskAsync(ToDoTask<string> task)
        {
            var processingTime = task.AdditionalData == "High Priority" ? 2000 : 5000;
            await Task.Delay(processingTime);

            Console.WriteLine($"Task processed: {task.Title} - Priority: {task.AdditionalData}");
        }

        public void EnqueueTask(ToDoTask<string> task)
        {
            _taskQueue.Enqueue(task);
        }

        public void Dispose()
        {
            _queueProcessor?.Dispose();
        }
    }
}
