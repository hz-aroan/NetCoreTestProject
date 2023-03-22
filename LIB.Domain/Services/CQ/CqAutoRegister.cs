using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LIB.Domain.Services.CQ;

public class CqAutoRegister
{
    // credit to MediatR



    public static void BuildCqTypes(IServiceCollection services, Assembly assembly)
    {
        var assembliesToScan = new[] { assembly };

        ConnectImplementationsToTypesClosing(typeof(IQueryHandler<,>), services, assembliesToScan, false);
        ConnectImplementationsToTypesClosing(typeof(IQueryHandlerWithPermission<,>), services, assembliesToScan, false);

        ConnectImplementationsToTypesClosing(typeof(ICommandHandler<>), services, assembliesToScan, false);
    }



    // -----------------------------



    private static void ConnectImplementationsToTypesClosing(Type openRequestInterface, IServiceCollection services, IEnumerable<Assembly> assembliesToScan, Boolean addIfAlreadyExists)
    {
        var concretions = new List<Type>();
        var interfaces = new List<Type>();
        foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes).Where(t => !IsOpenGeneric(t)))
        {
            var interfaceTypes = FindInterfacesThatClose(type, openRequestInterface).ToArray();
            if (!interfaceTypes.Any()) continue;

            if (IsConcrete(type)) concretions.Add(type);

            foreach (var interfaceType in interfaceTypes)
            {
                Fill(interfaces, interfaceType);
            }
        }

        foreach (var @interface in interfaces)
        {
            var exactMatches = concretions.Where(x => CanBeCastTo(x, @interface)).ToList();
            if (addIfAlreadyExists)
            {
                foreach (var type in exactMatches)
                {
                    services.AddScoped(@interface, type);
                }
            }
            else
            {
                if (exactMatches.Count > 1) exactMatches.RemoveAll(m => !IsMatchingWithInterface(m, @interface));

                foreach (var type in exactMatches)
                {
                    services.TryAddScoped(@interface, type);
                }
            }

            if (!IsOpenGeneric(@interface)) AddConcretionsThatCouldBeClosed(@interface, concretions, services);
        }
    }



    private static void AddConcretionsThatCouldBeClosed(Type @interface, List<Type> concretions, IServiceCollection services)
    {
        foreach (var type in concretions.Where(x => IsOpenGeneric(x) && CouldCloseTo(x, @interface)))
        {
            try
            {
                services.TryAddScoped(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
            }
            catch (Exception)
            {
            }
        }
    }



    private static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
    {
        if (pluggedType == null) yield break;

        if (!IsConcrete(pluggedType)) yield break;

        if (templateType.GetTypeInfo().IsInterface)
        {
            foreach (var interfaceType in pluggedType.GetInterfaces().Where(type => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == templateType))
            {
                yield return interfaceType;
            }
        }
        else if (pluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType && pluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == templateType)
            yield return pluggedType.GetTypeInfo().BaseType;

        if (pluggedType.GetTypeInfo().BaseType == typeof(Object)) yield break;

        foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.GetTypeInfo().BaseType, templateType))
        {
            yield return interfaceType;
        }
    }



    private static Boolean IsConcrete(Type type)
    {
        return !type.GetTypeInfo()
            .IsAbstract && !type.GetTypeInfo()
            .IsInterface;
    }



    public static Boolean IsOpenGeneric(Type type)
    {
        return type.GetTypeInfo()
            .IsGenericTypeDefinition || type.GetTypeInfo()
            .ContainsGenericParameters;
    }



    public static IEnumerable<Type> FindInterfacesThatClose(Type pluggedType, Type templateType)
    {
        return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
    }



    private static Boolean CanBeCastTo(Type pluggedType, Type pluginType)
    {
        if (pluggedType == null) return false;

        if (pluggedType == pluginType) return true;

        return pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo());
    }



    private static Boolean IsMatchingWithInterface(Type handlerType, Type handlerInterface)
    {
        if (handlerType == null || handlerInterface == null) return false;

        if (handlerType.IsInterface)
        {
            if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments)) return true;
        }
        else
        {
            return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
        }

        return false;
    }



    private static Boolean CouldCloseTo(Type openConcretion, Type closedInterface)
    {
        var openInterface = closedInterface.GetGenericTypeDefinition();
        var arguments = closedInterface.GenericTypeArguments;

        var concreteArguments = openConcretion.GenericTypeArguments;
        return arguments.Length == concreteArguments.Length && CanBeCastTo(openConcretion, openInterface);
    }



    private static void Fill<T>(IList<T> list, T value)
    {
        if (list.Contains(value)) return;
        list.Add(value);
    }
}