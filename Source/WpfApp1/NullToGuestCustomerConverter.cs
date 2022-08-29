using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp1
{
    /// <summary>
    /// Null to guest customer converter
    /// </summary>
    public class NullToGuestCustomerConverter : IValueConverter
    {
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>Result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string)value;
            string result = name;
            if (name == null || name.Length == 0) {
                result = "(chưa có thông tin)";
            }
            return result;
        }

        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
