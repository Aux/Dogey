using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestEase;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey
{
    [Header("User-Agent", "Dogey (https://github.com/Aux/Dogey)")]
    public interface ITriviaApi
    {
        [Get("api_token.php?command=request")]
        Task<TriviaResponse> GetSessionTokenAsync();
        [Get("api_token.php?command=reset")]
        Task<TriviaResponse> ResetTokenAsync(string token);

        [Get("api.php?amount=1&encode=base64")]
        Task<TriviaResponse> GetQuestionAsync(string token, 
            TriviaCategory? category = null, 
            [Query(QuerySerializationMethod.ToString)]TriviaDifficulty? difficulty = null, 
            [Query(QuerySerializationMethod.ToString)]TriviaType? type = null);
    }

    public class TriviaApiService
    {
        public const string ApiUrl = "https://opentdb.com/";

        public static HttpClient GetClient()
            => new HttpClient { BaseAddress = new Uri(ApiUrl) };

        private readonly ILogger<TriviaApiService> _logger;
        private readonly RatelimitService _ratelimiter;
        private readonly IConfiguration _config;
        private readonly ITriviaApi _api;

        private ConcurrentDictionary<ulong, (string, DateTime)> _sessions;

        public TriviaApiService(ILogger<TriviaApiService> logger, IConfiguration config, RatelimitService ratelimiter, ITriviaApi api)
        {
            _logger = logger;
            _config = config;
            _ratelimiter = ratelimiter;
            _api = api;

            _sessions = new ConcurrentDictionary<ulong, (string, DateTime)>();
            _ratelimiter.CreateInfo(nameof(TriviaApiService), 100, 10);
        }

        private async Task<string> GetSessionTokenAsync(ulong guildId)
        {
            if (_sessions.TryGetValue(guildId, out (string Token, DateTime ExpiresAt) value))
            {
                if (value.ExpiresAt <= DateTime.UtcNow)
                {
                    var resetToken = await _api.ResetTokenAsync(value.Token);
                    var session = new ValueTuple<string, DateTime>(resetToken.Token, DateTime.UtcNow.AddHours(6));
                    _sessions.TryUpdate(guildId, session, value);
                    return session.Item1;
                }
                return value.Token;
            }

            var sessionToken = await _api.GetSessionTokenAsync();
            if (_sessions.TryAdd(guildId, new ValueTuple<string, DateTime>(sessionToken.Token, DateTime.UtcNow.AddHours(6))))
                return sessionToken.Token;
            return null;
        }

        public async Task<TriviaQuestion> GetQuestionAsync(ulong guildId, TriviaCategory? category = null, TriviaDifficulty? difficulty = null, TriviaType? type = null)
        {
            //if (_ratelimiter.IsRatelimited(nameof(TriviaApiService))) return null;

            try
            {
                var token = await GetSessionTokenAsync(guildId);
                var response = await _api.GetQuestionAsync(token, category, difficulty, type);
                return response.Results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return null;
        }
    }
}
