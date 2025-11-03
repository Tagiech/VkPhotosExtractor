namespace VkPhotosExtractor.Application.Exceptions;

public enum ExternalErrorCode
{
    NetworkError,
    
    VkApiUnauthorized,
    VkApiUnavailable,
    
    VkIdBadGateway,
    VkIdUnavailable,
    VkIdAccessDenied,
    VkIdInvalidToken,
    VkIdRateLimitExceeded,
    VkIdApplicationRejected,
    VkIdStateMismatch,
    VkIdUnexpectedResponse
    
}