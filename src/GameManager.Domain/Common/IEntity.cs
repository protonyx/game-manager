namespace GameManager.Domain.Common;

public interface IEntity<out TKey>
{
    TKey Id { get; }
}