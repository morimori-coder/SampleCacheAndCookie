using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
namespace SampleCacheAndCookie
{
    public class InMemorySessionStore : ITicketStore
    {
        public InMemorySessionStore(IMemoryCache memoryCache) 
        {
            _memoryCache = memoryCache;
        }
        private readonly IMemoryCache _memoryCache;

        public async Task<string> StoreAsync(AuthenticationTicket ticket) 
        {
            var key = Guid.NewGuid().ToString();
            var serializeTicket = TicketSerializer.Default.Serialize(ticket);

            _memoryCache.Set(key, serializeTicket);

            return key;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket) 
        {
            var serializedTicket = TicketSerializer.Default.Serialize(ticket);

            _memoryCache.Set(key, serializedTicket);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            if (_memoryCache.TryGetValue<byte[]>(key, out var serializedTicket))
            {
                return TicketSerializer.Default.Deserialize(serializedTicket);
            }

            return null;
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
