public class ParseResult
{
    public bool Success { get; set; }
    public List<TaskItem> ParsedTasks { get; set; }
    public string ErrorMessage { get; set; }
}
