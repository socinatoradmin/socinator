#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.Management;

#endregion

namespace DominatorHouseCore.Utility
{
    public interface IPerfCounterService
    {
        PerfCounters GetActualValues();
        string LoadedMemoryDescrption { get; }
    }

    public class PerfCounterService : IPerfCounterService
    {
        private readonly PerformanceCounter _memory;
        private readonly PerformanceCounter _cpuCounter;
        private volatile bool _isInitialized;

        public string LoadedMemoryDescrption { get; }

        public PerfCounterService()
        {
            try
            {
                _memory = new PerformanceCounter("Memory", "Available MBytes");

                var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                _cpuCounter = new PerformanceCounter("Process", "% Processor Time", currentProcess.ProcessName);
                LoadedMemoryDescrption = GetRamsize();

                _isInitialized = true;
            }
            catch (Exception exc)
            {
                exc.DebugLog("fail to initialized performance counters");
                _isInitialized = false;
            }
        }

        public PerfCounters GetActualValues()
        {
            try
            {
                if (_isInitialized)
                {
                    var cpu = GetCpuUsage();
                    var mem = GetMemoryUsage();
                    return new PerfCounters(cpu, mem);
                }
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }

            return new PerfCounters(0, 0);
        }


        private double GetMemoryUsage()
        {
            try
            {
                var memAvailable = (double) _memory.NextValue();
                return memAvailable;
            }
            catch (Exception e)
            {
                e.DebugLog();
                return 0;
            }
        }

        private double GetCpuUsage()
        {
            try
            {
                return Math.Round(_cpuCounter.NextValue() / Environment.ProcessorCount);
            }
            catch (Exception e)
            {
                e.DebugLog("Issue on Getting CpuUsage");
                return 0;
            }
        }

        private string GetRamsize()
        {
            var objManagementClass = new ManagementClass("Win32_ComputerSystem");
            var objManagementObjectCollection = objManagementClass.GetInstances();
            foreach (var item in objManagementObjectCollection)
                return Convert.ToString(
                           Math.Round(Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value) / 1048576, 0),
                           CultureInfo.InvariantCulture) + " MB";

            return "0 MB";
        }
    }

    public sealed class PerfCounters
    {
        public double AvailableMemory { get; }
        public double CpuUsage { get; }

        public PerfCounters(double cpuUsage, double availableMemory)
        {
            AvailableMemory = availableMemory;
            CpuUsage = cpuUsage;
        }
    }
}