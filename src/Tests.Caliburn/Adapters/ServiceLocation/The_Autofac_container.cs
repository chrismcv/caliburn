﻿namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using Autofac;
    using Components;
    using global::Caliburn.Autofac;
    using global::Caliburn.Core.InversionOfControl;
    using NUnit.Framework;

    [TestFixture]
    public class The_Autofac_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AdvancedLogger>().As<ILogger>();
            builder.RegisterType<SimpleLogger>().As<ILogger>().Named(typeof(SimpleLogger).FullName, typeof(ILogger));
            builder.RegisterType<AdvancedLogger>().As<ILogger>().Named(typeof(AdvancedLogger).FullName, typeof(ILogger));

            var container = builder.Build();

            return new AutofacAdapter(container);
        }

        public override void GetAllInstances() {}
    }
}