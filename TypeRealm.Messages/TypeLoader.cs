using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProtoBuf;

namespace TypeRealm.Messages
{
    public static class TypeLoader
    {
        public static IEnumerable<Type> Messages => Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttribute(typeof(ProtoContractAttribute)) != null)
            .OrderBy(t => t.FullName);
    }
}
