using System;
using System.Threading;

namespace Simulator
{
    class Program
    {
        static SharableSpreadSheet sp;
        static void rand()
        {
            Random rd = new Random();
            Random rd1 = new Random();
            Random rd2 = new Random();
            Random rd3 = new Random();
            Random rd4 = new Random();
            int rnd = rd.Next(13);
            int row = rd1.Next(sp.getSize().Item1);
            int col = rd2.Next(sp.getSize().Item2);
            int exRow = rd3.Next(sp.getSize().Item1);
            int exCol = rd4.Next(sp.getSize().Item2);
         //   Thread.Sleep(1000);
            try
            {
                Thread thr = Thread.CurrentThread;
                if (rnd == 0)
                {
                    Console.WriteLine("User[" + thr.Name + "] found " + sp.getCell(row, col) + " in cell (" + row + "," + col + ")");

                }
                if (rnd == 1)
                {

                    sp.setCell(row, col, "changed_" + row + "_" + col);
                    Console.WriteLine("User[" + thr.Name + "] inserted (changed_" + row + col + ") to cell (" + row + "," + col + ")");
                }
                if (rnd == 2)
                {
                    Tuple<int, int> here = sp.searchString("test_cell_" + row + " " + col);
                    Console.WriteLine("User[" + thr.Name + "] searched for test_cell_" + row + " " + col + " found in(" + here.Item1 + "," + here.Item2 + ")");
                }
                if (rnd == 3)
                {
                    sp.exchangeRows(row, exRow);
                    Console.WriteLine("User[" + thr.Name + "] swapped rows" + row + "," + exRow);
                }
                if (rnd == 4)
                {
                    sp.exchangeCols(col, exCol);
                    Console.WriteLine("User[" + thr.Name + "] swapped colmuns" + col + "," + exCol);
                }
                if (rnd == 5)
                {
                    Console.WriteLine("User[" + thr.Name + "] searched for test_cell_" + exRow + "_" + exCol + "in row " + row + " found in " + sp.searchInRow(row, "test_cell_" + exRow + "_" + exCol));
                }
                if (rnd == 6)
                {
                    Console.WriteLine("User[" + thr.Name + "] searched for test_cell_" + exRow + "_" + exCol + " in col " + col + " found in " + sp.searchInCol(col, "test_cell_" + exRow + "_" + exCol));
                }
                if (rnd == 7)
                {
                    Tuple<int, int> herek = sp.searchInRange(col, exCol, row, exRow, "c#");
                    Console.WriteLine("User[" + thr.Name + "] searched in range " + col + "," + exCol + "," + row + "," + exRow + " found in" + herek.ToString());
                }
                if (rnd == 8)
                {
                    
                    sp.addRow(row);
                    Console.WriteLine("User[" + thr.Name + "] added row in the row index " + row);
                }
                if (rnd == 9)
                {
                   
                    sp.addCol(col);
                    Console.WriteLine("User[" + thr.Name + "] added col in the col index " + col);
                }
                if (rnd == 10)
                {
                    /*Tuple<int, int>[] words = sp.findAll("c#", true);
                    Console.WriteLine("User[" + thr.Name + "] founded the string (c#) in the following indexes: ");
                    for (int i = 0; i < words.Length; i++)
                    {
                        Console.WriteLine(words[i].ToString());
                    }*/
                    sp.exchangeRows(row, exRow);
                    Console.WriteLine("User[" + thr.Name + "] swapped rows" + row + "," + exRow);
                }
                if (rnd == 11)
                {
                    sp.setAll("test_cell_" + row + " " + col, "c#", true);
                    Console.WriteLine("User[" + thr.Name + "] changed all test_cell_" + row + " " + col + " to" + " c#");
                }

                if (rnd == 12)
                { 
                    Tuple<int, int> size = sp.getSize();
                    Console.WriteLine("User[" + thr.Name + "] founded that the size of the SpreadSherrt is, rows = " + size.Item1 + ", cols = " + size.Item2);
                }
            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

        }

        static void start_thread(int operations, int sleep)
        {
            for (int i = 0; i < operations; i++)
            {
                Thread.Sleep(sleep);
                rand();
                
            }
        }
        static void Main(string[] args)
        {
            int rows = int.Parse(args[0]);
            int cols = int.Parse(args[1]);
            int threads =int.Parse(args[2]);   // 
            int operations =  int.Parse(args[3]);
            int sleep = int.Parse(args[4]);
            sp = new SharableSpreadSheet(rows, cols, threads);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++) {
                    sp.setCell(i, j, "test_cell_" +i+"_"+j);
                }

            }
            Thread[] array_th = new Thread[threads];
            for(int i=0; i < threads; i++)
            {
                Thread th = new Thread(() => start_thread(operations, sleep));
                th.Name = "" + i;
                //array_th[i] = th;
                th.Start();
               // Thread.Sleep(sleep);
                
            }
           

        }

    }
}