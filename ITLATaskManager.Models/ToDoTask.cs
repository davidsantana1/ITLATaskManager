using System.Text.Json.Serialization;

namespace ITLATaskManager.Models
{
    public class ToDoTask<T>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public T AdditionalData { get; set; }
        [JsonIgnore]
        public Func<DateTime, int> DaysRemaining => currentDate =>
        {
            var daysLeft = (DueDate - currentDate).Days;
            return daysLeft < 0 ? 0 : daysLeft;
        };
    }
}
