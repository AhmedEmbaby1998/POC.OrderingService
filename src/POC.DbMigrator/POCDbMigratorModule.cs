using POC.EntityFrameworkCore;
using POC.OrderingService.Query;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace POC.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(POCEntityFrameworkCoreModule),
    typeof(POCApplicationContractsModule),
    typeof(OrderingServiceQueryModule)
)]
public class POCDbMigratorModule : AbpModule
{
}
