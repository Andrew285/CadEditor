using CadEditor.Controllers;
using CadEditor.Tools.Localization;
using System;
using System.Windows.Forms;

namespace CadEditor.Settings
{
    public class LanguageSettingsControl
    {
        private ApplicationController _applicationControlller;
        private TabPage _tabLanguage;
        private ComboBox comboBoxSelectLanguage;
        public event EventHandler LanguageChanged;

        public LanguageSettingsControl(ApplicationController applicationController) 
        {
            _applicationControlller = applicationController;
        }

        public void Initialize(TabPage tabLanguage)
        {
            _tabLanguage = tabLanguage;
            Label lblLanguage = new Label { Text = "Language:", Location = new System.Drawing.Point(20, 20) };
            comboBoxSelectLanguage = new ComboBox { Location = new System.Drawing.Point(120, 20) };
            comboBoxSelectLanguage.Items.AddRange(new string[] {"English", "Ukrainian"});
            comboBoxSelectLanguage.SelectedIndexChanged += language_SelectedIndexChanged;

            _tabLanguage.Controls.Add(lblLanguage);
            _tabLanguage.Controls.Add(comboBoxSelectLanguage);
        }

        private void language_SelectedIndexChanged(object sender, EventArgs e)
        {
            Languages language = Languages.ENGLISH;
            switch (comboBoxSelectLanguage.SelectedIndex)
            {
                case 0: language = Languages.ENGLISH; break;
                case 1: language = Languages.UKRAINIAN; break;
            }
            SetLanguage(language);
        }

        public void InvokeEvents()
        {
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public Languages GetSelectedLanguage()
        {
            return _applicationControlller.SettingsController.LanguageSettings.SeletectedLanguage;
        }

        public void SetLanguage(Languages language)
        {
            _applicationControlller.SettingsController.LanguageSettings.SeletectedLanguage = language;
            _applicationControlller.Localization.SetLanguage(_applicationControlller.SettingsController.LanguageSettings.SeletectedLanguage);
        }
    }
}
