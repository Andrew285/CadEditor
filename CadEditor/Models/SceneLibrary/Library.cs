using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CadEditor
{
    public class Library
    {
        private List<SaveData> saves;

        public Library() 
        {
            saves = new List<SaveData>();
        }

        public List<SaveData> GetAllSaves()
        {
            saves.Clear();

            string[] saveFiles = Directory.GetFiles(@"D:\Projects\VisualStudio\CadEditor\CadEditor\LibrarySaves\Scene");
            string[] screensFiles = Directory.GetFiles(@"D:\Projects\VisualStudio\CadEditor\CadEditor\LibrarySaves\Screenshots");

            if (saveFiles.Length != 0)
            {
                for (int i = 0; i < saveFiles.Length; i++)
                {
                    Bitmap bm = new Bitmap(screensFiles[i]);
                    string fileName = Path.GetFileName(saveFiles[i]);
                    string projectName = "No Title";
                    DateTime projectDate = new DateTime();
                    if (fileName.Contains(' '))
                    {
                        string[] fileNameParts = fileName.Split(' ');
                        projectName = fileNameParts[0];
                        string stringDate = fileNameParts[1].Replace("td", ":").Replace("d", ".").Replace("sp", " ");
                        string substringDate = stringDate.Substring(0, stringDate.Length - 4);
                        projectDate = DateTime.Parse(substringDate, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    SaveData saveData = new SaveData(bm, saveFiles[i], projectName, projectDate);
                    saves.Add(saveData);
                }
            }

            return saves;
        }

        public void AddSave(Bitmap picture, string path, string name, DateTime date)
        {
            saves.Add(new SaveData(picture, path, name, date));
        }

        public void Remove(SaveData data)
        {
            saves.Remove(data);
        }
    }

    public class SaveData
    {
        private Bitmap picture;
        private string filePath;
        private string title;
        private DateTime creationDate;

        public SaveData(Bitmap pic, string path, string name, DateTime date)
        {
            picture = pic;
            filePath = path;
            title = name;
            creationDate = date;
        }

        public Bitmap GetPicture()
        {
            return picture;
        }

        public string GetFilePath()
        {
            return filePath;
        }

        public string GetTitle()
        {
            return title;
        }

        public DateTime GetDate()
        {
            return creationDate;
        }
    }
}
