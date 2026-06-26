namespace Trakmark.IntegrationTests;

/// <summary>
/// xUnit collection name constant shared by all integration test classes.
/// </summary>
public static class IntegrationTestCollection
{
    /// <summary>The collection name used in <c>[Collection]</c> and <c>[CollectionDefinition]</c> attributes.</summary>
    public const string Name = "Integration";
}

/// <summary>
/// Defines the xUnit collection that shares a single <see cref="DatabaseFixture"/>
/// across all integration test classes.
/// </summary>
[CollectionDefinition(IntegrationTestCollection.Name)]
public sealed class IntegrationTestCollectionDefinition : ICollectionFixture<DatabaseFixture>
{
}
