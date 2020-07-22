using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;

namespace FTPUtils
{
    public enum FtpContentType  //文件类型，0代表未定义类型，1代表文件，2代表文件夹
    {
        undefined = 0,
        file = 1,
        folder = 2
    }

    public class FTPClient
    {
        #region 属性
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddr { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelatePath { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        #endregion

        #region 构造函数
        public FTPClient() { }
        public FTPClient(string ipAddr, string port, string userName, string password)
        {
            IpAddr = ipAddr;
            Port = port;
            UserName = userName;
            Password = password;
        }
        #endregion

        /// <summary>
        /// 获取服务器端的文件信息
        /// </summary>
        /// <param name="isOk"></param>
        /// <returns></returns>
        public string[] GetFilesDirectory(out bool isOk)
        {
            string method = WebRequestMethods.Ftp.ListDirectoryDetails;
            var statusCode = FtpStatusCode.DataAlreadyOpen;
            FtpWebResponse response = CallFTP(method);
            return ReadByLine(response, statusCode, out isOk);
        }

        /// <summary>
        /// 设置上级目录
        /// </summary>
        public void SetPrePath()
        {
            string relatePath = RelatePath;
            if (string.IsNullOrEmpty(relatePath) || relatePath.LastIndexOf("/") == 0)
            {
                relatePath = "";
            }
            else
            {
                relatePath = relatePath.Substring(0, relatePath.LastIndexOf("/"));
            }
            RelatePath = relatePath;
        }

        /// <summary>
        /// 设置相对路径
        /// </summary>
        /// <param name="folderName"></param>
        public void SetRelatePath(string folderName)
        {
            RelatePath = string.Format("{0}/{1}", RelatePath, folderName);
        }

        /// <summary>
        /// 删除服务器上的文件
        /// </summary>
        /// <param name="isOK"></param>
        public void DeleteFile(out bool isOK)
        {
            string method = WebRequestMethods.Ftp.DeleteFile;
            var statusCode = FtpStatusCode.FileActionOK;
            FtpWebResponse response = CallFTP(method);
            if (statusCode == response.StatusCode)
                isOK = true;
            else
                isOK = false;
            response.Close();
        }

        /// <summary>
        /// 请求FTP服务
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private FtpWebResponse CallFTP(string method)
        {
            //设置uri
            string uri = string.Format("ftp://{0}:{1}{2}", IpAddr, Port, RelatePath);
            //创建请求
            FtpWebRequest request;
            request = (FtpWebRequest)FtpWebRequest.Create(uri);
            //设置请求参数
            request.UseBinary = true;
            request.UsePassive = true;
            request.Credentials = new NetworkCredential(UserName, Password);
            request.KeepAlive = false;
            request.Method = method;
            //等待回复
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            return response;
        }

        /// <summary>
        /// 按行读取
        /// </summary>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <param name="isOk"></param>
        /// <returns></returns>
        private string[] ReadByLine(FtpWebResponse response, FtpStatusCode statusCode, out bool isOk)
        {
            List<string> lstAccpet = new List<string>();
            int clock = 0;
            isOk = false;
            while (clock <= 5)
            {
                if (response.StatusCode == statusCode)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string line = sr.ReadLine();
                        while (!string.IsNullOrEmpty(line))
                        {
                            lstAccpet.Add(line);
                            line = sr.ReadLine();
                        }
                    }
                    isOk = true;
                    break;
                }
                clock++;
                Thread.Sleep(200);
            }
            response.Close();
            return lstAccpet.ToArray();
        }

        
    }
}
