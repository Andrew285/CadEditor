using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CadEditor.Tools.Localization
{
    public class Localization
    {
        private Dictionary<string, Dictionary<string, string>> translations;
        private string currentLanguage;

        public Localization()
        {
            translations = new Dictionary<string, Dictionary<string, string>>();
            currentLanguage = "uk";
        }

        public void LoadTranslations()
        {
            try
            {
                string json = File.ReadAllText("D:\\Projects\\VisualStudio\\CadEditor\\CadEditor\\Tools\\Localization\\translations.json");
                translations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading translations: {ex.Message}");
            }
        }

        public void SaveTranslations()
        {

        }

        public string GetTranslationOf(string key)
        {
            if (translations != null && translations.ContainsKey(currentLanguage) && translations[currentLanguage].ContainsKey(key))
            {
                return translations[currentLanguage][key];
            }
            return key; // Return the key itself if translation is not found
        }

        public void SetLanguage(Languages language)
        {
            switch (language)
            {
                case Languages.ENGLISH: currentLanguage = "en"; break;
                case Languages.UKRAINIAN: currentLanguage = "uk"; break;
            }
        }
    }

    public enum Languages
    {
        ENGLISH,
        UKRAINIAN
    }
}
