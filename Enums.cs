using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace AssetManager
{

    public enum NodeType
    {
        [Description("Corporation")]
        Corporation = 1,
        [Description("Customer")]
        Customer = 2,
        [Description("Asset")]
        Asset = 3,
        [Description("Consumable")]
        Consumable = 4
    }
       
    public enum MeasurementUnits
    {
        [Description("Whole Number")]
        Integer = 1,
        [Description("Decimal")]
        Decimal,
        [Description("Text")]
        String,
        [Description("Yes-No")]
        Boolean,
        [Description("Option")]
        Option

    }   
  
    public enum ActivityType
    {
        NewAsset = 1,
        Transfer = 2,
        Deleted = 3,
        Undeleted = 4      
    }
           
    public enum ReportFieldType
    {
        [Description("Standard")]
        Standard = 0,
        [Description("Hidden")]
        Hidden = 1,
        [Description("Removed")]
        Removed = 2
    }

    public class EnumValue
    {
        public EnumValue() { }

        public Enum Enumvalue
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int ID
        {
            get; set;
        }

    }

    public class EnumerationManager
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static Collection<EnumValue> GetEnumList(Type enumvar)
        {
            Collection<EnumValue> _p = new Collection<EnumValue>();
            EnumValue _enumvalue;
            Array _a = Enum.GetNames(enumvar);
            foreach (Enum name in Enum.GetValues(enumvar))
            {
                _enumvalue = new EnumValue();
                _enumvalue.Description = GetEnumDescription(name);
                _enumvalue.Enumvalue = name;
                _enumvalue.ID = Convert.ToInt32(name);
                _p.Add(_enumvalue);
            }
            return _p;
        }
    }

    public static class EnumerationLists
    {
        #region Enumeration Lists

        //public static Collection<EnumValue> AssetStatusTypesList
        //{
        //    get { return EnumerationManager.GetEnumList(typeof(StatusType)); }
        //}

        #endregion
    }


}
