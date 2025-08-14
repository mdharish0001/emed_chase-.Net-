using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using emedl_chase.Model;
public class ECWTokenHelper
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static readonly ConcurrentDictionary<string, TokenCacheItem> TOKEN_CACHE = new();

    private class TokenCacheItem
    {
        public string Token { get; set; }
        public long ExpiresAt { get; set; }
    }

    public static async Task<string> GetECWTokenAsync(ECWConfig cred)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Check token cache
        if (TOKEN_CACHE.TryGetValue(cred.client_id, out var cached) && now < cached.ExpiresAt)
        {
            return cached.Token;
        }

        string jwtToken = GenerateEcwJwt(cred);

        var tokenPayload = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
            { "client_assertion", jwtToken },
            { "scope", cred.scope }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, cred.token_url)
        {
            Content = new FormUrlEncodedContent(tokenPayload)
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        try
        {
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var authData = System.Text.Json.JsonDocument.Parse(json).RootElement;

            if (!authData.TryGetProperty("access_token", out var tokenElement))
            {
                throw new Exception("Access token missing: " + json);
            }

            string token = tokenElement.GetString();

            // Cache token for 4 minutes
            TOKEN_CACHE[cred.client_id] = new TokenCacheItem
            {
                Token = token,
                ExpiresAt = now + 240
            };

            return token;
        }
        catch (HttpRequestException e)
        {
            throw new Exception($"Token request failed: {e.Message}");
        }
        catch (Exception e)
        {
            throw new Exception($"Token generation error: {e.Message}");
        }
    }

    private static string GenerateEcwJwt(ECWConfig cred)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = now + 300;

        var rsa = RSA.Create();
        string privateKeyPem = System.IO.File.ReadAllText(cred.private_key_path);
        rsa.ImportFromPem(privateKeyPem.ToCharArray());

        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa)
            {
                KeyId = cred.kid
            },
            SecurityAlgorithms.RsaSha384
        );

        var header = new JwtHeader(signingCredentials);
        header["kid"] = cred.kid;
        header["jku"] = cred.jku;

        var payload = new JwtPayload
        {
            { "iss", cred.client_id },
            { "sub", cred.client_id },
            { "aud", cred.token_url },
            { "exp", exp },
            { "iat", now },
            { "jti", now.ToString() }
        };

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
