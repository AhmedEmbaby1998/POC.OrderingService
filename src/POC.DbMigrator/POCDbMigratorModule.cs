using POC.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace POC.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(POCEntityFrameworkCoreModule),
    typeof(POCApplicationContractsModule)
)]
public class POCDbMigratorModule : AbpModule
{
}
