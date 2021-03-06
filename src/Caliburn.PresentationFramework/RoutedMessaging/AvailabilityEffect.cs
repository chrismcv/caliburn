namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System.Windows;
    using System.Windows.Controls;
    using Conventions;

    /// <summary>
    /// Common implementations of <see cref="IAvailabilityEffect"/>.
    /// </summary>
    public static class AvailabilityEffect
    {
        /// <summary>
        /// The element is not affected by changes in availability.
        /// </summary>
        public static readonly IAvailabilityEffect None = new NoneEffect();

        /// <summary>
        /// The element is not affected by changes in availability.
        /// </summary>
        private class NoneEffect : IAvailabilityEffect
        {
            /// <summary>
            /// Applies the effect to the target.
            /// </summary>
            /// <param name="target">The element.</param>
            /// <param name="isAvailable">Determines how the effect will be applied to the target.</param>
            public void ApplyTo(DependencyObject target, bool isAvailable) { }
        }

        /// <summary>
        /// This effect can disable the UI.
        /// </summary>
        public static readonly IAvailabilityEffect Disable = new DisableEffect();

        /// <summary>
        /// This effect can disable the UI.
        /// </summary>
        private class DisableEffect : IAvailabilityEffect
        {
            /// <summary>
            /// Applies the effect to the target.
            /// </summary>
            /// <param name="target">The element.</param>
            /// <param name="isAvailable">Determines how the effect will be applied to the target.</param>
            public void ApplyTo(DependencyObject target, bool isAvailable)
            {
#if SILVERLIGHT
                var control = target as Control;
                if (control == null)
                    return;

                if (control.HasBinding(Control.IsEnabledProperty))
                    return;

                if (control.IsEnabled && !isAvailable)
                    control.IsEnabled = false;
                else if (!control.IsEnabled && isAvailable)
                    control.IsEnabled = true;
#else
                var element = target as UIElement;
                if (element != null)
                {
                    if (element.HasBinding(ContentElement.IsEnabledProperty))
                        return;

                    if (element.IsEnabled && !isAvailable)
                        element.IsEnabled = false;
                    else if (!element.IsEnabled && isAvailable)
                        element.IsEnabled = true;
                }
                else
                {
                    var ce = target as ContentElement;
                    if (ce != null)
                    {
                        if (ce.HasBinding(ContentElement.IsEnabledProperty))
                            return;

                        if (ce.IsEnabled && !isAvailable)
                            ce.IsEnabled = false;
                        else if (!ce.IsEnabled && isAvailable)
                            ce.IsEnabled = true;
                    }
                }
#endif
            }
        }

        /// <summary>
        /// This effect can hide the UI.
        /// </summary>
        public static readonly IAvailabilityEffect Hide = new HideEffect();

        /// <summary>
        /// This effect can hide the UI.
        /// </summary>
        private class HideEffect : IAvailabilityEffect
        {
#if SILVERLIGHT
            public static readonly DependencyProperty OldOpacityProperty =
                DependencyProperty.RegisterAttached(
                    "OldOpacity",
                    typeof(double),
                    typeof(HideEffect),
                    null
                );
#endif

            /// <summary>
            /// Applies the effect to the target.
            /// </summary>
            /// <param name="target">The element.</param>
            /// <param name="isAvailable">Determines how the effect will be applied to the target.</param>
            public void ApplyTo(DependencyObject target, bool isAvailable)
            {
#if SILVERLIGHT
                var element = target as UIElement;
                if(element == null) 
                    return;

                if (element.Opacity != 0 && !isAvailable) 
                {
                    element.SetValue(OldOpacityProperty, element.Opacity);
                    element.Opacity = 0;
                }
                else if (element.Opacity == 0 && isAvailable) 
                {
                    object oldValue = element.GetValue(OldOpacityProperty);
                    double opacity = oldValue == null ? 1 : (double)oldValue;
                    element.Opacity = opacity;
                }
#else
                var element = target as UIElement;
                if (element == null)
                    return;

                if (element.Visibility == Visibility.Visible && !isAvailable)
                    element.Visibility = Visibility.Hidden;
                else if (element.Visibility != Visibility.Visible && isAvailable)
                    element.Visibility = Visibility.Visible;
#endif
            }
        }

        /// <summary>
        /// This effect can collapse the UI.
        /// </summary>
        public static readonly IAvailabilityEffect Collapse = new CollapseEffect();

        /// <summary>
        /// This effect can collapse the UI.
        /// </summary>
        private class CollapseEffect : IAvailabilityEffect
        {
            /// <summary>
            /// Applies the effect to the target.
            /// </summary>
            /// <param name="target">The element.</param>
            /// <param name="isAvailable">Determines how the effect will be applied to the target.</param>
            public void ApplyTo(DependencyObject target, bool isAvailable)
            {
                var element = target as UIElement;
                if (element == null)
                    return;

                if (element.Visibility == Visibility.Visible && !isAvailable) element.Visibility = Visibility.Collapsed;
                if (element.Visibility != Visibility.Visible && isAvailable) element.Visibility = Visibility.Visible;
            }
        }
    }
}