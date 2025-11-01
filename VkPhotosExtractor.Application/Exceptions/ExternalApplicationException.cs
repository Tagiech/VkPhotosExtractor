namespace VkPhotosExtractor.Application.Exceptions;

public sealed class ExternalApplicationException : ApplicationException<ExternalErrorCode>
{
    public override int StatusCode { get; }
    public override ExternalErrorCode ErrorCode { get; }
    
    public ExternalApplicationException(string message, ExternalErrorCode errorCode, int statusCode) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}