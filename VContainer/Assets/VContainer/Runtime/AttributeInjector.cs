using System;
using System.Reflection;

namespace VContainer
{
    public readonly struct AttributeInjector<T> where T : Attribute
    {
        public readonly T Attribute;
        public readonly Type ClassType;
        public readonly MethodInfo Method;
        
        public AttributeInjector(T attribute, Type classType, MethodInfo method)
        {
            Attribute = attribute;
            ClassType = classType;
            Method = method;
        }
    }
}
