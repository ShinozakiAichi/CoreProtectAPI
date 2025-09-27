using System.Net;

namespace CoreProtect.Api.Exceptions;

public sealed class ValidationException : ApiException
{
    public ValidationException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}
