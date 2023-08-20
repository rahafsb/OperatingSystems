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

namespace ProcessManagmentApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Display();
        }
        private void Display()
        {
            listView1.Items.Clear();
            Process[] plist = Process.GetProcesses();
            foreach (Process proc in plist)
            {
                
                ListViewItem objec = new ListViewItem(proc.ProcessName +" "+ proc.Id.ToString());
                objec.Tag = proc;
                listView1.Items.Add(objec);
                

            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListViewItem objecc = listView1.SelectedItems[0];
            Process pro = (Process)objecc.Tag;
            pro.Kill();
            Display();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var proc in Process.GetProcessesByName(textBox1.Text))
            {
                proc.Kill();
                
            }
            Display();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int num = (int)numericUpDown1.Value;
            Process del = Process.GetProcessById(num);
            del.Kill();
            Display();

        }

        
       
    }
}
