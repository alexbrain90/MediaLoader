using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MediaLoader
{
    class MainForm:Form
    {
        Thread thread;

        ProgressBar pb_OneFile, pb_Total;
        Button b_Start, b_Cancel, b_Settings, b_Update;
        Label l_FileName, l_TimeFill;

        public MainForm()
        {
            this.Text = "Загрузка флешек";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(474, 84);
            this.Icon = MediaLoader.Properties.Resources.Icon;
            this.FormClosing += MainForm_FormClosing;

            this.Controls.Add(b_Start = new Button());
            b_Start.Location = new Point(10, 10);
            b_Start.Size = new Size(this.ClientSize.Width - 50, this.ClientSize.Height - 20);
            b_Start.Image = MediaLoader.Properties.Resources.Begin;
            b_Start.Font = new Font(this.Font.ToString(), 20f);
            b_Start.Click += b_Start_Click;

            this.Controls.Add(b_Update = new Button());
            b_Update.Size = new Size(30, 30);
            b_Update.Location = new Point(this.ClientSize.Width - b_Update.Width - 4, 10);
            b_Update.Image = MediaLoader.Properties.Resources.Update;
            b_Update.Click += b_Update_Click;
            this.Controls.Add(b_Settings = new Button());
            b_Settings.Location = new Point(b_Update.Left, b_Update.Bottom + 4);
            b_Settings.Size = b_Update.Size;
            b_Settings.Image = MediaLoader.Properties.Resources.Settings;
            b_Settings.Click += b_Settings_Click;

            this.Controls.Add(b_Cancel = new Button());
            b_Cancel.Size = new Size(64, 64);
            b_Cancel.Location = new Point(this.ClientSize.Width - b_Cancel.Width - 10, 10);
            b_Cancel.Image = MediaLoader.Properties.Resources.Cancel64;
            b_Cancel.Font = new Font(this.Font.ToString(), 12f);
            b_Cancel.Click += b_Cancel_Click;

            this.Controls.Add(l_FileName = new Label());
            l_FileName.Location = new Point(10, b_Cancel.Top);
            l_FileName.Size = new Size(b_Cancel.Left - 20, 12);
            l_FileName.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(pb_OneFile = new ProgressBar());
            pb_OneFile.Location = new Point(l_FileName.Left, l_FileName.Bottom + 2);
            pb_OneFile.Size = new Size(b_Cancel.Left - 20, 12);

            this.Controls.Add(l_TimeFill = new Label());
            l_TimeFill.Location = new Point(l_FileName.Left, pb_OneFile.Bottom + 6);
            l_TimeFill.Size = l_FileName.Size;
            l_TimeFill.Font = l_FileName.Font;
            l_TimeFill.TextAlign = l_FileName.TextAlign;
            this.Controls.Add(pb_Total = new ProgressBar());
            pb_Total.Location = new Point(l_TimeFill.Left, l_TimeFill.Bottom + 2);
            pb_Total.Size = new Size(b_Cancel.Left - 20, 16);
            pb_Total.Maximum = 36000;

            b_Start.Visible = false;
            SetVisible(false);
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            b_Cancel_Click(sender, e);
        }

        void b_Update_Click(object sender, EventArgs e)
        {
            b_Start.Visible = false;
            b_Update.Visible = false;

            new UpdateForm().ShowDialog();

            b_Start.Visible = true;
            b_Update.Visible = true;
        }
        void b_Settings_Click(object sender, EventArgs e)
        {
            new ConfigForm().ShowDialog();
        }

        void b_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                thread.Abort();
            }
            catch { }
            SetVisible(false);
        }

        void b_Start_Click(object sender, EventArgs e)
        {
            b_Start.Visible = false;
            b_Update.Visible = false;

            DialogResult dr = new SelectForm().ShowDialog();
            b_Start.Visible = true;
            b_Update.Visible = true;
            if (dr == DialogResult.No)
                return;

            string letters = "abcdefghijklmnopqrstuvwxyz";
            int j = 0;
            DriveInfo info;
            bool success = false;

            SetVisible(true);

            while (true)
            {
                success = false;
                for (int i = 0; i < Program.Points.Length; i++)
                {
                    if (Path.GetFileName(Program.inPath) == Program.Points[i])
                    {
                        for (j = 0; j < letters.Length; j++)
                        {
                            try
                            {
                                info = new DriveInfo(letters.Substring(j, 1));
                                if (info.VolumeLabel == Program.Flashs[i])
                                    break;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        if (j != letters.Length)
                        {
                            success = true;
                            Program.outPath = letters.Substring(j, 1).ToUpper() + ":\\";

                            info = new DriveInfo(letters.Substring(j, 1));
                            dr = MessageBox.Show("Выполнить копирование файлов\r\nиз папки: " + Program.inPath + "\r\nна диск: " + Program.outPath + " (" + info.VolumeLabel + ")", "Флешка найдена", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.No)
                                break;

                            this.Text = "Загрузка " + Path.GetFileName(Program.inPath);

                            foreach (string oneFile in Directory.GetFiles(Program.outPath, "*", SearchOption.AllDirectories))
                            {
                                try
                                {
                                    File.Delete(oneFile);
                                }
                                catch { }
                            }
                            foreach (string oneDir in Directory.GetDirectories(Program.outPath, "*", SearchOption.AllDirectories))
                            {
                                try
                                {
                                    Directory.Delete(oneDir, true);
                                }
                                catch { }
                            }

                            thread = new Thread(LoadFiles);
                            thread.Start();
                            while (thread.IsAlive == true)
                            {
                                Thread.Sleep(1);
                                Application.DoEvents();
                            }
                            this.Text = "Загрузка флешек";
                        }
                    }
                }
                if (success == false)
                {
                    dr = MessageBox.Show("Не удалось обнаружить флешку для точки " + Path.GetFileName(Program.inPath) + "\r\nПовторить поиск?", "Флешка не найдена", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                        continue;
                    else
                        break;
                }
                else
                    break;
            }

            SetVisible(false);
        }

        void SetVisible(bool V)
        {
            b_Start.Visible = !V;
            b_Cancel.Visible = V;

            l_FileName.Visible = V;
            l_TimeFill.Visible = V;

            pb_OneFile.Visible = V;
            pb_Total.Visible = V;

            b_Settings.Visible = false;
            b_Update.Visible = !V;
        }

        void LoadFiles()
        {
            int totalTime = 0, currentHour = 900;
            int number = 1, n = 0; string tmp = "";
            string[] files = Directory.GetFiles(Program.MediaPath, "*.*", SearchOption.AllDirectories);

            Random R = new Random();
            while (totalTime < 36000)
            {
                if (currentHour >= 900)
                {
                    l_FileName.Text = "Рекламный блок №" + number.ToString();
                    copyFile(Program.inPath + number.ToString() + ".avi", Program.outPath + secToStr(totalTime) + " - " + number.ToString() + ".avi");
                    currentHour = currentHour - 900 + getSeconds(Program.inPath + number.ToString() + ".avi");
                    totalTime += getSeconds(Program.inPath + number.ToString() + ".avi");
                    setTotalProgress(totalTime);
                    tmp = secToStr(totalTime);
                    l_TimeFill.Text = tmp.Substring(0, 2) + "ч " + tmp.Substring(2, 2) + "м " + tmp.Substring(4, 2) + "с";

                    number++;
                    if (number == 5)
                        number = 1;
                    continue;
                }

                while (true)
                {
                    n = R.Next(0, files.Length);
                    if (files[n] != "")
                        break;
                }
                tmp = files[n];
                files[n] = "";

                l_FileName.Text = Path.GetFileNameWithoutExtension(tmp);
                copyFile(tmp, Program.outPath + secToStr(totalTime) + " - " + Path.GetFileName(tmp));
                currentHour += getSeconds(tmp);
                totalTime += getSeconds(tmp);
                setTotalProgress(totalTime);
                tmp = secToStr(totalTime);
                l_TimeFill.Text = tmp.Substring(0, 2) + "ч " + tmp.Substring(2, 2) + "м " + tmp.Substring(4, 2) + "с";
            }
        }

        string secToStr(int seconds)
        {
            int s, m, h;
            h = seconds / 3600;
            seconds -= h * 3600;
            m = seconds / 60;
            seconds -= m * 60;
            s = seconds;
            h++;

            return h.ToString("00") + m.ToString("00") + s.ToString("00");
        }
        int getSeconds(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            long tmp = fs.Position;
            fs.Position = 48;
            byte[] data = new byte[4];
            fs.Read(data, 0, 4);
            fs.Position = tmp;
            fs.Close();

            return (((data[3] * 256 + data[2]) * 256 + data[1]) * 256 + data[0]) / 25;
        }
        bool copyFile(string inFile, string outFile)
        {
            pb_OneFile.Value = 0;
            FileStream infs = new FileStream(inFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), outfs = new FileStream(outFile, FileMode.Create);
            outfs.SetLength(0);
            byte[] buffer = new byte[1048576];
            int read;
            pb_OneFile.Maximum = (int)infs.Length;

            while(infs.Position < infs.Length)
            {
                read = infs.Read(buffer, 0, buffer.Length);
                outfs.Write(buffer, 0, read);
                pb_OneFile.Value += read;
            }

            infs.Close();
            outfs.Close();
            return true;
        }

        void setTotalProgress(int value)
        {
            if (value > pb_Total.Maximum)
                pb_Total.Maximum = value;
            pb_Total.Value = value;
        }
    }
}
