namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Actions;
    using Conventions;
    using Core.Metadata;
    using Filters;

    /// <summary>
    /// The default implementation of <see cref="IViewModelDescription"/>.
    /// </summary>
    public class DefaultViewModelDescription : MetadataContainer, IViewModelDescription
    {
        private readonly IConventionManager _conventionManager;
        private readonly Type _targetType;
        private readonly Dictionary<string, IAction> _actions = new Dictionary<string, IAction>();
        private readonly Dictionary<Type, IViewApplicable[]> _viewConventions = new Dictionary<Type, IViewApplicable[]>();
        private readonly PropertyInfo[] _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelDescription"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="targetType">Type of the target.</param>
        public DefaultViewModelDescription(IConventionManager conventionManager, Type targetType)
        {
            _conventionManager = conventionManager;
            _targetType = targetType;
            _properties = targetType.GetProperties();

            AddMetadataFrom(targetType);
        }

        /// <summary>
        /// Gets the View Model's type.
        /// </summary>
        /// <value>The type of the View Model.</value>
        public Type TargetType
        {
            get { return _targetType; }
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public IFilterManager Filters { get; set; }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>The actions.</value>
        public IEnumerable<IAction> Actions
        {
            get { return _actions.Values; }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public IEnumerable<PropertyInfo> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public IAction GetAction(ActionMessage message)
        {
            IAction found;
            _actions.TryGetValue(message.MethodName, out found);
            return found;
        }

        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void AddAction(IAction action)
        {
            _actions[action.Name] = action;
        }

        public void SetConventionsFor(Type viewType, IEnumerable<IViewApplicable> applicableConventions)
        {
            _viewConventions[viewType] = applicableConventions.ToArray();
        }

        public IEnumerable<IViewApplicable> GetConventionsFor(DependencyObject view)
        {
            IViewApplicable[] conventions;

            if(!_viewConventions.TryGetValue(view.GetType(), out conventions))
            {
                conventions = _conventionManager.DetermineConventions(this, view).ToArray();
                _viewConventions[view.GetType()] = conventions;
            }

            return conventions;
        }
    }
}