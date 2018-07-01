using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dogey
{
    public class RatelimitService
    {
        public IEnumerable<RatelimitInfo> Ratelimits => _ratelimits.Values;

        private readonly ConcurrentDictionary<string, RatelimitInfo> _ratelimits;
        private readonly ILogger<RatelimitService> _logger;

        public RatelimitService(ILogger<RatelimitService> logger)
        {
            _ratelimits = new ConcurrentDictionary<string, RatelimitInfo>();
            _logger = logger;
        }

        public RatelimitInfo GetInfo(string serviceName)
        {
            if (_ratelimits.TryGetValue(serviceName, out RatelimitInfo value))
                return value;
            return null;
        }
        public RatelimitInfo CreateInfo(string serviceName, int? requestLimit = null, int? resetInterval = null)
        {
            var info = new RatelimitInfo(requestLimit, resetInterval);
            if (_ratelimits.TryAdd(serviceName, info))
                return info;
            _logger.LogWarning("`{serviceName}` already has a ratelimiter activated", serviceName);
            return null;
        }
        public RatelimitInfo GetOrCreateInfo(string serviceName, int? requestLimit = null, int? resetInterval = null)
        {
            if (_ratelimits.TryGetValue(serviceName, out RatelimitInfo value))
                return value;
            return CreateInfo(serviceName, requestLimit, resetInterval);
        }

        public bool IsRatelimited(string serviceName)
        {
            var info = GetInfo(serviceName);
            return info != null ? info.IsRatelimited() : false;
        }
    }

    public class RatelimitInfo
    {
        public const int DefaultRequestLimit = 30;
        public const int DefaultResetInterval = 30; // Minutes
        public int RequestLimit => _requestLimit;
        public int RequestsRemaining => _requestsRemaining;
        public TimeSpan ResetInterval => TimeSpan.FromMinutes(_resetInterval);
        public DateTime ResetAt => _resetAt;

        private int _requestLimit;
        private int _resetInterval;
        private int _requestsRemaining;
        private DateTime _resetAt;
        
        public RatelimitInfo(int? requestLimit = null, int? resetInterval = null)
        {
            _requestLimit = requestLimit ?? DefaultRequestLimit;
            _resetInterval = resetInterval ?? DefaultResetInterval;
            _resetAt = DateTime.MinValue;
        }

        public void SetRequestLimit(int? requestLimit = null)
        {
            _requestLimit = requestLimit ?? DefaultRequestLimit;
        }
        public void SetRequestInterval(int? resetInterval = null)
        {
            _resetInterval = resetInterval ?? DefaultResetInterval;
        }

        public bool IsRatelimited()
        {
            if (_resetAt <= DateTime.UtcNow)
            {
                _resetAt = DateTime.UtcNow.AddMinutes(_resetInterval);
                _requestsRemaining = _requestLimit;
            }

            if (_requestsRemaining == 0) return true;
            _requestsRemaining--;
            return false;
        }
    }
}
