using System.Threading;
using System.Threading.Tasks;

namespace TaskManager.Services.Tasks.DueDateChecker
{
    public interface ITaskStateService
    {
        Task UpdateTaskStatesAsync(CancellationToken cancellationToken);
    }
}
