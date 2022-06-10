using Autofac;
using System;
using System.Collections.Generic;
using System.Reflection;
using Treadmill.Adapters.RemoteTreadmill;
using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;
using Treadmill.Models;

namespace Treadmill.Infrastructure
{
  public interface ICompositionRoot
  {
    object Get(Type t);
    T Get<T>();
  }

  public class CompositionRoot : ICompositionRoot
  {
    private readonly DomainConfiguration _config;

    protected IContainer _Container { get; set; }

    public virtual IEnumerable<Assembly> Assemblies { get; } = new List<Assembly>() { Assembly.GetExecutingAssembly() };

    public CompositionRoot(DomainConfiguration config)
    {
      _config = config;
    }

    public virtual void Configure(ContainerBuilder builder)
    {
      foreach (var assembly in Assemblies)
      {
        builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
      }

      builder.RegisterType<RemoteTreadmillAdapter>()
                .As<IRemoteTreadmillAdapter>()
                .SingleInstance()
                .WithParameter("udpMetrics", new UdpService(_config.LocalIp, _config.LocalUdpPort))
                .WithParameter("health", new UdpService(_config.LocalIp, _config.LocalUdpHealthPort));

      builder.RegisterType<RemoteTreadmillClient>()
          .As<IRemoteTreadmillClient>()
          .WithParameter("remoteUrl", _config.RemoteTreadmillServiceUrl);

      builder.RegisterType<HttpService>().As<IHttpService>()
          .WithParameter("url", _config.ListenUri);

      builder.RegisterType<RemoteTreadmillService>().As<IRemoteTreadmillService>().SingleInstance();
      builder.RegisterType<ConnectionService>().As<IConnectionService>().SingleInstance();

      Log.Added += message => System.Diagnostics.Debug.Write(message);
    }

    public object Get(Type t) => _Container.Resolve(t);
    public T Get<T>() => _Container.Resolve<T>();
  }
}
