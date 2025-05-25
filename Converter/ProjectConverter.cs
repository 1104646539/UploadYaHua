using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using uploadyahua.Model;

namespace uploadyahua.Converter
{
    public class ProjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<Result> results && parameter is string type)
            {
                switch (type)
                {
                    case "TestItem":
                        return string.Join("\n", results.Select(r => r.TestItem));
                    case "TestValue":
                        return string.Join("\n", results.Select(r => r.TestValue));
                    case "TestResult":
                        return string.Join("\n", results.Select(r => r.TestResult));
                    case "Reference":
                        return string.Join("\n", results.Select(r => r.Reference));
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
