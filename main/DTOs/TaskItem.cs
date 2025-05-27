﻿public class TaskItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "pending";  // "pending" or "completed"
    public int UserId { get; internal set; }
}
