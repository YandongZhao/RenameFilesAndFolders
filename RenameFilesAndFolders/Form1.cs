using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 重命名文件和文件夹名
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog1.Description = "选择想要重命名的文件夹";
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                // No file is opened, bring up openFileDialog in selected path.
                this.textBox1.Text = folderBrowserDialog1.SelectedPath;
                openFileDialog1.InitialDirectory = folderBrowserDialog1.SelectedPath;
                openFileDialog1.FileName = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var rootDir = textBox1.Text;
            var enableFileName = checkBox1.Checked;
            var enableDirName = checkBox2.Checked;
            var enableContent = checkBox4.Checked;

            var matchString = textBox2.Text;
            var replaceString = textBox3.Text;

            var nest = checkBox3.Checked;

            ReplaceFilesAndDirs(rootDir, 
                enableFileName, 
                enableDirName, 
                enableContent, 
                matchString,
                replaceString,
                nest);
        }

        private string[] ignoreDirs = new[] { ".git", ".vs", "bin", "obj", "lib", "node_modules", "bower_components" };

        private void ReplaceFilesAndDirs(string rootDir, 
            bool enableFileName, 
            bool enableDirName, 
            bool enableContent,
            string matchString,
            string replaceString,
            bool nest)
        {
            // 先更改文件名及文件内容。
            if(enableFileName || enableContent)
            {
                var fileNames = Directory.GetFiles(rootDir);
                foreach (var fileName in fileNames)
                {
                    if (enableContent)
                    {
                        var content = File.ReadAllText(fileName);
                        if (content.Contains(matchString))
                        {
                            File.WriteAllText(fileName, content.Replace(matchString, replaceString), Encoding.UTF8);
                        }
                    }

                    if (enableFileName)
                    {
                        if (fileName.Contains(matchString))
                        {
                            File.Move(fileName, fileName.Replace(matchString, replaceString));
                        }
                    }
                }
            }

            var dirs = Directory.GetDirectories(rootDir);
            foreach(var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);
                if (ignoreDirs.Contains(dirInfo.Name)) continue;
                string newDir = dir;
                if (dir.Contains(matchString))
                {
                    newDir = dir.Replace(matchString, replaceString);
                    Directory.Move(dir, newDir);
                }
                if (nest)
                {
                    ReplaceFilesAndDirs(newDir,
                        enableFileName,
                        enableDirName,
                        enableContent,
                        matchString,
                        replaceString,
                        nest);
                }
            }
        }
    }
}
