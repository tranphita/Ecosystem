using Ecosystem.Application.Abstractions.Identity;
using Ecosystem.Application.Features.Authentication.DTOs;
using Ecosystem.Shared;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ecosystem.Infrastructure.Identity;

/// <summary>
/// Service for authenticating users against external Identity Provider using ROPC flow.
/// </summary>
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IdpSettings _idpSettings;

    public AuthenticationService(HttpClient httpClient, IOptions<IdpSettings> idpSettings)
    {
        _httpClient = httpClient;
        _idpSettings = idpSettings.Value;
        _httpClient.BaseAddress = new Uri(_idpSettings.BaseUrl);
    }

    public async Task<Result<IdpTokenResponse>> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Prepare ROPC token request
            var tokenRequestData = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = username,
                ["password"] = password,
                ["client_id"] = _idpSettings.ClientId,
                ["client_secret"] = _idpSettings.ClientSecret,
                ["scope"] = _idpSettings.Scope
            };

            var tokenRequestContent = new FormUrlEncodedContent(tokenRequestData);

            // Make token request to IDP
            var response = await _httpClient.PostAsync("/connect/token", tokenRequestContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result<IdpTokenResponse>.Failure(
                    new Error("Authentication.Failed", $"Failed to authenticate with IDP: {errorContent}"));
            }

            // Parse token response
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);

            if (tokenResponse == null)
            {
                return Result<IdpTokenResponse>.Failure(
                    new Error("Authentication.InvalidResponse", "Invalid token response from IDP"));
            }

            var idpTokenResponse = new IdpTokenResponse(
                tokenResponse.AccessToken,
                tokenResponse.RefreshToken,
                tokenResponse.TokenType,
                tokenResponse.ExpiresIn);

            return Result<IdpTokenResponse>.Success(idpTokenResponse);
        }
        catch (OperationCanceledException)
        {
            // Let the pipeline honour the cancellation
            throw;
        }
        catch (Exception ex)
        {
            return Result<IdpTokenResponse>.Failure(
                new Error("Authentication.Error", $"Error during authentication: {ex.Message}"));
        }
    }

    public async Task<Result<UserIdentityInfo>> GetUserInfoAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Prepare user info request with access token
            using var request = new HttpRequestMessage(HttpMethod.Get, _idpSettings.UserInfoEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result<UserIdentityInfo>.Failure(
                    new Error("UserInfo.Failed", $"Failed to get user info from IDP: {errorContent}"));
            }

            // Parse user info response
            var userInfoResponse = await response.Content.ReadFromJsonAsync<UserInfoResponse>(cancellationToken: cancellationToken);

            if (userInfoResponse == null)
            {
                return Result<UserIdentityInfo>.Failure(
                    new Error("UserInfo.InvalidResponse", "Invalid user info response from IDP"));
            }

            if (string.IsNullOrWhiteSpace(userInfoResponse.Email))
            {
                using var profileRequest = new HttpRequestMessage(HttpMethod.Get, "/api/identity/my-profile");
                profileRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var profileResponse = await _httpClient.SendAsync(profileRequest, cancellationToken);

                if (!profileResponse.IsSuccessStatusCode)
                {
                    var errorContent = await profileResponse.Content.ReadAsStringAsync(cancellationToken);
                    return Result<UserIdentityInfo>.Failure(
                        new Error("UserProfile.Failed", $"Failed to get user profile from IDP: {errorContent}"));
                }

                var profileInfo = await profileResponse.Content.ReadFromJsonAsync<MyProfileResponse>(cancellationToken: cancellationToken);

                if (profileInfo == null || string.IsNullOrWhiteSpace(profileInfo.Email))
                {
                    return Result<UserIdentityInfo>.Failure(
                        new Error("UserProfile.InvalidResponse", "Invalid user profile response from IDP"));
                }

                // Map MyProfileResponse to UserIdentityInfo
                var userIdentityInfoFromProfile = new UserIdentityInfo(
                    userInfoResponse.Sub,
                    profileInfo.UserName,
                    profileInfo.Email,
                    profileInfo.Name,
                    profileInfo.Surname);

                return Result<UserIdentityInfo>.Success(userIdentityInfoFromProfile);
            }

            var userIdentityInfo = new UserIdentityInfo(
                userInfoResponse.Sub,
                userInfoResponse.PreferredUsername ?? userInfoResponse.Email,
                userInfoResponse.Email,
                userInfoResponse.GivenName,
                userInfoResponse.FamilyName);

            return Result<UserIdentityInfo>.Success(userIdentityInfo);
        }
        catch (Exception ex)
        {
            return Result<UserIdentityInfo>.Failure(
                new Error("UserInfo.Error", $"Error getting user info: {ex.Message}"));
        }
    }

    #region Private DTOs for IDP Communication

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);

    private sealed record UserInfoResponse(
        [property: JsonPropertyName("sub")] Guid Sub,
        [property: JsonPropertyName("preferred_username")] string? PreferredUsername,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("given_name")] string GivenName,
        [property: JsonPropertyName("family_name")] string FamilyName);

    private sealed record MyProfileResponse(
        [property: JsonPropertyName("userName")] string UserName,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("surname")] string Surname);

    #endregion
}