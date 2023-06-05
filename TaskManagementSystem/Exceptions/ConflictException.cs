using System.Net;

namespace TaskManagementSystem.Exceptions
{
    public class ConflictException : CustomException
    {
        public ConflictException(string message)
            : base(message, null, HttpStatusCode.Conflict) { }
    }
}