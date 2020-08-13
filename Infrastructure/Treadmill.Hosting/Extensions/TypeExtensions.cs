using System;

namespace Treadmill.Hosting.Extensions
{
    public static class TypeExtensions
    {
        public static T ChangeType<T>(this object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public static object ChangeType(this object obj, Type t)
        {
            return Convert.ChangeType(obj, t);
        }
    }
}
