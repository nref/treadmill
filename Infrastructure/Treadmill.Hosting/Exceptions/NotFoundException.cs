using System;
using System.Net;

namespace Treadmill.Hosting.Exceptions
{
    public class NotFoundException : Exception
    {
        public HttpReponse Reponse => new HttpReponse
        {
            Code = HttpStatusCode.NotFound,
            Data = "NotFound"
        };
    }

}
