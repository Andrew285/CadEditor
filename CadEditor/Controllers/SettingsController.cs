using CadEditor.Settings;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;

namespace CadEditor.Controllers
{
    public class SettingsController
    {
        private static SettingsController instance;
        private static List<MainSettings> settings;

        public static SettingsController GetInstance()
        {
            if (instance == null)
            {
                instance = new SettingsController();
            }

            return instance;
        }

        public SettingsController()
        {
            settings = new List<MainSettings>()
            {
                SceneSettings.GetInstance(),
                ThemeSettings.GetInstance(),
            };
        }

        public void LoadData(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            List<string> currentData = new List<string>();
            MainSettings currentSettings = null;

            foreach (string line in lines)
            {
                if (line.Contains("End"))
                {
                    currentSettings.LoadData(currentData.ToArray(), currentSettings.GetType());
                    currentSettings = null;
                    currentData.Clear();
                }
                else if (line.Contains("Settings"))
                {
                    foreach (MainSettings set in settings)
                    {
                        if (line.Contains(set.GetType().Name))
                        {
                            currentSettings = set;
                            currentData = new List<string>();
                            break;
                        }
                    }
                }
                else if (currentData != null)
                {
                    currentData.Add(line);
                }
            }
        }

        public void SaveData(string filePath)
        {
            string exportedString = "";

            foreach (MainSettings set in settings)
            {
                Type type = set.GetType();
                exportedString += set.SaveData(type);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(exportedString);
                writer.Close();
            }
        }
    }
}
