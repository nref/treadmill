using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Treadmill.Hosting.Attributes;
using Treadmill.Infrastructure;

namespace Treadmill.Hosting.Extensions
{
    public static class ControllerExtensions
    {
        public static Dictionary<string, MethodInfo> GetActions
        (
            this IController controller,
            string method
        )
        {
            switch (method)
            {
                case "POST": return controller.GetPostActions();
                case "GET": return controller.GetGetActions();
                default: return default;
            };
        }

        public static Dictionary<string, MethodInfo> GetGetActions(this IController controller)
            => controller
                .GetMethodsWithAttribute<GetAttribute>()
                .ToDictionary(tup => tup.Item2.ResourceName, a => a.Item1);

        public static Dictionary<string, MethodInfo> GetPostActions(this IController controller)
            => controller
                .GetMethodsWithAttribute<PostAttribute>()
                .ToDictionary(tup => tup.Item2.ResourceName, a => a.Item1);

        public static List<Tuple<MethodInfo, T>> GetMethodsWithAttribute<T>(this IController controller) where T : Attribute
            => controller
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes<T>().Any())
                .Select(m => new Tuple<MethodInfo, T>(m, m.GetCustomAttribute<T>()))
                .ToList();
    }
}
