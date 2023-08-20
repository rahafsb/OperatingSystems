using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace SpreadsheetApp

{

    public partial class Form1 : Form
    {

        static SharableSpreadSheet sp;

        public Form1()
        {
            InitializeComponent();

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                int row = (int)numericUpDown1.Value;
                int col = (int)numericUpDown2.Value;
                sp = new SharableSpreadSheet(row, col, 2);

                for (int i = 0; i < col; i++)
                {
                    dataGridView1.Columns.Add("newColumnName", "Column Name in Text");
                }

                for (int i = 0; i < row; i++)
                {
                    dataGridView1.Rows.Add();

                }

            }

            else
            {
                int num = dataGridView1.Rows.Count;
                int num2 = dataGridView1.ColumnCount;
                int row = (int)numericUpDown1.Value;
                int col = (int)numericUpDown2.Value;
                sp = new SharableSpreadSheet(row, col);
                for (int i = 0; i < num; i++)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
                }

                for (int i = 0; i < num2; i++)
                {
                    dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);

                }
                for (int i = 0; i < col; i++)
                {
                    dataGridView1.Columns.Add("newColumnName", "Column Name in Text");
                }

                for (int i = 0; i < row; i++)
                {
                    dataGridView1.Rows.Add();
                }

            }

        }



        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {


        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            string s = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            sp.setCell(row, col, s);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {


        }

        private void button2_Click(object sender, EventArgs e)
        {
            int row = (int)numericUpDown3.Value;
            string search = textBox1.Text;

            try
            {
                int num = sp.searchInRow(row, search);
                textBox2.Text = "" + num;

            }

            catch (Exception exp)
            {
                textBox2.Text = "";
                Console.WriteLine(exp.Message);

            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {


        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string save = textBox3.Text + ".txt";
            sp.save(save);
            textBox3.Text = "";
        }



        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }



        private void button4_Click(object sender, EventArgs e)
        {
            String load = @textBox4.Text + ".txt";
            if (File.Exists(load))
            {
                sp.load(load);
                textBox4.Text = "";
                int row = sp.getSize().Item1;
                int col = sp.getSize().Item2;
                int num = dataGridView1.Rows.Count;
                int num2 = dataGridView1.ColumnCount;
                for (int i = 0; i < num; i++)
                {
                    dataGridView1.Rows.RemoveAt(num-i-1);
                }

                for (int i = 0; i < num2; i++)
                {
                    dataGridView1.Columns.RemoveAt(num2-i-1);
                }

                for (int i = 0; i < col; i++)

                {
                    dataGridView1.Columns.Add("newColumnName", "Column Name in Text");
                }

                for (int i = 0; i < row; i++)

                {
                    dataGridView1.Rows.Add();                   
                }
             
                for (int i = 0; i < row; i++)
                {                    
                    for (int j = 0; j < col; j++)
                    {
                        string s = sp.getCell(i, j);
                        //dataGridView1[i, j].Value = s;
                        dataGridView1.Rows[i].Cells[j].Value = s;
                        dataGridView1.EndEdit();

                    }

                }

            }

            else
            {

                textBox4.Text = "";

            }

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string look = textBox5.Text;
            try
            {
                Tuple<int, int> idx = sp.searchString(look);
                if (idx != null)
                {
                    textBox6.Text = idx.ToString();
                }
            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            int row1 = (int)numericUpDown4.Value;
            int row2 = (int)numericUpDown5.Value;
            try {
                sp.exchangeRows(row1, row2);
                int words = sp.getSize().Item2;
                string add = "";
                for (int i = 0; i < words; i++)
                {
                    add = dataGridView1.Rows[row1].Cells[i].Value.ToString();
                    dataGridView1.Rows[row1].Cells[i].Value = dataGridView1.Rows[row2].Cells[i].Value.ToString();
                    dataGridView1.Rows[row2].Cells[i].Value = add;
                }

            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
          }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            int col1 = (int)numericUpDown6.Value;
            int col2 = (int)numericUpDown7.Value;
            try
            {
                sp.exchangeCols(col1, col2);
                int words = sp.getSize().Item1;
                string add = "";
                for (int i = 0; i < words; i++)
                {
                    add = dataGridView1.Rows[i].Cells[col1].Value.ToString();
                    dataGridView1.Rows[i].Cells[col1].Value = dataGridView1.Rows[i].Cells[col2].Value.ToString();
                    dataGridView1.Rows[i].Cells[col2].Value = add;
                }

            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int row = (int)numericUpDown8.Value;
            try
            {
                sp.addRow(row);
                int col = sp.getSize().Item2;
                int rows = sp.getSize().Item1;
                dataGridView1.Rows.Add();
                for(int i = rows-1; i> row; i--)
                {
                    for(int j=0; j< col; j++)
                    {
                        if(i-1 == row)
                        {
                            dataGridView1.Rows[i].Cells[j].Value = "";
                            
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[j].Value = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                        }
                    }
                }



            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                int col = (int)numericUpDown9.Value;
                sp.addCol(col);
                dataGridView1.Columns.Add("newColumnName", "Column Name in Text");
                int rows = sp.getSize().Item1;
                int cols = sp.getSize().Item2;
                for(int i=0; i< rows; i++)
                {
                    for(int j=0; j < cols; j++)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = sp.getCell(i, j); 
                    }
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
          
        }
    }

}