using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace AssetManager.Converters
{

    public class StringToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                bool blnDate = DateTime.TryParse(value.ToString(), out DateTime enteredDate);
                if (blnDate)
                    return enteredDate;
                else
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            else
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    public class StringToIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                bool blnInteger = int.TryParse(value.ToString(), out int enteredInteger);
                if (blnInteger)
                    return enteredInteger;
                else
                    return 0;
            }
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                bool blnDecimal = decimal.TryParse(value.ToString(), out decimal enteredDecimal);
                if (blnDecimal)
                    return enteredDecimal;
                else
                    return 0.0;
            }
            else
                return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    public class DataTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value.GetType();
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                DateTime _newdate;
                bool _isdate = DateTime.TryParse(value.ToString(), out _newdate);
                if (_isdate &&  _newdate.Date < DateTime.Now.Date)
                {
                    return new SolidColorBrush(Colors.LightGray);
                }    
                else
                    return new SolidColorBrush(Colors.White);
            }
            else
                return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(Path.Combine(GlobalClass.Defaults.PhotosFileLocation, value.ToString()));
                image.EndInit();

                return image;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MeasurementUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {                
                    if ((int)value == (int)(MeasurementUnits.Boolean))                    
                        return EnumerationManager.GetEnumDescription(MeasurementUnits.Boolean);                    
                    else
                    if ((int)value == (int)(MeasurementUnits.Decimal))
                        return EnumerationManager.GetEnumDescription(MeasurementUnits.Decimal);
                    else
                    if ((int)value == (int)(MeasurementUnits.Integer))
                        return EnumerationManager.GetEnumDescription(MeasurementUnits.Integer);
                    else
                    if ((int)value == (int)(MeasurementUnits.Option))
                        return EnumerationManager.GetEnumDescription(MeasurementUnits.Option);
                    else
                    if ((int)value == (int)(MeasurementUnits.String))
                        return EnumerationManager.GetEnumDescription(MeasurementUnits.String);
                    else
                        return EnumerationManager.GetEnumDescription(MeasurementUnits.String);

            }
            catch
            {
                return EnumerationManager.GetEnumDescription(MeasurementUnits.Boolean);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }




    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                //look up asset for parent
                if (System.Convert.ToInt32(value) != 0)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
            catch
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverterCollapse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                //look up asset for parent
                if (System.Convert.ToInt32(value) != 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
        
    public class ImageVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@Application.Current.Resources["PhotosPath"].ToString() + value);
                image.EndInit();

                return Visibility.Visible;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class ByteArrayToImageConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        try
    //        {
    //            //converter from byte[] to image
    //            Stream StreamObj = new MemoryStream((byte[])value);
    //            BitmapImage BitObj = new BitmapImage();
    //            BitObj.BeginInit();
    //            BitObj.StreamSource = StreamObj;
    //            BitObj.EndInit();
    //            return BitObj;                
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    public class TrueFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (System.Convert.ToBoolean(value) == false)
                    return "No";
                else
                    return "Yes";
            }
            catch
            {
                return "No";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class SubtItemsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Models.AssetGroupModel ag = value as Models.AssetGroupModel;
                if (ag.CanBeParent.ToString() == "True")
                    return (bool)true;
                else
                    return (bool)false;
            }
            catch
            {
                return (bool)false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class MultiParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
          //  if (values[1] != null)
          //  {                
               return values.Clone();
           // }
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class CSVSplitter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<string> _strcoll = new List<string>();
            try
            {
                string _s = value.ToString();                
                _strcoll = _s.Split(',').ToList();                
                return _strcoll;
            }
            catch
            {
                return _strcoll;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                bool _boolvalue;
                bool _isbool = bool.TryParse(value.ToString(), out _boolvalue);
                if (_isbool)
                    return _boolvalue;
                else
                    return false;                
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
          

    public class TextBlockForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value as bool?) == true)
                if ((string)parameter == "1")
                    return Brushes.Blue;
                else
                    return Brushes.Black;
            else
                return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

       
       
}



