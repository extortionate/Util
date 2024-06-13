using OpenHardwareMonitor.Hardware;
using System;
using System.Linq;
using System.Threading;

namespace Pxrple_RTM_Tool
{
    public class TemperatureMonitor
    {
        private Computer computer;

        public event Action<float, float> TemperatureUpdated;

        public TemperatureMonitor()
        {
            computer = new Computer { CPUEnabled = true, GPUEnabled = true };
            computer.Open();
        }

        public void StartMonitoring()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    UpdateTemperatures();
                    Thread.Sleep(1000); // Adjust the update interval as needed
                }
            });
        }

        private void UpdateTemperatures()
        {
            foreach (var hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                foreach (var sensor in hardwareItem.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature)
                    {
                        if (hardwareItem.HardwareType == HardwareType.CPU)
                        {
                            TemperatureUpdated?.Invoke(sensor.Value ?? 0f, 0f);
                        }
                        else if (hardwareItem.HardwareType == HardwareType.GpuAti || hardwareItem.HardwareType == HardwareType.GpuNvidia)
                        {
                            TemperatureUpdated?.Invoke(0f, sensor.Value ?? 0f);
                        }
                    }
                }
            }
        }
    }
}