using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Services.Tasks.DueDateChecker;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DueDateCheckerController : ControllerBase
    {
        private readonly ITaskStateService _taskStateService;

        public DueDateCheckerController(ITaskStateService taskStateService)
        {
            _taskStateService = taskStateService;
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunDueDateCheck(CancellationToken cancellationToken)
        {
            await _taskStateService.UpdateTaskStatesAsync(cancellationToken);
            return Ok(new { message = "Task due dates checked and updated." });
        }
    }
}
