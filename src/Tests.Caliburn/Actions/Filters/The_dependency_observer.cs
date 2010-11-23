﻿namespace Tests.Caliburn.Actions.Filters
{
    using System;
    using System.Collections.Generic;
    using global::Caliburn.Core;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class The_dependency_observer : TestBase
    {
        IRoutedMessageHandler _handler;
        TheNotifierClass _notifier;
        DependencyObserver _observer;
        IMessageTrigger _trigger;
        bool _expectationsWasSet;

        protected override void given_the_context_of()
        {
            _expectationsWasSet = false;

            var methodFactory = new DefaultMethodFactory();

            _handler = StrictMock<IRoutedMessageHandler>();
            _notifier = new TheNotifierClass();
            _observer = new DependencyObserver(_handler, methodFactory, _notifier);
            _trigger = Mock<IMessageTrigger>();
        }

        void ConfigureObserver(IEnumerable<string> dependencies)
        {
            _observer.MakeAwareOf(_trigger, dependencies);
        }

        void ExpectTriggerUpdate(int count)
        {
            _handler.Expect(x => x.UpdateAvailability(_trigger)).Repeat.Times(count);
            _expectationsWasSet = true;
        }

        void AssertTriggerUpdateExpectations()
        {
            if(!_expectationsWasSet)
                throw new Exception("Expectations was not set");
            _handler.VerifyAllExpectations();
        }

        internal class TheNotifierClass : PropertyChangedBase
        {
            TheReferencedClass _anotherInstance = new TheReferencedClass();
            TheReferencedClass _instance = new TheReferencedClass();

            public int SomeProperty
            {
                get { return 0; }
            }

            public string SomeOtherProperty
            {
                get { return string.Empty; }
            }

            public TheReferencedClass Model
            {
                get { return _instance; }
                set
                {
                    _instance = value;
                    NotifyOfPropertyChange("Model");
                }
            }

            public TheReferencedClass AnotherModel
            {
                get { return _anotherInstance; }
                set
                {
                    _anotherInstance = value;
                    NotifyOfPropertyChange("AnotherModel");
                }
            }
        }

        internal class TheReferencedClass : PropertyChangedBase
        {
            public int SubscriptionCount = 0;

            //public override event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
            //{
            //    add { base.PropertyChanged += value; SubscriptionCount++; }
            //    remove { SubscriptionCount--; base.PropertyChanged -= value; }
            //}


            public int SomeModelProperty
            {
                get { return 0; }
            }

            public int AnotherModelProperty
            {
                get { return 0; }
            }
        }

        [Test]
        //ref http://caliburn.codeplex.com/WorkItem/View.aspx?WorkItemId=6100
        //see also http://caliburn.codeplex.com/Thread/View.aspx?ThreadId=212171
        public void backreferences_should_not_leak_the_observer()
        {
            var handlerRef = new WeakReference(_handler);

            //this reference emulates a back pointer to a long-living model
            var parent = _notifier.Model;


            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });

            //emulates the collection of the cluster composed by Screen, View, MessageHandler and ancillary filters 
            //(included Dependecies along with its internal PropertyPathMonitor)
            _observer = null;
            _handler = null;
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();


            Assert.That(handlerRef.IsAlive, Is.False, "message handler has not been released");


            //the first time a ppMonitor is notified AFTER the collection of its DependenyObserver,
            //it unregisters the unnecessary handler
            parent.NotifyOfPropertyChange("anyProperty");

            Assert.That(parent.SubscriptionCount, Is.EqualTo(0), "subscription to parent model has not been removed");
        }


        [Test, Ignore("NOTE: to make this test pass, the finalizer of DependencyObserver should be in place")]
		
        //see http://caliburn.codeplex.com/Thread/View.aspx?ThreadId=212171 for the rationale behind the finalizer removal
        public void backreferences_should_not_leak_the_observer_strict()
        {
            var handlerRef = new WeakReference(_handler);
            var parent = _notifier.Model;

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });

            _observer = null;
            _handler = null;
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            Assert.That(handlerRef.IsAlive, Is.False);
            Assert.That(parent.SubscriptionCount, Is.EqualTo(0));
        }

        [Test]
        public void should_allow_nodes_collection()
        {
            ExpectTriggerUpdate(1); //strict mock requires expectations

            var disconnectedChainRef = new WeakReference(_notifier.Model);

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            _notifier.Model = new TheReferencedClass();

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            Assert.That(disconnectedChainRef.IsAlive, Is.False);
        }

        [Test]
        public void should_detect_changes_on_intermediate_node()
        {
            ExpectTriggerUpdate(1);
            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            _notifier.Model = new TheReferencedClass();
            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_detect_registered_changes_on_referenced_model()
        {
            ExpectTriggerUpdate(1);

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_detect_registered_changes_on_target()
        {
            ExpectTriggerUpdate(1);

            ConfigureObserver(new[] {
                "SomeProperty"
            });
            _notifier.NotifyOfPropertyChange("SomeProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_detect_star_changes_on_leaf_node()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] {
                "Model.*"
            });
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");
            _notifier.Model.NotifyOfPropertyChange("AnotherModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_detect_star_changes_on_root()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] {
                "*"
            });
            _notifier.NotifyOfPropertyChange("SomeProperty");
            _notifier.NotifyOfPropertyChange("SomeOtherProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_ignore_changes_on_deeper_path()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] {
                "Model"
            });
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_ignore_changes_on_disconnected_chains()
        {
            ExpectTriggerUpdate(1); //first call is expected, second it's not

            var disconnectedChain = _notifier.Model;

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            _notifier.Model = new TheReferencedClass();
            disconnectedChain.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_ignore_changes_on_unregistered_path()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] {
                "AnotherModel.SomeModelProperty"
            });
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_ignore_unregistered_changes_on_target()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] {
                "SomeProperty"
            });
            _notifier.NotifyOfPropertyChange("SomeOtherProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_monitor_multiple_paths()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] {
                "Model.*", "AnotherModel.SomeModelProperty"
            });
            _notifier.Model.NotifyOfPropertyChange("AnotherModelProperty");
            _notifier.AnotherModel.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_reconnect_monitor_on_changed_chain()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            _notifier.Model = new TheReferencedClass();
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_reconnect_monitor_on_previously_null_nodes()
        {
            ExpectTriggerUpdate(2);

            _notifier.Model = null;

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            _notifier.Model = new TheReferencedClass();

            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_throw_exception_if_property_dose_not_exists_on_target()
        {
            Assert.Throws<CaliburnException>(() =>{
                ConfigureObserver(new[] {
                    "NotExistingProperty"
                });
            });
        }

        [Test]
        public void should_throw_on_star_invalid_use()
        {
            Assert.Throws(Is.InstanceOf<Exception>().And.Message.Contains("'*' marker in path"), () =>{
                ConfigureObserver(new[] {
                    "*.xxx"
                });
            });
        }
    }
}