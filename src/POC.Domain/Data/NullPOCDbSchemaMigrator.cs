using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace POC.Data;

/* This is used if database provider does't define
 * IPOCDbSchemaMigrator implementation.
 */
public class NullPOCDbSchemaMigrator : IPOCDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
