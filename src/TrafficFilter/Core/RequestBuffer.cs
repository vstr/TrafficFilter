using System;
using System.Collections.Generic;
using System.Linq;

namespace TrafficFilter.Core
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

        public bool IsFull(string requestPath)
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var dateTimeUtcExpired = dateTimeUtcNow - _expiresIn;
            var itemHashCode = requestPath.GetHashCode();

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

    public class RequestItem
    {
        public DateTime Created { get; set; }

        public int RequestHash { get; set; }
    }
}