using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace audition_injector
{
    public class ProcessMonitor
    {
        private Timer timer; // Timer untuk memeriksa proses
        private Stopwatch stopwatch; // Stopwatch untuk melacak durasi Audition berjalan
        private Label lblDuration; // Label untuk menampilkan durasi

        public ProcessMonitor(Label lblDuration)
        {
            this.lblDuration = lblDuration;
            stopwatch = new Stopwatch();
            timer = new Timer();
            timer.Interval = 1000; // Periksa setiap detik
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Memeriksa apakah Audition.exe sedang berjalan
            bool isAuditionRunning = Process.GetProcessesByName("Audition").Length > 0;

            if (isAuditionRunning && !stopwatch.IsRunning)
            {
                stopwatch.Start();
            }
            else if (!isAuditionRunning && stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            lblDuration.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }

        public void StartMonitoring()
        {
            timer.Start();
        }

        public void StopMonitoring()
        {
            timer.Stop();
            stopwatch.Reset();
        }
    }
}
