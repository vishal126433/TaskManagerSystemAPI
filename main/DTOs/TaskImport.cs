using System.Text.Json.Serialization;

namespace TaskManager.DTOs
{
    public class TaskImport
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string DueDate { get; set; }  // incoming string like "3/3/25"
        public string Status { get; set; }

        [JsonPropertyName("assign")]
        public string AssignTo { get; set; }  // string ID from select

        public string Type { get; set; }
        public string Priority { get; set; }
    }
}





