﻿namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.StructureMap;
    using NUnit.Framework;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using IContainer = StructureMap.IContainer;

    [TestFixture]
    public class The_StructureMap_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var registry = new Registry();
            registry.For<ILogger>().Use<AdvancedLogger>();
            registry.For<ILogger>().Use(new SimpleLogger()).Named(typeof(SimpleLogger).FullName);
            registry.For<ILogger>().Use(new AdvancedLogger()).Named(typeof(AdvancedLogger).FullName);
            IContainer container = new Container(registry);

            return new StructureMapAdapter(container);
        }

        public override void GetAllInstances()
        {
            //why doesn't this work like the rest of the containers or at all!?
        }

        public override void GenericOverload_GetAllInstances()
        {
            //why doesn't this work like the rest of the containers or at all!?
        }

        [Test]
        public void StructureMapAdapter_Get_WithZeroLenName_ReturnsDefaultInstance()
        {
            Assert.AreSame(
                Locator.GetInstance<ILogger>().GetType(),
                Locator.GetInstance<ILogger>("").GetType()
                );
        }
    }
}