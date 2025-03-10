using GameManager.Server.Models;
using HotChocolate.Data.Filters;

namespace GameManager.Server.Filters;

public class GameFilterType : FilterInputType<GameModel>
{
    protected override void Configure(IFilterInputTypeDescriptor<GameModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.Id).Type<CustomGuidOperationFilterInputType>();
        descriptor.Field(f => f.Name);
        descriptor.Field(f => f.State);
    }
}