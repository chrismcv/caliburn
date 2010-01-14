﻿namespace Caliburn.DynamicProxy
{
    using System.ComponentModel;
    using Core;

    /// <summary>
    /// Handles <see cref="INotifyPropertyChanged"/> on classes that already implement the interface.
    /// </summary>
    public class NotifyPropertyChangedNoInterfaceInterceptor : NotifyPropertyChangedBaseInterceptor
    {
        /// <summary>
        /// Called to raise a property change notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected override void OnPropertyChanged(object sender, string propertyName)
        {
            var notifier = sender as INotifyPropertyChangedEx;
            if(notifier != null) notifier.NotifyOfPropertyChange(propertyName);
        }
    }
}