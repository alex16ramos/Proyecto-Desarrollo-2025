using System;
using System.Security.Claims;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace FAFS;

// Base GENÉRICA
public abstract class FAFSApplicationTestBase<TStartupModule> : FAFSTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected ICurrentUser CurrentUser => GetRequiredService<ICurrentUser>();
    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor => GetRequiredService<ICurrentPrincipalAccessor>();

    protected IDisposable Login(Guid userId, string userName = "testuser")
    {
        var claims = new[]
        {
            new Claim(AbpClaimTypes.UserId, userId.ToString()),
            new Claim(AbpClaimTypes.UserName, userName)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        return CurrentPrincipalAccessor.Change(claimsPrincipal);
    }
}

// Base NO GENÉRICA
public abstract class FAFSApplicationTestBase : FAFSApplicationTestBase<FAFSApplicationTestModule>
{
}
