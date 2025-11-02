namespace VkPhotosExtractor.Application.Auth;

public interface ISecurityStringProvider
{
    string GenerateRandomString(int length);
    (string state, string codeChallenge) GenerateSecurityStrings(int stateLength, int codeChallengeLength);
    string? GetCodeVerifier(string state);
    void ClearCodeVerifier(string state);
}