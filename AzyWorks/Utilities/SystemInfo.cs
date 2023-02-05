using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Globalization;

using SystemInfoLibrary.Hardware;
using SystemInfoLibrary.Hardware.CPU;
using SystemInfoLibrary.Hardware.GPU;
using SystemInfoLibrary.Hardware.RAM;
using SystemInfoLibrary.OperatingSystem;

namespace AzyWorks.Utilities
{
    public class SystemInfo : DisposableObject
    {
        public const string WinLogOnProcessName = "winlogon.exe";

        private OperatingSystem _sys;
        private OperatingSystemInfo _sysInfo;
        private HardwareInfo _hwInfo;
        private Process _curProcess;

        private WinEmuFlag _winEmuFlag;

        private CPUInfo _cpuInfo;
        private GPUInfo _gpuInfo;
        private RAMInfo _ramInfo;

        public SystemInfo()
        {
            _sys = Environment.OSVersion;
            _sysInfo = OperatingSystemInfo.GetOperatingSystemInfo();
            _curProcess = Process.GetCurrentProcess();

            UpdateData();
            GetEmulationType();
        }

        public long MappedProcessMemory { get => Environment.WorkingSet; }

        public bool Is64BitProcess { get => Environment.Is64BitProcess; }

        public string ProcessName { get => _curProcess.ProcessName; }
        public int ProcessId { get => _curProcess.Id; }
        public int SessionId { get => _curProcess.SessionId; }
        public int ProcessThreadCount { get => _curProcess.Threads.Count; }
        public DateTime ProcessStartTime { get => _curProcess.StartTime; }
        public TimeSpan TotalCpuTime { get => _curProcess.TotalProcessorTime; }
        public TimeSpan UserCpuTime { get => _curProcess.UserProcessorTime; }
        public TimeSpan PriviligedCpuTime { get => _curProcess.PrivilegedProcessorTime; }

        public int ThreadId { get => Environment.CurrentManagedThreadId; }
        public int ThreadContextId { get => Thread.CurrentContext.ContextID; }
        public string ThreadName { get => Thread.CurrentThread.Name; set => Thread.CurrentThread.Name = value; }
        public string ExecutionContext { get => Thread.CurrentThread.ExecutionContext.ToString(); }
        public CultureInfo ThreadCulture { get => Thread.CurrentThread.CurrentCulture; set => Thread.CurrentThread.CurrentCulture = value; }

        public string SystemDirectory { get => Environment.SystemDirectory; }
        public string CurrentDirectory { get => Environment.CurrentDirectory; set => Environment.CurrentDirectory = value; }
        public string NewLine { get => Environment.NewLine; }
        public string CommandLine { get => Environment.CommandLine; }
        public string PlaformId { get => _sys.Platform.ToString(); }
        public string[] CommandLineArgs { get => Environment.GetCommandLineArgs(); }

        public string MachineName { get => Environment.MachineName; }
        public string UserName { get => Environment.UserName; }
        public string UserDomain { get => Environment.UserDomainName; }

        public string OsServicePack { get => _sys.ServicePack; }
        public string OsName { get => _sysInfo.Name; }
        public string OsType { get => _sysInfo.OperatingSystemType.ToString(); }
        public string OsArchitecture { get => _sysInfo.Architecture; }

        public string AppRuntime { get => _sysInfo.Runtime; }
        public System.Version JavaVersion { get => _sysInfo.JavaVersion; }

        public ulong FreeMemoryKB
        {
            get
            {
                UpdateData();

                return _ramInfo.Free;
            }
        }

        public ulong TotalMemoryKB { get => _ramInfo.Total; }

        public string GpuName { get => _gpuInfo.Name; }
        public string GpuVendor { get => _gpuInfo.Brand; }
        public ulong GpuMemoryTotal { get => _gpuInfo.MemoryTotal; }

        public string CpuName { get => _cpuInfo.Name; }
        public string CpuVendor { get => _cpuInfo.Brand; }
        public string CpuArchitecture { get => _cpuInfo.Architecture; }

        public double CpuFrequencyMHz
        {
            get
            {
                UpdateData();

                return _cpuInfo.Frequency;
            }
        }

        public int CpuLogicalCores { get => _cpuInfo.LogicalCores; }
        public int CpuPhysicalCores { get => _cpuInfo.PhysicalCores; }

        public bool IsMono { get => _sysInfo.IsMono; }
        public bool IsLinux { get => _sysInfo.OperatingSystemType == OperatingSystemType.Linux; }
        public bool IsUnity { get => _sysInfo.OperatingSystemType == OperatingSystemType.Unity5; }
        public bool IsBSD { get => _sysInfo.OperatingSystemType == OperatingSystemType.BSD; }
        public bool IsHaiku { get => _sysInfo.OperatingSystemType == OperatingSystemType.Haiku; }
        public bool IsWindows { get => _sysInfo.OperatingSystemType == OperatingSystemType.Windows; }
        public bool IsSolaris { get => _sysInfo.OperatingSystemType == OperatingSystemType.Solaris; }
        public bool IsMacOSX { get => _sysInfo.OperatingSystemType == OperatingSystemType.MacOSX; }
        public bool IsWebAssembly { get => _sysInfo.OperatingSystemType == OperatingSystemType.WebAssembly; }
        public bool IsEmulated { get => _winEmuFlag != WinEmuFlag.None; }
        public bool Is32Bit { get => Bits == 32; }
        public bool Is64Bit { get => Bits == 64; }

        public int Bits { get => IntPtr.Size * 8; }

        public WinEmuFlag EmulationType { get => _winEmuFlag; }

        private void GetEmulationType()
        {
            if (Process.GetProcessesByName(WinLogOnProcessName).Length < 0)
                _winEmuFlag = WinEmuFlag.Wine;
            else
                _winEmuFlag = WinEmuFlag.None;
        }

        private void UpdateData()
        {
            _sysInfo.Update();

            _hwInfo = _sysInfo.Hardware;
            _gpuInfo = _hwInfo.GPUs.First();
            _cpuInfo = _hwInfo.CPUs.First();
            _ramInfo = _hwInfo.RAM;
        }

        public override void Dispose()
        {
            base.Dispose();

            _sys = null;
            _sysInfo = null;
            _hwInfo = null;
            _cpuInfo = null;
            _gpuInfo = null;
            _ramInfo = null;
        }
    }
}