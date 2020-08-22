using System;

namespace Treadmill.Hosting.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : HttpMethodAttribute
    {
        public PostAttribute(string resourceName)
        {
            ResourceName = resourceName;
        }
    }
}
