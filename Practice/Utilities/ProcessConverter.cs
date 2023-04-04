using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Practice
{
    public class ProccessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = ((IEnumerable) value).Cast<object>().ToList();

            var answer = string.Empty;
            var res = result.Select(t => (process) t);
            res = res.Where(t => t.Abiity == true);
            var materials = new List<string>(res.Select(t => t.material.Name));

            answer = string.Join(", ", materials);

            return answer;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}