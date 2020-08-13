using System;

namespace Treadmill.Hosting.Attributes
{
    public abstract class HttpMethodAttribute : Attribute
    {
        public string ResourceName { get; protected set; }
    }
}
