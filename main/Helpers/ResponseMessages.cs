using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        }
    }
}
