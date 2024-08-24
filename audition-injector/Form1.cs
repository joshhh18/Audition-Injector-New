using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace audition_injector
{
    public partial class Form1 : Form
    {
        private ProcessMonitor processMonitor;
        private DllInjector dllInjector;

        public Form1()
        {
            InitializeComponent();
            processMonitor = new ProcessMonitor(lblDuration); // Initialize ProcessMonitor with lblDuration
            dllInjector = new DllInjector(comboBoxApplications); // Initialize DllInjector with ComboBox reference
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxApplications.Items.Clear(); // Clear existing items

            // Get list of running processes
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle)) // Filter processes with a main window
                {
                    comboBoxApplications.Items.Add(process.ProcessName);
                }
            }

            if (comboBoxApplications.Items.Count > 0)
            {
                comboBoxApplications.SelectedIndex = 0; // Select the first item by default
            }
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Joshhhh.dll");

            if (!File.Exists(dllPath))
            {
                MessageBox.Show("DLL not found in the application directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxApplications.SelectedItem == null)
            {
                MessageBox.Show("Please select an application to inject.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dllInjector.InjectDLL(dllPath); // Pass only the DLL path as the argument
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop monitoring process when form is closed
            processMonitor.StopMonitoring();
        }
    }
}
