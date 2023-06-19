using AutoMapper;
using GameManager.Application.Profiles;

namespace GameManager.Tests;

public class BasicTests
{
    
    [Fact]
    public void Test_AutoMapper_Profiles()
    {
        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DtoProfile>();
        });
        
        mappingConfig.AssertConfigurationIsValid();
    }

}