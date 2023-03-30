using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestLongTask
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool running;
        private event Action<string> messageReported;

        public MainWindow()
        {
            InitializeComponent();
            messageReported += MainWindow_messageReported;
        }

        private void MainWindow_messageReported(string message)
        {
            this.tb_msg.Text = message;
        }

        private async void btn_control_Click(object sender, RoutedEventArgs e)
        {
          
            if( !running )
            {
                StartLongRunningTask();
            }
            else
            {
                _cancellationTokenSource.Cancel();
            }
            running = !running;
            btn_control.Content = running ? "Stop" : "Start";
        }

        private void StartLongRunningTask()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => ReceiveForever(_cancellationTokenSource.Token),
                              _cancellationTokenSource.Token,
                              TaskCreationOptions.LongRunning,
                              TaskScheduler.Default);
        }

        private async Task ReceiveForever( CancellationToken cancellationToken)
        {
            Int64 index = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine( "{1}——Thread {0} invoked!", Thread.CurrentThread.ManagedThreadId , DateTime.Now);
                index++;
                await Dispatcher.BeginInvoke((Action)(() => messageReported.Invoke(string.Format("当前:{0}", index))));
                await Task.Delay(1000);
            }
            Console.WriteLine("----Task Complete---");
        }
    }
}
