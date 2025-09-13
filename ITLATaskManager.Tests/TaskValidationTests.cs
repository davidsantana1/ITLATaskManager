using ITLATaskManager.Models;
using ITLATaskManager.Utils;
using Xunit;

namespace ITLATaskManager.Tests
{
    public class TaskValidationTests
    {
        [Fact]
        public void DefaultValidator_WithValidTask_ReturnsTrue()
        {
            var task = CreateValidTask();

            var result = TaskValidation.DefaultValidator(task);

            Assert.True(result);
        }

        [Fact]
        public void DefaultValidator_WithEmptyDescription_ReturnsFalse()
        {
            var task = CreateValidTask();
            task.Description = "";

            var result = TaskValidation.DefaultValidator(task);

            Assert.False(result);
        }

        [Fact]
        public void DefaultValidator_WithPastDueDate_ReturnsFalse()
        {
            var task = CreateValidTask();
            task.DueDate = DateTime.Now.AddDays(-1);

            var result = TaskValidation.DefaultValidator(task);

            Assert.False(result);
        }

        private static ToDoTask<string> CreateValidTask()
        {
            return new ToDoTask<string>
            {
                Title = "Valid Task",
                Description = "This is a valid task description",
                DueDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                AdditionalData = "Valid Data",
            };
        }
    }
}
