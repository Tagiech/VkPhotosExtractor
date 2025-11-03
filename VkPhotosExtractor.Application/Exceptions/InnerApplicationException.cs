namespace VkPhotosExtractor.Application.Exceptions;

public sealed class InnerApplicationException : ApplicationException<InnerErrorCode>
{
    public override int StatusCode { get; }
    public override InnerErrorCode ErrorCode { get; }

    public InnerApplicationException(string message,
        InnerErrorCode errorCode,
        int statusCode,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}