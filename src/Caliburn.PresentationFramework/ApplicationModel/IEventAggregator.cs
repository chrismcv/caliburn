namespace Caliburn.PresentationFramework.ApplicationModel {
    using System;

    /// <summary>
    ///   Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        ///   Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "instance">The instance to subscribe for event publication.</param>
        void Subscribe(object instance);

        /// <summary>
        ///   Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "instance">The instance to unsubscribe.</param>
        void Unsubscribe(object instance);

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name="marshall">Allows the publisher to provide a custom thread marshaller for the message publication. The default uses the UI thread marshaller.</param>
        void Publish(object message, Action<Action> marshall);
    }
}