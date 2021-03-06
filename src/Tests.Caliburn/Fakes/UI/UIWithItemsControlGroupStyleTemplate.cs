﻿namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIWithItemsControlGroupStyleTemplate : UserControl
    {
        public UIWithItemsControlGroupStyleTemplate()
        {
            var stack = new StackPanel();
            Content = stack;

            var itemsControl = new ItemsControl();

            itemsControl.GroupStyle.Add(
                new GroupStyle {
                    HeaderTemplate = CreateItemTemplate()
                });

            stack.Children.Add(itemsControl);
        }

        DataTemplate CreateItemTemplate()
        {
            var template = new DataTemplate();

            template.VisualTree = new FrameworkElementFactory(typeof(StackPanel));

            var trigger = new DataTrigger();
            trigger.Binding = new Binding("Cty"); //misspelling
            trigger.Value = "Tallahassee";

            template.Triggers.Add(trigger);

            return template;
        }
    }
}