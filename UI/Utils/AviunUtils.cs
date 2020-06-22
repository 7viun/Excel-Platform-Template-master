using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace UI.Utils
{
    public class AviunUtils
    {
        #region Helpers for parameters
        /// <summary>
        /// Helper to return parameter value as string.
        /// One can also use param.AsValueString() to
        /// get the user interface representation.
        /// Labutils/jeremytammmik
        /// </summary>
        public static string GetParameterValue(Parameter param)
        {
            string s;
            switch (param.StorageType)
            {
                case StorageType.Double:
                    //
                    // the internal database unit for all lengths is feet.
                    // for instance, if a given room perimeter is returned as
                    // 102.36 as a double and the display unit is millimeters,
                    // then the length will be displayed as
                    // peri = 102.36220472440
                    // peri * 12 * 25.4
                    // 31200 mm
                    //
                    //s = param.AsValueString(); // value seen by user, in display units
                    //s = param.AsDouble().ToString(); // if not using not using LabUtils.RealString()
                    s = param.AsValueString();
                    break;

                case StorageType.Integer:
                    s = param.AsInteger().ToString();
                    break;

                case StorageType.String:
                    s = param.AsString();
                    break;

                case StorageType.ElementId:
                    s = param.AsElementId().IntegerValue.ToString();
                    break;

                case StorageType.None:
                    s = "?NONE?";
                    break;

                default:
                    s = "?ELSE?";
                    break;
            }
            return s;
        }
        #endregion
        #region Loop Method
        public static void LoopThrough(int count, Action action)
        {
            for (int i = 0; i < count; i++)
            {
                action();
            }
        }
        #endregion

        #region Convert Column Index to Letter
        /// <summary>
        /// Convert from column index to column letter in Excel
        /// </summary>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public static string ColumnIndexToColumnLetter(int colIndex)
        {
            int div = colIndex;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }
        #endregion
        #region Replace .ART with .XML
        public static string ArtToXml(string input)
        {
            string output;
            output = input.Replace(".ART", ".xml");
            return output;
        }
        #endregion
    }
}
