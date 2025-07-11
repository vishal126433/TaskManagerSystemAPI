﻿namespace AuthService.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime? Duedate { get; set; }  
        public string State { get; set; } = string.Empty;

        public string Status { get; set; } = "new";

        public int UserId { get; set; }  // <-- Add this

    }
}
