using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.AggregateService;
using Castle.Core.Internal;

namespace LevelUp.Pos.ProposedOrders.Framework
{
    internal static class Autofac
    {
        public static IContainer GetContainer()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Namespace?.Contains(SERVICES_NAMESPACE) ?? false)
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Namespace?.Contains(WRAPPERS_NAMESPACE) ?? false)
                .AsImplementedInterfaces();

            return builder.Build();
        }


        private const string SERVICES_NAMESPACE = "DiTalk.Services";
        private const string WRAPPERS_NAMESPACE = "DiTalk.Wrappers";
    }
}
