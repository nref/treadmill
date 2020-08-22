using System;

namespace Treadmill.Hosting.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : HttpMethodAttribute
    {
        public GetAttribute(string resourceName)
        {
            ResourceName = resourceName;
        }
    }
}
