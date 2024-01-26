using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Tools
{
    public enum ConvertType
    {
        COLOR,
        BOOL,
        STRING
    }

    public class TypeStringConverter
    {
        static string underlineSeperator = "_";

        public static string ToString(object obj, Type type)
        {
            if (type == typeof(Color))
            {
                Color color = (Color)obj;
                return color.A + underlineSeperator +
                    color.R + underlineSeperator +
                    color.G + underlineSeperator +
                color.B;
            }
            else if (type == typeof(bool))
            {
                return obj.ToString();
            }

            return null;
        }

        public static object ToObject(string dataString, Type type)
        {
            if (type == typeof(Color))
            {
                String[] colorData = dataString.Split(Char.Parse(underlineSeperator));

                if (colorData.Length == 4)
                {
                    int A = int.Parse(colorData[0]);
                    int R = int.Parse(colorData[1]);
                    int G = int.Parse(colorData[2]);
                    int B = int.Parse(colorData[3]);
                    return Color.FromArgb(A, R, G, B);
                }
                return Color.Black;
            }
            else if (type == typeof(bool))
            {
                return bool.Parse(dataString);
            }

            return null;
        }
    }
}
