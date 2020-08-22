using Treadmill.Api.Controllers;
using Treadmill.Infrastructure;

namespace Treadmill.Api
{
    public class ApiCompositionRoot : CompositionRoot
    {
        public ApiCompositionRoot(DomainConfiguration config) : base(config)
        {
            Container.Bind<IController>().To<TreadmillController>();
        }
    }
}
