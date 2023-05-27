using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor
{
    public static class Output
    {
        public static void ShowMessageBox(string title, string body)
        {
            MessageBox.Show(body, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
