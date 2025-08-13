using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManager.Helpers
{
    public static class ResponseMessages
    {
        public static class Message
        {
            public const string TaskCreated = "Task created successfully.";
            public const string TaskUpdated = "Task updated successfully.";
            public const string TaskDeleted = "Task deleted successfully.";
            public const string TaskNotFound = "Task not found.";
            public const string TaskParsed = "Tasks parsed successfully";
            public const string TaskProcessed = "Tasks processed.";
            public const string Run = "Task due dates checked and updated.";
            public const string checkedandupdated = "Task due dates checked and updated..";
            public const string EmptySearchQuery = "Query cannot be empty."; 
            public const string EmptyTask = "No Task Provided.";
            public const string RetrievalFailed = "An error occurred while retrieving tasks.";
            public const string UserCreated = "User created successfully.";
            public const string UserUpdated = "User updated successfully.";
            public const string UserDeleted = "User deleted successfully.";
            public const string UserNotFound = "User not found."; 
            public const string NoRefreshToken = "Refresh Token not found."; 
            public const string RequiredFieldsMissing = "Required field not found.";
            public const string QueryEmpty = "Query cannot be empty.";
            public const string NoTasksProvided = "No tasks provided.";
            public const string InvalidDate = "Invalid date format for task '{0}'";
            public const string UserNotFoundAssigned = "User not found for AssignTo '{0}' in task '{1}'";
            public const string InvalidAssignTo = "Invalid AssignTo value '{0}' for task '{1}'";
            public const string TaskCreateFailed = "Failed to create task '{0}': {1}";
            public const string NoUserServiceInitialised = "UserService not initialized.";
            public const string DBcontextNull = "DbContext cannot be null.";
            public const string LoggerNull = "logger cannot be null.";
            public const string HttpClientNull = "HttpClient cannot be null.";
            public const string AllFields = "All fields are required.";
            public const string ErrorUserFetching = "Error fetching all users";    
            public const string RefreshingToken = "Refreshing token for provided refresh token";
            public const string TokenSucceed = "Refresh token succeeded";
            public const string TokenFailed = "Refresh token Failed";
            public const string TokenError = "Error refresh token";
            public const string FetchingUser = "Fetching all users";
            public const string UsersFetched = "Fetched {Count} users";
            public const string TokenFailedcode = "Refresh token failed with status code {StatusCode}";
            public const string AttemptCreateUser = "Attempting to create user with username '{Username}'";
            public const string CreateUserWithId = "User created successfully with Id {UserId}";
            public const string CreateUserError = "Error creating user with username '{Username}'";
            public const string AttemptUpdateUser = "Attempting to update user with Id {UserId}";
            public const string UpdateUserFailed = "Update failed: user with Id {UserId} not found";
            public const string UserUpdatedLog = "User with Id {UserId} updated successfully";
            public const string UserUpdatedErrorLog = "Error updating user with Id { UserId }";
            public const string AttemptDeleteUser = "Attempting to delete user with Id {UserId}";
            public const string DeleteUserFailed = "Delete failed: user with Id {UserId} not found";
            public const string UserDeletedLog = "User with Id {UserId} deleted successfully";
            public const string UserDeletedErrorLog = "Error deleting user with Id {UserId}";           
            public const string ToggleActiveStatus = "Toggling active status for user with Id {UserId}";
            public const string UserIdNotFound = "User with Id {UserId} not found";
            public const string UserIdActiveStatus = "User with Id {UserId} active status set to {IsActive}";
            public const string ToggleActiveStatusError = "Error toggling active status for user with Id {UserId}";
            public const string EmailServiceNull = "EmailService cannot be null.";
            public const string SettingsNull = "settings Value cannot be null.";
            public const string TaskStateService = "inside task state service";
            public const string SmtpOptionNull = "smtpOptions Value cannot be null.";
            public const string TaskCreationFailed = "Task creation failed: missing required fields for user {UserId}";
            public const string TaskCreationLog = "Task created successfully for user {UserId}: {@Task}";
            public const string TaskCreationFail = "Error creating task for user {UserId}";
            public const string SearchTask = "Searching tasks with query '{Query}'";
            public const string SearchTaskError = "Error searching tasks";
            public const string SearchTaskUserId = "Searching tasks for user {UserId} with query '{Query}'";
            public const string SearchTaskUserIdError = "Error searching tasks for user {UserId}";
            public const string FetchTaskError = "Error fetching all tasks";
            public const string FetchTaskCount = "Fetching all tasks (for count summary)";
            public const string FetchpaginatedTaskError = "Error fetching paginated tasks";
            public const string FetchpaginatedTask = "Fetching paginated tasks: Page {PageNumber}, PageSize {PageSize}";
            public const string FetchTask = "Fetching tasks for user {UserId}";
            public const string FetchTaskuseridError = "Error fetching tasks for user {UserId}";
            public const string FetchTypeListError = "Error fetching type list";
            public const string FetchTypeList = "Fetching type list";
            public const string FetchPriorityListError = "Error fetching priority list";
            public const string FetchPriorityList= "Fetching priority list";
            public const string FetchStatusListError = "Error fetching status list";
            public const string FetchStatusList = "Fetching status list";
            public const string TaskUpdatedId = "Task with Id {TaskId} updated successfully";
            public const string TaskUpdatedIdError = "Error updating task with Id {TaskId}";
            public const string TaskUpdateNotFound = "Update failed: task with Id {TaskId} not found";
            public const string AttemptTaskUpdateId = "Attempting to update task with Id {TaskId}";
            public const string TaskDeletedId = "Task with Id {TaskId} deleted successfully";
            public const string TaskDeletedIdError = "Error deleting task with Id {TaskId}";
            public const string TaskDeletedNotFound = "Delete failed: task with Id {TaskId} not found";
            public const string AttemptTaskDeletedId = "Attempting to delete task with Id {TaskId}";
            public const string AttemptTaskCreateId = "Attempting to create task for user {UserId}";


        }
    }
}
