using System;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace MediaLoader
{
    class UpdateForm : Form
    {
        Thread t_Work;
        Label l_Info;

        public UpdateForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(220, 90);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.ShowInTaskbar = false;

            this.Controls.Add(l_Info = new Label());
            l_Info.Location = new Point(0, 0);
            l_Info.Size = new Size(220, 60);
            l_Info.TextAlign = ContentAlignment.MiddleCenter;
            l_Info.Font = new Font(l_Info.Font.ToString(), 10f);

            ProgressBar p_Status;
            this.Controls.Add(p_Status = new ProgressBar());
            p_Status.Location = new Point(10, 60);
            p_Status.Size = new Size(200, 20);
            p_Status.Style = ProgressBarStyle.Marquee;

            t_Work = new Thread(AppUpdate);
            t_Work.Start();
        }

        private void AppUpdate()
        {
            l_Info.Text = "Проверка новой версии";
            if (UpgradeFunc() == true)
                this.DialogResult = DialogResult.Yes;
            else
                this.DialogResult = DialogResult.No;
            this.Close();
            return;
        }
        private bool UpgradeFunc()
        {
            string Text = getHttpPage("https://dl.dropboxusercontent.com/u/47371890/MediaLoader/Info.txt");
            int n = 0;
            for (int i = 0; i < Text.Length - 1; i++)
            {
                if (Text.Substring(i, 2) == "\r\n")
                {
                    n = i;
                    break;
                }
            }
            string Version = Text.Substring(0, n);
            if (Version.Length > 7)
                return false;
            string Info = Text.Substring(n + 2);

            if (Version != Application.ProductVersion)
            {
                l_Info.Text = "Загрузка файлов";
                if (File.Exists(Application.ExecutablePath + "_new.exe") == true)
                    File.Delete(Application.ExecutablePath + "_new.exe");
                getHttpPage("https://dl.dropboxusercontent.com/u/47371890/MediaLoader/App.exe", Application.ExecutablePath + "_new.exe");

                MakeUpgrade(null, null);
                return true;
            }
            return false;
        }
        private void MakeUpgrade(object sender, EventArgs e)
        {
            Process P = new Process();
            P.StartInfo.FileName = Application.ExecutablePath + "_new.exe";
            P.StartInfo.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            P.StartInfo.UseShellExecute = true;
            P.Start();
            Application.Exit();
        }

        private string getHttpPage(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                WebResponse res = req.GetResponse();
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        private bool getHttpPage(string url, string fileName)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(url, fileName);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
