namespace CloudObjects.App.Interfaces
{
    public interface IEntity { }

    public interface IEntity<TEntityKey> : IEntity
        where TEntityKey : struct
    {
        TEntityKey Id { get; set; }
    }
}
