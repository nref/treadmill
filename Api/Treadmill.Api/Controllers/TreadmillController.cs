using System.Threading.Tasks;
using Treadmill.Domain;
using Treadmill.Dto;
using Treadmill.Hosting.Attributes;
using Treadmill.Infrastructure;

namespace Treadmill.Api.Controllers
{
    public class TreadmillController : IController
    {
        private readonly ITreadmillService _service;

        public string RoutePrefix => "treadmill";

        public TreadmillController(ITreadmillService service)
        {
            _service = service;
        }

        [Post("")]
        public async Task SetSpeed([FromRoute] double speed)
        {
            await _service.SetSpeed(speed);
        }

        [Post("test")]
        //public async Task Test([FromQuery] double a)
        //public async Task Test([FromBody] double a)
        //public async Task Test([FromRoute] double a)
        public async Task Test([FromBody] Workout w)
        //public async Task Test([FromRoute] double a, [FromQuery] double b, [FromBody] double c)
        {
            await Task.CompletedTask;
        }

        [Get("")]
        public async Task<double> GetSpeed()
        {
            await Task.CompletedTask;
            return 0;
        }
    }
}
