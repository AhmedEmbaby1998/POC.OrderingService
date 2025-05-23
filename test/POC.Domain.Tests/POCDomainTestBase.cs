using Volo.Abp.Modularity;

namespace POC;

/* Inherit from this class for your domain layer tests. */
public abstract class POCDomainTestBase<TStartupModule> : POCTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
