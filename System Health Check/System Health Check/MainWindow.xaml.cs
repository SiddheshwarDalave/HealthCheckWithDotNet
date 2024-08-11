using System;
using System.Diagnostics;
using System.Management;
using System.Windows;

namespace SystemHealthCheck
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RefreshHealthData();
        }

        private void RefreshHealthData()
        {
            CpuUsageTextBlock.Text = $"{GetCpuUsage()}%";
            MemoryUsageTextBlock.Text = $"{GetMemoryUsage()}%";
            DiskUsageTextBlock.Text = GetDiskUsage();
        }

        private double GetCpuUsage()
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // First call returns 0
            System.Threading.Thread.Sleep(1000); // Wait for a second to get a valid value
            return cpuCounter.NextValue();
        }

        private double GetMemoryUsage()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                double totalMemory = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                double freeMemory = Convert.ToDouble(obj["FreePhysicalMemory"]);
                double usedMemory = totalMemory - freeMemory;
                return Math.Round((usedMemory / totalMemory) * 100, 2);
            }
            return 0;
        }

        private string GetDiskUsage()
        {
            string diskInfo = "";
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DriveType=3");
            foreach (ManagementObject obj in searcher.Get())
            {
                double totalSize = Convert.ToDouble(obj["Size"]);
                double freeSpace = Convert.ToDouble(obj["FreeSpace"]);
                double usedSpace = totalSize - freeSpace;
                double usedPercentage = Math.Round((usedSpace / totalSize) * 100, 2);
                diskInfo += $"{obj["DeviceID"]}: {usedPercentage}%\n";
            }
            return diskInfo;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshHealthData();
        }
    }
}
