using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Extensions
{
    /// <summary>
    /// This class has enum helper for Flagged enums
    /// </summary>
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum value)
        {
            // Check if this is a flagged Enum
            if (value.HasFlagsAttribute<TEnum>())
            {
                var descriptions = new List<string>();
                foreach (TEnum x in Enum.GetValues(typeof(TEnum)))
                {
                    if (value.ToString().Split(',').Where(m => m.Trim() == x.ToString()).Any())
                    {
                        descriptions.Add(x.GetEnumDescription<TEnum>());
                    }
                }

                return string.Join(", ", descriptions);
            }

            return value.GetEnumDescription<TEnum>();
        }

        public static string GetName<TEnum>(this TEnum value)
        {
            // Check if this is a flagged Enum
            if (value.HasFlagsAttribute<TEnum>())
            {
                var descriptions = new List<string>();
                foreach (TEnum x in Enum.GetValues(typeof(TEnum)))
                {
                    if (value.ToString().Split(',').Where(m => m.Trim() == x.ToString()).Any())
                    {
                        descriptions.Add(x.GetEnumName<TEnum>());
                    }
                }

                return string.Join(", ", descriptions);
            }

            return value.GetEnumName<TEnum>();
        }

        public static bool HasFlagsAttribute<TEnum>(this TEnum value)
        {
            return value.GetType().GetCustomAttributes<FlagsAttribute>().Any();
        }


        public static string GetEnumDescription<TEnum>(this TEnum value)
        {
            // variables  
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // return  
            return attributes.Length == 0 ? value.ToString() : ((DescriptionAttribute)attributes[0]).Description;
        }


        public static string GetEnumName<TEnum>(this TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length == 0)
            {
                return Enum.GetName(typeof(TEnum), value);
            }

            return attributes[0].Name;
        }



        /// <summary>
        /// Returns true if permission contains all permissions passed in.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="requiredPermissions"></param>
        /// <returns></returns>
        public static bool HasAllPermissions(this AccountPermissions value, params AccountPermissions[] requiredPermissions)
        {
            var hasPermission = true;
            foreach (var item in requiredPermissions)
            {
                if (!value.HasFlag(item))
                {
                    hasPermission = false;
                    break;
                }
            }

            return hasPermission;
        }
    }

}
