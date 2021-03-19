using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapMyElevation.Resources;

namespace MapMyElevation.Helpers
{
    public class Enumerations
    {
        public enum ElevationUnit
        {
            Miles,
            Kilometers,
            Meters
        }

        // this list MUST be in the same order as the enum above:
        public static List<string> GetElevationUnits()
        {
            return new List<string>() { AppResources.Miles, AppResources.Kilometers, AppResources.Meters };
        }

        public enum DistanceUnit
        {
            Miles,
            Kilometers,
            Meters
        }

        // this list MUST be in the same order as the enum above:
        public static List<string> GetDistanceUnits()
        {
            return new List<string>() { AppResources.Miles, AppResources.Kilometers, AppResources.Meters };
        }

        //// reads enum and returns values as ienumerable<string>:
        //public static IEnumerable<string> GetNames<TEnum>() where TEnum : struct
        //{
        //    var type = typeof(TEnum);
        //    if (!type.IsEnum)
        //        throw new ArgumentException(String.Format("Type '{0}' is not an enum", type.Name));

        //    return (
        //      from field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
        //      where field.IsLiteral
        //      select field.Name)
        //    .ToList<string>();
        //}
    }
}
