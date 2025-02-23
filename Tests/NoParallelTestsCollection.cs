using Xunit;

[CollectionDefinition("NoParallelTests", DisableParallelization = true)]
public class NoParallelTestsCollection
{
    // La classe est vide, elle sert uniquement à définir la collection.
}