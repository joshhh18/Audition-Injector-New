using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace audition_injector
{
    public static class ModuleManager
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpLibFileName);

        public static IntPtr GetFunctionAddress(string moduleName, string functionName)
        {
            IntPtr hModule = GetModuleHandle(moduleName);
            if (hModule == IntPtr.Zero)
            {
                hModule = LoadLibrary(moduleName);
                if (hModule == IntPtr.Zero)
                {
                    MessageBox.Show($"Failed to load module: {moduleName}. Error Code: {Marshal.GetLastWin32Error()}", "Error");
                    return IntPtr.Zero;
                }
            }

            IntPtr functionAddress = GetProcAddress(hModule, functionName);
            if (functionAddress == IntPtr.Zero)
            {
                MessageBox.Show($"Failed to get address for function: {functionName}. Error Code: {Marshal.GetLastWin32Error()}", "Error");
                return IntPtr.Zero;
            }

            return functionAddress;
        }
    }
}
