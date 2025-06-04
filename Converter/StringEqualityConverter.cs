using System;
using System.Globalization;
using System.Windows.Data;

namespace uploadyahua.Converter
{
    /// <summary>
    /// �ַ��������ת���������ڽ��ַ���ֵ��ָ�������Ƚϣ����ز���ֵ
    /// ��Ҫ����RadioButton��
    /// </summary>
    public class StringEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ���ֵΪ�գ�����false
            if (value == null)
                return false;

            // �Ƚ��ַ���ֵ������Ƿ����
            string stringValue = value.ToString();
            string paramValue = parameter?.ToString() ?? string.Empty;

            return stringValue.Equals(paramValue, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ���ֵ���ǲ������ͻ�Ϊfalse������null
            if (!(value is bool) || !(bool)value)
                return null;

            // ���ֵΪtrue�����ز���
            return parameter?.ToString() ?? string.Empty;
        }
    }
} 