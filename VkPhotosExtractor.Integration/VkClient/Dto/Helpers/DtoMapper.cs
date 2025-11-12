using System.Globalization;
using VkPhotosExtractor.Application.Auth.Models;
using VkPhotosExtractor.Application.Exceptions;

namespace VkPhotosExtractor.Integration.VkClient.Dto.Helpers;

public static class DtoMapper
{
    public static ExternalErrorCode ToExternalErrorCode(this VkError error)
    {
        return error switch
        {
            VkError.AccessDenied => ExternalErrorCode.VkIdAccessDenied,
            VkError.InvalidToken => ExternalErrorCode.VkIdInvalidToken,
            VkError.ServerError => ExternalErrorCode.VkIdUnavailable,
            VkError.SlowDown => ExternalErrorCode.VkIdRateLimitExceeded,
            VkError.TemporarilyUnavailable => ExternalErrorCode.VkIdUnavailable,
            VkError.InvalidClient => ExternalErrorCode.VkIdApplicationRejected,
            _ => ExternalErrorCode.VkIdUnavailable
        };
    }
    
    public static RefreshTokenResponse ToRefreshTokenResponse(this VkRefreshTokenResponseDto dto)
    {
        return new RefreshTokenResponse(dto.RefreshToken,
            dto.AccessToken,
            dto.TokenType,
            TimeSpan.FromSeconds(dto.ExpiresIn),
            dto.UserId,
            string.IsNullOrEmpty(dto.Scope) ? [] : dto.Scope.Split(" "));
    }

    public static AuthResponse ToAuthResponse(this VkAuthResponseDto dto)
    {
        return new AuthResponse(dto.AccessToken,
            dto.RefreshToken,
            dto.IdToken,
            TimeSpan.FromSeconds(dto.ExpiresIn),
            dto.UserId,
            string.IsNullOrEmpty(dto.Scope) ? [] : dto.Scope.Split(" ")
        );
    }
    
    public static UserInfo ToUserInfo(this VkUserInfoDto dto, Guid userId)
    {
        var externalUserId = int.TryParse(dto.UserId, out var userIdInt) ? userIdInt : 0;
        var birthday = DateTime.ParseExact(dto.Birthday, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        return new UserInfo(userId,
            externalUserId,
            dto.FirstName,
            dto.LastName,
            dto.PhotoUrl ?? "",
            (Sex)dto.Sex,
            birthday);
    }
}