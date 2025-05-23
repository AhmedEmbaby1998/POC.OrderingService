using Volo.Abp.Modularity;

namespace POC;

[DependsOn(
    typeof(POCApplicationModule),
    typeof(POCDomainTestModule)
)]
public class POCApplicationTestModule : AbpModule
{

}
