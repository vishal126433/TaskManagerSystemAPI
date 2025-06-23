public class TaskItem
    {
    internal string dueDate;

    public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    public DateTime? Duedate { get; set; }  // ✅ Use DateTime


    public string Description { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public string Status { get; set; } = "pending";  // "pending" or "completed"
    public int UserId { get; internal set; }
}
