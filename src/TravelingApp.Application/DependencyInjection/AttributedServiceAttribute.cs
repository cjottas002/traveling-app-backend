using Microsoft.Extensions.DependencyInjection;
using TravelingApp.Application.Extensions;

namespace TravelingApp.Application.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class AttributedServiceAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime.ValidateArgument();
    public Type? Interface { get; set; }
    public Type[] ExtraInterfaces { get; set; } = [];
}

public sealed class ScopedServiceAttribute : AttributedServiceAttribute
{
    public ScopedServiceAttribute(Type? @interface = null, params Type[] extraInterfaces) : base(ServiceLifetime.Scoped)
    {
        Interface = @interface;
        ExtraInterfaces = extraInterfaces ?? [];
    }
}

public sealed class SingletonServiceAttribute : AttributedServiceAttribute
{
    public SingletonServiceAttribute(Type? @interface = null, params Type[] extraInterfaces) : base(ServiceLifetime.Singleton)
    {
        Interface = @interface;
        ExtraInterfaces = extraInterfaces ?? [];
    }
}

public sealed class TransientServiceAttribute : AttributedServiceAttribute
{
    public TransientServiceAttribute(Type? @interface = null, params Type[] extraInterfaces) : base(ServiceLifetime.Transient)
    {
        Interface = @interface;
        ExtraInterfaces = extraInterfaces ?? [];
    }
}
