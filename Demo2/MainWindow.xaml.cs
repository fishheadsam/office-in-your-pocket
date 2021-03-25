using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace Demo2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // TODO: Modify download server a public IP 修改下载地址到公网IP
        private readonly string ISOfile = "";
        public int progressPercentage = 0;



        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        // Progress Bar
        public event PropertyChangedEventHandler PropertyChanged;
        private int currentProgress = 0;
        public int CurrentProgress
        {
            get { return currentProgress; }
            set { 
                    currentProgress = value; 
                    OnPropertyChanged("CurrentProgress");
                }
        }
        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        //System.Action growProgress = () =>
        //{
        //    CurrentProgress = progressPercentage;
        //};


        // Response Student Role button 响应学生角色按钮
        private async void Download_Student_ISO(object sender, RoutedEventArgs e)
        {
            firefoxCheck.IsChecked = true;
            teamsCheck.IsChecked = true;
            zoomCheck.IsChecked = true;
            mailCheck.IsChecked = true;
            vlcCheck.IsChecked = true;
            onlyofficeCheck.IsChecked = true;
            obsCheck.IsChecked = false;
            string whereWeAre = System.IO.Directory.GetCurrentDirectory();
            await Task.Run(() => HttpDownload(ISOfile, whereWeAre));
        }

        // Resoponse Lecture Role button 响应教师角色按钮
        private async void Download_Lecture_ISO(object sender, RoutedEventArgs e)
        {
            firefoxCheck.IsChecked = true;
            teamsCheck.IsChecked = true;
            zoomCheck.IsChecked = true;
            obsCheck.IsChecked = true;
            mailCheck.IsChecked = true;
            vlcCheck.IsChecked = true;
            onlyofficeCheck.IsChecked = true;
            string whereWeAre = System.IO.Directory.GetCurrentDirectory();
            await Task.Run(() => HttpDownload(ISOfile, whereWeAre));
        }

        // Get


        // Download Function 下载功能
        public void HttpDownload(string url, string path)
        {
            int runningByteTotal = 0;
            string tempPath = System.IO.Path.GetDirectoryName(path) + @"\temp";
            System.IO.Directory.CreateDirectory(tempPath);
            string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(path) + ".tmp";
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);
            }
            try
            {
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                byte[] vs = new byte[1024];
                int size = stream.Read(vs, 0, (int)vs.Length);
                while (size > 0)
                {
                    fs.Write(vs, 0, size);
                    size = stream.Read(vs, 0, (int)vs.Length);
                    runningByteTotal += size;
                    double index = (double)(runningByteTotal);
                    double total = (double)vs.Length;
                    double rowProgressPercentage = index / total;
                    progressPercentage = (int)(rowProgressPercentage * 100);
                }
                fs.Close();
                stream.Close();
                System.IO.File.Move(tempFile, "demo.iso");
                // TODO: Add MD5 check function 检查下载的镜像
            }
            catch
            {

            }
        }

        // Format and Burning function
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


        // Start Burning 
        private void startButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("");
            string rufus_path = System.IO.Directory.GetCurrentDirectory() + @"\rufus.exe";
            string iso_path = System.IO.Directory.GetCurrentDirectory() + @"\demo.iso";
            string[] the_args = { "-i", iso_path};
            StartProcess(rufus_path, the_args);
        }


        // Exit function
        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }



    }
}
