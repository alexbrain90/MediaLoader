using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace MediaLoader
{
    class ConfigForm : Form
    {
        public ConfigForm()
        {
            this.Text = "Настройки";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(400, 600);
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.Icon = MediaLoader.Properties.Resources.Icon;
        }
    }
}
