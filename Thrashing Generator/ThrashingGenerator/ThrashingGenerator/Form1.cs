using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace ThrashingGenerator
{
    public partial class Form1 : Form
    {
        public static bool bit = true;
        public static int inc = 0;
        public Form1()
        {
            InitializeComponent();
        }

        public async void increase_memory()
        {
            while (bit)
            {
                Process.Start(new ProcessStartInfo("http://youtube.com") { UseShellExecute = true });
                Thread.Sleep(1000);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(increase_memory);
            t.Start();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            bit = false;
            Thread.Sleep(1000);
            Process[] browsers = Process.GetProcessesByName("msedge");
            foreach (Process browser in browsers)
            {
                browser.Kill();
                browser.WaitForExit();
                browser.Dispose();
            }
            bit = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
