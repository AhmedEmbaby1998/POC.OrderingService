using System.Threading.Tasks;

namespace POC.Data;

public interface IPOCDbSchemaMigrator
{
    Task MigrateAsync();
}
