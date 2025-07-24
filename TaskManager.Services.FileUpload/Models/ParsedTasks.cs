
namespace TaskManager.Services.FileUpload.Models
{
    public class ParsedTask
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //public string State { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public string Type { get; set; } = "general";
        public string Priority { get; set; } = "Low";
        public string Assign { get; set; } = string.Empty;

        public DateTime? Duedate { get; set; }
        //public int UserId { get; set; }
    }
}
