using System.Windows;
using System.Windows.Controls;
using AssetManager.Models;
namespace AssetManager
{
   public class SpecificationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate OptionTemplate { get; set; }
    

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
            AssetGroupSpecificationValuesModel _ag = item as AssetGroupSpecificationValuesModel;           
            if( _ag != null)
            { 
                int templateid= _ag.DataTypeID;
                if ((int) MeasurementUnits.Integer == templateid 
                    || (int)MeasurementUnits.Decimal == templateid
                    || (int)MeasurementUnits.String == templateid)
                    return TextTemplate;
                else
                    if ((int)MeasurementUnits.Boolean == templateid)
                    return BooleanTemplate;
                else
                    if ((int)MeasurementUnits.Option == templateid)
                    return OptionTemplate;
                else
                    return TextTemplate;
            }
            else
                return TextTemplate;
    }

}

   

}
