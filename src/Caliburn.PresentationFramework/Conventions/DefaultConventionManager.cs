namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using Actions;
    using Core;
    using Core.Invocation;
    using Filters;
    using ViewModels;

    /// <summary>
    /// The default implementation of <see cref="IConventionManager"/>.
    /// </summary>
    public class DefaultConventionManager : IConventionManager
    {
        private readonly IMethodFactory _methodFactory;
        private readonly IEventHandlerFactory _eventHandlerFactory;

        private readonly Dictionary<Type, IElementConvention> _elementConventions = new Dictionary<Type, IElementConvention>();
        private readonly List<IBindingConvention> _bindingConventions = new List<IBindingConvention>();
        private readonly List<IActionConvention> _actionConventions = new List<IActionConvention>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConventionManager"/> class.
        /// </summary>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        public DefaultConventionManager(IMethodFactory methodFactory, IEventHandlerFactory eventHandlerFactory)
        {
            _methodFactory = methodFactory;
            _eventHandlerFactory = eventHandlerFactory;

            GetDefaultElementConventions()
                .Apply(AddElementConvention);

            GetDefaultBindingConventions()
                .Apply(AddBindingConvention);

            GetDefaultActionConventions()
                .Apply(AddActionConvention);
        }

        /// <summary>
        /// Adds the element convention.
        /// </summary>
        /// <param name="convention">The convention.</param>
        public void AddElementConvention(IElementConvention convention)
        {
            _elementConventions[convention.Type] = convention;
        }

        /// <summary>
        /// Adds the binding convention.
        /// </summary>
        /// <param name="convention">The convention.</param>
        public void AddBindingConvention(IBindingConvention convention)
        {
            _bindingConventions.Add(convention);
        }

        /// <summary>
        /// Adds the action convention.
        /// </summary>
        /// <param name="convention">The convention.</param>
        public void AddActionConvention(IActionConvention convention)
        {
            _actionConventions.Add(convention);
        }

        /// <summary>
        /// Gets the element convention for the type of element specified.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The convention.</returns>
        public IElementConvention GetElementConvention(Type elementType)
        {
            if (elementType == null) 
                return null;

            IElementConvention convention;

            _elementConventions.TryGetValue(elementType, out convention);

            if (convention == null)
                convention = GetElementConvention(elementType.BaseType);

            return convention;
        }

        /// <summary>
        /// Determines the conventions for a view model and a set of UI elements.
        /// </summary>
        /// <param name="viewModelDescription">The view model description.</param>
        /// <param name="elementDescriptions">The element descriptions.</param>
        /// <returns>The applicable conventions.</returns>
        public virtual IEnumerable<IViewApplicable> DetermineConventions(IViewModelDescription viewModelDescription, IEnumerable<IElementDescription> elementDescriptions)
        {
            foreach (var elementDescription in elementDescriptions)
            {
                var actionMatches = from convention in _actionConventions
                                    from action in viewModelDescription.Actions
                                    where convention.Matches(viewModelDescription, elementDescription, action)
                                    select convention.CreateApplication(viewModelDescription, elementDescription, action);

                foreach (var match in actionMatches)
                {
                    yield return match;
                }

                if(actionMatches.Any())
                    continue;

                var propertyMatches = from convention in _bindingConventions
                                      from property in viewModelDescription.Properties
                                      where convention.Matches(viewModelDescription, elementDescription, property)
                                      select convention.CreateApplication(viewModelDescription, elementDescription, property);

                foreach (var match in propertyMatches)
                {
                    yield return match;
                }
            }
        }

        /// <summary>
        /// Applies the action creation conventions.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="targetMethod">The target method.</param>
        public virtual void ApplyActionCreationConventions(IAction action, IMethod targetMethod)
        {
            var canExecuteName = DeriveCanExecuteName(targetMethod.Info.Name);
            var found = targetMethod.FindMetadata<PreviewAttribute>()
                .FirstOrDefault(x => x.MethodName == canExecuteName);

            if (found != null)
                return;

            var canExecute = targetMethod.Info.DeclaringType.GetMethod(
                                 canExecuteName,
                                 targetMethod.Info.GetParameters().Select(x => x.ParameterType).ToArray()
                                 )
                             ?? targetMethod.Info.DeclaringType.GetMethod("get_" + canExecuteName);

            if (canExecute != null)
                action.Filters.Add(new PreviewAttribute(_methodFactory.CreateFrom(canExecute)));
        }

        /// <summary>
        /// Derives the name of the can execute method/property.
        /// </summary>
        /// <param name="baseName">Name of the base method.</param>
        /// <returns>The conventional name of the can execute poroperty.</returns>
        protected virtual string DeriveCanExecuteName(string baseName)
        {
            return "Can" + baseName;
        }

        /// <summary>
        /// Gets the default binding conventions.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IBindingConvention> GetDefaultBindingConventions()
        {
            yield return new DefaultBindingConvention();
            yield return new ItemsControlBindingConvention();
        }

        /// <summary>
        /// Gets the default action conventions.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IActionConvention> GetDefaultActionConventions()
        {
            yield return new DefaultActionConvention();
        }

        /// <summary>
        /// Gets the default element conventions.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IElementConvention> GetDefaultElementConventions()
        {
#if !SILVERLIGHT
                yield return ElementConvention<RichTextBox>("TextChanged", RichTextBox.DataContextProperty, (c, o) => c.Document = (FlowDocument)o, c => c.Document);
                yield return ElementConvention<Menu>("Click", Menu.ItemsSourceProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<MenuItem>("Click", MenuItem.ItemsSourceProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<Label>("DataContextChanged", Label.ContentProperty, (c, o) => c.Content = o, c => c.Content);
                yield return ElementConvention<DockPanel>("Loaded", DockPanel.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<UniformGrid>("Loaded", UniformGrid.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<WrapPanel>("Loaded", WrapPanel.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<Viewbox>("Loaded", Viewbox.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<BulletDecorator>("Loaded", BulletDecorator.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<Slider>("ValueChanged", Slider.ValueProperty, (c, o) => c.Value = (double)o, c => c.Value);
                yield return ElementConvention<Expander>("Expanded", Expander.IsExpandedProperty, (c, o) => c.IsExpanded = (bool)o, c => c.IsExpanded);
                yield return ElementConvention<UserControl>("Loaded", UserControl.ContentProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<Window>("Loaded", Window.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<StatusBar>("Loaded", StatusBar.ItemsSourceProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<ToolBar>("Loaded", ToolBar.ItemsSourceProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<ToolBarTray>("Loaded", ToolBarTray.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<TreeView>("SelectedItemChanged", TreeView.ItemsSourceProperty, (c, o) => c.ItemsSource = (IEnumerable)o, c => c.SelectedItem);
                yield return ElementConvention<TabControl>("SelectionChanged", TabControl.ItemsSourceProperty, (c, o) => c.ItemsSource = (IEnumerable)o, c => c.SelectedItem);
                yield return ElementConvention<TabItem>("DataContextChanged", TabItem.ContentProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<ListView>("SelectionChanged", ListView.ItemsSourceProperty, (c, o) => c.ItemsSource = (IEnumerable)o,
                                   c =>
                                   {
                                       if (c.SelectionMode == SelectionMode.Extended ||
                                          c.SelectionMode == SelectionMode.Multiple)
                                           return c.SelectedItems;
                                       return c.SelectedItem;
                                   });
                yield return ElementConvention<ListBox>("SelectionChanged", ListBox.ItemsSourceProperty, (c, o) => c.ItemsSource = (IEnumerable)o,
                                  c =>
                                  {
                                      if (c.SelectionMode == SelectionMode.Extended ||
                                         c.SelectionMode == SelectionMode.Multiple)
                                          return c.SelectedItems;
                                      return c.SelectedItem;
                                  });
                yield return ElementConvention<ComboBox>("SelectionChanged", ComboBox.ItemsSourceProperty, (c, o) => c.ItemsSource = (IEnumerable)o, c => c.IsEditable ? c.Text : c.SelectedItem);
#else
                yield return ElementConvention<UserControl>("Loaded", null, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<ListBox>("SelectionChanged", ListBox.ItemsSourceProperty, (c, o) => c.ItemsSource = (IEnumerable) o, c => c.SelectedItem);
                yield return ElementConvention<ComboBox>("SelectionChanged", ComboBox.ItemsSourceProperty, (c, o) => c.ItemsSource = (IEnumerable) o, c => c.SelectedItem);
#endif
                yield return ElementConvention<ButtonBase>("Click", ButtonBase.ContentProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<Button>("Click", Button.ContentProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<ToggleButton>("Click", ToggleButton.IsCheckedProperty, (c, o) => c.IsChecked = (bool)o, c => c.IsChecked);
                yield return ElementConvention<RadioButton>("Click", RadioButton.IsCheckedProperty, (c, o) => c.IsChecked = (bool)o, c => c.IsChecked);
                yield return ElementConvention<CheckBox>("Click", CheckBox.IsCheckedProperty, (c, o) => c.IsChecked = (bool)o, c => c.IsChecked);
                yield return ElementConvention<TextBox>("TextChanged", TextBox.TextProperty, (c, o) => c.Text = o.SafeToString(), c => c.Text);
                yield return ElementConvention<PasswordBox>("PasswordChanged", PasswordBox.DataContextProperty, (c, o) => c.Password = o.SafeToString(), c => c.Password);
                yield return ElementConvention<TextBlock>("DataContextChanged", TextBlock.TextProperty, (c, o) => c.Text = o.SafeToString(), c => c.Text);
                yield return ElementConvention<StackPanel>("Loaded", StackPanel.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<Grid>("Loaded", Grid.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<Border>("Loaded", Border.DataContextProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<ContentControl>("Loaded", ContentControl.ContentProperty, (c, o) => c.DataContext = o, c => c.DataContext);
                yield return ElementConvention<ItemsControl>("Loaded", ItemsControl.ItemsSourceProperty, (c, o) => c.DataContext = o, c => c.DataContext);
        }

        /// <summary>
        /// Creates an element convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultEvent">The default event.</param>
        /// <param name="bindableProperty">The bindable property.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        /// <returns>The element convention.</returns>
        protected virtual IElementConvention ElementConvention<T>(string defaultEvent, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter)
        {
            return new DefaultElementConvention<T>(
                _eventHandlerFactory,
                defaultEvent,
                bindableProperty,
                setter,
                getter
                );
        }
    }
}