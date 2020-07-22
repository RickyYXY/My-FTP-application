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
using FTPUtils;

namespace APPForm
{
    public partial class MainForm : Form
    {
        //private ftpClient ftpClient;

        private  DateTime clickTime;

        private bool isClicked = false;

        private FTPClient ftpClient;

        public MainForm()
        {
            InitializeComponent();
            //密码显示为"*"
            TextBox tb = toolStripTextBoxPassword.Control as TextBox;
            tb.PasswordChar = '*';
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
            //ftpClient.SetPrePath();
            //this.ListDirectory();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckUserInfo())
                {
                    string ipAddr = this.toolStripTextBoxIpAddr.Text.Trim();
                    string port = this.toolStripTextBoxPort.Text.Trim();
                    string userName = this.toolStripTextBoxName.Text.Trim();
                    string password = this.toolStripTextBoxPassword.Text.Trim();
                    ftpClient = new FTPClient(ipAddr, port, userName, password);
                    ShowFilesDirectory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 检查用户登录信息
        /// </summary>
        /// <returns></returns>
        private bool CheckUserInfo()
        {
            string ipaddr = this.toolStripTextBoxIpAddr.Text.Trim();
            string port = this.toolStripTextBoxPort.Text.Trim();
            string username = this.toolStripTextBoxName.Text.Trim();
            string password = this.toolStripTextBoxPassword.Text.Trim();
            if (string.IsNullOrEmpty(ipaddr) || string.IsNullOrEmpty(port)
                || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("请输入登录信息");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 服务器端文件图标鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTmp_MouseDown(object sender, MouseEventArgs e)
        {
            //如果是右键按下
            if (e.Button == MouseButtons.Right) //右键按下显示菜单
            {
                Button btnTmp = (Button)sender;
                string name = btnTmp.Text;
                menuSaveAs.Tag = name;
                menuDelete.Tag = name;
            }
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTmp_DoubleClick(object sender, EventArgs e)
        {
            Button btnTmp = (Button)sender;
            FtpContentType contentType;
            if (Enum.TryParse(btnTmp.Tag.ToString(), out contentType))
            {
                switch (contentType)
                {
                    case FtpContentType.folder:
                        //如果是目录，则重新设置当前的相对路径
                        ftpClient.SetRelatePath(btnTmp.Text);
                        ShowFilesDirectory();//再次列出目录
                        break;
                    default:
                        MessageBox.Show("非目录，请点击右键", "提示", MessageBoxButtons.OK);
                        break;
                }
            }
        }

        /// <summary>
        /// 用于模拟双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTmp_Click(object sender, EventArgs e)
        {
            if (isClicked)
            {
                TimeSpan span = DateTime.Now - clickTime;
                if (span.Milliseconds < SystemInformation.DoubleClickTime)
                {
                    btnTmp_DoubleClick(sender, e);
                    isClicked = false;
                }
            }
            else
            {
                isClicked = true;
                clickTime = DateTime.Now;
            }
        }

        #region FTP功能部分
        /// <summary>
        /// 在对应板块展示服务器端文件的内容
        /// </summary>
        private void ShowFilesDirectory()
        {
            bool isOk = false;
            string[] arrAccept = ftpClient.GetFilesDirectory(out isOk);//调用Ftp目录显示功能
            if (isOk)
            {
                FTPflowLayoutPanel.Controls.Clear();
                foreach (string accept in arrAccept)
                {
                    string name = accept.Substring(39);

                    //创建一个临时控件用于显示ftp服务器端的文件
                    Button btnTmp = new Button()
                    {
                        BackColor = Color.White,
                        TextImageRelation = TextImageRelation.ImageAboveText,
                        Text = name,
                        Width = 80,
                        Height = 80
                    };

                    btnTmp.FlatAppearance.BorderSize = 0;
                    btnTmp.FlatStyle = FlatStyle.Flat;

                    if (accept.IndexOf("<DIR>") != -1)
                    {
                        btnTmp.Image = global::APPForm.Properties.Resources.folder.ToBitmap();
                        btnTmp.Tag = FtpContentType.folder;
                    }
                    else
                    {
                        btnTmp.Image = global::APPForm.Properties.Resources.file.ToBitmap();
                        btnTmp.Tag = FtpContentType.file;
                        btnTmp.ContextMenuStrip = menuRightKey;
                        btnTmp.MouseDown += new MouseEventHandler(BtnTmp_MouseDown);
                    }

                    //btnTmp.DoubleClick += new EventHandler(btnTmp_DoubleClick);
                    btnTmp.Click += new EventHandler(btnTmp_Click);//Button中不支持双击事件，所以用单击事件模拟双击
                    FTPflowLayoutPanel.Controls.Add(btnTmp);
                }
                lblMsg.Text = "服务器端文件载入成功";
            }
            else
            {
                ftpClient.SetPrePath();
                lblMsg.Text = "链接失败，或者没有数据";
            }
        }
        #endregion

        //返回上级文件夹
        private void toolStripButtonReturn_Click(object sender, EventArgs e)
        {
            ftpClient.SetPrePath();
            ShowFilesDirectory();
        }
    }
}
