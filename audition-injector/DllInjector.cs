using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace audition_injector
{
    public class DllInjector
    {
        // Reference to the ComboBox for selecting applications
        private ComboBox comboBoxApplications;

        // Constructor to pass the ComboBox reference
        public DllInjector(ComboBox comboBox)
        {
            comboBoxApplications = comboBox;
        }

        public void InjectDLL(string dllPath)
        {
            if (comboBoxApplications.SelectedItem == null)
            {
                MessageBox.Show("Please select an application to inject.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedProcessName = comboBoxApplications.SelectedItem.ToString();
            Process[] processes = Process.GetProcessesByName(selectedProcessName);

            if (processes.Length == 0)
            {
                MessageBox.Show("Selected process is not running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process targetProcess = processes[0];
            int processId = targetProcess.Id;
            IntPtr hProcess = ProcessHandler.OpenTargetProcess(processId);

            if (hProcess == IntPtr.Zero)
            {
                MessageBox.Show("Failed to open process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IntPtr allocatedMemory = MemoryManager.AllocateMemory(hProcess, (uint)dllPath.Length);
            if (allocatedMemory == IntPtr.Zero)
            {
                MessageBox.Show("Failed to allocate memory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] dllPathBytes = System.Text.Encoding.ASCII.GetBytes(dllPath);
            if (!MemoryManager.WriteMemory(hProcess, allocatedMemory, dllPathBytes))
            {
                MessageBox.Show("Failed to write process memory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint oldProtect;
            if (!MemoryManager.ChangeMemoryProtection(hProcess, allocatedMemory, (uint)dllPath.Length, MemoryManager.PAGE_EXECUTE_READWRITE, out oldProtect))
            {
                MessageBox.Show("Failed to change memory protection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IntPtr loadLibraryAddr = ModuleManager.GetFunctionAddress("kernel32.dll", "LoadLibraryA");
            if (loadLibraryAddr == IntPtr.Zero)
            {
                MessageBox.Show("Failed to get address for LoadLibraryA.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IntPtr hThread = ProcessHandler.CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, allocatedMemory, 0, out _);
            if (hThread == IntPtr.Zero)
            {
                MessageBox.Show("Failed to create remote thread.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ProcessHandler.WaitForThread(hThread);
            MemoryManager.ChangeMemoryProtection(hProcess, allocatedMemory, (uint)dllPath.Length, oldProtect, out _);
            MessageBox.Show("DLL Injected successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
