using System;
using System.Windows.Data;

namespace Zen.Ux.WpfApp.Converters
{
    /// <summary>
    /// Converts image Id into a path from the Images\Index folder.
    /// </summary>
    public class ImageConverter : IValueConverter
    {
        /// <returns>Image path.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string size = (string)parameter;

            int id = int.Parse(value.ToString());
            if (id > 91) id = 0; // New customers are getting the default silhouette icon.

            return "Images/Index/" + size + "/" + id + ".jpg";
        }           

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
