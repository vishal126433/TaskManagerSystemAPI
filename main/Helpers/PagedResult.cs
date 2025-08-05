namespace TaskManager.Helpers // Or leave this blank if you want to avoid namespace issues
{
    public class PagedResult<T>
    {
        public List<T> Tasks { get; set; }
        public int TotalCount { get; set; }
    }
}
