using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace MediaLoader
{
    class SelectForm : Form
    {
        bool isSelected = false;
        ComboBox cb_List;
        Button b_Start, b_Cancel;

        public SelectForm()
        {
            //this.Text = "Выбор точки";
            this.ClientSize = new Size(380, 50);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;

            this.Controls.Add(b_Cancel = new Button());
            b_Cancel.Location = new Point(this.ClientSize.Width - 40, 10);
            b_Cancel.Size = new Size(30, 30);
            b_Cancel.Click += b_Cancel_Click;
            b_Cancel.Image = MediaLoader.Properties.Resources.Cancel;
            this.Controls.Add(b_Start = new Button());
            b_Start.Location = new Point(b_Cancel.Left - 40, 10);
            b_Start.Size = new Size(30, 30);
            b_Start.Click += b_Start_Click;
            b_Start.Image = MediaLoader.Properties.Resources.Start;
            this.Controls.Add(cb_List = new ComboBox());
            cb_List.Location = new Point(10, 15);
            cb_List.Size = new Size(b_Start.Left - 20, 30);

            foreach (string DirName in Directory.GetDirectories(Program.BlocksPath))
            {
                for (int i = 0; i < Program.Points.Length; i++)
                {
                    if (Path.GetFileName(DirName) == Program.Points[i])
                    {
                        cb_List.Items.Add(DirName);
                    }
                }
            }
        }

        void b_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
        void b_Start_Click(object sender, EventArgs e)
        {
            if (cb_List.SelectedIndex != -1)
            {
                this.DialogResult = DialogResult.Yes;
                Program.inPath = (string)cb_List.SelectedItem;
            }
            else
                this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
