using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Windows.Controls;

namespace Minesweeper
{
    internal class Timer
    {
        private bool isRunning = false;
        private readonly Stopwatch stopwatch = new();
        private readonly Label labelTimer;

        public Timer(Label labelTimer) 
        { 
            this.labelTimer = labelTimer;
        }

        public int GetTotalMiliseconds()
        {
            return Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
        }

        public static String ConvertMilisecondsToTime(int totalMiliseconds)
        {
            int totalSec = totalMiliseconds / 1000;
            int milisec = totalSec % 1000;
            int sec = totalSec % 60;
            int min = totalSec / 60;

            return $"{min:D2}:{sec:D2}:{milisec:D2}";
        }

        private String GetMinutes()
        {
            String min = stopwatch.Elapsed.Minutes.ToString();
            if (min.Length <= 1)
            {
                min = min.Insert(0, "0");
            }

            return min;
        }

        private String GetSeconds()
        {
            String sec = stopwatch.Elapsed.Seconds.ToString();
            if (sec.Length <= 1)
            {
                sec = sec.Insert(0, "0");
            }

            return sec;
        }

        private String GetMiliseconds()
        {
            String mili = stopwatch.Elapsed.Milliseconds.ToString();
            if (mili.Length <= 1)
            {
                mili = mili.Insert(0, "0");
            }
            else if (mili.Length >= 3)
            {
                mili = mili.Remove(2);
            }

            return mili;
        }

        private String GetActualTime()
        {
            return GetMinutes() + ":" + GetSeconds() + "." + GetMiliseconds();
        }

        public async Task RunAsync()
        {
            while (isRunning)
            {
                labelTimer.Content = GetActualTime();
                await Task.Delay(50);
            }
        }

        public void StartTimer()
        {
            if (isRunning)
                return;

            isRunning = true;
            stopwatch.Start();
            _ = RunAsync();
        }

        public void StopTimer()
        {
            isRunning = false;
            stopwatch.Stop();
        }

        public void RestartTimer()
        {
            stopwatch.Reset();
            isRunning = false;
            labelTimer.Content = "00:00.00";
        }
    }
}