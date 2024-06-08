using System.Windows.Forms;

namespace CadEditor
{
    public static class Output
    {
        public static void ShowMessageBox(string title, string body)
        {
            MessageBox.Show(body, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult ShowError(string title, string body)
        {
            return MessageBox.Show(body, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }
    }
}
