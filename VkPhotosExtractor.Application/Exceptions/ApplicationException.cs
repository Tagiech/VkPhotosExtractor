namespace VkPhotosExtractor.Application.Exceptions;

public abstract class ApplicationException<T> : Exception where T : Enum
{
    public abstract int StatusCode { get; }
    public abstract T ErrorCode { get; }
    protected ApplicationException(string message, Exception? innerException = null)
        : base(message, innerException)
    { }
}