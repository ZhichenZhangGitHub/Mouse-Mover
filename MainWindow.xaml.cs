using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace MouseMover
{
    public partial class MainWindow : Window
    {
        private System.Timers.Timer timer;
        private Random random;

        public MainWindow()
        {
            InitializeComponent();
            random = new Random();
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            timer = new System.Timers.Timer(1 * 3 * 1000); // 3min
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 获取当前鼠标位置
            GetCursorPos(out POINT currentPos);

            // 随机生成移动距离（-100到100像素）
            int deltaX = random.Next(-100, 101);
            int deltaY = random.Next(-100, 101);

            // 计算新的鼠标位置
            int targetX = currentPos.X + deltaX;
            int targetY = currentPos.Y + deltaY;
            
            // 随机移动时间
            int moveTime = random.Next(1000, 30001);

            // 平滑移动鼠标
            SmoothMoveMouse(currentPos.X, currentPos.Y, targetX, targetY, moveTime); // 在规定时间内完成移动
        }

        private void SmoothMoveMouse(int startX, int startY, int endX, int endY, int duration)
        {
            int steps = 100;
            int sleepTime = duration / steps;
            double deltaX = (endX - startX) / (double)steps;
            double deltaY = (endY - startY) / (double)steps;

            for (int i = 1; i <= steps; i++)
            {
                int newX = (int)(startX + deltaX * i);
                int newY = (int)(startY + deltaY * i);
                SetCursorPos(newX, newY);
                Thread.Sleep(sleepTime);
            }
        }
    }
}
