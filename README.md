# MediatR.Extensions.AttributedBehaviors

[![CI](https://github.com/ITIXO/MediatR.Extensions.AttributedBehaviors/actions/workflows/ci.yml/badge.svg)](https://github.com/ITIXO/MediatR.Extensions.AttributedBehaviors/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/vpre/MediatR.Extensions.AttributedBehaviors.svg)](https://www.nuget.org/packages/MediatR.Extensions.AttributedBehaviors)


MediatR extension adding ability to specify pipeline behaviors using attributes on command class. 
Leverages `Microsoft.Extensions.DependencyInjection` as a DI container.

Ever found yourself confused when looking for behaviors that are attached to the pipeline you are about to debug? 
Keeping all behaviors specified within the command class makes it really clear.

```csharp

// Instead of this
 services.AddScoped<IPipelineBehavior<MyRequest, MyReturnType>, MyFirstPipelineBehavior<MyRequest, MyReturnType>>()
         .AddScoped<IPipelineBehavior<MyRequest, MyReturnType>, MySecondPipelineBehavior>();

// You write this
[MediatRBehavior(typeof(MyFirstPipelineBehavior<MyRequest, MyReturnType>))]
[MediatRBehavior(typeof(MySecondPipelineBehavior))]
public class MyRequest : IRequest<MyReturnType> { }
```
You immediately see all the behaviors your request passes through in order as they are specified. 
No need to search in your code base for behaviors installation to see what behaviors are used.

# Install

## Install with Package Manager Console

```
Install-Package MediatR.Extensions.AttributedBehaviors
```

## Install with .NET CLI
```
dotnet add package MediatR.Extensions.AttributedBehaviors
```

# How to use

## Setup - Add configuration in startup.cs


```csharp

public void ConfigureServices(IServiceCollection services)
{   
    var executingAssembly = Assembly.GetExecutingAssembly()
    // Add MediatR
    services.AddMediatR(executingAssembly);

    // optionally register open generics universal behaviors (e.g. logging)
    // before registering attributed behaviors
    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

    //Add Attributed Behaviors
    services.AddMediatRAttributeBehaviors(executingAssembly);
    
    //Add other stuff
    ...
}

```

## Use

You no longer need to register your behaviors in service collection. You can keep them clearly visible right where they are used
```csharp

// no longer needed
 services.AddScoped<IPipelineBehavior<GetValidEventUrlRequest, ValidEventUrl>, ResolveEventIdByPublicIdPipelineBehavior<GetValidEventUrlRequest, ValidEventUrl>>()
         .AddScoped<IPipelineBehavior<GetValidEventUrlRequest, ValidEventUrl>, CheckEventDisabledPipelineBehavior<GetValidEventUrlRequest, ValidEventUrl>>();



// instead you can just simply specify them like this
[MediatRBehavior(typeof(ResolveEventIdByPublicIdPipelineBehavior<GetValidEventUrlRequest, ValidEventUrl>))]
[MediatRBehavior(typeof(CheckEventDisabledPipelineBehavior<GetValidEventUrlRequest, ValidEventUrl>))]
public class GetValidEventUrlRequest : IRequest<ValidEventUrl>
{
    public string PublicId { get; set; }
    public int EventId { get; set; }
}
```

## Custom behavior life-cycle

By default, Scoped lifetime is used. You can use your own lifetime as desired
```csharp

// singleton behavior
[MediatRBehavior(typeof(MySingletonPipelineBehavior<MyQuery>), serviceLifetime: ServiceLifetime.Singleton)]
public class MyQuery : IRequest
```

## Custom behavior ordering

By default, behaviors are orders as they are specified top-to-bottom. If you ever need to specify your custom ordering, you can.
```csharp

// override default top->bottom ordering
[MediatRBehavior(typeof(SecondPipelineBehavior<MyQuery>), order: 2)]
[MediatRBehavior(typeof(FirstPipelineBehavior<MyQuery>), order: 1)]
public class MyQuery : IRequest
```

## Changelog

### 2.0.0
Updated MediatR dependency to 10.x
Dropped support for netstandard2.0

### 1.0.1
Fixed Service collection extensions
Added unit tests

### 1.0.1
Matched dependencies versions with MediatR

### 1.0.0
Initial version