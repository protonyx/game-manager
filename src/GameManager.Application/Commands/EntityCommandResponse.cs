using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Commands;

public class EntityCommandResponse : ICommandResponse
{
    public Guid Id { get; }
    
    public object Value { get; }

    public EntityCommandResponse(Guid id, object value)
    {
        Id = id;
        Value = value;
    }
}