using Microsoft.Extensions.DependencyInjection;
using Treadmill.Api.Controllers;
using Treadmill.Infrastructure;

namespace Treadmill.Api
{
    public class ApiCompositionRoot : CompositionRoot
    {
        public ApiCompositionRoot(DomainConfiguration config) : base(config)
        {
            Container.Configure(_ =>
            {
                _.AddTransient<IController, TreadmillController>();
            });
        }
    }
}
