using System;
using System.Collections.Concurrent;

namespace TrafficFilter
{
    public interface IIpBlacklist
    {
        void Add(string ipAddress, int blockSeconds);

        bool Contains(string ipAddress);

        bool IsInBlacklist(string ipAddress);
    }

    public class IpBlacklist : IIpBlacklist
    {
        private readonly ConcurrentDictionary<string, DateTime> _ipBlacklist = new ConcurrentDictionary<string, DateTime>();

        public void Add(string ipAddress, int blockSeconds)
        {
            _ipBlacklist.TryAdd(ipAddress, DateTime.UtcNow.AddSeconds(blockSeconds));
        }

        public bool Contains(string ipAddress)
        {
            var result = _ipBlacklist.ContainsKey(ipAddress);
            return result;
        }

        public bool IsInBlacklist(string ipAddress)
        {
            if (ipAddress != null && _ipBlacklist.TryGetValue(ipAddress, out var value))
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
