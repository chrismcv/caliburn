﻿namespace Tests.Caliburn.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Fakes;
    using global::Caliburn.Core;
    using global::Caliburn.Core.Configuration;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.IoC;
    using global::Caliburn.Core.Threading;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class The_core_configuration : TestBase
    {
        private CoreConfiguration _config;
        private IModule _module;

        protected override void given_the_context_of()
        {
            _config = ConventionalModule<CoreConfiguration, ICoreServicesDescription>.Instance;
            _module = _config;
        }

        [Test]
        public void when_started_configures_required_components_and_children()
        {
            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IDispatcher)
                         select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IThreadPool)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IMethodFactory)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IEventHandlerFactory)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IAssemblySource)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);
        }

        [Test]
        public void can_provide_a_custom_dispatcher()
        {
            _config.Using(x => x.Dispatcher<DefaultDispatcher>());

            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IDispatcher)
                         select reg).FirstOrDefault();

            Assert.That(found.Implementation, Is.EqualTo(typeof(DefaultDispatcher)));
        }

        [Test]
        public void can_provide_a_custom_thread_pool()
        {
            _config.Using(x => x.ThreadPool<FakeThreadPool>());

            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IThreadPool)
                         select reg).FirstOrDefault();

            Assert.That(found.Implementation, Is.EqualTo(typeof(FakeThreadPool)));
        }

        [Test]
        public void can_provide_a_custom_method_factory()
        {
            _config.Using(x => x.MethodFactory<FakeMethodFactory>());

            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IMethodFactory)
                         select reg).FirstOrDefault();

            Assert.That(found.Implementation, Is.EqualTo(typeof(FakeMethodFactory)));
        }

        [Test]
        public void can_provide_a_custom_event_handler_factory()
        {
            _config.Using(x => x.EventHandlerFactory<FakeEventHandlerFactory>());

            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IEventHandlerFactory)
                         select reg).FirstOrDefault();

            Assert.That(found.Implementation, Is.EqualTo(typeof(FakeEventHandlerFactory)));
        }

        private class FakeMethodFactory : IMethodFactory
        {
            public IMethod CreateFrom(MethodInfo methodInfo)
            {
                throw new NotImplementedException();
            }
        }

        private class FakeEventHandlerFactory : IEventHandlerFactory
        {
            public IEventHandler Wire(object sender, string eventName)
            {
                throw new NotImplementedException();
            }

            public IEventHandler Wire(object sender, EventInfo eventInfo)
            {
                throw new NotImplementedException();
            }
        }
    }
}