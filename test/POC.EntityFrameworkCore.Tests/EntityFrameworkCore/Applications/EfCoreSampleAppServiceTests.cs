using POC.Samples;
using Xunit;

namespace POC.EntityFrameworkCore.Applications;

[Collection(POCTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<POCEntityFrameworkCoreTestModule>
{

}
