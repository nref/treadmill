using System.Threading.Tasks;

namespace Treadmill.Domain.Adapters
{
    public interface ITreadmillAdapter
    {
        Task Start();
        Task Stop();
        Task Pause();
        Task Resme();

        Task IncrementSpeed();
        Task DecrementSpeed();
        Task IncrementIncline();
        Task DecrementIncline();
    }
}
