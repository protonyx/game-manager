using HotChocolate.Data.Filters;

namespace GameManager.Server.Filters;

public class CustomGuidOperationFilterInputType : UuidOperationFilterInputType
{
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Operation(DefaultFilterOperations.Equals)
            .Type<UuidType>();
    }
}