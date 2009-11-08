using System.Linq;
using Caliburn.Testability;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes.Model;
using Tests.Caliburn.Fakes.UI;

namespace Tests.Caliburn.Testability
{
    [TestFixture]
    public class When_testing_data_templates : TestBase
    {
        [Test]
        public void can_locate_errors_in_items_control_templates()
        {
            var validator = Validator.For<UIBoundToCustomerWithItemsControl, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(3));
        }

        [Test]
        public void can_locate_errors_in_content_control_templates()
        {
            var validator = Validator.For<UIBoundToCustomerWithContentControl, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_triggers()
        {
            var validator = Validator.For<UIBoundToCustomerWithContentControlAndTriggers, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_multi_triggers()
        {
            var validator = Validator.For<UIBoundToCustomerWithContentControlAndMultiTriggers, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_group_styles()
        {
            var validator = Validator.For<UIWithItemsControlGroupStyleTemplate, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }
    }
}