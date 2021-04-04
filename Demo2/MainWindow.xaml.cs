using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace Demo2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string RufusUrl = "https://github.com/pbatard/rufus/releases/download/v3.13/rufus-3.13.exe";
        public string stuISOfileUrl, stumd5fileUrl;
        public string lecISOfileUrl, lecmd5fileUrl;
        public string whereWeAre = System.IO.Directory.GetCurrentDirectory();
        public string workfolder = System.IO.Directory.GetCurrentDirectory() + @"\data";
        public string downloadFileName = System.IO.Directory.GetCurrentDirectory() + @"\data\tmp.iso";

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        public void schoolList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (schoolList.SelectedIndex)
            {
                case 0:
                    stuISOfileUrl = "https://github.com/fishheadsam/office-in-your-pocket/releases/download/v0.1-alpha/OIP-20.04.2-v0.1.0-desktop-amd64.iso";
                    lecISOfileUrl = "https://github.com/fishheadsam/office-in-your-pocket/releases/download/v0.1-alpha/OIP-20.04.2-v0.1.0-desktop-amd64.iso";
                    break;

                case 1:
                    stuISOfileUrl = "https://github.com/fishheadsam/office-in-your-pocket/releases/download/v0.1-alpha/OIP-20.04.2-v0.1.0-desktop.iso";
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    break;

                case 5:
                    break;

                case 6:
                    break;

                case 7:
                    break;
            }
        }

        /// <summary>
        /// Response Student Role button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Download_Student_ISO(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(workfolder))
            {
                Directory.CreateDirectory(workfolder);
            }
            else
            {
                Directory.Delete(workfolder, true);
                Directory.CreateDirectory(workfolder);
            }
            DownloadFunction(RufusUrl, whereWeAre + @"\data\rufus.exe");
            DownloadFunction(stumd5fileUrl, workfolder + @"\tmp.md5");
            if (HttpFileExist(stuISOfileUrl))
            {
                DownloadFunction(stuISOfileUrl, downloadFileName);
            }
            else
            {
                MessageBox.Show("Hello");
            }
        }

        // Resoponse Lecture Role button 响应教师角色按钮
        private void Download_Lecture_ISO(object sender, RoutedEventArgs e)
        {
            DownloadFunction(RufusUrl, workfolder + @"\rufus.exe");
            DownloadFunction(lecmd5fileUrl, workfolder + @"\tmp.md5");
            if (HttpFileExist(lecISOfileUrl))
            {
                DownloadFunction(lecISOfileUrl, downloadFileName);
            }
            else
            {
                MessageBox.Show("Hello");
            }
        }

        /// <summary>
        /// Implement Download Function
        /// </summary>
        /// <param name="http_url">host target file path</param>
        /// <param name="save_url">local file save path</param>
        public void DownloadFunction(String http_url, String save_url)
        {
            if (HttpFileExist(http_url))
            {
                using WebClient wc = new WebClient();
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri(http_url), save_url);
            }
            else
            {
                MessageBox.Show("Download Failed, please try again or send me a email.");
            }
        }

        /// <summary>
        /// Update Progress Bar function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            status.Content = (e.ProgressPercentage / progressBar.Maximum) * 100 + "%";
        }

        /// <summary>
        /// Check host file exist
        /// </summary>
        /// <param name="http_file_url">host target file url</param>
        /// <returns></returns>
        private bool HttpFileExist(string http_file_url)
        {
            WebResponse response = null;
            bool result = false;
            try
            {
                response = WebRequest.Create(http_file_url).GetResponse();
                result = response != null;
            }
            catch (Exception e)
            {
                result = false;
                throw new Exception("Remote file not exist" + e.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Invoke Format and Burning
        /// </summary>
        /// <param name="runFilePath">Rufus path and downloaded ISO file path</param>
        /// <param name="args">using command input ISO file</param>
        /// <returns></returns>
        public bool StartProcess(string runFilePath, params string[] args)
        {
            string s = "";
            foreach (string arg in args)
            {
                s = s + arg + " ";
            }
            s = s.Trim();
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(runFilePath, s);
            process.StartInfo = startInfo;
            process.Start();
            return true;
        }

        /// <summary>
        /// Get ISO file MD5 value
        /// </summary>
        /// <param name="fileName">local file save path</param>
        /// <returns>MD5 value</returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + e.Message);
            }
        }

        /// <summary>
        /// Start burning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            string hashvalue = System.IO.File.ReadAllText(workfolder + @"\tmp.md5");
            if (progressBar.Value != 100)
            {
                MessageBox.Show("Please wait download finish!");
            }
            else
            {
                if (hashvalue == GetMD5HashFromFile(downloadFileName))
                {
                    MessageBox.Show("Please assign at least 5GB space in Persistent partition size.");
                    string rufus_path = System.IO.Directory.GetCurrentDirectory() + @"\data\rufus.exe";
                    string[] the_args = { "-i", downloadFileName };
                    StartProcess(rufus_path, the_args);
                }
                else
                {
                    MessageBox.Show("File integrity verification error! Please downloadit agian or contact me.");
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


        /// <summary>
        /// Exit application and clean up everything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(workfolder))
            {
                Directory.Delete(workfolder, true);
                System.Environment.Exit(0);
            }
            else
            {
                System.Environment.Exit(0);
            }
        }
    }
}