using Volo.Abp.Modularity;

namespace POC;

[DependsOn(
    typeof(POCDomainModule),
    typeof(POCTestBaseModule)
)]
public class POCDomainTestModule : AbpModule
{

}
