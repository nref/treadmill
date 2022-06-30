using Treadmill.Models;

namespace Treadmill.Adapters.RemoteTreadmill
{
  public class TreadmillMetric
  {
    public TreadmillState State { get; set; }
    public Metric Metric { get; set; }
    public string Value { get; set; }
  }
}
