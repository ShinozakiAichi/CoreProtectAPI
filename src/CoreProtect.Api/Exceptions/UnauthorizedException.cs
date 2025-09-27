using System.Net;

namespace CoreProtect.Api.Exceptions;

public sealed class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message) : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
