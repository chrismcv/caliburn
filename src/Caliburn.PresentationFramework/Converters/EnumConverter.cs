﻿namespace Caliburn.PresentationFramework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using ViewModels;

    /// <summary>
    /// An <see cref="IValueConverter"/> for <see cref="BindableEnum"/>.
    /// </summary>
    public class EnumConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(typeof(int).IsAssignableFrom(targetType))
                return System.Convert.ToInt32(value);

			if (typeof(byte).IsAssignableFrom(targetType))
				return System.Convert.ToByte(value);

            return BindableEnum.Create(value);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return null;

            if(value is string)
                return Enum.Parse(targetType, value.ToString(), true);

            return value.GetType() == targetType
                       ? value
                       : ((BindableEnum)value).Value;
        }
    }
}