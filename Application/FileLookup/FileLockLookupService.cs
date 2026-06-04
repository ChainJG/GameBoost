using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace GameBoost.Application.FileLookup
{
    public static class FileLockLookupService
    {
        private const int RmRebootReasonNone = 0;
        private const int ErrorMoreData = 234;
        private const int CchRmMaxAppName = 255;
        private const int CchRmMaxSvcName = 63;

        public static List<FileLockInfo> GetLockingProcesses(string filePath)
        {
            uint sessionHandle = 0;
            string sessionKey = Guid.NewGuid().ToString("N");

            int result = RmStartSession(out sessionHandle, 0, sessionKey);

            if (result != 0)
                throw new Win32Exception(result, "Failed to start Restart Manager session.");

            try
            {
                string[] resources = { filePath };

                result = RmRegisterResources(
                    sessionHandle,
                    (uint)resources.Length,
                    resources,
                    0,
                    IntPtr.Zero,
                    0,
                    IntPtr.Zero);

                if (result != 0)
                    throw new Win32Exception(result, "Failed to register file resource.");

                uint processInfoNeeded = 0;
                uint processInfoCount = 0;
                uint rebootReasons = RmRebootReasonNone;

                result = RmGetList(
                    sessionHandle,
                    out processInfoNeeded,
                    ref processInfoCount,
                    Array.Empty<RM_PROCESS_INFO>(),
                    ref rebootReasons);

                if (result == 0)
                    return new List<FileLockInfo>();

                if (result != ErrorMoreData)
                    throw new Win32Exception(result, "Failed to get locking process list.");

                var processInfo = new RM_PROCESS_INFO[processInfoNeeded];
                processInfoCount = processInfoNeeded;

                result = RmGetList(
                    sessionHandle,
                    out processInfoNeeded,
                    ref processInfoCount,
                    processInfo,
                    ref rebootReasons);

                if (result != 0)
                    throw new Win32Exception(result, "Failed to get locking process details.");

                var lockingProcesses = new List<FileLockInfo>();

                for (int i = 0; i < processInfoCount; i++)
                {
                    RM_PROCESS_INFO info = processInfo[i];

                    string processName = "";

                    try
                    {
                        processName = Process.GetProcessById(info.Process.dwProcessId).ProcessName;
                    }
                    catch
                    {
                        processName = "Unknown";
                    }

                    lockingProcesses.Add(new FileLockInfo
                    {
                        ProcessId = info.Process.dwProcessId,
                        ProcessName = processName,
                        ApplicationName = info.strAppName,
                        ServiceName = info.strServiceShortName,
                        ApplicationType = info.ApplicationType.ToString(),
                        Restartable = info.bRestartable
                    });
                }

                return lockingProcesses;
            }
            finally
            {
                if (sessionHandle != 0)
                    RmEndSession(sessionHandle);
            }
        }

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        private static extern int RmStartSession(
            out uint pSessionHandle,
            int dwSessionFlags,
            string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        private static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        private static extern int RmRegisterResources(
            uint pSessionHandle,
            uint nFiles,
            string[] rgsFilenames,
            uint nApplications,
            IntPtr rgApplications,
            uint nServices,
            IntPtr rgsServiceNames);

        [DllImport("rstrtmgr.dll")]
        private static extern int RmGetList(
            uint dwSessionHandle,
            out uint pnProcInfoNeeded,
            ref uint pnProcInfo,
            [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
            ref uint lpdwRebootReasons);

        [StructLayout(LayoutKind.Sequential)]
        private struct RM_UNIQUE_PROCESS
        {
            public int dwProcessId;
            public FILETIME ProcessStartTime;
        }

        private enum RM_APP_TYPE
        {
            RmUnknownApp = 0,
            RmMainWindow = 1,
            RmOtherWindow = 2,
            RmService = 3,
            RmExplorer = 4,
            RmConsole = 5,
            RmCritical = 1000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct RM_PROCESS_INFO
        {
            public RM_UNIQUE_PROCESS Process;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CchRmMaxAppName + 1)]
            public string strAppName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CchRmMaxSvcName + 1)]
            public string strServiceShortName;

            public RM_APP_TYPE ApplicationType;
            public uint AppStatus;
            public uint TSSessionId;

            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }
    }
}
