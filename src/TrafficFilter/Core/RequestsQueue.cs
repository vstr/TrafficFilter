using System;
using System.Collections.Generic;

namespace TrafficFilter.Core
{
    public class RequestsQueue : LinkedList<DateTime>
    {
        private readonly int _requestCountLimit;
        private readonly TimeSpan _expiresIn;

        public RequestsQueue(int requestCountLimit, TimeSpan expiresIn)
        {
            _requestCountLimit = requestCountLimit;
            _expiresIn = expiresIn;
        }

        public bool IsFull()
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var dateTimeUtcExpired = dateTimeUtcNow - _expiresIn;

            AddFirst(dateTimeUtcNow);

            while (Last.Value < dateTimeUtcExpired)
            {
                RemoveLast();
            }

            return Count > _requestCountLimit;
        }
    }
}