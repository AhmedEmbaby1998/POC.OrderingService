using Volo.Abp.Modularity;

namespace POC;

public abstract class POCApplicationTestBase<TStartupModule> : POCTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
