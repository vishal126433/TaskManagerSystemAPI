using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TaskManager.Helpers
{
    public static class ResponseMessages
    {
        public static class Task
        {
            public const string Created = "Task created successfully.";
            public const string Updated = "Task updated successfully.";
            public const string Deleted = "Task deleted successfully.";
            public const string NotFound = "Task not found.";
            public const string Parsed = "Tasks parsed successfully.";
            public const string Processed = "Tasks processed.";
            public const string Run = "Task due dates checked and updated.";
            public const string checkedandupdated = "Task due dates checked and updated..";
            public const string EmptySearchQuery = "Query cannot be empty."; 
            public const string EmptyTask = "No Task Provided.";


        }
        public static class User
        {
            public const string Created = "User created successfully.";
            public const string Updated = "User updated successfully.";
            public const string Deleted = "User deleted successfully.";
            public const string NotFound = "User not found."; 
            public const string NoRefreshToken = "Refresh Token not found."; 
            public const string RequiredFieldsMissing = "Required field not found.";

        }

        public static class Errors
        {
            public const string NotFound = "User not found.";

            public const string QueryEmpty = "Query cannot be empty.";
            public const string NoTasksProvided = "No tasks provided.";
            public const string InvalidDate = "Invalid date format for task '{0}'";
            public const string UserNotFound = "User not found for AssignTo '{0}' in task '{1}'";
            public const string InvalidAssignTo = "Invalid AssignTo value '{0}' for task '{1}'";
            public const string TaskCreateFailed = "Failed to create task '{0}': {1}";
        }
    }
}
