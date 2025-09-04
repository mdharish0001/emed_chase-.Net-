using emedl_chase.Model;
using emedl_chase.Option;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace emedl_chase.Helper
{
    public class EPICTokenHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();
        //private static readonly ConcurrentDictionary<string, TokenCacheItem> TOKEN_CACHE = new();



        public static async Task<string> GetAccessTokenAsync(string privatepath, string  clinetid, string tokenurl)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Check token cache
    

            string jwtToken = GenerateEcwJwt(privatepath,clinetid,tokenurl);

            var tokenPayload = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
            { "client_assertion", jwtToken },
           
        };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenurl)
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

                return token;
            }
            catch (HttpRequestException e)
            {
                return null;
                throw new Exception($"Token request failed: {e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"Token generation error: {e.Message}");
            }


        }
        public static string GenerateEcwJwt(string private_key_path, string  client_id, string token_url)
        {


            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var exp = now + 300;

            var claims = new Dictionary<string, object>
        {
            { "iss", client_id },
            { "sub", client_id },
            { "aud", token_url },
            { "iat", now },
            { "exp", now + 300 }, // 5 minutes
            { "jti", Guid.NewGuid().ToString() }
        };
            var rsa = RSA.Create();
            string privateKeyPem = System.IO.File.ReadAllText(private_key_path);
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            var securityKey = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha384);

            var handler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claims,
                SigningCredentials = credentials
            };

            var token = handler.CreateJwtSecurityToken(tokenDescriptor);
            string clientAssertion = handler.WriteToken(token);

            return clientAssertion;

            
        }
    }
}
