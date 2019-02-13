using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Config
{
    /// <summary>
    /// A property with a name and its associated value.
    /// </summary>
    public class Property
    {
        private PropertyItem item;
        private string value;

        /// <summary>
        /// The name of the property
        /// </summary>
        public PropertyItem Item { get => item; set => item = value; }

        /// <summary>
        /// The value of the property
        /// </summary>
        public string Value { get => value; set => this.value = value; }

        /// <summary>
        /// Generate a list of properties with their default values
        /// </summary>
        /// <returns>A complete list of properties</returns>
        public static List<Property> GetDefaultValues()
        {
            int i = 0;
            string currentProperty;
            PropertyItem[] propertyItems = (PropertyItem[])Enum.GetValues(typeof(PropertyItem));
            List<Property> defaultProperties = new List<Property>();
            string[] propertiesDefaultValues =
            {
                "None",
                "False",
                "False",
                "False",
                "True",
                "New",
                "Next",
                "2.0",
                @"C:\Program Files (x86)\Steam\steamapps\common\Mordhau\Mordhau\Config",
                "0",
                "0",
                "30",
                "24",
                "False",
                "0|0",
                "0|0",
                "0|0",
                "0|0"
            };
            foreach (PropertyItem propertyItem in propertyItems)
            {
                if (i < propertiesDefaultValues.Length)
                {
                    currentProperty = propertiesDefaultValues[i];
                }
                else
                {
                    currentProperty = "NONE";
                }
                defaultProperties.Add(new Property() { Item = propertyItem , value = currentProperty });
                i++;
            }
            defaultProperties.RemoveAt(0);
            return defaultProperties;
        }

        public static PropertyItem FromString(string name)
        {
            PropertyItem result;
            if (Enum.TryParse(name, out result))
            {
                return result;
            }
            else
            {
                return PropertyItem.None;
            }
        }
    }
}
