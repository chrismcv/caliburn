﻿namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using NUnit.Framework;

    [TestFixture]
    public class When_testing_priority_bindings
    {
        [Test]
        public void can_detect_bad_prioritybindings_in_styles()
        {
            var validator = Validator.For<UIWithPrioritybindingToStyle, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_detect_bad_prioritybindings_in_triggers()
        {
            var validator = Validator.For<UIWithPrioritybindingStyleAndTriggers, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_detect_bad_prioritybindings_on_elements()
        {
            var validator = Validator.For<SimpleUIWithPriorityBinding, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }
    }
}