namespace ForumBackendAPITest;

/**
 * This collection ensures that the Service tests are run sequentially, safely sharing the db ctx.
 */
[CollectionDefinition("SharedDbCtxCollection")]
public class TestDatabaseCollection : ICollectionFixture<TestDatabaseFixture>
{
    
}