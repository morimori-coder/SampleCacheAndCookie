using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace SampleCacheAndCookie
{
    // アクティブなセッション一覧を扱う、本来なら RDB や KVS などに入れる
    public class ActiveSessionRepository
    {
        public ActiveSessionRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        private readonly IMemoryCache _memoryCache;

        public void Activate(ClaimsPrincipal claimsPrincipal)
        {
            var hash = claimsPrincipal.FindFirstValue(ClaimTypes.SerialNumber);

            if (string.IsNullOrEmpty(hash))
            {
                return;
            }

            _memoryCache.Set(hash, true);
        }

        public void Inactivate(ClaimsPrincipal claimsPrincipal)
        {
            var hash = claimsPrincipal.FindFirstValue(ClaimTypes.SerialNumber);

            if (string.IsNullOrEmpty(hash))
            {
                return;
            }

            _memoryCache.Remove(hash);
        }

        public bool IsActive(ClaimsPrincipal claimsPrincipal)
        {
            var hash = claimsPrincipal.FindFirstValue(ClaimTypes.SerialNumber);

            if (string.IsNullOrEmpty(hash))
            {
                return false;
            }

            return _memoryCache.TryGetValue(hash, out _);
        }
    }

    public class ActiveSessionCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public ActiveSessionCookieAuthenticationEvents(ActiveSessionRepository activeSessionRepository)
        {
            _activeSessionRepository = activeSessionRepository;
        }

        private readonly ActiveSessionRepository _activeSessionRepository;

        public override Task SignedIn(CookieSignedInContext context)
        {
            _activeSessionRepository.Activate(context.HttpContext.User);

            return Task.CompletedTask;
        }

        public override Task SigningOut(CookieSigningOutContext context)
        {
            _activeSessionRepository.Inactivate(context.HttpContext.User);

            return Task.CompletedTask;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (!_activeSessionRepository.IsActive(context.Principal))
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }

}
