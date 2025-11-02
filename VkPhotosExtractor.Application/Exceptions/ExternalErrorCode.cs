namespace VkPhotosExtractor.Application.Exceptions;

public enum ExternalErrorCode
{
    NetworkError,
    
    VkApiUnauthorized,
    VkApiUnavailable,
    
    VkIdUnavailable,
    VkIdAccessDenied,
    VkIdInvalidToken,
    VkIdRateLimitExceeded,
    VkIdApplicationRejected,
    VkIdStateMismatch,
    VkIdUnexpectedResponse
    
}