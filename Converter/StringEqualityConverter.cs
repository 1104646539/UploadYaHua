using System;
using System.Globalization;
using System.Windows.Data;

namespace uploadyahua.Converter
{
    /// <summary>
    /// 字符串相等性转换器，用于将字符串值与指定参数比较，返回布尔值
    /// 主要用于RadioButton绑定
    /// </summary>
    public class StringEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果值为空，返回false
            if (value == null)
                return false;

            // 比较字符串值与参数是否相等
            string stringValue = value.ToString();
            string paramValue = parameter?.ToString() ?? string.Empty;

            return stringValue.Equals(paramValue, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果值不是布尔类型或为false，返回null
            if (!(value is bool) || !(bool)value)
                return null;

            // 如果值为true，返回参数
            return parameter?.ToString() ?? string.Empty;
        }
    }
} 