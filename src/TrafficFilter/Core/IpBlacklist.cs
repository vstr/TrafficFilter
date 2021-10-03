using System;
using System.Collections.Concurrent;
using System.Net;

namespace TrafficFilter.Core
{
    public interface IIpBlacklist
    {
        void Add(IPAddress ipAddress, int blockSeconds);

        bool Contains(IPAddress ipAddress);

        bool IsInBlacklist(IPAddress ipAddress);
    }

    public class IpBlacklist : IIpBlacklist
    {
        private readonly ConcurrentDictionary<IPAddress, DateTime> _ipBlacklist = new ConcurrentDictionary<IPAddress, DateTime>();

        public void Add(IPAddress ipAddress, int blockSeconds)
        {
            if (ipAddress != null)
            {
                _ipBlacklist.TryAdd(ipAddress, DateTime.UtcNow.AddSeconds(blockSeconds));
            }
        }

        public bool Contains(IPAddress ipAddress)
        {
            if (ipAddress == null) { return false; }

            var result = _ipBlacklist.ContainsKey(ipAddress);
            return result;
        }

        public bool IsInBlacklist(IPAddress ipAddress)
        {
            if (ipAddress == null)
            {
                return true;
            }
            else if (_ipBlacklist.TryGetValue(ipAddress, out var value))
            {
                if (value < DateTime.UtcNow)
                {
                    _ipBlacklist.TryRemove(ipAddress, out var _);
                    return false;
                }
                return true;
            }

            return false;
        }
    }
}