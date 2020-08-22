using System;
using System.Threading.Tasks;

namespace Treadmill.Domain
{
    public static class Async
    {
        public static async Task SafeExec(Func<Task> f, Action<Exception> onException = default)
        {
            try
            {
                await f().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (onException != default)
                {
                    onException(e);
                }
            }
        }

        public static async Task<T> SafeExec<T>(Func<Task<T>> f, Action<Exception> onException = default)
        {
            try
            {
                return await f().ConfigureAwait(false);
            }
            catch (Exception e)
            {   
                if (onException != default)
                {
                    onException(e);
                }
                return default;
            }
        }
    }
}
