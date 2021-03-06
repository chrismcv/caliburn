﻿namespace Tests.Caliburn.RoutedUIMessaging.Parsers
{
    using System.Windows.Input;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Parsers;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using NUnit.Framework;

    [TestFixture]
    public class The_gesture_trigger_parser : TestBase
    {
        GestureTriggerParser parser;

        protected override void given_the_context_of()
        {
            parser = new GestureTriggerParser();
        }

        [Test]
        public void can_parse_a_key_combination()
        {
            var result = parser.Parse(null, "Key: S, Modifiers: Control") as GestureMessageTrigger;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Key, Is.EqualTo(Key.S));
            Assert.That(result.Modifiers, Is.EqualTo(ModifierKeys.Control));
        }

        [Test]
        public void can_parse_a_mouse_action()
        {
            var result = parser.Parse(null, "MouseAction: LeftDoubleClick, Modifiers: Alt") as GestureMessageTrigger;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MouseAction, Is.EqualTo(MouseAction.LeftDoubleClick));
            Assert.That(result.Modifiers, Is.EqualTo(ModifierKeys.Alt));
        }
    }
}