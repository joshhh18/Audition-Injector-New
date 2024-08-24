using System;
using System.Runtime.InteropServices;

namespace audition_injector
{
    public static class MemoryManager
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        public const uint PAGE_EXECUTE_READWRITE = 0x40;

        public static IntPtr AllocateMemory(IntPtr hProcess, uint size)
        {
            const uint MEM_COMMIT = 0x1000;
            const uint MEM_RESERVE = 0x2000;
            return VirtualAllocEx(hProcess, IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
        }

        public static bool WriteMemory(IntPtr hProcess, IntPtr address, byte[] buffer)
        {
            return WriteProcessMemory(hProcess, address, buffer, (uint)buffer.Length, out _);
        }

        public static bool ChangeMemoryProtection(IntPtr hProcess, IntPtr address, uint size, uint newProtect, out uint oldProtect)
        {
            return VirtualProtectEx(hProcess, address, size, newProtect, out oldProtect);
        }
    }
}
