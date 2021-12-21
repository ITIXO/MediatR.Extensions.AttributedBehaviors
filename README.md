# MediatR.Extensions.AttributedBehaviors

MediatR extension adding ability to specify pipeline behaviors using attributes on command class.

Ever found yourself confused when searching for bahviors that are attached to the pipeline you are about to debug? 
Keeping all behaviors specified within the command class makes it really clear.

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

## Setup - Add configuration in startup 


```csharp

public void ConfigureServices(IServiceCollection services)
{   
    var executingAssembly = Assembly.GetExecutingAssembly()
    // Add MediatR
    services.AddMediatR(executingAssembly);

    //Add Attributed Behaviors
    services.AddMediatRAttributeBehaviors(executingAssembly);
    
    //Add other stuffs
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
