using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HG_ServerUI
{
    // https://stackoverflow.com/questions/66104868/how-to-bind-liststring-and-display-its-collection-into-a-datagridtemplatecolum
    public class ListToStringConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is List<string> lstStr)
            {
                return string.Join(",", lstStr);
            }
            return value?.GetType().ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("One way converter!");
        }
    }
}
