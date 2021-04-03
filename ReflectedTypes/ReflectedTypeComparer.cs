using System;
using System.Collections.Generic;

namespace ReflectedTypes
{
    /// <summary>
    /// Used for selecting generic method e.g.
    /// </summary>
    internal sealed class ReflectedTypeComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type? x, Type? y)
        {
            if (x == null || y == null)
                return false;

            return (x.Assembly == y.Assembly &&
                    x.Namespace == y.Namespace &&
                    x.Name == y.Name) 
                   || y.IsSubclassOf(x) 
                   || y.IsAssignableFrom(x);
        }

        public int GetHashCode(Type obj)
        {
            throw new NotImplementedException();
        }
    }
}