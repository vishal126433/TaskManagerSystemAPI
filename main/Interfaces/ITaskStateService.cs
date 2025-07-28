namespace TaskManager.Interfaces
{
    public interface ITaskStateService
    {
        Task UpdateTaskStatesAsync(CancellationToken cancellationToken);

    }
}
