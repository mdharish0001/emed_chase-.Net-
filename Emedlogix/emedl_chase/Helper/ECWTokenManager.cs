using emedl_chase.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace emedl_chase.Helper
{
    // ═══════════════════════════════════════════════════════════════
    //  ECWTokenManager — Singleton, thread-safe token cache
    //
    //  ABOUT REFRESH TOKENS:
    //  ECW uses OAuth2 client_credentials grant (machine-to-machine).
    //  This grant does NOT issue refresh tokens — that is an OAuth2
    //  standard rule, not an ECW limitation.
    //  When the token expires, you simply re-call client_credentials
    //  with your client_id + client_secret to get a new access_token.
    //  This class does that automatically, 5 minutes before expiry.
    // ═══════════════════════════════════════════════════════════════

    public sealed class ECWTokenManager
    {
        // Singleton
        private static readonly ECWTokenManager _instance = new();
        public static ECWTokenManager Instance => _instance;
        private ECWTokenManager() { }

        private readonly SemaphoreSlim _lock = new(1, 1);
        private string _token;
        private DateTime _expiresAt = DateTime.MinValue;
        private ECWConfig _config;

        // Renew 5 min before actual expiry to never hit mid-request expiry
        private const int BufferSeconds = 300;

        /// <summary>Call once before your batch starts.</summary>
        public void SetConfig(ECWConfig config) => _config = config;

        /// <summary>
        /// Returns a valid token. Automatically renews if expired or
        /// within 5 minutes of expiry. Thread-safe for batch processing.
        /// </summary>
        public async Task<string> GetTokenAsync()
        {
            // Fast path — valid token, no lock needed
            if (!string.IsNullOrEmpty(_token) &&
                DateTime.UtcNow < _expiresAt.AddSeconds(-BufferSeconds))
                return _token;

            // Needs renewal — only one thread renews at a time
            await _lock.WaitAsync();
            try
            {
                // Re-check after lock (another thread may have already renewed)
                if (!string.IsNullOrEmpty(_token) &&
                    DateTime.UtcNow < _expiresAt.AddSeconds(-BufferSeconds))
                    return _token;

                Console.WriteLine(
                    $"[Token] Fetching new ECW token at {DateTime.Now:HH:mm:ss}...");

                var result = await ECWTokenHelper.GetECWTokenAsync(_config);
                _token = result;

                // ECW tokens expire in 3600s (1 hour) by default
                // If your ECWTokenHelper returns the full response with expires_in,
                // use that value. Otherwise default to 3600.
                _expiresAt = DateTime.UtcNow.AddSeconds(3600);

                Console.WriteLine(
                    $"[Token] Token valid until {_expiresAt.ToLocalTime():HH:mm:ss} " +
                    $"(renews at " +
                    $"{_expiresAt.AddSeconds(-BufferSeconds).ToLocalTime():HH:mm:ss})");

                return _token;
            }
            finally
            {
                _lock.Release();
            }
        }

        public string CurrentToken => _token;
        public DateTime ExpiresAt => _expiresAt;
        public TimeSpan TimeUntilExpiry => _expiresAt - DateTime.UtcNow;
        public bool IsValid =>
            !string.IsNullOrEmpty(_token) && DateTime.UtcNow < _expiresAt;
    }
}