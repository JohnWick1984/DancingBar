using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DancingBar
{
    public partial class MainWindow : Window
    {
        private const int NumProgressBars = 5;
        private readonly List<ProgressBar> progressBars = new List<ProgressBar>();
        private CancellationTokenSource cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            InitializeProgressBars();
        }

        private void InitializeProgressBars()
        {
            for (int i = 0; i < NumProgressBars; i++)
            {
                var progressBar = new ProgressBar();
                progressBar.Width = 200;
                progressBar.Margin = new Thickness(10);
                progressBar.VerticalAlignment = VerticalAlignment.Center;
                progressBars.Add(progressBar);
                stackPanel.Children.Add(progressBar);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            cancelButton.IsEnabled = true;

            cancellationTokenSource = new CancellationTokenSource();

            StartProgressBars();
        }

        private void StartProgressBars()
        {
            var random = new Random();

            foreach (var progressBar in progressBars)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    var cancellationToken = cancellationTokenSource.Token;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int value = random.Next(101);
                        Color color = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

                        Dispatcher.Invoke(() =>
                        {
                            progressBar.Value = value;
                            progressBar.Foreground = new SolidColorBrush(color);
                        });

                        Thread.Sleep(50);
                    }
                });
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancelButton.IsEnabled = false;
            startButton.IsEnabled = true; 
        }
    }
}
