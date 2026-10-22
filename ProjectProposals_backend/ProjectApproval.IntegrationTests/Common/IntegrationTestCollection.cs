using Xunit;

namespace ProjectApproval.IntegrationTests.Common;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
