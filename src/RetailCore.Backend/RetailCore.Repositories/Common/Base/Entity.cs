namespace RetailCore.Repositories.Common;

public abstract class Entity : IIdentifiable
{
    public Guid Id { get; set; }
}
