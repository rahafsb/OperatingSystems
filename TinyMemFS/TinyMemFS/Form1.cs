using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinyMemFS
{
    public partial class Form1 : Form
    {
        static private TinyMemFS mf = new TinyMemFS();
      
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox1.Text)) { return; }
            string Path = @textBox1.Text;
            string name = textBox2.Text;
            bool do_ = mf.add(name, Path);
            if (do_)
            {
                ListViewItem add_ = new ListViewItem(name);
                listView1.Items.Add(add_);
            }
            //listView1.Items.Add(mf.get_content(name));
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }/// try and except every path if it's legitimit!!!!!!!!!

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox6.Text) || string.IsNullOrEmpty(textBox7.Text)) { return; }
            string Path = @textBox6.Text;
            string name = textBox7.Text;
            mf.save(name, Path);
            textBox6.Text = "";
            textBox7.Text = "";
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text)) { return; }
            string name = textBox3.Text;
            bool done = mf.remove(name);
            if (done)
            {
                // Removes the first item in the list.
                /* int at = 0;
                 ListViewItem nemw_ = new ListViewItem(name);
                 at = listView1.Items.IndexOf(nemw_);
                 listView1.Items.RemoveAt(at);*/
                List<String> lst = mf.listFiles();
                string[] add = new string[lst.Count];
                for(int i=0; i < lst.Count; i++)
                {
                    string[] words = lst[i].Split(",");
                    add[i] = words[0];
                }
                listView1.Clear();
                for(int j=0; j< lst.Count; j++)
                {
                    ListViewItem add_ = new ListViewItem(add[j]);
                    listView1.Items.Add(add_);
                }


            }
            textBox3.Text = "";
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text)) { return; }
            string path = @textBox4.Text;
            mf.encrypt(path);
            textBox4.Text = "";
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox5.Text)) { return; }
            string path = @textBox5.Text;
            mf.decrypt(path);
            textBox5.Text = "";
        }

        private void files_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            files.Items.Clear();
            List<String> files_ = mf.listFiles();
            if(files_.Count == 0) { return; }
            for(int i=0; i < files_.Count; i++)
            {
                files.Items.Add(files_[i]);
            }
        }
    }
}
