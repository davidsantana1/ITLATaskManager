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
    }
}
