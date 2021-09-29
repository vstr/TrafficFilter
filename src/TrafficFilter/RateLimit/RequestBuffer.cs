using System;
using System.Collections.Generic;
using System.Linq;

namespace TrafficFilter.RateLimit
{
    public class RequestBuffer : LinkedList<RequestItem>
    {
        private readonly int _requestCountLimit;
        private readonly TimeSpan _expiresIn;

        public RequestBuffer(int requestCountLimit, TimeSpan expiresIn)
        {
            _requestCountLimit = requestCountLimit;
            _expiresIn = expiresIn;
        }

        public bool IsFull(string request)
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var dateTimeUtcExpired = dateTimeUtcNow - _expiresIn;
            var itemHashCode = request.GetHashCode();

            var requestItem = new RequestItem()
            {
                Created = dateTimeUtcNow,
                RequestHash = itemHashCode
            };

            AddFirst(requestItem);

            while (Last.Value.Created < dateTimeUtcExpired)
            {
                RemoveLast();
            }

            var reqCount = this.Count(i => i.RequestHash == itemHashCode);

            return reqCount > _requestCountLimit;
        }
    }
}
