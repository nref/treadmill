using System;
using System.Threading.Tasks;

namespace Treadmill.Hosting.Extensions
{
    public static class FuncExtensions
    {
        public static async Task<T> SafeCall<T>(Func<Task<T>> f)
        {
            try
            {
                return await f().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }

        }

        public static async Task SafeCall(Func<Task> f, Action onError = default)
        {
            try
            {
                await f().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (onError != default)
                {
                    onError();
                }
            }
        }
    }
}
