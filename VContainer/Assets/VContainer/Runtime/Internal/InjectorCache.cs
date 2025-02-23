using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace VContainer.Internal
{
    public static class InjectorCache
    {
        static readonly ConcurrentDictionary<Type, IInjector> Injectors = new ConcurrentDictionary<Type, IInjector>();
        internal static readonly ConcurrentDictionary<Type, Action<AttributeInjector<Attribute>>> AttributeInjectors = new();

        public static void RegisterAttributeInjector(Type attributeType, Action<AttributeInjector<Attribute>> injector)
        {
            if (!AttributeInjectors.TryAdd(attributeType, injector))
            {
                throw new VContainerException(attributeType, $"Duplicate attribute injector: {attributeType.FullName}");
            }
        }

        public static bool UnregisterAttributeInjector(Type attributeType) => AttributeInjectors.TryRemove(attributeType, out _);
        
        public static IInjector GetOrBuild(Type type)
        {
            return Injectors.GetOrAdd(type, key =>
            {
                // SourceGenerator
                var generatedType = key.Assembly.GetType($"{key.FullName}GeneratedInjector", false);
                if (generatedType != null)
                {
                    return (IInjector)Activator.CreateInstance(generatedType);
                }

                // IL weaving (Deprecated)
                var getter = key.GetMethod("__GetGeneratedInjector", BindingFlags.Static | BindingFlags.Public);
                if (getter != null)
                {
                    return (IInjector)getter.Invoke(null, null);
                }
                return ReflectionInjector.Build(key);
            });
        }
    }
}
