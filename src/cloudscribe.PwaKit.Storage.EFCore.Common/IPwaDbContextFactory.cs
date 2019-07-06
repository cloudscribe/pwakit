namespace cloudscribe.PwaKit.Storage.EFCore.Common
{
    public interface IPwaDbContextFactory
    {
        IPwaDbContext CreateContext();

    }
}
