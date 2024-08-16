using System.Windows;
using System.Net.Http;
using System.IO;
using System.Windows.Threading;

namespace DounaiCheckinWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Create a DispatcherTimer instance
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(16);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Code to run at each tick (similar to Unity's Update method)
            Update();
        }

        private void Update()
        {
            B1.Margin = new Thickness((canvas.ActualWidth / 2) - (B1.ActualWidth / 2),
                (canvas.ActualHeight / 2) - (B1.ActualHeight / 2), 0, 0);
        }

        /// <summary>
        /// All the things start from here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginResult = Login();
            var checkinResult = Checkin();

            var result = MessageBox.Show(UnicodeToString(checkinResult) + "，点击“确定”关闭程序。", "Messages From Dounai",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                {
                    Application.Current.Shutdown();
                    break;
                }

                case MessageBoxResult.No:
                {
                    break;
                }
            }
        }

        public IEnumerable<string> LoginCookie;

        private string Login()
        {
            LoginCookie = null;
            
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://dounai.pro/auth/login");

            var request = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress);
            var content = new MultipartFormDataContent
            {
                { new StringContent(GetMail()), "email" },
                { new StringContent(GetPasswd()), "passwd" }
            };
            request.Content = content;
            
            var respond = httpClient.Send(request);

            MessageBox.Show(UnicodeToString("登录的结果：" + respond.Content.ReadAsStringAsync().Result));

            LoginCookie = respond.Headers.GetValues("set-cookie");

            return respond.Content.ReadAsStringAsync().Result;
        }

        private string Checkin()
        {
            var httpClient = new HttpClient();
            
            httpClient.BaseAddress = new Uri("https://dounai.pro/user/checkin");

            var request = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress);

            var str = "";
            foreach (var cookie in LoginCookie)
            {
                str += cookie + ";";
            }
            //MessageBox.Show(str);

            request.Headers.Add("cookie", str);
            
            var content = new StringContent("{}", null, "application/json");
            request.Content = content;


            var respond = httpClient.Send(request);

            return respond.Content.ReadAsStringAsync().Result;
        }
        
        
        
        
        
        
        
        

        static string UnicodeToString(string unicodeString)
        {
            return System.Text.RegularExpressions.Regex.Unescape(unicodeString);
        }

        static string GetMail()
        {
            string result = "";

            if (File.Exists("config.cfg") == false)
            {
                File.Create(Directory.GetCurrentDirectory());
                File.WriteAllText(Directory.GetCurrentDirectory() + "/config.cfg", "123456@abc.com|123456");
            }

            string text = File.ReadAllText("config.cfg");

            result = text.Split("|")[0];

            return result;
        }

        static string GetPasswd()
        {
            string result = "";

            if (File.Exists("config.cfg") == false)
            {
                File.Create(Directory.GetCurrentDirectory());
                File.WriteAllText(Directory.GetCurrentDirectory() + "/config.cfg", "123456@abc.com|123456");
            }

            string text = File.ReadAllText("config.cfg");

            result = text.Split("|")[1];

            return result;
        }
    }
}