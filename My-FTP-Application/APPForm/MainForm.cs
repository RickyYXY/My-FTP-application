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

namespace APPForm
{
    public partial class MainForm : Form
    {
        //private FtpHelper ftpHelper;

        private  DateTime clickTime;

        private bool isClicked = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        //初始化
        private void MainForm_Load(object sender, EventArgs e)
        {
            InitTreeView();
        }

        private void InitTreeView()
        {
            string[] drives = Environment.GetLogicalDrives();
            int i = 0;
            foreach (string drive in drives)
            {
                DriveInfo d = new DriveInfo(drive);
                if ((d.DriveType & DriveType.Fixed) == DriveType.Fixed)
                {
                    string drive1 = drive.Substring(0, drive.Length - 1);
                    this.treeLocal.Nodes[0].Nodes.Add(drive1);
                    this.treeLocal.Nodes[0].Nodes[i].ImageIndex = 1;
                    this.treeLocal.Nodes[0].Nodes[i].SelectedImageIndex = 1;
                    this.treeLocal.Nodes[0].Nodes[i].Tag = drive1;
                    this.treeLocal.Nodes[0].Nodes[i].Nodes.Add("");//增加一个空白节点，看起来是加号
                    i++;
                }
            }
        }

        //树菜单相关代码
        private void treeLocal_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level > 0)
            {
                //点击之前，先填充节点：
                string path = e.Node.FullPath.Substring(e.Node.FullPath.IndexOf("\\") + 1) + "\\";
                e.Node.Nodes.Clear();
                //string path = string.Format("{0}{1}",e.Node.Tag,this.treeLocal.PathSeparator);
                string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                int i = 0;
                foreach (string file in files)
                {
                    FileInfo f = new FileInfo(file);
                    if ((f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (f.Attributes & FileAttributes.System) != FileAttributes.System)
                    {
                        e.Node.Nodes.Add(Path.GetFileName(file));
                        e.Node.Nodes[i].ImageIndex = 3;
                        e.Node.Nodes[i].SelectedImageIndex = 3;
                        e.Node.Nodes[i].Tag = file;
                        i++;
                    }
                }
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
                foreach (string dir in dirs)
                {
                    DirectoryInfo d = new DirectoryInfo(dir);
                    if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (d.Attributes & FileAttributes.System) != FileAttributes.System)
                    {
                        e.Node.Nodes.Add(Path.GetFileName(dir));
                        e.Node.Nodes[i].ImageIndex = 2;
                        e.Node.Nodes[i].SelectedImageIndex = 2;
                        e.Node.Nodes[i].Tag = dir;
                        e.Node.Nodes[i].Nodes.Add("");//增加一个空白节点，看起来是加号
                        i++;
                    }
                }
            }
        }
        private void treeLocal_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level > 0 && e.Node.ImageIndex == 3)
            {
                this.menuLocal.Enabled = true;
                string path = e.Node.FullPath.Substring(e.Node.FullPath.IndexOf("\\") + 1);
                this.menuLoad.Tag = path;
            }
            else
            {
                this.menuLocal.Enabled = false;
            }
        }

        /* 下面是ftp相关代码*/
        /*-------------------------------------------*/
        //右键点击上传
        private void menuLoad_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //如果就目录,先设置上级目录，再列出目录
            //ftpHelper.SetPrePath();
            //this.ListDirectory();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (CheckInfo())
            //    {
            //        string ipAddr = this.txtAddress.Text.Trim();
            //        string port = this.txtPort.Text.Trim();
            //        string userName = this.txtUserName.Text.Trim();
            //        string password = this.txtPassword.Text.Trim();
            //        ftpHelper = new FtpHelper(ipAddr, port, userName, password);
            //        ListDirectory();
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }
        //private bool CheckInfo()
        //{
        //    //string ipAddr = this.txtAddress.Text.Trim();
        //    //string port = this.txtPort.Text.Trim();
        //    //string userName = this.txtUserName.Text.Trim();
        //    //string password = this.txtPassword.Text.Trim();
        //    //if (string.IsNullOrEmpty(ipAddr) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        //    //{
        //    //    MessageBox.Show("请输入登录信息");
        //    //    return false;
        //    //}
        //    //return true;
        //}
    }
}
