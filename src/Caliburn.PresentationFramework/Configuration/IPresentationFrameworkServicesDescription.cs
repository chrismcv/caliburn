namespace Caliburn.PresentationFramework.Configuration
{
    using Actions;
    using ApplicationModel;
    using Core.IoC;
    using PresentationFramework;
    using Parsers;
    using ViewModels;

    public interface IPresentationFrameworkServicesDescription
    {
        /// <summary>
        /// Customizes the routed message controller used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The routed message controller type.</typeparam>
        Singleton RoutedMessageController<T>() where T : IRoutedMessageController;

        /// <summary>
        /// Customizes the method binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The method binder type.</typeparam>
        Singleton MessageBinder<T>() where T : IMessageBinder;

        /// <summary>
        /// Customizes the message parser used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The message parser type.</typeparam>
        Singleton Parser<T>() where T : IParser;

        /// <summary>
        /// Customizes the view model description builder.
        /// </summary>
        /// <typeparam name="T">The action factory type.</typeparam>
        Singleton ViewModelDescriptionBuilder<T>() where T : IViewModelDescriptionFactory;

        /// <summary>
        /// Customizes the actions locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Singleton ActionLocator<T>() where T : IActionLocator;

        /// <summary>
        /// Customizes the view strategy used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The view strategy type.</typeparam>
        Singleton ViewStrategy<T>() where T : IViewStrategy;

        /// <summary>
        /// Customizes the binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The binder type.</typeparam>
        Singleton Binder<T>() where T : IBinder;

#if !SILVERLIGHT

        /// <summary>
        /// Customizes the window manager used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The window manager type.</typeparam>
        Singleton WindowManager<T>() where T : IWindowManager;
#endif
    }
}