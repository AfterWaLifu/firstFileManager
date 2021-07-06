using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace MFileManager
{
    public partial class Form1 : Form
    {
        #region Поля
        FormFileCreation ffc = new FormFileCreation();
        FormDirectoryCreation fdc = new FormDirectoryCreation();
        FormRename fr = new FormRename();
        string Path1;
        string Path2;
        string[] buffer = new string[1] { ""};
        string toMoveAdress = "";
        Random r = new Random();
        #endregion

        public Form1()
        {
            InitializeComponent();
            toolTip1.SetToolTip(comboBox1, "Выбрать логический диск для левого окна");
            toolTip1.SetToolTip(comboBox2, "Выбрать логический диск для правого окна");
            toolTip1.SetToolTip(textBox1, "Начните вводить для поиска по названию");
            toolTip1.SetToolTip(textBox2, "Начните вводить для поиска по названию");
            toolTip1.SetToolTip(textBox3, "Текущаий адресс левого окна");
            toolTip1.SetToolTip(textBox4, "Текущаий адресс правого окна");
            listView1.ContextMenuStrip = contextMenuStrip1;
            listView2.ContextMenuStrip = contextMenuStrip1;
            fillingDiskLists();
            this.ActiveControl = listView1;
        }

        #region Элементы формы

        #region Поиск
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            search(true, textBox1.Text);
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            search(false, textBox2.Text);
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                fillingFilesAndDirectories(textBox3.Text,true);
            }
            catch { }
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                fillingFilesAndDirectories(textBox4.Text, false);
            }
            catch { }
        }
        #endregion

        #region Панель
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (ActiveControl == listView1)
            {
                deleting(true);
            }
            else if (ActiveControl == listView2)
            {
                deleting(false);
            }
            fillingFilesAndDirectories(Path1, true);
            fillingFilesAndDirectories(Path2, false);
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (ActiveControl == listView1)
            {
                copyingToBuffer(true);
            }
            else if (ActiveControl == listView2)
            {
                copyingToBuffer(false);
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (ActiveControl == listView1)
            {
                pastingFromBuffer(true);
            }
            else if (ActiveControl == listView2)
            {
                pastingFromBuffer(false);
            }
            fillingFilesAndDirectories(Path1, true);
            fillingFilesAndDirectories(Path2, false);
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ffc.ShowDialog();
            string nameFile = ffc.textBox1.Text;
            string extantionFile = ffc.textBox2.Text;
            ffc.textBox1.Clear();
            ffc.textBox2.Clear();
            if (ActiveControl == listView1)
            {
                creatingFile(true, nameFile, extantionFile);
            }
            else if (ActiveControl == listView2)
            {
                creatingFile(false, nameFile, extantionFile);
            }
            fillingFilesAndDirectories(Path1, true);
            fillingFilesAndDirectories(Path2, false);
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            fdc.ShowDialog();
            string nameDir = fdc.textBox1.Text;
            fdc.textBox1.Clear();
            if (ActiveControl == listView1)
            {
                nameDir = nameDirFinisher(true, nameDir);
                Directory.CreateDirectory($"{Path1}{nameDir}");
            }
            else if (ActiveControl == listView2)
            {
                nameDir = nameDirFinisher(false, nameDir);
                Directory.CreateDirectory($"{Path2}{nameDir}");
            }
            fillingFilesAndDirectories(Path1, true);
            fillingFilesAndDirectories(Path2, false);
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (ActiveControl == listView1)
            {
                rename(true);
            }
            else if (ActiveControl == listView2)
            {
                rename(false);
            }
            fillingFilesAndDirectories(Path1, true);
            fillingFilesAndDirectories(Path2, false);
        }
        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fillingFilesAndDirectories(Path1, true);
            fillingFilesAndDirectories(Path2, false);
        }
        #endregion

        #region Выбор диска
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = comboBox1.SelectedItem.ToString();
            path = path.Substring(0,3);
            fillingFilesAndDirectories(path,true);
            textBox3.Text = Path1;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = comboBox2.SelectedItem.ToString();
            path = path.Substring(0, 3);
            fillingFilesAndDirectories(path, false);
            textBox4.Text = Path2;
        }
        #endregion

        #region Переходы по папкам
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string testForExtantion = listView1.SelectedItems[0].SubItems[2].Text;
            if (string.IsNullOrEmpty(testForExtantion))
            {
                openFolder(true);
            }
            else
            {
                string temp = listView1.SelectedItems[0].SubItems[1].Text;
                if (File.Exists(Path1 + temp))
                {
                    try
                    {
                        Process.Start(Path1 + temp);
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show("Не удаётся открыть файл, обновите список файлов и проверьте наличие " +
                        "программы по умолчанию.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string testForExtantion = listView2.SelectedItems[0].SubItems[2].Text;
            if (string.IsNullOrEmpty(testForExtantion))
            {
                openFolder(false);
            }
            else
            {
                string temp = listView2.SelectedItems[0].SubItems[1].Text;
                try
                {
                    Process.Start(Path2 + temp);
                }
                catch
                {
                    MessageBox.Show("Ошибка при переходе, нажмите \"Обновить\" или продолжите без обновления",
                   "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

        #endregion

        #region Методы бэкенда
        private void fillingDiskLists()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                comboBox1.Items.Add(drive);
                comboBox2.Items.Add(drive);
                comboBox1.Items[comboBox1.Items.Count - 1] += $" {drive.VolumeLabel} " +
                    $"{(drive.TotalSize-drive.TotalFreeSpace)/1073741824 }/{drive.TotalSize/ 1073741824} ГБайт";
                comboBox2.Items[comboBox2.Items.Count - 1] += $" {drive.VolumeLabel} " +
                    $"{(drive.TotalSize - drive.TotalFreeSpace) / 1073741824 }/{drive.TotalSize / 1073741824} ГБайт";
            }
            comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 1) comboBox2.SelectedIndex = 1;
        }
        private void fillingFilesAndDirectories(string path, bool box)
        {
            DirectoryInfo mainDirectory = new DirectoryInfo(path);
            DirectoryInfo[] directories = mainDirectory.GetDirectories();
            FileInfo[] files = mainDirectory.GetFiles();
            if (box)
            {
                Path1 = path;
                listView1.Items.Clear();
                folderBackCreation(true);
                for (int i = 0; i<directories.Length; i++)
                {
                    ListViewItem item = new ListViewItem(new string[] {
                    "", directories[i].Name, ""});
                    item.ImageIndex = 0;
                    listView1.Items.Add(item);
                }
                for (int i = 0; i < files.Length; i++)
                {
                    ListViewItem item = new ListViewItem(new string[] {
                    "", files[i].Name, files[i].Extension});
                    item.ImageIndex = 1;
                    listView1.Items.Add(item);
                }
            }
            else
            {
                Path2 = path;
                listView2.Items.Clear();
                folderBackCreation(false);
                for (int i = 0; i < directories.Length; i++)
                {
                    ListViewItem item = new ListViewItem(new string[] {
                    "", directories[i].Name, ""});
                    item.ImageIndex = 0;
                    listView2.Items.Add(item);
                }
                for (int i = 0; i < files.Length; i++)
                {
                    ListViewItem item = new ListViewItem(new string[] {
                    "", files[i].Name, files[i].Extension});
                    item.ImageIndex = 1;
                    listView2.Items.Add(item);
                }
            }
        }
        private void folderBackCreation(bool box)
        {
            ListViewItem backItem = new ListViewItem(new string[]
                {"", "На уровень вверх",""});
            backItem.ImageIndex = 2;
            if (box)
            {
                if (Path1.Length < 4) return;
                listView1.Items.Add(backItem);
            }
            else
            {
                if (Path2.Length < 4) return;
                listView2.Items.Add(backItem);
            }
        }
        private void search(bool box, string word)
        {
            DirectoryInfo directory;
            if (box)
            {
                directory = new DirectoryInfo(Path1);
            }
            else
            {
                directory = new DirectoryInfo(Path2);
            }
            DirectoryInfo[] directories = directory.GetDirectories();
            FileInfo[] files = directory.GetFiles();
            if (box)
            {
                listView1.Items.Clear();
                folderBackCreation(box);
                foreach(DirectoryInfo dir in directories)
                {
                    if (dir.Name.IndexOf(word) >= 0)
                    {
                        ListViewItem item = new ListViewItem(new string[] {
                    "", dir.Name, ""});
                        item.ImageIndex = 0;
                        listView1.Items.Add(item);
                    }
                }
                foreach (FileInfo file in files)
                {
                    if (file.Name.IndexOf(word) >= 0)
                    {
                        ListViewItem item = new ListViewItem(new string[] {
                    "", file.Name, file.Extension});
                        item.ImageIndex = 1;
                        listView1.Items.Add(item);
                    }
                }
            }
            else
            {
                listView2.Items.Clear();
                folderBackCreation(box);
                foreach (DirectoryInfo dir in directories)
                {
                    if (dir.Name.IndexOf(word) >= 0)
                    {
                        ListViewItem item = new ListViewItem(new string[] {
                    "", dir.Name, ""});
                        item.ImageIndex = 0;
                        listView2.Items.Add(item);
                    }
                }
                foreach (FileInfo file in files)
                {
                    if (file.Name.IndexOf(word) >= 0)
                    {
                        ListViewItem item = new ListViewItem(new string[] {
                    "", file.Name, file.Extension});
                        item.ImageIndex = 1;
                        listView2.Items.Add(item);
                    }
                }
            }
        }
        private void openFolder(bool box)
        {
            try
            {
                if (box)
                {
                    string element = listView1.SelectedItems[0].SubItems[1].Text;
                    if (element == "На уровень вверх")
                    {
                        Path1 = Path1.Substring(0, Path1.LastIndexOf(@"\"));
                        Path1 = Path1.Substring(0, Path1.LastIndexOf(@"\") + 1);
                    }
                    else Path1 += element + @"\";
                    fillingFilesAndDirectories(Path1, true);
                    textBox3.Text = Path1;
                }
                else
                {
                    string element = listView2.SelectedItems[0].SubItems[1].Text;
                    if (element == "На уровень вверх")
                    {
                        Path2 = Path2.Substring(0, Path2.LastIndexOf(@"\"));
                        Path2 = Path2.Substring(0, Path2.LastIndexOf(@"\") + 1);
                    }
                    else Path2 += element + @"\";
                    fillingFilesAndDirectories(Path2, false);
                    textBox4.Text = Path2;
                }
            }
            catch
            {
                if (box && Path1.Length > 3)
                {
                    Path1 = Path1.Substring(0, Path1.LastIndexOf(@"\"));
                    Path1 = Path1.Substring(0, Path1.LastIndexOf(@"\") + 1);
                }
                else if (!box && Path2.Length > 3)
                {
                    Path2 = Path2.Substring(0, Path2.LastIndexOf(@"\"));
                    Path2 = Path2.Substring(0, Path2.LastIndexOf(@"\") + 1);
                }
                MessageBox.Show("Ошибка при переходе, нажмите \"Обновить\" или продолжите без обновления",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void deleting(bool box)
        {
            if (box)
            {
                string[] names = new string[listView1.SelectedItems.Count];
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    names[i] = listView1.SelectedItems[i].SubItems[1].Text;
                }
                foreach (string s in names)
                {
                    try
                    {
                        File.Delete(Path1 + s);
                    }
                    catch
                    {
                        Directory.Delete(Path1 + s,true);
                    }
                    finally { }
                }
            }
            else
            {
                string[] names = new string[listView2.SelectedItems.Count];
                for (int i = 0; i < listView2.SelectedItems.Count; i++)
                {
                    names[i] = listView2.SelectedItems[i].SubItems[1].Text;
                }
                foreach (string s in names)
                {
                    try
                    {
                        File.Delete(Path2 + s);
                    }
                    catch
                    {
                        Directory.Delete(Path2 + s, true);
                    }
                    finally { }
                }
            }
        }
        private void creatingFile(bool box, string name, string extantion)
        {
            string fullName = nameFileFinisher(box, name, extantion);
            if (box)
            {
                FileStream fs = new FileStream($"{Path1}{fullName}", FileMode.Create);
                fs.Close();
                Process.Start($"{Path1}{fullName}");
            }
            else
            {
                FileStream fs = new FileStream($"{Path2}{fullName}", FileMode.Create);
                fs.Close();
                Process.Start($"{Path2}{fullName}");
            }
        }
        private string nameFileFinisher(bool box, string name, string extantion)
        {
            bool ifExist = false;
            if (string.IsNullOrEmpty(extantion))
            {
                extantion = "txt";
            }
            if (string.IsNullOrEmpty(name))
            {
                name = r.Next(0,10000).ToString();
            }
            if (box && File.Exists($"{Path1}{name}.{extantion}"))
            {
                ifExist = true;
            }
            else if (!box && File.Exists($"{Path2}{name}.{extantion}"))
            {
                ifExist = true;
            }
            if (ifExist)
            {
                name += " №" + r.Next(0, 10000).ToString();
            }

            return $"{name}.{extantion}";
        }
        private string nameDirFinisher(bool box, string name)
        {
            if ((box && Directory.Exists($"{Path1}{name}")) ||
                (!box && Directory.Exists($"{Path2}{name}")))
            {
                name += " №" + r.Next(0,1000).ToString();
            }
            return name;
        }
        private void copyingToBuffer(bool box)
        {
            if (box)
            {
                buffer = new string[listView1.SelectedItems.Count];
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    buffer[i] = listView1.SelectedItems[i].SubItems[1].Text;
                }
                toMoveAdress = Path1;
            }
            else
            {
                buffer = new string[listView2.SelectedItems.Count];
                for (int i = 0; i < listView2.SelectedItems.Count; i++)
                {
                    buffer[i] = listView2.SelectedItems[i].SubItems[1].Text;
                }
                toMoveAdress = Path2;
            }
        }
        private void pastingFromBuffer(bool box)
        {
            if (toMoveAdress == "")
            {
                MessageBox.Show("Вы ничего не поместили в буфер",
                "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (box)
            {
                foreach (string s in buffer)
                {
                    try
                    {
                        File.Move(toMoveAdress+s, Path1+s);
                    }
                    catch
                    {
                        Directory.Move(toMoveAdress + s, Path1 + s);
                    }
                }
            }
            else
            {
                foreach (string s in buffer)
                {
                    try
                    {
                        File.Move(toMoveAdress + s, Path2 + s);
                    }
                    catch
                    {
                        Directory.Move(toMoveAdress + s, Path2 + s);
                    }
                }
            }
            toMoveAdress = "";
            buffer = new string[1];
        }
        private void rename(bool box)
        {
            if (box)
            {
                string sourceName = listView1.SelectedItems[0].SubItems[1].Text;
                fr.textBox1.Text = sourceName;
                fr.ShowDialog();
                string name = fr.textBox1.Text;
                if (name == sourceName) return;
                if (listView1.SelectedItems[0].SubItems[2].Text == "")
                {
                    Directory.Move(Path1 + sourceName, Path1 + name);
                }
                else
                {
                    File.Move(Path1 + sourceName, Path1 + name);
                }
            }
            else
            {
                string sourceName = listView2.SelectedItems[0].SubItems[1].Text;
                fr.textBox1.Text = sourceName;
                fr.ShowDialog();
                string name = fr.textBox1.Text;
                if (name == sourceName) return;
                if (listView2.SelectedItems[0].SubItems[2].Text == "")
                {
                    Directory.Move(Path2 + sourceName, Path2 + name);
                }
                else
                {
                    File.Move(Path2 + sourceName, Path2 + name);
                }
            }
        }
        #endregion
    }
}