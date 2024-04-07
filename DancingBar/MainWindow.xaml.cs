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

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            cancelButton.IsEnabled = true;

            cancellationTokenSource = new CancellationTokenSource();

            await StartProgressBarsAsync();
            startButton.IsEnabled = true;
        }

        private async Task StartProgressBarsAsync()
        {
            var random = new Random();

            var tasks = new List<Task>();

            foreach (var progressBar in progressBars)
            {
                var cancellationToken = cancellationTokenSource.Token;

                var task = Task.Run(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int value = random.Next(101);
                        Color color = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                        UpdateProgressBar(progressBar, value, color);
                        await Task.Delay(50);
                    }
                }, cancellationToken);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
        private void UpdateProgressBar(ProgressBar progressBar, int value, Color color)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = value;
                progressBar.Foreground = new SolidColorBrush(color);
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancelButton.IsEnabled = false;
        }
    }
}
