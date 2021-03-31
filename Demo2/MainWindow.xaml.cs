using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System;

namespace Demo2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly string ISOfileUrl = "https://github.com/fishheadsam/office-in-your-pocket/releases/download/v0.1-alpha/OIP-20.04.2-v0.1.0-desktop-amd64.iso";

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }


        // Response Student Role button 响应学生角色按钮
        private void Download_Student_ISO(object sender, RoutedEventArgs e)
        {
            string whereWeAre = System.IO.Directory.GetCurrentDirectory();
            string downloadFileName = whereWeAre + @"\tmp.iso";

            firefoxCheck.IsChecked = true;
            teamsCheck.IsChecked = true;
            zoomCheck.IsChecked = true;
            mailCheck.IsChecked = true;
            vlcCheck.IsChecked = true;
            onlyofficeCheck.IsChecked = true;
            obsCheck.IsChecked = false;

            status.Content = "Download Start";
            DownloadFunction(ISOfileUrl, downloadFileName);
        }

        // Resoponse Lecture Role button 响应教师角色按钮
        private void Download_Lecture_ISO(object sender, RoutedEventArgs e)
        {
            firefoxCheck.IsChecked = true;
            teamsCheck.IsChecked = true;
            zoomCheck.IsChecked = true;
            obsCheck.IsChecked = true;
            mailCheck.IsChecked = true;
            vlcCheck.IsChecked = true;
            onlyofficeCheck.IsChecked = true;
            string whereWeAre = System.IO.Directory.GetCurrentDirectory();
            string downloadFileName = whereWeAre + @"\tmp.iso";

            DownloadFunction(ISOfileUrl, downloadFileName);
        }


        /// <summary>
        /// 实现下载功能
        /// </summary>
        public void DownloadFunction(String http_url, String save_url)
        {
            if (HttpFileExist(http_url))
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileAsync(new System.Uri(http_url), save_url);
                }
            }
            else
            {
                MessageBox.Show("Download Failed, please try again or send me a email.");
            }
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            status.Content = (e.ProgressPercentage / progressBar.Maximum) * 100 + "%";
        }


        /// <summary>
        ///  检查远程文件是否存在 Check file exist
        /// </summary>
        private bool HttpFileExist(string http_file_url)
        {
            WebResponse response = null;
            bool result = false;
            try
            {
                response = WebRequest.Create(http_file_url).GetResponse();
                result = response == null ? false : true;
            }
            catch (Exception e)
            {
                result = false;
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
        

        // 调用第三方软件烧录方法 Invoke Format and Burning function
        public bool StartProcess(string runFilePath, params string[] args)
        {
            string s = "";
            foreach(string arg in args)
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


        // 开始烧录 Start Burning 
        private void startButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("");
            string rufus_path = System.IO.Directory.GetCurrentDirectory() + @"\rufus.exe";
            string iso_path = System.IO.Directory.GetCurrentDirectory() + @"\tmp.iso";
            string[] the_args = { "-i", iso_path};
            StartProcess(rufus_path, the_args);
        }


        // 退出清理 Exit function
        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }



    }
}
