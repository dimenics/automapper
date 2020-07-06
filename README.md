# Dime.Automapper

[![Build Status](https://dev.azure.com/dimenicsbe/Utilities/_apis/build/status/AutoMapper%20-%20MAIN%20-%20CI?branchName=master)](https://dev.azure.com/dimenicsbe/Utilities/_build/latest?definitionId=69&branchName=master) [![Dime.AutoMapper package in Dime.Scheduler feed in Azure Artifacts](https://feeds.dev.azure.com/dimenicsbe/_apis/public/Packaging/Feeds/a7b896fd-9cd8-4291-afe1-f223483d87f0/Packages/da2ca1af-89ef-49ff-b518-74ca3e5df853/Badge)](https://dev.azure.com/dimenicsbe/Utilities/_packaging?_a=package&feed=a7b896fd-9cd8-4291-afe1-f223483d87f0&package=da2ca1af-89ef-49ff-b518-74ca3e5df853&preferRelease=true) ![No Maintenance Intended](http://unmaintain.tech/badge.svg)

## Introduction

Auto discovery of Automapper classes.

## Getting Started

- You must have Visual Studio 2019 Community or higher.
- The dotnet cli is also highly recommended.

## About this project

The purpose of this project was to reach automatic discovery of mapping classes through reflection. This has been addressed in AutoMapper 9, eliminating the need of this project.

Dime.Automapper:

```csharp

public static void Main(params string[] args)
{
    AutoMapperFactory.Initialize();

    IMapper mapper = AutoMapperFactory.Create();
    TestClassOne instance1 = new TestClassOne() { Id = 2 };
    TestClassTwo instance2 = mapper.Map<TestClassTwo>(instance1);
}

public class TestClassMapper : IAutoMapper
{
    public Action<IMapperConfigurationExpression> Configure()
    {
        return (x) =>
        {
            x.CreateMap<TestClassOne, TestClassTwo>();
        };
    }
}

```

AutoMapper 9:

``` csharp

internal sealed class ClientMapper : AutoMapper.Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientMapper"/> class
    /// </summary>
    public ClientMapper()
    {        
        CreateMap<Client, ClientDto>();
    }
}

MapperConfiguration configuration = new MapperConfiguration(cfg =>
{
    cfg.AddMaps("My.Library");
    cfg.DisableConstructorMapping();
});

var mapper = new Mapper(configuration);
```


## Build and Test

- Run dotnet restore
- Run dotnet build
- Run dotnet test

## Installation

Use the package manager NuGet to install Dime.Automapper:

`dotnet add package Dime.Automapper`

## Usage

``` csharp

public static void Main(params string[] args)
{
    AutoMapperFactory.Initialize();

    IMapper mapper = AutoMapperFactory.Create();
    TestClassOne instance1 = new TestClassOne() { Id = 2 };
    TestClassTwo instance2 = mapper.Map<TestClassTwo>(instance1);
}

public class TestClassMapper : IAutoMapper
{
    public Action<IMapperConfigurationExpression> Configure()
    {
        return (x) =>
        {
            x.CreateMap<TestClassOne, TestClassTwo>();
        };
    }
}
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[![License](http://img.shields.io/:license-mit-blue.svg?style=flat-square)](http://badges.mit-license.org)