using System.Reflection;
using System.Threading.Tasks;

namespace Treadmill.Hosting.Extensions
{
    public static class MethodInfoExtensions
    {
        public static async Task<object> InvokeAsync(this MethodInfo m, object obj, params object[] parameters)
        {
            dynamic awaitable = m.Invoke(obj, parameters);
            await awaitable;
            return awaitable.GetAwaiter().GetResult();
        }

        public static async Task InvokeAsync2(this MethodInfo m, object obj, params object[] parameters)
        {
            dynamic awaitable = m.Invoke(obj, parameters);
            await awaitable;
        }
    }
}
