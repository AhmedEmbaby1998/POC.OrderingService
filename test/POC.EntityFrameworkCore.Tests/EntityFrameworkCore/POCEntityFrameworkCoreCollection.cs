using Xunit;

namespace POC.EntityFrameworkCore;

[CollectionDefinition(POCTestConsts.CollectionDefinitionName)]
public class POCEntityFrameworkCoreCollection : ICollectionFixture<POCEntityFrameworkCoreFixture>
{

}
