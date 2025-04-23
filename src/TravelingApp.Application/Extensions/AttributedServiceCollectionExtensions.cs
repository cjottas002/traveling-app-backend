using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TravelingApp.Application.DependencyInjection;

namespace TravelingApp.Application.Extensions;

public static class AttributedServiceCollectionExtensions
{
    public static IServiceCollection AddAttributedServices(this IServiceCollection services, params string[] friendlyNamePrefixes)
    {
        return services.AddAttributedServices(friendlyNamePrefixes, excludeFullNames: []);
    }

    public static IServiceCollection AddAttributedServices(this IServiceCollection services, string[] friendlyNamePrefixes, string[] excludeFullNames)
    {
        ArgumentNullException.ThrowIfNull(services);

        var normalizedPrefixes = NormalizePrefixes(friendlyNamePrefixes);
        if (normalizedPrefixes.Length == 0)
            return services;

        var exclusionSet = new HashSet<string>(
            (excludeFullNames ?? []).Where(name => !string.IsNullOrWhiteSpace(name)),
            StringComparer.OrdinalIgnoreCase);
        var loadedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            RegisterAttributedTypesFromAssembly(services, loadedAssembly, normalizedPrefixes, loadedAssemblies, exclusionSet);
        }

        LoadReferencedAssembliesFromEntryAssembly(services, normalizedPrefixes, loadedAssemblies, exclusionSet);

        return services;
    }

    private static void LoadReferencedAssembliesFromEntryAssembly(IServiceCollection services, IReadOnlyCollection<string> normalizedPrefixes, ISet<string> loadedAssemblies, ISet<string> exclusionSet)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is null)
            return;

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<AssemblyName>(entryAssembly.GetReferencedAssemblies());

        while (queue.Count > 0)
        {
            var assemblyName = queue.Dequeue();
            var simpleName = assemblyName.Name;

            if (string.IsNullOrWhiteSpace(simpleName) || !HasFriendlyNamePrefix(simpleName, normalizedPrefixes) || !visited.Add(simpleName))
                continue;

            var assembly = TryLoadAssembly(assemblyName);
            if (assembly is null)
                continue;

            RegisterAttributedTypesFromAssembly(services, assembly, normalizedPrefixes, loadedAssemblies, exclusionSet);

            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                var referencedName = referencedAssembly.Name;
                if (!string.IsNullOrWhiteSpace(referencedName) && HasFriendlyNamePrefix(referencedName, normalizedPrefixes))
                    queue.Enqueue(referencedAssembly);
            }
        }
    }

    private static void RegisterAttributedTypesFromAssembly(IServiceCollection services, Assembly assembly, IReadOnlyCollection<string> normalizedPrefixes, ISet<string> loadedAssemblies, ISet<string> exclusionSet)
    {
        if (assembly.IsDynamic)
            return;

        var friendlyName = assembly.GetName().Name;
        if (string.IsNullOrWhiteSpace(friendlyName) || !HasFriendlyNamePrefix(friendlyName, normalizedPrefixes))
            return;

        var assemblyIdentity = assembly.FullName ?? friendlyName;
        if (!loadedAssemblies.Add(assemblyIdentity))
            return;

        foreach (var implementationType in GetLoadableTypes(assembly))
        {
            if (implementationType is not { IsClass: true, IsAbstract: false })
                continue;

            var attribute = implementationType.GetCustomAttribute<AttributedServiceAttribute>(false);
            if (attribute is null)
                continue;

            if (exclusionSet.Count > 0 && !string.IsNullOrWhiteSpace(implementationType.FullName) && exclusionSet.Contains(implementationType.FullName))
                continue;

            RegisterAttributedType(services, implementationType, attribute);
        }
    }

    private static void RegisterAttributedType(IServiceCollection services, Type implementationType, AttributedServiceAttribute attribute)
    {
        var primaryServiceType = attribute.Interface ?? implementationType;

        if (attribute.Interface is not null && !primaryServiceType.IsAssignableFrom(implementationType))
            throw new InvalidOperationException($"'{implementationType.FullName}' no implementa '{primaryServiceType.FullName}'.");

        services.Add(ServiceDescriptor.Describe(primaryServiceType, implementationType, attribute.Lifetime));

        foreach (var extraInterface in attribute.ExtraInterfaces.Distinct())
        {
            if (extraInterface == primaryServiceType)
                continue;

            if (!extraInterface.IsAssignableFrom(implementationType))
                throw new InvalidOperationException($"'{implementationType.FullName}' no implementa '{extraInterface.FullName}'.");

            services.Add(ServiceDescriptor.Describe(extraInterface, sp => sp.GetRequiredService(primaryServiceType), attribute.Lifetime));
        }
    }

    private static string[] NormalizePrefixes(IEnumerable<string>? prefixes)
    {
        return (prefixes ?? [])
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool HasFriendlyNamePrefix(string name, IEnumerable<string> prefixes)
    {
        return prefixes.Any(p => name.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private static Assembly? TryLoadAssembly(AssemblyName name)
    {
        try { return Assembly.Load(name); }
        catch { return null; }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { return ex.Types.OfType<Type>(); }
        catch { return []; }
    }
}
