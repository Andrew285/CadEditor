using CadEditor.Settings;
using CadEditor.View.Forms;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CadEditor.Controllers
{
    public class SettingsController
    {
        private static SettingsController instance;
        private static List<MainSettings> settings;
        private ApplicationController _applicationController;
        //public ProjectSettingsForm ProjectSettingsForm;
        public LanguageSettings LanguageSettings { get; private set; }

        //public static SettingsController GetInstance()
        //{
        //    if (instance == null)
        //    {
        //        instance = new SettingsController();
        //    }

        //    return instance;
        //}

        public SettingsController(ApplicationController applicationController)
        {
            settings = new List<MainSettings>()
            {
                SceneSettings.GetInstance(),
                ThemeSettings.GetInstance(),
            };

            _applicationController = applicationController;
        }

        public void LoadData(string filePath)
        {
            //string[] lines = File.ReadAllLines(filePath);
            //List<string> currentData = new List<string>();
            //MainSettings currentSettings = null;

            //foreach (string line in lines)
            //{
            //    if (line.Contains("End"))
            //    {
            //        currentSettings.LoadData(currentData.ToArray(), currentSettings.GetType());
            //        currentSettings = null;
            //        currentData.Clear();
            //    }
            //    else if (line.Contains("Settings"))
            //    {
            //        foreach (MainSettings set in settings)
            //        {
            //            if (line.Contains(set.GetType().Name))
            //            {
            //                currentSettings = set;
            //                currentData = new List<string>();
            //                break;
            //            }
            //        }
            //    }
            //    else if (currentData != null)
            //    {
            //        currentData.Add(line);
            //    }
            //}

            //ProjectSettingsForm.LoadData();

            try
            {
                string json = File.ReadAllText("D:\\Projects\\VisualStudio\\CadEditor\\CadEditor\\Configuration\\language_settings.json");
                if (json == "")
                {
                    LanguageSettings = new LanguageSettings();
                }
                else
                {
                    LanguageSettings = JsonConvert.DeserializeObject<LanguageSettings>(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}");
                LanguageSettings = new LanguageSettings(); // Initialize with default settings
            }

        }

        public void SaveData(string filePath)
        {
            //string exportedString = "";

            //foreach (MainSettings set in settings)
            //{
            //    Type type = set.GetType();
            //    exportedString += set.SaveData(type);
            //}

            //using (StreamWriter writer = new StreamWriter(filePath))
            //{
            //    writer.WriteLine(exportedString);
            //    writer.Close();
            //}

            // Serialize to JSON and save to file
            string json = JsonConvert.SerializeObject(LanguageSettings, Formatting.Indented);
            File.WriteAllText("D:\\Projects\\VisualStudio\\CadEditor\\CadEditor\\Configuration\\language_settings.json", json);
            MessageBox.Show("Settings applied successfully.");
        }
    }
}
