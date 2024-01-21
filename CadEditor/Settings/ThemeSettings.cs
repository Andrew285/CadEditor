using System.Drawing;
using System.Management.Instrumentation;

namespace CadEditor.Settings
{
    public class ThemeSettings: MainSettings
    {
        private static ThemeSettings instance;
        public static Color MainThemeColor = Color.White;
        public static Color MenuStripBackColor = Color.White;

        public static ThemeSettings GetInstance()
        {
            if (instance == null)
            {
                instance = new ThemeSettings();
            }

            return instance;
        }
    }
}
