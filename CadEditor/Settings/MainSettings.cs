using CadEditor.Tools;
using System;
using System.Reflection;

namespace CadEditor.Settings
{
    public abstract class MainSettings
    {
        public static string FilePath { get; private set; } = @"D:\Projects\VisualStudio\CadEditor\CadEditor\Configuration\Configuration.txt";

        public void LoadData(string[] lines, Type type)
        {
            foreach (var line in lines)
            {
                string[] data = line.Split(' ');
                FieldInfo field = type.GetField(data[0]);
                var value = TypeStringConverter.ToObject(data[1], field.FieldType);
                field.SetValue(type, value);
            }
        }

        public string SaveData(Type type)
        {
            string exportedString = type.Name + "\n";

            foreach (FieldInfo field in type.GetFields())
            {
                exportedString += field.Name + " " + 
                    TypeStringConverter.ToString(field.GetValue(type), field.GetValue(type).GetType()) + "\n";
            }
            exportedString += "End" + "\n";

            return exportedString;
        }
    }
}
