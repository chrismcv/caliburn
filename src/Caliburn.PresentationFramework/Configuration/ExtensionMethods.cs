﻿namespace Caliburn.PresentationFramework.Configuration
{
    using Core.Configuration;

    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds the presentation framework module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public static PresentationFrameworkConfiguration PresentationFramework(this IModuleHook hook)
        {
            return hook.Module(CaliburnModule<PresentationFrameworkConfiguration>.Instance);
        }
    }
}