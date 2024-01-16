using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
                    SaveData saveData = new SaveData(bm, saveFiles[i], Path.GetFileName(saveFiles[i]));
                    saves.Add(saveData);
                }
            }

            return saves;
        }

        public void AddSave(Bitmap picture, string path, string name)
        {
            saves.Add(new SaveData(picture, path, name));
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

        public SaveData(Bitmap pic, string path, string name)
        {
            picture = pic;
            filePath = path;
            title = name;
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
    }
}
