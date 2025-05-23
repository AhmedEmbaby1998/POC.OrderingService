using POC.Samples;
using Xunit;

namespace POC.EntityFrameworkCore.Domains;

[Collection(POCTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<POCEntityFrameworkCoreTestModule>
{

}
